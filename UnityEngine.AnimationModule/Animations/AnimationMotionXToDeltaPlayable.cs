using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimationMotionXToDeltaPlayable.bindings.h")]
	[StaticAccessor("AnimationMotionXToDeltaPlayableBindings", StaticAccessorType.DoubleColon)]
	[RequiredByNativeCode]
	internal struct AnimationMotionXToDeltaPlayable : IPlayable, IEquatable<AnimationMotionXToDeltaPlayable>
	{
		private AnimationMotionXToDeltaPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOfType<AnimationMotionXToDeltaPlayable>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationMotionXToDeltaPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static AnimationMotionXToDeltaPlayable Null
		{
			get
			{
				return AnimationMotionXToDeltaPlayable.m_NullPlayable;
			}
		}

		public static AnimationMotionXToDeltaPlayable Create(PlayableGraph graph)
		{
			PlayableHandle playableHandle = AnimationMotionXToDeltaPlayable.CreateHandle(graph);
			return new AnimationMotionXToDeltaPlayable(playableHandle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle playableHandle;
			if (!AnimationMotionXToDeltaPlayable.CreateHandleInternal(graph, ref @null))
			{
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				@null.SetInputCount(1);
				playableHandle = @null;
			}
			return playableHandle;
		}

		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator Playable(AnimationMotionXToDeltaPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator AnimationMotionXToDeltaPlayable(Playable playable)
		{
			return new AnimationMotionXToDeltaPlayable(playable.GetHandle());
		}

		public bool Equals(AnimationMotionXToDeltaPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		public bool IsAbsoluteMotion()
		{
			return AnimationMotionXToDeltaPlayable.IsAbsoluteMotionInternal(ref this.m_Handle);
		}

		public void SetAbsoluteMotion(bool value)
		{
			AnimationMotionXToDeltaPlayable.SetAbsoluteMotionInternal(ref this.m_Handle, value);
		}

		[NativeThrows]
		private static bool CreateHandleInternal(PlayableGraph graph, ref PlayableHandle handle)
		{
			return AnimationMotionXToDeltaPlayable.CreateHandleInternal_Injected(ref graph, ref handle);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsAbsoluteMotionInternal(ref PlayableHandle handle);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAbsoluteMotionInternal(ref PlayableHandle handle, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, ref PlayableHandle handle);

		private PlayableHandle m_Handle;

		private static readonly AnimationMotionXToDeltaPlayable m_NullPlayable = new AnimationMotionXToDeltaPlayable(PlayableHandle.Null);
	}
}
