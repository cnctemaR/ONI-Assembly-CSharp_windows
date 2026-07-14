using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SpaceHeater : StateMachineComponent<SpaceHeater.StatesInstance>, IGameObjectEffectDescriptor, ISingleSliderControl, ISliderControl
{
	public float TargetTemperature
	{
		get
		{
			return this.targetTemperature;
		}
	}

	public float MinSelfHeatKWs
	{
		get
		{
			return this.maxSelfHeatKWs * (this.minPower / this.maxPower);
		}
	}

	public float MinExhaustedKWs
	{
		get
		{
			return this.maxExhaustedKWs * (this.minPower / this.maxPower);
		}
	}

	public float CurrentSelfHeatKW
	{
		get
		{
			return Mathf.Lerp(this.MinSelfHeatKWs, this.maxSelfHeatKWs, this.UserSliderSetting);
		}
	}

	public float CurrentExhaustedKW
	{
		get
		{
			return Mathf.Lerp(this.MinExhaustedKWs, this.maxExhaustedKWs, this.UserSliderSetting);
		}
	}

	public float CurrentPowerConsumption
	{
		get
		{
			return Mathf.Lerp(this.minPower, this.maxPower, this.UserSliderSetting);
		}
	}

	public static void GenerateHeat(SpaceHeater.StatesInstance smi, float dt)
	{
		if (smi.master.produceHeat)
		{
			SpaceHeater.AddExhaustHeat(smi, dt);
			SpaceHeater.AddSelfHeat(smi, dt);
		}
	}

	private static float AddExhaustHeat(SpaceHeater.StatesInstance smi, float dt)
	{
		float currentExhaustedKW = smi.master.CurrentExhaustedKW;
		float num = ((smi.master.bubbleEmitter != null) ? smi.master.bubbleEmitter.DivertEnergy(currentExhaustedKW, dt) : 0f);
		float num2 = Mathf.Max(currentExhaustedKW - num, 0f);
		float num3 = (smi.master.heatLiquid ? 358.15f : smi.master.overheatTemperature);
		float num4 = (smi.master.IsTurboModeActive ? 10000f : num3);
		StructureTemperatureComponents.ExhaustHeat(smi.master.extents, num2, num4, dt);
		return num2;
	}

	public static void RefreshHeatEffect(SpaceHeater.StatesInstance smi)
	{
		if (smi.master.heatEffect != null && smi.master.produceHeat)
		{
			float num = (smi.IsInsideState(smi.sm.online.heating) ? (smi.master.CurrentExhaustedKW + smi.master.CurrentSelfHeatKW) : 0f);
			smi.master.heatEffect.SetHeatBeingProducedValue(num);
		}
	}

	private static float AddSelfHeat(SpaceHeater.StatesInstance smi, float dt)
	{
		float currentSelfHeatKW = smi.master.CurrentSelfHeatKW;
		GameComps.StructureTemperatures.ProduceEnergy(smi.master.structureTemperature, currentSelfHeatKW * dt, BUILDINGS.PREFABS.STEAMTURBINE2.HEAT_SOURCE, dt);
		return currentSelfHeatKW;
	}

	public bool IsTurboModeActive
	{
		get
		{
			return this.heatLiquid && this.UserSliderSetting > 0f;
		}
	}

	public static void RefreshTepidizerAnim(SpaceHeater.StatesInstance smi)
	{
		if (!smi.master.heatLiquid)
		{
			return;
		}
		if (smi.IsInsideState(smi.sm.offline) || smi.IsInsideState(smi.sm.online.undermassliquid) || smi.IsInsideState(smi.sm.online.undermassgas) || smi.IsInsideState(smi.sm.online.overtemp))
		{
			smi.master.animController.Play("off", KAnim.PlayMode.Once, 1f, 0f);
			return;
		}
		if (!smi.IsInsideState(smi.sm.online.heating))
		{
			smi.master.animController.Play("on", KAnim.PlayMode.Once, 1f, 0f);
			return;
		}
		if (smi.master.IsTurboModeActive)
		{
			smi.master.animController.Play("working_loop_turbo", KAnim.PlayMode.Loop, 1f, 0f);
			return;
		}
		smi.master.animController.Play("working_loop", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void SetUserSpecifiedPowerConsumptionValue(float value)
	{
		if (this.produceHeat)
		{
			this.UserSliderSetting = (value - this.minPower) / (this.maxPower - this.minPower);
			SpaceHeater.RefreshHeatEffect(base.smi);
			this.energyConsumer.BaseWattageRating = this.CurrentPowerConsumption;
			this.RefreshTurboModeStatusItem();
			if (this.IsTurboModeActive && base.smi.IsInsideState(base.smi.sm.online.overtemp))
			{
				base.smi.GoTo(base.smi.sm.online.heating);
			}
			if (base.smi.IsInsideState(base.smi.sm.online.heating))
			{
				SpaceHeater.RefreshTepidizerAnim(base.smi);
			}
		}
	}

	private void RefreshTurboModeStatusItem()
	{
		if (!this.heatLiquid || this.turboModeStatusItem == null)
		{
			return;
		}
		KSelectable component = base.GetComponent<KSelectable>();
		if (this.IsTurboModeActive)
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, this.turboModeStatusItem, this);
			return;
		}
		component.SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, null, null);
	}

	protected override void OnPrefabInit()
	{
		if (this.produceHeat)
		{
			this.heatStatusItem = new StatusItem("OperatingEnergy", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.heatStatusItem.resolveStringCallback = delegate(string str, object data)
			{
				SpaceHeater.StatesInstance statesInstance = (SpaceHeater.StatesInstance)data;
				float num = statesInstance.master.CurrentSelfHeatKW + statesInstance.master.CurrentExhaustedKW;
				str = string.Format(str, GameUtil.GetFormattedHeatEnergy(num * 1000f, GameUtil.HeatEnergyFormatterUnit.Automatic));
				return str;
			};
			this.heatStatusItem.resolveTooltipCallback = delegate(string str, object data)
			{
				SpaceHeater.StatesInstance statesInstance2 = (SpaceHeater.StatesInstance)data;
				float num2 = statesInstance2.master.CurrentSelfHeatKW + statesInstance2.master.CurrentExhaustedKW;
				str = str.Replace("{0}", GameUtil.GetFormattedHeatEnergy(num2 * 1000f, GameUtil.HeatEnergyFormatterUnit.Automatic));
				string text = string.Format(BUILDING.STATUSITEMS.OPERATINGENERGY.LINEITEM, BUILDING.STATUSITEMS.OPERATINGENERGY.OPERATING, GameUtil.GetFormattedHeatEnergy(statesInstance2.master.CurrentSelfHeatKW * 1000f, GameUtil.HeatEnergyFormatterUnit.DTU_S));
				text += string.Format(BUILDING.STATUSITEMS.OPERATINGENERGY.LINEITEM, BUILDING.STATUSITEMS.OPERATINGENERGY.EXHAUSTING, GameUtil.GetFormattedHeatEnergy(statesInstance2.master.CurrentExhaustedKW * 1000f, GameUtil.HeatEnergyFormatterUnit.DTU_S));
				str = str.Replace("{1}", text);
				return str;
			};
		}
		if (this.heatLiquid)
		{
			this.turboModeStatusItem = new StatusItem("TurboMode", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.turboModeStatusItem.resolveStringCallback = delegate(string str, object data)
			{
				SpaceHeater spaceHeater = (SpaceHeater)data;
				str = string.Format(str, GameUtil.GetFormattedWattage(spaceHeater.CurrentPowerConsumption, GameUtil.WattageFormatterUnit.Automatic, true), GameUtil.GetFormattedHeatEnergyRate((spaceHeater.CurrentSelfHeatKW + spaceHeater.CurrentExhaustedKW) * 1000f, GameUtil.HeatEnergyFormatterUnit.Automatic));
				return str;
			};
			this.turboModeStatusItem.resolveTooltipCallback = delegate(string str, object data)
			{
				SpaceHeater spaceHeater2 = (SpaceHeater)data;
				return string.Format(str, GameUtil.GetFormattedWattage(spaceHeater2.CurrentPowerConsumption, GameUtil.WattageFormatterUnit.Automatic, true), GameUtil.GetFormattedHeatEnergyRate((spaceHeater2.CurrentSelfHeatKW + spaceHeater2.CurrentExhaustedKW) * 1000f, GameUtil.HeatEnergyFormatterUnit.Automatic));
			};
		}
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("InsulationTutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Insulation, true);
		}, null, null);
		this.extents = base.GetComponent<OccupyArea>().GetExtents();
		this.overheatTemperature = base.GetComponent<BuildingComplete>().Def.OverheatTemperature;
		this.structureTemperature = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		base.smi.StartSM();
		this.SetUserSpecifiedPowerConsumptionValue(this.CurrentPowerConsumption);
	}

	public void SetLiquidHeater()
	{
		this.heatLiquid = true;
	}

	private SpaceHeater.MonitorState MonitorHeating(float dt)
	{
		this.monitorCells.Clear();
		GameUtil.GetNonSolidCells(Grid.PosToCell(base.transform.GetPosition()), this.radius, this.monitorCells);
		int num = 0;
		float num2 = 0f;
		foreach (int num3 in this.monitorCells)
		{
			if (Grid.Mass[num3] > this.minimumCellMass && ((Grid.Element[num3].IsGas && !this.heatLiquid) || (Grid.Element[num3].IsLiquid && this.heatLiquid)))
			{
				num++;
				num2 += Grid.Temperature[num3];
			}
		}
		if (num == 0)
		{
			if (!this.heatLiquid)
			{
				return SpaceHeater.MonitorState.NotEnoughGas;
			}
			return SpaceHeater.MonitorState.NotEnoughLiquid;
		}
		else
		{
			if (this.hasTargetTemperature && num2 / (float)num >= this.targetTemperature)
			{
				return SpaceHeater.MonitorState.TooHot;
			}
			if (this.heatLiquid && !this.IsTurboModeActive && num2 / (float)num >= 358.15f)
			{
				return SpaceHeater.MonitorState.TooHot;
			}
			return SpaceHeater.MonitorState.ReadyToHeat;
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.hasTargetTemperature)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.HEATER_TARGETTEMPERATURE, GameUtil.GetFormattedTemperature(this.targetTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.HEATER_TARGETTEMPERATURE, GameUtil.GetFormattedTemperature(this.targetTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
		}
		return list;
	}

	public string SliderTitleKey
	{
		get
		{
			return "STRINGS.UI.UISIDESCREENS.SPACEHEATERSIDESCREEN.TITLE";
		}
	}

	public string SliderUnits
	{
		get
		{
			return UI.UNITSUFFIXES.ELECTRICAL.WATT;
		}
	}

	public int SliderDecimalPlaces(int index)
	{
		return 0;
	}

	public float GetSliderMin(int index)
	{
		if (!this.produceHeat || this.heatLiquid)
		{
			return 0f;
		}
		return this.minPower;
	}

	public float GetSliderMax(int index)
	{
		if (!this.produceHeat || this.heatLiquid)
		{
			return 0f;
		}
		return this.maxPower;
	}

	public float GetSliderValue(int index)
	{
		return this.CurrentPowerConsumption;
	}

	public void SetSliderValue(float value, int index)
	{
		this.SetUserSpecifiedPowerConsumptionValue(value);
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.SPACEHEATERSIDESCREEN.TOOLTIP";
	}

	string ISliderControl.GetSliderTooltip(int index)
	{
		return string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.SPACEHEATERSIDESCREEN.TOOLTIP"), GameUtil.GetFormattedHeatEnergyRate((this.CurrentSelfHeatKW + this.CurrentExhaustedKW) * 1000f, GameUtil.HeatEnergyFormatterUnit.Automatic));
	}

	public float targetTemperature = 308.15f;

	public float minimumCellMass;

	public int radius = 2;

	[SerializeField]
	public bool heatLiquid;

	public bool hasTargetTemperature = true;

	[Serialize]
	public float UserSliderSetting;

	public bool produceHeat;

	public float maxPower;

	public float minPower;

	public float maxSelfHeatKWs;

	public float maxExhaustedKWs;

	private StatusItem heatStatusItem;

	private StatusItem turboModeStatusItem;

	private HandleVector<int>.Handle structureTemperature;

	private Extents extents;

	private float overheatTemperature;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpGet]
	private KBatchedAnimHeatPostProcessingEffect heatEffect;

	[MyCmpGet]
	private KBatchedAnimController animController;

	[MyCmpGet]
	private EnergyConsumer energyConsumer;

	[MyCmpGet]
	private LiquidHeaterBubbleEmitter bubbleEmitter;

	private List<int> monitorCells = new List<int>();

	public class StatesInstance : GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.GameInstance
	{
		public StatesInstance(SpaceHeater master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.offline;
			base.serializable = StateMachine.SerializeType.Never;
			this.statusItemUnderMassLiquid = new StatusItem("statusItemUnderMassLiquid", BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022, true, null);
			this.statusItemUnderMassGas = new StatusItem("statusItemUnderMassGas", BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022, true, null);
			this.statusItemOverTemp = new StatusItem("statusItemOverTemp", BUILDING.STATUSITEMS.HEATINGSTALLEDHOTENV.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDHOTENV.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 129022, true, null);
			this.statusItemOverTemp.resolveStringCallback = delegate(string str, object obj)
			{
				SpaceHeater.StatesInstance statesInstance = (SpaceHeater.StatesInstance)obj;
				float num = (statesInstance.master.heatLiquid ? 358.15f : statesInstance.master.TargetTemperature);
				return string.Format(str, GameUtil.GetFormattedTemperature(num, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
			};
			this.offline.Enter(new StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State.Callback(SpaceHeater.RefreshHeatEffect)).Enter(new StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State.Callback(SpaceHeater.RefreshTepidizerAnim)).EventTransition(GameHashes.OperationalChanged, this.online, (SpaceHeater.StatesInstance smi) => smi.master.operational.IsOperational);
			this.online.EventTransition(GameHashes.OperationalChanged, this.offline, (SpaceHeater.StatesInstance smi) => !smi.master.operational.IsOperational).Enter(new StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State.Callback(SpaceHeater.RefreshTepidizerAnim)).Enter(new StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State.Callback(SpaceHeater.RefreshHeatEffect))
				.Update("spaceheater_online", delegate(SpaceHeater.StatesInstance smi, float dt)
				{
					switch (smi.master.MonitorHeating(dt))
					{
					case SpaceHeater.MonitorState.ReadyToHeat:
						smi.GoTo(this.online.heating);
						return;
					case SpaceHeater.MonitorState.TooHot:
						smi.GoTo(this.online.overtemp);
						return;
					case SpaceHeater.MonitorState.NotEnoughLiquid:
						smi.GoTo(this.online.undermassliquid);
						return;
					case SpaceHeater.MonitorState.NotEnoughGas:
						smi.GoTo(this.online.undermassgas);
						return;
					default:
						return;
					}
				}, UpdateRate.SIM_1000ms, false);
			this.online.heating.Enter(new StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State.Callback(SpaceHeater.RefreshHeatEffect)).Enter(new StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State.Callback(SpaceHeater.RefreshTepidizerAnim)).Enter(delegate(SpaceHeater.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			})
				.ToggleStatusItem((SpaceHeater.StatesInstance smi) => smi.master.heatStatusItem, (SpaceHeater.StatesInstance smi) => smi)
				.Update(new Action<SpaceHeater.StatesInstance, float>(SpaceHeater.GenerateHeat), UpdateRate.SIM_200ms, false)
				.Exit(delegate(SpaceHeater.StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				})
				.Exit(new StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State.Callback(SpaceHeater.RefreshHeatEffect));
			this.online.undermassliquid.Enter(new StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State.Callback(SpaceHeater.RefreshTepidizerAnim)).ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, this.statusItemUnderMassLiquid, null);
			this.online.undermassgas.Enter(new StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State.Callback(SpaceHeater.RefreshTepidizerAnim)).ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, this.statusItemUnderMassGas, null);
			this.online.overtemp.Enter(new StateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State.Callback(SpaceHeater.RefreshTepidizerAnim)).ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, this.statusItemOverTemp, null);
		}

		public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State offline;

		public SpaceHeater.States.OnlineStates online;

		private StatusItem statusItemUnderMassLiquid;

		private StatusItem statusItemUnderMassGas;

		private StatusItem statusItemOverTemp;

		public class OnlineStates : GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State
		{
			public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State heating;

			public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State overtemp;

			public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State undermassliquid;

			public GameStateMachine<SpaceHeater.States, SpaceHeater.StatesInstance, SpaceHeater, object>.State undermassgas;
		}
	}

	private enum MonitorState
	{
		ReadyToHeat,
		TooHot,
		NotEnoughLiquid,
		NotEnoughGas
	}
}
