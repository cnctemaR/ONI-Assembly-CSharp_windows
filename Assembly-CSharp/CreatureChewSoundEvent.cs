using System;
using FMOD.Studio;
using UnityEngine;

public class CreatureChewSoundEvent : SoundEvent
{
	public CreatureChewSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, false, false, min_interval, true)
	{
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		string sound = GlobalAssets.GetSound(StringFormatter.Combine(base.name, "_", CreatureChewSoundEvent.GetChewSound(behaviour)), false);
		GameObject gameObject = behaviour.controller.gameObject;
		base.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
		if (base.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, sound, base.looping, this.isDynamic))
		{
			Vector3 vector = behaviour.GetComponent<Transform>().GetPosition();
			vector.z = 0f;
			if (base.objectIsSelectedAndVisible)
			{
				vector = SoundEvent.AudioHighlightListenerPosition(vector);
			}
			EventInstance eventInstance = SoundEvent.BeginOneShot(sound, vector, SoundEvent.GetVolume(base.objectIsSelectedAndVisible), false);
			if (behaviour.controller.gameObject.GetDef<BabyMonitor.Def>() != null)
			{
				eventInstance.setParameterValue("isBaby", 1f);
			}
			SoundEvent.EndOneShot(eventInstance);
		}
	}

	private static string GetChewSound(AnimEventManager.EventPlayerData behaviour)
	{
		string text = CreatureChewSoundEvent.DEFAULT_CHEW_SOUND;
		EatStates.Instance smi = behaviour.controller.GetSMI<EatStates.Instance>();
		if (smi != null)
		{
			Element latestMealElement = smi.GetLatestMealElement();
			if (latestMealElement != null)
			{
				string creatureChewSound = latestMealElement.substance.GetCreatureChewSound();
				if (!string.IsNullOrEmpty(creatureChewSound))
				{
					text = creatureChewSound;
				}
			}
		}
		return text;
	}

	private static string DEFAULT_CHEW_SOUND = "Rock";

	private const string FMOD_PARAM_IS_BABY_ID = "isBaby";
}
