using System;
using KSerialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class OilRefinery : StateMachineComponent<OilRefinery.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.Subscribe<OilRefinery>(-1697596308, OilRefinery.OnStorageChangedDelegate);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		this.meter = new MeterController(component, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Vector3.zero, null);
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
		return GameUtil.FloodFillCheck<OilRefinery>(new Func<int, OilRefinery, bool>(OilRefinery.IsCellOverPressure), this, num, 2, true, true);
	}

	private static bool IsCellOverPressure(int cell, OilRefinery oil_refinery)
	{
		return Grid.Mass[cell] > oil_refinery.overpressureMass;
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

	private static readonly EventSystem.IntraObjectHandler<OilRefinery> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<OilRefinery>(delegate(OilRefinery component, object data)
	{
		component.OnStorageChanged(data);
	});

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
			this.ready.Transition(this.needResources, (OilRefinery.StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting(), UpdateRate.SIM_200ms).Transition(this.overpressure, (OilRefinery.StatesInstance smi) => smi.master.IsOverPressure(), UpdateRate.SIM_200ms).ToggleChore((OilRefinery.StatesInstance smi) => new WorkChore<OilRefinery.WorkableTarget>(Db.Get().ChoreTypes.Fabricate, smi.master.workable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true), this.needResources);
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
			this.skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
			this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
			this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_oilrefinery_kanim") };
		}

		protected override void OnSpawn()
		{
			base.OnSpawn();
			base.SetWorkTime(float.PositiveInfinity);
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
