using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.UI;

public class PlanScreen : KIconToggleMenu
{
	public static PlanScreen Instance { get; private set; }

	public static void DestroyInstance()
	{
		PlanScreen.Instance = null;
	}

	public static Dictionary<PlanScreen.PlanCategory, string> IconNameMap
	{
		get
		{
			return PlanScreen.iconNameMap;
		}
	}

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
		if (BuildMenu.UseHotkeyBuildMenu())
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			base.OnPrefabInit();
			this.productInfoScreen = global::Util.KInstantiateUI<ProductInfoScreen>(this.productInfoScreenPrefab, this.recipeInfoScreenParent, true);
			this.productInfoScreen.rectTransform().pivot = new Vector2(0f, 0f);
			this.productInfoScreen.rectTransform().SetLocalPosition(new Vector3(280f, 0f, 0f));
			this.productInfoScreen.onElementsFullySelected = new global::System.Action(this.OnRecipeElementsFullySelected);
			Game.Instance.Subscribe(-107300940, new Action<object>(this.OnResearchComplete));
			Game.Instance.Subscribe(1174281782, new Action<object>(this.OnActiveToolChanged));
		}
		this.buildingGroupsRoot.gameObject.SetActive(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.initTime = KTime.Instance.UnscaledGameTime;
		if (BuildMenu.UseHotkeyBuildMenu())
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			PlanScreen.Instance = this;
			base.onSelect += this.OnClickCategory;
			this.Refresh();
			foreach (KToggle ktoggle in this.toggles)
			{
				ktoggle.group = base.GetComponent<ToggleGroup>();
			}
			this.GetBuildableStates(true);
			Game.Instance.Subscribe(288942073, new Action<object>(this.OnUIClear));
		}
	}

	public void Refresh()
	{
		List<KIconToggleMenu.ToggleInfo> list = new List<KIconToggleMenu.ToggleInfo>();
		if (this.tagCategoryMap == null)
		{
			int num = 0;
			this.tagCategoryMap = new Dictionary<Tag, PlanScreen.PlanCategory>();
			this.tagOrderMap = new Dictionary<Tag, int>();
			if (global::TUNING.BUILDINGS.PLANORDER.Length > 12)
			{
				Output.LogWarning(new object[]
				{
					"Insufficient keys to cover root plan menu",
					"Max of 12 keys supported but TUNING.BUILDINGS.PLANORDER has " + global::TUNING.BUILDINGS.PLANORDER.Length
				});
			}
			this.toggleEntries.Clear();
			for (int i = 0; i < global::TUNING.BUILDINGS.PLANORDER.Length; i++)
			{
				PlanScreen.PlanInfo planInfo = global::TUNING.BUILDINGS.PLANORDER[i];
				global::Action action = ((i >= 12) ? global::Action.NumActions : (global::Action.Plan1 + i));
				string text = PlanScreen.iconNameMap[planInfo.category];
				string text2 = planInfo.category.ToString().ToUpper();
				KIconToggleMenu.ToggleInfo toggleInfo = new KIconToggleMenu.ToggleInfo(UI.StripLinkFormatting(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + text2 + ".NAME")), text, planInfo.category, action, Strings.Get("STRINGS.UI.BUILDCATEGORIES." + text2 + ".TOOLTIP"), string.Empty);
				list.Add(toggleInfo);
				PlanScreen.PopulateOrderInfo(planInfo.category, planInfo.data, this.tagCategoryMap, this.tagOrderMap, ref num);
				List<BuildingDef> list2 = new List<BuildingDef>();
				foreach (BuildingDef buildingDef in Assets.BuildingDefs)
				{
					PlanScreen.PlanCategory planCategory;
					if (this.tagCategoryMap.TryGetValue(buildingDef.Tag, out planCategory))
					{
						if (planCategory == planInfo.category)
						{
							list2.Add(buildingDef);
						}
					}
				}
				this.toggleEntries.Add(new PlanScreen.ToggleEntry(toggleInfo, planInfo.category, list2));
			}
			base.Setup(list);
			this.toggles.ForEach(delegate(KToggle to)
			{
				ImageToggleState[] components = to.GetComponents<ImageToggleState>();
				foreach (ImageToggleState imageToggleState in components)
				{
					if (imageToggleState.TargetImage.sprite != null && imageToggleState.TargetImage.name == "FG" && !imageToggleState.useSprites)
					{
						imageToggleState.SetSprites(Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"), imageToggleState.TargetImage.sprite, imageToggleState.TargetImage.sprite, Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"));
					}
				}
				to.GetComponent<KToggle>().soundPlayer.Enabled = false;
			});
			for (int k = 0; k < this.toggleEntries.Count; k++)
			{
				PlanScreen.ToggleEntry toggleEntry = this.toggleEntries[k];
				toggleEntry.CollectToggleImages();
				this.toggleEntries[k] = toggleEntry;
			}
		}
	}

	private static void PopulateOrderInfo(PlanScreen.PlanCategory category, object data, Dictionary<Tag, PlanScreen.PlanCategory> category_map, Dictionary<Tag, int> order_map, ref int building_index)
	{
		if (data.GetType() == typeof(PlanScreen.PlanInfo))
		{
			PlanScreen.PlanInfo planInfo = (PlanScreen.PlanInfo)data;
			PlanScreen.PopulateOrderInfo(planInfo.category, planInfo.data, category_map, order_map, ref building_index);
		}
		else
		{
			string[] array = (string[])data;
			foreach (string text in array)
			{
				Tag tag = new Tag(text);
				category_map[tag] = category;
				order_map[tag] = building_index;
				building_index++;
			}
		}
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
		if (button_go == null)
		{
			Output.LogWithObj(base.gameObject, new object[] { "Button gameObject is null" });
			return;
		}
		if (button_go == this.selectedBuildingGameObject)
		{
			this.CloseRecipe(true);
			return;
		}
		this.ignoreToolChangeMessages++;
		this.selectedBuildingGameObject = button_go;
		this.currentlySelectedToggle = button_go.GetComponent<KToggle>();
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
		this.productInfoScreen.ClearProduct(false);
		ToolMenu.Instance.ClearSelection();
		PrebuildTool.Instance.Activate(def, this.BuildableState(def));
		this.productInfoScreen.Show(true);
		this.productInfoScreen.ConfigureScreen(def);
		this.ignoreToolChangeMessages--;
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
		ListPool<PlanScreen.PlanCategory, PlanScreen>.PooledList pooledList = ListPool<PlanScreen.PlanCategory, PlanScreen>.Allocate();
		for (int i = 0; i < num; i++)
		{
			this.buildable_state_update_idx = (this.buildable_state_update_idx + 1) % Assets.BuildingDefs.Length;
			BuildingDef buildingDef = Assets.BuildingDefs[this.buildable_state_update_idx];
			if (!buildingDef.Deprecated)
			{
				PlanScreen.PlanCategory planCategory;
				if (this.tagCategoryMap.TryGetValue(buildingDef.Tag, out planCategory))
				{
					PlanScreen.RequirementsState requirementsState = PlanScreen.RequirementsState.Complete;
					if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive)
					{
						if (!Db.Get().TechItems.IsTechItemComplete(buildingDef.PrefabID))
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
							this.ignoreToolChangeMessages++;
							this.productInfoScreen.ClearProduct(false);
							this.productInfoScreen.Show(true);
							this.productInfoScreen.ConfigureScreen(buildingDef);
							this.ignoreToolChangeMessages--;
						}
						if (requirementsState == PlanScreen.RequirementsState.Complete)
						{
							foreach (KIconToggleMenu.ToggleInfo toggleInfo in this.toggleInfo)
							{
								PlanScreen.PlanCategory planCategory2 = (PlanScreen.PlanCategory)toggleInfo.userData;
								if (planCategory2 == planCategory)
								{
									string text = "NotificationPing";
									Animator component = toggleInfo.toggle.GetComponent<Animator>();
									if (!component.GetCurrentAnimatorStateInfo(0).IsTag(text) && !pooledList.Contains(planCategory))
									{
										pooledList.Add(planCategory);
										toggleInfo.toggle.gameObject.GetComponent<Animator>().Play(text);
										if (KTime.Instance.UnscaledGameTime - this.initTime > 1.5f)
										{
											if (this.timeSinceNotificationPing >= this.specialNotificationEmbellishDelay)
											{
												string sound = GlobalAssets.GetSound("NewBuildable_Embellishment", false);
												if (sound != null)
												{
													EventInstance eventInstance = SoundEvent.BeginOneShot(sound, SoundListenerController.Instance.transform.GetPosition());
													SoundEvent.EndOneShot(eventInstance);
												}
											}
											string sound2 = GlobalAssets.GetSound("NewBuildable", false);
											if (sound2 != null)
											{
												EventInstance eventInstance2 = SoundEvent.BeginOneShot(sound2, SoundListenerController.Instance.transform.GetPosition());
												eventInstance2.setParameterValue("playCount", (float)this.notificationPingCount);
												SoundEvent.EndOneShot(eventInstance2);
											}
										}
										this.timeSinceNotificationPing = 0f;
										this.notificationPingCount++;
									}
								}
							}
						}
					}
				}
			}
		}
		pooledList.Recycle();
	}

	private void SetCategoryButtonState()
	{
		foreach (PlanScreen.ToggleEntry toggleEntry in this.toggleEntries)
		{
			KIconToggleMenu.ToggleInfo toggleInfo = toggleEntry.toggleInfo;
			toggleInfo.toggle.ActivateFlourish(this.activeCategoryInfo != null && toggleInfo.userData == this.activeCategoryInfo.userData);
			bool flag = false;
			bool flag2 = true;
			if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
			{
				flag = true;
				flag2 = false;
			}
			else
			{
				foreach (BuildingDef buildingDef in toggleEntry.buildingDefs)
				{
					PlanScreen.RequirementsState requirementsState = this.BuildableState(buildingDef);
					if (requirementsState == PlanScreen.RequirementsState.Complete)
					{
						flag = true;
						flag2 = false;
						break;
					}
				}
				if (flag2 && toggleEntry.AreAnyRequiredTechItemsAvailable())
				{
					flag2 = false;
				}
			}
			this.CategoryInteractive[toggleInfo] = !flag2;
			if (!flag)
			{
				toggleInfo.toggle.fgImage.SetAlpha((!flag2) ? 1f : 0.2509804f);
				ImageToggleState.State state = ((this.activeCategoryInfo == null || toggleInfo.userData != this.activeCategoryInfo.userData) ? ImageToggleState.State.Disabled : ImageToggleState.State.DisabledActive);
				foreach (ImageToggleState imageToggleState in toggleEntry.toggleImages)
				{
					imageToggleState.SetState(state);
				}
			}
			else
			{
				ImageToggleState.State state2 = ((this.activeCategoryInfo != null && toggleInfo.userData == this.activeCategoryInfo.userData) ? ImageToggleState.State.Active : ImageToggleState.State.Inactive);
				foreach (ImageToggleState imageToggleState2 in toggleEntry.toggleImages)
				{
					imageToggleState2.SetState(state2);
				}
			}
			GameObject gameObject = toggleInfo.toggle.fgImage.transform.Find("ResearchIcon").gameObject;
			gameObject.gameObject.SetActive(flag2);
		}
	}

	private void DeactivateBuildTools()
	{
		InterfaceTool activeTool = PlayerController.Instance.ActiveTool;
		if (activeTool != null)
		{
			Type type = activeTool.GetType();
			if (type == typeof(BuildTool) || typeof(BaseUtilityBuildTool).IsAssignableFrom(type))
			{
				activeTool.DeactivateTool(null);
			}
		}
	}

	public void CloseRecipe(bool playSound = false)
	{
		if (playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
		}
		if (PlayerController.Instance.ActiveTool is PrebuildTool || PlayerController.Instance.ActiveTool is BuildTool)
		{
			ToolMenu.Instance.ClearSelection();
		}
		this.DeactivateBuildTools();
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
		this.CloseRecipe(false);
		if (!this.CategoryInteractive.ContainsKey(toggle_info) || !this.CategoryInteractive[toggle_info])
		{
			this.CloseCategoryPanel(false);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
			return;
		}
		PlanScreen.PlanCategory planCategory = (PlanScreen.PlanCategory)toggle_info.userData;
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
			this.BuildButtonList(planCategory, this.GroupsTransform.gameObject);
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

	private void BuildButtonList(PlanScreen.PlanCategory plan_category, GameObject parent)
	{
		IOrderedEnumerable<BuildingDef> orderedEnumerable = from def in Assets.BuildingDefs
			where this.tagCategoryMap.ContainsKey(def.Tag) && this.tagCategoryMap[def.Tag] == plan_category && !def.Deprecated
			orderby this.tagOrderMap[def.Tag]
			select def;
		this.ActiveToggles.Clear();
		int num = 0;
		string text = plan_category.ToString();
		foreach (BuildingDef buildingDef in orderedEnumerable)
		{
			if (buildingDef.ShowInBuildMenu)
			{
				this.CreateButton(buildingDef, parent, text, num);
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
		button_go.name = UI.StripLinkFormatting(def.name) + " Group:" + plan_category;
		KToggle componentInChildren = button_go.GetComponentInChildren<KToggle>();
		componentInChildren.soundPlayer.Enabled = false;
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
		TechItem techItem = Db.Get().TechItems.TryGet(def.PrefabID);
		bool flag = DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || techItem == null || techItem.IsComplete();
		bool flag2 = flag || techItem == null || techItem.parentTech.ArePrerequisitesComplete();
		if (toggle.gameObject.activeSelf != flag2)
		{
			toggle.gameObject.SetActive(flag2);
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
		Sprite uisprite = def.GetUISprite("ui", false);
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
		int num = ((this.BuildableState(def) != PlanScreen.RequirementsState.Complete) ? 0 : 1);
		ImageToggleState.State state;
		if (toggle.gameObject == this.selectedBuildingGameObject && (this.BuildableState(def) == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive))
		{
			state = ImageToggleState.State.Active;
		}
		else
		{
			state = ((this.BuildableState(def) != PlanScreen.RequirementsState.Complete && !DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive) ? ImageToggleState.State.Disabled : ImageToggleState.State.Inactive);
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
		Material material = ((this.BuildableState(def) != PlanScreen.RequirementsState.Complete && !DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive) ? this.desaturatedUIMaterial : this.defaultUIMaterial);
		if (image.material != material)
		{
			image.material = material;
			if (material == this.desaturatedUIMaterial)
			{
				if (flag)
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
		if (!flag)
		{
			fgImage.sprite = this.Overlay_NeedTech;
			fgImage.gameObject.SetActive(true);
			string text = string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, techItem.parentTech.Name);
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
				string text3 = string.Format("{0}{1}: {2}", "• ", ingredient.tag.ProperName(), GameUtil.GetFormattedMass(ingredient.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				component.AddMultiStringTooltip(text3, this.buildingToolTipSettings.ResearchRequirement);
			}
			component.AddMultiStringTooltip(string.Empty, this.buildingToolTipSettings.ResearchRequirement);
		}
	}

	private void PositionTooltip(KToggle toggle, ToolTip tip)
	{
		tip.overrideParentObject = ((!this.productInfoScreen.gameObject.activeSelf) ? this.buildingGroupsRoot : this.productInfoScreen.rectTransform());
	}

	private void SetMaterialTint(KToggle toggle, bool disabled)
	{
		SwapUIAnimationController component = toggle.GetComponent<SwapUIAnimationController>();
		if (component != null)
		{
			component.SetState(!disabled);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.Consumed)
		{
			return;
		}
		if (!this.mouseOver || !this.ConsumeMouseScroll || e.TryConsume(global::Action.ZoomIn) || e.TryConsume(global::Action.ZoomOut))
		{
		}
		if (this.toggles == null)
		{
			return;
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

	public override void OnKeyUp(KButtonEvent e)
	{
		if (this.selectedBuildingGameObject != null && PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
		{
			this.CloseRecipe(false);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
		}
		else if (this.activeCategoryInfo != null && PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
		{
			this.OnUIClear(null);
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
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
			global::Debug.Log("No def!", null);
		}
		if (buildingDef.isKAnimTile && buildingDef.isUtility)
		{
			IList<Element> getSelectedElementAsList = this.productInfoScreen.materialSelectionPanel.GetSelectedElementAsList;
			bool flag = buildingDef.BuildingComplete.GetComponent<Wire>() != null;
			BaseUtilityBuildTool baseUtilityBuildTool = ((!flag) ? UtilityBuildTool.Instance : WireBuildTool.Instance);
			baseUtilityBuildTool.Activate(buildingDef, getSelectedElementAsList);
		}
		else
		{
			BuildTool.Instance.Activate(buildingDef, this.productInfoScreen.materialSelectionPanel.GetSelectedElementAsList, null);
		}
	}

	public void OnResearchComplete(object tech)
	{
		Tech tech2 = (Tech)tech;
		foreach (TechItem techItem in tech2.unlockedItems)
		{
			BuildingDef buildingDef = Assets.GetBuildingDef(techItem.Id);
			if (buildingDef != null)
			{
				PlanScreen.PlanCategory planCategory = this.tagCategoryMap[buildingDef.Tag];
				foreach (KIconToggleMenu.ToggleInfo toggleInfo in this.toggleInfo)
				{
					PlanScreen.PlanCategory planCategory2 = (PlanScreen.PlanCategory)toggleInfo.userData;
					if (planCategory == planCategory2)
					{
						toggleInfo.toggle.gameObject.GetComponent<PlanCategoryNotifications>().ToggleAttention(true);
					}
				}
			}
		}
	}

	private void OnUIClear(object data)
	{
		if (this.activeCategoryInfo != null)
		{
			this.selected = -1;
			this.OnClickCategory(this.activeCategoryInfo);
			SelectTool.Instance.Activate();
			PlayerController.Instance.ActivateTool(SelectTool.Instance);
			SelectTool.Instance.Select(null, true);
		}
	}

	private void OnActiveToolChanged(object data)
	{
		if (data == null)
		{
			return;
		}
		if (this.ignoreToolChangeMessages > 0)
		{
			return;
		}
		Type type = data.GetType();
		if (!typeof(BuildTool).IsAssignableFrom(type) && !typeof(PrebuildTool).IsAssignableFrom(type) && !typeof(BaseUtilityBuildTool).IsAssignableFrom(type))
		{
			this.CloseRecipe(false);
			this.CloseCategoryPanel(false);
		}
	}

	public PrioritySetting GetBuildingPriority()
	{
		return this.productInfoScreen.materialSelectionPanel.PriorityScreen.GetLastSelectedPriority();
	}

	[SerializeField]
	private GameObject planButtonPrefab;

	[SerializeField]
	private GameObject recipeInfoScreenParent;

	[SerializeField]
	private GameObject productInfoScreenPrefab;

	private static Dictionary<PlanScreen.PlanCategory, string> iconNameMap = new Dictionary<PlanScreen.PlanCategory, string>
	{
		{
			PlanScreen.PlanCategory.Base,
			"icon_category_base"
		},
		{
			PlanScreen.PlanCategory.Oxygen,
			"icon_category_oxygen"
		},
		{
			PlanScreen.PlanCategory.Power,
			"icon_category_electrical"
		},
		{
			PlanScreen.PlanCategory.Food,
			"icon_category_food"
		},
		{
			PlanScreen.PlanCategory.Plumbing,
			"icon_category_plumbing"
		},
		{
			PlanScreen.PlanCategory.HVAC,
			"icon_category_ventilation"
		},
		{
			PlanScreen.PlanCategory.Refining,
			"icon_category_refinery"
		},
		{
			PlanScreen.PlanCategory.Medical,
			"icon_category_medical"
		},
		{
			PlanScreen.PlanCategory.Furniture,
			"icon_category_furniture"
		},
		{
			PlanScreen.PlanCategory.Equipment,
			"icon_category_misc"
		},
		{
			PlanScreen.PlanCategory.Utilities,
			"icon_category_utilities"
		},
		{
			PlanScreen.PlanCategory.Automation,
			"icon_category_automation"
		},
		{
			PlanScreen.PlanCategory.Conveyance,
			"icon_category_shipping"
		}
	};

	private Dictionary<KIconToggleMenu.ToggleInfo, bool> CategoryInteractive = new Dictionary<KIconToggleMenu.ToggleInfo, bool>();

	private ProductInfoScreen productInfoScreen;

	[SerializeField]
	public PlanScreen.BuildingToolTipSettings buildingToolTipSettings;

	public PlanScreen.BuildingNameTextSetting buildingNameTextSettings;

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

	private List<PlanScreen.ToggleEntry> toggleEntries = new List<PlanScreen.ToggleEntry>();

	private int ignoreToolChangeMessages;

	private Dictionary<Def, PlanScreen.RequirementsState> buildableDefs = new Dictionary<Def, PlanScreen.RequirementsState>();

	[SerializeField]
	private TextStyleSetting[] CategoryLabelTextStyles;

	private float initTime;

	private Dictionary<Tag, PlanScreen.PlanCategory> tagCategoryMap;

	private Dictionary<Tag, int> tagOrderMap;

	private int buildable_state_update_idx;

	private int building_button_refresh_idx;

	private float buildGrid_bg_width = 274f;

	private float buildGrid_bg_borderHeight = 32f;

	private float buildGrid_bg_rowHeight;

	private int buildGrid_maxRowsBeforeScroll = 3;

	public enum PlanCategory
	{
		Base,
		Oxygen,
		Power,
		Food,
		Plumbing,
		HVAC,
		Refining,
		Medical,
		Equipment,
		Furniture,
		Utilities,
		Automation,
		Conveyance
	}

	public struct PlanInfo
	{
		public PlanInfo(PlanScreen.PlanCategory category, object data)
		{
			this.category = category;
			this.data = data;
		}

		public PlanScreen.PlanCategory category;

		public object data;
	}

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

	private struct ToggleEntry
	{
		public ToggleEntry(KIconToggleMenu.ToggleInfo toggle_info, PlanScreen.PlanCategory plan_category, List<BuildingDef> building_defs)
		{
			this.toggleInfo = toggle_info;
			this.planCategory = plan_category;
			this.buildingDefs = building_defs;
			this.requiredTechItems = new List<TechItem>();
			this.toggleImages = null;
			foreach (BuildingDef buildingDef in building_defs)
			{
				TechItem techItem = Db.Get().TechItems.TryGet(buildingDef.PrefabID);
				if (techItem == null)
				{
					this.requiredTechItems.Clear();
					break;
				}
				if (!this.requiredTechItems.Contains(techItem))
				{
					this.requiredTechItems.Add(techItem);
				}
			}
		}

		public bool AreAnyRequiredTechItemsAvailable()
		{
			if (this.requiredTechItems.Count == 0)
			{
				return true;
			}
			foreach (TechItem techItem in this.requiredTechItems)
			{
				if (techItem.IsComplete() || techItem.parentTech.ArePrerequisitesComplete())
				{
					return true;
				}
			}
			return false;
		}

		public void CollectToggleImages()
		{
			this.toggleImages = this.toggleInfo.toggle.gameObject.GetComponents<ImageToggleState>();
		}

		public KIconToggleMenu.ToggleInfo toggleInfo;

		public PlanScreen.PlanCategory planCategory;

		public List<BuildingDef> buildingDefs;

		private List<TechItem> requiredTechItems;

		public ImageToggleState[] toggleImages;
	}

	public enum RequirementsState
	{
		Tech,
		Materials,
		Complete
	}
}
