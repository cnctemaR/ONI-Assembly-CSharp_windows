using System;
using System.Collections.Generic;
using FMOD.Studio;
using STRINGS;
using UnityEngine;

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
		UIRegistry.detailsScreen = this;
		this.DeactivateSideContent();
		this.Show(false);
		base.Subscribe(Game.Instance.gameObject, -1503271301, new Action<object>(this.OnSelectObject));
	}

	private void OnSelectObject(object data)
	{
		if (data == null)
		{
			this.previouslyActiveTab = -1;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.CodexEntryButton.onClick += this.OpenCodexEntry;
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
		if (component != null)
		{
			component.SetName(newName);
		}
		else if (component2 != null)
		{
			component2.SetName(newName);
		}
		this.TabTitle.UpdateRenameTooltip(this.target);
	}

	protected override void OnDeactivate()
	{
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

	private void UpdateCodexButton()
	{
		string selectedObjectCodexID = this.GetSelectedObjectCodexID();
		this.CodexEntryButton.isInteractable = selectedObjectCodexID != "";
		this.CodexEntryButton.GetComponent<ToolTip>().SetSimpleTooltip(this.CodexEntryButton.isInteractable ? UI.TOOLTIPS.OPEN_CODEX_ENTRY : UI.TOOLTIPS.NO_CODEX_ENTRY);
	}

	public void OnRefreshData(object obj)
	{
		this.SetTitle(base.PreviousActiveTab);
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
		this.target = go;
		this.sortedSideScreens.Clear();
		CellSelectionObject component = this.target.GetComponent<CellSelectionObject>();
		if (component)
		{
			component.OnObjectSelected(null);
		}
		if (!this.HasActivated)
		{
			if (this.screens != null)
			{
				for (int i = 0; i < this.screens.Length; i++)
				{
					GameObject gameObject = KScreenManager.Instance.InstantiateScreen(this.screens[i].screen.gameObject, this.body.gameObject).gameObject;
					this.screens[i].screen = gameObject.GetComponent<TargetScreen>();
					this.screens[i].tabIdx = base.AddTab(this.screens[i].icon, Strings.Get(this.screens[i].displayName), this.screens[i].screen, Strings.Get(this.screens[i].tooltip));
				}
			}
			base.onTabActivated += this.OnTabActivated;
			this.HasActivated = true;
		}
		int num = -1;
		int num2 = 0;
		for (int j = 0; j < this.screens.Length; j++)
		{
			bool flag = this.screens[j].screen.IsValidForTarget(go);
			bool flag2 = this.screens[j].hideWhenDead && base.gameObject.HasTag(GameTags.Dead);
			bool flag3 = flag && !flag2;
			base.SetTabEnabled(this.screens[j].tabIdx, flag3);
			if (flag3)
			{
				num2++;
				if (num == -1)
				{
					if (SimDebugView.Instance.GetMode() != OverlayModes.None.ID)
					{
						if (SimDebugView.Instance.GetMode() == this.screens[j].focusInViewMode)
						{
							num = j;
						}
					}
					else if (flag3 && this.previouslyActiveTab >= 0 && this.previouslyActiveTab < this.screens.Length && this.screens[j].name == this.screens[this.previouslyActiveTab].name)
					{
						num = this.screens[j].tabIdx;
					}
				}
			}
		}
		if (num != -1)
		{
			this.ActivateTab(num);
		}
		else
		{
			this.ActivateTab(0);
		}
		this.tabHeaderContainer.gameObject.SetActive(base.CountTabs() > 1);
		if (this.sideScreens != null && this.sideScreens.Count > 0)
		{
			this.sideScreens.ForEach(delegate(DetailsScreen.SideScreenRef scn)
			{
				if (!scn.screenPrefab.IsValidForTarget(this.target))
				{
					if (scn.screenInstance != null && scn.screenInstance.gameObject.activeSelf)
					{
						scn.screenInstance.gameObject.SetActive(false);
					}
					return;
				}
				if (scn.screenInstance == null)
				{
					scn.screenInstance = global::Util.KInstantiateUI<SideScreenContent>(scn.screenPrefab.gameObject, this.sideScreenContentBody, false);
				}
				if (!this.sideScreen.activeInHierarchy)
				{
					this.sideScreen.SetActive(true);
				}
				scn.screenInstance.SetTarget(this.target);
				scn.screenInstance.Show(true);
				int sideScreenSortOrder = scn.screenInstance.GetSideScreenSortOrder();
				this.sortedSideScreens.Add(new KeyValuePair<GameObject, int>(scn.screenInstance.gameObject, sideScreenSortOrder));
				if (this.currentSideScreen == null || !this.currentSideScreen.gameObject.activeSelf || sideScreenSortOrder > this.sortedSideScreens.Find((KeyValuePair<GameObject, int> match) => match.Key == this.currentSideScreen.gameObject).Value)
				{
					this.currentSideScreen = scn.screenInstance;
				}
				this.RefreshTitle();
			});
		}
		this.sortedSideScreens.Sort(delegate(KeyValuePair<GameObject, int> x, KeyValuePair<GameObject, int> y)
		{
			if (x.Value <= y.Value)
			{
				return 1;
			}
			return -1;
		});
		for (int k = 0; k < this.sortedSideScreens.Count; k++)
		{
			this.sortedSideScreens[k].Key.transform.SetSiblingIndex(k);
		}
	}

	public void RefreshTitle()
	{
		if (this.currentSideScreen)
		{
			this.sideScreenTitle.SetText(this.currentSideScreen.GetTitle());
		}
	}

	private void OnTabActivated(int newTab, int oldTab)
	{
		this.SetTitle(newTab);
		if (oldTab != -1)
		{
			this.screens[oldTab].screen.SetTarget(null);
		}
		if (newTab != -1)
		{
			this.screens[newTab].screen.SetTarget(this.target);
		}
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

	private string GetSelectedObjectCodexID()
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

	public void OpenCodexEntry()
	{
		string selectedObjectCodexID = this.GetSelectedObjectCodexID();
		if (selectedObjectCodexID != "")
		{
			ManagementMenu.Instance.OpenCodexToEntry(selectedObjectCodexID);
		}
	}

	public void DeselectAndClose()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Back", false));
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

	public void SetTitle(int selectedTabIndex)
	{
		this.UpdateCodexButton();
		if (this.TabTitle != null)
		{
			this.TabTitle.SetTitle(this.target.GetProperName());
			MinionIdentity minionIdentity = null;
			UserNameable userNameable = null;
			if (this.target != null)
			{
				minionIdentity = this.target.gameObject.GetComponent<MinionIdentity>();
				userNameable = this.target.gameObject.GetComponent<UserNameable>();
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
			else
			{
				this.TabTitle.SetSubText("", "");
				this.TabTitle.SetUserEditable(false);
			}
			this.TabTitle.UpdateRenameTooltip(this.target);
		}
	}

	public void SetTitle(string title)
	{
		this.TabTitle.SetTitle(title);
	}

	public TargetScreen GetActiveTab()
	{
		if (this.previouslyActiveTab >= 0 && this.previouslyActiveTab < this.screens.Length)
		{
			return this.screens[this.previouslyActiveTab].screen;
		}
		return null;
	}

	public static DetailsScreen Instance;

	[SerializeField]
	private KButton CodexEntryButton;

	[Header("Panels")]
	public Transform UserMenuPanel;

	[Header("Name Editing (disabled)")]
	[SerializeField]
	private KButton CloseButton;

	[Header("Tabs")]
	[SerializeField]
	private EditableTitleBar TabTitle;

	[SerializeField]
	private DetailsScreen.Screens[] screens;

	[SerializeField]
	private GameObject tabHeaderContainer;

	[Header("Side Screens")]
	[SerializeField]
	private GameObject sideScreenContentBody;

	[SerializeField]
	private GameObject sideScreen;

	[SerializeField]
	private LocText sideScreenTitle;

	[SerializeField]
	private List<DetailsScreen.SideScreenRef> sideScreens;

	[Header("Secondary Side Screens")]
	[SerializeField]
	private GameObject sideScreen2ContentBody;

	[SerializeField]
	private GameObject sideScreen2;

	[SerializeField]
	private LocText sideScreen2Title;

	private KScreen activeSideScreen2;

	private bool HasActivated;

	private SideScreenContent currentSideScreen;

	private Dictionary<KScreen, KScreen> instantiatedSecondarySideScreens = new Dictionary<KScreen, KScreen>();

	private static readonly EventSystem.IntraObjectHandler<DetailsScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<DetailsScreen>(delegate(DetailsScreen component, object data)
	{
		component.OnRefreshData(data);
	});

	private List<KeyValuePair<GameObject, int>> sortedSideScreens = new List<KeyValuePair<GameObject, int>>();

	[Serializable]
	private struct Screens
	{
		public string name;

		public string displayName;

		public string tooltip;

		public Sprite icon;

		public TargetScreen screen;

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
