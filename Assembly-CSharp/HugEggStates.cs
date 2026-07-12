using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class HugEggStates : GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.moving;
		this.root.Enter(new StateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State.Callback(HugEggStates.SetTarget)).Enter(delegate(HugEggStates.Instance smi)
		{
			if (!HugEggStates.Reserve(smi))
			{
				smi.GoTo(this.behaviourcomplete);
			}
		}).Exit(new StateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State.Callback(HugEggStates.Unreserve))
			.ToggleStatusItem(CREATURES.STATUSITEMS.HUGEGG.NAME, CREATURES.STATUSITEMS.HUGEGG.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main)
			.OnTargetLost(this.target, this.behaviourcomplete);
		this.moving.MoveTo(new Func<HugEggStates.Instance, int>(HugEggStates.GetClimbableCell), this.hug, this.behaviourcomplete, false);
		this.hug.DefaultState(this.hug.pre).Enter(delegate(HugEggStates.Instance smi)
		{
			smi.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Front);
		}).Exit(delegate(HugEggStates.Instance smi)
		{
			smi.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
		});
		this.hug.pre.Face(this.target, 0.5f).PlayAnim((HugEggStates.Instance smi) => HugEggStates.GetAnims(smi).pre, KAnim.PlayMode.Once).OnAnimQueueComplete(this.hug.loop);
		this.hug.loop.QueueAnim((HugEggStates.Instance smi) => HugEggStates.GetAnims(smi).loop, true, null).ScheduleGoTo((HugEggStates.Instance smi) => smi.def.hugTime, this.hug.pst);
		this.hug.pst.QueueAnim((HugEggStates.Instance smi) => HugEggStates.GetAnims(smi).pst, false, null).Enter(new StateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State.Callback(HugEggStates.ApplyEffect)).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete((HugEggStates.Instance smi) => smi.def.behaviourTag, false);
	}

	private static void SetTarget(HugEggStates.Instance smi)
	{
		smi.sm.target.Set(smi.GetSMI<HugMonitor.Instance>().hugTarget, smi);
	}

	private static HugEggStates.AnimSet GetAnims(HugEggStates.Instance smi)
	{
		if (!(smi.sm.target.Get(smi).GetComponent<EggIncubator>() != null))
		{
			return smi.def.hugAnims;
		}
		return smi.def.incubatorHugAnims;
	}

	private static bool Reserve(HugEggStates.Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null && !gameObject.HasTag(GameTags.Creatures.ReservedByCreature))
		{
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
			return true;
		}
		return false;
	}

	private static void Unreserve(HugEggStates.Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null)
		{
			gameObject.RemoveTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static int GetClimbableCell(HugEggStates.Instance smi)
	{
		return Grid.PosToCell(smi.sm.target.Get(smi));
	}

	private static void ApplyEffect(HugEggStates.Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null)
		{
			EggIncubator component = gameObject.GetComponent<EggIncubator>();
			if (component != null)
			{
				component.Occupant.GetComponent<Effects>().Add("EggHug", true);
				return;
			}
			if (gameObject.HasTag(GameTags.Egg))
			{
				gameObject.GetComponent<Effects>().Add("EggHug", true);
			}
		}
	}

	public GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.ApproachSubState<EggIncubator> moving;

	public HugEggStates.HugState hug;

	public GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State behaviourcomplete;

	public StateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.TargetParameter target;

	public class AnimSet
	{
		public string pre;

		public string loop;

		public string pst;
	}

	public class Def : StateMachine.BaseDef
	{
		public Def(Tag behaviourTag)
		{
			this.behaviourTag = behaviourTag;
		}

		public float hugTime = 15f;

		public Tag behaviourTag;

		public HugEggStates.AnimSet hugAnims = new HugEggStates.AnimSet
		{
			pre = "hug_egg_pre",
			loop = "hug_egg_loop",
			pst = "hug_egg_pst"
		};

		public HugEggStates.AnimSet incubatorHugAnims = new HugEggStates.AnimSet
		{
			pre = "hug_incubator_pre",
			loop = "hug_incubator_loop",
			pst = "hug_incubator_pst"
		};
	}

	public new class Instance : GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.GameInstance
	{
		public Instance(Chore<HugEggStates.Instance> chore, HugEggStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, def.behaviourTag);
		}
	}

	public class HugState : GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State
	{
		public GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State pre;

		public GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State loop;

		public GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State pst;
	}
}
