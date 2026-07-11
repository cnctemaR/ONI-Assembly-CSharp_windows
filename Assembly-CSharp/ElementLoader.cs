using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Klei;
using ProcGenGame;
using STRINGS;
using UnityEngine;

public class ElementLoader
{
	public static List<ElementLoader.ElementEntry> CollectElementsFromYAML()
	{
		List<ElementLoader.ElementEntry> list = new List<ElementLoader.ElementEntry>();
		ListPool<FileHandle, ElementLoader>.PooledList pooledList = ListPool<FileHandle, ElementLoader>.Allocate();
		FileSystem.GetFiles(FileSystem.Normalize(ElementLoader.path), "*.yaml", pooledList);
		ListPool<YamlIO.Error, ElementLoader>.PooledList errors = ListPool<YamlIO.Error, ElementLoader>.Allocate();
		using (List<FileHandle>.Enumerator enumerator = pooledList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				FileHandle file = enumerator.Current;
				ElementLoader.ElementEntryCollection elementEntryCollection = YamlIO.LoadFile<ElementLoader.ElementEntryCollection>(file.full_path, delegate(YamlIO.Error error, bool force_log_as_warning)
				{
					error.file = file;
					errors.Add(error);
				}, null);
				if (elementEntryCollection != null)
				{
					list.AddRange(elementEntryCollection.elements);
				}
			}
		}
		pooledList.Recycle();
		if (Global.Instance != null && Global.Instance.modManager != null)
		{
			Global.Instance.modManager.HandleErrors(errors);
		}
		errors.Recycle();
		return list;
	}

	public static void Load(ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		ElementLoader.elements = new List<Element>();
		ElementLoader.elementTable = new Dictionary<int, Element>();
		List<ElementLoader.ElementEntry> list = ElementLoader.CollectElementsFromYAML();
		foreach (ElementLoader.ElementEntry elementEntry in list)
		{
			int num = Hash.SDBMLower(elementEntry.elementId);
			if (!ElementLoader.elementTable.ContainsKey(num))
			{
				Element element = new Element();
				element.id = (SimHashes)num;
				element.name = Strings.Get(elementEntry.localizationID);
				element.nameUpperCase = element.name.ToUpper();
				element.description = Strings.Get(elementEntry.description);
				element.tag = TagManager.Create(elementEntry.elementId, element.name);
				ElementLoader.CopyEntryToElement(elementEntry, element);
				ElementLoader.elements.Add(element);
				ElementLoader.elementTable[num] = element;
			}
		}
		foreach (Element element2 in ElementLoader.elements)
		{
			if (!ElementLoader.ManifestSubstanceForElement(element2, ref substanceList, substanceTable))
			{
				global::Debug.LogWarning("Missing substance for element: " + element2.id.ToString());
			}
		}
		ElementLoader.FinaliseElementsTable(ref substanceList, substanceTable);
		WorldGen.SetupDefaultElements();
	}

	private static void CopyEntryToElement(ElementLoader.ElementEntry entry, Element elem)
	{
		int num = Hash.SDBMLower(entry.elementId);
		elem.tag = TagManager.Create(entry.elementId.ToString());
		elem.specificHeatCapacity = entry.specificHeatCapacity;
		elem.thermalConductivity = entry.thermalConductivity;
		elem.molarMass = entry.molarMass;
		elem.strength = entry.strength;
		elem.disabled = entry.isDisabled;
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

	private static bool ManifestSubstanceForElement(Element elem, ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		elem.substance = null;
		if (substanceList.ContainsKey(elem.id))
		{
			elem.substance = substanceList[elem.id] as Substance;
			return false;
		}
		if (substanceTable != null)
		{
			elem.substance = substanceTable.GetSubstance(elem.id);
		}
		if (elem.substance == null)
		{
			elem.substance = new Substance();
			substanceTable.GetList().Add(elem.substance);
		}
		elem.substance.elementID = elem.id;
		elem.substance.renderedByWorld = elem.IsSolid;
		elem.substance.idx = substanceList.Count;
		if (elem.substance.uiColour == ElementLoader.noColour)
		{
			int count = ElementLoader.elements.Count;
			int idx = elem.substance.idx;
			elem.substance.uiColour = Color.HSVToRGB((float)idx / (float)count, 1f, 1f);
		}
		string text = UI.StripLinkFormatting(elem.name);
		elem.substance.name = text;
		if (Array.IndexOf<SimHashes>((SimHashes[])Enum.GetValues(typeof(SimHashes)), elem.id) >= 0)
		{
			elem.substance.nameTag = GameTagExtensions.Create(elem.id);
		}
		else
		{
			elem.substance.nameTag = ((text == null) ? Tag.Invalid : TagManager.Create(text));
		}
		elem.substance.audioConfig = ElementsAudio.Instance.GetConfigForElement(elem.id);
		substanceList.Add(elem.id, elem.substance);
		return true;
	}

	public static Element FindElementByName(string name)
	{
		Element element;
		try
		{
			element = ElementLoader.FindElementByHash((SimHashes)Enum.Parse(typeof(SimHashes), name));
		}
		catch
		{
			element = ElementLoader.FindElementByHash((SimHashes)Hash.SDBMLower(name));
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
			global::Debug.LogError(string.Format("Could not find element at loc [{0},{1}] grid is only [{2},{3}]", new object[]
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
			global::Debug.LogError(string.Format("Could not find element {0}: {1}", text, ex.ToString()));
			return defaultValue;
		}
		return (SimHashes)obj;
	}

	private static SpawnFXHashes GetSpawnFX(int column, int row, string[,] grid)
	{
		if (column >= grid.GetLength(0) || row > grid.GetLength(1))
		{
			global::Debug.LogError(string.Format("Could not find SpawnFXHashes at loc [{0},{1}] grid is only [{2},{3}]", new object[]
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
			global::Debug.LogError(string.Format("Could not find FX {0}: {1}", text, ex.ToString()));
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
						ElementLoader.ManifestSubstanceForElement(element, ref substanceList, substanceTable);
					}
				}
				global::Debug.Assert(element.substance.nameTag.IsValid);
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

	private static string path = Application.streamingAssetsPath + "/elements/";

	private static readonly Color noColour = new Color(0f, 0f, 0f, 0f);

	public class ElementEntryCollection
	{
		public ElementLoader.ElementEntry[] elements { get; set; }
	}

	public class ElementEntry
	{
		public ElementEntry()
		{
			this.lowTemp = 0f;
			this.highTemp = 10000f;
		}

		public string elementId { get; set; }

		public float specificHeatCapacity { get; set; }

		public float thermalConductivity { get; set; }

		public float solidSurfaceAreaMultiplier { get; set; }

		public float liquidSurfaceAreaMultiplier { get; set; }

		public float gasSurfaceAreaMultiplier { get; set; }

		public float defaultMass { get; set; }

		public float defaultTemperature { get; set; }

		public float defaultPressure { get; set; }

		public float molarMass { get; set; }

		public float lightAbsorptionFactor { get; set; }

		public string lowTempTransitionTarget { get; set; }

		public float lowTemp { get; set; }

		public string highTempTransitionTarget { get; set; }

		public float highTemp { get; set; }

		public string lowTempTransitionOreId { get; set; }

		public float lowTempTransitionOreMassConversion { get; set; }

		public string highTempTransitionOreId { get; set; }

		public float highTempTransitionOreMassConversion { get; set; }

		public string sublimateId { get; set; }

		public string sublimateFx { get; set; }

		public string materialCategory { get; set; }

		public string[] tags { get; set; }

		public bool isDisabled { get; set; }

		public float strength { get; set; }

		public float maxMass { get; set; }

		public byte hardness { get; set; }

		public float toxicity { get; set; }

		public float liquidCompression { get; set; }

		public float speed { get; set; }

		public float minHorizontalFlow { get; set; }

		public float minVerticalFlow { get; set; }

		public string convertId { get; set; }

		public float flow { get; set; }

		public int buildMenuSort { get; set; }

		public Element.State state { get; set; }

		public string localizationID { get; set; }

		public string description
		{
			get
			{
				return this.description_backing ?? ("STRINGS.ELEMENTS." + this.elementId.ToString().ToUpper() + ".DESC");
			}
			set
			{
				this.description_backing = value;
			}
		}

		private string description_backing;
	}
}
