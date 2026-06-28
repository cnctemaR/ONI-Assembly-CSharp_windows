using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
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
			global::Debug.LogError("The object selected doesn't have a fabricator!", null);
			return;
		}
		this.queue.SetFabricator(component);
		this.Initialize(component);
	}

	protected override void OnShow(bool show)
	{
		if (show)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().FabricatorSideScreenOpenSnapshot);
		}
		else
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FabricatorSideScreenOpenSnapshot, STOP_MODE.ALLOWFADEOUT);
		}
		base.OnShow(show);
	}

	public void Initialize(Fabricator target)
	{
		if (target == null)
		{
			global::Debug.LogError("Fabricator provided was null.", null);
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
			GameObject prefab = Assets.GetPrefab(recipe.Result);
			KToggle newToggle = global::Util.KInstantiateUI<KToggle>(this.recipeButton, this.recipeGrid, false);
			newToggle.GetComponentInChildren<LocText>().text = recipe.Name;
			KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
			Sprite sprite = ((!(recipe.Icon == null)) ? recipe.Icon : Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui"));
			if (sprite == null)
			{
				sprite = this.elementPlaceholderSpr;
			}
			Image componentInChildrenOnly = newToggle.gameObject.GetComponentInChildrenOnly<Image>();
			componentInChildrenOnly.sprite = sprite;
			newToggle.onClick += delegate
			{
				this.ToggleClicked(newToggle);
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
				this.ToggleClicked(this.recipeToggles[num]);
			}
			else
			{
				this.selectedRecipe = null;
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
					this.IngredientsDescriptorPanel.gameObject.SetActive(false);
					this.EffectsDescriptorPanel.gameObject.SetActive(false);
				}
				this.buildBtn.isInteractable = false;
				this.infiniteBuildBtn.isInteractable = false;
			}
		}
		this.scrollBarContainer.SetActive(this.recipeToggles.Count > 4);
	}

	private void ToggleClicked(KToggle toggle)
	{
		if (!this.recipeMap.ContainsKey(toggle))
		{
			global::Debug.LogError("Recipe not found on recipe list.", null);
			return;
		}
		this.selectedToggle = toggle;
		this.selectedToggle.isOn = true;
		this.selectedToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
		this.buildBtn.isInteractable = true;
		this.infiniteBuildBtn.isInteractable = true;
		this.recipeToggles.ForEach(delegate(KToggle tg)
		{
			if (tg != this.selectedToggle)
			{
				tg.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			}
		});
		this.selectedRecipe = this.recipeMap[toggle];
		this.selectedRecipeFabricatorMap[this.targetFab] = this.recipeToggles.IndexOf(toggle);
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
		string recipeDescription = GameUtil.GetRecipeDescription(this.selectedRecipe);
		this.subtitleLabel.SetText(this.selectedRecipe.Name);
		if (this.noRecipeSelectedLabel != null)
		{
			this.noRecipeSelectedLabel.gameObject.SetActive(false);
		}
		this.descriptionLabel.gameObject.SetActive(true);
		this.descriptionLabel.SetText(recipeDescription);
		this.RefreshIngredientDescriptors();
		GameObject prefab = Assets.GetPrefab(this.selectedRecipe.Result);
		List<Descriptor> list = new List<Descriptor>();
		List<Descriptor> list2 = new List<Descriptor>();
		list2.AddRange(GameUtil.GetGameObjectRequirements(prefab));
		if (list2.Count > 0)
		{
			GameUtil.IndentListOfDescriptors(list2);
			list2.Insert(0, new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTREQUIREMENTS, UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTREQUIREMENTS, Descriptor.DescriptorType.Effect, false));
			this.EffectsDescriptorPanel.gameObject.SetActive(true);
		}
		List<Descriptor> list3 = new List<Descriptor>();
		list3.AddRange(GameUtil.GetGameObjectEffects(prefab, false));
		if (list3.Count > 0)
		{
			GameUtil.IndentListOfDescriptors(list3);
			list3.Insert(0, new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS, UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS, Descriptor.DescriptorType.Effect, false));
			this.EffectsDescriptorPanel.gameObject.SetActive(true);
		}
		list.AddRange(list2);
		list.AddRange(list3);
		this.EffectsDescriptorPanel.SetDescriptors(list);
	}

	private void RefreshIngredientDescriptors()
	{
		if (this.selectedRecipe == null)
		{
			return;
		}
		List<Descriptor> ingredientDescriptions = this.GetIngredientDescriptions(this.selectedRecipe.Ingredients);
		if (ingredientDescriptions.Count > 0)
		{
			GameUtil.IndentListOfDescriptors(ingredientDescriptions);
			ingredientDescriptions.Insert(0, new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.COST, UI.UISIDESCREENS.FABRICATORSIDESCREEN.COST, Descriptor.DescriptorType.Requirement, false));
			this.IngredientsDescriptorPanel.gameObject.SetActive(true);
		}
		this.IngredientsDescriptorPanel.SetDescriptors(ingredientDescriptions);
	}

	private void Update()
	{
		this.RefreshIngredientDescriptors();
	}

	public List<Descriptor> GetIngredientDescriptions(List<Recipe.Ingredient> ingredients)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Recipe.Ingredient ingredient in ingredients)
		{
			Tag tag = ingredient.tag;
			LocString reciperquirement = UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT;
			LocString locString = UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPERQUIREMENT_INSUFFICIENT;
			if (WorldInventory.Instance.GetAmount(tag) >= ingredient.amount)
			{
				locString = UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPERQUIREMENT_SUFFICIENT;
			}
			string text;
			string text2;
			if (GameTags.DisplayAsCalories.Contains(tag))
			{
				EdiblesManager.FoodInfo foodInfo = EdiblesManager.instance.GetFoodInfo(tag.Name);
				float num = foodInfo.CaloriesPerUnit * ingredient.amount;
				text = GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true);
				float num2 = WorldInventory.Instance.GetAmount(tag) * foodInfo.CaloriesPerUnit;
				text2 = GameUtil.GetFormattedCalories(num2, GameUtil.TimeSlice.None, true);
			}
			else if (GameTags.DisplayAsUnits.Contains(tag))
			{
				text = GameUtil.GetFormattedUnits(ingredient.amount, GameUtil.TimeSlice.None, false);
				text2 = GameUtil.GetFormattedUnits(WorldInventory.Instance.GetAmount(tag), GameUtil.TimeSlice.None, false);
			}
			else
			{
				text = GameUtil.GetFormattedMass(ingredient.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
				text2 = GameUtil.GetFormattedMass(WorldInventory.Instance.GetAmount(tag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
			}
			Descriptor descriptor = new Descriptor(string.Format(reciperquirement, tag.ProperName(), text, text2), string.Format(locString, tag.ProperName(), text, text2), Descriptor.DescriptorType.Requirement, false);
			list.Add(descriptor);
		}
		return list;
	}

	private void CreateOrder(bool isInfinite)
	{
		if (this.selectedRecipe == null)
		{
			global::Debug.LogError("Cannot create an order for a null recipe", null);
			return;
		}
		List<Tag> list = new List<Tag>();
		foreach (Recipe.Ingredient ingredient in this.selectedRecipe.Ingredients)
		{
			list.Add(ingredient.tag);
		}
		this.targetFab.CreateOrder(this.selectedRecipe, list, isInfinite, this.createOrderSound);
	}

	public DescriptorPanel IngredientsDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;

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

	[SerializeField]
	private KButton buildBtn;

	[SerializeField]
	private KButton infiniteBuildBtn;

	[SerializeField]
	private BuildQueue queue;

	[SerializeField]
	private GameObject scrollBarContainer;

	[SerializeField]
	private LocText descriptionLabel;

	[SerializeField]
	private LocText subtitleLabel;

	[SerializeField]
	private LocText noRecipeSelectedLabel;

	private Dictionary<Fabricator, int> selectedRecipeFabricatorMap = new Dictionary<Fabricator, int>();

	[EventRef]
	public string createOrderSound;

	private Fabricator targetFab;

	private Recipe selectedRecipe;

	private Dictionary<KToggle, Recipe> recipeMap;

	private List<KToggle> recipeToggles = new List<KToggle>();
}
