using System;
using System.Collections.Generic;
using UnityEngine;

public class HarvestablePOIConfig : IMultiEntityConfig
{
	public List<GameObject> CreatePrefabs()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (HarvestablePOIConfig.HarvestablePOIParams harvestablePOIParams in this.GenerateConfigs())
		{
			list.Add(HarvestablePOIConfig.CreateHarvestablePOI(harvestablePOIParams));
		}
		return list;
	}

	public static GameObject CreateHarvestablePOI(HarvestablePOIConfig.HarvestablePOIParams def)
	{
		GameObject gameObject = EntityTemplates.CreateEntity(def.id, def.id, true);
		string text = Strings.Get(def.nameStringKey);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<HarvestablePOIConfigurator>().presetType = def.poiType.idHash;
		HarvestablePOIClusterGridEntity harvestablePOIClusterGridEntity = gameObject.AddOrGet<HarvestablePOIClusterGridEntity>();
		harvestablePOIClusterGridEntity.m_name = text;
		harvestablePOIClusterGridEntity.m_Anim = def.anim;
		if (def.poiType.initialDataBanks > 0 || def.poiType.initialLiberatedResources != null)
		{
			List<ClusterGridOneTimeResourceSpawner.Data> list = new List<ClusterGridOneTimeResourceSpawner.Data>();
			if (def.poiType.initialDataBanks > 0)
			{
				list.Add(new ClusterGridOneTimeResourceSpawner.Data
				{
					itemID = DatabankHelper.ID,
					mass = 1f * (float)def.poiType.initialDataBanks
				});
			}
			if (def.poiType.initialLiberatedResources != null)
			{
				foreach (SimHashes simHashes in def.poiType.initialLiberatedResources.Keys)
				{
					float num = def.poiType.initialLiberatedResources[simHashes];
					if (num > 0f)
					{
						list.Add(new ClusterGridOneTimeResourceSpawner.Data
						{
							itemID = simHashes.CreateTag(),
							mass = num
						});
					}
				}
			}
			if (list.Count > 0)
			{
				gameObject.AddOrGetDef<ClusterGridOneTimeResourceSpawner.Def>().thingsToSpawn = list;
			}
		}
		gameObject.AddOrGetDef<HarvestablePOIStates.Def>();
		if (def.poiType.canProvideArtifacts)
		{
			gameObject.AddOrGetDef<ArtifactPOIStates.Def>();
			gameObject.AddOrGet<ArtifactPOIConfigurator>().presetType = ArtifactPOIConfigurator.defaultArtifactPoiType.idHash;
		}
		gameObject.AddOrGet<InfoDescription>().description = Strings.Get(def.descStringKey);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.requiredDlcIds = def.poiType.GetRequiredDlcIds();
		component.forbiddenDlcIds = def.poiType.GetForbiddenDlcIds();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	private List<HarvestablePOIConfig.HarvestablePOIParams> GenerateConfigs()
	{
		List<HarvestablePOIConfig.HarvestablePOIParams> list = new List<HarvestablePOIConfig.HarvestablePOIParams>();
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("cloud", new HarvestablePOIConfigurator.HarvestablePOIType("CarbonAsteroidField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.RefinedCarbon,
				1.5f
			},
			{
				SimHashes.Carbon,
				5.5f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Carbon,
				28070f
			},
			{
				SimHashes.RefinedCarbon,
				8320f
			}
		}, 30000f, 45000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("metallic_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("MetallicAsteroidField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.MoltenIron,
				1.25f
			},
			{
				SimHashes.Cuprite,
				1.75f
			},
			{
				SimHashes.Obsidian,
				6.25f
			},
			{
				SimHashes.MoltenLead,
				1.75f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.MoltenIron,
				14240f
			},
			{
				SimHashes.Cuprite,
				12280f
			}
		}, 54000f, 81000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("satellite_field", new HarvestablePOIConfigurator.HarvestablePOIType("SatelliteField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Sand,
				3f
			},
			{
				SimHashes.IronOre,
				3f
			},
			{
				SimHashes.MoltenCopper,
				2.67f
			},
			{
				SimHashes.Glass,
				1.33f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.IronOre,
				17240f
			},
			{
				SimHashes.MoltenCopper,
				25050f
			},
			{
				SimHashes.Glass,
				16240f
			}
		}, 30000f, 45000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("rocky_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("RockyAsteroidField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Katairite,
				2f
			},
			{
				SimHashes.Granite,
				2f
			},
			{
				SimHashes.SedimentaryRock,
				3f
			},
			{
				SimHashes.IgneousRock,
				3f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.SedimentaryRock,
				23840f
			},
			{
				SimHashes.Granite,
				28020f
			}
		}, 81000f, 94000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("interstellar_ice_field", new HarvestablePOIConfigurator.HarvestablePOIType("InterstellarIceField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Ice,
				2.5f
			},
			{
				SimHashes.SolidCarbonDioxide,
				7f
			},
			{
				SimHashes.SolidOxygen,
				0.5f
			}
		}, 50, new Dictionary<SimHashes, float> { 
		{
			SimHashes.Ice,
			38840f
		} }, 54000f, 81000f, 30000f, 60000f, true, new List<string>
		{
			Db.Get().OrbitalTypeCategories.iceCloud.Id,
			Db.Get().OrbitalTypeCategories.iceRock.Id
		}, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("organic_mass_field", new HarvestablePOIConfigurator.HarvestablePOIType("OrganicMassField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.SlimeMold,
				3f
			},
			{
				SimHashes.Algae,
				3f
			},
			{
				SimHashes.ContaminatedOxygen,
				1f
			},
			{
				SimHashes.Dirt,
				3f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Dirt,
				6140f
			},
			{
				SimHashes.Algae,
				14080f
			}
		}, 54000f, 81000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("ice_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("IceAsteroidField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Ice,
				6f
			},
			{
				SimHashes.SolidCarbonDioxide,
				2f
			},
			{
				SimHashes.Oxygen,
				1.5f
			},
			{
				SimHashes.SolidMethane,
				0.5f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Ice,
				33140f
			},
			{
				SimHashes.SolidMethane,
				16550f
			}
		}, 54000f, 81000f, 30000f, 60000f, true, new List<string>
		{
			Db.Get().OrbitalTypeCategories.iceCloud.Id,
			Db.Get().OrbitalTypeCategories.iceRock.Id
		}, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("gas_giant_cloud", new HarvestablePOIConfigurator.HarvestablePOIType("GasGiantCloud", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Methane,
				1f
			},
			{
				SimHashes.LiquidMethane,
				1f
			},
			{
				SimHashes.SolidMethane,
				1f
			},
			{
				SimHashes.Hydrogen,
				7f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Methane,
				32140f
			},
			{
				SimHashes.LiquidMethane,
				12550f
			},
			{
				SimHashes.Hydrogen,
				4080f
			}
		}, 15000f, 20000f, 30000f, 60000f, true, HarvestablePOIConfig.GasFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("chlorine_cloud", new HarvestablePOIConfigurator.HarvestablePOIType("ChlorineCloud", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Chlorine,
				2.5f
			},
			{
				SimHashes.BleachStone,
				7.5f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Chlorine,
				13030f
			},
			{
				SimHashes.BleachStone,
				26480f
			}
		}, 54000f, 81000f, 30000f, 60000f, true, HarvestablePOIConfig.GasFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("gilded_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("GildedAsteroidField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Gold,
				2.5f
			},
			{
				SimHashes.Fullerene,
				1f
			},
			{
				SimHashes.RefinedCarbon,
				1f
			},
			{
				SimHashes.SedimentaryRock,
				4.5f
			},
			{
				SimHashes.Regolith,
				1f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Gold,
				19030f
			},
			{
				SimHashes.Regolith,
				12010f
			}
		}, 30000f, 45000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("glimmering_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("GlimmeringAsteroidField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.MoltenTungsten,
				2f
			},
			{
				SimHashes.Wolframite,
				6f
			},
			{
				SimHashes.Carbon,
				1f
			},
			{
				SimHashes.CarbonDioxide,
				1f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.MoltenTungsten,
				12040f
			},
			{
				SimHashes.Wolframite,
				12800f
			}
		}, 30000f, 45000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("helium_cloud", new HarvestablePOIConfigurator.HarvestablePOIType("HeliumCloud", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Hydrogen,
				2f
			},
			{
				SimHashes.Water,
				8f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Hydrogen,
				14800f
			},
			{
				SimHashes.Water,
				21800f
			}
		}, 30000f, 45000f, 30000f, 60000f, true, HarvestablePOIConfig.GasFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("oily_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("OilyAsteroidField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.SolidCarbonDioxide,
				7.75f
			},
			{
				SimHashes.SolidMethane,
				1.125f
			},
			{
				SimHashes.CrudeOil,
				1.125f
			}
		}, 50, new Dictionary<SimHashes, float> { 
		{
			SimHashes.CrudeOil,
			28780f
		} }, 15000f, 25000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("oxidized_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("OxidizedAsteroidField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Rust,
				8f
			},
			{
				SimHashes.SolidCarbonDioxide,
				2f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Rust,
				25080f
			},
			{
				SimHashes.SolidCarbonDioxide,
				12080f
			}
		}, 54000f, 81000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("salty_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("SaltyAsteroidField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.SaltWater,
				5f
			},
			{
				SimHashes.Brine,
				4f
			},
			{
				SimHashes.SolidCarbonDioxide,
				1f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.SaltWater,
				13000f
			},
			{
				SimHashes.Brine,
				15080f
			}
		}, 54000f, 81000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("frozen_ore_field", new HarvestablePOIConfigurator.HarvestablePOIType("FrozenOreField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Ice,
				2.33f
			},
			{
				SimHashes.DirtyIce,
				2.33f
			},
			{
				SimHashes.Snow,
				1.83f
			},
			{
				SimHashes.AluminumOre,
				2f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Ice,
				12000f
			},
			{
				SimHashes.Snow,
				23080f
			}
		}, 54000f, 81000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("foresty_ore_field", new HarvestablePOIConfigurator.HarvestablePOIType("ForestyOreField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.IgneousRock,
				7f
			},
			{
				SimHashes.AluminumOre,
				1f
			},
			{
				SimHashes.CarbonDioxide,
				2f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.IgneousRock,
				10720f
			},
			{
				SimHashes.AluminumOre,
				9080f
			}
		}, 54000f, 81000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("swampy_ore_field", new HarvestablePOIConfigurator.HarvestablePOIType("SwampyOreField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Mud,
				2f
			},
			{
				SimHashes.ToxicSand,
				7f
			},
			{
				SimHashes.Cobaltite,
				1f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Mud,
				8290f
			},
			{
				SimHashes.Cobaltite,
				18820f
			}
		}, 54000f, 81000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("sandy_ore_field", new HarvestablePOIConfigurator.HarvestablePOIType("SandyOreField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.SandStone,
				4f
			},
			{
				SimHashes.Algae,
				2f
			},
			{
				SimHashes.Cuprite,
				1f
			},
			{
				SimHashes.Sand,
				3f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.SandStone,
				12080f
			},
			{
				SimHashes.Cuprite,
				11450f
			}
		}, 54000f, 81000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("radioactive_gas_cloud", new HarvestablePOIConfigurator.HarvestablePOIType("RadioactiveGasCloud", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.UraniumOre,
				2f
			},
			{
				SimHashes.Chlorine,
				2f
			},
			{
				SimHashes.CarbonDioxide,
				7f
			}
		}, 50, new Dictionary<SimHashes, float> { 
		{
			SimHashes.UraniumOre,
			10040f
		} }, 5000f, 10000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("radioactive_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("RadioactiveAsteroidField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.UraniumOre,
				2f
			},
			{
				SimHashes.Sulfur,
				3f
			},
			{
				SimHashes.BleachStone,
				2f
			},
			{
				SimHashes.Rust,
				4f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.UraniumOre,
				8040f
			},
			{
				SimHashes.Sulfur,
				4080f
			}
		}, 5000f, 10000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("oxygen_rich_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("OxygenRichAsteroidField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Water,
				4f
			},
			{
				SimHashes.ContaminatedOxygen,
				2f
			},
			{
				SimHashes.Ice,
				4f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Water,
				8040f
			},
			{
				SimHashes.ContaminatedOxygen,
				40080f
			}
		}, 15000f, 25000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("interstellar_ocean", new HarvestablePOIConfigurator.HarvestablePOIType("InterstellarOcean", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.SaltWater,
				2.5f
			},
			{
				SimHashes.Brine,
				2.5f
			},
			{
				SimHashes.Salt,
				2.5f
			},
			{
				SimHashes.Ice,
				2.5f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.SaltWater,
				14040f
			},
			{
				SimHashes.Brine,
				12080f
			}
		}, 15000f, 25000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1, null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("ceres_debris_field", new HarvestablePOIConfigurator.HarvestablePOIType("DLC2CeresField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Cinnabar,
				4.5f
			},
			{
				SimHashes.Mercury,
				2.5f
			},
			{
				SimHashes.Ice,
				2.5f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Cinnabar,
				50820f
			},
			{
				SimHashes.Mercury,
				48090f
			}
		}, 15000f, 25000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1.Append<string>(DlcManager.DLC2), null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("ceres_starting_field", new HarvestablePOIConfigurator.HarvestablePOIType("DLC2CeresOreField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Cinnabar,
				2.5f
			},
			{
				SimHashes.Mercury,
				2.5f
			},
			{
				SimHashes.Ice,
				3.5f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Cinnabar,
				13720f
			},
			{
				SimHashes.Ice,
				9080f
			}
		}, 15000f, 25000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1.Append<string>(DlcManager.DLC2), null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("prehistoric_SO1", new HarvestablePOIConfigurator.HarvestablePOIType("DLC4PrehistoricMixingField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.NickelOre,
				4.5f
			},
			{
				SimHashes.Peat,
				2.5f
			},
			{
				SimHashes.Shale,
				1f
			},
			{
				SimHashes.Amber,
				1f
			},
			{
				SimHashes.Iridium,
				0.5f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.NickelOre,
				50720f
			},
			{
				SimHashes.Peat,
				20880f
			},
			{
				SimHashes.Shale,
				20080f
			},
			{
				SimHashes.Amber,
				12080f
			},
			{
				SimHashes.Iridium,
				10809f
			}
		}, 15000f, 25000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1.Append<string>(DlcManager.DLC4), null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("prehistoric_SO2", new HarvestablePOIConfigurator.HarvestablePOIType("DLC4PrehistoricOreField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.NickelOre,
				1.5f
			},
			{
				SimHashes.Peat,
				2.5f
			},
			{
				SimHashes.Shale,
				1f
			},
			{
				SimHashes.Amber,
				1f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Amber,
				8720f
			},
			{
				SimHashes.Peat,
				9080f
			}
		}, 15000f, 25000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1.Append<string>(DlcManager.DLC4), null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("prehistoric_impactor_1", new HarvestablePOIConfigurator.HarvestablePOIType("DLC4ImpactorDebrisField1", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Iridium,
				1.7f
			},
			{
				SimHashes.MaficRock,
				2.3f
			},
			{
				SimHashes.Gold,
				2.1f
			},
			{
				SimHashes.Granite,
				3.9f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Iridium,
				10720f
			},
			{
				SimHashes.Gold,
				10080f
			}
		}, 35000f, 45000f, 30000f, 30000f, false, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1.Append<string>(DlcManager.DLC4), null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("prehistoric_impactor_2", new HarvestablePOIConfigurator.HarvestablePOIType("DLC4ImpactorDebrisField2", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Isoresin,
				1.8f
			},
			{
				SimHashes.Petroleum,
				3.5f
			},
			{
				SimHashes.LiquidSulfur,
				4.7f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.Isoresin,
				2280f
			},
			{
				SimHashes.Petroleum,
				11080f
			}
		}, 33400f, 66800f, 30000f, 30000f, false, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1.Append<string>(DlcManager.DLC4), null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("prehistoric_impactor_3", new HarvestablePOIConfigurator.HarvestablePOIType("DLC4ImpactorDebrisField3", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.MoltenIridium,
				3.7f
			},
			{
				SimHashes.LiquidOxygen,
				0.6f
			},
			{
				SimHashes.LiquidHydrogen,
				0.6f
			},
			{
				SimHashes.Magma,
				5.1f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.MoltenIridium,
				3280f
			},
			{
				SimHashes.Magma,
				1580f
			}
		}, 110000f, 137500f, 30000f, 30000f, false, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1.Append<string>(DlcManager.DLC4), null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("aquatic_mix", new HarvestablePOIConfigurator.HarvestablePOIType("DLC5AquaticMixingField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.ZincOre,
				4.5f
			},
			{
				SimHashes.SiltStone,
				2.5f
			},
			{
				SimHashes.Corallium,
				2.5f
			},
			{
				SimHashes.Basalt,
				1.5f
			},
			{
				SimHashes.Coquina,
				1.5f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.ZincOre,
				18720f
			},
			{
				SimHashes.SiltStone,
				20880f
			},
			{
				SimHashes.Corallium,
				20080f
			},
			{
				SimHashes.Basalt,
				19020f
			},
			{
				SimHashes.Coquina,
				18020f
			}
		}, 15000f, 25000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1.Append<string>(DlcManager.DLC5), null)));
		list.Add(new HarvestablePOIConfig.HarvestablePOIParams("aquatic_field", new HarvestablePOIConfigurator.HarvestablePOIType("DLC5AquaticOreField", new Dictionary<SimHashes, float>
		{
			{
				SimHashes.ZincOre,
				4.5f
			},
			{
				SimHashes.SiltStone,
				2.5f
			},
			{
				SimHashes.Corallium,
				2.5f
			},
			{
				SimHashes.Basalt,
				1.5f
			},
			{
				SimHashes.Coquina,
				1.5f
			}
		}, 50, new Dictionary<SimHashes, float>
		{
			{
				SimHashes.ZincOre,
				8720f
			},
			{
				SimHashes.SiltStone,
				9080f
			},
			{
				SimHashes.Corallium,
				14080f
			},
			{
				SimHashes.Basalt,
				14080f
			},
			{
				SimHashes.Coquina,
				15040f
			}
		}, 15000f, 25000f, 30000f, 60000f, true, HarvestablePOIConfig.AsteroidFieldOrbit, 20, DlcManager.EXPANSION1.Append<string>(DlcManager.DLC5), null)));
		list.RemoveAll((HarvestablePOIConfig.HarvestablePOIParams poi) => !DlcManager.IsCorrectDlcSubscribed(poi.poiType));
		return list;
	}

	public const int DEFAULT_INITIAL_DATABANK_COUNT = 50;

	public const string CarbonAsteroidField = "CarbonAsteroidField";

	public const string MetallicAsteroidField = "MetallicAsteroidField";

	public const string SatelliteField = "SatelliteField";

	public const string RockyAsteroidField = "RockyAsteroidField";

	public const string InterstellarIceField = "InterstellarIceField";

	public const string OrganicMassField = "OrganicMassField";

	public const string IceAsteroidField = "IceAsteroidField";

	public const string GasGiantCloud = "GasGiantCloud";

	public const string ChlorineCloud = "ChlorineCloud";

	public const string GildedAsteroidField = "GildedAsteroidField";

	public const string GlimmeringAsteroidField = "GlimmeringAsteroidField";

	public const string HeliumCloud = "HeliumCloud";

	public const string OilyAsteroidField = "OilyAsteroidField";

	public const string OxidizedAsteroidField = "OxidizedAsteroidField";

	public const string SaltyAsteroidField = "SaltyAsteroidField";

	public const string FrozenOreField = "FrozenOreField";

	public const string ForestyOreField = "ForestyOreField";

	public const string SwampyOreField = "SwampyOreField";

	public const string SandyOreField = "SandyOreField";

	public const string RadioactiveGasCloud = "RadioactiveGasCloud";

	public const string RadioactiveAsteroidField = "RadioactiveAsteroidField";

	public const string OxygenRichAsteroidField = "OxygenRichAsteroidField";

	public const string InterstellarOcean = "InterstellarOcean";

	public const string DLC2CeresField = "DLC2CeresField";

	public const string DLC2CeresOreField = "DLC2CeresOreField";

	public const string DLC4PrehistoricMixingField = "DLC4PrehistoricMixingField";

	public const string DLC4PrehistoricOreField = "DLC4PrehistoricOreField";

	public const string DLC4ImpactorDebrisField1 = "DLC4ImpactorDebrisField1";

	public const string DLC4ImpactorDebrisField2 = "DLC4ImpactorDebrisField2";

	public const string DLC4ImpactorDebrisField3 = "DLC4ImpactorDebrisField3";

	public const string DLC5AquaticOreField = "DLC5AquaticOreField";

	public const string DLC5AquaticMixingField = "DLC5AquaticMixingField";

	private static readonly List<string> GasFieldOrbit = new List<string>
	{
		Db.Get().OrbitalTypeCategories.iceCloud.Id,
		Db.Get().OrbitalTypeCategories.heliumCloud.Id,
		Db.Get().OrbitalTypeCategories.purpleGas.Id,
		Db.Get().OrbitalTypeCategories.radioactiveGas.Id
	};

	private static readonly List<string> AsteroidFieldOrbit = new List<string>
	{
		Db.Get().OrbitalTypeCategories.iceRock.Id,
		Db.Get().OrbitalTypeCategories.frozenOre.Id,
		Db.Get().OrbitalTypeCategories.rocky.Id
	};

	public struct HarvestablePOIParams
	{
		public HarvestablePOIParams(string anim, HarvestablePOIConfigurator.HarvestablePOIType poiType)
		{
			this.id = "HarvestableSpacePOI_" + poiType.id;
			this.anim = anim;
			this.nameStringKey = new StringKey("STRINGS.UI.SPACEDESTINATIONS.HARVESTABLE_POI." + poiType.id.ToUpper() + ".NAME");
			this.descStringKey = new StringKey("STRINGS.UI.SPACEDESTINATIONS.HARVESTABLE_POI." + poiType.id.ToUpper() + ".DESC");
			this.poiType = poiType;
		}

		public string id;

		public string anim;

		public StringKey nameStringKey;

		public StringKey descStringKey;

		public HarvestablePOIConfigurator.HarvestablePOIType poiType;
	}
}
