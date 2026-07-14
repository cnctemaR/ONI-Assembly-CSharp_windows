using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class KelpPlantConfig : IEntityConfig, IHasDlcRestrictions
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
		string text = "KelpPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.KELPPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.KELPPLANT.DESC;
		float num = 4f;
		EffectorValues tier = DECOR.BONUS.TIER1;
		KAnimFile anim = Assets.GetAnim("kelp_kanim");
		string text4 = "idle_empty";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.BuildingFront;
		int num2 = 1;
		int num3 = 2;
		EffectorValues effectorValues = tier;
		List<Tag> list = new List<Tag> { GameTags.Hanging };
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, anim, text4, sceneLayer, num2, num3, effectorValues, default(EffectorValues), SimHashes.Creature, list, 297.15f);
		EntityTemplates.MakeHangingOffsets(gameObject, 1, 2);
		GameObject gameObject2 = gameObject;
		float num4 = 253.15f;
		float num5 = 263.15f;
		float num6 = 358.15f;
		float num7 = 373.15f;
		string id = KelpConfig.ID;
		string text5 = global::STRINGS.CREATURES.SPECIES.KELPPLANT.NAME;
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num4, num5, num6, num7, KelpPlantConfig.ALLOWED_ELEMENTS, false, 0f, 0.15f, id, false, true, true, false, true, 2400f, 0f, 7400f, "KelpPlantOriginal", text5);
		gameObject.AddOrGet<PressureVulnerable>().allCellsMustBeSafe = true;
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.ToxicSand.ToString(),
				massConsumptionRate = 0.016666668f
			}
		});
		gameObject.AddOrGet<DirectlyEdiblePlant_Growth>();
		gameObject.GetComponent<UprootedMonitor>().monitorCells = new CellOffset[]
		{
			new CellOffset(0, 1)
		};
		gameObject.AddOrGet<StandardCropPlant>();
		GameObject gameObject3 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		string text6 = "KelpPlantSeed";
		string text7 = global::STRINGS.CREATURES.SPECIES.SEEDS.KELPPLANT.NAME;
		string text8 = global::STRINGS.CREATURES.SPECIES.SEEDS.KELPPLANT.DESC;
		KAnimFile anim2 = Assets.GetAnim("seed_kelp_kanim");
		string text9 = "object";
		int num8 = 1;
		List<Tag> list2 = new List<Tag>();
		list2.Add(GameTags.WaterSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Bottom;
		text5 = global::STRINGS.CREATURES.SPECIES.KELPPLANT.DOMESTICATEDDESC;
		EntityTemplates.MakeHangingOffsets(EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject3, this, productionType, text6, text7, text8, anim2, text9, num8, list2, receptacleDirection, default(Tag), 4, text5, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false), "KelpPlant_preview", Assets.GetAnim("kelp_kanim"), "place", 1, 2), 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "KelpPlant";

	public const string SEED_ID = "KelpPlantSeed";

	public const int YIELD_UNITS_PER_HARVEST = 50;

	public const float LIFETIME_CYCLES = 5f;

	public const float FERTILIZATION_RATE = 0.016666668f;

	public static SimHashes[] ALLOWED_ELEMENTS = new SimHashes[]
	{
		SimHashes.Water,
		SimHashes.DirtyWater,
		SimHashes.SaltWater,
		SimHashes.Brine,
		SimHashes.MurkyBrine,
		SimHashes.PhytoOil,
		SimHashes.NaturalResin
	};

	public const float CALCULATED_YIELD_MASS_PER_HARVEST = 50f;

	public const float CALCULATED_YIELD_MASS_PER_CYCLE = 10f;

	public const float CALCULATED_GROWTH_PER_CYCLE = 0.2f;

	public const float CALCULATED_LIFETIME_SEC = 3000f;
}
