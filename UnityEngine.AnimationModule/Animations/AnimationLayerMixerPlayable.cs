using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[StaticAccessor("AnimationLayerMixerPlayableBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimationLayerMixerPlayable.bindings.h")]
	[NativeHeader("Runtime/Animation/Director/AnimationLayerMixerPlayable.h")]
	[RequiredByNativeCode]
	public struct AnimationLayerMixerPlayable : IPlayable, IEquatable<AnimationLayerMixerPlayable>
	{
		internal AnimationLayerMixerPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOfType<AnimationLayerMixerPlayable>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationLayerMixerPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static AnimationLayerMixerPlayable Null
		{
			get
			{
				return AnimationLayerMixerPlayable.m_NullPlayable;
			}
		}

		public static AnimationLayerMixerPlayable Create(PlayableGraph graph, int inputCount = 0)
		{
			PlayableHandle playableHandle = AnimationLayerMixerPlayable.CreateHandle(graph, inputCount);
			return new AnimationLayerMixerPlayable(playableHandle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, int inputCount = 0)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle playableHandle;
			if (!AnimationLayerMixerPlayable.CreateHandleInternal(graph, ref @null))
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

		public static implicit operator Playable(AnimationLayerMixerPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator AnimationLayerMixerPlayable(Playable playable)
		{
			return new AnimationLayerMixerPlayable(playable.GetHandle());
		}

		public bool Equals(AnimationLayerMixerPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		public bool IsLayerAdditive(uint layerIndex)
		{
			if ((ulong)layerIndex >= (ulong)((long)this.m_Handle.GetInputCount()))
			{
				throw new ArgumentOutOfRangeException("layerIndex", string.Format("layerIndex {0} must be in the range of 0 to {1}.", layerIndex, this.m_Handle.GetInputCount() - 1));
			}
			return AnimationLayerMixerPlayable.IsLayerAdditiveInternal(ref this.m_Handle, layerIndex);
		}

		public void SetLayerAdditive(uint layerIndex, bool value)
		{
			if ((ulong)layerIndex >= (ulong)((long)this.m_Handle.GetInputCount()))
			{
				throw new ArgumentOutOfRangeException("layerIndex", string.Format("layerIndex {0} must be in the range of 0 to {1}.", layerIndex, this.m_Handle.GetInputCount() - 1));
			}
			AnimationLayerMixerPlayable.SetLayerAdditiveInternal(ref this.m_Handle, layerIndex, value);
		}

		public void SetLayerMaskFromAvatarMask(uint layerIndex, AvatarMask mask)
		{
			if ((ulong)layerIndex >= (ulong)((long)this.m_Handle.GetInputCount()))
			{
				throw new ArgumentOutOfRangeException("layerIndex", string.Format("layerIndex {0} must be in the range of 0 to {1}.", layerIndex, this.m_Handle.GetInputCount() - 1));
			}
			if (mask == null)
			{
				throw new ArgumentNullException("mask");
			}
			AnimationLayerMixerPlayable.SetLayerMaskFromAvatarMaskInternal(ref this.m_Handle, layerIndex, mask);
		}

		[NativeThrows]
		private static bool CreateHandleInternal(PlayableGraph graph, ref PlayableHandle handle)
		{
			return AnimationLayerMixerPlayable.CreateHandleInternal_Injected(ref graph, ref handle);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsLayerAdditiveInternal(ref PlayableHandle handle, uint layerIndex);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLayerAdditiveInternal(ref PlayableHandle handle, uint layerIndex, bool value);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLayerMaskFromAvatarMaskInternal(ref PlayableHandle handle, uint layerIndex, AvatarMask mask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, ref PlayableHandle handle);

		private PlayableHandle m_Handle;

		private static readonly AnimationLayerMixerPlayable m_NullPlayable = new AnimationLayerMixerPlayable(PlayableHandle.Null);
	}
}
