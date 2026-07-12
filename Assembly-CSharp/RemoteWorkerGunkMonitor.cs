using System;

public class RemoteWorkerGunkMonitor : StateMachineComponent<RemoteWorkerGunkMonitor.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public float Gunk
	{
		get
		{
			return this.storage.GetMassAvailable(SimHashes.LiquidGunk);
		}
	}

	public float GunkLevel()
	{
		return this.Gunk / 20.000002f;
	}

	[MyCmpGet]
	private Storage storage;

	public const float CAPACITY_KG = 20.000002f;

	public const float HIGH_LEVEL = 16.000002f;

	public const float DRAIN_AMOUNT_KG_PER_S = 3.3333337f;

	public class StatesInstance : GameStateMachine<RemoteWorkerGunkMonitor.States, RemoteWorkerGunkMonitor.StatesInstance, RemoteWorkerGunkMonitor, object>.GameInstance
	{
		public StatesInstance(RemoteWorkerGunkMonitor master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<RemoteWorkerGunkMonitor.States, RemoteWorkerGunkMonitor.StatesInstance, RemoteWorkerGunkMonitor>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.InitializeStates(out default_state);
			default_state = this.ok;
			this.ok.Transition(this.full_gunk, new StateMachine<RemoteWorkerGunkMonitor.States, RemoteWorkerGunkMonitor.StatesInstance, RemoteWorkerGunkMonitor, object>.Transition.ConditionCallback(RemoteWorkerGunkMonitor.States.IsFullOfGunk), UpdateRate.SIM_200ms).Transition(this.high_gunk, new StateMachine<RemoteWorkerGunkMonitor.States, RemoteWorkerGunkMonitor.StatesInstance, RemoteWorkerGunkMonitor, object>.Transition.ConditionCallback(RemoteWorkerGunkMonitor.States.IsGunkHigh), UpdateRate.SIM_200ms);
			this.high_gunk.Transition(this.full_gunk, new StateMachine<RemoteWorkerGunkMonitor.States, RemoteWorkerGunkMonitor.StatesInstance, RemoteWorkerGunkMonitor, object>.Transition.ConditionCallback(RemoteWorkerGunkMonitor.States.IsFullOfGunk), UpdateRate.SIM_200ms).Transition(this.ok, new StateMachine<RemoteWorkerGunkMonitor.States, RemoteWorkerGunkMonitor.StatesInstance, RemoteWorkerGunkMonitor, object>.Transition.ConditionCallback(RemoteWorkerGunkMonitor.States.IsGunkLevelOk), UpdateRate.SIM_200ms).ToggleStatusItem(Db.Get().DuplicantStatusItems.RemoteWorkerHighGunkLevel, null);
			this.full_gunk.Transition(this.high_gunk, new StateMachine<RemoteWorkerGunkMonitor.States, RemoteWorkerGunkMonitor.StatesInstance, RemoteWorkerGunkMonitor, object>.Transition.ConditionCallback(RemoteWorkerGunkMonitor.States.IsGunkHigh), UpdateRate.SIM_200ms).Transition(this.ok, new StateMachine<RemoteWorkerGunkMonitor.States, RemoteWorkerGunkMonitor.StatesInstance, RemoteWorkerGunkMonitor, object>.Transition.ConditionCallback(RemoteWorkerGunkMonitor.States.IsGunkLevelOk), UpdateRate.SIM_200ms).ToggleStatusItem(Db.Get().DuplicantStatusItems.RemoteWorkerFullGunkLevel, null);
		}

		public static bool IsGunkLevelOk(RemoteWorkerGunkMonitor.StatesInstance smi)
		{
			return smi.master.Gunk < 16.000002f;
		}

		public static bool IsGunkHigh(RemoteWorkerGunkMonitor.StatesInstance smi)
		{
			return smi.master.Gunk >= 16.000002f && smi.master.Gunk < 20.000002f;
		}

		public static bool IsFullOfGunk(RemoteWorkerGunkMonitor.StatesInstance smi)
		{
			return smi.master.Gunk >= 20.000002f;
		}

		private GameStateMachine<RemoteWorkerGunkMonitor.States, RemoteWorkerGunkMonitor.StatesInstance, RemoteWorkerGunkMonitor, object>.State ok;

		private GameStateMachine<RemoteWorkerGunkMonitor.States, RemoteWorkerGunkMonitor.StatesInstance, RemoteWorkerGunkMonitor, object>.State high_gunk;

		private GameStateMachine<RemoteWorkerGunkMonitor.States, RemoteWorkerGunkMonitor.StatesInstance, RemoteWorkerGunkMonitor, object>.State full_gunk;
	}
}
