using System;

public class WorkerGunkRemover : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_remote_work_dock_kanim") };
		this.workAnims = WorkerGunkRemover.WORK_ANIMS;
		this.workingPstComplete = WorkerGunkRemover.WORK_PST_ANIM;
		this.workingPstFailed = WorkerGunkRemover.WORK_PST_ANIM;
		this.synchronizeAnims = true;
		this.triggerWorkReactions = false;
		this.workLayer = Grid.SceneLayer.BuildingUse;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.RemoteWorkerDraining;
		this.workTime = (this.workTimeRemaining = float.PositiveInfinity);
	}

	protected override bool OnWorkTick(WorkerBase worker, float dt)
	{
		base.OnWorkTick(worker, dt);
		Storage component = worker.GetComponent<Storage>();
		if (component != null)
		{
			float massAvailable = component.GetMassAvailable(SimHashes.LiquidGunk);
			float num = Math.Min(massAvailable, 1f * dt);
			this.progressBar.PercentFull = 1f - massAvailable / 600f;
			if (num > 0f)
			{
				component.TransferMass(this.storage, SimHashes.LiquidGunk.CreateTag(), num, false, false, true);
				return false;
			}
		}
		return true;
	}

	private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "drain_gunk_pre", "drain_gunk_loop" };

	private static readonly HashedString[] WORK_PST_ANIM = new HashedString[] { "drain_gunk_pst" };

	[MyCmpGet]
	private Storage storage;
}
