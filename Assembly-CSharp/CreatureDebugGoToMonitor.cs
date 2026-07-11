using System;

public class CreatureDebugGoToMonitor : GameStateMachine<CreatureDebugGoToMonitor, CreatureDebugGoToMonitor.Instance, IStateMachineTarget, CreatureDebugGoToMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ToggleBehaviour(GameTags.HasDebugDestination, new StateMachine<CreatureDebugGoToMonitor, CreatureDebugGoToMonitor.Instance, IStateMachineTarget, CreatureDebugGoToMonitor.Def>.Transition.ConditionCallback(CreatureDebugGoToMonitor.HasTargetCell), new Action<CreatureDebugGoToMonitor.Instance>(CreatureDebugGoToMonitor.ClearTargetCell));
	}

	private static bool HasTargetCell(CreatureDebugGoToMonitor.Instance smi)
	{
		return smi.targetCell != Grid.InvalidCell;
	}

	private static void ClearTargetCell(CreatureDebugGoToMonitor.Instance smi)
	{
		smi.targetCell = Grid.InvalidCell;
	}

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<CreatureDebugGoToMonitor, CreatureDebugGoToMonitor.Instance, IStateMachineTarget, CreatureDebugGoToMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget target, CreatureDebugGoToMonitor.Def def)
			: base(target, def)
		{
		}

		public void GoToCursor()
		{
			this.targetCell = DebugHandler.GetMouseCell();
		}

		public int targetCell = Grid.InvalidCell;
	}
}
