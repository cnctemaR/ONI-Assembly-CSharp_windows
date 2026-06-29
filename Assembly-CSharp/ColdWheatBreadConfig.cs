using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class ColdWheatBreadConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("ColdWheatBread", ITEMS.FOOD.COLDWHEATBREAD.NAME, ITEMS.FOOD.COLDWHEATBREAD.DESC, 1f, false, Assets.GetAnim("frostbread_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.COLD_WHEAT_BREAD);
		string text = "ColdWheatBread";
		string text2 = ITEMS.FOOD.COLDWHEATBREAD.RECIPEDESC;
		Recipe recipe = new Recipe(text, 1f, (SimHashes)0, null, text2, 50).SetFabricator("CookingStation", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient("ColdWheatSeed", 3f));
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ColdWheatBread";
}
