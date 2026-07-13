using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicRadPillConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("BasicRadPill", global::STRINGS.ITEMS.PILLS.BASICRADPILL.NAME, global::STRINGS.ITEMS.PILLS.BASICRADPILL.DESC, 1f, true, Assets.GetAnim("pill_radiation_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, new List<Tag> { GameTags.PedestalDisplayable });
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.BASICRADPILL);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Carbon", 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("BasicRadPill".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		BasicRadPillConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2), array, array2)
		{
			time = 50f,
			description = global::STRINGS.ITEMS.PILLS.BASICRADPILL.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
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

	public const string ID = "BasicRadPill";

	public static ComplexRecipe recipe;
}
