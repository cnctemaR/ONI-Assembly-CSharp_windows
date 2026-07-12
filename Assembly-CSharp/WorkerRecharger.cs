using System;

public class WorkerRecharger : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workAnims = WorkerRecharger.WORK_ANIMS;
		this.workingPstComplete = WorkerRecharger.WORK_PST_ANIM;
		this.workingPstFailed = WorkerRecharger.WORK_PST_ANIM;
		this.synchronizeAnims = true;
		this.triggerWorkReactions = false;
		this.workLayer = Grid.SceneLayer.BuildingUse;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.RemoteWorkerRecharging;
		this.workTime = (this.workTimeRemaining = float.PositiveInfinity);
	}

	protected override bool OnWorkTick(WorkerBase worker, float dt)
	{
		base.OnWorkTick(worker, dt);
		RemoteWorkerCapacitor component = worker.GetComponent<RemoteWorkerCapacitor>();
		if (component != null)
		{
			this.progressBar.PercentFull = component.ChargeRatio;
			return component.ApplyDeltaEnergy(4f * dt) == 0f;
		}
		return true;
	}

	private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "recharge_pre", "recharge_loop" };

	private static readonly HashedString[] WORK_PST_ANIM = new HashedString[] { "recharge_pst" };
}
