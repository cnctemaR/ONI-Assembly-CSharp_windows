using System;

public class VentController : GameStateMachine<VentController, VentController.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.off.PlayAnim("off").EventTransition(GameHashes.VentAnimatingChanged, this.working_pre, (VentController.Instance smi) => smi.GetComponent<Exhaust>().IsAnimating());
		this.working_pre.PlayAnim("working_pre").OnAnimQueueComplete(this.working_loop);
		this.working_loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.VentAnimatingChanged, this.working_pst, (VentController.Instance smi) => !smi.GetComponent<Exhaust>().IsAnimating());
		this.working_pst.PlayAnim("working_pst").OnAnimQueueComplete(this.off);
	}

	public GameStateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.State off;

	public GameStateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.State working_pre;

	public GameStateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.State working_loop;

	public GameStateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.State working_pst;

	public StateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.BoolParameter isAnimating;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, VentController.Def def)
			: base(master, def)
		{
		}
	}
}
