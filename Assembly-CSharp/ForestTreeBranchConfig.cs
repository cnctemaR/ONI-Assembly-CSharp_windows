using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ForestTreeBranchConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "ForestTreeBranch";
		string text2 = global::STRINGS.CREATURES.SPECIES.WOOD_TREE.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.WOOD_TREE.DESC;
		float num = 8f;
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("tree_kanim"), "idle_empty", Grid.SceneLayer.BuildingFront, 1, 1, tier, default(EffectorValues), SimHashes.Creature, new List<Tag>(), 298.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 258.15f, 288.15f, 313.15f, 448.15f, null, true, 0f, 0.15f, "WoodLog", true, true, false, true, 12000f);
		gameObject.AddOrGet<TreeBud>();
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<BudUprootedMonitor>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ForestTreeBranch";

	public const float WOOD_AMOUNT = 300f;
}
