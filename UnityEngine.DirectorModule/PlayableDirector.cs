using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
	[NativeHeader("Modules/Director/PlayableDirector.h")]
	[RequiredByNativeCode]
	public class PlayableDirector : Behaviour, IExposedPropertyTable
	{
		public PlayState state
		{
			get
			{
				return this.GetPlayState();
			}
		}

		public DirectorWrapMode extrapolationMode
		{
			get
			{
				return this.GetWrapMode();
			}
			set
			{
				this.SetWrapMode(value);
			}
		}

		public PlayableAsset playableAsset
		{
			get
			{
				return this.Internal_GetPlayableAsset() as PlayableAsset;
			}
			set
			{
				this.SetPlayableAsset(value);
			}
		}

		public PlayableGraph playableGraph
		{
			get
			{
				return this.GetGraphHandle();
			}
		}

		public bool playOnAwake
		{
			get
			{
				return this.GetPlayOnAwake();
			}
			set
			{
				this.SetPlayOnAwake(value);
			}
		}

		public void DeferredEvaluate()
		{
			this.EvaluateNextFrame();
		}

		internal void Play(FrameRate frameRate)
		{
			this.PlayOnFrame(frameRate);
		}

		public void Play(PlayableAsset asset)
		{
			bool flag = asset == null;
			if (flag)
			{
				throw new ArgumentNullException("asset");
			}
			this.Play(asset, this.extrapolationMode);
		}

		public void Play(PlayableAsset asset, DirectorWrapMode mode)
		{
			bool flag = asset == null;
			if (flag)
			{
				throw new ArgumentNullException("asset");
			}
			this.playableAsset = asset;
			this.extrapolationMode = mode;
			this.Play();
		}

		public void SetGenericBinding(Object key, Object value)
		{
			this.Internal_SetGenericBinding(key, value);
		}

		public DirectorUpdateMode timeUpdateMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PlayableDirector.get_timeUpdateMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PlayableDirector.set_timeUpdateMode_Injected(intPtr, value);
			}
		}

		public double time
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PlayableDirector.get_time_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PlayableDirector.set_time_Injected(intPtr, value);
			}
		}

		public double initialTime
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PlayableDirector.get_initialTime_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PlayableDirector.set_initialTime_Injected(intPtr, value);
			}
		}

		public double duration
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PlayableDirector.get_duration_Injected(intPtr);
			}
		}

		[NativeThrows]
		public void Evaluate()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.Evaluate_Injected(intPtr);
		}

		[NativeThrows]
		private void PlayOnFrame(FrameRate frameRate)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.PlayOnFrame_Injected(intPtr, ref frameRate);
		}

		[NativeThrows]
		public void Play()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.Play_Injected(intPtr);
		}

		public void Stop()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.Stop_Injected(intPtr);
		}

		public void Pause()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.Pause_Injected(intPtr);
		}

		public void Resume()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.Resume_Injected(intPtr);
		}

		[NativeThrows]
		public void RebuildGraph()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.RebuildGraph_Injected(intPtr);
		}

		public void ClearReferenceValue(PropertyName id)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.ClearReferenceValue_Injected(intPtr, ref id);
		}

		public void SetReferenceValue(PropertyName id, Object value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.SetReferenceValue_Injected(intPtr, ref id, Object.MarshalledUnityObject.Marshal<Object>(value));
		}

		public Object GetReferenceValue(PropertyName id, out bool idValid)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Object>(PlayableDirector.GetReferenceValue_Injected(intPtr, ref id, out idValid));
		}

		[NativeMethod("GetBindingFor")]
		public Object GetGenericBinding(Object key)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Object>(PlayableDirector.GetGenericBinding_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Object>(key)));
		}

		[NativeMethod("ClearBindingFor")]
		public void ClearGenericBinding(Object key)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.ClearGenericBinding_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Object>(key));
		}

		[NativeThrows]
		public void RebindPlayableGraphOutputs()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.RebindPlayableGraphOutputs_Injected(intPtr);
		}

		internal void ProcessPendingGraphChanges()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.ProcessPendingGraphChanges_Injected(intPtr);
		}

		[NativeMethod("HasBinding")]
		internal bool HasGenericBinding(Object key)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return PlayableDirector.HasGenericBinding_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Object>(key));
		}

		private PlayState GetPlayState()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return PlayableDirector.GetPlayState_Injected(intPtr);
		}

		private void SetWrapMode(DirectorWrapMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.SetWrapMode_Injected(intPtr, mode);
		}

		private DirectorWrapMode GetWrapMode()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return PlayableDirector.GetWrapMode_Injected(intPtr);
		}

		[NativeThrows]
		private void EvaluateNextFrame()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.EvaluateNextFrame_Injected(intPtr);
		}

		private PlayableGraph GetGraphHandle()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableGraph playableGraph;
			PlayableDirector.GetGraphHandle_Injected(intPtr, out playableGraph);
			return playableGraph;
		}

		private void SetPlayOnAwake(bool on)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.SetPlayOnAwake_Injected(intPtr, on);
		}

		private bool GetPlayOnAwake()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return PlayableDirector.GetPlayOnAwake_Injected(intPtr);
		}

		[NativeThrows]
		private void Internal_SetGenericBinding(Object key, Object value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.Internal_SetGenericBinding_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Object>(key), Object.MarshalledUnityObject.Marshal<Object>(value));
		}

		private void SetPlayableAsset(ScriptableObject asset)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			PlayableDirector.SetPlayableAsset_Injected(intPtr, Object.MarshalledUnityObject.Marshal<ScriptableObject>(asset));
		}

		private ScriptableObject Internal_GetPlayableAsset()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlayableDirector>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<ScriptableObject>(PlayableDirector.Internal_GetPlayableAsset_Injected(intPtr));
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<PlayableDirector> played;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<PlayableDirector> paused;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<PlayableDirector> stopped;

		[StaticAccessor("GetDirectorManager()", StaticAccessorType.Dot)]
		[NativeHeader("Runtime/Director/Core/DirectorManager.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ResetFrameTiming();

		[RequiredByNativeCode]
		private void SendOnPlayableDirectorPlay()
		{
			bool flag = this.played != null;
			if (flag)
			{
				this.played(this);
			}
		}

		[RequiredByNativeCode]
		private void SendOnPlayableDirectorPause()
		{
			bool flag = this.paused != null;
			if (flag)
			{
				this.paused(this);
			}
		}

		[RequiredByNativeCode]
		private void SendOnPlayableDirectorStop()
		{
			bool flag = this.stopped != null;
			if (flag)
			{
				this.stopped(this);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_timeUpdateMode_Injected(IntPtr _unity_self, DirectorUpdateMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern DirectorUpdateMode get_timeUpdateMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_time_Injected(IntPtr _unity_self, double value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double get_time_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_initialTime_Injected(IntPtr _unity_self, double value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double get_initialTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double get_duration_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Evaluate_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PlayOnFrame_Injected(IntPtr _unity_self, [In] ref FrameRate frameRate);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Play_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Pause_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Resume_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RebuildGraph_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearReferenceValue_Injected(IntPtr _unity_self, [In] ref PropertyName id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetReferenceValue_Injected(IntPtr _unity_self, [In] ref PropertyName id, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetReferenceValue_Injected(IntPtr _unity_self, [In] ref PropertyName id, out bool idValid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetGenericBinding_Injected(IntPtr _unity_self, IntPtr key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearGenericBinding_Injected(IntPtr _unity_self, IntPtr key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RebindPlayableGraphOutputs_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ProcessPendingGraphChanges_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasGenericBinding_Injected(IntPtr _unity_self, IntPtr key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PlayState GetPlayState_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetWrapMode_Injected(IntPtr _unity_self, DirectorWrapMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern DirectorWrapMode GetWrapMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EvaluateNextFrame_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGraphHandle_Injected(IntPtr _unity_self, out PlayableGraph ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPlayOnAwake_Injected(IntPtr _unity_self, bool on);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetPlayOnAwake_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetGenericBinding_Injected(IntPtr _unity_self, IntPtr key, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPlayableAsset_Injected(IntPtr _unity_self, IntPtr asset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_GetPlayableAsset_Injected(IntPtr _unity_self);
	}
}
