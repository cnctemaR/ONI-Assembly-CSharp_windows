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
		this.DeactivateSideContent();
		this.Show(false);
		base.Subscribe(Game.Instance.gameObject, -1503271301, new Action<object>(this.OnSelectObject));
		this.tabHeader.Init();
	}

	private void OnSelectObject(object data)
	{
		if (data == null)
		{
			this.previouslyActiveTab = -1;
			this.SelectSideScreenTab("sidescreen_config", false);
			return;
		}
		KPrefabID component = ((GameObject)data).GetComponent<KPrefabID>();
		if (component == null || this.previousTargetID != component.PrefabID())
		{
			this.SelectSideScreenTab("sidescreen_config", false);
			return;
		}
		this.SelectSideScreenTab(this.selectedSidescreenTabID, true);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.CodexEntryButton.onClick += this.CodexEntryButton_OnClick;
		this.PinResourceButton.onClick += this.PinResourceButton_OnClick;
		this.CloseButton.onClick += this.DeselectAndClose;
		this.TabTitle.OnNameChanged += this.OnNameChanged;
		this.TabTitle.OnStartedEditing += this.OnStartedEditing;
		MultiToggle multiToggle = this.sidescreenConfigTab;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			this.SelectSideScreenTab("sidescreen_config", true);
		}));
		MultiToggle multiToggle2 = this.sidescreenMaterialTab;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(delegate
		{
			this.SelectSideScreenTab("sidescreen_material", true);
		}));
		MultiToggle multiToggle3 = this.sidescreenSkinTab;
		multiToggle3.onClick = (global::System.Action)Delegate.Combine(multiToggle3.onClick, new global::System.Action(delegate
		{
			this.SelectSideScreenTab("sidescreen_skin", true);
		}));
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
		string text = "";
		global::Debug.Assert(this.target != null, "Details Screen has no target");
		KSelectable component = this.target.GetComponent<KSelectable>();
		DebugUtil.AssertArgs(component != null, new object[] { "Details Screen target is not a KSelectable", this.target });
		CellSelectionObject component2 = component.GetComponent<CellSelectionObject>();
		BuildingUnderConstruction component3 = component.GetComponent<BuildingUnderConstruction>();
		CreatureBrain component4 = component.GetComponent<CreatureBrain>();
		PlantableSeed component5 = component.GetComponent<PlantableSeed>();
		BudUprootedMonitor component6 = component.GetComponent<BudUprootedMonitor>();
		if (component2 != null)
		{
			text = CodexCache.FormatLinkID(component2.element.id.ToString());
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
			text = CodexCache.FormatLinkID(component.PrefabID().ToString());
			text = text.Replace("SEED", "");
		}
		else if (component6 != null)
		{
			if (component6.parentObject.Get() != null)
			{
				text = CodexCache.FormatLinkID(component6.parentObject.Get().PrefabID().ToString());
			}
			else if (component6.GetComponent<TreeBud>() != null)
			{
				text = CodexCache.FormatLinkID(component6.GetComponent<TreeBud>().buddingTrunk.Get().PrefabID().ToString());
			}
		}
		else
		{
			text = CodexCache.FormatLinkID(component.PrefabID().ToString());
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
		if (component != null && DetailsScreen.<PinResourceButton_TryGetResourceTagAndProperName>g__ShouldUse|56_0(component.PrefabTag))
		{
			targetTag = component.PrefabTag;
			targetProperName = component.GetProperName();
			return true;
		}
		CellSelectionObject component2 = this.target.GetComponent<CellSelectionObject>();
		if (component2 != null && DetailsScreen.<PinResourceButton_TryGetResourceTagAndProperName>g__ShouldUse|56_0(component2.element.tag))
		{
			targetTag = component2.element.tag;
			targetProperName = component2.GetProperName();
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
				text2 = GameUtil.GetFormattedCalories(RationTracker.Get().CountRationsByFoodType(tag.Name, ClusterManager.Instance.activeWorld.worldInventory, true), GameUtil.TimeSlice.None, true);
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
			if (this.target != null && this.target.GetComponent<KPrefabID>() != null)
			{
				this.previousTargetID = this.target.GetComponent<KPrefabID>().PrefabID();
			}
		}
		this.target = go;
		this.sortedSideScreens.Clear();
		CellSelectionObject component = this.target.GetComponent<CellSelectionObject>();
		if (component)
		{
			component.OnObjectSelected(null);
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
						sideScreenRef.screenInstance = global::Util.KInstantiateUI<SideScreenContent>(sideScreenRef.screenPrefab.gameObject, this.sideScreenConfigContentBody, false);
					}
					if (!this.sideScreen.activeSelf)
					{
						this.sideScreen.SetActive(true);
					}
					sideScreenRef.screenInstance.SetTarget(this.target);
					sideScreenRef.screenInstance.Show(true);
					int sideScreenSortOrder = sideScreenRef.screenInstance.GetSideScreenSortOrder();
					this.sortedSideScreens.Add(new KeyValuePair<GameObject, int>(sideScreenRef.screenInstance.gameObject, sideScreenSortOrder));
					if (this.currentSideScreen == null || !this.currentSideScreen.gameObject.activeSelf || sideScreenSortOrder > this.sortedSideScreens.Find((KeyValuePair<GameObject, int> match) => match.Key == this.currentSideScreen.gameObject).Value)
					{
						this.currentSideScreen = sideScreenRef.screenInstance;
					}
					this.RefreshTitle();
				}
			}
			if (!flag)
			{
				if (this.target.GetComponent<MinionIdentity>() == null && (this.target.GetComponent<Reconstructable>() == null || !this.target.GetComponent<Reconstructable>().AllowReconstruct) && this.target.GetComponent<BuildingFacade>() == null)
				{
					this.noConfigSideScreen.SetActive(false);
					this.sideScreen.SetActive(false);
				}
				else
				{
					this.noConfigSideScreen.SetActive(true);
					this.sideScreenTitleLabel.SetText(UI.UISIDESCREENS.NOCONFIG.TITLE);
					this.sideScreen.SetActive(true);
				}
			}
			else
			{
				this.noConfigSideScreen.SetActive(false);
			}
		}
		this.sortedSideScreens.Sort(delegate(KeyValuePair<GameObject, int> x, KeyValuePair<GameObject, int> y)
		{
			if (x.Value <= y.Value)
			{
				return 1;
			}
			return -1;
		});
		for (int i = 0; i < this.sortedSideScreens.Count; i++)
		{
			this.sortedSideScreens[i].Key.transform.SetSiblingIndex(i);
		}
	}

	public void RefreshTitle()
	{
		if (this.currentSideScreen)
		{
			this.sideScreenTitleLabel.SetText(this.currentSideScreen.GetTitle());
		}
	}

	private void SelectSideScreenTab(string tabID, bool considerPreviousMinLayoutHeight = true)
	{
		if (this.selectedSidescreenTabID == "sidescreen_config" && considerPreviousMinLayoutHeight)
		{
			this.tabBodyLayoutElement.minHeight = this.tabBodyLayoutElement.rectTransform().sizeDelta.y;
		}
		else if (tabID == "sidescreen_config")
		{
			this.tabBodyLayoutElement.minHeight = 0f;
		}
		this.selectedSidescreenTabID = tabID;
		this.RefreshSideScreenTabs();
	}

	private void RefreshSideScreenTabs()
	{
		string text = this.selectedSidescreenTabID;
		if (text != null)
		{
			if (!(text == "sidescreen_config"))
			{
				if (!(text == "sidescreen_material"))
				{
					if (text == "sidescreen_skin")
					{
						CosmeticsPanel reference = this.sideScreenSkinContentBody.GetComponent<HierarchyReferences>().GetReference<CosmeticsPanel>("CosmeticsPanel");
						reference.SetTarget(this.target);
						this.sidescreenConfigTab.ChangeState(0);
						this.sidescreenMaterialTab.ChangeState(0);
						this.sidescreenSkinTab.ChangeState(1);
						this.sideScreenConfigContentBody.SetActive(false);
						this.sideScreenMaterialContentBody.SetActive(false);
						this.sideScreenSkinContentBody.SetActive(true);
						this.sideScreenTitle.SetActive(false);
						reference.Refresh();
					}
				}
				else
				{
					this.sidescreenConfigTab.ChangeState(0);
					this.sidescreenMaterialTab.ChangeState(1);
					this.sidescreenSkinTab.ChangeState(0);
					this.sideScreenConfigContentBody.SetActive(false);
					this.sideScreenMaterialContentBody.SetActive(true);
					this.sideScreenSkinContentBody.SetActive(false);
					this.sideScreenTitle.SetActive(false);
					this.sideScreenMaterialContentBody.GetComponentInChildren<DetailsScreenMaterialPanel>().SetTarget(this.target);
				}
			}
			else
			{
				this.sidescreenConfigTab.ChangeState(1);
				this.sidescreenMaterialTab.ChangeState(0);
				this.sidescreenSkinTab.ChangeState(0);
				this.sideScreenConfigContentBody.SetActive(true);
				this.sideScreenMaterialContentBody.SetActive(false);
				this.sideScreenSkinContentBody.SetActive(false);
				this.sideScreenTitle.SetActive(true);
			}
		}
		int num = 1;
		if (!this.target.IsNullOrDestroyed())
		{
			if (this.target.GetComponent<Reconstructable>() == null || !this.target.GetComponent<Reconstructable>().AllowReconstruct)
			{
				this.sidescreenMaterialTab.gameObject.SetActive(false);
			}
			else
			{
				this.sidescreenMaterialTab.gameObject.SetActive(true);
				num++;
			}
			MinionIdentity component = this.target.GetComponent<MinionIdentity>();
			if (component != null)
			{
				HierarchyReferences component2 = this.sidescreenConfigTab.GetComponent<HierarchyReferences>();
				component2.GetReference<LocText>("label").SetText(UI.UISIDESCREENS.MINIONTODOSIDESCREEN.NAME);
				component2.GetReference<Image>("icon").sprite = Assets.GetSprite("icon_display_screen_errands");
				this.sidescreenConfigTab.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP);
			}
			else
			{
				HierarchyReferences component3 = this.sidescreenConfigTab.GetComponent<HierarchyReferences>();
				component3.GetReference<LocText>("label").SetText(UI.DETAILTABS.CONFIGURATION.NAME);
				component3.GetReference<Image>("icon").sprite = Assets.GetSprite("icon_display_screen_config");
				this.sidescreenConfigTab.GetComponent<ToolTip>().SetSimpleTooltip(UI.DETAILTABS.CONFIGURATION.TOOLTIP);
			}
			if (component == null && this.target.GetComponent<BuildingFacade>() == null)
			{
				this.sidescreenSkinTab.gameObject.SetActive(false);
			}
			else
			{
				this.sidescreenSkinTab.gameObject.SetActive(true);
				num++;
			}
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
		this.sideScreenMaterialContentBody.GetComponentInChildren<DetailsScreenMaterialPanel>().SetTarget(null);
		this.sideScreenSkinContentBody.GetComponentInChildren<CosmeticsPanel>().SetTarget(null);
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
			}
			else if (clustercraftExteriorDoor != null)
			{
				this.TrySetRocketTitle(clustercraftExteriorDoor);
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
			this.TabTitle.SetSubText(this.target.GetProperName(), "");
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
			this.TabTitle.SetTitle(SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(commandModule.GetComponent<LaunchConditionManager>()).GetRocketName());
			this.TabTitle.SetUserEditable(true);
		}
		this.TabTitle.SetSubText(this.target.GetProperName(), "");
	}

	public TargetPanel GetActiveTab()
	{
		return this.tabHeader.ActivePanel;
	}

	[CompilerGenerated]
	internal static bool <PinResourceButton_TryGetResourceTagAndProperName>g__ShouldUse|56_0(Tag targetTag)
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
	private GameObject sidescreenTabHeader;

	[SerializeField]
	private MultiToggle sidescreenConfigTab;

	[SerializeField]
	private MultiToggle sidescreenMaterialTab;

	[SerializeField]
	private MultiToggle sidescreenSkinTab;

	private const string sidescreenConfigID = "sidescreen_config";

	private const string sidescreenMaterialID = "sidescreen_material";

	private const string sidescreenSkinID = "sidescreen_skin";

	[Header("Side Screens")]
	[SerializeField]
	private GameObject sideScreenConfigContentBody;

	[SerializeField]
	private GameObject sideScreenMaterialContentBody;

	[SerializeField]
	private GameObject sideScreenSkinContentBody;

	[SerializeField]
	private GameObject sideScreen;

	[SerializeField]
	private GameObject sideScreenTitle;

	[SerializeField]
	private LocText sideScreenTitleLabel;

	[SerializeField]
	private List<DetailsScreen.SideScreenRef> sideScreens;

	[SerializeField]
	private GameObject noConfigSideScreen;

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

	private string selectedSidescreenTabID = "sidescreen_config";

	private SideScreenContent currentSideScreen;

	private Dictionary<KScreen, KScreen> instantiatedSecondarySideScreens = new Dictionary<KScreen, KScreen>();

	private static readonly EventSystem.IntraObjectHandler<DetailsScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<DetailsScreen>(delegate(DetailsScreen component, object data)
	{
		component.OnRefreshData(data);
	});

	private List<KeyValuePair<GameObject, int>> sortedSideScreens = new List<KeyValuePair<GameObject, int>>();

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

	[Serializable]
	public class SideScreenRef
	{
		public string name;

		public SideScreenContent screenPrefab;

		public Vector2 offset;

		[HideInInspector]
		public SideScreenContent screenInstance;
	}
}
