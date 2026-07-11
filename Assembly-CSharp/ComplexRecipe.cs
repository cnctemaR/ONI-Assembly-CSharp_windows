using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ComplexRecipe
{
	public ComplexRecipe(string id, ComplexRecipe.RecipeElement[] ingredients, ComplexRecipe.RecipeElement[] results)
	{
		this.id = id;
		this.ingredients = ingredients;
		this.results = results;
		ComplexRecipeManager.Get().Add(this);
	}

	public Tag FirstResult
	{
		get
		{
			return this.results[0].material;
		}
	}

	public float TotalResultUnits()
	{
		float num = 0f;
		foreach (ComplexRecipe.RecipeElement recipeElement in this.results)
		{
			num += recipeElement.amount;
		}
		return num;
	}

	public bool RequiresTechUnlock()
	{
		return !string.IsNullOrEmpty(this.requiredTech);
	}

	public bool IsRequiredTechUnlocked()
	{
		if (string.IsNullOrEmpty(this.requiredTech))
		{
			return true;
		}
		Tech tech = Db.Get().Techs.Get(this.requiredTech);
		return tech.IsComplete();
	}

	public Sprite GetUIIcon()
	{
		Sprite sprite = null;
		Tag tag = ((this.nameDisplay != ComplexRecipe.RecipeNameDisplay.Ingredient) ? this.results[0].material : this.ingredients[0].material);
		GameObject prefab = Assets.GetPrefab(tag);
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			sprite = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui", false, string.Empty);
		}
		return sprite;
	}

	public Color GetUIColor()
	{
		return Color.white;
	}

	public string GetUIName()
	{
		switch (this.nameDisplay)
		{
		case ComplexRecipe.RecipeNameDisplay.Result:
			return this.results[0].material.ProperName();
		case ComplexRecipe.RecipeNameDisplay.IngredientToResult:
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO, this.ingredients[0].material.ProperName(), this.results[0].material.ProperName());
		case ComplexRecipe.RecipeNameDisplay.ResultWithIngredient:
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_WITH, this.ingredients[0].material.ProperName(), this.results[0].material.ProperName());
		}
		return this.ingredients[0].material.ProperName();
	}

	public string id;

	public ComplexRecipe.RecipeElement[] ingredients;

	public ComplexRecipe.RecipeElement[] results;

	public float time;

	public GameObject FabricationVisualizer;

	public ComplexRecipe.RecipeNameDisplay nameDisplay;

	public string description;

	public List<Tag> fabricators;

	public int sortOrder;

	public string requiredTech;

	public enum RecipeNameDisplay
	{
		Ingredient,
		Result,
		IngredientToResult,
		ResultWithIngredient
	}

	public class RecipeElement
	{
		public RecipeElement(Tag material, float amount)
		{
			this.material = material;
			this.amount = amount;
		}

		public float amount { get; private set; }

		public Tag material;
	}
}
