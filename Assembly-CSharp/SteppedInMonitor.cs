using System;

public class SteppedInMonitor : GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.Transition(this.indirtywater, (SteppedInMonitor.Instance smi) => smi.IsInDirtyWater());
		this.indirtywater.Transition(this.satisfied, (SteppedInMonitor.Instance smi) => !smi.IsInDirtyWater()).AddEffect("SteppedInContaminatedWater");
	}

	public GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget>.State satisfied;

	public GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget>.State indirtywater;

	public new class Instance : GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public bool IsInDirtyWater()
		{
			int num = Grid.PosToCell(base.transform.position);
			int num2 = Grid.CellAbove(num);
			return Grid.Element[num].id == SimHashes.DirtyWater || Grid.Element[num2].id == SimHashes.DirtyWater;
		}
	}
}
