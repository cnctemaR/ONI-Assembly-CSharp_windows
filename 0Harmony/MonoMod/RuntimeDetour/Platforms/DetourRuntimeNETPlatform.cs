using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour.Platforms
{
	public class DetourRuntimeNETPlatform : DetourRuntimeILPlatform
	{
		public override MethodBase GetIdentifiable(MethodBase method)
		{
			if (DetourRuntimeNETPlatform._RTDynamicMethod_m_owner != null && method.GetType() == DetourRuntimeNETPlatform._RTDynamicMethod)
			{
				return (MethodBase)DetourRuntimeNETPlatform._RTDynamicMethod_m_owner.GetValue(method);
			}
			return base.GetIdentifiable(method);
		}

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
				else if (DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod_TakesRuntimeMethodHandleInternal)
				{
					DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod.Invoke(null, new object[] { DetourRuntimeNETPlatform._IRuntimeMethodInfo_get_Value.Invoke(DetourRuntimeNETPlatform._RuntimeMethodHandle_m_value.GetValue((RuntimeMethodHandle)DetourRuntimeNETPlatform._DynamicMethod_GetMethodDescriptor.Invoke(dynamicMethod, DetourRuntimeNETPlatform._NoArgs)), null) });
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
			DetourRuntimeNETPlatform.<>c__DisplayClass14_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			MMDbgLog.Log("mets: " + method.GetID(null, null, true, false, false));
			MMDbgLog.Log(string.Format("meth: 0x{0:X16}", (long)handle.Value));
			MMDbgLog.Log(string.Format("getf: 0x{0:X16}", (long)handle.GetFunctionPointer()));
			bool flag = false;
			IntPtr intPtr;
			for (;;)
			{
				IL_0064:
				if (!method.IsVirtual)
				{
					goto IL_00F5;
				}
				Type declaringType = method.DeclaringType;
				if (declaringType == null || !declaringType.IsValueType)
				{
					goto IL_00F5;
				}
				MMDbgLog.Log(string.Format("ldfn: 0x{0:X16}", (long)method.GetLdftnPointer()));
				bool flag2 = false;
				foreach (Type type in method.DeclaringType.GetInterfaces())
				{
					if (method.DeclaringType.GetInterfaceMap(type).TargetMethods.Contains(method))
					{
						flag2 = true;
						break;
					}
				}
				intPtr = method.GetLdftnPointer();
				if (!flag2)
				{
					break;
				}
				IL_00FE:
				if (PlatformHelper.Is(Platform.ARM))
				{
					if (IntPtr.Size == 4)
					{
						return intPtr;
					}
					int num = 0;
					CS$<>8__locals1.wasPreStub = false;
					IntPtr intPtr2 = this.<GetFunctionPointer>g__WalkPrecode|14_0(intPtr, ref CS$<>8__locals1);
					if (CS$<>8__locals1.wasPreStub)
					{
						this.PrepareMethod(method, handle);
						continue;
					}
					while (intPtr2 != intPtr)
					{
						if (num >= 16)
						{
							break;
						}
						num++;
						intPtr = intPtr2;
						CS$<>8__locals1.wasPreStub = false;
						intPtr2 = this.<GetFunctionPointer>g__WalkPrecode|14_0(intPtr, ref CS$<>8__locals1);
						if (CS$<>8__locals1.wasPreStub)
						{
							this.PrepareMethod(method, handle);
							goto IL_0064;
						}
					}
					return intPtr;
				}
				else if (IntPtr.Size == 4)
				{
					int num2 = (int)intPtr;
					if (*(IntPtr)num2 == 184 && *(IntPtr)(num2 + 5) == 144 && *(IntPtr)(num2 + 6) == 232 && *(IntPtr)(num2 + 11) == 233)
					{
						int num3 = num2 + 11;
						int num4 = *(IntPtr)(num3 + 1) + (num3 + 1 + 4);
						intPtr = this.NotThePreStub(intPtr, (IntPtr)num4, out CS$<>8__locals1.wasPreStub);
						if (CS$<>8__locals1.wasPreStub)
						{
							this.PrepareMethod(method, handle);
							continue;
						}
						MMDbgLog.Log(string.Format("ngen: 0x{0:X8}", (long)intPtr));
					}
					num2 = (int)intPtr;
					if (*(IntPtr)num2 != 233 || *(IntPtr)(num2 + 5) != 95)
					{
						return intPtr;
					}
					int num5 = num2;
					int num6 = *(IntPtr)(num5 + 1) + (num5 + 1 + 4);
					intPtr = this.NotThePreStub(intPtr, (IntPtr)num6, out CS$<>8__locals1.wasPreStub);
					if (CS$<>8__locals1.wasPreStub)
					{
						this.PrepareMethod(method, handle);
						continue;
					}
					goto IL_028E;
				}
				else
				{
					long num7 = (long)intPtr;
					if (*(UIntPtr)num7 == 1959363912U && *(UIntPtr)(num7 + 5L) == 1224837960U && *(UIntPtr)(num7 + 18L) == 1958886217U && *(UIntPtr)(num7 + 23L) == 47176)
					{
						intPtr = this.NotThePreStub(intPtr, (IntPtr)(*(UIntPtr)(num7 + 25L)), out CS$<>8__locals1.wasPreStub);
						if (CS$<>8__locals1.wasPreStub)
						{
							this.PrepareMethod(method, handle);
							continue;
						}
						goto IL_031D;
					}
					else if (*(UIntPtr)num7 == 233 && *(UIntPtr)(num7 + 5L) == 95)
					{
						long num8 = num7;
						long num9 = (long)(*(UIntPtr)(num8 + 1L)) + (num8 + 1L + 4L);
						intPtr = this.NotThePreStub(intPtr, (IntPtr)num9, out CS$<>8__locals1.wasPreStub);
						if (CS$<>8__locals1.wasPreStub)
						{
							this.PrepareMethod(method, handle);
							continue;
						}
						for (int j = 0; j < 16; j++)
						{
							num7 = (long)intPtr + (long)j;
							if (*(UIntPtr)num7 == 47176 && *(UIntPtr)(num7 + 10L) == 57599)
							{
								num9 = *(UIntPtr)(num7 + 2L);
								intPtr = this.NotThePreStub(intPtr, (IntPtr)num9, out CS$<>8__locals1.wasPreStub);
								if (CS$<>8__locals1.wasPreStub)
								{
									this.PrepareMethod(method, handle);
									goto IL_0064;
								}
								j = -1;
							}
							else if ((*(UIntPtr)num7 & 65520) == 47168 && (*(UIntPtr)(num7 + 10L) & 15794175U) == 65382U && *(UIntPtr)(num7 + 13L) == 34063 && (*(UIntPtr)num7 & 15) == (*(UIntPtr)(num7 + 12L) & 15))
							{
								num8 = num7;
								num9 = (long)(*(UIntPtr)(num8 + 13L + 2L)) + (num8 + 13L + 2L + 4L);
								intPtr = this.NotThePreStub(intPtr, (IntPtr)num9, out CS$<>8__locals1.wasPreStub);
								if (CS$<>8__locals1.wasPreStub)
								{
									this.PrepareMethod(method, handle);
									goto IL_0064;
								}
								j = -1;
							}
						}
						goto Block_37;
					}
					else
					{
						if (*(UIntPtr)num7 == 232 && !flag)
						{
							MMDbgLog.Log("Method thunk reset; regenerating");
							flag = true;
							long num10 = (long)(*(UIntPtr)(num7 + 1L)) + (num7 + 1L + 4L);
							MMDbgLog.Log(string.Format("PrecodeFixupThunk: 0x{0:X16}", num10));
							this.PrepareMethod(method, handle);
							continue;
						}
						return intPtr;
					}
				}
				IL_00F5:
				intPtr = base.GetFunctionPointer(method, handle);
				goto IL_00FE;
			}
			return intPtr;
			IL_028E:
			MMDbgLog.Log(string.Format("ngen: 0x{0:X8}", (int)intPtr));
			return intPtr;
			IL_031D:
			MMDbgLog.Log(string.Format("ngen: 0x{0:X16}", (long)intPtr));
			return intPtr;
			Block_37:
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

		private IntPtr NotThePreStub(IntPtr ptrGot, IntPtr ptrParsed, out bool wasPreStub)
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
			wasPreStub = ptrParsed == DetourRuntimeNETPlatform.ThePreStub;
			if (!wasPreStub)
			{
				return ptrParsed;
			}
			return ptrGot;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static DetourRuntimeNETPlatform()
		{
			Type rtdynamicMethod = DetourRuntimeNETPlatform._RTDynamicMethod;
			DetourRuntimeNETPlatform._RTDynamicMethod_m_owner = ((rtdynamicMethod != null) ? rtdynamicMethod.GetField("m_owner", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : null);
			DetourRuntimeNETPlatform._DynamicMethod_m_method = typeof(DynamicMethod).GetField("m_method", BindingFlags.Instance | BindingFlags.NonPublic);
			DetourRuntimeNETPlatform._DynamicMethod_GetMethodDescriptor = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", BindingFlags.Instance | BindingFlags.NonPublic);
			DetourRuntimeNETPlatform._RuntimeMethodHandle_m_value = typeof(RuntimeMethodHandle).GetField("m_value", BindingFlags.Instance | BindingFlags.NonPublic);
			Type type = typeof(RuntimeMethodHandle).Assembly.GetType("System.IRuntimeMethodInfo");
			DetourRuntimeNETPlatform._IRuntimeMethodInfo_get_Value = ((type != null) ? type.GetMethod("get_Value") : null);
			DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod = typeof(RuntimeHelpers).GetMethod("_CompileMethod", BindingFlags.Static | BindingFlags.NonPublic) ?? typeof(RuntimeHelpers).GetMethod("CompileMethod", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo runtimeHelpers__CompileMethod = DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod;
			DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod_TakesIntPtr = ((runtimeHelpers__CompileMethod != null) ? runtimeHelpers__CompileMethod.GetParameters()[0].ParameterType.FullName : null) == "System.IntPtr";
			MethodInfo runtimeHelpers__CompileMethod2 = DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod;
			DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod_TakesIRuntimeMethodInfo = ((runtimeHelpers__CompileMethod2 != null) ? runtimeHelpers__CompileMethod2.GetParameters()[0].ParameterType.FullName : null) == "System.IRuntimeMethodInfo";
			MethodInfo runtimeHelpers__CompileMethod3 = DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod;
			DetourRuntimeNETPlatform._RuntimeHelpers__CompileMethod_TakesRuntimeMethodHandleInternal = ((runtimeHelpers__CompileMethod3 != null) ? runtimeHelpers__CompileMethod3.GetParameters()[0].ParameterType.FullName : null) == "System.RuntimeMethodHandleInternal";
			DetourRuntimeNETPlatform.ThePreStub = IntPtr.Zero;
		}

		[CompilerGenerated]
		private unsafe IntPtr <GetFunctionPointer>g__WalkPrecode|14_0(IntPtr curr, ref DetourRuntimeNETPlatform.<>c__DisplayClass14_0 A_2)
		{
			long num = (long)curr;
			if (*(UIntPtr)num == 268435593U && *(UIntPtr)(num + 4L) == 2839556394U && *(UIntPtr)(num + 8L) == 3592356160U)
			{
				IntPtr intPtr = *(UIntPtr)(num + 16L);
				return this.NotThePreStub(curr, intPtr, out A_2.wasPreStub);
			}
			if (*(UIntPtr)num == 268435595U && *(UIntPtr)(num + 4L) == 2839556458U && *(UIntPtr)(num + 8L) == 3592356160U)
			{
				IntPtr intPtr2 = *(UIntPtr)(num + 16L);
				return this.NotThePreStub(curr, intPtr2, out A_2.wasPreStub);
			}
			if (*(UIntPtr)num == 268435468U && *(UIntPtr)(num + 4L) == 1476395115U && *(UIntPtr)(num + 8L) == 3592356192U)
			{
				IntPtr intPtr3 = *(UIntPtr)(num + 16L);
				return this.NotThePreStub(curr, intPtr3, out A_2.wasPreStub);
			}
			if (*(UIntPtr)num == 2432696336U && *(UIntPtr)(num + 4L) == 2432696352U && *(UIntPtr)(num + 8L) == 2432696833U && *(UIntPtr)(num + 12L) == 1476395120U && *(UIntPtr)(num + 16L) == 3592356352U)
			{
				IntPtr intPtr4 = *(UIntPtr)(num + 24L);
				return this.NotThePreStub(curr, intPtr4, out A_2.wasPreStub);
			}
			return curr;
		}

		private static readonly object[] _NoArgs = new object[0];

		private static readonly Type _RTDynamicMethod = typeof(DynamicMethod).GetNestedType("RTDynamicMethod", BindingFlags.NonPublic);

		private static readonly FieldInfo _RTDynamicMethod_m_owner;

		private static readonly FieldInfo _DynamicMethod_m_method;

		private static readonly MethodInfo _DynamicMethod_GetMethodDescriptor;

		private static readonly FieldInfo _RuntimeMethodHandle_m_value;

		private static readonly MethodInfo _IRuntimeMethodInfo_get_Value;

		private static readonly MethodInfo _RuntimeHelpers__CompileMethod;

		private static readonly bool _RuntimeHelpers__CompileMethod_TakesIntPtr;

		private static readonly bool _RuntimeHelpers__CompileMethod_TakesIRuntimeMethodInfo;

		private static readonly bool _RuntimeHelpers__CompileMethod_TakesRuntimeMethodHandleInternal;

		private static IntPtr ThePreStub;
	}
}
