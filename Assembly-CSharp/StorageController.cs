using System;

public class StorageController : GameStateMachine<StorageController, StorageController.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.root.EventTransition(GameHashes.OnStorageChange, this.working, null);
		this.off.PlayAnim("off", KAnim.PlayMode.Once, null).EventTransition(GameHashes.OperationalChanged, this.on, (StorageController.Instance smi) => smi.GetComponent<Operational>().IsOperational);
		this.on.PlayAnim("on", KAnim.PlayMode.Once, null).EventTransition(GameHashes.OperationalChanged, this.off, (StorageController.Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		this.working.PlayAnim("working", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.off);
	}

	public GameStateMachine<StorageController, StorageController.Instance, IStateMachineTarget, object>.State off;

	public GameStateMachine<StorageController, StorageController.Instance, IStateMachineTarget, object>.State on;

	public GameStateMachine<StorageController, StorageController.Instance, IStateMachineTarget, object>.State working;

	public new class Instance : GameStateMachine<StorageController, StorageController.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}
}
