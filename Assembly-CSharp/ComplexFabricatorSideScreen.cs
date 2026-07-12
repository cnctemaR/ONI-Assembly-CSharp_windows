using System;
using System.Collections.Generic;
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
		return target.GetComponent<ComplexFabricator>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		ComplexFabricator component = target.GetComponent<ComplexFabricator>();
		if (component == null)
		{
			global::Debug.LogError("The object selected doesn't have a ComplexFabricator!");
			return;
		}
		if (this.targetOrdersUpdatedSubHandle != -1)
		{
			base.Unsubscribe(this.targetOrdersUpdatedSubHandle);
		}
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
			GameObject gameObject = this.recipeToggles.Find((GameObject match) => this.recipeMap[match] == r);
			if (gameObject != null)
			{
				this.RefreshQueueCountDisplay(gameObject, this.targetFab);
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
			this.selectedRecipe = null;
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
		this.recipeMap = new Dictionary<GameObject, ComplexRecipe>();
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
		for (int i = 0; i < recipes.Length; i++)
		{
			ComplexRecipe recipe = recipes[i];
			bool flag = false;
			if (DebugHandler.InstantBuildMode)
			{
				flag = true;
			}
			else if (recipe.RequiresTechUnlock())
			{
				if (recipe.IsRequiredTechUnlocked())
				{
					flag = true;
				}
			}
			else if (target.GetRecipeQueueCount(recipe) != 0)
			{
				flag = true;
			}
			else if (this.AnyRecipeRequirementsDiscovered(recipe))
			{
				flag = true;
			}
			else if (this.HasAnyRecipeRequirements(recipe))
			{
				flag = true;
			}
			if (flag)
			{
				num++;
				global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(recipe.ingredients[0].material, "ui", false);
				global::Tuple<Sprite, Color> uisprite2 = Def.GetUISprite(recipe.results[0].material, recipe.results[0].facadeID);
				KToggle newToggle = null;
				ComplexFabricatorSideScreen.StyleSetting sideScreenStyle = target.sideScreenStyle;
				GameObject entryGO;
				if (sideScreenStyle - ComplexFabricatorSideScreen.StyleSetting.ListInputOutput > 1)
				{
					if (sideScreenStyle != ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid)
					{
						newToggle = global::Util.KInstantiateUI<KToggle>(this.recipeButton, this.recipeGrid, false);
						entryGO = newToggle.gameObject;
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
					else
					{
						newToggle = global::Util.KInstantiateUI<KToggle>(this.recipeButtonQueueHybrid, this.recipeGrid, false);
						entryGO = newToggle.gameObject;
						this.recipeMap.Add(entryGO, recipe);
						if (recipe.recipeCategoryID != "")
						{
							if (!this.recipeCategories.ContainsKey(recipe.recipeCategoryID))
							{
								GameObject gameObject = global::Util.KInstantiateUI(this.recipeCategoryHeader, this.recipeGrid, true);
								gameObject.GetComponentInChildren<LocText>().SetText(Strings.Get("STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_CATEGORIES." + recipe.recipeCategoryID.ToUpper()).String);
								HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
								RectTransform categoryContent = component.GetReference<RectTransform>("content");
								component.GetReference<Image>("icon").sprite = recipe.GetUIIcon();
								categoryContent.gameObject.SetActive(false);
								MultiToggle toggle = gameObject.GetComponentInChildren<MultiToggle>();
								MultiToggle toggle2 = toggle;
								toggle2.onClick = (global::System.Action)Delegate.Combine(toggle2.onClick, new global::System.Action(delegate
								{
									categoryContent.gameObject.SetActive(!categoryContent.gameObject.activeSelf);
									toggle.ChangeState(categoryContent.gameObject.activeSelf ? 1 : 0);
								}));
								this.recipeCategories.Add(recipe.recipeCategoryID, categoryContent.gameObject);
							}
							newToggle.transform.SetParent(this.recipeCategories[recipe.recipeCategoryID].rectTransform());
						}
						Image image = entryGO.GetComponentsInChildrenOnly<Image>()[2];
						if (recipe.nameDisplay == ComplexRecipe.RecipeNameDisplay.Ingredient)
						{
							image.sprite = uisprite.first;
							image.color = uisprite.second;
						}
						else if (recipe.nameDisplay == ComplexRecipe.RecipeNameDisplay.HEP)
						{
							image.sprite = this.radboltSprite;
						}
						else
						{
							image.sprite = uisprite2.first;
							image.color = uisprite2.second;
						}
						entryGO.GetComponentInChildren<LocText>().text = recipe.GetUIName(false);
						bool flag2 = this.HasAllRecipeRequirements(recipe);
						image.material = (flag2 ? Assets.UIPrefabs.TableScreenWidgets.DefaultUIMaterial : Assets.UIPrefabs.TableScreenWidgets.DesaturatedUIMaterial);
						this.RefreshQueueCountDisplay(entryGO, this.targetFab);
						entryGO.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("DecrementButton").onClick = delegate
						{
							target.DecrementRecipeQueueCount(recipe, false);
							this.RefreshQueueCountDisplay(entryGO, target);
						};
						entryGO.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("IncrementButton").onClick = delegate
						{
							target.IncrementRecipeQueueCount(recipe);
							this.RefreshQueueCountDisplay(entryGO, target);
						};
						entryGO.gameObject.SetActive(true);
					}
				}
				else
				{
					newToggle = global::Util.KInstantiateUI<KToggle>(this.recipeButtonMultiple, this.recipeGrid, false);
					entryGO = newToggle.gameObject;
					HierarchyReferences component2 = newToggle.GetComponent<HierarchyReferences>();
					foreach (ComplexRecipe.RecipeElement recipeElement in recipe.ingredients)
					{
						GameObject gameObject2 = global::Util.KInstantiateUI(component2.GetReference("FromIconPrefab").gameObject, component2.GetReference("FromIcons").gameObject, true);
						gameObject2.GetComponent<Image>().sprite = Def.GetUISprite(recipeElement.material, "ui", false).first;
						gameObject2.GetComponent<Image>().color = Def.GetUISprite(recipeElement.material, "ui", false).second;
						gameObject2.gameObject.name = recipeElement.material.Name;
					}
					foreach (ComplexRecipe.RecipeElement recipeElement2 in recipe.results)
					{
						GameObject gameObject3 = global::Util.KInstantiateUI(component2.GetReference("ToIconPrefab").gameObject, component2.GetReference("ToIcons").gameObject, true);
						gameObject3.GetComponent<Image>().sprite = Def.GetUISprite(recipeElement2.material, "ui", false).first;
						gameObject3.GetComponent<Image>().color = Def.GetUISprite(recipeElement2.material, "ui", false).second;
						gameObject3.gameObject.name = recipeElement2.material.Name;
					}
				}
				if (this.targetFab.sideScreenStyle == ComplexFabricatorSideScreen.StyleSetting.ClassicFabricator)
				{
					newToggle.GetComponentInChildren<LocText>().text = recipe.results[0].material.ProperName();
				}
				else if (this.targetFab.sideScreenStyle != ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid)
				{
					newToggle.GetComponentInChildren<LocText>().text = string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_WITH_NEWLINES, recipe.ingredients[0].material.ProperName(), recipe.results[0].material.ProperName());
				}
				ToolTip component3 = entryGO.GetComponent<ToolTip>();
				component3.toolTipPosition = ToolTip.TooltipPosition.Custom;
				component3.parentPositionAnchor = new Vector2(0f, 0.5f);
				component3.tooltipPivot = new Vector2(1f, 1f);
				component3.tooltipPositionOffset = new Vector2(-24f, 20f);
				component3.ClearMultiStringTooltip();
				component3.AddMultiStringTooltip(recipe.GetUIName(false), this.styleTooltipHeader);
				component3.AddMultiStringTooltip(recipe.description, this.styleTooltipBody);
				newToggle.onClick += delegate
				{
					this.ToggleClicked(newToggle);
				};
				entryGO.SetActive(true);
				this.recipeToggles.Add(entryGO);
			}
		}
		if (this.recipeToggles.Count > 0)
		{
			this.buttonScrollContainer.GetComponent<LayoutElement>().minHeight = Mathf.Min(451f, 2f + (float)num * this.recipeButtonQueueHybrid.GetComponent<LayoutElement>().minHeight);
			this.subtitleLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.SUBTITLE);
			this.noRecipesDiscoveredLabel.gameObject.SetActive(false);
		}
		else
		{
			this.subtitleLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPEDISCOVERED);
			this.noRecipesDiscoveredLabel.SetText(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPEDISCOVERED_BODY);
			this.noRecipesDiscoveredLabel.gameObject.SetActive(true);
			this.buttonScrollContainer.GetComponent<LayoutElement>().minHeight = this.noRecipesDiscoveredLabel.rectTransform.sizeDelta.y + 10f;
		}
		this.RefreshIngredientAvailabilityVis();
	}

	public void RefreshQueueCountDisplayForRecipe(ComplexRecipe recipe, ComplexFabricator fabricator)
	{
		GameObject gameObject = this.recipeToggles.Find((GameObject match) => this.recipeMap[match] == recipe);
		if (gameObject != null)
		{
			this.RefreshQueueCountDisplay(gameObject, fabricator);
		}
	}

	private void RefreshQueueCountDisplay(GameObject entryGO, ComplexFabricator fabricator)
	{
		HierarchyReferences component = entryGO.GetComponent<HierarchyReferences>();
		bool flag = fabricator.GetRecipeQueueCount(this.recipeMap[entryGO]) == ComplexFabricator.QUEUE_INFINITE;
		component.GetReference<LocText>("CountLabel").text = (flag ? "" : fabricator.GetRecipeQueueCount(this.recipeMap[entryGO]).ToString());
		component.GetReference<RectTransform>("InfiniteIcon").gameObject.SetActive(flag);
	}

	private void ToggleClicked(KToggle toggle)
	{
		if (!this.recipeMap.ContainsKey(toggle.gameObject))
		{
			global::Debug.LogError("Recipe not found on recipe list.");
			return;
		}
		if (this.selectedToggle == toggle)
		{
			this.selectedToggle.isOn = false;
			this.selectedToggle = null;
			this.selectedRecipe = null;
		}
		else
		{
			this.selectedToggle = toggle;
			this.selectedToggle.isOn = true;
			this.selectedRecipe = this.recipeMap[toggle.gameObject];
			this.selectedRecipeFabricatorMap[this.targetFab] = this.recipeToggles.IndexOf(toggle.gameObject);
		}
		this.RefreshIngredientAvailabilityVis();
		if (toggle.isOn)
		{
			this.recipeScreen = (SelectedRecipeQueueScreen)DetailsScreen.Instance.SetSecondarySideScreen(this.recipeScreenPrefab, UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_DETAILS);
			this.recipeScreen.SetRecipe(this, this.targetFab, this.selectedRecipe);
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

	private void Update()
	{
		this.RefreshIngredientAvailabilityVis();
	}

	private void RefreshIngredientAvailabilityVis()
	{
		foreach (KeyValuePair<GameObject, ComplexRecipe> keyValuePair in this.recipeMap)
		{
			HierarchyReferences component = keyValuePair.Key.GetComponent<HierarchyReferences>();
			bool flag = this.HasAllRecipeRequirements(keyValuePair.Value);
			KToggle component2 = keyValuePair.Key.GetComponent<KToggle>();
			if (flag)
			{
				if (this.selectedRecipe == keyValuePair.Value)
				{
					component2.ActivateFlourish(true, ImageToggleState.State.Active);
				}
				else
				{
					component2.ActivateFlourish(false, ImageToggleState.State.Inactive);
				}
			}
			else if (this.selectedRecipe == keyValuePair.Value)
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

	private Element[] GetRecipeElements(Recipe recipe)
	{
		Element[] array = new Element[recipe.Ingredients.Count];
		for (int i = 0; i < recipe.Ingredients.Count; i++)
		{
			Tag tag = recipe.Ingredients[i].tag;
			foreach (Element element in ElementLoader.elements)
			{
				if (GameTagExtensions.Create(element.id) == tag)
				{
					array[i] = element;
					break;
				}
			}
		}
		return array;
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
	private LocText noRecipesDiscoveredLabel;

	public TextStyleSetting styleTooltipHeader;

	public TextStyleSetting styleTooltipBody;

	private ComplexFabricator targetFab;

	private ComplexRecipe selectedRecipe;

	private Dictionary<GameObject, ComplexRecipe> recipeMap;

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
