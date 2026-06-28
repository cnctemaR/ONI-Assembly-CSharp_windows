using System;

public class FabricatorSM : StateMachineComponent<FabricatorSM.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	[MyCmpGet]
	private IHasBuildQueue fabricator;

	public class StatesInstance : GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.GameInstance
	{
		public StatesInstance(FabricatorSM master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idleQueue;
			this.root.Transition(this.idleQueue, (FabricatorSM.StatesInstance smi) => smi.master.fabricator.NumOrders == 0);
			this.idleQueue.ToggleStatusItem(Db.Get().BuildingStatusItems.FabricatorEmpty, null).Transition(this.waitingForMaterial, (FabricatorSM.StatesInstance smi) => smi.master.fabricator.NumOrders > 0);
			this.waitingForMaterial.Transition(this.waitingForWorker, (FabricatorSM.StatesInstance smi) => smi.master.fabricator.NeedsWorker);
			this.waitingForWorker.ToggleStatusItem(Db.Get().BuildingStatusItems.PendingWork, null).Transition(this.waitingForMaterial, (FabricatorSM.StatesInstance smi) => !smi.master.fabricator.NeedsWorker).Transition(this.operating, (FabricatorSM.StatesInstance smi) => smi.master.fabricator.HasWorker);
			this.operating.Transition(this.waitingForWorker, (FabricatorSM.StatesInstance smi) => !smi.master.fabricator.HasWorker);
		}

		public GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.State idleQueue;

		public GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.State waitingForMaterial;

		public GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.State waitingForWorker;

		public GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.State operating;
	}
}
