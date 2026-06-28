using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class AudioSettings
	{
		public static extern AudioSpeakerMode driverCapabilities
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern AudioSpeakerMode speakerMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern int profilerCaptureFlags
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public static extern double dspTime
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int outputSampleRate
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetDSPBufferSize(out int bufferLength, out int numBuffers);

		[Obsolete("AudioSettings.SetDSPBufferSize is deprecated and has been replaced by audio project settings and the AudioSettings.GetConfiguration/AudioSettings.Reset API.")]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetDSPBufferSize(int bufferLength, int numBuffers);

		public static AudioConfiguration GetConfiguration()
		{
			AudioConfiguration audioConfiguration;
			AudioSettings.INTERNAL_CALL_GetConfiguration(out audioConfiguration);
			return audioConfiguration;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetConfiguration(out AudioConfiguration value);

		public static bool Reset(AudioConfiguration config)
		{
			return AudioSettings.INTERNAL_CALL_Reset(ref config);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Reset(ref AudioConfiguration config);

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event AudioSettings.AudioConfigurationChangeHandler OnAudioConfigurationChanged;

		[RequiredByNativeCode]
		internal static void InvokeOnAudioConfigurationChanged(bool deviceWasChanged)
		{
			if (AudioSettings.OnAudioConfigurationChanged != null)
			{
				AudioSettings.OnAudioConfigurationChanged(deviceWasChanged);
			}
		}

		internal static extern bool unityAudioDisabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public delegate void AudioConfigurationChangeHandler(bool deviceWasChanged);
	}
}
