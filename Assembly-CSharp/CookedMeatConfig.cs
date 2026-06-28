using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class CookedMeatConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("CookedMeat", ITEMS.FOOD.COOKEDMEAT.NAME, ITEMS.FOOD.COOKEDMEAT.DESC, 1f, false, Assets.GetAnim("barbeque_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.COOKEDMEAT, true);
		string text = "CookedMeat";
		string text2 = ITEMS.FOOD.COOKEDMEAT.RECIPEDESC;
		Recipe recipe = new Recipe(text, 1f, (SimHashes)0, null, text2, 21).SetFabricator("CookingStation", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient("Meat", 1f));
		recipe.AddIngredient(new Recipe.Ingredient(SpiceNutConfig.ID, 1f));
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "CookedMeat";
}
