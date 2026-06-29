using System;

namespace STRINGS
{
	public class MISC
	{
		public class TAGS
		{
			public static LocString OTHER = "Miscellaneous";

			public static LocString FILTER = "Filtration Medium";

			public static LocString ICEORE = "Ice";

			public static LocString PHOSPHORUS = "Phosphorus";

			public static LocString BUILDABLERAW = "Raw Mineral";

			public static LocString BUILDABLEPROCESSED = "Refined Mineral";

			public static LocString BUILDABLEANY = "Generic Buildable";

			public static LocString REFINEDMETAL = "Refined Metal";

			public static LocString METAL = "Raw Metal";

			public static LocString PRECIOUSMETAL = "Precious Metal";

			public static LocString RAWPRECIOUSMETAL = "Raw Precious Metal";

			public static LocString ALLOY = "Alloy";

			public static LocString CRUSHABLE = "Crushable";

			public static LocString BAGABLECREATURE = "Critters";

			public static LocString LIFE = "Life";

			public static LocString LIQUIFIABLE = "Liquifiable";

			public static LocString LIQUID = "Liquid";

			public static LocString SPECIAL = "Special";

			public static LocString FARMABLE = "Cultivable Soil";

			public static LocString AGRICULTURE = "Agriculture";

			public static LocString COAL = "Coal";

			public static LocString BLEACHSTONE = "Bleach Stone";

			public static LocString ORGANICS = "Organic";

			public static LocString CONSUMABLEORE = "Consumable Ore";

			public static LocString ORE = "Ore";

			public static LocString BREATHABLE = "Breathable Gas";

			public static LocString UNBREATHABLE = "Unbreathable";

			public static LocString GAS = "Gas";

			public static LocString BURNS = "Flammable";

			public static LocString UNSTABLE = "Unstable";

			public static LocString TOXIC = "Toxic";

			public static LocString MIXTURE = "Mixture";

			public static LocString SOLID = "Solid";

			public static LocString INDUSTRIALPRODUCT = "Industrial Product";

			public static LocString INDUSTRIALINGREDIENT = "Industrial Ingredient";

			public static LocString CLOTHES = "Clothing";

			public static LocString EMITSLIGHT = "Light Emitter";

			public static LocString BED = "Bed";

			public static LocString MESSSTATION = "Dining Table";

			public static LocString SUIT = "Suit";

			public static LocString MULTITOOL = "Multitool";

			public static LocString CLINIC = "Clinic";

			public static LocString RELAXATION_POINT = "Leisure Area";

			public static LocString SOLIDMATERIAL = "Solid Material";

			public static LocString EXTRUDABLE = "Extrudable";

			public static LocString PLUMBABLE = "Plumbable";

			public static LocString COMPOSTABLE = "Compostable";

			public static LocString COMPOSTBASICPLANTFOOD = "Compost Muckroot";

			public static LocString EDIBLE = "Edible";

			public static LocString COOKINGINGREDIENT = "Cooking Ingredient";

			public static LocString MEDICINE = "Medicine";

			public static LocString SEED = "Seed";

			public static LocString ANYWATER = "Water Based";

			public static LocString MARKEDFORCOMPOST = "Compost Edibles";

			public static LocString COMPOSTMEAT = "Compost Meat";

			public static LocString PICKLED = "Pickled";

			public static LocString PLASTIC = "Plastic";

			public static LocString TOILET = "Toilet";

			public static LocString MASSAGE_TABLE = "Massage Table";

			public static LocString POWERSTATION = "Power Station";

			public static LocString FARMSTATION = "Farm Station";

			public static LocString MACHINE_SHOP = "Machine Shop";

			public static LocString ANTISEPTIC = "Antiseptic";

			public static LocString OIL = "Hydrocarbon";

			public static LocString DECORATION = "Decoration";

			public static LocString EGG = "Critter Egg";

			public static LocString GENE_SHUFFLER = "Neural Vacillator";

			public static LocString FARMING = "Farm Build-Delivery";

			public static LocString RESEARCH = "Research Delivery";

			public static LocString POWER = "Generator Delivery";

			public static LocString BUILDING = "Build Dig-Delivery";

			public static LocString COOKING = "Cook Delivery";

			public static LocString FABRICATING = "Fabricate Delivery";

			public static LocString WIRING = "Wire Build-Delivery";

			public static LocString ART = "Art Build-Delivery";

			public static LocString DOCTORING = "Care Delivery";

			public static LocString CONVEYOR = "Shipping Build";
		}

		public class STATUSITEMS
		{
			public class OXYROCK
			{
				public class NEIGHBORSBLOCKED
				{
					public static LocString NAME = "Oxylite blocked";

					public static LocString TOOLTIP = "This " + ELEMENTS.OXYROCK.NAME + " deposit is not exposed to air and cannot emit " + ELEMENTS.OXYGEN.NAME;
				}

				public class OVERPRESSURE
				{
					public static LocString NAME = "Inert";

					public static LocString TOOLTIP = "Environmental air pressure is too high for this " + ELEMENTS.OXYROCK.NAME + " deposit to emit " + ELEMENTS.OXYGEN.NAME;
				}
			}

			public class OXYROCKBLOCKED
			{
				public static LocString NAME = "{BlockedString}";

				public static LocString TOOLTIP = "This " + ELEMENTS.OXYROCK.NAME + " deposit has no room to emit " + ELEMENTS.OXYGEN.NAME;
			}

			public class OXYROCKEMITTING
			{
				public static LocString NAME = BUILDING.STATUSITEMS.EMITTINGOXYGENAVG.NAME;

				public static LocString TOOLTIP = BUILDING.STATUSITEMS.EMITTINGOXYGENAVG.TOOLTIP;
			}

			public class OXYROCKINACTIVE
			{
				public static LocString NAME = "Inert";

				public static LocString TOOLTIP = "Environmental air pressure is too high for this " + ELEMENTS.OXYROCK.NAME + " deposit to emit " + ELEMENTS.OXYGEN.NAME;
			}

			public class BLEACHSTONE
			{
				public class NEIGHBORSBLOCKED
				{
					public static LocString NAME = "Bleachstone blocked";

					public static LocString TOOLTIP = "This " + ELEMENTS.BLEACHSTONE.NAME + " deposit is not exposed to air and cannot emit " + ELEMENTS.CHLORINE.NAME;
				}

				public class OVERPRESSURE
				{
					public static LocString NAME = "Inert";

					public static LocString TOOLTIP = "Environmental air pressure is too high for this " + ELEMENTS.BLEACHSTONE.NAME + " deposit to emit " + ELEMENTS.CHLORINE.NAME;
				}
			}

			public class BLEACHSTONEBLOCKED
			{
				public static LocString NAME = "{BlockedString}";

				public static LocString TOOLTIP = "This " + ELEMENTS.BLEACHSTONE.NAME + " deposit has no room to emit " + ELEMENTS.CHLORINE.NAME;
			}

			public class BLEACHSTONEEMITTING
			{
				public static LocString NAME = BUILDING.STATUSITEMS.EMITTINGGASAVG.NAME;

				public static LocString TOOLTIP = BUILDING.STATUSITEMS.EMITTINGGASAVG.TOOLTIP;
			}

			public class BLEACHSTONEINACTIVE
			{
				public static LocString NAME = "Inert";

				public static LocString TOOLTIP = "Environmental air pressure is too high for this " + ELEMENTS.BLEACHSTONE.NAME + " deposit to emit " + ELEMENTS.CHLORINE.NAME;
			}

			public class EDIBLE
			{
				public static LocString NAME = "Rations: {0}";

				public static LocString TOOLTIP = "Can provide " + UI.FormatAsLink("{0}", "KCAL") + " of energy to Duplicants";
			}

			public class MARKEDFORDISINFECTION
			{
				public static LocString NAME = "Disinfection Pending";

				public static LocString TOOLTIP = "Awaiting a Duplicant to disinfect";
			}

			public class PENDINGCLEAR
			{
				public static LocString NAME = "Sweep Errand Assigned";

				public static LocString TOOLTIP = "Awaiting a Duplicant to sweep";
			}

			public class MARKEDFORCOMPOST
			{
				public static LocString NAME = "Marked For Compost";

				public static LocString TOOLTIP = "Awaiting a Duplicant to compost";
			}

			public class NOCLEARLOCATIONSAVAILABLE
			{
				public static LocString NAME = "No Sweep Destination";

				public static LocString TOOLTIP = "There are no valid destinations for this object to be swept to";
			}

			public class PENDINGHARVEST
			{
				public static LocString NAME = "Harvest Errand Assigned";

				public static LocString TOOLTIP = "Awaiting a Duplicant to harvest";
			}

			public class PENDINGUPROOT
			{
				public static LocString NAME = "Uproot Errand Assigned";

				public static LocString TOOLTIP = "Awaiting a Duplicant to uproot";
			}

			public class WAITINGFORDIG
			{
				public static LocString NAME = "Dig Errand Assigned";

				public static LocString TOOLTIP = "Awaiting a Duplicant to dig";
			}

			public class WAITINGFORMOP
			{
				public static LocString NAME = "Mop Errand Assigned";

				public static LocString TOOLTIP = "Awaiting a Duplicant to mop";
			}

			public class NOTMARKEDFORHARVEST
			{
				public static LocString NAME = "No Harvest Pending";

				public static LocString TOOLTIP = "Use the Harvest Tool to mark this plant for harvest";
			}

			public class ELEMENTALCATEGORY
			{
				public static LocString NAME = "{Category}";

				public static LocString TOOLTIP = "The selected object belongs to the {Category} resource category";
			}

			public class ELEMENTALMASS
			{
				public static LocString NAME = "{Mass}";

				public static LocString TOOLTIP = "The selected object has a mass of {Mass}";
			}

			public class ELEMENTALDISEASE
			{
				public static LocString NAME = "{Disease}";

				public static LocString TOOLTIP = "Current disease: {Disease}";
			}

			public class ELEMENTALTEMPERATURE
			{
				public static LocString NAME = "{Temp}";

				public static LocString TOOLTIP = "The selected object is currently {Temp}";
			}

			public class BURIEDITEM
			{
				public static LocString NAME = "Buried Object";

				public static LocString TOOLTIP = "Something seems to be hidden here";
			}

			public class HEALTHSTATUS
			{
				public class PERFECT
				{
					public static LocString NAME = "None";

					public static LocString TOOLTIP = "This Duplicant is in peak condition";
				}

				public class ALRIGHT
				{
					public static LocString NAME = "None";

					public static LocString TOOLTIP = "This Duplicant is none the worse for wear";
				}

				public class SCUFFED
				{
					public static LocString NAME = "Minor";

					public static LocString TOOLTIP = "This Duplicant has a few scrapes and bruises";
				}

				public class INJURED
				{
					public static LocString NAME = "Moderate";

					public static LocString TOOLTIP = "This Duplicant needs some patching up";
				}

				public class CRITICAL
				{
					public static LocString NAME = "Severe";

					public static LocString TOOLTIP = "This Duplicant is in serious need of medical attention";
				}

				public class INCAPACITATED
				{
					public static LocString NAME = "Paralyzing";

					public static LocString TOOLTIP = "This Duplicant will die if they do not receive medical attention";
				}

				public class DEAD
				{
					public static LocString NAME = "Conclusive";

					public static LocString TOOLTIP = "This Duplicant won't be getting back up";
				}
			}

			public class HIT
			{
				public static LocString NAME = "{targetName} took {damageAmount} damage from {attackerName}'s attack!";
			}

			public class OREMASS
			{
				public static LocString NAME = MISC.STATUSITEMS.ELEMENTALMASS.NAME;

				public static LocString TOOLTIP = MISC.STATUSITEMS.ELEMENTALMASS.TOOLTIP;
			}

			public class ORETEMP
			{
				public static LocString NAME = MISC.STATUSITEMS.ELEMENTALTEMPERATURE.NAME;

				public static LocString TOOLTIP = MISC.STATUSITEMS.ELEMENTALTEMPERATURE.TOOLTIP;
			}

			public class TREEFILTERABLETAGS
			{
				public static LocString NAME = "{Tags}";

				public static LocString TOOLTIP = "{Tags}";
			}

			public class SPOUTOVERPRESSURE
			{
				public static LocString NAME = "Over pressure {StudiedDetails}";

				public static LocString TOOLTIP = "Spout cannot vent due to high environmental pressure";

				public static LocString STUDIED = "(idle in {Time})";
			}

			public class SPOUTEMITTING
			{
				public static LocString NAME = "Venting {StudiedDetails}";

				public static LocString TOOLTIP = "This geyser is erupting";

				public static LocString STUDIED = "(idle in {Time})";
			}

			public class SPOUTPRESSUREBUILDING
			{
				public static LocString NAME = "Rising pressure {StudiedDetails}";

				public static LocString TOOLTIP = "This geyser's internal pressure is steadily building";

				public static LocString STUDIED = "(erupts in {Time})";
			}

			public class SPOUTIDLE
			{
				public static LocString NAME = "Idle {StudiedDetails}";

				public static LocString TOOLTIP = "This geyser is not currently erupting";

				public static LocString STUDIED = "(erupts in {Time})";
			}

			public class SPOUTDORMANT
			{
				public static LocString NAME = "Dormant";

				public static LocString TOOLTIP = "This geyser's geoactivity has halted" + UI.HORIZONTAL_BR_RULE + "It won't erupt again for some time";
			}

			public class PICKUPABLEUNREACHABLE
			{
				public static LocString NAME = "Unreachable";

				public static LocString TOOLTIP = "Duplicants cannot reach this object";
			}

			public class PRIORITIZED
			{
				public static LocString NAME = "High Priority";

				public static LocString TOOLTIP = "This errand has been mark as important and will be preferred over other pending tasks";
			}

			public class USING
			{
				public static LocString NAME = "Using {Target}";

				public static LocString TOOLTIP = "{Target} is currently in use";
			}

			public class ORDERATTACK
			{
				public static LocString NAME = "Pending Attack";

				public static LocString TOOLTIP = "Waiting for a Duplicant to murderize this defenseless critter";
			}

			public class ORDERCAPTURE
			{
				public static LocString NAME = "Pending Wrangle";

				public static LocString TOOLTIP = "Waiting for a Duplicant to capture this critter" + UI.HORIZONTAL_BR_RULE + "Only Duplicants trained as Ranchers can catch critters without traps";
			}

			public class OPERATING
			{
				public static LocString NAME = "In Use";

				public static LocString TOOLTIP = "This object is currently being used";
			}

			public class CLEANING
			{
				public static LocString NAME = "Cleaning";

				public static LocString TOOLTIP = "This building is currently being cleaned";
			}

			public class REGIONISBLOCKED
			{
				public static LocString NAME = "Blocked";

				public static LocString TOOLTIP = "Undug material is blocking off an essential tile";
			}

			public class STUDIED
			{
				public static LocString NAME = "Analysis Complete";

				public static LocString TOOLTIP = "Information on this Natural Feature has been compiled below.";
			}

			public class AWAITINGSTUDY
			{
				public static LocString NAME = "Analysis Pending";

				public static LocString TOOLTIP = "New information on this Natural Feature will be compiled once the field study is complete";
			}
		}

		public class POPFX
		{
			public static LocString RESOURCE_EATEN = "Resource Eaten";
		}

		public class NOTIFICATIONS
		{
			public class BASICCONTROLS
			{
				public static LocString NAME = "TIP: Basic Controls";

				public static LocString MESSAGEBODY = string.Concat(new string[]
				{
					"• Use the ",
					UI.FormatAsLink("WASD", "CONTROLS"),
					" keys to pan your camera and the ",
					UI.FormatAsLink("MOUSE WHEEL", "CONTROLS"),
					" to zoom in or out. <color=#F44A47><b>[H]</b></color> will return your screen to the Printing Pod.\n\n• Your simulation can be sped up or slowed down by using the speed buttons in the top left, or by pressing <color=#F44A47><b>[TAB]</b></color>.\n\n• <color=#F44A47><b>[SPACE]</b></color> will pause or resume your game."
				});

				public static LocString TOOLTIP = "Helpful tips to get you started";
			}

			public class WELCOMEMESSAGE
			{
				public static LocString NAME = "TIP: Colony Management";

				public static LocString MESSAGEBODY = string.Concat(new string[]
				{
					"Duplicants are self-motivated and require no individual management in order to perform errands in the colony.\n\nYou can use the ",
					UI.FormatAsLink("DIG TOOL", "TOOLS"),
					" <color=#F44A47><b>[G]</b></color> and the ",
					UI.FormatAsLink("Build menu", "misc"),
					" in the lower left of the screen to begin planning errands. Once you've placed a few errands, your Duplicants will automatically get to work for you."
				});

				public static LocString TOOLTIP = "Learn how to get Duplicants to do your bidding";
			}

			public class STRESSMANAGEMENTMESSAGE
			{
				public static LocString NAME = "TIP: Stress Management";

				public static LocString MESSAGEBODY = string.Concat(new string[]
				{
					"Duplicants are fragile creatures and must be kept mentally healthy in order to function. Subpar conditions will increase Duplicants' ",
					UI.FormatAsLink("Stress", "STRESS"),
					", while improved conditions will decrease it. Too much ",
					UI.FormatAsLink("Stress", "STRESS"),
					" will cause Duplicants to have a nervous breakdown.\n\nSelect a Duplicant and mouse over ",
					UI.FormatAsLink("Stress", "STRESS"),
					" in their INFORMATION TAB to view their individual ",
					UI.FormatAsLink("Stress", "STRESS"),
					" factors."
				});

				public static LocString TOOLTIP = "Learn how to keep Duplicants happy and productive";
			}

			public class TASKPRIORITIESMESSAGE
			{
				public static LocString NAME = "TIP: Errand Priorities";

				public static LocString MESSAGEBODY = "Duplicants will perform pending errands in order of most to least urgent. For example, Duplicants will always harvest " + UI.FormatAsLink("Food", "FOOD") + " before they build, and always build new structures before they dig.\n\nOpen the ERRANDS TAB <color=#F44A47><b>[J]</b></color> to set which errands Duplicants may perform, or specialize skilled Duplicants for specific types of work.";

				public static LocString TOOLTIP = "Learn how to manage Duplicants' priorities";
			}

			public class MOPPINGMESSAGE
			{
				public static LocString NAME = "TIP: Polluted Water";

				public static LocString MESSAGEBODY = string.Concat(new string[]
				{
					ELEMENTS.DIRTYWATER.NAME,
					" emits ",
					ELEMENTS.CONTAMINATEDOXYGEN.NAME,
					" and can accelerate the spread of ",
					UI.FormatAsLink("Disease", "DISEASE"),
					" through your base.\n\nSmall accidents can be cleaned up by clicking and dragging with the ",
					UI.FormatAsLink("Mop tool", "misc"),
					" <color=#F44A47><b>[M]</b></color>, while larger bodies may be worth filtering. Polluted water also ",
					UI.FormatAsLink("stress", "STRESS"),
					" out Duplicants that come into contact with it, so it is best removed quickly for your colony's safety."
				});

				public static LocString TOOLTIP = "Learn how to handle polluted materials";
			}

			public class LOCOMOTIONMESSAGE
			{
				public static LocString NAME = "TIP: Duplicant Movement";

				public static LocString MESSAGEBODY = "Duplicants must be able to reach their errands in order to work. When placing errands, keep in mind that Duplicants can only climb obstacles two tiles high, and are themselves two tiles tall.\n\nIf you are unsure if a errand you've placed is accessible, select a Duplicant and click SHOW NAVIGATION to view all areas within their reach.";

				public static LocString TOOLTIP = "Understanding your Duplicants' maneuverability";
			}

			public class PRIORITIESMESSAGE
			{
				public static LocString NAME = "TIP: Priorities";

				public static LocString MESSAGEBODY = "Duplicants will choose what work they do based on the priorities you set. To set priorities, open the Priorities menu to control master priorities, and then use the Sub-Priorities buttons under the tool menu. Many buildings also let you change their Sub-Priority level on selection.";

				public static LocString TOOLTIP = "Understanding your Duplicants' priorities";
			}

			public class FETCHINGWATERMESSAGE
			{
				public static LocString NAME = "TIP: Fetching Water";

				public static LocString MESSAGEBODY = string.Concat(new string[]
				{
					"In order to carry ",
					UI.FormatAsLink("liquid", "Liquids"),
					" from place to place, Duplicants must first bottle them for transport.\n\nBuild Pitcher Pumps over pools of liquid from the ",
					UI.FormatAsLink("Plumbing tab", "misc"),
					" of the ",
					UI.FormatAsLink("Build menu", "misc"),
					" to pump those liquids into bottles. Duplicants will then automatically deliver the bottled liquids where they need to go."
				});

				public static LocString TOOLTIP = "How to fetch a pail of water";
			}

			public class SCHEDULEMESSAGE
			{
				public static LocString NAME = "TIP: Scheduling";

				public static LocString MESSAGEBODY = "Duplicants crave structure and will only eat, sleep, work, or bathe during the times you allot for these activities.\n\nTo make the best use of your time, open the SCHEDULE TAB <color=#F44A47><b>[U]</b></color> to adjust your colony's agenda and plan how your Duplicants should utilize their day.";

				public static LocString TOOLTIP = "Learn how to schedule your Duplicants' time";
			}

			public class THERMALCOMFORT
			{
				public static LocString NAME = "TIP: Duplicant Temperature";

				public static LocString TOOLTIP = "Help your Duplicants keep their cool";

				public static LocString MESSAGEBODY = string.Concat(new string[]
				{
					"Environments that are extremely ",
					UI.FormatAsLink("Hot", "HEAT"),
					" or ",
					UI.FormatAsLink("Cold", "HEAT"),
					" will affect your Duplicants' internal body temperature and cause ",
					UI.FormatAsLink("Stress", "STRESS"),
					".\n\nThe THERMAL COMFORT OVERLAY <color=#F44A47><b>[F4]</b></color> will allow you to view all areas where Duplicants cannot regulate their temperature and will become uncomfortable."
				});
			}

			public class TUTORIAL_OVERHEATING
			{
				public static LocString NAME = "TIP: Building Temperature";

				public static LocString TOOLTIP = "Learn how to prevent meltdowns";

				public static LocString MESSAGEBODY = "When constructing buildings, take note of their " + UI.FormatAsLink("Overheat Temperature", "HEAT") + " and plan their locations accordingly. Maintaining low ambient temperatures and good ventilation will help keep building temperatures down.\n\nBuildings that exceed their Overheat Temperature will begin to take damage and if left untended, will meltdown and stop functioning until repaired.";
			}

			public class LOTS_OF_GERMS
			{
				public static LocString NAME = "TIP: Germs and Disease";

				public static LocString TOOLTIP = "Learn about the risks of Duplicant disease";

				public static LocString MESSAGEBODY = string.Concat(new string[]
				{
					UI.FormatAsLink("Germs", "DISEASE"),
					" are an invisible peril that cause ",
					UI.FormatAsLink("Disease", "DISEASE"),
					" in your Duplicants. The ",
					UI.FormatAsLink("Germ overlay", "misc"),
					" <color=#F44A47><b>[F9]</b></color> will allow you to view all germ concentrations in your colony and the sources spawning them.\n\nWash Basins can be built in dirty areas from the ",
					UI.FormatAsLink("Medicine tab", "misc"),
					" <color=#F44A47><b>[8]</b></color> to tell your Duplicants to where to wash up. If you keep your base clean and your Duplicants hygienic, their ",
					UI.FormatAsLink("Immunity", "IMMUNE SYSTEM"),
					" will handle the rest."
				});
			}

			public class BEING_INFECTED
			{
				public static LocString NAME = "TIP: Duplicant Immune Systems";

				public static LocString TOOLTIP = "Keep your Duplicants in peak health";

				public static LocString MESSAGEBODY = string.Concat(new string[]
				{
					"When Duplicants come into contact with various ",
					UI.FormatAsLink("Germs", "DISEASE"),
					", they'll need to expend points of ",
					UI.FormatAsLink("Immunity", "IMMUNE SYSTEM"),
					" to resist them and remain healthy. If repeated exposes causes their Immunity to drop to 0%, they'll be unable to resist germs and will contract the next disease they encounter.\n\nDoors with Access Permissions can be built from the BASE TAB<color=#F44A47> <b>[1]</b></color> of the ",
					UI.FormatAsLink("Build menu", "misc"),
					" to block Duplicants from entering biohazardous areas while they recover their spent immunity points."
				});
			}

			public class DISEASE_COOKING
			{
				public static LocString NAME = "TIP: Food Safety";

				public static LocString TOOLTIP = "Learn how to manage food contamination";

				public static LocString MESSAGEBODY = string.Concat(new string[]
				{
					"The ",
					UI.FormatAsLink("Food", "FOOD"),
					" your Duplicants cook will only ever be as clean as the ingredients used to make it. Storing food in sterile or ",
					UI.FormatAsLink("Refrigerated", UI.StripLinkFormatting(BUILDINGS.PREFABS.REFRIGERATOR.NAME)),
					" environments will keep food free of ",
					UI.FormatAsLink("Germs", "DISEASE"),
					", while carefully placed hygiene stations like ",
					BUILDINGS.PREFABS.WASHBASIN.NAME,
					" or ",
					BUILDINGS.PREFABS.SHOWER.NAME,
					" will prevent your cooks from infecting the food by handling it.\n\nDangerously contaminated food can be sent to compost by clicking the ",
					UI.FormatAsLink("Compost", "misc"),
					" button on the selected item."
				});
			}

			public class SUITS
			{
				public static LocString NAME = "TIP: Exosuits";

				public static LocString TOOLTIP = "Learn how to use exosuits";

				public static LocString MESSAGEBODY = string.Concat(new string[]
				{
					UI.FormatAsLink("Exosuits", "EXOSUIT"),
					" can be equipped to protect your Duplicants from environmental hazards like extreme ",
					UI.FormatAsLink("Heat", "Heat"),
					", airborne ",
					UI.FormatAsLink("Germs", "DISEASE"),
					", or unbreathable ",
					UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
					". In order to utilize these suits, you'll need to hook up an Exosuit Dock to a Suit Checkpoint, then store one of your suits inside.\n\nDuplicants will equip an exosuit when they walk past the Checkpoint in the chosen direction, and will unequip their suit when walking back the opposite way."
				});
			}

			public class NOMESSAGES
			{
				public static LocString NAME = string.Empty;

				public static LocString TOOLTIP = string.Empty;
			}

			public class NOALERTS
			{
				public static LocString NAME = string.Empty;

				public static LocString TOOLTIP = string.Empty;
			}

			public class NEWTRAIT
			{
				public static LocString NAME = "{0} has developed a trait";

				public static LocString TOOLTIP = "{0} has developed the trait(s):\n    • {1}";
			}

			public class RESEARCHCOMPLETE
			{
				public static LocString NAME = "Research Complete";

				public static LocString MESSAGEBODY = "Eureka! {0} Technology has been unlocked.\n\nNew buildings have become available:\n  • {1}";

				public static LocString TOOLTIP = "{0} research complete!";
			}

			public class ROLEMASTERED
			{
				public static LocString NAME = "Jobs Mastered";

				public static LocString MESSAGEBODY = "These Duplicants have mastered their jobs and may be eligible for promotion:\n{0}";

				public static LocString LINE = "\n• <b>{0}</b> mastered the {1} job";

				public static LocString TOOLTIP = "Job Mastered";
			}

			public class DUPLICANTABSORBED
			{
				public static LocString NAME = "New Duplicants have been reabsorbed";

				public static LocString MESSAGEBODY = "New Duplicants are no longer available for printing.\nCountdown to the next production was rebooted.";

				public static LocString TOOLTIP = "The printable Duplicants have been reabsorbed";
			}

			public class DUPLICANTDIED
			{
				public static LocString NAME = "Duplicants have died";

				public static LocString TOOLTIP = "These Duplicants have died:";
			}

			public class FOODROT
			{
				public static LocString NAME = "Food has decayed";

				public static LocString TOOLTIP = "These " + UI.FormatAsLink("Food", "FOOD") + " items have rotted and are no longer edible:\n• {0}";
			}

			public class FOODSTALE
			{
				public static LocString NAME = "Food has become stale";

				public static LocString TOOLTIP = "These " + UI.FormatAsLink("Food", "FOOD") + " items have become stale and could rot if not stored:";
			}

			public class REDALERT
			{
				public static LocString NAME = "Red Alert";

				public static LocString TOOLTIP = "The colony is prioritizing work over their individual well-being";
			}

			public class HEALING
			{
				public static LocString NAME = "Healing";

				public static LocString TOOLTIP = "This Duplicant is recovering from an injury";
			}

			public class UNREACHABLEITEM
			{
				public static LocString NAME = "Unreachable resources";

				public static LocString TOOLTIP = "Duplicants cannot retrieve these resources:";
			}

			public class INVALIDCONSTRUCTIONLOCATION
			{
				public static LocString NAME = "Invalid construction location";

				public static LocString TOOLTIP = "These buildings cannot be constructed in the planned areas:\n";
			}

			public class MISSINGMATERIALS
			{
				public static LocString NAME = "Missing materials";

				public static LocString TOOLTIP = "These resources are not available:";
			}

			public class BUILDINGOVERHEATED
			{
				public static LocString NAME = "Damage: Overheated";

				public static LocString TOOLTIP = "Excessive heat is damaging these buildings:\n";
			}

			public class BUILDINGCOLLAPSE
			{
				public static LocString NAME = "Building collapsed";

				public static LocString TOOLTIP = "These buildings have collapsed from a lack of foundation:\n• {0}";
			}

			public class NEEDOXYGENSOURCE
			{
				public static LocString NAME = "Insufficient oxygen generation";

				public static LocString TOOLTIP = "• Your colony has produced {EmittingRate} of " + ELEMENTS.OXYGEN.NAME + " this cycle\n• Your Duplicants have consumed {ConsumptionRate}";
			}

			public class UNREFRIGERATEDFOOD
			{
				public static LocString NAME = "Unrefrigerated Food";

				public static LocString TOOLTIP = "These " + UI.FormatAsLink("Food", "FOOD") + " items are in storage but are not refrigerated:\n";
			}

			public class FOODLOW
			{
				public static LocString NAME = "Food shortage";

				public static LocString TOOLTIP = "Your colony's " + UI.FormatAsLink("Food", "FOOD") + " reserves are low:\n    • You have {0} available\n    • Your Duplicants are consuming {1} per cycle";
			}

			public class NO_MEDICAL_COTS
			{
				public static LocString NAME = "Colony requires Med-Beds";

				public static LocString TOOLTIP = "Your sick Duplicants have nowhere to rest or receive medical care";
			}

			public class NEEDTOILET
			{
				public static LocString NAME = "Colony requires toilets";

				public static LocString TOOLTIP = "Your Duplicants have nowhere to relieve themselves";
			}

			public class NEEDFOOD
			{
				public static LocString NAME = "Colony requires a food source";

				public static LocString TOOLTIP = "Your colony will soon exhaust their supplies without a new " + UI.FormatAsLink("Food", "FOOD") + " source";
			}

			public class HYGENE_NEEDED
			{
				public static LocString NAME = "Colony requires wash basins";

				public static LocString TOOLTIP = UI.FormatAsLink("Germs", "DISEASE") + " are spreading in your colony because your Duplicants have nowhere to clean up";
			}

			public class NEEDSLEEP
			{
				public static LocString NAME = "Colony requires beds";

				public static LocString TOOLTIP = "Your Duplicants would appreciate a place to sleep";
			}

			public class NEEDENERGYSOURCE
			{
				public static LocString NAME = "Colony requires a " + UI.FormatAsLink("Power", "POWER") + " source";

				public static LocString TOOLTIP = UI.FormatAsLink("Power", "POWER") + " is required to operate electrical buildings";
			}

			public class RESOURCEMELTED
			{
				public static LocString NAME = "Resources melted";

				public static LocString TOOLTIP = "These resources have melted:";
			}

			public class VENTOVERPRESSURE
			{
				public static LocString NAME = "Vent overpressurized";

				public static LocString TOOLTIP = "These pipe systems have exited the ideal pressure range:";
			}

			public class VENTBLOCKED
			{
				public static LocString NAME = "Vent blocked";

				public static LocString TOOLTIP = "Blocked pipes have stopped these systems from functioning:";
			}

			public class OUTPUTBLOCKED
			{
				public static LocString NAME = "Output blocked";

				public static LocString TOOLTIP = "Blocked pipes have stopped these systems from functioning:";
			}

			public class BROKENMACHINE
			{
				public static LocString NAME = "Building broken";

				public static LocString TOOLTIP = "These buildings have taken significant damage and are nonfunctional:";
			}

			public class STRUCTURALDAMAGE
			{
				public static LocString NAME = "Structural damage";

				public static LocString TOOLTIP = "These buildings' structural integrity has been compromised";
			}

			public class STRUCTURALCOLLAPSE
			{
				public static LocString NAME = "Structural collapse";

				public static LocString TOOLTIP = "These buildings have collapsed:";
			}

			public class GASCLOUDWARNING
			{
				public static LocString NAME = "A gas cloud approaches";

				public static LocString TOOLTIP = "A toxic gas cloud will soon envelop the colony";
			}

			public class GASCLOUDARRIVING
			{
				public static LocString NAME = "The colony is entering a cloud of gas";

				public static LocString TOOLTIP = string.Empty;
			}

			public class GASCLOUDPEAK
			{
				public static LocString NAME = "The gas cloud is at its densest point";

				public static LocString TOOLTIP = string.Empty;
			}

			public class GASCLOUDDEPARTING
			{
				public static LocString NAME = "The gas cloud is receding";

				public static LocString TOOLTIP = string.Empty;
			}

			public class GASCLOUDGONE
			{
				public static LocString NAME = "The colony is once again in open space";

				public static LocString TOOLTIP = string.Empty;
			}

			public class AVAILABLE
			{
				public static LocString NAME = "Resource available";

				public static LocString TOOLTIP = "These resources have become available:";
			}

			public class ALLOCATED
			{
				public static LocString NAME = "Resource allocated";

				public static LocString TOOLTIP = "These resources are reserved for a planned building:";
			}

			public class INCREASEDEXPECTATIONS
			{
				public static LocString NAME = "Duplicants' expectations increased";

				public static LocString TOOLTIP = "Duplicants require better amenities over time.\nThese Duplicants have increased their expectations:";
			}

			public class NEARLYDRY
			{
				public static LocString NAME = "Nearly dry";

				public static LocString TOOLTIP = "These Duplicants will dry off soon:";
			}

			public class IMMIGRANTSLEFT
			{
				public static LocString NAME = "New Duplicants have been reabsorbed";

				public static LocString TOOLTIP = "The printable Duplicants have been Oozed";
			}

			public class LEVELUP
			{
				public static LocString NAME = "Skill increase";

				public static LocString TOOLTIP = "These Duplicants' skills have improved:";

				public static LocString TOOLTIP_PST = "\n\nTheir expectations have increased accordingly.";

				public static LocString SUFFIX = " - {0} raised to {1}";
			}

			public class GENESHUFFLER
			{
				public static LocString NAME = "Genes Shuffled";

				public static LocString TOOLTIP = "These Duplicants had their genetic makeup modified:";

				public static LocString SUFFIX = " has developed {0}";
			}

			public class HEALINGTRAITGAIN
			{
				public static LocString NAME = "New trait";

				public static LocString TOOLTIP = "These Duplicants' injuries weren't set and healed improperly. They developed traits as a result:";

				public static LocString SUFFIX = " has developed {0}";
			}

			public class COLONYLOST
			{
				public static LocString NAME = "Colony Lost";

				public static LocString TOOLTIP = "All Duplicants are dead or incapacitated";
			}

			public class FABRICATOREMPTY
			{
				public static LocString NAME = "Fabricator idle";

				public static LocString TOOLTIP = "These fabricators have no recipes queued:";
			}

			public class SUIT_DROPPED
			{
				public static LocString NAME = "No Docks available";

				public static LocString TOOLTIP = "An exosuit was dropped because there were no empty Exosuit Docks available";
			}

			public class DEATH_SUFFOCATION
			{
				public static LocString NAME = "Duplicants suffocated";

				public static LocString TOOLTIP = "These Duplicants died from a lack of " + ELEMENTS.OXYGEN.NAME + ":";
			}

			public class DEATH_FROZENSOLID
			{
				public static LocString NAME = "Duplicants have frozen";

				public static LocString TOOLTIP = "These Duplicants died from extremely low " + UI.FormatAsLink("Temperatures", "HEAT") + ":";
			}

			public class DEATH_OVERHEATING
			{
				public static LocString NAME = "Duplicants have overheated";

				public static LocString TOOLTIP = "These Duplicants died from extreme " + UI.FormatAsLink("Heat", "HEAT") + ":";
			}

			public class DEATH_STARVATION
			{
				public static LocString NAME = "Duplicants have starved";

				public static LocString TOOLTIP = "These Duplicants died from a lack of " + UI.FormatAsLink("Food", "FOOD") + ":";
			}

			public class DEATH_FELL
			{
				public static LocString NAME = "Duplicants splattered";

				public static LocString TOOLTIP = "These Duplicants fell to their deaths:";
			}

			public class DEATH_CRUSHED
			{
				public static LocString NAME = "Duplicants crushed";

				public static LocString TOOLTIP = "These Duplicants have been crushed:";
			}

			public class DEATH_SUFFOCATEDTANKEMPTY
			{
				public static LocString NAME = "Duplicants have suffocated";

				public static LocString TOOLTIP = "These Duplicants were unable to reach " + ELEMENTS.OXYGEN.NAME + " and died:";
			}

			public class DEATH_SUFFOCATEDAIRTOOHOT
			{
				public static LocString NAME = "Duplicants have suffocated";

				public static LocString TOOLTIP = "These Duplicants have asphyxiated in " + UI.FormatAsLink("heat", "HEAT") + " air:";
			}

			public class DEATH_SUFFOCATEDAIRTOOCOLD
			{
				public static LocString NAME = "Duplicants have suffocated";

				public static LocString TOOLTIP = "These Duplicants have asphyxiated in " + UI.FormatAsLink("Cold", "HEAT") + " air:";
			}

			public class DEATH_DROWNED
			{
				public static LocString NAME = "Duplicants have drowned";

				public static LocString TOOLTIP = "These Duplicants have drowned:";
			}

			public class DEATH_ENTOUMBED
			{
				public static LocString NAME = "Duplicants have been entombed";

				public static LocString TOOLTIP = "These Duplicants are trapped and need assistance:";
			}

			public class DEATH_RAPIDDECOMPRESSION
			{
				public static LocString NAME = "Duplicants pressurized";

				public static LocString TOOLTIP = "These Duplicants died in a low pressure environment:";
			}

			public class DEATH_OVERPRESSURE
			{
				public static LocString NAME = "Duplicants pressurized";

				public static LocString TOOLTIP = "These Duplicants died in a high pressure environment:";
			}

			public class DEATH_POISONED
			{
				public static LocString NAME = "Duplicants poisoned";

				public static LocString TOOLTIP = "These Duplicants died as a result of poisoning:";
			}

			public class DEATH_DISEASE
			{
				public static LocString NAME = "Duplicants have succumb to illness";

				public static LocString TOOLTIP = "These Duplicants died from an untreated " + UI.FormatAsLink("Disease", "DISEASE") + ":";
			}

			public class CIRCUIT_OVERLOADED
			{
				public static LocString NAME = "Circuit Overloaded";

				public static LocString TOOLTIP = "These wires melted due to excessive current demands on their circuits";
			}
		}

		public class PLACERS
		{
			public class DIGPLACER
			{
				public static LocString NAME = "Dig";
			}

			public class MOPPLACER
			{
				public static LocString NAME = "Mop";
			}
		}
	}
}
