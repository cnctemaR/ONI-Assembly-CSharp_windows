using System;
using STRINGS;

internal class FallStates : GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.loop;
		GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.State root = this.root;
		string text = CREATURES.STATUSITEMS.FALLING.NAME;
		string text2 = CREATURES.STATUSITEMS.FALLING.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486, null, null, main);
		this.loop.PlayAnim((FallStates.Instance smi) => smi.GetSMI<CreatureFallMonitor.Instance>().anim, KAnim.PlayMode.Loop).ToggleGravity().TagTransition(GameTags.Creatures.Falling, this.pst, true);
		this.pst.QueueAnim("idle_loop", true, null).GoTo(null);
	}

	private GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.State loop;

	private GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.State pst;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.GameInstance
	{
		public Instance(Chore<FallStates.Instance> chore, FallStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(FallStates.Instance.IsFalling, null);
		}

		public static Chore.Precondition IsFalling = new Chore.Precondition
		{
			id = "IsFalling",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Creatures.Falling);
			}
		};
	}
}
