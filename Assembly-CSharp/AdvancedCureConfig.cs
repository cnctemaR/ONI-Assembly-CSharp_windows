using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class AdvancedCureConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("AdvancedCure", ITEMS.PILLS.ADVANCEDCURE.NAME, ITEMS.PILLS.ADVANCEDCURE.DESC, 1f, true, Assets.GetAnim("vial_spore_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.MedicalSupplies);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Tungsten.CreateTag(), 1f),
			new ComplexRecipe.RecipeElement("LightBugOrangeEgg", 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("AdvancedCure", 1f)
		};
		string text = ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2);
		AdvancedCureConfig.recipe = new ComplexRecipe(text, array, array2)
		{
			time = 200f,
			description = ITEMS.PILLS.ADVANCEDCURE.RECIPEDESC,
			useResultAsDescription = true,
			fabricators = new List<Tag> { "Apothecary" },
			sortOrder = 20,
			requiredTech = "MedicineIV"
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "AdvancedCure";

	public static ComplexRecipe recipe;
}
