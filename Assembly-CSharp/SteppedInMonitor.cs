using System;
using Klei.AI;

public class SteppedInMonitor : GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.Transition(this.wetFloor, (SteppedInMonitor.Instance smi) => smi.IsFloorWet()).Transition(this.wetBody, (SteppedInMonitor.Instance smi) => smi.IsSubmerged());
		this.wetFloor.Enter(delegate(SteppedInMonitor.Instance smi)
		{
			smi.GetWetFeet(null);
		}).ToggleSchedulePeriodic("GetWetFeet", 2f, delegate(SteppedInMonitor.Instance smi)
		{
			smi.GetWetFeet(null);
		}).Transition(this.satisfied, (SteppedInMonitor.Instance smi) => !smi.IsFloorWet())
			.Transition(this.wetBody, (SteppedInMonitor.Instance smi) => smi.IsSubmerged());
		this.wetBody.Enter(delegate(SteppedInMonitor.Instance smi)
		{
			smi.GetSoaked(null);
		}).ToggleSchedulePeriodic("GetSoaked", 2f, delegate(SteppedInMonitor.Instance smi)
		{
			smi.GetSoaked(null);
		}).Transition(this.wetFloor, (SteppedInMonitor.Instance smi) => !smi.IsSubmerged());
	}

	public GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State wetFloor;

	public GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State wetBody;

	public new class Instance : GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public bool IsFloorWet()
		{
			int num = Grid.PosToCell(base.transform.position);
			return Grid.Element[num].IsLiquid;
		}

		public bool IsSubmerged()
		{
			int num = Grid.PosToCell(base.transform.position);
			int num2 = Grid.CellAbove(num);
			return Grid.Element[num2].IsLiquid;
		}

		public void GetWetFeet(object data)
		{
			if (!base.smi.master.GetComponent<Effects>().HasEffect("SoakingWet"))
			{
				base.smi.master.GetComponent<Effects>().Add("WetFeet", true);
			}
		}

		public void GetSoaked(object data)
		{
			if (base.smi.master.GetComponent<Effects>().HasEffect("WetFeet"))
			{
				base.smi.master.GetComponent<Effects>().Remove("WetFeet");
			}
			base.smi.master.GetComponent<Effects>().Add("SoakingWet", true);
		}
	}
}
