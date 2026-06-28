using System;
using System.Collections.Generic;
using System.Linq;
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
		this.dupeSortingToggle.onValueChanged.AddListener(new UnityAction<bool>(this.SortByName));
		this.permissionSortingToggle.onValueChanged.AddListener(new UnityAction<bool>(this.SortByPermission));
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
		this.identityList = new List<MinionIdentity>(Components.LiveMinionIdentities);
		this.dupeSortingToggle.isOn = true;
		this.Refresh(this.identityList, true);
		this.SortByName(true);
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

	private void Refresh(List<MinionIdentity> identities, bool rebuild)
	{
		Rotatable component = this.target.GetComponent<Rotatable>();
		bool flag = component != null && component.IsRotated;
		this.defaultsRow.SetRotated(flag);
		this.defaultsRow.SetContent(this.target.DefaultPermission, new Action<MinionIdentity, AccessControl.Permission>(this.OnDefaultPermissionChanged));
		if (rebuild)
		{
			this.ClearContent();
		}
		foreach (MinionIdentity minionIdentity in identities)
		{
			AccessControlSideScreenRow accessControlSideScreenRow;
			if (rebuild)
			{
				accessControlSideScreenRow = this.rowPool.GetFreeElement(this.rowGroup, true);
				this.identityRowMap.Add(minionIdentity, accessControlSideScreenRow);
			}
			else
			{
				accessControlSideScreenRow = this.identityRowMap[minionIdentity];
			}
			AccessControl.Permission setPermission = this.target.GetSetPermission(minionIdentity.gameObject);
			bool flag2 = this.target.IsDefaultPermission(minionIdentity.gameObject);
			accessControlSideScreenRow.SetRotated(flag);
			accessControlSideScreenRow.SetMinionContent(minionIdentity, setPermission, flag2, new Action<MinionIdentity, AccessControl.Permission>(this.OnPermissionChanged), new Action<MinionIdentity, bool>(this.OnPermissionDefault));
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

	private void SortByName(bool state)
	{
		this.ExecuteSort<string>(this.dupeSortingToggle, state, (MinionIdentity idt) => idt.GetProperName(), false);
	}

	private void SortByPermission(bool state)
	{
		this.ExecuteSort<int>(this.permissionSortingToggle, state, (MinionIdentity identity) => (int)((!this.target.IsDefaultPermission(identity.gameObject)) ? this.target.GetSetPermission(identity.gameObject) : ((AccessControl.Permission)(-1))), false);
	}

	private void ExecuteSort<T>(Toggle toggle, bool state, Func<MinionIdentity, T> sortFunction, bool refresh = false)
	{
		toggle.GetComponent<ImageToggleState>().SetActiveState(state);
		if (!state)
		{
			return;
		}
		this.identityList = ((!state) ? this.identityList.OrderByDescending<MinionIdentity, T>(sortFunction).ToList<MinionIdentity>() : this.identityList.OrderBy<MinionIdentity, T>(sortFunction).ToList<MinionIdentity>());
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

	private void ClearContent()
	{
		if (this.rowPool != null)
		{
			this.rowPool.ClearAll();
		}
		this.identityRowMap.Clear();
	}

	private void OnDefaultPermissionChanged(MinionIdentity identity, AccessControl.Permission permission)
	{
		this.target.DefaultPermission = permission;
		this.Refresh(this.identityList, false);
	}

	private void OnPermissionChanged(MinionIdentity identity, AccessControl.Permission permission)
	{
		this.target.SetPermission(identity.gameObject, permission);
	}

	private void OnPermissionDefault(MinionIdentity identity, bool isDefault)
	{
		if (isDefault)
		{
			this.target.ClearPermission(identity.gameObject);
		}
		else
		{
			this.target.SetPermission(identity.gameObject, this.target.DefaultPermission);
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

	[SerializeField]
	private AccessControlSideScreenRow rowPrefab;

	[SerializeField]
	private GameObject rowGroup;

	[SerializeField]
	private AccessControlSideScreenDoor defaultsRow;

	[SerializeField]
	private Toggle dupeSortingToggle;

	[SerializeField]
	private Toggle permissionSortingToggle;

	[SerializeField]
	private GameObject disabledOverlay;

	[SerializeField]
	private KImage headerBG;

	private AccessControl target;

	private Door doorTarget;

	private UIPool<AccessControlSideScreenRow> rowPool;

	private Dictionary<MinionIdentity, AccessControlSideScreenRow> identityRowMap = new Dictionary<MinionIdentity, AccessControlSideScreenRow>();

	private List<MinionIdentity> identityList = new List<MinionIdentity>();
}
