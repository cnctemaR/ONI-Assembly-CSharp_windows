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
			return Strings.Get(this.titleKey).ToString().Replace("{0}", string.Empty);
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
			global::Debug.LogError("The object selected doesn't have a ComplexFabricator!", null);
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
			ComplexFabricatorSideScreen $this = this;
			GameObject gameObject = this.recipeToggles.Find((GameObject match) => $this.recipeMap[match] == r);
			if (gameObject != null)
			{
				this.RefreshQueueCountDisplay(gameObject, this.targetFab);
			}
		}
		if (this.targetFab.CurrentMachineOrder != null)
		{
			this.currentOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.CURRENT_ORDER, this.targetFab.CurrentMachineOrder.parentOrder.recipe.GetUIName());
		}
		else
		{
			this.currentOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.CURRENT_ORDER, UI.UISIDESCREENS.FABRICATORSIDESCREEN.NO_WORKABLE_ORDER);
		}
		if (this.targetFab.GetMachineOrders().Count > 1)
		{
			this.nextOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NEXT_ORDER, this.targetFab.GetMachineOrders()[1].parentOrder.recipe.GetUIName());
		}
		else
		{
			this.nextOrderLabel.text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.NEXT_ORDER, UI.UISIDESCREENS.FABRICATORSIDESCREEN.NO_WORKABLE_ORDER);
		}
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
			global::Debug.LogError("ComplexFabricator provided was null.", null);
			return;
		}
		this.targetFab = target;
		base.gameObject.SetActive(true);
		ComplexRecipe[] recipes = this.targetFab.GetRecipes();
		Array.Sort<ComplexRecipe>(recipes, (ComplexRecipe a, ComplexRecipe b) => a.sortOrder - b.sortOrder);
		this.recipeMap = new Dictionary<GameObject, ComplexRecipe>();
		this.recipeToggles.ForEach(delegate(GameObject rbi)
		{
			global::UnityEngine.Object.Destroy(rbi.gameObject);
		});
		this.recipeToggles.Clear();
		GridLayoutGroup component = this.recipeGrid.GetComponent<GridLayoutGroup>();
		switch (this.targetFab.sideScreenStyle)
		{
		case ComplexFabricatorSideScreen.StyleSetting.ListResult:
		case ComplexFabricatorSideScreen.StyleSetting.ListInput:
		case ComplexFabricatorSideScreen.StyleSetting.ListInputOutput:
			component.constraintCount = 1;
			component.cellSize = new Vector2(262f, component.cellSize.y);
			goto IL_01B3;
		case ComplexFabricatorSideScreen.StyleSetting.ClassicFabricator:
			component.constraintCount = 128;
			component.cellSize = new Vector2(78f, 96f);
			this.buttonScrollContainer.minHeight = 100f;
			goto IL_01B3;
		case ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid:
			component.constraintCount = 1;
			component.cellSize = new Vector2(264f, 64f);
			this.buttonScrollContainer.minHeight = 66f;
			goto IL_01B3;
		}
		component.constraintCount = 3;
		component.cellSize = new Vector2(116f, component.cellSize.y);
		IL_01B3:
		int num = 0;
		ComplexRecipe[] array = recipes;
		for (int i = 0; i < array.Length; i++)
		{
			ComplexFabricatorSideScreen.<Initialize>c__AnonStorey2 <Initialize>c__AnonStorey2 = new ComplexFabricatorSideScreen.<Initialize>c__AnonStorey2();
			<Initialize>c__AnonStorey2.recipe = array[i];
			bool flag = false;
			if (DebugHandler.InstantBuildMode)
			{
				flag = true;
			}
			else if (<Initialize>c__AnonStorey2.recipe.RequiresTechUnlock() && <Initialize>c__AnonStorey2.recipe.IsRequiredTechUnlocked())
			{
				flag = true;
			}
			else if (this.HasAnyRecipeRequirements(<Initialize>c__AnonStorey2.recipe))
			{
				flag = true;
			}
			if (flag)
			{
				num++;
				Tuple<Sprite, Color> uisprite = Def.GetUISprite(<Initialize>c__AnonStorey2.recipe.ingredients[0].material, "ui", false);
				Tuple<Sprite, Color> uisprite2 = Def.GetUISprite(<Initialize>c__AnonStorey2.recipe.results[0].material, "ui", false);
				KToggle newToggle = null;
				GameObject entryGO;
				switch (target.sideScreenStyle)
				{
				case ComplexFabricatorSideScreen.StyleSetting.ListInputOutput:
				case ComplexFabricatorSideScreen.StyleSetting.GridInputOutput:
				{
					newToggle = global::Util.KInstantiateUI<KToggle>(this.recipeButtonMultiple, this.recipeGrid, false);
					entryGO = newToggle.gameObject;
					HierarchyReferences component2 = newToggle.GetComponent<HierarchyReferences>();
					foreach (ComplexRecipe.RecipeElement recipeElement in <Initialize>c__AnonStorey2.recipe.ingredients)
					{
						GameObject gameObject = global::Util.KInstantiateUI(component2.GetReference("FromIconPrefab").gameObject, component2.GetReference("FromIcons").gameObject, true);
						gameObject.GetComponent<Image>().sprite = Def.GetUISprite(recipeElement.material, "ui", false).first;
						gameObject.GetComponent<Image>().color = Def.GetUISprite(recipeElement.material, "ui", false).second;
						gameObject.gameObject.name = recipeElement.material.Name;
					}
					foreach (ComplexRecipe.RecipeElement recipeElement2 in <Initialize>c__AnonStorey2.recipe.results)
					{
						GameObject gameObject2 = global::Util.KInstantiateUI(component2.GetReference("ToIconPrefab").gameObject, component2.GetReference("ToIcons").gameObject, true);
						gameObject2.GetComponent<Image>().sprite = Def.GetUISprite(recipeElement2.material, "ui", false).first;
						gameObject2.GetComponent<Image>().color = Def.GetUISprite(recipeElement2.material, "ui", false).second;
						gameObject2.gameObject.name = recipeElement2.material.Name;
					}
					break;
				}
				case ComplexFabricatorSideScreen.StyleSetting.ClassicFabricator:
					goto IL_0608;
				case ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid:
				{
					newToggle = global::Util.KInstantiateUI<KToggle>(this.recipeButtonQueueHybrid, this.recipeGrid, false);
					entryGO = newToggle.gameObject;
					this.recipeMap.Add(entryGO, <Initialize>c__AnonStorey2.recipe);
					Image image = entryGO.GetComponentsInChildrenOnly<Image>()[2];
					if (!<Initialize>c__AnonStorey2.recipe.useResultAsDescription)
					{
						image.sprite = uisprite.first;
						image.color = uisprite.second;
					}
					else
					{
						image.sprite = uisprite2.first;
						image.color = uisprite2.second;
					}
					entryGO.GetComponentInChildren<LocText>().text = <Initialize>c__AnonStorey2.recipe.GetUIName();
					bool flag2 = this.HasAllRecipeRequirements(<Initialize>c__AnonStorey2.recipe);
					image.material = ((!flag2) ? Assets.UIPrefabs.TableScreenWidgets.DesaturatedUIMaterial : Assets.UIPrefabs.TableScreenWidgets.DefaultUIMaterial);
					this.RefreshQueueCountDisplay(entryGO, this.targetFab);
					entryGO.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("DecrementButton").onClick = delegate
					{
						target.DecrementRecipeQueueCount(<Initialize>c__AnonStorey2.recipe, false);
						this.RefreshQueueCountDisplay(entryGO, target);
					};
					entryGO.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("IncrementButton").onClick = delegate
					{
						target.IncrementRecipeQueueCount(<Initialize>c__AnonStorey2.recipe);
						this.RefreshQueueCountDisplay(entryGO, target);
					};
					entryGO.gameObject.SetActive(true);
					break;
				}
				default:
					goto IL_0608;
				}
				IL_06AB:
				if (this.targetFab.sideScreenStyle == ComplexFabricatorSideScreen.StyleSetting.ClassicFabricator)
				{
					newToggle.GetComponentInChildren<LocText>().text = <Initialize>c__AnonStorey2.recipe.results[0].material.ProperName();
				}
				else if (this.targetFab.sideScreenStyle != ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid)
				{
					newToggle.GetComponentInChildren<LocText>().text = string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_WITH_NEWLINES, <Initialize>c__AnonStorey2.recipe.ingredients[0].material.ProperName(), <Initialize>c__AnonStorey2.recipe.results[0].material.ProperName());
				}
				ToolTip component3 = entryGO.GetComponent<ToolTip>();
				component3.toolTipPosition = ToolTip.TooltipPosition.Custom;
				component3.parentPositionAnchor = new Vector2(0f, 0.5f);
				component3.tooltipPivot = new Vector2(1f, 1f);
				component3.tooltipPositionOffset = new Vector2(-24f, 20f);
				component3.ClearMultiStringTooltip();
				component3.AddMultiStringTooltip(<Initialize>c__AnonStorey2.recipe.GetUIName(), this.styleTooltipHeader);
				component3.AddMultiStringTooltip(<Initialize>c__AnonStorey2.recipe.description, this.styleTooltipBody);
				newToggle.onClick += delegate
				{
					this.ToggleClicked(newToggle);
				};
				entryGO.SetActive(true);
				this.recipeToggles.Add(entryGO);
				goto IL_0814;
				IL_0608:
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
				goto IL_06AB;
			}
			IL_0814:;
		}
		if (this.recipeToggles.Count > 0)
		{
			this.buttonScrollContainer.GetComponent<LayoutElement>().minHeight = Mathf.Min(451f, 2f + (float)num * this.recipeButtonQueueHybrid.rectTransform().sizeDelta.y);
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
		component.GetReference<LocText>("CountLabel").text = ((!flag) ? fabricator.GetRecipeQueueCount(this.recipeMap[entryGO]).ToString() : string.Empty);
		component.GetReference<RectTransform>("InfiniteIcon").gameObject.SetActive(flag);
	}

	private void ToggleClicked(KToggle toggle)
	{
		if (!this.recipeMap.ContainsKey(toggle.gameObject))
		{
			global::Debug.LogError("Recipe not found on recipe list.", null);
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
		}
		else
		{
			DetailsScreen.Instance.ClearSecondarySideScreen();
		}
	}

	private bool HasAnyRecipeRequirements(ComplexRecipe recipe)
	{
		foreach (ComplexRecipe.RecipeElement recipeElement in recipe.ingredients)
		{
			if (WorldInventory.Instance.GetAmount(recipeElement.material) + this.targetFab.inStorage.GetAmountAvailable(recipeElement.material) + this.targetFab.buildStorage.GetAmountAvailable(recipeElement.material) >= recipeElement.amount)
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
			if (WorldInventory.Instance.GetAmount(recipeElement.material) + this.targetFab.inStorage.GetAmountAvailable(recipeElement.material) + this.targetFab.buildStorage.GetAmountAvailable(recipeElement.material) < recipeElement.amount)
			{
				flag = false;
			}
		}
		return flag;
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
			component.GetReference<LocText>("Label").color = ((!flag) ? new Color(0.22f, 0.22f, 0.22f, 1f) : Color.black);
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
	private LocText currentOrderLabel;

	[SerializeField]
	private LocText nextOrderLabel;

	private Dictionary<ComplexFabricator, int> selectedRecipeFabricatorMap = new Dictionary<ComplexFabricator, int>();

	[EventRef]
	public string createOrderSound;

	[SerializeField]
	private RectTransform content;

	[SerializeField]
	private LocText subtitleLabel;

	[SerializeField]
	private LocText noRecipesDiscoveredLabel;

	public ScriptableObject styleTooltipHeader;

	public ScriptableObject styleTooltipBody;

	private ComplexFabricator targetFab;

	private ComplexRecipe selectedRecipe;

	private Dictionary<GameObject, ComplexRecipe> recipeMap;

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
