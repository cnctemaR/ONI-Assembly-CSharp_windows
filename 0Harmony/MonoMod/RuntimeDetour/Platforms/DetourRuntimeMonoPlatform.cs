using System;
using System.Reflection;
using System.Reflection.Emit;

namespace MonoMod.RuntimeDetour.Platforms
{
	internal class DetourRuntimeMonoPlatform : DetourRuntimeILPlatform
	{
		public override bool OnMethodCompiledWillBeCalled
		{
			get
			{
				return false;
			}
		}

		public override event OnMethodCompiledEvent OnMethodCompiled;

		protected override RuntimeMethodHandle GetMethodHandle(MethodBase method)
		{
			if (method is DynamicMethod)
			{
				MethodInfo dynamicMethod_CreateDynMethod = DetourRuntimeMonoPlatform._DynamicMethod_CreateDynMethod;
				if (dynamicMethod_CreateDynMethod != null)
				{
					dynamicMethod_CreateDynMethod.Invoke(method, DetourRuntimeMonoPlatform._NoArgs);
				}
				if (DetourRuntimeMonoPlatform._DynamicMethod_mhandle != null)
				{
					return (RuntimeMethodHandle)DetourRuntimeMonoPlatform._DynamicMethod_mhandle.GetValue(method);
				}
			}
			return method.MethodHandle;
		}

		protected unsafe override void DisableInlining(MethodBase method, RuntimeMethodHandle handle)
		{
			ushort* ptr = (long)handle.Value / 2L + 2L;
			ushort* ptr2 = ptr;
			*ptr2 |= 8;
		}

		private static readonly object[] _NoArgs = new object[0];

		private static readonly MethodInfo _DynamicMethod_CreateDynMethod = typeof(DynamicMethod).GetMethod("CreateDynMethod", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly FieldInfo _DynamicMethod_mhandle = typeof(DynamicMethod).GetField("mhandle", BindingFlags.Instance | BindingFlags.NonPublic);
	}
}
