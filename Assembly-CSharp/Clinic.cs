using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Clinic")]
public class Clinic : Workable, IGameObjectEffectDescriptor, ISingleSliderControl, ISliderControl
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.showProgressBar = false;
		this.assignable.subSlots = new AssignableSlot[] { Db.Get().AssignableSlots.MedicalBed };
		this.assignable.AddAutoassignPrecondition(new Func<MinionAssignablesProxy, bool>(this.CanAutoAssignTo));
		this.assignable.AddAssignPrecondition(new Func<MinionAssignablesProxy, bool>(this.CanManuallyAssignTo));
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

	protected override void OnCleanUp()
	{
		Prioritizable.RemoveRef(base.gameObject);
		Components.Clinics.Remove(this);
		base.OnCleanUp();
	}

	private KAnimFile[] GetAppropriateOverrideAnims(WorkerBase worker)
	{
		KAnimFile[] array = null;
		if (!worker.GetSMI<WoundMonitor.Instance>().ShouldExitInfirmary())
		{
			array = this.workerInjuredAnims;
		}
		else if (this.workerDiseasedAnims != null && this.IsValidEffect(this.diseaseEffect) && worker.GetSMI<SicknessMonitor.Instance>().IsSick())
		{
			array = this.workerDiseasedAnims;
		}
		return array;
	}

	public override Workable.AnimInfo GetAnim(WorkerBase worker)
	{
		this.overrideAnims = this.GetAppropriateOverrideAnims(worker);
		return base.GetAnim(worker);
	}

	protected override void OnStartWork(WorkerBase worker)
	{
		base.OnStartWork(worker);
		worker.GetComponent<Effects>().Add("Sleep", false);
	}

	protected override bool OnWorkTick(WorkerBase worker, float dt)
	{
		KAnimFile[] appropriateOverrideAnims = this.GetAppropriateOverrideAnims(worker);
		if (appropriateOverrideAnims == null || appropriateOverrideAnims != this.overrideAnims)
		{
			return true;
		}
		base.OnWorkTick(worker, dt);
		return false;
	}

	protected override void OnStopWork(WorkerBase worker)
	{
		worker.GetComponent<Effects>().Remove("Sleep");
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(WorkerBase worker)
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

	public override bool InstantlyFinish(WorkerBase worker)
	{
		return false;
	}

	private Chore CreateWorkChore(ChoreType chore_type, bool allow_prioritization, bool allow_in_red_alert, PriorityScreen.PriorityClass priority_class, bool ignore_schedule_block = false)
	{
		return new WorkChore<Clinic>(chore_type, this, null, true, null, null, null, allow_in_red_alert, null, ignore_schedule_block, true, null, false, true, allow_prioritization, priority_class, 5, false, false);
	}

	private Chore CreateBionicWorkChore(ChoreType chore_type, bool allow_prioritization, bool allow_in_red_alert, PriorityScreen.PriorityClass priority_class, bool ignore_schedule_block = false)
	{
		WorkChore<Clinic> workChore = new WorkChore<Clinic>(chore_type, this, null, true, null, null, null, allow_in_red_alert, null, ignore_schedule_block, true, null, false, true, allow_prioritization, priority_class, 5, false, false);
		workChore.AddPrecondition(ChorePreconditions.instance.IsBionic, null);
		return workChore;
	}

	private bool CanAutoAssignTo(MinionAssignablesProxy worker)
	{
		bool flag = false;
		MinionIdentity minionIdentity = worker.target as MinionIdentity;
		if (minionIdentity != null)
		{
			if (this.IsValidEffect(this.healthEffect))
			{
				Health component = minionIdentity.GetComponent<Health>();
				if (component != null && component.hitPoints < component.maxHitPoints)
				{
					flag = true;
				}
			}
			if (!flag && this.IsValidEffect(this.diseaseEffect))
			{
				flag = minionIdentity.GetComponent<MinionModifiers>().sicknesses.Count > 0;
			}
		}
		return flag;
	}

	private bool CanManuallyAssignTo(MinionAssignablesProxy worker)
	{
		bool flag = false;
		MinionIdentity minionIdentity = worker.target as MinionIdentity;
		if (minionIdentity != null)
		{
			flag = this.IsHealthBelowThreshold(minionIdentity.gameObject);
		}
		return flag;
	}

	private bool IsHealthBelowThreshold(GameObject minion)
	{
		Health health = ((minion != null) ? minion.GetComponent<Health>() : null);
		if (health != null)
		{
			float num = health.hitPoints / health.maxHitPoints;
			if (health != null)
			{
				return num < this.MedicalAttentionMinimum;
			}
		}
		return false;
	}

	private bool IsValidEffect(string effect)
	{
		return effect != null && effect != "";
	}

	private bool AllowDoctoring()
	{
		return this.IsValidEffect(this.doctoredDiseaseEffect) || this.IsValidEffect(this.doctoredHealthEffect);
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> descriptors = base.GetDescriptors(go);
		if (this.IsValidEffect(this.healthEffect))
		{
			Effect.AddModifierDescriptions(base.gameObject, descriptors, this.healthEffect, false);
		}
		if (this.diseaseEffect != this.healthEffect && this.IsValidEffect(this.diseaseEffect))
		{
			Effect.AddModifierDescriptions(base.gameObject, descriptors, this.diseaseEffect, false);
		}
		if (this.AllowDoctoring())
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.DOCTORING, UI.BUILDINGEFFECTS.TOOLTIPS.DOCTORING, Descriptor.DescriptorType.Effect);
			descriptors.Add(descriptor);
			if (this.IsValidEffect(this.doctoredHealthEffect))
			{
				Effect.AddModifierDescriptions(base.gameObject, descriptors, this.doctoredHealthEffect, true);
			}
			if (this.doctoredDiseaseEffect != this.doctoredHealthEffect && this.IsValidEffect(this.doctoredDiseaseEffect))
			{
				Effect.AddModifierDescriptions(base.gameObject, descriptors, this.doctoredDiseaseEffect, true);
			}
		}
		return descriptors;
	}

	public float MedicalAttentionMinimum
	{
		get
		{
			return this.sicknessSliderValue / 100f;
		}
	}

	string ISliderControl.SliderTitleKey
	{
		get
		{
			return "STRINGS.UI.UISIDESCREENS.MEDICALCOTSIDESCREEN.TITLE";
		}
	}

	string ISliderControl.SliderUnits
	{
		get
		{
			return UI.UNITSUFFIXES.PERCENT;
		}
	}

	int ISliderControl.SliderDecimalPlaces(int index)
	{
		return 0;
	}

	float ISliderControl.GetSliderMin(int index)
	{
		return 0f;
	}

	float ISliderControl.GetSliderMax(int index)
	{
		return 100f;
	}

	float ISliderControl.GetSliderValue(int index)
	{
		return this.sicknessSliderValue;
	}

	void ISliderControl.SetSliderValue(float percent, int index)
	{
		if (percent != this.sicknessSliderValue)
		{
			this.sicknessSliderValue = (float)Mathf.RoundToInt(percent);
			Game.Instance.Trigger(875045922, null);
		}
	}

	string ISliderControl.GetSliderTooltip(int index)
	{
		return string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.MEDICALCOTSIDESCREEN.TOOLTIP"), this.sicknessSliderValue);
	}

	string ISliderControl.GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.MEDICALCOTSIDESCREEN.TOOLTIP";
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

	public static readonly Chore.Precondition IsOverSicknessThreshold = new Chore.Precondition
	{
		id = "IsOverSicknessThreshold",
		description = DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_BEING_ATTACKED,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return ((Clinic)data).IsHealthBelowThreshold(context.consumerState.gameObject);
		}
	};

	[Serialize]
	private float sicknessSliderValue = 70f;

	public class ClinicSM : GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = StateMachine.SerializeType.Never;
			default_state = this.unoperational;
			this.unoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (Clinic.ClinicSM.Instance smi) => smi.GetComponent<Operational>().IsOperational).Enter(delegate(Clinic.ClinicSM.Instance smi)
			{
				smi.master.GetComponent<Assignable>().Unassign();
			});
			this.operational.DefaultState(this.operational.idle).EventTransition(GameHashes.OperationalChanged, this.unoperational, (Clinic.ClinicSM.Instance smi) => !smi.master.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.AssigneeChanged, this.unoperational, null)
				.ToggleRecurringChore((Clinic.ClinicSM.Instance smi) => smi.master.CreateWorkChore(Db.Get().ChoreTypes.Heal, false, true, PriorityScreen.PriorityClass.personalNeeds, false), (Clinic.ClinicSM.Instance smi) => !string.IsNullOrEmpty(smi.master.healthEffect))
				.ToggleRecurringChore((Clinic.ClinicSM.Instance smi) => smi.master.CreateWorkChore(Db.Get().ChoreTypes.HealCritical, false, true, PriorityScreen.PriorityClass.personalNeeds, false), (Clinic.ClinicSM.Instance smi) => !string.IsNullOrEmpty(smi.master.healthEffect))
				.ToggleRecurringChore((Clinic.ClinicSM.Instance smi) => smi.master.CreateWorkChore(Db.Get().ChoreTypes.RestDueToDisease, false, true, PriorityScreen.PriorityClass.personalNeeds, true), (Clinic.ClinicSM.Instance smi) => !string.IsNullOrEmpty(smi.master.diseaseEffect))
				.ToggleRecurringChore((Clinic.ClinicSM.Instance smi) => smi.master.CreateWorkChore(Db.Get().ChoreTypes.SleepDueToDisease, false, true, PriorityScreen.PriorityClass.personalNeeds, true), (Clinic.ClinicSM.Instance smi) => !string.IsNullOrEmpty(smi.master.diseaseEffect))
				.ToggleRecurringChore((Clinic.ClinicSM.Instance smi) => smi.master.CreateBionicWorkChore(Db.Get().ChoreTypes.BionicRestDueToDisease, false, true, PriorityScreen.PriorityClass.personalNeeds, true), (Clinic.ClinicSM.Instance smi) => !string.IsNullOrEmpty(smi.master.diseaseEffect));
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
				if (smi.master.worker != null)
				{
					flag = smi.HasEffect(smi.master.doctoredHealthEffect) || smi.HasEffect(smi.master.doctoredDiseaseEffect) || smi.HasEffect(smi.master.doctoredPlaceholderEffect);
				}
				if (smi.master.AllowDoctoring())
				{
					if (flag)
					{
						smi.GoTo(this.operational.healing.doctored);
						return;
					}
					smi.StartDoctorChore();
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
				Effects component = smi.master.worker.GetComponent<Effects>();
				if (smi.HasEffect(smi.master.doctoredPlaceholderEffect))
				{
					EffectInstance effectInstance = component.Get(smi.master.doctoredPlaceholderEffect);
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
					component.Remove(smi.master.doctoredPlaceholderEffect);
				}
			}).ScheduleGoTo(delegate(Clinic.ClinicSM.Instance smi)
			{
				Effects component2 = smi.master.worker.GetComponent<Effects>();
				float num3 = smi.master.doctorVisitInterval;
				if (smi.HasEffect(smi.master.doctoredHealthEffect))
				{
					EffectInstance effectInstance4 = component2.Get(smi.master.doctoredHealthEffect);
					num3 = Mathf.Min(num3, effectInstance4.GetTimeRemaining());
				}
				if (smi.HasEffect(smi.master.doctoredDiseaseEffect))
				{
					EffectInstance effectInstance4 = component2.Get(smi.master.doctoredDiseaseEffect);
					num3 = Mathf.Min(num3, effectInstance4.GetTimeRemaining());
				}
				return num3;
			}, this.operational.healing.undoctored).Exit(delegate(Clinic.ClinicSM.Instance smi)
			{
				Effects component3 = smi.master.worker.GetComponent<Effects>();
				if (smi.HasEffect(smi.master.doctoredDiseaseEffect) || smi.HasEffect(smi.master.doctoredHealthEffect))
				{
					EffectInstance effectInstance5 = component3.Get(smi.master.doctoredDiseaseEffect);
					if (effectInstance5 == null)
					{
						effectInstance5 = component3.Get(smi.master.doctoredHealthEffect);
					}
					EffectInstance effectInstance6 = smi.StartEffect(smi.master.doctoredPlaceholderEffect, true);
					effectInstance6.timeRemaining = effectInstance6.effect.duration - (effectInstance5.effect.duration - effectInstance5.timeRemaining);
					component3.Remove(smi.master.doctoredDiseaseEffect);
					component3.Remove(smi.master.doctoredHealthEffect);
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
					this.doctorChore = new WorkChore<DoctorChoreWorkable>(Db.Get().ChoreTypes.Doctor, base.smi.master, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, true, true);
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
					flag = base.smi.master.worker.GetComponent<Effects>().HasEffect(effect);
				}
				return flag;
			}

			public EffectInstance StartEffect(string effect, bool should_save)
			{
				if (base.master.IsValidEffect(effect))
				{
					WorkerBase worker = base.smi.master.worker;
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
					WorkerBase worker = base.smi.master.worker;
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
