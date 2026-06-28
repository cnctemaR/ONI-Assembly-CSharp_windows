using System;
using STRINGS;
using UnityEngine;

public class Breakable : BuildingWorkable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_break") };
		base.SetWorkTime(20f);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.chore = new WorkChore<Breakable>(Db.Get().ChoreTypes.StressActingOut, this, null, true, null, null, null, true, null, true, default(Tag), null, false, false);
		this.chore.AddPrecondition(ChorePreconditions.ConsumerHasTrait, "Aggressive");
	}

	protected override void OnStartWork(Worker worker)
	{
		this.progressBar.barColor = Color.red;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.operational.SetFlag(Breakable.notBrokenFlag, false);
		this.repairable.Break();
		base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.Broken, this);
		base.GetComponent<Notifier>().Add(this.brokenMachine, string.Empty);
		worker.Trigger(-1734580852, null);
		base.GetComponent<UserMenu>().Refresh();
	}

	public void Repair()
	{
		this.operational.SetFlag(Breakable.notBrokenFlag, true);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.Broken);
		base.GetComponent<Notifier>().Remove(this.brokenMachine);
	}

	[MyCmpReq]
	private Repairable repairable;

	[MyCmpReq]
	private Operational operational;

	public static Operational.Flag notBrokenFlag = new Operational.Flag("not_broken", Operational.Flag.Type.Functional);

	private Notification brokenMachine = new Notification(MISC.NOTIFICATIONS.BROKENMACHINE.NAME, NotificationType.BadMinor, null, null, null, true, 0f, null, null, null);

	private Chore chore;
}
