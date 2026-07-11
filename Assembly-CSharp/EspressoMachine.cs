using System;

public class EspressoMachine : StateMachineComponent<EspressoMachine.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public static Tag INGREDIENT_TAG = new Tag("SpiceNut");

	public class States : GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.unoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational, false);
			this.operational.PlayAnim("off").TagTransition(GameTags.Operational, this.unoperational, true).Transition(this.ready, new StateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.Transition.ConditionCallback(this.IsReady), UpdateRate.SIM_200ms)
				.EventTransition(GameHashes.OnStorageChange, this.ready, new StateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.Transition.ConditionCallback(this.IsReady));
			this.ready.TagTransition(GameTags.Operational, this.unoperational, true).DefaultState(this.ready.idle).ToggleChore(new Func<EspressoMachine.StatesInstance, Chore>(this.CreateChore), this.operational);
			this.ready.idle.PlayAnim("on", KAnim.PlayMode.Loop).WorkableStartTransition((EspressoMachine.StatesInstance smi) => smi.master.GetComponent<EspressoMachineWorkable>(), this.ready.working).Transition(this.operational, GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.Not(new StateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.Transition.ConditionCallback(this.IsReady)), UpdateRate.SIM_200ms)
				.EventTransition(GameHashes.OnStorageChange, this.operational, GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.Not(new StateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.Transition.ConditionCallback(this.IsReady)));
			this.ready.working.PlayAnim("working_pre").QueueAnim("working_loop", true, null).WorkableStopTransition((EspressoMachine.StatesInstance smi) => smi.master.GetComponent<EspressoMachineWorkable>(), this.ready.post);
			this.ready.post.PlayAnim("working_pst").OnAnimQueueComplete(this.ready);
		}

		private Chore CreateChore(EspressoMachine.StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<EspressoMachineWorkable>();
			ChoreType relax = Db.Get().ChoreTypes.Relax;
			Workable workable = component;
			ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
			Chore chore = new WorkChore<EspressoMachineWorkable>(relax, workable, null, null, true, null, null, null, false, recreation, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 0, false);
			chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, component);
			return chore;
		}

		private bool IsReady(EspressoMachine.StatesInstance smi)
		{
			PrimaryElement primaryElement = smi.GetComponent<Storage>().FindPrimaryElement(SimHashes.Water);
			if (primaryElement == null)
			{
				return false;
			}
			if (primaryElement.Mass < 1f)
			{
				return false;
			}
			float amountAvailable = smi.GetComponent<Storage>().GetAmountAvailable(EspressoMachine.INGREDIENT_TAG);
			return amountAvailable >= 1f;
		}

		private GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State unoperational;

		private GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State operational;

		private EspressoMachine.States.ReadyStates ready;

		public class ReadyStates : GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State
		{
			public GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State idle;

			public GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State working;

			public GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State post;
		}
	}

	public class StatesInstance : GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.GameInstance
	{
		public StatesInstance(EspressoMachine smi)
			: base(smi)
		{
		}
	}
}
