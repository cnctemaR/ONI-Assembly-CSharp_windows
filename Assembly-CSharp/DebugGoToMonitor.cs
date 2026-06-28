using System;

public class DebugGoToMonitor : StateMachineComponent<DebugGoToMonitor.StatesInstance>
{
	public void GoToCursor()
	{
		base.smi.GoTo(base.smi.sm.satisfied);
		base.smi.GoTo(base.smi.sm.moving);
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public class StatesInstance : GameStateMachine<DebugGoToMonitor.States, DebugGoToMonitor.StatesInstance, DebugGoToMonitor, object>.GameInstance
	{
		public StatesInstance(DebugGoToMonitor smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<DebugGoToMonitor.States, DebugGoToMonitor.StatesInstance, DebugGoToMonitor>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.satisfied;
			this.satisfied.DoNothing();
			this.moving.ToggleChore((DebugGoToMonitor.StatesInstance smi) => new MoveChore(smi.master, Db.Get().ChoreTypes.DebugGoTo, (MoveChore.StatesInstance smii) => DebugHandler.GetMouseCell(), false), this.satisfied);
		}

		public GameStateMachine<DebugGoToMonitor.States, DebugGoToMonitor.StatesInstance, DebugGoToMonitor, object>.State satisfied;

		public GameStateMachine<DebugGoToMonitor.States, DebugGoToMonitor.StatesInstance, DebugGoToMonitor, object>.State moving;
	}
}
