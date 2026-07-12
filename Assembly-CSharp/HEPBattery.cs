using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class HEPBattery : GameStateMachine<HEPBattery, HEPBattery.Instance, IStateMachineTarget, HEPBattery.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inoperational;
		this.inoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational, false).Update(delegate(HEPBattery.Instance smi, float dt)
		{
			smi.DoConsumeParticlesWhileDisabled(dt);
			smi.UpdateDecayStatusItem(false);
		}, UpdateRate.SIM_200ms, false);
		this.operational.Enter("SetActive(true)", delegate(HEPBattery.Instance smi)
		{
			smi.operational.SetActive(true, false);
		}).Exit("SetActive(false)", delegate(HEPBattery.Instance smi)
		{
			smi.operational.SetActive(false, false);
		}).PlayAnim("on", KAnim.PlayMode.Loop)
			.TagTransition(GameTags.Operational, this.inoperational, true)
			.Update(new Action<HEPBattery.Instance, float>(this.LauncherUpdate), UpdateRate.SIM_200ms, false);
	}

	public void LauncherUpdate(HEPBattery.Instance smi, float dt)
	{
		smi.UpdateDecayStatusItem(true);
		smi.UpdateMeter(null);
		smi.operational.SetActive(smi.particleStorage.Particles > 0f, false);
		smi.launcherTimer += dt;
		if (smi.launcherTimer < smi.def.minLaunchInterval || !smi.AllowSpawnParticles)
		{
			return;
		}
		if (smi.particleStorage.Particles >= smi.particleThreshold)
		{
			smi.launcherTimer = 0f;
			this.Fire(smi);
		}
	}

	public void Fire(HEPBattery.Instance smi)
	{
		int highEnergyParticleOutputCell = smi.GetComponent<Building>().GetHighEnergyParticleOutputCell();
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("HighEnergyParticle"), Grid.CellToPosCCC(highEnergyParticleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2, null, 0);
		gameObject.SetActive(true);
		if (gameObject != null)
		{
			HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
			component.payload = smi.particleStorage.ConsumeAndGet(smi.particleThreshold);
			component.SetDirection(smi.def.direction);
		}
	}

	public static readonly HashedString FIRE_PORT_ID = "HEPBatteryFire";

	public GameStateMachine<HEPBattery, HEPBattery.Instance, IStateMachineTarget, HEPBattery.Def>.State inoperational;

	public GameStateMachine<HEPBattery, HEPBattery.Instance, IStateMachineTarget, HEPBattery.Def>.State operational;

	public class Def : StateMachine.BaseDef
	{
		public float particleDecayRate;

		public float minLaunchInterval;

		public float minSlider;

		public float maxSlider;

		public EightDirection direction;
	}

	public new class Instance : GameStateMachine<HEPBattery, HEPBattery.Instance, IStateMachineTarget, HEPBattery.Def>.GameInstance, ISingleSliderControl, ISliderControl
	{
		public Instance(IStateMachineTarget master, HEPBattery.Def def)
			: base(master, def)
		{
			base.Subscribe(-801688580, new Action<object>(this.OnLogicValueChanged));
			this.meterController = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
			this.UpdateMeter(null);
		}

		public void DoConsumeParticlesWhileDisabled(float dt)
		{
			if (this.m_skipFirstUpdate)
			{
				this.m_skipFirstUpdate = false;
				return;
			}
			this.particleStorage.ConsumeAndGet(dt * base.def.particleDecayRate);
			this.UpdateMeter(null);
		}

		public void UpdateMeter(object data = null)
		{
			this.meterController.SetPositionPercent(this.particleStorage.Particles / this.particleStorage.Capacity());
		}

		public void UpdateDecayStatusItem(bool hasPower)
		{
			if (!hasPower)
			{
				if (this.particleStorage.Particles > 0f)
				{
					if (this.statusHandle == Guid.Empty)
					{
						this.statusHandle = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.LosingRadbolts, null);
						return;
					}
				}
				else if (this.statusHandle != Guid.Empty)
				{
					base.GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle, false);
					this.statusHandle = Guid.Empty;
					return;
				}
			}
			else if (this.statusHandle != Guid.Empty)
			{
				base.GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle, false);
				this.statusHandle = Guid.Empty;
			}
		}

		public bool AllowSpawnParticles
		{
			get
			{
				return this.hasLogicWire && this.isLogicActive;
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
			int portCell = base.GetComponent<LogicPorts>().GetPortCell(HEPBattery.FIRE_PORT_ID);
			return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell);
		}

		private void OnLogicValueChanged(object data)
		{
			LogicValueChanged logicValueChanged = (LogicValueChanged)data;
			if (logicValueChanged.portID == HEPBattery.FIRE_PORT_ID)
			{
				this.isLogicActive = logicValueChanged.newValue > 0;
				this.hasLogicWire = this.GetNetwork() != null;
			}
		}

		public string SliderTitleKey
		{
			get
			{
				return "STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TITLE";
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
			return base.def.minSlider;
		}

		public float GetSliderMax(int index)
		{
			return base.def.maxSlider;
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
			return "STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP";
		}

		string ISliderControl.GetSliderTooltip()
		{
			return string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP"), this.particleThreshold);
		}

		[MyCmpReq]
		public HighEnergyParticleStorage particleStorage;

		[MyCmpGet]
		public Operational operational;

		[Serialize]
		public float launcherTimer;

		[Serialize]
		public float particleThreshold = 50f;

		public bool ShowWorkingStatus;

		private bool m_skipFirstUpdate = true;

		private MeterController meterController;

		private Guid statusHandle = Guid.Empty;

		private bool hasLogicWire;

		private bool isLogicActive;
	}
}
