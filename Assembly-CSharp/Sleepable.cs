using System;
using Klei.AI;

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
		this.synchronizeAnims = false;
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
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return worker.GetSMI<StaminaMonitor.Instance>().ShouldExitSleep();
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		if (this.operational != null)
		{
			this.operational.SetActive(false, false);
		}
		if (worker != null)
		{
			worker.GetComponent<Effects>().Remove("Sleep");
			if (worker.GetAmounts().Get(Db.Get().Amounts.Stamina).value < worker.GetAmounts().Get(Db.Get().Amounts.Stamina).GetMax())
			{
				worker.Trigger(1338475637, this);
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Sleepables.Remove(this);
	}

	[MyCmpGet]
	private Operational operational;
}
