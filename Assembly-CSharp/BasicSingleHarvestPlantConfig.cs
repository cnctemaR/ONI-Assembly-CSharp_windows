using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicSingleHarvestPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.PENALTY.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("BasicSingleHarvestPlant", global::STRINGS.CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.NAME, global::STRINGS.CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.DESC, 400f, Assets.GetAnim("meallice_kanim"), "idle_empty", Grid.SceneLayer.BuildingBack, 1, 2, tier, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 15f, 5f, 218.15f, 283.15f, 291.15f, 295.15f, 303.15f, 398.15f, 0f, 0.15f, 1f, "BasicPlantFood");
		Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new FertilizationMonitor.FertilizerInfo[]
		{
			new FertilizationMonitor.FertilizerInfo
			{
				tag = GameTags.Fertilizer,
				massConsumptionRate = 0.0033333334f
			}
		});
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new FertilizationMonitor.FertilizerInfo[]
		{
			new FertilizationMonitor.FertilizerInfo
			{
				tag = tag,
				massConsumptionRate = 0.06666667f
			}
		});
		EntityTemplates.ExtendPlantWithYield(gameObject, new IYieldEffect[]
		{
			new YieldEffect.AddHarvestUnitsMultiple(1f)
		}, new IYieldEffect[]
		{
			new YieldEffect.AddSeeds(2)
		});
		gameObject.UpdateComponentRequirement<StandardCropPlant>(true);
		gameObject.UpdateComponentRequirement<KAnimControllerBase>(true).randomiseLoopedOffset = true;
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		string text = global::STRINGS.CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.DOMESTICATEDDESC;
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "BasicSingleHarvestPlantSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.BASICSINGLEHARVESTPLANT.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.BASICSINGLEHARVESTPLANT.DESC, Assets.GetAnim("seed_meallice_kanim"), "object", 0, new List<Tag> { GameTags.CropSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 1, text, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject, gameObject2, "BasicSingleHarvestPlant_preview", Assets.GetAnim("meallice_kanim"), "place", 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "BasicSingleHarvestPlant";

	public const string SEED_ID = "BasicSingleHarvestPlantSeed";

	public const float MID_YIELD_BONUS = 1f;

	public const int HIGH_YIELD_SEEDS = 2;

	public const float FERTILIZATION_RATE = 0.0033333334f;

	public const float WATER_RATE = 0.06666667f;
}
