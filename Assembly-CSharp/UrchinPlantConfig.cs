using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class UrchinPlantConfig : IEntityConfig, IHasDlcRestrictions
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
		string text = "UrchinPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.URCHINPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.URCHINPLANT.DESC;
		float num = 4f;
		EffectorValues tier = DECOR.BONUS.TIER2;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("urchin_plant_kanim"), "idle_full", Grid.SceneLayer.Building, 2, 2, tier, default(EffectorValues), SimHashes.Creature, null, 323.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 303.15f, 313.15f, 353.15f, 383.15f, new SimHashes[]
		{
			SimHashes.MurkyBrine,
			SimHashes.Brine,
			SimHashes.SaltWater,
			SimHashes.DirtyWater,
			SimHashes.Ink
		}, false, 0f, 0.15f, "Urchin", false, true, false, true, true, 2400f, 0f, 2200f, "UrchinPlantOriginal", global::STRINGS.CREATURES.SPECIES.URCHINPLANT.NAME);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = UrchinPlantConfig.FertilizerElement.CreateTag(),
				massConsumptionRate = 0.008333334f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<PressureVulnerable>().allCellsMustBeSafe = true;
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		string text4 = "UrchinPlantSeed";
		string text5 = global::STRINGS.CREATURES.SPECIES.SEEDS.URCHINPLANT.NAME;
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.URCHINPLANT.DESC;
		KAnimFile anim = Assets.GetAnim("seed_urchin_plant_kanim");
		string text7 = "object";
		int num2 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		list.Add(GameTags.BackwallSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		string text8 = global::STRINGS.CREATURES.SPECIES.URCHINPLANT.DOMESTICATEDDESC;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, this, productionType, text4, text5, text6, anim, text7, num2, list, receptacleDirection, default(Tag), 2, text8, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false), "UrchinPlant_preview", Assets.GetAnim("urchin_plant_kanim"), "place", 2, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		EntityTemplates.ExtendPlantEntityToRequireBackwall(inst);
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static readonly SimHashes FertilizerElement = SimHashes.RefinedCarbon;

	public const float FERTILIZATION_RATE = 0.008333334f;

	public const int LIFETIME_CYCLES = 16;

	public const int UNITS_PER_HARVEST = 1;

	public const string ID = "UrchinPlant";

	public const string SEED_ID = "UrchinPlantSeed";
}
