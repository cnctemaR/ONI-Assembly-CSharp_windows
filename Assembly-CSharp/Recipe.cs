using System;
using System.Collections.Generic;
using System.Globalization;
using STRINGS;
using UnityEngine;

public class Recipe : IHasSortOrder
{
	public Recipe()
	{
	}

	public Recipe(string prefabId, float outputUnits = 1f, SimHashes elementOverride = (SimHashes)0, string nameOverride = null, string recipeDescription = null, int sortOrder = 0)
	{
		Debug.Assert(prefabId != null);
		this.Result = TagManager.Create(prefabId, null);
		this.ResultElementOverride = elementOverride;
		this.nameOverride = nameOverride;
		this.OutputUnits = outputUnits;
		this.Ingredients = new List<Recipe.Ingredient>();
		this.recipeDescription = recipeDescription;
		this.sortOrder = sortOrder;
		this.FabricationVisualizer = null;
	}

	public int sortOrder { get; set; }

	public string Name
	{
		get
		{
			return (this.nameOverride != null) ? this.nameOverride : this.Result.ProperName();
		}
		set
		{
			this.nameOverride = value;
		}
	}

	public Recipe SetFabricator(string fabricator, float fabricationTime)
	{
		this.fabricators = new string[] { fabricator };
		this.FabricationTime = fabricationTime;
		RecipeManager.Get().Add(this);
		return this;
	}

	public Recipe SetFabricators(string[] fabricators, float fabricationTime)
	{
		this.fabricators = fabricators;
		this.FabricationTime = fabricationTime;
		RecipeManager.Get().Add(this);
		return this;
	}

	public Recipe SetIcon(Sprite Icon)
	{
		this.Icon = Icon;
		this.IconColor = Color.white;
		return this;
	}

	public Recipe SetIcon(Sprite Icon, Color IconColor)
	{
		this.Icon = Icon;
		this.IconColor = IconColor;
		return this;
	}

	public Recipe AddIngredient(Recipe.Ingredient ingredient)
	{
		this.Ingredients.Add(ingredient);
		return this;
	}

	public Recipe.Ingredient[] GetAllIngredients(IList<Tag> selectedTags)
	{
		List<Recipe.Ingredient> list = new List<Recipe.Ingredient>();
		for (int i = 0; i < this.Ingredients.Count; i++)
		{
			int num = (int)this.Ingredients[i].amount;
			if (i < selectedTags.Count)
			{
				list.Add(new Recipe.Ingredient(selectedTags[i], (float)num));
			}
			else
			{
				list.Add(new Recipe.Ingredient(this.Ingredients[i].tag, (float)num));
			}
		}
		return list.ToArray();
	}

	public Recipe.Ingredient[] GetAllIngredients(IList<Element> selected_elements)
	{
		List<Recipe.Ingredient> list = new List<Recipe.Ingredient>();
		for (int i = 0; i < this.Ingredients.Count; i++)
		{
			int num = (int)this.Ingredients[i].amount;
			bool flag = false;
			if (i < selected_elements.Count)
			{
				Element element = selected_elements[i];
				if (element != null && element.HasTag(this.Ingredients[i].tag))
				{
					list.Add(new Recipe.Ingredient(TagManager.Create(element.id), (float)num));
					flag = true;
				}
			}
			if (!flag)
			{
				list.Add(new Recipe.Ingredient(this.Ingredients[i].tag, (float)num));
			}
		}
		return list.ToArray();
	}

	public GameObject Craft(Storage resource_storage, IList<Tag> selectedTags)
	{
		Recipe.Ingredient[] allIngredients = this.GetAllIngredients(selectedTags);
		return this.CraftRecipe(resource_storage, allIngredients);
	}

	private GameObject CraftRecipe(Storage resource_storage, Recipe.Ingredient[] ingredientTags)
	{
		foreach (Recipe.Ingredient ingredient in ingredientTags)
		{
			resource_storage.Consume(ingredient);
		}
		GameObject prefab = Assets.GetPrefab(this.Result);
		GameObject gameObject = null;
		if (prefab != null)
		{
			gameObject = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Use, Folder.Loot, null, 0);
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			gameObject.GetComponent<KSelectable>().entityName = this.Name;
			if (component != null)
			{
				gameObject.GetComponent<KPrefabID>().RemoveTag(TagManager.Create("Vacuum", null));
				if (this.ResultElementOverride != (SimHashes)0)
				{
					if (component.GetComponent<ElementChunk>() != null)
					{
						component.SetElement(this.ResultElementOverride);
					}
					else
					{
						component.ElementID = this.ResultElementOverride;
					}
				}
				component.Units = this.OutputUnits;
			}
			Edible component2 = gameObject.GetComponent<Edible>();
			if (component2)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.Calories, string.Format(UI.ENDOFDAYREPORT.NOTES.CRAFTED, component2.name));
			}
			gameObject.SetActive(true);
			gameObject.GetComponent<KMonoBehaviour>().Trigger(748399584, null);
		}
		return gameObject;
	}

	public string[] MaterialOptionNames
	{
		get
		{
			List<string> list = new List<string>();
			foreach (Element element in ElementLoader.elements)
			{
				if (Array.IndexOf<Tag>(element.oreTags, this.Ingredients[0].tag) >= 0)
				{
					list.Add(element.id.ToString());
				}
			}
			return list.ToArray();
		}
	}

	public Element[] MaterialOptions()
	{
		List<Element> list = new List<Element>();
		foreach (Element element in ElementLoader.elements)
		{
			if (Array.IndexOf<Tag>(element.oreTags, this.Ingredients[0].tag) >= 0)
			{
				list.Add(element);
			}
		}
		return list.ToArray();
	}

	public BuildingDef GetBuildingDef()
	{
		GameObject prefab = Assets.GetPrefab(this.Result);
		BuildingComplete component = prefab.GetComponent<BuildingComplete>();
		if (component != null)
		{
			return component.Def;
		}
		return null;
	}

	private string nameOverride;

	public string HotKey;

	public string Type;

	public List<Recipe.Ingredient> Ingredients;

	public string recipeDescription;

	public List<Descriptor> EffectDescription = new List<Descriptor>();

	public Tag Result;

	public GameObject FabricationVisualizer;

	public SimHashes ResultElementOverride;

	public Sprite Icon;

	public Color IconColor = Color.white;

	public string[] fabricators;

	public float OutputUnits;

	public float FabricationTime;

	[Serializable]
	public class Ingredient
	{
		public Ingredient(string tag, float amount)
		{
			this.tag = TagManager.Create(tag, null);
			this.amount = amount;
		}

		public Ingredient(Tag tag, float amount)
		{
			this.tag = tag;
			this.amount = amount;
		}

		public string Name
		{
			get
			{
				return Recipe.Ingredient.TextInfo.ToTitleCase(this.tag.Name);
			}
		}

		public List<Element> GetElementOptions()
		{
			List<Element> list = new List<Element>(ElementLoader.elements);
			list.RemoveAll((Element e) => !e.IsSolid);
			list.RemoveAll((Element e) => !e.HasTag(this.tag));
			list.Sort((Element a, Element b) => a.electricalConductivity.CompareTo(b.electricalConductivity));
			return list;
		}

		public Tag tag;

		public float amount;

		private static TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;
	}
}
