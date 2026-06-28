using System;

namespace Klei
{
	public class WorldGenTags
	{
		public static readonly Tag UnassignedNode = TagManager.Create("UnassignedNode", null);

		public static readonly Tag Feature = TagManager.Create("Feature", null);

		public static readonly Tag CenteralFeature = TagManager.Create("CenteralFeature", null);

		public static readonly Tag Overworld = TagManager.Create("Overworld", null);

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

		public static readonly Tag NearSurface = TagManager.Create("NearSurface", null);

		public static readonly Tag NearDepths = TagManager.Create("NearDepths", null);

		public static readonly Tag AtSurface = TagManager.Create("AtSurface", null);

		public static readonly Tag AtDepths = TagManager.Create("AtDepths", null);

		public static readonly Tag IgnoreCaveOverride = TagManager.Create("IgnoreCaveOverride", null);

		public static readonly Tag CaveVoidSliver = TagManager.Create("CaveVoidSliver", null);

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

		public static readonly Tag StartNear = TagManager.Create("StartNear", null);

		public static readonly Tag StartMedium = TagManager.Create("StartMedium", null);

		public static readonly Tag StartFar = TagManager.Create("StartFar", null);

		public static readonly Tag Geode = TagManager.Create("Geode", null);

		public static readonly Tag River = TagManager.Create("River", null);

		public static readonly Tag TheVoid = TagManager.Create("TheVoid", null);

		public static readonly Tag StartWorld = TagManager.Create("StartWorld", null);

		public static readonly Tag StartLocation = TagManager.Create("StartLocation", null);

		public static readonly Tag FakeStart = TagManager.Create("FakeStart", null);

		public static readonly Tag NearStartLocation = TagManager.Create("NearStartLocation", null);

		public static readonly Tag SprinkleOfMetal = TagManager.Create("SprinkleOfMetal", null);

		public static readonly Tag SprinkleOfOxyRock = TagManager.Create("SprinkleOfOxyRock", null);

		public static readonly Tag OxySpace = TagManager.Create("OxySpace", null);

		public static readonly Tag Hive = TagManager.Create("Hive", null);

		public static readonly Tag Dry = TagManager.Create("Dry", null);

		public static readonly Tag Wet = TagManager.Create("Wet", null);

		public static readonly Tag RoomBorderNone = TagManager.Create("RoomBorderNone", null);

		public static readonly Tag RoomBorderMixed = TagManager.Create("BorderMixed", null);

		public static readonly Tag RoomBorderRandom = TagManager.Create("BorderRandom", null);

		public static readonly Tag AllowExceedNodeBorders = TagManager.Create("AllowExceedNodeBorders", null);

		public static readonly Tag DEBUG_Split = TagManager.Create("DEBUG_Split", null);

		public static readonly Tag DEBUG_SplitForChildCount = TagManager.Create("DEBUG_SplitForChildCount", null);

		public static readonly Tag DEBUG_SplitTopSite = TagManager.Create("DEBUG_SplitTopSite", null);

		public static readonly Tag DEBUG_SplitBottomSite = TagManager.Create("DEBUG_SplitBottomSite", null);

		public static readonly Tag DEBUG_SplitLargeStartingSites = TagManager.Create("DEBUG_SplitLargeStartingSites", null);

		public static readonly Tag DEBUG_NoSplitForChildCount = TagManager.Create("DEBUG_NoSplitForChildCount", null);
	}
}
