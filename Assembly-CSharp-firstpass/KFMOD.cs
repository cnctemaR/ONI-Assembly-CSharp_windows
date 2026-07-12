using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class KFMOD
{
	public static SoundDescription GetSoundEventDescription(HashedString path)
	{
		if (!KFMOD.soundDescriptions.ContainsKey(path))
		{
			return default(SoundDescription);
		}
		return KFMOD.soundDescriptions[path];
	}

	public static string GetEventReferencePath(EventReference event_ref)
	{
		EventDescription eventDescription = RuntimeManager.GetEventDescription(event_ref.Guid);
		string text = "";
		if (eventDescription.isValid())
		{
			eventDescription.getPath(out text);
		}
		return text;
	}

	public static void Initialize()
	{
		try
		{
			Settings instance = Settings.Instance;
			if (!DlcManager.IsExpansion1Active())
			{
				instance.Banks.RemoveAll((string b) => b.StartsWith("expansion1_"));
			}
			using (Dictionary<string, DlcManager.DlcInfo>.Enumerator enumerator = DlcManager.DLC_PACKS.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, DlcManager.DlcInfo> dlcInfo = enumerator.Current;
					if (!DlcManager.IsContentSubscribed(dlcInfo.Value.id))
					{
						instance.Banks.RemoveAll((string b) => b.IndexOf(dlcInfo.Value.directory, StringComparison.OrdinalIgnoreCase) >= 0);
					}
				}
			}
			if (global::UnityEngine.Object.FindObjectsOfType<RuntimeManager>().Length != 0)
			{
				global::Debug.LogError("FMOD got initialized before we tried to initialize it! This will cause bad things to happen!");
			}
			global::FMOD.Studio.System studioSystem = RuntimeManager.StudioSystem;
			KFMOD.didFmodInitializeSuccessfully = RuntimeManager.IsInitialized;
		}
		catch (Exception ex)
		{
			KFMOD.didFmodInitializeSuccessfully = false;
			if (!(ex.GetType() == typeof(SystemNotInitializedException)))
			{
				throw ex;
			}
			global::Debug.LogWarning(ex);
		}
		KFMOD.CollectParameterUpdaters();
		KFMOD.CollectSoundDescriptions();
	}

	public static void PlayOneShot(string sound, Vector3 position, float volume = 1f)
	{
		KFMOD.EndOneShot(KFMOD.BeginOneShot(sound, position, volume));
	}

	public static void PlayUISound(EventReference event_ref)
	{
		KFMOD.PlayUISound(KFMOD.GetEventReferencePath(event_ref));
	}

	public static void PlayUISound(string sound)
	{
		KFMOD.PlayOneShot(sound, Vector3.zero, 1f);
	}

	public static void PlayOneShotWithParameter(string sound, Vector3 position, string parameter, float parameterValue, float volume = 1f)
	{
		EventInstance eventInstance = KFMOD.BeginOneShot(sound, position, volume);
		eventInstance.setParameterByName(parameter, parameterValue, false);
		KFMOD.EndOneShot(eventInstance);
	}

	public static void PlayUISoundWithParameter(string sound, string parameter, float parameterValue)
	{
		KFMOD.PlayOneShotWithParameter(sound, Vector3.zero, parameter, parameterValue, 1f);
	}

	public static void PlayOneShotWithLabeledParameter(string sound, Vector3 position, string parameter, string parameterValue, float volume = 1f)
	{
		EventInstance eventInstance = KFMOD.BeginOneShot(sound, position, volume);
		eventInstance.setParameterByNameWithLabel(parameter, parameterValue, false);
		KFMOD.EndOneShot(eventInstance);
	}

	public static void PlayUISoundWithLabeledParameter(string sound, string parameter, string parameterValue)
	{
		KFMOD.PlayOneShotWithLabeledParameter(sound, Vector3.zero, parameter, parameterValue, 1f);
	}

	public static EventInstance BeginOneShot(EventReference event_ref, Vector3 position, float volume = 1f)
	{
		if (event_ref.IsNull || App.IsExiting || !RuntimeManager.IsInitialized)
		{
			return default(EventInstance);
		}
		EventInstance eventInstance = KFMOD.CreateInstance(event_ref);
		if (!eventInstance.isValid())
		{
			if (KFMODDebugger.instance != null)
			{
				string text;
				RuntimeManager.GetEventDescription(event_ref.Guid).getPath(out text);
			}
			return eventInstance;
		}
		Vector3 vector = new Vector3(position.x, position.y, position.z);
		if (KFMODDebugger.instance != null)
		{
			string text2;
			RuntimeManager.GetEventDescription(event_ref.Guid).getPath(out text2);
		}
		ATTRIBUTES_3D attributes_3D = vector.To3DAttributes();
		eventInstance.set3DAttributes(attributes_3D);
		eventInstance.setVolume(volume);
		return eventInstance;
	}

	public static EventInstance BeginOneShot(string sound, Vector3 position, float volume = 1f)
	{
		if (sound.IsNullOrWhiteSpace())
		{
			return default(EventInstance);
		}
		return KFMOD.BeginOneShot(RuntimeManager.PathToEventReference(sound), position, volume);
	}

	public static bool EndOneShot(EventInstance instance)
	{
		if (!instance.isValid())
		{
			return false;
		}
		instance.start();
		instance.release();
		return true;
	}

	public static EventInstance CreateInstance(EventReference event_ref)
	{
		if (!RuntimeManager.IsInitialized)
		{
			EventInstance eventInstance = default(EventInstance);
			return eventInstance;
		}
		EventInstance eventInstance2;
		try
		{
			eventInstance2 = RuntimeManager.CreateInstance(event_ref);
		}
		catch (EventNotFoundException ex)
		{
			global::Debug.LogWarning(ex);
			EventInstance eventInstance = default(EventInstance);
			return eventInstance;
		}
		EventDescription eventDescription;
		eventInstance2.getDescription(out eventDescription);
		string text;
		eventDescription.getPath(out text);
		HashedString hashedString = text;
		SoundDescription soundEventDescription = KFMOD.GetSoundEventDescription(hashedString);
		OneShotSoundParameterUpdater.Sound sound = new OneShotSoundParameterUpdater.Sound
		{
			ev = eventInstance2,
			path = hashedString,
			description = soundEventDescription
		};
		OneShotSoundParameterUpdater[] oneShotParameterUpdaters = soundEventDescription.oneShotParameterUpdaters;
		for (int i = 0; i < oneShotParameterUpdaters.Length; i++)
		{
			oneShotParameterUpdaters[i].Play(sound);
		}
		return eventInstance2;
	}

	public static EventInstance CreateInstance(string path)
	{
		return KFMOD.CreateInstance(RuntimeManager.PathToEventReference(path));
	}

	private static void CollectSoundDescriptions()
	{
		Bank[] array = null;
		RuntimeManager.StudioSystem.getBankList(out array);
		foreach (Bank bank in array)
		{
			EventDescription[] array3;
			bank.getEventList(out array3);
			foreach (EventDescription eventDescription in array3)
			{
				string text;
				eventDescription.getPath(out text);
				HashedString hashedString = text;
				SoundDescription soundDescription = default(SoundDescription);
				soundDescription.path = text;
				float num = 0f;
				float num2 = 0f;
				eventDescription.getMinMaxDistance(out num, out num2);
				if (num2 == 0f)
				{
					num2 = 60f;
				}
				soundDescription.falloffDistanceSq = num2 * num2;
				List<OneShotSoundParameterUpdater> list = new List<OneShotSoundParameterUpdater>();
				int num3 = 0;
				eventDescription.getParameterDescriptionCount(out num3);
				SoundDescription.Parameter[] array4 = new SoundDescription.Parameter[num3];
				for (int k = 0; k < num3; k++)
				{
					PARAMETER_DESCRIPTION parameter_DESCRIPTION;
					eventDescription.getParameterDescriptionByIndex(k, out parameter_DESCRIPTION);
					string text2 = parameter_DESCRIPTION.name;
					array4[k] = new SoundDescription.Parameter
					{
						name = new HashedString(text2),
						id = parameter_DESCRIPTION.id
					};
					OneShotSoundParameterUpdater oneShotSoundParameterUpdater = null;
					if (KFMOD.parameterUpdaters.TryGetValue(text2, out oneShotSoundParameterUpdater))
					{
						list.Add(oneShotSoundParameterUpdater);
					}
				}
				soundDescription.parameters = array4;
				soundDescription.oneShotParameterUpdaters = list.ToArray();
				KFMOD.soundDescriptions[hashedString] = soundDescription;
			}
		}
	}

	private static void CollectParameterUpdaters()
	{
		foreach (Type type in App.GetCurrentDomainTypes())
		{
			if (!type.IsAbstract)
			{
				bool flag = false;
				Type type2 = type.BaseType;
				while (type2 != null)
				{
					if (type2 == typeof(OneShotSoundParameterUpdater))
					{
						flag = true;
						break;
					}
					type2 = type2.BaseType;
				}
				if (flag)
				{
					OneShotSoundParameterUpdater oneShotSoundParameterUpdater = (OneShotSoundParameterUpdater)Activator.CreateInstance(type);
					DebugUtil.Assert(!KFMOD.parameterUpdaters.ContainsKey(oneShotSoundParameterUpdater.parameter));
					KFMOD.parameterUpdaters[oneShotSoundParameterUpdater.parameter] = oneShotSoundParameterUpdater;
				}
			}
		}
	}

	public static void RenderEveryTick(float dt)
	{
		foreach (KeyValuePair<HashedString, OneShotSoundParameterUpdater> keyValuePair in KFMOD.parameterUpdaters)
		{
			keyValuePair.Value.Update(dt);
		}
	}

	private static Dictionary<HashedString, SoundDescription> soundDescriptions = new Dictionary<HashedString, SoundDescription>();

	public static bool didFmodInitializeSuccessfully = true;

	private static Dictionary<HashedString, OneShotSoundParameterUpdater> parameterUpdaters = new Dictionary<HashedString, OneShotSoundParameterUpdater>();

	public static KFMOD.AudioDevice currentDevice;

	private struct SoundCountEntry
	{
		public int count;

		public float minObjects;

		public float maxObjects;
	}

	public struct AudioDevice
	{
		public int fmod_id;

		public string name;

		public Guid guid;

		public int systemRate;

		public SPEAKERMODE speakerMode;

		public int speakerModeChannels;

		public bool selected;
	}
}
