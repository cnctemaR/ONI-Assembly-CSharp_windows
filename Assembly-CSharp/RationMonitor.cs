using System;

public class RationMonitor : GameStateMachine<RationMonitor, RationMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.rationsavailable;
		base.serializable = true;
		this.root.EventHandler(GameHashes.EatCompleteEater, delegate(RationMonitor.Instance smi, object d)
		{
			smi.OnEatComplete(d);
		}).EventHandler(GameHashes.NewDay, (RationMonitor.Instance smi) => GameClock.Instance, delegate(RationMonitor.Instance smi)
		{
			smi.OnNewDay();
		}).ParamTransition<int>(this.dailyRations, this.rationsavailable, (RationMonitor.Instance smi, int p) => smi.HasRationsAvailable())
			.ParamTransition<int>(this.dailyRations, this.outofrations, (RationMonitor.Instance smi, int p) => !smi.HasRationsAvailable())
			.ParamTransition<float>(this.rationsAteToday, this.rationsavailable, (RationMonitor.Instance smi, float p) => smi.HasRationsAvailable())
			.ParamTransition<float>(this.rationsAteToday, this.outofrations, (RationMonitor.Instance smi, float p) => !smi.HasRationsAvailable());
		this.rationsavailable.DefaultState(this.rationsavailable.noediblesavailable);
		this.rationsavailable.noediblesavailable.InitializeStates(this.masterTarget, Db.Get().DuplicantStatusItems.NoRationsAvailable).EventTransition(GameHashes.ColonyHasRationsChanged, (RationMonitor.Instance smi) => SaveGame.Instance, this.rationsavailable.ediblesunreachable, (RationMonitor.Instance smi) => smi.AreThereAnyEdibles());
		this.rationsavailable.ediblesunreachable.InitializeStates(this.masterTarget, Db.Get().DuplicantStatusItems.RationsUnreachable).EventTransition(GameHashes.ColonyHasRationsChanged, (RationMonitor.Instance smi) => SaveGame.Instance, this.rationsavailable.noediblesavailable, (RationMonitor.Instance smi) => !smi.AreThereAnyEdibles()).EventTransition(GameHashes.ClosestEdibleChanged, this.rationsavailable.edibleavailable, (RationMonitor.Instance smi) => smi.IsEdibleAvailable());
		this.rationsavailable.edibleavailable.ToggleChore((RationMonitor.Instance smi) => new EatChore(smi.master), this.rationsavailable.noediblesavailable, false).DefaultState(this.rationsavailable.edibleavailable.readytoeat);
		this.rationsavailable.edibleavailable.readytoeat.EventTransition(GameHashes.ClosestEdibleChanged, this.rationsavailable.noediblesavailable, null).EventTransition(GameHashes.BeginChore, this.rationsavailable.edibleavailable.eating, (RationMonitor.Instance smi) => smi.IsEating());
		this.rationsavailable.edibleavailable.eating.DoNothing();
		this.outofrations.InitializeStates(this.masterTarget, Db.Get().DuplicantStatusItems.DailyRationLimitReached);
	}

	public StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget>.IntParameter dailyRations = new StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget>.IntParameter(16);

	public StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget>.FloatParameter rationsAteToday;

	public StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget>.BoolParameter isRationed;

	public RationMonitor.RationsAvailableState rationsavailable;

	public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget>.HungrySubState outofrations;

	public class EdibleAvailablestate : GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget>.State
	{
		public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget>.State readytoeat;

		public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget>.State eating;
	}

	public class RationsAvailableState : GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget>.State
	{
		public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget>.HungrySubState noediblesavailable;

		public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget>.HungrySubState ediblesunreachable;

		public RationMonitor.EdibleAvailablestate edibleavailable;
	}

	public new class Instance : GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.choreDriver = master.GetComponent<ChoreDriver>();
		}

		public Edible GetEdible()
		{
			return base.GetComponent<Sensors>().GetSensor<ClosestEdibleSensor>().GetEdible();
		}

		public bool AreThereAnyEdibles()
		{
			if (SaveGame.Instance != null)
			{
				ColonyRationMonitor.Instance smi = SaveGame.Instance.GetSMI<ColonyRationMonitor.Instance>();
				if (smi != null)
				{
					return !SaveGame.Instance.GetSMI<ColonyRationMonitor.Instance>().IsOutOfRations();
				}
			}
			return false;
		}

		public bool IsEdibleAvailable()
		{
			return this.GetEdible() != null;
		}

		public bool IsRationed()
		{
			return base.sm.isRationed.Get(base.smi);
		}

		public bool HasRationsAvailable()
		{
			return !this.IsRationed() || this.GetRationsRemaining() > 0f;
		}

		public float GetRationsAteToday()
		{
			return base.sm.rationsAteToday.Get(base.smi);
		}

		public float GetRationsRemaining()
		{
			if (this.IsRationed())
			{
				return Math.Max(0f, (float)base.sm.dailyRations.Get(base.smi) - base.sm.rationsAteToday.Get(base.smi));
			}
			return 10f;
		}

		public int GetDailyRations()
		{
			return base.smi.sm.dailyRations.Get(base.smi);
		}

		public bool IsEating()
		{
			return this.choreDriver.HasChore() && this.choreDriver.GetCurrentChore().choreType.urge == Db.Get().Urges.Eat;
		}

		public void SetDailyRations(int new_rations)
		{
			new_rations = Math.Max(0, new_rations);
			base.smi.sm.dailyRations.Set(new_rations, base.smi);
		}

		public void SetRationed(bool t)
		{
			base.smi.sm.isRationed.Set(t, base.smi);
			if (t)
			{
				base.smi.sm.dailyRations.Set(15, base.smi);
			}
		}

		public void OnNewDay()
		{
			base.smi.sm.rationsAteToday.Set(0f, base.smi);
		}

		public void OnEatComplete(object data)
		{
			Edible edible = (Edible)data;
			base.sm.rationsAteToday.Delta(edible.rationsConsumed, base.smi);
			RationTracker.Get().RegisterRationsConsumed(edible.rationsConsumed);
		}

		private ChoreDriver choreDriver;
	}
}
