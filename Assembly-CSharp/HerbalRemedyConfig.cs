using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class HerbalRemedyConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("HerbalRemedy", ITEMS.PILLS.HERBALREMEDY.NAME, ITEMS.PILLS.HERBALREMEDY.DESC, 1f, "pill_2_kanim", "idle", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.POLYGONAL, 1f, 1f, true);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.HERBALREMEDY);
		string text = ITEMS.PILLS.HERBALREMEDY.NAME;
		string text2 = ITEMS.PILLS.HERBALREMEDY.RECIPEDESC;
		Recipe recipe = new Recipe(gameObject, "Apothecary", 40f, 1f, (SimHashes)0, text, text2, 2);
		recipe.AddIngredient(new Recipe.Ingredient("Sand", 100f));
		recipe.AddIngredient(new Recipe.Ingredient("Algae", 100f));
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
