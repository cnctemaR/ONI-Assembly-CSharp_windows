using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class SeaTreeBranchConfig : IEntityConfig, IHasDlcRestrictions
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
		string text = "SeaTreeBranch";
		string text2 = global::STRINGS.CREATURES.SPECIES.SEATREEBRANCH.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SEATREEBRANCH.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER0;
		KAnimFile anim = Assets.GetAnim("sea_fairy_plant_kanim");
		string text4 = "branch_idle";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.BuildingFront;
		int num2 = 1;
		int num3 = 1;
		EffectorValues effectorValues = tier;
		List<Tag> list = new List<Tag>
		{
			GameTags.HideFromSpawnTool,
			GameTags.HideFromCodex,
			GameTags.PlantBranch,
			GameTags.ExcludeFromTemplate
		};
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, anim, text4, sceneLayer, num2, num3, effectorValues, default(EffectorValues), SimHashes.Creature, list, 302.65f);
		string text5 = "SeaTreeBranchOriginal";
		bool flag = false;
		GameObject gameObject2 = gameObject;
		float num4 = 248.15f;
		float num5 = 295.15f;
		float num6 = 310.15f;
		float num7 = 398.15f;
		bool flag2 = flag;
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num4, num5, num6, num7, PLANTS.SAFE_ELEMENTS.AllWaters, false, 0f, 0.15f, null, false, true, false, false, flag2, 2400f, 0f, 2200f, text5, global::STRINGS.CREATURES.SPECIES.SEATREEBRANCH.NAME);
		gameObject.AddOrGet<PressureVulnerable>().allCellsMustBeSafe = true;
		gameObject.AddOrGet<HarvestDesignatable>();
		gameObject.AddOrGet<CodexEntryRedirector>().CodexID = "SeaTree";
		gameObject.AddOrGet<UprootedMonitor>();
		Crop.CropVal cropVal = CROPS.CROP_TYPES.Find((Crop.CropVal m) => m.cropId == "SeaFairy");
		gameObject.AddOrGet<Crop>().Configure(cropVal);
		Modifiers component = gameObject.GetComponent<Modifiers>();
		if (gameObject.GetComponent<Traits>() == null)
		{
			gameObject.AddOrGet<Traits>();
			component.initialTraits.Add(text5);
		}
		component.initialAmounts.Add(Db.Get().Amounts.Maturity.Id);
		component.initialAmounts.Add(Db.Get().Amounts.Maturity2.Id);
		component.initialAttributes.Add(Db.Get().PlantAttributes.YieldAmount.Id);
		Trait trait = Db.Get().traits.Get(component.initialTraits[0]);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute.Id, 3f, global::STRINGS.CREATURES.SPECIES.SEATREEBRANCH.NAME, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Maturity2.maxAttribute.Id, 3f, global::STRINGS.CREATURES.SPECIES.SEATREEBRANCH.NAME, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().PlantAttributes.YieldAmount.Id, (float)cropVal.numProduced, global::STRINGS.CREATURES.SPECIES.SEATREEBRANCH.NAME, false, false, true));
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.HarvestableIDs, "SeaTreeBranch");
		SeaTreeBranch.Def def = gameObject.AddOrGetDef<SeaTreeBranch.Def>();
		def.MAX_BRANCH_COUNT = 8;
		def.BRANCH_PREFAB_NAME = "SeaTreeBranch";
		gameObject.AddOrGet<Harvestable>();
		gameObject.AddOrGet<HarvestDesignatable>();
		WiltCondition wiltCondition = gameObject.AddOrGet<WiltCondition>();
		wiltCondition.WiltDelay = 0f;
		wiltCondition.RecoveryDelay = 0f;
		SeedProducer seedProducer = gameObject.AddOrGet<SeedProducer>();
		seedProducer.Configure("SeaTreeSeed", SeedProducer.ProductionType.HarvestOnly, 1);
		seedProducer.seedDropChanceMultiplier = 0.125f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.AddOrGet<UprootedMonitor>().monitorCells = new CellOffset[0];
		inst.AddOrGet<HarvestDesignatable>().iconOffset = new Vector2(0f, Grid.CellSizeInMeters * 0.75f);
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SeaTreeBranch";

	public const float GROWING_DURATION_CYCLES = 3f;

	public const float GROWING_DURATION = 1800f;

	public const float BULB_GROWING_DURATION = 1800f;
}
