using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MainMenuSoundEvent : AnimEvent
{
	public MainMenuSoundEvent(string file_name, string sound_name, int frame)
		: base(file_name, sound_name, frame)
	{
		this.sound = GlobalAssets.GetSound(sound_name, false);
		if (this.sound == null || this.sound == string.Empty)
		{
		}
		this.frameNumber = frame;
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		this.PlaySound(behaviour);
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		EventInstance eventInstance = KFMOD.BeginOneShot(this.sound, Vector3.zero);
		if (eventInstance != null)
		{
			eventInstance.setParameterValue("frame", (float)this.frameNumber);
			KFMOD.EndOneShot(eventInstance);
		}
	}

	[EventRef]
	public string sound;

	public int frameNumber;
}
