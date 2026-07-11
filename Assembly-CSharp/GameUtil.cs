using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class GameUtil
{
	public static string GetTemperatureUnitSuffix()
	{
		GameUtil.TemperatureUnit temperatureUnit = GameUtil.temperatureUnit;
		string text;
		if (temperatureUnit != GameUtil.TemperatureUnit.Celsius)
		{
			if (temperatureUnit != GameUtil.TemperatureUnit.Fahrenheit)
			{
				text = UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
			}
			else
			{
				text = UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
			}
		}
		else
		{
			text = UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
		}
		return text;
	}

	private static string AddTemperatureUnitSuffix(string text)
	{
		return text + GameUtil.GetTemperatureUnitSuffix();
	}

	public static float GetTemperatureConvertedFromKelvin(float temperature, GameUtil.TemperatureUnit targetUnit)
	{
		if (targetUnit == GameUtil.TemperatureUnit.Celsius)
		{
			return temperature - 273.15f;
		}
		if (targetUnit != GameUtil.TemperatureUnit.Fahrenheit)
		{
			return temperature;
		}
		return temperature * 1.8f - 459.67f;
	}

	public static float GetConvertedTemperature(float temperature, bool roundOutput = false)
	{
		GameUtil.TemperatureUnit temperatureUnit = GameUtil.temperatureUnit;
		float num;
		if (temperatureUnit == GameUtil.TemperatureUnit.Celsius)
		{
			num = temperature - 273.15f;
			return (!roundOutput) ? num : Mathf.Round(num);
		}
		if (temperatureUnit != GameUtil.TemperatureUnit.Fahrenheit)
		{
			return (!roundOutput) ? temperature : Mathf.Round(temperature);
		}
		num = temperature * 1.8f - 459.67f;
		return (!roundOutput) ? num : Mathf.Round(num);
	}

	public static float GetTemperatureConvertedToKelvin(float temperature, GameUtil.TemperatureUnit fromUnit)
	{
		if (fromUnit == GameUtil.TemperatureUnit.Celsius)
		{
			return temperature + 273.15f;
		}
		if (fromUnit != GameUtil.TemperatureUnit.Fahrenheit)
		{
			return temperature;
		}
		return (temperature + 459.67f) * 5f / 9f;
	}

	public static float GetTemperatureConvertedToKelvin(float temperature)
	{
		GameUtil.TemperatureUnit temperatureUnit = GameUtil.temperatureUnit;
		if (temperatureUnit == GameUtil.TemperatureUnit.Celsius)
		{
			return temperature + 273.15f;
		}
		if (temperatureUnit != GameUtil.TemperatureUnit.Fahrenheit)
		{
			return temperature;
		}
		return (temperature + 459.67f) * 5f / 9f;
	}

	private static float GetConvertedTemperatureDelta(float kelvin_delta)
	{
		switch (GameUtil.temperatureUnit)
		{
		case GameUtil.TemperatureUnit.Celsius:
			return kelvin_delta;
		case GameUtil.TemperatureUnit.Fahrenheit:
			return kelvin_delta * 1.8f;
		case GameUtil.TemperatureUnit.Kelvin:
			return kelvin_delta;
		default:
			return kelvin_delta;
		}
	}

	public static float ApplyTimeSlice(float val, GameUtil.TimeSlice timeSlice)
	{
		if (timeSlice == GameUtil.TimeSlice.PerCycle)
		{
			return val * 600f;
		}
		return val;
	}

	public static string AddTimeSliceText(string text, GameUtil.TimeSlice timeSlice)
	{
		switch (timeSlice)
		{
		case GameUtil.TimeSlice.PerSecond:
			return text + UI.UNITSUFFIXES.PERSECOND;
		case GameUtil.TimeSlice.PerCycle:
			return text + UI.UNITSUFFIXES.PERCYCLE;
		}
		return text;
	}

	public static string AddPositiveSign(string text, bool positive)
	{
		if (positive)
		{
			return string.Format(UI.POSITIVE_FORMAT, text);
		}
		return text;
	}

	public static float AttributeSkillToAlpha(AttributeInstance attributeInstance)
	{
		return Mathf.Min(attributeInstance.GetTotalValue() / 10f, 1f);
	}

	public static float AttributeSkillToAlpha(float attributeSkill)
	{
		return Mathf.Min(attributeSkill / 10f, 1f);
	}

	public static float AptitudeToAlpha(float aptitude)
	{
		return Mathf.Min(aptitude / 10f, 1f);
	}

	public static float GetThermalEnergy(PrimaryElement pe)
	{
		return pe.Temperature * pe.Mass * pe.Element.specificHeatCapacity;
	}

	public static float CalculateTemperatureChange(float shc, float mass, float kilowatts)
	{
		return kilowatts / (shc * mass);
	}

	public static void DeltaThermalEnergy(PrimaryElement pe, float kilowatts, float targetTemperature)
	{
		float num = GameUtil.CalculateTemperatureChange(pe.Element.specificHeatCapacity, pe.Mass, kilowatts);
		float num2 = pe.Temperature + num;
		if (targetTemperature > pe.Temperature)
		{
			num2 = Mathf.Clamp(num2, pe.Temperature, targetTemperature);
		}
		else
		{
			num2 = Mathf.Clamp(num2, targetTemperature, pe.Temperature);
		}
		pe.Temperature = num2;
	}

	public static BindingEntry ActionToBinding(global::Action action)
	{
		foreach (BindingEntry bindingEntry in GameInputMapping.KeyBindings)
		{
			if (bindingEntry.mAction == action)
			{
				return bindingEntry;
			}
		}
		throw new ArgumentException(action.ToString() + " is not bound in GameInputBindings");
	}

	public static string GetIdentityDescriptor(GameObject go)
	{
		if (go.GetComponent<MinionIdentity>())
		{
			return DUPLICANTS.STATS.SUBJECTS.DUPLICANT;
		}
		if (go.GetComponent<CreatureBrain>())
		{
			return DUPLICANTS.STATS.SUBJECTS.CREATURE;
		}
		return DUPLICANTS.STATS.SUBJECTS.PLANT;
	}

	public static float GetEnergyInPrimaryElement(PrimaryElement element)
	{
		return 0.001f * (element.Temperature * (element.Mass * 1000f * element.Element.specificHeatCapacity));
	}

	public static float EnergyToTemperatureDelta(float kilojoules, PrimaryElement element)
	{
		global::Debug.Assert(element.Mass > 0f);
		float energyInPrimaryElement = GameUtil.GetEnergyInPrimaryElement(element);
		float num = Mathf.Max(energyInPrimaryElement - kilojoules, 1f);
		float temperature = element.Temperature;
		float num2 = num / (0.001f * (element.Mass * (element.Element.specificHeatCapacity * 1000f)));
		return num2 - temperature;
	}

	public static float CalculateEnergyDeltaForElement(PrimaryElement element, float startTemp, float endTemp)
	{
		return GameUtil.CalculateEnergyDeltaForElementChange(element.Mass, element.Element.specificHeatCapacity, startTemp, endTemp);
	}

	public static float CalculateEnergyDeltaForElementChange(float mass, float shc, float startTemp, float endTemp)
	{
		float num = endTemp - startTemp;
		return num * mass * shc;
	}

	public static float GetFinalTemperature(float t1, float m1, float t2, float m2)
	{
		float num = m1 + m2;
		float num2 = t1 * m1 + t2 * m2;
		float num3 = num2 / num;
		float num4 = Mathf.Min(t1, t2);
		float num5 = Mathf.Max(t1, t2);
		num3 = Mathf.Clamp(num3, num4, num5);
		if (float.IsNaN(num3) || float.IsInfinity(num3))
		{
			global::Debug.LogError(string.Format("Calculated an invalid temperature: t1={0}, m1={1}, t2={2}, m2={3}, min_temp={4}, max_temp={5}", new object[] { t1, m1, t2, m2, num4, num5 }));
		}
		return num3;
	}

	public static void ForceTotalConduction(PrimaryElement a, PrimaryElement b)
	{
		float num = a.Temperature * a.Element.specificHeatCapacity * a.Mass;
		float temperature = a.Temperature;
		float num2 = b.Temperature * b.Element.specificHeatCapacity * b.Mass;
		float temperature2 = b.Temperature;
		float num3 = num2 / (num + num2);
		a.Temperature = (temperature2 - temperature) * num3 + temperature;
		b.Temperature = (temperature - temperature2) * 1f - num3 + temperature2;
	}

	public static string FloatToString(float f, string format = null)
	{
		if (float.IsPositiveInfinity(f))
		{
			return UI.POS_INFINITY;
		}
		if (float.IsNegativeInfinity(f))
		{
			return UI.NEG_INFINITY;
		}
		return f.ToString(format);
	}

	public static string GetUnitFormattedName(GameObject go, bool upperName = false)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		if (component != null && Assets.IsTagCountable(component.PrefabTag))
		{
			PrimaryElement component2 = go.GetComponent<PrimaryElement>();
			return GameUtil.GetUnitFormattedName(go.GetProperName(), component2.Units, upperName);
		}
		return (!upperName) ? go.GetProperName() : StringFormatter.ToUpper(go.GetProperName());
	}

	public static string GetUnitFormattedName(string name, float count, bool upperName = false)
	{
		if (upperName)
		{
			name = name.ToUpper();
		}
		return StringFormatter.Replace(UI.NAME_WITH_UNITS, "{0}", name).Replace("{1}", string.Format("{0:0.##}", count));
	}

	public static string GetFormattedUnits(float units, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool displaySuffix = true)
	{
		string text = ((units != 1f) ? UI.UNITSUFFIXES.UNITS : UI.UNITSUFFIXES.UNIT);
		units = GameUtil.ApplyTimeSlice(units, timeSlice);
		string text2 = string.Empty;
		if (units == 0f)
		{
			text2 = "0";
		}
		else if (Mathf.Abs(units) < 1f)
		{
			text2 = GameUtil.FloatToString(units, "#,##0.#");
		}
		else if (Mathf.Abs(units) < 10f)
		{
			text2 = GameUtil.FloatToString(units, "#,###.#");
		}
		else
		{
			text2 = GameUtil.FloatToString(units, "#,###");
		}
		if (displaySuffix)
		{
			text2 += text;
		}
		return GameUtil.AddTimeSliceText(text2, timeSlice);
	}

	public static string ApplyBoldString(string source)
	{
		return "<b>" + source + "</b>";
	}

	public static float GetRoundedTemperatureInKelvin(float kelvin)
	{
		float num = 0f;
		GameUtil.TemperatureUnit temperatureUnit = GameUtil.temperatureUnit;
		if (temperatureUnit != GameUtil.TemperatureUnit.Celsius)
		{
			if (temperatureUnit != GameUtil.TemperatureUnit.Fahrenheit)
			{
				if (temperatureUnit == GameUtil.TemperatureUnit.Kelvin)
				{
					num = (float)Mathf.RoundToInt(kelvin);
				}
			}
			else
			{
				float num2 = (float)Mathf.RoundToInt(GameUtil.GetTemperatureConvertedFromKelvin(kelvin, GameUtil.TemperatureUnit.Fahrenheit));
				num = GameUtil.GetTemperatureConvertedToKelvin(num2, GameUtil.TemperatureUnit.Fahrenheit);
			}
		}
		else
		{
			num = GameUtil.GetTemperatureConvertedToKelvin(Mathf.Round(GameUtil.GetConvertedTemperature(Mathf.Round(kelvin), true)));
		}
		return num;
	}

	public static string GetFormattedTemperature(float temp, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation interpretation = GameUtil.TemperatureInterpretation.Absolute, bool displayUnits = true, bool roundInDestinationFormat = false)
	{
		if (interpretation != GameUtil.TemperatureInterpretation.Absolute)
		{
			if (interpretation != GameUtil.TemperatureInterpretation.Relative)
			{
			}
			temp = GameUtil.GetConvertedTemperatureDelta(temp);
		}
		else
		{
			temp = GameUtil.GetConvertedTemperature(temp, roundInDestinationFormat);
		}
		temp = GameUtil.ApplyTimeSlice(temp, timeSlice);
		string text = string.Empty;
		if (Mathf.Abs(temp) < 0.1f)
		{
			text = GameUtil.FloatToString(temp, "##0.####");
		}
		else
		{
			text = GameUtil.FloatToString(temp, "##0.#");
		}
		if (displayUnits)
		{
			text = GameUtil.AddTemperatureUnitSuffix(text);
		}
		return GameUtil.AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedCaloriesForItem(Tag tag, float amount, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool forceKcal = true)
	{
		EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(tag.Name);
		return GameUtil.GetFormattedCalories((foodInfo == null) ? (-1f) : (foodInfo.CaloriesPerUnit * amount), timeSlice, forceKcal);
	}

	public static string GetFormattedCalories(float calories, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool forceKcal = true)
	{
		string text = UI.UNITSUFFIXES.CALORIES.CALORIE;
		if (Mathf.Abs(calories) >= 1000f || forceKcal)
		{
			calories /= 1000f;
			text = UI.UNITSUFFIXES.CALORIES.KILOCALORIE;
		}
		calories = GameUtil.ApplyTimeSlice(calories, timeSlice);
		string text2 = string.Empty;
		if (calories == 0f)
		{
			text2 = "0" + text;
		}
		else if (Mathf.Abs(calories) < 1f)
		{
			text2 = GameUtil.FloatToString(calories, "#,##0.#") + text;
		}
		else if (Mathf.Abs(calories) < 10f)
		{
			text2 = GameUtil.FloatToString(calories, "#,###.#") + text;
		}
		else
		{
			text2 = GameUtil.FloatToString(calories, "#,###") + text;
		}
		return GameUtil.AddTimeSliceText(text2, timeSlice);
	}

	public static string GetFormattedPlantGrowth(float percent, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		percent = GameUtil.ApplyTimeSlice(percent, timeSlice);
		string text = string.Empty;
		if (Mathf.Abs(percent) == 0f)
		{
			text = "0";
		}
		else if (Mathf.Abs(percent) < 0.1f)
		{
			text = "##0.##";
		}
		else if (Mathf.Abs(percent) < 1f)
		{
			text = "##0.#";
		}
		else
		{
			text = "##0";
		}
		string text2 = GameUtil.FloatToString(percent, text) + UI.UNITSUFFIXES.PERCENT + " " + UI.UNITSUFFIXES.GROWTH;
		return GameUtil.AddTimeSliceText(text2, timeSlice);
	}

	public static string GetFormattedPercent(float percent, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		percent = GameUtil.ApplyTimeSlice(percent, timeSlice);
		string text = string.Empty;
		if (Mathf.Abs(percent) == 0f)
		{
			text = "0";
		}
		else if (Mathf.Abs(percent) < 0.1f)
		{
			text = "##0.##";
		}
		else if (Mathf.Abs(percent) < 1f)
		{
			text = "##0.#";
		}
		else
		{
			text = "##0";
		}
		string text2 = GameUtil.FloatToString(percent, text) + UI.UNITSUFFIXES.PERCENT;
		return GameUtil.AddTimeSliceText(text2, timeSlice);
	}

	public static string GetFormattedRoundedJoules(float joules)
	{
		if (Mathf.Abs(joules) > 1000f)
		{
			return GameUtil.FloatToString(joules / 1000f, "F1") + UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE;
		}
		return GameUtil.FloatToString(joules, "F1") + UI.UNITSUFFIXES.ELECTRICAL.JOULE;
	}

	public static string GetFormattedJoules(float joules, string floatFormat = "F1", GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		joules = GameUtil.ApplyTimeSlice(joules, timeSlice);
		string text;
		if (Math.Abs(joules) > 1000000f)
		{
			text = GameUtil.FloatToString(joules / 1000000f, floatFormat) + UI.UNITSUFFIXES.ELECTRICAL.MEGAJOULE;
		}
		else if (Mathf.Abs(joules) > 1000f)
		{
			text = GameUtil.FloatToString(joules / 1000f, floatFormat) + UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE;
		}
		else
		{
			text = GameUtil.FloatToString(joules, floatFormat) + UI.UNITSUFFIXES.ELECTRICAL.JOULE;
		}
		return GameUtil.AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedWattage(float watts, GameUtil.WattageFormatterUnit unit = GameUtil.WattageFormatterUnit.Automatic)
	{
		LocString locString = string.Empty;
		if (unit != GameUtil.WattageFormatterUnit.Automatic)
		{
			if (unit != GameUtil.WattageFormatterUnit.Kilowatts)
			{
				if (unit == GameUtil.WattageFormatterUnit.Watts)
				{
					locString = UI.UNITSUFFIXES.ELECTRICAL.WATT;
				}
			}
			else
			{
				watts /= 1000f;
				locString = UI.UNITSUFFIXES.ELECTRICAL.KILOWATT;
			}
		}
		else if (Mathf.Abs(watts) > 1000f)
		{
			watts /= 1000f;
			locString = UI.UNITSUFFIXES.ELECTRICAL.KILOWATT;
		}
		else
		{
			locString = UI.UNITSUFFIXES.ELECTRICAL.WATT;
		}
		return GameUtil.FloatToString(watts, "###0.##") + locString;
	}

	public static string GetFormattedHeatEnergy(float dtu, GameUtil.HeatEnergyFormatterUnit unit = GameUtil.HeatEnergyFormatterUnit.Automatic)
	{
		LocString locString = string.Empty;
		string text;
		switch (unit)
		{
		case GameUtil.HeatEnergyFormatterUnit.DTU_S:
			locString = UI.UNITSUFFIXES.HEAT.DTU;
			text = "###0.";
			break;
		case GameUtil.HeatEnergyFormatterUnit.KDTU_S:
			dtu /= 1000f;
			locString = UI.UNITSUFFIXES.HEAT.KDTU;
			text = "###0.##";
			break;
		default:
			if (Mathf.Abs(dtu) > 1000f)
			{
				dtu /= 1000f;
				locString = UI.UNITSUFFIXES.HEAT.KDTU;
				text = "###0.##";
			}
			else
			{
				locString = UI.UNITSUFFIXES.HEAT.DTU;
				text = "###0.";
			}
			break;
		}
		return GameUtil.FloatToString(dtu, text) + locString;
	}

	public static string GetFormattedHeatEnergyRate(float dtu_s, GameUtil.HeatEnergyFormatterUnit unit = GameUtil.HeatEnergyFormatterUnit.Automatic)
	{
		LocString locString = string.Empty;
		if (unit != GameUtil.HeatEnergyFormatterUnit.Automatic)
		{
			if (unit != GameUtil.HeatEnergyFormatterUnit.KDTU_S)
			{
				if (unit == GameUtil.HeatEnergyFormatterUnit.DTU_S)
				{
					locString = UI.UNITSUFFIXES.HEAT.DTU_S;
				}
			}
			else
			{
				dtu_s /= 1000f;
				locString = UI.UNITSUFFIXES.HEAT.KDTU_S;
			}
		}
		else if (Mathf.Abs(dtu_s) > 1000f)
		{
			dtu_s /= 1000f;
			locString = UI.UNITSUFFIXES.HEAT.KDTU_S;
		}
		else
		{
			locString = UI.UNITSUFFIXES.HEAT.DTU_S;
		}
		return GameUtil.FloatToString(dtu_s, "###0.##") + locString;
	}

	public static string GetFormattedInt(float num, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		num = GameUtil.ApplyTimeSlice(num, timeSlice);
		return GameUtil.AddTimeSliceText(GameUtil.FloatToString(num, "F0"), timeSlice);
	}

	public static string GetFormattedSimple(float num, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, string formatString = null)
	{
		num = GameUtil.ApplyTimeSlice(num, timeSlice);
		string text = string.Empty;
		if (formatString != null)
		{
			text = GameUtil.FloatToString(num, formatString);
		}
		else if (num == 0f)
		{
			text = "0";
		}
		else if (Mathf.Abs(num) < 1f)
		{
			text = GameUtil.FloatToString(num, "#,##0.##");
		}
		else if (Mathf.Abs(num) < 10f)
		{
			text = GameUtil.FloatToString(num, "#,###.##");
		}
		else
		{
			text = GameUtil.FloatToString(num, "#,###.##");
		}
		return GameUtil.AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedLux(int lux)
	{
		return lux.ToString() + UI.UNITSUFFIXES.LIGHT.LUX;
	}

	public static string GetLightDescription(int lux)
	{
		if (lux == 0)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.NO_LIGHT;
		}
		if (lux < 100)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.VERY_LOW_LIGHT;
		}
		if (lux < 1000)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.LOW_LIGHT;
		}
		if (lux < 10000)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.MEDIUM_LIGHT;
		}
		if (lux < 50000)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.HIGH_LIGHT;
		}
		if (lux < 100000)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.VERY_HIGH_LIGHT;
		}
		return UI.OVERLAYS.LIGHTING.RANGES.MAX_LIGHT;
	}

	public static string GetFormattedByTag(Tag tag, float amount, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		if (GameTags.DisplayAsCalories.Contains(tag))
		{
			return GameUtil.GetFormattedCaloriesForItem(tag, amount, timeSlice, true);
		}
		if (GameTags.DisplayAsUnits.Contains(tag))
		{
			return GameUtil.GetFormattedUnits(amount, timeSlice, true);
		}
		return GameUtil.GetFormattedMass(amount, timeSlice, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
	}

	public static string GetFormattedFoodQuality(int quality)
	{
		if (GameUtil.adjectives == null)
		{
			GameUtil.adjectives = LocString.GetStrings(typeof(DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVES));
		}
		LocString locString = ((quality < 0) ? DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_FORMAT_NEGATIVE : DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_FORMAT_POSITIVE);
		int num = quality - DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_INDEX_OFFSET;
		num = Mathf.Clamp(num, 0, GameUtil.adjectives.Length);
		return string.Format(locString, GameUtil.adjectives[num], GameUtil.AddPositiveSign(quality.ToString(), quality > 0));
	}

	public static string GetFormattedInfomation(float amount, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		amount = GameUtil.ApplyTimeSlice(amount, timeSlice);
		string text = string.Empty;
		if (amount < 1024f)
		{
			text = UI.UNITSUFFIXES.INFORMATION.KILOBYTE;
		}
		else if (amount < 1048576f)
		{
			amount /= 1000f;
			text = UI.UNITSUFFIXES.INFORMATION.MEGABYTE;
		}
		else if (amount < 1.0737418E+09f)
		{
			amount /= 1048576f;
			text = UI.UNITSUFFIXES.INFORMATION.GIGABYTE;
		}
		return GameUtil.AddTimeSliceText(amount + text, timeSlice);
	}

	public static LocString GetCurrentMassUnit(bool useSmallUnit = false)
	{
		LocString locString = null;
		GameUtil.MassUnit massUnit = GameUtil.massUnit;
		if (massUnit != GameUtil.MassUnit.Kilograms)
		{
			if (massUnit == GameUtil.MassUnit.Pounds)
			{
				locString = UI.UNITSUFFIXES.MASS.POUND;
			}
		}
		else if (useSmallUnit)
		{
			locString = UI.UNITSUFFIXES.MASS.GRAM;
		}
		else
		{
			locString = UI.UNITSUFFIXES.MASS.KILOGRAM;
		}
		return locString;
	}

	public static string GetFormattedMass(float mass, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, GameUtil.MetricMassFormat massFormat = GameUtil.MetricMassFormat.UseThreshold, bool includeSuffix = true, string floatFormat = "{0:0.#}")
	{
		if (mass == -3.4028235E+38f)
		{
			return UI.CALCULATING;
		}
		mass = GameUtil.ApplyTimeSlice(mass, timeSlice);
		string text;
		if (GameUtil.massUnit == GameUtil.MassUnit.Kilograms)
		{
			text = UI.UNITSUFFIXES.MASS.TONNE;
			if (massFormat == GameUtil.MetricMassFormat.UseThreshold)
			{
				float num = Mathf.Abs(mass);
				if (0f < num)
				{
					if (num < 5E-06f)
					{
						text = UI.UNITSUFFIXES.MASS.MICROGRAM;
						mass = Mathf.Floor(mass * 1E+09f);
					}
					else if (num < 0.005f)
					{
						mass *= 1000000f;
						text = UI.UNITSUFFIXES.MASS.MILLIGRAM;
					}
					else if (Mathf.Abs(mass) < 5f)
					{
						mass *= 1000f;
						text = UI.UNITSUFFIXES.MASS.GRAM;
					}
					else if (Mathf.Abs(mass) < 5000f)
					{
						text = UI.UNITSUFFIXES.MASS.KILOGRAM;
					}
					else
					{
						mass /= 1000f;
						text = UI.UNITSUFFIXES.MASS.TONNE;
					}
				}
				else
				{
					text = UI.UNITSUFFIXES.MASS.KILOGRAM;
				}
			}
			else if (massFormat == GameUtil.MetricMassFormat.Kilogram)
			{
				text = UI.UNITSUFFIXES.MASS.KILOGRAM;
			}
			else if (massFormat == GameUtil.MetricMassFormat.Gram)
			{
				mass *= 1000f;
				text = UI.UNITSUFFIXES.MASS.GRAM;
			}
			else if (massFormat == GameUtil.MetricMassFormat.Tonne)
			{
				mass /= 1000f;
				text = UI.UNITSUFFIXES.MASS.TONNE;
			}
		}
		else
		{
			mass /= 2.2f;
			text = UI.UNITSUFFIXES.MASS.POUND;
			if (massFormat == GameUtil.MetricMassFormat.UseThreshold)
			{
				float num2 = Mathf.Abs(mass);
				if (num2 < 5f && num2 > 0.001f)
				{
					mass *= 256f;
					text = UI.UNITSUFFIXES.MASS.DRACHMA;
				}
				else
				{
					mass *= 7000f;
					text = UI.UNITSUFFIXES.MASS.GRAIN;
				}
			}
		}
		if (!includeSuffix)
		{
			text = string.Empty;
			timeSlice = GameUtil.TimeSlice.None;
		}
		return GameUtil.AddTimeSliceText(string.Format(floatFormat, mass) + text, timeSlice);
	}

	public static string GetFormattedTime(float seconds)
	{
		return string.Format(UI.FORMATSECONDS, seconds.ToString("F0"));
	}

	public static string GetFormattedEngineEfficiency(float amount)
	{
		return amount + " km /" + UI.UNITSUFFIXES.MASS.KILOGRAM;
	}

	public static string GetFormattedDistance(float meters)
	{
		if (Mathf.Abs(meters) < 1f)
		{
			string text = (meters * 100f).ToString();
			string text2 = text.Substring(0, text.LastIndexOf('.') + Mathf.Min(3, text.Length - text.LastIndexOf('.')));
			if (text2 == "-0.0")
			{
				text2 = "0";
			}
			return text2 + " cm";
		}
		if (meters < 1000f)
		{
			return meters + " m";
		}
		return Util.FormatOneDecimalPlace(meters / 1000f) + " km";
	}

	public static string GetFormattedCycles(float seconds, string formatString = "F1")
	{
		if (Mathf.Abs(seconds) > 100f)
		{
			return string.Format(UI.FORMATDAY, GameUtil.FloatToString(seconds / 600f, formatString));
		}
		return GameUtil.GetFormattedTime(seconds);
	}

	public static float GetDisplaySHC(float shc)
	{
		if (GameUtil.temperatureUnit == GameUtil.TemperatureUnit.Fahrenheit)
		{
			shc /= 1.8f;
		}
		return shc;
	}

	public static string GetSHCSuffix()
	{
		return string.Format("(DTU/g)/{0}", GameUtil.GetTemperatureUnitSuffix());
	}

	public static string GetFormattedSHC(float shc)
	{
		shc = GameUtil.GetDisplaySHC(shc);
		return string.Format("{0} (DTU/g)/{1}", shc.ToString("0.000"), GameUtil.GetTemperatureUnitSuffix());
	}

	public static float GetDisplayThermalConductivity(float tc)
	{
		if (GameUtil.temperatureUnit == GameUtil.TemperatureUnit.Fahrenheit)
		{
			tc /= 1.8f;
		}
		return tc;
	}

	public static string GetThermalConductivitySuffix()
	{
		return string.Format("(DTU/(m*s))/{0}", GameUtil.GetTemperatureUnitSuffix());
	}

	public static string GetFormattedThermalConductivity(float tc)
	{
		tc = GameUtil.GetDisplayThermalConductivity(tc);
		return string.Format("{0} (DTU/(m*s))/{1}", tc.ToString("0.000"), GameUtil.GetTemperatureUnitSuffix());
	}

	public static string GetElementNameByElementHash(SimHashes elementHash)
	{
		return ElementLoader.FindElementByHash(elementHash).tag.ProperName();
	}

	public static bool HasTrait(GameObject go, string traitName)
	{
		Traits component = go.GetComponent<Traits>();
		return !(component == null) && component.HasTrait(traitName);
	}

	public static HashSet<int> GetFloodFillCavity(int startCell, bool allowLiquid)
	{
		HashSet<int> hashSet = new HashSet<int>();
		if (allowLiquid)
		{
			hashSet = GameUtil.FloodCollectCells(startCell, (int cell) => !Grid.Solid[cell], 300, null, true);
		}
		else
		{
			hashSet = GameUtil.FloodCollectCells(startCell, (int cell) => Grid.Element[cell].IsVacuum || Grid.Element[cell].IsGas, 300, null, true);
		}
		return hashSet;
	}

	public static HashSet<int> FloodCollectCells(int start_cell, Func<int, bool> is_valid, int maxSize = 300, HashSet<int> AddInvalidCellsToSet = null, bool clearOversizedResults = true)
	{
		HashSet<int> hashSet = new HashSet<int>();
		HashSet<int> hashSet2 = new HashSet<int>();
		GameUtil.probeFromCell(start_cell, is_valid, hashSet, hashSet2, maxSize);
		if (AddInvalidCellsToSet != null)
		{
			AddInvalidCellsToSet.UnionWith(hashSet2);
			if (hashSet.Count > maxSize)
			{
				AddInvalidCellsToSet.UnionWith(hashSet);
			}
		}
		if (hashSet.Count > maxSize && clearOversizedResults)
		{
			hashSet.Clear();
		}
		return hashSet;
	}

	public static HashSet<int> FloodCollectCells(HashSet<int> results, int start_cell, Func<int, bool> is_valid, int maxSize = 300, HashSet<int> AddInvalidCellsToSet = null, bool clearOversizedResults = true)
	{
		HashSet<int> hashSet = new HashSet<int>();
		GameUtil.probeFromCell(start_cell, is_valid, results, hashSet, maxSize);
		if (AddInvalidCellsToSet != null)
		{
			AddInvalidCellsToSet.UnionWith(hashSet);
			if (results.Count > maxSize)
			{
				AddInvalidCellsToSet.UnionWith(results);
			}
		}
		if (results.Count > maxSize && clearOversizedResults)
		{
			results.Clear();
		}
		return results;
	}

	private static void probeFromCell(int start_cell, Func<int, bool> is_valid, HashSet<int> cells, HashSet<int> invalidCells, int maxSize = 300)
	{
		if (cells.Count > maxSize || !Grid.IsValidCell(start_cell) || invalidCells.Contains(start_cell) || cells.Contains(start_cell) || !is_valid(start_cell))
		{
			invalidCells.Add(start_cell);
			return;
		}
		cells.Add(start_cell);
		GameUtil.probeFromCell(Grid.CellLeft(start_cell), is_valid, cells, invalidCells, maxSize);
		GameUtil.probeFromCell(Grid.CellRight(start_cell), is_valid, cells, invalidCells, maxSize);
		GameUtil.probeFromCell(Grid.CellAbove(start_cell), is_valid, cells, invalidCells, maxSize);
		GameUtil.probeFromCell(Grid.CellBelow(start_cell), is_valid, cells, invalidCells, maxSize);
	}

	public static bool FloodFillCheck<ArgType>(Func<int, ArgType, bool> fn, ArgType arg, int start_cell, int max_depth, bool stop_at_solid, bool stop_at_liquid)
	{
		int num = GameUtil.FloodFillFind<ArgType>(fn, arg, start_cell, max_depth, stop_at_solid, stop_at_liquid);
		return num != -1;
	}

	public static int FloodFillFind<ArgType>(Func<int, ArgType, bool> fn, ArgType arg, int start_cell, int max_depth, bool stop_at_solid, bool stop_at_liquid)
	{
		GameUtil.FloodFillNext.Enqueue(new GameUtil.FloodFillInfo
		{
			cell = start_cell,
			depth = 0
		});
		int num = -1;
		while (GameUtil.FloodFillNext.Count > 0)
		{
			GameUtil.FloodFillInfo floodFillInfo = GameUtil.FloodFillNext.Dequeue();
			if (floodFillInfo.depth < max_depth)
			{
				if (Grid.IsValidCell(floodFillInfo.cell))
				{
					Element element = Grid.Element[floodFillInfo.cell];
					if (!stop_at_solid || !element.IsSolid)
					{
						if (!stop_at_liquid || !element.IsLiquid)
						{
							if (!GameUtil.FloodFillVisited.Contains(floodFillInfo.cell))
							{
								GameUtil.FloodFillVisited.Add(floodFillInfo.cell);
								if (fn(floodFillInfo.cell, arg))
								{
									num = floodFillInfo.cell;
									break;
								}
								GameUtil.FloodFillNext.Enqueue(new GameUtil.FloodFillInfo
								{
									cell = Grid.CellLeft(floodFillInfo.cell),
									depth = floodFillInfo.depth + 1
								});
								GameUtil.FloodFillNext.Enqueue(new GameUtil.FloodFillInfo
								{
									cell = Grid.CellRight(floodFillInfo.cell),
									depth = floodFillInfo.depth + 1
								});
								GameUtil.FloodFillNext.Enqueue(new GameUtil.FloodFillInfo
								{
									cell = Grid.CellAbove(floodFillInfo.cell),
									depth = floodFillInfo.depth + 1
								});
								GameUtil.FloodFillNext.Enqueue(new GameUtil.FloodFillInfo
								{
									cell = Grid.CellBelow(floodFillInfo.cell),
									depth = floodFillInfo.depth + 1
								});
							}
						}
					}
				}
			}
		}
		GameUtil.FloodFillVisited.Clear();
		GameUtil.FloodFillNext.Clear();
		return num;
	}

	public static void FloodFillConditional(int start_cell, Func<int, bool> condition, ICollection<int> visited_cells, ICollection<int> valid_cells = null)
	{
		GameUtil.FloodFillNext.Enqueue(new GameUtil.FloodFillInfo
		{
			cell = start_cell,
			depth = 0
		});
		GameUtil.FloodFillConditional(GameUtil.FloodFillNext, condition, visited_cells, valid_cells, 10000);
	}

	public static void FloodFillConditional(Queue<GameUtil.FloodFillInfo> queue, Func<int, bool> condition, ICollection<int> visited_cells, ICollection<int> valid_cells = null, int max_depth = 10000)
	{
		while (queue.Count > 0)
		{
			GameUtil.FloodFillInfo floodFillInfo = queue.Dequeue();
			if (floodFillInfo.depth < max_depth)
			{
				if (Grid.IsValidCell(floodFillInfo.cell))
				{
					if (!visited_cells.Contains(floodFillInfo.cell))
					{
						visited_cells.Add(floodFillInfo.cell);
						if (condition(floodFillInfo.cell))
						{
							if (valid_cells != null)
							{
								valid_cells.Add(floodFillInfo.cell);
							}
							queue.Enqueue(new GameUtil.FloodFillInfo
							{
								cell = Grid.CellLeft(floodFillInfo.cell),
								depth = floodFillInfo.depth + 1
							});
							queue.Enqueue(new GameUtil.FloodFillInfo
							{
								cell = Grid.CellRight(floodFillInfo.cell),
								depth = floodFillInfo.depth + 1
							});
							queue.Enqueue(new GameUtil.FloodFillInfo
							{
								cell = Grid.CellAbove(floodFillInfo.cell),
								depth = floodFillInfo.depth + 1
							});
							queue.Enqueue(new GameUtil.FloodFillInfo
							{
								cell = Grid.CellBelow(floodFillInfo.cell),
								depth = floodFillInfo.depth + 1
							});
						}
					}
				}
			}
		}
		queue.Clear();
	}

	public static GameUtil.Hardness GetHardness(Element element)
	{
		if (!element.IsSolid)
		{
			return GameUtil.Hardness.NA;
		}
		if (element.hardness >= 255)
		{
			return GameUtil.Hardness.IMPENETRABLE;
		}
		if (element.hardness >= 150)
		{
			return GameUtil.Hardness.NEARLY_IMPENETRABLE;
		}
		if (element.hardness >= 50)
		{
			return GameUtil.Hardness.VERY_FIRM;
		}
		if (element.hardness >= 25)
		{
			return GameUtil.Hardness.FIRM;
		}
		if (element.hardness >= 10)
		{
			return GameUtil.Hardness.SOFT;
		}
		return GameUtil.Hardness.NA;
	}

	public static string GetHardnessString(Element element, bool addColor = true)
	{
		if (!element.IsSolid)
		{
			return ELEMENTS.HARDNESS.NA;
		}
		Color color = new Color(0.83137256f, 0.28627452f, 0.28235295f);
		Color color2 = new Color(0.7411765f, 0.34901962f, 0.49803922f);
		Color color3 = new Color(0.6392157f, 0.39215687f, 0.6039216f);
		Color color4 = new Color(0.5254902f, 0.41960785f, 0.64705884f);
		Color color5 = new Color(0.42745098f, 0.48235294f, 0.75686276f);
		Color color6 = new Color(0.44313726f, 0.67058825f, 0.8117647f);
		Color color7 = color4;
		string text = string.Empty;
		GameUtil.Hardness hardness = GameUtil.GetHardness(element);
		if (hardness != GameUtil.Hardness.NA)
		{
			if (hardness != GameUtil.Hardness.SOFT)
			{
				if (hardness != GameUtil.Hardness.FIRM)
				{
					if (hardness != GameUtil.Hardness.VERY_FIRM)
					{
						if (hardness != GameUtil.Hardness.NEARLY_IMPENETRABLE)
						{
							if (hardness == GameUtil.Hardness.IMPENETRABLE)
							{
								color7 = color;
								text = string.Format(ELEMENTS.HARDNESS.IMPENETRABLE, element.hardness);
							}
						}
						else
						{
							color7 = color2;
							text = string.Format(ELEMENTS.HARDNESS.NEARLYIMPENETRABLE, element.hardness);
						}
					}
					else
					{
						color7 = color3;
						text = string.Format(ELEMENTS.HARDNESS.VERYFIRM, element.hardness);
					}
				}
				else
				{
					color7 = color4;
					text = string.Format(ELEMENTS.HARDNESS.FIRM, element.hardness);
				}
			}
			else
			{
				color7 = color5;
				text = string.Format(ELEMENTS.HARDNESS.SOFT, element.hardness);
			}
		}
		else
		{
			color7 = color6;
			text = string.Format(ELEMENTS.HARDNESS.VERYSOFT, element.hardness);
		}
		if (addColor)
		{
			text = string.Format("<color=#{0}>{1}</color>", color7.ToHexString(), text);
		}
		return text;
	}

	public static GameUtil.GermResistanceModifier GetGermResistanceModifier(float modifier)
	{
		if (modifier > 0f)
		{
			if (modifier >= 5f)
			{
				return GameUtil.GermResistanceModifier.POSITIVE_LARGE;
			}
			if (modifier >= 2f)
			{
				return GameUtil.GermResistanceModifier.POSITIVE_MEDIUM;
			}
			if (modifier >= 1f)
			{
				return GameUtil.GermResistanceModifier.POSITIVE_SMALL;
			}
		}
		else if (modifier < 0f)
		{
			if (modifier <= -5f)
			{
				return GameUtil.GermResistanceModifier.NEGATIVE_LARGE;
			}
			if (modifier <= -2f)
			{
				return GameUtil.GermResistanceModifier.NEGATIVE_MEDIUM;
			}
			if (modifier <= -1f)
			{
				return GameUtil.GermResistanceModifier.NEGATIVE_SMALL;
			}
		}
		return GameUtil.GermResistanceModifier.NONE;
	}

	public static string GetGermResistanceModifierString(float modifier, bool addColor = true)
	{
		Color color = new Color(0.83137256f, 0.28627452f, 0.28235295f);
		Color color2 = new Color(0.7411765f, 0.34901962f, 0.49803922f);
		Color color3 = new Color(0.6392157f, 0.39215687f, 0.6039216f);
		Color color4 = new Color(0.5254902f, 0.41960785f, 0.64705884f);
		Color color5 = new Color(0.42745098f, 0.48235294f, 0.75686276f);
		Color color6 = new Color(0.44313726f, 0.67058825f, 0.8117647f);
		Color color7 = color4;
		string text = string.Empty;
		GameUtil.GermResistanceModifier germResistanceModifier = GameUtil.GetGermResistanceModifier(modifier);
		switch (germResistanceModifier + 5)
		{
		case GameUtil.GermResistanceModifier.NONE:
			color7 = color4;
			text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NEGATIVE_LARGE, modifier);
			break;
		case (GameUtil.GermResistanceModifier)3:
			color7 = color3;
			text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NEGATIVE_MEDIUM, modifier);
			break;
		case (GameUtil.GermResistanceModifier)4:
			color7 = color2;
			text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NEGATIVE_SMALL, modifier);
			break;
		case GameUtil.GermResistanceModifier.POSITIVE_LARGE:
			color7 = color;
			text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NONE, modifier);
			break;
		case (GameUtil.GermResistanceModifier)6:
			color7 = color5;
			text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.POSITIVE_SMALL, modifier);
			break;
		case (GameUtil.GermResistanceModifier)7:
			color7 = color6;
			text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.POSITIVE_MEDIUM, modifier);
			break;
		case (GameUtil.GermResistanceModifier)10:
			color7 = color6;
			text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.POSITIVE_LARGE, modifier);
			break;
		}
		if (addColor)
		{
			text = string.Format("<color=#{0}>{1}</color>", color7.ToHexString(), text);
		}
		return text;
	}

	public static string GetThermalConductivityString(Element element, bool addColor = true, bool addValue = true)
	{
		Color color = new Color(0.83137256f, 0.28627452f, 0.28235295f);
		Color color2 = new Color(0.7411765f, 0.34901962f, 0.49803922f);
		Color color3 = new Color(0.6392157f, 0.39215687f, 0.6039216f);
		Color color4 = new Color(0.5254902f, 0.41960785f, 0.64705884f);
		Color color5 = new Color(0.42745098f, 0.48235294f, 0.75686276f);
		string text = string.Empty;
		Color color6;
		if (element.thermalConductivity >= 50f)
		{
			color6 = color5;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VERY_HIGH_CONDUCTIVITY;
		}
		else if (element.thermalConductivity >= 10f)
		{
			color6 = color4;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.HIGH_CONDUCTIVITY;
		}
		else if (element.thermalConductivity >= 2f)
		{
			color6 = color3;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.MEDIUM_CONDUCTIVITY;
		}
		else if (element.thermalConductivity >= 1f)
		{
			color6 = color2;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.LOW_CONDUCTIVITY;
		}
		else
		{
			color6 = color;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VERY_LOW_CONDUCTIVITY;
		}
		if (addColor)
		{
			text = string.Format("<color=#{0}>{1}</color>", color6.ToHexString(), text);
		}
		if (addValue)
		{
			text = string.Format(UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VALUE_WITH_ADJECTIVE, element.thermalConductivity.ToString(), text);
		}
		return text;
	}

	public static string GetBreathableString(Element element, float Mass)
	{
		if (!element.IsGas && !element.IsVacuum)
		{
			return string.Empty;
		}
		Color color = new Color(0.44313726f, 0.67058825f, 0.8117647f);
		Color color2 = new Color(0.6392157f, 0.39215687f, 0.6039216f);
		Color color3 = new Color(0.83137256f, 0.28627452f, 0.28235295f);
		SimHashes id = element.id;
		Color color4;
		LocString locString;
		if (id != SimHashes.Oxygen)
		{
			if (id != SimHashes.ContaminatedOxygen)
			{
				color4 = color3;
				locString = UI.OVERLAYS.OXYGEN.LEGEND4;
			}
			else if (Mass >= SimDebugView.optimallyBreathable)
			{
				color4 = color;
				locString = UI.OVERLAYS.OXYGEN.LEGEND1;
			}
			else if (Mass >= SimDebugView.minimumBreathable + (SimDebugView.optimallyBreathable - SimDebugView.minimumBreathable) / 2f)
			{
				color4 = color;
				locString = UI.OVERLAYS.OXYGEN.LEGEND2;
			}
			else if (Mass >= SimDebugView.minimumBreathable)
			{
				color4 = color2;
				locString = UI.OVERLAYS.OXYGEN.LEGEND3;
			}
			else
			{
				color4 = color3;
				locString = UI.OVERLAYS.OXYGEN.LEGEND4;
			}
		}
		else if (Mass >= SimDebugView.optimallyBreathable)
		{
			color4 = color;
			locString = UI.OVERLAYS.OXYGEN.LEGEND1;
		}
		else if (Mass >= SimDebugView.minimumBreathable + (SimDebugView.optimallyBreathable - SimDebugView.minimumBreathable) / 2f)
		{
			color4 = color;
			locString = UI.OVERLAYS.OXYGEN.LEGEND2;
		}
		else if (Mass >= SimDebugView.minimumBreathable)
		{
			color4 = color2;
			locString = UI.OVERLAYS.OXYGEN.LEGEND3;
		}
		else
		{
			color4 = color3;
			locString = UI.OVERLAYS.OXYGEN.LEGEND4;
		}
		return string.Format(ELEMENTS.BREATHABLEDESC, color4.ToHexString(), locString);
	}

	public static string GetWireLoadColor(float load, float maxLoad)
	{
		Color color = new Color(0.9843137f, 0.6901961f, 0.23137255f);
		Color color2 = new Color(1f, 0.19215687f, 0.19215687f);
		Color color3;
		if (load > maxLoad)
		{
			color3 = color2;
		}
		else if (load / maxLoad >= 0.75f)
		{
			color3 = color;
		}
		else
		{
			color3 = Color.white;
		}
		return color3.ToHexString();
	}

	public static string AppendHotkeyString(string template, global::Action action)
	{
		return template + UI.FormatAsHotkey("[" + GameUtil.GetActionString(action) + "]");
	}

	public static string ReplaceHotkeyString(string template, global::Action action)
	{
		return template.Replace("{Hotkey}", UI.FormatAsHotkey("[" + GameUtil.GetActionString(action) + "]"));
	}

	public static string ReplaceHotkeyString(string template, global::Action action1, global::Action action2)
	{
		return template.Replace("{Hotkey}", UI.FormatAsHotkey("[" + GameUtil.GetActionString(action1) + "]") + UI.FormatAsHotkey("[" + GameUtil.GetActionString(action2) + "]"));
	}

	public static string GetKeycodeLocalized(KKeyCode key_code)
	{
		string text = key_code.ToString();
		switch (key_code)
		{
		case KKeyCode.Keypad0:
			text = INPUT.NUM + " 0";
			break;
		case KKeyCode.Keypad1:
			text = INPUT.NUM + " 1";
			break;
		case KKeyCode.Keypad2:
			text = INPUT.NUM + " 2";
			break;
		case KKeyCode.Keypad3:
			text = INPUT.NUM + " 3";
			break;
		case KKeyCode.Keypad4:
			text = INPUT.NUM + " 4";
			break;
		case KKeyCode.Keypad5:
			text = INPUT.NUM + " 5";
			break;
		case KKeyCode.Keypad6:
			text = INPUT.NUM + " 6";
			break;
		case KKeyCode.Keypad7:
			text = INPUT.NUM + " 7";
			break;
		case KKeyCode.Keypad8:
			text = INPUT.NUM + " 8";
			break;
		case KKeyCode.Keypad9:
			text = INPUT.NUM + " 9";
			break;
		case KKeyCode.KeypadPeriod:
			text = INPUT.NUM + " " + INPUT.PERIOD;
			break;
		case KKeyCode.KeypadDivide:
			text = INPUT.NUM + " /";
			break;
		case KKeyCode.KeypadMultiply:
			text = INPUT.NUM + " *";
			break;
		case KKeyCode.KeypadMinus:
			text = INPUT.NUM + " -";
			break;
		case KKeyCode.KeypadPlus:
			text = INPUT.NUM + " +";
			break;
		case KKeyCode.KeypadEnter:
			text = INPUT.NUM + " " + INPUT.ENTER;
			break;
		default:
			switch (key_code)
			{
			case KKeyCode.Mouse0:
				text = INPUT.MOUSE + " 0";
				break;
			case KKeyCode.Mouse1:
				text = INPUT.MOUSE + " 1";
				break;
			case KKeyCode.Mouse2:
				text = INPUT.MOUSE + " 2";
				break;
			case KKeyCode.Mouse3:
				text = INPUT.MOUSE + " 3";
				break;
			case KKeyCode.Mouse4:
				text = INPUT.MOUSE + " 4";
				break;
			case KKeyCode.Mouse5:
				text = INPUT.MOUSE + " 5";
				break;
			case KKeyCode.Mouse6:
				text = INPUT.MOUSE + " 6";
				break;
			default:
				switch (key_code)
				{
				case KKeyCode.RightShift:
					text = INPUT.RIGHT_SHIFT;
					break;
				case KKeyCode.LeftShift:
					text = INPUT.LEFT_SHIFT;
					break;
				case KKeyCode.RightControl:
					text = INPUT.RIGHT_CTRL;
					break;
				case KKeyCode.LeftControl:
					text = INPUT.LEFT_CTRL;
					break;
				case KKeyCode.RightAlt:
					text = INPUT.RIGHT_ALT;
					break;
				case KKeyCode.LeftAlt:
					text = INPUT.LEFT_ALT;
					break;
				default:
					switch (key_code)
					{
					case KKeyCode.Plus:
						text = "+";
						break;
					case KKeyCode.Comma:
						text = ",";
						break;
					case KKeyCode.Minus:
						text = "-";
						break;
					case KKeyCode.Period:
						text = INPUT.PERIOD;
						break;
					case KKeyCode.Slash:
						text = "/";
						break;
					default:
						switch (key_code)
						{
						case KKeyCode.LeftBracket:
							text = "[";
							break;
						case KKeyCode.Backslash:
							text = "\\";
							break;
						case KKeyCode.RightBracket:
							text = "]";
							break;
						default:
							switch (key_code)
							{
							case KKeyCode.Backspace:
								text = INPUT.BACKSPACE;
								break;
							case KKeyCode.Tab:
								text = INPUT.TAB;
								break;
							default:
								switch (key_code)
								{
								case KKeyCode.Colon:
									text = ":";
									break;
								case KKeyCode.Semicolon:
									text = ";";
									break;
								default:
									if (key_code != KKeyCode.MouseScrollDown)
									{
										if (key_code != KKeyCode.MouseScrollUp)
										{
											if (key_code != KKeyCode.None)
											{
												if (key_code != KKeyCode.Escape)
												{
													if (key_code != KKeyCode.Space)
													{
														if (KKeyCode.A <= key_code && key_code <= KKeyCode.Z)
														{
															text = ((char)(65 + (key_code - KKeyCode.A))).ToString();
														}
														else if (KKeyCode.Alpha0 <= key_code && key_code <= KKeyCode.Alpha9)
														{
															text = ((char)(48 + (key_code - KKeyCode.Alpha0))).ToString();
														}
														else if (KKeyCode.F1 <= key_code && key_code <= KKeyCode.F12)
														{
															text = "F" + (key_code - KKeyCode.F1 + 1).ToString();
														}
														else
														{
															global::Debug.LogWarning("Unable to find proper string for KKeyCode: " + key_code.ToString() + " using key_code.ToString()");
														}
													}
													else
													{
														text = INPUT.SPACE;
													}
												}
												else
												{
													text = INPUT.ESCAPE;
												}
											}
										}
										else
										{
											text = INPUT.MOUSE_SCROLL_UP;
										}
									}
									else
									{
										text = INPUT.MOUSE_SCROLL_DOWN;
									}
									break;
								case KKeyCode.Equals:
									text = "=";
									break;
								}
								break;
							case KKeyCode.Return:
								text = INPUT.ENTER;
								break;
							}
							break;
						case KKeyCode.BackQuote:
							text = INPUT.BACKQUOTE;
							break;
						}
						break;
					}
					break;
				}
				break;
			}
			break;
		case KKeyCode.Insert:
			text = INPUT.INSERT;
			break;
		}
		return text;
	}

	public static string GetActionString(global::Action action)
	{
		string empty = string.Empty;
		if (action == global::Action.NumActions)
		{
			return empty;
		}
		BindingEntry bindingEntry = GameUtil.ActionToBinding(action);
		KKeyCode mKeyCode = bindingEntry.mKeyCode;
		if (bindingEntry.mModifier == global::Modifier.None)
		{
			return GameUtil.GetKeycodeLocalized(mKeyCode).ToUpper();
		}
		string text = string.Empty;
		switch (bindingEntry.mModifier)
		{
		case global::Modifier.Alt:
			text = GameUtil.GetKeycodeLocalized(KKeyCode.LeftAlt).ToUpper();
			break;
		case global::Modifier.Ctrl:
			text = GameUtil.GetKeycodeLocalized(KKeyCode.LeftControl).ToUpper();
			break;
		case global::Modifier.Shift:
			text = GameUtil.GetKeycodeLocalized(KKeyCode.LeftShift).ToUpper();
			break;
		case global::Modifier.CapsLock:
			text = GameUtil.GetKeycodeLocalized(KKeyCode.CapsLock).ToUpper();
			break;
		}
		return text + " + " + GameUtil.GetKeycodeLocalized(mKeyCode).ToUpper();
	}

	public static void CreateExplosion(Vector3 explosion_pos)
	{
		Vector2 vector = new Vector2(explosion_pos.x, explosion_pos.y);
		float num = 5f;
		float num2 = num * num;
		foreach (Health health in Components.Health.Items)
		{
			Vector3 position = health.transform.GetPosition();
			Vector2 vector2 = new Vector2(position.x, position.y);
			float sqrMagnitude = (vector2 - vector).sqrMagnitude;
			if (num2 >= sqrMagnitude && health != null)
			{
				health.Damage(health.maxHitPoints);
			}
		}
	}

	private static void GetNonSolidCells(int x, int y, List<int> cells, int min_x, int min_y, int max_x, int max_y)
	{
		int num = Grid.XYToCell(x, y);
		if (Grid.IsValidCell(num) && !Grid.Solid[num] && !Grid.DupePassable[num] && x >= min_x && x <= max_x && y >= min_y && y <= max_y && !cells.Contains(num))
		{
			cells.Add(num);
			GameUtil.GetNonSolidCells(x + 1, y, cells, min_x, min_y, max_x, max_y);
			GameUtil.GetNonSolidCells(x - 1, y, cells, min_x, min_y, max_x, max_y);
			GameUtil.GetNonSolidCells(x, y + 1, cells, min_x, min_y, max_x, max_y);
			GameUtil.GetNonSolidCells(x, y - 1, cells, min_x, min_y, max_x, max_y);
		}
	}

	public static void GetNonSolidCells(int cell, int radius, List<int> cells)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		GameUtil.GetNonSolidCells(num, num2, cells, num - radius, num2 - radius, num + radius, num2 + radius);
	}

	public static float GetMaxStress()
	{
		if (Components.LiveMinionIdentities.Count <= 0)
		{
			return 0f;
		}
		float num = 0f;
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
		{
			num = Mathf.Max(num, Db.Get().Amounts.Stress.Lookup(minionIdentity).value);
		}
		return num;
	}

	public static float GetAverageStress()
	{
		if (Components.LiveMinionIdentities.Count <= 0)
		{
			return 0f;
		}
		float num = 0f;
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
		{
			num += Db.Get().Amounts.Stress.Lookup(minionIdentity).value;
		}
		return num / (float)Components.LiveMinionIdentities.Count;
	}

	public static string MigrateFMOD(FMODAsset asset)
	{
		if (asset == null)
		{
			return null;
		}
		return (asset.path == null) ? asset.name : asset.path;
	}

	private static void SortDescriptors(List<IEffectDescriptor> descriptorList)
	{
		descriptorList.Sort(delegate(IEffectDescriptor e1, IEffectDescriptor e2)
		{
			int num = global::TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e1.GetType());
			int num2 = global::TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e2.GetType());
			return num.CompareTo(num2);
		});
	}

	private static void SortGameObjectDescriptors(List<IGameObjectEffectDescriptor> descriptorList)
	{
		descriptorList.Sort(delegate(IGameObjectEffectDescriptor e1, IGameObjectEffectDescriptor e2)
		{
			int num = global::TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e1.GetType());
			int num2 = global::TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e2.GetType());
			return num.CompareTo(num2);
		});
	}

	public static void IndentListOfDescriptors(List<Descriptor> list, int indentCount = 1)
	{
		for (int i = 0; i < list.Count; i++)
		{
			Descriptor descriptor = list[i];
			for (int j = 0; j < indentCount; j++)
			{
				descriptor.IncreaseIndent();
			}
			list[i] = descriptor;
		}
	}

	public static List<Descriptor> GetAllDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IEffectDescriptor> list2 = new List<IEffectDescriptor>(def.BuildingComplete.GetComponents<IEffectDescriptor>());
		GameUtil.SortDescriptors(list2);
		foreach (IEffectDescriptor effectDescriptor in list2)
		{
			List<Descriptor> descriptors = effectDescriptor.GetDescriptors(def);
			if (descriptors != null)
			{
				list.AddRange(descriptors);
			}
		}
		return list;
	}

	public static List<Descriptor> GetAllDescriptors(GameObject go, bool simpleInfoScreen = false)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IGameObjectEffectDescriptor> list2 = new List<IGameObjectEffectDescriptor>(go.GetComponents<IGameObjectEffectDescriptor>());
		StateMachineController component = go.GetComponent<StateMachineController>();
		if (component != null)
		{
			list2.AddRange(component.GetDescriptors());
		}
		GameUtil.SortGameObjectDescriptors(list2);
		foreach (IGameObjectEffectDescriptor gameObjectEffectDescriptor in list2)
		{
			List<Descriptor> descriptors = gameObjectEffectDescriptor.GetDescriptors(go);
			if (descriptors != null)
			{
				foreach (Descriptor descriptor in descriptors)
				{
					if (!descriptor.onlyForSimpleInfoScreen || simpleInfoScreen)
					{
						list.Add(descriptor);
					}
				}
			}
		}
		KPrefabID component2 = go.GetComponent<KPrefabID>();
		if (component2 != null && component2.AdditionalRequirements != null)
		{
			foreach (Descriptor descriptor2 in component2.AdditionalRequirements)
			{
				if (!descriptor2.onlyForSimpleInfoScreen || simpleInfoScreen)
				{
					list.Add(descriptor2);
				}
			}
		}
		if (component2 != null && component2.AdditionalEffects != null)
		{
			foreach (Descriptor descriptor3 in component2.AdditionalEffects)
			{
				if (!descriptor3.onlyForSimpleInfoScreen || simpleInfoScreen)
				{
					list.Add(descriptor3);
				}
			}
		}
		return list;
	}

	public static List<Descriptor> GetDetailDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			if (descriptor.type == Descriptor.DescriptorType.Detail)
			{
				list.Add(descriptor);
			}
		}
		GameUtil.IndentListOfDescriptors(list, 1);
		return list;
	}

	public static List<Descriptor> GetRequirementDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			if (descriptor.type == Descriptor.DescriptorType.Requirement)
			{
				list.Add(descriptor);
			}
		}
		GameUtil.IndentListOfDescriptors(list, 1);
		return list;
	}

	public static List<Descriptor> GetEffectDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			if (descriptor.type == Descriptor.DescriptorType.Effect || descriptor.type == Descriptor.DescriptorType.DiseaseSource)
			{
				list.Add(descriptor);
			}
		}
		GameUtil.IndentListOfDescriptors(list, 1);
		return list;
	}

	public static List<Descriptor> GetInformationDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			if (descriptor.type == Descriptor.DescriptorType.Lifecycle)
			{
				list.Add(descriptor);
			}
		}
		GameUtil.IndentListOfDescriptors(list, 1);
		return list;
	}

	public static List<Descriptor> GetCropOptimumConditionDescriptors(List<Descriptor> descriptors)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			if (descriptor.type == Descriptor.DescriptorType.Lifecycle)
			{
				Descriptor descriptor2 = descriptor;
				descriptor2.text = "• " + descriptor2.text;
				list.Add(descriptor2);
			}
		}
		GameUtil.IndentListOfDescriptors(list, 1);
		return list;
	}

	public static List<Descriptor> GetGameObjectRequirements(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IGameObjectEffectDescriptor> list2 = new List<IGameObjectEffectDescriptor>(go.GetComponents<IGameObjectEffectDescriptor>());
		StateMachineController component = go.GetComponent<StateMachineController>();
		if (component != null)
		{
			list2.AddRange(component.GetDescriptors());
		}
		GameUtil.SortGameObjectDescriptors(list2);
		foreach (IGameObjectEffectDescriptor gameObjectEffectDescriptor in list2)
		{
			List<Descriptor> descriptors = gameObjectEffectDescriptor.GetDescriptors(go);
			if (descriptors != null)
			{
				foreach (Descriptor descriptor in descriptors)
				{
					if (descriptor.type == Descriptor.DescriptorType.Requirement)
					{
						list.Add(descriptor);
					}
				}
			}
		}
		KPrefabID component2 = go.GetComponent<KPrefabID>();
		if (component2.AdditionalRequirements != null)
		{
			list.AddRange(component2.AdditionalRequirements);
		}
		return list;
	}

	public static List<Descriptor> GetGameObjectEffects(GameObject go, bool simpleInfoScreen = false)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IGameObjectEffectDescriptor> list2 = new List<IGameObjectEffectDescriptor>(go.GetComponents<IGameObjectEffectDescriptor>());
		StateMachineController component = go.GetComponent<StateMachineController>();
		if (component != null)
		{
			list2.AddRange(component.GetDescriptors());
		}
		GameUtil.SortGameObjectDescriptors(list2);
		foreach (IGameObjectEffectDescriptor gameObjectEffectDescriptor in list2)
		{
			List<Descriptor> descriptors = gameObjectEffectDescriptor.GetDescriptors(go);
			if (descriptors != null)
			{
				foreach (Descriptor descriptor in descriptors)
				{
					if (!descriptor.onlyForSimpleInfoScreen || simpleInfoScreen)
					{
						if (descriptor.type == Descriptor.DescriptorType.Effect || descriptor.type == Descriptor.DescriptorType.DiseaseSource)
						{
							list.Add(descriptor);
						}
					}
				}
			}
		}
		KPrefabID component2 = go.GetComponent<KPrefabID>();
		if (component2 != null && component2.AdditionalEffects != null)
		{
			foreach (Descriptor descriptor2 in component2.AdditionalEffects)
			{
				if (!descriptor2.onlyForSimpleInfoScreen || simpleInfoScreen)
				{
					list.Add(descriptor2);
				}
			}
		}
		return list;
	}

	public static List<Descriptor> GetPlantRequirementDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(go, false);
		List<Descriptor> requirementDescriptors = GameUtil.GetRequirementDescriptors(allDescriptors);
		if (requirementDescriptors.Count > 0)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.PLANTREQUIREMENTS, UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.PLANTREQUIREMENTS, Descriptor.DescriptorType.Requirement);
			list.Add(descriptor);
			list.AddRange(requirementDescriptors);
		}
		return list;
	}

	public static List<Descriptor> GetPlantLifeCycleDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<Descriptor> informationDescriptors = GameUtil.GetInformationDescriptors(GameUtil.GetAllDescriptors(go, false));
		if (informationDescriptors.Count > 0)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.LIFECYCLE, UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.PLANTLIFECYCLE, Descriptor.DescriptorType.Lifecycle);
			list.Add(descriptor);
			list.AddRange(informationDescriptors);
		}
		return list;
	}

	public static List<Descriptor> GetPlantEffectDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Growing component = go.GetComponent<Growing>();
		if (component == null)
		{
			return list;
		}
		List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(go, false);
		List<Descriptor> list2 = new List<Descriptor>();
		list2.AddRange(GameUtil.GetEffectDescriptors(allDescriptors));
		if (list2.Count > 0)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.PLANTEFFECTS, UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.PLANTEFFECTS, Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
			list.AddRange(list2);
		}
		return list;
	}

	public static string GetGameObjectEffectsTooltipString(GameObject go)
	{
		string text = string.Empty;
		List<Descriptor> gameObjectEffects = GameUtil.GetGameObjectEffects(go, false);
		if (gameObjectEffects.Count > 0)
		{
			text = text + UI.BUILDINGEFFECTS.OPERATIONEFFECTS + "\n";
		}
		foreach (Descriptor descriptor in gameObjectEffects)
		{
			text = text + descriptor.IndentedText() + "\n";
		}
		return text;
	}

	public static List<Descriptor> GetEquipmentEffects(EquipmentDef def)
	{
		global::Debug.Assert(def != null);
		List<Descriptor> list = new List<Descriptor>();
		List<AttributeModifier> attributeModifiers = def.AttributeModifiers;
		if (attributeModifiers != null)
		{
			foreach (AttributeModifier attributeModifier in attributeModifiers)
			{
				global::Klei.AI.Attribute attribute = Db.Get().Attributes.Get(attributeModifier.AttributeId);
				string name = attribute.Name;
				string formattedString = attributeModifier.GetFormattedString(null);
				string text = ((attributeModifier.Value < 0f) ? "consumed" : "produced");
				string text2 = UI.GAMEOBJECTEFFECTS.EQUIPMENT_MODS.text.Replace("{Attribute}", name).Replace("{Style}", text).Replace("{Value}", formattedString);
				list.Add(new Descriptor(text2, text2, Descriptor.DescriptorType.Effect, false));
			}
		}
		return list;
	}

	public static string GetRecipeDescription(Recipe recipe)
	{
		string text = null;
		if (recipe != null)
		{
			text = recipe.recipeDescription;
		}
		if (text == null)
		{
			text = "MISSING RECIPEDESCRIPTION";
			global::Debug.LogWarning("Missing recipeDescription");
		}
		return text;
	}

	public static int GetCurrentCycle()
	{
		return GameClock.Instance.GetCycle() + 1;
	}

	public static GameObject GetTelepad()
	{
		if (Components.Telepads.Count > 0)
		{
			return Components.Telepads[0].gameObject;
		}
		return null;
	}

	public static GameObject KInstantiate(GameObject original, Vector3 position, Grid.SceneLayer sceneLayer, string name = null, int gameLayer = 0)
	{
		return GameUtil.KInstantiate(original, position, sceneLayer, null, name, gameLayer);
	}

	public static GameObject KInstantiate(GameObject original, Vector3 position, Grid.SceneLayer sceneLayer, GameObject parent, string name = null, int gameLayer = 0)
	{
		position.z = Grid.GetLayerZ(sceneLayer);
		Vector3 vector = position;
		Quaternion identity = Quaternion.identity;
		return Util.KInstantiate(original, vector, identity, parent, name, true, gameLayer);
	}

	public static GameObject KInstantiate(GameObject original, Grid.SceneLayer sceneLayer, string name = null, int gameLayer = 0)
	{
		return GameUtil.KInstantiate(original, Vector3.zero, sceneLayer, name, gameLayer);
	}

	public static GameObject KInstantiate(Component original, Grid.SceneLayer sceneLayer, string name = null, int gameLayer = 0)
	{
		return GameUtil.KInstantiate(original.gameObject, Vector3.zero, sceneLayer, name, gameLayer);
	}

	public unsafe static void IsEmissionBlocked(int cell, out bool all_not_gaseous, out bool all_over_pressure)
	{
		int* ptr = stackalloc int[checked(4 * 4)];
		*ptr = Grid.CellBelow(cell);
		ptr[1] = Grid.CellLeft(cell);
		ptr[2] = Grid.CellRight(cell);
		ptr[3] = Grid.CellAbove(cell);
		all_not_gaseous = true;
		all_over_pressure = true;
		for (int i = 0; i < 4; i++)
		{
			int num = ptr[i];
			if (Grid.IsValidCell(num))
			{
				Element element = Grid.Element[num];
				all_not_gaseous = all_not_gaseous && (!element.IsGas && !element.IsVacuum);
				all_over_pressure = all_over_pressure && ((!element.IsGas && !element.IsVacuum) || Grid.Mass[num] >= 1.8f);
			}
		}
	}

	public static float GetDecorAtCell(int cell)
	{
		float num = 0f;
		if (!Grid.Solid[cell])
		{
			num = Grid.Decor[cell];
			num += (float)DecorProvider.GetLightDecorBonus(cell);
		}
		return num;
	}

	public static string GetKeywordStyle(Tag tag)
	{
		Element element = ElementLoader.GetElement(tag);
		string text;
		if (element != null)
		{
			text = GameUtil.GetKeywordStyle(element);
		}
		else if (GameUtil.foodTags.Contains(tag))
		{
			text = "food";
		}
		else if (GameUtil.solidTags.Contains(tag))
		{
			text = "solid";
		}
		else
		{
			text = null;
		}
		return text;
	}

	public static string GetKeywordStyle(SimHashes hash)
	{
		Element element = ElementLoader.FindElementByHash(hash);
		if (element != null)
		{
			return GameUtil.GetKeywordStyle(element);
		}
		return null;
	}

	public static string GetKeywordStyle(Element element)
	{
		if (element.id == SimHashes.Oxygen)
		{
			return "oxygen";
		}
		if (element.IsSolid)
		{
			return "solid";
		}
		if (element.IsLiquid)
		{
			return "liquid";
		}
		if (element.IsGas)
		{
			return "gas";
		}
		if (element.IsVacuum)
		{
			return "vacuum";
		}
		return null;
	}

	public static string GetKeywordStyle(GameObject go)
	{
		string text = string.Empty;
		Edible component = go.GetComponent<Edible>();
		Equippable component2 = go.GetComponent<Equippable>();
		MedicinalPill component3 = go.GetComponent<MedicinalPill>();
		ResearchPointObject component4 = go.GetComponent<ResearchPointObject>();
		if (component != null)
		{
			text = "food";
		}
		else if (component2 != null)
		{
			text = "equipment";
		}
		else if (component3 != null)
		{
			text = "medicine";
		}
		else if (component4 != null)
		{
			text = "research";
		}
		return text;
	}

	public static string GenerateRandomDuplicantName()
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		bool flag = global::UnityEngine.Random.Range(0f, 1f) >= 0.5f;
		List<string> list = new List<string>(LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.NAME.NB)));
		list.AddRange((!flag) ? LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.NAME.FEMALE)) : LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.NAME.MALE)));
		text3 = list.GetRandom<string>();
		bool flag2 = global::UnityEngine.Random.Range(0f, 1f) > 0.7f;
		if (flag2)
		{
			List<string> list2 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.PREFIX.NB)));
			list2.AddRange((!flag) ? LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.PREFIX.FEMALE)) : LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.PREFIX.MALE)));
			text = list2.GetRandom<string>();
		}
		if (!string.IsNullOrEmpty(text))
		{
			text += " ";
		}
		bool flag3 = global::UnityEngine.Random.Range(0f, 1f) >= 0.9f;
		if (flag3)
		{
			List<string> list3 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.SUFFIX.NB)));
			list3.AddRange((!flag) ? LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.SUFFIX.FEMALE)) : LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.SUFFIX.MALE)));
			text2 = list3.GetRandom<string>();
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text2 = " " + text2;
		}
		return text + text3 + text2;
	}

	public static string GenerateRandomRocketName()
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = string.Empty;
		int num = 1;
		int num2 = 2;
		int num3 = 4;
		List<string> list = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.NOUN)));
		text = list.GetRandom<string>();
		int num4 = 0;
		if (global::UnityEngine.Random.value > 0.7f)
		{
			List<string> list2 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.PREFIX)));
			text2 = list2.GetRandom<string>();
			num4 |= num;
		}
		if (global::UnityEngine.Random.value > 0.5f)
		{
			List<string> list3 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.ADJECTIVE)));
			text3 = list3.GetRandom<string>();
			num4 |= num2;
		}
		if (global::UnityEngine.Random.value > 0.1f)
		{
			List<string> list4 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.SUFFIX)));
			text4 = list4.GetRandom<string>();
			num4 |= num3;
		}
		string text5;
		if (num4 == (num | num2 | num3))
		{
			text5 = NAMEGEN.ROCKET.FMT_PREFIX_ADJECTIVE_NOUN_SUFFIX;
		}
		else if (num4 == (num2 | num3))
		{
			text5 = NAMEGEN.ROCKET.FMT_ADJECTIVE_NOUN_SUFFIX;
		}
		else if (num4 == (num | num3))
		{
			text5 = NAMEGEN.ROCKET.FMT_PREFIX_NOUN_SUFFIX;
		}
		else if (num4 == num3)
		{
			text5 = NAMEGEN.ROCKET.FMT_NOUN_SUFFIX;
		}
		else if (num4 == (num | num2))
		{
			text5 = NAMEGEN.ROCKET.FMT_PREFIX_ADJECTIVE_NOUN;
		}
		else if (num4 == num)
		{
			text5 = NAMEGEN.ROCKET.FMT_PREFIX_NOUN;
		}
		else if (num4 == num2)
		{
			text5 = NAMEGEN.ROCKET.FMT_ADJECTIVE_NOUN;
		}
		else
		{
			text5 = NAMEGEN.ROCKET.FMT_NOUN;
		}
		DebugUtil.LogArgs(new object[]
		{
			"Rocket name bits:",
			Convert.ToString(num4, 2)
		});
		return text5.Replace("{Prefix}", text2).Replace("{Adjective}", text3).Replace("{Noun}", text)
			.Replace("{Suffix}", text4);
	}

	public static float GetThermalComfort(int cell, float tolerance = -0.083680004f)
	{
		float num = 0f;
		Element element = ElementLoader.FindElementByHash(SimHashes.Creature);
		Element element2 = Grid.Element[cell];
		if (element2.thermalConductivity != 0f)
		{
			num = SimUtil.CalculateEnergyFlowCreatures(cell, 310.15f, element.specificHeatCapacity, element.thermalConductivity, 1f, 0.0045f);
		}
		num -= tolerance;
		return num * 1000f;
	}

	public static string GetFormattedDiseaseName(byte idx, bool color = false)
	{
		Disease disease = Db.Get().Diseases[(int)idx];
		if (color)
		{
			return string.Format(UI.OVERLAYS.DISEASE.DISEASE_NAME_FORMAT, disease.Name, GameUtil.ColourToHex(disease.overlayColour));
		}
		return string.Format(UI.OVERLAYS.DISEASE.DISEASE_NAME_FORMAT_NO_COLOR, disease.Name);
	}

	public static string GetFormattedDisease(byte idx, int units, bool color = false)
	{
		if (idx == 255 || units <= 0)
		{
			return UI.OVERLAYS.DISEASE.NO_DISEASE;
		}
		Disease disease = Db.Get().Diseases[(int)idx];
		if (color)
		{
			return string.Format(UI.OVERLAYS.DISEASE.DISEASE_FORMAT, disease.Name, GameUtil.GetFormattedDiseaseAmount(units), GameUtil.ColourToHex(disease.overlayColour));
		}
		return string.Format(UI.OVERLAYS.DISEASE.DISEASE_FORMAT_NO_COLOR, disease.Name, GameUtil.GetFormattedDiseaseAmount(units));
	}

	public static string GetFormattedDiseaseAmount(int units)
	{
		return units.ToString("#,##0") + UI.UNITSUFFIXES.DISEASE.UNITS;
	}

	public static string ColourizeString(Color32 colour, string str)
	{
		return string.Format("<color=#{0}>{1}</color>", GameUtil.ColourToHex(colour), str);
	}

	public static string ColourToHex(Color32 colour)
	{
		return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { colour.r, colour.g, colour.b, colour.a });
	}

	public static string GetFormattedDecor(float value, bool enforce_max = false)
	{
		string text = string.Empty;
		LocString locString = ((value <= DecorMonitor.MAXIMUM_DECOR_VALUE || !enforce_max) ? UI.OVERLAYS.DECOR.VALUE : UI.OVERLAYS.DECOR.MAXIMUM_DECOR);
		if (enforce_max)
		{
			value = Math.Min(value, DecorMonitor.MAXIMUM_DECOR_VALUE);
		}
		if (value > 0f)
		{
			text = "+";
		}
		else if (value >= 0f)
		{
			locString = UI.OVERLAYS.DECOR.VALUE_ZERO;
		}
		return string.Format(locString, text, value);
	}

	public static Color GetDecorColourFromValue(int decor)
	{
		Color color = Color.black;
		float num = (float)decor / 100f;
		if (num > 0f)
		{
			color = Color.Lerp(new Color(0.15f, 0f, 0f), new Color(0f, 1f, 0f), Mathf.Abs(num));
		}
		else
		{
			color = Color.Lerp(new Color(0.15f, 0f, 0f), new Color(1f, 0f, 0f), Mathf.Abs(num));
		}
		return color;
	}

	public static List<Descriptor> GetMaterialDescriptors(Element element)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (element.attributeModifiers.Count > 0)
		{
			foreach (AttributeModifier attributeModifier in element.attributeModifiers)
			{
				string text = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString(null));
				string text2 = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString(null));
				Descriptor descriptor = default(Descriptor);
				descriptor.SetupDescriptor(text, text2, Descriptor.DescriptorType.Effect);
				descriptor.IncreaseIndent();
				list.Add(descriptor);
			}
		}
		list.AddRange(GameUtil.GetSignificantMaterialPropertyDescriptors(element));
		return list;
	}

	public static string GetMaterialTooltips(Element element)
	{
		string text = element.tag.ProperName();
		foreach (AttributeModifier attributeModifier in element.attributeModifiers)
		{
			string name = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId).Name;
			string formattedString = attributeModifier.GetFormattedString(null);
			text = text + "\n    • " + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, name, formattedString);
		}
		text += GameUtil.GetSignificantMaterialPropertyTooltips(element);
		return text;
	}

	public static string GetSignificantMaterialPropertyTooltips(Element element)
	{
		string text = string.Empty;
		List<Descriptor> significantMaterialPropertyDescriptors = GameUtil.GetSignificantMaterialPropertyDescriptors(element);
		if (significantMaterialPropertyDescriptors.Count > 0)
		{
			text += "\n";
			for (int i = 0; i < significantMaterialPropertyDescriptors.Count; i++)
			{
				text = text + "    • " + Util.StripTextFormatting(significantMaterialPropertyDescriptors[i].text) + "\n";
			}
		}
		return text;
	}

	public static List<Descriptor> GetSignificantMaterialPropertyDescriptors(Element element)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (element.thermalConductivity > 10f)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(ELEMENTS.MATERIAL_MODIFIERS.HIGH_THERMAL_CONDUCTIVITY, GameUtil.GetThermalConductivityString(element, false, false)), string.Format(ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.HIGH_THERMAL_CONDUCTIVITY, element.name, element.thermalConductivity.ToString("0.#####")), Descriptor.DescriptorType.Effect);
			descriptor.IncreaseIndent();
			list.Add(descriptor);
		}
		if (element.thermalConductivity < 1f)
		{
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(string.Format(ELEMENTS.MATERIAL_MODIFIERS.LOW_THERMAL_CONDUCTIVITY, GameUtil.GetThermalConductivityString(element, false, false)), string.Format(ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.LOW_THERMAL_CONDUCTIVITY, element.name, element.thermalConductivity.ToString("0.#####")), Descriptor.DescriptorType.Effect);
			descriptor2.IncreaseIndent();
			list.Add(descriptor2);
		}
		if (element.specificHeatCapacity <= 0.2f)
		{
			Descriptor descriptor3 = default(Descriptor);
			descriptor3.SetupDescriptor(ELEMENTS.MATERIAL_MODIFIERS.LOW_SPECIFIC_HEAT_CAPACITY, string.Format(ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.LOW_SPECIFIC_HEAT_CAPACITY, element.name, element.specificHeatCapacity * 1f), Descriptor.DescriptorType.Effect);
			descriptor3.IncreaseIndent();
			list.Add(descriptor3);
		}
		if (element.specificHeatCapacity >= 1f)
		{
			Descriptor descriptor4 = default(Descriptor);
			descriptor4.SetupDescriptor(ELEMENTS.MATERIAL_MODIFIERS.HIGH_SPECIFIC_HEAT_CAPACITY, string.Format(ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.HIGH_SPECIFIC_HEAT_CAPACITY, element.name, element.specificHeatCapacity * 1f), Descriptor.DescriptorType.Effect);
			descriptor4.IncreaseIndent();
			list.Add(descriptor4);
		}
		return list;
	}

	public static int NaturalBuildingCell(this KMonoBehaviour cmp)
	{
		return Grid.PosToCell(cmp.transform.GetPosition());
	}

	public static List<Descriptor> GetMaterialDescriptors(Tag tag)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.GetElement(tag);
		if (element != null)
		{
			if (element.attributeModifiers.Count > 0)
			{
				foreach (AttributeModifier attributeModifier in element.attributeModifiers)
				{
					string text = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString(null));
					string text2 = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString(null));
					Descriptor descriptor = default(Descriptor);
					descriptor.SetupDescriptor(text, text2, Descriptor.DescriptorType.Effect);
					descriptor.IncreaseIndent();
					list.Add(descriptor);
				}
			}
			list.AddRange(GameUtil.GetSignificantMaterialPropertyDescriptors(element));
		}
		else
		{
			GameObject gameObject = Assets.TryGetPrefab(tag);
			if (gameObject != null)
			{
				PrefabAttributeModifiers component = gameObject.GetComponent<PrefabAttributeModifiers>();
				if (component != null)
				{
					foreach (AttributeModifier attributeModifier2 in component.descriptors)
					{
						string text3 = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + attributeModifier2.AttributeId.ToUpper())), attributeModifier2.GetFormattedString(null));
						string text4 = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + attributeModifier2.AttributeId.ToUpper())), attributeModifier2.GetFormattedString(null));
						Descriptor descriptor2 = default(Descriptor);
						descriptor2.SetupDescriptor(text3, text4, Descriptor.DescriptorType.Effect);
						descriptor2.IncreaseIndent();
						list.Add(descriptor2);
					}
				}
			}
		}
		return list;
	}

	public static string GetMaterialTooltips(Tag tag)
	{
		string text = tag.ProperName();
		Element element = ElementLoader.GetElement(tag);
		if (element != null)
		{
			foreach (AttributeModifier attributeModifier in element.attributeModifiers)
			{
				string name = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId).Name;
				string formattedString = attributeModifier.GetFormattedString(null);
				text = text + "\n    • " + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, name, formattedString);
			}
			text += GameUtil.GetSignificantMaterialPropertyTooltips(element);
		}
		else
		{
			GameObject gameObject = Assets.TryGetPrefab(tag);
			if (gameObject != null)
			{
				PrefabAttributeModifiers component = gameObject.GetComponent<PrefabAttributeModifiers>();
				if (component != null)
				{
					foreach (AttributeModifier attributeModifier2 in component.descriptors)
					{
						string name2 = Db.Get().BuildingAttributes.Get(attributeModifier2.AttributeId).Name;
						string formattedString2 = attributeModifier2.GetFormattedString(null);
						text = text + "\n    • " + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, name2, formattedString2);
					}
				}
			}
		}
		return text;
	}

	public static bool AreChoresUIMergeable(Chore.Precondition.Context choreA, Chore.Precondition.Context choreB)
	{
		if (choreA.chore.target.isNull || choreB.chore.target.isNull)
		{
			return false;
		}
		ChoreType choreType = choreB.chore.choreType;
		ChoreType choreType2 = choreA.chore.choreType;
		return (choreA.chore.choreType == choreB.chore.choreType && choreA.chore.target.GetComponent<KPrefabID>().PrefabTag == choreB.chore.target.GetComponent<KPrefabID>().PrefabTag) || (choreA.chore.choreType == Db.Get().ChoreTypes.Dig && choreB.chore.choreType == Db.Get().ChoreTypes.Dig) || (choreA.chore.choreType == Db.Get().ChoreTypes.Relax && choreB.chore.choreType == Db.Get().ChoreTypes.Relax) || ((choreType2 == Db.Get().ChoreTypes.ReturnSuitIdle || choreType2 == Db.Get().ChoreTypes.ReturnSuitUrgent) && (choreType == Db.Get().ChoreTypes.ReturnSuitIdle || choreType == Db.Get().ChoreTypes.ReturnSuitUrgent)) || (choreA.chore.target.gameObject == choreB.chore.target.gameObject && choreA.chore.choreType == choreB.chore.choreType);
	}

	public static string GetChoreName(Chore chore, object choreData)
	{
		string text = string.Empty;
		if (chore.choreType == Db.Get().ChoreTypes.Fetch || chore.choreType == Db.Get().ChoreTypes.MachineFetch || chore.choreType == Db.Get().ChoreTypes.FabricateFetch || chore.choreType == Db.Get().ChoreTypes.FetchCritical || chore.choreType == Db.Get().ChoreTypes.PowerFetch)
		{
			text = chore.GetReportName(chore.gameObject.GetProperName());
		}
		else if (chore.choreType == Db.Get().ChoreTypes.StorageFetch || chore.choreType == Db.Get().ChoreTypes.FoodFetch)
		{
			FetchChore fetchChore = chore as FetchChore;
			FetchAreaChore fetchAreaChore = chore as FetchAreaChore;
			if (fetchAreaChore != null)
			{
				GameObject getFetchTarget = fetchAreaChore.GetFetchTarget;
				KMonoBehaviour kmonoBehaviour = choreData as KMonoBehaviour;
				if (getFetchTarget != null)
				{
					text = chore.GetReportName(getFetchTarget.GetProperName());
				}
				else if (kmonoBehaviour != null)
				{
					text = chore.GetReportName(kmonoBehaviour.GetProperName());
				}
				else
				{
					text = chore.GetReportName(null);
				}
			}
			else if (fetchChore != null)
			{
				Pickupable fetchTarget = fetchChore.fetchTarget;
				KMonoBehaviour kmonoBehaviour2 = choreData as KMonoBehaviour;
				if (fetchTarget != null)
				{
					text = chore.GetReportName(fetchTarget.GetProperName());
				}
				else if (kmonoBehaviour2 != null)
				{
					text = chore.GetReportName(kmonoBehaviour2.GetProperName());
				}
				else
				{
					text = chore.GetReportName(null);
				}
			}
		}
		else
		{
			text = chore.GetReportName(null);
		}
		return text;
	}

	public static string ChoreGroupsForChoreType(ChoreType choreType)
	{
		if (choreType.groups == null || choreType.groups.Length == 0)
		{
			return null;
		}
		string text = string.Empty;
		for (int i = 0; i < choreType.groups.Length; i++)
		{
			if (i != 0)
			{
				text += UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CHORE_GROUP_SEPARATOR;
			}
			text += choreType.groups[i].Name;
		}
		return text;
	}

	public static bool IsCapturingTimeLapse()
	{
		return Game.Instance != null && Game.Instance.timelapser != null && Game.Instance.timelapser.CapturingTimelapseScreenshot;
	}

	public static ExposureType GetExposureTypeForDisease(Disease disease)
	{
		for (int i = 0; i < GERM_EXPOSURE.TYPES.Length; i++)
		{
			if (disease.id == GERM_EXPOSURE.TYPES[i].germ_id)
			{
				return GERM_EXPOSURE.TYPES[i];
			}
		}
		return null;
	}

	public static Sickness GetSicknessForDisease(Disease disease)
	{
		for (int i = 0; i < GERM_EXPOSURE.TYPES.Length; i++)
		{
			if (disease.id == GERM_EXPOSURE.TYPES[i].germ_id)
			{
				return Db.Get().Sicknesses.Get(GERM_EXPOSURE.TYPES[i].sickness_id);
			}
		}
		return null;
	}

	public static GameUtil.TemperatureUnit temperatureUnit;

	public static GameUtil.MassUnit massUnit;

	private static string[] adjectives;

	[ThreadStatic]
	public static Queue<GameUtil.FloodFillInfo> FloodFillNext = new Queue<GameUtil.FloodFillInfo>();

	[ThreadStatic]
	public static HashSet<int> FloodFillVisited = new HashSet<int>();

	public static TagSet foodTags = new TagSet(new string[]
	{
		"BasicPlantFood",
		"MushBar",
		"ColdWheatSeed",
		"ColdWheatSeed",
		"SpiceNut",
		"PrickleFruit",
		"Meat",
		"Mushroom",
		"ColdWheat",
		GameTags.Compostable.Name
	});

	public static TagSet solidTags = new TagSet(new string[] { "Filter", "Coal", "BasicFabric", "SwampLilyFlower", "RefinedMetal" });

	public enum UnitClass
	{
		SimpleFloat,
		SimpleInteger,
		Temperature,
		Mass,
		Calories,
		Percent,
		Distance,
		Disease
	}

	public enum TemperatureUnit
	{
		Celsius,
		Fahrenheit,
		Kelvin
	}

	public enum MassUnit
	{
		Kilograms,
		Pounds
	}

	public enum MetricMassFormat
	{
		UseThreshold,
		Kilogram,
		Gram,
		Tonne
	}

	public enum TemperatureInterpretation
	{
		Absolute,
		Relative
	}

	public enum TimeSlice
	{
		None,
		ModifyOnly,
		PerSecond,
		PerCycle
	}

	public enum MeasureUnit
	{
		mass,
		kcal,
		quantity
	}

	public enum WattageFormatterUnit
	{
		Watts,
		Kilowatts,
		Automatic
	}

	public enum HeatEnergyFormatterUnit
	{
		DTU_S,
		KDTU_S,
		Automatic
	}

	public struct FloodFillInfo
	{
		public int cell;

		public int depth;
	}

	public enum Hardness
	{
		NA,
		VERY_SOFT = 0,
		SOFT = 10,
		FIRM = 25,
		VERY_FIRM = 50,
		NEARLY_IMPENETRABLE = 150,
		SUPER_HARD = 200,
		IMPENETRABLE = 255
	}

	public enum GermResistanceModifier
	{
		NONE,
		POSITIVE_SMALL,
		POSITIVE_MEDIUM,
		POSITIVE_LARGE = 5,
		NEGATIVE_SMALL = -1,
		NEGATIVE_MEDIUM = -2,
		NEGATIVE_LARGE = -5
	}
}
