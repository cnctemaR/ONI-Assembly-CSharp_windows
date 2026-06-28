using System;
using UnityEngine;

public class SleepChore : Chore<SleepChore.StatesInstance>
{
	public SleepChore(IStateMachineTarget target, GameObject bed)
		: base(Db.Get().ChoreTypes.Sleep, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0)
	{
		this.smi = new SleepChore.StatesInstance(this, target.gameObject, bed);
		base.AddPrecondition(ChorePreconditions.IsNotRedAlert, null);
		base.AddPrecondition(SleepChore.IsOkayTimeToSleep, null);
		if (bed != null)
		{
			base.AddPrecondition(ChorePreconditions.IsOperational, bed);
		}
	}

	public static Chore.Precondition IsOkayTimeToSleep = new Chore.Precondition
	{
		id = "IsOkayTimeToSleep",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Narcolepsy component = context.consumer.GetComponent<Narcolepsy>();
			bool flag = component != null && component.IsNarcolepsing();
			StaminaMonitor.Instance smi = context.consumer.GetSMI<StaminaMonitor.Instance>();
			bool flag2 = smi != null && smi.NeedsToSleep();
			bool flag3 = ChorePreconditions.IsScheduledTime.fn(ref context, Db.Get().ScheduleBlockTypes.Sleep);
			return flag || flag3 || flag2;
		}
	};

	public class StatesInstance : GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.GameInstance
	{
		public StatesInstance(SleepChore master, GameObject sleeper, GameObject bed)
			: base(master)
		{
			base.sm.sleeper.Set(sleeper, base.smi);
			base.sm.bed.Set(bed, base.smi);
		}

		public void CreateLocator()
		{
			int num = base.sm.sleeper.Get<Sensors>(base.smi).GetSensor<SafeCellSensor>().GetCell();
			if (num == Grid.InvalidCell)
			{
				num = Grid.PosToCell(base.sm.sleeper.Get<Transform>(base.smi).position);
			}
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
			Grid.Reserved[num] = true;
			this.locator = ChoreHelpers.CreateLocator("SleepLocator", vector);
			this.locatorCell = num;
			this.locator.AddComponent<Sleepable>();
			base.sm.bed.Set(this.locator, this);
		}

		public void DestroyLocator()
		{
			if (this.locator != null)
			{
				Grid.Reserved[this.locatorCell] = false;
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
				if (currentNavType != NavType.Ladder && currentNavType != NavType.Pole)
				{
					text = "anim_sleep_floor_kanim";
				}
				else
				{
					text = "anim_sleep_ladder_kanim";
				}
				sleepable.overrideAnims = new KAnimFile[] { Assets.GetAnim(text) };
			}
		}

		public bool hadPeacefulSleep = false;

		public bool hadNormalSleep = false;

		public bool hadBadSleep = false;

		public bool hadTerribleSleep = false;

		public int lastEvaluatedDay = -1;

		public float wakeUpBuffer = 2f;

		public string stateChangeNoiseSource;

		private int locatorCell;

		private GameObject locator = null;
	}

	public class States : GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.check_for_bed;
			base.Target(this.sleeper);
			this.root.Exit("DestroyLocator", delegate(SleepChore.StatesInstance smi)
			{
				smi.DestroyLocator();
			});
			this.check_for_bed.Enter("Check For Bed", delegate(SleepChore.StatesInstance smi)
			{
				this.sleepingOnFloor.Set(false, smi);
				if (this.bed.Get(smi) == null)
				{
					smi.CreateLocator();
					this.sleepingOnFloor.Set(true, smi);
				}
				smi.GoTo(this.approach);
			});
			this.approach.InitializeStates(this.sleeper, this.bed, this.sleep, null, null, null);
			this.sleep.Enter("SetAnims", delegate(SleepChore.StatesInstance smi)
			{
				smi.SetAnim();
			}).DefaultState(this.sleep.normal).ToggleEffect("Sleep")
				.DoSleep(this.sleeper, this.bed, this.success, null);
			this.sleep.normal.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Sleep, Db.Get().DuplicantStatusItems.Sleeping, null).QueueAnim("working_loop", true, null).EventTransition(GameHashes.SleepFail, this.sleep.interrupt, null);
			this.sleep.interrupt.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Sleep, Db.Get().DuplicantStatusItems.SleepingInterrupted, null).QueueAnim("interrupt", false, null).EventTransition(GameHashes.AnimQueueComplete, this.sleep.normal, (SleepChore.StatesInstance smi) => GameClock.Instance.IsNighttime())
				.EventTransition(GameHashes.AnimQueueComplete, this.success, (SleepChore.StatesInstance smi) => !GameClock.Instance.IsNighttime());
			this.success.ReturnSuccess();
		}

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.TargetParameter sleeper;

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.TargetParameter bed;

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter sleepingOnFloor;

		public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State check_for_bed;

		public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.ApproachSubState<Approachable> approach;

		public SleepChore.States.SleepStates sleep;

		public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State success;

		public class SleepStates : GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State
		{
			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State condition_transition;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State condition_transition_pre;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State normal;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt;
		}
	}
}
