using System;
using Klei;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class OilWellCap : Workable, ISliderControl, IElementEmitter
{
	public SimHashes Element
	{
		get
		{
			return this.gasElement;
		}
	}

	public float AverageEmitRate
	{
		get
		{
			return Game.Instance.accumulators.GetAverageRate(this.accumulator);
		}
	}

	public string SliderTitleKey
	{
		get
		{
			return "STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TITLE";
		}
	}

	public string SliderUnits
	{
		get
		{
			return UI.UNITSUFFIXES.PERCENT;
		}
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 100f;
	}

	public float GetSliderValue(int index)
	{
		return this.depressurizePercent * 100f;
	}

	public void SetSliderValue(float value, int index)
	{
		this.depressurizePercent = value / 100f;
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TOOLTIP";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.smi = new OilWellCap.StatesInstance(this);
		this.smi.StartSM();
		this.accumulator = Game.Instance.accumulators.Add("pressuregas", this);
		this.showProgressBar = false;
		base.SetWorkTime(float.PositiveInfinity);
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_oil_cap_kanim") };
		this.workingStatusItem = Db.Get().BuildingStatusItems.ReleasingPressure;
		this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		this.pressureMeter = new MeterController(component, "meter_target", "meter", Meter.Offset.Infront, new Vector3(0f, 0f, 0f), null);
		this.UpdatePressurePercent();
		Prioritizable.AddRef(base.gameObject);
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(this.accumulator);
		Prioritizable.RemoveRef(base.gameObject);
		base.OnCleanUp();
	}

	public void AddGasPressure(float dt)
	{
		this.storage.AddGasChunk(this.gasElement, this.addGasRate * dt, this.gasTemperature, 0, 0, true, true);
		this.UpdatePressurePercent();
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(MachineTechnician.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	public void ReleaseGasPressure(float dt)
	{
		PrimaryElement primaryElement = this.storage.FindPrimaryElement(this.gasElement);
		if (primaryElement != null && primaryElement.Mass > 0f)
		{
			float num = this.releaseGasRate * dt;
			if (base.worker != null)
			{
				num *= this.GetEfficiencyMultiplier(base.worker);
			}
			num = Mathf.Min(num, primaryElement.Mass);
			SimUtil.DiseaseInfo percentOfDisease = SimUtil.GetPercentOfDisease(primaryElement, num / primaryElement.Mass);
			primaryElement.Mass -= num;
			Game.Instance.accumulators.Accumulate(this.accumulator, num);
			SimMessages.AddRemoveSubstance(Grid.PosToCell(this), ElementLoader.GetElementIndex(this.gasElement), null, num, primaryElement.Temperature, percentOfDisease.idx, percentOfDisease.count, -1);
		}
		this.UpdatePressurePercent();
	}

	private void UpdatePressurePercent()
	{
		float massAvailable = this.storage.GetMassAvailable(this.gasElement);
		float num = massAvailable / this.maxGasPressure;
		num = Mathf.Clamp01(num);
		this.smi.sm.pressurePercent.Set(num, this.smi);
		this.pressureMeter.SetPositionPercent(num);
	}

	public bool NeedsDepressurizing()
	{
		return this.smi.GetPressurePercent() >= this.depressurizePercent;
	}

	private WorkChore<OilWellCap> CreateWorkChore()
	{
		WorkChore<OilWellCap> workChore = new WorkChore<OilWellCap>(Db.Get().ChoreTypes.Depressurize, this, null, null, true, null, null, null, true, null, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		workChore.AddPrecondition(OilWellCap.AllowedToDepressurize, this);
		return workChore;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.smi.sm.working.Set(true, this.smi);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		this.smi.sm.working.Set(false, this.smi);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return this.smi.GetPressurePercent() <= 0f;
	}

	private OilWellCap.StatesInstance smi;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private Storage storage;

	public SimHashes gasElement;

	public float gasTemperature;

	public float addGasRate = 1f;

	public float maxGasPressure = 10f;

	public float releaseGasRate = 10f;

	private float depressurizePercent = 0.75f;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	private MeterController pressureMeter;

	private static Chore.Precondition AllowedToDepressurize = new Chore.Precondition
	{
		id = "AllowedToDepressurize",
		description = DUPLICANTS.CHORES.PRECONDITIONS.ALLOWED_TO_DEPRESSURIZE,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			OilWellCap oilWellCap = (OilWellCap)data;
			return oilWellCap.NeedsDepressurizing();
		}
	};

	public class StatesInstance : GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.GameInstance
	{
		public StatesInstance(OilWellCap master)
			: base(master)
		{
		}

		public float GetPressurePercent()
		{
			return base.sm.pressurePercent.Get(base.smi);
		}
	}

	public class States : GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.root.ToggleRecurringChore((OilWellCap.StatesInstance smi) => smi.master.CreateWorkChore(), null);
			this.idle.PlayAnim("off").ToggleStatusItem(Db.Get().BuildingStatusItems.WellPressurizing, null).ParamTransition<float>(this.pressurePercent, this.overpressure, (OilWellCap.StatesInstance smi, float p) => p >= 1f)
				.ParamTransition<bool>(this.working, this.releasing_pressure, (OilWellCap.StatesInstance smi, bool p) => p)
				.EventTransition(GameHashes.OperationalChanged, this.active, (OilWellCap.StatesInstance smi) => smi.master.operational.IsOperational);
			this.active.DefaultState(this.active.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.WellPressurizing, null).EventTransition(GameHashes.OperationalChanged, this.idle, (OilWellCap.StatesInstance smi) => !smi.master.operational.IsOperational)
				.Enter(delegate(OilWellCap.StatesInstance smi)
				{
					smi.master.operational.SetActive(true, false);
				})
				.Exit(delegate(OilWellCap.StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				})
				.Update(delegate(OilWellCap.StatesInstance smi, float dt)
				{
					smi.master.AddGasPressure(dt);
				}, UpdateRate.SIM_200ms, false);
			this.active.pre.PlayAnim("working_pre").ParamTransition<float>(this.pressurePercent, this.overpressure, (OilWellCap.StatesInstance smi, float p) => p >= 1f).ParamTransition<bool>(this.working, this.releasing_pressure, (OilWellCap.StatesInstance smi, bool p) => p)
				.OnAnimQueueComplete(this.active.loop);
			this.active.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).ParamTransition<float>(this.pressurePercent, this.active.pst, (OilWellCap.StatesInstance smi, float p) => p >= 1f).ParamTransition<bool>(this.working, this.active.pst, (OilWellCap.StatesInstance smi, bool p) => p)
				.EventTransition(GameHashes.OperationalChanged, this.active.pst, (OilWellCap.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			this.active.pst.PlayAnim("working_pst").OnAnimQueueComplete(this.idle);
			this.overpressure.PlayAnim("over_pressured_pre", KAnim.PlayMode.Once).QueueAnim("over_pressured_loop", true, null).ToggleStatusItem(Db.Get().BuildingStatusItems.WellOverpressure, null)
				.ParamTransition<float>(this.pressurePercent, this.idle, (OilWellCap.StatesInstance smi, float p) => p <= 0f)
				.ParamTransition<bool>(this.working, this.releasing_pressure, (OilWellCap.StatesInstance smi, bool p) => p);
			this.releasing_pressure.DefaultState(this.releasing_pressure.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingElement, (OilWellCap.StatesInstance smi) => smi.master).ParamTransition<bool>(this.working, this.idle, (OilWellCap.StatesInstance smi, bool p) => !p)
				.Update(delegate(OilWellCap.StatesInstance smi, float dt)
				{
					smi.master.ReleaseGasPressure(dt);
				}, UpdateRate.SIM_200ms, false);
			this.releasing_pressure.pre.PlayAnim("steam_out_pre").OnAnimQueueComplete(this.releasing_pressure.loop);
			this.releasing_pressure.loop.PlayAnim("steam_out_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, this.releasing_pressure.pst, (OilWellCap.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			this.releasing_pressure.pst.PlayAnim("steam_out_pst").OnAnimQueueComplete(this.active);
		}

		public StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.FloatParameter pressurePercent;

		public StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.BoolParameter working;

		public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State idle;

		public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.PLPState active;

		public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State overpressure;

		public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.PLPState releasing_pressure;
	}
}
