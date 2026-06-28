using System;

[SkipSaveFileSerialization]
public class SensitiveFeet : StateMachineComponent<SensitiveFeet.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	protected bool IsUncomfortable()
	{
		int num = Grid.CellBelow(Grid.PosToCell(base.gameObject));
		return Grid.IsValidCell(num) && Grid.Solid[num] && Grid.Objects[num, 9] == null;
	}

	public class StatesInstance : GameStateMachine<SensitiveFeet.States, SensitiveFeet.StatesInstance, SensitiveFeet, object>.GameInstance
	{
		public StatesInstance(SensitiveFeet master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<SensitiveFeet.States, SensitiveFeet.StatesInstance, SensitiveFeet>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.satisfied;
			this.root.Update("SensitiveFeetCheck", delegate(SensitiveFeet.StatesInstance smi, float dt)
			{
				if (smi.master.IsUncomfortable())
				{
					smi.GoTo(this.suffering);
				}
				else
				{
					smi.GoTo(this.satisfied);
				}
			}, UpdateRate.SIM_1000ms, false);
			this.suffering.AddEffect("UncomfortableFeet").ToggleExpression(Db.Get().Expressions.Uncomfortable, null);
			this.satisfied.DoNothing();
		}

		public GameStateMachine<SensitiveFeet.States, SensitiveFeet.StatesInstance, SensitiveFeet, object>.State satisfied;

		public GameStateMachine<SensitiveFeet.States, SensitiveFeet.StatesInstance, SensitiveFeet, object>.State suffering;
	}
}
