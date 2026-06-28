using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ColdWheatConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "ColdWheat";
		string text2 = global::STRINGS.CREATURES.SPECIES.COLDWHEAT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.COLDWHEAT.DESC;
		float num = 1f;
		KAnimFile kanimFile = Assets.GetAnim("coldwheat_kanim");
		string text4 = "idle_empty";
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingFront, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 255f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 15f, 5f, 188.15f, 218.15f, 233.15f, 238.15f, 278.15f, 358.15f, new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, true, 0f, 0.15f, 1f, "ColdWheatSeed", true);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new FertilizationMonitor.FertilizerInfo[]
		{
			new FertilizationMonitor.FertilizerInfo
			{
				tag = GameTags.Fertilizer,
				massConsumptionRate = 0.006666667f
			}
		});
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new IrrigationMonitor.LiquidResourceInfo[]
		{
			new IrrigationMonitor.LiquidResourceInfo
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
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.DigOnly;
		text4 = "ColdWheatSeed";
		text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.COLDWHEAT.NAME;
		text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.COLDWHEAT.DESC;
		kanimFile = Assets.GetAnim("seed_coldwheat_kanim");
		List<Tag> list = new List<Tag> { GameTags.CropSeed };
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, global::STRINGS.CREATURES.SPECIES.COLDWHEAT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.2f, 0.2f, null, string.Empty);
		EntityTemplates.ExtendEntityToFood(gameObject3, FOOD.FOOD_TYPES.COLD_WHEAT_SEED, true);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "ColdWheat_preview", Assets.GetAnim("coldwheat_kanim"), "place", 1, 1);
		SoundEventVolumeCache.instance.AddVolume("coldwheat_kanim", "ColdWheat_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("coldwheat_kanim", "ColdWheat_harvest", NOISE_POLLUTION.CREATURES.TIER3);
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
