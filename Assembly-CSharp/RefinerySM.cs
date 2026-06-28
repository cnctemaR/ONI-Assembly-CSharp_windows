using System;

public class RefinerySM : StateMachineComponent<RefinerySM.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	[MyCmpGet]
	private Refinery refinery;

	public class StatesInstance : GameStateMachine<RefinerySM.States, RefinerySM.StatesInstance, RefinerySM, object>.GameInstance
	{
		public StatesInstance(RefinerySM master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<RefinerySM.States, RefinerySM.StatesInstance, RefinerySM>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.idle, (RefinerySM.StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
			this.idle.DefaultState(this.idle.idleQueue).PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.off, (RefinerySM.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational)
				.EventTransition(GameHashes.ActiveChanged, this.operating, (RefinerySM.StatesInstance smi) => smi.GetComponent<Operational>().IsActive);
			this.idle.idleQueue.ToggleStatusItem(Db.Get().BuildingStatusItems.FabricatorEmpty, null).EventTransition(GameHashes.FabricatorOrdersUpdated, this.idle.waitingForMaterial, (RefinerySM.StatesInstance smi) => smi.master.refinery.NumOrders > 0);
			this.idle.waitingForMaterial.EventTransition(GameHashes.FabricatorOrdersUpdated, this.idle.waitingForWorker, (RefinerySM.StatesInstance smi) => smi.master.refinery.NeedsWorker);
			this.idle.waitingForWorker.ToggleStatusItem(Db.Get().BuildingStatusItems.PendingWork, null).EventTransition(GameHashes.FabricatorOrdersUpdated, this.idle.idleQueue, (RefinerySM.StatesInstance smi) => !smi.master.refinery.NeedsWorker);
			this.operating.DefaultState(this.operating.working_pre);
			this.operating.working_pre.PlayAnim("working_pre").OnAnimQueueComplete(this.operating.working_loop);
			this.operating.working_loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, this.operating.working_pst, (RefinerySM.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.ActiveChanged, this.operating.working_pst, (RefinerySM.StatesInstance smi) => !smi.GetComponent<Operational>().IsActive);
			this.operating.working_pst.PlayAnim("working_pst").WorkableCompleteTransition((RefinerySM.StatesInstance smi) => smi.master.refinery, this.operating.working_pst_complete).OnAnimQueueComplete(this.idle);
			this.operating.working_pst_complete.PlayAnim("working_pst_complete").OnAnimQueueComplete(this.idle);
		}

		public RefinerySM.States.IdleStates off;

		public RefinerySM.States.IdleStates idle;

		public RefinerySM.States.OperatingStates operating;

		public class IdleStates : GameStateMachine<RefinerySM.States, RefinerySM.StatesInstance, RefinerySM, object>.State
		{
			public GameStateMachine<RefinerySM.States, RefinerySM.StatesInstance, RefinerySM, object>.State idleQueue;

			public GameStateMachine<RefinerySM.States, RefinerySM.StatesInstance, RefinerySM, object>.State waitingForMaterial;

			public GameStateMachine<RefinerySM.States, RefinerySM.StatesInstance, RefinerySM, object>.State waitingForWorker;
		}

		public class OperatingStates : GameStateMachine<RefinerySM.States, RefinerySM.StatesInstance, RefinerySM, object>.State
		{
			public GameStateMachine<RefinerySM.States, RefinerySM.StatesInstance, RefinerySM, object>.State working_pre;

			public GameStateMachine<RefinerySM.States, RefinerySM.StatesInstance, RefinerySM, object>.State working_loop;

			public GameStateMachine<RefinerySM.States, RefinerySM.StatesInstance, RefinerySM, object>.State working_pst;

			public GameStateMachine<RefinerySM.States, RefinerySM.StatesInstance, RefinerySM, object>.State working_pst_complete;
		}
	}
}
