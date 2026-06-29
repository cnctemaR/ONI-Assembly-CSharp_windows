using System;

namespace STRINGS
{
	public class CREATURES
	{
		public static LocString BAGGED_NAME_FMT = "Bagged {0}";

		public static LocString BAGGED_DESC_FMT = "This {0} has been captured and is now safe to relocate.";

		public class SPECIES
		{
			public class CHLORINEGEYSER
			{
				public static LocString NAME = UI.FormatAsLink("Chlorine Geyser", "GeyserGeneric_CHLORINE_GAS");

				public static LocString DESC = "A highly pressurized geyser that periodically erupts with " + ELEMENTS.CHLORINEGAS.NAME + ".";
			}

			public class FLUT
			{
				public static LocString NAME = UI.FormatAsLink("Pacu", "FLUT");

				public static LocString DESC = "Every organism in the known universe finds the Pacu extremely delicious.";
			}

			public class GLOM
			{
				public static LocString NAME = UI.FormatAsLink("Morb", "GLOM");

				public static LocString DESC = "Morbs are attracted to decomposing corpses and frequently excrete bursts of " + ELEMENTS.CONTAMINATEDOXYGEN.NAME + ".";

				public static LocString EGG_NAME = UI.FormatAsLink("Morb Pod", "MORBEGG");
			}

			public class HATCH
			{
				public static LocString NAME = UI.FormatAsLink("Hatch", "HATCH");

				public static LocString DESC = "Hatches excrete solid " + ELEMENTS.CARBON.NAME + " as waste and may be uncovered by digging up Buried Objects.";

				public static LocString EGG_NAME = UI.FormatAsLink("Hatchling Egg", "HATCHEGG");
			}

			public class OILFLOATER
			{
				public static LocString NAME = UI.FormatAsLink("Slickster", "OILFLOATER");

				public static LocString DESC = string.Concat(new string[]
				{
					"Slicksters are slimy critters that consume ",
					ELEMENTS.CARBONDIOXIDE.NAME,
					" and exude ",
					ELEMENTS.CRUDEOIL.NAME,
					"."
				});

				public static LocString EGG_NAME = UI.FormatAsLink("Slickster Larva Egg", "OILFLOATEREGG");
			}

			public class PUFT
			{
				public static LocString NAME = UI.FormatAsLink("Puft", "PUFT");

				public static LocString DESC = "Pufts are non-aggressive critters that excrete lumps of " + ELEMENTS.SLIMEMOLD.NAME + " with each breath.";

				public static LocString EGG_NAME = UI.FormatAsLink("Puftlet Egg", "PUFTEGG");
			}

			public class GREEDYGREEN
			{
				public static LocString NAME = UI.FormatAsLink("Avari Vine", "GREEDYGREEN");

				public static LocString DESC = "A rapidly growing, subterranean " + UI.FormatAsLink("Plant", "PLANTS") + ".";
			}

			public class SHOCKWORM
			{
				public static LocString NAME = UI.FormatAsLink("Shockworm", "SHOCKWORM");

				public static LocString DESC = "Shockworms are exceptionally aggressive and discharge electrical shocks to stun their prey.";
			}

			public class LIGHTBUG
			{
				public static LocString NAME = UI.FormatAsLink("Shine Bug", "LIGHTBUG");

				public static LocString DESC = "Shine Bugs emit a soft " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.";

				public static LocString EGG_NAME = UI.FormatAsLink("Shine Nymph Egg", "LIGHTBUGEGG");
			}

			public class GEYSER
			{
				public static LocString NAME = UI.FormatAsLink("Steam Geyser", "GEYSER");

				public static LocString DESC = string.Concat(new string[]
				{
					"A highly pressurized geyser that periodically erupts, spraying ",
					ELEMENTS.STEAM.NAME,
					" and boiling hot ",
					ELEMENTS.WATER.NAME,
					"."
				});

				public class STEAM
				{
					public static LocString NAME = UI.FormatAsLink("Cool Steam Vent", "GeyserGeneric_STEAM");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with hot " + ELEMENTS.STEAM.NAME + ".";
				}

				public class HOT_STEAM
				{
					public static LocString NAME = UI.FormatAsLink("Steam Vent", "GeyserGeneric_HOT_STEAM");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with scalding " + ELEMENTS.STEAM.NAME + ".";
				}

				public class HOT_WATER
				{
					public static LocString NAME = UI.FormatAsLink("Water Geyser", "GeyserGeneric_HOT_WATER");

					public static LocString DESC = "A highly pressurized geyser that periodically erupts with hot " + ELEMENTS.WATER.NAME + ".";
				}

				public class SLUSH_WATER
				{
					public static LocString NAME = UI.FormatAsLink("Cool Slush Geyser", "GeyserGeneric_SLUSHWATER");

					public static LocString DESC = "A highly pressurized geyser that periodically erupts with freezing " + ELEMENTS.CRUSHEDICE.NAME + ".";
				}

				public class FILTHY_WATER
				{
					public static LocString NAME = UI.FormatAsLink("Polluted Water Vent", "GeyserGeneric_FILTHYWATER");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with boiling " + ELEMENTS.DIRTYWATER.NAME + ".";
				}

				public class SMALL_VOLCANO
				{
					public static LocString NAME = UI.FormatAsLink("Minor Volcano", "GeyserGeneric_SMALL_VOLCANO");

					public static LocString DESC = "A miniature volcano that periodically erupts with molten " + ELEMENTS.MAGMA.NAME + ".";
				}

				public class BIG_VOLCANO
				{
					public static LocString NAME = UI.FormatAsLink("Volcano", "GeyserGeneric_BIG_VOLCANO");

					public static LocString DESC = "A massive volcano that periodically erupts with molten " + ELEMENTS.MAGMA.NAME + ".";
				}

				public class LIQUID_CO2
				{
					public static LocString NAME = UI.FormatAsLink("Carbon Dioxide Geyser", "GeyserGeneric_LIQUID_CO2");

					public static LocString DESC = "A highly pressurized geyser that periodically erupts with boiling liquid " + ELEMENTS.LIQUIDCARBONDIOXIDE.NAME + ".";
				}

				public class HOT_CO2
				{
					public static LocString NAME = UI.FormatAsLink("Carbon Dioxide Vent", "GeyserGeneric_HOT_CO2");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with hot gaseous " + ELEMENTS.CARBONDIOXIDE.NAME + ".";
				}

				public class HOT_HYDROGEN
				{
					public static LocString NAME = UI.FormatAsLink("Hydrogen Vent", "GeyserGeneric_HOT_HYDROGEN");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with hot gaseous " + ELEMENTS.HYDROGEN.NAME + ".";
				}

				public class HOT_PO2
				{
					public static LocString NAME = UI.FormatAsLink("Hot Polluted Oxygen Vent", "GeyserGeneric_HOT_PO2");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with hot " + ELEMENTS.CONTAMINATEDOXYGEN.NAME + ".";
				}

				public class SLIMY_PO2
				{
					public static LocString NAME = UI.FormatAsLink("Infectious Polluted Oxygen Vent", "GeyserGeneric_SLIMY_PO2");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with warm " + ELEMENTS.CONTAMINATEDOXYGEN.NAME + ".";
				}

				public class CHLORINE_GAS
				{
					public static LocString NAME = UI.FormatAsLink("Chlorine Gas Vent", "GeyserGeneric_CHLORINE_GAS");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with warm " + ELEMENTS.CHLORINEGAS.NAME + ".";
				}

				public class METHANE
				{
					public static LocString NAME = UI.FormatAsLink("Natural Gas Geyser", "GeyserGeneric_METHANE");

					public static LocString DESC = "A highly pressurized geyser that periodically erupts with hot " + ELEMENTS.METHANE.NAME + ".";
				}

				public class MOLTEN_COPPER
				{
					public static LocString NAME = UI.FormatAsLink("Copper Volcano", "GeyserGeneric_MOLTEN_COPPER");

					public static LocString DESC = "A large volcano that periodically erupts with molten " + ELEMENTS.MOLTENCOPPER.NAME + ".";
				}

				public class MOLTEN_IRON
				{
					public static LocString NAME = UI.FormatAsLink("Iron Volcano", "GeyserGeneric_MOLTEN_IRON");

					public static LocString DESC = "A large volcano that periodically erupts with molten " + ELEMENTS.MOLTENIRON.NAME + ".";
				}

				public class MOLTEN_GOLD
				{
					public static LocString NAME = UI.FormatAsLink("Gold Volcano", "GeyserGeneric_MOLTEN_GOLD");

					public static LocString DESC = "A large volcano that periodically erupts with molten " + ELEMENTS.MOLTENGOLD.NAME + ".";
				}

				public class OIL_DRIP
				{
					public static LocString NAME = UI.FormatAsLink("Leaky Oil Fissure", "GeyserGeneric_OIL_DRIP");

					public static LocString DESC = "A fissure in the asteroid that periodically erupts with boiling " + ELEMENTS.CRUDEOIL.NAME + ".";
				}
			}

			public class METHANEGEYSER
			{
				public static LocString NAME = UI.FormatAsLink("Natural Gas Geyser", "GeyserGeneric_METHANEGEYSER");

				public static LocString DESC = "A highly pressurized geyser that periodically erupts with " + ELEMENTS.METHANE.NAME + ".";
			}

			public class OIL_WELL
			{
				public static LocString NAME = UI.FormatAsLink("Oil Reservoir", "OIL_WELL");

				public static LocString DESC = "Oil Reservoirs are rock formations with " + ELEMENTS.CRUDEOIL.NAME + " deposits beneath their surface.\n\nOil can be extracted from a reservoir with sufficient pressure.";
			}

			public class MUSHROOMPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Dusk Cap", "MUSHROOMPLANT");

				public static LocString DESC = string.Concat(new string[]
				{
					"Dusk Caps produce ",
					ITEMS.FOOD.MUSHROOM.NAME,
					", fungal growths that can be harvested for ",
					UI.FormatAsLink("Food", "FOOD"),
					"."
				});

				public static LocString DOMESTICATEDDESC = "This plant produces edible " + UI.FormatAsLink("Mushrooms", UI.StripLinkFormatting(ITEMS.FOOD.MUSHROOM.NAME)) + ".";
			}

			public class STEAMSPOUT
			{
				public static LocString NAME = UI.FormatAsLink("Steam Spout", "GEYSERS");

				public static LocString DESC = "A rocky vent that spouts " + ELEMENTS.STEAM.NAME + ".";
			}

			public class PROPANESPOUT
			{
				public static LocString NAME = UI.FormatAsLink("Propane Spout", "GEYSERS");

				public static LocString DESC = "A rocky vent that spouts " + ELEMENTS.PROPANE.NAME + ".";
			}

			public class OILSPOUT
			{
				public static LocString NAME = UI.FormatAsLink("Oil Spout", "OILSPOUT");

				public static LocString DESC = "A rocky vent that spouts crude oil.";
			}

			public class HEATBULB
			{
				public static LocString NAME = UI.FormatAsLink("Fervine", "HEATBULB");

				public static LocString DESC = "A temperature reactive, subterranean " + UI.FormatAsLink("Plant", "PLANTS") + ".";
			}

			public class HEATBULBSEED
			{
				public static LocString NAME = UI.FormatAsLink("Fervine Bulb", "HEATBULBSEED");

				public static LocString DESC = "A temperature reactive, subterranean " + UI.FormatAsLink("Plant", "PLANTS") + ".";
			}

			public class FLUTEGG
			{
				public static LocString NAME = UI.FormatAsLink("Pacu Egg", "FLUTEGG");

				public static LocString DESC = "A tiny Pacu is nestled inside.\n\nIt is not yet ready for the world.";
			}

			public class MYSTERYEGG
			{
				public static LocString NAME = UI.FormatAsLink("Mysterious Egg", "MYSTERYEGG");

				public static LocString DESC = "What's growing inside? Something nice? Something mean?";
			}

			public class SWAMPLILY
			{
				public static LocString NAME = UI.FormatAsLink("Balm Lily", "SWAMPLILY");

				public static LocString DESC = "Balm Lilies produce " + ITEMS.INGREDIENTS.SWAMPLILYFLOWER.NAME + ", a lovely bloom with medicinal properties.";

				public static LocString DOMESTICATEDDESC = "This plant produces medicinal " + ITEMS.INGREDIENTS.SWAMPLILYFLOWER.NAME + ".";
			}

			public class JUNGLEGASPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Palmera Tree", "JUNGLEGASPLANT");

				public static LocString DESC = "A large, chlorine-dwelling " + UI.FormatAsLink("Plant", "PLANTS") + " that can be grown in farm buildings.\n\nPalmeras grow inedible buds that emit toxic hydrogen gas.";

				public static LocString DOMESTICATEDDESC = "A large, chlorine-dwelling " + UI.FormatAsLink("Plant", "PLANTS") + " that grows inedible buds which emit toxic hydrogen gas.";
			}

			public class PRICKLEFLOWER
			{
				public static LocString NAME = UI.FormatAsLink("Bristle Blossom", "PRICKLEFLOWER");

				public static LocString DESC = "Bristle Blossoms produce " + ITEMS.FOOD.PRICKLEFRUIT.NAME + ", a prickly edible bud.";

				public static LocString DOMESTICATEDDESC = "This plant produces edible " + UI.FormatAsLink("Bristle Berries", UI.StripLinkFormatting(ITEMS.FOOD.PRICKLEFRUIT.NAME)) + ".";
			}

			public class COLDWHEAT
			{
				public static LocString NAME = UI.FormatAsLink("Sleet Wheat", "COLDWHEAT");

				public static LocString DESC = string.Concat(new string[]
				{
					"Sleet Wheat produces ",
					ITEMS.FOOD.COLDWHEATSEED.NAME,
					", a chilly grain that can be processed into ",
					UI.FormatAsLink("Food", "FOOD"),
					"."
				});

				public static LocString DOMESTICATEDDESC = "This plant produces edible " + ITEMS.FOOD.COLDWHEATSEED.NAME + ".";
			}

			public class PRICKLEGRASS
			{
				public static LocString NAME = UI.FormatAsLink("Bluff Briar", "PRICKLEGRASS");

				public static LocString DESC = "Bluff Briars exude pheromones causing critters to view them as especially beautiful.";

				public static LocString DOMESTICATEDDESC = "This plant improves " + UI.FormatAsLink("Decor", "DECOR") + ".";
			}

			public class BASICSINGLEHARVESTPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Mealwood", "BASICSINGLEHARVESTPLANT");

				public static LocString DESC = string.Concat(new string[]
				{
					"Mealwoods produce ",
					ITEMS.FOOD.BASICPLANTFOOD.NAME,
					", an oddly wriggly grain that can be harvested for ",
					UI.FormatAsLink("Food", "FOOD"),
					"."
				});

				public static LocString DOMESTICATEDDESC = "This plant produces edible " + ITEMS.FOOD.BASICPLANTFOOD.NAME + ".";
			}

			public class BASICFABRICMATERIALPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Thimble Reed", "BASICFABRICMATERIALPLANT");

				public static LocString DESC = string.Concat(new string[]
				{
					"Thimble Reeds produce indescribably soft ",
					ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.NAME,
					" for ",
					UI.FormatAsLink("Clothing", "EQUIPMENT"),
					" production."
				});

				public static LocString DOMESTICATEDDESC = "This plant produces " + ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.NAME + ".";
			}

			public class BASICFORAGEPLANTPLANTED
			{
				public static LocString NAME = UI.FormatAsLink("Buried Muckroot", "BASICFORAGEPLANTPLANTED");

				public static LocString DESC = "Muckroots are incapable of propagating but can be harvested for a single " + UI.FormatAsLink("Food", "FOOD") + " serving.";
			}

			public class COLDBREATHER
			{
				public static LocString NAME = UI.FormatAsLink("Wheezewort", "COLDBREATHER");

				public static LocString DESC = "Wheezeworts can be grown in flower pots and absorb " + UI.FormatAsLink("Heat", "Heat") + " by respiring through their porous outer membranes.";

				public static LocString DOMESTICATEDDESC = "This plant absorbs " + UI.FormatAsLink("Heat", "Heat") + ".";
			}

			public class SPICE_VINE
			{
				public static LocString NAME = UI.FormatAsLink("Pincha Pepperplant", "SPICE_VINE");

				public static LocString DESC = string.Concat(new string[]
				{
					"Pincha Pepperplants produce flavorful ",
					ITEMS.FOOD.SPICENUT.NAME,
					" for spicing ",
					UI.FormatAsLink("Food", "FOOD"),
					"."
				});

				public static LocString DOMESTICATEDDESC = "This plant produces " + ITEMS.FOOD.SPICENUT.NAME + " spices.";
			}

			public class SEEDS
			{
				public class JUNGLEGASPLANT
				{
				}

				public class PRICKLEFLOWER
				{
					public static LocString NAME = UI.FormatAsLink("Blossom Seed", "PRICKLEFLOWER");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						UI.FormatAsLink("Bristle Blossom", "PRICKLEFLOWER"),
						".\n\nDigging up Buried Objects may uncover a Blossom Seed."
					});
				}

				public class MUSHROOMPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Fungal Spore", "MUSHROOMPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.MUSHROOMPLANT.NAME,
						".\n\nDigging up Buried Objects may uncover a Fungal Spore."
					});
				}

				public class COLDWHEAT
				{
					public static LocString NAME = UI.FormatAsLink("Sleet Wheat Grain", "COLDWHEAT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("seed", "Seeds"),
						" of a ",
						CREATURES.SPECIES.COLDWHEAT.NAME,
						" plant.\n\nGrain can be sown to cultivate more Sleet Wheat, or processed into ",
						UI.FormatAsLink("Food", "FOOD"),
						"."
					});
				}

				public class PRICKLEGRASS
				{
					public static LocString NAME = UI.FormatAsLink("Briar Seed", "PRICKLEGRASS");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.PRICKLEGRASS.NAME,
						".\n\nDigging up Buried Objects may uncover a Briar Seed."
					});
				}

				public class SWAMPLILY
				{
					public static LocString NAME = UI.FormatAsLink("Balm Lily Seed", "SWAMPLILY");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.SWAMPLILY.NAME,
						".\n\nDigging up Buried Objects may uncover a Balm Lily Seed."
					});
				}

				public class BASICSINGLEHARVESTPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Mealwood Seed", "BASICSINGLEHARVESTPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.NAME,
						".\n\nDigging up Buried Objects may uncover a Mealwood Seed."
					});
				}

				public class COLDBREATHER
				{
					public static LocString NAME = UI.FormatAsLink("Wort Seed", "COLDBREATHER");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.COLDBREATHER.NAME,
						".\n\nDigging up Buried Objects may uncover a Wort Seed."
					});
				}

				public class BASICFABRICMATERIALPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Thimble Reed Seed", "BASICFABRICMATERIALPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.BASICFABRICMATERIALPLANT.NAME,
						".\n\nDigging up Buried Objects may uncover a Thimble Reed Seed."
					});
				}

				public class SPICE_VINE
				{
					public static LocString NAME = UI.FormatAsLink("Pincha Pepper Seed", "SPICE_VINE");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.SPICE_VINE.NAME,
						".\n\nDigging up Buried Objects may uncover a Pincha Pepper Seed."
					});
				}

				public class OILEATER
				{
					public static LocString NAME = UI.FormatAsLink("Ink Bloom Seed", "OILEATER");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						UI.FormatAsLink("plant", "Ink Bloom"),
						".\n\nDigging up Buried Objects may uncover an Ink Bloom Seed."
					});
				}
			}
		}

		public class STATUSITEMS
		{
			public static LocString NAME_NON_GROWING_PLANT = "Wilted";

			public class SLEEPING
			{
				public static LocString NAME = "Sleeping";

				public static LocString TOOLTIP = "This critter is replenishing its stamina";
			}

			public class HOT
			{
				public static LocString NAME = "Toasty surroundings";

				public static LocString TOOLTIP = "This critter cannot let off enough heat to keep cool in this environment\n\nIt prefers temperatures between {0} and {1}";
			}

			public class COLD
			{
				public static LocString NAME = "Chilly surroundings";

				public static LocString TOOLTIP = "This critter cannot retain enough heat to stay warm in this environment\n\nIt prefers temperatures between {0} and {1}";
			}

			public class CROP_TOO_DARK
			{
				public static LocString NAME = "    • " + CREATURES.STATS.ILLUMINATION.NAME;

				public static LocString TOOLTIP = "Growth will resume when light requirements are met";
			}

			public class CROP_TOO_BRIGHT
			{
				public static LocString NAME = "    • " + CREATURES.STATS.ILLUMINATION.NAME;

				public static LocString TOOLTIP = "Growth will resume when darkness requirements are met";
			}

			public class HOT_CROP
			{
				public static LocString NAME = "    • " + DUPLICANTS.STATS.TEMPERATURE.NAME;

				public static LocString TOOLTIP = "Growth will resume when ambient temperature is between {low_temperature} and {high_temperature}";
			}

			public class COLD_CROP
			{
				public static LocString NAME = "    • " + DUPLICANTS.STATS.TEMPERATURE.NAME;

				public static LocString TOOLTIP = "Growth will resume when ambient temperature is between {low_temperature} and {high_temperature}";
			}

			public class PERFECTTEMPERATURE
			{
				public static LocString NAME = "Ideal Temperature";

				public static LocString TOOLTIP = "This critter finds the current ambient temperature comfortable\n\nIdeal Range: {0} - {1}";
			}

			public class EATING
			{
				public static LocString NAME = "Eating";

				public static LocString TOOLTIP = "This critter found something tasty";
			}

			public class DIGESTING
			{
				public static LocString NAME = "Digesting";

				public static LocString TOOLTIP = "This critter is working off a big meal";
			}

			public class COOLING
			{
				public static LocString NAME = "Chilly Breath";

				public static LocString TOOLTIP = "This critter's respiration is having a cooling effect on the area";
			}

			public class LOOKINGFORFOOD
			{
				public static LocString NAME = "Foraging";

				public static LocString TOOLTIP = "This critter is hungry and looking for food";
			}

			public class IDLE
			{
				public static LocString NAME = "Idle";

				public static LocString TOOLTIP = "Just enjoying life, y'know?";
			}

			public class EXCITED_TO_GET_RANCHED
			{
				public static LocString NAME = "Excited";

				public static LocString TOOLTIP = "This critter heard a Rancher call for it and is very excited!";
			}

			public class GETTING_RANCHED
			{
				public static LocString NAME = "Being Groomed";

				public static LocString TOOLTIP = "This critter's going to look so good when they're done";
			}

			public class EXCITED_TO_BE_RANCHED
			{
				public static LocString NAME = "Freshly Groomed";

				public static LocString TOOLTIP = "This critter just received some attention and feels great";
			}

			public class GETTING_WRANGLED
			{
				public static LocString NAME = "Being Wrangled";

				public static LocString TOOLTIP = "Someone's trying to capture this critter!";
			}

			public class HYPOTHERMIA
			{
				public static LocString NAME = "Freezing";

				public static LocString TOOLTIP = "Internal temperature is dangerously low";
			}

			public class SCALDING
			{
				public static LocString NAME = "Scalding";

				public static LocString TOOLTIP = "Current external temperature is perilously high [{ExternalTemperature} / {TargetTemperature}]";

				public static LocString NOTIFICATION_NAME = "Scalding";

				public static LocString NOTIFICATION_TOOLTIP = "Scalding temperatures are hurting these Duplicants:";
			}

			public class HYPERTHERMIA
			{
				public static LocString NAME = "Overheating";

				public static LocString TOOLTIP = "Internal temperature is dangerously high [{InternalTemperature} / {TargetTemperature}]";
			}

			public class TIRED
			{
				public static LocString NAME = "Fatigued";

				public static LocString TOOLTIP = "This critter needs some sleepytime";
			}

			public class BREATH
			{
				public static LocString NAME = "Suffocating";

				public static LocString TOOLTIP = "This critter is about to suffocate";
			}

			public class DEAD
			{
				public static LocString NAME = "Dead";

				public static LocString TOOLTIP = "This critter won't be getting back up...";
			}

			public class PLANTDEATH
			{
				public static LocString NAME = "Dead";

				public static LocString TOOLTIP = "This plant will produce no more harvests";

				public static LocString NOTIFICATION = "Plants have died";

				public static LocString NOTIFICATION_TOOLTIP = "These plants have died and will produce no more harvests:\n";
			}

			public class STRUGGLING
			{
				public static LocString NAME = "Struggling";

				public static LocString TOOLTIP = "This critter is trying to get away";
			}

			public class BURROWING
			{
				public static LocString NAME = "Burrowing";

				public static LocString TOOLTIP = "This critter is trying to hide";
			}

			public class BURROWED
			{
				public static LocString NAME = "Burrowed";

				public static LocString TOOLTIP = "Shh! It thinks it's hiding";
			}

			public class EMERGING
			{
				public static LocString NAME = "Emerging";

				public static LocString TOOLTIP = "This critter is leaving its burrow";
			}

			public class EXPELLING_SOLID
			{
				public static LocString NAME = "Expelling Waste";

				public static LocString TOOLTIP = "This critter is doing their \"business\"";
			}

			public class EXPELLING_GAS
			{
				public static LocString NAME = "Passing Gas";

				public static LocString TOOLTIP = "This critter is emitting gas\n\nYuck!";
			}

			public class EXPELLING_LIQUID
			{
				public static LocString NAME = "Expelling Waste";

				public static LocString TOOLTIP = "This critter doing their \"business\"";
			}

			public class DEBUGGOTO
			{
				public static LocString NAME = "Moving to debug location";

				public static LocString TOOLTIP = "All that obedience training paid off";
			}

			public class ATTACK_APPROACH
			{
				public static LocString NAME = "Stalking Target";

				public static LocString TOOLTIP = "This critter is hostile and readying to pounce!";
			}

			public class ATTACK
			{
				public static LocString NAME = "Combat!";

				public static LocString TOOLTIP = "This critter is on the attack!";
			}

			public class LAYINGANEGG
			{
				public static LocString NAME = "Laying an egg";

				public static LocString TOOLTIP = "Witness the miracle of life!";
			}

			public class SUFFOCATING
			{
				public static LocString NAME = "Suffocating";

				public static LocString TOOLTIP = "This critter cannot breathe";
			}

			public class HATCHING
			{
				public static LocString NAME = "Hatching";

				public static LocString TOOLTIP = "Here it comes!";
			}

			public class INCUBATING
			{
				public static LocString NAME = "Incubating";

				public static LocString TOOLTIP = "Cozily preparing to meet the world";
			}

			public class CONSIDERINGLURE
			{
				public static LocString NAME = "Piqued";

				public static LocString TOOLTIP = "This critter is tempted to bite a nearby lure";
			}

			public class FALLING
			{
				public static LocString NAME = "Falling";

				public static LocString TOOLTIP = "AHHHH!";
			}

			public class DROWNING
			{
				public static LocString NAME = "Drowning";

				public static LocString TOOLTIP = "This critter can't breathe in liquid!";
			}

			public class DRYINGOUT
			{
				public static LocString NAME = "    • Beached";

				public static LocString TOOLTIP = "This plant must be submerged in liquid to grow";
			}

			public class GROWING
			{
				public static LocString NAME = "Growing [{PercentGrow}%]";

				public static LocString TOOLTIP = "Next harvest: {TimeUntilNextHarvest}";
			}

			public class NEEDSFERTILIZER
			{
				public static LocString NAME = "    • " + CREATURES.STATS.FERTILIZATION.NAME;

				public static LocString TOOLTIP = "Growth will resume when fertilization requirements are met";

				public static LocString LINE_ITEM = "\n            • {Resource}: {Amount}";
			}

			public class NEEDSIRRIGATION
			{
				public static LocString NAME = "    • " + CREATURES.STATS.IRRIGATION.NAME;

				public static LocString TOOLTIP = "Growth will resume when liquid requirements are met";

				public static LocString LINE_ITEM = "\n            • {Resource}: {Amount}";
			}

			public class WRONGFERTILIZER
			{
				public static LocString NAME = "    • " + CREATURES.STATS.FERTILIZATION.NAME;

				public static LocString TOOLTIP = "This farm is storing materials that are not suitable for this plant" + UI.HORIZONTAL_BR_RULE + "Empty this building's storage to remove the unusable materials";

				public static LocString LINE_ITEM = "            • {0}: {1}\n";
			}

			public class WRONGIRRIGATION
			{
				public static LocString NAME = "    • " + CREATURES.STATS.FERTILIZATION.NAME;

				public static LocString TOOLTIP = "This farm is storing materials that are not suitable for this plant" + UI.HORIZONTAL_BR_RULE + "Empty this building's storage to remove the unusable materials";

				public static LocString LINE_ITEM = "            • {0}: {1}\n";
			}

			public class WRONGFERTILIZERMAJOR
			{
				public static LocString NAME = "    • " + CREATURES.STATS.FERTILIZATION.NAME;

				public static LocString TOOLTIP = "This farm is storing materials that are not suitable for this plant" + UI.HORIZONTAL_BR_RULE + "Empty this building's storage to remove the unusable materials";

				public static LocString LINE_ITEM = "        " + CREATURES.STATUSITEMS.WRONGFERTILIZER.LINE_ITEM;
			}

			public class WRONGIRRIGATIONMAJOR
			{
				public static LocString NAME = "    • " + CREATURES.STATS.IRRIGATION.NAME;

				public static LocString TOOLTIP = "This farm is storing materials that are not suitable for this plant" + UI.HORIZONTAL_BR_RULE + "Empty this building's storage to remove the incorrect materials";

				public static LocString LINE_ITEM = "        " + CREATURES.STATUSITEMS.WRONGIRRIGATION.LINE_ITEM;
			}

			public class CANTACCEPTFERTILIZER
			{
				public static LocString NAME = "    • " + CREATURES.STATS.FERTILIZATION.NAME;

				public static LocString TOOLTIP = "This farm plot does not accept fertilizer\n\nMove the selected plant to a fertilization capable plot for optimal growth";
			}

			public class CANTACCEPTIRRIGATION
			{
				public static LocString NAME = "    • " + CREATURES.STATS.IRRIGATION.NAME;

				public static LocString TOOLTIP = "This farm plot does not accept irrigation\n\nMove the selected plant to an irrigation capable plot for optimal growth";
			}

			public class READYFORHARVEST
			{
				public static LocString NAME = "Harvest Ready";

				public static LocString TOOLTIP = "This plant can be harvested for materials";
			}

			public class LOW_YIELD
			{
				public static LocString NAME = "Standard Yield";

				public static LocString TOOLTIP = "This plant produced an average yield";
			}

			public class NORMAL_YIELD
			{
				public static LocString NAME = "Good Yield";

				public static LocString TOOLTIP = "Comfortable conditions allowed this plant to produce a better yield\n{Effects}";

				public static LocString LINE_ITEM = "    • {0}\n";
			}

			public class HIGH_YIELD
			{
				public static LocString NAME = "Excellent Yield";

				public static LocString TOOLTIP = "Consistently ideal conditions allowed this plant to bear a large yield\n{Effects}";

				public static LocString LINE_ITEM = "    • {0}\n";
			}

			public class ENTOMBED
			{
				public static LocString NAME = "Entombed";

				public static LocString TOOLTIP = "This critter is trapped and needs help digging out";

				public static LocString LINE_ITEM = "    • Entombed";
			}

			public class WILTING
			{
				public static LocString NAME = "Growth Halted{Reasons}";

				public static LocString TOOLTIP = "Growth will resume when conditions improve";
			}

			public class WILTINGDOMESTIC
			{
				public static LocString NAME = "Growth Halted{Reasons}";

				public static LocString TOOLTIP = "Growth will resume when conditions improve";
			}

			public class WILTING_NON_GROWING_PLANT
			{
				public static LocString NAME = "Growth Halted{Reasons}";

				public static LocString TOOLTIP = "Growth will resume when conditions improve";
			}

			public class BARREN
			{
				public static LocString NAME = "Barren";

				public static LocString TOOLTIP = "This plant will produce no more seeds";
			}

			public class ATMOSPHERICPRESSURETOOLOW
			{
				public static LocString NAME = "    • Pressure";

				public static LocString TOOLTIP = "Growth will resume when air pressure is between {low_mass} and {high_mass}";
			}

			public class WRONGATMOSPHERE
			{
				public static LocString NAME = "    • Atmosphere";

				public static LocString TOOLTIP = "Growth will resume when submersed in one of the following gases: {elements}";
			}

			public class ATMOSPHERICPRESSURETOOHIGH
			{
				public static LocString NAME = "    • Pressure";

				public static LocString TOOLTIP = "Growth will resume when air pressure is between {low_mass} and {high_mass}";
			}

			public class PERFECTATMOSPHERICPRESSURE
			{
				public static LocString NAME = "Ideal Air Pressure";

				public static LocString TOOLTIP = "This critter is comfortable in the current atmospheric pressure\n\nIdeal Range: {0} - {1}";
			}

			public class HEALTHSTATUS
			{
				public static LocString NAME = "Injuries: {healthState}";

				public static LocString TOOLTIP = "Current physical status: {healthState}";
			}

			public class FLEEING
			{
				public static LocString NAME = "Fleeing";

				public static LocString TOOLTIP = "This critter is trying to escape\nGet'em!";
			}

			public class UNREFRIGERATED
			{
				public static LocString NAME = "Unrefrigerated";

				public static LocString TOOLTIP = "Temperatures above {RotTemperature} spoil food more quickly";
			}

			public class CONTAMINATEDATMOSPHERE
			{
				public static LocString NAME = "Pollution Exposure";

				public static LocString TOOLTIP = "Exposure to contaminants is accelerating this food's decay rate";
			}

			public class STERILIZINGATMOSPHERE
			{
				public static LocString NAME = "Sterile Atmosphere";

				public static LocString TOOLTIP = "Microbe destroying conditions are preventing this food from decaying";
			}

			public class FRESH
			{
				public static LocString NAME = "Fresh {RotPercentage}";

				public static LocString TOOLTIP = "Get'em while they're hot!\n\n{RotTooltip}";
			}

			public class STALE
			{
				public static LocString NAME = "Stale {RotPercentage}";

				public static LocString TOOLTIP = "This food is still edible but will soon expire\n{RotTooltip}";
			}

			public class SPOILED
			{
				public static LocString NAME = "Rotten";

				public static LocString TOOLTIP = "This food has putrefied and should not be consumed";
			}

			public class REFRIGERATED
			{
				public static LocString NAME = "Refrigerated";

				public static LocString TOOLTIP = "Ideal temperature storage is slowing this food's decay rate";
			}

			public class RECEPTACLEINOPERATIONAL
			{
				public static LocString NAME = "    • Farm plot inoperable";

				public static LocString TOOLTIP = "This farm plot cannot grow plants in its current state";
			}

			public class TRAPPED
			{
				public static LocString NAME = "Trapped";

				public static LocString TOOLTIP = "This critter has been contained and cannot escape";
			}

			public class EXHALING
			{
				public static LocString NAME = "Exhaling";

				public static LocString TOOLTIP = "This critter is expelling gas from its lungsacs";
			}

			public class INHALING
			{
				public static LocString NAME = "Inhaling";

				public static LocString TOOLTIP = "This critter is taking a deep breath";
			}

			public class EXTERNALTEMPERATURE
			{
				public static LocString NAME = "External Temperature";

				public static LocString TOOLTIP = "External Temperature" + UI.HORIZONTAL_BR_RULE + "This critter's environment is {0}";
			}

			public class RECEPTACLEOPERATIONAL
			{
				public static LocString NAME = "Farm plot operational";

				public static LocString TOOLTIP = "This plant's farm plot is operational";
			}

			public class DOMESTICATION
			{
				public static LocString NAME = "Domestication Level: {LevelName}";

				public static LocString TOOLTIP = "{LevelDesc}";
			}

			public class HUNGRY
			{
				public static LocString NAME = "Hungry";

				public static LocString TOOLTIP = "This critter's tummy is rumbling";
			}

			public class STARVING
			{
				public static LocString NAME = "Starving\nTime until death: {TimeUntilDeath}\n";

				public static LocString TOOLTIP = "This critter is starving and will die if it is not fed soon";

				public static LocString NOTIFICATION_NAME = "Critter Starvation";

				public static LocString NOTIFICATION_TOOLTIP = "These critters are starving and will die if not fed soon:";
			}
		}

		public class STATS
		{
			public class HEALTH
			{
				public static LocString NAME = "Health";
			}

			public class MATURITY
			{
				public static LocString NAME = "Growth Progress";

				public static LocString TOOLTIP = "Growth Progress" + UI.HORIZONTAL_BR_RULE;

				public static LocString TOOLTIP_GROWING = "Predicted Maturation: {0}";

				public static LocString TOOLTIP_GROWING_CROP = "Predicted Maturation Time: {0}\nNext harvest occurs in approximately {1}";

				public static LocString TOOLTIP_GROWN = "Growth paused while plant awaits harvest";

				public static LocString TOOLTIP_STALLED = "Poor conditions have halted this plant's growth";

				public static LocString AMOUNT_DESC_FMT = "{0}: {1}\nNext harvest in {2}";

				public static LocString GROWING = "Domestic growth rate";

				public static LocString GROWINGWILD = "Wild growth rate";
			}

			public class FERTILIZATION
			{
				public static LocString NAME = "Fertilization";

				public static LocString CONSUME_MODIFIER = "Consuming";

				public static LocString ABSORBING_MODIFIER = "Absorbing";
			}

			public class DOMESTICATION
			{
				public static LocString NAME = "Domestication";

				public static LocString TOOLTIP = "Domestication" + UI.HORIZONTAL_BR_RULE + "Fully domesticated critters will produce more materials than wild ones, and may even provide psychological benefits to your colony\n\nThis critter is {0} domesticated";
			}

			public class HAPPINESS
			{
				public static LocString NAME = "Happiness";

				public static LocString TOOLTIP = "Happiness" + UI.HORIZONTAL_BR_RULE + "High Happiness increases a critter's productivity and indirectly improves their egg laying rates\n\nIt also provides satisfaction in knowing they're living a good little critter life";
			}

			public class WILDNESS
			{
				public static LocString NAME = "Wildness";

				public static LocString TOOLTIP = "Wildness" + UI.HORIZONTAL_BR_RULE + "At zero Wildness a critter can start gaining Happiness, but they will also require more calories to survive";
			}

			public class FERTILITY
			{
				public static LocString NAME = "Reproduction";

				public static LocString TOOLTIP = "Reproduction" + UI.HORIZONTAL_BR_RULE + "At 100% Reproduction, critters will reach the end of their reproduction cycle and lay a new egg\n\nAfter an egg is laid, Reproduction is rolled back to 0%";
			}

			public class INCUBATION
			{
				public static LocString NAME = "Incubation";

				public static LocString TOOLTIP = "Incubation" + UI.HORIZONTAL_BR_RULE + "Eggs hatch into brand new critters at the end of their incubation period";
			}

			public class IRRIGATION
			{
				public static LocString NAME = "Irrigation";

				public static LocString CONSUME_MODIFIER = "Consuming";

				public static LocString ABSORBING_MODIFIER = "Absorbing";
			}

			public class ILLUMINATION
			{
				public static LocString NAME = "Illumination";
			}

			public class THERMALCONDUCTIVITY
			{
				public static LocString NAME = "Thermal Conductivity";

				public static LocString TOOLTIP = "Heat can pass through the selected object at a rate of (W/m)/K";
			}

			public class THERMALCONDUCTIVITYBARRIER
			{
				public static LocString NAME = "Thermal Conductivity Barrier";

				public static LocString TOOLTIP = "Thick conductivity barriers increase the time it takes an object to heat up or cool down";
			}

			public class ROT
			{
				public static LocString NAME = "Freshness Percent";

				public static LocString TOOLTIP = "Freshness" + UI.HORIZONTAL_BR_RULE + "Food items become stale at fifty percent freshness, and rot at zero percent";
			}

			public class AIRPRESSURE
			{
				public static LocString NAME = "Air Pressure";

				public static LocString TOOLTIP = "The average gas density of the air surrounding this plant";
			}
		}

		public class MODIFIERS
		{
			public class DOMESTICATION_INCREASING
			{
				public static LocString NAME = "Happiness Increasing";

				public static LocString TOOLTIP = "This critter is very happy its needs are being met";
			}

			public class DOMESTICATION_DECREASING
			{
				public static LocString NAME = "Happiness Decreasing";

				public static LocString TOOLTIP = "Unfavorable conditions are making this critter unhappy";
			}

			public class BASE_FERTILITY
			{
				public static LocString NAME = "Base Reproduction";

				public static LocString TOOLTIP = "This is the base speed with which critters produce new eggs";
			}

			public class INCUBATOR_INCUBATION_RATE
			{
				public static LocString NAME = "Incubation Rate";

				public static LocString TOOLTIP = "Determines an egg's gestation period when placed in an Incubator";
			}

			public class RANCHED
			{
				public static LocString NAME = "Groomed";

				public static LocString TOOLTIP = "This critter has recently been attended to by a Rancher";
			}

			public class HAPPY
			{
				public static LocString NAME = "Happy";

				public static LocString TOOLTIP = "All of this critter's needs have been met and it's feeling quite contented" + UI.HORIZONTAL_BR_RULE + "It will produce more as a result";
			}

			public class WILD
			{
				public static LocString NAME = "Wild";

				public static LocString TOOLTIP = "This critter is wild";
			}

			public class TAME
			{
				public static LocString NAME = "Tame";

				public static LocString TOOLTIP = "This critter is tame";
			}

			public class OUT_OF_CALORIES
			{
				public static LocString NAME = "Starving";

				public static LocString TOOLTIP = "Get this critter something to eat!";
			}

			public class OVERCROWDED
			{
				public static LocString NAME = "Overcrowded";

				public static LocString TOOLTIP = "This critter isn't comfortable with so many other critters in a room of this size";
			}

			public class CONFINED
			{
				public static LocString NAME = "Confined";

				public static LocString TOOLTIP = "This critter is inside a door, tile, or confined space.";
			}
		}

		public class DOMESTICATION
		{
			public class LEVELS
			{
				public class UNHAPPY
				{
					public static LocString NAME = "Wild";

					public static LocString TOOLTIP = "This critter is fending for itself and does not trust Duplicants";
				}

				public class NEUTRAL
				{
					public static LocString NAME = "Skittish";

					public static LocString TOOLTIP = "This critter's most basic needs are being met, but it's still wary of Duplicants";
				}

				public class HAPPY
				{
					public static LocString NAME = "Tame";

					public static LocString TOOLTIP = "This critter is well taken care of and fully trusts its Duplicant caregivers";
				}

				public class SPOILED
				{
					public static LocString NAME = "Affectionate";

					public static LocString TOOLTIP = "This critter loves its Duplicant caregivers and is spoiled absolutely rotten";
				}
			}

			public class CRITERIA
			{
				public class TEMPERATURE_OK
				{
					public static LocString NAME = "Comfortable Temperature";

					public static LocString TOOLTIP = "Not too hot, not too cold.";
				}

				public class ATMOSPHERE_OK
				{
					public static LocString NAME = "Acceptable Atmosphere";

					public static LocString TOOLTIP = "It's breathable.";
				}

				public class IN_PEN
				{
					public static LocString NAME = "Critter Pen";

					public static LocString TOOLTIP = "Has a pen to live in.";
				}

				public class HAS_FOOD_SOURCE
				{
					public static LocString NAME = "Fed";

					public static LocString TOOLTIP = "Has enough food to eat.";
				}

				public class WELL_FED
				{
					public static LocString NAME = "Well Fed";

					public static LocString TOOLTIP = "This critter has recently dined on its favorite food.";
				}

				public class WELL_GROOMED
				{
					public static LocString NAME = "Well Groomed";

					public static LocString TOOLTIP = "Getting plenty of personal attention from a Rancher.";
				}
			}

			public class EFFECTS
			{
				public class UNPRODUCTIVE
				{
					public static LocString NAME = "Unproductive";

					public static LocString TOOLTIP = "Without improved conditions, this critter will not provide any useful products";
				}

				public class LOW_PRODUCTIVITY
				{
					public static LocString NAME = "Low Productivity";

					public static LocString TOOLTIP = "Producing at a reduced level";
				}

				public class GOOD_PRODUCTIVITY
				{
					public static LocString NAME = "Good Productivity";

					public static LocString TOOLTIP = "Producing at a normal level";
				}

				public class DESTRUCTIVE
				{
					public static LocString NAME = "Destructive";

					public static LocString TOOLTIP = "Likely to mess up the place";
				}

				public class INCREASED_CRITERIA_PENALTY
				{
					public static LocString NAME = "Sensitive";

					public static LocString TOOLTIP = "Loses happiness faster than normal under poor conditions";
				}

				public class LOSE_HAPPINESS
				{
					public static LocString NAME = "Downward Spiral";

					public static LocString TOOLTIP = "Losing happiness until conditions improve";
				}

				public class GAIN_FERTILITY
				{
					public static LocString NAME = "Increased Reproduction";

					public static LocString TOOLTIP = "This critter will produce an egg soon if conditions remain favorable";
				}

				public class LOSE_FERTILITY
				{
					public static LocString NAME = "Decreased Reproduction";

					public static LocString TOOLTIP = "This creature is holding off on laying eggs due to bad conditions";
				}
			}
		}
	}
}
