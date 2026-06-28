using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[SkipSerialization]
public class StressVomiter : StateMachineComponent<StressVomiter.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public class StatesInstance : GameStateMachine<StressVomiter.States, StressVomiter.StatesInstance, StressVomiter>.GameInstance
	{
		public StatesInstance(StressVomiter master)
		{
			Func<List<Notification>, object, string> func = (List<Notification> notificationList, object data) => DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false);
			this.vomiting = new Notification(DUPLICANTS.STATUSITEMS.STRESSVOMITING.NOTIFICATION_NAME, NotificationType.Bad, null, func, null, true, 0f, null, null, null);
			base..ctor(master);
		}

		public Notification vomiting;
	}

	public class States : GameStateMachine<StressVomiter.States, StressVomiter.StatesInstance, StressVomiter>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.satisfied;
			this.satisfied.EventTransition(GameHashes.Stressed, this.stressed, (StressVomiter.StatesInstance smi) => smi.master.GetSMI<StressMonitor.Instance>() != null && smi.master.GetSMI<StressMonitor.Instance>().IsStressed());
			this.stressed.DefaultState(this.stressed.stressed).Transition(this.satisfied, (StressVomiter.StatesInstance smi) => smi.master.GetSMI<StressMonitor.Instance>() != null && !smi.master.GetSMI<StressMonitor.Instance>().IsStressed());
			this.stressed.stressed.EventTransition(GameHashes.NewDay, (StressVomiter.StatesInstance smi) => GameClock.Instance, this.stressed.hadenough, null);
			this.stressed.hadenough.ScheduleGoTo((StressVomiter.StatesInstance smi) => global::UnityEngine.Random.value * 525f, this.stressed.actingout);
			this.stressed.actingout.ToggleChore((StressVomiter.StatesInstance smi) => new VomitChore(Db.Get().ChoreTypes.StressVomit, smi.GetComponent<ChoreProvider>(), Db.Get().DuplicantStatusItems.Vomiting, smi.vomiting, null), this.satisfied, false);
		}

		public GameStateMachine<StressVomiter.States, StressVomiter.StatesInstance, StressVomiter>.State satisfied;

		public StressVomiter.States.StressedState stressed;

		public class StressedState : GameStateMachine<StressVomiter.States, StressVomiter.StatesInstance, StressVomiter>.State
		{
			public GameStateMachine<StressVomiter.States, StressVomiter.StatesInstance, StressVomiter>.State stressed;

			public GameStateMachine<StressVomiter.States, StressVomiter.StatesInstance, StressVomiter>.State hadenough;

			public GameStateMachine<StressVomiter.States, StressVomiter.StatesInstance, StressVomiter>.State actingout;
		}
	}
}
