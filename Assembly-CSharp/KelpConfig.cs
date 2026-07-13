using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class KelpConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = EntityTemplates.CreateLooseEntity(KelpConfig.ID, ITEMS.INGREDIENTS.KELP.NAME, ITEMS.INGREDIENTS.KELP.DESC, 1f, false, Assets.GetAnim("kelp_leaf_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, new List<Tag> { GameTags.IndustrialIngredient });
		gameObject.AddOrGet<EntitySplitter>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static string ID = "Kelp";

	public const float MASS_PER_UNIT = 1f;
}
