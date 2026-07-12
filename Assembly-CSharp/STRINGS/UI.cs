using System;
using System.Collections.Generic;

namespace STRINGS
{
	public class UI
	{
		public static string FormatAsBuildMenuTab(string text)
		{
			return "<b>" + text + "</b>";
		}

		public static string FormatAsBuildMenuTab(string text, string hotkey)
		{
			return "<b>" + text + "</b> " + UI.FormatAsHotkey(hotkey);
		}

		public static string FormatAsBuildMenuTab(string text, global::Action a)
		{
			return "<b>" + text + "</b> " + UI.FormatAsHotKey(a);
		}

		public static string FormatAsOverlay(string text)
		{
			return "<b>" + text + "</b>";
		}

		public static string FormatAsOverlay(string text, string hotkey)
		{
			return "<b>" + text + "</b> " + UI.FormatAsHotkey(hotkey);
		}

		public static string FormatAsOverlay(string text, global::Action a)
		{
			return "<b>" + text + "</b> " + UI.FormatAsHotKey(a);
		}

		public static string FormatAsManagementMenu(string text)
		{
			return "<b>" + text + "</b>";
		}

		public static string FormatAsManagementMenu(string text, string hotkey)
		{
			return "<b>" + text + "</b> " + UI.FormatAsHotkey(hotkey);
		}

		public static string FormatAsManagementMenu(string text, global::Action a)
		{
			return "<b>" + text + "</b> " + UI.FormatAsHotKey(a);
		}

		public static string FormatAsKeyWord(string text)
		{
			return UI.PRE_KEYWORD + text + UI.PST_KEYWORD;
		}

		public static string FormatAsHotkey(string text)
		{
			return "<b><color=#F44A4A>" + text + "</b></color>";
		}

		public static string FormatAsHotKey(global::Action a)
		{
			return "{Hotkey/" + a.ToString() + "}";
		}

		public static string FormatAsTool(string text, string hotkey)
		{
			return "<b>" + text + "</b> " + UI.FormatAsHotkey(hotkey);
		}

		public static string FormatAsTool(string text, global::Action a)
		{
			return "<b>" + text + "</b> " + UI.FormatAsHotKey(a);
		}

		public static string FormatAsLink(string text, string linkID)
		{
			text = UI.StripLinkFormatting(text);
			linkID = CodexCache.FormatLinkID(linkID);
			return string.Concat(new string[] { "<link=\"", linkID, "\">", text, "</link>" });
		}

		public static string FormatAsPositiveModifier(string text)
		{
			return UI.PRE_POS_MODIFIER + text + UI.PST_POS_MODIFIER;
		}

		public static string FormatAsNegativeModifier(string text)
		{
			return UI.PRE_NEG_MODIFIER + text + UI.PST_NEG_MODIFIER;
		}

		public static string FormatAsPositiveRate(string text)
		{
			return UI.PRE_RATE_POSITIVE + text + UI.PST_RATE;
		}

		public static string FormatAsNegativeRate(string text)
		{
			return UI.PRE_RATE_NEGATIVE + text + UI.PST_RATE;
		}

		public static string CLICK(UI.ClickType c)
		{
			return "(ClickType/" + c.ToString() + ")";
		}

		public static string FormatAsAutomationState(string text, UI.AutomationState state)
		{
			if (state == UI.AutomationState.Active)
			{
				return UI.PRE_AUTOMATION_ACTIVE + text + UI.PST_AUTOMATION;
			}
			return UI.PRE_AUTOMATION_STANDBY + text + UI.PST_AUTOMATION;
		}

		public static string FormatAsCaps(string text)
		{
			return text.ToUpper();
		}

		public static string ExtractLinkID(string text)
		{
			string text2 = text;
			int num = text2.IndexOf("<link=");
			if (num != -1)
			{
				int num2 = num + 7;
				int num3 = text2.IndexOf(">") - 1;
				text2 = text.Substring(num2, num3 - num2);
			}
			return text2;
		}

		public static string StripLinkFormatting(string text)
		{
			string text2 = text;
			try
			{
				while (text2.Contains("<link="))
				{
					int num = text2.IndexOf("</link>");
					if (num > -1)
					{
						text2 = text2.Remove(num, 7);
					}
					else
					{
						Debug.LogWarningFormat("String has no closing link tag: {0}", Array.Empty<object>());
					}
					int num2 = text2.IndexOf("<link=");
					if (num2 != -1)
					{
						text2 = text2.Remove(num2, 7);
					}
					else
					{
						Debug.LogWarningFormat("String has no open link tag: {0}", Array.Empty<object>());
					}
					int num3 = text2.IndexOf("\">");
					if (num3 != -1)
					{
						text2 = text2.Remove(num2, num3 - num2 + 2);
					}
					else
					{
						Debug.LogWarningFormat("String has no open link tag: {0}", Array.Empty<object>());
					}
				}
			}
			catch
			{
				Debug.Log("STRIP LINK FORMATTING FAILED ON: " + text);
				text2 = text;
			}
			return text2;
		}

		public static string PRE_KEYWORD = "<style=\"KKeyword\">";

		public static string PST_KEYWORD = "</style>";

		public static string PRE_POS_MODIFIER = "<b>";

		public static string PST_POS_MODIFIER = "</b>";

		public static string PRE_NEG_MODIFIER = "<b>";

		public static string PST_NEG_MODIFIER = "</b>";

		public static string PRE_RATE_NEGATIVE = "<style=\"consumed\">";

		public static string PRE_RATE_POSITIVE = "<style=\"produced\">";

		public static string PST_RATE = "</style>";

		public static string PRE_AUTOMATION_ACTIVE = "<b><style=\"logic_on\">";

		public static string PRE_AUTOMATION_STANDBY = "<b><style=\"logic_off\">";

		public static string PST_AUTOMATION = "</style></b>";

		public static string YELLOW_PREFIX = "<color=#ffff00ff>";

		public static string COLOR_SUFFIX = "</color>";

		public static string HORIZONTAL_RULE = "------------------";

		public static string HORIZONTAL_BR_RULE = "\n" + UI.HORIZONTAL_RULE + "\n";

		public static LocString POS_INFINITY = "Infinity";

		public static LocString NEG_INFINITY = "-Infinity";

		public static LocString PROCEED_BUTTON = "PROCEED";

		public static LocString COPY_BUILDING = "Copy";

		public static LocString COPY_BUILDING_TOOLTIP = "Create new build orders using the most recent building selection as a template. {Hotkey}";

		public static LocString NAME_WITH_UNITS = "{0} x {1}";

		public static LocString NA = "N/A";

		public static LocString POSITIVE_FORMAT = "+{0}";

		public static LocString NEGATIVE_FORMAT = "-{0}";

		public static LocString FILTER = "Filter";

		public static LocString SPEED_SLOW = "SLOW";

		public static LocString SPEED_MEDIUM = "MEDIUM";

		public static LocString SPEED_FAST = "FAST";

		public static LocString RED_ALERT = "RED ALERT";

		public static LocString JOBS = "PRIORITIES";

		public static LocString CONSUMABLES = "CONSUMABLES";

		public static LocString VITALS = "VITALS";

		public static LocString RESEARCH = "RESEARCH";

		public static LocString ROLES = "JOB ASSIGNMENTS";

		public static LocString RESEARCHPOINTS = "Research points";

		public static LocString SCHEDULE = "SCHEDULE";

		public static LocString REPORT = "REPORTS";

		public static LocString SKILLS = "SKILLS";

		public static LocString OVERLAYSTITLE = "OVERLAYS";

		public static LocString ALERTS = "ALERTS";

		public static LocString MESSAGES = "MESSAGES";

		public static LocString ACTIONS = "ACTIONS";

		public static LocString QUEUE = "Queue";

		public static LocString BASECOUNT = "Base {0}";

		public static LocString CHARACTERCONTAINER_SKILLS_TITLE = "ATTRIBUTES";

		public static LocString CHARACTERCONTAINER_TRAITS_TITLE = "TRAITS";

		public static LocString CHARACTERCONTAINER_APTITUDES_TITLE = "INTERESTS";

		public static LocString CHARACTERCONTAINER_APTITUDES_TITLE_TOOLTIP = "A Duplicant's starting Attributes are determined by their Interests\n\nLearning Skills related to their Interests will give Duplicants a Morale Boost";

		public static LocString CHARACTERCONTAINER_EXPECTATIONS_TITLE = "ADDITIONAL INFORMATION";

		public static LocString CHARACTERCONTAINER_SKILL_VALUE = " {0} {1}";

		public static LocString CHARACTERCONTAINER_NEED = "{0}: {1}";

		public static LocString CHARACTERCONTAINER_STRESSTRAIT = "Stress Reaction: {0}";

		public static LocString CHARACTERCONTAINER_JOYTRAIT = "Overjoyed Response: {0}";

		public static LocString CHARACTERCONTAINER_CONGENITALTRAIT = "Genetic Trait: {0}";

		public static LocString CHARACTERCONTAINER_NOARCHETYPESELECTED = "Random";

		public static LocString CHARACTERCONTAINER_ARCHETYPESELECT_TOOLTIP = "Influence what type of Duplicant the reroll button will produce";

		public static LocString CAREPACKAGECONTAINER_INFORMATION_TITLE = "CARE PACKAGE";

		public static LocString CHARACTERCONTAINER_ATTRIBUTEMODIFIER_INCREASED = "Increased <b>{0}</b>";

		public static LocString CHARACTERCONTAINER_ATTRIBUTEMODIFIER_DECREASED = "Decreased <b>{0}</b>";

		public static LocString PRODUCTINFO_SELECTMATERIAL = "Select {0}:";

		public static LocString PRODUCTINFO_RESEARCHREQUIRED = "Research required...";

		public static LocString PRODUCTINFO_REQUIRESRESEARCHDESC = "Requires {0} Research";

		public static LocString PRODUCTINFO_APPLICABLERESOURCES = "Required resources:";

		public static LocString PRODUCTINFO_MISSINGRESOURCES_TITLE = "Requires {0}: {1}";

		public static LocString PRODUCTINFO_MISSINGRESOURCES_HOVER = "Missing resources";

		public static LocString PRODUCTINFO_MISSINGRESOURCES_DESC = "{0} has yet to be discovered";

		public static LocString PRODUCTINFO_UNIQUE_PER_WORLD = "Limit one per " + UI.CLUSTERMAP.PLANETOID_KEYWORD;

		public static LocString PRODUCTINFO_ROCKET_INTERIOR = "Rocket interior only";

		public static LocString PRODUCTINFO_ROCKET_NOT_INTERIOR = "Cannot build inside rocket";

		public static LocString BUILDTOOL_ROTATE = "Rotate this building";

		public static LocString BUILDTOOL_ROTATE_CURRENT_DEGREES = "Currently rotated {Degrees} degrees";

		public static LocString BUILDTOOL_ROTATE_CURRENT_LEFT = "Currently facing left";

		public static LocString BUILDTOOL_ROTATE_CURRENT_RIGHT = "Currently facing right";

		public static LocString BUILDTOOL_ROTATE_CURRENT_UP = "Currently facing up";

		public static LocString BUILDTOOL_ROTATE_CURRENT_DOWN = "Currently facing down";

		public static LocString BUILDTOOL_ROTATE_CURRENT_UPRIGHT = "Currently upright";

		public static LocString BUILDTOOL_ROTATE_CURRENT_ON_SIDE = "Currently on its side";

		public static LocString BUILDTOOL_CANT_ROTATE = "This building cannot be rotated";

		public static LocString EQUIPMENTTAB_OWNED = "Owned Items";

		public static LocString EQUIPMENTTAB_HELD = "Held Items";

		public static LocString EQUIPMENTTAB_ROOM = "Assigned Rooms";

		public static LocString JOBSCREEN_PRIORITY = "Priority";

		public static LocString JOBSCREEN_HIGH = "High";

		public static LocString JOBSCREEN_LOW = "Low";

		public static LocString JOBSCREEN_EVERYONE = "Everyone";

		public static LocString JOBSCREEN_DEFAULT = "New Duplicants";

		public static LocString BUILD_REQUIRES_SKILL = "Skill: {Skill}";

		public static LocString BUILD_REQUIRES_SKILL_TOOLTIP = "At least one Duplicant must have the {Skill} Skill to construct this building";

		public static LocString VITALSSCREEN_NAME = "Name";

		public static LocString VITALSSCREEN_STRESS = "Stress";

		public static LocString VITALSSCREEN_HEALTH = "Health";

		public static LocString VITALSSCREEN_SICKNESS = "Disease";

		public static LocString VITALSSCREEN_CALORIES = "Fullness";

		public static LocString VITALSSCREEN_RATIONS = "Calories / Cycle";

		public static LocString VITALSSCREEN_EATENTODAY = "Eaten Today";

		public static LocString VITALSSCREEN_RATIONS_TOOLTIP = "Set how many calories this Duplicant may consume daily";

		public static LocString VITALSSCREEN_EATENTODAY_TOOLTIP = "The amount of food this Duplicant has eaten this cycle";

		public static LocString VITALSSCREEN_UNTIL_FULL = "Until Full";

		public static LocString RESEARCHSCREEN_UNLOCKSTOOLTIP = "Unlocks: {0}";

		public static LocString RESEARCHSCREEN_FILTER = "Search Tech";

		public static LocString ATTRIBUTELEVEL = "Expertise: Level {0} {1}";

		public static LocString ATTRIBUTELEVEL_SHORT = "Level {0} {1}";

		public static LocString NEUTRONIUMMASS = "Immeasurable";

		public static LocString CALCULATING = "Calculating...";

		public static LocString FORMATDAY = "{0} cycles";

		public static LocString FORMATSECONDS = "{0}s";

		public static LocString DELIVERED = "Delivered: {0} {1}";

		public static LocString PICKEDUP = "Picked Up: {0} {1}";

		public static LocString COPIED_SETTINGS = "Settings Applied";

		public static LocString WELCOMEMESSAGETITLE = "- ALERT -";

		public static LocString WELCOMEMESSAGEBODY = "I've awoken at the target location, but colonization efforts have already hit a hitch. I was supposed to land on the planet's surface, but became trapped many miles underground instead.\n\nAlthough the conditions are not ideal, it's imperative that I establish a colony here and begin mounting efforts to escape.";

		public static LocString WELCOMEMESSAGEBODY_SPACEDOUT = "The asteroid we call home has collided with an anomalous planet, decimating our colony. Rebuilding it is of the utmost importance.\n\nI've detected a new cluster of material-rich planetoids in nearby space. If I can guide the Duplicants through the perils of space travel, we could build a colony even bigger and better than before.";

		public static LocString WELCOMEMESSAGEBODY_KF23 = "This asteroid is oddly tilted, as though a powerful external force once knocked it off its axis.\n\nI'll need to recalibrate my approach to colony-building in order to make the most of this unusual distribution of resources.";

		public static LocString WELCOMEMESSAGEBEGIN = "BEGIN";

		public static LocString VIEWDUPLICANTS = "Choose a Blueprint";

		public static LocString DUPLICANTPRINTING = "Duplicant Printing";

		public static LocString ASSIGNDUPLICANT = "Assign Duplicant";

		public static LocString CRAFT = "ADD TO QUEUE";

		public static LocString CLEAR_COMPLETED = "CLEAR COMPLETED ORDERS";

		public static LocString CRAFT_CONTINUOUS = "CONTINUOUS";

		public static LocString INCUBATE_CONTINUOUS_TOOLTIP = "When checked, this building will continuously incubate eggs of the selected type";

		public static LocString PLACEINRECEPTACLE = "Plant";

		public static LocString REMOVEFROMRECEPTACLE = "Uproot";

		public static LocString CANCELPLACEINRECEPTACLE = "Cancel";

		public static LocString CANCELREMOVALFROMRECEPTACLE = "Cancel";

		public static LocString CHANGEPERSECOND = "Change per second: {0}";

		public static LocString CHANGEPERCYCLE = "Change per cycle: {0}";

		public static LocString MODIFIER_ITEM_TEMPLATE = "    • {0}: {1}";

		public static LocString LISTENTRYSTRING = "     {0}\n";

		public static LocString LISTENTRYSTRINGNOLINEBREAK = "     {0}";

		public static class PLATFORMS
		{
			public static LocString UNKNOWN = "Your game client";

			public static LocString STEAM = "Steam";

			public static LocString EPIC = "Epic Games Store";

			public static LocString WEGAME = "Wegame";
		}

		private enum KeywordType
		{
			Hotkey,
			BuildMenu,
			Attribute,
			Generic
		}

		public enum ClickType
		{
			Click,
			Clicked,
			Clicking,
			Clickable,
			Clicks,
			click,
			clicked,
			clicking,
			clickable,
			clicks,
			CLICK,
			CLICKED,
			CLICKING,
			CLICKABLE,
			CLICKS
		}

		public enum AutomationState
		{
			Active,
			Standby
		}

		public class VANILLA
		{
			public static LocString NAME = "base game";

			public static LocString NAME_ITAL = "<i>" + UI.VANILLA.NAME + "</i>";
		}

		public class DLC1
		{
			public static LocString NAME = "Spaced Out!";

			public static LocString NAME_ITAL = "<i>" + UI.DLC1.NAME + "</i>";
		}

		public class DIAGNOSTICS_SCREEN
		{
			public static LocString TITLE = "Diagnostics";

			public static LocString DIAGNOSTIC = "Diagnostic";

			public static LocString TOTAL = "Total";

			public static LocString RESERVED = "Reserved";

			public static LocString STATUS = "Status";

			public static LocString SEARCH = "Search";

			public static LocString CRITERIA_HEADER_TOOLTIP = "Expand or collapse diagnostic criteria panel";

			public static LocString SEE_ALL = "+ See All ({0})";

			public static LocString CRITERIA_TOOLTIP = "Toggle the <b>{0}</b> diagnostics evaluation of the <b>{1}</b> criteria.";

			public static LocString CRITERIA_ENABLED_COUNT = "{0}/{1} criteria enabled";

			public class CLICK_TOGGLE_MESSAGE
			{
				public static LocString ALWAYS = UI.CLICK(UI.ClickType.Click) + " to pin this diagnostic to the sidebar - Current State: <b>Visible On Alert Only</b>";

				public static LocString ALERT_ONLY = UI.CLICK(UI.ClickType.Click) + " to subscribe to this diagnostic - Current State: <b>Never Visible</b>";

				public static LocString NEVER = UI.CLICK(UI.ClickType.Click) + " to mute this diagnostic on the sidebar - Current State: <b>Always Visible</b>";

				public static LocString TUTORIAL_DISABLED = UI.CLICK(UI.ClickType.Click) + " to enable this diagnostic -  Current State: <b>Temporarily disabled</b>";
			}
		}

		public class WORLD_SELECTOR_SCREEN
		{
			public static LocString TITLE = UI.CLUSTERMAP.PLANETOID;
		}

		public class COLONY_DIAGNOSTICS
		{
			public static LocString NO_MINIONS = "    • There are no Duplicants on this {0}";

			public static LocString ROCKET = "rocket";

			public static LocString NO_MINIONS_REQUESTED = "    • Crew must be requested to update this diagnostic";

			public static LocString NO_DATA = "    • Not enough data for evaluation";

			public static LocString NO_DATA_SHORT = "    • No data";

			public static LocString MUTE_TUTORIAL = "Diagnostic can be muted in the <b><color=#E5B000>See All</color></b> panel";

			public static LocString GENERIC_STATUS_NORMAL = "All values nominal";

			public static LocString PLACEHOLDER_CRITERIA_NAME = "Placeholder Criteria Name";

			public static LocString GENERIC_CRITERIA_PASS = "Criteria met";

			public static LocString GENERIC_CRITERIA_FAIL = "Criteria not met";

			public class GENERIC_CRITERIA
			{
				public static LocString CHECKWORLDHASMINIONS = "Check world has Duplicants";
			}

			public class IDLEDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Idleness";

				public static LocString TOOLTIP_NAME = "<b>Idleness</b>";

				public static LocString NORMAL = "    • All Duplicants currently have tasks";

				public static LocString IDLE = "    • One or more Duplicants are idle";

				public static class CRITERIA
				{
					public static LocString CHECKIDLE = "Check idle";
				}
			}

			public class CHOREGROUPDIAGNOSTIC
			{
				public static LocString ALL_NAME = UI.COLONY_DIAGNOSTICS.ALLCHORESDIAGNOSTIC.ALL_NAME;

				public static class CRITERIA
				{
				}
			}

			public class ALLCHORESDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Errands";

				public static LocString TOOLTIP_NAME = "<b>Errands</b>";

				public static LocString NORMAL = "    • {0} errands pending or in progress";

				public static class CRITERIA
				{
				}
			}

			public class WORKTIMEDIAGNOSTIC
			{
				public static LocString ALL_NAME = UI.COLONY_DIAGNOSTICS.ALLCHORESDIAGNOSTIC.ALL_NAME;

				public static class CRITERIA
				{
				}
			}

			public class ALLWORKTIMEDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Work Time";

				public static LocString TOOLTIP_NAME = "<b>Work Time</b>";

				public static LocString NORMAL = "    • {0} of Duplicant time spent working";

				public static class CRITERIA
				{
				}
			}

			public class TRAVEL_TIME
			{
				public static LocString ALL_NAME = "Travel Time";

				public static LocString TOOLTIP_NAME = "<b>Travel Time</b>";

				public static LocString NORMAL = "    • {0} of Duplicant time spent traveling between errands";

				public static class CRITERIA
				{
				}
			}

			public class TRAPPEDDUPLICANTDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Trapped";

				public static LocString TOOLTIP_NAME = "<b>Trapped</b>";

				public static LocString NORMAL = "    • No Duplicants are trapped";

				public static LocString STUCK = "    • One or more Duplicants are trapped";

				public static class CRITERIA
				{
					public static LocString CHECKTRAPPED = "Check Trapped";
				}
			}

			public class BREATHABILITYDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Breathability";

				public static LocString TOOLTIP_NAME = "<b>Breathability</b>";

				public static LocString NORMAL = "    • Oxygen levels are satisfactory";

				public static LocString POOR = "    • Oxygen is becoming scarce or low pressure";

				public static LocString SUFFOCATING = "    • One or more Duplicants are suffocating";

				public static class CRITERIA
				{
					public static LocString CHECKSUFFOCATION = "Check suffocation";

					public static LocString CHECKLOWBREATHABILITY = "Check low breathability";
				}
			}

			public class STRESSDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Max Stress";

				public static LocString TOOLTIP_NAME = "<b>Max Stress</b>";

				public static LocString HIGH_STRESS = "    • One or more Duplicants is suffering high stress";

				public static LocString NORMAL = "    • Duplicants have acceptable stress levels";

				public static class CRITERIA
				{
					public static LocString CHECKSTRESSED = "Check stressed";
				}
			}

			public class DECORDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Decor";

				public static LocString TOOLTIP_NAME = "<b>Decor</b>";

				public static LocString LOW = "    • Decor levels are low";

				public static LocString NORMAL = "    • Decor levels are satisfactory";

				public static class CRITERIA
				{
					public static LocString CHECKDECOR = "Check decor";
				}
			}

			public class TOILETDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Toilets";

				public static LocString TOOLTIP_NAME = "<b>Toilets</b>";

				public static LocString NO_TOILETS = "    • Colony has no toilets";

				public static LocString NO_WORKING_TOILETS = "    • Colony has no working toilets";

				public static LocString TOILET_URGENT = "    • Duplicants urgently need to use a toilet";

				public static LocString FEW_TOILETS = "    • Toilet-to-Duplicant ratio is low";

				public static LocString INOPERATIONAL = "    • One or more toilets are out of order";

				public static LocString NORMAL = "    • Colony has adequate working toilets";

				public static class CRITERIA
				{
					public static LocString CHECKHASANYTOILETS = "Check has any toilets";

					public static LocString CHECKENOUGHTOILETS = "Check enough toilets";

					public static LocString CHECKBLADDERS = "Check Duplicants really need to use the toilet";
				}
			}

			public class BEDDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Beds";

				public static LocString TOOLTIP_NAME = "<b>Beds</b>";

				public static LocString NORMAL = "    • Colony has adequate bedding";

				public static LocString NOT_ENOUGH_BEDS = "    • One or more Duplicants are missing a bed";

				public static LocString MISSING_ASSIGNMENT = "    • One or more Duplicants don't have an assigned bed";

				public static class CRITERIA
				{
					public static LocString CHECKENOUGHBEDS = "Check enough beds";
				}
			}

			public class FOODDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Food";

				public static LocString TOOLTIP_NAME = "<b>Food</b>";

				public static LocString NORMAL = "    • Food supply is currently adequate";

				public static LocString LOW_CALORIES = "    • Food-to-Duplicant ratio is low";

				public static LocString HUNGRY = "    • One or more Duplicants are very hungry";

				public static LocString NO_FOOD = "    • Duplicants have no food";

				public class CRITERIA_HAS_FOOD
				{
					public static LocString PASS = "    • Duplicants have food";

					public static LocString FAIL = "    • Duplicants have no food";
				}

				public static class CRITERIA
				{
					public static LocString CHECKENOUGHFOOD = "Check enough food";

					public static LocString CHECKSTARVATION = "Check starvation";
				}
			}

			public class FARMDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Crops";

				public static LocString TOOLTIP_NAME = "<b>Crops</b>";

				public static LocString NORMAL = "    • Crops are being grown in sufficient quantity";

				public static LocString NONE = "    • No farm plots";

				public static LocString NONE_PLANTED = "    • No crops planted";

				public static LocString WILTING = "    • One or more crops are wilting";

				public static LocString INOPERATIONAL = "    • One or more farm plots are inoperable";

				public static class CRITERIA
				{
					public static LocString CHECKHASFARMS = "Check colony has farms";

					public static LocString CHECKPLANTED = "Check farms are planted";

					public static LocString CHECKWILTING = "Check crops wilting";

					public static LocString CHECKOPERATIONAL = "Check farm plots operational";
				}
			}

			public class POWERUSEDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Power use";

				public static LocString TOOLTIP_NAME = "<b>Power use</b>";

				public static LocString NORMAL = "    • Power supply is satisfactory";

				public static LocString OVERLOADED = "    • One or more power grids are damaged";

				public static LocString SIGNIFICANT_POWER_CHANGE_DETECTED = "Significant power use change detected. (Average:{0}, Current:{1})";

				public static LocString CIRCUIT_OVER_CAPACITY = "Circuit overloaded {0}/{1}";

				public static class CRITERIA
				{
					public static LocString CHECKOVERWATTAGE = "Check circuit overloaded";

					public static LocString CHECKPOWERUSECHANGE = "Check power use change";
				}
			}

			public class HEATDIAGNOSTIC
			{
				public static LocString ALL_NAME = UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.ALL_NAME;

				public static class CRITERIA
				{
					public static LocString CHECKHEAT = "Check heat";
				}
			}

			public class BATTERYDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Battery";

				public static LocString TOOLTIP_NAME = "<b>Battery</b>";

				public static LocString NORMAL = "    • All batteries functional";

				public static LocString NONE = "    • No batteries are connected to a power grid";

				public static LocString DEAD_BATTERY = "    • One or more batteries have died";

				public static LocString LIMITED_CAPACITY = "    • Low battery capacity relative to power use";

				public class CRITERIA_CHECK_CAPACITY
				{
					public static LocString PASS = "";

					public static LocString FAIL = "";
				}

				public static class CRITERIA
				{
					public static LocString CHECKCAPACITY = "Check capacity";

					public static LocString CHECKDEAD = "Check dead";
				}
			}

			public class RADIATIONDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Radiation";

				public static LocString TOOLTIP_NAME = "<b>Radiation</b>";

				public static LocString NORMAL = "    • No Radiation concerns";

				public static LocString AVERAGE_RADS = "Avg. {0}";

				public class CRITERIA_RADIATION_SICKNESS
				{
					public static LocString PASS = "Healthy";

					public static LocString FAIL = "Sick";
				}

				public class CRITERIA_RADIATION_EXPOSURE
				{
					public static LocString PASS = "Safe exposure levels";

					public static LocString FAIL_CONCERN = "Exposure levels are above safe limits for one or more Duplicants";

					public static LocString FAIL_WARNING = "One or more Duplicants are being exposed to extreme levels of radiation";
				}

				public static class CRITERIA
				{
					public static LocString CHECKSICK = "Check sick";

					public static LocString CHECKEXPOSED = "Check exposed";
				}
			}

			public class ENTOMBEDDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Entombed";

				public static LocString TOOLTIP_NAME = "<b>Entombed</b>";

				public static LocString NORMAL = "    • No buildings are entombed";

				public static LocString BUILDING_ENTOMBED = "    • One or more buildings are entombed";

				public static class CRITERIA
				{
					public static LocString CHECKENTOMBED = "Check entombed";
				}
			}

			public class ROCKETFUELDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Rocket Fuel";

				public static LocString TOOLTIP_NAME = "<b>Rocket Fuel</b>";

				public static LocString NORMAL = "    • This rocket has sufficient fuel";

				public static LocString WARNING = "    • This rocket has no fuel";

				public static class CRITERIA
				{
				}
			}

			public class ROCKETOXIDIZERDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Rocket Oxidizer";

				public static LocString TOOLTIP_NAME = "<b>Rocket Oxidizer</b>";

				public static LocString NORMAL = "    • This rocket has sufficient oxidizer";

				public static LocString WARNING = "    • This rocket has insufficient oxidizer";

				public static class CRITERIA
				{
				}
			}

			public class REACTORDIAGNOSTIC
			{
				public static LocString ALL_NAME = BUILDINGS.PREFABS.NUCLEARREACTOR.NAME;

				public static LocString TOOLTIP_NAME = BUILDINGS.PREFABS.NUCLEARREACTOR.NAME;

				public static LocString NORMAL = "    • Safe";

				public static LocString CRITERIA_TEMPERATURE_WARNING = "    • Temperature dangerously high";

				public static LocString CRITERIA_COOLANT_WARNING = "    • Coolant tank low";

				public static class CRITERIA
				{
					public static LocString CHECKTEMPERATURE = "Check temperature";

					public static LocString CHECKCOOLANT = "Check coolant";
				}
			}

			public class FLOATINGROCKETDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Flight Status";

				public static LocString TOOLTIP_NAME = "<b>Flight Status</b>";

				public static LocString NORMAL_FLIGHT = "    • This rocket is in flight towards its destination";

				public static LocString NORMAL_UTILITY = "    • This rocket is performing a task at its destination";

				public static LocString NORMAL_LANDED = "    • This rocket is currently landed on a " + UI.PRE_KEYWORD + "Rocket Platform" + UI.PST_KEYWORD;

				public static LocString WARNING_NO_DESTINATION = "    • This rocket is suspended in space with no set destination";

				public static LocString WARNING_NO_SPEED = "    • This rocket's flight has been halted";

				public static class CRITERIA
				{
				}
			}

			public class ROCKETINORBITDIAGNOSTIC
			{
				public static LocString ALL_NAME = "Rockets in Orbit";

				public static LocString TOOLTIP_NAME = "<b>Rockets in Orbit</b>";

				public static LocString NORMAL_ONE_IN_ORBIT = "    • {0} is in orbit waiting to land";

				public static LocString NORMAL_IN_ORBIT = "    • There are {0} rockets in orbit waiting to land";

				public static LocString WARNING_ONE_ROCKETS_STRANDED = "    • No " + BUILDINGS.PREFABS.LAUNCHPAD.NAME + " present. {0} stranded";

				public static LocString WARNING_ROCKETS_STRANDED = "    • No " + BUILDINGS.PREFABS.LAUNCHPAD.NAME + " present. {0} rockets stranded";

				public static LocString NORMAL_NO_ROCKETS = "    • No rockets waiting to land";

				public static class CRITERIA
				{
					public static LocString CHECKORBIT = "Check Orbiting Rockets";
				}
			}
		}

		public class TRACKERS
		{
			public static LocString BREATHABILITY = "Breathability";

			public static LocString FOOD = "Food";

			public static LocString STRESS = "Max Stress";

			public static LocString IDLE = "Idle Duplicants";
		}

		public class CONTROLS
		{
			public static LocString PRESS = "Press";

			public static LocString PRESSLOWER = "press";

			public static LocString PRESSUPPER = "PRESS";

			public static LocString PRESSING = "Pressing";

			public static LocString PRESSINGLOWER = "pressing";

			public static LocString PRESSINGUPPER = "PRESSING";

			public static LocString PRESSED = "Pressed";

			public static LocString PRESSEDLOWER = "pressed";

			public static LocString PRESSEDUPPER = "PRESSED";

			public static LocString PRESSES = "Presses";

			public static LocString PRESSESLOWER = "presses";

			public static LocString PRESSESUPPER = "PRESSES";

			public static LocString PRESSABLE = "Pressable";

			public static LocString PRESSABLELOWER = "pressable";

			public static LocString PRESSABLEUPPER = "PRESSABLE";

			public static LocString CLICK = "Click";

			public static LocString CLICKLOWER = "click";

			public static LocString CLICKUPPER = "CLICK";

			public static LocString CLICKING = "Clicking";

			public static LocString CLICKINGLOWER = "clicking";

			public static LocString CLICKINGUPPER = "CLICKING";

			public static LocString CLICKED = "Clicked";

			public static LocString CLICKEDLOWER = "clicked";

			public static LocString CLICKEDUPPER = "CLICKED";

			public static LocString CLICKS = "Clicks";

			public static LocString CLICKSLOWER = "clicks";

			public static LocString CLICKSUPPER = "CLICKS";

			public static LocString CLICKABLE = "Clickable";

			public static LocString CLICKABLELOWER = "clickable";

			public static LocString CLICKABLEUPPER = "CLICKABLE";
		}

		public class MATH_PICTURES
		{
			public class AXIS_LABELS
			{
				public static LocString CYCLES = "Cycles";
			}
		}

		public class SPACEDESTINATIONS
		{
			public class WORMHOLE
			{
				public static LocString NAME = "Temporal Tear";

				public static LocString DESCRIPTION = "The source of our misfortune, though it may also be our shot at freedom. Traces of Neutronium are detectable in my readings.";
			}

			public class RESEARCHDESTINATION
			{
				public static LocString NAME = "Alluring Anomaly";

				public static LocString DESCRIPTION = "Our researchers would have a field day with this if they could only get close enough.";
			}

			public class DEBRIS
			{
				public class SATELLITE
				{
					public static LocString NAME = "Satellite";

					public static LocString DESCRIPTION = "An artificial construct that has escaped its orbit. It no longer appears to be monitored.";
				}
			}

			public class NONE
			{
				public static LocString NAME = "Unselected";
			}

			public class ORBIT
			{
				public static LocString NAME_FMT = "Orbiting {Name}";
			}

			public class EMPTY_SPACE
			{
				public static LocString NAME = "Empty Space";
			}

			public class FOG_OF_WAR_SPACE
			{
				public static LocString NAME = "Unexplored Space";
			}

			public class ARTIFACT_POI
			{
				public class GRAVITASSPACESTATION1
				{
					public static LocString NAME = "Destroyed Satellite";

					public static LocString DESC = "The remnants of a bygone era, lost in time.\n\nHarvesting space junk requires a rocket equipped with a " + UI.FormatAsLink("Drillcone", "NOSECONEHARVEST") + ".";
				}

				public class GRAVITASSPACESTATION2
				{
					public static LocString NAME = "Demolished Rocket";

					public static LocString DESC = "A defunct rocket from a corporation that vanished long ago.\n\nHarvesting space junk requires a rocket equipped with a " + UI.FormatAsLink("Drillcone", "NOSECONEHARVEST") + ".";
				}

				public class GRAVITASSPACESTATION3
				{
					public static LocString NAME = "Ruined Rocket";

					public static LocString DESC = "The ruins of a rocket that stopped functioning ages ago.\n\nHarvesting space junk requires a rocket equipped with a " + UI.FormatAsLink("Drillcone", "NOSECONEHARVEST") + ".";
				}

				public class GRAVITASSPACESTATION4
				{
					public static LocString NAME = "Retired Planetary Excursion Module";

					public static LocString DESC = "A rocket part from a society that has been wiped out.\n\nHarvesting space junk requires a rocket equipped with a " + UI.FormatAsLink("Drillcone", "NOSECONEHARVEST") + ".";
				}

				public class GRAVITASSPACESTATION5
				{
					public static LocString NAME = "Destroyed Satellite";

					public static LocString DESC = "A destroyed Gravitas satellite.\n\nHarvesting space junk requires a rocket equipped with a " + UI.FormatAsLink("Drillcone", "NOSECONEHARVEST") + ".";
				}

				public class GRAVITASSPACESTATION6
				{
					public static LocString NAME = "Annihilated Satellite";

					public static LocString DESC = "The remains of a satellite made some time in the past.\n\nHarvesting space junk requires a rocket equipped with a " + UI.FormatAsLink("Drillcone", "NOSECONEHARVEST") + ".";
				}

				public class GRAVITASSPACESTATION7
				{
					public static LocString NAME = "Wrecked Space Shuttle";

					public static LocString DESC = "A defunct space shuttle that floats through space unattended.\n\nHarvesting space junk requires a rocket equipped with a " + UI.FormatAsLink("Drillcone", "NOSECONEHARVEST") + ".";
				}

				public class GRAVITASSPACESTATION8
				{
					public static LocString NAME = "Obsolete Space Station Module";

					public static LocString DESC = "The module from a space station that ceased to exist ages ago.\n\nHarvesting space junk requires a rocket equipped with a " + UI.FormatAsLink("Drillcone", "NOSECONEHARVEST") + ".";
				}

				public class RUSSELLSTEAPOT
				{
					public static LocString NAME = "Russell's Teapot";

					public static LocString DESC = "Has never been disproven to not exist.";
				}
			}

			public class HARVESTABLE_POI
			{
				public static LocString POI_PRODUCTION = "{0}";

				public static LocString POI_PRODUCTION_TOOLTIP = "{0}";

				public class CARBONASTEROIDFIELD
				{
					public static LocString NAME = "Carbon Asteroid Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"An asteroid containing ",
						UI.FormatAsLink("Refined Carbon", "REFINEDCARBON"),
						" and ",
						UI.FormatAsLink("Coal", "CARBON"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class METALLICASTEROIDFIELD
				{
					public static LocString NAME = "Metallic Asteroid Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"An asteroid field containing ",
						UI.FormatAsLink("Iron", "IRON"),
						", ",
						UI.FormatAsLink("Copper", "COPPER"),
						" and ",
						UI.FormatAsLink("Obsidian", "OBSIDIAN"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class SATELLITEFIELD
				{
					public static LocString NAME = "Space Debris";

					public static LocString DESC = "Space junk from a forgotten age.\n\nHarvesting resources requires a rocket equipped with a " + UI.FormatAsLink("Drillcone", "NOSECONEHARVEST") + ".";
				}

				public class ROCKYASTEROIDFIELD
				{
					public static LocString NAME = "Rocky Asteroid Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"An asteroid field containing ",
						UI.FormatAsLink("Copper Ore", "CUPRITE"),
						", ",
						UI.FormatAsLink("Sedimentary Rock", "SEDIMENTARYROCK"),
						" and ",
						UI.FormatAsLink("Igneous Rock", "IGNEOUSROCK"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class INTERSTELLARICEFIELD
				{
					public static LocString NAME = "Ice Asteroid Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"An asteroid field containing ",
						UI.FormatAsLink("Ice", "ICE"),
						", ",
						UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
						" and ",
						UI.FormatAsLink("Oxygen", "OXYGEN"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class ORGANICMASSFIELD
				{
					public static LocString NAME = "Organic Mass Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"A mass of harvestable resources containing ",
						UI.FormatAsLink("Algae", "ALGAE"),
						", ",
						UI.FormatAsLink("Slime", "SLIMEMOLD"),
						", ",
						UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN"),
						" and ",
						UI.FormatAsLink("Dirt", "DIRT"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class ICEASTEROIDFIELD
				{
					public static LocString NAME = "Exploded Ice Giant";

					public static LocString DESC = string.Concat(new string[]
					{
						"A cloud of planetary remains containing ",
						UI.FormatAsLink("Ice", "ICE"),
						", ",
						UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
						", ",
						UI.FormatAsLink("Oxygen", "OXYGEN"),
						" and ",
						UI.FormatAsLink("Methane", "METHANE"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class GASGIANTCLOUD
				{
					public static LocString NAME = "Exploded Gas Giant";

					public static LocString DESC = string.Concat(new string[]
					{
						"The harvestable remains of a planet containing ",
						UI.FormatAsLink("Hydrogen", "HYDROGEN"),
						" in ",
						UI.FormatAsLink("gas", "ELEMENTS_GAS"),
						" form, and ",
						UI.FormatAsLink("Methane", "SOLIDMETHANE"),
						" in ",
						UI.FormatAsLink("solid", "ELEMENTS_SOLID"),
						" and ",
						UI.FormatAsLink("liquid", "ELEMENTS_LIQUID"),
						" form.\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class CHLORINECLOUD
				{
					public static LocString NAME = "Chlorine Cloud";

					public static LocString DESC = string.Concat(new string[]
					{
						"A cloud of harvestable debris containing ",
						UI.FormatAsLink("Chlorine", "CHLORINEGAS"),
						" and ",
						UI.FormatAsLink("Bleach Stone", "BLEACHSTONE"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class GILDEDASTEROIDFIELD
				{
					public static LocString NAME = "Gilded Asteroid Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"An asteroid field containing ",
						UI.FormatAsLink("Gold", "GOLD"),
						", ",
						UI.FormatAsLink("Fullerene", "FULLERENE"),
						", ",
						UI.FormatAsLink("Regolith", "REGOLITH"),
						" and more.\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class GLIMMERINGASTEROIDFIELD
				{
					public static LocString NAME = "Glimmering Asteroid Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"An asteroid field containing ",
						UI.FormatAsLink("Tungsten", "TUNGSTEN"),
						", ",
						UI.FormatAsLink("Wolframite", "WOLFRAMITE"),
						" and more.\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class HELIUMCLOUD
				{
					public static LocString NAME = "Helium Cloud";

					public static LocString DESC = string.Concat(new string[]
					{
						"A cloud of resources containing ",
						UI.FormatAsLink("Water", "WATER"),
						" and ",
						UI.FormatAsLink("Hydrogen", "HYDROGEN"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class OILYASTEROIDFIELD
				{
					public static LocString NAME = "Oily Asteroid Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"An asteroid field containing ",
						UI.FormatAsLink("Methane", "SOLIDMETHANE"),
						", ",
						UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
						" and ",
						UI.FormatAsLink("Crude Oil", "CRUDEOIL"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class OXIDIZEDASTEROIDFIELD
				{
					public static LocString NAME = "Oxidized Asteroid Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"An asteroid field containing ",
						UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
						" and ",
						UI.FormatAsLink("Rust", "RUST"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class SALTYASTEROIDFIELD
				{
					public static LocString NAME = "Salty Asteroid Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"A field of harvestable resources containing ",
						UI.FormatAsLink("Salt Water", "SALTWATER"),
						",",
						UI.FormatAsLink("Brine", "BRINE"),
						" and ",
						UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class FROZENOREFIELD
				{
					public static LocString NAME = "Frozen Ore Asteroid Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"An asteroid field containing ",
						UI.FormatAsLink("Polluted Ice", "DIRTYICE"),
						", ",
						UI.FormatAsLink("Ice", "ICE"),
						", ",
						UI.FormatAsLink("Snow", "SNOW"),
						" and ",
						UI.FormatAsLink("Aluminum Ore", "ALUMINUMORE"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class FORESTYOREFIELD
				{
					public static LocString NAME = "Forested Ore Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"A field of harvestable resources containing ",
						UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
						", ",
						UI.FormatAsLink("Igneous Rock", "IGNEOUSROCK"),
						" and ",
						UI.FormatAsLink("Aluminum Ore", "ALUMINUMORE"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class SWAMPYOREFIELD
				{
					public static LocString NAME = "Swampy Ore Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"An asteroid field containing ",
						UI.FormatAsLink("Mud", "MUD"),
						", ",
						UI.FormatAsLink("Polluted Dirt", "TOXICSAND"),
						" and ",
						UI.FormatAsLink("Cobalt Ore", "COBALTITE"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class SANDYOREFIELD
				{
					public static LocString NAME = "Sandy Ore Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"An asteroid field containing ",
						UI.FormatAsLink("Sandstone", "SANDSTONE"),
						", ",
						UI.FormatAsLink("Algae", "ALGAE"),
						", ",
						UI.FormatAsLink("Copper Ore", "CUPRITE"),
						" and ",
						UI.FormatAsLink("Sand", "SAND"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class RADIOACTIVEGASCLOUD
				{
					public static LocString NAME = "Radioactive Gas Cloud";

					public static LocString DESC = string.Concat(new string[]
					{
						"A cloud of resources containing ",
						UI.FormatAsLink("Chlorine", "CHLORINEGAS"),
						", ",
						UI.FormatAsLink("Uranium Ore", "URANIUMORE"),
						" and ",
						UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class RADIOACTIVEASTEROIDFIELD
				{
					public static LocString NAME = "Radioactive Asteroid Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"An asteroid field containing ",
						UI.FormatAsLink("Bleach Stone", "BLEACHSTONE"),
						", ",
						UI.FormatAsLink("Rust", "RUST"),
						", ",
						UI.FormatAsLink("Uranium Ore", "URANIUMORE"),
						" and ",
						UI.FormatAsLink("Sulfur", "SULFUR"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class OXYGENRICHASTEROIDFIELD
				{
					public static LocString NAME = "Oxygen Rich Asteroid Field";

					public static LocString DESC = string.Concat(new string[]
					{
						"An asteroid field containing ",
						UI.FormatAsLink("Ice", "ICE"),
						", ",
						UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN"),
						" and ",
						UI.FormatAsLink("Water", "WATER"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}

				public class INTERSTELLAROCEAN
				{
					public static LocString NAME = "Interstellar Ocean";

					public static LocString DESC = string.Concat(new string[]
					{
						"An interplanetary body that consists of ",
						UI.FormatAsLink("Salt Water", "SALTWATER"),
						", ",
						UI.FormatAsLink("Brine", "BRINE"),
						", ",
						UI.FormatAsLink("Salt", "SALT"),
						" and ",
						UI.FormatAsLink("Ice", "ICE"),
						".\n\nHarvesting resources requires a rocket equipped with a ",
						UI.FormatAsLink("Drillcone", "NOSECONEHARVEST"),
						"."
					});
				}
			}

			public class GRAVITAS_SPACE_POI
			{
				public static LocString STATION = "Destroyed Gravitas Space Station";
			}

			public class TELESCOPE_TARGET
			{
				public static LocString NAME = "Telescope Target";
			}

			public class ASTEROIDS
			{
				public class ROCKYASTEROID
				{
					public static LocString NAME = "Rocky Asteroid";

					public static LocString DESCRIPTION = "A minor mineral planet. Unlike a comet, it does not possess a tail.";
				}

				public class METALLICASTEROID
				{
					public static LocString NAME = "Metallic Asteroid";

					public static LocString DESCRIPTION = "A shimmering conglomerate of various metals.";
				}

				public class CARBONACEOUSASTEROID
				{
					public static LocString NAME = "Carbon Asteroid";

					public static LocString DESCRIPTION = "A common asteroid containing several useful resources.";
				}

				public class OILYASTEROID
				{
					public static LocString NAME = "Oily Asteroid";

					public static LocString DESCRIPTION = "A viscous asteroid that is only loosely held together. Contains fossil fuel resources.";
				}

				public class GOLDASTEROID
				{
					public static LocString NAME = "Gilded Asteroid";

					public static LocString DESCRIPTION = "A rich asteroid with thin gold coating and veins of gold deposits throughout.";
				}
			}

			public class CLUSTERMAPMETEORSHOWERS
			{
				public class UNIDENTIFIED
				{
					public static LocString NAME = "Unidentified Object";

					public static LocString DESCRIPTION = "A cosmic anomaly is traveling through the galaxy.\n\nIts origins and purpose are currently unknown, though a " + BUILDINGS.PREFABS.CLUSTERTELESCOPE.NAME + " could change that.";
				}

				public class SLIME
				{
					public static LocString NAME = "Slimy Meteor Shower";

					public static LocString DESCRIPTION = "A shower of slimy, biodynamic meteors on a collision course with the surface of an asteroid.";
				}

				public class SNOW
				{
					public static LocString NAME = "Blizzard Meteor Shower";

					public static LocString DESCRIPTION = "A shower of cold, cold meteors on a collision course with the surface of an asteroid.";
				}

				public class ICE
				{
					public static LocString NAME = "Ice Meteor Shower";

					public static LocString DESCRIPTION = "A hailstorm of icy space rocks on a collision course with the surface of an asteroid.";
				}

				public class COPPER
				{
					public static LocString NAME = "Copper Meteor Shower";

					public static LocString DESCRIPTION = "A shower of metallic meteors on a collision course with the surface of an asteroid.";
				}

				public class IRON
				{
					public static LocString NAME = "Iron Meteor Shower";

					public static LocString DESCRIPTION = "A shower of metallic space rocks on a collision course with the surface of an asteroid.";
				}

				public class GOLD
				{
					public static LocString NAME = "Gold Meteor Shower";

					public static LocString DESCRIPTION = "A shower of shiny metallic space rocks on a collision course with the surface of an asteroid.";
				}

				public class URANIUM
				{
					public static LocString NAME = "Uranium Meteor Shower";

					public static LocString DESCRIPTION = "A toxic shower of radioactive meteors on a collision course with the surface of an asteroid.";
				}

				public class LIGHTDUST
				{
					public static LocString NAME = "Dust Fluff Meteor Shower";

					public static LocString DESCRIPTION = "A cloud-like shower of dust fluff meteors heading towards the surface of an asteroid.";
				}

				public class HEAVYDUST
				{
					public static LocString NAME = "Dense Dust Meteor Shower";

					public static LocString DESCRIPTION = "A dark cloud of heavy dust meteors heading towards the surface of an asteroid.";
				}

				public class REGOLITH
				{
					public static LocString NAME = "Regolith Meteor Shower";

					public static LocString DESCRIPTION = "A shower of rocky meteors on a collision course with the surface of an asteroid.";
				}

				public class OXYLITE
				{
					public static LocString NAME = "Oxylite Meteor Shower";

					public static LocString DESCRIPTION = "A shower of rocky, oxygen-rich meteors on a collision course with the surface of an asteroid.";
				}

				public class BLEACHSTONE
				{
					public static LocString NAME = "Bleach Stone Meteor Shower";

					public static LocString DESCRIPTION = "A shower of bleach stone meteors on a collision course with the surface of an asteroid.";
				}

				public class MOO
				{
					public static LocString NAME = "Gassy Mooteor Shower";

					public static LocString DESCRIPTION = "A herd of methane-infused meteors that cause a bit of a stink, but do no actual damage.";
				}
			}

			public class COMETS
			{
				public class ROCKCOMET
				{
					public static LocString NAME = "Rock Meteor";
				}

				public class DUSTCOMET
				{
					public static LocString NAME = "Dust Meteor";
				}

				public class IRONCOMET
				{
					public static LocString NAME = "Iron Meteor";
				}

				public class COPPERCOMET
				{
					public static LocString NAME = "Copper Meteor";
				}

				public class GOLDCOMET
				{
					public static LocString NAME = "Gold Meteor";
				}

				public class FULLERENECOMET
				{
					public static LocString NAME = "Fullerene Meteor";
				}

				public class URANIUMORECOMET
				{
					public static LocString NAME = "Uranium Meteor";
				}

				public class NUCLEAR_WASTE
				{
					public static LocString NAME = "Radioactive Meteor";
				}

				public class SATELLITE
				{
					public static LocString NAME = "Defunct Satellite";
				}

				public class FOODCOMET
				{
					public static LocString NAME = "Snack Bomb";
				}

				public class GASSYMOOCOMET
				{
					public static LocString NAME = "Gassy Mooteor";
				}

				public class SLIMECOMET
				{
					public static LocString NAME = "Slime Meteor";
				}

				public class SNOWBALLCOMET
				{
					public static LocString NAME = "Snow Meteor";
				}

				public class HARDICECOMET
				{
					public static LocString NAME = "Ice Meteor";
				}

				public class LIGHTDUSTCOMET
				{
					public static LocString NAME = "Dust Fluff Meteor";
				}

				public class ALGAECOMET
				{
					public static LocString NAME = "Algae Meteor";
				}

				public class PHOSPHORICCOMET
				{
					public static LocString NAME = "Phosphoric Meteor";
				}

				public class OXYLITECOMET
				{
					public static LocString NAME = "Oxylite Meteor";
				}

				public class BLEACHSTONECOMET
				{
					public static LocString NAME = "Bleach Stone Meteor";
				}
			}

			public class DWARFPLANETS
			{
				public class ICYDWARF
				{
					public static LocString NAME = "Interstellar Ice";

					public static LocString DESCRIPTION = "A terrestrial destination, frozen completely solid.";
				}

				public class ORGANICDWARF
				{
					public static LocString NAME = "Organic Mass";

					public static LocString DESCRIPTION = "A mass of organic material similar to the ooze used to print Duplicants. This sample is heavily degraded.";
				}

				public class DUSTYDWARF
				{
					public static LocString NAME = "Dusty Dwarf";

					public static LocString DESCRIPTION = "A loosely held together composite of minerals.";
				}

				public class SALTDWARF
				{
					public static LocString NAME = "Salty Dwarf";

					public static LocString DESCRIPTION = "A dwarf planet with unusually high sodium concentrations.";
				}

				public class REDDWARF
				{
					public static LocString NAME = "Red Dwarf";

					public static LocString DESCRIPTION = "An M-class star orbited by clusters of extractable aluminum and methane.";
				}
			}

			public class PLANETS
			{
				public class TERRAPLANET
				{
					public static LocString NAME = "Terrestrial Planet";

					public static LocString DESCRIPTION = "A planet with a walkable surface, though it does not possess the resources to sustain long-term life.";
				}

				public class VOLCANOPLANET
				{
					public static LocString NAME = "Volcanic Planet";

					public static LocString DESCRIPTION = "A large terrestrial object composed mainly of molten rock.";
				}

				public class SHATTEREDPLANET
				{
					public static LocString NAME = "Shattered Planet";

					public static LocString DESCRIPTION = "A once-habitable planet that has sustained massive damage.\n\nA powerful containment field prevents our rockets from traveling to its surface.";
				}

				public class RUSTPLANET
				{
					public static LocString NAME = "Oxidized Asteroid";

					public static LocString DESCRIPTION = "A small planet covered in large swathes of brown rust.";
				}

				public class FORESTPLANET
				{
					public static LocString NAME = "Living Planet";

					public static LocString DESCRIPTION = "A small green planet displaying several markers of primitive life.";
				}

				public class SHINYPLANET
				{
					public static LocString NAME = "Glimmering Planet";

					public static LocString DESCRIPTION = "A planet composed of rare, shimmering minerals. From the distance, it looks like gem in the sky.";
				}

				public class CHLORINEPLANET
				{
					public static LocString NAME = "Chlorine Planet";

					public static LocString DESCRIPTION = "A noxious planet permeated by unbreathable chlorine.";
				}

				public class SALTDESERTPLANET
				{
					public static LocString NAME = "Arid Planet";

					public static LocString DESCRIPTION = "A sweltering, desert-like planet covered in surface salt deposits.";
				}
			}

			public class GIANTS
			{
				public class GASGIANT
				{
					public static LocString NAME = "Gas Giant";

					public static LocString DESCRIPTION = "A massive volume of " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + " formed around a small solid center.";
				}

				public class ICEGIANT
				{
					public static LocString NAME = "Ice Giant";

					public static LocString DESCRIPTION = "A massive volume of frozen material, primarily composed of " + UI.FormatAsLink("Ice", "ICE") + ".";
				}

				public class HYDROGENGIANT
				{
					public static LocString NAME = "Helium Giant";

					public static LocString DESCRIPTION = "A massive volume of " + UI.FormatAsLink("Helium", "HELIUM") + " formed around a small solid center.";
				}
			}
		}

		public class SPACEARTIFACTS
		{
			public class ARTIFACTTIERS
			{
				public static LocString TIER_NONE = "Nothing";

				public static LocString TIER0 = "Rarity 0";

				public static LocString TIER1 = "Rarity 1";

				public static LocString TIER2 = "Rarity 2";

				public static LocString TIER3 = "Rarity 3";

				public static LocString TIER4 = "Rarity 4";

				public static LocString TIER5 = "Rarity 5";
			}

			public class PACUPERCOLATOR
			{
				public static LocString NAME = "Percolator";

				public static LocString DESCRIPTION = "Don't drink from it! There was a pacu... IN the percolator!";

				public static LocString ARTIFACT = "A coffee percolator with the remnants of a blend of coffee that was a personal favorite of Dr. Hassan Aydem.\n\nHe would specifically reserve the consumption of this particular blend for when he was reviewing research papers on Sunday afternoons.";
			}

			public class ROBOTARM
			{
				public static LocString NAME = "Robot Arm";

				public static LocString DESCRIPTION = "It's not functional. Just cool.";

				public static LocString ARTIFACT = "A commercially available robot arm that has had a significant amount of modifications made to it.\n\nThe initials B.A. appear on one of the fingers.";
			}

			public class HATCHFOSSIL
			{
				public static LocString NAME = "Pristine Fossil";

				public static LocString DESCRIPTION = "The preserved bones of an early species of Hatch.";

				public static LocString ARTIFACT = "The preservation of this skeleton occurred artificially using a technique called the \"The Ali Method\".\n\nIt should be noted that this fossilization technique was pioneered by one Dr. Ashkan Seyed Ali, an employee of Gravitas.";
			}

			public class MODERNART
			{
				public static LocString NAME = "Modern Art";

				public static LocString DESCRIPTION = "I don't get it.";

				public static LocString ARTIFACT = "A sculpture of the Neoplastism movement of Modern Art.\n\nGravitas records show that this piece was once used in a presentation called 'Form and Function in Corporate Aesthetic'.";
			}

			public class EGGROCK
			{
				public static LocString NAME = "Egg-Shaped Rock";

				public static LocString DESCRIPTION = "It's unclear whether this is its naturally occurring shape, or if its appearance as been sculpted.";

				public static LocString ARTIFACT = "The words \"Happy Farters Day Dad. Love Macy\" appear on the bottom of this rock, written in a childlish scrawl.";
			}

			public class RAINBOWEGGROCK
			{
				public static LocString NAME = "Egg-Shaped Rock";

				public static LocString DESCRIPTION = "It's unclear whether this is its naturally occurring shape, or if its appearance as been sculpted.\n\nThis one is rainbow colored.";

				public static LocString ARTIFACT = "The words \"Happy Father's Day, Dad. Love you!\" appear on the bottom of this rock, written in very neat handwriting. The words are surrounded by four hearts drawn in what appears to be a pink gel pen.";
			}

			public class OKAYXRAY
			{
				public static LocString NAME = "Old X-Ray";

				public static LocString DESCRIPTION = "Ew, weird. It has five fingers!";

				public static LocString ARTIFACT = "The description on this X-ray indicates that it was taken in the Gravitas Medical Facility.\n\nMost likely this X-ray was performed while investigating an injury that occurred within the facility.";
			}

			public class SHIELDGENERATOR
			{
				public static LocString NAME = "Shield Generator";

				public static LocString DESCRIPTION = "A mechanical prototype capable of producing a small section of shielding.";

				public static LocString ARTIFACT = "The energy field produced by this shield generator completely ignores those light behaviors which are wave-like and focuses instead on its particle behaviors.\n\nThis seemingly paradoxical state is possible when light is slowed down to the point at which it stops entirely.";
			}

			public class TEAPOT
			{
				public static LocString NAME = "Encrusted Teapot";

				public static LocString DESCRIPTION = "A teapot from the depths of space, coated in a thick layer of Neutronium.";

				public static LocString ARTIFACT = "The amount of Neutronium present in this teapot suggests that it has crossed the threshold of the spacetime continuum on countless occasions, floating through many multiple universes over a plethora of times and spaces.\n\nThough there are, theoretically, an infinite amount of outcomes to any one event over many multi-verses, the homogeneity of the still relatively young multiverse suggests that this is then not the only teapot which has crossed into multiple universes. Despite the infinite possible outcomes of infinite multiverses it appears one high probability constant is that there is, or once was, a teapot floating somewhere in space within every universe.";
			}

			public class DNAMODEL
			{
				public static LocString NAME = "Double Helix Model";

				public static LocString DESCRIPTION = "An educational model of genetic information.";

				public static LocString ARTIFACT = "A physical representation of the building blocks of life.\n\nThis one contains trace amounts of a Genetic Ooze prototype that was once used by Gravitas.";
			}

			public class SANDSTONE
			{
				public static LocString NAME = "Sandstone";

				public static LocString DESCRIPTION = "A beautiful rock composed of multiple layers of sediment.";

				public static LocString ARTIFACT = "This sample of sandstone appears to have been processed by the Gravitas Mining Gun that was made available to the general public.\n\nNote: The Gravitas public Mining Gun model is different than ones used by Duplicants in its larger size, and extra precautionary features added in order to be compliant with national safety standards.";
			}

			public class MAGMALAMP
			{
				public static LocString NAME = "Magma Lamp";

				public static LocString DESCRIPTION = "The sequel to \"Lava Lamp\".";

				public static LocString ARTIFACT = "Molten lava and obsidian combined in a way that allows the lava to maintain just enough heat to remain in liquid form.\n\nPlans of this lamp found in the Gravitas archives have been attributed to one Robin Nisbet, PhD.";
			}

			public class OBELISK
			{
				public static LocString NAME = "Small Obelisk";

				public static LocString DESCRIPTION = "A rectangular stone piece.\n\nIts function is unclear.";

				public static LocString ARTIFACT = "On close inspection this rectangle is actually a stone box built with a covert, almost seamless, lid, housing a tiny key.\n\nIt is still unclear what the key unlocks.";
			}

			public class RUBIKSCUBE
			{
				public static LocString NAME = "Rubik's Cube";

				public static LocString DESCRIPTION = "This mystery of the universe has already been solved.";

				public static LocString ARTIFACT = "A well-used, competition-compliant version of the popular puzzle cube.\n\nIt's worth noting that Dr. Dylan 'Nails' Winslow was once a regional Rubik's Cube champion.";
			}

			public class OFFICEMUG
			{
				public static LocString NAME = "Office Mug";

				public static LocString DESCRIPTION = "An intermediary place to store espresso before you move it to your mouth.";

				public static LocString ARTIFACT = "An office mug with the Gravitas logo on it. Though their office mugs were all emblazoned with the same logo, Gravitas colored their mugs differently to distinguish between their various departments.\n\nThis one is from the AI department.";
			}

			public class AMELIASWATCH
			{
				public static LocString NAME = "Wrist Watch";

				public static LocString DESCRIPTION = "It was discovered in a package labeled \"To be entrusted to Dr. Walker\".";

				public static LocString ARTIFACT = "This watch once belonged to pioneering aviator Amelia Earhart and travelled to space via astronaut Dr. Shannon Walker.\n\nHow it came to be floating in space is a matter of speculation, but perhaps the adventurous spirit of its original stewards became infused within the fabric of this timepiece and compelled the universe to launch it into the great unknown.";
			}

			public class MOONMOONMOON
			{
				public static LocString NAME = "Moonmoonmoon";

				public static LocString DESCRIPTION = "A moon's moon's moon. It's very small.";

				public static LocString ARTIFACT = "In contrast to most moons, this object's glowing properties do not come from reflecting an external source of light, but rather from an internal glow of mysterious origin.\n\nThe glow of this object also grants an extraordinary amount of Decor bonus to nearby Duplicants, almost as if it was designed that way.";
			}

			public class BIOLUMINESCENTROCK
			{
				public static LocString NAME = "Bioluminescent Rock";

				public static LocString DESCRIPTION = "A thriving colony of tiny, microscopic organisms is responsible for giving it its bluish glow.";

				public static LocString ARTIFACT = "The microscopic organisms within this rock are of a unique variety whose genetic code shows many tell-tale signs of being genetically engineered within a lab.\n\nFurther analysis reveals they share 99.999% of their genetic code with Shine Bugs.";
			}

			public class PLASMALAMP
			{
				public static LocString NAME = "Plasma Lamp";

				public static LocString DESCRIPTION = "No space colony is complete without one.";

				public static LocString ARTIFACT = "The bottom of this lamp contains the words 'Property of the Atmospheric Sciences Department'.\n\nIt's worth noting that the Gravitas Atmospheric Sciences Department once simulated an experiment testing the feasibility of survival in an environment filled with noble gasses, similar to the ones contained within this device.";
			}

			public class MOLDAVITE
			{
				public static LocString NAME = "Moldavite";

				public static LocString DESCRIPTION = "A unique green stone formed from the impact of a meteorite.";

				public static LocString ARTIFACT = "This extremely rare, museum grade moldavite once sat on the desk of Dr. Ren Sato, but it was stolen by some unknown person.\n\nDr. Sato suspected the perpetrator was none other than Director Stern, but was never able to confirm this theory.";
			}

			public class BRICKPHONE
			{
				public static LocString NAME = "Strange Brick";

				public static LocString DESCRIPTION = "It still works.";

				public static LocString ARTIFACT = "This cordless phone once held a direct line to an unknown location in which strange distant voices can be heard but not understood, nor interacted with.\n\nThough Gravitas spent a lot of money and years of study dedicated to discovering its secret, the mystery was never solved.";
			}

			public class SOLARSYSTEM
			{
				public static LocString NAME = "Self-Contained System";

				public static LocString DESCRIPTION = "A marvel of the cosmos, inside this display is an entirely self-contained solar system.";

				public static LocString ARTIFACT = "This marvel of a device was built using parts from an old Tornado-in-a-Box science fair project.\n\nVery faint, faded letters are still visible on the display bottom that read 'Camille P. Grade 5'.";
			}

			public class SINK
			{
				public static LocString NAME = "Sink";

				public static LocString DESCRIPTION = "No collection is complete without it.";

				public static LocString ARTIFACT = "A small trace of encrusted soap on this sink strongly suggests it was installed in a personal bathroom, rather than a public one which would have used a soap dispenser.\n\nThe soap sliver is light blue and contains a manufactured blueberry fragrance.";
			}

			public class ROCKTORNADO
			{
				public static LocString NAME = "Tornado Rock";

				public static LocString DESCRIPTION = "It's unclear how it formed, although I'm glad it did.";

				public static LocString ARTIFACT = "Speculations about the origin of this rock include a paper written by one Harold P. Moreson, Ph.D. in which he theorized it could be a rare form of hollow geode which failed to form any crystals inside.\n\nThis paper appears in the Gravitas archives, and in all probability, was one of the factors in the hiring of Moreson into the Geology department of the company.";
			}

			public class BLENDER
			{
				public static LocString NAME = "Blender";

				public static LocString DESCRIPTION = "Equipment used to conduct experiments answering the age-old question, \"Could that blend\"?";

				public static LocString ARTIFACT = "Trace amounts of edible foodstuffs present in this blender indicate that it was probably used to emulsify the ingredients of a mush bar.\n\nIt is also very likely that it was employed at least once in the production of a peanut butter and banana smoothie.";
			}

			public class SAXOPHONE
			{
				public static LocString NAME = "Mangled Saxophone";

				public static LocString DESCRIPTION = "The name \"Pesquet\" is barely legible on the inside.";

				public static LocString ARTIFACT = "Though it is often remarked that \"in space, no one can hear you scream\", Thomas Pesquet proved the same cannot be said for the smooth jazzy sounds of a saxophone.\n\nAlthough this instrument once belonged to the eminent French Astronaut its current bumped and bent shape suggests it has seen many adventures beyond that of just being used to perform an out-of-this-world saxophone solo.";
			}

			public class STETHOSCOPE
			{
				public static LocString NAME = "Stethoscope";

				public static LocString DESCRIPTION = "Listens to Duplicant heartbeats, or gurgly tummies.";

				public static LocString ARTIFACT = "The size and shape of this stethescope suggests it was not intended to be used by neither a human-sized nor a Duplicant-sized person but something half-way in between the two beings.";
			}

			public class VHS
			{
				public static LocString NAME = "Archaic Tech";

				public static LocString DESCRIPTION = "Be kind when you handle it. It's very fragile.";

				public static LocString ARTIFACT = "The label on this VHS tape reads \"Jackie and Olivia's House Warming Party\".\n\nUnfortunately, a device with which to play this recording no longer exists in this universe.";
			}

			public class REACTORMODEL
			{
				public static LocString NAME = "Model Nuclear Power Plant";

				public static LocString DESCRIPTION = "It's pronounced nu-clear.";

				public static LocString ARTIFACT = "Though this Nuclear Power Plant was never built, this model exists as an artifact to a time early in the life of Gravitas when it was researching all alternatives to solving the global energy problem.\n\nUltimately, the idea of building a Nuclear Power Plant was abandoned in favor of the \"much safer\" alternative of developing the Temporal Bow.";
			}

			public class MOODRING
			{
				public static LocString NAME = "Radiation Mood Ring";

				public static LocString DESCRIPTION = "How radioactive are you feeling?";

				public static LocString ARTIFACT = "A wholly unique ring not found anywhere outside of the Gravitas Laboratory.\n\nThough it can't be determined for sure who worked on this extraordinary curiousity it's worth noting that, for his Ph.D. thesis, Dr. Travaldo Farrington wrote a paper entitled \"Novelty Uses for Radiochromatic Dyes\".";
			}

			public class ORACLE
			{
				public static LocString NAME = "Useless Machine";

				public static LocString DESCRIPTION = "What does it do?";

				public static LocString ARTIFACT = "All of the parts for this contraption are recycled from projects abandoned by the Robotics department.\n\nThe design is very close to one published in an amateur DIY magazine that once sat in the lobby of the 'Employees Only' area of Gravitas' facilities.";
			}

			public class GRUBSTATUE
			{
				public static LocString NAME = "Grubgrub Statue";

				public static LocString DESCRIPTION = "A moving tribute to a tiny plant hugger.";

				public static LocString ARTIFACT = "It's very likely this statue was placed in a hidden, secluded place in the Gravitas laboratory since the creation of Grubgrubs was a closely held secret that the general public was not privy to.\n\nThis is a shame since the artistic quality of this statue is really quite accomplished.";
			}

			public class HONEYJAR
			{
				public static LocString NAME = "Honey Jar";

				public static LocString DESCRIPTION = "Sweet golden liquid with just a touch of uranium.";

				public static LocString ARTIFACT = "Records from the Genetics and Biology Lab of the Gravitas facility show that several early iterations of a radioactive Bee would continue to produce honey and that this honey was once accidentally stored in the employee kitchen which resulted in several incidents of minor radiation poisoning when it was erroneously labled as a sweetener for tea.\n\nEmployees who used this product reported that it was the \"sweetest honey they'd ever tasted\" and expressed no regret at the mix-up.";
			}
		}

		public class KEEPSAKES
		{
			public class CRITTER_MANIPULATOR
			{
				public static LocString NAME = "Ceramic Morb";

				public static LocString DESCRIPTION = "A pottery project produced in an HR-mandated art therapy class.\n\nIt's glazed with a substance that once landed a curious lab technician in the ER.";
			}

			public class MEGA_BRAIN
			{
				public static LocString NAME = "Model Plane";

				public static LocString DESCRIPTION = "A treasured souvenir that was once a common accompaniment to children's meals during commercial flights. There's a hole in the bottom from when Dr. Holland had it mounted on a stand.";
			}

			public class LONELY_MINION
			{
				public static LocString NAME = "Rusty Toolbox";

				public static LocString DESCRIPTION = "On the inside of the lid, someone used a screwdriver to carve a drawing of a group of smiling Duplicants gathered around a massive crater.";
			}

			public class FOSSIL_HUNT
			{
				public static LocString NAME = "Critter Collar";

				public static LocString DESCRIPTION = "The tag reads \"Molly\".\n\nOn the reverse is \"Designed by B363\" stamped above what appears to be an unusually shaped pawprint.";
			}
		}

		public class SANDBOXTOOLS
		{
			public class SETTINGS
			{
				public class INSTANT_BUILD
				{
					public static LocString NAME = "Instant build mode ON";

					public static LocString TOOLTIP = "Toggle between placing construction plans and fully built buildings";
				}

				public class BRUSH_SIZE
				{
					public static LocString NAME = "Size";

					public static LocString TOOLTIP = "Adjust brush size";
				}

				public class BRUSH_NOISE_SCALE
				{
					public static LocString NAME = "Noise A";

					public static LocString TOOLTIP = "Adjust brush noisiness A";
				}

				public class BRUSH_NOISE_DENSITY
				{
					public static LocString NAME = "Noise B";

					public static LocString TOOLTIP = "Adjust brush noisiness B";
				}

				public class TEMPERATURE
				{
					public static LocString NAME = "Temperature";

					public static LocString TOOLTIP = "Adjust absolute temperature";
				}

				public class TEMPERATURE_ADDITIVE
				{
					public static LocString NAME = "Temperature";

					public static LocString TOOLTIP = "Adjust additive temperature";
				}

				public class RADIATION
				{
					public static LocString NAME = "Absolute radiation";

					public static LocString TOOLTIP = "Adjust absolute radiation";
				}

				public class RADIATION_ADDITIVE
				{
					public static LocString NAME = "Additive radiation";

					public static LocString TOOLTIP = "Adjust additive radiation";
				}

				public class STRESS_ADDITIVE
				{
					public static LocString NAME = "Reduce Stress";

					public static LocString TOOLTIP = "Adjust stress reduction";
				}

				public class MORALE
				{
					public static LocString NAME = "Adjust Morale";

					public static LocString TOOLTIP = "Bonus Morale adjustment";
				}

				public class MASS
				{
					public static LocString NAME = "Mass";

					public static LocString TOOLTIP = "Adjust mass";
				}

				public class DISEASE
				{
					public static LocString NAME = "Germ";

					public static LocString TOOLTIP = "Adjust type of germ";
				}

				public class DISEASE_COUNT
				{
					public static LocString NAME = "Germs";

					public static LocString TOOLTIP = "Adjust germ count";
				}

				public class BRUSH
				{
					public static LocString NAME = "Brush";

					public static LocString TOOLTIP = "Paint elements into the world simulation {Hotkey}";
				}

				public class ELEMENT
				{
					public static LocString NAME = "Element";

					public static LocString TOOLTIP = "Adjust type of element";
				}

				public class SPRINKLE
				{
					public static LocString NAME = "Sprinkle";

					public static LocString TOOLTIP = "Paint elements into the simulation using noise {Hotkey}";
				}

				public class FLOOD
				{
					public static LocString NAME = "Fill";

					public static LocString TOOLTIP = "Fill a section of the simulation with the chosen element {Hotkey}";
				}

				public class SAMPLE
				{
					public static LocString NAME = "Sample";

					public static LocString TOOLTIP = "Copy the settings from a cell to use with brush tools {Hotkey}";
				}

				public class HEATGUN
				{
					public static LocString NAME = "Heat Gun";

					public static LocString TOOLTIP = "Inject thermal energy into the simulation {Hotkey}";
				}

				public class RADSTOOL
				{
					public static LocString NAME = "Radiation Tool";

					public static LocString TOOLTIP = "Inject or remove radiation from the simulation {Hotkey}";
				}

				public class SPAWNER
				{
					public static LocString NAME = "Spawner";

					public static LocString TOOLTIP = "Spawn critters, food, equipment, and other entities {Hotkey}";
				}

				public class STRESS
				{
					public static LocString NAME = "Stress";

					public static LocString TOOLTIP = "Manage Duplicants' stress levels {Hotkey}";
				}

				public class CLEAR_FLOOR
				{
					public static LocString NAME = "Clear Debris";

					public static LocString TOOLTIP = "Delete loose items cluttering the floor {Hotkey}";
				}

				public class DESTROY
				{
					public static LocString NAME = "Destroy";

					public static LocString TOOLTIP = "Delete everything in the selected cell(s) {Hotkey}";
				}

				public class SPAWN_ENTITY
				{
					public static LocString NAME = "Spawn";
				}

				public class FOW
				{
					public static LocString NAME = "Reveal";

					public static LocString TOOLTIP = "Dispel the Fog of War shrouding the map {Hotkey}";
				}

				public class CRITTER
				{
					public static LocString NAME = "Critter Removal";

					public static LocString TOOLTIP = "Remove Critters! {Hotkey}";
				}
			}

			public class FILTERS
			{
				public static LocString BACK = "Back";

				public static LocString COMMON = "Common Substances";

				public static LocString SOLID = "Solids";

				public static LocString LIQUID = "Liquids";

				public static LocString GAS = "Gases";

				public class ENTITIES
				{
					public static LocString SPECIAL = "Special";

					public static LocString GRAVITAS = "Gravitas";

					public static LocString PLANTS = "Plants";

					public static LocString SEEDS = "Seeds";

					public static LocString CREATURE = "Critters";

					public static LocString CREATURE_EGG = "Eggs";

					public static LocString FOOD = "Foods";

					public static LocString EQUIPMENT = "Equipment";

					public static LocString GEYSERS = "Geysers";

					public static LocString EXPERIMENTS = "Experimental";

					public static LocString INDUSTRIAL_PRODUCTS = "Industrial";

					public static LocString COMETS = "Meteors";

					public static LocString ARTIFACTS = "Artifacts";

					public static LocString STORYTRAITS = "Story Traits";
				}
			}

			public class CLEARFLOOR
			{
				public static LocString DELETED = "Deleted";
			}
		}

		public class RETIRED_COLONY_INFO_SCREEN
		{
			public static LocString SECONDS = "Seconds";

			public static LocString CYCLES = "Cycles";

			public static LocString CYCLE_COUNT = "Cycle Count: {0}";

			public static LocString DUPLICANT_AGE = "Age: {0} cycles";

			public static LocString SKILL_LEVEL = "Skill Level: {0}";

			public static LocString BUILDING_COUNT = "Count: {0}";

			public static LocString PREVIEW_UNAVAILABLE = "Preview\nUnavailable";

			public static LocString TIMELAPSE_UNAVAILABLE = "Timelapse\nUnavailable";

			public static LocString SEARCH = "SEARCH...";

			public class BUTTONS
			{
				public static LocString RETURN_TO_GAME = "RETURN TO GAME";

				public static LocString VIEW_OTHER_COLONIES = "BACK";

				public static LocString QUIT_TO_MENU = "QUIT TO MAIN MENU";

				public static LocString CLOSE = "CLOSE";
			}

			public class TITLES
			{
				public static LocString EXPLORER_HEADER = "COLONIES";

				public static LocString RETIRED_COLONIES = "Colony Summaries";

				public static LocString COLONY_STATISTICS = "Colony Statistics";

				public static LocString DUPLICANTS = "Duplicants";

				public static LocString BUILDINGS = "Buildings";

				public static LocString CHEEVOS = "Colony Achievements";

				public static LocString ACHIEVEMENT_HEADER = "ACHIEVEMENTS";

				public static LocString TIMELAPSE = "Timelapse";
			}

			public class STATS
			{
				public static LocString OXYGEN_CREATED = "Total Oxygen Produced";

				public static LocString OXYGEN_CONSUMED = "Total Oxygen Consumed";

				public static LocString POWER_CREATED = "Average Power Produced";

				public static LocString POWER_WASTED = "Average Power Wasted";

				public static LocString TRAVEL_TIME = "Total Travel Time";

				public static LocString WORK_TIME = "Total Work Time";

				public static LocString AVERAGE_TRAVEL_TIME = "Average Travel Time";

				public static LocString AVERAGE_WORK_TIME = "Average Work Time";

				public static LocString CALORIES_CREATED = "Calorie Generation";

				public static LocString CALORIES_CONSUMED = "Calorie Consumption";

				public static LocString LIVE_DUPLICANTS = "Duplicants";

				public static LocString AVERAGE_STRESS_CREATED = "Average Stress Created";

				public static LocString AVERAGE_STRESS_REMOVED = "Average Stress Removed";

				public static LocString NUMBER_DOMESTICATED_CRITTERS = "Domesticated Critters";

				public static LocString NUMBER_WILD_CRITTERS = "Wild Critters";

				public static LocString AVERAGE_GERMS = "Average Germs";

				public static LocString ROCKET_MISSIONS = "Rocket Missions Underway";
			}
		}

		public class DROPDOWN
		{
			public static LocString NONE = "Unassigned";
		}

		public class FRONTEND
		{
			public static LocString GAME_VERSION = "Game Version: ";

			public static LocString LOADING = "Loading...";

			public static LocString DONE_BUTTON = "DONE";

			public class DEMO_OVER_SCREEN
			{
				public static LocString TITLE = "Thanks for playing!";

				public static LocString BODY = "Thank you for playing the demo for Oxygen Not Included!\n\nThis game is still in development.\n\nGo to kleigames.com/o2 or ask one of us if you'd like more information.";

				public static LocString BUTTON_EXIT_TO_MENU = "EXIT TO MENU";
			}

			public class CUSTOMGAMESETTINGSSCREEN
			{
				public class SETTINGS
				{
					public class SANDBOXMODE
					{
						public static LocString NAME = "Sandbox Mode";

						public static LocString TOOLTIP = "Manipulate and customize the simulation with tools that ignore regular game constraints";

						public static class LEVELS
						{
							public static class DISABLED
							{
								public static LocString NAME = "Disabled";

								public static LocString TOOLTIP = "Unchecked: Sandbox Mode is turned off (Default)";
							}

							public static class ENABLED
							{
								public static LocString NAME = "Enabled";

								public static LocString TOOLTIP = "Checked: Sandbox Mode is turned on";
							}
						}
					}

					public class FASTWORKERSMODE
					{
						public static LocString NAME = "Fast Workers Mode";

						public static LocString TOOLTIP = "Dupes will finish most work immediately and require little sleep";

						public static class LEVELS
						{
							public static class DISABLED
							{
								public static LocString NAME = "Disabled";

								public static LocString TOOLTIP = "Unchecked: Fast Workers Mode is turned off (Default)";
							}

							public static class ENABLED
							{
								public static LocString NAME = "Enabled";

								public static LocString TOOLTIP = "Checked: Fast Workers Mode is turned on";
							}
						}
					}

					public class EXPANSION1ACTIVE
					{
						public static LocString NAME = UI.DLC1.NAME_ITAL + " Content Enabled";

						public static LocString TOOLTIP = "If checked, content from the " + UI.DLC1.NAME_ITAL + " Expansion will be available";

						public static class LEVELS
						{
							public static class DISABLED
							{
								public static LocString NAME = "Disabled";

								public static LocString TOOLTIP = "Unchecked: " + UI.DLC1.NAME_ITAL + " Content is turned off (Default)";
							}

							public static class ENABLED
							{
								public static LocString NAME = "Enabled";

								public static LocString TOOLTIP = "Checked: " + UI.DLC1.NAME_ITAL + " Content is turned on";
							}
						}
					}

					public class SAVETOCLOUD
					{
						public static LocString NAME = "Save To Cloud";

						public static LocString TOOLTIP = "This colony will be created in the cloud saves folder, and synced by the game platform.";

						public static LocString TOOLTIP_LOCAL = "This colony will be created in the local saves folder. It will not be a cloud save and will not be synced by the game platform.";

						public static LocString TOOLTIP_EXTRA = "This can be changed later with the colony management options in the load screen, from the main menu.";

						public static class LEVELS
						{
							public static class DISABLED
							{
								public static LocString NAME = "Disabled";

								public static LocString TOOLTIP = "Unchecked: This colony will be a local save";
							}

							public static class ENABLED
							{
								public static LocString NAME = "Enabled";

								public static LocString TOOLTIP = "Checked: This colony will be a cloud save (Default)";
							}
						}
					}

					public class CAREPACKAGES
					{
						public static LocString NAME = "Care Packages";

						public static LocString TOOLTIP = "Affects what resources can be printed from the Printing Pod";

						public static class LEVELS
						{
							public static class NORMAL
							{
								public static LocString NAME = "All";

								public static LocString TOOLTIP = "Checked: The Printing Pod will offer both Duplicant blueprints and care packages (Default)";
							}

							public static class DUPLICANTS_ONLY
							{
								public static LocString NAME = "Duplicants Only";

								public static LocString TOOLTIP = "Unchecked: The Printing Pod will only offer Duplicant blueprints";
							}
						}
					}

					public class IMMUNESYSTEM
					{
						public static LocString NAME = "Disease";

						public static LocString TOOLTIP = "Affects Duplicants' chances of contracting a disease after germ exposure";

						public static class LEVELS
						{
							public static class COMPROMISED
							{
								public static LocString NAME = "Outbreak Prone";

								public static LocString TOOLTIP = "The whole colony will be ravaged by plague if a Duplicant so much as sneezes funny";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Outbreak Prone (Highest Difficulty)";
							}

							public static class WEAK
							{
								public static LocString NAME = "Germ Susceptible";

								public static LocString TOOLTIP = "These Duplicants have an increased chance of contracting diseases from germ exposure";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Germ Susceptibility (Difficulty Up)";
							}

							public static class DEFAULT
							{
								public static LocString NAME = "Default";

								public static LocString TOOLTIP = "Default disease chance";
							}

							public static class STRONG
							{
								public static LocString NAME = "Germ Resistant";

								public static LocString TOOLTIP = "These Duplicants have a decreased chance of contracting diseases from germ exposure";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Germ Resistance (Difficulty Down)";
							}

							public static class INVINCIBLE
							{
								public static LocString NAME = "Total Immunity";

								public static LocString TOOLTIP = "Like diplomatic immunity, but without the diplomacy. These Duplicants will never get sick";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Total Immunity (No Disease)";
							}
						}
					}

					public class MORALE
					{
						public static LocString NAME = "Morale";

						public static LocString TOOLTIP = "Adjusts the minimum morale Duplicants must maintain to avoid gaining stress";

						public static class LEVELS
						{
							public static class VERYHARD
							{
								public static LocString NAME = "Draconian";

								public static LocString TOOLTIP = "The finest of the finest can barely keep up with these Duplicants' stringent demands";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Draconian (Highest Difficulty)";
							}

							public static class HARD
							{
								public static LocString NAME = "A Bit Persnickety";

								public static LocString TOOLTIP = "Duplicants require higher morale than usual to fend off stress";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "A Bit Persnickety (Difficulty Up)";
							}

							public static class DEFAULT
							{
								public static LocString NAME = "Default";

								public static LocString TOOLTIP = "Default morale needs";
							}

							public static class EASY
							{
								public static LocString NAME = "Chill";

								public static LocString TOOLTIP = "Duplicants require lower morale than usual to fend off stress";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Chill (Difficulty Down)";
							}

							public static class DISABLED
							{
								public static LocString NAME = "Totally Blasé";

								public static LocString TOOLTIP = "These Duplicants have zero standards and will never gain stress, regardless of their morale";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Totally Blasé (No Morale)";
							}
						}
					}

					public class CALORIE_BURN
					{
						public static LocString NAME = "Hunger";

						public static LocString TOOLTIP = "Affects how quickly Duplicants burn calories and become hungry";

						public static class LEVELS
						{
							public static class VERYHARD
							{
								public static LocString NAME = "Ravenous";

								public static LocString TOOLTIP = "Your Duplicants are on a see-food diet... They see food and they eat it";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Ravenous (Highest Difficulty)";
							}

							public static class HARD
							{
								public static LocString NAME = "Rumbly Tummies";

								public static LocString TOOLTIP = "Duplicants burn calories quickly and require more feeding than usual";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Rumbly Tummies (Difficulty Up)";
							}

							public static class DEFAULT
							{
								public static LocString NAME = "Default";

								public static LocString TOOLTIP = "Default calorie burn rate";
							}

							public static class EASY
							{
								public static LocString NAME = "Fasting";

								public static LocString TOOLTIP = "Duplicants burn calories slowly and get by with fewer meals";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Fasting (Difficulty Down)";
							}

							public static class DISABLED
							{
								public static LocString NAME = "Tummyless";

								public static LocString TOOLTIP = "These Duplicants were printed without tummies and need no food at all";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Tummyless (No Hunger)";
							}
						}
					}

					public class WORLD_CHOICE
					{
						public static LocString NAME = "World";

						public static LocString TOOLTIP = "New worlds added by mods can be selected here";
					}

					public class CLUSTER_CHOICE
					{
						public static LocString NAME = "Asteroid Belt";

						public static LocString TOOLTIP = "New asteroid belts added by mods can be selected here";
					}

					public class STORY_TRAIT_COUNT
					{
						public static LocString NAME = "Story Traits";

						public static LocString TOOLTIP = "Determines the number of story traits spawned";

						public static class LEVELS
						{
							public static class NONE
							{
								public static LocString NAME = "Zilch";

								public static LocString TOOLTIP = "Zero story traits. Zip. Nada. None";
							}

							public static class FEW
							{
								public static LocString NAME = "Stingy";

								public static LocString TOOLTIP = "Not zero, but not a lot";
							}

							public static class LOTS
							{
								public static LocString NAME = "Oodles";

								public static LocString TOOLTIP = "Plenty of story traits to go around";
							}
						}
					}

					public class DURABILITY
					{
						public static LocString NAME = "Durability";

						public static LocString TOOLTIP = "Affects how quickly equippable suits wear out";

						public static class LEVELS
						{
							public static class INDESTRUCTIBLE
							{
								public static LocString NAME = "Indestructible";

								public static LocString TOOLTIP = "Duplicants have perfected clothes manufacturing and are able to make suits that last forever";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Indestructible Suits (No Durability)";
							}

							public static class REINFORCED
							{
								public static LocString NAME = "Reinforced";

								public static LocString TOOLTIP = "Suits are more durable than usual";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Reinforced Suits (Difficulty Down)";
							}

							public static class DEFAULT
							{
								public static LocString NAME = "Default";

								public static LocString TOOLTIP = "Default suit durability";
							}

							public static class FLIMSY
							{
								public static LocString NAME = "Flimsy";

								public static LocString TOOLTIP = "Suits wear out faster than usual";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Flimsy Suits (Difficulty Up)";
							}

							public static class THREADBARE
							{
								public static LocString NAME = "Threadbare";

								public static LocString TOOLTIP = "These Duplicants are no tailors - suits wear out much faster than usual";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Threadbare Suits (Highest Difficulty)";
							}
						}
					}

					public class RADIATION
					{
						public static LocString NAME = "Radiation";

						public static LocString TOOLTIP = "Affects how susceptible Duplicants are to radiation sickness";

						public static class LEVELS
						{
							public static class HARDEST
							{
								public static LocString NAME = "Critical Mass";

								public static LocString TOOLTIP = "Duplicants feel ill at the merest mention of radiation...and may never truly recover";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Super Radiation (Highest Difficulty)";
							}

							public static class HARDER
							{
								public static LocString NAME = "Toxic Positivity";

								public static LocString TOOLTIP = "Duplicants are more sensitive to radiation exposure than usual";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Radiation Vulnerable (Difficulty Up)";
							}

							public static class DEFAULT
							{
								public static LocString NAME = "Default";

								public static LocString TOOLTIP = "Default radiation settings";
							}

							public static class EASIER
							{
								public static LocString NAME = "Healthy Glow";

								public static LocString TOOLTIP = "Duplicants are more resistant to radiation exposure than usual";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Radiation Shielded (Difficulty Down)";
							}

							public static class EASIEST
							{
								public static LocString NAME = "Nuke-Proof";

								public static LocString TOOLTIP = "Duplicants could bathe in radioactive waste and not even notice";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Radiation Protection (Lowest Difficulty)";
							}
						}
					}

					public class STRESS
					{
						public static LocString NAME = "Stress";

						public static LocString TOOLTIP = "Affects how quickly Duplicant stress rises";

						public static class LEVELS
						{
							public static class INDOMITABLE
							{
								public static LocString NAME = "Cloud Nine";

								public static LocString TOOLTIP = "A strong emotional support system makes these Duplicants impervious to all stress";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Cloud Nine (No Stress)";
							}

							public static class OPTIMISTIC
							{
								public static LocString NAME = "Chipper";

								public static LocString TOOLTIP = "Duplicants gain stress slower than usual";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Chipper (Difficulty Down)";
							}

							public static class DEFAULT
							{
								public static LocString NAME = "Default";

								public static LocString TOOLTIP = "Default stress change rate";
							}

							public static class PESSIMISTIC
							{
								public static LocString NAME = "Glum";

								public static LocString TOOLTIP = "Duplicants gain stress more quickly than usual";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Glum (Difficulty Up)";
							}

							public static class DOOMED
							{
								public static LocString NAME = "Frankly Depressing";

								public static LocString TOOLTIP = "These Duplicants were never taught coping mechanisms... they're devastated by stress as a result";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Frankly Depressing (Highest Difficulty)";
							}
						}
					}

					public class STRESS_BREAKS
					{
						public static LocString NAME = "Stress Reactions";

						public static LocString TOOLTIP = "Determines whether Duplicants wreak havoc on the colony when they reach maximum stress";

						public static class LEVELS
						{
							public static class DEFAULT
							{
								public static LocString NAME = "Enabled";

								public static LocString TOOLTIP = "Checked: Duplicants will wreak havoc when they reach 100% stress (Default)";
							}

							public static class DISABLED
							{
								public static LocString NAME = "Disabled";

								public static LocString TOOLTIP = "Unchecked: Duplicants will not wreak havoc at maximum stress";
							}
						}
					}

					public class WORLDGEN_SEED
					{
						public static LocString NAME = "Worldgen Seed";

						public static LocString TOOLTIP = "This number chooses the procedural parameters that create your unique map\n\nWorldgen seeds can be copied and pasted so others can play a replica of your world configuration";

						public static LocString FIXEDSEED = "This is a predetermined seed, and cannot be changed";
					}

					public class TELEPORTERS
					{
						public static LocString NAME = "Teleporters";

						public static LocString TOOLTIP = "Determines whether teleporters will be spawned during Worldgen";

						public static class LEVELS
						{
							public static class ENABLED
							{
								public static LocString NAME = "Enabled";

								public static LocString TOOLTIP = "Checked: Teleporters will spawn during Worldgen (Default)";
							}

							public static class DISABLED
							{
								public static LocString NAME = "Disabled";

								public static LocString TOOLTIP = "Unchecked: No Teleporters will spawn during Worldgen";
							}
						}
					}

					public class METEORSHOWERS
					{
						public static LocString NAME = "Meteor Showers";

						public static LocString TOOLTIP = "Adjusts the intensity of incoming space rocks";

						public static class LEVELS
						{
							public static class CLEAR_SKIES
							{
								public static LocString NAME = "Clear Skies";

								public static LocString TOOLTIP = "No meteor damage, no worries";
							}

							public static class INFREQUENT
							{
								public static LocString NAME = "Spring Showers";

								public static LocString TOOLTIP = "Meteor showers are less frequent and less intense than usual";
							}

							public static class DEFAULT
							{
								public static LocString NAME = "Default";

								public static LocString TOOLTIP = "Default meteor shower frequency and intensity";
							}

							public static class INTENSE
							{
								public static LocString NAME = "Cosmic Storm";

								public static LocString TOOLTIP = "Meteor showers are more frequent and more intense than usual";
							}

							public static class DOOMED
							{
								public static LocString NAME = "Doomsday";

								public static LocString TOOLTIP = "An onslaught of apocalyptic hailstorms that feels almost personal";
							}
						}
					}
				}
			}

			public class MAINMENU
			{
				public static LocString STARTDEMO = "START DEMO";

				public static LocString NEWGAME = "NEW GAME";

				public static LocString RESUMEGAME = "RESUME GAME";

				public static LocString LOADGAME = "LOAD GAME";

				public static LocString RETIREDCOLONIES = "COLONY SUMMARIES";

				public static LocString KLEIINVENTORY = "KLEI INVENTORY";

				public static LocString LOCKERMENU = "SUPPLY CLOSET";

				public static LocString SCENARIOS = "SCENARIOS";

				public static LocString TRANSLATIONS = "TRANSLATIONS";

				public static LocString OPTIONS = "OPTIONS";

				public static LocString QUITTODESKTOP = "QUIT";

				public static LocString RESTARTCONFIRM = "Should I really quit?\nAll unsaved progress will be lost.";

				public static LocString QUITCONFIRM = "Should I quit to the main menu?\nAll unsaved progress will be lost.";

				public static LocString RETIRECONFIRM = "Should I surrender under the soul-crushing weight of this universe's entropy and retire my colony?";

				public static LocString DESKTOPQUITCONFIRM = "Should I really quit?\nAll unsaved progress will be lost.";

				public static LocString RESUMEBUTTON_BASENAME = "{0}: Cycle {1}";

				public class DLC
				{
					public static LocString ACTIVATE_EXPANSION1 = "ACTIVATE DLC";

					public static LocString ACTIVATE_EXPANSION1_DESC = "The game will need to restart in order to activate <i>Spaced Out!</i>";

					public static LocString ACTIVATE_EXPANSION1_RAIL_DESC = "<i>Spaced Out!</i> will be activated the next time you launch the game. The game will now close.";

					public static LocString DEACTIVATE_EXPANSION1 = "DEACTIVATE DLC";

					public static LocString DEACTIVATE_EXPANSION1_DESC = "The game will need to restart in order to activate the <i>Oxygen Not Included</i> base game.";

					public static LocString DEACTIVATE_EXPANSION1_RAIL_DESC = "<i>Spaced Out!</i> will be deactivated the next time you launch the game. The game will now close.";

					public static LocString AD_DLC1 = "Spaced Out! DLC";
				}
			}

			public class DEVTOOLS
			{
				public static LocString TITLE = "About Dev Tools";

				public static LocString WARNING = "DANGER!!\n\nDev Tools are intended for developer use only. Using them may result in your save becoming unplayable, unstable, or severely damaged.\n\nThese tools are completely unsupported and may contain bugs. Are you sure you want to continue?";

				public static LocString DONTSHOW = "Do not show this message again";

				public static LocString BUTTON = "Show Dev Tools";
			}

			public class NEWGAMESETTINGS
			{
				public static LocString HEADER = "GAME SETTINGS";

				public class BUTTONS
				{
					public static LocString STANDARDGAME = "Standard Game";

					public static LocString CUSTOMGAME = "Custom Game";

					public static LocString CANCEL = "Cancel";

					public static LocString STARTGAME = "Start Game";
				}
			}

			public class COLONYDESTINATIONSCREEN
			{
				public static LocString TITLE = "CHOOSE A DESTINATION";

				public static LocString GENTLE_ZONE = "Habitable Zone";

				public static LocString DETAILS = "Destination Details";

				public static LocString START_SITE = "Immediate Surroundings";

				public static LocString COORDINATE = "Coordinates:";

				public static LocString CANCEL = "Back";

				public static LocString CUSTOMIZE = "Game Settings";

				public static LocString START_GAME = "Start Game";

				public static LocString SHUFFLE = "Shuffle";

				public static LocString SHUFFLETOOLTIP = "Reroll World Seed\n\nThis will shuffle the layout of your world and the geographical traits listed below";

				public static LocString SHUFFLETOOLTIP_DISABLED = "This world's seed is predetermined. It cannot be changed";

				public static LocString HEADER_ASTEROID_STARTING = "Starting Asteroid";

				public static LocString HEADER_ASTEROID_NEARBY = "Nearby Asteroids";

				public static LocString HEADER_ASTEROID_DISTANT = "Distant Asteroids";

				public static LocString TRAITS_HEADER = "World Traits";

				public static LocString STORY_TRAITS_HEADER = "Story Traits";

				public static LocString NO_TRAITS = "No Traits";

				public static LocString SINGLE_TRAIT = "1 Trait";

				public static LocString TRAIT_COUNT = "{0} Traits";

				public static LocString TOO_MANY_TRAITS_WARNING = UI.YELLOW_PREFIX + "Too many!" + UI.COLOR_SUFFIX;

				public static LocString TOO_MANY_TRAITS_WARNING_TOOLTIP = UI.YELLOW_PREFIX + "Squeezing this many story traits into this asteroid may cause worldgen to fail\n\nConsider lowering the number of story traits or changing the selected asteroid" + UI.COLOR_SUFFIX;

				public static LocString SHUFFLE_STORY_TRAITS_TOOLTIP = "Randomize Story Traits\n\nThis will select a comfortable number of story traits for the starting asteroid";

				public static LocString SELECTED_CLUSTER_TRAITS_HEADER = "Target Details";
			}

			public class MODESELECTSCREEN
			{
				public static LocString HEADER = "GAME MODE";

				public static LocString BLANK_DESC = "Select a playstyle...";

				public static LocString SURVIVAL_TITLE = "SURVIVAL";

				public static LocString SURVIVAL_DESC = "Stay on your toes and one step ahead of this unforgiving world. One slip up could bring your colony crashing down.";

				public static LocString NOSWEAT_TITLE = "NO SWEAT";

				public static LocString NOSWEAT_DESC = "When disaster strikes (and it inevitably will), take a deep breath and stay calm. You have ample time to find a solution.";
			}

			public class CLUSTERCATEGORYSELECTSCREEN
			{
				public static LocString HEADER = "ASTEROID STYLE";

				public static LocString BLANK_DESC = "Select an asteroid style...";

				public static LocString VANILLA_TITLE = "Standard";

				public static LocString VANILLA_DESC = "Scenarios designed for classic gameplay.";

				public static LocString CLASSIC_TITLE = "Classic";

				public static LocString CLASSIC_DESC = "Scenarios similar to the <b>classic Oxygen Not Included</b> experience. Large starting asteroids with many resources.\nLess emphasis on space travel.";

				public static LocString SPACEDOUT_TITLE = "Spaced Out!";

				public static LocString SPACEDOUT_DESC = "Scenarios designed for the <b>Spaced Out! DLC</b>.\nSmaller starting asteroids with resources distributed across the starmap. More emphasis on space travel.";

				public static LocString EVENT_TITLE = "The Lab";

				public static LocString EVENT_DESC = "Alternative gameplay experiences, including experimental scenarios designed for special events.";
			}

			public class PATCHNOTESSCREEN
			{
				public static LocString HEADER = "IMPORTANT UPDATE NOTES";

				public static LocString OK_BUTTON = "OK";

				public static LocString FULLPATCHNOTES_TOOLTIP = "View the full patch notes online";
			}

			public class MOTD
			{
				public static LocString IMAGE_HEADER = "JUNE 2023 QOL";

				public static LocString NEWS_HEADER = "JOIN THE DISCUSSION";

				public static LocString NEWS_BODY = "Stay up to date by joining our mailing list, or head on over to the forums and join the discussion.";

				public static LocString PATCH_NOTES_SUMMARY = "This update includes:\n\n•<indent=20px>Significant changes to Space Scanner and telescopes.</indent>\n•<indent=20px>New category of cosmetic skins.</indent>\n•<indent=20px>New Move To tool for transporting objects.</indent>\n•<indent=20px>A strange new asteroid and Puft Atmo Suit skin for Klei Fest.</indent>\n•<indent=20px>Bug fixes and quality of life improvements.</indent>\n\n   Check out the full patch notes for more details!";

				public static LocString UPDATE_TEXT = "LAUNCHED!";

				public static LocString UPDATE_TEXT_EXPANSION1 = "LAUNCHED!";
			}

			public class LOADSCREEN
			{
				public static LocString TITLE = "LOAD GAME";

				public static LocString TITLE_INSPECT = "LOAD GAME";

				public static LocString DELETEBUTTON = "DELETE";

				public static LocString BACKBUTTON = "< BACK";

				public static LocString CONFIRMDELETE = "Are you sure you want to delete {0}?\nYou cannot undo this action.";

				public static LocString SAVEDETAILS = "<b>File:</b> {0}\n\n<b>Save Date:</b>\n{1}\n\n<b>Base Name:</b> {2}\n<b>Duplicants Alive:</b> {3}\n<b>Cycle(s) Survived:</b> {4}";

				public static LocString AUTOSAVEWARNING = "<color=#F44A47FF>Autosave: This file will get deleted as new autosaves are created</color>";

				public static LocString CORRUPTEDSAVE = "<b><color=#F44A47FF>Could not load file {0}. Its data may be corrupted.</color></b>";

				public static LocString SAVE_FROM_SPACED_OUT_TOOLTIP = "<color=#F44A47FF>This save was created in the <i>Spaced Out!</i> DLC. Activate the DLC to play it</color>";

				public static LocString SAVE_FROM_VANILLA_TOOLTIP = "<color=#F44A47FF>This save was created in the base game. Deactivate the <i>Spaced Out!</i> DLC to play it.</color>";

				public static LocString SAVE_IS_SPACED_OUT_TOOLTIP = "<i>Spaced Out!</i> DLC save";

				public static LocString SAVE_IS_VANILLA_TOOLTIP = "Base game save";

				public static LocString SAVE_TOO_NEW = "<b><color=#F44A47FF>Could not load file {0}. File is using build {1}, v{2}. This build is {3}, v{4}.</color></b>";

				public static LocString SAVE_MISSING_CONTENT = "<b><color=#F44A47FF>Could not load file {0}. File was saved with content that is not currently installed.</color></b>";

				public static LocString UNSUPPORTED_SAVE_VERSION = "<b><color=#F44A47FF>This save file is from a previous version of the game and is no longer supported.</color></b>";

				public static LocString MORE_INFO = "More Info";

				public static LocString NEWEST_SAVE = "Newest Save";

				public static LocString BASE_NAME = "Base Name";

				public static LocString CYCLES_SURVIVED = "Cycles Survived";

				public static LocString DUPLICANTS_ALIVE = "Duplicants Alive";

				public static LocString WORLD_NAME = "Asteroid Type";

				public static LocString NO_FILE_SELECTED = "No file selected";

				public static LocString COLONY_INFO_FMT = "{0}: {1}";

				public static LocString LOAD_MORE_COLONIES_BUTTON = "Load more...";

				public static LocString VANILLA_RESTART = "Loading this colony will require restarting the game with " + UI.DLC1.NAME_ITAL + " content disabled";

				public static LocString EXPANSION1_RESTART = "Loading this colony will require restarting the game with " + UI.DLC1.NAME_ITAL + " content enabled";

				public static LocString UNSUPPORTED_VANILLA_TEMP = "<b><color=#F44A47FF>This save file is from the base version of the game and currently cannot be loaded while " + UI.DLC1.NAME_ITAL + " is installed.</color></b>";

				public static LocString CONTENT = "Content";

				public static LocString VANILLA_CONTENT = "Vanilla FIXME";

				public static LocString EXPANSION1_CONTENT = UI.DLC1.NAME_ITAL + " Expansion FIXME";

				public static LocString SAVE_INFO = "{0} saves  {1} autosaves  {2}";

				public static LocString COLONIES_TITLE = "Colony View";

				public static LocString COLONY_TITLE = "Viewing colony '{0}'";

				public static LocString COLONY_FILE_SIZE = "Size: {0}";

				public static LocString COLONY_FILE_NAME = "File: '{0}'";

				public static LocString NO_PREVIEW = "NO PREVIEW";

				public static LocString LOCAL_SAVE = "local";

				public static LocString CLOUD_SAVE = "cloud";

				public static LocString CONVERT_COLONY = "CONVERT COLONY";

				public static LocString CONVERT_ALL_COLONIES = "CONVERT ALL";

				public static LocString CONVERT_ALL_WARNING = UI.PRE_KEYWORD + "\nWarning:" + UI.PST_KEYWORD + " Converting all colonies may take some time.";

				public static LocString SAVE_INFO_DIALOG_TITLE = "SAVE INFORMATION";

				public static LocString SAVE_INFO_DIALOG_TEXT = "Access your save files using the options below.";

				public static LocString SAVE_INFO_DIALOG_TOOLTIP = "Access your save file locations from here.";

				public static LocString CONVERT_ERROR_TITLE = "SAVE CONVERSION UNSUCCESSFUL";

				public static LocString CONVERT_ERROR = string.Concat(new string[]
				{
					"Converting the colony ",
					UI.PRE_KEYWORD,
					"{Colony}",
					UI.PST_KEYWORD,
					" was unsuccessful!\nThe error was:\n\n<b>{Error}</b>\n\nPlease try again, or post a bug in the forums if this problem keeps happening."
				});

				public static LocString CONVERT_TO_CLOUD = "CONVERT TO CLOUD SAVES";

				public static LocString CONVERT_TO_LOCAL = "CONVERT TO LOCAL SAVES";

				public static LocString CONVERT_COLONY_TO_CLOUD = "Convert colony to use cloud saves";

				public static LocString CONVERT_COLONY_TO_LOCAL = "Convert to colony to use local saves";

				public static LocString CONVERT_ALL_TO_CLOUD = "Convert <b>all</b> colonies below to use cloud saves";

				public static LocString CONVERT_ALL_TO_LOCAL = "Convert <b>all</b> colonies below to use local saves";

				public static LocString CONVERT_ALL_TO_CLOUD_SUCCESS = string.Concat(new string[]
				{
					UI.PRE_KEYWORD,
					"SUCCESS!",
					UI.PST_KEYWORD,
					"\nAll existing colonies have been converted into ",
					UI.PRE_KEYWORD,
					"cloud",
					UI.PST_KEYWORD,
					" saves.\nNew colonies will use ",
					UI.PRE_KEYWORD,
					"cloud",
					UI.PST_KEYWORD,
					" saves by default.\n\n{Client} may take longer than usual to sync the next time you exit the game as a result of this change."
				});

				public static LocString CONVERT_ALL_TO_LOCAL_SUCCESS = string.Concat(new string[]
				{
					UI.PRE_KEYWORD,
					"SUCCESS!",
					UI.PST_KEYWORD,
					"\nAll existing colonies have been converted into ",
					UI.PRE_KEYWORD,
					"local",
					UI.PST_KEYWORD,
					" saves.\nNew colonies will use ",
					UI.PRE_KEYWORD,
					"local",
					UI.PST_KEYWORD,
					" saves by default.\n\n{Client} may take longer than usual to sync the next time you exit the game as a result of this change."
				});

				public static LocString CONVERT_TO_CLOUD_DETAILS = "Converting a colony to use cloud saves will move all of the save files for that colony into the cloud saves folder.\n\nThis allows your game platform to sync this colony to the cloud for your account, so it can be played on multiple machines.";

				public static LocString CONVERT_TO_LOCAL_DETAILS = "Converting a colony to NOT use cloud saves will move all of the save files for that colony into the local saves folder.\n\n" + UI.PRE_KEYWORD + "These save files will no longer be synced to the cloud." + UI.PST_KEYWORD;

				public static LocString OPEN_SAVE_FOLDER = "LOCAL SAVES";

				public static LocString OPEN_CLOUDSAVE_FOLDER = "CLOUD SAVES";

				public static LocString MIGRATE_TITLE = "SAVE FILE MIGRATION";

				public static LocString MIGRATE_SAVE_FILES = "MIGRATE SAVE FILES";

				public static LocString MIGRATE_COUNT = string.Concat(new string[]
				{
					"\nFound ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" saves and ",
					UI.PRE_KEYWORD,
					"{1}",
					UI.PST_KEYWORD,
					" autosaves that require migration."
				});

				public static LocString MIGRATE_RESULT = string.Concat(new string[]
				{
					UI.PRE_KEYWORD,
					"SUCCESS!",
					UI.PST_KEYWORD,
					"\nMigration moved ",
					UI.PRE_KEYWORD,
					"{0}/{1}",
					UI.PST_KEYWORD,
					" saves and ",
					UI.PRE_KEYWORD,
					"{2}/{3}",
					UI.PST_KEYWORD,
					" autosaves",
					UI.PST_KEYWORD,
					"."
				});

				public static LocString MIGRATE_RESULT_FAILURES = string.Concat(new string[]
				{
					UI.PRE_KEYWORD,
					"<b>WARNING:</b> Not all saves could be migrated.",
					UI.PST_KEYWORD,
					"\nMigration moved ",
					UI.PRE_KEYWORD,
					"{0}/{1}",
					UI.PST_KEYWORD,
					" saves and ",
					UI.PRE_KEYWORD,
					"{2}/{3}",
					UI.PST_KEYWORD,
					" autosaves.\n\nThe file ",
					UI.PRE_KEYWORD,
					"{ErrorColony}",
					UI.PST_KEYWORD,
					" encountered this error:\n\n<b>{ErrorMessage}</b>"
				});

				public static LocString MIGRATE_RESULT_FAILURES_MORE_INFO_TITLE = "MIGRATION INCOMPLETE";

				public static LocString MIGRATE_RESULT_FAILURES_MORE_INFO_PRE = "<b>The game was unable to move all save files to their new location.\nTo fix this, please:</b>\n\n";

				public static LocString MIGRATE_RESULT_FAILURES_MORE_INFO_ITEM1 = "    1. Try temporarily disabling virus scanners and malware\n         protection programs.";

				public static LocString MIGRATE_RESULT_FAILURES_MORE_INFO_ITEM2 = "    2. Turn off file sync services such as OneDrive and DropBox.";

				public static LocString MIGRATE_RESULT_FAILURES_MORE_INFO_ITEM3 = "    3. Restart the game to retry file migration.";

				public static LocString MIGRATE_RESULT_FAILURES_MORE_INFO_POST = "\n<b>If this still doesn't solve the problem, please post a bug in the forums and we will attempt to assist with your issue.</b>";

				public static LocString MIGRATE_INFO = "We've changed how save files are organized!\nPlease " + UI.CLICK(UI.ClickType.click) + " the button below to automatically update your save file storage.";

				public static LocString MIGRATE_DONE = "CONTINUE";

				public static LocString MIGRATE_FAILURES_FORUM_BUTTON = "VISIT FORUMS";

				public static LocString MIGRATE_FAILURES_DONE = "MORE INFO";

				public static LocString CLOUD_TUTORIAL_BOUNCER = "Upload Saves to Cloud";
			}

			public class SAVESCREEN
			{
				public static LocString TITLE = "SAVE SLOTS";

				public static LocString NEWSAVEBUTTON = "New Save";

				public static LocString OVERWRITEMESSAGE = "Are you sure you want to overwrite {0}?";

				public static LocString SAVENAMETITLE = "SAVE NAME";

				public static LocString CONFIRMNAME = "Confirm";

				public static LocString CANCELNAME = "Cancel";

				public static LocString IO_ERROR = "An error occurred trying to save your game. Please ensure there is sufficient disk space.\n\n{0}";

				public static LocString REPORT_BUG = "Report Bug";
			}

			public class RAILFORCEQUIT
			{
				public static LocString SAVE_EXIT = "Play time has expired and the game is exiting. Would you like to overwrite {0}?";

				public static LocString WARN_EXIT = "Play time has expired and the game will now exit.";

				public static LocString DLC_NOT_PURCHASED = "The <i>Spaced Out!</i> DLC has not yet been purchased in the WeGame store. Purchase <i>Spaced Out!</i> to support <i>Oxygen Not Included</i> and enjoy the new content!";
			}

			public class MOD_ERRORS
			{
				public static LocString TITLE = "MOD ERRORS";

				public static LocString DETAILS = "DETAILS";

				public static LocString CLOSE = "CLOSE";
			}

			public class MODS
			{
				public static LocString TITLE = "MODS";

				public static LocString MANAGE = "Subscription";

				public static LocString MANAGE_LOCAL = "Browse";

				public static LocString WORKSHOP = "STEAM WORKSHOP";

				public static LocString ENABLE_ALL = "ENABLE ALL";

				public static LocString DISABLE_ALL = "DISABLE ALL";

				public static LocString DRAG_TO_REORDER = "Drag to reorder";

				public static LocString REQUIRES_RESTART = "Mod changes require restart";

				public static LocString FAILED_TO_LOAD = "A mod failed to load and is being disabled:\n\n{0}: {1}\n\n{2}";

				public static LocString DB_CORRUPT = "An error occurred trying to load the Mod Database.\n\n{0}";

				public class CONTENT_FAILURE
				{
					public static LocString DISABLED_CONTENT = " - <b>Not compatible with <i>{Content}</i></b>";

					public static LocString NO_CONTENT = " - <b>No compatible mod found</b>";

					public static LocString OLD_API = " - <b>Mod out-of-date</b>";
				}

				public class TOOLTIPS
				{
					public static LocString ENABLED = "Enabled";

					public static LocString DISABLED = "Disabled";

					public static LocString MANAGE_STEAM_SUBSCRIPTION = "Manage Steam Subscription";

					public static LocString MANAGE_RAIL_SUBSCRIPTION = "Manage Subscription";

					public static LocString MANAGE_LOCAL_MOD = "Manage Local Mod";
				}

				public class RAILMODUPLOAD
				{
					public static LocString TITLE = "Upload Mod";

					public static LocString NAME = "Mod Name";

					public static LocString DESCRIPTION = "Mod Description";

					public static LocString VERSION = "Version Number";

					public static LocString PREVIEW_IMAGE = "Preview Image Path";

					public static LocString CONTENT_FOLDER = "Content Folder Path";

					public static LocString SHARE_TYPE = "Share Type";

					public static LocString SUBMIT = "Submit";

					public static LocString SUBMIT_READY = "This mod is ready to submit";

					public static LocString SUBMIT_NOT_READY = "The mod cannot be submitted. Check that all fields are properly entered and that the paths are valid.";

					public static class MOD_SHARE_TYPE
					{
						public static LocString PRIVATE = "Private";

						public static LocString TOOLTIP_PRIVATE = "This mod will only be visible to its creator";

						public static LocString FRIEND = "Friend";

						public static LocString TOOLTIP_FRIEND = "Friend";

						public static LocString PUBLIC = "Public";

						public static LocString TOOLTIP_PUBLIC = "This mod will be available to all players after publishing. It may be subject to review before being allowed to be published.";
					}

					public static class MOD_UPLOAD_RESULT
					{
						public static LocString SUCCESS = "Mod upload succeeded.";

						public static LocString FAILURE = "Mod upload failed.";
					}
				}
			}

			public class MOD_EVENTS
			{
				public static LocString REQUIRED = "REQUIRED";

				public static LocString NOT_FOUND = "NOT FOUND";

				public static LocString INSTALL_INFO_INACCESSIBLE = "INACCESSIBLE";

				public static LocString OUT_OF_ORDER = "ORDERING CHANGED";

				public static LocString ACTIVE_DURING_CRASH = "ACTIVE DURING CRASH";

				public static LocString EXPECTED_ENABLED = "NOT ENABLED";

				public static LocString EXPECTED_DISABLED = "NOT DISABLED";

				public static LocString VERSION_UPDATE = "VERSION UPDATE";

				public static LocString AVAILABLE_CONTENT_CHANGED = "CONTENT CHANGED";

				public static LocString INSTALL_FAILED = "INSTALL FAILED";

				public static LocString INSTALLED = "INSTALLED";

				public static LocString UNINSTALLED = "UNINSTALLED";

				public static LocString REQUIRES_RESTART = "RESTART REQUIRED";

				public static LocString BAD_WORLD_GEN = "LOAD FAILED";

				public static LocString DEACTIVATED = "DEACTIVATED";

				public static LocString ALL_MODS_DISABLED_EARLY_ACCESS = "DEACTIVATED";

				public class TOOLTIPS
				{
					public static LocString REQUIRED = "The current save game couldn't load this mod. Unexpected things may happen!";

					public static LocString NOT_FOUND = "This mod isn't installed";

					public static LocString INSTALL_INFO_INACCESSIBLE = "Mod files are inaccessible";

					public static LocString OUT_OF_ORDER = "Active mod has changed order with respect to some other active mod";

					public static LocString ACTIVE_DURING_CRASH = "Mod was active during a crash and may be the cause";

					public static LocString EXPECTED_ENABLED = "This mod needs to be enabled";

					public static LocString EXPECTED_DISABLED = "This mod needs to be disabled";

					public static LocString VERSION_UPDATE = "New version detected";

					public static LocString AVAILABLE_CONTENT_CHANGED = "Content added or removed";

					public static LocString INSTALL_FAILED = "Installation failed";

					public static LocString INSTALLED = "Installation succeeded";

					public static LocString UNINSTALLED = "Uninstalled";

					public static LocString BAD_WORLD_GEN = "Encountered an error while loading file";

					public static LocString DEACTIVATED = "Deactivated due to errors";

					public static LocString ALL_MODS_DISABLED_EARLY_ACCESS = "Deactivated due to Early Access for " + UI.DLC1.NAME_ITAL;
				}
			}

			public class MOD_DIALOGS
			{
				public static LocString ADDITIONAL_MOD_EVENTS = "(...additional entries omitted)";

				public class INSTALL_INFO_INACCESSIBLE
				{
					public static LocString TITLE = "STEAM CONTENT ERROR";

					public static LocString MESSAGE = "Failed to access local Steam files for mod {0}.\nTry restarting Oxygen not Included.\nIf that doesn't work, try re-subscribing to the mod via Steam.";
				}

				public class STEAM_SUBSCRIBED
				{
					public static LocString TITLE = "STEAM MOD SUBSCRIBED";

					public static LocString MESSAGE = "Subscribed to Steam mod: {0}";
				}

				public class STEAM_UPDATED
				{
					public static LocString TITLE = "STEAM MOD UPDATE";

					public static LocString MESSAGE = "Updating version of Steam mod: {0}";
				}

				public class STEAM_UNSUBSCRIBED
				{
					public static LocString TITLE = "STEAM MOD UNSUBSCRIBED";

					public static LocString MESSAGE = "Unsubscribed from Steam mod: {0}";
				}

				public class STEAM_REFRESH
				{
					public static LocString TITLE = "STEAM MODS REFRESHED";

					public static LocString MESSAGE = "Refreshed Steam mods:\n{0}";
				}

				public class ALL_MODS_DISABLED_EARLY_ACCESS
				{
					public static LocString TITLE = "ALL MODS DISABLED";

					public static LocString MESSAGE = "Mod support is temporarily suspended for the initial launch of " + UI.DLC1.NAME_ITAL + " into Early Access:\n{0}";
				}

				public class LOAD_FAILURE
				{
					public static LocString TITLE = "LOAD FAILURE";

					public static LocString MESSAGE = "Failed to load one or more mods:\n{0}\nThey will be re-installed when the game is restarted.\nGame may be unstable until then.";
				}

				public class SAVE_GAME_MODS_DIFFER
				{
					public static LocString TITLE = "MOD DIFFERENCES";

					public static LocString MESSAGE = "Save game mods differ from currently active mods:\n{0}";
				}

				public class MOD_ERRORS_ON_BOOT
				{
					public static LocString TITLE = "MOD ERRORS";

					public static LocString MESSAGE = "An error occurred during start-up with mods active.\nAll mods have been disabled to ensure a clean restart.\n{0}";

					public static LocString DEV_MESSAGE = "An error occurred during start-up with mods active.\n{0}\nDisable all mods and restart, or continue in an unstable state?";
				}

				public class MODS_SCREEN_CHANGES
				{
					public static LocString TITLE = "MODS CHANGED";

					public static LocString MESSAGE = "Previous config:\n{0}\nRestart required to reload mods.\nGame may be unstable until then.";
				}

				public class MOD_EVENTS
				{
					public static LocString TITLE = "MOD EVENTS";

					public static LocString MESSAGE = "{0}";

					public static LocString DEV_MESSAGE = "{0}\nCheck Player.log for details.";
				}

				public class RESTART
				{
					public static LocString OK = "RESTART";

					public static LocString CANCEL = "CONTINUE";

					public static LocString MESSAGE = "{0}\nRestart required.";

					public static LocString DEV_MESSAGE = "{0}\nRestart required.\nGame may be unstable until then.";
				}
			}

			public class PAUSE_SCREEN
			{
				public static LocString TITLE = "PAUSED";

				public static LocString RESUME = "Resume";

				public static LocString LOGBOOK = "Logbook";

				public static LocString OPTIONS = "Options";

				public static LocString SAVE = "Save";

				public static LocString SAVEAS = "Save As";

				public static LocString COLONY_SUMMARY = "Colony Summary";

				public static LocString LOCKERMENU = "Supply Closet";

				public static LocString LOAD = "Load";

				public static LocString QUIT = "Main Menu";

				public static LocString DESKTOPQUIT = "Quit to Desktop";

				public static LocString WORLD_SEED = "Coordinates: {0}";

				public static LocString WORLD_SEED_TOOLTIP = "Share coordinates with a friend and they can start a colony on an identical asteroid!\n\n{0} - The asteroid\n\n{1} - The world seed\n\n{2} - Difficulty and Custom settings\n\n{3} - Story Trait settings";

				public static LocString WORLD_SEED_COPY_TOOLTIP = "Copy Coordinates to clipboard\n\nShare coordinates with a friend and they can start a colony on an identical asteroid!";

				public static LocString MANAGEMENT_BUTTON = "Pause Menu";
			}

			public class OPTIONS_SCREEN
			{
				public static LocString TITLE = "OPTIONS";

				public static LocString GRAPHICS = "Graphics";

				public static LocString AUDIO = "Audio";

				public static LocString GAME = "Game";

				public static LocString CONTROLS = "Controls";

				public static LocString UNITS = "Temperature Units";

				public static LocString METRICS = "Data Communication";

				public static LocString LANGUAGE = "Change Language";

				public static LocString WORLD_GEN = "World Generation Key";

				public static LocString RESET_TUTORIAL = "Reset Tutorial Messages";

				public static LocString RESET_TUTORIAL_WARNING = "All tutorial messages will be reset, and\nwill show up again the next time you play the game.";

				public static LocString FEEDBACK = "Feedback";

				public static LocString CREDITS = "Credits";

				public static LocString BACK = "Done";

				public static LocString UNLOCK_SANDBOX = "Unlock Sandbox Mode";

				public static LocString MODS = "MODS";

				public static LocString SAVE_OPTIONS = "Save Options";

				public class TOGGLE_SANDBOX_SCREEN
				{
					public static LocString UNLOCK_SANDBOX_WARNING = "Sandbox Mode will be enabled for this save file";

					public static LocString CONFIRM = "Enable Sandbox Mode";

					public static LocString CANCEL = "Cancel";

					public static LocString CONFIRM_SAVE_BACKUP = "Enable Sandbox Mode, but save a backup first";

					public static LocString BACKUP_SAVE_GAME_APPEND = " (BACKUP)";
				}
			}

			public class INPUT_BINDINGS_SCREEN
			{
				public static LocString TITLE = "CUSTOMIZE KEYS";

				public static LocString RESET = "Reset";

				public static LocString APPLY = "Done";

				public static LocString DUPLICATE = "{0} was already bound to {1} and is now unbound.";

				public static LocString UNBOUND_ACTION = "{0} is unbound. Are you sure you want to continue?";

				public static LocString MULTIPLE_UNBOUND_ACTIONS = "You have multiple unbound actions, this may result in difficulty playing the game. Are you sure you want to continue?";

				public static LocString WAITING_FOR_INPUT = "???";
			}

			public class TRANSLATIONS_SCREEN
			{
				public static LocString TITLE = "TRANSLATIONS";

				public static LocString UNINSTALL = "Uninstall";

				public static LocString PREINSTALLED_HEADER = "Preinstalled Language Packs";

				public static LocString UGC_HEADER = "Subscribed Workshop Language Packs";

				public static LocString UGC_MOD_TITLE_FORMAT = "{0} (workshop)";

				public static LocString ARE_YOU_SURE = "Are you sure you want to uninstall this language pack?";

				public static LocString PLEASE_REBOOT = "Please restart your game for these changes to take effect.";

				public static LocString NO_PACKS = "Steam Workshop";

				public static LocString DOWNLOAD = "Start Download";

				public static LocString INSTALL = "Install";

				public static LocString INSTALLED = "Installed";

				public static LocString NO_STEAM = "Unable to retrieve language list from Steam";

				public static LocString RESTART = "RESTART";

				public static LocString CANCEL = "CANCEL";

				public static LocString MISSING_LANGUAGE_PACK = "Selected language pack ({0}) not found.\nReverting to default language.";

				public static LocString UNKNOWN = "Unknown";

				public class PREINSTALLED_LANGUAGES
				{
					public static LocString EN = "English (Klei)";

					public static LocString ZH_KLEI = "Chinese (Klei)";

					public static LocString KO_KLEI = "Korean (Klei)";

					public static LocString RU_KLEI = "Russian (Klei)";
				}
			}

			public class SCENARIOS_MENU
			{
				public static LocString TITLE = "Scenarios";

				public static LocString UNSUBSCRIBE = "Unsubscribe";

				public static LocString UNSUBSCRIBE_CONFIRM = "Are you sure you want to unsubscribe from this scenario?";

				public static LocString LOAD_SCENARIO_CONFIRM = "Load the \"{SCENARIO_NAME}\" scenario?";

				public static LocString LOAD_CONFIRM_TITLE = "LOAD";

				public static LocString SCENARIO_NAME = "Name:";

				public static LocString SCENARIO_DESCRIPTION = "Description";

				public static LocString BUTTON_DONE = "Done";

				public static LocString BUTTON_LOAD = "Load";

				public static LocString BUTTON_WORKSHOP = "Steam Workshop";

				public static LocString NO_SCENARIOS_AVAILABLE = "No scenarios available.\n\nSubscribe to some in the Steam Workshop.";
			}

			public class AUDIO_OPTIONS_SCREEN
			{
				public static LocString TITLE = "AUDIO OPTIONS";

				public static LocString HEADER_VOLUME = "VOLUME";

				public static LocString HEADER_SETTINGS = "SETTINGS";

				public static LocString DONE_BUTTON = "Done";

				public static LocString MUSIC_EVERY_CYCLE = "Play background music each morning";

				public static LocString MUSIC_EVERY_CYCLE_TOOLTIP = "If enabled, background music will play every cycle instead of every few cycles";

				public static LocString AUTOMATION_SOUNDS_ALWAYS = "Always play automation sounds";

				public static LocString AUTOMATION_SOUNDS_ALWAYS_TOOLTIP = "If enabled, automation sound effects will play even when outside of the " + UI.FormatAsOverlay("Automation Overlay");

				public static LocString MUTE_ON_FOCUS_LOST = "Mute when unfocused";

				public static LocString MUTE_ON_FOCUS_LOST_TOOLTIP = "If enabled, the game will be muted while minimized or if the application loses focus";

				public static LocString AUDIO_BUS_MASTER = "Master";

				public static LocString AUDIO_BUS_SFX = "SFX";

				public static LocString AUDIO_BUS_MUSIC = "Music";

				public static LocString AUDIO_BUS_AMBIENCE = "Ambience";

				public static LocString AUDIO_BUS_UI = "UI";
			}

			public class GAME_OPTIONS_SCREEN
			{
				public static LocString TITLE = "GAME OPTIONS";

				public static LocString GENERAL_GAME_OPTIONS = "GENERAL";

				public static LocString DISABLED_WARNING = "More options available in-game";

				public static LocString DEFAULT_TO_CLOUD_SAVES = "Default to cloud saves";

				public static LocString DEFAULT_TO_CLOUD_SAVES_TOOLTIP = "When a new colony is created, this controls whether it will be saved into the cloud saves folder for syncing or not.";

				public static LocString RESET_TUTORIAL_DESCRIPTION = "Mark all tutorial messages \"unread\"";

				public static LocString SANDBOX_DESCRIPTION = "Enable sandbox tools";

				public static LocString CONTROLS_DESCRIPTION = "Change key bindings";

				public static LocString TEMPERATURE_UNITS = "TEMPERATURE UNITS";

				public static LocString SAVE_OPTIONS = "SAVE";

				public static LocString CAMERA_SPEED_LABEL = "Camera Pan Speed: {0}%";
			}

			public class METRIC_OPTIONS_SCREEN
			{
				public static LocString TITLE = "DATA COMMUNICATION";

				public static LocString HEADER_METRICS = "USER DATA";
			}

			public class COLONY_SAVE_OPTIONS_SCREEN
			{
				public static LocString TITLE = "COLONY SAVE OPTIONS";

				public static LocString DESCRIPTION = "Note: These values are configured per save file";

				public static LocString AUTOSAVE_FREQUENCY = "Autosave frequency:";

				public static LocString AUTOSAVE_FREQUENCY_DESCRIPTION = "Every: {0} cycle(s)";

				public static LocString AUTOSAVE_NEVER = "Never";

				public static LocString TIMELAPSE_RESOLUTION = "Timelapse resolution:";

				public static LocString TIMELAPSE_RESOLUTION_DESCRIPTION = "{0}x{1}";

				public static LocString TIMELAPSE_DISABLED_DESCRIPTION = "Disabled";
			}

			public class FEEDBACK_SCREEN
			{
				public static LocString TITLE = "FEEDBACK";

				public static LocString HEADER = "We would love to hear from you!";

				public static LocString DESCRIPTION = "Let us know if you encounter any problems or how we can improve your Oxygen Not Included experience.\n\nWhen reporting a bug, please include your log and colony save file. The buttons to the right will help you find those files on your local drive.\n\nThank you for being part of the Oxygen Not Included community!";

				public static LocString ALT_DESCRIPTION = "Let us know if you encounter any problems or how we can improve your Oxygen Not Included experience.\n\nWhen reporting a bug, please include your log and colony save file.\n\nThank you for being part of the Oxygen Not Included community!";

				public static LocString BUG_FORUMS_BUTTON = "Report a Bug";

				public static LocString SUGGESTION_FORUMS_BUTTON = "Suggestions Forum";

				public static LocString LOGS_DIRECTORY_BUTTON = "Browse Log Files";

				public static LocString SAVE_FILES_DIRECTORY_BUTTON = "Browse Save Files";
			}

			public class WORLD_GEN_OPTIONS_SCREEN
			{
				public static LocString TITLE = "WORLD GENERATION OPTIONS";

				public static LocString USE_SEED = "Set Worldgen Seed";

				public static LocString DONE_BUTTON = "Done";

				public static LocString RANDOM_BUTTON = "Randomize";

				public static LocString RANDOM_BUTTON_TOOLTIP = "Randomize a new worldgen seed";

				public static LocString TOOLTIP = "This will override the current worldgen seed";
			}

			public class METRICS_OPTIONS_SCREEN
			{
				public static LocString TITLE = "DATA COMMUNICATION OPTIONS";

				public static LocString ENABLE_BUTTON = "Enable Data Communication";

				public static LocString DESCRIPTION = "Collecting user data helps us improve the game.\n\nPlayers who opt out of data communication will no longer send us crash reports and play data.\n\nThey will also be unable to receive new item unlocks from our servers, though existing unlocked items will continue to function.\n\nFor more details on our privacy policy and how we use the data we collect, please visit our <color=#ECA6C9><u><b>privacy center</b></u></color>.";

				public static LocString DONE_BUTTON = "Done";

				public static LocString RESTART_BUTTON = "Restart Game";

				public static LocString TOOLTIP = "Uncheck to disable data communication";

				public static LocString RESTART_WARNING = "A game restart is required to apply settings.";
			}

			public class UNIT_OPTIONS_SCREEN
			{
				public static LocString TITLE = "TEMPERATURE UNITS";

				public static LocString CELSIUS = "Celsius";

				public static LocString CELSIUS_TOOLTIP = "Change temperature unit to Celsius (°C)";

				public static LocString KELVIN = "Kelvin";

				public static LocString KELVIN_TOOLTIP = "Change temperature unit to Kelvin (K)";

				public static LocString FAHRENHEIT = "Fahrenheit";

				public static LocString FAHRENHEIT_TOOLTIP = "Change temperature unit to Fahrenheit (°F)";
			}

			public class GRAPHICS_OPTIONS_SCREEN
			{
				public static LocString TITLE = "GRAPHICS OPTIONS";

				public static LocString FULLSCREEN = "Fullscreen";

				public static LocString RESOLUTION = "Resolution:";

				public static LocString LOWRES = "Low Resolution Textures";

				public static LocString APPLYBUTTON = "Apply";

				public static LocString REVERTBUTTON = "Revert";

				public static LocString DONE_BUTTON = "Done";

				public static LocString UI_SCALE = "UI Scale";

				public static LocString HEADER_DISPLAY = "DISPLAY";

				public static LocString HEADER_UI = "INTERFACE";

				public static LocString COLORMODE = "Color Mode:";

				public static LocString COLOR_MODE_DEFAULT = "Default";

				public static LocString COLOR_MODE_PROTANOPIA = "Protanopia";

				public static LocString COLOR_MODE_DEUTERANOPIA = "Deuteranopia";

				public static LocString COLOR_MODE_TRITANOPIA = "Tritanopia";

				public static LocString ACCEPT_CHANGES = "Accept Changes?";

				public static LocString ACCEPT_CHANGES_STRING_COLOR = "Interface changes will be visible immediately, but applying color changes to in-game text will require a restart.\n\nAccept Changes?";

				public static LocString COLORBLIND_FEEDBACK = "Color blindness options are currently in progress.\n\nIf you would benefit from an alternative color mode or have had difficulties with any of the default colors, please visit the forums and let us know about your experiences.\n\nYour feedback is extremely helpful to us!";

				public static LocString COLORBLIND_FEEDBACK_BUTTON = "Provide Feedback";
			}

			public class WORLDGENSCREEN
			{
				public static LocString TITLE = "NEW GAME";

				public static LocString GENERATINGWORLD = "GENERATING WORLD";

				public static LocString SELECTSIZEPROMPT = "A new world is about to be created. Please select its size.";

				public static LocString LOADINGGAME = "LOADING WORLD...";

				public class SIZES
				{
					public static LocString TINY = "Tiny";

					public static LocString SMALL = "Small";

					public static LocString STANDARD = "Standard";

					public static LocString LARGE = "Big";

					public static LocString HUGE = "Colossal";
				}
			}

			public class MINSPECSCREEN
			{
				public static LocString TITLE = "WARNING!";

				public static LocString SIMFAILEDTOLOAD = "A problem occurred loading Oxygen Not Included. This is usually caused by the Visual Studio C++ 2015 runtime being improperly installed on the system. Please exit the game, run Windows Update, and try re-launching Oxygen Not Included.";

				public static LocString BODY = "We've detected that this computer does not meet the minimum requirements to run Oxygen Not Included. While you may continue with your current specs, the game might not run smoothly for you.\n\nPlease be aware that your experience may suffer as a result.";

				public static LocString OKBUTTON = "Okay, thanks!";

				public static LocString QUITBUTTON = "Quit";
			}

			public class SUPPORTWARNINGS
			{
				public static LocString AUDIO_DRIVERS = "A problem occurred initializing your audio device.\nSorry about that!\n\nThis is usually caused by outdated audio drivers.\n\nPlease visit your audio device manufacturer's website to download the latest drivers.";

				public static LocString AUDIO_DRIVERS_MORE_INFO = "More Info";

				public static LocString DUPLICATE_KEY_BINDINGS = "<b>Duplicate key bindings were detected.\nThis may be because your custom key bindings conflicted with a new feature's default key.\nPlease visit the controls screen to ensure your key bindings are set how you like them.</b>\n{0}";

				public static LocString SAVE_DIRECTORY_READ_ONLY = "A problem occurred while accessing your save directory.\nThis may be because your directory is set to read-only.\n\nPlease ensure your save directory is readable as well as writable and re-launch the game.\n{0}";

				public static LocString SAVE_DIRECTORY_INSUFFICIENT_SPACE = "There is insufficient disk space to write to your save directory.\n\nPlease free at least 15 MB to give your saves some room to breathe.\n{0}";

				public static LocString WORLD_GEN_FILES = "A problem occurred while accessing certain game files that will prevent starting new games.\n\nPlease ensure that the directory and files are readable as well as writable and re-launch the game:\n\n{0}";

				public static LocString WORLD_GEN_FAILURE = "A problem occurred while generating a world from this seed:\n{0}.\n\nUnfortunately, not all seeds germinate. Please try again with a different seed.";

				public static LocString WORLD_GEN_FAILURE_STORY = "A problem occurred while generating a world from this seed:\n{0}.\n\nNot all story traits were able to be placed. Please try again with a different seed or fewer story traits.";

				public static LocString PLAYER_PREFS_CORRUPTED = "A problem occurred while loading your game options.\nThey have been reset to their default settings.\n\n";

				public static LocString IO_UNAUTHORIZED = "An Unauthorized Access Error occurred when trying to write to disk.\nPlease check that you have permissions to write to:\n{0}\n\nThis may prevent the game from saving.";

				public static LocString IO_SUFFICIENT_SPACE = "An Insufficient Space Error occurred when trying to write to disk. \n\nPlease free up some space.\n{0}";

				public static LocString IO_UNKNOWN = "An unknown error occurred when trying to write or access a file.\n{0}";

				public static LocString MORE_INFO_BUTTON = "More Info";
			}

			public class SAVEUPGRADEWARNINGS
			{
				public static LocString SUDDENMORALEHELPER_TITLE = "MORALE CHANGES";

				public static LocString SUDDENMORALEHELPER = "Welcome to the Expressive Upgrade! This update introduces a new Morale system that replaces Food and Decor Expectations that were found in previous versions of the game.\n\nThe game you are trying to load was created before this system was introduced, and will need to be updated. You may either:\n\n\n1) Enable the new Morale system in this save, removing Food and Decor Expectations. It's possible that when you load your save your old colony won't meet your Duplicants' new Morale needs, so they'll receive a 5 cycle Morale boost to give you time to adjust.\n\n2) Disable Morale in this save. The new Morale mechanics will still be visible, but won't affect your Duplicants' stress. Food and Decor expectations will no longer exist in this save.";

				public static LocString SUDDENMORALEHELPER_BUFF = "1) Bring on Morale!";

				public static LocString SUDDENMORALEHELPER_DISABLE = "2) Disable Morale";

				public static LocString NEWAUTOMATIONWARNING_TITLE = "AUTOMATION CHANGES";

				public static LocString NEWAUTOMATIONWARNING = "The following buildings have acquired new automation ports!\n\nTake a moment to check whether these buildings in your colony are now unintentionally connected to existing " + BUILDINGS.PREFABS.LOGICWIRE.NAME + "s.";

				public static LocString MERGEDOWNCHANGES_TITLE = "BREATH OF FRESH AIR UPDATE CHANGES";

				public static LocString MERGEDOWNCHANGES = "Oxygen Not Included has had a <b>major update</b> since this save file was created! In addition to the <b>multitude of bug fixes and quality-of-life features</b>, please pay attention to these changes which may affect your existing colony:";

				public static LocString MERGEDOWNCHANGES_FOOD = "•<indent=20px>Fridges are more effective for early-game food storage</indent>\n•<indent=20px><b>Both</b> freezing temperatures and a sterile gas are needed for <b>total food preservation</b>.</indent>";

				public static LocString MERGEDOWNCHANGES_AIRFILTER = "•<indent=20px>" + BUILDINGS.PREFABS.AIRFILTER.NAME + " now requires <b>5w Power</b>.</indent>\n•<indent=20px>Duplicants will get <b>Stinging Eyes</b> from gasses such as chlorine and hydrogen.</indent>";

				public static LocString MERGEDOWNCHANGES_SIMULATION = "•<indent=20px>Many <b>simulation bugs</b> have been fixed.</indent>\n•<indent=20px>This may <b>change the effectiveness</b> of certain contraptions and " + BUILDINGS.PREFABS.STEAMTURBINE2.NAME + " setups.</indent>";

				public static LocString MERGEDOWNCHANGES_BUILDINGS = "•<indent=20px>The <b>" + BUILDINGS.PREFABS.OXYGENMASKSTATION.NAME + "</b> has been added to aid early-game exploration.</indent>\n•<indent=20px>Use the new <b>Meter Valves</b> for precise control of resources in pipes.</indent>";

				public static LocString SPACESCANNERANDTELESCOPECHANGES_TITLE = "JUNE 2023 QoL UPDATE CHANGES";

				public static LocString SPACESCANNERANDTELESCOPECHANGES_SUMMARY = "There have been significant changes to <b>Space Scanners</b> and <b>Telescopes</b> since this save file was created!\n\nMeteor showers have been disabled for 20 cycles to provide time to adapt.";

				public static LocString SPACESCANNERANDTELESCOPECHANGES_WARNING = "Please note these changes which may affect your existing colony:\n\n";

				public static LocString SPACESCANNERANDTELESCOPECHANGES_SPACESCANNERS = "•<indent=20px>Automation is synced between all Space Scanners targeting the same object.</indent>\n•<indent=20px>Network quality based on the total percentage of sky covered.</indent>\n•<indent=20px>Industrial machinery no longer impacts network quality.</indent>";

				public static LocString SPACESCANNERANDTELESCOPECHANGES_TELESCOPES = "•<indent=20px>Telescopes have a symmetrical scanning range.</indent>\n•<indent=20px>Obstructions block visibility from the blocked tile out toward the outer edge of scanning range.</indent>";
			}
		}

		public class SANDBOX_TOGGLE
		{
			public static LocString TOOLTIP_LOCKED = "<b>Sandbox Mode</b> must be unlocked in the options menu before it can be used. {Hotkey}";

			public static LocString TOOLTIP_UNLOCKED = "Toggle <b>Sandbox Mode</b> {Hotkey}";
		}

		public class SKILLS_SCREEN
		{
			public static LocString CURRENT_MORALE = "Current Morale: {0}\nMorale Need: {1}";

			public static LocString SORT_BY_DUPLICANT = "Duplicants";

			public static LocString SORT_BY_MORALE = "Morale";

			public static LocString SORT_BY_EXPERIENCE = "Skill Points";

			public static LocString SORT_BY_SKILL_AVAILABLE = "Skill Points";

			public static LocString SORT_BY_HAT = "Hat";

			public static LocString SELECT_HAT = "<b>SELECT HAT</b>";

			public static LocString POINTS_AVAILABLE = "<b>SKILL POINTS AVAILABLE</b>";

			public static LocString MORALE = "<b>Morale</b>";

			public static LocString MORALE_EXPECTATION = "<b>Morale Need</b>";

			public static LocString EXPERIENCE = "EXPERIENCE TO NEXT LEVEL";

			public static LocString EXPERIENCE_TOOLTIP = "{0}exp to next Skill Point";

			public static LocString NOT_AVAILABLE = "Not available";

			public class ASSIGNMENT_REQUIREMENTS
			{
				public static LocString EXPECTATION_TARGET_SKILL = "Current Morale: {0}\nSkill Morale Needs: {1}";

				public static LocString EXPECTATION_ALERT_TARGET_SKILL = "{2}'s Current: {0} Morale\n{3} Minimum Morale: {1}";

				public static LocString EXPECTATION_ALERT_DESC_EXPECTATION = "This Duplicant's Morale is too low to handle the rigors of this position, which will cause them Stress over time.";

				public class SKILLGROUP_ENABLED
				{
					public static LocString NAME = "Can perform {0}";

					public static LocString DESCRIPTION = "Capable of performing <b>{0}</b> skills";
				}

				public class MASTERY
				{
					public static LocString CAN_MASTER = "{0} <b>can learn</b> {1}";

					public static LocString HAS_MASTERED = "{0} has <b>already learned</b> {1}";

					public static LocString CANNOT_MASTER = "{0} <b>cannot learn</b> {1}";

					public static LocString STRESS_WARNING_MESSAGE = string.Concat(new string[]
					{
						"Learning {0} will put {1} into a ",
						UI.PRE_KEYWORD,
						"Morale",
						UI.PST_KEYWORD,
						" deficit and cause unnecessary ",
						UI.PRE_KEYWORD,
						"Stress",
						UI.PST_KEYWORD,
						"!"
					});

					public static LocString REQUIRES_MORE_SKILL_POINTS = "    • Not enough " + UI.PRE_KEYWORD + "Skill Points" + UI.PST_KEYWORD;

					public static LocString REQUIRES_PREVIOUS_SKILLS = "    • Missing prerequisite " + UI.PRE_KEYWORD + "Skill" + UI.PST_KEYWORD;

					public static LocString PREVENTED_BY_TRAIT = string.Concat(new string[]
					{
						"    • This Duplicant possesses the ",
						UI.PRE_KEYWORD,
						"{0}",
						UI.PST_KEYWORD,
						" Trait and cannot learn this Skill"
					});

					public static LocString SKILL_APTITUDE = string.Concat(new string[]
					{
						"{0} is interested in {1} and will receive a ",
						UI.PRE_KEYWORD,
						"Morale",
						UI.PST_KEYWORD,
						" bonus for learning it!"
					});

					public static LocString SKILL_GRANTED = "{0} has been granted {1} by a Trait, but does not have increased " + UI.FormatAsKeyWord("Morale Requirements") + " from learning it";
				}
			}
		}

		public class KLEI_INVENTORY_SCREEN
		{
			public static LocString OPEN_INVENTORY_BUTTON = "Open Klei Inventory";

			public static LocString ITEM_FACADE_FOR = "This blueprint works with any {ConfigProperName}.";

			public static LocString ARTABLE_ITEM_FACADE_FOR = "This blueprint works with any {ConfigProperName} of {ArtableQuality} quality.";

			public static LocString CLOTHING_ITEM_FACADE_FOR = "This blueprint can be used in any outfit.";

			public static LocString BALLOON_ARTIST_FACADE_FOR = "This blueprint can be used by any Balloon Artist.";

			public static LocString ITEM_RARITY_DETAILS = "{RarityName} quality.";

			public static LocString ITEM_PLAYER_OWNED_AMOUNT = "My colony has {OwnedCount} of these blueprints.";

			public static LocString ITEM_PLAYER_OWN_NONE = "My colony doesn't have any of these yet.";

			public static LocString ITEM_PLAYER_OWNED_AMOUNT_ICON = "x{OwnedCount}";

			public static LocString ITEM_PLAYER_UNLOCKED_BUT_UNOWNABLE = "This blueprint is part of my colony's permanent collection.";

			public static LocString ITEM_UNKNOWN_NAME = "Uh oh!";

			public static LocString ITEM_UNKNOWN_DESCRIPTION = "Hmm. Looks like this blueprint is missing from the supply closet. Perhaps due to a temporal anomaly...";

			public static class CATEGORIES
			{
				public static LocString EQUIPMENT = "Equipment";

				public static LocString DUPE_TOPS = "Tops & Onesies";

				public static LocString DUPE_BOTTOMS = "Bottoms";

				public static LocString DUPE_GLOVES = "Gloves";

				public static LocString DUPE_SHOES = "Footwear";

				public static LocString DUPE_HATS = "Headgear";

				public static LocString DUPE_ACCESSORIES = "Accessories";

				public static LocString ATMO_SUIT_HELMET = "Atmo Helmets";

				public static LocString ATMO_SUIT_BODY = "Atmo Suits";

				public static LocString ATMO_SUIT_GLOVES = "Atmo Gloves";

				public static LocString ATMO_SUIT_BELT = "Atmo Belts";

				public static LocString ATMO_SUIT_SHOES = "Atmo Boots";

				public static LocString PRIMOGARB = "Primo Garb";

				public static LocString ATMOSUITS = "Atmo Suits";

				public static LocString BUILDINGS = "Buildings";

				public static LocString CRITTERS = "Critters";

				public static LocString SWEEPYS = "Sweepys";

				public static LocString DUPLICANTS = "Duplicants";

				public static LocString ARTWORKS = "Artwork";

				public static LocString MONUMENTPARTS = "Monuments";

				public static LocString JOY_RESPONSE = "Overjoyed Responses";

				public static class JOY_RESPONSES
				{
					public static LocString BALLOON_ARTIST = "Balloon Artist";
				}
			}

			public static class COLUMN_HEADERS
			{
				public static LocString CATEGORY_HEADER = "BLUEPRINTS";

				public static LocString ITEMS_HEADER = "Items";

				public static LocString DETAILS_HEADER = "Details";
			}
		}

		public class ITEM_DROP_SCREEN
		{
			public static LocString THANKS_FOR_PLAYING = "New blueprints unlocked!";

			public static LocString WEB_REWARDS_AVAILABLE = "Rewards available online!";

			public static LocString NOTHING_AVAILABLE = "All available blueprints claimed";

			public static LocString OPEN_URL_BUTTON = "CLAIM";

			public static LocString PRINT_ITEM_BUTTON = "PRINT";

			public static LocString DISMISS_BUTTON = "DISMISS";

			public static class IN_GAME_BUTTON
			{
				public static LocString TOOLTIP_ITEMS_AVAILABLE = "Unlock new blueprints";

				public static LocString TOOLTIP_ERROR_NO_ITEMS = "No new blueprints to unlock";
			}
		}

		public class OUTFIT_BROWSER_SCREEN
		{
			public static LocString BUTTON_ADD_OUTFIT = "New Outfit";

			public static LocString BUTTON_PICK_OUTFIT = "Assign Outfit";

			public static LocString TOOLTIP_PICK_OUTFIT_ERROR_LOCKED = "Cannot assign this outfit to {MinionName} because my colony doesn't have all of these blueprints yet";

			public static LocString BUTTON_EDIT_OUTFIT = "Restyle Outfit";

			public static LocString BUTTON_COPY_OUTFIT = "Copy Outfit";

			public static LocString TOOLTIP_DELETE_OUTFIT = "Delete Outfit";

			public static LocString TOOLTIP_DELETE_OUTFIT_ERROR_READONLY = "This outfit cannot be deleted";

			public static LocString TOOLTIP_RENAME_OUTFIT = "Rename Outfit";

			public static LocString TOOLTIP_RENAME_OUTFIT_ERROR_READONLY = "This outfit cannot be renamed";

			public static LocString TOOLTIP_FILTER_BY_CLOTHING = "View your Clothing Outfits";

			public static LocString TOOLTIP_FILTER_BY_ATMO_SUITS = "View your Atmo Suit Outfits";

			public static class COLUMN_HEADERS
			{
				public static LocString GALLERY_HEADER = "OUTFITS";

				public static LocString MINION_GALLERY_HEADER = "WARDROBE";

				public static LocString DETAILS_HEADER = "Preview";
			}

			public class DELETE_WARNING_POPUP
			{
				public static LocString HEADER = "Delete \"{OutfitName}\"?";

				public static LocString BODY = "Are you sure you want to delete \"{OutfitName}\"?\n\nAny Duplicants assigned to wear this outfit on spawn will be printed wearing their default outfit instead. Existing Duplicants in saved games won't be affected.\n\nThis <b>cannot</b> be undone.";

				public static LocString BUTTON_YES_DELETE = "Yes, delete outfit";

				public static LocString BUTTON_DONT_DELETE = "Cancel";
			}

			public class RENAME_POPUP
			{
				public static LocString HEADER = "RENAME OUTFIT";
			}
		}

		public class LOCKER_MENU
		{
			public static LocString TITLE = "SUPPLY CLOSET";

			public static LocString BUTTON_INVENTORY = "All";

			public static LocString BUTTON_INVENTORY_DESCRIPTION = "View all of my colony's blueprints";

			public static LocString BUTTON_DUPLICANTS = "Duplicants";

			public static LocString BUTTON_DUPLICANTS_DESCRIPTION = "Manage individual Duplicants' outfits";

			public static LocString BUTTON_OUTFITS = "Wardrobe";

			public static LocString BUTTON_OUTFITS_DESCRIPTION = "Manage my colony's collection of outfits";

			public static LocString DEFAULT_DESCRIPTION = "Select a screen";

			public static LocString BUTTON_CLAIM = "Claim Blueprints";

			public static LocString BUTTON_CLAIM_DESCRIPTION = "Claim any available blueprints";

			public static LocString BUTTON_CLAIM_NONE_DESCRIPTION = "All available blueprints claimed";

			public static LocString UNOPENED_ITEMS_TOOLTIP = "New blueprints available";

			public static LocString UNOPENED_ITEMS_NONE_TOOLTIP = "All available blueprints claimed";
		}

		public class LOCKER_NAVIGATOR
		{
			public static LocString BUTTON_BACK = "BACK";

			public static LocString BUTTON_CLOSE = "CLOSE";

			public class DATA_COLLECTION_WARNING_POPUP
			{
				public static LocString HEADER = "Data Communication is Disabled";

				public static LocString BODY = "Data Communication must be enabled in order to access newly unlocked items. This setting can be found in the Options menu.\n\nExisting item unlocks can still be used while Data Communication is disabled.";

				public static LocString BUTTON_OK = "Continue";

				public static LocString BUTTON_OPEN_SETTINGS = "Options Menu";
			}
		}

		public class JOY_RESPONSE_DESIGNER_SCREEN
		{
			public static LocString CATEGORY_HEADER = "OVERJOYED RESPONSES";

			public static LocString BUTTON_APPLY_TO_MINION = "Assign to {MinionName}";

			public static LocString TOOLTIP_NO_FACADES_FOR_JOY_TRAIT = "There aren't any blueprints for {JoyResponseType} Duplicants yet";

			public static LocString TOOLTIP_PICK_JOY_RESPONSE_ERROR_LOCKED = "This Overjoyed Response blueprint cannot be assigned because my colony doesn't own it yet";

			public class CHANGES_NOT_SAVED_WARNING_POPUP
			{
				public static LocString HEADER = "Discard changes to {MinionName}'s Overjoyed Response?";
			}
		}

		public class OUTFIT_DESIGNER_SCREEN
		{
			public static LocString CATEGORY_HEADER = "CLOTHING";

			public class MINION_INSTANCE
			{
				public static LocString BUTTON_APPLY_TO_MINION = "Assign to {MinionName}";

				public static LocString BUTTON_APPLY_TO_TEMPLATE = "Apply to Template";

				public class APPLY_TEMPLATE_POPUP
				{
					public static LocString HEADER = "SAVE AS TEMPLATE";

					public static LocString DESC_SAVE_EXISTING = "\"{OutfitName}\" will be updated and applied to {MinionName} on save.";

					public static LocString DESC_SAVE_NEW = "A new outfit named \"{OutfitName}\" will be created and assigned to {MinionName} on save.";

					public static LocString BUTTON_SAVE_EXISTING = "Update Outfit";

					public static LocString BUTTON_SAVE_NEW = "Save New Outfit";
				}
			}

			public class OUTFIT_TEMPLATE
			{
				public static LocString BUTTON_SAVE = "Save Template";

				public static LocString BUTTON_COPY = "Save a Copy";

				public static LocString TOOLTIP_SAVE_ERROR_LOCKED = "Cannot save this outfit because my colony doesn't own all of its blueprints yet";

				public static LocString TOOLTIP_SAVE_ERROR_READONLY = "This wardrobe staple cannot be altered\n\nMake a copy to save your changes";
			}

			public class CHANGES_NOT_SAVED_WARNING_POPUP
			{
				public static LocString HEADER = "Discard changes to \"{OutfitName}\"?";

				public static LocString BODY = "There are unsaved changes which will be lost if you exit now.\n\nAre you sure you want to discard your changes?";

				public static LocString BUTTON_DISCARD = "Yes, discard changes";

				public static LocString BUTTON_RETURN = "Cancel";
			}

			public class COPY_POPUP
			{
				public static LocString HEADER = "RENAME COPY";
			}
		}

		public class OUTFIT_NAME
		{
			public static LocString NEW = "Custom Outfit";

			public static LocString COPY_OF = "Copy of {OutfitName}";

			public static LocString RESOLVE_CONFLICT = "{OutfitName} ({ConflictNumber})";

			public static LocString ERROR_NAME_EXISTS = "There's already an outfit named \"{OutfitName}\"";

			public static LocString MINIONS_OUTFIT = "{MinionName}'s Current Outfit";

			public static LocString NONE = "Default Outfit";

			public static LocString NONE_JOY_RESPONSE = "Default Overjoyed Response";

			public static LocString NONE_ATMO_SUIT = "Default Atmo Suit";
		}

		public class OUTFIT_DESCRIPTION
		{
			public static LocString CONTAINS_NON_OWNED_ITEMS = "This outfit cannot be worn because my colony doesn't have all of its blueprints yet.";

			public static LocString NO_JOY_RESPONSE_NAME = "Default Overjoyed Response";

			public static LocString NO_JOY_RESPONSE_DESC = "Default response to an overjoyed state.";
		}

		public class MINION_BROWSER_SCREEN
		{
			public static LocString CATEGORY_HEADER = "DUPLICANTS";

			public static LocString BUTTON_CHANGE_OUTFIT = "Open Wardrobe";

			public static LocString BUTTON_EDIT_OUTFIT_ITEMS = "Restyle Outfit";

			public static LocString BUTTON_EDIT_ATMO_SUIT_OUTFIT_ITEMS = "Restyle Atmo Suit";

			public static LocString BUTTON_EDIT_JOY_RESPONSE = "Restyle Overjoyed Response";

			public static LocString OUTFIT_TYPE_CLOTHING = "CLOTHING";

			public static LocString OUTFIT_TYPE_JOY_RESPONSE = "OVERJOYED RESPONSE";

			public static LocString OUTFIT_TYPE_ATMOSUIT = "ATMO SUIT";
		}

		public class PERMIT_RARITY
		{
			public static readonly LocString UNKNOWN = "Unknown";

			public static readonly LocString UNIVERSAL = "Universal";

			public static readonly LocString LOYALTY = "<color=#FFB037>Loyalty</color>";

			public static readonly LocString COMMON = "<color=#97B2B9>Common</color>";

			public static readonly LocString DECENT = "<color=#81EBDE>Decent</color>";

			public static readonly LocString NIFTY = "<color=#71E379>Nifty</color>";

			public static readonly LocString SPLENDID = "<color=#FF6DE7>Splendid</color>";
		}

		public class OUTFITS
		{
			public class BASIC_BLACK
			{
				public static LocString NAME = "Basic Black Outfit";
			}

			public class BASIC_WHITE
			{
				public static LocString NAME = "Basic White Outfit";
			}

			public class BASIC_RED
			{
				public static LocString NAME = "Basic Red Outfit";
			}

			public class BASIC_ORANGE
			{
				public static LocString NAME = "Basic Orange Outfit";
			}

			public class BASIC_YELLOW
			{
				public static LocString NAME = "Basic Yellow Outfit";
			}

			public class BASIC_GREEN
			{
				public static LocString NAME = "Basic Green Outfit";
			}

			public class BASIC_AQUA
			{
				public static LocString NAME = "Basic Aqua Outfit";
			}

			public class BASIC_PURPLE
			{
				public static LocString NAME = "Basic Purple Outfit";
			}

			public class BASIC_PINK_ORCHID
			{
				public static LocString NAME = "Basic Bubblegum Outfit";
			}

			public class BASIC_DEEPRED
			{
				public static LocString NAME = "Team Captain Outfit";
			}

			public class BASIC_BLUE_COBALT
			{
				public static LocString NAME = "True Blue Outfit";
			}

			public class BASIC_PINK_FLAMINGO
			{
				public static LocString NAME = "Pep Rally Outfit";
			}

			public class BASIC_GREEN_KELLY
			{
				public static LocString NAME = "Go Team! Outfit";
			}

			public class BASIC_GREY_CHARCOAL
			{
				public static LocString NAME = "Underdog Outfit";
			}

			public class BASIC_LEMON
			{
				public static LocString NAME = "Team Hype Outfit";
			}

			public class BASIC_SATSUMA
			{
				public static LocString NAME = "Superfan Outfit";
			}

			public class JELLYPUFF_BLUEBERRY
			{
				public static LocString NAME = "Blueberry Jelly Outfit";
			}

			public class JELLYPUFF_GRAPE
			{
				public static LocString NAME = "Grape Jelly Outfit";
			}

			public class JELLYPUFF_LEMON
			{
				public static LocString NAME = "Lemon Jelly Outfit";
			}

			public class JELLYPUFF_LIME
			{
				public static LocString NAME = "Lime Jelly Outfit";
			}

			public class JELLYPUFF_SATSUMA
			{
				public static LocString NAME = "Satsuma Jelly Outfit";
			}

			public class JELLYPUFF_STRAWBERRY
			{
				public static LocString NAME = "Strawberry Jelly Outfit";
			}

			public class JELLYPUFF_WATERMELON
			{
				public static LocString NAME = "Watermelon Jelly Outfit";
			}

			public class ATHLETE
			{
				public static LocString NAME = "Racing Outfit";
			}

			public class CIRCUIT
			{
				public static LocString NAME = "LED Party Outfit";
			}

			public class ATMOSUIT_LIMONE
			{
				public static LocString NAME = "Citrus Atmo Outfit";
			}

			public class ATMOSUIT_SPARKLE_RED
			{
				public static LocString NAME = "Red Glitter Atmo Outfit";
			}

			public class ATMOSUIT_SPARKLE_BLUE
			{
				public static LocString NAME = "Blue Glitter Atmo Outfit";
			}

			public class ATMOSUIT_SPARKLE_GREEN
			{
				public static LocString NAME = "Green Glitter Atmo Outfit";
			}

			public class ATMOSUIT_SPARKLE_LAVENDER
			{
				public static LocString NAME = "Violet Glitter Atmo Outfit";
			}

			public class ATMOSUIT_PUFT
			{
				public static LocString NAME = "Puft Atmo Outfit";
			}
		}

		public class ROLES_SCREEN
		{
			public static LocString MANAGEMENT_BUTTON = "JOBS";

			public static LocString ROLE_PROGRESS = "<b>Job Experience: {0}/{1}</b>\nDuplicants can become eligible for specialized jobs by maxing their current job experience";

			public static LocString NO_JOB_STATION_WARNING = string.Concat(new string[]
			{
				"Build a ",
				UI.PRE_KEYWORD,
				"Printing Pod",
				UI.PST_KEYWORD,
				" to unlock this menu\n\nThe ",
				UI.PRE_KEYWORD,
				"Printing Pod",
				UI.PST_KEYWORD,
				" can be found in the ",
				UI.FormatAsBuildMenuTab("Base Tab", global::Action.Plan1),
				" of the Build Menu"
			});

			public static LocString AUTO_PRIORITIZE = "Auto-Prioritize:";

			public static LocString AUTO_PRIORITIZE_ENABLED = "Duplicant priorities are automatically reconfigured when they are assigned a new job";

			public static LocString AUTO_PRIORITIZE_DISABLED = "Duplicant priorities can only be changed manually";

			public static LocString EXPECTATION_ALERT_EXPECTATION = "Current Morale: {0}\nJob Morale Needs: {1}";

			public static LocString EXPECTATION_ALERT_JOB = "Current Morale: {0}\n{2} Minimum Morale: {1}";

			public static LocString EXPECTATION_ALERT_TARGET_JOB = "{2}'s Current: {0} Morale\n{3} Minimum Morale: {1}";

			public static LocString EXPECTATION_ALERT_DESC_EXPECTATION = "This Duplicant's Morale is too low to handle the rigors of this position, which will cause them Stress over time.";

			public static LocString EXPECTATION_ALERT_DESC_JOB = "This Duplicant's Morale is too low to handle the assigned job, which will cause them Stress over time.";

			public static LocString EXPECTATION_ALERT_DESC_TARGET_JOB = "This Duplicant's Morale is too low to handle the rigors of this position, which will cause them Stress over time.";

			public static LocString HIGHEST_EXPECTATIONS_TIER = "<b>Highest Expectations</b>";

			public static LocString ADDED_EXPECTATIONS_AMOUNT = " (+{0} Expectation)";

			public class WIDGET
			{
				public static LocString NUMBER_OF_MASTERS_TOOLTIP = "<b>Duplicants who have mastered this job:</b>{0}";

				public static LocString NO_MASTERS_TOOLTIP = "<b>No Duplicants have mastered this job</b>";
			}

			public class TIER_NAMES
			{
				public static LocString ZERO = "Tier 0";

				public static LocString ONE = "Tier 1";

				public static LocString TWO = "Tier 2";

				public static LocString THREE = "Tier 3";

				public static LocString FOUR = "Tier 4";

				public static LocString FIVE = "Tier 5";

				public static LocString SIX = "Tier 6";

				public static LocString SEVEN = "Tier 7";

				public static LocString EIGHT = "Tier 8";

				public static LocString NINE = "Tier 9";
			}

			public class SLOTS
			{
				public static LocString UNASSIGNED = "Vacant Position";

				public static LocString UNASSIGNED_TOOLTIP = UI.CLICK(UI.ClickType.Click) + " to assign a Duplicant to this job opening";

				public static LocString NOSLOTS = "No slots available";

				public static LocString NO_ELIGIBLE_DUPLICANTS = "No Duplicants meet the requirements for this job";

				public static LocString ASSIGNMENT_PENDING = "(Pending)";

				public static LocString PICK_JOB = "No Job";

				public static LocString PICK_DUPLICANT = "None";
			}

			public class DROPDOWN
			{
				public static LocString NAME_AND_ROLE = "{0} <color=#F44A47FF>({1})</color>";

				public static LocString ALREADY_ROLE = "(Currently {0})";
			}

			public class SIDEBAR
			{
				public static LocString ASSIGNED_DUPLICANTS = "Assigned Duplicants";

				public static LocString UNASSIGNED_DUPLICANTS = "Unassigned Duplicants";

				public static LocString UNASSIGN = "Unassign job";
			}

			public class PRIORITY
			{
				public static LocString TITLE = "Job Priorities";

				public static LocString DESCRIPTION = "{0}s prioritize these work errands: ";

				public static LocString NO_EFFECT = "This job does not affect errand prioritization";
			}

			public class RESUME
			{
				public static LocString TITLE = "Qualifications";

				public static LocString PREVIOUS_ROLES = "PREVIOUS DUTIES";

				public static LocString UNASSIGNED = "Unassigned";

				public static LocString NO_SELECTION = "No Duplicant selected";
			}

			public class PERKS
			{
				public static LocString TITLE_BASICTRAINING = "Basic Job Training";

				public static LocString TITLE_MORETRAINING = "Additional Job Training";

				public static LocString NO_PERKS = "This job comes with no training";

				public static LocString ATTRIBUTE_EFFECT_FMT = "<b>{0}</b> " + UI.PRE_KEYWORD + "{1}" + UI.PST_KEYWORD;

				public class CAN_DIG_VERY_FIRM
				{
					public static LocString DESCRIPTION = UI.FormatAsLink(ELEMENTS.HARDNESS.HARDNESS_DESCRIPTOR.VERYFIRM + " Material", "HARDNESS") + " Mining";
				}

				public class CAN_DIG_NEARLY_IMPENETRABLE
				{
					public static LocString DESCRIPTION = UI.FormatAsLink("Abyssalite", "KATAIRITE") + " Mining";
				}

				public class CAN_DIG_SUPER_SUPER_HARD
				{
					public static LocString DESCRIPTION = UI.FormatAsLink("Diamond", "DIAMOND") + " and " + UI.FormatAsLink("Obsidian", "OBSIDIAN") + " Mining";
				}

				public class CAN_DIG_RADIOACTIVE_MATERIALS
				{
					public static LocString DESCRIPTION = UI.FormatAsLink("Corium", "CORIUM") + " Mining";
				}

				public class CAN_DIG_UNOBTANIUM
				{
					public static LocString DESCRIPTION = UI.FormatAsLink("Neutronium", "UNOBTANIUM") + " Mining";
				}

				public class CAN_ART
				{
					public static LocString DESCRIPTION = "Can produce artwork using " + BUILDINGS.PREFABS.CANVAS.NAME + " and " + BUILDINGS.PREFABS.SCULPTURE.NAME;
				}

				public class CAN_ART_UGLY
				{
					public static LocString DESCRIPTION = UI.PRE_KEYWORD + "Crude" + UI.PST_KEYWORD + " artwork quality";
				}

				public class CAN_ART_OKAY
				{
					public static LocString DESCRIPTION = UI.PRE_KEYWORD + "Mediocre" + UI.PST_KEYWORD + " artwork quality";
				}

				public class CAN_ART_GREAT
				{
					public static LocString DESCRIPTION = UI.PRE_KEYWORD + "Master" + UI.PST_KEYWORD + " artwork quality";
				}

				public class CAN_FARM_TINKER
				{
					public static LocString DESCRIPTION = UI.FormatAsLink("Crop Tending", "PLANTS") + " and " + ITEMS.INDUSTRIAL_PRODUCTS.FARM_STATION_TOOLS.NAME + " Crafting";
				}

				public class CAN_IDENTIFY_MUTANT_SEEDS
				{
					public static LocString DESCRIPTION = string.Concat(new string[]
					{
						"Can identify ",
						UI.PRE_KEYWORD,
						"Mutant Seeds",
						UI.PST_KEYWORD,
						" at the ",
						BUILDINGS.PREFABS.GENETICANALYSISSTATION.NAME
					});
				}

				public class CAN_WRANGLE_CREATURES
				{
					public static LocString DESCRIPTION = "Critter Wrangling";
				}

				public class CAN_USE_RANCH_STATION
				{
					public static LocString DESCRIPTION = "Grooming Station Usage";
				}

				public class CAN_POWER_TINKER
				{
					public static LocString DESCRIPTION = UI.FormatAsLink("Generator Tuning", "POWER") + " usage and " + ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.NAME + " Crafting";
				}

				public class CAN_ELECTRIC_GRILL
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.COOKINGSTATION.NAME + " Usage";
				}

				public class CAN_SPICE_GRINDER
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.SPICEGRINDER.NAME + " Usage";
				}

				public class CAN_MAKE_MISSILES
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.MISSILEFABRICATOR.NAME + " Usage";
				}

				public class ADVANCED_RESEARCH
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.ADVANCEDRESEARCHCENTER.NAME + " Usage";
				}

				public class INTERSTELLAR_RESEARCH
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.COSMICRESEARCHCENTER.NAME + " Usage";
				}

				public class NUCLEAR_RESEARCH
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.NUCLEARRESEARCHCENTER.NAME + " Usage";
				}

				public class ORBITAL_RESEARCH
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.DLC1COSMICRESEARCHCENTER.NAME + " Usage";
				}

				public class GEYSER_TUNING
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.GEOTUNER.NAME + " Usage";
				}

				public class CAN_CLOTHING_ALTERATION
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.CLOTHINGALTERATIONSTATION.NAME + " Usage";
				}

				public class CAN_STUDY_WORLD_OBJECTS
				{
					public static LocString DESCRIPTION = "Geographical Analysis";
				}

				public class CAN_STUDY_ARTIFACTS
				{
					public static LocString DESCRIPTION = "Artifact Analysis";
				}

				public class CAN_USE_CLUSTER_TELESCOPE
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.CLUSTERTELESCOPE.NAME + " Usage";
				}

				public class EXOSUIT_EXPERTISE
				{
					public static LocString DESCRIPTION = UI.FormatAsLink("Exosuit", "EXOSUIT") + " Penalty Reduction";
				}

				public class EXOSUIT_DURABILITY
				{
					public static LocString DESCRIPTION = "Slows " + UI.FormatAsLink("Exosuit", "EXOSUIT") + " Durability Damage";
				}

				public class CONVEYOR_BUILD
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.SOLIDCONDUIT.NAME + " Construction";
				}

				public class CAN_DO_PLUMBING
				{
					public static LocString DESCRIPTION = "Pipe Emptying";
				}

				public class CAN_USE_ROCKETS
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.COMMANDMODULE.NAME + " Usage";
				}

				public class CAN_DO_ASTRONAUT_TRAINING
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.ASTRONAUTTRAININGCENTER.NAME + " Usage";
				}

				public class CAN_MISSION_CONTROL
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.MISSIONCONTROL.NAME + " Usage";
				}

				public class CAN_PILOT_ROCKET
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.ROCKETCONTROLSTATION.NAME + " Usage";
				}

				public class CAN_COMPOUND
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.APOTHECARY.NAME + " Usage";
				}

				public class CAN_DOCTOR
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.DOCTORSTATION.NAME + " Usage";
				}

				public class CAN_ADVANCED_MEDICINE
				{
					public static LocString DESCRIPTION = BUILDINGS.PREFABS.ADVANCEDDOCTORSTATION.NAME + " Usage";
				}

				public class CAN_DEMOLISH
				{
					public static LocString DESCRIPTION = "Demolish Gravitas Buildings";
				}
			}

			public class ASSIGNMENT_REQUIREMENTS
			{
				public static LocString TITLE = "Qualifications";

				public static LocString NONE = "This position has no qualification requirements";

				public static LocString ALREADY_IS_ROLE = "{0} <b>is already</b> assigned to the {1} position";

				public static LocString ALREADY_IS_JOBLESS = "{0} <b>is already</b> unemployed";

				public static LocString MASTERED = "{0} has mastered the {1} position";

				public static LocString WILL_BE_UNASSIGNED = "Note: Assigning {0} to {1} will <color=#F44A47FF>unassign</color> them from {2}";

				public static LocString RELEVANT_ATTRIBUTES = "Relevant skills:";

				public static LocString APTITUDES = "Interests";

				public static LocString RELEVANT_APTITUDES = "Relevant Interests:";

				public static LocString NO_APTITUDE = "None";

				public class ELIGIBILITY
				{
					public static LocString ELIGIBLE = "{0} is qualified for the {1} position";

					public static LocString INELIGIBLE = "{0} is <color=#F44A47FF>not qualified</color> for the {1} position";
				}

				public class UNEMPLOYED
				{
					public static LocString NAME = "Unassigned";

					public static LocString DESCRIPTION = "Duplicant must not already have a job assignment";
				}

				public class HAS_COLONY_LEADER
				{
					public static LocString NAME = "Has colony leader";

					public static LocString DESCRIPTION = "A colony leader must be assigned";
				}

				public class HAS_ATTRIBUTE_DIGGING_BASIC
				{
					public static LocString NAME = "Basic Digging";

					public static LocString DESCRIPTION = "Must have at least {0} digging skill";
				}

				public class HAS_ATTRIBUTE_COOKING_BASIC
				{
					public static LocString NAME = "Basic Cooking";

					public static LocString DESCRIPTION = "Must have at least {0} cooking skill";
				}

				public class HAS_ATTRIBUTE_LEARNING_BASIC
				{
					public static LocString NAME = "Basic Learning";

					public static LocString DESCRIPTION = "Must have at least {0} learning skill";
				}

				public class HAS_ATTRIBUTE_LEARNING_MEDIUM
				{
					public static LocString NAME = "Medium Learning";

					public static LocString DESCRIPTION = "Must have at least {0} learning skill";
				}

				public class HAS_EXPERIENCE
				{
					public static LocString NAME = "{0} Experience";

					public static LocString DESCRIPTION = "Mastery of the <b>{0}</b> job";
				}

				public class HAS_COMPLETED_ANY_OTHER_ROLE
				{
					public static LocString NAME = "General Experience";

					public static LocString DESCRIPTION = "Mastery of <b>at least one</b> job";
				}

				public class CHOREGROUP_ENABLED
				{
					public static LocString NAME = "Can perform {0}";

					public static LocString DESCRIPTION = "Capable of performing <b>{0}</b> jobs";
				}
			}

			public class EXPECTATIONS
			{
				public static LocString TITLE = "Special Provisions Request";

				public static LocString NO_EXPECTATIONS = "No additional provisions are required to perform this job";

				public class PRIVATE_ROOM
				{
					public static LocString NAME = "Private Bedroom";

					public static LocString DESCRIPTION = "Duplicants in this job would appreciate their own place to unwind";
				}

				public class FOOD_QUALITY
				{
					public class MINOR
					{
						public static LocString NAME = "Standard Food";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire food that meets basic living standards";
					}

					public class MEDIUM
					{
						public static LocString NAME = "Good Food";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire decent food for their efforts";
					}

					public class HIGH
					{
						public static LocString NAME = "Great Food";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire better than average food";
					}

					public class VERY_HIGH
					{
						public static LocString NAME = "Superb Food";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier have a refined taste for food";
					}

					public class EXCEPTIONAL
					{
						public static LocString NAME = "Ambrosial Food";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier expect only the best cuisine";
					}
				}

				public class DECOR
				{
					public class MINOR
					{
						public static LocString NAME = "Minor Decor";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire slightly improved colony decor";
					}

					public class MEDIUM
					{
						public static LocString NAME = "Medium Decor";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire reasonably improved colony decor";
					}

					public class HIGH
					{
						public static LocString NAME = "High Decor";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire a decent increase in colony decor";
					}

					public class VERY_HIGH
					{
						public static LocString NAME = "Superb Decor";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire majorly improved colony decor";
					}

					public class UNREASONABLE
					{
						public static LocString NAME = "Decadent Decor";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire unrealistically luxurious improvements to decor";
					}
				}

				public class QUALITYOFLIFE
				{
					public class TIER0
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 0";
					}

					public class TIER1
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 1";
					}

					public class TIER2
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 2";
					}

					public class TIER3
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 3";
					}

					public class TIER4
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 4";
					}

					public class TIER5
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 5";
					}

					public class TIER6
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 6";
					}

					public class TIER7
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 7";
					}

					public class TIER8
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 8";
					}
				}
			}
		}

		public class GAMEPLAY_EVENT_INFO_SCREEN
		{
			public static LocString WHERE = "WHERE: {0}";

			public static LocString WHEN = "WHEN: {0}";
		}

		public class DEBUG_TOOLS
		{
			public static LocString ENTER_TEXT = "";

			public static LocString DEBUG_ACTIVE = "Debug tools active";

			public static LocString INVALID_LOCATION = "Invalid Location";

			public class PAINT_ELEMENTS_SCREEN
			{
				public static LocString TITLE = "CELL PAINTER";

				public static LocString ELEMENT = "Element";

				public static LocString MASS_KG = "Mass (kg)";

				public static LocString TEMPERATURE_KELVIN = "Temperature (K)";

				public static LocString DISEASE = "Disease";

				public static LocString DISEASE_COUNT = "Disease Count";

				public static LocString BUILDINGS = "Buildings:";

				public static LocString CELLS = "Cells:";

				public static LocString ADD_FOW_MASK = "Prevent FoW Reveal";

				public static LocString REMOVE_FOW_MASK = "Allow FoW Reveal";

				public static LocString PAINT = "Paint";

				public static LocString SAMPLE = "Sample";

				public static LocString STORE = "Store";

				public static LocString FILL = "Fill";

				public static LocString SPAWN_ALL = "Spawn All (Slow)";
			}

			public class SAVE_BASE_TEMPLATE
			{
				public static LocString TITLE = "Base and World Tools";

				public static LocString SAVE_TITLE = "Save Selection";

				public static LocString CLEAR_BUTTON = "Clear Floor";

				public static LocString DESTROY_BUTTON = "Destroy";

				public static LocString DECONSTRUCT_BUTTON = "Deconstruct";

				public static LocString CLEAR_SELECTION_BUTTON = "Clear Selection";

				public static LocString DEFAULT_SAVE_NAME = "TemplateSaveName";

				public static LocString MORE = "More";

				public static LocString BASE_GAME_FOLDER_NAME = "Base Game";

				public class SELECTION_INFO_PANEL
				{
					public static LocString TOTAL_MASS = "Total mass: {0}";

					public static LocString AVERAGE_MASS = "Average cell mass: {0}";

					public static LocString AVERAGE_TEMPERATURE = "Average temperature: {0}";

					public static LocString TOTAL_JOULES = "Total joules: {0}";

					public static LocString JOULES_PER_KILOGRAM = "Joules per kilogram: {0}";

					public static LocString TOTAL_RADS = "Total rads: {0}";

					public static LocString AVERAGE_RADS = "Average rads: {0}";
				}
			}
		}

		public class WORLDGEN
		{
			public static LocString NOHEADERS = "";

			public static LocString COMPLETE = "Success! Space adventure awaits.";

			public static LocString FAILED = "Goodness, has this ever gone terribly wrong!";

			public static LocString RESTARTING = "Rebooting...";

			public static LocString LOADING = "Loading world...";

			public static LocString GENERATINGWORLD = "The Galaxy Synthesizer";

			public static LocString CHOOSEWORLDSIZE = "Select the magnitude of your new galaxy.";

			public static LocString USING_PLAYER_SEED = "Using selected worldgen seed: {0}";

			public static LocString CLEARINGLEVEL = "Staring into the void...";

			public static LocString RETRYCOUNT = "Oh dear, let's try that again.";

			public static LocString GENERATESOLARSYSTEM = "Catalyzing Big Bang...";

			public static LocString GENERATESOLARSYSTEM1 = "Catalyzing Big Bang...";

			public static LocString GENERATESOLARSYSTEM2 = "Catalyzing Big Bang...";

			public static LocString GENERATESOLARSYSTEM3 = "Catalyzing Big Bang...";

			public static LocString GENERATESOLARSYSTEM4 = "Catalyzing Big Bang...";

			public static LocString GENERATESOLARSYSTEM5 = "Catalyzing Big Bang...";

			public static LocString GENERATESOLARSYSTEM6 = "Approaching event horizon...";

			public static LocString GENERATESOLARSYSTEM7 = "Approaching event horizon...";

			public static LocString GENERATESOLARSYSTEM8 = "Approaching event horizon...";

			public static LocString GENERATESOLARSYSTEM9 = "Approaching event horizon...";

			public static LocString SETUPNOISE = "BANG!";

			public static LocString BUILDNOISESOURCE = "Sorting quadrillions of atoms...";

			public static LocString BUILDNOISESOURCE1 = "Sorting quadrillions of atoms...";

			public static LocString BUILDNOISESOURCE2 = "Sorting quadrillions of atoms...";

			public static LocString BUILDNOISESOURCE3 = "Ironing the fabric of creation...";

			public static LocString BUILDNOISESOURCE4 = "Ironing the fabric of creation...";

			public static LocString BUILDNOISESOURCE5 = "Ironing the fabric of creation...";

			public static LocString BUILDNOISESOURCE6 = "Taking hot meteor shower...";

			public static LocString BUILDNOISESOURCE7 = "Tightening asteroid belts...";

			public static LocString BUILDNOISESOURCE8 = "Tightening asteroid belts...";

			public static LocString BUILDNOISESOURCE9 = "Tightening asteroid belts...";

			public static LocString GENERATENOISE = "Baking igneous rock...";

			public static LocString GENERATENOISE1 = "Multilayering sediment...";

			public static LocString GENERATENOISE2 = "Multilayering sediment...";

			public static LocString GENERATENOISE3 = "Multilayering sediment...";

			public static LocString GENERATENOISE4 = "Superheating gases...";

			public static LocString GENERATENOISE5 = "Superheating gases...";

			public static LocString GENERATENOISE6 = "Superheating gases...";

			public static LocString GENERATENOISE7 = "Vacuuming out vacuums...";

			public static LocString GENERATENOISE8 = "Vacuuming out vacuums...";

			public static LocString GENERATENOISE9 = "Vacuuming out vacuums...";

			public static LocString NORMALISENOISE = "Interpolating suffocating gas...";

			public static LocString WORLDLAYOUT = "Freezing ice formations...";

			public static LocString WORLDLAYOUT1 = "Freezing ice formations...";

			public static LocString WORLDLAYOUT2 = "Freezing ice formations...";

			public static LocString WORLDLAYOUT3 = "Freezing ice formations...";

			public static LocString WORLDLAYOUT4 = "Melting magma...";

			public static LocString WORLDLAYOUT5 = "Melting magma...";

			public static LocString WORLDLAYOUT6 = "Melting magma...";

			public static LocString WORLDLAYOUT7 = "Sprinkling sand...";

			public static LocString WORLDLAYOUT8 = "Sprinkling sand...";

			public static LocString WORLDLAYOUT9 = "Sprinkling sand...";

			public static LocString WORLDLAYOUT10 = "Sprinkling sand...";

			public static LocString COMPLETELAYOUT = "Cooling glass...";

			public static LocString COMPLETELAYOUT1 = "Cooling glass...";

			public static LocString COMPLETELAYOUT2 = "Cooling glass...";

			public static LocString COMPLETELAYOUT3 = "Cooling glass...";

			public static LocString COMPLETELAYOUT4 = "Digging holes...";

			public static LocString COMPLETELAYOUT5 = "Digging holes...";

			public static LocString COMPLETELAYOUT6 = "Digging holes...";

			public static LocString COMPLETELAYOUT7 = "Adding buckets of dirt...";

			public static LocString COMPLETELAYOUT8 = "Adding buckets of dirt...";

			public static LocString COMPLETELAYOUT9 = "Adding buckets of dirt...";

			public static LocString COMPLETELAYOUT10 = "Adding buckets of dirt...";

			public static LocString PROCESSRIVERS = "Pouring rivers...";

			public static LocString CONVERTTERRAINCELLSTOEDGES = "Hardening diamonds...";

			public static LocString PROCESSING = "Embedding metals...";

			public static LocString PROCESSING1 = "Embedding metals...";

			public static LocString PROCESSING2 = "Embedding metals...";

			public static LocString PROCESSING3 = "Burying precious ore...";

			public static LocString PROCESSING4 = "Burying precious ore...";

			public static LocString PROCESSING5 = "Burying precious ore...";

			public static LocString PROCESSING6 = "Burying precious ore...";

			public static LocString PROCESSING7 = "Excavating tunnels...";

			public static LocString PROCESSING8 = "Excavating tunnels...";

			public static LocString PROCESSING9 = "Excavating tunnels...";

			public static LocString BORDERS = "Just adding water...";

			public static LocString BORDERS1 = "Just adding water...";

			public static LocString BORDERS2 = "Staring at the void...";

			public static LocString BORDERS3 = "Staring at the void...";

			public static LocString BORDERS4 = "Staring at the void...";

			public static LocString BORDERS5 = "Avoiding awkward eye contact with the void...";

			public static LocString BORDERS6 = "Avoiding awkward eye contact with the void...";

			public static LocString BORDERS7 = "Avoiding awkward eye contact with the void...";

			public static LocString BORDERS8 = "Avoiding awkward eye contact with the void...";

			public static LocString BORDERS9 = "Avoiding awkward eye contact with the void...";

			public static LocString DRAWWORLDBORDER = "Establishing personal boundaries...";

			public static LocString PLACINGTEMPLATES = "Generating interest...";

			public static LocString SETTLESIM = "Infusing oxygen...";

			public static LocString SETTLESIM1 = "Infusing oxygen...";

			public static LocString SETTLESIM2 = "Too much oxygen. Removing...";

			public static LocString SETTLESIM3 = "Too much oxygen. Removing...";

			public static LocString SETTLESIM4 = "Ideal oxygen levels achieved...";

			public static LocString SETTLESIM5 = "Ideal oxygen levels achieved...";

			public static LocString SETTLESIM6 = "Planting space flora...";

			public static LocString SETTLESIM7 = "Planting space flora...";

			public static LocString SETTLESIM8 = "Releasing wildlife...";

			public static LocString SETTLESIM9 = "Releasing wildlife...";

			public static LocString ANALYZINGWORLD = "Shuffling DNA Blueprints...";

			public static LocString ANALYZINGWORLDCOMPLETE = "Tidying up for the Duplicants...";

			public static LocString PLACINGCREATURES = "Building the suspense...";
		}

		public class TOOLTIPS
		{
			public static LocString MANAGEMENTMENU_JOBS = string.Concat(new string[]
			{
				"Manage my Duplicant Priorities {Hotkey}\n\nDuplicant Priorities",
				UI.PST_KEYWORD,
				" are calculated <i>before</i> the ",
				UI.PRE_KEYWORD,
				"Building Priorities",
				UI.PST_KEYWORD,
				" set by the ",
				UI.FormatAsTool("Priority Tool", global::Action.Prioritize)
			});

			public static LocString MANAGEMENTMENU_CONSUMABLES = "Manage my Duplicants' diets and medications {Hotkey}";

			public static LocString MANAGEMENTMENU_VITALS = "View my Duplicants' vitals {Hotkey}";

			public static LocString MANAGEMENTMENU_RESEARCH = "View the Research Tree {Hotkey}";

			public static LocString MANAGEMENTMENU_REQUIRES_RESEARCH = string.Concat(new string[]
			{
				"Build a Research Station to unlock this menu\n\nThe ",
				BUILDINGS.PREFABS.RESEARCHCENTER.NAME,
				" can be found in the ",
				UI.FormatAsBuildMenuTab("Stations Tab", global::Action.Plan10),
				" of the Build Menu"
			});

			public static LocString MANAGEMENTMENU_DAILYREPORT = "View each cycle's Colony Report {Hotkey}";

			public static LocString MANAGEMENTMENU_CODEX = "Browse entries in my Database {Hotkey}";

			public static LocString MANAGEMENTMENU_SCHEDULE = "Adjust the colony's time usage {Hotkey}";

			public static LocString MANAGEMENTMENU_STARMAP = "Manage astronaut rocket missions {Hotkey}";

			public static LocString MANAGEMENTMENU_REQUIRES_TELESCOPE = string.Concat(new string[]
			{
				"Build a Telescope to unlock this menu\n\nThe ",
				BUILDINGS.PREFABS.TELESCOPE.NAME,
				" can be found in the ",
				UI.FormatAsBuildMenuTab("Stations Tab", global::Action.Plan10),
				" of the Build Menu"
			});

			public static LocString MANAGEMENTMENU_REQUIRES_TELESCOPE_CLUSTER = string.Concat(new string[]
			{
				"Build a Telescope to unlock this menu\n\nThe ",
				BUILDINGS.PREFABS.TELESCOPE.NAME,
				" can be found in the ",
				UI.FormatAsBuildMenuTab("Rocketry Tab", global::Action.Plan14),
				" of the Build Menu"
			});

			public static LocString MANAGEMENTMENU_SKILLS = "Manage Duplicants' Skill assignments {Hotkey}";

			public static LocString MANAGEMENTMENU_REQUIRES_SKILL_STATION = string.Concat(new string[]
			{
				"Build a Printing Pod to unlock this menu\n\nThe ",
				BUILDINGS.PREFABS.HEADQUARTERSCOMPLETE.NAME,
				" can be found in the ",
				UI.FormatAsBuildMenuTab("Base Tab", global::Action.Plan1),
				" of the Build Menu"
			});

			public static LocString MANAGEMENTMENU_PAUSEMENU = "Open the game menu {Hotkey}";

			public static LocString MANAGEMENTMENU_RESOURCES = "Open the resource management screen {Hotkey}";

			public static LocString OPEN_CODEX_ENTRY = "View full entry in database";

			public static LocString NO_CODEX_ENTRY = "No database entry available";

			public static LocString CHANGE_OUTFIT = "Change this Duplicant's outfit";

			public static LocString METERSCREEN_AVGSTRESS = "Highest Stress: {0}";

			public static LocString METERSCREEN_MEALHISTORY = "Calories Available: {0}";

			public static LocString METERSCREEN_POPULATION = "Population: {0}";

			public static LocString METERSCREEN_POPULATION_CLUSTER = UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD + " Population: {1}\nTotal Population: {2}";

			public static LocString METERSCREEN_SICK_DUPES = "Sick Duplicants: {0}";

			public static LocString METERSCREEN_INVALID_FOOD_TYPE = "Invalid Food Type: {0}";

			public static LocString PLAYBUTTON = "Start";

			public static LocString PAUSEBUTTON = "Pause";

			public static LocString PAUSE = "Pause {Hotkey}";

			public static LocString UNPAUSE = "Unpause {Hotkey}";

			public static LocString SPEEDBUTTON_SLOW = "Slow speed {Hotkey}";

			public static LocString SPEEDBUTTON_MEDIUM = "Medium speed {Hotkey}";

			public static LocString SPEEDBUTTON_FAST = "Fast speed {Hotkey}";

			public static LocString RED_ALERT_TITLE = "Toggle Red Alert";

			public static LocString RED_ALERT_CONTENT = "Duplicants will work, ignoring schedules and their basic needs\n\nUse in case of emergency";

			public static LocString DISINFECTBUTTON = "Disinfect buildings {Hotkey}";

			public static LocString MOPBUTTON = "Mop liquid spills {Hotkey}";

			public static LocString DIGBUTTON = "Set dig errands {Hotkey}";

			public static LocString CANCELBUTTON = "Cancel errands {Hotkey}";

			public static LocString DECONSTRUCTBUTTON = "Demolish buildings {Hotkey}";

			public static LocString ATTACKBUTTON = "Attack poor, wild critters {Hotkey}";

			public static LocString CAPTUREBUTTON = "Capture critters {Hotkey}";

			public static LocString CLEARBUTTON = "Move debris into storage {Hotkey}";

			public static LocString HARVESTBUTTON = "Harvest plants {Hotkey}";

			public static LocString PRIORITIZEMAINBUTTON = "";

			public static LocString PRIORITIZEBUTTON = string.Concat(new string[]
			{
				"Set Building Priority {Hotkey}\n\nDuplicant Priorities",
				UI.PST_KEYWORD,
				" ",
				UI.FormatAsHotKey(global::Action.ManagePriorities),
				" are calculated <i>before</i> the ",
				UI.PRE_KEYWORD,
				"Building Priorities",
				UI.PST_KEYWORD,
				" set by this tool"
			});

			public static LocString CLEANUPMAINBUTTON = "Mop and sweep messy floors {Hotkey}";

			public static LocString CANCELDECONSTRUCTIONBUTTON = "Cancel queued orders or deconstruct existing buildings {Hotkey}";

			public static LocString HELP_ROTATE_KEY = "Press " + UI.FormatAsHotKey(global::Action.RotateBuilding) + " to Rotate";

			public static LocString HELP_BUILDLOCATION_INVALID_CELL = "Invalid Cell";

			public static LocString HELP_BUILDLOCATION_MISSING_TELEPAD = "World has no " + BUILDINGS.PREFABS.HEADQUARTERSCOMPLETE.NAME + " or " + BUILDINGS.PREFABS.EXOBASEHEADQUARTERS.NAME;

			public static LocString HELP_BUILDLOCATION_FLOOR = "Must be built on solid ground";

			public static LocString HELP_BUILDLOCATION_WALL = "Must be built against a wall";

			public static LocString HELP_BUILDLOCATION_FLOOR_OR_ATTACHPOINT = "Must be built on solid ground or overlapping an {0}";

			public static LocString HELP_BUILDLOCATION_OCCUPIED = "Must be built in unoccupied space";

			public static LocString HELP_BUILDLOCATION_CEILING = "Must be built on the ceiling";

			public static LocString HELP_BUILDLOCATION_INSIDEGROUND = "Must be built in the ground";

			public static LocString HELP_BUILDLOCATION_ATTACHPOINT = "Must be built overlapping a {0}";

			public static LocString HELP_BUILDLOCATION_SPACE = "Must be built on the surface in space";

			public static LocString HELP_BUILDLOCATION_CORNER = "Must be built in a corner";

			public static LocString HELP_BUILDLOCATION_CORNER_FLOOR = "Must be built in a corner on the ground";

			public static LocString HELP_BUILDLOCATION_BELOWROCKETCEILING = "Must be placed further from the edge of space";

			public static LocString HELP_BUILDLOCATION_ONROCKETENVELOPE = "Must be built on the interior wall of a rocket";

			public static LocString HELP_BUILDLOCATION_LIQUID_CONDUIT_FORBIDDEN = "Obstructed by a building";

			public static LocString HELP_BUILDLOCATION_NOT_IN_TILES = "Cannot be built inside tile";

			public static LocString HELP_BUILDLOCATION_GASPORTS_OVERLAP = "Gas ports cannot overlap";

			public static LocString HELP_BUILDLOCATION_LIQUIDPORTS_OVERLAP = "Liquid ports cannot overlap";

			public static LocString HELP_BUILDLOCATION_SOLIDPORTS_OVERLAP = "Solid ports cannot overlap";

			public static LocString HELP_BUILDLOCATION_LOGIC_PORTS_OBSTRUCTED = "Automation ports cannot overlap";

			public static LocString HELP_BUILDLOCATION_WIRECONNECTORS_OVERLAP = "Power connectors cannot overlap";

			public static LocString HELP_BUILDLOCATION_HIGHWATT_NOT_IN_TILE = "Heavi-Watt connectors cannot be built inside tile";

			public static LocString HELP_BUILDLOCATION_WIRE_OBSTRUCTION = "Obstructed by Heavi-Watt Wire";

			public static LocString HELP_BUILDLOCATION_BACK_WALL = "Obstructed by back wall";

			public static LocString HELP_TUBELOCATION_NO_UTURNS = "Can't U-Turn";

			public static LocString HELP_TUBELOCATION_STRAIGHT_BRIDGES = "Can't Turn Here";

			public static LocString HELP_REQUIRES_ROOM = "Must be in a " + UI.PRE_KEYWORD + "Room" + UI.PST_KEYWORD;

			public static LocString OXYGENOVERLAYSTRING = "Displays ambient oxygen density {Hotkey}";

			public static LocString POWEROVERLAYSTRING = "Displays power grid components {Hotkey}";

			public static LocString TEMPERATUREOVERLAYSTRING = "Displays ambient temperature {Hotkey}";

			public static LocString HEATFLOWOVERLAYSTRING = "Displays comfortable temperatures for Duplicants {Hotkey}";

			public static LocString SUITOVERLAYSTRING = "Displays Exosuits and related buildings {Hotkey}";

			public static LocString LOGICOVERLAYSTRING = "Displays automation grid components {Hotkey}";

			public static LocString ROOMSOVERLAYSTRING = "Displays special purpose rooms and bonuses {Hotkey}";

			public static LocString JOULESOVERLAYSTRING = "Displays the thermal energy in each cell";

			public static LocString LIGHTSOVERLAYSTRING = "Displays the visibility radius of light sources {Hotkey}";

			public static LocString LIQUIDVENTOVERLAYSTRING = "Displays liquid pipe system components {Hotkey}";

			public static LocString GASVENTOVERLAYSTRING = "Displays gas pipe system components {Hotkey}";

			public static LocString DECOROVERLAYSTRING = "Displays areas with Morale-boosting decor values {Hotkey}";

			public static LocString PRIORITIESOVERLAYSTRING = "Displays work priority values {Hotkey}";

			public static LocString DISEASEOVERLAYSTRING = "Displays areas of disease risk {Hotkey}";

			public static LocString NOISE_POLLUTION_OVERLAY_STRING = "Displays ambient noise levels {Hotkey}";

			public static LocString CROPS_OVERLAY_STRING = "Displays plant growth progress {Hotkey}";

			public static LocString CONVEYOR_OVERLAY_STRING = "Displays conveyor transport components {Hotkey}";

			public static LocString TILEMODE_OVERLAY_STRING = "Displays material information {Hotkey}";

			public static LocString REACHABILITYOVERLAYSTRING = "Displays areas accessible by Duplicants";

			public static LocString RADIATIONOVERLAYSTRING = "Displays radiation levels {Hotkey}";

			public static LocString ENERGYREQUIRED = UI.FormatAsLink("Power", "POWER") + " Required";

			public static LocString ENERGYGENERATED = UI.FormatAsLink("Power", "POWER") + " Produced";

			public static LocString INFOPANEL = "The Info Panel contains an overview of the basic information about my Duplicant";

			public static LocString VITALSPANEL = "The Vitals Panel monitors the status and well being of my Duplicant";

			public static LocString STRESSPANEL = "The Stress Panel offers a detailed look at what is affecting my Duplicant psychologically";

			public static LocString STATSPANEL = "The Stats Panel gives me an overview of my Duplicant's individual stats";

			public static LocString ITEMSPANEL = "The Items Panel displays everything this Duplicant is in possession of";

			public static LocString STRESSDESCRIPTION = string.Concat(new string[]
			{
				"Accommodate my Duplicant's needs to manage their ",
				UI.FormatAsLink("Stress", "STRESS"),
				".\n\nLow ",
				UI.FormatAsLink("Stress", "STRESS"),
				" can provide a productivity boost, while high ",
				UI.FormatAsLink("Stress", "STRESS"),
				" can impair production or even lead to a nervous breakdown."
			});

			public static LocString ALERTSTOOLTIP = "Alerts provide important information about what's happening in the colony right now";

			public static LocString MESSAGESTOOLTIP = "Messages are events that have happened and tips to help me manage my colony";

			public static LocString NEXTMESSAGESTOOLTIP = "Next message";

			public static LocString CLOSETOOLTIP = "Close";

			public static LocString DISMISSMESSAGE = "Dismiss message";

			public static LocString RECIPE_QUEUE = "Queue {0} for continuous fabrication";

			public static LocString RED_ALERT_BUTTON_ON = "Enable Red Alert";

			public static LocString RED_ALERT_BUTTON_OFF = "Disable Red Alert";

			public static LocString JOBSSCREEN_PRIORITY = "High priority tasks are always performed before low priority tasks.\n\nHowever, a busy Duplicant will continue to work on their current work errand until it's complete, even if a more important errand becomes available.";

			public static LocString JOBSSCREEN_ATTRIBUTES = "The following attributes affect a Duplicant's efficiency at this errand:";

			public static LocString JOBSSCREEN_CANNOTPERFORMTASK = "{0} cannot perform this errand.";

			public static LocString JOBSSCREEN_RELEVANT_ATTRIBUTES = "Relevant Attributes:";

			public static LocString SORTCOLUMN = UI.CLICK(UI.ClickType.Click) + " to sort";

			public static LocString NOMATERIAL = "Not enough materials";

			public static LocString SELECTAMATERIAL = "There are insufficient materials to construct this building";

			public static LocString EDITNAME = "Give this Duplicant a new name";

			public static LocString RANDOMIZENAME = "Randomize this Duplicant's name";

			public static LocString EDITNAMEGENERIC = "Rename {0}";

			public static LocString EDITNAMEROCKET = "Rename this rocket";

			public static LocString BASE_VALUE = "Base Value";

			public static LocString MATIERIAL_MOD = "Made out of {0}";

			public static LocString VITALS_CHECKBOX_TEMPERATURE = string.Concat(new string[]
			{
				"This plant's internal ",
				UI.PRE_KEYWORD,
				"Temperature",
				UI.PST_KEYWORD,
				" is <b>{temperature}</b>"
			});

			public static LocString VITALS_CHECKBOX_PRESSURE = string.Concat(new string[]
			{
				"The current ",
				UI.PRE_KEYWORD,
				"Gas",
				UI.PST_KEYWORD,
				" pressure is <b>{pressure}</b>"
			});

			public static LocString VITALS_CHECKBOX_ATMOSPHERE = "This plant is immersed in {element}";

			public static LocString VITALS_CHECKBOX_ILLUMINATION_DARK = "This plant is currently in the dark";

			public static LocString VITALS_CHECKBOX_ILLUMINATION_LIGHT = "This plant is currently lit";

			public static LocString VITALS_CHECKBOX_FERTILIZER = string.Concat(new string[]
			{
				"<b>{mass}</b> of ",
				UI.PRE_KEYWORD,
				"Fertilizer",
				UI.PST_KEYWORD,
				" is currently available"
			});

			public static LocString VITALS_CHECKBOX_IRRIGATION = string.Concat(new string[]
			{
				"<b>{mass}</b> of ",
				UI.PRE_KEYWORD,
				"Liquid",
				UI.PST_KEYWORD,
				" is currently available"
			});

			public static LocString VITALS_CHECKBOX_SUBMERGED_TRUE = "This plant is fully submerged in " + UI.PRE_KEYWORD + "Liquid" + UI.PRE_KEYWORD;

			public static LocString VITALS_CHECKBOX_SUBMERGED_FALSE = "This plant must be submerged in " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD;

			public static LocString VITALS_CHECKBOX_DROWNING_TRUE = "This plant is not drowning";

			public static LocString VITALS_CHECKBOX_DROWNING_FALSE = "This plant is drowning in " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD;

			public static LocString VITALS_CHECKBOX_RECEPTACLE_OPERATIONAL = "This plant is housed in an operational farm plot";

			public static LocString VITALS_CHECKBOX_RECEPTACLE_INOPERATIONAL = "This plant is not housed in an operational farm plot";

			public static LocString VITALS_CHECKBOX_RADIATION = string.Concat(new string[]
			{
				"This plant is sitting in <b>{rads}</b> of ambient ",
				UI.PRE_KEYWORD,
				"Radiation",
				UI.PST_KEYWORD,
				". It needs at between {minRads} and {maxRads} to grow"
			});

			public static LocString VITALS_CHECKBOX_RADIATION_NO_MIN = string.Concat(new string[]
			{
				"This plant is sitting in <b>{rads}</b> of ambient ",
				UI.PRE_KEYWORD,
				"Radiation",
				UI.PST_KEYWORD,
				". It needs less than {maxRads} to grow"
			});
		}

		public class CLUSTERMAP
		{
			public static LocString PLANETOID = "Planetoid";

			public static LocString PLANETOID_KEYWORD = UI.PRE_KEYWORD + UI.CLUSTERMAP.PLANETOID + UI.PST_KEYWORD;

			public static LocString TITLE = "STARMAP";

			public static LocString LANDING_SITES = "LANDING SITES";

			public static LocString DESTINATION = "DESTINATION";

			public static LocString OCCUPANTS = "CREW";

			public static LocString ELEMENTS = "ELEMENTS";

			public static LocString UNKNOWN_DESTINATION = "Unknown";

			public static LocString TILES = "Tiles";

			public static LocString TILES_PER_CYCLE = "Tiles per cycle";

			public static LocString CHANGE_DESTINATION = UI.CLICK(UI.ClickType.Click) + " to change destination";

			public static LocString SELECT_DESTINATION = "Select a new destination on the map";

			public static LocString TOOLTIP_INVALID_DESTINATION_FOG_OF_WAR = "Rockets cannot travel to this hex until it has been analyzed\n\nSpace can be analyzed with a " + BUILDINGS.PREFABS.CLUSTERTELESCOPE.NAME + " or " + BUILDINGS.PREFABS.SCANNERMODULE.NAME;

			public static LocString TOOLTIP_INVALID_DESTINATION_NO_PATH = string.Concat(new string[]
			{
				"There is no navigable rocket path to this ",
				UI.CLUSTERMAP.PLANETOID_KEYWORD,
				"\n\nSpace can be analyzed with a ",
				BUILDINGS.PREFABS.CLUSTERTELESCOPE.NAME,
				" or ",
				BUILDINGS.PREFABS.SCANNERMODULE.NAME,
				" to clear the way"
			});

			public static LocString TOOLTIP_INVALID_DESTINATION_NO_LAUNCH_PAD = string.Concat(new string[]
			{
				"There is no ",
				BUILDINGS.PREFABS.LAUNCHPAD.NAME,
				" on this ",
				UI.CLUSTERMAP.PLANETOID_KEYWORD,
				" for a rocket to land on\n\nUse a ",
				BUILDINGS.PREFABS.PIONEERMODULE.NAME,
				" or ",
				BUILDINGS.PREFABS.SCOUTMODULE.NAME,
				" to deploy a scout and make first contact"
			});

			public static LocString TOOLTIP_INVALID_DESTINATION_REQUIRE_ASTEROID = "Must select a " + UI.CLUSTERMAP.PLANETOID_KEYWORD + " destination";

			public static LocString TOOLTIP_INVALID_DESTINATION_OUT_OF_RANGE = "This destination is further away than the rocket's maximum range of {0}.";

			public static LocString TOOLTIP_HIDDEN_HEX = "???";

			public static LocString TOOLTIP_PEEKED_HEX_WITH_OBJECT = "UNKNOWN OBJECT DETECTED!";

			public static LocString TOOLTIP_EMPTY_HEX = "EMPTY SPACE";

			public static LocString TOOLTIP_PATH_LENGTH = "Trip Distance: {0}/{1}";

			public static LocString TOOLTIP_PATH_LENGTH_RETURN = "Trip Distance: {0}/{1} (Return Trip)";

			public class STATUS
			{
				public static LocString NORMAL = "Normal";

				public class ROCKET
				{
					public static LocString GROUNDED = "Normal";

					public static LocString TRAVELING = "Traveling";

					public static LocString STRANDED = "Stranded";

					public static LocString IDLE = "Idle";
				}
			}

			public class ASTEROIDS
			{
				public class ELEMENT_AMOUNTS
				{
					public static LocString LOTS = "Plentiful";

					public static LocString SOME = "Significant amount";

					public static LocString LITTLE = "Small amount";

					public static LocString VERY_LITTLE = "Trace amount";
				}

				public class SURFACE_CONDITIONS
				{
					public static LocString LIGHT = "Peak Light";

					public static LocString RADIATION = "Cosmic Radiation";
				}
			}

			public class POI
			{
				public static LocString TITLE = "POINT OF INTEREST";

				public static LocString MASS_REMAINING = "<b>Total Mass Remaining</b>";

				public static LocString ROCKETS_AT_THIS_LOCATION = "<b>Rockets at this location</b>";

				public static LocString ARTIFACTS = "Artifact";

				public static LocString ARTIFACTS_AVAILABLE = "Available";

				public static LocString ARTIFACTS_DEPLETED = "Collected\nRecharge: {0}";
			}

			public class ROCKETS
			{
				public class SPEED
				{
					public static LocString NAME = "Rocket Speed: ";

					public static LocString TOOLTIP = "<b>Rocket Speed</b> is calculated by dividing <b>Engine Power</b> by <b>Burden</b>.\n\nRockets operating on autopilot will have a reduced speed.\n\nRocket speed can be further increased by the skill of the Duplicant flying the rocket.";
				}

				public class FUEL_REMAINING
				{
					public static LocString NAME = "Fuel Remaining: ";

					public static LocString TOOLTIP = "This rocket has {0} fuel in its tank";
				}

				public class OXIDIZER_REMAINING
				{
					public static LocString NAME = "Oxidizer Power Remaining: ";

					public static LocString TOOLTIP = "This rocket has enough oxidizer in its tank for {0} of fuel";
				}

				public class RANGE
				{
					public static LocString NAME = "Range Remaining: ";

					public static LocString TOOLTIP = "<b>Range remaining</b> is calculated by dividing the lesser of <b>fuel remaining</b> and <b>oxidizer power remaining</b> by <b>fuel consumed per tile</b>";
				}

				public class FUEL_PER_HEX
				{
					public static LocString NAME = "Fuel consumed per Tile: {0}";

					public static LocString TOOLTIP = "This rocket can travel one tile per {0} of fuel";
				}

				public class BURDEN_TOTAL
				{
					public static LocString NAME = "Rocket burden: ";

					public static LocString TOOLTIP = "The combined burden of all the modules in this rocket";
				}

				public class BURDEN_MODULE
				{
					public static LocString NAME = "Module Burden: ";

					public static LocString TOOLTIP = "The selected module adds {0} to the rocket's total " + DUPLICANTS.ATTRIBUTES.ROCKETBURDEN.NAME;
				}

				public class POWER_TOTAL
				{
					public static LocString NAME = "Rocket engine power: ";

					public static LocString TOOLTIP = "The total engine power added by all the modules in this rocket";
				}

				public class POWER_MODULE
				{
					public static LocString NAME = "Module Engine Power: ";

					public static LocString TOOLTIP = "The selected module adds {0} to the rocket's total " + DUPLICANTS.ATTRIBUTES.ROCKETENGINEPOWER.NAME;
				}

				public class MODULE_STATS
				{
					public static LocString NAME = "Module Stats: ";

					public static LocString NAME_HEADER = "Module Stats";

					public static LocString TOOLTIP = "Properties of the selected module";
				}

				public class MAX_MODULES
				{
					public static LocString NAME = "Max Modules: ";

					public static LocString TOOLTIP = "The {0} can support {1} rocket modules, plus itself";
				}

				public class MAX_HEIGHT
				{
					public static LocString NAME = "Height: {0}/{1}";

					public static LocString NAME_RAW = "Height: ";

					public static LocString NAME_MAX_SUPPORTED = "Maximum supported rocket height: ";

					public static LocString TOOLTIP = "The {0} can support a total rocket height {1}";
				}

				public class ARTIFACT_MODULE
				{
					public static LocString EMPTY = "Empty";
				}
			}
		}

		public class STARMAP
		{
			public static LocString TITLE = "STARMAP";

			public static LocString MANAGEMENT_BUTTON = "STARMAP";

			public static LocString SUBROW = "•  {0}";

			public static LocString UNKNOWN_DESTINATION = "Destination Unknown";

			public static LocString ANALYSIS_AMOUNT = "Analysis {0} Complete";

			public static LocString ANALYSIS_COMPLETE = "ANALYSIS COMPLETE";

			public static LocString NO_ANALYZABLE_DESTINATION_SELECTED = "No destination selected";

			public static LocString UNKNOWN_TYPE = "Type Unknown";

			public static LocString DISTANCE = "{0} km";

			public static LocString MODULE_MASS = "+ {0} t";

			public static LocString MODULE_STORAGE = "{0} / {1}";

			public static LocString ANALYSIS_DESCRIPTION = "Use a Telescope to analyze space destinations.\n\nCompleting analysis on an object will unlock rocket missions to that destination.";

			public static LocString RESEARCH_DESCRIPTION = "Gather Interstellar Research Data using Research Modules.";

			public static LocString ROCKET_RENAME_BUTTON_TOOLTIP = "Rename this rocket";

			public static LocString NO_ROCKETS_HELP_TEXT = "Rockets allow you to visit nearby celestial bodies.\n\nEach rocket must have a Command Module, an Engine, and Fuel.\n\nYou can also carry other modules that allow you to gather specific resources from the places you visit.\n\nRemember the more weight a rocket has, the more limited it'll be on the distance it can travel. You can add more fuel to fix that, but fuel will add weight as well.";

			public static LocString CONTAINER_REQUIRED = "{0} installation required to retrieve material";

			public static LocString CAN_CARRY_ELEMENT = "Gathered by: {1}";

			public static LocString CANT_CARRY_ELEMENT = "{0} installation required to retrieve material";

			public static LocString STATUS = "SELECTED";

			public static LocString DISTANCE_OVERLAY = "TOO FAR FOR THIS ROCKET";

			public static LocString COMPOSITION_UNDISCOVERED = "?????????";

			public static LocString COMPOSITION_UNDISCOVERED_TOOLTIP = "Further research required to identify resource\n\nSend a Research Module to this destination for more information";

			public static LocString COMPOSITION_UNDISCOVERED_AMOUNT = "???";

			public static LocString COMPOSITION_SMALL_AMOUNT = "Trace Amount";

			public static LocString CURRENT_MASS = "Current Mass";

			public static LocString CURRENT_MASS_TOOLTIP = "Warning: Missions to this destination will not return a full cargo load to avoid depleting the destination for future explorations\n\nDestination: {0} Resources Available\nRocket Capacity: {1}";

			public static LocString MAXIMUM_MASS = "Maximum Mass";

			public static LocString MINIMUM_MASS = "Minimum Mass";

			public static LocString MINIMUM_MASS_TOOLTIP = "This destination must retain at least this much mass in order to prevent depletion and allow the future regeneration of resources.\n\nDuplicants will always maintain a destination's minimum mass requirements, potentially returning with less cargo than their rocket can hold";

			public static LocString REPLENISH_RATE = "Replenished/Cycle:";

			public static LocString REPLENISH_RATE_TOOLTIP = "The rate at which this destination regenerates resources";

			public static LocString ROCKETLIST = "Rocket Hangar";

			public static LocString NO_ROCKETS_TITLE = "NO ROCKETS";

			public static LocString ROCKET_COUNT = "ROCKETS: {0}";

			public static LocString LAUNCH_MISSION = "LAUNCH MISSION";

			public static LocString CANT_LAUNCH_MISSION = "CANNOT LAUNCH";

			public static LocString LAUNCH_ROCKET = "Launch Rocket";

			public static LocString LAND_ROCKET = "Land Rocket";

			public static LocString SEE_ROCKETS_LIST = "See Rockets List";

			public static LocString DEFAULT_NAME = "Rocket";

			public static LocString ANALYZE_DESTINATION = "ANALYZE OBJECT";

			public static LocString SUSPEND_DESTINATION_ANALYSIS = "PAUSE ANALYSIS";

			public static LocString DESTINATIONTITLE = "Destination Status";

			public class DESTINATIONSTUDY
			{
				public static LocString UPPERATMO = "Study upper atmosphere";

				public static LocString LOWERATMO = "Study lower atmosphere";

				public static LocString MAGNETICFIELD = "Study magnetic field";

				public static LocString SURFACE = "Study surface";

				public static LocString SUBSURFACE = "Study subsurface";
			}

			public class COMPONENT
			{
				public static LocString FUEL_TANK = "Fuel Tank";

				public static LocString ROCKET_ENGINE = "Rocket Engine";

				public static LocString CARGO_BAY = "Cargo Bay";

				public static LocString OXIDIZER_TANK = "Oxidizer Tank";
			}

			public class MISSION_STATUS
			{
				public static LocString GROUNDED = "Grounded";

				public static LocString LAUNCHING = "Launching";

				public static LocString WAITING_TO_LAND = "Waiting To Land";

				public static LocString LANDING = "Landing";

				public static LocString UNDERWAY = "Underway";

				public static LocString UNDERWAY_BOOSTED = "Underway <color=#5FDB37FF>(Boosted)</color>";

				public static LocString DESTROYED = "Destroyed";

				public static LocString GO = "ALL SYSTEMS GO";
			}

			public class LISTTITLES
			{
				public static LocString MISSIONSTATUS = "Mission Status";

				public static LocString LAUNCHCHECKLIST = "Launch Checklist";

				public static LocString MAXRANGE = "Max Range";

				public static LocString MASS = "Mass";

				public static LocString STORAGE = "Storage";

				public static LocString FUEL = "Fuel";

				public static LocString OXIDIZER = "Oxidizer";

				public static LocString PASSENGERS = "Passengers";

				public static LocString RESEARCH = "Research";

				public static LocString ARTIFACTS = "Artifacts";

				public static LocString ANALYSIS = "Analysis";

				public static LocString WORLDCOMPOSITION = "World Composition";

				public static LocString RESOURCES = "Resources";

				public static LocString MODULES = "Modules";

				public static LocString TYPE = "Type";

				public static LocString DISTANCE = "Distance";

				public static LocString DESTINATION_MASS = "World Mass Available";

				public static LocString STORAGECAPACITY = "Storage Capacity";
			}

			public class ROCKETWEIGHT
			{
				public static LocString MASS = "Mass: ";

				public static LocString MASSPENALTY = "Mass Penalty: ";

				public static LocString CURRENTMASS = "Current Rocket Mass: ";

				public static LocString CURRENTMASSPENALTY = "Current Weight Penalty: ";
			}

			public class DESTINATIONSELECTION
			{
				public static LocString REACHABLE = "Destination set";

				public static LocString UNREACHABLE = "Destination set";

				public static LocString NOTSELECTED = "Destination set";
			}

			public class DESTINATIONSELECTION_TOOLTIP
			{
				public static LocString REACHABLE = "Viable destination selected, ready for launch";

				public static LocString UNREACHABLE = "The selected destination is beyond rocket reach";

				public static LocString NOTSELECTED = "Select the rocket's Command Module to set a destination";
			}

			public class HASFOOD
			{
				public static LocString NAME = "Food Loaded";

				public static LocString TOOLTIP = "Sufficient food stores have been loaded, ready for launch";
			}

			public class HASSUIT
			{
				public static LocString NAME = "Has " + EQUIPMENT.PREFABS.ATMO_SUIT.NAME;

				public static LocString TOOLTIP = "An " + EQUIPMENT.PREFABS.ATMO_SUIT.NAME + " has been loaded";
			}

			public class NOSUIT
			{
				public static LocString NAME = "Missing " + EQUIPMENT.PREFABS.ATMO_SUIT.NAME;

				public static LocString TOOLTIP = "Rocket cannot launch without an " + EQUIPMENT.PREFABS.ATMO_SUIT.NAME + " loaded";
			}

			public class NOFOOD
			{
				public static LocString NAME = "Insufficient Food";

				public static LocString TOOLTIP = "Rocket cannot launch without adequate food stores for passengers";
			}

			public class CARGOEMPTY
			{
				public static LocString NAME = "Emptied Cargo Bay";

				public static LocString TOOLTIP = "Cargo Bays must be emptied of all materials before launch";
			}

			public class LAUNCHCHECKLIST
			{
				public static LocString ASTRONAUT_TITLE = "Astronaut";

				public static LocString HASASTRONAUT = "Astronaut ready for liftoff";

				public static LocString ASTRONAUGHT = "No Astronaut assigned";

				public static LocString INSTALLED = "Installed";

				public static LocString INSTALLED_TOOLTIP = "A suitable {0} has been installed";

				public static LocString REQUIRED = "Required";

				public static LocString REQUIRED_TOOLTIP = "A {0} must be installed before launch";

				public static LocString MISSING_TOOLTIP = "No {0} installed\n\nThis rocket cannot launch without a completed {0}";

				public static LocString NO_DESTINATION = "No destination selected";

				public static LocString MINIMUM_MASS = "Resources available {0}";

				public static LocString RESOURCE_MASS_TOOLTIP = "{0} has {1} resources available\nThis rocket has capacity for {2}";

				public static LocString INSUFFICENT_MASS_TOOLTIP = "Launching to this destination will not return a full cargo load";

				public class CONSTRUCTION_COMPLETE
				{
					public class STATUS
					{
						public static LocString READY = "No active construction";

						public static LocString FAILURE = "No active construction";

						public static LocString WARNING = "No active construction";
					}

					public class TOOLTIP
					{
						public static LocString READY = "Construction of all modules is complete";

						public static LocString FAILURE = "In-progress module construction is preventing takeoff";

						public static LocString WARNING = "Construction warning";
					}
				}

				public class PILOT_BOARDED
				{
					public static LocString READY = "Pilot boarded";

					public static LocString FAILURE = "Pilot boarded";

					public static LocString WARNING = "Pilot boarded";

					public class TOOLTIP
					{
						public static LocString READY = "A Duplicant with the " + DUPLICANTS.ROLES.ROCKETPILOT.NAME + " skill is currently onboard";

						public static LocString FAILURE = "At least one crew member aboard the rocket must possess the " + DUPLICANTS.ROLES.ROCKETPILOT.NAME + " skill to launch\n\nQualified Duplicants must be assigned to the rocket crew, and have access to the module's hatch";

						public static LocString WARNING = "Pilot warning";
					}
				}

				public class CREW_BOARDED
				{
					public static LocString READY = "All crew boarded";

					public static LocString FAILURE = "All crew boarded";

					public static LocString WARNING = "All crew boarded";

					public class TOOLTIP
					{
						public static LocString READY = "All Duplicants assigned to the rocket crew are boarded and ready for launch\n\n    • {0}/{1} Boarded";

						public static LocString FAILURE = "No crew members have boarded this rocket\n\nDuplicants must be assigned to the rocket crew and have access to the module's hatch to board\n\n    • {0}/{1} Boarded";

						public static LocString WARNING = "Some Duplicants assigned to this rocket crew have not yet boarded\n    • {0}/{1} Boarded";

						public static LocString NONE = "There are no Duplicants assigned to this rocket crew\n    • {0}/{1} Boarded";
					}
				}

				public class NO_EXTRA_PASSENGERS
				{
					public static LocString READY = "Non-crew exited";

					public static LocString FAILURE = "Non-crew exited";

					public static LocString WARNING = "Non-crew exited";

					public class TOOLTIP
					{
						public static LocString READY = "All non-crew Duplicants have disembarked";

						public static LocString FAILURE = "Non-crew Duplicants must exit the rocket before launch";

						public static LocString WARNING = "Non-crew warning";
					}
				}

				public class FLIGHT_PATH_CLEAR
				{
					public class STATUS
					{
						public static LocString READY = "Clear launch path";

						public static LocString FAILURE = "Clear launch path";

						public static LocString WARNING = "Clear launch path";
					}

					public class TOOLTIP
					{
						public static LocString READY = "The rocket's launch path is clear for takeoff";

						public static LocString FAILURE = "This rocket does not have a clear line of sight to space, preventing launch\n\nThe rocket's launch path can be cleared by excavating undug tiles and deconstructing any buildings above the rocket";

						public static LocString WARNING = "";
					}
				}

				public class HAS_FUEL_TANK
				{
					public class STATUS
					{
						public static LocString READY = "Fuel Tank";

						public static LocString FAILURE = "Fuel Tank";

						public static LocString WARNING = "Fuel Tank";
					}

					public class TOOLTIP
					{
						public static LocString READY = "A fuel tank has been installed";

						public static LocString FAILURE = "No fuel tank installed\n\nThis rocket cannot launch without a completed fuel tank";

						public static LocString WARNING = "Fuel tank warning";
					}
				}

				public class HAS_ENGINE
				{
					public class STATUS
					{
						public static LocString READY = "Engine";

						public static LocString FAILURE = "Engine";

						public static LocString WARNING = "Engine";
					}

					public class TOOLTIP
					{
						public static LocString READY = "A suitable engine has been installed";

						public static LocString FAILURE = "No engine installed\n\nThis rocket cannot launch without a completed engine";

						public static LocString WARNING = "Engine warning";
					}
				}

				public class HAS_NOSECONE
				{
					public class STATUS
					{
						public static LocString READY = "Nosecone";

						public static LocString FAILURE = "Nosecone";

						public static LocString WARNING = "Nosecone";
					}

					public class TOOLTIP
					{
						public static LocString READY = "A suitable nosecone has been installed";

						public static LocString FAILURE = "No nosecone installed\n\nThis rocket cannot launch without a completed nosecone";

						public static LocString WARNING = "Nosecone warning";
					}
				}

				public class HAS_CONTROLSTATION
				{
					public class STATUS
					{
						public static LocString READY = "Control Station";

						public static LocString FAILURE = "Control Station";

						public static LocString WARNING = "Control Station";
					}

					public class TOOLTIP
					{
						public static LocString READY = "The control station is installed and waiting for the pilot";

						public static LocString FAILURE = "No Control Station\n\nA new Rocket Control Station must be installed inside the rocket";

						public static LocString WARNING = "Control Station warning";
					}
				}

				public class LOADING_COMPLETE
				{
					public class STATUS
					{
						public static LocString READY = "Cargo Loading Complete";

						public static LocString FAILURE = "";

						public static LocString WARNING = "Cargo Loading Complete";
					}

					public class TOOLTIP
					{
						public static LocString READY = "All possible loading and unloading has been completed";

						public static LocString FAILURE = "";

						public static LocString WARNING = "The " + BUILDINGS.PREFABS.LAUNCHPAD.NAME + " could still transfer cargo to or from this rocket";
					}
				}

				public class CARGO_TRANSFER_COMPLETE
				{
					public class STATUS
					{
						public static LocString READY = "Cargo Transfer Complete";

						public static LocString FAILURE = "";

						public static LocString WARNING = "Cargo Transfer Complete";
					}

					public class TOOLTIP
					{
						public static LocString READY = "All possible loading and unloading has been completed";

						public static LocString FAILURE = "";

						public static LocString WARNING = "The " + BUILDINGS.PREFABS.LAUNCHPAD.NAME + " could still transfer cargo to or from this rocket";
					}
				}

				public class INTERNAL_CONSTRUCTION_COMPLETE
				{
					public class STATUS
					{
						public static LocString READY = "Landers Ready";

						public static LocString FAILURE = "Landers Ready";

						public static LocString WARNING = "";
					}

					public class TOOLTIP
					{
						public static LocString READY = "All requested landers have been built and are ready for deployment";

						public static LocString FAILURE = "Additional landers must be constructed to fulfill the lander requests of this rocket";

						public static LocString WARNING = "";
					}
				}

				public class MAX_MODULES
				{
					public class STATUS
					{
						public static LocString READY = "Module limit";

						public static LocString FAILURE = "Module limit";

						public static LocString WARNING = "Module limit";
					}

					public class TOOLTIP
					{
						public static LocString READY = "The rocket's engine can support the number of installed rocket modules";

						public static LocString FAILURE = "The number of installed modules exceeds the engine's module limit\n\nExcess modules must be removed";

						public static LocString WARNING = "Module limit warning";
					}
				}

				public class HAS_RESOURCE
				{
					public class STATUS
					{
						public static LocString READY = "{0} {1} supplied";

						public static LocString FAILURE = "{0} missing {1}";

						public static LocString WARNING = "{0} missing {1}";
					}

					public class TOOLTIP
					{
						public static LocString READY = "{0} {1} supplied";

						public static LocString FAILURE = "{0} has less than {1} {2}";

						public static LocString WARNING = "{0} has less than {1} {2}";
					}
				}

				public class MAX_HEIGHT
				{
					public class STATUS
					{
						public static LocString READY = "Height limit";

						public static LocString FAILURE = "Height limit";

						public static LocString WARNING = "Height limit";
					}

					public class TOOLTIP
					{
						public static LocString READY = "The rocket's engine can support the height of the rocket";

						public static LocString FAILURE = "The height of the rocket exceeds the engine's limit\n\nExcess modules must be removed";

						public static LocString WARNING = "Height limit warning";
					}
				}

				public class PROPERLY_FUELED
				{
					public class STATUS
					{
						public static LocString READY = "Fueled";

						public static LocString FAILURE = "Fueled";

						public static LocString WARNING = "Fueled";
					}

					public class TOOLTIP
					{
						public static LocString READY = "The rocket is sufficiently fueled for a roundtrip to its destination and back";

						public static LocString READY_NO_DESTINATION = "This rocket's fuel tanks have been filled to capacity, but it has no destination";

						public static LocString FAILURE = "This rocket does not have enough fuel to reach its destination\n\nIf the tanks are full, a different Fuel Tank Module may be required";

						public static LocString WARNING = "The rocket has enough fuel for a one-way trip to its destination, but will not be able to make it back";
					}
				}

				public class SUFFICIENT_OXIDIZER
				{
					public class STATUS
					{
						public static LocString READY = "Sufficient Oxidizer";

						public static LocString FAILURE = "Sufficient Oxidizer";

						public static LocString WARNING = "Warning: Limited oxidizer";
					}

					public class TOOLTIP
					{
						public static LocString READY = "This rocket has sufficient oxidizer for a roundtrip to its destination and back";

						public static LocString FAILURE = "This rocket does not have enough oxidizer to reach its destination\n\nIf the oxidizer tanks are full, a different Oxidizer Tank Module may be required";

						public static LocString WARNING = "The rocket has enough oxidizer for a one-way trip to its destination, but will not be able to make it back";
					}
				}

				public class ON_LAUNCHPAD
				{
					public class STATUS
					{
						public static LocString READY = "On a launch pad";

						public static LocString FAILURE = "Not on a launch pad";

						public static LocString WARNING = "No launch pad";
					}

					public class TOOLTIP
					{
						public static LocString READY = "On a launch pad";

						public static LocString FAILURE = "Not on a launch pad";

						public static LocString WARNING = "No launch pad";
					}
				}
			}

			public class FULLTANK
			{
				public static LocString NAME = "Fuel Tank full";

				public static LocString TOOLTIP = "Tank is full, ready for launch";
			}

			public class EMPTYTANK
			{
				public static LocString NAME = "Fuel Tank not full";

				public static LocString TOOLTIP = "Fuel tank must be filled before launch";
			}

			public class FULLOXIDIZERTANK
			{
				public static LocString NAME = "Oxidizer Tank full";

				public static LocString TOOLTIP = "Tank is full, ready for launch";
			}

			public class EMPTYOXIDIZERTANK
			{
				public static LocString NAME = "Oxidizer Tank not full";

				public static LocString TOOLTIP = "Oxidizer tank must be filled before launch";
			}

			public class ROCKETSTATUS
			{
				public static LocString STATUS_TITLE = "Rocket Status";

				public static LocString NONE = "NONE";

				public static LocString SELECTED = "SELECTED";

				public static LocString LOCKEDIN = "LOCKED IN";

				public static LocString NODESTINATION = "No destination selected";

				public static LocString DESTINATIONVALUE = "None";

				public static LocString NOPASSENGERS = "No passengers";

				public static LocString STATUS = "Status";

				public static LocString TOTAL = "Total";

				public static LocString WEIGHTPENALTY = "Weight Penalty";

				public static LocString TIMEREMAINING = "Time Remaining";

				public static LocString BOOSTED_TIME_MODIFIER = "Less Than ";
			}

			public class ROCKETSTATS
			{
				public static LocString TOTAL_OXIDIZABLE_FUEL = "Total oxidizable fuel";

				public static LocString TOTAL_OXIDIZER = "Total oxidizer";

				public static LocString TOTAL_FUEL = "Total fuel";

				public static LocString NO_ENGINE = "NO ENGINE";

				public static LocString ENGINE_EFFICIENCY = "Main engine efficiency";

				public static LocString OXIDIZER_EFFICIENCY = "Average oxidizer efficiency";

				public static LocString SOLID_BOOSTER = "Solid boosters";

				public static LocString TOTAL_THRUST = "Total thrust";

				public static LocString TOTAL_RANGE = "Total range";

				public static LocString DRY_MASS = "Dry mass";

				public static LocString WET_MASS = "Wet mass";
			}

			public class STORAGESTATS
			{
				public static LocString STORAGECAPACITY = "{0} / {1}";
			}
		}

		public class RESEARCHSCREEN
		{
			public class FILTER_BUTTONS
			{
				public static LocString HEADER = "Preset Filters";

				public static LocString ALL = "All";

				public static LocString AVAILABLE = "Next";

				public static LocString COMPLETED = "Completed";

				public static LocString OXYGEN = "Oxygen";

				public static LocString FOOD = "Food";

				public static LocString WATER = "Water";

				public static LocString POWER = "Power";

				public static LocString MORALE = "Morale";

				public static LocString RANCHING = "Ranching";

				public static LocString FILTER = "Filters";

				public static LocString TILE = "Tiles";

				public static LocString TRANSPORT = "Transport";

				public static LocString AUTOMATION = "Automation";

				public static LocString MEDICINE = "Medicine";

				public static LocString ROCKET = "Rockets";

				public static LocString RADIATION = "Radiation";
			}
		}

		public class CODEX
		{
			public static LocString SEARCH_HEADER = "Search Database";

			public static LocString BACK_BUTTON = "Back ({0})";

			public static LocString TIPS = "Tips";

			public static LocString GAME_SYSTEMS = "Systems";

			public static LocString DETAILS = "Details";

			public static LocString RECIPE_ITEM = "{0} x {1}{2}";

			public static LocString RECIPE_FABRICATOR = "{1} ({0} seconds)";

			public static LocString RECIPE_FABRICATOR_HEADER = "Produced by";

			public static LocString BACK_BUTTON_TOOLTIP = UI.CLICK(UI.ClickType.Click) + " to go back:\n{0}";

			public static LocString BACK_BUTTON_NO_HISTORY_TOOLTIP = UI.CLICK(UI.ClickType.Click) + " to go back:\nN/A";

			public static LocString FORWARD_BUTTON_TOOLTIP = UI.CLICK(UI.ClickType.Click) + " to go forward:\n{0}";

			public static LocString FORWARD_BUTTON_NO_HISTORY_TOOLTIP = UI.CLICK(UI.ClickType.Click) + " to go forward:\nN/A";

			public static LocString TITLE = "DATABASE";

			public static LocString MANAGEMENT_BUTTON = "DATABASE";

			public class CODEX_DISCOVERED_MESSAGE
			{
				public static LocString TITLE = "New Log Entry";

				public static LocString BODY = "I've added a new entry to my log: {codex}\n";
			}

			public class SUBWORLDS
			{
				public static LocString ELEMENTS = "Elements";

				public static LocString PLANTS = "Plants";

				public static LocString CRITTERS = "Critters";

				public static LocString NONE = "None";
			}

			public class GEYSERS
			{
				public static LocString DESC = "Geysers and Fumaroles emit elements at variable intervals. They provide a sustainable source of material, albeit in typically low volumes.\n\nThe variable factors of a geyser are:\n\n    • Emission element \n    • Emission temperature \n    • Emission mass \n    • Cycle length \n    • Dormancy duration \n    • Disease emitted";
			}

			public class EQUIPMENT
			{
				public static LocString DESC = "Equipment description";
			}

			public class FOOD
			{
				public static LocString QUALITY = "Quality: {0}";

				public static LocString CALORIES = "Calories: {0}";

				public static LocString SPOILPROPERTIES = "Refrigeration temperature: {0}\nDeep Freeze temperature: {1}\nSpoil time: {2}";

				public static LocString NON_PERISHABLE = "Spoil time: Never";
			}

			public class CATEGORYNAMES
			{
				public static LocString ROOT = UI.FormatAsLink("Index", "HOME");

				public static LocString PLANTS = UI.FormatAsLink("Plants", "PLANTS");

				public static LocString CREATURES = UI.FormatAsLink("Critters", "CREATURES");

				public static LocString EMAILS = UI.FormatAsLink("E-mail", "EMAILS");

				public static LocString JOURNALS = UI.FormatAsLink("Journals", "JOURNALS");

				public static LocString MYLOG = UI.FormatAsLink("My Log", "MYLOG");

				public static LocString INVESTIGATIONS = UI.FormatAsLink("Investigations", "Investigations");

				public static LocString RESEARCHNOTES = UI.FormatAsLink("Research Notes", "RESEARCHNOTES");

				public static LocString NOTICES = UI.FormatAsLink("Notices", "NOTICES");

				public static LocString FOOD = UI.FormatAsLink("Food", "FOOD");

				public static LocString MINION_MODIFIERS = UI.FormatAsLink("Duplicant Effects (EDITOR ONLY)", "MINION_MODIFIERS");

				public static LocString BUILDINGS = UI.FormatAsLink("Buildings", "BUILDINGS");

				public static LocString ROOMS = UI.FormatAsLink("Rooms", "ROOMS");

				public static LocString TECH = UI.FormatAsLink("Research", "TECH");

				public static LocString TIPS = UI.FormatAsLink("Lessons", "LESSONS");

				public static LocString EQUIPMENT = UI.FormatAsLink("Equipment", "EQUIPMENT");

				public static LocString BIOMES = UI.FormatAsLink("Biomes", "BIOMES");

				public static LocString STORYTRAITS = UI.FormatAsLink("Story Traits", "STORYTRAITS");

				public static LocString VIDEOS = UI.FormatAsLink("Videos", "VIDEOS");

				public static LocString MISCELLANEOUSTIPS = UI.FormatAsLink("Tips", "MISCELLANEOUSTIPS");

				public static LocString MISCELLANEOUSITEMS = UI.FormatAsLink("Items", "MISCELLANEOUSITEMS");

				public static LocString ELEMENTS = UI.FormatAsLink("Elements", "ELEMENTS");

				public static LocString ELEMENTSSOLID = UI.FormatAsLink("Solids", "ELEMENTS_SOLID");

				public static LocString ELEMENTSGAS = UI.FormatAsLink("Gases", "ELEMENTS_GAS");

				public static LocString ELEMENTSLIQUID = UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID");

				public static LocString ELEMENTSOTHER = UI.FormatAsLink("Other", "ELEMENTS_OTHER");

				public static LocString ELEMENTSCLASSES = UI.FormatAsLink("Classes", "ELEMENTS_CLASSES");

				public static LocString INDUSTRIALINGREDIENTS = UI.FormatAsLink("Industrial Ingredients", "INDUSTRIALINGREDIENTS");

				public static LocString GEYSERS = UI.FormatAsLink("Geysers", "GEYSERS");

				public static LocString SYSTEMS = UI.FormatAsLink("Systems", "SYSTEMS");

				public static LocString ROLES = UI.FormatAsLink("Duplicant Skills", "ROLES");

				public static LocString DISEASE = UI.FormatAsLink("Disease", "DISEASE");

				public static LocString SICKNESS = UI.FormatAsLink("Sickness", "SICKNESS");

				public static LocString MEDIA = UI.FormatAsLink("Media", "MEDIA");
			}
		}

		public class DEVELOPMENTBUILDS
		{
			public static LocString WATERMARK = "BUILD: {0}";

			public static LocString TESTING_WATERMARK = "TESTING BUILD: {0}";

			public static LocString TESTING_TOOLTIP = "This game is currently running a Test version.\n\n" + UI.CLICK(UI.ClickType.Click) + " for more info.";

			public static LocString TESTING_MESSAGE_TITLE = "TESTING BUILD";

			public static LocString TESTING_MESSAGE = "This game is running a Test version of Oxygen Not Included. This means that some features may be in development or buggier than normal, and require more testing before they can be moved into the Release build.\n\nIf you encounter any bugs or strange behavior, please add a report to the bug forums. We appreciate it!";

			public static LocString TESTING_MORE_INFO = "BUG FORUMS";

			public static LocString FULL_PATCH_NOTES = "Full Patch Notes";

			public static LocString PREVIOUS_VERSION = "Previous Version";

			public class ALPHA
			{
				public class MESSAGES
				{
					public static LocString FORUMBUTTON = "FORUMS";

					public static LocString MAILINGLIST = "MAILING LIST";

					public static LocString PATCHNOTES = "PATCH NOTES";

					public static LocString FEEDBACK = "FEEDBACK";
				}

				public class LOADING
				{
					public static LocString TITLE = "<b>Welcome to Oxygen Not Included!</b>";

					public static LocString BODY = "This game is in the early stages of development which means you're likely to encounter strange, amusing, and occasionally just downright frustrating bugs.\n\nDuring this time Oxygen Not Included will be receiving regular updates to fix bugs, add features, and introduce additional content, so if you encounter issues or just have suggestions to share, please let us know on our forums: <u>http://forums.kleientertainment.com</u>\n\nA special thanks to those who joined us during our time in Alpha. We value your feedback and thank you for joining us in the development process. We couldn't do this without you.\n\nEnjoy your time in deep space!\n\n- Klei";

					public static LocString BODY_NOLINKS = "This DLC is currently in active development, which means you're likely to encounter strange, amusing, and occasionally just downright frustrating bugs.\n\n During this time Spaced Out! will be receiving regular updates to fix bugs, add features, and introduce additional content.\n\n We've got lots of content old and new to add to this DLC before it's ready, and we're happy to have you along with us. Enjoy your time in deep space!\n\n - The Team at Klei";

					public static LocString FORUMBUTTON = "Visit Forums";
				}

				public class HEALTHY_MESSAGE
				{
					public static LocString CONTINUEBUTTON = "Thanks!";
				}
			}

			public class PREVIOUS_UPDATE
			{
				public static LocString TITLE = "<b>Welcome to Oxygen Not Included!</b>";

				public static LocString BODY = "Whoops!\n\nYou're about to opt in to the <b>Previous Update branch</b>. That means opting out of all new features, fixes and content from the live branch.\n\nThis branch is temporary. It will be replaced when the next update is released. It's also completely unsupported: please don't report bugs or issues you find here.\n\nAre you sure you want to opt in?";

				public static LocString CONTINUEBUTTON = "Play Old Version";

				public static LocString FORUMBUTTON = "More Information";

				public static LocString QUITBUTTON = "Quit";
			}

			public class UPDATES
			{
				public static LocString UPDATES_HEADER = "NEXT UPGRADE LIVE IN";

				public static LocString NOW = "Less than a day";

				public static LocString TWENTY_FOUR_HOURS = "Less than a day";

				public static LocString FINAL_WEEK = "{0} days";

				public static LocString BIGGER_TIMES = "{1} weeks {0} days";
			}
		}

		public class UNITSUFFIXES
		{
			public static LocString SECOND = " s";

			public static LocString PERSECOND = "/s";

			public static LocString PERCYCLE = "/cycle";

			public static LocString UNIT = " unit";

			public static LocString UNITS = " units";

			public static LocString PERCENT = "%";

			public static LocString DEGREES = " degrees";

			public static LocString CRITTERS = " critters";

			public static LocString GROWTH = "growth";

			public static LocString SECONDS = "Seconds";

			public static LocString DUPLICANTS = "Duplicants";

			public static LocString GERMS = "Germs";

			public static LocString ROCKET_MISSIONS = "Missions";

			public class MASS
			{
				public static LocString TONNE = " t";

				public static LocString KILOGRAM = " kg";

				public static LocString GRAM = " g";

				public static LocString MILLIGRAM = " mg";

				public static LocString MICROGRAM = " mcg";

				public static LocString POUND = " lb";

				public static LocString DRACHMA = " dr";

				public static LocString GRAIN = " gr";
			}

			public class TEMPERATURE
			{
				public static LocString CELSIUS = " " + 'º'.ToString() + "C";

				public static LocString FAHRENHEIT = " " + 'º'.ToString() + "F";

				public static LocString KELVIN = " K";
			}

			public class CALORIES
			{
				public static LocString CALORIE = " cal";

				public static LocString KILOCALORIE = " kcal";
			}

			public class ELECTRICAL
			{
				public static LocString JOULE = " J";

				public static LocString KILOJOULE = " kJ";

				public static LocString MEGAJOULE = " MJ";

				public static LocString WATT = " W";

				public static LocString KILOWATT = " kW";
			}

			public class HEAT
			{
				public static LocString DTU = " DTU";

				public static LocString KDTU = " kDTU";

				public static LocString DTU_S = " DTU/s";

				public static LocString KDTU_S = " kDTU/s";
			}

			public class DISTANCE
			{
				public static LocString METER = " m";

				public static LocString KILOMETER = " km";
			}

			public class DISEASE
			{
				public static LocString UNITS = " germs";
			}

			public class NOISE
			{
				public static LocString UNITS = " dB";
			}

			public class INFORMATION
			{
				public static LocString BYTE = "B";

				public static LocString KILOBYTE = "kB";

				public static LocString MEGABYTE = "MB";

				public static LocString GIGABYTE = "GB";

				public static LocString TERABYTE = "TB";
			}

			public class LIGHT
			{
				public static LocString LUX = " lux";
			}

			public class RADIATION
			{
				public static LocString RADS = " rads";
			}

			public class HIGHENERGYPARTICLES
			{
				public static LocString PARTRICLE = " Radbolt";

				public static LocString PARTRICLES = " Radbolts";
			}
		}

		public class OVERLAYS
		{
			public class TILEMODE
			{
				public static LocString NAME = "MATERIALS OVERLAY";

				public static LocString BUTTON = "Materials Overlay";
			}

			public class OXYGEN
			{
				public static LocString NAME = "OXYGEN OVERLAY";

				public static LocString BUTTON = "Oxygen Overlay";

				public static LocString LEGEND1 = "Very Breathable";

				public static LocString LEGEND2 = "Breathable";

				public static LocString LEGEND3 = "Barely Breathable";

				public static LocString LEGEND4 = "Unbreathable";

				public static LocString LEGEND5 = "Barely Breathable";

				public static LocString LEGEND6 = "Unbreathable";

				public class TOOLTIPS
				{
					public static LocString LEGEND1 = string.Concat(new string[]
					{
						"<b>Very Breathable</b>\nHigh ",
						UI.PRE_KEYWORD,
						"Oxygen",
						UI.PST_KEYWORD,
						" concentrations"
					});

					public static LocString LEGEND2 = string.Concat(new string[]
					{
						"<b>Breathable</b>\nSufficient ",
						UI.PRE_KEYWORD,
						"Oxygen",
						UI.PST_KEYWORD,
						" concentrations"
					});

					public static LocString LEGEND3 = string.Concat(new string[]
					{
						"<b>Barely Breathable</b>\nLow ",
						UI.PRE_KEYWORD,
						"Oxygen",
						UI.PST_KEYWORD,
						" concentrations"
					});

					public static LocString LEGEND4 = string.Concat(new string[]
					{
						"<b>Unbreathable</b>\nExtremely low or absent ",
						UI.PRE_KEYWORD,
						"Oxygen",
						UI.PST_KEYWORD,
						" concentrations\n\nDuplicants will suffocate if trapped in these areas"
					});

					public static LocString LEGEND5 = "<b>Slightly Toxic</b>\nHarmful gas concentration";

					public static LocString LEGEND6 = "<b>Very Toxic</b>\nLethal gas concentration";
				}
			}

			public class ELECTRICAL
			{
				public static LocString NAME = "POWER OVERLAY";

				public static LocString BUTTON = "Power Overlay";

				public static LocString LEGEND1 = "<b>BUILDING POWER</b>";

				public static LocString LEGEND2 = "Consumer";

				public static LocString LEGEND3 = "Producer";

				public static LocString LEGEND4 = "<b>CIRCUIT POWER HEALTH</b>";

				public static LocString LEGEND5 = "Inactive";

				public static LocString LEGEND6 = "Safe";

				public static LocString LEGEND7 = "Strained";

				public static LocString LEGEND8 = "Overloaded";

				public static LocString DIAGRAM_HEADER = "Energy from the <b>Left Outlet</b> is used by the <b>Right Outlet</b>";

				public static LocString LEGEND_SWITCH = "Switch";

				public class TOOLTIPS
				{
					public static LocString LEGEND1 = "Displays whether buildings use or generate " + UI.FormatAsLink("Power", "POWER");

					public static LocString LEGEND2 = "<b>Consumer</b>\nThese buildings draw power from a circuit";

					public static LocString LEGEND3 = "<b>Producer</b>\nThese buildings generate power for a circuit";

					public static LocString LEGEND4 = "Displays the health of wire systems";

					public static LocString LEGEND5 = "<b>Inactive</b>\nThere is no power activity on these circuits";

					public static LocString LEGEND6 = "<b>Safe</b>\nThese circuits are not in danger of overloading";

					public static LocString LEGEND7 = "<b>Strained</b>\nThese circuits are close to consuming more power than their wires support";

					public static LocString LEGEND8 = "<b>Overloaded</b>\nThese circuits are consuming more power than their wires support";

					public static LocString LEGEND_SWITCH = "<b>Switch</b>\nActivates or deactivates connected circuits";
				}
			}

			public class TEMPERATURE
			{
				public static LocString NAME = "TEMPERATURE OVERLAY";

				public static LocString BUTTON = "Temperature Overlay";

				public static LocString EXTREMECOLD = "Absolute Zero";

				public static LocString VERYCOLD = "Cold";

				public static LocString COLD = "Chilled";

				public static LocString TEMPERATE = "Temperate";

				public static LocString HOT = "Warm";

				public static LocString VERYHOT = "Hot";

				public static LocString EXTREMEHOT = "Scorching";

				public static LocString MAXHOT = "Molten";

				public class TOOLTIPS
				{
					public static LocString TEMPERATURE = "Temperatures reaching {0}";
				}
			}

			public class STATECHANGE
			{
				public static LocString LOWPOINT = "Low energy state change";

				public static LocString STABLE = "Stable";

				public static LocString HIGHPOINT = "High energy state change";

				public class TOOLTIPS
				{
					public static LocString LOWPOINT = "Nearing a low energy state change";

					public static LocString STABLE = "Not near any state changes";

					public static LocString HIGHPOINT = "Nearing high energy state change";
				}
			}

			public class HEATFLOW
			{
				public static LocString NAME = "THERMAL TOLERANCE OVERLAY";

				public static LocString HOVERTITLE = "THERMAL TOLERANCE";

				public static LocString BUTTON = "Thermal Tolerance Overlay";

				public static LocString COOLING = "Body Heat Loss";

				public static LocString NEUTRAL = "Comfort Zone";

				public static LocString HEATING = "Body Heat Retention";

				public class TOOLTIPS
				{
					public static LocString COOLING = "<b>Body Heat Loss</b>\nUncomfortably cold\n\nDuplicants lose more heat in these areas than they can absorb\n* Warm Sweaters help Duplicants retain body heat";

					public static LocString NEUTRAL = "<b>Comfort Zone</b>\nComfortable area\n\nDuplicants can regulate their internal temperatures in these areas";

					public static LocString HEATING = "<b>Body Heat Retention</b>\nUncomfortably warm\n\nDuplicants absorb more heat in these areas than they can release\n* Cool Vests help Duplicants shed excess body heat";
				}
			}

			public class ROOMS
			{
				public static LocString NAME = "ROOM OVERLAY";

				public static LocString BUTTON = "Room Overlay";

				public static LocString ROOM = "Room {0}";

				public static LocString HOVERTITLE = "ROOMS";

				public static class NOROOM
				{
					public static LocString HEADER = "No Room";

					public static LocString DESC = "Enclose this space with walls and doors to make a room";

					public static LocString TOO_BIG = "<color=#F44A47FF>    • Size: {0} Tiles\n    • Maximum room size: {1} Tiles</color>";
				}

				public class TOOLTIPS
				{
					public static LocString ROOM = "Completed Duplicant bedrooms";

					public static LocString NOROOMS = "Duplicants have nowhere to sleep";
				}
			}

			public class JOULES
			{
				public static LocString NAME = "JOULES";

				public static LocString HOVERTITLE = "JOULES";

				public static LocString BUTTON = "Joules Overlay";
			}

			public class LIGHTING
			{
				public static LocString NAME = "LIGHT OVERLAY";

				public static LocString BUTTON = "Light Overlay";

				public static LocString LITAREA = "Lit Area";

				public static LocString DARK = "Unlit Area";

				public static LocString HOVERTITLE = "LIGHT";

				public static LocString DESC = "{0} Lux";

				public class RANGES
				{
					public static LocString NO_LIGHT = "Pitch Black";

					public static LocString VERY_LOW_LIGHT = "Dark";

					public static LocString LOW_LIGHT = "Dim";

					public static LocString MEDIUM_LIGHT = "Well Lit";

					public static LocString HIGH_LIGHT = "Bright";

					public static LocString VERY_HIGH_LIGHT = "Brilliant";

					public static LocString MAX_LIGHT = "Blinding";
				}

				public class TOOLTIPS
				{
					public static LocString NAME = "LIGHT OVERLAY";

					public static LocString LITAREA = "<b>Lit Area</b>\nWorking in well-lit areas improves Duplicant " + UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD;

					public static LocString DARK = "<b>Unlit Area</b>\nWorking in the dark has no effect on Duplicants";
				}
			}

			public class CROP
			{
				public static LocString NAME = "FARMING OVERLAY";

				public static LocString BUTTON = "Farming Overlay";

				public static LocString GROWTH_HALTED = "Halted Growth";

				public static LocString GROWING = "Growing";

				public static LocString FULLY_GROWN = "Fully Grown";

				public class TOOLTIPS
				{
					public static LocString GROWTH_HALTED = "<b>Halted Growth</b>\nSubstandard conditions prevent these plants from growing";

					public static LocString GROWING = "<b>Growing</b>\nThese plants are thriving in their current conditions";

					public static LocString FULLY_GROWN = "<b>Fully Grown</b>\nThese plants have reached maturation\n\nSelect the " + UI.FormatAsTool("Harvest Tool", global::Action.Harvest) + " to batch harvest";
				}
			}

			public class LIQUIDPLUMBING
			{
				public static LocString NAME = "PLUMBING OVERLAY";

				public static LocString BUTTON = "Plumbing Overlay";

				public static LocString CONSUMER = "Output Pipe";

				public static LocString FILTERED = "Filtered Output Pipe";

				public static LocString PRODUCER = "Building Intake";

				public static LocString CONNECTED = "Connected";

				public static LocString DISCONNECTED = "Disconnected";

				public static LocString NETWORK = "Liquid Network {0}";

				public static LocString DIAGRAM_BEFORE_ARROW = "Liquid flows from <b>Output Pipe</b>";

				public static LocString DIAGRAM_AFTER_ARROW = "<b>Building Intake</b>";

				public class TOOLTIPS
				{
					public static LocString CONNECTED = "Connected to a " + UI.FormatAsLink("Liquid Pipe", "LIQUIDCONDUIT");

					public static LocString DISCONNECTED = "Not connected to a " + UI.FormatAsLink("Liquid Pipe", "LIQUIDCONDUIT");

					public static LocString CONSUMER = "<b>Output Pipe</b>\nOutputs send liquid into pipes\n\nMust be on the same network as at least one " + UI.FormatAsLink("Intake", "LIQUIDPIPING");

					public static LocString FILTERED = "<b>Filtered Output Pipe</b>\nFiltered Outputs send filtered liquid into pipes\n\nMust be on the same network as at least one " + UI.FormatAsLink("Intake", "LIQUIDPIPING");

					public static LocString PRODUCER = "<b>Building Intake</b>\nIntakes send liquid into buildings\n\nMust be on the same network as at least one " + UI.FormatAsLink("Output", "LIQUIDPIPING");

					public static LocString NETWORK = "Liquid network {0}";
				}
			}

			public class GASPLUMBING
			{
				public static LocString NAME = "VENTILATION OVERLAY";

				public static LocString BUTTON = "Ventilation Overlay";

				public static LocString CONSUMER = "Output Pipe";

				public static LocString FILTERED = "Filtered Output Pipe";

				public static LocString PRODUCER = "Building Intake";

				public static LocString CONNECTED = "Connected";

				public static LocString DISCONNECTED = "Disconnected";

				public static LocString NETWORK = "Gas Network {0}";

				public static LocString DIAGRAM_BEFORE_ARROW = "Gas flows from <b>Output Pipe</b>";

				public static LocString DIAGRAM_AFTER_ARROW = "<b>Building Intake</b>";

				public class TOOLTIPS
				{
					public static LocString CONNECTED = "Connected to a " + UI.FormatAsLink("Gas Pipe", "GASPIPING");

					public static LocString DISCONNECTED = "Not connected to a " + UI.FormatAsLink("Gas Pipe", "GASPIPING");

					public static LocString CONSUMER = string.Concat(new string[]
					{
						"<b>Output Pipe</b>\nOutputs send ",
						UI.PRE_KEYWORD,
						"Gas",
						UI.PST_KEYWORD,
						" into ",
						UI.PRE_KEYWORD,
						"Pipes",
						UI.PST_KEYWORD,
						"\n\nMust be on the same network as at least one ",
						UI.FormatAsLink("Intake", "GASPIPING")
					});

					public static LocString FILTERED = string.Concat(new string[]
					{
						"<b>Filtered Output Pipe</b>\nFiltered Outputs send filtered ",
						UI.PRE_KEYWORD,
						"Gas",
						UI.PST_KEYWORD,
						" into ",
						UI.PRE_KEYWORD,
						"Pipes",
						UI.PST_KEYWORD,
						"\n\nMust be on the same network as at least one ",
						UI.FormatAsLink("Intake", "GASPIPING")
					});

					public static LocString PRODUCER = "<b>Building Intake</b>\nIntakes send gas into buildings\n\nMust be on the same network as at least one " + UI.FormatAsLink("Output", "GASPIPING");

					public static LocString NETWORK = "Gas network {0}";
				}
			}

			public class SUIT
			{
				public static LocString NAME = "EXOSUIT OVERLAY";

				public static LocString BUTTON = "Exosuit Overlay";

				public static LocString SUIT_ICON = "Exosuit";

				public static LocString SUIT_ICON_TOOLTIP = "<b>Exosuit</b>\nHighlights the current location of equippable exosuits";
			}

			public class LOGIC
			{
				public static LocString NAME = "AUTOMATION OVERLAY";

				public static LocString BUTTON = "Automation Overlay";

				public static LocString INPUT = "Input Port";

				public static LocString OUTPUT = "Output Port";

				public static LocString RIBBON_INPUT = "Ribbon Input Port";

				public static LocString RIBBON_OUTPUT = "Ribbon Output Port";

				public static LocString RESET_UPDATE = "Reset Port";

				public static LocString CONTROL_INPUT = "Control Port";

				public static LocString CIRCUIT_STATUS_HEADER = "GRID STATUS";

				public static LocString ONE = "Green";

				public static LocString ZERO = "Red";

				public static LocString DISCONNECTED = "DISCONNECTED";

				public abstract class TOOLTIPS
				{
					public static LocString INPUT = "<b>Input Port</b>\nReceives a signal from an automation grid";

					public static LocString OUTPUT = "<b>Output Port</b>\nSends a signal out to an automation grid";

					public static LocString RIBBON_INPUT = "<b>Ribbon Input Port</b>\nReceives a 4-bit signal from an automation grid";

					public static LocString RIBBON_OUTPUT = "<b>Ribbon Output Port</b>\nSends a 4-bit signal out to an automation grid";

					public static LocString RESET_UPDATE = "<b>Reset Port</b>\nReset a " + BUILDINGS.PREFABS.LOGICMEMORY.NAME + "'s internal Memory to " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby);

					public static LocString CONTROL_INPUT = "<b>Control Port</b>\nControl the signal selection of a " + BUILDINGS.PREFABS.LOGICGATEMULTIPLEXER.NAME + " or " + BUILDINGS.PREFABS.LOGICGATEDEMULTIPLEXER.NAME;

					public static LocString ONE = "<b>Green</b>\nThis port is currently " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active);

					public static LocString ZERO = "<b>Red</b>\nThis port is currently " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby);

					public static LocString DISCONNECTED = "<b>Disconnected</b>\nThis port is not connected to an automation grid";
				}
			}

			public class CONVEYOR
			{
				public static LocString NAME = "CONVEYOR OVERLAY";

				public static LocString BUTTON = "Conveyor Overlay";

				public static LocString OUTPUT = "Loader";

				public static LocString INPUT = "Receptacle";

				public abstract class TOOLTIPS
				{
					public static LocString OUTPUT = string.Concat(new string[]
					{
						"<b>Loader</b>\nLoads material onto a ",
						UI.PRE_KEYWORD,
						"Conveyor Rail",
						UI.PST_KEYWORD,
						" for transport to Receptacles"
					});

					public static LocString INPUT = string.Concat(new string[]
					{
						"<b>Receptacle</b>\nReceives material from a ",
						UI.PRE_KEYWORD,
						"Conveyor Rail",
						UI.PST_KEYWORD,
						" and stores it for Duplicant use"
					});
				}
			}

			public class DECOR
			{
				public static LocString NAME = "DECOR OVERLAY";

				public static LocString BUTTON = "Decor Overlay";

				public static LocString TOTAL = "Total Decor: ";

				public static LocString ENTRY = "{0} {1} {2}";

				public static LocString COUNT = "({0})";

				public static LocString VALUE = "{0}{1}";

				public static LocString VALUE_ZERO = "{0}{1}";

				public static LocString HEADER_POSITIVE = "Positive Value:";

				public static LocString HEADER_NEGATIVE = "Negative Value:";

				public static LocString LOWDECOR = "Negative Decor";

				public static LocString HIGHDECOR = "Positive Decor";

				public static LocString CLUTTER = "Debris";

				public static LocString LIGHTING = "Lighting";

				public static LocString CLOTHING = "{0}'s Outfit";

				public static LocString CLOTHING_TRAIT_DECORUP = "{0}'s Outfit (Innately Stylish)";

				public static LocString CLOTHING_TRAIT_DECORDOWN = "{0}'s Outfit (Shabby Dresser)";

				public static LocString HOVERTITLE = "DECOR";

				public static LocString MAXIMUM_DECOR = "{0}{1} (Maximum Decor)";

				public class TOOLTIPS
				{
					public static LocString LOWDECOR = string.Concat(new string[]
					{
						"<b>Negative Decor</b>\nArea with insufficient ",
						UI.PRE_KEYWORD,
						"Decor",
						UI.PST_KEYWORD,
						" values\n* Resources on the floor are considered \"debris\" and will decrease decor"
					});

					public static LocString HIGHDECOR = string.Concat(new string[]
					{
						"<b>Positive Decor</b>\nArea with sufficient ",
						UI.PRE_KEYWORD,
						"Decor",
						UI.PST_KEYWORD,
						" values\n* Lighting and aesthetically pleasing buildings increase decor"
					});
				}
			}

			public class PRIORITIES
			{
				public static LocString NAME = "PRIORITY OVERLAY";

				public static LocString BUTTON = "Priority Overlay";

				public static LocString ONE = "1 (Low Urgency)";

				public static LocString ONE_TOOLTIP = "Priority 1";

				public static LocString TWO = "2";

				public static LocString TWO_TOOLTIP = "Priority 2";

				public static LocString THREE = "3";

				public static LocString THREE_TOOLTIP = "Priority 3";

				public static LocString FOUR = "4";

				public static LocString FOUR_TOOLTIP = "Priority 4";

				public static LocString FIVE = "5";

				public static LocString FIVE_TOOLTIP = "Priority 5";

				public static LocString SIX = "6";

				public static LocString SIX_TOOLTIP = "Priority 6";

				public static LocString SEVEN = "7";

				public static LocString SEVEN_TOOLTIP = "Priority 7";

				public static LocString EIGHT = "8";

				public static LocString EIGHT_TOOLTIP = "Priority 8";

				public static LocString NINE = "9 (High Urgency)";

				public static LocString NINE_TOOLTIP = "Priority 9";
			}

			public class DISEASE
			{
				public static LocString NAME = "GERM OVERLAY";

				public static LocString BUTTON = "Germ Overlay";

				public static LocString HOVERTITLE = "Germ";

				public static LocString INFECTION_SOURCE = "Germ Source";

				public static LocString INFECTION_SOURCE_TOOLTIP = "<b>Germ Source</b>\nAreas where germs are produced\n* Placing Wash Basins or Hand Sanitizers near these areas may prevent disease spread";

				public static LocString NO_DISEASE = "Zero surface germs";

				public static LocString DISEASE_NAME_FORMAT = "{0}<color=#{1}></color>";

				public static LocString DISEASE_NAME_FORMAT_NO_COLOR = "{0}";

				public static LocString DISEASE_FORMAT = "{1} [{0}]<color=#{2}></color>";

				public static LocString DISEASE_FORMAT_NO_COLOR = "{1} [{0}]";

				public static LocString CONTAINER_FORMAT = "\n    {0}: {1}";

				public class DISINFECT_THRESHOLD_DIAGRAM
				{
					public static LocString UNITS = "Germs";

					public static LocString MIN_LABEL = "0";

					public static LocString MAX_LABEL = "1m";

					public static LocString THRESHOLD_PREFIX = "Disinfect At:";

					public static LocString TOOLTIP = "Automatically disinfect any building with more than {NumberOfGerms} germs.";

					public static LocString TOOLTIP_DISABLED = "Automatic building disinfection disabled.";
				}
			}

			public class CROPS
			{
				public static LocString NAME = "FARMING OVERLAY";

				public static LocString BUTTON = "Farming Overlay";
			}

			public class POWER
			{
				public static LocString WATTS_GENERATED = "Watts Generated";

				public static LocString WATTS_CONSUMED = "Watts Consumed";
			}

			public class RADIATION
			{
				public static LocString NAME = "RADIATION";

				public static LocString BUTTON = "Radiation Overlay";

				public static LocString DESC = "{rads} per cycle ({description})";

				public static LocString SHIELDING_DESC = "Radiation Blocking: {radiationAbsorptionFactor}";

				public static LocString HOVERTITLE = "RADIATION";

				public class RANGES
				{
					public static LocString NONE = "Completely Safe";

					public static LocString VERY_LOW = "Mostly Safe";

					public static LocString LOW = "Barely Safe";

					public static LocString MEDIUM = "Slight Hazard";

					public static LocString HIGH = "Significant Hazard";

					public static LocString VERY_HIGH = "Extreme Hazard";

					public static LocString MAX = "Maximum Hazard";

					public static LocString INPUTPORT = "Radbolt Input Port";

					public static LocString OUTPUTPORT = "Radbolt Output Port";
				}

				public class TOOLTIPS
				{
					public static LocString NONE = "Completely Safe";

					public static LocString VERY_LOW = "Mostly Safe";

					public static LocString LOW = "Barely Safe";

					public static LocString MEDIUM = "Slight Hazard";

					public static LocString HIGH = "Significant Hazard";

					public static LocString VERY_HIGH = "Extreme Hazard";

					public static LocString MAX = "Maximum Hazard";

					public static LocString INPUTPORT = "Radbolt Input Port";

					public static LocString OUTPUTPORT = "Radbolt Output Port";
				}
			}
		}

		public class TABLESCREENS
		{
			public static LocString DUPLICANT_PROPERNAME = "<b>{0}</b>";

			public static LocString SELECT_DUPLICANT_BUTTON = UI.CLICK(UI.ClickType.Click) + " to select <b>{0}</b>";

			public static LocString GOTO_DUPLICANT_BUTTON = "Double-" + UI.CLICK(UI.ClickType.click) + " to go to <b>{0}</b>";

			public static LocString COLUMN_SORT_BY_NAME = "Sort by <b>Name</b>";

			public static LocString COLUMN_SORT_BY_STRESS = "Sort by <b>Stress</b>";

			public static LocString COLUMN_SORT_BY_HITPOINTS = "Sort by <b>Health</b>";

			public static LocString COLUMN_SORT_BY_SICKNESSES = "Sort by <b>Disease</b>";

			public static LocString COLUMN_SORT_BY_FULLNESS = "Sort by <b>Fullness</b>";

			public static LocString COLUMN_SORT_BY_EATEN_TODAY = "Sort by number of <b>Calories</b> consumed today";

			public static LocString COLUMN_SORT_BY_EXPECTATIONS = "Sort by <b>Morale</b>";

			public static LocString NA = "N/A";

			public static LocString INFORMATION_NOT_AVAILABLE_TOOLTIP = "Information is not available because {1} is in {0}";

			public static LocString NOBODY_HERE = "Nobody here...";
		}

		public class CONSUMABLESSCREEN
		{
			public static LocString TITLE = "CONSUMABLES";

			public static LocString TOOLTIP_TOGGLE_ALL = "Toggle <b>all</b> food permissions <b>colonywide</b>";

			public static LocString TOOLTIP_TOGGLE_COLUMN = "Toggle colonywide <b>{0}</b> permission";

			public static LocString TOOLTIP_TOGGLE_ROW = "Toggle <b>all consumable permissions</b> for <b>{0}</b>";

			public static LocString NEW_MINIONS_TOOLTIP_TOGGLE_ROW = "Toggle <b>all consumable permissions</b> for <b>New Duplicants</b>";

			public static LocString NEW_MINIONS_FOOD_PERMISSION_ON = string.Concat(new string[]
			{
				"<b>New Duplicants</b> are <b>allowed</b> to eat \n",
				UI.PRE_KEYWORD,
				"{0}",
				UI.PST_KEYWORD,
				"</b> by default"
			});

			public static LocString NEW_MINIONS_FOOD_PERMISSION_OFF = string.Concat(new string[]
			{
				"<b>New Duplicants</b> are <b>not allowed</b> to eat \n",
				UI.PRE_KEYWORD,
				"{0}",
				UI.PST_KEYWORD,
				" by default"
			});

			public static LocString FOOD_PERMISSION_ON = "<b>{0}</b> is <b>allowed</b> to eat " + UI.PRE_KEYWORD + "{1}" + UI.PST_KEYWORD;

			public static LocString FOOD_PERMISSION_OFF = "<b>{0}</b> is <b>not allowed</b> to eat " + UI.PRE_KEYWORD + "{1}" + UI.PST_KEYWORD;

			public static LocString FOOD_CANT_CONSUME = "<b>{0}</b> <b>physically cannot</b> eat\n" + UI.PRE_KEYWORD + "{1}" + UI.PST_KEYWORD;

			public static LocString FOOD_REFUSE = "<b>{0}</b> <b>refuses</b> to eat\n" + UI.PRE_KEYWORD + "{1}" + UI.PST_KEYWORD;

			public static LocString FOOD_AVAILABLE = "Available: {0}";

			public static LocString FOOD_MORALE = UI.PRE_KEYWORD + "Morale" + UI.PST_KEYWORD + ": {0}";

			public static LocString FOOD_QUALITY = UI.PRE_KEYWORD + "Quality" + UI.PST_KEYWORD + ": {0}";

			public static LocString FOOD_QUALITY_VS_EXPECTATION = string.Concat(new string[]
			{
				"\nThis food will give ",
				UI.PRE_KEYWORD,
				"Morale",
				UI.PST_KEYWORD,
				" <b>{0}</b> if {1} eats it"
			});

			public static LocString CANNOT_ADJUST_PERMISSIONS = "Cannot adjust consumable permissions because they're in {0}";
		}

		public class JOBSSCREEN
		{
			public static LocString TITLE = "MANAGE DUPLICANT PRIORITIES";

			public static LocString TOOLTIP_TOGGLE_ALL = "Set priority of all Errand Types colonywide";

			public static LocString HEADER_TOOLTIP = string.Concat(new string[]
			{
				"<size=16>{Job} Errand Type</size>\n\n{Details}\n\nDuplicants will first choose what ",
				UI.PRE_KEYWORD,
				"Errand Type",
				UI.PST_KEYWORD,
				" to perform based on ",
				UI.PRE_KEYWORD,
				"Duplicant Priorities",
				UI.PST_KEYWORD,
				",\nthen they will choose individual tasks within that type using ",
				UI.PRE_KEYWORD,
				"Building Priorities",
				UI.PST_KEYWORD,
				" set by the ",
				UI.FormatAsLink("Priority Tool", "PRIORITIES"),
				" ",
				UI.FormatAsHotKey(global::Action.ManagePriorities)
			});

			public static LocString HEADER_DETAILS_TOOLTIP = "{Description}\n\nAffected errands: {ChoreList}";

			public static LocString HEADER_CHANGE_TOOLTIP = string.Concat(new string[]
			{
				"Set the priority for the ",
				UI.PRE_KEYWORD,
				"{Job}",
				UI.PST_KEYWORD,
				" Errand Type colonywide\n"
			});

			public static LocString NEW_MINION_ITEM_TOOLTIP = string.Concat(new string[]
			{
				"The ",
				UI.PRE_KEYWORD,
				"{Job}",
				UI.PST_KEYWORD,
				" Errand Type is automatically a {Priority} ",
				UI.PRE_KEYWORD,
				"Priority",
				UI.PST_KEYWORD,
				" for <b>Arriving Duplicants</b>"
			});

			public static LocString ITEM_TOOLTIP = UI.PRE_KEYWORD + "{Job}" + UI.PST_KEYWORD + " Priority for {Name}:\n<b>{Priority} Priority ({PriorityValue})</b>";

			public static LocString MINION_SKILL_TOOLTIP = string.Concat(new string[]
			{
				"{Name}'s ",
				UI.PRE_KEYWORD,
				"{Attribute}",
				UI.PST_KEYWORD,
				" Skill: "
			});

			public static LocString TRAIT_DISABLED = string.Concat(new string[]
			{
				"{Name} possesses the ",
				UI.PRE_KEYWORD,
				"{Trait}",
				UI.PST_KEYWORD,
				" trait and <b>cannot</b> do ",
				UI.PRE_KEYWORD,
				"{Job}",
				UI.PST_KEYWORD,
				" Errands"
			});

			public static LocString INCREASE_ROW_PRIORITY_NEW_MINION_TOOLTIP = string.Concat(new string[]
			{
				"Prioritize ",
				UI.PRE_KEYWORD,
				"All Errands",
				UI.PST_KEYWORD,
				" for <b>New Duplicants</b>"
			});

			public static LocString DECREASE_ROW_PRIORITY_NEW_MINION_TOOLTIP = string.Concat(new string[]
			{
				"Deprioritize ",
				UI.PRE_KEYWORD,
				"All Errands",
				UI.PST_KEYWORD,
				" for <b>New Duplicants</b>"
			});

			public static LocString INCREASE_ROW_PRIORITY_MINION_TOOLTIP = string.Concat(new string[]
			{
				"Prioritize ",
				UI.PRE_KEYWORD,
				"All Errands",
				UI.PST_KEYWORD,
				" for <b>{Name}</b>"
			});

			public static LocString DECREASE_ROW_PRIORITY_MINION_TOOLTIP = string.Concat(new string[]
			{
				"Deprioritize ",
				UI.PRE_KEYWORD,
				"All Errands",
				UI.PST_KEYWORD,
				" for <b>{Name}</b>"
			});

			public static LocString INCREASE_PRIORITY_TUTORIAL = "{Hotkey} Increase Priority";

			public static LocString DECREASE_PRIORITY_TUTORIAL = "{Hotkey} Decrease Priority";

			public static LocString CANNOT_ADJUST_PRIORITY = string.Concat(new string[]
			{
				"Priorities for ",
				UI.PRE_KEYWORD,
				"{0}",
				UI.PST_KEYWORD,
				" cannot be adjusted currently because they're in {1}"
			});

			public static LocString SORT_TOOLTIP = string.Concat(new string[]
			{
				"Sort by the ",
				UI.PRE_KEYWORD,
				"{Job}",
				UI.PST_KEYWORD,
				" Errand Type"
			});

			public static LocString DISABLED_TOOLTIP = string.Concat(new string[]
			{
				"{Name} may not perform ",
				UI.PRE_KEYWORD,
				"{Job}",
				UI.PST_KEYWORD,
				" Errands"
			});

			public static LocString OPTIONS = "Options";

			public static LocString TOGGLE_ADVANCED_MODE = "Enable Proximity";

			public static LocString TOGGLE_ADVANCED_MODE_TOOLTIP = "<b>Errand Proximity Settings</b>\n\nEnabling Proximity settings tells my Duplicants to always choose the closest, most urgent errand to perform.\n\nWhen disabled, Duplicants will choose between two high priority errands based on a hidden priority hierarchy instead.\n\nEnabling Proximity helps cut down on travel time in areas with lots of high priority errands, and is useful for large colonies.";

			public static LocString RESET_SETTINGS = "Reset Priorities";

			public static LocString RESET_SETTINGS_TOOLTIP = "<b>Reset Priorities</b>\n\nReturns all priorities to their default values.\n\nProximity Enabled: Priorities will be adjusted high-to-low.\n\nProximity Disabled: All priorities will be reset to neutral.";

			public class PRIORITY
			{
				public static LocString VERYHIGH = "Very High";

				public static LocString HIGH = "High";

				public static LocString STANDARD = "Standard";

				public static LocString LOW = "Low";

				public static LocString VERYLOW = "Very Low";

				public static LocString DISABLED = "Disallowed";
			}

			public class PRIORITY_CLASS
			{
				public static LocString IDLE = "Idle";

				public static LocString BASIC = "Normal";

				public static LocString HIGH = "Urgent";

				public static LocString PERSONAL_NEEDS = "Personal Needs";

				public static LocString EMERGENCY = "Emergency";

				public static LocString COMPULSORY = "Involuntary";
			}
		}

		public class VITALSSCREEN
		{
			public static LocString HEALTH = "Health";

			public static LocString SICKNESS = "Diseases";

			public static LocString NO_SICKNESSES = "No diseases";

			public static LocString MULTIPLE_SICKNESSES = "Multiple diseases ({0})";

			public static LocString SICKNESS_REMAINING = "{0}\n({1})";

			public static LocString STRESS = "Stress";

			public static LocString EXPECTATIONS = "Expectations";

			public static LocString CALORIES = "Fullness";

			public static LocString EATEN_TODAY = "Eaten Today";

			public static LocString EATEN_TODAY_TOOLTIP = "Consumed {0} of food this cycle";

			public static LocString ATMOSPHERE_CONDITION = "Atmosphere:";

			public static LocString SUBMERSION = "Liquid Level";

			public static LocString NOT_DROWNING = "Liquid Level";

			public static LocString FOOD_EXPECTATIONS = "Food Expectation";

			public static LocString FOOD_EXPECTATIONS_TOOLTIP = "This Duplicant desires food that is {0} quality or better";

			public static LocString DECOR_EXPECTATIONS = "Decor Expectation";

			public static LocString DECOR_EXPECTATIONS_TOOLTIP = "This Duplicant desires decor that is {0} or higher";

			public static LocString QUALITYOFLIFE_EXPECTATIONS = "Morale";

			public static LocString QUALITYOFLIFE_EXPECTATIONS_TOOLTIP = "This Duplicant requires " + UI.FormatAsLink("{0} Morale", "MORALE") + ".\n\nCurrent Morale:";

			public class CONDITIONS_GROWING
			{
				public class WILD
				{
					public static LocString BASE = "<b>Wild Growth\n[Life Cycle: {0}]</b>";

					public static LocString TOOLTIP = "This plant will take {0} to grow in the wild";
				}

				public class DOMESTIC
				{
					public static LocString BASE = "<b>Domestic Growth\n[Life Cycle: {0}]</b>";

					public static LocString TOOLTIP = "This plant will take {0} to grow domestically";
				}

				public class ADDITIONAL_DOMESTIC
				{
					public static LocString BASE = "<b>Additional Domestic Growth\n[Life Cycle: {0}]</b>";

					public static LocString TOOLTIP = "This plant will take {0} to grow domestically";
				}

				public class WILD_DECOR
				{
					public static LocString BASE = "<b>Wild Growth</b>";

					public static LocString TOOLTIP = "This plant must have these requirements met to grow in the wild";
				}

				public class WILD_INSTANT
				{
					public static LocString BASE = "<b>Wild Growth\n[{0}% Throughput]</b>";

					public static LocString TOOLTIP = "This plant must have these requirements met to grow in the wild";
				}

				public class ADDITIONAL_DOMESTIC_INSTANT
				{
					public static LocString BASE = "<b>Domestic Growth\n[{0}% Throughput]</b>";

					public static LocString TOOLTIP = "This plant must have these requirements met to grow domestically";
				}
			}
		}

		public class SCHEDULESCREEN
		{
			public static LocString SCHEDULE_EDITOR = "Schedule Editor";

			public static LocString SCHEDULE_NAME_DEFAULT = "Default Schedule";

			public static LocString SCHEDULE_NAME_FORMAT = "Schedule {0}";

			public static LocString SCHEDULE_DROPDOWN_ASSIGNED = "{0} (Assigned)";

			public static LocString SCHEDULE_DROPDOWN_BLANK = "<i>Move Duplicant...</i>";

			public static LocString SCHEDULE_DOWNTIME_MORALE = "Duplicants will receive {0} Morale from the scheduled Downtime shifts";

			public static LocString RENAME_BUTTON_TOOLTIP = "Rename custom schedule";

			public static LocString ALARM_BUTTON_ON_TOOLTIP = "Toggle Notifications\n\nSounds and notifications will play when shifts change for this schedule.\n\nENABLED\n" + UI.CLICK(UI.ClickType.Click) + " to disable";

			public static LocString ALARM_BUTTON_OFF_TOOLTIP = "Toggle Notifications\n\nNo sounds or notifications will play for this schedule.\n\nDISABLED\n" + UI.CLICK(UI.ClickType.Click) + " to enable";

			public static LocString DELETE_BUTTON_TOOLTIP = "Delete Schedule";

			public static LocString PAINT_TOOLS = "Paint Tools:";

			public static LocString ADD_SCHEDULE = "Add New Schedule";

			public static LocString POO = "dar";

			public static LocString DOWNTIME_MORALE = "Downtime Morale: {0}";

			public static LocString ALARM_TITLE_ENABLED = "Alarm On";

			public static LocString ALARM_TITLE_DISABLED = "Alarm Off";

			public static LocString SETTINGS = "Settings";

			public static LocString ALARM_BUTTON = "Shift Alarms";

			public static LocString RESET_SETTINGS = "Reset Shifts";

			public static LocString RESET_SETTINGS_TOOLTIP = "Restore this schedule to default shifts";

			public static LocString DELETE_SCHEDULE = "Delete Schedule";

			public static LocString DELETE_SCHEDULE_TOOLTIP = "Remove this schedule and unassign all Duplicants from it";

			public static LocString DUPLICANT_NIGHTOWL_TOOLTIP = string.Concat(new string[]
			{
				DUPLICANTS.TRAITS.NIGHTOWL.NAME,
				"\n• All ",
				UI.PRE_KEYWORD,
				"Attributes",
				UI.PST_KEYWORD,
				" <b>+3</b> at night"
			});

			public static LocString DUPLICANT_EARLYBIRD_TOOLTIP = string.Concat(new string[]
			{
				DUPLICANTS.TRAITS.EARLYBIRD.NAME,
				"\n• All ",
				UI.PRE_KEYWORD,
				"Attributes",
				UI.PST_KEYWORD,
				" <b>+2</b> in the morning"
			});
		}

		public class COLONYLOSTSCREEN
		{
			public static LocString COLONYLOST = "COLONY LOST";

			public static LocString COLONYLOSTDESCRIPTION = "All Duplicants are dead or incapacitated.";

			public static LocString RESTARTPROMPT = "Press <color=#F44A47><b>[ESC]</b></color> to return to a previous colony, or begin a new one.";

			public static LocString DISMISSBUTTON = "DISMISS";

			public static LocString QUITBUTTON = "MAIN MENU";
		}

		public class VICTORYSCREEN
		{
			public static LocString HEADER = "SUCCESS: IMPERATIVE ACHIEVED!";

			public static LocString DESCRIPTION = "I have fulfilled the conditions of one of my Hardwired Imperatives";

			public static LocString RESTARTPROMPT = "Press <color=#F44A47><b>[ESC]</b></color> to retire the colony and begin anew.";

			public static LocString DISMISSBUTTON = "DISMISS";

			public static LocString RETIREBUTTON = "RETIRE COLONY";
		}

		public class GENESHUFFLERMESSAGE
		{
			public static LocString HEADER = "NEURAL VACILLATION COMPLETE";

			public static LocString BODY_SUCCESS = "Whew! <b>{0}'s</b> brain is still vibrating, but they've never felt better!\n\n<b>{0}</b> acquired the <b>{1}</b> trait.\n\n<b>{1}:</b>\n{2}";

			public static LocString BODY_FAILURE = "The machine attempted to alter this Duplicant, but there's no improving on perfection.\n\n<b>{0}</b> already has all positive traits!";

			public static LocString DISMISSBUTTON = "DISMISS";
		}

		public class CRASHSCREEN
		{
			public static LocString TITLE = "\"Whoops! We're sorry, but it seems your game has encountered an error. It's okay though - these errors are how we find and fix problems to make our game more fun for everyone. If you use the box below to submit a crash report to us, we can use this information to get the issue sorted out.\"";

			public static LocString TITLE_MODS = "\"Oops-a-daisy! We're sorry, but it seems your game has encountered an error. If you uncheck all of the mods below, we will be able to help the next time this happens. Any mods that could be related to this error have already been unchecked.\"";

			public static LocString HEADER = "OPTIONAL CRASH DESCRIPTION";

			public static LocString HEADER_MODS = "ACTIVE MODS";

			public static LocString BODY = "Help! A black hole ate my game!";

			public static LocString THANKYOU = "Thank you!\n\nYou're making our game better, one crash at a time.";

			public static LocString UPLOADINFO = "UPLOAD ADDITIONAL INFO ({0})";

			public static LocString REPORTBUTTON = "REPORT CRASH";

			public static LocString REPORTING = "REPORTING, PLEASE WAIT...";

			public static LocString CONTINUEBUTTON = "CONTINUE GAME";

			public static LocString MOREINFOBUTTON = "MORE INFO";

			public static LocString COPYTOCLIPBOARDBUTTON = "COPY TO CLIPBOARD";

			public static LocString QUITBUTTON = "QUIT TO DESKTOP";

			public static LocString SAVEFAILED = "Save Failed: {0}";

			public static LocString LOADFAILED = "Load Failed: {0}\nSave Version: {1}\nExpected: {2}";

			public static LocString REPORTEDERROR = "Reported Error";
		}

		public class DEMOOVERSCREEN
		{
			public static LocString TIMEREMAINING = "Demo time remaining:";

			public static LocString TIMERTOOLTIP = "Demo time remaining";

			public static LocString TIMERINACTIVE = "Timer inactive";

			public static LocString DEMOOVER = "END OF DEMO";

			public static LocString DESCRIPTION = "Thank you for playing <color=#F44A47>Oxygen Not Included</color>!";

			public static LocString DESCRIPTION_2 = "";

			public static LocString QUITBUTTON = "RESET";
		}

		public class CREDITSSCREEN
		{
			public static LocString TITLE = "CREDITS";

			public static LocString CLOSEBUTTON = "CLOSE";

			public class THIRD_PARTY
			{
				public static LocString FMOD = "FMOD Sound System\nCopyright Firelight Technologies";

				public static LocString HARMONY = "Harmony by Andreas Pardeike";
			}
		}

		public class ALLRESOURCESSCREEN
		{
			public static LocString RESOURCES_TITLE = "RESOURCES";

			public static LocString RESOURCES = "Resources";

			public static LocString SEARCH = "Search";

			public static LocString NAME = "Resource";

			public static LocString TOTAL = "Total";

			public static LocString AVAILABLE = "Available";

			public static LocString RESERVED = "Reserved";

			public static LocString SEARCH_PLACEHODLER = "Enter text...";

			public static LocString FIRST_FRAME_NO_DATA = "...";

			public static LocString PIN_TOOLTIP = "Check to pin resource to side panel";

			public static LocString UNPIN_TOOLTIP = "Unpin resource";
		}

		public class PRIORITYSCREEN
		{
			public static LocString BASIC = "Set the order in which specific pending errands should be done\n\n1: Least Urgent\n9: Most Urgent";

			public static LocString HIGH = "";

			public static LocString TOP_PRIORITY = "Top Priority\n\nThis priority will override all other priorities and set the colony on Yellow Alert until the errand is completed";

			public static LocString HIGH_TOGGLE = "";

			public static LocString OPEN_JOBS_SCREEN = string.Concat(new string[]
			{
				UI.CLICK(UI.ClickType.Click),
				" to open the Priorities Screen\n\nDuplicants will first decide what to work on based on their ",
				UI.PRE_KEYWORD,
				"Duplicant Priorities",
				UI.PST_KEYWORD,
				", and then decide where to work based on ",
				UI.PRE_KEYWORD,
				"Building Priorities",
				UI.PST_KEYWORD
			});

			public static LocString DIAGRAM = string.Concat(new string[]
			{
				"Duplicants will first choose what ",
				UI.PRE_KEYWORD,
				"Errand Type",
				UI.PST_KEYWORD,
				" to perform using their ",
				UI.PRE_KEYWORD,
				"Duplicant Priorities",
				UI.PST_KEYWORD,
				" ",
				UI.FormatAsHotKey(global::Action.ManagePriorities),
				"\n\nThey will then choose one ",
				UI.PRE_KEYWORD,
				"Errand",
				UI.PST_KEYWORD,
				" from within that type using the ",
				UI.PRE_KEYWORD,
				"Building Priorities",
				UI.PST_KEYWORD,
				" set by this tool"
			});

			public static LocString DIAGRAM_TITLE = "BUILDING PRIORITY";
		}

		public class RESOURCESCREEN
		{
			public static LocString HEADER = "RESOURCES";

			public static LocString CATEGORY_TOOLTIP = "Counts all unallocated resources within reach\n\n" + UI.CLICK(UI.ClickType.Click) + " to expand";

			public static LocString AVAILABLE_TOOLTIP = "Available: <b>{0}</b>\n({1} of {2} allocated to pending errands)";

			public static LocString TREND_TOOLTIP = "The available amount of this resource has {0} {1} in the last cycle";

			public static LocString TREND_TOOLTIP_NO_CHANGE = "The available amount of this resource has NOT CHANGED in the last cycle";

			public static LocString FLAT_STR = "<b>NOT CHANGED</b>";

			public static LocString INCREASING_STR = "<color=" + Constants.POSITIVE_COLOR_STR + ">INCREASED</color>";

			public static LocString DECREASING_STR = "<color=" + Constants.NEGATIVE_COLOR_STR + ">DECREASED</color>";

			public static LocString CLEAR_NEW_RESOURCES = "Clear New";

			public static LocString CLEAR_ALL = "Unpin all resources";

			public static LocString SEE_ALL = "+ See All ({0})";

			public static LocString NEW_TAG = "NEW";
		}

		public class CONFIRMDIALOG
		{
			public static LocString OK = "OK";

			public static LocString CANCEL = "CANCEL";

			public static LocString DIALOG_HEADER = "MESSAGE";
		}

		public class FACADE_SELECTION_PANEL
		{
			public static LocString HEADER = "Select Blueprint";

			public static LocString STORE_BUTTON_TOOLTIP = "More Blueprints";
		}

		public class FILE_NAME_DIALOG
		{
			public static LocString ENTER_TEXT = "Enter Text...";
		}

		public class MINION_IDENTITY_SORT
		{
			public static LocString TITLE = "Sort By";

			public static LocString NAME = "Duplicant";

			public static LocString ROLE = "Role";

			public static LocString PERMISSION = "Permission";
		}

		public class UISIDESCREENS
		{
			public class ARTABLESELECTIONSIDESCREEN
			{
				public static LocString TITLE = "Style Selection";

				public static LocString BUTTON = "Repaint";

				public static LocString BUTTON_TOOLTIP = "Clears current artwork\n\nCreates errand for a skilled Duplicant to paint selected style";

				public static LocString CLEAR_BUTTON_TOOLTIP = "Clears current artwork\n\nAllows a skilled Duplicant to create artwork of their choice";
			}

			public class ARTIFACTANALYSISSIDESCREEN
			{
				public static LocString NO_ARTIFACTS_DISCOVERED = "No artifacts analyzed";

				public static LocString NO_ARTIFACTS_DISCOVERED_TOOLTIP = "Analyzing artifacts requires a Duplicant with the Masterworks skill";
			}

			public class BUTTONMENUSIDESCREEN
			{
				public static LocString TITLE = "Building Menu";

				public static LocString ALLOW_INTERNAL_CONSTRUCTOR = "Enable Auto-Delivery";

				public static LocString ALLOW_INTERNAL_CONSTRUCTOR_TOOLTIP = "Order Duplicants to deliver {0}" + UI.FormatAsLink("s", "NONE") + " to this building automatically when they need replacing";

				public static LocString DISALLOW_INTERNAL_CONSTRUCTOR = "Cancel Auto-Delivery";

				public static LocString DISALLOW_INTERNAL_CONSTRUCTOR_TOOLTIP = "Cancel automatic {0} deliveries to this building";
			}

			public class CONFIGURECONSUMERSIDESCREEN
			{
				public static LocString TITLE = "Configure Building";

				public static LocString SELECTION_DESCRIPTION_HEADER = "Description";
			}

			public class TREEFILTERABLESIDESCREEN
			{
				public static LocString TITLE = "Element Filter";

				public static LocString TITLE_CRITTER = "Critter Filter";

				public static LocString ALLBUTTON = "All";

				public static LocString ALLBUTTONTOOLTIP = "Allow storage of all resource categories in this container";

				public static LocString CATEGORYBUTTONTOOLTIP = "Allow storage of anything in the {0} resource category";

				public static LocString MATERIALBUTTONTOOLTIP = "Add or remove this material from storage";

				public static LocString ONLYALLOWTRANSPORTITEMSBUTTON = "Sweep Only";

				public static LocString ONLYALLOWTRANSPORTITEMSBUTTONTOOLTIP = "Only store objects marked Sweep <color=#F44A47><b>[K]</b></color> in this container";

				public static LocString ONLYALLOWSPICEDITEMSBUTTON = "Spiced Food Only";

				public static LocString ONLYALLOWSPICEDITEMSBUTTONTOOLTIP = "Only store foods that have been spiced at the " + UI.PRE_KEYWORD + "Spice Grinder" + UI.PST_KEYWORD;
			}

			public class TELESCOPESIDESCREEN
			{
				public static LocString TITLE = "Telescope Configuration";

				public static LocString NO_SELECTED_ANALYSIS_TARGET = "No analysis focus selected\nOpen the " + UI.FormatAsManagementMenu("Starmap", global::Action.ManageStarmap) + " to select a focus";

				public static LocString ANALYSIS_TARGET_SELECTED = "Object focus selected\nAnalysis underway";

				public static LocString OPENSTARMAPBUTTON = "OPEN STARMAP";

				public static LocString ANALYSIS_TARGET_HEADER = "Object Analysis";
			}

			public class CLUSTERTELESCOPESIDESCREEN
			{
				public static LocString TITLE = "Telescope Configuration";

				public static LocString CHECKBOX_METEORS = "Allow meteor shower identification";

				public static LocString CHECKBOX_TOOLTIP_METEORS = string.Concat(new string[]
				{
					"Prioritizes unidentified meteors that come within range in a previously revealed location\n\nWill interrupt a Duplicant working on revealing a new ",
					UI.PRE_KEYWORD,
					"Starmap",
					UI.PST_KEYWORD,
					" location"
				});
			}

			public class TEMPORALTEARSIDESCREEN
			{
				public static LocString TITLE = "Temporal Tear";

				public static LocString BUTTON_OPEN = "Enter Tear";

				public static LocString BUTTON_CLOSED = "Tear Closed";

				public static LocString BUTTON_LABEL = "Enter Temporal Tear";

				public static LocString CONFIRM_POPUP_MESSAGE = "Are you sure you want to fire this?";

				public static LocString CONFIRM_POPUP_CONFIRM = "Yes, I'm ready for a meteor shower.";

				public static LocString CONFIRM_POPUP_CANCEL = "No, I need more time to prepare.";

				public static LocString CONFIRM_POPUP_TITLE = "Temporal Tear Opener";
			}

			public class RAILGUNSIDESCREEN
			{
				public static LocString TITLE = "Launcher Configuration";

				public static LocString NO_SELECTED_LAUNCH_TARGET = "No destination selected\nOpen the " + UI.FormatAsManagementMenu("Starmap", global::Action.ManageStarmap) + " to set a course";

				public static LocString LAUNCH_TARGET_SELECTED = "Launcher destination {0} set";

				public static LocString OPENSTARMAPBUTTON = "OPEN STARMAP";

				public static LocString LAUNCH_RESOURCES_HEADER = "Launch Resources:";

				public static LocString MINIMUM_PAYLOAD_MASS = "Minimum launch mass:";
			}

			public class CLUSTERWORLDSIDESCREEN
			{
				public static LocString TITLE = UI.CLUSTERMAP.PLANETOID;

				public static LocString VIEW_WORLD = "Oversee " + UI.CLUSTERMAP.PLANETOID;

				public static LocString VIEW_WORLD_DISABLE_TOOLTIP = "Cannot view " + UI.CLUSTERMAP.PLANETOID;

				public static LocString VIEW_WORLD_TOOLTIP = "View this " + UI.CLUSTERMAP.PLANETOID + "'s surface";
			}

			public class ROCKETMODULESIDESCREEN
			{
				public static LocString TITLE = "Rocket Module";

				public static LocString CHANGEMODULEPANEL = "Add or Change Module";

				public static LocString ENGINE_MAX_HEIGHT = "This engine allows a <b>Maximum Rocket Height</b> of {0}";

				public class MODULESTATCHANGE
				{
					public static LocString TITLE = "Rocket stats on construction:";

					public static LocString BURDEN = "    • " + DUPLICANTS.ATTRIBUTES.ROCKETBURDEN.NAME + ": {0} ({1})";

					public static LocString RANGE = string.Concat(new string[]
					{
						"    • Potential ",
						DUPLICANTS.ATTRIBUTES.FUELRANGEPERKILOGRAM.NAME,
						": {0}/1",
						UI.UNITSUFFIXES.MASS.KILOGRAM,
						" Fuel ({1})"
					});

					public static LocString SPEED = "    • Speed: {0} ({1})";

					public static LocString ENGINEPOWER = "    • " + DUPLICANTS.ATTRIBUTES.ROCKETENGINEPOWER.NAME + ": {0} ({1})";

					public static LocString HEIGHT = "    • " + DUPLICANTS.ATTRIBUTES.HEIGHT.NAME + ": {0}/{2} ({1})";

					public static LocString HEIGHT_NOMAX = "    • " + DUPLICANTS.ATTRIBUTES.HEIGHT.NAME + ": {0} ({1})";

					public static LocString POSITIVEDELTA = UI.FormatAsPositiveModifier("{0}");

					public static LocString NEGATIVEDELTA = UI.FormatAsNegativeModifier("{0}");
				}

				public class BUTTONSWAPMODULEUP
				{
					public static LocString DESC = "Swap this rocket module with the one above";

					public static LocString INVALID = "No module above may be swapped.\n\n    • A module above may be unable to have modules placed above it.\n    • A module above may be unable to fit into the space below it.\n    • This module may be unable to fit into the space above it.";
				}

				public class BUTTONVIEWINTERIOR
				{
					public static LocString LABEL = "View Interior";

					public static LocString DESC = "What's goin' on in there?";

					public static LocString INVALID = "This module does not have an interior view";
				}

				public class BUTTONVIEWEXTERIOR
				{
					public static LocString LABEL = "View Exterior";

					public static LocString DESC = "Switch to external world view";

					public static LocString INVALID = "Not available in flight";
				}

				public class BUTTONSWAPMODULEDOWN
				{
					public static LocString DESC = "Swap this rocket module with the one below";

					public static LocString INVALID = "No module below may be swapped.\n\n    • A module below may be unable to have modules placed below it.\n    • A module below may be unable to fit into the space above it.\n    • This module may be unable to fit into the space below it.";
				}

				public class BUTTONCHANGEMODULE
				{
					public static LocString DESC = "Swap this module for a different module";

					public static LocString INVALID = "This module cannot be changed to a different type";
				}

				public class BUTTONREMOVEMODULE
				{
					public static LocString DESC = "Remove this module";

					public static LocString INVALID = "This module cannot be removed";
				}

				public class ADDMODULE
				{
					public static LocString DESC = "Add a new module above this one";

					public static LocString INVALID = "Modules cannot be added above this module, or there is no room above to add a module";
				}
			}

			public class CLUSTERLOCATIONFILTERSIDESCREEN
			{
				public static LocString TITLE = "Location Filter";

				public static LocString HEADER = "Send Green signal at locations";

				public static LocString EMPTY_SPACE_ROW = "In Space";
			}

			public class DISPENSERSIDESCREEN
			{
				public static LocString TITLE = "Dispenser";

				public static LocString BUTTON_CANCEL = "Cancel order";

				public static LocString BUTTON_DISPENSE = "Dispense item";
			}

			public class ROCKETRESTRICTIONSIDESCREEN
			{
				public static LocString TITLE = "Rocket Restrictions";

				public static LocString BUILDING_RESTRICTIONS_LABEL = "Interior Building Restrictions";

				public static LocString NONE_RESTRICTION_BUTTON = "None";

				public static LocString NONE_RESTRICTION_BUTTON_TOOLTIP = "There are no restrictions on buildings inside this rocket";

				public static LocString GROUNDED_RESTRICTION_BUTTON = "Grounded";

				public static LocString GROUNDED_RESTRICTION_BUTTON_TOOLTIP = "Buildings with their access restricted cannot be operated while grounded, though they can still be filled";

				public static LocString AUTOMATION = "Automation Controlled";

				public static LocString AUTOMATION_TOOLTIP = "Building restrictions are managed by automation\n\nBuildings with their access restricted cannot be operated, though they can still be filled";
			}

			public class HABITATMODULESIDESCREEN
			{
				public static LocString TITLE = "Spacefarer Module";

				public static LocString VIEW_BUTTON = "View Interior";

				public static LocString VIEW_BUTTON_TOOLTIP = "What's goin' on in there?";
			}

			public class HARVESTMODULESIDESCREEN
			{
				public static LocString TITLE = "Resource Gathering";

				public static LocString MINING_IN_PROGRESS = "Drilling...";

				public static LocString MINING_STOPPED = "Not drilling";

				public static LocString ENABLE = "Enable Drill";

				public static LocString DISABLE = "Disable Drill";
			}

			public class SELECTMODULESIDESCREEN
			{
				public static LocString TITLE = "Select Module";

				public static LocString BUILDBUTTON = "Build";

				public class CONSTRAINTS
				{
					public class RESEARCHED
					{
						public static LocString COMPLETE = "Research Completed";

						public static LocString FAILED = "Research Incomplete";
					}

					public class MATERIALS_AVAILABLE
					{
						public static LocString COMPLETE = "Materials available";

						public static LocString FAILED = "• Materials unavailable";
					}

					public class ONE_COMMAND_PER_ROCKET
					{
						public static LocString COMPLETE = "";

						public static LocString FAILED = "• Command module already installed";
					}

					public class ONE_ENGINE_PER_ROCKET
					{
						public static LocString COMPLETE = "";

						public static LocString FAILED = "• Engine module already installed";
					}

					public class ENGINE_AT_BOTTOM
					{
						public static LocString COMPLETE = "";

						public static LocString FAILED = "• Must install at bottom of rocket";
					}

					public class TOP_ONLY
					{
						public static LocString COMPLETE = "";

						public static LocString FAILED = "• Must install at top of rocket";
					}

					public class SPACE_AVAILABLE
					{
						public static LocString COMPLETE = "";

						public static LocString FAILED = "• Space above rocket blocked";
					}

					public class PASSENGER_MODULE_AVAILABLE
					{
						public static LocString COMPLETE = "";

						public static LocString FAILED = "• Max number of passenger modules installed";
					}

					public class MAX_MODULES
					{
						public static LocString COMPLETE = "";

						public static LocString FAILED = "• Max module limit of engine reached";
					}

					public class MAX_HEIGHT
					{
						public static LocString COMPLETE = "";

						public static LocString FAILED = "• Engine's height limit reached or exceeded";

						public static LocString FAILED_NO_ENGINE = "• Rocket requires space for an engine";
					}
				}
			}

			public class FILTERSIDESCREEN
			{
				public static LocString TITLE = "Filter Outputs";

				public static LocString NO_SELECTION = "None";

				public static LocString OUTPUTELEMENTHEADER = "Output 1";

				public static LocString SELECTELEMENTHEADER = "Output 2";

				public static LocString OUTPUTRED = "Output Red";

				public static LocString OUTPUTGREEN = "Output Green";

				public static LocString NOELEMENTSELECTED = "No element selected";

				public static class UNFILTEREDELEMENTS
				{
					public static LocString GAS = "Gas Output:\nAll";

					public static LocString LIQUID = "Liquid Output:\nAll";

					public static LocString SOLID = "Solid Output:\nAll";
				}

				public static class FILTEREDELEMENT
				{
					public static LocString GAS = "Filtered Gas Output:\n{0}";

					public static LocString LIQUID = "Filtered Liquid Output:\n{0}";

					public static LocString SOLID = "Filtered Solid Output:\n{0}";
				}
			}

			public class LOGICBROADCASTCHANNELSIDESCREEN
			{
				public static LocString TITLE = "Channel Selector";

				public static LocString HEADER = "Channel Selector";

				public static LocString IN_RANGE = "In Range";

				public static LocString OUT_OF_RANGE = "Out of Range";

				public static LocString NO_SENDERS = "No Channels Available";

				public static LocString NO_SENDERS_DESC = "Build a " + BUILDINGS.PREFABS.LOGICINTERASTEROIDSENDER.NAME + " to transmit a signal.";
			}

			public class CONDITIONLISTSIDESCREEN
			{
				public static LocString TITLE = "Condition List";
			}

			public class FABRICATORSIDESCREEN
			{
				public static LocString TITLE = "{0} Production Orders";

				public static LocString SUBTITLE = "Recipes";

				public static LocString NORECIPEDISCOVERED = "No discovered recipes";

				public static LocString NORECIPEDISCOVERED_BODY = "Discover new ingredients or research new technology to unlock some recipes.";

				public static LocString NORECIPESELECTED = "No recipe selected";

				public static LocString SELECTRECIPE = "Select a recipe to fabricate.";

				public static LocString COST = "<b>Ingredients:</b>\n";

				public static LocString RESULTREQUIREMENTS = "<b>Requirements:</b>";

				public static LocString RESULTEFFECTS = "<b>Effects:</b>";

				public static LocString KG = "- {0}: {1}\n";

				public static LocString INFORMATION = "INFORMATION";

				public static LocString CANCEL = "Cancel";

				public static LocString RECIPERQUIREMENT = "{0}: {1} / {2}";

				public static LocString RECIPEPRODUCT = "{0}: {1}";

				public static LocString UNITS_AND_CALS = "{0} [{1}]";

				public static LocString CALS = "{0}";

				public static LocString QUEUED_MISSING_INGREDIENTS_TOOLTIP = "Missing {0} of {1}\n";

				public static LocString CURRENT_ORDER = "Current order: {0}";

				public static LocString NEXT_ORDER = "Next order: {0}";

				public static LocString NO_WORKABLE_ORDER = "No workable order";

				public static LocString RECIPE_DETAILS = "Recipe Details";

				public static LocString RECIPE_QUEUE = "Order Production Quantity:";

				public static LocString RECIPE_FOREVER = "Forever";

				public static LocString CHANGE_RECIPE_ARROW_LABEL = "Change recipe";

				public static LocString INGREDIENTS = "<b>Ingredients:</b>";

				public static LocString RECIPE_EFFECTS = "<b>Effects:</b>";

				public static LocString ALLOW_MUTANT_SEED_INGREDIENTS = "Building accepts mutant seeds";

				public static LocString ALLOW_MUTANT_SEED_INGREDIENTS_TOOLTIP = "Toggle whether Duplicants will deliver mutant seed species to this building as recipe ingredients.";

				public class TOOLTIPS
				{
					public static LocString RECIPERQUIREMENT_SUFFICIENT = "This recipe consumes {1} of an available {2} of {0}";

					public static LocString RECIPERQUIREMENT_INSUFFICIENT = "This recipe requires {1} {0}\nAvailable: {2}";

					public static LocString RECIPEPRODUCT = "This recipe produces {1} {0}";
				}

				public class EFFECTS
				{
					public static LocString OXYGEN_TANK = STRINGS.EQUIPMENT.PREFABS.OXYGEN_TANK.NAME + " ({0})";

					public static LocString OXYGEN_TANK_UNDERWATER = STRINGS.EQUIPMENT.PREFABS.OXYGEN_TANK_UNDERWATER.NAME + " ({0})";

					public static LocString JETSUIT_TANK = STRINGS.EQUIPMENT.PREFABS.JET_SUIT.TANK_EFFECT_NAME + " ({0})";

					public static LocString LEADSUIT_BATTERY = STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.BATTERY_EFFECT_NAME + " ({0})";

					public static LocString COOL_VEST = STRINGS.EQUIPMENT.PREFABS.COOL_VEST.NAME + " ({0})";

					public static LocString WARM_VEST = STRINGS.EQUIPMENT.PREFABS.WARM_VEST.NAME + " ({0})";

					public static LocString FUNKY_VEST = STRINGS.EQUIPMENT.PREFABS.FUNKY_VEST.NAME + " ({0})";

					public static LocString RESEARCHPOINT = "{0}: +1";
				}

				public class RECIPE_CATEGORIES
				{
					public static LocString ATMO_SUIT_FACADES = "Atmo Suit Styles";

					public static LocString JET_SUIT_FACADES = "Jet Suit Styles";

					public static LocString LEAD_SUIT_FACADES = "Lead Suit Styles";

					public static LocString PRIMO_GARB_FACADES = "Primo Garb Styles";
				}
			}

			public class ASSIGNMENTGROUPCONTROLLER
			{
				public static LocString TITLE = "Duplicant Assignment";

				public static LocString PILOT = "Pilot";

				public static LocString OFFWORLD = "Offworld";

				public class TOOLTIPS
				{
					public static LocString DIFFERENT_WORLD = "This Duplicant is on a different " + UI.CLUSTERMAP.PLANETOID;

					public static LocString ASSIGN = "<b>Add</b> this Duplicant to rocket crew";

					public static LocString UNASSIGN = "<b>Remove</b> this Duplicant from rocket crew";
				}
			}

			public class LAUNCHPADSIDESCREEN
			{
				public static LocString TITLE = "Rocket Platform";

				public static LocString WAITING_TO_LAND_PANEL = "Waiting to land";

				public static LocString NO_ROCKETS_WAITING = "No rockets in orbit";

				public static LocString IN_ORBIT_ABOVE_PANEL = "Rockets in orbit";

				public static LocString NEW_ROCKET_BUTTON = "NEW ROCKET";

				public static LocString LAND_BUTTON = "LAND HERE";

				public static LocString CANCEL_LAND_BUTTON = "CANCEL";

				public static LocString LAUNCH_BUTTON = "BEGIN LAUNCH SEQUENCE";

				public static LocString LAUNCH_BUTTON_DEBUG = "BEGIN LAUNCH SEQUENCE (DEBUG ENABLED)";

				public static LocString LAUNCH_BUTTON_TOOLTIP = "Blast off!";

				public static LocString LAUNCH_BUTTON_NOT_READY_TOOLTIP = "This rocket is <b>not</b> ready to launch\n\n<b>Review the Launch Checklist in the status panel for more detail</b>";

				public static LocString LAUNCH_WARNINGS_BUTTON = "ACKNOWLEDGE WARNINGS";

				public static LocString LAUNCH_WARNINGS_BUTTON_TOOLTIP = "Some items in the Launch Checklist require attention\n\n<b>" + UI.CLICK(UI.ClickType.Click) + " to ignore warnings and proceed with launch</b>";

				public static LocString LAUNCH_REQUESTED_BUTTON = "CANCEL LAUNCH";

				public static LocString LAUNCH_REQUESTED_BUTTON_TOOLTIP = "This rocket will take off as soon as a Duplicant takes the controls\n\n<b>" + UI.CLICK(UI.ClickType.Click) + " to cancel launch</b>";

				public static LocString LAUNCH_AUTOMATION_CONTROLLED = "AUTOMATION CONTROLLED";

				public static LocString LAUNCH_AUTOMATION_CONTROLLED_TOOLTIP = "This " + BUILDINGS.PREFABS.LAUNCHPAD.NAME + "'s launch operation is controlled by automation signals";

				public class STATUS
				{
					public static LocString STILL_PREPPING = "Launch Checklist Incomplete";

					public static LocString READY_FOR_LAUNCH = "Ready to Launch";

					public static LocString LOADING_CREW = "Loading crew...";

					public static LocString UNLOADING_PASSENGERS = "Unloading non-crew...";

					public static LocString WAITING_FOR_PILOT = "Pilot requested at control station...";

					public static LocString COUNTING_DOWN = "5... 4... 3... 2... 1...";

					public static LocString TAKING_OFF = "Liftoff!!";
				}
			}

			public class AUTOPLUMBERSIDESCREEN
			{
				public static LocString TITLE = "Automatic Building Configuration";

				public class BUTTONS
				{
					public class POWER
					{
						public static LocString TOOLTIP = "Add Dev Generator and Electrical Wires";
					}

					public class PIPES
					{
						public static LocString TOOLTIP = "Add Dev Pumps and Pipes";
					}

					public class SOLIDS
					{
						public static LocString TOOLTIP = "Spawn solid resources for a relevant recipe or conversions";
					}

					public class MINION
					{
						public static LocString TOOLTIP = "Spawn a Duplicant in front of the building";
					}

					public class FACADE
					{
						public static LocString TOOLTIP = "Toggle the building blueprint";
					}
				}
			}

			public class SELFDESTRUCTSIDESCREEN
			{
				public static LocString TITLE = "Self Destruct";

				public static LocString MESSAGE_TEXT = "EMERGENCY PROCEDURES";

				public static LocString BUTTON_TEXT = "ABANDON SHIP!";

				public static LocString BUTTON_TEXT_CONFIRM = "CONFIRM ABANDON SHIP";

				public static LocString BUTTON_TOOLTIP = "This rocket is equipped with an emergency escape system.\n\nThe rocket's self-destruct sequence can be triggered to destroy it and propel fragments of the ship towards the nearest planetoid.\n\nAny Duplicants on board will be safely delivered in escape pods.";

				public static LocString BUTTON_TOOLTIP_CONFIRM = "<b>This will eject any passengers and destroy the rocket.<b>\n\nThe rocket's self-destruct sequence can be triggered to destroy it and propel fragments of the ship towards the nearest planetoid.\n\nAny Duplicants on board will be safely delivered in escape pods.";
			}

			public class GENESHUFFLERSIDESREEN
			{
				public static LocString TITLE = "Neural Vacillator";

				public static LocString COMPLETE = "Something feels different.";

				public static LocString UNDERWAY = "Neural Vacillation in progress.";

				public static LocString CONSUMED = "There are no charges left in this Vacillator.";

				public static LocString CONSUMED_WAITING = "Recharge requested, awaiting delivery by Duplicant.";

				public static LocString BUTTON = "Complete Neural Process";

				public static LocString BUTTON_RECHARGE = "Recharge";

				public static LocString BUTTON_RECHARGE_CANCEL = "Cancel Recharge";
			}

			public class MINIONTODOSIDESCREEN
			{
				public static LocString CURRENT_TITLE = "Current Errand";

				public static LocString LIST_TITLE = "\"To Do\" List";

				public static LocString CURRENT_SCHEDULE_BLOCK = "Schedule Block: {0}";

				public static LocString CHORE_TARGET = "{Target}";

				public static LocString CHORE_TARGET_AND_GROUP = "{Target} -- {Groups}";

				public static LocString SELF_LABEL = "Self";

				public static LocString TRUNCATED_CHORES = "{0} more";

				public static LocString TOOLTIP_IDLE = string.Concat(new string[]
				{
					"{IdleDescription}\n\nDuplicants will only <b>{Errand}</b> when there is nothing else for them to do\n\nTotal ",
					UI.PRE_KEYWORD,
					"Priority",
					UI.PST_KEYWORD,
					": {TotalPriority}\n    • ",
					UI.JOBSSCREEN.PRIORITY_CLASS.IDLE,
					": {ClassPriority}\n    • All {BestGroup} Errands: {TypePriority}"
				});

				public static LocString TOOLTIP_NORMAL = string.Concat(new string[]
				{
					"{Description}\n\nErrand Type: {Groups}\n\nTotal ",
					UI.PRE_KEYWORD,
					"Priority",
					UI.PST_KEYWORD,
					": {TotalPriority}\n    • {Name}'s {BestGroup} Priority: {PersonalPriorityValue} ({PersonalPriority})\n    • This {Building}'s Priority: {BuildingPriority}\n    • All {BestGroup} Errands: {TypePriority}"
				});

				public static LocString TOOLTIP_PERSONAL = string.Concat(new string[]
				{
					"{Description}\n\n<b>{Errand}</b> is a ",
					UI.JOBSSCREEN.PRIORITY_CLASS.PERSONAL_NEEDS,
					" errand and so will be performed before all Regular errands\n\nTotal ",
					UI.PRE_KEYWORD,
					"Priority",
					UI.PST_KEYWORD,
					": {TotalPriority}\n    • ",
					UI.JOBSSCREEN.PRIORITY_CLASS.PERSONAL_NEEDS,
					": {ClassPriority}\n    • All {BestGroup} Errands: {TypePriority}"
				});

				public static LocString TOOLTIP_EMERGENCY = string.Concat(new string[]
				{
					"{Description}\n\n<b>{Errand}</b> is an ",
					UI.JOBSSCREEN.PRIORITY_CLASS.EMERGENCY,
					" errand and so will be performed before all Regular and Personal errands\n\nTotal ",
					UI.PRE_KEYWORD,
					"Priority",
					UI.PST_KEYWORD,
					": {TotalPriority}\n    • ",
					UI.JOBSSCREEN.PRIORITY_CLASS.EMERGENCY,
					" : {ClassPriority}\n    • This {Building}'s Priority: {BuildingPriority}\n    • All {BestGroup} Errands: {TypePriority}"
				});

				public static LocString TOOLTIP_COMPULSORY = string.Concat(new string[]
				{
					"{Description}\n\n<b>{Errand}</b> is a ",
					UI.JOBSSCREEN.PRIORITY_CLASS.COMPULSORY,
					" action and so will occur immediately\n\nTotal ",
					UI.PRE_KEYWORD,
					"Priority",
					UI.PST_KEYWORD,
					": {TotalPriority}\n    • ",
					UI.JOBSSCREEN.PRIORITY_CLASS.COMPULSORY,
					": {ClassPriority}\n    • All {BestGroup} Errands: {TypePriority}"
				});

				public static LocString TOOLTIP_DESC_ACTIVE = "{Name}'s Current Errand: <b>{Errand}</b>";

				public static LocString TOOLTIP_DESC_INACTIVE = "{Name} could work on <b>{Errand}</b>, but it's not their top priority right now";

				public static LocString TOOLTIP_IDLEDESC_ACTIVE = "{Name} is currently <b>Idle</b>";

				public static LocString TOOLTIP_IDLEDESC_INACTIVE = "{Name} could become <b>Idle</b> when all other errands are canceled or completed";

				public static LocString TOOLTIP_NA = "--";

				public static LocString CHORE_GROUP_SEPARATOR = " or ";
			}

			public class MODULEFLIGHTUTILITYSIDESCREEN
			{
				public static LocString TITLE = "Deployables";

				public static LocString DEPLOY_BUTTON = "Deploy";

				public static LocString DEPLOY_BUTTON_TOOLTIP = "Send this module's contents to the surface of the currently orbited " + UI.CLUSTERMAP.PLANETOID_KEYWORD + "\n\nA specific deploy location may need to be chosen for certain modules";

				public static LocString REPEAT_BUTTON_TOOLTIP = "Automatically deploy this module's contents when a destination orbit is reached";

				public static LocString SELECT_DUPLICANT = "Select Duplicant";

				public static LocString PILOT_FMT = "{0} - Pilot";
			}

			public class HIGHENERGYPARTICLEDIRECTIONSIDESCREEN
			{
				public static LocString TITLE = "Emitting Particle Direction";

				public static LocString SELECTED_DIRECTION = "Selected direction: {0}";

				public static LocString DIRECTION_N = "N";

				public static LocString DIRECTION_NE = "NE";

				public static LocString DIRECTION_E = "E";

				public static LocString DIRECTION_SE = "SE";

				public static LocString DIRECTION_S = "S";

				public static LocString DIRECTION_SW = "SW";

				public static LocString DIRECTION_W = "W";

				public static LocString DIRECTION_NW = "NW";
			}

			public class MONUMENTSIDESCREEN
			{
				public static LocString TITLE = "Great Monument";

				public static LocString FLIP_FACING_BUTTON = UI.CLICK(UI.ClickType.CLICK) + " TO ROTATE";
			}

			public class PLANTERSIDESCREEN
			{
				public static LocString TITLE = "{0} Seeds";

				public static LocString INFORMATION = "INFORMATION";

				public static LocString AWAITINGREQUEST = "PLANT: {0}";

				public static LocString AWAITINGDELIVERY = "AWAITING DELIVERY: {0}";

				public static LocString AWAITINGREMOVAL = "AWAITING DIGGING UP: {0}";

				public static LocString ENTITYDEPOSITED = "PLANTED: {0}";

				public static LocString MUTATIONS_HEADER = "Mutations";

				public static LocString DEPOSIT = "Plant";

				public static LocString CANCELDEPOSIT = "Cancel";

				public static LocString REMOVE = "Uproot";

				public static LocString CANCELREMOVAL = "Cancel";

				public static LocString SELECT_TITLE = "SELECT";

				public static LocString SELECT_DESC = "Select a seed to plant.";

				public static LocString LIFECYCLE = "<b>Life Cycle</b>:";

				public static LocString PLANTREQUIREMENTS = "<b>Growth Requirements</b>:";

				public static LocString PLANTEFFECTS = "<b>Effects</b>:";

				public static LocString NUMBEROFHARVESTS = "Harvests: {0}";

				public static LocString YIELD = "{0}: {1} ";

				public static LocString YIELD_NONFOOD = "{0}: {1} ";

				public static LocString YIELD_SINGLE = "{0}";

				public static LocString YIELDPERHARVEST = "{0} {1} per harvest";

				public static LocString TOTALHARVESTCALORIESWITHPERUNIT = "{0} [{1} / unit]";

				public static LocString TOTALHARVESTCALORIES = "{0}";

				public static LocString BONUS_SEEDS = "Base " + UI.FormatAsLink("Seed", "PLANTS") + " Harvest Chance: {0}";

				public static LocString YIELD_SEED = "{1} {0}";

				public static LocString YIELD_SEED_SINGLE = "{0}";

				public static LocString YIELD_SEED_FINAL_HARVEST = "{1} {0} - Final harvest only";

				public static LocString YIELD_SEED_SINGLE_FINAL_HARVEST = "{0} - Final harvest only";

				public static LocString ROTATION_NEED_FLOOR = "<b>Requires upward plot orientation.</b>";

				public static LocString ROTATION_NEED_WALL = "<b>Requires sideways plot orientation.</b>";

				public static LocString ROTATION_NEED_CEILING = "<b>Requires downward plot orientation.</b>";

				public static LocString NO_SPECIES_SELECTED = "Select a seed species above...";

				public static LocString DISEASE_DROPPER_BURST = "{Disease} Burst: {DiseaseAmount}";

				public static LocString DISEASE_DROPPER_CONSTANT = "{Disease}: {DiseaseAmount}";

				public static LocString DISEASE_ON_HARVEST = "{Disease} on crop: {DiseaseAmount}";

				public static LocString AUTO_SELF_HARVEST = "Self-Harvest On Grown";

				public class TOOLTIPS
				{
					public static LocString PLANTLIFECYCLE = "Duration and number of harvests produced by this plant in a lifetime";

					public static LocString PLANTREQUIREMENTS = "Minimum conditions for basic plant growth";

					public static LocString PLANTEFFECTS = "Additional attributes of this plant";

					public static LocString YIELD = UI.FormatAsLink("{2}", "KCAL") + " produced [" + UI.FormatAsLink("{1}", "KCAL") + " / unit]";

					public static LocString YIELD_NONFOOD = "{0} produced per harvest";

					public static LocString NUMBEROFHARVESTS = "This plant can mature {0} times before the end of its life cycle";

					public static LocString YIELD_SEED = "Sow to grow more of this plant";

					public static LocString YIELD_SEED_FINAL_HARVEST = "{0}\n\nProduced in the final harvest of the plant's life cycle";

					public static LocString BONUS_SEEDS = "This plant has a {0} chance to produce new seeds when harvested";

					public static LocString DISEASE_DROPPER_BURST = "At certain points in this plant's lifecycle, it will emit a burst of {DiseaseAmount} {Disease}.";

					public static LocString DISEASE_DROPPER_CONSTANT = "This plant emits {DiseaseAmount} {Disease} while it is alive.";

					public static LocString DISEASE_ON_HARVEST = "The {Crop} produced by this plant will have {DiseaseAmount} {Disease} on it.";

					public static LocString AUTO_SELF_HARVEST = "This plant will instantly drop its crop and begin regrowing when it is matured.";
				}
			}

			public class EGGINCUBATOR
			{
				public static LocString TITLE = "Critter Eggs";

				public static LocString AWAITINGREQUEST = "INCUBATE: {0}";

				public static LocString AWAITINGDELIVERY = "AWAITING DELIVERY: {0}";

				public static LocString AWAITINGREMOVAL = "AWAITING REMOVAL: {0}";

				public static LocString ENTITYDEPOSITED = "INCUBATING: {0}";

				public static LocString DEPOSIT = "Incubate";

				public static LocString CANCELDEPOSIT = "Cancel";

				public static LocString REMOVE = "Remove";

				public static LocString CANCELREMOVAL = "Cancel";

				public static LocString SELECT_TITLE = "SELECT";

				public static LocString SELECT_DESC = "Select an egg to incubate.";
			}

			public class BASICRECEPTACLE
			{
				public static LocString TITLE = "Displayed Object";

				public static LocString AWAITINGREQUEST = "SELECT: {0}";

				public static LocString AWAITINGDELIVERY = "AWAITING DELIVERY: {0}";

				public static LocString AWAITINGREMOVAL = "AWAITING REMOVAL: {0}";

				public static LocString ENTITYDEPOSITED = "DISPLAYING: {0}";

				public static LocString DEPOSIT = "Select";

				public static LocString CANCELDEPOSIT = "Cancel";

				public static LocString REMOVE = "Remove";

				public static LocString CANCELREMOVAL = "Cancel";

				public static LocString SELECT_TITLE = "SELECT OBJECT";

				public static LocString SELECT_DESC = "Select an object to display here.";
			}

			public class LURE
			{
				public static LocString TITLE = "Select Bait";

				public static LocString INFORMATION = "INFORMATION";

				public static LocString AWAITINGREQUEST = "PLANT: {0}";

				public static LocString AWAITINGDELIVERY = "AWAITING DELIVERY: {0}";

				public static LocString AWAITINGREMOVAL = "AWAITING DIGGING UP: {0}";

				public static LocString ENTITYDEPOSITED = "PLANTED: {0}";

				public static LocString ATTRACTS = "Attract {1}s";
			}

			public class ROLESTATION
			{
				public static LocString TITLE = "Duplicant Skills";

				public static LocString OPENROLESBUTTON = "SKILLS";
			}

			public class RESEARCHSIDESCREEN
			{
				public static LocString TITLE = "Select Research";

				public static LocString CURRENTLYRESEARCHING = "Currently Researching";

				public static LocString NOSELECTEDRESEARCH = "No Research selected";

				public static LocString OPENRESEARCHBUTTON = "RESEARCH";
			}

			public class REFINERYSIDESCREEN
			{
				public static LocString RECIPE_FROM_TO = "{0} to {1}";

				public static LocString RECIPE_WITH = "{1} ({0})";

				public static LocString RECIPE_FROM_TO_WITH_NEWLINES = "{0}\nto\n{1}";

				public static LocString RECIPE_FROM_TO_COMPOSITE = "{0} to {1} and {2}";

				public static LocString RECIPE_FROM_TO_HEP = "{0} to " + UI.FormatAsLink("Radbolts", "RADIATION") + " and {1}";

				public static LocString RECIPE_SIMPLE_INCLUDE_AMOUNTS = "{0} {1}";

				public static LocString RECIPE_FROM_TO_INCLUDE_AMOUNTS = "{2} {0} to {3} {1}";

				public static LocString RECIPE_WITH_INCLUDE_AMOUNTS = "{3} {1} ({2} {0})";

				public static LocString RECIPE_FROM_TO_COMPOSITE_INCLUDE_AMOUNTS = "{3} {0} to {4} {1} and {5} {2}";

				public static LocString RECIPE_FROM_TO_HEP_INCLUDE_AMOUNTS = "{2} {0} to {3} " + UI.FormatAsLink("Radbolts", "RADIATION") + " and {4} {1}";
			}

			public class SEALEDDOORSIDESCREEN
			{
				public static LocString TITLE = "Sealed Door";

				public static LocString LABEL = "This door requires a sample to unlock.";

				public static LocString BUTTON = "SUBMIT BIOSCAN";

				public static LocString AWAITINGBUTTON = "AWAITING BIOSCAN";
			}

			public class ENCRYPTEDLORESIDESCREEN
			{
				public static LocString TITLE = "Encrypted File";

				public static LocString LABEL = "This computer contains encrypted files.";

				public static LocString BUTTON = "ATTEMPT DECRYPTION";

				public static LocString AWAITINGBUTTON = "AWAITING DECRYPTION";
			}

			public class ACCESS_CONTROL_SIDE_SCREEN
			{
				public static LocString TITLE = "Door Access Control";

				public static LocString DOOR_DEFAULT = "Default";

				public static LocString MINION_ACCESS = "Duplicant Access Permissions";

				public static LocString GO_LEFT_ENABLED = "Passing Left through this door is permitted\n\n" + UI.CLICK(UI.ClickType.Click) + " to revoke permission";

				public static LocString GO_LEFT_DISABLED = "Passing Left through this door is not permitted\n\n" + UI.CLICK(UI.ClickType.Click) + " to grant permission";

				public static LocString GO_RIGHT_ENABLED = "Passing Right through this door is permitted\n\n" + UI.CLICK(UI.ClickType.Click) + " to revoke permission";

				public static LocString GO_RIGHT_DISABLED = "Passing Right through this door is not permitted\n\n" + UI.CLICK(UI.ClickType.Click) + " to grant permission";

				public static LocString GO_UP_ENABLED = "Passing Up through this door is permitted\n\n" + UI.CLICK(UI.ClickType.Click) + " to revoke permission";

				public static LocString GO_UP_DISABLED = "Passing Up through this door is not permitted\n\n" + UI.CLICK(UI.ClickType.Click) + " to grant permission";

				public static LocString GO_DOWN_ENABLED = "Passing Down through this door is permitted\n\n" + UI.CLICK(UI.ClickType.Click) + " to revoke permission";

				public static LocString GO_DOWN_DISABLED = "Passing Down through this door is not permitted\n\n" + UI.CLICK(UI.ClickType.Click) + " to grant permission";

				public static LocString SET_TO_DEFAULT = UI.CLICK(UI.ClickType.Click) + " to clear custom permissions";

				public static LocString SET_TO_CUSTOM = UI.CLICK(UI.ClickType.Click) + " to assign custom permissions";

				public static LocString USING_DEFAULT = "Default Access";

				public static LocString USING_CUSTOM = "Custom Access";
			}

			public class ASSIGNABLESIDESCREEN
			{
				public static LocString TITLE = "Assign {0}";

				public static LocString ASSIGNED = "Assigned";

				public static LocString UNASSIGNED = "-";

				public static LocString DISABLED = "Ineligible";

				public static LocString SORT_BY_DUPLICANT = "Duplicant";

				public static LocString SORT_BY_ASSIGNMENT = "Assignment";

				public static LocString ASSIGN_TO_TOOLTIP = "Assign to {0}";

				public static LocString UNASSIGN_TOOLTIP = "Assigned to {0}";

				public static LocString DISABLED_TOOLTIP = "{0} is ineligible for this skill assignment";

				public static LocString PUBLIC = "Public";
			}

			public class COMETDETECTORSIDESCREEN
			{
				public static LocString TITLE = "Space Scanner";

				public static LocString HEADER = "Sends automation signal when selected object is detected";

				public static LocString ASSIGNED = "Assigned";

				public static LocString UNASSIGNED = "-";

				public static LocString DISABLED = "Ineligible";

				public static LocString SORT_BY_DUPLICANT = "Duplicant";

				public static LocString SORT_BY_ASSIGNMENT = "Assignment";

				public static LocString ASSIGN_TO_TOOLTIP = "Scanning for {0}";

				public static LocString UNASSIGN_TOOLTIP = "Scanning for {0}";

				public static LocString NOTHING = "Nothing";

				public static LocString COMETS = "Meteor Showers";

				public static LocString ROCKETS = "Rocket Landing Ping";

				public static LocString DUPEMADE = "Interplanetary Payloads";
			}

			public class GEOTUNERSIDESCREEN
			{
				public static LocString TITLE = "Select Geyser";

				public static LocString DESCRIPTION = "Select an analyzed geyser to transmit amplification data to.";

				public static LocString NOTHING = "No geyser selected";

				public static LocString UNSTUDIED_TOOLTIP = "This geyser must be analyzed before it can be selected\n\nDouble-click to view this geyser";

				public static LocString STUDIED_TOOLTIP = string.Concat(new string[]
				{
					"Increase this geyser's ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" and output"
				});

				public static LocString GEOTUNER_LIMIT_TOOLTIP = "This geyser cannot be targeted by more " + UI.PRE_KEYWORD + "Geotuners" + UI.PST_KEYWORD;

				public static LocString STUDIED_TOOLTIP_MATERIAL = "Required resource: {MATERIAL}";

				public static LocString STUDIED_TOOLTIP_POTENTIAL_OUTPUT = "Potential Output {POTENTIAL_OUTPUT}";

				public static LocString STUDIED_TOOLTIP_BASE_TEMP = "Base {BASE}";

				public static LocString STUDIED_TOOLTIP_VISIT_GEYSER = "Double-click to view this geyser";

				public static LocString STUDIED_TOOLTIP_GEOTUNER_MODIFIER_ROW_TITLE = "Geotuned ";

				public static LocString STUDIED_TOOLTIP_NUMBER_HOVERED = "This geyser is targeted by {0} Geotuners";
			}

			public class COMMAND_MODULE_SIDE_SCREEN
			{
				public static LocString TITLE = "Launch Conditions";

				public static LocString DESTINATION_BUTTON = "Show Starmap";

				public static LocString DESTINATION_BUTTON_EXPANSION = "Show Starmap";
			}

			public class CLUSTERDESTINATIONSIDESCREEN
			{
				public static LocString TITLE = "Destination";

				public static LocString FIRSTAVAILABLE = "Any " + BUILDINGS.PREFABS.LAUNCHPAD.NAME;

				public static LocString NONEAVAILABLE = "No landing site";

				public static LocString NO_TALL_SITES_AVAILABLE = "No landing sites fit the height of this rocket";

				public static LocString DROPDOWN_TOOLTIP_VALID_SITE = "Land at {0} when the site is clear";

				public static LocString DROPDOWN_TOOLTIP_FIRST_AVAILABLE = "Select the first available landing site";

				public static LocString DROPDOWN_TOOLTIP_TOO_SHORT = "This rocket's height exceeds the space available in this landing site";

				public static LocString DROPDOWN_TOOLTIP_PATH_OBSTRUCTED = "Landing path obstructed";

				public static LocString DROPDOWN_TOOLTIP_SITE_OBSTRUCTED = "Landing position on the platform is obstructed";

				public static LocString DROPDOWN_TOOLTIP_PAD_DISABLED = BUILDINGS.PREFABS.LAUNCHPAD.NAME + " is disabled";

				public static LocString CHANGE_DESTINATION_BUTTON = "Change";

				public static LocString CHANGE_DESTINATION_BUTTON_TOOLTIP = "Select a new destination for this rocket";

				public static LocString CLEAR_DESTINATION_BUTTON = "Clear";

				public static LocString CLEAR_DESTINATION_BUTTON_TOOLTIP = "Clear this rocket's selected destination";

				public static LocString LOOP_BUTTON_TOOLTIP = "Toggle a roundtrip flight between this rocket's destination and its original takeoff location";

				public class ASSIGNMENTSTATUS
				{
					public static LocString LOCAL = "Current";

					public static LocString DESTINATION = "Destination";
				}
			}

			public class EQUIPPABLESIDESCREEN
			{
				public static LocString TITLE = "Equip {0}";

				public static LocString ASSIGNEDTO = "Assigned to: {Assignee}";

				public static LocString UNASSIGNED = "Unassigned";

				public static LocString GENERAL_CURRENTASSIGNED = "(Owner)";
			}

			public class EQUIPPABLE_SIDE_SCREEN
			{
				public static LocString TITLE = "Assign To Duplicant";

				public static LocString CURRENTLY_EQUIPPED = "Currently Equipped:\n{0}";

				public static LocString NONE_EQUIPPED = "None";

				public static LocString EQUIP_BUTTON = "Equip";

				public static LocString DROP_BUTTON = "Drop";

				public static LocString SWAP_BUTTON = "Swap";
			}

			public class TELEPADSIDESCREEN
			{
				public static LocString TITLE = "Printables";

				public static LocString NEXTPRODUCTION = "Next Production: {0}";

				public static LocString GAMEOVER = "Colony Lost";

				public static LocString VICTORY_CONDITIONS = "Hardwired Imperatives";

				public static LocString SUMMARY_TITLE = "Colony Summary";

				public static LocString SKILLS_BUTTON = "Duplicant Skills";
			}

			public class VALVESIDESCREEN
			{
				public static LocString TITLE = "Flow Control";
			}

			public class LIMIT_VALVE_SIDE_SCREEN
			{
				public static LocString TITLE = "Meter Control";

				public static LocString AMOUNT = "Amount: {0}";

				public static LocString LIMIT = "Limit:";

				public static LocString RESET_BUTTON = "Reset Amount";

				public static LocString SLIDER_TOOLTIP_UNITS = "The amount of Units or Mass passing through the sensor.";
			}

			public class NUCLEAR_REACTOR_SIDE_SCREEN
			{
				public static LocString TITLE = "Reaction Mass Target";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Duplicants will attempt to keep the reactor supplied with ",
					UI.PRE_KEYWORD,
					"{0}{1}",
					UI.PST_KEYWORD,
					" of ",
					UI.PRE_KEYWORD,
					"{2}",
					UI.PST_KEYWORD
				});
			}

			public class MANUALGENERATORSIDESCREEN
			{
				public static LocString TITLE = "Battery Recharge Threshold";

				public static LocString CURRENT_THRESHOLD = "Current Threshold: {0}%";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Duplicants will be requested to operate this generator when the total charge of the connected ",
					UI.PRE_KEYWORD,
					"Batteries",
					UI.PST_KEYWORD,
					" falls below <b>{0}%</b>"
				});
			}

			public class MANUALDELIVERYGENERATORSIDESCREEN
			{
				public static LocString TITLE = "Fuel Request Threshold";

				public static LocString CURRENT_THRESHOLD = "Current Threshold: {0}%";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Duplicants will be requested to deliver ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" when the total charge of the connected ",
					UI.PRE_KEYWORD,
					"Batteries",
					UI.PST_KEYWORD,
					" falls below <b>{1}%</b>"
				});
			}

			public class TIME_OF_DAY_SIDE_SCREEN
			{
				public static LocString TITLE = "Time-of-Day Sensor";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" after the selected Turn On time, and a ",
					UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby),
					" after the selected Turn Off time"
				});

				public static LocString START = "Turn On";

				public static LocString STOP = "Turn Off";
			}

			public class CRITTER_COUNT_SIDE_SCREEN
			{
				public static LocString TITLE = "Critter Count Sensor";

				public static LocString TOOLTIP_ABOVE = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if there are more than <b>{0}</b> ",
					UI.PRE_KEYWORD,
					"Critters",
					UI.PST_KEYWORD,
					" or ",
					UI.PRE_KEYWORD,
					"Eggs",
					UI.PST_KEYWORD,
					" in the room"
				});

				public static LocString TOOLTIP_BELOW = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if there are fewer than <b>{0}</b> ",
					UI.PRE_KEYWORD,
					"Critters",
					UI.PST_KEYWORD,
					" or ",
					UI.PRE_KEYWORD,
					"Eggs",
					UI.PST_KEYWORD,
					" in the room"
				});

				public static LocString START = "Turn On";

				public static LocString STOP = "Turn Off";

				public static LocString VALUE_NAME = "Count";
			}

			public class OIL_WELL_CAP_SIDE_SCREEN
			{
				public static LocString TITLE = "Backpressure Release Threshold";

				public static LocString TOOLTIP = "Duplicants will be requested to release backpressure buildup when it exceeds <b>{0}%</b>";
			}

			public class MODULAR_CONDUIT_PORT_SIDE_SCREEN
			{
				public static LocString TITLE = "Pump Control";

				public static LocString LABEL_UNLOAD = "Unload Only";

				public static LocString LABEL_BOTH = "Load/Unload";

				public static LocString LABEL_LOAD = "Load Only";

				public static readonly List<LocString> LABELS = new List<LocString>
				{
					UI.UISIDESCREENS.MODULAR_CONDUIT_PORT_SIDE_SCREEN.LABEL_UNLOAD,
					UI.UISIDESCREENS.MODULAR_CONDUIT_PORT_SIDE_SCREEN.LABEL_BOTH,
					UI.UISIDESCREENS.MODULAR_CONDUIT_PORT_SIDE_SCREEN.LABEL_LOAD
				};

				public static LocString TOOLTIP_UNLOAD = "This pump will attempt to <b>Unload</b> cargo from the landed rocket, but not attempt to load new cargo";

				public static LocString TOOLTIP_BOTH = "This pump will both <b>Load</b> and <b>Unload</b> cargo from the landed rocket";

				public static LocString TOOLTIP_LOAD = "This pump will attempt to <b>Load</b> cargo onto the landed rocket, but will not unload it";

				public static readonly List<LocString> TOOLTIPS = new List<LocString>
				{
					UI.UISIDESCREENS.MODULAR_CONDUIT_PORT_SIDE_SCREEN.TOOLTIP_UNLOAD,
					UI.UISIDESCREENS.MODULAR_CONDUIT_PORT_SIDE_SCREEN.TOOLTIP_BOTH,
					UI.UISIDESCREENS.MODULAR_CONDUIT_PORT_SIDE_SCREEN.TOOLTIP_LOAD
				};

				public static LocString DESCRIPTION = "";
			}

			public class LOGIC_BUFFER_SIDE_SCREEN
			{
				public static LocString TITLE = "Buffer Time";

				public static LocString TOOLTIP = "Will continue to send a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " for <b>{0} seconds</b> after receiving a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGIC_FILTER_SIDE_SCREEN
			{
				public static LocString TITLE = "Filter Time";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Will only send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if it receives ",
					UI.FormatAsAutomationState("Green", UI.AutomationState.Active),
					" for longer than <b>{0} seconds</b>"
				});
			}

			public class TIME_RANGE_SIDE_SCREEN
			{
				public static LocString TITLE = "Time Schedule";

				public static LocString ON = "Activation Time";

				public static LocString ON_TOOLTIP = string.Concat(new string[]
				{
					"Activation time determines the time of day this sensor should begin sending a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					"\n\nThis sensor sends a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" {0} through the day"
				});

				public static LocString DURATION = "Active Duration";

				public static LocString DURATION_TOOLTIP = string.Concat(new string[]
				{
					"Active duration determines what percentage of the day this sensor will spend sending a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					"\n\nThis sensor will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" for {0} of the day"
				});
			}

			public class TIMER_SIDE_SCREEN
			{
				public static LocString TITLE = "Timer";

				public static LocString ON = "Green Duration";

				public static LocString GREEN_DURATION_TOOLTIP = string.Concat(new string[]
				{
					"Green duration determines the amount of time this sensor should send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					"\n\nThis sensor sends a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" for {0}"
				});

				public static LocString OFF = "Red Duration";

				public static LocString RED_DURATION_TOOLTIP = string.Concat(new string[]
				{
					"Red duration determines the amount of time this sensor should send a ",
					UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby),
					"\n\nThis sensor will send a ",
					UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby),
					" for {0}"
				});

				public static LocString CURRENT_TIME = "{0}/{1}";

				public static LocString MODE_LABEL_SECONDS = "Mode: Seconds";

				public static LocString MODE_LABEL_CYCLES = "Mode: Cycles";

				public static LocString RESET_BUTTON = "Reset Timer";

				public static LocString DISABLED = "Timer Disabled";
			}

			public class COUNTER_SIDE_SCREEN
			{
				public static LocString TITLE = "Counter";

				public static LocString RESET_BUTTON = "Reset Counter";

				public static LocString DESCRIPTION = "Send " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when count is reached:";

				public static LocString INCREMENT_MODE = "Mode: Increment";

				public static LocString DECREMENT_MODE = "Mode: Decrement";

				public static LocString ADVANCED_MODE = "Advanced Mode";

				public static LocString CURRENT_COUNT_SIMPLE = "{0} of ";

				public static LocString CURRENT_COUNT_ADVANCED = "{0} % ";

				public class TOOLTIPS
				{
					public static LocString ADVANCED_MODE = string.Concat(new string[]
					{
						"In Advanced Mode, the ",
						BUILDINGS.PREFABS.LOGICCOUNTER.NAME,
						" will count from <b>0</b> rather than <b>1</b>. It will reset when the max is reached, and send a ",
						UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
						" as a brief pulse rather than continuously."
					});
				}
			}

			public class PASSENGERMODULESIDESCREEN
			{
				public static LocString REQUEST_CREW = "Crew";

				public static LocString REQUEST_CREW_TOOLTIP = "Crew may not leave the module, non crew-must exit";

				public static LocString AUTO_CREW = "Auto";

				public static LocString AUTO_CREW_TOOLTIP = "All Duplicants may enter and exit the module freely until the rocket is ready for launch\n\nBefore launch the crew will automatically be requested";

				public static LocString RELEASE_CREW = "All";

				public static LocString RELEASE_CREW_TOOLTIP = "All Duplicants may enter and exit the module freely";

				public static LocString REQUIRE_SUIT_LABEL = "Atmosuit Required";

				public static LocString REQUIRE_SUIT_LABEL_TOOLTIP = "If checked, Duplicants will be required to wear an Atmo Suit when entering this rocket";

				public static LocString CHANGE_CREW_BUTTON = "Change crew";

				public static LocString CHANGE_CREW_BUTTON_TOOLTIP = "Assign Duplicants to crew this rocket's missions";

				public static LocString ASSIGNED_TO_CREW = "Assigned to crew";

				public static LocString UNASSIGNED = "Unassigned";
			}

			public class TIMEDSWITCHSIDESCREEN
			{
				public static LocString TITLE = "Time Schedule";

				public static LocString ONTIME = "On Time:";

				public static LocString OFFTIME = "Off Time:";

				public static LocString TIMETODEACTIVATE = "Time until deactivation: {0}";

				public static LocString TIMETOACTIVATE = "Time until activation: {0}";

				public static LocString WARNING = "Switch must be connected to a " + UI.FormatAsLink("Power", "POWER") + " grid";

				public static LocString CURRENTSTATE = "Current State:";

				public static LocString ON = "On";

				public static LocString OFF = "Off";
			}

			public class CAPTURE_POINT_SIDE_SCREEN
			{
				public static LocString TITLE = "Stable Management";

				public static LocString AUTOWRANGLE = "Auto-Wrangle Surplus";

				public static LocString AUTOWRANGLE_TOOLTIP = "A Duplicant will automatically wrangle any critters that exceed the population limit or that do not belong in this stable\n\nDuplicants must possess the Critter Ranching Skill in order to wrangle critters";

				public static LocString LIMIT_TOOLTIP = "Critters exceeding this population limit will automatically be wrangled:";

				public static LocString UNITS_SUFFIX = " Critters";
			}

			public class TEMPERATURESWITCHSIDESCREEN
			{
				public static LocString TITLE = "Temperature Threshold";

				public static LocString CURRENT_TEMPERATURE = "Current Temperature:\n{0}";

				public static LocString ACTIVATE_IF = "Send " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if:";

				public static LocString COLDER_BUTTON = "Below";

				public static LocString WARMER_BUTTON = "Above";
			}

			public class RADIATIONSWITCHSIDESCREEN
			{
				public static LocString TITLE = "Radiation Threshold";

				public static LocString CURRENT_TEMPERATURE = "Current Radiation:\n{0}/cycle";

				public static LocString ACTIVATE_IF = "Send " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if:";

				public static LocString COLDER_BUTTON = "Below";

				public static LocString WARMER_BUTTON = "Above";
			}

			public class WATTAGESWITCHSIDESCREEN
			{
				public static LocString TITLE = "Wattage Threshold";

				public static LocString CURRENT_TEMPERATURE = "Current Wattage:\n{0}";

				public static LocString ACTIVATE_IF = "Send " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if:";

				public static LocString COLDER_BUTTON = "Below";

				public static LocString WARMER_BUTTON = "Above";
			}

			public class HEPSWITCHSIDESCREEN
			{
				public static LocString TITLE = "Radbolt Threshold";
			}

			public class THRESHOLD_SWITCH_SIDESCREEN
			{
				public static LocString TITLE = "Pressure";

				public static LocString THRESHOLD_SUBTITLE = "Threshold:";

				public static LocString CURRENT_VALUE = "Current {0}:\n{1}";

				public static LocString ACTIVATE_IF = "Send " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if:";

				public static LocString ABOVE_BUTTON = "Above";

				public static LocString BELOW_BUTTON = "Below";

				public static LocString STATUS_ACTIVE = "Switch Active";

				public static LocString STATUS_INACTIVE = "Switch Inactive";

				public static LocString PRESSURE = "Ambient Pressure";

				public static LocString PRESSURE_TOOLTIP_ABOVE = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if the ",
					UI.PRE_KEYWORD,
					"Pressure",
					UI.PST_KEYWORD,
					" is above <b>{0}</b>"
				});

				public static LocString PRESSURE_TOOLTIP_BELOW = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if the ",
					UI.PRE_KEYWORD,
					"Pressure",
					UI.PST_KEYWORD,
					" is below <b>{0}</b>"
				});

				public static LocString TEMPERATURE = "Ambient Temperature";

				public static LocString TEMPERATURE_TOOLTIP_ABOVE = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if the ambient ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" is above <b>{0}</b>"
				});

				public static LocString TEMPERATURE_TOOLTIP_BELOW = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if the ambient ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" is below <b>{0}</b>"
				});

				public static LocString WATTAGE = "Wattage Reading";

				public static LocString WATTAGE_TOOLTIP_ABOVE = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if the ",
					UI.PRE_KEYWORD,
					"Wattage",
					UI.PST_KEYWORD,
					" consumed is above <b>{0}</b>"
				});

				public static LocString WATTAGE_TOOLTIP_BELOW = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if the ",
					UI.PRE_KEYWORD,
					"Wattage",
					UI.PST_KEYWORD,
					" consumed is below <b>{0}</b>"
				});

				public static LocString DISEASE_TITLE = "Germ Threshold";

				public static LocString DISEASE = "Ambient Germs";

				public static LocString DISEASE_TOOLTIP_ABOVE = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if the number of ",
					UI.PRE_KEYWORD,
					"Germs",
					UI.PST_KEYWORD,
					" is above <b>{0}</b>"
				});

				public static LocString DISEASE_TOOLTIP_BELOW = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if the number of ",
					UI.PRE_KEYWORD,
					"Germs",
					UI.PST_KEYWORD,
					" is below <b>{0}</b>"
				});

				public static LocString DISEASE_UNITS = "";

				public static LocString RADIATION = "Ambient Radiation";

				public static LocString RADIATION_TOOLTIP_ABOVE = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if the ambient ",
					UI.PRE_KEYWORD,
					"Radiation",
					UI.PST_KEYWORD,
					" is above <b>{0}</b>"
				});

				public static LocString RADIATION_TOOLTIP_BELOW = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if the ambient ",
					UI.PRE_KEYWORD,
					"Radiation",
					UI.PST_KEYWORD,
					" is below <b>{0}</b>"
				});

				public static LocString HEPS = "Radbolt Reading";

				public static LocString HEPS_TOOLTIP_ABOVE = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if the ",
					UI.PRE_KEYWORD,
					"Radbolts",
					UI.PST_KEYWORD,
					" is above <b>{0}</b>"
				});

				public static LocString HEPS_TOOLTIP_BELOW = string.Concat(new string[]
				{
					"Will send a ",
					UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
					" if the ",
					UI.PRE_KEYWORD,
					"Radbolts",
					UI.PST_KEYWORD,
					" is below <b>{0}</b>"
				});
			}

			public class CAPACITY_CONTROL_SIDE_SCREEN
			{
				public static LocString TITLE = "Automated Storage Capacity";

				public static LocString MAX_LABEL = "Max:";
			}

			public class DOOR_TOGGLE_SIDE_SCREEN
			{
				public static LocString TITLE = "Door Setting";

				public static LocString OPEN = "Door is open.";

				public static LocString AUTO = "Door is on auto.";

				public static LocString CLOSE = "Door is locked.";

				public static LocString PENDING_FORMAT = "{0} {1}";

				public static LocString OPEN_PENDING = "Awaiting Duplicant to open door.";

				public static LocString AUTO_PENDING = "Awaiting Duplicant to automate door.";

				public static LocString CLOSE_PENDING = "Awaiting Duplicant to lock door.";

				public static LocString ACCESS_FORMAT = "{0}\n\n{1}";

				public static LocString ACCESS_OFFLINE = "Emergency Access Permissions:\nAll Duplicants are permitted to use this door until " + UI.FormatAsLink("Power", "POWER") + " is restored.";

				public static LocString POI_INTERNAL = "This door cannot be manually controlled.";
			}

			public class ACTIVATION_RANGE_SIDE_SCREEN
			{
				public static LocString NAME = "Breaktime Policy";

				public static LocString ACTIVATE = "Break starts at:";

				public static LocString DEACTIVATE = "Break ends at:";
			}

			public class CAPACITY_SIDE_SCREEN
			{
				public static LocString TOOLTIP = "Adjust the maximum amount that can be stored here";
			}

			public class SUIT_SIDE_SCREEN
			{
				public static LocString TITLE = "Dock Inventory";

				public static LocString CONFIGURATION_REQUIRED = "Configuration Required:";

				public static LocString CONFIG_REQUEST_SUIT = "Deliver Suit";

				public static LocString CONFIG_REQUEST_SUIT_TOOLTIP = "Duplicants will immediately deliver and dock the nearest unequipped suit";

				public static LocString CONFIG_NO_SUIT = "Leave Empty";

				public static LocString CONFIG_NO_SUIT_TOOLTIP = "The next suited Duplicant to pass by will unequip their suit and dock it here";

				public static LocString CONFIG_CANCEL_REQUEST = "Cancel Request";

				public static LocString CONFIG_CANCEL_REQUEST_TOOLTIP = "Cancel this suit delivery";

				public static LocString CONFIG_DROP_SUIT = "Undock Suit";

				public static LocString CONFIG_DROP_SUIT_TOOLTIP = "Disconnect this suit, dropping it on the ground";

				public static LocString CONFIG_DROP_SUIT_NO_SUIT_TOOLTIP = "There is no suit in this building to undock";
			}

			public class AUTOMATABLE_SIDE_SCREEN
			{
				public static LocString TITLE = "Automatable Storage";

				public static LocString ALLOWMANUALBUTTON = "Allow Manual Use";

				public static LocString ALLOWMANUALBUTTONTOOLTIP = "Allow Duplicants to manually manage these storage materials";
			}

			public class STUDYABLE_SIDE_SCREEN
			{
				public static LocString TITLE = "Analyze Natural Feature";

				public static LocString STUDIED_STATUS = "Researchers have completed their analysis and compiled their findings.";

				public static LocString STUDIED_BUTTON = "ANALYSIS COMPLETE";

				public static LocString SEND_STATUS = "Send a researcher to gather data here.\n\nAnalyzing a feature takes time, but yields useful results.";

				public static LocString SEND_BUTTON = "ANALYZE";

				public static LocString PENDING_STATUS = "A researcher is in the process of studying this feature.";

				public static LocString PENDING_BUTTON = "CANCEL ANALYSIS";
			}

			public class MEDICALCOTSIDESCREEN
			{
				public static LocString TITLE = "Severity Requirement";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"A Duplicant may not use this cot until their ",
					UI.PRE_KEYWORD,
					"Health",
					UI.PST_KEYWORD,
					" falls below <b>{0}%</b>"
				});
			}

			public class WARPPORTALSIDESCREEN
			{
				public static LocString TITLE = "Teleporter";

				public static LocString IDLE = "Teleporter online.\nPlease select a passenger:";

				public static LocString WAITING = "Ready to transmit passenger.";

				public static LocString COMPLETE = "Passenger transmitted!";

				public static LocString UNDERWAY = "Transmitting passenger...";

				public static LocString CONSUMED = "Teleporter recharging:";

				public static LocString BUTTON = "Teleport!";

				public static LocString CANCELBUTTON = "Cancel";
			}

			public class RADBOLTTHRESHOLDSIDESCREEN
			{
				public static LocString TITLE = "Radbolt Threshold";

				public static LocString CURRENT_THRESHOLD = "Current Threshold: {0}%";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Releases a ",
					UI.PRE_KEYWORD,
					"Radbolt",
					UI.PST_KEYWORD,
					" when stored Radbolts exceed <b>{0}</b>"
				});

				public static LocString PROGRESS_BAR_LABEL = "Radbolt Generation";

				public static LocString PROGRESS_BAR_TOOLTIP = string.Concat(new string[]
				{
					"The building will emit a ",
					UI.PRE_KEYWORD,
					"Radbolt",
					UI.PST_KEYWORD,
					" in the chosen direction when fully charged"
				});
			}

			public class LOGICBITSELECTORSIDESCREEN
			{
				public static LocString RIBBON_READER_TITLE = "Ribbon Reader";

				public static LocString RIBBON_READER_DESCRIPTION = "Selected <b>Bit's Signal</b> will be read by the <b>Output Port</b>";

				public static LocString RIBBON_WRITER_TITLE = "Ribbon Writer";

				public static LocString RIBBON_WRITER_DESCRIPTION = "Received <b>Signal</b> will be written to selected <b>Bit</b>";

				public static LocString BIT = "Bit {0}";

				public static LocString STATE_ACTIVE = "Green";

				public static LocString STATE_INACTIVE = "Red";
			}

			public class LOGICALARMSIDESCREEN
			{
				public static LocString TITLE = "Notification Designer";

				public static LocString DESCRIPTION = "Notification will be sent upon receiving a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + "\n\nMaking modifications will clear any existing notifications being sent by this building.";

				public static LocString NAME = "<b>Name:</b>";

				public static LocString NAME_DEFAULT = "Notification";

				public static LocString TOOLTIP = "<b>Tooltip:</b>";

				public static LocString TOOLTIP_DEFAULT = "Tooltip";

				public static LocString TYPE = "<b>Type:</b>";

				public static LocString PAUSE = "<b>Pause:</b>";

				public static LocString ZOOM = "<b>Zoom:</b>";

				public class TOOLTIPS
				{
					public static LocString NAME = "Select notification text";

					public static LocString TOOLTIP = "Select notification hover text";

					public static LocString TYPE = "Select the visual and aural style of the notification";

					public static LocString PAUSE = "Time will pause upon notification when checked";

					public static LocString ZOOM = "The view will zoom to this building upon notification when checked";

					public static LocString BAD = "\"Boing boing!\"";

					public static LocString NEUTRAL = "\"Pop!\"";

					public static LocString DUPLICANT_THREATENING = "AHH!";
				}
			}

			public class GENETICANALYSISSIDESCREEN
			{
				public static LocString TITLE = "Genetic Analysis";

				public static LocString NONE_DISCOVERED = "No mutant seeds have been found.";

				public static LocString SELECT_SEEDS = "Select which seed types to analyze:";

				public static LocString SEED_NO_MUTANTS = "</i>No mutants found</i>";

				public static LocString SEED_FORBIDDEN = "</i>Won't analyze</i>";

				public static LocString SEED_ALLOWED = "</i>Will analyze</i>";
			}
		}

		public class USERMENUACTIONS
		{
			public class CLEANTOILET
			{
				public static LocString NAME = "Clean Toilet";

				public static LocString TOOLTIP = "Empty waste from this toilet";
			}

			public class CANCELCLEANTOILET
			{
				public static LocString NAME = "Cancel Clean";

				public static LocString TOOLTIP = "Cancel this cleaning order";
			}

			public class EMPTYBEEHIVE
			{
				public static LocString NAME = "Enable Autoharvest";

				public static LocString TOOLTIP = "Automatically harvest this hive when full";
			}

			public class CANCELEMPTYBEEHIVE
			{
				public static LocString NAME = "Disable Autoharvest";

				public static LocString TOOLTIP = "Do not automatically harvest this hive";
			}

			public class EMPTYDESALINATOR
			{
				public static LocString NAME = "Empty Desalinator";

				public static LocString TOOLTIP = "Empty salt from this desalinator";
			}

			public class CHANGE_ROOM
			{
				public static LocString REQUEST_OUTFIT = "Request Outfit";

				public static LocString REQUEST_OUTFIT_TOOLTIP = "Request outfit to be delivered to this change room";

				public static LocString CANCEL_REQUEST = "Cancel Request";

				public static LocString CANCEL_REQUEST_TOOLTIP = "Cancel outfit request";

				public static LocString DROP_OUTFIT = "Drop Outfit";

				public static LocString DROP_OUTFIT_TOOLTIP = "Drop outfit on floor";
			}

			public class DUMP
			{
				public static LocString NAME = "Empty";

				public static LocString TOOLTIP = "Dump bottle contents onto the floor";

				public static LocString NAME_OFF = "Cancel Empty";

				public static LocString TOOLTIP_OFF = "Cancel this empty order";
			}

			public class TAGFILTER
			{
				public static LocString NAME = "Filter Settings";

				public static LocString TOOLTIP = "Assign materials to storage";
			}

			public class CANCELCONSTRUCTION
			{
				public static LocString NAME = "Cancel Build";

				public static LocString TOOLTIP = "Cancel this build order";
			}

			public class DIG
			{
				public static LocString NAME = "Dig";

				public static LocString TOOLTIP = "Dig out this cell";

				public static LocString TOOLTIP_OFF = "Cancel this dig order";
			}

			public class CANCELMOP
			{
				public static LocString NAME = "Cancel Mop";

				public static LocString TOOLTIP = "Cancel this mop order";
			}

			public class CANCELDIG
			{
				public static LocString NAME = "Cancel Dig";

				public static LocString TOOLTIP = "Cancel this dig order";
			}

			public class UPROOT
			{
				public static LocString NAME = "Uproot";

				public static LocString TOOLTIP = "Convert this plant into a seed";
			}

			public class CANCELUPROOT
			{
				public static LocString NAME = "Cancel Uproot";

				public static LocString TOOLTIP = "Cancel this uproot order";
			}

			public class HARVEST_WHEN_READY
			{
				public static LocString NAME = "Enable Autoharvest";

				public static LocString TOOLTIP = "Automatically harvest this plant when it matures";
			}

			public class CANCEL_HARVEST_WHEN_READY
			{
				public static LocString NAME = "Disable Autoharvest";

				public static LocString TOOLTIP = "Do not automatically harvest this plant";
			}

			public class HARVEST
			{
				public static LocString NAME = "Harvest";

				public static LocString TOOLTIP = "Harvest materials from this plant";

				public static LocString TOOLTIP_DISABLED = "This plant has nothing to harvest";
			}

			public class CANCELHARVEST
			{
				public static LocString NAME = "Cancel Harvest";

				public static LocString TOOLTIP = "Cancel this harvest order";
			}

			public class ATTACK
			{
				public static LocString NAME = "Attack";

				public static LocString TOOLTIP = "Attack this critter";
			}

			public class CANCELATTACK
			{
				public static LocString NAME = "Cancel Attack";

				public static LocString TOOLTIP = "Cancel this attack order";
			}

			public class CAPTURE
			{
				public static LocString NAME = "Wrangle";

				public static LocString TOOLTIP = "Capture this critter alive";
			}

			public class CANCELCAPTURE
			{
				public static LocString NAME = "Cancel Wrangle";

				public static LocString TOOLTIP = "Cancel this wrangle order";
			}

			public class RELEASEELEMENT
			{
				public static LocString NAME = "Empty Building";

				public static LocString TOOLTIP = "Refund all resources currently in use by this building";
			}

			public class DECONSTRUCT
			{
				public static LocString NAME = "Deconstruct";

				public static LocString TOOLTIP = "Deconstruct this building and refund all resources";

				public static LocString NAME_OFF = "Cancel Deconstruct";

				public static LocString TOOLTIP_OFF = "Cancel this deconstruct order";
			}

			public class DEMOLISH
			{
				public static LocString NAME = "Demolish";

				public static LocString TOOLTIP = "Demolish this building";

				public static LocString NAME_OFF = "Cancel Demolition";

				public static LocString TOOLTIP_OFF = "Cancel this demolition order";
			}

			public class ROCKETUSAGERESTRICTION
			{
				public static LocString NAME_UNCONTROLLED = "Uncontrolled";

				public static LocString TOOLTIP_UNCONTROLLED = "Do not allow this building to be controlled by a " + BUILDINGS.PREFABS.ROCKETCONTROLSTATION.NAME;

				public static LocString NAME_CONTROLLED = "Controlled";

				public static LocString TOOLTIP_CONTROLLED = "Allow this building's operation to be controlled by a " + BUILDINGS.PREFABS.ROCKETCONTROLSTATION.NAME;
			}

			public class MANUAL_DELIVERY
			{
				public static LocString NAME = "Disable Delivery";

				public static LocString TOOLTIP = "Do not deliver materials to this building";

				public static LocString NAME_OFF = "Enable Delivery";

				public static LocString TOOLTIP_OFF = "Deliver materials to this building";
			}

			public class SELECTRESEARCH
			{
				public static LocString NAME = "Select Research";

				public static LocString TOOLTIP = "Choose a technology from the " + UI.FormatAsManagementMenu("Research Tree", global::Action.ManageResearch);
			}

			public class RELOCATE
			{
				public static LocString NAME = "Relocate";

				public static LocString TOOLTIP = "Move this building to a new location\n\nCosts no additional resources";

				public static LocString NAME_OFF = "Cancel Relocation";

				public static LocString TOOLTIP_OFF = "Cancel this relocation order";
			}

			public class ENABLEBUILDING
			{
				public static LocString NAME = "Disable Building";

				public static LocString TOOLTIP = "Halt the use of this building {Hotkey}\n\nDisabled buildings consume no energy or resources";

				public static LocString NAME_OFF = "Enable Building";

				public static LocString TOOLTIP_OFF = "Resume the use of this building {Hotkey}";
			}

			public class READLORE
			{
				public static LocString NAME = "Inspect";

				public static LocString ALREADYINSPECTED = "Already inspected";

				public static LocString TOOLTIP = "Recover files from this structure";

				public static LocString TOOLTIP_ALREADYINSPECTED = "This structure has already been inspected";

				public static LocString GOTODATABASE = "View Entry";

				public static LocString SEARCH_DISPLAY = "The display is still functional. I copy its message into my database.\n\nNew Database Entry discovered.";

				public static LocString SEARCH_ELLIESDESK = "All I find on the machine is a curt e-mail from a disgruntled employee.\n\nNew Database Entry discovered.";

				public static LocString SEARCH_POD = "I search my incoming message history and find a single entry. I move the odd message into my database.\n\nNew Database Entry discovered.";

				public static LocString ALREADY_SEARCHED = "I already took everything of interest from this. I can check the Database to re-read what I found.";

				public static LocString SEARCH_CABINET = "One intact document remains - an old yellowing newspaper clipping. It won't be of much use, but I add it to my database nonetheless.\n\nNew Database Entry discovered.";

				public static LocString SEARCH_STERNSDESK = "There's an old magazine article from a publication called the \"Nucleoid\" tucked in the top drawer. I add it to my database.\n\nNew Database Entry discovered.";

				public static LocString ALREADY_SEARCHED_STERNSDESK = "The desk is eerily empty inside.";

				public static LocString SEARCH_TELEPORTER_SENDER = "While scanning the antiquated computer code of this machine I uncovered some research notes. I add them to my database.\n\nNew Database Entry discovered.";

				public static LocString SEARCH_TELEPORTER_RECEIVER = "Incongruously placed research notes are hidden within the operating instructions of this device. I add them to my database.\n\nNew Database Entry discovered.";

				public static LocString SEARCH_CRYO_TANK = "There are some safety instructions included in the operating instructions of this Cryotank. I add them to my database.\n\nNew Database Entry discovered.";

				public static LocString SEARCH_PROPGRAVITASCREATUREPOSTER = "There's a handwritten note taped to the back of this poster. I add it to my database.\n\nNew Database Entry discovered.";

				public class SEARCH_COMPUTER_PODIUM
				{
					public static LocString SEARCH1 = "I search through the computer's database and find an unredacted e-mail.\n\nNew Database Entry unlocked.";
				}

				public class SEARCH_COMPUTER_SUCCESS
				{
					public static LocString SEARCH1 = "After searching through the computer's database, I managed to piece together some files that piqued my interest.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH2 = "Searching through the computer, I find some recoverable files that are still readable.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH3 = "The computer looks pristine on the outside, but is corrupted internally. Still, I managed to find one uncorrupted file, and have added it to my database.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH4 = "The computer was wiped almost completely clean, except for one file hidden in the recycle bin.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH5 = "I search the computer, storing what useful data I can find in my own memory.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH6 = "This computer is broken and requires some finessing to get working. Still, I recover a handful of interesting files.\n\nNew Database Entry unlocked.";
				}

				public class SEARCH_COMPUTER_FAIL
				{
					public static LocString SEARCH1 = "Unfortunately, the computer's hard drive is irreparably corrupted.";

					public static LocString SEARCH2 = "The computer was wiped clean before I got here. There is nothing to recover.";

					public static LocString SEARCH3 = "Some intact files are available on the computer, but nothing I haven't already discovered elsewhere. I find nothing else.";

					public static LocString SEARCH4 = "The computer has nothing of import.";

					public static LocString SEARCH5 = "Someone's left a solitaire game up. There's nothing else of interest on the computer.\n\nAlso, it looks as though they were about to lose.";

					public static LocString SEARCH6 = "The background on this computer depicts two kittens hugging in a field of daisies. There is nothing else of import to be found.";

					public static LocString SEARCH7 = "The user alphabetized the shortcuts on their desktop. There is nothing else of import to be found.";

					public static LocString SEARCH8 = "The background is a picture of a golden retriever in a science lab. It looks very confused. There is nothing else of import to be found.";

					public static LocString SEARCH9 = "This user never changed their default background. There is nothing else of import to be found. How dull.";
				}

				public class SEARCH_TECHNOLOGY_SUCCESS
				{
					public static LocString SEARCH1 = "I scour the internal systems and find something of interest.\n\nNew Database Entry discovered.";

					public static LocString SEARCH2 = "I see if I can salvage anything from the electronics. I add what I find to my database.\n\nNew Database Entry discovered.";

					public static LocString SEARCH3 = "I look for anything of interest within the abandoned machinery and add what I find to my database.\n\nNew Database Entry discovered.";
				}

				public class SEARCH_OBJECT_SUCCESS
				{
					public static LocString SEARCH1 = "I look around and recover an old file.\n\nNew Database Entry discovered.";

					public static LocString SEARCH2 = "There's a three-ringed binder inside. I scan the surviving documents.\n\nNew Database Entry discovered.";

					public static LocString SEARCH3 = "A discarded journal inside remains mostly intact. I scan the pages of use.\n\nNew Database Entry discovered.";

					public static LocString SEARCH4 = "A single page of a long printout remains legible. I scan it and add it to my database.\n\nNew Database Entry discovered.";

					public static LocString SEARCH5 = "A few loose papers can be found inside. I scan the ones that look interesting.\n\nNew Database Entry discovered.";

					public static LocString SEARCH6 = "I find a memory stick inside and copy its data into my database.\n\nNew Database Entry discovered.";
				}

				public class SEARCH_OBJECT_FAIL
				{
					public static LocString SEARCH1 = "I look around but find nothing of interest.";
				}

				public class SEARCH_SPACEPOI_SUCCESS
				{
					public static LocString SEARCH1 = "A quick analysis of the hardware of this debris has uncovered some searchable files within.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH2 = "There's an archaic interface I can interact with on this device.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH3 = "While investigating the software of this wreckage, a compelling file comes to my attention.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH4 = "Not much remains of the software that once ran this spacecraft except for one file that piques my interest.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH5 = "I find some noteworthy data hidden amongst the system files of this space junk.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH6 = "Despite being subjected to years of degradation, there are still recoverable files in this machinery.\n\nNew Database Entry unlocked.";
				}

				public class SEARCH_SPACEPOI_FAIL
				{
					public static LocString SEARCH1 = "There's nothing of interest left in this old space junk.";

					public static LocString SEARCH2 = "I've salvaged everything I can from this vehicle.";

					public static LocString SEARCH3 = "Years of neglect and radioactive decay have destroyed all the useful data from this derelict spacecraft.";
				}
			}

			public class OPENPOI
			{
				public static LocString NAME = "Rummage";

				public static LocString TOOLTIP = "Scrounge for usable materials";

				public static LocString NAME_OFF = "Cancel Rummage";

				public static LocString TOOLTIP_OFF = "Cancel this rummage order";
			}

			public class EMPTYSTORAGE
			{
				public static LocString NAME = "Empty Storage";

				public static LocString TOOLTIP = "Eject all resources from this container";

				public static LocString NAME_OFF = "Cancel Empty";

				public static LocString TOOLTIP_OFF = "Cancel this empty order";
			}

			public class COPY_BUILDING_SETTINGS
			{
				public static LocString NAME = "Copy Settings";

				public static LocString TOOLTIP = "Apply the settings and priorities of this building to other buildings of the same type {Hotkey}";
			}

			public class CLEAR
			{
				public static LocString NAME = "Sweep";

				public static LocString TOOLTIP = "Put this object away in the nearest storage container";

				public static LocString NAME_OFF = "Cancel Sweeping";

				public static LocString TOOLTIP_OFF = "Cancel this sweep order";
			}

			public class COMPOST
			{
				public static LocString NAME = "Compost";

				public static LocString TOOLTIP = "Mark this object for compost";

				public static LocString NAME_OFF = "Cancel Compost";

				public static LocString TOOLTIP_OFF = "Cancel this compost order";
			}

			public class PICKUPABLEMOVE
			{
				public static LocString NAME = "Move To";

				public static LocString TOOLTIP = "Move this object to a specific location";

				public static LocString NAME_OFF = "Cancel Move";

				public static LocString TOOLTIP_OFF = "Cancel order to move this object";
			}

			public class UNEQUIP
			{
				public static LocString NAME = "Unequip {0}";

				public static LocString TOOLTIP = "Take off and drop this equipment";
			}

			public class QUARANTINE
			{
				public static LocString NAME = "Quarantine";

				public static LocString TOOLTIP = "Isolate this Duplicant\nThe Duplicant will return to their assigned Cot";

				public static LocString TOOLTIP_DISABLED = "No quarantine zone assigned";

				public static LocString NAME_OFF = "Cancel Quarantine";

				public static LocString TOOLTIP_OFF = "Cancel this quarantine order";
			}

			public class DRAWPATHS
			{
				public static LocString NAME = "Show Navigation";

				public static LocString TOOLTIP = "Show all areas within this Duplicant's reach";

				public static LocString NAME_OFF = "Hide Navigation";

				public static LocString TOOLTIP_OFF = "Hide areas within this Duplicant's reach";
			}

			public class MOVETOLOCATION
			{
				public static LocString NAME = "Move To";

				public static LocString TOOLTIP = "Move this Duplicant to a specific location";
			}

			public class FOLLOWCAM
			{
				public static LocString NAME = "Follow Cam";

				public static LocString TOOLTIP = "Track this Duplicant with the camera";
			}

			public class WORKABLE_DIRECTION_BOTH
			{
				public static LocString NAME = " Set Direction: Both";

				public static LocString TOOLTIP = "Select to make Duplicants wash when passing by in either direction";
			}

			public class WORKABLE_DIRECTION_LEFT
			{
				public static LocString NAME = "Set Direction: Left";

				public static LocString TOOLTIP = "Select to make Duplicants wash when passing by from right to left";
			}

			public class WORKABLE_DIRECTION_RIGHT
			{
				public static LocString NAME = "Set Direction: Right";

				public static LocString TOOLTIP = "Select to make Duplicants wash when passing by from left to right";
			}

			public class MANUAL_PUMP_DELIVERY
			{
				public static class ALLOWED
				{
					public static LocString NAME = "Enable Auto-Bottle";

					public static LocString TOOLTIP = "If enabled, Duplicants will deliver bottled liquids to this building directly from Pitcher Pumps";
				}

				public static class DENIED
				{
					public static LocString NAME = "Disable Auto-Bottle";

					public static LocString TOOLTIP = "If disabled, Duplicants will no longer deliver bottled liquids directly from Pitcher Pumps";
				}

				public static class ALLOWED_GAS
				{
					public static LocString NAME = "Enable Auto-Bottle";

					public static LocString TOOLTIP = "If enabled, Duplicants will deliver gas canisters to this building directly from Canister Fillers";
				}

				public static class DENIED_GAS
				{
					public static LocString NAME = "Disable Auto-Bottle";

					public static LocString TOOLTIP = "If disabled, Duplicants will no longer deliver gas canisters directly from Canister Fillers";
				}
			}

			public class SUIT_MARKER_TRAVERSAL
			{
				public static class ONLY_WHEN_ROOM_AVAILABLE
				{
					public static LocString NAME = "Clearance: Vacancy";

					public static LocString TOOLTIP = "Suited Duplicants may only pass if there is an available dock to store their suit";
				}

				public static class ALWAYS
				{
					public static LocString NAME = "Clearance: Always";

					public static LocString TOOLTIP = "Suited Duplicants may pass even if there is no room to store their suits\n\nWhen all available docks are full, Duplicants will unequip their suits and drop them on the floor";
				}
			}

			public class ACTIVATEBUILDING
			{
				public static LocString ACTIVATE = "Activate";

				public static LocString TOOLTIP_ACTIVATE = "Request a Duplicant to activate this building";

				public static LocString TOOLTIP_ACTIVATED = "This building has already been activated";

				public static LocString ACTIVATE_CANCEL = "Cancel Activation";

				public static LocString ACTIVATED = "Activated";

				public static LocString TOOLTIP_CANCEL = "Cancel activation of this building";
			}

			public class ACCEPT_MUTANT_SEEDS
			{
				public static LocString ACCEPT = "Allow Mutants";

				public static LocString REJECT = "Forbid Mutants";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Toggle whether or not this building will accept ",
					UI.PRE_KEYWORD,
					"Mutant Seeds",
					UI.PST_KEYWORD,
					" for recipes that could use them"
				});

				public static LocString FISH_FEEDER_TOOLTIP = string.Concat(new string[]
				{
					"Toggle whether or not this feeder will accept ",
					UI.PRE_KEYWORD,
					"Mutant Seeds",
					UI.PST_KEYWORD,
					" for critters who eat them"
				});
			}
		}

		public class BUILDCATEGORIES
		{
			public static class BASE
			{
				public static LocString NAME = UI.FormatAsLink("Base", "BUILDCATEGORYBASE");

				public static LocString TOOLTIP = "Maintain the colony's infrastructure with these homebase basics. {Hotkey}";
			}

			public static class CONVEYANCE
			{
				public static LocString NAME = UI.FormatAsLink("Shipping", "BUILDCATEGORYCONVEYANCE");

				public static LocString TOOLTIP = "Transport ore and solid materials around my base. {Hotkey}";
			}

			public static class OXYGEN
			{
				public static LocString NAME = UI.FormatAsLink("Oxygen", "BUILDCATEGORYOXYGEN");

				public static LocString TOOLTIP = "Everything I need to keep the colony breathing. {Hotkey}";
			}

			public static class POWER
			{
				public static LocString NAME = UI.FormatAsLink("Power", "BUILDCATEGORYPOWER");

				public static LocString TOOLTIP = "Need to power the colony? Here's how to do it! {Hotkey}";
			}

			public static class FOOD
			{
				public static LocString NAME = UI.FormatAsLink("Food", "BUILDCATEGORYFOOD");

				public static LocString TOOLTIP = "Keep my Duplicants' spirits high and their bellies full. {Hotkey}";
			}

			public static class UTILITIES
			{
				public static LocString NAME = UI.FormatAsLink("Utilities", "BUILDCATEGORYUTILITIES");

				public static LocString TOOLTIP = "Heat up and cool down. {Hotkey}";
			}

			public static class PLUMBING
			{
				public static LocString NAME = UI.FormatAsLink("Plumbing", "BUILDCATEGORYPLUMBING");

				public static LocString TOOLTIP = "Get the colony's water running and its sewage flowing. {Hotkey}";
			}

			public static class HVAC
			{
				public static LocString NAME = UI.FormatAsLink("Ventilation", "BUILDCATEGORYHVAC");

				public static LocString TOOLTIP = "Control the flow of gas in the base. {Hotkey}";
			}

			public static class REFINING
			{
				public static LocString NAME = UI.FormatAsLink("Refinement", "BUILDCATEGORYREFINING");

				public static LocString TOOLTIP = "Use the resources I want, filter the ones I don't. {Hotkey}";
			}

			public static class ROCKETRY
			{
				public static LocString NAME = UI.FormatAsLink("Rocketry", "BUILDCATEGORYROCKETRY");

				public static LocString TOOLTIP = "With rockets, the sky's no longer the limit! {Hotkey}";
			}

			public static class MEDICAL
			{
				public static LocString NAME = UI.FormatAsLink("Medicine", "BUILDCATEGORYMEDICAL");

				public static LocString TOOLTIP = "A cure for everything but the common cold. {Hotkey}";
			}

			public static class FURNITURE
			{
				public static LocString NAME = UI.FormatAsLink("Furniture", "BUILDCATEGORYFURNITURE");

				public static LocString TOOLTIP = "Amenities to keep my Duplicants happy, comfy and efficient. {Hotkey}";
			}

			public static class EQUIPMENT
			{
				public static LocString NAME = UI.FormatAsLink("Stations", "BUILDCATEGORYEQUIPMENT");

				public static LocString TOOLTIP = "Unlock new technologies through the power of science! {Hotkey}";
			}

			public static class MISC
			{
				public static LocString NAME = UI.FormatAsLink("Decor", "BUILDCATEGORYMISC");

				public static LocString TOOLTIP = "Spruce up my colony with some lovely interior decorating. {Hotkey}";
			}

			public static class AUTOMATION
			{
				public static LocString NAME = UI.FormatAsLink("Automation", "BUILDCATEGORYAUTOMATION");

				public static LocString TOOLTIP = "Automate my base with a wide range of sensors. {Hotkey}";
			}

			public static class HEP
			{
				public static LocString NAME = UI.FormatAsLink("Radiation", "BUILDCATEGORYHEP");

				public static LocString TOOLTIP = "Here's where things get rad. {Hotkey}";
			}
		}

		public class NEWBUILDCATEGORIES
		{
			public static class BASE
			{
				public static LocString NAME = UI.FormatAsLink("Base", "BUILD_CATEGORY_BASE");

				public static LocString TOOLTIP = "Maintain the colony's infrastructure with these homebase basics. {Hotkey}";
			}

			public static class INFRASTRUCTURE
			{
				public static LocString NAME = UI.FormatAsLink("Utilities", "BUILD_CATEGORY_INFRASTRUCTURE");

				public static LocString TOOLTIP = "Power, plumbing, and ventilation can all be found here. {Hotkey}";
			}

			public static class FOODANDAGRICULTURE
			{
				public static LocString NAME = UI.FormatAsLink("Food", "BUILD_CATEGORY_FOODANDAGRICULTURE");

				public static LocString TOOLTIP = "Keep my Duplicants' spirits high and their bellies full. {Hotkey}";
			}

			public static class LOGISTICS
			{
				public static LocString NAME = UI.FormatAsLink("Logistics", "BUILD_CATEGORY_LOGISTICS");

				public static LocString TOOLTIP = "Devices for base automation and material transport. {Hotkey}";
			}

			public static class HEALTHANDHAPPINESS
			{
				public static LocString NAME = UI.FormatAsLink("Accommodation", "BUILD_CATEGORY_HEALTHANDHAPPINESS");

				public static LocString TOOLTIP = "Everything a Duplicant needs to stay happy, healthy, and fulfilled. {Hotkey}";
			}

			public static class INDUSTRIAL
			{
				public static LocString NAME = UI.FormatAsLink("Industrials", "BUILD_CATEGORY_INDUSTRIAL");

				public static LocString TOOLTIP = "Machinery for oxygen production, heat management, and material refinement. {Hotkey}";
			}

			public static class LADDERS
			{
				public static LocString NAME = "Ladders";

				public static LocString BUILDMENUTITLE = "Ladders";

				public static LocString TOOLTIP = "";
			}

			public static class TILES
			{
				public static LocString NAME = "Tiles and Drywall";

				public static LocString BUILDMENUTITLE = "Tiles and Drywall";

				public static LocString TOOLTIP = "";
			}

			public static class PRINTINGPODS
			{
				public static LocString NAME = "Printing Pods";

				public static LocString BUILDMENUTITLE = "Printing Pods";

				public static LocString TOOLTIP = "";
			}

			public static class DOORS
			{
				public static LocString NAME = "Doors";

				public static LocString BUILDMENUTITLE = "Doors";

				public static LocString TOOLTIP = "";
			}

			public static class STORAGE
			{
				public static LocString NAME = "Storage";

				public static LocString BUILDMENUTITLE = "Storage";

				public static LocString TOOLTIP = "";
			}

			public static class TRANSPORT
			{
				public static LocString NAME = "Transit Tubes";

				public static LocString BUILDMENUTITLE = "Transit Tubes";

				public static LocString TOOLTIP = "";
			}

			public static class PRODUCERS
			{
				public static LocString NAME = "Production";

				public static LocString BUILDMENUTITLE = "Production";

				public static LocString TOOLTIP = "";
			}

			public static class SCRUBBERS
			{
				public static LocString NAME = "Purification";

				public static LocString BUILDMENUTITLE = "Purification";

				public static LocString TOOLTIP = "";
			}

			public static class BATTERIES
			{
				public static LocString NAME = "Batteries";

				public static LocString BUILDMENUTITLE = "Batteries";

				public static LocString TOOLTIP = "";
			}

			public static class SWITCHES
			{
				public static LocString NAME = "Switches";

				public static LocString BUILDMENUTITLE = "Switches";

				public static LocString TOOLTIP = "";
			}

			public static class COOKING
			{
				public static LocString NAME = "Cooking";

				public static LocString BUILDMENUTITLE = "Cooking";

				public static LocString TOOLTIP = "";
			}

			public static class FARMING
			{
				public static LocString NAME = "Farming";

				public static LocString BUILDMENUTITLE = "Farming";

				public static LocString TOOLTIP = "";
			}

			public static class RANCHING
			{
				public static LocString NAME = "Ranching";

				public static LocString BUILDMENUTITLE = "Ranching";

				public static LocString TOOLTIP = "";
			}

			public static class WASHROOM
			{
				public static LocString NAME = "Washroom";

				public static LocString BUILDMENUTITLE = "Washroom";

				public static LocString TOOLTIP = "";
			}

			public static class VALVES
			{
				public static LocString NAME = "Valves";

				public static LocString BUILDMENUTITLE = "Valves";

				public static LocString TOOLTIP = "";
			}

			public static class PUMPS
			{
				public static LocString NAME = "Pumps";

				public static LocString BUILDMENUTITLE = "Pumps";

				public static LocString TOOLTIP = "";
			}

			public static class SENSORS
			{
				public static LocString NAME = "Sensors";

				public static LocString BUILDMENUTITLE = "Sensors";

				public static LocString TOOLTIP = "";
			}

			public static class PORTS
			{
				public static LocString NAME = "Ports";

				public static LocString BUILDMENUTITLE = "Ports";

				public static LocString TOOLTIP = "";
			}

			public static class MATERIALS
			{
				public static LocString NAME = "Materials";

				public static LocString BUILDMENUTITLE = "Materials";

				public static LocString TOOLTIP = "";
			}

			public static class OIL
			{
				public static LocString NAME = "Oil";

				public static LocString BUILDMENUTITLE = "Oil";

				public static LocString TOOLTIP = "";
			}

			public static class ADVANCED
			{
				public static LocString NAME = "Advanced";

				public static LocString BUILDMENUTITLE = "Advanced";

				public static LocString TOOLTIP = "";
			}

			public static class BEDS
			{
				public static LocString NAME = "Beds";

				public static LocString BUILDMENUTITLE = "Beds";

				public static LocString TOOLTIP = "";
			}

			public static class LIGHTS
			{
				public static LocString NAME = "Lights";

				public static LocString BUILDMENUTITLE = "Lights";

				public static LocString TOOLTIP = "";
			}

			public static class DINING
			{
				public static LocString NAME = "Dining";

				public static LocString BUILDMENUTITLE = "Dining";

				public static LocString TOOLTIP = "";
			}

			public static class MANUFACTURING
			{
				public static LocString NAME = "Manufacturing";

				public static LocString BUILDMENUTITLE = "Manufacturing";

				public static LocString TOOLTIP = "";
			}

			public static class TEMPERATURE
			{
				public static LocString NAME = "Temperature";

				public static LocString BUILDMENUTITLE = "Temperature";

				public static LocString TOOLTIP = "";
			}

			public static class RESEARCH
			{
				public static LocString NAME = "Research";

				public static LocString BUILDMENUTITLE = "Research";

				public static LocString TOOLTIP = "";
			}

			public static class GENERATORS
			{
				public static LocString NAME = "Generators";

				public static LocString BUILDMENUTITLE = "Generators";

				public static LocString TOOLTIP = "";
			}

			public static class WIRES
			{
				public static LocString NAME = "Wires";

				public static LocString BUILDMENUTITLE = "Wires";

				public static LocString TOOLTIP = "";
			}

			public static class LOGICGATES
			{
				public static LocString NAME = "Gates";

				public static LocString BUILDMENUTITLE = "Gates";

				public static LocString TOOLTIP = "";
			}

			public static class TRANSMISSIONS
			{
				public static LocString NAME = "Transmissions";

				public static LocString BUILDMENUTITLE = "Transmissions";

				public static LocString TOOLTIP = "";
			}

			public static class LOGICMANAGER
			{
				public static LocString NAME = "Monitoring";

				public static LocString BUILDMENUTITLE = "Monitoring";

				public static LocString TOOLTIP = "";
			}

			public static class LOGICAUDIO
			{
				public static LocString NAME = "Ambience";

				public static LocString BUILDMENUTITLE = "Ambience";

				public static LocString TOOLTIP = "";
			}

			public static class CONVEYANCESTRUCTURES
			{
				public static LocString NAME = "Structural";

				public static LocString BUILDMENUTITLE = "Structural";

				public static LocString TOOLTIP = "";
			}

			public static class BUILDMENUPORTS
			{
				public static LocString NAME = "Ports";

				public static LocString BUILDMENUTITLE = "Ports";

				public static LocString TOOLTIP = "";
			}

			public static class POWERCONTROL
			{
				public static LocString NAME = "Power\nRegulation";

				public static LocString BUILDMENUTITLE = "Power Regulation";

				public static LocString TOOLTIP = "";
			}

			public static class PLUMBINGSTRUCTURES
			{
				public static LocString NAME = "Plumbing";

				public static LocString BUILDMENUTITLE = "Plumbing";

				public static LocString TOOLTIP = "Get the colony's water running and its sewage flowing. {Hotkey}";
			}

			public static class PIPES
			{
				public static LocString NAME = "Pipes";

				public static LocString BUILDMENUTITLE = "Pipes";

				public static LocString TOOLTIP = "";
			}

			public static class VENTILATIONSTRUCTURES
			{
				public static LocString NAME = "Ventilation";

				public static LocString BUILDMENUTITLE = "Ventilation";

				public static LocString TOOLTIP = "Control the flow of gas in your base. {Hotkey}";
			}

			public static class CONVEYANCE
			{
				public static LocString NAME = "Ore\nTransport";

				public static LocString BUILDMENUTITLE = "Ore Transport";

				public static LocString TOOLTIP = "Transport ore and solid materials around my base. {Hotkey}";
			}

			public static class HYGIENE
			{
				public static LocString NAME = "Hygiene";

				public static LocString BUILDMENUTITLE = "Hygiene";

				public static LocString TOOLTIP = "Keeps my Duplicants clean.";
			}

			public static class MEDICAL
			{
				public static LocString NAME = "Medical";

				public static LocString BUILDMENUTITLE = "Medical";

				public static LocString TOOLTIP = "A cure for everything but the common cold. {Hotkey}";
			}

			public static class WELLNESS
			{
				public static LocString NAME = "Wellness";

				public static LocString BUILDMENUTITLE = "Wellness";

				public static LocString TOOLTIP = "";
			}

			public static class RECREATION
			{
				public static LocString NAME = "Recreation";

				public static LocString BUILDMENUTITLE = "Recreation";

				public static LocString TOOLTIP = "Everything needed to reduce stress and increase fun.";
			}

			public static class FURNITURE
			{
				public static LocString NAME = "Furniture";

				public static LocString BUILDMENUTITLE = "Furniture";

				public static LocString TOOLTIP = "Amenities to keep my Duplicants happy, comfy and efficient. {Hotkey}";
			}

			public static class DECOR
			{
				public static LocString NAME = "Decor";

				public static LocString BUILDMENUTITLE = "Decor";

				public static LocString TOOLTIP = "Spruce up your colony with some lovely interior decorating. {Hotkey}";
			}

			public static class OXYGEN
			{
				public static LocString NAME = "Oxygen";

				public static LocString BUILDMENUTITLE = "Oxygen";

				public static LocString TOOLTIP = "Everything I need to keep my colony breathing. {Hotkey}";
			}

			public static class UTILITIES
			{
				public static LocString NAME = "Temperature";

				public static LocString BUILDMENUTITLE = "Temperature";

				public static LocString TOOLTIP = "";
			}

			public static class REFINING
			{
				public static LocString NAME = "Refinement";

				public static LocString BUILDMENUTITLE = "Refinement";

				public static LocString TOOLTIP = "Use the resources you want, filter the ones you don't. {Hotkey}";
			}

			public static class EQUIPMENT
			{
				public static LocString NAME = "Equipment";

				public static LocString BUILDMENUTITLE = "Equipment";

				public static LocString TOOLTIP = "Unlock new technologies through the power of science! {Hotkey}";
			}

			public static class ARCHAEOLOGY
			{
				public static LocString NAME = "Archaeology";

				public static LocString BUILDMENUTITLE = "Archaeology";

				public static LocString TOOLTIP = "";
			}

			public static class METEORDEFENSE
			{
				public static LocString NAME = "Meteor Defense";

				public static LocString BUILDMENUTITLE = "Meteor Defense";

				public static LocString TOOLTIP = "";
			}

			public static class INDUSTRIALSTATION
			{
				public static LocString NAME = "Industrial";

				public static LocString BUILDMENUTITLE = "Industrial";

				public static LocString TOOLTIP = "";
			}

			public static class TELESCOPES
			{
				public static LocString NAME = "Telescopes";

				public static LocString BUILDMENUTITLE = "Telescopes";

				public static LocString TOOLTIP = "Unlock new technologies through the power of science! {Hotkey}";
			}

			public static class MISSILES
			{
				public static LocString NAME = "Meteor Defense";

				public static LocString BUILDMENUTITLE = "Meteor Defense";

				public static LocString TOOLTIP = "";
			}

			public static class FITTINGS
			{
				public static LocString NAME = "Fittings";

				public static LocString BUILDMENUTITLE = "Fittings";

				public static LocString TOOLTIP = "";
			}

			public static class SANITATION
			{
				public static LocString NAME = "Sanitation";

				public static LocString BUILDMENUTITLE = "Sanitation";

				public static LocString TOOLTIP = "";
			}

			public static class AUTOMATED
			{
				public static LocString NAME = "Automated";

				public static LocString BUILDMENUTITLE = "Automated";

				public static LocString TOOLTIP = "";
			}

			public static class ROCKETSTRUCTURES
			{
				public static LocString NAME = "Structural";

				public static LocString BUILDMENUTITLE = "Structural";

				public static LocString TOOLTIP = "";
			}

			public static class ROCKETNAV
			{
				public static LocString NAME = "Navigation";

				public static LocString BUILDMENUTITLE = "Navigation";

				public static LocString TOOLTIP = "";
			}

			public static class CONDUITSENSORS
			{
				public static LocString NAME = "Pipe Sensors";

				public static LocString BUILDMENUTITLE = "Pipe Sensors";

				public static LocString TOOLTIP = "";
			}

			public static class ROCKETRY
			{
				public static LocString NAME = "Rocketry";

				public static LocString BUILDMENUTITLE = "Rocketry";

				public static LocString TOOLTIP = "Rocketry {Hotkey}";
			}

			public static class ENGINES
			{
				public static LocString NAME = "Engines";

				public static LocString BUILDMENUTITLE = "Engines";

				public static LocString TOOLTIP = "";
			}

			public static class TANKS
			{
				public static LocString NAME = "Tanks";

				public static LocString BUILDMENUTITLE = "Tanks";

				public static LocString TOOLTIP = "";
			}

			public static class CARGO
			{
				public static LocString NAME = "Cargo";

				public static LocString BUILDMENUTITLE = "Cargo";

				public static LocString TOOLTIP = "";
			}

			public static class MODULE
			{
				public static LocString NAME = "Modules";

				public static LocString BUILDMENUTITLE = "Modules";

				public static LocString TOOLTIP = "";
			}
		}

		public class TOOLS
		{
			public static LocString TOOL_AREA_FMT = "{0} x {1}\n{2} tiles";

			public static LocString TOOL_LENGTH_FMT = "{0}";

			public static LocString FILTER_HOVERCARD_HEADER = "   <style=\"hovercard_element\">({0})</style>";

			public class SANDBOX
			{
				public class SANDBOX_TOGGLE
				{
					public static LocString NAME = "SANDBOX";
				}

				public class BRUSH
				{
					public static LocString NAME = "Brush";

					public static LocString HOVERACTION = "PAINT SIM";
				}

				public class SPRINKLE
				{
					public static LocString NAME = "Sprinkle";

					public static LocString HOVERACTION = "SPRINKLE SIM";
				}

				public class FLOOD
				{
					public static LocString NAME = "Fill";

					public static LocString HOVERACTION = "PAINT SECTION";
				}

				public class MARQUEE
				{
					public static LocString NAME = "Marquee";
				}

				public class SAMPLE
				{
					public static LocString NAME = "Sample";

					public static LocString HOVERACTION = "COPY SELECTION";
				}

				public class HEATGUN
				{
					public static LocString NAME = "Heat Gun";

					public static LocString HOVERACTION = "PAINT HEAT";
				}

				public class RADSTOOL
				{
					public static LocString NAME = "Radiation Tool";

					public static LocString HOVERACTION = "PAINT RADS";
				}

				public class STRESSTOOL
				{
					public static LocString NAME = "Happy Tool";

					public static LocString HOVERACTION = "PAINT CALM";
				}

				public class SPAWNER
				{
					public static LocString NAME = "Spawner";

					public static LocString HOVERACTION = "SPAWN";
				}

				public class CLEAR_FLOOR
				{
					public static LocString NAME = "Clear Floor";

					public static LocString HOVERACTION = "DELETE DEBRIS";
				}

				public class DESTROY
				{
					public static LocString NAME = "Destroy";

					public static LocString HOVERACTION = "DELETE";
				}

				public class SPAWN_ENTITY
				{
					public static LocString NAME = "Spawn";
				}

				public class FOW
				{
					public static LocString NAME = "Reveal";

					public static LocString HOVERACTION = "DE-FOG";
				}

				public class CRITTER
				{
					public static LocString NAME = "Critter Removal";

					public static LocString HOVERACTION = "DELETE CRITTERS";
				}
			}

			public class GENERIC
			{
				public static LocString BACK = "Back";

				public static LocString UNKNOWN = "UNKNOWN";

				public static LocString BUILDING_HOVER_NAME_FMT = "{Name}    <style=\"hovercard_element\">({Element})</style>";

				public static LocString LOGIC_INPUT_HOVER_FMT = "{Port}    <style=\"hovercard_element\">({Name})</style>";

				public static LocString LOGIC_OUTPUT_HOVER_FMT = "{Port}    <style=\"hovercard_element\">({Name})</style>";

				public static LocString LOGIC_MULTI_INPUT_HOVER_FMT = "{Port}    <style=\"hovercard_element\">({Name})</style>";

				public static LocString LOGIC_MULTI_OUTPUT_HOVER_FMT = "{Port}    <style=\"hovercard_element\">({Name})</style>";
			}

			public class ATTACK
			{
				public static LocString NAME = "Attack";

				public static LocString TOOLNAME = "Attack tool";

				public static LocString TOOLACTION = "DRAG";
			}

			public class CAPTURE
			{
				public static LocString NAME = "Wrangle";

				public static LocString TOOLNAME = "Wrangle tool";

				public static LocString TOOLACTION = "DRAG";

				public static LocString NOT_CAPTURABLE = "Cannot Wrangle";
			}

			public class BUILD
			{
				public static LocString NAME = "Build {0}";

				public static LocString TOOLNAME = "Build tool";

				public static LocString TOOLACTION = UI.CLICK(UI.ClickType.CLICK) + " TO BUILD";

				public static LocString TOOLACTION_DRAG = "DRAG";
			}

			public class PLACE
			{
				public static LocString NAME = "Place {0}";

				public static LocString TOOLNAME = "Place tool";

				public static LocString TOOLACTION = UI.CLICK(UI.ClickType.CLICK) + " TO PLACE";

				public class REASONS
				{
					public static LocString CAN_OCCUPY_AREA = "Location blocked";

					public static LocString ON_FOUNDATION = "Must place on the ground";

					public static LocString VISIBLE_TO_SPACE = "Must have a clear path to space";

					public static LocString RESTRICT_TO_WORLD = "Incorrect " + UI.CLUSTERMAP.PLANETOID;
				}
			}

			public class MOVETOLOCATION
			{
				public static LocString NAME = "Move";

				public static LocString TOOLNAME = "Move Here";

				public static LocString TOOLACTION = UI.CLICK(UI.ClickType.CLICK) ?? "";

				public static LocString UNREACHABLE = "UNREACHABLE";
			}

			public class COPYSETTINGS
			{
				public static LocString NAME = "Paste Settings";

				public static LocString TOOLNAME = "Paste Settings Tool";

				public static LocString TOOLACTION = "DRAG";
			}

			public class DIG
			{
				public static LocString NAME = "Dig";

				public static LocString TOOLNAME = "Dig tool";

				public static LocString TOOLACTION = "DRAG";
			}

			public class DISINFECT
			{
				public static LocString NAME = "Disinfect";

				public static LocString TOOLNAME = "Disinfect tool";

				public static LocString TOOLACTION = "DRAG";
			}

			public class DISCONNECT
			{
				public static LocString NAME = "Disconnect";

				public static LocString TOOLTIP = "Sever conduits and connectors {Hotkey}";

				public static LocString TOOLNAME = "Disconnect tool";

				public static LocString TOOLACTION = "DRAG";
			}

			public class CANCEL
			{
				public static LocString NAME = "Cancel";

				public static LocString TOOLNAME = "Cancel tool";

				public static LocString TOOLACTION = "DRAG";
			}

			public class DECONSTRUCT
			{
				public static LocString NAME = "Deconstruct";

				public static LocString TOOLNAME = "Deconstruct tool";

				public static LocString TOOLACTION = "DRAG";
			}

			public class CLEANUPCATEGORY
			{
				public static LocString NAME = "Clean";

				public static LocString TOOLNAME = "Clean Up tools";
			}

			public class PRIORITIESCATEGORY
			{
				public static LocString NAME = "Priority";
			}

			public class MARKFORSTORAGE
			{
				public static LocString NAME = "Sweep";

				public static LocString TOOLNAME = "Sweep tool";

				public static LocString TOOLACTION = "DRAG";
			}

			public class MOP
			{
				public static LocString NAME = "Mop";

				public static LocString TOOLNAME = "Mop tool";

				public static LocString TOOLACTION = "DRAG";

				public static LocString TOO_MUCH_LIQUID = "Too Much Liquid";

				public static LocString NOT_ON_FLOOR = "Not On Floor";
			}

			public class HARVEST
			{
				public static LocString NAME = "Harvest";

				public static LocString TOOLNAME = "Harvest tool";

				public static LocString TOOLACTION = "DRAG";
			}

			public class PRIORITIZE
			{
				public static LocString NAME = "Priority";

				public static LocString TOOLNAME = "Priority tool";

				public static LocString TOOLACTION = "DRAG";

				public static LocString SPECIFIC_PRIORITY = "Set Priority: {0}";
			}

			public class EMPTY_PIPE
			{
				public static LocString NAME = "Empty Pipe";

				public static LocString TOOLTIP = "Extract pipe contents {Hotkey}";

				public static LocString TOOLNAME = "Empty Pipe tool";

				public static LocString TOOLACTION = "DRAG";
			}

			public class FILTERSCREEN
			{
				public static LocString OPTIONS = "Tool Filter";
			}

			public class FILTERLAYERS
			{
				public static LocString BUILDINGS = "Buildings";

				public static LocString TILES = "Tiles";

				public static LocString WIRES = "Power Wires";

				public static LocString LIQUIDPIPES = "Liquid Pipes";

				public static LocString GASPIPES = "Gas Pipes";

				public static LocString SOLIDCONDUITS = "Conveyor Rails";

				public static LocString DIGPLACER = "Dig Orders";

				public static LocString CLEANANDCLEAR = "Sweep & Mop Orders";

				public static LocString ALL = "All";

				public static LocString HARVEST_WHEN_READY = "Enable Harvest";

				public static LocString DO_NOT_HARVEST = "Disable Harvest";

				public static LocString ATTACK = "Attack";

				public static LocString LOGIC = "Automation";

				public static LocString BACKWALL = "Background Buildings";

				public static LocString METAL = "Metal";

				public static LocString BUILDABLE = "Mineral";

				public static LocString FILTER = "Filtration Medium";

				public static LocString LIQUIFIABLE = "Liquefiable";

				public static LocString LIQUID = "Liquid";

				public static LocString GAS = "Gas";

				public static LocString CONSUMABLEORE = "Consumable Ore";

				public static LocString ORGANICS = "Organic";

				public static LocString FARMABLE = "Cultivable Soil";

				public static LocString BREATHABLE = "Breathable Gas";

				public static LocString UNBREATHABLE = "Unbreathable Gas";

				public static LocString AGRICULTURE = "Agriculture";

				public static LocString ABSOLUTETEMPERATURE = "Temperature";

				public static LocString ADAPTIVETEMPERATURE = "Adapt. Temperature";

				public static LocString HEATFLOW = "Thermal Tolerance";

				public static LocString STATECHANGE = "State Change";

				public static LocString CONSTRUCTION = "Construction";

				public static LocString DIG = "Digging";

				public static LocString CLEAN = "Cleaning";

				public static LocString OPERATE = "Duties";
			}
		}

		public class DETAILTABS
		{
			public class STATS
			{
				public static LocString NAME = "Skills";

				public static LocString TOOLTIP = "View this Duplicant's attributes, traits, and daily stress";

				public static LocString GROUPNAME_ATTRIBUTES = "ATTRIBUTES";

				public static LocString GROUPNAME_STRESS = "TODAY'S STRESS";

				public static LocString GROUPNAME_EXPECTATIONS = "EXPECTATIONS";

				public static LocString GROUPNAME_TRAITS = "TRAITS";
			}

			public class SIMPLEINFO
			{
				public static LocString NAME = "Status";

				public static LocString TOOLTIP = "View the current status of the selected object";

				public static LocString GROUPNAME_STATUS = "STATUS";

				public static LocString GROUPNAME_DESCRIPTION = "INFORMATION";

				public static LocString GROUPNAME_CONDITION = "CONDITION";

				public static LocString GROUPNAME_REQUIREMENTS = "REQUIREMENTS";

				public static LocString GROUPNAME_RESEARCH = "RESEARCH";

				public static LocString GROUPNAME_LORE = "RECOVERED FILES";

				public static LocString GROUPNAME_FERTILITY = "EGG CHANCES";

				public static LocString GROUPNAME_ROCKET = "ROCKETRY";

				public static LocString GROUPNAME_CARGOBAY = "CARGO BAYS";

				public static LocString GROUPNAME_ELEMENTS = "RESOURCES";

				public static LocString GROUPNAME_LIFE = "LIFEFORMS";

				public static LocString GROUPNAME_BIOMES = "BIOMES";

				public static LocString GROUPNAME_GEYSERS = "GEYSERS";

				public static LocString GROUPNAME_METEORSHOWERS = "METEOR SHOWERS";

				public static LocString GROUPNAME_WORLDTRAITS = "WORLD TRAITS";

				public static LocString GROUPNAME_CLUSTER_POI = "POINT OF INTEREST";

				public static LocString GROUPNAME_MOVABLE = "MOVING";

				public static LocString NO_METEORSHOWERS = "No meteor showers forecasted";

				public static LocString NO_GEYSERS = "No geysers detected";

				public static LocString UNKNOWN_GEYSERS = "Unknown Geysers ({num})";
			}

			public class DETAILS
			{
				public static LocString NAME = "Properties";

				public static LocString MINION_NAME = "About";

				public static LocString TOOLTIP = "More information";

				public static LocString MINION_TOOLTIP = "More information";

				public static LocString GROUPNAME_DETAILS = "DETAILS";

				public static LocString GROUPNAME_CONTENTS = "CONTENTS";

				public static LocString GROUPNAME_MINION_CONTENTS = "CARRIED ITEMS";

				public static LocString STORAGE_EMPTY = "None";

				public static LocString CONTENTS_MASS = "{0}: {1}";

				public static LocString CONTENTS_TEMPERATURE = "{0} at {1}";

				public static LocString CONTENTS_ROTTABLE = "\n • {0}";

				public static LocString CONTENTS_DISEASED = "\n • {0}";

				public static LocString NET_STRESS = "<b>Today's Net Stress: {0}%</b>";

				public class RADIATIONABSORPTIONFACTOR
				{
					public static LocString NAME = "Radiation Blocking: {0}";

					public static LocString TOOLTIP = "This object will block approximately {0} of radiation.";
				}
			}

			public class PERSONALITY
			{
				public static LocString NAME = "Bio";

				public static LocString TOOLTIP = "View this Duplicant's personality, resume, and amenities";

				public static LocString GROUPNAME_BIO = "ABOUT";

				public static LocString GROUPNAME_RESUME = "{0}'S RESUME";

				public class RESUME
				{
					public static LocString MASTERED_SKILLS = "<b><size=13>Learned Skills:</size></b>";

					public static LocString MASTERED_SKILLS_TOOLTIP = string.Concat(new string[]
					{
						"All ",
						UI.PRE_KEYWORD,
						"Traits",
						UI.PST_KEYWORD,
						" and ",
						UI.PRE_KEYWORD,
						"Morale Needs",
						UI.PST_KEYWORD,
						" become permanent once a Duplicant has learned a new ",
						UI.PRE_KEYWORD,
						"Skill",
						UI.PST_KEYWORD,
						"\n\n",
						BUILDINGS.PREFABS.RESETSKILLSSTATION.NAME,
						"s can be built from the ",
						UI.FormatAsBuildMenuTab("Stations Tab", global::Action.Plan10),
						" to completely reset a Duplicant's learned ",
						UI.PRE_KEYWORD,
						"Skills",
						UI.PST_KEYWORD,
						", refunding all ",
						UI.PRE_KEYWORD,
						"Skill Points",
						UI.PST_KEYWORD
					});

					public static LocString JOBTRAINING_TOOLTIP = string.Concat(new string[]
					{
						"{0} learned this ",
						UI.PRE_KEYWORD,
						"Skill",
						UI.PST_KEYWORD,
						" while working as a {1}"
					});

					public class APTITUDES
					{
						public static LocString NAME = "<b><size=13>Personal Interests:</size></b>";

						public static LocString TOOLTIP = "{0} enjoys these types of work";
					}

					public class PERKS
					{
						public static LocString NAME = "<b><size=13>Skill Training:</size></b>";

						public static LocString TOOLTIP = "These are permanent skills {0} gained from learned skills";
					}

					public class CURRENT_ROLE
					{
						public static LocString NAME = "<size=13><b>Current Job:</b> {0}</size>";

						public static LocString TOOLTIP = "{0} is currently working as a {1}";

						public static LocString NOJOB_TOOLTIP = "This {0} is... \"between jobs\" at present";
					}

					public class NO_MASTERED_SKILLS
					{
						public static LocString NAME = "None";

						public static LocString TOOLTIP = string.Concat(new string[]
						{
							"{0} has not learned any ",
							UI.PRE_KEYWORD,
							"Skills",
							UI.PST_KEYWORD,
							" yet"
						});
					}
				}

				public class EQUIPMENT
				{
					public static LocString GROUPNAME_ROOMS = "AMENITIES";

					public static LocString GROUPNAME_OWNABLE = "EQUIPMENT";

					public static LocString NO_ASSIGNABLES = "None";

					public static LocString NO_ASSIGNABLES_TOOLTIP = "{0} has not been assigned any buildings of their own";

					public static LocString UNASSIGNED = "Unassigned";

					public static LocString UNASSIGNED_TOOLTIP = "This Duplicant has not been assigned a {0}";

					public static LocString ASSIGNED_TOOLTIP = "{2} has been assigned a {0}\n\nEffects: {1}";

					public static LocString NOEQUIPMENT = "None";

					public static LocString NOEQUIPMENT_TOOLTIP = "{0}'s wearing their Printday Suit and nothing more";
				}
			}

			public class ENERGYCONSUMER
			{
				public static LocString NAME = "Energy";

				public static LocString TOOLTIP = "View how much power this building consumes";
			}

			public class ENERGYWIRE
			{
				public static LocString NAME = "Energy";

				public static LocString TOOLTIP = "View this wire's network";
			}

			public class ENERGYGENERATOR
			{
				public static LocString NAME = "Energy";

				public static LocString TOOLTIP = "Monitor the power this building is generating";

				public static LocString CIRCUITOVERVIEW = "CIRCUIT OVERVIEW";

				public static LocString GENERATORS = "POWER GENERATORS";

				public static LocString CONSUMERS = "POWER CONSUMERS";

				public static LocString BATTERIES = "BATTERIES";

				public static LocString DISCONNECTED = "Not connected to an electrical circuit";

				public static LocString NOGENERATORS = "No generators on this circuit";

				public static LocString NOCONSUMERS = "No consumers on this circuit";

				public static LocString NOBATTERIES = "No batteries on this circuit";

				public static LocString AVAILABLE_JOULES = UI.FormatAsLink("Power", "POWER") + " stored: {0}";

				public static LocString AVAILABLE_JOULES_TOOLTIP = "Amount of power stored in batteries";

				public static LocString WATTAGE_GENERATED = UI.FormatAsLink("Power", "POWER") + " produced: {0}";

				public static LocString WATTAGE_GENERATED_TOOLTIP = "The total amount of power generated by this circuit";

				public static LocString WATTAGE_CONSUMED = UI.FormatAsLink("Power", "POWER") + " consumed: {0}";

				public static LocString WATTAGE_CONSUMED_TOOLTIP = "The total amount of power used by this circuit";

				public static LocString POTENTIAL_WATTAGE_CONSUMED = "Potential power consumed: {0}";

				public static LocString POTENTIAL_WATTAGE_CONSUMED_TOOLTIP = "The total amount of power that can be used by this circuit if all connected buildings are active";

				public static LocString MAX_SAFE_WATTAGE = "Maximum safe wattage: {0}";

				public static LocString MAX_SAFE_WATTAGE_TOOLTIP = "Exceeding this value will overload the circuit and can result in damage to wiring and buildings";
			}

			public class DISEASE
			{
				public static LocString NAME = "Germs";

				public static LocString TOOLTIP = "View the disease risk presented by the selected object";

				public static LocString DISEASE_SOURCE = "DISEASE SOURCE";

				public static LocString IMMUNE_SYSTEM = "GERM HOST";

				public static LocString CONTRACTION_RATES = "CONTRACTION RATES";

				public static LocString CURRENT_GERMS = "SURFACE GERMS";

				public static LocString NO_CURRENT_GERMS = "SURFACE GERMS";

				public static LocString GERMS_INFO = "GERM LIFE CYCLE";

				public static LocString INFECTION_INFO = "INFECTION DETAILS";

				public static LocString DISEASE_INFO_POPUP_HEADER = "DISEASE INFO: {0}";

				public static LocString DISEASE_INFO_POPUP_BUTTON = "FULL INFO";

				public static LocString DISEASE_INFO_POPUP_TOOLTIP = "View detailed germ and infection info for {0}";

				public class DETAILS
				{
					public static LocString NODISEASE = "No surface germs";

					public static LocString NODISEASE_TOOLTIP = "There are no germs present on this object";

					public static LocString DISEASE_AMOUNT = "{0}: {1}";

					public static LocString DISEASE_AMOUNT_TOOLTIP = "{0} are present on the surface of the selected object";

					public static LocString DEATH_FORMAT = "{0} dead/cycle";

					public static LocString DEATH_FORMAT_TOOLTIP = "Germ count is being reduced by {0}/cycle";

					public static LocString GROWTH_FORMAT = "{0} spawned/cycle";

					public static LocString GROWTH_FORMAT_TOOLTIP = "Germ count is being increased by {0}/cycle";

					public static LocString NEUTRAL_FORMAT = "No change";

					public static LocString NEUTRAL_FORMAT_TOOLTIP = "Germ count is static";

					public class GROWTH_FACTORS
					{
						public static LocString TITLE = "\nGrowth factors:";

						public static LocString TOOLTIP = "These conditions are contributing to the multiplication of germs";

						public static LocString RATE_OF_CHANGE = "Change rate: {0}";

						public static LocString RATE_OF_CHANGE_TOOLTIP = "Germ count is fluctuating at a rate of {0}";

						public static LocString HALF_LIFE_NEG = "Half life: {0}";

						public static LocString HALF_LIFE_NEG_TOOLTIP = "In {0} the germ count on this object will be halved";

						public static LocString HALF_LIFE_POS = "Doubling time: {0}";

						public static LocString HALF_LIFE_POS_TOOLTIP = "In {0} the germ count on this object will be doubled";

						public static LocString HALF_LIFE_NEUTRAL = "Static";

						public static LocString HALF_LIFE_NEUTRAL_TOOLTIP = "The germ count is neither increasing nor decreasing";

						public class SUBSTRATE
						{
							public static LocString GROW = "    • Growing on {0}: {1}";

							public static LocString GROW_TOOLTIP = "Contact with this substance is causing germs to multiply";

							public static LocString NEUTRAL = "    • No change on {0}";

							public static LocString NEUTRAL_TOOLTIP = "Contact with this substance has no effect on germ count";

							public static LocString DIE = "    • Dying on {0}: {1}";

							public static LocString DIE_TOOLTIP = "Contact with this substance is causing germs to die off";
						}

						public class ENVIRONMENT
						{
							public static LocString TITLE = "    • Surrounded by {0}: {1}";

							public static LocString GROW_TOOLTIP = "This atmosphere is causing germs to multiply";

							public static LocString DIE_TOOLTIP = "This atmosphere is causing germs to die off";
						}

						public class TEMPERATURE
						{
							public static LocString TITLE = "    • Current temperature {0}: {1}";

							public static LocString GROW_TOOLTIP = "This temperature is allowing germs to multiply";

							public static LocString DIE_TOOLTIP = "This temperature is causing germs to die off";
						}

						public class PRESSURE
						{
							public static LocString TITLE = "    • Current pressure {0}: {1}";

							public static LocString GROW_TOOLTIP = "Atmospheric pressure is causing germs to multiply";

							public static LocString DIE_TOOLTIP = "Atmospheric pressure is causing germs to die off";
						}

						public class RADIATION
						{
							public static LocString TITLE = "    • Exposed to {0} Rads: {1}";

							public static LocString DIE_TOOLTIP = "Radiation exposure is causing germs to die off";
						}

						public class DYING_OFF
						{
							public static LocString TITLE = "    • <b>Dying off: {0}</b>";

							public static LocString TOOLTIP = "Low germ count in this area is causing germs to die rapidly\n\nFewer than {0} are on this {1} of material.\n({2} germs/" + UI.UNITSUFFIXES.MASS.KILOGRAM + ")";
						}

						public class OVERPOPULATED
						{
							public static LocString TITLE = "    • <b>Overpopulated: {0}</b>";

							public static LocString TOOLTIP = "Too many germs are present in this area, resulting in rapid die-off until the population stabilizes\n\nA maximum of {0} can be on this {1} of material.\n({2} germs/" + UI.UNITSUFFIXES.MASS.KILOGRAM + ")";
						}
					}
				}
			}

			public class NEEDS
			{
				public static LocString NAME = "Stress";

				public static LocString TOOLTIP = "View this Duplicant's psychological status";

				public static LocString CURRENT_STRESS_LEVEL = "Current " + UI.FormatAsLink("Stress", "STRESS") + " Level: {0}";

				public static LocString OVERVIEW = "Overview";

				public static LocString STRESS_CREATORS = UI.FormatAsLink("Stress", "STRESS") + " Creators";

				public static LocString STRESS_RELIEVERS = UI.FormatAsLink("Stress", "STRESS") + " Relievers";

				public static LocString CURRENT_NEED_LEVEL = "Current Level: {0}";

				public static LocString NEXT_NEED_LEVEL = "Next Level: {0}";
			}

			public class EGG_CHANCES
			{
				public static LocString CHANCE_FORMAT = "{0}: {1}";

				public static LocString CHANCE_FORMAT_TOOLTIP = "This critter has a {1} chance of laying {0}s.\n\nThis probability increases when the creature:\n{2}";

				public static LocString CHANCE_MOD_FORMAT = "    • {0}\n";

				public static LocString CHANCE_FORMAT_TOOLTIP_NOMOD = "This critter has a {1} chance of laying {0}s.";
			}

			public class BUILDING_CHORES
			{
				public static LocString NAME = "Errands";

				public static LocString TOOLTIP = "See what errands this building can perform and view its current queue";

				public static LocString CHORE_TYPE_TOOLTIP = "Errand Type: {0}";

				public static LocString AVAILABLE_CHORES = "AVAILABLE ERRANDS";

				public static LocString DUPE_TOOLTIP_FAILED = "{Name} cannot currently {Errand}\n\nReason:\n{FailedPrecondition}";

				public static LocString DUPE_TOOLTIP_SUCCEEDED = "{Description}\n\n{Errand}'s Type: {Groups}\n\n{Name}'s {BestGroup} Priority: {PersonalPriorityValue} ({PersonalPriority})\n{Building} Priority: {BuildingPriority}\nAll {BestGroup} Errands: {TypePriority}\n\nTotal Priority: {TotalPriority}";

				public static LocString DUPE_TOOLTIP_DESC_ACTIVE = "{Name} is currently busy: \"{Errand}\"";

				public static LocString DUPE_TOOLTIP_DESC_INACTIVE = "\"{Errand}\" is #{Rank} on {Name}'s To Do list, after they finish their current errand";
			}

			public class PROCESS_CONDITIONS
			{
				public static LocString NAME = "LAUNCH CHECKLIST";

				public static LocString ROCKETPREP = "Rocket Construction";

				public static LocString ROCKETPREP_TOOLTIP = "It is recommended that all boxes on the Rocket Construction checklist be ticked before launching";

				public static LocString ROCKETSTORAGE = "Cargo Manifest";

				public static LocString ROCKETSTORAGE_TOOLTIP = "It is recommended that all boxes on the Cargo Manifest checklist be ticked before launching";

				public static LocString ROCKETFLIGHT = "Flight Route";

				public static LocString ROCKETFLIGHT_TOOLTIP = "A rocket requires a clear path to a set destination to conduct a mission";

				public static LocString ROCKETBOARD = "Crew Manifest";

				public static LocString ROCKETBOARD_TOOLTIP = "It is recommended that all boxes on the Crew Manifest checklist be ticked before launching";

				public static LocString ALL = "Requirements";

				public static LocString ALL_TOOLTIP = "These conditions must be fulfilled in order to launch a rocket mission";
			}
		}

		public class BUILDMENU
		{
			public static LocString GRID_VIEW_TOGGLE_TOOLTIP = "Toggle Grid View";

			public static LocString LIST_VIEW_TOGGLE_TOOLTIP = "Toggle List View";

			public static LocString NO_SEARCH_RESULTS = "NO RESULTS FOUND";

			public static LocString SEARCH_RESULTS_HEADER = "SEARCH RESULTS";

			public static LocString SEARCH_TEXT_PLACEHOLDER = "Search all buildings...";

			public static LocString CLEAR_SEARCH_TOOLTIP = "Clear search";
		}

		public class BUILDINGEFFECTS
		{
			public static LocString OPERATIONREQUIREMENTS = "<b>Requirements:</b>";

			public static LocString REQUIRESPOWER = UI.FormatAsLink("Power", "POWER") + ": {0}";

			public static LocString REQUIRESELEMENT = "Supply of {0}";

			public static LocString REQUIRESLIQUIDINPUT = UI.FormatAsLink("Liquid Intake Pipe", "LIQUIDPIPING");

			public static LocString REQUIRESLIQUIDOUTPUT = UI.FormatAsLink("Liquid Output Pipe", "LIQUIDPIPING");

			public static LocString REQUIRESLIQUIDOUTPUTS = "Two " + UI.FormatAsLink("Liquid Output Pipes", "LIQUIDPIPING");

			public static LocString REQUIRESGASINPUT = UI.FormatAsLink("Gas Intake Pipe", "GASPIPING");

			public static LocString REQUIRESGASOUTPUT = UI.FormatAsLink("Gas Output Pipe", "GASPIPING");

			public static LocString REQUIRESGASOUTPUTS = "Two " + UI.FormatAsLink("Gas Output Pipes", "GASPIPING");

			public static LocString REQUIRESMANUALOPERATION = "Duplicant operation";

			public static LocString REQUIRESCREATIVITY = "Duplicant " + UI.FormatAsLink("Creativity", "ARTIST");

			public static LocString REQUIRESPOWERGENERATOR = UI.FormatAsLink("Power", "POWER") + " generator";

			public static LocString REQUIRESSEED = "1 Unplanted " + UI.FormatAsLink("Seed", "PLANTS");

			public static LocString PREFERS_ROOM = "Preferred Room: {0}";

			public static LocString REQUIRESROOM = "Dedicated Room: {0}";

			public static LocString ALLOWS_FERTILIZER = "Plant " + UI.FormatAsLink("Fertilization", "WILTCONDITIONS");

			public static LocString ALLOWS_IRRIGATION = "Plant " + UI.FormatAsLink("Liquid", "WILTCONDITIONS");

			public static LocString ASSIGNEDDUPLICANT = "Duplicant assignment";

			public static LocString CONSUMESANYELEMENT = "Any Element";

			public static LocString ENABLESDOMESTICGROWTH = "Enables " + UI.FormatAsLink("Plant Domestication", "PLANTS");

			public static LocString TRANSFORMER_INPUT_WIRE = "Input " + UI.FormatAsLink("Power Wire", "WIRE");

			public static LocString TRANSFORMER_OUTPUT_WIRE = "Output " + UI.FormatAsLink("Power Wire", "WIRE") + " (Limited to {0})";

			public static LocString OPERATIONEFFECTS = "<b>Effects:</b>";

			public static LocString BATTERYCAPACITY = UI.FormatAsLink("Power", "POWER") + " capacity: {0}";

			public static LocString BATTERYLEAK = UI.FormatAsLink("Power", "POWER") + " leak: {0}";

			public static LocString STORAGECAPACITY = "Storage capacity: {0}";

			public static LocString ELEMENTEMITTED_INPUTTEMP = "{0}: {1}";

			public static LocString ELEMENTEMITTED_ENTITYTEMP = "{0}: {1}";

			public static LocString ELEMENTEMITTED_MINORENTITYTEMP = "{0}: {1}";

			public static LocString ELEMENTEMITTED_MINTEMP = "{0}: {1}";

			public static LocString ELEMENTEMITTED_FIXEDTEMP = "{0}: {1}";

			public static LocString ELEMENTCONSUMED = "{0}: {1}";

			public static LocString ELEMENTEMITTED_TOILET = "{0}: {1} per use";

			public static LocString ELEMENTEMITTEDPERUSE = "{0}: {1} per use";

			public static LocString DISEASEEMITTEDPERUSE = "{0}: {1} per use";

			public static LocString DISEASECONSUMEDPERUSE = "All Diseases: -{0} per use";

			public static LocString ELEMENTCONSUMEDPERUSE = "{0}: {1} per use";

			public static LocString ENERGYCONSUMED = UI.FormatAsLink("Power", "POWER") + " consumed: {0}";

			public static LocString ENERGYGENERATED = UI.FormatAsLink("Power", "POWER") + ": +{0}";

			public static LocString HEATGENERATED = UI.FormatAsLink("Heat", "HEAT") + ": +{0}/s";

			public static LocString HEATCONSUMED = UI.FormatAsLink("Heat", "HEAT") + ": -{0}/s";

			public static LocString HEATER_TARGETTEMPERATURE = "Target " + UI.FormatAsLink("Temperature", "HEAT") + ": {0}";

			public static LocString HEATGENERATED_AIRCONDITIONER = UI.FormatAsLink("Heat", "HEAT") + ": +{0} (Approximate Value)";

			public static LocString HEATGENERATED_LIQUIDCONDITIONER = UI.FormatAsLink("Heat", "HEAT") + ": +{0} (Approximate Value)";

			public static LocString FABRICATES = "Fabricates";

			public static LocString FABRICATEDITEM = "{1}";

			public static LocString PROCESSES = "Refines:";

			public static LocString PROCESSEDITEM = "{1} {0}";

			public static LocString PLANTERBOX_PENTALTY = "Planter box penalty";

			public static LocString DECORPROVIDED = UI.FormatAsLink("Decor", "DECOR") + ": {1} (Radius: {2} tiles)";

			public static LocString OVERHEAT_TEMP = "Overheat " + UI.FormatAsLink("Temperature", "HEAT") + ": {0}";

			public static LocString MINIMUM_TEMP = "Freeze " + UI.FormatAsLink("Temperature", "HEAT") + ": {0}";

			public static LocString OVER_PRESSURE_MASS = "Overpressure: {0}";

			public static LocString REFILLOXYGENTANK = "Refills Exosuit " + STRINGS.EQUIPMENT.PREFABS.OXYGEN_TANK.NAME;

			public static LocString DUPLICANTMOVEMENTBOOST = "Runspeed: {0}";

			public static LocString STRESSREDUCEDPERMINUTE = UI.FormatAsLink("Stress", "STRESS") + ": {0} per minute";

			public static LocString REMOVESEFFECTSUBTITLE = "Cures";

			public static LocString REMOVEDEFFECT = "{0}";

			public static LocString ADDED_EFFECT = "Added Effect: {0}";

			public static LocString GASCOOLING = UI.FormatAsLink("Cooling factor", "HEAT") + ": {0}";

			public static LocString LIQUIDCOOLING = UI.FormatAsLink("Cooling factor", "HEAT") + ": {0}";

			public static LocString MAX_WATTAGE = "Max " + UI.FormatAsLink("Power", "POWER") + ": {0}";

			public static LocString MAX_BITS = UI.FormatAsLink("Bit", "LOGIC") + " Depth: {0}";

			public static LocString RESEARCH_MATERIALS = "{0}: {1} per " + UI.FormatAsLink("Research", "RESEARCH") + " point";

			public static LocString PRODUCES_RESEARCH_POINTS = "{0}";

			public static LocString HIT_POINTS_PER_CYCLE = UI.FormatAsLink("Health", "Health") + " per cycle: {0}";

			public static LocString KCAL_PER_CYCLE = UI.FormatAsLink("KCal", "FOOD") + " per cycle: {0}";

			public static LocString REMOVES_DISEASE = "Kills germs";

			public static LocString DOCTORING = "Doctoring";

			public static LocString RECREATION = "Recreation";

			public static LocString COOLANT = "Coolant: {1} {0}";

			public static LocString REFINEMENT_ENERGY = "Heat: {0}";

			public static LocString IMPROVED_BUILDINGS = "Improved Buildings";

			public static LocString IMPROVED_BUILDINGS_ITEM = "{0}";

			public static LocString GEYSER_PRODUCTION = "{0}: {1} at {2}";

			public static LocString GEYSER_DISEASE = "Germs: {0}";

			public static LocString GEYSER_PERIOD = "Eruption Period: {0} every {1}";

			public static LocString GEYSER_YEAR_UNSTUDIED = "Active Period: (Requires Analysis)";

			public static LocString GEYSER_YEAR_PERIOD = "Active Period: {0} every {1}";

			public static LocString GEYSER_YEAR_NEXT_ACTIVE = "Next Activity: {0}";

			public static LocString GEYSER_YEAR_NEXT_DORMANT = "Next Dormancy: {0}";

			public static LocString GEYSER_YEAR_AVR_OUTPUT_UNSTUDIED = "Average Output: (Requires Analysis)";

			public static LocString GEYSER_YEAR_AVR_OUTPUT = "Average Output: {0}";

			public static LocString CAPTURE_METHOD_WRANGLE = "Capture Method: Wrangling";

			public static LocString CAPTURE_METHOD_LURE = "Capture Method: Lures";

			public static LocString CAPTURE_METHOD_TRAP = "Capture Method: Traps";

			public static LocString DIET_HEADER = "Digestion:";

			public static LocString DIET_CONSUMED = "    • Diet: {Foodlist}";

			public static LocString DIET_STORED = "    • Stores: {Foodlist}";

			public static LocString DIET_CONSUMED_ITEM = "{Food}: {Amount}";

			public static LocString DIET_PRODUCED = "    • Excretion: {Items}";

			public static LocString DIET_PRODUCED_ITEM = "{Item}: {Percent} of consumed mass";

			public static LocString DIET_PRODUCED_ITEM_FROM_PLANT = "{Item}: {Amount} when properly fed";

			public static LocString SCALE_GROWTH = "Shearable {Item}: {Amount} per {Time}";

			public static LocString SCALE_GROWTH_ATMO = "Shearable {Item}: {Amount} per {Time} ({Atmosphere})";

			public static LocString SCALE_GROWTH_TEMP = "Shearable {Item}: {Amount} per {Time} ({TempMin}-{TempMax})";

			public static LocString ACCESS_CONTROL = "Duplicant Access Permissions";

			public static LocString ROCKETRESTRICTION_HEADER = "Restriction Control:";

			public static LocString ROCKETRESTRICTION_BUILDINGS = "    • Buildings: {buildinglist}";

			public static LocString ITEM_TEMPERATURE_ADJUST = "Stored " + UI.FormatAsLink("Temperature", "HEAT") + ": {0}";

			public static LocString NOISE_CREATED = UI.FormatAsLink("Noise", "SOUND") + ": {0} dB (Radius: {1} tiles)";

			public static LocString MESS_TABLE_SALT = "Table Salt: +{0}";

			public static LocString ACTIVE_PARTICLE_CONSUMPTION = "Radbolts: {Rate}";

			public static LocString PARTICLE_PORT_INPUT = "Radbolt Input Port";

			public static LocString PARTICLE_PORT_OUTPUT = "Radbolt Output Port";

			public static LocString IN_ORBIT_REQUIRED = "Active In Space";

			public class TOOLTIPS
			{
				public static LocString OPERATIONREQUIREMENTS = "All requirements must be met in order for this building to operate";

				public static LocString REQUIRESPOWER = string.Concat(new string[]
				{
					"Must be connected to a power grid with at least ",
					UI.FormatAsNegativeRate("{0}"),
					" of available ",
					UI.PRE_KEYWORD,
					"Power",
					UI.PST_KEYWORD
				});

				public static LocString REQUIRESELEMENT = string.Concat(new string[]
				{
					"Must receive deliveries of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" to function"
				});

				public static LocString REQUIRESLIQUIDINPUT = string.Concat(new string[]
				{
					"Must receive ",
					UI.PRE_KEYWORD,
					"Liquid",
					UI.PST_KEYWORD,
					" from a ",
					BUILDINGS.PREFABS.LIQUIDCONDUIT.NAME,
					" system"
				});

				public static LocString REQUIRESLIQUIDOUTPUT = string.Concat(new string[]
				{
					"Must expel ",
					UI.PRE_KEYWORD,
					"Liquid",
					UI.PST_KEYWORD,
					" through a ",
					BUILDINGS.PREFABS.LIQUIDCONDUIT.NAME,
					" system"
				});

				public static LocString REQUIRESLIQUIDOUTPUTS = string.Concat(new string[]
				{
					"Must expel ",
					UI.PRE_KEYWORD,
					"Liquid",
					UI.PST_KEYWORD,
					" through a ",
					BUILDINGS.PREFABS.LIQUIDCONDUIT.NAME,
					" system"
				});

				public static LocString REQUIRESGASINPUT = string.Concat(new string[]
				{
					"Must receive ",
					UI.PRE_KEYWORD,
					"Gas",
					UI.PST_KEYWORD,
					" from a ",
					BUILDINGS.PREFABS.GASCONDUIT.NAME,
					" system"
				});

				public static LocString REQUIRESGASOUTPUT = string.Concat(new string[]
				{
					"Must expel ",
					UI.PRE_KEYWORD,
					"Gas",
					UI.PST_KEYWORD,
					" through a ",
					BUILDINGS.PREFABS.GASCONDUIT.NAME,
					" system"
				});

				public static LocString REQUIRESGASOUTPUTS = string.Concat(new string[]
				{
					"Must expel ",
					UI.PRE_KEYWORD,
					"Gas",
					UI.PST_KEYWORD,
					" through a ",
					BUILDINGS.PREFABS.GASCONDUIT.NAME,
					" system"
				});

				public static LocString REQUIRESMANUALOPERATION = "A Duplicant must be present to run this building";

				public static LocString REQUIRESCREATIVITY = "A Duplicant must work on this object to create " + UI.PRE_KEYWORD + "Art" + UI.PST_KEYWORD;

				public static LocString REQUIRESPOWERGENERATOR = string.Concat(new string[]
				{
					"Must be connected to a ",
					UI.PRE_KEYWORD,
					"Power",
					UI.PST_KEYWORD,
					" producing generator to function"
				});

				public static LocString REQUIRESSEED = "Must receive a plant " + UI.PRE_KEYWORD + "Seed" + UI.PST_KEYWORD;

				public static LocString PREFERS_ROOM = "This building gains additional effects or functionality when built inside a " + UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD;

				public static LocString REQUIRESROOM = string.Concat(new string[]
				{
					"Must be built within a dedicated ",
					UI.PRE_KEYWORD,
					"Room",
					UI.PST_KEYWORD,
					"\n\n",
					UI.PRE_KEYWORD,
					"Room",
					UI.PST_KEYWORD,
					" will become a ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" after construction"
				});

				public static LocString ALLOWS_FERTILIZER = string.Concat(new string[]
				{
					"Allows ",
					UI.PRE_KEYWORD,
					"Fertilizer",
					UI.PST_KEYWORD,
					" to be delivered to plants"
				});

				public static LocString ALLOWS_IRRIGATION = string.Concat(new string[]
				{
					"Allows ",
					UI.PRE_KEYWORD,
					"Liquid",
					UI.PST_KEYWORD,
					" to be delivered to plants"
				});

				public static LocString ALLOWS_IRRIGATION_PIPE = string.Concat(new string[]
				{
					"Allows irrigation ",
					UI.PRE_KEYWORD,
					"Pipe",
					UI.PST_KEYWORD,
					" connection"
				});

				public static LocString ASSIGNEDDUPLICANT = "This amenity may only be used by the Duplicant it is assigned to";

				public static LocString OPERATIONEFFECTS = "The building will produce these effects when its requirements are met";

				public static LocString BATTERYCAPACITY = string.Concat(new string[]
				{
					"Can hold <b>{0}</b> of ",
					UI.PRE_KEYWORD,
					"Power",
					UI.PST_KEYWORD,
					" when connected to a ",
					UI.PRE_KEYWORD,
					"Generator",
					UI.PST_KEYWORD
				});

				public static LocString BATTERYLEAK = string.Concat(new string[]
				{
					UI.FormatAsNegativeRate("{0}"),
					" of this battery's charge will be lost as ",
					UI.PRE_KEYWORD,
					"Heat",
					UI.PST_KEYWORD
				});

				public static LocString STORAGECAPACITY = "Holds up to <b>{0}</b> of material";

				public static LocString ELEMENTEMITTED_INPUTTEMP = string.Concat(new string[]
				{
					"Produces ",
					UI.FormatAsPositiveRate("{1}"),
					" of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" when in use\n\nIt will be the combined ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" of the input materials."
				});

				public static LocString ELEMENTEMITTED_ENTITYTEMP = string.Concat(new string[]
				{
					"Produces ",
					UI.FormatAsPositiveRate("{1}"),
					" of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" when in use\n\nIt will be the ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" of the building at the time of production"
				});

				public static LocString ELEMENTEMITTED_MINORENTITYTEMP = string.Concat(new string[]
				{
					"Produces ",
					UI.FormatAsPositiveRate("{1}"),
					" of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" when in use\n\nIt will be at least <b>{2}</b>, or hotter if the building is hotter."
				});

				public static LocString ELEMENTEMITTED_MINTEMP = string.Concat(new string[]
				{
					"Produces ",
					UI.FormatAsPositiveRate("{1}"),
					" of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" when in use\n\nIt will be at least <b>{2}</b>, or hotter if the input materials are hotter."
				});

				public static LocString ELEMENTEMITTED_FIXEDTEMP = string.Concat(new string[]
				{
					"Produces ",
					UI.FormatAsPositiveRate("{1}"),
					" of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" when in use\n\nIt will be produced at <b>{2}</b>."
				});

				public static LocString ELEMENTCONSUMED = string.Concat(new string[]
				{
					"Consumes ",
					UI.FormatAsNegativeRate("{1}"),
					" of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" when in use"
				});

				public static LocString ELEMENTEMITTED_TOILET = string.Concat(new string[]
				{
					"Produces ",
					UI.FormatAsPositiveRate("{1}"),
					" of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" per use\n\nDuplicant waste is emitted at <b>{2}</b>."
				});

				public static LocString ELEMENTEMITTEDPERUSE = string.Concat(new string[]
				{
					"Produces ",
					UI.FormatAsPositiveRate("{1}"),
					" of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" per use\n\nIt will be the ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" of the input materials."
				});

				public static LocString DISEASEEMITTEDPERUSE = string.Concat(new string[]
				{
					"Produces ",
					UI.FormatAsPositiveRate("{1}"),
					" of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" per use"
				});

				public static LocString DISEASECONSUMEDPERUSE = "Removes " + UI.FormatAsNegativeRate("{0}") + " per use";

				public static LocString ELEMENTCONSUMEDPERUSE = string.Concat(new string[]
				{
					"Consumes ",
					UI.FormatAsNegativeRate("{1}"),
					" of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" per use"
				});

				public static LocString ENERGYCONSUMED = string.Concat(new string[]
				{
					"Draws ",
					UI.FormatAsNegativeRate("{0}"),
					" from the ",
					UI.PRE_KEYWORD,
					"Power Grid",
					UI.PST_KEYWORD,
					" it's connected to"
				});

				public static LocString ENERGYGENERATED = string.Concat(new string[]
				{
					"Produces ",
					UI.FormatAsPositiveRate("{0}"),
					" for the ",
					UI.PRE_KEYWORD,
					"Power Grid",
					UI.PST_KEYWORD,
					" it's connected to"
				});

				public static LocString ENABLESDOMESTICGROWTH = string.Concat(new string[]
				{
					"Accelerates ",
					UI.PRE_KEYWORD,
					"Plant",
					UI.PST_KEYWORD,
					" growth and maturation"
				});

				public static LocString HEATGENERATED = string.Concat(new string[]
				{
					"Generates ",
					UI.FormatAsPositiveRate("{0}"),
					" per second\n\nSum ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" change is affected by the material attributes of the heated substance:\n    • mass\n    • specific heat capacity\n    • surface area\n    • insulation thickness\n    • thermal conductivity"
				});

				public static LocString HEATCONSUMED = string.Concat(new string[]
				{
					"Dissipates ",
					UI.FormatAsNegativeRate("{0}"),
					" per second\n\nSum ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" change can be affected by the material attributes of the cooled substance:\n    • mass\n    • specific heat capacity\n    • surface area\n    • insulation thickness\n    • thermal conductivity"
				});

				public static LocString HEATER_TARGETTEMPERATURE = string.Concat(new string[]
				{
					"Stops heating when the surrounding average ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" is above <b>{0}</b>"
				});

				public static LocString FABRICATES = "Fabrication is the production of items and equipment";

				public static LocString PROCESSES = "Processes raw materials into refined materials";

				public static LocString PROCESSEDITEM = "Refining this material produces " + UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD;

				public static LocString PLANTERBOX_PENTALTY = "Plants grow more slowly when contained within boxes";

				public static LocString DECORPROVIDED = string.Concat(new string[]
				{
					"Improves ",
					UI.PRE_KEYWORD,
					"Decor",
					UI.PST_KEYWORD,
					" values by ",
					UI.FormatAsPositiveModifier("<b>{0}</b>"),
					" in a <b>{1}</b> tile radius"
				});

				public static LocString DECORDECREASED = string.Concat(new string[]
				{
					"Decreases ",
					UI.PRE_KEYWORD,
					"Decor",
					UI.PST_KEYWORD,
					" values by ",
					UI.FormatAsNegativeModifier("<b>{0}</b>"),
					" in a <b>{1}</b> tile radius"
				});

				public static LocString OVERHEAT_TEMP = "Begins overheating at <b>{0}</b>";

				public static LocString MINIMUM_TEMP = "Ceases to function when temperatures fall below <b>{0}</b>";

				public static LocString OVER_PRESSURE_MASS = "Ceases to function when the surrounding mass is above <b>{0}</b>";

				public static LocString REFILLOXYGENTANK = string.Concat(new string[]
				{
					"Refills ",
					UI.PRE_KEYWORD,
					"Exosuit",
					UI.PST_KEYWORD,
					" Oxygen tanks with ",
					UI.PRE_KEYWORD,
					"Oxygen",
					UI.PST_KEYWORD,
					" for reuse"
				});

				public static LocString DUPLICANTMOVEMENTBOOST = "Duplicants walk <b>{0}</b> faster on this tile";

				public static LocString STRESSREDUCEDPERMINUTE = string.Concat(new string[]
				{
					"Removes <b>{0}</b> of Duplicants' ",
					UI.PRE_KEYWORD,
					"Stress",
					UI.PST_KEYWORD,
					" for every uninterrupted minute of use"
				});

				public static LocString REMOVESEFFECTSUBTITLE = "Use of this building will remove the listed effects";

				public static LocString REMOVEDEFFECT = "{0}";

				public static LocString ADDED_EFFECT = "Effect being applied:\n\n{0}\n{1}";

				public static LocString GASCOOLING = string.Concat(new string[]
				{
					"Reduces the ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" of piped ",
					UI.PRE_KEYWORD,
					"Gases",
					UI.PST_KEYWORD,
					" by <b>{0}</b>"
				});

				public static LocString LIQUIDCOOLING = string.Concat(new string[]
				{
					"Reduces the ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" of piped ",
					UI.PRE_KEYWORD,
					"Liquid",
					UI.PST_KEYWORD,
					" by <b>{0}</b>"
				});

				public static LocString MAX_WATTAGE = string.Concat(new string[]
				{
					"Drawing more than the maximum allowed ",
					UI.PRE_KEYWORD,
					"Watts",
					UI.PST_KEYWORD,
					" can result in damage to the circuit"
				});

				public static LocString MAX_BITS = string.Concat(new string[]
				{
					"Sending an ",
					UI.PRE_KEYWORD,
					"Automation Signal",
					UI.PST_KEYWORD,
					" with a higher ",
					UI.PRE_KEYWORD,
					"Bit Depth",
					UI.PST_KEYWORD,
					" than the connected ",
					UI.PRE_KEYWORD,
					"Logic Wire",
					UI.PST_KEYWORD,
					" can result in damage to the circuit"
				});

				public static LocString RESEARCH_MATERIALS = string.Concat(new string[]
				{
					"This research station consumes ",
					UI.FormatAsNegativeRate("{1}"),
					" of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" for each ",
					UI.PRE_KEYWORD,
					"Research Point",
					UI.PST_KEYWORD,
					" produced"
				});

				public static LocString PRODUCES_RESEARCH_POINTS = string.Concat(new string[]
				{
					"Produces ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" research"
				});

				public static LocString REMOVES_DISEASE = string.Concat(new string[]
				{
					"The cooking process kills all ",
					UI.PRE_KEYWORD,
					"Germs",
					UI.PST_KEYWORD,
					" present in the ingredients, removing the ",
					UI.PRE_KEYWORD,
					"Disease",
					UI.PST_KEYWORD,
					" risk when eating the product"
				});

				public static LocString DOCTORING = "Doctoring increases existing health benefits and can allow the treatment of otherwise stubborn " + UI.PRE_KEYWORD + "Diseases" + UI.PST_KEYWORD;

				public static LocString RECREATION = string.Concat(new string[]
				{
					"Improves Duplicant ",
					UI.PRE_KEYWORD,
					"Morale",
					UI.PST_KEYWORD,
					" during scheduled ",
					UI.PRE_KEYWORD,
					"Downtime",
					UI.PST_KEYWORD
				});

				public static LocString HEATGENERATED_AIRCONDITIONER = string.Concat(new string[]
				{
					"Generates ",
					UI.PRE_KEYWORD,
					"Heat",
					UI.PST_KEYWORD,
					" based on the ",
					UI.PRE_KEYWORD,
					"Volume",
					UI.PST_KEYWORD,
					" and ",
					UI.PRE_KEYWORD,
					"Specific Heat Capacity",
					UI.PST_KEYWORD,
					" of the pumped ",
					UI.PRE_KEYWORD,
					"Gas",
					UI.PST_KEYWORD,
					"\n\nCooling 1",
					UI.UNITSUFFIXES.MASS.KILOGRAM,
					" of ",
					ELEMENTS.OXYGEN.NAME,
					" the entire <b>{1}</b> will output <b>{0}</b>"
				});

				public static LocString HEATGENERATED_LIQUIDCONDITIONER = string.Concat(new string[]
				{
					"Generates ",
					UI.PRE_KEYWORD,
					"Heat",
					UI.PST_KEYWORD,
					" based on the ",
					UI.PRE_KEYWORD,
					"Volume",
					UI.PST_KEYWORD,
					" and ",
					UI.PRE_KEYWORD,
					"Specific Heat Capacity",
					UI.PST_KEYWORD,
					" of the pumped ",
					UI.PRE_KEYWORD,
					"Liquid",
					UI.PST_KEYWORD,
					"\n\nCooling 10",
					UI.UNITSUFFIXES.MASS.KILOGRAM,
					" of ",
					ELEMENTS.WATER.NAME,
					" the entire <b>{1}</b> will output <b>{0}</b>"
				});

				public static LocString MOVEMENT_BONUS = "Increases the Runspeed of Duplicants";

				public static LocString COOLANT = string.Concat(new string[]
				{
					"<b>{1}</b> of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" coolant is required to cool off an item produced by this building\n\nCoolant ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" increase is variable and dictated by the amount of energy needed to cool the produced item"
				});

				public static LocString REFINEMENT_ENERGY_HAS_COOLANT = string.Concat(new string[]
				{
					UI.FormatAsPositiveRate("{0}"),
					" of ",
					UI.PRE_KEYWORD,
					"Heat",
					UI.PST_KEYWORD,
					" will be produced to cool off the fabricated item\n\nThis will raise the ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" of the contained ",
					UI.PRE_KEYWORD,
					"{1}",
					UI.PST_KEYWORD,
					" by ",
					UI.FormatAsPositiveModifier("{2}"),
					", and heat the containing building"
				});

				public static LocString REFINEMENT_ENERGY_NO_COOLANT = string.Concat(new string[]
				{
					UI.FormatAsPositiveRate("{0}"),
					" of ",
					UI.PRE_KEYWORD,
					"Heat",
					UI.PST_KEYWORD,
					" will be produced to cool off the fabricated item\n\nIf ",
					UI.PRE_KEYWORD,
					"{1}",
					UI.PST_KEYWORD,
					" is used for coolant, its ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" will be raised by ",
					UI.FormatAsPositiveModifier("{2}"),
					", and will heat the containing building"
				});

				public static LocString IMPROVED_BUILDINGS = UI.PRE_KEYWORD + "Tune Ups" + UI.PST_KEYWORD + " will improve these buildings:";

				public static LocString IMPROVED_BUILDINGS_ITEM = "{0}";

				public static LocString GEYSER_PRODUCTION = string.Concat(new string[]
				{
					"While erupting, this geyser will produce ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" at a rate of ",
					UI.FormatAsPositiveRate("{1}"),
					", and at a ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" of <b>{2}</b>"
				});

				public static LocString GEYSER_PRODUCTION_GEOTUNED = string.Concat(new string[]
				{
					"While erupting, this geyser will produce ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" at a rate of ",
					UI.FormatAsPositiveRate("{1}"),
					", and at a ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" of <b>{2}</b>"
				});

				public static LocString GEYSER_PRODUCTION_GEOTUNED_COUNT = "<b>{0}</b> of <b>{1}</b> Geotuners targeting this geyser are amplifying it";

				public static LocString GEYSER_PRODUCTION_GEOTUNED_TOTAL = "Total geotuning: {0} {1}";

				public static LocString GEYSER_PRODUCTION_GEOTUNED_TOTAL_ROW_TITLE = "Geotuned ";

				public static LocString GEYSER_DISEASE = UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD + " germs are present in the output of this geyser";

				public static LocString GEYSER_PERIOD = "This geyser will produce for <b>{0}</b> of every <b>{1}</b>";

				public static LocString GEYSER_YEAR_UNSTUDIED = "A researcher must analyze this geyser to determine its geoactive period";

				public static LocString GEYSER_YEAR_PERIOD = "This geyser will be active for <b>{0}</b> out of every <b>{1}</b>\n\nIt will be dormant the rest of the time";

				public static LocString GEYSER_YEAR_NEXT_ACTIVE = "This geyser will become active in <b>{0}</b>";

				public static LocString GEYSER_YEAR_NEXT_DORMANT = "This geyser will become dormant in <b>{0}</b>";

				public static LocString GEYSER_YEAR_AVR_OUTPUT_UNSTUDIED = "A researcher must analyze this geyser to determine its average output rate";

				public static LocString GEYSER_YEAR_AVR_OUTPUT = "This geyser emits an average of {average} of {element} during its lifetime\n\nThis includes its dormant period";

				public static LocString GEYSER_YEAR_AVR_OUTPUT_BREAKDOWN_TITLE = "Total Geotuning ";

				public static LocString GEYSER_YEAR_AVR_OUTPUT_BREAKDOWN_ROW = "Geotuned ";

				public static LocString CAPTURE_METHOD_WRANGLE = string.Concat(new string[]
				{
					"This critter can be captured\n\nMark critters for capture using the ",
					UI.FormatAsTool("Wrangle Tool", global::Action.Capture),
					"\n\nDuplicants must possess the ",
					UI.PRE_KEYWORD,
					"Critter Ranching",
					UI.PST_KEYWORD,
					" Skill in order to wrangle critters"
				});

				public static LocString CAPTURE_METHOD_LURE = "This critter can be moved using an " + BUILDINGS.PREFABS.AIRBORNECREATURELURE.NAME;

				public static LocString CAPTURE_METHOD_TRAP = "This critter can be captured using a " + BUILDINGS.PREFABS.CREATURETRAP.NAME;

				public static LocString NOISE_POLLUTION_INCREASE = "Produces noise at <b>{0} dB</b> in a <b>{1}</b> tile radius";

				public static LocString NOISE_POLLUTION_DECREASE = "Dampens noise at <b>{0} dB</b> in a <b>{1}</b> tile radius";

				public static LocString ITEM_TEMPERATURE_ADJUST = string.Concat(new string[]
				{
					"Stored items will reach a ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" of <b>{0}</b> over time"
				});

				public static LocString DIET_HEADER = "Creatures will eat and digest only specific materials";

				public static LocString DIET_CONSUMED = "This critter can typically consume these materials at the following rates:\n\n{Foodlist}";

				public static LocString DIET_PRODUCED = "This critter will \"produce\" the following materials:\n\n{Items}";

				public static LocString ROCKETRESTRICTION_HEADER = "Controls whether a building is operational within a rocket interior";

				public static LocString ROCKETRESTRICTION_BUILDINGS = "This station controls the operational status of the following buildings:\n\n{buildinglist}";

				public static LocString SCALE_GROWTH = string.Concat(new string[]
				{
					"This critter can be sheared every <b>{Time}</b> to produce ",
					UI.FormatAsPositiveModifier("{Amount}"),
					" of ",
					UI.PRE_KEYWORD,
					"{Item}",
					UI.PST_KEYWORD
				});

				public static LocString SCALE_GROWTH_ATMO = string.Concat(new string[]
				{
					"This critter can be sheared every <b>{Time}</b> to produce ",
					UI.FormatAsPositiveRate("{Amount}"),
					" of ",
					UI.PRE_KEYWORD,
					"{Item}",
					UI.PST_KEYWORD,
					"\n\nIt must be kept in ",
					UI.PRE_KEYWORD,
					"{Atmosphere}",
					UI.PST_KEYWORD,
					"-rich environments to regrow sheared ",
					UI.PRE_KEYWORD,
					"{Item}",
					UI.PST_KEYWORD
				});

				public static LocString SCALE_GROWTH_TEMP = string.Concat(new string[]
				{
					"This critter can be sheared every <b>{Time}</b> to produce ",
					UI.FormatAsPositiveRate("{Amount}"),
					" of ",
					UI.PRE_KEYWORD,
					"{Item}",
					UI.PST_KEYWORD,
					"\n\nIt must eat food between {TempMin}-{TempMax} to regrow sheared ",
					UI.PRE_KEYWORD,
					"{Item}",
					UI.PST_KEYWORD
				});

				public static LocString MESS_TABLE_SALT = string.Concat(new string[]
				{
					"Duplicants gain ",
					UI.FormatAsPositiveModifier("+{0}"),
					" ",
					UI.PRE_KEYWORD,
					"Morale",
					UI.PST_KEYWORD,
					" when using ",
					UI.PRE_KEYWORD,
					"Table Salt",
					UI.PST_KEYWORD,
					" with their food at a ",
					BUILDINGS.PREFABS.DININGTABLE.NAME
				});

				public static LocString ACCESS_CONTROL = "Settings to allow or restrict Duplicants from passing through the door.";

				public static LocString TRANSFORMER_INPUT_WIRE = string.Concat(new string[]
				{
					"Connect a ",
					UI.PRE_KEYWORD,
					"Wire",
					UI.PST_KEYWORD,
					" to the large ",
					UI.PRE_KEYWORD,
					"Input",
					UI.PST_KEYWORD,
					" with any amount of ",
					UI.PRE_KEYWORD,
					"Watts",
					UI.PST_KEYWORD,
					"."
				});

				public static LocString TRANSFORMER_OUTPUT_WIRE = string.Concat(new string[]
				{
					"The ",
					UI.PRE_KEYWORD,
					"Power",
					UI.PST_KEYWORD,
					" provided by the the small ",
					UI.PRE_KEYWORD,
					"Output",
					UI.PST_KEYWORD,
					" will be limited to {0}."
				});

				public static LocString FABRICATOR_INGREDIENTS = "Ingredients:\n{0}";

				public static LocString ACTIVE_PARTICLE_CONSUMPTION = string.Concat(new string[]
				{
					"This building requires ",
					UI.PRE_KEYWORD,
					"Radbolts",
					UI.PST_KEYWORD,
					" to function, consuming them at a rate of {Rate} while in use"
				});

				public static LocString PARTICLE_PORT_INPUT = "A Radbolt Port on this building allows it to receive " + UI.PRE_KEYWORD + "Radbolts" + UI.PST_KEYWORD;

				public static LocString PARTICLE_PORT_OUTPUT = string.Concat(new string[]
				{
					"This building has a configurable Radbolt Port for ",
					UI.PRE_KEYWORD,
					"Radbolt",
					UI.PST_KEYWORD,
					" emission"
				});

				public static LocString IN_ORBIT_REQUIRED = "This building is only operational while its parent rocket is in flight";
			}
		}

		public class LOGIC_PORTS
		{
			public static LocString INPUT_PORTS = UI.FormatAsLink("Auto Inputs", "LOGIC");

			public static LocString INPUT_PORTS_TOOLTIP = "Input ports change a state on this building when a signal is received";

			public static LocString OUTPUT_PORTS = UI.FormatAsLink("Auto Outputs", "LOGIC");

			public static LocString OUTPUT_PORTS_TOOLTIP = "Output ports send a signal when this building changes state";

			public static LocString INPUT_PORT_TOOLTIP = "Input Behavior:\n• {0}\n• {1}";

			public static LocString OUTPUT_PORT_TOOLTIP = "Output Behavior:\n• {0}\n• {1}";

			public static LocString CONTROL_OPERATIONAL = "Enable/Disable";

			public static LocString CONTROL_OPERATIONAL_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Enable building";

			public static LocString CONTROL_OPERATIONAL_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Disable building";

			public static LocString PORT_INPUT_DEFAULT_NAME = "INPUT";

			public static LocString PORT_OUTPUT_DEFAULT_NAME = "OUTPUT";

			public static LocString GATE_MULTI_INPUT_ONE_NAME = "INPUT A";

			public static LocString GATE_MULTI_INPUT_ONE_ACTIVE = "Green Signal";

			public static LocString GATE_MULTI_INPUT_ONE_INACTIVE = "Red Signal";

			public static LocString GATE_MULTI_INPUT_TWO_NAME = "INPUT B";

			public static LocString GATE_MULTI_INPUT_TWO_ACTIVE = "Green Signal";

			public static LocString GATE_MULTI_INPUT_TWO_INACTIVE = "Red Signal";

			public static LocString GATE_MULTI_INPUT_THREE_NAME = "INPUT C";

			public static LocString GATE_MULTI_INPUT_THREE_ACTIVE = "Green Signal";

			public static LocString GATE_MULTI_INPUT_THREE_INACTIVE = "Red Signal";

			public static LocString GATE_MULTI_INPUT_FOUR_NAME = "INPUT D";

			public static LocString GATE_MULTI_INPUT_FOUR_ACTIVE = "Green Signal";

			public static LocString GATE_MULTI_INPUT_FOUR_INACTIVE = "Red Signal";

			public static LocString GATE_SINGLE_INPUT_ONE_NAME = "INPUT";

			public static LocString GATE_SINGLE_INPUT_ONE_ACTIVE = "Green Signal";

			public static LocString GATE_SINGLE_INPUT_ONE_INACTIVE = "Red Signal";

			public static LocString GATE_MULTI_OUTPUT_ONE_NAME = "OUTPUT A";

			public static LocString GATE_MULTI_OUTPUT_ONE_ACTIVE = "Green Signal";

			public static LocString GATE_MULTI_OUTPUT_ONE_INACTIVE = "Red Signal";

			public static LocString GATE_MULTI_OUTPUT_TWO_NAME = "OUTPUT B";

			public static LocString GATE_MULTI_OUTPUT_TWO_ACTIVE = "Green Signal";

			public static LocString GATE_MULTI_OUTPUT_TWO_INACTIVE = "Red Signal";

			public static LocString GATE_MULTI_OUTPUT_THREE_NAME = "OUTPUT C";

			public static LocString GATE_MULTI_OUTPUT_THREE_ACTIVE = "Green Signal";

			public static LocString GATE_MULTI_OUTPUT_THREE_INACTIVE = "Red Signal";

			public static LocString GATE_MULTI_OUTPUT_FOUR_NAME = "OUTPUT D";

			public static LocString GATE_MULTI_OUTPUT_FOUR_ACTIVE = "Green Signal";

			public static LocString GATE_MULTI_OUTPUT_FOUR_INACTIVE = "Red Signal";

			public static LocString GATE_SINGLE_OUTPUT_ONE_NAME = "OUTPUT";

			public static LocString GATE_SINGLE_OUTPUT_ONE_ACTIVE = "Green Signal";

			public static LocString GATE_SINGLE_OUTPUT_ONE_INACTIVE = "Red Signal";

			public static LocString GATE_MULTIPLEXER_CONTROL_ONE_NAME = "CONTROL A";

			public static LocString GATE_MULTIPLEXER_CONTROL_ONE_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Set signal path to <b>down</b> position";

			public static LocString GATE_MULTIPLEXER_CONTROL_ONE_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Set signal path to <b>up</b> position";

			public static LocString GATE_MULTIPLEXER_CONTROL_TWO_NAME = "CONTROL B";

			public static LocString GATE_MULTIPLEXER_CONTROL_TWO_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Set signal path to <b>down</b> position";

			public static LocString GATE_MULTIPLEXER_CONTROL_TWO_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Set signal path to <b>up</b> position";
		}

		public class GAMEOBJECTEFFECTS
		{
			public static LocString CALORIES = "+{0}";

			public static LocString FOOD_QUALITY = "Quality: {0}";

			public static LocString FOOD_MORALE = "Morale: {0}";

			public static LocString FORGAVEATTACKER = "Forgiveness";

			public static LocString COLDBREATHER = UI.FormatAsLink("Cooling Effect", "HEAT");

			public static LocString LIFECYCLETITLE = "Growth:";

			public static LocString GROWTHTIME_SIMPLE = "Life Cycle: {0}";

			public static LocString GROWTHTIME_REGROWTH = "Domestic growth: {0} / {1}";

			public static LocString GROWTHTIME = "Growth: {0}";

			public static LocString INITIALGROWTHTIME = "Initial Growth: {0}";

			public static LocString REGROWTHTIME = "Regrowth: {0}";

			public static LocString REQUIRES_LIGHT = UI.FormatAsLink("Light", "LIGHT") + ": {Lux}";

			public static LocString REQUIRES_DARKNESS = UI.FormatAsLink("Darkness", "LIGHT");

			public static LocString REQUIRESFERTILIZER = "{0}: {1}";

			public static LocString IDEAL_FERTILIZER = "{0}: {1}";

			public static LocString EQUIPMENT_MODS = "{Attribute} {Value}";

			public static LocString ROTTEN = "Rotten";

			public static LocString REQUIRES_ATMOSPHERE = UI.FormatAsLink("Atmosphere", "ATMOSPHERE") + ": {0}";

			public static LocString REQUIRES_PRESSURE = UI.FormatAsLink("Air", "ATMOSPHERE") + " Pressure: {0} minimum";

			public static LocString IDEAL_PRESSURE = UI.FormatAsLink("Air", "ATMOSPHERE") + " Pressure: {0}";

			public static LocString REQUIRES_TEMPERATURE = UI.FormatAsLink("Temperature", "HEAT") + ": {0} to {1}";

			public static LocString IDEAL_TEMPERATURE = UI.FormatAsLink("Temperature", "HEAT") + ": {0} to {1}";

			public static LocString REQUIRES_SUBMERSION = UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " Submersion";

			public static LocString FOOD_EFFECTS = "Effects:";

			public static LocString EMITS_LIGHT = UI.FormatAsLink("Light Range", "LIGHT") + ": {0} tiles";

			public static LocString EMITS_LIGHT_LUX = UI.FormatAsLink("Brightness", "LIGHT") + ": {0} Lux";

			public static LocString AMBIENT_RADIATION = "Ambient Radiation";

			public static LocString AMBIENT_RADIATION_FMT = "{minRads} - {maxRads}";

			public static LocString AMBIENT_NO_MIN_RADIATION_FMT = "Less than {maxRads}";

			public static LocString REQUIRES_NO_MIN_RADIATION = "Maximum " + UI.FormatAsLink("Radiation", "RADIATION") + ": {MaxRads}";

			public static LocString REQUIRES_RADIATION = UI.FormatAsLink("Radiation", "RADIATION") + ": {MinRads} to {MaxRads}";

			public static LocString MUTANT_STERILE = "Doesn't Drop " + UI.FormatAsLink("Seeds", "PLANTS");

			public static LocString DARKNESS = "Darkness";

			public static LocString LIGHT = "Light";

			public static LocString SEED_PRODUCTION_DIG_ONLY = "Consumes 1 " + UI.FormatAsLink("Seed", "PLANTS");

			public static LocString SEED_PRODUCTION_HARVEST = "Harvest yields " + UI.FormatAsLink("Seeds", "PLANTS");

			public static LocString SEED_PRODUCTION_FINAL_HARVEST = "Final harvest yields " + UI.FormatAsLink("Seeds", "PLANTS");

			public static LocString SEED_PRODUCTION_FRUIT = "Fruit produces " + UI.FormatAsLink("Seeds", "PLANTS");

			public static LocString SEED_REQUIREMENT_CEILING = "Plot Orientation: Downward";

			public static LocString SEED_REQUIREMENT_WALL = "Plot Orientation: Sideways";

			public static LocString REQUIRES_RECEPTACLE = "Farm Plot";

			public static LocString PLANT_MARK_FOR_HARVEST = "Autoharvest Enabled";

			public static LocString PLANT_DO_NOT_HARVEST = "Autoharvest Disabled";

			public class INSULATED
			{
				public static LocString NAME = "Insulated";

				public static LocString TOOLTIP = "Proper insulation drastically reduces thermal conductivity";
			}

			public class TOOLTIPS
			{
				public static LocString CALORIES = "+{0}";

				public static LocString FOOD_QUALITY = "Quality: {0}";

				public static LocString FOOD_MORALE = "Morale: {0}";

				public static LocString COLDBREATHER = "Lowers ambient air temperature";

				public static LocString GROWTHTIME_SIMPLE = "This plant takes <b>{0}</b> to grow";

				public static LocString GROWTHTIME_REGROWTH = "This plant initially takes <b>{0}</b> to grow, but only <b>{1}</b> to mature after first harvest";

				public static LocString GROWTHTIME = "This plant takes <b>{0}</b> to grow";

				public static LocString INITIALGROWTHTIME = "This plant takes <b>{0}</b> to mature again once replanted";

				public static LocString REGROWTHTIME = "This plant takes <b>{0}</b> to mature again once harvested";

				public static LocString EQUIPMENT_MODS = "{Attribute} {Value}";

				public static LocString REQUIRESFERTILIZER = string.Concat(new string[]
				{
					"This plant requires <b>{1}</b> ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" for basic growth"
				});

				public static LocString IDEAL_FERTILIZER = string.Concat(new string[]
				{
					"This plant requires <b>{1}</b> of ",
					UI.PRE_KEYWORD,
					"{0}",
					UI.PST_KEYWORD,
					" for basic growth"
				});

				public static LocString REQUIRES_LIGHT = string.Concat(new string[]
				{
					"This plant requires a ",
					UI.PRE_KEYWORD,
					"Light",
					UI.PST_KEYWORD,
					" source bathing it in at least {Lux}"
				});

				public static LocString REQUIRES_DARKNESS = "This plant requires complete darkness";

				public static LocString REQUIRES_ATMOSPHERE = "This plant must be submerged in one of the following gases: {0}";

				public static LocString REQUIRES_ATMOSPHERE_LIQUID = "This plant must be submerged in one of the following liquids: {0}";

				public static LocString REQUIRES_ATMOSPHERE_MIXED = "This plant must be submerged in one of the following gases or liquids: {0}";

				public static LocString REQUIRES_PRESSURE = string.Concat(new string[]
				{
					"Ambient ",
					UI.PRE_KEYWORD,
					"Gas",
					UI.PST_KEYWORD,
					" pressure must be at least <b>{0}</b> for basic growth"
				});

				public static LocString IDEAL_PRESSURE = string.Concat(new string[]
				{
					"This plant requires ",
					UI.PRE_KEYWORD,
					"Gas",
					UI.PST_KEYWORD,
					" pressures above <b>{0}</b> for basic growth"
				});

				public static LocString REQUIRES_TEMPERATURE = string.Concat(new string[]
				{
					"Internal ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" must be between <b>{0}</b> and <b>{1}</b> for basic growth"
				});

				public static LocString IDEAL_TEMPERATURE = string.Concat(new string[]
				{
					"This plant requires internal ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" between <b>{0}</b> and <b>{1}</b> for basic growth"
				});

				public static LocString REQUIRES_SUBMERSION = string.Concat(new string[]
				{
					"This plant must be fully submerged in ",
					UI.PRE_KEYWORD,
					"Liquid",
					UI.PST_KEYWORD,
					" for basic growth"
				});

				public static LocString FOOD_EFFECTS = "Duplicants will gain the following effects from eating this food: {0}";

				public static LocString REQUIRES_RECEPTACLE = string.Concat(new string[]
				{
					"This plant must be housed in a ",
					UI.FormatAsLink("Planter Box", "PLANTERBOX"),
					", ",
					UI.FormatAsLink("Farm Tile", "FARMTILE"),
					", or ",
					UI.FormatAsLink("Hydroponic Farm", "HYDROPONICFARM"),
					" farm to grow domestically"
				});

				public static LocString EMITS_LIGHT = string.Concat(new string[]
				{
					"Emits ",
					UI.PRE_KEYWORD,
					"Light",
					UI.PST_KEYWORD,
					"\n\nDuplicants can operate buildings more quickly when they're well lit"
				});

				public static LocString EMITS_LIGHT_LUX = string.Concat(new string[]
				{
					"Emits ",
					UI.PRE_KEYWORD,
					"Light",
					UI.PST_KEYWORD,
					"\n\nDuplicants can operate buildings more quickly when they're well lit"
				});

				public static LocString METEOR_SHOWER_SINGLE_METEOR_PERCENTAGE_TOOLTIP = "Distribution of meteor types in this shower";

				public static LocString SEED_PRODUCTION_DIG_ONLY = "May be replanted, but will produce no further " + UI.PRE_KEYWORD + "Seeds" + UI.PST_KEYWORD;

				public static LocString SEED_PRODUCTION_HARVEST = "Harvesting this plant will yield new " + UI.PRE_KEYWORD + "Seeds" + UI.PST_KEYWORD;

				public static LocString SEED_PRODUCTION_FINAL_HARVEST = string.Concat(new string[]
				{
					"Yields new ",
					UI.PRE_KEYWORD,
					"Seeds",
					UI.PST_KEYWORD,
					" on the final harvest of its life cycle"
				});

				public static LocString SEED_PRODUCTION_FRUIT = "Consuming this plant's fruit will yield new " + UI.PRE_KEYWORD + "Seeds" + UI.PST_KEYWORD;

				public static LocString SEED_REQUIREMENT_CEILING = "This seed must be planted in a downward facing plot\n\nPress " + UI.FormatAsKeyWord("[O]") + " while building farm plots to rotate them";

				public static LocString SEED_REQUIREMENT_WALL = "This seed must be planted in a side facing plot\n\nPress " + UI.FormatAsKeyWord("[O]") + " while building farm plots to rotate them";

				public static LocString REQUIRES_NO_MIN_RADIATION = "This plant will stop growing if exposed to more than {MaxRads} of " + UI.FormatAsLink("Radiation", "RADIATION");

				public static LocString REQUIRES_RADIATION = "This plant will only grow if it has between {MinRads} and {MaxRads} of " + UI.FormatAsLink("Radiation", "RADIATION");

				public static LocString MUTANT_SEED_TOOLTIP = "\n\nGrowing near its maximum radiation increases the chance of mutant seeds being produced";

				public static LocString MUTANT_STERILE = "This plant will not produce seeds of its own due to changes to its DNA";
			}

			public class DAMAGE_POPS
			{
				public static LocString OVERHEAT = "Overheat Damage";

				public static LocString CORROSIVE_ELEMENT = "Corrosive Element Damage";

				public static LocString WRONG_ELEMENT = "Wrong Element Damage";

				public static LocString CIRCUIT_OVERLOADED = "Overload Damage";

				public static LocString LOGIC_CIRCUIT_OVERLOADED = "Signal Overload Damage";

				public static LocString LIQUID_PRESSURE = "Pressure Damage";

				public static LocString MINION_DESTRUCTION = "Tantrum Damage";

				public static LocString CONDUIT_CONTENTS_FROZE = "Cold Damage";

				public static LocString CONDUIT_CONTENTS_BOILED = "Heat Damage";

				public static LocString MICROMETEORITE = "Micrometeorite Damage";

				public static LocString COMET = "Meteor Damage";

				public static LocString ROCKET = "Rocket Thruster Damage";
			}
		}

		public class ASTEROIDCLOCK
		{
			public static LocString CYCLE = "Cycle";

			public static LocString CYCLES_OLD = "This Colony is {0} Cycle(s) Old";

			public static LocString TIME_PLAYED = "Time Played: {0} hours";

			public static LocString SCHEDULE_BUTTON_TOOLTIP = "Manage Schedule";
		}

		public class ENDOFDAYREPORT
		{
			public static LocString REPORT_TITLE = "DAILY REPORTS";

			public static LocString DAY_TITLE = "Cycle {0}";

			public static LocString DAY_TITLE_TODAY = "Cycle {0} - Today";

			public static LocString DAY_TITLE_YESTERDAY = "Cycle {0} - Yesterday";

			public static LocString NOTIFICATION_TITLE = "Cycle {0} report ready";

			public static LocString NOTIFICATION_TOOLTIP = "The daily report for Cycle {0} is ready to view";

			public static LocString NEXT = "Next";

			public static LocString PREV = "Prev";

			public static LocString ADDED = "Added";

			public static LocString REMOVED = "Removed";

			public static LocString NET = "Net";

			public static LocString DUPLICANT_DETAILS_HEADER = "Duplicant Details:";

			public static LocString TIME_DETAILS_HEADER = "Total Time Details:";

			public static LocString BASE_DETAILS_HEADER = "Base Details:";

			public static LocString AVERAGE_TIME_DETAILS_HEADER = "Average Time Details:";

			public static LocString MY_COLONY = "my colony";

			public static LocString NONE = "None";

			public class OXYGEN_CREATED
			{
				public static LocString NAME = UI.FormatAsLink("Oxygen", "OXYGEN") + " Generation:";

				public static LocString POSITIVE_TOOLTIP = "{0} of " + UI.FormatAsLink("Oxygen", "OXYGEN") + " was produced by {1} over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "{0} of " + UI.FormatAsLink("Oxygen", "OXYGEN") + " was consumed by {1} over the course of the day";
			}

			public class CALORIES_CREATED
			{
				public static LocString NAME = "Calorie Generation:";

				public static LocString POSITIVE_TOOLTIP = "{0} of " + UI.FormatAsLink("Food", "FOOD") + " was produced by {1} over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "{0} of " + UI.FormatAsLink("Food", "FOOD") + " was consumed by {1} over the course of the day";
			}

			public class NUMBER_OF_DOMESTICATED_CRITTERS
			{
				public static LocString NAME = "Domesticated Critters:";

				public static LocString POSITIVE_TOOLTIP = "{0} domestic critters live in {1}";

				public static LocString NEGATIVE_TOOLTIP = "{0} domestic critters live in {1}";
			}

			public class NUMBER_OF_WILD_CRITTERS
			{
				public static LocString NAME = "Wild Critters:";

				public static LocString POSITIVE_TOOLTIP = "{0} wild critters live in {1}";

				public static LocString NEGATIVE_TOOLTIP = "{0} wild critters live in {1}";
			}

			public class ROCKETS_IN_FLIGHT
			{
				public static LocString NAME = "Rocket Missions Underway:";

				public static LocString POSITIVE_TOOLTIP = "{0} rockets are currently flying missions for {1}";

				public static LocString NEGATIVE_TOOLTIP = "{0} rockets are currently flying missions for {1}";
			}

			public class STRESS_DELTA
			{
				public static LocString NAME = UI.FormatAsLink("Stress", "STRESS") + " Change:";

				public static LocString POSITIVE_TOOLTIP = UI.FormatAsLink("Stress", "STRESS") + " increased by a total of {0} for {1}";

				public static LocString NEGATIVE_TOOLTIP = UI.FormatAsLink("Stress", "STRESS") + " decreased by a total of {0} for {1}";
			}

			public class TRAVELTIMEWARNING
			{
				public static LocString WARNING_TITLE = "Long Commutes";

				public static LocString WARNING_MESSAGE = "My Duplicants are spending a significant amount of time traveling between their errands (> {0})";
			}

			public class TRAVEL_TIME
			{
				public static LocString NAME = "Travel Time:";

				public static LocString POSITIVE_TOOLTIP = "On average, {1} spent {0} of their time traveling between tasks";
			}

			public class WORK_TIME
			{
				public static LocString NAME = "Work Time:";

				public static LocString POSITIVE_TOOLTIP = "On average, {0} of {1}'s time was spent working";
			}

			public class IDLE_TIME
			{
				public static LocString NAME = "Idle Time:";

				public static LocString POSITIVE_TOOLTIP = "On average, {0} of {1}'s time was spent idling";
			}

			public class PERSONAL_TIME
			{
				public static LocString NAME = "Personal Time:";

				public static LocString POSITIVE_TOOLTIP = "On average, {0} of {1}'s time was spent tending to personal needs";
			}

			public class ENERGY_USAGE
			{
				public static LocString NAME = UI.FormatAsLink("Power", "POWER") + " Usage:";

				public static LocString POSITIVE_TOOLTIP = "{0} of " + UI.FormatAsLink("Power", "POWER") + " was created by {1} over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "{0} of " + UI.FormatAsLink("Power", "POWER") + " was consumed by {1} over the course of the day";
			}

			public class ENERGY_WASTED
			{
				public static LocString NAME = UI.FormatAsLink("Power", "POWER") + " Wasted:";

				public static LocString NEGATIVE_TOOLTIP = "{0} of " + UI.FormatAsLink("Power", "POWER") + " was lost today due to battery runoff and overproduction in {1}";
			}

			public class LEVEL_UP
			{
				public static LocString NAME = "Skill Increases:";

				public static LocString TOOLTIP = "Today {1} gained a total of {0} skill levels";
			}

			public class TOILET_INCIDENT
			{
				public static LocString NAME = "Restroom Accidents:";

				public static LocString TOOLTIP = "{0} Duplicants couldn't quite reach the toilet in time today";
			}

			public class DISEASE_ADDED
			{
				public static LocString NAME = UI.FormatAsLink("Diseases", "DISEASE") + " Contracted:";

				public static LocString POSITIVE_TOOLTIP = "{0} " + UI.FormatAsLink("Disease", "DISEASE") + " were contracted by {1}";

				public static LocString NEGATIVE_TOOLTIP = "{0} " + UI.FormatAsLink("Disease", "DISEASE") + " were cured by {1}";
			}

			public class CONTAMINATED_OXYGEN_FLATULENCE
			{
				public static LocString NAME = UI.FormatAsLink("Flatulence", "CONTAMINATEDOXYGEN") + " Generation:";

				public static LocString POSITIVE_TOOLTIP = "{0} of " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " was generated by {1} over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "{0} of " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " was consumed by {1} over the course of the day";
			}

			public class CONTAMINATED_OXYGEN_TOILET
			{
				public static LocString NAME = UI.FormatAsLink("Toilet Emissions: ", "CONTAMINATEDOXYGEN");

				public static LocString POSITIVE_TOOLTIP = "{0} of " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " was generated by {1} over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "{0} of " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " was consumed by {1} over the course of the day";
			}

			public class CONTAMINATED_OXYGEN_SUBLIMATION
			{
				public static LocString NAME = UI.FormatAsLink("Sublimation", "CONTAMINATEDOXYGEN") + ":";

				public static LocString POSITIVE_TOOLTIP = "{0} of " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " was generated by {1} over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "{0} of " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " was consumed by {1} over the course of the day";
			}

			public class DISEASE_STATUS
			{
				public static LocString NAME = "Disease Status:";

				public static LocString TOOLTIP = "There are {0} covering {1}";
			}

			public class CHORE_STATUS
			{
				public static LocString NAME = "Errands:";

				public static LocString POSITIVE_TOOLTIP = "{0} errands are queued for {1}";

				public static LocString NEGATIVE_TOOLTIP = "{0} errands were completed over the course of the day by {1}";
			}

			public class NOTES
			{
				public static LocString NOTE_ENTRY_LINE_ITEM = "{0}\n{1}: {2}";

				public static LocString BUTCHERED = "Butchered for {0}";

				public static LocString BUTCHERED_CONTEXT = "Butchered";

				public static LocString CRAFTED = "Crafted a {0}";

				public static LocString CRAFTED_USED = "{0} used as ingredient";

				public static LocString CRAFTED_CONTEXT = "Crafted";

				public static LocString HARVESTED = "Harvested {0}";

				public static LocString HARVESTED_CONTEXT = "Harvested";

				public static LocString EATEN = "{0} eaten";

				public static LocString ROTTED = "Rotten {0}";

				public static LocString ROTTED_CONTEXT = "Rotted";

				public static LocString GERMS = "On {0}";

				public static LocString TIME_SPENT = "{0}";

				public static LocString WORK_TIME = "{0}";

				public static LocString PERSONAL_TIME = "{0}";

				public static LocString FOODFIGHT_CONTEXT = "{0} ingested in food fight";
			}
		}

		public static class SCHEDULEBLOCKTYPES
		{
			public static class EAT
			{
				public static LocString NAME = "Mealtime";

				public static LocString DESCRIPTION = "EAT:\nDuring Mealtime Duplicants will head to their assigned mess halls and eat.";
			}

			public static class SLEEP
			{
				public static LocString NAME = "Sleep";

				public static LocString DESCRIPTION = "SLEEP:\nWhen it's time to sleep, Duplicants will head to their assigned rooms and rest.";
			}

			public static class WORK
			{
				public static LocString NAME = "Work";

				public static LocString DESCRIPTION = "WORK:\nDuring Work hours Duplicants will perform any pending errands in the colony.";
			}

			public static class RECREATION
			{
				public static LocString NAME = "Recreation";

				public static LocString DESCRIPTION = "HAMMER TIME:\nDuring Hammer Time, Duplicants will relieve their " + UI.FormatAsLink("Stress", "STRESS") + " through dance. Please be aware that no matter how hard my Duplicants try, they will absolutely not be able to touch this.";
			}

			public static class HYGIENE
			{
				public static LocString NAME = "Hygiene";

				public static LocString DESCRIPTION = "HYGIENE:\nDuring " + UI.FormatAsLink("Hygiene", "HYGIENE") + " hours Duplicants will head to their assigned washrooms to get cleaned up.";
			}
		}

		public static class SCHEDULEGROUPS
		{
			public static LocString TOOLTIP_FORMAT = "{0}\n\n{1}";

			public static LocString MISSINGBLOCKS = "Warning: Scheduling Issues ({0})";

			public static LocString NOTIME = "No {0} shifts allotted";

			public static class HYGENE
			{
				public static LocString NAME = "Bathtime";

				public static LocString DESCRIPTION = "During Bathtime shifts my Duplicants will take care of their hygienic needs, such as going to the bathroom, using the shower or washing their hands.\n\nOnce they're all caught up on personal hygiene, Duplicants will head back to work.";

				public static LocString NOTIFICATION_TOOLTIP = string.Concat(new string[]
				{
					"During ",
					UI.PRE_KEYWORD,
					"Bathtime",
					UI.PST_KEYWORD,
					" shifts my Duplicants will take care of their hygienic needs, such as going to the bathroom, using the shower or washing their hands."
				});
			}

			public static class WORKTIME
			{
				public static LocString NAME = "Work";

				public static LocString DESCRIPTION = "During Work shifts my Duplicants must perform the errands I have placed for them throughout the colony.\n\nIt's important when scheduling to maintain a good work-life balance for my Duplicants to maintain their health and prevent Morale loss.";

				public static LocString NOTIFICATION_TOOLTIP = string.Concat(new string[]
				{
					"During ",
					UI.PRE_KEYWORD,
					"Work",
					UI.PST_KEYWORD,
					" shifts my Duplicants must perform the errands I've placed for them throughout the colony."
				});
			}

			public static class RECREATION
			{
				public static LocString NAME = "Downtime";

				public static LocString DESCRIPTION = "During Downtime my Duplicants they may do as they please.\n\nThis may include personal matters like bathroom visits or snacking, or they may choose to engage in leisure activities like socializing with friends.\n\nDowntime increases Duplicant Morale.";

				public static LocString NOTIFICATION_TOOLTIP = string.Concat(new string[]
				{
					"During ",
					UI.PRE_KEYWORD,
					"Downtime",
					UI.PST_KEYWORD,
					" shifts my Duplicants they may do as they please."
				});
			}

			public static class SLEEP
			{
				public static LocString NAME = "Bedtime";

				public static LocString DESCRIPTION = "My Duplicants use Bedtime shifts to rest up after a hard day's work.\n\nScheduling too few bedtime shifts may prevent my Duplicants from regaining enough Stamina to make it through the following day.";

				public static LocString NOTIFICATION_TOOLTIP = string.Concat(new string[]
				{
					"My Duplicants use ",
					UI.PRE_KEYWORD,
					"Bedtime",
					UI.PST_KEYWORD,
					" shifts to rest up after a hard day's work."
				});
			}
		}

		public class ELEMENTAL
		{
			public class AGE
			{
				public static LocString NAME = "Age: {0}";

				public static LocString TOOLTIP = "The selected object is {0} cycles old";

				public static LocString UNKNOWN = "Unknown";

				public static LocString UNKNOWN_TOOLTIP = "The age of the selected object is unknown";
			}

			public class UPTIME
			{
				public static LocString NAME = "Uptime:\n{0}{1}: {2}\n{0}{3}: {4}\n{0}{5}: {6}";

				public static LocString THIS_CYCLE = "This Cycle";

				public static LocString LAST_CYCLE = "Last Cycle";

				public static LocString LAST_X_CYCLES = "Last {0} Cycles";
			}

			public class PRIMARYELEMENT
			{
				public static LocString NAME = "Primary Element: {0}";

				public static LocString TOOLTIP = "The selected object is primarily composed of {0}";
			}

			public class UNITS
			{
				public static LocString NAME = "Stack Units: {0}";

				public static LocString TOOLTIP = "This stack contains {0} units of {1}";
			}

			public class MASS
			{
				public static LocString NAME = "Mass: {0}";

				public static LocString TOOLTIP = "The selected object has a mass of {0}";
			}

			public class TEMPERATURE
			{
				public static LocString NAME = "Temperature: {0}";

				public static LocString TOOLTIP = "The selected object's current temperature is {0}";
			}

			public class DISEASE
			{
				public static LocString NAME = "Disease: {0}";

				public static LocString TOOLTIP = "There are {0} on the selected object";
			}

			public class SHC
			{
				public static LocString NAME = "Specific Heat Capacity: {0}";

				public static LocString TOOLTIP = "{SPECIFIC_HEAT_CAPACITY} is required to heat 1 g of the selected object by 1 {TEMPERATURE_UNIT}";
			}

			public class THERMALCONDUCTIVITY
			{
				public static LocString NAME = "Thermal Conductivity: {0}";

				public static LocString TOOLTIP = "This object can conduct heat to other materials at a rate of {THERMAL_CONDUCTIVITY} W for each degree {TEMPERATURE_UNIT} difference\n\nBetween two objects, the rate of heat transfer will be determined by the object with the lowest Thermal Conductivity";

				public class ADJECTIVES
				{
					public static LocString VALUE_WITH_ADJECTIVE = "{0} ({1})";

					public static LocString VERY_LOW_CONDUCTIVITY = "Highly Insulating";

					public static LocString LOW_CONDUCTIVITY = "Insulating";

					public static LocString MEDIUM_CONDUCTIVITY = "Conductive";

					public static LocString HIGH_CONDUCTIVITY = "Highly Conductive";

					public static LocString VERY_HIGH_CONDUCTIVITY = "Extremely Conductive";
				}
			}

			public class CONDUCTIVITYBARRIER
			{
				public static LocString NAME = "Insulation Thickness: {0}";

				public static LocString TOOLTIP = "Thick insulation reduces an object's Thermal Conductivity";
			}

			public class VAPOURIZATIONPOINT
			{
				public static LocString NAME = "Vaporization Point: {0}";

				public static LocString TOOLTIP = "The selected object will evaporate into a gas at {0}";
			}

			public class MELTINGPOINT
			{
				public static LocString NAME = "Melting Point: {0}";

				public static LocString TOOLTIP = "The selected object will melt into a liquid at {0}";
			}

			public class OVERHEATPOINT
			{
				public static LocString NAME = "Overheat Modifier: {0}";

				public static LocString TOOLTIP = "This building will overheat and take damage if its temperature reaches {0}\n\nBuilding with better building materials can increase overheat temperature";
			}

			public class FREEZEPOINT
			{
				public static LocString NAME = "Freeze Point: {0}";

				public static LocString TOOLTIP = "The selected object will cool into a solid at {0}";
			}

			public class DEWPOINT
			{
				public static LocString NAME = "Condensation Point: {0}";

				public static LocString TOOLTIP = "The selected object will condense into a liquid at {0}";
			}
		}

		public class IMMIGRANTSCREEN
		{
			public static LocString IMMIGRANTSCREENTITLE = "Select a Blueprint";

			public static LocString PROCEEDBUTTON = "Print";

			public static LocString CANCELBUTTON = "Cancel";

			public static LocString REJECTALL = "Reject All";

			public static LocString EMBARK = "EMBARK";

			public static LocString SELECTDUPLICANTS = "Select {0} Duplicants";

			public static LocString SELECTYOURCREW = "CHOOSE THREE DUPLICANTS TO BEGIN";

			public static LocString SHUFFLE = "REROLL";

			public static LocString SHUFFLETOOLTIP = "Reroll for a different Duplicant";

			public static LocString BACK = "BACK";

			public static LocString CONFIRMATIONTITLE = "Reject All Printables?";

			public static LocString CONFIRMATIONBODY = "The Printing Pod will need time to recharge if I reject these Printables.";

			public static LocString NAME_YOUR_COLONY = "NAME THE COLONY";

			public static LocString CARE_PACKAGE_ELEMENT_QUANTITY = "{0} of {1}";

			public static LocString CARE_PACKAGE_ELEMENT_COUNT = "{0} x {1}";

			public static LocString CARE_PACKAGE_ELEMENT_COUNT_ONLY = "x {0}";

			public static LocString CARE_PACKAGE_CURRENT_AMOUNT = "Available: {0}";

			public static LocString DUPLICATE_COLONY_NAME = "A colony named \"{0}\" already exists";
		}

		public class METERS
		{
			public class HEALTH
			{
				public static LocString TOOLTIP = "Health";
			}

			public class BREATH
			{
				public static LocString TOOLTIP = "Oxygen";
			}

			public class FUEL
			{
				public static LocString TOOLTIP = "Fuel";
			}

			public class BATTERY
			{
				public static LocString TOOLTIP = "Battery Charge";
			}
		}
	}
}
