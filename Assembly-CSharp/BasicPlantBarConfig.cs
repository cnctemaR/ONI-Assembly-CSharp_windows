using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicPlantBarConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("BasicPlantBar", ITEMS.FOOD.BASICPLANTBAR.NAME, ITEMS.FOOD.BASICPLANTBAR.DESC, 1f, false, Assets.GetAnim("liceloaf_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.BASICPLANTBAR, true);
		string text = "BasicPlantBar";
		float num = 1f;
		string text2 = ITEMS.FOOD.BASICPLANTBAR.RECIPEDESC;
		Recipe recipe = new Recipe(text, num, (SimHashes)0, null, text2, 2);
		recipe.AddIngredient(new Recipe.Ingredient("BasicPlantFood", 2f));
		recipe.AddIngredient(new Recipe.Ingredient("Water", 50f));
		recipe.FabricationVisualizer = MushBarConfig.CreateFabricationVisualizer(gameObject);
		recipe.SetFabricator("MicrobeMusher", FOOD.RECIPES.STANDARD_COOK_TIME);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "BasicPlantBar";
}
