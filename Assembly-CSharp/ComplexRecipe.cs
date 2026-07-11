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
		Tag tag = ((!this.useResultAsDescription) ? this.ingredients[0].material : this.results[0].material);
		GameObject prefab = Assets.GetPrefab(tag);
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			sprite = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui", false);
		}
		return sprite;
	}

	public Color GetUIColor()
	{
		return Color.white;
	}

	public string GetUIName()
	{
		if (this.displayInputAndOutput)
		{
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO, this.ingredients[0].material.ProperName(), this.results[0].material.ProperName());
		}
		if (this.useResultAsDescription)
		{
			return this.results[0].material.ProperName();
		}
		return this.ingredients[0].material.ProperName();
	}

	public string id;

	public ComplexRecipe.RecipeElement[] ingredients;

	public ComplexRecipe.RecipeElement[] results;

	public float time;

	public GameObject FabricationVisualizer;

	public bool useResultAsDescription;

	public bool displayInputAndOutput;

	public string description;

	public List<Tag> fabricators;

	public int sortOrder;

	public string requiredTech;

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
