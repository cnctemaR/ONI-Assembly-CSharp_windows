using System;
using System.Collections.Generic;
using System.Linq;
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
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(this.SortByName));
		MultiToggle multiToggle2 = this.generalSortingToggle;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(this.GeneralSortToggleClicked));
	}

	public override void SetTarget(GameObject target)
	{
		if (this.targetAssignableSubscriptionHandle != -1)
		{
			this.targetAssignable.Unsubscribe(this.targetAssignableSubscriptionHandle);
		}
		this.targetAssignable = target.GetComponent<Assignable>();
		if (this.targetAssignable == null)
		{
			global::Debug.LogError("Object selected has no Assignable component.", null);
			return;
		}
		if (this.rowPool == null)
		{
			this.rowPool = new UIPool<AssignableSideScreenRow>(this.rowPrefab);
		}
		if (this.targetAssignable.RequiresRegion != null)
		{
			if (this.targetAssignable.RequiresRegion.OwnerRegion != null)
			{
				this.targetAssignable.RequiresRegion.OwnerRegion.OnValidStateChanged += this.OnValidStateChanged;
			}
			this.targetAssignable.RequiresRegion.OnOwnerRegionSet += this.OnRegionChanged;
		}
		base.gameObject.SetActive(true);
		this.identityList = new List<MinionIdentity>(Components.LiveMinionIdentities);
		this.dupeSortingToggle.ChangeState(0);
		this.generalSortingToggle.ChangeState(0);
		this.activeSortToggle = null;
		Func<MinionIdentity, object> func = (MinionIdentity identity) => this.IsIdentityAssigned(identity);
		this.ExecuteSort(this.generalSortingToggle, func, true);
		if (!this.targetAssignable.CanBeAssigned)
		{
			this.HideScreen(true);
		}
		else
		{
			this.HideScreen(false);
		}
		this.targetAssignableSubscriptionHandle = this.targetAssignable.Subscribe(2070884250, new Action<object>(this.OnForceAssigneeChanged));
	}

	private void OnForceAssigneeChanged(object data = null)
	{
		foreach (KeyValuePair<IAssignableIdentity, AssignableSideScreenRow> keyValuePair in this.identityRowMap)
		{
			keyValuePair.Value.Refresh(null);
		}
	}

	private void Refresh(bool is_in_valid_region, List<MinionIdentity> identities)
	{
		this.ClearContent();
		this.validRegionContent.SetActive(is_in_valid_region);
		this.regionNeededText.gameObject.SetActive(!is_in_valid_region);
		if (!is_in_valid_region)
		{
			this.SetRegionNeededUI();
			return;
		}
		this.currentOwnerText.text = string.Format(UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.UNASSIGNED, new object[0]);
		BuildingComplete component = this.targetAssignable.GetComponent<BuildingComplete>();
		if (component != null)
		{
			Room roomOfBuilding = Game.Instance.roomProber.GetRoomOfBuilding(component);
			if (roomOfBuilding != null)
			{
				RoomTypes.RoomType roomType = RoomTypes.GetRoomType(roomOfBuilding);
				if (roomType.primary_constraint != null && !roomType.primary_constraint.building_criteria(component))
				{
					AssignableSideScreenRow freeElement = this.rowPool.GetFreeElement(this.rowGroup, true);
					freeElement.SetContent(roomOfBuilding, new Action<IAssignableIdentity>(this.OnRowClicked), this);
					freeElement.sideScreen = this;
					this.identityRowMap.Add(roomOfBuilding, freeElement);
					freeElement.Refresh(null);
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
			freeElement2.Refresh(null);
		}
		foreach (MinionIdentity minionIdentity in identities)
		{
			AssignableSideScreenRow freeElement3 = this.rowPool.GetFreeElement(this.rowGroup, true);
			freeElement3.sideScreen = this;
			this.identityRowMap.Add(minionIdentity, freeElement3);
			freeElement3.SetContent(minionIdentity, new Action<IAssignableIdentity>(this.OnRowClicked), this);
		}
	}

	private void GeneralSortToggleClicked()
	{
		Func<MinionIdentity, object> func = (MinionIdentity identity) => this.IsIdentityAssigned(identity);
		this.ExecuteSort(this.generalSortingToggle, func, false);
		this.dupeSortingToggle.ChangeState(0);
	}

	private void ExecuteSort(MultiToggle toggle, Func<MinionIdentity, object> sortFunction, bool refresh = false)
	{
		if (this.activeSortToggle == toggle)
		{
			if (this.sortReversed)
			{
				this.sortReversed = false;
				toggle.ChangeState(0);
				this.activeSortToggle = null;
				return;
			}
			this.sortReversed = true;
		}
		else
		{
			this.sortReversed = false;
		}
		toggle.ChangeState((!this.sortReversed) ? 1 : 2);
		this.identityList = ((!this.sortReversed) ? this.identityList.OrderByDescending<MinionIdentity, object>(sortFunction).ToList<MinionIdentity>() : this.identityList.OrderBy<MinionIdentity, object>(sortFunction).ToList<MinionIdentity>());
		bool flag = this.targetAssignable == null || this.targetAssignable.RequiresRegion == null || this.targetAssignable.RequiresRegion.IsRegionValid(false);
		if (refresh)
		{
			this.Refresh(flag, this.identityList);
		}
		else
		{
			for (int i = 0; i < this.identityList.Count; i++)
			{
				if (this.identityRowMap.ContainsKey(this.identityList[i]))
				{
					this.identityRowMap[this.identityList[i]].transform.SetSiblingIndex(i);
				}
			}
		}
		this.activeSortToggle = toggle;
	}

	private void SortByName()
	{
		this.ExecuteSort(this.dupeSortingToggle, (MinionIdentity idt) => idt.GetProperName(), false);
		this.generalSortingToggle.ChangeState(0);
	}

	private bool IsIdentityAssigned(MinionIdentity identity)
	{
		Assignables assignables = this.targetAssignable.GetAssignables(identity.gameObject);
		return assignables.GetAssignable(this.targetAssignable.slot) != null;
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
		this.regionNeededText.SetText(string.Empty);
	}

	private void HideScreen(bool hide)
	{
		if (hide)
		{
			this.transform.localScale = Vector3.zero;
			this.validRegionContent.SetActive(false);
			this.regionNeededText.gameObject.SetActive(false);
		}
		else if (this.transform.localScale != Vector3.one)
		{
			this.transform.localScale = Vector3.one;
		}
	}

	private void SetRegionNeededUI()
	{
		TagSet requiredRegions = this.targetAssignable.GetComponent<RequiresRegion>().RequiredRegions;
		string text = requiredRegions.ToString();
		string text2 = ((requiredRegions.Count <= 0) ? UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.NEEDSREGION : UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.NEEDSREGIONS);
		string properName = this.targetAssignable.GetProperName();
		this.regionNeededText.SetText(string.Format(text2, properName, text));
	}

	private void OnRowClicked(IAssignableIdentity identity)
	{
		if (this.targetAssignable.assignee != identity)
		{
			this.ChangeAssignment(identity);
		}
		else if (this.CanDeselect(identity))
		{
			this.ChangeAssignment(null);
		}
	}

	private bool CanDeselect(IAssignableIdentity identity)
	{
		return identity is MinionIdentity;
	}

	private void ChangeAssignment(IAssignableIdentity new_identity)
	{
		this.targetAssignable.Unassign();
		if (new_identity != null)
		{
			this.targetAssignable.Assign(new_identity);
		}
		foreach (KeyValuePair<IAssignableIdentity, AssignableSideScreenRow> keyValuePair in this.identityRowMap)
		{
			keyValuePair.Value.Refresh(null);
		}
	}

	private void OnRegionChanged(Region newRegion)
	{
		if (this.targetAssignable.RequiresRegion != null && this.targetAssignable.RequiresRegion.OwnerRegion != null)
		{
			this.targetAssignable.RequiresRegion.OwnerRegion.OnValidStateChanged -= this.OnValidStateChanged;
		}
		if (newRegion != null)
		{
			newRegion.OnValidStateChanged += this.OnValidStateChanged;
		}
	}

	private void OnValidStateChanged(bool state)
	{
		if (base.gameObject.activeInHierarchy)
		{
			this.Refresh(state, this.identityList);
		}
	}

	[SerializeField]
	private AssignableSideScreenRow rowPrefab;

	[SerializeField]
	private GameObject rowGroup;

	[SerializeField]
	private LocText regionNeededText;

	[SerializeField]
	private LocText currentOwnerText;

	[SerializeField]
	private MultiToggle dupeSortingToggle;

	[SerializeField]
	private MultiToggle generalSortingToggle;

	private MultiToggle activeSortToggle;

	private bool sortReversed;

	private int targetAssignableSubscriptionHandle = -1;

	[SerializeField]
	private GameObject validRegionContent;

	private UIPool<AssignableSideScreenRow> rowPool;

	private Dictionary<IAssignableIdentity, AssignableSideScreenRow> identityRowMap = new Dictionary<IAssignableIdentity, AssignableSideScreenRow>();

	private List<MinionIdentity> identityList = new List<MinionIdentity>();
}
