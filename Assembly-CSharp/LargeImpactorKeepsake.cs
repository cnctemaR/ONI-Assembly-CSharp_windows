using System;

public class LargeImpactorKeepsake : GameStateMachine<LargeImpactorKeepsake, LargeImpactorKeepsake.Instance, IStateMachineTarget, LargeImpactorKeepsake.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.root;
	}

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<LargeImpactorKeepsake, LargeImpactorKeepsake.Instance, IStateMachineTarget, LargeImpactorKeepsake.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, LargeImpactorKeepsake.Def def)
			: base(master, def)
		{
		}
	}
}
