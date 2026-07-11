using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SeaLettuceConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = SeaLettuceConfig.ID;
		string text2 = global::STRINGS.CREATURES.SPECIES.SEALETTUCE.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SEALETTUCE.DESC;
		float num = 1f;
		KAnimFile kanimFile = Assets.GetAnim("sea_lettuce_kanim");
		string text4 = "idle_empty";
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingBack, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 308.15f);
		GameObject gameObject2 = gameObject;
		num = 248.15f;
		float num2 = 295.15f;
		float num3 = 338.15f;
		float num4 = 398.15f;
		bool flag = false;
		SimHashes[] array = new SimHashes[]
		{
			SimHashes.Water,
			SimHashes.SaltWater,
			SimHashes.Brine
		};
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num, num2, num3, num4, array, flag, 0f, 0.15f, "Lettuce", true, true, true, true, 2400f);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.SaltWater.CreateTag(),
				massConsumptionRate = 0.008333334f
			}
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.BleachStone.CreateTag(),
				massConsumptionRate = 0.00083333335f
			}
		});
		gameObject.GetComponent<DrowningMonitor>().canDrownToDeath = false;
		gameObject.GetComponent<DrowningMonitor>().livesUnderWater = true;
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<KAnimControllerBase>().randomiseLoopedOffset = true;
		gameObject.AddOrGet<LoopingSounds>();
		gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		text4 = SeaLettuceConfig.ID + "Seed";
		text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.SEALETTUCE.NAME;
		text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.SEALETTUCE.DESC;
		kanimFile = Assets.GetAnim("seed_sealettuce_kanim");
		int num5 = 0;
		List<Tag> list = new List<Tag> { GameTags.WaterSeed };
		text = global::STRINGS.CREATURES.SPECIES.SEALETTUCE.DOMESTICATEDDESC;
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", num5, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 1, text, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, string.Empty, false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, SeaLettuceConfig.ID + "_preview", Assets.GetAnim("sea_lettuce_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("sea_lettuce_kanim", "SeaLettuce_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("sea_lettuce_kanim", "SeaLettuce_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static string ID = "SeaLettuce";

	public const float WATER_RATE = 0.008333334f;

	public const float FERTILIZATION_RATE = 0.00083333335f;
}
