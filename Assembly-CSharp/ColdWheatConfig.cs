using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ColdWheatConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("ColdWheat", global::STRINGS.CREATURES.SPECIES.COLDWHEAT.NAME, global::STRINGS.CREATURES.SPECIES.COLDWHEAT.DESC, 400f, Assets.GetAnim("coldwheat_kanim"), "idle_empty", Grid.SceneLayer.BuildingFront, 1, 1, tier, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 15f, 5f, 188.15f, 218.15f, 233.15f, 238.15f, 278.15f, 358.15f, 0f, 0.15f, 1f, "ColdWheatSeed");
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
			new YieldEffect.AddHarvestUnitsMultiple(1f)
		}, new IYieldEffect[]
		{
			new YieldEffect.AddHarvestUnitsMultiple(1f)
		});
		gameObject.AddOrGet<StandardCropPlant>();
		List<Tag> list = new List<Tag> { GameTags.CropSeed };
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.DigOnly, "ColdWheatSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.COLDWHEAT.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.COLDWHEAT.DESC, Assets.GetAnim("seed_coldwheat_kanim"), "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, global::STRINGS.CREATURES.SPECIES.COLDWHEAT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.2f, 0.2f);
		EntityTemplates.ExtendEntityToFood(gameObject2, FOOD.FOOD_TYPES.COLD_WHEAT_SEED, true);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject, gameObject2, "ColdWheat_preview", Assets.GetAnim("coldwheat_kanim"), "place", 1, 1);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ColdWheat";

	public const string SEED_ID = "ColdWheatSeed";

	public const float FERTILIZATION_RATE = 0.006666667f;

	public const float WATER_RATE = 0.033333335f;

	public const float MID_YIELD_BONUS = 1f;

	public const float HIGH_YIELD_BONUS = 1f;
}
