using System;

namespace STRINGS
{
	public class UI
	{
		public static LocString POSITIVE_FORMAT = "+{0}";

		public static LocString SPEED_SLOW = "SLOW";

		public static LocString SPEED_MEDIUM = "MEDIUM";

		public static LocString SPEED_FAST = "FAST";

		public static LocString RED_ALERT = "RED ALERT";

		public static LocString JOBS = "JOBS";

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

		public static LocString PRODUCTINFO_SELECTMATERIAL = "Select {0}:";

		public static LocString PRODUCTINFO_RESEARCHREQUIRED = "Research required...";

		public static LocString PRODUCTINFO_REQUIRESRESEARCHDESC = "Requires {0} Research";

		public static LocString PRODUCTINFO_APPLICABLERESOURCES = "Required resources:";

		public static LocString PRODUCTINFO_MISSINGRESOURCES_TITLE = "Requires {0}: {1}";

		public static LocString PRODUCTINFO_MISSINGRESOURCES_HOVER = "Missing resource(s)";

		public static LocString PRODUCTINFO_MISSINGRESOURCES_DESC = "{0} has not yet been discovered.";

		public static LocString EQUIPMENTTAB_OWNED = "Owned Items";

		public static LocString EQUIPMENTTAB_HELD = "Held Items";

		public static LocString EQUIPMENTTAB_ROOM = "Assigned Rooms";

		public static LocString JOBSCREEN_PRIORITY = "Priority";

		public static LocString JOBSCREEN_HIGH = "High";

		public static LocString JOBSCREEN_LOW = "Low";

		public static LocString JOBSCREEN_EVERYONE = "Everyone";

		public static LocString VITALSSCREEN_NAME = "Name";

		public static LocString VITALSSCREEN_STRESS = "Stress";

		public static LocString VITALSSCREEN_CALORIES = "Fullness";

		public static LocString VITALSSCREEN_RATIONS = "Calories/Day";

		public static LocString VITALSSCREEN_EATENTODAY = "Eaten Today";

		public static LocString VITALSSCREEN_RATIONS_TOOLTIP = "Set how many calories this Duplicant may consume daily";

		public static LocString VITALSSCREEN_EATENTODAY_TOOLTIP = "The amount of <style=\"food\">Food</style> your Duplicant has eaten this cycle";

		public static LocString VITALSSCREEN_UNTIL_FULL = "Until Full";

		public static LocString RESEARCHSCREEN_UNLOCKSTOOLTIP = "Unlocks: {0}";

		public static LocString ATTRIBUTELEVEL = "Level {0} {1}";

		public static LocString NEUTRONIUMMASS = "Immeasurable";

		public static LocString CALCULATING = "Calculating...";

		public static LocString FORMATDAY = "{0} cycles";

		public static LocString FORMATSECONDS = "{0}s";

		public static LocString DELIVERED = "Delivered: {0} {1}";

		public static LocString PICKEDUP = "Picked Up: {0} {1}";

		public static LocString WELCOMEMESSAGETITLE = "! ALERT !";

		public static LocString WELCOMEMESSAGEBODY = "Your crew has awoken miles beneath the surface of an unfamiliar terrestrial body. Oddly, they have no recollection of how they got here.\n\nIt might be best to start digging.";

		public static LocString WELCOMEMESSAGEBEGIN = "BEGIN";

		public static LocString VIEWDUPLICANTS = "Choose a Duplicant";

		public static LocString DUPLICANTPRINTING = "Duplicant Printing";

		public static LocString ASSIGNDUPLICANT = "Assign Duplicant";

		public static LocString CRAFT = "FABRICATE";

		public static LocString PLACEINRECEPTACLE = "Plant";

		public static LocString REMOVEFROMRECEPTACLE = "Dig Up";

		public static LocString CANCELPLACEINRECEPTACLE = "Cancel";

		public static LocString CANCELREMOVALFROMRECEPTACLE = "Cancel";

		public static LocString CHANGEPERSECOND = "Change per second: {0}";

		public static LocString CHANGEPERCYCLE = "Change per cycle: {0}";

		public static LocString LISTENTRYSTRING = "     {0}\n";

		public static LocString LISTENTRYSTRINGNOLINEBREAK = "     {0}";

		public static LocString LISTENTRYTAB = "     ";

		public class FRONTEND
		{
			public class MAINMENU
			{
				public static LocString NEWGAME = "NEW GAME";

				public static LocString RESUMEGAME = "RESUME GAME";

				public static LocString LOADGAME = "LOAD GAME";

				public static LocString OPTIONS = "OPTIONS";

				public static LocString QUITTODESKTOP = "QUIT";

				public static LocString RESTARTCONFIRM = "Are you sure you want to quit?\nAll unsaved progress will be lost.";

				public static LocString QUITCONFIRM = "Are you sure you want to quit?\nAll unsaved progress will be lost.";

				public static LocString RESUMEBUTTON_BASENAME = "{0}: Cycle {1}";
			}

			public class PATCHNOTESSCREEN
			{
				public static LocString TITLE = "IMPORTANT UPDATE NOTES";

				public static LocString BODY = "<b>Changes you should know about: </b>\n\n{0}";

				public static LocString PATCHNOTES = "- Fixed crash when queuing research\n- Fixed crash when clicking on research center\n- Gas Pumps are not working properly\n- Farm Tiles have been removed from the game(if you had any existing farm tiles, they will turn into clay)";

				public static LocString OK_BUTTON = "OK";
			}

			public class LOADSCREEN
			{
				public static LocString TITLE = "LOAD GAME";

				public static LocString TITLE_INSPECT = "LOAD GAME";

				public static LocString DELETEBUTTON = "Delete Slot";

				public static LocString CONFIRMDELETE = "Are you sure you want to delete {0}?\nYou cannot undo this action.";

				public static LocString SAVEDETAILS = "<b>File:</b> {0}\n\n<b>Save Date:</b>\n{1}\n\n<b>Base Name:</b> {2}\n<b>Duplicants Alive:</b> {3}\n<b>Cycles Survived:</b> {4}";

				public static LocString AUTOSAVEWARNING = " <b><color=#ff0000>(Auto Save: This file will get deleted as new Auto Saves are created)</color></b>";

				public static LocString CORRUPTEDSAVE = "<b><color=#ff0000>Could not load file {0}. Its data may be corrupted.</color></b>";

				public static LocString SAVE_TOO_NEW = "<b><color=#ff0000>Could not load file {0}. File is using build {1}. This build is {2}.</color></b>";
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

				public static LocString METRICS = "Metrics";

				public static LocString CREDITS = "Credits";

				public static LocString BACK = "Back";
			}

			public class AUDIO_OPTIONS_SCREEN
			{
				public static LocString TITLE = "AUDIO OPTIONS";

				public static LocString DONE_BUTTON = "Done";
			}

			public class METRICS_OPTIONS_SCREEN
			{
				public static LocString TITLE = "METRICS OPTIONS";

				public static LocString ENABLE_BUTTON = "Enable";

				public static LocString DONE_BUTTON = "Done";

				public static LocString TOOLTIP = "Collect metrics to help us improve the game";
			}

			public class UNIT_OPTIONS_SCREEN
			{
				public static LocString TITLE = "TEMPERATURE UNITS";

				public static LocString CELSIUS = "Celsius";

				public static LocString CELSIUS_TOOLTIP = "Change temperature unit to Celsius (\ufffdC)";

				public static LocString KELVIN = "Kelvin";

				public static LocString KELVIN_TOOLTIP = "Change temperature unit to Kelvin (K)";

				public static LocString FAHRENHEIT = "Fahrenheit";

				public static LocString FAHRENHEIT_TOOLTIP = "Change temperature unit to Fahrenheit (\ufffdF)";
			}

			public class GRAPHICS_OPTIONS_SCREEN
			{
				public static LocString TITLE = "GRAPHICS OPTIONS";

				public static LocString FULLSCREEN = "Fullscreen:";

				public static LocString RESOLUTION = "Resolution:";

				public static LocString APPLYBUTTON = "Apply";

				public static LocString REVERTBUTTON = "Revert";

				public static LocString DONE_BUTTON = "Done";

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

			public class INPUTBINDINGSCREEN
			{
				public static LocString DUPLICATE = "{0} was already bound to {1} and is now unbound.";

				public static LocString UNBOUND_ACTION = "{0} is unbound. Are you sure you want to continue?";

				public static LocString MULTIPLE_UNBOUND_ACTIONS = "You have multiple unbound actions, this may result in difficulty playing the game. Are you sure you want to continue?";
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

			public static LocString MANAGEMENTMENU_JOBS = "Manage Duplicants' job prioritizations";

			public static LocString MANAGEMENTMENU_VITALS = "View Duplicants' vitals";

			public static LocString MANAGEMENTMENU_RESEARCH = "View Research Tree";

			public static LocString MANAGEMENTMENU_DAILYREPORT = "View Daily Reports";

			public static LocString MANAGEMENTMENU_SCHEDULE = "Adjust the colony's timetable";

			public static LocString MANAGEMENTMENU_REQUIRES_RESEARCH = "Build a <style=\"light\">Research Station</style> to unlock";

			public static LocString METERSCREEN_AVGSTRESS = "Highest Stress: {0}";

			public static LocString METERSCREEN_MEALHISTORY = "Calories Available: {0}";

			public static LocString PLAYBUTTON = "Start";

			public static LocString PAUSEBUTTON = "Pause";

			public static LocString PAUSE = "Pause simulation";

			public static LocString UNPAUSE = "Unpause simulation";

			public static LocString SPEEDBUTTON_SLOW = "Slow speed {0}";

			public static LocString SPEEDBUTTON_MEDIUM = "Medium speed {0}";

			public static LocString SPEEDBUTTON_FAST = "Fast speed {0}";

			public static LocString RED_ALERT = "Toggle Red Alert\nDuplicants will enter a state of emergency and ignore their basic needs to work";

			public static LocString MOPBUTTON = "Drag to wipe up liquid messes";

			public static LocString DIGBUTTON = "Drag to set dig tasks and excavate resources";

			public static LocString CANCELBUTTON = "Drag to cancel pending job tasks";

			public static LocString DECONSTRUCTBUTTON = "Drag to demolish buildings and refund their resources";

			public static LocString ATTACKBUTTON = "Drag to batch harvest mature plants";

			public static LocString CLEARBUTTON = "Drag to declutter floors and move resources into storage";

			public static LocString HARVESTBUTTON = "Drag to batch harvest mature plants";

			public static LocString PRIORITIZEMAINBUTTON = "Drag to bump tasks up or down in importance";

			public static LocString PRIORITIZEBUTTON = "Drag to bump job tasks up or down in importance";

			public static LocString DEPRIORITIZEBUTTON = "Drag to deprioritize dig and build tasks";

			public static LocString TRASHREGIONBUTTON = "Drag to create storage regions for materials";

			public static LocString SELECTREGIONBUTTON = "Click to select existing storage regions";

			public static LocString ERASEREGIONBUTTON = "Drag to erase existing exosuit regions";

			public static LocString CLEANUPMAINBUTTON = "Mop and declutter messy floors";

			public static LocString STORAGEMAINBUTTON = "Create and manage exosuit regions";

			public static LocString CANCELDECONSTRUCTIONBUTTON = "Cancel queued orders or deconstruct existing buildings";

			public static LocString HELP_ROTATE_KEY = "Press <color=#833A5FFF>[O]</color> to Rotate.";

			public static LocString HELP_BUILDLOCATION_FLOOR = "Must be built on the ground";

			public static LocString HELP_BUILDLOCATION_OCCUPIED = "Must be built in unoccupied space";

			public static LocString HELP_BUILDLOCATION_CEILING = "Must be built on the ceiling";

			public static LocString HELP_BUILDLOCATION_INSIDEGROUND = "Must be built in the ground";

			public static LocString OXYGENOVERLAYSTRING = "Displays ambient oxygen density";

			public static LocString POWEROVERLAYSTRING = "Displays and edits power grids";

			public static LocString TEMPERATUREOVERLAYSTRING = "Displays ambient temperature";

			public static LocString ROOMSOVERLAYSTRING = "Displays fully enclosed rooms";

			public static LocString LIGHTSOVERLAYSTRING = "Displays the visibility radius of light sources";

			public static LocString REGIONOVERLAYSTRING = "Displays areas requiring Duplicant safety gear";

			public static LocString LIQUIDVENTOVERLAYSTRING = "Displays and edits liquid pipe systems";

			public static LocString GASVENTOVERLAYSTRING = "Displays and edits gas pipe systems";

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

			public static LocString SORTCOLUMN = "Click to sort";

			public static LocString NOMATERIAL = "Not enough materials";

			public static LocString SELECTAMATERIAL = "There are insufficient materials to construct this building";

			public static LocString EDITNAME = "Give this Duplicant a name";

			public static LocString RANDOMIZENAME = "Randomize this Duplicant's name";

			public static LocString BASE_VALUE = "Base Value";
		}

		public class DEVELOPMENTBUILDS
		{
			public static LocString BUILDWATERMARK = "DEVELOPMENT BUILD: CL#{0}";

			public class ALPHA
			{
				public class MESSAGES
				{
					public static LocString HEADER = "DEVELOPMENT BUILD";

					public static LocString BODY = "Stay up to date by joining our mailing list, or head on over to the forums and join the discussion.";

					public static LocString FORUMBUTTON = "FORUMS";

					public static LocString MAILINGLIST = "MAILING LIST";
				}

				public class LOADING
				{
					public static LocString TITLE = "<b>Welcome to Oxygen Not Included!</b>";

					public static LocString BODY = "This game is in the early stages of development which means you're likely to encounter strange, amusing, and occasionally just downright frustrating bugs.\n\nDuring this time Oxygen Not Included will be receiving regular updates to fix bugs, add features and introduce additional content, so if you encounter issues or just have suggestions to share, please let us know on our forums: <u>http://forums.kleientertainment.com</u>\n\nWe value your feedback and thank you for joining us in the development process. We couldn't do this without you.\n\nEnjoy your time in deep space!\n\n- Klei";

					public static LocString CONTINUEBUTTON = "Okay, thanks for the heads up!";
				}
			}
		}

		public class UNITSUFFIXES
		{
			public static LocString PERSECOND = "/s";

			public static LocString PERCYCLE = "/cycle";

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
		}

		public class OVERLAYS
		{
			public class OXYGEN
			{
				public static LocString NAME = "OXYGEN OVERLAY";

				public static LocString BUTTON = "Oxygen Overlay";

				public static LocString LEGEND1 = "Very Breathable";

				public static LocString LEGEND2 = "Breathable";

				public static LocString LEGEND3 = "Barely Breathable";

				public static LocString LEGEND4 = "Unbreathable";

				public static LocString LEGEND5 = "Slightly Toxic";

				public static LocString LEGEND6 = "Very Toxic";

				public class TOOLTIPS
				{
					public static LocString LEGEND1 = "High oxygen concentration";

					public static LocString LEGEND2 = "Prominent oxygen concentration";

					public static LocString LEGEND3 = "Prominent carbon dioxide concentration";

					public static LocString LEGEND4 = "High carbon dioxide concentration";

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

				public static LocString LEGEND4 = "<b>CIRCUIT POWER</b>";

				public static LocString LEGEND5 = "Stable";

				public static LocString LEGEND6 = "Straining";

				public static LocString LEGEND7 = "Broken";

				public class TOOLTIPS
				{
					public static LocString LEGEND2 = "Uses <style=\"power\">Power</style> from circuit";

					public static LocString LEGEND3 = "Generates <style=\"power\">Power</style> for circuit";

					public static LocString LEGEND5 = "Optimal <style=\"power\">Power</style> usage";

					public static LocString LEGEND6 = "Requires more <style=\"power\">Power</style>";

					public static LocString LEGEND7 = "System overloaded";
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

				public static LocString DIAGRAM_BEFORE_ARROW = "Liquid flows from Output";

				public static LocString DIAGRAM_AFTER_ARROW = "Intake via pipes";

				public class TOOLTIPS
				{
					public static LocString CONNECTED = "Connected to a <style=\"LiquidPiping\">Liquid Pipe</style>";

					public static LocString DISCONNECTED = "Not connected to a <style=\"LiquidPiping\">Liquid Pipe</style>";

					public static LocString CONSUMER = "Point where a building sends liquid into pipes\n* Must be connected to a <style=\"LiquidDestination\">Destination</style>";

					public static LocString FILTERED = "Point where a building sends one filtered liquid into pipes\n* Must be connected to a <style=\"LiquidDestination\">Intake</style>";

					public static LocString PRODUCER = "Point where a building receives liquid from pipes\n* Must be connected to a <style=\"LiquidSource\">Output</style>";

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

				public static LocString DIAGRAM_BEFORE_ARROW = "Gas flows from Output";

				public static LocString DIAGRAM_AFTER_ARROW = "Intake via pipes";

				public class TOOLTIPS
				{
					public static LocString CONNECTED = "Connected to a <style=\"GasPiping\">Gas Pipe</style>";

					public static LocString DISCONNECTED = "Not connected to a <style=\"GasPiping\">Gas Pipe</style>";

					public static LocString CONSUMER = "Point where a building sends gas into pipes\n* Must be connected to a <style=\"GasDestination\">Destination</style>";

					public static LocString PRODUCER = "Point where a building receives gas from pipes\n* Must be connected to a <style=\"GasSource\">Source</style>";

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

				public static LocString CLUTTER = "Clutter";

				public static LocString LIGHTING = "Lighting";

				public class TOOLTIPS
				{
					public static LocString LOWDECOR = "Areas with insufficient decor values";

					public static LocString HIGHDECOR = "Areas with sufficient decor values";
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
		}

		public class PRIORITYSCREEN
		{
			public static LocString BUILDMENUPRIORITYTOOLTIP = "Set construction priority level\n------------------\nPriority 1: Lowest\nPriority 9: Highest";

			public static LocString TOOLPRIORITYTOOLTIP = "Set priority level for the currently selected tool\n------------------\nPriority 1: Lowest\nPriority 9: Highest";

			public static LocString USERMENUPRIORITYTOOLTIP = "Set priority level for the currently selected object\n------------------\nPriority 1: Lowest\nPriority 9: Highest";
		}

		public class UISIDESCREENS
		{
			public class TREEFILTERABLESIDESCREEN
			{
				public static LocString TITLE = "Element Filter";

				public static LocString ALLBUTTON = "All";

				public static LocString ALLBUTTONTOOLTIP = "Store all materials in this storage";

				public static LocString CATEGORYBUTTONTOOLTIP = "Allow {0} materials to be stored here";

				public static LocString MATERIALBUTTONTOOLTIP = "Add or remove this material from storage";

				public static LocString ONLYALLOWTRANSPORTITEMSBUTTON = "Sweep Only";

				public static LocString ONLYALLOWTRANSPORTITEMSBUTTONTOOLTIP = "If enabled, only objects marked for sweeping will be stored here";
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

				public static LocString SELECTRECIPE = "Select a recipe to Fabricate";

				public static LocString COST = "<b>Ingredients:</b>\n";

				public static LocString RESULTEFFECTS = "<b>Effects:</b>";

				public static LocString KG = "- {0}: {1}\n";

				public static LocString INFORMATION = "INFORMATION";

				public static LocString CANCEL = "Cancel";

				public static LocString RECIPERQUIREMENT = "<style={0}>{1}</style>: <style=\"consumed\">-{2}</style>";

				public class EFFECTS
				{
					public static LocString OXYGENTANK = "Surface <style=\"equipment\">Oxygen Tank</style> (<style=\"produced\">{0}</style>)";

					public static LocString OXYGENTANKUNDERWATER = "Underwater <style=\"equipment\">Oxygen Tank</style> (<style=\"produced\">{0}</style>)";

					public static LocString RESEARCHPOINT = "<style=\"research\">{0}</style>: <style=\"produced\">+1</style>";
				}
			}

			public class PLANTERSIDESCREEN
			{
				public static LocString PLANTREQUIREMENTS = "<b>Requirements</b>:\n";

				public static LocString GROWTHTIME = "Growth: {0}";

				public static LocString INITIALGROWTHTIME = "Initial Growth: {0}";

				public static LocString REGROWTHTIME = "Regrowth: {0}";

				public static LocString REQUIRESFERTILIZER = "<style=\"solid\">Fertilizer</style>";

				public static LocString PLANTEFFECTS = "<b>Effects</b>:\n";

				public static LocString NUMBEROFHARVESTS = "Max Harvests: {0}";

				public static LocString YIELD = "Yield: {0} <style=\"food\">{1}</style>";

				public static LocString YIELDPERHARVEST = "Yield: {0} <style=\"food\">{1}</style> per harvest";

				public static LocString TOTALHARVESTCALORIESWITHPERUNIT = "<style=\"produced\">{0}</style> (<style=\"produced\">{1}</style> per unit)";

				public static LocString TOTALHARVESTCALORIES = "<style=\"produced\">{0}</style>";

				public static LocString SELECTSEED_DESC = "Select a seed to plant";

				public static LocString SELECTSEED_TITLE = "Select seed";
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
				public static LocString TITLE = "Research Station";

				public static LocString NOSELECTEDRESEARCH = "NO RESEARCH SELECTED";

				public static LocString OPENRESEARCHBUTTON = "Open Research";
			}

			public class ASSIGNABLESIDESCREEN
			{
				public static LocString TITLE = "Assign {0}";

				public static LocString NEEDSREGION = "This {0} needs to be inside a {1} region";

				public static LocString NEEDSREGIONS = "This {0} needs to be inside one of the following regions: {1}";

				public static LocString ASSIGNEDTO = "Assigned to: {0}";

				public static LocString UNASSIGNED = "Unassigned";

				public static LocString GENERAL_CURRENTASSIGNED = "(Owner)";
			}

			public class EQUIPPABLESIDESCREEN
			{
				public static LocString TITLE = "Assign Duplicant";

				public static LocString CURRENTLY_EQUIPPED = "Currently Equipped:\n{0}";

				public static LocString NONE_EQUIPPED = "None";
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
				public static LocString TITLE = "Battery Threshold";

				public static LocString CURRENT_THRESHOLD = "Current Threshold: {0}%";

				public static LocString TOOLTIP = "Adjust how much wattage batteries should use before reactivating this generator";
			}

			public class TIMEDSWITCHSIDESCREEN
			{
				public static LocString TITLE = "Timed Power Switch";

				public static LocString ONTIME = "On Time:";

				public static LocString OFFTIME = "Off Time:";

				public static LocString TIMETODEACTIVATE = "Time until deactivation: {0}";

				public static LocString TIMETOACTIVATE = "Time until activation: {0}";

				public static LocString WARNING = "Timed Switches need to be connected to work";

				public static LocString CURRENTSTATE = "Current State:";

				public static LocString ON = "On";

				public static LocString OFF = "Off";
			}

			public class TEMPERATURESWITCHSIDESCREEN
			{
				public static LocString TITLE = "Temperature Power Switch";

				public static LocString CURRENTTEMPERATURE = "Current Temperature:\n{0}";

				public static LocString ACTIVATEIF = "Activate if:";

				public static LocString COLDERBUTTON = "Colder than";

				public static LocString WARMERBUTTON = "Warmer than";

				public static LocString WARNING = "Thermo Switches must be connected to a <style=\"power\">Power</style> grid";
			}
		}

		public class USERMENUACTIONS
		{
			public class CLEANTOILET
			{
				public static LocString NAME = "Clean Toilet";

				public static LocString TOOLTIP = "Remove waste from this toilet";
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

				public static LocString TOOLTIP_OFF = "Cancel dig order";
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
				public static LocString NAME = "Dig Up";

				public static LocString TOOLTIP = "Convert this <style=\"plant\">Plant</style> into a <style=\"seed\">Seed</style>";
			}

			public class CANCELUPROOT
			{
				public static LocString NAME = "Cancel Dig Up";

				public static LocString TOOLTIP = "Cancel this dig up order";
			}

			public class HARVEST
			{
				public static LocString NAME = "Harvest";

				public static LocString TOOLTIP = "Harvest <style=\"food\">Food</style> from this <style=\"plant\">Plant</style>";

				public static LocString TOOLTIP_DISABLED = "This <style=\"plant\">Plant</style> must reach maturity before being harvested";
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

				public static LocString TOOLTIP = "Take out all resources currently used by this building";
			}

			public class DEMOLISH
			{
				public static LocString NAME = "Deconstruct";

				public static LocString TOOLTIP = "Demolish this building\n------------------\nRefunds all resources";

				public static LocString NAME_OFF = "Cancel Deconstruct";

				public static LocString TOOLTIP_OFF = "Cancel this deconstruct order";
			}

			public class SELECTRESEARCH
			{
				public static LocString NAME = "Select Research";

				public static LocString TOOLTIP = "Choose a technology from the research tree";
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

				public static LocString TOOLTIP = "Remove materials from this storage region";

				public static LocString NAME_OFF = "Cancel Empty";

				public static LocString TOOLTIP_OFF = "Cancel this empty order";
			}

			public class CLEAR
			{
				public static LocString NAME = "Sweep";

				public static LocString TOOLTIP = "Move this object into storage";

				public static LocString NAME_OFF = "Cancel Sweeping";

				public static LocString TOOLTIP_OFF = "Cancel this sweep order";
			}

			public class QUARANTINE
			{
				public static LocString NAME = "Quarantine";

				public static LocString TOOLTIP = "Isolate this Duplicant";

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

			public class MANUALCONTROL
			{
				public static LocString NAME = "Manual Control";

				public static LocString TOOLTIP = "Manually control this Duplicant\nControlled Duplicants cannot perform tasks unless ordered";

				public static LocString NAME_OFF = "Release Control";

				public static LocString TOOLTIP_OFF = "Release control of this Duplicant";
			}

			public class MOVETOLOCATION
			{
				public static LocString NAME = "Move To";

				public static LocString TOOLTIP = "Tell this Duplicant to move to a specific spot";
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
			}

			public class BUILD
			{
				public static LocString NAME = "Build {0}";

				public static LocString TOOLNAME = "Build tool";

				public static LocString TOOLACTION = "CLICK TO BUILD";

				public static LocString TOOLACTION_DRAG = "DRAG TO BUILD";
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

				public static LocString TOOLACTION = "DRAG TO HARVEST";
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
			}
		}

		public class DETAILTABS
		{
			public class STATS
			{
				public static LocString NAME = "Stats";

				public static LocString TOOLTIP = "View this Duplicant's attributes, traits, and diseases";

				public static LocString GROUPNAME_ATTRIBUTES = "ATTRIBUTES";

				public static LocString GROUPNAME_TRAITS = "TRAITS";
			}

			public class SIMPLEINFO
			{
				public static LocString NAME = "Info";

				public static LocString TOOLTIP = "Status and basic information";

				public static LocString GROUPNAME_STATUS = "STATUS";

				public static LocString GROUPNAME_DESCRIPTION = "INFORMATION";

				public static LocString GROUPNAME_NEEDS = "NEEDS";

				public static LocString GROUPNAME_RESEARCH = "RESEARCH";
			}

			public class DETAILS
			{
				public static LocString NAME = "Details";

				public static LocString TOOLTIP = "More information";

				public static LocString GROUPNAME_DETAILS = "DETAILS";

				public static LocString GROUPNAME_CONTENTS = "CONTENTS";

				public static LocString GROUPNAME_MINION_CONTENTS = "CARRIED ITEMS";

				public static LocString STORAGE_EMPTY = "Nothing";
			}

			public class ENERGYCONSUMER
			{
				public static LocString NAME = "Energy";

				public static LocString TOOLTIP = "View how much <style=\"power\">Power</style> this building consumes";
			}

			public class ENERGYWIRE
			{
				public static LocString NAME = "Energy";

				public static LocString TOOLTIP = "View this wire's network";
			}

			public class ENERGYGENERATOR
			{
				public static LocString NAME = "Energy";

				public static LocString TOOLTIP = "Monitor the <style=\"power\">Power</style> this building is generating";

				public static LocString CIRCUITOVERVIEW = "CIRCUIT OVERVIEW";

				public static LocString GENERATORS = "POWER GENERATORS";

				public static LocString CONSUMERS = "POWER CONSUMERS";

				public static LocString BATTERIES = "BATTERIES";

				public static LocString DISCONNECTED = "Not connected to an electrical circuit";

				public static LocString NOGENERATORS = "No generators on this circuit";

				public static LocString NOCONSUMERS = "No consumers on this circuit";

				public static LocString NOBATTERIES = "No batteries on this circuit";

				public static LocString AVAILABLE_JOULES = "<style=\"power\">Power</style> stored: {0}";

				public static LocString AVAILABLE_JOULES_TOOLTIP = "Amount of <style=\"power\">Power</style> stored in batteries";

				public static LocString WATTAGE_GENERATED = "<style=\"power\">Power</style> produced: {0}";

				public static LocString WATTAGE_GENERATED_TOOLTIP = "The total amount of <style=\"power\">Power</style> generated by this circuit";

				public static LocString WATTAGE_CONSUMED = "<style=\"power\">Power</style> consumed: {0}";

				public static LocString WATTAGE_CONSUMED_TOOLTIP = "The total amount of <style=\"power\">Power</style> used by this circuit";

				public static LocString POTENTIAL_WATTAGE_CONSUMED = "Potential <style=\"power\">Power</style> consumed: {0}";

				public static LocString POTENTIAL_WATTAGE_CONSUMED_TOOLTIP = "The total amount of <style=\"power\">Power</style> that can be used by this circuit if all connected buildings are active";
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

				public static LocString TOOLTIP = "View this Duplicant's inventory";

				public static LocString GROUPNAME_ROOMS = "ROOMS";

				public static LocString GROUPNAME_OWNABLE = "EQUIPMENT";

				public static LocString GROUPNAME_EQUIPMENT = "CARRIED ITEMS";

				public static LocString UNASSIGNED = "Unassigned";

				public static LocString UNASSIGNED_TOOLTIP = "This Duplicant has not been assigned a {0}";

				public static LocString ASSIGNED_TOOLTIP = "This Duplicant has been assigned a {0}";

				public static LocString NOEQUIPMENT = "No equipment";

				public static LocString NOEQUIPMENT_TOOLTIP = "This Duplicant has not been equipped with a <style=\"equipment\">Tool</style>";

				public static LocString EQUIPMENT_TOOLTIP = "This Duplicant has been equipped with a <style=\"equipment\">{0}</style>";
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

			public static LocString ASSIGNEDDUPLICANT = "Duplicant assignment";

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

			public static LocString REFILLOXYGENTANK = "Refills <style=\"equipment\">Exosuit Oxygen Tank</style>";

			public static LocString DUPLICANTMOVEMENTBOOST = "Runspeed: <style=\"produced\">+{0}</style>";

			public static LocString STRESSREDUCEDPERMINUTE = "<style=\"stress\">Stress</style>: <style=\"consumed\">{0} per minute</style>";

			public static LocString REMOVESEFFECTSUBTITLE = "Cures";

			public static LocString REMOVEDEFFECT = "<style=\"disease\">{0}</style>";

			public static LocString GASCOOLING = "<style=\"heat\">Cooling factor: </style>: <style=\"produced\">{0}</style>";

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

				public static LocString REQUIRESSEED = "Must receive a seed from a dug out plant";

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

				public static LocString REFILLOXYGENTANK = "Refills Exosuit Oxygen Tanks with oxygen for reuse";

				public static LocString DUPLICANTMOVEMENTBOOST = "Duplicants walk {0} faster on this tile";

				public static LocString STRESSREDUCEDPERMINUTE = "Removes {0} of Duplicants' Stress for every uninterrupted minute of use";

				public static LocString REMOVESEFFECTSUBTITLE = "Use of this building will remove the listed effects";

				public static LocString REMOVEDEFFECT = "{0}";

				public static LocString GASCOOLING = "Reduces the temperature of piped gases by {0}";
			}
		}

		public class GAMEOBJECTEFFECTS
		{
			public static LocString CALORIES = "<style=\"produced\">+{0}</style>";

			public static LocString FORGAVEATTACKER = "Forgiveness";

			public static LocString FRIEDMUSHBARREMOVESDISEASE = "Removes <style=\"disease\">Disease</style> chance from <style=\"food\">Mush Bar</style>";
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
			public class MASS
			{
				public static LocString NAME = "Mass";

				public static LocString TOOLTIP = "This object has a mass of {0}";
			}

			public class SHC
			{
				public static LocString NAME = "Specific Heat Capacity";

				public static LocString TOOLTIP = "Specific heat capacity is the amount of energy required to raise this object's temperature";
			}

			public class THERMALCONDUCTIVITY
			{
				public static LocString NAME = "Thermal Conductivity";

				public static LocString TOOLTIP = "Thermal conductivity indicates how well this object conducts <style=\"heat\">Heat</style>";
			}

			public class MELTINGPOINT
			{
				public static LocString NAME = "Melting Point";

				public static LocString TOOLTIP = "Melting point describes the temperature at which this object will melt into liquid";
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

			public static LocString SHUFFLETOOLTIP = "Reshuffle this dupe for a different one";

			public static LocString BACK = "BACK";

			public static LocString CONFIRMATIONTITLE = "Reject Duplicants?";

			public static LocString CONFIRMATIONBODY = "Are you sure you want to reject all Duplicants?\n\nIt won't hurt their feelings, but it'll be awhile before the Printing Pod can print again.";
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
