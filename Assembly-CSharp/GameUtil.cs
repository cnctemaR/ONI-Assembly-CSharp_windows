using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

	private static float GetConvertedTemperature(float temperature)
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
			Debug.LogError("Calculated an invalid temperature");
		}
		return num3;
	}

	public static string GetFormattedTemperature(float temp, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation interpretation = GameUtil.TemperatureInterpretation.Absolute)
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
		if (temp < 0.1f)
		{
			text = temp.ToString("F2");
		}
		else
		{
			text = temp.ToString("F1");
		}
		return GameUtil.AddTimeSliceText(GameUtil.AddTemperatureUnitSuffix(text), timeSlice);
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
		else if (Mathf.Abs(calories) < 0.1f)
		{
			text2 = calories.ToString("#,###.00") + text;
		}
		else if (Mathf.Abs(calories) < 10f)
		{
			text2 = calories.ToString("#,###.0") + text;
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
		percent /= 100f;
		string text = string.Empty;
		if (Mathf.Abs(percent) < 0.001f)
		{
			text = percent.ToString("P2");
		}
		else if (Mathf.Abs(percent) < 0.01f)
		{
			text = percent.ToString("P1");
		}
		else
		{
			text = percent.ToString("P0");
		}
		return GameUtil.AddTimeSliceText(text, timeSlice);
	}

	public static string GetFormattedJoules(float joules)
	{
		if (Mathf.Abs(joules) > 1000f)
		{
			return (joules / 1000f).ToString("F1") + UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE;
		}
		return joules.ToString("F1") + UI.UNITSUFFIXES.ELECTRICAL.JOULE;
	}

	public static string GetFormattedWattage(float watts)
	{
		if (Mathf.Abs(watts) > 1000f)
		{
			return (watts / 1000f).ToString("F1") + UI.UNITSUFFIXES.ELECTRICAL.KILOWATT;
		}
		return watts.ToString("F1") + UI.UNITSUFFIXES.ELECTRICAL.WATT;
	}

	public static string GetFormattedInt(float num, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		num = GameUtil.ApplyTimeSlice(num, timeSlice);
		return GameUtil.AddTimeSliceText(num.ToString("F0"), timeSlice);
	}

	public static string GetFormattedSimple(float num, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
	{
		num = GameUtil.ApplyTimeSlice(num, timeSlice);
		return GameUtil.AddTimeSliceText(num.ToString("F2"), timeSlice);
	}

	public static string GetFormattedMass(float mass, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None, bool useThreshold = true, string floatFormat = "F1")
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
			if (useThreshold)
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
		}
		else
		{
			mass /= 2.2f;
			text = UI.UNITSUFFIXES.MASS.POUND;
			if (useThreshold)
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
		return GameUtil.AddTimeSliceText(mass.ToString(floatFormat) + text, timeSlice);
	}

	public static string GetFormattedTime(float seconds)
	{
		return string.Format(UI.FORMATSECONDS, seconds.ToString("F0"));
	}

	public static string GetFormattedCycles(float seconds)
	{
		if (seconds > 100f)
		{
			return string.Format(UI.FORMATDAY, (seconds / 600f).ToString("F1"));
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

	public static int FloodFillFind(Func<int, bool> fn, int cell, int depth)
	{
		List<int> list = new List<int>();
		int num = -1;
		GameUtil.FloodFindRecursive(fn, cell, depth, list, false, ref num);
		return num;
	}

	private static bool FloodFindRecursive(Func<int, bool> fn, int cell, int max_depth, List<int> visited, bool bCheck, ref int result)
	{
		if (max_depth <= 0)
		{
			return bCheck;
		}
		if (visited.Contains(cell))
		{
			return bCheck;
		}
		visited.Add(cell);
		if (!Grid.IsValidCell(cell))
		{
			return bCheck;
		}
		Element element = Grid.Element[cell];
		if (element.IsSolid)
		{
			return bCheck;
		}
		if (element.IsLiquid)
		{
			return bCheck;
		}
		if (fn(cell))
		{
			result = cell;
			bCheck = true;
			return bCheck;
		}
		int num = max_depth - 1;
		bCheck = bCheck || GameUtil.FloodFindRecursive(fn, Grid.CellAbove(cell), num, visited, bCheck, ref result);
		bCheck = bCheck || GameUtil.FloodFindRecursive(fn, Grid.CellBelow(cell), num, visited, bCheck, ref result);
		bCheck = bCheck || GameUtil.FloodFindRecursive(fn, Grid.CellLeft(cell), num, visited, bCheck, ref result);
		bCheck = bCheck || GameUtil.FloodFindRecursive(fn, Grid.CellRight(cell), num, visited, bCheck, ref result);
		return bCheck;
	}

	public static bool FloodFillCheck(Func<int, bool> fn, int cell, int depth)
	{
		List<int> list = new List<int>();
		return GameUtil.FloodCheckRecursive(fn, cell, depth, list, false);
	}

	private static bool FloodCheckRecursive(Func<int, bool> fn, int cell, int max_depth, List<int> visited, bool bCheck)
	{
		if (max_depth <= 0)
		{
			return bCheck;
		}
		if (visited.Contains(cell))
		{
			return bCheck;
		}
		visited.Add(cell);
		Element element = Grid.Element[cell];
		if (element.IsSolid)
		{
			return bCheck;
		}
		if (element.IsLiquid)
		{
			return bCheck;
		}
		if (!Grid.IsValidCell(cell))
		{
			return bCheck;
		}
		if (fn(cell))
		{
			bCheck = true;
			return bCheck;
		}
		int num = max_depth - 1;
		bCheck = bCheck || GameUtil.FloodCheckRecursive(fn, Grid.CellAbove(cell), num, visited, bCheck);
		bCheck = bCheck || GameUtil.FloodCheckRecursive(fn, Grid.CellBelow(cell), num, visited, bCheck);
		bCheck = bCheck || GameUtil.FloodCheckRecursive(fn, Grid.CellLeft(cell), num, visited, bCheck);
		bCheck = bCheck || GameUtil.FloodCheckRecursive(fn, Grid.CellRight(cell), num, visited, bCheck);
		return bCheck;
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
			else if (Mass >= 0.3f)
			{
				color4 = color;
				locString = UI.OVERLAYS.OXYGEN.LEGEND2;
			}
			else if (Mass > 0.05f)
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

	public static string GetActionString(global::Action action)
	{
		string text = string.Empty;
		if (action == global::Action.NumActions)
		{
			return text;
		}
		KKeyCode mKeyCode = GameUtil.ActionToBinding(action).mKeyCode;
		KKeyCode kkeyCode = mKeyCode;
		switch (kkeyCode)
		{
		case KKeyCode.Keypad0:
			text += "NUM 0";
			break;
		case KKeyCode.Keypad1:
			text += "NUM 1";
			break;
		case KKeyCode.Keypad2:
			text += "NUM 2";
			break;
		case KKeyCode.Keypad3:
			text += "NUM 3";
			break;
		case KKeyCode.Keypad4:
			text += "NUM 4";
			break;
		case KKeyCode.Keypad5:
			text += "NUM 5";
			break;
		case KKeyCode.Keypad6:
			text += "NUM 6";
			break;
		case KKeyCode.Keypad7:
			text += "NUM 7";
			break;
		case KKeyCode.Keypad8:
			text += "NUM 8";
			break;
		case KKeyCode.Keypad9:
			text += "NUM 9";
			break;
		case KKeyCode.KeypadPeriod:
			text += "NUM PERIOD";
			break;
		case KKeyCode.KeypadDivide:
			text += "NUM /";
			break;
		case KKeyCode.KeypadMultiply:
			text += "NUM *";
			break;
		case KKeyCode.KeypadMinus:
			text += "NUM -";
			break;
		case KKeyCode.KeypadPlus:
			text += "NUM +";
			break;
		case KKeyCode.KeypadEnter:
			text += "NUM ENTER";
			break;
		default:
			switch (kkeyCode)
			{
			case KKeyCode.Backspace:
				text += "BACKSPACE";
				break;
			case KKeyCode.Tab:
				text += "TAB";
				break;
			default:
				switch (kkeyCode)
				{
				case KKeyCode.Plus:
					return text + '+';
				default:
					switch (kkeyCode)
					{
					case KKeyCode.Colon:
						return text + ":";
					case KKeyCode.Semicolon:
						return text + ";";
					default:
						switch (kkeyCode)
						{
						case KKeyCode.LeftBracket:
							return text + "[";
						case KKeyCode.Backslash:
							return text + '\\';
						case KKeyCode.RightBracket:
							return text + "]";
						default:
							if (kkeyCode == KKeyCode.RightShift || kkeyCode == KKeyCode.LeftShift)
							{
								return text + "SHIFT";
							}
							if (kkeyCode == KKeyCode.None)
							{
								return text;
							}
							if (kkeyCode != KKeyCode.Space)
							{
								if (KKeyCode.A <= mKeyCode && mKeyCode <= KKeyCode.Z)
								{
									text += (char)(65 + (mKeyCode - KKeyCode.A));
								}
								else if (KKeyCode.Alpha0 <= mKeyCode && mKeyCode <= KKeyCode.Alpha9)
								{
									text += (char)(48 + (mKeyCode - KKeyCode.Alpha0));
								}
								else if (KKeyCode.F1 <= mKeyCode && mKeyCode <= KKeyCode.F12)
								{
									text = "F" + (mKeyCode - KKeyCode.F1 + 1).ToString();
								}
								else
								{
									text = mKeyCode.ToString().ToUpper();
									Debug.LogWarning("Unable to find proper string for KKeyCode: " + mKeyCode.ToString() + "using key_code.ToString()");
								}
								return text;
							}
							return text + "SPACE";
						}
						break;
					case KKeyCode.Equals:
						break;
					}
					break;
				case KKeyCode.Minus:
					break;
				case KKeyCode.Period:
					return text + "PERIOD";
				case KKeyCode.Slash:
					return text + '/';
				}
				text += '-';
				break;
			case KKeyCode.Return:
				text += "ENTER";
				break;
			}
			break;
		}
		return text;
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
				health.Kill(Db.Get().Deaths.Explosion);
			}
		}
	}

	private static void GetEmptyCells(int x, int y, List<int> cells, int min_x, int min_y, int max_x, int max_y)
	{
		int num = Grid.XYToCell(x, y);
		if (Grid.IsValidCell(num) && !Grid.Solid[num] && !Grid.ForceField[num] && x >= min_x && x <= max_x && y >= min_y && y <= max_y && !cells.Contains(num))
		{
			cells.Add(num);
			GameUtil.GetEmptyCells(x + 1, y, cells, min_x, min_y, max_x, max_y);
			GameUtil.GetEmptyCells(x - 1, y, cells, min_x, min_y, max_x, max_y);
			GameUtil.GetEmptyCells(x, y + 1, cells, min_x, min_y, max_x, max_y);
			GameUtil.GetEmptyCells(x, y - 1, cells, min_x, min_y, max_x, max_y);
		}
	}

	public static void GetEmptyCells(int cell, int radius, List<int> cells)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		GameUtil.GetEmptyCells(num, num2, cells, num - radius, num2 - radius, num + radius, num2 + radius);
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

	public static float GetAverageToxicity()
	{
		if (Components.LiveMinionIdentities.Count <= 0)
		{
			return 0f;
		}
		float num = 0f;
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
		{
			num += Db.Get().Amounts.Toxicity.Lookup(minionIdentity).value;
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

	public static List<Descriptor> GetBuildingRequirementDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IEffectDescriptor> list2 = new List<IEffectDescriptor>(def.BuildingComplete.GetComponents<IEffectDescriptor>());
		list2.Sort((IEffectDescriptor e1, IEffectDescriptor e2) => (e1.DescriptionOrder >= e2.DescriptionOrder) ? 1 : (-1));
		int num = 0;
		foreach (IEffectDescriptor effectDescriptor in list2)
		{
			List<Descriptor> requirementDescriptions = effectDescriptor.GetRequirementDescriptions(def);
			if (requirementDescriptions != null)
			{
				foreach (Descriptor descriptor in requirementDescriptions)
				{
					num++;
					list.Add(descriptor);
				}
			}
		}
		if (num > 0)
		{
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(UI.BUILDINGEFFECTS.OPERATIONREQUIREMENTS, UI.BUILDINGEFFECTS.TOOLTIPS.OPERATIONREQUIREMENTS);
			list.Insert(0, descriptor2);
		}
		return list;
	}

	public static List<Descriptor> GetBuildingEffectsDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		List<IEffectDescriptor> list2 = new List<IEffectDescriptor>(def.BuildingComplete.GetComponents<IEffectDescriptor>());
		list2.Sort((IEffectDescriptor e1, IEffectDescriptor e2) => (e1.DescriptionOrder >= e2.DescriptionOrder) ? 1 : (-1));
		int num = 0;
		if (def.EffectDescription != null && def.EffectDescription.Count > 0)
		{
			for (int i = 0; i < def.EffectDescription.Count; i++)
			{
				list.Add(def.EffectDescription[i]);
			}
		}
		foreach (IEffectDescriptor effectDescriptor in list2)
		{
			List<Descriptor> effectDescriptions = effectDescriptor.GetEffectDescriptions(def);
			if (effectDescriptions != null)
			{
				foreach (Descriptor descriptor in effectDescriptions)
				{
					list.Add(descriptor);
					num++;
				}
			}
		}
		if (num > 0)
		{
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(UI.BUILDINGEFFECTS.OPERATIONEFFECTS, UI.BUILDINGEFFECTS.TOOLTIPS.OPERATIONEFFECTS);
			list.Insert(0, descriptor2);
		}
		return list;
	}

	public static string GetGameObjectEffectsString(GameObject go)
	{
		string text = string.Empty;
		List<IGameObjectEffectDescriptor> list = new List<IGameObjectEffectDescriptor>(go.GetComponents<IGameObjectEffectDescriptor>());
		list.Sort((IGameObjectEffectDescriptor e1, IGameObjectEffectDescriptor e2) => (e1.DescriptionOrder >= e2.DescriptionOrder) ? 1 : (-1));
		foreach (IGameObjectEffectDescriptor gameObjectEffectDescriptor in list)
		{
			List<string> effectDescriptions = gameObjectEffectDescriptor.GetEffectDescriptions(go);
			if (effectDescriptions != null)
			{
				foreach (string text2 in effectDescriptions)
				{
					text += string.Format(UI.LISTENTRYSTRING, text2);
				}
			}
		}
		return text;
	}

	public static string GetGameObjectEffectsTooltipString(GameObject go)
	{
		string text = string.Empty;
		List<IGameObjectEffectDescriptor> list = new List<IGameObjectEffectDescriptor>(go.GetComponents<IGameObjectEffectDescriptor>());
		list.Sort((IGameObjectEffectDescriptor e1, IGameObjectEffectDescriptor e2) => (e1.DescriptionOrder >= e2.DescriptionOrder) ? 1 : (-1));
		int num = 0;
		foreach (IGameObjectEffectDescriptor gameObjectEffectDescriptor in list)
		{
			List<string> effectDescriptions = gameObjectEffectDescriptor.GetEffectDescriptions(go);
			if (effectDescriptions != null)
			{
				foreach (string text2 in effectDescriptions)
				{
					text += string.Format(UI.LISTENTRYSTRING, text2);
					num++;
				}
			}
		}
		if (num > 0)
		{
			text = text.Insert(0, UI.BUILDINGEFFECTS.OPERATIONEFFECTS + "\n");
		}
		return text;
	}

	public static List<string> GetEquipmentEffects(EquipmentDef def)
	{
		List<string> list = new List<string>();
		List<AttributeModifier> attributeModifiers = def.AttributeModifiers;
		if (attributeModifiers != null)
		{
			foreach (AttributeModifier attributeModifier in attributeModifiers)
			{
				Klei.AI.Attribute attribute = Db.Get().Attributes.Get(attributeModifier.AttributeId);
				string name = attribute.Name;
				float value = attributeModifier.Value;
				string text = ((value < 0f) ? "consumed" : "produced");
				string text2 = ((value <= 0f) ? string.Empty : "+");
				string text3 = string.Concat(new string[]
				{
					name,
					" <style=\"",
					text,
					"\">",
					text2,
					value.ToString(),
					"</style>"
				});
				list.Add(text3);
			}
		}
		return list;
	}

	public static List<string> GetCropRequirements(Crop crop)
	{
		return new List<string>();
	}

	public static List<string> GetCropHarvestDetails(Crop crop)
	{
		List<string> list = new List<string>();
		if (crop != null)
		{
			string crop_id = crop.cropId;
			CROPS.CropVal cropVal = CROPS.CROP_TYPES.Find((CROPS.CropVal m) => m.crop_id == crop_id);
			string text = string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.NUMBEROFHARVESTS, cropVal.harvests);
			string text2 = string.Empty;
			Tag tag = new Tag(crop.cropId);
			GameObject prefab = Assets.GetPrefab(tag);
			if (prefab != null)
			{
				Edible component = prefab.GetComponent<Edible>();
				if (component != null)
				{
					float num = (float)component.FoodInfo.Rations;
					if (cropVal.crop_num > 1)
					{
						text2 = "      • " + string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOTALHARVESTCALORIESWITHPERUNIT, GameUtil.GetFormattedCalories(num * (float)cropVal.crop_num * 100000f, GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories(num * 100000f, GameUtil.TimeSlice.None, true));
					}
					else
					{
						text2 = "      • " + string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOTALHARVESTCALORIES, GameUtil.GetFormattedCalories(num * (float)cropVal.crop_num * 100000f, GameUtil.TimeSlice.None, true));
					}
				}
			}
			string text3 = string.Empty;
			if (cropVal.harvests > 1)
			{
				text3 = string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELDPERHARVEST, cropVal.crop_num, crop.GetProperName());
			}
			else
			{
				text3 = string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD, cropVal.crop_num, crop.GetProperName());
			}
			list.Add(text3);
			if (text2 != string.Empty)
			{
				list.Add(text2);
			}
			if (text != string.Empty)
			{
				list.Add(text);
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
			Debug.LogWarning("Missing recipeDescription");
		}
		return text;
	}

	public static int GetCurrentDay()
	{
		return (int)Mathf.Floor(GameClock.Instance.GetTime() / 600f) + 1;
	}

	public static GameObject GetTelepad()
	{
		GameObject gameObject = null;
		foreach (Notifier notifier in Components.Notifiers)
		{
			if (notifier.GetComponent<Telepad>())
			{
				gameObject = notifier.gameObject;
				break;
			}
		}
		return gameObject;
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

	public static List<DecorProviderInfo> GetDecorProvidersAtCell(int cell)
	{
		return null;
	}

	public static string GetKeywordStyle(Tag tag)
	{
		Element element = ElementLoader.GetElement(tag);
		string text;
		if (element != null)
		{
			text = element.keywordStyle;
		}
		else
		{
			string text2 = tag.ToString();
			if (text2 == "Filter" || text2 == "Coal")
			{
				text = "solid";
			}
			else
			{
				Debug.LogWarning("No keyword style for tag " + tag);
				text = null;
			}
		}
		return text;
	}

	public static string GetKeywordStyle(SimHashes hash)
	{
		Element element = ElementLoader.FindElementByHash(hash);
		string text = null;
		if (element != null)
		{
			text = element.keywordStyle;
		}
		return text;
	}

	public static string GetKeywordStyle(Element element)
	{
		string text = null;
		if (element != null)
		{
			text = element.keywordStyle;
		}
		return text;
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

	public static string StripTextFormatting(string original)
	{
		return Regex.Replace(original, "<[^>]*>([^<]*)<[^>]*>", "$1");
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

	public static string GenerateRandomDuplicantName(bool isMaleDuplicant)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		List<string> list = new List<string>(NAMEGEN.DUPLICANT.NAME.UNISEX);
		list.AddRange((!isMaleDuplicant) ? NAMEGEN.DUPLICANT.NAME.FEMALE : NAMEGEN.DUPLICANT.NAME.MALE);
		text3 = list.GetRandom<string>();
		bool flag = global::UnityEngine.Random.Range(0f, 1f) > 0.7f;
		if (flag)
		{
			List<string> list2 = new List<string>(NAMEGEN.DUPLICANT.PREFIX.UNISEX);
			list2.AddRange((!isMaleDuplicant) ? NAMEGEN.DUPLICANT.PREFIX.FEMALE : NAMEGEN.DUPLICANT.PREFIX.MALE);
			text = list2.GetRandom<string>();
		}
		if (!string.IsNullOrEmpty(text))
		{
			text += " ";
		}
		bool flag2 = global::UnityEngine.Random.Range(0f, 1f) >= 0.9f;
		if (flag2)
		{
			List<string> list3 = new List<string>(NAMEGEN.DUPLICANT.SUFFIX.UNISEX);
			list3.AddRange((!isMaleDuplicant) ? NAMEGEN.DUPLICANT.SUFFIX.FEMALE : NAMEGEN.DUPLICANT.SUFFIX.MALE);
			text2 = list3.GetRandom<string>();
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text2 = " " + text2;
		}
		return text + text3 + text2;
	}

	public static GameUtil.TemperatureUnit temperatureUnit;

	public static GameUtil.MassUnit massUnit;

	public enum UnitClass
	{
		SimpleFloat,
		SimpleInteger,
		Temperature,
		Mass,
		Calories,
		Percent
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

	public enum TemperatureInterpretation
	{
		Absolute,
		Relative
	}

	public enum TimeSlice
	{
		None,
		PerSecond,
		PerCycle
	}
}
