using System;

public class FallWhenDeadMonitor : GameStateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.standing;
		this.standing.Transition(this.entombed, (FallWhenDeadMonitor.Instance smi) => smi.IsEntombed(), UpdateRate.SIM_200ms).Transition(this.falling, (FallWhenDeadMonitor.Instance smi) => smi.IsFalling(), UpdateRate.SIM_200ms);
		this.falling.ToggleGravity(this.standing);
		this.entombed.Transition(this.standing, (FallWhenDeadMonitor.Instance smi) => !smi.IsEntombed(), UpdateRate.SIM_200ms);
	}

	public GameStateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance, IStateMachineTarget, object>.State standing;

	public GameStateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance, IStateMachineTarget, object>.State falling;

	public GameStateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance, IStateMachineTarget, object>.State entombed;

	public new class Instance : GameStateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public bool IsEntombed()
		{
			Pickupable component = base.GetComponent<Pickupable>();
			return component != null && component.IsEntombed;
		}

		public bool IsFalling()
		{
			int num = Grid.PosToCell(base.master.transform.GetPosition());
			int num2 = Grid.CellBelow(num);
			return Grid.IsValidCell(num2) && !Grid.Solid[num2];
		}
	}
}
