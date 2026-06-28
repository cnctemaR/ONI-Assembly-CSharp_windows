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

	protected virtual void Initialize()
	{
	}

	private void LoadRecipes()
	{
		this.recipes = new List<Recipe>();
	}

	public void Add(Recipe recipe)
	{
		this.recipes.Add(recipe);
	}

	private static RecipeManager _Instance;

	public List<Recipe> recipes = new List<Recipe>();

	[Serializable]
	public class RecipeInfo : Resource
	{
		public string SmelterName;

		public string HotKey;

		public string OutputPrefabID;

		public SimHashes OutputElement;

		public int OutputMass;

		public string InputElementTag;

		public string InputMass;

		public float FabricationTime;
	}
}
