using System;

public class PeeChoreMonitor : GameStateMachine<PeeChoreMonitor, PeeChoreMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.building;
		base.serializable = true;
		this.building.Update(delegate(PeeChoreMonitor.Instance smi, float dt)
		{
			this.pee_fuse.Delta(-dt, smi);
		}).Transition(this.paused, (PeeChoreMonitor.Instance smi) => this.IsSleeping(smi), UpdateRate.SIM_200ms).Transition(this.critical, (PeeChoreMonitor.Instance smi) => this.pee_fuse.Get(smi) <= 60f, UpdateRate.SIM_200ms);
		this.critical.Update(delegate(PeeChoreMonitor.Instance smi, float dt)
		{
			this.pee_fuse.Delta(-dt, smi);
		}).Transition(this.paused, (PeeChoreMonitor.Instance smi) => this.IsSleeping(smi), UpdateRate.SIM_200ms).Transition(this.pee, (PeeChoreMonitor.Instance smi) => this.pee_fuse.Get(smi) <= 0f, UpdateRate.SIM_200ms);
		this.paused.Transition(this.building, (PeeChoreMonitor.Instance smi) => !this.IsSleeping(smi), UpdateRate.SIM_200ms);
		this.pee.ToggleChore(new Func<PeeChoreMonitor.Instance, Chore>(this.CreatePeeChore), this.building);
	}

	private bool IsSleeping(PeeChoreMonitor.Instance smi)
	{
		StaminaMonitor.Instance smi2 = smi.master.gameObject.GetSMI<StaminaMonitor.Instance>();
		return (smi2 == null || !smi2.IsSleeping()) && false;
	}

	private Chore CreatePeeChore(PeeChoreMonitor.Instance smi)
	{
		return new PeeChore(smi.master);
	}

	public GameStateMachine<PeeChoreMonitor, PeeChoreMonitor.Instance, IStateMachineTarget, object>.State building;

	public GameStateMachine<PeeChoreMonitor, PeeChoreMonitor.Instance, IStateMachineTarget, object>.State critical;

	public GameStateMachine<PeeChoreMonitor, PeeChoreMonitor.Instance, IStateMachineTarget, object>.State paused;

	public GameStateMachine<PeeChoreMonitor, PeeChoreMonitor.Instance, IStateMachineTarget, object>.State pee;

	private StateMachine<PeeChoreMonitor, PeeChoreMonitor.Instance, IStateMachineTarget, object>.FloatParameter pee_fuse = new StateMachine<PeeChoreMonitor, PeeChoreMonitor.Instance, IStateMachineTarget, object>.FloatParameter(120f);

	public new class Instance : GameStateMachine<PeeChoreMonitor, PeeChoreMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}
}
