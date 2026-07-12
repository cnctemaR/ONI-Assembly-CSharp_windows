using System;

public class WorkerOilRefiller : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_remote_work_dock_kanim") };
		this.workAnims = WorkerOilRefiller.WORK_ANIMS;
		this.workingPstComplete = WorkerOilRefiller.WORK_PST_ANIM;
		this.workingPstFailed = WorkerOilRefiller.WORK_PST_ANIM;
		this.synchronizeAnims = true;
		this.triggerWorkReactions = false;
		this.workLayer = Grid.SceneLayer.BuildingUse;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.RemoteWorkerOiling;
		this.workTime = (this.workTimeRemaining = float.PositiveInfinity);
	}

	protected override bool OnWorkTick(WorkerBase worker, float dt)
	{
		base.OnWorkTick(worker, dt);
		Storage component = worker.GetComponent<Storage>();
		if (component != null)
		{
			float massAvailable = component.GetMassAvailable(GameTags.LubricatingOil);
			float num = Math.Min(60f - massAvailable, 1f * dt);
			this.progressBar.PercentFull = massAvailable / 60f;
			if (num > 0f)
			{
				this.storage.TransferMass(component, GameTags.LubricatingOil, num, false, false, true);
				return false;
			}
		}
		return true;
	}

	private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "oil_pre", "oil_loop" };

	private static readonly HashedString[] WORK_PST_ANIM = new HashedString[] { "oil_pst" };

	[MyCmpGet]
	private Storage storage;
}
