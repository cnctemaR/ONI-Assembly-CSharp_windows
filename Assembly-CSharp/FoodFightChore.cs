using System;
using STRINGS;
using UnityEngine;

public class FoodFightChore : Chore<FoodFightChore.StatesInstance>
{
	public FoodFightChore(IStateMachineTarget master, GameObject locator)
		: base(Db.Get().ChoreTypes.Party, master, master.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.high, 5, false, true, 0, false, ReportManager.ReportType.PersonalTime)
	{
		base.smi = new FoodFightChore.StatesInstance(this, locator);
		this.showAvailabilityInHoverText = false;
		this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		this.AddPrecondition(FoodFightChore.EdibleIsNotNull, null);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			global::Debug.LogError("FOODFIGHTCHORE null context.consumer");
			return;
		}
		RationMonitor.Instance smi = context.consumerState.consumer.GetSMI<RationMonitor.Instance>();
		if (smi == null)
		{
			global::Debug.LogError("FOODFIGHTCHORE null RationMonitor.Instance");
			return;
		}
		Edible edible = smi.GetEdible();
		if (edible.gameObject == null)
		{
			global::Debug.LogError("FOODFIGHTCHORE null edible.gameObject");
			return;
		}
		if (base.smi == null)
		{
			global::Debug.LogError("FOODFIGHTCHORE null smi");
			return;
		}
		if (base.smi.sm == null)
		{
			global::Debug.LogError("FOODFIGHTCHORE null smi.sm");
			return;
		}
		if (base.smi.sm.ediblesource == null)
		{
			global::Debug.LogError("FOODFIGHTCHORE null smi.sm.ediblesource");
			return;
		}
		base.smi.sm.ediblesource.Set(edible.gameObject, base.smi, false);
		KCrashReporter.Assert(edible.FoodInfo.CaloriesPerUnit > 0f, edible.GetProperName() + " has invalid calories per unit. Will result in NaNs", null);
		float num = 0.5f;
		KCrashReporter.Assert(num > 0f, "FoodFightChore is requesting an invalid amount of food", null);
		base.smi.sm.requestedfoodunits.Set(num, base.smi, false);
		base.smi.sm.eater.Set(context.consumerState.gameObject, base.smi, false);
		base.Begin(context);
	}

	public static readonly Chore.Precondition EdibleIsNotNull = new Chore.Precondition
	{
		id = "EdibleIsNotNull",
		description = DUPLICANTS.CHORES.PRECONDITIONS.EDIBLE_IS_NOT_NULL,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return null != context.consumerState.consumer.GetSMI<RationMonitor.Instance>().GetEdible();
		}
	};

	public class StatesInstance : GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.GameInstance
	{
		public StatesInstance(FoodFightChore master, GameObject locator)
			: base(master)
		{
			base.sm.locator.Set(locator, base.smi, false);
		}

		public void UpdateAttackTarget()
		{
			int num = 0;
			MinionIdentity minionIdentity;
			for (;;)
			{
				num++;
				minionIdentity = Components.LiveMinionIdentities[global::UnityEngine.Random.Range(0, Components.LiveMinionIdentities.Count)];
				if (num > 30)
				{
					break;
				}
				if (Components.LiveMinionIdentities.Count <= 1 || ((!(base.sm.attackableTarget.Get(base.smi) != null) || !(minionIdentity.gameObject == base.sm.attackableTarget.Get(base.smi).gameObject)) && Game.Instance.roomProber.GetRoomOfGameObject(minionIdentity.gameObject) != null && (Game.Instance.roomProber.GetRoomOfGameObject(minionIdentity.gameObject).roomType == Db.Get().RoomTypes.MessHall || Game.Instance.roomProber.GetRoomOfGameObject(minionIdentity.gameObject).roomType == Db.Get().RoomTypes.GreatHall) && !(minionIdentity.gameObject == base.smi.master.gameObject) && Game.Instance.roomProber.GetRoomOfGameObject(minionIdentity.gameObject) == Game.Instance.roomProber.GetRoomOfGameObject(base.smi.master.gameObject)))
				{
					goto IL_0152;
				}
			}
			minionIdentity = null;
			IL_0152:
			if (minionIdentity == null)
			{
				base.smi.GoTo(base.sm.end);
				return;
			}
			base.smi.sm.attackableTarget.Set(minionIdentity.GetComponent<AttackableBase>(), base.smi);
		}

		private int locatorCell;
	}

	public class States : GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.fetch;
			base.Target(this.eater);
			this.root.ToggleAnims("anim_loco_run_angry_kanim", 0f);
			this.fetch.InitializeStates(this.eater, this.ediblesource, this.ediblechunk, this.requestedfoodunits, this.actualfoodunits, this.moveToArena, null).ToggleAnims("anim_loco_run_angry_kanim", 0f);
			this.moveToArena.InitializeStates(this.eater, this.locator, this.waitForParticipants, null, null, null);
			this.waitForParticipants.Enter(delegate(FoodFightChore.StatesInstance smi)
			{
				smi.master.GetComponent<Facing>().SetFacing(Game.Instance.roomProber.GetRoomOfGameObject(smi.master.gameObject).cavity.GetCenter().x <= smi.master.transform.position.x);
			}).ToggleAnims("anim_rage_kanim", 0f).PlayAnim("idle_pre")
				.QueueAnim("idle_default", true, null)
				.ScheduleGoTo(30f, this.emoteRoar)
				.EventTransition(GameHashes.GameplayEventCommence, this.emoteRoar, null);
			this.emoteRoar.Enter("ChooseTarget", delegate(FoodFightChore.StatesInstance smi)
			{
				smi.UpdateAttackTarget();
			}).ToggleAnims("anim_rage_kanim", 0f).PlayAnim("rage_pre")
				.QueueAnim("rage_loop", false, null)
				.QueueAnim("rage_pst", false, null)
				.OnAnimQueueComplete(this.fight);
			this.fight.DefaultState(this.fight.moveto);
			this.fight.moveto.InitializeStates(this.eater, this.attackableTarget, this.fight.throwFood, null, null, NavigationTactics.Range_3_ProhibitOverlap);
			this.fight.throwFood.Enter(delegate(FoodFightChore.StatesInstance smi)
			{
				smi.master.GetComponent<Facing>().Face(this.attackableTarget.Get(smi).transform.position.x);
				GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(FoodCometConfig.ID), smi.master.transform.position + Vector3.up);
				gameObject.SetActive(true);
				Comet comet = gameObject.GetComponent<Comet>();
				Vector3 vector = this.attackableTarget.Get(smi).transform.position - smi.master.transform.position;
				vector.Normalize();
				comet.Velocity = vector * 5f;
				Comet comet2 = comet;
				comet2.OnImpact = (global::System.Action)Delegate.Combine(comet2.OnImpact, new global::System.Action(delegate
				{
					GameObject gameObject3 = Grid.Objects[Grid.PosToCell(comet), 0];
					if (gameObject3 != null)
					{
						if (global::UnityEngine.Random.Range(0, 100) > 75)
						{
							new FleeChore(gameObject3.GetComponent<IStateMachineTarget>(), smi.master.gameObject);
							return;
						}
						gameObject3.Trigger(508119890, null);
					}
				}));
				GameObject gameObject2 = smi.master.GetComponent<Storage>().FindFirst(GameTags.Edible);
				if (!(gameObject2 != null))
				{
					smi.GoTo(this.end);
					return;
				}
				Edible component = gameObject2.GetComponent<Edible>();
				float num = Math.Min(200000f, component.Calories);
				component.Calories -= num;
				ReportManager.Instance.ReportValueWithGameObjectContext(ReportManager.ReportType.CaloriesCreated, -num, smi.master.gameObject, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.FOODFIGHT_CONTEXT, "{0}", component.GetProperName()));
				if (component.Calories <= 0f)
				{
					Util.KDestroyGameObject(gameObject2);
					smi.GoTo(this.end);
					return;
				}
				smi.GoTo(this.emoteRoar);
			});
			this.end.Enter(delegate(FoodFightChore.StatesInstance smi)
			{
				Util.KDestroyGameObject(this.ediblechunk.Get(smi));
				smi.StopSM("complete");
			});
		}

		public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.TargetParameter eater;

		public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.TargetParameter ediblesource;

		public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.TargetParameter ediblechunk;

		public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.TargetParameter attackableTarget;

		public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.FloatParameter requestedfoodunits;

		public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.FloatParameter actualfoodunits;

		public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.TargetParameter locator;

		public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State waitForParticipants;

		public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State emoteRoar;

		public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.ApproachSubState<IApproachable> moveToArena;

		public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.FetchSubState fetch;

		public FoodFightChore.States.AttackStates fight;

		public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State end;

		public class AttackStates : GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State
		{
			public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.ApproachSubState<AttackableBase> moveto;

			public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State throwFood;
		}
	}
}
