using System;
using System.Diagnostics;
using Klei.AI;
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
			float num;
			if (mass <= 0f)
			{
				num = 0f;
			}
			else
			{
				Element element = Grid.Element[cell];
				if (element.IsVacuum)
				{
					num = 0f;
				}
				else
				{
					float num2 = Grid.Temperature[cell];
					float thermalConductivity = element.thermalConductivity;
					float num3 = SimUtil.CalculateEnergyFlow(num2, thermalConductivity, dest_temp, dest_thermal_conductivity, surface_area, thickness);
					num = num3 * 0.001f;
				}
			}
			return num;
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
			float num;
			if (kilojoules * specific_heat_capacity * mass == 0f)
			{
				num = 0f;
			}
			else
			{
				num = kilojoules / (specific_heat_capacity * mass);
			}
			return num;
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

		public static SimUtil.DiseaseInfo CalculateFinalDiseaseInfo(SimUtil.DiseaseInfo a, SimUtil.DiseaseInfo b)
		{
			return SimUtil.CalculateFinalDiseaseInfo(a.idx, a.count, b.idx, b.count);
		}

		public static SimUtil.DiseaseInfo CalculateFinalDiseaseInfo(byte src1_idx, int src1_count, byte src2_idx, int src2_count)
		{
			SimUtil.DiseaseInfo diseaseInfo = default(SimUtil.DiseaseInfo);
			if (src1_idx == src2_idx)
			{
				diseaseInfo.idx = src1_idx;
				diseaseInfo.count = src1_count + src2_count;
			}
			else if (src1_idx == 255)
			{
				diseaseInfo.idx = src2_idx;
				diseaseInfo.count = src2_count;
			}
			else if (src2_idx == 255)
			{
				diseaseInfo.idx = src1_idx;
				diseaseInfo.count = src1_count;
			}
			else
			{
				Disease disease = Db.Get().Diseases[(int)src1_idx];
				Disease disease2 = Db.Get().Diseases[(int)src2_idx];
				float num = disease.strength * (float)src1_count;
				float num2 = disease2.strength * (float)src2_count;
				if (num > num2)
				{
					int num3 = (int)((float)src2_count - num / num2 * (float)src1_count);
					if (num3 < 0)
					{
						diseaseInfo.idx = src1_idx;
						diseaseInfo.count = -num3;
					}
					else
					{
						diseaseInfo.idx = src2_idx;
						diseaseInfo.count = num3;
					}
				}
				else
				{
					int num4 = (int)((float)src1_count - num2 / num * (float)src2_count);
					if (num4 < 0)
					{
						diseaseInfo.idx = src2_idx;
						diseaseInfo.count = -num4;
					}
					else
					{
						diseaseInfo.idx = src1_idx;
						diseaseInfo.count = num4;
					}
				}
			}
			if (diseaseInfo.count <= 0)
			{
				diseaseInfo.count = 0;
				diseaseInfo.idx = byte.MaxValue;
			}
			return diseaseInfo;
		}

		public static byte DiseaseCountToAlpha254(int count)
		{
			float num = Mathf.Log((float)count, 10f);
			num /= SimUtil.MAX_DISEASE_LOG_RANGE;
			num = Math.Max(0f, Math.Min(1f, num));
			num -= SimUtil.MIN_DISEASE_LOG_SUBTRACTION / SimUtil.MAX_DISEASE_LOG_RANGE;
			num = Math.Max(0f, num);
			num /= 1f - SimUtil.MIN_DISEASE_LOG_SUBTRACTION / SimUtil.MAX_DISEASE_LOG_RANGE;
			return (byte)(num * 254f);
		}

		public static float DiseaseCountToAlpha(int count)
		{
			return (float)SimUtil.DiseaseCountToAlpha254(count) / 255f;
		}

		public static SimUtil.DiseaseInfo GetPercentOfDisease(PrimaryElement pe, float percent)
		{
			return new SimUtil.DiseaseInfo
			{
				idx = pe.DiseaseIdx,
				count = (int)((float)pe.DiseaseCount * percent)
			};
		}

		private const int MAX_ALPHA_COUNT = 1000000;

		private static float MIN_DISEASE_LOG_SUBTRACTION = 2f;

		private static float MAX_DISEASE_LOG_RANGE = 6f;

		public struct DiseaseInfo
		{
			public byte idx;

			public int count;

			public static readonly SimUtil.DiseaseInfo Invalid = new SimUtil.DiseaseInfo
			{
				idx = byte.MaxValue,
				count = 0
			};
		}
	}
}
