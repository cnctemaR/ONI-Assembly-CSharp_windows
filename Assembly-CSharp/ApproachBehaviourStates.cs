using System;
using UnityEngine;

public class ApproachBehaviourStates : GameStateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.approach;
		this.root.Enter(new StateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.State.Callback(ApproachBehaviourStates.RefreshTarget)).Enter(new StateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.State.Callback(ApproachBehaviourStates.Reserve)).Exit(new StateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.State.Callback(ApproachBehaviourStates.Unreserve))
			.EventHandler(GameHashes.ApproachableTargetChanged, new StateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.State.Callback(ApproachBehaviourStates.RefreshTarget));
		this.approach.InitializeStates(this.self, this.target, (ApproachBehaviourStates.Instance smi) => smi.targetOffsets, this.interact, this.failure, null).ToggleMainStatusItem((ApproachBehaviourStates.Instance smi) => smi.GetMonitor().GetApproachStatusItem(), null);
		this.interact.Enter(delegate(ApproachBehaviourStates.Instance smi)
		{
			smi.GetMonitor().OnArrive();
		}).DefaultState(this.interact.pre).OnTargetLost(this.target, this.failure)
			.ToggleMainStatusItem((ApproachBehaviourStates.Instance smi) => smi.GetMonitor().GetBehaviourStatusItem(), null);
		this.interact.pre.PlayAnim((ApproachBehaviourStates.Instance smi) => smi.def.preAnim, KAnim.PlayMode.Once).OnAnimQueueComplete(this.interact.loop);
		this.interact.loop.PlayAnim((ApproachBehaviourStates.Instance smi) => smi.def.loopAnim, KAnim.PlayMode.Once).OnAnimQueueComplete(this.interact.pst);
		this.interact.pst.PlayAnim((ApproachBehaviourStates.Instance smi) => smi.def.pstAnim, KAnim.PlayMode.Once).OnAnimQueueComplete(this.behaviourComplete);
		this.behaviourComplete.BehaviourComplete((ApproachBehaviourStates.Instance smi) => smi.def.behaviourTag, false).Exit(delegate(ApproachBehaviourStates.Instance smi)
		{
			smi.GetMonitor().OnSuccess();
		});
		this.failure.Enter(delegate(ApproachBehaviourStates.Instance smi)
		{
			smi.GetMonitor().OnFailure();
		}).GoTo(null);
	}

	private static void Reserve(ApproachBehaviourStates.Instance smi)
	{
		if (smi.def.reserveTag != Tag.Invalid)
		{
			smi.sm.target.Get(smi).GetComponent<KPrefabID>().SetTag(smi.def.reserveTag, true);
		}
	}

	private static void Unreserve(ApproachBehaviourStates.Instance smi)
	{
		if (smi.def.reserveTag != Tag.Invalid && smi.sm.target.Get(smi) != null)
		{
			smi.sm.target.Get(smi).GetComponent<KPrefabID>().RemoveTag(smi.def.reserveTag);
		}
	}

	public static void RefreshTarget(ApproachBehaviourStates.Instance smi)
	{
		GameObject gameObject = smi.GetMonitor().GetTarget();
		if (gameObject == null)
		{
			smi.GoTo(smi.sm.failure);
			return;
		}
		smi.targetOffsets = smi.GetMonitor().GetApproachOffsets();
		smi.sm.target.Set(gameObject, smi, false);
	}

	public ApproachBehaviourStates.InteractState interact;

	public GameStateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.State behaviourComplete;

	public GameStateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.ApproachSubState<IApproachable> approach;

	public GameStateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.State failure;

	public StateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.TargetParameter self;

	public StateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.TargetParameter target;

	public class Def : StateMachine.BaseDef
	{
		public Def(Tag monitorId, Tag behaviourTag)
		{
			this.monitorId = monitorId;
			this.behaviourTag = behaviourTag;
		}

		public Tag monitorId;

		public Tag behaviourTag;

		public Tag reserveTag = GameTags.Creatures.ReservedByCreature;

		public string preAnim = "";

		public string loopAnim = "";

		public string pstAnim = "";
	}

	public class InteractState : GameStateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.State
	{
		public GameStateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.State pre;

		public GameStateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.State loop;

		public GameStateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.State pst;
	}

	public new class Instance : GameStateMachine<ApproachBehaviourStates, ApproachBehaviourStates.Instance, IStateMachineTarget, ApproachBehaviourStates.Def>.GameInstance
	{
		public Instance(Chore<ApproachBehaviourStates.Instance> chore, ApproachBehaviourStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, def.behaviourTag);
			base.sm.self.Set(base.smi.gameObject, base.smi, false);
		}

		public IApproachableBehaviour GetMonitor()
		{
			if (this.monitor.IsNullOrDestroyed())
			{
				this.SetMonitor();
			}
			return this.monitor;
		}

		private void SetMonitor()
		{
			foreach (ICreatureMonitor creatureMonitor in base.gameObject.GetAllSMI<ICreatureMonitor>())
			{
				if (creatureMonitor.Id == base.def.monitorId)
				{
					this.monitor = creatureMonitor as IApproachableBehaviour;
					break;
				}
			}
			global::Debug.Assert(base.smi.monitor != null, "Could not find monitor with ID");
		}

		private IApproachableBehaviour monitor;

		public CellOffset[] targetOffsets;
	}
}
