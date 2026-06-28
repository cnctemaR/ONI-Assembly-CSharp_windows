using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicFabricMaterialPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(BasicFabricMaterialPlantConfig.ID, global::STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.NAME, global::STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.DESC, 400f, Assets.GetAnim("swampreed_kanim"), "idle_empty", Grid.SceneLayer.BuildingBack, 1, 3, tier, SimHashes.Creature, null);
		float num = 258.15f;
		float num2 = 298.15f;
		float num3 = 318.15f;
		float num4 = 423.15f;
		float num5 = 1f;
		string crop_id = "BasicFabric";
		gameObject.UpdateComponentRequirement<EntombVulnerable>(true);
		gameObject.UpdateComponentRequirement<WiltCondition>(true);
		gameObject.UpdateComponentRequirement<Uprootable>(true);
		gameObject.UpdateComponentRequirement<UprootedMonitor>(true);
		SubmersionMonitor submersionMonitor = gameObject.UpdateComponentRequirement<SubmersionMonitor>(true);
		submersionMonitor.Configure(1f, 1f, 0.2f);
		TemperatureVulnerable temperatureVulnerable = gameObject.UpdateComponentRequirement<TemperatureVulnerable>(true);
		temperatureVulnerable.Configure(num2, num, num3, num4, 0f, 0f);
		gameObject.UpdateComponentRequirement<OccupyArea>(true).objectLayer = ObjectLayer.Building;
		Growing growing = gameObject.UpdateComponentRequirement<Growing>(true);
		growing.Configure(num5, num5);
		if (crop_id != null)
		{
			Crop.CropVal cropVal = CROPS.CROP_TYPES.Find((Crop.CropVal m) => m.cropId == crop_id);
			Crop crop = gameObject.UpdateComponentRequirement<Crop>(true);
			crop.Configure(cropVal);
			growing.Configure(cropVal.cropDuration, cropVal.regrowDuration);
			gameObject.UpdateComponentRequirement<Harvestable>(true);
			gameObject.UpdateComponentRequirement<Prioritizable>(true);
		}
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.UpdateComponentRequirement<KAnimControllerBase>(true).randomiseLoopedOffset = true;
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		string text = global::STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.DOMESTICATEDDESC;
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "BasicFabricMaterialPlantSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.BASICFABRICMATERIALPLANT.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.BASICFABRICMATERIALPLANT.DESC, Assets.GetAnim("seed_swampreed_kanim"), "object", 0, new List<Tag> { GameTags.WaterSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 1, text, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject, gameObject2, BasicFabricMaterialPlantConfig.ID + "_preview", Assets.GetAnim("swampreed_kanim"), "place", 1, 3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static string ID = "BasicFabricPlant";

	public static float FERTILIZATION_RATE = 0.033333335f;
}
