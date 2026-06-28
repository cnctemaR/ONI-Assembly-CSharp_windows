using System;

public class DebugGoToMonitor : GameStateMachine<DebugGoToMonitor, DebugGoToMonitor.Instance, IStateMachineTarget, DebugGoToMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.DoNothing();
		this.hastarget.TagTransition(GameTags.HasDebugDestination, this.satisfied, true).ToggleChore((DebugGoToMonitor.Instance smi) => new MoveChore(smi.master, Db.Get().ChoreTypes.DebugGoTo, (MoveChore.StatesInstance smii) => DebugHandler.GetMouseCell(), false), this.satisfied);
	}

	public GameStateMachine<DebugGoToMonitor, DebugGoToMonitor.Instance, IStateMachineTarget, DebugGoToMonitor.Def>.State satisfied;

	public GameStateMachine<DebugGoToMonitor, DebugGoToMonitor.Instance, IStateMachineTarget, DebugGoToMonitor.Def>.State hastarget;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<DebugGoToMonitor, DebugGoToMonitor.Instance, IStateMachineTarget, DebugGoToMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget target, DebugGoToMonitor.Def def)
			: base(target, def)
		{
		}

		public void GoToCursor()
		{
			base.smi.gameObject.AddTag(GameTags.HasDebugDestination);
			base.smi.GoTo(base.smi.sm.satisfied);
			base.smi.GoTo(base.smi.sm.hastarget);
		}
	}
}
