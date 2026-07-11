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
		storage.gameObject.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		storage.gameObject.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
		this.operational = true;
		Operational component = storage.gameObject.GetComponent<Operational>();
		if (component != null)
		{
			this.operational = component.IsOperational;
		}
		this.OnOperationalChanged(this.operational);
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

	private void Register(SimTemperatureTransfer stt)
	{
		stt.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Remove(stt.onSimRegistered, new Action<SimTemperatureTransfer>(this.OnItemSimRegistered));
		stt.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Combine(stt.onSimRegistered, new Action<SimTemperatureTransfer>(this.OnItemSimRegistered));
		if (Sim.IsValidHandle(stt.SimHandle))
		{
			this.OnItemSimRegistered(stt);
		}
	}

	private void Unregister(SimTemperatureTransfer stt)
	{
		stt.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Remove(stt.onSimRegistered, new Action<SimTemperatureTransfer>(this.OnItemSimRegistered));
		if (Sim.IsValidHandle(stt.SimHandle))
		{
			SimMessages.ModifyElementChunkTemperatureAdjuster(stt.SimHandle, 0f, 0f, 0f);
		}
	}

	private void OnItemSimRegistered(SimTemperatureTransfer stt)
	{
		if (stt == null)
		{
			return;
		}
		if (Sim.IsValidHandle(stt.SimHandle))
		{
			float num = this.temperature;
			float num2 = this.heatCapacity;
			float num3 = this.thermalConductivity;
			if (!this.operational)
			{
				num = 0f;
				num2 = 0f;
				num3 = 0f;
			}
			SimMessages.ModifyElementChunkTemperatureAdjuster(stt.SimHandle, num, num2, num3);
		}
	}

	private void OnOperationalChanged(object data)
	{
		this.operational = (bool)data;
		if (this.operational)
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
		this.storage.gameObject.Unsubscribe(-1697596308, new Action<object>(this.OnStorageChanged));
		foreach (GameObject gameObject in this.storage.items)
		{
			if (gameObject != null)
			{
				SimTemperatureTransfer component = gameObject.GetComponent<SimTemperatureTransfer>();
				this.Unregister(component);
			}
		}
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		SimTemperatureTransfer component = gameObject.GetComponent<SimTemperatureTransfer>();
		if (component == null)
		{
			return;
		}
		Pickupable component2 = gameObject.GetComponent<Pickupable>();
		if (component2 == null)
		{
			return;
		}
		bool flag = this.operational && component2.storage == this.storage;
		if (flag)
		{
			this.Register(component);
		}
		else
		{
			this.Unregister(component);
		}
	}

	private float temperature;

	private float heatCapacity;

	private float thermalConductivity;

	private bool operational;

	private Storage storage;
}
