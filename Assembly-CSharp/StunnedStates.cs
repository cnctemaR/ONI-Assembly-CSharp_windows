using System;
using STRINGS;

public class StunnedStates : GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.stunned;
		this.root.ToggleStatusItem(CREATURES.STATUSITEMS.GETTING_WRANGLED.NAME, CREATURES.STATUSITEMS.GETTING_WRANGLED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
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
