using System;

public class FloodableMonitor : GameStateMachine<FloodableMonitor, FloodableMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = false;
		this.satisfied.DoNothing();
		this.flooded.DoNothing();
	}

	public GameStateMachine<FloodableMonitor, FloodableMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public GameStateMachine<FloodableMonitor, FloodableMonitor.Instance, IStateMachineTarget, object>.State flooded;

	public new class Instance : GameStateMachine<FloodableMonitor, FloodableMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}
}
