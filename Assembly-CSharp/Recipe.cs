using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Recipe : IHasSortOrder
{
	public Recipe()
	{
	}

	public Recipe(GameObject prefab, string[] fabricators, float fabricationTime, float outputUnits = 1f, SimHashes elementOverride = (SimHashes)0, string nameOverride = null, string recipeDescription = null)
	{
		Debug.Assert(prefab != null);
		this.Result = prefab;
		this.fabricators = fabricators;
		this.ResultElementOverride = elementOverride;
		this.Name = ((nameOverride != null) ? nameOverride : prefab.GetProperName());
		this.OutputUnits = outputUnits;
		this.FabricationTime = fabricationTime;
		this.Ingredients = new List<Recipe.Ingredient>();
		this.recipeDescription = recipeDescription;
		this.EffectDescription = new List<string>();
	}

	public Recipe(GameObject prefab, string fabricator, float fabricationTime, float outputUnits = 1f, SimHashes elementOverride = (SimHashes)0, string nameOverride = null, string recipeDescription = null, int sortOrder = 0)
	{
		Debug.Assert(prefab != null);
		this.Result = prefab;
		this.fabricators = new string[] { fabricator };
		this.ResultElementOverride = elementOverride;
		this.Name = ((nameOverride != null) ? nameOverride : prefab.GetProperName());
		this.OutputUnits = outputUnits;
		this.FabricationTime = fabricationTime;
		this.Ingredients = new List<Recipe.Ingredient>();
		this.recipeDescription = recipeDescription;
		this.sortOrder = sortOrder;
	}

	public int sortOrder { get; set; }

	public void AddIngredient(Recipe.Ingredient ingredient)
	{
		this.Ingredients.Add(ingredient);
	}

	public Recipe.Ingredient[] GetAllIngredients(IList<Tag> selectedTags)
	{
		List<Recipe.Ingredient> list = new List<Recipe.Ingredient>();
		int i = 0;
		while (i < this.Ingredients.Count)
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(this.Ingredients[i].tag);
			int num = (int)this.Ingredients[i].amount;
			bool flag = false;
			if (i >= selectedTags.Count)
			{
				goto IL_0079;
			}
			Tag tag = selectedTags[i];
			if (!selectedTags.Contains(tag))
			{
				goto IL_0079;
			}
			list.Add(new Recipe.Ingredient(tag, (float)num));
			IL_0094:
			i++;
			continue;
			IL_0079:
			if (!flag)
			{
				list.Add(new Recipe.Ingredient(tagSet[0], (float)num));
				goto IL_0094;
			}
			goto IL_0094;
		}
		return list.ToArray();
	}

	public Recipe.Ingredient[] GetAllIngredients(IList<Element> selected_elements)
	{
		List<Recipe.Ingredient> list = new List<Recipe.Ingredient>();
		for (int i = 0; i < this.Ingredients.Count; i++)
		{
			TagSet tagSet = new TagSet();
			tagSet.Add(this.Ingredients[i].tag);
			int num = (int)this.Ingredients[i].amount;
			bool flag = false;
			if (i < selected_elements.Count)
			{
				Element element = selected_elements[i];
				if (element != null)
				{
					foreach (Tag tag in tagSet)
					{
						if (element.HasTag(tag))
						{
							list.Add(new Recipe.Ingredient(TagManager.Create(element.id), (float)num));
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				list.Add(new Recipe.Ingredient(tagSet[0], (float)num));
			}
		}
		return list.ToArray();
	}

	public Element[] GetElements()
	{
		Element[] array = new Element[this.Ingredients.Count];
		for (int i = 0; i < this.Ingredients.Count; i++)
		{
			Tag tag = this.Ingredients[i].tag;
			foreach (Element element in ElementLoader.elements)
			{
				Tag tag2 = TagManager.Create(element.id);
				if (tag2 == tag)
				{
					array[i] = element;
					break;
				}
			}
		}
		return array;
	}

	public GameObject Craft(Storage resource_storage, IList<Tag> selectedTags)
	{
		Recipe.Ingredient[] allIngredients = this.GetAllIngredients(selectedTags);
		return this.CraftRecipe(resource_storage, allIngredients);
	}

	public GameObject Craft(Storage resource_storage, IList<Element> selected_element)
	{
		Recipe.Ingredient[] allIngredients = this.GetAllIngredients(selected_element);
		return this.CraftRecipe(resource_storage, allIngredients);
	}

	private GameObject CraftRecipe(Storage resource_storage, Recipe.Ingredient[] ingredientTags)
	{
		foreach (Recipe.Ingredient ingredient in ingredientTags)
		{
			resource_storage.Consume(ingredient);
		}
		GameObject gameObject = null;
		if (this.Result != null)
		{
			gameObject = GameUtil.KInstantiate(this.Result, Grid.SceneLayer.Use, Folder.Loot, null, 0);
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
				ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.rations * 100000f, "Crafted a " + component2.name);
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
		BuildingComplete component = this.Result.GetComponent<BuildingComplete>();
		if (component != null)
		{
			return component.Def;
		}
		return null;
	}

	public string Name;

	public string HotKey;

	public string Type;

	public float PlanOrder;

	public int NumProduced;

	public List<Recipe.Ingredient> Ingredients;

	public string recipeDescription;

	public List<string> EffectDescription = new List<string>();

	public GameObject Result;

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
