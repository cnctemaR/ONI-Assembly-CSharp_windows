using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PrickleFlowerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "PrickleFlower";
		string text2 = global::STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.DESC;
		float num = 1f;
		KAnimFile kanimFile = Assets.GetAnim("bristleblossom_kanim");
		string text4 = "idle_empty";
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingFront, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 15f, 5f, 273.15f, 278.15f, 283.15f, 288.15f, 296.15f, 398.15f, new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, true, 0f, 0.15f, 1f, PrickleFruitConfig.ID, true);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new IrrigationMonitor.LiquidResourceInfo[]
		{
			new IrrigationMonitor.LiquidResourceInfo
			{
				tag = GameTags.Water,
				massConsumptionRate = 0.13333334f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		IlluminationVulnerable illuminationVulnerable = gameObject.UpdateComponentRequirement<IlluminationVulnerable>(true);
		illuminationVulnerable.Configure(false);
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		text4 = "PrickleFlowerSeed";
		text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.NAME;
		text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.DESC;
		kanimFile = Assets.GetAnim("seed_bristleblossom_kanim");
		int num2 = 0;
		List<Tag> list = new List<Tag> { GameTags.CropSeed };
		text = global::STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.DOMESTICATEDDESC;
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", num2, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, text, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "");
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "PrickleFlower_preview", Assets.GetAnim("bristleblossom_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_grow", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<PrimaryElement>().Temperature = 288.15f;
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const float WATER_RATE = 0.13333334f;

	public const string ID = "PrickleFlower";

	public const string SEED_ID = "PrickleFlowerSeed";

	public const int MID_YIELD_SEEDS = 1;

	public const int HIGH_YIELD_SEEDS = 1;
}
