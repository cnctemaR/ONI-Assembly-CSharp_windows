using System;
using KSerialization;

public class RoverChoreMonitor : GameStateMachine<RoverChoreMonitor, RoverChoreMonitor.Instance, IStateMachineTarget, RoverChoreMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.loop;
		this.loop.ToggleBehaviour(GameTags.Creatures.Tunnel, (RoverChoreMonitor.Instance smi) => true, null).ToggleBehaviour(GameTags.Creatures.Builder, (RoverChoreMonitor.Instance smi) => true, null);
	}

	public GameStateMachine<RoverChoreMonitor, RoverChoreMonitor.Instance, IStateMachineTarget, RoverChoreMonitor.Def>.State loop;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<RoverChoreMonitor, RoverChoreMonitor.Instance, IStateMachineTarget, RoverChoreMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, RoverChoreMonitor.Def def)
			: base(master, def)
		{
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
		}

		[Serialize]
		public int lastDigCell = -1;

		private Action<object> OnDestinationReachedDelegate;
	}
}
