using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Information about clip being played and blended by the Animator.</para>
	/// </summary>
	[NativeHeader("Runtime/Animation/AnimatorInfo.h")]
	[NativeHeader("Runtime/Animation/ScriptBindings/Animation.bindings.h")]
	[UsedByNativeCode]
	public struct AnimatorClipInfo
	{
		/// <summary>
		///   <para>Returns the animation clip played by the Animator.</para>
		/// </summary>
		public AnimationClip clip
		{
			get
			{
				return (this.m_ClipInstanceID == 0) ? null : AnimatorClipInfo.InstanceIDToAnimationClipPPtr(this.m_ClipInstanceID);
			}
		}

		/// <summary>
		///   <para>Returns the blending weight used by the Animator to blend this clip.</para>
		/// </summary>
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
