using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FriedMushroomConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("FriedMushroom", ITEMS.FOOD.FRIEDMUSHROOM.NAME, ITEMS.FOOD.FRIEDMUSHROOM.DESC, 1f, false, Assets.GetAnim("funguscapfried_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, true, SimHashes.Creature, null);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.FRIED_MUSHROOM, true);
		string text = "FriedMushroom";
		string text2 = ITEMS.FOOD.FRIEDMUSHROOM.RECIPEDESC;
		Recipe recipe = new Recipe(text, 1f, (SimHashes)0, null, text2, 20).SetFabricator("CookingStation", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient(MushroomConfig.ID, 1f));
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "FriedMushroom";
}
