using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicPlantFoodConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("BasicPlantFood", ITEMS.FOOD.BASICPLANTFOOD.NAME, ITEMS.FOOD.BASICPLANTFOOD.DESC, 1f, false, Assets.GetAnim("meallicegrain_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, true, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.BASICPLANTFOOD, true);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "BasicPlantFood";
}
