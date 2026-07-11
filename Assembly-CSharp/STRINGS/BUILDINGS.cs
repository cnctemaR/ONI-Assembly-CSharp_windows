using System;

namespace STRINGS
{
	public class BUILDINGS
	{
		public class PREFABS
		{
			public class AIRCONDITIONER
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Regulator", "AIRCONDITIONER");

				public static LocString DESC = "A thermo regulator doesn't remove heat, but relocates it to a new area.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Cools the ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" piped through it, but outputs ",
					UI.FormatAsLink("Heat", "HEAT"),
					" in its immediate vicinity."
				});
			}

			public class ALGAEDISTILLERY
			{
				public static LocString NAME = UI.FormatAsLink("Algae Distiller", "ALGAEDISTILLERY");

				public static LocString DESC = "Algae distillers convert disease-causing slime into algae for oxygen production.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Refines",
					ELEMENTS.SLIMEMOLD.NAME,
					"into ",
					ELEMENTS.ALGAE.NAME,
					"."
				});
			}

			public class FERTILIZERMAKER
			{
				public static LocString NAME = UI.FormatAsLink("Fertilizer Synthesizer", "FERTILIZERMAKER");

				public static LocString DESC = "Fertilizer Synthesizers convert polluted water into fertilizer for domestic plants.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Uses ",
					ELEMENTS.DIRTYWATER.NAME,
					" to produce ",
					ELEMENTS.FERTILIZER.NAME,
					"."
				});
			}

			public class ALGAEHABITAT
			{
				public static LocString NAME = UI.FormatAsLink("Algae Terrarium", "ALGAEHABITAT");

				public static LocString DESC = "Algae colony, Duplicant colony... we're more alike than we are different.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Consumes ",
					ELEMENTS.ALGAE.NAME,
					" to produce ",
					ELEMENTS.OXYGEN.NAME,
					" and remove some ",
					ELEMENTS.CARBONDIOXIDE.NAME,
					".\n\nGains a 10% efficiency boost in direct ",
					UI.FormatAsLink("Light", "LIGHT"),
					"."
				});

				public static LocString SIDESCREEN_TITLE = "Empty " + ELEMENTS.DIRTYWATER.NAME + " Threshold";
			}

			public class BATTERY
			{
				public static LocString NAME = UI.FormatAsLink("Tiny Battery", "BATTERY");

				public static LocString DESC = "Batteries allow extra power from generators to be stored for later, rather than lost.";

				public static LocString EFFECT = "Stores a bit of runoff " + UI.FormatAsLink("Power", "POWER") + " from generators, but loses charge over time.";

				public static LocString CHARGE_LOSS = "{Battery} charge loss";
			}

			public class AIRBORNECREATURELURE
			{
				public static LocString NAME = UI.FormatAsLink("Critter Lure", "AIRBORNECREATURELURE");

				public static LocString DESC = "Lures can relocate Pufts or Shine Bugs to specific locations in your base.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Attracts one type of airborne critter.\n\nMust be baited with ",
					ELEMENTS.SLIMEMOLD.NAME,
					" or ",
					ELEMENTS.PHOSPHORITE.NAME,
					"."
				});
			}

			public class BATTERYMEDIUM
			{
				public static LocString NAME = UI.FormatAsLink("Battery", "BATTERYMEDIUM");

				public static LocString DESC = "Larger batteries hold more power and keep systems running longer before recharging.";

				public static LocString EFFECT = "Stores most runoff " + UI.FormatAsLink("Power", "POWER") + " from generators, but loses charge over time.";
			}

			public class BATTERYSMART
			{
				public static LocString NAME = UI.FormatAsLink("Smart Battery", "BATTERYSMART");

				public static LocString DESC = "Smart batteries can automatically turn systems off when there is no charge to sustain them.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Stores most runoff ",
					UI.FormatAsLink("Power", "POWER"),
					" from generators, but loses charge over time.\n\nLogic input becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" when charged above the set threshold."
				});

				public static LocString LOGIC_PORT_DESC = string.Empty + UI.FormatAsLink("Active", "LOGIC") + " if charge exceeds threshold";

				public static LocString ACTIVATE_TOOLTIP = "Logic input will become " + UI.FormatAsLink("Active", "LOGIC") + " when battery is less than {0}% charged";

				public static LocString DEACTIVATE_TOOLTIP = "Logic input will go on " + UI.FormatAsLink("Standby", "LOGIC") + " when battery is more than {0}% charged";

				public static LocString SIDESCREEN_TITLE = "Logic Activation Parameters";

				public static LocString SIDESCREEN_ACTIVATE = "Active:";

				public static LocString SIDESCREEN_DEACTIVATE = "Standby:";
			}

			public class BED
			{
				public static LocString NAME = UI.FormatAsLink("Cot", "BED");

				public static LocString DESC = "Duplicants without a bed will develop sore backs from sleeping on the floor.";

				public static LocString EFFECT = "Gives one Duplicant a place to sleep.\n\nDuplicants will automatically return to their cots to sleep at night.";
			}

			public class BOTTLEEMPTIER
			{
				public static LocString NAME = UI.FormatAsLink("Bottle Emptier", "BOTTLEEMPTIER");

				public static LocString DESC = "A bottle emptier's Element Filter can be used to designate areas for specific liquid storage.";

				public static LocString EFFECT = "Empties bottled " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " back into the world.";
			}

			public class CHECKPOINT
			{
				public static LocString NAME = UI.FormatAsLink("Duplicant Checkpoint", "CHECKPOINT");

				public static LocString DESC = "Checkpoints can be connected to automated sensors to determine when it is safe to enter.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Allows Duplicants to pass when ",
					UI.FormatAsLink("Active", "LOGIC"),
					".\n\nPrevents Duplicants from passing when on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					"."
				});

				public static LocString LOGIC_PORT_DESC = "Duplicant Stop/Go";
			}

			public class FIREPOLE
			{
				public static LocString NAME = UI.FormatAsLink("Fire Pole", "FIREPOLE");

				public static LocString DESC = "Fire poles provide quick downward travel, while ladders provide better upward movement.";

				public static LocString EFFECT = "Allows rapid Duplicant descent.\n\nSignificantly slows upward climbing.";
			}

			public class FLOORSWITCH
			{
				public static LocString NAME = UI.FormatAsLink("Weight Plate", "FLOORSWITCH");

				public static LocString DESC = "Weight plates can be used to turn on amenities only when Duplicants pass by.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" when an object or Duplicant is placed atop of it.\n\nCannot be triggered by ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" or ",
					UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID"),
					"."
				});

				public static LocString LOGIC_PORT_DESC = UI.FormatAsLink("Active", "LOGIC") + "/" + UI.FormatAsLink("Inactive", "LOGIC");
			}

			public class KILN
			{
				public static LocString NAME = UI.FormatAsLink("Kiln", "KILN");

				public static LocString DESC = "Converts " + ELEMENTS.CLAY.NAME + " to " + ELEMENTS.CERAMIC.NAME;

				public static LocString EFFECT = "Converts " + ELEMENTS.CLAY.NAME + " to " + ELEMENTS.CERAMIC.NAME;
			}

			public class LIQUIDCONDITIONER
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Aquatuner", "LIQUIDCONDITIONER");

				public static LocString DESC = "A thermo aquatuner cools liquid and outputs the heat elsewhere.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Cools the ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" piped through it, but outputs ",
					UI.FormatAsLink("Heat", "HEAT"),
					" in its immediate vicinity."
				});
			}

			public class LUXURYBED
			{
				public static LocString NAME = UI.FormatAsLink("Comfy Bed", "LUXURYBED");

				public static LocString DESC = "Duplicants prefer comfy beds to cots and will gain more stamina back from sleeping in them.";

				public static LocString EFFECT = "Provides a sleeping area for one Duplicant and restores additional " + UI.FormatAsLink("Stamina", "STAMINA") + ".\n\nDuplicants will automatically sleep in their assigned beds at night.";
			}

			public class MEDICALCOT
			{
				public static LocString NAME = UI.FormatAsLink("Med-Bed", "MEDICALCOT");

				public static LocString DESC = "Duplicants use med-beds to recover from sickness and receive medical aid from peers.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Improves recovery from ",
					UI.FormatAsLink("Diseases", "DISEASE"),
					".\n\nAccelerates ",
					UI.FormatAsLink("Health", "HEALTH"),
					" restoration and the healing of physical injuries."
				});
			}

			public class MASSAGETABLE
			{
				public static LocString NAME = UI.FormatAsLink("Massage Table", "MASSAGETABLE");

				public static LocString DESC = "Massage tables quickly reduce extreme stress, at the cost of power production.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Rapidly reduces ",
					UI.FormatAsLink("Stress", "STRESS"),
					" for the Duplicant user.\n\nDuplicants will automatically seek a massage table when ",
					UI.FormatAsLink("Stress", "STRESS"),
					" exceeds breaktime range."
				});

				public static LocString ACTIVATE_TOOLTIP = "Duplicants must take a break when their stress reaches {0}";

				public static LocString DEACTIVATE_TOOLTIP = "Breaktime ends when stress is reduced to {0}";
			}

			public class CEILINGLIGHT
			{
				public static LocString NAME = UI.FormatAsLink("Ceiling Light", "CEILINGLIGHT");

				public static LocString DESC = "Light reduces Duplicant stress and is required to grow certain plants.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Provides ",
					UI.FormatAsLink("Light", "LIGHT"),
					" when ",
					UI.FormatAsLink("Power", "POWER"),
					"."
				});
			}

			public class AIRFILTER
			{
				public static LocString NAME = UI.FormatAsLink("Deodorizer", "AIRFILTER");

				public static LocString DESC = "Oh! Citrus scented!";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Uses ",
					ELEMENTS.SAND.NAME,
					" to filter ",
					ELEMENTS.CONTAMINATEDOXYGEN.NAME,
					" from the air, reducing ",
					UI.FormatAsLink("Disease", "DISEASE"),
					" spread."
				});
			}

			public class CANVAS
			{
				public static LocString NAME = UI.FormatAsLink("Blank Canvas", "CANVAS");

				public static LocString DESC = "Once built, a Duplicant can paint a blank canvas to produce a decorative painting.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Increases ",
					UI.FormatAsLink("Decor", "DECOR"),
					" and reduces ",
					UI.FormatAsLink("Stress", "STRESS"),
					".\n\nMust be painted by a Duplicant."
				});

				public static LocString POORQUALITYNAME = "Crude Painting";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Painting";

				public static LocString EXCELLENTQUALITYNAME = "Masterpiece";
			}

			public class CO2SCRUBBER
			{
				public static LocString NAME = UI.FormatAsLink("Carbon Skimmer", "CO2SCRUBBER");

				public static LocString DESC = "Carbon skimmers remove large amounts of carbon dioxide, but produce no breathable air.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Uses ",
					ELEMENTS.WATER.NAME,
					" to filter ",
					ELEMENTS.CARBONDIOXIDE.NAME,
					" from the air."
				});
			}

			public class COMPOST
			{
				public static LocString NAME = UI.FormatAsLink("Compost", "COMPOST");

				public static LocString DESC = "Composts safely deal with biological waste and help fertilize domestic plants.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Reduces ",
					ELEMENTS.TOXICSAND.NAME,
					" and other compostables down into ",
					ELEMENTS.DIRT.NAME,
					"."
				});
			}

			public class COOKINGSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Electric Grill", "COOKINGSTATION");

				public static LocString DESC = "Proper cooking eliminates foodborne disease and produces tasty, stress-relieving meals.";

				public static LocString EFFECT = "Cooks a wide variety of improved " + UI.FormatAsLink("Foods", "FOOD") + ".\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class DININGTABLE
			{
				public static LocString NAME = UI.FormatAsLink("Mess Table", "DININGTABLE");

				public static LocString DESC = "Duplicants prefer to dine at a table, rather than eat off the floor.";

				public static LocString EFFECT = "Gives one Duplicant a place to eat.\n\nDuplicants will automatically eat at their assigned table when hungry.";
			}

			public class POIBUNKEREXTERIORDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Security Door", "POIBUNKEREXTERIORDOOR");

				public static LocString EFFECT = "A strong door with a sophisticated genetic lock.";

				public static LocString DESC = string.Empty;
			}

			public class POIDOORINTERNAL
			{
				public static LocString NAME = UI.FormatAsLink("Security Door", "POIDOORINTERNAL");

				public static LocString EFFECT = "A strong door with a sophisticated genetic lock.";

				public static LocString DESC = string.Empty;
			}

			public class POIFACILITYDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Facility Door", "FACILITYDOOR");

				public static LocString EFFECT = "A fancy door to a big facility.";

				public static LocString DESC = string.Empty;
			}

			public class DOOR
			{
				public static LocString NAME = UI.FormatAsLink("Pneumatic Door", "DOOR");

				public static LocString DESC = "Door controls can be used to prevent Duplicants from entering restricted areas.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Encloses areas without blocking ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" or ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" flow.\n\nSets Duplicant Access Permissions for area restriction.\n\nWild critters cannot pass through doors."
				});

				public static LocString PRESSURE_SUIT_REQUIRED = EQUIPMENT.PREFABS.ATMO_SUIT.NAME + " required {0}";

				public static LocString PRESSURE_SUIT_NOT_REQUIRED = EQUIPMENT.PREFABS.ATMO_SUIT.NAME + " not required {0}";

				public static LocString ABOVE = "above";

				public static LocString BELOW = "below";

				public static LocString LEFT = "on left";

				public static LocString RIGHT = "on right";

				public static LocString LOGIC_PORT_DESC = "Open/Close";

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
				public static LocString NAME = UI.FormatAsLink("Electrolyzer", "ELECTROLYZER");

				public static LocString DESC = "Water goes in one end, life sustaining oxygen comes out the other.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Converts ",
					ELEMENTS.WATER.NAME,
					" into ",
					ELEMENTS.OXYGEN.NAME,
					" and ",
					ELEMENTS.HYDROGEN.NAME,
					".\n\nBecomes idle when the area reaches maximum pressure capacity."
				});
			}

			public class POWERTRANSFORMER
			{
				public static LocString NAME = UI.FormatAsLink("Power Transformer", "POWERTRANSFORMER");

				public static LocString DESC = "Shh! It only transforms when no one's looking.";

				public static LocString EFFECT = "Protects circuits from overloading by increasing or decreasing " + UI.FormatAsLink("Power", "POWER") + " flow.";
			}

			public class FLOORLAMP
			{
				public static LocString NAME = UI.FormatAsLink("Lamp", "FLOORLAMP");

				public static LocString DESC = "The emitting radius of a building's light can be viewed in the Light Overlay.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Provides ",
					UI.FormatAsLink("Light", "LIGHT"),
					" when ",
					UI.FormatAsLink("Power", "POWER"),
					"."
				});
			}

			public class FLOWERVASE
			{
				public static LocString NAME = UI.FormatAsLink("Flower Pot", "FLOWERVASE");

				public static LocString DESC = "Flower pots allow decorative plants to be moved to new locations.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Increases ",
					UI.FormatAsLink("Decor", "DECOR"),
					" and reduces ",
					UI.FormatAsLink("Stress", "STRESS"),
					".\n\nHouses a single ",
					UI.FormatAsLink("Plant", "PLANTS"),
					" when sown with a ",
					UI.FormatAsLink("Seed", "PLANTS"),
					"."
				});
			}

			public class FLUSHTOILET
			{
				public static LocString NAME = UI.FormatAsLink("Lavatory", "FLUSHTOILET");

				public static LocString DESC = "Lavatories transmit fewer germs to Duplicants' skin and require no manual emptying.";

				public static LocString EFFECT = "Gives Duplicants a place to " + UI.FormatAsLink("Hygienically", "HYGIENE") + " relieve themselves.";
			}

			public class FACILITYBACKWALLWINDOW
			{
				public static LocString NAME = UI.FormatAsLink("Window", string.Empty);

				public static LocString DESC = "Window";

				public static LocString EFFECT = string.Empty;
			}

			public class SHOWER
			{
				public static LocString NAME = UI.FormatAsLink("Shower", "SHOWER");

				public static LocString DESC = "Showers remove the \"Grimy\" effect and kill all germs present on Duplicant's skin.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Improves Duplicant ",
					UI.FormatAsLink("Hygiene", "HYGIENE"),
					" and removes ",
					UI.FormatAsLink("Germs", "DISEASE"),
					"."
				});
			}

			public class CONDUIT
			{
				public class STATUS_ITEM
				{
					public static LocString NAME = "Marked for Emptying";

					public static LocString TOOLTIP = "Awaiting a Plumber to clear this pipe";
				}
			}

			public class GASCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pipe", "GASCONDUIT");

				public static LocString DESC = "Gas pipes are used to connect the inputs and outputs of ventilated buildings.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Carries ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" between ",
					UI.FormatAsLink("Outputs", "GASPIPING"),
					" and ",
					UI.FormatAsLink("Intakes", "GASPIPING"),
					".\n\nCan be run through tiles."
				});
			}

			public class GASCONDUITBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Gas Bridge", "GASCONDUITBRIDGE");

				public static LocString DESC = "Separate pipe systems prevent mingled contents from causing building damage.";

				public static LocString EFFECT = "Runs one " + UI.FormatAsLink("Gas Pipe", "GASPIPING") + " section over another without joining them.\n\nCan be run through tiles.";
			}

			public class GASCONDUITPREFERENTIALFLOW
			{
				public static LocString NAME = UI.FormatAsLink("Priority Gas Flow", "GASCONDUITPREFERENTIALFLOW");

				public static LocString DESC = string.Empty;

				public static LocString EFFECT = "Has a secondary input which is only drawn from when the primary input is empty.";
			}

			public class LIQUIDCONDUITPREFERENTIALFLOW
			{
				public static LocString NAME = UI.FormatAsLink("Priority Liquid Flow", "LIQUIDCONDUITPREFERENTIALFLOW");

				public static LocString DESC = string.Empty;

				public static LocString EFFECT = "Has a secondary input which is only drawn from when the primary input is empty.";
			}

			public class GASCONDUITOVERFLOW
			{
				public static LocString NAME = UI.FormatAsLink("Gas Overflow Valve", "GASCONDUITOVERFLOW");

				public static LocString DESC = string.Empty;

				public static LocString EFFECT = "Has a secondary output which is only used when the primary output is blocked.";
			}

			public class LIQUIDCONDUITOVERFLOW
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Overflow Valve", "LIQUIDCONDUITOVERFLOW");

				public static LocString DESC = string.Empty;

				public static LocString EFFECT = "Has a secondary output which is only used when the primary output is blocked.";
			}

			public class GASFILTER
			{
				public static LocString NAME = UI.FormatAsLink("Gas Filter", "GASFILTER");

				public static LocString DESC = "Gas filters send the selected filter gas into a special pipe, expelling everything else through a second output.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Sieves one ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" from the air, sending it into a dedicated ",
					UI.FormatAsLink("Pipe", "GASPIPING"),
					"."
				});

				public static LocString STATUS_ITEM = "Filters: {0}";

				public static LocString ELEMENT_NOT_SPECIFIED = "Not Specified";
			}

			public class GASPERMEABLEMEMBRANE
			{
				public static LocString NAME = UI.FormatAsLink("Airflow Tile", "GASPERMEABLEMEMBRANE");

				public static LocString DESC = "Building with airflow permeable tiles promotes better gas circulation within a colony.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Used as floor and wall tile to build rooms.\n\nBlocks ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" flow without obstructing ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					"."
				});
			}

			public class GASPUMP
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pump", "GASPUMP");

				public static LocString DESC = "Piping a pump's intake to another building's output will send gas to that building.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Draws in ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" runs it through ",
					UI.FormatAsLink("Pipes", "GASPIPING"),
					".\n\nMust be immersed in gas."
				});
			}

			public class GASMINIPUMP
			{
				public static LocString NAME = UI.FormatAsLink("Mini Gas Pump", "GASPUMP");

				public static LocString DESC = "Mini pumps are useful for moving small quantities of gas with minimum power draw.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Draws in a small amount of ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" runs it through ",
					UI.FormatAsLink("Pipes", "GASPIPING"),
					".\n\nMust be immersed in gas."
				});
			}

			public class GASVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Gas Valve", "GASVALVE");

				public static LocString DESC = "Valves control the amount of gas that moves through pipes, preventing waste.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Controls the ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" volume permitted through ",
					UI.FormatAsLink("Pipes", "GASPIPING"),
					"."
				});
			}

			public class GASLOGICVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Gas Shutoff", "GASLOGICVALVE");

				public static LocString DESC = "Automated piping saves time and resources by removing the need for Duplicant management.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Automatically turns ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" flow on or off using ",
					UI.FormatAsLink("Automation", "LOGIC"),
					" technology."
				});
			}

			public class GASVENT
			{
				public static LocString NAME = UI.FormatAsLink("Gas Vent", "GASVENT");

				public static LocString DESC = "Vents are an exit point for gases from ventilation systems.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Releases ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" from ",
					UI.FormatAsLink("Gas Pipes", "GASPIPING"),
					"."
				});
			}

			public class GASVENTHIGHPRESSURE
			{
				public static LocString NAME = UI.FormatAsLink("High Pressure Gas Vent", "GASVENTHIGHPRESSURE");

				public static LocString DESC = "High pressure Vents can exhaust gas into more highly pressurized environments.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Releases ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" from ",
					UI.FormatAsLink("Gas Pipes", "GASPIPING"),
					" into high pressure locations."
				});
			}

			public class GENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Coal Generator", "GENERATOR");

				public static LocString DESC = "Coal generators produce more energy than manual generators, but emit heat and exhaust.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Converts ",
					ELEMENTS.CARBON.NAME,
					" into electrical ",
					UI.FormatAsLink("Power", "POWER"),
					".\n\nProduces ",
					ELEMENTS.CARBONDIOXIDE.NAME,
					"."
				});

				public static LocString OVERPRODUCTION = "{Generator} overproduction";
			}

			public class GENERICFABRICATOR
			{
				public static LocString NAME = UI.FormatAsLink("Omniprinter", "GENERICFABRICATOR");

				public static LocString DESC = "Omniprinters are incapable of printing organic matter.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Raw Mineral", "RAWMINERAL") + " into unique materials and objects.";
			}

			public class GRAVE
			{
				public static LocString NAME = UI.FormatAsLink("Tasteful Memorial", "GRAVE");

				public static LocString DESC = "Burying dead Duplicants reduces health hazards and stress on the colony.";

				public static LocString EFFECT = "Provides a resting place for deceased Duplicants.\n\nLiving Duplicants will automatically place an unburied corpse inside.";
			}

			public class HEADQUARTERS
			{
				public static LocString NAME = UI.FormatAsLink("Printing Pod", "HEADQUARTERS");

				public static LocString DESC = "New Duplicants will come out here, but thank goodness, they'll never go back in.";

				public static LocString EFFECT = "An exceptionally advanced bioprinter of unknown origin.\n\nIt periodically produces new Duplicants.";
			}

			public class HYDROGENGENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Hydrogen Generator", "HYDROGENGENERATOR");

				public static LocString DESC = "Hydrogen generators are exceptionally efficient power sources and emit next to no waste.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Converts ",
					ELEMENTS.HYDROGEN.NAME,
					" into electrical ",
					UI.FormatAsLink("Power", "POWER"),
					"."
				});
			}

			public class METHANEGENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Natural Gas Generator", "METHANEGENERATOR");

				public static LocString DESC = "Natural gas generators leak polluted water and are best built above a waste reservoir.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Converts ",
					ELEMENTS.METHANE.NAME,
					" into electrical ",
					UI.FormatAsLink("Power", "POWER"),
					".\n\nProduces ",
					ELEMENTS.CARBONDIOXIDE.NAME,
					" and ",
					ELEMENTS.DIRTYWATER.NAME,
					"."
				});
			}

			public class PETROLEUMGENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Petroleum Generator", "PETROLEUMGENERATOR");

				public static LocString DESC = "Petroleum generators have a high energy output but produce a great deal of waste.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Converts ",
					ELEMENTS.PETROLEUM.NAME,
					" into electrical ",
					UI.FormatAsLink("Power", "POWER"),
					".\n\nProduces ",
					ELEMENTS.CARBONDIOXIDE.NAME,
					" and ",
					ELEMENTS.DIRTYWATER.NAME,
					"."
				});
			}

			public class HYDROPONICFARM
			{
				public static LocString NAME = UI.FormatAsLink("Hydroponic Farm", "HYDROPONICFARM");

				public static LocString DESC = "Hydroponic farms reduce Duplicant traffic by automating liquid delivery with irrigation pipes.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Grows one ",
					UI.FormatAsLink("Plant", "PLANTS"),
					" from a ",
					UI.FormatAsLink("Seed", "PLANTS"),
					".\n\nCan be used as floor tile and rotated before construction.\n\nMust be irrigated through liquid piping."
				});
			}

			public class INSULATEDGASCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Insulated Gas Pipe", "INSULATEDGASCONDUIT");

				public static LocString DESC = "Pipe insulation prevents gas contents from significantly changing temperature in transit.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Carries ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" with minimal change in ",
					UI.FormatAsLink("Temperature", "HEAT"),
					".\n\nCan be run through tiles."
				});
			}

			public class GASCONDUITRADIANT
			{
				public static LocString NAME = UI.FormatAsLink("Radiant Gas Pipe", "GASCONDUITRADIANT");

				public static LocString DESC = "Radiant pipes significantly improve the rate of temperature exchange with the surrounding environment.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Carries ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" and encourages significant change in ",
					UI.FormatAsLink("Temperature", "HEAT"),
					".\n\nCan be run through tiles."
				});
			}

			public class INSULATEDLIQUIDCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Insulated Liquid Pipe", "INSULATEDLIQUIDCONDUIT");

				public static LocString DESC = "Pipe insulation prevents liquid contents from significantly changing temperature in transit.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Carries ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" with minimal change in ",
					UI.FormatAsLink("Temperature", "HEAT"),
					".\n\nCan be run through tiles."
				});
			}

			public class LIQUIDCONDUITRADIANT
			{
				public static LocString NAME = UI.FormatAsLink("Radiant Liquid Pipe", "LIQUIDCONDUITRADIANT");

				public static LocString DESC = "Radiant pipes significantly improve the rate of temperature exchange with the surrounding environment.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Carries ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" and encourages significant change in ",
					UI.FormatAsLink("Temperature", "HEAT"),
					".\n\nCan be run through tiles."
				});
			}

			public class INSULATEDWIRE
			{
				public static LocString NAME = UI.FormatAsLink("Insulated Wire", "INSULATEDWIRE");

				public static LocString DESC = "This stuff won't go melting if things get heated.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Connects buildings to ",
					UI.FormatAsLink("Power", "POWER"),
					" sources in extreme ",
					UI.FormatAsLink("Heat", "HEAT"),
					".\n\nCan be run through tiles."
				});
			}

			public class INSULATIONTILE
			{
				public static LocString NAME = UI.FormatAsLink("Insulated Tile", "INSULATIONTILE");

				public static LocString DESC = "The lowered thermal conductivity of insulated tiles slows heat from passing through them.";

				public static LocString EFFECT = "Used as floor and wall tile to build rooms.\n\nReduces " + UI.FormatAsLink("Heat", "HEAT") + " transfer between walls, retaining ambient heat in an area.";
			}

			public class EXTERIORWALL
			{
				public static LocString NAME = UI.FormatAsLink("Wallpaper", "EXTERIORWALL");

				public static LocString DESC = "You'd be surprised how much research went into making this wall.";

				public static LocString EFFECT = "Back wall included.";
			}

			public class FARMTILE
			{
				public static LocString NAME = UI.FormatAsLink("Farm Tile", "FARMTILE");

				public static LocString DESC = "Duplicants can deliver fertilizer and liquids to farm tiles, accelerating plant growth.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Grows one ",
					UI.FormatAsLink("Plant", "PLANTS"),
					" from a ",
					UI.FormatAsLink("Seed", "PLANTS"),
					".\n\nCan be used as floor tile and rotated before construction."
				});
			}

			public class LADDER
			{
				public static LocString NAME = UI.FormatAsLink("Ladder", "LADDER");

				public static LocString DESC = "(That means they climb it.)";

				public static LocString EFFECT = "Enables vertical mobility for Duplicants.";
			}

			public class LADDERFAST
			{
				public static LocString NAME = UI.FormatAsLink("Plastic Ladder", "LADDERFAST");

				public static LocString DESC = "Plastic ladders are mildly antiseptic and can help limit the spread of germs in a colony.";

				public static LocString EFFECT = "Increases Duplicant climbing speed.";
			}

			public class LIQUIDCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pipe", "LIQUIDCONDUIT");

				public static LocString DESC = "Liquid pipes are used to connect the inputs and outputs of plumbed buildings.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Carries ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" between ",
					UI.FormatAsLink("Outputs", "LIQUIDPIPING"),
					" and ",
					UI.FormatAsLink("Intakes", "LIQUIDPIPING"),
					".\n\nCan be run through tiles."
				});
			}

			public class LIQUIDCONDUITBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Bridge", "LIQUIDCONDUITBRIDGE");

				public static LocString DESC = "Separate pipe systems prevent mingled contents from causing building damage.";

				public static LocString EFFECT = "Runs one " + UI.FormatAsLink("Liquid Pipe", "LIQUIDPIPING") + " section over another without joining them.\n\nCan be run through tiles.";
			}

			public class LIQUIDCOOLEDFAN
			{
				public static LocString NAME = UI.FormatAsLink("Hydrofan", "LIQUIDCOOLEDFAN");

				public static LocString DESC = "A Duplicant can work a hydrofan to temporarily cool small areas as needed.";

				public static LocString EFFECT = "Dissipates a small amount of the " + UI.FormatAsLink("Heat", "HEAT") + ".";
			}

			public class BURNER
			{
				public static LocString NAME = UI.FormatAsLink("Burner", "BURNER");

				public static LocString DESC = "Burners reach incredible temperatures and can quickly melt large areas.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Produces a searing ",
					UI.FormatAsLink("Heat Jet", "HEAT"),
					" using liquid ",
					ELEMENTS.CRUDEOIL.NAME,
					"."
				});
			}

			public class CREATURETRAP
			{
				public static LocString NAME = UI.FormatAsLink("Critter Trap", "CREATURETRAP");

				public static LocString DESC = "Traps cannot catch flying critters.";

				public static LocString EFFECT = "Captures a living critter for transport.\n\nSingle use.";
			}

			public class CREATUREDELIVERYPOINT
			{
				public static LocString NAME = UI.FormatAsLink("Critter Drop-Off", "CREATUREDELIVERYPOINT");

				public static LocString DESC = "Duplicants automatically bring captured critters to these relocation points for release.";

				public static LocString EFFECT = "Releases trapped critters back into the world.\n\nCan be used multiple times.";
			}

			public class LIQUIDFILTER
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Filter", "LIQUIDFILTER");

				public static LocString DESC = "Liquid filters send the selected liquid into a special pipe, expelling everything else through a second output.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Sieves one ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" out of a mix, sending it into a dedicated ",
					UI.FormatAsLink("Pipe", "LIQUIDPIPING"),
					"."
				});
			}

			public class LIQUIDPUMP
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pump", "LIQUIDPUMP");

				public static LocString DESC = "Piping a pump's intake to another building's output will send liquid to that building.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Draws in ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" and runs it through ",
					UI.FormatAsLink("Pipes", "LIQUIDPIPING"),
					".\n\nMust be submerged in liquid."
				});
			}

			public class LIQUIDMINIPUMP
			{
				public static LocString NAME = UI.FormatAsLink("Mini Liquid Pump", "LIQUIDMINIPUMP");

				public static LocString DESC = "Mini pumps are useful for moving small quantities of liquid with minimum power draw.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Draws in a small amount of ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" and runs it through ",
					UI.FormatAsLink("Pipes", "LIQUIDPIPING"),
					".\n\nMust be submerged in liquid."
				});
			}

			public class LIQUIDPUMPINGSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Pitcher Pump", "LIQUIDPUMPINGSTATION");

				public static LocString DESC = "Pitcher pumps are used to fetch water and other liquids for delivery to buildings.";

				public static LocString EFFECT = "Manually pumps " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " into bottles for transport.\n\nDuplicants can only carry bottled liquids.";
			}

			public class LIQUIDVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Valve", "LIQUIDVALVE");

				public static LocString DESC = "Valves control the amount of liquid that moves through pipes, preventing waste.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Controls the ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" volume permitted through ",
					UI.FormatAsLink("Pipes", "LIQUIDPIPING"),
					"."
				});
			}

			public class LIQUIDLOGICVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Shutoff", "LIQUIDLOGICVALVE");

				public static LocString DESC = "Automated piping saves time and resources by removing the need for Duplicant management.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Automatically turns ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" flow on or off using ",
					UI.FormatAsLink("Automation", "LOGIC"),
					" technology."
				});
			}

			public class LIQUIDVENT
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Vent", "LIQUIDVENT");

				public static LocString DESC = "Vents are an exit point for liquids from plumbing systems.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Releases ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" from ",
					UI.FormatAsLink("Liquid Pipes", "LIQUIDPIPING"),
					"."
				});
			}

			public class MANUALGENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Manual Generator", "MANUALGENERATOR");

				public static LocString DESC = "Watching Duplicants run on it is adorable... the electrical power is just an added bonus.";

				public static LocString EFFECT = "Converts manual labor into electrical " + UI.FormatAsLink("Power", "POWER") + ".";
			}

			public class MANUALPRESSUREDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Manual Airlock", "MANUALPRESSUREDOOR");

				public static LocString DESC = "Airlocks can quarter off dangerous areas or prevent gases from seeping into the colony.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Blocks ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" and ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" flow, maintaining pressure between areas.\n\nSets Duplicant Access Permissions for area restriction.\n\nWild critters cannot pass through doors."
				});
			}

			public class MEDICALBED
			{
				public static LocString NAME = UI.FormatAsLink("Pharma Chamber", "MEDICALBED");

				public static LocString DESC = "Pharma Chambers decrease patients' disease recovery time, but cannot heal physical injuries.";

				public static LocString EFFECT = "Greatly accelerates recovery from " + UI.FormatAsLink("Diseases", "DISEASE") + ".";
			}

			public class MESHTILE
			{
				public static LocString NAME = UI.FormatAsLink("Mesh Tile", "MESHTILE");

				public static LocString DESC = "Mesh tiles can be used to make Duplicant pathways in areas where liquid needs to flow.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Used as floor and wall tile to build rooms.\n\nDoes not obstruct ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" or ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" flow."
				});
			}

			public class PLASTICTILE
			{
				public static LocString NAME = UI.FormatAsLink("Plastic Tile", "PLASTICTILE");

				public static LocString DESC = "Plastic tiles are mildly antiseptic and can help limit the spread of germs in a colony.";

				public static LocString EFFECT = "Used as floor and wall tile to build rooms.\n\nSignificantly increases Duplicant runspeed.";
			}

			public class GLASSTILE
			{
				public static LocString NAME = UI.FormatAsLink("Window Tile", "GLASSTILE");

				public static LocString DESC = "Window tiles allow the passage of light and decor, while blocking liquids and gasses. Plate construction makes windows susceptible to impact damage.";

				public static LocString EFFECT = "Used as floor and wall tile to build rooms.\n\nAllows light and decor to pass through";
			}

			public class METALTILE
			{
				public static LocString NAME = UI.FormatAsLink("Metal Tile", "METALTILE");

				public static LocString DESC = "Heat travels much more quickly through metal tiles than other types of flooring.";

				public static LocString EFFECT = "Used as floor and wall tile to build rooms.\n\nSignificantly increases Duplicant runspeed.";
			}

			public class BUNKERTILE
			{
				public static LocString NAME = UI.FormatAsLink("Bunker Tile", "BUNKERTILE");

				public static LocString DESC = "Extremely resilient to pressure and impact.";

				public static LocString EFFECT = "Used as floor and wall tile to build rooms.\n\nCan withstand extreme pressures and impacts.";
			}

			public class MICROBEMUSHER
			{
				public static LocString NAME = UI.FormatAsLink("Microbe Musher", "MICROBEMUSHER");

				public static LocString DESC = "Food from a musher will keep Duplicants alive, but may cause stress and disease over time.";

				public static LocString EFFECT = "Produces low quality " + UI.FormatAsLink("Food", "FOOD") + " using common ingredients.\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class MINERALDEOXIDIZER
			{
				public static LocString NAME = UI.FormatAsLink("Algae Deoxydizer", "MINERALDEOXIDIZER");

				public static LocString DESC = "Algae deoxydizers are inefficient, but can output enough " + ELEMENTS.OXYGEN.NAME + " to keep a colony breathing.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Converts large amounts of ",
					ELEMENTS.ALGAE.NAME,
					" into ",
					ELEMENTS.OXYGEN.NAME,
					".\n\nBecomes idle when the area reaches maximum pressure capacity."
				});
			}

			public class ORESCRUBBER
			{
				public static LocString NAME = UI.FormatAsLink("Ore Scrubber", "ORESCRUBBER");

				public static LocString DESC = "Ore scrubbers sanitize freshly mined resources before bringing them into the colony.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Kills a significant amount of ",
					UI.FormatAsLink("Germs", "DISEASE"),
					" present on ",
					UI.FormatAsLink("Raw Ore", "RAWMINERAL"),
					"."
				});
			}

			public class OUTHOUSE
			{
				public static LocString NAME = UI.FormatAsLink("Outhouse", "OUTHOUSE");

				public static LocString DESC = "They colony that eats together, excretes together.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Gives Duplicants a place to relieve themselves.\n\nRequires no ",
					UI.FormatAsLink("Piping", "GASPIPING"),
					".\n\nMust be periodically emptied of ",
					ELEMENTS.TOXICSAND.NAME,
					"."
				});
			}

			public class APOTHECARY
			{
				public static LocString NAME = UI.FormatAsLink("Apothecary", "APOTHECARY");

				public static LocString DESC = "All-natural remedies to cure what ails you.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Produces ",
					UI.FormatAsLink("Medicine", "MEDICINE"),
					" to cure most basic ",
					UI.FormatAsLink("Diseases", "DISEASE"),
					".\n\nDuplicants will not fabricate items unless recipes are queued."
				});
			}

			public class PLANTERBOX
			{
				public static LocString NAME = UI.FormatAsLink("Planter Box", "PLANTERBOX");

				public static LocString DESC = "Domestically grown seeds mature more quickly than wild plants.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Grows one ",
					UI.FormatAsLink("Plant", "PLANTS"),
					" from a ",
					UI.FormatAsLink("Seed", "PLANTS"),
					"."
				});
			}

			public class PRESSUREDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Mechanized Airlock", "PRESSUREDOOR");

				public static LocString DESC = "Mechanized airlocks have the same function as other doors, but open and close more quickly.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Blocks ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" and ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" flow, maintaining pressure between areas.\n\nSets Duplicant Access Permissions for area restriction.\n\nFunctions as a Manual Airlock when no ",
					UI.FormatAsLink("Power", "POWER"),
					" is available."
				});
			}

			public class BUNKERDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Bunker Door", "BUNKERDOOR");

				public static LocString DESC = "A massive, slow-moving door which is nearly indestructible.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Blocks ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					" and ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					" flow, maintaining pressure between areas.\n\nSets Duplicant Access Permissions for area restriction.\n\nHas very high impact and pressure resistance."
				});
			}

			public class RATIONBOX
			{
				public static LocString NAME = UI.FormatAsLink("Ration Box", "RATIONBOX");

				public static LocString DESC = "Ration boxes keep food safe from hungry critters, but don't slow food spoilage.";

				public static LocString EFFECT = "Stores a small amount of " + UI.FormatAsLink("Food", "FOOD") + ".\n\nFood must be delivered to boxes by Duplicants.";
			}

			public class REFRIGERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Refrigerator", "REFRIGERATOR");

				public static LocString DESC = "Food spoilage can be slowed by certain ambient conditions as well as by refrigerators.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Stores ",
					UI.FormatAsLink("Food", "FOOD"),
					" at an ideal ",
					UI.FormatAsLink("Temperature", "HEAT"),
					" to prevent spoilage."
				});

				public static LocString LOGIC_PORT_DESC = "Full/Not Full";
			}

			public class ROLESTATION
			{
				public static LocString NAME = UI.FormatAsLink("Jobs Board", "ROLESTATION");

				public static LocString DESC = "Employment can permanently improve Duplicants' skills and teach them unique traits.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Allows Duplicants to be assigned to specialized ",
					UI.FormatAsLink("Jobs", "JOBS"),
					".\n\nUnlocks ",
					UI.FormatAsLink("Hats", "JOBS"),
					"."
				});
			}

			public class RESEARCHCENTER
			{
				public static LocString NAME = UI.FormatAsLink("Research Station", "RESEARCHCENTER");

				public static LocString DESC = "Research stations are necessary for unlocking all research tiers.";

				public static LocString EFFECT = "Conducts " + UI.FormatAsLink("Novice Research", "RESEARCH") + " to unlock new technologies.";
			}

			public class ADVANCEDRESEARCHCENTER
			{
				public static LocString NAME = UI.FormatAsLink("Super Computer", "ADVANCEDRESEARCHCENTER");

				public static LocString DESC = "Super computers unlock higher technology tiers than research stations alone.";

				public static LocString EFFECT = "Conducts " + UI.FormatAsLink("Intermediate Research", "RESEARCH") + " to unlock new technologies.";
			}

			public class SCULPTURE
			{
				public static LocString NAME = UI.FormatAsLink("Sculpting Block", "SCULPTURE");

				public static LocString DESC = "Duplicants with high art skill will produce better, more decorative sculptures.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Increases ",
					UI.FormatAsLink("Decor", "DECOR"),
					" and reduces ",
					UI.FormatAsLink("Stress", "STRESS"),
					".\n\nMust be sculpted by a Duplicant."
				});

				public static LocString POORQUALITYNAME = "\"Abstract\" Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Genius Sculpture";
			}

			public class ICESCULPTURE
			{
				public static LocString NAME = UI.FormatAsLink("Ice Block", "ICESCULPTURE");

				public static LocString DESC = "Ice sculptures will melt over time if not kept sufficiently chilled.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Increases ",
					UI.FormatAsLink("Decor", "DECOR"),
					" and reduces ",
					UI.FormatAsLink("Stress", "STRESS"),
					".\n\nMust be sculpted by a Duplicant."
				});

				public static LocString POORQUALITYNAME = "\"Abstract\" Ice Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Genius Ice Sculpture";
			}

			public class SHEARINGSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Shearing Station", "SHEARINGSTATION");

				public static LocString DESC = "Shearing stations are used to shear Dreckos.";

				public static LocString EFFECT = "Shears dreckos.";
			}

			public class SUITMARKER
			{
				public static LocString NAME = UI.FormatAsLink("Exosuit Checkpoint", "SUITMARKER");

				public static LocString DESC = "A checkpoint must have an exosuit dock built on the opposite side its arrow faces.";

				public static LocString EFFECT = "Marks a threshold where Duplicants must change into or out of " + EQUIPMENT.PREFABS.ATMO_SUIT.NAME + ".\n\nMust be built next to an Exosuit Dock.\n\nCan be rotated before construction.";
			}

			public class SUITLOCKER
			{
				public static LocString NAME = UI.FormatAsLink("Exosuit Dock", "SUITLOCKER");

				public static LocString DESC = "Docks can refill exosuits with air or empty them of waste, but can only charge one suit at a time.";

				public static LocString EFFECT = "Stores and recharges " + EQUIPMENT.PREFABS.ATMO_SUIT.NAME + ".\n\nBuild next to an Exosuit Checkpoint to make Duplicants change into suits when passing by.";
			}

			public class SUITFABRICATOR
			{
				public static LocString NAME = UI.FormatAsLink("Exosuit Forge", "SUITFABRICATOR");

				public static LocString DESC = "Exosuits can be filled with oxygen to allow Duplicants to safely enter uninhabitable areas.";

				public static LocString EFFECT = "Forges protective " + EQUIPMENT.PREFABS.ATMO_SUIT.NAME + " for Duplicants to wear.\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class CLOTHINGFABRICATOR
			{
				public static LocString NAME = UI.FormatAsLink("Textile Loom", "CLOTHINGFABRICATOR");

				public static LocString DESC = "A textile loom can be used to spin Reed Fiber into wearable Duplicant clothing.";

				public static LocString EFFECT = "Tailors Duplicant " + UI.FormatAsLink("Clothing", "EQUIPMENT") + " items.\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class SPACEHEATER
			{
				public static LocString NAME = UI.FormatAsLink("Space Heater", "SPACEHEATER");

				public static LocString DESC = "A space heater will radiate heat for as long as it's powered.";

				public static LocString EFFECT = "Radiates a moderate amount of " + UI.FormatAsLink("Heat", "HEAT") + ".";
			}

			public class STORAGELOCKER
			{
				public static LocString NAME = UI.FormatAsLink("Storage Compactor", "STORAGELOCKER");

				public static LocString DESC = "Resources left on the floor become \"debris\" and lower decor when not put away.";

				public static LocString EFFECT = "Stores the resources of your choosing.";
			}

			public class STORAGELOCKERSMART
			{
				public static LocString NAME = UI.FormatAsLink("Smart Storage Compactor", "STORAGELOCKERSMART");

				public static LocString DESC = "This improved version of the Storage Compactor becomes " + UI.FormatAsLink("Active", "LOGIC") + " when full";

				public static LocString EFFECT = "Stores the resources of your choosing.";
			}

			public class LIQUIDHEATER
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Tepidizer", "LIQUIDHEATER");

				public static LocString DESC = "Liquid tepidizers kill waterborne germs and heat liquid to a perfect showering temperature.";

				public static LocString EFFECT = "Warms large bodies of " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".\n\nMust be fully submerged.";
			}

			public class SWITCH
			{
				public static LocString NAME = UI.FormatAsLink("Switch", "SWITCH");

				public static LocString DESC = "Switches do not affect buildings preceding them on a circuit, but control all buildings after.";

				public static LocString EFFECT = "Turns " + UI.FormatAsLink("Power", "POWER") + " on or off.\n\nDoes not affect circuitry preceding the switch.";

				public static LocString TURN_ON = "Turn On";

				public static LocString TURN_OFF = "Turn Off";
			}

			public class LOGICPOWERRELAY
			{
				public static LocString NAME = UI.FormatAsLink("Power Shutoff", "LOGICPOWERRELAY");

				public static LocString DESC = "Automated systems save power and time by removing the need for Duplicant management.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Automatically turns ",
					UI.FormatAsLink("Power", "POWER"),
					" on or off using ",
					UI.FormatAsLink("Automation", "LOGIC"),
					" technology.\n\nDoes not affect circuitry preceding the switch."
				});

				public static LocString LOGIC_PORT_DESC = "On/Off";
			}

			public class TEMPERATURECONTROLLEDSWITCH
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Switch", "TEMPERATURECONTROLLEDSWITCH");

				public static LocString DESC = "Automated switches can be used to manage circuits in areas where Duplicants cannot enter.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Automatically turns ",
					UI.FormatAsLink("Power", "POWER"),
					" on or off using ambient ",
					UI.FormatAsLink("Temperature", "HEAT"),
					".\n\nDoes not affect circuitry preceding the switch."
				});
			}

			public class PRESSURESWITCHLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Hydro Switch", "PRESSURESWITCHLIQUID");

				public static LocString DESC = "A hydro switch shuts off power when the liquid pressure surrounding it surpasses the set threshold.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Automatically turns ",
					UI.FormatAsLink("Power", "POWER"),
					" on or off using ambient ",
					UI.FormatAsLink("Liquid Pressure", "PRESSURE"),
					".\n\nDoes not affect circuitry preceding the switch.\n\nMust be submerged in ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					"."
				});
			}

			public class PRESSURESWITCHGAS
			{
				public static LocString NAME = UI.FormatAsLink("Atmo Switch", "PRESSURESWITCHGAS");

				public static LocString DESC = "An atmo switch shuts off power when the air pressure surrounding it surpasses the set threshold.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Automatically turns ",
					UI.FormatAsLink("Power", "POWER"),
					" on or off using ambient ",
					UI.FormatAsLink("Gas Pressure", "PRESSURE"),
					".\n\nDoes not affect circuitry preceding the switch."
				});
			}

			public class TILE
			{
				public static LocString NAME = UI.FormatAsLink("Tile", "TILE");

				public static LocString DESC = "Tiles can be used to build rooms, floors, or bridges to new areas.";

				public static LocString EFFECT = "Used as floor and wall tile to build rooms.\n\nIncreases Duplicant runspeed.";
			}

			public class WATERPURIFIER
			{
				public static LocString NAME = UI.FormatAsLink("Water Sieve", "WATERPURIFIER");

				public static LocString DESC = "Sieves cannot kill germs and pass any disease they receive into their waste and water output.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Produces clean ",
					ELEMENTS.WATER.NAME,
					" from ",
					ELEMENTS.DIRTYWATER.NAME,
					" using ",
					ELEMENTS.SAND.NAME,
					".\n\nProduces ",
					ELEMENTS.TOXICSAND.NAME,
					"."
				});
			}

			public class DISTILLATIONCOLUMN
			{
				public static LocString NAME = UI.FormatAsLink("Distillation Column", "DISTILLATIONCOLUMN");

				public static LocString DESC = "Gets hot and steamy.";

				public static LocString EFFECT = string.Format("Separates any {0} piped through it into " + ELEMENTS.STEAM.NAME + " and {1}", ELEMENTS.DIRTYWATER.NAME.text, ELEMENTS.TOXICSAND.NAME.text);
			}

			public class WIRE
			{
				public static LocString NAME = UI.FormatAsLink("Wire", "WIRE");

				public static LocString DESC = "Electrical wire is used to connect generators, batteries, and buildings.";

				public static LocString EFFECT = "Connects buildings to " + UI.FormatAsLink("Power", "POWER") + " sources.\n\nCan be run through tiles.";
			}

			public class WIREBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Wire Bridge", "WIREBRIDGE");

				public static LocString DESC = "Splitting generators onto separate systems can prevent power overloads and wasted electricity.";

				public static LocString EFFECT = "Runs one wire section over another without joining them.\n\nCan be run through tiles.";
			}

			public class HIGHWATTAGEWIRE
			{
				public static LocString NAME = UI.FormatAsLink("Heavi-Watt Wire", "HIGHWATTAGEWIRE");

				public static LocString DESC = "Higher wattage wire is used to avoid power overloads, particularly for strong generators.";

				public static LocString EFFECT = "Carries more " + UI.FormatAsLink("Wattage", "POWER") + " than regular Wire without overloading.\n\nCannot be run through tiles.";
			}

			public class WIREBRIDGEHIGHWATTAGE
			{
				public static LocString NAME = UI.FormatAsLink("Heavi-Watt Joint Plate", "WIREBRIDGEHIGHWATTAGE");

				public static LocString DESC = "Joint plates can run Heavi wires through walls without leaking gas or liquid.";

				public static LocString EFFECT = "Allows Heavi-Watt Wire to be run through wall and floor tiles.\n\nFunctions as a regular tile.";
			}

			public class WIREREFINED
			{
				public static LocString NAME = UI.FormatAsLink("Conductive Wire", "WIREREFINED");

				public static LocString DESC = "My Duplicants prefer the look of conductive wire to the regular raggedy stuff.";

				public static LocString EFFECT = "Connects buildings to " + UI.FormatAsLink("Power", "POWER") + " sources.\n\nCan be run through tiles.";
			}

			public class WIREREFINEDBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Conductive Wire Bridge", "WIREREFINEDBRIDGE");

				public static LocString DESC = "Splitting generators onto separate systems can prevent power overloads and wasted electricity.";

				public static LocString EFFECT = "Carries more " + UI.FormatAsLink("Wattage", "POWER") + " than a regular Wire Bridge without overloading.\n\nRuns one wire section over another without joining them.\n\nCan be run through tiles.";
			}

			public class WIREREFINEDHIGHWATTAGE
			{
				public static LocString NAME = UI.FormatAsLink("Heavi Conductive Wire", "WIREREFINEDHIGHWATTAGE");

				public static LocString DESC = "Higher wattage wire is used to avoid power overloads, particularly for strong generators.";

				public static LocString EFFECT = "Carries more " + UI.FormatAsLink("Wattage", "POWER") + " than regular Wire without overloading.\n\nCannot be run through tiles.";
			}

			public class WIREREFINEDBRIDGEHIGHWATTAGE
			{
				public static LocString NAME = UI.FormatAsLink("Heavi Conductive Joint Plate", "WIREREFINEDBRIDGEHIGHWATTAGE");

				public static LocString DESC = "Joint plates can run Heavi wires through walls without leaking gas or liquid.";

				public static LocString EFFECT = "Carries more " + UI.FormatAsLink("Wattage", "POWER") + " than a regular Heavi-Watt Joint Plate without overloading.\n\nAllows Heavi-Watt Wire to be run through wall and floor tiles.\n\nFunctions as a regular tile.";
			}

			public class HANDSANITIZER
			{
				public static LocString NAME = UI.FormatAsLink("Hand Sanitizer", "HANDSANITIZER");

				public static LocString DESC = "Hand sanitizers kill germs more effectively than wash basins.";

				public static LocString EFFECT = "Removes most " + UI.FormatAsLink("Germs", "DISEASE") + " from Duplicants.\n\nGerm-covered Duplicants use Hand Sanitizers when passing by in the selected direction.";
			}

			public class WASHBASIN
			{
				public static LocString NAME = UI.FormatAsLink("Wash Basin", "WASHBASIN");

				public static LocString DESC = "Germ spread can be reduced by building wash basins where Duplicants are likely to get dirty.";

				public static LocString EFFECT = "Removes some " + UI.FormatAsLink("Germs", "DISEASE") + " from Duplicants.\n\nGerm-covered Duplicants use Wash Basins when passing by in the selected direction.";
			}

			public class WASHSINK
			{
				public static LocString NAME = UI.FormatAsLink("Sink", "WASHSINK");

				public static LocString DESC = "Sinks are plumbed and do not need to be manually emptied or refilled.";

				public static LocString EFFECT = "Removes " + UI.FormatAsLink("Germs", "DISEASE") + " from Duplicants.\n\nGerm-covered Duplicants use Sinks when passing by in the selected direction.";
			}

			public class TILEPOI
			{
				public static LocString NAME = UI.FormatAsLink("Tile", "TILEPOI");

				public static LocString DESC = string.Empty;

				public static LocString EFFECT = "Used as floor and wall tile to build rooms.";
			}

			public class POLYMERIZER
			{
				public static LocString NAME = UI.FormatAsLink("Polymer Press", "POLYMERIZER");

				public static LocString DESC = "Plastic can be used to craft unique buildings and goods.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Converts ",
					ELEMENTS.PETROLEUM.NAME,
					" into raw ",
					ELEMENTS.POLYPROPYLENE.NAME,
					"."
				});
			}

			public class DIRECTIONALWORLDPUMPLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Channel", "DIRECTIONALWORLDPUMPLIQUID");

				public static LocString DESC = "Channels move more volume than pumps and require no power, but need sufficient pressure to function.";

				public static LocString EFFECT = "Directionally moves large volumes of liquid through a channel.\n\nCan be used as floor tile and rotated before construction.";
			}

			public class STEAMTURBINE
			{
				public static LocString NAME = UI.FormatAsLink("Steam Turbine", "STEAMTURBINE");

				public static LocString DESC = "Useful for converting the geothermal energy of magma into usable power.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Generates exceptional electrical ",
					UI.FormatAsLink("Power", "POWER"),
					" using pressurized, ",
					UI.FormatAsLink("Scalding", "HEAT"),
					" ",
					ELEMENTS.STEAM.NAME,
					".\n\nOutputs significantly cooler ",
					ELEMENTS.STEAM.NAME,
					" than it receives.\n\nAir pressure beneath this building must be higher than pressure above for air to flow."
				});
			}

			public class SOLARPANEL
			{
				public static LocString NAME = UI.FormatAsLink("Solar Panel", "SOLARPANEL");

				public static LocString DESC = "Solar panels convert high intensity Sunlight into usable power.";

				public static LocString EFFECT = "Converts Sunlight into electrical " + UI.FormatAsLink("Power", "POWER") + ".";
			}

			public class COMETDETECTOR
			{
				public static LocString NAME = UI.FormatAsLink("Meteor Scanner", "COMETDETECTOR");

				public static LocString DESC = "Scans the sky to predict incoming meteor showers.";

				public static LocString EFFECT = "Produces a logic signal before a meteor shower begins.";
			}

			public class OILREFINERY
			{
				public static LocString NAME = UI.FormatAsLink("Oil Refinery", "OILREFINERY");

				public static LocString DESC = "Petroleum can only be produced from the refinement of crude oil.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Converts ",
					ELEMENTS.CRUDEOIL.NAME,
					" into ",
					ELEMENTS.PETROLEUM.NAME,
					" and ",
					ELEMENTS.METHANE.NAME,
					"."
				});
			}

			public class OILWELLCAP
			{
				public static LocString NAME = UI.FormatAsLink("Oil Well", "OILWELLCAP");

				public static LocString DESC = "Water pumped into an oil reservoir cannot be recovered.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Extracts ",
					ELEMENTS.CRUDEOIL.NAME,
					" using clean ",
					ELEMENTS.WATER.NAME,
					".\n\nMust be built atop an Oil Reservoir."
				});
			}

			public class METALREFINERY
			{
				public static LocString NAME = UI.FormatAsLink("Metal Refinery", "METALREFINERY");

				public static LocString DESC = "Refined metals are necessary to build advanced electronics and technologies.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Produces ",
					UI.FormatAsLink("Refined Metals", "REFINEDMETAL"),
					" from raw ",
					UI.FormatAsLink("Metal Ore", "RAWMETAL"),
					".\n\nOutputs significantly ",
					UI.FormatAsLink("Heated", "HEAT"),
					" ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					"."
				});

				public static LocString RECIPE_DESCRIPTION = "Extracts pure {0} from {1}.";
			}

			public class GLASSFORGE
			{
				public static LocString NAME = UI.FormatAsLink("Glass Forge", "GLASSFORGE");

				public static LocString DESC = "Refined metals are necessary to build advanced electronics and technologies.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Produces ",
					UI.FormatAsLink("Molten Glass", "GLASS"),
					" from raw ",
					UI.FormatAsLink("Sand", "SAND"),
					".\n\nOutputs significantly ",
					UI.FormatAsLink("Heated", "HEAT"),
					" ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					"."
				});

				public static LocString RECIPE_DESCRIPTION = "Extracts pure {0} from {1}.";
			}

			public class SPACEWALL
			{
				public static LocString NAME = UI.FormatAsLink("Space Wall", "SPACEWALL");

				public static LocString DESC = "Keeps the space out and the atmosphere in";

				public static LocString EFFECT = "...";
			}

			public class ROCKCRUSHER
			{
				public static LocString NAME = UI.FormatAsLink("Rock Granulator", "ROCKCRUSHER");

				public static LocString DESC = "Sand is used in an array of liquid and air filtering processes.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Produces ",
					ELEMENTS.SAND.NAME,
					" from ",
					UI.FormatAsLink("Raw Mineral", "RAWMINERAL"),
					".\n\nInefficiently produces ",
					UI.FormatAsLink("Refined Metals", "REFINEDMETAL"),
					" from raw ",
					UI.FormatAsLink("Metal Ore", "RAWMETAL"),
					"."
				});

				public static LocString RECIPE_DESCRIPTION = "Crushes {0} into {1}.";

				public static LocString METAL_RECIPE_DESCRIPTION = "Crushes {1} into " + UI.FormatAsLink("Sand", "SAND") + " and pure {0}.";

				public static LocString LIME_RECIPE_DESCRIPTION = "Crushes {1} into {0}";
			}

			public class THERMALBLOCK
			{
				public static LocString NAME = UI.FormatAsLink("Tempshift Plate", "THERMALBLOCK");

				public static LocString DESC = "Construction materials have different thermal properties that affect how well they retain heat.";

				public static LocString EFFECT = "Accelerates or buffers " + UI.FormatAsLink("Heat", "HEAT") + " dispersal depending on the construction material used.\n\nHas a small area of effect.";
			}

			public class POWERCONTROLSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Power Control Station", "POWERCONTROLSTATION");

				public static LocString DESC = "Only one Duplicant may be assigned to a Station at a time.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Produces ",
					ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.NAME,
					" to increase the ",
					UI.FormatAsLink("Power", "POWER"),
					" output of generators.\n\nAssigned Duplicants must possess the ",
					UI.FormatAsLink("Tune Up", UI.StripLinkFormatting("PowerTechnician")),
					" trait.\n\nThis building is a necessary component of the Power Plant room."
				});
			}

			public class FARMSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Farm Station", "FARMSTATION");

				public static LocString DESC = "This station only has an effect on crops grown within the same room.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Produces ",
					ITEMS.INDUSTRIAL_PRODUCTS.FARM_STATION_TOOLS.NAME,
					" to increase ",
					UI.FormatAsLink("Plant", "PLANTS"),
					" growth rates.\n\nAssigned Duplicants must possess the ",
					UI.FormatAsLink("Crop Tending", UI.StripLinkFormatting("Farmer")),
					" trait.\n\nThis building is a necessary component of the Greenhouse room."
				});
			}

			public class FISHDELIVERYPOINT
			{
				public static LocString NAME = UI.FormatAsLink("Fish Release", "FISHDELIVERYPOINT");

				public static LocString DESC = "This building must be built above liquid to prevent fish from suffocating.";

				public static LocString EFFECT = "Releases trapped fish back into the world.\n\nCan be used multiple times.";
			}

			public class FISHFEEDER
			{
				public static LocString NAME = UI.FormatAsLink("Fish Feeder", "FISHFEEDER");

				public static LocString DESC = "Build this feeder above a body of water to feed the fish within.";

				public static LocString EFFECT = "Automatically dispenses stored food into the area below.\n\nDispenses once per day.";
			}

			public class FISHTRAP
			{
				public static LocString NAME = UI.FormatAsLink("Fish Trap", "FISHTRAP");

				public static LocString DESC = "Trapped fish will automatically be bagged for transport.";

				public static LocString EFFECT = "Attracts and traps swimming fish.\n\nSingle use.";
			}

			public class RANCHSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Grooming Station", "RANCHSTATION");

				public static LocString DESC = "Grooming critters make them look nice, smell pretty, feel happy, and produce more.";

				public static LocString EFFECT = "Allows the assigned Rancher to care for critters.\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Critter Wrangling", UI.StripLinkFormatting("Rancher")) + " trait.\n\nThis building is a necessary component of the Stable room.";
			}

			public class MACHINESHOP
			{
				public static LocString NAME = UI.FormatAsLink("Mechanics Station", "MACHINESHOP");

				public static LocString DESC = "Duplicants will only improve the efficiency of buildings in the same room as this station.";

				public static LocString EFFECT = "Allows the assigned Engineer to improve building production efficiency.\n\nThis building is a necessary component of the Machine Shop room.";
			}

			public class MASSIVEHEATSINK
			{
				public static LocString NAME = UI.FormatAsLink("Anti Entropy Thermo-Nullifier", "MASSIVEHEATSINK");

				public static LocString DESC = string.Empty;

				public static LocString EFFECT = string.Concat(new string[]
				{
					"A self-sustaining machine powered by what appears to be refined ",
					ELEMENTS.UNOBTANIUM.NAME,
					".\n\nAbsorbs and neutralizes ",
					UI.FormatAsLink("Heat", "HEAT"),
					" energy when submersed in ",
					ELEMENTS.HYDROGEN.NAME,
					"."
				});
			}

			public class LOGICWIRE
			{
				public static LocString NAME = UI.FormatAsLink("Automation Wire", "LOGICWIRE");

				public static LocString DESC = "Automation wire is used to connect electronic components to the inputs of automation gates.";

				public static LocString EFFECT = "Connects automatable buildings to " + UI.FormatAsLink("Automation Gates", "LOGIC") + ".\n\nCan be run through tiles.";
			}

			public class LOGICWIREBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Automation Wire Bridge", "LOGICWIREBRIDGE");

				public static LocString DESC = "Wire bridges allow multiple automation grids to exist in a small area without connecting.";

				public static LocString EFFECT = "Runs one Automation Wire section over another without joining them.\n\nCan be run through tiles.";
			}

			public class LOGICGATEAND
			{
				public static LocString NAME = UI.FormatAsLink("AND Gate", "LOGICGATEAND");

				public static LocString DESC = "This gate only turns buildings on when both the input buildings are on active the same time.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" when the systems connected to its Inputs are both ",
					UI.FormatAsLink("Active", "LOGIC"),
					".\n\nGoes into ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" when one or both Inputs are on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					"."
				});
			}

			public class LOGICGATEOR
			{
				public static LocString NAME = UI.FormatAsLink("OR Gate", "LOGICGATEOR");

				public static LocString DESC = "This gate can only turn buildings on if it has one or more active inputs connected to it.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" if one or both of the systems connected to its Inputs are ",
					UI.FormatAsLink("Active", "LOGIC"),
					".\n\nGoes into ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" when neither Inputs are ",
					UI.FormatAsLink("Active", "LOGIC"),
					"."
				});
			}

			public class LOGICGATENOT
			{
				public static LocString NAME = UI.FormatAsLink("NOT Gate", "LOGICGATENOT");

				public static LocString DESC = "This gate reverses logic, turning buildings on when it receives off values and vice versa.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" if the system connected to its Input is on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					".\n\nGoes into ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" when its Input is ",
					UI.FormatAsLink("Active", "LOGIC"),
					"."
				});
			}

			public class LOGICGATEXOR
			{
				public static LocString NAME = UI.FormatAsLink("XOR Gate", "LOGICGATEXOR");

				public static LocString DESC = "This gate needs exactly one of its inputs to be active to turn on buildings.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" if one of the systems connected to its Inputs is ",
					UI.FormatAsLink("Active", "LOGIC"),
					".\n\nGoes into ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" if both or neither Inputs are ",
					UI.FormatAsLink("Active", "LOGIC"),
					"."
				});
			}

			public class LOGICGATEBUFFER
			{
				public static LocString NAME = UI.FormatAsLink("BUFFER Gate", "LOGICGATEBUFFER");

				public static LocString DESC = "This gate will keep buildings running for a short time after the input building is deactivated.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" if the system connected to its Input is ",
					UI.FormatAsLink("Active", "LOGIC"),
					".\n\nStays ",
					UI.FormatAsLink("Active", "LOGIC"),
					" for a short time after its Input enters ",
					UI.FormatAsLink("Standby", "LOGIC"),
					"."
				});
			}

			public class LOGICGATEFILTER
			{
				public static LocString NAME = UI.FormatAsLink("FILTER Gate", "LOGICGATEFILTER");

				public static LocString DESC = "This gate will keep buildings turned off for a short time after the input building is activated.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Enters ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" if the system connected to its Input is also on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					".\n\nStays in ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" for a short time after its Input becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					"."
				});
			}

			public class LOGICMEMORY
			{
				public static LocString NAME = UI.FormatAsLink("Memory Toggle", "LOGICMEMORY");

				public static LocString DESC = string.Empty;

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" when the ",
					UI.FormatAsLink("logic", "Set"),
					" port is ",
					UI.FormatAsLink("Active", "LOGIC"),
					". Goes to ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" when the reset port is ",
					UI.FormatAsLink("Active", "LOGIC"),
					"."
				});

				public static LocString STATUS_ITEM_VALUE = "Current Value: {0}";

				public static LocString READ_PORT_DESC = "Current Value";

				public static LocString SET_PORT_DESC = "Make Active";

				public static LocString RESET_PORT_DESC = "Make Inactive";
			}

			public class LOGICSWITCH
			{
				public static LocString NAME = UI.FormatAsLink("Signal Switch", "LOGICSWITCH");

				public static LocString DESC = "Signal switches do not turn grids on and off like power switches, but add an additional signal.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Can be made ",
					UI.FormatAsLink("Active", "LOGIC"),
					" or ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" on an ",
					UI.FormatAsLink("Automation", "LOGIC"),
					" grid.\n\nMust be manually toggled by a Duplicant."
				});

				public static LocString LOGIC_PORT_DESC = "Active/Standby";
			}

			public class LOGICPRESSURESENSORGAS
			{
				public static LocString NAME = UI.FormatAsLink("Atmo Sensor", "LOGICPRESSURESENSORGAS");

				public static LocString DESC = "Atmo Sensors can be used to prevent excess oxygen production and overpressurization.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" or on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" when Gas Pressure enters the chosen range."
				});

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on ambient Gas Pressure";
			}

			public class LOGICPRESSURESENSORLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Hydro Sensor", "LOGICPRESSURESENSORLIQUID");

				public static LocString DESC = "Hydro Sensors can signal a pump system to refill a basin once it contains too little liquid.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" or on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" when Liquid Pressure enters the chosen range.\n\nMust be submerged in ",
					UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
					"."
				});

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on Ambient Liquid Pressure";
			}

			public class LOGICTEMPERATURESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Sensor", "LOGICTEMPERATURESENSOR");

				public static LocString DESC = "Thermo Sensors can autodisable buildings when they approach inoperable temperatures.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" or on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" when ambient ",
					UI.FormatAsLink("Temperature", "HEAT"),
					" enters the chosen range."
				});

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on Ambient " + UI.FormatAsLink("Temperature", "HEAT") + string.Empty;
			}

			public class LOGICTIMEOFDAYSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Clock Sensor", "LOGICTIMEOFDAYSENSOR");

				public static LocString DESC = "Clock Sensors ensure that systems always turn on at the same time of day or night, every cycle.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Sets an automatic ",
					UI.FormatAsLink("Active", "LOGIC"),
					" and ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" schedule using a timer."
				});
			}

			public class LOGICDISEASESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Germ Sensor", "LOGICDISEASESENSOR");

				public static LocString DESC = "Detects the presence and quantity of nearby germs";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" or on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" depending on quantity of surrounding ",
					UI.FormatAsLink("Germs", "DISEASE"),
					"."
				});

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on ambient Germs";
			}

			public class LOGICELEMENTSENSORGAS
			{
				public static LocString NAME = UI.FormatAsLink("Gaseous Element Sensor", "LOGICELEMENTSENSORGAS");

				public static LocString DESC = "These sensors can detect the presence of a specific gas and alter systems accordingly.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" when the selected Gaseous Element is detected.\n\nRemains on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" when the Element is not present."
				});

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on the ambient Gas";
			}

			public class LOGICELEMENTSENSORLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Element Sensor", "LOGICELEMENTSENSORLIQUID");

				public static LocString DESC = "These sensors can detect the presence of a specific liquid and alter systems accordingly.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" when the selected Liquid Element is detected.\n\nRemains on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" when the Element is not present."
				});

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on the ambient Liquid";
			}

			public class GASCONDUITDISEASESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pipe Germ Sensor", "GASCONDUITDISEASESENSOR");

				public static LocString DESC = "Gas Pipe Germ Sensors are used to control automation behaviour in the presence of disease.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes",
					UI.FormatAsLink("Active", "LOGIC"),
					" or on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" depending on quantity of the contained ",
					UI.FormatAsLink("Germs", "DISEASE"),
					"."
				});

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on the contained Germs";
			}

			public class LIQUIDCONDUITDISEASESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pipe Germ Sensor", "LIQUIDCONDUITDISEASESENSOR");

				public static LocString DESC = "Liquid Pipe Germ Sensors are used to control automation behaviour in the presence of disease.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" or on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" depending on quantity of the contained ",
					UI.FormatAsLink("Germs", "DISEASE"),
					"."
				});

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on the contained Germs";
			}

			public class GASCONDUITELEMENTSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pipe Element Sensor", "LOGICELEMENTSENSORGAS");

				public static LocString DESC = "Gas Pipe Element Sensors can be used to detect the presence of a specific gas in a pipe";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " when the configured Gas Element is detected.";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on the contained Gas";
			}

			public class LIQUIDCONDUITELEMENTSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pipe Element Sensor", "LOGICELEMENTSENSORLIQUID");

				public static LocString DESC = "Liquid Pipe Element Sensors can be used to detect the presence of a specific liquid in a pipe";

				public static LocString EFFECT = "Becomes " + UI.FormatAsLink("Active", "LOGIC") + " when the configured Liquid Element is detected.";

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on the contained Liquid";
			}

			public class GASCONDUITTEMPERATURESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pipe Thermo Sensor", "GASCONDUITTEMPERATURESENSOR");

				public static LocString DESC = "Gas Pipe Thermo Sensors can disable buildings when contents reach a certain temperature.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" or on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" when the pipe contents ",
					UI.FormatAsLink("Temperature", "HEAT"),
					" enters the chosen range."
				});

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on the " + UI.FormatAsLink("Temperature", "HEAT") + " of the contained Gas";
			}

			public class LIQUIDCONDUITTEMPERATURESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pipe Thermo Sensor", "LIQUIDCONDUITTEMPERATURESENSOR");

				public static LocString DESC = "Liquid Pipe Thermo Sensors can disable buildings when their contents reach a certain temperature.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Becomes ",
					UI.FormatAsLink("Active", "LOGIC"),
					" or on ",
					UI.FormatAsLink("Standby", "LOGIC"),
					" when the pipe contents ",
					UI.FormatAsLink("Temperature", "HEAT"),
					" enters the chosen range."
				});

				public static LocString LOGIC_PORT_DESC = "Active/Standby based on the " + UI.FormatAsLink("Temperature", "HEAT") + " of the contained Liquid";
			}

			public class TRAVELTUBEENTRANCE
			{
				public static LocString NAME = UI.FormatAsLink("Transit Tube Access", "TRAVELTUBEENTRANCE");

				public static LocString DESC = "Access points are required for Duplicants to enter tubes, but are not required to exit them.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Allows Duplicants to enter the connected ",
					BUILDINGS.PREFABS.TRAVELTUBE.NAME,
					" system.\n\nStops drawing ",
					UI.FormatAsLink("Power", "POWER"),
					" once fully charged."
				});
			}

			public class TRAVELTUBE
			{
				public static LocString NAME = UI.FormatAsLink("Transit Tube", "TRAVELTUBE");

				public static LocString DESC = "Duplicants will only exit a transit tube when a safe landing area is available beneath it.";

				public static LocString EFFECT = "Quickly transports Duplicants from a " + UI.FormatAsLink("Transit Tube Access", "TRAVELTUBEENTRANCE") + " to the tube's end.\n\nOnly transports Duplicants.";
			}

			public class TRAVELTUBEWALLBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Transit Tube Crossing", "TRAVELTUBEWALLBRIDGE");

				public static LocString DESC = "Tube crossings can run transit tubes through walls without leaking gas or liquid.";

				public static LocString EFFECT = "Allows " + BUILDINGS.PREFABS.TRAVELTUBE.NAME + " to be run through wall and floor tiles.\n\nFunctions as a regular tile.";
			}

			public class SOLIDCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT");

				public static LocString DESC = "Rails automatically move materials where they'll be needed most, saving Duplicants the walk.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Transports ",
					UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID"),
					" on a track between ",
					BUILDINGS.PREFABS.SOLIDCONDUITINBOX.NAME,
					" and ",
					BUILDINGS.PREFABS.SOLIDCONDUITOUTBOX.NAME,
					".\n\nCan be run through tiles."
				});
			}

			public class SOLIDCONDUITINBOX
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Loader", "SOLIDCONDUITINBOX");

				public static LocString DESC = "The filters on a loader can be used to determine what materials should be sent down the rail.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Loads ",
					UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID"),
					" onto ",
					UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT"),
					" for transport.\n\nOnly loads the resources of your choosing."
				});
			}

			public class SOLIDCONDUITOUTBOX
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Receptacle", "SOLIDCONDUITOUTBOX");

				public static LocString DESC = "When materials reach the end of a rail they enter a receptacle to be used by Duplicants.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Unloads ",
					UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID"),
					" from a ",
					UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT"),
					" into storage."
				});
			}

			public class SOLIDTRANSFERARM
			{
				public static LocString NAME = UI.FormatAsLink("Auto-Sweeper", "SOLIDTRANSFERARM");

				public static LocString DESC = "An auto-sweeper's suction range can be viewed at any time by clicking on the building.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Automates ",
					UI.FormatAsLink("Sweeping", "CHORES"),
					" and ",
					UI.FormatAsLink("Supplying", "CHORES"),
					" errands by sucking up all nearby ",
					UI.FormatAsLink("Debris", "DECOR"),
					".\n\nMaterials are automatically delivered to any ",
					BUILDINGS.PREFABS.SOLIDCONDUITINBOX.NAME,
					", ",
					BUILDINGS.PREFABS.SOLIDCONDUITOUTBOX.NAME,
					", storage, or buildings within range."
				});
			}

			public class SOLIDCONDUITBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Bridge", "SOLIDCONDUITBRIDGE");

				public static LocString DESC = "Separating rail systems helps prevent materials from reaching the wrong destinations.";

				public static LocString EFFECT = "Runs one " + UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") + " section over another without joining them.\n\nCan be run through tiles.";
			}

			public class CREATUREFEEDER
			{
				public static LocString NAME = UI.FormatAsLink("Critter Feeder", "CREATUREFEEDER");

				public static LocString DESC = "Critters tend to stay close to their food source and will wander less when given a Feeder.";

				public static LocString EFFECT = "Automatically dispenses food for hungry critters.";
			}

			public class EGGINCUBATOR
			{
				public static LocString NAME = UI.FormatAsLink("Incubator", "EGGINCUBATOR");

				public static LocString DESC = "Incubators can maintain the ideal internal conditions for several species of critter egg.";

				public static LocString EFFECT = "Incubates critter eggs until ready to hatch.\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Critter Wrangling", "Rancher") + " trait.";
			}

			public class EGGCRACKER
			{
				public static LocString NAME = UI.FormatAsLink("Egg Cracker", "EGGCRACKER");

				public static LocString DESC = "It's little-endian.";

				public static LocString EFFECT = "Cracks open a critter egg for use as a cooking ingredient.";

				public static LocString RECIPE_DESCRIPTION = "Turns {0} into {1}.";

				public static LocString RESULT_DESCRIPTION = "Cracked {0}";
			}
		}

		public static class DAMAGESOURCES
		{
			public static LocString NOTIFICATION_TOOLTIP = "A {0} sustained damage from {1}";

			public static LocString CONDUIT_CONTENTS_FROZE = "pipe contents becoming too cold";

			public static LocString CONDUIT_CONTENTS_BOILED = "pipe contents becoming too hot";

			public static LocString BUILDING_OVERHEATED = "overheating";

			public static LocString BAD_INPUT_ELEMENT = "receiving an incorrect substance";

			public static LocString MINION_DESTRUCTION = "an angry Duplicant. Rude!";

			public static LocString LIQUID_PRESSURE = "neighboring liquid pressure";

			public static LocString CIRCUIT_OVERLOADED = "an overloaded circuit";

			public static LocString MICROMETEORITE = "micrometeorite";

			public static LocString COMET = "falling space rocks";
		}

		public static class AUTODISINFECTABLE
		{
			public static class ENABLE_AUTODISINFECT
			{
				public static LocString NAME = "Enable Disinfect";

				public static LocString TOOLTIP = "Automatically disinfect this building when it becomes contaminated";
			}

			public static class DISABLE_AUTODISINFECT
			{
				public static LocString NAME = "Disable Disinfect";

				public static LocString TOOLTIP = "Do not automatically disinfect this building";
			}

			public static class NO_DISEASE
			{
				public static LocString TOOLTIP = "This building is clean";
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

		public static class AUTOMATABLE
		{
			public static class ENABLE_AUTOMATIONONLY
			{
				public static LocString NAME = "Disable Manual";

				public static LocString TOOLTIP = "This building's storage may be accessed by Auto-Sweepers only" + UI.HORIZONTAL_BR_RULE + "Duplicants will not be permitted to add or remove materials from this building";
			}

			public static class DISABLE_AUTOMATIONONLY
			{
				public static LocString NAME = "Enable Manual";

				public static LocString TOOLTIP = "This building's storage may be accessed by both Duplicants and Auto-Sweeper buildings";
			}
		}
	}
}
