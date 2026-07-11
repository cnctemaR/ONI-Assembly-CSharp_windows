using System;
using STRINGS;

public class TrappedStates : GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.trapped;
		this.root.ToggleStatusItem(CREATURES.STATUSITEMS.TRAPPED.NAME, CREATURES.STATUSITEMS.TRAPPED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.trapped.Enter(delegate(TrappedStates.Instance smi)
		{
			Navigator component = smi.GetComponent<Navigator>();
			if (component.IsValidNavType(NavType.Floor))
			{
				component.SetCurrentNavType(NavType.Floor);
			}
		}).ToggleTag(GameTags.Creatures.Deliverable).PlayAnim("trapped", KAnim.PlayMode.Loop)
			.TagTransition(GameTags.Trapped, null, true);
	}

	private GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>.State trapped;

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

		public static readonly Chore.Precondition IsTrapped = new Chore.Precondition
		{
			id = "IsTrapped",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Trapped);
			}
		};
	}
}
