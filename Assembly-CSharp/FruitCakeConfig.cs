using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FruitCakeConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("FruitCake", global::STRINGS.ITEMS.FOOD.FRUITCAKE.NAME, global::STRINGS.ITEMS.FOOD.FRUITCAKE.DESC, 1f, false, Assets.GetAnim("fruitcake_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.FRUITCAKE);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "FruitCake";

	public const string ANIM = "fruitcake_kanim";

	public static ComplexRecipe recipe;
}
