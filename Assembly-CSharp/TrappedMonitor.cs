using System;

public class TrappedMonitor : GameStateMachine<TrappedMonitor, TrappedMonitor.Instance, IStateMachineTarget, TrappedMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.free;
		this.free.DoNothing();
		this.trapped.ToggleTag(GameTags.Trapped);
	}

	public GameStateMachine<TrappedMonitor, TrappedMonitor.Instance, IStateMachineTarget, TrappedMonitor.Def>.State free;

	public GameStateMachine<TrappedMonitor, TrappedMonitor.Instance, IStateMachineTarget, TrappedMonitor.Def>.State trapped;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<TrappedMonitor, TrappedMonitor.Instance, IStateMachineTarget, TrappedMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, TrappedMonitor.Def def)
			: base(master, def)
		{
		}
	}
}
