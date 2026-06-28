using System;

[SkipSaveFileSerialization]
public class SolitarySleeper : StateMachineComponent<SolitarySleeper.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	protected bool IsUncomfortable()
	{
		if (!base.gameObject.GetSMI<StaminaMonitor.Instance>().IsSleeping())
		{
			return false;
		}
		int num = 5;
		bool flag = true;
		bool flag2 = true;
		int num2 = Grid.PosToCell(base.gameObject);
		for (int i = 1; i < num; i++)
		{
			int num3 = Grid.OffsetCell(num2, i, 0);
			int num4 = Grid.OffsetCell(num2, -i, 0);
			if (Grid.Solid[num4])
			{
				flag = false;
			}
			if (Grid.Solid[num3])
			{
				flag2 = false;
			}
			foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
			{
				if (flag && Grid.PosToCell(minionIdentity.gameObject) == num4)
				{
					return true;
				}
				if (flag2 && Grid.PosToCell(minionIdentity.gameObject) == num3)
				{
					return true;
				}
			}
		}
		return false;
	}

	public class StatesInstance : GameStateMachine<SolitarySleeper.States, SolitarySleeper.StatesInstance, SolitarySleeper, object>.GameInstance
	{
		public StatesInstance(SolitarySleeper master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<SolitarySleeper.States, SolitarySleeper.StatesInstance, SolitarySleeper>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.satisfied;
			this.root.EventTransition(GameHashes.Died, null, (SolitarySleeper.StatesInstance smi) => smi.gameObject.GetSMI<DeathMonitor.Instance>().IsDead()).EventTransition(GameHashes.NewDay, this.satisfied, null).ToggleSchedulePeriodic("SolitarySleeperCheck", 6f, delegate(SolitarySleeper.StatesInstance smi)
			{
				if (smi.master.IsUncomfortable())
				{
					if (smi.GetCurrentState() != this.suffering)
					{
						smi.GoTo(this.suffering);
					}
				}
				else if (smi.GetCurrentState() != this.satisfied)
				{
					smi.GoTo(this.satisfied);
				}
			});
			this.suffering.AddEffect("PeopleTooCloseWhileSleeping").ToggleExpression(Db.Get().Expressions.Uncomfortable, null).ToggleSchedulePeriodic("PeopleTooCloseSleepFail", 2f, delegate(SolitarySleeper.StatesInstance smi)
			{
				smi.master.gameObject.Trigger(1338475637, this);
			});
			this.satisfied.DoNothing();
		}

		public GameStateMachine<SolitarySleeper.States, SolitarySleeper.StatesInstance, SolitarySleeper, object>.State satisfied;

		public GameStateMachine<SolitarySleeper.States, SolitarySleeper.StatesInstance, SolitarySleeper, object>.State suffering;
	}
}
