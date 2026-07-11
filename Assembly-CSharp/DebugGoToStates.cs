using System;
using STRINGS;

internal class DebugGoToStates : GameStateMachine<DebugGoToStates, DebugGoToStates.Instance, IStateMachineTarget, DebugGoToStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.moving;
		GameStateMachine<DebugGoToStates, DebugGoToStates.Instance, IStateMachineTarget, DebugGoToStates.Def>.State state = this.moving.MoveTo(new Func<DebugGoToStates.Instance, int>(DebugGoToStates.GetTargetCell), this.behaviourcomplete, this.behaviourcomplete, true);
		string text = CREATURES.STATUSITEMS.DEBUGGOTO.NAME;
		string text2 = CREATURES.STATUSITEMS.DEBUGGOTO.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 63486, null, null, main);
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
