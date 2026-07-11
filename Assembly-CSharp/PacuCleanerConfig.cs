using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PacuCleanerConfig : IEntityConfig
{
	public static GameObject CreatePacu(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BasePacuConfig.CreatePrefab(id, "PacuCleanerBaseTrait", name, desc, anim_file, is_baby, "glp_", 243.15f, 278.15f);
		gameObject = EntityTemplates.ExtendEntityToWildCreature(gameObject, PacuTuning.PEN_SIZE_PER_CREATURE, 25f);
		if (!is_baby)
		{
			gameObject.AddComponent<Storage>().capacityKg = 10f;
			PassiveElementConsumer passiveElementConsumer = gameObject.AddOrGet<PassiveElementConsumer>();
			passiveElementConsumer.elementToConsume = SimHashes.DirtyWater;
			passiveElementConsumer.consumptionRate = 0.2f;
			passiveElementConsumer.capacityKG = 10f;
			passiveElementConsumer.consumptionRadius = 3;
			passiveElementConsumer.showInStatusPanel = true;
			passiveElementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
			passiveElementConsumer.isRequired = false;
			passiveElementConsumer.storeOnConsume = true;
			passiveElementConsumer.showDescriptor = false;
			gameObject.AddOrGet<UpdateElementConsumerPosition>();
			BubbleSpawner bubbleSpawner = gameObject.AddComponent<BubbleSpawner>();
			bubbleSpawner.element = SimHashes.Water;
			bubbleSpawner.emitMass = 2f;
			bubbleSpawner.emitVariance = 0.5f;
			bubbleSpawner.initialVelocity = new Vector2f(0, 1);
			ElementConverter elementConverter = gameObject.AddOrGet<ElementConverter>();
			elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
			{
				new ElementConverter.ConsumedElement(SimHashes.DirtyWater.CreateTag(), 0.2f)
			};
			elementConverter.outputElements = new ElementConverter.OutputElement[]
			{
				new ElementConverter.OutputElement(0.2f, SimHashes.Water, 0f, true, true, 0f, 0.5f, 1f, byte.MaxValue, 0)
			};
		}
		return gameObject;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFertileCreature(EntityTemplates.ExtendEntityToWildCreature(PacuCleanerConfig.CreatePacu("PacuCleaner", global::STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.NAME, global::STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.DESC, "pacu_kanim", false), PacuTuning.PEN_SIZE_PER_CREATURE, 25f), "PacuCleanerEgg", global::STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.EGG_NAME, global::STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.DESC, "egg_pacu_kanim", PacuTuning.EGG_MASS, "PacuCleanerBaby", 15.000001f, 5f, PacuTuning.EGG_CHANCES_CLEANER, 501, false, true, false, 0.75f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		ElementConsumer component = inst.GetComponent<ElementConsumer>();
		if (component != null)
		{
			component.EnableConsumption(true);
		}
	}

	public const string ID = "PacuCleaner";

	public const string BASE_TRAIT_ID = "PacuCleanerBaseTrait";

	public const string EGG_ID = "PacuCleanerEgg";

	public const float POLLUTED_WATER_CONVERTED_PER_CYCLE = 120f;

	public const SimHashes INPUT_ELEMENT = SimHashes.DirtyWater;

	public const SimHashes OUTPUT_ELEMENT = SimHashes.Water;

	public static readonly EffectorValues DECOR = global::TUNING.BUILDINGS.DECOR.BONUS.TIER4;

	public const int EGG_SORT_ORDER = 501;
}
