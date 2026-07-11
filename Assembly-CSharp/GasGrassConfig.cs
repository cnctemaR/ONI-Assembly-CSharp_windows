using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GasGrassConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "GasGrass";
		string text2 = global::STRINGS.CREATURES.SPECIES.GASGRASS.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.GASGRASS.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER3;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("gassygrass_kanim"), "idle_empty", Grid.SceneLayer.BuildingFront, 1, 3, tier, default(EffectorValues), SimHashes.Creature, null, 255f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 218.15f, 0f, 348.15f, 373.15f, null, true, 0f, 0.15f, "GasGrassHarvested", true, true, true, true, 2400f);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Chlorine,
				massConsumptionRate = 0.00083333335f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<HarvestDesignatable>().defaultHarvestStateWhenPlanted = false;
		CropSleepingMonitor.Def def = gameObject.AddOrGetDef<CropSleepingMonitor.Def>();
		def.lightIntensityThreshold = 20000f;
		def.prefersDarkness = false;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "GasGrassSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.GASGRASS.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.GASGRASS.DESC, Assets.GetAnim("seed_gassygrass_kanim"), "object", 1, new List<Tag> { GameTags.CropSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, global::STRINGS.CREATURES.SPECIES.GASGRASS.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.2f, 0.2f, null, "", false), "GasGrass_preview", Assets.GetAnim("gassygrass_kanim"), "place", 1, 1);
		SoundEventVolumeCache.instance.AddVolume("gassygrass_kanim", "GasGrass_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("gassygrass_kanim", "GasGrass_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "GasGrass";

	public const string SEED_ID = "GasGrassSeed";

	public const float FERTILIZATION_RATE = 0.00083333335f;
}
