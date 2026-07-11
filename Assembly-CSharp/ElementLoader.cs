using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProcGenGame;
using UnityEngine;

public class ElementLoader
{
	public static void Load(ref Hashtable substanceList, ElementLoader.SolidEntry[] solid_entries, ElementLoader.LiquidEntry[] liquid_entries, ElementLoader.GasEntry[] gas_entries, SubstanceTable substanceTable)
	{
		ElementLoader.SetupElementsTable();
		ElementLoader.ParseCommon(solid_entries, ref substanceList, substanceTable, TagManager.Create("Solid"), Element.State.Solid);
		ElementLoader.ParseSolid(solid_entries);
		ElementLoader.ParseCommon(liquid_entries, ref substanceList, substanceTable, TagManager.Create("Liquid"), Element.State.Liquid);
		ElementLoader.ParseLiquid(liquid_entries);
		ElementLoader.ParseCommon(gas_entries, ref substanceList, substanceTable, TagManager.Create("Gas"), Element.State.Gas);
		ElementLoader.ParseGas(gas_entries);
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
			if (substance.name == null || substance.name == string.Empty)
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

	public static byte GetElementIndex(Tag element_tag)
	{
		byte b = byte.MaxValue;
		for (int i = 0; i < ElementLoader.elements.Count; i++)
		{
			Element element = ElementLoader.elements[i];
			if (element_tag == element.tag)
			{
				b = (byte)i;
				break;
			}
		}
		return b;
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
		if (ElementLoader.elements != null)
		{
			return;
		}
		SimHashes[] array = Enum.GetValues(typeof(SimHashes)) as SimHashes[];
		ElementLoader.elements = new List<Element>();
		ElementLoader.elementTable = new Dictionary<int, Element>();
		foreach (SimHashes simHashes in array)
		{
			Element element = new Element();
			element.id = simHashes;
			element.name = Strings.Get("STRINGS.ELEMENTS." + simHashes.ToString().ToUpper() + ".NAME");
			element.nameUpperCase = element.name.ToUpper();
			ElementLoader.elements.Add(element);
			ElementLoader.elementTable[(int)element.id] = element;
		}
	}

	private static SimHashes GetID(int column, int row, string[,] grid, SimHashes defaultValue = SimHashes.Vacuum)
	{
		if (column >= grid.GetLength(0) || row > grid.GetLength(1))
		{
			Output.LogError(new object[] { string.Format("Could not find element at loc [{0},{1}] grid is only [{2},{3}]", new object[]
			{
				column,
				row,
				grid.GetLength(0),
				grid.GetLength(1)
			}) });
			return defaultValue;
		}
		string text = grid[column, row];
		if (text == null || text == string.Empty)
		{
			return defaultValue;
		}
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
		return (SimHashes)obj;
	}

	private static SpawnFXHashes GetSpawnFX(int column, int row, string[,] grid)
	{
		if (column >= grid.GetLength(0) || row > grid.GetLength(1))
		{
			Output.LogError(new object[] { string.Format("Could not find SpawnFXHashes at loc [{0},{1}] grid is only [{2},{3}]", new object[]
			{
				column,
				row,
				grid.GetLength(0),
				grid.GetLength(1)
			}) });
			return SpawnFXHashes.None;
		}
		string text = grid[column, row];
		if (text == null || text == string.Empty)
		{
			return SpawnFXHashes.None;
		}
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
		return (SpawnFXHashes)obj;
	}

	private static Tag CreateMaterialCategoryTag(Element element, Tag phaseTag, string materialCategoryField)
	{
		if (!string.IsNullOrEmpty(materialCategoryField))
		{
			Tag tag = TagManager.Create(materialCategoryField);
			if (!GameTags.MaterialCategories.Contains(tag) && !GameTags.IgnoredMaterialCategories.Contains(tag))
			{
				global::Debug.LogWarningFormat("Element {0} has category {1}, but that isn't in GameTags.MaterialCategores!", new object[] { element.id, materialCategoryField });
			}
			return tag;
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
					list.Add(TagManager.Create(text));
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

	private static void ParseCommon(ElementLoader.ElementEntry[] entries, ref Hashtable substance_list, SubstanceTable substance_table, Tag phase_tag, Element.State state)
	{
		foreach (ElementLoader.ElementEntry elementEntry in entries)
		{
			ElementLoader.ParseCommon(elementEntry, ref substance_list, substance_table, phase_tag, state);
		}
	}

	private static void ParseCommon(ElementLoader.ElementEntry entry, ref Hashtable substance_list, SubstanceTable substance_table, Tag phase_tag, Element.State state)
	{
		Element element = ElementLoader.FindElementByHash(entry.elementId);
		element.tag = TagManager.Create(element.id.ToString(), element.name);
		element.defaultValues = new Sim.PhysicsData
		{
			temperature = entry.defaultTemperature,
			mass = entry.defaultMass,
			pressure = entry.defaultPressure
		};
		element.state = state;
		if (element.id == SimHashes.Void || element.id == SimHashes.Vacuum)
		{
			element.state = Element.State.Vacuum;
		}
		element.molarMass = entry.molarMass;
		element.specificHeatCapacity = entry.specificHeatCapacity;
		element.thermalConductivity = entry.thermalConductivity;
		element.solidSurfaceAreaMultiplier = entry.solidSurfaceAreaMultiplier;
		element.liquidSurfaceAreaMultiplier = entry.liquidSurfaceAreaMultiplier;
		element.gasSurfaceAreaMultiplier = entry.gasSurfaceAreaMultiplier;
		element.lightAbsorptionFactor = entry.lightAbsorptionFactor;
		element.lowTempTransitionTarget = entry.lowTempTransitionTarget;
		element.lowTemp = entry.lowTemp;
		element.highTempTransitionTarget = entry.highTempTransitionTarget;
		element.highTemp = entry.highTemp;
		element.sublimateId = entry.sublimateId;
		element.sublimateFX = entry.sublimateFx;
		element.materialCategory = ElementLoader.CreateMaterialCategoryTag(element, phase_tag, entry.materialCategory);
		element.oreTags = ElementLoader.CreateOreTags(element, phase_tag, entry.tags);
		element.disabled = entry.isDisabled;
		if (substance_table != null && !ElementLoader.SetOrCreateSubstanceForElement(element.id, ref substance_list, substance_table))
		{
			global::Debug.LogWarning("Missing substance for element: " + element.id.ToString(), null);
		}
	}

	private static void ParseSolid(ElementLoader.SolidEntry entry)
	{
		Element element = ElementLoader.FindElementByHash(entry.elementId);
		element.strength = entry.strength;
		element.maxMass = entry.maxMass;
		element.hardness = entry.hardness;
		GameTags.SolidElements.Add(element.tag);
	}

	private static void ParseSolid(ElementLoader.SolidEntry[] solid_entries)
	{
		foreach (ElementLoader.SolidEntry solidEntry in solid_entries)
		{
			ElementLoader.ParseSolid(solidEntry);
		}
	}

	private static void ParseLiquid(ElementLoader.LiquidEntry entry)
	{
		Element element = ElementLoader.FindElementByHash(entry.elementId);
		element.maxMass = entry.maxMassForLiquidUnderCompression;
		element.maxCompression = entry.maxMassForLiquidUnderCompression;
		element.viscosity = entry.speed;
		element.minHorizontalLiquidFlow = entry.minHorizontalLiquidFlow;
		element.minVerticalLiquidFlow = entry.minVerticalLiquidFlow;
		element.toxicity = entry.toxicity;
		element.convertId = entry.convertId;
		element.highTempTransitionOreID = SimHashes.Vacuum;
		element.highTempTransitionOreMassConversion = 0f;
		if (entry.highTempTransitionOreId != (SimHashes)0)
		{
			element.highTempTransitionOreID = entry.highTempTransitionOreId;
			element.highTempTransitionOreMassConversion = entry.highTempTransitionOreMassConversion;
		}
		GameTags.LiquidElements.Add(element.tag);
	}

	private static void ParseLiquid(ElementLoader.LiquidEntry[] liquid_entries)
	{
		foreach (ElementLoader.LiquidEntry liquidEntry in liquid_entries)
		{
			ElementLoader.ParseLiquid(liquidEntry);
		}
	}

	private static void ParseGas(ElementLoader.GasEntry[] gas_entries)
	{
		foreach (ElementLoader.GasEntry gasEntry in gas_entries)
		{
			ElementLoader.ParseGas(gasEntry);
		}
	}

	private static void ParseGas(ElementLoader.GasEntry entry)
	{
		Element element = ElementLoader.FindElementByHash(entry.elementId);
		element.defaultValues.mass = 1f;
		element.flow = entry.flow;
		element.toxicity = entry.toxicity;
		element.maxMass = 1.8f;
		element.lowTempTransitionOreID = SimHashes.Vacuum;
		element.lowTempTransitionOreMassConversion = 0f;
		if (entry.lowTempTransitionOreId != (SimHashes)0)
		{
			element.lowTempTransitionOreID = entry.lowTempTransitionOreId;
			element.lowTempTransitionOreMassConversion = entry.lowTempTransitionOreMassConversion;
		}
		GameTags.GasElements.Add(element.tag);
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
			ElementLoader.elements[j].idx = (byte)j;
		}
	}

	public static List<Element> elements;

	public static Dictionary<int, Element> elementTable;

	private static readonly Color noColour = new Color(0f, 0f, 0f, 0f);

	private static readonly char[] listSeparators = new char[] { ',', ' ', '|' };

	private static SimHashes getElementIndexHash;

	private static Predicate<Element> getElementIndexCallback = (Element e) => ElementLoader.getElementIndexHash == e.id;

	public class ElementEntry : Resource
	{
		public SimHashes elementId;

		public float specificHeatCapacity;

		public float thermalConductivity;

		public float solidSurfaceAreaMultiplier;

		public float liquidSurfaceAreaMultiplier;

		public float gasSurfaceAreaMultiplier;

		public float defaultMass;

		public float defaultTemperature;

		public float defaultPressure;

		public float molarMass;

		public float lightAbsorptionFactor;

		public SimHashes lowTempTransitionTarget = SimHashes.Unobtanium;

		public float lowTemp;

		public SimHashes highTempTransitionTarget = SimHashes.Unobtanium;

		public float highTemp = 10000f;

		public SimHashes sublimateId;

		public SpawnFXHashes sublimateFx;

		public string materialCategory;

		public string tags;

		public bool isDisabled;

		public string notes;
	}

	public class SolidEntry : ElementLoader.ElementEntry
	{
		public float strength;

		public float maxMass;

		public byte hardness;
	}

	public class LiquidEntry : ElementLoader.ElementEntry
	{
		public float maxMassForLiquidUnderCompression;

		public float toxicity;

		public float liquidCompression;

		public float speed;

		public float minHorizontalLiquidFlow;

		public float minVerticalLiquidFlow;

		public SimHashes convertId;

		public SimHashes highTempTransitionOreId;

		public float highTempTransitionOreMassConversion;
	}

	public class GasEntry : ElementLoader.ElementEntry
	{
		public float flow;

		public float toxicity;

		public SimHashes lowTempTransitionOreId;

		public float lowTempTransitionOreMassConversion;
	}
}
