using System;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager
{
	public static RecipeManager Get()
	{
		if (RecipeManager._Instance == null)
		{
			RecipeManager._Instance = new RecipeManager();
		}
		return RecipeManager._Instance;
	}

	public static void DestroyInstance()
	{
		RecipeManager._Instance = null;
	}

	public void Add(Recipe recipe)
	{
		this.recipes.Add(recipe);
		if (recipe.FabricationVisualizer != null)
		{
			global::UnityEngine.Object.DontDestroyOnLoad(recipe.FabricationVisualizer);
		}
	}

	private static RecipeManager _Instance;

	public List<Recipe> recipes = new List<Recipe>();
}
