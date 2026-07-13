using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Animation/ScriptBindings/AnimatorUtility.bindings.h")]
	public class AnimatorUtility
	{
		[FreeFunction("AnimatorUtilityBindings::OptimizeTransformHierarchy")]
		public static void OptimizeTransformHierarchy([NotNull] GameObject go, string[] exposedTransforms)
		{
			if (go == null)
			{
				ThrowHelper.ThrowArgumentNullException(go, "go");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(go);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(go, "go");
			}
			AnimatorUtility.OptimizeTransformHierarchy_Injected(intPtr, exposedTransforms);
		}

		[FreeFunction("AnimatorUtilityBindings::DeoptimizeTransformHierarchy")]
		public static void DeoptimizeTransformHierarchy([NotNull] GameObject go)
		{
			if (go == null)
			{
				ThrowHelper.ThrowArgumentNullException(go, "go");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(go);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(go, "go");
			}
			AnimatorUtility.DeoptimizeTransformHierarchy_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OptimizeTransformHierarchy_Injected(IntPtr go, string[] exposedTransforms);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DeoptimizeTransformHierarchy_Injected(IntPtr go);
	}
}
