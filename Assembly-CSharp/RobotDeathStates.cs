using System;
using STRINGS;

public class RobotDeathStates : GameStateMachine<RobotDeathStates, RobotDeathStates.Instance, IStateMachineTarget, RobotDeathStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.loop;
		this.loop.ToggleStatusItem(CREATURES.STATUSITEMS.DEAD.NAME, CREATURES.STATUSITEMS.DEAD.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).PlayAnim("death").OnAnimQueueComplete(this.pst);
		this.pst.TriggerOnEnter(GameHashes.DeathAnimComplete, null).TriggerOnEnter(GameHashes.Died, (RobotDeathStates.Instance smi) => smi.gameObject).BehaviourComplete(GameTags.Creatures.Die, false);
	}

	private GameStateMachine<RobotDeathStates, RobotDeathStates.Instance, IStateMachineTarget, RobotDeathStates.Def>.State loop;

	private GameStateMachine<RobotDeathStates, RobotDeathStates.Instance, IStateMachineTarget, RobotDeathStates.Def>.State pst;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<RobotDeathStates, RobotDeathStates.Instance, IStateMachineTarget, RobotDeathStates.Def>.GameInstance
	{
		public Instance(Chore<RobotDeathStates.Instance> chore, RobotDeathStates.Def def)
			: base(chore, def)
		{
			chore.choreType.interruptPriority = Db.Get().ChoreTypes.Die.interruptPriority;
			chore.masterPriority.priority_class = PriorityScreen.PriorityClass.compulsory;
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Die);
		}
	}
}
