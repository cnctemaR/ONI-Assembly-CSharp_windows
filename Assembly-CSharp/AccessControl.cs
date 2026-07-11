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
			return true;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (AccessControl.accessControlActive == null)
		{
			AccessControl.accessControlActive = new StatusItem("accessControlActive", BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.NAME, BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486);
		}
		base.Subscribe(279163026, new Action<object>(this.OnControlStateChanged));
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

	[OnSerialized]
	private void OnSerialized()
	{
		this.savedPermissions = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.savedPermissions != null)
		{
			foreach (KeyValuePair<Ref<KPrefabID>, AccessControl.Permission> keyValuePair in this.savedPermissions)
			{
				if (keyValuePair.Key != null)
				{
					KPrefabID kprefabID = keyValuePair.Key.Get();
					if (kprefabID != null)
					{
						GameObject gameObject = kprefabID.gameObject;
						this.permissions[gameObject] = keyValuePair.Value;
					}
				}
			}
			this.savedPermissions = null;
		}
		this.SetStatusItem();
	}

	private void OnControlStateChanged(object data)
	{
		this.overrideAccess = (Door.ControlState)data;
	}

	public void SetPermission(GameObject key, AccessControl.Permission permission)
	{
		this.permissions[key] = permission;
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
		if (!this.permissions.TryGetValue(key, out permission))
		{
			permission = this.DefaultPermission;
		}
		return permission;
	}

	public void ClearPermission(GameObject key)
	{
		this.permissions.Remove(key);
		this.SetStatusItem();
	}

	public bool IsDefaultPermission(GameObject key)
	{
		return !this.permissions.ContainsKey(key);
	}

	private void SetStatusItem()
	{
		if (this._defaultPermission != AccessControl.Permission.Both || this.permissions.Count > 0)
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

	[Serialize]
	public bool controlEnabled;

	public Door.ControlState overrideAccess;

	private static StatusItem accessControlActive;

	public enum Permission
	{
		Both,
		GoLeft,
		GoRight,
		Neither
	}
}
