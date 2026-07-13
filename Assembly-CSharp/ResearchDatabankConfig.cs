using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ResearchDatabankConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("ResearchDatabank", global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RESEARCH_DATABANK.NAME, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RESEARCH_DATABANK.DESC, 1f, true, Assets.GetAnim("floppy_disc_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.TechComponents,
			GameTags.Experimental,
			GameTags.PedestalDisplayable
		});
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			gameObject.AddTag(GameTags.HideFromSpawnTool);
		}
		gameObject.AddOrGet<EntitySplitter>().maxStackSize = (float)ROCKETRY.DESTINATION_RESEARCH.BASIC;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		if (Game.IsDlcActiveForCurrentSave("DLC2_ID") && SaveLoader.Instance.ClusterLayout != null && SaveLoader.Instance.ClusterLayout.clusterTags.Contains("CeresCluster"))
		{
			inst.AddOrGet<KBatchedAnimController>().SwapAnims(new KAnimFile[] { Assets.GetAnim("floppy_disc_ceres_kanim") });
		}
	}

	public const string ID = "ResearchDatabank";

	public static readonly Tag TAG = TagManager.Create("ResearchDatabank");

	public const float MASS = 1f;
}
