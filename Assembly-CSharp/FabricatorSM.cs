using System;

public class FabricatorSM : StateMachineComponent<FabricatorSM.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	private ComplexFabricator fabricator;

	public class StatesInstance : GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.GameInstance
	{
		public StatesInstance(FabricatorSM master)
			: base(master)
		{
			master.fabricator = master.GetComponent<ComplexFabricator>();
		}
	}

	public class States : GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idleQueue;
			this.root.Transition(this.idleQueue, (FabricatorSM.StatesInstance smi) => smi.master.fabricator.NumOrders == 0, UpdateRate.SIM_200ms);
			this.idleQueue.ToggleStatusItem(Db.Get().BuildingStatusItems.FabricatorEmpty, null).Transition(this.waitingForMaterial, (FabricatorSM.StatesInstance smi) => smi.master.fabricator.NumOrders > 0, UpdateRate.SIM_200ms);
			this.waitingForMaterial.Transition(this.waitingForWorker, (FabricatorSM.StatesInstance smi) => smi.master.fabricator.WaitingForWorker, UpdateRate.SIM_200ms);
			this.waitingForWorker.ToggleStatusItem(Db.Get().BuildingStatusItems.PendingWork, null).Transition(this.waitingForMaterial, (FabricatorSM.StatesInstance smi) => !smi.master.fabricator.WaitingForWorker, UpdateRate.SIM_200ms).Transition(this.operating, (FabricatorSM.StatesInstance smi) => smi.master.fabricator.HasWorker, UpdateRate.SIM_200ms);
			this.operating.Transition(this.waitingForWorker, (FabricatorSM.StatesInstance smi) => !smi.master.fabricator.HasWorker, UpdateRate.SIM_200ms);
		}

		public GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.State idleQueue;

		public GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.State waitingForMaterial;

		public GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.State waitingForWorker;

		public GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.State operating;
	}
}
