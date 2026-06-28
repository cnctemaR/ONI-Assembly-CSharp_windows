using System;
using UnityEngine;

public class SleepChoreMonitor : GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		base.serializable = false;
		this.root.EventHandler(GameHashes.AssignablesChanged, delegate(SleepChoreMonitor.Instance smi)
		{
			smi.UpdateBed();
		});
		this.satisfied.EventTransition(GameHashes.AddUrge, this.nobedassigned, (SleepChoreMonitor.Instance smi) => smi.HasSleepUrge());
		this.nobedassigned.Enter("SetBed", delegate(SleepChoreMonitor.Instance smi)
		{
			if (smi.HasSleepUrge())
			{
				smi.AutoAssignBed();
			}
		}).ToggleChore(new Func<SleepChoreMonitor.Instance, Chore>(this.CreateSleepOnFloorChore), this.satisfied, false).ParamTransition<GameObject>(this.bed, this.bedassigned, (SleepChoreMonitor.Instance smi, GameObject p) => p != null);
		this.bedassigned.DefaultState(this.bedassigned.bedunreachable).ParamTransition<GameObject>(this.bed, this.nobedassigned, (SleepChoreMonitor.Instance smi, GameObject p) => p == null).EventTransition(GameHashes.AssignablesChanged, this.nobedassigned, null);
		this.bedassigned.bedunreachable.ToggleChore(new Func<SleepChoreMonitor.Instance, Chore>(this.CreateSleepOnFloorChore), this.bedassigned.bedunreachable, false).EventTransition(GameHashes.AssignableReachabilityChanged, this.bedassigned.bedreachable, (SleepChoreMonitor.Instance smi) => smi.IsBedReachable()).ToggleStatusItem(Db.Get().DuplicantStatusItems.BedUnreachable, null);
		this.bedassigned.bedreachable.ToggleChore(new Func<SleepChoreMonitor.Instance, Chore>(this.CreateSleepChore), this.satisfied, false).EventTransition(GameHashes.AssignableReachabilityChanged, this.bedassigned.bedunreachable, (SleepChoreMonitor.Instance smi) => !smi.IsBedReachable());
	}

	private Chore CreateSleepOnFloorChore(SleepChoreMonitor.Instance smi)
	{
		return new SleepOnFloorChore(smi.master);
	}

	private Chore CreateSleepChore(SleepChoreMonitor.Instance smi)
	{
		return new SleepChore(smi.master, this.bed.Get(smi));
	}

	public GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget>.State nobedassigned;

	public GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget>.State satisfied;

	public SleepChoreMonitor.BedAssignedState bedassigned;

	public StateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget>.TargetParameter bed;

	public class BedAssignedState : GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget>.State
	{
		public GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget>.State bedunreachable;

		public GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget>.State bedreachable;
	}

	public new class Instance : GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void UpdateBed()
		{
			Ownables component = base.sm.masterTarget.Get(base.smi).GetComponent<Ownables>();
			Assignable assignable = component.GetAssignable(Db.Get().OwnableSlots.Bed);
			base.smi.sm.bed.Set(assignable, base.smi);
		}

		public void AutoAssignBed()
		{
			Ownables component = base.sm.masterTarget.Get(base.smi).GetComponent<Ownables>();
			Navigator component2 = component.GetComponent<Navigator>();
			Assignable assignable = component.AutoAssignSlot(component2, Db.Get().OwnableSlots.Bed);
			base.smi.sm.bed.Set(assignable, base.smi);
		}

		public bool HasSleepUrge()
		{
			return base.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.Sleep);
		}

		public bool IsBedReachable()
		{
			return base.GetComponent<Sensors>().GetSensor<AssignableReachabilitySensor>().IsReachable(Db.Get().OwnableSlots.Bed);
		}
	}
}
