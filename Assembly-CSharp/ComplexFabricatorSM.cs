using System;

public class ComplexFabricatorSM : StateMachineComponent<ComplexFabricatorSM.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	[MyCmpGet]
	private ComplexFabricator fabricator;

	public class StatesInstance : GameStateMachine<ComplexFabricatorSM.States, ComplexFabricatorSM.StatesInstance, ComplexFabricatorSM, object>.GameInstance
	{
		public StatesInstance(ComplexFabricatorSM master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<ComplexFabricatorSM.States, ComplexFabricatorSM.StatesInstance, ComplexFabricatorSM>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.idle, (ComplexFabricatorSM.StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
			this.idle.DefaultState(this.idle.idleQueue).PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.off, (ComplexFabricatorSM.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational)
				.EventTransition(GameHashes.ActiveChanged, this.operating, (ComplexFabricatorSM.StatesInstance smi) => smi.GetComponent<Operational>().IsActive);
			this.idle.idleQueue.ToggleStatusItem(Db.Get().BuildingStatusItems.FabricatorEmpty, null).EventTransition(GameHashes.FabricatorOrdersUpdated, this.idle.waitingForMaterial, (ComplexFabricatorSM.StatesInstance smi) => smi.master.fabricator.NumOrders > 0);
			this.idle.waitingForMaterial.EventTransition(GameHashes.FabricatorOrdersUpdated, this.idle.waitingForWorker, (ComplexFabricatorSM.StatesInstance smi) => smi.master.fabricator.WaitingForWorker);
			this.idle.waitingForWorker.ToggleStatusItem(Db.Get().BuildingStatusItems.PendingWork, null).EventTransition(GameHashes.FabricatorOrdersUpdated, this.idle.idleQueue, (ComplexFabricatorSM.StatesInstance smi) => !smi.master.fabricator.WaitingForWorker).EnterTransition(this.operating, (ComplexFabricatorSM.StatesInstance smi) => !smi.master.fabricator.duplicantOperated);
			this.operating.DefaultState(this.operating.working_pre);
			this.operating.working_pre.PlayAnim("working_pre").OnAnimQueueComplete(this.operating.working_loop);
			this.operating.working_loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, this.operating.working_pst, (ComplexFabricatorSM.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.ActiveChanged, this.operating.working_pst, (ComplexFabricatorSM.StatesInstance smi) => !smi.GetComponent<Operational>().IsActive);
			this.operating.working_pst.PlayAnim("working_pst").WorkableCompleteTransition((ComplexFabricatorSM.StatesInstance smi) => smi.master.fabricator.Workable, this.operating.working_pst_complete).OnAnimQueueComplete(this.idle);
			this.operating.working_pst_complete.PlayAnim("working_pst_complete").OnAnimQueueComplete(this.idle);
		}

		public ComplexFabricatorSM.States.IdleStates off;

		public ComplexFabricatorSM.States.IdleStates idle;

		public ComplexFabricatorSM.States.OperatingStates operating;

		public class IdleStates : GameStateMachine<ComplexFabricatorSM.States, ComplexFabricatorSM.StatesInstance, ComplexFabricatorSM, object>.State
		{
			public GameStateMachine<ComplexFabricatorSM.States, ComplexFabricatorSM.StatesInstance, ComplexFabricatorSM, object>.State idleQueue;

			public GameStateMachine<ComplexFabricatorSM.States, ComplexFabricatorSM.StatesInstance, ComplexFabricatorSM, object>.State waitingForMaterial;

			public GameStateMachine<ComplexFabricatorSM.States, ComplexFabricatorSM.StatesInstance, ComplexFabricatorSM, object>.State waitingForWorker;
		}

		public class OperatingStates : GameStateMachine<ComplexFabricatorSM.States, ComplexFabricatorSM.StatesInstance, ComplexFabricatorSM, object>.State
		{
			public GameStateMachine<ComplexFabricatorSM.States, ComplexFabricatorSM.StatesInstance, ComplexFabricatorSM, object>.State working_pre;

			public GameStateMachine<ComplexFabricatorSM.States, ComplexFabricatorSM.StatesInstance, ComplexFabricatorSM, object>.State working_loop;

			public GameStateMachine<ComplexFabricatorSM.States, ComplexFabricatorSM.StatesInstance, ComplexFabricatorSM, object>.State working_pst;

			public GameStateMachine<ComplexFabricatorSM.States, ComplexFabricatorSM.StatesInstance, ComplexFabricatorSM, object>.State working_pst_complete;
		}
	}
}
