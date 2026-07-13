using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class CreatureSimTemperatureTransfer : SimTemperatureTransfer, ISim200ms
{
	protected override void OnPrefabInit()
	{
		this.primaryElement = base.GetComponent<PrimaryElement>();
		this.average_kilowatts_exchanged = new RunningWeightedAverage(-10f, 10f, 20, true);
		this.averageTemperatureTransferPerSecond = new AttributeModifier(this.temperatureAttributeName + "Delta", 0f, CreatureSimTemperatureTransfer.RESULT_MODIFIER_NAME, false, true, false);
		this.GetAttributes().Add(this.averageTemperatureTransferPerSecond);
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		AttributeInstance attributeInstance = base.gameObject.GetAttributes().Add(Db.Get().Attributes.ThermalConductivityBarrier);
		AttributeModifier attributeModifier = new AttributeModifier(Db.Get().Attributes.ThermalConductivityBarrier.Id, this.skinThickness, this.skinThicknessAttributeModifierName, false, false, true);
		attributeInstance.Add(attributeModifier);
		base.OnSpawn();
	}

	public bool LastTemperatureRecordIsReliable
	{
		get
		{
			return Time.time - this.lastTemperatureRecordTime < 2f && this.average_kilowatts_exchanged.ValidRecordsInLastSeconds(4f) > 5;
		}
	}

	protected unsafe void unsafeUpdateAverageKiloWattsExchanged(float dt)
	{
		if (Time.time < this.lastTemperatureRecordTime + 0.2f)
		{
			return;
		}
		if (Sim.IsValidHandle(this.simHandle))
		{
			int handleIndex = Sim.GetHandleIndex(this.simHandle);
			if (Game.Instance.simData.elementChunks[handleIndex].deltaKJ == 0f)
			{
				return;
			}
			this.average_kilowatts_exchanged.AddSample(Game.Instance.simData.elementChunks[handleIndex].deltaKJ, Time.time);
			this.lastTemperatureRecordTime = Time.time;
		}
	}

	private void Update()
	{
		this.unsafeUpdateAverageKiloWattsExchanged(Time.deltaTime);
	}

	public void Sim200ms(float dt)
	{
		this.averageTemperatureTransferPerSecond.SetValue(SimUtil.EnergyFlowToTemperatureDelta(this.average_kilowatts_exchanged.GetUnweightedAverage, this.primaryElement.Element.specificHeatCapacity, this.primaryElement.Mass));
		float num = 0f;
		foreach (AttributeModifier attributeModifier in this.NonSimTemperatureModifiers)
		{
			num += attributeModifier.Value;
		}
		if (Sim.IsValidHandle(this.simHandle))
		{
			float num2 = num * (this.primaryElement.Mass * 1000f) * this.primaryElement.Element.specificHeatCapacity * 0.001f;
			float num3 = num2 * dt;
			SimMessages.ModifyElementChunkEnergy(this.simHandle, num3);
			this.heatEffect.SetHeatBeingProducedValue(num2);
			return;
		}
		this.heatEffect.SetHeatBeingProducedValue(0f);
	}

	public void RefreshRegistration()
	{
		base.SimUnregister();
		AttributeInstance attributeInstance = base.gameObject.GetAttributes().Get(Db.Get().Attributes.ThermalConductivityBarrier);
		this.thickness = attributeInstance.GetTotalValue();
		this.simHandle = -1;
		base.SimRegister();
	}

	public static float PotentialEnergyFlowToCreature(int cell, PrimaryElement transfererPrimaryElement, SimTemperatureTransfer temperatureTransferer, float deltaTime = 1f)
	{
		return SimUtil.CalculateEnergyFlowCreatures(cell, transfererPrimaryElement.Temperature, transfererPrimaryElement.Element.specificHeatCapacity, transfererPrimaryElement.Element.thermalConductivity, temperatureTransferer.SurfaceArea, temperatureTransferer.Thickness);
	}

	public static string RESULT_MODIFIER_NAME = DUPLICANTS.MODIFIERS.TEMPEXCHANGE.NAME;

	public string temperatureAttributeName = "Temperature";

	public float skinThickness = DUPLICANTSTATS.STANDARD.Temperature.SKIN_THICKNESS;

	public string skinThicknessAttributeModifierName = DUPLICANTS.MODEL.STANDARD.NAME;

	public AttributeModifier averageTemperatureTransferPerSecond;

	[MyCmpAdd]
	private KBatchedAnimHeatPostProcessingEffect heatEffect;

	private PrimaryElement primaryElement;

	public RunningWeightedAverage average_kilowatts_exchanged;

	public List<AttributeModifier> NonSimTemperatureModifiers = new List<AttributeModifier>();

	private float lastTemperatureRecordTime;
}
