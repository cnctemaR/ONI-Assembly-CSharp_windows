using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

internal class UpdateObjectCountParameter : LoopingSoundParameterUpdater
{
	public static UpdateObjectCountParameter.Settings GetSettings(HashedString path_hash, SoundDescription description)
	{
		UpdateObjectCountParameter.Settings settings = default(UpdateObjectCountParameter.Settings);
		if (!UpdateObjectCountParameter.settings.TryGetValue(path_hash, out settings))
		{
			settings = default(UpdateObjectCountParameter.Settings);
			EventDescription eventDescription = RuntimeManager.GetEventDescription(description.path);
			USER_PROPERTY user_PROPERTY;
			if (eventDescription.getUserProperty("minObj", out user_PROPERTY) == RESULT.OK)
			{
				settings.minObjects = (float)((short)user_PROPERTY.floatValue());
			}
			else
			{
				settings.minObjects = 1f;
			}
			USER_PROPERTY user_PROPERTY2;
			if (eventDescription.getUserProperty("maxObj", out user_PROPERTY2) == RESULT.OK)
			{
				settings.maxObjects = user_PROPERTY2.floatValue();
			}
			else
			{
				settings.maxObjects = 0f;
			}
			USER_PROPERTY user_PROPERTY3;
			if (eventDescription.getUserProperty("curveType", out user_PROPERTY3) == RESULT.OK && user_PROPERTY3.stringValue() == "exp")
			{
				settings.useExponentialCurve = true;
			}
			settings.parameterIdx = description.GetParameterIdx(UpdateObjectCountParameter.parameterHash);
			settings.path = path_hash;
			UpdateObjectCountParameter.settings[path_hash] = settings;
		}
		return settings;
	}

	public static void ApplySettings(EventInstance ev, int count, UpdateObjectCountParameter.Settings settings)
	{
		float num = 0f;
		if (settings.maxObjects != settings.minObjects)
		{
			num = ((float)count - settings.minObjects) / (settings.maxObjects - settings.minObjects);
			num = Mathf.Clamp01(num);
		}
		if (settings.useExponentialCurve)
		{
			num *= num;
		}
		ev.setParameterValueByIndex(settings.parameterIdx, num);
	}

	public UpdateObjectCountParameter()
		: base("objectCount")
	{
	}

	public override void Add(LoopingSoundParameterUpdater.Sound sound)
	{
		UpdateObjectCountParameter.Settings settings = UpdateObjectCountParameter.GetSettings(sound.path, sound.description);
		UpdateObjectCountParameter.Entry entry = new UpdateObjectCountParameter.Entry
		{
			ev = sound.ev,
			settings = settings
		};
		this.entries.Add(entry);
	}

	public override void Update(float dt)
	{
		DictionaryPool<HashedString, int, LoopingSoundManager>.PooledDictionary pooledDictionary = DictionaryPool<HashedString, int, LoopingSoundManager>.Allocate();
		foreach (UpdateObjectCountParameter.Entry entry in this.entries)
		{
			int num = 0;
			pooledDictionary.TryGetValue(entry.settings.path, out num);
			num = (pooledDictionary[entry.settings.path] = num + 1);
		}
		foreach (UpdateObjectCountParameter.Entry entry2 in this.entries)
		{
			int num2 = pooledDictionary[entry2.settings.path];
			UpdateObjectCountParameter.ApplySettings(entry2.ev, num2, entry2.settings);
		}
		pooledDictionary.Recycle();
	}

	public override void Remove(LoopingSoundParameterUpdater.Sound sound)
	{
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (this.entries[i].ev.handle == sound.ev.handle)
			{
				this.entries.RemoveAt(i);
				return;
			}
		}
	}

	public static void Clear()
	{
		UpdateObjectCountParameter.settings.Clear();
	}

	private List<UpdateObjectCountParameter.Entry> entries = new List<UpdateObjectCountParameter.Entry>();

	private static Dictionary<HashedString, UpdateObjectCountParameter.Settings> settings = new Dictionary<HashedString, UpdateObjectCountParameter.Settings>();

	private static readonly HashedString parameterHash = "objectCount";

	private struct Entry
	{
		public EventInstance ev;

		public UpdateObjectCountParameter.Settings settings;
	}

	public struct Settings
	{
		public HashedString path;

		public int parameterIdx;

		public float minObjects;

		public float maxObjects;

		public bool useExponentialCurve;
	}
}
