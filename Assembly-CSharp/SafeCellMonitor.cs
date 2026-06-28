using System;

public class SafeCellMonitor : GameStateMachine<SafeCellMonitor, SafeCellMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = false;
		this.root.ToggleUrge(Db.Get().Urges.MoveToSafety);
		this.satisfied.EventTransition(GameHashes.SafeCellDetected, this.danger, (SafeCellMonitor.Instance smi) => smi.IsAreaUnsafe());
		this.danger.EventTransition(GameHashes.SafeCellLost, this.satisfied, (SafeCellMonitor.Instance smi) => !smi.IsAreaUnsafe()).ToggleChore((SafeCellMonitor.Instance smi) => new MoveToSafetyChore(smi.master), this.satisfied, false);
	}

	public GameStateMachine<SafeCellMonitor, SafeCellMonitor.Instance, IStateMachineTarget>.State satisfied;

	public GameStateMachine<SafeCellMonitor, SafeCellMonitor.Instance, IStateMachineTarget>.State danger;

	public new class Instance : GameStateMachine<SafeCellMonitor, SafeCellMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.safeCellSensor = base.GetComponent<Sensors>().GetSensor<SafeCellSensor>();
		}

		public bool IsAreaUnsafe()
		{
			return this.safeCellSensor.HasSafeCell();
		}

		private SafeCellSensor safeCellSensor;
	}
}
