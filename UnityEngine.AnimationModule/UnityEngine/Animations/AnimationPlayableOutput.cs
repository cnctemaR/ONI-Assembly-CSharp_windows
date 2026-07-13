using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationPlayableOutput.bindings.h")]
	[NativeHeader("Modules/Animation/Animator.h")]
	[NativeHeader("Runtime/Director/Core/HPlayableGraph.h")]
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
	[NativeHeader("Modules/Animation/Director/AnimationPlayableOutput.h")]
	[StaticAccessor("AnimationPlayableOutputBindings", StaticAccessorType.DoubleColon)]
	public struct AnimationPlayableOutput : IPlayableOutput
	{
		public static AnimationPlayableOutput Create(PlayableGraph graph, string name, Animator target)
		{
			PlayableOutputHandle playableOutputHandle;
			bool flag = !AnimationPlayableGraphExtensions.InternalCreateAnimationOutput(ref graph, name, out playableOutputHandle);
			AnimationPlayableOutput animationPlayableOutput;
			if (flag)
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

		internal AnimationPlayableOutput(PlayableOutputHandle handle)
		{
			bool flag = handle.IsValid();
			if (flag)
			{
				bool flag2 = !handle.IsPlayableOutputOfType<AnimationPlayableOutput>();
				if (flag2)
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationPlayableOutput.");
				}
			}
			this.m_Handle = handle;
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
		private static Animator InternalGetTarget(ref PlayableOutputHandle handle)
		{
			return Unmarshal.UnmarshalUnityObject<Animator>(AnimationPlayableOutput.InternalGetTarget_Injected(ref handle));
		}

		[NativeThrows]
		private static void InternalSetTarget(ref PlayableOutputHandle handle, Animator target)
		{
			AnimationPlayableOutput.InternalSetTarget_Injected(ref handle, Object.MarshalledUnityObject.Marshal<Animator>(target));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalGetTarget_Injected(ref PlayableOutputHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetTarget_Injected(ref PlayableOutputHandle handle, IntPtr target);

		private PlayableOutputHandle m_Handle;
	}
}
