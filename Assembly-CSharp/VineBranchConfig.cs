using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class VineBranchConfig : IEntityConfig, IHasDlcRestrictions
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
		string text = "VineBranch";
		string text2 = global::STRINGS.CREATURES.SPECIES.VINEBRANCH.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.VINEBRANCH.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER0;
		KAnimFile anim = Assets.GetAnim("vine_kanim");
		string text4 = "line_idle";
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
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, anim, text4, sceneLayer, num2, num3, effectorValues, default(EffectorValues), SimHashes.Creature, list, 308.15f);
		string text5 = "VineBranchOriginal";
		bool flag = false;
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 273.15f, 298.15f, 318.15f, 378.15f, null, false, 0f, 0.15f, null, true, true, false, flag, 2400f, 0f, 2200f, text5, global::STRINGS.CREATURES.SPECIES.VINEBRANCH.NAME);
		gameObject.AddOrGet<HarvestDesignatable>();
		gameObject.AddOrGet<CodexEntryRedirector>().CodexID = "VineMother";
		gameObject.AddOrGet<UprootedMonitor>();
		Crop.CropVal cropVal = CROPS.CROP_TYPES.Find((Crop.CropVal m) => m.cropId == VineFruitConfig.ID);
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
		trait.Add(new AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute.Id, 3f, global::STRINGS.CREATURES.SPECIES.VINEBRANCH.NAME, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Maturity2.maxAttribute.Id, 3f, global::STRINGS.CREATURES.SPECIES.VINEBRANCH.NAME, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().PlantAttributes.YieldAmount.Id, (float)cropVal.numProduced, global::STRINGS.CREATURES.SPECIES.VINEBRANCH.NAME, false, false, true));
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.HarvestableIDs, "VineBranch");
		gameObject.AddOrGetDef<VineBranch.Def>().BRANCH_PREFAB_NAME = "VineBranch";
		gameObject.AddOrGet<Harvestable>();
		gameObject.AddOrGet<HarvestDesignatable>();
		WiltCondition wiltCondition = gameObject.AddOrGet<WiltCondition>();
		wiltCondition.WiltDelay = 0f;
		wiltCondition.RecoveryDelay = 0f;
		SeedProducer seedProducer = gameObject.AddOrGet<SeedProducer>();
		seedProducer.Configure("VineMotherSeed", SeedProducer.ProductionType.HarvestOnly, 1);
		seedProducer.seedDropChanceMultiplier = 0.16666667f;
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

	public const string ID = "VineBranch";

	public const float GROWING_DURATION = 1800f;

	public const float FRUIT_GROWING_DURATION = 1800f;

	public const int FRUIT_COUNT_PER_HARVEST = 1;
}
