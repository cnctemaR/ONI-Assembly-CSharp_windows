using System;
using UnityEngine;

public class ResearchType
{
	public ResearchType(string id, string name, string description, Color color, Recipe.Ingredient[] fabricationIngredients, float fabricationTime, string kAnim_ID, string[] fabricators, string recipeDescription)
	{
		this._id = id;
		this._name = name;
		this._description = description;
		this._color = color;
		this.CreatePrefab(fabricationIngredients, fabricationTime, kAnim_ID, fabricators, recipeDescription, color);
	}

	public GameObject CreatePrefab(Recipe.Ingredient[] fabricationIngredients, float fabricationTime, string kAnim_ID, string[] fabricators, string recipeDescription, Color color)
	{
		GameObject gameObject = EntityTemplates.CreateBasicEntity(this.id, this.name, this.description, 1f, kAnim_ID, "ui", Grid.SceneLayer.BuildingFront);
		ResearchPointObject researchPointObject = gameObject.UpdateComponentRequirement<ResearchPointObject>(true);
		researchPointObject.TypeID = this.id;
		string name = this.name;
		this._recipe = new Recipe(gameObject, fabricators, 1f, 1f, (SimHashes)0, name, null);
		this._recipe.recipeDescription = recipeDescription;
		this._recipe.Icon = Assets.GetSprite("research_type_icon");
		this._recipe.IconColor = color;
		foreach (Recipe.Ingredient ingredient in fabricationIngredients)
		{
			this._recipe.AddIngredient(ingredient);
		}
		this._recipe.FabricationTime = fabricationTime;
		RecipeManager.Get().Add(this._recipe);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public string id
	{
		get
		{
			return this._id;
		}
	}

	public string name
	{
		get
		{
			return this._name;
		}
	}

	public string description
	{
		get
		{
			return this._description;
		}
	}

	public string recipe
	{
		get
		{
			return this.recipe;
		}
	}

	public Color color
	{
		get
		{
			return this._color;
		}
	}

	private string _id;

	private string _name;

	private string _description;

	private Recipe _recipe;

	private Sprite _icon;

	private Color _color;
}
