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

	public void Play(IAnimBehaviour behaviour, string sound)
	{
		EventInstance eventInstance = KFMOD.BeginOneShot(sound, Vector3.zero);
		eventInstance.setParameterValue("frame", (float)this.frameNumber);
		KFMOD.EndOneShot(eventInstance);
	}

	public override void OnPlay(IAnimBehaviour behaviour)
	{
		this.Play(behaviour, this.sound);
	}

	[EventRef]
	public string sound;

	public int frameNumber;
}
