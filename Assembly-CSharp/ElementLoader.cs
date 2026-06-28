using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Klei;
using UnityEngine;

public class ElementLoader
{
	public static void Load(ref Hashtable substanceList, string textSolid, string textLiquid, string textGas, SubstanceTable substanceTable)
	{
		ElementLoader.SetupElementsTable();
		string[,] array = CSVReader.SplitCsvGrid(textSolid, "Solid Elements");
		ElementLoader.ParseSolid(array, ref substanceList, substanceTable);
		array = CSVReader.SplitCsvGrid(textLiquid, "Liquid Elements");
		ElementLoader.ParseLiquid(array, ref substanceList, substanceTable);
		array = CSVReader.SplitCsvGrid(textGas, "Gas Elements");
		ElementLoader.ParseGas(array, ref substanceList, substanceTable);
		ElementLoader.FinaliseElementsTable(ref substanceList, substanceTable);
		WorldGen.SetupDefaultElements();
	}

	private static bool SetOrCreateSubstanceForElement(SimHashes key, ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		bool flag = false;
		Element element = ElementLoader.FindElementByHash(key);
		if (!substanceList.ContainsKey(key))
		{
			flag = true;
			Substance substance = null;
			if (substanceTable != null)
			{
				substance = substanceTable.GetSubstance(key);
			}
			if (substance == null)
			{
				substance = new Substance();
				substanceTable.GetList().Add(substance);
			}
			ElementLoader.CleanupSubstance(substance, element);
			substance.elementID = key;
			substance.renderedByWorld = element.IsSolid;
			substance.idx = substanceList.Count;
			if (substance.debugColour == ElementLoader.noColour)
			{
				int length = Enum.GetValues(typeof(SimHashes)).Length;
				int idx = substance.idx;
				ColourHSV colourHSV = new ColourHSV(350f * ((float)idx / (float)length), 1f, 1f);
				substance.debugColour = colourHSV.ToColor();
			}
			if (substance.name == null || substance.name == string.Empty)
			{
				substance.name = key.ToString();
			}
			substance.audioConfig = ElementsAudio.Instance.GetConfigForElement(key);
			if (substance.audioConfig == null)
			{
				Debug.LogWarning("Missing audio config for element [" + substance.name + "]");
			}
			substanceList.Add(key, substance);
		}
		element.substance = substanceList[key] as Substance;
		return flag;
	}

	private static void CleanupSubstance(Substance substance, Element element)
	{
	}

	public static Element GetElement(string name)
	{
		SimHashes simHashes = (SimHashes)((int)Enum.Parse(typeof(SimHashes), name));
		return ElementLoader.FindElementByHash(simHashes);
	}

	public static int GetStateAsInt(Element element)
	{
		int num = 0;
		if (element.state == Element.State.Solid || element.state == Element.State.Liquid || element.state == Element.State.Gas)
		{
			switch (element.state)
			{
			case Element.State.Gas:
				num = 2;
				break;
			case Element.State.Liquid:
				num = 1;
				break;
			case Element.State.Solid:
				num = 0;
				break;
			}
		}
		else if (element.IsGas)
		{
			num = 2;
		}
		else if (element.IsLiquid)
		{
			num = 1;
		}
		else if (element.IsSolid)
		{
			num = 0;
		}
		return num;
	}

	public static Element FindElementByHash(SimHashes hash)
	{
		Element element = null;
		ElementLoader.elementTable.TryGetValue(hash, out element);
		return element;
	}

	public static int GetElementIndex(SimHashes hash)
	{
		return ElementLoader.elements.FindIndex((Element e) => e.id == hash);
	}

	public static Element GetElement(Tag tag)
	{
		foreach (Element element in ElementLoader.elements)
		{
			Tag tag2 = TagManager.Create(element.id);
			if (tag2 == tag)
			{
				return element;
			}
		}
		return null;
	}

	public static SimHashes GetElementID(Tag tag)
	{
		foreach (Element element in ElementLoader.elements)
		{
			Tag tag2 = TagManager.Create(element.id);
			if (tag2 == tag)
			{
				return element.id;
			}
		}
		return SimHashes.Vacuum;
	}

	private static void SetupElementsTable()
	{
		SimHashes[] array = Enum.GetValues(typeof(SimHashes)) as SimHashes[];
		ElementLoader.elements = new List<Element>();
		ElementLoader.elementTable = new Dictionary<SimHashes, Element>();
		foreach (SimHashes simHashes in array)
		{
			Element element = new Element();
			element.id = simHashes;
			ElementLoader.elements.Add(element);
			ElementLoader.elementTable[element.id] = element;
		}
	}

	private static string GetID(int column, int row, string[,] grid)
	{
		if (grid[0, row] == null || grid[0, row] == string.Empty)
		{
			return null;
		}
		if (grid[column, row] == string.Empty)
		{
			return null;
		}
		return grid[column, row];
	}

	private static Tag CreateMaterialCategoryTag(Element element, Tag phaseTag, string materialCategoryField)
	{
		if (!string.IsNullOrEmpty(materialCategoryField))
		{
			return new Tag(materialCategoryField);
		}
		return phaseTag;
	}

	private static Tag[] CreateOreTags(Element element, Tag phaseTag, string tagsField)
	{
		List<Tag> list = new List<Tag>();
		if (!string.IsNullOrEmpty(tagsField))
		{
			string[] array = tagsField.Split(ElementLoader.listSeparators);
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(new Tag(text));
				}
			}
		}
		list.Add(phaseTag);
		if (element.materialCategory.IsValid && !list.Contains(element.materialCategory))
		{
			list.Add(element.materialCategory);
		}
		return list.ToArray();
	}

	private static void ParseSolid(string[,] grid, ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		bool flag = substanceTable == null;
		for (int i = 1; i < grid.GetUpperBound(1); i++)
		{
			string id = ElementLoader.GetID(0, i, grid);
			if (id != null)
			{
				object obj = null;
				try
				{
					obj = Enum.Parse(typeof(SimHashes), id);
				}
				catch (Exception ex)
				{
					Output.LogError(new object[] { string.Format("Could not find element {0}: {1}", id, ex.ToString()) });
					goto IL_02BA;
				}
				SimHashes simHashes = (SimHashes)((int)obj);
				Element element = ElementLoader.FindElementByHash(simHashes);
				Sim.PhysicsData physicsData;
				physicsData.temperature = 0f;
				physicsData.pressure = 0f;
				physicsData.mass = 0f;
				float num = float.Parse(grid[1, i]);
				float num2 = float.Parse(grid[3, i]);
				element.strength = float.Parse(grid[4, i]);
				float num3 = float.Parse(grid[12, i]);
				float num4 = float.Parse(grid[9, i]);
				float num5 = float.Parse(grid[5, i]);
				physicsData.mass = float.Parse(grid[8, i]);
				if (simHashes != SimHashes.Void && simHashes != SimHashes.Vacuum)
				{
					element.state = Element.State.Solid;
				}
				element.lowTemp = 0f;
				element.highTemp = num5;
				element.specificHeatCapacity = num;
				element.thermalConductivity = float.Parse(grid[2, i]);
				element.electricalConductivity = num2;
				element.molarMass = num3;
				physicsData.pressure = 0f;
				physicsData.temperature = float.Parse(grid[7, i]);
				element.defaultValues = physicsData;
				element.maxMass = num4;
				element.emitDistance = float.Parse(grid[13, i]);
				element.emitIntensity = int.Parse(grid[14, i]);
				element.materialCategory = ElementLoader.CreateMaterialCategoryTag(element, new Tag("Solid"), grid[15, i]);
				element.oreTags = ElementLoader.CreateOreTags(element, new Tag("Solid"), grid[16, i]);
				if (!flag)
				{
					ElementLoader.SetOrCreateSubstanceForElement(simHashes, ref substanceList, substanceTable);
				}
				SimHashes simHashes2 = SimHashes.Unobtanium;
				string text = grid[6, i];
				if (text != null && text != string.Empty)
				{
					object obj2 = Enum.Parse(typeof(SimHashes), text);
					if (obj2 != null)
					{
						simHashes2 = (SimHashes)((int)obj2);
					}
				}
				element.highTempTransitionTarget = simHashes2;
				element.hardness = byte.Parse(grid[11, i]);
				element.keywordStyle = grid[17, i];
				element.tag = new Tag(element.id.ToString());
			}
			IL_02BA:;
		}
	}

	private static void ParseLiquid(string[,] grid, ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		bool flag = substanceTable == null;
		for (int i = 1; i < grid.GetUpperBound(1); i++)
		{
			string id = ElementLoader.GetID(0, i, grid);
			if (id != null)
			{
				object obj = null;
				try
				{
					obj = Enum.Parse(typeof(SimHashes), id);
				}
				catch (Exception ex)
				{
					Output.LogError(new object[] { string.Format("Could not find element {0} : {1}", id, ex.ToString()) });
					goto IL_03EF;
				}
				SimHashes simHashes = (SimHashes)((int)obj);
				Element element = ElementLoader.FindElementByHash(simHashes);
				Sim.PhysicsData physicsData;
				physicsData.temperature = 0f;
				physicsData.pressure = 0f;
				physicsData.mass = 0f;
				physicsData.mass = 10000f;
				float num = float.Parse(grid[6, i]);
				float num2 = float.Parse(grid[17, i]);
				float num3 = float.Parse(grid[1, i]);
				float num4 = float.Parse(grid[2, i]);
				float num5 = float.Parse(grid[10, i]);
				float num6 = float.Parse(grid[9, i]);
				physicsData.mass = float.Parse(grid[16, i]);
				element.state = Element.State.Liquid;
				element.lowTemp = num6;
				element.highTemp = num5;
				element.specificHeatCapacity = num;
				element.thermalConductivity = float.Parse(grid[7, i]);
				element.molarMass = num2;
				element.maxCompression = num4;
				element.viscosity = float.Parse(grid[3, i]);
				element.minHorizontalLiquidFlow = float.Parse(grid[4, i]);
				element.minVerticalLiquidFlow = float.Parse(grid[5, i]);
				physicsData.temperature = float.Parse(grid[15, i]);
				physicsData.pressure = 0f;
				if (physicsData.temperature < 5f)
				{
				}
				element.defaultValues = physicsData;
				element.maxMass = num3;
				element.emitDistance = float.Parse(grid[18, i]);
				element.emitIntensity = int.Parse(grid[19, i]);
				element.toxicity = float.Parse(grid[20, i]);
				element.materialCategory = ElementLoader.CreateMaterialCategoryTag(element, new Tag("Liquid"), grid[21, i]);
				element.oreTags = ElementLoader.CreateOreTags(element, new Tag("Liquid"), grid[22, i]);
				if (!flag && !ElementLoader.SetOrCreateSubstanceForElement(simHashes, ref substanceList, substanceTable))
				{
					Output.Log(new object[] { "no substance for", simHashes });
				}
				SimHashes simHashes2 = SimHashes.Unobtanium;
				string text = grid[11, i];
				if (text != null && text != string.Empty)
				{
					object obj2 = Enum.Parse(typeof(SimHashes), text);
					if (obj2 != null)
					{
						simHashes2 = (SimHashes)((int)obj2);
					}
				}
				element.lowTempTransitionTarget = simHashes2;
				SimHashes simHashes3 = SimHashes.Unobtanium;
				string text2 = grid[12, i];
				if (text2 != null && text2 != string.Empty)
				{
					object obj3 = Enum.Parse(typeof(SimHashes), text2);
					if (obj3 != null)
					{
						simHashes3 = (SimHashes)((int)obj3);
					}
				}
				element.highTempTransitionTarget = simHashes3;
				string text3 = grid[13, i];
				string text4 = grid[14, i];
				if (text3 != null && text3 != string.Empty)
				{
					object obj4 = Enum.Parse(typeof(SimHashes), text3);
					if (obj4 != null)
					{
						element.highTempTransitionOreID = (SimHashes)((int)obj4);
						element.highTempTransitionOreMassConversion = float.Parse(text4);
					}
				}
				else
				{
					element.highTempTransitionOreID = SimHashes.Vacuum;
					element.highTempTransitionOreMassConversion = 0f;
				}
				element.tag = new Tag(element.id.ToString());
				element.keywordStyle = grid[23, i];
			}
			IL_03EF:;
		}
	}

	private static void ParseGas(string[,] grid, ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		bool flag = substanceTable == null;
		for (int i = 1; i < grid.GetUpperBound(1); i++)
		{
			string id = ElementLoader.GetID(0, i, grid);
			if (id != null)
			{
				object obj = null;
				try
				{
					obj = Enum.Parse(typeof(SimHashes), id);
				}
				catch (Exception ex)
				{
					Output.LogError(new object[] { string.Format("Could not find element {0} : {1}", id, ex.ToString()) });
					goto IL_0295;
				}
				SimHashes simHashes = (SimHashes)((int)obj);
				Element element = ElementLoader.FindElementByHash(simHashes);
				Sim.PhysicsData physicsData;
				physicsData.temperature = 0f;
				physicsData.pressure = 0f;
				physicsData.mass = 1f;
				float num = float.Parse(grid[1, i]);
				float num2 = float.Parse(grid[8, i]);
				float num3 = float.Parse(grid[4, i]);
				float num4 = float.Parse(grid[3, i]);
				element.state = Element.State.Gas;
				element.lowTemp = num3;
				element.highTemp = 10000f;
				element.specificHeatCapacity = num;
				element.molarMass = num2;
				element.thermalConductivity = float.Parse(grid[2, i]);
				element.flow = num4;
				element.emitDistance = float.Parse(grid[9, i]);
				element.emitIntensity = int.Parse(grid[10, i]);
				element.toxicity = float.Parse(grid[11, i]);
				physicsData.pressure = float.Parse(grid[7, i]);
				physicsData.temperature = float.Parse(grid[6, i]);
				physicsData.mass = 1f;
				if (physicsData.temperature < 5f)
				{
				}
				element.defaultValues = physicsData;
				float num5 = 1.8f;
				element.maxMass = num5;
				element.materialCategory = ElementLoader.CreateMaterialCategoryTag(element, new Tag("Gas"), grid[12, i]);
				element.oreTags = ElementLoader.CreateOreTags(element, new Tag("Gas"), grid[13, i]);
				if (!flag)
				{
					ElementLoader.SetOrCreateSubstanceForElement(simHashes, ref substanceList, substanceTable);
				}
				SimHashes simHashes2 = SimHashes.Unobtanium;
				string text = grid[5, i];
				if (text != null && text != string.Empty)
				{
					object obj2 = Enum.Parse(typeof(SimHashes), text);
					if (obj2 != null)
					{
						simHashes2 = (SimHashes)((int)obj2);
					}
				}
				element.lowTempTransitionTarget = simHashes2;
				element.tag = new Tag(element.id.ToString());
				element.keywordStyle = grid[14, i];
			}
			IL_0295:;
		}
	}

	private static void FinaliseElementsTable(ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		foreach (SimHashes simHashes in Enum.GetValues(typeof(SimHashes)) as SimHashes[])
		{
			Element element = ElementLoader.FindElementByHash(simHashes);
			if (element != null)
			{
				if (element.substance == null)
				{
					if (substanceTable == null)
					{
						element.substance = new Substance();
					}
					else
					{
						ElementLoader.SetOrCreateSubstanceForElement(element.id, ref substanceList, substanceTable);
					}
				}
				if (element.thermalConductivity == 0f)
				{
					Element element2 = element;
					element2.state |= Element.State.TemperatureInsulated;
				}
				if (element.strength == 0f)
				{
					Element element3 = element;
					element3.state |= Element.State.Unbreakable;
				}
				if (element.IsSolid)
				{
					Element element4 = ElementLoader.FindElementByHash(element.highTempTransitionTarget);
					if (element4 != null)
					{
						element.highTempTransition = element4;
					}
				}
				else if (element.IsLiquid)
				{
					Element element5 = ElementLoader.FindElementByHash(element.highTempTransitionTarget);
					if (element5 != null)
					{
						element.highTempTransition = element5;
					}
					Element element6 = ElementLoader.FindElementByHash(element.lowTempTransitionTarget);
					if (element6 != null)
					{
						element.lowTempTransition = element6;
					}
				}
				else if (element.IsGas)
				{
					Element element7 = ElementLoader.FindElementByHash(element.lowTempTransitionTarget);
					if (element7 != null)
					{
						element.lowTempTransition = element7;
					}
				}
			}
		}
		IOrderedEnumerable<Element> orderedEnumerable = from e in ElementLoader.elements
			orderby (int)(e.state & Element.State.Solid) descending, e.id
			select e;
		ElementLoader.elements = orderedEnumerable.ToList<Element>();
		for (int j = 0; j < ElementLoader.elements.Count; j++)
		{
			if (ElementLoader.elements[j].substance != null)
			{
				ElementLoader.elements[j].substance.idx = j;
			}
		}
	}

	public static List<Element> elements;

	public static Dictionary<SimHashes, Element> elementTable;

	private static Color noColour = new Color(0f, 0f, 0f, 0f);

	private static char[] listSeparators = new char[] { ',', ' ', '|' };

	private enum FieldNamesSolid
	{
		ID,
		SpecificHeat,
		ThermalConductivity,
		ElectricalConductivity,
		Strength,
		HighTemperature,
		HighTemperatureTransition,
		DefaultTemperature,
		DefaultMass,
		MaxMass,
		HardnessTier,
		Hardness,
		MolarMass,
		EmitDistance,
		EmitIntensity,
		MaterialCategory,
		Tags,
		KeywordStyle,
		AmbienceType,
		MiningSound,
		Notes
	}

	private enum FieldNamesLiquid
	{
		ID,
		MaxMassUnderCompression,
		LiquidCompression,
		Viscosity,
		MinHorizontalLiquidFlow,
		MinVerticalLiquidFlow,
		SpecificHeat,
		ThermalConductivity,
		ElectricalConductivity,
		LowTemperature,
		HighTemperature,
		LowTemeratureTransition,
		HighTemperatureTransition,
		HighTemperatureTransitionOreID,
		HighTemperatureTransitionOreMassConversion,
		DefaultTemperature,
		DefaultMass,
		MolarMass,
		EmitDistance,
		EmitIntensity,
		Toxicity,
		MaterialCategory,
		Tags,
		KeywordStyle,
		AmbienceType,
		Notes
	}

	private enum FieldNamesGas
	{
		ID,
		SpecificHeat,
		ThermalConductivity,
		Flow,
		LowTemperature,
		LowTemperatureTransition,
		DefaultTemperature,
		DefaultPressure,
		MolarMass,
		EmitDistance,
		EmitIntensity,
		Toxicity,
		MaterialCategory,
		Tags,
		KeywordStyle,
		AmbienceType,
		Notes
	}
}
