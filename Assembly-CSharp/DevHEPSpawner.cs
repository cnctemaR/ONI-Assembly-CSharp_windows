using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class DevHEPSpawner : StateMachineComponent<DevHEPSpawner.StatesInstance>, IHighEnergyParticleDirection, ISingleSliderControl, ISliderControl
{
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
		DevHEPSpawner component = ((GameObject)data).GetComponent<DevHEPSpawner>();
		if (component != null)
		{
			this.Direction = component.Direction;
			this.boltAmount = component.boltAmount;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<DevHEPSpawner>(-905833192, DevHEPSpawner.OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		this.directionController = new EightDirectionController(base.GetComponent<KBatchedAnimController>(), "redirector_target", "redirect", EightDirectionController.Offset.Infront);
		this.Direction = this.Direction;
		this.particleController = new MeterController(base.GetComponent<KBatchedAnimController>(), "orb_target", "orb_off", Meter.Offset.NoChange, Grid.SceneLayer.NoLayer, Array.Empty<string>());
		this.particleController.gameObject.AddOrGet<LoopingSounds>();
		this.progressMeterController = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
	}

	public void LauncherUpdate(float dt)
	{
		if (this.boltAmount <= 0f)
		{
			return;
		}
		this.launcherTimer += dt;
		this.progressMeterController.SetPositionPercent(this.launcherTimer / 5f);
		if (this.launcherTimer > 5f)
		{
			this.launcherTimer -= 5f;
			int highEnergyParticleOutputCell = base.GetComponent<Building>().GetHighEnergyParticleOutputCell();
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("HighEnergyParticle"), Grid.CellToPosCCC(highEnergyParticleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2, null, 0);
			gameObject.SetActive(true);
			if (gameObject != null)
			{
				HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
				component.payload = this.boltAmount;
				component.SetDirection(this.Direction);
				this.directionController.PlayAnim("redirect_send", KAnim.PlayMode.Once);
				this.directionController.controller.Queue("redirect", KAnim.PlayMode.Once, 1f, 0f);
				this.particleController.meterController.Play("orb_send", KAnim.PlayMode.Once, 1f, 0f);
				this.particleController.meterController.Queue("orb_off", KAnim.PlayMode.Once, 1f, 0f);
			}
		}
	}

	public string SliderTitleKey
	{
		get
		{
			return "";
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
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 500f;
	}

	public float GetSliderValue(int index)
	{
		return this.boltAmount;
	}

	public void SetSliderValue(float value, int index)
	{
		this.boltAmount = value;
	}

	public string GetSliderTooltipKey(int index)
	{
		return "";
	}

	string ISliderControl.GetSliderTooltip()
	{
		return "";
	}

	[MyCmpGet]
	private Operational operational;

	[Serialize]
	private EightDirection _direction;

	public float boltAmount;

	private EightDirectionController directionController;

	private float launcherTimer;

	private MeterController particleController;

	private MeterController progressMeterController;

	[Serialize]
	public Ref<HighEnergyParticlePort> capturedByRef = new Ref<HighEnergyParticlePort>();

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<DevHEPSpawner> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<DevHEPSpawner>(delegate(DevHEPSpawner component, object data)
	{
		component.OnCopySettings(data);
	});

	public class StatesInstance : GameStateMachine<DevHEPSpawner.States, DevHEPSpawner.StatesInstance, DevHEPSpawner, object>.GameInstance
	{
		public StatesInstance(DevHEPSpawner smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<DevHEPSpawner.States, DevHEPSpawner.StatesInstance, DevHEPSpawner>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.inoperational;
			this.inoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.ready, false);
			this.ready.PlayAnim("on").TagTransition(GameTags.Operational, this.inoperational, true).Update(delegate(DevHEPSpawner.StatesInstance smi, float dt)
			{
				smi.master.LauncherUpdate(dt);
			}, UpdateRate.SIM_EVERY_TICK, false);
		}

		public StateMachine<DevHEPSpawner.States, DevHEPSpawner.StatesInstance, DevHEPSpawner, object>.BoolParameter isAbsorbingRadiation;

		public GameStateMachine<DevHEPSpawner.States, DevHEPSpawner.StatesInstance, DevHEPSpawner, object>.State ready;

		public GameStateMachine<DevHEPSpawner.States, DevHEPSpawner.StatesInstance, DevHEPSpawner, object>.State inoperational;
	}
}
