using System;

public class ActiveController : GameStateMachine<ActiveController, ActiveController.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.off.PlayAnim("off", KAnim.PlayMode.Once, null).EventTransition(GameHashes.ActiveChanged, this.working_pre, (ActiveController.Instance smi) => smi.GetComponent<Operational>().IsActive);
		this.working_pre.PlayAnim("working_pre", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.working_loop);
		this.working_loop.PlayAnim("working_loop", KAnim.PlayMode.Loop, null).EventTransition(GameHashes.ActiveChanged, this.working_pst, (ActiveController.Instance smi) => !smi.GetComponent<Operational>().IsActive);
		this.working_pst.PlayAnim("working_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.off);
	}

	public GameStateMachine<ActiveController, ActiveController.Instance, IStateMachineTarget, object>.State off;

	public GameStateMachine<ActiveController, ActiveController.Instance, IStateMachineTarget, object>.State working_pre;

	public GameStateMachine<ActiveController, ActiveController.Instance, IStateMachineTarget, object>.State working_loop;

	public GameStateMachine<ActiveController, ActiveController.Instance, IStateMachineTarget, object>.State working_pst;

	public new class Instance : GameStateMachine<ActiveController, ActiveController.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}
}
