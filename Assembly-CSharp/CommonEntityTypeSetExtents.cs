using System;
using UnityEngine;

public static class CommonEntityTypeSetExtents
{
	public static KAnimControllerBase AddKAnimController(this GameObject prefab, string anims)
	{
		KAnimControllerBase kanimControllerBase = prefab.UpdateComponentRequirement<KAnimControllerBase>(true);
		kanimControllerBase.AddAnims(new KAnimFile[] { Assets.GetAnim(anims) });
		return kanimControllerBase;
	}

	public static KBatchedAnimController AddKBatchedAnimController(this GameObject prefab, string anims)
	{
		KBatchedAnimController kbatchedAnimController = prefab.UpdateComponentRequirement<KBatchedAnimController>(true);
		kbatchedAnimController.AddAnims(new KAnimFile[] { Assets.GetAnim(anims) });
		return kbatchedAnimController;
	}

	public static KBatchedAnimController AddAnimController(this GameObject prefab, string KAnimName, Grid.SceneLayer layer)
	{
		KBatchedAnimController kbatchedAnimController = prefab.AddKBatchedAnimController(KAnimName);
		kbatchedAnimController.sceneLayer = layer;
		return kbatchedAnimController;
	}

	public static ElementConsumer AddElementConsumer(this GameObject prefab, SimHashes element, float consumption_rate, float minimum_mass, byte radius = 1)
	{
		ElementConsumer elementConsumer = prefab.UpdateComponentRequirement<ElementConsumer>(true);
		elementConsumer.elementToConsume = element;
		elementConsumer.consumptionRate = consumption_rate;
		elementConsumer.minimumMass = minimum_mass;
		elementConsumer.consumptionRadius = radius;
		return elementConsumer;
	}

	public static ElementEmitter AddElementEmitter(this GameObject prefab, SimHashes element, float emission_frequency, float emission_mass)
	{
		ElementEmitter elementEmitter = prefab.UpdateComponentRequirement<ElementEmitter>(true);
		elementEmitter.outputElement = new ElementConverter.OutputElement(null, emission_mass, element, 0f, false, 0f, 0f);
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

	public static Butcherable AddButcherable(this GameObject prefab, string[] DropPrefabs)
	{
		Butcherable butcherable = prefab.UpdateComponentRequirement<Butcherable>(true);
		butcherable.SetDrops(DropPrefabs);
		return butcherable;
	}

	public static InfoDescription AddDescription(this GameObject prefab, string description)
	{
		InfoDescription infoDescription = prefab.UpdateComponentRequirement<InfoDescription>(true);
		infoDescription.description = description;
		return infoDescription;
	}
}
