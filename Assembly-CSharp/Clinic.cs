using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Clinic : Workable, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.showProgressBar = false;
		this.assignable.subSlots = new AssignableSlot[] { Db.Get().AssignableSlots.MedicalBed };
		this.assignable.AddAutoassignPrecondition(new Func<MinionIdentity, bool>(this.CanAutoAssignTo));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable.AddRef(base.gameObject);
		Components.Clinics.Add(this);
		base.SetWorkTime(float.PositiveInfinity);
		this.clinicSMI = new Clinic.ClinicSM.Instance(this);
		this.clinicSMI.StartSM();
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
	}

	protected override void OnCleanUp()
	{
		Prioritizable.RemoveRef(base.gameObject);
		Components.Clinics.Remove(this);
		base.OnCleanUp();
	}

	private KAnimFile[] GetAppropriateOverrideAnims(Worker worker)
	{
		KAnimFile[] array = null;
		if (!worker.GetSMI<WoundMonitor.Instance>().ShouldExitInfirmary())
		{
			array = this.workerInjuredAnims;
		}
		else if (this.workerDiseasedAnims != null && this.IsValidEffect(this.diseaseEffect) && worker.GetSMI<DiseaseMonitor.Instance>().IsSick())
		{
			array = this.workerDiseasedAnims;
		}
		return array;
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		this.overrideAnims = this.GetAppropriateOverrideAnims(worker);
		return base.GetAnim(worker);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		worker.GetComponent<Effects>().Add("Sleep", false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		KAnimFile[] appropriateOverrideAnims = this.GetAppropriateOverrideAnims(worker);
		if (appropriateOverrideAnims == null || appropriateOverrideAnims != this.overrideAnims)
		{
			return true;
		}
		base.OnWorkTick(worker, dt);
		return false;
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.GetComponent<Effects>().Remove("Sleep");
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.assignable.Unassign();
		base.OnCompleteWork(worker);
		Effects component = worker.GetComponent<Effects>();
		for (int i = 0; i < Clinic.EffectsRemoved.Length; i++)
		{
			string text = Clinic.EffectsRemoved[i];
			component.Remove(text);
		}
	}

	private bool CanAutoAssignTo(MinionIdentity worker)
	{
		bool flag = false;
		if (this.IsValidEffect(this.healthEffect))
		{
			Health component = worker.GetComponent<Health>();
			if (component != null && component.hitPoints < component.maxHitPoints)
			{
				flag = true;
			}
		}
		if (!flag && this.IsValidEffect(this.diseaseEffect))
		{
			MinionModifiers component2 = worker.GetComponent<MinionModifiers>();
			Diseases diseases = component2.diseases;
			flag = diseases.Count > 0;
		}
		return flag;
	}

	private bool IsValidEffect(string effect)
	{
		return effect != null && effect != string.Empty;
	}

	private bool AllowDoctoring()
	{
		return this.IsValidEffect(this.doctoredDiseaseEffect) || this.IsValidEffect(this.doctoredHealthEffect);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.IsValidEffect(this.healthEffect))
		{
			Effect.AddModifierDescriptions(base.gameObject, list, this.healthEffect, false);
		}
		if (this.diseaseEffect != this.healthEffect && this.IsValidEffect(this.diseaseEffect))
		{
			Effect.AddModifierDescriptions(base.gameObject, list, this.diseaseEffect, false);
		}
		if (this.AllowDoctoring())
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.DOCTORING, UI.BUILDINGEFFECTS.TOOLTIPS.DOCTORING, Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
			if (this.IsValidEffect(this.doctoredHealthEffect))
			{
				Effect.AddModifierDescriptions(base.gameObject, list, this.doctoredHealthEffect, true);
			}
			if (this.doctoredDiseaseEffect != this.doctoredHealthEffect && this.IsValidEffect(this.doctoredDiseaseEffect))
			{
				Effect.AddModifierDescriptions(base.gameObject, list, this.doctoredDiseaseEffect, true);
			}
		}
		return list;
	}

	[MyCmpReq]
	private Assignable assignable;

	private static readonly string[] EffectsRemoved = new string[] { "SoreBack" };

	private const int MAX_RANGE = 10;

	private const float CHECK_RANGE_INTERVAL = 10f;

	public float doctorVisitInterval = 300f;

	public KAnimFile[] workerInjuredAnims;

	public KAnimFile[] workerDiseasedAnims;

	public string diseaseEffect;

	public string healthEffect;

	public string doctoredDiseaseEffect;

	public string doctoredHealthEffect;

	public string doctoredPlaceholderEffect;

	private Clinic.ClinicSM.Instance clinicSMI;

	public class ClinicSM : GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = false;
			default_state = this.unoperational;
			this.unoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (Clinic.ClinicSM.Instance smi) => smi.GetComponent<Operational>().IsOperational).Enter(delegate(Clinic.ClinicSM.Instance smi)
			{
				Assignable component = smi.master.GetComponent<Assignable>();
				component.Unassign();
			});
			this.operational.DefaultState(this.operational.idle).EventTransition(GameHashes.OperationalChanged, this.unoperational, (Clinic.ClinicSM.Instance smi) => !smi.master.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.AssigneeChanged, this.unoperational, null)
				.ToggleRecurringChore((Clinic.ClinicSM.Instance smi) => new WorkChore<Clinic>(Db.Get().ChoreTypes.Heal, smi.master, null, null, true, null, null, null, true, null, false, true, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false), (Clinic.ClinicSM.Instance smi) => !string.IsNullOrEmpty(smi.master.healthEffect))
				.ToggleRecurringChore((Clinic.ClinicSM.Instance smi) => new WorkChore<Clinic>(Db.Get().ChoreTypes.HealCritical, smi.master, null, null, true, null, null, null, true, null, false, true, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false), (Clinic.ClinicSM.Instance smi) => !string.IsNullOrEmpty(smi.master.healthEffect))
				.ToggleRecurringChore((Clinic.ClinicSM.Instance smi) => new WorkChore<Clinic>(Db.Get().ChoreTypes.RestDueToDisease, smi.master, null, null, true, null, null, null, true, null, true, true, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false), (Clinic.ClinicSM.Instance smi) => !string.IsNullOrEmpty(smi.master.diseaseEffect))
				.ToggleRecurringChore((Clinic.ClinicSM.Instance smi) => new WorkChore<Clinic>(Db.Get().ChoreTypes.SleepDueToDisease, smi.master, null, null, true, null, null, null, true, null, true, true, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false), (Clinic.ClinicSM.Instance smi) => !string.IsNullOrEmpty(smi.master.diseaseEffect));
			this.operational.idle.WorkableStartTransition((Clinic.ClinicSM.Instance smi) => smi.master, this.operational.healing);
			this.operational.healing.DefaultState(this.operational.healing.undoctored).WorkableStopTransition((Clinic.ClinicSM.Instance smi) => smi.GetComponent<Clinic>(), this.operational.idle).Enter(delegate(Clinic.ClinicSM.Instance smi)
			{
				smi.master.GetComponent<Operational>().SetActive(true, false);
			})
				.Exit(delegate(Clinic.ClinicSM.Instance smi)
				{
					smi.master.GetComponent<Operational>().SetActive(false, false);
				});
			this.operational.healing.undoctored.Enter(delegate(Clinic.ClinicSM.Instance smi)
			{
				smi.StartEffect(smi.master.healthEffect, false);
				smi.StartEffect(smi.master.diseaseEffect, false);
				bool flag = false;
				Worker worker = smi.master.worker;
				if (worker != null)
				{
					flag = smi.HasEffect(smi.master.doctoredHealthEffect) || smi.HasEffect(smi.master.doctoredDiseaseEffect) || smi.HasEffect(smi.master.doctoredPlaceholderEffect);
				}
				if (smi.master.AllowDoctoring())
				{
					if (flag)
					{
						smi.GoTo(this.operational.healing.doctored);
					}
					else
					{
						smi.StartDoctorChore();
					}
				}
			}).Exit(delegate(Clinic.ClinicSM.Instance smi)
			{
				smi.StopEffect(smi.master.healthEffect);
				smi.StopEffect(smi.master.diseaseEffect);
				smi.StopDoctorChore();
			});
			this.operational.healing.newlyDoctored.Enter(delegate(Clinic.ClinicSM.Instance smi)
			{
				smi.StartEffect(smi.master.doctoredDiseaseEffect, true);
				smi.StartEffect(smi.master.doctoredHealthEffect, true);
				smi.GoTo(this.operational.healing.doctored);
			});
			this.operational.healing.doctored.Enter(delegate(Clinic.ClinicSM.Instance smi)
			{
				Effects component2 = smi.master.worker.GetComponent<Effects>();
				if (smi.HasEffect(smi.master.doctoredPlaceholderEffect))
				{
					EffectInstance effectInstance = component2.Get(smi.master.doctoredPlaceholderEffect);
					EffectInstance effectInstance2 = smi.StartEffect(smi.master.doctoredDiseaseEffect, true);
					if (effectInstance2 != null)
					{
						float num = effectInstance.effect.duration - effectInstance.timeRemaining;
						effectInstance2.timeRemaining = effectInstance2.effect.duration - num;
					}
					EffectInstance effectInstance3 = smi.StartEffect(smi.master.doctoredHealthEffect, true);
					if (effectInstance3 != null)
					{
						float num2 = effectInstance.effect.duration - effectInstance.timeRemaining;
						effectInstance3.timeRemaining = effectInstance3.effect.duration - num2;
					}
					component2.Remove(smi.master.doctoredPlaceholderEffect);
				}
			}).ScheduleGoTo(delegate(Clinic.ClinicSM.Instance smi)
			{
				Worker worker2 = smi.master.worker;
				Effects component3 = worker2.GetComponent<Effects>();
				float num3 = smi.master.doctorVisitInterval;
				if (smi.HasEffect(smi.master.doctoredHealthEffect))
				{
					EffectInstance effectInstance4 = component3.Get(smi.master.doctoredHealthEffect);
					num3 = Mathf.Min(num3, effectInstance4.GetTimeRemaining());
				}
				if (smi.HasEffect(smi.master.doctoredDiseaseEffect))
				{
					EffectInstance effectInstance4 = component3.Get(smi.master.doctoredDiseaseEffect);
					num3 = Mathf.Min(num3, effectInstance4.GetTimeRemaining());
				}
				return num3;
			}, this.operational.healing.undoctored).Exit(delegate(Clinic.ClinicSM.Instance smi)
			{
				Effects component4 = smi.master.worker.GetComponent<Effects>();
				if (smi.HasEffect(smi.master.doctoredDiseaseEffect) || smi.HasEffect(smi.master.doctoredHealthEffect))
				{
					EffectInstance effectInstance5 = component4.Get(smi.master.doctoredDiseaseEffect);
					if (effectInstance5 == null)
					{
						effectInstance5 = component4.Get(smi.master.doctoredHealthEffect);
					}
					EffectInstance effectInstance6 = smi.StartEffect(smi.master.doctoredPlaceholderEffect, true);
					effectInstance6.timeRemaining = effectInstance6.effect.duration - (effectInstance5.effect.duration - effectInstance5.timeRemaining);
					component4.Remove(smi.master.doctoredDiseaseEffect);
					component4.Remove(smi.master.doctoredHealthEffect);
				}
			});
		}

		public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State unoperational;

		public Clinic.ClinicSM.OperationalStates operational;

		public class OperationalStates : GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State
		{
			public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State idle;

			public Clinic.ClinicSM.HealingStates healing;
		}

		public class HealingStates : GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State
		{
			public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State undoctored;

			public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State doctored;

			public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State newlyDoctored;
		}

		public new class Instance : GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.GameInstance
		{
			public Instance(Clinic master)
				: base(master)
			{
			}

			public void StartDoctorChore()
			{
				if (base.master.IsValidEffect(base.master.doctoredHealthEffect) || base.master.IsValidEffect(base.master.doctoredDiseaseEffect))
				{
					this.doctorChore = new WorkChore<DoctorChoreWorkable>(Db.Get().ChoreTypes.Doctor, base.smi.master, null, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, true, true);
					WorkChore<DoctorChoreWorkable> workChore = this.doctorChore;
					workChore.onComplete = (Action<Chore>)Delegate.Combine(workChore.onComplete, new Action<Chore>(delegate(Chore chore)
					{
						base.smi.GoTo(base.smi.sm.operational.healing.newlyDoctored);
					}));
				}
			}

			public void StopDoctorChore()
			{
				if (this.doctorChore != null)
				{
					this.doctorChore.Cancel("StopDoctorChore");
					this.doctorChore = null;
				}
			}

			public bool HasEffect(string effect)
			{
				bool flag = false;
				if (base.master.IsValidEffect(effect))
				{
					Worker worker = base.smi.master.worker;
					Effects component = worker.GetComponent<Effects>();
					flag = component.HasEffect(effect);
				}
				return flag;
			}

			public EffectInstance StartEffect(string effect, bool should_save)
			{
				if (base.master.IsValidEffect(effect))
				{
					Worker worker = base.smi.master.worker;
					if (worker != null)
					{
						Effects component = worker.GetComponent<Effects>();
						if (!component.HasEffect(effect))
						{
							return component.Add(effect, should_save);
						}
					}
				}
				return null;
			}

			public void StopEffect(string effect)
			{
				if (base.master.IsValidEffect(effect))
				{
					Worker worker = base.smi.master.worker;
					if (worker != null)
					{
						Effects component = worker.GetComponent<Effects>();
						if (component.HasEffect(effect))
						{
							component.Remove(effect);
						}
					}
				}
			}

			private WorkChore<DoctorChoreWorkable> doctorChore;
		}
	}
}
