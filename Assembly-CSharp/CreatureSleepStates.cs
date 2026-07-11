using System;
using STRINGS;

public class CreatureSleepStates : GameStateMachine<CreatureSleepStates, CreatureSleepStates.Instance, IStateMachineTarget, CreatureSleepStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.pre;
		this.root.ToggleStatusItem(CREATURES.STATUSITEMS.SLEEPING.NAME, CREATURES.STATUSITEMS.SLEEPING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.pre.QueueAnim("sleep_pre", false, null).OnAnimQueueComplete(this.loop);
		this.loop.QueueAnim("sleep_loop", true, null).Transition(this.pst, new StateMachine<CreatureSleepStates, CreatureSleepStates.Instance, IStateMachineTarget, CreatureSleepStates.Def>.Transition.ConditionCallback(CreatureSleepStates.ShouldWakeUp), UpdateRate.SIM_1000ms);
		this.pst.QueueAnim("sleep_pst", false, null).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Behaviours.SleepBehaviour, false);
	}

	public static bool ShouldWakeUp(CreatureSleepStates.Instance smi)
	{
		return !GameClock.Instance.IsNighttime();
	}

	public GameStateMachine<CreatureSleepStates, CreatureSleepStates.Instance, IStateMachineTarget, CreatureSleepStates.Def>.State pre;

	public GameStateMachine<CreatureSleepStates, CreatureSleepStates.Instance, IStateMachineTarget, CreatureSleepStates.Def>.State loop;

	public GameStateMachine<CreatureSleepStates, CreatureSleepStates.Instance, IStateMachineTarget, CreatureSleepStates.Def>.State pst;

	public GameStateMachine<CreatureSleepStates, CreatureSleepStates.Instance, IStateMachineTarget, CreatureSleepStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<CreatureSleepStates, CreatureSleepStates.Instance, IStateMachineTarget, CreatureSleepStates.Def>.GameInstance
	{
		public Instance(Chore<CreatureSleepStates.Instance> chore, CreatureSleepStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviours.SleepBehaviour);
		}
	}
}
