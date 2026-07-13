using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/Animation/AnimatorInfo.h")]
	[UsedByNativeCode]
	[NativeHeader("Modules/Animation/ScriptBindings/Animation.bindings.h")]
	public struct AnimatorClipInfo
	{
		public AnimationClip clip
		{
			get
			{
				return (this.m_ClipInstanceID != 0) ? AnimatorClipInfo.InstanceIDToAnimationClipPPtr(this.m_ClipInstanceID) : null;
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
		private static AnimationClip InstanceIDToAnimationClipPPtr(EntityId entityId)
		{
			return Unmarshal.UnmarshalUnityObject<AnimationClip>(AnimatorClipInfo.InstanceIDToAnimationClipPPtr_Injected(ref entityId));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InstanceIDToAnimationClipPPtr_Injected([In] ref EntityId entityId);

		private int m_ClipInstanceID;

		private float m_Weight;
	}
}
