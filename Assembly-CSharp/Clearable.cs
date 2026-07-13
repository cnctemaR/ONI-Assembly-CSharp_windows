using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/Clearable")]
public class Clearable : Workable, ISaveLoadable, IRender1000ms
{
	protected override void OnPrefabInit()
	{
		base.Subscribe<Clearable>(2127324410, Clearable.OnCancelDelegate);
		base.Subscribe<Clearable>(856640610, Clearable.OnStoreDelegate);
		base.Subscribe<Clearable>(-2064133523, Clearable.OnAbsorbDelegate);
		base.Subscribe<Clearable>(493375141, Clearable.OnRefreshUserMenuDelegate);
		base.Subscribe<Clearable>(-1617557748, Clearable.OnEquippedDelegate);
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Clearing;
		this.simRenderLoadBalance = true;
		this.autoRegisterSimRender = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.isMarkedForClear)
		{
			if (this.pickupable.KPrefabID.HasTag(GameTags.Stored))
			{
				if (!base.transform.parent.GetComponent<Storage>().allowClearable)
				{
					this.isMarkedForClear = false;
				}
				else
				{
					this.MarkForClear(true, true);
				}
			}
			else
			{
				this.MarkForClear(true, false);
			}
		}
		this.RefreshClearableStatus(true);
	}

	private void OnStore(object _)
	{
		this.CancelClearing();
	}

	private void OnCancel(object _)
	{
		for (ObjectLayerListItem objectLayerListItem = this.pickupable.objectLayerListItem; objectLayerListItem != null; objectLayerListItem = objectLayerListItem.nextItem)
		{
			if (objectLayerListItem.pickupable != null)
			{
				objectLayerListItem.pickupable.Clearable.CancelClearing();
			}
		}
	}

	public void CancelClearing()
	{
		if (this.isMarkedForClear)
		{
			this.isMarkedForClear = false;
			base.GetComponent<KPrefabID>().RemoveTag(GameTags.Garbage);
			Prioritizable.RemoveRef(base.gameObject);
			if (this.clearHandle.IsValid())
			{
				GlobalChoreProvider.Instance.UnregisterClearable(this.clearHandle);
				this.clearHandle.Clear();
			}
			this.RefreshClearableStatus(true);
			SimAndRenderScheduler.instance.Remove(this);
		}
	}

	public void MarkForClear(bool restoringFromSave = false, bool allowWhenStored = false)
	{
		if (!this.isClearable)
		{
			return;
		}
		if ((!this.isMarkedForClear || restoringFromSave) && !this.pickupable.IsEntombed && !this.clearHandle.IsValid() && (!this.HasTag(GameTags.Stored) || allowWhenStored))
		{
			Prioritizable.AddRef(base.gameObject);
			this.pickupable.KPrefabID.AddTag(GameTags.Garbage, false);
			this.isMarkedForClear = true;
			this.clearHandle = GlobalChoreProvider.Instance.RegisterClearable(this);
			this.RefreshClearableStatus(true);
			SimAndRenderScheduler.instance.Add(this, this.simRenderLoadBalance);
		}
	}

	private void OnClickClear()
	{
		this.MarkForClear(false, false);
	}

	private void OnClickCancel()
	{
		this.CancelClearing();
	}

	private void OnEquipped(object data)
	{
		this.CancelClearing();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.clearHandle.IsValid())
		{
			GlobalChoreProvider.Instance.UnregisterClearable(this.clearHandle);
			this.clearHandle.Clear();
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.isClearable || base.GetComponent<Health>() != null || this.pickupable.KPrefabID.HasTag(GameTags.Stored) || this.pickupable.KPrefabID.HasTag(GameTags.MarkedForMove))
		{
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo = (this.isMarkedForClear ? new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.CLEAR.NAME_OFF, new global::System.Action(this.OnClickCancel), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.CLEAR.TOOLTIP_OFF, true) : new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.CLEAR.NAME, new global::System.Action(this.OnClickClear), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.CLEAR.TOOLTIP, true));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (pickupable != null)
		{
			Clearable component = pickupable.GetComponent<Clearable>();
			if (component != null && component.isMarkedForClear)
			{
				this.MarkForClear(false, false);
			}
		}
	}

	public void Render1000ms(float dt)
	{
		this.RefreshClearableStatus(false);
	}

	public void RefreshClearableStatus(bool force_update)
	{
		if (force_update || this.isMarkedForClear)
		{
			bool flag = false;
			bool flag2 = false;
			if (this.isMarkedForClear)
			{
				flag2 = !(flag = GlobalChoreProvider.Instance.ClearableHasDestination(this.pickupable));
			}
			this.pendingClearGuid = this.selectable.ToggleStatusItem(Db.Get().MiscStatusItems.PendingClear, this.pendingClearGuid, flag, this);
			this.pendingClearNoStorageGuid = this.selectable.ToggleStatusItem(Db.Get().MiscStatusItems.PendingClearNoStorage, this.pendingClearNoStorageGuid, flag2, this);
		}
	}

	[MyCmpReq]
	private Pickupable pickupable;

	[MyCmpReq]
	private KSelectable selectable;

	[Serialize]
	private bool isMarkedForClear;

	private HandleVector<int>.Handle clearHandle;

	public bool isClearable = true;

	private Guid pendingClearGuid;

	private Guid pendingClearNoStorageGuid;

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnStore(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnAbsorb(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnEquippedDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnEquipped(data);
	});
}
