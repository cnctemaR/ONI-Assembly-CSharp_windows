using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class Climacophobic : StateMachineComponent<Climacophobic.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	protected bool IsUncomfortable()
	{
		int num = 5;
		int num2 = Grid.PosToCell(base.gameObject);
		bool flag3;
		if (this.isCellLadder(num2))
		{
			int num3 = 1;
			bool flag = true;
			bool flag2 = true;
			for (int i = 1; i < num; i++)
			{
				int num4 = Grid.OffsetCell(num2, 0, i);
				int num5 = Grid.OffsetCell(num2, 0, -i);
				if (flag && this.isCellLadder(num4))
				{
					num3++;
				}
				else
				{
					flag = false;
				}
				if (flag2 && this.isCellLadder(num5))
				{
					num3++;
				}
				else
				{
					flag2 = false;
				}
			}
			flag3 = num3 >= num;
		}
		else
		{
			flag3 = false;
		}
		return flag3;
	}

	private bool isCellLadder(int cell)
	{
		bool flag;
		if (!Grid.IsValidCell(cell))
		{
			flag = false;
		}
		else
		{
			GameObject gameObject = Grid.Objects[cell, 1];
			flag = !(gameObject == null) && !(gameObject.GetComponent<Ladder>() == null);
		}
		return flag;
	}

	public class StatesInstance : GameStateMachine<Climacophobic.States, Climacophobic.StatesInstance, Climacophobic, object>.GameInstance
	{
		public StatesInstance(Climacophobic master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<Climacophobic.States, Climacophobic.StatesInstance, Climacophobic>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.satisfied;
			this.root.ToggleSchedulePeriodic("ClimacophobicCheck", 1f, delegate(Climacophobic.StatesInstance smi)
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
			this.suffering.AddEffect("Vertigo").ToggleExpression(Db.Get().Expressions.Uncomfortable, null);
			this.satisfied.DoNothing();
		}

		public GameStateMachine<Climacophobic.States, Climacophobic.StatesInstance, Climacophobic, object>.State satisfied;

		public GameStateMachine<Climacophobic.States, Climacophobic.StatesInstance, Climacophobic, object>.State suffering;
	}
}
