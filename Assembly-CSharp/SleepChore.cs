using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class SleepChore : Chore<SleepChore.StatesInstance>
{
	public SleepChore(ChoreType choreType, IStateMachineTarget target, GameObject bed, bool bedIsLocator, bool isInterruptable)
		: base(choreType, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.PersonalTime)
	{
		base.smi = new SleepChore.StatesInstance(this, target.gameObject, bed, bedIsLocator, isInterruptable);
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
		int num = sleeper.GetComponent<Sensors>().GetSensor<SafeCellSensor>().GetSleepCellQuery();
		if (num == Grid.InvalidCell)
		{
			num = Grid.PosToCell(sleeper.transform.GetPosition());
		}
		return ChoreHelpers.CreateSleepLocator(Grid.CellToPosCBC(num, Grid.SceneLayer.Move)).GetComponent<Sleepable>();
	}

	public static bool IsDarkAtCell(int cell)
	{
		return Grid.LightIntensity[cell] <= 0;
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
			Traits component = sleeper.GetComponent<Traits>();
			if (component != null)
			{
				base.sm.needsNightLight.Set(component.HasTrait("NightLight"), base.smi);
			}
			if (bedIsLocator)
			{
				this.AddLocator(bed);
				return;
			}
			base.sm.bed.Set(bed, base.smi);
		}

		public void CheckLightLevel()
		{
			GameObject gameObject = base.sm.sleeper.Get(base.smi);
			int num = Grid.PosToCell(gameObject);
			if (Grid.IsValidCell(num))
			{
				bool flag = SleepChore.IsDarkAtCell(num);
				if (base.sm.needsNightLight.Get(base.smi))
				{
					if (flag)
					{
						gameObject.Trigger(-1307593733, null);
						return;
					}
				}
				else if (!flag && !this.IsLoudSleeper() && !this.IsGlowStick())
				{
					gameObject.Trigger(-1063113160, null);
				}
			}
		}

		public bool IsLoudSleeper()
		{
			return base.sm.sleeper.Get(base.smi).GetComponent<Snorer>() != null;
		}

		public bool IsGlowStick()
		{
			return base.sm.sleeper.Get(base.smi).GetComponent<GlowStick>() != null;
		}

		public void EvaluateSleepQuality()
		{
		}

		public void AddLocator(GameObject sleepable)
		{
			this.locator = sleepable;
			int num = Grid.PosToCell(this.locator);
			Grid.Reserved[num] = true;
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
				.TriggerOnExit(GameHashes.SleepFinished, null)
				.EventHandler(GameHashes.SleepDisturbedByLight, delegate(SleepChore.StatesInstance smi)
				{
					this.isDisturbedByLight.Set(true, smi);
				})
				.EventHandler(GameHashes.SleepDisturbedByNoise, delegate(SleepChore.StatesInstance smi)
				{
					this.isDisturbedByNoise.Set(true, smi);
				})
				.EventHandler(GameHashes.SleepDisturbedByFearOfDark, delegate(SleepChore.StatesInstance smi)
				{
					this.isScaredOfDark.Set(true, smi);
				})
				.EventHandler(GameHashes.SleepDisturbedByMovement, delegate(SleepChore.StatesInstance smi)
				{
					this.isDisturbedByMovement.Set(true, smi);
				});
			this.sleep.uninterruptable.DoNothing();
			this.sleep.normal.ParamTransition<bool>(this.isInterruptable, this.sleep.uninterruptable, GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.IsFalse).ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.Sleeping, null).QueueAnim("working_loop", true, null)
				.ParamTransition<bool>(this.isDisturbedByNoise, this.sleep.interrupt_noise, GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.IsTrue)
				.ParamTransition<bool>(this.isDisturbedByLight, this.sleep.interrupt_light, GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.IsTrue)
				.ParamTransition<bool>(this.isScaredOfDark, this.sleep.interrupt_scared, GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.IsTrue)
				.ParamTransition<bool>(this.isDisturbedByMovement, this.sleep.interrupt_movement, GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.IsTrue)
				.Update(delegate(SleepChore.StatesInstance smi, float dt)
				{
					smi.CheckLightLevel();
				}, UpdateRate.SIM_200ms, false);
			this.sleep.interrupt_scared.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.SleepingInterruptedByFearOfDark, null).QueueAnim("interrupt_afraid", false, null).OnAnimQueueComplete(this.sleep.interrupt_scared_transition);
			this.sleep.interrupt_scared_transition.Enter(delegate(SleepChore.StatesInstance smi)
			{
				if (!smi.master.GetComponent<Effects>().HasEffect(Db.Get().effects.Get("TerribleSleep")))
				{
					smi.master.GetComponent<Effects>().Add(Db.Get().effects.Get("BadSleepAfraidOfDark"), true);
				}
				GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State state = (smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Sleep) ? this.sleep.normal : this.success);
				this.isScaredOfDark.Set(false, smi);
				smi.GoTo(state);
			});
			this.sleep.interrupt_movement.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.SleepingInterruptedByMovement, null).PlayAnim("interrupt_light").OnAnimQueueComplete(this.sleep.interrupt_movement_transition)
				.Enter(delegate(SleepChore.StatesInstance smi)
				{
					GameObject gameObject = smi.sm.bed.Get(smi);
					if (gameObject != null)
					{
						gameObject.Trigger(-717201811, null);
					}
				});
			this.sleep.interrupt_movement_transition.Enter(delegate(SleepChore.StatesInstance smi)
			{
				if (!smi.master.GetComponent<Effects>().HasEffect(Db.Get().effects.Get("TerribleSleep")))
				{
					smi.master.GetComponent<Effects>().Add(Db.Get().effects.Get("BadSleepMovement"), true);
				}
				GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State state2 = (smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Sleep) ? this.sleep.normal : this.success);
				this.isDisturbedByMovement.Set(false, smi);
				smi.GoTo(state2);
			});
			this.sleep.interrupt_noise.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.SleepingInterruptedByNoise, null).QueueAnim("interrupt_light", false, null).OnAnimQueueComplete(this.sleep.interrupt_noise_transition);
			this.sleep.interrupt_noise_transition.Enter(delegate(SleepChore.StatesInstance smi)
			{
				Effects component = smi.master.GetComponent<Effects>();
				component.Add(Db.Get().effects.Get("TerribleSleep"), true);
				if (component.HasEffect(Db.Get().effects.Get("BadSleep")))
				{
					component.Remove(Db.Get().effects.Get("BadSleep"));
				}
				this.isDisturbedByNoise.Set(false, smi);
				GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State state3 = (smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Sleep) ? this.sleep.normal : this.success);
				smi.GoTo(state3);
			});
			this.sleep.interrupt_light.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.SleepingInterruptedByLight, null).QueueAnim("interrupt", false, null).OnAnimQueueComplete(this.sleep.interrupt_light_transition);
			this.sleep.interrupt_light_transition.Enter(delegate(SleepChore.StatesInstance smi)
			{
				if (!smi.master.GetComponent<Effects>().HasEffect(Db.Get().effects.Get("TerribleSleep")))
				{
					smi.master.GetComponent<Effects>().Add(Db.Get().effects.Get("BadSleep"), true);
				}
				GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State state4 = (smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Sleep) ? this.sleep.normal : this.success);
				this.isDisturbedByLight.Set(false, smi);
				smi.GoTo(state4);
			});
			this.success.Enter(delegate(SleepChore.StatesInstance smi)
			{
				smi.EvaluateSleepQuality();
			}).ReturnSuccess();
		}

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.TargetParameter sleeper;

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.TargetParameter bed;

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter isInterruptable;

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter isDisturbedByNoise;

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter isDisturbedByLight;

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter isDisturbedByMovement;

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter isScaredOfDark;

		public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter needsNightLight;

		public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.ApproachSubState<IApproachable> approach;

		public SleepChore.States.SleepStates sleep;

		public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State success;

		public class SleepStates : GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State
		{
			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State condition_transition;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State condition_transition_pre;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State uninterruptable;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State normal;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_noise;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_noise_transition;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_light;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_light_transition;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_scared;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_scared_transition;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_movement;

			public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_movement_transition;
		}
	}
}
