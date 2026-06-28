using System;
using STRINGS;

namespace TUNING
{
	public class EQUIPMENT
	{
		public static EquipmentSlot[] SLOTS = new EquipmentSlot[]
		{
			new EquipmentSlot(EQUIPMENT.SUITS.SLOT, MISC.TAGS.SUIT, true),
			new EquipmentSlot(EQUIPMENT.TOOLS.TOOLSLOT, MISC.TAGS.MULTITOOL, false)
		};

		public class ATTRIBUTE_MOD_IDS
		{
			public static string DECOR = "Decor";

			public static string INSULATION = "Insulation";

			public static string ATHLETICS = "Athletics";

			public static string DIGGING = "Digging";

			public static string MAX_UNDERWATER_TRAVELCOST = "MaxUnderwaterTravelCost";
		}

		public class TOOLS
		{
			public static string TOOLSLOT = "Multitool";

			public static string TOOLFABRICATOR = "MultitoolWorkbench";

			public static string TOOL_ANIM = "constructor_gun_kanim";

			public static int BORINGMACHINE_DIG = 5;

			public static int BORINGMACHINE_FABTIME = 40;

			public static int BORINGMACHINE_MASS = 25;

			public static int QUARRYINGGUN_DIG = 2;

			public static int QUARRYINGGUN_FABTIME = EQUIPMENT.TOOLS.BORINGMACHINE_FABTIME;

			public static int QUARRYINGGUN_MASS = 100;
		}

		public class SUITS
		{
			public static string SLOT = "Suit";

			public static string FABRICATOR = "SuitFabricator";

			public static string ANIM = "clothing_kanim";

			public static string SNAPON = "snapTo_neck";

			public static int ATMOSUIT_FABTIME = 40;

			public static int ATMOSUIT_INSULATION = 5;

			public static int ATMOSUIT_ATHLETICS = -3;

			public static int ATMOSUIT_MASS = 200;

			public static int AQUASUIT_FABTIME = EQUIPMENT.SUITS.ATMOSUIT_FABTIME;

			public static int AQUASUIT_INSULATION = 0;

			public static int AQUASUIT_ATHLETICS = EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS;

			public static int AQUASUIT_MASS = EQUIPMENT.SUITS.ATMOSUIT_MASS;

			public static int AQUASUIT_UNDERWATER_TRAVELCOST = 6;

			public static int TEMPERATURESUIT_FABTIME = EQUIPMENT.SUITS.ATMOSUIT_FABTIME;

			public static int TEMPERATURESUIT_INSULATION = 10;

			public static int TEMPERATURESUIT_ATHLETICS = EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS;

			public static int TEMPERATURESUIT_MASS = EQUIPMENT.SUITS.ATMOSUIT_MASS;
		}

		public class VESTS
		{
			public static string SLOT = "Suit";

			public static string FABRICATOR = "ClothingFabricator";

			public static string SNAPON0 = "snapTo_body";

			public static string SNAPON1 = "snapTo_arm";

			public static string WARM_VEST_ANIM0 = "body_shirt_hot01_kanim";

			public static string WARM_VEST_ANIM1 = "body_shirt_hot02_kanim";

			public static string WARM_VEST_ICON0 = "shirt_hot01_kanim";

			public static string WARM_VEST_ICON1 = "shirt_hot02_kanim";

			public static float WARM_VEST_FABTIME = 180f;

			public static float WARM_VEST_INSULATION = 0.01f;

			public static int WARM_VEST_MASS = 4;

			public static string COOL_VEST_ANIM0 = "body_shirt_cold01_kanim";

			public static string COOL_VEST_ANIM1 = "body_shirt_cold02_kanim";

			public static string COOL_VEST_ICON0 = "shirt_cold01_kanim";

			public static string COOL_VEST_ICON1 = "shirt_cold02_kanim";

			public static float COOL_VEST_FABTIME = EQUIPMENT.VESTS.WARM_VEST_FABTIME;

			public static float COOL_VEST_INSULATION = 0.01f;

			public static int COOL_VEST_MASS = EQUIPMENT.VESTS.WARM_VEST_MASS;

			public static string FUNKY_VEST_ANIM0 = "body_shirt_decor01_kanim";

			public static string FUNKY_VEST_ICON0 = "shirt_decor01_kanim";

			public static float FUNKY_VEST_FABTIME = EQUIPMENT.VESTS.WARM_VEST_FABTIME;

			public static float FUNKY_VEST_DECOR = 1f;

			public static int FUNKY_VEST_MASS = EQUIPMENT.VESTS.WARM_VEST_MASS;
		}
	}
}
