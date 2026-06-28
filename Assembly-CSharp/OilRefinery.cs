using System;
using KSerialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class OilRefinery : StateMachineComponent<OilRefinery.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		this.meter = new MeterController(component, "meter_target", "meter", Meter.Offset.Infront, Vector3.zero, null);
		base.smi.StartSM();
		this.maxSrcMass = base.GetComponent<ConduitConsumer>().capacityKG;
	}

	private void OnStorageChanged(object data)
	{
		float massAvailable = this.storage.GetMassAvailable(SimHashes.CrudeOil);
		float num = Mathf.Clamp01(massAvailable / this.maxSrcMass);
		this.meter.SetPositionPercent(num);
	}

	private bool IsOverPressure()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		num = Grid.CellAbove(num);
		return GameUtil.FloodFillCheck(new Func<int, bool>(this.IsCellOverPressure), num, 2, true, true);
	}

	private bool IsCellOverPressure(int cell)
	{
		return Grid.Mass[cell] > this.overpressureMass;
	}

	[SerializeField]
	public float overpressureMass = 2.5f;

	private float maxSrcMass;

	[MyCmpGet]
	private Storage storage;

	[MyCmpReq]
	private Operational operational;

	[MyCmpAdd]
	private OilRefinery.WorkableTarget workable;

	private const bool hasMeter = true;

	private MeterController meter;

	public class StatesInstance : GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.GameInstance
	{
		public StatesInstance(OilRefinery smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.disabled;
			this.root.EventTransition(GameHashes.OperationalChanged, this.disabled, (OilRefinery.StatesInstance smi) => !smi.master.operational.IsOperational);
			this.disabled.EventTransition(GameHashes.OperationalChanged, this.needResources, (OilRefinery.StatesInstance smi) => smi.master.operational.IsOperational);
			this.needResources.EventTransition(GameHashes.OnStorageChange, this.ready, (OilRefinery.StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());
			this.ready.Transition(this.needResources, (OilRefinery.StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting(), UpdateRate.SIM_200ms).Transition(this.overpressure, (OilRefinery.StatesInstance smi) => smi.master.IsOverPressure(), UpdateRate.SIM_200ms).ToggleChore((OilRefinery.StatesInstance smi) => new WorkChore<OilRefinery.WorkableTarget>(Db.Get().ChoreTypes.Fabricate, smi.master.workable, null, null, true, null, null, null, true, null, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false), this.needResources);
			this.overpressure.ToggleStatusItem(Db.Get().BuildingStatusItems.PressureOk, null).Transition(this.ready, (OilRefinery.StatesInstance smi) => !smi.master.IsOverPressure(), UpdateRate.SIM_200ms);
		}

		public GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.State disabled;

		public GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.State overpressure;

		public GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.State needResources;

		public GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.State ready;
	}

	public class WorkableTarget : Workable
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.showProgressBar = false;
			this.workerStatusItem = null;
			this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_oilrefinery_kanim") };
		}

		protected override void OnSpawn()
		{
			base.OnSpawn();
			base.SetWorkTime(float.PositiveInfinity);
		}

		public override void AwardExperience(float work_dt, MinionResume resume)
		{
			resume.AddExperienceIfRole(MachineTechnician.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		}

		protected override void OnStartWork(Worker worker)
		{
			this.operational.SetActive(true, false);
		}

		protected override void OnStopWork(Worker worker)
		{
			this.operational.SetActive(false, false);
		}

		protected override void OnCompleteWork(Worker worker)
		{
			this.operational.SetActive(false, false);
		}

		[MyCmpGet]
		public Operational operational;
	}
}
