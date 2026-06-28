using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PrickleFlowerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PrickleFlower", global::STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.NAME, global::STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.DESC, 1f, Assets.GetAnim("bristleblossom_kanim"), "idle_empty", Grid.SceneLayer.BuildingFront, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
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
		string text = global::STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.DOMESTICATEDDESC;
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "PrickleFlowerSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.DESC, Assets.GetAnim("seed_bristleblossom_kanim"), "object", 0, new List<Tag> { GameTags.CropSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, text, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, string.Empty);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject2, "PrickleFlower_preview", Assets.GetAnim("bristleblossom_kanim"), "place", 1, 2);
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
