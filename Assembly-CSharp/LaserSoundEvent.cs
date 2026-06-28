using System;

public class LaserSoundEvent : SoundEvent
{
	public LaserSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, min_interval, true)
	{
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		if (this.ShouldPlaySound(behaviour))
		{
			this.PlaySound(behaviour);
		}
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		base.PlaySound(behaviour);
	}
}
