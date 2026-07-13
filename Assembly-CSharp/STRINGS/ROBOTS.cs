using System;

namespace STRINGS
{
	public class ROBOTS
	{
		public static LocString CATEGORY_NAME = "Robots";

		public class STATS
		{
			public class INTERNALBATTERY
			{
				public static LocString NAME = "Rechargeable Battery";

				public static LocString TOOLTIP = "When this bot's battery runs out it must temporarily stop working to go recharge";
			}

			public class INTERNALCHEMICALBATTERY
			{
				public static LocString NAME = "Chemical Battery";

				public static LocString TOOLTIP = "This bot will shut down permanently when its battery runs out";
			}

			public class INTERNALBIOBATTERY
			{
				public static LocString NAME = "Biofuel";

				public static LocString TOOLTIP = "This bot will shut down permanently when its biofuel runs out";
			}

			public class INTERNALELECTROBANK
			{
				public static LocString NAME = "Power Bank";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"When this bot's ",
					UI.PRE_KEYWORD,
					"Power Bank",
					UI.PST_KEYWORD,
					" runs out, it will stop working until a fully charged one is delivered"
				});
			}
		}

		public class ATTRIBUTES
		{
			public class INTERNALBATTERYDELTA
			{
				public static LocString NAME = "Rechargeable Battery Drain";

				public static LocString TOOLTIP = "The rate at which battery life is depleted";
			}
		}

		public class STATUSITEMS
		{
			public class CANTREACHSTATION
			{
				public static LocString NAME = "Unreachable Dock";

				public static LocString DESC = "Obstacles are preventing {0} from heading home";

				public static LocString TOOLTIP = "Obstacles are preventing {0} from heading home";
			}

			public class MOVINGTOCHARGESTATION
			{
				public static LocString NAME = "Traveling to Dock";

				public static LocString DESC = "{0} is on its way home to recharge";

				public static LocString TOOLTIP = "{0} is on its way home to recharge";
			}

			public class LOWBATTERY
			{
				public static LocString NAME = "Low Battery";

				public static LocString DESC = "{0}'s battery is low and needs to recharge";

				public static LocString TOOLTIP = "{0}'s battery is low and needs to recharge";
			}

			public class LOWBATTERYNOCHARGE
			{
				public static LocString NAME = "Low Battery";

				public static LocString DESC = "{0}'s battery is low\n\nThe internal battery cannot be recharged and this robot will cease functioning after it is depleted.";

				public static LocString TOOLTIP = "{0}'s battery is low\n\nThe internal battery cannot be recharged and this robot will cease functioning after it is depleted.";
			}

			public class DEADBATTERY
			{
				public static LocString NAME = "Shut Down";

				public static LocString DESC = "RIP {0}\n\n{0}'s battery has been depleted and cannot be recharged";

				public static LocString TOOLTIP = "RIP {0}\n\n{0}'s battery has been depleted and cannot be recharged";
			}

			public class DEADBATTERYFLYDO
			{
				public static LocString NAME = "Shut Down";

				public static LocString DESC = "{0}'s battery has been depleted\n\n{0} will resume function when a new battery has been delivered";

				public static LocString TOOLTIP = "{0}'s battery has been depleted\n\n{0} will resume function when a new battery has been delivered";
			}

			public class DUSTBINFULL
			{
				public static LocString NAME = "Dust Bin Full";

				public static LocString DESC = "{0} must return to its dock to unload";

				public static LocString TOOLTIP = "{0} must return to its dock to unload";
			}

			public class WORKING
			{
				public static LocString NAME = "Working";

				public static LocString DESC = "{0} is working diligently. Great job, {0}!";

				public static LocString TOOLTIP = "{0} is working diligently. Great job, {0}!";
			}

			public class UNLOADINGSTORAGE
			{
				public static LocString NAME = "Unloading";

				public static LocString DESC = "{0} is emptying out its dust bin";

				public static LocString TOOLTIP = "{0} is emptying out its dust bin";
			}

			public class CHARGING
			{
				public static LocString NAME = "Charging";

				public static LocString DESC = "{0} is recharging its battery";

				public static LocString TOOLTIP = "{0} is recharging its battery";
			}

			public class REACTPOSITIVE
			{
				public static LocString NAME = "Happy Reaction";

				public static LocString DESC = "This bot saw something nice!";

				public static LocString TOOLTIP = "This bot saw something nice!";
			}

			public class REACTNEGATIVE
			{
				public static LocString NAME = "Bothered Reaction";

				public static LocString DESC = "This bot saw something upsetting";

				public static LocString TOOLTIP = "This bot saw something upsetting";
			}
		}

		public class MODELS
		{
			public class MORB
			{
				public static LocString NAME = UI.FormatAsLink("Biobot", "STORYTRAITMORBROVER");

				public static LocString DESC = "A Pathogen-Fueled Extravehicular Geo-Exploratory Guidebot (model Y), aka \"P.E.G.G.Y.\"\n\nIt can be assigned basic building tasks and digging duties in hazardous environments.";

				public static LocString CODEX_DESC = "The pathogen-fueled guidebot is designed to maximize a colony's chances of surviving in hostile environments by meeting three core outcomes:\n\n1. Filtration and removal of toxins from environment;\n2. Safe disposal of filtered toxins through conversion into usable biofuel;\n3. Creation of geo-exploration equipment for colony expansion with minimal colonist endangerment.\n\nThe elements aggregated during this process may result in the unintentional spread of contaminants. Specialized training required for safe handling.";
			}

			public class SCOUT
			{
				public static LocString NAME = "Rover";

				public static LocString DESC = "A curious bot that can remotely explore new " + UI.CLUSTERMAP.PLANETOID_KEYWORD + " locations.";
			}

			public class SWEEPBOT
			{
				public static LocString NAME = UI.FormatAsLink("Sweepy", "SWEEPY");

				public static LocString DESC = string.Concat(new string[]
				{
					"An automated sweeping robot.\n\nSweeps up ",
					UI.FormatAsLink("Solid", "ELEMENTS_SOLID"),
					" debris and ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" spills and stores the material back in its ",
					UI.FormatAsLink("Sweepy Dock", "SWEEPBOTSTATION"),
					"."
				});
			}

			public class FLYDO
			{
				public static LocString NAME = UI.FormatAsLink("Flydo", "FETCHDRONE");

				public static LocString DESC = "A programmable delivery robot.\n\nPicks up " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " objects for delivery to selected destinations.";
			}
		}
	}
}
