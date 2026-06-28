using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;

namespace Klei
{
	public static class SimUtil
	{
		public static float CalculateEnergyFlow(float source_temp, float source_thermal_conductivity, float dest_temp, float dest_thermal_conductivity, float surface_area = 1f, float thickness = 1f)
		{
			float num = source_temp - dest_temp;
			return num * Math.Min(source_thermal_conductivity, dest_thermal_conductivity) * (surface_area / thickness);
		}

		public static float CalculateEnergyFlow(int cell, float dest_temp, float dest_specific_heat_capacity, float dest_thermal_conductivity, float surface_area = 1f, float thickness = 1f)
		{
			float mass = Grid.Cell[cell].mass;
			if (mass <= 0f)
			{
				return 0f;
			}
			Element element = Grid.Element[cell];
			if (element.IsVacuum)
			{
				return 0f;
			}
			float num = Grid.Temperature[cell];
			float thermalConductivity = element.thermalConductivity;
			float num2 = SimUtil.CalculateEnergyFlow(num, thermalConductivity, dest_temp, dest_thermal_conductivity, surface_area, thickness);
			return num2 * 0.001f;
		}

		public static float ClampEnergyTransfer(float dt, float source_temp, float source_mass, float source_specific_heat_capacity, float dest_temp, float dest_mass, float dest_specific_heat_capacity, float max_watts_transferred)
		{
			return SimUtil.ClampEnergyTransfer(dt, source_temp, source_mass * source_specific_heat_capacity, dest_temp, dest_mass * dest_specific_heat_capacity, max_watts_transferred);
		}

		public static float ClampEnergyTransfer(float dt, float source_temp, float source_heat_capacity, float dest_temp, float dest_heat_capacity, float max_watts_transferred)
		{
			float num = max_watts_transferred * dt / 1000f;
			SimUtil.CheckValidValue(num);
			float num2 = Math.Min(source_temp, dest_temp);
			float num3 = Math.Max(source_temp, dest_temp);
			float num4 = source_temp - num / source_heat_capacity;
			float num5 = dest_temp + num / dest_heat_capacity;
			SimUtil.CheckValidValue(num4);
			SimUtil.CheckValidValue(num5);
			num4 = Mathf.Clamp(num4, num2, num3);
			num5 = Mathf.Clamp(num5, num2, num3);
			float num6 = Math.Abs(num4 - source_temp);
			float num7 = Math.Abs(num5 - dest_temp);
			float num8 = num6 * source_heat_capacity;
			float num9 = num7 * dest_heat_capacity;
			float num10 = ((max_watts_transferred >= 0f) ? 1f : (-1f));
			float num11 = Math.Min(num8, num9) * num10;
			SimUtil.CheckValidValue(num11);
			return num11;
		}

		private static float GetMassAreaScale(Element element)
		{
			return (!element.IsGas) ? 0.01f : 10f;
		}

		public static float CalculateEnergyFlowCreatures(int cell, float creature_temperature, float creature_shc, float creature_thermal_conductivity, float creature_surface_area = 1f, float creature_surface_thickness = 1f)
		{
			return SimUtil.CalculateEnergyFlow(cell, creature_temperature, creature_shc, creature_thermal_conductivity, creature_surface_area, creature_surface_thickness);
		}

		public static float EnergyFlowToTemperatureDelta(float kilojoules, float specific_heat_capacity, float mass)
		{
			if (kilojoules * specific_heat_capacity * mass == 0f)
			{
				return 0f;
			}
			return kilojoules / (specific_heat_capacity * mass);
		}

		public static float CalculateFinalTemperature(float mass1, float temp1, float mass2, float temp2)
		{
			float num = mass1 * temp1;
			float num2 = mass2 * temp2;
			float num3 = num + num2;
			float num4 = num3 / (mass1 + mass2);
			float num5;
			float num6;
			if (temp1 > temp2)
			{
				num5 = temp2;
				num6 = temp1;
			}
			else
			{
				num5 = temp1;
				num6 = temp2;
			}
			return Math.Max(num5, Math.Min(num6, num4));
		}

		[Conditional("STRICT_CHECKING")]
		public static void CheckValidValue(float value)
		{
			if (float.IsNaN(value) || float.IsInfinity(value))
			{
				Assert.IsTrue(false);
			}
		}
	}
}
