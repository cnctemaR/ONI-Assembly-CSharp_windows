using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class IntermediateCureConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("IntermediateCure", ITEMS.PILLS.INTERMEDIATECURE.NAME, ITEMS.PILLS.INTERMEDIATECURE.DESC, 1f, true, Assets.GetAnim("iv_slimelung_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.MedicalSupplies);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SwampLilyFlowerConfig.ID, 1f),
			new ComplexRecipe.RecipeElement(SimHashes.Phosphorite.CreateTag(), 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("IntermediateCure", 1f)
		};
		string text = ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2);
		IntermediateCureConfig.recipe = new ComplexRecipe(text, array, array2)
		{
			time = 100f,
			description = ITEMS.PILLS.INTERMEDIATECURE.RECIPEDESC,
			useResultAsDescription = true,
			fabricators = new List<Tag> { "Apothecary" },
			sortOrder = 10
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "IntermediateCure";

	public static ComplexRecipe recipe;
}
