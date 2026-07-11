using System;

public class SpeedOneShotUpdater : OneShotSoundParameterUpdater
{
	public SpeedOneShotUpdater()
		: base("Speed")
	{
	}

	public override void Play(OneShotSoundParameterUpdater.Sound sound)
	{
		sound.ev.setParameterByID(sound.description.GetParameterId(base.parameter), SpeedLoopingSoundUpdater.GetSpeedParameterValue(), false);
	}
}
