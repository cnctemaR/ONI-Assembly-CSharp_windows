using System;
using System.Collections.Generic;

public class GameTags
{
	public static readonly Tag Empty = TagManager.Create("Empty", null);

	public static readonly Tag Miscellaneous = TagManager.Create("Miscellaneous", null);

	public static readonly Tag Specimen = TagManager.Create("Specimen", null);

	public static readonly Tag Seed = TagManager.Create("Seed", null);

	public static readonly Tag Edible = TagManager.Create("Edible", null);

	public static readonly Tag Plant = TagManager.Create("Plant", null);

	public static readonly Tag Pickupable = TagManager.Create("Pickupable", null);

	public static readonly Tag Liquifiable = TagManager.Create("Liquifiable", null);

	public static readonly Tag IceOre = TagManager.Create("IceOre", null);

	public static readonly Tag OxyRock = TagManager.Create("OxyRock", null);

	public static readonly Tag Life = TagManager.Create("Life", null);

	public static readonly Tag Fertilizer = TagManager.Create("Fertilizer", null);

	public static readonly Tag Farmable = TagManager.Create("Farmable", null);

	public static readonly Tag Organics = TagManager.Create("Organics", null);

	public static readonly Tag Creature = TagManager.Create("Creature", null);

	public static readonly Tag Hexaped = TagManager.Create("Hexaped", null);

	public static readonly Tag HeatBulb = TagManager.Create("HeatBulb", null);

	public static readonly Tag Egg = TagManager.Create("Egg", null);

	public static readonly Tag Spawner = TagManager.Create("Spawner", null);

	public static readonly Tag Alloy = TagManager.Create("Alloy", null);

	public static readonly Tag Metal = TagManager.Create("Metal", null);

	public static readonly Tag RefinedMetal = TagManager.Create("RefinedMetal", null);

	public static readonly Tag PreciousMetal = TagManager.Create("PreciousMetal", null);

	public static readonly Tag StoredMetal = TagManager.Create("StoredMetal", null);

	public static readonly Tag Solid = TagManager.Create("Solid", null);

	public static readonly Tag Liquid = TagManager.Create("Liquid", null);

	public static readonly Tag Water = TagManager.Create("Water", null);

	public static readonly Tag Void = TagManager.Create("Void", null);

	public static readonly Tag Oxygen = TagManager.Create("Oxygen", null);

	public static readonly Tag BuildableRaw = TagManager.Create("BuildableRaw", null);

	public static readonly Tag BuildableProcessed = TagManager.Create("BuildableProcessed", null);

	public static readonly Tag Phosphorus = TagManager.Create("Phosphorus", null);

	public static readonly Tag Building = TagManager.Create("Building", null);

	public static readonly Tag Filler = TagManager.Create("Filler", null);

	public static readonly Tag Item = TagManager.Create("Item", null);

	public static readonly Tag Ore = TagManager.Create("Ore", null);

	public static readonly Tag GenericOre = TagManager.Create("GenericOre", null);

	public static readonly Tag Ingot = TagManager.Create("Ingot", null);

	public static readonly Tag Dirt = TagManager.Create("Dirt", null);

	public static readonly Tag Filter = TagManager.Create("Filter", null);

	public static readonly Tag ConsumableOre = TagManager.Create("ConsumableOre", null);

	public static readonly Tag Unstable = TagManager.Create("Unstable", null);

	public static readonly Tag EmitsLight = TagManager.Create("EmitsLight", null);

	public static readonly Tag Toxic = TagManager.Create("Toxic", null);

	public static readonly Tag Special = TagManager.Create("Special", null);

	public static readonly Tag Breathable = TagManager.Create("Breathable", null);

	public static readonly Tag Gas = TagManager.Create("Gas", null);

	public static readonly Tag Minion = TagManager.Create("Minion", null);

	public static readonly Tag Corpse = TagManager.Create("Corpse", null);

	public static readonly Tag RiverSource = TagManager.Create("RiverSource", null);

	public static readonly Tag RiverSink = TagManager.Create("RiverSink", null);

	public static readonly Tag Garbage = TagManager.Create("Garbage", null);

	public static readonly Tag MISSING_TAG = TagManager.Create("MISSING_TAG", null);

	public static readonly Tag PlantRenderer = TagManager.Create("PlantRenderer", null);

	public static readonly Tag Usable = TagManager.Create("Usable", null);

	public static readonly Tag Suit = TagManager.Create("Suit", null);

	public static readonly Tag AtmoSuit = TagManager.Create("AtmoSuit", null);

	public static readonly Tag AquaSuit = TagManager.Create("AquaSuit", null);

	public static readonly Tag TemperatureSuit = TagManager.Create("TemperatureSuit", null);

	public static readonly List<Tag> AllSuitTags = new List<Tag>(new Tag[]
	{
		GameTags.Suit,
		GameTags.AquaSuit,
		GameTags.AtmoSuit,
		GameTags.TemperatureSuit
	});

	public static readonly List<Tag> OxygenSuitTaags = new List<Tag>(new Tag[]
	{
		GameTags.AtmoSuit,
		GameTags.AquaSuit
	});

	public static readonly Tag CropSeed = TagManager.Create("CropSeed", null);

	public static readonly Tag DecorSeed = TagManager.Create("DecorSeed", null);

	public static readonly Tag MassChunk = TagManager.Create("MassChunk", null);

	public static readonly Tag UnitChunk = TagManager.Create("UnitChunk", null);

	public static readonly Tag MinionSelectPreview = TagManager.Create("MinionSelectPreview", null);
}
