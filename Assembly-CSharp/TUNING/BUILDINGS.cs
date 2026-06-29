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

		public const int TUBE_LAUNCHER_MAX_CHARGES = 3;

		public const float TUBE_LAUNCHER_RECHARGE_TIME = 10f;

		public const float TUBE_LAUNCHER_WORK_TIME = 1f;

		public const float SMELTER_INGOT_INPUTKG = 500f;

		public const float SMELTER_INGOT_OUTPUTKG = 100f;

		public const float SMELTER_FABRICATIONTIME = 120f;

		public const float GEOREFINERY_SLAB_INPUTKG = 1000f;

		public const float GEOREFINERY_SLAB_OUTPUTKG = 200f;

		public const float GEOREFINERY_FABRICATIONTIME = 120f;

		public const float PHARMACY_FABRICATIONTIME = 40f;

		public const float PHARMACY_GENERIC_INPUTKG = 100f;

		public const float PHARMACY_GENERIC_SINGLE = 1f;

		public const float MASS_BURN_RATE_HYDROGENGENERATOR = 0.1f;

		public const float COOKER_FOOD_TEMPERATURE = 368.15f;

		public const float OVERHEAT_DAMAGE_INTERVAL = 7.5f;

		public const float MIN_BUILD_TEMPERATURE = 288.15f;

		public const float MAX_BUILD_TEMPERATURE = 318.15f;

		public const float MELTDOWN_TEMPERATURE = 533.15f;

		public const float REPAIR_FORCE_TEMPERATURE = 293.15f;

		public const int REPAIR_EFFECTIVENESS_BASE = 10;

		public static PlanScreen.PlanInfo[] PLANORDER = new PlanScreen.PlanInfo[]
		{
			new PlanScreen.PlanInfo(PlanScreen.PlanCategory.Base, new string[]
			{
				"Ladder", "FirePole", "LadderFast", "Tile", "GasPermeableMembrane", "MeshTile", "InsulationTile", "PlasticTile", "MetalTile", "Door",
				"ManualPressureDoor", "PressureDoor", "StorageLocker", "StorageLockerSmart", "TravelTube", "TravelTubeEntrance", "TravelTubeWallBridge"
			}),
			new PlanScreen.PlanInfo(PlanScreen.PlanCategory.Oxygen, new string[] { "MineralDeoxidizer", "AlgaeHabitat", "AirFilter", "CO2Scrubber", "Electrolyzer" }),
			new PlanScreen.PlanInfo(PlanScreen.PlanCategory.Power, new string[]
			{
				"ManualGenerator",
				"Generator",
				"HydrogenGenerator",
				"MethaneGenerator",
				"PetroleumGenerator",
				"SteamTurbine",
				"Wire",
				"WireBridge",
				"HighWattageWire",
				"WireBridgeHighWattage",
				"WireRefined",
				"WireRefinedBridge",
				"WireRefinedHighWattage",
				"WireRefinedBridgeHighWattage",
				"Battery",
				"BatteryMedium",
				"BatterySmart",
				"PowerTransformer",
				SwitchConfig.ID,
				LogicPowerRelayConfig.ID,
				TemperatureControlledSwitchConfig.ID,
				PressureSwitchLiquidConfig.ID,
				PressureSwitchGasConfig.ID
			}),
			new PlanScreen.PlanInfo(PlanScreen.PlanCategory.Food, new string[]
			{
				"MicrobeMusher", "CookingStation", "PlanterBox", "FarmTile", "HydroponicFarm", "RationBox", "Refrigerator", "CreatureDeliveryPoint", "FishDeliveryPoint", "CreatureFeeder",
				"FishFeeder", "EggIncubator", "CreatureTrap", "FishTrap", "AirborneCreatureLure"
			}),
			new PlanScreen.PlanInfo(PlanScreen.PlanCategory.Plumbing, new string[]
			{
				"Outhouse",
				"FlushToilet",
				"Shower",
				"LiquidPumpingStation",
				"BottleEmptier",
				"LiquidConduit",
				"InsulatedLiquidConduit",
				"LiquidConduitRadiant",
				"LiquidConduitBridge",
				"LiquidConduitPreferentialFlow",
				"LiquidConduitOverflow",
				"LiquidPump",
				"LiquidMiniPump",
				"LiquidVent",
				"LiquidFilter",
				"LiquidValve",
				"LiquidLogicValve",
				LiquidConduitElementSensorConfig.ID,
				LiquidConduitDiseaseSensorConfig.ID,
				LiquidConduitTemperatureSensorConfig.ID
			}),
			new PlanScreen.PlanInfo(PlanScreen.PlanCategory.HVAC, new string[]
			{
				"GasConduit",
				"InsulatedGasConduit",
				"GasConduitRadiant",
				"GasConduitBridge",
				"GasConduitPreferentialFlow",
				"GasConduitOverflow",
				"GasPump",
				"GasMiniPump",
				"GasVent",
				"GasVentHighPressure",
				"GasFilter",
				"GasValve",
				"GasLogicValve",
				GasConduitElementSensorConfig.ID,
				GasConduitDiseaseSensorConfig.ID,
				GasConduitTemperatureSensorConfig.ID
			}),
			new PlanScreen.PlanInfo(PlanScreen.PlanCategory.Refining, new string[] { "Compost", "WaterPurifier", "FertilizerMaker", "AlgaeDistillery", "RockCrusher", "MetalRefinery", "OilRefinery", "Polymerizer" }),
			new PlanScreen.PlanInfo(PlanScreen.PlanCategory.Medical, new string[] { "WashBasin", "WashSink", "HandSanitizer", "Apothecary", "MedicalCot", "MedicalBed", "Grave" }),
			new PlanScreen.PlanInfo(PlanScreen.PlanCategory.Furniture, new string[]
			{
				BedConfig.ID,
				LuxuryBedConfig.ID,
				"MassageTable",
				"DiningTable",
				"FlowerVase",
				"Canvas",
				"Sculpture",
				"IceSculpture",
				"FloorLamp",
				"CeilingLight"
			}),
			new PlanScreen.PlanInfo(PlanScreen.PlanCategory.Equipment, new string[]
			{
				"ResearchCenter", "AdvancedResearchCenter", "PowerControlStation", "FarmStation", "RanchStation", "ShearingStation", "RoleStation", "ClothingFabricator", "SuitFabricator", "SuitMarker",
				"SuitLocker"
			}),
			new PlanScreen.PlanInfo(PlanScreen.PlanCategory.Utilities, new string[] { "SpaceHeater", "LiquidHeater", "LiquidCooledFan", "AirConditioner", "LiquidConditioner", "OreScrubber", "OilWellCap", "ThermalBlock" }),
			new PlanScreen.PlanInfo(PlanScreen.PlanCategory.Automation, new string[]
			{
				"LogicWire",
				"LogicWireBridge",
				"LogicGateAND",
				"LogicGateOR",
				"LogicGateXOR",
				"LogicGateNOT",
				"LogicGateBUFFER",
				"LogicGateFILTER",
				LogicMemoryConfig.ID,
				LogicSwitchConfig.ID,
				LogicPressureSensorGasConfig.ID,
				LogicPressureSensorLiquidConfig.ID,
				LogicTemperatureSensorConfig.ID,
				LogicTimeOfDaySensorConfig.ID,
				LogicDiseaseSensorConfig.ID,
				LogicElementSensorGasConfig.ID,
				"FloorSwitch",
				"Checkpoint"
			}),
			new PlanScreen.PlanInfo(PlanScreen.PlanCategory.Conveyance, new string[] { "SolidTransferArm", "SolidConduit", "SolidConduitInbox", "SolidConduitOutbox", "SolidConduitBridge" })
		};

		public static List<string> COMPONENT_DESCRIPTION_ORDER = new List<string>
		{
			"AlgaeHabitat",
			"BottleEmptier",
			"Fabricator",
			"MicrobeMusher",
			"CookingStation",
			"RoleStation",
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
			"TinkerStation",
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
			"BatterySmart",
			"Polymerizer",
			"OilRefinery",
			"Compost",
			"Refrigerator",
			BedConfig.ID,
			"OreScrubber",
			"Refinery",
			"LiquidCooledRefinery",
			"MinimumOperatingTemperature",
			"RoomTracker",
			"EnergyConsumerSelfSustaining",
			"Edible",
			"PlantableSeed",
			"FriedMushBar",
			"BasicSingleHarvestPlant",
			"PrickleFlower",
			"DiseaseTrigger",
			"MedicinalHerb",
			"MedicinalPill",
			"SeedProducer",
			"Geyser",
			"Def",
			"Overheatable",
			"Work",
			"ToiletWorkableUse",
			"ReceptacleMonitor",
			"Light2D",
			"Ladder",
			"SimCellOccupier",
			"Vent",
			"TilePOI",
			"LogicPorts",
			"Capturable",
			"Trappable",
			"ScaleGrowthMonitor",
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

			public const float HIGH_3 = 1273.15f;

			public const float HIGH_4 = 2273.15f;
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

		public class DECOR_MATERIAL_MOD
		{
			public const float NORMAL = 0f;

			public const float HIGH_1 = 0.1f;

			public const float HIGH_2 = 0.2f;

			public const float HIGH_3 = 0.5f;

			public const float HIGH_4 = 1f;
		}

		public class CONSTRUCTION_MASS_KG
		{
			public static float[] TIER_TINY = new float[] { 5f };

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

		public class SELF_HEAT_KILOWATTS
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

			public const float SHORT_WORK_TIME = 15f;

			public const float MEDIUM_WORK_TIME = 30f;

			public const float LONG_WORK_TIME = 90f;

			public const float EXTENSIVE_WORK_TIME = 180f;
		}

		public class FABRICATION_TIME_SECONDS
		{
			public const float SHORT = 40f;

			public const float MODERATE = 80f;

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
