using System;
using STRINGS;

internal class DrowningStates : GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.drowning;
		GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>.State root = this.root;
		string text = CREATURES.STATUSITEMS.DROWNING.NAME;
		string text2 = CREATURES.STATUSITEMS.DROWNING.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486, null, null, main);
		this.drowning.PlayAnim("harvest", KAnim.PlayMode.Loop).TagTransition(GameTags.Creatures.Drowning, null, true);
	}

	public GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>.State drowning;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>.GameInstance
	{
		public Instance(Chore<DrowningStates.Instance> chore, DrowningStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.HasTag, GameTags.Creatures.Drowning);
		}
	}
}
