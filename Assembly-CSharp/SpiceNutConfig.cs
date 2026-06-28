using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SpiceNutConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(SpiceNutConfig.ID, ITEMS.FOOD.SPICENUT.NAME, ITEMS.FOOD.SPICENUT.DESC, 1f, false, Assets.GetAnim("spicenut_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, true, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.SPICENUT, true);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static float SEEDS_PER_FRUIT = 1f;

	public static string ID = "SpiceNut";
}
