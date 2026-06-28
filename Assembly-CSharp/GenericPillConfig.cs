using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GenericPillConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("GenericPill", ITEMS.PILLS.PLACEBO.NAME, ITEMS.PILLS.PLACEBO.DESC, 1f, "pill_1_kanim", "idle", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.GENERICPILL);
		string text = ITEMS.PILLS.PLACEBO.NAME;
		string text2 = ITEMS.PILLS.PLACEBO.RECIPEDESC;
		Recipe recipe = new Recipe(gameObject, "Apothecary", 40f, 1f, (SimHashes)0, text, text2, 1);
		recipe.AddIngredient(new Recipe.Ingredient("Water", 100f));
		recipe.AddIngredient(new Recipe.Ingredient("Sand", 100f));
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
