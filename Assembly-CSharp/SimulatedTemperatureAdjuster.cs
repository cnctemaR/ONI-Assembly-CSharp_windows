using System;
using System.Collections.Generic;
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
		storage.temperatureAdjuster = this;
		storage.gameObject.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		for (int i = 0; i < storage.items.Count; i++)
		{
			GameObject gameObject = storage.items[i];
			if (gameObject != null)
			{
				SimTemperatureTransfer component = gameObject.GetComponent<SimTemperatureTransfer>();
				this.Register(component);
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

	public void Register(SimTemperatureTransfer stt)
	{
		if (stt == null)
		{
			return;
		}
		stt.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Remove(stt.onSimRegistered, new Action<SimTemperatureTransfer>(this.OnItemSimRegistered));
		stt.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Combine(stt.onSimRegistered, new Action<SimTemperatureTransfer>(this.OnItemSimRegistered));
		if (Sim.IsValidHandle(stt.SimHandle))
		{
			this.OnItemSimRegistered(stt);
		}
	}

	public void Unregister(SimTemperatureTransfer stt)
	{
		if (stt == null)
		{
			return;
		}
		stt.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Remove(stt.onSimRegistered, new Action<SimTemperatureTransfer>(this.OnItemSimRegistered));
		SimMessages.ModifyElementChunkTemperatureAdjuster(stt.SimHandle, 0f, 0f, 0f);
	}

	private void OnItemSimRegistered(SimTemperatureTransfer stt)
	{
		if (stt == null)
		{
			return;
		}
		if (Sim.IsValidHandle(stt.SimHandle))
		{
			SimMessages.ModifyElementChunkTemperatureAdjuster(stt.SimHandle, this.temperature, this.heatCapacity, this.thermalConductivity);
		}
	}

	private void OnOperationalChanged(object data)
	{
		bool flag = (bool)data;
		if (flag)
		{
			foreach (GameObject gameObject in this.storage.items)
			{
				if (gameObject != null)
				{
					SimTemperatureTransfer component = gameObject.GetComponent<SimTemperatureTransfer>();
					this.OnItemSimRegistered(component);
				}
			}
		}
		else
		{
			foreach (GameObject gameObject2 in this.storage.items)
			{
				if (gameObject2 != null)
				{
					SimTemperatureTransfer component2 = gameObject2.GetComponent<SimTemperatureTransfer>();
					this.Unregister(component2);
				}
			}
		}
	}

	public void CleanUp()
	{
		foreach (GameObject gameObject in this.storage.items)
		{
			if (gameObject != null)
			{
				SimTemperatureTransfer component = gameObject.GetComponent<SimTemperatureTransfer>();
				this.Unregister(component);
			}
		}
		this.storage.temperatureAdjuster = null;
	}

	private float temperature;

	private float heatCapacity;

	private float thermalConductivity;

	private Storage storage;
}
