using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class HighEnergyParticleSpawner : StateMachineComponent<HighEnergyParticleSpawner.StatesInstance>, IHighEnergyParticleDirection, IProgressBarSideScreen, ISingleSliderControl, ISliderControl
{
	public float PredictedPerCycleConsumptionRate
	{
		get
		{
			return (float)Mathf.FloorToInt(this.recentPerSecondConsumptionRate * 600f);
		}
	}

	public EightDirection Direction
	{
		get
		{
			return this._direction;
		}
		set
		{
			this._direction = value;
			if (this.directionController != null)
			{
				this.directionController.SetRotation((float)(45 * EightDirectionUtil.GetDirectionIndex(this._direction)));
				this.directionController.controller.enabled = false;
				this.directionController.controller.enabled = true;
			}
		}
	}

	private void OnCopySettings(object data)
	{
		HighEnergyParticleSpawner component = ((GameObject)data).GetComponent<HighEnergyParticleSpawner>();
		if (component != null)
		{
			this.Direction = component.Direction;
			this.particleThreshold = component.particleThreshold;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<HighEnergyParticleSpawner>(-905833192, HighEnergyParticleSpawner.OnCopySettingsDelegate);
		base.Subscribe<HighEnergyParticleSpawner>(-801688580, HighEnergyParticleSpawner.OnLogicValueChangedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		KSelectable component = base.GetComponent<KSelectable>();
		if (HighEnergyParticleSpawner.infoStatusItem_Logic == null)
		{
			HighEnergyParticleSpawner.infoStatusItem_Logic = new StatusItem("HEPSpawnerLogic", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			HighEnergyParticleSpawner.infoStatusItem_Logic.resolveStringCallback = new Func<string, object, string>(HighEnergyParticleSpawner.ResolveInfoStatusItem);
			HighEnergyParticleSpawner.infoStatusItem_Logic.resolveTooltipCallback = new Func<string, object, string>(HighEnergyParticleSpawner.ResolveInfoStatusItemTooltip);
		}
		component.AddStatusItem(HighEnergyParticleSpawner.infoStatusItem_Logic, this);
		this.HEPStatusHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.CollectingHEP, this);
		this.directionController = new EightDirectionController(base.GetComponent<KBatchedAnimController>(), "redirector_target", "redirect", EightDirectionController.Offset.Infront);
		this.Direction = this.Direction;
		this.particleController = new MeterController(base.GetComponent<KBatchedAnimController>(), "orb_target", "orb_off", Meter.Offset.NoChange, Grid.SceneLayer.NoLayer, Array.Empty<string>());
		this.particleController.gameObject.AddOrGet<LoopingSounds>();
		this.progressMeterController = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
	}

	public float GetProgressBarMaxValue()
	{
		return this.particleThreshold;
	}

	public float GetProgressBarFillPercentage()
	{
		return this.particleStorage.Particles / this.particleThreshold;
	}

	public string GetProgressBarTitleLabel()
	{
		return UI.UISIDESCREENS.HIGHENERGYPARTICLESPAWNERSIDESCREEN.PROGRESS_BAR_LABEL;
	}

	public string GetProgressBarLabel()
	{
		return Mathf.FloorToInt(this.particleStorage.Particles).ToString() + "/" + Mathf.FloorToInt(this.particleThreshold).ToString();
	}

	public string GetProgressBarTooltip()
	{
		return UI.UISIDESCREENS.HIGHENERGYPARTICLESPAWNERSIDESCREEN.PROGRESS_BAR_TOOLTIP;
	}

	private void OnSimConsumeRadiationCallback(Sim.ConsumedRadiationCallback radiationCBInfo, object data)
	{
	}

	public void DoConsumeParticlesWhileDisabled(float dt)
	{
		this.particleStorage.ConsumeAndGet(dt * 1f);
	}

	public void LauncherUpdate(float dt)
	{
		this.radiationSampleTimer += dt;
		if (this.radiationSampleTimer >= this.radiationSampleRate)
		{
			this.radiationSampleTimer -= this.radiationSampleRate;
			int num = Grid.PosToCell(this);
			float num2 = Grid.Radiation[num];
			if (num2 != 0f)
			{
				base.smi.sm.isAbsorbingRadiation.Set(true, base.smi);
				this.recentPerSecondConsumptionRate = num2 / 600f;
				this.particleStorage.Store(this.recentPerSecondConsumptionRate * this.radiationSampleRate);
			}
			else
			{
				base.smi.sm.isAbsorbingRadiation.Set(false, base.smi);
			}
		}
		this.progressMeterController.SetPositionPercent(this.GetProgressBarFillPercentage());
		if (!this.particleVisualPlaying && this.particleStorage.Particles > this.particleThreshold / 2f)
		{
			this.particleController.meterController.Play("orb_pre", KAnim.PlayMode.Once, 1f, 0f);
			this.particleController.meterController.Queue("orb_idle", KAnim.PlayMode.Loop, 1f, 0f);
			this.particleVisualPlaying = true;
		}
		this.launcherTimer += dt;
		if (this.launcherTimer < this.minLaunchInterval || !this.AllowSpawnParticles)
		{
			return;
		}
		if (this.particleStorage.Particles >= this.particleThreshold)
		{
			this.launcherTimer = 0f;
			int highEnergyParticleOutputCell = base.GetComponent<Building>().GetHighEnergyParticleOutputCell();
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("HighEnergyParticle"), Grid.CellToPosCCC(highEnergyParticleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2, null, 0);
			gameObject.SetActive(true);
			if (gameObject != null)
			{
				HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
				component.payload = this.particleStorage.ConsumeAndGet(this.particleThreshold);
				component.SetDirection(this.Direction);
				this.directionController.PlayAnim("redirect_send", KAnim.PlayMode.Once);
				this.directionController.controller.Queue("redirect", KAnim.PlayMode.Once, 1f, 0f);
				this.particleController.meterController.Play("orb_send", KAnim.PlayMode.Once, 1f, 0f);
				this.particleController.meterController.Queue("orb_off", KAnim.PlayMode.Once, 1f, 0f);
				this.particleVisualPlaying = false;
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public bool AllowSpawnParticles
	{
		get
		{
			return !this.hasLogicWire || (this.hasLogicWire && this.isLogicActive);
		}
	}

	public bool HasLogicWire
	{
		get
		{
			return this.hasLogicWire;
		}
	}

	public bool IsLogicActive
	{
		get
		{
			return this.isLogicActive;
		}
	}

	private LogicCircuitNetwork GetNetwork()
	{
		int portCell = base.GetComponent<LogicPorts>().GetPortCell(HighEnergyParticleSpawner.PORT_ID);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell);
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == HighEnergyParticleSpawner.PORT_ID)
		{
			this.isLogicActive = logicValueChanged.newValue > 0;
			this.hasLogicWire = this.GetNetwork() != null;
		}
	}

	private static string ResolveInfoStatusItem(string format_str, object data)
	{
		HighEnergyParticleSpawner highEnergyParticleSpawner = (HighEnergyParticleSpawner)data;
		if (!highEnergyParticleSpawner.HasLogicWire)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLESPAWNER.NORMAL;
		}
		if (highEnergyParticleSpawner.IsLogicActive)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLESPAWNER.LOGIC_CONTROLLED_ACTIVE;
		}
		return BUILDING.STATUSITEMS.HIGHENERGYPARTICLESPAWNER.LOGIC_CONTROLLED_STANDBY;
	}

	private static string ResolveInfoStatusItemTooltip(string format_str, object data)
	{
		HighEnergyParticleSpawner highEnergyParticleSpawner = (HighEnergyParticleSpawner)data;
		if (!highEnergyParticleSpawner.HasLogicWire)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLESPAWNER.TOOLTIPS.NORMAL;
		}
		if (highEnergyParticleSpawner.IsLogicActive)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLESPAWNER.TOOLTIPS.LOGIC_CONTROLLED_ACTIVE;
		}
		return BUILDING.STATUSITEMS.HIGHENERGYPARTICLESPAWNER.TOOLTIPS.LOGIC_CONTROLLED_STANDBY;
	}

	public string SliderTitleKey
	{
		get
		{
			return "STRINGS.UI.UISIDESCREENS.HIGHENERGYPARTICLESPAWNERSIDESCREEN.TITLE";
		}
	}

	public string SliderUnits
	{
		get
		{
			return UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES;
		}
	}

	public int SliderDecimalPlaces(int index)
	{
		return 0;
	}

	public float GetSliderMin(int index)
	{
		return (float)this.minSlider;
	}

	public float GetSliderMax(int index)
	{
		return (float)this.maxSlider;
	}

	public float GetSliderValue(int index)
	{
		return this.particleThreshold;
	}

	public void SetSliderValue(float value, int index)
	{
		this.particleThreshold = value;
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.HIGHENERGYPARTICLESPAWNERSIDESCREEN.TOOLTIP";
	}

	string ISliderControl.GetSliderTooltip()
	{
		return string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.HIGHENERGYPARTICLESPAWNERSIDESCREEN.TOOLTIP"), this.particleThreshold);
	}

	public static readonly HashedString PORT_ID = "HEPSpawner";

	[MyCmpReq]
	private HighEnergyParticleStorage particleStorage;

	[MyCmpGet]
	private Operational operational;

	private Guid HEPStatusHandle;

	private float recentPerSecondConsumptionRate;

	public int minSlider;

	public int maxSlider;

	private MeterController particleContainerVisual;

	[Serialize]
	private EightDirection _direction;

	public float minLaunchInterval;

	public float radiationSampleRate;

	[Serialize]
	public float particleThreshold = 50f;

	private EightDirectionController directionController;

	private float launcherTimer;

	private float radiationSampleTimer;

	private MeterController particleController;

	private bool particleVisualPlaying;

	private MeterController progressMeterController;

	[Serialize]
	public Ref<HighEnergyParticlePort> capturedByRef = new Ref<HighEnergyParticlePort>();

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<HighEnergyParticleSpawner> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<HighEnergyParticleSpawner>(delegate(HighEnergyParticleSpawner component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<HighEnergyParticleSpawner> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<HighEnergyParticleSpawner>(delegate(HighEnergyParticleSpawner component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private bool hasLogicWire;

	private bool isLogicActive;

	private static StatusItem infoStatusItem_Logic;

	public class StatesInstance : GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.GameInstance
	{
		public StatesInstance(HighEnergyParticleSpawner smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.inoperational;
			this.inoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.ready, false).Update(delegate(HighEnergyParticleSpawner.StatesInstance smi, float dt)
			{
				smi.master.DoConsumeParticlesWhileDisabled(dt);
			}, UpdateRate.SIM_200ms, false);
			this.ready.TagTransition(GameTags.Operational, this.inoperational, true).DefaultState(this.ready.idle).Update(delegate(HighEnergyParticleSpawner.StatesInstance smi, float dt)
			{
				smi.master.LauncherUpdate(dt);
			}, UpdateRate.SIM_EVERY_TICK, false);
			this.ready.idle.ParamTransition<bool>(this.isAbsorbingRadiation, this.ready.absorbing, GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.IsTrue).PlayAnim("on");
			this.ready.absorbing.Enter("SetActive(true)", delegate(HighEnergyParticleSpawner.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Exit("SetActive(false)", delegate(HighEnergyParticleSpawner.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).ParamTransition<bool>(this.isAbsorbingRadiation, this.ready.idle, GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.IsFalse)
				.PlayAnim("working_loop", KAnim.PlayMode.Loop);
		}

		public StateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.BoolParameter isAbsorbingRadiation;

		public GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State inoperational;

		public HighEnergyParticleSpawner.States.ReadyStates ready;

		public class ReadyStates : GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State
		{
			public GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State create;

			public GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State idle;

			public GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State absorbing;
		}
	}
}
