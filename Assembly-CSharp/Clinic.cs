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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (Clinic.toiletsOutsideRangeStatusItem == null)
		{
			Clinic.toiletsOutsideRangeStatusItem = new StatusItem("TOILETS_OUTSIDE_RANGE", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			Clinic.toiletsOutsideRangeStatusItem.resolveStringCallback = (string str, object data) => string.Format(str, 10);
		}
		if (Clinic.foodContainersOutsideRangeStatusItem == null)
		{
			Clinic.foodContainersOutsideRangeStatusItem = new StatusItem("FOOD_CONTAINERS_OUTSIDE_RANGE", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			Clinic.foodContainersOutsideRangeStatusItem.resolveStringCallback = (string str, object data) => string.Format(str, 10);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable.AddRef(base.gameObject);
		Components.Clinics.Add(this);
		this.srcCell = Grid.PosToCell(this.transform.position);
		this.Subscribe(-1503271301, new Action<object>(this.OnSelectObject));
		base.SetWorkTime(float.PositiveInfinity);
		this.clinicSMI = new Clinic.ClinicSM.Instance(this);
		this.clinicSMI.StartSM();
		this.toiletInRangeSMI = new InRangeSM.Instance(this, 10f, new Func<bool>(this.AnyToiletsInRange), Clinic.toiletsOutsideRangeStatusItem);
		this.toiletInRangeSMI.StartSM();
		this.foodInRangeSMI = new InRangeSM.Instance(this, 10f, new Func<bool>(this.AnyFoodContainersInRange), Clinic.foodContainersOutsideRangeStatusItem);
		this.foodInRangeSMI.StartSM();
	}

	protected override void OnCleanUp()
	{
		this.DestroyVisualizer();
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
		worker.GetComponent<MinionBrain>().SetMaxNavCost(10);
		worker.GetComponent<Navigator>().SetAbilityFlag(PathFinderFlags.TransitionsCostOne);
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
		worker.GetComponent<MinionBrain>().SetMaxNavCost(int.MaxValue);
		worker.GetComponent<Navigator>().ClearAbilityFlag(PathFinderFlags.TransitionsCostOne);
		worker.GetComponent<Effects>().Remove("Sleep");
		base.GetComponent<KAnimControllerBase>().Play(Clinic.SICK_PST_ANIMS, KAnim.PlayMode.Once);
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.Unassign();
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

	private bool CellHasToilet(int cell)
	{
		bool flag = false;
		int num = Grid.CellBelow(cell);
		if (Grid.IsValidCell(num) && Grid.Solid[num])
		{
			GameObject gameObject = Grid.Objects[cell, 1];
			if (gameObject != null && (gameObject.GetComponent<Toilet>() != null || gameObject.GetComponent<FlushToilet>() != null))
			{
				int num2 = Grid.PosToCell(gameObject.transform.position);
				if (MinionGroupProber.Get().GetNavCostBetweenCells(this.srcCell, num2, 10) != PathProber.InvalidCost)
				{
					flag = true;
				}
			}
		}
		return flag;
	}

	private bool CellHasFoodContainer(int cell)
	{
		bool flag = false;
		int num = Grid.CellBelow(cell);
		if (Grid.IsValidCell(num) && Grid.Solid[num])
		{
			GameObject gameObject = Grid.Objects[cell, 1];
			if (gameObject != null && (gameObject.GetComponent<RationBox>() != null || gameObject.GetComponent<Refrigerator>() != null))
			{
				int num2 = Grid.PosToCell(gameObject.transform.position);
				if (MinionGroupProber.Get().GetNavCostBetweenCells(this.srcCell, num2, 10) != PathProber.InvalidCost)
				{
					flag = true;
				}
			}
		}
		return flag;
	}

	private bool AnyToiletsInRange()
	{
		return GameUtil.FloodFillCheck(new Func<int, bool>(this.CellHasToilet), this.srcCell, 10, true, false);
	}

	private bool AnyFoodContainersInRange()
	{
		return GameUtil.FloodFillCheck(new Func<int, bool>(this.CellHasFoodContainer), this.srcCell, 10, true, false);
	}

	private void OnSelectObject(object data)
	{
		bool flag = (bool)data;
		if (flag)
		{
			this.CreateVisualizer();
		}
		else
		{
			this.DestroyVisualizer();
		}
	}

	private void CreateVisualizer()
	{
		if (this.visData != null)
		{
			return;
		}
		this.visData = new List<Clinic.VisData>();
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RadialGrid_form", false));
		GameUtil.FloodFillCheck(new Func<int, bool>(this.CreateEffect), this.srcCell, 10, true, false);
	}

	private bool CreateEffect(int cell)
	{
		KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("radialgrid_kanim", Grid.CellToPosCCC(cell, Grid.SceneLayer.Background), SceneOrganizer.Instance.GetFolder(Folder.FX).transform, false, Grid.SceneLayer.Background, true);
		kbatchedAnimController.destroyOnAnimComplete = false;
		kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
		kbatchedAnimController.PlaySpeedMultiplier = Clinic.RADIAL_PLAY_SPEED;
		int manhattanDistance = Clinic.GetManhattanDistance(cell, this.srcCell);
		float num = Clinic.RADIAL_DELAY * (float)manhattanDistance;
		this.visData.Add(new Clinic.VisData
		{
			controller = kbatchedAnimController,
			delay = num
		});
		UIScheduler.Instance.Schedule("radialgrid_pre", num, new Action<object>(Clinic.StartEffect), kbatchedAnimController, null);
		return false;
	}

	private static void StartEffect(object data)
	{
		KBatchedAnimController kbatchedAnimController = (KBatchedAnimController)data;
		kbatchedAnimController.TintColour = new Color32(100, 149, 237, byte.MaxValue);
		kbatchedAnimController.gameObject.SetActive(true);
		kbatchedAnimController.Play(Clinic.PreAnims, KAnim.PlayMode.Loop);
	}

	private void DestroyVisualizer()
	{
		if (this.visData == null)
		{
			return;
		}
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RadialGrid_disappear", false));
		foreach (Clinic.VisData visData in this.visData)
		{
			UIScheduler.Instance.Schedule("radialgrid_pst", visData.delay, new Action<object>(Clinic.DestroyEffect), visData.controller, null);
		}
		this.visData = null;
	}

	private static void DestroyEffect(object data)
	{
		KBatchedAnimController kbatchedAnimController = (KBatchedAnimController)data;
		kbatchedAnimController.destroyOnAnimComplete = true;
		kbatchedAnimController.Play(Clinic.PostAnim, KAnim.PlayMode.Once, 1f, 0f);
	}

	private static int GetManhattanDistance(int cell, int center_cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		Vector2I vector2I2 = Grid.CellToXY(center_cell);
		Vector2I vector2I3 = vector2I - vector2I2;
		return Math.Abs(vector2I3.x) + Math.Abs(vector2I3.y);
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

	private const int MAX_RANGE = 10;

	private const float CHECK_RANGE_INTERVAL = 10f;

	private static readonly string[] EffectsRemoved = new string[] { "SoreBack" };

	public float doctorVisitInterval = 300f;

	public KAnimFile[] workerInjuredAnims;

	public KAnimFile[] workerDiseasedAnims;

	public string diseaseEffect;

	public string healthEffect;

	public string doctoredDiseaseEffect;

	public string doctoredHealthEffect;

	private string doctoredPlaceholderEffect = "DoctoredOffCotEffect";

	private int srcCell;

	private Clinic.ClinicSM.Instance clinicSMI;

	private InRangeSM.Instance toiletInRangeSMI;

	private InRangeSM.Instance foodInRangeSMI;

	private static StatusItem toiletsOutsideRangeStatusItem;

	private static StatusItem foodContainersOutsideRangeStatusItem;

	private static readonly HashedString[] SICK_ANIMS = new HashedString[] { "working_pre", "working_loop" };

	private static readonly HashedString[] SICK_PST_ANIMS = new HashedString[] { "working_pst" };

	private static float RADIAL_DELAY = 0.025f;

	private static float RADIAL_PLAY_SPEED = 5f;

	private List<Clinic.VisData> visData;

	private static readonly HashedString[] PreAnims = new HashedString[] { "grid_pre", "grid_loop" };

	private static readonly HashedString PostAnim = "grid_pst";

	public class ClinicSM : GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = false;
			default_state = this.invalidRegion;
			this.root.EventTransition(GameHashes.RegionChanged, this.invalidRegion, (Clinic.ClinicSM.Instance smi) => !smi.master.IsInMedicalRegion());
			this.invalidRegion.EventTransition(GameHashes.RegionChanged, this.unoperational, (Clinic.ClinicSM.Instance smi) => smi.master.IsInMedicalRegion());
			this.unoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (Clinic.ClinicSM.Instance smi) => smi.GetComponent<Operational>().IsOperational);
			this.operational.DefaultState(this.operational.idle).EventTransition(GameHashes.OperationalChanged, this.unoperational, (Clinic.ClinicSM.Instance smi) => !smi.master.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.AssigneeChanged, this.unoperational, null)
				.ToggleRecurringChore(delegate(Clinic.ClinicSM.Instance smi)
				{
					Tag medicalRegionTag = global::TUNING.REGIONS.MedicalRegionTag;
					return new WorkChore<Clinic>(Db.Get().ChoreTypes.Heal, smi.master, null, true, null, null, null, true, null, true, medicalRegionTag, null, false, true, true);
				})
				.ToggleRecurringChore(delegate(Clinic.ClinicSM.Instance smi)
				{
					Tag medicalRegionTag2 = global::TUNING.REGIONS.MedicalRegionTag;
					return new WorkChore<Clinic>(Db.Get().ChoreTypes.HealCritical, smi.master, null, true, null, null, null, true, null, true, medicalRegionTag2, null, false, true, true);
				})
				.ToggleRecurringChore(delegate(Clinic.ClinicSM.Instance smi)
				{
					Tag medicalRegionTag3 = global::TUNING.REGIONS.MedicalRegionTag;
					return new WorkChore<Clinic>(Db.Get().ChoreTypes.RestDueToDisease, smi.master, null, true, null, null, null, true, null, true, medicalRegionTag3, null, false, true, true);
				})
				.ToggleRecurringChore(delegate(Clinic.ClinicSM.Instance smi)
				{
					Tag medicalRegionTag4 = global::TUNING.REGIONS.MedicalRegionTag;
					return new WorkChore<Clinic>(Db.Get().ChoreTypes.SleepDueToDisease, smi.master, null, true, null, null, null, true, null, true, medicalRegionTag4, null, false, true, false);
				});
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
					this.doctorChore = new WorkChore<DoctorChore>(Db.Get().ChoreTypes.Doctor, base.smi.master, null, true, null, null, null, true, null, true, default(Tag), null, false, true, true);
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

	private struct VisData
	{
		public KBatchedAnimController controller;

		public float delay;
	}
}
