using System;

internal abstract class UserVolumeOneShotUpdater : OneShotSoundParameterUpdater
{
	public UserVolumeOneShotUpdater(string parameter, string player_pref)
		: base(parameter)
	{
		this.playerPref = player_pref;
	}

	public override void Play(OneShotSoundParameterUpdater.Sound sound)
	{
		if (!string.IsNullOrEmpty(this.playerPref))
		{
			float @float = KPlayerPrefs.GetFloat(this.playerPref);
			sound.ev.setParameterValueByIndex(sound.description.GetParameterIdx(base.parameter), @float);
		}
	}

	private string playerPref;
}
