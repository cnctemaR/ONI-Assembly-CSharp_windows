using System;

namespace ProcGen
{
	public class WorldGenTags
	{
		public static readonly Tag ConnectToSiblings = TagManager.Create("ConnectToSiblings", null);

		public static readonly Tag ConnectTypeMinSpan = TagManager.Create("ConnectTypeMinSpan", null);

		public static readonly Tag ConnectTypeSpan = TagManager.Create("ConnectTypeSpan", null);

		public static readonly Tag ConnectTypeNone = TagManager.Create("ConnectTypeNone", null);

		public static readonly Tag ConnectTypeFull = TagManager.Create("ConnectTypeFull", null);

		public static readonly Tag ConnectTypeRandom = TagManager.Create("ConnectTypeRandom", null);

		public static readonly Tag Cell = TagManager.Create("Cell", null);

		public static readonly Tag Edge = TagManager.Create("Edge", null);

		public static readonly Tag Corner = TagManager.Create("Corner", null);

		public static readonly Tag EdgeUnpassable = TagManager.Create("EdgeUnpassable", null);

		public static readonly Tag EdgeClosed = TagManager.Create("EdgeClosed", null);

		public static readonly Tag EdgeOpen = TagManager.Create("EdgeOpen", null);

		public static readonly Tag IgnoreCaveOverride = TagManager.Create("IgnoreCaveOverride", null);

		public static readonly Tag ErodePointToCentroid = TagManager.Create("ErodePointToCentroid", null);

		public static readonly Tag ErodePointToCentroidInv = TagManager.Create("ErodePointToCentroidInv", null);

		public static readonly Tag ErodePointToEdge = TagManager.Create("ErodePointToEdge", null);

		public static readonly Tag ErodePointToEdgeInv = TagManager.Create("ErodePointToEdgeInv", null);

		public static readonly Tag DistFunctionPointCentroid = TagManager.Create("DistFunctionPointCentroid", null);

		public static readonly Tag DistFunctionPointEdge = TagManager.Create("DistFunctionPointEdge", null);

		public static readonly Tag SplitOnParentDensity = TagManager.Create("SplitOnParentDensity", null);

		public static readonly Tag SplitTwice = TagManager.Create("SplitTwice", null);

		public static readonly Tag UltraHighDensitySplit = TagManager.Create("UltraHighDensitySplit", null);

		public static readonly Tag VeryHighDensitySplit = TagManager.Create("VeryHighDensitySplit", null);

		public static readonly Tag HighDensitySplit = TagManager.Create("HighDensitySplit", null);

		public static readonly Tag MediumDensitySplit = TagManager.Create("MediumDensitySplit", null);

		public static readonly Tag UnassignedNode = TagManager.Create("UnassignedNode", null);

		public static readonly Tag Feature = TagManager.Create("Feature", null);

		public static readonly Tag CenteralFeature = TagManager.Create("CenteralFeature", null);

		public static readonly Tag Overworld = TagManager.Create("Overworld", null);

		public static readonly Tag StartNear = TagManager.Create("StartNear", null);

		public static readonly Tag StartMedium = TagManager.Create("StartMedium", null);

		public static readonly Tag StartFar = TagManager.Create("StartFar", null);

		public static readonly Tag NearEdge = TagManager.Create("NearEdge", null);

		public static readonly Tag NearSurface = TagManager.Create("NearSurface", null);

		public static readonly Tag NearDepths = TagManager.Create("NearDepths", null);

		public static readonly Tag AtSurface = TagManager.Create("AtSurface", null);

		public static readonly Tag AtDepths = TagManager.Create("AtDepths", null);

		public static readonly Tag AtEdge = TagManager.Create("AtEdge", null);

		public static readonly Tag EdgeOfVoid = TagManager.Create("EdgeOfVoid", null);

		public static readonly Tag Dry = TagManager.Create("Dry", null);

		public static readonly Tag Wet = TagManager.Create("Wet", null);

		public static readonly Tag River = TagManager.Create("River", null);

		public static readonly Tag StartWorld = TagManager.Create("StartWorld", null);

		public static readonly Tag StartLocation = TagManager.Create("StartLocation", null);

		public static readonly Tag NearStartLocation = TagManager.Create("NearStartLocation", null);

		public static readonly Tag POI = TagManager.Create("POI", null);

		public static readonly Tag RoomBorderNone = TagManager.Create("RoomBorderNone", null);

		public static readonly Tag RoomBorderMixed = TagManager.Create("BorderMixed", null);

		public static readonly Tag RoomBorderRandom = TagManager.Create("BorderRandom", null);

		public static readonly Tag AllowExceedNodeBorders = TagManager.Create("AllowExceedNodeBorders", null);

		public static readonly Tag CaveVoidSliver = TagManager.Create("CaveVoidSliver", null);

		public static readonly Tag Geode = TagManager.Create("Geode", null);

		public static readonly Tag TheVoid = TagManager.Create("TheVoid", null);

		public static readonly Tag SprinkleOfMetal = TagManager.Create("SprinkleOfMetal", null);

		public static readonly Tag SprinkleOfOxyRock = TagManager.Create("SprinkleOfOxyRock", null);

		public static readonly Tag Infected = TagManager.Create("Infected", null);

		public static readonly Tag InfectedDweebcephaly = TagManager.Create("Infected:Dweebcephaly", null);

		public static readonly Tag InfectedLazibonitis = TagManager.Create("Infected:Lazibonitis", null);

		public static readonly Tag InfectedDiarrhea = TagManager.Create("Infected:Diarrhea", null);

		public static readonly Tag InfectedFoodPoisoning = TagManager.Create("Infected:FoodPoisoning", null);

		public static readonly Tag InfectedPutridOdour = TagManager.Create("Infected:PutridOdour", null);

		public static readonly Tag InfectedSpores = TagManager.Create("Infected:Spores", null);

		public static readonly Tag InfectedColdBrain = TagManager.Create("Infected:ColdBrain", null);

		public static readonly Tag InfectedHeatRash = TagManager.Create("Infected:HeatRash", null);

		public static readonly Tag InfectedSlimeLung = TagManager.Create("Infected:SlimeLung", null);

		public static readonly Tag DEBUG_Split = TagManager.Create("DEBUG_Split", null);

		public static readonly Tag DEBUG_SplitForChildCount = TagManager.Create("DEBUG_SplitForChildCount", null);

		public static readonly Tag DEBUG_SplitTopSite = TagManager.Create("DEBUG_SplitTopSite", null);

		public static readonly Tag DEBUG_SplitBottomSite = TagManager.Create("DEBUG_SplitBottomSite", null);

		public static readonly Tag DEBUG_SplitLargeStartingSites = TagManager.Create("DEBUG_SplitLargeStartingSites", null);

		public static readonly Tag DEBUG_NoSplitForChildCount = TagManager.Create("DEBUG_NoSplitForChildCount", null);

		public static readonly TagSet DebugTags = new TagSet(new Tag[]
		{
			WorldGenTags.DEBUG_Split,
			WorldGenTags.DEBUG_SplitForChildCount,
			WorldGenTags.DEBUG_SplitTopSite,
			WorldGenTags.DEBUG_SplitBottomSite,
			WorldGenTags.DEBUG_SplitLargeStartingSites,
			WorldGenTags.DEBUG_NoSplitForChildCount
		});

		public static readonly TagSet MapTags = new TagSet(new Tag[]
		{
			WorldGenTags.Cell,
			WorldGenTags.Edge,
			WorldGenTags.Corner,
			WorldGenTags.EdgeUnpassable,
			WorldGenTags.EdgeClosed,
			WorldGenTags.EdgeOpen
		});

		public static readonly TagSet CommandTags = new TagSet(new Tag[]
		{
			WorldGenTags.IgnoreCaveOverride,
			WorldGenTags.ErodePointToCentroid,
			WorldGenTags.ErodePointToCentroidInv,
			WorldGenTags.DistFunctionPointCentroid,
			WorldGenTags.DistFunctionPointEdge,
			WorldGenTags.SplitOnParentDensity,
			WorldGenTags.SplitTwice,
			WorldGenTags.UltraHighDensitySplit,
			WorldGenTags.VeryHighDensitySplit,
			WorldGenTags.HighDensitySplit,
			WorldGenTags.MediumDensitySplit
		});

		public static readonly TagSet WorldTags = new TagSet(new Tag[]
		{
			WorldGenTags.UnassignedNode,
			WorldGenTags.Feature,
			WorldGenTags.CenteralFeature,
			WorldGenTags.Overworld,
			WorldGenTags.NearSurface,
			WorldGenTags.NearDepths,
			WorldGenTags.AtSurface,
			WorldGenTags.AtDepths,
			WorldGenTags.AtEdge,
			WorldGenTags.StartNear,
			WorldGenTags.StartMedium
		});

		public static readonly TagSet DistanceTags = new TagSet(new Tag[]
		{
			WorldGenTags.NearEdge,
			WorldGenTags.NearSurface,
			WorldGenTags.NearDepths,
			WorldGenTags.AtSurface,
			WorldGenTags.AtDepths,
			WorldGenTags.AtEdge,
			WorldGenTags.StartWorld
		});
	}
}
