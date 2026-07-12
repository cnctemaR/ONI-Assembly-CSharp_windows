using System;
using STRINGS;
using UnityEngine;

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
		}).ToggleTag(GameTags.Creatures.Deliverable).PlayAnim(new Func<TrappedStates.Instance, string>(TrappedStates.GetTrappedAnimName), KAnim.PlayMode.Loop)
			.TagTransition(GameTags.Trapped, null, true);
	}

	public static string GetTrappedAnimName(TrappedStates.Instance smi)
	{
		string text = "trapped";
		int num = Grid.PosToCell(smi.transform.GetPosition());
		GameObject gameObject = Grid.Objects[num, 1];
		if (gameObject != null)
		{
			if (gameObject.GetComponent<TrappedStates.ITrapStateAnimationInstructions>() != null)
			{
				string trappedAnimationName = gameObject.GetComponent<TrappedStates.ITrapStateAnimationInstructions>().GetTrappedAnimationName();
				if (trappedAnimationName != null)
				{
					return trappedAnimationName;
				}
			}
			if (gameObject.GetSMI<TrappedStates.ITrapStateAnimationInstructions>() != null)
			{
				string trappedAnimationName2 = gameObject.GetSMI<TrappedStates.ITrapStateAnimationInstructions>().GetTrappedAnimationName();
				if (trappedAnimationName2 != null)
				{
					return trappedAnimationName2;
				}
			}
		}
		Trappable component = smi.gameObject.GetComponent<Trappable>();
		if (component != null && component.HasTag(GameTags.Creatures.Swimmer) && Grid.IsValidCell(num) && !Grid.IsLiquid(num))
		{
			text = "trapped_onLand";
		}
		return text;
	}

	public const string DEFAULT_TRAPPED_ANIM_NAME = "trapped";

	private GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>.State trapped;

	public class Def : StateMachine.BaseDef
	{
	}

	public interface ITrapStateAnimationInstructions
	{
		string GetTrappedAnimationName();
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
