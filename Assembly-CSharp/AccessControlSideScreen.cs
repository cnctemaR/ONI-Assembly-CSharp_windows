using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AccessControlSideScreen : SideScreenContent
{
	public override string GetTitle()
	{
		if (this.target != null)
		{
			return string.Format(base.GetTitle(), this.target.GetProperName());
		}
		return base.GetTitle();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.sortByNameToggle.onValueChanged.AddListener(delegate(bool reverse_sort)
		{
			this.SortEntries(reverse_sort, new Comparison<MinionAssignablesProxy>(AccessControlSideScreen.MinionIdentitySort.CompareByName));
		});
		this.sortByRoleToggle.onValueChanged.AddListener(delegate(bool reverse_sort)
		{
			this.SortEntries(reverse_sort, new Comparison<MinionAssignablesProxy>(AccessControlSideScreen.MinionIdentitySort.CompareByRole));
		});
		this.sortByPermissionToggle.onValueChanged.AddListener(new UnityAction<bool>(this.SortByPermission));
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<AccessControl>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		if (this.target != null)
		{
			this.ClearTarget();
		}
		this.target = target.GetComponent<AccessControl>();
		this.doorTarget = target.GetComponent<Door>();
		if (this.target == null)
		{
			return;
		}
		target.Subscribe(1734268753, new Action<object>(this.OnDoorStateChanged));
		target.Subscribe(-1525636549, new Action<object>(this.OnAccessControlChanged));
		if (this.rowPool == null)
		{
			this.rowPool = new UIPool<AccessControlSideScreenRow>(this.rowPrefab);
		}
		base.gameObject.SetActive(true);
		this.identityList = new List<MinionAssignablesProxy>(Components.MinionAssignablesProxy.Items);
		this.Refresh(this.identityList, true);
	}

	public override void ClearTarget()
	{
		base.ClearTarget();
		if (this.target != null)
		{
			this.target.Unsubscribe(1734268753, new Action<object>(this.OnDoorStateChanged));
			this.target.Unsubscribe(-1525636549, new Action<object>(this.OnAccessControlChanged));
		}
	}

	private void Refresh(List<MinionAssignablesProxy> identities, bool rebuild)
	{
		Rotatable component = this.target.GetComponent<Rotatable>();
		bool flag = component != null && component.IsRotated;
		this.defaultsRow.SetRotated(flag);
		this.defaultsRow.SetContent(this.target.DefaultPermission, new Action<MinionAssignablesProxy, AccessControl.Permission>(this.OnDefaultPermissionChanged));
		if (rebuild)
		{
			this.ClearContent();
		}
		foreach (MinionAssignablesProxy minionAssignablesProxy in identities)
		{
			AccessControlSideScreenRow accessControlSideScreenRow;
			if (rebuild)
			{
				accessControlSideScreenRow = this.rowPool.GetFreeElement(this.rowGroup, true);
				this.identityRowMap.Add(minionAssignablesProxy, accessControlSideScreenRow);
			}
			else
			{
				accessControlSideScreenRow = this.identityRowMap[minionAssignablesProxy];
			}
			AccessControl.Permission setPermission = this.target.GetSetPermission(minionAssignablesProxy);
			bool flag2 = this.target.IsDefaultPermission(minionAssignablesProxy);
			accessControlSideScreenRow.SetRotated(flag);
			accessControlSideScreenRow.SetMinionContent(minionAssignablesProxy, setPermission, flag2, new Action<MinionAssignablesProxy, AccessControl.Permission>(this.OnPermissionChanged), new Action<MinionAssignablesProxy, bool>(this.OnPermissionDefault));
		}
		this.RefreshOnline();
		this.ContentContainer.SetActive(this.target.controlEnabled);
	}

	private void RefreshOnline()
	{
		bool flag = this.target.Online && (this.doorTarget == null || this.doorTarget.CurrentState == Door.ControlState.Auto);
		this.disabledOverlay.SetActive(!flag);
		this.headerBG.ColorState = ((!flag) ? KImage.ColorSelector.Inactive : KImage.ColorSelector.Active);
	}

	private void SortByPermission(bool state)
	{
		this.ExecuteSort<int>(this.sortByPermissionToggle, state, (MinionAssignablesProxy identity) => (int)((!this.target.IsDefaultPermission(identity)) ? this.target.GetSetPermission(identity) : ((AccessControl.Permission)(-1))), false);
	}

	private void ExecuteSort<T>(Toggle toggle, bool state, Func<MinionAssignablesProxy, T> sortFunction, bool refresh = false)
	{
		toggle.GetComponent<ImageToggleState>().SetActiveState(state);
		if (!state)
		{
			return;
		}
		this.identityList = ((!state) ? this.identityList.OrderByDescending<MinionAssignablesProxy, T>(sortFunction).ToList<MinionAssignablesProxy>() : this.identityList.OrderBy<MinionAssignablesProxy, T>(sortFunction).ToList<MinionAssignablesProxy>());
		if (refresh)
		{
			this.Refresh(this.identityList, false);
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
	}

	private void SortEntries(bool reverse_sort, Comparison<MinionAssignablesProxy> compare)
	{
		this.identityList.Sort(compare);
		if (reverse_sort)
		{
			this.identityList.Reverse();
		}
		for (int i = 0; i < this.identityList.Count; i++)
		{
			if (this.identityRowMap.ContainsKey(this.identityList[i]))
			{
				this.identityRowMap[this.identityList[i]].transform.SetSiblingIndex(i);
			}
		}
	}

	private void ClearContent()
	{
		if (this.rowPool != null)
		{
			this.rowPool.ClearAll();
		}
		this.identityRowMap.Clear();
	}

	private void OnDefaultPermissionChanged(MinionAssignablesProxy identity, AccessControl.Permission permission)
	{
		this.target.DefaultPermission = permission;
		this.Refresh(this.identityList, false);
		foreach (MinionAssignablesProxy minionAssignablesProxy in this.identityList)
		{
			if (this.target.IsDefaultPermission(minionAssignablesProxy))
			{
				this.target.ClearPermission(minionAssignablesProxy);
			}
		}
	}

	private void OnPermissionChanged(MinionAssignablesProxy identity, AccessControl.Permission permission)
	{
		this.target.SetPermission(identity, permission);
	}

	private void OnPermissionDefault(MinionAssignablesProxy identity, bool isDefault)
	{
		if (isDefault)
		{
			this.target.ClearPermission(identity);
		}
		else
		{
			this.target.SetPermission(identity, this.target.DefaultPermission);
		}
		this.Refresh(this.identityList, false);
	}

	private void OnAccessControlChanged(object data)
	{
		this.RefreshOnline();
	}

	private void OnDoorStateChanged(object data)
	{
		this.RefreshOnline();
	}

	private void OnSelectSortFunc(IListableOption role, object data)
	{
		if (role != null)
		{
			foreach (AccessControlSideScreen.MinionIdentitySort.SortInfo sortInfo in AccessControlSideScreen.MinionIdentitySort.SortInfos)
			{
				if (sortInfo.name == role.GetProperName())
				{
					this.sortInfo = sortInfo;
					this.identityList.Sort(this.sortInfo.compare);
					for (int j = 0; j < this.identityList.Count; j++)
					{
						if (this.identityRowMap.ContainsKey(this.identityList[j]))
						{
							this.identityRowMap[this.identityList[j]].transform.SetSiblingIndex(j);
						}
					}
					break;
				}
			}
		}
	}

	[SerializeField]
	private AccessControlSideScreenRow rowPrefab;

	[SerializeField]
	private GameObject rowGroup;

	[SerializeField]
	private AccessControlSideScreenDoor defaultsRow;

	[SerializeField]
	private Toggle sortByNameToggle;

	[SerializeField]
	private Toggle sortByPermissionToggle;

	[SerializeField]
	private Toggle sortByRoleToggle;

	[SerializeField]
	private GameObject disabledOverlay;

	[SerializeField]
	private KImage headerBG;

	private AccessControl target;

	private Door doorTarget;

	private UIPool<AccessControlSideScreenRow> rowPool;

	private AccessControlSideScreen.MinionIdentitySort.SortInfo sortInfo = AccessControlSideScreen.MinionIdentitySort.SortInfos[0];

	private Dictionary<MinionAssignablesProxy, AccessControlSideScreenRow> identityRowMap = new Dictionary<MinionAssignablesProxy, AccessControlSideScreenRow>();

	private List<MinionAssignablesProxy> identityList = new List<MinionAssignablesProxy>();

	private static class MinionIdentitySort
	{
		public static int CompareByName(MinionAssignablesProxy a, MinionAssignablesProxy b)
		{
			return a.GetProperName().CompareTo(b.GetProperName());
		}

		public static int CompareByRole(MinionAssignablesProxy a, MinionAssignablesProxy b)
		{
			GameObject targetGameObject = a.GetTargetGameObject();
			GameObject targetGameObject2 = b.GetTargetGameObject();
			MinionResume minionResume = ((!targetGameObject) ? null : targetGameObject.GetComponent<MinionResume>());
			MinionResume minionResume2 = ((!targetGameObject2) ? null : targetGameObject2.GetComponent<MinionResume>());
			if (minionResume2 == null)
			{
				return 1;
			}
			if (minionResume == null)
			{
				return -1;
			}
			int num = minionResume.CurrentRole.CompareTo(minionResume2.CurrentRole);
			return (num != 0) ? num : AccessControlSideScreen.MinionIdentitySort.CompareByName(a, b);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MinionIdentitySort()
		{
			AccessControlSideScreen.MinionIdentitySort.SortInfo[] array = new AccessControlSideScreen.MinionIdentitySort.SortInfo[2];
			int num = 0;
			AccessControlSideScreen.MinionIdentitySort.SortInfo sortInfo = new AccessControlSideScreen.MinionIdentitySort.SortInfo();
			sortInfo.name = UI.MINION_IDENTITY_SORT.NAME;
			sortInfo.compare = new Comparison<MinionAssignablesProxy>(AccessControlSideScreen.MinionIdentitySort.CompareByName);
			array[num] = sortInfo;
			int num2 = 1;
			sortInfo = new AccessControlSideScreen.MinionIdentitySort.SortInfo();
			sortInfo.name = UI.MINION_IDENTITY_SORT.ROLE;
			sortInfo.compare = new Comparison<MinionAssignablesProxy>(AccessControlSideScreen.MinionIdentitySort.CompareByRole);
			array[num2] = sortInfo;
			AccessControlSideScreen.MinionIdentitySort.SortInfos = array;
		}

		public static readonly AccessControlSideScreen.MinionIdentitySort.SortInfo[] SortInfos;

		public class SortInfo : IListableOption
		{
			public string GetProperName()
			{
				return this.name;
			}

			public LocString name;

			public Comparison<MinionAssignablesProxy> compare;
		}
	}
}
