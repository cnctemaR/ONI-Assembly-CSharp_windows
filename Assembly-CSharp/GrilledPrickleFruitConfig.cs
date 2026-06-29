using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GrilledPrickleFruitConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("GrilledPrickleFruit", ITEMS.FOOD.GRILLEDPRICKLEFRUIT.NAME, ITEMS.FOOD.GRILLEDPRICKLEFRUIT.DESC, 1f, false, Assets.GetAnim("gristleberry_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.7f, true, SimHashes.Creature, null);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.GRILLED_PRICKLEFRUIT);
		string text = "GrilledPrickleFruit";
		string text2 = ITEMS.FOOD.GRILLEDPRICKLEFRUIT.RECIPEDESC;
		Recipe recipe = new Recipe(text, 1f, (SimHashes)0, null, text2, 20).SetFabricator("CookingStation", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient(PrickleFruitConfig.ID, 1f));
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "GrilledPrickleFruit";
}
