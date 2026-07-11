using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CodexRecipePanel : CodexWidget<CodexRecipePanel>
{
	public CodexRecipePanel()
	{
	}

	public CodexRecipePanel(ComplexRecipe recipe)
	{
		this.complexRecipe = recipe;
	}

	public CodexRecipePanel(Recipe rec)
	{
		this.recipe = rec;
	}

	public string linkID { get; set; }

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
		}
		else if (this.complexRecipe != null)
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
			Tuple<Sprite, Color> uisprite = Def.GetUISprite(ingredient.tag, "ui", false);
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
		Tuple<Sprite, Color> uisprite2 = Def.GetUISprite(this.recipe.Result, "ui", false);
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
		this.title.text = this.complexRecipe.results[0].material.ProperName();
		ComplexRecipe.RecipeElement[] ingredients = this.complexRecipe.ingredients;
		for (int i = 0; i < ingredients.Length; i++)
		{
			ComplexRecipe.RecipeElement ing = ingredients[i];
			GameObject gameObject = Util.KInstantiateUI(this.materialPrefab, this.ingredientsContainer, true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			Tuple<Sprite, Color> uisprite = Def.GetUISprite(ing.material, "ui", false);
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
				ManagementMenu.Instance.codexScreen.ChangeArticle(CodexCache.FormatLinkID(ing.material.ToString()), false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
			};
		}
		ComplexRecipe.RecipeElement[] results = this.complexRecipe.results;
		for (int j = 0; j < results.Length; j++)
		{
			ComplexRecipe.RecipeElement res = results[j];
			GameObject gameObject2 = Util.KInstantiateUI(this.materialPrefab, this.resultsContainer, true);
			HierarchyReferences component2 = gameObject2.GetComponent<HierarchyReferences>();
			Tuple<Sprite, Color> uisprite2 = Def.GetUISprite(res.material, "ui", false);
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
				ManagementMenu.Instance.codexScreen.ChangeArticle(CodexCache.FormatLinkID(res.material.ToString()), false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
			};
		}
		string fabricatorId = this.complexRecipe.id.Substring(0, this.complexRecipe.id.IndexOf('_'));
		GameObject gameObject3 = Util.KInstantiateUI(this.fabricatorPrefab, this.fabricatorContainer, true);
		HierarchyReferences component3 = gameObject3.GetComponent<HierarchyReferences>();
		Tuple<Sprite, Color> uisprite3 = Def.GetUISprite(fabricatorId, "ui", false);
		component3.GetReference<Image>("Icon").sprite = uisprite3.first;
		component3.GetReference<Image>("Icon").color = uisprite3.second;
		component3.GetReference<LocText>("Time").text = GameUtil.GetFormattedTime(this.complexRecipe.time);
		component3.GetReference<LocText>("Time").color = Color.black;
		GameObject prefab3 = Assets.GetPrefab(fabricatorId.ToTag());
		component3.GetReference<ToolTip>("Tooltip").toolTip = prefab3.GetProperName();
		component3.GetReference<KButton>("Button").onClick += delegate
		{
			ManagementMenu.Instance.codexScreen.ChangeArticle(CodexCache.FormatLinkID(fabricatorId), false, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
		};
	}

	private void ClearPanel()
	{
		IEnumerator enumerator = this.ingredientsContainer.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				global::UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		IEnumerator enumerator2 = this.resultsContainer.transform.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				object obj2 = enumerator2.Current;
				Transform transform2 = (Transform)obj2;
				global::UnityEngine.Object.Destroy(transform2.gameObject);
			}
		}
		finally
		{
			IDisposable disposable2;
			if ((disposable2 = enumerator2 as IDisposable) != null)
			{
				disposable2.Dispose();
			}
		}
		IEnumerator enumerator3 = this.fabricatorContainer.transform.GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				object obj3 = enumerator3.Current;
				Transform transform3 = (Transform)obj3;
				global::UnityEngine.Object.Destroy(transform3.gameObject);
			}
		}
		finally
		{
			IDisposable disposable3;
			if ((disposable3 = enumerator3 as IDisposable) != null)
			{
				disposable3.Dispose();
			}
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
}
