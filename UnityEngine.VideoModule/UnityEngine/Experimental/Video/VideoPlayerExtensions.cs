using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Audio;
using UnityEngine.Video;

namespace UnityEngine.Experimental.Video
{
	/// <summary>
	///   <para>Extension methods for the Video.VideoPlayer class.</para>
	/// </summary>
	[NativeHeader("VideoScriptingClasses.h")]
	[NativeHeader("Modules/Video/Public/VideoPlayer.h")]
	[NativeHeader("Modules/Video/Public/ScriptBindings/VideoPlayerExtensions.bindings.h")]
	[StaticAccessor("VideoPlayerExtensionsBindings", StaticAccessorType.DoubleColon)]
	public static class VideoPlayerExtensions
	{
		/// <summary>
		///   <para>Return the Experimental.Audio.AudioSampleProvider for the specified track, used to receive audio samples during playback.</para>
		/// </summary>
		/// <param name="vp">The "this" pointer for the extension method.</param>
		/// <param name="trackIndex">The audio track index for which the sample provider is queried.</param>
		/// <returns>
		///   <para>The sample provider for the specified track.</para>
		/// </returns>
		public static AudioSampleProvider GetAudioSampleProvider(this VideoPlayer vp, ushort trackIndex)
		{
			ushort controlledAudioTrackCount = vp.controlledAudioTrackCount;
			if (trackIndex >= controlledAudioTrackCount)
			{
				throw new ArgumentOutOfRangeException("trackIndex", trackIndex, "VideoPlayer is currently configured with " + controlledAudioTrackCount + " tracks.");
			}
			VideoAudioOutputMode audioOutputMode = vp.audioOutputMode;
			if (audioOutputMode != VideoAudioOutputMode.APIOnly)
			{
				throw new InvalidOperationException("VideoPlayer.GetAudioSampleProvider requires audioOutputMode to be APIOnly. Current: " + audioOutputMode);
			}
			AudioSampleProvider audioSampleProvider = AudioSampleProvider.Lookup(VideoPlayerExtensions.InternalGetAudioSampleProviderId(vp, trackIndex), vp, trackIndex);
			if (audioSampleProvider == null)
			{
				throw new InvalidOperationException("VideoPlayer.GetAudioSampleProvider got null provider.");
			}
			if (audioSampleProvider.owner != vp)
			{
				throw new InvalidOperationException("Internal error: VideoPlayer.GetAudioSampleProvider got provider used by another object.");
			}
			if (audioSampleProvider.trackIndex != trackIndex)
			{
				throw new InvalidOperationException(string.Concat(new object[] { "Internal error: VideoPlayer.GetAudioSampleProvider got provider for track ", audioSampleProvider.trackIndex, " instead of ", trackIndex }));
			}
			return audioSampleProvider;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint InternalGetAudioSampleProviderId(VideoPlayer vp, ushort trackIndex);
	}
}
