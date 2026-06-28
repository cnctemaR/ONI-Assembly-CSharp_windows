using System;

public class ReachabilityMonitor : GameStateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.reachable;
		base.serializable = false;
		this.root.ToggleSchedulePeriodic("UpdateReachability", 3f, delegate(ReachabilityMonitor.Instance smi)
		{
			smi.UpdateReachability();
		});
		this.reachable.Enter("TriggerEvent", delegate(ReachabilityMonitor.Instance smi)
		{
			smi.TriggerEvent();
		}).ParamTransition<bool>(this.isReachable, this.unreachable, (ReachabilityMonitor.Instance smi, bool p) => !p);
		this.unreachable.Enter("TriggerEvent", delegate(ReachabilityMonitor.Instance smi)
		{
			smi.TriggerEvent();
		}).ParamTransition<bool>(this.isReachable, this.reachable, (ReachabilityMonitor.Instance smi, bool p) => p);
	}

	public GameStateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable>.State reachable;

	public GameStateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable>.State unreachable;

	public StateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable>.BoolParameter isReachable = new StateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable>.BoolParameter(true);

	public new class Instance : GameStateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable>.GameInstance
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
			if (flag)
			{
				Game.Instance.Trigger(1992428732, base.gameObject);
			}
			else
			{
				Game.Instance.Trigger(124211798, base.gameObject);
			}
		}

		public void UpdateReachability()
		{
			if (base.master != null)
			{
				CellOffset[] offsets = base.master.GetOffsets();
				bool flag = MinionGroupProber.Get().IsReachable(Grid.PosToCell(base.master), offsets);
				base.sm.isReachable.Set(flag, base.smi);
			}
		}
	}
}
