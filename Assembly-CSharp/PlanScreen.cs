using System;
using System.Collections.Generic;
using System.Linq;
using FileHelpers;
using FMOD.Studio;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class PlanScreen : KIconToggleMenu
{
	public static PlanScreen Instance { get; private set; }

	public override float GetSortKey()
	{
		return 2f;
	}

	public PlanScreen.RequirementsState BuildableState(BuildingDef def)
	{
		PlanScreen.RequirementsState requirementsState = PlanScreen.RequirementsState.Materials;
		if (!this.buildableDefs.TryGetValue(def, out requirementsState))
		{
		}
		return requirementsState;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(PlanScreen.FabricatorConfig));
		this.fabricatorConfigs = (PlanScreen.FabricatorConfig[])fileHelperEngine.ReadString(this.fabricatorConfigAsset.text);
		this.productInfoScreen = global::Util.KInstantiateUI<ProductInfoScreen>(this.productInfoScreenPrefab, this.recipeInfoScreenParent, true);
		this.productInfoScreen.rectTransform().localPosition = new Vector3(280f, 0f, 0f);
		this.productInfoScreen.onElementsFullySelected = new global::System.Action(this.OnRecipeElementsFullySelected);
		Game.Instance.Subscribe(-107300940, new EventSystem.EventHandler(this.OnResearchComplete));
		this.buildingGroupsRoot.gameObject.SetActive(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		PlanScreen.Instance = this;
		UIRegistry.planScreen = this;
		base.onSelect += this.OnClickCategory;
		this.Refresh();
		foreach (KToggle ktoggle in this.toggles)
		{
			ktoggle.group = base.GetComponent<ToggleGroup>();
		}
		this.GetBuildableStates(true);
	}

	private PlanScreen.FabricatorConfig GetFabricatorConfig(string category)
	{
		PlanScreen.FabricatorConfig fabricatorConfig = null;
		string text = category.ToLower();
		for (int i = 0; i < this.fabricatorConfigs.Length; i++)
		{
			if (text == this.fabricatorConfigs[i].planCategory.ToLower())
			{
				fabricatorConfig = this.fabricatorConfigs[i];
				break;
			}
		}
		return fabricatorConfig;
	}

	public void Refresh()
	{
		this.activeSubGroups = new Dictionary<string, PlanScreen.FabricatorConfig>();
		Dictionary<string, List<BuildingDef>> dictionary = new Dictionary<string, List<BuildingDef>>();
		foreach (BuildingDef buildingDef in Assets.BuildingDefs)
		{
			if (buildingDef.PlanCategory != null && !buildingDef.Deprecated)
			{
				List<BuildingDef> list = null;
				if (!dictionary.TryGetValue(buildingDef.PlanCategory, out list))
				{
					list = new List<BuildingDef>();
					dictionary[buildingDef.PlanCategory] = list;
				}
				list.Add(buildingDef);
			}
		}
		List<string> list2 = new List<string>();
		List<KIconToggleMenu.ToggleInfo> list3 = new List<KIconToggleMenu.ToggleInfo>();
		int num = 0;
		foreach (object obj in Enum.GetValues(typeof(PlanCategory)))
		{
			PlanCategory planCategory = (PlanCategory)((int)obj);
			if (planCategory != PlanCategory.MAX)
			{
				string text = planCategory.ToString();
				PlanScreen.FabricatorConfig fabricatorConfig = this.GetFabricatorConfig(text);
				if (fabricatorConfig == null)
				{
					Output.LogError(new object[] { text });
				}
				if (fabricatorConfig.parentCategory.Length != 0)
				{
					this.activeSubGroups.Add(text, fabricatorConfig);
					fabricatorConfig = this.GetFabricatorConfig(fabricatorConfig.parentCategory);
					text = fabricatorConfig.planCategory;
				}
				list2.Add(planCategory.ToString());
				if (num >= 12)
				{
					throw new ArgumentOutOfRangeException();
				}
				global::Action action = global::Action.Plan1 + num;
				KIconToggleMenu.ToggleInfo toggleInfo = new KIconToggleMenu.ToggleInfo(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + text.ToUpper() + ".NAME"), fabricatorConfig.iconName, text, action, Strings.Get("STRINGS.UI.BUILDCATEGORIES." + text.ToUpper() + ".TOOLTIP"));
				list3.Add(toggleInfo);
				num++;
			}
		}
		foreach (KeyValuePair<string, List<BuildingDef>> keyValuePair in dictionary)
		{
			if (!list2.Contains(keyValuePair.Key))
			{
				Output.LogError(new object[] { keyValuePair.Key + " category is not in enum" });
			}
		}
		base.Setup(list3);
		this.toggles.ForEach(delegate(KToggle to)
		{
			ImageToggleState[] components = to.GetComponents<ImageToggleState>();
			foreach (ImageToggleState imageToggleState in components)
			{
				if (imageToggleState.TargetImage.sprite != null && imageToggleState.TargetImage.name == "FG" && !imageToggleState.spritesInitialized)
				{
					imageToggleState.SetSprites(Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"), imageToggleState.TargetImage.sprite, imageToggleState.TargetImage.sprite, Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"));
				}
			}
		});
	}

	protected override void OnCmpEnable()
	{
		this.Refresh();
		this.productInfoScreen.Show(false);
	}

	protected override void OnCmpDisable()
	{
		this.ClearButtons();
	}

	private void ClearButtons()
	{
		foreach (KeyValuePair<BuildingDef, KToggle> keyValuePair in this.ActiveToggles)
		{
			keyValuePair.Value.gameObject.SetActive(false);
			keyValuePair.Value.transform.SetParent(null);
			global::UnityEngine.Object.DestroyImmediate(keyValuePair.Value.gameObject);
		}
		this.ActiveToggles.Clear();
	}

	private void OnSelectBuilding(GameObject button_go, BuildingDef def)
	{
		string text = GlobalAssets.GetSound("HUD_Click", false);
		if (button_go == null)
		{
			Output.LogWithObj(base.gameObject, new object[] { "Button gameObject is null" });
			return;
		}
		if (button_go == this.selectedBuildingGameObject)
		{
			this.CloseRecipe();
			text = GlobalAssets.GetSound("HUD_Click_Deselect", false);
			if (text != null)
			{
				KMonoBehaviour.PlaySound(text);
			}
			return;
		}
		this.selectedBuildingGameObject = button_go;
		this.currentlySelectedToggle = button_go.GetComponent<KToggle>();
		if (text != null)
		{
			KMonoBehaviour.PlaySound(text);
		}
		this.productInfoScreen.ClearProduct(false);
		ToolMenu.Instance.ClearSelection();
		PrebuildTool.Instance.Activate(def, this.BuildableState(def));
		this.productInfoScreen.Show(true);
		this.productInfoScreen.ConfigureScreen(def);
	}

	private void GetBuildableStates(bool force_update)
	{
		if (Assets.BuildingDefs == null || Assets.BuildingDefs.Length == 0)
		{
			return;
		}
		if (this.timeSinceNotificationPing < this.specialNotificationEmbellishDelay)
		{
			this.timeSinceNotificationPing += Time.unscaledDeltaTime;
		}
		if (this.timeSinceNotificationPing >= this.notificationPingExpire)
		{
			this.notificationPingCount = 0;
		}
		int num = 10;
		if (force_update)
		{
			num = Assets.BuildingDefs.Length;
			this.buildable_state_update_idx = 0;
		}
		List<string> list = new List<string>();
		for (int i = 0; i < num; i++)
		{
			BuildingDef buildingDef = Assets.BuildingDefs[this.buildable_state_update_idx];
			if (!buildingDef.Deprecated)
			{
				PlanScreen.RequirementsState requirementsState = PlanScreen.RequirementsState.Complete;
				if (!DebugHandler.InstantBuildMode)
				{
					if (buildingDef.RequiredTech != null && !buildingDef.RequiredTech.IsComplete())
					{
						requirementsState = PlanScreen.RequirementsState.Tech;
					}
					else if (!ProductInfoScreen.MaterialsMet(buildingDef.CraftRecipe))
					{
						requirementsState = PlanScreen.RequirementsState.Materials;
					}
				}
				if (!this.buildableDefs.ContainsKey(buildingDef))
				{
					this.buildableDefs.Add(buildingDef, requirementsState);
				}
				else if (this.buildableDefs[buildingDef] != requirementsState)
				{
					this.buildableDefs[buildingDef] = requirementsState;
					if (this.productInfoScreen.currentDef == buildingDef)
					{
						this.productInfoScreen.ClearProduct(false);
						this.productInfoScreen.Show(true);
						this.productInfoScreen.ConfigureScreen(buildingDef);
					}
					if (requirementsState == PlanScreen.RequirementsState.Complete)
					{
						foreach (KIconToggleMenu.ToggleInfo toggleInfo in this.toggleInfo)
						{
							if ((string)toggleInfo.userData == buildingDef.PlanCategory)
							{
								string text = "NotificationPing";
								Animator component = toggleInfo.toggle.GetComponent<Animator>();
								if (!component.GetCurrentAnimatorStateInfo(0).IsTag(text) && !list.Contains(buildingDef.PlanCategory))
								{
									list.Add(buildingDef.PlanCategory);
									toggleInfo.toggle.gameObject.GetComponent<Animator>().Play(text);
									if (this.timeSinceNotificationPing >= this.specialNotificationEmbellishDelay)
									{
										string sound = GlobalAssets.GetSound("NewBuildable_Embellishment", false);
										if (sound != null)
										{
											EventInstance eventInstance = SoundEvent.BeginOneShot(sound, SoundListenerController.Instance.transform.position);
											SoundEvent.EndOneShot(eventInstance);
										}
									}
									string sound2 = GlobalAssets.GetSound("NewBuildable", false);
									if (sound2 != null)
									{
										EventInstance eventInstance2 = SoundEvent.BeginOneShot(sound2, SoundListenerController.Instance.transform.position);
										eventInstance2.setParameterValue("playCount", (float)this.notificationPingCount);
										SoundEvent.EndOneShot(eventInstance2);
									}
									this.timeSinceNotificationPing = 0f;
									this.notificationPingCount++;
								}
							}
						}
					}
				}
				this.buildable_state_update_idx = (this.buildable_state_update_idx + 1) % Assets.BuildingDefs.Length;
			}
		}
	}

	private void SetCategoryButtonState()
	{
		foreach (KIconToggleMenu.ToggleInfo toggleInfo in this.toggleInfo)
		{
			if (this.activeCategoryInfo != null)
			{
				toggleInfo.toggle.ActivateFlourish(toggleInfo.userData == this.activeCategoryInfo.userData);
			}
			else
			{
				toggleInfo.toggle.ActivateFlourish(false);
			}
			bool flag = false;
			bool flag2 = true;
			if (DebugHandler.InstantBuildMode)
			{
				flag = true;
				flag2 = false;
			}
			else
			{
				foreach (BuildingDef buildingDef in Assets.BuildingDefs)
				{
					if (buildingDef.PlanCategory == (string)toggleInfo.userData)
					{
						PlanScreen.RequirementsState requirementsState = this.BuildableState(buildingDef);
						if (requirementsState == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode)
						{
							flag = true;
							flag2 = false;
							break;
						}
						if (buildingDef.RequiredTech == null || buildingDef.RequiredTech.IsComplete() || buildingDef.RequiredTech.ArePrerequisitesComplete())
						{
							flag2 = false;
						}
					}
				}
			}
			if (!this.CategoryInteractive.ContainsKey(toggleInfo))
			{
				this.CategoryInteractive.Add(toggleInfo, !flag2);
			}
			else
			{
				this.CategoryInteractive[toggleInfo] = !flag2;
			}
			if (!flag)
			{
				toggleInfo.toggle.fgImage.SetAlpha((!flag2) ? 1f : 0.2509804f);
				if (this.activeCategoryInfo != null && toggleInfo.userData == this.activeCategoryInfo.userData)
				{
					this.SetImageToggleState(toggleInfo.toggle.gameObject, ImageToggleState.State.DisabledActive);
				}
				else
				{
					this.SetImageToggleState(toggleInfo.toggle.gameObject, ImageToggleState.State.Disabled);
				}
			}
			else if (this.activeCategoryInfo == null || toggleInfo.userData != this.activeCategoryInfo.userData)
			{
				this.SetImageToggleState(toggleInfo.toggle.gameObject, ImageToggleState.State.Inactive);
			}
			else
			{
				this.SetImageToggleState(toggleInfo.toggle.gameObject, ImageToggleState.State.Active);
			}
			GameObject gameObject = toggleInfo.toggle.fgImage.transform.FindChild("ResearchIcon").gameObject;
			gameObject.gameObject.SetActive(flag2);
		}
	}

	public void SetImageToggleState(GameObject target, ImageToggleState.State state)
	{
		ImageToggleState[] components = target.GetComponents<ImageToggleState>();
		foreach (ImageToggleState imageToggleState in components)
		{
			imageToggleState.SetState(state);
		}
	}

	public void CloseRecipe()
	{
		if (PlayerController.Instance.ActiveTool == PrebuildTool.Instance)
		{
			ToolMenu.Instance.ClearSelection();
		}
		if (PlayerController.Instance.ActiveTool == BuildTool.Instance || PlayerController.Instance.ActiveTool == UtilityBuildTool.Instance || PlayerController.Instance.ActiveTool == WireBuildTool.Instance)
		{
			PlayerController.Instance.ActivateTool(SelectTool.Instance);
		}
		if (this.productInfoScreen != null)
		{
			this.productInfoScreen.ClearProduct(true);
		}
		if (this.activeCategoryInfo != null)
		{
			this.UpdateBuildingButtonList(this.activeCategoryInfo);
		}
		this.selectedBuildingGameObject = null;
	}

	private void CloseCategoryPanel(bool playSound = true)
	{
		this.activeCategoryInfo = null;
		if (playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
		}
		this.buildingGroupsRoot.GetComponent<ExpandRevealUIContent>().Collapse(delegate(object s)
		{
			this.ClearButtons();
			this.buildingGroupsRoot.gameObject.SetActive(false);
		});
		this.PlanCategoryLabel.text = string.Empty;
	}

	private void OnClickCategory(KIconToggleMenu.ToggleInfo toggle_info)
	{
		this.CloseRecipe();
		if (!this.CategoryInteractive.ContainsKey(toggle_info) || !this.CategoryInteractive[toggle_info])
		{
			this.CloseCategoryPanel(false);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
			return;
		}
		string text = (string)toggle_info.userData;
		if (this.activeCategoryInfo == toggle_info)
		{
			this.CloseCategoryPanel(true);
		}
		else
		{
			this.ClearButtons();
			this.buildingGroupsRoot.gameObject.SetActive(true);
			this.activeCategoryInfo = toggle_info;
			UISounds.PlaySound(UISounds.Sound.ClickObject);
			this.BuildButtonList(text, this.GroupsTransform.gameObject);
			this.PlanCategoryLabel.text = this.activeCategoryInfo.text.ToUpper();
			this.buildingGroupsRoot.GetComponent<ExpandRevealUIContent>().Expand(null);
		}
		this.ConfigurePanelSize();
		this.SetScrollPoint(0f);
	}

	private void UpdateBuildingButtonList(KIconToggleMenu.ToggleInfo toggle_info)
	{
		KToggle ktoggle = toggle_info.toggle;
		if (ktoggle == null)
		{
			foreach (KIconToggleMenu.ToggleInfo toggleInfo in this.toggleInfo)
			{
				if (toggleInfo.userData == toggle_info.userData)
				{
					ktoggle = toggleInfo.toggle;
				}
			}
		}
		int num = 2;
		if (ktoggle != null && this.ActiveToggles.Count != 0)
		{
			ktoggle.gameObject.GetComponent<PlanCategoryNotifications>().ToggleAttention(false);
			for (int i = 0; i < num; i++)
			{
				if (this.building_button_refresh_idx >= this.ActiveToggles.Count)
				{
					this.building_button_refresh_idx = 0;
				}
				this.RefreshBuildingButton(this.ActiveToggles.ElementAt<KeyValuePair<BuildingDef, KToggle>>(this.building_button_refresh_idx).Key, this.ActiveToggles.ElementAt<KeyValuePair<BuildingDef, KToggle>>(this.building_button_refresh_idx).Value);
				this.building_button_refresh_idx++;
			}
		}
		if (this.productInfoScreen.gameObject.activeSelf)
		{
			this.productInfoScreen.materialSelectionPanel.UpdateResourceToggleValues();
		}
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		this.GetBuildableStates(false);
		this.SetCategoryButtonState();
		if (this.activeCategoryInfo != null)
		{
			this.UpdateBuildingButtonList(this.activeCategoryInfo);
		}
	}

	private void BuildButtonList(string plan_category, GameObject parent)
	{
		IOrderedEnumerable<BuildingDef> orderedEnumerable = from def in Assets.BuildingDefs
			where def.PlanCategory == plan_category && !def.Deprecated
			orderby def.PlanOrder
			select def;
		this.ActiveToggles.Clear();
		int num = 0;
		foreach (BuildingDef buildingDef in orderedEnumerable)
		{
			if (buildingDef.ShowInBuildMenu)
			{
				this.CreateButton(buildingDef, parent, plan_category, num);
				num++;
			}
		}
	}

	private void ConfigurePanelSize()
	{
		GridLayoutGroup component = this.GroupsTransform.GetComponent<GridLayoutGroup>();
		this.buildGrid_bg_rowHeight = component.cellSize.y + component.spacing.y;
		int num = this.GroupsTransform.childCount;
		for (int i = 0; i < this.GroupsTransform.childCount; i++)
		{
			if (!this.GroupsTransform.GetChild(i).gameObject.activeSelf)
			{
				num--;
			}
		}
		int num2 = Mathf.CeilToInt((float)num / 3f);
		this.BuildingGroupContentsRect.GetComponent<ScrollRect>().verticalScrollbar.gameObject.SetActive(num2 >= 4);
		this.buildingGroupsRoot.sizeDelta = new Vector2(this.buildGrid_bg_width, this.buildGrid_bg_borderHeight + (float)Mathf.Clamp(num2, 0, this.buildGrid_maxRowsBeforeScroll) * this.buildGrid_bg_rowHeight);
	}

	private void SetScrollPoint(float targetY)
	{
		this.BuildingGroupContentsRect.anchoredPosition = new Vector2(this.BuildingGroupContentsRect.anchoredPosition.x, targetY);
	}

	private GameObject CreateButton(BuildingDef def, GameObject parent, string plan_category, int btnIndex)
	{
		GameObject button_go = global::Util.KInstantiateUI(this.planButtonPrefab, parent, true);
		button_go.name = def.name + " Group:" + plan_category;
		KToggle componentInChildren = button_go.GetComponentInChildren<KToggle>();
		this.ActiveToggles.Add(def, componentInChildren);
		this.RefreshBuildingButton(def, componentInChildren);
		componentInChildren.onClick += delegate
		{
			this.OnSelectBuilding(button_go, def);
		};
		return button_go;
	}

	public void RefreshBuildingButton(BuildingDef def, KToggle toggle)
	{
		if (toggle == null)
		{
			return;
		}
		bool flag = true;
		if (def.RequiredTech != null && !def.RequiredTech.IsComplete() && !DebugHandler.InstantBuildMode && !def.RequiredTech.ArePrerequisitesComplete())
		{
			flag = false;
		}
		if (toggle.gameObject.activeSelf != flag)
		{
			toggle.gameObject.SetActive(flag);
			this.ConfigurePanelSize();
			this.SetScrollPoint(0f);
		}
		if (!toggle.gameObject.activeInHierarchy)
		{
			return;
		}
		if (toggle.bgImage == null)
		{
			return;
		}
		Image image = toggle.bgImage.GetComponentsInChildren<Image>()[1];
		Sprite uisprite = def.GetUISprite("ui");
		if (uisprite == null)
		{
			uisprite = this.defaultBuildingIconSprite;
		}
		image.sprite = uisprite;
		image.SetNativeSize();
		image.rectTransform().sizeDelta /= 4f;
		ToolTip component = toggle.gameObject.GetComponent<ToolTip>();
		this.PositionTooltip(toggle, component);
		component.ClearMultiStringTooltip();
		string name = def.Name;
		string effect = def.Effect;
		component.AddMultiStringTooltip(name, this.buildingToolTipSettings.BuildButtonName);
		component.AddMultiStringTooltip(effect, this.buildingToolTipSettings.BuildButtonDescription);
		LocText componentInChildren = toggle.GetComponentInChildren<LocText>();
		if (componentInChildren != null)
		{
			componentInChildren.text = def.Name;
		}
		bool flag2 = def.RequiredTech == null || def.RequiredTech.IsComplete() || DebugHandler.InstantBuildMode;
		int num = ((this.BuildableState(def) != PlanScreen.RequirementsState.Complete) ? 0 : 1);
		ImageToggleState.State state;
		if (toggle.gameObject == this.selectedBuildingGameObject && (this.BuildableState(def) == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode))
		{
			state = ImageToggleState.State.Active;
		}
		else
		{
			state = ((this.BuildableState(def) != PlanScreen.RequirementsState.Complete && !DebugHandler.InstantBuildMode) ? ImageToggleState.State.Disabled : ImageToggleState.State.Inactive);
		}
		if (toggle.gameObject == this.selectedBuildingGameObject && state == ImageToggleState.State.Disabled)
		{
			state = ImageToggleState.State.DisabledActive;
		}
		else if (state == ImageToggleState.State.Disabled)
		{
			state = ImageToggleState.State.Disabled;
		}
		toggle.GetComponent<ImageToggleState>().SetState(state);
		Material material = ((this.BuildableState(def) != PlanScreen.RequirementsState.Complete && !DebugHandler.InstantBuildMode) ? this.desaturatedUIMaterial : this.defaultUIMaterial);
		if (image.material != material)
		{
			image.material = material;
			if (material == this.desaturatedUIMaterial)
			{
				if (flag2)
				{
					image.color = new Color(1f, 1f, 1f, 0.6f);
				}
				else
				{
					image.color = new Color(1f, 1f, 1f, 0.15f);
				}
			}
			else
			{
				image.color = Color.white;
			}
		}
		Image fgImage = toggle.gameObject.GetComponent<KToggle>().fgImage;
		fgImage.gameObject.SetActive(false);
		if (!flag2)
		{
			fgImage.sprite = this.Overlay_NeedTech;
			fgImage.gameObject.SetActive(true);
			string text = string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, def.RequiredTech.Name);
			component.AddMultiStringTooltip("\n", this.buildingToolTipSettings.ResearchRequirement);
			component.AddMultiStringTooltip(text, this.buildingToolTipSettings.ResearchRequirement);
		}
		else if (this.BuildableState(def) != PlanScreen.RequirementsState.Complete)
		{
			fgImage.gameObject.SetActive(false);
			component.AddMultiStringTooltip("\n", this.buildingToolTipSettings.ResearchRequirement);
			string text2 = UI.PRODUCTINFO_MISSINGRESOURCES_HOVER;
			component.AddMultiStringTooltip(text2, this.buildingToolTipSettings.ResearchRequirement);
			foreach (Recipe.Ingredient ingredient in def.CraftRecipe.Ingredients)
			{
				string text3 = string.Format("{0}{1}: {2}", "• ", ingredient.tag.ProperName(), GameUtil.GetFormattedMass(ingredient.amount, GameUtil.TimeSlice.None, true, "F1"));
				component.AddMultiStringTooltip(text3, this.buildingToolTipSettings.ResearchRequirement);
			}
			component.AddMultiStringTooltip(string.Empty, this.buildingToolTipSettings.ResearchRequirement);
		}
	}

	private void PositionTooltip(KToggle toggle, ToolTip tip)
	{
		if (!this.productInfoScreen.gameObject.activeSelf)
		{
			tip.overrideParentObject = this.buildingGroupsRoot;
		}
		else
		{
			tip.overrideParentObject = this.productInfoScreen.rectTransform();
		}
	}

	private void SetMaterialTint(KToggle toggle, bool disabled)
	{
		SwapUIAnimationController component = toggle.GetComponent<SwapUIAnimationController>();
		if (component != null)
		{
			if (disabled)
			{
				component.SetState(false);
			}
			else if (!disabled)
			{
				component.SetState(true);
			}
		}
	}

	public void ExternalClose()
	{
		if (this.activeCategoryInfo != null)
		{
			this.selected = -1;
			this.OnClickCategory(this.activeCategoryInfo);
			SelectTool.Instance.Activate();
			PlayerController.Instance.ActivateTool(SelectTool.Instance);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.Consumed)
		{
			return;
		}
		if (this.mouseOver && this.ConsumeMouseScroll)
		{
			if (!e.TryConsume(global::Action.ZoomIn))
			{
				if (e.TryConsume(global::Action.ZoomOut))
				{
				}
			}
		}
		if (this.toggles == null)
		{
			return;
		}
		if (this.selectedBuildingGameObject != null && e.TryConsume(global::Action.MouseRight))
		{
			this.CloseRecipe();
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
		}
		else if (this.activeCategoryInfo != null && e.TryConsume(global::Action.MouseRight))
		{
			this.ExternalClose();
		}
		if (!e.Consumed && this.activeCategoryInfo != null && e.TryConsume(global::Action.Escape))
		{
			this.OnClickCategory(this.activeCategoryInfo);
			SelectTool.Instance.Activate();
			this.ClearSelection();
		}
		else if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	private void OnRecipeElementsFullySelected()
	{
		BuildingDef buildingDef = null;
		foreach (KeyValuePair<BuildingDef, KToggle> keyValuePair in this.ActiveToggles)
		{
			if (keyValuePair.Value == this.currentlySelectedToggle)
			{
				buildingDef = keyValuePair.Key;
				break;
			}
		}
		if (buildingDef == null)
		{
			Debug.Log("No def!");
		}
		if (buildingDef.isKAnimTile && buildingDef.isUtility)
		{
			IList<Element> getSelectedElementAsList = this.productInfoScreen.materialSelectionPanel.GetSelectedElementAsList;
			bool flag = buildingDef.BuildingComplete.GetComponent<Wire>() != null;
			if (flag)
			{
				WireBuildTool.Instance.Activate(buildingDef, getSelectedElementAsList);
			}
			else
			{
				UtilityBuildTool.Instance.Activate(buildingDef, getSelectedElementAsList);
			}
		}
		else
		{
			BuildTool.Instance.Activate(buildingDef, this.productInfoScreen.materialSelectionPanel.GetSelectedElementAsList);
		}
	}

	public void OnResearchComplete(object tech)
	{
		Tech tech2 = (Tech)tech;
		foreach (BuildingDef buildingDef in tech2.unlockedBuildings)
		{
			foreach (KIconToggleMenu.ToggleInfo toggleInfo in this.toggleInfo)
			{
				if ((string)toggleInfo.userData == buildingDef.PlanCategory)
				{
					toggleInfo.toggle.gameObject.GetComponent<PlanCategoryNotifications>().ToggleAttention(true);
				}
			}
		}
	}

	[SerializeField]
	private GameObject planButtonPrefab;

	[SerializeField]
	private GameObject recipeInfoScreenParent;

	[SerializeField]
	private GameObject productInfoScreenPrefab;

	[MyCmpAdd]
	private Notifier Notifier;

	private Dictionary<KIconToggleMenu.ToggleInfo, bool> CategoryInteractive = new Dictionary<KIconToggleMenu.ToggleInfo, bool>();

	private ProductInfoScreen productInfoScreen;

	[SerializeField]
	public PlanScreen.BuildingToolTipSettings buildingToolTipSettings;

	public PlanScreen.BuildingNameTextSetting buildingNameTextSettings;

	[SerializeField]
	private TextAsset fabricatorConfigAsset;

	private PlanScreen.FabricatorConfig[] fabricatorConfigs;

	private KIconToggleMenu.ToggleInfo activeCategoryInfo;

	private Dictionary<BuildingDef, KToggle> ActiveToggles = new Dictionary<BuildingDef, KToggle>();

	private float timeSinceNotificationPing;

	private float notificationPingExpire = 0.5f;

	private float specialNotificationEmbellishDelay = 8f;

	private int notificationPingCount;

	private GameObject selectedBuildingGameObject;

	public Transform GroupsTransform;

	public Sprite Overlay_NeedTech;

	public RectTransform buildingGroupsRoot;

	public RectTransform BuildButtonBGPanel;

	public RectTransform BuildingGroupContentsRect;

	public Sprite defaultBuildingIconSprite;

	public Material defaultUIMaterial;

	public Material desaturatedUIMaterial;

	public LocText PlanCategoryLabel;

	private Dictionary<Def, PlanScreen.RequirementsState> buildableDefs = new Dictionary<Def, PlanScreen.RequirementsState>();

	[SerializeField]
	private TextStyleSetting[] CategoryLabelTextStyles;

	private Dictionary<string, PlanScreen.FabricatorConfig> activeSubGroups;

	private int buildable_state_update_idx;

	private int building_button_refresh_idx;

	private float buildGrid_bg_width = 274f;

	private float buildGrid_bg_borderHeight = 32f;

	private float buildGrid_bg_rowHeight;

	private int buildGrid_maxRowsBeforeScroll = 3;

	[Serializable]
	public struct BuildingToolTipSettings
	{
		public TextStyleSetting BuildButtonName;

		public TextStyleSetting BuildButtonDescription;

		public TextStyleSetting MaterialRequirement;

		public TextStyleSetting ResearchRequirement;
	}

	[Serializable]
	public struct BuildingNameTextSetting
	{
		public TextStyleSetting ActiveSelected;

		public TextStyleSetting ActiveDeselected;

		public TextStyleSetting InactiveSelected;

		public TextStyleSetting InactiveDeselected;
	}

	[IgnoreCommentedLines("#")]
	[IgnoreFirst(1)]
	[DelimitedRecord(",")]
	[IgnoreEmptyLines]
	private class FabricatorConfig
	{
		public string planCategory;

		[FieldNullValue(typeof(string), "")]
		public string parentCategory;

		public string iconName;
	}

	public enum RequirementsState
	{
		Tech,
		Materials,
		Complete
	}
}
