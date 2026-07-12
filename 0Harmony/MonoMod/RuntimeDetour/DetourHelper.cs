using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour.Platforms;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour
{
	internal static class DetourHelper
	{
		public static IDetourRuntimePlatform Runtime
		{
			get
			{
				if (DetourHelper._Runtime != null)
				{
					return DetourHelper._Runtime;
				}
				object runtimeLock = DetourHelper._RuntimeLock;
				IDetourRuntimePlatform detourRuntimePlatform;
				lock (runtimeLock)
				{
					if (DetourHelper._Runtime != null)
					{
						detourRuntimePlatform = DetourHelper._Runtime;
					}
					else if (DetourHelper._RuntimeInit)
					{
						detourRuntimePlatform = null;
					}
					else
					{
						DetourHelper._RuntimeInit = true;
						if (ReflectionHelper.IsMono)
						{
							DetourHelper._Runtime = new DetourRuntimeMonoPlatform();
						}
						else if (ReflectionHelper.IsCore)
						{
							DetourHelper._Runtime = DetourRuntimeNETCorePlatform.Create();
						}
						else
						{
							DetourHelper._Runtime = new DetourRuntimeNETPlatform();
						}
						detourRuntimePlatform = DetourHelper._Runtime;
					}
				}
				return detourRuntimePlatform;
			}
			set
			{
				DetourHelper._Runtime = value;
			}
		}

		public static IDetourNativePlatform Native
		{
			get
			{
				if (DetourHelper._Native != null)
				{
					return DetourHelper._Native;
				}
				object nativeLock = DetourHelper._NativeLock;
				IDetourNativePlatform detourNativePlatform;
				lock (nativeLock)
				{
					if (DetourHelper._Native != null)
					{
						detourNativePlatform = DetourHelper._Native;
					}
					else if (DetourHelper._NativeInit)
					{
						detourNativePlatform = null;
					}
					else
					{
						DetourHelper._NativeInit = true;
						IDetourNativePlatform detourNativePlatform2;
						if (PlatformHelper.Is(Platform.ARM))
						{
							detourNativePlatform2 = new DetourNativeARMPlatform();
						}
						else
						{
							detourNativePlatform2 = new DetourNativeX86Platform();
						}
						if (PlatformHelper.Is(Platform.Windows))
						{
							detourNativePlatform = (DetourHelper._Native = new DetourNativeWindowsPlatform(detourNativePlatform2));
						}
						else
						{
							if (ReflectionHelper.IsMono)
							{
								try
								{
									return DetourHelper._Native = new DetourNativeMonoPlatform(detourNativePlatform2, "libmonosgen-2.0." + PlatformHelper.LibrarySuffix);
								}
								catch
								{
								}
							}
							string environmentVariable = Environment.GetEnvironmentVariable("MONOMOD_RUNTIMEDETOUR_MONOPOSIXHELPER");
							if ((ReflectionHelper.IsMono && environmentVariable != "0") || environmentVariable == "1")
							{
								try
								{
									return DetourHelper._Native = new DetourNativeMonoPosixPlatform(detourNativePlatform2);
								}
								catch
								{
								}
							}
							try
							{
								return DetourHelper._Native = new DetourNativeLibcPlatform(detourNativePlatform2);
							}
							catch
							{
							}
							detourNativePlatform = detourNativePlatform2;
						}
					}
				}
				return detourNativePlatform;
			}
			set
			{
				DetourHelper._Native = value;
			}
		}

		public static void MakeWritable(this IDetourNativePlatform plat, NativeDetourData detour)
		{
			plat.MakeWritable(detour.Method, detour.Size);
		}

		public static void MakeExecutable(this IDetourNativePlatform plat, NativeDetourData detour)
		{
			plat.MakeExecutable(detour.Method, detour.Size);
		}

		public static void FlushICache(this IDetourNativePlatform plat, NativeDetourData detour)
		{
			plat.FlushICache(detour.Method, detour.Size);
		}

		public unsafe static void Write(this IntPtr to, ref int offs, byte value)
		{
			*(UIntPtr)((long)to + (long)offs) = value;
			offs++;
		}

		public unsafe static void Write(this IntPtr to, ref int offs, ushort value)
		{
			*(UIntPtr)((long)to + (long)offs) = (short)value;
			offs += 2;
		}

		public unsafe static void Write(this IntPtr to, ref int offs, uint value)
		{
			*(UIntPtr)((long)to + (long)offs) = (int)value;
			offs += 4;
		}

		public unsafe static void Write(this IntPtr to, ref int offs, ulong value)
		{
			*(UIntPtr)((long)to + (long)offs) = (long)value;
			offs += 8;
		}

		public static MethodBase GetIdentifiable(this MethodBase method)
		{
			return DetourHelper.Runtime.GetIdentifiable(method);
		}

		public static IntPtr GetNativeStart(this MethodBase method)
		{
			return DetourHelper.Runtime.GetNativeStart(method);
		}

		public static IntPtr GetNativeStart(this Delegate method)
		{
			return method.Method.GetNativeStart();
		}

		public static IntPtr GetNativeStart(this Expression method)
		{
			return ((MethodCallExpression)method).Method.GetNativeStart();
		}

		public static MethodInfo CreateILCopy(this MethodBase method)
		{
			return DetourHelper.Runtime.CreateCopy(method);
		}

		public static bool TryCreateILCopy(this MethodBase method, out MethodInfo dm)
		{
			return DetourHelper.Runtime.TryCreateCopy(method, out dm);
		}

		public static T Pin<T>(this T method) where T : MethodBase
		{
			DetourHelper.Runtime.Pin(method);
			return method;
		}

		public static T Unpin<T>(this T method) where T : MethodBase
		{
			DetourHelper.Runtime.Unpin(method);
			return method;
		}

		public static MethodInfo GenerateNativeProxy(IntPtr target, MethodBase signature)
		{
			MethodInfo methodInfo = signature as MethodInfo;
			Type type = ((methodInfo != null) ? methodInfo.ReturnType : null) ?? typeof(void);
			ParameterInfo[] parameters = signature.GetParameters();
			Type[] array = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = parameters[i].ParameterType;
			}
			MethodInfo methodInfo2;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("Native<" + ((long)target).ToString("X16", CultureInfo.InvariantCulture) + ">", type, array))
			{
				methodInfo2 = dynamicMethodDefinition.StubCriticalDetour().Generate().Pin<MethodInfo>();
			}
			NativeDetourData nativeDetourData = DetourHelper.Native.Create(methodInfo2.GetNativeStart(), target, null);
			DetourHelper.Native.MakeWritable(nativeDetourData);
			DetourHelper.Native.Apply(nativeDetourData);
			DetourHelper.Native.MakeExecutable(nativeDetourData);
			DetourHelper.Native.FlushICache(nativeDetourData);
			DetourHelper.Native.Free(nativeDetourData);
			return methodInfo2;
		}

		private static NativeDetourData ToNativeDetourData(IntPtr method, IntPtr target, uint size, byte type, IntPtr extra)
		{
			return new NativeDetourData
			{
				Method = method,
				Target = target,
				Size = size,
				Type = type,
				Extra = extra
			};
		}

		public static DynamicMethodDefinition StubCriticalDetour(this DynamicMethodDefinition dm)
		{
			ILProcessor ilprocessor = dm.GetILProcessor();
			ModuleDefinition module = ilprocessor.Body.Method.Module;
			for (int i = 0; i < 32; i++)
			{
				ilprocessor.Emit(OpCodes.Nop);
			}
			ilprocessor.Emit(OpCodes.Ldstr, dm.Definition.Name + " should've been detoured!");
			ilprocessor.Emit(OpCodes.Newobj, module.ImportReference(DetourHelper._ctor_Exception));
			ilprocessor.Emit(OpCodes.Throw);
			return dm;
		}

		public static void EmitDetourCopy(this ILProcessor il, IntPtr src, IntPtr dst, byte type)
		{
			ModuleDefinition module = il.Body.Method.Module;
			il.Emit(OpCodes.Ldsfld, module.ImportReference(DetourHelper._f_Native));
			il.Emit(OpCodes.Ldc_I8, (long)src);
			il.Emit(OpCodes.Conv_I);
			il.Emit(OpCodes.Ldc_I8, (long)dst);
			il.Emit(OpCodes.Conv_I);
			il.Emit(OpCodes.Ldc_I4, (int)type);
			il.Emit(OpCodes.Conv_U1);
			il.Emit(OpCodes.Callvirt, module.ImportReference(DetourHelper._m_Copy));
		}

		public static void EmitDetourApply(this ILProcessor il, NativeDetourData data)
		{
			ModuleDefinition module = il.Body.Method.Module;
			il.Emit(OpCodes.Ldsfld, module.ImportReference(DetourHelper._f_Native));
			il.Emit(OpCodes.Ldc_I8, (long)data.Method);
			il.Emit(OpCodes.Conv_I);
			il.Emit(OpCodes.Ldc_I8, (long)data.Target);
			il.Emit(OpCodes.Conv_I);
			il.Emit(OpCodes.Ldc_I4, (int)data.Size);
			il.Emit(OpCodes.Ldc_I4, (int)data.Type);
			il.Emit(OpCodes.Conv_U1);
			il.Emit(OpCodes.Ldc_I8, (long)data.Extra);
			il.Emit(OpCodes.Conv_I);
			il.Emit(OpCodes.Call, module.ImportReference(DetourHelper._m_ToNativeDetourData));
			il.Emit(OpCodes.Callvirt, module.ImportReference(DetourHelper._m_Apply));
		}

		private static readonly object _RuntimeLock = new object();

		private static bool _RuntimeInit = false;

		private static IDetourRuntimePlatform _Runtime;

		private static readonly object _NativeLock = new object();

		private static bool _NativeInit = false;

		private static IDetourNativePlatform _Native;

		private static readonly FieldInfo _f_Native = typeof(DetourHelper).GetField("_Native", BindingFlags.Static | BindingFlags.NonPublic);

		private static readonly MethodInfo _m_ToNativeDetourData = typeof(DetourHelper).GetMethod("ToNativeDetourData", BindingFlags.Static | BindingFlags.NonPublic);

		private static readonly MethodInfo _m_Copy = typeof(IDetourNativePlatform).GetMethod("Copy");

		private static readonly MethodInfo _m_Apply = typeof(IDetourNativePlatform).GetMethod("Apply");

		private static readonly ConstructorInfo _ctor_Exception = typeof(Exception).GetConstructor(new Type[] { typeof(string) });
	}
}
