using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicFabricMaterialPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string id = BasicFabricMaterialPlantConfig.ID;
		string text = global::STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.NAME;
		string text2 = global::STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, text, text2, num, Assets.GetAnim("swampreed_kanim"), "idle_empty", Grid.SceneLayer.BuildingBack, 1, 3, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject gameObject2 = gameObject;
		float num2 = 248.15f;
		float num3 = 295.15f;
		float num4 = 310.15f;
		float num5 = 398.15f;
		string id2 = BasicFabricConfig.ID;
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num2, num3, num4, num5, new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide,
			SimHashes.DirtyWater,
			SimHashes.Water
		}, false, 0f, 0.15f, id2, false, true, true, true, 2400f);
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
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, BasicFabricMaterialPlantConfig.SEED_ID, global::STRINGS.CREATURES.SPECIES.SEEDS.BASICFABRICMATERIALPLANT.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.BASICFABRICMATERIALPLANT.DESC, Assets.GetAnim("seed_swampreed_kanim"), "object", 0, new List<Tag> { GameTags.WaterSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 1, global::STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false), BasicFabricMaterialPlantConfig.ID + "_preview", Assets.GetAnim("swampreed_kanim"), "place", 1, 3);
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

	public static string SEED_ID = "BasicFabricMaterialPlantSeed";

	public const float WATER_RATE = 0.26666668f;
}
