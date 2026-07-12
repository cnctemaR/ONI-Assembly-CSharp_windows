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

				public static LocString REPAIR_WORN_RECIPE_NAME = "Repair" + EQUIPMENT.PREFABS.ATMO_SUIT.NAME;

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
						public static LocString NAME = UI.FormatAsLink("Basic Aqua Gloves", "BASIC_BLUE_MIDDLE");

						public static LocString DESC = "A good, solid pair of aqua-blue gloves that go with everything.";
					}

					public class BASIC_YELLOW
					{
						public static LocString NAME = UI.FormatAsLink("Basic Yellow Gloves", "BASIC_YELLOW");

						public static LocString DESC = "A good, solid pair of yellow gloves that go with everything.";
					}

					public class BASIC_BLACK
					{
						public static LocString NAME = UI.FormatAsLink("Basic Black Gloves", "BASIC_BLACK");

						public static LocString DESC = "A good, solid pair of black gloves that go with everything.";
					}

					public class BASIC_PINK_ORCHID
					{
						public static LocString NAME = UI.FormatAsLink("Basic Bubblegum Gloves", "BASIC_PINK_ORCHID");

						public static LocString DESC = "A good, solid pair of bubblegum-pink gloves that go with everything.";
					}

					public class BASIC_GREEN
					{
						public static LocString NAME = UI.FormatAsLink("Basic Green Gloves", "BASIC_GREEN");

						public static LocString DESC = "A good, solid pair of green gloves that go with everything.";
					}

					public class BASIC_ORANGE
					{
						public static LocString NAME = UI.FormatAsLink("Basic Orange Gloves", "BASIC_ORANGE");

						public static LocString DESC = "A good, solid pair of orange gloves that go with everything.";
					}

					public class BASIC_PURPLE
					{
						public static LocString NAME = UI.FormatAsLink("Basic Purple Gloves", "BASIC_PURPLE");

						public static LocString DESC = "A good, solid pair of purple gloves that go with everything.";
					}

					public class BASIC_RED
					{
						public static LocString NAME = UI.FormatAsLink("Basic Red Gloves", "BASIC_RED");

						public static LocString DESC = "A good, solid pair of red gloves that go with everything.";
					}

					public class BASIC_WHITE
					{
						public static LocString NAME = UI.FormatAsLink("Basic White Gloves", "BASIC_WHITE");

						public static LocString DESC = "A good, solid pair of white gloves that go with everything.";
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
						public static LocString NAME = UI.FormatAsLink("Basic Aqua Shirt", "BASIC_BLUE_MIDDLE");

						public static LocString DESC = "A nice aqua-blue shirt that goes with everything.";
					}

					public class BASIC_BLACK
					{
						public static LocString NAME = UI.FormatAsLink("Basic Black Shirt", "BASIC_BLACK");

						public static LocString DESC = "A nice black shirt that goes with everything.";
					}

					public class BASIC_PINK_ORCHID
					{
						public static LocString NAME = UI.FormatAsLink("Basic Bubblegum Shirt", "BASIC_PINK_ORCHID");

						public static LocString DESC = "A nice bubblegum-pink shirt that goes with everything.";
					}

					public class BASIC_GREEN
					{
						public static LocString NAME = UI.FormatAsLink("Basic Green Shirt", "BASIC_GREEN");

						public static LocString DESC = "A nice green shirt that goes with everything.";
					}

					public class BASIC_ORANGE
					{
						public static LocString NAME = UI.FormatAsLink("Basic Orange Shirt", "BASIC_ORANGE");

						public static LocString DESC = "A nice orange shirt that goes with everything.";
					}

					public class BASIC_PURPLE
					{
						public static LocString NAME = UI.FormatAsLink("Basic Purple Shirt", "BASIC_PURPLE");

						public static LocString DESC = "A nice purple shirt that goes with everything.";
					}

					public class BASIC_RED_BURNT
					{
						public static LocString NAME = UI.FormatAsLink("Basic Red Shirt", "BASIC_RED_BURNT");

						public static LocString DESC = "A nice red shirt that goes with everything.";
					}

					public class BASIC_WHITE
					{
						public static LocString NAME = UI.FormatAsLink("Basic White Shirt", "BASIC_WHITE");

						public static LocString DESC = "A nice white shirt that goes with everything.";
					}

					public class BASIC_YELLOW
					{
						public static LocString NAME = UI.FormatAsLink("Basic Yellow Shirt", "BASIC_YELLOW");

						public static LocString DESC = "A nice yellow shirt that goes with everything.";
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
						public static LocString NAME = UI.FormatAsLink("Basic Aqua Pants", "BASIC_BLUE_MIDDLE");

						public static LocString DESC = "A clean pair of aqua-blue pants that go with everything.";
					}

					public class BASIC_PINK_ORCHID
					{
						public static LocString NAME = UI.FormatAsLink("Basic Bubblegum Pants", "BASIC_PINK_ORCHID");

						public static LocString DESC = "A clean pair of bubblegum-pink pants that go with everything.";
					}

					public class BASIC_GREEN
					{
						public static LocString NAME = UI.FormatAsLink("Basic Green Pants", "BASIC_GREEN");

						public static LocString DESC = "A clean pair of green pants that go with everything.";
					}

					public class BASIC_ORANGE
					{
						public static LocString NAME = UI.FormatAsLink("Basic Orange Pants", "BASIC_ORANGE");

						public static LocString DESC = "A clean pair of orange pants that go with everything.";
					}

					public class BASIC_PURPLE
					{
						public static LocString NAME = UI.FormatAsLink("Basic Purple Pants", "BASIC_PURPLE");

						public static LocString DESC = "A clean pair of purple pants that go with everything.";
					}

					public class BASIC_RED
					{
						public static LocString NAME = UI.FormatAsLink("Basic Red Pants", "BASIC_RED");

						public static LocString DESC = "A clean pair of red pants that go with everything.";
					}

					public class BASIC_WHITE
					{
						public static LocString NAME = UI.FormatAsLink("Basic White Pants", "BASIC_WHITE");

						public static LocString DESC = "A clean pair of white pants that go with everything.";
					}

					public class BASIC_YELLOW
					{
						public static LocString NAME = UI.FormatAsLink("Basic Yellow Pants", "BASIC_YELLOW");

						public static LocString DESC = "A clean pair of yellow pants that go with everything.";
					}

					public class BASIC_BLACK
					{
						public static LocString NAME = UI.FormatAsLink("Basic Black Pants", "BASIC_BLACK");

						public static LocString DESC = "A clean pair of black pants that go with everything.";
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
						public static LocString NAME = UI.FormatAsLink("Basic Aqua Shoes", "BASIC_BLUE_MIDDLE");

						public static LocString DESC = "A fresh pair of aqua-blue shoes that go with everything.";
					}

					public class BASIC_PINK_ORCHID
					{
						public static LocString NAME = UI.FormatAsLink("Basic Bubblegum Shoes", "BASIC_PINK_ORCHID");

						public static LocString DESC = "A fresh pair of bubblegum-pink shoes that go with everything.";
					}

					public class BASIC_GREEN
					{
						public static LocString NAME = UI.FormatAsLink("Basic Green Shoes", "BASIC_GREEN");

						public static LocString DESC = "A fresh pair of green shoes that go with everything.";
					}

					public class BASIC_ORANGE
					{
						public static LocString NAME = UI.FormatAsLink("Basic Orange Shoes", "BASIC_ORANGE");

						public static LocString DESC = "A fresh pair of orange shoes that go with everything.";
					}

					public class BASIC_PURPLE
					{
						public static LocString NAME = UI.FormatAsLink("Basic Purple Shoes", "BASIC_PURPLE");

						public static LocString DESC = "A fresh pair of purple shoes that go with everything.";
					}

					public class BASIC_RED
					{
						public static LocString NAME = UI.FormatAsLink("Basic Red Shoes", "BASIC_RED");

						public static LocString DESC = "A fresh pair of red shoes that go with everything.";
					}

					public class BASIC_WHITE
					{
						public static LocString NAME = UI.FormatAsLink("Basic White Shoes", "BASIC_WHITE");

						public static LocString DESC = "A fresh pair of white shoes that go with everything.";
					}

					public class BASIC_YELLOW
					{
						public static LocString NAME = UI.FormatAsLink("Basic Yellow Shoes", "BASIC_YELLOW");

						public static LocString DESC = "A fresh pair of yellow shoes that go with everything.";
					}

					public class BASIC_BLACK
					{
						public static LocString NAME = UI.FormatAsLink("Basic Black Shoes", "BASIC_BLACK");

						public static LocString DESC = "A fresh pair of black shoes that go with everything.";
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
