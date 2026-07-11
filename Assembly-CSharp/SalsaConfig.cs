using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SalsaConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("Salsa", ITEMS.FOOD.SALSA.NAME, ITEMS.FOOD.SALSA.DESC, 1f, false, Assets.GetAnim("zestysalsa_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.5f, true, 0, SimHashes.Creature, null);
		return EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.SALSA);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Salsa";

	public static ComplexRecipe recipe;
}
