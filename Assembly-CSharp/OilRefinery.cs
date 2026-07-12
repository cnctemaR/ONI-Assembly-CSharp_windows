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
		float num = Mathf.Clamp01(this.storage.GetMassAvailable(SimHashes.CrudeOil) / this.maxSrcMass);
		this.meter.SetPositionPercent(num);
	}

	private static bool UpdateStateCb(int cell, object data)
	{
		OilRefinery oilRefinery = data as OilRefinery;
		if (Grid.Element[cell].IsGas)
		{
			oilRefinery.cellCount += 1f;
			oilRefinery.envPressure += Grid.Mass[cell];
		}
		return true;
	}

	private void TestAreaPressure()
	{
		this.envPressure = 0f;
		this.cellCount = 0f;
		if (this.occupyArea != null && base.gameObject != null)
		{
			this.occupyArea.TestArea(Grid.PosToCell(base.gameObject), this, new Func<int, object, bool>(OilRefinery.UpdateStateCb));
			this.envPressure /= this.cellCount;
		}
	}

	private bool IsOverPressure()
	{
		return this.envPressure >= this.overpressureMass;
	}

	private bool IsOverWarningPressure()
	{
		return this.envPressure >= this.overpressureWarningMass;
	}

	private bool wasOverPressure;

	[SerializeField]
	public float overpressureWarningMass = 4.5f;

	[SerializeField]
	public float overpressureMass = 5f;

	private float maxSrcMass;

	private float envPressure;

	private float cellCount;

	[MyCmpGet]
	private Storage storage;

	[MyCmpReq]
	private Operational operational;

	[MyCmpAdd]
	private OilRefinery.WorkableTarget workable;

	[MyCmpReq]
	private OccupyArea occupyArea;

	[MyCmpAdd]
	private ManuallySetRemoteWorkTargetComponent remoteChore;

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

		public void TestAreaPressure()
		{
			base.smi.master.TestAreaPressure();
			bool flag = base.smi.master.IsOverPressure();
			bool flag2 = base.smi.master.IsOverWarningPressure();
			if (flag)
			{
				base.smi.master.wasOverPressure = true;
				base.sm.isOverPressure.Set(true, this, false);
				return;
			}
			if (base.smi.master.wasOverPressure && !flag2)
			{
				base.sm.isOverPressure.Set(false, this, false);
			}
		}
	}

	public class States : GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.disabled;
			this.root.EventTransition(GameHashes.OperationalChanged, this.disabled, (OilRefinery.StatesInstance smi) => !smi.master.operational.IsOperational);
			this.disabled.EventTransition(GameHashes.OperationalChanged, this.needResources, (OilRefinery.StatesInstance smi) => smi.master.operational.IsOperational);
			this.needResources.EventTransition(GameHashes.OnStorageChange, this.ready, (OilRefinery.StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting(false));
			this.ready.Update("Test Pressure Update", delegate(OilRefinery.StatesInstance smi, float dt)
			{
				smi.TestAreaPressure();
			}, UpdateRate.SIM_1000ms, false).ParamTransition<bool>(this.isOverPressure, this.overpressure, GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.IsTrue).Transition(this.needResources, (OilRefinery.StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting(false), UpdateRate.SIM_200ms)
				.ToggleChore((OilRefinery.StatesInstance smi) => new WorkChore<OilRefinery.WorkableTarget>(Db.Get().ChoreTypes.Fabricate, smi.master.workable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true), new Action<OilRefinery.StatesInstance, Chore>(OilRefinery.States.SetRemoteChore), this.needResources);
			this.overpressure.Update("Test Pressure Update", delegate(OilRefinery.StatesInstance smi, float dt)
			{
				smi.TestAreaPressure();
			}, UpdateRate.SIM_1000ms, false).ParamTransition<bool>(this.isOverPressure, this.ready, GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.IsFalse).ToggleStatusItem(Db.Get().BuildingStatusItems.PressureOk, null);
		}

		private static void SetRemoteChore(OilRefinery.StatesInstance smi, Chore chore)
		{
			smi.master.remoteChore.SetChore(chore);
		}

		public StateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.BoolParameter isOverPressure;

		public StateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.BoolParameter isOverPressureWarning;

		public GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.State disabled;

		public GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.State overpressure;

		public GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.State needResources;

		public GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.State ready;
	}

	[AddComponentMenu("KMonoBehaviour/Workable/WorkableTarget")]
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

		protected override void OnStartWork(WorkerBase worker)
		{
			this.operational.SetActive(true, false);
		}

		protected override void OnStopWork(WorkerBase worker)
		{
			this.operational.SetActive(false, false);
		}

		protected override void OnCompleteWork(WorkerBase worker)
		{
			this.operational.SetActive(false, false);
		}

		public override bool InstantlyFinish(WorkerBase worker)
		{
			return false;
		}

		[MyCmpGet]
		public Operational operational;
	}
}
