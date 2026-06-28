using System;

public class DropUnusedInventoryMonitor : GameStateMachine<DropUnusedInventoryMonitor, DropUnusedInventoryMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.EventTransition(GameHashes.OnStorageChange, this.hasinventory, (DropUnusedInventoryMonitor.Instance smi) => smi.GetComponent<Storage>().Count > 0);
		this.hasinventory.EventTransition(GameHashes.OnStorageChange, this.hasinventory, (DropUnusedInventoryMonitor.Instance smi) => smi.GetComponent<Storage>().Count == 0).ToggleChore((DropUnusedInventoryMonitor.Instance smi) => new DropUnusedInventoryChore(Db.Get().ChoreTypes.DropUnusedInventory, smi.master), this.satisfied, false);
	}

	public GameStateMachine<DropUnusedInventoryMonitor, DropUnusedInventoryMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public GameStateMachine<DropUnusedInventoryMonitor, DropUnusedInventoryMonitor.Instance, IStateMachineTarget, object>.State hasinventory;

	public new class Instance : GameStateMachine<DropUnusedInventoryMonitor, DropUnusedInventoryMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}
}
