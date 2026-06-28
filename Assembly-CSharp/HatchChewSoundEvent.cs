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
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		int audioCategory = HatchChewSoundEvent.GetAudioCategory(behaviour);
		EventInstance eventInstance = SoundEvent.BeginOneShot(base.sound, position);
		eventInstance.setParameterValue("material_ID", (float)audioCategory);
		SoundEvent.EndOneShot(eventInstance);
	}

	private static int GetAudioCategory(AnimEventManager.EventPlayerData behaviour)
	{
		Element element = null;
		if (element == null)
		{
			EatStates.Instance smi = behaviour.controller.GetSMI<EatStates.Instance>();
			if (smi != null)
			{
				smi.GetLatestMealElement();
			}
		}
		if (element == null)
		{
			return 0;
		}
		if (element.id == SimHashes.Dirt)
		{
			return 0;
		}
		if (element.id == SimHashes.CrushedIce)
		{
			return 1;
		}
		if (element.HasTag(GameTags.IceOre))
		{
			return 1;
		}
		if (element.id == SimHashes.OxyRock)
		{
			return 3;
		}
		if (element.HasTag(GameTags.Metal))
		{
			return 5;
		}
		if (element.HasTag(GameTags.RefinedMetal))
		{
			return 6;
		}
		if (element.id == SimHashes.Sand)
		{
			return 8;
		}
		if (element.id == SimHashes.Algae)
		{
			return 10;
		}
		return 7;
	}
}
