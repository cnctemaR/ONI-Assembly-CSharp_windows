using System;

[SkipSaveFileSerialization]
public class Workaholic : StateMachineComponent<Workaholic.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	protected bool IsUncomfortable()
	{
		return base.smi.master.GetComponent<ChoreDriver>().GetCurrentChore() is IdleChore;
	}

	public class StatesInstance : GameStateMachine<Workaholic.States, Workaholic.StatesInstance, Workaholic, object>.GameInstance
	{
		public StatesInstance(Workaholic master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<Workaholic.States, Workaholic.StatesInstance, Workaholic>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.satisfied;
			this.root.Update("WorkaholicCheck", delegate(Workaholic.StatesInstance smi, float dt)
			{
				if (smi.master.IsUncomfortable())
				{
					smi.GoTo(this.suffering);
					return;
				}
				smi.GoTo(this.satisfied);
			}, UpdateRate.SIM_1000ms, false);
			this.suffering.AddEffect("Restless").ToggleExpression(Db.Get().Expressions.Uncomfortable, null);
			this.satisfied.DoNothing();
		}

		public GameStateMachine<Workaholic.States, Workaholic.StatesInstance, Workaholic, object>.State satisfied;

		public GameStateMachine<Workaholic.States, Workaholic.StatesInstance, Workaholic, object>.State suffering;
	}
}
