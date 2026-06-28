using System;
using Klei.AI;

public class StaminaMonitor : GameStateMachine<StaminaMonitor, StaminaMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = true;
		this.root.ToggleStateMachine((StaminaMonitor.Instance smi) => new UrgeMonitor.Instance(smi.master, Db.Get().Urges.Sleep, Db.Get().Amounts.Stamina, Db.Get().ScheduleBlockTypes.Sleep, 100f, 0f, false)).ToggleStateMachine((StaminaMonitor.Instance smi) => new SleepChoreMonitor.Instance(smi.master));
		this.satisfied.Transition(this.sleepy, (StaminaMonitor.Instance smi) => smi.NeedsToSleep() || smi.WantsToSleep());
		this.sleepy.ToggleSchedulePeriodic("Check Sleep State", 1f, delegate(StaminaMonitor.Instance smi)
		{
			smi.TryExitSleepState();
		}).DefaultState(this.sleepy.needssleep);
		this.sleepy.needssleep.Transition(this.sleepy.sleeping, (StaminaMonitor.Instance smi) => smi.IsSleeping()).ToggleExpression(Db.Get().Expressions.Tired, null).ToggleStatusItem(Db.Get().DuplicantStatusItems.Tired, null)
			.ToggleThought(Db.Get().Thoughts.Sleepy, null);
		this.sleepy.sleeping.Transition(this.satisfied, (StaminaMonitor.Instance smi) => !smi.IsSleeping());
	}

	public GameStateMachine<StaminaMonitor, StaminaMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public StaminaMonitor.SleepyState sleepy;

	private const float OUTSIDE_SCHEDULE_STAMINA_THRESHOLD = 0f;

	public class SleepyState : GameStateMachine<StaminaMonitor, StaminaMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<StaminaMonitor, StaminaMonitor.Instance, IStateMachineTarget, object>.State needssleep;

		public GameStateMachine<StaminaMonitor, StaminaMonitor.Instance, IStateMachineTarget, object>.State sleeping;
	}

	public new class Instance : GameStateMachine<StaminaMonitor, StaminaMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.stamina = Db.Get().Amounts.Stamina.Lookup(base.gameObject);
			this.choreDriver = base.GetComponent<ChoreDriver>();
			this.schedulable = base.GetComponent<Schedulable>();
		}

		public bool NeedsToSleep()
		{
			return this.stamina.value <= 0f;
		}

		public bool WantsToSleep()
		{
			return this.choreDriver.HasChore() && this.choreDriver.GetCurrentChore().SatisfiesUrge(Db.Get().Urges.Sleep);
		}

		public void TryExitSleepState()
		{
			if (!this.NeedsToSleep() && !this.WantsToSleep())
			{
				base.smi.GoTo(base.smi.sm.satisfied);
			}
		}

		public bool IsSleeping()
		{
			bool flag = false;
			if (this.WantsToSleep())
			{
				Worker component = this.choreDriver.GetComponent<Worker>();
				Workable workable = component.workable;
				if (workable != null)
				{
					flag = true;
				}
			}
			return flag;
		}

		public bool IsNightTime()
		{
			return TimeOfDay.Instance.GetCurrentTimeRegion() == TimeOfDay.TimeRegion.Night;
		}

		public bool ShouldExitSleep()
		{
			bool flag;
			if (this.schedulable.IsAllowed(Db.Get().ScheduleBlockTypes.Sleep))
			{
				flag = false;
			}
			else
			{
				Narcolepsy component = base.GetComponent<Narcolepsy>();
				flag = (!(component != null) || !component.IsNarcolepsing()) && this.stamina.value >= this.stamina.GetMax() && TimeOfDay.Instance.GetCurrentTimeRegion() != TimeOfDay.TimeRegion.Night;
			}
			return flag;
		}

		private ChoreDriver choreDriver;

		private Schedulable schedulable;

		public AmountInstance stamina;
	}
}
