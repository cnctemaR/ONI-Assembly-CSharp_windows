using System;

public class WorldSpawnableMonitor : GameStateMachine<WorldSpawnableMonitor, WorldSpawnableMonitor.Instance, IStateMachineTarget, WorldSpawnableMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
	}

	public class Def : StateMachine.BaseDef
	{
		public Func<int, int> adjustSpawnLocationCb;
	}

	public new class Instance : GameStateMachine<WorldSpawnableMonitor, WorldSpawnableMonitor.Instance, IStateMachineTarget, WorldSpawnableMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, WorldSpawnableMonitor.Def def)
			: base(master, def)
		{
		}
	}
}
