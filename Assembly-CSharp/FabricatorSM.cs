using System;

public class FabricatorSM : StateMachineComponent<FabricatorSM.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	[MyCmpGet]
	private Fabricator fabricator;

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
			this.root.Transition(this.idleQueue, (FabricatorSM.StatesInstance smi) => smi.master.fabricator.Orders.Count == 0);
			this.idleQueue.ToggleStatusItem(Db.Get().BuildingStatusItems.FabricatorEmpty, null).Transition(this.waitingForMaterial, (FabricatorSM.StatesInstance smi) => smi.master.fabricator.Orders.Count > 0);
			this.waitingForMaterial.Transition(this.waitingForWorker, delegate(FabricatorSM.StatesInstance smi)
			{
				if (smi.master.fabricator.MachineOrders.Count > 0)
				{
					Fabricator.MachineOrder machineOrder = smi.master.fabricator.MachineOrders[0];
					if (machineOrder.fetchList != null && machineOrder.fetchList.IsComplete && smi.master.fabricator.worker == null)
					{
						return true;
					}
				}
				return false;
			});
			this.waitingForWorker.ToggleStatusItem(Db.Get().BuildingStatusItems.PendingWork, null).Transition(this.waitingForMaterial, delegate(FabricatorSM.StatesInstance smi)
			{
				if (smi.master.fabricator.MachineOrders.Count > 0)
				{
					Fabricator.MachineOrder machineOrder2 = smi.master.fabricator.MachineOrders[0];
					if (machineOrder2.fetchList != null && !machineOrder2.fetchList.IsComplete)
					{
						return true;
					}
				}
				return false;
			}).Transition(this.operating, (FabricatorSM.StatesInstance smi) => smi.master.fabricator.worker != null);
			this.operating.Transition(this.waitingForWorker, (FabricatorSM.StatesInstance smi) => smi.master.fabricator.worker == null);
		}

		public GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.State idleQueue;

		public GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.State waitingForMaterial;

		public GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.State waitingForWorker;

		public GameStateMachine<FabricatorSM.States, FabricatorSM.StatesInstance, FabricatorSM, object>.State operating;
	}
}
