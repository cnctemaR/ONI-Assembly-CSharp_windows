using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Klei;
using ProcGenGame;
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
				substance.debugColour = Color.HSVToRGB((float)idx / (float)length, 1f, 1f);
			}
			if (substance.name == null || substance.name == "")
			{
				substance.name = key.ToString();
			}
			substance.audioConfig = ElementsAudio.Instance.GetConfigForElement(key);
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
		SimHashes simHashes = (SimHashes)Enum.Parse(typeof(SimHashes), name);
		return ElementLoader.FindElementByHash(simHashes);
	}

	public static int GetStateAsInt(Element element)
	{
		int num = 0;
		if (element.state == Element.State.Solid || element.state == Element.State.Liquid || element.state == Element.State.Gas)
		{
			Element.State state = element.state;
			if (state != Element.State.Solid)
			{
				if (state != Element.State.Liquid)
				{
					if (state == Element.State.Gas)
					{
						num = 2;
					}
				}
				else
				{
					num = 1;
				}
			}
			else
			{
				num = 0;
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

	public static Element FindElementByName(string name)
	{
		Element element = null;
		object obj = Enum.Parse(typeof(SimHashes), name);
		if (obj != null)
		{
			SimHashes simHashes = (SimHashes)obj;
			ElementLoader.elementTable.TryGetValue((int)simHashes, out element);
		}
		return element;
	}

	public static Element FindElementByHash(SimHashes hash)
	{
		Element element = null;
		ElementLoader.elementTable.TryGetValue((int)hash, out element);
		return element;
	}

	public static int GetElementIndex(SimHashes hash)
	{
		ElementLoader.getElementIndexHash = hash;
		return ElementLoader.elements.FindIndex(ElementLoader.getElementIndexCallback);
	}

	public static Element GetElement(Tag tag)
	{
		for (int i = 0; i < ElementLoader.elements.Count; i++)
		{
			Element element = ElementLoader.elements[i];
			if (tag == element.tag)
			{
				return element;
			}
		}
		return null;
	}

	public static SimHashes GetElementID(Tag tag)
	{
		for (int i = 0; i < ElementLoader.elements.Count; i++)
		{
			Element element = ElementLoader.elements[i];
			if (tag == element.tag)
			{
				return element.id;
			}
		}
		return SimHashes.Vacuum;
	}

	private static void SetupElementsTable()
	{
		if (ElementLoader.elements == null)
		{
			SimHashes[] array = Enum.GetValues(typeof(SimHashes)) as SimHashes[];
			ElementLoader.elements = new List<Element>();
			ElementLoader.elementTable = new Dictionary<int, Element>();
			foreach (SimHashes simHashes in array)
			{
				Element element = new Element();
				element.id = simHashes;
				ElementLoader.elements.Add(element);
				ElementLoader.elementTable[(int)element.id] = element;
			}
		}
	}

	private static SimHashes GetID(int column, int row, string[,] grid, SimHashes defaultValue = SimHashes.Vacuum)
	{
		SimHashes simHashes;
		if (column >= grid.GetLength(0) || row > grid.GetLength(1))
		{
			Output.LogError(new object[] { string.Format("Could not find element at loc [{0},{1}] grid is only [{2},{3}]", new object[]
			{
				column,
				row,
				grid.GetLength(0),
				grid.GetLength(1)
			}) });
			simHashes = defaultValue;
		}
		else
		{
			string text = grid[column, row];
			if (text == null || text == "")
			{
				simHashes = defaultValue;
			}
			else
			{
				object obj = null;
				try
				{
					obj = Enum.Parse(typeof(SimHashes), text);
				}
				catch (Exception ex)
				{
					Output.LogError(new object[] { string.Format("Could not find element {0}: {1}", text, ex.ToString()) });
					return defaultValue;
				}
				simHashes = (SimHashes)obj;
			}
		}
		return simHashes;
	}

	private static SpawnFXHashes GetSpawnFX(int column, int row, string[,] grid)
	{
		SpawnFXHashes spawnFXHashes;
		if (column >= grid.GetLength(0) || row > grid.GetLength(1))
		{
			Output.LogError(new object[] { string.Format("Could not find SpawnFXHashes at loc [{0},{1}] grid is only [{2},{3}]", new object[]
			{
				column,
				row,
				grid.GetLength(0),
				grid.GetLength(1)
			}) });
			spawnFXHashes = SpawnFXHashes.None;
		}
		else
		{
			string text = grid[column, row];
			if (text == null || text == "")
			{
				spawnFXHashes = SpawnFXHashes.None;
			}
			else
			{
				object obj = null;
				try
				{
					obj = Enum.Parse(typeof(SpawnFXHashes), text);
				}
				catch (Exception ex)
				{
					Output.LogError(new object[] { string.Format("Could not find FX {0}: {1}", text, ex.ToString()) });
					return SpawnFXHashes.None;
				}
				spawnFXHashes = (SpawnFXHashes)obj;
			}
		}
		return spawnFXHashes;
	}

	private static Tag CreateMaterialCategoryTag(Element element, Tag phaseTag, string materialCategoryField)
	{
		Tag tag2;
		if (!string.IsNullOrEmpty(materialCategoryField))
		{
			Tag tag = TagManager.Create(materialCategoryField, null);
			if (!GameTags.MaterialCategories.Contains(tag) && !GameTags.IgnoredMaterialCategories.Contains(tag))
			{
				global::Debug.LogWarningFormat("Element {0} has category {1}, but that isn't in GameTags.MaterialCategores!", new object[] { element.id, materialCategoryField });
			}
			tag2 = tag;
		}
		else
		{
			tag2 = phaseTag;
		}
		return tag2;
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
					list.Add(TagManager.Create(text, null));
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
		for (int i = 0; i < grid.GetUpperBound(1); i++)
		{
			SimHashes id = ElementLoader.GetID(0, i, grid, SimHashes.Vacuum);
			Element element = ElementLoader.FindElementByHash(id);
			Sim.PhysicsData physicsData;
			physicsData.temperature = 0f;
			physicsData.pressure = 0f;
			physicsData.mass = 0f;
			try
			{
				float num = float.Parse(grid[1, i]);
				float num2 = float.Parse(grid[6, i]);
				element.strength = float.Parse(grid[7, i]);
				float num3 = float.Parse(grid[17, i]);
				float num4 = float.Parse(grid[14, i]);
				float num5 = float.Parse(grid[8, i]);
				physicsData.mass = float.Parse(grid[13, i]);
				if (id != SimHashes.Void && id != SimHashes.Vacuum)
				{
					element.state = Element.State.Solid;
				}
				element.lowTemp = 0f;
				element.highTemp = num5;
				element.specificHeatCapacity = num;
				element.thermalConductivity = float.Parse(grid[2, i]);
				element.solidSurfaceAreaMultiplier = float.Parse(grid[3, i]);
				element.liquidSurfaceAreaMultiplier = float.Parse(grid[4, i]);
				element.gasSurfaceAreaMultiplier = float.Parse(grid[5, i]);
				element.electricalConductivity = num2;
				element.molarMass = num3;
				physicsData.pressure = 0f;
				physicsData.temperature = float.Parse(grid[12, i]);
				element.defaultValues = physicsData;
				element.maxMass = num4;
				element.emitDistance = float.Parse(grid[18, i]);
				element.emitIntensity = int.Parse(grid[19, i]);
				element.materialCategory = ElementLoader.CreateMaterialCategoryTag(element, TagManager.Create("Solid", null), grid[20, i]);
				element.oreTags = ElementLoader.CreateOreTags(element, TagManager.Create("Solid", null), grid[21, i]);
				if (!flag)
				{
					ElementLoader.SetOrCreateSubstanceForElement(id, ref substanceList, substanceTable);
				}
				element.highTempTransitionTarget = ElementLoader.GetID(9, i, grid, SimHashes.Unobtanium);
				element.sublimateId = ElementLoader.GetID(10, i, grid, (SimHashes)0);
				element.sublimateFX = ElementLoader.GetSpawnFX(11, i, grid);
				element.hardness = byte.Parse(grid[16, i]);
				element.tag = TagManager.Create(element.id.ToString(), element.name);
				GameTags.SolidElements.Add(element.tag);
			}
			catch (Exception ex)
			{
				global::Debug.LogError(string.Concat(new object[] { "Exception while trying to parse [", id, "] Solids csv file: ", ex.Message, "\n", ex.StackTrace }), null);
			}
		}
	}

	private static void ParseLiquid(string[,] grid, ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		bool flag = substanceTable == null;
		for (int i = 0; i < grid.GetUpperBound(1); i++)
		{
			SimHashes id = ElementLoader.GetID(0, i, grid, SimHashes.Vacuum);
			Element element = ElementLoader.FindElementByHash(id);
			Sim.PhysicsData physicsData;
			physicsData.temperature = 0f;
			physicsData.pressure = 0f;
			physicsData.mass = 0f;
			physicsData.mass = 10000f;
			try
			{
				float num = float.Parse(grid[6, i]);
				float num2 = float.Parse(grid[23, i]);
				float num3 = float.Parse(grid[1, i]);
				float num4 = float.Parse(grid[2, i]);
				float num5 = float.Parse(grid[13, i]);
				float num6 = float.Parse(grid[12, i]);
				physicsData.mass = float.Parse(grid[22, i]);
				element.state = Element.State.Liquid;
				element.lowTemp = num6;
				element.highTemp = num5;
				element.specificHeatCapacity = num;
				element.thermalConductivity = float.Parse(grid[7, i]);
				element.solidSurfaceAreaMultiplier = float.Parse(grid[8, i]);
				element.liquidSurfaceAreaMultiplier = float.Parse(grid[9, i]);
				element.gasSurfaceAreaMultiplier = float.Parse(grid[10, i]);
				element.molarMass = num2;
				element.maxCompression = num4;
				element.viscosity = float.Parse(grid[3, i]);
				element.minHorizontalLiquidFlow = float.Parse(grid[4, i]);
				element.minVerticalLiquidFlow = float.Parse(grid[5, i]);
				physicsData.temperature = float.Parse(grid[21, i]);
				physicsData.pressure = 0f;
				if (physicsData.temperature < 5f)
				{
				}
				element.defaultValues = physicsData;
				element.maxMass = num3;
				element.emitDistance = float.Parse(grid[24, i]);
				element.emitIntensity = int.Parse(grid[25, i]);
				element.toxicity = float.Parse(grid[26, i]);
				element.materialCategory = ElementLoader.CreateMaterialCategoryTag(element, TagManager.Create("Liquid", null), grid[27, i]);
				element.oreTags = ElementLoader.CreateOreTags(element, TagManager.Create("Liquid", null), grid[28, i]);
				if (!flag)
				{
					if (!ElementLoader.SetOrCreateSubstanceForElement(id, ref substanceList, substanceTable))
					{
						Output.Log(new object[] { "no substance for", id });
					}
				}
				element.lowTempTransitionTarget = ElementLoader.GetID(14, i, grid, SimHashes.Unobtanium);
				element.highTempTransitionTarget = ElementLoader.GetID(15, i, grid, SimHashes.Unobtanium);
				element.sublimateId = ElementLoader.GetID(16, i, grid, (SimHashes)0);
				element.sublimateFX = ElementLoader.GetSpawnFX(17, i, grid);
				element.convertId = ElementLoader.GetID(18, i, grid, (SimHashes)0);
				string text = grid[19, i];
				string text2 = grid[20, i];
				if (text != null && text != "")
				{
					object obj = Enum.Parse(typeof(SimHashes), text);
					if (obj != null)
					{
						element.highTempTransitionOreID = (SimHashes)obj;
						element.highTempTransitionOreMassConversion = float.Parse(text2);
					}
				}
				else
				{
					element.highTempTransitionOreID = SimHashes.Vacuum;
					element.highTempTransitionOreMassConversion = 0f;
				}
				element.tag = TagManager.Create(element.id.ToString(), element.name);
				GameTags.LiquidElements.Add(element.tag);
			}
			catch (Exception ex)
			{
				global::Debug.LogError(string.Concat(new object[] { "Exception while trying to parse [", id, "] Liquids csv file: ", ex.Message, "\n", ex.StackTrace }), null);
			}
		}
	}

	private static void ParseGas(string[,] grid, ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		bool flag = substanceTable == null;
		for (int i = 0; i < grid.GetUpperBound(1); i++)
		{
			SimHashes id = ElementLoader.GetID(0, i, grid, SimHashes.Vacuum);
			Element element = ElementLoader.FindElementByHash(id);
			Sim.PhysicsData physicsData;
			physicsData.temperature = 0f;
			physicsData.pressure = 0f;
			physicsData.mass = 1f;
			try
			{
				float num = float.Parse(grid[1, i]);
				float num2 = float.Parse(grid[11, i]);
				float num3 = float.Parse(grid[7, i]);
				float num4 = float.Parse(grid[6, i]);
				element.state = Element.State.Gas;
				element.lowTemp = num3;
				element.highTemp = 10000f;
				element.specificHeatCapacity = num;
				element.molarMass = num2;
				element.thermalConductivity = float.Parse(grid[2, i]);
				element.solidSurfaceAreaMultiplier = float.Parse(grid[3, i]);
				element.liquidSurfaceAreaMultiplier = float.Parse(grid[4, i]);
				element.gasSurfaceAreaMultiplier = float.Parse(grid[5, i]);
				element.flow = num4;
				element.emitDistance = float.Parse(grid[12, i]);
				element.emitIntensity = int.Parse(grid[13, i]);
				element.toxicity = float.Parse(grid[14, i]);
				physicsData.pressure = float.Parse(grid[10, i]);
				physicsData.temperature = float.Parse(grid[9, i]);
				physicsData.mass = 1f;
				if (physicsData.temperature < 5f)
				{
				}
				element.defaultValues = physicsData;
				float num5 = 1.8f;
				element.maxMass = num5;
				element.materialCategory = ElementLoader.CreateMaterialCategoryTag(element, TagManager.Create("Gas", null), grid[15, i]);
				element.oreTags = ElementLoader.CreateOreTags(element, TagManager.Create("Gas", null), grid[16, i]);
				if (!flag)
				{
					ElementLoader.SetOrCreateSubstanceForElement(id, ref substanceList, substanceTable);
				}
				element.lowTempTransitionTarget = ElementLoader.GetID(8, i, grid, SimHashes.Unobtanium);
				element.tag = TagManager.Create(element.id.ToString(), element.name);
				GameTags.GasElements.Add(element.tag);
			}
			catch (Exception ex)
			{
				global::Debug.LogError(string.Concat(new object[] { "Exception while trying to parse [", id, "] Gas csv file: ", ex.Message, "\n", ex.StackTrace }), null);
			}
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

	public static Dictionary<int, Element> elementTable;

	private static Color noColour = new Color(0f, 0f, 0f, 0f);

	private static char[] listSeparators = new char[] { ',', ' ', '|' };

	private static SimHashes getElementIndexHash;

	private static Predicate<Element> getElementIndexCallback = (Element e) => ElementLoader.getElementIndexHash == e.id;

	private enum FieldNamesSolid
	{
		ID,
		SpecificHeat,
		ThermalConductivity,
		SolidSurfaceAreaMultiplier,
		LiquidSurfaceAreaMultiplier,
		GasSurfaceAreaMultiplier,
		ElectricalConductivity,
		Strength,
		HighTemperature,
		HighTemperatureTransition,
		SublimateID,
		SublimateFX,
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
		SolidSurfaceAreaMultiplier,
		LiquidSurfaceAreaMultiplier,
		GasSurfaceAreaMultiplier,
		ElectricalConductivity,
		LowTemperature,
		HighTemperature,
		LowTemeratureTransition,
		HighTemperatureTransition,
		SublimateID,
		SublimateFX,
		ConvertID,
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
		SolidSurfaceAreaMultiplier,
		LiquidSurfaceAreaMultiplier,
		GasSurfaceAreaMultiplier,
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
