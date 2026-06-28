using System;
using System.Collections.Generic;

public class SoundEventVolumeCache
{
	public static SoundEventVolumeCache instance
	{
		get
		{
			return Singleton<SoundEventVolumeCache>.Instance;
		}
	}

	public void AddVolume(string animFile, string eventName, EffectorValues vals)
	{
		HashedString hashedString = new HashedString(animFile + ":" + eventName);
		if (!this.volumeCache.ContainsKey(hashedString))
		{
			this.volumeCache.Add(hashedString, vals);
		}
		else
		{
			this.volumeCache[hashedString] = vals;
		}
	}

	public EffectorValues GetVolume(string animFile, string eventName)
	{
		HashedString hashedString = new HashedString(animFile + ":" + eventName);
		if (!this.volumeCache.ContainsKey(hashedString))
		{
			return default(EffectorValues);
		}
		return this.volumeCache[hashedString];
	}

	public Dictionary<HashedString, EffectorValues> volumeCache = new Dictionary<HashedString, EffectorValues>();
}
