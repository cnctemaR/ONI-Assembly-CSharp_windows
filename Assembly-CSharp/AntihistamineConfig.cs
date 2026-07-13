using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class AntihistamineConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("Antihistamine", global::STRINGS.ITEMS.PILLS.ANTIHISTAMINE.NAME, global::STRINGS.ITEMS.PILLS.ANTIHISTAMINE.DESC, 1f, true, Assets.GetAnim("pill_allergies_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.ANTIHISTAMINE);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Antihistamine", 10f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(new Tag[]
			{
				"PrickleFlowerSeed",
				KelpConfig.ID
			}, new float[] { 1f, 10f }),
			new ComplexRecipe.RecipeElement(SimHashes.Dirt.CreateTag(), 1f)
		};
		string text = ComplexRecipeManager.MakeRecipeID("Apothecary", array2, array);
		AntihistamineConfig.recipes.Add(this.CreateComplexRecipe(text, array2, array));
		return gameObject;
	}

	public ComplexRecipe CreateComplexRecipe(string recipeID, ComplexRecipe.RecipeElement[] input, ComplexRecipe.RecipeElement[] output)
	{
		return new ComplexRecipe(recipeID, input, output)
		{
			time = 100f,
			description = global::STRINGS.ITEMS.PILLS.ANTIHISTAMINE.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "Apothecary" },
			sortOrder = 10
		};
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Antihistamine";

	public static List<ComplexRecipe> recipes = new List<ComplexRecipe>();
}
