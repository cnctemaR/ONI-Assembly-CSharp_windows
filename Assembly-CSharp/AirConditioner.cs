using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/AirConditioner")]
public class AirConditioner : KMonoBehaviour, ISaveLoadable, IEffectDescriptor, ISim200ms
{
	public float lastEnvTemp { get; private set; }

	public float lastGasTemp { get; private set; }

	public float TargetTemperature
	{
		get
		{
			return this.targetTemperature;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<AirConditioner>(-592767678, AirConditioner.OnOperationalChangedDelegate);
		base.Subscribe<AirConditioner>(824508782, AirConditioner.OnActiveChangedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("InsulationTutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Insulation, true);
		}, null, null);
		this.structureTemperature = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		this.cooledAirOutputCell = this.building.GetUtilityOutputCell();
	}

	public void Sim200ms(float dt)
	{
		if (this.operational != null && !this.operational.IsOperational)
		{
			this.operational.SetActive(false, false);
			return;
		}
		this.UpdateState(dt);
	}

	private static bool UpdateStateCb(int cell, object data)
	{
		AirConditioner airConditioner = data as AirConditioner;
		airConditioner.cellCount++;
		airConditioner.envTemp += Grid.Temperature[cell];
		return true;
	}

	private void UpdateState(float dt)
	{
		bool flag = this.consumer.IsSatisfied;
		this.envTemp = 0f;
		this.cellCount = 0;
		if (this.occupyArea != null && base.gameObject != null)
		{
			this.occupyArea.TestArea(Grid.PosToCell(base.gameObject), this, AirConditioner.UpdateStateCbDelegate);
			this.envTemp /= (float)this.cellCount;
		}
		this.lastEnvTemp = this.envTemp;
		List<GameObject> items = this.storage.items;
		for (int i = 0; i < items.Count; i++)
		{
			PrimaryElement component = items[i].GetComponent<PrimaryElement>();
			if (component.Mass > 0f && (!this.isLiquidConditioner || !component.Element.IsGas) && (this.isLiquidConditioner || !component.Element.IsLiquid))
			{
				flag = true;
				this.lastGasTemp = component.Temperature;
				float num = component.Temperature + this.temperatureDelta;
				if (num < 1f)
				{
					num = 1f;
					this.lowTempLag = Mathf.Min(this.lowTempLag + dt / 5f, 1f);
				}
				else
				{
					this.lowTempLag = Mathf.Min(this.lowTempLag - dt / 5f, 0f);
				}
				float num2 = (this.isLiquidConditioner ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow).AddElement(this.cooledAirOutputCell, component.ElementID, component.Mass, num, component.DiseaseIdx, component.DiseaseCount);
				component.KeepZeroMassObject = true;
				float num3 = num2 / component.Mass;
				int num4 = (int)((float)component.DiseaseCount * num3);
				component.Mass -= num2;
				component.ModifyDiseaseCount(-num4, "AirConditioner.UpdateState");
				float num5 = (num - component.Temperature) * component.Element.specificHeatCapacity * num2;
				float num6 = ((this.lastSampleTime > 0f) ? (Time.time - this.lastSampleTime) : 1f);
				this.lastSampleTime = Time.time;
				GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, -num5, BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, num6);
				break;
			}
		}
		if (Time.time - this.lastSampleTime > 2f)
		{
			GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, 0f, BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, Time.time - this.lastSampleTime);
			this.lastSampleTime = Time.time;
		}
		this.operational.SetActive(flag, false);
		this.UpdateStatus();
	}

	private void OnOperationalChanged(object data)
	{
		if (this.operational.IsOperational)
		{
			this.UpdateState(0f);
		}
	}

	private void OnActiveChanged(object data)
	{
		this.UpdateStatus();
	}

	private void UpdateStatus()
	{
		if (this.operational.IsActive)
		{
			if (this.lowTempLag >= 1f && !this.showingLowTemp)
			{
				this.statusHandle = (this.isLiquidConditioner ? this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.CoolingStalledColdLiquid, this) : this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.CoolingStalledColdGas, this));
				this.showingLowTemp = true;
				this.showingHotEnv = false;
				return;
			}
			if (this.lowTempLag <= 0f && (this.showingHotEnv || this.showingLowTemp))
			{
				this.statusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Cooling, null);
				this.showingLowTemp = false;
				this.showingHotEnv = false;
				return;
			}
			if (this.statusHandle == Guid.Empty)
			{
				this.statusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Cooling, null);
				this.showingLowTemp = false;
				this.showingHotEnv = false;
				return;
			}
		}
		else
		{
			this.statusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, null, null);
		}
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		string formattedTemperature = GameUtil.GetFormattedTemperature(this.temperatureDelta, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative, true, false);
		Element element = ElementLoader.FindElementByName(this.isLiquidConditioner ? "Water" : "Oxygen");
		float num;
		if (this.isLiquidConditioner)
		{
			num = Mathf.Abs(this.temperatureDelta * element.specificHeatCapacity * 10000f);
		}
		else
		{
			num = Mathf.Abs(this.temperatureDelta * element.specificHeatCapacity * 1000f);
		}
		float num2 = num * 1f;
		Descriptor descriptor = default(Descriptor);
		string text = string.Format(this.isLiquidConditioner ? UI.BUILDINGEFFECTS.HEATGENERATED_LIQUIDCONDITIONER : UI.BUILDINGEFFECTS.HEATGENERATED_AIRCONDITIONER, GameUtil.GetFormattedHeatEnergy(num2, GameUtil.HeatEnergyFormatterUnit.Automatic), GameUtil.GetFormattedTemperature(Mathf.Abs(this.temperatureDelta), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative, true, false));
		string text2 = string.Format(this.isLiquidConditioner ? UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED_LIQUIDCONDITIONER : UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED_AIRCONDITIONER, GameUtil.GetFormattedHeatEnergy(num2, GameUtil.HeatEnergyFormatterUnit.Automatic), GameUtil.GetFormattedTemperature(Mathf.Abs(this.temperatureDelta), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative, true, false));
		descriptor.SetupDescriptor(text, text2, Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		Descriptor descriptor2 = default(Descriptor);
		descriptor2.SetupDescriptor(string.Format(this.isLiquidConditioner ? UI.BUILDINGEFFECTS.LIQUIDCOOLING : UI.BUILDINGEFFECTS.GASCOOLING, formattedTemperature), string.Format(this.isLiquidConditioner ? UI.BUILDINGEFFECTS.TOOLTIPS.LIQUIDCOOLING : UI.BUILDINGEFFECTS.TOOLTIPS.GASCOOLING, formattedTemperature), Descriptor.DescriptorType.Effect);
		list.Add(descriptor2);
		return list;
	}

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	protected Storage storage;

	[MyCmpReq]
	protected Operational operational;

	[MyCmpReq]
	private ConduitConsumer consumer;

	[MyCmpReq]
	private BuildingComplete building;

	[MyCmpGet]
	private OccupyArea occupyArea;

	private HandleVector<int>.Handle structureTemperature;

	public float temperatureDelta = -14f;

	public float maxEnvironmentDelta = -50f;

	private float lowTempLag;

	private bool showingLowTemp;

	public bool isLiquidConditioner;

	private bool showingHotEnv;

	private Guid statusHandle;

	[Serialize]
	private float targetTemperature;

	private int cooledAirOutputCell = -1;

	private static readonly EventSystem.IntraObjectHandler<AirConditioner> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<AirConditioner>(delegate(AirConditioner component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<AirConditioner> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<AirConditioner>(delegate(AirConditioner component, object data)
	{
		component.OnActiveChanged(data);
	});

	private float lastSampleTime = -1f;

	private float envTemp;

	private int cellCount;

	private static readonly Func<int, object, bool> UpdateStateCbDelegate = (int cell, object data) => AirConditioner.UpdateStateCb(cell, data);
}
