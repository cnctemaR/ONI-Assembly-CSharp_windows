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
		GameObject gameObject2 = gameObject;
		num = 218.15f;
		float num2 = 278.15f;
		float num3 = 303.15f;
		float num4 = 398.15f;
		SimHashes[] array = new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		};
		text4 = PrickleFruitConfig.ID;
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num, num2, num3, num4, array, true, 0f, 0.15f, text4, true, true);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Water,
				massConsumptionRate = 0.033333335f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		DiseaseDropper.Def def = gameObject.AddOrGetDef<DiseaseDropper.Def>();
		def.diseaseIdx = Db.Get().Diseases.GetIndex(Db.Get().Diseases.PollenGerms.id);
		def.singleEmitQuantity = 1000000;
		IlluminationVulnerable illuminationVulnerable = gameObject.AddOrGet<IlluminationVulnerable>();
		illuminationVulnerable.SetPrefersDarkness(false);
		gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		text4 = "PrickleFlowerSeed";
		text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.NAME;
		text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.DESC;
		kanimFile = Assets.GetAnim("seed_bristleblossom_kanim");
		int num5 = 0;
		List<Tag> list = new List<Tag> { GameTags.CropSeed };
		text = global::STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.DOMESTICATEDDESC;
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", num5, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, text, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, string.Empty, false);
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

	public const float WATER_RATE = 0.033333335f;

	public const string ID = "PrickleFlower";

	public const string SEED_ID = "PrickleFlowerSeed";
}
