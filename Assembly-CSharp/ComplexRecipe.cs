using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ComplexRecipe
{
	public Tag FirstResult
	{
		get
		{
			return this.results[0].material;
		}
	}

	public ComplexRecipe(string id, ComplexRecipe.RecipeElement[] ingredients, ComplexRecipe.RecipeElement[] results)
	{
		this.id = id;
		this.ingredients = ingredients;
		this.results = results;
		ComplexRecipeManager.Get().Add(this);
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
		return string.IsNullOrEmpty(this.requiredTech) || Db.Get().Techs.Get(this.requiredTech).IsComplete();
	}

	public Sprite GetUIIcon()
	{
		Sprite sprite = null;
		KBatchedAnimController component = Assets.GetPrefab((this.nameDisplay == ComplexRecipe.RecipeNameDisplay.Ingredient) ? this.ingredients[0].material : this.results[0].material).GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			sprite = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui", false, "");
		}
		return sprite;
	}

	public Color GetUIColor()
	{
		return Color.white;
	}

	public string GetUIName(bool includeAmounts)
	{
		switch (this.nameDisplay)
		{
		case ComplexRecipe.RecipeNameDisplay.Result:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_SIMPLE_INCLUDE_AMOUNTS, this.results[0].material.ProperName(), this.results[0].amount);
			}
			return this.results[0].material.ProperName();
		case ComplexRecipe.RecipeNameDisplay.IngredientToResult:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_INCLUDE_AMOUNTS, new object[]
				{
					this.ingredients[0].material.ProperName(),
					this.results[0].material.ProperName(),
					this.ingredients[0].amount,
					this.results[0].amount
				});
			}
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO, this.ingredients[0].material.ProperName(), this.results[0].material.ProperName());
		case ComplexRecipe.RecipeNameDisplay.ResultWithIngredient:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_WITH_INCLUDE_AMOUNTS, new object[]
				{
					this.ingredients[0].material.ProperName(),
					this.results[0].material.ProperName(),
					this.ingredients[0].amount,
					this.results[0].amount
				});
			}
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_WITH, this.ingredients[0].material.ProperName(), this.results[0].material.ProperName());
		}
		if (includeAmounts)
		{
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_SIMPLE_INCLUDE_AMOUNTS, this.ingredients[0].material.ProperName(), this.ingredients[0].amount);
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
