using System;
using STRINGS;

public class DropAllWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
		this.Subscribe(-1697596308, new EventSystem.EventHandler(this.OnStorageChange));
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
	}

	public void DropAll()
	{
		if (this.chore == null)
		{
			this.chore = new WorkChore<DropAllWorkable>(Db.Get().ChoreTypes.EmptyStorage, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true);
		}
		else
		{
			this.chore.Cancel("Cancelled emptying");
			this.chore = null;
			base.GetComponent<KSelectable>().RemoveStatusItem(this.workerStatusItem);
			base.ShowProgressBar(false);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage[] components = base.GetComponents<Storage>();
		foreach (Storage storage in components)
		{
			storage.DropAll();
		}
		this.chore = null;
		this.Trigger(-1957399615, null);
	}

	private void OnRefreshUserMenu(object data)
	{
		Storage[] components = base.GetComponents<Storage>();
		bool flag = false;
		foreach (Storage storage in components)
		{
			flag = flag || !storage.IsEmpty();
		}
		if (flag)
		{
			if (this.chore == null)
			{
				UserMenu userMenu = this.userMenu;
				string text = UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP;
				userMenu.AddButton(new KIconButtonMenu.ButtonInfo("iconEmptyOut", UI.USERMENUACTIONS.EMPTYSTORAGE.NAME, new global::System.Action(this.DropAll), global::Action.DropAll, null, null, null, null, text));
			}
			else
			{
				UserMenu userMenu2 = this.userMenu;
				string text = UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP_OFF;
				userMenu2.AddButton(new KIconButtonMenu.ButtonInfo("iconEmptyOut", UI.USERMENUACTIONS.EMPTYSTORAGE.NAME_OFF, new global::System.Action(this.DropAll), global::Action.DropAll, null, null, null, null, text));
			}
		}
	}

	private void OnStorageChange(object data)
	{
		this.userMenu.Refresh();
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	private Chore chore;
}
