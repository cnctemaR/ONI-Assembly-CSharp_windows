using System;
using STRINGS;

internal class TrappedStates : GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.trapped;
		GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>.State root = this.root;
		string text = CREATURES.STATUSITEMS.TRAPPED.NAME;
		string text2 = CREATURES.STATUSITEMS.TRAPPED.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486, null, null, main);
		this.trapped.PlayAnim("trapped_pre").QueueAnim("trapped", true, null).TagTransition(GameTags.Trapped, this.pst, true);
		this.pst.PlayAnim("escape").OnAnimQueueComplete(null);
	}

	private GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>.State trapped;

	private GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>.State pst;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>.GameInstance
	{
		public Instance(Chore<TrappedStates.Instance> chore, TrappedStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(TrappedStates.Instance.IsTrapped, null);
		}

		public static Chore.Precondition IsTrapped = new Chore.Precondition
		{
			id = "IsTrapped",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Trapped);
			}
		};
	}
}
