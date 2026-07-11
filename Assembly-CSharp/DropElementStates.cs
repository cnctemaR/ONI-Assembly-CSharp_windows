using System;
using STRINGS;

public class DropElementStates : GameStateMachine<DropElementStates, DropElementStates.Instance, IStateMachineTarget, DropElementStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.dropping;
		this.root.ToggleStatusItem(CREATURES.STATUSITEMS.EXPELLING_GAS.NAME, CREATURES.STATUSITEMS.EXPELLING_GAS.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.dropping.PlayAnim("dirty").OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.Enter("DropElement", delegate(DropElementStates.Instance smi)
		{
			smi.GetSMI<ElementDropperMonitor.Instance>().DropPeriodicElement();
		}).QueueAnim("idle_loop", true, null).BehaviourComplete(GameTags.Creatures.WantsToDropElements, false);
	}

	public GameStateMachine<DropElementStates, DropElementStates.Instance, IStateMachineTarget, DropElementStates.Def>.State dropping;

	public GameStateMachine<DropElementStates, DropElementStates.Instance, IStateMachineTarget, DropElementStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<DropElementStates, DropElementStates.Instance, IStateMachineTarget, DropElementStates.Def>.GameInstance
	{
		public Instance(Chore<DropElementStates.Instance> chore, DropElementStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToDropElements);
		}
	}
}
