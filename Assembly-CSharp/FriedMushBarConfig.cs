using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FriedMushBarConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("FriedMushBar", ITEMS.FOOD.FRIEDMUSHBAR.NAME, ITEMS.FOOD.FRIEDMUSHBAR.DESC, 1f, false, Assets.GetAnim("mushbarfried_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.FRIEDMUSHBAR);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "FriedMushBar";

	public static ComplexRecipe recipe;
}
