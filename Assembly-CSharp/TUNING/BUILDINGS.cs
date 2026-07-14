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

		public const float MASS_BURN_RATE_HYDROGENGENERATOR = 0.1f;

		public const float COOKER_FOOD_TEMPERATURE = 368.15f;

		public const float OVERHEAT_DAMAGE_INTERVAL = 7.5f;

		public const float MIN_BUILD_TEMPERATURE = 0f;

		public const float MAX_BUILD_TEMPERATURE = 318.15f;

		public const float MELTDOWN_TEMPERATURE = 533.15f;

		public const float REPAIR_FORCE_TEMPERATURE = 293.15f;

		public const int REPAIR_EFFECTIVENESS_BASE = 10;

		public static Dictionary<string, string> PLANSUBCATEGORYSORTING = new Dictionary<string, string>
		{
			{
				"Ladder",
				BUILDINGS.PlanSubcategoryName.ladders.ToString()
			},
			{
				"FirePole",
				BUILDINGS.PlanSubcategoryName.ladders.ToString()
			},
			{
				"LadderFast",
				BUILDINGS.PlanSubcategoryName.ladders.ToString()
			},
			{
				"Tile",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"SnowTile",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"WoodTile",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"GasPermeableMembrane",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"MeshTile",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"RubberTile",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"InsulationTile",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"PlasticTile",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"MetalTile",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"GlassTile",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"StorageTile",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"BunkerTile",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"ExteriorWall",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"GlassExteriorWall",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"CarpetTile",
				BUILDINGS.PlanSubcategoryName.tiles.ToString()
			},
			{
				"ExobaseHeadquarters",
				BUILDINGS.PlanSubcategoryName.printingpods.ToString()
			},
			{
				"Door",
				BUILDINGS.PlanSubcategoryName.doors.ToString()
			},
			{
				"WoodenDoor",
				BUILDINGS.PlanSubcategoryName.doors.ToString()
			},
			{
				"ManualPressureDoor",
				BUILDINGS.PlanSubcategoryName.doors.ToString()
			},
			{
				"InsulatedDoor",
				BUILDINGS.PlanSubcategoryName.doors.ToString()
			},
			{
				"PressureDoor",
				BUILDINGS.PlanSubcategoryName.doors.ToString()
			},
			{
				"BunkerDoor",
				BUILDINGS.PlanSubcategoryName.doors.ToString()
			},
			{
				"StorageLocker",
				BUILDINGS.PlanSubcategoryName.storage.ToString()
			},
			{
				"StorageLockerSmart",
				BUILDINGS.PlanSubcategoryName.storage.ToString()
			},
			{
				"LiquidReservoir",
				BUILDINGS.PlanSubcategoryName.storage.ToString()
			},
			{
				"GasReservoir",
				BUILDINGS.PlanSubcategoryName.storage.ToString()
			},
			{
				"ObjectDispenser",
				BUILDINGS.PlanSubcategoryName.storage.ToString()
			},
			{
				"TravelTube",
				BUILDINGS.PlanSubcategoryName.transport.ToString()
			},
			{
				"TravelTubeEntrance",
				BUILDINGS.PlanSubcategoryName.transport.ToString()
			},
			{
				"TravelTubeWallBridge",
				BUILDINGS.PlanSubcategoryName.transport.ToString()
			},
			{
				RemoteWorkerDockConfig.ID,
				BUILDINGS.PlanSubcategoryName.operations.ToString()
			},
			{
				RemoteWorkTerminalConfig.ID,
				BUILDINGS.PlanSubcategoryName.operations.ToString()
			},
			{
				"MineralDeoxidizer",
				BUILDINGS.PlanSubcategoryName.producers.ToString()
			},
			{
				"SublimationStation",
				BUILDINGS.PlanSubcategoryName.producers.ToString()
			},
			{
				"Oxysconce",
				BUILDINGS.PlanSubcategoryName.producers.ToString()
			},
			{
				"Electrolyzer",
				BUILDINGS.PlanSubcategoryName.producers.ToString()
			},
			{
				"RustDeoxidizer",
				BUILDINGS.PlanSubcategoryName.producers.ToString()
			},
			{
				"AirFilter",
				BUILDINGS.PlanSubcategoryName.scrubbers.ToString()
			},
			{
				"CO2Scrubber",
				BUILDINGS.PlanSubcategoryName.scrubbers.ToString()
			},
			{
				"AlgaeHabitat",
				BUILDINGS.PlanSubcategoryName.scrubbers.ToString()
			},
			{
				"UnderwaterBreathingStation",
				BUILDINGS.PlanSubcategoryName.distributors.ToString()
			},
			{
				"DevGenerator",
				BUILDINGS.PlanSubcategoryName.generators.ToString()
			},
			{
				"ManualGenerator",
				BUILDINGS.PlanSubcategoryName.generators.ToString()
			},
			{
				"Generator",
				BUILDINGS.PlanSubcategoryName.generators.ToString()
			},
			{
				"WoodGasGenerator",
				BUILDINGS.PlanSubcategoryName.generators.ToString()
			},
			{
				"PeatGenerator",
				BUILDINGS.PlanSubcategoryName.generators.ToString()
			},
			{
				"ReefGenerator",
				BUILDINGS.PlanSubcategoryName.generators.ToString()
			},
			{
				"HydrogenGenerator",
				BUILDINGS.PlanSubcategoryName.generators.ToString()
			},
			{
				"MethaneGenerator",
				BUILDINGS.PlanSubcategoryName.generators.ToString()
			},
			{
				"PetroleumGenerator",
				BUILDINGS.PlanSubcategoryName.generators.ToString()
			},
			{
				"SteamTurbine",
				BUILDINGS.PlanSubcategoryName.generators.ToString()
			},
			{
				"SteamTurbine2",
				BUILDINGS.PlanSubcategoryName.generators.ToString()
			},
			{
				"SolarPanel",
				BUILDINGS.PlanSubcategoryName.generators.ToString()
			},
			{
				"Wire",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"WireBridge",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"HighWattageWire",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"WireBridgeHighWattage",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"WireRefined",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"WireRefinedBridge",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"WireRefinedHighWattage",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"WireRefinedBridgeHighWattage",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"WireRubber",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"WireRubberBridge",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"Battery",
				BUILDINGS.PlanSubcategoryName.batteries.ToString()
			},
			{
				"BatteryMedium",
				BUILDINGS.PlanSubcategoryName.batteries.ToString()
			},
			{
				"BatterySmart",
				BUILDINGS.PlanSubcategoryName.batteries.ToString()
			},
			{
				"ElectrobankCharger",
				BUILDINGS.PlanSubcategoryName.electrobankbuildings.ToString()
			},
			{
				"SmallElectrobankDischarger",
				BUILDINGS.PlanSubcategoryName.electrobankbuildings.ToString()
			},
			{
				"LargeElectrobankDischarger",
				BUILDINGS.PlanSubcategoryName.electrobankbuildings.ToString()
			},
			{
				"PowerTransformerSmall",
				BUILDINGS.PlanSubcategoryName.powercontrol.ToString()
			},
			{
				"PowerTransformer",
				BUILDINGS.PlanSubcategoryName.powercontrol.ToString()
			},
			{
				SwitchConfig.ID,
				BUILDINGS.PlanSubcategoryName.switches.ToString()
			},
			{
				LogicPowerRelayConfig.ID,
				BUILDINGS.PlanSubcategoryName.switches.ToString()
			},
			{
				TemperatureControlledSwitchConfig.ID,
				BUILDINGS.PlanSubcategoryName.switches.ToString()
			},
			{
				PressureSwitchLiquidConfig.ID,
				BUILDINGS.PlanSubcategoryName.switches.ToString()
			},
			{
				PressureSwitchGasConfig.ID,
				BUILDINGS.PlanSubcategoryName.switches.ToString()
			},
			{
				"MicrobeMusher",
				BUILDINGS.PlanSubcategoryName.cooking.ToString()
			},
			{
				"CookingStation",
				BUILDINGS.PlanSubcategoryName.cooking.ToString()
			},
			{
				"Deepfryer",
				BUILDINGS.PlanSubcategoryName.cooking.ToString()
			},
			{
				"GourmetCookingStation",
				BUILDINGS.PlanSubcategoryName.cooking.ToString()
			},
			{
				"SpiceGrinder",
				BUILDINGS.PlanSubcategoryName.cooking.ToString()
			},
			{
				"FoodDehydrator",
				BUILDINGS.PlanSubcategoryName.cooking.ToString()
			},
			{
				"FoodRehydrator",
				BUILDINGS.PlanSubcategoryName.cooking.ToString()
			},
			{
				"Smoker",
				BUILDINGS.PlanSubcategoryName.cooking.ToString()
			},
			{
				"SushiBar",
				BUILDINGS.PlanSubcategoryName.cooking.ToString()
			},
			{
				"PlanterBox",
				BUILDINGS.PlanSubcategoryName.farming.ToString()
			},
			{
				"FarmTile",
				BUILDINGS.PlanSubcategoryName.farming.ToString()
			},
			{
				"HydroponicFarm",
				BUILDINGS.PlanSubcategoryName.farming.ToString()
			},
			{
				"WideFarmTile",
				BUILDINGS.PlanSubcategoryName.farming.ToString()
			},
			{
				"LargeBackwallFarm",
				BUILDINGS.PlanSubcategoryName.farming.ToString()
			},
			{
				"RationBox",
				BUILDINGS.PlanSubcategoryName.storage.ToString()
			},
			{
				"Refrigerator",
				BUILDINGS.PlanSubcategoryName.storage.ToString()
			},
			{
				"MiniFridge",
				BUILDINGS.PlanSubcategoryName.storage.ToString()
			},
			{
				"CreatureDeliveryPoint",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"UnderwaterMilkFeeder",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"CritterDropOff",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"CritterPickUp",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"FishDeliveryPoint",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"FishPickUp",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"CreatureFeeder",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"FishFeeder",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"MilkFeeder",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"EggIncubator",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"EggCracker",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"CreatureGroundTrap",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"CreatureAirTrap",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"WaterTrap",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"CritterCondo",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"UnderwaterCritterCondo",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"AirBorneCritterCondo",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"Outhouse",
				BUILDINGS.PlanSubcategoryName.washroom.ToString()
			},
			{
				"FlushToilet",
				BUILDINGS.PlanSubcategoryName.washroom.ToString()
			},
			{
				"WallToilet",
				BUILDINGS.PlanSubcategoryName.washroom.ToString()
			},
			{
				ShowerConfig.ID,
				BUILDINGS.PlanSubcategoryName.washroom.ToString()
			},
			{
				"GunkEmptier",
				BUILDINGS.PlanSubcategoryName.washroom.ToString()
			},
			{
				"LiquidConduit",
				BUILDINGS.PlanSubcategoryName.pipes.ToString()
			},
			{
				"InsulatedLiquidConduit",
				BUILDINGS.PlanSubcategoryName.pipes.ToString()
			},
			{
				"LiquidConduitRadiant",
				BUILDINGS.PlanSubcategoryName.pipes.ToString()
			},
			{
				"LiquidConduitBridge",
				BUILDINGS.PlanSubcategoryName.pipes.ToString()
			},
			{
				"ContactConductivePipeBridge",
				BUILDINGS.PlanSubcategoryName.pipes.ToString()
			},
			{
				"LiquidVent",
				BUILDINGS.PlanSubcategoryName.pipes.ToString()
			},
			{
				"LiquidPump",
				BUILDINGS.PlanSubcategoryName.pumps.ToString()
			},
			{
				"LiquidMiniPump",
				BUILDINGS.PlanSubcategoryName.pumps.ToString()
			},
			{
				"LiquidPumpingStation",
				BUILDINGS.PlanSubcategoryName.pumps.ToString()
			},
			{
				"DevPumpLiquid",
				BUILDINGS.PlanSubcategoryName.pumps.ToString()
			},
			{
				"BottleEmptier",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"LiquidFilter",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"LiquidConduitPreferentialFlow",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"LiquidConduitOverflow",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"LiquidValve",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"LiquidLogicValve",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"LiquidLimitValve",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"LiquidBottler",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"BottleEmptierConduitLiquid",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				LiquidConduitElementSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LiquidConduitDiseaseSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LiquidConduitTemperatureSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				"ModularLaunchpadPortLiquid",
				BUILDINGS.PlanSubcategoryName.buildmenuports.ToString()
			},
			{
				"ModularLaunchpadPortLiquidUnloader",
				BUILDINGS.PlanSubcategoryName.buildmenuports.ToString()
			},
			{
				"GasConduit",
				BUILDINGS.PlanSubcategoryName.pipes.ToString()
			},
			{
				"InsulatedGasConduit",
				BUILDINGS.PlanSubcategoryName.pipes.ToString()
			},
			{
				"GasConduitRadiant",
				BUILDINGS.PlanSubcategoryName.pipes.ToString()
			},
			{
				"GasConduitBridge",
				BUILDINGS.PlanSubcategoryName.pipes.ToString()
			},
			{
				"GasVent",
				BUILDINGS.PlanSubcategoryName.pipes.ToString()
			},
			{
				"GasVentHighPressure",
				BUILDINGS.PlanSubcategoryName.pipes.ToString()
			},
			{
				"GasPump",
				BUILDINGS.PlanSubcategoryName.pumps.ToString()
			},
			{
				"GasMiniPump",
				BUILDINGS.PlanSubcategoryName.pumps.ToString()
			},
			{
				"DevPumpGas",
				BUILDINGS.PlanSubcategoryName.pumps.ToString()
			},
			{
				"GasBottler",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"BottleEmptierGas",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"BottleEmptierConduitGas",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"GasFilter",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"GasConduitPreferentialFlow",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"GasConduitOverflow",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"GasValve",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"GasLogicValve",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"GasLimitValve",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				GasConduitElementSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				GasConduitDiseaseSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				GasConduitTemperatureSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				"ModularLaunchpadPortGas",
				BUILDINGS.PlanSubcategoryName.buildmenuports.ToString()
			},
			{
				"ModularLaunchpadPortGasUnloader",
				BUILDINGS.PlanSubcategoryName.buildmenuports.ToString()
			},
			{
				"Compost",
				BUILDINGS.PlanSubcategoryName.organic.ToString()
			},
			{
				"FertilizerMaker",
				BUILDINGS.PlanSubcategoryName.organic.ToString()
			},
			{
				"AlgaeDistillery",
				BUILDINGS.PlanSubcategoryName.organic.ToString()
			},
			{
				"EthanolDistillery",
				BUILDINGS.PlanSubcategoryName.organic.ToString()
			},
			{
				"SludgePress",
				BUILDINGS.PlanSubcategoryName.organic.ToString()
			},
			{
				"MilkFatSeparator",
				BUILDINGS.PlanSubcategoryName.organic.ToString()
			},
			{
				"MilkPress",
				BUILDINGS.PlanSubcategoryName.organic.ToString()
			},
			{
				"IceKettle",
				BUILDINGS.PlanSubcategoryName.materials.ToString()
			},
			{
				"WaterPurifier",
				BUILDINGS.PlanSubcategoryName.materials.ToString()
			},
			{
				"Desalinator",
				BUILDINGS.PlanSubcategoryName.materials.ToString()
			},
			{
				"RockCrusher",
				BUILDINGS.PlanSubcategoryName.materials.ToString()
			},
			{
				"Kiln",
				BUILDINGS.PlanSubcategoryName.materials.ToString()
			},
			{
				"FabricatedWoodMaker",
				BUILDINGS.PlanSubcategoryName.materials.ToString()
			},
			{
				"MetalRefinery",
				BUILDINGS.PlanSubcategoryName.materials.ToString()
			},
			{
				"GlassForge",
				BUILDINGS.PlanSubcategoryName.materials.ToString()
			},
			{
				"RubberMaker",
				BUILDINGS.PlanSubcategoryName.materials.ToString()
			},
			{
				"OilRefinery",
				BUILDINGS.PlanSubcategoryName.oil.ToString()
			},
			{
				"Polymerizer",
				BUILDINGS.PlanSubcategoryName.oil.ToString()
			},
			{
				"OxyliteRefinery",
				BUILDINGS.PlanSubcategoryName.advanced.ToString()
			},
			{
				"ChemicalRefinery",
				BUILDINGS.PlanSubcategoryName.advanced.ToString()
			},
			{
				"SupermaterialRefinery",
				BUILDINGS.PlanSubcategoryName.advanced.ToString()
			},
			{
				"DiamondPress",
				BUILDINGS.PlanSubcategoryName.advanced.ToString()
			},
			{
				"Chlorinator",
				BUILDINGS.PlanSubcategoryName.advanced.ToString()
			},
			{
				"WashBasin",
				BUILDINGS.PlanSubcategoryName.hygiene.ToString()
			},
			{
				"WashSink",
				BUILDINGS.PlanSubcategoryName.hygiene.ToString()
			},
			{
				"HandSanitizer",
				BUILDINGS.PlanSubcategoryName.hygiene.ToString()
			},
			{
				"DecontaminationShower",
				BUILDINGS.PlanSubcategoryName.hygiene.ToString()
			},
			{
				"Apothecary",
				BUILDINGS.PlanSubcategoryName.medical.ToString()
			},
			{
				"DoctorStation",
				BUILDINGS.PlanSubcategoryName.medical.ToString()
			},
			{
				"AdvancedDoctorStation",
				BUILDINGS.PlanSubcategoryName.medical.ToString()
			},
			{
				"MedicalCot",
				BUILDINGS.PlanSubcategoryName.medical.ToString()
			},
			{
				"DevLifeSupport",
				BUILDINGS.PlanSubcategoryName.medical.ToString()
			},
			{
				"MassageTable",
				BUILDINGS.PlanSubcategoryName.wellness.ToString()
			},
			{
				"Grave",
				BUILDINGS.PlanSubcategoryName.wellness.ToString()
			},
			{
				"OilChanger",
				BUILDINGS.PlanSubcategoryName.wellness.ToString()
			},
			{
				"Bed",
				BUILDINGS.PlanSubcategoryName.beds.ToString()
			},
			{
				"LuxuryBed",
				BUILDINGS.PlanSubcategoryName.beds.ToString()
			},
			{
				LadderBedConfig.ID,
				BUILDINGS.PlanSubcategoryName.beds.ToString()
			},
			{
				"FloorLamp",
				BUILDINGS.PlanSubcategoryName.lights.ToString()
			},
			{
				"CeilingLight",
				BUILDINGS.PlanSubcategoryName.lights.ToString()
			},
			{
				"GlassCeilingLight",
				BUILDINGS.PlanSubcategoryName.lights.ToString()
			},
			{
				"SunLamp",
				BUILDINGS.PlanSubcategoryName.lights.ToString()
			},
			{
				"DevLightGenerator",
				BUILDINGS.PlanSubcategoryName.lights.ToString()
			},
			{
				"MercuryCeilingLight",
				BUILDINGS.PlanSubcategoryName.lights.ToString()
			},
			{
				"DiningTable",
				BUILDINGS.PlanSubcategoryName.dining.ToString()
			},
			{
				"MultiMinionDiningTable",
				BUILDINGS.PlanSubcategoryName.dining.ToString()
			},
			{
				"WaterCooler",
				BUILDINGS.PlanSubcategoryName.recreation.ToString()
			},
			{
				"Phonobox",
				BUILDINGS.PlanSubcategoryName.recreation.ToString()
			},
			{
				"ArcadeMachine",
				BUILDINGS.PlanSubcategoryName.recreation.ToString()
			},
			{
				"EspressoMachine",
				BUILDINGS.PlanSubcategoryName.recreation.ToString()
			},
			{
				"HotTub",
				BUILDINGS.PlanSubcategoryName.recreation.ToString()
			},
			{
				"MechanicalSurfboard",
				BUILDINGS.PlanSubcategoryName.recreation.ToString()
			},
			{
				"Sauna",
				BUILDINGS.PlanSubcategoryName.recreation.ToString()
			},
			{
				"Juicer",
				BUILDINGS.PlanSubcategoryName.recreation.ToString()
			},
			{
				"SodaFountain",
				BUILDINGS.PlanSubcategoryName.recreation.ToString()
			},
			{
				"BeachChair",
				BUILDINGS.PlanSubcategoryName.recreation.ToString()
			},
			{
				"VerticalWindTunnel",
				BUILDINGS.PlanSubcategoryName.recreation.ToString()
			},
			{
				"Telephone",
				BUILDINGS.PlanSubcategoryName.recreation.ToString()
			},
			{
				"FlowerVase",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"FlowerVaseWall",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"FlowerVaseHanging",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"FlowerVaseHangingFancy",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				PixelPackConfig.ID,
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"SmallSculpture",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"Sculpture",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"IceSculpture",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"MarbleSculpture",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"MetalSculpture",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"WoodSculpture",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"FossilSculpture",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"CeilingFossilSculpture",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"CrownMoulding",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"CornerMoulding",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"Canvas",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"CanvasWide",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"CanvasTall",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"ItemPedestal",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"Shelf",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"ParkSign",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"MonumentBottom",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"MonumentMiddle",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"MonumentTop",
				BUILDINGS.PlanSubcategoryName.decor.ToString()
			},
			{
				"ResearchCenter",
				BUILDINGS.PlanSubcategoryName.research.ToString()
			},
			{
				"AdvancedResearchCenter",
				BUILDINGS.PlanSubcategoryName.research.ToString()
			},
			{
				"GeoTuner",
				BUILDINGS.PlanSubcategoryName.research.ToString()
			},
			{
				"NuclearResearchCenter",
				BUILDINGS.PlanSubcategoryName.research.ToString()
			},
			{
				"OrbitalResearchCenter",
				BUILDINGS.PlanSubcategoryName.research.ToString()
			},
			{
				"CosmicResearchCenter",
				BUILDINGS.PlanSubcategoryName.research.ToString()
			},
			{
				"DLC1CosmicResearchCenter",
				BUILDINGS.PlanSubcategoryName.research.ToString()
			},
			{
				"DataMiner",
				BUILDINGS.PlanSubcategoryName.research.ToString()
			},
			{
				"ArtifactAnalysisStation",
				BUILDINGS.PlanSubcategoryName.archaeology.ToString()
			},
			{
				"MissileFabricator",
				BUILDINGS.PlanSubcategoryName.meteordefense.ToString()
			},
			{
				"AstronautTrainingCenter",
				BUILDINGS.PlanSubcategoryName.exploration.ToString()
			},
			{
				"PowerControlStation",
				BUILDINGS.PlanSubcategoryName.industrialstation.ToString()
			},
			{
				"ResetSkillsStation",
				BUILDINGS.PlanSubcategoryName.industrialstation.ToString()
			},
			{
				"RoleStation",
				BUILDINGS.PlanSubcategoryName.workstations.ToString()
			},
			{
				"RanchStation",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"ShearingStation",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"MilkingStation",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"UnderwaterRanchStation",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"UnderwaterShearingStation",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"UnderwaterMilkingStation",
				BUILDINGS.PlanSubcategoryName.ranching.ToString()
			},
			{
				"FarmStation",
				BUILDINGS.PlanSubcategoryName.farming.ToString()
			},
			{
				"GeneticAnalysisStation",
				BUILDINGS.PlanSubcategoryName.farming.ToString()
			},
			{
				"CraftingTable",
				BUILDINGS.PlanSubcategoryName.manufacturing.ToString()
			},
			{
				"AdvancedCraftingTable",
				BUILDINGS.PlanSubcategoryName.manufacturing.ToString()
			},
			{
				"ClothingFabricator",
				BUILDINGS.PlanSubcategoryName.manufacturing.ToString()
			},
			{
				"ClothingAlterationStation",
				BUILDINGS.PlanSubcategoryName.manufacturing.ToString()
			},
			{
				"SuitFabricator",
				BUILDINGS.PlanSubcategoryName.manufacturing.ToString()
			},
			{
				"OxygenMaskMarker",
				BUILDINGS.PlanSubcategoryName.equipment.ToString()
			},
			{
				"OxygenMaskLocker",
				BUILDINGS.PlanSubcategoryName.equipment.ToString()
			},
			{
				"SuitMarker",
				BUILDINGS.PlanSubcategoryName.equipment.ToString()
			},
			{
				"SuitLocker",
				BUILDINGS.PlanSubcategoryName.equipment.ToString()
			},
			{
				"JetSuitMarker",
				BUILDINGS.PlanSubcategoryName.equipment.ToString()
			},
			{
				"JetSuitLocker",
				BUILDINGS.PlanSubcategoryName.equipment.ToString()
			},
			{
				"MissileLauncher",
				BUILDINGS.PlanSubcategoryName.missiles.ToString()
			},
			{
				"LeadSuitMarker",
				BUILDINGS.PlanSubcategoryName.equipment.ToString()
			},
			{
				"LeadSuitLocker",
				BUILDINGS.PlanSubcategoryName.equipment.ToString()
			},
			{
				"Campfire",
				BUILDINGS.PlanSubcategoryName.temperature.ToString()
			},
			{
				"DevHeater",
				BUILDINGS.PlanSubcategoryName.temperature.ToString()
			},
			{
				"SpaceHeater",
				BUILDINGS.PlanSubcategoryName.temperature.ToString()
			},
			{
				"LiquidHeater",
				BUILDINGS.PlanSubcategoryName.temperature.ToString()
			},
			{
				"LiquidConditioner",
				BUILDINGS.PlanSubcategoryName.temperature.ToString()
			},
			{
				"LiquidCooledFan",
				BUILDINGS.PlanSubcategoryName.temperature.ToString()
			},
			{
				"IceCooledFan",
				BUILDINGS.PlanSubcategoryName.temperature.ToString()
			},
			{
				"IceMachine",
				BUILDINGS.PlanSubcategoryName.temperature.ToString()
			},
			{
				"AirConditioner",
				BUILDINGS.PlanSubcategoryName.temperature.ToString()
			},
			{
				"ThermalBlock",
				BUILDINGS.PlanSubcategoryName.temperature.ToString()
			},
			{
				"OreScrubber",
				BUILDINGS.PlanSubcategoryName.sanitation.ToString()
			},
			{
				"OilWellCap",
				BUILDINGS.PlanSubcategoryName.oil.ToString()
			},
			{
				"SweepBotStation",
				BUILDINGS.PlanSubcategoryName.sanitation.ToString()
			},
			{
				"LogicWire",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"LogicWireBridge",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"LogicRibbon",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"LogicRibbonBridge",
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				LogicRibbonReaderConfig.ID,
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				LogicRibbonWriterConfig.ID,
				BUILDINGS.PlanSubcategoryName.wires.ToString()
			},
			{
				"LogicDuplicantSensor",
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicPressureSensorGasConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicPressureSensorLiquidConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicTemperatureSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicLightSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicWattageSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicTimeOfDaySensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicTimerSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicDiseaseSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicElementSensorGasConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicElementSensorLiquidConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicCritterCountSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicRadiationSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicHEPSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				CometDetectorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				LogicCounterConfig.ID,
				BUILDINGS.PlanSubcategoryName.logicmanager.ToString()
			},
			{
				"Checkpoint",
				BUILDINGS.PlanSubcategoryName.logicmanager.ToString()
			},
			{
				LogicAlarmConfig.ID,
				BUILDINGS.PlanSubcategoryName.logicmanager.ToString()
			},
			{
				LogicHammerConfig.ID,
				BUILDINGS.PlanSubcategoryName.logicaudio.ToString()
			},
			{
				LogicSwitchConfig.ID,
				BUILDINGS.PlanSubcategoryName.switches.ToString()
			},
			{
				"FloorSwitch",
				BUILDINGS.PlanSubcategoryName.switches.ToString()
			},
			{
				"LogicGateNOT",
				BUILDINGS.PlanSubcategoryName.logicgates.ToString()
			},
			{
				"LogicGateAND",
				BUILDINGS.PlanSubcategoryName.logicgates.ToString()
			},
			{
				"LogicGateOR",
				BUILDINGS.PlanSubcategoryName.logicgates.ToString()
			},
			{
				"LogicGateBUFFER",
				BUILDINGS.PlanSubcategoryName.logicgates.ToString()
			},
			{
				"LogicGateFILTER",
				BUILDINGS.PlanSubcategoryName.logicgates.ToString()
			},
			{
				"LogicGateXOR",
				BUILDINGS.PlanSubcategoryName.logicgates.ToString()
			},
			{
				LogicMemoryConfig.ID,
				BUILDINGS.PlanSubcategoryName.logicgates.ToString()
			},
			{
				"LogicGateMultiplexer",
				BUILDINGS.PlanSubcategoryName.logicgates.ToString()
			},
			{
				"LogicGateDemultiplexer",
				BUILDINGS.PlanSubcategoryName.logicgates.ToString()
			},
			{
				"LogicInterasteroidSender",
				BUILDINGS.PlanSubcategoryName.transmissions.ToString()
			},
			{
				"LogicInterasteroidReceiver",
				BUILDINGS.PlanSubcategoryName.transmissions.ToString()
			},
			{
				"SolidConduit",
				BUILDINGS.PlanSubcategoryName.conveyancestructures.ToString()
			},
			{
				"UnderwaterVentDrill",
				BUILDINGS.PlanSubcategoryName.conveyancestructures.ToString()
			},
			{
				"SolidConduitBridge",
				BUILDINGS.PlanSubcategoryName.conveyancestructures.ToString()
			},
			{
				"SolidConduitInbox",
				BUILDINGS.PlanSubcategoryName.conveyancestructures.ToString()
			},
			{
				"SolidConduitOutbox",
				BUILDINGS.PlanSubcategoryName.conveyancestructures.ToString()
			},
			{
				"SolidFilter",
				BUILDINGS.PlanSubcategoryName.conveyancestructures.ToString()
			},
			{
				"SolidVent",
				BUILDINGS.PlanSubcategoryName.conveyancestructures.ToString()
			},
			{
				"DevPumpSolid",
				BUILDINGS.PlanSubcategoryName.pumps.ToString()
			},
			{
				"SolidLogicValve",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				"SolidLimitValve",
				BUILDINGS.PlanSubcategoryName.valves.ToString()
			},
			{
				SolidConduitDiseaseSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				SolidConduitElementSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				SolidConduitTemperatureSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.sensors.ToString()
			},
			{
				"AutoMiner",
				BUILDINGS.PlanSubcategoryName.automated.ToString()
			},
			{
				"SolidTransferArm",
				BUILDINGS.PlanSubcategoryName.automated.ToString()
			},
			{
				"ModularLaunchpadPortSolid",
				BUILDINGS.PlanSubcategoryName.buildmenuports.ToString()
			},
			{
				"ModularLaunchpadPortSolidUnloader",
				BUILDINGS.PlanSubcategoryName.buildmenuports.ToString()
			},
			{
				"Telescope",
				BUILDINGS.PlanSubcategoryName.telescopes.ToString()
			},
			{
				"ClusterTelescope",
				BUILDINGS.PlanSubcategoryName.telescopes.ToString()
			},
			{
				"ClusterTelescopeEnclosed",
				BUILDINGS.PlanSubcategoryName.telescopes.ToString()
			},
			{
				"LaunchPad",
				BUILDINGS.PlanSubcategoryName.rocketstructures.ToString()
			},
			{
				"Gantry",
				BUILDINGS.PlanSubcategoryName.rocketstructures.ToString()
			},
			{
				"ModularLaunchpadPortBridge",
				BUILDINGS.PlanSubcategoryName.rocketstructures.ToString()
			},
			{
				"RailGun",
				BUILDINGS.PlanSubcategoryName.fittings.ToString()
			},
			{
				"RailGunPayloadOpener",
				BUILDINGS.PlanSubcategoryName.fittings.ToString()
			},
			{
				"LandingBeacon",
				BUILDINGS.PlanSubcategoryName.rocketnav.ToString()
			},
			{
				"SteamEngine",
				BUILDINGS.PlanSubcategoryName.engines.ToString()
			},
			{
				"KeroseneEngine",
				BUILDINGS.PlanSubcategoryName.engines.ToString()
			},
			{
				"BiodieselEngine",
				BUILDINGS.PlanSubcategoryName.engines.ToString()
			},
			{
				"HydrogenEngine",
				BUILDINGS.PlanSubcategoryName.engines.ToString()
			},
			{
				"SolidBooster",
				BUILDINGS.PlanSubcategoryName.engines.ToString()
			},
			{
				"LiquidFuelTank",
				BUILDINGS.PlanSubcategoryName.tanks.ToString()
			},
			{
				"OxidizerTank",
				BUILDINGS.PlanSubcategoryName.tanks.ToString()
			},
			{
				"OxidizerTankLiquid",
				BUILDINGS.PlanSubcategoryName.tanks.ToString()
			},
			{
				"CargoBay",
				BUILDINGS.PlanSubcategoryName.cargo.ToString()
			},
			{
				"GasCargoBay",
				BUILDINGS.PlanSubcategoryName.cargo.ToString()
			},
			{
				"LiquidCargoBay",
				BUILDINGS.PlanSubcategoryName.cargo.ToString()
			},
			{
				"SpecialCargoBay",
				BUILDINGS.PlanSubcategoryName.cargo.ToString()
			},
			{
				"CommandModule",
				BUILDINGS.PlanSubcategoryName.rocketnav.ToString()
			},
			{
				RocketControlStationConfig.ID,
				BUILDINGS.PlanSubcategoryName.rocketnav.ToString()
			},
			{
				LogicClusterLocationSensorConfig.ID,
				BUILDINGS.PlanSubcategoryName.rocketnav.ToString()
			},
			{
				"MissionControl",
				BUILDINGS.PlanSubcategoryName.rocketnav.ToString()
			},
			{
				"MissionControlCluster",
				BUILDINGS.PlanSubcategoryName.rocketnav.ToString()
			},
			{
				"RoboPilotCommandModule",
				BUILDINGS.PlanSubcategoryName.rocketnav.ToString()
			},
			{
				"TouristModule",
				BUILDINGS.PlanSubcategoryName.module.ToString()
			},
			{
				"ResearchModule",
				BUILDINGS.PlanSubcategoryName.module.ToString()
			},
			{
				"RocketInteriorPowerPlug",
				BUILDINGS.PlanSubcategoryName.fittings.ToString()
			},
			{
				"RocketInteriorLiquidInput",
				BUILDINGS.PlanSubcategoryName.fittings.ToString()
			},
			{
				"RocketInteriorLiquidOutput",
				BUILDINGS.PlanSubcategoryName.fittings.ToString()
			},
			{
				"RocketInteriorGasInput",
				BUILDINGS.PlanSubcategoryName.fittings.ToString()
			},
			{
				"RocketInteriorGasOutput",
				BUILDINGS.PlanSubcategoryName.fittings.ToString()
			},
			{
				"RocketInteriorSolidInput",
				BUILDINGS.PlanSubcategoryName.fittings.ToString()
			},
			{
				"RocketInteriorSolidOutput",
				BUILDINGS.PlanSubcategoryName.fittings.ToString()
			},
			{
				"ManualHighEnergyParticleSpawner",
				BUILDINGS.PlanSubcategoryName.producers.ToString()
			},
			{
				"HighEnergyParticleSpawner",
				BUILDINGS.PlanSubcategoryName.producers.ToString()
			},
			{
				"DevHEPSpawner",
				BUILDINGS.PlanSubcategoryName.producers.ToString()
			},
			{
				"HighEnergyParticleRedirector",
				BUILDINGS.PlanSubcategoryName.transmissions.ToString()
			},
			{
				"HEPBattery",
				BUILDINGS.PlanSubcategoryName.batteries.ToString()
			},
			{
				"HEPBridgeTile",
				BUILDINGS.PlanSubcategoryName.transmissions.ToString()
			},
			{
				"NuclearReactor",
				BUILDINGS.PlanSubcategoryName.producers.ToString()
			},
			{
				"UraniumCentrifuge",
				BUILDINGS.PlanSubcategoryName.producers.ToString()
			},
			{
				"RadiationLight",
				BUILDINGS.PlanSubcategoryName.producers.ToString()
			},
			{
				"DevRadiationGenerator",
				BUILDINGS.PlanSubcategoryName.producers.ToString()
			}
		};

		public static List<PlanScreen.PlanInfo> PLANORDER = new List<PlanScreen.PlanInfo>
		{
			new PlanScreen.PlanInfo(new HashedString("Base"), false, new List<string>
			{
				"Ladder", "FirePole", "LadderFast", "Tile", "SnowTile", "WoodTile", "GasPermeableMembrane", "MeshTile", "RubberTile", "InsulationTile",
				"PlasticTile", "MetalTile", "GlassTile", "StorageTile", "BunkerTile", "CarpetTile", "ExteriorWall", "GlassExteriorWall", "ExobaseHeadquarters", "Door",
				"WoodenDoor", "ManualPressureDoor", "InsulatedDoor", "PressureDoor", "BunkerDoor", "StorageLocker", "StorageLockerSmart", "LiquidReservoir", "GasReservoir", "ObjectDispenser",
				"TravelTube", "TravelTubeEntrance", "TravelTubeWallBridge"
			}, null, null),
			new PlanScreen.PlanInfo(new HashedString("Oxygen"), false, new List<string> { "MineralDeoxidizer", "SublimationStation", "Oxysconce", "AlgaeHabitat", "AirFilter", "CO2Scrubber", "Electrolyzer", "RustDeoxidizer", "UnderwaterBreathingStation" }, null, null),
			new PlanScreen.PlanInfo(new HashedString("Power"), false, new List<string>
			{
				"DevGenerator",
				"ManualGenerator",
				"Generator",
				"WoodGasGenerator",
				"PeatGenerator",
				"ReefGenerator",
				"HydrogenGenerator",
				"MethaneGenerator",
				"PetroleumGenerator",
				"SteamTurbine",
				"SteamTurbine2",
				"SolarPanel",
				"Wire",
				"WireBridge",
				"HighWattageWire",
				"WireBridgeHighWattage",
				"WireRefined",
				"WireRefinedBridge",
				"WireRefinedHighWattage",
				"WireRefinedBridgeHighWattage",
				"WireRubber",
				"WireRubberBridge",
				"Battery",
				"BatteryMedium",
				"BatterySmart",
				"ElectrobankCharger",
				"SmallElectrobankDischarger",
				"LargeElectrobankDischarger",
				"PowerTransformerSmall",
				"PowerTransformer",
				SwitchConfig.ID,
				LogicPowerRelayConfig.ID,
				TemperatureControlledSwitchConfig.ID,
				PressureSwitchLiquidConfig.ID,
				PressureSwitchGasConfig.ID
			}, null, null),
			new PlanScreen.PlanInfo(new HashedString("Food"), false, new List<string>
			{
				"UnderwaterMilkFeeder", "MicrobeMusher", "CookingStation", "Deepfryer", "GourmetCookingStation", "SpiceGrinder", "FoodDehydrator", "FoodRehydrator", "Smoker", "SushiBar",
				"PlanterBox", "FarmTile", "HydroponicFarm", "WideFarmTile", "LargeBackwallFarm", "RationBox", "Refrigerator", "MiniFridge", "CreatureDeliveryPoint", "CritterPickUp",
				"CritterDropOff", "FishPickUp", "FishDeliveryPoint", "CreatureFeeder", "FishFeeder", "MilkFeeder", "EggIncubator", "EggCracker", "CreatureGroundTrap", "WaterTrap",
				"CreatureAirTrap", "CritterCondo", "UnderwaterCritterCondo", "AirBorneCritterCondo"
			}, null, null),
			new PlanScreen.PlanInfo(new HashedString("Plumbing"), false, new List<string>
			{
				"DevPumpLiquid",
				"Outhouse",
				"FlushToilet",
				"WallToilet",
				ShowerConfig.ID,
				"GunkEmptier",
				"LiquidPumpingStation",
				"BottleEmptier",
				"BottleEmptierConduitLiquid",
				"LiquidBottler",
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
				"LiquidLimitValve",
				LiquidConduitElementSensorConfig.ID,
				LiquidConduitDiseaseSensorConfig.ID,
				LiquidConduitTemperatureSensorConfig.ID,
				"ModularLaunchpadPortLiquid",
				"ModularLaunchpadPortLiquidUnloader",
				"ContactConductivePipeBridge"
			}, null, null),
			new PlanScreen.PlanInfo(new HashedString("HVAC"), false, new List<string>
			{
				"DevPumpGas",
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
				"GasLimitValve",
				"GasBottler",
				"BottleEmptierGas",
				"BottleEmptierConduitGas",
				"ModularLaunchpadPortGas",
				"ModularLaunchpadPortGasUnloader",
				GasConduitElementSensorConfig.ID,
				GasConduitDiseaseSensorConfig.ID,
				GasConduitTemperatureSensorConfig.ID
			}, null, null),
			new PlanScreen.PlanInfo(new HashedString("Refining"), false, new List<string>
			{
				"FabricatedWoodMaker", "Compost", "WaterPurifier", "Desalinator", "FertilizerMaker", "AlgaeDistillery", "EthanolDistillery", "RockCrusher", "Kiln", "SludgePress",
				"MetalRefinery", "GlassForge", "OilRefinery", "Polymerizer", "RubberMaker", "OxyliteRefinery", "Chlorinator", "ChemicalRefinery", "SupermaterialRefinery", "DiamondPress",
				"MilkFatSeparator", "MilkPress"
			}, null, null),
			new PlanScreen.PlanInfo(new HashedString(BUILDINGS.PlanSubcategoryName.medical.ToString()), false, new List<string>
			{
				"DevLifeSupport", "WashBasin", "WashSink", "HandSanitizer", "DecontaminationShower", "OilChanger", "Apothecary", "DoctorStation", "AdvancedDoctorStation", "MedicalCot",
				"MassageTable", "Grave"
			}, null, null),
			new PlanScreen.PlanInfo(new HashedString("Furniture"), false, new List<string>
			{
				"Shelf",
				"Bed",
				"LuxuryBed",
				LadderBedConfig.ID,
				"FloorLamp",
				"CeilingLight",
				"GlassCeilingLight",
				"SunLamp",
				"DevLightGenerator",
				"MercuryCeilingLight",
				"DiningTable",
				"MultiMinionDiningTable",
				"WaterCooler",
				"Phonobox",
				"ArcadeMachine",
				"EspressoMachine",
				"HotTub",
				"MechanicalSurfboard",
				"Sauna",
				"Juicer",
				"SodaFountain",
				"BeachChair",
				"VerticalWindTunnel",
				PixelPackConfig.ID,
				"Telephone",
				"FlowerVase",
				"FlowerVaseWall",
				"FlowerVaseHanging",
				"FlowerVaseHangingFancy",
				"SmallSculpture",
				"Sculpture",
				"IceSculpture",
				"WoodSculpture",
				"MarbleSculpture",
				"MetalSculpture",
				"FossilSculpture",
				"CeilingFossilSculpture",
				"CrownMoulding",
				"CornerMoulding",
				"Canvas",
				"CanvasWide",
				"CanvasTall",
				"ItemPedestal",
				"MonumentBottom",
				"MonumentMiddle",
				"MonumentTop",
				"ParkSign"
			}, null, null),
			new PlanScreen.PlanInfo(new HashedString("Equipment"), false, new List<string>
			{
				"ResearchCenter",
				"AdvancedResearchCenter",
				"NuclearResearchCenter",
				"OrbitalResearchCenter",
				"CosmicResearchCenter",
				"DLC1CosmicResearchCenter",
				"Telescope",
				"GeoTuner",
				"DataMiner",
				"PowerControlStation",
				"FarmStation",
				"GeneticAnalysisStation",
				"RanchStation",
				"ShearingStation",
				"MilkingStation",
				"UnderwaterRanchStation",
				"UnderwaterShearingStation",
				"UnderwaterMilkingStation",
				"RoleStation",
				"ResetSkillsStation",
				"ArtifactAnalysisStation",
				RemoteWorkerDockConfig.ID,
				RemoteWorkTerminalConfig.ID,
				"MissileFabricator",
				"CraftingTable",
				"AdvancedCraftingTable",
				"ClothingFabricator",
				"ClothingAlterationStation",
				"SuitFabricator",
				"OxygenMaskMarker",
				"OxygenMaskLocker",
				"SuitMarker",
				"SuitLocker",
				"JetSuitMarker",
				"JetSuitLocker",
				"LeadSuitMarker",
				"LeadSuitLocker",
				"AstronautTrainingCenter"
			}, null, null),
			new PlanScreen.PlanInfo(new HashedString("Utilities"), true, new List<string>
			{
				"UnderwaterVentDrill", "Campfire", "DevHeater", "IceKettle", "SpaceHeater", "LiquidHeater", "LiquidCooledFan", "IceCooledFan", "IceMachine", "AirConditioner",
				"LiquidConditioner", "OreScrubber", "OilWellCap", "ThermalBlock", "SweepBotStation"
			}, null, null),
			new PlanScreen.PlanInfo(new HashedString("Automation"), true, new List<string>
			{
				"LogicWire",
				"LogicWireBridge",
				"LogicRibbon",
				"LogicRibbonBridge",
				LogicSwitchConfig.ID,
				"LogicDuplicantSensor",
				LogicPressureSensorGasConfig.ID,
				LogicPressureSensorLiquidConfig.ID,
				LogicTemperatureSensorConfig.ID,
				LogicLightSensorConfig.ID,
				LogicWattageSensorConfig.ID,
				LogicTimeOfDaySensorConfig.ID,
				LogicTimerSensorConfig.ID,
				LogicDiseaseSensorConfig.ID,
				LogicElementSensorGasConfig.ID,
				LogicElementSensorLiquidConfig.ID,
				LogicCritterCountSensorConfig.ID,
				LogicRadiationSensorConfig.ID,
				LogicHEPSensorConfig.ID,
				LogicCounterConfig.ID,
				LogicAlarmConfig.ID,
				LogicHammerConfig.ID,
				"LogicInterasteroidSender",
				"LogicInterasteroidReceiver",
				LogicRibbonReaderConfig.ID,
				LogicRibbonWriterConfig.ID,
				"FloorSwitch",
				"Checkpoint",
				CometDetectorConfig.ID,
				"LogicGateNOT",
				"LogicGateAND",
				"LogicGateOR",
				"LogicGateBUFFER",
				"LogicGateFILTER",
				"LogicGateXOR",
				LogicMemoryConfig.ID,
				"LogicGateMultiplexer",
				"LogicGateDemultiplexer"
			}, null, null),
			new PlanScreen.PlanInfo(new HashedString("Conveyance"), true, new List<string>
			{
				"DevPumpSolid",
				"SolidTransferArm",
				"SolidConduit",
				"SolidConduitBridge",
				"SolidConduitInbox",
				"SolidConduitOutbox",
				"SolidFilter",
				"SolidVent",
				"SolidLogicValve",
				"SolidLimitValve",
				SolidConduitDiseaseSensorConfig.ID,
				SolidConduitElementSensorConfig.ID,
				SolidConduitTemperatureSensorConfig.ID,
				"AutoMiner",
				"ModularLaunchpadPortSolid",
				"ModularLaunchpadPortSolidUnloader"
			}, null, null),
			new PlanScreen.PlanInfo(new HashedString("Rocketry"), true, new List<string>
			{
				"ClusterTelescope",
				"ClusterTelescopeEnclosed",
				"MissionControl",
				"MissionControlCluster",
				"LaunchPad",
				"Gantry",
				"SteamEngine",
				"KeroseneEngine",
				"BiodieselEngine",
				"SolidBooster",
				"LiquidFuelTank",
				"OxidizerTank",
				"OxidizerTankLiquid",
				"CargoBay",
				"GasCargoBay",
				"LiquidCargoBay",
				"CommandModule",
				"RoboPilotCommandModule",
				"TouristModule",
				"ResearchModule",
				"SpecialCargoBay",
				"HydrogenEngine",
				RocketControlStationConfig.ID,
				"RocketInteriorPowerPlug",
				"RocketInteriorLiquidInput",
				"RocketInteriorLiquidOutput",
				"RocketInteriorGasInput",
				"RocketInteriorGasOutput",
				"RocketInteriorSolidInput",
				"RocketInteriorSolidOutput",
				LogicClusterLocationSensorConfig.ID,
				"RailGun",
				"RailGunPayloadOpener",
				"LandingBeacon",
				"MissileLauncher",
				"ModularLaunchpadPortBridge"
			}, null, null),
			new PlanScreen.PlanInfo(new HashedString("HEP"), true, new List<string> { "RadiationLight", "ManualHighEnergyParticleSpawner", "NuclearReactor", "UraniumCentrifuge", "HighEnergyParticleSpawner", "DevHEPSpawner", "HighEnergyParticleRedirector", "HEPBattery", "HEPBridgeTile", "DevRadiationGenerator" }, DlcManager.EXPANSION1, null)
		};

		public static List<Type> COMPONENT_DESCRIPTION_ORDER = new List<Type>
		{
			typeof(BottleEmptier),
			typeof(CookingStation),
			typeof(GourmetCookingStation),
			typeof(RoleStation),
			typeof(ResearchCenter),
			typeof(NuclearResearchCenter),
			typeof(LiquidCooledFan),
			typeof(HandSanitizer),
			typeof(HandSanitizer.Work),
			typeof(PlantAirConditioner),
			typeof(Clinic),
			typeof(BuildingElementEmitter),
			typeof(ElementConverter),
			typeof(ElementConsumer),
			typeof(PassiveElementConsumer),
			typeof(TinkerStation),
			typeof(EnergyConsumer),
			typeof(AirConditioner),
			typeof(Storage),
			typeof(Battery),
			typeof(AirFilter),
			typeof(FlushToilet),
			typeof(Toilet),
			typeof(EnergyGenerator),
			typeof(MassageTable),
			typeof(Shower),
			typeof(Ownable),
			typeof(PlantablePlot),
			typeof(RelaxationPoint),
			typeof(BuildingComplete),
			typeof(Building),
			typeof(BuildingPreview),
			typeof(BuildingUnderConstruction),
			typeof(Crop),
			typeof(Growing),
			typeof(Equippable),
			typeof(ColdBreather),
			typeof(ResearchPointObject),
			typeof(SuitTank),
			typeof(IlluminationVulnerable),
			typeof(TemperatureVulnerable),
			typeof(ExternalTemperatureMonitor),
			typeof(CritterTemperatureMonitor),
			typeof(PressureVulnerable),
			typeof(SubmersionMonitor),
			typeof(BatterySmart),
			typeof(Compost),
			typeof(Refrigerator),
			typeof(Bed),
			typeof(OreScrubber),
			typeof(OreScrubber.Work),
			typeof(MinimumOperatingTemperature),
			typeof(RoomTracker),
			typeof(EnergyConsumerSelfSustaining),
			typeof(ArcadeMachine),
			typeof(Telescope),
			typeof(EspressoMachine),
			typeof(JetSuitTank),
			typeof(Phonobox),
			typeof(ArcadeMachine),
			typeof(BeachChair),
			typeof(Sauna),
			typeof(VerticalWindTunnel),
			typeof(HotTub),
			typeof(Juicer),
			typeof(SodaFountain),
			typeof(MechanicalSurfboard),
			typeof(BottleEmptier),
			typeof(AccessControl),
			typeof(GammaRayOven),
			typeof(Reactor),
			typeof(HighEnergyParticlePort),
			typeof(LeadSuitTank),
			typeof(ActiveParticleConsumer.Def),
			typeof(WaterCooler),
			typeof(Edible),
			typeof(PlantableSeed),
			typeof(SicknessTrigger),
			typeof(MedicinalPill),
			typeof(SeedProducer),
			typeof(Geyser),
			typeof(SpaceHeater),
			typeof(Overheatable),
			typeof(CreatureCalorieMonitor.Def),
			typeof(LureableMonitor.Def),
			typeof(FertilizationMonitor.Def),
			typeof(IrrigationMonitor.Def),
			typeof(ScaleGrowthMonitor.Def),
			typeof(TravelTubeEntrance.Work),
			typeof(ToiletWorkableUse),
			typeof(ReceptacleMonitor),
			typeof(Light2D),
			typeof(Ladder),
			typeof(SimCellOccupier),
			typeof(Vent),
			typeof(LogicPorts),
			typeof(Capturable),
			typeof(Trappable),
			typeof(SpaceArtifact),
			typeof(MessStation),
			typeof(PlantElementEmitter),
			typeof(Radiator),
			typeof(DecorProvider)
		};

		public class PHARMACY
		{
			public class FABRICATIONTIME
			{
				public const float TIER0 = 50f;

				public const float TIER1 = 100f;

				public const float TIER2 = 200f;
			}
		}

		public class NUCLEAR_REACTOR
		{
			public class REACTOR_MASSES
			{
				public const float MIN = 1f;

				public const float MAX = 10f;
			}
		}

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

			public const float HIGH_3 = 200f;

			public const float HIGH_4 = 500f;

			public const float HIGH_5 = 900f;
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
			public static readonly float[] TIER_TINY = new float[] { 5f };

			public static readonly float[] TIER_SMALL = new float[] { 10f };

			public static readonly float[] TIER0 = new float[] { 25f };

			public static readonly float[] TIER1 = new float[] { 50f };

			public static readonly float[] TIER2 = new float[] { 100f };

			public static readonly float[] TIER3 = new float[] { 200f };

			public static readonly float[] TIER4 = new float[] { 400f };

			public static readonly float[] TIER5 = new float[] { 800f };

			public static readonly float[] TIER6 = new float[] { 1200f };

			public static readonly float[] TIER7 = new float[] { 2000f };
		}

		public class ROCKETRY_MASS_KG
		{
			public static float[] COMMAND_MODULE_MASS = new float[] { 200f };

			public static float[] CARGO_MASS = new float[] { 1000f };

			public static float[] CARGO_MASS_SMALL = new float[] { 400f };

			public static float[] FUEL_TANK_DRY_MASS = new float[] { 100f };

			public static float[] FUEL_TANK_WET_MASS = new float[] { 900f };

			public static float[] FUEL_TANK_WET_MASS_SMALL = new float[] { 300f };

			public static float[] FUEL_TANK_WET_MASS_GAS = new float[] { 100f };

			public static float[] FUEL_TANK_WET_MASS_GAS_LARGE = new float[] { 150f };

			public static float[] OXIDIZER_TANK_OXIDIZER_MASS = new float[] { 900f };

			public static float[] ENGINE_MASS_SMALL = new float[] { 200f };

			public static float[] ENGINE_MASS_LARGE = new float[] { 500f };

			public static float[] NOSE_CONE_TIER1 = new float[] { 200f, 100f };

			public static float[] NOSE_CONE_TIER2 = new float[] { 400f, 200f };

			public static float[] HOLLOW_TIER1 = new float[] { 200f };

			public static float[] HOLLOW_TIER2 = new float[] { 400f };

			public static float[] HOLLOW_TIER3 = new float[] { 800f };

			public static float[] DENSE_TIER0 = new float[] { 200f };

			public static float[] DENSE_TIER1 = new float[] { 500f };

			public static float[] DENSE_TIER2 = new float[] { 1000f };

			public static float[] DENSE_TIER3 = new float[] { 2000f };
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

		public class JOULES_LEAK_PER_CYCLE
		{
			public const float TIER0 = 400f;

			public const float TIER1 = 1000f;

			public const float TIER2 = 2000f;
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

			public const float TIER_NUCLEAR = 16384f;
		}

		public class MELTING_POINT_KELVIN
		{
			public const float TIER0 = 800f;

			public const float TIER1 = 1600f;

			public const float TIER2 = 2400f;

			public const float TIER3 = 3200f;

			public const float TIER4 = 9999f;
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

			public const int TIER4 = 1000;
		}

		public class DAMAGE_SOURCES
		{
			public const int CONDUIT_CONTENTS_BOILED = 1;

			public const int CONDUIT_CONTENTS_FROZE = 1;

			public const int BAD_INPUT_ELEMENT = 1;

			public const int BUILDING_OVERHEATED = 1;

			public const int HIGH_LIQUID_PRESSURE = 10;

			public const int MICROMETEORITE = 1;

			public const int CORROSIVE_ELEMENT = 1;
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

			public const float VERY_LONG_WORK_TIME = 150f;

			public const float EXTENSIVE_WORK_TIME = 180f;
		}

		public class FABRICATION_TIME_SECONDS
		{
			public const float VERY_SHORT = 20f;

			public const float SHORT = 40f;

			public const float MODERATE = 80f;

			public const float LONG = 250f;
		}

		public class DECOR
		{
			public static readonly EffectorValues NONE = new EffectorValues
			{
				amount = 0,
				radius = 1
			};

			public class BONUS
			{
				public static readonly EffectorValues TIER0 = new EffectorValues
				{
					amount = 5,
					radius = 1
				};

				public static readonly EffectorValues TIER1 = new EffectorValues
				{
					amount = 10,
					radius = 2
				};

				public static readonly EffectorValues TIER2 = new EffectorValues
				{
					amount = 15,
					radius = 3
				};

				public static readonly EffectorValues TIER3 = new EffectorValues
				{
					amount = 20,
					radius = 4
				};

				public static readonly EffectorValues TIER4 = new EffectorValues
				{
					amount = 25,
					radius = 5
				};

				public static readonly EffectorValues TIER5 = new EffectorValues
				{
					amount = 30,
					radius = 6
				};

				public class MONUMENT
				{
					public static readonly EffectorValues COMPLETE = new EffectorValues
					{
						amount = 40,
						radius = 10
					};

					public static readonly EffectorValues INCOMPLETE = new EffectorValues
					{
						amount = 10,
						radius = 5
					};
				}
			}

			public class PENALTY
			{
				public static readonly EffectorValues TIER0 = new EffectorValues
				{
					amount = -5,
					radius = 1
				};

				public static readonly EffectorValues TIER1 = new EffectorValues
				{
					amount = -10,
					radius = 2
				};

				public static readonly EffectorValues TIER2 = new EffectorValues
				{
					amount = -15,
					radius = 3
				};

				public static readonly EffectorValues TIER3 = new EffectorValues
				{
					amount = -20,
					radius = 4
				};

				public static readonly EffectorValues TIER4 = new EffectorValues
				{
					amount = -20,
					radius = 5
				};

				public static readonly EffectorValues TIER5 = new EffectorValues
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

		public enum PlanSubcategoryName
		{
			ladders,
			tiles,
			printingpods,
			doors,
			storage,
			transport,
			operations,
			producers,
			scrubbers,
			distributors,
			generators,
			wires,
			batteries,
			electrobankbuildings,
			powercontrol,
			switches,
			cooking,
			farming,
			ranching,
			washroom,
			pipes,
			pumps,
			valves,
			sensors,
			buildmenuports,
			organic,
			materials,
			oil,
			advanced,
			hygiene,
			medical,
			wellness,
			beds,
			lights,
			dining,
			recreation,
			decor,
			research,
			archaeology,
			meteordefense,
			exploration,
			industrialstation,
			workstations,
			manufacturing,
			equipment,
			missiles,
			temperature,
			sanitation,
			logicmanager,
			logicaudio,
			logicgates,
			transmissions,
			conveyancestructures,
			automated,
			telescopes,
			rocketstructures,
			fittings,
			rocketnav,
			module,
			cargo,
			engines,
			tanks
		}
	}
}
