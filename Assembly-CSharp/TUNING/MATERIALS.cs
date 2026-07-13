using System;
using System.Linq;
using STRINGS;

namespace TUNING
{
	public class MATERIALS
	{
		public static string GetMaterialString(string materialCategory)
		{
			string[] array = materialCategory.Split('&', StringSplitOptions.None);
			string text;
			if (array.Length == 1)
			{
				text = UI.FormatAsLink(Strings.Get("STRINGS.MISC.TAGS." + materialCategory.ToUpper()), materialCategory);
			}
			else
			{
				text = string.Join(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.PREPARED_SEPARATOR, array.Select<string, string>((string s) => UI.FormatAsLink(Strings.Get("STRINGS.MISC.TAGS." + s.ToUpper()), s)));
			}
			return text;
		}

		public const string METAL = "Metal";

		public const string REFINED_METAL = "RefinedMetal";

		public const string GLASS = "Glass";

		public const string TRANSPARENT = "Transparent";

		public const string PLASTIC = "Plastic";

		public const string BUILDABLERAW = "BuildableRaw";

		public const string PRECIOUSROCK = "PreciousRock";

		public const string WOOD = "BuildingWood";

		public const string BUILDINGFIBER = "BuildingFiber";

		public const string LEAD = "Lead";

		public const string INSULATOR = "Insulator";

		public const string FOSSILS_TAG = "Fossils";

		public static readonly string[] ALL_METALS = new string[] { "Metal" };

		public static readonly string[] RAW_METALS = new string[] { "Metal" };

		public static readonly string[] REFINED_METALS = new string[] { "RefinedMetal" };

		public static readonly string[] ALLOYS = new string[] { "Alloy" };

		public static readonly string[] ALL_MINERALS = new string[] { "BuildableRaw" };

		public static readonly string[] RAW_MINERALS = new string[] { "BuildableRaw" };

		public static readonly string[] RAW_MINERALS_OR_METALS = new string[] { "BuildableRaw&Metal" };

		public static readonly string[] RAW_MINERALS_OR_WOOD = new string[] { "BuildableRaw&" + GameTags.BuildingWood.ToString() };

		public static readonly string[] WOODS = new string[] { "BuildingWood" };

		public static readonly string[] FOSSILS = new string[] { "Fossils" };

		public static readonly string[] REFINED_MINERALS = new string[] { "BuildableProcessed" };

		public static readonly string[] PRECIOUS_ROCKS = new string[] { "PreciousRock" };

		public static readonly string[] FARMABLE = new string[] { "Farmable" };

		public static readonly string[] EXTRUDABLE = new string[] { "Extrudable" };

		public static readonly string[] PLUMBABLE = new string[] { "Plumbable" };

		public static readonly string[] PLUMBABLE_OR_METALS = new string[] { "Plumbable&Metal" };

		public static readonly string[] PLASTICS = new string[] { "Plastic" };

		public static readonly string[] GLASSES = new string[] { "Glass" };

		public static readonly string[] TRANSPARENTS = new string[] { "Transparent" };

		public static readonly string[] BUILDING_FIBER = new string[] { "BuildingFiber" };

		public static readonly string[] ANY_BUILDABLE = new string[] { "BuildableAny" };

		public static readonly string[] FLYING_CRITTER_FOOD = new string[] { "FlyingCritterEdible" };

		public static readonly string[] RADIATION_CONTAINMENT = new string[] { "Metal", "Lead" };
	}
}
