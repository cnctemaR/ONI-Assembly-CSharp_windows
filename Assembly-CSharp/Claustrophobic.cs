using System;

[SkipSaveFileSerialization]
public class Claustrophobic : StateMachineComponent<Claustrophobic.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	protected bool IsUncomfortable()
	{
		int num = 4;
		int num2 = Grid.PosToCell(base.gameObject);
		int i = 0;
		while (i < num - 1)
		{
			int num3 = Grid.OffsetCell(num2, 0, i);
			bool flag;
			if (Grid.IsValidCell(num3) && Grid.Solid[num3])
			{
				flag = true;
			}
			else
			{
				if (!Grid.IsValidCell(Grid.CellRight(num2)) || !Grid.IsValidCell(Grid.CellLeft(num2)) || !Grid.Solid[Grid.CellLeft(num2)] || !Grid.IsValidCell(Grid.CellRight(num2)))
				{
					i++;
					continue;
				}
				flag = true;
			}
			return flag;
		}
		return false;
	}

	public class StatesInstance : GameStateMachine<Claustrophobic.States, Claustrophobic.StatesInstance, Claustrophobic, object>.GameInstance
	{
		public StatesInstance(Claustrophobic master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<Claustrophobic.States, Claustrophobic.StatesInstance, Claustrophobic>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.satisfied;
			this.root.ToggleSchedulePeriodic("ClaustrophobicCheck", 1f, delegate(Claustrophobic.StatesInstance smi)
			{
				if (smi.master.IsUncomfortable())
				{
					smi.GoTo(this.suffering);
				}
				else
				{
					smi.GoTo(this.satisfied);
				}
			});
			this.suffering.AddEffect("Claustrophobic").ToggleExpression(Db.Get().Expressions.Uncomfortable, null);
			this.satisfied.DoNothing();
		}

		public GameStateMachine<Claustrophobic.States, Claustrophobic.StatesInstance, Claustrophobic, object>.State satisfied;

		public GameStateMachine<Claustrophobic.States, Claustrophobic.StatesInstance, Claustrophobic, object>.State suffering;
	}
}
