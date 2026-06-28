using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class Animation : Behaviour, IEnumerable
	{
		public extern AnimationClip clip
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool playAutomatically
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern WrapMode wrapMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public void Stop()
		{
			Animation.INTERNAL_CALL_Stop(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Stop(Animation self);

		public void Stop(string name)
		{
			this.Internal_StopByName(name);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_StopByName(string name);

		public void Rewind(string name)
		{
			this.Internal_RewindByName(name);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_RewindByName(string name);

		public void Rewind()
		{
			Animation.INTERNAL_CALL_Rewind(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Rewind(Animation self);

		public void Sample()
		{
			Animation.INTERNAL_CALL_Sample(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Sample(Animation self);

		public extern bool isPlaying
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsPlaying(string name);

		public AnimationState this[string name]
		{
			get
			{
				return this.GetState(name);
			}
		}

		[ExcludeFromDocs]
		public bool Play()
		{
			PlayMode playMode = PlayMode.StopSameLayer;
			return this.Play(playMode);
		}

		public bool Play([DefaultValue("PlayMode.StopSameLayer")] PlayMode mode)
		{
			return this.PlayDefaultAnimation(mode);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Play(string animation, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

		[ExcludeFromDocs]
		public bool Play(string animation)
		{
			PlayMode playMode = PlayMode.StopSameLayer;
			return this.Play(animation, playMode);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CrossFade(string animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

		[ExcludeFromDocs]
		public void CrossFade(string animation, float fadeLength)
		{
			PlayMode playMode = PlayMode.StopSameLayer;
			this.CrossFade(animation, fadeLength, playMode);
		}

		[ExcludeFromDocs]
		public void CrossFade(string animation)
		{
			PlayMode playMode = PlayMode.StopSameLayer;
			float num = 0.3f;
			this.CrossFade(animation, num, playMode);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Blend(string animation, [DefaultValue("1.0F")] float targetWeight, [DefaultValue("0.3F")] float fadeLength);

		[ExcludeFromDocs]
		public void Blend(string animation, float targetWeight)
		{
			float num = 0.3f;
			this.Blend(animation, targetWeight, num);
		}

		[ExcludeFromDocs]
		public void Blend(string animation)
		{
			float num = 0.3f;
			float num2 = 1f;
			this.Blend(animation, num2, num);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimationState CrossFadeQueued(string animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

		[ExcludeFromDocs]
		public AnimationState CrossFadeQueued(string animation, float fadeLength, QueueMode queue)
		{
			PlayMode playMode = PlayMode.StopSameLayer;
			return this.CrossFadeQueued(animation, fadeLength, queue, playMode);
		}

		[ExcludeFromDocs]
		public AnimationState CrossFadeQueued(string animation, float fadeLength)
		{
			PlayMode playMode = PlayMode.StopSameLayer;
			QueueMode queueMode = QueueMode.CompleteOthers;
			return this.CrossFadeQueued(animation, fadeLength, queueMode, playMode);
		}

		[ExcludeFromDocs]
		public AnimationState CrossFadeQueued(string animation)
		{
			PlayMode playMode = PlayMode.StopSameLayer;
			QueueMode queueMode = QueueMode.CompleteOthers;
			float num = 0.3f;
			return this.CrossFadeQueued(animation, num, queueMode, playMode);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimationState PlayQueued(string animation, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

		[ExcludeFromDocs]
		public AnimationState PlayQueued(string animation, QueueMode queue)
		{
			PlayMode playMode = PlayMode.StopSameLayer;
			return this.PlayQueued(animation, queue, playMode);
		}

		[ExcludeFromDocs]
		public AnimationState PlayQueued(string animation)
		{
			PlayMode playMode = PlayMode.StopSameLayer;
			QueueMode queueMode = QueueMode.CompleteOthers;
			return this.PlayQueued(animation, queueMode, playMode);
		}

		public void AddClip(AnimationClip clip, string newName)
		{
			this.AddClip(clip, newName, int.MinValue, int.MaxValue);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame, [DefaultValue("false")] bool addLoopFrame);

		[ExcludeFromDocs]
		public void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame)
		{
			bool flag = false;
			this.AddClip(clip, newName, firstFrame, lastFrame, flag);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveClip(AnimationClip clip);

		public void RemoveClip(string clipName)
		{
			this.RemoveClip2(clipName);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetClipCount();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RemoveClip2(string clipName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool PlayDefaultAnimation(PlayMode mode);

		[Obsolete("use PlayMode instead of AnimationPlayMode.")]
		public bool Play(AnimationPlayMode mode)
		{
			return this.PlayDefaultAnimation((PlayMode)mode);
		}

		[Obsolete("use PlayMode instead of AnimationPlayMode.")]
		public bool Play(string animation, AnimationPlayMode mode)
		{
			return this.Play(animation, (PlayMode)mode);
		}

		public void SyncLayer(int layer)
		{
			Animation.INTERNAL_CALL_SyncLayer(this, layer);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SyncLayer(Animation self, int layer);

		public IEnumerator GetEnumerator()
		{
			return new Animation.Enumerator(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern AnimationState GetState(string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern AnimationState GetStateAtIndex(int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetStateCount();

		public AnimationClip GetClip(string name)
		{
			AnimationState state = this.GetState(name);
			AnimationClip animationClip;
			if (state)
			{
				animationClip = state.clip;
			}
			else
			{
				animationClip = null;
			}
			return animationClip;
		}

		public extern bool animatePhysics
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use cullingType instead")]
		public extern bool animateOnlyIfVisible
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AnimationCullingType cullingType
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Bounds localBounds
		{
			get
			{
				Bounds bounds;
				this.INTERNAL_get_localBounds(out bounds);
				return bounds;
			}
			set
			{
				this.INTERNAL_set_localBounds(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_localBounds(out Bounds value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_localBounds(ref Bounds value);

		private sealed class Enumerator : IEnumerator
		{
			internal Enumerator(Animation outer)
			{
				this.m_Outer = outer;
			}

			public object Current
			{
				get
				{
					return this.m_Outer.GetStateAtIndex(this.m_CurrentIndex);
				}
			}

			public bool MoveNext()
			{
				int stateCount = this.m_Outer.GetStateCount();
				this.m_CurrentIndex++;
				return this.m_CurrentIndex < stateCount;
			}

			public void Reset()
			{
				this.m_CurrentIndex = -1;
			}

			private Animation m_Outer;

			private int m_CurrentIndex = -1;
		}
	}
}
