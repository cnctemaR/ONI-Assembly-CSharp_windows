using System;
using FMOD.Studio;
using UnityEngine;

public class HatchChewSoundEvent : SoundEvent
{
	public HatchChewSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, min_interval, true)
	{
	}

	public override void OnPlay(IAnimBehaviour behaviour)
	{
		Vector3 position = behaviour.GetComponent<Transform>().position;
		int audioCategory = HatchChewSoundEvent.GetAudioCategory(behaviour);
		EventInstance eventInstance = SoundEvent.BeginOneShot(this.sound, position);
		eventInstance.setParameterValue("material_ID", (float)audioCategory);
		SoundEvent.EndOneShot(eventInstance);
	}

	private static int GetAudioCategory(IAnimBehaviour behaviour)
	{
		Hatch component = behaviour.GetComponent<Hatch>();
		Element latestMealElement = component.latestMealElement;
		if (latestMealElement.id == SimHashes.Dirt)
		{
			return 0;
		}
		if (latestMealElement.id == SimHashes.CrushedIce)
		{
			return 1;
		}
		if (latestMealElement.HasTag(GameTags.IceOre))
		{
			return 1;
		}
		if (latestMealElement.id == SimHashes.OxyRock)
		{
			return 3;
		}
		if (latestMealElement.HasTag(GameTags.Metal))
		{
			return 5;
		}
		if (latestMealElement.HasTag(GameTags.RefinedMetal))
		{
			return 6;
		}
		if (latestMealElement.id == SimHashes.Sand)
		{
			return 8;
		}
		if (latestMealElement.id == SimHashes.Algae)
		{
			return 10;
		}
		return 7;
	}
}
