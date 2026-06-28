using System;

namespace TUNING
{
	public class EQUIPMENT
	{
		public static EquipmentSlot[] SLOTS = new EquipmentSlot[]
		{
			new EquipmentSlot(EQUIPMENT.SUITS.SUITSLOT, EQUIPMENT.SUITS.SUITSLOT),
			new EquipmentSlot(EQUIPMENT.TOOLS.TOOLSLOT, EQUIPMENT.TOOLS.TOOLSLOT)
		};

		public class ATTRIBUTEMOD_IDS
		{
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
			public static string SUITSLOT = "Suit";

			public static string SUITFABRICATOR = "SuitFabricator";

			public static string SUIT_ANIM = "clothing_kanim";

			public static string SUIT_SNAPON = "snapTo_neck";

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
	}
}
