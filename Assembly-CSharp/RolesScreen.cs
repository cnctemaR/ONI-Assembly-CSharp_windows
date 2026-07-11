using System;
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
		Components.LiveMinionIdentities.OnAdd += new Action<MinionIdentity>(this.MarkDirty);
		Components.LiveMinionIdentities.OnRemove += new Action<MinionIdentity>(this.MarkDirty);
		this.CloseButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		MultiToggle multiToggle = this.toggleAutoPrioritize;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(this.OnToggleAutoPrioritize));
		this.toggleAutoPrioritize.GetComponent<ToolTip>().OnToolTip = new Func<string>(this.OnHoverToggleAutoPrioritize);
	}

	protected override void OnShow(bool show)
	{
		if (show)
		{
			this.RefreshAll(null);
		}
		base.OnShow(show);
	}

	private void RefreshAll(object eventData = null)
	{
		this.dirty = false;
		this.RefreshRoleWidgets();
		this.RefreshSideBar();
		this.linesPending = true;
		this.toggleAutoPrioritize.ChangeState((!Game.Instance.autoPrioritizeRoles) ? 0 : 1);
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
		float num = 0f;
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.roleWidgets)
		{
			float rowPosition = this.GetRowPosition(Game.Instance.roleManager.GetRowIndex(keyValuePair.Key));
			num = Mathf.Max(rowPosition, num);
			keyValuePair.Value.rectTransform().anchoredPosition = Vector2.down * rowPosition;
		}
		num = Mathf.Max(num, this.GetRowHeight(0) + (float)this.HEADER_HEIGHT);
		float rowHeight = this.GetRowHeight(Game.Instance.roleManager.NumberOfRows);
		foreach (GameObject gameObject in this.tierColumns)
		{
			gameObject.GetComponent<LayoutElement>().minHeight = num + rowHeight;
		}
		this.linesPending = true;
	}

	public float GetRowPosition(int rowIndex)
	{
		float num = 0f;
		for (int i = 1; i < rowIndex; i++)
		{
			num += this.GetRowHeight(i);
		}
		return num + (float)this.HEADER_HEIGHT;
	}

	public float GetRowHeight(int rowIndex)
	{
		float num = 32f;
		int num2 = 0;
		foreach (RoleConfig roleConfig in Game.Instance.roleManager.RolesConfigs)
		{
			if (Game.Instance.roleManager.GetRowIndex(roleConfig.id) == rowIndex)
			{
				num2 = Math.Max(num2, Game.Instance.roleManager.GetRoleAssignees(roleConfig.id).Count);
			}
		}
		return Math.Max((float)this.layoutRowHeight, 72f + (float)num2 * num);
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
			Game.Instance.roleManager.AssignToRole((role as RoleConfig).id, data as MinionResume, false, false);
			this.RefreshRoleWidgets();
			this.RefreshSideBar();
		}
	}

	private void OnToggleAutoPrioritize()
	{
		Game.Instance.autoPrioritizeRoles = !Game.Instance.autoPrioritizeRoles;
		this.toggleAutoPrioritize.ChangeState((!Game.Instance.autoPrioritizeRoles) ? 0 : 1);
		this.toggleAutoPrioritize.GetComponent<ToolTip>().forceRefresh = true;
	}

	private string OnHoverToggleAutoPrioritize()
	{
		return (!Game.Instance.autoPrioritizeRoles) ? UI.ROLES_SCREEN.AUTO_PRIORITIZE_DISABLED : UI.ROLES_SCREEN.AUTO_PRIORITIZE_ENABLED;
	}

	public new const float SCREEN_SORT_KEY = 101f;

	private Dictionary<string, GameObject> roleWidgets = new Dictionary<string, GameObject>();

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
	private GameObject SlotWidgetPool;

	[SerializeField]
	private MultiToggle toggleAutoPrioritize;

	private List<GameObject> freeWidgetSlots = new List<GameObject>();

	private int HEADER_HEIGHT = 192;

	private bool dirty;

	private bool layoutRoles = true;

	private bool linesPending;

	[HideInInspector]
	public MinionResume activeResume;

	public Sprite[] TierIcons;

	public static readonly string[] tierNames = new string[]
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

	private int layoutRowHeight = 96;

	private Coroutine expandRoutine;
}
