using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class AccessControl : KMonoBehaviour, ISaveLoadable
{
	public AccessControl.Permission DefaultPermission
	{
		get
		{
			return this._defaultPermission;
		}
		set
		{
			this._defaultPermission = value;
			this.SetStatusItem();
		}
	}

	public bool Online
	{
		get
		{
			return true;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (AccessControl.accessControlActive == null)
		{
			AccessControl.accessControlActive = new StatusItem("accessControlActive", BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.NAME, BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 63486);
		}
		base.Subscribe<AccessControl>(279163026, AccessControl.OnControlStateChangedDelegate);
		base.Subscribe<AccessControl>(-905833192, AccessControl.OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		List<Tuple<MinionAssignablesProxy, AccessControl.Permission>> list = new List<Tuple<MinionAssignablesProxy, AccessControl.Permission>>();
		for (int i = this.savedPermissions.Count - 1; i >= 0; i--)
		{
			KPrefabID kprefabID = this.savedPermissions[i].Key.Get();
			if (kprefabID != null)
			{
				MinionIdentity component = kprefabID.GetComponent<MinionIdentity>();
				if (component != null)
				{
					list.Add(new Tuple<MinionAssignablesProxy, AccessControl.Permission>(component.assignableProxy.Get(), this.savedPermissions[i].Value));
					this.savedPermissions.RemoveAt(i);
				}
			}
		}
		foreach (Tuple<MinionAssignablesProxy, AccessControl.Permission> tuple in list)
		{
			this.SetPermission(tuple.first.gameObject, tuple.second);
		}
		this.SetStatusItem();
	}

	private void OnControlStateChanged(object data)
	{
		this.overrideAccess = (Door.ControlState)data;
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		AccessControl component = gameObject.GetComponent<AccessControl>();
		if (component != null)
		{
			this.savedPermissions.Clear();
			foreach (KeyValuePair<Ref<KPrefabID>, AccessControl.Permission> keyValuePair in component.savedPermissions)
			{
				if (keyValuePair.Key.Get() != null)
				{
					this.SetPermission(keyValuePair.Key.Get().gameObject, keyValuePair.Value);
				}
			}
			this._defaultPermission = component._defaultPermission;
		}
	}

	public void SetPermission(GameObject key, AccessControl.Permission permission)
	{
		KPrefabID component = key.GetComponent<KPrefabID>();
		if (component == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < this.savedPermissions.Count; i++)
		{
			if (this.savedPermissions[i].Key.GetId() == component.InstanceID)
			{
				flag = true;
				KeyValuePair<Ref<KPrefabID>, AccessControl.Permission> keyValuePair = this.savedPermissions[i];
				this.savedPermissions[i] = new KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>(keyValuePair.Key, permission);
				break;
			}
		}
		if (!flag)
		{
			this.savedPermissions.Add(new KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>(new Ref<KPrefabID>(component), permission));
		}
		this.SetStatusItem();
	}

	public AccessControl.Permission GetPermission(GameObject key)
	{
		Door.ControlState controlState = this.overrideAccess;
		if (controlState == Door.ControlState.Closed)
		{
			return AccessControl.Permission.Neither;
		}
		if (controlState != Door.ControlState.Opened)
		{
			return this.GetSetPermission(key);
		}
		return AccessControl.Permission.Both;
	}

	public AccessControl.Permission GetSetPermission(GameObject key)
	{
		AccessControl.Permission permission = this.DefaultPermission;
		KPrefabID component = key.GetComponent<KPrefabID>();
		if (component != null)
		{
			for (int i = 0; i < this.savedPermissions.Count; i++)
			{
				if (this.savedPermissions[i].Key.GetId() == component.InstanceID)
				{
					permission = this.savedPermissions[i].Value;
					break;
				}
			}
		}
		return permission;
	}

	public void ClearPermission(GameObject key)
	{
		AccessControl.Permission defaultPermission = this.DefaultPermission;
		KPrefabID component = key.GetComponent<KPrefabID>();
		if (component != null)
		{
			for (int i = 0; i < this.savedPermissions.Count; i++)
			{
				if (this.savedPermissions[i].Key.GetId() == component.InstanceID)
				{
					this.savedPermissions.RemoveAt(i);
					break;
				}
			}
		}
		this.SetStatusItem();
	}

	public bool IsDefaultPermission(GameObject key)
	{
		bool flag = false;
		KPrefabID component = key.GetComponent<KPrefabID>();
		if (component != null)
		{
			for (int i = 0; i < this.savedPermissions.Count; i++)
			{
				if (this.savedPermissions[i].Key.GetId() == component.InstanceID)
				{
					flag = true;
					break;
				}
			}
		}
		return !flag;
	}

	private void SetStatusItem()
	{
		if (this._defaultPermission != AccessControl.Permission.Both || this.savedPermissions.Count > 0)
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.AccessControl, AccessControl.accessControlActive, null);
		}
		else
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.AccessControl, null, null);
		}
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	[Serialize]
	private List<KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>> savedPermissions = new List<KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>>();

	[Serialize]
	private AccessControl.Permission _defaultPermission;

	[Serialize]
	public bool controlEnabled;

	public Door.ControlState overrideAccess;

	private static StatusItem accessControlActive;

	private static readonly EventSystem.IntraObjectHandler<AccessControl> OnControlStateChangedDelegate = new EventSystem.IntraObjectHandler<AccessControl>(delegate(AccessControl component, object data)
	{
		component.OnControlStateChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<AccessControl> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<AccessControl>(delegate(AccessControl component, object data)
	{
		component.OnCopySettings(data);
	});

	public enum Permission
	{
		Both,
		GoLeft,
		GoRight,
		Neither
	}
}
