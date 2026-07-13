using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class Techs : ResourceSet<Tech>
	{
		public Techs(ResourceSet parent)
			: base("Techs", parent)
		{
			if (!DlcManager.IsExpansion1Active())
			{
				this.TECH_TIERS = new List<List<global::Tuple<string, float>>>
				{
					new List<global::Tuple<string, float>>(),
					new List<global::Tuple<string, float>>
					{
						new global::Tuple<string, float>("basic", 15f)
					},
					new List<global::Tuple<string, float>>
					{
						new global::Tuple<string, float>("basic", 20f)
					},
					new List<global::Tuple<string, float>>
					{
						new global::Tuple<string, float>("basic", 30f),
						new global::Tuple<string, float>("advanced", 20f)
					},
					new List<global::Tuple<string, float>>
					{
						new global::Tuple<string, float>("basic", 35f),
						new global::Tuple<string, float>("advanced", 30f)
					},
					new List<global::Tuple<string, float>>
					{
						new global::Tuple<string, float>("basic", 40f),
						new global::Tuple<string, float>("advanced", 50f)
					},
					new List<global::Tuple<string, float>>
					{
						new global::Tuple<string, float>("basic", 50f),
						new global::Tuple<string, float>("advanced", 70f)
					},
					new List<global::Tuple<string, float>>
					{
						new global::Tuple<string, float>("basic", 70f),
						new global::Tuple<string, float>("advanced", 100f)
					},
					new List<global::Tuple<string, float>>
					{
						new global::Tuple<string, float>("basic", 70f),
						new global::Tuple<string, float>("advanced", 100f),
						new global::Tuple<string, float>("space", 200f)
					},
					new List<global::Tuple<string, float>>
					{
						new global::Tuple<string, float>("basic", 70f),
						new global::Tuple<string, float>("advanced", 100f),
						new global::Tuple<string, float>("space", 400f)
					},
					new List<global::Tuple<string, float>>
					{
						new global::Tuple<string, float>("basic", 70f),
						new global::Tuple<string, float>("advanced", 100f),
						new global::Tuple<string, float>("space", 800f)
					},
					new List<global::Tuple<string, float>>
					{
						new global::Tuple<string, float>("basic", 70f),
						new global::Tuple<string, float>("advanced", 100f),
						new global::Tuple<string, float>("space", 1600f)
					}
				};
				return;
			}
			this.TECH_TIERS = new List<List<global::Tuple<string, float>>>
			{
				new List<global::Tuple<string, float>>(),
				new List<global::Tuple<string, float>>
				{
					new global::Tuple<string, float>("basic", 15f)
				},
				new List<global::Tuple<string, float>>
				{
					new global::Tuple<string, float>("basic", 20f)
				},
				new List<global::Tuple<string, float>>
				{
					new global::Tuple<string, float>("basic", 30f),
					new global::Tuple<string, float>("advanced", 20f)
				},
				new List<global::Tuple<string, float>>
				{
					new global::Tuple<string, float>("basic", 35f),
					new global::Tuple<string, float>("advanced", 30f)
				},
				new List<global::Tuple<string, float>>
				{
					new global::Tuple<string, float>("basic", 40f),
					new global::Tuple<string, float>("advanced", 50f),
					new global::Tuple<string, float>("orbital", 0f),
					new global::Tuple<string, float>("nuclear", 20f)
				},
				new List<global::Tuple<string, float>>
				{
					new global::Tuple<string, float>("basic", 50f),
					new global::Tuple<string, float>("advanced", 70f),
					new global::Tuple<string, float>("orbital", 30f),
					new global::Tuple<string, float>("nuclear", 40f)
				},
				new List<global::Tuple<string, float>>
				{
					new global::Tuple<string, float>("basic", 70f),
					new global::Tuple<string, float>("advanced", 100f),
					new global::Tuple<string, float>("orbital", 250f),
					new global::Tuple<string, float>("nuclear", 370f)
				},
				new List<global::Tuple<string, float>>
				{
					new global::Tuple<string, float>("basic", 100f),
					new global::Tuple<string, float>("advanced", 130f),
					new global::Tuple<string, float>("orbital", 400f),
					new global::Tuple<string, float>("nuclear", 435f)
				},
				new List<global::Tuple<string, float>>
				{
					new global::Tuple<string, float>("basic", 100f),
					new global::Tuple<string, float>("advanced", 130f),
					new global::Tuple<string, float>("orbital", 600f)
				},
				new List<global::Tuple<string, float>>
				{
					new global::Tuple<string, float>("basic", 100f),
					new global::Tuple<string, float>("advanced", 130f),
					new global::Tuple<string, float>("orbital", 800f)
				},
				new List<global::Tuple<string, float>>
				{
					new global::Tuple<string, float>("basic", 100f),
					new global::Tuple<string, float>("advanced", 130f),
					new global::Tuple<string, float>("orbital", 1600f)
				}
			};
		}

		public void Init()
		{
			new Tech("FarmingTech", new List<string> { "AlgaeHabitat", "PlanterBox", "RationBox", "Compost" }, this, null).AddSearchTerms(SEARCH_TERMS.FARM);
			new Tech("FineDining", new List<string> { "CookingStation", "EggCracker", "DiningTable", "FarmTile" }, this, null).AddSearchTerms(SEARCH_TERMS.FOOD);
			new Tech("FoodRepurposing", new List<string> { "Juicer", "SpiceGrinder", "MilkPress", "Smoker" }, this, null).AddSearchTerms(SEARCH_TERMS.FOOD);
			new Tech("FinerDining", new List<string> { "GourmetCookingStation", "FoodDehydrator", "FoodRehydrator", "Deepfryer" }, this, null).AddSearchTerms(SEARCH_TERMS.FOOD);
			Tech tech = new Tech("Agriculture", new List<string> { "FarmStation", "FertilizerMaker", "Refrigerator", "HydroponicFarm", "ParkSign", "RadiationLight" }, this, null);
			tech.AddSearchTerms(SEARCH_TERMS.FARM);
			tech.AddSearchTerms(SEARCH_TERMS.FRIDGE);
			Tech tech2 = new Tech("Ranching", new List<string> { "RanchStation", "CreatureDeliveryPoint", "ShearingStation", "CreatureFeeder", "FishDeliveryPoint", "FishFeeder", "CritterPickUp", "CritterDropOff" }, this, null);
			tech2.AddSearchTerms(SEARCH_TERMS.CRITTER);
			tech2.AddSearchTerms(SEARCH_TERMS.FOOD);
			tech2.AddSearchTerms(SEARCH_TERMS.RANCHING);
			Tech tech3 = new Tech("AnimalControl", new List<string>
			{
				"CreatureAirTrap",
				"CreatureGroundTrap",
				"WaterTrap",
				"EggIncubator",
				LogicCritterCountSensorConfig.ID
			}, this, null);
			tech3.AddSearchTerms(SEARCH_TERMS.CRITTER);
			tech3.AddSearchTerms(SEARCH_TERMS.FOOD);
			tech3.AddSearchTerms(SEARCH_TERMS.RANCHING);
			Tech tech4 = new Tech("AnimalComfort", new List<string> { "CritterCondo", "UnderwaterCritterCondo", "AirBorneCritterCondo" }, this, null);
			tech4.AddSearchTerms(SEARCH_TERMS.CRITTER);
			tech4.AddSearchTerms(SEARCH_TERMS.RANCHING);
			Tech tech5 = new Tech("DairyOperation", new List<string> { "MilkFeeder", "MilkFatSeparator", "MilkingStation" }, this, null);
			tech5.AddSearchTerms(SEARCH_TERMS.CRITTER);
			tech5.AddSearchTerms(SEARCH_TERMS.RANCHING);
			new Tech("ImprovedOxygen", new List<string> { "Electrolyzer", "RustDeoxidizer" }, this, null).AddSearchTerms(SEARCH_TERMS.OXYGEN);
			new Tech("GasPiping", new List<string> { "GasConduit", "GasConduitBridge", "GasPump", "GasVent" }, this, null);
			new Tech("ImprovedGasPiping", new List<string>
			{
				"InsulatedGasConduit",
				LogicPressureSensorGasConfig.ID,
				"GasLogicValve",
				"GasVentHighPressure"
			}, this, null);
			new Tech("SpaceGas", new List<string> { "CO2Engine", "ModularLaunchpadPortGas", "ModularLaunchpadPortGasUnloader", "GasCargoBaySmall" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("PressureManagement", new List<string> { "LiquidValve", "GasValve", "GasPermeableMembrane", "ManualPressureDoor" }, this, null);
			new Tech("DirectedAirStreams", new List<string> { "AirFilter", "CO2Scrubber", "PressureDoor" }, this, null).AddSearchTerms(SEARCH_TERMS.FILTER);
			new Tech("LiquidFiltering", new List<string> { "OreScrubber", "Desalinator" }, this, null).AddSearchTerms(SEARCH_TERMS.FILTER);
			new Tech("MedicineI", new List<string> { "Apothecary", "LubricationStick" }, this, null).AddSearchTerms(SEARCH_TERMS.MEDICINE);
			new Tech("MedicineII", new List<string> { "DoctorStation", "HandSanitizer" }, this, null).AddSearchTerms(SEARCH_TERMS.MEDICINE);
			new Tech("MedicineIII", new List<string>
			{
				GasConduitDiseaseSensorConfig.ID,
				LiquidConduitDiseaseSensorConfig.ID,
				LogicDiseaseSensorConfig.ID
			}, this, null).AddSearchTerms(SEARCH_TERMS.MEDICINE);
			new Tech("MedicineIV", new List<string>
			{
				"AdvancedDoctorStation",
				"AdvancedApothecary",
				"HotTub",
				LogicRadiationSensorConfig.ID
			}, this, null).AddSearchTerms(SEARCH_TERMS.MEDICINE);
			new Tech("LiquidPiping", new List<string> { "LiquidConduit", "LiquidConduitBridge", "LiquidPump", "LiquidVent" }, this, null);
			new Tech("ImprovedLiquidPiping", new List<string>
			{
				"InsulatedLiquidConduit",
				LogicPressureSensorLiquidConfig.ID,
				"LiquidLogicValve",
				"LiquidConduitPreferentialFlow",
				"LiquidConduitOverflow",
				"LiquidReservoir"
			}, this, null);
			new Tech("PrecisionPlumbing", new List<string> { "EspressoMachine", "LiquidFuelTankCluster", "MercuryCeilingLight" }, this, null);
			new Tech("SanitationSciences", new List<string>
			{
				"FlushToilet",
				"WashSink",
				ShowerConfig.ID,
				"MeshTile",
				"GunkEmptier"
			}, this, null).AddSearchTerms(SEARCH_TERMS.TOILET);
			new Tech("FlowRedirection", new List<string> { "MechanicalSurfboard", "LiquidBottler", "ModularLaunchpadPortLiquid", "ModularLaunchpadPortLiquidUnloader", "LiquidCargoBaySmall" }, this, null);
			new Tech("LiquidDistribution", new List<string> { "BottleEmptierConduitLiquid", "RocketInteriorLiquidInput", "RocketInteriorLiquidOutput", "WallToilet" }, this, null);
			new Tech("AdvancedSanitation", new List<string> { "DecontaminationShower" }, this, null);
			new Tech("AdvancedFiltration", new List<string> { "GasFilter", "LiquidFilter", "SludgePress", "OilChanger" }, this, null).AddSearchTerms(SEARCH_TERMS.FILTER);
			Tech tech6 = new Tech("Distillation", new List<string> { "AlgaeDistillery", "EthanolDistillery", "WaterPurifier" }, this, null);
			tech6.AddSearchTerms(SEARCH_TERMS.WATER);
			new Tech("AdvancedDistillation", new List<string> { "ChemicalRefinery" }, this, null);
			tech6.AddSearchTerms(SEARCH_TERMS.POWER);
			new Tech("Catalytics", new List<string> { "OxyliteRefinery", "Chlorinator", "SupermaterialRefinery", "SUPER_LIQUIDS", "SodaFountain", "GasCargoBayCluster" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("AdvancedResourceExtraction", new List<string> { "NoseconeHarvest" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			Tech tech7 = new Tech("PowerRegulation", new List<string>
			{
				"BatteryMedium",
				SwitchConfig.ID,
				"WireBridge",
				"SmallElectrobankDischarger"
			}, this, null);
			tech7.AddSearchTerms(SEARCH_TERMS.POWER);
			tech7.AddSearchTerms(SEARCH_TERMS.BATTERY);
			tech7.AddSearchTerms(SEARCH_TERMS.WIRE);
			Tech tech8 = new Tech("AdvancedPowerRegulation", new List<string>
			{
				"HighWattageWire",
				"WireBridgeHighWattage",
				"HydrogenGenerator",
				LogicPowerRelayConfig.ID,
				"PowerTransformerSmall",
				LogicWattageSensorConfig.ID
			}, this, null);
			tech8.AddSearchTerms(SEARCH_TERMS.POWER);
			tech8.AddSearchTerms(SEARCH_TERMS.WIRE);
			tech8.AddSearchTerms(SEARCH_TERMS.GENERATOR);
			Tech tech9 = new Tech("PrettyGoodConductors", new List<string> { "WireRefined", "WireRefinedBridge", "WireRefinedHighWattage", "WireRefinedBridgeHighWattage", "PowerTransformer", "LargeElectrobankDischarger" }, this, null);
			tech9.AddSearchTerms(SEARCH_TERMS.WIRE);
			tech9.AddSearchTerms(SEARCH_TERMS.POWER);
			Tech tech10 = new Tech("RenewableEnergy", new List<string> { "SteamTurbine2", "SolarPanel", "Sauna", "SteamEngineCluster" }, this, null);
			tech10.AddSearchTerms(SEARCH_TERMS.POWER);
			tech10.AddSearchTerms(SEARCH_TERMS.STEAM);
			Tech tech11 = new Tech("Combustion", new List<string> { "Generator", "WoodGasGenerator", "PeatGenerator" }, this, null);
			tech11.AddSearchTerms(SEARCH_TERMS.POWER);
			tech11.AddSearchTerms(SEARCH_TERMS.GENERATOR);
			Tech tech12 = new Tech("ImprovedCombustion", new List<string> { "MethaneGenerator", "OilRefinery", "PetroleumGenerator" }, this, null);
			tech12.AddSearchTerms(SEARCH_TERMS.POWER);
			tech12.AddSearchTerms(SEARCH_TERMS.GENERATOR);
			Tech tech13 = new Tech("InteriorDecor", new List<string> { "FlowerVase", "FloorLamp", "CeilingLight" }, this, null);
			tech13.AddSearchTerms(SEARCH_TERMS.MORALE);
			tech13.AddSearchTerms(SEARCH_TERMS.ARTWORK);
			Tech tech14 = new Tech("Artistry", new List<string> { "WoodenDoor", "FlowerVaseWall", "FlowerVaseHanging", "CornerMoulding", "CrownMoulding", "ItemPedestal", "SmallSculpture", "IceSculpture" }, this, null);
			tech14.AddSearchTerms(SEARCH_TERMS.MORALE);
			tech14.AddSearchTerms(SEARCH_TERMS.ARTWORK);
			new Tech("Clothing", new List<string> { "ClothingFabricator", "CarpetTile", "ExteriorWall" }, this, null).AddSearchTerms(SEARCH_TERMS.TILE);
			Tech tech15 = new Tech("Acoustics", new List<string> { "BatterySmart", "Phonobox", "PowerControlStation", "ElectrobankCharger", "Electrobank" }, this, null);
			tech15.AddSearchTerms(SEARCH_TERMS.POWER);
			tech15.AddSearchTerms(SEARCH_TERMS.BATTERY);
			Tech tech16 = new Tech("SpacePower", new List<string> { "BatteryModule", "SolarPanelModule", "RocketInteriorPowerPlug" }, this, null);
			tech16.AddSearchTerms(SEARCH_TERMS.POWER);
			tech16.AddSearchTerms(SEARCH_TERMS.BATTERY);
			tech16.AddSearchTerms(SEARCH_TERMS.ROCKET);
			Tech tech17 = new Tech("NuclearRefinement", new List<string> { "NuclearReactor", "UraniumCentrifuge", "SelfChargingElectrobank" }, this, null);
			tech17.AddSearchTerms(SEARCH_TERMS.POWER);
			tech17.AddSearchTerms(SEARCH_TERMS.BATTERY);
			Tech tech18 = new Tech("FineArt", new List<string> { "Canvas", "Sculpture", "Shelf" }, this, null);
			tech18.AddSearchTerms(SEARCH_TERMS.MORALE);
			tech18.AddSearchTerms(SEARCH_TERMS.ARTWORK);
			Tech tech19 = new Tech("EnvironmentalAppreciation", new List<string> { "BeachChair" }, this, null);
			tech19.AddSearchTerms(SEARCH_TERMS.MORALE);
			if (DlcManager.IsContentSubscribed("DLC4_ID"))
			{
				tech19.AddSearchTerms(SEARCH_TERMS.ARTWORK);
				tech19.AddSearchTerms(SEARCH_TERMS.DINOSAUR);
			}
			Tech tech20 = new Tech("Luxury", new List<string> { "LuxuryBed", "LadderFast", "PlasticTile", "ClothingAlterationStation", "WoodTile", "MultiMinionDiningTable" }, this, null);
			tech20.AddSearchTerms(SEARCH_TERMS.TILE);
			tech20.AddSearchTerms(SEARCH_TERMS.MORALE);
			Tech tech21 = new Tech("RefractiveDecor", new List<string> { "CanvasWide", "MetalSculpture", "WoodSculpture" }, this, null);
			tech21.AddSearchTerms(SEARCH_TERMS.MORALE);
			tech21.AddSearchTerms(SEARCH_TERMS.ARTWORK);
			new Tech("GlassFurnishings", new List<string> { "GlassTile", "FlowerVaseHangingFancy", "SunLamp" }, this, null);
			new Tech("Screens", new List<string> { PixelPackConfig.ID }, this, null);
			Tech tech22 = new Tech("RenaissanceArt", new List<string> { "CanvasTall", "MarbleSculpture", "FossilSculpture", "CeilingFossilSculpture" }, this, null);
			tech22.AddSearchTerms(SEARCH_TERMS.MORALE);
			tech22.AddSearchTerms(SEARCH_TERMS.ARTWORK);
			new Tech("Plastics", new List<string> { "Polymerizer", "OilWellCap" }, this, null);
			new Tech("ValveMiniaturization", new List<string> { "LiquidMiniPump", "GasMiniPump" }, this, null);
			new Tech("HydrocarbonPropulsion", new List<string> { "KeroseneEngineClusterSmall", "MissionControlCluster" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("BetterHydroCarbonPropulsion", new List<string> { "KeroseneEngineCluster", "BiodieselEngineCluster" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("CryoFuelPropulsion", new List<string> { "HydrogenEngineCluster", "OxidizerTankLiquidCluster" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("Suits", new List<string> { "SuitsOverlay", "AtmoSuit", "SuitFabricator", "SuitMarker", "SuitLocker" }, this, null);
			new Tech("Jobs", new List<string> { "WaterCooler", "CraftingTable", "DisposableElectrobank_RawMetal", "Campfire" }, this, null);
			new Tech("AdvancedResearch", new List<string> { "BetaResearchPoint", "AdvancedResearchCenter", "ResetSkillsStation", "ClusterTelescope", "ExobaseHeadquarters", "AdvancedCraftingTable" }, this, null);
			new Tech("SpaceProgram", new List<string>
			{
				"LaunchPad",
				"HabitatModuleSmall",
				"OrbitalCargoModule",
				RocketControlStationConfig.ID
			}, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("CrashPlan", new List<string> { "OrbitalResearchPoint", "PioneerModule", "OrbitalResearchCenter", "DLC1CosmicResearchCenter" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("DurableLifeSupport", new List<string> { "NoseconeBasic", "HabitatModuleMedium", "ArtifactAnalysisStation", "ArtifactCargoBay", "SpecialCargoBayCluster" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("NuclearResearch", new List<string> { "DeltaResearchPoint", "NuclearResearchCenter", "ManualHighEnergyParticleSpawner", "DisposableElectrobank_UraniumOre" }, this, null);
			new Tech("AdvancedNuclearResearch", new List<string> { "HighEnergyParticleSpawner", "HighEnergyParticleRedirector", "HEPBridgeTile" }, this, null);
			new Tech("NuclearStorage", new List<string> { "HEPBattery" }, this, null);
			new Tech("NuclearPropulsion", new List<string> { "HEPEngine" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("NotificationSystems", new List<string>
			{
				LogicHammerConfig.ID,
				LogicAlarmConfig.ID,
				"Telephone"
			}, this, null).AddSearchTerms(SEARCH_TERMS.AUTOMATION);
			new Tech("ArtificialFriends", new List<string> { "SweepBotStation", "ScoutModule", "FetchDrone" }, this, null).AddSearchTerms(SEARCH_TERMS.ROBOT);
			new Tech("BasicRefinement", new List<string> { "RockCrusher", "Kiln" }, this, null);
			new Tech("RefinedObjects", new List<string>
			{
				"FabricatedWoodMaker",
				"FirePole",
				"ThermalBlock",
				LadderBedConfig.ID
			}, this, null);
			new Tech("Smelting", new List<string> { "MetalRefinery", "MetalTile" }, this, null);
			new Tech("HighTempForging", new List<string> { "GlassForge", "BunkerTile", "BunkerDoor", "GeoTuner" }, this, null).AddSearchTerms(SEARCH_TERMS.GLASS);
			new Tech("HighPressureForging", new List<string> { "DiamondPress" }, this, null);
			new Tech("RadiationProtection", new List<string>
			{
				"LeadSuit",
				"LeadSuitMarker",
				"LeadSuitLocker",
				LogicHEPSensorConfig.ID
			}, this, null);
			new Tech("TemperatureModulation", new List<string> { "InsulatedDoor", "LiquidCooledFan", "IceCooledFan", "IceMachine", "IceKettle", "InsulationTile", "SpaceHeater" }, this, null);
			new Tech("HVAC", new List<string>
			{
				"AirConditioner",
				LogicTemperatureSensorConfig.ID,
				GasConduitTemperatureSensorConfig.ID,
				GasConduitElementSensorConfig.ID,
				"GasConduitRadiant",
				"GasReservoir",
				"GasLimitValve"
			}, this, null);
			new Tech("LiquidTemperature", new List<string>
			{
				"LiquidConduitRadiant",
				"LiquidConditioner",
				LiquidConduitTemperatureSensorConfig.ID,
				LiquidConduitElementSensorConfig.ID,
				"LiquidHeater",
				"LiquidLimitValve",
				"ContactConductivePipeBridge"
			}, this, null);
			new Tech("LogicControl", new List<string>
			{
				"AutomationOverlay",
				LogicSwitchConfig.ID,
				"LogicWire",
				"LogicWireBridge",
				"LogicDuplicantSensor"
			}, this, null).AddSearchTerms(SEARCH_TERMS.AUTOMATION);
			new Tech("GenericSensors", new List<string>
			{
				"FloorSwitch",
				LogicElementSensorGasConfig.ID,
				LogicElementSensorLiquidConfig.ID,
				"LogicGateNOT",
				LogicTimeOfDaySensorConfig.ID,
				LogicTimerSensorConfig.ID,
				LogicLightSensorConfig.ID,
				LogicClusterLocationSensorConfig.ID
			}, this, null).AddSearchTerms(SEARCH_TERMS.AUTOMATION);
			new Tech("LogicCircuits", new List<string> { "LogicGateAND", "LogicGateOR", "LogicGateBUFFER", "LogicGateFILTER" }, this, null).AddSearchTerms(SEARCH_TERMS.AUTOMATION);
			new Tech("ParallelAutomation", new List<string>
			{
				"LogicRibbon",
				"LogicRibbonBridge",
				LogicRibbonWriterConfig.ID,
				LogicRibbonReaderConfig.ID
			}, this, null).AddSearchTerms(SEARCH_TERMS.AUTOMATION);
			Tech tech23 = new Tech("DupeTrafficControl", new List<string>
			{
				LogicCounterConfig.ID,
				LogicMemoryConfig.ID,
				"LogicGateXOR",
				"ArcadeMachine",
				"Checkpoint",
				"CosmicResearchCenter"
			}, this, null);
			tech23.AddSearchTerms(SEARCH_TERMS.AUTOMATION);
			tech23.AddSearchTerms(SEARCH_TERMS.RESEARCH);
			tech23.AddSearchTerms(SEARCH_TERMS.MORALE);
			new Tech("Multiplexing", new List<string> { "LogicGateMultiplexer", "LogicGateDemultiplexer" }, this, null).AddSearchTerms(SEARCH_TERMS.AUTOMATION);
			new Tech("SkyDetectors", new List<string>
			{
				CometDetectorConfig.ID,
				"Telescope",
				"ResearchClusterModule",
				"ClusterTelescopeEnclosed",
				"AstronautTrainingCenter"
			}, this, null).AddSearchTerms(SEARCH_TERMS.RESEARCH);
			new Tech("Missiles", new List<string> { "MissileFabricator", "MissileLauncher" }, this, null);
			new Tech("TravelTubes", new List<string> { "TravelTubeEntrance", "TravelTube", "TravelTubeWallBridge", "VerticalWindTunnel" }, this, null).AddSearchTerms(SEARCH_TERMS.TRANSPORT);
			new Tech("SmartStorage", new List<string> { "ConveyorOverlay", "SolidTransferArm", "StorageLockerSmart", "ObjectDispenser" }, this, null).AddSearchTerms(SEARCH_TERMS.STORAGE);
			Tech tech24 = new Tech("SolidManagement", new List<string>
			{
				"SolidFilter",
				SolidConduitTemperatureSensorConfig.ID,
				SolidConduitElementSensorConfig.ID,
				SolidConduitDiseaseSensorConfig.ID,
				"StorageTile",
				"CargoBayCluster"
			}, this, null);
			tech24.AddSearchTerms(SEARCH_TERMS.AUTOMATION);
			tech24.AddSearchTerms(SEARCH_TERMS.TRANSPORT);
			tech24.AddSearchTerms(SEARCH_TERMS.STORAGE);
			new Tech("HighVelocityTransport", new List<string> { "RailGun", "LandingBeacon" }, this, null).AddSearchTerms(SEARCH_TERMS.TRANSPORT);
			Tech tech25 = new Tech("BasicRocketry", new List<string> { "CommandModule", "SteamEngine", "ResearchModule", "Gantry" }, this, null);
			tech25.AddSearchTerms(SEARCH_TERMS.ROCKET);
			tech25.AddSearchTerms(SEARCH_TERMS.RESEARCH);
			tech25.AddSearchTerms(SEARCH_TERMS.STEAM);
			new Tech("CargoI", new List<string> { "CargoBay" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("CargoII", new List<string> { "LiquidCargoBay", "GasCargoBay" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("CargoIII", new List<string> { "TouristModule", "SpecialCargoBay" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("EnginesI", new List<string> { "SolidBooster", "MissionControl" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("EnginesII", new List<string> { "KeroseneEngine", "BiodieselEngine", "LiquidFuelTank", "OxidizerTank" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("EnginesIII", new List<string> { "OxidizerTankLiquid", "OxidizerTankCluster", "HydrogenEngine" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			Tech tech26 = new Tech("Jetpacks", new List<string> { "JetSuit", "JetSuitMarker", "JetSuitLocker", "LiquidCargoBayCluster" }, this, null);
			tech26.AddSearchTerms(SEARCH_TERMS.ROCKET);
			tech26.AddSearchTerms(SEARCH_TERMS.MISSILE);
			new Tech("SolidTransport", new List<string> { "SolidConduitInbox", "SolidConduit", "SolidConduitBridge", "SolidVent", "SolidCargoBaySmall", "ModularLaunchpadPortSolid", "ModularLaunchpadPortSolidUnloader", "ModularLaunchpadPortBridge" }, this, null).AddSearchTerms(SEARCH_TERMS.TRANSPORT);
			Tech tech27 = new Tech("Monuments", new List<string> { "MonumentBottom", "MonumentMiddle", "MonumentTop" }, this, null);
			tech27.AddSearchTerms(SEARCH_TERMS.ARTWORK);
			tech27.AddSearchTerms(SEARCH_TERMS.MORALE);
			Tech tech28 = new Tech("SolidSpace", new List<string> { "SolidLogicValve", "SolidConduitOutbox", "SolidLimitValve", "RocketInteriorSolidInput", "RocketInteriorSolidOutput" }, this, null);
			tech28.AddSearchTerms(SEARCH_TERMS.AUTOMATION);
			tech28.AddSearchTerms(SEARCH_TERMS.ROCKET);
			tech28.AddSearchTerms(SEARCH_TERMS.TRANSPORT);
			new Tech("RoboticTools", new List<string> { "AutoMiner", "RailGunPayloadOpener", "RoboPilotModule" }, this, null).AddSearchTerms(SEARCH_TERMS.ROBOT);
			new Tech("PortableGasses", new List<string> { "GasBottler", "BottleEmptierGas", "OxygenMask", "OxygenMaskLocker", "OxygenMaskMarker", "Oxysconce" }, this, null).AddSearchTerms(SEARCH_TERMS.OXYGEN);
			new Tech("GasDistribution", new List<string> { "BottleEmptierConduitGas", "RocketInteriorGasInput", "RocketInteriorGasOutput", "OxidizerTankCluster" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			this.InitBaseGameOnly();
			this.InitExpansion1();
		}

		private void InitBaseGameOnly()
		{
			if (DlcManager.IsExpansion1Active())
			{
				return;
			}
			if (DlcManager.IsContentSubscribed("DLC3_ID"))
			{
				new Tech("DataScienceBaseGame", new List<string>
				{
					"DataMiner",
					RemoteWorkerDockConfig.ID,
					RemoteWorkTerminalConfig.ID,
					"RoboPilotCommandModule"
				}, this, null).AddSearchTerms(SEARCH_TERMS.ROBOT);
			}
		}

		private void InitExpansion1()
		{
			if (!DlcManager.IsExpansion1Active())
			{
				return;
			}
			base.Get("HighTempForging").AddUnlockedItemIDs(new string[] { "Gantry" });
			new Tech("Bioengineering", new List<string> { "GeneticAnalysisStation" }, this, null).AddSearchTerms(SEARCH_TERMS.RESEARCH);
			new Tech("SpaceCombustion", new List<string> { "SugarEngine", "SmallOxidizerTank" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("HighVelocityDestruction", new List<string> { "NoseconeHarvest" }, this, null).AddSearchTerms(SEARCH_TERMS.ROCKET);
			new Tech("AdvancedScanners", new List<string> { "ScannerModule", "LogicInterasteroidSender", "LogicInterasteroidReceiver" }, this, null).AddSearchTerms(SEARCH_TERMS.AUTOMATION);
			if (DlcManager.IsContentSubscribed("DLC3_ID"))
			{
				new Tech("DataScience", new List<string>
				{
					"DataMiner",
					RemoteWorkerDockConfig.ID,
					RemoteWorkTerminalConfig.ID
				}, this, null);
			}
		}

		public void PostProcess()
		{
			foreach (Tech tech in this.resources)
			{
				List<TechItem> list = new List<TechItem>();
				foreach (string text in tech.unlockedItemIDs)
				{
					TechItem techItem = Db.Get().TechItems.TryGet(text);
					if (techItem != null)
					{
						list.Add(techItem);
					}
				}
				tech.unlockedItems = list;
			}
		}

		public void Load(TextAsset tree_file)
		{
			ResourceTreeLoader<ResourceTreeNode> resourceTreeLoader = new ResourceTreeLoader<ResourceTreeNode>(tree_file);
			List<TechTreeTitle> list = new List<TechTreeTitle>();
			for (int i = 0; i < Db.Get().TechTreeTitles.Count; i++)
			{
				list.Add(Db.Get().TechTreeTitles[i]);
			}
			list.Sort((TechTreeTitle a, TechTreeTitle b) => a.center.y.CompareTo(b.center.y));
			foreach (ResourceTreeNode resourceTreeNode in resourceTreeLoader)
			{
				if (!string.Equals(resourceTreeNode.Id.Substring(0, 1), "_"))
				{
					Tech tech = base.TryGet(resourceTreeNode.Id);
					if (tech != null)
					{
						string text = "";
						for (int j = 0; j < list.Count; j++)
						{
							if (list[j].center.y >= resourceTreeNode.center.y)
							{
								text = list[j].Id;
								break;
							}
						}
						tech.SetNode(resourceTreeNode, text);
						foreach (ResourceTreeNode resourceTreeNode2 in resourceTreeNode.references)
						{
							Tech tech2 = base.TryGet(resourceTreeNode2.Id);
							if (tech2 != null)
							{
								text = "";
								for (int k = 0; k < list.Count; k++)
								{
									if (list[k].center.y >= resourceTreeNode.center.y)
									{
										text = list[k].Id;
										break;
									}
								}
								tech2.SetNode(resourceTreeNode2, text);
								tech2.requiredTech.Add(tech);
								tech.unlockedTech.Add(tech2);
							}
						}
					}
				}
			}
			foreach (Tech tech3 in this.resources)
			{
				tech3.tier = Techs.GetTier(tech3);
				foreach (global::Tuple<string, float> tuple in this.TECH_TIERS[tech3.tier])
				{
					if (!tech3.costsByResearchTypeID.ContainsKey(tuple.first))
					{
						tech3.costsByResearchTypeID.Add(tuple.first, tuple.second);
					}
				}
			}
			for (int l = this.Count - 1; l >= 0; l--)
			{
				if (!((Tech)this.GetResource(l)).FoundNode)
				{
					this.Remove((Tech)this.GetResource(l));
				}
			}
		}

		public static int GetTier(Tech tech)
		{
			if (tech == null)
			{
				return 0;
			}
			int num = 0;
			foreach (Tech tech2 in tech.requiredTech)
			{
				num = Math.Max(num, Techs.GetTier(tech2));
			}
			return num + 1;
		}

		private void AddPrerequisite(Tech tech, string prerequisite_name)
		{
			Tech tech2 = base.TryGet(prerequisite_name);
			if (tech2 != null)
			{
				tech.requiredTech.Add(tech2);
				tech2.unlockedTech.Add(tech);
			}
		}

		public Tech TryGetTechForTechItem(string itemId)
		{
			Predicate<string> <>9__0;
			for (int i = 0; i < this.Count; i++)
			{
				Tech tech = (Tech)this.GetResource(i);
				List<string> unlockedItemIDs = tech.unlockedItemIDs;
				Predicate<string> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = (string candidateItemId) => candidateItemId == itemId);
				}
				if (unlockedItemIDs.Find(predicate) != null)
				{
					return tech;
				}
			}
			return null;
		}

		public bool IsTechItemComplete(string id)
		{
			foreach (Tech tech in this.resources)
			{
				using (List<TechItem>.Enumerator enumerator2 = tech.unlockedItems.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.Id == id)
						{
							return tech.IsComplete();
						}
					}
				}
			}
			return true;
		}

		private readonly List<List<global::Tuple<string, float>>> TECH_TIERS;
	}
}
