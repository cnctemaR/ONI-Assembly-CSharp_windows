using System;

namespace STRINGS
{
	public class CREATURES
	{
		public static LocString BAGGED_NAME_FMT = "Bagged {0}";

		public static LocString BAGGED_DESC_FMT = "This {0} has been captured and is now safe to relocate.";

		public class FAMILY
		{
			public static LocString HATCH = UI.FormatAsLink("Hatch", "HATCHSPECIES");

			public static LocString LIGHTBUG = UI.FormatAsLink("Shine Bug", "LIGHTBUGSPECIES");

			public static LocString OILFLOATER = UI.FormatAsLink("Slickster", "OILFLOATERSPECIES");

			public static LocString DRECKO = UI.FormatAsLink("Drecko", "DRECKOSPECIES");

			public static LocString GLOM = UI.FormatAsLink("Morb", "GLOMSPECIES");

			public static LocString PUFT = UI.FormatAsLink("Puft", "PUFTSPECIES");

			public static LocString PACU = UI.FormatAsLink("Pacu", "PACUSPECIES");

			public static LocString MOO = UI.FormatAsLink("Moo", "MOOSPECIES");

			public static LocString MOLE = UI.FormatAsLink("Shove Vole", "MOLESPECIES");

			public static LocString SQUIRREL = UI.FormatAsLink("Pip", "SQUIRRELSPECIES");

			public static LocString CRAB = UI.FormatAsLink("Pokeshell", "CRABSPECIES");

			public static LocString STATERPILLAR = UI.FormatAsLink("Plug Slug", "STATERPILLARSPECIES");

			public static LocString DIVERGENTSPECIES = UI.FormatAsLink("Divergent", "DIVERGENTSPECIES");

			public static LocString SWEEPBOT = UI.FormatAsLink("Sweepies", "SWEEPBOT");

			public static LocString SCOUTROVER = UI.FormatAsLink("Rover", "SCOUTROVER");

			public static LocString WOODDEER = UI.FormatAsLink("Flox", "DEERSPECIES");

			public static LocString SEAL = UI.FormatAsLink("Spigot Seal", "SEALSPECIES");

			public static LocString ICEBELLY = UI.FormatAsLink("Bammoth", "BELLYSPECIES");
		}

		public class FAMILY_PLURAL
		{
			public static LocString HATCHSPECIES = UI.FormatAsLink("Hatches", "HATCHSPECIES");

			public static LocString LIGHTBUGSPECIES = UI.FormatAsLink("Shine Bugs", "LIGHTBUGSPECIES");

			public static LocString OILFLOATERSPECIES = UI.FormatAsLink("Slicksters", "OILFLOATERSPECIES");

			public static LocString DRECKOSPECIES = UI.FormatAsLink("Dreckos", "DRECKOSPECIES");

			public static LocString GLOMSPECIES = UI.FormatAsLink("Morbs", "GLOMSPECIES");

			public static LocString PUFTSPECIES = UI.FormatAsLink("Pufts", "PUFTSPECIES");

			public static LocString PACUSPECIES = UI.FormatAsLink("Pacus", "PACUSPECIES");

			public static LocString MOOSPECIES = UI.FormatAsLink("Moos", "MOOSPECIES");

			public static LocString MOLESPECIES = UI.FormatAsLink("Shove Voles", "MOLESPECIES");

			public static LocString CRABSPECIES = UI.FormatAsLink("Pokeshells", "CRABSPECIES");

			public static LocString SQUIRRELSPECIES = UI.FormatAsLink("Pips", "SQUIRRELSPECIES");

			public static LocString STATERPILLARSPECIES = UI.FormatAsLink("Plug Slugs", "STATERPILLARSPECIES");

			public static LocString BEETASPECIES = UI.FormatAsLink("Beetas", "BEETASPECIES");

			public static LocString DIVERGENTSPECIES = UI.FormatAsLink("Divergents", "DIVERGENTSPECIES");

			public static LocString SWEEPBOT = UI.FormatAsLink("Sweepies", "SWEEPBOT");

			public static LocString SCOUTROVER = UI.FormatAsLink("Rovers", "SCOUTROVER");

			public static LocString DEERSPECIES = UI.FormatAsLink("Floxes", "DEERSPECIES");

			public static LocString SEALSPECIES = UI.FormatAsLink("Spigot Seals", "SEALSPECIES");

			public static LocString BELLYSPECIES = UI.FormatAsLink("Bammoths", "BELLYSPECIES");
		}

		public class PLANT_MUTATIONS
		{
			public static LocString PLANT_NAME_FMT = "{PlantName} ({MutationList})";

			public static LocString UNIDENTIFIED = "Unidentified Subspecies";

			public static LocString UNIDENTIFIED_DESC = "This seed must be identified at the " + BUILDINGS.PREFABS.GENETICANALYSISSTATION.NAME + " before it can be planted.";

			public static LocString BONUS_CROP_FMT = "Bonus Crop: +{Amount} {Crop}";

			public class NONE
			{
				public static LocString NAME = "Original";
			}

			public class MODERATELYLOOSE
			{
				public static LocString NAME = "Easygoing";

				public static LocString DESCRIPTION = "Plants with this mutation are easier to take care of, but don't yield as much produce.";
			}

			public class MODERATELYTIGHT
			{
				public static LocString NAME = "Specialized";

				public static LocString DESCRIPTION = "Plants with this mutation are pickier about their conditions but yield more produce.";
			}

			public class EXTREMELYTIGHT
			{
				public static LocString NAME = "Superspecialized";

				public static LocString DESCRIPTION = "Plants with this mutation are very difficult to keep alive, but produce a bounty.";
			}

			public class BONUSLICE
			{
				public static LocString NAME = "Licey";

				public static LocString DESCRIPTION = "Something about this mutation causes Meal Lice to pupate on this plant.";
			}

			public class SUNNYSPEED
			{
				public static LocString NAME = "Leafy";

				public static LocString DESCRIPTION = "This mutation provides the plant with sun-collecting leaves, allowing faster growth.";
			}

			public class SLOWBURN
			{
				public static LocString NAME = "Wildish";

				public static LocString DESCRIPTION = "These plants grow almost as slow as their wild cousins, but also consume almost no fertilizer.";
			}

			public class BLOOMS
			{
				public static LocString NAME = "Blooming";

				public static LocString DESCRIPTION = "Vestigial flowers increase the beauty of this plant. Don't inhale the pollen, though!";
			}

			public class LOADEDWITHFRUIT
			{
				public static LocString NAME = "Bountiful";

				public static LocString DESCRIPTION = "This mutation produces lots of extra produce, though it also takes a long time to pick it all!";
			}

			public class ROTTENHEAPS
			{
				public static LocString NAME = "Exuberant";

				public static LocString DESCRIPTION = "Plants with this mutation grow extremely quickly, though the produce they make is sometimes questionable.";
			}

			public class HEAVYFRUIT
			{
				public static LocString NAME = "Juicy Fruits";

				public static LocString DESCRIPTION = "Extra water in these plump mutant veggies causes them to fall right off the plant! There's no extra nutritional value, though...";
			}
		}

		public class SPECIES
		{
			public class CRAB
			{
				public static LocString NAME = UI.FormatAsLink("Pokeshell", "Crab");

				public static LocString DESC = string.Concat(new string[]
				{
					"Pokeshells are nonhostile critters unless their eggs are nearby.\n\nThey eat ",
					UI.FormatAsLink("Polluted Dirt", "TOXICSAND"),
					" and ",
					UI.FormatAsLink("Rot Piles", "COMPOST"),
					".\n\nThe shells they leave behind after molting can be crushed into ",
					UI.FormatAsLink("Lime", "LIME"),
					"."
				});

				public static LocString EGG_NAME = UI.FormatAsLink("Pinch Roe", "Crab");

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Pokeshell Spawn", "CRAB");

					public static LocString DESC = "A snippy little Pokeshell Spawn.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Pokeshell", "CRAB") + ".";
				}

				public class VARIANT_WOOD
				{
					public static LocString NAME = UI.FormatAsLink("Oakshell", "CRABWOOD");

					public static LocString DESC = string.Concat(new string[]
					{
						"Oakshells are nonhostile critters unless their eggs are nearby.\n\nThey eat ",
						UI.FormatAsLink("Polluted Dirt", "TOXICSAND"),
						", ",
						UI.FormatAsLink("Slime", "SLIMEMOLD"),
						" and ",
						UI.FormatAsLink("Rot Piles", "COMPOST"),
						".\n\nThe shells they leave behind after molting can be crushed into ",
						UI.FormatAsLink("Wood", "WOOD"),
						".\n\nOakshells thrive in ",
						UI.FormatAsLink("Ethanol", "ETHANOL"),
						"."
					});

					public static LocString EGG_NAME = UI.FormatAsLink("Oak Pinch Roe", "CRABWOOD");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Oakshell Spawn", "CRABWOOD");

						public static LocString DESC = "A knotty little Oakshell Spawn.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Oakshell", "CRABWOOD") + ".";
					}
				}

				public class VARIANT_FRESH_WATER
				{
					public static LocString NAME = UI.FormatAsLink("Sanishell", "CRABFRESHWATER");

					public static LocString DESC = string.Concat(new string[]
					{
						"Sanishells are nonhostile critters unless their eggs are nearby.\n\nThey thrive in ",
						UI.FormatAsLink("Water", "WATER"),
						" and eliminate ",
						UI.FormatAsLink("Germs", "DISEASE"),
						" from any liquid it inhabits.\n\nThey eat ",
						UI.FormatAsLink("Polluted Dirt", "TOXICSAND"),
						", ",
						UI.FormatAsLink("Slime", "SLIMEMOLD"),
						" and ",
						UI.FormatAsLink("Rot Piles", "COMPOST"),
						"."
					});

					public static LocString EGG_NAME = UI.FormatAsLink("Sani Pinch Roe", "CRABFRESHWATER");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Sanishell Spawn", "CRABFRESHWATER");

						public static LocString DESC = "A picky little Sanishell Spawn.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Sanishell", "CRABFRESHWATER") + ".";
					}
				}
			}

			public class BEE
			{
				public static LocString NAME = UI.FormatAsLink("Beeta", "BEE");

				public static LocString DESC = string.Concat(new string[]
				{
					"Beetas are hostile critters that thrive in ",
					UI.FormatAsLink("Radioactive", "RADIATION"),
					" environments.\n\nThey commonly gather ",
					UI.FormatAsLink("Uranium Ore", "URANIUMORE"),
					" for their ",
					UI.FormatAsLink("Beeta Hives", "BEEHIVE"),
					" to produce ",
					UI.FormatAsLink("Enriched Uranium", "ENRICHEDURANIUM"),
					"."
				});

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Beetiny", "BEE");

					public static LocString DESC = "A harmless little Beetiny.\n\nIn time, it will mature into a vicious adult " + UI.FormatAsLink("Beeta", "BEE") + ".";
				}
			}

			public class CHLORINEGEYSER
			{
				public static LocString NAME = UI.FormatAsLink("Chlorine Geyser", "GeyserGeneric_CHLORINE_GAS");

				public static LocString DESC = "A highly pressurized geyser that periodically erupts with " + UI.FormatAsLink("Chlorine Gas", "CHLORINEGAS") + ".";
			}

			public class PACU
			{
				public static LocString NAME = UI.FormatAsLink("Pacu", "PACU");

				public static LocString DESC = string.Concat(new string[]
				{
					"Pacus are aquatic creatures that can live in any liquid, such as ",
					UI.FormatAsLink("Water", "WATER"),
					" or ",
					UI.FormatAsLink("Contaminated Water", "DIRTYWATER"),
					".\n\nEvery organism in the known universe finds the Pacu extremely delicious."
				});

				public static LocString EGG_NAME = UI.FormatAsLink("Fry Egg", "PACU");

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Pacu Fry", "PACU");

					public static LocString DESC = "A wriggly little Pacu Fry.\n\nIn time, it will mature into an adult " + UI.FormatAsLink("Pacu", "PACU") + ".";
				}

				public class VARIANT_TROPICAL
				{
					public static LocString NAME = UI.FormatAsLink("Tropical Pacu", "PACUTROPICAL");

					public static LocString DESC = "Every organism in the known universe finds the Pacu extremely delicious.";

					public static LocString EGG_NAME = UI.FormatAsLink("Tropical Fry Egg", "PACUTROPICAL");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Tropical Fry", "PACUTROPICAL");

						public static LocString DESC = "A wriggly little Tropical Fry.\n\nIn time it will mature into an adult Pacu morph, the " + UI.FormatAsLink("Tropical Pacu", "PACUTROPICAL") + ".";
					}
				}

				public class VARIANT_CLEANER
				{
					public static LocString NAME = UI.FormatAsLink("Gulp Fish", "PACUCLEANER");

					public static LocString DESC = "Every organism in the known universe finds the Pacu extremely delicious.";

					public static LocString EGG_NAME = UI.FormatAsLink("Gulp Fry Egg", "PACUCLEANER");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Gulp Fry", "PACUCLEANER");

						public static LocString DESC = "A wriggly little Gulp Fry.\n\nIn time, it will mature into an adult " + UI.FormatAsLink("Gulp Fish", "PACUCLEANER") + ".";
					}
				}
			}

			public class GLOM
			{
				public static LocString NAME = UI.FormatAsLink("Morb", "GLOM");

				public static LocString DESC = "Morbs are attracted to unhygienic conditions and frequently excrete bursts of " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + ".";

				public static LocString EGG_NAME = UI.FormatAsLink("Morb Pod", "MORB");
			}

			public class HATCH
			{
				public static LocString NAME = UI.FormatAsLink("Hatch", "HATCH");

				public static LocString DESC = "Hatches excrete solid " + UI.FormatAsLink("Coal", "CARBON") + " as waste and may be uncovered by digging up Buried Objects.";

				public static LocString EGG_NAME = UI.FormatAsLink("Hatchling Egg", "HATCH");

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Hatchling", "HATCH");

					public static LocString DESC = "An innocent little Hatchling.\n\nIn time, it will mature into an adult " + UI.FormatAsLink("Hatch", "HATCH") + ".";
				}

				public class VARIANT_HARD
				{
					public static LocString NAME = UI.FormatAsLink("Stone Hatch", "HATCHHARD");

					public static LocString DESC = "Stone Hatches excrete solid " + UI.FormatAsLink("Coal", "CARBON") + " as waste and enjoy burrowing into the ground.";

					public static LocString EGG_NAME = UI.FormatAsLink("Stone Hatchling Egg", "HATCHHARD");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Stone Hatchling", "HATCHHARD");

						public static LocString DESC = "A doofy little Stone Hatchling.\n\nIt matures into an adult Hatch morph, the " + UI.FormatAsLink("Stone Hatch", "HATCHHARD") + ", which loves nibbling on various rocks and metals.";
					}
				}

				public class VARIANT_VEGGIE
				{
					public static LocString NAME = UI.FormatAsLink("Sage Hatch", "HATCHVEGGIE");

					public static LocString DESC = "Sage Hatches excrete solid " + UI.FormatAsLink("Coal", "CARBON") + " as waste and enjoy burrowing into the ground.";

					public static LocString EGG_NAME = UI.FormatAsLink("Sage Hatchling Egg", "HATCHVEGGIE");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Sage Hatchling", "HATCHVEGGIE");

						public static LocString DESC = "A doofy little Sage Hatchling.\n\nIt matures into an adult Hatch morph, the " + UI.FormatAsLink("Sage Hatch", "HATCHVEGGIE") + ", which loves nibbling on organic materials.";
					}
				}

				public class VARIANT_METAL
				{
					public static LocString NAME = UI.FormatAsLink("Smooth Hatch", "HATCHMETAL");

					public static LocString DESC = string.Concat(new string[]
					{
						"Smooth Hatches enjoy burrowing into the ground and excrete ",
						UI.FormatAsLink("Refined Metal", "REFINEDMETAL"),
						" when fed ",
						UI.FormatAsLink("Metal Ore", "RAWMETAL"),
						"."
					});

					public static LocString EGG_NAME = UI.FormatAsLink("Smooth Hatchling Egg", "HATCHMETAL");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Smooth Hatchling", "HATCHMETAL");

						public static LocString DESC = "A doofy little Smooth Hatchling.\n\nIt matures into an adult Hatch morph, the " + UI.FormatAsLink("Smooth Hatch", "HATCHMETAL") + ", which loves nibbling on different types of metals.";
					}
				}
			}

			public class STATERPILLAR
			{
				public static LocString NAME = UI.FormatAsLink("Plug Slug", "STATERPILLAR");

				public static LocString DESC = "Plug Slugs are dynamic creatures that generate electrical " + UI.FormatAsLink("Power", "POWER") + " during the night.\n\nTheir power can be harnessed by leaving an exposed wire near areas where they like to sleep.";

				public static LocString EGG_NAME = UI.FormatAsLink("Slug Egg", "STATERPILLAR");

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Plug Sluglet", "STATERPILLAR");

					public static LocString DESC = "A chubby little Plug Sluglet.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Plug Slug", "STATERPILLAR") + ".";
				}

				public class VARIANT_GAS
				{
					public static LocString NAME = UI.FormatAsLink("Smog Slug", "STATERPILLAR");

					public static LocString DESC = string.Concat(new string[]
					{
						"Smog Slugs are porous creatures that draw in unbreathable ",
						UI.FormatAsLink("Gases", "ELEMENTS_GAS"),
						" during the day.\n\nAt night, they sleep near exposed ",
						UI.FormatAsLink("Gas Pipes,", "GASCONDUIT"),
						" where they deposit their cache."
					});

					public static LocString EGG_NAME = UI.FormatAsLink("Smog Slug Egg", "STATERPILLAR");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Smog Sluglet", "STATERPILLAR");

						public static LocString DESC = "A tubby little Smog Sluglet.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Smog Slug", "STATERPILLAR") + ".";
					}
				}

				public class VARIANT_LIQUID
				{
					public static LocString NAME = UI.FormatAsLink("Sponge Slug", "STATERPILLAR");

					public static LocString DESC = string.Concat(new string[]
					{
						"Sponge Slugs are thirsty creatures that soak up ",
						UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID"),
						" during the day.\n\nThey deposit their stored ",
						UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID"),
						" into the exposed ",
						UI.FormatAsLink("Liquid Pipes", "LIQUIDCONDUIT"),
						" they sleep next to at night."
					});

					public static LocString EGG_NAME = UI.FormatAsLink("Sponge Slug Egg", "STATERPILLAR");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Sponge Sluglet", "STATERPILLAR");

						public static LocString DESC = "A chonky little Sponge Sluglet.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Sponge Slug", "STATERPILLAR") + ".";
					}
				}
			}

			public class DIVERGENT
			{
				public class VARIANT_BEETLE
				{
					public static LocString NAME = UI.FormatAsLink("Sweetle", "DIVERGENTBEETLE");

					public static LocString DESC = string.Concat(new string[]
					{
						"Sweetles are nonhostile critters that excrete large amounts of ",
						UI.FormatAsLink("Sucrose", "SUCROSE"),
						".\n\nThey are closely related to the ",
						UI.FormatAsLink("Grubgrub", "DIVERGENTWORM"),
						" and exhibit similar, albeit less effective farming behaviors."
					});

					public static LocString EGG_NAME = UI.FormatAsLink("Sweetle Egg", "DIVERGENTBEETLE");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Sweetle Larva", "DIVERGENTBEETLE");

						public static LocString DESC = "A crawly little Sweetle Larva.\n\nIn time, it will mature into an adult " + UI.FormatAsLink("Sweetle", "DIVERGENTBEETLE") + ".";
					}
				}

				public class VARIANT_WORM
				{
					public static LocString NAME = UI.FormatAsLink("Grubgrub", "DIVERGENTWORM");

					public static LocString DESC = string.Concat(new string[]
					{
						"Grubgrubs form symbiotic relationships with plants, especially ",
						UI.FormatAsLink("Grubfruit Plants", "WORMPLANT"),
						", and instinctually tend to them.\n\nGrubgrubs are closely related to ",
						UI.FormatAsLink("Sweetles", "DIVERGENTBEETLE"),
						"."
					});

					public static LocString EGG_NAME = UI.FormatAsLink("Grubgrub Egg", "DIVERGENTWORM");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Grubgrub Wormling", "DIVERGENTWORM");

						public static LocString DESC = "A squirmy little Grubgrub Wormling.\n\nIn time, it will mature into an adult " + UI.FormatAsLink("Grubgrub", "WORM") + " and drastically grow in size.";
					}
				}
			}

			public class DRECKO
			{
				public static LocString NAME = UI.FormatAsLink("Drecko", "DRECKO");

				public static LocString DESC = string.Concat(new string[]
				{
					"Dreckos are nonhostile critters that graze on ",
					UI.FormatAsLink("Pincha Pepperplants", "SPICE_VINE"),
					", ",
					UI.FormatAsLink("Balm Lily", "SWAMPLILY"),
					" or ",
					UI.FormatAsLink("Mealwood Plants", "BASICSINGLEHARVESTPLANT"),
					".\n\nTheir backsides are covered in thick woolly fibers that only grow in ",
					UI.FormatAsLink("Hydrogen", "HYDROGEN"),
					" climates."
				});

				public static LocString EGG_NAME = UI.FormatAsLink("Drecklet Egg", "DRECKO");

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Drecklet", "DRECKO");

					public static LocString DESC = "A little, bug-eyed Drecklet.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Drecko", "DRECKO") + ".";
				}

				public class VARIANT_PLASTIC
				{
					public static LocString NAME = UI.FormatAsLink("Glossy Drecko", "DRECKOPLASTIC");

					public static LocString DESC = string.Concat(new string[]
					{
						"Glossy Dreckos are nonhostile critters that graze on live ",
						UI.FormatAsLink("Mealwood Plants", "BASICSINGLEHARVESTPLANT"),
						" and ",
						UI.FormatAsLink("Bristle Blossoms", "PRICKLEFLOWER"),
						".\n\nTheir backsides are covered in bioplastic scales that only grow in ",
						UI.FormatAsLink("Hydrogen Gas", "HYDROGEN"),
						" climates."
					});

					public static LocString EGG_NAME = UI.FormatAsLink("Glossy Drecklet Egg", "DRECKOPLASTIC");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Glossy Drecklet", "DRECKOPLASTIC");

						public static LocString DESC = "A bug-eyed little Glossy Drecklet.\n\nIn time it will mature into an adult Drecko morph, the " + UI.FormatAsLink("Glossy Drecko", "DRECKOPLASTIC") + ".";
					}
				}
			}

			public class WOODDEER
			{
				public static LocString NAME = UI.FormatAsLink("Flox", "WOODDEER");

				public static LocString DESC = string.Concat(new string[]
				{
					"Floxes are nonhostile critters that graze on ",
					UI.FormatAsLink("Bristle Blossoms", "PRICKLEFLOWER"),
					" and ",
					UI.FormatAsLink("Pikeapples", "HARDSKINBERRY"),
					".\n\nWhen Flox antlers are fully grown, they harden into shearable ",
					UI.FormatAsLink("Wood", "WOOD"),
					"."
				});

				public static LocString EGG_NAME = UI.FormatAsLink("Flox Fawn Egg", "WOODDEER");

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Flox Fawn", "WOODDEER");

					public static LocString DESC = "A little yipping Flox Fawn.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Flox", "WOODDEER") + ".";
				}
			}

			public class ICEBELLY
			{
				public static LocString NAME = UI.FormatAsLink("Bammoth", "ICEBELLY");

				public static LocString DESC = string.Concat(new string[]
				{
					"Bammoths are lumbering, nonhostile creatures that subsist on ",
					UI.FormatAsLink("Plume Squash", "CARROT"),
					", ",
					UI.FormatAsLink("Nosh Beans", "BEANPLANTSEED"),
					" and ",
					UI.FormatAsLink("Squash Fries", "FRIESCARROT"),
					".\n\nTheir thick overcoats can be shorn for ",
					ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.NAME,
					"."
				});

				public static LocString EGG_NAME = UI.FormatAsLink("Bammini Egg", "ICEBELLY");

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Bammini", "ICEBELLY");

					public static LocString DESC = "A floofy little Bammini.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Bammoth", "ICEBELLY") + ".";
				}

				public class VARIANT_GOLD
				{
					public static LocString NAME = UI.FormatAsLink("Regal Bammoth", "GOLDBELLY");

					public static LocString DESC = string.Concat(new string[]
					{
						"Regal Bammoths are nonhostile creatures that subsist on ",
						UI.FormatAsLink("Plume Squash", "CARROT"),
						" and ",
						UI.FormatAsLink("Nosh Beans", "BEANPLANTSEED"),
						".\n\nTheir ornate crests grow only when they are fed ",
						UI.FormatAsLink("Squash Fries", "FRIESCARROT"),
						"."
					});

					public static LocString EGG_NAME = UI.FormatAsLink("Regal Bammini Egg", "GOLDBELLY");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Regal Bammini", "GOLDBELLY");

						public static LocString DESC = "A haughty little Regal Bammini.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Regal Bammoth", "GOLDBELLY") + ".";
					}
				}
			}

			public class SEAL
			{
				public static LocString NAME = UI.FormatAsLink("Spigot Seal", "SEAL");

				public static LocString DESC = string.Concat(new string[]
				{
					"Spigot Seals are charismatic creatures who produce ",
					UI.FormatAsLink("Ethanol", "ETHANOL"),
					".\n\nThey eat ",
					UI.FormatAsLink("Sucrose", "SUCROSE"),
					", and drink ",
					ELEMENTS.SUGARWATER.NAME,
					" straight from the ",
					UI.FormatAsLink("Bonbon Tree", "SPACETREE"),
					"."
				});

				public static LocString EGG_NAME = UI.FormatAsLink("Spigot Pup Egg", "SEAL");

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Spigot Pup", "SEAL");

					public static LocString DESC = "A wobbly little Spigot Pup.\n\nIn time, it will mature into an adult " + UI.FormatAsLink("Spigot Seal", "SEAL") + ".";
				}
			}

			public class SQUIRREL
			{
				public static LocString NAME = UI.FormatAsLink("Pip", "SQUIRREL");

				public static LocString DESC = string.Concat(new string[]
				{
					"Pips are pesky, nonhostile critters that subsist on ",
					UI.FormatAsLink("Thimble Reeds", "BASICFABRICPLANT"),
					" and ",
					UI.FormatAsLink("Arbor Tree", "FOREST_TREE"),
					" branches.\n\nThey are known to bury ",
					UI.FormatAsLink("Seeds", "PLANTS"),
					" in the ground whenever they can find a suitable area with enough space."
				});

				public static LocString EGG_NAME = UI.FormatAsLink("Pip Egg", "SQUIRREL");

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Pipsqueak", "SQUIRREL");

					public static LocString DESC = "A little purring Pipsqueak.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Pip", "SQUIRREL") + ".";
				}

				public class VARIANT_HUG
				{
					public static LocString NAME = UI.FormatAsLink("Cuddle Pip", "SQUIRREL");

					public static LocString DESC = "Cuddle Pips are fluffy, affectionate critters who exhibit a strong snuggling instinct towards all types of eggs.\n\nThis is temporarily amplified when they are hugged by a passing Duplicant.";

					public static LocString EGG_NAME = UI.FormatAsLink("Cuddle Pip Egg", "SQUIRREL");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Cuddle Pipsqueak", "SQUIRREL");

						public static LocString DESC = "A fuzzy little Cuddle Pipsqueak.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Cuddle Pip", "SQUIRREL") + ".";
					}
				}
			}

			public class OILFLOATER
			{
				public static LocString NAME = UI.FormatAsLink("Slickster", "OILFLOATER");

				public static LocString DESC = string.Concat(new string[]
				{
					"Slicksters are slimy critters that consume ",
					UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
					" and exude ",
					UI.FormatAsLink("Crude Oil", "CRUDEOIL"),
					"."
				});

				public static LocString EGG_NAME = UI.FormatAsLink("Larva Egg", "OILFLOATER");

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Slickster Larva", "OILFLOATER");

					public static LocString DESC = "A goopy little Slickster Larva.\n\nOne day it will grow into an adult " + UI.FormatAsLink("Slickster", "OILFLOATER") + ".";
				}

				public class VARIANT_HIGHTEMP
				{
					public static LocString NAME = UI.FormatAsLink("Molten Slickster", "OILFLOATERHIGHTEMP");

					public static LocString DESC = string.Concat(new string[]
					{
						"Molten Slicksters are slimy critters that consume ",
						UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
						" and exude ",
						UI.FormatAsLink("Petroleum", "PETROLEUM"),
						"."
					});

					public static LocString EGG_NAME = UI.FormatAsLink("Molten Larva Egg", "OILFLOATERHIGHTEMP");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Molten Larva", "OILFLOATERHIGHTEMP");

						public static LocString DESC = "A goopy little Molten Larva.\n\nOne day it will grow into an adult Slickster morph, the " + UI.FormatAsLink("Molten Slickster", "OILFLOATERHIGHTEMP") + ".";
					}
				}

				public class VARIANT_DECOR
				{
					public static LocString NAME = UI.FormatAsLink("Longhair Slickster", "OILFLOATERDECOR");

					public static LocString DESC = "Longhair Slicksters are friendly critters that consume " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and thrive in close contact with Duplicant companions.\n\nLonghairs have extremely beautiful and luxurious coats.";

					public static LocString EGG_NAME = UI.FormatAsLink("Longhair Larva Egg", "OILFLOATERDECOR");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Longhair Larva", "OILFLOATERDECOR");

						public static LocString DESC = "A snuggly little Longhair Larva.\n\nOne day it will grow into an adult Slickster morph, the " + UI.FormatAsLink("Longhair Slickster", "OILFLOATERDECOR") + ".";
					}
				}
			}

			public class PUFT
			{
				public static LocString NAME = UI.FormatAsLink("Puft", "PUFT");

				public static LocString DESC = "Pufts are non-aggressive critters that excrete lumps of " + UI.FormatAsLink("Slime", "SLIMEMOLD") + " with each breath.";

				public static LocString EGG_NAME = UI.FormatAsLink("Puftlet Egg", "PUFT");

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Puftlet", "PUFT");

					public static LocString DESC = "A gassy little Puftlet.\n\nIn time it will grow into an adult " + UI.FormatAsLink("Puft", "PUFT") + ".";
				}

				public class VARIANT_ALPHA
				{
					public static LocString NAME = UI.FormatAsLink("Puft Prince", "PUFTALPHA");

					public static LocString DESC = "The Puft Prince is a lazy critter that excretes little " + UI.FormatAsLink("Solid", "SOLID") + " lumps of whatever it has been breathing.";

					public static LocString EGG_NAME = UI.FormatAsLink("Puftlet Prince Egg", "PUFTALPHA");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Puftlet Prince", "PUFTALPHA");

						public static LocString DESC = "A gassy little Puftlet Prince.\n\nOne day it will grow into an adult Puft morph, the " + UI.FormatAsLink("Puft Prince", "PUFTALPHA") + ".\n\nIt seems a bit snobby...";
					}
				}

				public class VARIANT_OXYLITE
				{
					public static LocString NAME = UI.FormatAsLink("Dense Puft", "PUFTOXYLITE");

					public static LocString DESC = "Dense Pufts are non-aggressive critters that excrete condensed " + UI.FormatAsLink("Oxylite", "OXYROCK") + " with each breath.";

					public static LocString EGG_NAME = UI.FormatAsLink("Dense Puftlet Egg", "PUFTOXYLITE");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Dense Puftlet", "PUFTOXYLITE");

						public static LocString DESC = "A stocky little Dense Puftlet.\n\nOne day it will grow into an adult Puft morph, the " + UI.FormatAsLink("Dense Puft", "PUFTOXYLITE") + ".";
					}
				}

				public class VARIANT_BLEACHSTONE
				{
					public static LocString NAME = UI.FormatAsLink("Squeaky Puft", "PUFTBLEACHSTONE");

					public static LocString DESC = "Squeaky Pufts are non-aggressive critters that excrete lumps of " + UI.FormatAsLink("Bleachstone", "BLEACHSTONE") + " with each breath.";

					public static LocString EGG_NAME = UI.FormatAsLink("Squeaky Puftlet Egg", "PUFTBLEACHSTONE");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Squeaky Puftlet", "PUFTBLEACHSTONE");

						public static LocString DESC = "A frazzled little Squeaky Puftlet.\n\nOne day it will grow into an adult Puft morph, the " + UI.FormatAsLink("Squeaky Puft", "PUFTBLEACHSTONE") + ".";
					}
				}
			}

			public class MOO
			{
				public static LocString NAME = UI.FormatAsLink("Gassy Moo", "MOO");

				public static LocString DESC = string.Concat(new string[]
				{
					"Moos are extraterrestrial critters that feed on ",
					UI.FormatAsLink("Gas Grass", "GASGRASS"),
					" and excrete ",
					UI.FormatAsLink("Natural Gas", "METHANE"),
					".\n\nWhen domesticated and fed, they can be milked for ",
					ELEMENTS.MILK.NAME,
					"."
				});
			}

			public class MOLE
			{
				public static LocString NAME = UI.FormatAsLink("Shove Vole", "MOLE");

				public static LocString DESC = string.Concat(new string[]
				{
					"Shove Voles are burrowing critters that eat the ",
					UI.FormatAsLink("Regolith", "REGOLITH"),
					" collected on terrestrial surfaces.\n\nThey cannot burrow through ",
					UI.FormatAsLink("Refined Metals", "REFINEDMETAL"),
					"."
				});

				public static LocString EGG_NAME = UI.FormatAsLink("Shove Vole Egg", "MOLE");

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Vole Pup", "MOLE");

					public static LocString DESC = "A snuggly little pup.\n\nOne day it will grow into an adult " + UI.FormatAsLink("Shove Vole", "MOLE") + ".";
				}

				public class VARIANT_DELICACY
				{
					public static LocString NAME = UI.FormatAsLink("Delecta Vole", "MOLEDELICACY");

					public static LocString DESC = string.Concat(new string[]
					{
						"Delecta Voles are burrowing critters whose bodies sprout shearable ",
						UI.FormatAsLink("Tonic Root", "GINGERCONFIG"),
						" when ",
						UI.FormatAsLink("Regolith", "REGOLITH"),
						" is ingested at preferred temperatures.\n\nThey cannot burrow through ",
						UI.FormatAsLink("Refined Metals", "REFINEDMETAL"),
						"."
					});

					public static LocString EGG_NAME = UI.FormatAsLink("Delecta Vole Egg", "MOLEDELICACY");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Delecta Vole Pup", "MOLEDELICACY");

						public static LocString DESC = "A tender little Delecta Vole pup.\n\nOne day it will grow into an adult Shove Vole morph, the " + UI.FormatAsLink("Delecta Vole", "MOLEDELICACY") + ".";
					}
				}
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

				public static LocString EGG_NAME = UI.FormatAsLink("Shine Nymph Egg", "LIGHTBUG");

				public class BABY
				{
					public static LocString NAME = UI.FormatAsLink("Shine Nymph", "LIGHTBUG");

					public static LocString DESC = "A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUG") + ".";
				}

				public class VARIANT_ORANGE
				{
					public static LocString NAME = UI.FormatAsLink("Sun Bug", "LIGHTBUGORANGE");

					public static LocString DESC = "Shine Bugs emit a soft " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.\n\nThe light of the Sun morph has been turned orange through selective breeding.";

					public static LocString EGG_NAME = UI.FormatAsLink("Sun Nymph Egg", "LIGHTBUGORANGE");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Sun Nymph", "LIGHTBUGORANGE");

						public static LocString DESC = "A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGORANGE") + ".\n\nThis one is a Sun morph.";
					}
				}

				public class VARIANT_PURPLE
				{
					public static LocString NAME = UI.FormatAsLink("Royal Bug", "LIGHTBUGPURPLE");

					public static LocString DESC = "Shine Bugs emit a soft " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.\n\nThe light of the Royal morph has been turned purple through selective breeding.";

					public static LocString EGG_NAME = UI.FormatAsLink("Royal Nymph Egg", "LIGHTBUGPURPLE");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Royal Nymph", "LIGHTBUGPURPLE");

						public static LocString DESC = "A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGPURPLE") + ".\n\nThis one is a Royal morph.";
					}
				}

				public class VARIANT_PINK
				{
					public static LocString NAME = UI.FormatAsLink("Coral Bug", "LIGHTBUGPINK");

					public static LocString DESC = "Shine Bugs emit a soft " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.\n\nThe light of the Coral morph has been turned pink through selective breeding.";

					public static LocString EGG_NAME = UI.FormatAsLink("Coral Nymph Egg", "LIGHTBUGPINK");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Coral Nymph", "LIGHTBUGPINK");

						public static LocString DESC = "A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGPINK") + ".\n\nThis one is a Coral morph.";
					}
				}

				public class VARIANT_BLUE
				{
					public static LocString NAME = UI.FormatAsLink("Azure Bug", "LIGHTBUGBLUE");

					public static LocString DESC = "Shine Bugs emit a soft " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.\n\nThe light of the Azure morph has been turned blue through selective breeding.";

					public static LocString EGG_NAME = UI.FormatAsLink("Azure Nymph Egg", "LIGHTBUGBLUE");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Azure Nymph", "LIGHTBUGBLUE");

						public static LocString DESC = "A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGBLUE") + ".\n\nThis one is an Azure morph.";
					}
				}

				public class VARIANT_BLACK
				{
					public static LocString NAME = UI.FormatAsLink("Abyss Bug", "LIGHTBUGBLACK");

					public static LocString DESC = "This Shine Bug emits no " + UI.FormatAsLink("Light", "LIGHT") + ", but it makes up for it by having an excellent personality.";

					public static LocString EGG_NAME = UI.FormatAsLink("Abyss Nymph Egg", "LIGHTBUGBLACK");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Abyss Nymph", "LIGHTBUGBLACK");

						public static LocString DESC = "A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGBLACK") + ".\n\nThis one is an Abyss morph.";
					}
				}

				public class VARIANT_CRYSTAL
				{
					public static LocString NAME = UI.FormatAsLink("Radiant Bug", "LIGHTBUGCRYSTAL");

					public static LocString DESC = "Shine Bugs emit a soft " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.\n\nThe light of the Radiant morph has been amplified through selective breeding.";

					public static LocString EGG_NAME = UI.FormatAsLink("Radiant Nymph Egg", "LIGHTBUGCRYSTAL");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Radiant Nymph", "LIGHTBUGCRYSTAL");

						public static LocString DESC = "A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGCRYSTAL") + ".\n\nThis one is a Radiant morph.";
					}
				}

				public class VARIANT_RADIOACTIVE
				{
					public static LocString NAME = UI.FormatAsLink("Ionizing Bug", "LIGHTBUGRADIOACTIVE");

					public static LocString DESC = "Shine Bugs emit a dangerously radioactive " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.";

					public static LocString EGG_NAME = UI.FormatAsLink("Ionizing Nymph Egg", "LIGHTBUGCRYSTAL");

					public class BABY
					{
						public static LocString NAME = UI.FormatAsLink("Ionizing Nymph", "LIGHTBUGRADIOACTIVE");

						public static LocString DESC = "A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGRADIOACTIVE") + ".\n\nThis one is an Ionizing morph.";
					}
				}
			}

			public class GEYSER
			{
				public static LocString NAME = UI.FormatAsLink("Steam Geyser", "GEYSER");

				public static LocString DESC = string.Concat(new string[]
				{
					"A highly pressurized geyser that periodically erupts, spraying ",
					UI.FormatAsLink("Steam", "STEAM"),
					" and boiling hot ",
					UI.FormatAsLink("Water", "WATER"),
					"."
				});

				public class STEAM
				{
					public static LocString NAME = UI.FormatAsLink("Cool Steam Vent", "GeyserGeneric_STEAM");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with " + UI.FormatAsLink("Steam", "STEAM") + ".";
				}

				public class HOT_STEAM
				{
					public static LocString NAME = UI.FormatAsLink("Steam Vent", "GeyserGeneric_HOT_STEAM");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with scalding " + UI.FormatAsLink("Steam", "STEAM") + ".";
				}

				public class SALT_WATER
				{
					public static LocString NAME = UI.FormatAsLink("Salt Water Geyser", "GeyserGeneric_SALT_WATER");

					public static LocString DESC = "A highly pressurized geyser that periodically erupts with " + UI.FormatAsLink("Salt Water", "SALTWATER") + ".";
				}

				public class SLUSH_SALT_WATER
				{
					public static LocString NAME = UI.FormatAsLink("Cool Salt Slush Geyser", "GeyserGeneric_SLUSH_SALT_WATER");

					public static LocString DESC = "A highly pressurized geyser that periodically erupts with freezing " + ELEMENTS.BRINE.NAME + ".";
				}

				public class HOT_WATER
				{
					public static LocString NAME = UI.FormatAsLink("Water Geyser", "GeyserGeneric_HOT_WATER");

					public static LocString DESC = "A highly pressurized geyser that periodically erupts with hot " + UI.FormatAsLink("Water", "WATER") + ".";
				}

				public class SLUSH_WATER
				{
					public static LocString NAME = UI.FormatAsLink("Cool Slush Geyser", "GeyserGeneric_SLUSHWATER");

					public static LocString DESC = "A highly pressurized geyser that periodically erupts with freezing " + ELEMENTS.DIRTYWATER.NAME + ".";
				}

				public class FILTHY_WATER
				{
					public static LocString NAME = UI.FormatAsLink("Polluted Water Vent", "GeyserGeneric_FILTHYWATER");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with boiling " + UI.FormatAsLink("Contaminated Water", "DIRTYWATER") + ".";
				}

				public class SMALL_VOLCANO
				{
					public static LocString NAME = UI.FormatAsLink("Minor Volcano", "GeyserGeneric_SMALL_VOLCANO");

					public static LocString DESC = "A miniature volcano that periodically erupts with molten " + UI.FormatAsLink("Magma", "MAGMA") + ".";
				}

				public class BIG_VOLCANO
				{
					public static LocString NAME = UI.FormatAsLink("Volcano", "GeyserGeneric_BIG_VOLCANO");

					public static LocString DESC = "A massive volcano that periodically erupts with molten " + UI.FormatAsLink("Magma", "MAGMA") + ".";
				}

				public class LIQUID_CO2
				{
					public static LocString NAME = UI.FormatAsLink("Carbon Dioxide Geyser", "GeyserGeneric_LIQUID_CO2");

					public static LocString DESC = "A highly pressurized geyser that periodically erupts with boiling " + UI.FormatAsLink("Liquid Carbon Dioxide", "LIQUIDCARBONDIOXIDE") + ".";
				}

				public class HOT_CO2
				{
					public static LocString NAME = UI.FormatAsLink("Carbon Dioxide Vent", "GeyserGeneric_HOT_CO2");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with hot " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + ".";
				}

				public class HOT_HYDROGEN
				{
					public static LocString NAME = UI.FormatAsLink("Hydrogen Vent", "GeyserGeneric_HOT_HYDROGEN");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with hot gaseous " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + ".";
				}

				public class HOT_PO2
				{
					public static LocString NAME = UI.FormatAsLink("Hot Polluted Oxygen Vent", "GeyserGeneric_HOT_PO2");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with hot " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + ".";
				}

				public class SLIMY_PO2
				{
					public static LocString NAME = UI.FormatAsLink("Infectious Polluted Oxygen Vent", "GeyserGeneric_SLIMY_PO2");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with warm " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + ".";
				}

				public class CHLORINE_GAS
				{
					public static LocString NAME = UI.FormatAsLink("Chlorine Gas Vent", "GeyserGeneric_CHLORINE_GAS");

					public static LocString DESC = "A highly pressurized vent that periodically erupts with warm " + UI.FormatAsLink("Chlorine Gas", "CHLORINEGAS") + ".";
				}

				public class METHANE
				{
					public static LocString NAME = UI.FormatAsLink("Natural Gas Geyser", "GeyserGeneric_METHANE");

					public static LocString DESC = "A highly pressurized geyser that periodically erupts with hot " + UI.FormatAsLink("Natural Gas", "METHANE") + ".";
				}

				public class MOLTEN_COPPER
				{
					public static LocString NAME = UI.FormatAsLink("Copper Volcano", "GeyserGeneric_MOLTEN_COPPER");

					public static LocString DESC = "A large volcano that periodically erupts with " + UI.FormatAsLink("Molten Copper", "MOLTENCOPPER") + ".";
				}

				public class MOLTEN_IRON
				{
					public static LocString NAME = UI.FormatAsLink("Iron Volcano", "GeyserGeneric_MOLTEN_IRON");

					public static LocString DESC = "A large volcano that periodically erupts with " + UI.FormatAsLink("Molten Iron", "MOLTENIRON") + ".";
				}

				public class MOLTEN_ALUMINUM
				{
					public static LocString NAME = UI.FormatAsLink("Aluminum Volcano", "GeyserGeneric_MOLTEN_ALUMINUM");

					public static LocString DESC = "A large volcano that periodically erupts with " + UI.FormatAsLink("Molten Aluminum", "MOLTENALUMINUM") + ".";
				}

				public class MOLTEN_TUNGSTEN
				{
					public static LocString NAME = UI.FormatAsLink("Tungsten Volcano", "GeyserGeneric_MOLTEN_TUNGSTEN");

					public static LocString DESC = "A large volcano that periodically erupts with " + UI.FormatAsLink("Molten Tungsten", "MOLTENTUNGSTEN") + ".";
				}

				public class MOLTEN_GOLD
				{
					public static LocString NAME = UI.FormatAsLink("Gold Volcano", "GeyserGeneric_MOLTEN_GOLD");

					public static LocString DESC = "A large volcano that periodically erupts with " + UI.FormatAsLink("Molten Gold", "MOLTENGOLD") + ".";
				}

				public class MOLTEN_COBALT
				{
					public static LocString NAME = UI.FormatAsLink("Cobalt Volcano", "GeyserGeneric_MOLTEN_COBALT");

					public static LocString DESC = "A large volcano that periodically erupts with " + UI.FormatAsLink("Molten Cobalt", "MOLTENCOBALT") + ".";
				}

				public class MOLTEN_NIOBIUM
				{
					public static LocString NAME = UI.FormatAsLink("Niobium Volcano", "NiobiumGeyser");

					public static LocString DESC = "A large volcano that periodically erupts with " + UI.FormatAsLink("Niobium", "NIOBIUM") + ".";
				}

				public class OIL_DRIP
				{
					public static LocString NAME = UI.FormatAsLink("Leaky Oil Fissure", "GeyserGeneric_OIL_DRIP");

					public static LocString DESC = "A fissure that periodically erupts with boiling " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + ".";
				}

				public class LIQUID_SULFUR
				{
					public static LocString NAME = UI.FormatAsLink("Liquid Sulfur Geyser", "GeyserGeneric_LIQUID_SULFUR");

					public static LocString DESC = "A highly pressurized geyser that periodically erupts with boiling " + UI.FormatAsLink("Liquid Sulfur", "LIQUIDSULFUR") + ".";
				}
			}

			public class METHANEGEYSER
			{
				public static LocString NAME = UI.FormatAsLink("Natural Gas Geyser", "GeyserGeneric_METHANEGEYSER");

				public static LocString DESC = "A highly pressurized geyser that periodically erupts with " + UI.FormatAsLink("Natural Gas", "METHANE") + ".";
			}

			public class OIL_WELL
			{
				public static LocString NAME = UI.FormatAsLink("Oil Reservoir", "OIL_WELL");

				public static LocString DESC = "Oil Reservoirs are rock formations with " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + " deposits beneath their surface.\n\nOil can be extracted from a reservoir with sufficient pressure.";
			}

			public class MUSHROOMPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Dusk Cap", "MUSHROOMPLANT");

				public static LocString DESC = string.Concat(new string[]
				{
					"Dusk Caps produce ",
					UI.FormatAsLink("Mushrooms", "MUSHROOM"),
					", fungal growths that can be harvested for ",
					UI.FormatAsLink("Food", "FOOD"),
					"."
				});

				public static LocString DOMESTICATEDDESC = "This plant produces edible " + UI.FormatAsLink("Mushrooms", "MUSHROOM") + ".";
			}

			public class STEAMSPOUT
			{
				public static LocString NAME = UI.FormatAsLink("Steam Spout", "GEYSERS");

				public static LocString DESC = "A rocky vent that spouts " + UI.FormatAsLink("Steam", "STEAM") + ".";
			}

			public class PROPANESPOUT
			{
				public static LocString NAME = UI.FormatAsLink("Propane Spout", "GEYSERS");

				public static LocString DESC = "A rocky vent that spouts " + ELEMENTS.PROPANE.NAME + ".";
			}

			public class OILSPOUT
			{
				public static LocString NAME = UI.FormatAsLink("Oil Spout", "OILSPOUT");

				public static LocString DESC = "A rocky vent that spouts " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + ".";
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

			public class PACUEGG
			{
				public static LocString NAME = UI.FormatAsLink("Pacu Egg", "PACUEGG");

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

				public static LocString DESC = "A large, chlorine-dwelling " + UI.FormatAsLink("Plant", "PLANTS") + " that can be grown in farm buildings.\n\nPalmeras grow inedible buds that emit unbreathable hydrogen gas.";

				public static LocString DOMESTICATEDDESC = "A large, chlorine-dwelling " + UI.FormatAsLink("Plant", "PLANTS") + " that grows inedible buds which emit unbreathable hydrogen gas.";
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

			public class GASGRASS
			{
				public static LocString NAME = UI.FormatAsLink("Gas Grass", "GASGRASS");

				public static LocString DESC = "Gas Grass is an alien lifeform tentatively categorized as a \"plant,\" which makes up the entirety of the " + UI.FormatAsLink("Gassy Moo's", "MOO") + " diet.";

				public static LocString DOMESTICATEDDESC = "An alien grass variety that is eaten by " + UI.FormatAsLink("Gassy Moos", "MOO") + ".";
			}

			public class PINKROCK
			{
				public static LocString NAME = "Lumen Quartz";

				public static LocString DESC = "Lumen quartz emits a soft glowing " + UI.FormatAsLink("Light", "LIGHT") + " that makes everything look and feel a little rosier.\n\nIt's even brighter and more beautiful once it's been carved.";

				public static LocString EFFECT = "";
			}

			public class PINKROCKCARVED
			{
				public static LocString NAME = "Carved Lumen Quartz";

				public static LocString DESC = string.Concat(new string[]
				{
					"A Duplicant has carved this Lumen quartz in a way that really maximizes its ",
					UI.FormatAsLink("Decor", "DECOR"),
					" value and ",
					UI.FormatAsLink("brightness", "LIGHT"),
					"."
				});

				public static LocString EFFECT = "";
			}

			public class PRICKLEGRASS
			{
				public static LocString NAME = UI.FormatAsLink("Bluff Briar", "PRICKLEGRASS");

				public static LocString DESC = "Bluff Briars exude pheromones causing critters to view them as especially beautiful.";

				public static LocString DOMESTICATEDDESC = "This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".";

				public static LocString GROWTH_BONUS = "Growth Bonus";

				public static LocString WILT_PENALTY = "Wilt Penalty";
			}

			public class CYLINDRICA
			{
				public static LocString NAME = UI.FormatAsLink("Bliss Burst", "CYLINDRICA");

				public static LocString DESC = "Bliss Bursts release an explosion of " + UI.FormatAsLink("Decor", "DECOR") + " into otherwise dull environments.";

				public static LocString DOMESTICATEDDESC = "This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".";

				public static LocString GROWTH_BONUS = "Growth Bonus";

				public static LocString WILT_PENALTY = "Wilt Penalty";
			}

			public class TOEPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Tranquil Toes", "TOEPLANT");

				public static LocString DESC = "Tranquil Toes improve " + UI.FormatAsLink("Decor", "DECOR") + " by giving their surroundings the visual equivalent of a foot rub.";

				public static LocString DOMESTICATEDDESC = "This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".";

				public static LocString GROWTH_BONUS = "Growth Bonus";

				public static LocString WILT_PENALTY = "Wilt Penalty";
			}

			public class WINECUPS
			{
				public static LocString NAME = UI.FormatAsLink("Mellow Mallow", "WINECUPS");

				public static LocString DESC = string.Concat(new string[]
				{
					"Mellow Mallows heighten ",
					UI.FormatAsLink("Decor", "DECOR"),
					" and alleviate ",
					UI.FormatAsLink("Stress", "STRESS"),
					" with their calming color and cradle shape."
				});

				public static LocString DOMESTICATEDDESC = "This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".";

				public static LocString GROWTH_BONUS = "Growth Bonus";

				public static LocString WILT_PENALTY = "Wilt Penalty";
			}

			public class EVILFLOWER
			{
				public static LocString NAME = UI.FormatAsLink("Sporechid", "EVILFLOWER");

				public static LocString DESC = "Sporechids have an eerily alluring appearance to mask the fact that they host particularly nasty strain of brain fungus.";

				public static LocString DOMESTICATEDDESC = string.Concat(new string[]
				{
					"This plant improves ambient ",
					UI.FormatAsLink("Decor", "DECOR"),
					" but produces high quantities of ",
					UI.FormatAsLink("Zombie Spores", "ZOMBIESPORES"),
					"."
				});

				public static LocString GROWTH_BONUS = "Growth Bonus";

				public static LocString WILT_PENALTY = "Wilt Penalty";
			}

			public class LEAFYPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Mirth Leaf", "LEAFYPLANT");

				public static LocString DESC = string.Concat(new string[]
				{
					"Mirth Leaves sport a calm green hue known for alleviating ",
					UI.FormatAsLink("Stress", "STRESS"),
					" and improving ",
					UI.FormatAsLink("Morale", "MORALE"),
					"."
				});

				public static LocString DOMESTICATEDDESC = "This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".";

				public static LocString GROWTH_BONUS = "Growth Bonus";

				public static LocString WILT_PENALTY = "Wilt Penalty";
			}

			public class CACTUSPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Jumping Joya", "CACTUSPLANT");

				public static LocString DESC = string.Concat(new string[]
				{
					"Joyas are ",
					UI.FormatAsLink("Decorative", "DECOR"),
					" ",
					UI.FormatAsLink("Plants", "PLANTS"),
					" that are colloquially said to make gardeners \"jump for joy\"."
				});

				public static LocString DOMESTICATEDDESC = "This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".";

				public static LocString GROWTH_BONUS = "Growth Bonus";

				public static LocString WILT_PENALTY = "Wilt Penalty";
			}

			public class BULBPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Buddy Bud", "BULBPLANT");

				public static LocString DESC = "Buddy Buds are leafy plants that have a positive effect on " + UI.FormatAsLink("Morale", "MORALE") + ", much like a friend.";

				public static LocString DOMESTICATEDDESC = "This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".";

				public static LocString GROWTH_BONUS = "Growth Bonus";

				public static LocString WILT_PENALTY = "Wilt Penalty";
			}

			public class ICEFLOWER
			{
				public static LocString NAME = UI.FormatAsLink("Idylla Flower", "ICEFLOWER");

				public static LocString DESC = string.Concat(new string[]
				{
					"Idyllas are ",
					UI.FormatAsLink("Decorative", "DECOR"),
					" ",
					UI.FormatAsLink("Plants", "PLANTS"),
					" with soft, pompom-shaped blossoms.\n\nGazing dreamily at them has a positive effect on ",
					UI.FormatAsLink("Morale", "MORALE"),
					"."
				});

				public static LocString DOMESTICATEDDESC = "This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".";

				public static LocString GROWTH_BONUS = "Growth Bonus";

				public static LocString WILT_PENALTY = "Wilt Penalty";
			}

			public class HARDSKINBERRYPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Pikeapple Bush", "HARDSKINBERRYPLANT");

				public static LocString DESC = string.Concat(new string[]
				{
					"Pikeapple bushes produce ",
					ITEMS.FOOD.HARDSKINBERRY.NAME,
					", a useful yet almost unbearably fragrant ",
					UI.FormatAsLink("Food", "FOOD"),
					"."
				});

				public static LocString DOMESTICATEDDESC = "This plant produces edible " + ITEMS.FOOD.HARDSKINBERRY.NAME + ".";
			}

			public class CARROTPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Plume Squash Plant", "CARROTPLANT");

				public static LocString DESC = "Plume Squashes are sweet, nutrient-rich tubers that can be harvested for " + UI.FormatAsLink("Food", "FOOD") + ".";

				public static LocString DOMESTICATEDDESC = "This plant produces edible " + ITEMS.FOOD.CARROT.NAME + ".";
			}

			public class SPACETREE
			{
				public static LocString NAME = UI.FormatAsLink("Bonbon Tree", "SPACETREE");

				public static LocString DESC = string.Concat(new string[]
				{
					"Bonbon Tree branches synthesize ",
					UI.FormatAsLink("Light", "LIGHT"),
					" to produce ",
					ELEMENTS.SUGARWATER.NAME,
					".\n\nBranches can be harvested for ",
					ELEMENTS.WOODLOG.NAME,
					".\n\nBonbon Trees can shed most environmental debris, and rarely become entombed."
				});

				public static LocString DOMESTICATEDDESC = string.Concat(new string[]
				{
					"This plant produces ",
					ELEMENTS.SUGARWATER.NAME,
					" and ",
					ELEMENTS.WOODLOG.NAME,
					"."
				});
			}

			public class BLUE_GRASS
			{
				public static LocString NAME = UI.FormatAsLink("Alveo Vera", "BLUE_GRASS");

				public static LocString DESC = string.Concat(new string[]
				{
					"Alveo Veras consume ",
					ELEMENTS.CARBONDIOXIDE.NAME,
					" and ",
					ELEMENTS.ICE.NAME,
					", and form harvestable ",
					ELEMENTS.OXYROCK.NAME,
					" deposits."
				});

				public static LocString DOMESTICATEDDESC = "This plant produces " + ELEMENTS.OXYROCK.NAME + ".";
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

			public class SWAMPHARVESTPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Bog Bucket", "SWAMPHARVESTPLANT");

				public static LocString DESC = string.Concat(new string[]
				{
					"Bog Buckets produce juicy, sweet ",
					UI.FormatAsLink("Bog Jellies", "SWAMPFRUIT"),
					" for ",
					UI.FormatAsLink("Food", "FOOD"),
					"."
				});

				public static LocString DOMESTICATEDDESC = "This plant produces edible " + UI.FormatAsLink("Bog Jellies", "SWAMPFRUIT") + ".";
			}

			public class WORMPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Spindly Grubfruit Plant", "WORMPLANT");

				public static LocString DESC = string.Concat(new string[]
				{
					"Spindly Grubfruit Plants produce ",
					UI.FormatAsLink("Spindly Grubfruit", "WORMBASICFRUIT"),
					" for ",
					UI.FormatAsLink("Food", "FOOD"),
					".\n\nIf it is tended by a ",
					CREATURES.FAMILY.DIVERGENTSPECIES,
					" critter, it will produce high quality fruits instead."
				});

				public static LocString DOMESTICATEDDESC = "This plant produces edible " + ITEMS.FOOD.WORMBASICFRUIT.NAME + ".";
			}

			public class SUPERWORMPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Grubfruit Plant", "WORMPLANT");

				public static LocString DESC = string.Concat(new string[]
				{
					"A Grubfruit Plant that has flourished after being tended by a ",
					CREATURES.FAMILY.DIVERGENTSPECIES,
					" critter.\n\nIt will produce high quality ",
					UI.FormatAsLink("Grubfruits", "WORMSUPERFRUIT"),
					"."
				});

				public static LocString DOMESTICATEDDESC = "This plant produces edible " + ITEMS.FOOD.WORMSUPERFRUIT.NAME + ".";
			}

			public class BASICFABRICMATERIALPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Thimble Reed", "BASICFABRICPLANT");

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

			public class ICECAVESFORAGEPLANTPLANTED
			{
				public static LocString NAME = UI.FormatAsLink("Sherberry Plant", "ICECAVESFORAGEPLANTPLANTED");

				public static LocString DESC = "Sherberries are incapable of propagating, but can be harvested for a single " + UI.FormatAsLink("Food", "FOOD") + " serving.";
			}

			public class FORESTFORAGEPLANTPLANTED
			{
				public static LocString NAME = UI.FormatAsLink("Hexalent", "FORESTFORAGEPLANTPLANTED");

				public static LocString DESC = "Hexalents are incapable of propagating but can be harvested for a single, calorie dense " + UI.FormatAsLink("Food", "FOOD") + " serving.";
			}

			public class SWAMPFORAGEPLANTPLANTED
			{
				public static LocString NAME = UI.FormatAsLink("Swamp Chard", "SWAMPFORAGEPLANTPLANTED");

				public static LocString DESC = "Swamp Chards are incapable of propagating but can be harvested for a single low quality and calorie dense " + UI.FormatAsLink("Food", "FOOD") + " serving.";
			}

			public class COLDBREATHER
			{
				public static LocString NAME = UI.FormatAsLink("Wheezewort", "COLDBREATHER");

				public static LocString DESC = string.Concat(new string[]
				{
					"Wheezeworts can be planted in ",
					UI.FormatAsLink("Planter Boxes", "PLANTERBOX"),
					", ",
					UI.FormatAsLink("Farm Tiles", "FARMTILE"),
					" or ",
					UI.FormatAsLink("Hydroponic Farms", "HYDROPONICFARM"),
					", and absorb ",
					UI.FormatAsLink("Heat", "Heat"),
					" by respiring through their porous outer membranes."
				});

				public static LocString DOMESTICATEDDESC = "This plant absorbs " + UI.FormatAsLink("Heat", "Heat") + ".";
			}

			public class COLDBREATHERCLUSTER
			{
				public static LocString NAME = UI.FormatAsLink("Wheezewort", "COLDBREATHERCLUSTER");

				public static LocString DESC = string.Concat(new string[]
				{
					"Wheezeworts can be planted in ",
					UI.FormatAsLink("Planter Boxes", "PLANTERBOX"),
					", ",
					UI.FormatAsLink("Farm Tiles", "FARMTILE"),
					" or ",
					UI.FormatAsLink("Hydroponic Farms", "HYDROPONICFARM"),
					", and absorb ",
					UI.FormatAsLink("Heat", "Heat"),
					" by respiring through their porous outer membranes."
				});

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

			public class SALTPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Dasha Saltvine", "SALTPLANT");

				public static LocString DESC = string.Concat(new string[]
				{
					"Dasha Saltvines consume small amounts of ",
					UI.FormatAsLink("Chlorine Gas", "CHLORINEGAS"),
					" and form sodium deposits as they grow, producing harvestable ",
					UI.FormatAsLink("Salt", "SALT"),
					"."
				});

				public static LocString DOMESTICATEDDESC = "This plant produces unrefined " + UI.FormatAsLink("Salt", "SALT") + ".";
			}

			public class FILTERPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Hydrocactus", "FILTERPLANT");

				public static LocString DESC = string.Concat(new string[]
				{
					"Hydrocacti act as natural ",
					UI.FormatAsLink("Water", "WATER"),
					" filters when given access to ",
					UI.FormatAsLink("Sand", "SAND"),
					"."
				});

				public static LocString DOMESTICATEDDESC = string.Concat(new string[]
				{
					"This plant uses ",
					UI.FormatAsLink("Sand", "SAND"),
					" to convert ",
					UI.FormatAsLink("Polluted Water", "DIRTYWATER"),
					" into ",
					UI.FormatAsLink("Water", "WATER"),
					"."
				});
			}

			public class OXYFERN
			{
				public static LocString NAME = UI.FormatAsLink("Oxyfern", "OXYFERN");

				public static LocString DESC = string.Concat(new string[]
				{
					"Oxyferns absorb ",
					UI.FormatAsLink("Carbon Dioxide Gas", "CARBONDIOXIDE"),
					" and exude breathable ",
					UI.FormatAsLink("Oxygen", "OXYGEN"),
					"."
				});

				public static LocString DOMESTICATEDDESC = string.Concat(new string[]
				{
					"This plant converts ",
					UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
					" into ",
					UI.FormatAsLink("Oxygen", "OXYGEN"),
					"."
				});
			}

			public class BEAN_PLANT
			{
				public static LocString NAME = UI.FormatAsLink("Nosh Sprout", "BEAN_PLANT");

				public static LocString DESC = "Nosh Sprouts thrive in colder climates and produce edible " + UI.FormatAsLink("Nosh Beans", "BEANPLANTSEED") + ".";

				public static LocString DOMESTICATEDDESC = "This plant produces " + UI.FormatAsLink("Nosh Beans", "BEANPLANTSEED") + ".";
			}

			public class WOOD_TREE
			{
				public static LocString NAME = UI.FormatAsLink("Arbor Tree", "FOREST_TREE");

				public static LocString DESC = string.Concat(new string[]
				{
					"Arbor Trees grow ",
					UI.FormatAsLink("Arbor Tree Branches", "FOREST_TREE"),
					" and can be harvested for ",
					UI.FormatAsLink("Wood", "WOOD"),
					"."
				});

				public static LocString DOMESTICATEDDESC = string.Concat(new string[]
				{
					"This plant produces ",
					UI.FormatAsLink("Arbor Tree Branches", "FOREST_TREE"),
					" that can be harvested for ",
					UI.FormatAsLink("Wood", "WOOD"),
					"."
				});
			}

			public class WOOD_TREE_BRANCH
			{
				public static LocString NAME = UI.FormatAsLink("Arbor Tree Branch", "FOREST_TREE");

				public static LocString DESC = "Arbor Trees Branches can be harvested for " + UI.FormatAsLink("Wood", "WOOD") + ".";
			}

			public class SEALETTUCE
			{
				public static LocString NAME = UI.FormatAsLink("Waterweed", "SEALETTUCE");

				public static LocString DESC = "Waterweeds thrive in salty water and can be harvested for fresh, edible " + UI.FormatAsLink("Lettuce", "LETTUCE") + ".";

				public static LocString DOMESTICATEDDESC = "This plant produces " + UI.FormatAsLink("Lettuce", "LETTUCE") + ".";
			}

			public class CRITTERTRAPPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Saturn Critter Trap", "CRITTERTRAPPLANT");

				public static LocString DESC = "Critter Traps are carnivorous plants that trap unsuspecting critters and consume them, releasing " + UI.FormatAsLink("Hydrogen Gas", "HYDROGEN") + " as waste.";

				public static LocString DOMESTICATEDDESC = "This plant eats critters and produces " + UI.FormatAsLink("Hydrogen Gas", "HYDROGEN") + ".";
			}

			public class SAPTREE
			{
				public static LocString NAME = UI.FormatAsLink("Experiment 52B", "SAPTREE");

				public static LocString DESC = "A " + UI.FormatAsLink("Liquid Resin", "RESIN") + "-producing cybernetic tree that shows signs of sentience.\n\nIt is rooted firmly in place, and is waiting for some brave soul to bring it food.";
			}

			public class SEEDS
			{
				public class LEAFYPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Mirth Leaf Seed", "LEAFYPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						UI.FormatAsLink("Mirth Leaf", "LEAFYPLANT"),
						".\n\nDigging up Buried Objects may uncover a Mirth Leaf Seed."
					});
				}

				public class CACTUSPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Joya Seed", "CACTUSPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						UI.FormatAsLink("Jumping Joya", "CACTUSPLANT"),
						".\n\nDigging up Buried Objects may uncover a Joya Seed."
					});
				}

				public class BULBPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Buddy Bud Seed", "BULBPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						UI.FormatAsLink("Buddy Bud", "BULBPLANT"),
						".\n\nDigging up Buried Objects may uncover a Buddy Bud Seed."
					});
				}

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
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.COLDWHEAT.NAME,
						" plant.\n\nGrain can be sown to cultivate more Sleet Wheat, or processed into ",
						UI.FormatAsLink("Food", "FOOD"),
						"."
					});
				}

				public class GASGRASS
				{
					public static LocString NAME = UI.FormatAsLink("Gas Grass Seed", "GASGRASS");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.GASGRASS.NAME,
						" plant."
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

				public class CYLINDRICA
				{
					public static LocString NAME = UI.FormatAsLink("Bliss Burst Seed", "CYLINDRICA");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.CYLINDRICA.NAME,
						".\n\nDigging up Buried Objects may uncover a Bliss Burst Seed."
					});
				}

				public class TOEPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Tranquil Toe Seed", "TOEPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.TOEPLANT.NAME,
						".\n\nDigging up Buried Objects may uncover a Tranquil Toe Seed."
					});
				}

				public class WINECUPS
				{
					public static LocString NAME = UI.FormatAsLink("Mallow Seed", "WINECUPS");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.WINECUPS.NAME,
						".\n\nDigging up Buried Objects may uncover a Mallow Seed."
					});
				}

				public class EVILFLOWER
				{
					public static LocString NAME = UI.FormatAsLink("Sporechid Seed", "EVILFLOWER");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.EVILFLOWER.NAME,
						".\n\nDigging up Buried Objects may uncover a ",
						CREATURES.SPECIES.SEEDS.EVILFLOWER.NAME,
						"."
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

				public class ICEFLOWER
				{
					public static LocString NAME = UI.FormatAsLink("Idylla Seed", "ICEFLOWER");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of an ",
						CREATURES.SPECIES.ICEFLOWER.NAME,
						".\n\nDigging up Buried Objects may uncover an Idylla Seed."
					});
				}

				public class HARDSKINBERRYPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Pikeapple Bush Seed", "HARDSKINBERRYPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.HARDSKINBERRYPLANT.NAME,
						".\n\nDigging up Buried Objects may uncover a Pikeapple Bush Seed."
					});
				}

				public class SPACETREE
				{
					public static LocString NAME = UI.FormatAsLink("Bonbon Tree Seed", "SPACETREE");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.SPACETREE.NAME,
						".\n\nDigging up Buried Objects may uncover a Bonbon Tree Seed."
					});
				}

				public class BLUE_GRASS
				{
					public static LocString NAME = UI.FormatAsLink("Alveo Vera Seed", "BLUE_GRASS");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.BLUE_GRASS.NAME,
						".\n\nDigging up Buried Objects may uncover an Alveo Vera Seed."
					});
				}

				public class CARROTPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Plume Squash Seed", "CARROTPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.CARROTPLANT.NAME,
						".\n\nDigging up Buried Objects may uncover a Plume Squash Seed."
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

				public class SWAMPHARVESTPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Bog Bucket Seed", "SWAMPHARVESTPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.SWAMPHARVESTPLANT.NAME,
						".\n\nDigging up Buried Objects may uncover a Bog Bucket Seed."
					});
				}

				public class WORMPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Grubfruit Seed", "WORMPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.WORMPLANT.NAME,
						".\n\nDigging up Buried Objects may uncover a Grubfruit Seed."
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
					public static LocString NAME = UI.FormatAsLink("Thimble Reed Seed", "BASICFABRICPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.BASICFABRICMATERIALPLANT.NAME,
						".\n\nDigging up Buried Objects may uncover a Thimble Reed Seed."
					});
				}

				public class SALTPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Dasha Saltvine Seed", "SALTPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.SALTPLANT.NAME,
						".\n\nDigging up Buried Objects may uncover a Dasha Saltvine Seed."
					});
				}

				public class FILTERPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Hydrocactus Seed", "FILTERPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						CREATURES.SPECIES.FILTERPLANT.NAME,
						".\n\nDigging up Buried Objects may uncover a Hydrocactus Seed."
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

				public class BEAN_PLANT
				{
					public static LocString NAME = UI.FormatAsLink("Nosh Bean", "BEAN_PLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						UI.FormatAsLink("Nosh Sprout", "BEAN_PLANT"),
						".\n\nDigging up Buried Objects may uncover a Nosh Bean."
					});
				}

				public class WOOD_TREE
				{
					public static LocString NAME = UI.FormatAsLink("Arbor Acorn", "FOREST_TREE");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of an ",
						UI.FormatAsLink("Arbor Tree", "FOREST_TREE"),
						".\n\nDigging up Buried Objects may uncover an Arbor Acorn."
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
						UI.FormatAsLink("Plant", "Ink Bloom"),
						".\n\nDigging up Buried Objects may uncover an Ink Bloom Seed."
					});
				}

				public class OXYFERN
				{
					public static LocString NAME = UI.FormatAsLink("Oxyfern Seed", "OXYFERN");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of an ",
						UI.FormatAsLink("Oxyfern", "OXYFERN"),
						" plant."
					});
				}

				public class SEALETTUCE
				{
					public static LocString NAME = UI.FormatAsLink("Waterweed Seed", "SEALETTUCE");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						UI.FormatAsLink("Waterweed", "SEALETTUCE"),
						".\n\nDigging up Buried Objects may uncover a Waterweed Seed."
					});
				}

				public class CRITTERTRAPPLANT
				{
					public static LocString NAME = UI.FormatAsLink("Saturn Critter Trap Seed", "CRITTERTRAPPLANT");

					public static LocString DESC = string.Concat(new string[]
					{
						"The ",
						UI.FormatAsLink("Seed", "PLANTS"),
						" of a ",
						UI.FormatAsLink("Saturn Critter Trap", "CRITTERTRAPPLANT"),
						".\n\nDigging up Buried Objects may uncover a Saturn Critter Trap Seed."
					});
				}
			}
		}

		public class STATUSITEMS
		{
			public static LocString NAME_NON_GROWING_PLANT = "Wilted";

			public class DROWSY
			{
				public static LocString NAME = "Drowsy";

				public static LocString TOOLTIP = "This critter is looking for a place to nap";
			}

			public class SLEEPING
			{
				public static LocString NAME = "Sleeping";

				public static LocString TOOLTIP = "This critter is replenishing its " + UI.PRE_KEYWORD + "Stamina" + UI.PST_KEYWORD;
			}

			public class CALL_ADULT
			{
				public static LocString NAME = "Calling Adult";

				public static LocString TOOLTIP = "This baby's craving attention from one of its own kind";
			}

			public class HOT
			{
				public static LocString NAME = "Toasty surroundings";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter cannot let off enough ",
					UI.PRE_KEYWORD,
					"Heat",
					UI.PST_KEYWORD,
					" to keep cool in this environment\n\nIt prefers ",
					UI.PRE_KEYWORD,
					"Temperatures",
					UI.PST_KEYWORD,
					" between <b>{0}</b> and <b>{1}</b>"
				});
			}

			public class COLD
			{
				public static LocString NAME = "Chilly surroundings";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter cannot retain enough ",
					UI.PRE_KEYWORD,
					"Heat",
					UI.PST_KEYWORD,
					" to stay warm in this environment\n\nIt prefers ",
					UI.PRE_KEYWORD,
					"Temperatures",
					UI.PST_KEYWORD,
					" between <b>{0}</b> and <b>{1}</b>"
				});
			}

			public class CROP_TOO_DARK
			{
				public static LocString NAME = "    • " + CREATURES.STATS.ILLUMINATION.NAME;

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Growth will resume when ",
					UI.PRE_KEYWORD,
					"Light",
					UI.PST_KEYWORD,
					" requirements are met"
				});
			}

			public class CROP_TOO_BRIGHT
			{
				public static LocString NAME = "    • " + CREATURES.STATS.ILLUMINATION.NAME;

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Growth will resume when ",
					UI.PRE_KEYWORD,
					"Light",
					UI.PST_KEYWORD,
					" requirements are met"
				});
			}

			public class CROP_BLIGHTED
			{
				public static LocString NAME = "    • Blighted";

				public static LocString TOOLTIP = "This plant has been struck by blight and will need to be replaced";
			}

			public class HOT_CROP
			{
				public static LocString NAME = "    • " + DUPLICANTS.STATS.TEMPERATURE.NAME;

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Growth will resume when ambient ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" is between <b>{low_temperature}</b> and <b>{high_temperature}</b>"
				});
			}

			public class COLD_CROP
			{
				public static LocString NAME = "    • " + DUPLICANTS.STATS.TEMPERATURE.NAME;

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Growth will resume when ambient ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" is between <b>{low_temperature}</b> and <b>{high_temperature}</b>"
				});
			}

			public class PERFECTTEMPERATURE
			{
				public static LocString NAME = "Ideal Temperature";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter finds the current ambient ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" comfortable\n\nIdeal Range: <b>{0}</b> - <b>{1}</b>"
				});
			}

			public class EATING
			{
				public static LocString NAME = "Eating";

				public static LocString TOOLTIP = "This critter found something tasty";
			}

			public class DRINKINGMILK
			{
				public static LocString NAME = "Drinking";

				public static LocString TOOLTIP = "This critter found a tasty beverage";
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

			public class LOOKINGFORLIQUID
			{
				public static LocString NAME = "Parched";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter is looking for ",
					UI.PRE_KEYWORD,
					"Liquids",
					UI.PST_KEYWORD,
					" to mop up"
				});
			}

			public class LOOKINGFORGAS
			{
				public static LocString NAME = "Seeking Gas";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter is on the lookout for unbreathable ",
					UI.PRE_KEYWORD,
					"Gases",
					UI.PST_KEYWORD,
					" to collect"
				});
			}

			public class LOOKINGFORMILK
			{
				public static LocString NAME = "Seeking Brackene";

				public static LocString TOOLTIP = "This critter is thirsty for " + UI.PRE_KEYWORD + "Brackene" + UI.PST_KEYWORD;
			}

			public class IDLE
			{
				public static LocString NAME = "Idle";

				public static LocString TOOLTIP = "Just enjoying life, y'know?";
			}

			public class HIVE_DIGESTING
			{
				public static LocString NAME = "Digesting";

				public static LocString TOOLTIP = "Digesting yummy food!";
			}

			public class EXCITED_TO_GET_RANCHED
			{
				public static LocString NAME = "Excited";

				public static LocString TOOLTIP = "This critter heard a Duplicant call for it and is very excited!";
			}

			public class GETTING_RANCHED
			{
				public static LocString NAME = "Being Groomed";

				public static LocString TOOLTIP = "This critter's going to look so good when they're done";
			}

			public class GETTING_MILKED
			{
				public static LocString NAME = "Being Milked";

				public static LocString TOOLTIP = "This critter's going to be so relieved when they're done";
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

			public class BAGGED
			{
				public static LocString NAME = "Trussed";

				public static LocString TOOLTIP = "Tied up and ready for relocation";
			}

			public class IN_INCUBATOR
			{
				public static LocString NAME = "Incubation Complete";

				public static LocString TOOLTIP = "This critter has hatched and is waiting to be released from its incubator";
			}

			public class HYPOTHERMIA
			{
				public static LocString NAME = "Body Temperature Low";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Internal ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" is dangerously low"
				});
			}

			public class SCALDING
			{
				public static LocString NAME = "Scalding";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Current external ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" is perilously high [<b>{ExternalTemperature}</b> / <b>{TargetTemperature}</b>]"
				});

				public static LocString NOTIFICATION_NAME = "Scalding";

				public static LocString NOTIFICATION_TOOLTIP = string.Concat(new string[]
				{
					"Scalding ",
					UI.PRE_KEYWORD,
					"Temperatures",
					UI.PST_KEYWORD,
					" are hurting these Duplicants:"
				});
			}

			public class SCOLDING
			{
				public static LocString NAME = "Frostbite";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Current external ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" is perilously low [<b>{ExternalTemperature}</b> / <b>{TargetTemperature}</b>]"
				});

				public static LocString NOTIFICATION_NAME = "Frostbite";

				public static LocString NOTIFICATION_TOOLTIP = string.Concat(new string[]
				{
					"Extremely Low ",
					UI.PRE_KEYWORD,
					"Temperatures",
					UI.PST_KEYWORD,
					" are hurting these Duplicants:"
				});
			}

			public class HYPERTHERMIA
			{
				public static LocString NAME = "Body Temperature High";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Internal ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" is dangerously high [<b>{InternalTemperature}</b> / <b>{TargetTemperature}</b>]"
				});
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

			public class FORAGINGMATERIAL
			{
				public static LocString NAME = "Foraging for Materials";

				public static LocString TOOLTIP = "This critter is stocking up on supplies for later use";
			}

			public class PLANTINGSEED
			{
				public static LocString NAME = "Planting Seed";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter is burying a ",
					UI.PRE_KEYWORD,
					"Seed",
					UI.PST_KEYWORD,
					" for later"
				});
			}

			public class RUMMAGINGSEED
			{
				public static LocString NAME = "Rummaging for seeds";

				public static LocString TOOLTIP = "This critter is searching for tasty " + UI.PRE_KEYWORD + "Seeds" + UI.PST_KEYWORD;
			}

			public class HUGEGG
			{
				public static LocString NAME = "Hugging Eggs";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter is snuggling up to an ",
					UI.PRE_KEYWORD,
					"Egg",
					UI.PST_KEYWORD,
					" "
				});
			}

			public class HUGMINIONWAITING
			{
				public static LocString NAME = "Hoping for hugs";

				public static LocString TOOLTIP = "This critter is hoping for a Duplicant to pass by and give it a hug\n\nA hug from a Duplicant will prompt it to cuddle more eggs";
			}

			public class HUGMINION
			{
				public static LocString NAME = "Hugging";

				public static LocString TOOLTIP = "This critter is happily hugging a Duplicant";
			}

			public class EXPELLING_SOLID
			{
				public static LocString NAME = "Expelling Waste";

				public static LocString TOOLTIP = "This critter is doing their \"business\"";
			}

			public class EXPELLING_GAS
			{
				public static LocString NAME = "Passing Gas";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter is emitting ",
					UI.PRE_KEYWORD,
					"Gas",
					UI.PST_KEYWORD,
					"\n\nYuck!"
				});
			}

			public class EXPELLING_LIQUID
			{
				public static LocString NAME = "Expelling Waste";

				public static LocString TOOLTIP = "This critter is doing their \"business\"";
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

			public class ATTACKINGENTITY
			{
				public static LocString NAME = "Attacking";

				public static LocString TOOLTIP = "This critter is violently defending their young";
			}

			public class PROTECTINGENTITY
			{
				public static LocString NAME = "Protecting";

				public static LocString TOOLTIP = "This creature is guarding something special to them and will likely attack if approached";
			}

			public class LAYINGANEGG
			{
				public static LocString NAME = "Laying egg";

				public static LocString TOOLTIP = "Witness the miracle of life!";
			}

			public class TENDINGANEGG
			{
				public static LocString NAME = "Tending egg";

				public static LocString TOOLTIP = "Nurturing the miracle of life!";
			}

			public class GROWINGUP
			{
				public static LocString NAME = "Maturing";

				public static LocString TOOLTIP = "This baby critter is about to reach adulthood";
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

				public static LocString TOOLTIP = "This critter is tempted to bite a nearby " + UI.PRE_KEYWORD + "Lure" + UI.PST_KEYWORD;
			}

			public class FALLING
			{
				public static LocString NAME = "Falling";

				public static LocString TOOLTIP = "AHHHH!";
			}

			public class FLOPPING
			{
				public static LocString NAME = "Flopping";

				public static LocString TOOLTIP = "Fish out of water!";
			}

			public class DRYINGOUT
			{
				public static LocString NAME = "    • Beached";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This plant must be submerged in ",
					UI.PRE_KEYWORD,
					"Liquid",
					UI.PST_KEYWORD,
					" to grow"
				});
			}

			public class GROWING
			{
				public static LocString NAME = "Growing [{PercentGrow}%]";

				public static LocString TOOLTIP = "Next harvest: <b>{TimeUntilNextHarvest}</b>";
			}

			public class CROP_SLEEPING
			{
				public static LocString NAME = "Sleeping [{REASON}]";

				public static LocString TOOLTIP = "Requires: {REQUIREMENTS}";

				public static LocString REQUIREMENT_LUMINANCE = "<b>{0}</b> Lux";

				public static LocString REASON_TOO_DARK = "Too Dark";

				public static LocString REASON_TOO_BRIGHT = "Too Bright";
			}

			public class MOLTING
			{
				public static LocString NAME = "Molting";

				public static LocString TOOLTIP = "This critter is shedding its skin. Yuck";
			}

			public class CLEANING
			{
				public static LocString NAME = "Cleaning";

				public static LocString TOOLTIP = "This critter is de-germ-ifying its liquid surroundings";
			}

			public class MILKPRODUCER
			{
				public static LocString NAME = "Producing Brackene {amount}";

				public static LocString TOOLTIP = "This critter's internal tank is refilling itself";
			}

			public class NEEDSFERTILIZER
			{
				public static LocString NAME = "    • " + CREATURES.STATS.FERTILIZATION.NAME;

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Growth will resume when ",
					UI.PRE_KEYWORD,
					"Fertilization",
					UI.PST_KEYWORD,
					" requirements are met"
				});

				public static LocString LINE_ITEM = "\n            • {Resource}: {Amount}";
			}

			public class NEEDSIRRIGATION
			{
				public static LocString NAME = "    • " + CREATURES.STATS.IRRIGATION.NAME;

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Growth will resume when ",
					UI.PRE_KEYWORD,
					"Liquid",
					UI.PST_KEYWORD,
					" requirements are met"
				});

				public static LocString LINE_ITEM = "\n            • {Resource}: {Amount}";
			}

			public class WRONGFERTILIZER
			{
				public static LocString NAME = "    • " + CREATURES.STATS.FERTILIZATION.NAME;

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This farm is storing materials that are not suitable for this plant\n\nEmpty this building's ",
					UI.PRE_KEYWORD,
					"Storage",
					UI.PST_KEYWORD,
					" to remove the unusable materials"
				});

				public static LocString LINE_ITEM = "            • {0}: {1}\n";
			}

			public class WRONGIRRIGATION
			{
				public static LocString NAME = "    • " + CREATURES.STATS.FERTILIZATION.NAME;

				public static LocString TOOLTIP = "This farm is storing materials that are not suitable for this plant\n\nEmpty this building's storage to remove the unusable materials";

				public static LocString LINE_ITEM = "            • {0}: {1}\n";
			}

			public class WRONGFERTILIZERMAJOR
			{
				public static LocString NAME = "    • " + CREATURES.STATS.FERTILIZATION.NAME;

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This farm is storing materials that are not suitable for this plant\n\n",
					UI.PRE_KEYWORD,
					"Empty Storage",
					UI.PST_KEYWORD,
					" on this building to remove the unusable materials"
				});

				public static LocString LINE_ITEM = "        " + CREATURES.STATUSITEMS.WRONGFERTILIZER.LINE_ITEM;
			}

			public class WRONGIRRIGATIONMAJOR
			{
				public static LocString NAME = "    • Irrigation (Stored)";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This farm is storing ",
					UI.PRE_KEYWORD,
					"Liquids",
					UI.PST_KEYWORD,
					" that are not suitable for this plant\n\n",
					UI.PRE_KEYWORD,
					"Empty Storage",
					UI.PST_KEYWORD,
					" on this building to remove the unusable liquids"
				});

				public static LocString LINE_ITEM = "        " + CREATURES.STATUSITEMS.WRONGIRRIGATION.LINE_ITEM;
			}

			public class CANTACCEPTFERTILIZER
			{
				public static LocString NAME = "    • " + CREATURES.STATS.FERTILIZATION.NAME;

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This farm plot does not accept ",
					UI.PRE_KEYWORD,
					"Fertilizer",
					UI.PST_KEYWORD,
					"\n\nMove the selected plant to a fertilization capable plot for optimal growth"
				});
			}

			public class CANTACCEPTIRRIGATION
			{
				public static LocString NAME = "    • " + CREATURES.STATS.IRRIGATION.NAME;

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This farm plot does not accept ",
					UI.PRE_KEYWORD,
					"Irrigation",
					UI.PST_KEYWORD,
					"\n\nMove the selected plant to an irrigation capable plot for optimal growth"
				});
			}

			public class READYFORHARVEST
			{
				public static LocString NAME = "Harvest Ready";

				public static LocString TOOLTIP = "This plant can be harvested for materials";
			}

			public class READYFORHARVEST_BRANCH
			{
				public static LocString NAME = "Harvest Ready";

				public static LocString TOOLTIP = "This branch can be harvested for materials";
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

				public static LocString TOOLTIP = "This {0} is trapped and needs help digging out";

				public static LocString LINE_ITEM = "    • Entombed";
			}

			public class DROWNING
			{
				public static LocString NAME = "Drowning";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter can't breathe in ",
					UI.PRE_KEYWORD,
					"Liquid",
					UI.PST_KEYWORD,
					"!"
				});
			}

			public class DISABLED
			{
				public static LocString NAME = "Disabled";

				public static LocString TOOLTIP = "Something is preventing this critter from functioning!";
			}

			public class SATURATED
			{
				public static LocString NAME = "Too Wet!";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter likes ",
					UI.PRE_KEYWORD,
					"Liquid",
					UI.PST_KEYWORD,
					", but not that much!"
				});
			}

			public class PRODUCINGSUGARWATER
			{
				public static LocString NAME = "Producing Nectar ({0})";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Current ",
					UI.PRE_KEYWORD,
					"Nectar",
					UI.PST_KEYWORD,
					" production rate: {0}\n\n",
					UI.PRE_KEYWORD,
					"Nectar",
					UI.PST_KEYWORD,
					" production rate depends on total branch productivity\n\nBranches must be fully grown and exposed to ",
					UI.PRE_KEYWORD,
					"Light",
					UI.PST_KEYWORD,
					" in order to be productive\n\n{BRANCH_0}\n{BRANCH_1}\n{BRANCH_2}\n{BRANCH_3}\n{BRANCH_4}\n\nOptimal productivity: {1} branches at {2} each"
				});

				public static LocString BRANCH_LINE = "{0} Branch productivity: {1}";

				public static LocString BRANCH_LINE_MISSING = "{0} Branch not found";

				public static LocString BRANCH_LINE_GROWING = "{0} Branch growth : {1}";
			}

			public class SUGARWATERPRODUCTIONPAUSED
			{
				public static LocString NAME = "Nectar Production Halted: {0}";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This plant's branches are not currently producing ",
					UI.PRE_KEYWORD,
					"Nectar",
					UI.PST_KEYWORD,
					"\n\nBranches must be fully grown and exposed to ",
					UI.PRE_KEYWORD,
					"Light",
					UI.PST_KEYWORD,
					" in order to be productive\n\n{BRANCH_0}\n{BRANCH_1}\n{BRANCH_2}\n{BRANCH_3}\n{BRANCH_4}\n\nOptimal productivity: {0} branches at {1} each\n\n",
					UI.PRE_KEYWORD,
					"Nectar",
					UI.PST_KEYWORD,
					" production will resume when conditions improve"
				});

				public static LocString BRANCH_LINE = "{0} Branch productivity: {1}";

				public static LocString BRANCH_LINE_MISSING = "{0} Branch not found";

				public static LocString BRANCH_LINE_GROWING = "{0} Branch growth : {1}";
			}

			public class SUGARWATERPRODUCTIONWILTED
			{
				public static LocString NAME = "Nectar Production Halted: {0}";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This plant is wilted\n\n",
					UI.PRE_KEYWORD,
					"Nectar",
					UI.PST_KEYWORD,
					" production will resume when conditions improve"
				});
			}

			public class SPACETREEBRANCHLIGHTSTATUS
			{
				public static LocString NAME = "Branch Productivity: {0}";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"The selected branch's ",
					UI.PRE_KEYWORD,
					"Light",
					UI.PST_KEYWORD,
					" exposure contributes to this plant's total ",
					UI.PRE_KEYWORD,
					"Nectar",
					UI.PST_KEYWORD,
					" production\n\nOptimal lux: {0}\n\nCurrent lux: {1}"
				});
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

				public static LocString TOOLTIP = "This plant will produce no more " + UI.PRE_KEYWORD + "Seeds" + UI.PST_KEYWORD;
			}

			public class ATMOSPHERICPRESSURETOOLOW
			{
				public static LocString NAME = "    • Pressure";

				public static LocString TOOLTIP = "Growth will resume when air pressure is between <b>{low_mass}</b> and <b>{high_mass}</b>";
			}

			public class WRONGATMOSPHERE
			{
				public static LocString NAME = "    • Atmosphere";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Growth will resume when submersed in one of the following ",
					UI.PRE_KEYWORD,
					"Gases",
					UI.PST_KEYWORD,
					": {elements}"
				});
			}

			public class ATMOSPHERICPRESSURETOOHIGH
			{
				public static LocString NAME = "    • Pressure";

				public static LocString TOOLTIP = "Growth will resume when air pressure is between <b>{low_mass}</b> and <b>{high_mass}</b>";
			}

			public class PERFECTATMOSPHERICPRESSURE
			{
				public static LocString NAME = "Ideal Air Pressure";

				public static LocString TOOLTIP = "This critter is comfortable in the current atmospheric pressure\n\nIdeal Range: <b>{0}</b> - <b>{1}</b>";
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

			public class REFRIGERATEDFROZEN
			{
				public static LocString NAME = "Deep Freeze";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					UI.PRE_KEYWORD,
					"Temperatures",
					UI.PST_KEYWORD,
					" below <b>{PreserveTemperature}</b> are greatly prolonging the shelf-life of this food\n\n",
					UI.PRE_KEYWORD,
					"Temperatures",
					UI.PST_KEYWORD,
					" above <b>{RotTemperature}</b> spoil food more quickly"
				});
			}

			public class REFRIGERATED
			{
				public static LocString NAME = "Refrigerated";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Ideal ",
					UI.PRE_KEYWORD,
					"Temperature",
					UI.PST_KEYWORD,
					" storage is slowing this food's ",
					UI.PRE_KEYWORD,
					"Decay Rate",
					UI.PST_KEYWORD,
					"\n\n",
					UI.PRE_KEYWORD,
					"Temperatures",
					UI.PST_KEYWORD,
					" above <b>{RotTemperature}</b> spoil food more quickly\n\nStore food below {PreserveTemperature} to further reduce spoilage."
				});
			}

			public class UNREFRIGERATED
			{
				public static LocString NAME = "Unrefrigerated";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This food is warm\n\n",
					UI.PRE_KEYWORD,
					"Temperatures",
					UI.PST_KEYWORD,
					" above <b>{RotTemperature}</b> spoil food more quickly"
				});
			}

			public class CONTAMINATEDATMOSPHERE
			{
				public static LocString NAME = "Pollution Exposure";

				public static LocString TOOLTIP = "Exposure to contaminants is accelerating this food's " + UI.PRE_KEYWORD + "Decay Rate" + UI.PST_KEYWORD;
			}

			public class STERILIZINGATMOSPHERE
			{
				public static LocString NAME = "Sterile Atmosphere";

				public static LocString TOOLTIP = "Microbe destroying conditions have decreased this food's " + UI.PRE_KEYWORD + "Decay Rate" + UI.PST_KEYWORD;
			}

			public class EXCHANGINGELEMENTCONSUME
			{
				public static LocString NAME = "Consuming {ConsumeElement} at {ConsumeRate}";

				public static LocString TOOLTIP = "{ConsumeElement} is being used at a rate of " + UI.FormatAsNegativeRate("{ConsumeRate}");
			}

			public class EXCHANGINGELEMENTOUTPUT
			{
				public static LocString NAME = "Outputting {OutputElement} at {OutputRate}";

				public static LocString TOOLTIP = "{OutputElement} is being expelled at a rate of " + UI.FormatAsPositiveRate("{OutputRate}");
			}

			public class FRESH
			{
				public static LocString NAME = "Fresh {RotPercentage}";

				public static LocString TOOLTIP = "Get'em while they're hot!\n\n{RotTooltip}";
			}

			public class STALE
			{
				public static LocString NAME = "Stale {RotPercentage}";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This ",
					UI.PRE_KEYWORD,
					"Food",
					UI.PST_KEYWORD,
					" is still edible but will soon expire\n{RotTooltip}"
				});
			}

			public class SPOILED
			{
				public static LocString NAME = "Rotten";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This ",
					UI.PRE_KEYWORD,
					"Food",
					UI.PST_KEYWORD,
					" has putrefied and should not be consumed"
				});
			}

			public class STUNTED_SCALE_GROWTH
			{
				public static LocString NAME = "Stunted Scales";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter's ",
					UI.PRE_KEYWORD,
					"Scale Growth",
					UI.PST_KEYWORD,
					" is being stunted by an unfavorable environment"
				});
			}

			public class RECEPTACLEINOPERATIONAL
			{
				public static LocString NAME = "    • Farm plot inoperable";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This farm plot cannot grow ",
					UI.PRE_KEYWORD,
					"Plants",
					UI.PST_KEYWORD,
					" in its current state"
				});
			}

			public class TRAPPED
			{
				public static LocString NAME = "Trapped";

				public static LocString TOOLTIP = "This critter has been contained and cannot escape";
			}

			public class EXHALING
			{
				public static LocString NAME = "Exhaling";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter is expelling ",
					UI.PRE_KEYWORD,
					"Gas",
					UI.PST_KEYWORD,
					" from its lungsacs"
				});
			}

			public class INHALING
			{
				public static LocString NAME = "Inhaling";

				public static LocString TOOLTIP = "This critter is taking a deep breath";
			}

			public class EXTERNALTEMPERATURE
			{
				public static LocString NAME = "External Temperature";

				public static LocString TOOLTIP = "External Temperature\n\nThis critter's environment is {0}";
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
				public static LocString NAME = "Looking for Food";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter is hungry\n\nIt will keep seeking ",
					UI.PRE_KEYWORD,
					"Food",
					UI.PST_KEYWORD,
					" until its tummy is full\n"
				});
			}

			public class HIVEHUNGRY
			{
				public static LocString NAME = "Food Supply Low";

				public static LocString TOOLTIP = "The food reserves in this hive are running low";
			}

			public class STARVING
			{
				public static LocString NAME = "Starving\nTime until death: {TimeUntilDeath}\n";

				public static LocString TOOLTIP = "This critter is starving and will die if it is not fed soon";

				public static LocString NOTIFICATION_NAME = "Critter Starvation";

				public static LocString NOTIFICATION_TOOLTIP = "These critters are starving and will die if not fed soon:";
			}

			public class OLD
			{
				public static LocString NAME = "Elderly";

				public static LocString TOOLTIP = "This sweet ol'critter is over the hill and will pass on in <b>{TimeUntilDeath}</b>";
			}

			public class DIVERGENT_WILL_TEND
			{
				public static LocString NAME = "Moving to Plant";

				public static LocString TOOLTIP = "This critter is off to tend a plant that's caught its attention";
			}

			public class DIVERGENT_TENDING
			{
				public static LocString NAME = "Plant Tending";

				public static LocString TOOLTIP = "This critter is snuggling a plant to help it grow";
			}

			public class NOSLEEPSPOT
			{
				public static LocString NAME = "Nowhere To Sleep";

				public static LocString TOOLTIP = "This critter wants to sleep but can't find a good spot to snuggle up!";
			}

			public class ORIGINALPLANTMUTATION
			{
				public static LocString NAME = "Original Plant";

				public static LocString TOOLTIP = "This is the original, unmutated variant of this species.";
			}

			public class UNKNOWNMUTATION
			{
				public static LocString NAME = "Unknown Mutation";

				public static LocString TOOLTIP = "This seed carries some unexpected genetic markers. Analyze it at the " + UI.FormatAsLink(BUILDINGS.PREFABS.GENETICANALYSISSTATION.NAME, "GENETICANALYSISSTATION") + " to learn its secrets.";
			}

			public class SPECIFICPLANTMUTATION
			{
				public static LocString NAME = "Mutant Plant: {MutationName}";

				public static LocString TOOLTIP = "This plant is mutated with a genetic variant I call {MutationName}.";
			}

			public class CROP_TOO_NONRADIATED
			{
				public static LocString NAME = "    • Low Radiation Levels";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Growth will resume when ",
					UI.PRE_KEYWORD,
					"Radiation",
					UI.PST_KEYWORD,
					" requirements are met"
				});
			}

			public class CROP_TOO_RADIATED
			{
				public static LocString NAME = "    • High Radiation Levels";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Growth will resume when ",
					UI.PRE_KEYWORD,
					"Radiation",
					UI.PST_KEYWORD,
					" requirements are met"
				});
			}

			public class ELEMENT_GROWTH_GROWING
			{
				public static LocString NAME = "Picky Eater: Just Right";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter's ",
					UI.PRE_KEYWORD,
					"Quill Growth",
					UI.PST_KEYWORD,
					" rate is optimal\n\nPreferred food temperature range: {templo} - {temphi}"
				});

				public static LocString PREFERRED_TEMP = "Last eaten: {element} at {temperature}";
			}

			public class ELEMENT_GROWTH_STUNTED
			{
				public static LocString NAME = "Picky Eater: {reason}";

				public static LocString TOO_HOT = "Too Hot";

				public static LocString TOO_COLD = "Too Cold";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter's ",
					UI.PRE_KEYWORD,
					"Quill Growth",
					UI.PST_KEYWORD,
					" rate has slowed because they ate food outside their preferred temperature range\n\nPreferred food temperature range: {templo} - {temphi}"
				});
			}

			public class ELEMENT_GROWTH_HALTED
			{
				public static LocString NAME = "Picky Eater: Hungry";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter's ",
					UI.PRE_KEYWORD,
					"Quill Growth",
					UI.PST_KEYWORD,
					" is halted because they are hungry\n\nPreferred food temperature range: {templo} - {temphi}"
				});
			}

			public class ELEMENT_GROWTH_COMPLETE
			{
				public static LocString NAME = "Picky Eater: All Done";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter's ",
					UI.PRE_KEYWORD,
					"Tonic Root",
					UI.PST_KEYWORD,
					" quills are fully grown\n\nPreferred food temperature range: {templo} - {temphi}"
				});
			}

			public class GRAVITAS_CREATURE_MANIPULATOR_COOLDOWN
			{
				public static LocString NAME = "Processing Sample: {percent}";

				public static LocString TOOLTIP = "This building is busy analyzing genetic data from a recently scanned specimen\n\nRemaining: {timeleft}";
			}

			public class BECKONING
			{
				public static LocString NAME = "Mooing";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter is inviting faraway friends to graze on this asteroid's abundant food supply\n\nA new ",
					UI.PRE_KEYWORD,
					"Gassy Moo",
					UI.PST_KEYWORD,
					" will soon arrive"
				});
			}

			public class BECKONINGBLOCKED
			{
				public static LocString NAME = "Moo-ted";

				public static LocString TOOLTIP = "This critter needs a clear view of space in order to invite a friend to this asteroid";
			}

			public class MILKFULL
			{
				public static LocString NAME = "Udderly Full";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter is full of ",
					UI.PRE_KEYWORD,
					"Brackene",
					UI.PST_KEYWORD,
					" and ready to be milked"
				});
			}

			public class CRITTERCONDO
			{
				public class MOVINGTO
				{
					public static LocString NAME = "Heading for Critter Condo";

					public static LocString TOOLTIP = "This critter is making its way to an unoccupied critter condo";
				}

				public class INTERACTING
				{
					public static LocString NAME = "Getting Cozy";

					public static LocString TOOLTIP = "This critter is relaxing in a critter condo\n\nIt's like a mini vacation!";
				}
			}

			public class UNDERWATERCRITTERCONDO
			{
				public class MOVINGTO
				{
					public static LocString NAME = "Heading for Water Fort";

					public static LocString TOOLTIP = "This critter is making its way to an unoccupied water fort";
				}

				public class INTERACTING
				{
					public static LocString NAME = "Getting Cozy";

					public static LocString TOOLTIP = "This critter is relaxing in a water fort\n\nIt's like a mini vacation!";
				}
			}

			public class AIRBORNECRITTERCONDO
			{
				public class MOVINGTO
				{
					public static LocString NAME = "Heading for Airborne Critter Condo";

					public static LocString TOOLTIP = "This critter is making its way to an unoccupied airborne critter condo";
				}

				public class INTERACTING
				{
					public static LocString NAME = "Getting Cozy";

					public static LocString TOOLTIP = "This critter is nesting in an airborne critter condo\n\nIt's like a mini vacation!";
				}
			}

			public class TEMPERATURE_HOT_UNCOMFORTABLE
			{
				public static LocString NAME = "Body Temperature High";

				public static LocString TOOLTIP = "This critter is feeling unpleasantly hot\n\nComfortable temperature range: {1} to {2}\nCurrent internal temperature: {0}\n\n{3}";
			}

			public class TEMPERATURE_HOT_DEADLY
			{
				public static LocString NAME = "Scalding";

				public static LocString TOOLTIP = "This critter is feeling dangerously overheated\n\nLivable temperature range: {1} to {2}\nCurrent external temperature: {0}\n\n{3}";
			}

			public class TEMPERATURE_COLD_UNCOMFORTABLE
			{
				public static LocString NAME = "Body Temperature Low";

				public static LocString TOOLTIP = "This critter is feeling unpleasantly cold\n\nComfortable temperature range: {1} to {2}\nCurrent internal temperature: {0}\n\n{3}";
			}

			public class TEMPERATURE_COLD_DEADLY
			{
				public static LocString NAME = "Freezing";

				public static LocString TOOLTIP = "This critter is feeling dangerously cold\n\nLivable temperature range: {1} to {2}\nCurrent external temperature: {0}\n\n{3}";
			}
		}

		public class STATS
		{
			public class HEALTH
			{
				public static LocString NAME = "Health";
			}

			public class AGE
			{
				public static LocString NAME = "Age";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter will die when its ",
					UI.PRE_KEYWORD,
					"Age",
					UI.PST_KEYWORD,
					" reaches its species' maximum lifespan"
				});
			}

			public class MATURITY
			{
				public static LocString NAME = "Growth Progress";

				public static LocString TOOLTIP = "Growth Progress\n\n";

				public static LocString TOOLTIP_GROWING = "Predicted Maturation: <b>{0}</b>";

				public static LocString TOOLTIP_GROWING_CROP = "Predicted Maturation Time: <b>{0}</b>\nNext harvest occurs in approximately <b>{1}</b>";

				public static LocString TOOLTIP_GROWN = "Growth paused while plant awaits harvest";

				public static LocString TOOLTIP_STALLED = "Poor conditions have halted this plant's growth";

				public static LocString AMOUNT_DESC_FMT = "{0}: {1}\nNext harvest in <b>{2}</b>";

				public static LocString GROWING = "Domestic Growth Rate";

				public static LocString GROWINGWILD = "Wild Growth Rate";
			}

			public class CRITTERTEMPERATURE
			{
				public static LocString NAME = "Body Temperature";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter's comfortable ",
					UI.PRE_KEYWORD,
					"Body Temperature",
					UI.PST_KEYWORD,
					" range is between <b>{0}</b> and <b>{1}</b>\n\nIts livable ",
					UI.PRE_KEYWORD,
					"External Temperature",
					UI.PST_KEYWORD,
					" range is between <b>{2}</b> and <b>{3}</b>"
				});
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

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Fully ",
					UI.PRE_KEYWORD,
					"Tame",
					UI.PST_KEYWORD,
					" critters produce more materials than wild ones, and may even provide psychological benefits to my colony\n\nThis critter is <b>{0}</b> domesticated"
				});
			}

			public class HAPPINESS
			{
				public static LocString NAME = "Happiness";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"High ",
					UI.PRE_KEYWORD,
					"Happiness",
					UI.PST_KEYWORD,
					" increases a critter's productivity and indirectly improves their ",
					UI.PRE_KEYWORD,
					"Egg",
					UI.PST_KEYWORD,
					" laying rates\n\nIt also provides the satisfaction in knowing they're living a good little critter life"
				});
			}

			public class WILDNESS
			{
				public static LocString NAME = "Wildness";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"At 0% ",
					UI.PRE_KEYWORD,
					"Wildness",
					UI.PST_KEYWORD,
					" a critter becomes ",
					UI.PRE_KEYWORD,
					"Tame",
					UI.PST_KEYWORD,
					", increasing its ",
					UI.PRE_KEYWORD,
					"Metabolism",
					UI.PST_KEYWORD,
					" and requiring regular care from Duplicants\n\nDuplicants must possess the ",
					UI.PRE_KEYWORD,
					"Critter Ranching",
					UI.PST_KEYWORD,
					" skill to care for critters"
				});
			}

			public class FERTILITY
			{
				public static LocString NAME = "Reproduction";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"At 100% ",
					UI.PRE_KEYWORD,
					"Reproduction",
					UI.PST_KEYWORD,
					", critters will reach the end of their reproduction cycle and lay a new ",
					UI.PRE_KEYWORD,
					"Egg",
					UI.PST_KEYWORD,
					"\n\nAfter an ",
					UI.PRE_KEYWORD,
					"Egg",
					UI.PST_KEYWORD,
					" is laid, ",
					UI.PRE_KEYWORD,
					"Reproduction",
					UI.PST_KEYWORD,
					" is rolled back to 0%"
				});
			}

			public class BECKONING
			{
				public static LocString NAME = "Accu-moo-lation";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"At 100% ",
					UI.PRE_KEYWORD,
					"Accu-moo-lation",
					UI.PST_KEYWORD,
					", a Gassy Moo calls a friend to join them on this asteroid\n\nAfter the new Gassy Moo has landed, ",
					UI.PRE_KEYWORD,
					"Accu-moo-lation",
					UI.PST_KEYWORD,
					" is rolled back to 0%"
				});
			}

			public class INCUBATION
			{
				public static LocString NAME = "Incubation";

				public static LocString TOOLTIP = "Eggs hatch into brand new " + UI.FormatAsLink("Critters", "CREATURES") + " at the end of their incubation period";
			}

			public class VIABILITY
			{
				public static LocString NAME = "Viability";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Eggs will lose ",
					UI.PRE_KEYWORD,
					"Viability",
					UI.PST_KEYWORD,
					" over time when exposed to poor environmental conditions\n\nAt 0% ",
					UI.PRE_KEYWORD,
					"Viability",
					UI.PST_KEYWORD,
					" a critter egg will crack and produce a ",
					ITEMS.FOOD.RAWEGG.NAME,
					" and ",
					ITEMS.INDUSTRIAL_PRODUCTS.EGG_SHELL.NAME
				});
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

			public class THERMALCONDUCTIVITYBARRIER
			{
				public static LocString NAME = "Thermal Conductivity Barrier";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Thick ",
					UI.PRE_KEYWORD,
					"Conductivity Barriers",
					UI.PST_KEYWORD,
					" increase the time it takes an object to heat up or cool down"
				});
			}

			public class ROT
			{
				public static LocString NAME = "Freshness";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Food items become stale at fifty percent ",
					UI.PRE_KEYWORD,
					"Freshness",
					UI.PST_KEYWORD,
					", and rot at zero percent"
				});
			}

			public class SCALEGROWTH
			{
				public static LocString NAME = "Scale Growth";

				public static LocString TOOLTIP = "The amount of time required for this critter to regrow its scales";
			}

			public class MILKPRODUCTION
			{
				public static LocString NAME = "Brackene Supply";

				public static LocString TOOLTIP = "The amount of time required for this critter to replenish its natural reserves of " + UI.PRE_KEYWORD + "Brackene" + UI.PST_KEYWORD;
			}

			public class ELEMENTGROWTH
			{
				public static LocString NAME = "Quill Growth";

				public static LocString TOOLTIP = "The amount of time required for this critter to regrow its " + UI.PRE_KEYWORD + "Tonic Root" + UI.PST_KEYWORD;
			}

			public class AIRPRESSURE
			{
				public static LocString NAME = "Air Pressure";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"The average ",
					UI.PRE_KEYWORD,
					"Gas",
					UI.PST_KEYWORD,
					" density of the air surrounding this plant"
				});
			}
		}

		public class ATTRIBUTES
		{
			public class INCUBATIONDELTA
			{
				public static LocString NAME = "Incubation Rate";

				public static LocString DESC = "";
			}

			public class POWERCHARGEDELTA
			{
				public static LocString NAME = "Power Charge Loss Rate";

				public static LocString DESC = "";
			}

			public class VIABILITYDELTA
			{
				public static LocString NAME = "Viability Loss Rate";

				public static LocString DESC = "";
			}

			public class SCALEGROWTHDELTA
			{
				public static LocString NAME = "Scale Growth";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Determines how long this ",
					UI.PRE_KEYWORD,
					"Critter's",
					UI.PST_KEYWORD,
					" scales will take to grow back."
				});
			}

			public class MILKPRODUCTIONDELTA
			{
				public static LocString NAME = "Brackene Supply";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Determines how long this ",
					UI.PRE_KEYWORD,
					"Critter",
					UI.PST_KEYWORD,
					" takes to replenish its natural supply of ",
					UI.PRE_KEYWORD,
					"Brackene",
					UI.PST_KEYWORD,
					"."
				});
			}

			public class WILDNESSDELTA
			{
				public static LocString NAME = "Wildness";

				public static LocString DESC = string.Concat(new string[]
				{
					"Wild creatures can survive on fewer ",
					UI.PRE_KEYWORD,
					"Calories",
					UI.PST_KEYWORD,
					" than domesticated ones."
				});
			}

			public class FERTILITYDELTA
			{
				public static LocString NAME = "Reproduction Rate";

				public static LocString DESC = string.Concat(new string[]
				{
					"Determines the amount of time needed for a ",
					UI.PRE_KEYWORD,
					"Critter",
					UI.PST_KEYWORD,
					" to lay new ",
					UI.PRE_KEYWORD,
					"Eggs",
					UI.PST_KEYWORD,
					"."
				});
			}

			public class MATURITYDELTA
			{
				public static LocString NAME = "Growth Speed";

				public static LocString DESC = "Determines the amount of time needed to reach maturation.";
			}

			public class MATURITYMAX
			{
				public static LocString NAME = "Life Cycle";

				public static LocString DESC = "The amount of time it takes this plant to grow.";
			}

			public class ROTDELTA
			{
				public static LocString NAME = "Freshness";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"Food items become stale at fifty percent ",
					UI.PRE_KEYWORD,
					"Freshness",
					UI.PST_KEYWORD,
					", and rot at zero percent"
				});
			}

			public class BECKONINGDELTA
			{
				public static LocString NAME = "Accu-moo-lation";

				public static LocString DESC = "Accu-moo-lation increases when this critter eats.";
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

				public static LocString TOOLTIP = "This is the base speed with which critters produce new " + UI.PRE_KEYWORD + "Eggs" + UI.PST_KEYWORD;
			}

			public class BASE_INCUBATION_RATE
			{
				public static LocString NAME = "Base Incubation Rate";
			}

			public class BASE_PRODUCTION_RATE
			{
				public static LocString NAME = "Base production rate";
			}

			public class SCALE_GROWTH_RATE
			{
				public static LocString NAME = "Scale Regrowth Rate";
			}

			public class ELEMENT_GROWTH_RATE
			{
				public static LocString NAME = "Quill Regrowth Rate";
			}

			public class INCUBATOR_SONG
			{
				public static LocString NAME = "Lullabied";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This egg was recently sung to by a kind Duplicant\n\nIncreased ",
					UI.PRE_KEYWORD,
					"Incubation Rate",
					UI.PST_KEYWORD,
					"\n\nDuplicants must possess the ",
					UI.PRE_KEYWORD,
					"Critter Ranching",
					UI.PST_KEYWORD,
					" skill to sing to eggs"
				});
			}

			public class EGGHUG
			{
				public static LocString NAME = "Cuddled";

				public static LocString TOOLTIP = "This egg was recently hugged by an affectionate critter\n\nIncreased " + UI.PRE_KEYWORD + "Incubation Rate" + UI.PST_KEYWORD;
			}

			public class HUGGINGFRENZY
			{
				public static LocString NAME = "Hugging Spree";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter was recently hugged by a Duplicant and is feeling extra affectionate\n\nWhile in this state, it hugs ",
					UI.PRE_KEYWORD,
					"Eggs",
					UI.PST_KEYWORD,
					" more frequently"
				});
			}

			public class INCUBATING
			{
				public static LocString NAME = "Incubating";

				public static LocString TOOLTIP = "This egg is happily incubating";
			}

			public class INCUBATING_SUPPRESSED
			{
				public static LocString NAME = "Growth Suppressed";

				public static LocString TOOLTIP = "Environmental conditions are preventing this egg from developing\n\nIt will not hatch if current conditions continue";
			}

			public class GOTMILK
			{
				public static LocString NAME = "Hydrated";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter recently drank ",
					UI.PRE_KEYWORD,
					"Brackene",
					UI.PST_KEYWORD,
					"\n\nIt doesn't mind overcrowding quite as much right now"
				});
			}

			public class RANCHED
			{
				public static LocString NAME = "Groomed";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter has recently been attended to by a kind Duplicant\n\nDuplicants must possess the ",
					UI.PRE_KEYWORD,
					"Critter Ranching",
					UI.PST_KEYWORD,
					" skill to care for critters"
				});
			}

			public class HAPPY_WILD
			{
				public static LocString NAME = "Mood: Happy";

				public static LocString TOOLTIP = "This critter's in high spirits because its needs are being exceeded";
			}

			public class HAPPY_TAME
			{
				public static LocString NAME = "Mood: Happy";

				public static LocString TOOLTIP = "This critter's in high spirits because its needs are being exceeded\n\nIt will lay more eggs as a result";
			}

			public class NEUTRAL
			{
				public static LocString NAME = "Mood: Satisfied";

				public static LocString TOOLTIP = "This critter's needs are being adequately met\n\nIt will produce its default amount of materials";
			}

			public class GLUM
			{
				public static LocString NAME = "Mood: Glum";

				public static LocString TOOLTIP = "This critter's feeling down because its needs aren't being met\n\nIt will produce less materials as a result";
			}

			public class CRITTER_TEMPERATURE_UNCOMFORTABLE
			{
				public static LocString NAME = "Uncomfortable Body Temperature";

				public static LocString TOOLTIP = "";
			}

			public class CRITTER_TEMPERATURE_DEADLY
			{
				public static LocString NAME = "Deadly Temperature";

				public static LocString TOOLTIP = "";
			}

			public class MISERABLE
			{
				public static LocString NAME = "Mood: Miserable";

				public static LocString TOOLTIP = "This critter's wallowing in despair because its needs aren't being met\n\nIt will produce less materials as a result, and will not lay eggs";
			}

			public class ATE_FROM_FEEDER
			{
				public static LocString NAME = "Ate From Feeder";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter is getting more ",
					UI.PRE_KEYWORD,
					"Tame",
					UI.PST_KEYWORD,
					" because it ate from a feeder"
				});
			}

			public class WILD
			{
				public static LocString NAME = "Wild";

				public static LocString TOOLTIP = "This critter is wild";
			}

			public class AGE
			{
				public static LocString NAME = "Aging";

				public static LocString TOOLTIP = "Time takes its toll on all things";
			}

			public class BABY
			{
				public static LocString NAME = "Tiny Baby!";

				public static LocString TOOLTIP = "This critter will grow into an adult as it ages and becomes wise to the ways of the world";
			}

			public class TAME
			{
				public static LocString NAME = "Tame";

				public static LocString TOOLTIP = "This critter is " + UI.PRE_KEYWORD + "Tame" + UI.PST_KEYWORD;
			}

			public class OUT_OF_CALORIES
			{
				public static LocString NAME = "Starving";

				public static LocString TOOLTIP = "Get this critter something to eat!";
			}

			public class FUTURE_OVERCROWDED
			{
				public static LocString NAME = "Cramped";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This ",
					UI.PRE_KEYWORD,
					"Room",
					UI.PST_KEYWORD,
					" will become too crowded once all nearby ",
					UI.PRE_KEYWORD,
					"Eggs",
					UI.PST_KEYWORD,
					" hatch\n\nThe selected critter has slowed its ",
					UI.PRE_KEYWORD,
					"Reproduction",
					UI.PST_KEYWORD,
					" to prevent further overpopulation"
				});
			}

			public class OVERCROWDED
			{
				public static LocString NAME = "Crowded";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter isn't comfortable with so many other critters in a ",
					UI.PRE_KEYWORD,
					"Room",
					UI.PST_KEYWORD,
					" of this size"
				});

				public static LocString FISHTOOLTIP = "This critter is uncomfortable with either the size of this pool, or the number of other critters sharing it";
			}

			public class CONFINED
			{
				public static LocString NAME = "Confined";

				public static LocString TOOLTIP = "This critter is trapped inside a door, tile, or confined space\n\nSounds uncomfortable!";
			}

			public class DIVERGENTPLANTTENDED
			{
				public static LocString NAME = "Sweetle Tending";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"A ",
					CREATURES.SPECIES.DIVERGENT.VARIANT_BEETLE.NAME,
					" rubbed against this ",
					UI.PRE_KEYWORD,
					"Plant",
					UI.PST_KEYWORD,
					" for a tiny growth boost"
				});
			}

			public class DIVERGENTPLANTTENDEDWORM
			{
				public static LocString NAME = "Grubgrub Rub";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"A ",
					CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.NAME,
					" rubbed against this ",
					UI.PRE_KEYWORD,
					"Plant",
					UI.PST_KEYWORD,
					", dramatically boosting growth"
				});
			}

			public class MOOWELLFED
			{
				public static LocString NAME = "Welcoming Moo'd";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter's recent meal is boosting their ",
					UI.PRE_KEYWORD,
					"Brackene",
					UI.PST_KEYWORD,
					" supply and making them wish all their friends were here\n\nThey're thinking about calling a new Gassy Moo to this asteroid"
				});
			}

			public class WOODDEERWELLFED
			{
				public static LocString NAME = "Just Ate";

				public static LocString TOOLTIP = "This critter's recent meal is boosting their antler growth";
			}

			public class ICEBELLYWELLFED
			{
				public static LocString NAME = "Just Ate";

				public static LocString TOOLTIP = "This critter's recent meal is boosting their fur growth";
			}

			public class GOLDBELLYWELLFED
			{
				public static LocString NAME = "Regal Meal";

				public static LocString TOOLTIP = string.Concat(new string[]
				{
					"This critter's recent meal is boosting their ",
					UI.PRE_KEYWORD,
					"Regal Bammoth Crest",
					UI.PST_KEYWORD,
					" growth"
				});
			}

			public class CRITTERCONDOINTERACTEFFECT
			{
				public static LocString NAME = "Cozy";

				public static LocString TOOLTIP = "This critter recently enjoyed a visit to a critter condo";
			}

			public class UNDERWATERCRITTERCONDOINTERACTEFFECT
			{
				public static LocString TOOLTIP = "This critter recently enjoyed a visit to a water fort";
			}

			public class AIRBORNECRITTERCONDOINTERACTEFFECT
			{
				public static LocString TOOLTIP = "This critter recently nested in an airborne critter condo";
			}
		}

		public class FERTILITY_MODIFIERS
		{
			public class DIET
			{
				public static LocString NAME = "Diet";

				public static LocString DESC = "Eats: {0}";
			}

			public class NEARBY_CREATURE
			{
				public static LocString NAME = "Nearby Critters";

				public static LocString DESC = "Penned with: {0}";
			}

			public class NEARBY_CREATURE_NEG
			{
				public static LocString NAME = "No Nearby Critters";

				public static LocString DESC = "Not penned with: {0}";
			}

			public class TEMPERATURE
			{
				public static LocString NAME = "Temperature";

				public static LocString DESC = "Body temperature: Between {0} and {1}";
			}

			public class CROPTENDING
			{
				public static LocString NAME = "Crop Tending";

				public static LocString DESC = "Tends to: {0}";
			}

			public class LIVING_IN_ELEMENT
			{
				public static LocString NAME = "Habitat";

				public static LocString DESC = "Dwells in {0}";

				public static LocString UNBREATHABLE = "Dwells in unbreathable" + UI.FormatAsLink("Gas", "UNBREATHABLE");

				public static LocString LIQUID = "Dwells in " + UI.FormatAsLink("Liquid", "LIQUID");
			}
		}
	}
}
