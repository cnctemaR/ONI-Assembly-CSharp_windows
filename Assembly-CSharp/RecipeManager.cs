using System;
using System.Collections.Generic;

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

	public static void Destroy()
	{
		RecipeManager._Instance = null;
	}

	public void Add(Recipe recipe)
	{
		this.recipes.Add(recipe);
		if (recipe.FabricationVisualizer != null)
		{
			recipe.FabricationVisualizer.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.GlobalDoNotDestroy).transform;
		}
	}

	private static RecipeManager _Instance;

	public List<Recipe> recipes = new List<Recipe>();
}
