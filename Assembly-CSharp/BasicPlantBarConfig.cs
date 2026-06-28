using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicPlantBarConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("BasicPlantBar", ITEMS.FOOD.BASICPLANTBAR.NAME, ITEMS.FOOD.BASICPLANTBAR.DESC, 1f, "liceloaf_kanim", "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.BASICPLANTBAR);
		string text = ITEMS.FOOD.BASICPLANTBAR.RECIPEDESC;
		Recipe recipe = new Recipe(gameObject, "MicrobeMusher", 20f, 1f, (SimHashes)0, null, text, 2);
		recipe.AddIngredient(new Recipe.Ingredient("BasicPlantFood", 5f));
		recipe.AddIngredient(new Recipe.Ingredient("Water", 50f));
		RecipeManager.Get().Add(recipe);
		EntityTemplates.SetDescriptionOrder(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
