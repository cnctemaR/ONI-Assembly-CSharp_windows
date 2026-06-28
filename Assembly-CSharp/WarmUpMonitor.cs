using System;

public class WarmUpMonitor : GameStateMachine<WarmUpMonitor, WarmUpMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.warm;
		this.warm.Transition(this.cold, (WarmUpMonitor.Instance smi) => smi.IsCold());
		this.cold.ToggleUrge(Db.Get().Urges.WarmUp).Transition(this.warm, (WarmUpMonitor.Instance smi) => smi.IsWarm());
	}

	private const float COLD_DELTA = -5f;

	private const float WARM_DELTA = 0f;

	public GameStateMachine<WarmUpMonitor, WarmUpMonitor.Instance, IStateMachineTarget>.State warm;

	public GameStateMachine<WarmUpMonitor, WarmUpMonitor.Instance, IStateMachineTarget>.State cold;

	public new class Instance : GameStateMachine<WarmUpMonitor, WarmUpMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.warmBlooded = master.GetComponent<WarmBlooded>();
		}

		public bool IsCold()
		{
			return this.warmBlooded.temperature.value < 305.15f;
		}

		public bool IsWarm()
		{
			return this.warmBlooded.temperature.value > 310.15f;
		}

		private WarmBlooded warmBlooded;
	}
}
