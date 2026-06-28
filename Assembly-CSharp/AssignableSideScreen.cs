using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;

public class AssignableSideScreen : SideScreenContent
{
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
		this.currentOwnerStr = UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.GENERAL_CURRENTASSIGNED;
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
		foreach (MinionIdentity minionIdentity in identities)
		{
			Assignables assignables = this.targetAssignable.GetAssignables(minionIdentity.gameObject);
			AssignableSideScreenRow freeElement = this.rowPool.GetFreeElement(this.rowGroup, true);
			this.identityRowMap.Add(minionIdentity, freeElement);
			string text = string.Empty;
			if (assignables == this.targetAssignable.assignee)
			{
				this.SetSelectedUI(minionIdentity, true);
				this.currentOwnerText.text = string.Format(UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.ASSIGNEDTO, minionIdentity.GetProperName());
			}
			else if (assignables.GetAssignable(this.targetAssignable.slot) != null)
			{
				text = assignables.GetAssignable(this.targetAssignable.slot).GetProperName();
			}
			freeElement.SetContent(minionIdentity, text, new Action<MinionIdentity>(this.OnRowClicked));
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
		foreach (KeyValuePair<MinionIdentity, AssignableSideScreenRow> keyValuePair in this.identityRowMap)
		{
			keyValuePair.Value.Selected = false;
		}
		this.identityRowMap.Clear();
		this.currentSelectedIdentity = null;
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

	private void OnRowClicked(MinionIdentity identity)
	{
		if (identity == null)
		{
			return;
		}
		if (this.currentSelectedIdentity != identity)
		{
			if (this.currentSelectedIdentity != null)
			{
				this.SetSelectedUI(this.currentSelectedIdentity, false);
			}
			this.SetSelectedUI(identity, true);
		}
		else
		{
			this.SetSelectedUI(this.currentSelectedIdentity, false);
			this.currentSelectedIdentity = null;
		}
	}

	private void SetSelectedUI(MinionIdentity identity, bool isSelected)
	{
		this.currentSelectedIdentity = identity;
		AssignableSideScreenRow assignableSideScreenRow = this.identityRowMap[identity];
		assignableSideScreenRow.Selected = isSelected;
		if (this.targetAssignable is Ownable)
		{
			this.targetAssignable.Assign((!isSelected) ? null : identity.GetComponent<Ownables>());
		}
		else if (this.targetAssignable is Equippable)
		{
			this.targetAssignable.Assign((!isSelected) ? null : identity.GetComponent<Equipment>());
			this.targetAssignable.ClickAssign(identity.GetComponent<Equipment>());
		}
		assignableSideScreenRow.SetAssignmentText((!isSelected) ? string.Empty : (this.targetAssignable.GetProperName() + "\n" + this.currentOwnerStr));
		this.currentOwnerText.text = string.Format(UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.ASSIGNEDTO, identity.GetProperName());
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

	[SerializeField]
	private GameObject validRegionContent;

	private Assignable targetAssignable;

	private UIPool<AssignableSideScreenRow> rowPool;

	private Dictionary<MinionIdentity, AssignableSideScreenRow> identityRowMap = new Dictionary<MinionIdentity, AssignableSideScreenRow>();

	private List<MinionIdentity> identityList = new List<MinionIdentity>();

	private MinionIdentity currentSelectedIdentity;

	private string currentOwnerStr;
}
