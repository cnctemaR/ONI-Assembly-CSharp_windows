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
		}).ToggleChore(new Func<SleepChoreMonitor.Instance, Chore>(this.CreateSleepOnFloorChore), this.satisfied, this.satisfied).ParamTransition<GameObject>(this.bed, this.bedassigned, (SleepChoreMonitor.Instance smi, GameObject p) => p != null);
		this.bedassigned.DefaultState(this.bedassigned.bedunreachable).ParamTransition<GameObject>(this.bed, this.nobedassigned, (SleepChoreMonitor.Instance smi, GameObject p) => p == null).EventTransition(GameHashes.AssignablesChanged, this.nobedassigned, null);
		this.bedassigned.bedunreachable.ToggleChore(new Func<SleepChoreMonitor.Instance, Chore>(this.CreateSleepOnFloorChore), this.bedassigned.bedunreachable).EventTransition(GameHashes.AssignableReachabilityChanged, this.bedassigned.bedreachable, (SleepChoreMonitor.Instance smi) => smi.IsBedReachable()).ToggleStatusItem(Db.Get().DuplicantStatusItems.BedUnreachable, null);
		this.bedassigned.bedreachable.ToggleChore(new Func<SleepChoreMonitor.Instance, Chore>(this.CreateSleepChore), this.satisfied, this.satisfied).EventTransition(GameHashes.AssignableReachabilityChanged, this.bedassigned.bedunreachable, (SleepChoreMonitor.Instance smi) => !smi.IsBedReachable());
	}

	private Chore CreateSleepOnFloorChore(SleepChoreMonitor.Instance smi)
	{
		return new SleepChore(smi.master, null);
	}

	private Chore CreateSleepChore(SleepChoreMonitor.Instance smi)
	{
		return new SleepChore(smi.master, this.bed.Get(smi));
	}

	public GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget, object>.State nobedassigned;

	public GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget, object>.State satisfied;

	public SleepChoreMonitor.BedAssignedState bedassigned;

	public StateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget, object>.TargetParameter bed;

	public class BedAssignedState : GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget, object>.State bedunreachable;

		public GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget, object>.State bedreachable;
	}

	public new class Instance : GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void UpdateBed()
		{
			Ownables component = base.sm.masterTarget.Get(base.smi).GetComponent<Ownables>();
			Assignable assignable = component.GetAssignable(Db.Get().AssignableSlots.MedicalBed);
			Assignable assignable2;
			if (assignable != null && assignable.CanAutoAssignTo(base.gameObject.GetComponent<MinionIdentity>()))
			{
				assignable2 = assignable;
			}
			else
			{
				assignable2 = component.GetAssignable(Db.Get().AssignableSlots.Bed);
			}
			base.smi.sm.bed.Set(assignable2, base.smi);
		}

		public void AutoAssignBed()
		{
			Ownables component = base.sm.masterTarget.Get(base.smi).GetComponent<Ownables>();
			Assignable assignable = component.AutoAssignSlot(Db.Get().AssignableSlots.Bed);
			Assignable assignable2 = component.GetAssignable(Db.Get().AssignableSlots.MedicalBed);
			Assignable assignable3;
			if (assignable2 != null && assignable2.CanAutoAssignTo(base.gameObject.GetComponent<MinionIdentity>()))
			{
				assignable3 = assignable2;
			}
			else
			{
				assignable3 = assignable;
			}
			base.smi.sm.bed.Set(assignable3, base.smi);
		}

		public bool HasSleepUrge()
		{
			return base.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.Sleep);
		}

		public bool IsBedReachable()
		{
			AssignableReachabilitySensor sensor = base.GetComponent<Sensors>().GetSensor<AssignableReachabilitySensor>();
			return sensor.IsReachable(Db.Get().AssignableSlots.Bed) || sensor.IsReachable(Db.Get().AssignableSlots.MedicalBed);
		}
	}
}
