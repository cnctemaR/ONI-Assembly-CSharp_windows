using System;
using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/HotTubWorkable")]
public class HotTubWorkable : Workable, IWorkerPrioritizable
{
	private HotTubWorkable()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.synchronizeAnims = false;
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.faceTargetWhenWorking = true;
		base.SetWorkTime(90f);
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo anim = base.GetAnim(worker);
		anim.smi = new HotTubWorkerStateMachine.StatesInstance(worker);
		return anim;
	}

	protected override void OnStartWork(Worker worker)
	{
		this.faceLeft = global::UnityEngine.Random.value > 0.5f;
		worker.GetComponent<Effects>().Add("HotTubRelaxing", false);
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.GetComponent<Effects>().Remove("HotTubRelaxing");
	}

	public override Vector3 GetFacingTarget()
	{
		return base.transform.GetPosition() + (this.faceLeft ? Vector3.left : Vector3.right);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(this.hotTub.trackingEffect))
		{
			component.Add(this.hotTub.trackingEffect, true);
		}
		if (!string.IsNullOrEmpty(this.hotTub.specificEffect))
		{
			component.Add(this.hotTub.specificEffect, true);
		}
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = this.hotTub.basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(this.hotTub.trackingEffect) && component.HasEffect(this.hotTub.trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(this.hotTub.specificEffect) && component.HasEffect(this.hotTub.specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}

	public HotTub hotTub;

	private bool faceLeft;
}
