using System;
using Klei.AI;
using UnityEngine;

public class SleepChore : Chore<SleepChore.StatesInstance>
{
	public SleepChore(IStateMachineTarget target, GameObject bed)
		: base(Db.Get().ChoreTypes.Sleep, target, target.GetComponent<ChoreProvider>(), false, null, null, null, int.MaxValue, false, true, 0)
	{
		this.smi = new SleepChore.StatesInstance(this, target.gameObject, bed);
		base.AddPrecondition(ChorePreconditions.IsNotRedAlert, null);
		base.AddPrecondition(SleepChore.IsOkayTimeToSleep, null);
		if (bed != null)
		{
			base.AddPrecondition(ChorePreconditions.IsOperational, bed);
		}
	}

	// Note: this type is marked as 'beforefieldinit'.
	static SleepChore()
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "IsOkayTimeToSleep";
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Narcolepsy component = context.consumer.GetComponent<Narcolepsy>();
			bool flag = component != null && component.IsNarcolepsing();
			StaminaMonitor.Instance smi = context.consumer.GetSMI<StaminaMonitor.Instance>();
			bool flag2 = smi != null && smi.NeedsToSleep();
			bool flag3 = ChorePreconditions.IsScheduledTime.fn(ref context, Db.Get().ScheduleBlockTypes.Sleep);
			return flag || flag3 || flag2;
		};
		SleepChore.IsOkayTimeToSleep = precondition;
	}

	public static Chore.Precondition IsOkayTimeToSleep;

	public class StatesInstance : GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.GameInstance
	{
		public StatesInstance(SleepChore master, GameObject sleeper, GameObject bed)
			: base(master)
		{
			base.sm.sleeper.Set(sleeper, base.smi);
			base.sm.bed.Set(bed, base.smi);
		}

		public void EvaluateSleepQuality()
		{
			if (base.sm.sleepingOnFloor.Get(base.smi))
			{
				base.sm.sleeper.Get<Effects>(base.smi).Add(Db.Get().effects.Get("SoreBack"), true);
			}
		}

		public bool SleepingPeacefully()
		{
			return false;
		}

		public void SetHadNormalSleep()
		{
			if (!this.hadNormalSleep)
			{
				this.stateChangeNoiseSource = GameUtil.GetLoudestNoisePollutorAtCell(Grid.CellAbove(Grid.PosToCell(base.gameObject)));
			}
			this.hadNormalSleep = true;
		}

		public void SetHadBadSleep()
		{
			if (!this.hadBadSleep)
			{
				this.stateChangeNoiseSource = GameUtil.GetLoudestNoisePollutorAtCell(Grid.CellAbove(Grid.PosToCell(base.gameObject)));
			}
			this.hadBadSleep = true;
		}

		public void SetHadTerribleSleep()
		{
			if (!this.hadTerribleSleep)
			{
				this.stateChangeNoiseSource = GameUtil.GetLoudestNoisePollutorAtCell(Grid.CellAbove(Grid.PosToCell(base.gameObject)));
			}
			this.hadTerribleSleep = true;
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
				string text = ((base.sm.sleeper.Get<Navigator>(base.smi).CurrentNavType != NavType.Ladder) ? "anim_sleep_floor_kanim" : "anim_sleep_ladder_kanim");
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

		private int locatorCell;

		private GameObject locator;
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
			}).DefaultState(this.sleep.condition_transition_pre).ToggleEffect("Sleep")
				.DoSleep(this.sleeper, this.bed, this.success, null);
			this.sleep.condition_transition_pre.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Sleep, Db.Get().DuplicantStatusItems.Sleeping, null).QueueAnim("working_loop", true, null).ScheduleGoTo((SleepChore.StatesInstance smi) => smi.wakeUpBuffer, this.sleep.condition_transition);
			this.sleep.condition_transition.Transition(this.sleep.normal, (SleepChore.StatesInstance smi) => smi.timeinstate > smi.wakeUpBuffer && !smi.SleepingPeacefully()).Transition(this.sleep.peaceful, (SleepChore.StatesInstance smi) => smi.timeinstate > smi.wakeUpBuffer && smi.SleepingPeacefully());
			this.sleep.peaceful.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Sleep, Db.Get().DuplicantStatusItems.SleepingPeacefully, null).PlayAnim("trans_peaceful", KAnim.PlayMode.Once, null).QueueAnim("peaceful_loop", true, null)
				.EventTransition(GameHashes.SleepFail, this.sleep.interrupt, null)
				.EventTransition(GameHashes.SleepDisturbed, this.sleep.interrupt_light, null)
				.Transition(this.sleep.normal, (SleepChore.StatesInstance smi) => !smi.SleepingPeacefully())
				.Exit(delegate(SleepChore.StatesInstance smi)
				{
					KAnimControllerBase kanimControllerBase = smi.Get<KAnimControllerBase>();
					if (kanimControllerBase != null)
					{
						kanimControllerBase.Play("trans_working", KAnim.PlayMode.Once, 1f, 0f);
					}
				});
			this.sleep.normal.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Sleep, Db.Get().DuplicantStatusItems.Sleeping, null).QueueAnim("working_loop", true, null).EventTransition(GameHashes.SleepFail, this.sleep.interrupt, null)
				.EventTransition(GameHashes.SleepDisturbed, this.sleep.interrupt_light, null);
			this.sleep.interrupt_light.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Sleep, Db.Get().DuplicantStatusItems.SleepingInterruptedLight, null).QueueAnim("interrupt_light", false, null).EventTransition(GameHashes.AnimQueueComplete, this.sleep.bad, (SleepChore.StatesInstance smi) => smi.timeinstate > 0f && smi.hadBadSleep && !smi.hadTerribleSleep)
				.EventTransition(GameHashes.AnimQueueComplete, this.sleep.terrible, (SleepChore.StatesInstance smi) => smi.timeinstate > 0f && smi.hadTerribleSleep);
			this.sleep.bad.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Sleep, Db.Get().DuplicantStatusItems.SleepingBadly, null).QueueAnim("trans_bad", false, null).QueueAnim("bad_loop", true, null)
				.EventTransition(GameHashes.SleepFail, this.sleep.interrupt, (SleepChore.StatesInstance smi) => smi.timeinstate > smi.wakeUpBuffer)
				.EventTransition(GameHashes.SleepDisturbed, this.sleep.interrupt_light, (SleepChore.StatesInstance smi) => smi.timeinstate > smi.wakeUpBuffer)
				.Exit(delegate(SleepChore.StatesInstance smi)
				{
					KAnimControllerBase kanimControllerBase2 = smi.Get<KAnimControllerBase>();
					if (kanimControllerBase2 != null)
					{
						kanimControllerBase2.Play("trans_bad_working", KAnim.PlayMode.Once, 1f, 0f);
					}
				});
			this.sleep.interrupt.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Sleep, Db.Get().DuplicantStatusItems.SleepingInterrupted, null).QueueAnim("interrupt", false, null).EventTransition(GameHashes.AnimQueueComplete, this.sleep.normal, null);
			this.sleep.terrible.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Sleep, Db.Get().DuplicantStatusItems.SleepingTerribly, null).QueueAnim("trans_terrible", false, null).QueueAnim("terrible_loop", true, null)
				.EventTransition(GameHashes.SleepFail, this.sleep.interrupt, null)
				.EventTransition(GameHashes.SleepDisturbed, this.sleep.interrupt_light, null)
				.Exit(delegate(SleepChore.StatesInstance smi)
				{
					KAnimControllerBase kanimControllerBase3 = smi.Get<KAnimControllerBase>();
					if (kanimControllerBase3 != null)
					{
						kanimControllerBase3.Play("trans_terrible_working", KAnim.PlayMode.Once, 1f, 0f);
					}
				});
			this.success.Enter(delegate(SleepChore.StatesInstance smi)
			{
				smi.EvaluateSleepQuality();
			}).ReturnSuccess();
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

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State peaceful;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State normal;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State bad;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State terrible;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_light;
		}
	}
}
