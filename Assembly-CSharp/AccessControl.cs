using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
			return this.cached_powered;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (AccessControl.accessControlActive == null)
		{
			AccessControl.accessControlActive = new StatusItem("accessControlActive", BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.NAME, BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			AccessControl.accessControlOffline = new StatusItem("accessControlOffline", BUILDING.STATUSITEMS.ACCESS_CONTROL.OFFLINE.NAME, BUILDING.STATUSITEMS.ACCESS_CONTROL.OFFLINE.TOOLTIP, string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
		}
		if (this.operational != null)
		{
			this.Subscribe(187661686, new Action<object>(this.OnOperationalFlagChanged));
		}
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		foreach (KeyValuePair<Ref<KPrefabID>, AccessControl.Permission> keyValuePair in this.savedPermissions)
		{
			if (keyValuePair.Key != null)
			{
				GameObject gameObject = keyValuePair.Key.Get().gameObject;
				this.permissions[gameObject] = keyValuePair.Value;
			}
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		this.savedPermissions = new List<KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>>();
		foreach (KeyValuePair<GameObject, AccessControl.Permission> keyValuePair in this.permissions)
		{
			if (keyValuePair.Key != null)
			{
				KPrefabID component = keyValuePair.Key.GetComponent<KPrefabID>();
				this.savedPermissions.Add(new KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>(new Ref<KPrefabID>(component), keyValuePair.Value));
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.operational != null)
		{
			this.cached_powered = this.operational.GetFlag(EnergyConsumer.PoweredFlag);
			this.Trigger(-1525636549, this);
		}
		this.SetStatusItem();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.operational != null)
		{
			this.Unsubscribe(187661686, new Action<object>(this.OnOperationalFlagChanged));
		}
	}

	public void SetPermission(GameObject key, AccessControl.Permission permission)
	{
		this.permissions[key] = permission;
	}

	public AccessControl.Permission GetPermission(GameObject key)
	{
		if (!this.cached_powered)
		{
			return AccessControl.Permission.Both;
		}
		return this.GetSetPermission(key);
	}

	public AccessControl.Permission GetSetPermission(GameObject key)
	{
		AccessControl.Permission permission = this.DefaultPermission;
		if (!this.permissions.TryGetValue(key, out permission))
		{
			permission = this.DefaultPermission;
		}
		return permission;
	}

	public void ClearPermission(GameObject key)
	{
		this.permissions.Remove(key);
	}

	public bool IsDefaultPermission(GameObject key)
	{
		return !this.permissions.ContainsKey(key);
	}

	private void OnOperationalFlagChanged(object obj)
	{
		Operational.Flag flag = (Operational.Flag)obj;
		if (flag == EnergyConsumer.PoweredFlag)
		{
			this.cached_powered = this.operational.GetFlag(EnergyConsumer.PoweredFlag);
			this.Trigger(-1525636549, this);
			this.SetStatusItem();
		}
	}

	private void SetStatusItem()
	{
		if (!this.cached_powered)
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.AccessControl, AccessControl.accessControlOffline, null);
		}
		else if (this._defaultPermission != AccessControl.Permission.Both || this.permissions.Count > 0)
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

	private Dictionary<GameObject, AccessControl.Permission> permissions = new Dictionary<GameObject, AccessControl.Permission>();

	[Serialize]
	private List<KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>> savedPermissions;

	[Serialize]
	private AccessControl.Permission _defaultPermission;

	private bool cached_powered = true;

	private static StatusItem accessControlActive;

	private static StatusItem accessControlOffline;

	public enum Permission
	{
		Both,
		GoLeft,
		GoRight,
		Neither
	}
}
