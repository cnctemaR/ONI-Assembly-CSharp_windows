using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Animation/AnimatorInfo.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Animation/ScriptBindings/Animation.bindings.h")]
	public struct AnimatorClipInfo
	{
		public AnimationClip clip
		{
			get
			{
				return (this.m_ClipInstanceID == 0) ? null : AnimatorClipInfo.InstanceIDToAnimationClipPPtr(this.m_ClipInstanceID);
			}
		}

		public float weight
		{
			get
			{
				return this.m_Weight;
			}
		}

		[FreeFunction("AnimationBindings::InstanceIDToAnimationClipPPtr")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationClip InstanceIDToAnimationClipPPtr(int instanceID);

		private int m_ClipInstanceID;

		private float m_Weight;
	}
}
