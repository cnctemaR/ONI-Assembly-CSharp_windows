using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ButterflyPlantConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC4;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		string text = "ButterflyPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.BUTTERFLYPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.BUTTERFLYPLANT.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("pollinator_plant_kanim"), "idle_empty", Grid.SceneLayer.BuildingBack, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 233.15f, 283.15f, 318.15f, 353.15f, new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide,
			SimHashes.ChlorineGas
		}, true, 0f, 0.15f, "Butterfly", true, true, true, true, 2400f, 0f, 7400f, "ButterflyPlantOriginal", global::STRINGS.CREATURES.SPECIES.BUTTERFLYPLANT.NAME);
		global::UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<MutantPlant>());
		global::UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<HarvestDesignatable>());
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Dirt,
				massConsumptionRate = 0.016666668f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<LoopingSounds>();
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Crop;
		string text4 = "ButterflyPlantSeed";
		string text5 = global::STRINGS.CREATURES.SPECIES.SEEDS.BUTTERFLYPLANTSEED.NAME;
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.BUTTERFLYPLANTSEED.DESC;
		KAnimFile anim = Assets.GetAnim("seed_pollinator_plant_kanim");
		string text7 = "object";
		int num2 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		string text8 = global::STRINGS.CREATURES.SPECIES.BUTTERFLYPLANT.DOMESTICATEDDESC;
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, this, productionType, text4, text5, text6, anim, text7, num2, list, receptacleDirection, default(Tag), 2, text8, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", true);
		EntityTemplates.ExtendEntityToFood(gameObject3, FOOD.FOOD_TYPES.BUTTERFLY_SEED);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "ButterflyPlant_preview", Assets.GetAnim("pollinator_plant_kanim"), "place", 1, 2);
		gameObject.AddOrGet<Growing>().maxAge = 0f;
		gameObject.AddOrGet<Crop>().cropSpawnOffset = new Vector3(-0.0365f, 1.26175f, 0f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ButterflyPlant";

	public const string SEED_ID = "ButterflyPlantSeed";
}
