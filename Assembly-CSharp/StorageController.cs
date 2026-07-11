using System;

public class StorageController : GameStateMachine<StorageController, StorageController.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.root.EventTransition(GameHashes.OnStorageInteracted, this.working, null);
		this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (StorageController.Instance smi) => smi.GetComponent<Operational>().IsOperational);
		this.on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, this.off, (StorageController.Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		this.working.PlayAnim("working").OnAnimQueueComplete(this.off);
	}

	public GameStateMachine<StorageController, StorageController.Instance, IStateMachineTarget, object>.State off;

	public GameStateMachine<StorageController, StorageController.Instance, IStateMachineTarget, object>.State on;

	public GameStateMachine<StorageController, StorageController.Instance, IStateMachineTarget, object>.State working;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<StorageController, StorageController.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, StorageController.Def def)
			: base(master)
		{
		}
	}
}
