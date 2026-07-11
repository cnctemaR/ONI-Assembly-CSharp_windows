using System;

public class PoweredController : GameStateMachine<PoweredController, PoweredController.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (PoweredController.Instance smi) => smi.GetComponent<Operational>().IsOperational);
		this.on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, this.off, (PoweredController.Instance smi) => !smi.GetComponent<Operational>().IsOperational);
	}

	public GameStateMachine<PoweredController, PoweredController.Instance, IStateMachineTarget, object>.State off;

	public GameStateMachine<PoweredController, PoweredController.Instance, IStateMachineTarget, object>.State on;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<PoweredController, PoweredController.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, PoweredController.Def def)
			: base(master, def)
		{
		}

		public bool ShowWorkingStatus;
	}
}
