using System;

namespace STRINGS
{
	public class EQUIPMENT
	{
		public class PREFABS
		{
			public class OXYGEN_MASK
			{
				public static LocString NAME = UI.FormatAsLink("Oxygen Mask", "OXYGEN_MASK");

				public static LocString DESC = "Ensures my Duplicants can breathe easy... for a little while, anyways.";

				public static LocString EFFECT = "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in toxic and low breathability environments.\n\nMust be refilled with oxygen at an " + UI.FormatAsLink("Oxygen Mask Dock", "OXYGENMASKLOCKER") + " when depleted.";

				public static LocString RECIPE_DESC = "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in toxic and low breathability environments.";

				public static LocString GENERICNAME = "Suit";

				public static LocString WORN_NAME = UI.FormatAsLink("Worn Oxygen Mask", "OXYGEN_MASK");

				public static LocString WORN_DESC = string.Concat(new string[]
				{
					"A worn out ",
					UI.FormatAsLink("Oxygen Mask", "OXYGEN_MASK"),
					".\n\nMasks can be repaired at a ",
					UI.FormatAsLink("Crafting Station", "CRAFTINGTABLE"),
					"."
				});
			}

			public class ATMO_SUIT
			{
				public static LocString NAME = UI.FormatAsLink("Atmo Suit", "ATMO_SUIT");

				public static LocString DESC = "Ensures my Duplicants can breathe easy, anytime, anywhere.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Supplies Duplicants with ",
					UI.FormatAsLink("Oxygen", "OXYGEN"),
					" in toxic and low breathability environments, and protects against extreme temperatures.\n\nMust be refilled with oxygen at an ",
					UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER"),
					" when depleted."
				});

				public static LocString RECIPE_DESC = "Supplies Duplicants with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " in toxic and low breathability environments.";

				public static LocString GENERICNAME = "Suit";

				public static LocString WORN_NAME = UI.FormatAsLink("Worn Atmo Suit", "ATMO_SUIT");

				public static LocString WORN_DESC = string.Concat(new string[]
				{
					"A worn out ",
					UI.FormatAsLink("Atmo Suit", "ATMO_SUIT"),
					".\n\nSuits can be repaired at an ",
					UI.FormatAsLink("Exosuit Forge", "SUITFABRICATOR"),
					"."
				});

				public static LocString REPAIR_WORN_RECIPE_NAME = "Repair " + EQUIPMENT.PREFABS.ATMO_SUIT.NAME;

				public static LocString REPAIR_WORN_DESC = "Restore a " + UI.FormatAsLink("Worn Atmo Suit", "ATMO_SUIT") + " to working order.";
			}

			public class ATMO_SUIT_SET
			{
				public class PUFT
				{
					public static LocString NAME = "Puft Atmo Suit";

					public static LocString DESC = "Critter-forward protective gear for the intrepid explorer!\nReleased for Klei Fest 2023.";
				}
			}

			public class HOLIDAY_2023_CRATE
			{
				public static LocString NAME = "Holiday Gift Crate";

				public static LocString DESC = "An unaddressed package has been discovered near the Printing Pod. It exudes seasonal cheer, and trace amounts of Neutronium have been detected.";
			}

			public class ATMO_SUIT_HELMET
			{
				public static LocString NAME = "Default Atmo Helmet";

				public static LocString DESC = "Default helmet for atmo suits.";

				public class FACADES
				{
					public class SPARKLE_RED
					{
						public static LocString NAME = "Red Glitter Atmo Helmet";

						public static LocString DESC = "Protective gear at its sparkliest.";
					}

					public class SPARKLE_GREEN
					{
						public static LocString NAME = "Green Glitter Atmo Helmet";

						public static LocString DESC = "Protective gear at its sparkliest.";
					}

					public class SPARKLE_BLUE
					{
						public static LocString NAME = "Blue Glitter Atmo Helmet";

						public static LocString DESC = "Protective gear at its sparkliest.";
					}

					public class SPARKLE_PURPLE
					{
						public static LocString NAME = "Violet Glitter Atmo Helmet";

						public static LocString DESC = "Protective gear at its sparkliest.";
					}

					public class LIMONE
					{
						public static LocString NAME = "Citrus Atmo Helmet";

						public static LocString DESC = "Fresh, fruity and full of breathable air.";
					}

					public class PUFT
					{
						public static LocString NAME = "Puft Atmo Helmet";

						public static LocString DESC = "Convincing enough to fool most Pufts and even a few Duplicants.\nReleased for Klei Fest 2023.";
					}

					public class CLUBSHIRT_PURPLE
					{
						public static LocString NAME = "Eggplant Atmo Helmet";

						public static LocString DESC = "It is neither an egg, nor a plant. But it <i>is</i> a functional helmet.";
					}

					public class TRIANGLES_TURQ
					{
						public static LocString NAME = "Confetti Atmo Helmet";

						public static LocString DESC = "Doubles as a party hat.";
					}

					public class CUMMERBUND_RED
					{
						public static LocString NAME = "Blastoff Atmo Helmet";

						public static LocString DESC = "Red means go!";
					}

					public class WORKOUT_LAVENDER
					{
						public static LocString NAME = "Pink Punch Atmo Helmet";

						public static LocString DESC = "Unapologetically ostentatious.";
					}

					public class CANTALOUPE
					{
						public static LocString NAME = "Rocketmelon Atmo Helmet";

						public static LocString DESC = "A melon for your melon.";
					}

					public class MONDRIAN_BLUE_RED_YELLOW
					{
						public static LocString NAME = "Cubist Atmo Helmet";

						public static LocString DESC = "Abstract geometrics are both hip <i>and</i> square.";
					}

					public class OVERALLS_RED
					{
						public static LocString NAME = "Spiffy Atmo Helmet";

						public static LocString DESC = "The twin antennae serve as an early warning system for low ceilings.";
					}
				}
			}

			public class ATMO_SUIT_BODY
			{
				public static LocString NAME = "Default Atmo Uniform";

				public static LocString DESC = "Default top and bottom of an atmo suit.";

				public class FACADES
				{
					public class SPARKLE_RED
					{
						public static LocString NAME = "Red Glitter Atmo Suit";

						public static LocString DESC = "Protects the wearer from hostile environments <i>and</i> drab fashion.";
					}

					public class SPARKLE_GREEN
					{
						public static LocString NAME = "Green Glitter Atmo Suit";

						public static LocString DESC = "Protects the wearer from hostile environments <i>and</i> drab fashion.";
					}

					public class SPARKLE_BLUE
					{
						public static LocString NAME = "Blue Glitter Atmo Suit";

						public static LocString DESC = "Protects the wearer from hostile environments <i>and</i> drab fashion.";
					}

					public class SPARKLE_LAVENDER
					{
						public static LocString NAME = "Violet Glitter Atmo Suit";

						public static LocString DESC = "Protects the wearer from hostile environments <i>and</i> drab fashion.";
					}

					public class LIMONE
					{
						public static LocString NAME = "Citrus Atmo Suit";

						public static LocString DESC = "Perfect for summery, atmospheric excursions.";
					}

					public class PUFT
					{
						public static LocString NAME = "Puft Atmo Suit";

						public static LocString DESC = "Warning: prolonged wear may result in feelings of Puft-up pride.\nReleased for Klei Fest 2023.";
					}

					public class BASIC_PURPLE
					{
						public static LocString NAME = "Crisp Eggplant Atmo Suit";

						public static LocString DESC = "It really emphasizes wide shoulders.";
					}

					public class PRINT_TRIANGLES_TURQ
					{
						public static LocString NAME = "Confetti Atmo Suit";

						public static LocString DESC = "It puts the \"fun\" in \"perfunctory nods to personnel individuality\"!";
					}

					public class BASIC_NEON_PINK
					{
						public static LocString NAME = "Crisp Neon Pink Atmo Suit";

						public static LocString DESC = "The neck is a little snug.";
					}

					public class MULTI_RED_BLACK
					{
						public static LocString NAME = "Red-bellied Atmo Suit";

						public static LocString DESC = "It really highlights the midsection.";
					}

					public class CANTALOUPE
					{
						public static LocString NAME = "Rocketmelon Atmo Suit";

						public static LocString DESC = "It starts to smell ripe pretty quickly.";
					}

					public class MULTI_BLUE_GREY_BLACK
					{
						public static LocString NAME = "Swagger Atmo Suit";

						public static LocString DESC = "Engineered to resemble stonewashed denim and black leather.";
					}

					public class MULTI_BLUE_YELLOW_RED
					{
						public static LocString NAME = "Fundamental Stripe Atmo Suit";

						public static LocString DESC = "Designed by the Primary Colors Appreciation Society.";
					}
				}
			}

			public class ATMO_SUIT_GLOVES
			{
				public static LocString NAME = "Default Atmo Gloves";

				public static LocString DESC = "Default atmo suit gloves.";

				public class FACADES
				{
					public class SPARKLE_RED
					{
						public static LocString NAME = "Red Glitter Atmo Gloves";

						public static LocString DESC = "Sparkly red gloves for hostile environments.";
					}

					public class SPARKLE_GREEN
					{
						public static LocString NAME = "Green Glitter Atmo Gloves";

						public static LocString DESC = "Sparkly green gloves for hostile environments.";
					}

					public class SPARKLE_BLUE
					{
						public static LocString NAME = "Blue Glitter Atmo Gloves";

						public static LocString DESC = "Sparkly blue gloves for hostile environments.";
					}

					public class SPARKLE_LAVENDER
					{
						public static LocString NAME = "Violet Glitter Atmo Gloves";

						public static LocString DESC = "Sparkly violet gloves for hostile environments.";
					}

					public class LIMONE
					{
						public static LocString NAME = "Citrus Atmo Gloves";

						public static LocString DESC = "Lime-inspired gloves brighten up hostile environments.";
					}

					public class PUFT
					{
						public static LocString NAME = "Puft Atmo Gloves";

						public static LocString DESC = "A little Puft-love for delicate extremities.\nReleased for Klei Fest 2023.";
					}

					public class GOLD
					{
						public static LocString NAME = "Gold Atmo Gloves";

						public static LocString DESC = "A golden touch! Without all the Midas-type baggage.";
					}

					public class PURPLE
					{
						public static LocString NAME = "Eggplant Atmo Gloves";

						public static LocString DESC = "Fab purple gloves for hostile environments.";
					}

					public class WHITE
					{
						public static LocString NAME = "White Atmo Gloves";

						public static LocString DESC = "For the Duplicant who never gets their hands dirty.";
					}

					public class STRIPES_LAVENDER
					{
						public static LocString NAME = "Wildberry Atmo Gloves";

						public static LocString DESC = "Functional finger-protectors with fruity flair.";
					}

					public class CANTALOUPE
					{
						public static LocString NAME = "Rocketmelon Atmo Gloves";

						public static LocString DESC = "It takes eighteen melon rinds to make a single glove.";
					}

					public class BROWN
					{
						public static LocString NAME = "Leather Atmo Gloves";

						public static LocString DESC = "They creak rather loudly during the break-in period.";
					}
				}
			}

			public class ATMO_SUIT_BELT
			{
				public static LocString NAME = "Default Atmo Belt";

				public static LocString DESC = "Default belt for atmo suits.";

				public class FACADES
				{
					public class SPARKLE_RED
					{
						public static LocString NAME = "Red Glitter Atmo Belt";

						public static LocString DESC = "It's red! It's shiny! It keeps atmo suit pants on!";
					}

					public class SPARKLE_GREEN
					{
						public static LocString NAME = "Green Glitter Atmo Belt";

						public static LocString DESC = "It's green! It's shiny! It keeps atmo suit pants on!";
					}

					public class SPARKLE_BLUE
					{
						public static LocString NAME = "Blue Glitter Atmo Belt";

						public static LocString DESC = "It's blue! It's shiny! It keeps atmo suit pants on!";
					}

					public class SPARKLE_LAVENDER
					{
						public static LocString NAME = "Violet Glitter Atmo Belt";

						public static LocString DESC = "It's violet! It's shiny! It keeps atmo suit pants on!";
					}

					public class LIMONE
					{
						public static LocString NAME = "Citrus Atmo Belt";

						public static LocString DESC = "This lime-hued belt really pulls an atmo suit together.";
					}

					public class PUFT
					{
						public static LocString NAME = "Puft Atmo Belt";

						public static LocString DESC = "If critters wore belts...\nReleased for Klei Fest 2023.";
					}

					public class TWOTONE_PURPLE
					{
						public static LocString NAME = "Eggplant Atmo Belt";

						public static LocString DESC = "In the more pretentious space-fashion circles, it's known as \"aubergine.\"";
					}

					public class BASIC_GOLD
					{
						public static LocString NAME = "Gold Atmo Belt";

						public static LocString DESC = "Better to be overdressed than underdressed.";
					}

					public class BASIC_GREY
					{
						public static LocString NAME = "Slate Atmo Belt";

						public static LocString DESC = "Slick and understated space style.";
					}

					public class BASIC_NEON_PINK
					{
						public static LocString NAME = "Neon Pink Atmo Belt";

						public static LocString DESC = "Visible from several planetoids away.";
					}

					public class CANTALOUPE
					{
						public static LocString NAME = "Rocketmelon Atmo Belt";

						public static LocString DESC = "A tribute to the <i>cucumis melo cantalupensis</i>.";
					}

					public class TWOTONE_BROWN
					{
						public static LocString NAME = "Leather Atmo Belt";

						public static LocString DESC = "Crafted from the tanned hide of a thick-skinned critter.";
					}
				}
			}

			public class ATMO_SUIT_SHOES
			{
				public static LocString NAME = "Default Atmo Boots";

				public static LocString DESC = "Default footwear for atmo suits.";

				public class FACADES
				{
					public class LIMONE
					{
						public static LocString NAME = "Citrus Atmo Boots";

						public static LocString DESC = "Cheery boots for stomping around in hostile environments.";
					}

					public class PUFT
					{
						public static LocString NAME = "Puft Atmo Boots";

						public static LocString DESC = "These boots were made for puft-ing.\nReleased for Klei Fest 2023.";
					}

					public class SPARKLE_BLACK
					{
						public static LocString NAME = "Black Glitter Atmo Boots";

						public static LocString DESC = "A timeless color, with a little pizzazz.";
					}

					public class BASIC_BLACK
					{
						public static LocString NAME = "Stealth Atmo Boots";

						public static LocString DESC = "They attract no attention at all.";
					}

					public class BASIC_PURPLE
					{
						public static LocString NAME = "Eggplant Atmo Boots";

						public static LocString DESC = "Purple boots for stomping around in hostile environments.";
					}

					public class BASIC_LAVENDER
					{
						public static LocString NAME = "Lavender Atmo Boots";

						public static LocString DESC = "Soothing space booties for tired feet.";
					}

					public class CANTALOUPE
					{
						public static LocString NAME = "Rocketmelon Atmo Boots";

						public static LocString DESC = "Keeps feet safe (and juicy) in hostile environments.";
					}
				}
			}

			public class AQUA_SUIT
			{
				public static LocString NAME = UI.FormatAsLink("Aqua Suit", "AQUA_SUIT");

				public static LocString DESC = "Because breathing underwater is better than... not.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in underwater environments.\n\nMust be refilled with ",
					UI.FormatAsLink("Oxygen", "OXYGEN"),
					" at an ",
					UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER"),
					" when depleted."
				});

				public static LocString RECIPE_DESC = "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in underwater environments.";

				public static LocString WORN_NAME = UI.FormatAsLink("Worn Lead Suit", "AQUA_SUIT");

				public static LocString WORN_DESC = string.Concat(new string[]
				{
					"A worn out ",
					UI.FormatAsLink("Aqua Suit", "AQUA_SUIT"),
					".\n\nSuits can be repaired at a ",
					UI.FormatAsLink("Crafting Station", "CRAFTINGTABLE"),
					"."
				});
			}

			public class TEMPERATURE_SUIT
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Suit", "TEMPERATURE_SUIT");

				public static LocString DESC = "Keeps my Duplicants cool in case things heat up.";

				public static LocString EFFECT = "Provides insulation in regions with extreme <style=\"heat\">Temperatures</style>.\n\nMust be powered at a Thermo Suit Dock when depleted.";

				public static LocString RECIPE_DESC = "Provides insulation in regions with extreme <style=\"heat\">Temperatures</style>.";

				public static LocString WORN_NAME = UI.FormatAsLink("Worn Lead Suit", "TEMPERATURE_SUIT");

				public static LocString WORN_DESC = string.Concat(new string[]
				{
					"A worn out ",
					UI.FormatAsLink("Thermo Suit", "TEMPERATURE_SUIT"),
					".\n\nSuits can be repaired at a ",
					UI.FormatAsLink("Crafting Station", "CRAFTINGTABLE"),
					"."
				});
			}

			public class JET_SUIT
			{
				public static LocString NAME = UI.FormatAsLink("Jet Suit", "JET_SUIT");

				public static LocString DESC = "Allows my Duplicants to take to the skies, for a time.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Supplies Duplicants with ",
					UI.FormatAsLink("Oxygen", "OXYGEN"),
					" in toxic and low breathability environments.\n\nMust be refilled with ",
					UI.FormatAsLink("Oxygen", "OXYGEN"),
					" and ",
					UI.FormatAsLink("Petroleum", "PETROLEUM"),
					" at a ",
					UI.FormatAsLink("Jet Suit Dock", "JETSUITLOCKER"),
					" when depleted."
				});

				public static LocString RECIPE_DESC = "Supplies Duplicants with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " in toxic and low breathability environments.\n\nAllows Duplicant flight.";

				public static LocString GENERICNAME = "Jet Suit";

				public static LocString TANK_EFFECT_NAME = "Fuel Tank";

				public static LocString WORN_NAME = UI.FormatAsLink("Worn Jet Suit", "JET_SUIT");

				public static LocString WORN_DESC = string.Concat(new string[]
				{
					"A worn out ",
					UI.FormatAsLink("Jet Suit", "JET_SUIT"),
					".\n\nSuits can be repaired at an ",
					UI.FormatAsLink("Exosuit Forge", "SUITFABRICATOR"),
					"."
				});
			}

			public class LEAD_SUIT
			{
				public static LocString NAME = UI.FormatAsLink("Lead Suit", "LEAD_SUIT");

				public static LocString DESC = "Because exposure to radiation doesn't grant Duplicants superpowers.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Supplies Duplicants with ",
					UI.FormatAsLink("Oxygen", "OXYGEN"),
					" and protection in areas with ",
					UI.FormatAsLink("Radiation", "RADIATION"),
					".\n\nMust be refilled with ",
					UI.FormatAsLink("Oxygen", "OXYGEN"),
					" at a ",
					UI.FormatAsLink("Lead Suit Dock", "LEADSUITLOCKER"),
					" when depleted."
				});

				public static LocString RECIPE_DESC = string.Concat(new string[]
				{
					"Supplies Duplicants with ",
					UI.FormatAsLink("Oxygen", "OXYGEN"),
					" in toxic and low breathability environments.\n\nProtects Duplicants from ",
					UI.FormatAsLink("Radiation", "RADIATION"),
					"."
				});

				public static LocString GENERICNAME = "Lead Suit";

				public static LocString BATTERY_EFFECT_NAME = "Suit Battery";

				public static LocString SUIT_OUT_OF_BATTERIES = "Suit Batteries Empty";

				public static LocString WORN_NAME = UI.FormatAsLink("Worn Lead Suit", "LEAD_SUIT");

				public static LocString WORN_DESC = string.Concat(new string[]
				{
					"A worn out ",
					UI.FormatAsLink("Lead Suit", "LEAD_SUIT"),
					".\n\nSuits can be repaired at an ",
					UI.FormatAsLink("Exosuit Forge", "SUITFABRICATOR"),
					"."
				});
			}

			public class COOL_VEST
			{
				public static LocString NAME = UI.FormatAsLink("Cool Vest", "COOL_VEST");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "Don't sweat it!";

				public static LocString EFFECT = "Protects the wearer from <style=\"heat\">Heat</style> by decreasing insulation.";

				public static LocString RECIPE_DESC = "Protects the wearer from <style=\"heat\">Heat</style> by decreasing insulation";
			}

			public class WARM_VEST
			{
				public static LocString NAME = UI.FormatAsLink("Warm Coat", "WARM_VEST");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "Happiness is a warm Duplicant.";

				public static LocString EFFECT = "Protects the wearer from <style=\"heat\">Cold</style> by increasing insulation.";

				public static LocString RECIPE_DESC = "Protects the wearer from <style=\"heat\">Cold</style> by increasing insulation";
			}

			public class FUNKY_VEST
			{
				public static LocString NAME = UI.FormatAsLink("Snazzy Suit", "FUNKY_VEST");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "This transforms my Duplicant into a walking beacon of charm and style.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Increases Decor in a small area effect around the wearer. Can be upgraded to ",
					UI.FormatAsLink("Primo Garb", "CUSTOMCLOTHING"),
					" at the ",
					UI.FormatAsLink("Clothing Refashionator", "CLOTHINGALTERATIONSTATION"),
					"."
				});

				public static LocString RECIPE_DESC = "Increases Decor in a small area effect around the wearer. Can be upgraded to " + UI.FormatAsLink("Primo Garb", "CUSTOMCLOTHING") + " at the " + UI.FormatAsLink("Clothing Refashionator", "CLOTHINGALTERATIONSTATION");
			}

			public class CUSTOMCLOTHING
			{
				public static LocString NAME = UI.FormatAsLink("Primo Garb", "CUSTOMCLOTHING");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "This transforms my Duplicant into a colony-inspiring fashion icon.";

				public static LocString EFFECT = "Increases Decor in a small area effect around the wearer.";

				public static LocString RECIPE_DESC = "Increases Decor in a small area effect around the wearer";

				public class FACADES
				{
					public static LocString CLUBSHIRT = UI.FormatAsLink("Purple Polyester Suit", "CUSTOMCLOTHING");

					public static LocString CUMMERBUND = UI.FormatAsLink("Classic Cummerbund", "CUSTOMCLOTHING");

					public static LocString DECOR_02 = UI.FormatAsLink("Snazzier Red Suit", "CUSTOMCLOTHING");

					public static LocString DECOR_03 = UI.FormatAsLink("Snazzier Blue Suit", "CUSTOMCLOTHING");

					public static LocString DECOR_04 = UI.FormatAsLink("Snazzier Green Suit", "CUSTOMCLOTHING");

					public static LocString DECOR_05 = UI.FormatAsLink("Snazzier Violet Suit", "CUSTOMCLOTHING");

					public static LocString GAUDYSWEATER = UI.FormatAsLink("Pompom Knit Suit", "CUSTOMCLOTHING");

					public static LocString LIMONE = UI.FormatAsLink("Citrus Spandex Suit", "CUSTOMCLOTHING");

					public static LocString MONDRIAN = UI.FormatAsLink("Cubist Knit Suit", "CUSTOMCLOTHING");

					public static LocString OVERALLS = UI.FormatAsLink("Spiffy Overalls", "CUSTOMCLOTHING");

					public static LocString TRIANGLES = UI.FormatAsLink("Confetti Suit", "CUSTOMCLOTHING");

					public static LocString WORKOUT = UI.FormatAsLink("Pink Unitard", "CUSTOMCLOTHING");
				}
			}

			public class CLOTHING_GLOVES
			{
				public static LocString NAME = "Default Gloves";

				public static LocString DESC = "The default gloves.";

				public class FACADES
				{
					public class BASIC_BLUE_MIDDLE
					{
						public static LocString NAME = "Basic Aqua Gloves";

						public static LocString DESC = "A good, solid pair of aqua-blue gloves that go with everything.";
					}

					public class BASIC_YELLOW
					{
						public static LocString NAME = "Basic Yellow Gloves";

						public static LocString DESC = "A good, solid pair of yellow gloves that go with everything.";
					}

					public class BASIC_BLACK
					{
						public static LocString NAME = "Basic Black Gloves";

						public static LocString DESC = "A good, solid pair of black gloves that go with everything.";
					}

					public class BASIC_PINK_ORCHID
					{
						public static LocString NAME = "Basic Bubblegum Gloves";

						public static LocString DESC = "A good, solid pair of bubblegum-pink gloves that go with everything.";
					}

					public class BASIC_GREEN
					{
						public static LocString NAME = "Basic Green Gloves";

						public static LocString DESC = "A good, solid pair of green gloves that go with everything.";
					}

					public class BASIC_ORANGE
					{
						public static LocString NAME = "Basic Orange Gloves";

						public static LocString DESC = "A good, solid pair of orange gloves that go with everything.";
					}

					public class BASIC_PURPLE
					{
						public static LocString NAME = "Basic Purple Gloves";

						public static LocString DESC = "A good, solid pair of purple gloves that go with everything.";
					}

					public class BASIC_RED
					{
						public static LocString NAME = "Basic Red Gloves";

						public static LocString DESC = "A good, solid pair of red gloves that go with everything.";
					}

					public class BASIC_WHITE
					{
						public static LocString NAME = "Basic White Gloves";

						public static LocString DESC = "A good, solid pair of white gloves that go with everything.";
					}

					public class GLOVES_ATHLETIC_DEEPRED
					{
						public static LocString NAME = "Team Captain Sports Gloves";

						public static LocString DESC = "Red-striped gloves for winning at any activity.";
					}

					public class GLOVES_ATHLETIC_SATSUMA
					{
						public static LocString NAME = "Superfan Sports Gloves";

						public static LocString DESC = "Orange-striped gloves for enthusiastic athletes.";
					}

					public class GLOVES_ATHLETIC_LEMON
					{
						public static LocString NAME = "Hype Sports Gloves";

						public static LocString DESC = "Yellow-striped gloves for athletes who seek to raise the bar.";
					}

					public class GLOVES_ATHLETIC_KELLYGREEN
					{
						public static LocString NAME = "Go Team Sports Gloves";

						public static LocString DESC = "Green-striped gloves for the perenially good sport.";
					}

					public class GLOVES_ATHLETIC_COBALT
					{
						public static LocString NAME = "True Blue Sports Gloves";

						public static LocString DESC = "Blue-striped gloves perfect for shaking hands after the game.";
					}

					public class GLOVES_ATHLETIC_FLAMINGO
					{
						public static LocString NAME = "Pep Rally Sports Gloves";

						public static LocString DESC = "Pink-striped glove designed to withstand countless high-fives.";
					}

					public class GLOVES_ATHLETIC_CHARCOAL
					{
						public static LocString NAME = "Underdog Sports Gloves";

						public static LocString DESC = "The muted stripe minimizes distractions so its wearer can focus on trying very, very hard.";
					}

					public class CUFFLESS_BLUEBERRY
					{
						public static LocString NAME = "Blueberry Glovelets";

						public static LocString DESC = "Wrist coverage is <i>so</i> overrated.";
					}

					public class CUFFLESS_GRAPE
					{
						public static LocString NAME = "Grape Glovelets";

						public static LocString DESC = "Wrist coverage is <i>so</i> overrated.";
					}

					public class CUFFLESS_LEMON
					{
						public static LocString NAME = "Lemon Glovelets";

						public static LocString DESC = "Wrist coverage is <i>so</i> overrated.";
					}

					public class CUFFLESS_LIME
					{
						public static LocString NAME = "Lime Glovelets";

						public static LocString DESC = "Wrist coverage is <i>so</i> overrated.";
					}

					public class CUFFLESS_SATSUMA
					{
						public static LocString NAME = "Satsuma Glovelets";

						public static LocString DESC = "Wrist coverage is <i>so</i> overrated.";
					}

					public class CUFFLESS_STRAWBERRY
					{
						public static LocString NAME = "Strawberry Glovelets";

						public static LocString DESC = "Wrist coverage is <i>so</i> overrated.";
					}

					public class CUFFLESS_WATERMELON
					{
						public static LocString NAME = "Watermelon Glovelets";

						public static LocString DESC = "Wrist coverage is <i>so</i> overrated.";
					}

					public class CIRCUIT_GREEN
					{
						public static LocString NAME = "LED Gloves";

						public static LocString DESC = "Great for gesticulating at parties.";
					}

					public class ATHLETE
					{
						public static LocString NAME = "Racing Gloves";

						public static LocString DESC = "Crafted for high-speed handshakes.";
					}

					public class BASIC_BROWN_KHAKI
					{
						public static LocString NAME = "Basic Khaki Gloves";

						public static LocString DESC = "They don't show dirt.";
					}

					public class BASIC_BLUEGREY
					{
						public static LocString NAME = "Basic Gunmetal Gloves";

						public static LocString DESC = "A tough name for soft gloves.";
					}

					public class CUFFLESS_BLACK
					{
						public static LocString NAME = "Stealth Glovelets";

						public static LocString DESC = "It's easy to forget they're even on.";
					}

					public class DENIM_BLUE
					{
						public static LocString NAME = "Denim Gloves";

						public static LocString DESC = "They're not great for dexterity.";
					}

					public class BASIC_GREY
					{
						public static LocString NAME = "Basic Gray Gloves";

						public static LocString DESC = "A good, solid pair of gray gloves that go with everything.";
					}

					public class BASIC_PINKSALMON
					{
						public static LocString NAME = "Basic Coral Gloves";

						public static LocString DESC = "A good, solid pair of bright pink gloves that go with everything.";
					}

					public class BASIC_TAN
					{
						public static LocString NAME = "Basic Tan Gloves";

						public static LocString DESC = "A good, solid pair of tan gloves that go with everything.";
					}

					public class BALLERINA_PINK
					{
						public static LocString NAME = "Ballet Gloves";

						public static LocString DESC = "Wrist ruffles highlight the poetic movements of the phalanges.";
					}

					public class FORMAL_WHITE
					{
						public static LocString NAME = "White Silk Gloves";

						public static LocString DESC = "They're as soft as...well, silk.";
					}

					public class LONG_WHITE
					{
						public static LocString NAME = "White Evening Gloves";

						public static LocString DESC = "Super-long gloves for super-formal occasions.";
					}

					public class TWOTONE_CREAM_CHARCOAL
					{
						public static LocString NAME = "Contrast Cuff Gloves";

						public static LocString DESC = "For elegance so understated, it may go completely unnoticed.";
					}

					public class SOCKSUIT_BEIGE
					{
						public static LocString NAME = "Vintage Handsock";

						public static LocString DESC = "Designed by someone with cold hands and an excess of old socks.";
					}

					public class BASIC_SLATE
					{
						public static LocString NAME = "Basic Slate Gloves";

						public static LocString DESC = "A good, solid pair of slate gloves that go with everything.";
					}

					public class KNIT_GOLD
					{
						public static LocString NAME = "Gold Knit Gloves";

						public static LocString DESC = "Produces a pleasantly muffled \"whump\" when high-fiving.";
					}

					public class KNIT_MAGENTA
					{
						public static LocString NAME = "Magenta Knit Gloves";

						public static LocString DESC = "Produces a pleasantly muffled \"whump\" when high-fiving.";
					}

					public class SPARKLE_WHITE
					{
						public static LocString NAME = "White Glitter Gloves";

						public static LocString DESC = "Each sequin was attached using sealant borrowed from the rocketry department.";
					}

					public class GINCH_PINK_SALTROCK
					{
						public static LocString NAME = "Frilly Saltrock Gloves";

						public static LocString DESC = "Thick, soft pink gloves with added flounce.";
					}

					public class GINCH_PURPLE_DUSKY
					{
						public static LocString NAME = "Frilly Dusk Gloves";

						public static LocString DESC = "Thick, soft purple gloves with added flounce.";
					}

					public class GINCH_BLUE_BASIN
					{
						public static LocString NAME = "Frilly Basin Gloves";

						public static LocString DESC = "Thick, soft blue gloves with added flounce.";
					}

					public class GINCH_TEAL_BALMY
					{
						public static LocString NAME = "Frilly Balm Gloves";

						public static LocString DESC = "The soft teal fabric soothes hard-working hands.";
					}

					public class GINCH_GREEN_LIME
					{
						public static LocString NAME = "Frilly Leach Gloves";

						public static LocString DESC = "Thick, soft green gloves with added flounce.";
					}

					public class GINCH_YELLOW_YELLOWCAKE
					{
						public static LocString NAME = "Frilly Yellowcake Gloves";

						public static LocString DESC = "Thick, soft yellow gloves with added flounce.";
					}

					public class GINCH_ORANGE_ATOMIC
					{
						public static LocString NAME = "Frilly Atomic Gloves";

						public static LocString DESC = "Thick, bright orange gloves with added flounce.";
					}

					public class GINCH_RED_MAGMA
					{
						public static LocString NAME = "Frilly Magma Gloves";

						public static LocString DESC = "Thick, soft red gloves with added flounce.";
					}

					public class GINCH_GREY_GREY
					{
						public static LocString NAME = "Frilly Slate Gloves";

						public static LocString DESC = "Thick, soft grey gloves with added flounce.";
					}

					public class GINCH_GREY_CHARCOAL
					{
						public static LocString NAME = "Frilly Charcoal Gloves";

						public static LocString DESC = "Thick, soft dark grey gloves with added flounce.";
					}
				}
			}

			public class CLOTHING_TOPS
			{
				public static LocString NAME = "Default Top";

				public static LocString DESC = "The default shirt.";

				public class FACADES
				{
					public class BASIC_BLUE_MIDDLE
					{
						public static LocString NAME = "Basic Aqua Shirt";

						public static LocString DESC = "A nice aqua-blue shirt that goes with everything.";
					}

					public class BASIC_BLACK
					{
						public static LocString NAME = "Basic Black Shirt";

						public static LocString DESC = "A nice black shirt that goes with everything.";
					}

					public class BASIC_PINK_ORCHID
					{
						public static LocString NAME = "Basic Bubblegum Shirt";

						public static LocString DESC = "A nice bubblegum-pink shirt that goes with everything.";
					}

					public class BASIC_GREEN
					{
						public static LocString NAME = "Basic Green Shirt";

						public static LocString DESC = "A nice green shirt that goes with everything.";
					}

					public class BASIC_ORANGE
					{
						public static LocString NAME = "Basic Orange Shirt";

						public static LocString DESC = "A nice orange shirt that goes with everything.";
					}

					public class BASIC_PURPLE
					{
						public static LocString NAME = "Basic Purple Shirt";

						public static LocString DESC = "A nice purple shirt that goes with everything.";
					}

					public class BASIC_RED_BURNT
					{
						public static LocString NAME = "Basic Red Shirt";

						public static LocString DESC = "A nice red shirt that goes with everything.";
					}

					public class BASIC_WHITE
					{
						public static LocString NAME = "Basic White Shirt";

						public static LocString DESC = "A nice white shirt that goes with everything.";
					}

					public class BASIC_YELLOW
					{
						public static LocString NAME = "Basic Yellow Shirt";

						public static LocString DESC = "A nice yellow shirt that goes with everything.";
					}

					public class RAGLANTOP_DEEPRED
					{
						public static LocString NAME = "Team Captain T-shirt";

						public static LocString DESC = "A slightly sweat-stained tee for natural leaders.";
					}

					public class RAGLANTOP_COBALT
					{
						public static LocString NAME = "True Blue T-shirt";

						public static LocString DESC = "A slightly sweat-stained tee for the real team players.";
					}

					public class RAGLANTOP_FLAMINGO
					{
						public static LocString NAME = "Pep Rally T-shirt";

						public static LocString DESC = "A slightly sweat-stained tee to boost team spirits.";
					}

					public class RAGLANTOP_KELLYGREEN
					{
						public static LocString NAME = "Go Team T-shirt";

						public static LocString DESC = "A slightly sweat-stained tee for cheering from the sidelines.";
					}

					public class RAGLANTOP_CHARCOAL
					{
						public static LocString NAME = "Underdog T-shirt";

						public static LocString DESC = "For those who don't win a lot.";
					}

					public class RAGLANTOP_LEMON
					{
						public static LocString NAME = "Hype T-shirt";

						public static LocString DESC = "A slightly sweat-stained tee to wear when talking a big game.";
					}

					public class RAGLANTOP_SATSUMA
					{
						public static LocString NAME = "Superfan T-shirt";

						public static LocString DESC = "A slightly sweat-stained tee for the long-time supporter.";
					}

					public class JELLYPUFFJACKET_BLUEBERRY
					{
						public static LocString NAME = "Blueberry Jelly Jacket";

						public static LocString DESC = "It's best to keep jelly-filled puffer jackets away from sharp corners.";
					}

					public class JELLYPUFFJACKET_GRAPE
					{
						public static LocString NAME = "Grape Jelly Jacket";

						public static LocString DESC = "It's best to keep jelly-filled puffer jackets away from sharp corners.";
					}

					public class JELLYPUFFJACKET_LEMON
					{
						public static LocString NAME = "Lemon Jelly Jacket";

						public static LocString DESC = "It's best to keep jelly-filled puffer jackets away from sharp corners.";
					}

					public class JELLYPUFFJACKET_LIME
					{
						public static LocString NAME = "Lime Jelly Jacket";

						public static LocString DESC = "It's best to keep jelly-filled puffer jackets away from sharp corners.";
					}

					public class JELLYPUFFJACKET_SATSUMA
					{
						public static LocString NAME = "Satsuma Jelly Jacket";

						public static LocString DESC = "It's best to keep jelly-filled puffer jackets away from sharp corners.";
					}

					public class JELLYPUFFJACKET_STRAWBERRY
					{
						public static LocString NAME = "Strawberry Jelly Jacket";

						public static LocString DESC = "It's best to keep jelly-filled puffer jackets away from sharp corners.";
					}

					public class JELLYPUFFJACKET_WATERMELON
					{
						public static LocString NAME = "Watermelon Jelly Jacket";

						public static LocString DESC = "It's best to keep jelly-filled puffer jackets away from sharp corners.";
					}

					public class CIRCUIT_GREEN
					{
						public static LocString NAME = "LED Jacket";

						public static LocString DESC = "For dancing in the dark.";
					}

					public class TSHIRT_WHITE
					{
						public static LocString NAME = "Classic White Tee";

						public static LocString DESC = "It's practically begging for a big Bog Jelly stain down the front.";
					}

					public class TSHIRT_MAGENTA
					{
						public static LocString NAME = "Classic Magenta Tee";

						public static LocString DESC = "It will never chafe against delicate inner-elbow skin.";
					}

					public class ATHLETE
					{
						public static LocString NAME = "Racing Jacket";

						public static LocString DESC = "The epitome of fast fashion.";
					}

					public class DENIM_BLUE
					{
						public static LocString NAME = "Denim Jacket";

						public static LocString DESC = "The top half of a Canadian tuxedo.";
					}

					public class GONCH_STRAWBERRY
					{
						public static LocString NAME = "Executive Undershirt";

						public static LocString DESC = "The breathable base layer every power suit needs.";
					}

					public class GONCH_SATSUMA
					{
						public static LocString NAME = "Underling Undershirt";

						public static LocString DESC = "Extra-absorbent fabric in the underarms to mop up nervous sweat.";
					}

					public class GONCH_LEMON
					{
						public static LocString NAME = "Groupthink Undershirt";

						public static LocString DESC = "Because the most popular choice is always the right choice.";
					}

					public class GONCH_LIME
					{
						public static LocString NAME = "Stakeholder Undershirt";

						public static LocString DESC = "Soft against the skin, for those who have skin in the game.";
					}

					public class GONCH_BLUEBERRY
					{
						public static LocString NAME = "Admin Undershirt";

						public static LocString DESC = "Criminally underappreciated.";
					}

					public class GONCH_GRAPE
					{
						public static LocString NAME = "Buzzword Undershirt";

						public static LocString DESC = "A value-added vest for touching base and thinking outside the box using best practices ASAP.";
					}

					public class GONCH_WATERMELON
					{
						public static LocString NAME = "Synergy Undershirt";

						public static LocString DESC = "Asking for it by name often triggers dramatic eye-rolls from bystanders.";
					}

					public class NERD_BROWN
					{
						public static LocString NAME = "Research Shirt";

						public static LocString DESC = "Comes with a thoughtfully chewed-up ballpoint pen.";
					}

					public class GI_WHITE
					{
						public static LocString NAME = "Rebel Gi Jacket";

						public static LocString DESC = "The contrasting trim hides stains from messy post-sparring snacks.";
					}

					public class JACKET_SMOKING_BURGUNDY
					{
						public static LocString NAME = "Donor Jacket";

						public static LocString DESC = "Crafted from the softest, most philanthropic fibers.";
					}

					public class MECHANIC
					{
						public static LocString NAME = "Engineer Jacket";

						public static LocString DESC = "Designed to withstand the rigors of applied science.";
					}

					public class VELOUR_BLACK
					{
						public static LocString NAME = "PhD Velour Jacket";

						public static LocString DESC = "A formal jacket for those who are \"not that kind of doctor.\"";
					}

					public class VELOUR_BLUE
					{
						public static LocString NAME = "Shortwave Velour Jacket";

						public static LocString DESC = "A luxe, pettable jacket paired with a clip-on tie.";
					}

					public class VELOUR_PINK
					{
						public static LocString NAME = "Gamma Velour Jacket";

						public static LocString DESC = "Some scientists are less shy than others.";
					}

					public class WAISTCOAT_PINSTRIPE_SLATE
					{
						public static LocString NAME = "Nobel Pinstripe Waistcoat";

						public static LocString DESC = "One must dress for the prize that one wishes to win.";
					}

					public class WATER
					{
						public static LocString NAME = "HVAC Khaki Shirt";

						public static LocString DESC = "Designed to regulate temperature and humidity.";
					}

					public class TWEED_PINK_ORCHID
					{
						public static LocString NAME = "Power Brunch Blazer";

						public static LocString DESC = "Winners never quit, quitters never win.";
					}

					public class DRESS_SLEEVELESS_BOW_BW
					{
						public static LocString NAME = "PhD Dress";

						public static LocString DESC = "Ready for a post-thesis-defense party.";
					}

					public class BODYSUIT_BALLERINA_PINK
					{
						public static LocString NAME = "Ballet Leotard";

						public static LocString DESC = "Lab-crafted fabric with a level of stretchiness that defies the laws of physics.";
					}

					public class SOCKSUIT_BEIGE
					{
						public static LocString NAME = "Vintage Sockshirt";

						public static LocString DESC = "Like a sock for the torso. With sleeves.";
					}

					public class X_SPORCHID
					{
						public static LocString NAME = "Sporefest Sweater";

						public static LocString DESC = "This soft knit can be worn anytime, not just during Zombie Spore season.";
					}

					public class X1_PINCHAPEPPERNUTBELLS
					{
						public static LocString NAME = "Pinchabell Jacket";

						public static LocString DESC = "The peppernuts jingle just loudly enough to be distracting.";
					}

					public class POMPOM_SHINEBUGS_PINK_PEPPERNUT
					{
						public static LocString NAME = "Pom Bug Sweater";

						public static LocString DESC = "No Shine Bugs were harmed in the making of this sweater.";
					}

					public class SNOWFLAKE_BLUE
					{
						public static LocString NAME = "Crystal-Iced Sweater";

						public static LocString DESC = "Tiny imperfections in the front pattern ensure that no two are truly identical.";
					}

					public class PJ_CLOVERS_GLITCH_KELLY
					{
						public static LocString NAME = "Lucky Jammies";

						public static LocString DESC = "Even the most brilliant minds need a little extra luck sometimes.";
					}

					public class PJ_HEARTS_CHILLI_STRAWBERRY
					{
						public static LocString NAME = "Sweetheart Jammies";

						public static LocString DESC = "Plush chenille fabric and a drool-absorbent collar? This sleepsuit really <i>is</i> \"The One.\"";
					}

					public class BUILDER
					{
						public static LocString NAME = "Hi-Vis Jacket";

						public static LocString DESC = "Unmissable style for the safety-minded.";
					}

					public class FLORAL_PINK
					{
						public static LocString NAME = "Downtime Shirt";

						public static LocString DESC = "For maxing and relaxing when errands are too taxing.";
					}

					public class GINCH_PINK_SALTROCK
					{
						public static LocString NAME = "Frilly Saltrock Undershirt";

						public static LocString DESC = "A seamless pink undershirt with laser-cut ruffles.";
					}

					public class GINCH_PURPLE_DUSKY
					{
						public static LocString NAME = "Frilly Dusk Undershirt";

						public static LocString DESC = "A seamless purple undershirt with laser-cut ruffles.";
					}

					public class GINCH_BLUE_BASIN
					{
						public static LocString NAME = "Frilly Basin Undershirt";

						public static LocString DESC = "A seamless blue undershirt with laser-cut ruffles.";
					}

					public class GINCH_TEAL_BALMY
					{
						public static LocString NAME = "Frilly Balm Undershirt";

						public static LocString DESC = "A seamless teal undershirt with laser-cut ruffles.";
					}

					public class GINCH_GREEN_LIME
					{
						public static LocString NAME = "Frilly Leach Undershirt";

						public static LocString DESC = "A seamless green undershirt with laser-cut ruffles.";
					}

					public class GINCH_YELLOW_YELLOWCAKE
					{
						public static LocString NAME = "Frilly Yellowcake Undershirt";

						public static LocString DESC = "A seamless yellow undershirt with laser-cut ruffles.";
					}

					public class GINCH_ORANGE_ATOMIC
					{
						public static LocString NAME = "Frilly Atomic Undershirt";

						public static LocString DESC = "A seamless orange undershirt with laser-cut ruffles.";
					}

					public class GINCH_RED_MAGMA
					{
						public static LocString NAME = "Frilly Magma Undershirt";

						public static LocString DESC = "A seamless red undershirt with laser-cut ruffles.";
					}

					public class GINCH_GREY_GREY
					{
						public static LocString NAME = "Frilly Slate Undershirt";

						public static LocString DESC = "A seamless grey undershirt with laser-cut ruffles.";
					}

					public class GINCH_GREY_CHARCOAL
					{
						public static LocString NAME = "Frilly Charcoal Undershirt";

						public static LocString DESC = "A seamless dark grey undershirt with laser-cut ruffles.";
					}

					public class KNIT_POLKADOT_TURQ
					{
						public static LocString NAME = "Polka Dot Track Jacket";

						public static LocString DESC = "The dots are infused with odor-neutralizing enzymes!";
					}

					public class FLASHY
					{
						public static LocString NAME = "Superstar Jacket";

						public static LocString DESC = "Some of us were not made to be subtle.";
					}
				}
			}

			public class CLOTHING_BOTTOMS
			{
				public static LocString NAME = "Default Bottom";

				public static LocString DESC = "The default bottoms.";

				public class FACADES
				{
					public class BASIC_BLUE_MIDDLE
					{
						public static LocString NAME = "Basic Aqua Pants";

						public static LocString DESC = "A clean pair of aqua-blue pants that go with everything.";
					}

					public class BASIC_PINK_ORCHID
					{
						public static LocString NAME = "Basic Bubblegum Pants";

						public static LocString DESC = "A clean pair of bubblegum-pink pants that go with everything.";
					}

					public class BASIC_GREEN
					{
						public static LocString NAME = "Basic Green Pants";

						public static LocString DESC = "A clean pair of green pants that go with everything.";
					}

					public class BASIC_ORANGE
					{
						public static LocString NAME = "Basic Orange Pants";

						public static LocString DESC = "A clean pair of orange pants that go with everything.";
					}

					public class BASIC_PURPLE
					{
						public static LocString NAME = "Basic Purple Pants";

						public static LocString DESC = "A clean pair of purple pants that go with everything.";
					}

					public class BASIC_RED
					{
						public static LocString NAME = "Basic Red Pants";

						public static LocString DESC = "A clean pair of red pants that go with everything.";
					}

					public class BASIC_WHITE
					{
						public static LocString NAME = "Basic White Pants";

						public static LocString DESC = "A clean pair of white pants that go with everything.";
					}

					public class BASIC_YELLOW
					{
						public static LocString NAME = "Basic Yellow Pants";

						public static LocString DESC = "A clean pair of yellow pants that go with everything.";
					}

					public class BASIC_BLACK
					{
						public static LocString NAME = "Basic Black Pants";

						public static LocString DESC = "A clean pair of black pants that go with everything.";
					}

					public class SHORTS_BASIC_DEEPRED
					{
						public static LocString NAME = "Team Captain Shorts";

						public static LocString DESC = "A fresh pair of shorts for natural leaders.";
					}

					public class SHORTS_BASIC_SATSUMA
					{
						public static LocString NAME = "Superfan Shorts";

						public static LocString DESC = "A fresh pair of shorts for long-time supporters of...shorts.";
					}

					public class SHORTS_BASIC_YELLOWCAKE
					{
						public static LocString NAME = "Yellowcake Shorts";

						public static LocString DESC = "A fresh pair of uranium-powder-colored shorts that are definitely not radioactive. Probably.";
					}

					public class SHORTS_BASIC_KELLYGREEN
					{
						public static LocString NAME = "Go Team Shorts";

						public static LocString DESC = "A fresh pair of shorts for cheering from the sidelines.";
					}

					public class SHORTS_BASIC_BLUE_COBALT
					{
						public static LocString NAME = "True Blue Shorts";

						public static LocString DESC = "A fresh pair of shorts for the real team players.";
					}

					public class SHORTS_BASIC_PINK_FLAMINGO
					{
						public static LocString NAME = "Pep Rally Shorts";

						public static LocString DESC = "The peppiest pair of shorts this side of the asteroid.";
					}

					public class SHORTS_BASIC_CHARCOAL
					{
						public static LocString NAME = "Underdog Shorts";

						public static LocString DESC = "A fresh pair of shorts. They're cleaner than they look.";
					}

					public class CIRCUIT_GREEN
					{
						public static LocString NAME = "LED Pants";

						public static LocString DESC = "These legs are lit.";
					}

					public class ATHLETE
					{
						public static LocString NAME = "Racing Pants";

						public static LocString DESC = "Fast, furious fashion.";
					}

					public class BASIC_LIGHTBROWN
					{
						public static LocString NAME = "Basic Khaki Pants";

						public static LocString DESC = "Transition effortlessly from subterranean day to subterranean night.";
					}

					public class BASIC_REDORANGE
					{
						public static LocString NAME = "Basic Crimson Pants";

						public static LocString DESC = "Like red pants, but slightly fancier-sounding.";
					}

					public class GONCH_STRAWBERRY
					{
						public static LocString NAME = "Executive Briefs";

						public static LocString DESC = "Bossy (under)pants.";
					}

					public class GONCH_SATSUMA
					{
						public static LocString NAME = "Underling Briefs";

						public static LocString DESC = "The seams are already unraveling.";
					}

					public class GONCH_LEMON
					{
						public static LocString NAME = "Groupthink Briefs";

						public static LocString DESC = "All the cool people are wearing them.";
					}

					public class GONCH_LIME
					{
						public static LocString NAME = "Stakeholder Briefs";

						public static LocString DESC = "They're really invested in keeping the wearer comfortable.";
					}

					public class GONCH_BLUEBERRY
					{
						public static LocString NAME = "Admin Briefs";

						public static LocString DESC = "The workhorse of the underwear world.";
					}

					public class GONCH_GRAPE
					{
						public static LocString NAME = "Buzzword Briefs";

						public static LocString DESC = "Underwear that works hard, plays hard, and gives 110% to maximize the \"bottom\" line.";
					}

					public class GONCH_WATERMELON
					{
						public static LocString NAME = "Synergy Briefs";

						public static LocString DESC = "Teamwork makes the dream work.";
					}

					public class DENIM_BLUE
					{
						public static LocString NAME = "Jeans";

						public static LocString DESC = "The bottom half of a Canadian tuxedo.";
					}

					public class GI_WHITE
					{
						public static LocString NAME = "White Capris";

						public static LocString DESC = "The cropped length is ideal for wading through flooded hallways.";
					}

					public class NERD_BROWN
					{
						public static LocString NAME = "Research Pants";

						public static LocString DESC = "The pockets are full of illegible notes that didn't quite survive the wash.";
					}

					public class SKIRT_BASIC_BLUE_MIDDLE
					{
						public static LocString NAME = "Aqua Rayon Skirt";

						public static LocString DESC = "The tag says \"Dry Clean Only.\" There are no dry cleaners in space.";
					}

					public class SKIRT_BASIC_PURPLE
					{
						public static LocString NAME = "Purple Rayon Skirt";

						public static LocString DESC = "It's not the most breathable fabric, but it <i>is</i> a lovely shade of purple.";
					}

					public class SKIRT_BASIC_GREEN
					{
						public static LocString NAME = "Olive Rayon Skirt";

						public static LocString DESC = "Designed not to get snagged on ladders.";
					}

					public class SKIRT_BASIC_ORANGE
					{
						public static LocString NAME = "Apricot Rayon Skirt";

						public static LocString DESC = "Ready for spontaneous workplace twirling.";
					}

					public class SKIRT_BASIC_PINK_ORCHID
					{
						public static LocString NAME = "Bubblegum Rayon Skirt";

						public static LocString DESC = "The bubblegum scent lasts 100 washes!";
					}

					public class SKIRT_BASIC_RED
					{
						public static LocString NAME = "Garnet Rayon Skirt";

						public static LocString DESC = "It's business time.";
					}

					public class SKIRT_BASIC_YELLOW
					{
						public static LocString NAME = "Yellow Rayon Skirt";

						public static LocString DESC = "A formerly white skirt that has not aged well.";
					}

					public class SKIRT_BASIC_POLKADOT
					{
						public static LocString NAME = "Polka Dot Skirt";

						public static LocString DESC = "Polka dots are a way to infinity.";
					}

					public class SKIRT_BASIC_WATERMELON
					{
						public static LocString NAME = "Picnic Skirt";

						public static LocString DESC = "The seeds are spittable, but will bear no fruit.";
					}

					public class SKIRT_DENIM_BLUE
					{
						public static LocString NAME = "Denim Tux Skirt";

						public static LocString DESC = "Designed for the casual red carpet.";
					}

					public class SKIRT_LEOPARD_PRINT_BLUE_PINK
					{
						public static LocString NAME = "Disco Leopard Skirt";

						public static LocString DESC = "A faux-fur party staple.";
					}

					public class SKIRT_SPARKLE_BLUE
					{
						public static LocString NAME = "Blue Tinsel Skirt";

						public static LocString DESC = "The tinsel is scratchy, but look how shiny!";
					}

					public class BASIC_ORANGE_SATSUMA
					{
						public static LocString NAME = "Hi-Vis Pants";

						public static LocString DESC = "They make the wearer feel truly seen.";
					}

					public class PINSTRIPE_SLATE
					{
						public static LocString NAME = "Nobel Pinstripe Trousers";

						public static LocString DESC = "There's a waterproof pocket to keep acceptance speeches smudge-free.";
					}

					public class VELOUR_BLACK
					{
						public static LocString NAME = "Black Velour Trousers";

						public static LocString DESC = "Fuzzy, formal and finely cut.";
					}

					public class VELOUR_BLUE
					{
						public static LocString NAME = "Shortwave Velour Pants";

						public static LocString DESC = "Formal wear with a sensory side.";
					}

					public class VELOUR_PINK
					{
						public static LocString NAME = "Gamma Velour Pants";

						public static LocString DESC = "They're stretchy <i>and</i> flame retardant.";
					}

					public class SKIRT_BALLERINA_PINK
					{
						public static LocString NAME = "Ballet Tutu";

						public static LocString DESC = "A tulle skirt spun and assembled by an army of patent-pending nanobots.";
					}

					public class SKIRT_TWEED_PINK_ORCHID
					{
						public static LocString NAME = "Power Brunch Skirt";

						public static LocString DESC = "It has pockets!";
					}

					public class GINCH_PINK_GLUON
					{
						public static LocString NAME = "Gluon Shorties";

						public static LocString DESC = "Comfy pink short-shorts with a ruffled hem.";
					}

					public class GINCH_PURPLE_CORTEX
					{
						public static LocString NAME = "Cortex Shorties";

						public static LocString DESC = "Comfy purple short-shorts with a ruffled hem.";
					}

					public class GINCH_BLUE_FROSTY
					{
						public static LocString NAME = "Frosty Shorties";

						public static LocString DESC = "Icy blue short-shorts with a ruffled hem.";
					}

					public class GINCH_TEAL_LOCUS
					{
						public static LocString NAME = "Locus Shorties";

						public static LocString DESC = "Comfy teal short-shorts with a ruffled hem.";
					}

					public class GINCH_GREEN_GOOP
					{
						public static LocString NAME = "Goop Shorties";

						public static LocString DESC = "Short-shorts with a ruffled hem and one pocket full of melted snacks.";
					}

					public class GINCH_YELLOW_BILE
					{
						public static LocString NAME = "Bile Shorties";

						public static LocString DESC = "Ruffled short-shorts in a stomach-turning shade of yellow.";
					}

					public class GINCH_ORANGE_NYBBLE
					{
						public static LocString NAME = "Nybble Shorties";

						public static LocString DESC = "Comfy orange ruffled short-shorts for computer scientists.";
					}

					public class GINCH_RED_IRONBOW
					{
						public static LocString NAME = "Ironbow Shorties";

						public static LocString DESC = "Comfy red short-shorts with a ruffled hem.";
					}

					public class GINCH_GREY_PHLEGM
					{
						public static LocString NAME = "Phlegmy Shorties";

						public static LocString DESC = "Ruffled short-shorts in a rather sticky shade of light grey.";
					}

					public class GINCH_GREY_OBELUS
					{
						public static LocString NAME = "Obelus Shorties";

						public static LocString DESC = "Comfy grey short-shorts with a ruffled hem.";
					}

					public class KNIT_POLKADOT_TURQ
					{
						public static LocString NAME = "Polka Dot Track Pants";

						public static LocString DESC = "For clowning around during mandatory physical fitness week.";
					}

					public class GI_BELT_WHITE_BLACK
					{
						public static LocString NAME = "Rebel Gi Pants";

						public static LocString DESC = "Relaxed-fit pants designed for roundhouse kicks.";
					}

					public class BELT_KHAKI_TAN
					{
						public static LocString NAME = "HVAC Khaki Pants";

						public static LocString DESC = "Rip-resistant fabric makes crawling through ducts a breeze.";
					}
				}
			}

			public class CLOTHING_SHOES
			{
				public static LocString NAME = "Default Footwear";

				public static LocString DESC = "The default style of footwear.";

				public class FACADES
				{
					public class BASIC_BLUE_MIDDLE
					{
						public static LocString NAME = "Basic Aqua Shoes";

						public static LocString DESC = "A fresh pair of aqua-blue shoes that go with everything.";
					}

					public class BASIC_PINK_ORCHID
					{
						public static LocString NAME = "Basic Bubblegum Shoes";

						public static LocString DESC = "A fresh pair of bubblegum-pink shoes that go with everything.";
					}

					public class BASIC_GREEN
					{
						public static LocString NAME = "Basic Green Shoes";

						public static LocString DESC = "A fresh pair of green shoes that go with everything.";
					}

					public class BASIC_ORANGE
					{
						public static LocString NAME = "Basic Orange Shoes";

						public static LocString DESC = "A fresh pair of orange shoes that go with everything.";
					}

					public class BASIC_PURPLE
					{
						public static LocString NAME = "Basic Purple Shoes";

						public static LocString DESC = "A fresh pair of purple shoes that go with everything.";
					}

					public class BASIC_RED
					{
						public static LocString NAME = "Basic Red Shoes";

						public static LocString DESC = "A fresh pair of red shoes that go with everything.";
					}

					public class BASIC_WHITE
					{
						public static LocString NAME = "Basic White Shoes";

						public static LocString DESC = "A fresh pair of white shoes that go with everything.";
					}

					public class BASIC_YELLOW
					{
						public static LocString NAME = "Basic Yellow Shoes";

						public static LocString DESC = "A fresh pair of yellow shoes that go with everything.";
					}

					public class BASIC_BLACK
					{
						public static LocString NAME = "Basic Black Shoes";

						public static LocString DESC = "A fresh pair of black shoes that go with everything.";
					}

					public class BASIC_BLUEGREY
					{
						public static LocString NAME = "Basic Gunmetal Shoes";

						public static LocString DESC = "A fresh pair of pastel shoes that go with everything.";
					}

					public class BASIC_TAN
					{
						public static LocString NAME = "Basic Tan Shoes";

						public static LocString DESC = "They're remarkably unremarkable.";
					}

					public class SOCKS_ATHLETIC_DEEPRED
					{
						public static LocString NAME = "Team Captain Gym Socks";

						public static LocString DESC = "Breathable socks with sporty red stripes.";
					}

					public class SOCKS_ATHLETIC_SATSUMA
					{
						public static LocString NAME = "Superfan Gym Socks";

						public static LocString DESC = "Breathable socks with sporty orange stripes.";
					}

					public class SOCKS_ATHLETIC_LEMON
					{
						public static LocString NAME = "Hype Gym Socks";

						public static LocString DESC = "Breathable socks with sporty yellow stripes.";
					}

					public class SOCKS_ATHLETIC_KELLYGREEN
					{
						public static LocString NAME = "Go Team Gym Socks";

						public static LocString DESC = "Breathable socks with sporty green stripes.";
					}

					public class SOCKS_ATHLETIC_COBALT
					{
						public static LocString NAME = "True Blue Gym Socks";

						public static LocString DESC = "Breathable socks with sporty blue stripes.";
					}

					public class SOCKS_ATHLETIC_FLAMINGO
					{
						public static LocString NAME = "Pep Rally Gym Socks";

						public static LocString DESC = "Breathable socks with sporty pink stripes.";
					}

					public class SOCKS_ATHLETIC_CHARCOAL
					{
						public static LocString NAME = "Underdog Gym Socks";

						public static LocString DESC = "Breathable socks that do nothing whatsoever to eliminate foot odor.";
					}

					public class BASIC_GREY
					{
						public static LocString NAME = "Basic Gray Shoes";

						public static LocString DESC = "A fresh pair of gray shoes that go with everything.";
					}

					public class DENIM_BLUE
					{
						public static LocString NAME = "Denim Shoes";

						public static LocString DESC = "Not technically essential for a Canadian tuxedo, but why not?";
					}

					public class LEGWARMERS_STRAWBERRY
					{
						public static LocString NAME = "Slouchy Strawberry Socks";

						public static LocString DESC = "Freckly knitted socks that don't stay up.";
					}

					public class LEGWARMERS_SATSUMA
					{
						public static LocString NAME = "Slouchy Satsuma Socks";

						public static LocString DESC = "Sweet knitted socks for spontaneous dance segments.";
					}

					public class LEGWARMERS_LEMON
					{
						public static LocString NAME = "Slouchy Lemon Socks";

						public static LocString DESC = "Zesty knitted socks that don't stay up.";
					}

					public class LEGWARMERS_LIME
					{
						public static LocString NAME = "Slouchy Lime Socks";

						public static LocString DESC = "Juicy knitted socks that don't stay up.";
					}

					public class LEGWARMERS_BLUEBERRY
					{
						public static LocString NAME = "Slouchy Blueberry Socks";

						public static LocString DESC = "Knitted socks with a fun bobble-stitch texture.";
					}

					public class LEGWARMERS_GRAPE
					{
						public static LocString NAME = "Slouchy Grape Socks";

						public static LocString DESC = "These fabulous knitted socks that don't stay up are really raisin the bar.";
					}

					public class LEGWARMERS_WATERMELON
					{
						public static LocString NAME = "Slouchy Watermelon Socks";

						public static LocString DESC = "Summery knitted socks that don't stay up.";
					}

					public class BALLERINA_PINK
					{
						public static LocString NAME = "Ballet Shoes";

						public static LocString DESC = "There's no \"pointe\" in aiming for anything less than perfection.";
					}

					public class MARYJANE_SOCKS_BW
					{
						public static LocString NAME = "Frilly Sock Shoes";

						public static LocString DESC = "They add a little <i>je ne sais quoi</i> to everyday lab wear.";
					}

					public class CLASSICFLATS_CREAM_CHARCOAL
					{
						public static LocString NAME = "Dressy Shoes";

						public static LocString DESC = "An enduring style, for enduring endless small talk.";
					}

					public class VELOUR_BLUE
					{
						public static LocString NAME = "Shortwave Velour Shoes";

						public static LocString DESC = "Not the easiest to keep clean.";
					}

					public class VELOUR_PINK
					{
						public static LocString NAME = "Gamma Velour Shoes";

						public static LocString DESC = "Finally, a pair of work-appropriate fuzzy shoes.";
					}

					public class VELOUR_BLACK
					{
						public static LocString NAME = "Black Velour Shoes";

						public static LocString DESC = "Matching velour lining gently tickles feet with every step.";
					}

					public class FLASHY
					{
						public static LocString NAME = "Superstar Shoes";

						public static LocString DESC = "Why walk when you can <i>moon</i>walk?";
					}

					public class GINCH_PINK_SALTROCK
					{
						public static LocString NAME = "Frilly Saltrock Socks";

						public static LocString DESC = "Thick, soft pink socks with extra flounce.";
					}

					public class GINCH_PURPLE_DUSKY
					{
						public static LocString NAME = "Frilly Dusk Socks";

						public static LocString DESC = "Thick, soft purple socks with extra flounce.";
					}

					public class GINCH_BLUE_BASIN
					{
						public static LocString NAME = "Frilly Basin Socks";

						public static LocString DESC = "Thick, soft blue socks with extra flounce.";
					}

					public class GINCH_TEAL_BALMY
					{
						public static LocString NAME = "Frilly Balm Socks";

						public static LocString DESC = "Thick, soothing teal socks with extra flounce.";
					}

					public class GINCH_GREEN_LIME
					{
						public static LocString NAME = "Frilly Leach Socks";

						public static LocString DESC = "Thick, soft green socks with extra flounce.";
					}

					public class GINCH_YELLOW_YELLOWCAKE
					{
						public static LocString NAME = "Frilly Yellowcake Socks";

						public static LocString DESC = "Dangerously soft yellow socks with extra flounce.";
					}

					public class GINCH_ORANGE_ATOMIC
					{
						public static LocString NAME = "Frilly Atomic Socks";

						public static LocString DESC = "Thick, soft orange socks with extra flounce.";
					}

					public class GINCH_RED_MAGMA
					{
						public static LocString NAME = "Frilly Magma Socks";

						public static LocString DESC = "Thick, toasty red socks with extra flounce.";
					}

					public class GINCH_GREY_GREY
					{
						public static LocString NAME = "Frilly Slate Socks";

						public static LocString DESC = "Thick, soft grey socks with extra flounce.";
					}

					public class GINCH_GREY_CHARCOAL
					{
						public static LocString NAME = "Frilly Charcoal Socks";

						public static LocString DESC = "Thick, soft dark grey socks with extra flounce.";
					}
				}
			}

			public class CLOTHING_HATS
			{
				public static LocString NAME = "Default Headgear";

				public static LocString DESC = "<DESC>";

				public class FACADES
				{
				}
			}

			public class CLOTHING_ACCESORIES
			{
				public static LocString NAME = "Default Accessory";

				public static LocString DESC = "<DESC>";

				public class FACADES
				{
				}
			}

			public class OXYGEN_TANK
			{
				public static LocString NAME = UI.FormatAsLink("Oxygen Tank", "OXYGEN_TANK");

				public static LocString GENERICNAME = "Equipment";

				public static LocString DESC = "It's like a to-go bag for your lungs.";

				public static LocString EFFECT = "Allows Duplicants to breathe in hazardous environments.\n\nDoes not work when submerged in <style=\"liquid\">Liquid</style>.";

				public static LocString RECIPE_DESC = "Allows Duplicants to breathe in hazardous environments.\n\nDoes not work when submerged in <style=\"liquid\">Liquid</style>";
			}

			public class OXYGEN_TANK_UNDERWATER
			{
				public static LocString NAME = "Oxygen Rebreather";

				public static LocString GENERICNAME = "Equipment";

				public static LocString DESC = "";

				public static LocString EFFECT = "Allows Duplicants to breathe while submerged in <style=\"liquid\">Liquid</style>.\n\nDoes not work outside of liquid.";

				public static LocString RECIPE_DESC = "Allows Duplicants to breathe while submerged in <style=\"liquid\">Liquid</style>.\n\nDoes not work outside of liquid";
			}

			public class EQUIPPABLEBALLOON
			{
				public static LocString NAME = UI.FormatAsLink("Balloon Friend", "EQUIPPABLEBALLOON");

				public static LocString DESC = "A floating friend to reassure my Duplicants they are so very, very clever.";

				public static LocString EFFECT = "Gives Duplicants a boost in brain function.\n\nSupplied by Duplicants with the Balloon Artist " + UI.FormatAsLink("Overjoyed", "MORALE") + " response.";

				public static LocString RECIPE_DESC = "Gives Duplicants a boost in brain function.\n\nSupplied by Duplicants with the Balloon Artist " + UI.FormatAsLink("Overjoyed", "MORALE") + " response";

				public static LocString GENERICNAME = "Balloon Friend";

				public class FACADES
				{
					public class DEFAULT_BALLOON
					{
						public static LocString NAME = UI.FormatAsLink("Balloon Friend", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A floating friend to reassure my Duplicants that they are so very, very clever.";
					}

					public class BALLOON_FIREENGINE_LONG_SPARKLES
					{
						public static LocString NAME = UI.FormatAsLink("Magma Glitter", "EQUIPPABLEBALLOON");

						public static LocString DESC = "They float <i>and</i> sparkle!";
					}

					public class BALLOON_YELLOW_LONG_SPARKLES
					{
						public static LocString NAME = UI.FormatAsLink("Lavatory Glitter", "EQUIPPABLEBALLOON");

						public static LocString DESC = "Sparkly balloons in an all-too-familiar hue.";
					}

					public class BALLOON_BLUE_LONG_SPARKLES
					{
						public static LocString NAME = UI.FormatAsLink("Wheezewort Glitter", "EQUIPPABLEBALLOON");

						public static LocString DESC = "They float <i>and</i> sparkle!";
					}

					public class BALLOON_GREEN_LONG_SPARKLES
					{
						public static LocString NAME = UI.FormatAsLink("Mush Bar Glitter", "EQUIPPABLEBALLOON");

						public static LocString DESC = "They float <i>and</i> sparkle!";
					}

					public class BALLOON_PINK_LONG_SPARKLES
					{
						public static LocString NAME = UI.FormatAsLink("Petal Glitter", "EQUIPPABLEBALLOON");

						public static LocString DESC = "They float <i>and</i> sparkle!";
					}

					public class BALLOON_PURPLE_LONG_SPARKLES
					{
						public static LocString NAME = UI.FormatAsLink("Dusky Glitter", "EQUIPPABLEBALLOON");

						public static LocString DESC = "They float <i>and</i> sparkle!";
					}

					public class BALLOON_BABY_PACU_EGG
					{
						public static LocString NAME = UI.FormatAsLink("Floatie Fish", "EQUIPPABLEBALLOON");

						public static LocString DESC = "They do not taste as good as the real thing.";
					}

					public class BALLOON_BABY_GLOSSY_DRECKO_EGG
					{
						public static LocString NAME = UI.FormatAsLink("Glossy Glee", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A happy little trio of inflatable critters.";
					}

					public class BALLOON_BABY_HATCH_EGG
					{
						public static LocString NAME = UI.FormatAsLink("Helium Hatches", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A happy little trio of inflatable critters.";
					}

					public class BALLOON_BABY_POKESHELL_EGG
					{
						public static LocString NAME = UI.FormatAsLink("Peppy Pokeshells", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A happy little trio of inflatable critters.";
					}

					public class BALLOON_BABY_PUFT_EGG
					{
						public static LocString NAME = UI.FormatAsLink("Puffed-Up Pufts", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A happy little trio of inflatable critters.";
					}

					public class BALLOON_BABY_SHOVOLE_EGG
					{
						public static LocString NAME = UI.FormatAsLink("Voley Voley Voles", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A happy little trio of inflatable critters.";
					}

					public class BALLOON_BABY_PIP_EGG
					{
						public static LocString NAME = UI.FormatAsLink("Pip Pip Hooray", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A happy little trio of inflatable critters.";
					}

					public class CANDY_BLUEBERRY
					{
						public static LocString NAME = UI.FormatAsLink("Candied Blueberry", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A juicy bunch of blueberry-scented balloons.";
					}

					public class CANDY_GRAPE
					{
						public static LocString NAME = UI.FormatAsLink("Candied Grape", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A juicy bunch of grape-scented balloons.";
					}

					public class CANDY_LEMON
					{
						public static LocString NAME = UI.FormatAsLink("Candied Lemon", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A juicy lemon-scented bunch of balloons.";
					}

					public class CANDY_LIME
					{
						public static LocString NAME = UI.FormatAsLink("Candied Lime", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A juicy lime-scented bunch of balloons.";
					}

					public class CANDY_ORANGE
					{
						public static LocString NAME = UI.FormatAsLink("Candied Satsuma", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A juicy satsuma-scented bunch of balloons.";
					}

					public class CANDY_STRAWBERRY
					{
						public static LocString NAME = UI.FormatAsLink("Candied Strawberry", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A juicy strawberry-scented bunch of balloons.";
					}

					public class CANDY_WATERMELON
					{
						public static LocString NAME = UI.FormatAsLink("Candied Watermelon", "EQUIPPABLEBALLOON");

						public static LocString DESC = "A juicy watermelon-scented bunch of balloons.";
					}

					public class HAND_GOLD
					{
						public static LocString NAME = UI.FormatAsLink("Gold Fingers", "EQUIPPABLEBALLOON");

						public static LocString DESC = "Inflatable gestures of encouragement.";
					}
				}
			}

			public class SLEEPCLINICPAJAMAS
			{
				public static LocString NAME = UI.FormatAsLink("Pajamas", "SLEEP_CLINIC_PAJAMAS");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "A soft, fleecy ticket to dreamland.";

				public static LocString EFFECT = "Helps Duplicants fall asleep by reducing " + UI.FormatAsLink("Stamina", "HEALTH") + ".\n\nEnables the wearer to dream and produce Dream Journals.";

				public static LocString DESTROY_TOAST = "Ripped Pajamas";
			}
		}
	}
}
