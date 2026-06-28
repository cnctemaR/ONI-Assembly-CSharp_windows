using System;
using UnityEngine;

[SkipSerialization]
public class Aggressive : StateMachineComponent<Aggressive.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public class StatesInstance : GameStateMachine<Aggressive.States, Aggressive.StatesInstance, Aggressive>.GameInstance
	{
		public StatesInstance(Aggressive master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<Aggressive.States, Aggressive.StatesInstance, Aggressive>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.satisfied;
			this.satisfied.EventTransition(GameHashes.Stressed, this.stressed, (Aggressive.StatesInstance smi) => smi.master.GetSMI<StressMonitor.Instance>() != null && smi.master.GetSMI<StressMonitor.Instance>().IsStressed());
			this.stressed.DefaultState(this.stressed.stressed).Transition(this.satisfied, (Aggressive.StatesInstance smi) => smi.master.GetSMI<StressMonitor.Instance>() != null && !smi.master.GetSMI<StressMonitor.Instance>().IsStressed());
			this.stressed.stressed.EventTransition(GameHashes.NewDay, (Aggressive.StatesInstance smi) => GameClock.Instance, this.stressed.hadenough, null);
			this.stressed.hadenough.ScheduleGoTo((Aggressive.StatesInstance smi) => global::UnityEngine.Random.value * 525f, this.stressed.actingout);
			this.stressed.actingout.ToggleUrge(Db.Get().Urges.Aggression).ToggleStatusItem(Db.Get().DuplicantStatusItems.LashingOut, null).EventTransition(GameHashes.FinishedActingOut, this.satisfied, null);
		}

		public GameStateMachine<Aggressive.States, Aggressive.StatesInstance, Aggressive>.State satisfied;

		public Aggressive.States.StressedState stressed;

		public class StressedState : GameStateMachine<Aggressive.States, Aggressive.StatesInstance, Aggressive>.State
		{
			public GameStateMachine<Aggressive.States, Aggressive.StatesInstance, Aggressive>.State stressed;

			public GameStateMachine<Aggressive.States, Aggressive.StatesInstance, Aggressive>.State hadenough;

			public GameStateMachine<Aggressive.States, Aggressive.StatesInstance, Aggressive>.State actingout;
		}
	}
}
