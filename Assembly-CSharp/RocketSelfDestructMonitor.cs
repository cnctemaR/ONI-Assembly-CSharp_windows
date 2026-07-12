using System;

public class RocketSelfDestructMonitor : GameStateMachine<RocketSelfDestructMonitor, RocketSelfDestructMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.idle.EventTransition(GameHashes.RocketSelfDestructRequested, this.exploding, null);
		this.exploding.Update(delegate(RocketSelfDestructMonitor.Instance smi, float dt)
		{
			if (smi.timeinstate >= 3f)
			{
				smi.master.Trigger(-1311384361, null);
				smi.GoTo(this.idle);
			}
		}, UpdateRate.SIM_200ms, false);
	}

	public GameStateMachine<RocketSelfDestructMonitor, RocketSelfDestructMonitor.Instance, IStateMachineTarget, object>.State idle;

	public GameStateMachine<RocketSelfDestructMonitor, RocketSelfDestructMonitor.Instance, IStateMachineTarget, object>.State exploding;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<RocketSelfDestructMonitor, RocketSelfDestructMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, RocketSelfDestructMonitor.Def def)
			: base(master)
		{
		}

		public KBatchedAnimController eyes;
	}
}
