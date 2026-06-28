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
		string text = ITEMS.FOOD.BASICPLANTBAR.RECIPEDESC;
		Recipe recipe = new Recipe("BasicPlantBar", 1f, (SimHashes)0, null, text, 2).SetFabricator("MicrobeMusher", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient("BasicPlantFood", 5f));
		recipe.AddIngredient(new Recipe.Ingredient("Water", 50f));
		recipe.FabricationVisualizer = MushBarConfig.CreateFabricationVisualizer(gameObject);
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
