using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class SleepChore : Chore<SleepChore.StatesInstance>
{
	public SleepChore(ChoreType choreType, IStateMachineTarget target, GameObject bed, bool bedIsLocator, bool isInterruptable)
		: base(choreType, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.emergency, 0, false, true, 0, null)
	{
		this.smi = new SleepChore.StatesInstance(this, target.gameObject, bed, bedIsLocator, isInterruptable);
		if (isInterruptable)
		{
			base.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		}
		base.AddPrecondition(SleepChore.IsOkayTimeToSleep, null);
		Operational component = bed.GetComponent<Operational>();
		if (component != null)
		{
			base.AddPrecondition(ChorePreconditions.instance.IsOperational, component);
		}
	}

	public static Sleepable GetSafeFloorLocator(GameObject sleeper)
	{
		int num = sleeper.GetComponent<Sensors>().GetSensor<SafeCellSensor>().GetCell();
		if (num == Grid.InvalidCell)
		{
			num = Grid.PosToCell(sleeper.transform.GetPosition());
		}
		Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
		GameObject gameObject = ChoreHelpers.CreateSleepLocator(vector);
		return gameObject.GetComponent<Sleepable>();
	}

	public static readonly Chore.Precondition IsOkayTimeToSleep = new Chore.Precondition
	{
		id = "IsOkayTimeToSleep",
		description = DUPLICANTS.CHORES.PRECONDITIONS.IS_OKAY_TIME_TO_SLEEP,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Narcolepsy component = context.consumerState.consumer.GetComponent<Narcolepsy>();
			bool flag = component != null && component.IsNarcolepsing();
			StaminaMonitor.Instance smi = context.consumerState.consumer.GetSMI<StaminaMonitor.Instance>();
			bool flag2 = smi != null && smi.NeedsToSleep();
			bool flag3 = ChorePreconditions.instance.IsScheduledTime.fn(ref context, Db.Get().ScheduleBlockTypes.Sleep);
			return flag || flag3 || flag2;
		}
	};

	public class StatesInstance : GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.GameInstance
	{
		public StatesInstance(SleepChore master, GameObject sleeper, GameObject bed, bool bedIsLocator, bool isInterruptable)
			: base(master)
		{
			base.sm.sleeper.Set(sleeper, base.smi);
			base.sm.isInterruptable.Set(isInterruptable, base.smi);
			if (bedIsLocator)
			{
				this.AddLocator(bed);
			}
			else
			{
				base.sm.bed.Set(bed, base.smi);
			}
		}

		public void EvaluateSleepQuality()
		{
		}

		public void AddLocator(GameObject sleepable)
		{
			this.locator = sleepable;
			Grid.Reserved[Grid.PosToCell(this.locator)] = true;
			base.sm.bed.Set(this.locator, this);
		}

		public void DestroyLocator()
		{
			if (this.locator != null)
			{
				Grid.Reserved[Grid.PosToCell(this.locator)] = false;
				ChoreHelpers.DestroyLocator(this.locator);
				base.sm.bed.Set(null, this);
				this.locator = null;
			}
		}

		public void SetAnim()
		{
			Sleepable sleepable = base.sm.bed.Get<Sleepable>(base.smi);
			if (sleepable.GetComponent<Building>() == null)
			{
				NavType currentNavType = base.sm.sleeper.Get<Navigator>(base.smi).CurrentNavType;
				string text;
				if (currentNavType != NavType.Ladder)
				{
					if (currentNavType != NavType.Pole)
					{
						text = "anim_sleep_floor_kanim";
					}
					else
					{
						text = "anim_sleep_pole_kanim";
					}
				}
				else
				{
					text = "anim_sleep_ladder_kanim";
				}
				sleepable.overrideAnims = new KAnimFile[] { Assets.GetAnim(text) };
			}
		}

		public bool hadPeacefulSleep;

		public bool hadNormalSleep;

		public bool hadBadSleep;

		public bool hadTerribleSleep;

		public int lastEvaluatedDay = -1;

		public float wakeUpBuffer = 2f;

		public string stateChangeNoiseSource;

		private GameObject locator;
	}

	public class States : GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approach;
			base.Target(this.sleeper);
			this.root.Exit("DestroyLocator", delegate(SleepChore.StatesInstance smi)
			{
				smi.DestroyLocator();
			});
			this.approach.InitializeStates(this.sleeper, this.bed, this.sleep, null, null, null);
			this.sleep.Enter("SetAnims", delegate(SleepChore.StatesInstance smi)
			{
				smi.SetAnim();
			}).DefaultState(this.sleep.normal).ToggleTag(GameTags.Asleep)
				.DoSleep(this.sleeper, this.bed, this.success, null)
				.TriggerOnExit(GameHashes.SleepFinished);
			this.sleep.uninterruptable.DoNothing();
			this.sleep.normal.ParamTransition<bool>(this.isInterruptable, this.sleep.uninterruptable, (SleepChore.StatesInstance smi, bool p) => !p).ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Sleep, Db.Get().DuplicantStatusItems.Sleeping, null).QueueAnim("working_loop", true, null)
				.EventTransition(GameHashes.SleepDisturbed, this.sleep.interrupt, null);
			this.sleep.interrupt.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Sleep, Db.Get().DuplicantStatusItems.SleepingInterrupted, null).QueueAnim("interrupt", false, null).OnAnimQueueComplete(this.sleep.interrupt_transition);
			this.sleep.interrupt_transition.Enter(delegate(SleepChore.StatesInstance smi)
			{
				smi.master.GetComponent<Effects>().Add(Db.Get().effects.Get("TerribleSleep"), true);
				GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State state = ((!smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Sleep)) ? this.success : this.sleep.normal);
				smi.GoTo(state);
			});
			this.success.Enter(delegate(SleepChore.StatesInstance smi)
			{
				smi.EvaluateSleepQuality();
			}).ReturnSuccess();
		}

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.TargetParameter sleeper;

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.TargetParameter bed;

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter isInterruptable;

		public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.ApproachSubState<IApproachable> approach;

		public SleepChore.States.SleepStates sleep;

		public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State success;

		public class SleepStates : GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State
		{
			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State condition_transition;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State condition_transition_pre;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State uninterruptable;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State normal;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_transition;
		}
	}
}
