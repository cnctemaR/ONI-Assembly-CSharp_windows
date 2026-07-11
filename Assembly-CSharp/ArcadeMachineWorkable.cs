using System;
using Klei.AI;
using TUNING;

public class ArcadeMachineWorkable : Workable, IWorkerPrioritizable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.synchronizeAnims = false;
		this.forcePlayPst = true;
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		base.SetWorkTime(15f);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		component.Add("TookABreak", true);
		if (!string.IsNullOrEmpty(this.trackingEffect))
		{
			component.Add(this.trackingEffect, true);
		}
		if (!string.IsNullOrEmpty(this.specificEffect))
		{
			component.Add(this.specificEffect, true);
		}
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = this.basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(this.trackingEffect) && component.HasEffect(this.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(this.specificEffect) && component.HasEffect(this.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}

	protected override void OnStartWork(Worker worker)
	{
		this.owner.AddWorker(worker);
	}

	protected override void OnStopWork(Worker worker)
	{
		this.owner.RemoveWorker(worker);
	}

	public ISharedWorkable owner;

	public int basePriority = RELAXATION.PRIORITY.TIER4;

	public string specificEffect = "PlayedArcade";

	public string trackingEffect = "RecentlyPlayedArcade";
}
