using System;
using System.Collections.Generic;
using System.Linq;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/AccessControl")]
public class AccessControl : KMonoBehaviour, ISaveLoadable, IGameObjectEffectDescriptor
{
	private int GetTagId(Tag game_tag)
	{
		return GridRestrictionSerializer.Instance.GetTagId(game_tag);
	}

	public AccessControl.Permission GetDefaultPermission(Tag groupTag)
	{
		foreach (KeyValuePair<Tag, AccessControl.Permission> keyValuePair in this.defaultPermissionByTag)
		{
			if (keyValuePair.Key == groupTag)
			{
				return keyValuePair.Value;
			}
		}
		return AccessControl.Permission.Both;
	}

	public void SetDefaultPermission(Tag groupTag, AccessControl.Permission permission)
	{
		bool flag = false;
		KeyValuePair<Tag, AccessControl.Permission> keyValuePair = new KeyValuePair<Tag, AccessControl.Permission>(groupTag, permission);
		for (int i = 0; i < this.defaultPermissionByTag.Count; i++)
		{
			if (this.defaultPermissionByTag[i].Key == groupTag)
			{
				this.defaultPermissionByTag[i] = keyValuePair;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.defaultPermissionByTag.Add(keyValuePair);
		}
		this.SetStatusItem();
		this.SetGridRestrictions(this.GetTagId(groupTag), permission);
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
			AccessControl.accessControlActive = new StatusItem("accessControlActive", BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.NAME, BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022, true, null);
		}
		base.Subscribe<AccessControl>(279163026, AccessControl.OnControlStateChangedDelegate);
		base.Subscribe<AccessControl>(-905833192, AccessControl.OnCopySettingsDelegate);
	}

	[Obsolete("Added support for Robots Access Controls")]
	private void CheckForBadData()
	{
		List<KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>> list = new List<KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>>();
		foreach (KeyValuePair<Ref<KPrefabID>, AccessControl.Permission> keyValuePair in this.savedPermissions)
		{
			if (keyValuePair.Key.Get() == null)
			{
				list.Add(keyValuePair);
			}
		}
		foreach (KeyValuePair<Ref<KPrefabID>, AccessControl.Permission> keyValuePair2 in list)
		{
			this.savedPermissions.Remove(keyValuePair2);
		}
	}

	private void UpgradeSavePreRobotDoorPermission()
	{
		ListPool<global::Tuple<MinionAssignablesProxy, AccessControl.Permission>, AccessControl>.PooledList pooledList = ListPool<global::Tuple<MinionAssignablesProxy, AccessControl.Permission>, AccessControl>.Allocate();
		for (int i = this.savedPermissions.Count - 1; i >= 0; i--)
		{
			KPrefabID kprefabID = this.savedPermissions[i].Key.Get();
			if (kprefabID != null)
			{
				MinionIdentity component = kprefabID.GetComponent<MinionIdentity>();
				if (component != null)
				{
					pooledList.Add(new global::Tuple<MinionAssignablesProxy, AccessControl.Permission>(component.assignableProxy.Get(), this.savedPermissions[i].Value));
					this.savedPermissions.RemoveAt(i);
					this.ClearGridRestrictions(kprefabID);
				}
			}
		}
		foreach (global::Tuple<MinionAssignablesProxy, AccessControl.Permission> tuple in pooledList)
		{
			this.SetPermission(tuple.first, tuple.second);
		}
		pooledList.Recycle();
	}

	private void UpgradeSavesToPostRobotDoorPermissions()
	{
		if (this._defaultPermission != AccessControl.Permission.Both)
		{
			this.SetDefaultPermission(GameTags.Minions.Models.Standard, this._defaultPermission);
			this.SetDefaultPermission(GameTags.Minions.Models.Bionic, this._defaultPermission);
			this._defaultPermission = AccessControl.Permission.Both;
		}
		foreach (KeyValuePair<Ref<KPrefabID>, AccessControl.Permission> keyValuePair in this.savedPermissions)
		{
			this.SetPermission(keyValuePair.Key.Get().GetComponent<MinionAssignablesProxy>(), keyValuePair.Value);
		}
		this.savedPermissions.Clear();
	}

	protected override void OnSpawn()
	{
		this.isTeleporter = base.GetComponent<NavTeleporter>() != null;
		base.OnSpawn();
		if (this.savedPermissions.Count > 0)
		{
			this.CheckForBadData();
		}
		if (this.registered)
		{
			this.RegisterInGrid(true);
			this.RestorePermissions();
		}
		this.UpgradeSavePreRobotDoorPermission();
		this.UpgradeSavesToPostRobotDoorPermissions();
		this.SetStatusItem();
	}

	protected override void OnCleanUp()
	{
		this.RegisterInGrid(false);
		base.OnCleanUp();
	}

	private void OnControlStateChanged(object data)
	{
		this.overrideAccess = ((Boxed<Door.ControlState>)data).value;
		this.SetStatusItem();
	}

	private void OnCopySettings(object data)
	{
		AccessControl component = ((GameObject)data).GetComponent<AccessControl>();
		if (component != null)
		{
			this.savedPermissionsById.Clear();
			foreach (KeyValuePair<int, AccessControl.Permission> keyValuePair in component.savedPermissionsById)
			{
				this.SetPermission(keyValuePair.Key, keyValuePair.Value);
			}
			this.defaultPermissionByTag = new List<KeyValuePair<Tag, AccessControl.Permission>>(component.defaultPermissionByTag);
			foreach (KeyValuePair<Tag, AccessControl.Permission> keyValuePair2 in this.defaultPermissionByTag)
			{
				this.SetGridRestrictions(this.GetTagId(keyValuePair2.Key), keyValuePair2.Value);
			}
		}
	}

	public void SetRegistered(bool newRegistered)
	{
		if (newRegistered && !this.registered)
		{
			this.RegisterInGrid(true);
			this.RestorePermissions();
			return;
		}
		if (!newRegistered && this.registered)
		{
			this.RegisterInGrid(false);
		}
	}

	private void SetPermission(int id, AccessControl.Permission permission)
	{
		bool flag = false;
		for (int i = 0; i < this.savedPermissionsById.Count; i++)
		{
			if (this.savedPermissionsById[i].Key == id)
			{
				flag = true;
				KeyValuePair<int, AccessControl.Permission> keyValuePair = this.savedPermissionsById[i];
				this.savedPermissionsById[i] = new KeyValuePair<int, AccessControl.Permission>(keyValuePair.Key, permission);
				break;
			}
		}
		if (!flag)
		{
			this.savedPermissionsById.Add(new KeyValuePair<int, AccessControl.Permission>(id, permission));
		}
		this.SetStatusItem();
		this.SetGridRestrictions(id, permission);
	}

	public void SetPermission(MinionAssignablesProxy key, AccessControl.Permission permission)
	{
		this.SetPermission(key.GetComponent<KPrefabID>().InstanceID, permission);
	}

	public void SetPermission(Tag gameTag, AccessControl.Permission permission)
	{
		this.SetPermission(this.GetTagId(gameTag), permission);
	}

	private void RestorePermissions()
	{
		foreach (KeyValuePair<Tag, AccessControl.Permission> keyValuePair in this.defaultPermissionByTag)
		{
			this.SetGridRestrictions(this.GetTagId(keyValuePair.Key), keyValuePair.Value);
		}
		foreach (KeyValuePair<int, AccessControl.Permission> keyValuePair2 in this.savedPermissionsById)
		{
			this.SetGridRestrictions(keyValuePair2.Key, keyValuePair2.Value);
		}
	}

	private void RegisterInGrid(bool register)
	{
		Building component = base.GetComponent<Building>();
		OccupyArea component2 = base.GetComponent<OccupyArea>();
		if (component2 == null && component == null)
		{
			return;
		}
		if (register)
		{
			Rotatable component3 = base.GetComponent<Rotatable>();
			Grid.Restriction.Orientation orientation;
			if (!this.isTeleporter)
			{
				orientation = ((component3 == null || component3.GetOrientation() == Orientation.Neutral) ? Grid.Restriction.Orientation.Vertical : Grid.Restriction.Orientation.Horizontal);
			}
			else
			{
				orientation = Grid.Restriction.Orientation.SingleCell;
			}
			if (component != null)
			{
				this.registeredBuildingCells = component.PlacementCells;
				int[] array = this.registeredBuildingCells;
				for (int i = 0; i < array.Length; i++)
				{
					Grid.RegisterRestriction(array[i], orientation);
				}
			}
			else
			{
				foreach (CellOffset cellOffset in component2.OccupiedCellsOffsets)
				{
					Grid.RegisterRestriction(Grid.OffsetCell(Grid.PosToCell(component2), cellOffset), orientation);
				}
			}
			if (this.isTeleporter)
			{
				Grid.RegisterRestriction(base.GetComponent<NavTeleporter>().GetCell(), orientation);
			}
		}
		else
		{
			if (component != null)
			{
				if (component.GetMyWorldId() != 255 && this.registeredBuildingCells != null)
				{
					int[] array = this.registeredBuildingCells;
					for (int i = 0; i < array.Length; i++)
					{
						Grid.UnregisterRestriction(array[i]);
					}
					this.registeredBuildingCells = null;
				}
			}
			else
			{
				foreach (CellOffset cellOffset2 in component2.OccupiedCellsOffsets)
				{
					Grid.UnregisterRestriction(Grid.OffsetCell(Grid.PosToCell(component2), cellOffset2));
				}
			}
			if (this.isTeleporter)
			{
				int cell = base.GetComponent<NavTeleporter>().GetCell();
				if (cell != Grid.InvalidCell)
				{
					Grid.UnregisterRestriction(cell);
				}
			}
		}
		this.registered = register;
	}

	private void SetGridRestrictions(int id, AccessControl.Permission permission)
	{
		if (!this.registered || !base.isSpawned)
		{
			return;
		}
		Building component = base.GetComponent<Building>();
		OccupyArea component2 = base.GetComponent<OccupyArea>();
		if (component2 == null && component == null)
		{
			return;
		}
		Grid.Restriction.Directions directions = (Grid.Restriction.Directions)0;
		switch (permission)
		{
		case AccessControl.Permission.Both:
			directions = (Grid.Restriction.Directions)0;
			break;
		case AccessControl.Permission.GoLeft:
			directions = Grid.Restriction.Directions.Right;
			break;
		case AccessControl.Permission.GoRight:
			directions = Grid.Restriction.Directions.Left;
			break;
		case AccessControl.Permission.Neither:
			directions = Grid.Restriction.Directions.Left | Grid.Restriction.Directions.Right;
			break;
		}
		if (this.isTeleporter)
		{
			if (directions != (Grid.Restriction.Directions)0)
			{
				directions = Grid.Restriction.Directions.Teleport;
			}
			else
			{
				directions = (Grid.Restriction.Directions)0;
			}
		}
		if (component != null)
		{
			int[] array = this.registeredBuildingCells;
			for (int i = 0; i < array.Length; i++)
			{
				Grid.SetRestriction(array[i], id, directions);
			}
		}
		else
		{
			foreach (CellOffset cellOffset in component2.OccupiedCellsOffsets)
			{
				Grid.SetRestriction(Grid.OffsetCell(Grid.PosToCell(component2), cellOffset), id, directions);
			}
		}
		if (this.isTeleporter)
		{
			Grid.SetRestriction(base.GetComponent<NavTeleporter>().GetCell(), id, directions);
		}
	}

	private void ClearGridRestrictions(KPrefabID kpid)
	{
		if (kpid == null)
		{
			return;
		}
		Building component = base.GetComponent<Building>();
		OccupyArea component2 = base.GetComponent<OccupyArea>();
		if (component2 == null && component == null)
		{
			return;
		}
		int instanceID = kpid.InstanceID;
		if (component != null)
		{
			int[] array = this.registeredBuildingCells;
			for (int i = 0; i < array.Length; i++)
			{
				Grid.ClearRestriction(array[i], instanceID);
			}
			return;
		}
		foreach (CellOffset cellOffset in component2.OccupiedCellsOffsets)
		{
			Grid.ClearRestriction(Grid.OffsetCell(Grid.PosToCell(component2), cellOffset), instanceID);
		}
	}

	private void ClearGridRestrictions(int id, Tag default_id)
	{
		Building component = base.GetComponent<Building>();
		OccupyArea component2 = base.GetComponent<OccupyArea>();
		if (component2 == null && component == null)
		{
			return;
		}
		int num = this.GetTagId(default_id);
		if (id != Tag.Invalid.GetHash())
		{
			num = id;
		}
		if (component != null)
		{
			int[] array = this.registeredBuildingCells;
			for (int i = 0; i < array.Length; i++)
			{
				Grid.ClearRestriction(array[i], num);
			}
			return;
		}
		foreach (CellOffset cellOffset in component2.OccupiedCellsOffsets)
		{
			Grid.ClearRestriction(Grid.OffsetCell(Grid.PosToCell(component2), cellOffset), num);
		}
	}

	public AccessControl.Permission GetSetPermission(MinionAssignablesProxy key)
	{
		return this.GetSetPermission(key.GetComponent<KPrefabID>().InstanceID, key.GetMinionModel());
	}

	public AccessControl.Permission GetSetPermission(Tag robotTag)
	{
		return this.GetSetPermission(this.GetTagId(robotTag), GameTags.Robot);
	}

	public AccessControl.Permission GetSetPermission(int primary_id, Tag secondary_id)
	{
		AccessControl.Permission permission = this.GetDefaultPermission(secondary_id);
		for (int i = 0; i < this.savedPermissionsById.Count; i++)
		{
			if (this.savedPermissionsById[i].Key == primary_id)
			{
				permission = this.savedPermissionsById[i].Value;
				break;
			}
		}
		return permission;
	}

	public void ClearPermission(MinionAssignablesProxy key)
	{
		KPrefabID component = key.GetComponent<KPrefabID>();
		if (component != null)
		{
			this.ClearPermission(component.InstanceID, key.GetMinionModel());
		}
		this.SetStatusItem();
		this.ClearGridRestrictions(component.InstanceID, key.GetMinionModel());
	}

	public void ClearPermission(Tag tag, Tag default_key)
	{
		int tagId = this.GetTagId(tag);
		this.ClearPermission(tagId, default_key);
	}

	private void ClearPermission(int key, Tag default_key)
	{
		for (int i = 0; i < this.savedPermissionsById.Count; i++)
		{
			if (this.savedPermissionsById[i].Key == key)
			{
				this.savedPermissionsById.RemoveAt(i);
				break;
			}
		}
		this.SetStatusItem();
		this.ClearGridRestrictions(key, default_key);
	}

	public bool IsDefaultPermission(MinionAssignablesProxy key)
	{
		KPrefabID component = key.GetComponent<KPrefabID>();
		return !(component != null) || this.IsDefaultPermission(component.InstanceID);
	}

	public bool IsDefaultPermission(Tag robotTag)
	{
		return this.IsDefaultPermission(this.GetTagId(robotTag));
	}

	private bool IsDefaultPermission(int id)
	{
		bool flag = false;
		for (int i = 0; i < this.savedPermissionsById.Count; i++)
		{
			if (this.savedPermissionsById[i].Key == id)
			{
				flag = true;
				break;
			}
		}
		return !flag;
	}

	private void SetStatusItem()
	{
		if (this.overrideAccess == Door.ControlState.Locked)
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.AccessControl, null, null);
			return;
		}
		if (this.defaultPermissionByTag.Any<KeyValuePair<Tag, AccessControl.Permission>>((KeyValuePair<Tag, AccessControl.Permission> default_permission) => default_permission.Value > AccessControl.Permission.Both) || this.savedPermissionsById.Count > 0)
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.AccessControl, AccessControl.accessControlActive, null);
			return;
		}
		this.selectable.SetStatusItem(Db.Get().StatusItemCategories.AccessControl, null, null);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.ACCESS_CONTROL, UI.BUILDINGEFFECTS.TOOLTIPS.ACCESS_CONTROL, Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		return list;
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private bool isTeleporter;

	private int[] registeredBuildingCells;

	[Serialize]
	[Obsolete("Added support for Robots Access Controls, use savedPermissionsById", false)]
	private List<KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>> savedPermissions = new List<KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>>();

	[Serialize]
	[Obsolete("Added support for Robots Access Controls, use defaultPermissionByTag", false)]
	private AccessControl.Permission _defaultPermission;

	[Serialize]
	private List<KeyValuePair<Tag, AccessControl.Permission>> defaultPermissionByTag = new List<KeyValuePair<Tag, AccessControl.Permission>>();

	[Serialize]
	private List<KeyValuePair<int, AccessControl.Permission>> savedPermissionsById = new List<KeyValuePair<int, AccessControl.Permission>>();

	[Serialize]
	public bool registered = true;

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
