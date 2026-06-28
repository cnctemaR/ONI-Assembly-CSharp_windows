using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WorkChore<WorkableType> : Chore<WorkChore<WorkableType>.StatesInstance> where WorkableType : Workable
{
	public WorkChore(ChoreType chore_type, IStateMachineTarget target, ChoreProvider chore_provider = null, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null, bool allow_in_red_alert = true, ScheduleBlockType schedule_block = null, bool only_when_operational = true, [Optional] Tag required_region, KAnimFile override_anims = null, bool is_preemptable = false, bool allow_in_context_menu = true)
		: base(chore_type, target, chore_provider, run_until_complete, on_complete, on_begin, on_end, int.MaxValue, is_preemptable, allow_in_context_menu)
	{
		this.smi = new WorkChore<WorkableType>.StatesInstance(this, target.gameObject, override_anims);
		base.SetPrioritizable(target.GetComponent<Prioritizable>());
		if (!allow_in_red_alert)
		{
			base.AddPrecondition(ChorePreconditions.IsNotRedAlert, null);
		}
		if (schedule_block != null)
		{
			base.AddPrecondition(ChorePreconditions.IsScheduledTime, schedule_block);
		}
		base.AddPrecondition(ChorePreconditions.CanMoveTo, this.smi.sm.workable.Get<WorkableType>(this.smi));
		if (only_when_operational && target.gameObject.GetComponent<Operational>() != null)
		{
			base.AddPrecondition(ChorePreconditions.IsOperational, target.gameObject);
			target.Subscribe(-592767678, new EventSystem.EventHandler(this.OnOperationalChanged));
		}
		if (only_when_operational && target.gameObject.GetComponent<Deconstructable>() != null)
		{
			base.AddPrecondition(ChorePreconditions.IsMarkedForDeconstruction, target.gameObject);
		}
		if (required_region.GetHashCode() != 0)
		{
			base.AddPrecondition(ChorePreconditions.IsAssignedtoMe, this.smi.sm.workable.Get<WorkableType>(this.smi));
			base.AddPrecondition(ChorePreconditions.IsRegionValid, required_region);
		}
	}

	public override string ToString()
	{
		return "WorkChore<" + typeof(WorkableType).ToString() + ">";
	}

	public override void Cleanup()
	{
		base.Cleanup();
		if (this.target != null)
		{
			this.target.Unsubscribe(-592767678, new EventSystem.EventHandler(this.OnOperationalChanged));
		}
	}

	private void OnOperationalChanged(object data)
	{
		if (!(bool)data)
		{
			this.Fail("No Longer Operational");
		}
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		this.smi.sm.worker.Set(context.consumer.gameObject, this.smi);
		base.Begin(context);
	}

	public class StatesInstance : GameStateMachine<WorkChore<WorkableType>.States, WorkChore<WorkableType>.StatesInstance, WorkChore<WorkableType>>.GameInstance
	{
		public StatesInstance(WorkChore<WorkableType> master, GameObject workable, KAnimFile override_anims)
			: base(master)
		{
			this.overrideAnims = override_anims;
			base.sm.workable.Set(workable, base.smi);
		}

		public void EnableAnimOverrides()
		{
			if (this.overrideAnims != null)
			{
				base.sm.worker.Get<KAnimControllerBase>(base.smi).AddAnimOverrides(this.overrideAnims, 0f);
			}
		}

		public void DisableAnimOverrides()
		{
			if (this.overrideAnims != null)
			{
				base.sm.worker.Get<KAnimControllerBase>(base.smi).RemoveAnimOverrides(this.overrideAnims);
			}
		}

		private KAnimFile overrideAnims;
	}

	public class States : GameStateMachine<WorkChore<WorkableType>.States, WorkChore<WorkableType>.StatesInstance, WorkChore<WorkableType>>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.approach;
			base.Target(this.worker);
			this.approach.InitializeStates(this.worker, this.workable, this.work, null, null, null);
			this.work.Enter(delegate(WorkChore<WorkableType>.StatesInstance smi)
			{
				smi.EnableAnimOverrides();
			}).ToggleWork<WorkableType>(this.workable, this.success, null).Exit(delegate(WorkChore<WorkableType>.StatesInstance smi)
			{
				smi.DisableAnimOverrides();
			});
			this.success.ReturnSuccess();
		}

		public GameStateMachine<WorkChore<WorkableType>.States, WorkChore<WorkableType>.StatesInstance, WorkChore<WorkableType>>.ApproachSubState<WorkableType> approach;

		public GameStateMachine<WorkChore<WorkableType>.States, WorkChore<WorkableType>.StatesInstance, WorkChore<WorkableType>>.State work;

		public GameStateMachine<WorkChore<WorkableType>.States, WorkChore<WorkableType>.StatesInstance, WorkChore<WorkableType>>.State success;

		public StateMachine<WorkChore<WorkableType>.States, WorkChore<WorkableType>.StatesInstance, WorkChore<WorkableType>>.TargetParameter workable;

		public StateMachine<WorkChore<WorkableType>.States, WorkChore<WorkableType>.StatesInstance, WorkChore<WorkableType>>.TargetParameter worker;
	}
}
