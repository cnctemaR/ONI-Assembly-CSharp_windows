using System;
using TUNING;

public class Sleepable : BuildingWorkable
{
	private Sleepable()
	{
		this.showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Sleeping;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_sleep_bed") };
	}

	protected override void OnSpawn()
	{
		Components.Sleepables.Add(this);
		base.SetWorkTime(float.PositiveInfinity);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.operational.SetActive(true, false);
		worker.Trigger(-1283701846, this);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return worker.GetSMI<StaminaMonitor.Instance>().ShouldExitSleep();
	}

	protected override void OnAbortWork(Worker worker)
	{
		base.OnAbortWork(worker);
		worker.Trigger(1338475637, this);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		this.operational.SetActive(false, false);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Sleepables.Remove(this);
	}

	private void OnRegionChanged(Region new_region)
	{
		GameUtil.UpdateRegion(new_region, this, Db.Get().OwnableSlots.Bed, REGIONS.RoomRegionTag);
	}

	[MyCmpReq]
	private Operational operational;
}
