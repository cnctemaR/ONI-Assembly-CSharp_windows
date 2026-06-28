using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTemperatureManager
{
	public HandleVector<StructureTemperature>.Handle Register(StructureTemperature item, float temperature, Element element)
	{
		if (temperature <= 0f)
		{
			Output.LogError(new object[] { "You are attempting to register a building with an invalid temperature" });
			return HandleVector<StructureTemperature>.InvalidHandle;
		}
		HandleVector<StructureTemperature>.Handle handle = this.buildings.Add(item);
		Extents extents = item.GetExtents();
		byte b = (byte)ElementLoader.elements.IndexOf(element);
		byte b2 = byte.MaxValue;
		Conduit component = item.GetComponent<Conduit>();
		if (component != null)
		{
			Vent component2 = component.GetComponent<Vent>();
			b2 = ((component2.transferType != Vent.Transfer.Gas) ? 1 : 0);
		}
		if (temperature <= 0f)
		{
			Output.LogWarning(new object[] { "Building temperature is <= 0K" });
		}
		SimMessages.BuildingHeatExchange(true, handle.index, extents, item.Mass, temperature, item.CurrentOperatingTemperature, b, b2, -1);
		while (handle.index >= this.meltingPoints.Count)
		{
			this.meltingPoints.Add(float.NaN);
			this.temperatures.Add(float.NaN);
			this.nearMeltingNotified.Add(false);
		}
		this.temperatures[handle.index] = temperature;
		Overheatable component3 = item.GetComponent<Overheatable>();
		float meltingPoint = component3.MeltingPoint;
		this.meltingPoints[handle.index] = meltingPoint;
		this.nearMeltingNotified[handle.index] = false;
		return handle;
	}

	public void Unregister(ref HandleVector<StructureTemperature>.Handle handle)
	{
		if (handle.index == -1)
		{
			return;
		}
		HandleVector<StructureTemperature>.Handle h = handle;
		HandleVector<global::System.Action>.Handle handle2 = Game.Instance.callbackManager.Add(delegate
		{
			this.buildings.Release(h);
		}, "BuildingTemperatureManager");
		Extents extents = this.buildings.GetItem(handle).GetExtents();
		SimMessages.BuildingHeatExchange(false, handle.index, extents, 0f, 0f, 0f, 0, byte.MaxValue, handle2.index);
		handle.index = -1;
	}

	public unsafe void UpdateTemperatures(int num_infos, Sim.BuildingTemperatureInfo* infos)
	{
		for (int i = 0; i < num_infos; i++)
		{
			Sim.BuildingTemperatureInfo buildingTemperatureInfo = infos[i];
			StructureTemperature structureTemperature = this.buildings.Items[buildingTemperatureInfo.id];
			if (structureTemperature != null)
			{
				Debug.Assert(!float.IsNaN(buildingTemperatureInfo.temperature));
				this.temperatures[buildingTemperatureInfo.id] = buildingTemperatureInfo.temperature;
			}
		}
		this.DoMeltingPointChecks();
	}

	public float GetTemperature(HandleVector<StructureTemperature>.Handle handle)
	{
		if (handle.index == -1)
		{
			return -1f;
		}
		return this.temperatures[handle.index];
	}

	private void DoMeltingPointChecks()
	{
		int num = Mathf.Min(this.temperatures.Count, 256);
		int num2 = Mathf.Min(num, this.temperatures.Count - this.checkOffset);
		int num3 = this.checkOffset + num2;
		this.DoMeltingPointChecks(this.checkOffset, num3);
		this.checkOffset = num3;
		num3 = num - num2;
		if (num3 > 0)
		{
			this.DoMeltingPointChecks(0, num3);
			this.checkOffset = num3;
		}
	}

	private void DoMeltingPointChecks(int idx, int end)
	{
		while (idx < end)
		{
			StructureTemperature structureTemperature = this.buildings.Items[idx];
			if (!(structureTemperature == null))
			{
				if (this.temperatures[idx] >= this.meltingPoints[idx])
				{
					structureTemperature.Trigger(1930836866, null);
				}
				else
				{
					bool flag = this.temperatures[idx] >= this.meltingPoints[idx] - 15f;
					if (flag != this.nearMeltingNotified[idx])
					{
						this.nearMeltingNotified[idx] = flag;
						structureTemperature.Trigger(-2009062694, flag);
					}
				}
			}
			idx++;
		}
	}

	private const int INITIAL_SIZE = 512;

	private HandleVector<StructureTemperature> buildings = new HandleVector<StructureTemperature>(512);

	private List<float> meltingPoints = new List<float>();

	private List<float> temperatures = new List<float>();

	private List<bool> nearMeltingNotified = new List<bool>();

	private int checkOffset;
}
