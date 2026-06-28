using System;

public class RanchableMonitor : GameStateMachine<RanchableMonitor, RanchableMonitor.Instance, IStateMachineTarget, RanchableMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ToggleBehaviour(GameTags.Creatures.WantsToGetRanched, (RanchableMonitor.Instance smi) => smi.ShouldGoGetRanched(), null);
	}

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<RanchableMonitor, RanchableMonitor.Instance, IStateMachineTarget, RanchableMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, RanchableMonitor.Def def)
			: base(master, def)
		{
		}

		public bool ShouldGoGetRanched()
		{
			return this.targetRanchStation != null && this.targetRanchStation.IsRunning() && this.targetRanchStation.shouldCreatureGoGetRanched;
		}

		public RanchStation.Instance targetRanchStation;
	}
}
