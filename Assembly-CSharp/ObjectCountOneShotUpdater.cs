using System;
using System.Collections.Generic;

internal class ObjectCountOneShotUpdater : OneShotSoundParameterUpdater
{
	public ObjectCountOneShotUpdater()
		: base("objectCount")
	{
	}

	public override void Update(float dt)
	{
		this.soundCounts.Clear();
	}

	public override void Play(OneShotSoundParameterUpdater.Sound sound)
	{
		UpdateObjectCountParameter.Settings settings = UpdateObjectCountParameter.GetSettings(sound.path, sound.description);
		int num = 0;
		this.soundCounts.TryGetValue(sound.path, out num);
		num = (this.soundCounts[sound.path] = num + 1);
		UpdateObjectCountParameter.ApplySettings(sound.ev, num, settings);
	}

	private Dictionary<HashedString, int> soundCounts = new Dictionary<HashedString, int>();
}
