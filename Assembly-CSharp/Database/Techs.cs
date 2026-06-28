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
			foreach (Tech tech3 in this)
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
			foreach (Tech tech in this)
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
				new string[] { "DiningTable", "FarmTile", "CookingStation" }
			},
			{
				"Agriculture",
				new string[] { "FertilizerMaker", "HydroponicFarm", "Refrigerator", "FarmStation" }
			},
			{
				"AnimalControl",
				new string[] { "CreatureTrap", "AirborneCreatureLure", "CreatureDeliveryPoint", "AirborneCreatureLure" }
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
					"GasLogicValve"
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
					LogicPressureSensorLiquidConfig.ID,
					"LiquidLogicValve"
				}
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
				new string[]
				{
					SwitchConfig.ID,
					"BatteryMedium",
					"WireBridge"
				}
			},
			{
				"AdvancedPowerRegulation",
				new string[] { "HighWattageWire", "WireBridgeHighWattage", "PowerTransformer", "PowerControlStation" }
			},
			{
				"PrettyGoodConductors",
				new string[] { "WireRefined", "WireRefinedBridge", "WireRefinedHighWattage", "WireRefinedBridgeHighWattage" }
			},
			{
				"RenewableEnergy",
				new string[] { "SteamTurbine" }
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
				"ValveMiniaturization",
				new string[] { "LiquidMiniPump", "GasMiniPump" }
			},
			{
				"Suits",
				new string[] { "SuitMarker", "SuitLocker", "SuitFabricator", "SuitsOverlay" }
			},
			{
				"AdvancedResearch",
				new string[] { "AdvancedResearchCenter", "BetaResearchPoint" }
			},
			{
				"BasicRefinement",
				new string[] { "RockCrusher" }
			},
			{
				"MedicalResearch",
				new string[] { "Apothecary" }
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
				"TemperatureModulation",
				new string[] { "LiquidCooledFan", "SpaceHeater", "InsulationTile" }
			},
			{
				"HVAC",
				new string[]
				{
					"AirConditioner",
					LogicTemperatureSensorConfig.ID
				}
			},
			{
				"LiquidTemperature",
				new string[] { "LiquidHeater", "LiquidConditioner" }
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
					LogicDiseaseSensorConfig.ID,
					"BatterySmart"
				}
			},
			{
				"LogicCircuits",
				new string[] { "LogicGateAND", "LogicGateOR", "LogicGateXOR", "LogicGateNOT", "LogicGateBUFFER", "LogicGateFILTER", "BatterySmart" }
			},
			{
				"DupeTrafficControl",
				new string[] { "Checkpoint" }
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
				new Tuple<string, float>(ResearchTypes.ID.ALPHA, 15f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>(ResearchTypes.ID.ALPHA, 20f),
				new Tuple<string, float>(ResearchTypes.ID.BETA, 10f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>(ResearchTypes.ID.ALPHA, 30f),
				new Tuple<string, float>(ResearchTypes.ID.BETA, 20f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>(ResearchTypes.ID.ALPHA, 35f),
				new Tuple<string, float>(ResearchTypes.ID.BETA, 30f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>(ResearchTypes.ID.ALPHA, 40f),
				new Tuple<string, float>(ResearchTypes.ID.BETA, 50f)
			}
		};
	}
}
