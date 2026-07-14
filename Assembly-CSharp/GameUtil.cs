using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Database;
using Klei;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.Pool;

public static class GameUtil
{
	public static CellOffset[] Expand(this CellOffset[] original)
	{
		List<CellOffset> list = new List<CellOffset>(original);
		Vector4 vector = new Vector2(float.MaxValue, float.MinValue);
		Vector4 vector2 = new Vector2(float.MaxValue, float.MinValue);
		foreach (CellOffset cellOffset in original)
		{
			if ((float)cellOffset.x < vector.x)
			{
				vector.x = (float)cellOffset.x;
			}
			if ((float)cellOffset.x > vector.y)
			{
				vector.y = (float)cellOffset.x;
			}
			if ((float)cellOffset.y < vector2.x)
			{
				vector2.x = (float)cellOffset.y;
			}
			if ((float)cellOffset.y > vector2.y)
			{
				vector2.y = (float)cellOffset.y;
			}
		}
		foreach (CellOffset cellOffset2 in original)
		{
			Vector2Int zero = Vector2Int.zero;
			if ((float)cellOffset2.x == vector.x)
			{
				list.Add(new CellOffset(cellOffset2.x - 1, cellOffset2.y));
				zero.x = -1;
			}
			if ((float)cellOffset2.x == vector.y)
			{
				list.Add(new CellOffset(cellOffset2.x + 1, cellOffset2.y));
				zero.x = 1;
			}
			if ((float)cellOffset2.y == vector2.x)
			{
				list.Add(new CellOffset(cellOffset2.x, cellOffset2.y - 1));
				zero.y = -1;
			}
			if ((float)cellOffset2.y == vector2.y)
			{
				list.Add(new CellOffset(cellOffset2.x, cellOffset2.y + 1));
				zero.y = 1;
			}
			if (zero.x != 0 && zero.y != 0)
			{
				list.Add(new CellOffset((int)((zero.x < 0) ? vector.x : vector.y) + zero.x, (int)((zero.y < 0) ? vector2.x : vector2.y) + zero.y));
			}
		}
		return list.ToArray();
	}

	public static void TintLiquidSymbolOnBuilding(string symbolName, KBatchedAnimController controller, Element element)
	{
		Color color = (element.IsMoltenMetal ? WaterCubes.MOLTEN_METAL_COLOR : element.substance.colour);
		color.a = 1f;
		string text = symbolName + "_bloom";
		string text2 = (element.substance.Glows ? text : symbolName);
		controller.SetSymbolVisiblity(new KAnimHashedString(symbolName), !element.substance.Glows);
		controller.SetSymbolVisiblity(new KAnimHashedString(text), element.substance.Glows);
		controller.SetSymbolTint(new KAnimHashedString(text2), color);
	}

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
		if (temperatureUnit != GameUtil.TemperatureUnit.Celsius)
		{
			if (temperatureUnit != GameUtil.TemperatureUnit.Fahrenheit)
			{
				if (!roundOutput)
				{
					return temperature;
				}
				return Mathf.Round(temperature);
			}
			else
			{
				float num = temperature * 1.8f - 459.67f;
				if (!roundOutput)
				{
					return num;
				}
				return Mathf.Round(num);
			}
		}
		else
		{
			float num = temperature - 273.15f;
			if (!roundOutput)
			{
				return num;
			}
			return Mathf.Round(num);
		}
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

	public static float ApplyTimeSlice(int val, GameUtil.TimeSlice timeSlice)
	{
		if (timeSlice == GameUtil.TimeSlice.PerCycle)
		{
			return (float)val * 600f;
		}
		return (float)val;
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

	public static void AddTimeSliceText(StringBuilder builder, GameUtil.TimeSlice timeSlice)
	{
		switch (timeSlice)
		{
		case GameUtil.TimeSlice.None:
		case GameUtil.TimeSlice.ModifyOnly:
			break;
		case GameUtil.TimeSlice.PerSecond:
			builder.Append(UI.UNITSUFFIXES.PERSECOND);
			return;
		case GameUtil.TimeSlice.PerCycle:
			builder.Append(UI.UNITSUFFIXES.PERCYCLE);
			break;
		default:
			return;
		}
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

	public static string GetIdentityDescriptor(GameObject go, GameUtil.IdentityDescriptorTense tense = GameUtil.IdentityDescriptorTense.Normal)
	{
		if (go.GetComponent<MinionIdentity>())
		{
			switch (tense)
			{
			case GameUtil.IdentityDescriptorTense.Normal:
				return DUPLICANTS.STATS.SUBJECTS.DUPLICANT;
			case GameUtil.IdentityDescriptorTense.Possessive:
				return DUPLICANTS.STATS.SUBJECTS.DUPLICANT_POSSESSIVE;
			case GameUtil.IdentityDescriptorTense.Plural:
				return DUPLICANTS.STATS.SUBJECTS.DUPLICANT_PLURAL;
			}
		}
		else if (go.GetComponent<CreatureBrain>())
		{
			switch (tense)
			{
			case GameUtil.IdentityDescriptorTense.Normal:
				return DUPLICANTS.STATS.SUBJECTS.CREATURE;
			case GameUtil.IdentityDescriptorTense.Possessive:
				return DUPLICANTS.STATS.SUBJECTS.CREATURE_POSSESSIVE;
			case GameUtil.IdentityDescriptorTense.Plural:
				return DUPLICANTS.STATS.SUBJECTS.CREATURE_PLURAL;
			}
		}
		else
		{
			switch (tense)
			{
			case GameUtil.IdentityDescriptorTense.Normal:
				return DUPLICANTS.STATS.SUBJECTS.PLANT;
			case GameUtil.IdentityDescriptorTense.Possessive:
				return DUPLICANTS.STATS.SUBJECTS.PLANT_POSESSIVE;
			case GameUtil.IdentityDescriptorTense.Plural:
				return DUPLICANTS.STATS.SUBJECTS.PLANT_PLURAL;
			}
		}
		return "";
	}

	public static float GetEnergyInPrimaryElement(PrimaryElement element)
	{
		return 0.001f * (element.Temperature * (element.Mass * 1000f * element.Element.specificHeatCapacity));
	}

	public static float EnergyToTemperatureDelta(float kilojoules, PrimaryElement element)
	{
		global::Debug.Assert(element.Mass > 0f);
		float num = Mathf.Max(GameUtil.GetEnergyInPrimaryElement(element) - kilojoules, 1f);
		float temperature = element.Temperature;
		return num / (0.001f * (element.Mass * (element.Element.specificHeatCapacity * 1000f))) - temperature;
	}

	public static float CalculateEnergyDeltaForElement(PrimaryElement element, float startTemp, float endTemp)
	{
		return GameUtil.CalculateEnergyDeltaForElementChange(element.Mass, element.Element.specificHeatCapacity, startTemp, endTemp);
	}

	public static float CalculateEnergyDeltaForElementChange(float mass, float shc, float startTemp, float endTemp)
	{
		return (endTemp - startTemp) * mass * shc;
	}

	public static float GetFinalTemperature(float t1, float m1, float t2, float m2)
	{
		float num = m1 + m2;
		float num2 = (t1 * m1 + t2 * m2) / num;
		float num3 = Mathf.Min(t1, t2);
		float num4 = Mathf.Max(t1, t2);
		num2 = Mathf.Clamp(num2, num3, num4);
		if (float.IsNaN(num2) || float.IsInfinity(num2))
		{
			global::Debug.LogError(string.Format("Calculated an invalid temperature: t1={0}, m1={1}, t2={2}, m2={3}, min_temp={4}, max_temp={5}", new object[] { t1, m1, t2, m2, num3, num4 }));
		}
		return num2;
	}

	public static void ForceConduction(PrimaryElement a, PrimaryElement b, float dt)
	{
		float num = a.Temperature * a.Element.specificHeatCapacity * a.Mass;
		float num2 = b.Temperature * b.Element.specificHeatCapacity * b.Mass;
		float num3 = Math.Min(a.Element.thermalConductivity, b.Element.thermalConductivity);
		float num4 = Math.Min(a.Mass, b.Mass);
		float num5 = (b.Temperature - a.Temperature) * (num3 * num4) * dt;
		float num6 = (num + num2) / (a.Element.specificHeatCapacity * a.Mass + b.Element.specificHeatCapacity * b.Mass);
		float num7 = Math.Abs((num6 - a.Temperature) * a.Element.specificHeatCapacity * a.Mass);
		float num8 = Math.Abs((num6 - b.Temperature) * b.Element.specificHeatCapacity * b.Mass);
		float num9 = Math.Min(num7, num8);
		num5 = Math.Min(num5, num9);
		num5 = Math.Max(num5, -num9);
		a.Temperature = (num + num5) / a.Element.specificHeatCapacity / a.Mass;
		b.Temperature = (num2 - num5) / b.Element.specificHeatCapacity / b.Mass;
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

	public unsafe static void AppendFloatToString(StringBuilder builder, float f, string format = null)
	{
		if (float.IsPositiveInfinity(f))
		{
			builder.Append(UI.POS_INFINITY);
			return;
		}
		if (float.IsNegativeInfinity(f))
		{
			builder.Append(UI.NEG_INFINITY);
			return;
		}
		if (format != null)
		{
			Span<char> span = new Span<char>(stackalloc byte[(UIntPtr)128], 64);
			int num;
			f.TryFormat(span, out num, format, null);
			builder.Append(span.Slice(0, num));
			return;
		}
		builder.Append(f);
	}

	public static string GetFloatWithDecimalPoint(float f)
	{
		string text;
		if (f == 0f)
		{
			text = "0";
		}
		else if (Mathf.Abs(f) < 1f)
		{
			text = "#,##0.#";
		}
		else
		{
			text = "#,###.#";
		}
		return GameUtil.FloatToString(f, text);
	}

	public static void AppendFloatWithDecimalPoint(StringBuilder builder, float f)
	{
		if (f == 0f)
		{
			builder.AppendFormat("{0:0}", f);
			return;
		}
		if (Mathf.Abs(f) < 1f)
		{
			builder.AppendFormat("{0:#,##0.#}", f);
			return;
		}
		builder.AppendFormat("{0:#,###.#}", f);
	}

	public static string GetStandardFloat(float f)
	{
		string text;
		if (f == 0f)
		{
			text = "0";
		}
		else if (Mathf.Abs(f) < 1f)
		{
			text = "#,##0.#";
		}
		else if (Mathf.Abs(f) < 10f)
		{
			text = "#,###.#";
		}
		else
		{
			text = "#,###";
		}
		return GameUtil.FloatToString(f, text);
	}

	public static void AppendStandardFloat(StringBuilder builder, float f)
	{
		if (float.IsPositiveInfinity(f))
		{
			builder.Append(UI.POS_INFINITY);
			return;
		}
		if (float.IsNegativeInfinity(f))
		{
			builder.Append(UI.NEG_INFINITY);
			return;
		}
		if (f == 0f)
		{
			builder.AppendFormat("{0:0}", f);
			return;
		}
		if (Math.Abs(f) < 1f)
		{
			builder.AppendFormat("{0:#,##0.##}", f);
			return;
		}
		if (Math.Abs(f) < 10f)
		{
			builder.AppendFormat("{0:#,##0.##}", f);
			return;
		}
		builder.AppendFormat("{0:#,###}", f);
	}

	public static string GetStandardPercentageFloat(float f, bool allowHundredths = false)
	{
		string text;
		if (Mathf.Abs(f) == 0f)
		{
			text = "0";
		}
		else if (Mathf.Abs(f) < 0.1f && allowHundredths)
		{
			text = "##0.##";
		}
		else if (Mathf.Abs(f) < 1f)
		{
			text = "##0.#";
		}
		else
		{
			text = "##0";
		}
		return GameUtil.FloatToString(f, text);
	}

	public static void AppendStandardPercentageFloat(StringBuilder builder, float f, bool allowHundredths = false)
	{
		if (Mathf.Abs(f) == 0f)
		{
			builder.AppendFormat("{0:0}", f);
			return;
		}
		if (Mathf.Abs(f) < 0.1f && allowHundredths)
		{
			builder.AppendFormat("{0:##0.##}", f);
			return;
		}
		if (Mathf.Abs(f) < 1f)
		{
			builder.AppendFormat("{0:##0.#}", f);
			return;
		}
		if (f < 100f && f >= 99.5f)
		{
			f = 99f;
		}
		builder.AppendFormat("{0:##0}", f);
	}

	public static string GetUnitFormattedName(GameObject go, bool upperName = false)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		if (component != null && Assets.IsTagCountable(component.PrefabTag))
		{
			PrimaryElement component2 = go.GetComponent<PrimaryElement>();
			return GameUtil.GetUnitFormattedName(go.GetProperName(), component2.Units, upperName);
		}
		if (!upperName)
		{
			return go.GetProperName();
		}
		return StringFormatter.ToUpper(go.GetProperName());
	}

	public static string GetUnitFormattedName(string name, float count, bool upperName = false)
	{
		if (upperName)
		{
			name = name.ToUpper();
		}
		return StringFormatter.Replace(UI.NAME_WITH_UNITS, "{0}", name).Replace("{1}", string.Format("{0:0.##}", count));
	}

	public static void AppendFormattedUnits(StringBuilder builder, float units, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool displaySuffix = true, string floatFormatOverride = "")
	{
		units = GameUtil.ApplyTimeSlice(units, timeSlice);
		if (!floatFormatOverride.IsNullOrWhiteSpace())
		{
			builder.AppendFormat(floatFormatOverride, units);
		}
		else
		{
			GameUtil.AppendStandardFloat(builder, units);
		}
		if (displaySuffix)
		{
			builder.Append((units == 1f) ? UI.UNITSUFFIXES.UNIT : UI.UNITSUFFIXES.UNITS);
		}
		GameUtil.AddTimeSliceText(builder, timeSlice);
	}

	public static string GetFormattedUnits(float units, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool displaySuffix = true, string floatFormatOverride = "")
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedUnits(stringBuilder, units, timeSlice, displaySuffix, floatFormatOverride);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static void AppendFormattedRocketRangePerCycle(StringBuilder builder, float range, bool displaySuffix = true)
	{
		if (displaySuffix)
		{
			builder.AppendFormat("{0:N1} {1}", range, UI.CLUSTERMAP.TILES_PER_CYCLE);
			return;
		}
		builder.AppendFormat("{0:N1}", range);
	}

	public static string GetFormattedRocketRangePerCycle(float range, bool displaySuffix = true)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedRocketRangePerCycle(stringBuilder, range, displaySuffix);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static void AppendFormattedRocketRange(StringBuilder builder, int rangeInTiles, bool displaySuffix = true)
	{
		builder.Append(rangeInTiles);
		if (displaySuffix)
		{
			builder.Append(" ");
			builder.Append(UI.CLUSTERMAP.TILES);
		}
	}

	public static string GetFormattedRocketRange(int rangeInTiles, bool displaySuffix = true)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedRocketRange(stringBuilder, rangeInTiles, displaySuffix);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static string ApplyBoldString(string source)
	{
		return "<b>" + source + "</b>";
	}

	public static void AppendBoldString(StringBuilder builder, string source)
	{
		builder.AppendFormat("<b>{0}</b>", source);
	}

	public static float GetRoundedTemperatureInKelvin(float kelvin)
	{
		float num = 0f;
		switch (GameUtil.temperatureUnit)
		{
		case GameUtil.TemperatureUnit.Celsius:
			num = GameUtil.GetTemperatureConvertedToKelvin(Mathf.Round(GameUtil.GetConvertedTemperature(Mathf.Round(kelvin), true)));
			break;
		case GameUtil.TemperatureUnit.Fahrenheit:
			num = GameUtil.GetTemperatureConvertedToKelvin((float)Mathf.RoundToInt(GameUtil.GetTemperatureConvertedFromKelvin(kelvin, GameUtil.TemperatureUnit.Fahrenheit)), GameUtil.TemperatureUnit.Fahrenheit);
			break;
		case GameUtil.TemperatureUnit.Kelvin:
			num = (float)Mathf.RoundToInt(kelvin);
			break;
		}
		return num;
	}

	public static void AppendFormattedTemperature(StringBuilder builder, float temp, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation interpretation = GameUtil.TemperatureInterpretation.Absolute, bool displayUnits = true, bool roundInDestinationFormat = false)
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
		if (Mathf.Abs(temp) < 0.1f)
		{
			builder.AppendFormat("{0:##0.####}", temp);
		}
		else
		{
			builder.AppendFormat("{0:##0.#}", temp);
		}
		if (displayUnits)
		{
			builder.Append(GameUtil.GetTemperatureUnitSuffix());
		}
		GameUtil.AddTimeSliceText(builder, timeSlice);
	}

	public static string GetFormattedTemperature(float temp, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation interpretation = GameUtil.TemperatureInterpretation.Absolute, bool displayUnits = true, bool roundInDestinationFormat = false)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedTemperature(stringBuilder, temp, timeSlice, interpretation, displayUnits, roundInDestinationFormat);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static void AppendFormattedCaloriesForItem(StringBuilder builder, Tag tag, float amount, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool forceKcal = true)
	{
		EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(tag.Name);
		GameUtil.AppendFormattedCalories(builder, (foodInfo != null) ? (foodInfo.CaloriesPerUnit * amount) : (-1f), timeSlice, forceKcal);
	}

	public static string GetFormattedCaloriesForItem(Tag tag, float amount, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool forceKcal = true)
	{
		return GameUtil.GetFormattedCaloriesForItem(tag, amount, true, timeSlice, forceKcal);
	}

	public static string GetFormattedCaloriesForItem(Tag tag, float amount, bool showSuffix, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool forceKcal = true)
	{
		EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(tag.Name);
		return GameUtil.GetFormattedCalories((foodInfo != null) ? (foodInfo.CaloriesPerUnit * amount) : (-1f), showSuffix, timeSlice, forceKcal);
	}

	public static void AppendFormattedCalories(StringBuilder builder, float calories, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool forceKcal = true)
	{
		GameUtil.AppendFormattedCalories(builder, calories, true, timeSlice, forceKcal);
	}

	public static void AppendFormattedCalories(StringBuilder builder, float calories, bool showSuffix, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool forceKcal = true)
	{
		string text = UI.UNITSUFFIXES.CALORIES.CALORIE;
		if (Mathf.Abs(calories) >= 1000f || forceKcal)
		{
			calories /= 1000f;
			text = UI.UNITSUFFIXES.CALORIES.KILOCALORIE;
		}
		calories = GameUtil.ApplyTimeSlice(calories, timeSlice);
		GameUtil.AppendStandardFloat(builder, calories);
		if (showSuffix)
		{
			builder.Append(text);
			GameUtil.AddTimeSliceText(builder, timeSlice);
		}
	}

	public static string GetFormattedCalories(float calories, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool forceKcal = true)
	{
		return GameUtil.GetFormattedCalories(calories, true, timeSlice, forceKcal);
	}

	public static string GetFormattedCalories(float calories, bool showSuffix, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool forceKcal = true)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedCalories(stringBuilder, calories, showSuffix, timeSlice, forceKcal);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static string GetFormattedPreyConsumptionValuePerCycle(Tag preyTag, float crittersPerSecond, bool perCycle = true)
	{
		Assets.GetPrefab(preyTag).GetComponent<PrimaryElement>();
		return GameUtil.GetFormattedUnits(crittersPerSecond, GameUtil.TimeSlice.PerCycle, true, "");
	}

	public static string GetFormattedDirectPlantConsumptionValuePerCycle(Tag plantTag, float consumer_caloriesLossPerCaloriesPerKG, bool perCycle = true)
	{
		IPlantConsumptionInstructions[] plantConsumptionInstructions = GameUtil.GetPlantConsumptionInstructions(Assets.GetPrefab(plantTag));
		if (plantConsumptionInstructions == null || plantConsumptionInstructions.Length == 0)
		{
			return "Error";
		}
		foreach (IPlantConsumptionInstructions plantConsumptionInstructions2 in plantConsumptionInstructions)
		{
			if (plantConsumptionInstructions2.GetDietFoodType() == Diet.Info.FoodType.EatPlantDirectly)
			{
				return plantConsumptionInstructions2.GetFormattedConsumptionPerCycle(consumer_caloriesLossPerCaloriesPerKG);
			}
		}
		return "Error";
	}

	public static string GetFormattedBranchGrowerPlantProductionValuePerCycle(Tag productTag, float outputAmountPerBranch, int branchCount, bool perCycle = true)
	{
		return GameUtil.SafeStringFormat(UI.BUILDINGEFFECTS.TOOLTIPS.BRANCH_GROWER_PLANT_POTENTIAL_OUTPUT, new object[]
		{
			GameUtil.GetFormattedByTag(productTag, outputAmountPerBranch, false, GameUtil.TimeSlice.PerCycle),
			GameUtil.GetFormattedByTag(productTag, outputAmountPerBranch * (float)branchCount, GameUtil.TimeSlice.PerCycle)
		});
	}

	public static string GetFormattedBranchGrowerPlantPlantFiberProductionValuePerCycle(Tag productTag, float outputAmountPerBranch, int branchCount, bool perCycle = true)
	{
		return GameUtil.SafeStringFormat(UI.BUILDINGEFFECTS.TOOLTIPS.BRANCH_GROWER_PLANT_POTENTIAL_OUTPUT, new object[]
		{
			GameUtil.GetFormattedMass(GameUtil.ApplyTimeSlice(outputAmountPerBranch, GameUtil.TimeSlice.PerCycle), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"),
			GameUtil.GetFormattedMass(outputAmountPerBranch * (float)branchCount, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}")
		});
	}

	public static string GetFormattedPlantStorageConsumptionValuePerCycle(Tag plantTag, float consumer_caloriesLossPerCaloriesPerKG, bool perCycle = true)
	{
		IPlantConsumptionInstructions[] plantConsumptionInstructions = GameUtil.GetPlantConsumptionInstructions(Assets.GetPrefab(plantTag));
		if (plantConsumptionInstructions == null || plantConsumptionInstructions.Length == 0)
		{
			return "Error";
		}
		foreach (IPlantConsumptionInstructions plantConsumptionInstructions2 in plantConsumptionInstructions)
		{
			if (plantConsumptionInstructions2.GetDietFoodType() == Diet.Info.FoodType.EatPlantStorage)
			{
				return plantConsumptionInstructions2.GetFormattedConsumptionPerCycle(consumer_caloriesLossPerCaloriesPerKG);
			}
		}
		return "Error";
	}

	public static IPlantConsumptionInstructions[] GetPlantConsumptionInstructions(GameObject prefab)
	{
		IPlantConsumptionInstructions[] components = prefab.GetComponents<IPlantConsumptionInstructions>();
		List<IPlantConsumptionInstructions> allSMI = prefab.GetAllSMI<IPlantConsumptionInstructions>();
		List<IPlantConsumptionInstructions> list = new List<IPlantConsumptionInstructions>();
		if (components != null)
		{
			list.AddRange(components);
		}
		if (allSMI != null)
		{
			list.AddRange(allSMI);
		}
		return list.ToArray();
	}

	public static void AppendFormattedPlantGrowth(StringBuilder builder, float percent, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		percent = GameUtil.ApplyTimeSlice(percent, timeSlice);
		GameUtil.AppendStandardPercentageFloat(builder, percent, true);
		builder.Append(UI.UNITSUFFIXES.PERCENT);
		builder.Append(" ");
		builder.Append(UI.UNITSUFFIXES.GROWTH);
		GameUtil.AddTimeSliceText(builder, timeSlice);
	}

	public static string GetFormattedPlantGrowth(float percent, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedPlantGrowth(stringBuilder, percent, timeSlice);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static void AppendFormattedPercent(StringBuilder builder, float percent, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		GameUtil.AppendStandardPercentageFloat(builder, GameUtil.ApplyTimeSlice(percent, timeSlice), false);
		builder.Append(UI.UNITSUFFIXES.PERCENT);
		GameUtil.AddTimeSliceText(builder, timeSlice);
	}

	public static string GetFormattedPercent(float percent, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedPercent(stringBuilder, percent, timeSlice);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static void AppendFormattedRoundedJoules(StringBuilder builder, float joules)
	{
		if (Mathf.Abs(joules) > 1000f)
		{
			builder.AppendFormat("{0:F1}", joules / 1000f);
			builder.Append(UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE);
			return;
		}
		builder.AppendFormat("{0:F1}", joules);
		builder.Append(UI.UNITSUFFIXES.ELECTRICAL.JOULE);
	}

	public static string GetFormattedRoundedJoules(float joules)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedRoundedJoules(stringBuilder, joules);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static string GetFormattedJoules(float joules, string floatFormat = "F1", GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		if (timeSlice == GameUtil.TimeSlice.PerSecond)
		{
			return GameUtil.GetFormattedWattage(joules, GameUtil.WattageFormatterUnit.Automatic, true);
		}
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

	public static void AppendFormattedRads(StringBuilder builder, float rads, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		rads = GameUtil.ApplyTimeSlice(rads, timeSlice);
		GameUtil.AppendStandardFloat(builder, rads);
		builder.Append(UI.UNITSUFFIXES.RADIATION.RADS);
		GameUtil.AddTimeSliceText(builder, timeSlice);
	}

	public static string GetFormattedRads(float rads, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedRads(stringBuilder, rads, timeSlice);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static void AppendFormattedHighEnergyParticles(StringBuilder builder, float units, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool displayUnits = true)
	{
		GameUtil.AppendFloatWithDecimalPoint(builder, units);
		if (displayUnits)
		{
			builder.Append((units == 1f) ? UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLE : UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES);
		}
		GameUtil.AddTimeSliceText(builder, timeSlice);
	}

	public static string GetFormattedHighEnergyParticles(float units, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool displayUnits = true)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedHighEnergyParticles(stringBuilder, units, timeSlice, displayUnits);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static void AppendFormattedWattage(StringBuilder builder, float watts, GameUtil.WattageFormatterUnit unit = GameUtil.WattageFormatterUnit.Automatic, bool displayUnits = true)
	{
		string text = null;
		switch (unit)
		{
		case GameUtil.WattageFormatterUnit.Watts:
			text = UI.UNITSUFFIXES.ELECTRICAL.WATT;
			break;
		case GameUtil.WattageFormatterUnit.Kilowatts:
			watts /= 1000f;
			text = UI.UNITSUFFIXES.ELECTRICAL.KILOWATT;
			break;
		case GameUtil.WattageFormatterUnit.Automatic:
			if (Mathf.Abs(watts) > 1000f)
			{
				watts /= 1000f;
				text = UI.UNITSUFFIXES.ELECTRICAL.KILOWATT;
			}
			else
			{
				text = UI.UNITSUFFIXES.ELECTRICAL.WATT;
			}
			break;
		}
		GameUtil.AppendFloatToString(builder, watts, "###0.##");
		if (displayUnits && text != null)
		{
			builder.Append(text);
		}
	}

	public static string GetFormattedWattage(float watts, GameUtil.WattageFormatterUnit unit = GameUtil.WattageFormatterUnit.Automatic, bool displayUnits = true)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedWattage(stringBuilder, watts, unit, displayUnits);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static void AppendFormattedHeatEnergy(StringBuilder builder, float dtu, GameUtil.HeatEnergyFormatterUnit unit = GameUtil.HeatEnergyFormatterUnit.Automatic)
	{
		string text;
		string text2;
		switch (unit)
		{
		case GameUtil.HeatEnergyFormatterUnit.DTU_S:
			text = UI.UNITSUFFIXES.HEAT.DTU;
			text2 = "###0.";
			break;
		case GameUtil.HeatEnergyFormatterUnit.KDTU_S:
			dtu /= 1000f;
			text = UI.UNITSUFFIXES.HEAT.KDTU;
			text2 = "###0.##";
			break;
		default:
			if (Mathf.Abs(dtu) > 1000f)
			{
				dtu /= 1000f;
				text = UI.UNITSUFFIXES.HEAT.KDTU;
				text2 = "###0.##";
			}
			else
			{
				text = UI.UNITSUFFIXES.HEAT.DTU;
				text2 = "###0.";
			}
			break;
		}
		GameUtil.AppendFloatToString(builder, dtu, text2);
		builder.Append(text);
	}

	public static string GetFormattedHeatEnergy(float dtu, GameUtil.HeatEnergyFormatterUnit unit = GameUtil.HeatEnergyFormatterUnit.Automatic)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedHeatEnergy(stringBuilder, dtu, unit);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static void AppendFormattedHeatEnergyRate(StringBuilder builder, float dtu_s, GameUtil.HeatEnergyFormatterUnit unit = GameUtil.HeatEnergyFormatterUnit.Automatic)
	{
		string text = null;
		switch (unit)
		{
		case GameUtil.HeatEnergyFormatterUnit.DTU_S:
			text = UI.UNITSUFFIXES.HEAT.DTU_S;
			break;
		case GameUtil.HeatEnergyFormatterUnit.KDTU_S:
			dtu_s /= 1000f;
			text = UI.UNITSUFFIXES.HEAT.KDTU_S;
			break;
		case GameUtil.HeatEnergyFormatterUnit.Automatic:
			if (Mathf.Abs(dtu_s) > 1000f)
			{
				dtu_s /= 1000f;
				text = UI.UNITSUFFIXES.HEAT.KDTU_S;
			}
			else
			{
				text = UI.UNITSUFFIXES.HEAT.DTU_S;
			}
			break;
		}
		GameUtil.AppendFloatToString(builder, dtu_s, null);
		if (text != null)
		{
			builder.Append(text);
		}
	}

	public static string GetFormattedHeatEnergyRate(float dtu_s, GameUtil.HeatEnergyFormatterUnit unit = GameUtil.HeatEnergyFormatterUnit.Automatic)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedHeatEnergyRate(stringBuilder, dtu_s, unit);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static string GetFormattedInt(float num, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		num = GameUtil.ApplyTimeSlice(num, timeSlice);
		return GameUtil.AddTimeSliceText(GameUtil.FloatToString(num, "F0"), timeSlice);
	}

	public static string GetSpeciesNameFromGameObject(GameObject critterGameObject)
	{
		CreatureBrain component = critterGameObject.GetComponent<CreatureBrain>();
		if (component != null)
		{
			return GameUtil.GetNameForSpecies(component.species);
		}
		return "UNKNOWN SPECIES";
	}

	public static string GetNameForSpecies(Tag species)
	{
		Option<string> option = Option.None;
		if (species == GameTags.Creatures.Species.HatchSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.HATCHSPECIES);
		}
		else if (species == GameTags.Creatures.Species.LightBugSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.LIGHTBUGSPECIES);
		}
		else if (species == GameTags.Creatures.Species.OilFloaterSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.OILFLOATERSPECIES);
		}
		else if (species == GameTags.Creatures.Species.DreckoSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.DRECKOSPECIES);
		}
		else if (species == GameTags.Creatures.Species.GlomSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.GLOMSPECIES);
		}
		else if (species == GameTags.Creatures.Species.PuftSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.PUFTSPECIES);
		}
		else if (species == GameTags.Creatures.Species.PacuSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.PACUSPECIES);
		}
		else if (species == GameTags.Creatures.Species.MooSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.MOOSPECIES);
		}
		else if (species == GameTags.Creatures.Species.MoleSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.MOLESPECIES);
		}
		else if (species == GameTags.Creatures.Species.SquirrelSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.SQUIRRELSPECIES);
		}
		else if (species == GameTags.Creatures.Species.CrabSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.CRABSPECIES);
		}
		else if (species == GameTags.Creatures.Species.DivergentSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.DIVERGENTSPECIES);
		}
		else if (species == GameTags.Creatures.Species.StaterpillarSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.STATERPILLARSPECIES);
		}
		else if (species == GameTags.Creatures.Species.BeetaSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.BEETASPECIES);
		}
		else if (species == GameTags.Creatures.Species.BellySpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.BELLYSPECIES);
		}
		else if (species == GameTags.Creatures.Species.SealSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.SEALSPECIES);
		}
		else if (species == GameTags.Creatures.Species.DeerSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.DEERSPECIES);
		}
		else if (species == GameTags.Creatures.Species.RaptorSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.RAPTORSPECIES);
		}
		else if (species == GameTags.Creatures.Species.ChameleonSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.CHAMELEONSPECIES);
		}
		else if (species == GameTags.Creatures.Species.PrehistoricPacuSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.PREHISTORICPACUSPECIES);
		}
		else if (species == GameTags.Creatures.Species.StegoSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.STEGOSPECIES);
		}
		else if (species == GameTags.Creatures.Species.ButterflySpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.BUTTERFLYSPECIES);
		}
		else if (species == GameTags.Creatures.Species.ParrotFishSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.PARROTFISHSPECIES);
		}
		else if (species == GameTags.Creatures.Species.SnailSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.SNAILSPECIES);
		}
		else if (species == GameTags.Creatures.Species.SquidSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.SQUIDSPECIES);
		}
		else if (species == GameTags.Creatures.Species.PufferFishSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.PUFFERFISHSPECIES);
		}
		else if (species == GameTags.Creatures.Species.SeaFairySpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.SEAFAIRYSPECIES);
		}
		else if (species == GameTags.Creatures.Species.SeaTurtleSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.SEATURTLESPECIES);
		}
		else if (species == GameTags.Creatures.Species.SeaHorseSpecies)
		{
			option = Option.Some<string>(global::STRINGS.CREATURES.FAMILY_PLURAL.SEAHORSESPECIES);
		}
		else
		{
			option = Option.None;
		}
		return option.Value;
	}

	public static void AppendFormattedSimple(StringBuilder builder, float num, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, string formatString = null)
	{
		num = GameUtil.ApplyTimeSlice(num, timeSlice);
		if (formatString != null)
		{
			GameUtil.AppendFloatToString(builder, num, formatString);
		}
		else if (num == 0f)
		{
			builder.Append("0");
		}
		else if (Mathf.Abs(num) < 1f)
		{
			GameUtil.AppendFloatToString(builder, num, "#,##0.##");
		}
		else if (Mathf.Abs(num) < 10f)
		{
			GameUtil.AppendFloatToString(builder, num, "#,###.##");
		}
		else
		{
			GameUtil.AppendFloatToString(builder, num, "#,###.##");
		}
		GameUtil.AddTimeSliceText(builder, timeSlice);
	}

	public static string GetFormattedSimple(float num, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, string formatString = null)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedSimple(stringBuilder, num, timeSlice, formatString);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static void AppendFormattedLux(StringBuilder builder, int lux)
	{
		builder.Append(lux);
		builder.Append(UI.UNITSUFFIXES.LIGHT.LUX);
	}

	public static string GetFormattedLux(int lux)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedLux(stringBuilder, lux);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static string GetLightDescription(int lux)
	{
		if (lux == 0)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.NO_LIGHT;
		}
		if (lux < DUPLICANTSTATS.STANDARD.Light.LOW_LIGHT)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.VERY_LOW_LIGHT;
		}
		if (lux < DUPLICANTSTATS.STANDARD.Light.MEDIUM_LIGHT)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.LOW_LIGHT;
		}
		if (lux < DUPLICANTSTATS.STANDARD.Light.HIGH_LIGHT)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.MEDIUM_LIGHT;
		}
		if (lux < DUPLICANTSTATS.STANDARD.Light.VERY_HIGH_LIGHT)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.HIGH_LIGHT;
		}
		if (lux < DUPLICANTSTATS.STANDARD.Light.MAX_LIGHT)
		{
			return UI.OVERLAYS.LIGHTING.RANGES.VERY_HIGH_LIGHT;
		}
		return UI.OVERLAYS.LIGHTING.RANGES.MAX_LIGHT;
	}

	public static string GetRadiationDescription(float radsPerCycle)
	{
		if (radsPerCycle == 0f)
		{
			return UI.OVERLAYS.RADIATION.RANGES.NONE;
		}
		if (radsPerCycle < 100f)
		{
			return UI.OVERLAYS.RADIATION.RANGES.VERY_LOW;
		}
		if (radsPerCycle < 200f)
		{
			return UI.OVERLAYS.RADIATION.RANGES.LOW;
		}
		if (radsPerCycle < 400f)
		{
			return UI.OVERLAYS.RADIATION.RANGES.MEDIUM;
		}
		if (radsPerCycle < 2000f)
		{
			return UI.OVERLAYS.RADIATION.RANGES.HIGH;
		}
		if (radsPerCycle < 4000f)
		{
			return UI.OVERLAYS.RADIATION.RANGES.VERY_HIGH;
		}
		return UI.OVERLAYS.RADIATION.RANGES.MAX;
	}

	public static void AppendFormattedByTag(StringBuilder builder, Tag tag, float amount, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		if (GameTags.DisplayAsCalories.Contains(tag))
		{
			GameUtil.AppendFormattedCaloriesForItem(builder, tag, amount, timeSlice, true);
			return;
		}
		if (GameTags.DisplayAsUnits.Contains(tag))
		{
			GameUtil.AppendFormattedUnits(builder, amount, timeSlice, true, "");
			return;
		}
		GameUtil.AppendFormattedMass(builder, amount, timeSlice, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
	}

	public static string GetFormattedByTag(Tag tag, float amount, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		return GameUtil.GetFormattedByTag(tag, amount, true, timeSlice);
	}

	public static string GetFormattedByTag(Tag tag, float amount, bool showSuffix, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		if (GameTags.DisplayAsCalories.Contains(tag))
		{
			return GameUtil.GetFormattedCaloriesForItem(tag, amount, showSuffix, timeSlice, true);
		}
		if (GameTags.DisplayAsUnits.Contains(tag))
		{
			return GameUtil.GetFormattedUnits(amount, timeSlice, showSuffix, "");
		}
		return GameUtil.GetFormattedMass(amount, timeSlice, GameUtil.MetricMassFormat.UseThreshold, showSuffix, "{0:0.#}");
	}

	public static string GetFormattedFoodQuality(int quality)
	{
		if (GameUtil.adjectives == null)
		{
			GameUtil.adjectives = LocString.GetStrings(typeof(DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVES));
		}
		LocString locString = ((quality >= 0) ? DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_FORMAT_POSITIVE : DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_FORMAT_NEGATIVE);
		int num = quality - DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_INDEX_OFFSET;
		num = Mathf.Clamp(num, 0, GameUtil.adjectives.Length);
		return string.Format(locString, GameUtil.adjectives[num], GameUtil.AddPositiveSign(quality.ToString(), quality > 0));
	}

	public static string GetFormattedBytes(ulong amount)
	{
		string[] array = new string[]
		{
			UI.UNITSUFFIXES.INFORMATION.BYTE,
			UI.UNITSUFFIXES.INFORMATION.KILOBYTE,
			UI.UNITSUFFIXES.INFORMATION.MEGABYTE,
			UI.UNITSUFFIXES.INFORMATION.GIGABYTE,
			UI.UNITSUFFIXES.INFORMATION.TERABYTE
		};
		int num = ((amount == 0UL) ? 0 : ((int)Math.Floor(Math.Floor(Math.Log(amount)) / Math.Log(1024.0))));
		double num2 = amount / Math.Pow(1024.0, (double)num);
		global::Debug.Assert(num >= 0 && num < array.Length);
		return string.Format("{0:F} {1}", num2, array[num]);
	}

	public static string GetFormattedInfomation(float amount, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		amount = GameUtil.ApplyTimeSlice(amount, timeSlice);
		string text = "";
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
		return GameUtil.AddTimeSliceText(amount.ToString() + text, timeSlice);
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

	public static void AppendFormattedMass(StringBuilder builder, float mass, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, GameUtil.MetricMassFormat massFormat = GameUtil.MetricMassFormat.UseThreshold, bool includeSuffix = true, string floatFormat = "{0:0.#}")
	{
		if (mass == -3.4028235E+38f)
		{
			builder.Append(UI.CALCULATING);
			return;
		}
		if (float.IsPositiveInfinity(mass))
		{
			builder.Append(UI.POS_INFINITY);
			builder.Append(UI.UNITSUFFIXES.MASS.TONNE);
			return;
		}
		if (float.IsNegativeInfinity(mass))
		{
			builder.Append(UI.NEG_INFINITY);
			builder.Append(UI.UNITSUFFIXES.MASS.TONNE);
			return;
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
		builder.AppendFormat(floatFormat, mass);
		if (includeSuffix)
		{
			builder.Append(text);
			GameUtil.AddTimeSliceText(builder, timeSlice);
		}
	}

	public static string GetFormattedMass(float mass, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, GameUtil.MetricMassFormat massFormat = GameUtil.MetricMassFormat.UseThreshold, bool includeSuffix = true, string floatFormat = "{0:0.#}")
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedMass(stringBuilder, mass, timeSlice, massFormat, includeSuffix, floatFormat);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static void AppendFormattedTime(StringBuilder builder, float seconds)
	{
		builder.AppendFormat(UI.FORMATSECONDS, (int)seconds);
	}

	public static string GetFormattedTime(float seconds, string floatFormat = "F0")
	{
		return string.Format(UI.FORMATSECONDS, seconds.ToString(floatFormat));
	}

	public static void AppendFormattedEngineEfficiency(StringBuilder builder, float amount)
	{
		builder.Append(amount);
		builder.Append(" km /");
		builder.Append(UI.UNITSUFFIXES.MASS.KILOGRAM);
	}

	public static string GetFormattedEngineEfficiency(float amount)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedEngineEfficiency(stringBuilder, amount);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static void AppendFormattedDistance(StringBuilder builder, float meters)
	{
		if (Mathf.Abs(meters) < 1f)
		{
			builder.AppendFormat("{0:0.0} cm", Math.Abs(meters * 100f));
			return;
		}
		if (meters < 1000f)
		{
			builder.Append(meters);
			builder.Append(" m");
			return;
		}
		builder.AppendFormat("{0:0.0} km", meters / 1000f);
	}

	public static string GetFormattedDistance(float meters)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedDistance(stringBuilder, meters);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static void AppendFormattedCycles(StringBuilder builder, float seconds, bool forceCycles = false)
	{
		if (forceCycles || Math.Abs(seconds) > 100f)
		{
			builder.AppendFormat(UI.FORMATDAY, seconds / 600f);
			return;
		}
		GameUtil.AppendFormattedTime(builder, seconds);
	}

	public static string GetFormattedCycles(float seconds, string formatString = "F1", bool forceCycles = false)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendFormattedCycles(stringBuilder, seconds, forceCycles);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
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

	public static string SafeStringFormat(string source, params object[] args)
	{
		for (int i = 0; i < args.Length; i++)
		{
			string text = "{" + i.ToString() + "}";
			if (!source.Contains(text))
			{
				KCrashReporter.ReportDevNotification(string.Format("Format error in string: \"{0}\". Source is missing the {{{1}}} format marker for argument \"{2}\" insertion.", source, i, args[i]), Environment.StackTrace, "", false, null);
			}
			else
			{
				source = source.Replace(text, args[i].ToString());
			}
		}
		return source;
	}

	public static bool HasTrait(GameObject go, string traitName)
	{
		Traits component = go.GetComponent<Traits>();
		return !(component == null) && component.HasTrait(traitName);
	}

	public static float GetRadiationAbsorptionPercentage(int cell)
	{
		if (Grid.IsValidCell(cell))
		{
			return GameUtil.GetRadiationAbsorptionPercentage(Grid.Element[cell], Grid.Mass[cell], Grid.IsSolidCell(cell) && (Grid.Properties[cell] & 128) == 128);
		}
		return 0f;
	}

	public static float GetRadiationAbsorptionPercentage(Element elem, float mass, bool isConstructed)
	{
		float num = 2000f;
		float num2 = 0.3f;
		float num3 = 0.7f;
		float num4 = 0.8f;
		float num5;
		if (isConstructed)
		{
			num5 = elem.radiationAbsorptionFactor * num4;
		}
		else
		{
			num5 = elem.radiationAbsorptionFactor * num2 + mass / num * elem.radiationAbsorptionFactor * num3;
		}
		return Mathf.Clamp(num5, 0f, 1f);
	}

	public static void AppendHardnessString(StringBuilder builder, Element element, bool addColor = true)
	{
		if (!element.IsSolid)
		{
			builder.Append(ELEMENTS.HARDNESS.NA);
			return;
		}
		Color color = GameUtil.Hardness.firmColor;
		string text;
		if (element.hardness >= 255)
		{
			color = GameUtil.Hardness.ImpenetrableColor;
			text = ELEMENTS.HARDNESS.IMPENETRABLE;
		}
		else if (element.hardness >= 150)
		{
			color = GameUtil.Hardness.nearlyImpenetrableColor;
			text = ELEMENTS.HARDNESS.NEARLYIMPENETRABLE;
		}
		else if (element.hardness >= 50)
		{
			color = GameUtil.Hardness.veryFirmColor;
			text = ELEMENTS.HARDNESS.VERYFIRM;
		}
		else if (element.hardness >= 25)
		{
			color = GameUtil.Hardness.firmColor;
			text = ELEMENTS.HARDNESS.FIRM;
		}
		else if (element.hardness >= 10)
		{
			color = GameUtil.Hardness.softColor;
			text = ELEMENTS.HARDNESS.SOFT;
		}
		else
		{
			color = GameUtil.Hardness.verySoftColor;
			text = ELEMENTS.HARDNESS.VERYSOFT;
		}
		if (addColor)
		{
			builder.AppendFormat("<color=#{0}>", color.ToHexString());
		}
		builder.AppendFormat(text, element.hardness);
		if (addColor)
		{
			builder.Append("</color>");
		}
	}

	public static string GetHardnessString(Element element, bool addColor = true)
	{
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		GameUtil.AppendHardnessString(stringBuilder, element, addColor);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
	}

	public static string GetGermResistanceModifierString(float modifier, bool addColor = true)
	{
		Color color = Color.black;
		string text = "";
		if (modifier > 0f)
		{
			if (modifier >= 5f)
			{
				color = GameUtil.GermResistanceValues.PositiveLargeColor;
				text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.POSITIVE_LARGE, modifier);
			}
			else if (modifier >= 2f)
			{
				color = GameUtil.GermResistanceValues.PositiveMediumColor;
				text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.POSITIVE_MEDIUM, modifier);
			}
			else if (modifier > 0f)
			{
				color = GameUtil.GermResistanceValues.PositiveSmallColor;
				text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.POSITIVE_SMALL, modifier);
			}
		}
		else if (modifier < 0f)
		{
			if (modifier <= -5f)
			{
				color = GameUtil.GermResistanceValues.NegativeLargeColor;
				text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NEGATIVE_LARGE, modifier);
			}
			else if (modifier <= -2f)
			{
				color = GameUtil.GermResistanceValues.NegativeMediumColor;
				text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NEGATIVE_MEDIUM, modifier);
			}
			else if (modifier < 0f)
			{
				color = GameUtil.GermResistanceValues.NegativeSmallColor;
				text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NEGATIVE_SMALL, modifier);
			}
		}
		else
		{
			addColor = false;
			text = string.Format(DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NONE, modifier);
		}
		if (addColor)
		{
			text = string.Format("<color=#{0}>{1}</color>", color.ToHexString(), text);
		}
		return text;
	}

	public static string GetThermalConductivityString(Element element, bool addColor = true, bool addValue = true)
	{
		Color color = GameUtil.ThermalConductivityValues.mediumConductivityColor;
		string text;
		if (element.thermalConductivity >= 50f)
		{
			color = GameUtil.ThermalConductivityValues.veryHighConductivityColor;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VERY_HIGH_CONDUCTIVITY;
		}
		else if (element.thermalConductivity >= 10f)
		{
			color = GameUtil.ThermalConductivityValues.highConductivityColor;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.HIGH_CONDUCTIVITY;
		}
		else if (element.thermalConductivity >= 2f)
		{
			color = GameUtil.ThermalConductivityValues.mediumConductivityColor;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.MEDIUM_CONDUCTIVITY;
		}
		else if (element.thermalConductivity >= 1f)
		{
			color = GameUtil.ThermalConductivityValues.lowConductivityColor;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.LOW_CONDUCTIVITY;
		}
		else
		{
			color = GameUtil.ThermalConductivityValues.veryLowConductivityColor;
			text = UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VERY_LOW_CONDUCTIVITY;
		}
		if (addColor)
		{
			text = string.Format("<color=#{0}>{1}</color>", color.ToHexString(), text);
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
			return "";
		}
		Color color = GameUtil.BreathableValues.positiveColor;
		SimHashes id = element.id;
		LocString locString;
		if (id != SimHashes.Oxygen)
		{
			if (id != SimHashes.ContaminatedOxygen)
			{
				color = GameUtil.BreathableValues.negativeColor;
				locString = UI.OVERLAYS.OXYGEN.LEGEND4;
			}
			else if (Mass >= SimDebugView.optimallyBreathable)
			{
				color = GameUtil.BreathableValues.positiveColor;
				locString = UI.OVERLAYS.OXYGEN.LEGEND1;
			}
			else if (Mass >= SimDebugView.minimumBreathable + (SimDebugView.optimallyBreathable - SimDebugView.minimumBreathable) / 2f)
			{
				color = GameUtil.BreathableValues.positiveColor;
				locString = UI.OVERLAYS.OXYGEN.LEGEND2;
			}
			else if (Mass >= SimDebugView.minimumBreathable)
			{
				color = GameUtil.BreathableValues.warningColor;
				locString = UI.OVERLAYS.OXYGEN.LEGEND3;
			}
			else
			{
				color = GameUtil.BreathableValues.negativeColor;
				locString = UI.OVERLAYS.OXYGEN.LEGEND4;
			}
		}
		else if (Mass >= SimDebugView.optimallyBreathable)
		{
			color = GameUtil.BreathableValues.positiveColor;
			locString = UI.OVERLAYS.OXYGEN.LEGEND1;
		}
		else if (Mass >= SimDebugView.minimumBreathable + (SimDebugView.optimallyBreathable - SimDebugView.minimumBreathable) / 2f)
		{
			color = GameUtil.BreathableValues.positiveColor;
			locString = UI.OVERLAYS.OXYGEN.LEGEND2;
		}
		else if (Mass >= SimDebugView.minimumBreathable)
		{
			color = GameUtil.BreathableValues.warningColor;
			locString = UI.OVERLAYS.OXYGEN.LEGEND3;
		}
		else
		{
			color = GameUtil.BreathableValues.negativeColor;
			locString = UI.OVERLAYS.OXYGEN.LEGEND4;
		}
		return string.Format(ELEMENTS.BREATHABLEDESC, color.ToHexString(), locString);
	}

	public static string GetWireLoadColor(float load, float maxLoad, float potentialLoad)
	{
		Color color;
		if (load > maxLoad + POWER.FLOAT_FUDGE_FACTOR)
		{
			color = GameUtil.WireLoadValues.negativeColor;
		}
		else if (potentialLoad > maxLoad && load / maxLoad >= 0.75f)
		{
			color = GameUtil.WireLoadValues.warningColor;
		}
		else
		{
			color = Color.white;
		}
		return color.ToHexString();
	}

	public static string GetHotkeyString(global::Action action)
	{
		if (KInputManager.currentControllerIsGamepad)
		{
			return UI.FormatAsHotkey(GameUtil.GetActionString(action));
		}
		return UI.FormatAsHotkey("[" + GameUtil.GetActionString(action) + "]");
	}

	public static string ReplaceHotkeyString(string template, global::Action action)
	{
		return template.Replace("{Hotkey}", GameUtil.GetHotkeyString(action));
	}

	public static string ReplaceHotkeyString(string template, global::Action action1, global::Action action2)
	{
		return template.Replace("{Hotkey}", GameUtil.GetHotkeyString(action1) + GameUtil.GetHotkeyString(action2));
	}

	public static string GetKeycodeLocalized(KKeyCode key_code)
	{
		string text = key_code.ToString();
		if (key_code <= KKeyCode.Slash)
		{
			if (key_code <= KKeyCode.Tab)
			{
				if (key_code == KKeyCode.None)
				{
					return text;
				}
				if (key_code == KKeyCode.Backspace)
				{
					return INPUT.BACKSPACE;
				}
				if (key_code == KKeyCode.Tab)
				{
					return INPUT.TAB;
				}
			}
			else if (key_code <= KKeyCode.Escape)
			{
				if (key_code == KKeyCode.Return)
				{
					return INPUT.ENTER;
				}
				if (key_code == KKeyCode.Escape)
				{
					return INPUT.ESCAPE;
				}
			}
			else
			{
				if (key_code == KKeyCode.Space)
				{
					return INPUT.SPACE;
				}
				switch (key_code)
				{
				case KKeyCode.Plus:
					return "+";
				case KKeyCode.Comma:
					return ",";
				case KKeyCode.Minus:
					return "-";
				case KKeyCode.Period:
					return INPUT.PERIOD;
				case KKeyCode.Slash:
					return "/";
				}
			}
		}
		else if (key_code <= KKeyCode.Insert)
		{
			switch (key_code)
			{
			case KKeyCode.Colon:
				return ":";
			case KKeyCode.Semicolon:
				return ";";
			case KKeyCode.Less:
				break;
			case KKeyCode.Equals:
				return "=";
			default:
				switch (key_code)
				{
				case KKeyCode.LeftBracket:
					return "[";
				case KKeyCode.Backslash:
					return "\\";
				case KKeyCode.RightBracket:
					return "]";
				case KKeyCode.Caret:
				case KKeyCode.Underscore:
					break;
				case KKeyCode.BackQuote:
					return INPUT.BACKQUOTE;
				default:
					switch (key_code)
					{
					case KKeyCode.Keypad0:
						return INPUT.NUM + " 0";
					case KKeyCode.Keypad1:
						return INPUT.NUM + " 1";
					case KKeyCode.Keypad2:
						return INPUT.NUM + " 2";
					case KKeyCode.Keypad3:
						return INPUT.NUM + " 3";
					case KKeyCode.Keypad4:
						return INPUT.NUM + " 4";
					case KKeyCode.Keypad5:
						return INPUT.NUM + " 5";
					case KKeyCode.Keypad6:
						return INPUT.NUM + " 6";
					case KKeyCode.Keypad7:
						return INPUT.NUM + " 7";
					case KKeyCode.Keypad8:
						return INPUT.NUM + " 8";
					case KKeyCode.Keypad9:
						return INPUT.NUM + " 9";
					case KKeyCode.KeypadPeriod:
						return INPUT.NUM + " " + INPUT.PERIOD;
					case KKeyCode.KeypadDivide:
						return INPUT.NUM + " /";
					case KKeyCode.KeypadMultiply:
						return INPUT.NUM + " *";
					case KKeyCode.KeypadMinus:
						return INPUT.NUM + " -";
					case KKeyCode.KeypadPlus:
						return INPUT.NUM + " +";
					case KKeyCode.KeypadEnter:
						return INPUT.NUM + " " + INPUT.ENTER;
					case KKeyCode.Insert:
						return INPUT.INSERT;
					}
					break;
				}
				break;
			}
		}
		else if (key_code <= KKeyCode.Mouse6)
		{
			switch (key_code)
			{
			case KKeyCode.RightShift:
				return INPUT.RIGHT_SHIFT;
			case KKeyCode.LeftShift:
				return INPUT.LEFT_SHIFT;
			case KKeyCode.RightControl:
				return INPUT.RIGHT_CTRL;
			case KKeyCode.LeftControl:
				return INPUT.LEFT_CTRL;
			case KKeyCode.RightAlt:
				return INPUT.RIGHT_ALT;
			case KKeyCode.LeftAlt:
				return INPUT.LEFT_ALT;
			default:
				switch (key_code)
				{
				case KKeyCode.Mouse0:
					return INPUT.MOUSE + " 0";
				case KKeyCode.Mouse1:
					return INPUT.MOUSE + " 1";
				case KKeyCode.Mouse2:
					return INPUT.MOUSE + " 2";
				case KKeyCode.Mouse3:
					return INPUT.MOUSE + " 3";
				case KKeyCode.Mouse4:
					return INPUT.MOUSE + " 4";
				case KKeyCode.Mouse5:
					return INPUT.MOUSE + " 5";
				case KKeyCode.Mouse6:
					return INPUT.MOUSE + " 6";
				}
				break;
			}
		}
		else
		{
			if (key_code == KKeyCode.MouseScrollDown)
			{
				return INPUT.MOUSE_SCROLL_DOWN;
			}
			if (key_code == KKeyCode.MouseScrollUp)
			{
				return INPUT.MOUSE_SCROLL_UP;
			}
		}
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
		return text;
	}

	public static string GetActionString(global::Action action)
	{
		string text = "";
		if (action == global::Action.NumActions)
		{
			return text;
		}
		BindingEntry bindingEntry = GameUtil.ActionToBinding(action);
		KKeyCode mKeyCode = bindingEntry.mKeyCode;
		if (KInputManager.currentControllerIsGamepad)
		{
			return KInputManager.steamInputInterpreter.GetActionGlyph(action);
		}
		if (bindingEntry.mModifier == global::Modifier.None)
		{
			return GameUtil.GetKeycodeLocalized(mKeyCode).ToUpper();
		}
		string text2 = "";
		global::Modifier mModifier = bindingEntry.mModifier;
		switch (mModifier)
		{
		case global::Modifier.Alt:
			text2 = INPUT.ALT.ToString();
			break;
		case global::Modifier.Ctrl:
			text2 = INPUT.CTRL.ToString();
			break;
		case (global::Modifier)3:
			break;
		case global::Modifier.Shift:
			text2 = INPUT.SHIFT.ToString();
			break;
		default:
			if (mModifier != global::Modifier.CapsLock)
			{
				if (mModifier == global::Modifier.Backtick)
				{
					text2 = GameUtil.GetKeycodeLocalized(KKeyCode.BackQuote);
				}
			}
			else
			{
				text2 = GameUtil.GetKeycodeLocalized(KKeyCode.CapsLock);
			}
			break;
		}
		return (text2 + " + " + GameUtil.GetKeycodeLocalized(mKeyCode)).ToUpper();
	}

	public static void CreateExplosion(Vector3 explosion_pos)
	{
		Vector2 vector = new Vector2(explosion_pos.x, explosion_pos.y);
		float num = 5f;
		float num2 = num * num;
		foreach (Health health in Components.Health.Items)
		{
			Vector3 position = health.transform.GetPosition();
			float sqrMagnitude = (new Vector2(position.x, position.y) - vector).sqrMagnitude;
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

	public static float GetMaxStressInActiveWorld()
	{
		if (Components.LiveMinionIdentities.Count <= 0)
		{
			return 0f;
		}
		float num = 0f;
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
		{
			if (!minionIdentity.IsNullOrDestroyed() && minionIdentity.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
			{
				AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup(minionIdentity);
				if (amountInstance != null)
				{
					num = Mathf.Max(num, amountInstance.value);
				}
			}
		}
		return num;
	}

	public static float GetAverageStressInActiveWorld()
	{
		if (Components.LiveMinionIdentities.Count <= 0)
		{
			return 0f;
		}
		float num = 0f;
		int num2 = 0;
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
		{
			if (!minionIdentity.IsNullOrDestroyed() && minionIdentity.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
			{
				num += Db.Get().Amounts.Stress.Lookup(minionIdentity).value;
				num2++;
			}
		}
		return num / (float)num2;
	}

	public static string MigrateFMOD(FMODAsset asset)
	{
		if (asset == null)
		{
			return null;
		}
		if (asset.path == null)
		{
			return asset.name;
		}
		return asset.path;
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
		return GameUtil.GetRequirementDescriptors(descriptors, true);
	}

	public static List<Descriptor> GetRequirementDescriptors(List<Descriptor> descriptors, bool indent)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in descriptors)
		{
			if (descriptor.type == Descriptor.DescriptorType.Requirement)
			{
				list.Add(descriptor);
			}
		}
		if (indent)
		{
			GameUtil.IndentListOfDescriptors(list, 1);
		}
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
					if ((!descriptor.onlyForSimpleInfoScreen || simpleInfoScreen) && (descriptor.type == Descriptor.DescriptorType.Effect || descriptor.type == Descriptor.DescriptorType.DiseaseSource))
					{
						list.Add(descriptor);
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

	public static void PartitionBuildingDescriptors(GameObject buildingComplete, bool simpleInfoScreen, out List<Descriptor> allDescs, [TupleElementNames(new string[] { "converter", "descriptors" })] List<ValueTuple<ElementConverter, List<Descriptor>>> converterDescCache, List<Descriptor> nonConverterReqs, List<Descriptor> nonConverterEffects, out bool hasConverterReqs)
	{
		ElementConverter[] components = buildingComplete.GetComponents<ElementConverter>();
		allDescs = GameUtil.GetAllDescriptors(buildingComplete, simpleInfoScreen);
		hasConverterReqs = false;
		HashSetPool<string, BuildingDef>.PooledHashSet pooledHashSet = HashSetPool<string, BuildingDef>.Allocate();
		foreach (ElementConverter elementConverter in components)
		{
			if (elementConverter.consumedElements != null && elementConverter.consumedElements.Length != 0)
			{
				List<Descriptor> descriptors = elementConverter.GetDescriptors(buildingComplete);
				converterDescCache.Add(new ValueTuple<ElementConverter, List<Descriptor>>(elementConverter, descriptors));
				if (descriptors != null)
				{
					foreach (Descriptor descriptor in descriptors)
					{
						pooledHashSet.Add(descriptor.text);
						if (descriptor.type == Descriptor.DescriptorType.Requirement)
						{
							hasConverterReqs = true;
						}
					}
				}
			}
		}
		IConverterByproduct defImplementingInterface = buildingComplete.GetDefImplementingInterface<IConverterByproduct>();
		if (defImplementingInterface != null && defImplementingInterface.ByproductRate > 0f)
		{
			foreach (ValueTuple<ElementConverter, List<Descriptor>> valueTuple in converterDescCache)
			{
				ElementConverter item = valueTuple.Item1;
				List<Descriptor> item2 = valueTuple.Item2;
				bool flag = false;
				ElementConverter.ConsumedElement[] consumedElements = item.consumedElements;
				for (int i = 0; i < consumedElements.Length; i++)
				{
					if (consumedElements[i].Tag == defImplementingInterface.ByproductAssociatedInputTag)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					defImplementingInterface.GetByproductDescriptors(buildingComplete, item2);
					break;
				}
			}
		}
		foreach (Descriptor descriptor2 in allDescs)
		{
			if (!pooledHashSet.Contains(descriptor2.text))
			{
				switch (descriptor2.type)
				{
				case Descriptor.DescriptorType.Requirement:
					nonConverterReqs.Add(descriptor2);
					break;
				case Descriptor.DescriptorType.Effect:
				case Descriptor.DescriptorType.DiseaseSource:
					nonConverterEffects.Add(descriptor2);
					break;
				}
			}
		}
		pooledHashSet.Recycle();
	}

	public static void BuildPartitionedRequirements(List<Descriptor> result, List<Descriptor> nonConverterReqs, [TupleElementNames(new string[] { "converter", "descriptors" })] List<ValueTuple<ElementConverter, List<Descriptor>>> converterDescCache, bool hasConverterReqs)
	{
		foreach (Descriptor descriptor in nonConverterReqs)
		{
			descriptor.IncreaseIndent();
			result.Add(descriptor);
		}
		if (hasConverterReqs)
		{
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(UI.BUILDINGEFFECTS.OPERATIONINPUTS, "", Descriptor.DescriptorType.Requirement);
			descriptor2.IncreaseIndent();
			result.Add(descriptor2);
		}
		foreach (ValueTuple<ElementConverter, List<Descriptor>> valueTuple in converterDescCache)
		{
			foreach (Descriptor descriptor3 in valueTuple.Item2)
			{
				if (descriptor3.type == Descriptor.DescriptorType.Requirement)
				{
					Descriptor descriptor4 = descriptor3;
					descriptor4.text = "• " + descriptor4.text;
					descriptor4.IncreaseIndent();
					descriptor4.IncreaseIndent();
					result.Add(descriptor4);
				}
			}
		}
	}

	public static void BuildPartitionedEffects(List<Descriptor> result, List<Descriptor> nonConverterEffects, [TupleElementNames(new string[] { "converter", "descriptors" })] List<ValueTuple<ElementConverter, List<Descriptor>>> converterDescCache)
	{
		foreach (Descriptor descriptor in nonConverterEffects)
		{
			descriptor.IncreaseIndent();
			result.Add(descriptor);
		}
		foreach (ValueTuple<ElementConverter, List<Descriptor>> valueTuple in converterDescCache)
		{
			ElementConverter item = valueTuple.Item1;
			List<Descriptor> item2 = valueTuple.Item2;
			string text = item.consumedElements[0].Name;
			for (int i = 1; i < item.consumedElements.Length; i++)
			{
				text = text + ", " + item.consumedElements[i].Name;
			}
			Descriptor descriptor2 = new Descriptor(text + ":", "", Descriptor.DescriptorType.Effect, false);
			descriptor2.IncreaseIndent();
			result.Add(descriptor2);
			foreach (Descriptor descriptor3 in item2)
			{
				if (descriptor3.type != Descriptor.DescriptorType.Requirement)
				{
					Descriptor descriptor4 = descriptor3;
					descriptor4.IncreaseIndent();
					descriptor4.IncreaseIndent();
					result.Add(descriptor4);
				}
			}
		}
	}

	public static List<Descriptor> GetPlantRequirementDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<Descriptor> requirementDescriptors = GameUtil.GetRequirementDescriptors(GameUtil.GetAllDescriptors(go, false));
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
		string text = "";
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
				string name = Db.Get().Attributes.Get(attributeModifier.AttributeId).Name;
				string formattedString = attributeModifier.GetFormattedString();
				string text = ((attributeModifier.Value >= 0f) ? "produced" : "consumed");
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
			text = RESEARCH.TYPES.MISSINGRECIPEDESC;
			global::Debug.LogWarning("Missing recipeDescription");
		}
		return text;
	}

	public static int GetCurrentCycle()
	{
		return GameClock.Instance.GetCycle() + 1;
	}

	public static float GetCurrentTimeInCycles()
	{
		return GameClock.Instance.GetTimeInCycles() + 1f;
	}

	public static GameObject GetActiveTelepad()
	{
		GameObject gameObject = GameUtil.GetTelepad(ClusterManager.Instance.activeWorldId);
		if (gameObject == null)
		{
			gameObject = GameUtil.GetTelepad(ClusterManager.Instance.GetStartWorld().id);
		}
		return gameObject;
	}

	public static GameObject GetTelepad(int worldId)
	{
		if (Components.Telepads.Count > 0)
		{
			for (int i = 0; i < Components.Telepads.Count; i++)
			{
				if (Components.Telepads[i].GetMyWorldId() == worldId)
				{
					return Components.Telepads[i].gameObject;
				}
			}
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
		return Util.KInstantiate(original, position, Quaternion.identity, parent, name, true, gameLayer);
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
		int* ptr = stackalloc int[(UIntPtr)16];
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
				all_not_gaseous = all_not_gaseous && !element.IsGas && !element.IsVacuum;
				all_over_pressure = all_over_pressure && ((!element.IsGas && !element.IsVacuum) || Grid.Mass[num] >= 1.8f);
			}
		}
	}

	public static float GetDecorAtCell(int cell)
	{
		return GameUtil.GetDecorAtCell(cell, true);
	}

	public static float GetDecorAtCell(int cell, bool includeLightDecor)
	{
		float num = 0f;
		if (!Grid.Solid[cell])
		{
			num = Grid.Decor[cell];
			if (includeLightDecor)
			{
				num += (float)DecorProvider.GetLightDecorBonus(cell);
			}
		}
		return num;
	}

	public static string GetUnitTypeMassOrUnit(GameObject go)
	{
		string text = UI.UNITSUFFIXES.UNITS;
		KPrefabID component = go.GetComponent<KPrefabID>();
		if (component != null)
		{
			text = (component.Tags.Contains(GameTags.Seed) ? UI.UNITSUFFIXES.UNITS : UI.UNITSUFFIXES.MASS.KILOGRAM);
		}
		return text;
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
		string text = "";
		global::UnityEngine.Object component = go.GetComponent<Edible>();
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

	public static Sprite GetBiomeSprite(string id)
	{
		string text = "biomeIcon" + char.ToUpper(id[0]).ToString() + id.Substring(1).ToLower();
		Sprite sprite = Assets.GetSprite(text);
		if (sprite != null)
		{
			return new global::Tuple<Sprite, Color>(sprite, Color.white).first;
		}
		global::Debug.LogWarning("Missing codex biome icon: " + text);
		return null;
	}

	public static string GenerateRandomDuplicantName()
	{
		string text = "";
		string text2 = "";
		bool flag = global::UnityEngine.Random.Range(0f, 1f) >= 0.5f;
		List<string> list = new List<string>(LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.NAME.NB)));
		list.AddRange(flag ? LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.NAME.MALE)) : LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.NAME.FEMALE)));
		string random = list.GetRandom<string>();
		if (global::UnityEngine.Random.Range(0f, 1f) > 0.7f)
		{
			List<string> list2 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.PREFIX.NB)));
			list2.AddRange(flag ? LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.PREFIX.MALE)) : LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.PREFIX.FEMALE)));
			text = list2.GetRandom<string>();
		}
		if (!string.IsNullOrEmpty(text))
		{
			text += " ";
		}
		if (global::UnityEngine.Random.Range(0f, 1f) >= 0.9f)
		{
			List<string> list3 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.SUFFIX.NB)));
			list3.AddRange(flag ? LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.SUFFIX.MALE)) : LocString.GetStrings(typeof(NAMEGEN.DUPLICANT.SUFFIX.FEMALE)));
			text2 = list3.GetRandom<string>();
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text2 = " " + text2;
		}
		return text + random + text2;
	}

	public static string GenerateRandomLaunchPadName()
	{
		return NAMEGEN.LAUNCHPAD.FORMAT.Replace("{Name}", global::UnityEngine.Random.Range(1, 1000).ToString());
	}

	public static string GenerateRandomRocketName()
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		int num = 1;
		int num2 = 2;
		int num3 = 4;
		string random = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.NOUN))).GetRandom<string>();
		int num4 = 0;
		if (global::UnityEngine.Random.value > 0.7f)
		{
			text = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.PREFIX))).GetRandom<string>();
			num4 |= num;
		}
		if (global::UnityEngine.Random.value > 0.5f)
		{
			text2 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.ADJECTIVE))).GetRandom<string>();
			num4 |= num2;
		}
		if (global::UnityEngine.Random.value > 0.1f)
		{
			text3 = new List<string>(LocString.GetStrings(typeof(NAMEGEN.ROCKET.SUFFIX))).GetRandom<string>();
			num4 |= num3;
		}
		string text4;
		if (num4 == (num | num2 | num3))
		{
			text4 = NAMEGEN.ROCKET.FMT_PREFIX_ADJECTIVE_NOUN_SUFFIX;
		}
		else if (num4 == (num2 | num3))
		{
			text4 = NAMEGEN.ROCKET.FMT_ADJECTIVE_NOUN_SUFFIX;
		}
		else if (num4 == (num | num3))
		{
			text4 = NAMEGEN.ROCKET.FMT_PREFIX_NOUN_SUFFIX;
		}
		else if (num4 == num3)
		{
			text4 = NAMEGEN.ROCKET.FMT_NOUN_SUFFIX;
		}
		else if (num4 == (num | num2))
		{
			text4 = NAMEGEN.ROCKET.FMT_PREFIX_ADJECTIVE_NOUN;
		}
		else if (num4 == num)
		{
			text4 = NAMEGEN.ROCKET.FMT_PREFIX_NOUN;
		}
		else if (num4 == num2)
		{
			text4 = NAMEGEN.ROCKET.FMT_ADJECTIVE_NOUN;
		}
		else
		{
			text4 = NAMEGEN.ROCKET.FMT_NOUN;
		}
		DebugUtil.LogArgs(new object[]
		{
			"Rocket name bits:",
			Convert.ToString(num4, 2)
		});
		return text4.Replace("{Prefix}", text).Replace("{Adjective}", text2).Replace("{Noun}", random)
			.Replace("{Suffix}", text3);
	}

	public static string GenerateRandomWorldName(string[] nameTables)
	{
		if (nameTables == null)
		{
			global::Debug.LogWarning("No name tables provided to generate world name. Using GENERIC");
			nameTables = new string[] { "GENERIC" };
		}
		string text = "";
		foreach (string text2 in nameTables)
		{
			text += Strings.Get("STRINGS.NAMEGEN.WORLD.ROOTS." + text2.ToUpper());
		}
		string text3 = GameUtil.RandomValueFromSeparatedString(text, "\n");
		if (string.IsNullOrEmpty(text3))
		{
			text3 = GameUtil.RandomValueFromSeparatedString(Strings.Get(NAMEGEN.WORLD.ROOTS.GENERIC), "\n");
		}
		string text4 = GameUtil.RandomValueFromSeparatedString(NAMEGEN.WORLD.SUFFIXES.GENERICLIST, "\n");
		return text3 + text4;
	}

	public static float GetThermalComfort(Tag duplicantType, int cell, float tolerance)
	{
		DUPLICANTSTATS statsFor = DUPLICANTSTATS.GetStatsFor(duplicantType);
		float num = 0f;
		Element element = ElementLoader.FindElementByHash(SimHashes.Creature);
		if (Grid.Element[cell].thermalConductivity != 0f)
		{
			num = SimUtil.CalculateEnergyFlowCreatures(cell, statsFor.Temperature.Internal.IDEAL, element.specificHeatCapacity, element.thermalConductivity, statsFor.Temperature.SURFACE_AREA, statsFor.Temperature.SKIN_THICKNESS + 0.0025f);
		}
		num -= tolerance;
		return num * 1000f;
	}

	public static void FocusCamera(Transform target, bool select = true, bool show_back_button = true)
	{
		GameUtil.FocusCamera(target.GetPosition(), 2f, true, show_back_button);
		if (select)
		{
			KSelectable component = target.GetComponent<KSelectable>();
			SelectTool.Instance.Select(component, false);
		}
	}

	public static void FocusCameraOnWorld(int worldID, Vector3 pos, float forceOrthgraphicSize = 10f, global::System.Action callback = null, bool show_back_button = true)
	{
		CameraController.Instance.ActiveWorldStarWipe(worldID, pos, forceOrthgraphicSize, callback);
		if (show_back_button && NotificationScreen_TemporaryActions.Instance != null)
		{
			NotificationScreen_TemporaryActions.Instance.CreateCameraReturnActionButton(CameraController.Instance.transform.position);
		}
	}

	public static void FocusCamera(int cell, bool show_back_button = true)
	{
		GameUtil.FocusCamera(Grid.CellToPos(cell), 2f, true, show_back_button);
	}

	public static void FocusCamera(Vector3 position, float speed = 2f, bool playSound = true, bool show_back_button = true)
	{
		CameraController.Instance.CameraGoTo(position, speed, playSound);
		if (show_back_button && NotificationScreen_TemporaryActions.Instance != null)
		{
			NotificationScreen_TemporaryActions.Instance.CreateCameraReturnActionButton(CameraController.Instance.transform.position);
		}
	}

	public static string RandomValueFromSeparatedString(string source, string separator = "\n")
	{
		int num = 0;
		int num2 = 0;
		for (;;)
		{
			num = source.IndexOf(separator, num);
			if (num == -1)
			{
				break;
			}
			num += separator.Length;
			num2++;
		}
		if (num2 == 0)
		{
			return "";
		}
		int num3 = global::UnityEngine.Random.Range(0, num2);
		num = 0;
		for (int i = 0; i < num3; i++)
		{
			num = source.IndexOf(separator, num) + separator.Length;
		}
		int num4 = source.IndexOf(separator, num);
		return source.Substring(num, (num4 == -1) ? (source.Length - num) : (num4 - num));
	}

	public static string GetFormattedDiseaseName(byte idx, bool color = false)
	{
		Disease disease = Db.Get().Diseases[(int)idx];
		if (color)
		{
			return string.Format(UI.OVERLAYS.DISEASE.DISEASE_NAME_FORMAT, disease.Name, GameUtil.ColourToHex(GlobalAssets.Instance.colorSet.GetColorByName(disease.overlayColourName)));
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
			return string.Format(UI.OVERLAYS.DISEASE.DISEASE_FORMAT, disease.Name, GameUtil.GetFormattedDiseaseAmount(units, GameUtil.TimeSlice.None), GameUtil.ColourToHex(GlobalAssets.Instance.colorSet.GetColorByName(disease.overlayColourName)));
		}
		return string.Format(UI.OVERLAYS.DISEASE.DISEASE_FORMAT_NO_COLOR, disease.Name, GameUtil.GetFormattedDiseaseAmount(units, GameUtil.TimeSlice.None));
	}

	public static string GetFormattedDiseaseAmount(int units, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		GameUtil.ApplyTimeSlice(units, timeSlice);
		return GameUtil.AddTimeSliceText(units.ToString("#,##0") + UI.UNITSUFFIXES.DISEASE.UNITS, timeSlice);
	}

	public static string GetFormattedDiseaseAmount(long units, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		GameUtil.ApplyTimeSlice((float)units, timeSlice);
		return GameUtil.AddTimeSliceText(units.ToString("#,##0") + UI.UNITSUFFIXES.DISEASE.UNITS, timeSlice);
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
		string text = "";
		LocString locString = ((value > DecorMonitor.MAXIMUM_DECOR_VALUE && enforce_max) ? UI.OVERLAYS.DECOR.MAXIMUM_DECOR : UI.OVERLAYS.DECOR.VALUE);
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
				string text = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString());
				string text2 = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString());
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
			string formattedString = attributeModifier.GetFormattedString();
			text = text + "\n    • " + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, name, formattedString);
		}
		text += GameUtil.GetSignificantMaterialPropertyTooltips(element);
		return text;
	}

	public static string GetSignificantMaterialPropertyTooltips(Element element)
	{
		string text = "";
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
		if (Sim.IsRadiationEnabled() && element.radiationAbsorptionFactor >= 0.8f)
		{
			Descriptor descriptor5 = default(Descriptor);
			descriptor5.SetupDescriptor(ELEMENTS.MATERIAL_MODIFIERS.EXCELLENT_RADIATION_SHIELD, string.Format(ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.EXCELLENT_RADIATION_SHIELD, element.name, element.radiationAbsorptionFactor), Descriptor.DescriptorType.Effect);
			descriptor5.IncreaseIndent();
			list.Add(descriptor5);
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
					string text = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString());
					string text2 = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + attributeModifier.AttributeId.ToUpper())), attributeModifier.GetFormattedString());
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
						string text3 = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + attributeModifier2.AttributeId.ToUpper())), attributeModifier2.GetFormattedString());
						string text4 = string.Format(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + attributeModifier2.AttributeId.ToUpper())), attributeModifier2.GetFormattedString());
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
				string formattedString = attributeModifier.GetFormattedString();
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
						string formattedString2 = attributeModifier2.GetFormattedString();
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
		string text = "";
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
		string text = "";
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

	public static List<BuildingDef> GetBuildingsRequiringSkillPerk(string perkID)
	{
		return Assets.BuildingDefs.Where<BuildingDef>((BuildingDef building) => building.RequiredSkillPerkID == perkID).ToList<BuildingDef>();
	}

	public static string NamesOfBuildingsRequiringSkillPerk(string perkID)
	{
		List<string> list = (from building in GameUtil.GetBuildingsRequiringSkillPerk(perkID)
			select GameUtil.SafeStringFormat(UI.ROLES_SCREEN.PERKS.CAN_USE_BUILDING.DESCRIPTION, new object[] { building.Name })).ToList<string>();
		if (list == null || list.Count == 0)
		{
			return null;
		}
		return string.Join("\n", list);
	}

	public static string NamesOfBoostersWithSkillPerk(string perkID)
	{
		List<string> list = (from tag in BionicUpgradeComponentConfig.GetBoostersWithSkillPerk(perkID)
			select Strings.Get(string.Format("STRINGS.ITEMS.BIONIC_BOOSTERS.{0}.NAME", tag.ToString().ToUpper())).String).ToList<string>();
		return string.Join("\n", list);
	}

	public static string NamesOfSkillsWithSkillPerk(string perkID)
	{
		List<string> list = (from match in Db.Get().Skills.resources
			where !match.deprecated && match.GivesPerk(perkID)
			select match.Name).ToList<string>();
		return string.Join("\n", list.ToArray());
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
		int i = 0;
		while (i < GERM_EXPOSURE.TYPES.Length)
		{
			if (disease.id == GERM_EXPOSURE.TYPES[i].germ_id)
			{
				if (GERM_EXPOSURE.TYPES[i].sickness_id == null)
				{
					return null;
				}
				return Db.Get().Sicknesses.Get(GERM_EXPOSURE.TYPES[i].sickness_id);
			}
			else
			{
				i++;
			}
		}
		return null;
	}

	public static void SubscribeToTags<T>(T target, EventSystem.IntraObjectHandler<T> handler, bool triggerImmediately) where T : KMonoBehaviour
	{
		if (triggerImmediately)
		{
			Boxed<TagChangedEventData> boxed = Boxed<TagChangedEventData>.Get(new TagChangedEventData(Tag.Invalid, false));
			handler.Trigger(target.gameObject, boxed);
			Boxed<TagChangedEventData>.Release(boxed);
		}
		target.Subscribe<T>(-1582839653, handler);
	}

	public static void UnsubscribeToTags<T>(T target, EventSystem.IntraObjectHandler<T> handler) where T : KMonoBehaviour
	{
		target.Unsubscribe<T>(-1582839653, handler, false);
	}

	public static EventSystem.IntraObjectHandler<T> CreateHasTagHandler<T>(Tag tag, Action<T, object> callback) where T : KMonoBehaviour
	{
		return new EventSystem.IntraObjectHandler<T>(delegate(T component, object data)
		{
			TagChangedEventData value = ((Boxed<TagChangedEventData>)data).value;
			if (value.tag == Tag.Invalid)
			{
				KPrefabID component2 = component.GetComponent<KPrefabID>();
				value = new TagChangedEventData(tag, component2.HasTag(tag));
			}
			if (value.tag == tag && value.added)
			{
				callback(component, data);
			}
		});
	}

	public static void DestroyCell(int cell, CellElementEvent eventSource, bool deleteMinions = true)
	{
		List<GameObject> list;
		using (CollectionPool<List<GameObject>, GameObject>.Get(out list))
		{
			list.Add(Grid.Objects[cell, 2]);
			list.Add(Grid.Objects[cell, 1]);
			list.Add(Grid.Objects[cell, 9]);
			list.Add(Grid.Objects[cell, 5]);
			list.Add(Grid.Objects[cell, 12]);
			list.Add(Grid.Objects[cell, 15]);
			list.Add(Grid.Objects[cell, 16]);
			list.Add(Grid.Objects[cell, 19]);
			list.Add(Grid.Objects[cell, 20]);
			list.Add(Grid.Objects[cell, 23]);
			list.Add(Grid.Objects[cell, 26]);
			list.Add(Grid.Objects[cell, 29]);
			list.Add(Grid.Objects[cell, 27]);
			list.Add(Grid.Objects[cell, 31]);
			list.Add(Grid.Objects[cell, 30]);
			foreach (Comet comet in Components.Meteors.GetItems((int)Grid.WorldIdx[cell]))
			{
				if (!comet.IsNullOrDestroyed() && Grid.PosToCell(comet) == cell)
				{
					list.Add(comet.gameObject);
				}
			}
			foreach (GameObject gameObject in list)
			{
				if (gameObject != null)
				{
					Util.KDestroyGameObject(gameObject);
				}
			}
			GameUtil.ClearCell(cell, deleteMinions);
			FallingWater.instance.ClearParticles(cell);
			if (ElementLoader.elements[(int)Grid.ElementIdx[cell]].id == SimHashes.Void)
			{
				SimMessages.ReplaceElement(cell, SimHashes.Void, eventSource, 0f, 0f, byte.MaxValue, 0, -1);
			}
			else
			{
				SimMessages.ReplaceElement(cell, SimHashes.Vacuum, eventSource, 0f, 0f, byte.MaxValue, 0, -1);
			}
			if (BackwallManager.HasBackwall(cell))
			{
				SimMessages.Dig(cell, -1, true, true);
			}
		}
	}

	public static void ClearCell(int cell, bool deleteMinions = true)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		List<ScenePartitionerEntry> list;
		using (CollectionPool<List<ScenePartitionerEntry>, ScenePartitionerEntry>.Get(out list))
		{
			GameScenePartitioner.Instance.GatherEntries(vector2I.x, vector2I.y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, list);
			for (int i = 0; i < list.Count; i++)
			{
				Pickupable pickupable = list[i].obj as Pickupable;
				if (!(pickupable == null))
				{
					bool flag = pickupable.KPrefabID.HasTag(GameTags.BaseMinion);
					if (deleteMinions || !flag)
					{
						Util.KDestroyGameObject(pickupable.gameObject);
					}
				}
			}
		}
	}

	public static GameUtil.TemperatureUnit temperatureUnit;

	public static GameUtil.MassUnit massUnit;

	private static string[] adjectives;

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
		Disease,
		Radiation,
		Energy,
		Power,
		Lux,
		Time,
		Seconds,
		Cycles
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

	public enum IdentityDescriptorTense
	{
		Normal,
		Possessive,
		Plural
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

	public static class Hardness
	{
		public const int VERY_SOFT = 0;

		public const int SOFT = 10;

		public const int FIRM = 25;

		public const int VERY_FIRM = 50;

		public const int NEARLY_IMPENETRABLE = 150;

		public const int SUPER_DUPER_HARD = 200;

		public const int RADIOACTIVE_MATERIALS = 251;

		public const int IMPENETRABLE = 255;

		public static Color ImpenetrableColor = new Color(0.83137256f, 0.28627452f, 0.28235295f);

		public static Color nearlyImpenetrableColor = new Color(0.7411765f, 0.34901962f, 0.49803922f);

		public static Color veryFirmColor = new Color(0.6392157f, 0.39215687f, 0.6039216f);

		public static Color firmColor = new Color(0.5254902f, 0.41960785f, 0.64705884f);

		public static Color softColor = new Color(0.42745098f, 0.48235294f, 0.75686276f);

		public static Color verySoftColor = new Color(0.44313726f, 0.67058825f, 0.8117647f);
	}

	public static class GermResistanceValues
	{
		public const float MEDIUM = 2f;

		public const float LARGE = 5f;

		public static Color NegativeLargeColor = new Color(0.83137256f, 0.28627452f, 0.28235295f);

		public static Color NegativeMediumColor = new Color(0.7411765f, 0.34901962f, 0.49803922f);

		public static Color NegativeSmallColor = new Color(0.6392157f, 0.39215687f, 0.6039216f);

		public static Color PositiveSmallColor = new Color(0.5254902f, 0.41960785f, 0.64705884f);

		public static Color PositiveMediumColor = new Color(0.42745098f, 0.48235294f, 0.75686276f);

		public static Color PositiveLargeColor = new Color(0.44313726f, 0.67058825f, 0.8117647f);
	}

	public static class ThermalConductivityValues
	{
		public const float VERY_HIGH = 50f;

		public const float HIGH = 10f;

		public const float MEDIUM = 2f;

		public const float LOW = 1f;

		public static Color veryLowConductivityColor = new Color(0.83137256f, 0.28627452f, 0.28235295f);

		public static Color lowConductivityColor = new Color(0.7411765f, 0.34901962f, 0.49803922f);

		public static Color mediumConductivityColor = new Color(0.6392157f, 0.39215687f, 0.6039216f);

		public static Color highConductivityColor = new Color(0.5254902f, 0.41960785f, 0.64705884f);

		public static Color veryHighConductivityColor = new Color(0.42745098f, 0.48235294f, 0.75686276f);
	}

	public static class BreathableValues
	{
		public static Color positiveColor = new Color(0.44313726f, 0.67058825f, 0.8117647f);

		public static Color warningColor = new Color(0.6392157f, 0.39215687f, 0.6039216f);

		public static Color negativeColor = new Color(0.83137256f, 0.28627452f, 0.28235295f);
	}

	public static class WireLoadValues
	{
		public static Color warningColor = new Color(0.9843137f, 0.6901961f, 0.23137255f);

		public static Color negativeColor = new Color(1f, 0.19215687f, 0.19215687f);
	}
}
