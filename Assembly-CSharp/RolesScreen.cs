using System;
using System.Collections;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class RolesScreen : KModalScreen
{
	public GameObject GetSlotWidget(GameObject parent)
	{
		GameObject gameObject;
		if (this.freeWidgetSlots.Count > 0)
		{
			gameObject = this.freeWidgetSlots[0];
			this.freeWidgetSlots.Remove(gameObject);
			gameObject.transform.SetParent(parent.transform);
		}
		else
		{
			gameObject = Util.KInstantiateUI(this.Prefab_Slot, parent, true);
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	public void RecycleSlotWidget(GameObject slotWidget)
	{
		this.freeWidgetSlots.Add(slotWidget);
		slotWidget.SetActive(false);
		slotWidget.transform.SetParent(this.SlotWidgetPool.transform);
	}

	protected override void OnActivate()
	{
		this.ConsumeMouseScroll = true;
		base.OnActivate();
		this.RefreshAll(null);
		this.jobsTableScreen.ToggleColumnSortWidgets(this.expandedJobs);
		Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
		liveMinionIdentities.OnAdd = (Action<MinionIdentity>)Delegate.Combine(liveMinionIdentities.OnAdd, new Action<MinionIdentity>(this.MarkDirty));
		Components.Cmps<MinionIdentity> liveMinionIdentities2 = Components.LiveMinionIdentities;
		liveMinionIdentities2.OnRemove = (Action<MinionIdentity>)Delegate.Combine(liveMinionIdentities2.OnRemove, new Action<MinionIdentity>(this.MarkDirty));
		MultiToggle multiToggle = this.expandToggle;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			this.ToggleExpandedJobs(!this.expandedJobs, false);
		}));
		this.CloseButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
	}

	protected override void OnShow(bool show)
	{
		if (show)
		{
			this.RefreshAll(null);
			if (Components.RoleStations.Count == 0 && !DebugHandler.InstantBuildMode)
			{
				this.ToggleExpandedJobs(true, true);
				this.FGDisable.SetActive(true);
			}
			else
			{
				this.ToggleExpandedJobs(false, true);
				this.FGDisable.SetActive(false);
			}
		}
		else if (this.expandRoutine != null)
		{
			this.ToggleExpandedJobs(false, true);
		}
		base.OnShow(show);
	}

	private void RefreshAll(object eventData = null)
	{
		this.dirty = false;
		this.RefreshRoleWidgets();
		this.RefreshSideBar();
		this.linesPending = true;
	}

	private void ToggleExpandedJobs(bool open, bool instant = false)
	{
		this.expandedJobs = open;
		this.jobsTableScreen.ToggleColumnSortWidgets(this.expandedJobs);
		if (!instant)
		{
			this.expandRoutine = base.StartCoroutine(this.AnimateExpandJobs(open));
		}
		else
		{
			this.expandRoutine = null;
			HierarchyReferences component = base.GetComponent<HierarchyReferences>();
			this.screenDivideOffset = Mathf.RoundToInt((!open) ? this.jobs_closed_height : this.jobs_open_height);
			RectTransform rectTransform = component.GetReference("SideBar").rectTransform();
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (float)(-(float)this.screenDivideOffset));
			rectTransform = component.GetReference("RoleExplorer").rectTransform();
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (float)(-(float)this.screenDivideOffset));
			rectTransform = component.GetReference("JobsTableScreen").rectTransform();
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, (float)this.screenDivideOffset);
		}
		this.expandToggle.ChangeState((!open) ? 0 : 1);
	}

	protected IEnumerator AnimateExpandJobs(bool open)
	{
		HierarchyReferences refs = base.GetComponent<HierarchyReferences>();
		RectTransform rect = null;
		if (open)
		{
			for (float i = 0f; i < this.expand_transition_duration; i += Time.unscaledDeltaTime)
			{
				this.screenDivideOffset = Mathf.RoundToInt(Mathf.Lerp((!open) ? this.jobs_open_height : this.jobs_closed_height, (!open) ? this.jobs_closed_height : this.jobs_open_height, i / this.expand_transition_duration));
				rect = refs.GetReference("JobsTableScreen").rectTransform();
				rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, (float)this.screenDivideOffset);
				yield return 0;
			}
		}
		this.ToggleExpandedJobs(open, true);
		yield break;
	}

	public void MarkDirty(object eventData = null)
	{
		this.dirty = true;
	}

	private void Update()
	{
		if (this.dirty)
		{
			this.RefreshAll(null);
		}
		if (this.linesPending)
		{
			foreach (GameObject gameObject in this.roleWidgets.Values)
			{
				gameObject.GetComponent<RoleWidget>().RefreshLines();
			}
			this.linesPending = false;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed && (e.TryConsume(global::Action.MouseRight) || e.TryConsume(global::Action.Escape)))
		{
			ManagementMenu.Instance.CloseAll();
			return;
		}
		base.OnKeyDown(e);
	}

	public void RefreshRoleWidgets()
	{
		foreach (RoleConfig roleConfig in Game.Instance.roleManager.RolesConfigs)
		{
			if (!this.roleWidgets.ContainsKey(roleConfig.id))
			{
				while (Mathf.Max(roleConfig.tier, 6) >= this.tierColumns.Count)
				{
					GameObject gameObject = Util.KInstantiateUI(this.Prefab_tierColumn, this.tierTableHorizontalLayout, true);
					gameObject.GetComponent<VerticalLayoutGroup>().enabled = !this.layoutRoles;
					this.tierColumns.Add(gameObject);
					HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
					if (this.tierColumns.Count % 2 == 0)
					{
						component.GetReference("BG").gameObject.SetActive(false);
					}
					component.GetReference<LocText>("Label").text = RolesScreen.tierNames[this.tierColumns.Count - 1];
					component.GetReference<Image>("TierIcon").sprite = this.TierIcons[this.tierColumns.Count - 1];
					string text = "<b>" + RolesScreen.tierNames[this.tierColumns.Count] + "</b> \n\n";
					if (Expectations.ExpectationsByTier[this.tierColumns.Count - 1].Length == 0)
					{
						text = UI.ROLES_SCREEN.EXPECTATIONS.NO_EXPECTATIONS;
					}
					foreach (Expectation expectation in Expectations.ExpectationsByTier[this.tierColumns.Count - 1])
					{
						text = string.Concat(new string[] { "<b>", expectation.name, "</b>: ", expectation.description, "\n\n" });
						HierarchyReferences component2 = gameObject.GetComponent<HierarchyReferences>();
						GameObject gameObject2 = Util.KInstantiate(component2.GetReference("ExpectationPrefab").gameObject, component2.GetReference("ExpectationContainer").gameObject, expectation.name);
						gameObject2.SetActive(true);
						if (expectation is AttributeModifierExpectation)
						{
							gameObject2.GetComponentsInChildren<Image>()[1].sprite = (expectation as AttributeModifierExpectation).icon;
							gameObject2.GetComponentInChildren<LocText>().text = "+ " + (expectation as AttributeModifierExpectation).modifier.Value.ToString();
						}
						gameObject2.GetComponent<ToolTip>().SetSimpleTooltip(text);
					}
				}
				GameObject gameObject3 = Util.KInstantiateUI(this.Prefab_RoleWidget, this.tierColumns[roleConfig.tier], true);
				this.roleWidgets.Add(roleConfig.id, gameObject3);
			}
			this.roleWidgets[roleConfig.id].GetComponent<RoleWidget>().Refresh(roleConfig.id);
		}
		if (this.layoutRoles)
		{
			this.RefreshWidgetPositions();
		}
	}

	public void RefreshWidgetPositions()
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.roleWidgets)
		{
			keyValuePair.Value.rectTransform().anchoredPosition = Vector2.down * this.GetRowPosition(Game.Instance.roleManager.GetRowIndex(keyValuePair.Key));
		}
		this.linesPending = true;
	}

	public float GetRowPosition(int rowIndex)
	{
		float num = 32f;
		int num2 = 0;
		float num3 = 0f;
		for (int i = 1; i < rowIndex; i++)
		{
			num2 = 0;
			foreach (RoleConfig roleConfig in Game.Instance.roleManager.RolesConfigs)
			{
				if (Game.Instance.roleManager.GetRowIndex(roleConfig.id) == i)
				{
					num2 = Math.Max(num2, Game.Instance.roleManager.GetRoleAssignees(roleConfig.id).Count);
				}
			}
			num3 += Math.Max((float)this.layoutRowHeight, 72f + (float)num2 * num);
		}
		int num4 = 192;
		return num3 + (float)num4;
	}

	public RoleWidget GetRoleWidget(string roleID)
	{
		return this.roleWidgets[roleID].GetComponent<RoleWidget>();
	}

	public Vector2 GetRoleWidgetLineTargetPosition(string roleID)
	{
		return this.roleWidgets[roleID].GetComponent<RoleWidget>().lines_right.GetPosition();
	}

	public void RefreshSideBar()
	{
		this.roleWidgets["NoRole"].GetComponent<RoleWidget>().Refresh("NoRole");
	}

	private void minionDropEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		Image reference = entry.GetComponent<HierarchyReferences>().GetReference<Image>("SkillBox");
		Image reference2 = entry.GetComponent<HierarchyReferences>().GetReference<Image>("AptitudeBox");
		RoleConfig roleConfig = entry.entryData as RoleConfig;
		MinionResume minionResume = targetData as MinionResume;
		if (roleConfig == null)
		{
			reference.transform.parent.gameObject.SetActive(false);
			reference2.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			entry.tooltip.SetSimpleTooltip(Game.Instance.roleManager.RoleCriteriaString(roleConfig.id, minionResume));
			entry.button.isInteractable = Game.Instance.roleManager.CanAssignToRole(roleConfig.id, minionResume);
			reference.transform.parent.gameObject.SetActive(true);
			reference2.transform.parent.gameObject.SetActive(true);
			float num = 0f;
			for (int i = 0; i < roleConfig.relevantAttributes.Length; i++)
			{
				num += minionResume.GetAttributes().Get(roleConfig.relevantAttributes[i]).GetTotalDisplayValue();
			}
			float num2 = minionResume.AptitudeByRoleGroup[roleConfig.roleGroup];
			reference.transform.parent.GetComponentInChildren<LocText>(true).text = num.ToString();
			if (roleConfig.relevantAttributes.Length > 0)
			{
				reference.SetAlpha(GameUtil.AttributeSkillToAlpha(num));
			}
			else
			{
				reference.SetAlpha(0f);
			}
			if (roleConfig.relevantAttributes.Length > 0)
			{
				reference2.gameObject.SetActive(num2 > 0f);
			}
			else
			{
				reference2.gameObject.SetActive(false);
			}
		}
	}

	private int minionDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		RoleConfig roleConfig = a as RoleConfig;
		RoleConfig roleConfig2 = b as RoleConfig;
		MinionResume minionResume = targetData as MinionResume;
		bool flag = Game.Instance.roleManager.CanAssignToRole(roleConfig.id, minionResume);
		bool flag2 = Game.Instance.roleManager.CanAssignToRole(roleConfig2.id, minionResume);
		if (flag && !flag2)
		{
			return 1;
		}
		if (flag2 && !flag)
		{
			return -1;
		}
		float num = minionResume.AptitudeByRoleGroup[roleConfig.roleGroup];
		float num2 = minionResume.AptitudeByRoleGroup[roleConfig2.roleGroup];
		if (num != num2)
		{
			return (num <= num2) ? (-1) : 1;
		}
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < roleConfig.relevantAttributes.Length; i++)
		{
			num3 += minionResume.GetAttributes().Get(roleConfig.relevantAttributes[i]).GetTotalDisplayValue();
		}
		for (int j = 0; j < roleConfig2.relevantAttributes.Length; j++)
		{
			num4 += minionResume.GetAttributes().Get(roleConfig2.relevantAttributes[j]).GetTotalDisplayValue();
		}
		if (num3 > num4)
		{
			return 1;
		}
		if (num4 > num3)
		{
			return -1;
		}
		return 0;
	}

	private void OnMinionDropEntryClick(IListableOption role, object data)
	{
		if (role != null)
		{
			Game.Instance.roleManager.AssignToRole((role as RoleConfig).id, data as MinionResume, false);
			this.RefreshRoleWidgets();
			this.RefreshSideBar();
		}
	}

	private Dictionary<string, GameObject> roleWidgets = new Dictionary<string, GameObject>();

	private Dictionary<MinionIdentity, GameObject> minionWidgets = new Dictionary<MinionIdentity, GameObject>();

	private List<GameObject> tierColumns = new List<GameObject>();

	[SerializeField]
	private GameObject unassignedMinionList;

	[SerializeField]
	private GameObject assignedMinionList;

	[SerializeField]
	private GameObject tierTableHorizontalLayout;

	[SerializeField]
	private GameObject rootContent;

	[SerializeField]
	private GameObject Prefab_MinionWidget;

	[SerializeField]
	private GameObject Prefab_RoleWidget;

	[SerializeField]
	private GameObject Prefab_RoleSlot;

	[SerializeField]
	private GameObject Prefab_tierColumn;

	[SerializeField]
	private GameObject Prefab_Slot;

	[SerializeField]
	private KButton CloseButton;

	[SerializeField]
	private GameObject FGDisable;

	[SerializeField]
	private GameObject SlotWidgetPool;

	private List<GameObject> freeWidgetSlots = new List<GameObject>();

	private bool dirty;

	private bool layoutRoles = true;

	private bool linesPending;

	[HideInInspector]
	public MinionResume activeResume;

	public Sprite[] TierIcons;

	public static string[] tierNames = new string[]
	{
		UI.ROLES_SCREEN.TIER_NAMES.ZERO,
		UI.ROLES_SCREEN.TIER_NAMES.ONE,
		UI.ROLES_SCREEN.TIER_NAMES.TWO,
		UI.ROLES_SCREEN.TIER_NAMES.THREE,
		UI.ROLES_SCREEN.TIER_NAMES.FOUR,
		UI.ROLES_SCREEN.TIER_NAMES.FIVE,
		UI.ROLES_SCREEN.TIER_NAMES.SIX,
		UI.ROLES_SCREEN.TIER_NAMES.SEVEN
	};

	[SerializeField]
	private MultiToggle expandToggle;

	private int screenDivideOffset = -400;

	private int layoutRowHeight = 96;

	private bool expandedJobs;

	public JobsTableScreen jobsTableScreen;

	private Coroutine expandRoutine;

	private float expand_transition_duration = 0.125f;

	private float jobs_open_height = 400f;

	private float jobs_closed_height = 108f;
}
