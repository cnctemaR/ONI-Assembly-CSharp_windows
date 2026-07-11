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
			ResourceTreeLoader<ResourceTreeNode> resourceTreeLoader = new ResourceTreeLoader<ResourceTreeNode>(tree_file);
			foreach (ResourceTreeNode resourceTreeNode in resourceTreeLoader)
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
			this.tierCount = 0;
			foreach (Tech tech3 in this.resources)
			{
				tech3.tier = this.GetTier(tech3);
				List<Tuple<string, float>> list = this.TECH_TIERS[tech3.tier];
				foreach (Tuple<string, float> tuple in list)
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
				foreach (TechItem techItem in tech.unlockedItems)
				{
					if (techItem.Id == id)
					{
						return tech.IsComplete();
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
				"Agriculture",
				new string[] { "FertilizerMaker", "HydroponicFarm", "Refrigerator", "FarmStation" }
			},
			{
				"Ranching",
				new string[] { "CreatureDeliveryPoint", "FishDeliveryPoint", "CreatureFeeder", "FishFeeder", "RanchStation", "ShearingStation" }
			},
			{
				"AnimalControl",
				new string[] { "CreatureTrap", "FishTrap", "AirborneCreatureLure", "EggIncubator" }
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
					LogicPressureSensorGasConfig.ID,
					"GasVentHighPressure",
					"GasLogicValve",
					"GasConduitPreferentialFlow",
					"GasConduitOverflow"
				}
			},
			{
				"Clothing",
				new string[] { "ClothingFabricator", "Phonobox" }
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
					"PlasticTile",
					"ExteriorWall"
				}
			},
			{
				"RefractiveDecor",
				new string[] { "GlassTile" }
			},
			{
				"ImprovedLiquidPiping",
				new string[]
				{
					"InsulatedLiquidConduit",
					LogicPressureSensorLiquidConfig.ID,
					"LiquidLogicValve",
					"LiquidConduitPreferentialFlow",
					"LiquidConduitOverflow"
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
				"Medbay",
				new string[]
				{
					"HandSanitizer",
					"MedicalBed",
					GasConduitDiseaseSensorConfig.ID,
					LiquidConduitDiseaseSensorConfig.ID
				}
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
				new string[]
				{
					SwitchConfig.ID,
					"BatteryMedium",
					"WireBridge"
				}
			},
			{
				"AdvancedPowerRegulation",
				new string[] { "HighWattageWire", "WireBridgeHighWattage", "PowerTransformerSmall", "PowerControlStation" }
			},
			{
				"PrettyGoodConductors",
				new string[] { "WireRefined", "WireRefinedBridge", "WireRefinedHighWattage", "WireRefinedBridgeHighWattage", "PowerTransformer" }
			},
			{
				"RenewableEnergy",
				new string[] { "SteamTurbine", "SolarPanel" }
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
				new string[] { "Canvas", "Sculpture", "IceSculpture" }
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
				new string[] { "SuitMarker", "SuitLocker", "SuitFabricator", "SuitsOverlay" }
			},
			{
				"AdvancedResearch",
				new string[] { "AdvancedResearchCenter", "RoleStation", "BetaResearchPoint", "WaterCooler" }
			},
			{
				"BasicRefinement",
				new string[] { "RockCrusher", "Kiln" }
			},
			{
				"MedicalResearch",
				new string[]
				{
					"Apothecary",
					LogicDiseaseSensorConfig.ID
				}
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
				new string[] { "LiquidCooledFan", "SpaceHeater", "InsulationTile" }
			},
			{
				"HVAC",
				new string[]
				{
					"AirConditioner",
					LogicTemperatureSensorConfig.ID,
					"GasConduitRadiant",
					GasConduitTemperatureSensorConfig.ID,
					GasConduitElementSensorConfig.ID
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
					"AutomationOverlay",
					"LogicWire",
					"LogicWireBridge",
					LogicSwitchConfig.ID,
					LogicPowerRelayConfig.ID
				}
			},
			{
				"GenericSensors",
				new string[]
				{
					LogicTimeOfDaySensorConfig.ID,
					"FloorSwitch",
					LogicElementSensorGasConfig.ID,
					"BatterySmart"
				}
			},
			{
				"LogicCircuits",
				new string[] { "LogicGateAND", "LogicGateOR", "LogicGateXOR", "LogicGateNOT", "LogicGateBUFFER", "LogicGateFILTER", "BatterySmart" }
			},
			{
				"DupeTrafficControl",
				new string[]
				{
					"Checkpoint",
					LogicMemoryConfig.ID,
					"ArcadeMachine"
				}
			},
			{
				"SkyDetectors",
				new string[] { CometDetectorConfig.ID }
			},
			{
				"TravelTubes",
				new string[] { "TravelTubeEntrance", "TravelTube", "TravelTubeWallBridge" }
			},
			{
				"SmartStorage",
				new string[] { "StorageLockerSmart", "SolidTransferArm", "ConveyorOverlay" }
			},
			{
				"SolidTransport",
				new string[] { "SolidConduit", "SolidConduitBridge", "SolidConduitInbox", "SolidConduitOutbox" }
			}
		};

		private readonly List<List<Tuple<string, float>>> TECH_TIERS = new List<List<Tuple<string, float>>>
		{
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 15f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 20f),
				new Tuple<string, float>("beta", 10f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 30f),
				new Tuple<string, float>("beta", 20f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 35f),
				new Tuple<string, float>("beta", 30f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 40f),
				new Tuple<string, float>("beta", 50f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 50f),
				new Tuple<string, float>("beta", 70f)
			}
		};
	}
}
