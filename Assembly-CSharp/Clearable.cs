using System;
using KSerialization;
using STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
public class Clearable : Workable, ISaveLoadable, IRender200ms
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
			if (this.HasTag(GameTags.Stored))
			{
				this.isMarkedForClear = false;
			}
			else
			{
				this.MarkForClear(true);
			}
		}
	}

	private void OnStore(object data)
	{
		this.CancelClearing();
	}

	private void OnCancel(object data)
	{
		for (ObjectLayerListItem objectLayerListItem = this.pickupable.objectLayerListItem; objectLayerListItem != null; objectLayerListItem = objectLayerListItem.nextItem)
		{
			if (objectLayerListItem.gameObject != null)
			{
				objectLayerListItem.gameObject.GetComponent<Clearable>().CancelClearing();
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
			this.RefreshClearableStatus();
			SimAndRenderScheduler.instance.Remove(this);
		}
	}

	public void MarkForClear(bool force = false)
	{
		if (!this.isClearable)
		{
			return;
		}
		if ((!this.isMarkedForClear || force) && !this.pickupable.IsEntombed && !this.clearHandle.IsValid() && !this.HasTag(GameTags.Stored))
		{
			Prioritizable.AddRef(base.gameObject);
			base.GetComponent<KPrefabID>().AddTag(GameTags.Garbage, false);
			this.isMarkedForClear = true;
			this.clearHandle = GlobalChoreProvider.Instance.RegisterClearable(this);
			this.RefreshClearableStatus();
			SimAndRenderScheduler.instance.Add(this, this.simRenderLoadBalance);
		}
	}

	private void OnClickClear()
	{
		this.MarkForClear(false);
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
		if (!this.isClearable || base.GetComponent<Health>() != null || this.HasTag(GameTags.Stored))
		{
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo;
		if (this.isMarkedForClear)
		{
			string text = "action_move_to_storage";
			string text2 = UI.USERMENUACTIONS.CLEAR.NAME_OFF;
			global::System.Action action = new global::System.Action(this.OnClickCancel);
			string text3 = UI.USERMENUACTIONS.CLEAR.TOOLTIP_OFF;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true);
		}
		else
		{
			string text3 = "action_move_to_storage";
			string text2 = UI.USERMENUACTIONS.CLEAR.NAME;
			global::System.Action action = new global::System.Action(this.OnClickClear);
			string text = UI.USERMENUACTIONS.CLEAR.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true);
		}
		KIconButtonMenu.ButtonInfo buttonInfo2 = buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo2, 1f);
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (pickupable != null)
		{
			Clearable component = pickupable.GetComponent<Clearable>();
			if (component != null && component.isMarkedForClear)
			{
				this.MarkForClear(false);
			}
		}
	}

	public void Render200ms(float dt)
	{
		this.RefreshClearableStatus();
	}

	public void RefreshClearableStatus()
	{
		if (this.isMarkedForClear)
		{
			bool flag = GlobalChoreProvider.Instance.ClearableHasDestination(this.pickupable);
			this.selectable.ToggleStatusItem(Db.Get().MiscStatusItems.PendingClear, flag, this);
			this.selectable.ToggleStatusItem(Db.Get().MiscStatusItems.PendingClearNoStorage, !flag, this);
		}
		else
		{
			this.selectable.ToggleStatusItem(Db.Get().MiscStatusItems.PendingClear, false, this);
			this.selectable.ToggleStatusItem(Db.Get().MiscStatusItems.PendingClearNoStorage, false, this);
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
