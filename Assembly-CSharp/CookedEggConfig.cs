using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class CookedEggConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("CookedEgg", ITEMS.FOOD.COOKEDEGG.NAME, ITEMS.FOOD.COOKEDEGG.DESC, 1f, false, Assets.GetAnim("cookedegg_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		return EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.COOKED_EGG);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "CookedEgg";

	public static ComplexRecipe recipe;
}
