using System;
using System.Collections.Generic;
using Klei;
using STRINGS;
using UnityEngine;

public class SimulatedTemperatureAdjuster
{
	public SimulatedTemperatureAdjuster(float simulated_temperature, float heat_capacity, float thermal_conductivity, Storage storage)
	{
		this.temperature = simulated_temperature;
		this.heatCapacity = heat_capacity;
		this.thermalConductivity = thermal_conductivity;
		this.storage = storage;
	}

	public void Update(float dt)
	{
		if (this.storage.items.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < this.storage.items.Count; i++)
		{
			GameObject gameObject = this.storage.items[i];
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			float num = component.Mass * component.Element.specificHeatCapacity;
			float num2 = component.Temperature;
			float num3 = SimUtil.CalculateEnergyFlow(this.temperature, this.heatCapacity, num2, this.thermalConductivity, 1f, 1f);
			if (num3 < 1f)
			{
				float num4 = SimUtil.ClampEnergyTransfer(dt, this.temperature, this.heatCapacity, num2, num, num3);
				SimTemperatureTransfer component2 = gameObject.GetComponent<SimTemperatureTransfer>();
				component2.ModifyEnergy(num4);
			}
		}
	}

	public List<Descriptor> GetDescriptors()
	{
		return SimulatedTemperatureAdjuster.GetDescriptors(this.temperature);
	}

	public static List<Descriptor> GetDescriptors(float temperature)
	{
		List<Descriptor> list = new List<Descriptor>();
		string formattedTemperature = GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true);
		Descriptor descriptor = new Descriptor(string.Format(UI.BUILDINGEFFECTS.ITEM_TEMPERATURE_ADJUST, formattedTemperature), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ITEM_TEMPERATURE_ADJUST, formattedTemperature), Descriptor.DescriptorType.Effect, false);
		list.Add(descriptor);
		return list;
	}

	private float temperature;

	private float heatCapacity;

	private float thermalConductivity;

	private Storage storage;
}
