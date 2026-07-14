using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FMOD.Studio;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class DetailsScreen : KTabMenu
{
	public static void DestroyInstance()
	{
		DetailsScreen.Instance = null;
	}

	public GameObject target { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.SortScreenOrder();
		base.ConsumeMouseScroll = true;
		global::Debug.Assert(DetailsScreen.Instance == null);
		DetailsScreen.Instance = this;
		this.InitiateSidescreenTabs();
		this.DeactivateSideContent();
		this.Show(false);
		base.Subscribe(Game.Instance.gameObject, -1503271301, new Action<object>(this.OnSelectObject));
		this.tabHeader.Init();
	}

	public bool CanObjectDisplayTabOfType(GameObject obj, DetailsScreen.SidescreenTabTypes type)
	{
		for (int i = 0; i < this.sidescreenTabs.Length; i++)
		{
			DetailsScreen.SidescreenTab sidescreenTab = this.sidescreenTabs[i];
			if (sidescreenTab.type == type)
			{
				return sidescreenTab.ValidateTarget(obj);
			}
		}
		return false;
	}

	public DetailsScreen.SidescreenTab GetTabOfType(DetailsScreen.SidescreenTabTypes type)
	{
		for (int i = 0; i < this.sidescreenTabs.Length; i++)
		{
			DetailsScreen.SidescreenTab sidescreenTab = this.sidescreenTabs[i];
			if (sidescreenTab.type == type)
			{
				return sidescreenTab;
			}
		}
		return null;
	}

	public void InitiateSidescreenTabs()
	{
		for (int i = 0; i < this.sidescreenTabs.Length; i++)
		{
			DetailsScreen.SidescreenTab sidescreenTab = this.sidescreenTabs[i];
			sidescreenTab.Initiate(this.original_tab, this.original_tab_body, delegate(DetailsScreen.SidescreenTab _tab)
			{
				this.SelectSideScreenTab(_tab.type);
			});
			switch (sidescreenTab.type)
			{
			case DetailsScreen.SidescreenTabTypes.Errands:
				sidescreenTab.ValidateTargetCallback = (GameObject target, DetailsScreen.SidescreenTab _tab) => target.GetComponent<MinionIdentity>() != null;
				break;
			case DetailsScreen.SidescreenTabTypes.Material:
				sidescreenTab.ValidateTargetCallback = delegate(GameObject target, DetailsScreen.SidescreenTab _tab)
				{
					Reconstructable component = target.GetComponent<Reconstructable>();
					return component != null && component.AllowReconstruct;
				};
				break;
			case DetailsScreen.SidescreenTabTypes.Blueprints:
				sidescreenTab.ValidateTargetCallback = delegate(GameObject target, DetailsScreen.SidescreenTab _tab)
				{
					global::UnityEngine.Object component2 = target.GetComponent<MinionIdentity>();
					BuildingFacade component3 = target.GetComponent<BuildingFacade>();
					return component2 != null || component3 != null;
				};
				break;
			}
		}
	}

	private void OnSelectObject(object data)
	{
		if (data == null)
		{
			this.previouslyActiveTab = -1;
			this.SelectSideScreenTab(DetailsScreen.SidescreenTabTypes.Config);
			return;
		}
		KPrefabID component = ((GameObject)data).GetComponent<KPrefabID>();
		if (!(component == null) && !(this.previousTargetID != component.PrefabID()))
		{
			this.SelectSideScreenTab(this.selectedSidescreenTabID);
			return;
		}
		if (component != null && component.GetComponent<MinionIdentity>())
		{
			this.SelectSideScreenTab(DetailsScreen.SidescreenTabTypes.Errands);
			return;
		}
		this.SelectSideScreenTab(DetailsScreen.SidescreenTabTypes.Config);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.CodexEntryButton.onClick += this.CodexEntryButton_OnClick;
		this.PinResourceButton.onClick += this.PinResourceButton_OnClick;
		this.CloseButton.onClick += this.DeselectAndClose;
		this.TabTitle.OnNameChanged += this.OnNameChanged;
		this.TabTitle.OnStartedEditing += this.OnStartedEditing;
		this.sideScreen2.SetActive(false);
		base.Subscribe<DetailsScreen>(-1514841199, DetailsScreen.OnRefreshDataDelegate);
	}

	private void OnStartedEditing()
	{
		base.isEditing = true;
		KScreenManager.Instance.RefreshStack();
	}

	private void OnNameChanged(string newName)
	{
		base.isEditing = false;
		if (string.IsNullOrEmpty(newName))
		{
			return;
		}
		MinionIdentity component = this.target.GetComponent<MinionIdentity>();
		UserNameable component2 = this.target.GetComponent<UserNameable>();
		ClustercraftExteriorDoor component3 = this.target.GetComponent<ClustercraftExteriorDoor>();
		CommandModule component4 = this.target.GetComponent<CommandModule>();
		if (component != null)
		{
			component.SetName(newName);
			if (ScheduleScreen.Instance != null)
			{
				ScheduleScreen.Instance.Trigger(1980521255, null);
			}
		}
		else if (component4 != null)
		{
			SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(component4.GetComponent<LaunchConditionManager>()).SetRocketName(newName);
		}
		else if (component3 != null)
		{
			component3.GetTargetWorld().GetComponent<UserNameable>().SetName(newName);
		}
		else if (component2 != null)
		{
			component2.SetName(newName);
		}
		this.TabTitle.UpdateRenameTooltip(this.target);
	}

	protected override void OnDeactivate()
	{
		if (this.target != null && this.setRocketTitleHandle != -1)
		{
			this.target.Unsubscribe(this.setRocketTitleHandle);
		}
		this.setRocketTitleHandle = -1;
		this.DeactivateSideContent();
		base.OnDeactivate();
	}

	protected override void OnShow(bool show)
	{
		if (!show)
		{
			this.DeactivateSideContent();
		}
		else
		{
			this.MaskSideContent(false);
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().MenuOpenHalfEffect);
		}
		base.OnShow(show);
	}

	protected override void OnCmpDisable()
	{
		this.DeactivateSideContent();
		base.OnCmpDisable();
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!base.isEditing && this.target != null && PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
		{
			this.DeselectAndClose();
		}
	}

	private static Component GetComponent(GameObject go, string name)
	{
		Type type = Type.GetType(name);
		Component component;
		if (type != null)
		{
			component = go.GetComponent(type);
		}
		else
		{
			component = go.GetComponent(name);
		}
		return component;
	}

	private static bool IsExcludedPrefabTag(GameObject go, Tag[] excluded_tags)
	{
		if (excluded_tags == null || excluded_tags.Length == 0)
		{
			return false;
		}
		bool flag = false;
		KPrefabID component = go.GetComponent<KPrefabID>();
		foreach (Tag tag in excluded_tags)
		{
			if (component.PrefabTag == tag)
			{
				flag = true;
				break;
			}
		}
		return flag;
	}

	private string CodexEntryButton_GetCodexId()
	{
		global::Debug.Assert(this.target != null, "Details Screen has no target");
		KSelectable component = this.target.GetComponent<KSelectable>();
		DebugUtil.AssertArgs(component != null, new object[] { "Details Screen target is not a KSelectable", this.target });
		CodexEntryRedirector component2 = component.GetComponent<CodexEntryRedirector>();
		BuildingUnderConstruction component3 = component.GetComponent<BuildingUnderConstruction>();
		CreatureBrain component4 = component.GetComponent<CreatureBrain>();
		PlantableSeed component5 = component.GetComponent<PlantableSeed>();
		ICellSelectionProxy component6 = component.GetComponent<ICellSelectionProxy>();
		string text;
		if (component6 != null && component6.Element != null)
		{
			text = CodexCache.FormatLinkID(component6.Element.id.ToString());
		}
		else if (component2 != null && !string.IsNullOrEmpty(component2.CodexID))
		{
			text = CodexCache.FormatLinkID(component2.CodexID);
		}
		else if (component3 != null)
		{
			text = CodexCache.FormatLinkID(component3.Def.PrefabID);
		}
		else if (component4 != null)
		{
			text = CodexCache.FormatLinkID(component.PrefabID().ToString());
			text = text.Replace("BABY", "");
		}
		else if (component5 != null)
		{
			text = CodexCache.FormatLinkID(component5.PrefabID().ToString());
		}
		else
		{
			text = UI.ExtractLinkID(component.GetProperName());
			if (string.IsNullOrEmpty(text))
			{
				text = CodexCache.FormatLinkID(component.PrefabID().ToString());
			}
		}
		if (CodexCache.entries.ContainsKey(text) || CodexCache.FindSubEntry(text) != null)
		{
			return text;
		}
		return "";
	}

	private void CodexEntryButton_Refresh()
	{
		string text = this.CodexEntryButton_GetCodexId();
		this.CodexEntryButton.isInteractable = text != "";
		this.CodexEntryButton.GetComponent<ToolTip>().SetSimpleTooltip(this.CodexEntryButton.isInteractable ? UI.TOOLTIPS.OPEN_CODEX_ENTRY : UI.TOOLTIPS.NO_CODEX_ENTRY);
	}

	public void CodexEntryButton_OnClick()
	{
		string text = this.CodexEntryButton_GetCodexId();
		if (text != "")
		{
			ManagementMenu.Instance.OpenCodexToEntry(text, null);
		}
	}

	private bool PinResourceButton_TryGetResourceTagAndProperName(out Tag targetTag, out string targetProperName)
	{
		KPrefabID component = this.target.GetComponent<KPrefabID>();
		if (component != null && DetailsScreen.<PinResourceButton_TryGetResourceTagAndProperName>g__ShouldUse|51_0(component.PrefabTag))
		{
			targetTag = component.PrefabTag;
			targetProperName = component.GetProperName();
			return true;
		}
		ICellSelectionProxy component2 = this.target.GetComponent<ICellSelectionProxy>();
		if (component2 != null && component2.Element != null && DetailsScreen.<PinResourceButton_TryGetResourceTagAndProperName>g__ShouldUse|51_0(component2.Element.tag))
		{
			targetTag = component2.Element.tag;
			targetProperName = component2.Element.name;
			return true;
		}
		targetTag = null;
		targetProperName = null;
		return false;
	}

	private void PinResourceButton_Refresh()
	{
		Tag tag;
		string text;
		if (this.PinResourceButton_TryGetResourceTagAndProperName(out tag, out text))
		{
			ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Contains(tag);
			GameUtil.MeasureUnit measureUnit;
			if (!AllResourcesScreen.Instance.units.TryGetValue(tag, out measureUnit))
			{
				measureUnit = GameUtil.MeasureUnit.quantity;
			}
			string text2;
			switch (measureUnit)
			{
			case GameUtil.MeasureUnit.mass:
				text2 = GameUtil.GetFormattedMass(ClusterManager.Instance.activeWorld.worldInventory.GetAmount(tag, false), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
				break;
			case GameUtil.MeasureUnit.kcal:
				text2 = GameUtil.GetFormattedCalories(WorldResourceAmountTracker<RationTracker>.Get().CountAmountForItemWithID(tag.Name, ClusterManager.Instance.activeWorld.worldInventory, true), GameUtil.TimeSlice.None, true);
				break;
			case GameUtil.MeasureUnit.quantity:
				text2 = GameUtil.GetFormattedUnits(ClusterManager.Instance.activeWorld.worldInventory.GetAmount(tag, false), GameUtil.TimeSlice.None, true, "");
				break;
			default:
				text2 = "";
				break;
			}
			this.PinResourceButton.gameObject.SetActive(true);
			this.PinResourceButton.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.TOOLTIPS.OPEN_RESOURCE_INFO, text2, text));
			return;
		}
		this.PinResourceButton.gameObject.SetActive(false);
	}

	public void PinResourceButton_OnClick()
	{
		Tag tag;
		string text;
		if (this.PinResourceButton_TryGetResourceTagAndProperName(out tag, out text))
		{
			AllResourcesScreen.Instance.SetFilter(UI.StripLinkFormatting(text));
			AllResourcesScreen.Instance.Show(true);
		}
	}

	public void OnRefreshData(object obj)
	{
		this.RefreshTitle();
		for (int i = 0; i < this.tabs.Count; i++)
		{
			if (this.tabs[i].gameObject.activeInHierarchy)
			{
				this.tabs[i].Trigger(-1514841199, obj);
			}
		}
	}

	public void Refresh(GameObject go)
	{
		if (this.screens == null)
		{
			return;
		}
		if (this.target != go)
		{
			if (this.setRocketTitleHandle != -1)
			{
				this.target.Unsubscribe(this.setRocketTitleHandle);
				this.setRocketTitleHandle = -1;
			}
			if (this.target != null)
			{
				KPrefabID component = this.target.GetComponent<KPrefabID>();
				if (component != null)
				{
					this.previousTargetID = component.PrefabID();
				}
				else
				{
					this.previousTargetID = null;
				}
			}
		}
		this.target = go;
		this.sortedSideScreens.Clear();
		ICellSelectionProxy component2 = this.target.GetComponent<ICellSelectionProxy>();
		if (component2 != null)
		{
			component2.OnObjectSelected(null);
		}
		this.UpdateTitle();
		this.tabHeader.RefreshTabDisplayForTarget(this.target);
		if (this.sideScreens != null && this.sideScreens.Count > 0)
		{
			bool flag = false;
			foreach (DetailsScreen.SideScreenRef sideScreenRef in this.sideScreens)
			{
				if (!sideScreenRef.screenPrefab.IsValidForTarget(this.target))
				{
					if (sideScreenRef.screenInstance != null && sideScreenRef.screenInstance.gameObject.activeSelf)
					{
						sideScreenRef.screenInstance.gameObject.SetActive(false);
					}
				}
				else
				{
					flag = true;
					if (sideScreenRef.screenInstance == null)
					{
						DetailsScreen.SidescreenTab tabOfType = this.GetTabOfType(sideScreenRef.tab);
						sideScreenRef.screenInstance = global::Util.KInstantiateUI<SideScreenContent>(sideScreenRef.screenPrefab.gameObject, tabOfType.bodyInstance, false);
					}
					if (!this.sideScreen.activeSelf)
					{
						this.sideScreen.SetActive(true);
					}
					sideScreenRef.screenInstance.SetTarget(this.target);
					sideScreenRef.screenInstance.Show(true);
					int sideScreenSortOrder = sideScreenRef.screenInstance.GetSideScreenSortOrder();
					this.sortedSideScreens.Add(new KeyValuePair<DetailsScreen.SideScreenRef, int>(sideScreenRef, sideScreenSortOrder));
				}
			}
			if (!flag)
			{
				if (!this.CanObjectDisplayTabOfType(this.target, DetailsScreen.SidescreenTabTypes.Material) && !this.CanObjectDisplayTabOfType(this.target, DetailsScreen.SidescreenTabTypes.Blueprints))
				{
					this.sideScreen.SetActive(false);
				}
				else
				{
					this.sideScreen.SetActive(true);
				}
			}
		}
		this.sortedSideScreens.Sort(delegate(KeyValuePair<DetailsScreen.SideScreenRef, int> x, KeyValuePair<DetailsScreen.SideScreenRef, int> y)
		{
			if (x.Value <= y.Value)
			{
				return 1;
			}
			return -1;
		});
		for (int i = 0; i < this.sortedSideScreens.Count; i++)
		{
			this.sortedSideScreens[i].Key.screenInstance.transform.SetSiblingIndex(i);
		}
		for (int j = 0; j < this.sidescreenTabs.Length; j++)
		{
			DetailsScreen.SidescreenTab tab = this.sidescreenTabs[j];
			tab.RepositionTitle();
			KeyValuePair<DetailsScreen.SideScreenRef, int> keyValuePair = this.sortedSideScreens.Find((KeyValuePair<DetailsScreen.SideScreenRef, int> t) => t.Key.tab == tab.type);
			tab.SetNoConfigMessageVisibility(keyValuePair.Key == null);
		}
		this.RefreshTitle();
	}

	public void RefreshTitle()
	{
		if (this.target == null)
		{
			return;
		}
		this.TabTitle.SetTitle(this.target.GetProperName());
		for (int i = 0; i < this.sidescreenTabs.Length; i++)
		{
			DetailsScreen.SidescreenTab tab = this.sidescreenTabs[i];
			if (tab.IsVisible)
			{
				KeyValuePair<DetailsScreen.SideScreenRef, int> keyValuePair = this.sortedSideScreens.Find((KeyValuePair<DetailsScreen.SideScreenRef, int> match) => match.Key.tab == tab.type);
				if (keyValuePair.Key != null)
				{
					tab.SetTitleVisibility(keyValuePair.Key.screenInstance.CheckShouldShowTopTitle == null || keyValuePair.Key.screenInstance.CheckShouldShowTopTitle());
					tab.SetTitle(keyValuePair.Key.screenInstance.GetTitle());
				}
				else
				{
					tab.SetTitle(UI.UISIDESCREENS.NOCONFIG.TITLE);
					tab.SetTitleVisibility(tab.type == DetailsScreen.SidescreenTabTypes.Config || tab.type == DetailsScreen.SidescreenTabTypes.Errands);
				}
			}
			if (tab.type == DetailsScreen.SidescreenTabTypes.Config)
			{
				if (this.target.GetComponent<MinionIdentity>() == null)
				{
					tab.Tooltip_Key = "STRINGS.UI.DETAILTABS.CONFIGURATION.TOOLTIP";
				}
				else
				{
					tab.Tooltip_Key = "STRINGS.UI.DETAILTABS.CONFIGURATION.TOOLTIP_DUPLICANT";
				}
				tab.tabInstance.GetComponent<ToolTip>().SetSimpleTooltip(Strings.Get(tab.Tooltip_Key));
			}
		}
	}

	private void SelectSideScreenTab(DetailsScreen.SidescreenTabTypes tabID)
	{
		this.selectedSidescreenTabID = tabID;
		this.RefreshSideScreenTabs();
	}

	private void RefreshSideScreenTabs()
	{
		int num = 1;
		for (int i = 0; i < this.sidescreenTabs.Length; i++)
		{
			DetailsScreen.SidescreenTab sidescreenTab = this.sidescreenTabs[i];
			bool flag = sidescreenTab.ValidateTarget(this.target);
			sidescreenTab.SetVisible(flag);
			sidescreenTab.SetSelected(this.selectedSidescreenTabID == sidescreenTab.type);
			num += (flag ? 1 : 0);
		}
		this.RefreshTitle();
		DetailsScreen.SidescreenTabTypes sidescreenTabTypes = this.selectedSidescreenTabID;
		if (sidescreenTabTypes != DetailsScreen.SidescreenTabTypes.Material)
		{
			if (sidescreenTabTypes == DetailsScreen.SidescreenTabTypes.Blueprints)
			{
				DetailsScreen.SidescreenTab tabOfType = this.GetTabOfType(DetailsScreen.SidescreenTabTypes.Blueprints);
				CosmeticsPanel reference = tabOfType.bodyInstance.GetComponent<HierarchyReferences>().GetReference<CosmeticsPanel>("CosmeticsPanel");
				reference.SetTarget(this.target);
				reference.Refresh();
				CosmeticsPanel reference2 = tabOfType.OverrideBody.GetComponent<HierarchyReferences>().GetReference<CosmeticsPanel>("CosmeticsPanel");
				LayoutRebuilder.ForceRebuildLayoutImmediate(reference2.GetComponent<RectTransform>());
				float num2 = Mathf.Min(384f, reference2.GetComponent<RectTransform>().sizeDelta.y + 16f);
				tabOfType.OverrideBody.GetComponent<LayoutElement>().minHeight = num2;
				tabOfType.OverrideBody.GetComponent<LayoutElement>().preferredHeight = num2;
			}
		}
		else
		{
			this.GetTabOfType(DetailsScreen.SidescreenTabTypes.Material).bodyInstance.GetComponentInChildren<DetailsScreenMaterialPanel>().SetTarget(this.target);
		}
		this.sidescreenTabHeader.SetActive(num > 1);
	}

	public KScreen SetSecondarySideScreen(KScreen secondaryPrefab, string title)
	{
		this.ClearSecondarySideScreen();
		if (this.instantiatedSecondarySideScreens.ContainsKey(secondaryPrefab))
		{
			this.activeSideScreen2 = this.instantiatedSecondarySideScreens[secondaryPrefab];
			this.activeSideScreen2.gameObject.SetActive(true);
		}
		else
		{
			this.activeSideScreen2 = KScreenManager.Instance.InstantiateScreen(secondaryPrefab.gameObject, this.sideScreen2ContentBody);
			this.activeSideScreen2.Activate();
			this.instantiatedSecondarySideScreens.Add(secondaryPrefab, this.activeSideScreen2);
		}
		this.sideScreen2Title.text = title;
		this.sideScreen2.SetActive(true);
		return this.activeSideScreen2;
	}

	public void ClearSecondarySideScreen()
	{
		if (this.activeSideScreen2 != null)
		{
			this.activeSideScreen2.gameObject.SetActive(false);
			this.activeSideScreen2 = null;
		}
		this.sideScreen2.SetActive(false);
	}

	public void DeactivateSideContent()
	{
		if (SideDetailsScreen.Instance != null && SideDetailsScreen.Instance.gameObject.activeInHierarchy)
		{
			SideDetailsScreen.Instance.Show(false);
		}
		if (this.sideScreens != null && this.sideScreens.Count > 0)
		{
			this.sideScreens.ForEach(delegate(DetailsScreen.SideScreenRef scn)
			{
				if (scn.screenInstance != null)
				{
					scn.screenInstance.ClearTarget();
					scn.screenInstance.Show(false);
				}
			});
		}
		DetailsScreen.SidescreenTab tabOfType = this.GetTabOfType(DetailsScreen.SidescreenTabTypes.Material);
		DetailsScreen.SidescreenTab tabOfType2 = this.GetTabOfType(DetailsScreen.SidescreenTabTypes.Blueprints);
		tabOfType.bodyInstance.GetComponentInChildren<DetailsScreenMaterialPanel>().SetTarget(null);
		tabOfType2.bodyInstance.GetComponentInChildren<CosmeticsPanel>().SetTarget(null);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MenuOpenHalfEffect, STOP_MODE.ALLOWFADEOUT);
		this.sideScreen.SetActive(false);
	}

	public void MaskSideContent(bool hide)
	{
		if (hide)
		{
			this.sideScreen.transform.localScale = Vector3.zero;
			return;
		}
		this.sideScreen.transform.localScale = Vector3.one;
	}

	public void DeselectAndClose()
	{
		if (base.gameObject.activeInHierarchy)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Back", false));
		}
		if (this.GetActiveTab() != null)
		{
			this.GetActiveTab().SetTarget(null);
		}
		SelectTool.Instance.Select(null, false);
		ClusterMapSelectTool.Instance.Select(null, false);
		if (this.target == null)
		{
			return;
		}
		this.target = null;
		this.previousTargetID = null;
		this.DeactivateSideContent();
		this.Show(false);
	}

	private void SortScreenOrder()
	{
		Array.Sort<DetailsScreen.Screens>(this.screens, (DetailsScreen.Screens x, DetailsScreen.Screens y) => x.displayOrderPriority.CompareTo(y.displayOrderPriority));
	}

	public void UpdatePortrait(GameObject target)
	{
		KSelectable component = target.GetComponent<KSelectable>();
		if (component == null)
		{
			return;
		}
		this.TabTitle.portrait.ClearPortrait();
		Building component2 = component.GetComponent<Building>();
		if (component2)
		{
			Sprite uisprite = component2.Def.GetUISprite("ui", false);
			if (uisprite != null)
			{
				this.TabTitle.portrait.SetPortrait(uisprite);
				return;
			}
		}
		if (target.GetComponent<MinionIdentity>())
		{
			this.TabTitle.SetPortrait(component.gameObject);
			return;
		}
		Edible component3 = target.GetComponent<Edible>();
		if (component3 != null)
		{
			Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(component3.GetComponent<KBatchedAnimController>().AnimFiles[0], "ui", false, "");
			this.TabTitle.portrait.SetPortrait(uispriteFromMultiObjectAnim);
			return;
		}
		PrimaryElement component4 = target.GetComponent<PrimaryElement>();
		if (component4 != null)
		{
			this.TabTitle.portrait.SetPortrait(Def.GetUISpriteFromMultiObjectAnim(ElementLoader.FindElementByHash(component4.ElementID).substance.anim, "ui", false, ""));
			return;
		}
		CellSelectionObject component5 = target.GetComponent<CellSelectionObject>();
		if (component5 != null)
		{
			string text = (component5.element.IsSolid ? "ui" : component5.element.substance.name);
			Sprite uispriteFromMultiObjectAnim2 = Def.GetUISpriteFromMultiObjectAnim(component5.element.substance.anim, text, false, "");
			this.TabTitle.portrait.SetPortrait(uispriteFromMultiObjectAnim2);
			return;
		}
		ICellSelectionProxy component6 = target.GetComponent<ICellSelectionProxy>();
		if (component6 != null && component6.Element != null && component6.Element.substance != null)
		{
			string text2 = (component6.Element.IsSolid ? "ui" : component6.Element.substance.name);
			Sprite uispriteFromMultiObjectAnim3 = Def.GetUISpriteFromMultiObjectAnim(component6.Element.substance.anim, text2, false, "");
			this.TabTitle.portrait.SetPortrait(uispriteFromMultiObjectAnim3);
			return;
		}
	}

	public bool CompareTargetWith(GameObject compare)
	{
		return this.target == compare;
	}

	public void UpdateTitle()
	{
		this.CodexEntryButton_Refresh();
		this.PinResourceButton_Refresh();
		this.TabTitle.SetTitle(this.target.GetProperName());
		if (this.TabTitle != null)
		{
			this.TabTitle.SetTitle(this.target.GetProperName());
			MinionIdentity minionIdentity = null;
			UserNameable userNameable = null;
			ClustercraftExteriorDoor clustercraftExteriorDoor = null;
			CommandModule commandModule = null;
			if (this.target != null)
			{
				minionIdentity = this.target.gameObject.GetComponent<MinionIdentity>();
				userNameable = this.target.gameObject.GetComponent<UserNameable>();
				clustercraftExteriorDoor = this.target.gameObject.GetComponent<ClustercraftExteriorDoor>();
				commandModule = this.target.gameObject.GetComponent<CommandModule>();
			}
			if (minionIdentity != null)
			{
				this.TabTitle.SetSubText(minionIdentity.GetComponent<MinionResume>().GetSkillsSubtitle(), "");
				this.TabTitle.SetUserEditable(true);
			}
			else if (userNameable != null)
			{
				this.TabTitle.SetSubText("", "");
				this.TabTitle.SetUserEditable(true);
			}
			else if (commandModule != null)
			{
				this.TrySetRocketTitle(commandModule);
				this.TabTitle.SetSubText(commandModule.GetComponent<BuildingComplete>().Def.Name, "");
			}
			else if (clustercraftExteriorDoor != null)
			{
				this.TrySetRocketTitle(clustercraftExteriorDoor);
				this.TabTitle.SetSubText(clustercraftExteriorDoor.GetComponent<BuildingComplete>().Def.Name, "");
			}
			else
			{
				this.TabTitle.SetSubText("", "");
				this.TabTitle.SetUserEditable(false);
			}
			this.TabTitle.UpdateRenameTooltip(this.target);
		}
	}

	private void TrySetRocketTitle(ClustercraftExteriorDoor clusterCraftDoor)
	{
		if (clusterCraftDoor.HasTargetWorld())
		{
			WorldContainer targetWorld = clusterCraftDoor.GetTargetWorld();
			this.TabTitle.SetTitle(targetWorld.GetComponent<ClusterGridEntity>().Name);
			this.TabTitle.SetUserEditable(true);
			clusterCraftDoor.GetComponent<KSelectable>().SetName(targetWorld.GetComponent<ClusterGridEntity>().Name);
			this.setRocketTitleHandle = -1;
			return;
		}
		if (this.setRocketTitleHandle == -1)
		{
			this.setRocketTitleHandle = this.target.Subscribe(-71801987, delegate(object clusterCraftDoor)
			{
				this.OnRefreshData(null);
				this.target.Unsubscribe(this.setRocketTitleHandle);
				this.setRocketTitleHandle = -1;
			});
		}
	}

	private void TrySetRocketTitle(CommandModule commandModule)
	{
		if (commandModule != null)
		{
			string rocketName = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(commandModule.GetComponent<LaunchConditionManager>()).GetRocketName();
			commandModule.GetComponent<KSelectable>().SetName(rocketName);
			this.TabTitle.SetTitle(rocketName);
			this.TabTitle.SetUserEditable(true);
		}
	}

	public TargetPanel GetActiveTab()
	{
		return this.tabHeader.ActivePanel;
	}

	[CompilerGenerated]
	internal static bool <PinResourceButton_TryGetResourceTagAndProperName>g__ShouldUse|51_0(Tag targetTag)
	{
		foreach (Tag tag in GameTags.MaterialCategories)
		{
			if (DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(tag).Contains(targetTag))
			{
				return true;
			}
		}
		foreach (Tag tag2 in GameTags.CalorieCategories)
		{
			if (DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(tag2).Contains(targetTag))
			{
				return true;
			}
		}
		foreach (Tag tag3 in GameTags.UnitCategories)
		{
			if (DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(tag3).Contains(targetTag))
			{
				return true;
			}
		}
		return false;
	}

	public static DetailsScreen Instance;

	[SerializeField]
	private KButton CodexEntryButton;

	[SerializeField]
	private KButton PinResourceButton;

	[Header("Panels")]
	public Transform UserMenuPanel;

	[Header("Name Editing (disabled)")]
	[SerializeField]
	private KButton CloseButton;

	[Header("Tabs")]
	[SerializeField]
	private DetailTabHeader tabHeader;

	[SerializeField]
	private EditableTitleBar TabTitle;

	[SerializeField]
	private DetailsScreen.Screens[] screens;

	[SerializeField]
	private GameObject tabHeaderContainer;

	[Header("Side Screen Tabs")]
	[SerializeField]
	private DetailsScreen.SidescreenTab[] sidescreenTabs;

	[SerializeField]
	private GameObject sidescreenTabHeader;

	[SerializeField]
	private GameObject original_tab;

	[SerializeField]
	private GameObject original_tab_body;

	[Header("Side Screens")]
	[SerializeField]
	private GameObject sideScreen;

	[SerializeField]
	private List<DetailsScreen.SideScreenRef> sideScreens;

	[SerializeField]
	private LayoutElement tabBodyLayoutElement;

	[Header("Secondary Side Screens")]
	[SerializeField]
	private GameObject sideScreen2ContentBody;

	[SerializeField]
	private GameObject sideScreen2;

	[SerializeField]
	private LocText sideScreen2Title;

	private KScreen activeSideScreen2;

	private Tag previousTargetID = null;

	private bool HasActivated;

	private DetailsScreen.SidescreenTabTypes selectedSidescreenTabID;

	private Dictionary<KScreen, KScreen> instantiatedSecondarySideScreens = new Dictionary<KScreen, KScreen>();

	private static readonly EventSystem.IntraObjectHandler<DetailsScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<DetailsScreen>(delegate(DetailsScreen component, object data)
	{
		component.OnRefreshData(data);
	});

	private List<KeyValuePair<DetailsScreen.SideScreenRef, int>> sortedSideScreens = new List<KeyValuePair<DetailsScreen.SideScreenRef, int>>();

	private int setRocketTitleHandle = -1;

	[Serializable]
	private struct Screens
	{
		public string name;

		public string displayName;

		public string tooltip;

		public Sprite icon;

		public TargetPanel screen;

		public int displayOrderPriority;

		public bool hideWhenDead;

		public HashedString focusInViewMode;

		[HideInInspector]
		public int tabIdx;
	}

	public enum SidescreenTabTypes
	{
		Config,
		Errands,
		Material,
		Blueprints
	}

	[Serializable]
	public class SidescreenTab
	{
		private void OnTabClicked()
		{
			global::System.Action onClicked = this.OnClicked;
			if (onClicked == null)
			{
				return;
			}
			onClicked();
		}

		public bool IsVisible { get; private set; }

		public bool IsSelected { get; private set; }

		public void Initiate(GameObject originalTabInstance, GameObject originalBodyInstance, Action<DetailsScreen.SidescreenTab> on_tab_clicked_callback)
		{
			if (on_tab_clicked_callback != null)
			{
				this.OnClicked = delegate
				{
					on_tab_clicked_callback(this);
				};
			}
			originalBodyInstance.gameObject.SetActive(false);
			if (this.OverrideBody == null)
			{
				this.bodyInstance = global::UnityEngine.Object.Instantiate<GameObject>(originalBodyInstance);
				this.bodyInstance.name = this.type.ToString() + " Tab - body instance";
				this.bodyInstance.SetActive(true);
				this.bodyInstance.transform.SetParent(originalBodyInstance.transform.parent, false);
			}
			else
			{
				this.bodyInstance = this.OverrideBody;
			}
			this.bodyReferences = this.bodyInstance.GetComponent<HierarchyReferences>();
			originalTabInstance.gameObject.SetActive(false);
			if (this.tabInstance == null)
			{
				this.tabInstance = global::UnityEngine.Object.Instantiate<GameObject>(originalTabInstance.gameObject).GetComponent<MultiToggle>();
				this.tabInstance.name = this.type.ToString() + " Tab Instance";
				this.tabInstance.gameObject.SetActive(true);
				this.tabInstance.transform.SetParent(originalTabInstance.transform.parent, false);
				MultiToggle multiToggle = this.tabInstance;
				multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(this.OnTabClicked));
				HierarchyReferences component = this.tabInstance.GetComponent<HierarchyReferences>();
				component.GetReference<LocText>("label").SetText(Strings.Get(this.Title_Key));
				component.GetReference<Image>("icon").sprite = this.Icon;
				this.tabInstance.GetComponent<ToolTip>().SetSimpleTooltip(Strings.Get(this.Tooltip_Key));
			}
		}

		public void SetSelected(bool isSelected)
		{
			this.IsSelected = isSelected;
			this.tabInstance.ChangeState(isSelected ? 1 : 0);
			this.bodyInstance.SetActive(isSelected);
		}

		public void SetTitle(string title)
		{
			if (this.bodyReferences != null && this.bodyReferences.HasReference("TitleLabel"))
			{
				this.bodyReferences.GetReference<LocText>("TitleLabel").SetText(title);
			}
		}

		public void SetTitleVisibility(bool visible)
		{
			if (this.bodyReferences != null && this.bodyReferences.HasReference("Title"))
			{
				Component reference = this.bodyReferences.GetReference("Title");
				reference.gameObject.SetActive(visible);
				reference.transform.parent.GetComponent<LayoutGroup>().padding.top = (visible ? 24 : 0);
			}
		}

		public void SetNoConfigMessageVisibility(bool visible)
		{
			if (this.bodyReferences != null && this.bodyReferences.HasReference("NoConfigMessage"))
			{
				this.bodyReferences.GetReference("NoConfigMessage").gameObject.SetActive(visible);
			}
		}

		public void RepositionTitle()
		{
			if (this.bodyReferences != null && this.bodyReferences.GetReference("Title") != null)
			{
				this.bodyReferences.GetReference("Title").transform.SetSiblingIndex(0);
			}
		}

		public void SetVisible(bool visible)
		{
			this.IsVisible = visible;
			this.tabInstance.gameObject.SetActive(visible);
			this.bodyInstance.SetActive(this.IsSelected && this.IsVisible);
		}

		public bool ValidateTarget(GameObject target)
		{
			return !(target == null) && (this.ValidateTargetCallback == null || this.ValidateTargetCallback(target, this));
		}

		public DetailsScreen.SidescreenTabTypes type;

		public string Title_Key;

		public string Tooltip_Key;

		public Sprite Icon;

		public GameObject OverrideBody;

		public Func<GameObject, DetailsScreen.SidescreenTab, bool> ValidateTargetCallback;

		public global::System.Action OnClicked;

		[NonSerialized]
		public MultiToggle tabInstance;

		[NonSerialized]
		public GameObject bodyInstance;

		private HierarchyReferences bodyReferences;

		private const string bodyRef_Title = "Title";

		private const string bodyRef_TitleLabel = "TitleLabel";

		private const string bodyRef_NoConfigMessage = "NoConfigMessage";
	}

	[Serializable]
	public class SideScreenRef
	{
		public string name;

		public SideScreenContent screenPrefab;

		public Vector2 offset;

		public DetailsScreen.SidescreenTabTypes tab;

		[HideInInspector]
		public SideScreenContent screenInstance;
	}
}
