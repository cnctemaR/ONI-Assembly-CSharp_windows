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
		KAnimFile kanimFile = Assets.GetAnim("fungusplant_kanim");
		string text4 = "idle_empty";
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingFront, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 15f, 5f, 268.15f, 278.15f, 293.15f, 296.15f, 308.15f, 318.15f, new SimHashes[] { SimHashes.CarbonDioxide }, true, 0f, 0.15f, MushroomConfig.ID, true, true);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.SlimeMold,
				massConsumptionRate = 0.006666667f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		IlluminationVulnerable illuminationVulnerable = gameObject.UpdateComponentRequirement<IlluminationVulnerable>(true);
		illuminationVulnerable.Configure(true);
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		text4 = "MushroomSeed";
		text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.MUSHROOMPLANT.NAME;
		text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.MUSHROOMPLANT.DESC;
		kanimFile = Assets.GetAnim("seed_fungusplant_kanim");
		int num2 = 0;
		List<Tag> list = new List<Tag> { GameTags.CropSeed };
		text = global::STRINGS.CREATURES.SPECIES.MUSHROOMPLANT.DOMESTICATEDDESC;
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", num2, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, text, EntityTemplates.CollisionShape.CIRCLE, 0.33f, 0.33f, null, string.Empty);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "MushroomPlant_preview", Assets.GetAnim("fungusplant_kanim"), "place", 1, 2);
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
