using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal sealed class AudioExtensionManager
	{
		internal static bool IsListenerSpatializerExtensionRegistered()
		{
			foreach (AudioSpatializerExtensionDefinition audioSpatializerExtensionDefinition in AudioExtensionManager.m_ListenerSpatializerExtensionDefinitions)
			{
				if (AudioSettings.GetSpatializerPluginName() == audioSpatializerExtensionDefinition.spatializerName)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsSourceSpatializerExtensionRegistered()
		{
			foreach (AudioSpatializerExtensionDefinition audioSpatializerExtensionDefinition in AudioExtensionManager.m_SourceSpatializerExtensionDefinitions)
			{
				if (AudioSettings.GetSpatializerPluginName() == audioSpatializerExtensionDefinition.spatializerName)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsSourceAmbisonicDecoderExtensionRegistered()
		{
			foreach (AudioAmbisonicExtensionDefinition audioAmbisonicExtensionDefinition in AudioExtensionManager.m_SourceAmbisonicDecoderExtensionDefinitions)
			{
				if (AudioSettings.GetAmbisonicDecoderPluginName() == audioAmbisonicExtensionDefinition.ambisonicPluginName)
				{
					return true;
				}
			}
			return false;
		}

		internal static AudioSourceExtension AddSpatializerExtension(AudioSource source)
		{
			AudioSourceExtension audioSourceExtension;
			if (!source.spatialize)
			{
				audioSourceExtension = null;
			}
			else if (source.spatializerExtension != null)
			{
				audioSourceExtension = source.spatializerExtension;
			}
			else
			{
				AudioExtensionManager.RegisterBuiltinDefinitions();
				foreach (AudioSpatializerExtensionDefinition audioSpatializerExtensionDefinition in AudioExtensionManager.m_SourceSpatializerExtensionDefinitions)
				{
					if (AudioSettings.GetSpatializerPluginName() == audioSpatializerExtensionDefinition.spatializerName)
					{
						AudioSourceExtension audioSourceExtension2 = source.AddSpatializerExtension(audioSpatializerExtensionDefinition.definition.GetExtensionType());
						if (audioSourceExtension2 != null)
						{
							audioSourceExtension2.audioSource = source;
							source.spatializerExtension = audioSourceExtension2;
							AudioExtensionManager.WriteExtensionProperties(audioSourceExtension2, audioSpatializerExtensionDefinition.definition.GetExtensionType().Name);
							return audioSourceExtension2;
						}
					}
				}
				audioSourceExtension = null;
			}
			return audioSourceExtension;
		}

		internal static AudioSourceExtension AddAmbisonicDecoderExtension(AudioSource source)
		{
			AudioSourceExtension audioSourceExtension;
			if (source.ambisonicExtension != null)
			{
				audioSourceExtension = source.ambisonicExtension;
			}
			else
			{
				AudioExtensionManager.RegisterBuiltinDefinitions();
				foreach (AudioAmbisonicExtensionDefinition audioAmbisonicExtensionDefinition in AudioExtensionManager.m_SourceAmbisonicDecoderExtensionDefinitions)
				{
					if (AudioSettings.GetAmbisonicDecoderPluginName() == audioAmbisonicExtensionDefinition.ambisonicPluginName)
					{
						AudioSourceExtension audioSourceExtension2 = source.AddAmbisonicExtension(audioAmbisonicExtensionDefinition.definition.GetExtensionType());
						if (audioSourceExtension2 != null)
						{
							audioSourceExtension2.audioSource = source;
							source.ambisonicExtension = audioSourceExtension2;
							return audioSourceExtension2;
						}
					}
				}
				audioSourceExtension = null;
			}
			return audioSourceExtension;
		}

		internal static void WriteExtensionProperties(AudioSourceExtension extension, string extensionName)
		{
			if (AudioExtensionManager.m_SpatializerExtensionName == 0)
			{
				AudioExtensionManager.m_SpatializerExtensionName = extensionName;
			}
			for (int i = 0; i < extension.audioSource.GetNumExtensionProperties(); i++)
			{
				if (extension.audioSource.ReadExtensionName(i) == AudioExtensionManager.m_SpatializerExtensionName)
				{
					PropertyName propertyName = extension.audioSource.ReadExtensionPropertyName(i);
					float num = extension.audioSource.ReadExtensionPropertyValue(i);
					extension.WriteExtensionProperty(propertyName, num);
				}
			}
			extension.audioSource.ClearExtensionProperties(AudioExtensionManager.m_SpatializerExtensionName);
		}

		internal static AudioListenerExtension AddSpatializerExtension(AudioListener listener)
		{
			AudioListenerExtension audioListenerExtension;
			if (listener.spatializerExtension != null)
			{
				audioListenerExtension = listener.spatializerExtension;
			}
			else
			{
				AudioExtensionManager.RegisterBuiltinDefinitions();
				foreach (AudioSpatializerExtensionDefinition audioSpatializerExtensionDefinition in AudioExtensionManager.m_ListenerSpatializerExtensionDefinitions)
				{
					if (AudioSettings.GetSpatializerPluginName() == audioSpatializerExtensionDefinition.spatializerName || AudioSettings.GetAmbisonicDecoderPluginName() == audioSpatializerExtensionDefinition.spatializerName)
					{
						AudioListenerExtension audioListenerExtension2 = listener.AddExtension(audioSpatializerExtensionDefinition.definition.GetExtensionType());
						if (audioListenerExtension2 != null)
						{
							audioListenerExtension2.audioListener = listener;
							listener.spatializerExtension = audioListenerExtension2;
							AudioExtensionManager.WriteExtensionProperties(audioListenerExtension2, audioSpatializerExtensionDefinition.definition.GetExtensionType().Name);
							return audioListenerExtension2;
						}
					}
				}
				audioListenerExtension = null;
			}
			return audioListenerExtension;
		}

		internal static void WriteExtensionProperties(AudioListenerExtension extension, string extensionName)
		{
			if (AudioExtensionManager.m_ListenerSpatializerExtensionName == 0)
			{
				AudioExtensionManager.m_ListenerSpatializerExtensionName = extensionName;
			}
			for (int i = 0; i < extension.audioListener.GetNumExtensionProperties(); i++)
			{
				if (extension.audioListener.ReadExtensionName(i) == AudioExtensionManager.m_ListenerSpatializerExtensionName)
				{
					PropertyName propertyName = extension.audioListener.ReadExtensionPropertyName(i);
					float num = extension.audioListener.ReadExtensionPropertyValue(i);
					extension.WriteExtensionProperty(propertyName, num);
				}
			}
			extension.audioListener.ClearExtensionProperties(AudioExtensionManager.m_ListenerSpatializerExtensionName);
		}

		internal static AudioListenerExtension GetSpatializerExtension(AudioListener listener)
		{
			AudioListenerExtension audioListenerExtension;
			if (listener.spatializerExtension != null)
			{
				audioListenerExtension = listener.spatializerExtension;
			}
			else
			{
				audioListenerExtension = null;
			}
			return audioListenerExtension;
		}

		internal static AudioSourceExtension GetSpatializerExtension(AudioSource source)
		{
			return (!source.spatialize) ? null : source.spatializerExtension;
		}

		internal static AudioSourceExtension GetAmbisonicExtension(AudioSource source)
		{
			return source.ambisonicExtension;
		}

		internal static Type GetListenerSpatializerExtensionType()
		{
			foreach (AudioSpatializerExtensionDefinition audioSpatializerExtensionDefinition in AudioExtensionManager.m_ListenerSpatializerExtensionDefinitions)
			{
				if (AudioSettings.GetSpatializerPluginName() == audioSpatializerExtensionDefinition.spatializerName)
				{
					return audioSpatializerExtensionDefinition.definition.GetExtensionType();
				}
			}
			return null;
		}

		internal static Type GetListenerSpatializerExtensionEditorType()
		{
			foreach (AudioSpatializerExtensionDefinition audioSpatializerExtensionDefinition in AudioExtensionManager.m_ListenerSpatializerExtensionDefinitions)
			{
				if (AudioSettings.GetSpatializerPluginName() == audioSpatializerExtensionDefinition.spatializerName)
				{
					return audioSpatializerExtensionDefinition.editorDefinition.GetExtensionType();
				}
			}
			return null;
		}

		internal static Type GetSourceSpatializerExtensionType()
		{
			foreach (AudioSpatializerExtensionDefinition audioSpatializerExtensionDefinition in AudioExtensionManager.m_SourceSpatializerExtensionDefinitions)
			{
				if (AudioSettings.GetSpatializerPluginName() == audioSpatializerExtensionDefinition.spatializerName)
				{
					return audioSpatializerExtensionDefinition.definition.GetExtensionType();
				}
			}
			return null;
		}

		internal static Type GetSourceSpatializerExtensionEditorType()
		{
			foreach (AudioSpatializerExtensionDefinition audioSpatializerExtensionDefinition in AudioExtensionManager.m_SourceSpatializerExtensionDefinitions)
			{
				if (AudioSettings.GetSpatializerPluginName() == audioSpatializerExtensionDefinition.spatializerName)
				{
					return audioSpatializerExtensionDefinition.editorDefinition.GetExtensionType();
				}
			}
			return null;
		}

		internal static Type GetSourceAmbisonicExtensionType()
		{
			foreach (AudioAmbisonicExtensionDefinition audioAmbisonicExtensionDefinition in AudioExtensionManager.m_SourceAmbisonicDecoderExtensionDefinitions)
			{
				if (AudioSettings.GetAmbisonicDecoderPluginName() == audioAmbisonicExtensionDefinition.ambisonicPluginName)
				{
					return audioAmbisonicExtensionDefinition.definition.GetExtensionType();
				}
			}
			return null;
		}

		internal static PropertyName GetSpatializerName()
		{
			return AudioExtensionManager.m_SpatializerName;
		}

		internal static PropertyName GetSourceSpatializerExtensionName()
		{
			return AudioExtensionManager.m_SpatializerExtensionName;
		}

		internal static PropertyName GetListenerSpatializerExtensionName()
		{
			return AudioExtensionManager.m_ListenerSpatializerExtensionName;
		}

		internal static void AddExtensionToManager(AudioSourceExtension extension)
		{
			AudioExtensionManager.RegisterBuiltinDefinitions();
			if (extension.m_ExtensionManagerUpdateIndex == -1)
			{
				AudioExtensionManager.m_SourceExtensionsToUpdate.Add(extension);
				extension.m_ExtensionManagerUpdateIndex = AudioExtensionManager.m_SourceExtensionsToUpdate.Count - 1;
			}
		}

		internal static void RemoveExtensionFromManager(AudioSourceExtension extension)
		{
			int extensionManagerUpdateIndex = extension.m_ExtensionManagerUpdateIndex;
			if (extensionManagerUpdateIndex >= 0 && extensionManagerUpdateIndex < AudioExtensionManager.m_SourceExtensionsToUpdate.Count)
			{
				int num = AudioExtensionManager.m_SourceExtensionsToUpdate.Count - 1;
				AudioExtensionManager.m_SourceExtensionsToUpdate[extensionManagerUpdateIndex] = AudioExtensionManager.m_SourceExtensionsToUpdate[num];
				AudioExtensionManager.m_SourceExtensionsToUpdate[extensionManagerUpdateIndex].m_ExtensionManagerUpdateIndex = extensionManagerUpdateIndex;
				AudioExtensionManager.m_SourceExtensionsToUpdate.RemoveAt(num);
			}
			extension.m_ExtensionManagerUpdateIndex = -1;
		}

		internal static void Update()
		{
			AudioExtensionManager.RegisterBuiltinDefinitions();
			AudioListener audioListener = AudioExtensionManager.GetAudioListener() as AudioListener;
			if (audioListener != null)
			{
				AudioListenerExtension audioListenerExtension = AudioExtensionManager.AddSpatializerExtension(audioListener);
				if (audioListenerExtension != null)
				{
					audioListenerExtension.ExtensionUpdate();
				}
			}
			for (int i = 0; i < AudioExtensionManager.m_SourceExtensionsToUpdate.Count; i++)
			{
				AudioExtensionManager.m_SourceExtensionsToUpdate[i].ExtensionUpdate();
			}
			AudioExtensionManager.m_NextStopIndex = ((AudioExtensionManager.m_NextStopIndex < AudioExtensionManager.m_SourceExtensionsToUpdate.Count) ? AudioExtensionManager.m_NextStopIndex : 0);
			int num = ((AudioExtensionManager.m_SourceExtensionsToUpdate.Count <= 0) ? 0 : (1 + AudioExtensionManager.m_SourceExtensionsToUpdate.Count / 8));
			for (int j = 0; j < num; j++)
			{
				AudioSourceExtension audioSourceExtension = AudioExtensionManager.m_SourceExtensionsToUpdate[AudioExtensionManager.m_NextStopIndex];
				if (audioSourceExtension.audioSource == null || !audioSourceExtension.audioSource.enabled || !audioSourceExtension.audioSource.isPlaying)
				{
					audioSourceExtension.Stop();
					AudioExtensionManager.RemoveExtensionFromManager(audioSourceExtension);
				}
				else
				{
					AudioExtensionManager.m_NextStopIndex++;
					AudioExtensionManager.m_NextStopIndex = ((AudioExtensionManager.m_NextStopIndex < AudioExtensionManager.m_SourceExtensionsToUpdate.Count) ? AudioExtensionManager.m_NextStopIndex : 0);
				}
			}
		}

		internal static void GetReadyToPlay(AudioSourceExtension extension)
		{
			if (extension != null)
			{
				extension.Play();
				AudioExtensionManager.AddExtensionToManager(extension);
			}
		}

		private static void RegisterBuiltinDefinitions()
		{
			bool flag = false;
			if (!AudioExtensionManager.m_BuiltinDefinitionsRegistered)
			{
				if (flag || AudioSettings.GetSpatializerPluginName() == "GVR Audio Spatializer")
				{
				}
				if (flag || AudioSettings.GetAmbisonicDecoderPluginName() == "GVR Audio Spatializer")
				{
				}
				AudioExtensionManager.m_BuiltinDefinitionsRegistered = true;
			}
		}

		private static bool RegisterListenerSpatializerDefinition(string spatializerName, AudioExtensionDefinition extensionDefinition, AudioExtensionDefinition editorDefinition)
		{
			foreach (AudioSpatializerExtensionDefinition audioSpatializerExtensionDefinition in AudioExtensionManager.m_ListenerSpatializerExtensionDefinitions)
			{
				if (spatializerName == audioSpatializerExtensionDefinition.spatializerName)
				{
					Debug.Log("RegisterListenerSpatializerDefinition failed for " + extensionDefinition.GetExtensionType() + ". We only allow one audio listener extension to be registered for each spatializer.");
					return false;
				}
			}
			AudioSpatializerExtensionDefinition audioSpatializerExtensionDefinition2 = new AudioSpatializerExtensionDefinition(spatializerName, extensionDefinition, editorDefinition);
			AudioExtensionManager.m_ListenerSpatializerExtensionDefinitions.Add(audioSpatializerExtensionDefinition2);
			return true;
		}

		private static bool RegisterSourceSpatializerDefinition(string spatializerName, AudioExtensionDefinition extensionDefinition, AudioExtensionDefinition editorDefinition)
		{
			foreach (AudioSpatializerExtensionDefinition audioSpatializerExtensionDefinition in AudioExtensionManager.m_SourceSpatializerExtensionDefinitions)
			{
				if (spatializerName == audioSpatializerExtensionDefinition.spatializerName)
				{
					Debug.Log("RegisterSourceSpatializerDefinition failed for " + extensionDefinition.GetExtensionType() + ". We only allow one audio source extension to be registered for each spatializer.");
					return false;
				}
			}
			AudioSpatializerExtensionDefinition audioSpatializerExtensionDefinition2 = new AudioSpatializerExtensionDefinition(spatializerName, extensionDefinition, editorDefinition);
			AudioExtensionManager.m_SourceSpatializerExtensionDefinitions.Add(audioSpatializerExtensionDefinition2);
			return true;
		}

		private static bool RegisterSourceAmbisonicDefinition(string ambisonicDecoderName, AudioExtensionDefinition extensionDefinition)
		{
			foreach (AudioAmbisonicExtensionDefinition audioAmbisonicExtensionDefinition in AudioExtensionManager.m_SourceAmbisonicDecoderExtensionDefinitions)
			{
				if (ambisonicDecoderName == audioAmbisonicExtensionDefinition.ambisonicPluginName)
				{
					Debug.Log("RegisterSourceAmbisonicDefinition failed for " + extensionDefinition.GetExtensionType() + ". We only allow one audio source extension to be registered for each ambisonic decoder.");
					return false;
				}
			}
			AudioAmbisonicExtensionDefinition audioAmbisonicExtensionDefinition2 = new AudioAmbisonicExtensionDefinition(ambisonicDecoderName, extensionDefinition);
			AudioExtensionManager.m_SourceAmbisonicDecoderExtensionDefinitions.Add(audioAmbisonicExtensionDefinition2);
			return true;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Object GetAudioListener();

		private static List<AudioSpatializerExtensionDefinition> m_ListenerSpatializerExtensionDefinitions = new List<AudioSpatializerExtensionDefinition>();

		private static List<AudioSpatializerExtensionDefinition> m_SourceSpatializerExtensionDefinitions = new List<AudioSpatializerExtensionDefinition>();

		private static List<AudioAmbisonicExtensionDefinition> m_SourceAmbisonicDecoderExtensionDefinitions = new List<AudioAmbisonicExtensionDefinition>();

		private static List<AudioSourceExtension> m_SourceExtensionsToUpdate = new List<AudioSourceExtension>();

		private static int m_NextStopIndex = 0;

		private static bool m_BuiltinDefinitionsRegistered = false;

		private static PropertyName m_SpatializerName = 0;

		private static PropertyName m_SpatializerExtensionName = 0;

		private static PropertyName m_ListenerSpatializerExtensionName = 0;
	}
}
