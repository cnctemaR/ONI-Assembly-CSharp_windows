using System;

namespace STRINGS
{
	public class DUPLICANTS
	{
		public static LocString RACE_PREFIX = "Race: {0}";

		public static LocString RACE = "Duplicant";

		public class STATS
		{
			public class MORALE
			{
				public static LocString NAME = "<DO NOT TRANSLATE>";

				public static LocString TOOLTIP = string.Empty;
			}

			public class BREATH
			{
				public static LocString NAME = "Breath";

				public static LocString TOOLTIP = "Breath\n----------\nA Duplicant will begin suffocating at 0%";
			}

			public class STAMINA
			{
				public static LocString NAME = "Stamina";

				public static LocString TOOLTIP = "Stamina\n----------\nA Duplicant will pass out from fatigue at 0%";
			}

			public class CALORIES
			{
				public static LocString NAME = "Calories";

				public static LocString TOOLTIP = "Calories\n----------\nThis Duplicant has {0} of stored energy they can burn before starving";
			}

			public class TEMPERATURE
			{
				public static LocString NAME = "Body Temperature";

				public static LocString TOOLTIP = "Body Temperature\n----------\nA healthy Duplicant's body temperature is {1}";
			}

			public class EXTERNALTEMPERATURE
			{
				public static LocString NAME = "External Temperature";

				public static LocString TOOLTIP = "External Temperature\n----------\nThis Duplicant's immediate environment is {0}";
			}

			public class DECOR
			{
				public static LocString NAME = "Decor";

				public static LocString TOOLTIP = "Decor\n----------\nA Duplicant will gain <style=\"stress\">Stress</style> while in rooms with lower <style=\"decor\">Decor</style> values than their expectations";
			}

			public class STRESS
			{
				public static LocString NAME = "Stress";

				public static LocString TOOLTIP = "Stress\n----------\nA Duplicant will have a Nervous Breakdown at 100% <style=\"stress\">Stress</style>";
			}

			public class TOXICITY
			{
				public static LocString NAME = "<DO NOT TRANSLATE>";

				public static LocString TOOLTIP = string.Empty;
			}

			public class BLADDER
			{
				public static LocString NAME = "Bladder";

				public static LocString TOOLTIP = "Bladder\n----------\nA Duplicant will make a mess at 100% if no bathrooms are available";
			}

			public class HITPOINTS
			{
				public static LocString NAME = "Health";

				public static LocString TOOLTIP = "Health\n----------\nA combatant will become incapacitated at zero <style=\"Health\">Health</style>";
			}
		}

		public class CHORES
		{
			public class DIE
			{
				public static LocString NAME = "Die";

				public static LocString STATUS = "Dying";
			}

			public class ENTOMBED
			{
				public static LocString NAME = "Entombed";

				public static LocString STATUS = "Entombed";
			}

			public class HEALCRITICAL
			{
				public static LocString NAME = "Heal";

				public static LocString STATUS = "Healing";
			}

			public class BEINCAPACITATED
			{
				public static LocString NAME = "BeIncapacitated";

				public static LocString STATUS = "Dying";
			}

			public class DEBUGGOTO
			{
				public static LocString NAME = "DebugGoTo";

				public static LocString STATUS = "DebugGoTo";
			}

			public class MOVETO
			{
				public static LocString NAME = "Move To";

				public static LocString STATUS = "Moving to location";
			}

			public class DROPUNUSEDINVENTORY
			{
				public static LocString NAME = "Drop Unused Inventory";

				public static LocString STATUS = "Dropping unused inventory";
			}

			public class PEE
			{
				public static LocString NAME = "Pee";

				public static LocString STATUS = "Bio Break";
			}

			public class STRESSVOMIT
			{
				public static LocString NAME = "StressVomit";

				public static LocString STATUS = "Stress Vomiting";
			}

			public class EMOTEHIGHPRIORITY
			{
				public static LocString NAME = "EmoteHighPriority";

				public static LocString STATUS = "High Priority Emote";
			}

			public class MANUALCONTROLPRIORITIZECHORE
			{
				public static LocString NAME = "ManualControlGoTo";

				public static LocString STATUS = "Moving to position";
			}

			public class MANUALCONTROLGOTO
			{
				public static LocString NAME = "ManualControlGoTo";

				public static LocString STATUS = "Moving to position";
			}

			public class MANUALCONTROLIDLE
			{
				public static LocString NAME = "Manual Idle";

				public static LocString STATUS = "Idle";
			}

			public class FLEE
			{
				public static LocString NAME = "Flee";

				public static LocString STATUS = "Fleeing";
			}

			public class MENTALBREAK
			{
				public static LocString NAME = "MentalBreak";

				public static LocString STATUS = "Mental break";
			}

			public class RECOVERBREATH
			{
				public static LocString NAME = "RecoverBreath";

				public static LocString STATUS = "Recovering breath";
			}

			public class MOVETOQUARANTINE
			{
				public static LocString NAME = "MoveToQuarantine";

				public static LocString STATUS = "Moving to quarantine";
			}

			public class MOVETOLOCATION
			{
				public static LocString NAME = "MoveToLocation";

				public static LocString STATUS = "Move to location";
			}

			public class ATTACK
			{
				public static LocString NAME = "Attack";

				public static LocString STATUS = "Attacking";
			}

			public class USETOILET
			{
				public static LocString NAME = "Use Toilet";

				public static LocString STATUS = "Going to use toilet";
			}

			public class WASHHANDS
			{
				public static LocString NAME = "WashHands";

				public static LocString STATUS = "Washing hands";
			}

			public class EAT
			{
				public static LocString NAME = "Eat";

				public static LocString STATUS = "Going to eat";
			}

			public class PRIORITIZECHORE
			{
				public static LocString NAME = "Prioritize Chore";

				public static LocString STATUS = "Prioritize chore";
			}

			public class VOMIT
			{
				public static LocString NAME = "Vomit";

				public static LocString STATUS = "Vomiting";
			}

			public class SLEEP
			{
				public static LocString NAME = "Sleep";

				public static LocString STATUS = "Sleeping";
			}

			public class SLEEPONFLOOR
			{
				public static LocString NAME = "SleepOnFloor";

				public static LocString STATUS = "Sleeping";
			}

			public class TAKEMEDICINE
			{
				public static LocString NAME = "TakeMedicine";

				public static LocString STATUS = "Taking medicine";
			}

			public class DOCTOR
			{
				public static LocString NAME = "Doctor";

				public static LocString STATUS = "Nursing";
			}

			public class DELIVERFOOD
			{
				public static LocString NAME = "DeliverFood";

				public static LocString STATUS = "Delivering food";
			}

			public class FETCHCRITICAL
			{
				public static LocString NAME = "Deliver";

				public static LocString STATUS = "Delivering";
			}

			public class SHOWER
			{
				public static LocString NAME = "Shower";

				public static LocString STATUS = "Showering";
			}

			public class SIGH
			{
				public static LocString NAME = "Sigh";

				public static LocString STATUS = "Sighing";
			}

			public class RESTDUETODISEASE
			{
				public static LocString NAME = "RestDueToDisease";

				public static LocString STATUS = "Resting";
			}

			public class HEAL
			{
				public static LocString NAME = "Heal";

				public static LocString STATUS = "Healing";
			}

			public class STRESSACTINGOUT
			{
				public static LocString NAME = "StressActingOut";

				public static LocString STATUS = "Lashing out";
			}

			public class RELAX
			{
				public static LocString NAME = "Relax";

				public static LocString STATUS = "Relaxing";
			}

			public class EQUIP
			{
				public static LocString NAME = "Equip";

				public static LocString STATUS = "Moving to equip";
			}

			public class RECHARGE
			{
				public static LocString NAME = "Recharge";

				public static LocString STATUS = "Rechargin equipment";
			}

			public class UNEQUIP
			{
				public static LocString NAME = "Unequip";

				public static LocString STATUS = "Moving to unequip";
			}

			public class WARMUP
			{
				public static LocString NAME = "Warmup";

				public static LocString STATUS = "Going to warmup";
			}

			public class EMPTYSTORAGE
			{
				public static LocString NAME = "Emptying Storage";

				public static LocString STATUS = "Going to empty storage";
			}

			public class UPGRADE
			{
				public static LocString NAME = "Upgrade";

				public static LocString STATUS = "Going to upgrade";
			}

			public class ART
			{
				public static LocString NAME = "Arting";

				public static LocString STATUS = "Going to decorate";
			}

			public class MOP
			{
				public static LocString NAME = "Mop";

				public static LocString STATUS = "Going to mop";
			}

			public class RELOCATE
			{
				public static LocString NAME = "Relocate";

				public static LocString STATUS = "Going to relocate";
			}

			public class TOGGLE
			{
				public static LocString NAME = "Toggle";

				public static LocString STATUS = "Going to toggle";
			}

			public class RESCUEINCAPACITATED
			{
				public static LocString NAME = "Toggle";

				public static LocString STATUS = "Rescuing friend";
			}

			public class REPAIR
			{
				public static LocString NAME = "Repair";

				public static LocString STATUS = "Going to repair";
			}

			public class DECONSTRUCT
			{
				public static LocString NAME = "Deconstruct";

				public static LocString STATUS = "Going to deconstruct";
			}

			public class RESEARCH
			{
				public static LocString NAME = "Research";

				public static LocString STATUS = "Going to research";
			}

			public class GENERATEPOWER
			{
				public static LocString NAME = "Generate Power";

				public static LocString STATUS = "Going to generate power";
			}

			public class HARVEST
			{
				public static LocString NAME = "Harvest";

				public static LocString STATUS = "Going to harvest";
			}

			public class UPROOT
			{
				public static LocString NAME = "Dig Up";

				public static LocString STATUS = "Going to dig up";
			}

			public class CLEANTOILET
			{
				public static LocString NAME = "Clean";

				public static LocString STATUS = "Going to clean";
			}

			public class LIQUIDCOOLEDFAN
			{
				public static LocString NAME = "Use Fan";

				public static LocString STATUS = "Going to use fan";
			}

			public class COOK
			{
				public static LocString NAME = "Cooking";

				public static LocString STATUS = "Going to cook";
			}

			public class COOKFETCH
			{
				public static LocString NAME = "Deliver";

				public static LocString STATUS = "Delivering";
			}

			public class MUSH
			{
				public static LocString NAME = "Mushing";

				public static LocString STATUS = "Going to mush";
			}

			public class MUSHFETCH
			{
				public static LocString NAME = "Deliver";

				public static LocString STATUS = "Delivering";
			}

			public class COMPOSTWORKABLE
			{
				public static LocString NAME = "Compost";

				public static LocString STATUS = "Going to compost";
			}

			public class FLIPCOMPOST
			{
				public static LocString NAME = "Flip";

				public static LocString STATUS = "Going to flip compost";
			}

			public class FABRICATE
			{
				public static LocString NAME = "Fabricating";

				public static LocString STATUS = "Going to fabricate";
			}

			public class FABRICATEFETCH
			{
				public static LocString NAME = "Deliver";

				public static LocString STATUS = "Delivering";
			}

			public class BUILD
			{
				public static LocString NAME = "Build";

				public static LocString STATUS = "Going to build";
			}

			public class BUILDDIG
			{
				public static LocString NAME = "Dig";

				public static LocString STATUS = "Going to dig";
			}

			public class BUILDFETCH
			{
				public static LocString NAME = "Deliver";

				public static LocString STATUS = "Delivering";
			}

			public class DIG
			{
				public static LocString NAME = "Dig";

				public static LocString STATUS = "Going to dig";
			}

			public class FETCH
			{
				public static LocString NAME = "Deliver";

				public static LocString STATUS = "Delivering";
			}

			public class TRANSPORT
			{
				public static LocString NAME = "Sweep";

				public static LocString STATUS = "Going to sweep";
			}

			public class MOVETOSAFETY
			{
				public static LocString NAME = "MoveToSafety";

				public static LocString STATUS = "Moving to safety";
			}

			public class IDLE
			{
				public static LocString NAME = "Idle";

				public static LocString STATUS = "Idle";
			}
		}

		public class CHOREGROUPS
		{
			public class COMBAT
			{
				public static LocString NAME = "Combat";

				public static LocString DESC = "Attack wild creatures.";
			}

			public class COOK
			{
				public static LocString NAME = "Cook";

				public static LocString DESC = "Produce <style=\"food\">Food</style> for Duplicants with food preparation buildings.";
			}

			public class ART
			{
				public static LocString NAME = "Art";

				public static LocString DESC = "Sculpt or paint art pieces to improve colony <style=\"decor\">Decor</style>.";
			}

			public class COMPOST
			{
				public static LocString NAME = "Compost";

				public static LocString DESC = "Deliver <style=\"solid\">Contaminated Dirt</style> to compost to produce <style=\"solid\">Fertilizer</style>.";
			}

			public class RESEARCH
			{
				public static LocString NAME = "Research";

				public static LocString DESC = "Work <style=\"research\">Research Stations</style> to unlock new technologies.";
			}

			public class GENERATEPOWER
			{
				public static LocString NAME = "Generate";

				public static LocString DESC = "Run on manual generators to produce <style=\"power\">Power</style>.";
			}

			public class HARVEST
			{
				public static LocString NAME = "Harvest";

				public static LocString DESC = "Pick the <style=\"food\">Food</style> from <style=\"plant\">Plants</style>.";
			}

			public class BUILD
			{
				public static LocString NAME = "Build";

				public static LocString DESC = "Construct new buildings.";
			}

			public class DELIVER
			{
				public static LocString NAME = "Deliver";

				public static LocString DESC = "Transport materials to critical utility buildings and high priority storage.";
			}

			public class SWEEP
			{
				public static LocString NAME = "Sweep";

				public static LocString DESC = "Move materials to noncritical buildings and normal priority storage.";
			}

			public class DIG
			{
				public static LocString NAME = "Dig";

				public static LocString DESC = "Excavate raw materials.";
			}

			public class TOGGLE
			{
				public static LocString NAME = "Toggle";

				public static LocString DESC = "Manually enable, disable or adjust building switches and filters.";
			}
		}

		public class STATUSITEMS
		{
			public class GENERIC_DELIVER
			{
				public static LocString NAME = "Delivering resources to {Target}";

				public static LocString TOOLTIP = "This Duplicant is transporting materials to {Target}";
			}

			public class SLEEPING
			{
				public static LocString NAME = "Sleeping";

				public static LocString TOOLTIP = "This Duplicant is recovering stamina";
			}

			public class REDALERT
			{
				public static LocString NAME = "Red Alert";

				public static LocString TOOLTIP = "Your Duplicants will not eat, sleep, or take leisure time during a state of Red Alert";
			}

			public class LOWOXYGEN
			{
				public static LocString NAME = "Oxygen low";

				public static LocString TOOLTIP = "This Duplicant is working in a dangerously low <style=\"oxygen\">Oxygen</style> area";

				public static LocString NOTIFICATION_NAME = "Low <style=\"oxygen\">Oxygen</style> area entered";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are working in areas with low <style=\"oxygen\">Oxygen</style>:";
			}

			public class MENTALBREAK
			{
				public static LocString NAME = "Nervous breakdown";

				public static LocString TOOLTIP = "<style=\"stress\">Stress</style> has completely eroded this Duplicant's ability to function";
			}

			public class SEVEREWOUNDS
			{
				public static LocString NAME = "Severely injured";

				public static LocString TOOLTIP = "This Duplicant is badly hurt";

				public static LocString NOTIFICATION_NAME = "Severely injured";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are badly hurt and require medical attention";
			}

			public class INCAPACITATED
			{
				public static LocString NAME = "Incapacitated. Time until death: {TimeUntilDeath}";

				public static LocString TOOLTIP = "This Duplicant is near death! They need to be rescued quickly";

				public static LocString NOTIFICATION_NAME = "Incapacitated";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants need to be rescued before they kick the bucket";
			}

			public class BEDUNREACHABLE
			{
				public static LocString NAME = "Bed beyond reach";

				public static LocString TOOLTIP = "This Duplicant cannot reach their bed";

				public static LocString NOTIFICATION_NAME = "Bed beyond reach";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants cannot sleep because their bed is out of reach:";
			}

			public class COLD
			{
				public static LocString NAME = "Too cold";

				public static LocString TOOLTIP = "This Duplicant is about to freeze to death";

				public static LocString NOTIFICATION_NAME = "Too cold";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are about to freeze to death:";
			}

			public class DAILYRATIONLIMITREACHED
			{
				public static LocString NAME = "Daily calorie limit reached";

				public static LocString TOOLTIP = "This Duplicant has consumed their allotted <style=\"food\">Rations</style> for the day";

				public static LocString NOTIFICATION_NAME = "Daily calorie limit reached";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have consumed their allotted <style=\"food\">Rations</style> for the day:";
			}

			public class HOLDINGBREATH
			{
				public static LocString NAME = "Holding breath";

				public static LocString TOOLTIP = "This Duplicant cannot breathe in their current location";
			}

			public class RECOVERINGBREATH
			{
				public static LocString NAME = "Recovering breath";

				public static LocString TOOLTIP = "This Duplicant had to hold their breath for too long and needs a breather";
			}

			public class HOT
			{
				public static LocString NAME = "Too hot";

				public static LocString TOOLTIP = "This Duplicant is overheating";

				public static LocString NOTIFICATION_NAME = "Too hot";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are overheating:";
			}

			public class HUNGRY
			{
				public static LocString NAME = "Hungry";

				public static LocString TOOLTIP = "This Duplicant would love a snack";
			}

			public class POORDECOR
			{
				public static LocString NAME = "Drab decor";

				public static LocString TOOLTIP = "This Duplicant finds this room's lack of <style=\"decor\">Decor</style> depressing";
			}

			public class MANUALLYCONTROLLED
			{
				public static LocString NAME = "Manual control";

				public static LocString TOOLTIP = "This Duplicant is under manual control and cannot act independently";
			}

			public class MANUALCONTROLIDLE
			{
				public static LocString NAME = "Manual idle";

				public static LocString TOOLTIP = "This Duplicant is manually controlled and will be idle until given a task";
			}

			public class MANUALCONTROLGOTO
			{
				public static LocString NAME = "Manual movement";

				public static LocString TOOLTIP = "This Duplicant is manually controlled and has been ordered to a specific location";
			}

			public class MANUALCONTROLCHORE
			{
				public static LocString NAME = "Manually task";

				public static LocString TOOLTIP = "This Duplicant is manually controlled and has been ordered to perform a specific task";
			}

			public class NERVOUSBREAKDOWN
			{
				public static LocString NAME = "Nervous breakdown";

				public static LocString TOOLTIP = "<style=\"stress\">Stress</style> has completely eroded this Duplicant's ability to function";

				public static LocString NOTIFICATION_NAME = "Nervous breakdown";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have cracked under the <style=\"stress\">Stress</style> and need assistance:";
			}

			public class STRESSED
			{
				public static LocString NAME = "Stressed";

				public static LocString TOOLTIP = "This Duplicant is feeling the <style=\"stress\">Stress</style>";

				public static LocString NOTIFICATION_NAME = "Stressed";

				public static LocString NOTIFICATION_TOOLTIP = "The colony is troubled by excess <style=\"stress\">Stress</style>.\nDecorate your base and build better amenities to help these Duplicants unwind:";
			}

			public class NORATIONSAVAILABLE
			{
				public static LocString NAME = "No food available";

				public static LocString TOOLTIP = "There is nothing in the colony for this Duplicant to eat";

				public static LocString NOTIFICATION_NAME = "No food available";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have nothing to eat:";
			}

			public class QUARANTINEAREAUNASSIGNED
			{
				public static LocString NAME = "No quarantine zone assigned";

				public static LocString TOOLTIP = "This Duplicant has not been assigned a room";

				public static LocString NOTIFICATION_NAME = "No quarantine zone assigned";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have not been assigned a room:";
			}

			public class QUARANTINEAREAUNREACHABLE
			{
				public static LocString NAME = "Unreachable quarantine zone";

				public static LocString TOOLTIP = "This Duplicant cannot reach their quarantine zone";

				public static LocString NOTIFICATION_NAME = "Unreachable quarantine zone";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants cannot reach their assigned quarantine zones:";
			}

			public class QUARANTINED
			{
				public static LocString NAME = "Quarantined";

				public static LocString TOOLTIP = "This Duplicant has been isolated from the colony";
			}

			public class RATIONSUNREACHABLE
			{
				public static LocString NAME = "Cannot reach food";

				public static LocString TOOLTIP = "There is <style=\"food\">Food</style> in the colony but this Duplicant cannot reach it";

				public static LocString NOTIFICATION_NAME = "Food beyond reach";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants cannot access the colony's <style=\"food\">Food</style>:";
			}

			public class ROTTEN
			{
				public static LocString NAME = "Rotten";

				public static LocString TOOLTIP = "Gross!";
			}

			public class STARVING
			{
				public static LocString NAME = "Starving";

				public static LocString TOOLTIP = "This Duplicant needs <style=\"food\">Food</style>!";

				public static LocString NOTIFICATION_NAME = "Starvation";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are starving and need <style=\"food\">Food</style>:";
			}

			public class ENTOMBED
			{
				public static LocString NAME = "Entombed";

				public static LocString TOOLTIP = "This Duplicant needs someone to help dig them out!";

				public static LocString NOTIFICATION_NAME = "Entombed";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are trapped:";
			}

			public class EARLYMORNING
			{
				public static LocString NAME = "Jazzed to start the day";

				public static LocString TOOLTIP = "This Duplicant is an Early Bird and gets a burst of energy in the morning";
			}

			public class NIGHTTIME
			{
				public static LocString NAME = "Impassioned by the night";

				public static LocString TOOLTIP = "This Duplicant is a Night Owl and gets a burst of energy at night";
			}

			public class SUFFOCATING
			{
				public static LocString NAME = "Suffocating";

				public static LocString TOOLTIP = "This Duplicant cannot breathe!";

				public static LocString NOTIFICATION_NAME = "Suffocating";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants cannot find <style=\"oxygen\">Oxygen</style>:";
			}

			public class TIRED
			{
				public static LocString NAME = "Tired";

				public static LocString TOOLTIP = "This Duplicant could use a nice nap";
			}

			public class IDLE
			{
				public static LocString NAME = "Idle";

				public static LocString TOOLTIP = "This Duplicant cannot reach any pending job tasks";

				public static LocString NOTIFICATION_NAME = "Idle";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants cannot reach any pending job tasks:";
			}

			public class FIGHTING
			{
				public static LocString NAME = "In Combat";

				public static LocString TOOLTIP = "This Duplicant is attacking a creature";

				public static LocString NOTIFICATION_NAME = "Combat!";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have engaged a creature in combat:";
			}

			public class FLEEING
			{
				public static LocString NAME = "Fleeing";

				public static LocString TOOLTIP = "This Duplicant is trying to escape combat";

				public static LocString NOTIFICATION_NAME = "Fleeing!";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are trying to escape combat:";
			}

			public class DEAD
			{
				public static LocString NAME = "Dead: {Death}";

				public static LocString TOOLTIP = "This Duplicant definitely isn't sleeping";
			}

			public class LASHINGOUT
			{
				public static LocString NAME = "Lashing out";

				public static LocString TOOLTIP = "This Duplicant is breaking stuff to relieve <style=\"stress\">Stress</style>";

				public static LocString NOTIFICATION_NAME = "Lashing out";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants broke buildings to relieve <style=\"stress\">Stress</style>:";
			}

			public class MOVETOSUITNOTREQUIRED
			{
				public static LocString NAME = "Exiting <style=\"equipment\">Exosuit</style> area";

				public static LocString TOOLTIP = "This Duplicant is leaving an area where an <style=\"equipment\">Exosuit</style> was required";
			}

			public class DROPPINGUNUSEDINVENTORY
			{
				public static LocString NAME = "Dropping objects";

				public static LocString TOOLTIP = "This Duplicant is dropping what they're holding";
			}

			public class MOVINGTOSAFEAREA
			{
				public static LocString NAME = "Moving to safe area";

				public static LocString TOOLTIP = "This Duplicant is finding a safer place";
			}

			public class TOILETUNREACHABLE
			{
				public static LocString NAME = "Restroom unreachable";

				public static LocString TOOLTIP = "This Duplicant cannot access a functioning toilet";

				public static LocString NOTIFICATION_NAME = "Restroom out of reach";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants cannot access a functioning toilet:";
			}

			public class NOUSABLETOILETS
			{
				public static LocString NAME = "Restroom out of order";

				public static LocString TOOLTIP = "The only toilets in this Duplicant's reach are out of order";

				public static LocString NOTIFICATION_NAME = "Restrooms out of order";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants want to use a toilet that is out of order:";
			}

			public class NOTOILETS
			{
				public static LocString NAME = "No restrooms";

				public static LocString TOOLTIP = "There are no toilets available for this Duplicant";

				public static LocString NOTIFICATION_NAME = "No restrooms built";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are distressed by the lack of toilets:";
			}

			public class FULLBLADDER
			{
				public static LocString NAME = "Full bladder";

				public static LocString TOOLTIP = "This Duplicant would really appreciate a toilet";
			}

			public class STRESSFULLYEMPTYINGBLADDER
			{
				public static LocString NAME = "Making a mess";

				public static LocString TOOLTIP = "This Duplicant couldn't find a toilet and is super embarrassed";

				public static LocString NOTIFICATION_NAME = "Made a mess";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants could not find a toilet in time.\n\nUse the <color=#833A5FFF>MOP TOOL</color> [K] to clean up their mess:";
			}

			public class WASHINGHANDS
			{
				public static LocString NAME = "Washing hands";

				public static LocString TOOLTIP = "This Duplicant is <style=\"hygiene\">Cleaning</style> their hands";
			}

			public class SHOWERING
			{
				public static LocString NAME = "Showering";

				public static LocString TOOLTIP = "This Duplicant is getting squeaky <style=\"hygiene\">Clean</style>";
			}

			public class RELAXING
			{
				public static LocString NAME = "Relaxing";

				public static LocString TOOLTIP = "This Duplicant's just taking it easy";
			}

			public class VOMITING
			{
				public static LocString NAME = "Throwing up";

				public static LocString TOOLTIP = "This Duplicant has unceremoniously hurled as a result of a <style=\"disease\">Disease</style>";

				public static LocString NOTIFICATION_NAME = "Throwing up";

				public static LocString NOTIFICATION_TOOLTIP = "A <style=\"disease\">Disease</style> has caused these Duplicants to throw up:";
			}

			public class STRESSVOMITING
			{
				public static LocString NAME = "Stress vomiting";

				public static LocString TOOLTIP = "This Duplicant is releasing their <style=\"stress\">Stress</style> all over the floor";

				public static LocString NOTIFICATION_NAME = "Stress vomiting";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants became too <style=\"stress\">Stress</style> and it caused them to throw up:";
			}

			public class HASDISEASE
			{
				public static LocString NAME = "Feeling ill";

				public static LocString TOOLTIP = "This Duplicant has contracted a <style=\"disease\">Disease</style>";

				public static LocString NOTIFICATION_NAME = "Illness";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have contracted a <style=\"disease\">Disease</style>:";
			}

			public class BODYREGULATINGHEATING
			{
				public static LocString NAME = "Heating up at: {TempDelta}";

				public static LocString TOOLTIP = "This Duplicant is regulating their <style=\"heat\">Temperature</style>";
			}

			public class BODYREGULATINGCOOLING
			{
				public static LocString NAME = "Cooling down at: {TempDelta}";

				public static LocString TOOLTIP = "This Duplicant is regulating their <style=\"heat\">Temperature</style>";
			}

			public class BREATHINGO2
			{
				public static LocString NAME = "Inhaling {ConsumptionRate} O2";

				public static LocString TOOLTIP = "Duplicants require <style=\"oxygen\">Oxygen</style> to live";
			}

			public class EMITTINGCO2
			{
				public static LocString NAME = "Exhaling {EmittingRate} CO2";

				public static LocString TOOLTIP = "Duplicants breathe out <style=\"gas\">Carbon Dioxide</style>";
			}

			public class PICKUPDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class STOREDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class CLEARDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class STOREFORBUILDDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class STOREFORBUILDPRIORITIZEDDELIVERSTATUS
			{
				public static LocString NAME = "Allocating {Item} to {Target}";

				public static LocString TOOLTIP = "This Duplicant is delivering {Item} resources to a construction task to build {Target}";
			}

			public class BUILDDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class BUILDPRIORITIZEDSTATUS
			{
				public static LocString NAME = "Building {Target}";

				public static LocString TOOLTIP = "This Duplicant is constructing a {Target}";
			}

			public class FABRICATEDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class USEITEMDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class STOREPRIORITYDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class STORECRITICALDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class COMPOSTFLIPSTATUS
			{
				public static LocString NAME = "Going to flip compost";

				public static LocString TOOLTIP = "This Duplicant is going to flip the compost";
			}

			public class DECONSTRUCTDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class TOGGLEDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class EMPTYSTORAGEDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class HARVESTDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class SLEEPDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class EATDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class WARMUPDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class REPAIRDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class REPAIRWORKSTATUS
			{
				public static LocString NAME = "Repairing {Target}";

				public static LocString TOOLTIP = "This Duplicant is fixing the {Target}";
			}

			public class BREAKDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class BREAKWORKSTATUS
			{
				public static LocString NAME = "Breaking {Target}";

				public static LocString TOOLTIP = "This Duplicant is going totally bananas on the {Target}";
			}

			public class EQUIPDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class COOKDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class MUSHDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class PACIFYDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class RESCUEDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class RESCUEWORKSTATUS
			{
				public static LocString NAME = "Rescuing {Target}";

				public static LocString TOOLTIP = "This Duplicant is saving {Target} from certain peril!";
			}

			public class MOPDELIVERSTATUS
			{
				public static LocString NAME = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.NAME;

				public static LocString TOOLTIP = DUPLICANTS.STATUSITEMS.GENERIC_DELIVER.TOOLTIP;
			}

			public class DIGGING
			{
				public static LocString NAME = "Digging";

				public static LocString TOOLTIP = "This Duplicant is excavating raw resources";
			}

			public class EATING
			{
				public static LocString NAME = "Eating {Target}";

				public static LocString TOOLTIP = "This Duplicant is replenishing their <style=\"food\">Calorie</style> stores";
			}

			public class CLEANING
			{
				public static LocString NAME = "Cleaning {Target}";

				public static LocString TOOLTIP = "This Duplicant is <style=\"hygiene\">Cleaning Up</style> {Target}";
			}

			public class PICKINGUP
			{
				public static LocString NAME = "Picking up {Target}";

				public static LocString TOOLTIP = "This Duplicant is retrieving {Target}";
			}

			public class MOPPING
			{
				public static LocString NAME = "Mopping";

				public static LocString TOOLTIP = "This Duplicant is <style=\"hygiene\">Cleaning</style> up a nasty spill";
			}

			public class ARTING
			{
				public static LocString NAME = "Decorating";

				public static LocString TOOLTIP = "This Duplicant is expressing themselves artistically";
			}

			public class MUSHING
			{
				public static LocString NAME = "Mushing microbes";

				public static LocString TOOLTIP = "This Duplicant is trying to produce something edible";
			}

			public class COOKING
			{
				public static LocString NAME = "Cooking food";

				public static LocString TOOLTIP = "This Duplicant is cooking up something tasty";
			}

			public class RESEARCHING
			{
				public static LocString NAME = "Researching <style=\"research\">{Tech}</style>";

				public static LocString TOOLTIP = "This Duplicant is intently researching <style=\"research\">{Tech}</style> technology";
			}

			public class STORING
			{
				public static LocString NAME = "Storing {Item}";

				public static LocString TOOLTIP = "This Duplicant is putting {Item} in storage";
			}

			public class BUILDING
			{
				public static LocString NAME = "Building {Target}";

				public static LocString TOOLTIP = "This Duplicant is constructing a {Target}";
			}

			public class EQUIPPING
			{
				public static LocString NAME = "Equipping <style=\"equipment\">{Item}</style>";

				public static LocString TOOLTIP = "This Duplicant is equipping a <style=\"equipment\">{Item}</style>";
			}

			public class WARMINGUP
			{
				public static LocString NAME = "Warming up";

				public static LocString TOOLTIP = "This Duplicant got too <style=\"heat\">Cold</style> and is trying to warm up";
			}

			public class GENERATINGPOWER
			{
				public static LocString NAME = "Generating power";

				public static LocString TOOLTIP = "This Duplicant is using the {Target} to produce electrical <style=\"power\">Power</style>";
			}

			public class HARVESTING
			{
				public static LocString NAME = "Harvesting {Target}";

				public static LocString TOOLTIP = "This Duplicant is gathering resources from a {Target}";
			}

			public class UPROOTING
			{
				public static LocString NAME = "Digging up {Target}";

				public static LocString TOOLTIP = "This Duplicant is turning a {Target} into a seed";
			}

			public class EMPTYING
			{
				public static LocString NAME = "Emptying {Target}";

				public static LocString TOOLTIP = "This Duplicant is removing materials from the {Target}";
			}

			public class TOGGLING
			{
				public static LocString NAME = "Toggling {Target} switch";

				public static LocString TOOLTIP = "This Duplicant is toggling the {Target}'s switch";
			}

			public class DECONSTRUCTING
			{
				public static LocString NAME = "Deconstructing {Target}";

				public static LocString TOOLTIP = "This Duplicant is tearing down the {Target}";
			}

			public class FABRICATING
			{
				public static LocString NAME = "Fabricating {Item}";

				public static LocString TOOLTIP = "This Duplicant is making a {Item}";
			}

			public class CLEARING
			{
				public static LocString NAME = "Sweeping {Target}";

				public static LocString TOOLTIP = "This Duplicant is sweeping away {Target}";
			}
		}

		public class DISEASES
		{
			public static LocString CURED_POPUP = "Cured of <style=\"disease\">{0}</style>";

			public static LocString INFECTED_POPUP = "Became infected by <style=\"disease\">{0}</style>";

			public static LocString NOTIFICATION_TOOLTIP = "{0} was infected with <style=\"disease\">{1}</style> by {2}";

			public static LocString STATUS_ITEM_TOOLTIP = "{Symptoms}\n\nInfected by {InfectionSource}\nDuration remaining: {Duration}\nRemedies: <style=\"medicine\">{Cures}</style>";

			public static LocString NOMEDICINETAKEN = "None";

			public static LocString CURES = "Instantly cures <style=\"disease\">{0}</style>";

			public static LocString BOOSTSCURESPEED = "Heal speed (<style=\"disease\">{0}</style>): <style=\"produced\">+{1}</style>";

			public static LocString REDUCECURESPEED = "Heal speed (<style=\"disease\">{0}</style>): <style=\"consumed\">{1}</style>";

			public static LocString RECUPERATING = "Sleeping it off";

			public static LocString DISEASE_TRIGGER_EAT = "May cause <style=\"disease\">{Diseases}</style>";

			public class INFECTIONSOURCES
			{
				public static LocString TOXIC_AREA = "standing in a toxic area";

				public static LocString FOOD = "eating a {0}";
			}

			public class ALLDISEASES
			{
				public static LocString NAME = "All Diseases";
			}

			public class SAWCORPSOSIS
			{
				public static LocString NAME = "Existential Angst";

				public static LocString DESCRIPTION = "This Duplicant saw the corpse of a friend";
			}

			public class FOODPOISONING
			{
				public static LocString NAME = "Food Poisoning";

				public static LocString DESCRIPTION = "This Duplicant's last meal might have been past its prime";
			}

			public class DIARRHEA
			{
				public static LocString NAME = "Diarrhea";

				public static LocString DESCRIPTION = "This Duplicant's gut is giving them some trouble";
			}

			public class SPORES
			{
				public static LocString NAME = "The Spores";

				public static LocString DESCRIPTION = "This Duplicant is a walking fungal pod";
			}

			public class DWEEBCEPHALY
			{
				public static LocString NAME = "Brain Fog";

				public static LocString DESCRIPTION = "This Duplicant is experiencing some cognitive impairment";
			}

			public class LAZIBONITIS
			{
				public static LocString NAME = "The Lackadaisies";

				public static LocString DESCRIPTION = "This Duplicant really doesn't feel like working right now";
			}

			public class PUTRIDODOUR
			{
				public static LocString NAME = "Trench Stench";

				public static LocString DESCRIPTION = "The pungent odor wafting off this Duplicant is nauseating their peers";

				public static LocString CRINGE_EFFECT = "Smelled a putrid odor";
			}
		}

		public class MODIFIERS
		{
			public class SKILLLEVEL
			{
				public static LocString NAME = "Skill Level";
			}

			public class ENTITLEMENT
			{
				public static LocString NAME = "Entitlement";

				public static LocString TOOLTIP = "Duplicants will demand better <style=\"decor\">Decor</style> and accommodations with each skill level they gain";
			}

			public class BASEDUPLICANT
			{
				public static LocString NAME = "Duplicant";
			}

			public class HOMEOSTASIS
			{
				public static LocString NAME = "Homeostasis";
			}

			public class BURNINGCALORIES
			{
				public static LocString NAME = "Burning calories";
			}

			public class EATINGCALORIES
			{
				public static LocString NAME = "Eating";
			}

			public class TEMPEXCHANGE
			{
				public static LocString NAME = "Environmental exchange";
			}

			public class DIRTYHANDS
			{
				public static LocString NAME = "Dirty Hands";

				public static LocString TOOLTIP = "This Duplicant needs to <style=\"hygiene\">Wash</style> their hands";

				public static LocString CAUSE = "Obtained through prolonged digging or construction";
			}

			public class UNCLEAN
			{
				public static LocString NAME = "Grimy";

				public static LocString TOOLTIP = "This Duplicant would appreciate a <style=\"hygiene\">Shower</style>";

				public static LocString CAUSE = "Obtained by coming into contact with contaminated material";
			}

			public class SOREBACK
			{
				public static LocString NAME = "Sore Back";

				public static LocString TOOLTIP = "This Duplicant was forced to sleep on the floor and would appreciate their own bed";

				public static LocString CAUSE = "Obtained by sleeping on the ground";
			}

			public class NOSTRESSCREATOR
			{
				public static LocString NAME = "None";
			}

			public class STRESSREDUCTION
			{
				public static LocString TOOLTIP = "This Duplicant's <style=\"stress\">Stress</style> is just melting away";
			}

			public class WORKING
			{
				public static LocString NAME = "Working";

				public static LocString TOOLTIP = "This Duplicant is working up a sweat";
			}

			public class UNCOMFORTABLESLEEP
			{
				public static LocString NAME = "Uncomfortable Sleep";

				public static LocString TOOLTIP = "This Duplicant collapsed from sheer exhaustion";
			}

			public class SLEEP
			{
				public static LocString NAME = "Sleeping";

				public static LocString TOOLTIP = "This Duplicant is recovering stamina";
			}

			public class RESTFULSLEEP
			{
				public static LocString NAME = "Restful Sleep";

				public static LocString TOOLTIP = "This Duplicant is sleeping peacefully";
			}

			public class SLEEPY
			{
				public static LocString NAME = "Sleepy";

				public static LocString TOOLTIP = "This Duplicant is getting tired";
			}

			public class HUNGRY
			{
				public static LocString NAME = "Hungry";

				public static LocString TOOLTIP = "This Duplicant is ready for lunch";
			}

			public class STARVING
			{
				public static LocString NAME = "Starving";

				public static LocString TOOLTIP = "This Duplicant needs to eat something, soon";
			}

			public class HOT
			{
				public static LocString NAME = "Hot";

				public static LocString TOOLTIP = "This Duplicant is uncomfortably warm";
			}

			public class COLD
			{
				public static LocString NAME = "Cold";

				public static LocString TOOLTIP = "This Duplicant is uncomfortably cold";
			}

			public class ANEWHOPE
			{
				public static LocString NAME = "New Hope";

				public static LocString TOOLTIP = "This Duplicant feels pretty optimistic about their new home";
			}

			public class MOURNING
			{
				public static LocString NAME = "Mourning";

				public static LocString TOOLTIP = "This Duplicant is grieving the loss of a friend";
			}

			public class NARCOLEPTICSLEEP
			{
				public static LocString NAME = "Narcoleptic Sleep";

				public static LocString TOOLTIP = "This Duplicant just needs to rest their eyes for a second";
			}

			public class DISTURBEDSLEEP
			{
				public static LocString NAME = "Disturbed Sleep";

				public static LocString TOOLTIP = "This Duplicant's sleep was disturbed";
			}

			public class INTERRUPTEDSLEEP
			{
				public static LocString NAME = "Interrupted Sleep";

				public static LocString TOOLTIP = "This Duplicant was rudely awoken";
			}

			public class INSPIRED
			{
				public static LocString NAME = "Inspired";

				public static LocString TOOLTIP = "This Duplicant has had a creative vision!";
			}

			public class NEWCREWARRIVAL
			{
				public static LocString NAME = "New Friend";

				public static LocString TOOLTIP = "This Duplicant is happy to see a new face in the colony!";
			}

			public class UNDERWATER
			{
				public static LocString NAME = "Underwater";

				public static LocString TOOLTIP = "This Duplicant's movement is slowed";
			}

			public class NIGHTMARES
			{
				public static LocString NAME = "Nightmares";

				public static LocString TOOLTIP = "This Duplicant was visited by something in the night";
			}

			public class WASATTACKED
			{
				public static LocString NAME = "Was attacked";

				public static LocString TOOLTIP = "This Duplicant was recently attacked by something and it <style=\"stress\">Stressed</style> them out";
			}

			public class LIGHTWOUNDS
			{
				public static LocString NAME = "Light wounds";

				public static LocString TOOLTIP = "This Duplicant's injuries are a bit uncomfortable";
			}

			public class MODERATEWOUNDS
			{
				public static LocString NAME = "Moderate wounds";

				public static LocString TOOLTIP = "This Duplicants' injuries are affecting their ability to work";
			}

			public class SEVEREWOUNDS
			{
				public static LocString NAME = "Severe wounds";

				public static LocString TOOLTIP = "This Duplicant's injuries are severely affecting their work and well-being";
			}

			public class FULLBLADDER
			{
				public static LocString NAME = "Full Bladder";

				public static LocString TOOLTIP = "This Duplicant's bladder is full";
			}

			public class STRESSFULYEMPTYINGBLADDER
			{
				public static LocString NAME = "Making a Mess";

				public static LocString TOOLTIP = "This Duplicant had no choice but to to empty their bladder";
			}

			public class REDALERT
			{
				public static LocString NAME = "Red Alert";

				public static LocString TOOLTIP = "This Duplicant is <style=\"stress\">Stressed</style> out by the current Red Alert";
			}

			public class FUSSY
			{
				public static LocString NAME = "Fussy";

				public static LocString TOOLTIP = "This Duplicant is hard to please";
			}

			public class MENTALBREAK
			{
				public static LocString NAME = "Nervous Breakdown";

				public static LocString TOOLTIP = "<style=\"stress\">Stress</style> has completely eroded this Duplicant's ability to function";
			}

			public class WARMINGUP
			{
				public static LocString NAME = "Warming Up";

				public static LocString TOOLTIP = "This Duplicant is trying to warm back up";
			}

			public class DARKNESS
			{
				public static LocString NAME = "Darkness";

				public static LocString TOOLTIP = "This Duplicant doesn't like being in the dark!";
			}

			public class STEPPEDINCONTAMINATEDWATER
			{
				public static LocString NAME = "Stepped In Contaminated Water";

				public static LocString TOOLTIP = "This Duplicant stepped in dirty water and was super grossed out";
			}

			public class WELLFED
			{
				public static LocString NAME = "Well Fed";

				public static LocString TOOLTIP = "This Duplicant feels satisfied after having a big meal";
			}

			public class STALEFOOD
			{
				public static LocString NAME = "Stale Food";

				public static LocString TOOLTIP = "This Duplicant is in a bad mood from having to eat stale <style=\"food\">Food</style>";
			}

			public class SMELLEDPUTRIDODOUR
			{
				public static LocString NAME = "Smelled Putrid Odor";

				public static LocString TOOLTIP = "This Duplicant smelled something unspeakably foul";
			}

			public class VOMITING
			{
				public static LocString NAME = "Vomiting";

				public static LocString TOOLTIP = "Better out than in, as they say";
			}

			public class BREATHING
			{
				public static LocString NAME = "Breathing";
			}

			public class HOLDINGBREATH
			{
				public static LocString NAME = "Holding Breath";
			}

			public class RECOVERINGBREATH
			{
				public static LocString NAME = "Recovering Breath";
			}

			public class ROTTING
			{
				public static LocString NAME = "Rotting";
			}

			public class DEAD
			{
				public static LocString NAME = "Dead";
			}

			public class TOXICENVIRONMENT
			{
				public static LocString NAME = "Toxic Environment";
			}

			public class RESTING
			{
				public static LocString NAME = "Resting";
			}
		}

		public class TRAITS
		{
			public static LocString CANNOT_DO_TASK = "Cannot perform job";

			public static LocString REFUSES_TO_DO_TASK = "Refuses to do job";

			public class CANTRESEARCH
			{
				public static LocString NAME = "Yokel";

				public static LocString DESC = "This Duplicant isn't the brightest star in the sky";
			}

			public class CANTBUILD
			{
				public static LocString NAME = "Dimensionally Inept";

				public static LocString DESC = "This Duplicant is incapable of visualizing an object in 3D space";
			}

			public class CANTCOOK
			{
				public static LocString NAME = "Gastrophobia";

				public static LocString DESC = "This Duplicant has an unshakable fear of kitchens and the culinary arts";
			}

			public class CANTDIG
			{
				public static LocString NAME = "Can't Dig It";

				public static LocString DESC = "This Duplicant just can't operate a dig tool for the life of them";
			}

			public class MOUTHBREATHER
			{
				public static LocString NAME = "Mouth Breather";

				public static LocString DESC = "This Duplicant sucks up way more than their fair share of <style=\"oxygen\">Oxygen</style>";
			}

			public class FUSSY
			{
				public static LocString NAME = "Fussy";

				public static LocString DESC = "Nothing's ever quite good enough for this Duplicant";
			}

			public class TWINKLETOES
			{
				public static LocString NAME = "Twinkletoes";

				public static LocString DESC = "This Duplicant is light as a feather on their feet";
			}

			public class AGGRESSIVE
			{
				public static LocString NAME = "Destructive";

				public static LocString DESC = "This Duplicant will take out their frustrations on defenseless machines when stressed";
			}

			public class ANXIOUS
			{
				public static LocString NAME = "Anxious";

				public static LocString DESC = "This Duplicant collapses when put under too much pressure";
			}

			public class STRESSVOMITER
			{
				public static LocString NAME = "Vomiter";

				public static LocString DESC = "This Duplicant is liable to puke everywhere when stressed";
			}

			public class IRONGUT
			{
				public static LocString NAME = "Iron Gut";

				public static LocString DESC = "This Duplicant can eat just about anything without getting sick!";
			}

			public class STRONGIMMUNESYSTEM
			{
				public static LocString NAME = "Naturally Robust";

				public static LocString DESC = "This Duplicant's hardy immune system repels most common illnesses";
			}

			public class AMPHIBIOUS
			{
				public static LocString NAME = "Amphibious";

				public static LocString DESC = "This Duplicant moves as quickly underwater as they do on land";
			}

			public class SCAREDYCAT
			{
				public static LocString NAME = "Scaredy-Cat";

				public static LocString DESC = "This Duplicant is afraid of violence.";
			}

			public class WEAKIMMUNESYSTEM
			{
				public static LocString NAME = "Biohazard";

				public static LocString DESC = "All the Vitamin C in space can't stop this Duplicant from getting sick";
			}

			public class IRRITABLEBOWEL
			{
				public static LocString NAME = "Irritable Bowel";

				public static LocString DESC = "This Duplicant takes a little longer than usual to \"do their business\"";
			}

			public class CALORIEBURNER
			{
				public static LocString NAME = "Bottomless Stomach";

				public static LocString DESC = "This Duplicant might actually be several blackholes in a trenchcoat";
			}

			public class SMALLBLADDER
			{
				public static LocString NAME = "Small Bladder";

				public static LocString DESC = "This Duplicant has a tiny, pea-sized bladder. Adorable!";
			}

			public class ANEMIC
			{
				public static LocString NAME = "Anemic";

				public static LocString DESC = "This Duplicant is the exact opposite of athletic";
			}

			public class GREASEMONKEY
			{
				public static LocString NAME = "Grease Monkey";

				public static LocString DESC = "This Duplicant likes to throw a wrench into the colony's plans... in a good way";
			}

			public class MOLEHANDS
			{
				public static LocString NAME = "Mole Hands";

				public static LocString DESC = "These hands are great for tunneling, but finding gloves is a nightmare";
			}

			public class FASTLEARNER
			{
				public static LocString NAME = "Quick Learner";

				public static LocString DESC = "This Duplicant's sharp as a tack and picks up new skills with amazing speed";
			}

			public class SLOWLEARNER
			{
				public static LocString NAME = "Slow Learner";

				public static LocString DESC = "This Duplicant is a little slow on the uptake, but gosh do they try";
			}

			public class DIVERSLUNG
			{
				public static LocString NAME = "Diver's Lungs";

				public static LocString DESC = "This Duplicant may have been a talented opera singer in another life";
			}

			public class FLATULENCE
			{
				public static LocString NAME = "Flatulence";

				public static LocString DESC = "Some Duplicants are just full of it";
			}

			public class SNORER
			{
				public static LocString NAME = "Loud Sleeper";

				public static LocString DESC = "In space, everyone can hear you snore";
			}

			public class NARCOLEPSY
			{
				public static LocString NAME = "Narcoleptic";

				public static LocString DESC = "This Duplicant can and will fall asleep anytime, anyplace";
			}

			public class INTERIORDECORATOR
			{
				public static LocString NAME = "Interior Decorator";

				public static LocString DESC = "Just a little more to the left...";
			}

			public class UNCULTURED
			{
				public static LocString NAME = "Uncultured";

				public static LocString DESC = "This Duplicant has no appreciation for the finer things";
			}

			public class EARLYBIRD
			{
				public static LocString NAME = "Early Bird";

				public static LocString DESC = "This Duplicant wakes up feeling fresh and efficient!";

				public static LocString EXTENDED_DESC = "In the first part of the day, adds <style=\"produced\">{0}</style> to all skills.";
			}

			public class NIGHTOWL
			{
				public static LocString NAME = "Night Owl";

				public static LocString DESC = "This Duplicant does their best work when they ought to be sleeping";
			}
		}

		public class PERSONALITIES
		{
			public class ELEANOR
			{
				public static LocString NAME = "Eleanor";

				public static LocString DESC = "A severe climate requires a severe woman. This {0} is that woman.";
			}

			public class BEA
			{
				public static LocString NAME = "Bea";

				public static LocString DESC = "A little ray of sunshine in a bleak world, {0}s loves people and they love her right back.";
			}

			public class NISBET
			{
				public static LocString NAME = "Nisbet";

				public static LocString DESC = "{0}s like to punch people to show their affection. Everyone's too afraid to tell them it sort of hurts.";
			}

			public class MILDRED
			{
				public static LocString NAME = "Mildred";

				public static LocString DESC = "The world's a tad too complex for this here {0}, but good golly gosh if she doesn't try her darnedest.";
			}

			public class ELVIRA
			{
				public static LocString NAME = "Elvira";

				public static LocString DESC = "This {0} can't remember a time when she wasn't blanketed in the cold embrace of darkness.";
			}

			public class ELLIE
			{
				public static LocString NAME = "Ellie";

				public static LocString DESC = "Nothing makes an {0} happier than a big tin of glitter and a pack of unicorn stickers.";
			}

			public class DORRIS
			{
				public static LocString NAME = "Dorris";

				public static LocString DESC = "This {0} wakes up every day of her life with an urge to clip coupons for stores that don't exist.";
			}

			public class MYSTIQUE
			{
				public static LocString NAME = "Mystique";

				public static LocString DESC = "{0}s defy all labels.";
			}

			public class YVETTE
			{
				public static LocString NAME = "Yvette";

				public static LocString DESC = "A {0}'s every waking moment is plagued by her terrible genetic curse... a horrifyingly acute sense of smell.";
			}

			public class SHANNON
			{
				public static LocString NAME = "Shannon";

				public static LocString DESC = "Some have described {0}s as \"paranoid\". They prefer the term \"prepared\".";
			}

			public class RUBY
			{
				public static LocString NAME = "Ruby";

				public static LocString DESC = "This {0} asks the pressing questions, like \"Where can I get a leather jacket in space?\"";
			}

			public class CASS
			{
				public static LocString NAME = "Cass";

				public static LocString DESC = "This {0} is painfully aware that she spits when she talks. She avoids joining in on most conversations because of it.";
			}

			public class ROSALIND
			{
				public static LocString NAME = "Rosalind";

				public static LocString DESC = "Sometimes this {0} feels like she'd have to keel over to get people to notice her.";
			}

			public class STACY
			{
				public static LocString NAME = "Stacy";

				public static LocString DESC = "A {0} can somehow talk for hours and say absolutely nothing. It's a feat to behold!";
			}

			public class ERNESTINE
			{
				public static LocString NAME = "Ernestine";

				public static LocString DESC = "This {0} has one stubborn hair on her chin that plucking never seems to deter. It's okay though, she's growing attached to it.";
			}

			public class NADIA
			{
				public static LocString NAME = "Nadia";

				public static LocString DESC = "The burden of living is just too much for some people. Like this {0}, for instance.";
			}

			public class VIOLA
			{
				public static LocString NAME = "Viola";

				public static LocString DESC = "Small, gentle, quiet. {0}s always look as if the slightest breeze might shatter them.";
			}

			public class METRODORA
			{
				public static LocString NAME = "Metrodora";

				public static LocString DESC = "{0}s love watching old black and white films, especially if they have robots in them.";
			}

			public class TILLY
			{
				public static LocString NAME = "Tilly";

				public static LocString DESC = "{0}s like to frequently and loudly remind everyone of the pointlessness of existence.";
			}

			public class BLANCHE
			{
				public static LocString NAME = "Blanche";

				public static LocString DESC = "{0}s have zero redeeming qualities. People only ever keep them around because they make genius puns.";
			}

			public class BUBBLES
			{
				public static LocString NAME = "Bubbles";

				public static LocString DESC = "This {0} is constantly challenging others to fight her, regardless of whether or not she can actually take them.";
			}

			public class SYBIL
			{
				public static LocString NAME = "Sybil";

				public static LocString DESC = "{0}s always get what they want... Even when they shouldn't.";
			}

			public class MIMA
			{
				public static LocString NAME = "Mi-Ma";

				public static LocString DESC = "Ol' {0} here can't stand lookin' at people's knees.";
			}

			public class SUKI
			{
				public static LocString NAME = "Suki";

				public static LocString DESC = "{0}s have grins like sharks and stares to pierce through your soul.";
			}

			public class NAILS
			{
				public static LocString NAME = "Nails";

				public static LocString DESC = "People usually expect a Duplicant named \"{0}\" to be tough, but they're all pretty huge wimps.";
			}

			public class PAM
			{
				public static LocString NAME = "Pam";

				public static LocString DESC = "No one ever asks for a {0}'s bad opinions, but she's always happy to offer them anyway.";
			}

			public class OLIVE
			{
				public static LocString NAME = "Olive";

				public static LocString DESC = "This {0} feels like there's an emotional wall between themself and the world that their multitool just can't penetrate.";
			}

			public class MAE
			{
				public static LocString NAME = "Mae";

				public static LocString DESC = "There's nothing a {0} can't do if she sets her mind to it.";
			}

			public class KATRIN
			{
				public static LocString NAME = "Katrin";

				public static LocString DESC = "{0}s have trouble making eye contact in conversation, but no one really minds. They know she's listening.";
			}

			public class JOSIE
			{
				public static LocString NAME = "Josie";

				public static LocString DESC = "A {0} is admired by all for her seemingly tireless work ethic. Little do people know, she's dying on the inside.";
			}

			public class GOSSMAN
			{
				public static LocString NAME = "Gossman";

				public static LocString DESC = "{0}s love baseball and dogs, neither of which exist in space. It's been a bit of an ordeal for her.";
			}

			public class MARIE
			{
				public static LocString NAME = "Marie";

				public static LocString DESC = "This {0} is positively glowing! What's her secret? Radioactive isotopes, of course.";
			}

			public class BRAYNEN
			{
				public static LocString NAME = "Braynen";

				public static LocString DESC = "This {0} is severely disappointed by the colony's lack of cute, pudgy birds.";
			}

			public class MOZZARELLA
			{
				public static LocString NAME = "Mozzarella";

				public static LocString DESC = "World... too... bright....";
			}

			public class FRAN
			{
				public static LocString NAME = "Fran";

				public static LocString DESC = "This {0} dreams of owning their own personal computer so they can start a blog full of pictures of toast.";
			}

			public class SADIE
			{
				public static LocString NAME = "Sadie";

				public static LocString DESC = "This {0} never clips her fingernails. The unchecked growth is starting to get a little unsettling.";
			}

			public class MINGMEI
			{
				public static LocString NAME = "Ming-Mei";

				public static LocString DESC = "One time a {0} declared herself mayor of the colony for a whole week and no one even questioned it.";
			}

			public class RED
			{
				public static LocString NAME = "Red";

				public static LocString DESC = "{0}s are completely incapable of distinguishing between positive and negative attention, much to the colony's dismay.";
			}

			public class MAHA
			{
				public static LocString NAME = "Maha";

				public static LocString DESC = "{0}s always snort when they laugh. It's the worst.";
			}

			public class CELESTE
			{
				public static LocString NAME = "Celeste";

				public static LocString DESC = "This {0} is paralyzed by the knowledge that others have memories and perceptions of her she can't control.";
			}

			public class MELODY
			{
				public static LocString NAME = "Melody";

				public static LocString DESC = "Who wants to hear the song this {0} wrote about her last breakup? ...Anyone?";
			}

			public class CAMILLA
			{
				public static LocString NAME = "Camilla";

				public static LocString DESC = "This {0} has a dark, terrible secret... She LOVES scrapbooking.";
			}

			public class NORI
			{
				public static LocString NAME = "Nori";

				public static LocString DESC = "Get a bunch of {0}s together in a room and you'll have... a bunch of {0}s together in a room.";
			}

			public class PEIJING
			{
				public static LocString NAME = "Peijing";

				public static LocString DESC = "Everyone who meets a {0} has to admit that they are, in fact, extremely cool.";
			}

			public class ISAMU
			{
				public static LocString NAME = "Isamu";

				public static LocString DESC = "This {0} traps people in long conversations about her \"novel\", but she's never once sat down to actually write it.";
			}

			public class LEAH
			{
				public static LocString NAME = "Leah";

				public static LocString DESC = "There's nothing {0}s are more proud of than their thick, dignified eyebrows.";
			}

			public class MOON
			{
				public static LocString NAME = "Moon";

				public static LocString DESC = "A {0}'s monotonous voice and lack of facial expression make it impossible for others to tell when she's messing with them.";
			}

			public class BERTIE
			{
				public static LocString NAME = "Bertie";

				public static LocString DESC = "This {0} is convinced she can remember bits of her past prints. Everyone else is convinced she's making it up.";
			}

			public class GRUNGY
			{
				public static LocString NAME = "Grungy";

				public static LocString DESC = "A {0} deeply relishes the feeling of dirt beneath her nails.";
			}

			public class ADA
			{
				public static LocString NAME = "Ada";

				public static LocString DESC = "{0}s enjoy writing poetry in their downtime. Dark poetry.";
			}

			public class NEIL
			{
				public static LocString NAME = "Neil";

				public static LocString DESC = "{0}s generally like to wear strange and unusual hats to compensate for their total lack of personality.";
			}

			public class DAN
			{
				public static LocString NAME = "Dan";

				public static LocString DESC = "Every {0} has this unshakable feeling that his life has already happened and he's just watching it as if in a movie.";
			}

			public class GABRIEL
			{
				public static LocString NAME = "Gabriel";

				public static LocString DESC = "{0}s are spoiled rotten, but at least they have great hair.";
			}

			public class HASSAN
			{
				public static LocString NAME = "Hassan";

				public static LocString DESC = "If someone says something nice to a {0} he'll think about it nonstop for no less than three weeks.";
			}

			public class STINKY
			{
				public static LocString NAME = "Stinky";

				public static LocString DESC = "This {0} has never been invited to a party, which is a shame... his dance moves are incredible.";
			}

			public class FERGUS
			{
				public static LocString NAME = "Fergus";

				public static LocString DESC = "{0}es are well known for being wholly incorrigible pranksters.";
			}

			public class JOSHUA
			{
				public static LocString NAME = "Joshua";

				public static LocString DESC = "{0}s are precious goobers. Other Duplicants are strangely incapable of cursing in a {0}'s presence.";
			}

			public class HORATIO
			{
				public static LocString NAME = "Horatio";

				public static LocString DESC = "{0}s always feel as if someone is staring at them, although people rarely are.";
			}

			public class LENNY
			{
				public static LocString NAME = "Lenny";

				public static LocString DESC = "Dirt's great. Space's great. Everything's great, according to {0}s.";
			}

			public class KARL
			{
				public static LocString NAME = "Karl";

				public static LocString DESC = "Every single {0} has perpetually damp hands. It must be genetic.";
			}

			public class JASPER
			{
				public static LocString NAME = "Jasper";

				public static LocString DESC = "{0}s just want to dance!";
			}

			public class ABE
			{
				public static LocString NAME = "Abe";

				public static LocString DESC = "{0}s are sweet, delicate flowers. They need to be treated gingerly, and with great consideration for their feelings.";
			}

			public class HUGH
			{
				public static LocString NAME = "Hugh";

				public static LocString DESC = "A {0} is truly a man stuck in the past. And in a space colony.";
			}

			public class ZEKE
			{
				public static LocString NAME = "Zeke";

				public static LocString DESC = "A {0}'s snobby exterior is a defense to protect the kind, mushy heart that lurks within.";
			}

			public class JACK
			{
				public static LocString NAME = "Jack";

				public static LocString DESC = "A lifetime of running from his responsibilities has made this {0} emotionally impenetrable.";
			}

			public class RALPH
			{
				public static LocString NAME = "Ralph";

				public static LocString DESC = "Everyone feels slightly more irritable after having spent time with a {0}.";
			}

			public class BURT
			{
				public static LocString NAME = "Burt";

				public static LocString DESC = "Only the bleak void of space could understand the depths of a {0}'s tortured soul.";
			}

			public class HOWARD
			{
				public static LocString NAME = "Howard";

				public static LocString DESC = "This {0} can control a room with his exceptionally loud sneezes.";
			}

			public class VIRGIL
			{
				public static LocString NAME = "Virgil";

				public static LocString DESC = "{0}s are overcome with childlike wonder at the infinite nature of the universe.";
			}

			public class PRICE
			{
				public static LocString NAME = "Price";

				public static LocString DESC = "This {0} once had dreams of becoming a doctor, but sadly he's averse to the sight of blood.";
			}

			public class WOODROW
			{
				public static LocString NAME = "Woodrow";

				public static LocString DESC = "{0}s are exceptionally bad at reading social cues and never know when to stop talking.";
			}

			public class GUS
			{
				public static LocString NAME = "Gus";

				public static LocString DESC = "This {0} just wants everyone to be happy.";
			}

			public class HAROLD
			{
				public static LocString NAME = "Harold";

				public static LocString DESC = "This {0} is cripplingly self-conscious of his long middle toes.";
			}

			public class VERNON
			{
				public static LocString NAME = "Vernon";

				public static LocString DESC = "The colony's managed to convince this {0} he's totally and completely unique. It's hilarious!";
			}

			public class FLOYD
			{
				public static LocString NAME = "Floyd";

				public static LocString DESC = "{0}s have a theory that no matter what time it is, it's always actually 3AM.";
			}

			public class MAMORU
			{
				public static LocString NAME = "Mamoru";

				public static LocString DESC = "The \"cool loner\" vibes that radiate off a {0} never fail to make the colony swoon.";
			}

			public class QUINCY
			{
				public static LocString NAME = "Quincy";

				public static LocString DESC = "At any given moment a {0} is viscerally reliving ten different humiliating memories.";
			}

			public class ROWAN
			{
				public static LocString NAME = "Rowan";

				public static LocString DESC = "{0}s have good hearts and express their emotions most efficiently by yelling.";
			}

			public class ARCHIBALD
			{
				public static LocString NAME = "Archibald";

				public static LocString DESC = "The colony is convinced that this {0} was some sort of fancy monarch in a past life.";
			}

			public class MAX
			{
				public static LocString NAME = "Max";

				public static LocString DESC = "{0}es are painfully gullible, and other Duplicants like to take advantage of it every chance they get.";
			}

			public class SPIKE
			{
				public static LocString NAME = "Spike";

				public static LocString DESC = "{0}s detest small spaces. You can imagine how they feel about the colony.";
			}

			public class OTTO
			{
				public static LocString NAME = "Otto";

				public static LocString DESC = "{0}s always insult people by accident and exist in a perpetual state of deep regret.";
			}

			public class EUGENE
			{
				public static LocString NAME = "Eugene";

				public static LocString DESC = "Once you get a {0} talking it's impossible to get them to stop. Chitchat at your own risk.";
			}

			public class TURNER
			{
				public static LocString NAME = "Turner";

				public static LocString DESC = "{0}s alway packs a juice box on their way to political protests.";
			}

			public class LONGWEI
			{
				public static LocString NAME = "Longwei";

				public static LocString DESC = "This {0} picks his nose when he thinks no one's watching. Thankfully, he never eats it.";
			}

			public class HIDEKI
			{
				public static LocString NAME = "Hideki";

				public static LocString DESC = "This {0} is consumed by a deep, soulful weariness that no amount of sleep could possibly vanquish.";
			}

			public class SETH
			{
				public static LocString NAME = "Seth";

				public static LocString DESC = "Sometimes the colony sends {0}s on fake \"spelunking missions\" to get some peace and quiet.";
			}

			public class SAUL
			{
				public static LocString NAME = "Saul";

				public static LocString DESC = "This {0}'s the kind of guy who'd pull the legs off a Hatch for fun.";
			}

			public class WILL
			{
				public static LocString NAME = "Will";

				public static LocString DESC = "{0}s have great hair and are always about ten seconds away from starving to death.";
			}

			public class BRADY
			{
				public static LocString NAME = "Brady";

				public static LocString DESC = "{0}s are pretty cool when they can manage to suppress their natural narcissism.";
			}

			public class CECIL
			{
				public static LocString NAME = "Cecil";

				public static LocString DESC = "There's a technical term for a {0}'s personality... \"Dweeb\".";
			}

			public class NIKOLA
			{
				public static LocString NAME = "Nikola";

				public static LocString DESC = "This {0} once claimed he could build a laser so powerful it would rip the colony in half. No one dared him to prove it.";
			}

			public class ELVIS
			{
				public static LocString NAME = "Elvis";

				public static LocString DESC = "Every {0} is convinced he was printed in the wrong generation... And on the wrong celestial body.";
			}

			public class PIERRE
			{
				public static LocString NAME = "Pierre";

				public static LocString DESC = "Big softies with a tendency to nag, {0}s sometimes feel like the mothers the other Duplicants never had.";
			}

			public class CHUANLI
			{
				public static LocString NAME = "Chuanli";

				public static LocString DESC = "A {0}'s one dream is to shed their mortal coil and converge with all of time and space. It shouldn't be too much to ask.";
			}

			public class BERTRAND
			{
				public static LocString NAME = "Bertrand";

				public static LocString DESC = "This {0} always feels great after a bubble bath and a good long cry.";
			}

			public class CASPER
			{
				public static LocString NAME = "Casper";

				public static LocString DESC = "This {0} can't wait to die so their poetry can be appreciated posthumously.";
			}

			public class MEEP
			{
				public static LocString NAME = "Meep";

				public static LocString DESC = "{0}s have a face only a twelve tonne DNA Replicator could love.";
			}

			public class MCNAIR
			{
				public static LocString NAME = "McNair";

				public static LocString DESC = "All of space is brightened by a {0}'s dazzling smile.";
			}

			public class GRUB
			{
				public static LocString NAME = "Grub";

				public static LocString DESC = "{0}s think plants are neat.";
			}
		}

		public class NEEDS
		{
			public class DECOR
			{
				public static LocString NAME = "Decor Expectation";

				public static LocString PROFESSION_NAME = string.Empty;

				public static LocString OBSERVED_DECOR = "Surroundings";

				public static LocString EXPECTATION_TOOLTIP = "Areas with <style=\"decor\">Decor</style> values lower than a Duplicant's <style=\"decor\">Decor Expectations</style> will cause their <style=\"stress\">Stress</style>\nto rise, while high decor values will cause their stress to fall.\n\nA Duplicant's expectations will increase as they level up and become better at their jobs.";
			}
		}

		public class ATTRIBUTES
		{
			public static LocString UNPROFESSIONAL_NAME = "Lump";

			public static LocString UNPROFESSIONAL_DESC = "This Duplicant has no discernible skills to speak of";

			public static LocString PROFESSION_DESC = "This Duplicant is most proficient at {0}";

			public class CONSTRUCTION
			{
				public static LocString NAME = "Construction";

				public static LocString PROFESSION_NAME = "Architect";

				public static LocString DESC = "The Construction attribute determines a Duplicant's building speed.";

				public static LocString SPEEDMODIFIER = "$value% Construction Speed";
			}

			public class DIGGING
			{
				public static LocString NAME = "Digging";

				public static LocString PROFESSION_NAME = "Miner";

				public static LocString DESC = "The Digging attribute determines a Duplicant's mining speed.";

				public static LocString SPEEDMODIFIER = "$value% Digging Speed";
			}

			public class MACHINERY
			{
				public static LocString NAME = "Tinkering";

				public static LocString PROFESSION_NAME = "Technician";

				public static LocString DESC = "The Tinkering attribute determines a Duplicant's efficiency when operating machines.";

				public static LocString SPEEDMODIFIER = "$value% Tinkering Speed";
			}

			public class ATHLETICS
			{
				public static LocString NAME = "Athletics";

				public static LocString PROFESSION_NAME = "Runner";

				public static LocString DESC = "The Athletics attribute determines a Duplicant's movement speed.";

				public static LocString SPEEDMODIFIER = "$value% Movement Speed";
			}

			public class LEARNING
			{
				public static LocString NAME = "Learning";

				public static LocString PROFESSION_NAME = "Scientist";

				public static LocString DESC = "The Learning attribute determines a Duplicant's skill training and <style=\"research\">Research</style> effectiveness.";

				public static LocString SPEEDMODIFIER = "$value% Skill Leveling";

				public static LocString RESEARCHPOINTS = "$value Research Points Per Day";
			}

			public class COOKING
			{
				public static LocString NAME = "Cooking";

				public static LocString PROFESSION_NAME = "Chef";

				public static LocString DESC = "The Cooking attribute determines a Duplicant's <style=\"food\">Food</style> production speed.";

				public static LocString SPEEDMODIFIER = "$value% Cooking Speed";
			}

			public class INSULATION
			{
				public static LocString NAME = "Insulation";

				public static LocString PROFESSION_NAME = string.Empty;

				public static LocString DESC = "The Insulation attribute determines how much a Duplicant's temperature fluctuates with their environment's.";

				public static LocString SPEEDMODIFIER = "$value% Temperature Retention";
			}

			public class MEDICAL
			{
				public static LocString NAME = "Medicine";

				public static LocString PROFESSION_NAME = "Physician";

				public static LocString DESC = "The Medicine attribute determines a Duplicant's <style=\"disease\">Disease</style> resistance and ability to heal after injury.";

				public static LocString SPEEDMODIFIER = "$value% Disease Resistance";
			}

			public class STRENGTH
			{
				public static LocString NAME = "Strength";

				public static LocString PROFESSION_NAME = "Body Builder";

				public static LocString DESC = "The Strength attribute determines a Duplicant's carrying capacity and combat effectiveness.";

				public static LocString SPEEDMODIFIER = "$value% Carrying Weight";
			}

			public class DECOR
			{
				public static LocString NAME = "Decor";

				public static LocString DESC = "<style=\"decor\">Decor</style> affects a Duplicant's Room Opinion.";
			}

			public class DECORRADIUS
			{
				public static LocString NAME = "Decor Radius";

				public static LocString DESC = "Distance of the <style=\"decor\">Decor</style> effect.";
			}

			public class DECOREXPECTATION
			{
				public static LocString NAME = "Decor Expectation";

				public static LocString DESC = "Not meeting a Duplicant's expectations increases their <style=\"stress\">Stress</style>.";
			}

			public class HYGIENE
			{
				public static LocString NAME = "Hygiene";

				public static LocString PROFESSION_NAME = string.Empty;

				public static LocString DESC = "<style=\"hygiene\">Hygiene</style> affects a Duplicant's sense of cleanliness.";
			}

			public class ART
			{
				public static LocString NAME = "Creativity";

				public static LocString PROFESSION_NAME = "Artist";

				public static LocString DESC = "Creativity affects the speed and quality of a Duplicant's artistic creations.";

				public static LocString SPEEDMODIFIER = "$value% Decorating Speed";
			}

			public class AIRCONSUMPTIONRATE
			{
				public static LocString NAME = "Air Consumption Rate";

				public static LocString PROFESSION_NAME = string.Empty;

				public static LocString DESC = "The Air Consumption attribute determines how much <style=\"oxygen\">Oxygen</style> per minute a Duplicant requires to live.";
			}

			public class STRESSDELTA
			{
				public static LocString NAME = "Stress Change";

				public static LocString DESC = "Modifies the rate at which a Duplicant's <style=\"stress\">Stress</style> changes.";
			}

			public class BLADDERDELTA
			{
				public static LocString NAME = "Bladder Change";

				public static LocString DESC = string.Empty;
			}

			public class CALORIESDELTA
			{
				public static LocString NAME = "Calories Burn Rate";

				public static LocString DESC = string.Empty;
			}

			public class STAMINADELTA
			{
				public static LocString NAME = "Stamina Change";

				public static LocString DESC = string.Empty;
			}

			public class TOXICITYDELTA
			{
				public static LocString NAME = "Toxicity Change";

				public static LocString DESC = string.Empty;
			}

			public class TOILETEFFICIENCY
			{
				public static LocString NAME = "Bladder Efficiency";

				public static LocString PROFESSION_NAME = string.Empty;

				public static LocString DESC = "Modifies the speed with which a Duplicant completes their \"personal business\".";
			}

			public class MAXUNDERWATERTRAVELCOST
			{
				public static LocString NAME = "Underwater Movement";

				public static LocString PROFESSION_NAME = string.Empty;

				public static LocString DESC = string.Empty;
			}
		}

		public class OPINIONS
		{
			public static LocString ROOMWALLSOPINION = "Duplicants don't find bare rockface very inviting.\nUse wall tiles and doors to build more comfortable Rooms for them.";

			public static LocString MESSHALLWALLSOPINION = "Duplicants don't find bare rockface very inviting.\nUse wall tiles and doors to build more comfortable Mess Halls for them.";

			public static LocString TOILETWALLSOPINION = "Duplicants like a little privacy when they do their biz.\nUse wall tiles and doors to build more comfortable Washrooms for them.";

			public static LocString ROOMWALLSMISSING = "Room is missing walls or doors.";

			public static LocString ROOMWALLSSTANDARD = "Room is enclosed by walls and doors.";

			public static LocString TOILETWALLSMISSING = "Restroom is missing walls or doors.";

			public static LocString TOILETWALLSSTANDARD = "Restroom is enclosed by walls and doors.";

			public static LocString LIGHTOPINION = "Duplicants are drawn to lit places, and dislike being in the dark.\nMake sure your Duplicants have plenty of <style=\"light\">Light</style> to keep their eyes unstrained and their <style=\"stress\">Stress</style> low.";

			public static LocString DARKNESS = "There's too little <style=\"light\">Light</style> for this Duplicant to see.";

			public static LocString LIGHTNESS = "There is enough <style=\"light\">Light</style> for this Duplicant to see.";
		}

		public class THOUGHTS
		{
			public class STARVING
			{
				public static LocString TOOLTIP = "Starving";
			}

			public class HOT
			{
				public static LocString TOOLTIP = "Hot";
			}

			public class COLD
			{
				public static LocString TOOLTIP = "Cold";
			}

			public class FULLBLADDER
			{
				public static LocString TOOLTIP = "Full Bladder";
			}

			public class HAPPY
			{
				public static LocString TOOLTIP = "Happy";
			}

			public class UNHAPPY
			{
				public static LocString TOOLTIP = "Unhappy";
			}

			public class POORDECOR
			{
				public static LocString TOOLTIP = "Poor Decor";
			}

			public class SLEEPY
			{
				public static LocString TOOLTIP = "Sleepy";
			}

			public class SUFFOCATING
			{
				public static LocString TOOLTIP = "Suffocating";
			}

			public class ANGRY
			{
				public static LocString TOOLTIP = "Angry";
			}

			public class RAGING
			{
				public static LocString TOOLTIP = "Raging";
			}

			public class GOTINFECTED
			{
				public static LocString TOOLTIP = "Got Infected";
			}

			public class PUTRIDODOUR
			{
				public static LocString TOOLTIP = "Smelled Something Putrid";
			}
		}

		public class RELAXATION
		{
			public class RELAXATION_EFFECT
			{
				public static LocString NAME = "Relaxing";

				public static LocString DESCRIPTION = "Reduces <style=\"stress\">Stress</style> by {0}/day";
			}
		}
	}
}
