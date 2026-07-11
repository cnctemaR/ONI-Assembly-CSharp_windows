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

	public override float GetSortKey()
	{
		if (this.isEditing)
		{
			return 10f;
		}
		return base.GetSortKey();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.SortScreenOrder();
		this.ConsumeMouseScroll = true;
		DetailsScreen.Instance = this;
		UIRegistry.detailsScreen = this;
		this.DeactivateSideContent();
		base.Show(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.CodexEntryButton.onClick += this.OpenCodexEntry;
		this.CloseButton.onClick += this.DeselectAndClose;
		this.TabTitle.OnNameChanged += this.OnNameChanged;
		this.TabTitle.OnStartedEditing += this.OnStartedEditing;
		base.Subscribe(-1514841199, new Action<object>(this.OnRefreshData));
	}

	private void OnStartedEditing()
	{
		this.isEditing = true;
		KScreenManager.Instance.RefreshStack();
	}

	private void OnNameChanged(string newName)
	{
		this.isEditing = false;
		if (string.IsNullOrEmpty(newName))
		{
			return;
		}
		MinionIdentity component = this.target.GetComponent<MinionIdentity>();
		if (component == null)
		{
			return;
		}
		component.SetName(newName);
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

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.isEditing)
		{
			e.Consumed = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!this.isEditing && this.target != null && PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
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
		this.CodexEntryButton.isInteractable = selectedObjectCodexID != string.Empty;
		this.CodexEntryButton.GetComponent<ToolTip>().SetSimpleTooltip((!this.CodexEntryButton.isInteractable) ? UI.TOOLTIPS.NO_CODEX_ENTRY : UI.TOOLTIPS.OPEN_CODEX_ENTRY);
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
			string requiredComponentType = this.screens[j].requiredComponentType;
			bool flag = requiredComponentType == null || requiredComponentType == string.Empty || DetailsScreen.GetComponent(go, requiredComponentType) != null;
			if (flag && requiredComponentType == "Storage")
			{
				flag = go.GetComponent<Storage>().showInUI;
			}
			bool flag2 = false;
			for (int k = 0; k < this.screens[j].excludeComponentType.Length; k++)
			{
				string text = this.screens[j].excludeComponentType[k];
				if (text != null && DetailsScreen.GetComponent(go, text) != null)
				{
					flag2 = true;
					break;
				}
			}
			bool flag3 = this.screens[j].hideWhenDead && base.gameObject.HasTag(GameTags.Dead);
			base.SetTabEnabled(this.screens[j].tabIdx, flag && !flag2 && !flag3);
			if (flag)
			{
				num2++;
				if (num == -1)
				{
					if (SimDebugView.Instance.GetMode() != SimViewMode.None)
					{
						if (SimDebugView.Instance.GetMode() == this.screens[j].focusInViewMode)
						{
							num = j;
						}
					}
					else
					{
						num = j;
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
				scn.screenInstance.transform.SetAsFirstSibling();
				scn.screenInstance.SetTarget(this.target);
				scn.screenInstance.Show(true);
				this.currentSideScreen = scn.screenInstance;
				this.RefreshTitle();
			});
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
		}
		else
		{
			this.sideScreen.transform.localScale = Vector3.one;
		}
	}

	private string GetSelectedObjectCodexID()
	{
		string text = string.Empty;
		CellSelectionObject component = SelectTool.Instance.selected.GetComponent<CellSelectionObject>();
		BuildingUnderConstruction component2 = SelectTool.Instance.selected.GetComponent<BuildingUnderConstruction>();
		CreatureBrain component3 = SelectTool.Instance.selected.GetComponent<CreatureBrain>();
		PlantableSeed component4 = SelectTool.Instance.selected.GetComponent<PlantableSeed>();
		if (component != null)
		{
			text = CodexCache.FormatLinkID(component.element.id.ToString());
		}
		else if (component2 != null)
		{
			text = CodexCache.FormatLinkID(component2.Def.PrefabID);
		}
		else if (component3 != null)
		{
			text = CodexCache.FormatLinkID(SelectTool.Instance.selected.PrefabID().ToString());
			text = text.Replace("BABY", string.Empty);
		}
		else if (component4 != null)
		{
			text = CodexCache.FormatLinkID(SelectTool.Instance.selected.PrefabID().ToString());
			text = text.Replace("SEED", string.Empty);
		}
		else
		{
			text = CodexCache.FormatLinkID(SelectTool.Instance.selected.PrefabID().ToString());
		}
		if (CodexCache.entries.ContainsKey(text) || CodexCache.FindSubEntry(text) != null)
		{
			return text;
		}
		return string.Empty;
	}

	public void OpenCodexEntry()
	{
		string selectedObjectCodexID = this.GetSelectedObjectCodexID();
		if (selectedObjectCodexID != string.Empty)
		{
			ManagementMenu.Instance.OpenCodexToEntry(selectedObjectCodexID);
		}
	}

	public void DeselectAndClose()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Back", false));
		SelectTool.Instance.Select(null, false);
		if (this.target == null)
		{
			return;
		}
		this.target = null;
		this.DeactivateSideContent();
		base.Show(false);
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
		MinionIdentity component3 = target.GetComponent<MinionIdentity>();
		if (component3)
		{
			this.TabTitle.SetPortrait(component.gameObject);
			return;
		}
		Edible component4 = target.GetComponent<Edible>();
		if (component4 != null)
		{
			KBatchedAnimController component5 = component4.GetComponent<KBatchedAnimController>();
			Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(component5.AnimFiles[0], "ui", false);
			this.TabTitle.portrait.SetPortrait(uispriteFromMultiObjectAnim);
			return;
		}
		PrimaryElement component6 = target.GetComponent<PrimaryElement>();
		if (component6 != null)
		{
			this.TabTitle.portrait.SetPortrait(Def.GetUISpriteFromMultiObjectAnim(ElementLoader.FindElementByHash(component6.ElementID).substance.anim, "ui", false));
			return;
		}
		CellSelectionObject component7 = target.GetComponent<CellSelectionObject>();
		if (component7 != null)
		{
			string text = ((!component7.element.IsSolid) ? component7.element.substance.name : "ui");
			Sprite uispriteFromMultiObjectAnim2 = Def.GetUISpriteFromMultiObjectAnim(component7.element.substance.anim, text, false);
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
			if (this.target != null)
			{
				minionIdentity = this.target.gameObject.GetComponent<MinionIdentity>();
			}
			if (minionIdentity != null)
			{
				this.TabTitle.SetSubText(minionIdentity.GetComponent<MinionResume>().GetCurrentRoleString(), minionIdentity.GetComponent<MinionResume>().GetCurrentRoleDescription());
				this.TabTitle.SetUserEditable(true);
			}
			else
			{
				this.TabTitle.SetSubText(string.Empty, string.Empty);
				this.TabTitle.SetUserEditable(false);
			}
		}
	}

	public void SetTitle(string title)
	{
		this.TabTitle.SetTitle(title);
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

	private bool HasActivated;

	private bool isEditing;

	private SideScreenContent currentSideScreen;

	[Serializable]
	private struct Screens
	{
		public string name;

		public string displayName;

		public string tooltip;

		public Sprite icon;

		public TargetScreen screen;

		public string requiredComponentType;

		public string[] excludeComponentType;

		public Tag[] excludedPrefabTags;

		public int displayOrderPriority;

		public bool hideWhenDead;

		public SimViewMode focusInViewMode;

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
