using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class WatchRoboDancerWorkable : Workable, IWorkerPrioritizable
{
	private WatchRoboDancerWorkable()
	{
		base.SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.synchronizeAnims = false;
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.WatchRoboDancerWorkable;
		base.SetWorkTime(30f);
		this.showProgressBar = false;
	}

	protected override void OnCompleteWork(WorkerBase worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(WatchRoboDancerWorkable.TRACKING_EFFECT))
		{
			component.Add(WatchRoboDancerWorkable.TRACKING_EFFECT, true);
		}
		if (!string.IsNullOrEmpty(WatchRoboDancerWorkable.SPECIFIC_EFFECT))
		{
			component.Add(WatchRoboDancerWorkable.SPECIFIC_EFFECT, true);
		}
	}

	public bool GetWorkerPriority(WorkerBase worker, out int priority)
	{
		priority = this.basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(WatchRoboDancerWorkable.TRACKING_EFFECT) && component.HasEffect(WatchRoboDancerWorkable.TRACKING_EFFECT))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(WatchRoboDancerWorkable.SPECIFIC_EFFECT) && component.HasEffect(WatchRoboDancerWorkable.SPECIFIC_EFFECT))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}

	protected override void OnStartWork(WorkerBase worker)
	{
		worker.GetComponent<Effects>().Add("Dancing", false);
	}

	protected override bool OnWorkTick(WorkerBase worker, float dt)
	{
		worker.GetComponent<Facing>().Face(this.owner.transform.position.x);
		return base.OnWorkTick(worker, dt);
	}

	protected override void OnStopWork(WorkerBase worker)
	{
		worker.GetComponent<Effects>().Remove("Dancing");
		ChoreHelpers.DestroyLocator(base.gameObject);
	}

	public override Workable.AnimInfo GetAnim(WorkerBase worker)
	{
		int num = global::UnityEngine.Random.Range(0, this.workerOverrideAnims.Length);
		this.overrideAnims = this.workerOverrideAnims[num];
		return base.GetAnim(worker);
	}

	public GameObject owner;

	public int basePriority = RELAXATION.PRIORITY.TIER3;

	public static string SPECIFIC_EFFECT = "SawRoboDancer";

	public static string TRACKING_EFFECT = "RecentlySawRoboDancer";

	public KAnimFile[][] workerOverrideAnims = new KAnimFile[][]
	{
		new KAnimFile[] { Assets.GetAnim("anim_interacts_robotdance_kanim") },
		new KAnimFile[] { Assets.GetAnim("anim_interacts_robotdance1_kanim") }
	};
}
