using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Audio;
using UnityEngine.Scripting;

namespace UnityEngineInternal.Video
{
	[UsedByNativeCode]
	[NativeHeader("Modules/Video/Public/Base/MediaComponent.h")]
	internal class VideoPlayback
	{
		private VideoPlayback(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		public void StartPlayback()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayback.StartPlayback_Injected(intPtr);
		}

		public void PausePlayback()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayback.PausePlayback_Injected(intPtr);
		}

		public void StopPlayback()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayback.StopPlayback_Injected(intPtr);
		}

		public VideoError GetStatus()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetStatus_Injected(intPtr);
		}

		public bool IsReady()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.IsReady_Injected(intPtr);
		}

		public bool IsPlaying()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.IsPlaying_Injected(intPtr);
		}

		public void Step()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayback.Step_Injected(intPtr);
		}

		public bool CanStep()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.CanStep_Injected(intPtr);
		}

		public uint GetWidth()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetWidth_Injected(intPtr);
		}

		public uint GetHeight()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetHeight_Injected(intPtr);
		}

		public float GetFrameRate()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetFrameRate_Injected(intPtr);
		}

		public float GetDuration()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetDuration_Injected(intPtr);
		}

		public ulong GetFrameCount()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetFrameCount_Injected(intPtr);
		}

		public uint GetPixelAspectRatioNumerator()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetPixelAspectRatioNumerator_Injected(intPtr);
		}

		public uint GetPixelAspectRatioDenominator()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetPixelAspectRatioDenominator_Injected(intPtr);
		}

		public VideoPixelFormat GetPixelFormat()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetPixelFormat_Injected(intPtr);
		}

		public bool CanNotSkipOnDrop()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.CanNotSkipOnDrop_Injected(intPtr);
		}

		public void SetSkipOnDrop(bool skipOnDrop)
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayback.SetSkipOnDrop_Injected(intPtr, skipOnDrop);
		}

		public bool GetSkipOnDrop()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetSkipOnDrop_Injected(intPtr);
		}

		public bool GetTexture(Texture texture, out long outputFrameNum)
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetTexture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(texture), out outputFrameNum);
		}

		public void SeekToFrame(long frameIndex, VideoPlayback.Callback seekCompletedCallback)
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayback.SeekToFrame_Injected(intPtr, frameIndex, seekCompletedCallback);
		}

		public void SeekToTime(double secs, VideoPlayback.Callback seekCompletedCallback)
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayback.SeekToTime_Injected(intPtr, secs, seekCompletedCallback);
		}

		public float GetPlaybackSpeed()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetPlaybackSpeed_Injected(intPtr);
		}

		public void SetPlaybackSpeed(float value)
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayback.SetPlaybackSpeed_Injected(intPtr, value);
		}

		public bool GetLoop()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetLoop_Injected(intPtr);
		}

		public void SetLoop(bool value)
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayback.SetLoop_Injected(intPtr, value);
		}

		public void SetAdjustToLinearSpace(bool enable)
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayback.SetAdjustToLinearSpace_Injected(intPtr, enable);
		}

		[NativeHeader("Modules/Audio/Public/AudioSource.h")]
		public ushort GetAudioTrackCount()
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetAudioTrackCount_Injected(intPtr);
		}

		public ushort GetAudioChannelCount(ushort trackIdx)
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetAudioChannelCount_Injected(intPtr, trackIdx);
		}

		public uint GetAudioSampleRate(ushort trackIdx)
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetAudioSampleRate_Injected(intPtr, trackIdx);
		}

		public string GetAudioLanguageCode(ushort trackIdx)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				VideoPlayback.GetAudioLanguageCode_Injected(intPtr, trackIdx, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public void SetAudioTarget(ushort trackIdx, bool enabled, bool softwareOutput, AudioSource audioSource)
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VideoPlayback.SetAudioTarget_Injected(intPtr, trackIdx, enabled, softwareOutput, Object.MarshalledUnityObject.Marshal<AudioSource>(audioSource));
		}

		private uint GetAudioSampleProviderId(ushort trackIndex)
		{
			IntPtr intPtr = VideoPlayback.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VideoPlayback.GetAudioSampleProviderId_Injected(intPtr, trackIndex);
		}

		public AudioSampleProvider GetAudioSampleProvider(ushort trackIndex)
		{
			bool flag = trackIndex >= this.GetAudioTrackCount();
			if (flag)
			{
				throw new ArgumentOutOfRangeException("trackIndex", trackIndex, "VideoPlayback has " + this.GetAudioTrackCount().ToString() + " tracks.");
			}
			AudioSampleProvider audioSampleProvider = AudioSampleProvider.Lookup(this.GetAudioSampleProviderId(trackIndex), null, trackIndex);
			bool flag2 = audioSampleProvider == null;
			if (flag2)
			{
				throw new InvalidOperationException("VideoPlayback.GetAudioSampleProvider got null provider.");
			}
			bool flag3 = audioSampleProvider.owner != null;
			if (flag3)
			{
				throw new InvalidOperationException("Internal error: VideoPlayback.GetAudioSampleProvider got unexpected non-null provider owner.");
			}
			bool flag4 = audioSampleProvider.trackIndex != trackIndex;
			if (flag4)
			{
				throw new InvalidOperationException("Internal error: VideoPlayback.GetAudioSampleProvider got provider for track " + audioSampleProvider.trackIndex.ToString() + " instead of " + trackIndex.ToString());
			}
			return audioSampleProvider;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool PlatformSupportsH264();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool PlatformSupportsH265();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StartPlayback_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PausePlayback_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopPlayback_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VideoError GetStatus_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsReady_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsPlaying_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Step_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CanStep_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetWidth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetHeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFrameRate_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetDuration_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong GetFrameCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetPixelAspectRatioNumerator_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetPixelAspectRatioDenominator_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VideoPixelFormat GetPixelFormat_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CanNotSkipOnDrop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSkipOnDrop_Injected(IntPtr _unity_self, bool skipOnDrop);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetSkipOnDrop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetTexture_Injected(IntPtr _unity_self, IntPtr texture, out long outputFrameNum);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SeekToFrame_Injected(IntPtr _unity_self, long frameIndex, VideoPlayback.Callback seekCompletedCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SeekToTime_Injected(IntPtr _unity_self, double secs, VideoPlayback.Callback seekCompletedCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetPlaybackSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPlaybackSpeed_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetLoop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLoop_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAdjustToLinearSpace_Injected(IntPtr _unity_self, bool enable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ushort GetAudioTrackCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ushort GetAudioChannelCount_Injected(IntPtr _unity_self, ushort trackIdx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetAudioSampleRate_Injected(IntPtr _unity_self, ushort trackIdx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAudioLanguageCode_Injected(IntPtr _unity_self, ushort trackIdx, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAudioTarget_Injected(IntPtr _unity_self, ushort trackIdx, bool enabled, bool softwareOutput, IntPtr audioSource);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetAudioSampleProviderId_Injected(IntPtr _unity_self, ushort trackIndex);

		internal IntPtr m_Ptr;

		public delegate void Callback();

		internal static class BindingsMarshaller
		{
			public static VideoPlayback ConvertToManaged(IntPtr ptr)
			{
				return new VideoPlayback(ptr);
			}

			public static IntPtr ConvertToNative(VideoPlayback videoPlayback)
			{
				return videoPlayback.m_Ptr;
			}
		}
	}
}
