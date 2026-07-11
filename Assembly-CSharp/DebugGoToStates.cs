using System;
using STRINGS;

public class DebugGoToStates : GameStateMachine<DebugGoToStates, DebugGoToStates.Instance, IStateMachineTarget, DebugGoToStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.moving;
		this.moving.MoveTo(new Func<DebugGoToStates.Instance, int>(DebugGoToStates.GetTargetCell), this.behaviourcomplete, this.behaviourcomplete, true).ToggleStatusItem(CREATURES.STATUSITEMS.DEBUGGOTO.NAME, CREATURES.STATUSITEMS.DEBUGGOTO.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.behaviourcomplete.BehaviourComplete(GameTags.HasDebugDestination, false);
	}

	private static int GetTargetCell(DebugGoToStates.Instance smi)
	{
		return smi.GetSMI<CreatureDebugGoToMonitor.Instance>().targetCell;
	}

	public GameStateMachine<DebugGoToStates, DebugGoToStates.Instance, IStateMachineTarget, DebugGoToStates.Def>.State moving;

	public GameStateMachine<DebugGoToStates, DebugGoToStates.Instance, IStateMachineTarget, DebugGoToStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<DebugGoToStates, DebugGoToStates.Instance, IStateMachineTarget, DebugGoToStates.Def>.GameInstance
	{
		public Instance(Chore<DebugGoToStates.Instance> chore, DebugGoToStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.HasDebugDestination);
		}
	}
}
