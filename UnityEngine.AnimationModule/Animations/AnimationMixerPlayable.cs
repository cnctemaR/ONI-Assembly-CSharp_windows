using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[StaticAccessor("AnimationMixerPlayableBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationMixerPlayable.bindings.h")]
	[NativeHeader("Modules/Animation/Director/AnimationMixerPlayable.h")]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[RequiredByNativeCode]
	public struct AnimationMixerPlayable : IPlayable, IEquatable<AnimationMixerPlayable>
	{
		public static AnimationMixerPlayable Null
		{
			get
			{
				return AnimationMixerPlayable.m_NullPlayable;
			}
		}

		[Obsolete("normalizeWeights is obsolete. It has no effect and will be removed.")]
		public static AnimationMixerPlayable Create(PlayableGraph graph, int inputCount, bool normalizeWeights)
		{
			return AnimationMixerPlayable.Create(graph, inputCount);
		}

		public static AnimationMixerPlayable Create(PlayableGraph graph, int inputCount = 0)
		{
			PlayableHandle playableHandle = AnimationMixerPlayable.CreateHandle(graph, inputCount);
			return new AnimationMixerPlayable(playableHandle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, int inputCount = 0)
		{
			PlayableHandle @null = PlayableHandle.Null;
			bool flag = !AnimationMixerPlayable.CreateHandleInternal(graph, ref @null);
			PlayableHandle playableHandle;
			if (flag)
			{
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				@null.SetInputCount(inputCount);
				playableHandle = @null;
			}
			return playableHandle;
		}

		internal AnimationMixerPlayable(PlayableHandle handle)
		{
			bool flag = handle.IsValid();
			if (flag)
			{
				bool flag2 = !handle.IsPlayableOfType<AnimationMixerPlayable>();
				if (flag2)
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationMixerPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator Playable(AnimationMixerPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator AnimationMixerPlayable(Playable playable)
		{
			return new AnimationMixerPlayable(playable.GetHandle());
		}

		public bool Equals(AnimationMixerPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		[NativeThrows]
		private static bool CreateHandleInternal(PlayableGraph graph, ref PlayableHandle handle)
		{
			return AnimationMixerPlayable.CreateHandleInternal_Injected(ref graph, ref handle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, ref PlayableHandle handle);

		private PlayableHandle m_Handle;

		private static readonly AnimationMixerPlayable m_NullPlayable = new AnimationMixerPlayable(PlayableHandle.Null);
	}
}
