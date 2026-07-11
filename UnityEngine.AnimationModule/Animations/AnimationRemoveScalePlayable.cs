using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[StaticAccessor("AnimationRemoveScalePlayableBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[NativeHeader("Runtime/Animation/Director/AnimationRemoveScalePlayable.h")]
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimationRemoveScalePlayable.bindings.h")]
	[RequiredByNativeCode]
	internal struct AnimationRemoveScalePlayable : IPlayable, IEquatable<AnimationRemoveScalePlayable>
	{
		internal AnimationRemoveScalePlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOfType<AnimationRemoveScalePlayable>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationRemoveScalePlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static AnimationRemoveScalePlayable Null
		{
			get
			{
				return AnimationRemoveScalePlayable.m_NullPlayable;
			}
		}

		public static AnimationRemoveScalePlayable Create(PlayableGraph graph, int inputCount)
		{
			PlayableHandle playableHandle = AnimationRemoveScalePlayable.CreateHandle(graph, inputCount);
			return new AnimationRemoveScalePlayable(playableHandle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, int inputCount)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle playableHandle;
			if (!AnimationRemoveScalePlayable.CreateHandleInternal(graph, ref @null))
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

		public static implicit operator Playable(AnimationRemoveScalePlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator AnimationRemoveScalePlayable(Playable playable)
		{
			return new AnimationRemoveScalePlayable(playable.GetHandle());
		}

		public bool Equals(AnimationRemoveScalePlayable other)
		{
			return this.Equals(other.GetHandle());
		}

		[NativeThrows]
		private static bool CreateHandleInternal(PlayableGraph graph, ref PlayableHandle handle)
		{
			return AnimationRemoveScalePlayable.CreateHandleInternal_Injected(ref graph, ref handle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, ref PlayableHandle handle);

		private PlayableHandle m_Handle;

		private static readonly AnimationRemoveScalePlayable m_NullPlayable = new AnimationRemoveScalePlayable(PlayableHandle.Null);
	}
}
