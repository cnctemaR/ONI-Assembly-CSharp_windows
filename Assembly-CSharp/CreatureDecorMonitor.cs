using System;

public class CreatureDecorMonitor : GameStateMachine<CreatureDecorMonitor, CreatureDecorMonitor.Instance, IStateMachineTarget, CreatureDecorMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.lowDecor;
		this.lowDecor.UpdateTransition(this.highDecor, new Func<CreatureDecorMonitor.Instance, float, bool>(CreatureDecorMonitor.IsInHighDecor), UpdateRate.SIM_4000ms, false).Update(new Action<CreatureDecorMonitor.Instance, float>(CreatureDecorMonitor.TriggerLowDecorUpdate), UpdateRate.SIM_4000ms, false).TriggerOnEnter(GameHashes.CreatureLowDecor, null);
		this.highDecor.UpdateTransition(this.lowDecor, new Func<CreatureDecorMonitor.Instance, float, bool>(CreatureDecorMonitor.IsInLowDecor), UpdateRate.SIM_4000ms, false).Update(new Action<CreatureDecorMonitor.Instance, float>(CreatureDecorMonitor.TriggerHighDecorUpdate), UpdateRate.SIM_4000ms, false).TriggerOnEnter(GameHashes.CreatureHighDecor, null);
	}

	private static void TriggerHighDecorUpdate(CreatureDecorMonitor.Instance smi, float dt)
	{
		Action<float> onHighDecorUpdate = smi.OnHighDecorUpdate;
		if (onHighDecorUpdate == null)
		{
			return;
		}
		onHighDecorUpdate(dt);
	}

	private static void TriggerLowDecorUpdate(CreatureDecorMonitor.Instance smi, float dt)
	{
		Action<float> onLowDecorUpdate = smi.OnLowDecorUpdate;
		if (onLowDecorUpdate == null)
		{
			return;
		}
		onLowDecorUpdate(dt);
	}

	private static bool IsInHighDecor(CreatureDecorMonitor.Instance smi, float dt)
	{
		return Grid.Decor[Grid.PosToCell(smi)] >= smi.def.DecorValueTreshold;
	}

	private static bool IsInLowDecor(CreatureDecorMonitor.Instance smi, float dt)
	{
		return !CreatureDecorMonitor.IsInHighDecor(smi, dt);
	}

	private const UpdateRate UPDATE_RATE = UpdateRate.SIM_4000ms;

	private GameStateMachine<CreatureDecorMonitor, CreatureDecorMonitor.Instance, IStateMachineTarget, CreatureDecorMonitor.Def>.State lowDecor;

	private GameStateMachine<CreatureDecorMonitor, CreatureDecorMonitor.Instance, IStateMachineTarget, CreatureDecorMonitor.Def>.State highDecor;

	public class Def : StateMachine.BaseDef
	{
		public float DecorValueTreshold;
	}

	public new class Instance : GameStateMachine<CreatureDecorMonitor, CreatureDecorMonitor.Instance, IStateMachineTarget, CreatureDecorMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CreatureDecorMonitor.Def def)
			: base(master, def)
		{
		}

		public Action<float> OnHighDecorUpdate;

		public Action<float> OnLowDecorUpdate;
	}
}
