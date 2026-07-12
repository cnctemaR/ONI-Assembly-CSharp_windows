using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CodexRecipePanel : CodexWidget<CodexRecipePanel>
{
	public string linkID { get; set; }

	public CodexRecipePanel()
	{
	}

	public CodexRecipePanel(ComplexRecipe recipe, bool shouldUseFabricatorForTitle = false)
	{
		this.complexRecipe = recipe;
		this.useFabricatorForTitle = shouldUseFabricatorForTitle;
	}

	public CodexRecipePanel(Recipe rec)
	{
		this.recipe = rec;
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		HierarchyReferences component = contentGameObject.GetComponent<HierarchyReferences>();
		this.title = component.GetReference<LocText>("Title");
		this.materialPrefab = component.GetReference<RectTransform>("MaterialPrefab").gameObject;
		this.fabricatorPrefab = component.GetReference<RectTransform>("FabricatorPrefab").gameObject;
		this.ingredientsContainer = component.GetReference<RectTransform>("IngredientsContainer").gameObject;
		this.resultsContainer = component.GetReference<RectTransform>("ResultsContainer").gameObject;
		this.fabricatorContainer = component.GetReference<RectTransform>("FabricatorContainer").gameObject;
		this.ClearPanel();
		if (this.recipe != null)
		{
			this.ConfigureRecipe();
			return;
		}
		if (this.complexRecipe != null && SaveLoader.Instance.IsDlcListActiveForCurrentSave(this.complexRecipe.GetDlcIds()))
		{
			this.ConfigureComplexRecipe();
		}
	}

	private void ConfigureRecipe()
	{
		this.title.text = this.recipe.Result.ProperName();
		foreach (Recipe.Ingredient ingredient in this.recipe.Ingredients)
		{
			GameObject gameObject = Util.KInstantiateUI(this.materialPrefab, this.ingredientsContainer, true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(ingredient.tag, "ui", false);
			component.GetReference<Image>("Icon").sprite = uisprite.first;
			component.GetReference<Image>("Icon").color = uisprite.second;
			component.GetReference<LocText>("Amount").text = GameUtil.GetFormattedByTag(ingredient.tag, ingredient.amount, GameUtil.TimeSlice.None);
			component.GetReference<LocText>("Amount").color = Color.black;
			string text = ingredient.tag.ProperName();
			GameObject prefab = Assets.GetPrefab(ingredient.tag);
			if (prefab.GetComponent<Edible>() != null)
			{
				text = text + "\n    • " + string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(prefab.GetComponent<Edible>().GetQuality()));
			}
			gameObject.GetComponent<ToolTip>().toolTip = text;
		}
		GameObject gameObject2 = Util.KInstantiateUI(this.materialPrefab, this.resultsContainer, true);
		HierarchyReferences component2 = gameObject2.GetComponent<HierarchyReferences>();
		global::Tuple<Sprite, Color> uisprite2 = Def.GetUISprite(this.recipe.Result, "ui", false);
		component2.GetReference<Image>("Icon").sprite = uisprite2.first;
		component2.GetReference<Image>("Icon").color = uisprite2.second;
		component2.GetReference<LocText>("Amount").text = GameUtil.GetFormattedByTag(this.recipe.Result, this.recipe.OutputUnits, GameUtil.TimeSlice.None);
		component2.GetReference<LocText>("Amount").color = Color.black;
		string text2 = this.recipe.Result.ProperName();
		GameObject prefab2 = Assets.GetPrefab(this.recipe.Result);
		if (prefab2.GetComponent<Edible>() != null)
		{
			text2 = text2 + "\n    • " + string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(prefab2.GetComponent<Edible>().GetQuality()));
		}
		gameObject2.GetComponent<ToolTip>().toolTip = text2;
	}

	private void ConfigureComplexRecipe()
	{
		ComplexRecipe.RecipeElement[] array = this.complexRecipe.ingredients;
		for (int i = 0; i < array.Length; i++)
		{
			ComplexRecipe.RecipeElement ing = array[i];
			HierarchyReferences component = Util.KInstantiateUI(this.materialPrefab, this.ingredientsContainer, true).GetComponent<HierarchyReferences>();
			global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(ing.material, "ui", false);
			component.GetReference<Image>("Icon").sprite = uisprite.first;
			component.GetReference<Image>("Icon").color = uisprite.second;
			component.GetReference<LocText>("Amount").text = GameUtil.GetFormattedByTag(ing.material, ing.amount, GameUtil.TimeSlice.None);
			component.GetReference<LocText>("Amount").color = Color.black;
			string text = ing.material.ProperName();
			GameObject prefab = Assets.GetPrefab(ing.material);
			if (prefab.GetComponent<Edible>() != null)
			{
				text = text + "\n    • " + string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(prefab.GetComponent<Edible>().GetQuality()));
			}
			component.GetReference<ToolTip>("Tooltip").toolTip = text;
			component.GetReference<KButton>("Button").onClick += delegate
			{
				ManagementMenu.Instance.codexScreen.ChangeArticle(UI.ExtractLinkID(Assets.GetPrefab(ing.material).GetProperName()), false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
			};
		}
		array = this.complexRecipe.results;
		for (int i = 0; i < array.Length; i++)
		{
			ComplexRecipe.RecipeElement res = array[i];
			HierarchyReferences component2 = Util.KInstantiateUI(this.materialPrefab, this.resultsContainer, true).GetComponent<HierarchyReferences>();
			global::Tuple<Sprite, Color> uisprite2 = Def.GetUISprite(res.material, "ui", false);
			component2.GetReference<Image>("Icon").sprite = uisprite2.first;
			component2.GetReference<Image>("Icon").color = uisprite2.second;
			component2.GetReference<LocText>("Amount").text = GameUtil.GetFormattedByTag(res.material, res.amount, GameUtil.TimeSlice.None);
			component2.GetReference<LocText>("Amount").color = Color.black;
			string text2 = res.material.ProperName();
			GameObject prefab2 = Assets.GetPrefab(res.material);
			if (prefab2.GetComponent<Edible>() != null)
			{
				text2 = text2 + "\n    • " + string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(prefab2.GetComponent<Edible>().GetQuality()));
			}
			component2.GetReference<ToolTip>("Tooltip").toolTip = text2;
			component2.GetReference<KButton>("Button").onClick += delegate
			{
				ManagementMenu.Instance.codexScreen.ChangeArticle(UI.ExtractLinkID(Assets.GetPrefab(res.material).GetProperName()), false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
			};
		}
		string text3 = this.complexRecipe.id.Substring(0, this.complexRecipe.id.IndexOf('_'));
		HierarchyReferences component3 = Util.KInstantiateUI(this.fabricatorPrefab, this.fabricatorContainer, true).GetComponent<HierarchyReferences>();
		global::Tuple<Sprite, Color> uisprite3 = Def.GetUISprite(text3, "ui", false);
		component3.GetReference<Image>("Icon").sprite = uisprite3.first;
		component3.GetReference<Image>("Icon").color = uisprite3.second;
		component3.GetReference<LocText>("Time").text = GameUtil.GetFormattedTime(this.complexRecipe.time, "F0");
		component3.GetReference<LocText>("Time").color = Color.black;
		GameObject fabricator = Assets.GetPrefab(text3.ToTag());
		component3.GetReference<ToolTip>("Tooltip").toolTip = fabricator.GetProperName();
		component3.GetReference<KButton>("Button").onClick += delegate
		{
			ManagementMenu.Instance.codexScreen.ChangeArticle(UI.ExtractLinkID(fabricator.GetProperName()), false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
		};
		if (this.useFabricatorForTitle)
		{
			this.title.text = fabricator.GetProperName();
			return;
		}
		this.title.text = this.complexRecipe.results[0].material.ProperName();
	}

	private void ClearPanel()
	{
		foreach (object obj in this.ingredientsContainer.transform)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		foreach (object obj2 in this.resultsContainer.transform)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj2).gameObject);
		}
		foreach (object obj3 in this.fabricatorContainer.transform)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj3).gameObject);
		}
	}

	private LocText title;

	private GameObject materialPrefab;

	private GameObject fabricatorPrefab;

	private GameObject ingredientsContainer;

	private GameObject resultsContainer;

	private GameObject fabricatorContainer;

	private ComplexRecipe complexRecipe;

	private Recipe recipe;

	private bool useFabricatorForTitle;
}
