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

				public static LocString TOOLTIP = "Breath\n----------\nA Duplicant with zero remaining breath will begin suffocating";
			}

			public class STAMINA
			{
				public static LocString NAME = "Stamina";

				public static LocString TOOLTIP = "Stamina\n----------\nDuplicants will pass out from fatigue when stamina reaches zero";
			}

			public class CALORIES
			{
				public static LocString NAME = "Calories";

				public static LocString TOOLTIP = "Calories\n----------\nThis Duplicant can burn {0} before starving";
			}

			public class TEMPERATURE
			{
				public static LocString NAME = "Body Temperature";

				public static LocString TOOLTIP = "Body Temperature\n----------\nA healthy Duplicant's body temperature is {1}";
			}

			public class EXTERNALTEMPERATURE
			{
				public static LocString NAME = "External Temperature";

				public static LocString TOOLTIP = "External Temperature\n----------\nThis Duplicant's environment is {0}";
			}

			public class DECOR
			{
				public static LocString NAME = "Decor";

				public static LocString TOOLTIP = "Decor\n----------\nDuplicants become stressed in areas with decor lower than their expectations\nOpen the Decor Overlay <color=#F44A47>[F8]</color> to view current decor values";
			}

			public class STRESS
			{
				public static LocString NAME = "Stress";

				public static LocString TOOLTIP = "Stress\n----------\nDuplicants exhibit their Stress Responses at one hundred percent stress";
			}

			public class TOXICITY
			{
				public static LocString NAME = "<DO NOT TRANSLATE>";

				public static LocString TOOLTIP = string.Empty;
			}

			public class BLADDER
			{
				public static LocString NAME = "Bladder";

				public static LocString TOOLTIP = "Bladder\n----------\nDuplicants make \"messes\" if no toilets are available at one hundred percent bladder fullness";
			}

			public class HITPOINTS
			{
				public static LocString NAME = "Health";

				public static LocString TOOLTIP = "Health\n----------\nCombatants become incapacitated at zero health";
			}

			public class SKIN_THICKNESS
			{
				public static LocString NAME = "Skin Thickness";
			}
		}

		public class DEATHS
		{
			public class GENERIC
			{
				public static LocString NAME = "Generic";

				public static LocString DESCRIPTION = "{Target} has died.";
			}

			public class FROZEN
			{
				public static LocString NAME = "Frozen";

				public static LocString DESCRIPTION = "{Target} has frozen to death.";
			}

			public class SUFFOCATION
			{
				public static LocString NAME = "Suffocation";

				public static LocString DESCRIPTION = "{Target} has suffocated to death.";
			}

			public class STARVATION
			{
				public static LocString NAME = "Starvation";

				public static LocString DESCRIPTION = "{Target} has starved to death.";
			}

			public class OVERHEATING
			{
				public static LocString NAME = "Starvation";

				public static LocString DESCRIPTION = "{Target} has starved to death.";
			}

			public class DROWNED
			{
				public static LocString NAME = "Drowned";

				public static LocString DESCRIPTION = "{Target} has drowned.";
			}

			public class EXPLOSION
			{
				public static LocString NAME = "Explosion";

				public static LocString DESCRIPTION = "{Target} has died in an explosion.";
			}

			public class COMBAT
			{
				public static LocString NAME = "Slain";

				public static LocString DESCRIPTION = "{Target} was slain in combat.";
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
				public static LocString NAME = "Incapacitated";

				public static LocString STATUS = "Dying";
			}

			public class DEBUGGOTO
			{
				public static LocString NAME = "DebugGoTo";

				public static LocString STATUS = "DebugGoTo";
			}

			public class STRESSIDLE
			{
				public static LocString NAME = "Antsy Idle";

				public static LocString STATUS = "This Duplicant needs something to do to take their mind of their stress";
			}

			public class MOVETO
			{
				public static LocString NAME = "Move to";

				public static LocString STATUS = "Moving to location";
			}

			public class DROPUNUSEDINVENTORY
			{
				public static LocString NAME = "Drop unused inventory";

				public static LocString STATUS = "Dropping unused inventory";
			}

			public class PEE
			{
				public static LocString NAME = "Pee";

				public static LocString STATUS = "Bio Break";
			}

			public class STRESSVOMIT
			{
				public static LocString NAME = "Stress vomit";

				public static LocString STATUS = "Stress vomiting";
			}

			public class UGLY_CRY
			{
				public static LocString NAME = "Ugly Cry";

				public static LocString STATUS = "Ugly crying";
			}

			public class BINGE_EAT
			{
				public static LocString NAME = "Binge Eat";

				public static LocString STATUS = "Binge eating";
			}

			public class EMOTEHIGHPRIORITY
			{
				public static LocString NAME = "Emoting";

				public static LocString STATUS = "Expressing themself";
			}

			public class FLEE
			{
				public static LocString NAME = "Flee";

				public static LocString STATUS = "Fleeing";
			}

			public class RECOVERBREATH
			{
				public static LocString NAME = "Recover breath";

				public static LocString STATUS = "Recovering breath";
			}

			public class MOVETOQUARANTINE
			{
				public static LocString NAME = "Move to quarantine";

				public static LocString STATUS = "Moving to quarantine";
			}

			public class MOVETOLOCATION
			{
				public static LocString NAME = "Move to location";

				public static LocString STATUS = "Moving to location";
			}

			public class ATTACK
			{
				public static LocString NAME = "Attack";

				public static LocString STATUS = "Attacking";
			}

			public class USETOILET
			{
				public static LocString NAME = "Use toilet";

				public static LocString STATUS = "Going to use toilet";
			}

			public class WASHHANDS
			{
				public static LocString NAME = "Wash hands";

				public static LocString STATUS = "Washing hands";
			}

			public class EAT
			{
				public static LocString NAME = "Eat";

				public static LocString STATUS = "Going to eat";
			}

			public class PRIORITIZECHORE
			{
				public static LocString NAME = "Prioritize chore";

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
				public static LocString NAME = "Sleep";

				public static LocString STATUS = "Sleeping";
			}

			public class TAKEMEDICINE
			{
				public static LocString NAME = "Take medicine";

				public static LocString STATUS = "Taking medicine";
			}

			public class DOCTOR
			{
				public static LocString NAME = "Doctor";

				public static LocString STATUS = "Nursing";
			}

			public class DELIVERFOOD
			{
				public static LocString NAME = "Deliver food";

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
				public static LocString NAME = "Rest";

				public static LocString STATUS = "Resting";
			}

			public class HEAL
			{
				public static LocString NAME = "Heal";

				public static LocString STATUS = "Healing";
			}

			public class STRESSACTINGOUT
			{
				public static LocString NAME = "Lash out";

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

				public static LocString STATUS = "Recharging equipment";
			}

			public class UNEQUIP
			{
				public static LocString NAME = "Unequip";

				public static LocString STATUS = "Moving to unequip";
			}

			public class WARMUP
			{
				public static LocString NAME = "Warm up";

				public static LocString STATUS = "Going to warm up";
			}

			public class COOLDOWN
			{
				public static LocString NAME = "Cool off";

				public static LocString STATUS = "Going to cool off";
			}

			public class EMPTYSTORAGE
			{
				public static LocString NAME = "Empty storage";

				public static LocString STATUS = "Going to empty storage";
			}

			public class UPGRADE
			{
				public static LocString NAME = "Upgrade";

				public static LocString STATUS = "Going to upgrade";
			}

			public class ART
			{
				public static LocString NAME = "Decorate";

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
				public static LocString NAME = "Rescue friend";

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

			public class RESEARCHFETCH
			{
				public static LocString NAME = "Deliver";

				public static LocString STATUS = "Delivering";
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
				public static LocString NAME = "Uproot";

				public static LocString STATUS = "Going to uproot";
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
				public static LocString NAME = "Cook";

				public static LocString STATUS = "Going to cook";
			}

			public class COOKFETCH
			{
				public static LocString NAME = "Deliver";

				public static LocString STATUS = "Delivering";
			}

			public class MUSH
			{
				public static LocString NAME = "Mush";

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
				public static LocString NAME = "Move to safety";

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

				public static LocString DESC = "Fight wild creatures.";
			}

			public class MOP
			{
				public static LocString NAME = "Mop";

				public static LocString DESC = "Clean up liquid messes.";
			}

			public class COOK
			{
				public static LocString NAME = "Cook";

				public static LocString DESC = "Operate food preparation buildings to produce calories.";
			}

			public class ART
			{
				public static LocString NAME = "Art";

				public static LocString DESC = "Sculpt or paint to improve colony decor.";
			}

			public class COMPOST
			{
				public static LocString NAME = "Compost";

				public static LocString DESC = "Tend composts to produce fertilizer.";
			}

			public class RESEARCH
			{
				public static LocString NAME = "Research";

				public static LocString DESC = "Use research stations to unlock new technologies.";
			}

			public class GENERATEPOWER
			{
				public static LocString NAME = "Power";

				public static LocString DESC = "Produce power by running on manual generators.";
			}

			public class REPAIR
			{
				public static LocString NAME = "Repair";

				public static LocString DESC = "Repair damaged buildings.";
			}

			public class HARVEST
			{
				public static LocString NAME = "Harvest";

				public static LocString DESC = "Gather crops from mature plants.";
			}

			public class BUILD
			{
				public static LocString NAME = "Build";

				public static LocString DESC = "Construct new buildings.";
			}

			public class DELIVER
			{
				public static LocString NAME = "Deliver";

				public static LocString DESC = "Run resources to critical buildings and high priority storage.";
			}

			public class SWEEP
			{
				public static LocString NAME = "Sweep";

				public static LocString DESC = "Run resources to noncritical buildings or storage.";
			}

			public class DIG
			{
				public static LocString NAME = "Dig";

				public static LocString DESC = "Mine raw resources.";
			}

			public class TOGGLE
			{
				public static LocString NAME = "Toggle";

				public static LocString DESC = "Manually enable, disable or adjust building switches and filters.";
			}

			public class LIQUIDCOOLEDFAN
			{
				public static LocString NAME = "Hydrofan";

				public static LocString DESC = "Operate hydrofans to cool ambient temperatures.";
			}

			public class MASSAGE
			{
				public static LocString NAME = "Massage";

				public static LocString DESC = "Take breaks for massages.";
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
				public static LocString NAME = "Red Alert!";

				public static LocString TOOLTIP = "The colony is in a state of Red Alert. Duplicants will not eat, sleep, use the bathroom or engage in leisure activities while the Red Alert is active";
			}

			public class LOWOXYGEN
			{
				public static LocString NAME = "Oxygen low";

				public static LocString TOOLTIP = "This Duplicant is working in a low breathability area";

				public static LocString NOTIFICATION_NAME = "Low <style=\"oxygen\">Oxygen</style> area entered";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are working in areas with low <style=\"oxygen\">Oxygen</style>:";
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
				public static LocString NAME = "Incapacitated\nTime until death: {TimeUntilDeath}";

				public static LocString TOOLTIP = "This Duplicant is near death! They need to be rescued quickly\n----------\nDuplicants must have an assigned Medical Cot or Rejuvenator to be rescued";

				public static LocString NOTIFICATION_NAME = "Incapacitated";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants need to be rescued before they kick the bucket\n----------\nDuplicants must have an assigned Medical Cot or Rejuvenator to be rescued";
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
				public static LocString NAME = "Chilly surroundings";

				public static LocString TOOLTIP = "This Duplicant cannot retain enough heat to stay warm and may be under insulated for this area\nStress Modification: {StressModification}\n\nCurrent Environmental Exchange: {currentTransferWattage}\n\nInsulation Thickness: {conductivityBarrier}";
			}

			public class DAILYRATIONLIMITREACHED
			{
				public static LocString NAME = "Daily calorie limit reached";

				public static LocString TOOLTIP = "This Duplicant has consumed their allotted rations for the day";

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

				public static LocString TOOLTIP = "This Duplicant held their breath too long and needs a breather";
			}

			public class HOT
			{
				public static LocString NAME = "Toasty surroundings";

				public static LocString TOOLTIP = "This Duplicant cannot let off enough heat to stay cool and may be over insulated for this area\nStress Modification: {StressModification}\n\nCurrent Environmental Exchange: {currentTransferWattage}\n\nInsulation Thickness: {conductivityBarrier}";
			}

			public class HUNGRY
			{
				public static LocString NAME = "Hungry";

				public static LocString TOOLTIP = "This Duplicant is low on calories and would love a snack";
			}

			public class POORDECOR
			{
				public static LocString NAME = "Drab decor";

				public static LocString TOOLTIP = "This Duplicant finds the lack of Decor in this area depressing";
			}

			public class POOR_FOOD_QUALITY
			{
				public static LocString NAME = "Lousy Meal";

				public static LocString TOOLTIP = "This Duplicant's last meal didn't meet their expectations";
			}

			public class GOOD_FOOD_QUALITY
			{
				public static LocString NAME = "Decadent Meal";

				public static LocString TOOLTIP = "This Duplicant's last meal exceeded their expectations";
			}

			public class NERVOUSBREAKDOWN
			{
				public static LocString NAME = "Nervous breakdown";

				public static LocString TOOLTIP = "Stress has completely eroded this Duplicant's ability to function";

				public static LocString NOTIFICATION_NAME = "Nervous breakdown";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have cracked under the <style=\"stress\">Stress</style> and need assistance:";
			}

			public class STRESSED
			{
				public static LocString NAME = "Stressed";

				public static LocString TOOLTIP = "This Duplicant is feeling the pressure";

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

				public static LocString TOOLTIP = "There is food in the colony that this Duplicant cannot reach";

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

				public static LocString TOOLTIP = "This Duplicant needs food!";

				public static LocString NOTIFICATION_NAME = "Starvation";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are starving and need <style=\"food\">Food</style>:";
			}

			public class STRESS_SIGNAL_AGGRESIVE
			{
				public static LocString NAME = "Frustrated";

				public static LocString TOOLTIP = "This Duplicant is trying to keep their cool";
			}

			public class STRESS_SIGNAL_BINGE_EAT
			{
				public static LocString NAME = "Stress Cravings";

				public static LocString TOOLTIP = "This Duplicant is consumed by hunger";
			}

			public class STRESS_SIGNAL_UGLY_CRIER
			{
				public static LocString NAME = "Tearing up";

				public static LocString TOOLTIP = "This Duplicant is trying and failing to suppress their emotions";
			}

			public class STRESS_SIGNAL_VOMITER
			{
				public static LocString NAME = "Stress Burp";

				public static LocString TOOLTIP = "Kind of like having butterflies in your stomach, except they're burps";
			}

			public class ENTOMBEDCHORE
			{
				public static LocString NAME = "Entombed";

				public static LocString TOOLTIP = "This Duplicant needs someone to help dig them out!";

				public static LocString NOTIFICATION_NAME = "Entombed";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are trapped:";
			}

			public class EARLYMORNING
			{
				public static LocString NAME = "Jazzed to start the day";

				public static LocString TOOLTIP = "This Duplicant gets a burst of energy in the morning from their Early Bird trait";
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

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants cannot reach breathable <style=\"oxygen\">Oxygen</style>:";
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
				public static LocString NAME = "In combat";

				public static LocString TOOLTIP = "This Duplicant is attacking a creature!";

				public static LocString NOTIFICATION_NAME = "Combat!";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have engaged a creature in combat:";
			}

			public class FLEEING
			{
				public static LocString NAME = "Fleeing";

				public static LocString TOOLTIP = "This Duplicant is trying to escape combat!";

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

				public static LocString TOOLTIP = "This Duplicant is breaking stuff to relieve stress";

				public static LocString NOTIFICATION_NAME = "Lashing out";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants broke buildings to relieve <style=\"stress\">Stress</style>:";
			}

			public class MOVETOSUITNOTREQUIRED
			{
				public static LocString NAME = "Exiting <style=\"equipment\">Exosuit</style> area";

				public static LocString TOOLTIP = "This Duplicant is leaving an area where an exosuit was required";
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

				public static LocString NOTIFICATION_NAME = "Restroom out of order";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants want to use a toilet that is out of order:";
			}

			public class NOTOILETS
			{
				public static LocString NAME = "No restrooms";

				public static LocString TOOLTIP = "There are no toilets available for this Duplicant";

				public static LocString NOTIFICATION_NAME = "No restrooms built";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants are distressed by the colony's lack of toilets:";
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

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants could not find a toilet in time.\nUse the <color=#833A5FFF>MOP TOOL</color> [K] to clean up their mess:\n";
			}

			public class WASHINGHANDS
			{
				public static LocString NAME = "Washing hands";

				public static LocString TOOLTIP = "This Duplicant is washing their hands";
			}

			public class SHOWERING
			{
				public static LocString NAME = "Showering";

				public static LocString TOOLTIP = "This Duplicant is gonna be squeaky clean";
			}

			public class RELAXING
			{
				public static LocString NAME = "Relaxing";

				public static LocString TOOLTIP = "This Duplicant's just taking it easy";
			}

			public class VOMITING
			{
				public static LocString NAME = "Throwing up";

				public static LocString TOOLTIP = "This Duplicant has unceremoniously hurled as the result of a disease";

				public static LocString NOTIFICATION_NAME = "Throwing up";

				public static LocString NOTIFICATION_TOOLTIP = "A <style=\"disease\">Disease</style> has caused these Duplicants to throw up:";
			}

			public class STRESSVOMITING
			{
				public static LocString NAME = "Stress vomiting";

				public static LocString TOOLTIP = "This Duplicant is relieving their stress all over the floor";

				public static LocString NOTIFICATION_NAME = "Stress vomiting";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants became too <style=\"stress\">Stressed</style> and it caused them to throw up:";
			}

			public class HASDISEASE
			{
				public static LocString NAME = "Feeling ill";

				public static LocString TOOLTIP = "This Duplicant has contracted a Disease";

				public static LocString NOTIFICATION_NAME = "Illness";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants have contracted a <style=\"disease\">Disease</style>:";
			}

			public class BODYREGULATINGHEATING
			{
				public static LocString NAME = "Regulating temperature at: {TempDelta}";

				public static LocString TOOLTIP = "This Duplicant is regulating their internal temperature";
			}

			public class BODYREGULATINGCOOLING
			{
				public static LocString NAME = "Regulating temperature at: {TempDelta}";

				public static LocString TOOLTIP = "This Duplicant is regulating their internal temperature";
			}

			public class BREATHINGO2
			{
				public static LocString NAME = "Inhaling {ConsumptionRate} O2";

				public static LocString TOOLTIP = "All Duplicants require oxygen to live";
			}

			public class EMITTINGCO2
			{
				public static LocString NAME = "Exhaling {EmittingRate} CO2";

				public static LocString TOOLTIP = "All Duplicants breathe out carbon dioxide";
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

				public static LocString TOOLTIP = "This Duplicant is replenishing their calorie stores";
			}

			public class CLEANING
			{
				public static LocString NAME = "Cleaning {Target}";

				public static LocString TOOLTIP = "This Duplicant is cleaning the {Target}";
			}

			public class PICKINGUP
			{
				public static LocString NAME = "Picking up {Target}";

				public static LocString TOOLTIP = "This Duplicant is retrieving {Target}";
			}

			public class MOPPING
			{
				public static LocString NAME = "Mopping";

				public static LocString TOOLTIP = "This Duplicant is cleaning up a nasty spill";
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

				public static LocString TOOLTIP = "This Duplicant is intently researching {Tech} technology";
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

				public static LocString TOOLTIP = "This Duplicant is equipping a {Item}";
			}

			public class WARMINGUP
			{
				public static LocString NAME = "Warming up";

				public static LocString TOOLTIP = "This Duplicant got too cold and is trying to warm up";
			}

			public class GENERATINGPOWER
			{
				public static LocString NAME = "Generating power";

				public static LocString TOOLTIP = "This Duplicant is using the {Target} to produce electrical power";
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
			public static LocString CURED_POPUP = "Cured of {0}";

			public static LocString INFECTED_POPUP = "Became infected by {0}";

			public static LocString NOTIFICATION_TOOLTIP = "{0} contracted <style=\"disease\">{1}</style> by {2}";

			public static LocString STATUS_ITEM_TOOLTIP = "{Symptoms}\n\nContracted by {InfectionSource}\nDuration remaining: {Duration}\nRemedies: {Cures}";

			public static LocString NOMEDICINETAKEN = "None";

			public static LocString CURES = "Instantly cures <style=\"disease\">{0}</style>";

			public static LocString BOOSTSCURESPEED = "Heal speed (<style=\"disease\">{0}</style>): <style=\"produced\">+{1}</style>";

			public static LocString REDUCECURESPEED = "Heal speed (<style=\"disease\">{0}</style>): <style=\"consumed\">{1}</style>";

			public static LocString RECUPERATING = "Resting up";

			public class TRIGGERS
			{
				public static LocString EATCOMPLETEEDIBLE = "May cause <style=\"disease\">{Diseases}</style>";

				public class TOOLTIPS
				{
					public static LocString EATCOMPLETEEDIBLE = "May cause {Diseases}";
				}
			}

			public class INFECTIONSOURCES
			{
				public static LocString INTERNAL_TEMPERATURE = "extreme internal temperature";

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

			public class COLDBRAIN
			{
				public static LocString NAME = "Hypothermia";

				public static LocString DESCRIPTION = "This Duplicant's thought processes have been slowed to a crawl as a result of extreme cold exposure";
			}

			public class HEATRASH
			{
				public static LocString NAME = "Heat Stroke";

				public static LocString DESCRIPTION = "This Duplicant's thought processes have short circuited as a result of extreme heat exposure";
			}

			public class SPORES
			{
				public static LocString NAME = "The Spores";

				public static LocString DESCRIPTION = "This Duplicant has become a walking fungal pod as a result of illness";
			}

			public class DWEEBCEPHALY
			{
				public static LocString NAME = "Brain Fog";

				public static LocString DESCRIPTION = "This Duplicant is experiencing cognitive impairment as a result of illness";
			}

			public class LAZIBONITIS
			{
				public static LocString NAME = "The Lackadaisies";

				public static LocString DESCRIPTION = "This Duplicant really doesn't feel like working right now";
			}

			public class PUTRIDODOUR
			{
				public static LocString NAME = "Trench Stench";

				public static LocString DESCRIPTION = "The pungent odor wafting off this Duplicant is nauseating to their peers";

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

				public static LocString TOOLTIP = "Duplicants will demand better decor and accommodations with each Profession level they gain";
			}

			public class BASEDUPLICANT
			{
				public static LocString NAME = "Duplicant";
			}

			public class HOMEOSTASIS
			{
				public static LocString NAME = "Homeostasis";
			}

			public class WARMAIR
			{
				public static LocString NAME = "Warm Air";
			}

			public class COLDAIR
			{
				public static LocString NAME = "Cold Air";
			}

			public class CLAUSTROPHOBIC
			{
				public static LocString NAME = "Claustrophobic";

				public static LocString TOOLTIP = "This Duplicant recently found themselves in an upsettingly cramped space";

				public static LocString CAUSE = "This Duplicant got so good at their job that they became claustrophobic";
			}

			public class VERTIGO
			{
				public static LocString NAME = "Vertigo";

				public static LocString TOOLTIP = "This Duplicant had to climb a tall ladder that left them dizzy and unsettled";

				public static LocString CAUSE = "This Duplicant got so good at their job that they became bad at ladders";
			}

			public class UNCOMFORTABLEFEET
			{
				public static LocString NAME = "Aching Feet";

				public static LocString TOOLTIP = "This Duplicant recently walked across untiled floor, much to their chagrin";

				public static LocString CAUSE = "This Duplicant got so good at their job that their feet became sensitive";
			}

			public class PEOPLETOOCLOSEWHILESLEEPING
			{
				public static LocString NAME = "Personal Bubble Burst";

				public static LocString TOOLTIP = "This Duplicant had to sleep too close to others and it was awkward for them";

				public static LocString CAUSE = "This Duplicant got so good at their job that they stopped being comfortable sleeping near other people";
			}

			public class RESTLESS
			{
				public static LocString NAME = "Restless";

				public static LocString TOOLTIP = "This Duplicant went a few minutes without working and is now completely awash with guilt";

				public static LocString CAUSE = "This Duplicant got so good at their job that they forgot how to be comfortable doing anything else";
			}

			public class UNFASHIONABLECLOTHING
			{
				public static LocString NAME = "Fashion Crime";

				public static LocString TOOLTIP = "This Duplicant had to wear something that was an affront to fashion";

				public static LocString CAUSE = "This Duplicant got so good at their job that they became incapable of tolerating unfashionable clothing";
			}

			public class BURNINGCALORIES
			{
				public static LocString NAME = "Burning Calories";
			}

			public class EATINGCALORIES
			{
				public static LocString NAME = "Eating";
			}

			public class TEMPEXCHANGE
			{
				public static LocString NAME = "Environmental Exchange";
			}

			public class CLOTHING
			{
				public static LocString NAME = "Clothing";
			}

			public class DIRTYHANDS
			{
				public static LocString NAME = "Dirty Hands";

				public static LocString TOOLTIP = "This Duplicant needs to wash their hands";

				public static LocString CAUSE = "Obtained through prolonged digging or construction";
			}

			public class CRYFACE
			{
				public static LocString NAME = "Cry Face";

				public static LocString TOOLTIP = "This Duplicant recently had a crying fit and it shows";

				public static LocString CAUSE = "Obtained through the Ugly Crier stress response";
			}

			public class UNCLEAN
			{
				public static LocString NAME = "Grimy";

				public static LocString TOOLTIP = "This Duplicant is dirty and needs a shower";

				public static LocString CAUSE = "Obtained by coming into contact with polluted substances";
			}

			public class SOREBACK
			{
				public static LocString NAME = "Sore Back";

				public static LocString TOOLTIP = "This Duplicant feels achey from sleeping on the floor last night and would like a bed";

				public static LocString CAUSE = "Obtained by sleeping on the ground";
			}

			public class GOODEATS
			{
				public static LocString NAME = "Soul Food";

				public static LocString TOOLTIP = "This Duplicant had a yummy homecooked meal and is totally stuffed";

				public static LocString CAUSE = "Obtained by eating a hearty meal";
			}

			public class FRESH_AND_CLEAN
			{
				public static LocString NAME = "Refreshingly Clean";

				public static LocString TOOLTIP = "This Duplicant took a warm shower and it was great!";

				public static LocString CAUSE = "Obtained by taking a comfortably heated shower";
			}

			public class BURNED_BY_SCALDING_WATER
			{
				public static LocString NAME = "Scalded";

				public static LocString TOOLTIP = "Ouch! This Duplicant showered or was doused in water that was much too hot";

				public static LocString CAUSE = "Obtained by exposure to hot water";
			}

			public class STRESSED_BY_COLD_WATER
			{
				public static LocString NAME = "Numb";

				public static LocString TOOLTIP = "Brr! This Duplicant showered or was doused in water that was much too cold";

				public static LocString CAUSE = "Obtained by exposure to icy water";
			}

			public class SMELLEDSTINKY
			{
				public static LocString NAME = "Smelled Stinky";

				public static LocString TOOLTIP = "This Duplicant got a whiff of a certain somebody";
			}

			public class NOSTRESSCREATOR
			{
				public static LocString NAME = "None";
			}

			public class STRESSREDUCTION
			{
				public static LocString TOOLTIP = "This Duplicant's stress is just melting away";
			}

			public class UGLY_CRYING
			{
				public static LocString NAME = "Ugly Crying";

				public static LocString TOOLTIP = "This Duplicant is having a cathartic ugly cry as a result of stress";

				public static LocString NOTIFICATION_NAME = "Ugly Crying";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants became too <style=\"stress\">Stressed</style> and it caused them to cry:";
			}

			public class BINGE_EATING
			{
				public static LocString NAME = "Insatiable Hunger";

				public static LocString TOOLTIP = "This Duplicant is stuffing their face a result of stress";

				public static LocString NOTIFICATION_NAME = "Binge Eating";

				public static LocString NOTIFICATION_TOOLTIP = "These Duplicants became too <style=\"stress\">Stressed</style> and started eating everything in sight:";
			}

			public class WORKING
			{
				public static LocString NAME = "Working";

				public static LocString TOOLTIP = "This Duplicant is working up a sweat";
			}

			public class UNCOMFORTABLESLEEP
			{
				public static LocString NAME = "Sleeping Uncomfortably";

				public static LocString TOOLTIP = "This Duplicant collapsed from sheer exhaustion";
			}

			public class SLEEP
			{
				public static LocString NAME = "Sleeping";

				public static LocString TOOLTIP = "This Duplicant is recovering stamina";
			}

			public class RESTFULSLEEP
			{
				public static LocString NAME = "Sleeping Peacefully";

				public static LocString TOOLTIP = "This Duplicant is getting a good night's rest";
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

			public class WETFEET
			{
				public static LocString NAME = "Soggy Feet";

				public static LocString TOOLTIP = "This Duplicant recently stepped in liquid";
			}

			public class SOAKINGWET
			{
				public static LocString NAME = "Sopping Wet";

				public static LocString TOOLTIP = "This Duplicant was recently submerged in liquid";
			}

			public class ANEWHOPE
			{
				public static LocString NAME = "New Hope";

				public static LocString TOOLTIP = "This Duplicant feels pretty optimistic about their new home";
			}

			public class NOOXYGEN
			{
				public static LocString NAME = "No Oxygen";

				public static LocString TOOLTIP = "There is no breathable air in this area";
			}

			public class LOWOXYGEN
			{
				public static LocString NAME = "Low Oxygen";

				public static LocString TOOLTIP = "The air in this area is unpleasantly thin";
			}

			public class MOURNING
			{
				public static LocString NAME = "Mourning";

				public static LocString TOOLTIP = "This Duplicant is grieving the loss of a friend";
			}

			public class NARCOLEPTICSLEEP
			{
				public static LocString NAME = "Narcoleptic Nap";

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

				public static LocString TOOLTIP = "This Duplicant was rudely awoken while they slept";
			}

			public class CENTEROFATTENTION
			{
				public static LocString NAME = "Center of Attention";

				public static LocString TOOLTIP = "This Duplicant feels like someone is watching them...";
			}

			public class INSPIRED
			{
				public static LocString NAME = "Inspired";

				public static LocString TOOLTIP = "This Duplicant has had a creative vision!";
			}

			public class NEWCREWARRIVAL
			{
				public static LocString NAME = "New Friend";

				public static LocString TOOLTIP = "This Duplicant is happy to see a new face in the colony";
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
				public static LocString NAME = "Recently assailed";

				public static LocString TOOLTIP = "This Duplicant is stressed out after having been attacked";
			}

			public class LIGHTWOUNDS
			{
				public static LocString NAME = "Light Wounds";

				public static LocString TOOLTIP = "This Duplicant sustained injuries that are a bit uncomfortable";
			}

			public class MODERATEWOUNDS
			{
				public static LocString NAME = "Moderate Wounds";

				public static LocString TOOLTIP = "This Duplicant sustained injuries that are affecting their ability to work";
			}

			public class SEVEREWOUNDS
			{
				public static LocString NAME = "Severe Wounds";

				public static LocString TOOLTIP = "This Duplicant sustained serious injuries that are impacting their work and well-being";
			}

			public class ROTTEMPERATURE
			{
				public static LocString NAME = "Unrefrigerated";
			}

			public class ROTATMOSPHERE
			{
				public static LocString NAME = "Contaminated Air";
			}

			public class BASEROT
			{
				public static LocString NAME = "Base Decay Rate";
			}

			public class FULLBLADDER
			{
				public static LocString NAME = "Full Bladder";

				public static LocString TOOLTIP = "This Duplicant's bladder is full";
			}

			public class STRESSFULYEMPTYINGBLADDER
			{
				public static LocString NAME = "Making a mess";

				public static LocString TOOLTIP = "This Duplicant had no choice but to to empty their bladder";
			}

			public class REDALERT
			{
				public static LocString NAME = "Red Alert!";

				public static LocString TOOLTIP = "The Red Alert is stressing this Duplicant out";
			}

			public class FUSSY
			{
				public static LocString NAME = "Fussy";

				public static LocString TOOLTIP = "This Duplicant is hard to please";
			}

			public class WARMINGUP
			{
				public static LocString NAME = "Warming Up";

				public static LocString TOOLTIP = "This Duplicant is trying to warm back up";
			}

			public class COOLINGDOWN
			{
				public static LocString NAME = "Cooling Down";

				public static LocString TOOLTIP = "This Duplicant is trying to cool back down";
			}

			public class DARKNESS
			{
				public static LocString NAME = "Darkness";

				public static LocString TOOLTIP = "Eep! This Duplicant doesn't like being in the dark!";
			}

			public class STEPPEDINCONTAMINATEDWATER
			{
				public static LocString NAME = "Stepped in polluted water";

				public static LocString TOOLTIP = "Gross! This Duplicant stepped in something yucky";
			}

			public class WELLFED
			{
				public static LocString NAME = "Well fed";

				public static LocString TOOLTIP = "This Duplicant feels satisfied after having a big meal";
			}

			public class STALEFOOD
			{
				public static LocString NAME = "Bad leftovers";

				public static LocString TOOLTIP = "This Duplicant is in a bad mood from having to eat stale food";
			}

			public class SMELLEDPUTRIDODOUR
			{
				public static LocString NAME = "Smelled a putrid odor";

				public static LocString TOOLTIP = "This Duplicant got a whiff of something unspeakably foul";
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
				public static LocString NAME = "Holding breath";
			}

			public class RECOVERINGBREATH
			{
				public static LocString NAME = "Recovering breath";
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
				public static LocString NAME = "Toxic environment";
			}

			public class RESTING
			{
				public static LocString NAME = "Resting";
			}

			public class INTRAVENOUS_NUTRITION
			{
				public static LocString NAME = "Intravenous Feeding";
			}
		}

		public class CONGENITALTRAITS
		{
			public class NONE
			{
				public static LocString NAME = "None";

				public static LocString DESC = "This Duplicant seems pretty average overall";
			}

			public class JOSHUA
			{
				public static LocString NAME = "Cheery Disposition";

				public static LocString DESC = "This Duplicant brightens others' days wherever he goes";
			}

			public class ELLIE
			{
				public static LocString NAME = "Fastidious";

				public static LocString DESC = "This Duplicant needs things done in a very particular way";
			}

			public class LEIRA
			{
				public static LocString NAME = "Starving Artist";

				public static LocString DESC = "This Duplicant's two loves in life are art and food";
			}

			public class STINKY
			{
				public static LocString NAME = "Stinkiness";

				public static LocString DESC = "This Duplicant is genetically cursed by a pungent bodily odor";
			}

			public class CATALINA
			{
				public static LocString NAME = "Atlas";

				public static LocString DESC = "This Duplicant feels the weight of the world on her shoulders";
			}

			public class ABE
			{
				public static LocString NAME = "Doting";

				public static LocString DESC = "This Duplicant just wants to make sure everyone's eating right";
			}
		}

		public class TRAITS
		{
			public static LocString CANNOT_DO_TASK = "Cannot perform job";

			public static LocString REFUSES_TO_DO_TASK = "Refuses to do job";

			public class NEEDS
			{
				public class CLAUSTROPHOBIC
				{
					public static LocString NAME = "Claustrophobe";

					public static LocString DESC = "This Duplicant feels suffocated in spaces less than four tiles high or three tiles wide";
				}

				public class FASHIONABLE
				{
					public static LocString NAME = "Fashionista";

					public static LocString DESC = "This Duplicant dies a bit inside when forced to wear unstylish clothing";
				}

				public class CLIMACOPHOBIC
				{
					public static LocString NAME = "Vertigo Prone";

					public static LocString DESC = "Climbing ladders more than four tiles tall makes this Duplicant's stomach do flips";
				}

				public class SOLITARYSLEEPER
				{
					public static LocString NAME = "Solitary Sleeper";

					public static LocString DESC = "This Duplicant prefers to sleep alone";
				}

				public class PREFERSWARMER
				{
					public static LocString NAME = "Skinny";

					public static LocString DESC = "This Duplicant doesn't have much insulation, so they are more temperature sensitive than others";
				}

				public class PREFERSCOOLER
				{
					public static LocString NAME = "Pudgy";

					public static LocString DESC = "This Duplicant has some extra insulation, so the room temperature affects them a little less";
				}

				public class SENSITIVEFEET
				{
					public static LocString NAME = "Delicate Feetsies";

					public static LocString DESC = "This Duplicant is a sensitive sole and would rather walk on tile floorings than bedrock";
				}

				public class WORKAHOLIC
				{
					public static LocString NAME = "Workaholic";

					public static LocString DESC = "This Duplicant gets antsy when left idle";
				}
			}

			public class CANTRESEARCH
			{
				public static LocString NAME = "Yokel";

				public static LocString DESC = "This Duplicant isn't the brightest star in the solar system";
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

				public static LocString DESC = "This Duplicant can't operate a dig tool for the life of them";
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

			public class STRONGARM
			{
				public static LocString NAME = "Buff";

				public static LocString DESC = "This Duplicant is very strong and cool";
			}

			public class NOODLEARMS
			{
				public static LocString NAME = "Noodle Arms";

				public static LocString DESC = "This Duplicant's arms have all the tensile strength of an overcooked linguine";
			}

			public class AGGRESSIVE
			{
				public static LocString NAME = "Destructive";

				public static LocString DESC = "This Duplicant will take out their frustrations on defenseless machines when stressed";
			}

			public class UGLYCRIER
			{
				public static LocString NAME = "Ugly Crier";

				public static LocString DESC = "Things won't be pretty if this Duplicant gets too stressed";
			}

			public class BINGEEATER
			{
				public static LocString NAME = "Binge Eater";

				public static LocString DESC = "This Duplicant will dangerously overeat when stressed";
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

				public static LocString EXTENDED_DESC = "Adds <style=\"produced\">{0}</style> to all skills at morning, removes them at night";
			}

			public class NIGHTOWL
			{
				public static LocString NAME = "Night Owl";

				public static LocString DESC = "This Duplicant does their best work when they ought to be sleeping";
			}
		}

		public class PERSONALITIES
		{
			public class CATALINA
			{
				public static LocString NAME = "Catalina";

				public static LocString DESC = "A {0} is admired by all for her seemingly tireless work ethic. Little do people know, she's dying on the inside.";
			}

			public class NISBET
			{
				public static LocString NAME = "Nisbet";

				public static LocString DESC = "This {0} likes to punch people to show her affection. Everyone's too afraid of her to tell her it hurts.";
			}

			public class ELLIE
			{
				public static LocString NAME = "Ellie";

				public static LocString DESC = "Nothing makes an {0} happier than a big tin of glitter and a pack of unicorn stickers.";
			}

			public class RUBY
			{
				public static LocString NAME = "Ruby";

				public static LocString DESC = "This {0} asks the pressing questions, like \"Where can I get a leather jacket in space?\"";
			}

			public class LEIRA
			{
				public static LocString NAME = "Leira";

				public static LocString DESC = "{0}s just want everyone to be happy.";
			}

			public class BUBBLES
			{
				public static LocString NAME = "Bubbles";

				public static LocString DESC = "This {0} is constantly challenging others to fight her, regardless of whether or not she can actually take them.";
			}

			public class MIMA
			{
				public static LocString NAME = "Mi-Ma";

				public static LocString DESC = "Ol' {0} here can't stand lookin' at people's knees.";
			}

			public class NAILS
			{
				public static LocString NAME = "Nails";

				public static LocString DESC = "People often expect a Duplicant named \"{0}\" to be tough, but they're all pretty huge wimps.";
			}

			public class MAE
			{
				public static LocString NAME = "Mae";

				public static LocString DESC = "There's nothing a {0} can't do if she sets her mind to it.";
			}

			public class GOSSMANN
			{
				public static LocString NAME = "Gossmann";

				public static LocString DESC = "{0}s are big goofballs who can make anyone laugh.";
			}

			public class MARIE
			{
				public static LocString NAME = "Marie";

				public static LocString DESC = "This {0} is positively glowing! What's her secret? Radioactive isotopes, of course.";
			}

			public class LINDSAY
			{
				public static LocString NAME = "Lindsay";

				public static LocString DESC = "A {0} is a charming, delightful woman, unless you make the mistake of messing with one of her friends.";
			}

			public class DEVON
			{
				public static LocString NAME = "Devon";

				public static LocString DESC = "This {0} dreams of owning their own personal computer so they can start a blog full of pictures of toast.";
			}

			public class REN
			{
				public static LocString NAME = "Ren";

				public static LocString DESC = "Every {0} has this unshakable feeling that his life's already happened and he's just watching it unfold from a memory.";
			}

			public class FRANKIE
			{
				public static LocString NAME = "Frankie";

				public static LocString DESC = "There's nothing {0}s are more proud of than their thick, dignified eyebrows.";
			}

			public class BANHI
			{
				public static LocString NAME = "Banhi";

				public static LocString DESC = "The \"cool loner\" vibes that radiate off a {0} never fail to make the colony swoon.";
			}

			public class ADA
			{
				public static LocString NAME = "Ada";

				public static LocString DESC = "{0}s enjoy writing poetry in their downtime. Dark poetry.";
			}

			public class HASSAN
			{
				public static LocString NAME = "Hassan";

				public static LocString DESC = "If someone says something nice to a {0} he'll think about it nonstop for no less than three weeks.";
			}

			public class STINKY
			{
				public static LocString NAME = "Stinky";

				public static LocString DESC = "This {0} has never been invited to a party, which is a shame. His dance moves are incredible.";
			}

			public class JOSHUA
			{
				public static LocString NAME = "Joshua";

				public static LocString DESC = "{0}s are precious goobers. Other Duplicants are strangely incapable of cursing in a {0}'s presence.";
			}

			public class ABE
			{
				public static LocString NAME = "Abe";

				public static LocString DESC = "{0}s are sweet, delicate flowers. They need to be treated gingerly, with great consideration for their feelings.";
			}

			public class BURT
			{
				public static LocString NAME = "Burt";

				public static LocString DESC = "Every single {0} has perpetually damp hands. It must be genetic.";
			}

			public class TRAVALDO
			{
				public static LocString NAME = "Travaldo";

				public static LocString DESC = "A {0}'s monotonous voice and lack of facial expression makes it impossible for others to tell when he's messing with them.";
			}

			public class HAROLD
			{
				public static LocString NAME = "Harold";

				public static LocString DESC = "This {0} is cripplingly self-conscious of his long middle toes.";
			}

			public class MAX
			{
				public static LocString NAME = "Max";

				public static LocString DESC = "At any given moment a {0} is viscerally reliving ten different humiliating memories.";
			}

			public class ROWAN
			{
				public static LocString NAME = "Rowan";

				public static LocString DESC = "{0}s have exceptionally large hearts and express their emotions most efficiently by yelling.";
			}

			public class OTTO
			{
				public static LocString NAME = "Otto";

				public static LocString DESC = "{0}s always insult people by accident and exist in a perpetual state of deep regret.";
			}

			public class TURNER
			{
				public static LocString NAME = "Turner";

				public static LocString DESC = "This {0} is paralyzed by the knowledge that others have memories and perceptions of them they can't control.";
			}

			public class NIKOLA
			{
				public static LocString NAME = "Nikola";

				public static LocString DESC = "This {0} once claimed he could build a laser so powerful it would rip the colony in half. No one asked him to prove it.";
			}

			public class MEEP
			{
				public static LocString NAME = "Meep";

				public static LocString DESC = "{0}s have a face only a twelve tonne Printing Pod could love.";
			}

			public class J
			{
				public static LocString NAME = "Berkeley";

				public static LocString DESC = "0100111001001111010101000101001001001001010001110100100001010100010011100100111101010100010010000100010101010010";
			}

			public class A
			{
				public static LocString NAME = "Yates";

				public static LocString DESC = "010011100100111101010100010011010100010101001110010011110101010001001101010001010100111001001111010101000100110101000101";
			}
		}

		public class NEEDS
		{
			public class DECOR
			{
				public static LocString NAME = "Decor Expectation";

				public static LocString PROFESSION_NAME = "Critic";

				public static LocString OBSERVED_DECOR = "Surroundings";

				public static LocString EXPECTATION_TOOLTIP = "Most objects have <style=\"decor\">Decor</style> values that can increase or decrease a Duplicant's opinion of their surroundings.\n\nThis Duplicant requires {0} or higher decor values, and will become <style=\"stress\">Stressed</style> in areas with lower decor.";
			}

			public class FOOD_QUALITY
			{
				public static LocString NAME = "Food Quality";

				public static LocString PROFESSION_NAME = "Gourmet";

				public static LocString EXPECTATION_TOOLTIP = "Food Quality Expectation is the minimum quality of <style=\"food\">Food</style> a Duplicant can tolerate eating.\n\nThis Duplicant requires {0} or higher food, and will become <style=\"stress\">Stressed</style> if to forced to eat lower quality foods.";

				public static LocString BAD_FOOD_MOD = "Food Quality";

				public static LocString NORMAL_FOOD_MOD = "Food Quality";

				public static LocString GOOD_FOOD_MOD = "Food Quality";

				public static LocString ADJECTIVE_FORMAT_POSITIVE = "{0} (<style=\"produced\">{1}</style>)";

				public static LocString ADJECTIVE_FORMAT_NEGATIVE = "{0} (<style=\"consumed\">{1}</style>)";

				public static LocString TOOLTIP = "{0}";

				public static LocString FOODQUALITY = "\nFood Quality Score of {0}";

				public static LocString FOODQUALITY_EXPECTATION = "\nThis Duplicant is content to eat food with a Quality Score of {0} or higher";

				public static int ADJECTIVE_INDEX_OFFSET = -3;

				public class ADJECTIVES
				{
					public static LocString MINUS_3 = "Grisly";

					public static LocString MINUS_2 = "Terrible";

					public static LocString MINUS_1 = "Poor";

					public static LocString ZERO = "Standard";

					public static LocString PLUS_1 = "Good";

					public static LocString PLUS_2 = "Great";

					public static LocString PLUS_3 = "Superb";

					public static LocString PLUS_4 = "Ambrosial";
				}
			}
		}

		public class ATTRIBUTES
		{
			public static LocString UNPROFESSIONAL_NAME = "Lump";

			public static LocString UNPROFESSIONAL_DESC = "This Duplicant has no discernible skills";

			public static LocString PROFESSION_DESC = "This Duplicant's highest attribute is {0}\n----------\nDuplicants develop higher expectations as their profession level increases";

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

			public class SNEEZYNESS
			{
				public static LocString NAME = "Sneeziness";

				public static LocString DESC = "The Sneeziness attribute determines how frequently a Duplicant sneezes.";
			}

			public class LEARNING
			{
				public static LocString NAME = "Learning";

				public static LocString PROFESSION_NAME = "Scientist";

				public static LocString DESC = "The Learning attribute determines a Duplicant's <style=\"research\">Research</style> effectiveness and rate of skill level gain.";

				public static LocString SPEEDMODIFIER = "$value% Skill Leveling";

				public static LocString RESEARCHPOINTS = "$value Research Points Per Day";
			}

			public class COOKING
			{
				public static LocString NAME = "Cooking";

				public static LocString PROFESSION_NAME = "Chef";

				public static LocString DESC = "The Cooking attribute determines a Duplicant's speed when preparing <style=\"food\">Food</style>.";

				public static LocString SPEEDMODIFIER = "$value% Cooking Speed";
			}

			public class INSULATION
			{
				public static LocString NAME = "Insulation";

				public static LocString PROFESSION_NAME = string.Empty;

				public static LocString DESC = "The Insulation attribute determines a Duplicant's innate ability to retain or lose body heat in any given environment.";

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

				public static LocString SPEEDMODIFIER = "$value Carrying Weight";
			}

			public class DECOR
			{
				public static LocString NAME = "Decor";

				public static LocString DESC = "<style=\"decor\">Decor</style> affects a Duplicant's <style=\"decor\">Stress</style> and their opinion of their surroundings.";
			}

			public class THERMALCONDUCTIVITYBARRIER
			{
				public static LocString NAME = "Insulation Thickness";

				public static LocString TOOLTIP = "Insulation Thickness determines how quickly a Duplicant retains or loses body <style=\"heat\">Heat</style> in any given area.\n\nIt is the sum of a Duplicant's equipment and their natural Insulation values.";
			}

			public class DECORRADIUS
			{
				public static LocString NAME = "Decor Radius";

				public static LocString DESC = "Distance of a item's <style=\"decor\">Decor</style> effect.";
			}

			public class DECOREXPECTATION
			{
				public static LocString NAME = "Decor Expectation";

				public static LocString DESC = "Not meeting a Duplicant's <style=\"decor\">Decor</style> expectations will increase their <style=\"stress\">Stress</style>.";
			}

			public class FOODEXPECTATION
			{
				public static LocString NAME = "Food Quality Expectation";

				public static LocString DESC = "Not meeting a Duplicant's <style=\"food\">Food</style> expectations will increase their <style=\"stress\">Stress</style>.";
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

				public static LocString DESC = "Air Consumption determines how much <style=\"oxygen\">Oxygen</style> a Duplicant requires per minute to live.";
			}

			public class STRESSDELTA
			{
				public static LocString NAME = "Stress Change";

				public static LocString DESC = "Determines how quickly a Duplicant gains or reduces <style=\"stress\">Stress</style>.";
			}

			public class BLADDERDELTA
			{
				public static LocString NAME = "Bladder Change";

				public static LocString DESC = string.Empty;
			}

			public class CALORIESDELTA
			{
				public static LocString NAME = "Calories Change Rate";

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

				public static LocString DESC = "Determines how long a Duplicant needs to do their \"business\".";
			}

			public class ROOMTEMPERATUREPREFERENCE
			{
				public static LocString NAME = "Temperature Preference";

				public static LocString PROFESSION_NAME = string.Empty;

				public static LocString DESC = "Determines the minimum body heat a Duplicant prefers to maintain.";
			}

			public class MAXUNDERWATERTRAVELCOST
			{
				public static LocString NAME = "Underwater Movement";

				public static LocString PROFESSION_NAME = string.Empty;

				public static LocString DESC = string.Empty;
			}

			public class OVERHEATTEMPERATURE
			{
				public static LocString NAME = "Overheat Temperature";

				public static LocString DESC = "A building at Overheat <style=\"heat\">Temperature</style> will take damage and melt down if not cooled";
			}

			public class FATALTEMPERATURE
			{
				public static LocString NAME = "Meltdown Temperature";

				public static LocString DESC = "A building at Meltdown <style=\"heat\">Temperature</style> will lose functionality and take damage";
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

			public class POOR_FOOD_QUALITY
			{
				public static LocString TOOLTIP = "Lousy Meal";
			}

			public class GOOD_FOOD_QUALITY
			{
				public static LocString TOOLTIP = "Delicious Meal";
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

				public static LocString DESCRIPTION = "Reduces <style=\"stress\">Stress</style> by {0}/cycle";
			}
		}
	}
}
