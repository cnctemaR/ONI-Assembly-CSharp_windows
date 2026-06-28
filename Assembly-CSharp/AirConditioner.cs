using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class AirConditioner : KMonoBehaviour, ISaveLoadable, IEffectDescriptor
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
		this.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		this.Subscribe(824508782, new Action<object>(this.OnActiveChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.structureTemperature = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		this.cooledAirOutputCell = this.building.GetUtilityOutputCell();
	}

	private void Update()
	{
		if (this.operational != null && !this.operational.IsOperational)
		{
			this.operational.SetActive(false, false);
			return;
		}
		this.UpdateState(Time.deltaTime);
	}

	private void UpdateState(float dt)
	{
		bool flag = this.consumer.IsSatisfied;
		int cells = 0;
		float envTemp = 0f;
		if (this.occupyArea != null)
		{
			this.occupyArea.TestArea(Grid.PosToCell(base.gameObject), null, delegate(int cell, object data)
			{
				cells++;
				envTemp += Grid.Temperature[cell];
				return true;
			});
			envTemp /= (float)cells;
		}
		this.lastEnvTemp = envTemp;
		List<GameObject> items = this.storage.items;
		for (int i = 0; i < items.Count; i++)
		{
			PrimaryElement component = items[i].GetComponent<PrimaryElement>();
			if (component.Mass > 0f)
			{
				if (!this.isLiquidConditioner || !component.Element.IsGas)
				{
					if (this.isLiquidConditioner || !component.Element.IsLiquid)
					{
						flag = true;
						this.lastGasTemp = component.Temperature;
						float num = component.Temperature + this.temperatureDelta;
						if (num < 5f)
						{
							num = 5f;
							this.lowTempLag = Mathf.Min(this.lowTempLag + dt / 5f, 1f);
						}
						else
						{
							this.lowTempLag = Mathf.Min(this.lowTempLag - dt / 5f, 0f);
						}
						ConduitFlow conduitFlow = Game.Instance.gasConduitFlow;
						if (this.isLiquidConditioner)
						{
							conduitFlow = Game.Instance.liquidConduitFlow;
						}
						float num2 = conduitFlow.AddElement(this.cooledAirOutputCell, component.ElementID, component.Mass, num, component.DiseaseIdx, component.DiseaseCount);
						component.KeepZeroMassObject = true;
						float num3 = num2 / component.Mass;
						int num4 = (int)((float)component.DiseaseCount * num3);
						component.Mass -= num2;
						component.ModifyDiseaseCount(-num4, "AirConditioner.UpdateState");
						float num5 = num - component.Temperature;
						float num6 = num5 * component.Element.specificHeatCapacity * num2;
						float num7 = ((this.lastSampleTime <= 0f) ? 1f : (Time.time - this.lastSampleTime));
						this.lastSampleTime = Time.time;
						GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, -num6, BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, num7);
						break;
					}
				}
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
				this.statusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.CoolingStalledColdGas, this);
				this.showingLowTemp = true;
				this.showingHotEnv = false;
			}
			else if (this.lowTempLag <= 0f && (this.showingHotEnv || this.showingLowTemp))
			{
				this.statusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Cooling, null);
				this.showingLowTemp = false;
				this.showingHotEnv = false;
			}
			else if (this.statusHandle == Guid.Empty)
			{
				this.statusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Cooling, null);
				this.showingLowTemp = false;
				this.showingHotEnv = false;
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
		string formattedTemperature = GameUtil.GetFormattedTemperature(this.temperatureDelta, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative, true);
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.GASCOOLING, formattedTemperature), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GASCOOLING, formattedTemperature), Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
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

	private float lastSampleTime = -1f;
}
