using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class FishFoodConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("FishFood", ITEMS.FOOD.FISHFOOD.NAME, ITEMS.FOOD.FISHFOOD.DESC, 1f, true, Assets.GetAnim("fishfood_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, new List<Tag> { GameTags.Other });
		gameObject.AddOrGet<EntitySplitter>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "FishFood";

	public const string ANIM = "fishfood_kanim";

	public const string EFFECT_ID = "AteWellPreparedFishFood";

	public static ComplexRecipe recipe;
}
