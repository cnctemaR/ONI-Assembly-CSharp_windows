using System;

public class InRangeSM : GameStateMachine<InRangeSM, InRangeSM.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = false;
		default_state = this.ranges;
		this.root.Enter(delegate(InRangeSM.Instance smi)
		{
			smi.CheckRange();
		});
		this.ranges.DefaultState(this.ranges.outside).ToggleSchedulePeriodic("InRangeCheck", (InRangeSM.Instance smi) => smi.checkInterval, delegate(InRangeSM.Instance smi)
		{
			smi.CheckRange();
		}, null);
		this.ranges.outside.ToggleStatusItem((InRangeSM.Instance smi) => smi.statusItem, (InRangeSM.Instance smi) => smi.master);
	}

	public InRangeSM.RangeStates ranges;

	public class RangeStates : GameStateMachine<InRangeSM, InRangeSM.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<InRangeSM, InRangeSM.Instance, IStateMachineTarget, object>.State outside;

		public GameStateMachine<InRangeSM, InRangeSM.Instance, IStateMachineTarget, object>.State inside;
	}

	public new class Instance : GameStateMachine<InRangeSM, InRangeSM.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, float check_interval, Func<bool> any_target_in_range_func, StatusItem outside_range_status_item)
			: base(master)
		{
			this.checkInterval = check_interval;
			this.anyTargetInRangeFunc = any_target_in_range_func;
			this.statusItem = outside_range_status_item;
		}

		public void CheckRange()
		{
			if (this.anyTargetInRangeFunc())
			{
				this.GoTo(base.smi.sm.ranges.inside);
			}
			else
			{
				this.GoTo(base.smi.sm.ranges.outside);
			}
		}

		public float checkInterval;

		public StatusItem statusItem;

		private Func<bool> anyTargetInRangeFunc;
	}
}
