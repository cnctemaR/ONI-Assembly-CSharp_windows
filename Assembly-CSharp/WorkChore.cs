using System;
using UnityEngine;

public class WorkChore<WorkableType> : Chore<WorkChore<WorkableType>.StatesInstance> where WorkableType : Workable
{
	public WorkChore(ChoreType chore_type, IStateMachineTarget target, ChoreProvider chore_provider = null, Tag[] chore_tags = null, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null, bool allow_in_red_alert = true, ScheduleBlockType schedule_block = null, bool ignore_schedule_block = false, bool only_when_operational = true, KAnimFile override_anims = null, bool is_preemptable = false, bool allow_in_context_menu = true, bool allow_prioritization = true, PriorityScreen.PriorityClass priority_class = PriorityScreen.PriorityClass.basic, int priority_class_value = 0, bool ignore_building_assignment = false)
		: base(chore_type, target, chore_provider, run_until_complete, on_complete, on_begin, on_end, priority_class, priority_class_value, is_preemptable, allow_in_context_menu, 0, chore_tags)
	{
		this.smi = new WorkChore<WorkableType>.StatesInstance(this, target.gameObject, override_anims);
		this.onlyWhenOperational = only_when_operational;
		if (allow_prioritization)
		{
			base.SetPrioritizable(target.GetComponent<Prioritizable>());
		}
		base.AddPrecondition(ChorePreconditions.instance.IsNotTransferArm, null);
		if (!allow_in_red_alert)
		{
			base.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		}
		if (schedule_block != null)
		{
			base.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, schedule_block);
		}
		else if (!ignore_schedule_block)
		{
			base.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Work);
		}
		base.AddPrecondition(ChorePreconditions.instance.CanMoveTo, this.smi.sm.workable.Get<WorkableType>(this.smi));
		Operational component = target.GetComponent<Operational>();
		if (only_when_operational && component != null)
		{
			base.AddPrecondition(ChorePreconditions.instance.IsOperational, component);
		}
		if (only_when_operational)
		{
			Deconstructable component2 = target.GetComponent<Deconstructable>();
			if (component2 != null)
			{
				base.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component2);
			}
			BuildingEnabledButton component3 = target.GetComponent<BuildingEnabledButton>();
			if (component3 != null)
			{
				base.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, component3);
			}
		}
		if (!ignore_building_assignment && this.smi.sm.workable.Get(this.smi).GetComponent<Assignable>() != null)
		{
			base.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, this.smi.sm.workable.Get<Assignable>(this.smi));
		}
		WorkableType workableType = target as WorkableType;
		if (workableType != null && workableType.requiredRolePerk.IsValid)
		{
			base.AddPrecondition(ChorePreconditions.instance.HasRolePerk, workableType.requiredRolePerk);
		}
	}

	public bool onlyWhenOperational { get; private set; }

	public override string ToString()
	{
		return "WorkChore<" + typeof(WorkableType).ToString() + ">";
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		this.smi.sm.worker.Set(context.consumerState.gameObject, this.smi);
		base.Begin(context);
	}

	public bool IsOperationalValid()
	{
		if (this.onlyWhenOperational)
		{
			Operational component = this.smi.master.GetComponent<Operational>();
			if (component != null && !component.IsOperational)
			{
				return false;
			}
		}
		return true;
	}

	public override bool CanPreempt(Chore.Precondition.Context context)
	{
		if (!base.CanPreempt(context))
		{
			return false;
		}
		if (context.chore.driver == null)
		{
			return false;
		}
		if (context.chore.driver == context.consumerState.choreDriver)
		{
			return false;
		}
		Workable workable = this.smi.sm.workable.Get<WorkableType>(this.smi);
		if (workable == null)
		{
			return false;
		}
		int navigationCost = context.chore.driver.GetComponent<Navigator>().GetNavigationCost(workable);
		int num = 4;
		if (navigationCost == -1 || navigationCost < num)
		{
			return false;
		}
		int navigationCost2 = context.consumerState.navigator.GetNavigationCost(workable);
		return navigationCost2 * 2 <= navigationCost;
	}

	public class StatesInstance : GameStateMachine<WorkChore<WorkableType>.States, WorkChore<WorkableType>.StatesInstance, WorkChore<WorkableType>, object>.GameInstance
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
			this.approach.InitializeStates(this.worker, this.workable, this.work, null, null, null).Update("CheckOperational", delegate(WorkChore<WorkableType>.StatesInstance smi, float dt)
			{
				if (!smi.master.IsOperationalValid())
				{
					smi.StopSM("Building not operational");
				}
			}, UpdateRate.SIM_200ms, false);
			this.work.Enter(delegate(WorkChore<WorkableType>.StatesInstance smi)
			{
				smi.EnableAnimOverrides();
			}).ToggleWork<WorkableType>(this.workable, this.success, null, (WorkChore<WorkableType>.StatesInstance smi) => smi.master.IsOperationalValid()).Exit(delegate(WorkChore<WorkableType>.StatesInstance smi)
			{
				smi.DisableAnimOverrides();
			});
			this.success.ReturnSuccess();
		}

		public GameStateMachine<WorkChore<WorkableType>.States, WorkChore<WorkableType>.StatesInstance, WorkChore<WorkableType>, object>.ApproachSubState<WorkableType> approach;

		public GameStateMachine<WorkChore<WorkableType>.States, WorkChore<WorkableType>.StatesInstance, WorkChore<WorkableType>, object>.State work;

		public GameStateMachine<WorkChore<WorkableType>.States, WorkChore<WorkableType>.StatesInstance, WorkChore<WorkableType>, object>.State success;

		public StateMachine<WorkChore<WorkableType>.States, WorkChore<WorkableType>.StatesInstance, WorkChore<WorkableType>, object>.TargetParameter workable;

		public StateMachine<WorkChore<WorkableType>.States, WorkChore<WorkableType>.StatesInstance, WorkChore<WorkableType>, object>.TargetParameter worker;
	}
}
