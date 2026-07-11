using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class RetiredColonyData
{
	public RetiredColonyData(string colonyName, int cycleCount, string date, string[] achievements, MinionAssignablesProxy[] minions, BuildingComplete[] buildingCompletes)
	{
		this.colonyName = colonyName;
		this.cycleCount = cycleCount;
		this.achievements = achievements;
		this.date = date;
		this.Duplicants = new RetiredColonyData.RetiredDuplicantData[(minions != null) ? minions.Length : 0];
		for (int i = 0; i < this.Duplicants.Length; i++)
		{
			this.Duplicants[i] = new RetiredColonyData.RetiredDuplicantData();
			this.Duplicants[i].name = minions[i].GetProperName();
			this.Duplicants[i].age = (int)Mathf.Floor((float)GameClock.Instance.GetCycle() - minions[i].GetArrivalTime());
			this.Duplicants[i].skillPointsGained = minions[i].GetTotalSkillpoints();
			this.Duplicants[i].accessories = new Dictionary<string, string>();
			if (minions[i].GetTargetGameObject().GetComponent<Accessorizer>() != null)
			{
				foreach (ResourceRef<Accessory> resourceRef in minions[i].GetTargetGameObject().GetComponent<Accessorizer>().GetAccessories())
				{
					if (resourceRef.Get() != null)
					{
						this.Duplicants[i].accessories.Add(resourceRef.Get().slot.Id, resourceRef.Get().Id);
					}
				}
			}
			else
			{
				StoredMinionIdentity component = minions[i].GetTargetGameObject().GetComponent<StoredMinionIdentity>();
				this.Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Eyes.Id, Db.Get().Accessories.Get(component.bodyData.eyes).Id);
				this.Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Arm.Id, Db.Get().Accessories.Get(component.bodyData.arms).Id);
				this.Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Body.Id, Db.Get().Accessories.Get(component.bodyData.body).Id);
				this.Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Hair.Id, Db.Get().Accessories.Get(component.bodyData.hair).Id);
				if (component.bodyData.hat != HashedString.Invalid)
				{
					this.Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Hat.Id, Db.Get().Accessories.Get(component.bodyData.hat).Id);
				}
				this.Duplicants[i].accessories.Add(Db.Get().AccessorySlots.HeadShape.Id, Db.Get().Accessories.Get(component.bodyData.headShape).Id);
				this.Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Mouth.Id, Db.Get().Accessories.Get(component.bodyData.mouth).Id);
			}
		}
		this.buildings = new List<Tuple<string, int>>();
		if (buildingCompletes != null)
		{
			for (int j = 0; j < buildingCompletes.Length; j++)
			{
				BuildingComplete b = buildingCompletes[j];
				int num = this.buildings.FindIndex((Tuple<string, int> match) => match.first == b.PrefabID());
				if (num == -1)
				{
					this.buildings.Add(new Tuple<string, int>(b.PrefabID().ToString(), 0));
					num = this.buildings.Count - 1;
				}
				this.buildings[num].second++;
			}
		}
		this.Stats = null;
		if (ReportManager.Instance != null)
		{
			Tuple<float, float>[] array = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int k = 0; k < array.Length; k++)
			{
				array[k] = new Tuple<float, float>((float)ReportManager.Instance.reports[k].day, ReportManager.Instance.reports[k].GetEntry(ReportManager.ReportType.OxygenCreated).accPositive);
			}
			Tuple<float, float>[] array2 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int l = 0; l < array2.Length; l++)
			{
				array2[l] = new Tuple<float, float>((float)ReportManager.Instance.reports[l].day, ReportManager.Instance.reports[l].GetEntry(ReportManager.ReportType.CaloriesCreated).accPositive * 0.001f);
			}
			Tuple<float, float>[] array3 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int m = 0; m < array3.Length; m++)
			{
				array3[m] = new Tuple<float, float>((float)ReportManager.Instance.reports[m].day, ReportManager.Instance.reports[m].GetEntry(ReportManager.ReportType.EnergyCreated).accPositive * 0.001f);
			}
			Tuple<float, float>[] array4 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int n = 0; n < array4.Length; n++)
			{
				array4[n] = new Tuple<float, float>((float)ReportManager.Instance.reports[n].day, ReportManager.Instance.reports[n].GetEntry(ReportManager.ReportType.EnergyWasted).accNegative * -1f * 0.001f);
			}
			Tuple<float, float>[] array5 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num2 = 0; num2 < array5.Length; num2++)
			{
				array5[num2] = new Tuple<float, float>((float)ReportManager.Instance.reports[num2].day, ReportManager.Instance.reports[num2].GetEntry(ReportManager.ReportType.WorkTime).accPositive);
			}
			Tuple<float, float>[] array6 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num3 = 0; num3 < array5.Length; num3++)
			{
				int num4 = 0;
				float num5 = 0f;
				ReportManager.ReportEntry entry = ReportManager.Instance.reports[num3].GetEntry(ReportManager.ReportType.WorkTime);
				for (int num6 = 0; num6 < entry.contextEntries.Count; num6++)
				{
					num4++;
					num5 += entry.contextEntries[num6].accPositive;
				}
				num5 /= (float)num4;
				num5 /= 600f;
				array6[num3] = new Tuple<float, float>((float)ReportManager.Instance.reports[num3].day, num5);
			}
			Tuple<float, float>[] array7 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num7 = 0; num7 < array7.Length; num7++)
			{
				array7[num7] = new Tuple<float, float>((float)ReportManager.Instance.reports[num7].day, ReportManager.Instance.reports[num7].GetEntry(ReportManager.ReportType.TravelTime).accPositive);
			}
			Tuple<float, float>[] array8 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num8 = 0; num8 < array7.Length; num8++)
			{
				int num9 = 0;
				float num10 = 0f;
				ReportManager.ReportEntry entry2 = ReportManager.Instance.reports[num8].GetEntry(ReportManager.ReportType.TravelTime);
				for (int num11 = 0; num11 < entry2.contextEntries.Count; num11++)
				{
					num9++;
					num10 += entry2.contextEntries[num11].accPositive;
				}
				num10 /= (float)num9;
				array8[num8] = new Tuple<float, float>((float)ReportManager.Instance.reports[num8].day, num10);
			}
			Tuple<float, float>[] array9 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num12 = 0; num12 < array5.Length; num12++)
			{
				array9[num12] = new Tuple<float, float>((float)ReportManager.Instance.reports[num12].day, (float)ReportManager.Instance.reports[num12].GetEntry(ReportManager.ReportType.WorkTime).contextEntries.Count);
			}
			Tuple<float, float>[] array10 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num13 = 0; num13 < array10.Length; num13++)
			{
				int num14 = 0;
				float num15 = 0f;
				ReportManager.ReportEntry entry3 = ReportManager.Instance.reports[num13].GetEntry(ReportManager.ReportType.StressDelta);
				for (int num16 = 0; num16 < entry3.contextEntries.Count; num16++)
				{
					num14++;
					num15 += entry3.contextEntries[num16].accPositive;
				}
				array10[num13] = new Tuple<float, float>((float)ReportManager.Instance.reports[num13].day, num15 / (float)num14);
			}
			Tuple<float, float>[] array11 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
			for (int num17 = 0; num17 < array11.Length; num17++)
			{
				int num18 = 0;
				float num19 = 0f;
				ReportManager.ReportEntry entry4 = ReportManager.Instance.reports[num17].GetEntry(ReportManager.ReportType.StressDelta);
				for (int num20 = 0; num20 < entry4.contextEntries.Count; num20++)
				{
					num18++;
					num19 += entry4.contextEntries[num20].accNegative;
				}
				num19 *= -1f;
				array11[num17] = new Tuple<float, float>((float)ReportManager.Instance.reports[num17].day, num19 / (float)num18);
			}
			this.Stats = new RetiredColonyData.RetiredColonyStatistic[]
			{
				new RetiredColonyData.RetiredColonyStatistic(RetiredColonyData.DataIDs.OxygenProduced, array, UI.RETIRED_COLONY_INFO_SCREEN.STATS.OXYGEN_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.MASS.KILOGRAM),
				new RetiredColonyData.RetiredColonyStatistic(RetiredColonyData.DataIDs.CaloriesProduced, array2, UI.RETIRED_COLONY_INFO_SCREEN.STATS.CALORIES_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.CALORIES.KILOCALORIE),
				new RetiredColonyData.RetiredColonyStatistic(RetiredColonyData.DataIDs.PowerProduced, array3, UI.RETIRED_COLONY_INFO_SCREEN.STATS.POWER_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE),
				new RetiredColonyData.RetiredColonyStatistic(RetiredColonyData.DataIDs.PowerWasted, array4, UI.RETIRED_COLONY_INFO_SCREEN.STATS.POWER_WASTED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE),
				new RetiredColonyData.RetiredColonyStatistic(RetiredColonyData.DataIDs.WorkTime, array5, UI.RETIRED_COLONY_INFO_SCREEN.STATS.WORK_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.SECONDS),
				new RetiredColonyData.RetiredColonyStatistic(RetiredColonyData.DataIDs.AverageWorkTime, array6, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_WORK_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
				new RetiredColonyData.RetiredColonyStatistic(RetiredColonyData.DataIDs.TravelTime, array7, UI.RETIRED_COLONY_INFO_SCREEN.STATS.TRAVEL_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.SECONDS),
				new RetiredColonyData.RetiredColonyStatistic(RetiredColonyData.DataIDs.AverageTravelTime, array8, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_TRAVEL_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
				new RetiredColonyData.RetiredColonyStatistic(RetiredColonyData.DataIDs.LiveDuplicants, array9, UI.RETIRED_COLONY_INFO_SCREEN.STATS.LIVE_DUPLICANTS, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.DUPLICANTS),
				new RetiredColonyData.RetiredColonyStatistic(RetiredColonyData.DataIDs.AverageStressCreated, array10, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_STRESS_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
				new RetiredColonyData.RetiredColonyStatistic(RetiredColonyData.DataIDs.AverageStressRemoved, array11, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_STRESS_REMOVED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT)
			};
		}
	}

	public string colonyName { get; set; }

	public int cycleCount { get; set; }

	public string date { get; set; }

	public string[] achievements { get; set; }

	public RetiredColonyData.RetiredDuplicantData[] Duplicants { get; set; }

	public List<Tuple<string, int>> buildings { get; set; }

	public RetiredColonyData.RetiredColonyStatistic[] Stats { get; set; }

	public static class DataIDs
	{
		public static string OxygenProduced = "oxygenProduced";

		public static string CaloriesProduced = "caloriesProduced";

		public static string PowerProduced = "powerProduced";

		public static string PowerWasted = "powerWasted";

		public static string WorkTime = "workTime";

		public static string TravelTime = "travelTime";

		public static string AverageWorkTime = "averageWorkTime";

		public static string AverageTravelTime = "averageTravelTime";

		public static string LiveDuplicants = "liveDuplicants";

		public static string AverageStressCreated = "averageStressCreated";

		public static string AverageStressRemoved = "averageStressRemoved";

		public static string AverageGerms = "averageGerms";
	}

	public class RetiredColonyStatistic
	{
		public RetiredColonyStatistic(string id, Tuple<float, float>[] data, string name, string axisNameX, string axisNameY)
		{
			this.id = id;
			this.value = data;
			this.name = name;
			this.nameX = axisNameX;
			this.nameY = axisNameY;
		}

		public Tuple<float, float> GetByMaxValue()
		{
			if (this.value.Length == 0)
			{
				return new Tuple<float, float>(0f, 0f);
			}
			int num = -1;
			float num2 = -1f;
			for (int i = 0; i < this.value.Length; i++)
			{
				if (this.value[i].second > num2)
				{
					num2 = this.value[i].second;
					num = i;
				}
			}
			if (num == -1)
			{
				num = 0;
			}
			return this.value[num];
		}

		public Tuple<float, float> GetByMaxKey()
		{
			if (this.value.Length == 0)
			{
				return new Tuple<float, float>(0f, 0f);
			}
			int num = -1;
			float num2 = -1f;
			for (int i = 0; i < this.value.Length; i++)
			{
				if (this.value[i].first > num2)
				{
					num2 = this.value[i].first;
					num = i;
				}
			}
			return this.value[num];
		}

		public string id;

		public Tuple<float, float>[] value;

		public string name;

		public string nameX;

		public string nameY;
	}

	public class RetiredDuplicantData
	{
		public string name;

		public int age;

		public int skillPointsGained;

		public Dictionary<string, string> accessories;
	}
}
