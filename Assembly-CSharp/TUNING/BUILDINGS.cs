using System;
using System.Collections.Generic;

namespace TUNING
{
	public class BUILDINGS
	{
		public const float DEFAULT_STORAGE_CAPACITY = 2000f;

		public const float MINERAL_DEOXIDIZER_OXYGEN_GENERATED = 0.6f;

		public const float MINERAL_DEOXIDIZER_OXYGEN_TEMP = 303.15f;

		public const float WATER2OXYGEN_RATIO = 0.888f;

		public const float OXYGEN_CONVERSION_RATE = 1f;

		public const float AIR_FILTER_OXYGEN_CONSUME_RATE = 0.1f;

		public const float AIR_FILTER_SAND_CONSUME_RATE = 1f;

		public const float INDUSTRIAL_AIR_FILTER_OXYGEN_CONSUME_RATE = 0.3f;

		public const float AIRCONDITIONER_TEMPDELTA = -14f;

		public const float MUSHBAR_RECIPE_FABRICATIONTIME = 40f;

		public const float MUSHBAR_RECIPE_WATERKG = 75f;

		public const float MUSHBAR_RECIPE_DIRTKG = 75f;

		public const int FRIEDMUSHBAR_RECIPE_MUSHBAR_AMOUNT = 2;

		public const int BASICPLANTBAR_RECIPE_GRAIN_AMOUNT = 5;

		public const int BASICPLANTBAR_RECIPE_PRODUCTION_AMOUNT = 1;

		public const float BASICPLANTBAR_RECIPE_WATERKG = 50f;

		public const float BASICPLANTBAR_RECIPE_FABRICATIONTIME = 20f;

		public const float COMPOST_FLIP_TIME = 20f;

		public const float SMELTER_INGOT_INPUTKG = 500f;

		public const float SMELTER_INGOT_OUTPUTKG = 100f;

		public const float SMELTER_FABRICATIONTIME = 120f;

		public const float GEOREFINERY_SLAB_INPUTKG = 1000f;

		public const float GEOREFINERY_SLAB_OUTPUTKG = 200f;

		public const float GEOREFINERY_FABRICATIONTIME = 120f;

		public const float PHARMACY_FABRICATIONTIME = 40f;

		public const float PHARMACY_GENERIC_INPUTKG = 100f;

		public const float PHARMACY_GENERIC_SINGLE = 1f;

		public const float ROCKCRUSHER_FABRICATIONTIME = 80f;

		public const float ROCKCRUSHER_INPUTKG = 100f;

		public const float ROCKCRUSHER_OUTPUTKG = 20f;

		public const float MASS_BURN_RATE_COALGENERATOR = 1f;

		public const float MASS_BURN_RATE_HYDROGENGENERATOR = 0.1f;

		public static Dictionary<PlanCategory, string[]> PLANORDER = new Dictionary<PlanCategory, string[]>
		{
			{
				PlanCategory.Base,
				new string[] { "Ladder", "Tile", "MeshTile", "GasPermeableMembrane", "InsulationTile", "StorageLocker", "Door", "ManualPressureDoor", "PressureDoor" }
			},
			{
				PlanCategory.Oxygen,
				new string[] { "MineralDeoxidizer", "AlgaeHabitat", "Electrolyzer", "AirFilter", "CO2Scrubber" }
			},
			{
				PlanCategory.Power,
				new string[] { "ManualGenerator", "Generator", "HydrogenGenerator", "Wire", "InsulatedWire", "WireBridge", "Switch", "Battery", "BatteryMedium" }
			},
			{
				PlanCategory.Food,
				new string[] { "RationBox", "Refrigerator", "MicrobeMusher", "PlanterBox", "CookingStation" }
			},
			{
				PlanCategory.Utilities,
				new string[] { "LiquidCooledFan", "AirConditioner" }
			},
			{
				PlanCategory.Plumbing,
				new string[]
				{
					"Outhouse", "FlushToilet", "Shower", "LiquidConduit", "InsulatedLiquidConduit", "LiquidConduitJoiner", "LiquidConduitSplitter", "LiquidConduitBridge", "LiquidPump", "LiquidValve",
					"LiquidVent", "LiquidReservoir", "LiquidFilter", "Liquifier", "GasConduit", "InsulatedGasConduit", "GasConduitJoiner", "GasConduitSplitter", "GasConduitBridge", "GasPump",
					"GasValve", "GasVent", "GasFilter"
				}
			},
			{
				PlanCategory.Refining,
				new string[] { "WaterPurifier", "AlgaeDistillery", "Compost", "FertilizerMaker" }
			},
			{
				PlanCategory.Medical,
				new string[] { "HandSanitizer", "Apothecary", "MedicalCot", "MedicalBed" }
			},
			{
				PlanCategory.Furniture,
				new string[] { "Bed", "MassageTable", "DiningTable", "FloorLamp", "CeilingLight" }
			},
			{
				PlanCategory.Equipment,
				new string[] { "ResearchCenter", "AdvancedResearchCenter" }
			},
			{
				PlanCategory.Misc,
				new string[] { "FlowerVase", "Canvas", "Sculpture", "Grave" }
			}
		};

		public static Dictionary<string, string[]> RESEARCH = new Dictionary<string, string[]>
		{
			{
				"FarmingTech",
				new string[] { "PlanterBox", "AlgaeHabitat", "Compost" }
			},
			{
				"FineDining",
				new string[] { "CookingStation", "Refrigerator", "FertilizerMaker" }
			},
			{
				"GasPiping",
				new string[] { "GasConduit", "GasPump", "GasConduitBridge", "GasPermeableMembrane", "GasVent" }
			},
			{
				"TemperatureModulation",
				new string[] { "LiquidCooledFan", "AirConditioner", "TemperatureControlledSwitch" }
			},
			{
				"Insulation",
				new string[] { "InsulatedWire", "InsulatedGasConduit", "InsulatedLiquidConduit", "InsulationTile" }
			},
			{
				"PressureManagement",
				new string[] { "SuitFabricator", "SuitRecharger", "LiquidValve", "GasValve", "PressureDoor" }
			},
			{
				"LiquidPiping",
				new string[] { "LiquidConduit", "LiquidPump", "LiquidConduitBridge", "LiquidVent" }
			},
			{
				"SanitationSciences",
				new string[] { "HandSanitizer", "FlushToilet", "Shower", "AirFilter", "ManualPressureDoor" }
			},
			{
				"Medbay",
				new string[] { "MedicalBed", "Apothecary", "MeshTile" }
			},
			{
				"Percolation",
				new string[] { "Electrolyzer", "WaterPurifier", "AlgaeDistillery" }
			},
			{
				"AdvancedFiltration",
				new string[] { "CO2Scrubber", "GasFilter", "LiquidFilter" }
			},
			{
				"PowerRegulation",
				new string[] { "Switch", "TimedSwitch", "WireBridge", "BatteryMedium" }
			},
			{
				"Combustion",
				new string[] { "Generator", "HydrogenGenerator" }
			},
			{
				"InteriorDecor",
				new string[] { "Canvas", "Sculpture", "DiningTable", "CeilingLight" }
			}
		};

		public static string[] COMPONENT_DESCRIPTION_ORDER = new string[]
		{
			"AlgaeHabitat", "Fabricator", "MicrobeMusher", "CookingStation", "ResearchCenter", "LiquidCooledFan", "OxygenRecharger", "BuildingElementEmitter", "ElementConverter", "ElementConsumer",
			"PassiveElementConsumer", "EnergyConsumer", "AirConditioner", "Storage", "Battery", "Switch", "ManualDelivery", "ManualDeliveryKG", "AirFilter", "FlushToilet",
			"Toilet", "EnergyGenerator", "MassageTable", "Shower", "Ownable", "PlantablePlot", "RelaxationPoint", "BuildingComplete", "DecorProvider"
		};

		public static string[] GAMEOBJECT_COMPONENT_DESCRIPTION_ORDER = new string[] { "Edible", "FriedMushBar", "BasicSingleHarvestPlant", "PrickleFlower", "DiseaseTrigger", "MedicinalHerb", "MedicinalPill", "DecorProvider" };

		public class CONSTRUCTION_MASS
		{
			public static float[] TIER0 = new float[] { 25f };

			public static float[] TIER1 = new float[] { 50f };

			public static float[] TIER2 = new float[] { 100f };

			public static float[] TIER3 = new float[] { 200f };

			public static float[] TIER4 = new float[] { 400f };

			public static float[] TIER5 = new float[] { 800f };

			public static float[] TIER6 = new float[] { 1200f };

			public static float[] TIER7 = new float[] { 2000f };
		}

		public class ENERGY_CONSUMPTION_WHEN_ACTIVE
		{
			public const float TIER0 = 0f;

			public const float TIER1 = 5f;

			public const float TIER2 = 60f;

			public const float TIER3 = 120f;

			public const float TIER4 = 240f;

			public const float TIER5 = 480f;

			public const float TIER6 = 960f;

			public const float TIER7 = 2000f;
		}

		public class TEMPERATURE_MODIFICATION_WHEN_ACTIVE
		{
			public const float TIER0 = 0f;

			public const float TIER1 = 2f;

			public const float TIER2 = 4f;

			public const float TIER3 = 8f;

			public const float TIER4 = 16f;

			public const float TIER5 = 32f;

			public const float TIER6 = 64f;

			public const float TIER7 = 128f;
		}

		public class OPERATING_TEMPERATURE
		{
			public const float TIER0 = 0f;

			public const float TIER1 = 350f;

			public const float TIER2 = 400f;

			public const float TIER3 = 500f;

			public const float TIER4 = 800f;

			public const float TIER5 = 1200f;

			public const float TIER6 = 1600f;

			public const float TIER7 = 2000f;
		}

		public class MELTING_POINT
		{
			public const float TIER0 = 800f;

			public const float TIER1 = 1600f;

			public const float TIER2 = 2400f;

			public const float TIER3 = 3200f;
		}

		public class CONSTRUCTION_TIME
		{
			public const float TIER0 = 3f;

			public const float TIER1 = 10f;

			public const float TIER2 = 30f;

			public const float TIER3 = 60f;

			public const float TIER4 = 120f;

			public const float TIER5 = 240f;

			public const float TIER6 = 480f;
		}

		public class RELOCATION_TIME
		{
			public const float DECONSTRUCT = 4f;

			public const float CONSTRUCT = 4f;
		}

		public class WORK_TIME
		{
			public const float VERYSHORT_WORK_TIME = 5f;

			public const float MEDIUM_WORK_TIME = 45f;

			public const float LONG_WORK_TIME = 90f;
		}

		public class DECOR
		{
			public static DecorValues NONE = new DecorValues
			{
				decor = 0,
				radius = 1
			};

			public class BONUS
			{
				public static DecorValues TIER0 = new DecorValues
				{
					decor = 5,
					radius = 1
				};

				public static DecorValues TIER1 = new DecorValues
				{
					decor = 10,
					radius = 2
				};

				public static DecorValues TIER2 = new DecorValues
				{
					decor = 15,
					radius = 3
				};

				public static DecorValues TIER3 = new DecorValues
				{
					decor = 20,
					radius = 4
				};

				public static DecorValues TIER4 = new DecorValues
				{
					decor = 25,
					radius = 5
				};

				public static DecorValues TIER5 = new DecorValues
				{
					decor = 30,
					radius = 6
				};
			}

			public class PENALTY
			{
				public static DecorValues TIER0 = new DecorValues
				{
					decor = -5,
					radius = 1
				};

				public static DecorValues TIER1 = new DecorValues
				{
					decor = -10,
					radius = 2
				};

				public static DecorValues TIER2 = new DecorValues
				{
					decor = -15,
					radius = 3
				};

				public static DecorValues TIER3 = new DecorValues
				{
					decor = -20,
					radius = 4
				};

				public static DecorValues TIER4 = new DecorValues
				{
					decor = -20,
					radius = 5
				};

				public static DecorValues TIER5 = new DecorValues
				{
					decor = -25,
					radius = 6
				};
			}
		}

		public class MASS
		{
			public const float TIER0 = 25f;

			public const float TIER1 = 50f;

			public const float TIER2 = 100f;

			public const float TIER3 = 200f;

			public const float TIER4 = 400f;

			public const float TIER5 = 800f;

			public const float TIER6 = 1200f;

			public const float TIER7 = 2000f;
		}

		public class UPGRADES
		{
			public const float BUILDTIME_TIER0 = 120f;

			public class MATERIALTAGS
			{
				public const string METAL = "Metal";

				public const string REFINEDMETAL = "RefinedMetal";

				public const string CARBON = "Carbon";
			}

			public class MATERIALMASS
			{
				public const int TIER0 = 100;

				public const int TIER1 = 200;

				public const int TIER2 = 400;

				public const int TIER3 = 500;
			}

			public class MODIFIERAMOUNTS
			{
				public const float MANUALGENERATOR_ENERGYGENERATION = 1.2f;

				public const float MANUALGENERATOR_CAPACITY = 2f;

				public const float ELECTROLYZER_ENERGYCONSUMPTION = 0.76f;

				public const float ELECTROLYZER_LIQUIDINPUT = 2f;

				public const float ELECTROLYZER_MASSGENERATION = 1.3f;

				public const float PROPANEGENERATOR_ENERGYGENERATION = 1.6f;

				public const float PROPANEGENERATOR_HEATGENERATION = 1.6f;

				public const float MINERALDEOXIDIZER_ENERGYCONSUMPTION = 0.76f;

				public const float MINERALDEOXIDIZER_MASSGENERATION = 1.1f;

				public const float MICROBEMUSHER_LIQUIDINPUT = 2f;

				public const float GENERATOR_HEATGENERATION = 0.8f;

				public const float GENERATOR_ENERGYGENERATION = 1.3f;

				public const float TURBINE_ENERGYGENERATION = 1.2f;

				public const float TURBINE_CAPACITY = 1.2f;

				public const float SUITRECHARGER_EXECUTIONTIME = 1.2f;

				public const float SUITRECHARGER_HEATGENERATION = 1.2f;

				public const float STORAGELOCKER_CAPACITY = 2f;

				public const float SOLARPANEL_ENERGYGENERATION = 1.2f;

				public const float SMELTER_HEATGENERATION = 0.7f;
			}
		}
	}
}
