using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
	public class Techs : ResourceSet<Tech>
	{
		public Techs(ResourceSet parent)
			: base("Techs", parent)
		{
		}

		public void Load(TextAsset tree_file)
		{
			foreach (ResourceTreeNode resourceTreeNode in new ResourceTreeLoader<ResourceTreeNode>(tree_file))
			{
				if (!string.Equals(resourceTreeNode.Id.Substring(0, 1), "_"))
				{
					Tech tech = base.TryGet(resourceTreeNode.Id);
					if (tech == null)
					{
						tech = new Tech(resourceTreeNode.Id, this, Strings.Get("STRINGS.RESEARCH.TECHS." + resourceTreeNode.Id.ToUpper() + ".NAME"), Strings.Get("STRINGS.RESEARCH.TECHS." + resourceTreeNode.Id.ToUpper() + ".DESC"), resourceTreeNode);
					}
					foreach (ResourceTreeNode resourceTreeNode2 in resourceTreeNode.references)
					{
						Tech tech2 = base.TryGet(resourceTreeNode2.Id);
						if (tech2 == null)
						{
							tech2 = new Tech(resourceTreeNode2.Id, this, Strings.Get("STRINGS.RESEARCH.TECHS." + resourceTreeNode2.Id.ToUpper() + ".NAME"), Strings.Get("STRINGS.RESEARCH.TECHS." + resourceTreeNode2.Id.ToUpper() + ".DESC"), resourceTreeNode2);
						}
						tech2.requiredTech.Add(tech);
						tech.unlockedTech.Add(tech2);
					}
				}
			}
			this.tierCount = 0;
			foreach (Tech tech3 in this.resources)
			{
				tech3.tier = this.GetTier(tech3);
				foreach (global::Tuple<string, float> tuple in this.TECH_TIERS[tech3.tier])
				{
					tech3.costsByResearchTypeID.Add(tuple.first, tuple.second);
				}
				this.tierCount = Math.Max(tech3.tier + 1, this.tierCount);
			}
		}

		private int GetTier(Tech tech)
		{
			if (tech.requiredTech.Count == 0)
			{
				return 0;
			}
			int num = 0;
			foreach (Tech tech2 in tech.requiredTech)
			{
				num = Math.Max(num, this.GetTier(tech2));
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

		public int tierCount;

		public static Dictionary<string, string[]> TECH_GROUPING = new Dictionary<string, string[]>
		{
			{
				"FarmingTech",
				new string[] { "AlgaeHabitat", "PlanterBox", "RationBox", "Compost" }
			},
			{
				"FineDining",
				new string[] { "DiningTable", "FarmTile", "CookingStation", "EggCracker" }
			},
			{
				"FoodRepurposing",
				new string[] { "Juicer" }
			},
			{
				"FinerDining",
				new string[] { "GourmetCookingStation" }
			},
			{
				"Agriculture",
				new string[] { "FertilizerMaker", "HydroponicFarm", "Refrigerator", "FarmStation", "ParkSign" }
			},
			{
				"Ranching",
				new string[] { "CreatureDeliveryPoint", "FishDeliveryPoint", "CreatureFeeder", "FishFeeder", "RanchStation", "ShearingStation", "FlyingCreatureBait" }
			},
			{
				"AnimalControl",
				new string[]
				{
					"CreatureTrap",
					"FishTrap",
					"AirborneCreatureLure",
					"EggIncubator",
					LogicCritterCountSensorConfig.ID
				}
			},
			{
				"ImprovedOxygen",
				new string[] { "Electrolyzer", "RustDeoxidizer" }
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
					LogicPressureSensorGasConfig.ID,
					"GasVentHighPressure",
					"GasLogicValve",
					"GasBottler",
					"GasConduitPreferentialFlow",
					"GasConduitOverflow"
				}
			},
			{
				"PressureManagement",
				new string[] { "LiquidValve", "GasValve", "ManualPressureDoor", "GasPermeableMembrane" }
			},
			{
				"DirectedAirStreams",
				new string[] { "PressureDoor", "AirFilter", "CO2Scrubber" }
			},
			{
				"LiquidFiltering",
				new string[] { "OreScrubber", "Desalinator" }
			},
			{
				"MedicineI",
				new string[] { "Apothecary" }
			},
			{
				"MedicineII",
				new string[] { "DoctorStation", "HandSanitizer" }
			},
			{
				"MedicineIII",
				new string[]
				{
					LogicDiseaseSensorConfig.ID,
					GasConduitDiseaseSensorConfig.ID,
					LiquidConduitDiseaseSensorConfig.ID
				}
			},
			{
				"MedicineIV",
				new string[] { "AdvancedDoctorStation", "HotTub" }
			},
			{
				"LiquidPiping",
				new string[] { "LiquidConduit", "LiquidPump", "LiquidVent", "LiquidConduitBridge" }
			},
			{
				"ImprovedLiquidPiping",
				new string[]
				{
					"InsulatedLiquidConduit",
					LogicPressureSensorLiquidConfig.ID,
					"LiquidLogicValve",
					"LiquidConduitPreferentialFlow",
					"LiquidConduitOverflow",
					"LiquidReservoir"
				}
			},
			{
				"PrecisionPlumbing",
				new string[] { "EspressoMachine" }
			},
			{
				"SanitationSciences",
				new string[]
				{
					"WashSink",
					"FlushToilet",
					ShowerConfig.ID,
					"MeshTile"
				}
			},
			{
				"FlowRedirection",
				new string[] { "MechanicalSurfboard" }
			},
			{
				"AdvancedFiltration",
				new string[] { "GasFilter", "LiquidFilter" }
			},
			{
				"Distillation",
				new string[] { "WaterPurifier", "AlgaeDistillery", "EthanolDistillery", "BottleEmptierGas" }
			},
			{
				"Catalytics",
				new string[] { "OxyliteRefinery", "SupermaterialRefinery", "SodaFountain" }
			},
			{
				"PowerRegulation",
				new string[]
				{
					SwitchConfig.ID,
					"BatteryMedium",
					"WireBridge"
				}
			},
			{
				"AdvancedPowerRegulation",
				new string[]
				{
					"HydrogenGenerator",
					"HighWattageWire",
					"WireBridgeHighWattage",
					"PowerTransformerSmall",
					LogicPowerRelayConfig.ID
				}
			},
			{
				"PrettyGoodConductors",
				new string[] { "WireRefined", "WireRefinedBridge", "WireRefinedHighWattage", "WireRefinedBridgeHighWattage", "PowerTransformer" }
			},
			{
				"RenewableEnergy",
				new string[] { "SteamTurbine", "SteamTurbine2", "SolarPanel", "Sauna" }
			},
			{
				"Combustion",
				new string[] { "Generator", "WoodGasGenerator" }
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
				new string[] { "CrownMoulding", "CornerMoulding", "SmallSculpture", "IceSculpture", "ItemPedestal", "FlowerVaseWall", "FlowerVaseHanging" }
			},
			{
				"Clothing",
				new string[] { "ClothingFabricator", "CarpetTile" }
			},
			{
				"Acoustics",
				new string[] { "Phonobox", "BatterySmart", "PowerControlStation" }
			},
			{
				"FineArt",
				new string[] { "Canvas", "Sculpture" }
			},
			{
				"EnvironmentalAppreciation",
				new string[] { "BeachChair" }
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
				"RefractiveDecor",
				new string[] { "MetalSculpture", "CanvasWide" }
			},
			{
				"GlassFurnishings",
				new string[] { "GlassTile", "FlowerVaseHangingFancy", "SunLamp" }
			},
			{
				"RenaissanceArt",
				new string[] { "MarbleSculpture", "CanvasTall", "MonumentBottom", "MonumentMiddle", "MonumentTop" }
			},
			{
				"Plastics",
				new string[] { "Polymerizer", "OilWellCap" }
			},
			{
				"ValveMiniaturization",
				new string[] { "LiquidMiniPump", "GasMiniPump" }
			},
			{
				"Suits",
				new string[] { "ExteriorWall", "SuitMarker", "SuitLocker", "SuitFabricator", "SuitsOverlay" }
			},
			{
				"Jobs",
				new string[] { "RoleStation", "WaterCooler" }
			},
			{
				"AdvancedResearch",
				new string[] { "AdvancedResearchCenter", "BetaResearchPoint", "ResetSkillsStation" }
			},
			{
				"BasicRefinement",
				new string[] { "RockCrusher", "Kiln" }
			},
			{
				"RefinedObjects",
				new string[] { "ThermalBlock", "FirePole" }
			},
			{
				"Smelting",
				new string[] { "MetalRefinery", "MetalTile" }
			},
			{
				"HighTempForging",
				new string[] { "GlassForge", "BunkerTile", "BunkerDoor" }
			},
			{
				"TemperatureModulation",
				new string[] { "LiquidCooledFan", "IceCooledFan", "IceMachine", "SpaceHeater", "InsulationTile" }
			},
			{
				"HVAC",
				new string[]
				{
					"AirConditioner",
					LogicTemperatureSensorConfig.ID,
					"GasConduitRadiant",
					GasConduitTemperatureSensorConfig.ID,
					GasConduitElementSensorConfig.ID,
					"GasReservoir"
				}
			},
			{
				"LiquidTemperature",
				new string[]
				{
					"LiquidHeater",
					"LiquidConditioner",
					"LiquidConduitRadiant",
					LiquidConduitTemperatureSensorConfig.ID,
					LiquidConduitElementSensorConfig.ID
				}
			},
			{
				"LogicControl",
				new string[]
				{
					"LogicWire",
					"LogicDuplicantSensor",
					LogicSwitchConfig.ID,
					"LogicWireBridge",
					"AutomationOverlay"
				}
			},
			{
				"GenericSensors",
				new string[]
				{
					LogicTimeOfDaySensorConfig.ID,
					"FloorSwitch",
					LogicElementSensorGasConfig.ID,
					LogicElementSensorLiquidConfig.ID,
					"BatterySmart",
					"LogicGateNOT"
				}
			},
			{
				"LogicCircuits",
				new string[] { "LogicGateAND", "LogicGateOR", "LogicGateBUFFER", "LogicGateFILTER" }
			},
			{
				"DupeTrafficControl",
				new string[]
				{
					"Checkpoint",
					LogicMemoryConfig.ID,
					"ArcadeMachine",
					"CosmicResearchCenter",
					"LogicGateXOR"
				}
			},
			{
				"SkyDetectors",
				new string[]
				{
					CometDetectorConfig.ID,
					"Telescope",
					"AstronautTrainingCenter"
				}
			},
			{
				"TravelTubes",
				new string[] { "TravelTubeEntrance", "TravelTube", "TravelTubeWallBridge", "VerticalWindTunnel" }
			},
			{
				"SmartStorage",
				new string[] { "StorageLockerSmart", "SolidTransferArm", "ObjectDispenser", "ConveyorOverlay" }
			},
			{
				"SolidTransport",
				new string[] { "SolidConduit", "SolidConduitBridge", "SolidConduitInbox", "SolidConduitOutbox", "SolidVent", "SolidLogicValve", "AutoMiner" }
			},
			{
				"BasicRocketry",
				new string[] { "CommandModule", "SteamEngine", "ResearchModule", "Gantry" }
			},
			{
				"CargoI",
				new string[] { "CargoBay" }
			},
			{
				"CargoII",
				new string[] { "LiquidCargoBay", "GasCargoBay" }
			},
			{
				"CargoIII",
				new string[] { "TouristModule", "SpecialCargoBay" }
			},
			{
				"EnginesI",
				new string[] { "SolidBooster" }
			},
			{
				"EnginesII",
				new string[] { "KeroseneEngine", "LiquidFuelTank", "OxidizerTank" }
			},
			{
				"EnginesIII",
				new string[] { "OxidizerTankLiquid", "HydrogenEngine" }
			},
			{
				"Jetpacks",
				new string[] { "JetSuit", "JetSuitMarker", "JetSuitLocker" }
			}
		};

		private readonly List<List<global::Tuple<string, float>>> TECH_TIERS = new List<List<global::Tuple<string, float>>>
		{
			new List<global::Tuple<string, float>>
			{
				new global::Tuple<string, float>("alpha", 15f)
			},
			new List<global::Tuple<string, float>>
			{
				new global::Tuple<string, float>("alpha", 20f)
			},
			new List<global::Tuple<string, float>>
			{
				new global::Tuple<string, float>("alpha", 30f),
				new global::Tuple<string, float>("beta", 20f)
			},
			new List<global::Tuple<string, float>>
			{
				new global::Tuple<string, float>("alpha", 35f),
				new global::Tuple<string, float>("beta", 30f)
			},
			new List<global::Tuple<string, float>>
			{
				new global::Tuple<string, float>("alpha", 40f),
				new global::Tuple<string, float>("beta", 50f)
			},
			new List<global::Tuple<string, float>>
			{
				new global::Tuple<string, float>("alpha", 50f),
				new global::Tuple<string, float>("beta", 70f)
			},
			new List<global::Tuple<string, float>>
			{
				new global::Tuple<string, float>("alpha", 70f),
				new global::Tuple<string, float>("beta", 100f)
			},
			new List<global::Tuple<string, float>>
			{
				new global::Tuple<string, float>("alpha", 70f),
				new global::Tuple<string, float>("beta", 100f),
				new global::Tuple<string, float>("gamma", 200f)
			},
			new List<global::Tuple<string, float>>
			{
				new global::Tuple<string, float>("alpha", 70f),
				new global::Tuple<string, float>("beta", 100f),
				new global::Tuple<string, float>("gamma", 400f)
			},
			new List<global::Tuple<string, float>>
			{
				new global::Tuple<string, float>("alpha", 70f),
				new global::Tuple<string, float>("beta", 100f),
				new global::Tuple<string, float>("gamma", 800f)
			},
			new List<global::Tuple<string, float>>
			{
				new global::Tuple<string, float>("alpha", 70f),
				new global::Tuple<string, float>("beta", 100f),
				new global::Tuple<string, float>("gamma", 1600f)
			}
		};
	}
}
