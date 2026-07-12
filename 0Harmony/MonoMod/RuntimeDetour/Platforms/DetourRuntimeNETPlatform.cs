using System;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour.Platforms
{
	public class DetourRuntimeNETPlatform : DetourRuntimeILPlatform
	{
		protected override RuntimeMethodHandle GetMethodHandle(MethodBase method)
		{
			DynamicMethod dynamicMethod = method as DynamicMethod;
			if (dynamicMethod != null)
			{
				if (DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod_TakesIntPtr)
				{
					DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod.Invoke(null, new object[] { ((RuntimeMethodHandle)DetourRuntimeNETPlatform._DynamicMethod_GetMethodDescriptor.Invoke(dynamicMethod, DetourRuntimeNETPlatform._NoArgs)).Value });
				}
				else if (DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod_TakesIRuntimeMethodInfo)
				{
					DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod.Invoke(null, new object[] { DetourRuntimeNETPlatform._RuntimeMethodHandle_m_value.GetValue((RuntimeMethodHandle)DetourRuntimeNETPlatform._DynamicMethod_GetMethodDescriptor.Invoke(dynamicMethod, DetourRuntimeNETPlatform._NoArgs)) });
				}
				else
				{
					try
					{
						dynamicMethod.CreateDelegate(typeof(MulticastDelegate));
					}
					catch
					{
					}
				}
				if (DetourRuntimeNETPlatform._DynamicMethod_m_method != null)
				{
					return (RuntimeMethodHandle)DetourRuntimeNETPlatform._DynamicMethod_m_method.GetValue(method);
				}
				if (DetourRuntimeNETPlatform._DynamicMethod_GetMethodDescriptor != null)
				{
					return (RuntimeMethodHandle)DetourRuntimeNETPlatform._DynamicMethod_GetMethodDescriptor.Invoke(method, DetourRuntimeNETPlatform._NoArgs);
				}
			}
			return method.MethodHandle;
		}

		protected override void DisableInlining(MethodBase method, RuntimeMethodHandle handle)
		{
		}

		protected unsafe override IntPtr GetFunctionPointer(MethodBase method, RuntimeMethodHandle handle)
		{
			MMDbgLog.Log("mets: " + method.GetID(null, null, true, false, false));
			MMDbgLog.Log(string.Format("meth: 0x{0:X16}", (long)handle.Value));
			MMDbgLog.Log(string.Format("getf: 0x{0:X16}", (long)handle.GetFunctionPointer()));
			if (method.IsVirtual)
			{
				Type declaringType = method.DeclaringType;
				if (declaringType != null && declaringType.IsValueType)
				{
					MMDbgLog.Log(string.Format("ldfn: 0x{0:X16}", (long)method.GetLdftnPointer()));
					return method.GetLdftnPointer();
				}
			}
			bool flag = false;
			IntPtr intPtr;
			long num;
			for (;;)
			{
				intPtr = base.GetFunctionPointer(method, handle);
				if (PlatformHelper.Is(Platform.ARM))
				{
					return intPtr;
				}
				if (IntPtr.Size == 4)
				{
					break;
				}
				num = (long)intPtr;
				if (*(UIntPtr)num == 1959363912U && *(UIntPtr)(num + 5L) == 1224837960U && *(UIntPtr)(num + 18L) == 1958886217U && *(UIntPtr)(num + 23L) == 47176)
				{
					goto Block_15;
				}
				if (*(UIntPtr)num == 233 && *(UIntPtr)(num + 5L) == 95)
				{
					goto Block_17;
				}
				if (*(UIntPtr)num != 232 || flag)
				{
					return intPtr;
				}
				MMDbgLog.Log("Method thunk reset; regenerating");
				flag = true;
				long num2 = (long)(*(UIntPtr)(num + 1L)) + (num + 1L + 4L);
				MMDbgLog.Log(string.Format("PrecodeFixupThunk: 0x{0:X16}", num2));
				this.PrepareMethod(method, handle);
			}
			int num3 = (int)intPtr;
			if (*(IntPtr)num3 == 184 && *(IntPtr)(num3 + 5) == 144 && *(IntPtr)(num3 + 6) == 232 && *(IntPtr)(num3 + 11) == 233)
			{
				int num4 = num3 + 11;
				int num5 = *(IntPtr)(num4 + 1) + (num4 + 1 + 4);
				intPtr = this.NotThePreStub(intPtr, (IntPtr)num5);
				MMDbgLog.Log(string.Format("ngen: 0x{0:X8}", (long)intPtr));
				return intPtr;
			}
			if (*(IntPtr)num3 == 233 && *(IntPtr)(num3 + 5) == 95)
			{
				int num6 = num3;
				int num7 = *(IntPtr)(num6 + 1) + (num6 + 1 + 4);
				intPtr = this.NotThePreStub(intPtr, (IntPtr)num7);
				MMDbgLog.Log(string.Format("ngen: 0x{0:X8}", (int)intPtr));
				return intPtr;
			}
			return intPtr;
			Block_15:
			intPtr = this.NotThePreStub(intPtr, (IntPtr)(*(UIntPtr)(num + 25L)));
			MMDbgLog.Log(string.Format("ngen: 0x{0:X16}", (long)intPtr));
			return intPtr;
			Block_17:
			long num8 = num;
			long num9 = (long)(*(UIntPtr)(num8 + 1L)) + (num8 + 1L + 4L);
			intPtr = this.NotThePreStub(intPtr, (IntPtr)num9);
			MMDbgLog.Log(string.Format("ngen: 0x{0:X16}", (long)intPtr));
			return intPtr;
		}

		public override bool OnMethodCompiledWillBeCalled
		{
			get
			{
				return false;
			}
		}

		public override event OnMethodCompiledEvent OnMethodCompiled;

		private IntPtr NotThePreStub(IntPtr ptrGot, IntPtr ptrParsed)
		{
			if (DetourRuntimeNETPlatform.ThePreStub == IntPtr.Zero)
			{
				DetourRuntimeNETPlatform.ThePreStub = (IntPtr)(-2);
				Type type = typeof(HttpWebRequest).Assembly.GetType("System.Net.Connection");
				MethodInfo methodInfo = ((type != null) ? type.GetMethod("SubmitRequest", BindingFlags.Instance | BindingFlags.NonPublic) : null);
				if (methodInfo != null)
				{
					DetourRuntimeNETPlatform.ThePreStub = this.GetNativeStart(methodInfo);
					MMDbgLog.Log(string.Format("ThePreStub: 0x{0:X16}", (long)DetourRuntimeNETPlatform.ThePreStub));
				}
				else if (PlatformHelper.Is(Platform.Windows))
				{
					DetourRuntimeNETPlatform.ThePreStub = (IntPtr)(-1);
				}
			}
			if (!(ptrParsed == DetourRuntimeNETPlatform.ThePreStub))
			{
				return ptrParsed;
			}
			return ptrGot;
		}

		private static readonly object[] _NoArgs = new object[0];

		private static readonly FieldInfo _DynamicMethod_m_method = typeof(DynamicMethod).GetField("m_method", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly MethodInfo _DynamicMethod_GetMethodDescriptor = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly FieldInfo _RuntimeMethodHandle_m_value = typeof(RuntimeMethodHandle).GetField("m_value", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly MethodInfo _RuntimeHelpers__CompileMethod = typeof(RuntimeHelpers).GetMethod("_CompileMethod", BindingFlags.Static | BindingFlags.NonPublic);

		private static readonly bool _RuntimeHelpers__CompileMethod_TakesIntPtr = DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod != null && DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod.GetParameters()[0].ParameterType.FullName == "System.IntPtr";

		private static readonly bool _RuntimeHelpers__CompileMethod_TakesIRuntimeMethodInfo = DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod != null && DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod.GetParameters()[0].ParameterType.FullName == "System.IRuntimeMethodInfo";

		private static IntPtr ThePreStub = IntPtr.Zero;
	}
}
