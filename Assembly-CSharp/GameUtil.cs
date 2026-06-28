using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class GameUtil
{
	private static string AddTemperatureUnitSuffix(string text)
	{
		string text2 = string.Empty;
		GameUtil.TemperatureUnit temperatureUnit = GameUtil.temperatureUnit;
		if (temperatureUnit != GameUtil.TemperatureUnit.Celsius)
		{
			if (temperatureUnit != GameUtil.TemperatureUnit.Fahrenheit)
			{
				text2 = UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
			}
			else
			{
				text2 = UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
			}
		}
		else
		{
			text2 = UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
		}
		return text + text2;
	}

	public static float GetConvertedTemperature(float temperature)
	{
		GameUtil.TemperatureUnit temperatureUnit = GameUtil.temperatureUnit;
		if (temperatureUnit == GameUtil.TemperatureUnit.Celsius)
		{
			return temperature - 273.15f;
		}
		if (temperatureUnit != GameUtil.TemperatureUnit.Fahrenheit)
		{
			return temperature;
		}
		return temperature * 1.8f - 459.67f;
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

	private static float GetConvertedTemperatureDelta(float kelivnDelta)
	{
		switch (GameUtil.temperatureUnit)
		{
		case GameUtil.TemperatureUnit.Celsius:
			return kelivnDelta;
		case GameUtil.TemperatureUnit.Fahrenheit:
			return kelivnDelta * 1.8f;
		case GameUtil.TemperatureUnit.Kelvin:
			return kelivnDelta;
		default:
			return kelivnDelta;
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

	public static float GetEnergyInPrimaryElement(PrimaryElement element)
	{
		return 0.001f * (element.Temperature * (element.Mass * 1000f * element.Element.specificHeatCapacity));
	}

	public static float EnergyToTemperatureDelta(float kilojoules, PrimaryElement element)
	{
		float energyInPrimaryElement = GameUtil.GetEnergyInPrimaryElement(element);
		float num = Mathf.Max(energyInPrimaryElement - kilojoules, 1f);
		float temperature = element.Temperature;
		float num2 = num / (0.001f * (element.Mass * (element.Element.specificHeatCapacity * 1000f)));
		return num2 - temperature;
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
			global::Debug.LogError("Calculated an invalid temperature", null);
		}
		return num3;
	}

	public static string GetUnitFormattedName(GameObject go, bool upperName = false)
	{
		PrimaryElement component = go.GetComponent<PrimaryElement>();
		if (component != null && component.CountableUnits)
		{
			return GameUtil.GetUnitFormattedName(go.GetProperName(), component.Units, upperName);
		}
		return (!upperName) ? go.GetProperName() : go.GetProperName().ToUpper();
	}

	public static string GetUnitFormattedName(string name, float count, bool upperName = false)
	{
		if (upperName)
		{
			name = name.ToUpper();
		}
		return string.Format(UI.NAME_WITH_UNITS, name, string.Format("{0:0.##}", count));
	}

	public static string GetFormattedUnits(float units, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool displaySuffix = true)
	{
		string text = UI.UNITSUFFIXES.UNITS;
		units = GameUtil.ApplyTimeSlice(units, timeSlice);
		string text2 = string.Empty;
		if (units == 0f)
		{
			text2 = "0";
		}
		else if (Mathf.Abs(units) < 1f)
		{
			text2 = units.ToString("#,##0.#");
		}
		else if (Mathf.Abs(units) < 10f)
		{
			text2 = units.ToString("#,###.#");
		}
		else
		{
			text2 = units.ToString("#,###");
		}
		if (displaySuffix)
		{
			text2 += text;
		}
		return GameUtil.AddTimeSliceText(text2, timeSlice);
	}

	public static string GetFormattedTemperature(float temp, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation interpretation = GameUtil.TemperatureInterpretation.Absolute, bool displayUnits = true)
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
			temp = GameUtil.GetConvertedTemperature(temp);
		}
		temp = GameUtil.ApplyTimeSlice(temp, timeSlice);
		string text = string.Empty;
		if (Mathf.Abs(temp) < 0.1f)
		{
			text = temp.ToString("##0.####");
		}
		else
		{
			text = temp.ToString("##0.#");
		}
		if (displayUnits)
		{
			text = GameUtil.AddTemperatureUnitSuffix(text);
		}
		return GameUtil.AddTimeSliceText(text, timeSlice);
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
			text2 = calories.ToString("#,##0.#") + text;
		}
		else if (Mathf.Abs(calories) < 10f)
		{
			text2 = calories.ToString("#,###.#") + text;
		}
		else
		{
			text2 = calories.ToString("#,###") + text;
		}
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
		string text2 = string.Format(UI.UNITSUFFIXES.PERCENT, percent.ToString(text));
		return GameUtil.AddTimeSliceText(text2, timeSlice);
	}

	public static string GetFormattedRoundedJoules(float joules)
	{
		if (Mathf.Abs(joules) > 1000f)
		{
			return (joules / 1000f).ToString("F1") + UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE;
		}
		return joules.ToString("F1") + UI.UNITSUFFIXES.ELECTRICAL.JOULE;
	}

	public static string GetFormattedJoules(float joules, string floatFormat = "F1")
	{
		if (Mathf.Abs(joules) > 1000f)
		{
			return (joules / 1000f).ToString(floatFormat) + UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE;
		}
		return joules.ToString(floatFormat) + UI.UNITSUFFIXES.ELECTRICAL.JOULE;
	}

	public static string GetFormattedWattage(float watts, string floatFormat = "F1")
	{
		if (Mathf.Abs(watts) > 1000f)
		{
			return (watts / 1000f).ToString(floatFormat) + UI.UNITSUFFIXES.ELECTRICAL.KILOWATT;
		}
		return watts.ToString(floatFormat) + UI.UNITSUFFIXES.ELECTRICAL.WATT;
	}

	public static string GetFormattedInt(float num, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		num = GameUtil.ApplyTimeSlice(num, timeSlice);
		return GameUtil.AddTimeSliceText(num.ToString("F0"), timeSlice);
	}

	public static string GetFormattedSimple(float num, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, string formatString = null)
	{
		num = GameUtil.ApplyTimeSlice(num, timeSlice);
		string text = string.Empty;
		if (formatString != null)
		{
			text = num.ToString(formatString);
		}
		else if (num == 0f)
		{
			text = "0";
		}
		else if (Mathf.Abs(num) < 1f)
		{
			text = num.ToString("#,##0.#");
		}
		else if (Mathf.Abs(num) < 10f)
		{
			text = num.ToString("#,###.#");
		}
		else
		{
			text = num.ToString("#,###");
		}
		return GameUtil.AddTimeSliceText(text, timeSlice);
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
			text = UI.UNITSUFFIXES.MASS.KILOGRAM;
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
				}
			}
			else if (massFormat == GameUtil.MetricMassFormat.Gram)
			{
				mass *= 1000f;
				text = UI.UNITSUFFIXES.MASS.GRAM;
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
		return meters + " m";
	}

	public static string GetFormattedCycles(float seconds, string formatString = "F1")
	{
		if (Mathf.Abs(seconds) > 100f)
		{
			return string.Format(UI.FORMATDAY, (seconds / 600f).ToString(formatString));
		}
		return GameUtil.GetFormattedTime(seconds);
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
			hashSet = GameUtil.FloodCollectCells(startCell, (int cell) => !Grid.Solid[cell], 300, null);
		}
		else
		{
			hashSet = GameUtil.FloodCollectCells(startCell, (int cell) => Grid.Element[cell].IsVacuum || Grid.Element[cell].IsGas, 300, null);
		}
		return hashSet;
	}

	public static HashSet<int> FloodCollectCells(int start_cell, Func<int, bool> is_valid, int maxSize = 300, HashSet<int> AddInvalidCells = null)
	{
		HashSet<int> hashSet = new HashSet<int>();
		HashSet<int> hashSet2 = new HashSet<int>();
		GameUtil.probeFromCell(start_cell, is_valid, hashSet, hashSet2, maxSize);
		if (AddInvalidCells != null)
		{
			AddInvalidCells.UnionWith(hashSet2);
			if (hashSet.Count > maxSize)
			{
				AddInvalidCells.UnionWith(hashSet);
			}
		}
		if (hashSet.Count > maxSize)
		{
			hashSet.Clear();
		}
		return hashSet;
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

	public static bool FloodFillCheck(Func<int, bool> fn, int start_cell, int max_depth, bool stop_at_solid, bool stop_at_liquid)
	{
		int num = GameUtil.FloodFillFind(fn, start_cell, max_depth, stop_at_solid, stop_at_liquid);
		return num != -1;
	}

	public static int FloodFillFind(Func<int, bool> fn, int start_cell, int max_depth, bool stop_at_solid, bool stop_at_liquid)
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
								if (fn(floodFillInfo.cell))
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
		string text = string.Empty;
		Color color7;
		if (element.hardness >= 255)
		{
			color7 = color;
			text = string.Format(ELEMENTS.HARDNESS.IMPENETRABLE, element.hardness);
		}
		else if (element.hardness >= 150)
		{
			color7 = color2;
			text = string.Format(ELEMENTS.HARDNESS.NEARLYIMPENETRABLE, element.hardness);
		}
		else if (element.hardness >= 50)
		{
			color7 = color3;
			text = string.Format(ELEMENTS.HARDNESS.VERYFIRM, element.hardness);
		}
		else if (element.hardness >= 25)
		{
			color7 = color4;
			text = string.Format(ELEMENTS.HARDNESS.FIRM, element.hardness);
		}
		else if (element.hardness >= 10)
		{
			color7 = color5;
			text = string.Format(ELEMENTS.HARDNESS.SOFT, element.hardness);
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
		if (id != SimHashes.ContaminatedOxygen)
		{
			if (id != SimHashes.Oxygen)
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
		else if (Mass >= 0.3f)
		{
			color4 = color2;
			locString = UI.OVERLAYS.OXYGEN.LEGEND6;
		}
		else if (Mass > 0.05f)
		{
			color4 = color2;
			locString = UI.OVERLAYS.OXYGEN.LEGEND5;
		}
		else
		{
			color4 = color3;
			locString = UI.OVERLAYS.OXYGEN.LEGEND4;
		}
		return string.Format(ELEMENTS.BREATHABLEDESC, color4.ToHexString(), locString);
	}

	public static string GetHotkeyString(global::Action action)
	{
		Color color = new Color(0.95686275f, 0.2901961f, 0.2784314f);
		return string.Concat(new string[]
		{
			"<color=#",
			color.ToHexString(),
			">(",
			GameUtil.GetActionString(action),
			") </color>"
		});
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
				case KKeyCode.Backspace:
					text = INPUT.BACKSPACE;
					break;
				case KKeyCode.Tab:
					text = INPUT.TAB;
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
															global::Debug.LogWarning("Unable to find proper string for KKeyCode: " + key_code.ToString() + " using key_code.ToString()", null);
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
							}
							break;
						}
						break;
					case KKeyCode.BackQuote:
						text = INPUT.BACKQUOTE;
						break;
					}
					break;
				case KKeyCode.Return:
					text = INPUT.ENTER;
					break;
				}
				break;
			}
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
		KKeyCode mKeyCode = GameUtil.ActionToBinding(action).mKeyCode;
		return GameUtil.GetKeycodeLocalized(mKeyCode).ToUpper();
	}

	public static void CreateExplosion(Vector3 explosion_pos)
	{
		Vector2 vector = new Vector2(explosion_pos.x, explosion_pos.y);
		float num = 5f;
		float num2 = num * num;
		foreach (Health health in Components.Health)
		{
			Vector3 position = health.transform.position;
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
		if (Grid.IsValidCell(num) && !Grid.Solid[num] && !Grid.ForceField[num] && x >= min_x && x <= max_x && y >= min_y && y <= max_y && !cells.Contains(num))
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
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
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
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
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
			int num = global::TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e1.GetType().Name);
			int num2 = global::TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e2.GetType().Name);
			return num.CompareTo(num2);
		});
	}

	private static void SortGameObjectDescriptors(List<IGameObjectEffectDescriptor> descriptorList)
	{
		descriptorList.Sort(delegate(IGameObjectEffectDescriptor e1, IGameObjectEffectDescriptor e2)
		{
			int num = global::TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e1.GetType().Name);
			int num2 = global::TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e2.GetType().Name);
			return num.CompareTo(num2);
		});
	}

	public static void IndentListOfDescriptors(List<Descriptor> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			Descriptor descriptor = list[i];
			descriptor.IncreaseIndent();
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
				foreach (Descriptor descriptor in descriptors)
				{
					list.Add(descriptor);
				}
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
		if (component2.AdditionalRequirements != null)
		{
			foreach (Descriptor descriptor2 in component2.AdditionalRequirements)
			{
				if (!descriptor2.onlyForSimpleInfoScreen || simpleInfoScreen)
				{
					list.Add(descriptor2);
				}
			}
		}
		if (component2.AdditionalEffects != null)
		{
			foreach (Descriptor descriptor3 in component2.AdditionalEffects)
			{
				if (!descriptor3.onlyForSimpleInfoScreen || simpleInfoScreen)
				{
					list.Add(descriptor3);
				}
			}
		}
		GameUtil.SortGameObjectDescriptors(list2);
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
		GameUtil.IndentListOfDescriptors(list);
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
		GameUtil.IndentListOfDescriptors(list);
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
		GameUtil.IndentListOfDescriptors(list);
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
		GameUtil.IndentListOfDescriptors(list);
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
		GameUtil.IndentListOfDescriptors(list);
		return list;
	}

	public static List<Descriptor> GetGameObjectRequirements(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IGameObjectEffectDescriptor> list2 = new List<IGameObjectEffectDescriptor>(go.GetComponents<IGameObjectEffectDescriptor>());
		StateMachineController component = go.GetComponent<StateMachineController>();
		if (component != null)
		{
			list2.AddRange(component.GetAllSMI<IGameObjectEffectDescriptor>());
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
		GameUtil.IndentListOfDescriptors(list);
		return list;
	}

	public static List<Descriptor> GetGameObjectEffects(GameObject go, bool simpleInfoScreen = false)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IGameObjectEffectDescriptor> list2 = new List<IGameObjectEffectDescriptor>(go.GetComponents<IGameObjectEffectDescriptor>());
		StateMachineController component = go.GetComponent<StateMachineController>();
		if (component != null)
		{
			list2.AddRange(component.GetAllSMI<IGameObjectEffectDescriptor>());
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
			global::Debug.LogWarning("Missing recipeDescription", null);
		}
		return text;
	}

	public static int GetCurrentDay()
	{
		return (int)Mathf.Floor(GameClock.Instance.GetTime() / 600f) + 1;
	}

	public static GameObject GetTelepad()
	{
		if (Components.Telepads.Count > 0)
		{
			return Components.Telepads[0].gameObject;
		}
		return null;
	}

	public static GameObject KInstantiate(GameObject original, Vector3 position, Grid.SceneLayer sceneLayer, Folder folder, string name = null, int gameLayer = 0)
	{
		GameObject gameObject = GameUtil.KInstantiate(original, position, sceneLayer, SceneOrganizer.Instance.GetFolder(folder), name, gameLayer);
		SaveLoadRoot component = gameObject.GetComponent<SaveLoadRoot>();
		if (component != null)
		{
			component.folder = folder;
		}
		return gameObject;
	}

	public static GameObject KInstantiate(GameObject original, Vector3 position, Grid.SceneLayer sceneLayer, GameObject parent, string name = null, int gameLayer = 0)
	{
		position.z = Grid.GetLayerZ(sceneLayer);
		GameObject gameObject = Util.KInstantiate(original, position, Quaternion.identity, parent, name, true, gameLayer);
		SaveLoadRoot component = gameObject.GetComponent<SaveLoadRoot>();
		if (component != null && gameObject.GetComponent<SavedObject>() == null)
		{
			gameObject.AddComponent<SavedObject>();
		}
		return gameObject;
	}

	public static GameObject KInstantiate(GameObject original, Grid.SceneLayer sceneLayer, Folder folder, string name = null, int gameLayer = 0)
	{
		return GameUtil.KInstantiate(original, Vector3.zero, sceneLayer, folder, name, gameLayer);
	}

	public static GameObject KInstantiate(Component original, Grid.SceneLayer sceneLayer, Folder folder, string name = null, int gameLayer = 0)
	{
		return GameUtil.KInstantiate(original.gameObject, Vector3.zero, sceneLayer, folder, name, gameLayer);
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
			Element element = Grid.Element[num];
			all_not_gaseous = all_not_gaseous && (!element.IsGas && !element.IsVacuum);
			all_over_pressure = all_over_pressure && ((!element.IsGas && !element.IsVacuum) || Grid.Cell[num].mass >= 1.8f);
		}
	}

	public static float GetNoisePollutionAtCell(int cell)
	{
		return AudioEventManager.Get().GetNoisePollutionAtCell(cell);
	}

	public static string GetLoudestNoisePollutorAtCell(int cell)
	{
		return AudioEventManager.GetLoudestNoisePollutorAtCell(cell);
	}

	public static int GetDecorAtCell(int cell)
	{
		int num = 0;
		if (!Grid.Solid[cell])
		{
			num = Grid.Decor[cell];
			num += DecorProvider.GetLightDecorBonus(cell);
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
		else
		{
			string text2 = tag.ToString();
			TagSet tagSet = new TagSet(new string[] { "BasicPlantFood", "MushBar", "ColdWheatSeed", "ColdWheatSeed", "SpiceNut", "PrickleFruit", "Meat", "Mushroom" });
			if (tagSet.Contains(tag))
			{
				text = "food";
			}
			else if (text2 == "Filter" || text2 == "Coal" || text2 == "BasicFabric")
			{
				text = "solid";
			}
			else
			{
				text = null;
			}
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

	public static void UpdateRegion(Region new_region, KMonoBehaviour cmp, OwnableSlot slot, Tag region_tag)
	{
		Ownable component = cmp.GetComponent<Ownable>();
		if (component == null)
		{
			return;
		}
		bool flag = component.slot == slot;
		bool flag2 = new_region != null && new_region.RegionTag == region_tag;
		if (flag && !flag2)
		{
			component.Unassign();
		}
		else if (!flag && flag2)
		{
			component.slot = slot;
		}
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

	public static float GetThermalComfort(int cell, float tolerance = -0.08368001f)
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

	public static string GetFormattedDecibels(float db)
	{
		Color noisePollutionCategoryColourFromDecibels = SimDebugView.Instance.GetNoisePollutionCategoryColourFromDecibels(db);
		return string.Format(string.Format(UI.OVERLAYS.NOISE_POLLUTION.VALUE, noisePollutionCategoryColourFromDecibels.ToHexString(), db.ToString()), new object[0]);
	}

	public static string GetDecibelLoudnessString(float db)
	{
		string text = string.Empty;
		Color noisePollutionCategoryColourFromDecibels = SimDebugView.Instance.GetNoisePollutionCategoryColourFromDecibels(db);
		if (db <= 36f)
		{
			text = UI.OVERLAYS.NOISE_POLLUTION.NAMES.PEACEFUL;
		}
		else if (db >= 36f && db < 45f)
		{
			text = UI.OVERLAYS.NOISE_POLLUTION.NAMES.QUIET;
		}
		else if (db >= 45f && db < 60f)
		{
			text = UI.OVERLAYS.NOISE_POLLUTION.NAMES.TOSSANDTURN;
		}
		else if (db >= 60f && db < 80f)
		{
			text = UI.OVERLAYS.NOISE_POLLUTION.NAMES.WAKEUP;
		}
		else if (db >= 80f && db < 106f)
		{
			text = UI.OVERLAYS.NOISE_POLLUTION.NAMES.PASSIVE;
		}
		else if (db >= 106f && db < 125f)
		{
			text = UI.OVERLAYS.NOISE_POLLUTION.NAMES.ACTIVE;
		}
		else
		{
			text = UI.OVERLAYS.NOISE_POLLUTION.NAMES.EXTREME;
		}
		return string.Format(UI.OVERLAYS.NOISE_POLLUTION.LOUDNESS_STRING, noisePollutionCategoryColourFromDecibels.ToHexString(), text);
	}

	public static GameUtil.TemperatureUnit temperatureUnit;

	public static GameUtil.MassUnit massUnit;

	private static string[] adjectives;

	[ThreadStatic]
	private static Queue<GameUtil.FloodFillInfo> FloodFillNext = new Queue<GameUtil.FloodFillInfo>();

	[ThreadStatic]
	private static List<int> FloodFillVisited = new List<int>();

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
		Gram
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

	private struct FloodFillInfo
	{
		public int cell;

		public int depth;
	}
}
