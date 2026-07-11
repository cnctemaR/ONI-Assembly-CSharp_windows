using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Controls the global audio settings from script.</para>
	/// </summary>
	public sealed class AudioSettings
	{
		/// <summary>
		///   <para>Returns the speaker mode capability of the current audio driver. (Read Only)</para>
		/// </summary>
		public static extern AudioSpeakerMode driverCapabilities
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Gets the current speaker mode. Default is 2 channel stereo.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Returns the current time of the audio system.</para>
		/// </summary>
		[ThreadAndSerializationSafe]
		public static extern double dspTime
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Get the mixer's current output rate.</para>
		/// </summary>
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

		[GeneratedByOldBindingsGenerator]
		[Obsolete("AudioSettings.SetDSPBufferSize is deprecated and has been replaced by audio project settings and the AudioSettings.GetConfiguration/AudioSettings.Reset API.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetDSPBufferSize(int bufferLength, int numBuffers);

		/// <summary>
		///   <para>Returns the name of the spatializer selected on the currently-running platform.</para>
		/// </summary>
		/// <returns>
		///   <para>The spatializer plugin name.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetSpatializerPluginName();

		/// <summary>
		///   <para>Returns the current configuration of the audio device and system. The values in the struct may then be modified and reapplied via AudioSettings.Reset.</para>
		/// </summary>
		/// <returns>
		///   <para>The new configuration to be applied.</para>
		/// </returns>
		public static AudioConfiguration GetConfiguration()
		{
			AudioConfiguration audioConfiguration;
			AudioSettings.INTERNAL_CALL_GetConfiguration(out audioConfiguration);
			return audioConfiguration;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetConfiguration(out AudioConfiguration value);

		/// <summary>
		///   <para>Performs a change of the device configuration. In response to this the AudioSettings.OnAudioConfigurationChanged delegate is invoked with the argument deviceWasChanged=false. It cannot be guaranteed that the exact settings specified can be used, but the an attempt is made to use the closest match supported by the system.</para>
		/// </summary>
		/// <param name="config">The new configuration to be used.</param>
		/// <returns>
		///   <para>True if all settings could be successfully applied.</para>
		/// </returns>
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

		[RequiredByNativeCode]
		internal static void InvokeOnAudioManagerUpdate()
		{
			AudioExtensionManager.Update();
		}

		[RequiredByNativeCode]
		internal static void InvokeOnAudioSourcePlay(AudioSource source)
		{
			AudioSourceExtension audioSourceExtension = AudioExtensionManager.AddSpatializerExtension(source);
			if (audioSourceExtension != null)
			{
				AudioExtensionManager.GetReadyToPlay(audioSourceExtension);
			}
			if (source.clip != null && source.clip.ambisonic)
			{
				AudioSourceExtension audioSourceExtension2 = AudioExtensionManager.AddAmbisonicDecoderExtension(source);
				if (audioSourceExtension2 != null)
				{
					AudioExtensionManager.GetReadyToPlay(audioSourceExtension2);
				}
			}
		}

		internal static extern bool unityAudioDisabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetAmbisonicDecoderPluginName();

		/// <summary>
		///   <para>A delegate called whenever the global audio settings are changed, either by AudioSettings.Reset or by an external device change such as the OS control panel changing the sample rate or because the default output device was changed, for example when plugging in an HDMI monitor or a USB headset.</para>
		/// </summary>
		/// <param name="deviceWasChanged">True if the change was caused by an device change.</param>
		public delegate void AudioConfigurationChangeHandler(bool deviceWasChanged);
	}
}
