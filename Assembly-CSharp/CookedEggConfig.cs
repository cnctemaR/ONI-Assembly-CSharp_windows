using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class CookedEggConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("CookedEgg", ITEMS.FOOD.COOKEDEGG.NAME, ITEMS.FOOD.COOKEDEGG.DESC, 1f, false, Assets.GetAnim("cookedegg_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.COOKED_EGG, true);
		string text = "CookedEgg";
		string text2 = ITEMS.FOOD.COOKEDEGG.RECIPEDESC;
		Recipe recipe = new Recipe(text, 1f, (SimHashes)0, null, text2, 1).SetFabricator("CookingStation", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient(GameTags.Egg, 1f));
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "CookedEgg";
}
