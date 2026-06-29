using System;

public class CreatureSleepMonitor : GameStateMachine<CreatureSleepMonitor, CreatureSleepMonitor.Instance, IStateMachineTarget, CreatureSleepMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ToggleBehaviour(GameTags.Creatures.Behaviours.SleepBehaviour, new Func<CreatureSleepMonitor.Instance, bool>(CreatureSleepMonitor.ShouldSleep), null);
	}

	public static bool ShouldSleep(CreatureSleepMonitor.Instance smi)
	{
		return GameClock.Instance.IsNighttime();
	}

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<CreatureSleepMonitor, CreatureSleepMonitor.Instance, IStateMachineTarget, CreatureSleepMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CreatureSleepMonitor.Def def)
			: base(master, def)
		{
		}
	}
}
