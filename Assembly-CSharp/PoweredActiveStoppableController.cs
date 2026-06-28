using System;

public class PoweredActiveStoppableController : GameStateMachine<PoweredActiveStoppableController, PoweredActiveStoppableController.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.off.PlayAnim("off", KAnim.PlayMode.Once, null).EventTransition(GameHashes.ActiveChanged, this.working_pre, (PoweredActiveStoppableController.Instance smi) => smi.GetComponent<Operational>().IsActive);
		this.working_pre.PlayAnim("working_pre", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.working_loop);
		this.working_loop.PlayAnim("working_loop", KAnim.PlayMode.Loop, null).EventTransition(GameHashes.OperationalChanged, this.stop, (PoweredActiveStoppableController.Instance smi) => !smi.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.ActiveChanged, this.working_pst, (PoweredActiveStoppableController.Instance smi) => !smi.GetComponent<Operational>().IsActive);
		this.working_pst.PlayAnim("working_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.off);
		this.stop.PlayAnim("stop", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.off);
	}

	public GameStateMachine<PoweredActiveStoppableController, PoweredActiveStoppableController.Instance, IStateMachineTarget, object>.State off;

	public GameStateMachine<PoweredActiveStoppableController, PoweredActiveStoppableController.Instance, IStateMachineTarget, object>.State working_pre;

	public GameStateMachine<PoweredActiveStoppableController, PoweredActiveStoppableController.Instance, IStateMachineTarget, object>.State working_loop;

	public GameStateMachine<PoweredActiveStoppableController, PoweredActiveStoppableController.Instance, IStateMachineTarget, object>.State working_pst;

	public GameStateMachine<PoweredActiveStoppableController, PoweredActiveStoppableController.Instance, IStateMachineTarget, object>.State stop;

	public new class Instance : GameStateMachine<PoweredActiveStoppableController, PoweredActiveStoppableController.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}
}
