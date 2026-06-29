using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PacuCleanerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = BasePacuConfig.CreatePrefab("PacuCleaner", "PacuCleanerBaseTrait", global::STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.NAME, global::STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.DESC, "glp_", 243.15f, 278.15f);
		gameObject = EntityTemplates.ExtendEntityToWildCreature(gameObject, PacuTuning.PEN_SIZE_PER_CREATURE, 25f);
		gameObject = EntityTemplates.ExtendEntityToFertileCreature(gameObject, "PacuCleanerEgg", global::STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.EGG_NAME, global::STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.DESC, "egg_pacu_kanim", "PacuCleaner", PacuTuning.EGG_CHANCES_CLEANER, PacuCleanerConfig.EGG_SORT_ORDER, false, true);
		Storage storage = gameObject.AddComponent<Storage>();
		storage.capacityKg = 10f;
		ElementConsumer elementConsumer = gameObject.AddOrGet<PassiveElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.DirtyWater;
		elementConsumer.consumptionRate = PacuCleanerConfig.POLLUTED_WATER_CONVERTED_PER_CYCLE / 600f;
		elementConsumer.capacityKG = 10f;
		elementConsumer.consumptionRadius = 3;
		elementConsumer.showInStatusPanel = true;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
		elementConsumer.isRequired = false;
		elementConsumer.storeOnConsume = true;
		elementConsumer.showDescriptor = false;
		BubbleSpawner bubbleSpawner = gameObject.AddComponent<BubbleSpawner>();
		bubbleSpawner.element = PacuCleanerConfig.OUTPUT_ELEMENT;
		bubbleSpawner.emitMass = 2f;
		bubbleSpawner.emitVariance = 0.5f;
		bubbleSpawner.initialVelocity = new Vector2f(0, 1);
		ElementConverter elementConverter = gameObject.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(PacuCleanerConfig.INPUT_ELEMENT.CreateTag(), PacuCleanerConfig.POLLUTED_WATER_CONVERTED_PER_CYCLE / 600f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(PacuCleanerConfig.POLLUTED_WATER_CONVERTED_PER_CYCLE / 600f, SimHashes.Water, 0f, true, 0f, 0.5f, false, 1f, byte.MaxValue, 0)
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		inst.GetComponent<ElementConsumer>().EnableConsumption(true);
	}

	public const string ID = "PacuCleaner";

	public const string BASE_TRAIT_ID = "PacuCleanerBaseTrait";

	public const string EGG_ID = "PacuCleanerEgg";

	public static float POLLUTED_WATER_CONVERTED_PER_CYCLE = 120f;

	public static SimHashes INPUT_ELEMENT = SimHashes.DirtyWater;

	public static SimHashes OUTPUT_ELEMENT = SimHashes.Water;

	public static EffectorValues DECOR = global::TUNING.BUILDINGS.DECOR.BONUS.TIER4;

	public static int EGG_SORT_ORDER = PacuConfig.EGG_SORT_ORDER + 1;
}
