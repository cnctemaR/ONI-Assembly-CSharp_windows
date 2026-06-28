using System;
using Klei;
using UnityEngine;

public static class CommonEntityTypeSetExtents
{
	public static ElementEmitter AddElementEmitter(this GameObject prefab, SimHashes element, float emission_frequency, float emission_mass, SimUtil.DiseaseInfo disease)
	{
		ElementEmitter elementEmitter = prefab.UpdateComponentRequirement<ElementEmitter>(true);
		elementEmitter.outputElement = new ElementConverter.OutputElement(emission_mass / emission_frequency, element, 0f, false, 0f, 0.5f, false, 1f, byte.MaxValue, 0);
		elementEmitter.emissionFrequency = emission_frequency;
		return elementEmitter;
	}

	public static Weapon AddWeapon(this GameObject prefab, float base_damage_min, float base_damage_max, AttackProperties.DamageType attackType = AttackProperties.DamageType.Standard, AttackProperties.TargetType targetType = AttackProperties.TargetType.Single, int maxHits = 1, float aoeRadius = 0f)
	{
		Weapon weapon = prefab.UpdateComponentRequirement<Weapon>(true);
		weapon.Configure(base_damage_min, base_damage_max, attackType, targetType, maxHits, aoeRadius);
		return weapon;
	}

	public static AquaticReproducer AddAquaticReproducer(this GameObject prefab, Tag effPrefab, float cycleLength, float randomCycleOffset, float maxPopulationDensity)
	{
		AquaticReproducer aquaticReproducer = prefab.UpdateComponentRequirement<AquaticReproducer>(true);
		aquaticReproducer.cycleLength = cycleLength;
		aquaticReproducer.randomCycleOffset = randomCycleOffset;
		aquaticReproducer.maxmimumDensity = maxPopulationDensity;
		aquaticReproducer.EggPrefabTag = effPrefab;
		aquaticReproducer.SpawnEgg = true;
		return aquaticReproducer;
	}
}
