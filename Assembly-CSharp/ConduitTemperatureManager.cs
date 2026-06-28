using System;
using Klei;

public class ConduitTemperatureManager : KCompactedVector<ConduitTemperatureManager.Data>
{
	public ConduitTemperatureManager(float contents_surface_area)
		: base(0)
	{
		this.contentsSurfaceArea = contents_surface_area;
	}

	public HandleVector<int>.Handle Allocate(HandleVector<int>.Handle conduit_structure_temperature_handle, ref ConduitFlow.ConduitContents contents)
	{
		ConduitTemperatureManager.Data data = this.CreateDataItem(conduit_structure_temperature_handle, ref contents);
		return base.Allocate(data);
	}

	public void SetData(HandleVector<int>.Handle handle, HandleVector<int>.Handle conduit_structure_temperature_handle, ref ConduitFlow.ConduitContents contents)
	{
		ConduitTemperatureManager.Data data = this.CreateDataItem(conduit_structure_temperature_handle, ref contents);
		base.SetData(handle, data);
	}

	private ConduitTemperatureManager.Data CreateDataItem(HandleVector<int>.Handle conduit_structure_temperature_handle, ref ConduitFlow.ConduitContents contents)
	{
		Element element = ElementLoader.FindElementByHash(contents.element);
		ConduitTemperatureManager.Data data = default(ConduitTemperatureManager.Data);
		data.temperature = contents.temperature;
		data.thermalConductivity = element.thermalConductivity;
		data.heatCapacity = contents.mass * element.specificHeatCapacity;
		data.conduitStructureTemperatureHandle = conduit_structure_temperature_handle;
		StructureTemperatureData data2 = GameComps.StructureTemperatures.GetData(conduit_structure_temperature_handle);
		Element element2 = data2.primaryElement.Element;
		data.conduitHeatCapacity = data2.building.Def.MassForTemperatureModification * element2.specificHeatCapacity;
		data.conduitThermalConductivity = element2.thermalConductivity;
		data.lowStateTransitionTemperature = ((element.lowTempTransition == null) ? 0f : (element.lowTemp - 3f));
		data.highStateTransitionTemperature = ((element.highTempTransition == null) ? float.PositiveInfinity : (element.highTemp + 3f));
		return data;
	}

	public void SimUpdate(float dt)
	{
		for (int i = 0; i < this.data.Count; i++)
		{
			ConduitTemperatureManager.Data data = this.data[i];
			if (data.heatCapacity != 0f)
			{
				StructureTemperatureData data2 = GameComps.StructureTemperatures.GetData(data.conduitStructureTemperatureHandle);
				float temperature = data.temperature;
				float temperature2 = data2.Temperature;
				float num = SimUtil.CalculateEnergyFlow(temperature, data.thermalConductivity, temperature2, data.conduitThermalConductivity, this.contentsSurfaceArea * ConduitTemperatureManager.ContentsScaleFactor, 1f);
				float num2 = SimUtil.ClampEnergyTransfer(dt, temperature, data.heatCapacity, temperature2, data.conduitHeatCapacity, num);
				float num3 = -num2;
				float num4 = this.ModifyTemperature(data.temperature, data.heatCapacity, data2.Temperature, data.conduitHeatCapacity, ref num3);
				data.temperature = num4;
				this.data[i] = data;
				if (num4 < data.lowStateTransitionTemperature)
				{
					data2.primaryElement.Trigger(-700727624, null);
				}
				else if (num4 > data.highStateTransitionTemperature)
				{
					data2.primaryElement.Trigger(-1152799878, null);
				}
				data2.ModifyEnergy(num2);
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

	private static float ContentsScaleFactor = 50f;

	private float contentsSurfaceArea;

	public struct Data
	{
		public int cell;

		public float temperature;

		public float thermalConductivity;

		public float heatCapacity;

		public HandleVector<int>.Handle conduitStructureTemperatureHandle;

		public float conduitHeatCapacity;

		public float conduitThermalConductivity;

		public float lowStateTransitionTemperature;

		public float highStateTransitionTemperature;
	}
}
