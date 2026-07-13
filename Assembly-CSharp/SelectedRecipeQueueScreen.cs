using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SelectedRecipeQueueScreen : KScreen
{
	private ComplexRecipe selectedRecipe
	{
		get
		{
			return this.CalculateSelectedRecipe();
		}
	}

	private List<ComplexRecipe> selectedRecipes
	{
		get
		{
			return this.target.GetRecipesWithCategoryID(this.selectedRecipeCategoryID);
		}
	}

	private ComplexRecipe firstSelectedRecipe
	{
		get
		{
			return this.selectedRecipes[0];
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.DecrementButton.onClick = delegate
		{
			if (this.selectedRecipe == null)
			{
				return;
			}
			this.target.DecrementRecipeQueueCount(this.selectedRecipe, false);
			this.RefreshIngredientDescriptors();
			this.RefreshQueueCountDisplay();
			this.ownerScreen.RefreshQueueCountDisplayForRecipeCategory(this.selectedRecipeCategoryID, this.target);
		};
		this.IncrementButton.onClick = delegate
		{
			if (this.selectedRecipe == null)
			{
				return;
			}
			this.target.IncrementRecipeQueueCount(this.selectedRecipe);
			this.RefreshIngredientDescriptors();
			this.RefreshQueueCountDisplay();
			this.ownerScreen.RefreshQueueCountDisplayForRecipeCategory(this.selectedRecipeCategoryID, this.target);
		};
		this.InfiniteButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_FOREVER;
		this.InfiniteButton.onClick += delegate
		{
			if (this.selectedRecipe == null)
			{
				return;
			}
			if (this.target.GetRecipeQueueCount(this.selectedRecipe) != ComplexFabricator.QUEUE_INFINITE)
			{
				this.target.SetRecipeQueueCount(this.selectedRecipe, ComplexFabricator.QUEUE_INFINITE);
			}
			else
			{
				this.target.SetRecipeQueueCount(this.selectedRecipe, 0);
			}
			this.RefreshQueueCountDisplay();
			this.ownerScreen.RefreshQueueCountDisplayForRecipeCategory(this.selectedRecipeCategoryID, this.target);
		};
		this.QueueCount.onEndEdit += delegate
		{
			base.isEditing = false;
			if (this.selectedRecipe == null)
			{
				return;
			}
			this.target.SetRecipeQueueCount(this.selectedRecipe, Mathf.RoundToInt(this.QueueCount.currentValue));
			this.RefreshIngredientDescriptors();
			this.RefreshQueueCountDisplay();
			this.ownerScreen.RefreshQueueCountDisplayForRecipeCategory(this.selectedRecipeCategoryID, this.target);
		};
		this.QueueCount.onStartEdit += delegate
		{
			base.isEditing = true;
			KScreenManager.Instance.RefreshStack();
		};
		MultiToggle multiToggle = this.previousRecipeButton;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(this.CyclePreviousRecipe));
		MultiToggle multiToggle2 = this.nextRecipeButton;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(this.CycleNextRecipe));
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (this.firstSelectedRecipe != null)
		{
			GameObject prefab = Assets.GetPrefab(this.firstSelectedRecipe.results[0].material);
			Equippable equippable = ((prefab != null) ? prefab.GetComponent<Equippable>() : null);
			if (equippable != null && equippable.GetBuildOverride() != null)
			{
				this.minionWidget.RemoveEquipment(equippable);
			}
		}
	}

	private void AutoSelectBestRecipeInCategory()
	{
		int num = -1;
		List<ComplexRecipe> list = new List<ComplexRecipe>();
		this.selectedMaterialOption.Clear();
		ComplexRecipe complexRecipe = null;
		if (this.target.mostRecentRecipeSelectionByCategory.ContainsKey(this.selectedRecipeCategoryID))
		{
			complexRecipe = this.target.GetRecipe(this.target.mostRecentRecipeSelectionByCategory[this.selectedRecipeCategoryID]);
		}
		if (complexRecipe != null)
		{
			foreach (ComplexRecipe.RecipeElement recipeElement in complexRecipe.ingredients)
			{
				this.selectedMaterialOption.Add(recipeElement.material);
			}
		}
		else
		{
			foreach (ComplexRecipe complexRecipe2 in this.selectedRecipes)
			{
				int num2 = this.target.GetRecipeQueueCount(complexRecipe2);
				if (num2 == ComplexFabricator.QUEUE_INFINITE)
				{
					num2 = int.MaxValue;
				}
				if (num2 >= num)
				{
					if (num2 > num)
					{
						list.Clear();
						num = num2;
					}
					list.Add(complexRecipe2);
				}
			}
			int num3 = list[0].ingredients.Length;
			Tag[] array = new Tag[num3];
			for (int j = 0; j < num3; j++)
			{
				float num4 = -1f;
				foreach (ComplexRecipe complexRecipe3 in list)
				{
					float amount = this.target.GetMyWorld().worldInventory.GetAmount(complexRecipe3.ingredients[j].material, true);
					if (amount > num4)
					{
						array[j] = complexRecipe3.ingredients[j].material;
						num4 = amount;
					}
				}
			}
			this.selectedMaterialOption.AddRange(array);
		}
		this.RefreshIngredientDescriptors();
		this.RefreshQueueCountDisplay();
	}

	public bool IsSelectedMaterials(ComplexRecipe recipe)
	{
		if (this.selectedRecipeCategoryID != recipe.recipeCategoryID)
		{
			return false;
		}
		for (int i = 0; i < recipe.ingredients.Length; i++)
		{
			if (recipe.ingredients[i].material != this.selectedMaterialOption[i])
			{
				return false;
			}
		}
		return true;
	}

	public void SelectNextQueuedRecipeInCategory()
	{
		this.cycleRecipeVariantIdx++;
		this.selectedMaterialOption.Clear();
		List<ComplexRecipe> list = this.selectedRecipes.Where<ComplexRecipe>((ComplexRecipe match) => this.target.IsRecipeQueued(match)).ToList<ComplexRecipe>();
		if (list.Count == 0)
		{
			this.AutoSelectBestRecipeInCategory();
			return;
		}
		ComplexRecipe complexRecipe = list[this.cycleRecipeVariantIdx % list.Count];
		for (int i = 0; i < complexRecipe.ingredients.Length; i++)
		{
			this.selectedMaterialOption.Add(complexRecipe.ingredients[i].material);
		}
		this.RefreshIngredientDescriptors();
		this.RefreshQueueCountDisplay();
	}

	public void SetRecipeCategory(ComplexFabricatorSideScreen owner, ComplexFabricator target, string recipeCategoryID)
	{
		this.ownerScreen = owner;
		this.target = target;
		this.selectedRecipeCategoryID = recipeCategoryID;
		this.AutoSelectBestRecipeInCategory();
		this.recipeName.text = this.firstSelectedRecipe.GetUIName(false);
		global::Tuple<Sprite, Color> tuple;
		if (this.firstSelectedRecipe.nameDisplay == ComplexRecipe.RecipeNameDisplay.Ingredient)
		{
			tuple = Def.GetUISprite(this.firstSelectedRecipe.ingredients[0].material, "ui", false);
		}
		else if (this.firstSelectedRecipe.nameDisplay == ComplexRecipe.RecipeNameDisplay.Custom && !string.IsNullOrEmpty(this.firstSelectedRecipe.customSpritePrefabID))
		{
			tuple = Def.GetUISprite(this.firstSelectedRecipe.customSpritePrefabID, "ui", false);
		}
		else
		{
			tuple = Def.GetUISprite(this.firstSelectedRecipe.results[0].material, this.firstSelectedRecipe.results[0].facadeID);
		}
		if (this.firstSelectedRecipe.nameDisplay == ComplexRecipe.RecipeNameDisplay.HEP)
		{
			this.recipeIcon.sprite = owner.radboltSprite;
			this.recipeIcon.sprite = owner.radboltSprite;
		}
		else
		{
			this.recipeIcon.sprite = tuple.first;
			this.recipeIcon.color = tuple.second;
		}
		string text = (this.firstSelectedRecipe.time.ToString() + " " + UI.UNITSUFFIXES.SECONDS).ToLower();
		this.recipeMainDescription.SetText(this.firstSelectedRecipe.description);
		this.recipeDuration.SetText(text);
		string text2 = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPE_WORKTIME, text);
		this.recipeDurationTooltip.SetSimpleTooltip(text2);
		this.cycleRecipeVariantIdx = 0;
		this.RefreshIngredientDescriptors();
		this.RefreshResultDescriptors();
		this.RefreshSizeScrollContainerSize();
		this.RefreshQueueCountDisplay();
		this.ToggleAndRefreshMinionDisplay();
	}

	private void RefreshSizeScrollContainerSize()
	{
		float num = 16f;
		float num2 = 0f;
		num2 += (float)(this.materialSelectionRowsByContainer.Count * 32);
		foreach (KeyValuePair<GameObject, List<GameObject>> keyValuePair in this.materialSelectionRowsByContainer)
		{
			num2 += (float)(Mathf.Max(1, keyValuePair.Value.Count) * 48);
		}
		num2 += (float)((this.materialSelectionRowsByContainer.Count - 1) * 12);
		float num3 = (float)Mathf.Max(this.selectedRecipes[0].results.Length * 32 + (this.recipeEffectsDescriptorRows.Count - this.selectedRecipes[0].results.Length) * 16, 40);
		num3 += 46f;
		float num4 = num + num2 + num3;
		this.scrollContainer.minHeight = Mathf.Min((float)(Screen.height - 448), num4);
	}

	private void CyclePreviousRecipe()
	{
		this.ownerScreen.CycleRecipe(-1);
	}

	private void CycleNextRecipe()
	{
		this.ownerScreen.CycleRecipe(1);
	}

	private void ToggleAndRefreshMinionDisplay()
	{
		this.minionWidget.gameObject.SetActive(this.RefreshMinionDisplayAnim());
	}

	private bool RefreshMinionDisplayAnim()
	{
		GameObject prefab = Assets.GetPrefab(this.firstSelectedRecipe.results[0].material);
		if (prefab == null)
		{
			return false;
		}
		Equippable component = prefab.GetComponent<Equippable>();
		if (component == null)
		{
			return false;
		}
		KAnimFile buildOverride = component.GetBuildOverride();
		if (buildOverride == null)
		{
			return false;
		}
		this.minionWidget.SetDefaultPortraitAnimator();
		KAnimFile kanimFile = buildOverride;
		if (!this.firstSelectedRecipe.results[0].facadeID.IsNullOrWhiteSpace())
		{
			EquippableFacadeResource equippableFacadeResource = Db.GetEquippableFacades().TryGet(this.firstSelectedRecipe.results[0].facadeID);
			if (equippableFacadeResource != null)
			{
				kanimFile = Assets.GetAnim(equippableFacadeResource.BuildOverride);
			}
		}
		this.minionWidget.UpdateEquipment(component, kanimFile);
		return true;
	}

	private ComplexRecipe CalculateSelectedRecipe()
	{
		foreach (ComplexRecipe complexRecipe in this.target.GetRecipesWithCategoryID(this.selectedRecipeCategoryID))
		{
			bool flag = true;
			for (int i = 0; i < this.selectedMaterialOption.Count; i++)
			{
				if (complexRecipe.ingredients[i].material != this.selectedMaterialOption[i])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return complexRecipe;
			}
		}
		return null;
	}

	private void RefreshQueueCountDisplay()
	{
		this.ResearchRequiredContainer.SetActive(!this.selectedRecipes[0].IsRequiredTechUnlocked());
		if (this.selectedRecipe == null)
		{
			return;
		}
		bool flag = true;
		foreach (Tag tag in this.selectedMaterialOption)
		{
			if (!DiscoveredResources.Instance.IsDiscovered(tag))
			{
				flag = DebugHandler.InstantBuildMode;
			}
		}
		this.UndiscoveredMaterialsContainer.SetActive(!flag);
		int recipeQueueCount = this.target.GetRecipeQueueCount(this.selectedRecipe);
		bool flag2 = recipeQueueCount == ComplexFabricator.QUEUE_INFINITE;
		if (!flag2)
		{
			this.QueueCount.SetAmount((float)recipeQueueCount);
		}
		else
		{
			this.QueueCount.SetDisplayValue("");
		}
		this.InfiniteIcon.gameObject.SetActive(flag2);
	}

	private void RefreshResultDescriptors()
	{
		List<SelectedRecipeQueueScreen.DescriptorWithSprite> list = new List<SelectedRecipeQueueScreen.DescriptorWithSprite>();
		list.AddRange(this.GetResultDescriptions(this.selectedRecipes[0]));
		foreach (Descriptor descriptor in this.target.AdditionalEffectsForRecipe(this.selectedRecipes[0]))
		{
			list.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(descriptor, null, false));
		}
		if (list.Count > 0)
		{
			this.EffectsDescriptorPanel.gameObject.SetActive(true);
			foreach (KeyValuePair<SelectedRecipeQueueScreen.DescriptorWithSprite, GameObject> keyValuePair in this.recipeEffectsDescriptorRows)
			{
				Util.KDestroyGameObject(keyValuePair.Value);
			}
			this.recipeEffectsDescriptorRows.Clear();
			bool flag = true;
			foreach (SelectedRecipeQueueScreen.DescriptorWithSprite descriptorWithSprite in list)
			{
				GameObject gameObject = Util.KInstantiateUI(this.recipeElementDescriptorPrefab, this.EffectsDescriptorPanel.gameObject, true);
				HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
				Image reference = component.GetReference<Image>("Icon");
				bool flag2 = descriptorWithSprite.tintedSprite != null && descriptorWithSprite.tintedSprite.first != null;
				reference.sprite = ((descriptorWithSprite.tintedSprite == null) ? null : descriptorWithSprite.tintedSprite.first);
				reference.gameObject.SetActive(true);
				if (!flag2)
				{
					reference.color = Color.clear;
					if (flag)
					{
						gameObject.GetComponent<VerticalLayoutGroup>().padding.top = -8;
						flag = false;
					}
				}
				else
				{
					reference.color = ((descriptorWithSprite.tintedSprite == null) ? Color.white : descriptorWithSprite.tintedSprite.second);
					flag = true;
				}
				reference.gameObject.GetComponent<LayoutElement>().minWidth = (float)(flag2 ? 32 : 40);
				reference.gameObject.GetComponent<LayoutElement>().minHeight = (float)(flag2 ? 32 : 0);
				reference.gameObject.GetComponent<LayoutElement>().preferredHeight = (float)(flag2 ? 32 : 0);
				component.GetReference<LocText>("Label").SetText(flag2 ? descriptorWithSprite.descriptor.IndentedText() : descriptorWithSprite.descriptor.text);
				component.GetReference<RectTransform>("FilterControls").gameObject.SetActive(false);
				component.GetReference<ToolTip>("Tooltip").SetSimpleTooltip(descriptorWithSprite.descriptor.tooltipText);
				this.recipeEffectsDescriptorRows.Add(descriptorWithSprite, gameObject);
			}
		}
	}

	private List<SelectedRecipeQueueScreen.DescriptorWithSprite> GetResultDescriptions(ComplexRecipe recipe)
	{
		List<SelectedRecipeQueueScreen.DescriptorWithSprite> list = new List<SelectedRecipeQueueScreen.DescriptorWithSprite>();
		if (recipe.producedHEP > 0)
		{
			list.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(new Descriptor(string.Format("<b>{0}</b>: {1}", UI.FormatAsLink(ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME, "HEP"), recipe.producedHEP), string.Format("<b>{0}</b>: {1}", ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME, recipe.producedHEP), Descriptor.DescriptorType.Requirement, false), new global::Tuple<Sprite, Color>(Assets.GetSprite("radbolt"), Color.white), false));
		}
		foreach (ComplexRecipe.RecipeElement recipeElement in recipe.results)
		{
			GameObject prefab = Assets.GetPrefab(recipeElement.material);
			string formattedByTag = GameUtil.GetFormattedByTag(recipeElement.material, recipeElement.amount, GameUtil.TimeSlice.None);
			list.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(new Descriptor(string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPEPRODUCT, recipeElement.facadeID.IsNullOrWhiteSpace() ? recipeElement.material.ProperName() : recipeElement.facadeID.ProperName(), formattedByTag), string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPEPRODUCT, recipeElement.facadeID.IsNullOrWhiteSpace() ? recipeElement.material.ProperName() : recipeElement.facadeID.ProperName(), formattedByTag), Descriptor.DescriptorType.Requirement, false), Def.GetUISprite(recipeElement.material, recipeElement.facadeID), false));
			Element element = ElementLoader.GetElement(recipeElement.material);
			if (element != null)
			{
				List<SelectedRecipeQueueScreen.DescriptorWithSprite> list2 = new List<SelectedRecipeQueueScreen.DescriptorWithSprite>();
				foreach (Descriptor descriptor in GameUtil.GetMaterialDescriptors(element))
				{
					list2.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(descriptor, null, false));
				}
				foreach (SelectedRecipeQueueScreen.DescriptorWithSprite descriptorWithSprite in list2)
				{
					descriptorWithSprite.descriptor.IncreaseIndent();
				}
				list.AddRange(list2);
			}
			else
			{
				List<SelectedRecipeQueueScreen.DescriptorWithSprite> list3 = new List<SelectedRecipeQueueScreen.DescriptorWithSprite>();
				foreach (Descriptor descriptor2 in GameUtil.GetEffectDescriptors(GameUtil.GetAllDescriptors(prefab, false)))
				{
					list3.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(descriptor2, null, false));
				}
				foreach (SelectedRecipeQueueScreen.DescriptorWithSprite descriptorWithSprite2 in list3)
				{
					descriptorWithSprite2.descriptor.IncreaseIndent();
				}
				list.AddRange(list3);
			}
		}
		return list;
	}

	private void RefreshIngredientDescriptors()
	{
		new List<SelectedRecipeQueueScreen.DescriptorWithSprite>();
		this.IngredientsDescriptorPanel.gameObject.SetActive(true);
		this.materialSelectionContainers.ForEach(delegate(GameObject container)
		{
			Util.KDestroyGameObject(container);
		});
		this.materialSelectionContainers.Clear();
		this.materialSelectionRowsByContainer.Clear();
		for (int i = 0; i < this.selectedRecipes[0].ingredients.Length; i++)
		{
			GameObject gameObject = Util.KInstantiateUI(this.materialSelectionContainerPrefab, this.IngredientsDescriptorPanel.gameObject, true);
			this.materialSelectionContainers.Add(gameObject);
			this.materialSelectionRowsByContainer.Add(this.materialSelectionContainers[i], new List<GameObject>());
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			int idx = i;
			List<Tag> list = new List<Tag>();
			bool flag = false;
			HashSet<Tag> hashSet = new HashSet<Tag>();
			for (int j = 0; j < this.selectedRecipes.Count; j++)
			{
				Tag newTag = this.selectedRecipes[j].ingredients[idx].material;
				if (!list.Contains(newTag))
				{
					bool flag2 = DiscoveredResources.Instance.IsDiscovered(newTag);
					if (!flag2)
					{
						hashSet.Add(newTag);
					}
					if (flag2 || DebugHandler.InstantBuildMode)
					{
						flag = true;
						GameObject gameObject2 = Util.KInstantiateUI(this.materialFilterRowPrefab, this.materialSelectionContainers[idx].gameObject, true);
						this.materialSelectionRowsByContainer[this.materialSelectionContainers[idx]].Add(gameObject2);
						list.Add(newTag);
						LocText reference = gameObject2.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
						bool flag3 = false;
						string ingredientDescription = this.GetIngredientDescription(this.selectedRecipes[j].ingredients[idx], out flag3);
						bool flag4 = this.selectedMaterialOption[i] == this.selectedRecipes[j].ingredients[i].material;
						if (flag4)
						{
							component.GetReference<Image>("HeaderBG").color = (flag3 ? Util.ColorFromHex("D9DAE3") : Util.ColorFromHex("E3DAD9"));
						}
						reference.color = (flag3 ? Color.black : new Color(0.2f, 0.2f, 0.2f, 1f));
						HierarchyReferences component2 = gameObject2.GetComponent<HierarchyReferences>();
						component2.GetReference<RectTransform>("SelectionHover").gameObject.SetActive(flag4);
						component2.GetReference<RectTransform>("SelectionHover").GetComponent<Image>().color = (flag3 ? Util.ColorFromHex("F0F6FC") : Util.ColorFromHex("FBE9EB"));
						component2.GetReference<LocText>("OrderCountLabel").SetText(this.target.GetIngredientQueueCount(this.selectedRecipeCategoryID, newTag).ToString());
						Image reference2 = component2.GetReference<Image>("Icon");
						reference2.material = ((!flag3) ? GlobalResources.Instance().AnimMaterialUIDesaturated : GlobalResources.Instance().AnimUIMaterial);
						reference2.color = (flag3 ? Color.white : new Color(1f, 1f, 1f, 0.55f));
						reference.SetText(ingredientDescription);
						reference2.sprite = Def.GetUISprite(newTag, "").first;
						MultiToggle component3 = gameObject2.GetComponent<MultiToggle>();
						component3.ChangeState(flag4 ? 1 : 0);
						component3.onClick = (global::System.Action)Delegate.Combine(component3.onClick, new global::System.Action(delegate
						{
							Tag newTag2 = newTag;
							this.selectedMaterialOption[idx] = newTag2;
							this.RefreshIngredientDescriptors();
							this.RefreshQueueCountDisplay();
							this.ownerScreen.RefreshQueueCountDisplayForRecipeCategory(this.selectedRecipeCategoryID, this.target);
						}));
					}
				}
			}
			ToolTip reference3 = component.GetReference<ToolTip>("HeaderTooltip");
			string text = UI.UISIDESCREENS.FABRICATORSIDESCREEN.UNDISCOVERED_INGREDIENTS_IN_CATEGORY;
			object[] array = new object[1];
			array[0] = "    • " + string.Join("\n    • ", hashSet.Select<Tag, string>((Tag t) => t.ProperName()).ToArray<string>());
			string text2 = GameUtil.SafeStringFormat(text, array);
			reference3.SetSimpleTooltip((hashSet.Count == 0) ? UI.UISIDESCREENS.FABRICATORSIDESCREEN.ALL_INGREDIENTS_IN_CATEGORY_DISOVERED : text2);
			RectTransform reference4 = component.GetReference<RectTransform>("NoDiscoveredRow");
			reference4.gameObject.SetActive(!flag);
			if (!flag)
			{
				reference4.GetComponent<ToolTip>().SetSimpleTooltip(text2);
			}
			string text3 = GameUtil.SafeStringFormat(UI.UISIDESCREENS.FABRICATORSIDESCREEN.INGREDIENT_CATEGORY, new object[] { i + 1 });
			if (!flag)
			{
				component.GetReference<Image>("HeaderBG").color = Util.ColorFromHex("E3DAD9");
			}
			if (hashSet.Count > 0)
			{
				text3 = string.Concat(new string[]
				{
					text3,
					" <color=#bf5858>(",
					list.Count.ToString(),
					"/",
					(list.Count + hashSet.Count).ToString(),
					")",
					UIConstants.ColorSuffix
				});
			}
			component.GetReference<LocText>("HeaderLabel").SetText(text3);
		}
		if (!this.target.mostRecentRecipeSelectionByCategory.ContainsKey(this.selectedRecipeCategoryID))
		{
			this.target.mostRecentRecipeSelectionByCategory.Add(this.selectedRecipeCategoryID, null);
		}
		this.target.mostRecentRecipeSelectionByCategory[this.selectedRecipeCategoryID] = this.selectedRecipe.id;
	}

	private string GetIngredientDescription(ComplexRecipe.RecipeElement ingredient, out bool hasEnoughMaterial)
	{
		GameObject prefab = Assets.GetPrefab(ingredient.material);
		string formattedByTag = GameUtil.GetFormattedByTag(ingredient.material, ingredient.amount, GameUtil.TimeSlice.None);
		float amount = this.target.GetMyWorld().worldInventory.GetAmount(ingredient.material, true);
		string formattedByTag2 = GameUtil.GetFormattedByTag(ingredient.material, amount, GameUtil.TimeSlice.None);
		hasEnoughMaterial = amount >= ingredient.amount;
		string text = GameUtil.SafeStringFormat(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_REQUIREMENT, new object[]
		{
			prefab.GetProperName(),
			formattedByTag
		});
		text += "\n";
		if (hasEnoughMaterial)
		{
			text = text + "<size=12>" + GameUtil.SafeStringFormat(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_AVAILABLE, new object[] { formattedByTag2 }) + "</size>";
		}
		else
		{
			text = text + "<size=12><color=#E68280>" + GameUtil.SafeStringFormat(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_AVAILABLE, new object[] { formattedByTag2 }) + "</color></size>";
		}
		return text;
	}

	public Image recipeIcon;

	public LocText recipeName;

	public LocText recipeMainDescription;

	public LocText recipeDuration;

	public ToolTip recipeDurationTooltip;

	public GameObject IngredientsDescriptorPanel;

	public GameObject EffectsDescriptorPanel;

	public KNumberInputField QueueCount;

	public MultiToggle DecrementButton;

	public MultiToggle IncrementButton;

	public KButton InfiniteButton;

	public GameObject InfiniteIcon;

	public GameObject ResearchRequiredContainer;

	public GameObject UndiscoveredMaterialsContainer;

	[SerializeField]
	private GameObject materialFilterRowPrefab;

	[SerializeField]
	private GameObject materialSelectionContainerPrefab;

	private List<GameObject> materialSelectionContainers = new List<GameObject>();

	private Dictionary<GameObject, List<GameObject>> materialSelectionRowsByContainer = new Dictionary<GameObject, List<GameObject>>();

	private ComplexFabricator target;

	private ComplexFabricatorSideScreen ownerScreen;

	private List<Tag> selectedMaterialOption = new List<Tag>();

	private string selectedRecipeCategoryID;

	[SerializeField]
	private GameObject recipeElementDescriptorPrefab;

	private Dictionary<SelectedRecipeQueueScreen.DescriptorWithSprite, GameObject> recipeIngredientDescriptorRows = new Dictionary<SelectedRecipeQueueScreen.DescriptorWithSprite, GameObject>();

	private Dictionary<SelectedRecipeQueueScreen.DescriptorWithSprite, GameObject> recipeEffectsDescriptorRows = new Dictionary<SelectedRecipeQueueScreen.DescriptorWithSprite, GameObject>();

	[SerializeField]
	private FullBodyUIMinionWidget minionWidget;

	[SerializeField]
	private MultiToggle previousRecipeButton;

	[SerializeField]
	private MultiToggle nextRecipeButton;

	[SerializeField]
	private LayoutElement scrollContainer;

	private int cycleRecipeVariantIdx;

	private class DescriptorWithSprite
	{
		public Descriptor descriptor { get; }

		public global::Tuple<Sprite, Color> tintedSprite { get; }

		public DescriptorWithSprite(Descriptor desc, global::Tuple<Sprite, Color> sprite, bool filterRowVisible = false)
		{
			this.descriptor = desc;
			this.tintedSprite = sprite;
			this.showFilterRow = filterRowVisible;
		}

		public bool showFilterRow;
	}
}
