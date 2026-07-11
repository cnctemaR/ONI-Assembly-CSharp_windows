using System;
using STRINGS;

internal class StunnedStates : GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.stunned;
		GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>.State root = this.root;
		string text = CREATURES.STATUSITEMS.GETTING_WRANGLED.NAME;
		string text2 = CREATURES.STATUSITEMS.GETTING_WRANGLED.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 63486, null, null, main);
		this.stunned.PlayAnim("idle_loop", KAnim.PlayMode.Loop).TagTransition(GameTags.Creatures.Stunned, null, true);
	}

	public GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>.State stunned;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>.GameInstance
	{
		public Instance(Chore<StunnedStates.Instance> chore, StunnedStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(StunnedStates.Instance.IsStunned, null);
		}

		public static readonly Chore.Precondition IsStunned = new Chore.Precondition
		{
			id = "IsStunned",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Creatures.Stunned);
			}
		};
	}
}
