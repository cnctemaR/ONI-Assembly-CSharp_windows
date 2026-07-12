using System;
using TUNING;

namespace STRINGS
{
	public class ROOMS
	{
		public class CATEGORY
		{
			public class NONE
			{
				public static LocString NAME = "None";
			}

			public class FOOD
			{
				public static LocString NAME = "Dining";
			}

			public class SLEEP
			{
				public static LocString NAME = "Sleep";
			}

			public class RECREATION
			{
				public static LocString NAME = "Recreation";
			}

			public class BATHROOM
			{
				public static LocString NAME = "Washroom";
			}

			public class BIONIC
			{
				public static LocString NAME = "";
			}

			public class HOSPITAL
			{
				public static LocString NAME = "Medical";
			}

			public class INDUSTRIAL
			{
				public static LocString NAME = "Industrial";
			}

			public class AGRICULTURAL
			{
				public static LocString NAME = "Agriculture";
			}

			public class PARK
			{
				public static LocString NAME = "Parks";
			}

			public class SCIENCE
			{
				public static LocString NAME = "Science";
			}
		}

		public class TYPES
		{
			public static LocString CONFLICTED = "Conflicted Room";

			public class NEUTRAL
			{
				public static LocString NAME = "Miscellaneous Room";

				public static LocString DESCRIPTION = "An enclosed space with plenty of potential and no dedicated use.";

				public static LocString EFFECT = "- No effect";

				public static LocString TOOLTIP = "This area has walls and doors but no dedicated use";
			}

			public class LATRINE
			{
				public static LocString NAME = "Latrine";

				public static LocString DESCRIPTION = "It's a step up from doing one's business in full view of the rest of the colony.\n\nUsing a toilet in an enclosed room will improve Duplicants' Morale.";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Using a toilet in an enclosed room will improve Duplicants' Morale";
			}

			public class BIONICUPKEEP
			{
				public static LocString NAME = "";

				public static LocString DESCRIPTION = "";

				public static LocString EFFECT = "";

				public static LocString TOOLTIP = "";
			}

			public class PLUMBEDBATHROOM
			{
				public static LocString NAME = "Washroom";

				public static LocString DESCRIPTION = "A sanctuary of personal hygiene.\n\nUsing a fully plumbed Washroom will improve Duplicants' Morale.";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Using a fully plumbed Washroom will improve Duplicants' Morale";
			}

			public class BARRACKS
			{
				public static LocString NAME = "Barracks";

				public static LocString DESCRIPTION = "A basic communal sleeping area for up-and-coming colonies.\n\nSleeping in Barracks will improve Duplicants' Morale.";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Sleeping in Barracks will improve Duplicants' Morale";
			}

			public class BEDROOM
			{
				public static LocString NAME = "Luxury Barracks";

				public static LocString DESCRIPTION = "An upscale communal sleeping area full of things that greatly enhance quality of rest for occupants.\n\nSleeping in a Luxury Barracks will improve Duplicants' Morale.";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Sleeping in a Luxury Barracks will improve Duplicants' Morale";
			}

			public class PRIVATE_BEDROOM
			{
				public static LocString NAME = "Private Bedroom";

				public static LocString DESCRIPTION = "A comfortable, roommate-free retreat where tired Duplicants can get uninterrupted rest.\n\nSleeping in a Private Bedroom will greatly improve Duplicants' Morale.";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Sleeping in a Private Bedroom will greatly improve Duplicants' Morale";
			}

			public class MESSHALL
			{
				public static LocString NAME = "Mess Hall";

				public static LocString DESCRIPTION = "A simple dining room setup that's easy to improve upon.\n\nEating at a mess table in a Mess Hall will increase Duplicants' Morale.";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Eating at a Mess Table in a Mess Hall will improve Duplicants' Morale";
			}

			public class KITCHEN
			{
				public static LocString NAME = "Kitchen";

				public static LocString DESCRIPTION = "A cooking area equipped to take meals to the next level.\n\nAdding ingredients from a Spice Grinder to foods cooked on an Electric Grill or Gas Range provides a variety of positive benefits.";

				public static LocString EFFECT = "- Enables Spice Grinder use";

				public static LocString TOOLTIP = "Using a Spice Grinder in a Kitchen adds benefits to foods cooked on Electric Grill or Gas Range";
			}

			public class GREATHALL
			{
				public static LocString NAME = "Great Hall";

				public static LocString DESCRIPTION = "A great place to eat, with great decor and great company. Great!\n\nEating in a Great Hall will significantly improve Duplicants' Morale.";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Eating in a Great Hall will significantly improve Duplicants' Morale";
			}

			public class HOSPITAL
			{
				public static LocString NAME = "Hospital";

				public static LocString DESCRIPTION = "A dedicated medical facility that helps minimize recovery time.\n\nSick Duplicants assigned to medical buildings located within a Hospital are also less likely to spread Disease.";

				public static LocString EFFECT = "- Quarantine sick Duplicants";

				public static LocString TOOLTIP = "Sick Duplicants assigned to medical buildings located within a Hospital are less likely to spread Disease";
			}

			public class MASSAGE_CLINIC
			{
				public static LocString NAME = "Massage Clinic";

				public static LocString DESCRIPTION = "A soothing space with a very relaxing ambience, especially when well-decorated.\n\nReceiving massages at a Massage Clinic will significantly improve Stress reduction.";

				public static LocString EFFECT = "- Massage stress relief bonus";

				public static LocString TOOLTIP = "Receiving massages at a Massage Clinic will significantly improve Stress reduction";
			}

			public class POWER_PLANT
			{
				public static LocString NAME = "Power Plant";

				public static LocString DESCRIPTION = "The perfect place for Duplicants to flex their Electrical Engineering skills.\n\nHeavy-duty generators built within a Power Plant can be tuned up using microchips from power control stations to improve their " + UI.FormatAsLink("Power", "POWER") + " production.";

				public static LocString EFFECT = "- Enables " + ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.NAME + " tune-ups on heavy-duty generators";

				public static LocString TOOLTIP = "Heavy-duty generators built in a Power Plant can be tuned up using microchips from Power Control Stations to improve their Power production";
			}

			public class MACHINE_SHOP
			{
				public static LocString NAME = "Machine Shop";

				public static LocString DESCRIPTION = "It smells like elbow grease.\n\nDuplicants working in a Machine Shop can maintain buildings and increase their production speed.";

				public static LocString EFFECT = "- Increased fabrication efficiency";

				public static LocString TOOLTIP = "Duplicants working in a Machine Shop can maintain buildings and increase their production speed";
			}

			public class FARM
			{
				public static LocString NAME = "Greenhouse";

				public static LocString DESCRIPTION = "An enclosed agricultural space best utilized by Duplicants with Crop Tending skills.\n\nCrops grown within a Greenhouse can be tended with Farm Station fertilizer to increase their growth speed.";

				public static LocString EFFECT = "- Enables Farm Station use";

				public static LocString TOOLTIP = "Crops grown within a Greenhouse can be tended with Farm Station fertilizer to increase their growth speed";
			}

			public class CREATUREPEN
			{
				public static LocString NAME = "Stable";

				public static LocString DESCRIPTION = "Critters don't mind it here, as long as things don't get too crowded.\n\nStabled critters can be tended to in order to improve their happiness, hasten their domestication and increase their production.\n\nEnables the use of Grooming Stations, Shearing Stations, Critter Condos, Critter Fountains and Milking Stations.";

				public static LocString EFFECT = "- Critter taming and mood bonus";

				public static LocString TOOLTIP = "A stable enables Grooming Station, Critter Condo, Critter Fountain, Shearing Station and Milking Station use";
			}

			public class REC_ROOM
			{
				public static LocString NAME = "Recreation Room";

				public static LocString DESCRIPTION = "Where Duplicants go to mingle with off-duty peers and indulge in a little R&R.\n\nScheduled Downtime will further improve Morale for Duplicants visiting a Recreation Room.";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Scheduled Downtime will further improve Morale for Duplicants visiting a Recreation Room";
			}

			public class PARK
			{
				public static LocString NAME = "Park";

				public static LocString DESCRIPTION = "A little greenery goes a long way.\n\nPassing through natural spaces throughout the day will raise the Morale of Duplicants.";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "Passing through natural spaces throughout the day will raise the Morale of Duplicants";
			}

			public class NATURERESERVE
			{
				public static LocString NAME = "Nature Reserve";

				public static LocString DESCRIPTION = "A lot of greenery goes an even longer way.\n\nPassing through a Nature Reserve will grant higher Morale bonuses to Duplicants than a Park.";

				public static LocString EFFECT = "- Morale bonus";

				public static LocString TOOLTIP = "A Nature Reserve will grant higher Morale bonuses to Duplicants than a Park";
			}

			public class LABORATORY
			{
				public static LocString NAME = "Laboratory";

				public static LocString DESCRIPTION = "Where wild hypotheses meet rigorous scientific experimentation.\n\nScience stations built in a Laboratory function more efficiently.\n\nA Laboratory enables the use of the Geotuner and the Mission Control Station.";

				public static LocString EFFECT = "- Efficiency bonus";

				public static LocString TOOLTIP = "Science buildings built in a Laboratory function more efficiently\n\nA Laboratory enables Geotuner and Mission Control Station use";
			}

			public class PRIVATE_BATHROOM
			{
				public static LocString NAME = "Private Bathroom";

				public static LocString DESCRIPTION = "Finally, a place to truly be alone with one's thoughts.\n\nDuplicants relieve even more Stress when using the toilet in a Private Bathroom than in a Latrine.";

				public static LocString EFFECT = "- Stress relief bonus";

				public static LocString TOOLTIP = "Duplicants relieve even more stress when using the toilet in a Private Bathroom than in a Latrine";
			}

			public class BIONIC_UPKEEP
			{
				public static LocString NAME = "";

				public static LocString DESCRIPTION = "";

				public static LocString EFFECT = "";

				public static LocString TOOLTIP = "";
			}
		}

		public class CRITERIA
		{
			public static LocString HEADER = "<b>Requirements:</b>";

			public static LocString NEUTRAL_TYPE = "Enclosed by wall tile";

			public static LocString POSSIBLE_TYPES_HEADER = "Possible Room Types";

			public static LocString NO_TYPE_CONFLICTS = "Remove conflicting buildings";

			public static LocString IN_CODE_ERROR = "String Key Not Found: {0}";

			public class CRITERIA_FAILED
			{
				public static LocString MISSING_BUILDING = "Missing {0}";

				public static LocString FAILED = "{0}";
			}

			public static class DECORATION
			{
				public static LocString NAME = UI.FormatAsLink("Decor item", "BUILDCATEGORYREQUIREMENTCLASSDECORATION");

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.DECORATION.NAME;
			}

			public class CEILING_HEIGHT
			{
				public static LocString NAME = "Minimum height: {0} tiles";

				public static LocString DESCRIPTION = "Must have a ceiling height of at least {0} tiles";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.CEILING_HEIGHT.NAME;
			}

			public class MINIMUM_SIZE
			{
				public static LocString NAME = "Minimum size: {0} tiles";

				public static LocString DESCRIPTION = "Must have an area of at least {0} tiles";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.MINIMUM_SIZE.NAME;
			}

			public class MAXIMUM_SIZE
			{
				public static LocString NAME = "Maximum size: {0} tiles";

				public static LocString DESCRIPTION = "Must have an area no larger than {0} tiles";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.MAXIMUM_SIZE.NAME;
			}

			public class INDUSTRIALMACHINERY
			{
				public static LocString NAME = UI.FormatAsLink("Industrial machinery", "BUILDCATEGORYREQUIREMENTCLASSINDUSTRIALMACHINERY");

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.INDUSTRIALMACHINERY.NAME;
			}

			public class HAS_BED
			{
				public static LocString NAME = "One or more " + UI.FormatAsLink("beds", "BUILDCATEGORYREQUIREMENTCLASSBEDTYPE");

				public static LocString DESCRIPTION = "Requires at least one Cot or Comfy Bed";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.HAS_BED.NAME;
			}

			public class HAS_LUXURY_BED
			{
				public static LocString NAME = "One or more " + UI.FormatAsLink("Comfy Beds", "LUXURYBED");

				public static LocString DESCRIPTION = "Requires at least one Comfy Bed";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.HAS_LUXURY_BED.NAME;
			}

			public class LUXURYBEDTYPE
			{
				public static LocString NAME = "Single " + UI.FormatAsLink("Comfy Bed", "LUXURYBED");

				public static LocString DESCRIPTION = "Must have no more than one Comfy Bed";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.LUXURYBEDTYPE.NAME;
			}

			public class BED_SINGLE
			{
				public static LocString NAME = "Single " + UI.FormatAsLink("beds", "BUILDCATEGORYREQUIREMENTCLASSBEDTYPE");

				public static LocString DESCRIPTION = "Must have no more than one Cot or Comfy Bed";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.BED_SINGLE.NAME;
			}

			public class IS_BACKWALLED
			{
				public static LocString NAME = "Has backwall tiles";

				public static LocString DESCRIPTION = "Must be covered in backwall tiles";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.IS_BACKWALLED.NAME;
			}

			public class NO_COTS
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Cots", "BED");

				public static LocString DESCRIPTION = "Room cannot contain a Cot";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.NO_COTS.NAME;
			}

			public class NO_LUXURY_BEDS
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Comfy Beds", "LUXURYBED");

				public static LocString DESCRIPTION = "Room cannot contain a Comfy Bed";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.NO_LUXURY_BEDS.NAME;
			}

			public class BEDTYPE
			{
				public static LocString NAME = UI.FormatAsLink("Beds", "BUILDCATEGORYREQUIREMENTCLASSBEDTYPE");

				public static LocString DESCRIPTION = "Requires two or more Cots or Comfy Beds";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.BEDTYPE.NAME;
			}

			public class BUILDING_DECOR_POSITIVE
			{
				public static LocString NAME = "Positive " + UI.FormatAsLink("decor", "BUILDCATEGORYREQUIREMENTCLASSDECORATION");

				public static LocString DESCRIPTION = "Requires at least one building with positive decor";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.BUILDING_DECOR_POSITIVE.NAME;
			}

			public class DECORATIVE_ITEM
			{
				public static LocString NAME = UI.FormatAsLink("Decor item", "BUILDCATEGORYREQUIREMENTCLASSDECORATION") + " ({0})";

				public static LocString DESCRIPTION = "Requires {0} or more Decor items";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.DECORATIVE_ITEM.NAME;
			}

			public class DECOR20
			{
				// Note: this type is marked as 'beforefieldinit'.
				static DECOR20()
				{
					string text = "Requires a decorative item with a minimum Decor value of ";
					int amount = BUILDINGS.DECOR.BONUS.TIER3.amount;
					ROOMS.CRITERIA.DECOR20.DESCRIPTION = text + amount.ToString();
					ROOMS.CRITERIA.DECOR20.CONFLICT_DESCRIPTION = ROOMS.CRITERIA.DECOR20.NAME;
				}

				public static LocString NAME = UI.FormatAsLink("Fancy decor item", "BUILDCATEGORYREQUIREMENTCLASSDECORATION");

				public static LocString DESCRIPTION;

				public static LocString CONFLICT_DESCRIPTION;
			}

			public class CLINIC
			{
				public static LocString NAME = UI.FormatAsLink("Medical equipment", "BUILDCATEGORYREQUIREMENTCLASSCLINIC");

				public static LocString DESCRIPTION = "Requires one or more Sick Bays or Disease Clinics";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.CLINIC.NAME;
			}

			public class POWERPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Heavy-Duty Generator", "BUILDCATEGORYREQUIREMENTCLASSGENERATORTYPE") + "\n    • Two or more " + UI.FormatAsLink("Power Buildings", "BUILDCATEGORYREQUIREMENTCLASSPOWERBUILDING");

				public static LocString DESCRIPTION = "Requires a Heavy-Duty Generator and two or more Power Buildings";

				public static LocString CONFLICT_DESCRIPTION = "Heavy-Duty Generator and two or more Power buildings";
			}

			public class FARMSTATIONTYPE
			{
				public static LocString NAME = UI.FormatAsLink("Farm Station", "FARMSTATION");

				public static LocString DESCRIPTION = "Requires a single Farm Station";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.FARMSTATIONTYPE.NAME;
			}

			public class CREATURERELOCATOR
			{
				public static LocString NAME = UI.FormatAsLink("Critter relocator", "BUILDCATEGORYREQUIREMENTCLASSCREATURERELOCATOR");

				public static LocString DESCRIPTION = "Requires a single Critter Drop-Off or Fish Release";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.CREATURERELOCATOR.NAME;
			}

			public class CREATURE_FEEDER
			{
				public static LocString NAME = UI.FormatAsLink("Critter Feeder", "CREATUREFEEDER");

				public static LocString DESCRIPTION = "Requires a single Critter Feeder";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.CREATURE_FEEDER.NAME;
			}

			public class RANCHSTATIONTYPE
			{
				public static LocString NAME = UI.FormatAsLink("Ranching building", "BUILDCATEGORYREQUIREMENTCLASSRANCHSTATIONTYPE");

				public static LocString DESCRIPTION = "Requires a single Grooming Station, Critter Condo, Critter Fountain, Shearing Station or Milking Station";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.RANCHSTATIONTYPE.NAME;
			}

			public class SPICESTATION
			{
				public static LocString NAME = UI.FormatAsLink("Spice Grinder", "SPICEGRINDER");

				public static LocString DESCRIPTION = "Requires a single Spice Grinder";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.SPICESTATION.NAME;
			}

			public class COOKTOP
			{
				public static LocString NAME = UI.FormatAsLink("Cooking station", "BUILDCATEGORYREQUIREMENTCLASSCOOKTOP");

				public static LocString DESCRIPTION = "Requires a single Electric Grill or Gas Range";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.COOKTOP.NAME;
			}

			public class REFRIGERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Refrigerator", "REFRIGERATOR");

				public static LocString DESCRIPTION = "Requires a single Refrigerator";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.REFRIGERATOR.NAME;
			}

			public class RECBUILDING
			{
				public static LocString NAME = UI.FormatAsLink("Recreational building", "BUILDCATEGORYREQUIREMENTCLASSRECBUILDING");

				public static LocString DESCRIPTION = "Requires one or more recreational buildings";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.RECBUILDING.NAME;
			}

			public class PARK
			{
				public static LocString NAME = UI.FormatAsLink("Park Sign", "PARKSIGN");

				public static LocString DESCRIPTION = "Requires one or more Park Signs";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.PARK.NAME;
			}

			public class MACHINESHOPTYPE
			{
				public static LocString NAME = "Mechanics Station";

				public static LocString DESCRIPTION = "Requires requires one or more Mechanics Stations";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.MACHINESHOPTYPE.NAME;
			}

			public class FOOD_BOX
			{
				public static LocString NAME = "Food storage";

				public static LocString DESCRIPTION = "Requires one or more Ration Boxes or Refrigerators";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.FOOD_BOX.NAME;
			}

			public class LIGHTSOURCE
			{
				public static LocString NAME = UI.FormatAsLink("Light source", "BUILDCATEGORYREQUIREMENTCLASSLIGHTSOURCE");

				public static LocString DESCRIPTION = "Requires one or more light sources";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.LIGHTSOURCE.NAME;
			}

			public class DESTRESSINGBUILDING
			{
				public static LocString NAME = UI.FormatAsLink("De-Stressing Building", "MASSAGETABLE");

				public static LocString DESCRIPTION = "Requires one or more De-Stressing buildings";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.DESTRESSINGBUILDING.NAME;
			}

			public class MASSAGE_TABLE
			{
				public static LocString NAME = UI.FormatAsLink("Massage Table", "MASSAGETABLE");

				public static LocString DESCRIPTION = "Requires one or more Massage Tables";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.MASSAGE_TABLE.NAME;
			}

			public class MESSTABLE
			{
				public static LocString NAME = UI.FormatAsLink("Mess Table", "DININGTABLE");

				public static LocString DESCRIPTION = "Requires a single Mess Table";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.MESSTABLE.NAME;
			}

			public class NO_MESS_STATION
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Mess Table", "DININGTABLE");

				public static LocString DESCRIPTION = "Cannot contain a Mess Table";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.NO_MESS_STATION.NAME;
			}

			public class MESS_STATION_MULTIPLE
			{
				public static LocString NAME = UI.FormatAsLink("Mess Tables", "DININGTABLE");

				public static LocString DESCRIPTION = "Requires two or more Mess Tables";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.MESS_STATION_MULTIPLE.NAME;
			}

			public class RESEARCH_STATION
			{
				public static LocString NAME = UI.FormatAsLink("Research station", "BUILDCATEGORYREQUIREMENTCLASSRESEARCH_STATION");

				public static LocString DESCRIPTION = "Requires one or more Research Stations or Super Computers";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.RESEARCH_STATION.NAME;
			}

			public class BIONICUPKEEP
			{
				public static LocString NAME = UI.FormatAsLink("Bionic service station", "BUILDCATEGORYREQUIREMENTCLASSBIONICUPKEEP");

				public static LocString DESCRIPTION = "Requires at least one Lubrication Station and one Gunk Extractor";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.BIONICUPKEEP.NAME;
			}

			public class BIONIC_GUNKEMPTIER
			{
				public static LocString NAME = UI.FormatAsLink("Gunk Extractor", "BUILDCATEGORYREQUIREMENTCLASSBIONIC_GUNKEMPTIER");

				public static LocString DESCRIPTION = "Requires one or more Gunk Extractors";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.BIONIC_GUNKEMPTIER.NAME;
			}

			public class BIONIC_LUBRICATION
			{
				public static LocString NAME = UI.FormatAsLink("Lubrication Station", "BUILDCATEGORYREQUIREMENTCLASSBIONIC_LUBRICATION");

				public static LocString DESCRIPTION = "Requires one or more Lubrication Stations";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.BIONIC_LUBRICATION.NAME;
			}

			public class TOILETTYPE
			{
				public static LocString NAME = UI.FormatAsLink("Toilet", "BUILDCATEGORYREQUIREMENTCLASSTOILETTYPE");

				public static LocString DESCRIPTION = "Requires one or more Outhouses or Lavatories";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.TOILETTYPE.NAME;
			}

			public class FLUSHTOILETTYPE
			{
				public static LocString NAME = UI.FormatAsLink("Flush Toilet", "BUILDCATEGORYREQUIREMENTCLASSFLUSHTOILETTYPE");

				public static LocString DESCRIPTION = "Requires one or more Lavatories";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.FLUSHTOILETTYPE.NAME;
			}

			public class NO_OUTHOUSES
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Outhouses", "OUTHOUSE");

				public static LocString DESCRIPTION = "Cannot contain basic Outhouses";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.NO_OUTHOUSES.NAME;
			}

			public class WASHSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Wash station", "BUILDCATEGORYREQUIREMENTCLASSWASHSTATION");

				public static LocString DESCRIPTION = "Requires one or more Wash Basins, Sinks, Hand Sanitizers, or Showers";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.WASHSTATION.NAME;
			}

			public class ADVANCEDWASHSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Plumbed wash station", "BUILDCATEGORYREQUIREMENTCLASSWASHSTATION");

				public static LocString DESCRIPTION = "Requires one or more Sinks, Hand Sanitizers, or Showers";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.ADVANCEDWASHSTATION.NAME;
			}

			public class NO_INDUSTRIAL_MACHINERY
			{
				public static LocString NAME = "No " + UI.FormatAsLink("industrial machinery", "BUILDCATEGORYREQUIREMENTCLASSINDUSTRIALMACHINERY");

				public static LocString DESCRIPTION = "Cannot contain any building labeled Industrial Machinery";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.NO_INDUSTRIAL_MACHINERY.NAME;
			}

			public class WILDANIMAL
			{
				public static LocString NAME = "Wildlife";

				public static LocString DESCRIPTION = "Requires at least one wild critter";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.WILDANIMAL.NAME;
			}

			public class WILDANIMALS
			{
				public static LocString NAME = "More wildlife";

				public static LocString DESCRIPTION = "Requires two or more wild critters";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.WILDANIMALS.NAME;
			}

			public class WILDPLANT
			{
				public static LocString NAME = "Two wild plants";

				public static LocString DESCRIPTION = "Requires two or more wild plants";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.WILDPLANT.NAME;
			}

			public class WILDPLANTS
			{
				public static LocString NAME = "Four wild plants";

				public static LocString DESCRIPTION = "Requires four or more wild plants";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.WILDPLANTS.NAME;
			}

			public class SCIENCEBUILDING
			{
				public static LocString NAME = UI.FormatAsLink("Science building", "BUILDCATEGORYREQUIREMENTCLASSSCIENCEBUILDING");

				public static LocString DESCRIPTION = "Requires one or more science buildings";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.SCIENCEBUILDING.NAME;
			}

			public class SCIENCE_BUILDINGS
			{
				public static LocString NAME = "Two " + UI.FormatAsLink("science buildings", "BUILDCATEGORYREQUIREMENTCLASSSCIENCEBUILDING");

				public static LocString DESCRIPTION = "Requires two or more science buildings";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.SCIENCE_BUILDINGS.NAME;
			}

			public class ROCKETINTERIOR
			{
				public static LocString NAME = UI.FormatAsLink("Rocket interior", "BUILDCATEGORYREQUIREMENTCLASSROCKETINTERIOR");

				public static LocString DESCRIPTION = "Must be built inside a rocket";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.ROCKETINTERIOR.NAME;
			}

			public class WARMINGSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Warming station", "BUILDCATEGORYREQUIREMENTCLASSWARMINGSTATION");

				public static LocString DESCRIPTION = "Raises the ambient temperature";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.WARMINGSTATION.NAME;
			}

			public class GENERATORTYPE
			{
				public static LocString NAME = UI.FormatAsLink("Generator", "BUILDCATEGORYREQUIREMENTCLASSGENERATORTYPE");

				public static LocString DESCRIPTION = "Generates electrical power";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.GENERATORTYPE.NAME;
			}

			public class HEAVYDUTYGENERATORTYPE
			{
				public static LocString NAME = UI.FormatAsLink("Heavy-duty generator", "BUILDCATEGORYREQUIREMENTCLASSGENERATORTYPE");

				public static LocString DESCRIPTION = "For big power needs";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.HEAVYDUTYGENERATORTYPE.NAME;
			}

			public class LIGHTDUTYGENERATORTYPE
			{
				public static LocString NAME = UI.FormatAsLink("Basic generator", "BUILDCATEGORYREQUIREMENTCLASSGENERATORTYPE");

				public static LocString DESCRIPTION = "For basic power needs";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.LIGHTDUTYGENERATORTYPE.NAME;
			}

			public class POWERBUILDING
			{
				public static LocString NAME = UI.FormatAsLink("Power building", "BUILDCATEGORYREQUIREMENTCLASSPOWERBUILDING");

				public static LocString DESCRIPTION = "Buildings that generate, store, or manage power";

				public static LocString CONFLICT_DESCRIPTION = ROOMS.CRITERIA.POWERBUILDING.NAME;
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

			public class PLANT_COUNT
			{
				public static LocString NAME = "Plants: {0}";
			}
		}

		public class EFFECTS
		{
			public static LocString HEADER = "<b>Effects:</b>";
		}
	}
}
