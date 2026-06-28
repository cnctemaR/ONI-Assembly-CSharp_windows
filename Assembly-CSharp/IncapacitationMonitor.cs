using System;

public class IncapacitationMonitor : GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.healthy;
		base.serializable = true;
		this.notHealthy.ToggleUrge(Db.Get().Urges.BeIncapacitated).ToggleChore((IncapacitationMonitor.Instance smi) => new BeIncapacitatedChore(smi.master), this.notHealthy.Incapacitated, true).EventTransition(GameHashes.IncapacitationRecovery, this.healthy, null);
	}

	public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget>.State healthy;

	public IncapacitationMonitor.NotHealth notHealthy;

	public class NotHealth : GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget>.State
	{
		public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget>.State Incapacitated;

		public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget>.State Recovering;
	}

	public new class Instance : GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget>.GameInstance
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
			this.isIncapacitated = true;
			base.smi.GoTo(base.sm.notHealthy.Incapacitated);
		}

		public bool isIncapacitated;
	}
}
