using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class RefinerySideScreen : SideScreenContent
{
	public override string GetTitle()
	{
		if (this.targetFab == null)
		{
			return Strings.Get(this.titleKey).ToString().Replace("{0}", string.Empty);
		}
		return string.Format(Strings.Get(this.titleKey), this.targetFab.GetProperName());
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Refinery>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		Refinery component = target.GetComponent<Refinery>();
		if (component == null)
		{
			global::Debug.LogError("The object selected doesn't have a Refinery!", null);
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

	public void Initialize(Refinery target)
	{
		if (target == null)
		{
			global::Debug.LogError("Refinery provided was null.", null);
			return;
		}
		this.targetFab = target;
		base.gameObject.SetActive(true);
		RefinementRecipe[] recipes = this.targetFab.GetRecipes();
		Array.Sort<RefinementRecipe>(recipes, (RefinementRecipe a, RefinementRecipe b) => a.sortOrder - b.sortOrder);
		this.recipeMap = new Dictionary<KToggle, RefinementRecipe>();
		this.recipeToggles.ForEach(delegate(KToggle rbi)
		{
			global::UnityEngine.Object.Destroy(rbi.gameObject);
		});
		this.recipeToggles.Clear();
		RefinementRecipe[] array = recipes;
		for (int i = 0; i < array.Length; i++)
		{
			RefinementRecipe refinementRecipe = array[i];
			if (WorldInventory.Instance.IsDiscovered(refinementRecipe.material) || DebugHandler.InstantBuildMode)
			{
				GameObject prefab = Assets.GetPrefab(refinementRecipe.material);
				KToggle newToggle = global::Util.KInstantiateUI<KToggle>(this.recipeButton, this.recipeGrid, false);
				newToggle.GetComponentInChildren<LocText>().text = refinementRecipe.material.ProperName();
				KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
				Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui");
				if (uispriteFromMultiObjectAnim == null)
				{
					uispriteFromMultiObjectAnim = this.elementPlaceholderSpr;
				}
				Image componentInChildrenOnly = newToggle.gameObject.GetComponentInChildrenOnly<Image>();
				componentInChildrenOnly.sprite = uispriteFromMultiObjectAnim;
				newToggle.onClick += delegate
				{
					this.ToggleClicked(newToggle);
				};
				newToggle.gameObject.SetActive(true);
				this.recipeMap.Add(newToggle, refinementRecipe);
				this.recipeToggles.Add(newToggle);
			}
		}
		if (this.recipeToggles.Count > 0)
		{
			bool flag = false;
			if (this.selectedRecipeFabricatorMap.ContainsKey(this.targetFab))
			{
				int num = this.selectedRecipeFabricatorMap[this.targetFab];
				if (num < this.recipeToggles.Count)
				{
					this.ToggleClicked(this.recipeToggles[num]);
					flag = true;
				}
			}
			if (!flag)
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
					this.noRecipeSelectedLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.SELECTRECIPE);
					this.noRecipeSelectedLabel.gameObject.SetActive(true);
					this.IngredientsDescriptorPanel.gameObject.SetActive(false);
					this.EffectsDescriptorPanel.gameObject.SetActive(false);
				}
				this.buildBtn.isInteractable = false;
				this.infiniteBuildBtn.isInteractable = false;
			}
		}
		else
		{
			this.subtitleLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPEDISCOVERED);
			this.descriptionLabel.gameObject.SetActive(false);
			if (this.noRecipeSelectedLabel != null)
			{
				this.noRecipeSelectedLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPEDISCOVERED_BODY);
				this.noRecipeSelectedLabel.gameObject.SetActive(true);
				this.IngredientsDescriptorPanel.gameObject.SetActive(false);
				this.EffectsDescriptorPanel.gameObject.SetActive(false);
			}
			this.buildBtn.isInteractable = false;
			this.infiniteBuildBtn.isInteractable = false;
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
		this.buildBtn.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.RECIPE_QUEUE, this.selectedRecipe.material.ProperName());
		this.infiniteBuildBtn.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.RECIPE_QUEUE_INFINITE, this.selectedRecipe.material.ProperName());
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
		this.subtitleLabel.SetText(this.selectedRecipe.material.ProperName());
		if (this.noRecipeSelectedLabel != null)
		{
			this.noRecipeSelectedLabel.gameObject.SetActive(false);
		}
		this.descriptionLabel.gameObject.SetActive(true);
		this.descriptionLabel.SetText(this.selectedRecipe.description);
		this.RefreshIngredientDescriptors();
		this.RefreshResultDescriptors();
	}

	private void RefreshResultDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(this.GetResultDescription(this.selectedRecipe));
		list.AddRange(this.targetFab.AdditionalEffectsForRecipe(this.selectedRecipe));
		if (list.Count > 0)
		{
			GameUtil.IndentListOfDescriptors(list);
			list.Insert(0, new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS, UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS, Descriptor.DescriptorType.Effect, false));
			this.EffectsDescriptorPanel.gameObject.SetActive(true);
		}
		this.EffectsDescriptorPanel.SetDescriptors(list);
	}

	private void RefreshIngredientDescriptors()
	{
		if (this.selectedRecipe == null)
		{
			return;
		}
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.COST, UI.UISIDESCREENS.FABRICATORSIDESCREEN.COST, Descriptor.DescriptorType.Requirement, false));
		Descriptor ingredientDescription = this.GetIngredientDescription(this.selectedRecipe);
		ingredientDescription.IncreaseIndent();
		list.Add(ingredientDescription);
		this.IngredientsDescriptorPanel.gameObject.SetActive(true);
		this.IngredientsDescriptorPanel.SetDescriptors(list);
	}

	private void Update()
	{
		this.RefreshIngredientDescriptors();
	}

	public Descriptor GetIngredientDescription(RefinementRecipe recipe)
	{
		GameObject prefab = Assets.GetPrefab(recipe.material);
		float amount = WorldInventory.Instance.GetAmount(recipe.material);
		LocString reciperquirement = UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT;
		LocString locString = UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPERQUIREMENT_INSUFFICIENT;
		if (WorldInventory.Instance.GetAmount(recipe.material) >= recipe.amount)
		{
			locString = UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPERQUIREMENT_SUFFICIENT;
		}
		string text;
		string text2;
		if (GameTags.DisplayAsCalories.Contains(recipe.material))
		{
			EdiblesManager.FoodInfo foodInfo = EdiblesManager.instance.GetFoodInfo(recipe.material.Name);
			float num = foodInfo.CaloriesPerUnit * recipe.amount;
			text = GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true);
			text2 = GameUtil.GetFormattedCalories(amount * foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true);
		}
		else if (GameTags.DisplayAsUnits.Contains(recipe.material))
		{
			text = GameUtil.GetFormattedUnits(recipe.amount, GameUtil.TimeSlice.None, false);
			text2 = GameUtil.GetFormattedUnits(amount, GameUtil.TimeSlice.None, false);
		}
		else
		{
			text = GameUtil.GetFormattedMass(recipe.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
			text2 = GameUtil.GetFormattedMass(amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
		}
		return new Descriptor(string.Format(reciperquirement, prefab.GetProperName(), text, text2), string.Format(locString, prefab.GetProperName(), text, text2), Descriptor.DescriptorType.Requirement, false);
	}

	public List<Descriptor> GetResultDescription(RefinementRecipe recipe)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (RefinementRecipe.Result result in recipe.results)
		{
			GameObject prefab = Assets.GetPrefab(result.tag);
			string text;
			if (GameTags.DisplayAsCalories.Contains(result.tag))
			{
				EdiblesManager.FoodInfo foodInfo = EdiblesManager.instance.GetFoodInfo(result.tag.Name);
				float num = foodInfo.CaloriesPerUnit * result.amount;
				text = GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true);
			}
			else if (GameTags.DisplayAsUnits.Contains(result.tag))
			{
				text = GameUtil.GetFormattedUnits(result.amount, GameUtil.TimeSlice.None, true);
			}
			else
			{
				text = GameUtil.GetFormattedMass(result.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
			}
			LocString recipeproduct = UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPEPRODUCT;
			LocString recipeproduct2 = UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPEPRODUCT;
			list.Add(new Descriptor(string.Format(recipeproduct, prefab.GetProperName(), text), string.Format(recipeproduct2, prefab.GetProperName(), text), Descriptor.DescriptorType.Requirement, false));
			List<Descriptor> materialDescriptors = GameUtil.GetMaterialDescriptors(ElementLoader.GetElement(result.tag));
			GameUtil.IndentListOfDescriptors(materialDescriptors);
			list.AddRange(materialDescriptors);
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
		this.targetFab.CreateOrder(this.selectedRecipe, isInfinite, this.createOrderSound);
	}

	private Element[] GetRecipeElements(Recipe recipe)
	{
		Element[] array = new Element[recipe.Ingredients.Count];
		for (int i = 0; i < recipe.Ingredients.Count; i++)
		{
			Tag tag = recipe.Ingredients[i].tag;
			foreach (Element element in ElementLoader.elements)
			{
				Tag tag2 = GameTagExtensions.Create(element.id);
				if (tag2 == tag)
				{
					array[i] = element;
					break;
				}
			}
		}
		return array;
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

	private Dictionary<Refinery, int> selectedRecipeFabricatorMap = new Dictionary<Refinery, int>();

	[EventRef]
	public string createOrderSound;

	private Refinery targetFab;

	private RefinementRecipe selectedRecipe;

	private Dictionary<KToggle, RefinementRecipe> recipeMap;

	private List<KToggle> recipeToggles = new List<KToggle>();
}
