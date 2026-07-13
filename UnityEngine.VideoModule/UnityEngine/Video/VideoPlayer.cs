using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Video
{
	[NativeHeader("Modules/Video/Public/VideoPlayer.h")]
	[RequireComponent(typeof(Transform))]
	[RequiredByNativeCode]
	public sealed class VideoPlayer : Behaviour
	{
		public VideoSource source
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_source_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_source_Injected(intPtr, value);
			}
		}

		public VideoTimeUpdateMode timeUpdateMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_timeUpdateMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_timeUpdateMode_Injected(intPtr, value);
			}
		}

		[NativeName("VideoUrl")]
		public unsafe string url
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					VideoPlayer.get_url_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			set
			{
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					VideoPlayer.set_url_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		[NativeName("VideoClip")]
		public VideoClip clip
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<VideoClip>(VideoPlayer.get_clip_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_clip_Injected(intPtr, Object.MarshalledUnityObject.Marshal<VideoClip>(value));
			}
		}

		public VideoRenderMode renderMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_renderMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_renderMode_Injected(intPtr, value);
			}
		}

		public bool canSetTimeUpdateMode
		{
			[NativeName("CanSetTimeUpdateMode")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_canSetTimeUpdateMode_Injected(intPtr);
			}
		}

		[NativeHeader("Runtime/Camera/Camera.h")]
		public Camera targetCamera
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Camera>(VideoPlayer.get_targetCamera_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_targetCamera_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Camera>(value));
			}
		}

		[NativeHeader("Runtime/Graphics/RenderTexture.h")]
		public RenderTexture targetTexture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<RenderTexture>(VideoPlayer.get_targetTexture_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_targetTexture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RenderTexture>(value));
			}
		}

		[NativeHeader("Runtime/Graphics/Renderer.h")]
		public Renderer targetMaterialRenderer
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Renderer>(VideoPlayer.get_targetMaterialRenderer_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_targetMaterialRenderer_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Renderer>(value));
			}
		}

		public unsafe string targetMaterialProperty
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					VideoPlayer.get_targetMaterialProperty_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			set
			{
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					VideoPlayer.set_targetMaterialProperty_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		public VideoAspectRatio aspectRatio
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_aspectRatio_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_aspectRatio_Injected(intPtr, value);
			}
		}

		public float targetCameraAlpha
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_targetCameraAlpha_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_targetCameraAlpha_Injected(intPtr, value);
			}
		}

		public Video3DLayout targetCamera3DLayout
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_targetCamera3DLayout_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_targetCamera3DLayout_Injected(intPtr, value);
			}
		}

		[NativeHeader("Runtime/Graphics/Texture.h")]
		public Texture texture
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Texture>(VideoPlayer.get_texture_Injected(intPtr));
			}
		}

		public void Prepare()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayer.Prepare_Injected(intPtr);
		}

		public bool isPrepared
		{
			[NativeName("IsPrepared")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_isPrepared_Injected(intPtr);
			}
		}

		public bool waitForFirstFrame
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_waitForFirstFrame_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_waitForFirstFrame_Injected(intPtr, value);
			}
		}

		public bool playOnAwake
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_playOnAwake_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_playOnAwake_Injected(intPtr, value);
			}
		}

		public void Play()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayer.Play_Injected(intPtr);
		}

		public void Pause()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayer.Pause_Injected(intPtr);
		}

		public void Stop()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayer.Stop_Injected(intPtr);
		}

		public bool isPlaying
		{
			[NativeName("IsPlaying")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_isPlaying_Injected(intPtr);
			}
		}

		public bool isPaused
		{
			[NativeName("IsPaused")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_isPaused_Injected(intPtr);
			}
		}

		public bool canSetTime
		{
			[NativeName("CanSetTime")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_canSetTime_Injected(intPtr);
			}
		}

		[NativeName("SecPosition")]
		public double time
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_time_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_time_Injected(intPtr, value);
			}
		}

		[NativeName("FramePosition")]
		public long frame
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_frame_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_frame_Injected(intPtr, value);
			}
		}

		public double clockTime
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_clockTime_Injected(intPtr);
			}
		}

		public bool canStep
		{
			[NativeName("CanStep")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_canStep_Injected(intPtr);
			}
		}

		public void StepForward()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayer.StepForward_Injected(intPtr);
		}

		public bool canSetPlaybackSpeed
		{
			[NativeName("CanSetPlaybackSpeed")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_canSetPlaybackSpeed_Injected(intPtr);
			}
		}

		public float playbackSpeed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_playbackSpeed_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_playbackSpeed_Injected(intPtr, value);
			}
		}

		[NativeName("Loop")]
		public bool isLooping
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_isLooping_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_isLooping_Injected(intPtr, value);
			}
		}

		[Obsolete("VideoPlayer.canSetTimeSource is deprecated. Use canSetTimeUpdateMode instead. (UnityUpgradable) -> canSetTimeUpdateMode")]
		public bool canSetTimeSource
		{
			[NativeName("CanSetTimeSource")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_canSetTimeSource_Injected(intPtr);
			}
		}

		[Obsolete("VideoPlayer.timeSource is deprecated. Use timeUpdateMode instead. (UnityUpgradable) -> timeUpdateMode")]
		public VideoTimeSource timeSource
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_timeSource_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_timeSource_Injected(intPtr, value);
			}
		}

		public VideoTimeReference timeReference
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_timeReference_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_timeReference_Injected(intPtr, value);
			}
		}

		public double externalReferenceTime
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_externalReferenceTime_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_externalReferenceTime_Injected(intPtr, value);
			}
		}

		public bool canSetSkipOnDrop
		{
			[NativeName("CanSetSkipOnDrop")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_canSetSkipOnDrop_Injected(intPtr);
			}
		}

		public bool skipOnDrop
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_skipOnDrop_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_skipOnDrop_Injected(intPtr, value);
			}
		}

		public ulong frameCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_frameCount_Injected(intPtr);
			}
		}

		public float frameRate
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_frameRate_Injected(intPtr);
			}
		}

		[NativeName("Duration")]
		public double length
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_length_Injected(intPtr);
			}
		}

		public uint width
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_width_Injected(intPtr);
			}
		}

		public uint height
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_height_Injected(intPtr);
			}
		}

		public uint pixelAspectRatioNumerator
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_pixelAspectRatioNumerator_Injected(intPtr);
			}
		}

		public uint pixelAspectRatioDenominator
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_pixelAspectRatioDenominator_Injected(intPtr);
			}
		}

		public ushort audioTrackCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_audioTrackCount_Injected(intPtr);
			}
		}

		public string GetAudioLanguageCode(ushort trackIndex)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				VideoPlayer.GetAudioLanguageCode_Injected(intPtr, trackIndex, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public ushort GetAudioChannelCount(ushort trackIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayer.GetAudioChannelCount_Injected(intPtr, trackIndex);
		}

		public uint GetAudioSampleRate(ushort trackIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayer.GetAudioSampleRate_Injected(intPtr, trackIndex);
		}

		public static extern ushort controlledAudioTrackMaxCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public ushort controlledAudioTrackCount
		{
			get
			{
				return this.GetControlledAudioTrackCount();
			}
			set
			{
				int controlledAudioTrackMaxCount = (int)VideoPlayer.controlledAudioTrackMaxCount;
				bool flag = (int)value > controlledAudioTrackMaxCount;
				if (flag)
				{
					throw new ArgumentException(string.Format("Cannot control more than {0} tracks.", controlledAudioTrackMaxCount), "value");
				}
				this.SetControlledAudioTrackCount(value);
			}
		}

		private ushort GetControlledAudioTrackCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayer.GetControlledAudioTrackCount_Injected(intPtr);
		}

		private void SetControlledAudioTrackCount(ushort value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayer.SetControlledAudioTrackCount_Injected(intPtr, value);
		}

		public void EnableAudioTrack(ushort trackIndex, bool enabled)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayer.EnableAudioTrack_Injected(intPtr, trackIndex, enabled);
		}

		public bool IsAudioTrackEnabled(ushort trackIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayer.IsAudioTrackEnabled_Injected(intPtr, trackIndex);
		}

		public VideoAudioOutputMode audioOutputMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_audioOutputMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_audioOutputMode_Injected(intPtr, value);
			}
		}

		public bool canSetDirectAudioVolume
		{
			[NativeName("CanSetDirectAudioVolume")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_canSetDirectAudioVolume_Injected(intPtr);
			}
		}

		public float GetDirectAudioVolume(ushort trackIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayer.GetDirectAudioVolume_Injected(intPtr, trackIndex);
		}

		public void SetDirectAudioVolume(ushort trackIndex, float volume)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayer.SetDirectAudioVolume_Injected(intPtr, trackIndex, volume);
		}

		public bool GetDirectAudioMute(ushort trackIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayer.GetDirectAudioMute_Injected(intPtr, trackIndex);
		}

		public void SetDirectAudioMute(ushort trackIndex, bool mute)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayer.SetDirectAudioMute_Injected(intPtr, trackIndex, mute);
		}

		[NativeHeader("Modules/Audio/Public/AudioSource.h")]
		public AudioSource GetTargetAudioSource(ushort trackIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<AudioSource>(VideoPlayer.GetTargetAudioSource_Injected(intPtr, trackIndex));
		}

		public void SetTargetAudioSource(ushort trackIndex, AudioSource source)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayer.SetTargetAudioSource_Injected(intPtr, trackIndex, Object.MarshalledUnityObject.Marshal<AudioSource>(source));
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event VideoPlayer.EventHandler prepareCompleted;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event VideoPlayer.EventHandler loopPointReached;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event VideoPlayer.EventHandler started;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event VideoPlayer.EventHandler frameDropped;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event VideoPlayer.ErrorEventHandler errorReceived;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event VideoPlayer.EventHandler seekCompleted;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event VideoPlayer.TimeEventHandler clockResyncOccurred;

		public bool sendFrameReadyEvents
		{
			[NativeName("AreFrameReadyEventsEnabled")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return VideoPlayer.get_sendFrameReadyEvents_Injected(intPtr);
			}
			[NativeName("EnableFrameReadyEvents")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VideoPlayer.set_sendFrameReadyEvents_Injected(intPtr, value);
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event VideoPlayer.FrameReadyEventHandler frameReady;

		[RequiredByNativeCode]
		private static void InvokePrepareCompletedCallback_Internal(VideoPlayer source)
		{
			bool flag = source.prepareCompleted != null;
			if (flag)
			{
				source.prepareCompleted(source);
			}
		}

		[RequiredByNativeCode]
		private static void InvokeFrameReadyCallback_Internal(VideoPlayer source, long frameIdx)
		{
			bool flag = source.frameReady != null;
			if (flag)
			{
				source.frameReady(source, frameIdx);
			}
		}

		[RequiredByNativeCode]
		private static void InvokeLoopPointReachedCallback_Internal(VideoPlayer source)
		{
			bool flag = source.loopPointReached != null;
			if (flag)
			{
				source.loopPointReached(source);
			}
		}

		[RequiredByNativeCode]
		private static void InvokeStartedCallback_Internal(VideoPlayer source)
		{
			bool flag = source.started != null;
			if (flag)
			{
				source.started(source);
			}
		}

		[RequiredByNativeCode]
		private static void InvokeFrameDroppedCallback_Internal(VideoPlayer source)
		{
			bool flag = source.frameDropped != null;
			if (flag)
			{
				source.frameDropped(source);
			}
		}

		[RequiredByNativeCode]
		private static void InvokeErrorReceivedCallback_Internal(VideoPlayer source, string errorStr)
		{
			bool flag = source.errorReceived != null;
			if (flag)
			{
				source.errorReceived(source, errorStr);
			}
		}

		[RequiredByNativeCode]
		private static void InvokeSeekCompletedCallback_Internal(VideoPlayer source)
		{
			bool flag = source.seekCompleted != null;
			if (flag)
			{
				source.seekCompleted(source);
			}
		}

		[RequiredByNativeCode]
		private static void InvokeClockResyncOccurredCallback_Internal(VideoPlayer source, double seconds)
		{
			bool flag = source.clockResyncOccurred != null;
			if (flag)
			{
				source.clockResyncOccurred(source, seconds);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VideoSource get_source_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_source_Injected(IntPtr _unity_self, VideoSource value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VideoTimeUpdateMode get_timeUpdateMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_timeUpdateMode_Injected(IntPtr _unity_self, VideoTimeUpdateMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_url_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_url_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_clip_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_clip_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VideoRenderMode get_renderMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_renderMode_Injected(IntPtr _unity_self, VideoRenderMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_canSetTimeUpdateMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_targetCamera_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetCamera_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_targetTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetTexture_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_targetMaterialRenderer_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetMaterialRenderer_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_targetMaterialProperty_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetMaterialProperty_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VideoAspectRatio get_aspectRatio_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_aspectRatio_Injected(IntPtr _unity_self, VideoAspectRatio value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_targetCameraAlpha_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetCameraAlpha_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Video3DLayout get_targetCamera3DLayout_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetCamera3DLayout_Injected(IntPtr _unity_self, Video3DLayout value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_texture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Prepare_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isPrepared_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_waitForFirstFrame_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_waitForFirstFrame_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_playOnAwake_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_playOnAwake_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Play_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Pause_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isPlaying_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isPaused_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_canSetTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double get_time_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_time_Injected(IntPtr _unity_self, double value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long get_frame_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_frame_Injected(IntPtr _unity_self, long value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double get_clockTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_canStep_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StepForward_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_canSetPlaybackSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_playbackSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_playbackSpeed_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isLooping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_isLooping_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_canSetTimeSource_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VideoTimeSource get_timeSource_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_timeSource_Injected(IntPtr _unity_self, VideoTimeSource value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VideoTimeReference get_timeReference_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_timeReference_Injected(IntPtr _unity_self, VideoTimeReference value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double get_externalReferenceTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_externalReferenceTime_Injected(IntPtr _unity_self, double value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_canSetSkipOnDrop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_skipOnDrop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_skipOnDrop_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong get_frameCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_frameRate_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double get_length_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_width_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_height_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_pixelAspectRatioNumerator_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_pixelAspectRatioDenominator_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ushort get_audioTrackCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAudioLanguageCode_Injected(IntPtr _unity_self, ushort trackIndex, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ushort GetAudioChannelCount_Injected(IntPtr _unity_self, ushort trackIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetAudioSampleRate_Injected(IntPtr _unity_self, ushort trackIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ushort GetControlledAudioTrackCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetControlledAudioTrackCount_Injected(IntPtr _unity_self, ushort value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableAudioTrack_Injected(IntPtr _unity_self, ushort trackIndex, bool enabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsAudioTrackEnabled_Injected(IntPtr _unity_self, ushort trackIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VideoAudioOutputMode get_audioOutputMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_audioOutputMode_Injected(IntPtr _unity_self, VideoAudioOutputMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_canSetDirectAudioVolume_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetDirectAudioVolume_Injected(IntPtr _unity_self, ushort trackIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDirectAudioVolume_Injected(IntPtr _unity_self, ushort trackIndex, float volume);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetDirectAudioMute_Injected(IntPtr _unity_self, ushort trackIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDirectAudioMute_Injected(IntPtr _unity_self, ushort trackIndex, bool mute);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetTargetAudioSource_Injected(IntPtr _unity_self, ushort trackIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTargetAudioSource_Injected(IntPtr _unity_self, ushort trackIndex, IntPtr source);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_sendFrameReadyEvents_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sendFrameReadyEvents_Injected(IntPtr _unity_self, bool value);

		public delegate void EventHandler(VideoPlayer source);

		public delegate void ErrorEventHandler(VideoPlayer source, string message);

		public delegate void FrameReadyEventHandler(VideoPlayer source, long frameIdx);

		public delegate void TimeEventHandler(VideoPlayer source, double seconds);
	}
}
