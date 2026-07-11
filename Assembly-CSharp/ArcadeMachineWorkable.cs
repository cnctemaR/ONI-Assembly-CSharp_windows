using System;
using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ArcadeMachineWorkable")]
public class ArcadeMachineWorkable : Workable, IWorkerPrioritizable
{
	private ArcadeMachineWorkable()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.synchronizeAnims = false;
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		base.SetWorkTime(15f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		worker.GetComponent<Effects>().Add("ArcadePlaying", false);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		worker.GetComponent<Effects>().Remove("ArcadePlaying");
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(ArcadeMachineWorkable.trackingEffect))
		{
			component.Add(ArcadeMachineWorkable.trackingEffect, true);
		}
		if (!string.IsNullOrEmpty(ArcadeMachineWorkable.specificEffect))
		{
			component.Add(ArcadeMachineWorkable.specificEffect, true);
		}
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = this.basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(ArcadeMachineWorkable.trackingEffect) && component.HasEffect(ArcadeMachineWorkable.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(ArcadeMachineWorkable.specificEffect) && component.HasEffect(ArcadeMachineWorkable.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}

	public ArcadeMachine owner;

	public int basePriority = RELAXATION.PRIORITY.TIER3;

	private static string specificEffect = "PlayedArcade";

	private static string trackingEffect = "RecentlyPlayedArcade";
}
