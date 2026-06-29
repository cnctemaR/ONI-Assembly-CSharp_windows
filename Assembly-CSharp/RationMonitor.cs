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
		}).ParamTransition<float>(this.rationsAteToday, this.rationsavailable, (RationMonitor.Instance smi, float p) => smi.HasRationsAvailable())
			.ParamTransition<float>(this.rationsAteToday, this.outofrations, (RationMonitor.Instance smi, float p) => !smi.HasRationsAvailable());
		this.rationsavailable.DefaultState(this.rationsavailable.noediblesavailable);
		this.rationsavailable.noediblesavailable.InitializeStates(this.masterTarget, Db.Get().DuplicantStatusItems.NoRationsAvailable).EventTransition(GameHashes.ColonyHasRationsChanged, new Func<RationMonitor.Instance, KMonoBehaviour>(RationMonitor.GetSaveGame), this.rationsavailable.ediblesunreachable, new StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RationMonitor.AreThereAnyEdibles));
		this.rationsavailable.ediblereachablebutnotpermitted.InitializeStates(this.masterTarget, Db.Get().DuplicantStatusItems.RationsNotPermitted).EventTransition(GameHashes.ColonyHasRationsChanged, new Func<RationMonitor.Instance, KMonoBehaviour>(RationMonitor.GetSaveGame), this.rationsavailable.noediblesavailable, new StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RationMonitor.AreThereNoEdibles)).EventTransition(GameHashes.ClosestEdibleChanged, this.rationsavailable.ediblesunreachable, new StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RationMonitor.NotIsEdibleInReachButNotPermitted));
		this.rationsavailable.ediblesunreachable.InitializeStates(this.masterTarget, Db.Get().DuplicantStatusItems.RationsUnreachable).EventTransition(GameHashes.ColonyHasRationsChanged, new Func<RationMonitor.Instance, KMonoBehaviour>(RationMonitor.GetSaveGame), this.rationsavailable.noediblesavailable, new StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RationMonitor.AreThereNoEdibles)).EventTransition(GameHashes.ClosestEdibleChanged, this.rationsavailable.edibleavailable, new StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RationMonitor.IsEdibleAvailable))
			.EventTransition(GameHashes.ClosestEdibleChanged, this.rationsavailable.ediblereachablebutnotpermitted, new StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RationMonitor.IsEdibleInReachButNotPermitted));
		this.rationsavailable.edibleavailable.ToggleChore((RationMonitor.Instance smi) => new EatChore(smi.master), this.rationsavailable.noediblesavailable).DefaultState(this.rationsavailable.edibleavailable.readytoeat);
		this.rationsavailable.edibleavailable.readytoeat.EventTransition(GameHashes.ClosestEdibleChanged, this.rationsavailable.noediblesavailable, null).EventTransition(GameHashes.BeginChore, this.rationsavailable.edibleavailable.eating, (RationMonitor.Instance smi) => smi.IsEating());
		this.rationsavailable.edibleavailable.eating.DoNothing();
		this.outofrations.InitializeStates(this.masterTarget, Db.Get().DuplicantStatusItems.DailyRationLimitReached);
	}

	private static bool AreThereNoEdibles(RationMonitor.Instance smi)
	{
		return !RationMonitor.AreThereAnyEdibles(smi);
	}

	private static bool AreThereAnyEdibles(RationMonitor.Instance smi)
	{
		if (SaveGame.Instance != null)
		{
			ColonyRationMonitor.Instance smi2 = SaveGame.Instance.GetSMI<ColonyRationMonitor.Instance>();
			if (smi2 != null)
			{
				return !smi2.IsOutOfRations();
			}
		}
		return false;
	}

	private static KMonoBehaviour GetSaveGame(RationMonitor.Instance smi)
	{
		return SaveGame.Instance;
	}

	private static bool IsEdibleAvailable(RationMonitor.Instance smi)
	{
		return smi.GetEdible() != null;
	}

	private static bool NotIsEdibleInReachButNotPermitted(RationMonitor.Instance smi)
	{
		return !RationMonitor.IsEdibleInReachButNotPermitted(smi);
	}

	private static bool IsEdibleInReachButNotPermitted(RationMonitor.Instance smi)
	{
		return smi.GetComponent<Sensors>().GetSensor<ClosestEdibleSensor>().edibleInReachButNotPermitted;
	}

	public StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.FloatParameter rationsAteToday;

	public RationMonitor.RationsAvailableState rationsavailable;

	public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.HungrySubState outofrations;

	public class EdibleAvailablestate : GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State readytoeat;

		public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State eating;
	}

	public class RationsAvailableState : GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.HungrySubState noediblesavailable;

		public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.HungrySubState ediblereachablebutnotpermitted;

		public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.HungrySubState ediblesunreachable;

		public RationMonitor.EdibleAvailablestate edibleavailable;
	}

	public new class Instance : GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.GameInstance
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

		public bool HasRationsAvailable()
		{
			return true;
		}

		public float GetRationsAteToday()
		{
			return base.sm.rationsAteToday.Get(base.smi);
		}

		public float GetRationsRemaining()
		{
			return 1f;
		}

		public bool IsEating()
		{
			return this.choreDriver.HasChore() && this.choreDriver.GetCurrentChore().choreType.urge == Db.Get().Urges.Eat;
		}

		public void OnNewDay()
		{
			base.smi.sm.rationsAteToday.Set(0f, base.smi);
		}

		public void OnEatComplete(object data)
		{
			Edible edible = (Edible)data;
			base.sm.rationsAteToday.Delta(edible.caloriesConsumed, base.smi);
			RationTracker.Get().RegisterRationsConsumed(edible.caloriesConsumed);
		}

		private ChoreDriver choreDriver;
	}
}
