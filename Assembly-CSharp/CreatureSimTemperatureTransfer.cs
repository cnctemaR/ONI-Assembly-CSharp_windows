using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using STRINGS;

public class CreatureSimTemperatureTransfer : SimTemperatureTransfer, ISim200ms
{
	public float deltaEnergy
	{
		get
		{
			return this.deltaKJ;
		}
		protected set
		{
			this.deltaKJ = value;
		}
	}

	public float currentExchangeWattage
	{
		get
		{
			return this.deltaKJ * 5f * 1000f;
		}
	}

	protected override void OnPrefabInit()
	{
		this.primaryElement = base.GetComponent<PrimaryElement>();
		this.average_kilowatts_exchanged = new RunningWeightedAverage(-10f, 10f, 20, true);
		this.surfaceArea = 1f;
		this.thickness = 0.002f;
		this.groundTransferScale = 0f;
		AttributeInstance attributeInstance = base.gameObject.GetAttributes().Add(Db.Get().Attributes.ThermalConductivityBarrier);
		AttributeModifier attributeModifier = new AttributeModifier(Db.Get().Attributes.ThermalConductivityBarrier.Id, this.thickness, DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME, false, false, true);
		attributeInstance.Add(attributeModifier);
		this.averageTemperatureTransferPerSecond = new AttributeModifier("TemperatureDelta", 0f, DUPLICANTS.MODIFIERS.TEMPEXCHANGE.NAME, false, true, false);
		this.GetAttributes().Add(this.averageTemperatureTransferPerSecond);
		base.OnPrefabInit();
	}

	public void Sim200ms(float dt)
	{
		this.average_kilowatts_exchanged.AddSample(this.currentExchangeWattage * 0.001f);
		this.averageTemperatureTransferPerSecond.SetValue(SimUtil.EnergyFlowToTemperatureDelta(this.average_kilowatts_exchanged.GetWeightedAverage, this.primaryElement.Element.specificHeatCapacity, this.primaryElement.Mass));
		float num = 0f;
		foreach (AttributeModifier attributeModifier in this.NonSimTemperatureModifiers)
		{
			num += attributeModifier.Value;
		}
		if (Sim.IsValidHandle(this.simHandle))
		{
			SimMessages.ModifyElementChunkEnergy(this.simHandle, num * dt * (this.primaryElement.Mass * 1000f) * this.primaryElement.Element.specificHeatCapacity * 0.001f);
		}
	}

	public void RefreshRegistration()
	{
		base.SimUnregister();
		AttributeInstance attributeInstance = base.gameObject.GetAttributes().Get("ThermalConductivityBarrier");
		this.thickness = attributeInstance.GetTotalValue();
		this.simHandle = -1;
		base.SimRegister();
	}

	public static float PotentialEnergyFlowToCreature(int cell, PrimaryElement transfererPrimaryElement, SimTemperatureTransfer temperatureTransferer, float deltaTime = 1f)
	{
		return SimUtil.CalculateEnergyFlowCreatures(cell, transfererPrimaryElement.Temperature, transfererPrimaryElement.Element.specificHeatCapacity, transfererPrimaryElement.Element.thermalConductivity, temperatureTransferer.SurfaceArea, temperatureTransferer.Thickness);
	}

	public AttributeModifier averageTemperatureTransferPerSecond;

	private PrimaryElement primaryElement;

	public RunningWeightedAverage average_kilowatts_exchanged;

	public List<AttributeModifier> NonSimTemperatureModifiers = new List<AttributeModifier>();
}
