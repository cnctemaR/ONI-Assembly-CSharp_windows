using System;
using STRINGS;

internal class DebugGoToStates : GameStateMachine<DebugGoToStates, DebugGoToStates.Instance, IStateMachineTarget, DebugGoToStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		GameStateMachine<DebugGoToStates, DebugGoToStates.Instance, IStateMachineTarget, DebugGoToStates.Def>.State state = this.root.ToggleTag(GameTags.HasDebugDestination).MoveTo((DebugGoToStates.Instance smi) => DebugHandler.GetMouseCell(), null, null, true);
		string text = CREATURES.STATUSITEMS.DEBUGGOTO.NAME;
		string text2 = CREATURES.STATUSITEMS.DEBUGGOTO.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486, null, null, main);
	}

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<DebugGoToStates, DebugGoToStates.Instance, IStateMachineTarget, DebugGoToStates.Def>.GameInstance
	{
		public Instance(Chore<DebugGoToStates.Instance> chore, DebugGoToStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(DebugGoToStates.Instance.HasDebugTarget, null);
		}

		public static Chore.Precondition HasDebugTarget = new Chore.Precondition
		{
			id = "HasDebugTarget",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.HasDebugDestination);
			}
		};
	}
}
