using System;
using System.Collections.Generic;

public class ComplexRecipeManager
{
	public static ComplexRecipeManager Get()
	{
		if (ComplexRecipeManager._Instance == null)
		{
			ComplexRecipeManager._Instance = new ComplexRecipeManager();
		}
		return ComplexRecipeManager._Instance;
	}

	public static void Destroy()
	{
		ComplexRecipeManager._Instance = null;
	}

	public static string MakeRecipeID(string fabricator, Tag signatureElement)
	{
		return fabricator + "_" + signatureElement;
	}

	public void Add(ComplexRecipe recipe)
	{
		this.recipes.Add(recipe);
		if (recipe.FabricationVisualizer != null)
		{
			recipe.FabricationVisualizer.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.GlobalDoNotDestroy).transform;
		}
	}

	public ComplexRecipe GetRecipe(string id)
	{
		return this.recipes.Find((ComplexRecipe r) => r.id == id);
	}

	private static ComplexRecipeManager _Instance;

	public List<ComplexRecipe> recipes = new List<ComplexRecipe>();
}
