using System;

public class SpeedOneShotUpdater : OneShotSoundParameterUpdater
{
	public SpeedOneShotUpdater()
		: base("Speed")
	{
	}

	public override void Play(OneShotSoundParameterUpdater.Sound sound)
	{
		sound.ev.setParameterValueByIndex(sound.description.GetParameterIdx(base.parameter), SpeedLoopingSoundUpdater.GetSpeedParameterValue());
	}
}
