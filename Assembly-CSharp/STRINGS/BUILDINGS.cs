using System;

namespace STRINGS
{
	public class BUILDINGS
	{
		public class PREFABS
		{
			public class AIRCONDITIONER
			{
				public static LocString NAME = "Thermo Regulator";

				public static LocString DESC = "A thermo regulator doesn't remove heat, but relocates it to a new area.";

				public static LocString EFFECT = "Cools the <style=\"gas\">Gas</style> piped through it, but outputs <style=\"heat\">Heat</style> in its immediate vicinity.";
			}

			public class ALGAEDISTILLERY
			{
				public static LocString NAME = "Algae Distiller";

				public static LocString DESC = "Algae distillers convert disease-causing slime into algae for oxygen production.";

				public static LocString EFFECT = "Converts <style=\"misc\">Slime</style> into <style=\"misc\">Algae</style>.";
			}

			public class FERTILIZERMAKER
			{
				public static LocString NAME = "Fertilizer Synthesizer";

				public static LocString DESC = "Fertilizer Synthesizers convert polluted water into fertilizer for domestic plants.";

				public static LocString EFFECT = "Uses <style=\"liquid\">Polluted Water</style> to produce <style=\"solid\">Fertilizer</style>.";
			}

			public class ALGAEHABITAT
			{
				public static LocString NAME = "Algae Terrarium";

				public static LocString DESC = "Placing algae terrariums in well lit areas will produce more oxygen.";

				public static LocString EFFECT = "Recycles <style=\"gas\">Carbon Dioxide</style> into a bit of <style=\"oxygen\">Oxygen</style>.\n\nGains a minor efficiency boost in direct <style=\"light\">Light</style>.";
			}

			public class BATTERY
			{
				public static LocString NAME = "Tiny Battery";

				public static LocString DESC = "Batteries allow extra power from generators to be stored for later, rather than lost.";

				public static LocString EFFECT = "Stores a bit of runoff <style=\"power\">Power</style> from generators, but loses charge over time.";
			}

			public class BATTERYMEDIUM
			{
				public static LocString NAME = "Battery";

				public static LocString DESC = "Larger batteries hold more power and keep systems running longer before recharging.";

				public static LocString EFFECT = "Stores most runoff <style=\"power\">Power</style> from generators, but loses charge over time.";
			}

			public class BED
			{
				public static LocString NAME = "Cot";

				public static LocString DESC = "Duplicants without a private bed will develop sore backs from sleeping on the floor.";

				public static LocString EFFECT = "Provides a sleeping area for one Duplicant and relieves <style=\"stress\">Stress</style>.\n\nDuplicants will automatically sleep in their assigned cots at night.";
			}

			public class BOTTLEEMPTIER
			{
				public static LocString NAME = "Bottle Emptier";

				public static LocString DESC = "A bottle emptier's Element Filter can be used to designate areas for specific liquid storage.";

				public static LocString EFFECT = "Empties bottled <style=\"liquid\">Liquids</style> back into the world.";
			}

			public class LIQUIDCONDITIONER
			{
				public static LocString NAME = "Thermo Aquatuner";

				public static LocString DESC = "A thermo aquatuner cools liquid and outputs the heat elsewhere.";

				public static LocString EFFECT = "Cools the <style=\"liquid\">Liquid</style> piped through it, but outputs <style=\"heat\">Heat</style> in its immediate vicinity.";
			}

			public class MEDICALCOT
			{
				public static LocString NAME = "Med-Bed";

				public static LocString DESC = "Duplicants use med-beds to recover from sickness and receive medical aid from peers.";

				public static LocString EFFECT = "Improves recovery from <style=\"disease\">Diseases</style>.\n\nAccelerates <style=\"Health\">Health</style> restoration and the healing of physical injuries.";
			}

			public class MASSAGETABLE
			{
				public static LocString NAME = "Massage Table";

				public static LocString DESC = "Massage tables quickly reduce extreme stress, at the cost of power production.";

				public static LocString EFFECT = "Rapidly reduces <style=\"stress\">Stress</style> for the Duplicant user.\n\nDuplicants will automatically seek a massage table when <style=\"stress\">Stress</style> exceeds breaktime range.";

				public static LocString ACTIVATE_TOOLTIP = "Duplicants must take a break when their stress reaches {0}";

				public static LocString DEACTIVATE_TOOLTIP = "Breaktime ends when stress is reduced to {0}";
			}

			public class CEILINGLIGHT
			{
				public static LocString NAME = "Ceiling Light";

				public static LocString DESC = "Light reduces Duplicant stress and is required to grow certain plants.";

				public static LocString EFFECT = "Provides <style=\"light\">Light</style> when <style=\"power\">Powered</style>.";
			}

			public class AIRFILTER
			{
				public static LocString NAME = "Deodorizer";

				public static LocString DESC = "Airborne germs travel easily through polluted oxygen, spreading contamination if unfiltered.";

				public static LocString EFFECT = "Nullifies <style=\"gas\">Polluted Oxygen</style>.";
			}

			public class CANVAS
			{
				public static LocString NAME = "Blank Canvas";

				public static LocString DESC = "Once built, a Duplicant can paint a blank canvas to produce a decorative painting.";

				public static LocString EFFECT = "Increases <style=\"decor\">Decor</style> and reduces <style=\"stress\">Stress</style>.\n\nMust be painted by a Duplicant.";

				public static LocString POORQUALITYNAME = "Crude Painting";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Painting";

				public static LocString EXCELLENTQUALITYNAME = "Masterpiece";
			}

			public class CO2SCRUBBER
			{
				public static LocString NAME = "Carbon Skimmer";

				public static LocString DESC = "Carbon Skimmers remove large amounts of carbon dioxide, but produce no breathable air.";

				public static LocString EFFECT = "Filters <style=\"gas\">Carbon Dioxide</style> from the air.";
			}

			public class COMPOST
			{
				public static LocString NAME = "Compost";

				public static LocString DESC = "Composts safely deal with biological waste and help fertilize domestic plants.";

				public static LocString EFFECT = string.Format("Breaks <style=\"solid\">{0}</style> down into <style=\"solid\">{1}</style>.", ELEMENTS.TOXICSAND.NAME.text, ELEMENTS.FERTILIZER.NAME.text);
			}

			public class COOKINGSTATION
			{
				public static LocString NAME = "Electric Grill";

				public static LocString DESC = "Cooking eliminates foodborne disease and produces tasty, stress-relieving meals.";

				public static LocString EFFECT = "Cooks improved <style=\"food\">Food</style> using many ingredients.\n\nDuplicants will not fabricate unless recipes are queued.";
			}

			public class DININGTABLE
			{
				public static LocString NAME = "Mess Table";

				public static LocString DESC = "Duplicants prefer to dine at a table, rather than eat off the floor.";

				public static LocString EFFECT = "Provides an eating area for one Duplicant.\n\nDuplicants will automatically eat at their assigned table when hungry.";
			}

			public class POIBUNKEREXTERIORDOOR
			{
				public static LocString NAME = "Security Door";

				public static LocString EFFECT = "A strong door with a sophisticated genetic lock.";

				public static LocString DESC = string.Empty;
			}

			public class POIDOORINTERNAL
			{
				public static LocString NAME = "Security Door";

				public static LocString EFFECT = "A strong door with a sophisticated genetic lock.";

				public static LocString DESC = string.Empty;
			}

			public class DOOR
			{
				public static LocString NAME = "Pneumatic Door";

				public static LocString DESC = "Door controls can be used to prevent Duplicants from entering restricted areas.";

				public static LocString EFFECT = "Encloses areas without blocking <style=\"liquid\">Liquid</style> or <style=\"gas\">Gas</style> flow.\n\nSets Duplicant Access Permissions for area restriction.\n\nWild creatures cannot pass through doors.";

				public static LocString PRESSURE_SUIT_REQUIRED = "<style=\"equipment\">Exosuit</style> required {0}";

				public static LocString PRESSURE_SUIT_NOT_REQUIRED = "<style=\"equipment\">Exosuit</style> not required {0}";

				public static LocString ABOVE = "above";

				public static LocString BELOW = "below";

				public static LocString LEFT = "on the left";

				public static LocString RIGHT = "on the right";

				public static class CONTROL_STATE
				{
					public class OPEN
					{
						public static LocString NAME = "Open";

						public static LocString TOOLTIP = "This door will remain open";
					}

					public class CLOSE
					{
						public static LocString NAME = "Lock";

						public static LocString TOOLTIP = "Nothing may pass through";
					}

					public class AUTO
					{
						public static LocString NAME = "Auto";

						public static LocString TOOLTIP = "Duplicants open and close this door as needed";
					}
				}
			}

			public class ELECTROLYZER
			{
				public static LocString NAME = "Electrolyzer";

				public static LocString DESC = "Electrolyzers output a high volume of oxygen and are a reliable source of breathable air.";

				public static LocString EFFECT = "Produces <style=\"oxygen\">Oxygen</style> and <style=\"gas\">Hydrogen</style> using <style=\"liquid\">Water</style>.\n\nBecomes idle when the area reaches maximum pressure capacity.";
			}

			public class POWERTRANSFORMER
			{
				public static LocString NAME = "Power Transformer";

				public static LocString DESC = "Shh! It only transforms when no one's looking.";

				public static LocString EFFECT = "Protects circuits from overloading by increasing or decreasing <style=\"power\">Power</style> flow.";
			}

			public class FLOORLAMP
			{
				public static LocString NAME = "Lamp";

				public static LocString DESC = "The emitting radius of a building's light can be viewed in the Light Overlay.";

				public static LocString EFFECT = "Provides <style=\"light\">Light</style> when <style=\"power\">Powered</style>.";
			}

			public class FLOWERVASE
			{
				public static LocString NAME = "Flower Pot";

				public static LocString DESC = "Flower pots allow decorative plants to be moved to new locations.";

				public static LocString EFFECT = "Increases <style=\"decor\">Decor</style> and reduces <style=\"stress\">Stress</style>.\n\nHouses a single <style=\"plant\">Plant</style> when sown with a <style=\"seed\">Seed</style>.";
			}

			public class FLUSHTOILET
			{
				public static LocString NAME = "Lavatory";

				public static LocString DESC = "Lavatories transmit fewer germs to Duplicants' skin and require no manual emptying.";

				public static LocString EFFECT = "Gives Duplicants a place to <style=\"hygiene\">Hygienically</style> relieve themselves.";
			}

			public class SHOWER
			{
				public static LocString NAME = "Shower";

				public static LocString DESC = "Showers remove the \"Grimy\" effect and kill all germs present on Duplicant's skin.";

				public static LocString EFFECT = "Improves Duplicant <style=\"hygiene\">Hygiene</style> and removes <style=\"disease\">Germs</style>.";
			}

			public class GASCONDUIT
			{
				public static LocString NAME = "Gas Pipe";

				public static LocString DESC = "Gas pipes are used to connect the inputs and outputs of ventilated buildings.";

				public static LocString EFFECT = "Carries <style=\"gas\">Gas</style> between <style=\"GasPiping\">Outputs</style> and <style=\"GasPiping\">Intakes</style>.\n\nCan be run through tiles.";
			}

			public class GASCONDUITBRIDGE
			{
				public static LocString NAME = "Gas Bridge";

				public static LocString DESC = "Separate pipe systems prevent mingled contents from causing building damage.";

				public static LocString EFFECT = "Runs one <style=\"GasPiping\">Gas Pipe</style> section over another without joining them.\n\nCan be run through tiles.";
			}

			public class GASFILTER
			{
				public static LocString NAME = "Gas Filter";

				public static LocString DESC = "Gas filters send the selected filter gas into a special pipe, expelling everything else through a second output.";

				public static LocString EFFECT = "Sieves one <style=\"gas\">Gas</style> from the air, sending it into a dedicated <style=\"GasPiping\">Pipe</style>.";
			}

			public class GASPERMEABLEMEMBRANE
			{
				public static LocString NAME = "Airflow Tile";

				public static LocString DESC = "Building with airflow permeable tiles promotes better gas circulation within a colony.";

				public static LocString EFFECT = "Used as floor and wall tile to build rooms.\n\nBlocks <style=\"liquid\">Liquid</style> flow without obstructing <style=\"gas\">Gas</style>.";
			}

			public class GASPUMP
			{
				public static LocString NAME = "Gas Pump";

				public static LocString DESC = "Piping a pump's intake to another building's output will send gas to that building.";

				public static LocString EFFECT = "Draws in <style=\"gas\">Gas</style> runs it through <style=\"GasPiping\">Pipes</style>.\n\nMust be immersed in gas.";
			}

			public class GASVALVE
			{
				public static LocString NAME = "Gas Valve";

				public static LocString DESC = "Valves control the amount of gas that moves through pipes, preventing waste.";

				public static LocString EFFECT = "Controls the <style=\"gas\">Gas</style> volume permitted through <style=\"GasPiping\">Pipes</style>.";
			}

			public class GASVENT
			{
				public static LocString NAME = "Gas Vent";

				public static LocString DESC = "Vents are an exit point for gases from ventilation systems.";

				public static LocString EFFECT = "Releases <style=\"gas\">Gas</style> from <style=\"GasPiping\">Gas Pipes</style>.";
			}

			public class GENERATOR
			{
				public static LocString NAME = "Coal Generator";

				public static LocString DESC = "Coal generators produce twice the energy of manual generators, but emit heat and exhaust.";

				public static LocString EFFECT = "Converts <style=\"RawMineral\">Coal</style> into electrical <style=\"power\">Power</style>.";
			}

			public class GENERICFABRICATOR
			{
				public static LocString NAME = "Generic Fabricator";

				public static LocString DESC = "This is the generic fabricator.";

				public static LocString EFFECT = "Converts <style=\"RawMineral\">Raw Minerals</style> into fabrications.";
			}

			public class GRAVE
			{
				public static LocString NAME = "Tasteful Memorial";

				public static LocString DESC = "Burying dead Duplicants reduces health hazards and stress on the colony.";

				public static LocString EFFECT = "Provides a resting place for deceased Duplicants.\n\nLiving Duplicants will automatically place an unburied corpse inside.";
			}

			public class HEADQUARTERS
			{
				public static LocString NAME = "Printing Pod";

				public static LocString DESC = "New Duplicants will come out here, but thank goodness, they'll never go back in.";

				public static LocString EFFECT = "An exceptionally advanced bioprinter of unknown origin.\n\nIt periodically produces new Duplicants.";
			}

			public class HYDROGENGENERATOR
			{
				public static LocString NAME = "Hydrogen Generator";

				public static LocString DESC = "Hydrogen generators are exceptionally efficient power sources and emit next to no waste.";

				public static LocString EFFECT = "Converts <style=\"gas\">Hydrogen Gas</style> into electrical <style=\"power\">Power</style>.";
			}

			public class METHANEGENERATOR
			{
				public static LocString NAME = "Natural Gas Generator";

				public static LocString DESC = "Natural gas generators leak polluted water and are best built above a waste reservoir.";

				public static LocString EFFECT = "Converts <style=\"gas\">Natural Gas</style> into electrical <style=\"power\">Power</style>.";
			}

			public class HYDROPONICFARM
			{
				public static LocString NAME = "Hydroponic Farm";

				public static LocString DESC = "Hydroponic farms reduce Duplicant traffic by automating liquid delivery with irrigation pipes.";

				public static LocString EFFECT = "Grows a single <style=\"plant\">Plant</style> when sown with a <style=\"seed\">Seed</style>.\n\nCan be used as floor tile and rotated before construction.\n\nMust be irrigated through liquid piping.";
			}

			public class INSULATEDGASCONDUIT
			{
				public static LocString NAME = "Insulated Gas Pipe";

				public static LocString DESC = "Pipe insulation prevents gas contents from significantly changing temperature in transit.";

				public static LocString EFFECT = "Carries <style=\"gas\">Gas</style> with minimal change in <style=\"heat\">Temperature</style>.\n\nCan be run through tiles.";
			}

			public class INSULATEDLIQUIDCONDUIT
			{
				public static LocString NAME = "Insulated Liquid Pipe";

				public static LocString DESC = "Pipe insulation prevents liquid contents from significantly changing temperature in transit.";

				public static LocString EFFECT = "Carries <style=\"liquid\">Liquid</style> with minimal change in <style=\"heat\">Temperature</style>.\n\nCan be run through tiles.";
			}

			public class INSULATEDWIRE
			{
				public static LocString NAME = "Insulated Wire";

				public static LocString DESC = "This stuff won't go melting if things get heated.";

				public static LocString EFFECT = "Connects buildings to <style=\"power\">Power</style> sources in extreme <style=\"heat\">Heat</style>.\n\nCan be run through tiles.";
			}

			public class INSULATIONTILE
			{
				public static LocString NAME = "Insulated Tile";

				public static LocString DESC = "The lowered thermal conductivity of insulated tiles slows heat from passing through them.";

				public static LocString EFFECT = "Used as floor and wall tile to build rooms.\n\nReduces <style=\"heat\">Heat</style> transfer between walls, retaining ambient heat in an area.";
			}

			public class INSULATIONWALL
			{
				public static LocString NAME = "Insulation";

				public static LocString DESC = "You'd be surprised how much research went into making warm wallpaper.";

				public static LocString EFFECT = "Reduces <style=\"heat\">Heat</style> loss through colony walls.";
			}

			public class FARMTILE
			{
				public static LocString NAME = "Farm Tile";

				public static LocString DESC = "Duplicants can deliver fertilizer and liquids to farm tiles, accelerating plant growth.";

				public static LocString EFFECT = "Grows a single <style=\"plant\">Plant</style> when sown with a <style=\"seed\">Seed</style>.\n\nCan be used as floor tile and rotated before construction.";
			}

			public class LADDER
			{
				public static LocString NAME = "Ladder";

				public static LocString DESC = "(That means they climb it.)";

				public static LocString EFFECT = "Enables vertical mobility for Duplicants.";
			}

			public class LIQUIDCONDUIT
			{
				public static LocString NAME = "Liquid Pipe";

				public static LocString DESC = "Liquid pipes are used to connect the inputs and outputs of plumbed buildings.";

				public static LocString EFFECT = "Carries <style=\"liquid\">Liquid</style> between <style=\"LiquidPiping\">Outputs</style> and <style=\"LiquidPiping\">Intakes</style>.\n\nCan be run through tiles.";
			}

			public class LIQUIDCONDUITBRIDGE
			{
				public static LocString NAME = "Liquid Bridge";

				public static LocString DESC = "Separate pipe systems prevent mingled contents from causing building damage.";

				public static LocString EFFECT = "Runs one <style=\"LiquidPiping\">Liquid Pipe</style> section over another without joining them.\n\nCan be run through tiles.";
			}

			public class LIQUIDCOOLEDFAN
			{
				public static LocString NAME = "Hydrofan";

				public static LocString DESC = "A Duplicant can work a hydrofan to temporarily cool small areas as needed.";

				public static LocString EFFECT = "Dissipates a small amount of the <style=\"heat\">Heat</style>.";
			}

			public class LIQUIDFILTER
			{
				public static LocString NAME = "Liquid Filter";

				public static LocString DESC = "Liquid filters send the selected filter gas into a special pipe, expelling everything else through a second output.";

				public static LocString EFFECT = "Sieves one <style=\"liquid\">Liquid</style> out of a mix, sending it into a dedicated <style=\"LiquidPiping\">Pipe</style>.";
			}

			public class LIQUIDPUMP
			{
				public static LocString NAME = "Liquid Pump";

				public static LocString DESC = "Piping a pump's intake to another building's output will send liquid to that building.";

				public static LocString EFFECT = "Draws in <style=\"liquid\">Liquid</style> and runs it through <style=\"LiquidPiping\">Pipes</style>.\n\nMust be submerged in liquid.";
			}

			public class LIQUIDPUMPINGSTATION
			{
				public static LocString NAME = "Liquid Bottler";

				public static LocString DESC = "This building will pump whatever liquid is present beneath it into bottles for delivery.";

				public static LocString EFFECT = "Pumps <style=\"liquid\">Liquid</style> into bottles for transport.\n\nDuplicants can only carry bottled liquids.";
			}

			public class LIQUIDVALVE
			{
				public static LocString NAME = "Liquid Valve";

				public static LocString DESC = "Valves control the amount of liquid that moves through pipes, preventing waste.";

				public static LocString EFFECT = "Controls the <style=\"liquid\">Liquid</style> volume permitted through <style=\"LiquidPiping\">Pipes</style>.";
			}

			public class LIQUIDVENT
			{
				public static LocString NAME = "Liquid Vent";

				public static LocString DESC = "Vents are an exit point for liquids from plumbing systems.";

				public static LocString EFFECT = "Releases <style=\"liquid\">Liquid</style> from <style=\"LiquidPiping\">Liquid Pipes</style>.";
			}

			public class MANUALGENERATOR
			{
				public static LocString NAME = "Manual Generator";

				public static LocString DESC = "Manual generators power buildings when connected by wire and require no fuel.";

				public static LocString EFFECT = "Converts manual labor into electrical <style=\"power\">Power</style>.";
			}

			public class MANUALPRESSUREDOOR
			{
				public static LocString NAME = "Manual Airlock";

				public static LocString DESC = "Airlocks can quarter off dangerous areas or prevent gases from seeping into the colony.";

				public static LocString EFFECT = "Blocks <style=\"liquid\">Liquid</style> and <style=\"gas\">Gas</style> flow, maintaining pressure between areas.\n\nSets Duplicant Access Permissions for area restriction.\n\nWild creatures cannot pass through doors.";
			}

			public class MEDICALBED
			{
				public static LocString NAME = "Rejuvenator";

				public static LocString DESC = "Rejuvenators allow Duplicants to regain lost health and heal wounds extremely quickly.";

				public static LocString EFFECT = "Greatly accelerates <style=\"Health\">Health</style> recovery and the healing of physical injuries.";
			}

			public class MESHTILE
			{
				public static LocString NAME = "Mesh Tile";

				public static LocString DESC = "Mesh tiles can be used to make Duplicant pathways in areas where liquid needs to flow.";

				public static LocString EFFECT = "Used as floor and wall tile to build rooms.\n\nDoes not obstruct <style=\"liquid\">Liquid</style> or <style=\"gas\">Gas</style> flow.";
			}

			public class MICROBEMUSHER
			{
				public static LocString NAME = "Microbe Musher";

				public static LocString DESC = "Microbe mushers produce low cost, low quality food for Duplicants.";

				public static LocString EFFECT = "Produces low quality <style=\"food\">Food</style> using common ingredients.\n\nDuplicants will not fabricate unless recipes are queued.";
			}

			public class MINERALDEOXIDIZER
			{
				public static LocString NAME = "Algae Deoxydizer";

				public static LocString DESC = "Algae deoxydizers output enough oxygen to keep a colony breathing, but are inefficient.";

				public static LocString EFFECT = "Converts <style=\"misc\">Algae</style> into <style=\"oxygen\">Oxygen</style>.\n\nBecomes idle when the area reaches maximum pressure capacity.";
			}

			public class ORESCRUBBER
			{
				public static LocString NAME = "Ore Scrubber";

				public static LocString DESC = "Ore scrubbers sanitize freshly mined resources before bringing them into the colony.";

				public static LocString EFFECT = "Kills a significant amount of <style=\"disease\">Germs</style> present on raw <style=\"RawMineral\">ore</style>.";
			}

			public class OUTHOUSE
			{
				public static LocString NAME = "Outhouse";

				public static LocString DESC = "Duplicants with no bathroom access will make \"messes\", causing stress and spreading germs.";

				public static LocString EFFECT = string.Format("Gives Duplicants a place to relieve themselves.\n\nRequires no <style=\"GasPiping\">Piping</style>.\n\nMust be periodically emptied of <style=\"solid\">{0}</style>.", ELEMENTS.TOXICSAND.NAME.text);
			}

			public class PLANTERBOX
			{
				public static LocString NAME = "Planter Box";

				public static LocString DESC = "Seeds that are grown domestically mature more quickly than wild plants.";

				public static LocString EFFECT = "Grows a single <style=\"plant\">Plant</style> when sown with a <style=\"seed\">Seed</style>.";
			}

			public class PRESSUREDOOR
			{
				public static LocString NAME = "Mechanized Airlock";

				public static LocString DESC = "Mechanized airlocks have the same function as other doors, but open and close more quickly.";

				public static LocString EFFECT = "Blocks <style=\"liquid\">Liquid</style> and <style=\"gas\">Gas</style> flow, maintaining pressure between areas.\n\nSets Duplicant Access Permissions for area restriction.\n\nFunctions as a Manual Airlock when no <style=\"power\">Power</style> is available.";
			}

			public class RATIONBOX
			{
				public static LocString NAME = "Ration Box";

				public static LocString DESC = "Ration boxes keep food safe from hungry creatures, but don't slow food spoilage.";

				public static LocString EFFECT = "Stores a small amount of <style=\"food\">Food</style>.\n\nFood must be delivered to boxes by Duplicants.";
			}

			public class REFRIGERATOR
			{
				public static LocString NAME = "Refrigerator";

				public static LocString DESC = "Food spoilage can be prevented by low temperatures and sterile atmospheres.";

				public static LocString EFFECT = "Stores <style=\"food\">Food</style> at an ideal <style=\"heat\">Temperature</style>.";
			}

			public class RESEARCHCENTER
			{
				public static LocString NAME = "Research Station";

				public static LocString DESC = "Research stations are necessary for unlocking all research tiers.";

				public static LocString EFFECT = "Conducts <style=\"research\">Novice Research</style> to unlock new technologies.";
			}

			public class ADVANCEDRESEARCHCENTER
			{
				public static LocString NAME = "Super Computer";

				public static LocString DESC = "Super computers unlock higher technology tiers than research stations alone.";

				public static LocString EFFECT = "Conducts <style=\"research\">Intermediate Research</style> to unlock new technologies.";
			}

			public class SCULPTURE
			{
				public static LocString NAME = "Sculpting Block";

				public static LocString DESC = "Duplicants with high art skill will produce better, more decorative sculptures.";

				public static LocString EFFECT = "Increases <style=\"decor\">Decor</style> and reduces <style=\"stress\">Stress</style>.\n\nMust be sculpted by a Duplicant.";

				public static LocString POORQUALITYNAME = "\"Abstract\" Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Sculpture";
			}

			public class SEEDSPLICER
			{
				public static LocString NAME = "Seed Splicer";

				public static LocString DESC = "Just think of it as genetic \"enhancement\".";

				public static LocString EFFECT = "Splices <style=\"seed\">Seeds</style> to produce advanced <style=\"plant\">Plant</style> strains.\n\nDuplicants will not fabricate unless recipes are queued.";
			}

			public class SPACEHEATER
			{
				public static LocString NAME = "Space Heater";

				public static LocString DESC = "A space heater will radiate heat for as long as it's powered.";

				public static LocString EFFECT = "Radiates a moderate amount of <style=\"heat\">Heat</style>.";
			}

			public class STORAGELOCKER
			{
				public static LocString NAME = "Storage Compactor";

				public static LocString DESC = "Resources left on the floor become \"debris\" and lower decor when not put away.";

				public static LocString EFFECT = "Stores the resources of your choosing.";
			}

			public class CLOTHINGFABRICATOR
			{
				public static LocString NAME = "Textile Loom";

				public static LocString DESC = "A textile loom can be used to spin Reed Fiber into wearable Duplicant clothing.";

				public static LocString EFFECT = "Tailors Duplicant <style=\"equipment\">Clothing</style> items.\n\nDuplicants will not fabricate unless recipes are queued.";
			}

			public class LIQUIDHEATER
			{
				public static LocString NAME = "Liquid Tepidizer";

				public static LocString DESC = "Liquid tepidizers kill waterborne germs and heat liquid to a perfect showering temperature.";

				public static LocString EFFECT = "Warms large bodies of <style=\"liquid\">Liquid</style>.\n\nMust be fully submerged.";
			}

			public class SWITCH
			{
				public static LocString NAME = "Switch";

				public static LocString DESC = "Switches do not affect buildings preceding them on a circuit, but control all buildings after.";

				public static LocString EFFECT = "Turns <style=\"power\">Power</style> on or off.\n\nDoes not affect circuitry preceding the switch.";

				public static LocString TURN_ON = "Turn On";

				public static LocString TURN_OFF = "Turn Off";
			}

			public class TEMPERATURECONTROLLEDSWITCH
			{
				public static LocString NAME = "Thermo Switch";

				public static LocString DESC = "Automated switches can be used to manage circuits in areas where Duplicants cannot enter.";

				public static LocString EFFECT = "Automatically turns <style=\"power\">Power</style> on or off using ambient <style=\"heat\">Temperature</style>.\n\nDoes not affect circuitry preceding the switch.";
			}

			public class PRESSURESWITCHLIQUID
			{
				public static LocString NAME = "Hydro Switch";

				public static LocString DESC = "A hydro switch shuts off power when the liquid pressure surrounding it surpasses the set threshold.";

				public static LocString EFFECT = "Automatically turns <style=\"power\">Power</style> on or off using ambient <style=\"liquid\">Liquid Pressure</style>.\n\nDoes not affect circuitry preceding the switch.\n\nMust be submerged in <style=\"liquid\">Liquid</style>.";
			}

			public class PRESSURESWITCHGAS
			{
				public static LocString NAME = "Atmo Switch";

				public static LocString DESC = "An atmo switch shuts off power when the air pressure surrounding it surpasses the set threshold.";

				public static LocString EFFECT = "Automatically turns <style=\"power\">Power</style> on or off using ambient <style=\"gas\">Gas Pressure</style>.\n\nDoes not affect circuitry preceding the switch.";
			}

			public class TILE
			{
				public static LocString NAME = "Tile";

				public static LocString DESC = "Tiles can be used to build rooms, floors, or bridges to new areas.";

				public static LocString EFFECT = "Used as floor and wall tile to build rooms.";
			}

			public class WATERPURIFIER
			{
				public static LocString NAME = "Water Purifier";

				public static LocString DESC = "Clean water is a necessary resource for most life sustaining processes.";

				public static LocString EFFECT = "Uses <style=\"misc\">Sand</style> to filter <style=\"liquid\">Polluted Water</style>.\n\nTransfers a portion of disease to the Polluted Dirt.";
			}

			public class DISTILLATIONCOLUMN
			{
				public static LocString NAME = "Distillation Column";

				public static LocString DESC = "Gets hot and steamy.";

				public static LocString EFFECT = string.Format("Separates any <style=\"liquid\">{0}</style> piped through it into <style=\"gas\">Steam</style> and <style=\"solid\">{1}</style>.", ELEMENTS.DIRTYWATER.NAME.text, ELEMENTS.TOXICSAND.NAME.text);
			}

			public class WIRE
			{
				public static LocString NAME = "Wire";

				public static LocString DESC = "Electrical wire is used to connect generators, batteries, and buildings.";

				public static LocString EFFECT = "Connects buildings to <style=\"power\">Power</style> sources.\n\nCan be run through tiles.";
			}

			public class HIGHWATTAGEWIRE
			{
				public static LocString NAME = "Heavi-Watt Wire";

				public static LocString DESC = "Higher wattage wire is used to avoid power overloads, particularly for strong generators.";

				public static LocString EFFECT = "Carries more <style=\"power\">Wattage</style> than regular Wire without overloading.\n\nCannot be run through tiles.";
			}

			public class WIREBRIDGE
			{
				public static LocString NAME = "Wire Bridge";

				public static LocString DESC = "Splitting generators onto separate systems can prevent power overloads and wasted electricity.";

				public static LocString EFFECT = "Runs one wire section over another without joining them.\n\nCan be run through tiles.";
			}

			public class HANDSANITIZER
			{
				public static LocString NAME = "Hand Sanitizer";

				public static LocString DESC = "Hand sanitizers kill germs more effectively than wash basins.";

				public static LocString EFFECT = "Removes most <style=\"disease\">Germs</style> from Duplicants.\n\nDuplicants use Hand Sanitizers when passing by in the selected direction.";
			}

			public class WASHBASIN
			{
				public static LocString NAME = "Wash Basin";

				public static LocString DESC = "Germ spread can be reduced by building wash basins where Duplicants are likely to get dirty.";

				public static LocString EFFECT = "Removes some <style=\"disease\">Germs</style> from Duplicants.\n\nDuplicants use Wash Basins when passing by in the selected direction only if they have germs on them.";
			}

			public class TILEPOI
			{
				public static LocString NAME = "Tile";

				public static LocString DESC = string.Empty;

				public static LocString EFFECT = "Used as floor and wall tile to build rooms.";
			}
		}

		public static class DAMAGESOURCES
		{
			public static LocString NOTIFICATION_TOOLTIP = "A {0} sustained damage from {1}";

			public static LocString CONDUIT_CONTENTS_FROZE = "frozen pipe contents";

			public static LocString CONDUIT_CONTENTS_BOILED = "boiling pipe contents";

			public static LocString BUILDING_OVERHEATED = "overheating";

			public static LocString BAD_INPUT_ELEMENT = "receiving an incorrect substance";

			public static LocString MINION_DESTRUCTION = "an angry Duplicant. Rude!";

			public static LocString LIQUID_PRESSURE = "neighboring liquid pressure";

			public static LocString CIRCUIT_OVERLOADED = "an overloaded circuit";
		}

		public static class AUTODISINFECTABLE
		{
			public static class ENABLE_AUTODISINFECT
			{
				public static LocString NAME = "Enable Auto-Disinfect";

				public static LocString TOOLTIP = "Dupes will automatically disinfect this building";
			}

			public static class DISABLE_AUTODISINFECT
			{
				public static LocString NAME = "Disable Auto-Disinfect";

				public static LocString TOOLTIP = "Dupes will no longer automatically disinfect this building";
			}

			public static class NO_DISEASE
			{
				public static LocString TOOLTIP = "This building is already clean";
			}
		}

		public static class DISINFECTABLE
		{
			public static class ENABLE_DISINFECT
			{
				public static LocString NAME = "Disinfect";

				public static LocString TOOLTIP = "Mark this building for disinfection";
			}

			public static class DISABLE_DISINFECT
			{
				public static LocString NAME = "Cancel Disinfect";

				public static LocString TOOLTIP = "Cancel this disinfect order";
			}

			public static class NO_DISEASE
			{
				public static LocString TOOLTIP = "This building is already clean";
			}
		}

		public static class REPAIRABLE
		{
			public static class ENABLE_AUTOREPAIR
			{
				public static LocString NAME = "Enable Autorepair";

				public static LocString TOOLTIP = "Automatically repair this building when damaged";
			}

			public static class DISABLE_AUTOREPAIR
			{
				public static LocString NAME = "Disable Autorepair";

				public static LocString TOOLTIP = "Only repair this building when ordered";
			}
		}
	}
}
