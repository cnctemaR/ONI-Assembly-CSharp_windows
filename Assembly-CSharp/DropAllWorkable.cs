using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class DropAllWorkable : Workable
{
	protected DropAllWorkable()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<DropAllWorkable>(493375141, DropAllWorkable.OnRefreshUserMenuDelegate);
		base.Subscribe<DropAllWorkable>(-1697596308, DropAllWorkable.OnStorageChangeDelegate);
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
		this.synchronizeAnims = false;
		base.SetWorkTime(0.1f);
		Prioritizable.AddRef(base.gameObject);
	}

	private Storage[] GetStorages()
	{
		if (this.storages == null)
		{
			this.storages = base.GetComponents<Storage>();
		}
		return this.storages;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.showCmd = this.GetNewShowCmd();
	}

	public void DropAll()
	{
		if (DebugHandler.InstantBuildMode)
		{
			this.OnCompleteWork(null);
			return;
		}
		if (this.chore == null)
		{
			this.chore = new WorkChore<DropAllWorkable>(Db.Get().ChoreTypes.EmptyStorage, this, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			return;
		}
		this.chore.Cancel("Cancelled emptying");
		this.chore = null;
		base.GetComponent<KSelectable>().RemoveStatusItem(this.workerStatusItem, false);
		base.ShowProgressBar(false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage[] array = this.GetStorages();
		for (int i = 0; i < array.Length; i++)
		{
			List<GameObject> list = new List<GameObject>(array[i].items);
			for (int j = 0; j < list.Count; j++)
			{
				GameObject gameObject = array[i].Drop(list[j], true);
				if (gameObject != null)
				{
					Pickupable component = gameObject.GetComponent<Pickupable>();
					if (component != null)
					{
						component.TryToOffsetIfBuried();
					}
				}
			}
		}
		this.chore = null;
		base.Trigger(-1957399615, null);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (this.showCmd)
		{
			KIconButtonMenu.ButtonInfo buttonInfo = ((this.chore == null) ? new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.EMPTYSTORAGE.NAME, new global::System.Action(this.DropAll), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP, true) : new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.EMPTYSTORAGE.NAME_OFF, new global::System.Action(this.DropAll), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP_OFF, true));
			Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
		}
	}

	private bool GetNewShowCmd()
	{
		bool flag = false;
		Storage[] array = this.GetStorages();
		for (int i = 0; i < array.Length; i++)
		{
			flag = flag || !array[i].IsEmpty();
		}
		return flag;
	}

	private void OnStorageChange(object data)
	{
		bool newShowCmd = this.GetNewShowCmd();
		if (newShowCmd != this.showCmd)
		{
			this.showCmd = newShowCmd;
			Game.Instance.userMenu.Refresh(base.gameObject);
		}
	}

	private Chore chore;

	private bool showCmd;

	private Storage[] storages;

	[MyCmpAdd]
	private Prioritizable _prioritizable;

	private static readonly EventSystem.IntraObjectHandler<DropAllWorkable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<DropAllWorkable>(delegate(DropAllWorkable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<DropAllWorkable> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<DropAllWorkable>(delegate(DropAllWorkable component, object data)
	{
		component.OnStorageChange(data);
	});
}
