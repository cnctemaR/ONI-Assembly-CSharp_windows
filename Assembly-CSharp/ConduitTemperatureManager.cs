using System;
using Klei;

public class ConduitTemperatureManager : KCompactedVector<ConduitTemperatureManager.Data>
{
	public ConduitTemperatureManager()
		: base(0)
	{
	}

	public void Clear()
	{
		this.data.Clear();
		this.handles.Clear();
	}

	public HandleVector<int>.Handle Allocate(int cell, ref ConduitFlow.ConduitContents contents)
	{
		ConduitTemperatureManager.Data data = this.CreateDataItem(cell, ref contents);
		return base.Allocate(data);
	}

	public void SetData(HandleVector<int>.Handle handle, int cell, ref ConduitFlow.ConduitContents contents)
	{
		ConduitTemperatureManager.Data data = this.CreateDataItem(cell, ref contents);
		base.SetData(handle, data);
	}

	private ConduitTemperatureManager.Data CreateDataItem(int cell, ref ConduitFlow.ConduitContents contents)
	{
		Element element = ElementLoader.FindElementByHash(contents.element);
		return new ConduitTemperatureManager.Data
		{
			cell = cell,
			temperature = contents.temperature,
			thermalConductivity = element.thermalConductivity,
			heatCapacity = contents.mass * element.specificHeatCapacity
		};
	}

	public void SimUpdate(float dt)
	{
		if (ConduitTemperatureManager.ScaleFactor <= 0f)
		{
			return;
		}
		for (int i = 0; i < this.data.Count; i++)
		{
			ConduitTemperatureManager.Data data = this.data[i];
			if (data.heatCapacity != 0f)
			{
				Sim.Cell cell = Grid.Cell[data.cell];
				Element element = Grid.Element[data.cell];
				float num = cell.mass * element.specificHeatCapacity;
				float num2 = SimUtil.CalculateEnergyFlow(cell.temperature, num, element.thermalConductivity, data.temperature, data.heatCapacity, data.thermalConductivity, ConduitTemperatureManager.ScaleFactor, 1f);
				float num3 = SimUtil.ClampEnergyTransfer(dt, cell.temperature, num, data.temperature, data.heatCapacity, num2);
				float num4 = this.ModifyTemperature(data.temperature, data.heatCapacity, cell.temperature, num, ref num3);
				data.temperature = num4;
				this.data[i] = data;
				if (num3 != 0f)
				{
					SimMessages.ModifyEnergy(data.cell, -num3, SimMessages.EnergySourceID.ConduitTemperatureManager);
				}
			}
		}
	}

	private float ModifyTemperature(float source_temperature, float source_heat_capacity, float cell_temperature, float cell_heat_capacity, ref float kilojoules)
	{
		if (source_heat_capacity * cell_heat_capacity <= 0f)
		{
			kilojoules = 0f;
			return source_temperature;
		}
		float num = source_temperature;
		float num2 = Math.Max(0f, num + kilojoules / source_heat_capacity);
		if (float.IsInfinity(num2) || float.IsNaN(num2))
		{
			Output.LogError(new object[] { "Invalid temperature" });
			kilojoules = 0f;
			return source_temperature;
		}
		source_temperature = num2;
		float num3 = Math.Max(0f, cell_temperature - kilojoules / cell_heat_capacity);
		if ((num - cell_temperature) * (source_temperature - num3) < 0f)
		{
			float num4 = num * source_heat_capacity + cell_temperature * cell_heat_capacity;
			float num5 = num4 / (source_heat_capacity + cell_heat_capacity);
			source_temperature = num5;
			kilojoules = (num5 - cell_temperature) * cell_heat_capacity;
		}
		return source_temperature;
	}

	private static float ScaleFactor;

	public struct Data
	{
		public int cell;

		public float temperature;

		public float thermalConductivity;

		public float heatCapacity;
	}
}
