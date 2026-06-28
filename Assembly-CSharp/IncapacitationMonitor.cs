using System;

public class IncapacitationMonitor : GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.healthy;
		base.serializable = true;
		this.notHealthy.ToggleUrge(Db.Get().Urges.BeIncapacitated).ToggleChore((IncapacitationMonitor.Instance smi) => new BeIncapacitatedChore(smi.master), this.healthy, true).EventTransition(GameHashes.IncapacitationRecovery, this.healthy, null);
	}

	public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State healthy;

	public IncapacitationMonitor.NotHealth notHealthy;

	public class NotHealth : GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State Incapacitated;

		public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State Recovering;
	}

	public new class Instance : GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			Health component = master.GetComponent<Health>();
			if (component)
			{
				component.CanBeIncapacitated = true;
			}
		}

		public void Incapacitate()
		{
			base.smi.GoTo(base.sm.notHealthy.Incapacitated);
		}
	}
}
