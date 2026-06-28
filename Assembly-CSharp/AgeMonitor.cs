using System;

public class AgeMonitor : GameStateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.tame;
		base.serializable = true;
		this.tame.TagTransition(GameTags.Creatures.Wild, this.wild, false);
		this.wild.TagTransition(GameTags.Creatures.Wild, this.tame, true);
	}

	public GameStateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>.State wild;

	public GameStateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>.State tame;

	public class Def : StateMachine.BaseDef
	{
		public float minLifeTimeInCycles;

		public float maxLifeTimeInCycles;
	}

	public new class Instance : GameStateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, AgeMonitor.Def def)
			: base(master, def)
		{
		}
	}
}
