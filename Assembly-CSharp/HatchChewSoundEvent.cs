using System;
using FMOD.Studio;
using UnityEngine;

public class HatchChewSoundEvent : SoundEvent
{
	public HatchChewSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, true, true, min_interval, false)
	{
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		if (base.ShouldPlaySound(behaviour, false))
		{
			this.PlaySound(behaviour);
		}
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 position = behaviour.GetComponent<Transform>().position;
		int audioCategory = HatchChewSoundEvent.GetAudioCategory(behaviour);
		EventInstance eventInstance = SoundEvent.BeginOneShot(base.sound, position);
		eventInstance.setParameterValue("material_ID", (float)audioCategory);
		SoundEvent.EndOneShot(eventInstance);
	}

	private static int GetAudioCategory(AnimEventManager.EventPlayerData behaviour)
	{
		Hatch component = behaviour.GetComponent<Hatch>();
		Element latestMealElement = component.latestMealElement;
		int num;
		if (latestMealElement.id == SimHashes.Dirt)
		{
			num = 0;
		}
		else if (latestMealElement.id == SimHashes.CrushedIce)
		{
			num = 1;
		}
		else if (latestMealElement.HasTag(GameTags.IceOre))
		{
			num = 1;
		}
		else if (latestMealElement.id == SimHashes.OxyRock)
		{
			num = 3;
		}
		else if (latestMealElement.HasTag(GameTags.Metal))
		{
			num = 5;
		}
		else if (latestMealElement.HasTag(GameTags.RefinedMetal))
		{
			num = 6;
		}
		else if (latestMealElement.id == SimHashes.Sand)
		{
			num = 8;
		}
		else if (latestMealElement.id == SimHashes.Algae)
		{
			num = 10;
		}
		else
		{
			num = 7;
		}
		return num;
	}
}
