using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class TubeWormConfig : IEntityConfig, IHasDlcRestrictions
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
		string text = "TubeWorm";
		string text2 = global::STRINGS.CREATURES.SPECIES.TUBEWORM.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.TUBEWORM.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("tube_worm_kanim"), "idle_empty", Grid.SceneLayer.BuildingFront, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 348.15f);
		GameObject gameObject2 = gameObject;
		float num2 = 303.15f;
		float num3 = 323.15f;
		float num4 = 383.15f;
		float num5 = 403.15f;
		string text4 = SimHashes.Polypropylene.ToString();
		gameObject = EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num2, num3, num4, num5, PLANTS.SAFE_ELEMENTS.MurkyWaters, false, 0f, 0.15f, text4, false, false, true, false, true, 2400f, 0f, 2200f, "TubeWormOriginal", global::STRINGS.CREATURES.SPECIES.TUBEWORM.NAME);
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<DirectlyEdiblePlant_Growth>();
		gameObject.AddOrGet<LoopingSounds>();
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Sulfur.CreateTag(),
				massConsumptionRate = 0.033333335f
			}
		});
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.MurkyBrine.CreateTag(),
				massConsumptionRate = 0.05f
			},
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Brine.CreateTag(),
				massConsumptionRate = 0.05f
			}
		});
		gameObject.AddOrGet<PressureVulnerable>().allCellsMustBeSafe = true;
		GameObject gameObject3 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		string text5 = "TubeWormSeed";
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.TUBEWORM.NAME;
		string text7 = global::STRINGS.CREATURES.SPECIES.SEEDS.TUBEWORM.DESC;
		KAnimFile anim = Assets.GetAnim("seed_tube_worm_kanim");
		string text8 = "object";
		int num6 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		text4 = global::STRINGS.CREATURES.SPECIES.TUBEWORM.DOMESTICATEDDESC;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject3, this, productionType, text5, text6, text7, anim, text8, num6, list, receptacleDirection, default(Tag), 21, text4, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false), "TubeWorm_preview", Assets.GetAnim("tube_worm_kanim"), "place", 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "TubeWorm";

	public const string SEED_ID = "TubeWormSeed";

	public const float LIFETIME_CYCLES = 8f;

	public const int HARVEST_YIELD = 200;

	public const float SULFUR_CONSUMPTION_RATE = 0.033333335f;

	public const float MURKY_BRINE_CONSUMPTION_RATE = 0.05f;

	public const SimHashes PRODUCT_ELEMENT = SimHashes.Polypropylene;
}
