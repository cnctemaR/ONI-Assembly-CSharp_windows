using System;

public class SuitRegionMonitor : GameStateMachine<SuitRegionMonitor, SuitRegionMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.root.Update("UpdateIsInSuitRegion", delegate(SuitRegionMonitor.Instance smi)
		{
			smi.UpdateIsInSuitRegion();
		});
		this.satisfied.DoNothing();
		this.needstoexitregion.DoNothing();
	}

	public GameStateMachine<SuitRegionMonitor, SuitRegionMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public GameStateMachine<SuitRegionMonitor, SuitRegionMonitor.Instance, IStateMachineTarget, object>.State needstoexitregion;

	public StateMachine<SuitRegionMonitor, SuitRegionMonitor.Instance, IStateMachineTarget, object>.BoolParameter isInSuitRegion;

	public new class Instance : GameStateMachine<SuitRegionMonitor, SuitRegionMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void UpdateIsInSuitRegion()
		{
			int num = Grid.PosToCell(base.transform.position);
			int num2 = Grid.CellAbove(num);
			base.sm.isInSuitRegion.Set(Grid.SuitRequired[num] || Grid.SuitRequired[num2], base.smi);
		}
	}
}
