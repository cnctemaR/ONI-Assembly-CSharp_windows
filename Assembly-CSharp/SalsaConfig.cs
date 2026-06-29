using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SalsaConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("Salsa", ITEMS.FOOD.SALSA.NAME, ITEMS.FOOD.SALSA.DESC, 1f, false, Assets.GetAnim("zestysalsa_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.5f, true, SimHashes.Creature, null);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.SALSA);
		string text = "Salsa";
		string text2 = ITEMS.FOOD.SALSA.RECIPEDESC;
		Recipe recipe = new Recipe(text, 1f, (SimHashes)0, null, text2, 101).SetFabricator("CookingStation", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient(PrickleFruitConfig.ID, 2f));
		recipe.AddIngredient(new Recipe.Ingredient(SpiceNutConfig.ID, 2f));
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Salsa";
}
