using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/Animation/RuntimeAnimatorController.h")]
	[ExcludeFromObjectFactory]
	[UsedByNativeCode]
	public class RuntimeAnimatorController : Object
	{
		protected RuntimeAnimatorController()
		{
		}

		public AnimationClip[] animationClips
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RuntimeAnimatorController>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RuntimeAnimatorController.get_animationClips_Injected(intPtr);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationClip[] get_animationClips_Injected(IntPtr _unity_self);
	}
}
