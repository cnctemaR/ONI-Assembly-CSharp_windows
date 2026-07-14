using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ComplexFabricatorSideScreen : SideScreenContent
{
	public override string GetTitle()
	{
		if (this.targetFab == null)
		{
			return Strings.Get(this.titleKey).ToString().Replace("{0}", "");
		}
		return string.Format(Strings.Get(this.titleKey), this.targetFab.GetProperName());
	}

	public override bool IsValidForTarget(GameObject target)
	{
		ComplexFabricator component = target.GetComponent<ComplexFabricator>();
		return component != null && component.enabled;
	}

	public override void SetTarget(GameObject target)
	{
		ComplexFabricator component = target.GetComponent<ComplexFabricator>();
		if (component == null)
		{
			global::Debug.LogError("The object selected doesn't have a ComplexFabricator!");
			return;
		}
		this.UnsubscribeTarget();
		this.Initialize(component);
		this.targetOrdersUpdatedSubHandle = this.targetFab.Subscribe(1721324763, new Action<object>(this.UpdateQueueCountLabels));
		this.UpdateQueueCountLabels(null);
	}

	private void UpdateQueueCountLabels(object data = null)
	{
		ComplexRecipe[] recipes = this.targetFab.GetRecipes();
		for (int i = 0; i < recipes.Length; i++)
		{
			ComplexRecipe r = recipes[i];
			GameObject gameObject = this.recipeToggles.Find((GameObject match) => this.recipeCategoryToggleMap[match].Contains(r));
			if (gameObject != null)
			{
				this.RefreshQueueCountDisplay(gameObject, this.targetFab);
				this.RefreshQueueTooltip(gameObject);
			}
		}
		if (this.targetFab.CurrentWorkingOrder != null)
		{
			this.currentOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.CURRENT_ORDER, this.targetFab.CurrentWorkingOrder.GetUIName(false));
		}
		else
		{
			this.currentOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.CURRENT_ORDER, UI.UISIDESCREENS.FABRICATORSIDESCREEN.NO_WORKABLE_ORDER);
		}
		if (this.targetFab.NextOrder != null)
		{
			this.nextOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NEXT_ORDER, this.targetFab.NextOrder.GetUIName(false));
			return;
		}
		this.nextOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NEXT_ORDER, UI.UISIDESCREENS.FABRICATORSIDESCREEN.NO_WORKABLE_ORDER);
	}

	protected override void OnShow(bool show)
	{
		if (show)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().FabricatorSideScreenOpenSnapshot);
		}
		else
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FabricatorSideScreenOpenSnapshot, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			DetailsScreen.Instance.ClearSecondarySideScreen();
			this.selectedRecipeCategory = "";
			this.selectedToggle = null;
		}
		base.OnShow(show);
	}

	public void Initialize(ComplexFabricator target)
	{
		if (target == null)
		{
			global::Debug.LogError("ComplexFabricator provided was null.");
			return;
		}
		this.targetFab = target;
		base.gameObject.SetActive(true);
		this.recipeCategoryToggleMap = new Dictionary<GameObject, List<ComplexRecipe>>();
		this.recipeToggles.ForEach(delegate(GameObject rbi)
		{
			global::UnityEngine.Object.Destroy(rbi.gameObject);
		});
		this.recipeToggles.Clear();
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.recipeCategories)
		{
			global::UnityEngine.Object.Destroy(keyValuePair.Value.transform.parent.gameObject);
		}
		this.recipeCategories.Clear();
		int num = 0;
		ComplexRecipe[] recipes = this.targetFab.GetRecipes();
		Dictionary<string, List<ComplexRecipe>> dictionary = new Dictionary<string, List<ComplexRecipe>>();
		foreach (ComplexRecipe complexRecipe in recipes)
		{
			if (!dictionary.ContainsKey(complexRecipe.recipeCategoryID))
			{
				dictionary.Add(complexRecipe.recipeCategoryID, new List<ComplexRecipe>());
			}
			dictionary[complexRecipe.recipeCategoryID].Add(complexRecipe);
		}
		HashSet<string> hashSet = new HashSet<string>();
		Predicate<ComplexRecipe> <>9__1;
		Predicate<ComplexRecipe> <>9__2;
		Predicate<ComplexRecipe> <>9__3;
		Predicate<ComplexRecipe> <>9__4;
		Predicate<ComplexRecipe> <>9__5;
		Predicate<ComplexRecipe> <>9__7;
		foreach (KeyValuePair<string, List<ComplexRecipe>> keyValuePair2 in dictionary)
		{
			ComplexRecipe complexRecipe2 = keyValuePair2.Value[0];
			bool flag = false;
			if (DebugHandler.InstantBuildMode)
			{
				flag = true;
			}
			else if (keyValuePair2.Value[0].RequiresTechUnlock())
			{
				if (keyValuePair2.Value[0].IsRequiredTechOrPOIUnlocked() || Db.Get().Techs.Get(keyValuePair2.Value[0].requiredTech).ArePrerequisitesComplete())
				{
					if (keyValuePair2.Value[0].RequiresAllIngredientsDiscovered)
					{
						List<ComplexRecipe> value = keyValuePair2.Value;
						Predicate<ComplexRecipe> predicate;
						if ((predicate = <>9__1) == null)
						{
							predicate = (<>9__1 = (ComplexRecipe match) => this.AllRecipeRequirementsDiscovered(match));
						}
						if (value.Find(predicate) == null)
						{
							goto IL_0375;
						}
					}
					flag = true;
				}
			}
			else
			{
				List<ComplexRecipe> value2 = keyValuePair2.Value;
				Predicate<ComplexRecipe> predicate2;
				if ((predicate2 = <>9__2) == null)
				{
					predicate2 = (<>9__2 = (ComplexRecipe match) => target.GetRecipeQueueCount(match) != 0);
				}
				if (value2.Find(predicate2) != null)
				{
					flag = true;
				}
				else if (keyValuePair2.Value[0].RequiresAllIngredientsDiscovered)
				{
					List<ComplexRecipe> value3 = keyValuePair2.Value;
					Predicate<ComplexRecipe> predicate3;
					if ((predicate3 = <>9__3) == null)
					{
						predicate3 = (<>9__3 = (ComplexRecipe match) => this.AllRecipeRequirementsDiscovered(match));
					}
					if (value3.Find(predicate3) != null)
					{
						flag = true;
					}
				}
				else
				{
					List<ComplexRecipe> value4 = keyValuePair2.Value;
					Predicate<ComplexRecipe> predicate4;
					if ((predicate4 = <>9__4) == null)
					{
						predicate4 = (<>9__4 = (ComplexRecipe match) => this.AnyRecipeRequirementsDiscovered(match));
					}
					if (value4.Find(predicate4) != null)
					{
						flag = true;
					}
					else
					{
						List<ComplexRecipe> value5 = keyValuePair2.Value;
						Predicate<ComplexRecipe> predicate5;
						if ((predicate5 = <>9__5) == null)
						{
							predicate5 = (<>9__5 = (ComplexRecipe match) => this.HasAnyRecipeRequirements(match));
						}
						if (value5.Find(predicate5) != null)
						{
							flag = true;
						}
					}
				}
			}
			IL_0375:
			if (!flag)
			{
				hashSet.Add(complexRecipe2.GetUIName(false));
			}
			else
			{
				num++;
				global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(complexRecipe2.ingredients[0].material, "ui", false);
				global::Tuple<Sprite, Color> uisprite2 = Def.GetUISprite(complexRecipe2.results[0].material, complexRecipe2.results[0].facadeID);
				KToggle newToggle = null;
				GameObject gameObject;
				if (target.sideScreenStyle == ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid)
				{
					newToggle = global::Util.KInstantiateUI<KToggle>(this.recipeButtonQueueHybrid, this.recipeGrid, false);
					gameObject = newToggle.gameObject;
					this.recipeCategoryToggleMap.Add(gameObject, keyValuePair2.Value);
					Image image = gameObject.GetComponentsInChildrenOnly<Image>()[2];
					if (complexRecipe2.nameDisplay == ComplexRecipe.RecipeNameDisplay.Ingredient)
					{
						image.sprite = uisprite.first;
						image.color = uisprite.second;
					}
					else if (complexRecipe2.nameDisplay == ComplexRecipe.RecipeNameDisplay.HEP)
					{
						image.sprite = this.radboltSprite;
					}
					else if (complexRecipe2.nameDisplay == ComplexRecipe.RecipeNameDisplay.Custom)
					{
						image.sprite = complexRecipe2.GetUIIcon();
					}
					else
					{
						image.sprite = uisprite2.first;
						image.color = uisprite2.second;
					}
					gameObject.GetComponentInChildren<LocText>().text = complexRecipe2.GetUIName(false);
					List<ComplexRecipe> value6 = keyValuePair2.Value;
					Predicate<ComplexRecipe> predicate6;
					if ((predicate6 = <>9__7) == null)
					{
						predicate6 = (<>9__7 = (ComplexRecipe match) => this.HasAllRecipeRequirements(match));
					}
					bool flag2 = value6.Find(predicate6) != null;
					image.material = (flag2 ? Assets.UIPrefabs.TableScreenWidgets.DefaultUIMaterial : Assets.UIPrefabs.TableScreenWidgets.DesaturatedUIMaterial);
					this.RefreshQueueCountDisplay(gameObject, this.targetFab);
					this.RefreshQueueTooltip(gameObject);
					gameObject.gameObject.SetActive(true);
				}
				else
				{
					newToggle = global::Util.KInstantiateUI<KToggle>(this.recipeButton, this.recipeGrid, false);
					gameObject = newToggle.gameObject;
					Image componentInChildrenOnly = newToggle.gameObject.GetComponentInChildrenOnly<Image>();
					if (target.sideScreenStyle == ComplexFabricatorSideScreen.StyleSetting.GridInput || target.sideScreenStyle == ComplexFabricatorSideScreen.StyleSetting.ListInput)
					{
						componentInChildrenOnly.sprite = uisprite.first;
						componentInChildrenOnly.color = uisprite.second;
					}
					else
					{
						componentInChildrenOnly.sprite = uisprite2.first;
						componentInChildrenOnly.color = uisprite2.second;
					}
				}
				ToolTip reference = gameObject.GetComponent<HierarchyReferences>().GetReference<ToolTip>("ButtonTooltip");
				reference.toolTipPosition = ToolTip.TooltipPosition.Custom;
				reference.parentPositionAnchor = new Vector2(0f, 0.5f);
				reference.tooltipPivot = new Vector2(1f, 1f);
				reference.tooltipPositionOffset = new Vector2(-24f, 20f);
				reference.ClearMultiStringTooltip();
				reference.AddMultiStringTooltip(complexRecipe2.GetUIName(false), this.styleTooltipHeader);
				reference.AddMultiStringTooltip(complexRecipe2.description, this.styleTooltipBody);
				if (complexRecipe2.runTimeDescription != null)
				{
					reference.AddMultiStringTooltip("\n" + complexRecipe2.runTimeDescription(), this.styleTooltipBody);
				}
				if (keyValuePair2.Value.Count > 1)
				{
					reference.AddMultiStringTooltip("\n" + UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.ADDITIONAL_INGREDIENT_OPTIONS_MESSAGE, this.styleTooltipBody);
				}
				newToggle.onClick += delegate
				{
					this.ToggleClicked(newToggle);
				};
				gameObject.SetActive(true);
				this.recipeToggles.Add(gameObject);
			}
		}
		if (this.recipeToggles.Count > 0)
		{
			VerticalLayoutGroup component = this.buttonContentContainer.GetComponent<VerticalLayoutGroup>();
			this.buttonScrollContainer.GetComponent<LayoutElement>().minHeight = Mathf.Min(451f, (float)(component.padding.top + component.padding.bottom) + (float)num * this.recipeButtonQueueHybrid.GetComponent<LayoutElement>().minHeight + (float)(num - 1) * component.spacing);
			string text = this.targetFab.SideScreenSubtitleLabel;
			if (hashSet.Count > 0)
			{
				text = string.Concat(new string[]
				{
					text,
					"  <color=#f5b042>(",
					(dictionary.Count - hashSet.Count).ToString(),
					"/",
					dictionary.Count.ToString(),
					")</color>"
				});
			}
			this.subtitleLabel.SetText(text);
			this.noRecipesDiscoveredLabel.gameObject.SetActive(false);
		}
		else
		{
			string text = string.Concat(new string[]
			{
				UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPEDISCOVERED,
				"  <color=#f5b042>(",
				(dictionary.Count - hashSet.Count).ToString(),
				"/",
				dictionary.Count.ToString(),
				")</color>"
			});
			this.subtitleLabel.SetText(text);
			this.noRecipesDiscoveredLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPEDISCOVERED_BODY);
			this.noRecipesDiscoveredLabel.gameObject.SetActive(true);
			this.buttonScrollContainer.GetComponent<LayoutElement>().minHeight = this.noRecipesDiscoveredLabel.GetComponent<LayoutElement>().minHeight + 10f;
		}
		if (hashSet.Count > 0)
		{
			this.subtitleTooltip.SetSimpleTooltip(UI.UISIDESCREENS.FABRICATORSIDESCREEN.UNDISCOVERED_RECIPES + "\n\n    • " + string.Join("\n    • ", hashSet.ToArray<string>()));
		}
		else
		{
			this.subtitleTooltip.SetSimpleTooltip("");
		}
		this.RefreshIngredientAvailabilityVis();
	}

	public void RefreshQueueCountDisplayForRecipeCategory(string recipeCategoryID, ComplexFabricator fabricator)
	{
		foreach (GameObject gameObject in this.recipeToggles)
		{
			if (this.recipeCategoryToggleMap[gameObject][0].recipeCategoryID == recipeCategoryID)
			{
				this.RefreshQueueCountDisplay(gameObject, fabricator);
				this.RefreshQueueTooltip(gameObject);
				break;
			}
		}
	}

	private void RefreshQueueCountDisplay(GameObject entryGO, ComplexFabricator fabricator)
	{
		HierarchyReferences component = entryGO.GetComponent<HierarchyReferences>();
		int recipeCategoryQueueCount = fabricator.GetRecipeCategoryQueueCount(this.recipeCategoryToggleMap[entryGO][0].recipeCategoryID);
		bool flag = recipeCategoryQueueCount == ComplexFabricator.QUEUE_INFINITE;
		component.GetReference<LocText>("CountLabel").text = (flag ? "" : recipeCategoryQueueCount.ToString());
		component.GetReference<RectTransform>("InfiniteIcon").gameObject.SetActive(flag);
		bool flag2 = !this.recipeCategoryToggleMap[entryGO][0].IsRequiredTechOrPOIUnlocked();
		GameObject gameObject = component.GetReference<RectTransform>("TechRequired").gameObject;
		gameObject.SetActive(flag2);
		KButton component2 = gameObject.GetComponent<KButton>();
		component2.ClearOnClick();
		if (flag2)
		{
			component2.onClick += delegate
			{
				ManagementMenu.Instance.OpenResearch(this.recipeCategoryToggleMap[entryGO][0].requiredTech);
			};
		}
		KButton reference = component.GetReference<KButton>("QueueBoxButton");
		reference.bgImage.colorStyleSetting = ((recipeCategoryQueueCount == 0) ? this.emptyQueueColorStyle : this.standardQueueColorStyle);
		reference.bgImage.ApplyColorStyleSetting();
		reference.ClearOnClick();
		string recipeCategoryID = this.recipeCategoryToggleMap[entryGO][0].recipeCategoryID;
		reference.onClick += delegate
		{
			if (this.selectedToggle == null || this.selectedToggle.gameObject != entryGO.gameObject)
			{
				this.ToggleClicked(entryGO.GetComponent<KToggle>());
			}
			else
			{
				this.recipeScreen.SelectNextQueuedRecipeInCategory();
			}
			this.RefreshQueueTooltip(entryGO);
		};
		GameObject gameObject2 = component.GetReference<RectTransform>("DotContainer").gameObject;
		GameObject gameObject3 = component.GetReference<RectTransform>("DotPrefab").gameObject;
		for (int i = 0; i < gameObject2.transform.childCount; i++)
		{
			if (gameObject2.transform.GetChild(i).gameObject != gameObject3)
			{
				global::UnityEngine.Object.Destroy(gameObject2.transform.GetChild(i).gameObject);
			}
		}
		int num = (from match in fabricator.GetRecipesWithCategoryID(this.recipeCategoryToggleMap[entryGO][0].recipeCategoryID)
			where this.targetFab.GetRecipeQueueCount(match) != 0
			select match).Count<ComplexRecipe>();
		if (num > 1)
		{
			for (int j = 0; j < Mathf.Min(num, 5); j++)
			{
				global::Util.KInstantiateUI(gameObject3, gameObject2, false).SetActive(true);
			}
		}
	}

	private void RefreshQueueTooltip(GameObject entryGO)
	{
		HierarchyReferences component = entryGO.GetComponent<HierarchyReferences>();
		string recipeCategoryID = this.recipeCategoryToggleMap[entryGO][0].recipeCategoryID;
		ToolTip reference = component.GetReference<ToolTip>("QueueTooltip");
		int recipeCategoryQueueCount = this.targetFab.GetRecipeCategoryQueueCount(recipeCategoryID);
		if (recipeCategoryQueueCount != 0)
		{
			string text = "<b>" + UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_QUEUE + ((recipeCategoryQueueCount == ComplexFabricator.QUEUE_INFINITE) ? "99+" : recipeCategoryQueueCount.ToString()) + "</b>\n";
			foreach (ComplexRecipe complexRecipe in this.targetFab.GetRecipesWithCategoryID(this.recipeCategoryToggleMap[entryGO][0].recipeCategoryID))
			{
				int recipeQueueCount = this.targetFab.GetRecipeQueueCount(complexRecipe);
				if (recipeQueueCount != 0)
				{
					string text2 = "";
					foreach (ComplexRecipe.RecipeElement recipeElement in complexRecipe.ingredients)
					{
						if (text2 != "")
						{
							text2 += ", ";
						}
						text2 = text2 + "<color=#C76B99>" + TagManager.GetProperName(recipeElement.material, true) + "</color>";
					}
					if (recipeQueueCount == ComplexFabricator.QUEUE_INFINITE)
					{
						text2 = UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_FOREVER + ": " + text2;
					}
					else
					{
						text2 = recipeQueueCount.ToString() + "x " + text2;
					}
					if (text != "")
					{
						text += "\n";
					}
					if (this.recipeScreen != null && this.recipeScreen.gameObject.activeInHierarchy && this.recipeScreen.IsSelectedMaterials(complexRecipe))
					{
						text2 = "<b>" + text2 + "</b>";
					}
					text += text2;
				}
			}
			text = text + "\n\n" + UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_QUEUE_CLICK_DESCRIPTION;
			reference.SetSimpleTooltip(text);
			return;
		}
		reference.SetSimpleTooltip(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_NONE);
	}

	private void ToggleClicked(KToggle toggle)
	{
		if (!this.recipeCategoryToggleMap.ContainsKey(toggle.gameObject))
		{
			global::Debug.LogError("Recipe not found on recipe list.");
			return;
		}
		if (this.selectedToggle == toggle)
		{
			this.selectedToggle.isOn = false;
			this.selectedToggle = null;
			this.selectedRecipeCategory = "";
		}
		else
		{
			this.selectedToggle = toggle;
			this.selectedToggle.isOn = true;
			this.selectedRecipeCategory = this.recipeCategoryToggleMap[toggle.gameObject][0].recipeCategoryID;
			this.selectedRecipeFabricatorMap[this.targetFab] = this.recipeToggles.IndexOf(toggle.gameObject);
		}
		this.RefreshIngredientAvailabilityVis();
		if (toggle.isOn)
		{
			this.recipeScreen = (SelectedRecipeQueueScreen)DetailsScreen.Instance.SetSecondarySideScreen(this.recipeScreenPrefab, this.targetFab.SideScreenRecipeScreenTitle);
			this.recipeScreen.SetRecipeCategory(this, this.targetFab, this.selectedRecipeCategory);
			return;
		}
		DetailsScreen.Instance.ClearSecondarySideScreen();
	}

	public void CycleRecipe(int increment)
	{
		int num = 0;
		if (this.selectedToggle != null)
		{
			num = this.recipeToggles.IndexOf(this.selectedToggle.gameObject);
		}
		int num2 = (num + increment) % this.recipeToggles.Count;
		if (num2 < 0)
		{
			num2 = this.recipeToggles.Count + num2;
		}
		this.ToggleClicked(this.recipeToggles[num2].GetComponent<KToggle>());
	}

	private bool HasAnyRecipeRequirements(ComplexRecipe recipe)
	{
		foreach (ComplexRecipe.RecipeElement recipeElement in recipe.ingredients)
		{
			if (this.targetFab.GetMyWorld().worldInventory.GetAmountWithoutTag(recipeElement.material, true, this.targetFab.ForbiddenTags) + this.targetFab.inStorage.GetAmountAvailable(recipeElement.material, this.targetFab.ForbiddenTags) + this.targetFab.buildStorage.GetAmountAvailable(recipeElement.material, this.targetFab.ForbiddenTags) >= recipeElement.amount)
			{
				return true;
			}
		}
		return false;
	}

	private bool HasAllRecipeRequirements(ComplexRecipe recipe)
	{
		bool flag = true;
		foreach (ComplexRecipe.RecipeElement recipeElement in recipe.ingredients)
		{
			if (this.targetFab.GetMyWorld().worldInventory.GetAmountWithoutTag(recipeElement.material, true, this.targetFab.ForbiddenTags) + this.targetFab.inStorage.GetAmountAvailable(recipeElement.material, this.targetFab.ForbiddenTags) + this.targetFab.buildStorage.GetAmountAvailable(recipeElement.material, this.targetFab.ForbiddenTags) < recipeElement.amount)
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	private bool AnyRecipeRequirementsDiscovered(ComplexRecipe recipe)
	{
		foreach (ComplexRecipe.RecipeElement recipeElement in recipe.ingredients)
		{
			if (DiscoveredResources.Instance.IsDiscovered(recipeElement.material))
			{
				return true;
			}
		}
		return false;
	}

	private bool AllRecipeRequirementsDiscovered(ComplexRecipe recipe)
	{
		foreach (ComplexRecipe.RecipeElement recipeElement in recipe.ingredients)
		{
			if (!DiscoveredResources.Instance.IsDiscovered(recipeElement.material))
			{
				return false;
			}
		}
		return true;
	}

	private void Update()
	{
		this.RefreshIngredientAvailabilityVis();
	}

	private void UnsubscribeTarget()
	{
		if (this.targetOrdersUpdatedSubHandle != -1 && this.targetFab != null)
		{
			this.targetFab.Unsubscribe(this.targetOrdersUpdatedSubHandle);
			this.targetOrdersUpdatedSubHandle = -1;
		}
	}

	public override void ClearTarget()
	{
		base.ClearTarget();
		this.UnsubscribeTarget();
	}

	private void RefreshIngredientAvailabilityVis()
	{
		foreach (KeyValuePair<GameObject, List<ComplexRecipe>> keyValuePair in this.recipeCategoryToggleMap)
		{
			HierarchyReferences component = keyValuePair.Key.GetComponent<HierarchyReferences>();
			bool flag = keyValuePair.Value.Find((ComplexRecipe match) => this.HasAllRecipeRequirements(match)) != null;
			KToggle component2 = keyValuePair.Key.GetComponent<KToggle>();
			if (flag)
			{
				if (keyValuePair.Value[0].recipeCategoryID == this.selectedRecipeCategory)
				{
					component2.ActivateFlourish(true, ImageToggleState.State.Active);
				}
				else
				{
					component2.ActivateFlourish(false, ImageToggleState.State.Inactive);
				}
			}
			else if (keyValuePair.Value[0].recipeCategoryID == this.selectedRecipeCategory)
			{
				component2.ActivateFlourish(true, ImageToggleState.State.DisabledActive);
			}
			else
			{
				component2.ActivateFlourish(false, ImageToggleState.State.Disabled);
			}
			component.GetReference<LocText>("Label").color = (flag ? Color.black : new Color(0.22f, 0.22f, 0.22f, 1f));
		}
	}

	[Header("Recipe List")]
	[SerializeField]
	private GameObject recipeGrid;

	[Header("Recipe button variants")]
	[SerializeField]
	private GameObject recipeButton;

	[SerializeField]
	private GameObject recipeButtonMultiple;

	[SerializeField]
	private GameObject recipeButtonQueueHybrid;

	[SerializeField]
	private GameObject recipeCategoryHeader;

	[SerializeField]
	private Sprite buttonSelectedBG;

	[SerializeField]
	private Sprite buttonNormalBG;

	[SerializeField]
	private Sprite elementPlaceholderSpr;

	[SerializeField]
	public Sprite radboltSprite;

	private KToggle selectedToggle;

	public LayoutElement buttonScrollContainer;

	public RectTransform buttonContentContainer;

	[SerializeField]
	private GameObject elementContainer;

	[SerializeField]
	private LocText currentOrderLabel;

	[SerializeField]
	private LocText nextOrderLabel;

	private Dictionary<ComplexFabricator, int> selectedRecipeFabricatorMap = new Dictionary<ComplexFabricator, int>();

	public EventReference createOrderSound;

	[SerializeField]
	private RectTransform content;

	[SerializeField]
	private LocText subtitleLabel;

	[SerializeField]
	private ToolTip subtitleTooltip;

	[SerializeField]
	private LocText noRecipesDiscoveredLabel;

	public TextStyleSetting styleTooltipHeader;

	public TextStyleSetting styleTooltipBody;

	public ColorStyleSetting emptyQueueColorStyle;

	public ColorStyleSetting standardQueueColorStyle;

	private ComplexFabricator targetFab;

	private string selectedRecipeCategory;

	private Dictionary<GameObject, List<ComplexRecipe>> recipeCategoryToggleMap;

	private Dictionary<string, GameObject> recipeCategories = new Dictionary<string, GameObject>();

	private List<GameObject> recipeToggles = new List<GameObject>();

	public SelectedRecipeQueueScreen recipeScreenPrefab;

	private SelectedRecipeQueueScreen recipeScreen;

	private int targetOrdersUpdatedSubHandle = -1;

	public enum StyleSetting
	{
		GridResult,
		ListResult,
		GridInput,
		ListInput,
		ListInputOutput,
		GridInputOutput,
		ClassicFabricator,
		ListQueueHybrid
	}
}
