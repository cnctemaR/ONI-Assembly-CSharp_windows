using System;

namespace STRINGS
{
	public class UI
	{
		public static LocString NAME_WITH_UNITS = "{0} x {1}";

		public static LocString POSITIVE_FORMAT = "+{0}";

		public static LocString SPEED_SLOW = "SLOW";

		public static LocString SPEED_MEDIUM = "MEDIUM";

		public static LocString SPEED_FAST = "FAST";

		public static LocString RED_ALERT = "RED ALERT";

		public static LocString JOBS = "JOBS";

		public static LocString CONSUMABLES = "CONSUMABLES";

		public static LocString VITALS = "VITALS";

		public static LocString RESEARCH = "RESEARCH";

		public static LocString RESEARCHPOINTS = "Research points";

		public static LocString SCHEDULE = "SCHEDULE";

		public static LocString REPORT = "REPORTS";

		public static LocString OVERLAYSTITLE = "OVERLAYS";

		public static LocString ALERTS = "ALERTS";

		public static LocString MESSAGES = "MESSAGES";

		public static LocString ACTIONS = "ACTIONS";

		public static LocString QUEUE = "Queue";

		public static LocString BASECOUNT = "Base {0}";

		public static LocString CHARACTERCONTAINER_SKILLS_TITLE = "ATTRIBUTES";

		public static LocString CHARACTERCONTAINER_TRAITS_TITLE = "TRAITS";

		public static LocString CHARACTERCONTAINER_EXPECTATIONS_TITLE = "ADDITIONAL";

		public static LocString CHARACTERCONTAINER_SKILL_VALUE = " {0} {1}";

		public static LocString CHARACTERCONTAINER_NEED = "{0}: {1}";

		public static LocString CHARACTERCONTAINER_STRESSTRAIT = "Stress Response: {0}";

		public static LocString CHARACTERCONTAINER_CONGENITALTRAIT = "Genetic Trait: {0}";

		public static LocString PRODUCTINFO_SELECTMATERIAL = "Select {0}:";

		public static LocString PRODUCTINFO_RESEARCHREQUIRED = "Research required...";

		public static LocString PRODUCTINFO_REQUIRESRESEARCHDESC = "Requires {0} Research";

		public static LocString PRODUCTINFO_APPLICABLERESOURCES = "Required resources:";

		public static LocString PRODUCTINFO_MISSINGRESOURCES_TITLE = "Requires {0}: {1}";

		public static LocString PRODUCTINFO_MISSINGRESOURCES_HOVER = "Missing resource(s)";

		public static LocString PRODUCTINFO_MISSINGRESOURCES_DESC = "{0} has yet to be discovered.";

		public static LocString EQUIPMENTTAB_OWNED = "Owned Items";

		public static LocString EQUIPMENTTAB_HELD = "Held Items";

		public static LocString EQUIPMENTTAB_ROOM = "Assigned Rooms";

		public static LocString JOBSCREEN_PRIORITY = "Priority";

		public static LocString JOBSCREEN_HIGH = "High";

		public static LocString JOBSCREEN_LOW = "Low";

		public static LocString JOBSCREEN_EVERYONE = "Everyone";

		public static LocString JOBSCREEN_DEFAULT = "New Duplicants";

		public static LocString VITALSSCREEN_NAME = "Name";

		public static LocString VITALSSCREEN_STRESS = "Stress";

		public static LocString VITALSSCREEN_HEALTH = "Health";

		public static LocString VITALSSCREEN_CALORIES = "Fullness";

		public static LocString VITALSSCREEN_RATIONS = "Calories/Cycle";

		public static LocString VITALSSCREEN_EATENTODAY = "Eaten Today";

		public static LocString VITALSSCREEN_RATIONS_TOOLTIP = "Set how many calories this Duplicant may consume daily";

		public static LocString VITALSSCREEN_EATENTODAY_TOOLTIP = "The amount of food your Duplicant has eaten this cycle";

		public static LocString VITALSSCREEN_UNTIL_FULL = "Until Full";

		public static LocString RESEARCHSCREEN_UNLOCKSTOOLTIP = "Unlocks: {0}";

		public static LocString ATTRIBUTELEVEL = "Profession: Level {0} {1}";

		public static LocString NEUTRONIUMMASS = "Immeasurable";

		public static LocString CALCULATING = "Calculating...";

		public static LocString FORMATDAY = "{0} cycles";

		public static LocString FORMATSECONDS = "{0}s";

		public static LocString DELIVERED = "Delivered: {0} {1}";

		public static LocString PICKEDUP = "Picked Up: {0} {1}";

		public static LocString COPIED_SETTINGS = "Settings Applied";

		public static LocString WELCOMEMESSAGETITLE = "! ALERT !";

		public static LocString WELCOMEMESSAGEBODY = "Your crew has awoken miles beneath the surface of an unfamiliar asteroid. Oddly, they have no recollection of how they got here.";

		public static LocString WELCOMEMESSAGEBEGIN = "BEGIN";

		public static LocString VIEWDUPLICANTS = "Choose a Duplicant";

		public static LocString DUPLICANTPRINTING = "Duplicant Printing";

		public static LocString ASSIGNDUPLICANT = "Assign Duplicant";

		public static LocString CRAFT = "+1";

		public static LocString CRAFT_CONTINUOUS = "CONTINUOUS";

		public static LocString PLACEINRECEPTACLE = "Plant";

		public static LocString REMOVEFROMRECEPTACLE = "Uproot";

		public static LocString CANCELPLACEINRECEPTACLE = "Cancel";

		public static LocString CANCELREMOVALFROMRECEPTACLE = "Cancel";

		public static LocString CHANGEPERSECOND = "Change per second: {0}";

		public static LocString CHANGEPERCYCLE = "Change per cycle: {0}";

		public static LocString LISTENTRYSTRING = "     {0}\n";

		public static LocString LISTENTRYSTRINGNOLINEBREAK = "     {0}";

		public class FRONTEND
		{
			public static LocString GAME_VERSION = "Game Version: ";

			public static LocString LOADING = "Loading...";

			public class MAINMENU
			{
				public static LocString NEWGAME = "NEW GAME";

				public static LocString RESUMEGAME = "RESUME GAME";

				public static LocString LOADGAME = "LOAD GAME";

				public static LocString TRANSLATIONS = "TRANSLATIONS";

				public static LocString OPTIONS = "OPTIONS";

				public static LocString QUITTODESKTOP = "QUIT";

				public static LocString RESTARTCONFIRM = "Are you sure you want to quit?\nAll unsaved progress will be lost.";

				public static LocString QUITCONFIRM = "Are you sure you want to quit?\nAll unsaved progress will be lost.";

				public static LocString RESUMEBUTTON_BASENAME = "{0}: Cycle {1}";
			}

			public class PATCHNOTESSCREEN
			{
				public static LocString TITLE = "AGRICULTURAL UPGRADE";

				public static LocString BODY = "<b>Welcome to the Agricultural Upgrade!</b>\n\n{0}";

				public static LocString PATCHNOTES = "Update Features:\n\n• New items, buildings and recipes\n• Improved crop tending\n• New farming and Harvest Rating systems\n• New Consumables screen\n• New Duplicant Stress Responses\n• Additional energy source: Natural Gas\n• New power management tool: Transformer\n• Screenshot Mode: Alt-S\n• Duplicant Follow Cam";

				public static LocString OK_BUTTON = "OK";
			}

			public class LOADSCREEN
			{
				public static LocString TITLE = "LOAD GAME";

				public static LocString TITLE_INSPECT = "LOAD GAME";

				public static LocString DELETEBUTTON = "DELETE";

				public static LocString CONFIRMDELETE = "Are you sure you want to delete {0}?\nYou cannot undo this action.";

				public static LocString SAVEDETAILS = "<b>File:</b> {0}\n\n<b>Save Date:</b>\n{1}\n\n<b>Base Name:</b> {2}\n<b>Duplicants Alive:</b> {3}\n<b>Cycles Survived:</b> {4}";

				public static LocString AUTOSAVEWARNING = " <b><color=#ff0000>(Auto Save: This file will get deleted as new Auto Saves are created)</color></b>";

				public static LocString CORRUPTEDSAVE = "<b><color=#ff0000>Could not load file {0}. Its data may be corrupted.</color></b>";

				public static LocString SAVE_TOO_NEW = "<b><color=#ff0000>Could not load file {0}. File is using build {1}. This build is {2}.</color></b>";

				public static LocString UNSUPPORTED_SAVE_VERSION = "<b><color=#ff0000>This save file is from a previous version of the game and is no longer supported.</color></b>\n\nYou may revert to the previous build to play your old save. Please follow the link below for details.";

				public static LocString MORE_INFO = "More Info";
			}

			public class SAVESCREEN
			{
				public static LocString TITLE = "SAVE SLOTS";

				public static LocString NEWSAVEBUTTON = "New Save";

				public static LocString OVERWRITEMESSAGE = "Are you sure you want to overwrite {0}?";

				public static LocString SAVENAMETITLE = "SAVE NAME";

				public static LocString CONFIRMNAME = "Confirm";

				public static LocString CANCELNAME = "Cancel";
			}

			public class PAUSE_SCREEN
			{
				public static LocString TITLE = "PAUSED";

				public static LocString RESUME = "Resume";

				public static LocString LOGBOOK = "Logbook";

				public static LocString OPTIONS = "Options";

				public static LocString SAVE = "Save";

				public static LocString SAVEAS = "Save As";

				public static LocString LOAD = "Load";

				public static LocString QUIT = "Main Menu";

				public static LocString DESKTOPQUIT = "Quit to Desktop";
			}

			public class OPTIONS_SCREEN
			{
				public static LocString TITLE = "OPTIONS";

				public static LocString GRAPHICS = "Graphics";

				public static LocString AUDIO = "Audio";

				public static LocString CONTROLS = "Controls";

				public static LocString UNITS = "Temperature Units";

				public static LocString METRICS = "Send Metrics Data";

				public static LocString LANGUAGE = "Choose a different Language";

				public static LocString WORLD_GEN = "World Generation Key";

				public static LocString CREDITS = "Credits";

				public static LocString BACK = "Back";
			}

			public class INPUT_BINDINGS_SCREEN
			{
				public static LocString TITLE = "CUSTOMIZE KEYS";

				public static LocString RESET = "Reset";

				public static LocString APPLY = "Apply";

				public static LocString DUPLICATE = "{0} was already bound to {1} and is now unbound.";

				public static LocString UNBOUND_ACTION = "{0} is unbound. Are you sure you want to continue?";

				public static LocString MULTIPLE_UNBOUND_ACTIONS = "You have multiple unbound actions, this may result in difficulty playing the game. Are you sure you want to continue?";

				public static LocString WAITING_FOR_INPUT = "???";
			}

			public class TRANSLATIONS_SCREEN
			{
				public static LocString TITLE = "TRANSLATIONS";

				public static LocString UNINSTALL = "Uninstall";

				public static LocString PLEASE_RESTART = "This requires a restart, press ok to continue";

				public static LocString NO_PACKS = "Steam Workshop";

				public static LocString DOWNLOAD = "Start Download";

				public static LocString INSTALL = "Install";

				public static LocString INSTALLED = "Installed";

				public static LocString NO_STEAM = "Unable to get language list from Steam";
			}

			public class AUDIO_OPTIONS_SCREEN
			{
				public static LocString TITLE = "AUDIO OPTIONS";

				public static LocString DONE_BUTTON = "Done";

				public static LocString MUSIC_EVERY_CYCLE = "Start background music every morning";

				public static LocString MUSIC_EVERY_CYCLE_TOOLTIP = "If enabled, background music will play every cycle instead of every few cycles.";

				public static LocString AUDIO_BUS_MASTER = "Master";

				public static LocString AUDIO_BUS_SFX = "SFX";

				public static LocString AUDIO_BUS_MUSIC = "Music";

				public static LocString AUDIO_BUS_AMBIENCE = "Ambience";

				public static LocString AUDIO_BUS_UI = "UI";
			}

			public class WORLD_GEN_OPTIONS_SCREEN
			{
				public static LocString TITLE = "WORLD GENERATION OPTIONS";

				public static LocString ENABLE_BUTTON = "Set World Generation Seed";

				public static LocString DONE_BUTTON = "Done";

				public static LocString CHOOSE_RANDOM_BUTTON = "Use any seed";

				public static LocString TOOLTIP = "If set, this will override the world generation seed";
			}

			public class METRICS_OPTIONS_SCREEN
			{
				public static LocString TITLE = "METRICS OPTIONS";

				public static LocString ENABLE_BUTTON = "Send Metrics Data";

				public static LocString DONE_BUTTON = "Done";

				public static LocString TOOLTIP = "If enabled, metrics data will be sent for the purpose of improving the game.";
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

				public static LocString FULLSCREEN = "Fullscreen:";

				public static LocString RESOLUTION = "Resolution:";

				public static LocString APPLYBUTTON = "Apply";

				public static LocString REVERTBUTTON = "Revert";

				public static LocString DONE_BUTTON = "Done";

				public static LocString UI_SCALE = "UI Scale";

				public static LocString ACCEPT_CHANGES = "Accept Changes?";
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

				public static LocString SIMFAILEDTOLOAD = "A problem occurred loading Oxygen Not Included. This is usually caused by the Visual Studio C++ 2015 runtime being improperly installed on the system. Please exit the game, run Windows Update and try re-launching Oxygen Not Included.";

				public static LocString BODY = "We've detected that this computer does not meet the minimum requirements to run Oxygen Not Included. While you may continue with your current specs, the game might not run smoothly for you.\n\nPlease be aware that your experience may suffer as a result.";

				public static LocString OKBUTTON = "Okay, thanks!";

				public static LocString QUITBUTTON = "Quit";
			}

			public class AUDIODRIVERSCREEN
			{
				public static LocString WARNING = "A problem occurred initializing your audio device.\nSorry about that!\n\nThis is usually caused by outdated audio drivers.\n\nPlease visit your audio device manufacturer's website to download the latest drivers.";
			}
		}

		public class WORLDGEN
		{
			public static LocString NOHEADERS = string.Empty;

			public static LocString COMPLETE = "Success! Space adventure awaits.";

			public static LocString FAILED = "Goodness, has this ever gone terribly wrong!";

			public static LocString RESTARTING = "Rebooting...";

			public static LocString LOADING = "Loading world...";

			public static LocString GENERATINGWORLD = "The Galaxy Synthesizer";

			public static LocString CHOOSEWORLDSIZE = "Select the magnitude of your new galaxy.";

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

			public static LocString NORMALISENOISE = "Interpolating toxic gas...";

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
			public static LocString TOOLTIP_SEPERATOR = "\n----------\n";

			public static LocString MANAGEMENTMENU_JOBS = "Manage Duplicant job priorities";

			public static LocString MANAGEMENTMENU_CONSUMABLES = "Manage Duplicant diets";

			public static LocString MANAGEMENTMENU_VITALS = "View Duplicants' vitals";

			public static LocString MANAGEMENTMENU_RESEARCH = "View the Research Tree";

			public static LocString MANAGEMENTMENU_DAILYREPORT = "View daily Colony Reports";

			public static LocString MANAGEMENTMENU_SCHEDULE = "Adjust colony timetable";

			public static LocString MANAGEMENTMENU_REQUIRES_RESEARCH = "Build a Research Station to unlock\n------------------\nResearch buildings can be found in the Stations Tab <color=#F44A47>[9]</color> of the Build Menu";

			public static LocString METERSCREEN_AVGSTRESS = "Highest Stress: {0}";

			public static LocString METERSCREEN_MEALHISTORY = "Calories Available: {0}";

			public static LocString METERSCREEN_POPULATION = "Population: {0}";

			public static LocString PLAYBUTTON = "Start";

			public static LocString PAUSEBUTTON = "Pause";

			public static LocString PAUSE = "Pause";

			public static LocString UNPAUSE = "Unpause";

			public static LocString SPEEDBUTTON_SLOW = "Slow speed {0}";

			public static LocString SPEEDBUTTON_MEDIUM = "Medium speed {0}";

			public static LocString SPEEDBUTTON_FAST = "Fast speed {0}";

			public static LocString RED_ALERT_TITLE = "Toggle Red Alert";

			public static LocString RED_ALERT_CONTENT = "Colony enters a state of emergency. Duplicants ignore basic needs to work.";

			public static LocString MOPBUTTON = "Drag to wipe up liquid messes";

			public static LocString DIGBUTTON = "Drag to set dig tasks and excavate resources";

			public static LocString CANCELBUTTON = "Drag to cancel pending job tasks";

			public static LocString DECONSTRUCTBUTTON = "Drag to demolish buildings and refund their resources";

			public static LocString ATTACKBUTTON = "Drag to batch harvest mature plants";

			public static LocString CLEARBUTTON = "Drag to move debris off the floor and into storage";

			public static LocString HARVESTBUTTON = "Drag to batch harvest mature plants";

			public static LocString PRIORITIZEMAINBUTTON = "Drag to bump tasks up or down in importance";

			public static LocString PRIORITIZEBUTTON = "Drag to bump job tasks up or down in importance";

			public static LocString DEPRIORITIZEBUTTON = "Drag to deprioritize dig and build tasks";

			public static LocString TRASHREGIONBUTTON = "Drag to create storage regions for materials";

			public static LocString SELECTREGIONBUTTON = "Click to select existing storage regions";

			public static LocString ERASEREGIONBUTTON = "Drag to erase existing exosuit regions";

			public static LocString CLEANUPMAINBUTTON = "Mop and sweep messy floors";

			public static LocString STORAGEMAINBUTTON = "Create and manage exosuit regions";

			public static LocString CANCELDECONSTRUCTIONBUTTON = "Cancel queued orders or deconstruct existing buildings";

			public static LocString HELP_ROTATE_KEY = "Press <color=#F44A47>[O]</color> to Rotate";

			public static LocString HELP_BUILDLOCATION_FLOOR = "Must be built on solid ground";

			public static LocString HELP_BUILDLOCATION_OCCUPIED = "Must be built in unoccupied space";

			public static LocString HELP_BUILDLOCATION_CEILING = "Must be built on the ceiling";

			public static LocString HELP_BUILDLOCATION_INSIDEGROUND = "Must be built in the ground";

			public static LocString OXYGENOVERLAYSTRING = "Displays ambient oxygen density";

			public static LocString POWEROVERLAYSTRING = "Displays power grid components";

			public static LocString TEMPERATUREOVERLAYSTRING = "Displays ambient temperature";

			public static LocString HEATFLOWOVERLAYSTRING = "Displays areas with comfortable temperatures for Duplicants";

			public static LocString ROOMSOVERLAYSTRING = "Displays fully enclosed rooms";

			public static LocString LIGHTSOVERLAYSTRING = "Displays the visibility radius of light sources";

			public static LocString REGIONOVERLAYSTRING = "Displays areas requiring Duplicant safety gear";

			public static LocString LIQUIDVENTOVERLAYSTRING = "Displays liquid pipe system components";

			public static LocString GASVENTOVERLAYSTRING = "Displays gas pipe system components";

			public static LocString DECOROVERLAYSTRING = "Displays decor values";

			public static LocString PRIORITIESOVERLAYSTRING = "Displays task priority values";

			public static LocString REACHABILITYOVERLAYSTRING = "Displays areas accessible by Duplicants";

			public static LocString ENERGYREQUIRED = "<style=\"power\">Power</style> Required";

			public static LocString ENERGYGENERATED = "<style=\"power\">Power</style> Produced";

			public static LocString INFOPANEL = "The Info Panel contains an overview of the basic information about your Duplicant";

			public static LocString VITALSPANEL = "The Vitals Panel monitors the status and well being of your Duplicant";

			public static LocString STRESSPANEL = "The Stress Panel offers an in detail look at what is affecting your Duplicant psychologically";

			public static LocString STATSPANEL = "The Stats Panel gives an overview of your Duplicant's individual stats";

			public static LocString ITEMSPANEL = "The Items Panel displays everything your Duplicant is in possession of";

			public static LocString STRESSDESCRIPTION = "Accommodate your Duplicant's needs to manage their <style=\"stress\">Stress</style>.\n\nLow <style=\"stress\">Stress</style> can provide a productivity boost, while high <style=\"stress\">Stress</style> can impair production or even lead to a nervous breakdown.";

			public static LocString ALERTSTOOLTIP = "Alerts provide important information about what's happening to your colony right now";

			public static LocString MESSAGESTOOLTIP = "Messages are events that have happened and tips to help you manage your colony";

			public static LocString NEXTMESSAGESTOOLTIP = "Next message";

			public static LocString CLOSETOOLTIP = "Close";

			public static LocString DISMISSMESSAGE = "Dismiss message";

			public static LocString RECIPE_QUEUE = "Queue 1 {0} for fabrication";

			public static LocString RECIPE_QUEUE_INFINITE = "Continuously fabricate {0} if resources are available";

			public static LocString RED_ALERT_BUTTON_ON = "Enable Red Alert";

			public static LocString RED_ALERT_BUTTON_OFF = "Disable Red Alert";

			public static LocString JOBSSCREEN_PRIORITY = "High priority jobs are performed before low priority jobs. A Duplicant will always continue work on their current job until it is complete, even if a higher priority job becomes available.";

			public static LocString JOBSSCREEN_ATTRIBUTES = "The following attributes affect a Duplicant's efficiency at this job:";

			public static LocString JOBSSCREEN_CANNOTPERFORMTASK = "{0} cannot perform this task.";

			public static LocString JOBSSCREEN_RELEVANT_ATTRIBUTES = "Relevant Attributes:";

			public static LocString SORTCOLUMN = "Click to sort";

			public static LocString NOMATERIAL = "Not enough materials";

			public static LocString SELECTAMATERIAL = "There are insufficient materials to construct this building";

			public static LocString EDITNAME = "Give this Duplicant a new name";

			public static LocString RANDOMIZENAME = "Randomize this Duplicant's name";

			public static LocString BASE_VALUE = "Base Value";

			public static LocString MATIERIAL_MOD = "Made out of {0}";

			public class RESOURCECATEGORY
			{
				public static LocString EXPAND = "Click to expand";

				public static LocString COLLAPSE = "Click to collapse";

				public static LocString DESC = "Counts resources within Duplicant reach that are not allocated to construction";
			}
		}

		public class DEVELOPMENTBUILDS
		{
			public static LocString WATERMARK = "DEVELOPMENT BUILD: {0}";

			public static LocString FULL_PATCH_NOTES = "Full Patch Notes";

			public static LocString PREVIOUS_VERSION = "Previous Version";

			public class ALPHA
			{
				public class MESSAGES
				{
					public static LocString HEADER = "DEVELOPMENT BUILD";

					public static LocString BODY = "Stay up to date by joining our mailing list, or head on over to the forums and join the discussion.";

					public static LocString FORUMBUTTON = "FORUMS";

					public static LocString MAILINGLIST = "MAILING LIST";

					public static LocString PATCHNOTES = "PATCH NOTES";
				}

				public class LOADING
				{
					public static LocString TITLE = "<b>Welcome to Oxygen Not Included!</b>";

					public static LocString BODY = "This game is in the early stages of development which means you're likely to encounter strange, amusing, and occasionally just downright frustrating bugs.\n\nDuring this time Oxygen Not Included will be receiving regular updates to fix bugs, add features and introduce additional content, so if you encounter issues or just have suggestions to share, please let us know on our forums: <u>http://forums.kleientertainment.com</u>\n\nA special thanks to those who joined us during our time in Alpha. We value your feedback and thank you for joining us in the development process. We couldn't do this without you.\n\nEnjoy your time in deep space!\n\n- Klei";

					public static LocString CONTINUEBUTTON = "Okay, thanks for the heads up!";
				}

				public class HEALTHY_MESSAGE
				{
					public static LocString CONTINUEBUTTON = "Thanks!";
				}
			}
		}

		public class UNITSUFFIXES
		{
			public static LocString PERSECOND = "/s";

			public static LocString PERCYCLE = "/cycle";

			public static LocString UNITS = " units";

			public class MASS
			{
				public static LocString KILOGRAM = " Kg";

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

				public static LocString WATT = " W";

				public static LocString KILOWATT = " kW";
			}

			public class DISTANCE
			{
				public static LocString METER = " m";
			}
		}

		public class OVERLAYS
		{
			public class OXYGEN
			{
				public static LocString NAME = "OXYGEN OVERLAY";

				public static LocString BUTTON = "Oxygen Overlay";

				public static LocString LEGEND1 = "High Concentration";

				public static LocString LEGEND2 = "Sufficient Concentration";

				public static LocString LEGEND3 = "Low Concentration";

				public static LocString LEGEND4 = "Unbreathable";

				public static LocString LEGEND5 = "Slightly Toxic";

				public static LocString LEGEND6 = "Very Toxic";

				public class TOOLTIPS
				{
					public static LocString LEGEND1 = "Oxygen concentration is high and allows Duplicants to breathe";

					public static LocString LEGEND2 = "Oxygen concentration is sufficient and allows Duplicants to breathe";

					public static LocString LEGEND3 = "Oxygen concentration is low and is difficult for Duplicants to breathe";

					public static LocString LEGEND4 = "Oxygen is extremely low pressure or not present and will suffocate Duplicants";

					public static LocString LEGEND5 = "Harmful gas concentration";

					public static LocString LEGEND6 = "Lethal gas concentration";
				}
			}

			public class ELECTRICAL
			{
				public static LocString NAME = "POWER GRID OVERLAY";

				public static LocString BUTTON = "Power Grid Overlay";

				public static LocString LEGEND1 = "<b>BUILDING POWER</b>";

				public static LocString LEGEND2 = "Consumer";

				public static LocString LEGEND3 = "Producer";

				public static LocString LEGEND4 = "<b>CIRCUIT POWER LOAD</b>";

				public static LocString LEGEND5 = "Energy Surplus";

				public static LocString LEGEND6 = "Strained";

				public static LocString LEGEND7 = "Overloaded";

				public static LocString LEGEND8 = "Underpowered";

				public static LocString DIAGRAM_HEADER = "Energy from LEFT circuit is provided to RIGHT circuit";

				public static LocString LEGEND_SWITCH = "Switch";

				public class TOOLTIPS
				{
					public static LocString LEGEND1 = "Displays whether buildings use or generate <style=\"power\">Power</style>";

					public static LocString LEGEND2 = "Uses <style=\"power\">Power</style> from a circuit";

					public static LocString LEGEND3 = "Generates <style=\"power\">Power</style> for a circuit";

					public static LocString LEGEND4 = "Displays the <style=\"power\">Power</style> load of wiring";

					public static LocString LEGEND5 = "This circuit is producing more <style=\"power\">Power</style> than it can consume";

					public static LocString LEGEND6 = "This circuit is consuming nearly all the <style=\"power\">Power</style> it produces";

					public static LocString LEGEND7 = "Too much <style=\"power\">Power</style> being drawn from system";

					public static LocString LEGEND8 = "This circuit is consuming more <style=\"power\">Power</style> than it can produce";

					public static LocString LEGEND_SWITCH = "Activates or deactivates a circuit";
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

			public class HEATFLOW
			{
				public static LocString NAME = "THERMAL COMFORT OVERLAY";

				public static LocString HOVERTITLE = "THERMAL COMFORT";

				public static LocString BUTTON = "Thermal Comfort Overlay";

				public static LocString COOLING = "Body Heat Loss";

				public static LocString NEUTRAL = "Comfort Zone";

				public static LocString HEATING = "Body Heat Retention";

				public class TOOLTIPS
				{
					public static LocString COOLING = "Uncomfortable Area\n------------------\nDuplicants give off more heat than they can absorb from this environment, making them too cold";

					public static LocString NEUTRAL = "Comfortable Area\n------------------\nDuplicants can regulate their internal temperatures in this environment";

					public static LocString HEATING = "Uncomfortable Area\n------------------\nDuplicants absorb more heat from this environment than they can give off, making them too hot";
				}
			}

			public class ROOMS
			{
				public static LocString NAME = "ROOM OVERLAY";

				public static LocString BUTTON = "Room Overlay";

				public static LocString ROOM = "Room {0}";

				public static LocString NOROOMS = "No rooms created";

				public class TOOLTIPS
				{
					public static LocString ROOM = "Completed Duplicant bedrooms";

					public static LocString NOROOMS = "Duplicants have nowhere to sleep";
				}
			}

			public class LIGHTING
			{
				public static LocString NAME = "LIGHT OVERLAY";

				public static LocString BUTTON = "Light Overlay";

				public static LocString LITAREA = "Lit area";

				public static LocString DARK = "Unlit area";

				public class TOOLTIPS
				{
					public static LocString NAME = "LIGHT OVERLAY";

					public static LocString LITAREA = "Duplicants have adequate lighting in these areas";

					public static LocString DARK = "Duplicants cannot see in these areas";
				}
			}

			public class STORAGEREGIONS
			{
				public static LocString NAME = "EXOSUIT OVERLAY";

				public static LocString BUTTON = "Exosuit Overlay";

				public static LocString REGION = "Exosuit Region {0}";

				public static LocString NOREGIONS = "No exosuit regions created";

				public class TOOLTIPS
				{
					public static LocString REGION = "{0} is accepting materials";

					public static LocString NOREGIONS = "No storage areas have been designated to receive materials";
				}
			}

			public class LIQUIDPLUMBING
			{
				public static LocString NAME = "LIQUID PLUMBING OVERLAY";

				public static LocString BUTTON = "Liquid Plumbing Overlay";

				public static LocString CONSUMER = "Output";

				public static LocString FILTERED = "Filtered Output";

				public static LocString PRODUCER = "Intake";

				public static LocString CONNECTED = "Connected";

				public static LocString DISCONNECTED = "Disconnected";

				public static LocString NETWORK = "Liquid Network {0}";

				public static LocString DIAGRAM_BEFORE_ARROW = "Liquid flows from OUTPUT";

				public static LocString DIAGRAM_AFTER_ARROW = "INTAKE via pipes";

				public class TOOLTIPS
				{
					public static LocString CONNECTED = "Connected to a <style=\"LiquidPiping\">Liquid Pipe</style>";

					public static LocString DISCONNECTED = "Not connected to a <style=\"LiquidPiping\">Liquid Pipe</style>";

					public static LocString CONSUMER = "Sends liquid into pipes from a building\n* Must connect to at least one <style=\"LiquidDestination\">Intake</style>";

					public static LocString FILTERED = "Sends one filtered liquid into pipes from a building\n* Must connect to at least one <style=\"LiquidDestination\">Intake</style>";

					public static LocString PRODUCER = "Sends liquid into a building from pipes\n* Must connect to at least one <style=\"LiquidSource\">Output</style>";

					public static LocString NETWORK = "Liquid network {0}";
				}
			}

			public class GASPLUMBING
			{
				public static LocString NAME = "GAS PLUMBING OVERLAY";

				public static LocString BUTTON = "Gas Plumbing Overlay";

				public static LocString CONSUMER = "Output";

				public static LocString FILTERED = "Filtered Output";

				public static LocString PRODUCER = "Intake";

				public static LocString CONNECTED = "Connected";

				public static LocString DISCONNECTED = "Disconnected";

				public static LocString NETWORK = "Gas Network {0}";

				public static LocString DIAGRAM_BEFORE_ARROW = "Gas flows from OUTPUT";

				public static LocString DIAGRAM_AFTER_ARROW = "INTAKE via pipes";

				public class TOOLTIPS
				{
					public static LocString CONNECTED = "Connected to a <style=\"GasPiping\">Gas Pipe</style>";

					public static LocString DISCONNECTED = "Not connected to a <style=\"GasPiping\">Gas Pipe</style>";

					public static LocString CONSUMER = "Sends gas into pipes from a building\n* Must connect to at least one <style=\"GasDestination\">Intake</style>";

					public static LocString FILTERED = "Sends one filtered gas into pipes from a building\n* Must connect to at least one <style=\"GasDestination\">Intake</style>";

					public static LocString PRODUCER = "Sends gas into a building from pipes\n* Must connect to at least one <style=\"GasSource\">Output</style>";

					public static LocString NETWORK = "Gas network {0}";
				}
			}

			public class EXOSUIT
			{
				public static LocString NAME = "EXOSUIT OVERLAY";

				public static LocString BUTTON = "Exosuit Overlay";

				public static LocString EXOSUITREQUIRED = "Exosuit required";

				public static LocString EXOSUITNOTREQUIRED = "Exosuit not required";

				public class TOOLTIPS
				{
					public static LocString EXOSUITREQUIRED = "Duplicants must wear an <style=\"equipment\">Exosuit</style> to survive in these areas";

					public static LocString EXOSUITNOTREQUIRED = "Duplicants can safely enter these areas unprotected";
				}
			}

			public class DECOR
			{
				public static LocString NAME = "DECOR OVERLAY";

				public static LocString BUTTON = "Decor Overlay";

				public static LocString TOTAL = "Total";

				public static LocString LOWDECOR = "Low Decor";

				public static LocString HIGHDECOR = "High Decor";

				public static LocString CLUTTER = "Debris";

				public static LocString LIGHTING = "Lighting";

				public static LocString CLOTHING = "{0}'s Clothing";

				public static LocString HOVERTITLE = "DECOR";

				public class TOOLTIPS
				{
					public static LocString LOWDECOR = "Areas with insufficient decor values\n------------------\nResources on the floor are considered \"debris\" and will decrease decor";

					public static LocString HIGHDECOR = "Areas with sufficient decor values\n------------------\nLighting and aesthetically pleasing buildings increase decor";
				}
			}

			public class PRIORITIES
			{
				public static LocString NAME = "PRIORITY OVERLAY";

				public static LocString BUTTON = "Priority Overlay";

				public static LocString ONE = "Priority 1 (Lowest)";

				public static LocString ONE_TOOLTIP = "Priority 1";

				public static LocString TWO = "Priority 2";

				public static LocString TWO_TOOLTIP = "Priority 2";

				public static LocString THREE = "Priority 3";

				public static LocString THREE_TOOLTIP = "Priority 3";

				public static LocString FOUR = "Priority 4";

				public static LocString FOUR_TOOLTIP = "Priority 4";

				public static LocString FIVE = "Priority 5";

				public static LocString FIVE_TOOLTIP = "Priority 5";

				public static LocString SIX = "Priority 6";

				public static LocString SIX_TOOLTIP = "Priority 6";

				public static LocString SEVEN = "Priority 7";

				public static LocString SEVEN_TOOLTIP = "Priority 7";

				public static LocString EIGHT = "Priority 8";

				public static LocString EIGHT_TOOLTIP = "Priority 8";

				public static LocString NINE = "Priority 9 (Highest)";

				public static LocString NINE_TOOLTIP = "Priority 9";
			}

			public class POWER
			{
				public static LocString WATTS_GENERATED = "Watts Generated";

				public static LocString WATTS_CONSUMED = "Watts Consumed";
			}
		}

		public class TABLESCREENS
		{
			public static LocString COLUMN_SORT_BY_NAME = "Sort by name";

			public static LocString COLUMN_SORT_BY_STRESS = "Sort by stress level";

			public static LocString COLUMN_SORT_BY_EXPECTATIONS = "Sort by quality expectations";
		}

		public class CONSUMABLESSCREEN
		{
			public static LocString TITLE = "CONSUMABLES";

			public static LocString TOOLTIP_TOGGLE_ALL = "Toggle all food permissions colonywide";

			public static LocString TOOLTIP_TOGGLE_COLUMN = "Toggle colonywide <b>{0}</b> permission";

			public static LocString TOOLTIP_TOGGLE_ROW = "Toggle all food permissions for <b>{0}</b>";

			public static LocString NEW_MINIONS_TOOLTIP_TOGGLE_ROW = "Toggle all food permissions for <b>new Duplicants</b>";

			public static LocString NEW_MINIONS_FOOD_PERMISSION_ON = "<b>New Duplicants</b> will eat \n<b>{0}</b> by default";

			public static LocString NEW_MINIONS_FOOD_PERMISSION_OFF = "<b>New Duplicants</b> will not eat \n<b>{0}</b> by default";

			public static LocString FOOD_PERMISSION_ON = "<b>{0}</b> will eat\n<b>{1}</b>";

			public static LocString FOOD_PERMISSION_OFF = "<b>{0}</b> will not eat\n<b>{1}</b>";

			public static LocString FOOD_CANT_CONSUME = "<b>{0}</b> cannot eat\n<b>{1}</b>";

			public static LocString FOOD_REFUSE = "<b>{0}</b> refuses to eat\n<b>{1}</b>";

			public static LocString FOOD_AVAILABLE = "Available: {0}";

			public static LocString FOOD_QUALITY = "Quality: {0}";

			public static LocString FOOD_EXPECTATIONS = "Expectations";

			public static LocString STRESS = "Stress";
		}

		public class JOBSSCREEN
		{
			public static LocString TITLE = "JOBS";

			public static LocString TOOLTIP_TOGGLE_ALL = "Toggle all tasks colonywide";

			public static LocString TOOLTIP_TOGGLE_COLUMN = "Toggle <b>{0} Tasks</b> colonywide";

			public static LocString TOOLTIP_TOGGLE_ROW = "Toggle all tasks for <b>{0}</b>";

			public static LocString COLUMN_SORT_BY_JOB_SKILL = "Sort colony by {0} skill";

			public static LocString NEW_MINIONS_TOOLTIP_TOGGLE_ROW = "Toggle all tasks for <b>new Duplicants</b>";

			public static LocString NEW_MINIONS_JOB_PERMISSION_ON = "<b>New Duplicants</b> will perform \n<b>{0} Tasks</b> by default";

			public static LocString NEW_MINIONS_JOB_PERMISSION_OFF = "<b>New Duplicants</b> will not perform \n<b>{0} Tasks</b> by default";

			public static LocString JOB_PERMISSION_ON = "<b>{0}</b> will perform \n<b>{1} Tasks</b>";

			public static LocString JOB_PERMISSION_OFF = "<b>{0}</b> will not perform \n<b>{1} Tasks</b>";

			public static LocString JOB_CANT_CONSUME = "<b>{0}</b> cannot perform \n<b>{1} Tasks</b>";

			public static LocString JOB_REFUSE = "<b>{0}</b> refuses to perform \n<b>{1} Tasks</b>";
		}

		public class SCHEDULESCREEN
		{
			public static LocString SCHEDULE = "SCHEDULE:";

			public static LocString BLOCKS = "BLOCKS:";

			public static LocString SELECTBLOCKTYPETOOLTIP = "Select a block type to adjust the colony's schedule";

			public static LocString CLICKTOSELECT = "Click to select.";

			public static LocString CLICKTOCHANGE = "Click to change block type.";

			public static LocString TIMESLOT = "Time Slot";

			public static LocString SELECTHELP = "Select an option and then click or drag on the timetable to adjust the Duplicant's schedule.";

			public static LocString ALLOWED = "Allowed";

			public static LocString DENIED = "Denied";
		}

		public class COLONYLOSTSCREEN
		{
			public static LocString COLONYLOST = "COLONY LOST";

			public static LocString COLONYLOSTDESCRIPTION = "All Duplicants are dead or incapacitated.";

			public static LocString RESTARTPROMPT = "Press <color=#F44A47>[ESC]</color> to return to a previous colony, or begin a new one.";

			public static LocString DISMISSBUTTON = "DISMISS";

			public static LocString QUITBUTTON = "MAIN MENU";
		}

		public class CRASHSCREEN
		{
			public static LocString TITLE = "Whoops! We're sorry, but it seems your game has encountered an error.\nIt's okay though - these errors are how we find and fix problems to make our game more fun for everyone.\nIf you use the box below to submit a crash report to us, we can use this information to get the issue sorted out.";

			public static LocString HEADER = "OPTIONAL CRASH DESCRIPTION";

			public static LocString BODY = "Help! A black hole ate my game!";

			public static LocString THANKYOU = "Thank you!\n\nYou're making our game better, one crash at a time.";

			public static LocString UPLOADINFO = "UPLOAD ADDITIONAL INFO ({0})";

			public static LocString REPORTBUTTON = "REPORT CRASH";

			public static LocString REPORTING = "REPORTING, PLEASE WAIT...";

			public static LocString CONTINUEBUTTON = "CONTINUE GAME";

			public static LocString QUITBUTTON = "QUIT TO DESKTOP";

			public static LocString SAVEFAILED = "Save Failed: {0}";

			public static LocString LOADFAILED = "Load Failed: {0}\nSave Version: {1}\nExpected: {2}";
		}

		public class DEMOOVERSCREEN
		{
			public static LocString TIMEREMAINING = "Demo time remaining:";

			public static LocString TIMERTOOLTIP = "Demo time remaining";

			public static LocString TIMERINACTIVE = "Timer inactive";

			public static LocString DEMOOVER = "END OF DEMO";

			public static LocString DESCRIPTION = "Thank you for playing <color=#F44A47>Oxygen Not Included</color>!";

			public static LocString DESCRIPTION_2 = string.Empty;

			public static LocString QUITBUTTON = "RESET";
		}

		public class CONTEXTSCREEN
		{
			public static LocString ASSIGNEDTO = "Assigned To:";

			public static LocString UNAVAILABLE = "UNAVAILABLE";

			public class PRECONDITIONS
			{
				public static LocString CANCHAT = "Can't reach";

				public static LocString ISNOTREDALERT = "Red alert";

				public static LocString CANMOVETO = "Can't reach";

				public static LocString CANPICKUP = "Can't reach";

				public static LocString ISOPERATIONAL = "Not operational";

				public static LocString ISMARKEDFORDECONSTRUCTION = "Not marked for destruction";

				public static LocString ISFUNCTIONAL = "Not operational";

				public static LocString CANCURE = "Can cure";

				public static LocString ISTANKLOW = "Tank not low";

				public static LocString OWNSMATERIALS = "No resources available";

				public static LocString ISFETCHTARGETAVAILABLE = "No fetchable items available";

				public static LocString ISPERMITTED = "Job not allowed";
			}
		}

		public class CREDITSSCREEN
		{
			public static LocString TITLE = "CREDITS";

			public static LocString CLOSEBUTTON = "CLOSE";

			public class THIRD_PARTY
			{
				public static LocString FMOD = "FMOD Sound System\nCopyright Firelight Technologies";
			}
		}

		public class PRIORITYSCREEN
		{
			public static LocString BUILDMENUPRIORITYTOOLTIP = "Set construction priority level\n------------------\nPriority 1: Lowest\nPriority 9: Highest";

			public static LocString TOOLPRIORITYTOOLTIP = "Set priority level for the currently selected tool\n------------------\nPriority 1: Lowest\nPriority 9: Highest";

			public static LocString USERMENUPRIORITYTOOLTIP = "Set priority level for the currently selected object\n------------------\nPriority 1: Lowest\nPriority 9: Highest";
		}

		public class RESOURCESCREEN
		{
			public static LocString CATEGORY_TOOLTIP = "Click to expand\n------------------\nCounts resources within Duplicant reach that are not allocated to construction";

			public static LocString AVAILABLE_TOOLTIP = "Available: {0}";
		}

		public class CONFIRMDIALOG
		{
			public static LocString OK = "OK";

			public static LocString CANCEL = "CANCEL";
		}

		public class FILE_NAME_DIALOG
		{
			public static LocString ENTER_TEXT = "Enter Text...";
		}

		public class UISIDESCREENS
		{
			public class TREEFILTERABLESIDESCREEN
			{
				public static LocString TITLE = "Element Filter";

				public static LocString ALLBUTTON = "All";

				public static LocString ALLBUTTONTOOLTIP = "Allow storage of all resource categories in this container";

				public static LocString CATEGORYBUTTONTOOLTIP = "Allow storage of anything in the {0} resource category";

				public static LocString MATERIALBUTTONTOOLTIP = "Add or remove this material from storage";

				public static LocString ONLYALLOWTRANSPORTITEMSBUTTON = "Sweep Only";

				public static LocString ONLYALLOWTRANSPORTITEMSBUTTONTOOLTIP = "Only store objects marked Sweep <color=#F44A47>[K]</color> in this container";
			}

			public class FILTERSIDESCREEN
			{
				public static LocString TITLE = "Filter Outputs";

				public static LocString SELECTELEMENTHEADER = "Select Element";

				public static LocString NOELEMENTSELECTED = "No element selected";

				public static class UNFILTEREDELEMENTS
				{
					public static LocString GAS = "Gas Output:\nAll";

					public static LocString LIQUID = "Liquid Output:\nAll";
				}

				public static class FILTEREDELEMENT
				{
					public static LocString GAS = "Filtered Gas Output:\n{0}";

					public static LocString LIQUID = "Filtered Liquid Output:\n{0}";
				}
			}

			public class FABRICATORSIDESCREEN
			{
				public static LocString TITLE = "{0} Recipes";

				public static LocString NORECIPESELECTED = "No recipe selected";

				public static LocString SELECTRECIPE = "Select a recipe to fabricate.";

				public static LocString COST = "<b>Ingredients:</b>\n";

				public static LocString RESULTEFFECTS = "<b>Effects:</b>";

				public static LocString KG = "- {0}: {1}\n";

				public static LocString INFORMATION = "INFORMATION";

				public static LocString CANCEL = "Cancel";

				public static LocString RECIPERQUIREMENT = "<style={0}>{1}</style>: <style=\"consumed\">{2}</style> / {3}";

				public static LocString UNITS_AND_CALS = "{0} ({1})";

				public static LocString CALS = "{0}";

				public static LocString QUEUED_MISSING_INGREDIENTS_TOOLTIP = "Missing {0} of {1}\n";

				public class TOOLTIPS
				{
					public static LocString RECIPERQUIREMENT_SUFFICIENT = "This recipe consumes {1} of an available {2} of {0}";

					public static LocString RECIPERQUIREMENT_INSUFFICIENT = "This recipe requires {1} {0}\nAvailable: {2}";
				}

				public class EFFECTS
				{
					public static LocString OXYGEN_TANK = "<style=\"equipment\">Oxygen Tank</style> (<style=\"produced\">{0}</style>)";

					public static LocString OXYGEN_TANK_UNDERWATER = "<style=\"equipment\">Oxygen Rebreather</style> (<style=\"produced\">{0}</style>)";

					public static LocString COOL_VEST = "<style=\"equipment\">Cool Vest</style> (<style=\"produced\">{0}</style>)";

					public static LocString WARM_VEST = "<style=\"equipment\">Warm Sweater</style> (<style=\"produced\">{0}</style>)";

					public static LocString FUNKY_VEST = "<style=\"equipment\">Snazzy Suit</style> (<style=\"produced\">{0}</style>)";

					public static LocString RESEARCHPOINT = "<style=\"research\">{0}</style>: <style=\"produced\">+1</style>";
				}
			}

			public class PLANTERSIDESCREEN
			{
				public static LocString PLANTREQUIREMENTS = "<b>Growth Requirements</b>:";

				public static LocString HARVESTDETAILS = "<b>Harvest Rating Tiers</b>:";

				public static LocString OPTIMUMCONDITIONS = "<style=\"harvestQuality\">Harvest Rating</style>: Max <style=\"produced\">+{0} Points</style>/cycle:";

				public static LocString PLANTEFFECTS = "<b>Effects</b>:";

				public static LocString NUMBEROFHARVESTS = "Harvests: {0}";

				public static LocString YIELD = "{1} <style=\"food\">{0}</style>";

				public static LocString YIELD_SINGLE = "<style=\"food\">{0}</style>";

				public static LocString YIELDPERHARVEST = "{0} <style=\"food\">{1}</style> per harvest";

				public static LocString TOTALHARVESTCALORIESWITHPERUNIT = "<style=\"produced\">{0}</style> (<style=\"produced\">{1}</style> per unit)";

				public static LocString TOTALHARVESTCALORIES = "<style=\"produced\">{0}</style>";

				public static LocString YIELD_SEED = "{1} <style=\"seed\">{0}</style>";

				public static LocString YIELD_SEED_SINGLE = "<style=\"seed\">{0}</style>";

				public static LocString YIELD_SEED_FINAL_HARVEST = "{1} <style=\"seed\">{0}</style> (Final harvest only)";

				public static LocString YIELD_SEED_SINGLE_FINAL_HARVEST = "<style=\"seed\">{0}</style> (Final harvest only)";

				public static LocString LOW_YIELD = "Standard (0+ Points):";

				public static LocString NORMAL_YIELD = "Good (40+ Points):";

				public static LocString HIGH_YIELD = "Excellent (80+ Points):";

				public static LocString SELECTSEED_DESC = "Select a seed to plant.";

				public static LocString SELECTSEED_TITLE = "Select seed";

				public static LocString ROTATION_NEED_FLOOR = "<style=\"consumed\"><b>Requires upward plot orientation.</b></style>";

				public static LocString ROTATION_NEED_WALL = "<style=\"consumed\"><b>Requires sideways plot orientation.</b></style>";

				public static LocString ROTATION_NEED_CEILING = "<style=\"consumed\"><b>Requires downward plot orientation.</b></style>";

				public class TOOLTIPS
				{
					public static LocString PLANTREQUIREMENTS = "Lifecycle and growth requirements";

					public static LocString PLANTEFFECTS = "Conditions for Harvest Rating Point generation and additional effects";

					public static LocString YIELD = "<style=\"produced\">{2}</style> produced in this harvest tier (<style=\"produced\">{1}</style> per unit)";

					public static LocString LOW_YIELD = "Standard Harvest Tier\n------------------\nPlants produce Standard yields if they mature with 40 Harvest Rating Points or less.";

					public static LocString NORMAL_YIELD = "Good Harvest Tier\n------------------\nPlants produce Good yields if they mature with {0} or more Harvest Rating Points.\nAchieving this tier will yield more materials.";

					public static LocString HIGH_YIELD = "Excellent Harvest Tier\n------------------\nPlants produce Excellent yields if they mature with {0} or more Harvest Rating Points.\nAchieving this tier will yield the maximum number of materials this plant can produce.";

					public static LocString HARVESTDETAILS = "Complete yield bonuses for each Harvest Rating Tier";

					public static LocString OPTIMUMCONDITIONS = "Plants continuously generate Harvest Rating Points while in ideal conditions.\n------------------\nThis plant can generate up to {1} points per cycle for each satisfied condition.\nIt can earn a maximum of {0} points per cycle.";

					public static LocString NUMBEROFHARVESTS = "This plant can mature {0} times before the end of its lifecycle.";

					public static LocString YIELD_SEED = "Can be sown to grow more of this plant";

					public static LocString YIELD_SEED_FINAL_HARVEST = "{0}\n\nThese seeds will be produced in the final harvest of the plant's lifecycle";
				}
			}

			public class SINGLEENTITYRECEPTACLE
			{
				public static LocString TITLE = "{0} Seeds";

				public static LocString INFORMATION = "INFORMATION";

				public static LocString AWAITINGREQUEST = "PLANT: {0}";

				public static LocString AWAITINGDELIVERY = "AWAITING DELIVERY: {0}";

				public static LocString AWAITINGREMOVAL = "AWAITING DIGGING UP: {0}";

				public static LocString ENTITYDEPOSITED = "PLANTED: {0}";
			}

			public class RESEARCHSIDESCREEN
			{
				public static LocString TITLE = "Select Research";

				public static LocString CURRENTLYRESEARCHING = "Currently Researching";

				public static LocString NOSELECTEDRESEARCH = "No Research selected";

				public static LocString OPENRESEARCHBUTTON = "RESEARCH";
			}

			public class ACCESS_CONTROL_SIDE_SCREEN
			{
				public static LocString TITLE = "Door Access Control";

				public static LocString DOOR_DEFAULT = "Default";

				public static LocString MINION_ACCESS = "Duplicant Access Permissions";

				public static LocString GO_LEFT_ENABLED = "Passing Left through this door is permitted\n\nClick to revoke permission";

				public static LocString GO_LEFT_DISABLED = "Passing Left through this door is not permitted\n\nClick to grant permission";

				public static LocString GO_RIGHT_ENABLED = "Passing Right through this door is permitted\n\nClick to revoke permission";

				public static LocString GO_RIGHT_DISABLED = "Passing Right through this door is not permitted\n\nClick to grant permission";

				public static LocString SET_TO_DEFAULT = "Click to clear custom permissions";

				public static LocString SET_TO_CUSTOM = "Click to assign custom permissions";

				public static LocString USING_DEFAULT = "Default Access";

				public static LocString USING_CUSTOM = "Custom Access";
			}

			public class ASSIGNABLESIDESCREEN
			{
				public static LocString TITLE = "Assign {0}";

				public static LocString NEEDSREGION = "This {0} needs to be inside a {1} region";

				public static LocString NEEDSREGIONS = "This {0} needs to be inside one of the following regions: {1}";

				public static LocString ASSIGNEDTO = "Assigned to: {0}";

				public static LocString UNASSIGNED = "Unassigned";

				public static LocString GENERAL_CURRENTASSIGNED = "(Owner)";

				public static LocString SORT_BY_DUPLICANT = "Duplicant";

				public static LocString SORT_BY_ASSIGNMENT = "Assignment";
			}

			public class EQUIPPABLESIDESCREEN
			{
				public static LocString TITLE = "Equip {0}";

				public static LocString ASSIGNEDTO = "Assigned to: {0}";

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
				public static LocString TITLE = "Duplicant Printing";

				public static LocString NEXTPRODUCTION = "Next Production: {0}";

				public static LocString GAMEOVER = "Colony Lost";
			}

			public class VALVESIDESCREEN
			{
				public static LocString TITLE = "Flow Control";
			}

			public class MANUALGENERATORSIDESCREEN
			{
				public static LocString TITLE = "Battery Recharge Threshold";

				public static LocString CURRENT_THRESHOLD = "Current Threshold: {0}%";

				public static LocString TOOLTIP = "Duplicants will operate this generator when battery charge falls below the selected percentage";
			}

			public class TIMEDSWITCHSIDESCREEN
			{
				public static LocString TITLE = "Time Schedule";

				public static LocString ONTIME = "On Time:";

				public static LocString OFFTIME = "Off Time:";

				public static LocString TIMETODEACTIVATE = "Time until deactivation: {0}";

				public static LocString TIMETOACTIVATE = "Time until activation: {0}";

				public static LocString WARNING = "Switch must be connected to a <style=\"power\">Power</style> grid";

				public static LocString CURRENTSTATE = "Current State:";

				public static LocString ON = "On";

				public static LocString OFF = "Off";
			}

			public class TEMPERATURESWITCHSIDESCREEN
			{
				public static LocString TITLE = "Temperature Threshold";

				public static LocString CURRENT_TEMPERATURE = "Current Temperature:\n{0}";

				public static LocString ACTIVATE_IF = "Activate if:";

				public static LocString COLDER_BUTTON = "Below";

				public static LocString WARMER_BUTTON = "Above";
			}

			public class THRESHOLD_SWITCH_SIDESCREEN
			{
				public static LocString TITLE = "Pressure Threshold";

				public static LocString CURRENT_VALUE = "Current {0}:\n{1}";

				public static LocString ACTIVATE_IF = "Activate if:";

				public static LocString ABOVE_BUTTON = "Above";

				public static LocString BELOW_BUTTON = "Below";

				public static LocString STATUS_ACTIVE = "Switch Active";

				public static LocString STATUS_INACTIVE = "Switch Inactive";

				public static LocString PRESSURE = "Pressure";

				public static LocString PRESSURE_TOOLTIP_ABOVE = "Switch will be on if the pressure is above {0}";

				public static LocString PRESSURE_TOOLTIP_BELOW = "Switch will be off if the pressure is below {0}";

				public static LocString TEMPERATURE = "Temperature";

				public static LocString TEMPERATURE_TOOLTIP_ABOVE = "Switch will be on if the ambient temperature is above {0}";

				public static LocString TEMPERATURE_TOOLTIP_BELOW = "Switch will be off if the ambient temperature is below {0}";
			}

			public class DOOR_TOGGLE_SIDE_SCREEN
			{
				public static LocString TITLE = "Door Toggle";

				public static LocString OPEN = "Door is open.";

				public static LocString AUTO = "Door is on auto.";

				public static LocString CLOSE = "Door is locked.";

				public static LocString PENDING_FORMAT = "{0} {1}";

				public static LocString OPEN_PENDING = "Awaiting Duplicant to open door.";

				public static LocString AUTO_PENDING = "Awaiting Duplicant to automate door.";

				public static LocString CLOSE_PENDING = "Awaiting Duplicant to lock door.";

				public static LocString ACCESS_FORMAT = "{0}\n\n{1}";

				public static LocString ACCESS_OFFLINE = "Emergency Access Permissions:\nAll Duplicants are permitted to use this door until <style=\"power\">Power</style> is restored.";
			}

			public class ACTIVATION_RANGE_SIDE_SCREEN
			{
				public static LocString NAME = "Breaktime Policy";

				public static LocString ACTIVATE = "Max Stress:";

				public static LocString DEACTIVATE = "Min Stress:";
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

			public class OVERRIDETASK
			{
				public static LocString NAME = "Override Task";

				public static LocString TOOLTIP = "Force this Duplicant to perform a specific task";
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

				public static LocString TOOLTIP = "Duplicants will automatically harvest this plant when it matures";
			}

			public class CANCEL_HARVEST_WHEN_READY
			{
				public static LocString NAME = "Disable Autoharvest";

				public static LocString TOOLTIP = "Duplicants will require orders to harvest from this plant";
			}

			public class HARVEST
			{
				public static LocString NAME = "Harvest";

				public static LocString TOOLTIP = "Harvest materials from this plant";

				public static LocString TOOLTIP_DISABLED = "This plant has nothing to harvest yet";
			}

			public class CANCELHARVEST
			{
				public static LocString NAME = "Cancel Harvest";

				public static LocString TOOLTIP = "Cancel this harvest order";
			}

			public class ATTACK
			{
				public static LocString NAME = "Attack";

				public static LocString TOOLTIP = "Attack this creature";
			}

			public class CANCELATTACK
			{
				public static LocString NAME = "Cancel Attack";

				public static LocString TOOLTIP = "Cancel this attack order";
			}

			public class RELEASEELEMENT
			{
				public static LocString NAME = "Empty Building";

				public static LocString TOOLTIP = "Refund all resources currently in use by this building";
			}

			public class DEMOLISH
			{
				public static LocString NAME = "Deconstruct";

				public static LocString TOOLTIP = "Demolish this building and refund all resources";

				public static LocString NAME_OFF = "Cancel Deconstruct";

				public static LocString TOOLTIP_OFF = "Cancel this deconstruct order";
			}

			public class SELECTRESEARCH
			{
				public static LocString NAME = "Select Research";

				public static LocString TOOLTIP = "Choose a technology from the Research Tree <color=#F44A47>[R]</color>";
			}

			public class RELOCATE
			{
				public static LocString NAME = "Relocate";

				public static LocString TOOLTIP = "Move this building to a new location\n------------------\nCosts no additional resources";

				public static LocString NAME_OFF = "Cancel Relocation";

				public static LocString TOOLTIP_OFF = "Cancel this relocation order";
			}

			public class ENABLEBUILDING
			{
				public static LocString NAME = "Disable Building";

				public static LocString TOOLTIP = "Halt the use of this building\n------------------\nDisabled buildings consume no energy or resources";

				public static LocString NAME_OFF = "Enable Building";

				public static LocString TOOLTIP_OFF = "Resume the use of this building";
			}

			public class EMPTYSTORAGE
			{
				public static LocString NAME = "Empty Storage";

				public static LocString TOOLTIP = "Eject all resources from this storage region";

				public static LocString NAME_OFF = "Cancel Empty";

				public static LocString TOOLTIP_OFF = "Cancel this empty order";
			}

			public class COPY_BUILDING_SETTINGS
			{
				public static LocString NAME = "Copy Settings";

				public static LocString TOOLTIP = "Apply the settings and priorities of this building to others of the same type";
			}

			public class CLEAR
			{
				public static LocString NAME = "Sweep";

				public static LocString TOOLTIP = "Put this object away in the nearest storage container";

				public static LocString NAME_OFF = "Cancel Sweeping";

				public static LocString TOOLTIP_OFF = "Cancel this sweep order";
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
		}

		public class BUILDCATEGORIES
		{
			public static class BASE
			{
				public static LocString NAME = "Base";

				public static LocString TOOLTIP = "Maintain your colony's infrastructure with these homebase basics.";
			}

			public static class OXYGEN
			{
				public static LocString NAME = "Oxygen";

				public static LocString TOOLTIP = "Everything you need to keep your colony breathing.";
			}

			public static class POWER
			{
				public static LocString NAME = "Power";

				public static LocString TOOLTIP = "Need to power your colony? Here's how you do it!";
			}

			public static class FOOD
			{
				public static LocString NAME = "Food";

				public static LocString TOOLTIP = "Keep your Duplicants' spirits high and their bellies full.";
			}

			public static class UTILITIES
			{
				public static LocString NAME = "Utilities";

				public static LocString TOOLTIP = "Guaranteed to keep your colony warm and cozy.";
			}

			public static class PLUMBING
			{
				public static LocString NAME = "Plumbing";

				public static LocString TOOLTIP = "Move gases and liquid around with the greatest of ease.";
			}

			public static class REFINING
			{
				public static LocString NAME = "Refinement";

				public static LocString TOOLTIP = "Use the resources you want, filter the ones you don't.";
			}

			public static class MEDICAL
			{
				public static LocString NAME = "Medicine";

				public static LocString TOOLTIP = "A cure for everything but the common cold.";
			}

			public static class FURNITURE
			{
				public static LocString NAME = "Furniture";

				public static LocString TOOLTIP = "Amenities to keep your Duplicants happy, comfy and efficient.";
			}

			public static class EQUIPMENT
			{
				public static LocString NAME = "Stations";

				public static LocString TOOLTIP = "Unlock new technologies through the power of science!";
			}

			public static class MISC
			{
				public static LocString NAME = "Decor";

				public static LocString TOOLTIP = "Spruce up your colony with some lovely interior decorating.";
			}
		}

		public class TOOLS
		{
			public class GENERIC
			{
				public static LocString BACK = "Back";

				public static LocString UNKNOWN = "UNKNOWN";
			}

			public class BUILD
			{
				public static LocString NAME = "Build {0}";

				public static LocString TOOLNAME = "Build tool";

				public static LocString TOOLACTION = "CLICK TO BUILD";

				public static LocString TOOLACTION_DRAG = "DRAG TO BUILD";
			}

			public class MOVETOLOCATION
			{
				public static LocString TOOLNAME = "Move To Tool";

				public static LocString TOOLACTION = "CLICK TO MOVE {0} HERE";
			}

			public class COPYSETTINGS
			{
				public static LocString NAME = "Paste Settings";

				public static LocString TOOLNAME = "Paste Settings Tool";

				public static LocString TOOLACTION = "DRAG TO PASTE SETTINGS";
			}

			public class DIG
			{
				public static LocString NAME = "Dig";

				public static LocString TOOLNAME = "Dig tool";

				public static LocString TOOLACTION = "DRAG TO DIG";
			}

			public class CANCEL
			{
				public static LocString NAME = "Cancel";

				public static LocString TOOLNAME = "Cancel tool";

				public static LocString TOOLACTION = "DRAG TO CANCEL ACTION";
			}

			public class DECONSTRUCT
			{
				public static LocString NAME = "Deconstruct";

				public static LocString TOOLNAME = "Deconstruct tool";

				public static LocString TOOLACTION = "DRAG TO DECONSTRUCT";
			}

			public class CLEANUPCATEGORY
			{
				public static LocString NAME = "Clean";

				public static LocString TOOLNAME = "Clean Up tools";
			}

			public class REGIONCATEGORY
			{
				public static LocString NAME = "Regions";

				public static LocString TOOLNAME = "Region tool";

				public static LocString CREATETOOLTIP = "DRAG TO CREATE";

				public static LocString ERASETOOLTIP = "DRAG TO ERASE";

				public static LocString MERGETOOLTIP = "DRAG TO MERGE";
			}

			public class PRIORITIESCATEGORY
			{
				public static LocString NAME = "Prioritize";
			}

			public class ERASEREGION
			{
				public static LocString NAME = "Clear Region";

				public static LocString TOOLNAME = "Clear Region tool";

				public static LocString TOOLACTION = "DRAG TO ERASE";
			}

			public class MARKFORSTORAGE
			{
				public static LocString NAME = "Sweep";

				public static LocString TOOLNAME = "Sweep tool";

				public static LocString TOOLACTION = "DRAG TO SWEEP";
			}

			public class MOP
			{
				public static LocString NAME = "Mop";

				public static LocString TOOLNAME = "Mop tool";

				public static LocString TOOLACTION = "DRAG TO MOP";
			}

			public class ATTACK
			{
				public static LocString NAME = "Harvest";

				public static LocString TOOLNAME = "Harvest tool";

				public static LocString TOOLACTION = "DRAG TO TOGGLE HARVEST";
			}

			public class PRIORITIZE
			{
				public static LocString NAME = "Prioritize";

				public static LocString TOOLNAME = "Prioritize tool";

				public static LocString TOOLACTION = "DRAG TO PRIORITIZE";

				public static LocString SPECIFIC_PRIORITY = "Set Priority: {0}";
			}

			public class EXOSUITREGION
			{
				public static LocString NAME = "Drag to designate an exosuit region";

				public static LocString TOOLNAME = "Exosuit Region tool";
			}

			public class FILTERSCREEN
			{
				public static LocString OPTIONS = "Options";
			}

			public class FILTERLAYERS
			{
				public static LocString BUILDINGS = "Buildings";

				public static LocString TILES = "Tiles";

				public static LocString WIRES = "Wires";

				public static LocString LIQUIDPIPES = "Liquid Pipes";

				public static LocString GASPIPES = "Gas Pipes";

				public static LocString DIGPLACER = "Dig Orders";

				public static LocString CLEANANDCLEAR = "Sweep & Mop Orders";

				public static LocString ALL = "All";

				public static LocString HARVEST_WHEN_READY = "Autoharvest";

				public static LocString DO_NOT_HARVEST = "Manual Harvest";
			}
		}

		public class DETAILTABS
		{
			public class STATS
			{
				public static LocString NAME = "Stats";

				public static LocString TOOLTIP = "View this Duplicant's attributes, traits, and diseases";

				public static LocString GROUPNAME_ATTRIBUTES = "ATTRIBUTES";

				public static LocString GROUPNAME_ATTRIBUTES_SKILLS = "<b>Skills</b>";

				public static LocString GROUPNAME_ATTRIBUTES_EXPECTATIONS = "<b>Expectations</b>";

				public static LocString GROUPNAME_TRAITS = "TRAITS";
			}

			public class SIMPLEINFO
			{
				public static LocString NAME = "Info";

				public static LocString TOOLTIP = "View the status of the selected item";

				public static LocString GROUPNAME_STATUS = "STATUS";

				public static LocString GROUPNAME_DESCRIPTION = "INFORMATION";

				public static LocString GROUPNAME_CONDITION = "CONDITION";

				public static LocString GROUPNAME_RESEARCH = "RESEARCH";
			}

			public class DETAILS
			{
				public static LocString NAME = "Details";

				public static LocString TOOLTIP = "More information";

				public static LocString GROUPNAME_DETAILS = "DETAILS";

				public static LocString GROUPNAME_CONTENTS = "CONTENTS";

				public static LocString GROUPNAME_MINION_CONTENTS = "CARRIED ITEMS";

				public static LocString STORAGE_EMPTY = "None";

				public static LocString CONTENTS_MASS = "{0}: {1}";

				public static LocString CONTENTS_TEMPERATURE = "{0} at {1}";

				public static LocString CONTENTS_ROTTABLE = "\n\t• {0}";
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

				public static LocString AVAILABLE_JOULES = "<style=\"power\">Power</style> stored: {0}";

				public static LocString AVAILABLE_JOULES_TOOLTIP = "Amount of power stored in batteries";

				public static LocString WATTAGE_GENERATED = "<style=\"power\">Power</style> produced: {0}";

				public static LocString WATTAGE_GENERATED_TOOLTIP = "The total amount of power generated by this circuit";

				public static LocString WATTAGE_CONSUMED = "<style=\"power\">Power</style> consumed: {0}";

				public static LocString WATTAGE_CONSUMED_TOOLTIP = "The total amount of power used by this circuit";

				public static LocString POTENTIAL_WATTAGE_CONSUMED = "Potential power consumed: {0}";

				public static LocString POTENTIAL_WATTAGE_CONSUMED_TOOLTIP = "The total amount of power that can be used by this circuit if all connected buildings are active";

				public static LocString MAX_SAFE_WATTAGE = "Maximum Safe Wattage: {0}";

				public static LocString MAX_SAFE_WATTAGE_TOOLTIP = "Exceeding this value will overload the circuit and can result in damage to wiring and buildings";
			}

			public class NEEDS
			{
				public static LocString NAME = "Stress";

				public static LocString TOOLTIP = "View this Duplicant's psychological status";

				public static LocString CURRENT_STRESS_LEVEL = "Current <style=\"stress\">Stress</style> Level: {0}";

				public static LocString OVERVIEW = "Overview";

				public static LocString STRESS_CREATORS = "<style=\"stress\">Stress</style> Creators";

				public static LocString STRESS_RELIEVERS = "<style=\"stress\">Stress</style> Relievers";

				public static LocString CURRENT_NEED_LEVEL = "Current Level: {0}";

				public static LocString NEXT_NEED_LEVEL = "Next Level: {0}";
			}

			public class POSSESSIONS
			{
				public static LocString NAME = "Assigned";

				public static LocString TOOLTIP = "View this Duplicant's personal amenities";

				public static LocString GROUPNAME_ROOMS = "ROOMS";

				public static LocString GROUPNAME_OWNABLE = "EQUIPMENT";

				public static LocString GROUPNAME_EQUIPMENT = "HELD ITEMS";

				public static LocString UNASSIGNED = "Unassigned";

				public static LocString UNASSIGNED_TOOLTIP = "This Duplicant has not been assigned a {0}";

				public static LocString ASSIGNED_TOOLTIP = "This Duplicant has been assigned a {0}.\nEffects:{1}";

				public static LocString NOEQUIPMENT = "No equipment";

				public static LocString NOEQUIPMENT_TOOLTIP = "This Duplicant has not been equipped with a tool";

				public static LocString EQUIPMENT_TOOLTIP = "This Duplicant has been equipped with a {0}";
			}
		}

		public class BUILDINGEFFECTS
		{
			public static LocString OPERATIONREQUIREMENTS = "<b>Requirements:</b>";

			public static LocString REQUIRESPOWER = "<style=\"power\">Power</style>: <style=\"consumed\">-{0}</style>";

			public static LocString REQUIRESELEMENT = "Supply of <style={0}>{1}</style>";

			public static LocString REQUIRESLIQUIDINPUT = "<style=\"LiquidPiping\">Liquid Intake Pipe</style>";

			public static LocString REQUIRESLIQUIDOUTPUT = "<style=\"LiquidPiping\">Liquid Output Pipe</style>";

			public static LocString REQUIRESLIQUIDOUTPUTS = "Two <style=\"LiquidPiping\">Liquid Output Pipes</style>";

			public static LocString REQUIRESGASINPUT = "<style=\"GasPiping\">Gas Intake Pipe</style>";

			public static LocString REQUIRESGASOUTPUT = "<style=\"GasPiping\">Gas Output Pipe</style>";

			public static LocString REQUIRESGASOUTPUTS = "Two <style=\"GasPiping\">Gas Output Pipes</style>";

			public static LocString REQUIRESMANUALOPERATION = "Duplicant operation";

			public static LocString REQUIRESPOWERGENERATOR = "<style=\"power\">Power</style> generator";

			public static LocString REQUIRESSEED = "1 Unplanted <style=\"seed\">Seed</style>";

			public static LocString ALLOWS_FERTILIZER = "Plant <style=\"solid\">Fertilization</style>";

			public static LocString ALLOWS_IRRIGATION = "Plant <style=\"LiquidPiping\">Irrigation</style>";

			public static LocString ASSIGNEDDUPLICANT = "Duplicant assignment";

			public static LocString CONSUMESANYELEMENT = "Any <style=\"{0}\">Element</style>";

			public static LocString OPERATIONEFFECTS = "<b>Effects:</b>";

			public static LocString BATTERYEFFECT = "<style=\"power\">Power</style> capacity: {0}";

			public static LocString STORAGECAPACITY = "Storage capacity: {0}";

			public static LocString ELEMENTEMITTED = "<style={0}>{1}</style>: <style=\"produced\">+{2}</style>";

			public static LocString ELEMENTCONSUMED = "<style={0}>{1}</style>: <style=\"consumed\">-{2}</style>";

			public static LocString ELEMENTEMITTEDPERUSE = "<style={0}>{1}</style>: <style=\"produced\">+{2} per use</style>";

			public static LocString ELEMENTCONSUMEDPERUSE = "<style={0}>{1}</style>: <style=\"consumed\">-{2} per use</style>";

			public static LocString ENERGYCONSUMED = "<style=\"power\">Power</style> consumed: {0}";

			public static LocString ENERGYGENERATED = "<style=\"power\">Power</style>: <style=\"produced\">+{0}</style>";

			public static LocString HEATGENERATED = "<style=\"heat\">Heat</style>: <style=\"produced\">+{0}</style>";

			public static LocString HEATCONSUMED = "<style=\"heat\">Heat</style>: <style=\"consumed\">-{0}</style>";

			public static LocString FABRICATES = "Fabricates";

			public static LocString FABRICATEDITEM = "<style=\"{0}\">{1}</style>";

			public static LocString PLANTERBOX_PENTALTY = "Planter box penalty";

			public static LocString DECORPROVIDED = "<style=\"decor\">Decor</style>: <style=\"{0}\">{1}</style> ({2} tile radius)";

			public static LocString OVERHEAT_TEMP = "Overheat Temperature: {0}";

			public static LocString REFILLOXYGENTANK = "Refills <style=\"equipment\">Exosuit Oxygen Tank</style>";

			public static LocString DUPLICANTMOVEMENTBOOST = "Runspeed: <style=\"produced\">+{0}</style>";

			public static LocString STRESSREDUCEDPERMINUTE = "<style=\"stress\">Stress</style>: <style=\"consumed\">{0} per minute</style>";

			public static LocString REMOVESEFFECTSUBTITLE = "Cures";

			public static LocString REMOVEDEFFECT = "<style=\"disease\">{0}</style>";

			public static LocString ADDED_EFFECT = "<style=\"disease\">{0}</style>";

			public static LocString GASCOOLING = "<style=\"heat\">Cooling factor: </style>: <style=\"produced\">{0}</style>";

			public static LocString MAX_WATTAGE = "Max <style=\"power\">Power</style>: {0}";

			public static LocString PRODUCES_RESEARCH_POINTS = "{0}";

			public static LocString HIT_POINTS_PER_CYCLE = "<style=\"Health\">Health</style> per cycle: <style=\"produced\">{0}</style>";

			public static LocString KCAL_PER_CYCLE = "<style=\"food\">KCal</style> per cycle: <style=\"produced\">{0}</style>";

			public class TOOLTIPS
			{
				public static LocString OPERATIONREQUIREMENTS = "All requirements must be met in order for this building to operate";

				public static LocString REQUIRESPOWER = "Must be connected to a power grid with at least -{0} of available power";

				public static LocString REQUIRESELEMENT = "Must receive deliveries of <style={0}>{1}</style> to function";

				public static LocString REQUIRESLIQUIDINPUT = "Must receive liquid from a liquid pipe system";

				public static LocString REQUIRESLIQUIDOUTPUT = "Must expel liquid through a liquid pipe system";

				public static LocString REQUIRESLIQUIDOUTPUTS = "Must expel liquid through a liquid pipe system";

				public static LocString REQUIRESGASINPUT = "Must receive gas from a gas pipe system";

				public static LocString REQUIRESGASOUTPUT = "Must expel gas through a gas pipe system";

				public static LocString REQUIRESGASOUTPUTS = "Must expel gas through a gas pipe system";

				public static LocString REQUIRESMANUALOPERATION = "A Duplicant must be present to run this building";

				public static LocString REQUIRESPOWERGENERATOR = "Must be connected to a power producing generator to function";

				public static LocString REQUIRESSEED = "Must receive a plant seed";

				public static LocString ALLOWS_FERTILIZER = "This farm plot enables plant fertilization to improve Harvest Rating and improve yield";

				public static LocString ALLOWS_IRRIGATION = "This farm plot enables plant irrigation to improve Harvest Rating and improve yield";

				public static LocString ASSIGNEDDUPLICANT = "This amenity may only be used by the Duplicant it is assigned to";

				public static LocString OPERATIONEFFECTS = "The building will produce these effects when its requirements are met";

				public static LocString BATTERYEFFECT = "Can hold {0} of power when connected to a generator";

				public static LocString STORAGECAPACITY = "Holds up to {0} of material";

				public static LocString ELEMENTEMITTED = "Produces {2} of {1} when in use";

				public static LocString ELEMENTCONSUMED = "Consumes {2} of {1} when in use";

				public static LocString ELEMENTEMITTEDPERUSE = "Produces {2} of {1} with each use";

				public static LocString ELEMENTCONSUMEDPERUSE = "Consumes {2} of {1} with each use";

				public static LocString ENERGYCONSUMED = "Draws {0} from the power grid it's connected to";

				public static LocString ENERGYGENERATED = "Produces {0} for the power grid it's connected to";

				public static LocString HEATGENERATED = "Generates +{0} of heat";

				public static LocString HEATCONSUMED = "Dissipates -{0} of heat";

				public static LocString FABRICATES = "Fabrication is the production of items and equipment";

				public static LocString PLANTERBOX_PENTALTY = "Plants grow more slowly when contained within boxes";

				public static LocString DECORPROVIDED = "Improves Decor values by {0} in a {1} tile radius";

				public static LocString DECORDECREASED = "Decreases Decor values by {0} in a {1} tile radius";

				public static LocString OVERHEAT_TEMP = "Begins overheating at {0} and melts down at {1}";

				public static LocString REFILLOXYGENTANK = "Refills Exosuit Oxygen Tanks with oxygen for reuse";

				public static LocString DUPLICANTMOVEMENTBOOST = "Duplicants walk {0} faster on this tile";

				public static LocString STRESSREDUCEDPERMINUTE = "Removes {0} of Duplicants' Stress for every uninterrupted minute of use";

				public static LocString REMOVESEFFECTSUBTITLE = "Use of this building will remove the listed effects";

				public static LocString REMOVEDEFFECT = "{0}";

				public static LocString ADDED_EFFECT = "{0}";

				public static LocString GASCOOLING = "Reduces the temperature of piped gases by {0}";

				public static LocString MAX_WATTAGE = "Drawing more than the maximum allowed power can result in damage to the circuit";

				public static LocString PRODUCES_RESEARCH_POINTS = "Produces {0} research";
			}
		}

		public class GAMEOBJECTEFFECTS
		{
			public static LocString CALORIES = "<style=\"produced\">+{0}</style>";

			public static LocString FOOD_QUALITY = "Quality: {0}";

			public static LocString FORGAVEATTACKER = "Forgiveness";

			public static LocString FRIEDMUSHBARREMOVESDISEASE = "Removes <style=\"disease\">Disease</style> potential from <style=\"food\">Food</style>";

			public static LocString COOKEDMEATTEMPORARYTRAIT = "Temporarily grants the <style=\"stress\">Soul Food</style> effect";

			public static LocString COLDBREATHER = "<style=\"heat\">Cooling Effect</style>";

			public static LocString LIFECYCLETITLE = "Growth:";

			public static LocString GROWTHTIME_SIMPLE = "Lifecycle: {0}";

			public static LocString GROWTHTIME_REGROWTH = "Lifecycle: {0} / {1}";

			public static LocString GROWTHTIME = "Growth: {0}";

			public static LocString INITIALGROWTHTIME = "Initial Growth: {0}";

			public static LocString REGROWTHTIME = "Regrowth: {0}";

			public static LocString REQUIRESFERTILIZER = "<style=\"solid\">{0}</style>: <style=\"consumed\">{1}</style>";

			public static LocString IDEAL_FERTILIZER = "<style=\"solid\">{0}</style>: <style=\"consumed\">{1}</style>";

			public static LocString EQUIPMENT_MODS = "{Attribute} <style=\"{Style}\">{Value}</style>";

			public static LocString ROTTEN = "Rotten";

			public static LocString REQUIRES_PRESSURE = "<style=\"gas\">Air</style> Pressure: {0} minimum";

			public static LocString IDEAL_PRESSURE = "<style=\"gas\">Air</style> Pressure: {0}";

			public static LocString REQUIRES_TEMPERATURE = "<style=\"heat\">Temperature</style>: {0} to {1}";

			public static LocString IDEAL_TEMPERATURE = "<style=\"heat\">Temperature</style>: {0} to {1}";

			public static LocString REQUIRES_SUBMERSION = "<style=\"liquid\">Liquid</style> Submersion";

			public static LocString SEED_PRODUCTION_DIG_ONLY = "Consumes 1 <style=\"seed\">Seed</style>";

			public static LocString SEED_PRODUCTION_HARVEST = "Harvest yields <style=\"seed\">Seeds</style>";

			public static LocString SEED_PRODUCTION_FINAL_HARVEST = "Final harvest yields <style=\"seed\">Seeds</style>";

			public static LocString SEED_PRODUCTION_FRUIT = "Fruit produces <style=\"seed\">Seeds</style>";

			public static LocString SEED_REQUIREMENT_CEILING = "Plot Orientation: Downward";

			public static LocString SEED_REQUIREMENT_WALL = "Plot Orientation: Sideways";

			public static LocString PLANT_MARK_FOR_HARVEST = "Autoharvest";

			public static LocString PLANT_DO_NOT_HARVEST = "Manual Harvest";

			public class TOOLTIPS
			{
				public static LocString CALORIES = "<style=\"produced\">+{0}</style>";

				public static LocString FOOD_QUALITY = "Quality: {0}";

				public static LocString FRIEDMUSHBARREMOVESDISEASE = "Removes <style=\"disease\">Disease</style> potential from <style=\"food\">Mush Bars</style>";

				public static LocString COOKEDMEATTEMPORARYTRAIT = "Temporarily grants the <style=\"stress\">Soul Food</style> effect";

				public static LocString COLDBREATHER = "Lowers ambient air temperature";

				public static LocString GROWTHTIME_SIMPLE = "This plant takes {0} to grow";

				public static LocString GROWTHTIME_REGROWTH = "This plant initially takes {0} to grow, but only {1} to mature again after first harvest";

				public static LocString GROWTHTIME = "This plant takes {0} to grow";

				public static LocString INITIALGROWTHTIME = "This plant takes {0} to mature again once replanted";

				public static LocString REGROWTHTIME = "This plant takes {0} to mature again once harvested";

				public static LocString EQUIPMENT_MODS = "{Attribute} {Value}";

				public static LocString REQUIRESFERTILIZER = "This plant requires {1} {0} for basic growth";

				public static LocString IDEAL_FERTILIZER = "This plant will generate Harvest Rating Points if provided with {1} of {0}";

				public static LocString REQUIRES_PRESSURE = "Gas pressure around this plant must be at least {0} for basic growth";

				public static LocString IDEAL_PRESSURE = "This plant will generate Harvest Rating Points in atmospheric pressures above {0}";

				public static LocString REQUIRES_TEMPERATURE = "Atmospheric temperature must be kept between {0} and {1} for basic growth";

				public static LocString IDEAL_TEMPERATURE = "This plant will generate Harvest Rating Points in temperatures between {0} and {1}";

				public static LocString REQUIRES_SUBMERSION = "This plant must be fully submerged in liquid for basic growth";

				public static LocString SEED_PRODUCTION_DIG_ONLY = "This plant can be replanted but will produce no further seeds";

				public static LocString SEED_PRODUCTION_HARVEST = "Harvesting this plant will yield new seeds";

				public static LocString SEED_PRODUCTION_FINAL_HARVEST = "This plant will yield new seeds on the final harvest of its lifecycle";

				public static LocString SEED_PRODUCTION_FRUIT = "Consuming this plant's fruit will yield new plant seeds";

				public static LocString SEED_REQUIREMENT_CEILING = "This seed must be planted in a downward facing plot\n\nPress <color=#F44A47>[O]</color> while building farm plots to rotate them";

				public static LocString SEED_REQUIREMENT_WALL = "This seed must be planted in a side facing plot\n\nPress <color=#F44A47>[O]</color> while building plots to rotate them";
			}

			public class DAMAGE_POPS
			{
				public static LocString OVERHEAT = "Overheat Damage";

				public static LocString WRONG_ELEMENT = "Wrong Element Damage";

				public static LocString CIRCUIT_OVERLOADED = "Overload Damage";

				public static LocString LIQUID_PRESSURE = "Pressure Damage";

				public static LocString MINION_DESTRUCTION = "Anger Damage";

				public static LocString CONDUIT_CONTENTS_FROZE = "Freezing Damage";

				public static LocString CONDUIT_CONTENTS_BOILED = "Boiling Damage";
			}
		}

		public class ASTEROIDCLOCK
		{
			public static LocString CYCLE = "Cycle";

			public static LocString CYCLES_OLD = "This Colony is {0} Cycles Old";

			public static LocString TIME_PLAYED = "Time Played: {0} hours";
		}

		public class ENDOFDAYREPORT
		{
			public static LocString REPORT_TITLE = "DAILY REPORTS";

			public static LocString DAY_TITLE = "Cycle {0}";

			public static LocString DAY_TITLE_TODAY = "Cycle {0} - Today";

			public static LocString DAY_TITLE_YESTERDAY = "Cycle {0} - Yesterday";

			public static LocString NOTIFICATION_TITLE = "Cycle {0} report ready";

			public static LocString NOTIFICATION_TOOLTIP = "View the end of day report for Cycle {0}";

			public static LocString NEXT = "Next";

			public static LocString PREV = "Prev";

			public static LocString ADDED = "Added";

			public static LocString REMOVED = "Removed";

			public static LocString NET = "Net";

			public class OXYGEN_CREATED
			{
				public static LocString NAME = "<style=\"oxygen\">Oxygen</style> Generation:";

				public static LocString POSITIVE_TOOLTIP = "Your colony generated {0} of <style=\"oxygen\">Oxygen</style> over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "Your colony consumed {0} of <style=\"oxygen\">Oxygen</style> over the course of the day";
			}

			public class CALORIES_CREATED
			{
				public static LocString NAME = "Calorie Generation:";

				public static LocString POSITIVE_TOOLTIP = "Your colony produced {0} of <style=\"food\">Food</style> over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "Your colony consumed {0} of <style=\"food\">Food</style> over the course of the day";
			}

			public class STRESS_DELTA
			{
				public static LocString NAME = "<style=\"stress\">Stress</style> Change:";

				public static LocString POSITIVE_TOOLTIP = "Your colony's total <style=\"stress\">Stress</style> has increased by {0}";

				public static LocString NEGATIVE_TOOLTIP = "Your colony's total <style=\"stress\">Stress</style> has decreased by {0}";
			}

			public class TRAVEL_TIME
			{
				public static LocString NAME = "Travel Time:";

				public static LocString POSITIVE_TOOLTIP = "Your Duplicants spent a total of {0} traveling over the course of the day";
			}

			public class IDLE_TIME
			{
				public static LocString NAME = "Idle Time:";

				public static LocString POSITIVE_TOOLTIP = "Your Duplicants spent a total of {0} idling over the course of the day";
			}

			public class ENERGY_USAGE
			{
				public static LocString NAME = "<style=\"power\">Power</style> Usage:";

				public static LocString POSITIVE_TOOLTIP = "Your colony created {0} of <style=\"power\">Power</style> over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "Your colony consumed {0} of <style=\"power\">Power</style> over the course of the day";
			}

			public class ENERGY_WASTED
			{
				public static LocString NAME = "<style=\"power\">Power</style> Wasted:";

				public static LocString POSITIVE_TOOLTIP = "Your colony created {0} of <style=\"power\">Power</style> today that was not used or stored in a battery";
			}

			public class LEVEL_UP
			{
				public static LocString NAME = "Skill Increases:";

				public static LocString TOOLTIP = "Your Duplicants gained a total of {0} skill levels today";
			}

			public class TOILET_INCIDENT
			{
				public static LocString NAME = "Restroom Accidents:";

				public static LocString TOOLTIP = "{0} Duplicants didn't quite reach the toilet in time today";
			}

			public class DISEASE_ADDED
			{
				public static LocString NAME = "<style=\"disease\">Diseases</style> Contracted:";

				public static LocString POSITIVE_TOOLTIP = "{0} Duplicant(s) contracted a <style=\"disease\">Disease</style>";

				public static LocString NEGATIVE_TOOLTIP = "{0} Duplicant(s) were cured of a <style=\"disease\">Disease</style>";
			}

			public class CONTAMINATED_OXYGEN_FLATULENCE
			{
				public static LocString NAME = "<style=\"gas\">Flatulence</style> Generation:";

				public static LocString POSITIVE_TOOLTIP = "Your colony generated {0} of <style=\"gas\">Polluted Oxygen</style> over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "Your colony consumed {0} of <style=\"gas\">Polluted Oxygen</style> over the course of the day";
			}

			public class CONTAMINATED_OXYGEN_TOILET
			{
				public static LocString NAME = "<style=\"gas\">Toilet </style> Emissions:";

				public static LocString POSITIVE_TOOLTIP = "Your colony generated {0} of <style=\"gas\">Polluted Oxygen</style> over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "Your colony consumed {0} of <style=\"gas\">Polluted Oxygen</style> over the course of the day";
			}

			public class CONTAMINATED_OXYGEN_SUBLIMATION
			{
				public static LocString NAME = "<style=\"gas\">Sublimation</style>:";

				public static LocString POSITIVE_TOOLTIP = "Your colony generated {0} of <style=\"gas\">Polluted Oxygen</style> over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "Your colony consumed {0} of <style=\"gas\">Polluted Oxygen</style> over the course of the day";
			}

			public class NOTES
			{
				public static LocString BUTCHERED = "Butchered to make a {0}";

				public static LocString CRAFTED = "Crafted a {0}";

				public static LocString HARVESTED = "Harvested a {0}";

				public static LocString ATE = "Ate {0}";
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

				public static LocString DESCRIPTION = "WORK:\nDuring Work hours Duplicants will perform any pending job tasks in the colony.";
			}

			public static class RECREATION
			{
				public static LocString NAME = "Recreation";

				public static LocString DESCRIPTION = "HAMMER TIME:\nDuring Hammer Time, Duplicants will relieve their <style=\"stress\">Stress</style> through dance. Please be aware that no matter how hard your Duplicants try, they will absolutely not be able to touch this.";
			}

			public static class HYGIENE
			{
				public static LocString NAME = "Hygiene";

				public static LocString DESCRIPTION = "HYGIENE:\nDuring <style=\"hygiene\">Hygiene</style> hours Duplicants will head to their assigned washrooms to get cleaned up.";
			}
		}

		public class ELEMENTAL
		{
			public class PRIMARYELEMENT
			{
				public static LocString NAME = "Primary Element: {0}";

				public static LocString TOOLTIP = "The selected item is primarily composed of {0}";
			}

			public class UNITS
			{
				public static LocString NAME = "Stack Units: {0}";

				public static LocString TOOLTIP = "This stack contains {0} units of {1}";
			}

			public class MASS
			{
				public static LocString NAME = "Mass: {0}";

				public static LocString TOOLTIP = "The selected item has a mass of {0}";
			}

			public class TEMPERATURE
			{
				public static LocString NAME = "Temperature: {0}";

				public static LocString TOOLTIP = "The selected item's current temperature is {0}";
			}

			public class SHC
			{
				public static LocString NAME = "Specific Heat Capacity: {0}";

				public static LocString TOOLTIP = "The minimum amount of energy needed to raise the selected item's temperature is {0}";
			}

			public class THERMALCONDUCTIVITY
			{
				public static LocString NAME = "Thermal Conductivity: {0}";

				public static LocString TOOLTIP = "The selected item can conduct heat at a rate of {0}";
			}

			public class CONDUCTIVITYBARRIER
			{
				public static LocString NAME = "Insulation Thickness: {0}";

				public static LocString TOOLTIP = "Thicker insulation reduces an item's Thermal Conductivity";
			}

			public class VAPOURIZATIONPOINT
			{
				public static LocString NAME = "Vaporization Point: {0}";

				public static LocString TOOLTIP = "The selected item will evaporate into a gas at {0}";
			}

			public class MELTINGPOINT
			{
				public static LocString NAME = "Melting Point: {0}";

				public static LocString TOOLTIP = "The selected item will melt into a liquid at {0}";
			}

			public class OVERHEATPOINT
			{
				public static LocString NAME = "Overheat Modifier: {0}";

				public static LocString TOOLTIP = "This building will overheat and take damage if its temperature reaches {0}\n\nBuilding with better building materials can increase overheat temperature";
			}

			public class FREEZEPOINT
			{
				public static LocString NAME = "Freeze Point: {0}";

				public static LocString TOOLTIP = "The selected item will cool into a solid at {0}";
			}

			public class DEWPOINT
			{
				public static LocString NAME = "Dew Point: {0}";

				public static LocString TOOLTIP = "The selected item will condense into a liquid at {0}";
			}
		}

		public class IMMIGRANTSCREEN
		{
			public static LocString IMMIGRANTSCREENTITLE = "Select a DNA Blueprint";

			public static LocString PROCEEDBUTTON = "Print";

			public static LocString CANCELBUTTON = "Cancel";

			public static LocString REJECTALL = "Reject All";

			public static LocString EMBARK = "EMBARK";

			public static LocString SELECTDUPLICANTS = "Select {0} Duplicants";

			public static LocString SELECTYOURCREW = "ASSEMBLE YOUR CREW TO BEGIN";

			public static LocString SHUFFLE = "SHUFFLE";

			public static LocString SHUFFLETOOLTIP = "Shuffle for a different Duplicant";

			public static LocString BACK = "BACK";

			public static LocString CONFIRMATIONTITLE = "Reject All Duplicants?";

			public static LocString CONFIRMATIONBODY = "The Printing Pod will need time to recharge if you Ooze these Duplicants.";

			public static LocString NAME_YOUR_COLONY = "NAME YOUR COLONY";

			public static LocString SHUFFLE_COLONY_NAME = "Randomize colony name";
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
		}
	}
}
