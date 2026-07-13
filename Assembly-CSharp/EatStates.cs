using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class EatStates : GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>
{
	private static Effect CreatePredationStunEffect()
	{
		return new Effect("StunnedEat", "", "", 5f, false, false, true, "", -1f, null, "")
		{
			tag = new Tag?(GameTags.Creatures.StunnedBeingEaten)
		};
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.goingtoeat;
		this.root.Enter(new StateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State.Callback(EatStates.SetTarget)).Exit(new StateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State.Callback(EatStates.UnreserveEdible));
		GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State state = this.goingtoeat.MoveTo(new Func<EatStates.Instance, int>(EatStates.GetEdibleCell), this.arrivedAtEdible, this.behaviourcomplete, false);
		string text = CREATURES.STATUSITEMS.HUNGRY.NAME;
		string text2 = CREATURES.STATUSITEMS.HUNGRY.TOOLTIP;
		string text3 = "";
		StatusItem.IconType iconType = StatusItem.IconType.Info;
		NotificationType notificationType = NotificationType.Neutral;
		bool flag = false;
		StatusItemCategory statusItemCategory = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, text3, iconType, notificationType, flag, default(HashedString), 129022, null, null, statusItemCategory);
		this.arrivedAtEdible.EnterTransition(this.pounce, (EatStates.Instance smi) => smi.IsPredator).Transition(this.eating, (EatStates.Instance smi) => !smi.IsPredator, UpdateRate.SIM_200ms);
		GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State state2 = this.pounce.Face(this.target, 0f).DefaultState(this.pounce.pre);
		string text4 = CREATURES.STATUSITEMS.HUNTING.NAME;
		string text5 = CREATURES.STATUSITEMS.HUNTING.TOOLTIP;
		string text6 = "";
		StatusItem.IconType iconType2 = StatusItem.IconType.Info;
		NotificationType notificationType2 = NotificationType.Neutral;
		bool flag2 = false;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(text4, text5, text6, iconType2, notificationType2, flag2, default(HashedString), 129022, null, null, statusItemCategory);
		this.pounce.pre.PlayAnim("pounce_pre").OnAnimQueueComplete(this.pounce.roll);
		this.pounce.roll.Enter(delegate(EatStates.Instance smi)
		{
			if (EatStates.CheckHuntSuccess(smi))
			{
				smi.GoTo(this.pounce.hit);
				return;
			}
			smi.GoTo(this.pounce.miss);
		});
		this.pounce.hit.Enter(delegate(EatStates.Instance smi)
		{
			EatStates.FreezeEdible(smi);
		}).QueueAnim("pounce_hit", false, null).OnAnimQueueComplete(this.eating);
		this.pounce.miss.Enter(delegate(EatStates.Instance smi)
		{
			EatStates.OnPounceMiss(smi);
		}).QueueAnim("pounce_miss", false, null).OnAnimQueueComplete(this.failedHunt);
		this.failedHunt.PlayAnim("idle_loop", KAnim.PlayMode.Loop).ScheduleGoTo(5f, this.behaviourcomplete);
		GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State state3 = this.eating.EnterTransition(this.behaviourcomplete, (EatStates.Instance smi) => EatStates.EdibleGotAway(smi)).Face(this.target, 0f).DefaultState(this.eating.pre);
		string text7 = CREATURES.STATUSITEMS.EATING.NAME;
		string text8 = CREATURES.STATUSITEMS.EATING.TOOLTIP;
		string text9 = "";
		StatusItem.IconType iconType3 = StatusItem.IconType.Info;
		NotificationType notificationType3 = NotificationType.Neutral;
		bool flag3 = false;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state3.ToggleStatusItem(text7, text8, text9, iconType3, notificationType3, flag3, default(HashedString), 129022, null, null, statusItemCategory);
		this.eating.pre.Enter(delegate(EatStates.Instance smi)
		{
			EatStates.FreezeEdible(smi);
		}).QueueAnim((EatStates.Instance smi) => smi.eatAnims[0], false, null).OnAnimQueueComplete(this.eating.loop);
		this.eating.loop.Enter(new StateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State.Callback(EatStates.EatComplete)).QueueAnim((EatStates.Instance smi) => smi.eatAnims[1], false, null).OnAnimQueueComplete(this.eating.pst);
		this.eating.pst.QueueAnim((EatStates.Instance smi) => smi.eatAnims[2], false, null).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.Enter(delegate(EatStates.Instance smi)
		{
			smi.solidConsumer.ClearTargetEdible();
		}).PlayAnim("idle_loop", KAnim.PlayMode.Loop).BehaviourComplete(GameTags.Creatures.WantsToEat, false);
	}

	private static void SetTarget(EatStates.Instance smi)
	{
		smi.solidConsumer = smi.GetSMI<SolidConsumerMonitor.Instance>();
		smi.sm.target.Set(smi.solidConsumer.targetEdible, smi, false);
		EatStates.ReserveEdible(smi);
		smi.OverrideEatAnims(smi, smi.solidConsumer.GetTargetEdibleEatAnims());
		smi.sm.offset.Set(smi.solidConsumer.targetEdibleOffset, smi, false);
	}

	private static void ReserveEdible(EatStates.Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null)
		{
			DebugUtil.Assert(!gameObject.HasTag(GameTags.Creatures.ReservedByCreature));
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static void UnreserveEdible(EatStates.Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null)
		{
			if (gameObject.HasTag(GameTags.Creatures.ReservedByCreature))
			{
				gameObject.RemoveTag(GameTags.Creatures.ReservedByCreature);
				return;
			}
			global::Debug.LogWarningFormat(smi.gameObject, "{0} UnreserveEdible but it wasn't reserved: {1}", new object[] { smi.gameObject, gameObject });
		}
	}

	private static void EatComplete(EatStates.Instance smi)
	{
		PrimaryElement primaryElement = smi.sm.target.Get<PrimaryElement>(smi);
		if (primaryElement != null)
		{
			smi.lastMealElement = primaryElement.Element;
		}
		smi.Trigger(1386391852, smi.sm.target.Get<KPrefabID>(smi));
	}

	private static bool EdibleGotAway(EatStates.Instance smi)
	{
		int edibleCell = EatStates.GetEdibleCell(smi);
		return Grid.PosToCell(smi) != edibleCell;
	}

	private static void FreezeEdible(EatStates.Instance smi)
	{
		if (!smi.IsPredator)
		{
			return;
		}
		GameObject gameObject = smi.sm.target.Get(smi);
		Effects component = gameObject.GetComponent<Effects>();
		if (component != null)
		{
			component.Add(EatStates.PredationStunEffect, false);
		}
		Brain component2 = gameObject.GetComponent<Brain>();
		if (component2 != null)
		{
			Game.BrainScheduler.PrioritizeBrain(component2);
		}
	}

	private static void OnPounceMiss(EatStates.Instance smi)
	{
		smi.GetComponent<Effects>().Add("PredatorFailedHunt", true);
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null)
		{
			gameObject.Trigger(-787691065, smi.GetComponent<FactionAlignment>());
		}
	}

	private static bool HuntPredicateWild(GameObject obj)
	{
		if (obj == null)
		{
			return false;
		}
		AmountInstance amountInstance = Db.Get().Amounts.Age.Lookup(obj);
		if (amountInstance == null)
		{
			return true;
		}
		float num = amountInstance.value / amountInstance.GetMax();
		return num >= EatStates.HUNT_WILD_MIN_AGE && global::UnityEngine.Random.Range(0f, 1f) < EatStates.HUNT_WILD_PRED_RATE.Lerp(num);
	}

	private static bool HuntPredicateTame(GameObject obj)
	{
		if (obj == null)
		{
			return false;
		}
		AmountInstance amountInstance = Db.Get().Amounts.Age.Lookup(obj);
		if (amountInstance == null)
		{
			return true;
		}
		float num = amountInstance.value / amountInstance.GetMax();
		return global::UnityEngine.Random.Range(0f, 1f) < EatStates.HUNT_TAME_PRED_RATE.Lerp(num);
	}

	private static bool CheckHuntSuccess(EatStates.Instance smi)
	{
		WildnessMonitor.Instance smi2 = smi.gameObject.GetSMI<WildnessMonitor.Instance>();
		GameObject gameObject = smi.sm.target.Get(smi);
		WildnessMonitor.Instance instance = ((gameObject != null) ? gameObject.GetSMI<WildnessMonitor.Instance>() : null);
		bool flag = smi2 != null && smi2.IsWild();
		bool flag2 = instance != null && instance.IsWild();
		if (flag && flag2)
		{
			return EatStates.HuntPredicateWild(gameObject);
		}
		return EatStates.HuntPredicateTame(gameObject);
	}

	private static int GetEdibleCell(EatStates.Instance smi)
	{
		if (smi.Edible == null)
		{
			return Grid.InvalidCell;
		}
		return Grid.PosToCell(smi.Edible.transform.GetPosition() + smi.sm.offset.Get(smi));
	}

	private static Effect PredationStunEffect = EatStates.CreatePredationStunEffect();

	public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.ApproachSubState<Pickupable> goingtoeat;

	public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State arrivedAtEdible;

	public EatStates.PounceState pounce;

	public EatStates.EatingState eating;

	public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State failedHunt;

	public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State behaviourcomplete;

	public StateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.Vector3Parameter offset;

	public StateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.TargetParameter target;

	private static float HUNT_WILD_MIN_AGE = 0.825f;

	private static MathUtil.MinMax HUNT_WILD_PRED_RATE = new MathUtil.MinMax(0.1f, 1.1f);

	private static MathUtil.MinMax HUNT_TAME_PRED_RATE = new MathUtil.MinMax(0.4f, 1.05f);

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.GameInstance
	{
		public GameObject Edible
		{
			get
			{
				return base.smi.sm.target.Get(this);
			}
		}

		public bool IsPredator { get; private set; }

		public void OverrideEatAnims(EatStates.Instance smi, string[] preLoopPstAnims)
		{
			global::Debug.Assert(preLoopPstAnims != null && preLoopPstAnims.Length == 3);
			smi.eatAnims = preLoopPstAnims;
		}

		public Instance(Chore<EatStates.Instance> chore, EatStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToEat);
			chore.AddPrecondition(ChorePreconditions.instance.DoesntHaveTag, GameTags.Creatures.SuppressedDiet);
			this.IsPredator = base.gameObject.GetComponent<FactionAlignment>().Alignment == FactionManager.FactionID.Predator;
		}

		public Element GetLatestMealElement()
		{
			return this.lastMealElement;
		}

		public Element lastMealElement;

		public SolidConsumerMonitor.Instance solidConsumer;

		public string[] eatAnims = new string[] { "eat_pre", "eat_loop", "eat_pst" };
	}

	public class PounceState : GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State
	{
		public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State pre;

		public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State roll;

		public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State hit;

		public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State miss;
	}

	public class EatingState : GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State
	{
		public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State pre;

		public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State loop;

		public GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>.State pst;
	}
}
