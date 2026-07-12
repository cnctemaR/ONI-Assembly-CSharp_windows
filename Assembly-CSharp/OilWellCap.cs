using System;
using Klei;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/OilWellCap")]
public class OilWellCap : Workable, ISingleSliderControl, ISliderControl, IElementEmitter
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

	public int SliderDecimalPlaces(int index)
	{
		return 0;
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

	string ISliderControl.GetSliderTooltip()
	{
		return string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TOOLTIP"), this.depressurizePercent * 100f);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<OilWellCap>(-905833192, OilWellCap.OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		OilWellCap component = ((GameObject)data).GetComponent<OilWellCap>();
		if (component != null)
		{
			this.depressurizePercent = component.depressurizePercent;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable.AddRef(base.gameObject);
		this.accumulator = Game.Instance.accumulators.Add("pressuregas", this);
		this.showProgressBar = false;
		base.SetWorkTime(float.PositiveInfinity);
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_oil_cap_kanim") };
		this.workingStatusItem = Db.Get().BuildingStatusItems.ReleasingPressure;
		this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		this.pressureMeter = new MeterController(component, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new Vector3(0f, 0f, 0f), null);
		this.smi = new OilWellCap.StatesInstance(this);
		this.smi.StartSM();
		this.UpdatePressurePercent();
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
			SimMessages.AddRemoveSubstance(Grid.PosToCell(this), ElementLoader.GetElementIndex(this.gasElement), null, num, primaryElement.Temperature, percentOfDisease.idx, percentOfDisease.count, true, -1);
		}
		this.UpdatePressurePercent();
	}

	private void UpdatePressurePercent()
	{
		float num = this.storage.GetMassAvailable(this.gasElement) / this.maxGasPressure;
		num = Mathf.Clamp01(num);
		this.smi.sm.pressurePercent.Set(num, this.smi, false);
		this.pressureMeter.SetPositionPercent(num);
	}

	public bool NeedsDepressurizing()
	{
		return this.smi.GetPressurePercent() >= this.depressurizePercent;
	}

	private WorkChore<OilWellCap> CreateWorkChore()
	{
		WorkChore<OilWellCap> workChore = new WorkChore<OilWellCap>(Db.Get().ChoreTypes.Depressurize, this, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		workChore.AddPrecondition(OilWellCap.AllowedToDepressurize, this);
		return workChore;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.smi.sm.working.Set(true, this.smi, false);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		this.smi.sm.working.Set(false, this.smi, false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return this.smi.GetPressurePercent() <= 0f;
	}

	public override bool InstantlyFinish(Worker worker)
	{
		this.ReleaseGasPressure(60f);
		return true;
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

	[Serialize]
	private float depressurizePercent = 0.75f;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	private MeterController pressureMeter;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<OilWellCap> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<OilWellCap>(delegate(OilWellCap component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly Chore.Precondition AllowedToDepressurize = new Chore.Precondition
	{
		id = "AllowedToDepressurize",
		description = DUPLICANTS.CHORES.PRECONDITIONS.ALLOWED_TO_DEPRESSURIZE,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return ((OilWellCap)data).NeedsDepressurizing();
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
			default_state = this.inoperational;
			this.inoperational.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.operational, new StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Transition.ConditionCallback(this.IsOperational));
			this.operational.ToggleRecurringChore((OilWellCap.StatesInstance smi) => smi.master.CreateWorkChore(), null).DefaultState(this.operational.idle);
			this.operational.idle.PlayAnim("off").ToggleStatusItem(Db.Get().BuildingStatusItems.WellPressurizing, null).ParamTransition<float>(this.pressurePercent, this.operational.overpressure, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsGTEOne)
				.ParamTransition<bool>(this.working, this.operational.releasing_pressure, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsTrue)
				.EventTransition(GameHashes.OperationalChanged, this.inoperational, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Not(new StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Transition.ConditionCallback(this.IsOperational)))
				.EventTransition(GameHashes.OnStorageChange, this.operational.active, new StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Transition.ConditionCallback(this.IsAbleToPump));
			this.operational.active.DefaultState(this.operational.active.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.WellPressurizing, null).Enter(delegate(OilWellCap.StatesInstance smi)
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
			this.operational.active.pre.PlayAnim("working_pre").ParamTransition<float>(this.pressurePercent, this.operational.overpressure, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsGTEOne).ParamTransition<bool>(this.working, this.operational.releasing_pressure, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsTrue)
				.OnAnimQueueComplete(this.operational.active.loop);
			this.operational.active.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).ParamTransition<float>(this.pressurePercent, this.operational.active.pst, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsGTEOne).ParamTransition<bool>(this.working, this.operational.active.pst, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsTrue)
				.EventTransition(GameHashes.OperationalChanged, this.operational.active.pst, new StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Transition.ConditionCallback(this.MustStopPumping))
				.EventTransition(GameHashes.OnStorageChange, this.operational.active.pst, new StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Transition.ConditionCallback(this.MustStopPumping));
			this.operational.active.pst.PlayAnim("working_pst").OnAnimQueueComplete(this.operational.idle);
			this.operational.overpressure.PlayAnim("over_pressured_pre", KAnim.PlayMode.Once).QueueAnim("over_pressured_loop", true, null).ToggleStatusItem(Db.Get().BuildingStatusItems.WellOverpressure, null)
				.ParamTransition<float>(this.pressurePercent, this.operational.idle, (OilWellCap.StatesInstance smi, float p) => p <= 0f)
				.ParamTransition<bool>(this.working, this.operational.releasing_pressure, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsTrue);
			this.operational.releasing_pressure.DefaultState(this.operational.releasing_pressure.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingElement, (OilWellCap.StatesInstance smi) => smi.master);
			this.operational.releasing_pressure.pre.PlayAnim("steam_out_pre").OnAnimQueueComplete(this.operational.releasing_pressure.loop);
			this.operational.releasing_pressure.loop.PlayAnim("steam_out_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, this.operational.releasing_pressure.pst, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Not(new StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Transition.ConditionCallback(this.IsOperational))).ParamTransition<bool>(this.working, this.operational.releasing_pressure.pst, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsFalse)
				.Update(delegate(OilWellCap.StatesInstance smi, float dt)
				{
					smi.master.ReleaseGasPressure(dt);
				}, UpdateRate.SIM_200ms, false);
			this.operational.releasing_pressure.pst.PlayAnim("steam_out_pst").OnAnimQueueComplete(this.operational.idle);
		}

		private bool IsOperational(OilWellCap.StatesInstance smi)
		{
			return smi.master.operational.IsOperational;
		}

		private bool IsAbleToPump(OilWellCap.StatesInstance smi)
		{
			return smi.master.operational.IsOperational && smi.GetComponent<ElementConverter>().HasEnoughMassToStartConverting(false);
		}

		private bool MustStopPumping(OilWellCap.StatesInstance smi)
		{
			return !smi.master.operational.IsOperational || !smi.GetComponent<ElementConverter>().CanConvertAtAll();
		}

		public StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.FloatParameter pressurePercent;

		public StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.BoolParameter working;

		public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State inoperational;

		public OilWellCap.States.OperationalStates operational;

		public class OperationalStates : GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State
		{
			public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State idle;

			public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.PreLoopPostState active;

			public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State overpressure;

			public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.PreLoopPostState releasing_pressure;
		}
	}
}
