using System;
using TUNING;
using UnityEngine;

[RequireComponent(typeof(RestRestoreHealth))]
public class Clinic : BuildingWorkable, IAssignable
{
	public Clinic()
	{
		this.showProgressBar = false;
	}

	public Assignable Assignable
	{
		get
		{
			return this.assignable;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.smi = new Clinic.ClinicSM.Instance(this);
		this.smi.StartSM();
		Components.Clinics.Add(this);
		base.SetWorkTime(float.PositiveInfinity);
	}

	protected override void OnCleanUp()
	{
		Components.Clinics.Remove(this);
		base.OnCleanUp();
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		FactionAlignment component = worker.GetComponent<FactionAlignment>();
		if (component != null)
		{
			component.ToggleAlignmentActive(false);
		}
		this.smi.restoreHealth.StartHealing(worker);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (worker.GetSMI<WoundMonitor.Instance>().ShouldExitClinic() && !worker.GetSMI<DiseaseMonitor.Instance>().IsSick())
		{
			return true;
		}
		this.smi.restoreHealth.OnHealTick(worker);
		base.OnWorkTick(worker, dt);
		return false;
	}

	protected override void OnStopWork(Worker worker)
	{
		FactionAlignment component = worker.GetComponent<FactionAlignment>();
		if (component != null)
		{
			component.ToggleAlignmentActive(true);
		}
		this.smi.restoreHealth.StopHealing(worker);
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.assignable.Unassign();
		base.OnCompleteWork(worker);
	}

	private void OnRegionChanged(Region new_region)
	{
		GameUtil.UpdateRegion(new_region, this, Db.Get().OwnableSlots.Clinic, REGIONS.MedicalRegionTag);
	}

	virtual GameObject IAssignable.get_gameObject()
	{
		return base.gameObject;
	}

	[MyCmpGet]
	public Assignable assignable;

	private Clinic.ClinicSM.Instance smi;

	public class ClinicSM : GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = false;
			default_state = this.invalidRegion;
			this.root.EventTransition(GameHashes.RegionChanged, this.invalidRegion, (Clinic.ClinicSM.Instance smi) => !smi.IsInMedicalRegion());
			this.invalidRegion.EventTransition(GameHashes.RegionChanged, this.unoperational, (Clinic.ClinicSM.Instance smi) => smi.IsInMedicalRegion());
			this.unoperational.EventTransition(GameHashes.OperationalChanged, this.operational, (Clinic.ClinicSM.Instance smi) => smi.GetComponent<Operational>().IsOperational);
			this.operational.DefaultState(this.operational.idle).EventTransition(GameHashes.AssigneeChanged, this.operational.exiting, null).ToggleChore(delegate(Clinic.ClinicSM.Instance smi)
			{
				Tag medicalRegionTag = REGIONS.MedicalRegionTag;
				return new WorkChore<Clinic>(Db.Get().ChoreTypes.Heal, smi.master, null, true, null, null, null, true, null, true, medicalRegionTag, null, false, true);
			}, this.unoperational, false)
				.ToggleChore(delegate(Clinic.ClinicSM.Instance smi)
				{
					Tag medicalRegionTag2 = REGIONS.MedicalRegionTag;
					return new WorkChore<Clinic>(Db.Get().ChoreTypes.HealCritical, smi.master, null, true, null, null, null, true, null, true, medicalRegionTag2, null, false, true);
				}, this.unoperational, false)
				.ToggleChore(delegate(Clinic.ClinicSM.Instance smi)
				{
					Tag medicalRegionTag3 = REGIONS.MedicalRegionTag;
					return new WorkChore<Clinic>(Db.Get().ChoreTypes.RestDueToDisease, smi.master, null, true, null, null, null, true, null, true, medicalRegionTag3, null, false, true);
				}, this.unoperational, false);
			this.operational.idle.EventTransition(GameHashes.WorkStarted, this.operational.healing, null);
			this.operational.healing.EventTransition(GameHashes.AssigneeChanged, this.operational.exiting, null).EventTransition(GameHashes.WorkStopped, this.operational.exiting, null).EventTransition(GameHashes.OperationalChanged, this.operational.exiting, (Clinic.ClinicSM.Instance smi) => !smi.GetComponent<Operational>().IsOperational)
				.Enter(delegate(Clinic.ClinicSM.Instance smi)
				{
					if (!smi.master.GetComponent<Operational>().IsOperational)
					{
						smi.GoTo(this.operational.exiting);
					}
					else
					{
						smi.master.gameObject.GetComponent<Operational>().SetActive(true, false);
						smi.Queue("working_pre", KAnim.PlayMode.Once);
						smi.Queue("working_loop", KAnim.PlayMode.Loop);
						smi.master.GetComponent<Operational>().SetActive(true, false);
					}
				});
			this.operational.exiting.PlayAnim("working_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.unoperational).Enter(delegate(Clinic.ClinicSM.Instance smi)
			{
				smi.master.gameObject.GetComponent<Operational>().SetActive(false, false);
			});
		}

		public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, IStateMachineTarget>.State unoperational;

		public Clinic.ClinicSM.OperationalStates operational;

		public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, IStateMachineTarget>.State invalidRegion;

		public class OperationalStates : GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, IStateMachineTarget>.State
		{
			public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, IStateMachineTarget>.State idle;

			public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, IStateMachineTarget>.State healing;

			public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, IStateMachineTarget>.State exiting;
		}

		public new class Instance : GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, IStateMachineTarget>.GameInstance
		{
			public Instance(IStateMachineTarget master)
				: base(master)
			{
				this.restoreHealth = base.gameObject.GetComponent<RestRestoreHealth>();
			}

			public bool IsInMedicalRegion()
			{
				RequiresRegion component = base.GetComponent<RequiresRegion>();
				if (component == null)
				{
					return true;
				}
				Region ownerRegion = component.OwnerRegion;
				return ownerRegion != null && ownerRegion.RegionTag == REGIONS.MedicalRegionTag;
			}

			public RestRestoreHealth restoreHealth;
		}
	}
}
