using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicSingleHarvestPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "BasicSingleHarvestPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.DESC;
		float num = 1f;
		KAnimFile kanimFile = Assets.GetAnim("meallice_kanim");
		string text4 = "idle_empty";
		EffectorValues tier = DECOR.PENALTY.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingBack, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject gameObject2 = gameObject;
		SimHashes[] array = new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		};
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, 15f, 5f, 218.15f, 283.15f, 291.15f, 295.15f, 303.15f, 398.15f, array, true, 0f, 0.15f, "BasicPlantFood", true, false);
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<KAnimControllerBase>().randomiseLoopedOffset = true;
		gameObject.AddOrGet<LoopingSounds>();
		gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		text4 = "BasicSingleHarvestPlantSeed";
		text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.BASICSINGLEHARVESTPLANT.NAME;
		text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.BASICSINGLEHARVESTPLANT.DESC;
		kanimFile = Assets.GetAnim("seed_meallice_kanim");
		int num2 = 0;
		List<Tag> list = new List<Tag> { GameTags.CropSeed };
		text = global::STRINGS.CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.DOMESTICATEDDESC;
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", num2, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 1, text, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, string.Empty);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Dirt,
				massConsumptionRate = 0.016666668f
			}
		});
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "BasicSingleHarvestPlant_preview", Assets.GetAnim("meallice_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("meallice_kanim", "MealLice_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("meallice_kanim", "MealLice_LP", NOISE_POLLUTION.CREATURES.TIER4);
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

	public const float DIRT_RATE = 0.016666668f;
}
