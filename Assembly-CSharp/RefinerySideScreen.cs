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
		this.queue.AddAvailableMaterialStorage(component.inStorage);
		this.queue.AddAvailableMaterialStorage(component.buildStorage);
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
		ComplexRecipe[] recipes = this.targetFab.GetRecipes();
		Array.Sort<ComplexRecipe>(recipes, (ComplexRecipe a, ComplexRecipe b) => a.sortOrder - b.sortOrder);
		this.recipeMap = new Dictionary<KToggle, ComplexRecipe>();
		this.recipeToggles.ForEach(delegate(KToggle rbi)
		{
			global::UnityEngine.Object.Destroy(rbi.gameObject);
		});
		this.recipeToggles.Clear();
		GridLayoutGroup component = this.recipeGrid.GetComponent<GridLayoutGroup>();
		component.constraintCount = ((this.targetFab.sideScreenStyle != RefinerySideScreen.StyleSetting.ListResult && this.targetFab.sideScreenStyle != RefinerySideScreen.StyleSetting.ListInput && this.targetFab.sideScreenStyle != RefinerySideScreen.StyleSetting.ListInputOutput) ? 3 : 1);
		component.cellSize = new Vector2((float)((this.targetFab.sideScreenStyle != RefinerySideScreen.StyleSetting.ListResult && this.targetFab.sideScreenStyle != RefinerySideScreen.StyleSetting.ListInput && this.targetFab.sideScreenStyle != RefinerySideScreen.StyleSetting.ListInputOutput) ? 116 : 348), component.cellSize.y);
		int num = 0;
		ComplexRecipe[] array = recipes;
		for (int i = 0; i < array.Length; i++)
		{
			ComplexRecipe complexRecipe = array[i];
			bool flag = true;
			foreach (ComplexRecipe.RecipeElement recipeElement in complexRecipe.ingredients)
			{
				if (!WorldInventory.Instance.IsDiscovered(recipeElement.material) && !DebugHandler.InstantBuildMode)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				num++;
				Tuple<Sprite, Color> uisprite = Def.GetUISprite(complexRecipe.ingredients[0].material, "ui", false);
				Tuple<Sprite, Color> uisprite2 = Def.GetUISprite(complexRecipe.results[0].material, "ui", false);
				KToggle newToggle;
				if (target.sideScreenStyle == RefinerySideScreen.StyleSetting.GridInputOutput || target.sideScreenStyle == RefinerySideScreen.StyleSetting.ListInputOutput || target.sideScreenStyle == RefinerySideScreen.StyleSetting.ListInputOutput)
				{
					newToggle = global::Util.KInstantiateUI<KToggle>(this.recipeButtonMultiple, this.recipeGrid, false);
					HierarchyReferences component2 = newToggle.GetComponent<HierarchyReferences>();
					foreach (ComplexRecipe.RecipeElement recipeElement2 in complexRecipe.ingredients)
					{
						GameObject gameObject = global::Util.KInstantiateUI(component2.GetReference("FromIconPrefab").gameObject, component2.GetReference("FromIcons").gameObject, true);
						gameObject.GetComponent<Image>().sprite = Def.GetUISprite(recipeElement2.material, "ui", false).first;
						gameObject.GetComponent<Image>().color = Def.GetUISprite(recipeElement2.material, "ui", false).second;
						gameObject.gameObject.name = recipeElement2.material.Name;
					}
					foreach (ComplexRecipe.RecipeElement recipeElement3 in complexRecipe.results)
					{
						GameObject gameObject2 = global::Util.KInstantiateUI(component2.GetReference("ToIconPrefab").gameObject, component2.GetReference("ToIcons").gameObject, true);
						gameObject2.GetComponent<Image>().sprite = Def.GetUISprite(recipeElement3.material, "ui", false).first;
						gameObject2.GetComponent<Image>().color = Def.GetUISprite(recipeElement3.material, "ui", false).second;
						gameObject2.gameObject.name = recipeElement3.material.Name;
					}
				}
				else
				{
					newToggle = global::Util.KInstantiateUI<KToggle>(this.recipeButton, this.recipeGrid, false);
					if (target.sideScreenStyle == RefinerySideScreen.StyleSetting.GridInput || target.sideScreenStyle == RefinerySideScreen.StyleSetting.ListInput)
					{
						Image image = newToggle.gameObject.GetComponentsInChildrenOnly<Image>()[0];
						image.sprite = uisprite.first;
						image.color = uisprite.second;
					}
					else
					{
						Image image2 = newToggle.gameObject.GetComponentsInChildrenOnly<Image>()[0];
						image2.sprite = uisprite2.first;
						image2.color = uisprite2.second;
					}
				}
				newToggle.GetComponentInChildren<LocText>().text = string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO, complexRecipe.ingredients[0].material.ProperName(), complexRecipe.results[0].material.ProperName());
				newToggle.GetComponent<ToolTip>().SetSimpleTooltip(complexRecipe.description);
				newToggle.onClick += delegate
				{
					this.ToggleClicked(newToggle);
				};
				newToggle.gameObject.SetActive(true);
				this.recipeMap.Add(newToggle, complexRecipe);
				this.recipeToggles.Add(newToggle);
			}
		}
		this.buttonScrollContainer.GetComponent<LayoutElement>().minHeight = Mathf.Min(232f, 4f + (float)num * this.recipeButtonMultiple.GetComponent<LayoutElement>().minHeight);
		if (this.recipeToggles.Count > 0)
		{
			bool flag2 = false;
			if (this.selectedRecipeFabricatorMap.ContainsKey(this.targetFab))
			{
				int num2 = this.selectedRecipeFabricatorMap[this.targetFab];
				if (num2 < this.recipeToggles.Count)
				{
					this.ToggleClicked(this.recipeToggles[num2]);
					flag2 = true;
				}
			}
			if (!flag2)
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
		this.buildBtn.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.RECIPE_QUEUE, this.selectedRecipe.results[0].material.ProperName());
		this.infiniteBuildBtn.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.RECIPE_QUEUE_INFINITE, this.selectedRecipe.results[0].material.ProperName());
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
		this.subtitleLabel.SetText(this.selectedRecipe.results[0].material.ProperName());
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

	public Descriptor GetIngredientDescription(ComplexRecipe recipe)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		for (int i = 0; i < recipe.ingredients.Length; i++)
		{
			ComplexRecipe.RecipeElement recipeElement = recipe.ingredients[i];
			GameObject prefab = Assets.GetPrefab(recipeElement.material);
			float amount = WorldInventory.Instance.GetAmount(recipeElement.material);
			LocString reciperquirement = UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT;
			LocString locString = UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPERQUIREMENT_INSUFFICIENT;
			if (WorldInventory.Instance.GetAmount(recipeElement.material) >= recipeElement.amount)
			{
				locString = UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPERQUIREMENT_SUFFICIENT;
			}
			string text3;
			string text4;
			if (GameTags.DisplayAsCalories.Contains(recipeElement.material))
			{
				EdiblesManager.FoodInfo foodInfo = EdiblesManager.instance.GetFoodInfo(recipeElement.material.Name);
				float num = foodInfo.CaloriesPerUnit * recipeElement.amount;
				text3 = GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true);
				text4 = GameUtil.GetFormattedCalories(amount * foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true);
			}
			else if (GameTags.DisplayAsUnits.Contains(recipeElement.material))
			{
				text3 = GameUtil.GetFormattedUnits(recipeElement.amount, GameUtil.TimeSlice.None, false);
				text4 = GameUtil.GetFormattedUnits(amount, GameUtil.TimeSlice.None, false);
			}
			else
			{
				text3 = GameUtil.GetFormattedMass(recipeElement.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
				text4 = GameUtil.GetFormattedMass(amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
			}
			text += string.Format(reciperquirement, prefab.GetProperName(), text3, text4);
			if (i != recipe.ingredients.Length - 1)
			{
				text += "\n    ";
			}
			text2 += string.Format(locString, prefab.GetProperName(), text3, text4);
		}
		return new Descriptor(text, text2, Descriptor.DescriptorType.Requirement, false);
	}

	public List<Descriptor> GetResultDescription(ComplexRecipe recipe)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (ComplexRecipe.RecipeElement recipeElement in recipe.results)
		{
			GameObject prefab = Assets.GetPrefab(recipeElement.material);
			string text;
			if (GameTags.DisplayAsCalories.Contains(recipeElement.material))
			{
				EdiblesManager.FoodInfo foodInfo = EdiblesManager.instance.GetFoodInfo(recipeElement.material.Name);
				float num = foodInfo.CaloriesPerUnit * recipeElement.amount;
				text = GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true);
			}
			else if (GameTags.DisplayAsUnits.Contains(recipeElement.material))
			{
				text = GameUtil.GetFormattedUnits(recipeElement.amount, GameUtil.TimeSlice.None, true);
			}
			else
			{
				text = GameUtil.GetFormattedMass(recipeElement.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
			}
			LocString recipeproduct = UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPEPRODUCT;
			LocString recipeproduct2 = UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPEPRODUCT;
			list.Add(new Descriptor(string.Format(recipeproduct, prefab.GetProperName(), text), string.Format(recipeproduct2, prefab.GetProperName(), text), Descriptor.DescriptorType.Requirement, false));
			Element element = ElementLoader.GetElement(recipeElement.material);
			if (element != null)
			{
				List<Descriptor> materialDescriptors = GameUtil.GetMaterialDescriptors(element);
				GameUtil.IndentListOfDescriptors(materialDescriptors);
				list.AddRange(materialDescriptors);
			}
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
	private GameObject recipeButtonMultiple;

	[SerializeField]
	private Sprite buttonSelectedBG;

	[SerializeField]
	private Sprite buttonNormalBG;

	[SerializeField]
	private Sprite elementPlaceholderSpr;

	private KToggle selectedToggle;

	public LayoutElement buttonScrollContainer;

	public RectTransform buttonContentContainer;

	[SerializeField]
	private GameObject elementContainer;

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

	private Dictionary<Refinery, int> selectedRecipeFabricatorMap = new Dictionary<Refinery, int>();

	[EventRef]
	public string createOrderSound;

	[SerializeField]
	private RectTransform content;

	private Refinery targetFab;

	private ComplexRecipe selectedRecipe;

	private Dictionary<KToggle, ComplexRecipe> recipeMap;

	private List<KToggle> recipeToggles = new List<KToggle>();

	public enum StyleSetting
	{
		GridResult,
		ListResult,
		GridInput,
		ListInput,
		ListInputOutput,
		GridInputOutput
	}
}
