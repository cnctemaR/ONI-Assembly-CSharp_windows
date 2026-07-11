using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FriedMushroomConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("FriedMushroom", ITEMS.FOOD.FRIEDMUSHROOM.NAME, ITEMS.FOOD.FRIEDMUSHROOM.DESC, 1f, false, Assets.GetAnim("funguscapfried_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, true, 0, SimHashes.Creature, null);
		return EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.FRIED_MUSHROOM);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "FriedMushroom";

	public static ComplexRecipe recipe;
}
