using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class Clinic : Ownable, IEffectDescriptor
{
	public Clinic()
	{
		this.showProgressBar = false;
		base.slot = Db.Get().OwnableSlots.Clinic;
		this.subSlots = new AssignableSlot[] { Db.Get().OwnableSlots.MedicalBed };
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

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		worker.GetComponent<Effects>().Add("Sleep", false);
		this.overrideAnims = this.GetAppropriateOverrideAnims(worker);
		base.GetComponent<KAnimControllerBase>().Play(Clinic.SICK_ANIMS, KAnim.PlayMode.Loop);
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
		base.GetComponent<KAnimControllerBase>().Play(Clinic.SICK_PST_ANIMS, KAnim.PlayMode.Once);
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.Unassign();
		base.OnCompleteWork(worker);
		Effects component = worker.GetComponent<Effects>();
		for (int i = 0; i < Clinic.EffectsRemoved.Length; i++)
		{
			string text = Clinic.EffectsRemoved[i];
			component.Remove(text);
		}
	}

	private void OnRegionChanged(Region new_region)
	{
		GameUtil.UpdateRegion(new_region, this, Db.Get().OwnableSlots.Clinic, global::TUNING.REGIONS.MedicalRegionTag);
	}

	private bool IsInMedicalRegion()
	{
		RequiresRegion component = base.GetComponent<RequiresRegion>();
		if (component == null)
		{
			return true;
		}
		Region ownerRegion = component.OwnerRegion;
		return ownerRegion != null && ownerRegion.RegionTag == global::TUNING.REGIONS.MedicalRegionTag;
	}

	public override bool CanAutoAssignTo(KMonoBehaviour worker)
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

	private void AddModifierDescriptions(List<Descriptor> descs, string effect_id, bool increase_indent = false)
	{
		Effect effect = Db.Get().effects.Get(effect_id);
		foreach (AttributeModifier attributeModifier in effect.SelfModifiers)
		{
			Descriptor descriptor = new Descriptor(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + attributeModifier.AttributeId.ToUpper() + ".NAME") + ": " + attributeModifier.GetFormattedString(base.gameObject), string.Empty, Descriptor.DescriptorType.Effect, false);
			if (increase_indent)
			{
				descriptor.IncreaseIndent();
			}
			descs.Add(descriptor);
		}
	}

	public new List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> descriptors = base.GetDescriptors(def);
		if (this.IsValidEffect(this.healthEffect))
		{
			this.AddModifierDescriptions(descriptors, this.healthEffect, false);
		}
		if (this.diseaseEffect != this.healthEffect && this.IsValidEffect(this.diseaseEffect))
		{
			this.AddModifierDescriptions(descriptors, this.diseaseEffect, false);
		}
		if (this.AllowDoctoring())
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.DOCTORING, UI.BUILDINGEFFECTS.TOOLTIPS.DOCTORING, Descriptor.DescriptorType.Effect);
			descriptors.Add(descriptor);
			if (this.IsValidEffect(this.doctoredHealthEffect))
			{
				this.AddModifierDescriptions(descriptors, this.doctoredHealthEffect, true);
			}
			if (this.doctoredDiseaseEffect != this.doctoredHealthEffect && this.IsValidEffect(this.doctoredDiseaseEffect))
			{
				this.AddModifierDescriptions(descriptors, this.doctoredDiseaseEffect, true);
			}
		}
		return descriptors;
	}

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

	private static readonly HashedString[] SICK_ANIMS = new HashedString[] { "working_pre", "working_loop" };

	private static readonly HashedString[] SICK_PST_ANIMS = new HashedString[] { "working_pst" };

	public class ClinicSM : GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = false;
			default_state = this.invalidRegion;
			this.root.EventTransition(GameHashes.RegionChanged, this.invalidRegion, (Clinic.ClinicSM.Instance smi) => !smi.master.IsInMedicalRegion()).EventHandler(GameHashes.UpdateRoom, delegate(Clinic.ClinicSM.Instance smi)
			{
				smi.OnUpdateRoom(null);
			}).Enter(delegate(Clinic.ClinicSM.Instance smi)
			{
				smi.OnUpdateRoom(null);
			});
			this.invalidRegion.EventTransition(GameHashes.RegionChanged, this.unoperational, (Clinic.ClinicSM.Instance smi) => smi.master.IsInMedicalRegion());
			this.unoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (Clinic.ClinicSM.Instance smi) => smi.GetComponent<Operational>().IsOperational);
			this.operational.DefaultState(this.operational.idle).EventTransition(GameHashes.OperationalChanged, this.unoperational, (Clinic.ClinicSM.Instance smi) => !smi.master.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.AssigneeChanged, this.unoperational, null)
				.ToggleRecurringChore(delegate(Clinic.ClinicSM.Instance smi)
				{
					ChoreType heal = Db.Get().ChoreTypes.Heal;
					Clinic master = smi.master;
					Tag medicalRegionTag = global::TUNING.REGIONS.MedicalRegionTag;
					return new WorkChore<Clinic>(heal, master, null, true, null, null, null, true, null, true, medicalRegionTag, null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
				}, (Clinic.ClinicSM.Instance smi) => !string.IsNullOrEmpty(smi.master.healthEffect))
				.ToggleRecurringChore(delegate(Clinic.ClinicSM.Instance smi)
				{
					ChoreType healCritical = Db.Get().ChoreTypes.HealCritical;
					Clinic master2 = smi.master;
					Tag medicalRegionTag2 = global::TUNING.REGIONS.MedicalRegionTag;
					return new WorkChore<Clinic>(healCritical, master2, null, true, null, null, null, true, null, true, medicalRegionTag2, null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
				}, (Clinic.ClinicSM.Instance smi) => !string.IsNullOrEmpty(smi.master.healthEffect))
				.ToggleRecurringChore(delegate(Clinic.ClinicSM.Instance smi)
				{
					ChoreType restDueToDisease = Db.Get().ChoreTypes.RestDueToDisease;
					Clinic master3 = smi.master;
					Tag medicalRegionTag3 = global::TUNING.REGIONS.MedicalRegionTag;
					return new WorkChore<Clinic>(restDueToDisease, master3, null, true, null, null, null, true, null, true, medicalRegionTag3, null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
				}, (Clinic.ClinicSM.Instance smi) => !string.IsNullOrEmpty(smi.master.diseaseEffect))
				.ToggleRecurringChore(delegate(Clinic.ClinicSM.Instance smi)
				{
					ChoreType sleepDueToDisease = Db.Get().ChoreTypes.SleepDueToDisease;
					Clinic master4 = smi.master;
					Tag medicalRegionTag4 = global::TUNING.REGIONS.MedicalRegionTag;
					return new WorkChore<Clinic>(sleepDueToDisease, master4, null, true, null, null, null, true, null, true, medicalRegionTag4, null, false, true, false, PriorityScreen.PriorityClass.basic, int.MaxValue);
				}, (Clinic.ClinicSM.Instance smi) => !string.IsNullOrEmpty(smi.master.diseaseEffect));
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
				Effects component = smi.master.worker.GetComponent<Effects>();
				if (smi.HasEffect(smi.master.doctoredPlaceholderEffect))
				{
					EffectInstance effectInstance = component.Get(smi.master.doctoredPlaceholderEffect);
					EffectInstance effectInstance2 = smi.StartEffect(smi.master.doctoredDiseaseEffect, true);
					if (effectInstance2 != null)
					{
						effectInstance2.startTime = effectInstance.startTime;
					}
					EffectInstance effectInstance3 = smi.StartEffect(smi.master.doctoredHealthEffect, true);
					if (effectInstance3 != null)
					{
						effectInstance3.startTime = effectInstance.startTime;
					}
					component.Remove(smi.master.doctoredPlaceholderEffect);
				}
			}).ScheduleGoTo(delegate(Clinic.ClinicSM.Instance smi)
			{
				Worker worker2 = smi.master.worker;
				Effects component2 = worker2.GetComponent<Effects>();
				float num = smi.master.doctorVisitInterval;
				if (smi.HasEffect(smi.master.doctoredHealthEffect))
				{
					EffectInstance effectInstance4 = component2.Get(smi.master.doctoredHealthEffect);
					num = Mathf.Min(num, effectInstance4.GetTimeRemaining());
				}
				if (smi.HasEffect(smi.master.doctoredDiseaseEffect))
				{
					EffectInstance effectInstance4 = component2.Get(smi.master.doctoredDiseaseEffect);
					num = Mathf.Min(num, effectInstance4.GetTimeRemaining());
				}
				return num;
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
					effectInstance6.startTime = effectInstance5.startTime;
					component3.Remove(smi.master.doctoredDiseaseEffect);
					component3.Remove(smi.master.doctoredHealthEffect);
				}
			});
		}

		public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State unoperational;

		public Clinic.ClinicSM.OperationalStates operational;

		public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State invalidRegion;

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
					this.doctorChore = new WorkChore<DoctorChore>(Db.Get().ChoreTypes.Doctor, base.smi.master, null, true, null, null, null, true, null, true, default(Tag), null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
					WorkChore<DoctorChore> workChore = this.doctorChore;
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

			public void OnUpdateRoom(object data = null)
			{
				Room roomOfBuilding = Game.Instance.roomProber.GetRoomOfBuilding(base.GetComponent<BuildingComplete>());
				if (roomOfBuilding != null && RoomTypes.GetRoomType(roomOfBuilding).category == RoomTypes.TypeCategories["Hospital"])
				{
					base.smi.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ClinicOutsideHospital, true);
				}
				else
				{
					base.smi.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ClinicOutsideHospital, null);
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

			private WorkChore<DoctorChore> doctorChore;
		}
	}
}
