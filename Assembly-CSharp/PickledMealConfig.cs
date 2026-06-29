using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PickledMealConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("PickledMeal", ITEMS.FOOD.PICKLEDMEAL.NAME, ITEMS.FOOD.PICKLEDMEAL.DESC, 1f, false, Assets.GetAnim("pickledmeal_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.7f, true, SimHashes.Creature, null);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.PICKLEDMEAL);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddPrefabTag(GameTags.Pickled);
		string text = "PickledMeal";
		string text2 = ITEMS.FOOD.PICKLEDMEAL.RECIPEDESC;
		Recipe recipe = new Recipe(text, 1f, (SimHashes)0, null, text2, 21).SetFabricator("CookingStation", FOOD.RECIPES.SMALL_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient("BasicPlantFood", 3f));
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "PickledMeal";
}
