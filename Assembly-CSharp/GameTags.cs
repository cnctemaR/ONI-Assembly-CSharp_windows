using System;
using System.Collections.Generic;

public class GameTags
{
	public static readonly Tag DeprecatedContent = TagManager.Create("DeprecatedContent", null);

	public static readonly Tag Any = TagManager.Create("Any", null);

	public static readonly Tag Miscellaneous = TagManager.Create("Miscellaneous", null);

	public static readonly Tag Specimen = TagManager.Create("Specimen", null);

	public static readonly Tag Seed = TagManager.Create("Seed", null);

	public static readonly Tag Edible = TagManager.Create("Edible", null);

	public static readonly Tag CookingIngredient = TagManager.Create("CookingIngredient", null);

	public static readonly Tag Medicine = TagManager.Create("Medicine", null);

	public static readonly Tag Plant = TagManager.Create("Plant", null);

	public static readonly Tag Pickupable = TagManager.Create("Pickupable", null);

	public static readonly Tag Liquifiable = TagManager.Create("Liquifiable", null);

	public static readonly Tag IceOre = TagManager.Create("IceOre", null);

	public static readonly Tag OxyRock = TagManager.Create("OxyRock", null);

	public static readonly Tag Life = TagManager.Create("Life", null);

	public static readonly Tag Fertilizer = TagManager.Create("Fertilizer", null);

	public static readonly Tag Farmable = TagManager.Create("Farmable", null);

	public static readonly Tag Agriculture = TagManager.Create("Agriculture", null);

	public static readonly Tag Organics = TagManager.Create("Organics", null);

	public static readonly Tag IndustrialProduct = TagManager.Create("IndustrialProduct", null);

	public static readonly Tag IndustrialIngredient = TagManager.Create("IndustrialIngredient", null);

	public static readonly Tag Other = TagManager.Create("Other", null);

	public static readonly Tag Plastic = TagManager.Create("Plastic", null);

	public static readonly Tag BuildableAny = TagManager.Create("BuildableAny", null);

	public static readonly Tag Decoration = TagManager.Create("Decoration", null);

	public static readonly Tag Incapacitated = TagManager.Create("Incapacitated", null);

	public static readonly Tag CaloriesDepleted = TagManager.Create("CaloriesDepleted", null);

	public static readonly Tag HitPointsDepleted = TagManager.Create("HitPointsDepleted", null);

	public static readonly Tag Wilting = TagManager.Create("Wilting", null);

	public static readonly Tag Creature = TagManager.Create("Creature", null);

	public static readonly Tag Hexaped = TagManager.Create("Hexaped", null);

	public static readonly Tag HeatBulb = TagManager.Create("HeatBulb", null);

	public static readonly Tag Egg = TagManager.Create("Egg", null);

	public static readonly Tag Trapped = TagManager.Create("Trapped", null);

	public static readonly Tag BagableCreature = TagManager.Create("BagableCreature", null);

	public static readonly Tag Spawner = TagManager.Create("Spawner", null);

	public static readonly Tag FullyIncubated = TagManager.Create("FullyIncubated", null);

	public static readonly Tag Alloy = TagManager.Create("Alloy", null);

	public static readonly Tag Metal = TagManager.Create("Metal", null);

	public static readonly Tag RefinedMetal = TagManager.Create("RefinedMetal", null);

	public static readonly Tag PreciousMetal = TagManager.Create("PreciousMetal", null);

	public static readonly Tag StoredMetal = TagManager.Create("StoredMetal", null);

	public static readonly Tag Solid = TagManager.Create("Solid", null);

	public static readonly Tag Liquid = TagManager.Create("Liquid", null);

	public static readonly Tag LiquidSource = TagManager.Create("LiquidSource", null);

	public static readonly Tag Water = TagManager.Create("Water", null);

	public static readonly Tag DirtyWater = TagManager.Create("DirtyWater", null);

	public static readonly Tag AnyWater = TagManager.Create("AnyWater", null);

	public static readonly Tag Algae = TagManager.Create("Algae", null);

	public static readonly Tag Void = TagManager.Create("Void", null);

	public static readonly Tag Oxygen = TagManager.Create("Oxygen", null);

	public static readonly Tag Hydrogen = TagManager.Create("Hydrogen", null);

	public static readonly Tag Methane = TagManager.Create("Methane", null);

	public static readonly Tag CarbonDioxide = TagManager.Create("CarbonDioxide", null);

	public static readonly Tag Carbon = TagManager.Create("Carbon", null);

	public static readonly Tag BuildableRaw = TagManager.Create("BuildableRaw", null);

	public static readonly Tag BuildableProcessed = TagManager.Create("BuildableProcessed", null);

	public static readonly Tag Phosphorus = TagManager.Create("Phosphorus", null);

	public static readonly Tag Phosphorite = TagManager.Create("Phosphorite", null);

	public static readonly Tag SlimeMold = TagManager.Create("SlimeMold", null);

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

	public static readonly Tag Unbreathable = TagManager.Create("Unbreathable", null);

	public static readonly Tag Gas = TagManager.Create("Gas", null);

	public static readonly Tag Crushable = TagManager.Create("Crushable", null);

	public static readonly Tag IronOre = TagManager.Create("IronOre", null);

	public static readonly Tag Minion = TagManager.Create("Minion", null);

	public static readonly Tag Corpse = TagManager.Create("Corpse", null);

	public static readonly Tag RiverSource = TagManager.Create("RiverSource", null);

	public static readonly Tag RiverSink = TagManager.Create("RiverSink", null);

	public static readonly Tag Garbage = TagManager.Create("Garbage", null);

	public static readonly Tag OilWell = TagManager.Create("OilWell", null);

	public static readonly Tag MISSING_TAG = TagManager.Create("MISSING_TAG", null);

	public static readonly Tag PlantRenderer = TagManager.Create("PlantRenderer", null);

	public static readonly Tag Usable = TagManager.Create("Usable", null);

	public static readonly Tag Suit = TagManager.Create("Suit", null);

	public static readonly Tag AtmoSuit = TagManager.Create("Atmo_Suit", null);

	public static readonly Tag AquaSuit = TagManager.Create("Aqua_Suit", null);

	public static readonly Tag TemperatureSuit = TagManager.Create("Temperature_Suit", null);

	public static readonly List<Tag> AllSuitTags = new List<Tag>
	{
		GameTags.Suit,
		GameTags.AquaSuit,
		GameTags.AtmoSuit,
		GameTags.TemperatureSuit
	};

	public static readonly List<Tag> OxygenSuitTags = new List<Tag>
	{
		GameTags.AtmoSuit,
		GameTags.AquaSuit
	};

	public static readonly Tag Clothes = TagManager.Create("Clothes", null);

	public static readonly Tag WarmVest = TagManager.Create("Warm_Vest", null);

	public static readonly Tag CoolVest = TagManager.Create("Cool_Vest", null);

	public static readonly Tag FunkyVest = TagManager.Create("Funky_Vest", null);

	public static readonly List<Tag> AllClothesTags = new List<Tag>
	{
		GameTags.Clothes,
		GameTags.WarmVest,
		GameTags.CoolVest,
		GameTags.FunkyVest
	};

	public static readonly Tag Assigned = TagManager.Create("Assigned", null);

	public static readonly Tag Helmet = TagManager.Create("Helmet", null);

	public static readonly Tag Equipped = TagManager.Create("Equipped", null);

	public static readonly Tag Entombed = TagManager.Create("Entombed", null);

	public static readonly Tag Preserved = TagManager.Create("Preserved", null);

	public static readonly Tag Compostable = TagManager.Create("Compostable", null);

	public static readonly Tag MarkedForCompost = TagManager.Create("MarkedForCompost", null);

	public static readonly Tag Pickled = TagManager.Create("Pickled", null);

	public static readonly Tag Dying = TagManager.Create("Dying", null);

	public static readonly Tag Dead = TagManager.Create("Dead", null);

	public static readonly Tag Reachable = TagManager.Create("Reachable", null);

	public static readonly Tag PreventChoreInterruption = TagManager.Create("PreventChoreInterruption", null);

	public static readonly Tag RecoveringBreath = TagManager.Create("RecoveringBreath", null);

	public static readonly Tag NoOxygen = TagManager.Create("NoOxygen", null);

	public static readonly Tag Idle = TagManager.Create("Idle", null);

	public static readonly Tag HasDebugDestination = TagManager.Create("HasDebugDestination", null);

	public static readonly Tag DupeBrain = TagManager.Create("DupeBrain", null);

	public static readonly Tag CreatureBrain = TagManager.Create("CreatureBrain", null);

	public static readonly Tag Operational = TagManager.Create("Operational", null);

	public static readonly Tag Stored = TagManager.Create("Stored", null);

	public static readonly Tag StoredPrivate = TagManager.Create("StoredPrivate", null);

	public static readonly Tag Sealed = TagManager.Create("Sealed", null);

	public static readonly Tag CropSeed = TagManager.Create("CropSeed", null);

	public static readonly Tag DecorSeed = TagManager.Create("DecorSeed", null);

	public static readonly Tag WaterSeed = TagManager.Create("WaterSeed", null);

	public static readonly Tag Harvestable = TagManager.Create("Harvestable", null);

	public static readonly Tag Hanging = TagManager.Create("Hanging", null);

	public static readonly Tag MassChunk = TagManager.Create("MassChunk", null);

	public static readonly Tag UnitChunk = TagManager.Create("UnitChunk", null);

	public static readonly Tag MinionSelectPreview = TagManager.Create("MinionSelectPreview", null);

	public static readonly Tag Empty = TagManager.Create("Empty", null);

	public static TagSet SolidElements = new TagSet();

	public static TagSet LiquidElements = new TagSet();

	public static TagSet GasElements = new TagSet();

	public static TagSet CalorieCategories = new TagSet
	{
		GameTags.Edible,
		GameTags.MarkedForCompost
	};

	public static TagSet UnitCategories = new TagSet
	{
		GameTags.CookingIngredient,
		GameTags.Medicine,
		GameTags.Seed,
		GameTags.Clothes,
		GameTags.IndustrialIngredient
	};

	public static TagSet IgnoredMaterialCategories = new TagSet
	{
		GameTags.Special,
		GameTags.Breathable,
		GameTags.Unbreathable
	};

	public static TagSet MaterialCategories = new TagSet
	{
		GameTags.Alloy,
		GameTags.Metal,
		GameTags.RefinedMetal,
		GameTags.BuildableRaw,
		GameTags.BuildableProcessed,
		GameTags.Filter,
		GameTags.Liquifiable,
		GameTags.Liquid,
		GameTags.ConsumableOre,
		GameTags.Organics,
		GameTags.Farmable,
		GameTags.Agriculture,
		GameTags.Other,
		GameTags.Plastic
	};

	public static TagSet OtherEntityTags = new TagSet { GameTags.BagableCreature };

	public static TagSet AllCategories = new TagSet(new TagSet[]
	{
		GameTags.CalorieCategories,
		GameTags.UnitCategories,
		GameTags.MaterialCategories,
		GameTags.OtherEntityTags
	});

	public static TagSet DisplayAsCalories = new TagSet(GameTags.CalorieCategories);

	public static TagSet DisplayAsUnits = new TagSet(GameTags.UnitCategories);

	public abstract class ChoreTypes
	{
		public static readonly Tag Farming = TagManager.Create("Farming", null);

		public static readonly Tag Ranching = TagManager.Create("Ranching", null);

		public static readonly Tag Research = TagManager.Create("Research", null);

		public static readonly Tag Power = TagManager.Create("Power", null);

		public static readonly Tag Building = TagManager.Create("Building", null);

		public static readonly Tag Cooking = TagManager.Create("Cooking", null);

		public static readonly Tag Fabricating = TagManager.Create("Fabricating", null);

		public static readonly Tag Wiring = TagManager.Create("Wiring", null);

		public static readonly Tag Art = TagManager.Create("Art", null);

		public static readonly Tag Digging = TagManager.Create("Digging", null);

		public static readonly Tag Doctoring = TagManager.Create("Doctoring", null);

		public static readonly Tag Conveyor = TagManager.Create("Conveyor", null);

		public static readonly Tag[] FabricateChores = new Tag[] { GameTags.ChoreTypes.Fabricating };

		public static readonly Tag[] FarmingChores = new Tag[] { GameTags.ChoreTypes.Farming };

		public static readonly Tag[] RanchingChores = new Tag[] { GameTags.ChoreTypes.Ranching };

		public static readonly Tag[] CookingChores = new Tag[] { GameTags.ChoreTypes.Cooking };

		public static readonly Tag[] PowerChores = new Tag[] { GameTags.ChoreTypes.Power };

		public static readonly Tag[] WiringChores = new Tag[] { GameTags.ChoreTypes.Wiring };

		public static readonly Tag[] BuildingChores = new Tag[] { GameTags.ChoreTypes.Building };

		public static readonly Tag[] ResearchChores = new Tag[] { GameTags.ChoreTypes.Research };

		public static readonly Tag[] ArtChores = new Tag[] { GameTags.ChoreTypes.Art };

		public static readonly Tag[] DigChores = new Tag[] { GameTags.ChoreTypes.Digging };

		public static readonly Tag[] DoctoringChores = new Tag[] { GameTags.ChoreTypes.Doctoring };

		public static readonly Tag[] ConveyorChores = new Tag[] { GameTags.ChoreTypes.Conveyor };
	}

	public static class Creatures
	{
		public static readonly Tag Stunned = TagManager.Create("Stunned", null);

		public static readonly Tag Falling = TagManager.Create("Falling", null);

		public static readonly Tag WantsToEnterBurrow = TagManager.Create("WantsToBurrow", null);

		public static readonly Tag Burrowed = TagManager.Create("Burrowed", null);

		public static readonly Tag WantsToExitBurrow = TagManager.Create("WantsToExitBurrow", null);

		public static readonly Tag WantsToEat = TagManager.Create("WantsToEat", null);

		public static readonly Tag WantsToGetRanched = TagManager.Create("WantsToGetRanched", null);

		public static readonly Tag Flee = TagManager.Create("Flee", null);

		public static readonly Tag Attack = TagManager.Create("Attack", null);

		public static readonly Tag Die = TagManager.Create("Die", null);

		public static readonly Tag Poop = TagManager.Create("Poop", null);

		public static readonly Tag MoveToLure = TagManager.Create("MoveToLure", null);

		public static readonly Tag Drowning = TagManager.Create("Drowning", null);

		public static readonly Tag Hungry = TagManager.Create("Hungry", null);

		public static readonly Tag Flying = TagManager.Create("Flying", null);

		public static readonly Tag Fertile = TagManager.Create("Fertile", null);

		public static readonly Tag Submerged = TagManager.Create("Submerged", null);

		public static readonly Tag ExitSubmerged = TagManager.Create("ExitSubmerged", null);

		public static readonly Tag WantsToDropElements = TagManager.Create("WantsToDropElements", null);

		public static readonly Tag OriginallyWild = TagManager.Create("Wild", null);

		public static readonly Tag Wild = TagManager.Create("Wild", null);
	}
}
