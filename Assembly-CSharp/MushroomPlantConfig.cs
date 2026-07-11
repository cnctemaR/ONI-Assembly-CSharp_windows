using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class MushroomPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "MushroomPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.MUSHROOMPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.MUSHROOMPLANT.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("fungusplant_kanim"), "idle_empty", Grid.SceneLayer.BuildingFront, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 228.15f, 278.15f, 308.15f, 398.15f, new SimHashes[] { SimHashes.CarbonDioxide }, true, 0f, 0.15f, MushroomConfig.ID, true, true, true, true, 2400f);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.SlimeMold,
				massConsumptionRate = 0.006666667f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<IlluminationVulnerable>().SetPrefersDarkness(true);
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "MushroomSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.MUSHROOMPLANT.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.MUSHROOMPLANT.DESC, Assets.GetAnim("seed_fungusplant_kanim"), "object", 0, new List<Tag> { GameTags.CropSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, global::STRINGS.CREATURES.SPECIES.MUSHROOMPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.33f, 0.33f, null, "", false), "MushroomPlant_preview", Assets.GetAnim("fungusplant_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const float FERTILIZATION_RATE = 0.006666667f;

	public const string ID = "MushroomPlant";

	public const string SEED_ID = "MushroomSeed";
}
