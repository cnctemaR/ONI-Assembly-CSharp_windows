using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SaltySticksPlantConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		string text = "SaltySticksPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.SALTYSTICKSPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SALTYSTICKSPLANT.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.PENALTY.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("salty_sticks_kanim"), "idle_empty", Grid.SceneLayer.BuildingBack, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 273.15f, 283.15f, 313.15f, 318.15f, new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, true, 0f, 0.15f, "SaltySticksFood", true, false, true, false, true, 2400f, 0f, 9800f, "SaltySticksPlantOriginal", global::STRINGS.CREATURES.SPECIES.SALTYSTICKSPLANT.NAME);
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<DirectlyEdiblePlant_Growth>();
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		string text4 = "SaltySticksPlantSeed";
		string text5 = global::STRINGS.CREATURES.SPECIES.SEEDS.SALTYSTICKSPLANT.NAME;
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.SALTYSTICKSPLANT.DESC;
		KAnimFile anim = Assets.GetAnim("seed_salty_sticks_kanim");
		string text7 = "object";
		int num2 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		string text8 = global::STRINGS.CREATURES.SPECIES.SALTYSTICKSPLANT.DOMESTICATEDDESC;
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, this, productionType, text4, text5, text6, anim, text7, num2, list, receptacleDirection, default(Tag), 1, text8, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Salt.CreateTag(),
				massConsumptionRate = 0.016666668f
			}
		});
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "SaltySticksPlant_preview", Assets.GetAnim("salty_sticks_kanim"), "place", 1, 2);
		gameObject.AddOrGet<PlantFiberProducer>().amount = 2f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SaltySticksPlant";

	public const string SEED_ID = "SaltySticksPlantSeed";

	public const float FERTILIZER_RATE = 0.016666668f;

	public const int GROWTH_CYCLES = 4;

	public const float PLANT_FIBER_PRODUCED_PER_CYCLE = 2f;
}
