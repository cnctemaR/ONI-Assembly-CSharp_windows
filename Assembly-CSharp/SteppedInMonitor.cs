using System;
using Klei.AI;

public class SteppedInMonitor : GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.Transition(this.wetFloor, new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsFloorWet), UpdateRate.SIM_200ms).Transition(this.wetBody, new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsSubmerged), UpdateRate.SIM_200ms);
		this.wetFloor.Enter(new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State.Callback(SteppedInMonitor.GetWetFeet)).Update(new Action<SteppedInMonitor.Instance, float>(SteppedInMonitor.GetWetFeet), UpdateRate.SIM_1000ms, false).Transition(this.satisfied, GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Not(new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsFloorWet)), UpdateRate.SIM_200ms)
			.Transition(this.wetBody, new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsSubmerged), UpdateRate.SIM_200ms);
		this.wetBody.Enter(new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State.Callback(SteppedInMonitor.GetSoaked)).Update(new Action<SteppedInMonitor.Instance, float>(SteppedInMonitor.GetSoaked), UpdateRate.SIM_1000ms, false).Transition(this.wetFloor, GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Not(new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsSubmerged)), UpdateRate.SIM_200ms);
	}

	private static void GetWetFeet(SteppedInMonitor.Instance smi, float dt)
	{
		SteppedInMonitor.GetWetFeet(smi);
	}

	private static void GetWetFeet(SteppedInMonitor.Instance smi)
	{
		if (!smi.effects.HasEffect("SoakingWet"))
		{
			smi.effects.Add("WetFeet", true);
		}
	}

	private static void GetSoaked(SteppedInMonitor.Instance smi, float dt)
	{
		SteppedInMonitor.GetSoaked(smi);
	}

	private static void GetSoaked(SteppedInMonitor.Instance smi)
	{
		if (smi.effects.HasEffect("WetFeet"))
		{
			smi.effects.Remove("WetFeet");
		}
		smi.effects.Add("SoakingWet", true);
	}

	private static bool IsFloorWet(SteppedInMonitor.Instance smi)
	{
		int num = Grid.PosToCell(smi);
		return Grid.IsValidCell(num) && Grid.Element[num].IsLiquid;
	}

	private static bool IsSubmerged(SteppedInMonitor.Instance smi)
	{
		int num = Grid.PosToCell(smi);
		int num2 = Grid.CellAbove(num);
		return Grid.IsValidCell(num2) && Grid.Element[num2].IsLiquid;
	}

	public GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State wetFloor;

	public GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State wetBody;

	public new class Instance : GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.effects = base.GetComponent<Effects>();
		}

		public Effects effects;
	}
}
