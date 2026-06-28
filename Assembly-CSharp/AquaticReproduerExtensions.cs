using System;
using UnityEngine;

public static class AquaticReproduerExtensions
{
	public static AquaticReproducer AddAquaticReproducer(this GameObject prefab, Tag effPrefab, float cycleLength, float randomCycleOffset, float maxPopulationDensity)
	{
		AquaticReproducer aquaticReproducer = prefab.AddOrGet<AquaticReproducer>();
		aquaticReproducer.cycleLength = cycleLength;
		aquaticReproducer.randomCycleOffset = randomCycleOffset;
		aquaticReproducer.maxmimumDensity = maxPopulationDensity;
		aquaticReproducer.EggPrefabTag = effPrefab;
		aquaticReproducer.SpawnEgg = true;
		return aquaticReproducer;
	}
}
