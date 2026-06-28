using System;
using System.Collections.Generic;

public class RefineryRecipeManager
{
	public static RefineryRecipeManager Get()
	{
		if (RefineryRecipeManager._Instance == null)
		{
			RefineryRecipeManager._Instance = new RefineryRecipeManager();
		}
		return RefineryRecipeManager._Instance;
	}

	public static void Destroy()
	{
		RefineryRecipeManager._Instance = null;
	}

	public void Add(RefinementRecipe recipe)
	{
		this.recipes.Add(recipe);
		if (recipe.FabricationVisualizer != null)
		{
			recipe.FabricationVisualizer.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.GlobalDoNotDestroy).transform;
		}
	}

	public RefinementRecipe GetRecipe(Tag tag)
	{
		return this.recipes.Find((RefinementRecipe r) => r.material == tag);
	}

	private static RefineryRecipeManager _Instance;

	public List<RefinementRecipe> recipes = new List<RefinementRecipe>();
}
