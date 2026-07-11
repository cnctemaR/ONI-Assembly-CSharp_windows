using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProcGenGame;
using STRINGS;
using UnityEngine;

public class ElementLoader
{
	public static void Load(ref Hashtable substanceList, string elementsFileContent, SubstanceTable substanceTable)
	{
		ElementLoader.ElementEntry[] array = JsonConvert.DeserializeObject<ElementLoader.ElementEntry[]>(elementsFileContent);
		ElementLoader.elements = new List<Element>();
		ElementLoader.elementTable = new Dictionary<int, Element>();
		foreach (ElementLoader.ElementEntry elementEntry in array)
		{
			int num = Hash.SDBMLower(elementEntry.elementId);
			Element element = new Element();
			element.id = (SimHashes)num;
			ElementLoader.elements.Add(element);
			ElementLoader.elementTable[num] = element;
			element.name = Strings.Get(elementEntry.localizationID);
			element.nameUpperCase = element.name.ToUpper();
			element.tag = TagManager.Create(elementEntry.elementId, element.name);
			ElementLoader.Copy(elementEntry, element);
		}
		ElementLoader.LoadUserElementData();
		foreach (Element element2 in ElementLoader.elements)
		{
			if (!ElementLoader.SetOrCreateSubstanceForElement(element2, ref substanceList, substanceTable))
			{
				global::Debug.LogWarning("Missing substance for element: " + element2.id.ToString(), null);
			}
		}
		ElementLoader.FinaliseElementsTable(ref substanceList, substanceTable);
		WorldGen.SetupDefaultElements();
	}

	private static void LoadUserElementData()
	{
		if (Global.Instance == null || Global.Instance.layeredFileSystem == null)
		{
			return;
		}
		foreach (string text in ElementLoader.additionalJSONFiles)
		{
			if (Global.Instance.layeredFileSystem.Exists(text))
			{
				string text2 = Global.Instance.layeredFileSystem.ReadText(text);
				ElementLoader.ElementEntry[] array = JsonConvert.DeserializeObject<ElementLoader.ElementEntry[]>(text2);
				ElementLoader.ElementEntry elementEntry = new ElementLoader.ElementEntry();
				foreach (ElementLoader.ElementEntry elementEntry2 in array)
				{
					int num = Hash.SDBMLower(elementEntry2.elementId);
					Element element = ElementLoader.FindElementByHash((SimHashes)num);
					if (element == null)
					{
						element = new Element();
						element.id = (SimHashes)num;
						element.name = Strings.Get(elementEntry2.localizationID);
						element.nameUpperCase = element.name.ToUpper();
						element.tag = TagManager.Create(elementEntry2.elementId, element.name);
						ElementLoader.elements.Add(element);
						ElementLoader.elementTable[(int)element.id] = element;
					}
					if (elementEntry2.specificHeatCapacity != elementEntry.specificHeatCapacity)
					{
						element.specificHeatCapacity = elementEntry2.specificHeatCapacity;
					}
					if (elementEntry2.thermalConductivity != elementEntry.thermalConductivity)
					{
						element.thermalConductivity = elementEntry2.thermalConductivity;
					}
					if (elementEntry2.molarMass != elementEntry.molarMass)
					{
						element.molarMass = elementEntry2.molarMass;
					}
					if (elementEntry2.strength != elementEntry.strength)
					{
						element.strength = elementEntry2.strength;
					}
					if (elementEntry2.flow != elementEntry.flow)
					{
						element.flow = elementEntry2.flow;
					}
					if (elementEntry2.maxMass != elementEntry.maxMass)
					{
						element.maxMass = elementEntry2.maxMass;
					}
					if (elementEntry2.liquidCompression != elementEntry.liquidCompression)
					{
						element.maxCompression = elementEntry2.liquidCompression;
					}
					if (elementEntry2.speed != elementEntry.speed)
					{
						element.viscosity = elementEntry2.speed;
					}
					if (elementEntry2.minHorizontalFlow != elementEntry.minHorizontalFlow)
					{
						element.minHorizontalFlow = elementEntry2.minHorizontalFlow;
					}
					if (elementEntry2.minVerticalFlow != elementEntry.minVerticalFlow)
					{
						element.minVerticalFlow = elementEntry2.minVerticalFlow;
					}
					if (elementEntry2.maxMass != elementEntry.maxMass)
					{
						element.maxMass = elementEntry2.maxMass;
					}
					if (elementEntry2.solidSurfaceAreaMultiplier != elementEntry.solidSurfaceAreaMultiplier)
					{
						element.solidSurfaceAreaMultiplier = elementEntry2.solidSurfaceAreaMultiplier;
					}
					if (elementEntry2.liquidSurfaceAreaMultiplier != elementEntry.liquidSurfaceAreaMultiplier)
					{
						element.liquidSurfaceAreaMultiplier = elementEntry2.liquidSurfaceAreaMultiplier;
					}
					if (elementEntry2.gasSurfaceAreaMultiplier != elementEntry.gasSurfaceAreaMultiplier)
					{
						element.gasSurfaceAreaMultiplier = elementEntry2.gasSurfaceAreaMultiplier;
					}
					if (elementEntry2.state != elementEntry.state)
					{
						element.state = elementEntry2.state;
					}
					if (elementEntry2.hardness != elementEntry.hardness)
					{
						element.hardness = elementEntry2.hardness;
					}
					if (elementEntry2.lowTemp != elementEntry.lowTemp)
					{
						element.lowTemp = elementEntry2.lowTemp;
					}
					if (elementEntry2.lowTempTransitionTarget != elementEntry.lowTempTransitionTarget)
					{
						element.lowTempTransitionTarget = (SimHashes)Hash.SDBMLower(elementEntry2.lowTempTransitionTarget);
					}
					if (elementEntry2.highTemp != elementEntry.highTemp)
					{
						element.highTemp = elementEntry2.highTemp;
					}
					if (elementEntry2.highTempTransitionTarget != elementEntry.highTempTransitionTarget)
					{
						element.highTempTransitionTarget = (SimHashes)Hash.SDBMLower(elementEntry2.highTempTransitionTarget);
					}
					if (elementEntry2.highTempTransitionOreId != elementEntry.highTempTransitionOreId)
					{
						element.highTempTransitionOreID = (SimHashes)Hash.SDBMLower(elementEntry2.highTempTransitionOreId);
					}
					if (elementEntry2.highTempTransitionOreMassConversion != elementEntry.highTempTransitionOreMassConversion)
					{
						element.highTempTransitionOreMassConversion = elementEntry2.highTempTransitionOreMassConversion;
					}
					if (elementEntry2.lowTempTransitionOreId != elementEntry.lowTempTransitionOreId)
					{
						element.lowTempTransitionOreID = (SimHashes)Hash.SDBMLower(elementEntry2.lowTempTransitionOreId);
					}
					if (elementEntry2.lowTempTransitionOreMassConversion != elementEntry.lowTempTransitionOreMassConversion)
					{
						element.lowTempTransitionOreMassConversion = elementEntry2.lowTempTransitionOreMassConversion;
					}
					if (elementEntry2.sublimateId != elementEntry.sublimateId)
					{
						element.sublimateId = (SimHashes)Hash.SDBMLower(elementEntry2.sublimateId);
					}
					if (elementEntry2.convertId != elementEntry.convertId)
					{
						element.convertId = (SimHashes)Hash.SDBMLower(elementEntry2.convertId);
					}
					if (elementEntry2.sublimateFx != elementEntry.sublimateFx)
					{
						element.sublimateFX = (SpawnFXHashes)Hash.SDBMLower(elementEntry2.sublimateFx);
					}
					if (elementEntry2.lightAbsorptionFactor != elementEntry.lightAbsorptionFactor)
					{
						element.lightAbsorptionFactor = elementEntry2.lightAbsorptionFactor;
					}
					Sim.PhysicsData defaultValues = element.defaultValues;
					if (elementEntry2.defaultTemperature != elementEntry.defaultTemperature)
					{
						defaultValues.temperature = elementEntry2.defaultTemperature;
					}
					if (elementEntry2.defaultMass != elementEntry.defaultMass)
					{
						defaultValues.mass = elementEntry2.defaultMass;
					}
					if (elementEntry2.defaultPressure != elementEntry.defaultPressure)
					{
						defaultValues.pressure = elementEntry2.defaultPressure;
					}
					element.defaultValues = defaultValues;
					if (elementEntry2.toxicity != elementEntry.toxicity)
					{
						element.toxicity = elementEntry2.toxicity;
					}
					Tag tag = TagManager.Create(elementEntry2.state.ToString());
					if (elementEntry2.materialCategory != elementEntry.materialCategory)
					{
						element.materialCategory = ElementLoader.CreateMaterialCategoryTag(element.id, tag, elementEntry2.materialCategory);
					}
					if (elementEntry2.tags != elementEntry.tags)
					{
						element.oreTags = ElementLoader.CreateOreTags(element.materialCategory, tag, elementEntry2.tags);
					}
					if (elementEntry2.buildMenuSort != elementEntry.buildMenuSort)
					{
						element.buildMenuSort = elementEntry2.buildMenuSort;
					}
					Element.State state = elementEntry2.state;
					if (state != Element.State.Solid)
					{
						if (state != Element.State.Liquid)
						{
							if (state == Element.State.Gas)
							{
								GameTags.GasElements.Add(element.tag);
							}
						}
						else
						{
							GameTags.LiquidElements.Add(element.tag);
						}
					}
					else
					{
						GameTags.SolidElements.Add(element.tag);
					}
				}
			}
		}
	}

	private static void Copy(ElementLoader.ElementEntry entry, Element elem)
	{
		int num = Hash.SDBMLower(entry.elementId);
		elem.tag = TagManager.Create(entry.elementId.ToString());
		elem.specificHeatCapacity = entry.specificHeatCapacity;
		elem.thermalConductivity = entry.thermalConductivity;
		elem.molarMass = entry.molarMass;
		elem.strength = entry.strength;
		elem.flow = entry.flow;
		elem.maxMass = entry.maxMass;
		elem.maxCompression = entry.liquidCompression;
		elem.viscosity = entry.speed;
		elem.minHorizontalFlow = entry.minHorizontalFlow;
		elem.minVerticalFlow = entry.minVerticalFlow;
		elem.maxMass = entry.maxMass;
		elem.solidSurfaceAreaMultiplier = entry.solidSurfaceAreaMultiplier;
		elem.liquidSurfaceAreaMultiplier = entry.liquidSurfaceAreaMultiplier;
		elem.gasSurfaceAreaMultiplier = entry.gasSurfaceAreaMultiplier;
		elem.state = entry.state;
		elem.hardness = entry.hardness;
		elem.lowTemp = entry.lowTemp;
		elem.lowTempTransitionTarget = (SimHashes)Hash.SDBMLower(entry.lowTempTransitionTarget);
		elem.highTemp = entry.highTemp;
		elem.highTempTransitionTarget = (SimHashes)Hash.SDBMLower(entry.highTempTransitionTarget);
		elem.highTempTransitionOreID = (SimHashes)Hash.SDBMLower(entry.highTempTransitionOreId);
		elem.highTempTransitionOreMassConversion = entry.highTempTransitionOreMassConversion;
		elem.lowTempTransitionOreID = (SimHashes)Hash.SDBMLower(entry.lowTempTransitionOreId);
		elem.lowTempTransitionOreMassConversion = entry.lowTempTransitionOreMassConversion;
		elem.sublimateId = (SimHashes)Hash.SDBMLower(entry.sublimateId);
		elem.convertId = (SimHashes)Hash.SDBMLower(entry.convertId);
		elem.sublimateFX = (SpawnFXHashes)Hash.SDBMLower(entry.sublimateFx);
		elem.lightAbsorptionFactor = entry.lightAbsorptionFactor;
		elem.toxicity = entry.toxicity;
		Tag tag = TagManager.Create(entry.state.ToString());
		elem.materialCategory = ElementLoader.CreateMaterialCategoryTag(elem.id, tag, entry.materialCategory);
		elem.oreTags = ElementLoader.CreateOreTags(elem.materialCategory, tag, entry.tags);
		elem.buildMenuSort = entry.buildMenuSort;
		Sim.PhysicsData physicsData = default(Sim.PhysicsData);
		physicsData.temperature = entry.defaultTemperature;
		physicsData.mass = entry.defaultMass;
		physicsData.pressure = entry.defaultPressure;
		Element.State state = entry.state;
		if (state != Element.State.Solid)
		{
			if (state != Element.State.Liquid)
			{
				if (state == Element.State.Gas)
				{
					GameTags.GasElements.Add(elem.tag);
					physicsData.mass = 1f;
					elem.maxMass = 1.8f;
				}
			}
			else
			{
				GameTags.LiquidElements.Add(elem.tag);
			}
		}
		else
		{
			GameTags.SolidElements.Add(elem.tag);
		}
		elem.defaultValues = physicsData;
	}

	private static bool SetOrCreateSubstanceForElement(Element elem, ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		bool flag = false;
		SimHashes id = elem.id;
		if (!substanceList.ContainsKey(id))
		{
			flag = true;
			Substance substance = null;
			if (substanceTable != null)
			{
				substance = substanceTable.GetSubstance(id);
			}
			if (substance == null)
			{
				substance = new Substance();
				substanceTable.GetList().Add(substance);
			}
			ElementLoader.CleanupSubstance(substance, elem);
			substance.elementID = id;
			substance.renderedByWorld = elem.IsSolid;
			substance.idx = substanceList.Count;
			if (substance.uiColour == ElementLoader.noColour)
			{
				int count = ElementLoader.elements.Count;
				int idx = substance.idx;
				substance.uiColour = Color.HSVToRGB((float)idx / (float)count, 1f, 1f);
			}
			string text = UI.StripLinkFormatting(elem.name);
			substance.name = text;
			if (Array.IndexOf<SimHashes>((SimHashes[])Enum.GetValues(typeof(SimHashes)), elem.id) >= 0)
			{
				substance.nameTag = GameTagExtensions.Create(elem.id);
			}
			else
			{
				substance.nameTag = ((text == null) ? Tag.Invalid : TagManager.Create(text));
			}
			substance.audioConfig = ElementsAudio.Instance.GetConfigForElement(id);
			substanceList.Add(id, substance);
		}
		elem.substance = substanceList[id] as Substance;
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
		for (int num = 0; num != ElementLoader.elements.Count; num++)
		{
			if (ElementLoader.elements[num].id == hash)
			{
				return num;
			}
		}
		return -1;
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

	private static SimHashes GetID(int column, int row, string[,] grid, SimHashes defaultValue = SimHashes.Vacuum)
	{
		if (column >= grid.GetLength(0) || row > grid.GetLength(1))
		{
			Output.LogError(string.Format("Could not find element at loc [{0},{1}] grid is only [{2},{3}]", new object[]
			{
				column,
				row,
				grid.GetLength(0),
				grid.GetLength(1)
			}));
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
			Output.LogError(string.Format("Could not find element {0}: {1}", text, ex.ToString()));
			return defaultValue;
		}
		return (SimHashes)obj;
	}

	private static SpawnFXHashes GetSpawnFX(int column, int row, string[,] grid)
	{
		if (column >= grid.GetLength(0) || row > grid.GetLength(1))
		{
			Output.LogError(string.Format("Could not find SpawnFXHashes at loc [{0},{1}] grid is only [{2},{3}]", new object[]
			{
				column,
				row,
				grid.GetLength(0),
				grid.GetLength(1)
			}));
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
			Output.LogError(string.Format("Could not find FX {0}: {1}", text, ex.ToString()));
			return SpawnFXHashes.None;
		}
		return (SpawnFXHashes)obj;
	}

	private static Tag CreateMaterialCategoryTag(SimHashes element_id, Tag phaseTag, string materialCategoryField)
	{
		if (!string.IsNullOrEmpty(materialCategoryField))
		{
			Tag tag = TagManager.Create(materialCategoryField);
			if (!GameTags.MaterialCategories.Contains(tag) && !GameTags.IgnoredMaterialCategories.Contains(tag))
			{
				global::Debug.LogWarningFormat("Element {0} has category {1}, but that isn't in GameTags.MaterialCategores!", new object[] { element_id, materialCategoryField });
			}
			return tag;
		}
		return phaseTag;
	}

	private static Tag[] CreateOreTags(Tag materialCategory, Tag phaseTag, string[] ore_tags_split)
	{
		List<Tag> list = new List<Tag>();
		if (ore_tags_split != null)
		{
			foreach (string text in ore_tags_split)
			{
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(TagManager.Create(text));
				}
			}
		}
		list.Add(phaseTag);
		if (materialCategory.IsValid && !list.Contains(materialCategory))
		{
			list.Add(materialCategory);
		}
		return list.ToArray();
	}

	private static void FinaliseElementsTable(ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		foreach (Element element in ElementLoader.elements)
		{
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
						ElementLoader.SetOrCreateSubstanceForElement(element, ref substanceList, substanceTable);
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
		for (int i = 0; i < ElementLoader.elements.Count; i++)
		{
			if (ElementLoader.elements[i].substance != null)
			{
				ElementLoader.elements[i].substance.idx = i;
			}
			ElementLoader.elements[i].idx = (byte)i;
		}
	}

	public static List<Element> elements;

	public static Dictionary<int, Element> elementTable;

	public static List<string> additionalJSONFiles = new List<string>();

	private static readonly Color noColour = new Color(0f, 0f, 0f, 0f);

	public class ElementEntry : Resource
	{
		public string elementId;

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

		public string lowTempTransitionTarget;

		public float lowTemp;

		public string highTempTransitionTarget;

		public float highTemp = 10000f;

		public string lowTempTransitionOreId;

		public float lowTempTransitionOreMassConversion;

		public string highTempTransitionOreId;

		public float highTempTransitionOreMassConversion;

		public string sublimateId;

		public string sublimateFx;

		public string materialCategory;

		public string[] tags;

		public bool isDisabled;

		public float strength;

		public float maxMass;

		public byte hardness;

		public float toxicity;

		public float liquidCompression;

		public float speed;

		public float minHorizontalFlow;

		public float minVerticalFlow;

		public string convertId;

		public float flow;

		public int buildMenuSort;

		[JsonConverter(typeof(StringEnumConverter))]
		public Element.State state;

		public string localizationID;
	}
}
