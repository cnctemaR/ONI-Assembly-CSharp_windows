using System;
using Klei.AI;
using UnityEngine;

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
		this.forcePlayPst = true;
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
		this.isDoneSleeping = false;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (this.isDoneSleeping)
		{
			if (Time.time > this.wakeTime)
			{
				return true;
			}
		}
		else if (worker.GetSMI<StaminaMonitor.Instance>().ShouldExitSleep())
		{
			this.isDoneSleeping = true;
			this.wakeTime = Time.time + global::UnityEngine.Random.value * 3f;
		}
		return false;
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

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Sleepables.Remove(this);
	}

	[MyCmpGet]
	private Operational operational;

	private float wakeTime;

	private bool isDoneSleeping;
}
