using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class AssignableSideScreen : SideScreenContent
{
	public Assignable targetAssignable { get; private set; }

	public override string GetTitle()
	{
		if (this.targetAssignable != null)
		{
			return string.Format(base.GetTitle(), this.targetAssignable.GetProperName());
		}
		return base.GetTitle();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		MultiToggle multiToggle = this.dupeSortingToggle;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			this.SortByName(true);
		}));
		MultiToggle multiToggle2 = this.generalSortingToggle;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(delegate
		{
			this.SortByAssignment(true);
		}));
		base.Subscribe(Game.Instance.gameObject, 875045922, new Action<object>(this.OnRefreshData));
	}

	private void OnRefreshData(object obj)
	{
		this.SetTarget(this.targetAssignable.gameObject);
	}

	public override void ClearTarget()
	{
		if (this.targetAssignableSubscriptionHandle != -1 && this.targetAssignable != null)
		{
			this.targetAssignable.Unsubscribe(this.targetAssignableSubscriptionHandle);
			this.targetAssignableSubscriptionHandle = -1;
		}
		this.targetAssignable = null;
		Components.LiveMinionIdentities.OnAdd -= this.OnMinionIdentitiesChanged;
		Components.LiveMinionIdentities.OnRemove -= this.OnMinionIdentitiesChanged;
		base.ClearTarget();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Assignable>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		Components.LiveMinionIdentities.OnAdd += this.OnMinionIdentitiesChanged;
		Components.LiveMinionIdentities.OnRemove += this.OnMinionIdentitiesChanged;
		if (this.targetAssignableSubscriptionHandle != -1 && this.targetAssignable != null)
		{
			this.targetAssignable.Unsubscribe(this.targetAssignableSubscriptionHandle);
		}
		this.targetAssignable = target.GetComponent<Assignable>();
		if (this.targetAssignable == null)
		{
			global::Debug.LogError(string.Format("{0} selected has no Assignable component.", target.GetProperName()));
			return;
		}
		if (this.rowPool == null)
		{
			this.rowPool = new UIPool<AssignableSideScreenRow>(this.rowPrefab);
		}
		base.gameObject.SetActive(true);
		this.identityList = new List<MinionAssignablesProxy>(Components.MinionAssignablesProxy.Items);
		this.dupeSortingToggle.ChangeState(0);
		this.generalSortingToggle.ChangeState(0);
		this.activeSortToggle = null;
		this.activeSortFunction = null;
		if (!this.targetAssignable.CanBeAssigned)
		{
			this.HideScreen(true);
		}
		else
		{
			this.HideScreen(false);
		}
		this.targetAssignableSubscriptionHandle = this.targetAssignable.Subscribe(684616645, new Action<object>(this.OnAssigneeChanged));
		this.Refresh(this.identityList);
		this.SortByAssignment(false);
	}

	private void OnMinionIdentitiesChanged(MinionIdentity change)
	{
		this.identityList = new List<MinionAssignablesProxy>(Components.MinionAssignablesProxy.Items);
		this.Refresh(this.identityList);
	}

	private void OnAssigneeChanged(object data = null)
	{
		foreach (KeyValuePair<IAssignableIdentity, AssignableSideScreenRow> keyValuePair in this.identityRowMap)
		{
			keyValuePair.Value.Refresh(null);
		}
	}

	private void Refresh(List<MinionAssignablesProxy> identities)
	{
		this.ClearContent();
		this.currentOwnerText.text = string.Format(UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.UNASSIGNED, Array.Empty<object>());
		if (this.targetAssignable == null)
		{
			return;
		}
		if (this.targetAssignable.GetComponent<Equippable>() == null && !this.targetAssignable.HasTag(GameTags.NotRoomAssignable))
		{
			Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(this.targetAssignable.gameObject);
			if (roomOfGameObject != null)
			{
				RoomType roomType = roomOfGameObject.roomType;
				if (roomType.primary_constraint != null && !roomType.primary_constraint.building_criteria(this.targetAssignable.GetComponent<KPrefabID>()))
				{
					AssignableSideScreenRow freeElement = this.rowPool.GetFreeElement(this.rowGroup, true);
					freeElement.sideScreen = this;
					this.identityRowMap.Add(roomOfGameObject, freeElement);
					freeElement.SetContent(roomOfGameObject, new Action<IAssignableIdentity>(this.OnRowClicked), this);
					return;
				}
			}
		}
		if (this.targetAssignable.canBePublic)
		{
			AssignableSideScreenRow freeElement2 = this.rowPool.GetFreeElement(this.rowGroup, true);
			freeElement2.sideScreen = this;
			freeElement2.transform.SetAsFirstSibling();
			this.identityRowMap.Add(Game.Instance.assignmentManager.assignment_groups["public"], freeElement2);
			freeElement2.SetContent(Game.Instance.assignmentManager.assignment_groups["public"], new Action<IAssignableIdentity>(this.OnRowClicked), this);
		}
		foreach (MinionAssignablesProxy minionAssignablesProxy in identities)
		{
			AssignableSideScreenRow freeElement3 = this.rowPool.GetFreeElement(this.rowGroup, true);
			freeElement3.sideScreen = this;
			this.identityRowMap.Add(minionAssignablesProxy, freeElement3);
			freeElement3.SetContent(minionAssignablesProxy, new Action<IAssignableIdentity>(this.OnRowClicked), this);
		}
		this.ExecuteSort(this.activeSortFunction);
	}

	private void SortByName(bool reselect)
	{
		this.SelectSortToggle(this.dupeSortingToggle, reselect);
		this.ExecuteSort((IAssignableIdentity i1, IAssignableIdentity i2) => i1.GetProperName().CompareTo(i2.GetProperName()) * (this.sortReversed ? (-1) : 1));
	}

	private void SortByAssignment(bool reselect)
	{
		this.SelectSortToggle(this.generalSortingToggle, reselect);
		Comparison<IAssignableIdentity> comparison = delegate(IAssignableIdentity i1, IAssignableIdentity i2)
		{
			int num = this.targetAssignable.CanAssignTo(i1).CompareTo(this.targetAssignable.CanAssignTo(i2));
			if (num != 0)
			{
				return num * -1;
			}
			num = this.identityRowMap[i1].currentState.CompareTo(this.identityRowMap[i2].currentState);
			if (num != 0)
			{
				return num * (this.sortReversed ? (-1) : 1);
			}
			return i1.GetProperName().CompareTo(i2.GetProperName());
		};
		this.ExecuteSort(comparison);
	}

	private void SelectSortToggle(MultiToggle toggle, bool reselect)
	{
		this.dupeSortingToggle.ChangeState(0);
		this.generalSortingToggle.ChangeState(0);
		if (toggle != null)
		{
			if (reselect && this.activeSortToggle == toggle)
			{
				this.sortReversed = !this.sortReversed;
			}
			this.activeSortToggle = toggle;
		}
		this.activeSortToggle.ChangeState(this.sortReversed ? 2 : 1);
	}

	private void ExecuteSort(Comparison<IAssignableIdentity> sortFunction)
	{
		if (sortFunction != null)
		{
			List<IAssignableIdentity> list = new List<IAssignableIdentity>(this.identityRowMap.Keys);
			list.Sort(sortFunction);
			for (int i = 0; i < list.Count; i++)
			{
				this.identityRowMap[list[i]].transform.SetSiblingIndex(i);
			}
			this.activeSortFunction = sortFunction;
		}
	}

	private void ClearContent()
	{
		if (this.rowPool != null)
		{
			this.rowPool.DestroyAll();
		}
		foreach (KeyValuePair<IAssignableIdentity, AssignableSideScreenRow> keyValuePair in this.identityRowMap)
		{
			keyValuePair.Value.targetIdentity = null;
		}
		this.identityRowMap.Clear();
	}

	private void HideScreen(bool hide)
	{
		if (hide)
		{
			base.transform.localScale = Vector3.zero;
			return;
		}
		if (base.transform.localScale != Vector3.one)
		{
			base.transform.localScale = Vector3.one;
		}
	}

	private void OnRowClicked(IAssignableIdentity identity)
	{
		if (this.targetAssignable.assignee != identity)
		{
			this.ChangeAssignment(identity);
			return;
		}
		if (this.CanDeselect(identity))
		{
			this.ChangeAssignment(null);
		}
	}

	private bool CanDeselect(IAssignableIdentity identity)
	{
		return identity is MinionAssignablesProxy;
	}

	private void ChangeAssignment(IAssignableIdentity new_identity)
	{
		this.targetAssignable.Unassign();
		if (new_identity != null)
		{
			this.targetAssignable.Assign(new_identity);
		}
	}

	private void OnValidStateChanged(bool state)
	{
		if (base.gameObject.activeInHierarchy)
		{
			this.Refresh(this.identityList);
		}
	}

	[SerializeField]
	private AssignableSideScreenRow rowPrefab;

	[SerializeField]
	private GameObject rowGroup;

	[SerializeField]
	private LocText currentOwnerText;

	[SerializeField]
	private MultiToggle dupeSortingToggle;

	[SerializeField]
	private MultiToggle generalSortingToggle;

	private MultiToggle activeSortToggle;

	private Comparison<IAssignableIdentity> activeSortFunction;

	private bool sortReversed;

	private int targetAssignableSubscriptionHandle = -1;

	private UIPool<AssignableSideScreenRow> rowPool;

	private Dictionary<IAssignableIdentity, AssignableSideScreenRow> identityRowMap = new Dictionary<IAssignableIdentity, AssignableSideScreenRow>();

	private List<MinionAssignablesProxy> identityList = new List<MinionAssignablesProxy>();
}
