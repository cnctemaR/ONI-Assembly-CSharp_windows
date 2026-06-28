using System;
using KSerialization;
using STRINGS;

public class Dumpable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.isMarkedForDumping)
		{
			this.chore = new WorkChore<Dumpable>(Db.Get().ChoreTypes.EmptyStorage, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true, true);
		}
		base.SetWorkTime(0.1f);
	}

	public void ToggleDumping()
	{
		if (DebugHandler.InstantBuildMode)
		{
			this.OnCompleteWork(null);
		}
		else if (this.isMarkedForDumping)
		{
			this.isMarkedForDumping = false;
			this.chore.Cancel("Cancel Dumping!");
			this.chore = null;
			base.ShowProgressBar(false);
		}
		else
		{
			this.isMarkedForDumping = true;
			this.chore = new WorkChore<Dumpable>(Db.Get().ChoreTypes.EmptyStorage, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true, true);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.isMarkedForDumping = false;
		this.chore = null;
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		SimMessages.AddRemoveSubstance(Grid.PosToCell(this), component.ElementID, CellEventLogger.Instance.Dumpable, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, -1);
		Util.KDestroyGameObject(base.gameObject);
	}

	private void OnRefreshUserMenu(object data)
	{
		Pickupable component = base.GetComponent<Pickupable>();
		if (component != null && component.storage != null)
		{
			return;
		}
		if (!this.isMarkedForDumping)
		{
			UserMenu userMenu = this.userMenu;
			string text = UI.USERMENUACTIONS.DUMP.TOOLTIP;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.DUMP.NAME, new global::System.Action(this.ToggleDumping), global::Action.DropAll, null, null, null, text, true), 1f);
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			string text = UI.USERMENUACTIONS.DUMP.TOOLTIP_OFF;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.DUMP.NAME_OFF, new global::System.Action(this.ToggleDumping), global::Action.DropAll, null, null, null, text, true), 1f);
		}
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	private Chore chore;

	[Serialize]
	private bool isMarkedForDumping;
}
