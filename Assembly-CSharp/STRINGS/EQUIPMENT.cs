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

				public static LocString EFFECT = "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in toxic and low breathability environments.\n\nMust be refilled with oxygen at an " + UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER") + " when depleted.";

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

				public static LocString EFFECT = "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in toxic and low breathability environments.\n\nMust be refilled with oxygen at an " + UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER") + " when depleted.";

				public static LocString RECIPE_DESC = "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in toxic and low breathability environments.";

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
				public static LocString NAME = UI.FormatAsLink("Warm Sweater", "WARM_VEST");

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
					public static LocString CLUBSHIRT = UI.FormatAsLink("Purple Polyester", "CUSTOMCLOTHING");

					public static LocString CUMMERBUND = UI.FormatAsLink("Classic Cummerbund", "CUSTOMCLOTHING");

					public static LocString DECOR_02 = UI.FormatAsLink("Snazzier Red Suit", "CUSTOMCLOTHING");

					public static LocString DECOR_03 = UI.FormatAsLink("Snazzier Blue Suit", "CUSTOMCLOTHING");

					public static LocString DECOR_04 = UI.FormatAsLink("Snazzier Green Suit", "CUSTOMCLOTHING");

					public static LocString DECOR_05 = UI.FormatAsLink("Snazzier Violet Suit", "CUSTOMCLOTHING");

					public static LocString GAUDYSWEATER = UI.FormatAsLink("Pompom Knit", "CUSTOMCLOTHING");

					public static LocString LIMONE = UI.FormatAsLink("Citrus Spandex", "CUSTOMCLOTHING");

					public static LocString MONDRIAN = UI.FormatAsLink("Cubist Knit", "CUSTOMCLOTHING");

					public static LocString OVERALLS = UI.FormatAsLink("Spiffy Overalls", "CUSTOMCLOTHING");

					public static LocString TRIANGLES = UI.FormatAsLink("Confetti Suit", "CUSTOMCLOTHING");

					public static LocString WORKOUT = UI.FormatAsLink("Pink Unitard", "CUSTOMCLOTHING");
				}
			}

			public class CLOTHING_GLOVES
			{
				public static LocString NAME = UI.FormatAsLink("Gloves", "CLOTHING_GLOVES");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "Testing desc for gloves skins";

				public static LocString EFFECT = "Testing effect for gloves skins";

				public static LocString RECIPE_DESC = "Testing recipe desc for gloves skins";

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
				}
			}

			public class CLOTHING_TOPS
			{
				public static LocString NAME = UI.FormatAsLink("Tops", "CLOTHING_TOPS");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "Testing desc for tops skins";

				public static LocString EFFECT = "Testing effect for tops skins";

				public static LocString RECIPE_DESC = "Testing recipe desc for tops skins";

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
				}
			}

			public class CLOTHING_BOTTOMS
			{
				public static LocString NAME = UI.FormatAsLink("Bottoms", "CLOTHING_BOTTOMS");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "Testing desc for bottoms skins";

				public static LocString EFFECT = "Testing effect for bottoms skins";

				public static LocString RECIPE_DESC = "Testing recipe desc for bottoms skins";

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
				}
			}

			public class CLOTHING_SHOES
			{
				public static LocString NAME = UI.FormatAsLink("Shoes", "CLOTHING_SHOES");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "Testing desc for shoes skins";

				public static LocString EFFECT = "Testing effect for shoes skins";

				public static LocString RECIPE_DESC = "Testing recipe desc for shoes skins";

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
				}
			}

			public class SLEEPCLINICPAJAMAS
			{
				public static LocString NAME = UI.FormatAsLink("Pajamas", "SLEEP_CLINIC_PAJAMAS");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "A soft, fleecy ticket to dreamland.";

				public static LocString EFFECT = string.Concat(new string[]
				{
					"Helps Duplicants fall asleep by reducing ",
					UI.FormatAsLink("Stamina", "STAMINA"),
					".\n\nEnables the wearer to dream and produce ",
					UI.FormatAsLink("Dream Journals", "DREAMJOURNAL"),
					"."
				});

				public static LocString DESTROY_TOAST = "Ripped Pajamas";
			}
		}
	}
}
