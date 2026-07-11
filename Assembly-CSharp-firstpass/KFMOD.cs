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
		return KFMOD.soundDescriptions[path];
	}

	public static void Initialize()
	{
		try
		{
			global::FMOD.Studio.System studioSystem = RuntimeManager.StudioSystem;
			KFMOD.didFmodInitializeSuccessfully = RuntimeManager.IsInitialized;
		}
		catch (Exception ex)
		{
			KFMOD.didFmodInitializeSuccessfully = false;
			if (ex.GetType() != typeof(SystemNotInitializedException))
			{
				throw ex;
			}
			global::Debug.LogWarning(ex, null);
		}
		KFMOD.CollectParameterUpdaters();
		KFMOD.CollectSoundDescriptions();
	}

	public static void PlayOneShot(string sound, Vector3 position)
	{
		EventInstance eventInstance = KFMOD.BeginOneShot(sound, position);
		KFMOD.EndOneShot(eventInstance);
	}

	public static void PlayOneShot(string sound)
	{
		KFMOD.PlayOneShot(sound, Vector3.zero);
	}

	public static EventInstance BeginOneShot(string sound, Vector3 position)
	{
		if (string.IsNullOrEmpty(sound) || App.IsExiting || !RuntimeManager.IsInitialized)
		{
			return default(EventInstance);
		}
		EventInstance eventInstance = KFMOD.CreateInstance(sound);
		if (!eventInstance.isValid())
		{
			if (KFMODDebugger.instance != null)
			{
			}
			return eventInstance;
		}
		Vector3 vector = new Vector3(position.x, position.y, 0f);
		if (KFMODDebugger.instance != null)
		{
		}
		ATTRIBUTES_3D attributes_3D = vector.To3DAttributes();
		eventInstance.set3DAttributes(attributes_3D);
		eventInstance.setVolume(1f);
		return eventInstance;
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

	public static EventInstance CreateInstance(string path)
	{
		if (KFMODDebugger.instance != null)
		{
		}
		if (!RuntimeManager.IsInitialized)
		{
			return default(EventInstance);
		}
		EventInstance eventInstance;
		try
		{
			eventInstance = RuntimeManager.CreateInstance(path);
		}
		catch (EventNotFoundException ex)
		{
			global::Debug.LogWarning(ex, null);
			return default(EventInstance);
		}
		HashedString hashedString = path;
		SoundDescription soundEventDescription = KFMOD.GetSoundEventDescription(hashedString);
		OneShotSoundParameterUpdater.Sound sound = new OneShotSoundParameterUpdater.Sound
		{
			ev = eventInstance,
			path = hashedString,
			description = soundEventDescription
		};
		foreach (OneShotSoundParameterUpdater oneShotSoundParameterUpdater in soundEventDescription.oneShotParameterUpdaters)
		{
			oneShotSoundParameterUpdater.Play(sound);
		}
		return eventInstance;
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
				eventDescription.getMaximumDistance(out num);
				if (num == 0f)
				{
					num = 60f;
				}
				soundDescription.falloffDistanceSq = num * num;
				List<OneShotSoundParameterUpdater> list = new List<OneShotSoundParameterUpdater>();
				int num2 = 0;
				eventDescription.getParameterCount(out num2);
				SoundDescription.Parameter[] array4 = new SoundDescription.Parameter[num2];
				for (int k = 0; k < num2; k++)
				{
					PARAMETER_DESCRIPTION parameter_DESCRIPTION;
					eventDescription.getParameterByIndex(k, out parameter_DESCRIPTION);
					string text2 = parameter_DESCRIPTION.name;
					array4[k] = new SoundDescription.Parameter
					{
						name = new HashedString(text2),
						idx = k
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
				for (Type type2 = type.BaseType; type2 != null; type2 = type2.BaseType)
				{
					if (type2 == typeof(OneShotSoundParameterUpdater))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					OneShotSoundParameterUpdater oneShotSoundParameterUpdater = (OneShotSoundParameterUpdater)Activator.CreateInstance(type);
					DebugUtil.Assert(!KFMOD.parameterUpdaters.ContainsKey(oneShotSoundParameterUpdater.parameter), "Assert!");
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
