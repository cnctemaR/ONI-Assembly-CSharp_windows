using System;

public class LaserSoundEvent : SoundEvent
{
	public LaserSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, min_interval, true)
	{
	}

	public override void OnPlay(IAnimBehaviour behaviour)
	{
		string sound = this.sound;
		base.Play(behaviour, sound);
	}
}
