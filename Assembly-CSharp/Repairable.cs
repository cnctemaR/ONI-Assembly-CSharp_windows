using System;

public class Repairable : BuildingWorkable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.SetWorkTime(80f);
		this.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.broken = false;
		this.breakable.Repair();
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingRepair);
		this.chore = null;
		this.userMenu.Refresh();
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo anim = base.GetAnim(worker);
		anim.smi = new MultitoolController.Instance(this, worker, "build", EffectPrefabs.Instance.BuildEffect);
		return anim;
	}

	public void Break()
	{
		this.broken = true;
	}

	private void OnRefreshUserMenu(object data)
	{
		if (this.broken)
		{
			if (this.chore == null)
			{
				this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_repair", "Repair", new global::System.Action(this.OnRepair), global::Action.NumActions, null, null, null, null, string.Empty));
			}
			else
			{
				this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_repair", "Cancel Repair", new global::System.Action(this.OnCancelRepair), global::Action.NumActions, null, null, null, null, string.Empty));
			}
		}
	}

	private void OnRepair()
	{
		if (DebugHandler.InstantBuildMode)
		{
			this.OnCompleteWork(null);
		}
		else
		{
			this.chore = new WorkChore<Repairable>(Db.Get().ChoreTypes.Repair, this, null, true, null, null, null, true, null, false, default(Tag), null, false, true);
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingRepair, this);
		}
	}

	private void OnCancelRepair()
	{
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingRepair);
		if (this.chore != null)
		{
			this.chore.Cancel("Repair cancelled");
		}
		this.chore = null;
	}

	[MyCmpReq]
	private Breakable breakable;

	[MyCmpReq]
	private UserMenu userMenu;

	private Chore chore;

	private bool broken;
}
