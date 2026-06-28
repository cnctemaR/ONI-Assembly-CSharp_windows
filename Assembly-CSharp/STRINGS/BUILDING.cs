using System;

namespace STRINGS
{
	public class BUILDING
	{
		public class STATUSITEMS
		{
			public class ANGERDAMAGE
			{
				public static LocString NAME = "Damage: Tantrum";

				public static LocString TOOLTIP = "A stressed Duplicant is damaging this building";

				public static LocString NOTIFICATION = "Building Damage: Tantrum";

				public static LocString NOTIFICATION_TOOLTIP = "Stressed Duplicants are damaging these buildings:\n\n{0}";
			}

			public class PIPECONTENTS
			{
				public static LocString EMPTY = "Empty";

				public static LocString CONTENTS = "{0} of {1} at {2}";

				public static LocString CONTENTS_WITH_DISEASE = "\n  {0}";
			}

			public class ASSIGNEDTO
			{
				public static LocString NAME = "Assigned to: {Assignee}";

				public static LocString TOOLTIP = "Only {Assignee} can use this amenity";
			}

			public class ASSIGNEDPUBLIC
			{
				public static LocString NAME = "Assigned to: Public";

				public static LocString TOOLTIP = "Any Duplicant can use this amenity";
			}

			public class ASSIGNEDTOROOM
			{
				public static LocString NAME = "Assigned to: {0}";

				public static LocString TOOLTIP = "Any Duplicant assigned to this room can use this amenity";
			}

			public class AWAITINGSEEDDELIVERY
			{
				public static LocString NAME = "Awaiting Delivery";

				public static LocString TOOLTIP = "Awaiting delivery of selected seed";
			}

			public class AWAITINGBAITDELIVERY
			{
				public static LocString NAME = "Awaiting Bait";

				public static LocString TOOLTIP = "Awaiting delivery of selected bait";
			}

			public class CLINICOUTSIDEHOSPITAL
			{
				public static LocString NAME = "Medical building outside Med Bay";

				public static LocString TOOLTIP = "Rebuild this medical equipment in a Med Bay to more effectively quarantine sick Duplicants";
			}

			public class BOTTLE_EMPTIER
			{
				public static class ALLOWED
				{
					public static LocString NAME = "Autobottling: On";

					public static LocString TOOLTIP = "Duplicants may specifically fetch liquid from Pitcher Pumps to bring to this location";
				}

				public static class DENIED
				{
					public static LocString NAME = "Autobottling: Off";

					public static LocString TOOLTIP = "Duplicants may not specifically fetch liquid from Pitcher Pumps to bring to this location";
				}
			}

			public class BROKEN
			{
				public static LocString NAME = "Broken";

				public static LocString TOOLTIP = UI.HORIZONTAL_BR_RULE + "This building received damage from {DamageInfo}";
			}

			public class CHANGEDOORCONTROLSTATE
			{
				public static LocString NAME = "Pending Door State Change: {ControlState}";

				public static LocString TOOLTIP = "Waiting for a Duplicant to change control state";
			}

			public class SUIT_LOCKER
			{
				public class NEED_CONFIGURATION
				{
					public static LocString NAME = "Current Status: Needs Configuration";

					public static LocString TOOLTIP = "Set this Dock to store an Exosuit or remain on Standby";
				}

				public class READY
				{
					public static LocString NAME = "Current Status: On Standby";

					public static LocString TOOLTIP = "This Dock is functional and ready to receive an Exosuit";
				}

				public class SUIT_REQUESTED
				{
					public static LocString NAME = "Current Status: Awaiting Delivery";

					public static LocString TOOLTIP = "Waiting for a Duplicant to deliver an Exosuit";
				}

				public class CHARGING
				{
					public static LocString NAME = "Current Status: Charging Suit";

					public static LocString TOOLTIP = "This Exosuit is docked and charging";
				}

				public class NO_OXYGEN
				{
					public static LocString NAME = "Current Status: No Oxygen";

					public static LocString TOOLTIP = "This Dock is not receiving enough oxygen to refill an Exosuit";
				}

				public class NOT_OPERATIONAL
				{
					public static LocString NAME = "Current Status: Offline";

					public static LocString TOOLTIP = "This Exosuit Dock requires power";
				}

				public class FULLY_CHARGED
				{
					public static LocString NAME = "Current Status: Full Charge";

					public static LocString TOOLTIP = "This Exosuit is fully charged and ready for use";
				}
			}

			public class SUITMARKERTRAVERSALONLYWHENROOMAVAILABLE
			{
				public static LocString NAME = "Clearance: Vacancy Only";

				public static LocString TOOLTIP = "Suited Duplicants may pass only if there is room in an Exosuit Dock to store their suit";
			}

			public class SUITMARKERTRAVERSALANYTIME
			{
				public static LocString NAME = "Clearance: Always Permitted";

				public static LocString TOOLTIP = "Suited Duplicants may pass even if there is no room to store their suits" + UI.HORIZONTAL_BR_RULE + "When Exosuit Docks are all full, Duplicants will unequip their suits and drop them on the floor";
			}

			public class SUIT_LOCKER_NEEDS_CONFIGURATION
			{
				public static LocString NAME = "Not Configured";

				public static LocString TOOLTIP = "Exosuit Dock settings not configured";
			}

			public class CURRENTDOORCONTROLSTATE
			{
				public static LocString NAME = "Current State: {ControlState}";

				public static LocString TOOLTIP = "Current State: {ControlState}" + UI.HORIZONTAL_BR_RULE + "Auto: Duplicants open and close this door as needed\nLocked: Nothing may pass through\nOpen: This door will remain open";

				public static LocString OPENED = "Opened";

				public static LocString AUTO = "Auto";

				public static LocString CLOSED = "Locked";
			}

			public class CONDUITBLOCKED
			{
				public static LocString NAME = "Pipe Blocked";

				public static LocString TOOLTIP = "This pipe system is not serviceable";
			}

			public class CONSTRUCTIONUNREACHABLE
			{
				public static LocString NAME = "Unreachable Build";

				public static LocString TOOLTIP = "Duplicants cannot reach this construction site";
			}

			public class MOPUNREACHABLE
			{
				public static LocString NAME = "Unreachable Mop";

				public static LocString TOOLTIP = "Duplicants cannot reach this area";
			}

			public class DIGUNREACHABLE
			{
				public static LocString NAME = "Unreachable Dig";

				public static LocString TOOLTIP = "Duplicants cannot reach this area";
			}

			public class CONSTRUCTABLEDIGUNREACHABLE
			{
				public static LocString NAME = "Unreachable Dig";

				public static LocString TOOLTIP = "This construction site contains cells that cannot be dug out";
			}

			public class EMPTYPUMPINGSTATION
			{
				public static LocString NAME = "Empty";

				public static LocString TOOLTIP = "This pumping station cannot access any liquid";
			}

			public class ENTOMBED
			{
				public static LocString NAME = "Entombed";

				public static LocString TOOLTIP = "Must be dug out by a Duplicant";

				public static LocString NOTIFICATION_NAME = "Building entombment";

				public static LocString NOTIFICATION_TOOLTIP = "These buildings are entombed and need to be dug out:";
			}

			public class GENESHUFFLECOMPLETED
			{
				public static LocString NAME = "Neural vacillation complete";

				public static LocString TOOLTIP = "The Duplicant has completed the neural vacillation process and is ready to be released";
			}

			public class OVERHEATED
			{
				public static LocString NAME = "Damage: Overheating";

				public static LocString TOOLTIP = "This building is taking damage and will meltdown if not cooled";
			}

			public class OPERATINGENERGY
			{
				public static LocString NAME = "Heat Production: {0}";

				public static LocString TOOLTIP = "This building is producing {0} of energy\n\nSources:\n{1}";

				public static LocString LINEITEM = "    • {0}: {1}\n";

				public static LocString OPERATING = "Normal operation";

				public static LocString EXHAUSTING = "Excess produced";

				public static LocString PIPECONTENTS_TRANSFER = "Transferred from pipes";
			}

			public class FLOODED
			{
				public static LocString NAME = "Building Flooded";

				public static LocString TOOLTIP = "Building cannot function at current saturation";

				public static LocString NOTIFICATION_NAME = "Flooding";

				public static LocString NOTIFICATION_TOOLTIP = "These buildings are flooded:";
			}

			public class GASVENTOBSTRUCTED
			{
				public static LocString NAME = "Gas Vent Obstructed";

				public static LocString TOOLTIP = "A pipe has been obstructed and is preventing gas flow to this vent";
			}

			public class GASVENTOVERPRESSURE
			{
				public static LocString NAME = "Gas Vent Overpressure";

				public static LocString TOOLTIP = "High air or liquid pressure in this area is preventing further liquid emission\nReduce pressure by pumping gas away or clearing more space";
			}

			public class DIRECTION_CONTROL
			{
				public static LocString NAME = "Use Direction: {Direction}";

				public static LocString TOOLTIP = "Duplicants will only use this building when walking by it. Currently allowed direction: {Direction}";

				public static class DIRECTIONS
				{
					public static LocString LEFT = "Left";

					public static LocString RIGHT = "Right";

					public static LocString BOTH = "Both";
				}
			}

			public class WATTSONGAMEOVER
			{
				public static LocString NAME = "Colony Lost";

				public static LocString TOOLTIP = "All Duplicants are dead or incapacitated";
			}

			public class INVALIDBUILDINGLOCATION
			{
				public static LocString NAME = "Invalid Building Location";

				public static LocString TOOLTIP = "Cannot construct building in this location";
			}

			public class LIQUIDVENTOBSTRUCTED
			{
				public static LocString NAME = "Liquid Vent Obstructed";

				public static LocString TOOLTIP = "A pipe has been obstructed and is preventing liquid flow to this vent";
			}

			public class LIQUIDVENTOVERPRESSURE
			{
				public static LocString NAME = "Liquid Vent Overpressure";

				public static LocString TOOLTIP = "High air or liquid pressure in this area is preventing further liquid emission\nReduce pressure by pumping liquid away or clearing more space";
			}

			public class MANUALLYCONTROLLED
			{
				public static LocString NAME = "Manually Controlled";

				public static LocString TOOLTIP = "This Duplicant is under your control";
			}

			public class MATERIALSUNAVAILABLE
			{
				public static LocString NAME = "Insufficient Resources\n{ItemsRemaining}";

				public static LocString TOOLTIP = "Crucial materials for this building are beyond reach or unavailable";

				public static LocString NOTIFICATION_NAME = "Building lacks resources";

				public static LocString NOTIFICATION_TOOLTIP = "Crucial materials are unavailable or beyond reach for these buildings:";

				public static LocString LINE_ITEM_MASS = "• {0}: {1}";

				public static LocString LINE_ITEM_UNITS = "• {0}";
			}

			public class MATERIALSUNAVAILABLEFORREFILL
			{
				public static LocString NAME = "Resources Low\n{ItemsRemaining}";

				public static LocString TOOLTIP = "This building will soon require materials that are unavailable";

				public static LocString LINE_ITEM = "• {0}";
			}

			public class MELTINGDOWN
			{
				public static LocString NAME = "Melting Down";

				public static LocString TOOLTIP = "This building is collapsing";

				public static LocString NOTIFICATION_NAME = "Building meltdown";

				public static LocString NOTIFICATION_TOOLTIP = "These buildings are collapsing:";
			}

			public class MISSINGFOUNDATION
			{
				public static LocString NAME = "Missing Foundation";

				public static LocString TOOLTIP = "Build Tiles beneath this building" + UI.HORIZONTAL_BR_RULE + "Tiles can be found in the <color=#833A5FFF>Base Tab</color> <color=#F44A47>[1]</color> of the Build Menu";
			}

			public class NEEDBORINGMACHINE
			{
				public static LocString NAME = "Multitool Required";

				public static LocString TOOLTIP = "A multitool is required to mine this material" + UI.HORIZONTAL_BR_RULE + "Multitools can be made at Crafting Stations in the <color=#833A5FFF>Stations Tab</color> <color=#F44A47>[0]</color> of the Build Menu";
			}

			public class NEUTRONIUMUNMINABLE
			{
				public static LocString NAME = "Cannot Mine";

				public static LocString TOOLTIP = "This resource cannot be mined by Duplicant tools";
			}

			public class NEEDGASIN
			{
				public static LocString NAME = "No Gas Intake";

				public static LocString TOOLTIP = "This building has nowhere to receive gas from";
			}

			public class NEEDGASOUT
			{
				public static LocString NAME = "No Gas Output";

				public static LocString TOOLTIP = "This building has nowhere to send gas";
			}

			public class NEEDLIQUIDIN
			{
				public static LocString NAME = "No Liquid Intake";

				public static LocString TOOLTIP = "This building has nowhere to receive liquid from";
			}

			public class NEEDLIQUIDOUT
			{
				public static LocString NAME = "No Liquid Output";

				public static LocString TOOLTIP = "This building has nowhere to send liquid";
			}

			public class LIQUIDPIPEEMPTY
			{
				public static LocString NAME = "Empty Pipe";

				public static LocString TOOLTIP = "There is no liquid in this pipe";
			}

			public class LIQUIDPIPEOBSTRUCTED
			{
				public static LocString NAME = "Not Pumping";

				public static LocString TOOLTIP = "This pump is not active";
			}

			public class GASPIPEEMPTY
			{
				public static LocString NAME = "Empty Pipe";

				public static LocString TOOLTIP = "There is no gas in this pipe";
			}

			public class GASPIPEOBSTRUCTED
			{
				public static LocString NAME = "Not Pumping";

				public static LocString TOOLTIP = "This pump is not active";
			}

			public class NEEDPLANT
			{
				public static LocString NAME = "No Seeds";

				public static LocString TOOLTIP = "Uproot wild plants to obtain seeds";
			}

			public class NEEDSEED
			{
				public static LocString NAME = "No Seed Selected";

				public static LocString TOOLTIP = "Uproot wild plants to obtain seeds";
			}

			public class NEEDPOWER
			{
				public static LocString NAME = "No Power";

				public static LocString TOOLTIP = "All connected power sources have lost charge";
			}

			public class NOTENOUGHPOWER
			{
				public static LocString NAME = "Not Enough Power";

				public static LocString TOOLTIP = "Building will be useable once it has more stored charge";
			}

			public class NEEDRESOURCE
			{
				public static LocString NAME = "Resource Required";

				public static LocString TOOLTIP = "This building is missing required materials";
			}

			public class NEWDUPLICANTSAVAILABLE
			{
				public static LocString NAME = "New Duplicants Available";

				public static LocString TOOLTIP = "A new colony member is ready to be printed";

				public static LocString NOTIFICATION_NAME = "New Duplicants are available";

				public static LocString NOTIFICATION_TOOLTIP = "The Printing Pod <color=#F44A47>[H]</color> is ready to print a new Duplicant.\nPlease select a DNA blueprint:";
			}

			public class NOAPPLICABLERESEARCHSELECTED
			{
				public static LocString NAME = "Inapplicable Research";

				public static LocString TOOLTIP = "This building cannot produce the correct research type for the selected research task";

				public static LocString NOTIFICATION_NAME = "<style=\"research\">Research Center</style> idle";

				public static LocString NOTIFICATION_TOOLTIP = "These buildings cannot produce the correct <style=\"research\">Research Type</style> for the selected <style=\"research\">Research Task</style>:";
			}

			public class NOAVAILABLESEED
			{
				public static LocString NAME = "No Seed Available";

				public static LocString TOOLTIP = "The selected seed is not available";
			}

			public class NOSTORAGEFILTERSET
			{
				public static LocString NAME = "Storage Not Assigned";

				public static LocString TOOLTIP = "No resources types are marked for storage in this building";
			}

			public class NOSUITMARKER
			{
				public static LocString NAME = "No Checkpoint";

				public static LocString TOOLTIP = "Docks must be placed beside an Exosuit Checkpoint, opposite the side its checkpoint faces";
			}

			public class SUITMARKERWRONGSIDE
			{
				public static LocString NAME = "Invalid Checkpoint";

				public static LocString TOOLTIP = "This building has been built on the wrong side of an Exosuit Checkpoint\n\nExosuit Docks must be placed beside a Checkpoint, opposite the side its checkpoint faces";
			}

			public class NOFILTERELEMENTSELECTED
			{
				public static LocString NAME = "No Filter Selected";

				public static LocString TOOLTIP = "Select a resource to filter";
			}

			public class NOLUREELEMENTSELECTED
			{
				public static LocString NAME = "No Bait Selected";

				public static LocString TOOLTIP = "Select a resource to use as bait";
			}

			public class NOFISHABLEWATERBELOW
			{
				public static LocString NAME = "No Fishable Water";

				public static LocString TOOLTIP = "There are no edible fish beneath this structure";
			}

			public class NOPOWERCONSUMERS
			{
				public static LocString NAME = "No Power Consumers";

				public static LocString TOOLTIP = "No buildings are connected to this power source";
			}

			public class NOWIRECONNECTED
			{
				public static LocString NAME = "No Wire Connected";

				public static LocString TOOLTIP = "This building has not been connected to a power grid";
			}

			public class PENDINGDECONSTRUCTION
			{
				public static LocString NAME = "Deconstruction Task Assigned";

				public static LocString TOOLTIP = "Waiting for a Duplicant to deconstruct this building";
			}

			public class PENDINGFISH
			{
				public static LocString NAME = "Fishing Task Assigned";

				public static LocString TOOLTIP = "Waiting for a Duplicant to fish";
			}

			public class PENDINGHARVEST
			{
				public static LocString NAME = "Harvest Task Assigned";

				public static LocString TOOLTIP = "Waiting for a Duplicant to harvest";
			}

			public class PENDINGUPROOT
			{
				public static LocString NAME = "Uproot Task Assigned";

				public static LocString TOOLTIP = "Waiting for a Duplicant to uproot";
			}

			public class PENDINGREPAIR
			{
				public static LocString NAME = "Repair Task Assigned";

				public static LocString TOOLTIP = "Waiting for a Duplicant to repair\nReceived damage {DamageInfo}";
			}

			public class PENDINGSWITCHTOGGLE
			{
				public static LocString NAME = "Toggle Switch Task Assigned";

				public static LocString TOOLTIP = "Waiting for a Duplicant to toggle switch";
			}

			public class PENDINGWORK
			{
				public static LocString NAME = "Work Task Assigned";

				public static LocString TOOLTIP = "Waiting for a Duplicant to operate this building";
			}

			public class POWERBUTTONOFF
			{
				public static LocString NAME = "Function Suspended";

				public static LocString TOOLTIP = "This building has been toggled off\nPress Enable Building to resume its use";
			}

			public class PUMPINGSTATION
			{
				public static LocString NAME = "Liquid Available: {Liquids}";

				public static LocString TOOLTIP = "This pumping station has access to: {Liquids}";
			}

			public class PRESSUREOK
			{
				public static LocString NAME = "Max Gas Pressure";

				public static LocString TOOLTIP = "High ambient air pressure is preventing this building from emitting gas" + UI.HORIZONTAL_BR_RULE + "Reduce pressure by pumping gas away or clearing more space";
			}

			public class UNDERPRESSURE
			{
				public static LocString NAME = "Low Air Pressure";

				public static LocString TOOLTIP = "A minimum atmospheric pressure of {TargetPressure} is needed for this building to operate";
			}

			public class STORAGELOCKER
			{
				public static LocString NAME = "Storing: {Stored} / {Capacity} kg";

				public static LocString TOOLTIP = "This container is storing {Stored} kg of material";
			}

			public class UNASSIGNED
			{
				public static LocString NAME = "Unassigned";

				public static LocString TOOLTIP = "Assign a Duplicant to use this amenity";
			}

			public class UNDERCONSTRUCTION
			{
				public static LocString NAME = "Under Construction";

				public static LocString TOOLTIP = "This building is currently being built";
			}

			public class UNDERCONSTRUCTIONNOWORKER
			{
				public static LocString NAME = "Construction Task Assigned";

				public static LocString TOOLTIP = "Waiting for a Duplicant to build";
			}

			public class WAITINGFORMATERIALS
			{
				public static LocString NAME = "Awaiting Material Delivery\n{ItemsRemaining}";

				public static LocString TOOLTIP = "Waiting for a Duplicant to deliver:\n{ItemsRemaining}";

				public static LocString LINE_ITEM_MASS = "• {0}: {1}";

				public static LocString LINE_ITEM_UNITS = "• {0}";
			}

			public class WAITINGFORREPAIRMATERIALS
			{
				public static LocString NAME = "Awaiting Repair Material Delivery\n{ItemsRemaining}\n";

				public static LocString TOOLTIP = "Waiting for a Duplicant to deliver:\n{ItemsRemaining}";

				public static LocString LINE_ITEM = "• {0}: {1}";
			}

			public class NORMAL
			{
				public static LocString NAME = "Normal";

				public static LocString TOOLTIP = "Nothing out of the ordinary here";
			}

			public class MANUALGENERATORCHARGINGUP
			{
				public static LocString NAME = "Charging Up";

				public static LocString TOOLTIP = "This power source is being charged";
			}

			public class MANUALGENERATORRELEASINGENERGY
			{
				public static LocString NAME = "Powering";

				public static LocString TOOLTIP = "This generator is supplying energy to power consumers";
			}

			public class GENERATOROFFLINE
			{
				public static LocString NAME = "Generator Idle";

				public static LocString TOOLTIP = "This power source is idle";
			}

			public class PIPE
			{
				public static LocString NAME = "Contents: {Contents}";

				public static LocString TOOLTIP = "This pipe is delivering {Contents}";
			}

			public class FABRICATOREMPTY
			{
				public static LocString NAME = "No Fabrications Queued";

				public static LocString TOOLTIP = "Select a recipe to begin fabrication";
			}

			public class TOILET
			{
				public static LocString NAME = "{FlushesRemaining} \"Visits\" Remaining";

				public static LocString TOOLTIP = "{FlushesRemaining} more Duplicants can use this amenity before it requires maintenance";
			}

			public class TOILETNEEDSEMPTYING
			{
				public static LocString NAME = "Requires Emptying";

				public static LocString TOOLTIP = "This amenity cannot be used while full" + UI.HORIZONTAL_BR_RULE + "Emptying it will produce <style=\"solid\">Polluted Dirt</style>";
			}

			public class UNUSABLE
			{
				public static LocString NAME = "Out of Order";

				public static LocString TOOLTIP = "This amenity requires maintenance";
			}

			public class NORESEARCHSELECTED
			{
				public static LocString NAME = "No Research Task Selected";

				public static LocString TOOLTIP = "Open the <color=#833A5FFF>Research Tree</color> [R] to select a new <style=\"research\">Research</style> project";

				public static LocString NOTIFICATION_NAME = "No <style=\"research\">Research Task</style> selected";

				public static LocString NOTIFICATION_TOOLTIP = "Open the <color=#833A5FFF>Research Tree</color> [R] to select a new <style=\"research\">Research</style> project";
			}

			public class RESEARCHING
			{
				public static LocString NAME = "Current <style=\"research\">Research</style>: {Tech}";

				public static LocString TOOLTIP = "Research produced at this station will be invested in {Tech}";
			}

			public class VALVE
			{
				public static LocString NAME = "Max Flow Rate: {MaxFlow}";

				public static LocString TOOLTIP = "This valve is allowing flow at a volume of {MaxFlow}";
			}

			public class VALVEREQUEST
			{
				public static LocString NAME = "Requested Flow Rate: {QueuedMaxFlow}";

				public static LocString TOOLTIP = "Waiting for a Duplicant to adjust flow rate";
			}

			public class EMITTINGLIGHT
			{
				public static LocString NAME = "Emitting Light";

				public static LocString TOOLTIP = "Open the Light Overlay [{LightGridOverlay}] to view this light's visibility radius";
			}

			public class RATIONBOXCONTENTS
			{
				public static LocString NAME = "Storing: {Stored}";

				public static LocString TOOLTIP = "This box contains {Stored} of food";
			}

			public class EMITTINGELEMENT
			{
				public static LocString NAME = "Emitting {ElementType}: {FlowRate}";

				public static LocString TOOLTIP = "Producing {ElementType} at {FlowRate}";
			}

			public class EMITTINGCO2
			{
				public static LocString NAME = "Emitting CO<sub>2</sub>: {FlowRate}";

				public static LocString TOOLTIP = "Producing CO<sub>2</sub> at {FlowRate}";
			}

			public class EMITTINGOXYGENAVG
			{
				public static LocString NAME = "Emitting <style=\"oxygen\">Oxygen</style>: {FlowRate}";

				public static LocString TOOLTIP = "Producing oxygen at a rate of {FlowRate}";
			}

			public class EMITTINGGASAVG
			{
				public static LocString NAME = "Emitting <style=\"gas\">{Element}</style>: {FlowRate}";

				public static LocString TOOLTIP = "Producing {Element} at a rate of {FlowRate}";
			}

			public class PUMPINGLIQUIDORGAS
			{
				public static LocString NAME = "Average Flow Rate: {FlowRate}";

				public static LocString TOOLTIP = "This building is pumping an average volume of {FlowRate}";
			}

			public class WIRECIRCUITSTATUS
			{
				public static LocString NAME = "Circuit Status: {CurrentLoad} / {MaxLoad}";

				public static LocString TOOLTIP = "The current load on this circuit";
			}

			public class WIREMAXWATTAGESTATUS
			{
				public static LocString NAME = "Max Wattage: {WireMaxWattage}";

				public static LocString TOOLTIP = "The maximum wattage that this wire can safely sustain";
			}

			public class NOLIQUIDELEMENTTOPUMP
			{
				public static LocString NAME = "Pump Not In Liquid";

				public static LocString TOOLTIP = "This pump must be submerged in liquid to work";
			}

			public class NOGASELEMENTTOPUMP
			{
				public static LocString NAME = "Pump Not In Gas";

				public static LocString TOOLTIP = "This pump must be submerged in gas to work";
			}

			public class ELEMENTEMITTEROUTPUT
			{
				public static LocString NAME = "Emitting {ElementTypes}: {FlowRate}";

				public static LocString TOOLTIP = "This object is releasing {ElementTypes} at a rate of {FlowRate}";
			}

			public class ELEMENTCONSUMER
			{
				public static LocString NAME = "Consuming {ElementTypes}: {FlowRate}";

				public static LocString TOOLTIP = "This building is utilizing ambient {ElementTypes} from the environment";
			}

			public class CONSUMINGFROMSTORAGE
			{
				public static LocString NAME = "Consuming {ElementTypes}: {FlowRate}";

				public static LocString TOOLTIP = "This building is consuming {ElementTypes} from storage";
			}

			public class ELEMENTCONVERTEROUTPUT
			{
				public static LocString NAME = "Emitting {ElementTypes}: {FlowRate}";

				public static LocString TOOLTIP = "This building is releasing {ElementTypes} at a rate of {FlowRate}";
			}

			public class ELEMENTCONVERTERINPUT
			{
				public static LocString NAME = "Using {ElementTypes}: {FlowRate}";

				public static LocString TOOLTIP = "This building is using {ElementTypes} from storage at a rate of {FlowRate}";
			}

			public class AWAITINGCOMPOSTFLIP
			{
				public static LocString NAME = "Requires Flipping";

				public static LocString TOOLTIP = "Compost must be flipped periodically to produce fertilizer";
			}

			public class AWAITINGWASTE
			{
				public static LocString NAME = "Awaiting Compostables";

				public static LocString TOOLTIP = "More waste material is required to begin the composting process";
			}

			public class BATTERIESSUFFICIENTLYFULL
			{
				public static LocString NAME = "Batteries Sufficiently Full";

				public static LocString TOOLTIP = "All batteries are above the refill threshold";
			}

			public class INSUFFICIENT_CONVERSION_MASS
			{
				public static LocString NAME = "Insufficient Resources";

				public static LocString TOOLTIP = "The mass of material that was delivered to this building was too low\n\nDeliver more material to run this building";
			}

			public class JOULESAVAILABLE
			{
				public static LocString NAME = "<style=\"power\">Power</style> Available: {JoulesAvailable}";

				public static LocString TOOLTIP = "{JoulesAvailable} of stored power available for use";
			}

			public class WATTAGE
			{
				public static LocString NAME = "Wattage: {Wattage}";

				public static LocString TOOLTIP = "This building is generating {Wattage} of power";
			}

			public class WATTSON
			{
				public static LocString NAME = "Next Duplicant: {TimeRemaining}";

				public static LocString TOOLTIP = "The Printing Pod can print out new Duplicants over time.\nThe next one will be ready in {TimeRemaining}";

				public static LocString UNAVAILABLE = "UNAVAILABLE";
			}

			public class FLUSHTOILET
			{
				public static LocString NAME = "Lavatory Ready";

				public static LocString TOOLTIP = "This bathroom is ready to receive visitors";
			}

			public class FLUSHTOILETINUSE
			{
				public static LocString NAME = "Lavatory In Use";

				public static LocString TOOLTIP = "This bathroom is occupied";
			}

			public class WIRECONNECTED
			{
				public static LocString NAME = "Wire Connected";

				public static LocString TOOLTIP = "This wire is connected to a network";
			}

			public class WIRENOMINAL
			{
				public static LocString NAME = "Wire Nominal";

				public static LocString TOOLTIP = "This wire is in good condition";
			}

			public class WIREDISCONNECTED
			{
				public static LocString NAME = "Wire Disconnected";

				public static LocString TOOLTIP = "This wire is not connecting a power consumer to a generator";
			}

			public class COOLING
			{
				public static LocString NAME = "Cooling";

				public static LocString TOOLTIP = "This building is cooling the surrounding area";
			}

			public class COOLINGSTALLEDHOTENV
			{
				public static LocString NAME = "Gas Too Hot";

				public static LocString TOOLTIP = "Incoming pipe contents cannot be cooled more than {2} below the surrounding environment\n\nEnvironment: {0}\nCurrent Pipe Contents: {1}";
			}

			public class COOLINGSTALLEDCOLDGAS
			{
				public static LocString NAME = "Gas Too Cold";

				public static LocString TOOLTIP = "This building cannot cool incoming pipe contents below {0}\n\nCurrent Pipe Contents: {0}";
			}

			public class COOLINGSTALLEDHOTLIQUID
			{
				public static LocString NAME = "Liquid Too Hot";

				public static LocString TOOLTIP = "Incoming pipe contents cannot be cooled more than {2} below the surrounding environment\n\nEnvironment: {0}\nCurrent Pipe Contents: {1}";
			}

			public class COOLINGSTALLEDCOLDLIQUID
			{
				public static LocString NAME = "Liquid Too Cold";

				public static LocString TOOLTIP = "This building cannot cool incoming pipe contents below {0}\n\nCurrent Pipe Contents: {0}";
			}

			public class CANNOTCOOLFURTHER
			{
				public static LocString NAME = "Minimum Temperature Reached";

				public static LocString TOOLTIP = "This building cannot cool the surrounding environment below {0}";
			}

			public class HEATINGSTALLEDHOTENV
			{
				public static LocString NAME = "Max Temperature Reached";

				public static LocString TOOLTIP = "This building cannot heat the surrounding environment beyond {0}";
			}

			public class HEATINGSTALLEDLOWMASS_GAS
			{
				public static LocString NAME = "Low Air Pressure";

				public static LocString TOOLTIP = "A minimum atmospheric pressure of {TargetPressure} is needed for this building to heat up";
			}

			public class HEATINGSTALLEDLOWMASS_LIQUID
			{
				public static LocString NAME = "Not Submerged In Liquid";

				public static LocString TOOLTIP = "This building must be submerged in liquid to function";
			}

			public class BUILDINGDISABLED
			{
				public static LocString NAME = "Building Disabled";

				public static LocString TOOLTIP = "Press <color=#F44A47>[ENTER]</color> to resume use";
			}

			public class WORKING
			{
				public static LocString NAME = "Nominal";

				public static LocString TOOLTIP = "This building is working as intended";
			}

			public class NEEDSREGION
			{
				public static LocString NAME = "Missing Region";

				public static LocString TOOLTIP = "This building must be inside a {0} region";
			}

			public class NEEDSVALIDREGION
			{
				public static LocString NAME = "Valid Region Required";

				public static LocString TOOLTIP = "This building region has not yet met its requirements";
			}

			public class GRAVEEMPTY
			{
				public static LocString NAME = "Empty";

				public static LocString TOOLTIP = "This memorial honors no one.";
			}

			public class GRAVE
			{
				public static LocString NAME = "RIP {DeadDupe}";

				public static LocString TOOLTIP = "{Epitaph}";
			}

			public class AWAITINGARTING
			{
				public static LocString NAME = "Incomplete Artwork";

				public static LocString TOOLTIP = "This building requires a Duplicant's artistic touch";
			}

			public class LOOKINGUGLY
			{
				public static LocString NAME = "Crude";

				public static LocString TOOLTIP = "Honestly, Morbs could've done better";
			}

			public class LOOKINGOKAY
			{
				public static LocString NAME = "Quaint";

				public static LocString TOOLTIP = "Duplicants find this art piece quite charming";
			}

			public class LOOKINGGREAT
			{
				public static LocString NAME = "Masterpiece";

				public static LocString TOOLTIP = "This poignant piece stirs something deep within each Duplicant's soul";
			}

			public class EXPIRED
			{
				public static LocString NAME = "Depleted";

				public static LocString TOOLTIP = "This building has no more use";
			}

			public class EXCAVATOR_BOMB
			{
				public class UNARMED
				{
					public static LocString NAME = "Unarmed";

					public static LocString TOOLTIP = "This explosive is currently inactive";
				}

				public class ARMED
				{
					public static LocString NAME = "Armed";

					public static LocString TOOLTIP = "Stand back, this baby's ready to blow!";
				}

				public class COUNTDOWN
				{
					public static LocString NAME = "Countdown: {0}";

					public static LocString TOOLTIP = "{0} seconds until detonation";
				}

				public class DUPE_DANGER
				{
					public static LocString NAME = "Duplicant Preservation Override";

					public static LocString TOOLTIP = "Explosive disabled due to close Duplicant proximity";
				}

				public class EXPLODING
				{
					public static LocString NAME = "Exploding";

					public static LocString TOOLTIP = "Kaboom!";
				}
			}

			public class BURNER
			{
				public class BURNING_FUEL
				{
					public static LocString NAME = "Consuming Fuel: {0}";

					public static LocString TOOLTIP = "{0} fuel remaining";
				}

				public class HAS_FUEL
				{
					public static LocString NAME = "Fueled: {0}";

					public static LocString TOOLTIP = "{0} fuel remaining";
				}
			}

			public class CREATURE_TRAP
			{
				public class NEEDSBAIT
				{
					public static LocString NAME = "Needs Bait";

					public static LocString TOOLTIP = "This trap needs to be baited before it can be set";
				}

				public class READY
				{
					public static LocString NAME = "Set";

					public static LocString TOOLTIP = "This trap has been set and is ready to catch a creature";
				}

				public class SPRUNG
				{
					public static LocString NAME = "Sprung";

					public static LocString TOOLTIP = "This trap has caught a {0}!";
				}
			}

			public class ACCESS_CONTROL
			{
				public class ACTIVE
				{
					public static LocString NAME = "Access Restrictions";

					public static LocString TOOLTIP = "Some Duplicants are prohibited from passing through this door by the current access settings";
				}

				public class OFFLINE
				{
					public static LocString NAME = "Access Control Offline";

					public static LocString TOOLTIP = "This door has granted Emergency Access Permissions" + UI.HORIZONTAL_BR_RULE + "All Duplicants are permitted to pass through it until power is restored";
				}
			}

			public class REQUIRESMINER
			{
				public static LocString NAME = "Requires Miner";

				public static LocString TOOLTIP = "Only Duplicants assigned to one of the Miner roles can dig this tile";
			}

			public class REQUIRESRESEARCHER
			{
				public static LocString NAME = "Requires Researcher";

				public static LocString TOOLTIP = "Only Duplicants assigned to one of the Researcher roles can work this building";
			}

			public class REQUIRESROLE
			{
				public static LocString NAME = "Requires {Role}";

				public static LocString TOOLTIP = "Only Duplicants assigned to the {Role} role can work this building";
			}

			public class SWITCHSTATUSACTIVE
			{
				public static LocString NAME = "Active";

				public static LocString TOOLTIP = "This switch is currently toggled on";
			}

			public class SWITCHSTATUSINACTIVE
			{
				public static LocString NAME = "Inactive";

				public static LocString TOOLTIP = "This switch is currently toggled off";
			}

			public class FOOD_CONTAINERS_OUTSIDE_RANGE
			{
				public static LocString NAME = "Unreachable food";

				public static LocString TOOLTIP = "Recuperating Duplicants must have food available within {0} cells";
			}

			public class TOILETS_OUTSIDE_RANGE
			{
				public static LocString NAME = "Unreachable restroom";

				public static LocString TOOLTIP = "Recuperating Duplicants must have toilets available within {0} cells";
			}

			public class TURBINE_BLOCKED_OUTPUT
			{
				public static LocString NAME = "Output Blocked";

				public static LocString TOOLTIP = "This turbine's output is blocked";
			}

			public class TURBINE_INSUFFICIENT_MASS
			{
				public static LocString NAME = "Underpowered";

				public static LocString TOOLTIP = "This turbine requires {MASS} of {ELEMENT} at {TEMPERATURE} to run";
			}

			public class TURBINE_SPINNING_UP
			{
				public static LocString NAME = "Warming Up";

				public static LocString TOOLTIP = "The turbine is spinning up";
			}

			public class WELL_PRESSURIZING
			{
				public static LocString NAME = "Backpressure: {0}";

				public static LocString TOOLTIP = "Well pressure increases with each use and must be periodically relieved to prevent shutdown";
			}

			public class WELL_OVERPRESSURE
			{
				public static LocString NAME = "Overpressure";

				public static LocString TOOLTIP = "This well can no longer function due to excessive backpressure";
			}

			public class RELEASING_PRESSURE
			{
				public static LocString NAME = "Releasing Pressure";

				public static LocString TOOLTIP = "Pressure buildup is being safely released";
			}

			public class LOGIC_FEEDBACK_LOOP
			{
				public static LocString NAME = "Feedback Loop";

				public static LocString TOOLTIP = "Feedback loops prevent automation grids from functioning\n\nFeedback loops occur when the Output of an automated building connects back to its own Input through the Automation grid";
			}

			public class ENOUGH_COOLANT
			{
				public static LocString NAME = "Awaiting Coolant";

				public static LocString TOOLTIP = "{1} of {0} must be present in storage to begin production";
			}

			public class LOGIC
			{
				public static LocString LOGIC_CONTROLLED_ENABLED = "Enabled by Automation Grid";

				public static LocString LOGIC_CONTROLLED_DISABLED = "Disabled by Automation Grid";
			}

			public class TOO_COLD
			{
				public static LocString NAME = "Too Cold";

				public static LocString TOOLTIP = "Either this building or its surrounding environment is too cold to operate";
			}

			public class CHECKPOINT
			{
				public static LocString LOGIC_CONTROLLED_OPEN = "Clearance: Permitted";

				public static LocString LOGIC_CONTROLLED_CLOSED = "Clearance: Not Permitted";

				public static LocString LOGIC_CONTROLLED_DISCONNECTED = "No Automation";

				public class TOOLTIPS
				{
					public static LocString LOGIC_CONTROLLED_OPEN = "Automated Checkpoint is on Standby, preventing Duplicants from passing";

					public static LocString LOGIC_CONTROLLED_CLOSED = "Automated Checkpoint is Active, allowing Duplicants to pass";

					public static LocString LOGIC_CONTROLLED_DISCONNECTED = "This Checkpoint has not been connected to an automation grid";
				}
			}

			public class AWAITINGFUEL
			{
				public static LocString NAME = "Awaiting Fuel: {0}";

				public static LocString TOOLTIP = "This building requires {1} of {0} to operate";
			}

			public class NOLOGICWIRECONNECTED
			{
				public static LocString NAME = "No Automation Wire Connected";

				public static LocString TOOLTIP = "This building has not been connected to an automation grid";
			}

			public class NOTUBECONNECTED
			{
				public static LocString NAME = "No Transit Tube Connected";

				public static LocString TOOLTIP = "A straight Transit Tube must be connected to the top of the launch rail for this building to function.";
			}

			public class STOREDCHARGE
			{
				public static LocString NAME = "Stored Charge: {0}/{1}";

				public static LocString TOOLTIP = "This building has stored {0} out of a maximum {1}. It will consume {2} per use.";
			}
		}

		public class DETAILS
		{
			public static LocString USE_COUNT = "Uses: {0}";

			public static LocString USE_COUNT_TOOLTIP = "This building has been used {0} times";
		}
	}
}
