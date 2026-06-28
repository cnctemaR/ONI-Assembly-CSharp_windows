using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class DiseaseMonitor : GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = true;
		default_state = this.healthy;
		this.healthy.EventTransition(GameHashes.DiseaseAdded, this.sick, (DiseaseMonitor.Instance smi) => smi.IsSick());
		this.sick.DefaultState(this.sick.notify).EventTransition(GameHashes.DiseaseCured, this.post_nocheer, (DiseaseMonitor.Instance smi) => !smi.IsSick()).ToggleAnims("anim_idle_sick_kanim", 0f)
			.ToggleExpression(Db.Get().Expressions.Sick, null)
			.ToggleUrge(Db.Get().Urges.RestDueToDisease)
			.ToggleSchedulePeriodic("AutoAssignClinic", 10f, delegate(DiseaseMonitor.Instance smi)
			{
				smi.AutoAssignClinic();
			})
			.Exit(delegate(DiseaseMonitor.Instance smi)
			{
				smi.UnassignClinic();
			});
		this.sick.notify.DefaultState(this.sick.notify.notify);
		this.sick.notify.notify.ToggleThought(Db.Get().Thoughts.GotInfected, null).ToggleChore((DiseaseMonitor.Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.Emote, DiseaseMonitor.SickAnims, null), this.sick.notify.cooldown);
		this.sick.notify.cooldown.ScheduleGoTo(5f, this.sick.notify);
		this.post_nocheer.Enter(delegate(DiseaseMonitor.Instance smi)
		{
			if (smi.IsNightTime())
			{
				smi.GoTo(this.healthy);
			}
			else
			{
				smi.GoTo(this.post);
			}
		});
		this.post.ToggleChore((DiseaseMonitor.Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.EmoteHighPriority, DiseaseMonitor.SickPostKAnim, DiseaseMonitor.SickPostAnims, KAnim.PlayMode.Once), this.healthy);
	}

	public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.State healthy;

	public DiseaseMonitor.SickStates sick;

	public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.State post;

	public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.State post_nocheer;

	private static readonly HashedString[] SickAnims = new HashedString[] { "idle_pre", "idle_default" };

	private static readonly HashedString SickPostKAnim = "anim_cheer_kanim";

	private static readonly HashedString[] SickPostAnims = new HashedString[] { "cheer_pre", "cheer_loop", "cheer_pst" };

	public class NotifyStates : GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.State notify;

		public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.State cooldown;
	}

	public class SickStates : GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.State
	{
		public DiseaseMonitor.NotifyStates notify;
	}

	public new class Instance : GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.activeDiseases = master.GetComponent<MinionModifiers>().diseases;
		}

		private string OnGetToolTip(List<Notification> notifications, object data)
		{
			return DUPLICANTS.STATUSITEMS.HASDISEASE.TOOLTIP;
		}

		public bool IsSick()
		{
			return this.activeDiseases.Count > 0;
		}

		public void AutoAssignClinic()
		{
			Ownables component = base.sm.masterTarget.Get(base.smi).GetComponent<Ownables>();
			OwnableSlot clinic = Db.Get().OwnableSlots.Clinic;
			AssignableSlotInstance slot = component.GetSlot(clinic);
			if (slot == null)
			{
				return;
			}
			if (slot.assignable != null)
			{
				return;
			}
			Navigator component2 = component.GetComponent<Navigator>();
			component.AutoAssignSlot(component2, clinic);
		}

		public void UnassignClinic()
		{
			Ownables component = base.sm.masterTarget.Get(base.smi).GetComponent<Ownables>();
			OwnableSlot clinic = Db.Get().OwnableSlots.Clinic;
			AssignableSlotInstance slot = component.GetSlot(clinic);
			if (slot != null)
			{
				slot.Unassign();
			}
		}

		public bool IsNightTime()
		{
			return TimeOfDay.Instance.GetCurrentTimeRegion() == TimeOfDay.TimeRegion.Night;
		}

		private Diseases activeDiseases;
	}
}
