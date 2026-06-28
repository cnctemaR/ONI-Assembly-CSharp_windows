using System;
using Klei.AI;
using TUNING;

public class Sleepable : Workable
{
	private Sleepable()
	{
		this.showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = null;
	}

	protected override void OnSpawn()
	{
		Components.Sleepables.Add(this);
		base.SetWorkTime(float.PositiveInfinity);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (this.operational != null)
		{
			this.operational.SetActive(true, false);
		}
		worker.Trigger(-1283701846, this);
		worker.GetComponent<Effects>().Add("Sleep", false);
		this.newDayEventHandle = GameClock.Instance.Subscribe(631075836, delegate(object o)
		{
			if (worker.GetAmounts().Get(Db.Get().Amounts.Stamina).value == worker.GetAmounts().Get(Db.Get().Amounts.Stamina).GetMax())
			{
				this.OnCompleteWork(worker);
			}
			else
			{
				this.OnAbortWork(worker);
			}
		});
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
		if (this.operational != null)
		{
			this.operational.SetActive(false, false);
		}
		worker.GetComponent<Effects>().Remove("Sleep");
		if (this.newDayEventHandle != -1)
		{
			GameClock.Instance.Unsubscribe(this.newDayEventHandle);
		}
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

	[MyCmpGet]
	private Operational operational;

	private int newDayEventHandle = -1;
}
