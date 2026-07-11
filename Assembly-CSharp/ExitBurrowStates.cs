using System;
using STRINGS;

public class ExitBurrowStates : GameStateMachine<ExitBurrowStates, ExitBurrowStates.Instance, IStateMachineTarget, ExitBurrowStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.exiting;
		this.root.ToggleStatusItem(CREATURES.STATUSITEMS.EMERGING.NAME, CREATURES.STATUSITEMS.EMERGING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.exiting.PlayAnim("emerge").Enter(new StateMachine<ExitBurrowStates, ExitBurrowStates.Instance, IStateMachineTarget, ExitBurrowStates.Def>.State.Callback(ExitBurrowStates.MoveToCellAbove)).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToExitBurrow, false);
	}

	private static void MoveToCellAbove(ExitBurrowStates.Instance smi)
	{
		smi.transform.SetPosition(Grid.CellToPosCBC(Grid.CellAbove(Grid.PosToCell(smi.transform.GetPosition())), Grid.SceneLayer.Creatures));
	}

	private GameStateMachine<ExitBurrowStates, ExitBurrowStates.Instance, IStateMachineTarget, ExitBurrowStates.Def>.State exiting;

	private GameStateMachine<ExitBurrowStates, ExitBurrowStates.Instance, IStateMachineTarget, ExitBurrowStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<ExitBurrowStates, ExitBurrowStates.Instance, IStateMachineTarget, ExitBurrowStates.Def>.GameInstance
	{
		public Instance(Chore<ExitBurrowStates.Instance> chore, ExitBurrowStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToExitBurrow);
		}
	}
}
