using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FriedMushBarConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("FriedMushBar", ITEMS.FOOD.FRIEDMUSHBAR.NAME, ITEMS.FOOD.FRIEDMUSHBAR.DESC, 1f, false, Assets.GetAnim("mushbarfried_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		gameObject = EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.FRIEDMUSHBAR, true);
		string text = ITEMS.FOOD.FRIEDMUSHBAR.RECIPEDESC;
		Recipe recipe = new Recipe("FriedMushBar", 1f, (SimHashes)0, null, text, 1).SetFabricator("CookingStation", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient("MushBar", 2f));
		Edible component = gameObject.GetComponent<Edible>();
		if (component != null)
		{
			component.consumptionEffects.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.FRIEDMUSHBARREMOVESDISEASE, UI.GAMEOBJECTEFFECTS.TOOLTIPS.FRIEDMUSHBARREMOVESDISEASE, Descriptor.DescriptorType.Effect, false));
		}
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "FriedMushBar";
}
