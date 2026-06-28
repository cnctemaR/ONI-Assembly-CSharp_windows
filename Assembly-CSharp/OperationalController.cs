using System;

public class OperationalController : GameStateMachine<OperationalController, OperationalController.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.root.EventTransition(GameHashes.OperationalChanged, this.off, (OperationalController.Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		this.off.PlayAnim("off", KAnim.PlayMode.Once, null).EventTransition(GameHashes.OperationalChanged, this.working_pre, (OperationalController.Instance smi) => smi.GetComponent<Operational>().IsOperational);
		this.working_pre.PlayAnim("working_pre", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.working_loop);
		this.working_loop.PlayAnim("working_loop", KAnim.PlayMode.Loop, null).EventTransition(GameHashes.OperationalChanged, this.working_pst, (OperationalController.Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		this.working_pst.PlayAnim("working_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.off);
	}

	public GameStateMachine<OperationalController, OperationalController.Instance, IStateMachineTarget>.State off;

	public GameStateMachine<OperationalController, OperationalController.Instance, IStateMachineTarget>.State working_pre;

	public GameStateMachine<OperationalController, OperationalController.Instance, IStateMachineTarget>.State working_loop;

	public GameStateMachine<OperationalController, OperationalController.Instance, IStateMachineTarget>.State working_pst;

	public new class Instance : GameStateMachine<OperationalController, OperationalController.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}
}
