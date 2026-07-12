using System;

public class ExitableDock : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workAnims = ExitableDock.WORK_ANIMS;
		this.workAnimPlayMode = KAnim.PlayMode.Once;
		this.synchronizeAnims = true;
		this.triggerWorkReactions = false;
		this.workLayer = Grid.SceneLayer.BuildingUse;
	}

	protected override void OnCompleteWork(WorkerBase worker)
	{
		base.OnCompleteWork(worker);
		worker.GetComponent<RemoteWorkerSM>().Docked = false;
	}

	private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "exit_dock" };
}
