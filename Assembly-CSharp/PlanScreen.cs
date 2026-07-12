using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlanScreen : KIconToggleMenu
{
	public static PlanScreen Instance { get; private set; }

	public static void DestroyInstance()
	{
		PlanScreen.Instance = null;
	}

	public static Dictionary<HashedString, string> IconNameMap
	{
		get
		{
			return PlanScreen.iconNameMap;
		}
	}

	private static HashedString CacheHashedString(string str)
	{
		return HashCache.Get().Add(str);
	}

	public override float GetSortKey()
	{
		return 2f;
	}

	private PlanScreen.RequirementsState BuildableState(BuildingDef def)
	{
		PlanScreen.RequirementsState requirementsState;
		if (def == null || !this._buildableStates.TryGetValue(def, out requirementsState))
		{
			requirementsState = PlanScreen.RequirementsState.Materials;
		}
		return requirementsState;
	}

	private bool IsDefResearched(BuildingDef def)
	{
		bool flag = false;
		if (!this._researchedDefs.TryGetValue(def, out flag))
		{
			flag = this.UpdateDefResearched(def);
		}
		return flag;
	}

	private bool UpdateDefResearched(BuildingDef def)
	{
		return this._researchedDefs[def] = Db.Get().TechItems.IsTechItemComplete(def.PrefabID);
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
			PlanScreen.Instance = this;
			this.productInfoScreen = global::Util.KInstantiateUI<ProductInfoScreen>(this.productInfoScreenPrefab, this.recipeInfoScreenParent, false);
			this.productInfoScreen.rectTransform().pivot = new Vector2(0f, 0f);
			this.productInfoScreen.rectTransform().SetLocalPosition(new Vector3(326f, 0f, 0f));
			this.productInfoScreen.onElementsFullySelected = new global::System.Action(this.OnRecipeElementsFullySelected);
			KInputManager.InputChange.AddListener(new UnityAction(this.RefreshToolTip));
			this.planScreenScrollRect = base.transform.parent.GetComponentInParent<KScrollRect>();
			Game.Instance.Subscribe(-107300940, new Action<object>(this.OnResearchComplete));
			Game.Instance.Subscribe(1174281782, new Action<object>(this.OnActiveToolChanged));
		}
		this.buildingGroupsRoot.gameObject.SetActive(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.ConsumeMouseScroll = true;
		this.initTime = KTime.Instance.UnscaledGameTime;
		if (BuildMenu.UseHotkeyBuildMenu())
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			base.onSelect += this.OnClickCategory;
			this.Refresh();
			foreach (KToggle ktoggle in this.toggles)
			{
				ktoggle.group = base.GetComponent<ToggleGroup>();
			}
			this.GetBuildableStates(true);
			Game.Instance.Subscribe(288942073, new Action<object>(this.OnUIClear));
		}
		this.copyBuildingButton.GetComponent<MultiToggle>().onClick = delegate
		{
			this.OnClickCopyBuilding();
		};
		this.RefreshCopyBuildingButton(null);
		Game.Instance.Subscribe(-1503271301, new Action<object>(this.RefreshCopyBuildingButton));
		Game.Instance.Subscribe(1983128072, delegate(object data)
		{
			this.CloseRecipe(false);
		});
		this.pointerEnterActions = (KScreen.PointerEnterActions)Delegate.Combine(this.pointerEnterActions, new KScreen.PointerEnterActions(this.PointerEnter));
		this.pointerExitActions = (KScreen.PointerExitActions)Delegate.Combine(this.pointerExitActions, new KScreen.PointerExitActions(this.PointerExit));
		this.copyBuildingButton.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString(UI.COPY_BUILDING_TOOLTIP, global::Action.CopyBuilding));
		this.RefreshScale(null);
		this.refreshScaleHandle = Game.Instance.Subscribe(-442024484, new Action<object>(this.RefreshScale));
	}

	private void RefreshScale(object data = null)
	{
		base.GetComponent<GridLayoutGroup>().cellSize = (ScreenResolutionMonitor.UsingGamepadUIMode() ? new Vector2(54f, 50f) : new Vector2(45f, 45f));
		this.toggles.ForEach(delegate(KToggle to)
		{
			to.GetComponentInChildren<LocText>().fontSize = (float)(ScreenResolutionMonitor.UsingGamepadUIMode() ? PlanScreen.fontSizeBigMode : PlanScreen.fontSizeStandardMode);
		});
		LayoutElement component = this.copyBuildingButton.GetComponent<LayoutElement>();
		component.minWidth = (float)(ScreenResolutionMonitor.UsingGamepadUIMode() ? 58 : 54);
		component.minHeight = (float)(ScreenResolutionMonitor.UsingGamepadUIMode() ? 58 : 54);
		base.gameObject.rectTransform().anchoredPosition = new Vector2(0f, (float)(ScreenResolutionMonitor.UsingGamepadUIMode() ? (-68) : (-74)));
		this.adjacentPinnedButtons.GetComponent<HorizontalLayoutGroup>().padding.bottom = (ScreenResolutionMonitor.UsingGamepadUIMode() ? 14 : 6);
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.subCategoryObjects)
		{
			keyValuePair.Value.GetComponentInChildren<GridLayoutGroup>().cellSize = (ScreenResolutionMonitor.UsingGamepadUIMode() ? PlanScreen.bigBuildingButtonSize : PlanScreen.standarduildingButtonSize);
		}
		Vector2 sizeDelta = this.buildingGroupsRoot.rectTransform().sizeDelta;
		Vector2 vector = (ScreenResolutionMonitor.UsingGamepadUIMode() ? new Vector2(320f, sizeDelta.y) : new Vector2(264f, sizeDelta.y));
		this.buildingGroupsRoot.rectTransform().sizeDelta = vector;
		this.productInfoScreen.rectTransform().anchoredPosition = new Vector2(vector.x + 8f, this.productInfoScreen.rectTransform().anchoredPosition.y);
	}

	protected override void OnCleanUp()
	{
		if (Game.Instance != null)
		{
			Game.Instance.Unsubscribe(this.refreshScaleHandle);
		}
		base.OnCleanUp();
	}

	private void OnClickCopyBuilding()
	{
		if (this.lastSelectedBuilding == null)
		{
			return;
		}
		PlanScreen.Instance.CopyBuildingOrder(this.lastSelectedBuilding);
	}

	public void RefreshCopyBuildingButton(object data = null)
	{
		this.adjacentPinnedButtons.rectTransform().anchoredPosition = new Vector2(Mathf.Min(base.gameObject.rectTransform().sizeDelta.x, base.transform.parent.rectTransform().rect.width), 0f);
		MultiToggle component = this.copyBuildingButton.GetComponent<MultiToggle>();
		if (SelectTool.Instance != null && SelectTool.Instance.selected != null)
		{
			Building component2 = SelectTool.Instance.selected.GetComponent<Building>();
			if (component2 != null && component2.Def.ShouldShowInBuildMenu() && component2.Def.IsAvailable())
			{
				this.lastSelectedBuilding = component2;
			}
		}
		if (this.lastSelectedBuilding != null)
		{
			global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(this.lastSelectedBuilding.gameObject, "ui", false);
			component.gameObject.SetActive(true);
			component.transform.Find("FG").GetComponent<Image>().sprite = uisprite.first;
			component.transform.Find("FG").GetComponent<Image>().color = Color.white;
			component.ChangeState(1);
			return;
		}
		component.gameObject.SetActive(false);
		component.ChangeState(0);
	}

	public void RefreshToolTip()
	{
		for (int i = 0; i < global::TUNING.BUILDINGS.PLANORDER.Count; i++)
		{
			PlanScreen.PlanInfo planInfo = global::TUNING.BUILDINGS.PLANORDER[i];
			if (DlcManager.IsContentActive(planInfo.RequiredDlcId))
			{
				global::Action action = ((i < 14) ? (global::Action.Plan1 + i) : global::Action.NumActions);
				string text = HashCache.Get().Get(planInfo.category).ToUpper();
				this.toggleInfo[i].tooltip = GameUtil.ReplaceHotkeyString(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + text + ".TOOLTIP"), action);
			}
		}
		this.copyBuildingButton.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString(UI.COPY_BUILDING_TOOLTIP, global::Action.CopyBuilding));
	}

	public void Refresh()
	{
		List<KIconToggleMenu.ToggleInfo> list = new List<KIconToggleMenu.ToggleInfo>();
		if (this.tagCategoryMap == null)
		{
			int num = 0;
			this.tagCategoryMap = new Dictionary<Tag, HashedString>();
			this.tagOrderMap = new Dictionary<Tag, int>();
			if (global::TUNING.BUILDINGS.PLANORDER.Count > 14)
			{
				DebugUtil.LogWarningArgs(new object[]
				{
					"Insufficient keys to cover root plan menu",
					"Max of 14 keys supported but TUNING.BUILDINGS.PLANORDER has " + global::TUNING.BUILDINGS.PLANORDER.Count.ToString()
				});
			}
			this.toggleEntries.Clear();
			for (int i = 0; i < global::TUNING.BUILDINGS.PLANORDER.Count; i++)
			{
				PlanScreen.PlanInfo planInfo = global::TUNING.BUILDINGS.PLANORDER[i];
				if (DlcManager.IsContentActive(planInfo.RequiredDlcId))
				{
					global::Action action = ((i < 14) ? (global::Action.Plan1 + i) : global::Action.NumActions);
					string text = PlanScreen.iconNameMap[planInfo.category];
					string text2 = HashCache.Get().Get(planInfo.category).ToUpper();
					KIconToggleMenu.ToggleInfo toggleInfo = new KIconToggleMenu.ToggleInfo(UI.StripLinkFormatting(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + text2 + ".NAME")), text, planInfo.category, action, GameUtil.ReplaceHotkeyString(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + text2 + ".TOOLTIP"), action), "");
					list.Add(toggleInfo);
					PlanScreen.PopulateOrderInfo(planInfo.category, planInfo.buildingAndSubcategoryData, this.tagCategoryMap, this.tagOrderMap, ref num);
					List<BuildingDef> list2 = new List<BuildingDef>();
					foreach (BuildingDef buildingDef in Assets.BuildingDefs)
					{
						HashedString hashedString;
						if (buildingDef.IsAvailable() && this.tagCategoryMap.TryGetValue(buildingDef.Tag, out hashedString) && !(hashedString != planInfo.category))
						{
							list2.Add(buildingDef);
						}
					}
					this.toggleEntries.Add(new PlanScreen.ToggleEntry(toggleInfo, planInfo.category, list2, planInfo.hideIfNotResearched));
				}
			}
			base.Setup(list);
			this.toggles.ForEach(delegate(KToggle to)
			{
				foreach (ImageToggleState imageToggleState in to.GetComponents<ImageToggleState>())
				{
					if (imageToggleState.TargetImage.sprite != null && imageToggleState.TargetImage.name == "FG" && !imageToggleState.useSprites)
					{
						imageToggleState.SetSprites(Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"), imageToggleState.TargetImage.sprite, imageToggleState.TargetImage.sprite, Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"));
					}
				}
				to.GetComponent<KToggle>().soundPlayer.Enabled = false;
				to.GetComponentInChildren<LocText>().fontSize = (float)(ScreenResolutionMonitor.UsingGamepadUIMode() ? PlanScreen.fontSizeBigMode : PlanScreen.fontSizeStandardMode);
			});
			for (int j = 0; j < this.toggleEntries.Count; j++)
			{
				PlanScreen.ToggleEntry toggleEntry = this.toggleEntries[j];
				toggleEntry.CollectToggleImages();
				this.toggleEntries[j] = toggleEntry;
			}
		}
	}

	public void CopyBuildingOrder(Building building)
	{
		foreach (PlanScreen.PlanInfo planInfo in global::TUNING.BUILDINGS.PLANORDER)
		{
			foreach (KeyValuePair<string, string> keyValuePair in planInfo.buildingAndSubcategoryData)
			{
				if (building.Def.PrefabID == keyValuePair.Key)
				{
					this.OpenCategoryByName(HashCache.Get().Get(planInfo.category));
					this.OnSelectBuilding(this.ActiveToggles[building.Def].gameObject, building.Def);
					this.productInfoScreen.materialSelectionPanel.SelectSourcesMaterials(building);
					Rotatable component = building.GetComponent<Rotatable>();
					if (component != null)
					{
						BuildTool.Instance.SetToolOrientation(component.GetOrientation());
						break;
					}
					break;
				}
			}
		}
	}

	private static void PopulateOrderInfo(HashedString category, object data, Dictionary<Tag, HashedString> category_map, Dictionary<Tag, int> order_map, ref int building_index)
	{
		if (data.GetType() == typeof(PlanScreen.PlanInfo))
		{
			PlanScreen.PlanInfo planInfo = (PlanScreen.PlanInfo)data;
			PlanScreen.PopulateOrderInfo(planInfo.category, planInfo.buildingAndSubcategoryData, category_map, order_map, ref building_index);
			return;
		}
		foreach (KeyValuePair<string, string> keyValuePair in ((List<KeyValuePair<string, string>>)data))
		{
			Tag tag = new Tag(keyValuePair.Key);
			category_map[tag] = category;
			order_map[tag] = building_index;
			building_index++;
		}
	}

	protected override void OnCmpEnable()
	{
		this.Refresh();
		this.RefreshCopyBuildingButton(null);
	}

	protected override void OnCmpDisable()
	{
		this.ClearButtons();
	}

	private void ClearButtons()
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.subCategoryObjects)
		{
			global::UnityEngine.Object.DestroyImmediate(keyValuePair.Value.gameObject);
		}
		this.subCategoryObjects.Clear();
		this.ActiveToggles.Clear();
		this.copyBuildingButton.gameObject.SetActive(false);
		this.copyBuildingButton.GetComponent<MultiToggle>().ChangeState(0);
	}

	public void OnSelectBuilding(GameObject button_go, BuildingDef def)
	{
		if (button_go == null)
		{
			global::Debug.Log("Button gameObject is null", base.gameObject);
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
		HashedString hashedString = this.tagCategoryMap[def.Tag];
		PlanScreen.ToggleEntry toggleEntry;
		if (this.GetToggleEntryForCategory(hashedString, out toggleEntry) && toggleEntry.pendingResearchAttentions.Contains(def.Tag))
		{
			toggleEntry.pendingResearchAttentions.Remove(def.Tag);
			button_go.GetComponent<PlanCategoryNotifications>().ToggleAttention(false);
			if (toggleEntry.pendingResearchAttentions.Count == 0)
			{
				toggleEntry.toggleInfo.toggle.GetComponent<PlanCategoryNotifications>().ToggleAttention(false);
			}
		}
		this.productInfoScreen.ClearProduct(false);
		ToolMenu.Instance.ClearSelection();
		PrebuildTool.Instance.Activate(def, this.GetTooltipForBuildable(def));
		this.lastSelectedBuilding = def.BuildingComplete.GetComponent<Building>();
		this.RefreshCopyBuildingButton(null);
		this.productInfoScreen.Show(true);
		this.productInfoScreen.ConfigureScreen(def);
		this.ignoreToolChangeMessages--;
	}

	private void GetBuildableStates(bool force_update)
	{
		if (Assets.BuildingDefs == null || Assets.BuildingDefs.Count == 0)
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
			num = Assets.BuildingDefs.Count;
			this.buildable_state_update_idx = 0;
		}
		ListPool<HashedString, PlanScreen>.PooledList pooledList = ListPool<HashedString, PlanScreen>.Allocate();
		for (int i = 0; i < num; i++)
		{
			this.buildable_state_update_idx = (this.buildable_state_update_idx + 1) % Assets.BuildingDefs.Count;
			BuildingDef buildingDef = Assets.BuildingDefs[this.buildable_state_update_idx];
			PlanScreen.RequirementsState buildableStateForDef = this.GetBuildableStateForDef(buildingDef);
			HashedString hashedString;
			if (this.tagCategoryMap.TryGetValue(buildingDef.Tag, out hashedString) && (!this._buildableStates.ContainsKey(buildingDef) || this._buildableStates[buildingDef] != buildableStateForDef))
			{
				this._buildableStates[buildingDef] = buildableStateForDef;
				if (this.productInfoScreen.currentDef == buildingDef)
				{
					this.ignoreToolChangeMessages++;
					this.productInfoScreen.ClearProduct(false);
					this.productInfoScreen.Show(true);
					this.productInfoScreen.ConfigureScreen(buildingDef);
					this.ignoreToolChangeMessages--;
				}
				if (buildableStateForDef == PlanScreen.RequirementsState.Complete)
				{
					foreach (KIconToggleMenu.ToggleInfo toggleInfo in this.toggleInfo)
					{
						if ((HashedString)toggleInfo.userData == hashedString)
						{
							Bouncer component = toggleInfo.toggle.GetComponent<Bouncer>();
							if (component != null && !component.IsBouncing() && !pooledList.Contains(hashedString))
							{
								pooledList.Add(hashedString);
								component.Bounce();
								if (KTime.Instance.UnscaledGameTime - this.initTime > 1.5f)
								{
									if (this.timeSinceNotificationPing >= this.specialNotificationEmbellishDelay)
									{
										string sound = GlobalAssets.GetSound("NewBuildable_Embellishment", false);
										if (sound != null)
										{
											SoundEvent.EndOneShot(SoundEvent.BeginOneShot(sound, SoundListenerController.Instance.transform.GetPosition(), 1f, false));
										}
									}
									string sound2 = GlobalAssets.GetSound("NewBuildable", false);
									if (sound2 != null)
									{
										EventInstance eventInstance = SoundEvent.BeginOneShot(sound2, SoundListenerController.Instance.transform.GetPosition(), 1f, false);
										eventInstance.setParameterByName("playCount", (float)this.notificationPingCount, false);
										SoundEvent.EndOneShot(eventInstance);
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
		pooledList.Recycle();
	}

	private PlanScreen.RequirementsState GetBuildableStateForDef(BuildingDef def)
	{
		if (!def.IsAvailable())
		{
			return PlanScreen.RequirementsState.Invalid;
		}
		PlanScreen.RequirementsState requirementsState = PlanScreen.RequirementsState.Complete;
		if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && !this.IsDefResearched(def))
		{
			requirementsState = PlanScreen.RequirementsState.Tech;
		}
		else if (def.BuildingComplete.GetComponent<Telepad>() != null && ClusterUtil.ActiveWorldHasPrinter())
		{
			requirementsState = PlanScreen.RequirementsState.TelepadBuilt;
		}
		else if (def.BuildingComplete.HasTag(GameTags.RocketInteriorBuilding) && !ClusterUtil.ActiveWorldIsRocketInterior())
		{
			requirementsState = PlanScreen.RequirementsState.RocketInteriorOnly;
		}
		else if (def.BuildingComplete.HasTag(GameTags.NotRocketInteriorBuilding) && ClusterUtil.ActiveWorldIsRocketInterior())
		{
			requirementsState = PlanScreen.RequirementsState.RocketInteriorForbidden;
		}
		else if (def.BuildingComplete.HasTag(GameTags.UniquePerWorld) && BuildingInventory.Instance.BuildingCountForWorld_BAD_PERF(def.Tag, ClusterManager.Instance.activeWorldId) > 0)
		{
			requirementsState = PlanScreen.RequirementsState.UniquePerWorld;
		}
		else if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && !ProductInfoScreen.MaterialsMet(def.CraftRecipe))
		{
			requirementsState = PlanScreen.RequirementsState.Materials;
		}
		return requirementsState;
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
					if (this.BuildableState(buildingDef) == PlanScreen.RequirementsState.Complete)
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
			GameObject gameObject = toggleInfo.toggle.fgImage.transform.Find("ResearchIcon").gameObject;
			if (!flag)
			{
				if (flag2 && toggleEntry.hideIfNotResearched)
				{
					toggleInfo.toggle.gameObject.SetActive(false);
				}
				else if (flag2)
				{
					toggleInfo.toggle.gameObject.SetActive(true);
					toggleInfo.toggle.fgImage.SetAlpha(0.2509804f);
					gameObject.gameObject.SetActive(true);
				}
				else
				{
					toggleInfo.toggle.gameObject.SetActive(true);
					toggleInfo.toggle.fgImage.SetAlpha(1f);
					gameObject.gameObject.SetActive(false);
				}
				ImageToggleState.State state = ((this.activeCategoryInfo != null && toggleInfo.userData == this.activeCategoryInfo.userData) ? ImageToggleState.State.DisabledActive : ImageToggleState.State.Disabled);
				ImageToggleState[] array = toggleEntry.toggleImages;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetState(state);
				}
			}
			else
			{
				toggleInfo.toggle.gameObject.SetActive(true);
				toggleInfo.toggle.fgImage.SetAlpha(1f);
				gameObject.gameObject.SetActive(false);
				ImageToggleState.State state2 = ((this.activeCategoryInfo == null || toggleInfo.userData != this.activeCategoryInfo.userData) ? ImageToggleState.State.Inactive : ImageToggleState.State.Active);
				ImageToggleState[] array = toggleEntry.toggleImages;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetState(state2);
				}
			}
		}
		this.RefreshCopyBuildingButton(null);
	}

	private void DeactivateBuildTools()
	{
		InterfaceTool activeTool = PlayerController.Instance.ActiveTool;
		if (activeTool != null)
		{
			Type type = activeTool.GetType();
			if (type == typeof(BuildTool) || typeof(BaseUtilityBuildTool).IsAssignableFrom(type) || type == typeof(PrebuildTool))
			{
				activeTool.DeactivateTool(null);
				PlayerController.Instance.ActivateTool(SelectTool.Instance);
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
		this.PlanCategoryLabel.text = "";
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
		if (this.activeCategoryInfo == toggle_info)
		{
			this.CloseCategoryPanel(true);
		}
		else
		{
			this.OpenCategoryPanel(toggle_info, true);
		}
		this.ConfigurePanelSize();
		this.SetScrollPoint(0f);
	}

	private void OpenCategoryPanel(KIconToggleMenu.ToggleInfo toggle_info, bool play_sound = true)
	{
		HashedString hashedString = (HashedString)toggle_info.userData;
		this.ClearButtons();
		this.buildingGroupsRoot.gameObject.SetActive(true);
		this.activeCategoryInfo = toggle_info;
		if (play_sound)
		{
			UISounds.PlaySound(UISounds.Sound.ClickObject);
		}
		this.BuildButtonList(hashedString, this.GroupsTransform.gameObject);
		this.PlanCategoryLabel.text = this.activeCategoryInfo.text.ToUpper();
		this.buildingGroupsRoot.GetComponent<ExpandRevealUIContent>().Expand(null);
	}

	public void OpenCategoryByName(string category)
	{
		PlanScreen.ToggleEntry toggleEntry;
		if (this.GetToggleEntryForCategory(category, out toggleEntry))
		{
			this.OpenCategoryPanel(toggleEntry.toggleInfo, false);
		}
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
			for (int i = 0; i < num; i++)
			{
				if (this.building_button_refresh_idx >= this.ActiveToggles.Count)
				{
					this.building_button_refresh_idx = 0;
				}
				this.RefreshBuildingButton(this.ActiveToggles.ElementAt<KeyValuePair<BuildingDef, KToggle>>(this.building_button_refresh_idx).Key, this.ActiveToggles.ElementAt<KeyValuePair<BuildingDef, KToggle>>(this.building_button_refresh_idx).Value, (HashedString)toggle_info.userData);
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

	private void BuildButtonList(HashedString plan_category, GameObject parent)
	{
		this.ActiveToggles.Clear();
		int num = 0;
		string text = plan_category.ToString();
		Dictionary<string, List<BuildingDef>> dictionary = new Dictionary<string, List<BuildingDef>>();
		foreach (KeyValuePair<string, string> keyValuePair in global::TUNING.BUILDINGS.PLANORDER.Find((PlanScreen.PlanInfo match) => match.category == plan_category).buildingAndSubcategoryData)
		{
			BuildingDef buildingDef = Assets.GetBuildingDef(keyValuePair.Key);
			if (buildingDef.IsAvailable() && buildingDef.ShowInBuildMenu)
			{
				if (this.USE_SUB_CATEGORY_LAYOUT)
				{
					if (!dictionary.ContainsKey(keyValuePair.Value))
					{
						dictionary.Add(keyValuePair.Value, new List<BuildingDef>());
					}
					dictionary[keyValuePair.Value].Add(buildingDef);
				}
				else
				{
					if (!dictionary.ContainsKey("default"))
					{
						dictionary.Add("default", new List<BuildingDef>());
					}
					dictionary["default"].Add(buildingDef);
				}
			}
		}
		this.subCategoryObjects.Clear();
		foreach (KeyValuePair<string, List<BuildingDef>> keyValuePair2 in dictionary)
		{
			this.subCategoryObjects.Add(keyValuePair2.Key, global::Util.KInstantiateUI(this.subgroupPrefab, parent, true));
			GameObject gameObject = this.subCategoryObjects[keyValuePair2.Key].GetComponent<HierarchyReferences>().GetReference<GridLayoutGroup>("Grid").gameObject;
			this.subCategoryObjects[keyValuePair2.Key].GetComponent<HierarchyReferences>().GetReference<RectTransform>("Header").gameObject.SetActive(this.USE_SUB_CATEGORY_LAYOUT);
			foreach (BuildingDef buildingDef2 in keyValuePair2.Value)
			{
				this.CreateButton(buildingDef2, gameObject, text, num);
				num++;
			}
		}
	}

	private void ConfigurePanelSize()
	{
		this.buildGrid_bg_rowHeight = (ScreenResolutionMonitor.UsingGamepadUIMode() ? PlanScreen.bigBuildingButtonSize.y : PlanScreen.standarduildingButtonSize.y);
		GridLayoutGroup reference = this.subgroupPrefab.GetComponent<HierarchyReferences>().GetReference<GridLayoutGroup>("Grid");
		this.buildGrid_bg_rowHeight += reference.spacing.y;
		int num = 0;
		for (int i = 0; i < this.GroupsTransform.childCount; i++)
		{
			int num2 = 0;
			HierarchyReferences component = this.GroupsTransform.GetChild(i).GetComponent<HierarchyReferences>();
			if (!(component == null))
			{
				GridLayoutGroup reference2 = component.GetReference<GridLayoutGroup>("Grid");
				if (!(reference2 == null))
				{
					for (int j = 0; j < reference2.transform.childCount; j++)
					{
						if (reference2.transform.GetChild(j).gameObject.activeSelf)
						{
							num2++;
						}
					}
					num += num2 / reference2.constraintCount;
					if (num2 % reference2.constraintCount != 0)
					{
						num++;
					}
				}
			}
		}
		int num3 = num;
		int num4 = Math.Max(1, Screen.height / (int)this.buildGrid_bg_rowHeight - 3);
		num4 = Math.Min(num4, 6);
		this.BuildingGroupContentsRect.GetComponent<ScrollRect>().verticalScrollbar.gameObject.SetActive(num3 >= num4 - 1);
		float num5 = this.buildGrid_bg_borderHeight + (float)Mathf.Clamp(num3, 0, num4) * this.buildGrid_bg_rowHeight;
		if (this.USE_SUB_CATEGORY_LAYOUT)
		{
			float minHeight = this.subgroupPrefab.GetComponent<HierarchyReferences>().GetReference("HeaderLabel").transform.parent.GetComponent<LayoutElement>().minHeight;
			num5 += minHeight;
		}
		this.buildingGroupsRoot.sizeDelta = new Vector2(this.buildGrid_bg_width, num5);
		this.RefreshScale(null);
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
		this.RefreshBuildingButton(def, componentInChildren, plan_category);
		componentInChildren.onClick += delegate
		{
			this.OnSelectBuilding(button_go, def);
		};
		return button_go;
	}

	private static bool TechRequirementsMet(TechItem techItem)
	{
		return DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || techItem == null || techItem.IsComplete();
	}

	private static bool TechRequirementsUpcoming(TechItem techItem)
	{
		return PlanScreen.TechRequirementsMet(techItem);
	}

	private bool GetToggleEntryForCategory(HashedString category, out PlanScreen.ToggleEntry toggleEntry)
	{
		toggleEntry = null;
		foreach (PlanScreen.ToggleEntry toggleEntry2 in this.toggleEntries)
		{
			if (toggleEntry2.planCategory == category)
			{
				toggleEntry = toggleEntry2;
				return true;
			}
		}
		return false;
	}

	public void RefreshBuildingButton(BuildingDef def, KToggle toggle, HashedString buildingCategory)
	{
		if (toggle == null)
		{
			return;
		}
		PlanScreen.ToggleEntry toggleEntry;
		if (this.GetToggleEntryForCategory(buildingCategory, out toggleEntry) && toggleEntry.pendingResearchAttentions.Contains(def.Tag))
		{
			toggle.GetComponent<PlanCategoryNotifications>().ToggleAttention(true);
		}
		TechItem techItem = Db.Get().TechItems.TryGet(def.PrefabID);
		bool flag = PlanScreen.TechRequirementsMet(techItem);
		bool flag2 = PlanScreen.TechRequirementsUpcoming(techItem);
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
		float num = (ScreenResolutionMonitor.UsingGamepadUIMode() ? 3.25f : 4f);
		image.rectTransform().sizeDelta /= num;
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
			componentInChildren.fontSize = (float)(ScreenResolutionMonitor.UsingGamepadUIMode() ? PlanScreen.fontSizeBigMode : PlanScreen.fontSizeStandardMode);
			componentInChildren.text = def.Name;
		}
		PlanScreen.RequirementsState requirementsState = this.BuildableState(def);
		bool flag3 = requirementsState == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive;
		bool flag4 = toggle.gameObject == this.selectedBuildingGameObject;
		ImageToggleState.State state = ((requirementsState == PlanScreen.RequirementsState.Complete) ? ImageToggleState.State.Inactive : ImageToggleState.State.Disabled);
		if (flag4 && flag3)
		{
			state = ImageToggleState.State.Active;
		}
		else if (!flag4 && flag3)
		{
			state = ImageToggleState.State.Inactive;
		}
		else if (flag4 && !flag3)
		{
			state = ImageToggleState.State.DisabledActive;
		}
		else if (!flag4 && !flag3)
		{
			state = ImageToggleState.State.Disabled;
		}
		toggle.GetComponent<ImageToggleState>().SetState(state);
		Material material = (flag3 ? this.defaultUIMaterial : this.desaturatedUIMaterial);
		if (image.material != material)
		{
			image.material = material;
			if (!flag3)
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
		if (requirementsState == PlanScreen.RequirementsState.Tech)
		{
			fgImage.sprite = this.Overlay_NeedTech;
			fgImage.gameObject.SetActive(true);
		}
		else
		{
			fgImage.gameObject.SetActive(false);
		}
		string tooltipForRequirementsState = PlanScreen.GetTooltipForRequirementsState(def, requirementsState);
		if (tooltipForRequirementsState != null)
		{
			component.AddMultiStringTooltip("\n", this.buildingToolTipSettings.ResearchRequirement);
			component.AddMultiStringTooltip(tooltipForRequirementsState, this.buildingToolTipSettings.ResearchRequirement);
		}
	}

	public bool IsDefBuildable(BuildingDef def)
	{
		return this.BuildableState(def) == PlanScreen.RequirementsState.Complete;
	}

	public string GetTooltipForBuildable(BuildingDef def)
	{
		PlanScreen.RequirementsState requirementsState = this.BuildableState(def);
		return PlanScreen.GetTooltipForRequirementsState(def, requirementsState);
	}

	public static string GetTooltipForRequirementsState(BuildingDef def, PlanScreen.RequirementsState state)
	{
		TechItem techItem = Db.Get().TechItems.TryGet(def.PrefabID);
		string text = null;
		if (Game.Instance.SandboxModeActive)
		{
			text = UIConstants.ColorPrefixYellow + UI.SANDBOXTOOLS.SETTINGS.INSTANT_BUILD.NAME + UIConstants.ColorSuffix;
		}
		else if (DebugHandler.InstantBuildMode)
		{
			text = UIConstants.ColorPrefixYellow + UI.DEBUG_TOOLS.DEBUG_ACTIVE + UIConstants.ColorSuffix;
		}
		else
		{
			switch (state)
			{
			case PlanScreen.RequirementsState.Tech:
				text = string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, techItem.ParentTech.Name);
				break;
			case PlanScreen.RequirementsState.Materials:
				text = UI.PRODUCTINFO_MISSINGRESOURCES_HOVER;
				foreach (Recipe.Ingredient ingredient in def.CraftRecipe.Ingredients)
				{
					string text2 = string.Format("{0}{1}: {2}", "• ", ingredient.tag.ProperName(), GameUtil.GetFormattedMass(ingredient.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
					text = text + "\n" + text2;
				}
				break;
			case PlanScreen.RequirementsState.TelepadBuilt:
				text = UI.PRODUCTINFO_UNIQUE_PER_WORLD;
				break;
			case PlanScreen.RequirementsState.UniquePerWorld:
				text = UI.PRODUCTINFO_UNIQUE_PER_WORLD;
				break;
			case PlanScreen.RequirementsState.RocketInteriorOnly:
				text = UI.PRODUCTINFO_ROCKET_INTERIOR;
				break;
			case PlanScreen.RequirementsState.RocketInteriorForbidden:
				text = UI.PRODUCTINFO_ROCKET_NOT_INTERIOR;
				break;
			}
		}
		return text;
	}

	private void PositionTooltip(KToggle toggle, ToolTip tip)
	{
		tip.overrideParentObject = this.buildingGroupsRoot;
		tip.tooltipPivot = Vector2.zero;
		tip.parentPositionAnchor = new Vector2(1f, 0f);
		tip.tooltipPositionOffset = (this.productInfoScreen.gameObject.activeSelf ? new Vector2(16f + this.productInfoScreen.rectTransform().sizeDelta.x, 0f) : new Vector2(-40f, 0f));
	}

	private void SetMaterialTint(KToggle toggle, bool disabled)
	{
		SwapUIAnimationController component = toggle.GetComponent<SwapUIAnimationController>();
		if (component != null)
		{
			component.SetState(!disabled);
		}
	}

	private void PointerEnter(PointerEventData data)
	{
		this.planScreenScrollRect.mouseIsOver = true;
	}

	private void PointerExit(PointerEventData data)
	{
		this.planScreenScrollRect.mouseIsOver = false;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.Consumed)
		{
			return;
		}
		if (this.mouseOver && base.ConsumeMouseScroll)
		{
			if (KInputManager.currentControllerIsGamepad)
			{
				if (e.IsAction(global::Action.ZoomIn) || e.IsAction(global::Action.ZoomOut))
				{
					this.planScreenScrollRect.OnKeyDown(e);
				}
			}
			else if (!e.TryConsume(global::Action.ZoomIn))
			{
				e.TryConsume(global::Action.ZoomOut);
			}
		}
		if (e.IsAction(global::Action.CopyBuilding) && e.TryConsume(global::Action.CopyBuilding))
		{
			this.OnClickCopyBuilding();
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
			return;
		}
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (this.mouseOver && base.ConsumeMouseScroll)
		{
			if (KInputManager.currentControllerIsGamepad)
			{
				if (e.IsAction(global::Action.ZoomIn) || e.IsAction(global::Action.ZoomOut))
				{
					this.planScreenScrollRect.OnKeyUp(e);
				}
			}
			else if (!e.TryConsume(global::Action.ZoomIn))
			{
				e.TryConsume(global::Action.ZoomOut);
			}
		}
		if (e.Consumed)
		{
			return;
		}
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
		DebugUtil.DevAssert(buildingDef, "def is null", null);
		if (buildingDef)
		{
			if (buildingDef.isKAnimTile && buildingDef.isUtility)
			{
				IList<Tag> getSelectedElementAsList = this.productInfoScreen.materialSelectionPanel.GetSelectedElementAsList;
				((buildingDef.BuildingComplete.GetComponent<Wire>() != null) ? WireBuildTool.Instance : UtilityBuildTool.Instance).Activate(buildingDef, getSelectedElementAsList);
				return;
			}
			BuildTool.Instance.Activate(buildingDef, this.productInfoScreen.materialSelectionPanel.GetSelectedElementAsList);
		}
	}

	public void OnResearchComplete(object tech)
	{
		foreach (TechItem techItem in ((Tech)tech).unlockedItems)
		{
			BuildingDef buildingDef = Assets.GetBuildingDef(techItem.Id);
			if (buildingDef != null)
			{
				this.UpdateDefResearched(buildingDef);
				if (this.tagCategoryMap.ContainsKey(buildingDef.Tag))
				{
					HashedString hashedString = this.tagCategoryMap[buildingDef.Tag];
					PlanScreen.ToggleEntry toggleEntry;
					if (this.GetToggleEntryForCategory(hashedString, out toggleEntry))
					{
						toggleEntry.pendingResearchAttentions.Add(buildingDef.Tag);
						toggleEntry.toggleInfo.toggle.GetComponent<PlanCategoryNotifications>().ToggleAttention(true);
						toggleEntry.Refresh();
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

	[SerializeField]
	private GameObject copyBuildingButton;

	private bool USE_SUB_CATEGORY_LAYOUT;

	private int refreshScaleHandle = -1;

	[SerializeField]
	private GameObject adjacentPinnedButtons;

	private static Dictionary<HashedString, string> iconNameMap = new Dictionary<HashedString, string>
	{
		{
			PlanScreen.CacheHashedString("Base"),
			"icon_category_base"
		},
		{
			PlanScreen.CacheHashedString("Oxygen"),
			"icon_category_oxygen"
		},
		{
			PlanScreen.CacheHashedString("Power"),
			"icon_category_electrical"
		},
		{
			PlanScreen.CacheHashedString("Food"),
			"icon_category_food"
		},
		{
			PlanScreen.CacheHashedString("Plumbing"),
			"icon_category_plumbing"
		},
		{
			PlanScreen.CacheHashedString("HVAC"),
			"icon_category_ventilation"
		},
		{
			PlanScreen.CacheHashedString("Refining"),
			"icon_category_refinery"
		},
		{
			PlanScreen.CacheHashedString("Medical"),
			"icon_category_medical"
		},
		{
			PlanScreen.CacheHashedString("Furniture"),
			"icon_category_furniture"
		},
		{
			PlanScreen.CacheHashedString("Equipment"),
			"icon_category_misc"
		},
		{
			PlanScreen.CacheHashedString("Utilities"),
			"icon_category_utilities"
		},
		{
			PlanScreen.CacheHashedString("Automation"),
			"icon_category_automation"
		},
		{
			PlanScreen.CacheHashedString("Conveyance"),
			"icon_category_shipping"
		},
		{
			PlanScreen.CacheHashedString("Rocketry"),
			"icon_category_rocketry"
		},
		{
			PlanScreen.CacheHashedString("HEP"),
			"icon_category_radiation"
		}
	};

	private Dictionary<KIconToggleMenu.ToggleInfo, bool> CategoryInteractive = new Dictionary<KIconToggleMenu.ToggleInfo, bool>();

	private ProductInfoScreen productInfoScreen;

	[SerializeField]
	public PlanScreen.BuildingToolTipSettings buildingToolTipSettings;

	public PlanScreen.BuildingNameTextSetting buildingNameTextSettings;

	private KIconToggleMenu.ToggleInfo activeCategoryInfo;

	public Dictionary<BuildingDef, KToggle> ActiveToggles = new Dictionary<BuildingDef, KToggle>();

	private float timeSinceNotificationPing;

	private float notificationPingExpire = 0.5f;

	private float specialNotificationEmbellishDelay = 8f;

	private int notificationPingCount;

	private Dictionary<string, GameObject> subCategoryObjects = new Dictionary<string, GameObject>();

	private static Vector2 bigBuildingButtonSize = new Vector2(98f, 123f);

	private static Vector2 standarduildingButtonSize = PlanScreen.bigBuildingButtonSize * 0.8f;

	private static int fontSizeBigMode = 16;

	private static int fontSizeStandardMode = 14;

	private GameObject selectedBuildingGameObject;

	[SerializeField]
	private GameObject subgroupPrefab;

	public Transform GroupsTransform;

	public Sprite Overlay_NeedTech;

	public RectTransform buildingGroupsRoot;

	public RectTransform BuildButtonBGPanel;

	public RectTransform BuildingGroupContentsRect;

	public Sprite defaultBuildingIconSprite;

	private KScrollRect planScreenScrollRect;

	public Material defaultUIMaterial;

	public Material desaturatedUIMaterial;

	public LocText PlanCategoryLabel;

	private List<PlanScreen.ToggleEntry> toggleEntries = new List<PlanScreen.ToggleEntry>();

	private int ignoreToolChangeMessages;

	private Dictionary<Def, PlanScreen.RequirementsState> _buildableStates = new Dictionary<Def, PlanScreen.RequirementsState>();

	private Dictionary<Def, bool> _researchedDefs = new Dictionary<Def, bool>();

	[SerializeField]
	private TextStyleSetting[] CategoryLabelTextStyles;

	private float initTime;

	private Dictionary<Tag, HashedString> tagCategoryMap;

	private Dictionary<Tag, int> tagOrderMap;

	private Building lastSelectedBuilding;

	private int buildable_state_update_idx;

	private int building_button_refresh_idx;

	private float buildGrid_bg_width = 320f;

	private float buildGrid_bg_borderHeight = 48f;

	private float buildGrid_bg_rowHeight;

	public struct PlanInfo
	{
		public PlanInfo(HashedString category, bool hideIfNotResearched, List<string> listData, string RequiredDlcId = "")
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			foreach (string text in listData)
			{
				list.Add(new KeyValuePair<string, string>(text, global::TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.ContainsKey(text) ? global::TUNING.BUILDINGS.PLANSUBCATEGORYSORTING[text] : "uncategorized"));
			}
			this.category = category;
			this.hideIfNotResearched = hideIfNotResearched;
			this.data = listData;
			this.buildingAndSubcategoryData = list;
			this.RequiredDlcId = RequiredDlcId;
		}

		public HashedString category;

		public bool hideIfNotResearched;

		public List<string> data;

		public List<KeyValuePair<string, string>> buildingAndSubcategoryData;

		public string RequiredDlcId;
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

	private class ToggleEntry
	{
		public ToggleEntry(KIconToggleMenu.ToggleInfo toggle_info, HashedString plan_category, List<BuildingDef> building_defs, bool hideIfNotResearched)
		{
			this.toggleInfo = toggle_info;
			this.planCategory = plan_category;
			this.buildingDefs = building_defs;
			this.hideIfNotResearched = hideIfNotResearched;
			this.pendingResearchAttentions = new List<Tag>();
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
			this._areAnyRequiredTechItemsAvailable = false;
			this.Refresh();
		}

		public bool AreAnyRequiredTechItemsAvailable()
		{
			return this._areAnyRequiredTechItemsAvailable;
		}

		public void Refresh()
		{
			if (this._areAnyRequiredTechItemsAvailable)
			{
				return;
			}
			if (this.requiredTechItems.Count == 0)
			{
				this._areAnyRequiredTechItemsAvailable = true;
				return;
			}
			using (List<TechItem>.Enumerator enumerator = this.requiredTechItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (PlanScreen.TechRequirementsUpcoming(enumerator.Current))
					{
						this._areAnyRequiredTechItemsAvailable = true;
						break;
					}
				}
			}
		}

		public void CollectToggleImages()
		{
			this.toggleImages = this.toggleInfo.toggle.gameObject.GetComponents<ImageToggleState>();
		}

		public KIconToggleMenu.ToggleInfo toggleInfo;

		public HashedString planCategory;

		public List<BuildingDef> buildingDefs;

		public List<Tag> pendingResearchAttentions;

		private List<TechItem> requiredTechItems;

		public ImageToggleState[] toggleImages;

		public bool hideIfNotResearched;

		private bool _areAnyRequiredTechItemsAvailable;
	}

	public enum RequirementsState
	{
		Invalid,
		Tech,
		Materials,
		Complete,
		TelepadBuilt,
		UniquePerWorld,
		RocketInteriorOnly,
		RocketInteriorForbidden
	}
}
