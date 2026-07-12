using System;

public class EnterableDock : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.EnteringDock;
		this.workAnims = EnterableDock.WORK_ANIMS;
		this.workAnimPlayMode = KAnim.PlayMode.Once;
		this.synchronizeAnims = true;
		this.triggerWorkReactions = false;
		this.workLayer = Grid.SceneLayer.BuildingUse;
	}

	protected override void OnCompleteWork(WorkerBase worker)
	{
		worker.GetComponent<RemoteWorkerSM>().Docked = true;
		base.OnCompleteWork(worker);
	}

	private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "enter_dock" };
}
