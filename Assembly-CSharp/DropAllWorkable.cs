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
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
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
			this.chore = new WorkChore<DropAllWorkable>(Db.Get().ChoreTypes.EmptyStorage, this, null, null, true, null, null, null, true, null, false, null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue, false);
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
		base.Trigger(-1957399615, null);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (this.showCmd)
		{
			if (this.chore == null)
			{
				UserMenu userMenu = this.userMenu;
				string text = "action_empty_contents";
				string text2 = UI.USERMENUACTIONS.EMPTYSTORAGE.NAME;
				global::System.Action action = new global::System.Action(this.DropAll);
				global::Action action2 = global::Action.BuildingUtility1;
				string text3 = UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP;
				userMenu.AddButton(new KIconButtonMenu.ButtonInfo(text, text2, action, action2, null, null, null, text3, true), 1f);
			}
			else
			{
				UserMenu userMenu2 = this.userMenu;
				string text3 = "action_empty_contents";
				string text2 = UI.USERMENUACTIONS.EMPTYSTORAGE.NAME_OFF;
				global::System.Action action = new global::System.Action(this.DropAll);
				global::Action action2 = global::Action.BuildingUtility1;
				string text = UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP_OFF;
				userMenu2.AddButton(new KIconButtonMenu.ButtonInfo(text3, text2, action, action2, null, null, null, text, true), 1f);
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
