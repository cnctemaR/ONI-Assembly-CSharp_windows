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
			return num3 >= num;
		}
		return false;
	}

	private bool isCellLadder(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		GameObject gameObject = Grid.Objects[cell, 1];
		return !(gameObject == null) && !(gameObject.GetComponent<Ladder>() == null);
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
			this.root.Update("ClimacophobicCheck", delegate(Climacophobic.StatesInstance smi, float dt)
			{
				if (smi.master.IsUncomfortable())
				{
					smi.GoTo(this.suffering);
					return;
				}
				smi.GoTo(this.satisfied);
			}, UpdateRate.SIM_1000ms, false);
			this.suffering.AddEffect("Vertigo").ToggleExpression(Db.Get().Expressions.Uncomfortable, null);
			this.satisfied.DoNothing();
		}

		public GameStateMachine<Climacophobic.States, Climacophobic.StatesInstance, Climacophobic, object>.State satisfied;

		public GameStateMachine<Climacophobic.States, Climacophobic.StatesInstance, Climacophobic, object>.State suffering;
	}
}
