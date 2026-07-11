using System;
using STRINGS;

public class FleeStates : GameStateMachine<FleeStates, FleeStates.Instance, IStateMachineTarget, FleeStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.plan;
		this.root.Enter("SetFleeTarget", delegate(FleeStates.Instance smi)
		{
			this.fleeToTarget.Set(CreatureHelpers.GetFleeTargetLocatorObject(smi.master.gameObject, smi.GetSMI<ThreatMonitor.Instance>().MainThreat), smi);
		}).ToggleStatusItem(CREATURES.STATUSITEMS.FLEEING.NAME, CREATURES.STATUSITEMS.FLEEING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.plan.Enter(delegate(FleeStates.Instance smi)
		{
			ThreatMonitor.Instance smi2 = smi.master.gameObject.GetSMI<ThreatMonitor.Instance>();
			this.fleeToTarget.Set(CreatureHelpers.GetFleeTargetLocatorObject(smi.master.gameObject, smi2.MainThreat), smi);
			if (this.fleeToTarget.Get(smi) != null)
			{
				smi.GoTo(this.approach);
				return;
			}
			smi.GoTo(this.cower);
		});
		this.approach.InitializeStates(this.mover, this.fleeToTarget, this.cower, this.cower, null, NavigationTactics.ReduceTravelDistance).Enter(delegate(FleeStates.Instance smi)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, CREATURES.STATUSITEMS.FLEEING.NAME.text, smi.master.transform, 1.5f, false);
		});
		this.cower.Enter(delegate(FleeStates.Instance smi)
		{
			string text = "DEFAULT COWER ANIMATION";
			if (smi.Get<KBatchedAnimController>().HasAnimation("cower"))
			{
				text = "cower";
			}
			else if (smi.Get<KBatchedAnimController>().HasAnimation("idle"))
			{
				text = "idle";
			}
			else if (smi.Get<KBatchedAnimController>().HasAnimation("idle_loop"))
			{
				text = "idle_loop";
			}
			smi.Get<KBatchedAnimController>().Play(text, KAnim.PlayMode.Loop, 1f, 0f);
		}).ScheduleGoTo(2f, this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Flee, false);
	}

	private StateMachine<FleeStates, FleeStates.Instance, IStateMachineTarget, FleeStates.Def>.TargetParameter mover;

	public StateMachine<FleeStates, FleeStates.Instance, IStateMachineTarget, FleeStates.Def>.TargetParameter fleeToTarget;

	public GameStateMachine<FleeStates, FleeStates.Instance, IStateMachineTarget, FleeStates.Def>.State plan;

	public GameStateMachine<FleeStates, FleeStates.Instance, IStateMachineTarget, FleeStates.Def>.ApproachSubState<IApproachable> approach;

	public GameStateMachine<FleeStates, FleeStates.Instance, IStateMachineTarget, FleeStates.Def>.State cower;

	public GameStateMachine<FleeStates, FleeStates.Instance, IStateMachineTarget, FleeStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<FleeStates, FleeStates.Instance, IStateMachineTarget, FleeStates.Def>.GameInstance
	{
		public Instance(Chore<FleeStates.Instance> chore, FleeStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Flee);
			base.sm.mover.Set(base.GetComponent<Navigator>(), base.smi);
		}
	}
}
