using System;
using System.Collections.Generic;
using STRINGS;

public class StunnedStates : GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.init;
		this.init.TagTransition(GameTags.Creatures.StunnedForCapture, this.stun_for_capture, false).TagTransition(GameTags.Creatures.StunnedBeingEaten, this.stun_for_being_eaten, false);
		GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>.State state = this.stun_for_capture;
		string text = CREATURES.STATUSITEMS.GETTING_WRANGLED.NAME;
		string text2 = CREATURES.STATUSITEMS.GETTING_WRANGLED.TOOLTIP;
		string text3 = "";
		StatusItem.IconType iconType = StatusItem.IconType.Info;
		NotificationType notificationType = NotificationType.Neutral;
		bool flag = false;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, text3, iconType, notificationType, flag, default(HashedString), 129022, null, null, main).PlayAnim("idle_loop", KAnim.PlayMode.Loop).TagTransition(GameTags.Creatures.StunnedForCapture, null, true);
		this.stun_for_being_eaten.PlayAnim("eaten", KAnim.PlayMode.Once).TagTransition(GameTags.Creatures.StunnedBeingEaten, null, true);
	}

	private static List<Tag> StunnedTags = new List<Tag>
	{
		GameTags.Creatures.StunnedForCapture,
		GameTags.Creatures.StunnedBeingEaten
	};

	public GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>.State init;

	public GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>.State stun_for_capture;

	public GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>.State stun_for_being_eaten;

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
				return context.consumerState.prefabid.HasAnyTags(StunnedStates.StunnedTags);
			}
		};
	}
}
