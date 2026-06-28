using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SwampLilyFlowerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		List<Tag> list = new List<Tag> { GameTags.CookingIngredient };
		GameObject gameObject = EntityTemplates.CreateLooseEntity(SwampLilyFlowerConfig.ID, ITEMS.INGREDIENTS.SWAMPLILYFLOWER.NAME, ITEMS.INGREDIENTS.SWAMPLILYFLOWER.DESC, 1f, false, Assets.GetAnim("swamplilyflower_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, list);
		gameObject.UpdateComponentRequirement<EntitySplitter>(true);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static float SEEDS_PER_FRUIT = 1f;

	public static string ID = "SwampLilyFlower";
}
