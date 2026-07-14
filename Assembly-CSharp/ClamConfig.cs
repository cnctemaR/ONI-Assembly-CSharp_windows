using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ClamConfig : IEntityConfig, IHasDlcRestrictions
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
		string text = "Clam";
		string text2 = global::STRINGS.CREATURES.SPECIES.CLAM.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.CLAM.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER2;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("clam_kanim"), "idle_full", Grid.SceneLayer.BuildingBack, 3, 3, tier, default(EffectorValues), SimHashes.Creature, null, 303.15f);
		gameObject.AddOrGet<ClamHarvestable>();
		GameObject gameObject2 = gameObject;
		float num2 = 273.15f;
		float num3 = 298.15f;
		float num4 = 318.15f;
		float num5 = 373.15f;
		string text4 = SimHashes.Pearl.ToString();
		gameObject = EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num2, num3, num4, num5, PLANTS.SAFE_ELEMENTS.AllWaters, false, 0f, 0.15f, text4, false, true, true, false, true, 2400f, 0f, 2200f, "ClamOriginal", global::STRINGS.CREATURES.SPECIES.PLANKTONCORAL.NAME);
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<LoopingSounds>();
		GameObject gameObject3 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		string text5 = "ClamSeed";
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.CLAM.NAME;
		string text7 = global::STRINGS.CREATURES.SPECIES.SEEDS.CLAM.DESC;
		KAnimFile anim = Assets.GetAnim("seed_clam_kanim");
		string text8 = "object";
		int num6 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.LargeSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		text4 = global::STRINGS.CREATURES.SPECIES.CLAM.DOMESTICATEDDESC;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject3, this, productionType, text5, text6, text7, anim, text8, num6, list, receptacleDirection, default(Tag), 20, text4, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false), "Clam_preview", Assets.GetAnim("clam_kanim"), "place", 3, 3);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Sand.CreateTag(),
				massConsumptionRate = 0.058333334f
			}
		});
		gameObject.AddOrGet<PressureVulnerable>().allCellsMustBeSafe = true;
		gameObject.AddOrGet<ClamPoopStation>();
		gameObject.AddTag(GameTags.BlockBuildOverPlantFeature);
		gameObject.GetComponent<Growing>().shouldGrowOld = false;
		gameObject.GetComponent<UprootedMonitor>().monitorCells = new CellOffset[]
		{
			new CellOffset(0, -1),
			new CellOffset(-1, -1),
			new CellOffset(1, -1)
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
		prefab.GetComponent<StandardCropPlant>().anims = ClamConfig.CROP_PLANT_DEFAULT_ANIM_SET;
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Clam";

	public const string SEED_ID = "ClamSeed";

	public const float LIFETIME_CYCLES = 8f;

	public const int HARVEST_YIELD_MASS = 50;

	public const float SAND_CONSUMPTION_RATE = 0.058333334f;

	public static StandardCropPlant.AnimSet CROP_PLANT_DEFAULT_ANIM_SET = new StandardCropPlant.AnimSet(StandardCropPlant.defaultAnimSet)
	{
		wilt_recover_base = "wilt_recover"
	};

	public static StandardCropPlant.AnimSet CROP_PLANT_CLOSED_ANIM_SET = new StandardCropPlant.AnimSet(StandardCropPlant.defaultAnimSet)
	{
		grow_pst = "idle_full_closed",
		idle_full = "idle_full_closed",
		wilt_recover_base = "wilt_recover"
	};
}
