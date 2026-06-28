using System;
using Klei.AI;

public class SteppedInMonitor : GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.Transition(this.wetFloor, (SteppedInMonitor.Instance smi) => smi.IsFloorWet(), UpdateRate.SIM_200ms).Transition(this.wetBody, (SteppedInMonitor.Instance smi) => smi.IsSubmerged(), UpdateRate.SIM_200ms);
		this.wetFloor.Enter(delegate(SteppedInMonitor.Instance smi)
		{
			smi.GetWetFeet(null);
		}).Update("GetWetFeet", delegate(SteppedInMonitor.Instance smi, float dt)
		{
			smi.GetWetFeet(null);
		}, UpdateRate.SIM_1000ms, false).Transition(this.satisfied, (SteppedInMonitor.Instance smi) => !smi.IsFloorWet(), UpdateRate.SIM_200ms)
			.Transition(this.wetBody, (SteppedInMonitor.Instance smi) => smi.IsSubmerged(), UpdateRate.SIM_200ms);
		this.wetBody.Enter(delegate(SteppedInMonitor.Instance smi)
		{
			smi.GetSoaked(null);
		}).Update("GetSoaked", delegate(SteppedInMonitor.Instance smi, float dt)
		{
			smi.GetSoaked(null);
		}, UpdateRate.SIM_1000ms, false).Transition(this.wetFloor, (SteppedInMonitor.Instance smi) => !smi.IsSubmerged(), UpdateRate.SIM_200ms);
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

		public bool IsFloorWet()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			return Grid.Element[num].IsLiquid;
		}

		public bool IsSubmerged()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			int num2 = Grid.CellAbove(num);
			return Grid.Element[num2].IsLiquid;
		}

		public void GetWetFeet(object data)
		{
			if (!this.effects.HasEffect("SoakingWet"))
			{
				this.effects.Add("WetFeet", true);
			}
		}

		public void GetSoaked(object data)
		{
			if (this.effects.HasEffect("WetFeet"))
			{
				this.effects.Remove("WetFeet");
			}
			this.effects.Add("SoakingWet", true);
		}

		private Effects effects;
	}
}
