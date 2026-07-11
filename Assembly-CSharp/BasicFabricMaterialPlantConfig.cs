using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicFabricMaterialPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = BasicFabricMaterialPlantConfig.ID;
		string text2 = global::STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.DESC;
		float num = 1f;
		KAnimFile kanimFile = Assets.GetAnim("swampreed_kanim");
		string text4 = "idle_empty";
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingBack, 1, 3, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject gameObject2 = gameObject;
		text4 = BasicFabricConfig.ID;
		SimHashes[] array = new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide,
			SimHashes.DirtyWater,
			SimHashes.Water
		};
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, 248.15f, 295.15f, 310.15f, 398.15f, array, false, 0f, 0.15f, text4, false, true);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.DirtyWater,
				massConsumptionRate = 0.26666668f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<KAnimControllerBase>().randomiseLoopedOffset = true;
		gameObject.AddOrGet<LoopingSounds>();
		gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		text4 = "BasicFabricMaterialPlantSeed";
		text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.BASICFABRICMATERIALPLANT.NAME;
		text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.BASICFABRICMATERIALPLANT.DESC;
		kanimFile = Assets.GetAnim("seed_swampreed_kanim");
		int num2 = 0;
		List<Tag> list = new List<Tag> { GameTags.WaterSeed };
		text = global::STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.DOMESTICATEDDESC;
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", num2, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 1, text, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, string.Empty, false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, BasicFabricMaterialPlantConfig.ID + "_preview", Assets.GetAnim("swampreed_kanim"), "place", 1, 3);
		SoundEventVolumeCache.instance.AddVolume("swampreed_kanim", "FabricPlant_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("swampreed_kanim", "FabricPlant_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static string ID = "BasicFabricPlant";

	public const float WATER_RATE = 0.26666668f;
}
