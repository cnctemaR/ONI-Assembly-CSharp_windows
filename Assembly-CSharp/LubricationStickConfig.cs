using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class LubricationStickConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC3;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("LubricationStick", ITEMS.LUBRICATIONSTICK.NAME, ITEMS.LUBRICATIONSTICK.DESC, LubricationStickConfig.MASS_PER_RECIPE, true, Assets.GetAnim("lubricant_applicator_kanim"), "idle1", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.4f, 1f, true, 0, SimHashes.LiquidGunk, null);
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddTag(GameTags.MedicalSupplies);
		gameObject.AddTag(GameTags.SolidLubricant);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.LiquidGunk.CreateTag(), GunkMonitor.GUNK_CAPACITY),
			new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 200f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("LubricationStick".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
			new ComplexRecipe.RecipeElement(SimHashes.DirtyWater.CreateTag(), 200f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		LubricationStickConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2), array, array2)
		{
			time = 100f,
			description = ITEMS.LUBRICATIONSTICK.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "Apothecary" },
			sortOrder = 1,
			requiredTech = Db.Get().TechItems.lubricationStick.parentTechId
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "LubricationStick";

	public static ComplexRecipe recipe;

	private const float WATER_MASS = 200f;

	public static float MASS_PER_RECIPE = GunkMonitor.GUNK_CAPACITY;
}
