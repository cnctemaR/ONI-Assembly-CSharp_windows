using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class FabricatorSideScreen : SideScreenContent
{
	public override string GetTitle()
	{
		if (this.targetFab == null)
		{
			return Strings.Get(this.titleKey).ToString().Replace("{0}", string.Empty);
		}
		return string.Format(Strings.Get(this.titleKey), this.targetFab.GetProperName());
	}

	public override void SetTarget(GameObject target)
	{
		Fabricator component = target.GetComponent<Fabricator>();
		if (component == null)
		{
			Debug.LogError("The object selected doesn't have a fabricator!");
			return;
		}
		this.queue.SetFabricator(component);
		this.Initialize(component);
	}

	public void Initialize(Fabricator target)
	{
		if (target == null)
		{
			Debug.LogError("Fabricator provided was null.");
			return;
		}
		this.targetFab = target;
		base.gameObject.SetActive(true);
		Recipe[] recipes = this.targetFab.GetRecipes();
		Array.Sort<Recipe>(recipes, (Recipe a, Recipe b) => a.sortOrder - b.sortOrder);
		this.recipeMap = new Dictionary<KToggle, Recipe>();
		this.recipeToggles.ForEach(delegate(KToggle rbi)
		{
			global::UnityEngine.Object.Destroy(rbi.gameObject);
		});
		this.recipeToggles.Clear();
		Recipe[] array = recipes;
		for (int i = 0; i < array.Length; i++)
		{
			Recipe recipe = array[i];
			KToggle newToggle = Util.KInstantiateUI<KToggle>(this.recipeButton, this.recipeGrid, false);
			newToggle.GetComponentInChildren<LocText>().text = recipe.Name;
			KBatchedAnimController component = recipe.Result.GetComponent<KBatchedAnimController>();
			Sprite sprite = ((!(recipe.Icon == null)) ? recipe.Icon : Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui"));
			if (sprite == null)
			{
				sprite = this.elementPlaceholderSpr;
			}
			Image componentInChildrenOnly = newToggle.gameObject.GetComponentInChildrenOnly<Image>();
			componentInChildrenOnly.sprite = sprite;
			newToggle.onClick += delegate
			{
				this.ToggleClicked(newToggle, true);
			};
			newToggle.gameObject.SetActive(true);
			if (recipe.Icon != null)
			{
				componentInChildrenOnly.color = recipe.IconColor;
				componentInChildrenOnly.rectTransform().sizeDelta = new Vector2(-30f, -40f);
				componentInChildrenOnly.rectTransform().anchoredPosition = new Vector2(0f, 14f);
			}
			this.recipeMap.Add(newToggle, recipe);
			this.recipeToggles.Add(newToggle);
		}
		if (this.recipeToggles.Count > 0)
		{
			if (this.selectedRecipeFabricatorMap.ContainsKey(this.targetFab))
			{
				int num = this.selectedRecipeFabricatorMap[this.targetFab];
				this.ToggleClicked(this.recipeToggles[num], false);
			}
			else
			{
				this.recipeToggles.ForEach(delegate(KToggle tg)
				{
					if (tg != this.selectedToggle)
					{
						tg.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
					}
				});
				this.subtitleLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPESELECTED);
				this.descriptionLabel.gameObject.SetActive(false);
				if (this.noRecipeSelectedLabel != null)
				{
					this.noRecipeSelectedLabel.gameObject.SetActive(true);
				}
				this.buildBtn.interactable = false;
				this.infiniteBuildBtn.interactable = false;
			}
		}
	}

	private void ToggleClicked(KToggle toggle, bool shouldSound)
	{
		if (!this.recipeMap.ContainsKey(toggle))
		{
			Debug.LogError("Recipe not found on recipe list.");
			return;
		}
		if (shouldSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
		}
		this.selectedToggle = toggle;
		this.selectedToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
		this.buildBtn.interactable = true;
		this.infiniteBuildBtn.interactable = true;
		this.recipeToggles.ForEach(delegate(KToggle tg)
		{
			if (tg != this.selectedToggle)
			{
				tg.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			}
		});
		this.selectedRecipe = this.recipeMap[toggle];
		this.selectedRecipeFabricatorMap[this.targetFab] = this.recipeToggles.IndexOf(toggle);
		Element[] recipeElements = this.GetRecipeElements(this.selectedRecipe);
		if (this.elementContainers == null)
		{
			this.elementContainers = new List<GameObject>();
		}
		else
		{
			this.elementContainers.ForEach(delegate(GameObject ec)
			{
				global::UnityEngine.Object.Destroy(ec.gameObject);
			});
			this.elementContainers.Clear();
		}
		this.buildBtn.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.RECIPE_QUEUE, this.selectedRecipe.Name);
		this.infiniteBuildBtn.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.RECIPE_QUEUE_INFINITE, this.selectedRecipe.Name);
		this.buildBtn.ClearOnClick();
		this.infiniteBuildBtn.ClearOnClick();
		this.buildBtn.onClick += delegate
		{
			this.CreateOrder(false);
		};
		this.infiniteBuildBtn.onClick += delegate
		{
			this.CreateOrder(true);
		};
		string text = GameUtil.GetRecipeDescription(this.selectedRecipe);
		if (!string.IsNullOrEmpty(text))
		{
			text += "\n\n";
		}
		text += UI.UISIDESCREENS.FABRICATORSIDESCREEN.COST;
		List<string> ingredientDescriptions = this.GetIngredientDescriptions(recipeElements);
		string text2 = string.Empty;
		if (ingredientDescriptions != null)
		{
			foreach (string text3 in ingredientDescriptions)
			{
				text2 += string.Format(UI.LISTENTRYSTRING, text3);
			}
		}
		text += text2;
		string text4 = GameUtil.GetGameObjectEffectsString(this.selectedRecipe.Result);
		if (this.selectedRecipe.EffectDescription != null && this.selectedRecipe.EffectDescription.Count > 0)
		{
			for (int i = 0; i < this.selectedRecipe.EffectDescription.Count; i++)
			{
				text4 += string.Format(UI.LISTENTRYSTRING, this.selectedRecipe.EffectDescription[i]);
			}
		}
		if (!string.IsNullOrEmpty(text4))
		{
			text = text + "\n" + UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS + "\n";
			text += text4;
		}
		this.subtitleLabel.SetText(this.selectedRecipe.Result.GetProperName());
		if (this.noRecipeSelectedLabel != null)
		{
			this.noRecipeSelectedLabel.gameObject.SetActive(false);
		}
		this.descriptionLabel.gameObject.SetActive(true);
		this.descriptionLabel.SetText(text);
	}

	public List<string> GetIngredientDescriptions(Element[] elements)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < elements.Length; i++)
		{
			Tag tag = this.selectedRecipe.Ingredients[i].tag;
			GameObject prefab = Assets.GetPrefab(tag);
			string text = GameUtil.GetKeywordStyle(tag);
			if (text == null)
			{
				text = "solid";
			}
			string text2 = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT, text, prefab.GetProperName(), GameUtil.GetFormattedMass(this.selectedRecipe.Ingredients[i].amount, GameUtil.TimeSlice.None, true, "F1"));
			list.Add(string.Format(text2, new object[0]));
		}
		return list;
	}

	private void CreateOrder(bool isInfinite)
	{
		if (this.selectedRecipe == null)
		{
			Debug.LogError("Cannot create an order for a null recipe");
			return;
		}
		List<Tag> list = new List<Tag>();
		foreach (Recipe.Ingredient ingredient in this.selectedRecipe.Ingredients)
		{
			list.Add(ingredient.tag);
		}
		this.targetFab.CreateOrder(this.selectedRecipe, list, isInfinite);
	}

	private Element[] GetRecipeElements(Recipe recipe)
	{
		Element[] array = new Element[recipe.Ingredients.Count];
		for (int i = 0; i < recipe.Ingredients.Count; i++)
		{
			Tag tag = recipe.Ingredients[i].tag;
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

	private Element[] CheckElementsForOrder(Recipe recipe)
	{
		Element[] recipeElements = this.GetRecipeElements(recipe);
		for (int i = 0; i < recipeElements.Length; i++)
		{
			Tag tag = recipe.Ingredients[i].tag;
			foreach (Element element in ElementLoader.elements)
			{
				Tag tag2 = TagManager.Create(element.id);
				if (tag2 == tag)
				{
					recipeElements[i] = element;
					break;
				}
			}
			if (recipeElements[i] == null)
			{
				throw new ArgumentException("Tag in smelter recipe doesn't match element name.");
			}
		}
		return recipeElements;
	}

	[Header("Recipe List")]
	[SerializeField]
	private GameObject recipeGrid;

	[SerializeField]
	private GameObject recipeButton;

	[SerializeField]
	private Sprite buttonSelectedBG;

	[SerializeField]
	private Sprite buttonNormalBG;

	[SerializeField]
	private Sprite elementPlaceholderSpr;

	private KToggle selectedToggle;

	[SerializeField]
	private GameObject elementContainer;

	private List<GameObject> elementContainers;

	[SerializeField]
	private KButton buildBtn;

	[SerializeField]
	private KButton infiniteBuildBtn;

	[SerializeField]
	private BuildQueue queue;

	[SerializeField]
	private LocText descriptionLabel;

	[SerializeField]
	private LocText subtitleLabel;

	[SerializeField]
	private LocText noRecipeSelectedLabel;

	private Dictionary<Fabricator, int> selectedRecipeFabricatorMap = new Dictionary<Fabricator, int>();

	private Fabricator targetFab;

	private Recipe selectedRecipe;

	private Dictionary<KToggle, Recipe> recipeMap;

	private List<KToggle> recipeToggles = new List<KToggle>();
}
