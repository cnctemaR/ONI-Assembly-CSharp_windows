using System;
using Klei.AI;
using UnityEngine;

public class RemoteWorker : StandardWorker
{
	public override Attributes GetAttributes()
	{
		RemoteWorkerDock homeDepot = this.remoteWorkerSM.HomeDepot;
		WorkerBase workerBase = ((homeDepot != null) ? homeDepot.GetActiveTerminalWorker() : null) ?? null;
		if (workerBase != null)
		{
			return workerBase.GetAttributes();
		}
		return null;
	}

	public override AttributeConverterInstance GetAttributeConverter(string id)
	{
		RemoteWorkerDock homeDepot = this.remoteWorkerSM.HomeDepot;
		WorkerBase workerBase = ((homeDepot != null) ? homeDepot.GetActiveTerminalWorker() : null) ?? null;
		if (workerBase != null)
		{
			return workerBase.GetAttributeConverter(id);
		}
		return null;
	}

	protected override void TryPlayingIdle()
	{
		if (this.remoteWorkerSM.Docked)
		{
			base.GetComponent<KAnimControllerBase>().Play("in_dock_idle", KAnim.PlayMode.Once, 1f, 0f);
			return;
		}
		base.TryPlayingIdle();
	}

	protected override void InternalStopWork(Workable target_workable, bool is_aborted)
	{
		base.InternalStopWork(target_workable, is_aborted);
		Vector3 position = base.transform.GetPosition();
		RemoteWorkerSM remoteWorkerSM = this.remoteWorkerSM;
		position.z = Grid.GetLayerZ((remoteWorkerSM != null && remoteWorkerSM.Docked) ? Grid.SceneLayer.BuildingUse : Grid.SceneLayer.Move);
		base.transform.SetPosition(position);
	}

	[MyCmpGet]
	private RemoteWorkerSM remoteWorkerSM;
}
