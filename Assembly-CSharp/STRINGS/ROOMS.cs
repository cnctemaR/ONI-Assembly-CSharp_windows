using System;

namespace STRINGS
{
	public class ROOMS
	{
		public class TYPES
		{
			public static LocString CONFLICTED = "Conflicted Room";

			public class NEUTRAL
			{
				public static LocString NAME = "Miscellaneous Room";

				public static LocString EFFECT = "- No effect";

				public static LocString TOOLTIP = "This area qualifies as a room but has no dedicated use.";
			}

			public class LATRINE
			{
				public static LocString NAME = "Latrine";

				public static LocString EFFECT = "- Stress relief bonus";

				public static LocString TOOLTIP = "Using a toilet will relieve additional stress for Duplicants if it is located in a Latrine.";
			}

			public class BARRACKS
			{
				public static LocString NAME = "Barracks";

				public static LocString EFFECT = "- Stamina recovery bonus";

				public static LocString TOOLTIP = "Sleeping in a bed will restore additional stamina for Duplicants if it is located within Barracks.";
			}

			public class MESSHALL
			{
				public static LocString NAME = "Mess Hall";

				public static LocString EFFECT = "- Stress relief bonus";

				public static LocString TOOLTIP = "Eating at a Mess Table will relieve additional stress for Duplicants if it is located within a Mess Hall.";
			}

			public class HOSPITAL
			{
				public static LocString NAME = "Med Bay";

				public static LocString EFFECT = "- Quarantine sick Duplicants";

				public static LocString TOOLTIP = "Sick Duplicants assigned to medical beds located within a Med Bay are less likely to spread Disease.";
			}

			public class POWER_PLANT
			{
				public static LocString NAME = "Power Plant";

				public static LocString EFFECT = "- Power production increase";

				public static LocString TOOLTIP = "Electrical Engineers can tune-up generators built within a Power Plant, improving their power production.";
			}

			public class MACHINE_SHOP
			{
				public static LocString NAME = "Machine Shop";

				public static LocString EFFECT = "- Increased fabrication efficiency";

				public static LocString TOOLTIP = "Engineers working in a Machine Shop can maintain buildings and increase the speed with which they produce items.";
			}

			public class FARM
			{
				public static LocString NAME = "Greenhouse";

				public static LocString EFFECT = "- Accelerated plant growth";

				public static LocString TOOLTIP = "Crops grown within a Greenhouse can be tended by Farmers to increase their growth speed.";
			}

			public class CREATUREPEN
			{
				public static LocString NAME = "Stable";

				public static LocString EFFECT = "- Hastens critter domestication";

				public static LocString TOOLTIP = "Stabled critters will be more productive and less hostile.";
			}

			public class REC_ROOM
			{
				public static LocString NAME = "Recreation Room";

				public static LocString EFFECT = "- Stress relief bonus";

				public static LocString TOOLTIP = "Leisure time will relieve additional stress for Duplicants visiting a Recreation Room.";
			}

			public class PRIVATE_BEDROOM
			{
				public static LocString NAME = "Private Bedroom";

				public static LocString EFFECT = "- Stamina recovery bonus";

				public static LocString TOOLTIP = "Duplicants recover even more stamina while sleeping in a Private Bedroom than in Barracks.";
			}

			public class PRIVATE_BATHROOM
			{
				public static LocString NAME = "Private Bathroom";

				public static LocString EFFECT = "- Stress relief bonus";

				public static LocString TOOLTIP = "Duplicants relieve even more stress when using the toilet in a Private Bathroom than in a Latrine.";
			}
		}

		public class CRITERIA
		{
			public static LocString HEADER = "Room Criteria";

			public static LocString NEUTRAL_TYPE = "Enclosed by wall tiles and doors";

			public static LocString POSSIBLE_TYPES_HEADER = "Possible Room Types";

			public static LocString NO_TYPE_CONFLICTS = "Remove conflicting buildings";

			public class CRITERIA_FAILED
			{
				public static LocString MISSING_BUILDING = "Missing {0}";

				public static LocString FAILED = "{0}";
			}

			public class MINIMUM_SIZE
			{
				public static LocString NAME = "Minimum size: {0} tiles";

				public static LocString DESCRIPTION = "Must have an area of at least {0} tiles";
			}

			public class MAXIMUM_SIZE
			{
				public static LocString NAME = "Maximum size: {0} tiles";

				public static LocString DESCRIPTION = "Must have an area no larger than {0} tiles";
			}

			public class BED_SINGLE
			{
				public static LocString NAME = "Single bed";

				public static LocString DESCRIPTION = "Requires one Cot or Comfy Bed";
			}

			public class BED_MULTIPLE
			{
				public static LocString NAME = "Beds";

				public static LocString DESCRIPTION = "Requires two or more Cots or Comfy Beds";
			}

			public class BUILDING_DECOR_POSITIVE
			{
				public static LocString NAME = "Positive decor";

				public static LocString DESCRIPTION = "Requires at least one building with positive decor";
			}

			public class DECORATIVE_ITEM
			{
				public static LocString NAME = "Decoration item";

				public static LocString DESCRIPTION = "Requires one or more Paintings, Sculptures, or Vases";
			}

			public class CLINIC
			{
				public static LocString NAME = "Medical bed";

				public static LocString DESCRIPTION = "Requires one or more Med-Beds or Pharma Chambers";
			}

			public class POWER_STATION
			{
				public static LocString NAME = "Power Control Station";

				public static LocString DESCRIPTION = "Requires a single Power Control Station";
			}

			public class FARM_STATION
			{
				public static LocString NAME = "Farm Station";

				public static LocString DESCRIPTION = "Requires a single Farm Station";
			}

			public class CREATURE_RELOCATOR
			{
				public static LocString NAME = "Critter Relocator";

				public static LocString DESCRIPTION = "Requires a single Critter Drop-Off";
			}

			public class CREATURE_FEEDER
			{
				public static LocString NAME = "Critter Feeder";

				public static LocString DESCRIPTION = "Requires a single Critter Feeder";
			}

			public class RANCH_STATION
			{
				public static LocString NAME = "Ranch Station";

				public static LocString DESCRIPTION = "Requires a single Ranch Station";
			}

			public class REC_BUILDING
			{
				public static LocString NAME = "Recreational building";

				public static LocString DESCRIPTION = "Requires one or more Massage Tables";
			}

			public class MACHINE_SHOP
			{
				public static LocString NAME = "Mechanics Station";

				public static LocString DESCRIPTION = "Requires requires one or more Mechanics Stations";
			}

			public class FOOD_BOX
			{
				public static LocString NAME = "Food storage";

				public static LocString DESCRIPTION = "Requires one or more Ration Boxes or Refrigerators";
			}

			public class LIGHT
			{
				public static LocString NAME = "Light source";

				public static LocString DESCRIPTION = "Requires one or more light sources";
			}

			public class MASSAGE_TABLE
			{
				public static LocString NAME = "Massage Table";

				public static LocString DESCRIPTION = "Requires one or more Massage Tables";
			}

			public class MESS_STATION_SINGLE
			{
				public static LocString NAME = "Mess Table";

				public static LocString DESCRIPTION = "Requires a single Mess Table";
			}

			public class MESS_STATION_MULTIPLE
			{
				public static LocString NAME = "Mess Tables";

				public static LocString DESCRIPTION = "Requires two or more Mess Tables";
			}

			public class RESEARCH_STATION
			{
				public static LocString NAME = "Research station";

				public static LocString DESCRIPTION = "Requires one or more Research Stations or Super Computers";
			}

			public class TOILET
			{
				public static LocString NAME = "Toilet";

				public static LocString DESCRIPTION = "Requires one or more Outhouses or Lavatories";
			}

			public class WASH_STATION
			{
				public static LocString NAME = "Wash station";

				public static LocString DESCRIPTION = "Requires one or more Wash Basins, Sinks, Hand Sanitizers, or Showers";
			}

			public class NO_INDUSTRIAL_MACHINERY
			{
				public static LocString NAME = "No industrial machinery";

				public static LocString DESCRIPTION = "Cannot contain any building labelled Industrial Machinery";
			}
		}

		public class DETAILS
		{
			public static LocString HEADER = "Room Details";

			public class ASSIGNED_TO
			{
				public static LocString NAME = "<b>Assignments:</b>\n{0}";

				public static LocString UNASSIGNED = "Unassigned";
			}

			public class AVERAGE_TEMPERATURE
			{
				public static LocString NAME = "Average temperature: {0}";
			}

			public class AVERAGE_ATMO_MASS
			{
				public static LocString NAME = "Average air pressure: {0}";
			}

			public class SIZE
			{
				public static LocString NAME = "Room size: {0} Tiles";
			}

			public class BUILDING_COUNT
			{
				public static LocString NAME = "Buildings: {0}";
			}

			public class CREATURE_COUNT
			{
				public static LocString NAME = "Critters: {0}";
			}
		}
	}
}
