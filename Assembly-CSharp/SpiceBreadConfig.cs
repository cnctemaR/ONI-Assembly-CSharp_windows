using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SpiceBreadConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("SpiceBread", ITEMS.FOOD.SPICEBREAD.NAME, ITEMS.FOOD.SPICEBREAD.DESC, 1f, false, Assets.GetAnim("pepperbread_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, true, SimHashes.Creature, null);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.SPICEBREAD, true);
		string text = "SpiceBread";
		string text2 = ITEMS.FOOD.SPICEBREAD.RECIPEDESC;
		Recipe recipe = new Recipe(text, 1f, (SimHashes)0, null, text2, 100).SetFabricator("CookingStation", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient("ColdWheatSeed", 5f));
		recipe.AddIngredient(new Recipe.Ingredient(SpiceNutConfig.ID, 1f));
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SpiceBread";
}
