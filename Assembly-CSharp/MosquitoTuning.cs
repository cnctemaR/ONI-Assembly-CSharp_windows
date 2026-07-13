using System;
using System.Collections.Generic;

public static class MosquitoTuning
{
	public const float BASE_EGG_DROP_TIME = 0.9f;

	public const float EGG_MASS = 1f;

	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "MosquitoEgg".ToTag(),
			weight = 1f
		}
	};
}
