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

				public static LocString DESC = "You'll know it's on when the little flag is wiggling.";

				public static LocString EFFECT = "Cools the <style=\"gas\">Gas</style> piped through it, but outputs <style=\"heat\">Heat</style> in its immediate vicinity.";
			}

			public class ALGAEDISTILLERY
			{
				public static LocString NAME = "Bio Distiller";

				public static LocString DESC = "The resulting algae still looks pretty slimy, to be honest.";

				public static LocString EFFECT = "Converts <style=\"misc\">Slime</style> into <style=\"misc\">Algae</style>.";
			}

			public class FERTILIZERMAKER
			{
				public static LocString NAME = "Fertilizer Maker";

				public static LocString DESC = "Don't breathe too deeply.";

				public static LocString EFFECT = "Uses <style=\"liquid\">Contaminated Water</style> to produce <style=\"solid\">Fertilizer</style>.";
			}

			public class ALGAEHABITAT
			{
				public static LocString NAME = "Algae Terrarium";

				public static LocString DESC = "Algae colony. Duplicant colony.\nWe're more similar than we are different.";

				public static LocString EFFECT = "Recycles <style=\"gas\">Carbon Dioxide</style> into a small amount of <style=\"oxygen\">Oxygen</style>.\n\nGains a minor efficiency boost in direct <style=\"light\">Light</style>.";
			}

			public class BATTERY
			{
				public static LocString NAME = "Tiny Battery";

				public static LocString DESC = "It isn't very efficient to let half your energy seep into the cosmos. Fight that entropy!";

				public static LocString EFFECT = "Stores a bit of runoff <style=\"power\">Power</style> from generators, but loses charge over time.";
			}

			public class BATTERYMEDIUM
			{
				public static LocString NAME = "Battery";

				public static LocString DESC = "Wow! Apparently size does matter.";

				public static LocString EFFECT = "Stores most of the runoff <style=\"power\">Power</style> from generators, but loses charge over time.";
			}

			public class BED
			{
				public static LocString NAME = "Cot";

				public static LocString DESC = "A soft patch in a hard world.\nYour Duplicants will be grateful to have their own bed.";

				public static LocString EFFECT = "Reduces <style=\"stress\">Stress</style> by giving Duplicants a place to rest.\n\nAt night Duplicants will automatically sleep in the cot that has been assigned to them.";
			}

			public class MEDICALCOT
			{
				public static LocString NAME = "Medical Cot";

				public static LocString DESC = "Nurse Duplicants back to health after a harrowing brush with death.";

				public static LocString EFFECT = "Accelerates the regeneration of a wounded Duplicant's <style=\"Health\">Health</style>.";
			}

			public class MASSAGETABLE
			{
				public static LocString NAME = "Massage Table";

				public static LocString DESC = "The Duplicants are carrying a lot of tension in their... everything.";

				public static LocString EFFECT = "Rapidly reduces <style=\"stress\">Stress</style> for one Duplicant at a time.";
			}

			public class CEILINGLIGHT
			{
				public static LocString NAME = "Ceiling Lamp";

				public static LocString DESC = "This light imitates the Duplicants' natural aboveground habitat.\nSort of. Maybe.";

				public static LocString EFFECT = "Improves <style=\"decor\">Decor Opinion</style> and reduces <style=\"stress\">Stress</style> by providing <style=\"light\">Light</style>.";
			}

			public class AIRFILTER
			{
				public static LocString NAME = "Air Deodorizer";

				public static LocString DESC = "Oh! Citrus scented!";

				public static LocString EFFECT = "Reduces the threat of <style=\"disease\">Disease</style> by filtering <style=\"gas\">Contaminated Oxygen</style> out of the air.";
			}

			public class CANVAS
			{
				public static LocString NAME = "Blank Canvas";

				public static LocString DESC = "Hopefully your Duplicants have been practicing their color theory.";

				public static LocString EFFECT = "Increases <style=\"decor\">Decor Opinion</style> and reduces Duplicants' <style=\"stress\">Stress</style>.\n\nMust be painted by a Duplicant.";

				public static LocString POORQUALITYNAME = "Crude Painting";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Painting";

				public static LocString EXCELLENTQUALITYNAME = "Masterpiece";
			}

			public class CO2SCRUBBER
			{
				public static LocString NAME = "Air Scrubber";

				public static LocString DESC = "Everyone appreciates a little clean air.";

				public static LocString EFFECT = "Filters <style=\"gas\">Carbon Dioxide</style> and removes it from the air.";
			}

			public class COMPOST
			{
				public static LocString NAME = "Compost";

				public static LocString DESC = "Don't waste your waste!";

				public static LocString EFFECT = string.Format("Breaks <style=\"solid\">{0}</style> down into <style=\"solid\">{1}</style>.", ELEMENTS.TOXICSAND.NAME, ELEMENTS.FERTILIZER.NAME);
			}

			public class COOKINGSTATION
			{
				public static LocString NAME = "Cooking Station";

				public static LocString DESC = "Two wrongs don't make a right, but two mush bars apparently make one okay meal.";

				public static LocString EFFECT = "Converts two <style=\"food\">Mush Bars</style> into one improved <style=\"food\">Deep Fried Mush Bar</style>.\n\nDuplicants will not fabricate unless recipes are queued.";
			}

			public class DININGTABLE
			{
				public static LocString NAME = "Mess Table";

				public static LocString DESC = "The colony that eats together, excretes together.";

				public static LocString EFFECT = "Gives Duplicants a place to eat and reduces <style=\"stress\">Stress</style>.\n\nDuplicants will automatically eat at their assigned table when hungry.";
			}

			public class DOOR
			{
				public static LocString NAME = "Pneumatic Door";

				public static LocString DESC = "Makes a decent bedroom door, if you don't mind the occasional voyeur.";

				public static LocString EFFECT = "Encloses rooms without blocking the flow of <style=\"gas\">Gas</style> or <style=\"liquid\">Liquid</style>.";

				public static LocString PRESSURE_SUIT_REQUIRED = "<style=\"equipment\">Exosuit</style> required {0}";

				public static LocString PRESSURE_SUIT_NOT_REQUIRED = "<style=\"equipment\">Exosuit</style> not required {0}";

				public static LocString ABOVE = "above";

				public static LocString BELOW = "below";

				public static LocString LEFT = "on the left";

				public static LocString RIGHT = "on the right";

				public static class CONTROL_STATE
				{
					public static LocString OPEN = "Open Door";

					public static LocString CLOSE = "Close Door";

					public static LocString AUTO = "Auto";
				}
			}

			public class ELECTROLYZER
			{
				public static LocString NAME = "Electrolyzer";

				public static LocString DESC = "Water goes in one end, life-sustaining oxygen comes out the other.";

				public static LocString EFFECT = "Produces a steady supply of <style=\"oxygen\">Oxygen</style> using piped in <style=\"liquid\">Water</style>.\n\nBecomes idle when the room enters maximum air pressure range.";
			}

			public class FLOORLAMP
			{
				public static LocString NAME = "Floor Lamp";

				public static LocString DESC = "Now your Duplicants can read all those books they don't have!";

				public static LocString EFFECT = "Improves <style=\"decor\">Decor Opinion</style> and reduces <style=\"stress\">Stress</style> by providing <style=\"light\">Light</style>.";
			}

			public class FLOWERVASE
			{
				public static LocString NAME = "Flower Vase";

				public static LocString DESC = "Duplicants can't agree on how to pronounce \"vase\", but they *can* agree it looks nice.";

				public static LocString EFFECT = "Increases <style=\"decor\">Decor Opinion</style> and reduces Duplicant <style=\"stress\">Stress</style>.";
			}

			public class FLUSHTOILET
			{
				public static LocString NAME = "Lavatory";

				public static LocString DESC = "No muss, no fuss.\nThis toilet is highly functional and (almost) always clean.";

				public static LocString EFFECT = "Greatly reduces <style=\"stress\">Stress</style> and <style=\"disease\">Disease</style> by more hygienically handling Duplicant waste.";
			}

			public class SHOWER
			{
				public static LocString NAME = "Shower";

				public static LocString DESC = "Very few things in life can top a nice, hot shower.";

				public static LocString EFFECT = "Prevents <style=\"disease\">Disease</style> by improving Duplicant <style=\"hygiene\">Hygiene</style>.";
			}

			public class GASCONDUIT
			{
				public static LocString NAME = "Gas Pipe";

				public static LocString DESC = "Easily transport gas with this simple, airtight piping.";

				public static LocString EFFECT = "Transports <style=\"gas\">Gas</style> between <style=\"GasPiping\">Gas Intakes</style> and <style=\"GasPiping\">Gas Outputs</style>.\n\nCan be run through floor and wall tiles.";
			}

			public class GASCONDUITBRIDGE
			{
				public static LocString NAME = "Gas Pipe Bridge";

				public static LocString DESC = "Keep those gas pipes separated.";

				public static LocString EFFECT = "Runs one section of <style=\"GasPiping\">Gas Pipe</style> over another without joining their systems.\n\nCannot be run through floor or wall tiles.";
			}

			public class GASFILTER
			{
				public static LocString NAME = "Gas Filter";

				public static LocString DESC = "Remind those gases it's time for work, not mingling.";

				public static LocString EFFECT = "Sieves one type of <style=\"gas\">Gas</style> out from a mixed composition, sending it into a separate <style=\"GasPiping\">Pipe</style>.";
			}

			public class GASPERMEABLEMEMBRANE
			{
				public static LocString NAME = "Gas Permeable Tile";

				public static LocString DESC = "It's like walking on a dream!";

				public static LocString EFFECT = "Used as wall and floor tile to build rooms.\n\nBlocks the flow of <style=\"liquid\">Liquid</style> without obstructing the flow of <style=\"gas\">Gas</style>.";
			}

			public class GASPUMP
			{
				public static LocString NAME = "Gas Pump";

				public static LocString DESC = "Get your colony all pumped up!";

				public static LocString EFFECT = "Draws in <style=\"gas\">Gas</style> and runs it through the connected <style=\"GasPiping\">Gas Pipe</style>.";
			}

			public class GASVALVE
			{
				public static LocString NAME = "Gas Valve";

				public static LocString DESC = "It's the metal pipe equivalent of kinking a garden hose.";

				public static LocString EFFECT = "Increases or decreases <style=\"gas\">Gas</style> volume in pipes to maintain ideal <style=\"GasPiping>Gas Pipe</style> pressure.";
			}

			public class GASVENT
			{
				public static LocString NAME = "Gas Vent";

				public static LocString DESC = "Sometimes you just gotta vent.";

				public static LocString EFFECT = "An output point for <style=\"GasPiping\">Gas Pipes</style>.";
			}

			public class GENERATOR
			{
				public static LocString NAME = "Coal Generator";

				public static LocString DESC = "It may not be the most clean-burning power source, but at least it's also inefficient!";

				public static LocString EFFECT = "A <style=\"power\">Power</style> source that burns <style=\"RawMineral\">Coal</style> to generate electricity.";
			}

			public class GRAVE
			{
				public static LocString NAME = "Tasteful Memorial";

				public static LocString DESC = "A way to remember your fallen comrades... without remembering the smell.";

				public static LocString EFFECT = "Reduces the spread of <style=\"disease\">Disease</style> and <style=\"stress\">Stress</style> caused by dead Duplicants.\n\nLiving Duplicants will automatically place an unburied corpse inside.";
			}

			public class HEADQUARTERS
			{
				public static LocString NAME = "Printing Pod";

				public static LocString DESC = "Newly printed Duplicants will come out here, but thank goodness, they'll never go back in.";

				public static LocString EFFECT = "A bioprinting machine that periodically produces new Duplicants.";
			}

			public class HYDROGENGENERATOR
			{
				public static LocString NAME = "Hydrogen Generator";

				public static LocString DESC = "Hey, wait... this thing doesn't generate hydrogen at all!";

				public static LocString EFFECT = "A <style=\"power\">Power</style> source that converts <style=\"gas\">Hydrogen</style> into electricity.";
			}

			public class INSULATEDGASCONDUIT
			{
				public static LocString NAME = "Insulated Gas Pipe";

				public static LocString DESC = "Ensure your gas stays piping hot. Or cold.\nOr whatever.";

				public static LocString EFFECT = "Transports <style=\"gas\">Gas</style> between <style=\"GasPiping\">Intakes</style> and <style=\"GasPiping\">Outputs</style> with minimal change in <style=\"heat\">Temperature</style>.\n\nCan be run through floor and wall tiles.";
			}

			public class INSULATEDLIQUIDCONDUIT
			{
				public static LocString NAME = "Insulated Liquid Pipe";

				public static LocString DESC = "Ensure your liquids stay piping hot. Or cold.\nOr whatever.";

				public static LocString EFFECT = "Transports <style=\"liquid\">Liquid</style> between <style=\"LiquidPiping\">Intakes</style> and <style=\"LiquidPiping\">Outputs</style> with minimal change in <style=\"heat\">Temperature</style>.\n\nCan be run through floor and wall tiles.";
			}

			public class INSULATEDWIRE
			{
				public static LocString NAME = "Insulated Wire";

				public static LocString DESC = "This stuff won't go melting if things get heated.";

				public static LocString EFFECT = "Connects buildings to <style=\"power\">Power</style> sources in extreme <style=\"heat\">Heat</style>.\n\nCan be run through wall and floor tiles.";
			}

			public class INSULATIONTILE
			{
				public static LocString NAME = "Insulated Tile";

				public static LocString DESC = "Gives your Duplicants nice, toasty feet.\nPlus it's purple!";

				public static LocString EFFECT = "Used as wall and floor tile to build rooms.\n\nReduces <style=\"heat\">Heat</style> transfer between walls.";
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

				public static LocString DESC = "Shag rugs are out. Farm floors are all the rage now!";

				public static LocString EFFECT = "Grows a single <style=\"plant\">Plant</style> when sown with a <style=\"seed\">Seed</style>.\n\nCan be used as floor tile.";
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

				public static LocString DESC = "Easily transport liquid with this simple, airtight piping.";

				public static LocString EFFECT = "Transports <style=\"liquid\">Liquid</style> between <style=\"LiquidPiping\">Liquid Intakes</style> and <style=\"LiquidPiping\">Liquid Outputs</style>.\n\nCan be run through floor and wall tiles.";
			}

			public class LIQUIDCONDUITBRIDGE
			{
				public static LocString NAME = "Liquid Pipe Bridge";

				public static LocString DESC = "A pipe bridge for making sure your streams don't cross.";

				public static LocString EFFECT = "Runs one section of <style=\"LiquidPiping\">Liquid Pipe</style> overtop another without joining their systems.\n\nCannot be run through wall or floor tiles.";
			}

			public class LIQUIDCOOLEDFAN
			{
				public static LocString NAME = "Hydrofan";

				public static LocString DESC = "It's almost like a real breeze.\nHow refreshing!";

				public static LocString EFFECT = "Dissipates a small amount of the <style=\"heat\">Heat</style>.";
			}

			public class LIQUIDFILTER
			{
				public static LocString NAME = "Liquid Filter";

				public static LocString DESC = "Ever tried separating liquids with your bare hands?\nVery difficult!";

				public static LocString EFFECT = "Can sieve one selected <style=\"liquid\">Liquid</style> out of a liquid mixture, sending it into a separate <style=\"LiquidPiping\">Pipe</style>.";
			}

			public class LIQUIDPUMP
			{
				public static LocString NAME = "Liquid Pump";

				public static LocString DESC = "Get your colony all pumped up!";

				public static LocString EFFECT = "Draws in <style=\"liquid\">Liquid</style> and runs it through the connected <style=\"LiquidPiping\">Liquid Pipe</style>.\n\nMust be submerged in liquid to function.";
			}

			public class LIQUIDRESERVOIR
			{
				public static LocString NAME = "Reservoir";

				public static LocString DESC = "Store your colony's liquids for those not-so-rainy days.";

				public static LocString EFFECT = "Manually accepts <style=\"liquid\">Liquid</style> and moves it through the connected <style=\"LiquidPiping\">Pipe</style>.";
			}

			public class LIQUIDVALVE
			{
				public static LocString NAME = "Liquid Valve";

				public static LocString DESC = "It's the metal pipe equivalent of kinking a garden hose.";

				public static LocString EFFECT = "Increases or decreases <style=\"liquid\">Liquid</style> volume in pipes to maintain ideal <style=\"LiquidPiping>Liquid Pipe</style> pressure.";
			}

			public class LIQUIDVENT
			{
				public static LocString NAME = "Liquid Vent";

				public static LocString DESC = "Sometimes you just gotta vent.";

				public static LocString EFFECT = "An output point for <style=\"LiquidPiping\">Liquid Pipes</style>.";
			}

			public class MANUALGENERATOR
			{
				public static LocString NAME = "Manual Generator";

				public static LocString DESC = "Watching Duplicants run on it is adorable... the electrical power is just an added bonus.";

				public static LocString EFFECT = "A <style=\"power\">Power</style> source that must be manually operated to produce electricity.";
			}

			public class MANUALPRESSUREDOOR
			{
				public static LocString NAME = "Manual Airlock";

				public static LocString DESC = "No gas escapes these airlocks, yet oddly you can smell what's on the other side.";

				public static LocString EFFECT = "Creates an airtight seal that maintains atmosphere and pressure between areas.";
			}

			public class MEDICALBED
			{
				public static LocString NAME = "Rejuvenator";

				public static LocString DESC = "Stick a Duplicant into this state of the art triage tube and they'll pop out, good as new.";

				public static LocString EFFECT = "Greatly accelerates the regeneration of a wounded Duplicant's <style=\"Health\">Health</style>.";
			}

			public class MESHTILE
			{
				public static LocString NAME = "Mesh Tile";

				public static LocString DESC = "How do Duplicants keep their tiny feet from slipping through the holes?";

				public static LocString EFFECT = "Can be used as wall or floor tile to enclose rooms.\n\nDoes not obstruct the flow of <style=\"liquid\">Liquid</style> or <style=\"gas\">Gas</style>.";
			}

			public class MICROBEMUSHER
			{
				public static LocString NAME = "Microbe Musher";

				public static LocString DESC = "Apparently Duplicants can glean some nutrition from the various goops this outputs.";

				public static LocString EFFECT = "Produces low quality <style=\"food\">Food</style> for Duplicants using common ingredients.\n\nDuplicants will not fabricate unless recipes are queued.";
			}

			public class MINERALDEOXIDIZER
			{
				public static LocString NAME = "Algae Deoxydizer";

				public static LocString DESC = "Breathe easy knowing you've crushed a bunch of helpless algae into oblivion.";

				public static LocString EFFECT = "Converts <style=\"misc\">Algae</style> into <style=\"oxygen\">Oxygen</style>.\n\nBecomes idle when the room enters maximum air pressure range.";
			}

			public class MULTITOOLWORKBENCH
			{
				public static LocString NAME = "Crafting Station";

				public static LocString DESC = "Make your Duplicants faster, stronger... better!";

				public static LocString EFFECT = "Uses <style=\"RawMetal\">Raw Metal</style> or <style=\"RefinedMetal\">Refined Metal</style> to craft <style=\"equipment\">Multitools</style>.\n\nDuplicants will not fabricate unless recipes are queued.";
			}

			public class OUTHOUSE
			{
				public static LocString NAME = "Outhouse";

				public static LocString DESC = "Outhouses keep Duplicants from getting down in the dumps.\nNot building any'd be a real waste!";

				public static LocString EFFECT = string.Format("Reduces <style=\"stress\">Stress</style> and the spread of <style=\"disease\">Disease</style> by regulating where Duplicants relieve themselves.\n\nRequires no <style=\"GasPiping\">Piping</style>.\n\nMust be emptied of <style=\"solid\">{0}</style> periodically.", ELEMENTS.TOXICSAND.NAME);
			}

			public class APOTHECARY
			{
				public static LocString NAME = "Apothecary";

				public static LocString DESC = "All-natural remedies to cure what ails you.";

				public static LocString EFFECT = "Cures most Duplicant <style=\"disease\">Diseases</style> by producing basic <style=\"medicine\">Medicines</style>.\n\nDuplicants will not fabricate unless recipes are queued.";
			}

			public class PLANTERBOX
			{
				public static LocString NAME = "Planter Box";

				public static LocString DESC = "Think inside the box.";

				public static LocString EFFECT = "Houses a single <style=\"plant\">Plant</style> which will periodically produce <style=\"food\">Food</style> or <style=\"medicine\">Medicine</style>.";
			}

			public class PRESSUREDOOR
			{
				public static LocString NAME = "Mechanized Airlock";

				public static LocString DESC = "Is there anything more thrilling than the automation of manual processes?";

				public static LocString EFFECT = "Creates an airtight seal that maintains atmosphere and pressure between areas.\n\nFunctions as a Manual Airlock when no <style=\"power\">Power</style> is available.";
			}

			public class RATIONBOX
			{
				public static LocString NAME = "Ration Box";

				public static LocString DESC = "Storing food in a box?\nSeems rational!";

				public static LocString EFFECT = "Stores a small amount of <style=\"food\">Food</style>.";
			}

			public class REFRIGERATOR
			{
				public static LocString NAME = "Refrigerator";

				public static LocString DESC = "Spoil your Duplicants with this hi-tech, spoilage-free food box.";

				public static LocString EFFECT = "Drastically increases the shelf life of <style=\"food\">Food</style> by storing it at an ideal <style=\"heat\">Temperature</style>.";
			}

			public class RESEARCHCENTER
			{
				public static LocString NAME = "Research Station";

				public static LocString DESC = "Duplicants' first chemistry set.";

				public static LocString EFFECT = "Can be worked by Duplicants to conduct <style=\"research\">Novice Research</style> and unlock new technologies.";
			}

			public class ADVANCEDRESEARCHCENTER
			{
				public static LocString NAME = "Super Computer";

				public static LocString DESC = "Every additional monitor multiplies the research potential exponentially.";

				public static LocString EFFECT = "Can be worked by Duplicants to conduct <style=\"research\">Intermediary Research</style> and unlock new technologies.";
			}

			public class SCULPTURE
			{
				public static LocString NAME = "Sculpting Block";

				public static LocString DESC = "An object of questionable form and no discernible function.\nThe Duplicants love it.";

				public static LocString EFFECT = "Increases <style=\"decor\">Decor Opinion</style> and reduces Duplicants' <style=\"stress\">Stress</style>.\n\nMust be sculpted by a Duplicant.";

				public static LocString POORQUALITYNAME = "\"Abstract\" Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Sculpture";
			}

			public class SMELTER
			{
				public static LocString NAME = "Smelter";

				public static LocString DESC = "Whoever smelt it, dealt it!\nOh, goodness, that was a vulgar crack.";

				public static LocString EFFECT = "Converts <style=\"RawMetal\">Raw Metal</style> into <style=\"RefinedMetal\">Refined Metal</style>.\n\nDuplicants will not fabricate unless recipes are queued.";
			}

			public class SPACEHEATER
			{
				public static LocString NAME = "Space Heater";

				public static LocString DESC = "Stave off frostbite with this heartwarming doodad.\n(Warms other body parts too!)";

				public static LocString EFFECT = "Produces a moderate amount of <style=\"heat\">Heat</style> in its immediate vicinity.";
			}

			public class STORAGELOCKER
			{
				public static LocString NAME = "Storage Compactor";

				public static LocString DESC = "Keep your clutter off the floor!";

				public static LocString EFFECT = "Stores the resources of your choosing.";
			}

			public class SUITFABRICATOR
			{
				public static LocString NAME = "Exosuit Station";

				public static LocString DESC = "Exosuits allow Duplicants to breathe in the harshest of environments.\nThey're also flatteringly form-fitting.";

				public static LocString EFFECT = "Uses <style=\"RawMetal\">Raw Metal</style> or <style=\"RefinedMetal\">Refined Metal</style> to craft protective <style=\"equipment\">Exosuits</style> for Duplicants.\n\nDuplicants will not fabricate unless recipes are queued.";
			}

			public class SUITRECHARGER
			{
				public static LocString NAME = "Exosuit Recharger";

				public static LocString DESC = "Keep the colony's exosuits in tip-top shape with this oxygen refilling station...\nUnless you'd prefer to see your Duplicants suffocate.";

				public static LocString EFFECT = "Refills a depleted <style=\"equipment\">Exosuit</style> with <style=\"oxygen\">Oxygen</style>.";
			}

			public class SWITCH
			{
				public static LocString NAME = "Power Switch";

				public static LocString DESC = "Put the power right in your Duplicants' cute little hands.";

				public static LocString EFFECT = "Toggles all circuity following the switch without affecting the preceding circuitry.";
			}

			public class TILE
			{
				public static LocString NAME = "Tile";

				public static LocString DESC = "Duplicants find these tiles easier on the feetsies than jagged bedrock.";

				public static LocString EFFECT = "Used as wall and floor tile to build rooms.";
			}

			public class WALLLIGHT
			{
				public static LocString NAME = "Lamp";

				public static LocString DESC = "The light helps imitate the Duplicants' natural aboveground habitat.\nSort of. Maybe.";

				public static LocString EFFECT = "Improves <style=\"decor\">Decor Opinion</style> and reduces <style=\"stress\">Stress</style> by providing <style=\"light\">Light</style>.";
			}

			public class WATERPURIFIER
			{
				public static LocString NAME = "Water Purifier";

				public static LocString DESC = "You know it's just going to get dirty again.";

				public static LocString EFFECT = "Uses <style=\"misc\">Sand</style> to purify <style=\"liquid\">Contaminated Water</style>.";
			}

			public class DISTILLATIONCOLUMN
			{
				public static LocString NAME = "Distillation Column";

				public static LocString DESC = "Gets hot and steamy.";

				public static LocString EFFECT = string.Format("Separates any <style=\"liquid\">{0}</style> piped through it into <style=\"gas\">Steam</style> and <style=\"solid\">{1}</style>.", ELEMENTS.DIRTYWATER.NAME, ELEMENTS.TOXICSAND.NAME);
			}

			public class WIRE
			{
				public static LocString NAME = "Electrical Wire";

				public static LocString DESC = "A sleek, easy way of delivering electricity right where it needs to go.";

				public static LocString EFFECT = "Connects buildings to <style=\"power\">Power</style> sources.\n\nCan be run through wall and floor tiles.";
			}

			public class WIREBRIDGE
			{
				public static LocString NAME = "Wire Bridge";

				public static LocString DESC = "Two wires, running so close and yet never crossing paths.";

				public static LocString EFFECT = "Runs one section of wire overtop another without joining the <style=\"power\">Power</style> grids.\n\nCan be run through wall and floor tiles.";
			}

			public class HANDSANITIZER
			{
				public static LocString NAME = "Hand Sanitizer";

				public static LocString DESC = "The only thing filthy hands are good for is picking up nasty diseases.";

				public static LocString EFFECT = "Produces a sanitizing gel to remove the <style=\"disease\">Dirty Hands</style> effect from Duplicants.\n\nDuplicants will only use the Hand Sanitizer before eating a meal.";
			}
		}
	}
}
