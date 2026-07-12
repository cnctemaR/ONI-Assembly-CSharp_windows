using System;

public class RemoteWorkerOilMonitor : StateMachineComponent<RemoteWorkerOilMonitor.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public float Oil
	{
		get
		{
			return this.storage.GetMassAvailable(GameTags.LubricatingOil);
		}
	}

	public float OilLevel()
	{
		return this.Oil / 20.000002f;
	}

	[MyCmpGet]
	private Storage storage;

	public const float CAPACITY_KG = 20.000002f;

	public const float LOW_LEVEL = 4.0000005f;

	public const float FILL_RATE_KG_PER_S = 2.5000002f;

	public const float CONSUMPTION_RATE_KG_PER_S = 0.033333335f;

	public class StatesInstance : GameStateMachine<RemoteWorkerOilMonitor.States, RemoteWorkerOilMonitor.StatesInstance, RemoteWorkerOilMonitor, object>.GameInstance
	{
		public StatesInstance(RemoteWorkerOilMonitor master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<RemoteWorkerOilMonitor.States, RemoteWorkerOilMonitor.StatesInstance, RemoteWorkerOilMonitor>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.InitializeStates(out default_state);
			default_state = this.ok;
			this.ok.Transition(this.out_of_oil, new StateMachine<RemoteWorkerOilMonitor.States, RemoteWorkerOilMonitor.StatesInstance, RemoteWorkerOilMonitor, object>.Transition.ConditionCallback(RemoteWorkerOilMonitor.States.IsOutOfOil), UpdateRate.SIM_200ms).Transition(this.low_oil, new StateMachine<RemoteWorkerOilMonitor.States, RemoteWorkerOilMonitor.StatesInstance, RemoteWorkerOilMonitor, object>.Transition.ConditionCallback(RemoteWorkerOilMonitor.States.IsLowOnOil), UpdateRate.SIM_200ms);
			this.low_oil.Transition(this.out_of_oil, new StateMachine<RemoteWorkerOilMonitor.States, RemoteWorkerOilMonitor.StatesInstance, RemoteWorkerOilMonitor, object>.Transition.ConditionCallback(RemoteWorkerOilMonitor.States.IsOutOfOil), UpdateRate.SIM_200ms).Transition(this.ok, new StateMachine<RemoteWorkerOilMonitor.States, RemoteWorkerOilMonitor.StatesInstance, RemoteWorkerOilMonitor, object>.Transition.ConditionCallback(RemoteWorkerOilMonitor.States.IsOkForOil), UpdateRate.SIM_200ms).ToggleStatusItem(Db.Get().DuplicantStatusItems.RemoteWorkerLowOil, null);
			this.out_of_oil.Transition(this.low_oil, new StateMachine<RemoteWorkerOilMonitor.States, RemoteWorkerOilMonitor.StatesInstance, RemoteWorkerOilMonitor, object>.Transition.ConditionCallback(RemoteWorkerOilMonitor.States.IsLowOnOil), UpdateRate.SIM_200ms).Transition(this.ok, new StateMachine<RemoteWorkerOilMonitor.States, RemoteWorkerOilMonitor.StatesInstance, RemoteWorkerOilMonitor, object>.Transition.ConditionCallback(RemoteWorkerOilMonitor.States.IsOkForOil), UpdateRate.SIM_200ms).ToggleStatusItem(Db.Get().DuplicantStatusItems.RemoteWorkerOutOfOil, null);
		}

		public static bool IsOkForOil(RemoteWorkerOilMonitor.StatesInstance smi)
		{
			return smi.master.Oil > 4.0000005f;
		}

		public static bool IsLowOnOil(RemoteWorkerOilMonitor.StatesInstance smi)
		{
			return smi.master.Oil >= float.Epsilon && smi.master.Oil < 4.0000005f;
		}

		public static bool IsOutOfOil(RemoteWorkerOilMonitor.StatesInstance smi)
		{
			return smi.master.Oil < float.Epsilon;
		}

		private GameStateMachine<RemoteWorkerOilMonitor.States, RemoteWorkerOilMonitor.StatesInstance, RemoteWorkerOilMonitor, object>.State ok;

		private GameStateMachine<RemoteWorkerOilMonitor.States, RemoteWorkerOilMonitor.StatesInstance, RemoteWorkerOilMonitor, object>.State low_oil;

		private GameStateMachine<RemoteWorkerOilMonitor.States, RemoteWorkerOilMonitor.StatesInstance, RemoteWorkerOilMonitor, object>.State out_of_oil;
	}
}
