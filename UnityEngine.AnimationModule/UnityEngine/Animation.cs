using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Animation/Animation.h")]
	public sealed class Animation : Behaviour, IEnumerable
	{
		public AnimationClip clip
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<AnimationClip>(Animation.get_clip_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animation.set_clip_Injected(intPtr, Object.MarshalledUnityObject.Marshal<AnimationClip>(value));
			}
		}

		public bool playAutomatically
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animation.get_playAutomatically_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animation.set_playAutomatically_Injected(intPtr, value);
			}
		}

		public WrapMode wrapMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animation.get_wrapMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animation.set_wrapMode_Injected(intPtr, value);
			}
		}

		public void Stop()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animation.Stop_Injected(intPtr);
		}

		public void Stop(string name)
		{
			this.StopNamed(name);
		}

		[NativeName("Stop")]
		private unsafe void StopNamed(string name)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Animation.StopNamed_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public void Rewind()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animation.Rewind_Injected(intPtr);
		}

		public void Rewind(string name)
		{
			this.RewindNamed(name);
		}

		[NativeName("Rewind")]
		private unsafe void RewindNamed(string name)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Animation.RewindNamed_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public void Sample()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animation.Sample_Injected(intPtr);
		}

		public bool isPlaying
		{
			[NativeName("IsPlaying")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animation.get_isPlaying_Injected(intPtr);
			}
		}

		public unsafe bool IsPlaying(string name)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = Animation.IsPlaying_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

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
			return this.Play(PlayMode.StopSameLayer);
		}

		public bool Play([DefaultValue("PlayMode.StopSameLayer")] PlayMode mode)
		{
			return this.PlayDefaultAnimation(mode);
		}

		[NativeName("Play")]
		private bool PlayDefaultAnimation(PlayMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animation.PlayDefaultAnimation_Injected(intPtr, mode);
		}

		[ExcludeFromDocs]
		public bool Play(string animation)
		{
			return this.Play(animation, PlayMode.StopSameLayer);
		}

		public unsafe bool Play(string animation, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(animation, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = animation.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = Animation.Play_Injected(intPtr, ref managedSpanWrapper, mode);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[ExcludeFromDocs]
		public void CrossFade(string animation)
		{
			this.CrossFade(animation, 0.3f);
		}

		[ExcludeFromDocs]
		public void CrossFade(string animation, float fadeLength)
		{
			this.CrossFade(animation, fadeLength, PlayMode.StopSameLayer);
		}

		public unsafe void CrossFade(string animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(animation, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = animation.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Animation.CrossFade_Injected(intPtr, ref managedSpanWrapper, fadeLength, mode);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[ExcludeFromDocs]
		public void Blend(string animation)
		{
			this.Blend(animation, 1f);
		}

		[ExcludeFromDocs]
		public void Blend(string animation, float targetWeight)
		{
			this.Blend(animation, targetWeight, 0.3f);
		}

		public unsafe void Blend(string animation, [DefaultValue("1.0F")] float targetWeight, [DefaultValue("0.3F")] float fadeLength)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(animation, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = animation.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Animation.Blend_Injected(intPtr, ref managedSpanWrapper, targetWeight, fadeLength);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[ExcludeFromDocs]
		public AnimationState CrossFadeQueued(string animation)
		{
			return this.CrossFadeQueued(animation, 0.3f);
		}

		[ExcludeFromDocs]
		public AnimationState CrossFadeQueued(string animation, float fadeLength)
		{
			return this.CrossFadeQueued(animation, fadeLength, QueueMode.CompleteOthers);
		}

		[ExcludeFromDocs]
		public AnimationState CrossFadeQueued(string animation, float fadeLength, QueueMode queue)
		{
			return this.CrossFadeQueued(animation, fadeLength, queue, PlayMode.StopSameLayer);
		}

		[FreeFunction("AnimationBindings::CrossFadeQueuedImpl", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public unsafe AnimationState CrossFadeQueued(string animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode)
		{
			AnimationState animationState;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(animation, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = animation.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				animationState = Animation.CrossFadeQueued_Injected(intPtr, ref managedSpanWrapper, fadeLength, queue, mode);
			}
			finally
			{
				char* ptr = null;
			}
			return animationState;
		}

		[ExcludeFromDocs]
		public AnimationState PlayQueued(string animation)
		{
			return this.PlayQueued(animation, QueueMode.CompleteOthers);
		}

		[ExcludeFromDocs]
		public AnimationState PlayQueued(string animation, QueueMode queue)
		{
			return this.PlayQueued(animation, queue, PlayMode.StopSameLayer);
		}

		[FreeFunction("AnimationBindings::PlayQueuedImpl", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public unsafe AnimationState PlayQueued(string animation, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode)
		{
			AnimationState animationState;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(animation, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = animation.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				animationState = Animation.PlayQueued_Injected(intPtr, ref managedSpanWrapper, queue, mode);
			}
			finally
			{
				char* ptr = null;
			}
			return animationState;
		}

		public void AddClip(AnimationClip clip, string newName)
		{
			this.AddClip(clip, newName, int.MinValue, int.MaxValue);
		}

		[ExcludeFromDocs]
		public void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame)
		{
			this.AddClip(clip, newName, firstFrame, lastFrame, false);
		}

		public unsafe void AddClip([NotNull] AnimationClip clip, string newName, int firstFrame, int lastFrame, [DefaultValue("false")] bool addLoopFrame)
		{
			if (clip == null)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(clip);
				if (intPtr2 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(clip, "clip");
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(newName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = newName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Animation.AddClip_Injected(intPtr, intPtr2, ref managedSpanWrapper, firstFrame, lastFrame, addLoopFrame);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public void RemoveClip([NotNull] AnimationClip clip)
		{
			if (clip == null)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(clip);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			Animation.RemoveClip_Injected(intPtr, intPtr2);
		}

		public void RemoveClip(string clipName)
		{
			this.RemoveClipNamed(clipName);
		}

		[NativeName("RemoveClip")]
		private unsafe void RemoveClipNamed(string clipName)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(clipName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = clipName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Animation.RemoveClipNamed_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public int GetClipCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animation.GetClipCount_Injected(intPtr);
		}

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
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Animation.SyncLayer_Injected(intPtr, layer);
		}

		public IEnumerator GetEnumerator()
		{
			return new Animation.Enumerator(this);
		}

		[FreeFunction("AnimationBindings::GetState", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		internal unsafe AnimationState GetState(string name)
		{
			AnimationState state_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				state_Injected = Animation.GetState_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return state_Injected;
		}

		[FreeFunction("AnimationBindings::GetStateAtIndex", HasExplicitThis = true, ThrowsException = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		internal AnimationState GetStateAtIndex(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animation.GetStateAtIndex_Injected(intPtr, index);
		}

		[NativeName("GetAnimationStateCount")]
		internal int GetStateCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Animation.GetStateCount_Injected(intPtr);
		}

		public AnimationClip GetClip(string name)
		{
			AnimationState state = this.GetState(name);
			bool flag = state;
			AnimationClip animationClip;
			if (flag)
			{
				animationClip = state.clip;
			}
			else
			{
				animationClip = null;
			}
			return animationClip;
		}

		public bool animatePhysics
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animation.get_animatePhysics_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animation.set_animatePhysics_Injected(intPtr, value);
			}
		}

		public AnimationUpdateMode updateMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animation.get_updateMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animation.set_updateMode_Injected(intPtr, value);
			}
		}

		[Obsolete("Use cullingType instead")]
		public bool animateOnlyIfVisible
		{
			[FreeFunction("AnimationBindings::GetAnimateOnlyIfVisible", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animation.get_animateOnlyIfVisible_Injected(intPtr);
			}
			[FreeFunction("AnimationBindings::SetAnimateOnlyIfVisible", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animation.set_animateOnlyIfVisible_Injected(intPtr, value);
			}
		}

		public AnimationCullingType cullingType
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Animation.get_cullingType_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animation.set_cullingType_Injected(intPtr, value);
			}
		}

		public Bounds localBounds
		{
			[NativeName("GetLocalAABB")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds bounds;
				Animation.get_localBounds_Injected(intPtr, out bounds);
				return bounds;
			}
			[NativeName("SetLocalAABB")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animation>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Animation.set_localBounds_Injected(intPtr, ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_clip_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_clip_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_playAutomatically_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_playAutomatically_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern WrapMode get_wrapMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wrapMode_Injected(IntPtr _unity_self, WrapMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopNamed_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Rewind_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RewindNamed_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Sample_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isPlaying_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsPlaying_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool PlayDefaultAnimation_Injected(IntPtr _unity_self, PlayMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Play_Injected(IntPtr _unity_self, ref ManagedSpanWrapper animation, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CrossFade_Injected(IntPtr _unity_self, ref ManagedSpanWrapper animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Blend_Injected(IntPtr _unity_self, ref ManagedSpanWrapper animation, [DefaultValue("1.0F")] float targetWeight, [DefaultValue("0.3F")] float fadeLength);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationState CrossFadeQueued_Injected(IntPtr _unity_self, ref ManagedSpanWrapper animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationState PlayQueued_Injected(IntPtr _unity_self, ref ManagedSpanWrapper animation, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddClip_Injected(IntPtr _unity_self, IntPtr clip, ref ManagedSpanWrapper newName, int firstFrame, int lastFrame, [DefaultValue("false")] bool addLoopFrame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveClip_Injected(IntPtr _unity_self, IntPtr clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveClipNamed_Injected(IntPtr _unity_self, ref ManagedSpanWrapper clipName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetClipCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SyncLayer_Injected(IntPtr _unity_self, int layer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationState GetState_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationState GetStateAtIndex_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetStateCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_animatePhysics_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_animatePhysics_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationUpdateMode get_updateMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_updateMode_Injected(IntPtr _unity_self, AnimationUpdateMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_animateOnlyIfVisible_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_animateOnlyIfVisible_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationCullingType get_cullingType_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cullingType_Injected(IntPtr _unity_self, AnimationCullingType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_localBounds_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_localBounds_Injected(IntPtr _unity_self, [In] ref Bounds value);

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
