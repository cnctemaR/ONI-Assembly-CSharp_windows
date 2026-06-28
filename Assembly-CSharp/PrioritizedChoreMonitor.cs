using System;

public class PrioritizedChoreMonitor : GameStateMachine<PrioritizedChoreMonitor, PrioritizedChoreMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
	}

	public GameStateMachine<PrioritizedChoreMonitor, PrioritizedChoreMonitor.Instance, IStateMachineTarget>.State satisfied;

	public new class Instance : GameStateMachine<PrioritizedChoreMonitor, PrioritizedChoreMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void SetCommand(PriorityCommand new_command)
		{
			if (this.command != null)
			{
				this.command.Cleanup();
				this.command = null;
			}
			this.command = new_command;
			new_command.Begin();
		}

		private PriorityCommand command;
	}
}
