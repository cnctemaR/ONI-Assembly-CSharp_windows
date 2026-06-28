using System;

namespace STRINGS
{
	public class BUILDING
	{
		public class STATUSITEMS
		{
			public class PIPECONTENTS
			{
				public static LocString NAME = "{0} of {1} at {2}";
			}

			public class ASSIGNEDTO
			{
				public static LocString NAME = "Assigned to: {Assignee}";

				public static LocString TOOLTIP = "Only {Assignee} can use this amenity";
			}

			public class AWAITINGSEEDDELIVERY
			{
				public static LocString NAME = "Awaiting delivery";

				public static LocString TOOLTIP = "Awaiting delivery of selected <style=\"seed\">Seed</style>";
			}

			public class BROKEN
			{
				public static LocString NAME = "Broken";

				public static LocString TOOLTIP = "This building was broken by a <style=\"stress\">Stressed</style> Duplicant\n\nIt must be repaired";
			}

			public class CHANGEDOORCONTROLSTATE
			{
				public static LocString NAME = "Pending Door State Change: {ControlState}";

				public static LocString TOOLTIP = "Waiting for a Duplicant to change control state";
			}

			public class CURRENTDOORCONTROLSTATE
			{
				public static LocString NAME = "Current State: {ControlState}";

				public static LocString TOOLTIP = "Current State: {ControlState}\n\nAuto: Duplicants will open and close this door as needed\nClosed: This door will remain closed\nOpen: This door will remain open";

				public static LocString OPENED = "Opened";

				public static LocString AUTO = "Auto";

				public static LocString CLOSED = "Closed";
			}

			public class CONDUITBLOCKED
			{
				public static LocString NAME = "Pipe Blocked";

				public static LocString TOOLTIP = "This pipe system is not serviceable";
			}

			public class CONSTRUCTIONUNREACHABLE
			{
				public static LocString NAME = "Unreachable";

				public static LocString TOOLTIP = "Duplicants cannot reach this construction site";
			}

			public class DIGUNREACHABLE
			{
				public static LocString NAME = "Unreachable";

				public static LocString TOOLTIP = "Duplicants cannot reach this area";
			}

			public class ENTOMBED
			{
				public static LocString NAME = "Entombed";

				public static LocString TOOLTIP = "Must be dug out by a Duplicant";

				public static LocString NOTIFICATION_NAME = "Building entombment";

				public static LocString NOTIFICATION_TOOLTIP = "These buildings are entombed and need to be dug out:";
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

				public static LocString TOOLTIP = "Cannot release gas through this vent";
			}

			public class GASVENTOVERPRESSURE
			{
				public static LocString NAME = "Gas Vent Overpressurized";

				public static LocString TOOLTIP = "Vent cannot function at current pressure";
			}

			public class WATTSONGAMEOVER
			{
				public static LocString NAME = "Colony Lost";

				public static LocString TOOLTIP = "Colony Lost";
			}

			public class INVALIDBUILDINGLOCATION
			{
				public static LocString NAME = "Invalid Building Location";

				public static LocString TOOLTIP = "Cannot construct building in this location";
			}

			public class LIQUIDVENTOBSTRUCTED
			{
				public static LocString NAME = "Liquid Vent Obstructed";

				public static LocString TOOLTIP = "Cannot release liquid through this vent";
			}

			public class LIQUIDVENTOVERPRESSURE
			{
				public static LocString NAME = "Liquid Vent Overpressurized";

				public static LocString TOOLTIP = "Vent cannot function at current pressure";
			}

			public class MANUALLYCONTROLLED
			{
				public static LocString NAME = "Manually Controlled";

				public static LocString TOOLTIP = "This Duplicant is under your control";
			}

			public class MATERIALSUNAVAILABLE
			{
				public static LocString NAME = "Insufficient Resources\n{ItemsRemaining}";

				public static LocString TOOLTIP = "Materials for this building are beyond reach or unavailable";

				public static LocString NOTIFICATION_NAME = "Insufficient resources";

				public static LocString NOTIFICATION_TOOLTIP = "Crucial materials are unavailable or beyond reach for these buildings:";
			}

			public class MATERIALSUNAVAILABLEFORREFILL
			{
				public static LocString NAME = "Resources Low\n{ItemsRemaining}";

				public static LocString TOOLTIP = "This building will soon require materials that are unavailable";
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

				public static LocString TOOLTIP = "Build Tiles beneath this building\n------------------\nTiles can be found in the Base Tab <color=#FF0000>(1)</color> of the Build Menu";
			}

			public class NEEDBORINGMACHINE
			{
				public static LocString NAME = "Multitool Required";

				public static LocString TOOLTIP = "A <style=\"equipment\">Multitool</style> is required to mine this material\n------------------\nMultitools can be made at Crafting Stations in the Stations Tab <color=#FF0000>(9)</style> of the Build Menu";
			}

			public class NEUTRONIUMUNMINABLE
			{
				public static LocString NAME = "Cannot Mine";

				public static LocString TOOLTIP = "This material cannot be mined by Duplicant tools";
			}

			public class NEEDGASIN
			{
				public static LocString NAME = "No Gas Intake";

				public static LocString TOOLTIP = "This building has nowhere to receive <style=\"gas\">Gas</style> from";
			}

			public class NEEDGASOUT
			{
				public static LocString NAME = "No Gas Output";

				public static LocString TOOLTIP = "This building has nowhere to send <style=\"gas\">Gas</style>";
			}

			public class NEEDLIQUIDIN
			{
				public static LocString NAME = "No Liquid Intake";

				public static LocString TOOLTIP = "This building has nowhere to receive <style=\"liquid\">Liquid</style> from";
			}

			public class NEEDLIQUIDOUT
			{
				public static LocString NAME = "No Liquid Output";

				public static LocString TOOLTIP = "This building has nowhere to send <style=\"liquid\">Liquid</style>";
			}

			public class LIQUIDPIPEEMPTY
			{
				public static LocString NAME = "Empty Pipe";

				public static LocString TOOLTIP = "There is no <style=\"liquid\">Liquid</style> in the pipe";
			}

			public class LIQUIDPIPEOBSTRUCTED
			{
				public static LocString NAME = "Not Pumping";

				public static LocString TOOLTIP = "This pump is not active";
			}

			public class GASPIPEEMPTY
			{
				public static LocString NAME = "Empty Pipe";

				public static LocString TOOLTIP = "There is no <style=\"Gas\">Gas</style> in the pipe";
			}

			public class GASPIPEOBSTRUCTED
			{
				public static LocString NAME = "Not Pumping";

				public static LocString TOOLTIP = "This pump is not active";
			}

			public class NEEDPLANT
			{
				public static LocString NAME = "No Seeds";

				public static LocString TOOLTIP = "Dig up wild <style=\"plant\">Plants</style> to obtain <style=\"seed\">Seeds</style>";
			}

			public class NEEDSEED
			{
				public static LocString NAME = "No Seed selected";

				public static LocString TOOLTIP = "Dig up wild plants to obtain <style=\"seed\">Seeds</style>";
			}

			public class NEEDPOWER
			{
				public static LocString NAME = "No Power";

				public static LocString TOOLTIP = "All connected <style=\"power\">Power</style> sources have lost charge";
			}

			public class NEEDRESOURCE
			{
				public static LocString NAME = "Resource Required";

				public static LocString TOOLTIP = "This building is missing required materials";
			}

			public class NEWDUPLICANTSAVAILABLE
			{
				public static LocString NAME = "New Duplicants Available";

				public static LocString TOOLTIP = "A new colony member is ready be printed";

				public static LocString NOTIFICATION_NAME = "New Duplicants are available";

				public static LocString NOTIFICATION_TOOLTIP = "The Printing Pod is ready to print a new colony member\n\nPlease select a DNA blueprint";
			}

			public class NOAPPLICABLERESEARCHSELECTED
			{
				public static LocString NAME = "Inapplicable Research";

				public static LocString TOOLTIP = "This building cannot produce the correct <style=\"research\">Research Type</style> for the selected <style=\"research\">Research Task</style>";

				public static LocString NOTIFICATION_NAME = "<style=\"research\">Research Center</style> idle";

				public static LocString NOTIFICATION_TOOLTIP = "These buildings cannot produce the correct <style=\"research\">Research Type</style> for the selected <style=\"research\">Research Task</style>:";
			}

			public class NOAVAILABLESEED
			{
				public static LocString NAME = "No Seed available";

				public static LocString TOOLTIP = "The selected <style=\"seed\">Seed</style> is not available";
			}

			public class NOSTORAGEFILTERSET
			{
				public static LocString NAME = "Storage Not Allocated";

				public static LocString TOOLTIP = "No resources types are marked for storage in this building";
			}

			public class NOFILTERELEMENTSELECTED
			{
				public static LocString NAME = "No Filter Selected";

				public static LocString TOOLTIP = "Select a resource to filter";
			}

			public class NOFISHABLEWATERBELOW
			{
				public static LocString NAME = "No Fishable Water Below";

				public static LocString TOOLTIP = "There are no edible fish beneath this structure";
			}

			public class NOPOWERCONSUMERS
			{
				public static LocString NAME = "No Power Consumers";

				public static LocString TOOLTIP = "No buildings are connected to this <style=\"power\">Power</style> source";
			}

			public class NOPOWERSOURCE
			{
				public static LocString NAME = "No Power";

				public static LocString TOOLTIP = "This building must be connected to a <style=\"power\">Power</style> source";
			}

			public class NOWIRECONNECTED
			{
				public static LocString NAME = "No Wire Connected";

				public static LocString TOOLTIP = "This building has not been connected to a <style=\"power\">Power</style> grid";
			}

			public class PENDINGDECONSTRUCTION
			{
				public static LocString NAME = "Deconstruction Pending";

				public static LocString TOOLTIP = "Waiting for a Duplicant to deconstruct this building";
			}

			public class PENDINGFISH
			{
				public static LocString NAME = "Fishing Pending";

				public static LocString TOOLTIP = "Waiting for a Duplicant to fish";
			}

			public class PENDINGHARVEST
			{
				public static LocString NAME = "Harvest Pending";

				public static LocString TOOLTIP = "Waiting for a Duplicant to harvest";
			}

			public class PENDINGUPROOT
			{
				public static LocString NAME = "Dig Up Pending";

				public static LocString TOOLTIP = "Waiting for a Duplicant to dig up";
			}

			public class PENDINGREPAIR
			{
				public static LocString NAME = "Repair Pending";

				public static LocString TOOLTIP = "Waiting for a Duplicant to repair";
			}

			public class PENDINGSWITCHTOGGLE
			{
				public static LocString NAME = "Toggle Switch Pending";

				public static LocString TOOLTIP = "Waiting for a Duplicant to toggle switch";
			}

			public class PENDINGWORK
			{
				public static LocString NAME = "Work Pending";

				public static LocString TOOLTIP = "Waiting for a Duplicant to operate this building";
			}

			public class POWERBUTTONOFF
			{
				public static LocString NAME = "Function Suspended";

				public static LocString TOOLTIP = "This building has been toggled off\n\nPress Enable Building to resume its use";
			}

			public class PRESSUREOK
			{
				public static LocString NAME = "Max Gas Pressure";

				public static LocString TOOLTIP = "Maximum air pressurization has been reached";
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
				public static LocString NAME = "Pending Construction";

				public static LocString TOOLTIP = "Waiting for a Duplicant to build";
			}

			public class WAITINGFORMATERIALS
			{
				public static LocString NAME = "Awaiting Material Delivery\n{ItemsRemaining}";

				public static LocString TOOLTIP = "Waiting for a Duplicant to deliver:\n{ItemsRemaining}";
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

				public static LocString TOOLTIP = "This <style=\"power\">Power</style> source is supplying <style=\"power\">Power</style> consumers";
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

				public static LocString TOOLTIP = "Queue a recipe to begin fabrication";
			}

			public class TOILET
			{
				public static LocString NAME = "{FlushesRemaining} \"Visits\" Remaining";

				public static LocString TOOLTIP = "This amenity can handle {FlushesRemaining} more \"visits\" before needing maintenance";
			}

			public class TOILETNEEDSEMPTYING
			{
				public static LocString NAME = "Requires Emptying";

				public static LocString TOOLTIP = "This amenity cannot be used while full\n------------------\nEmptying it will produce <style=\"solid\">Contaminated Dirt</style>";
			}

			public class UNUSABLE
			{
				public static LocString NAME = "Out of Order";

				public static LocString TOOLTIP = "This amenity requires maintenance";
			}

			public class NORESEARCHSELECTED
			{
				public static LocString NAME = "No Research Task selected";

				public static LocString TOOLTIP = "Open the <color=#833A5FFF>RESEARCH TREE</color> [R] to select a new <style=\"research\">Research</style> project";

				public static LocString NOTIFICATION_NAME = "No <style=\"research\">Research Task</style> selected";

				public static LocString NOTIFICATION_TOOLTIP = "Open the <color=#833A5FFF>RESEARCH TREE</color> [R] to select a new <style=\"research\">Research</style> project";
			}

			public class RESEARCHING
			{
				public static LocString NAME = "Current <style=\"research\">Research</style>: {Tech}";

				public static LocString TOOLTIP = "<style=\"research\">Research</style> produced at this station will be invested in {Tech}";
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

				public static LocString TOOLTIP = "Open the Light Overlay [{LightGridOverlay}] to view this <style=\"light\">Light</style>'s visibility radius";
			}

			public class RATIONBOXCONTENTS
			{
				public static LocString NAME = "Storing: {Stored}";

				public static LocString TOOLTIP = "This box contains {Stored} of <style=\"food\">Food</style>";
			}

			public class EMITTINGELEMENT
			{
				public static LocString NAME = "Emitting {ElementType}: {FlowRate}";

				public static LocString TOOLTIP = "Producing {ElementType} at {FlowRate}";
			}

			public class EMITTINGCO2
			{
				public static LocString NAME = "Emitting CO2: {FlowRate}";

				public static LocString TOOLTIP = "Producing CO2 at {FlowRate}";
			}

			public class EMITTINGOXYGENAVG
			{
				public static LocString NAME = "Emitting <style=\"oxygen\">Oxygen</style>: {FlowRate}";

				public static LocString TOOLTIP = "Producing <style=\"oxygen\">Oxygen</style> at {FlowRate}";
			}

			public class EMITTINGGASAVG
			{
				public static LocString NAME = "Emitting <style=\"gas\">{Element}</style>: {FlowRate}";

				public static LocString TOOLTIP = "Producing <style=\"gas\">{Element}</style> at {FlowRate}";
			}

			public class PUMPINGLIQUIDORGAS
			{
				public static LocString NAME = "Average Flow Rate: {FlowRate}";

				public static LocString TOOLTIP = "This building is pumping an average volume of {FlowRate}";
			}

			public class NOLIQUIDELEMENTTOPUMP
			{
				public static LocString NAME = "Pump Not In Liquid";

				public static LocString TOOLTIP = "This pump must be submerged in <style=\"liquid\">Liquid</style> to work";
			}

			public class NOGASELEMENTTOPUMP
			{
				public static LocString NAME = "Pump Not In Gas";

				public static LocString TOOLTIP = "This pump must be submerged in <style=\"gas\">Gas</style> to work";
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
				public static LocString NAME = "Requires flipping";

				public static LocString TOOLTIP = "Compost must be flipped periodically to produce <style=\"solid\">Fertilizer</style>";
			}

			public class AWAITINGWASTE
			{
				public static LocString NAME = "Awaiting compostables";

				public static LocString TOOLTIP = "More waste material is required to begin to composting process";
			}

			public class JOULESAVAILABLE
			{
				public static LocString NAME = "<style=\"power\">Power</style> Available: {JoulesAvailable}";

				public static LocString TOOLTIP = "{JoulesAvailable} of stored power available for use";
			}

			public class WATTAGE
			{
				public static LocString NAME = "Wattage: {Wattage}";

				public static LocString TOOLTIP = "This building is generating {Wattage} of <style=\"power\">Power</style>";
			}

			public class WATTSON
			{
				public static LocString NAME = "Next Duplicant: {TimeRemaining}s";

				public static LocString TOOLTIP = "The Printing Pod can print out new Duplicants over time.\nThe next one will be ready in {TimeRemaining}s";
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

			public class CANNOTCOOLFURTHER
			{
				public static LocString NAME = "Cannot cool further";

				public static LocString TOOLTIP = "This building cannot cool the surrounding area any further";
			}

			public class BUILDINGDISABLED
			{
				public static LocString NAME = "Building Disabled";

				public static LocString TOOLTIP = "This building has been disabled.";
			}

			public class WORKING
			{
				public static LocString NAME = "Functioning";

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
				public static LocString NAME = "Awaiting decoration";

				public static LocString TOOLTIP = "A Duplicant must work on this to create art";
			}

			public class LOOKINGUGLY
			{
				public static LocString NAME = "Crude";

				public static LocString TOOLTIP = "Honestly, a Morb could do better than this";
			}

			public class LOOKINGOKAY
			{
				public static LocString NAME = "Quaint";

				public static LocString TOOLTIP = "Duplicants find this art quite charming";
			}

			public class LOOKINGGREAT
			{
				public static LocString NAME = "Masterpiece";

				public static LocString TOOLTIP = "This poignant piece stirs something deep within Duplicants' souls";
			}
		}
	}
}
