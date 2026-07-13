using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class SpaceTreeBranchConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC2;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		string text = "SpaceTreeBranch";
		string text2 = global::STRINGS.CREATURES.SPECIES.SPACETREE.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SPACETREE.DESC;
		float num = 8f;
		EffectorValues tier = DECOR.BONUS.TIER1;
		KAnimFile anim = Assets.GetAnim("syrup_tree_kanim");
		string text4 = "idle_empty";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.BuildingFront;
		int num2 = 1;
		int num3 = 1;
		EffectorValues effectorValues = tier;
		List<Tag> list = new List<Tag>
		{
			GameTags.HideFromSpawnTool,
			GameTags.HideFromCodex,
			GameTags.PlantBranch
		};
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, anim, text4, sceneLayer, num2, num3, effectorValues, default(EffectorValues), SimHashes.Creature, list, 255f);
		string text5 = "SpaceTreeBranchOriginal";
		string text6 = global::STRINGS.CREATURES.SPECIES.SPACETREE.NAME;
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 173.15f, 198.15f, 258.15f, 293.15f, null, false, 0f, 0.15f, null, true, true, false, true, 12000f, 0f, 12200f, text5, text6);
		WiltCondition component = gameObject.GetComponent<WiltCondition>();
		component.WiltDelay = 0f;
		component.RecoveryDelay = 0f;
		Modifiers component2 = gameObject.GetComponent<Modifiers>();
		if (gameObject.GetComponent<Traits>() == null)
		{
			gameObject.AddOrGet<Traits>();
			component2.initialTraits.Add(text5);
		}
		KPrefabID component3 = gameObject.GetComponent<KPrefabID>();
		Crop.CropVal cropVal = new Crop.CropVal("WoodLog", 2700f, 75, true);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.HarvestableIDs, component3.PrefabID().ToString());
		component2.initialAttributes.Add(Db.Get().PlantAttributes.YieldAmount.Id);
		component2.initialAmounts.Add(Db.Get().Amounts.Maturity.Id);
		Trait trait = Db.Get().traits.Get(component2.initialTraits[0]);
		trait.Add(new AttributeModifier(Db.Get().PlantAttributes.YieldAmount.Id, (float)cropVal.numProduced, text6, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute.Id, cropVal.cropDuration / 600f, text6, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().PlantAttributes.MinLightLux.Id, 300f, global::STRINGS.CREATURES.SPECIES.SPACETREE.NAME, false, false, true));
		component2.initialAttributes.Add(Db.Get().PlantAttributes.MinLightLux.Id);
		gameObject.AddOrGet<IlluminationVulnerable>().SetPrefersDarkness(false);
		if (DlcManager.FeaturePlantMutationsEnabled())
		{
			gameObject.AddOrGet<MutantPlant>().SpeciesID = component3.PrefabTag;
			SymbolOverrideControllerUtil.AddToPrefab(gameObject);
		}
		gameObject.AddOrGet<Crop>().Configure(cropVal);
		gameObject.AddOrGet<Harvestable>();
		gameObject.AddOrGet<HarvestDesignatable>();
		gameObject.UpdateComponentRequirement<Uprootable>(false);
		gameObject.AddOrGet<CodexEntryRedirector>().CodexID = "SpaceTree";
		gameObject.AddOrGetDef<PlantBranch.Def>().animationSetupCallback = new Action<PlantBranchGrower.Instance, PlantBranch.Instance>(this.AdjustAnimation);
		gameObject.AddOrGetDef<SpaceTreeBranch.Def>().OPTIMAL_LUX_LEVELS = 10000;
		gameObject.AddOrGetDef<UnstableEntombDefense.Def>().Cooldown = 5f;
		gameObject.AddOrGet<BudUprootedMonitor>().destroyOnParentLost = true;
		return gameObject;
	}

	public void AdjustAnimation(PlantBranchGrower.Instance trunk, PlantBranch.Instance branch)
	{
		int num = Grid.PosToCell(trunk);
		int num2 = Grid.PosToCell(branch);
		CellOffset offset = Grid.GetOffset(num, num2);
		SpaceTreeBranch.Instance smi = branch.GetSMI<SpaceTreeBranch.Instance>();
		KBatchedAnimController component = branch.GetComponent<KBatchedAnimController>();
		if (smi != null && component != null && SpaceTreeBranchConfig.animationSets.ContainsKey(offset))
		{
			SpaceTreeBranch.AnimSet animSet = SpaceTreeBranchConfig.animationSets[offset];
			smi.Animations = animSet;
			component.Offset = SpaceTreeBranchConfig.animOffset[offset];
			smi.RefreshAnimation();
			branch.GetSMI<UnstableEntombDefense.Instance>().UnentombAnimName = SpaceTreeBranchConfig.entombDefenseAnimNames[offset];
			return;
		}
		global::Debug.LogWarning(string.Concat(new string[]
		{
			"Error on AdjustAnimation().SpaceTreeBranchConfig.cs, spaceBranchFound: ",
			(smi != null).ToString(),
			", animControllerFound: ",
			(component != null).ToString(),
			", animationSetFound: ",
			SpaceTreeBranchConfig.animationSets.ContainsKey(offset).ToString()
		}));
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.AddOrGet<Harvestable>().readyForHarvestStatusItem = Db.Get().CreatureStatusItems.ReadyForHarvest_Branch;
		inst.AddOrGet<HarvestDesignatable>().iconOffset = new Vector2(0f, Grid.CellSizeInMeters * 0.5f);
	}

	public void OnSpawn(GameObject inst)
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static SpaceTreeBranchConfig()
	{
		Dictionary<CellOffset, string> dictionary = new Dictionary<CellOffset, string>();
		CellOffset cellOffset = new CellOffset(-1, 1);
		dictionary[cellOffset] = "shake_branch_b";
		CellOffset cellOffset2 = new CellOffset(-1, 2);
		dictionary[cellOffset2] = "shake_branch_c";
		CellOffset cellOffset3 = new CellOffset(0, 2);
		dictionary[cellOffset3] = "shake_branch_d";
		CellOffset cellOffset4 = new CellOffset(1, 2);
		dictionary[cellOffset4] = "shake_branch_e";
		CellOffset cellOffset5 = new CellOffset(1, 1);
		dictionary[cellOffset5] = "shake_branch_f";
		SpaceTreeBranchConfig.entombDefenseAnimNames = dictionary;
		Dictionary<CellOffset, SpaceTreeBranch.AnimSet> dictionary2 = new Dictionary<CellOffset, SpaceTreeBranch.AnimSet>();
		cellOffset5 = new CellOffset(-1, 1);
		dictionary2[cellOffset5] = new SpaceTreeBranch.AnimSet
		{
			spawn = "branch_b_grow",
			undeveloped = "grow_b_healthy_short",
			spawn_pst = "branch_b_grow_pst",
			ready_harvest = "harvest_ready_branch_b",
			fill = "grow_fill_branch_b",
			wilted = "branch_b_wilt",
			wilted_short_trunk_healthy = "grow_b_wilt_short",
			wilted_short_trunk_wilted = "branch_b_wilt_short",
			hidden = "branch_b_hidden",
			manual_harvest_pre = "syrup_harvest_branch_b_pre",
			manual_harvest_loop = "syrup_harvest_branch_b_loop",
			manual_harvest_pst = "syrup_harvest_branch_b_pst",
			meterAnim_flowerWilted = new string[] { "leaves_b_wilt" },
			die = "branch_b_harvest",
			meterTargets = new string[] { "leaves_b_target" },
			meterAnimNames = new string[] { "leaves_b_meter" }
		};
		cellOffset4 = new CellOffset(-1, 2);
		dictionary2[cellOffset4] = new SpaceTreeBranch.AnimSet
		{
			spawn = "branch_c_grow",
			undeveloped = "grow_c_healthy_short",
			spawn_pst = "branch_c_grow_pst",
			ready_harvest = "harvest_ready_branch_c",
			fill = "grow_fill_branch_c",
			wilted = "branch_c_wilt",
			wilted_short_trunk_healthy = "grow_c_wilt_short",
			wilted_short_trunk_wilted = "branch_c_wilt_short",
			hidden = "branch_c_hidden",
			manual_harvest_pre = "syrup_harvest_branch_c_pre",
			manual_harvest_loop = "syrup_harvest_branch_c_loop",
			manual_harvest_pst = "syrup_harvest_branch_c_pst",
			meterAnim_flowerWilted = new string[] { "leaves_c_wilt" },
			die = "branch_c_harvest",
			meterTargets = new string[] { "leaves_c_target" },
			meterAnimNames = new string[] { "leaves_c_meter" }
		};
		cellOffset3 = new CellOffset(0, 2);
		dictionary2[cellOffset3] = new SpaceTreeBranch.AnimSet
		{
			spawn = "branch_d_grow",
			undeveloped = "grow_d_healthy_short",
			spawn_pst = "branch_d_grow_pst",
			ready_harvest = "harvest_ready_branch_d",
			fill = "grow_fill_branch_d",
			wilted = "branch_d_wilt",
			wilted_short_trunk_healthy = "grow_d_wilt_short",
			wilted_short_trunk_wilted = "branch_d_wilt_short",
			hidden = "branch_d_hidden",
			manual_harvest_pre = "syrup_harvest_branch_d_pre",
			manual_harvest_loop = "syrup_harvest_branch_d_loop",
			manual_harvest_pst = "syrup_harvest_branch_d_pst",
			meterAnim_flowerWilted = new string[] { "leaves_d_wilt" },
			die = "branch_d_harvest",
			meterTargets = new string[] { "leaves_d_target" },
			meterAnimNames = new string[] { "leaves_d_meter" }
		};
		cellOffset2 = new CellOffset(1, 2);
		dictionary2[cellOffset2] = new SpaceTreeBranch.AnimSet
		{
			spawn = "branch_e_grow",
			undeveloped = "grow_e_healthy_short",
			spawn_pst = "branch_e_grow_pst",
			ready_harvest = "harvest_ready_branch_e",
			fill = "grow_fill_branch_e",
			wilted = "branch_e_wilt",
			wilted_short_trunk_healthy = "grow_e_wilt_short",
			wilted_short_trunk_wilted = "branch_e_wilt_short",
			hidden = "branch_e_hidden",
			manual_harvest_pre = "syrup_harvest_branch_e_pre",
			manual_harvest_loop = "syrup_harvest_branch_e_loop",
			manual_harvest_pst = "syrup_harvest_branch_e_pst",
			meterAnim_flowerWilted = new string[] { "leaves_e_wilt" },
			die = "branch_e_harvest",
			meterTargets = new string[] { "leaves_e_target" },
			meterAnimNames = new string[] { "leaves_e_meter" }
		};
		cellOffset = new CellOffset(1, 1);
		dictionary2[cellOffset] = new SpaceTreeBranch.AnimSet
		{
			spawn = "branch_f_grow",
			undeveloped = "grow_f_healthy_short",
			spawn_pst = "branch_f_grow_pst",
			ready_harvest = "harvest_ready_branch_f",
			fill = "grow_fill_branch_f",
			wilted = "branch_f_wilt",
			wilted_short_trunk_healthy = "grow_f_wilt_short",
			wilted_short_trunk_wilted = "branch_f_wilt_short",
			hidden = "branch_f_hidden",
			manual_harvest_pre = "syrup_harvest_branch_f_pre",
			manual_harvest_loop = "syrup_harvest_branch_f_loop",
			manual_harvest_pst = "syrup_harvest_branch_f_pst",
			meterAnim_flowerWilted = new string[] { "leaves_f1_wilt", "leaves_f2_wilt" },
			die = "branch_f_harvest",
			meterTargets = new string[] { "leaves_f1_target", "leaves_f2_target" },
			meterAnimNames = new string[] { "leaves_f1_meter", "leaves_f2_meter" }
		};
		SpaceTreeBranchConfig.animationSets = dictionary2;
		Dictionary<CellOffset, Vector3> dictionary3 = new Dictionary<CellOffset, Vector3>();
		cellOffset = new CellOffset(-1, 1);
		dictionary3[cellOffset] = new Vector3(1f, -1f, 0f);
		cellOffset2 = new CellOffset(-1, 2);
		dictionary3[cellOffset2] = new Vector3(1f, -2f, 0f);
		cellOffset3 = new CellOffset(0, 2);
		dictionary3[cellOffset3] = new Vector3(0f, -2f, 0f);
		cellOffset4 = new CellOffset(1, 2);
		dictionary3[cellOffset4] = new Vector3(-1f, -2f, 0f);
		cellOffset5 = new CellOffset(1, 1);
		dictionary3[cellOffset5] = new Vector3(-1f, -1f, 0f);
		SpaceTreeBranchConfig.animOffset = dictionary3;
	}

	public const string ID = "SpaceTreeBranch";

	public static string[] BRANCH_NAMES = new string[] { "<sprite=\"oni_sprite_assets\" name=\"oni_sprite_assets_syrup_tree_l\">", "<sprite=\"oni_sprite_assets\" name=\"oni_sprite_assets_syrup_tree_tl\">", "<sprite=\"oni_sprite_assets\" name=\"oni_sprite_assets_syrup_tree_t\">", "<sprite=\"oni_sprite_assets\" name=\"oni_sprite_assets_syrup_tree_tr\">", "<sprite=\"oni_sprite_assets\" name=\"oni_sprite_assets_syrup_tree_r\">" };

	public const float GROWTH_DURATION = 2700f;

	public const int WOOD_AMOUNT = 75;

	private static Dictionary<CellOffset, string> entombDefenseAnimNames;

	private static Dictionary<CellOffset, SpaceTreeBranch.AnimSet> animationSets;

	private static Dictionary<CellOffset, Vector3> animOffset;
}
