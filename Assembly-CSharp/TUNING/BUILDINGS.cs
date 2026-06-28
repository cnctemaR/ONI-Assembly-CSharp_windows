using System;
using System.Collections.Generic;

namespace TUNING
{
	public class BUILDINGS
	{
		public const float DEFAULT_STORAGE_CAPACITY = 2000f;

		public const float STANDARD_MANUAL_REFILL_LEVEL = 0.2f;

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

		public const float COOKER_FOOD_TEMPERATURE = 368.15f;

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
				new string[]
				{
					"Ladder", "LadderFast", "Tile", "GasPermeableMembrane", "MeshTile", "InsulationTile", "PlasticTile", "Door", "ManualPressureDoor", "PressureDoor",
					"StorageLocker"
				}
			},
			{
				PlanCategory.Oxygen,
				new string[] { "MineralDeoxidizer", "AlgaeHabitat", "AirFilter", "CO2Scrubber", "Electrolyzer" }
			},
			{
				PlanCategory.Power,
				new string[]
				{
					"ManualGenerator",
					"Generator",
					"HydrogenGenerator",
					"MethaneGenerator",
					"PetroleumGenerator",
					"Wire",
					"WireBridge",
					"HighWattageWire",
					"WireBridgeHighWattage",
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
				new string[] { "MicrobeMusher", "CookingStation", "PlanterBox", "FarmTile", "HydroponicFarm", "RationBox", "Refrigerator", "CreatureTrap", "CreatureDeliveryPoint" }
			},
			{
				PlanCategory.Plumbing,
				new string[]
				{
					"Outhouse", "FlushToilet", "Shower", "LiquidPumpingStation", "BottleEmptier", "LiquidConduit", "InsulatedLiquidConduit", "LiquidConduitBridge", "LiquidPump", "LiquidValve",
					"LiquidVent", "LiquidFilter"
				}
			},
			{
				PlanCategory.HVAC,
				new string[] { "GasConduit", "InsulatedGasConduit", "GasConduitBridge", "GasPump", "GasValve", "GasVent", "GasVentHighPressure", "GasFilter" }
			},
			{
				PlanCategory.Utilities,
				new string[] { "SpaceHeater", "LiquidHeater", "LiquidCooledFan", "AirConditioner", "LiquidConditioner", "OreScrubber", "OilWellCap" }
			},
			{
				PlanCategory.Refining,
				new string[] { "Compost", "FertilizerMaker", "WaterPurifier", "AlgaeDistillery", "OilRefinery", "Polymerizer" }
			},
			{
				PlanCategory.Medical,
				new string[] { "WashBasin", "WashSink", "HandSanitizer", "Apothecary", "MedicalCot", "MedicalBed", "Grave" }
			},
			{
				PlanCategory.Furniture,
				new string[]
				{
					BedConfig.ID,
					LuxuryBedConfig.ID,
					"MassageTable",
					"DiningTable"
				}
			},
			{
				PlanCategory.Equipment,
				new string[] { "ResearchCenter", "AdvancedResearchCenter", "GenericFabricator", "ClothingFabricator", "SuitFabricator", "SuitMarker", "SuitLocker" }
			},
			{
				PlanCategory.Misc,
				new string[] { "FlowerVase", "Canvas", "Sculpture", "FloorLamp", "CeilingLight" }
			}
		};

		public static Dictionary<string, string[]> RESEARCH = new Dictionary<string, string[]>
		{
			{
				"FarmingTech",
				new string[] { "AlgaeHabitat", "PlanterBox", "RationBox", "Compost" }
			},
			{
				"FineDining",
				new string[] { "DiningTable", "FarmTile", "CookingStation" }
			},
			{
				"Agriculture",
				new string[] { "FertilizerMaker", "HydroponicFarm", "Refrigerator" }
			},
			{
				"AnimalControl",
				new string[] { "CreatureTrap", "CreatureDeliveryPoint" }
			},
			{
				"ImprovedOxygen",
				new string[] { "CO2Scrubber", "Electrolyzer" }
			},
			{
				"GasPiping",
				new string[] { "GasConduit", "GasPump", "GasVent", "GasConduitBridge" }
			},
			{
				"ImprovedGasPiping",
				new string[]
				{
					"InsulatedGasConduit",
					PressureSwitchGasConfig.ID,
					"GasVentHighPressure"
				}
			},
			{
				"Clothing",
				new string[] { "ClothingFabricator" }
			},
			{
				"PressureManagement",
				new string[] { "LiquidValve", "GasValve", "ManualPressureDoor", "GasPermeableMembrane" }
			},
			{
				"DirectedAirStreams",
				new string[] { "PressureDoor", "OreScrubber", "AirFilter" }
			},
			{
				"LiquidPiping",
				new string[] { "LiquidConduit", "LiquidPump", "LiquidVent", "LiquidConduitBridge" }
			},
			{
				"Luxury",
				new string[]
				{
					LuxuryBedConfig.ID,
					"LadderFast",
					"PlasticTile"
				}
			},
			{
				"ImprovedLiquidPiping",
				new string[]
				{
					"InsulatedLiquidConduit",
					PressureSwitchLiquidConfig.ID
				}
			},
			{
				"SanitationSciences",
				new string[] { "WashSink", "FlushToilet", "Shower", "MeshTile" }
			},
			{
				"Medbay",
				new string[] { "HandSanitizer", "MedicalBed" }
			},
			{
				"AdvancedFiltration",
				new string[] { "GasFilter", "LiquidFilter" }
			},
			{
				"Distillation",
				new string[] { "WaterPurifier", "AlgaeDistillery" }
			},
			{
				"PowerRegulation",
				new string[] { "Switch", "BatteryMedium", "WireBridge" }
			},
			{
				"AdvancedPowerRegulation",
				new string[] { "HighWattageWire", "WireBridgeHighWattage", "PowerTransformer" }
			},
			{
				"Combustion",
				new string[] { "Generator", "HydrogenGenerator" }
			},
			{
				"ImprovedCombustion",
				new string[] { "MethaneGenerator", "OilRefinery", "PetroleumGenerator" }
			},
			{
				"InteriorDecor",
				new string[] { "FlowerVase", "FloorLamp", "CeilingLight" }
			},
			{
				"Artistry",
				new string[] { "Canvas", "Sculpture" }
			},
			{
				"Plastics",
				new string[] { "Polymerizer", "OilWellCap" }
			},
			{
				"Suits",
				new string[] { "SuitMarker", "SuitLocker", "SuitFabricator" }
			},
			{
				"AdvancedResearch",
				new string[] { "AdvancedResearchCenter" }
			},
			{
				"MedicalResearch",
				new string[] { "Apothecary" }
			},
			{
				"TemperatureModulation",
				new string[] { "LiquidCooledFan", "SpaceHeater", "InsulationTile" }
			},
			{
				"HVAC",
				new string[] { "AirConditioner", "TemperatureControlledSwitch" }
			},
			{
				"LiquidTemperature",
				new string[] { "LiquidHeater", "LiquidConditioner" }
			}
		};

		public static List<string> COMPONENT_DESCRIPTION_ORDER = new List<string>
		{
			"AlgaeHabitat",
			"BottleEmptier",
			"Fabricator",
			"MicrobeMusher",
			"CookingStation",
			"ResearchCenter",
			"LiquidCooledFan",
			"OxygenRecharger",
			"HandSanitizer",
			"PlantAirConditioner",
			"Clinic",
			"BuildingElementEmitter",
			"ElementConverter",
			"ElementConsumer",
			"PassiveElementConsumer",
			"EnergyConsumer",
			"AirConditioner",
			"Storage",
			"Battery",
			"Switch",
			"CircuitSwitch",
			"ManualDelivery",
			"ManualDeliveryKG",
			"AirFilter",
			"FlushToilet",
			"Toilet",
			"EnergyGenerator",
			"MassageTable",
			"Shower",
			"LiquidHeater",
			"Ownable",
			"PlantablePlot",
			"RelaxationPoint",
			"BuildingComplete",
			"Building",
			"BuildingPreview",
			"BuildingUnderConstruction",
			"TemperatureControlledSwitch",
			"TimedSwitch",
			"Crop",
			"Instance",
			"Growing",
			"Equippable",
			"ColdBreather",
			"SeedSplicer",
			"ResearchPointObject",
			"SuitTank",
			"IlluminationVulnerable",
			"TemperatureVulnerable",
			"PressureVulnerable",
			"SubmersionMonitor",
			"PressureSwitch",
			"FertilizationMonitor",
			"RestRestoreHealth",
			"GeneShuffler",
			"Polymerizer",
			"OilRefinery",
			"Compost",
			"Refrigerator",
			BedConfig.ID,
			"OreScrubber",
			"Edible",
			"PlantableSeed",
			"FriedMushBar",
			"BasicSingleHarvestPlant",
			"PrickleFlower",
			"DiseaseTrigger",
			"MedicinalHerb",
			"MedicinalPill",
			"SeedProducer",
			"Def",
			"Overheatable",
			"Work",
			"ToiletWorkableUse",
			"ReceptacleMonitor",
			"Light2D",
			"DecorProvider"
		};

		public class OVERPRESSURE
		{
			public const float TIER0 = 1.8f;
		}

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

			public const float TIER7 = 1200f;

			public const float TIER8 = 1600f;
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

		public class FABRICATION_TIME_SECONDS
		{
			public const float STANDARD = 40f;

			public const float LONG = 250f;
		}

		public class DECOR
		{
			public static EffectorValues NONE = new EffectorValues
			{
				amount = 0,
				radius = 1
			};

			public class BONUS
			{
				public static EffectorValues TIER0 = new EffectorValues
				{
					amount = 5,
					radius = 1
				};

				public static EffectorValues TIER1 = new EffectorValues
				{
					amount = 10,
					radius = 2
				};

				public static EffectorValues TIER2 = new EffectorValues
				{
					amount = 15,
					radius = 3
				};

				public static EffectorValues TIER3 = new EffectorValues
				{
					amount = 20,
					radius = 4
				};

				public static EffectorValues TIER4 = new EffectorValues
				{
					amount = 25,
					radius = 5
				};

				public static EffectorValues TIER5 = new EffectorValues
				{
					amount = 30,
					radius = 6
				};
			}

			public class PENALTY
			{
				public static EffectorValues TIER0 = new EffectorValues
				{
					amount = -5,
					radius = 1
				};

				public static EffectorValues TIER1 = new EffectorValues
				{
					amount = -10,
					radius = 2
				};

				public static EffectorValues TIER2 = new EffectorValues
				{
					amount = -15,
					radius = 3
				};

				public static EffectorValues TIER3 = new EffectorValues
				{
					amount = -20,
					radius = 4
				};

				public static EffectorValues TIER4 = new EffectorValues
				{
					amount = -20,
					radius = 5
				};

				public static EffectorValues TIER5 = new EffectorValues
				{
					amount = -25,
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
