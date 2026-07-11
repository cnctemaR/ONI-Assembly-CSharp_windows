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
		this.sick.DefaultState(this.sick.minor).EventTransition(GameHashes.DiseaseCured, this.post_nocheer, (DiseaseMonitor.Instance smi) => !smi.IsSick()).ToggleThought(Db.Get().Thoughts.GotInfected, null);
		this.sick.minor.EventTransition(GameHashes.DiseaseAdded, this.sick.major, (DiseaseMonitor.Instance smi) => smi.HasMajorDisease());
		this.sick.major.EventTransition(GameHashes.DiseaseCured, this.sick.minor, (DiseaseMonitor.Instance smi) => !smi.HasMajorDisease()).ToggleUrge(Db.Get().Urges.RestDueToDisease).Update("AutoAssignClinic", delegate(DiseaseMonitor.Instance smi, float dt)
		{
			smi.AutoAssignClinic();
		}, UpdateRate.SIM_4000ms, false)
			.Exit(delegate(DiseaseMonitor.Instance smi)
			{
				smi.UnassignClinic();
			});
		this.post_nocheer.Enter(delegate(DiseaseMonitor.Instance smi)
		{
			if (smi.IsSleepingOrSleepSchedule())
			{
				smi.GoTo(this.healthy);
			}
			else
			{
				smi.GoTo(this.post);
			}
		});
		this.post.ToggleChore((DiseaseMonitor.Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.EmoteHighPriority, DiseaseMonitor.SickPostKAnim, DiseaseMonitor.SickPostAnims, KAnim.PlayMode.Once, false), this.healthy);
	}

	public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.State healthy;

	public DiseaseMonitor.SickStates sick;

	public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.State post;

	public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.State post_nocheer;

	private static readonly HashedString SickPostKAnim = "anim_cheer_kanim";

	private static readonly HashedString[] SickPostAnims = new HashedString[] { "cheer_pre", "cheer_loop", "cheer_pst" };

	public class SickStates : GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.State minor;

		public GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance, IStateMachineTarget, object>.State major;
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

		public bool HasMajorDisease()
		{
			foreach (DiseaseInstance diseaseInstance in this.activeDiseases)
			{
				if (diseaseInstance.modifier.severity >= Disease.Severity.Major)
				{
					return true;
				}
			}
			return false;
		}

		public void AutoAssignClinic()
		{
			Ownables soleOwner = base.sm.masterTarget.Get(base.smi).GetComponent<MinionIdentity>().GetSoleOwner();
			AssignableSlot clinic = Db.Get().AssignableSlots.Clinic;
			AssignableSlotInstance slot = soleOwner.GetSlot(clinic);
			if (slot == null)
			{
				return;
			}
			if (slot.assignable != null)
			{
				return;
			}
			soleOwner.AutoAssignSlot(clinic);
		}

		public void UnassignClinic()
		{
			Ownables soleOwner = base.sm.masterTarget.Get(base.smi).GetComponent<MinionIdentity>().GetSoleOwner();
			AssignableSlot clinic = Db.Get().AssignableSlots.Clinic;
			AssignableSlotInstance slot = soleOwner.GetSlot(clinic);
			if (slot != null)
			{
				slot.Unassign(true);
			}
		}

		public bool IsSleepingOrSleepSchedule()
		{
			Schedulable component = base.GetComponent<Schedulable>();
			if (component != null && component.IsAllowed(Db.Get().ScheduleBlockTypes.Sleep))
			{
				return true;
			}
			KPrefabID component2 = base.GetComponent<KPrefabID>();
			return component2 != null && component2.HasTag(GameTags.Asleep);
		}

		private Diseases activeDiseases;
	}
}
