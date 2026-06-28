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
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
		this.synchronizeAnims = false;
		base.SetWorkTime(0.1f);
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
		}
		else if (this.chore == null)
		{
			this.chore = new WorkChore<DropAllWorkable>(Db.Get().ChoreTypes.EmptyStorage, this, null, true, null, null, null, true, null, false, default(Tag), null, false, true, true, int.MaxValue);
		}
		else
		{
			this.chore.Cancel("Cancelled emptying");
			this.chore = null;
			base.GetComponent<KSelectable>().RemoveStatusItem(this.workerStatusItem, false);
			base.ShowProgressBar(false);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage[] array = this.GetStorages();
		for (int i = 0; i < array.Length; i++)
		{
			List<GameObject> list = new List<GameObject>(array[i].items);
			for (int j = 0; j < list.Count; j++)
			{
				GameObject gameObject = array[i].Drop(list[j]);
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
		this.Trigger(-1957399615, null);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (this.showCmd)
		{
			if (this.chore == null)
			{
				UserMenu userMenu = this.userMenu;
				string text = UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP;
				userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.EMPTYSTORAGE.NAME, new global::System.Action(this.DropAll), global::Action.BuildingUtility1, null, null, null, text, true), 1f);
			}
			else
			{
				UserMenu userMenu2 = this.userMenu;
				string text = UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP_OFF;
				userMenu2.AddButton(new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.EMPTYSTORAGE.NAME_OFF, new global::System.Action(this.DropAll), global::Action.BuildingUtility1, null, null, null, text, true), 1f);
			}
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
			this.userMenu.Refresh();
		}
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	private Chore chore;

	private bool showCmd;

	private Storage[] storages;
}
