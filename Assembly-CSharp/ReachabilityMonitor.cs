using System;

public class ReachabilityMonitor : GameStateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.unreachable;
		base.serializable = false;
		this.root.FastUpdate("UpdateReachability", ReachabilityMonitor.updateReachabilityCB, UpdateRate.SIM_1000ms, true);
		this.reachable.ToggleTag(GameTags.Reachable).Enter("TriggerEvent", delegate(ReachabilityMonitor.Instance smi)
		{
			smi.TriggerEvent();
		}).ParamTransition<bool>(this.isReachable, this.unreachable, (ReachabilityMonitor.Instance smi, bool p) => !p);
		this.unreachable.Enter("TriggerEvent", delegate(ReachabilityMonitor.Instance smi)
		{
			smi.TriggerEvent();
		}).ParamTransition<bool>(this.isReachable, this.reachable, (ReachabilityMonitor.Instance smi, bool p) => p);
	}

	public GameStateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable, object>.State reachable;

	public GameStateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable, object>.State unreachable;

	public StateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable, object>.BoolParameter isReachable = new StateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable, object>.BoolParameter(false);

	private static ReachabilityMonitor.UpdateReachabilityCB updateReachabilityCB = new ReachabilityMonitor.UpdateReachabilityCB();

	private class UpdateReachabilityCB : UpdateBucketWithUpdater<ReachabilityMonitor.Instance>.IUpdater
	{
		public void Update(ReachabilityMonitor.Instance smi, float dt)
		{
			smi.UpdateReachability();
		}
	}

	public new class Instance : GameStateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable, object>.GameInstance
	{
		public Instance(Workable workable)
			: base(workable)
		{
			this.UpdateReachability();
		}

		public void TriggerEvent()
		{
			bool flag = base.sm.isReachable.Get(base.smi);
			base.Trigger(-1432940121, flag);
		}

		public void UpdateReachability()
		{
			if (base.master != null)
			{
				int num = Grid.PosToCell(base.master);
				CellOffset[] offsets = base.master.GetOffsets(num);
				MinionGroupProber minionGroupProber = MinionGroupProber.Get();
				bool flag = minionGroupProber.IsReachable(num) || minionGroupProber.IsReachable(num, offsets);
				base.sm.isReachable.Set(flag, base.smi);
			}
		}
	}
}
