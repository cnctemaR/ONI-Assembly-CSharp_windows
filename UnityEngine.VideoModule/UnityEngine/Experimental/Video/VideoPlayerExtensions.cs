using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Audio;
using UnityEngine.Video;

namespace UnityEngine.Experimental.Video
{
	[StaticAccessor("VideoPlayerExtensionsBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("VideoScriptingClasses.h")]
	[NativeHeader("Modules/Video/Public/VideoPlayer.h")]
	[NativeHeader("Modules/Video/Public/ScriptBindings/VideoPlayerExtensions.bindings.h")]
	public static class VideoPlayerExtensions
	{
		public static AudioSampleProvider GetAudioSampleProvider(this VideoPlayer vp, ushort trackIndex)
		{
			ushort controlledAudioTrackCount = vp.controlledAudioTrackCount;
			bool flag = trackIndex >= controlledAudioTrackCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("trackIndex", trackIndex, "VideoPlayer is currently configured with " + controlledAudioTrackCount.ToString() + " tracks.");
			}
			VideoAudioOutputMode audioOutputMode = vp.audioOutputMode;
			bool flag2 = audioOutputMode != VideoAudioOutputMode.APIOnly;
			if (flag2)
			{
				throw new InvalidOperationException("VideoPlayer.GetAudioSampleProvider requires audioOutputMode to be APIOnly. Current: " + audioOutputMode.ToString());
			}
			AudioSampleProvider audioSampleProvider = AudioSampleProvider.Lookup(vp.InternalGetAudioSampleProviderId(trackIndex), vp, trackIndex);
			bool flag3 = audioSampleProvider == null;
			if (flag3)
			{
				throw new InvalidOperationException("VideoPlayer.GetAudioSampleProvider got null provider.");
			}
			bool flag4 = audioSampleProvider.owner != vp;
			if (flag4)
			{
				throw new InvalidOperationException("Internal error: VideoPlayer.GetAudioSampleProvider got provider used by another object.");
			}
			bool flag5 = audioSampleProvider.trackIndex != trackIndex;
			if (flag5)
			{
				throw new InvalidOperationException("Internal error: VideoPlayer.GetAudioSampleProvider got provider for track " + audioSampleProvider.trackIndex.ToString() + " instead of " + trackIndex.ToString());
			}
			return audioSampleProvider;
		}

		internal static uint InternalGetAudioSampleProviderId([NotNull] this VideoPlayer vp, ushort trackIndex)
		{
			if (vp == null)
			{
				ThrowHelper.ThrowArgumentNullException(vp, "vp");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VideoPlayer>(vp);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(vp, "vp");
			}
			return VideoPlayerExtensions.InternalGetAudioSampleProviderId_Injected(intPtr, trackIndex);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint InternalGetAudioSampleProviderId_Injected(IntPtr vp, ushort trackIndex);
	}
}
