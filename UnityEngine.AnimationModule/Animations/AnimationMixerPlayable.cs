using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[NativeHeader("Runtime/Animation/Director/AnimationMixerPlayable.h")]
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimationMixerPlayable.bindings.h")]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[StaticAccessor("AnimationMixerPlayableBindings", StaticAccessorType.DoubleColon)]
	[RequiredByNativeCode]
	public struct AnimationMixerPlayable : IPlayable, IEquatable<AnimationMixerPlayable>
	{
		internal AnimationMixerPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOfType<AnimationMixerPlayable>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationMixerPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static AnimationMixerPlayable Null
		{
			get
			{
				return AnimationMixerPlayable.m_NullPlayable;
			}
		}

		public static AnimationMixerPlayable Create(PlayableGraph graph, int inputCount = 0, bool normalizeWeights = false)
		{
			PlayableHandle playableHandle = AnimationMixerPlayable.CreateHandle(graph, inputCount, normalizeWeights);
			return new AnimationMixerPlayable(playableHandle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, int inputCount = 0, bool normalizeWeights = false)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle playableHandle;
			if (!AnimationMixerPlayable.CreateHandleInternal(graph, inputCount, normalizeWeights, ref @null))
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
		private static bool CreateHandleInternal(PlayableGraph graph, int inputCount, bool normalizeWeights, ref PlayableHandle handle)
		{
			return AnimationMixerPlayable.CreateHandleInternal_Injected(ref graph, inputCount, normalizeWeights, ref handle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, int inputCount, bool normalizeWeights, ref PlayableHandle handle);

		private PlayableHandle m_Handle;

		private static readonly AnimationMixerPlayable m_NullPlayable = new AnimationMixerPlayable(PlayableHandle.Null);
	}
}
