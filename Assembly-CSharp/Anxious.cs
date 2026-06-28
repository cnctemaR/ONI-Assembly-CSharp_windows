using System;

[SkipSerialization]
public class Anxious : StateMachineComponent<Anxious.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public class StatesInstance : GameStateMachine<Anxious.States, Anxious.StatesInstance, Anxious>.GameInstance
	{
		public StatesInstance(Anxious master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<Anxious.States, Anxious.StatesInstance, Anxious>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.satisfied;
			this.satisfied.DoNothing();
		}

		public GameStateMachine<Anxious.States, Anxious.StatesInstance, Anxious>.State satisfied;
	}
}
