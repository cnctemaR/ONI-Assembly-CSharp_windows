using System;

public class SafeCellMonitor : GameStateMachine<SafeCellMonitor, SafeCellMonitor.Instance, IStateMachineTarget, SafeCellMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.safe;
		this.root.ToggleUrge(Db.Get().Urges.MoveToSafety);
		this.safe.EventTransition(GameHashes.SafeCellDetected, this.danger, (SafeCellMonitor.Instance smi) => smi.IsAreaUnsafe());
		this.danger.EventTransition(GameHashes.SafeCellLost, this.safe, (SafeCellMonitor.Instance smi) => !smi.IsAreaUnsafe()).ToggleChore((SafeCellMonitor.Instance smi) => new MoveToSafetyChore(smi.master), this.safe);
	}

	public GameStateMachine<SafeCellMonitor, SafeCellMonitor.Instance, IStateMachineTarget, SafeCellMonitor.Def>.State safe;

	public GameStateMachine<SafeCellMonitor, SafeCellMonitor.Instance, IStateMachineTarget, SafeCellMonitor.Def>.State danger;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<SafeCellMonitor, SafeCellMonitor.Instance, IStateMachineTarget, SafeCellMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, SafeCellMonitor.Def def)
			: base(master, def)
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
