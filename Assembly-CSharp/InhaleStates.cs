using System;
using STRINGS;

internal class InhaleStates : GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.goingtoeat;
		this.root.Enter("SetTarget", delegate(InhaleStates.Instance smi)
		{
			this.targetCell.Set(smi.GetSMI<GasAndLiquidConsumerMonitor.Instance>().targetCell, smi);
		});
		GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State state = this.goingtoeat.MoveTo((InhaleStates.Instance smi) => this.targetCell.Get(smi), this.inhaling, null, false);
		string text = CREATURES.STATUSITEMS.LOOKINGFORFOOD.NAME;
		string text2 = CREATURES.STATUSITEMS.LOOKINGFORFOOD.TOOLTIP;
		StatusItemCategory statusItemCategory = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, statusItemCategory);
		GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State state2 = this.inhaling.DefaultState(this.inhaling.pre);
		text2 = CREATURES.STATUSITEMS.INHALING.NAME;
		text = CREATURES.STATUSITEMS.INHALING.TOOLTIP;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(text2, text, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, statusItemCategory);
		this.inhaling.pre.PlayAnim("inhale_pre").QueueAnim("inhale_loop", true, null).Update("Consume", delegate(InhaleStates.Instance smi, float dt)
		{
			smi.GetSMI<GasAndLiquidConsumerMonitor.Instance>().Consume(dt);
		}, UpdateRate.SIM_200ms, false)
			.EventTransition(GameHashes.ElementNoLongerAvailable, this.inhaling.pst, null)
			.Enter("StartInhaleSound", delegate(InhaleStates.Instance smi)
			{
				smi.StartInhaleSound();
			})
			.Exit("StopInhaleSound", delegate(InhaleStates.Instance smi)
			{
				smi.StopInhaleSound();
			})
			.ScheduleGoTo((InhaleStates.Instance smi) => smi.def.inhaleTime, this.inhaling.pst);
		this.inhaling.pst.Transition(this.inhaling.full, new StateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.Transition.ConditionCallback(InhaleStates.IsFull), UpdateRate.SIM_200ms).Transition(this.behaviourcomplete, GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.Not(new StateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.Transition.ConditionCallback(InhaleStates.IsFull)), UpdateRate.SIM_200ms);
		this.inhaling.full.QueueAnim("inhale_pst", false, null).QueueAnim("idle_loop", false, null).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToEat, false);
	}

	private static bool IsFull(InhaleStates.Instance smi)
	{
		CreatureCalorieMonitor.Instance smi2 = smi.GetSMI<CreatureCalorieMonitor.Instance>();
		return smi2 != null && smi2.stomach.GetFullness() >= 1f;
	}

	public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State goingtoeat;

	public InhaleStates.InhalingStates inhaling;

	public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State behaviourcomplete;

	public StateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.IntParameter targetCell;

	public class Def : StateMachine.BaseDef
	{
		public string inhaleSound;

		public float inhaleTime = 3f;
	}

	public new class Instance : GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.GameInstance
	{
		public Instance(Chore<InhaleStates.Instance> chore, InhaleStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToEat);
			this.inhaleSound = GlobalAssets.GetSound(def.inhaleSound, false);
		}

		public void StartInhaleSound()
		{
			LoopingSounds component = base.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.StartSound(base.smi.inhaleSound);
			}
		}

		public void StopInhaleSound()
		{
			LoopingSounds component = base.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.StopSound(base.smi.inhaleSound);
			}
		}

		public string inhaleSound;
	}

	public class InhalingStates : GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State
	{
		public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State pre;

		public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State pst;

		public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State full;
	}
}
