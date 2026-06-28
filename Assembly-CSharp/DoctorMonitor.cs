using System;

public class DoctorMonitor : GameStateMachine<DoctorMonitor, DoctorMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		base.serializable = true;
		this.root.ToggleUrge(Db.Get().Urges.Doctor);
	}

	public new class Instance : GameStateMachine<DoctorMonitor, DoctorMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}
}
