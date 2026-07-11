using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Director/Core/HPlayableGraph.h")]
	[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimationPlayableOutput.bindings.h")]
	[StaticAccessor("AnimationPlayableOutputBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Runtime/Animation/Animator.h")]
	[NativeHeader("Runtime/Animation/Director/AnimationPlayableOutput.h")]
	public struct AnimationPlayableOutput : IPlayableOutput
	{
		internal AnimationPlayableOutput(PlayableOutputHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOutputOfType<AnimationPlayableOutput>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationPlayableOutput.");
				}
			}
			this.m_Handle = handle;
		}

		public static AnimationPlayableOutput Create(PlayableGraph graph, string name, Animator target)
		{
			PlayableOutputHandle playableOutputHandle;
			AnimationPlayableOutput animationPlayableOutput;
			if (!AnimationPlayableGraphExtensions.InternalCreateAnimationOutput(ref graph, name, out playableOutputHandle))
			{
				animationPlayableOutput = AnimationPlayableOutput.Null;
			}
			else
			{
				AnimationPlayableOutput animationPlayableOutput2 = new AnimationPlayableOutput(playableOutputHandle);
				animationPlayableOutput2.SetTarget(target);
				animationPlayableOutput = animationPlayableOutput2;
			}
			return animationPlayableOutput;
		}

		public static AnimationPlayableOutput Null
		{
			get
			{
				return new AnimationPlayableOutput(PlayableOutputHandle.Null);
			}
		}

		public PlayableOutputHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator PlayableOutput(AnimationPlayableOutput output)
		{
			return new PlayableOutput(output.GetHandle());
		}

		public static explicit operator AnimationPlayableOutput(PlayableOutput output)
		{
			return new AnimationPlayableOutput(output.GetHandle());
		}

		public Animator GetTarget()
		{
			return AnimationPlayableOutput.InternalGetTarget(ref this.m_Handle);
		}

		public void SetTarget(Animator value)
		{
			AnimationPlayableOutput.InternalSetTarget(ref this.m_Handle, value);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Animator InternalGetTarget(ref PlayableOutputHandle handle);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetTarget(ref PlayableOutputHandle handle, Animator target);

		private PlayableOutputHandle m_Handle;
	}
}
