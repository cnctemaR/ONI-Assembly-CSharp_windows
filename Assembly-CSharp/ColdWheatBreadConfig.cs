using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class ColdWheatBreadConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("ColdWheatBread", ITEMS.FOOD.COLDWHEATBREAD.NAME, ITEMS.FOOD.COLDWHEATBREAD.DESC, 1f, false, Assets.GetAnim("frostbread_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		return EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.COLD_WHEAT_BREAD);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ColdWheatBread";

	public static ComplexRecipe recipe;
}
