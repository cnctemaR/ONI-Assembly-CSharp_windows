using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ForestTreeBranchConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		string text = "ForestTreeBranch";
		string text2 = global::STRINGS.CREATURES.SPECIES.WOOD_TREE.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.WOOD_TREE.DESC;
		float num = 8f;
		EffectorValues tier = DECOR.BONUS.TIER1;
		KAnimFile anim = Assets.GetAnim("tree_kanim");
		string text4 = "idle_empty";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.BuildingFront;
		int num2 = 1;
		int num3 = 1;
		EffectorValues effectorValues = tier;
		List<Tag> list = new List<Tag>
		{
			GameTags.HideFromSpawnTool,
			GameTags.PlantBranch
		};
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, anim, text4, sceneLayer, num2, num3, effectorValues, default(EffectorValues), SimHashes.Creature, list, 298.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 258.15f, 288.15f, 313.15f, 448.15f, null, true, 0f, 0.15f, "WoodLog", true, true, false, true, 12000f, 0f, 9800f, "ForestTreeBranchOriginal", global::STRINGS.CREATURES.SPECIES.WOOD_TREE.NAME);
		gameObject.AddOrGet<TreeBud>();
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<BudUprootedMonitor>();
		PlantBranch.Def def = gameObject.AddOrGetDef<PlantBranch.Def>();
		def.preventStartSMIOnSpawn = true;
		def.onEarlySpawn = new Action<PlantBranch.Instance>(this.TranslateOldTrunkToNewSystem);
		def.animationSetupCallback = new Action<PlantBranchGrower.Instance, PlantBranch.Instance>(this.AdjustAnimation);
		return gameObject;
	}

	public void AdjustAnimation(PlantBranchGrower.Instance trunk, PlantBranch.Instance branch)
	{
		int num = Grid.PosToCell(trunk);
		int num2 = Grid.PosToCell(branch);
		CellOffset offset = Grid.GetOffset(num, num2);
		StandardCropPlant component = branch.GetComponent<StandardCropPlant>();
		KBatchedAnimController component2 = branch.GetComponent<KBatchedAnimController>();
		component.anims = ForestTreeBranchConfig.animationSets[offset];
		component2.Offset = ForestTreeBranchConfig.animOffset[offset];
		component2.Play(component.anims.grow, KAnim.PlayMode.Paused, 1f, 0f);
		component.RefreshPositionPercent();
	}

	public void TranslateOldTrunkToNewSystem(PlantBranch.Instance smi)
	{
		BuddingTrunk andForgetOldTrunk = smi.GetComponent<TreeBud>().GetAndForgetOldTrunk();
		if (andForgetOldTrunk != null)
		{
			PlantBranchGrower.Instance smi2 = andForgetOldTrunk.GetSMI<PlantBranchGrower.Instance>();
			smi.SetTrunk(smi2);
		}
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.AddOrGet<Harvestable>().readyForHarvestStatusItem = Db.Get().CreatureStatusItems.ReadyForHarvest_Branch;
	}

	public void OnSpawn(GameObject inst)
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static ForestTreeBranchConfig()
	{
		Dictionary<CellOffset, StandardCropPlant.AnimSet> dictionary = new Dictionary<CellOffset, StandardCropPlant.AnimSet>();
		CellOffset cellOffset = new CellOffset(-1, 0);
		dictionary[cellOffset] = new StandardCropPlant.AnimSet
		{
			grow = "branch_a_grow",
			grow_pst = "branch_a_grow_pst",
			idle_full = "branch_a_idle_full",
			wilt_base = "branch_a_wilt",
			harvest = "branch_a_harvest"
		};
		CellOffset cellOffset2 = new CellOffset(-1, 1);
		dictionary[cellOffset2] = new StandardCropPlant.AnimSet
		{
			grow = "branch_b_grow",
			grow_pst = "branch_b_grow_pst",
			idle_full = "branch_b_idle_full",
			wilt_base = "branch_b_wilt",
			harvest = "branch_b_harvest"
		};
		CellOffset cellOffset3 = new CellOffset(-1, 2);
		dictionary[cellOffset3] = new StandardCropPlant.AnimSet
		{
			grow = "branch_c_grow",
			grow_pst = "branch_c_grow_pst",
			idle_full = "branch_c_idle_full",
			wilt_base = "branch_c_wilt",
			harvest = "branch_c_harvest"
		};
		CellOffset cellOffset4 = new CellOffset(0, 2);
		dictionary[cellOffset4] = new StandardCropPlant.AnimSet
		{
			grow = "branch_d_grow",
			grow_pst = "branch_d_grow_pst",
			idle_full = "branch_d_idle_full",
			wilt_base = "branch_d_wilt",
			harvest = "branch_d_harvest"
		};
		CellOffset cellOffset5 = new CellOffset(1, 2);
		dictionary[cellOffset5] = new StandardCropPlant.AnimSet
		{
			grow = "branch_e_grow",
			grow_pst = "branch_e_grow_pst",
			idle_full = "branch_e_idle_full",
			wilt_base = "branch_e_wilt",
			harvest = "branch_e_harvest"
		};
		CellOffset cellOffset6 = new CellOffset(1, 1);
		dictionary[cellOffset6] = new StandardCropPlant.AnimSet
		{
			grow = "branch_f_grow",
			grow_pst = "branch_f_grow_pst",
			idle_full = "branch_f_idle_full",
			wilt_base = "branch_f_wilt",
			harvest = "branch_f_harvest"
		};
		CellOffset cellOffset7 = new CellOffset(1, 0);
		dictionary[cellOffset7] = new StandardCropPlant.AnimSet
		{
			grow = "branch_g_grow",
			grow_pst = "branch_g_grow_pst",
			idle_full = "branch_g_idle_full",
			wilt_base = "branch_g_wilt",
			harvest = "branch_g_harvest"
		};
		ForestTreeBranchConfig.animationSets = dictionary;
		Dictionary<CellOffset, Vector3> dictionary2 = new Dictionary<CellOffset, Vector3>();
		cellOffset7 = new CellOffset(-1, 0);
		dictionary2[cellOffset7] = new Vector3(1f, 0f, 0f);
		cellOffset6 = new CellOffset(-1, 1);
		dictionary2[cellOffset6] = new Vector3(1f, -1f, 0f);
		cellOffset5 = new CellOffset(-1, 2);
		dictionary2[cellOffset5] = new Vector3(1f, -2f, 0f);
		cellOffset4 = new CellOffset(0, 2);
		dictionary2[cellOffset4] = new Vector3(0f, -2f, 0f);
		cellOffset3 = new CellOffset(1, 2);
		dictionary2[cellOffset3] = new Vector3(-1f, -2f, 0f);
		cellOffset2 = new CellOffset(1, 1);
		dictionary2[cellOffset2] = new Vector3(-1f, -1f, 0f);
		cellOffset = new CellOffset(1, 0);
		dictionary2[cellOffset] = new Vector3(-1f, 0f, 0f);
		ForestTreeBranchConfig.animOffset = dictionary2;
	}

	public const string ID = "ForestTreeBranch";

	public const float WOOD_AMOUNT = 300f;

	private static Dictionary<CellOffset, StandardCropPlant.AnimSet> animationSets;

	private static Dictionary<CellOffset, Vector3> animOffset;
}
