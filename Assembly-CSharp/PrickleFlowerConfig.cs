using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PrickleFlowerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PrickleFlower", global::STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.NAME, global::STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.DESC, 400f, Assets.GetAnim("bristleblossom_kanim"), "idle_empty", Grid.SceneLayer.BuildingFront, 1, 2, tier, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 15f, 5f, 218.15f, 250.15f, 270.15f, 276.15f, 296.15f, 398.15f, 0f, 0.15f, 1f, PrickleFruitConfig.ID);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new FertilizationMonitor.FertilizerInfo[]
		{
			new FertilizationMonitor.FertilizerInfo
			{
				tag = GameTags.Fertilizer,
				massConsumptionRate = 0.006666667f
			}
		});
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new FertilizationMonitor.FertilizerInfo[]
		{
			new FertilizationMonitor.FertilizerInfo
			{
				tag = GameTags.Water,
				massConsumptionRate = 0.033333335f
			}
		});
		EntityTemplates.ExtendPlantWithYield(gameObject, new IYieldEffect[]
		{
			new YieldEffect.AddSeeds(1)
		}, new IYieldEffect[]
		{
			new YieldEffect.AddSeeds(1)
		});
		gameObject.AddOrGet<StandardCropPlant>();
		string text = global::STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.DOMESTICATEDDESC;
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.FinalHarvest, "PrickleFlowerSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.DESC, Assets.GetAnim("seed_bristleblossom_kanim"), "object", 0, new List<Tag> { GameTags.CropSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, text, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject, gameObject2, "PrickleFlower_preview", Assets.GetAnim("bristleblossom_kanim"), "place", 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const float FERTILIZATION_RATE = 0.006666667f;

	public const float WATER_RATE = 0.033333335f;

	public const string ID = "PrickleFlower";

	public const string SEED_ID = "PrickleFlowerSeed";

	public const int MID_YIELD_SEEDS = 1;

	public const int HIGH_YIELD_SEEDS = 1;
}
