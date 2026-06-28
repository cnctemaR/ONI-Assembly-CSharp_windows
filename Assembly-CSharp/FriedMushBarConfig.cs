using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FriedMushBarConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("FriedMushBar", ITEMS.FOOD.FRIEDMUSHBAR.NAME, ITEMS.FOOD.FRIEDMUSHBAR.DESC, 1f, "mushbarfried_kanim", "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.FRIEDMUSHBAR);
		string text = ITEMS.FOOD.FRIEDMUSHBAR.RECIPEDESC;
		Recipe recipe = new Recipe(gameObject, "CookingStation", 40f, 1f, (SimHashes)0, null, text, 4);
		recipe.AddIngredient(new Recipe.Ingredient("MushBar", 2f));
		RecipeManager.Get().Add(recipe);
		EntityTemplates.SetDescriptionOrder(gameObject);
		Edible component = gameObject.GetComponent<Edible>();
		if (component != null)
		{
			component.consumptionEffectsString.Add(UI.GAMEOBJECTEFFECTS.FRIEDMUSHBARREMOVESDISEASE);
		}
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
