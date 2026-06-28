using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GenericPillConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("GenericPill", ITEMS.PILLS.PLACEBO.NAME, ITEMS.PILLS.PLACEBO.DESC, 1f, true, Assets.GetAnim("pill_1_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.GENERICPILL);
		string text = "GenericPill";
		string text2 = ITEMS.PILLS.PLACEBO.NAME;
		string text3 = ITEMS.PILLS.PLACEBO.RECIPEDESC;
		Recipe recipe = new Recipe(text, 1f, (SimHashes)0, text2, text3, 1).SetFabricator("Apothecary", 40f);
		recipe.AddIngredient(new Recipe.Ingredient("Water", 100f));
		recipe.AddIngredient(new Recipe.Ingredient("Sand", 100f));
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "GenericPill";
}
