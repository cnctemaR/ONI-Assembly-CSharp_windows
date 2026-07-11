using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SushiConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("Sushi", ITEMS.FOOD.SUSHI.NAME, ITEMS.FOOD.SUSHI.DESC, 1f, false, Assets.GetAnim("zestysalsa_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.5f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.SUSHI);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Sushi";

	public static ComplexRecipe recipe;
}
