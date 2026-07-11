using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei;
using STRINGS;
using UnityEngine;

[DebuggerDisplay("{Name}")]
public class Recipe : IHasSortOrder
{
	public Recipe()
	{
	}

	public Recipe(string prefabId, float outputUnits = 1f, SimHashes elementOverride = (SimHashes)0, string nameOverride = null, string recipeDescription = null, int sortOrder = 0)
	{
		this.Result = TagManager.Create(prefabId);
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
			float amount = this.Ingredients[i].amount;
			if (i < selectedTags.Count)
			{
				list.Add(new Recipe.Ingredient(selectedTags[i], amount));
			}
			else
			{
				list.Add(new Recipe.Ingredient(this.Ingredients[i].tag, amount));
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
					list.Add(new Recipe.Ingredient(GameTagExtensions.Create(element.id), (float)num));
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
		SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid;
		float num = 0f;
		float num2 = 0f;
		foreach (Recipe.Ingredient ingredient in ingredientTags)
		{
			GameObject gameObject = resource_storage.FindFirst(ingredient.tag);
			if (gameObject != null)
			{
				Edible component = gameObject.GetComponent<Edible>();
				if (component)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, -component.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.CRAFTED_USED, "{0}", component.GetProperName()), UI.ENDOFDAYREPORT.NOTES.CRAFTED_CONTEXT);
				}
			}
			SimUtil.DiseaseInfo diseaseInfo2;
			float num3;
			resource_storage.ConsumeAndGetDisease(ingredient, out diseaseInfo2, out num3);
			diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo, diseaseInfo2);
			num = SimUtil.CalculateFinalTemperature(num2, num, ingredient.amount, num3);
			num2 += ingredient.amount;
		}
		GameObject prefab = Assets.GetPrefab(this.Result);
		GameObject gameObject2 = null;
		if (prefab != null)
		{
			gameObject2 = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore, Folder.Loot, null, 0);
			PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
			gameObject2.GetComponent<KSelectable>().entityName = this.Name;
			if (component2 != null)
			{
				gameObject2.GetComponent<KPrefabID>().RemoveTag(TagManager.Create("Vacuum"));
				if (this.ResultElementOverride != (SimHashes)0)
				{
					if (component2.GetComponent<ElementChunk>() != null)
					{
						component2.SetElement(this.ResultElementOverride);
					}
					else
					{
						component2.ElementID = this.ResultElementOverride;
					}
				}
				component2.Temperature = num;
				component2.Units = this.OutputUnits;
			}
			Edible component3 = gameObject2.GetComponent<Edible>();
			if (component3)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component3.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.CRAFTED, "{0}", component3.GetProperName()), UI.ENDOFDAYREPORT.NOTES.CRAFTED_CONTEXT);
			}
			gameObject2.SetActive(true);
			if (component2 != null)
			{
				component2.AddDisease(diseaseInfo.idx, diseaseInfo.count, "Recipe.CraftRecipe");
			}
			gameObject2.GetComponent<KMonoBehaviour>().Trigger(748399584, null);
		}
		return gameObject2;
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

	public Sprite GetUIIcon()
	{
		Sprite sprite = null;
		if (this.Icon != null)
		{
			sprite = this.Icon;
		}
		else
		{
			GameObject prefab = Assets.GetPrefab(this.Result);
			KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				sprite = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui");
			}
		}
		return sprite;
	}

	public Color GetUIColor()
	{
		return (!(this.Icon != null)) ? Color.white : this.IconColor;
	}

	private string nameOverride;

	public string HotKey;

	public string Type;

	public List<Recipe.Ingredient> Ingredients;

	public string recipeDescription;

	public Tag Result;

	public GameObject FabricationVisualizer;

	public SimHashes ResultElementOverride;

	public Sprite Icon;

	public Color IconColor = Color.white;

	public string[] fabricators;

	public float OutputUnits;

	public float FabricationTime;

	[DebuggerDisplay("{tag} {amount}")]
	[Serializable]
	public class Ingredient
	{
		public Ingredient(string tag, float amount)
		{
			this.tag = TagManager.Create(tag);
			this.amount = amount;
		}

		public Ingredient(Tag tag, float amount)
		{
			this.tag = tag;
			this.amount = amount;
		}

		public List<Element> GetElementOptions()
		{
			List<Element> list = new List<Element>(ElementLoader.elements);
			list.RemoveAll((Element e) => !e.IsSolid);
			list.RemoveAll((Element e) => !e.HasTag(this.tag));
			return list;
		}

		public Tag tag;

		public float amount;
	}
}
