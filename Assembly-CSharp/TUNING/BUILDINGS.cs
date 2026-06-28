using System;
using System.Collections.Generic;

namespace TUNING
{
	public class BUILDINGS
	{
		public const float DEFAULT_STORAGE_CAPACITY = 2000f;

		public const float MASS_TEMPERATURE_SCALE = 0.2f;

		public const float AIRCONDITIONER_TEMPDELTA = -14f;

		public const float MAX_ENVIRONMENT_DELTA = -50f;

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

		public const float OVERHEAT_DAMAGE_INTERVAL = 7.5f;

		public const float MIN_BUILD_TEMPERATURE = 288.15f;

		public const float MAX_BUILD_TEMPERATURE = 318.15f;

		public const float MELTDOWN_TEMPERATURE = 533.15f;

		public const float REPAIR_FORCE_TEMPERATURE = 293.15f;

		public const int REPAIR_EFFECTIVENESS_BASE = 10;

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
				new string[]
				{
					"ManualGenerator",
					"Generator",
					"HydrogenGenerator",
					"MethaneGenerator",
					"Wire",
					"HighWattageWire",
					"WireBridge",
					"Battery",
					"BatteryMedium",
					"PowerTransformer",
					SwitchConfig.ID,
					TemperatureControlledSwitchConfig.ID,
					PressureSwitchLiquidConfig.ID,
					PressureSwitchGasConfig.ID
				}
			},
			{
				PlanCategory.Food,
				new string[] { "RationBox", "Refrigerator", "MicrobeMusher", "CookingStation", "PlanterBox", "Aquafarm", "FarmTile", "HydroponicFarm" }
			},
			{
				PlanCategory.Plumbing,
				new string[]
				{
					"Outhouse", "FlushToilet", "Shower", "LiquidConduit", "InsulatedLiquidConduit", "LiquidConduitBridge", "LiquidConduitSplitter", "LiquidConduitBridge", "LiquidPump", "LiquidValve",
					"LiquidVent", "LiquidReservoir", "LiquidFilter", "Liquifier", "GasConduit", "InsulatedGasConduit", "GasConduitBridge", "GasConduitSplitter", "GasConduitBridge", "GasPump",
					"GasValve", "GasVent", "GasFilter"
				}
			},
			{
				PlanCategory.Utilities,
				new string[] { "LiquidCooledFan", "AirConditioner", "SpaceHeater", "LiquidHeater" }
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
				new string[] { "ResearchCenter", "AdvancedResearchCenter", "ClothingFabricator" }
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
				new string[] { "CookingStation", "Refrigerator", "FarmTile", "FertilizerMaker", "Aquafarm", "HydroponicFarm" }
			},
			{
				"GasPiping",
				new string[] { "GasConduit", "GasPump", "GasConduitBridge", "GasPermeableMembrane", "GasVent" }
			},
			{
				"TemperatureModulation",
				new string[] { "LiquidCooledFan", "AirConditioner", "TemperatureControlledSwitch", "LiquidHeater" }
			},
			{
				"Insulation",
				new string[] { "InsulatedWire", "InsulatedGasConduit", "InsulatedLiquidConduit", "InsulationTile", "ClothingFabricator" }
			},
			{
				"PressureManagement",
				new string[]
				{
					"LiquidValve",
					"GasValve",
					"PressureDoor",
					PressureSwitchLiquidConfig.ID,
					PressureSwitchGasConfig.ID
				}
			},
			{
				"LiquidPiping",
				new string[] { "LiquidConduit", "LiquidPump", "LiquidConduitBridge", "LiquidVent" }
			},
			{
				"SanitationSciences",
				new string[] { "HandSanitizer", "FlushToilet", "Shower", "AirFilter" }
			},
			{
				"Medbay",
				new string[] { "MedicalBed", "Apothecary", "MeshTile" }
			},
			{
				"Percolation",
				new string[] { "Electrolyzer", "CO2Scrubber" }
			},
			{
				"AdvancedFiltration",
				new string[] { "GasFilter", "LiquidFilter", "WaterPurifier", "AlgaeDistillery" }
			},
			{
				"PowerRegulation",
				new string[] { "Switch", "TimedSwitch", "WireBridge", "BatteryMedium" }
			},
			{
				"AdvancedPowerRegulation",
				new string[] { "HighWattageWire", "PowerTransformer" }
			},
			{
				"Combustion",
				new string[] { "Generator" }
			},
			{
				"ImprovedCombustion",
				new string[] { "MethaneGenerator", "HydrogenGenerator" }
			},
			{
				"InteriorDecor",
				new string[] { "Canvas", "Sculpture", "DiningTable", "CeilingLight", "Door" }
			}
		};

		public static List<string> COMPONENT_DESCRIPTION_ORDER = new List<string>
		{
			"AlgaeHabitat", "Fabricator", "MicrobeMusher", "CookingStation", "ResearchCenter", "LiquidCooledFan", "OxygenRecharger", "HandSanitizer", "PlantAirConditioner", "BuildingElementEmitter",
			"ElementConverter", "ElementConsumer", "PassiveElementConsumer", "EnergyConsumer", "AirConditioner", "Storage", "Battery", "Switch", "CircuitSwitch", "ManualDelivery",
			"ManualDeliveryKG", "AirFilter", "FlushToilet", "Toilet", "EnergyGenerator", "MassageTable", "Shower", "LiquidHeater", "Ownable", "PlantablePlot",
			"RelaxationPoint", "BuildingComplete", "Building", "BuildingPreview", "BuildingUnderConstruction", "TemperatureControlledSwitch", "TimedSwitch", "Crop", "Instance", "Growing",
			"Equippable", "ColdBreather", "ResearchPointObject", "SuitTank", "TemperatureVulnerable", "PressureVulnerable", "SubmersionMonitor", "PressureSwitch", "FertilizationMonitor", "RestRestoreHealth",
			"Edible", "PlantableSeed", "FriedMushBar", "BasicSingleHarvestPlant", "PrickleFlower", "DiseaseTrigger", "MedicinalHerb", "MedicinalPill", "SeedProducer", "Def",
			"DecorProvider", "Overheatable"
		};

		public class OVERHEAT_TEMPERATURES
		{
			public const float LOW_3 = 10f;

			public const float LOW_2 = 328.15f;

			public const float LOW_1 = 338.15f;

			public const float NORMAL = 348.15f;

			public const float HIGH_1 = 363.15f;

			public const float HIGH_2 = 398.15f;

			public const float HIGH_3 = 2273.15f;
		}

		public class OVERHEAT_MATERIAL_MOD
		{
			public const float LOW_3 = -200f;

			public const float LOW_2 = -20f;

			public const float LOW_1 = -10f;

			public const float NORMAL = 0f;

			public const float HIGH_1 = 15f;

			public const float HIGH_2 = 50f;

			public const float HIGH_3 = 2000f;
		}

		public class CONSTRUCTION_MASS_KG
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

		public class EXHAUST_ENERGY_ACTIVE
		{
			public const float TIER0 = 0f;

			public const float TIER1 = 0.125f;

			public const float TIER2 = 0.25f;

			public const float TIER3 = 0.5f;

			public const float TIER4 = 1f;

			public const float TIER5 = 2f;

			public const float TIER6 = 4f;

			public const float TIER7 = 8f;

			public const float TIER8 = 16f;
		}

		public class OPERATING_TEMPERATURE
		{
			public const float TIER0 = 0f;

			public const float TIER1 = 1f;

			public const float TIER2 = 2f;

			public const float TIER3 = 4f;

			public const float TIER4 = 8f;

			public const float TIER5 = 16f;

			public const float TIER6 = 32f;

			public const float TIER7 = 64f;

			public const float TIER8 = 128f;
		}

		public class OPERATING_KILOWATTS
		{
			public const float TIER0 = 0f;

			public const float TIER1 = 0.5f;

			public const float TIER2 = 1f;

			public const float TIER3 = 2f;

			public const float TIER4 = 4f;

			public const float TIER5 = 8f;

			public const float TIER6 = 16f;

			public const float TIER7 = 32f;

			public const float TIER8 = 64f;
		}

		public class MELTING_POINT_KELVIN
		{
			public const float TIER0 = 800f;

			public const float TIER1 = 1600f;

			public const float TIER2 = 2400f;

			public const float TIER3 = 3200f;
		}

		public class CONSTRUCTION_TIME_SECONDS
		{
			public const float TIER0 = 3f;

			public const float TIER1 = 10f;

			public const float TIER2 = 30f;

			public const float TIER3 = 60f;

			public const float TIER4 = 120f;

			public const float TIER5 = 240f;

			public const float TIER6 = 480f;
		}

		public class HITPOINTS
		{
			public const int TIER0 = 10;

			public const int TIER1 = 30;

			public const int TIER2 = 100;

			public const int TIER3 = 250;
		}

		public class DAMAGE_SOURCES
		{
			public const int CONDUIT_CONTENTS_BOILED = 2147483647;

			public const int CONDUIT_CONTENTS_FROZE = 2147483647;

			public const int BAD_INPUT_ELEMENT = 1;

			public const int BUILDING_OVERHEATED = 1;

			public const int HIGH_LIQUID_PRESSURE = 10;
		}

		public class RELOCATION_TIME_SECONDS
		{
			public const float DECONSTRUCT = 4f;

			public const float CONSTRUCT = 4f;
		}

		public class WORK_TIME_SECONDS
		{
			public const float VERYSHORT_WORK_TIME = 5f;

			public const float MEDIUM_WORK_TIME = 30f;

			public const float LONG_WORK_TIME = 90f;

			public const float EXTENSIVE_WORK_TIME = 180f;
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

		public class MASS_KG
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

				public const float PROPANEGENERATOR_ENERGYGENERATION = 1.6f;

				public const float PROPANEGENERATOR_HEATGENERATION = 1.6f;

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
