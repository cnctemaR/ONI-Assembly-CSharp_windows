using System;
using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class SpaceDestinationTypes : ResourceSet<SpaceDestinationType>
	{
		public SpaceDestinationTypes(ResourceSet parent)
			: base("SpaceDestinations", parent)
		{
			string text = "Satellite";
			string text2 = UI.SPACEDESTINATIONS.DEBRIS.SATELLITE.NAME;
			string text3 = UI.SPACEDESTINATIONS.DEBRIS.SATELLITE.DESCRIPTION;
			int num = 16;
			string text4 = "asteroid";
			Dictionary<SimHashes, MathUtil.MinMax> dictionary = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Steel,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Copper,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Glass,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			ArtifactDropRate artifactDropRate = Db.Get().ArtifactDropRates.Bad;
			this.Satellite = base.Add(new SpaceDestinationType(text, parent, text2, text3, num, text4, dictionary, null, artifactDropRate, 64000000, 63994000, 18, true));
			this.MetallicAsteroid = base.Add(new SpaceDestinationType("MetallicAsteroid", parent, UI.SPACEDESTINATIONS.ASTEROIDS.METALLICASTEROID.NAME, UI.SPACEDESTINATIONS.ASTEROIDS.METALLICASTEROID.DESCRIPTION, 32, "nebula", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Iron,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Copper,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Obsidian,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int> { { "HatchMetal", 3 } }, Db.Get().ArtifactDropRates.Mediocre, 128000000, 127988000, 12, true));
			this.RockyAsteroid = base.Add(new SpaceDestinationType("RockyAsteroid", parent, UI.SPACEDESTINATIONS.ASTEROIDS.ROCKYASTEROID.NAME, UI.SPACEDESTINATIONS.ASTEROIDS.ROCKYASTEROID.DESCRIPTION, 32, "new_12", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Cuprite,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SedimentaryRock,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.IgneousRock,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int> { { "HatchHard", 3 } }, Db.Get().ArtifactDropRates.Good, 128000000, 127988000, 18, true));
			text4 = "CarbonaceousAsteroid";
			text3 = UI.SPACEDESTINATIONS.ASTEROIDS.CARBONACEOUSASTEROID.NAME;
			text2 = UI.SPACEDESTINATIONS.ASTEROIDS.CARBONACEOUSASTEROID.DESCRIPTION;
			num = 32;
			text = "new_08";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.RefinedCarbon,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Carbon,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Diamond,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			artifactDropRate = Db.Get().ArtifactDropRates.Mediocre;
			this.CarbonaceousAsteroid = base.Add(new SpaceDestinationType(text4, parent, text3, text2, num, text, dictionary, null, artifactDropRate, 128000000, 127988000, 6, true));
			this.IcyDwarf = base.Add(new SpaceDestinationType("IcyDwarf", parent, UI.SPACEDESTINATIONS.DWARFPLANETS.ICYDWARF.NAME, UI.SPACEDESTINATIONS.DWARFPLANETS.ICYDWARF.DESCRIPTION, 64, "icyMoon", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Ice,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidCarbonDioxide,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidOxygen,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int>
			{
				{ "ColdBreatherSeed", 3 },
				{ "ColdWheatSeed", 4 }
			}, Db.Get().ArtifactDropRates.Great, 256000000, 255982000, 24, true));
			this.OrganicDwarf = base.Add(new SpaceDestinationType("OrganicDwarf", parent, UI.SPACEDESTINATIONS.DWARFPLANETS.ORGANICDWARF.NAME, UI.SPACEDESTINATIONS.DWARFPLANETS.ORGANICDWARF.DESCRIPTION, 64, "organicAsteroid", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.SlimeMold,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Algae,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.ContaminatedOxygen,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int>
			{
				{ "Moo", 1 },
				{ "GasGrassSeed", 4 }
			}, Db.Get().ArtifactDropRates.Great, 256000000, 255982000, 30, true));
			text = "DustyMoon";
			text2 = UI.SPACEDESTINATIONS.DWARFPLANETS.DUSTYDWARF.NAME;
			text3 = UI.SPACEDESTINATIONS.DWARFPLANETS.DUSTYDWARF.DESCRIPTION;
			num = 64;
			text4 = "new_05";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Regolith,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.MaficRock,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SedimentaryRock,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			artifactDropRate = Db.Get().ArtifactDropRates.Amazing;
			this.DustyMoon = base.Add(new SpaceDestinationType(text, parent, text2, text3, num, text4, dictionary, null, artifactDropRate, 256000000, 255982000, 42, true));
			this.TerraPlanet = base.Add(new SpaceDestinationType("TerraPlanet", parent, UI.SPACEDESTINATIONS.PLANETS.TERRAPLANET.NAME, UI.SPACEDESTINATIONS.PLANETS.TERRAPLANET.DESCRIPTION, 96, "terra", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Water,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Algae,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Oxygen,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Dirt,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int>
			{
				{ "PrickleFlowerSeed", 4 },
				{ "PacuEgg", 4 }
			}, Db.Get().ArtifactDropRates.Amazing, 384000000, 383980000, 54, true));
			text4 = "VolcanoPlanet";
			text3 = UI.SPACEDESTINATIONS.PLANETS.VOLCANOPLANET.NAME;
			text2 = UI.SPACEDESTINATIONS.PLANETS.VOLCANOPLANET.DESCRIPTION;
			num = 96;
			text = "planet";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Magma,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.IgneousRock,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Katairite,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			artifactDropRate = Db.Get().ArtifactDropRates.Amazing;
			this.VolcanoPlanet = base.Add(new SpaceDestinationType(text4, parent, text3, text2, num, text, dictionary, null, artifactDropRate, 384000000, 383980000, 54, true));
			text = "GasGiant";
			text2 = UI.SPACEDESTINATIONS.GIANTS.GASGIANT.NAME;
			text3 = UI.SPACEDESTINATIONS.GIANTS.GASGIANT.DESCRIPTION;
			num = 96;
			text4 = "gasGiant";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Methane,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Hydrogen,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			artifactDropRate = Db.Get().ArtifactDropRates.Perfect;
			this.GasGiant = base.Add(new SpaceDestinationType(text, parent, text2, text3, num, text4, dictionary, null, artifactDropRate, 384000000, 383980000, 60, true));
			text4 = "IceGiant";
			text3 = UI.SPACEDESTINATIONS.GIANTS.ICEGIANT.NAME;
			text2 = UI.SPACEDESTINATIONS.GIANTS.ICEGIANT.DESCRIPTION;
			num = 96;
			text = "icyMoon";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Ice,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidCarbonDioxide,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidOxygen,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidMethane,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			artifactDropRate = Db.Get().ArtifactDropRates.Perfect;
			this.IceGiant = base.Add(new SpaceDestinationType(text4, parent, text3, text2, num, text, dictionary, null, artifactDropRate, 384000000, 383980000, 60, true));
			this.SaltDwarf = base.Add(new SpaceDestinationType("SaltDwarf", parent, UI.SPACEDESTINATIONS.DWARFPLANETS.SALTDWARF.NAME, UI.SPACEDESTINATIONS.DWARFPLANETS.SALTDWARF.DESCRIPTION, 64, "new_01", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.SaltWater,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidCarbonDioxide,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Brine,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int> { { "SaltPlantSeed", 3 } }, Db.Get().ArtifactDropRates.Bad, 256000000, 255982000, 30, true));
			text = "RustPlanet";
			text2 = UI.SPACEDESTINATIONS.PLANETS.RUSTPLANET.NAME;
			text3 = UI.SPACEDESTINATIONS.PLANETS.RUSTPLANET.DESCRIPTION;
			num = 96;
			text4 = "new_06";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Rust,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidCarbonDioxide,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			artifactDropRate = Db.Get().ArtifactDropRates.Perfect;
			this.RustPlanet = base.Add(new SpaceDestinationType(text, parent, text2, text3, num, text4, dictionary, null, artifactDropRate, 384000000, 383980000, 60, true));
			this.ForestPlanet = base.Add(new SpaceDestinationType("ForestPlanet", parent, UI.SPACEDESTINATIONS.PLANETS.FORESTPLANET.NAME, UI.SPACEDESTINATIONS.PLANETS.FORESTPLANET.DESCRIPTION, 96, "new_07", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.AluminumOre,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidOxygen,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int>
			{
				{ "Squirrel", 1 },
				{ "ForestTreeSeed", 4 }
			}, Db.Get().ArtifactDropRates.Mediocre, 384000000, 383980000, 24, true));
			text4 = "RedDwarf";
			text3 = UI.SPACEDESTINATIONS.DWARFPLANETS.REDDWARF.NAME;
			text2 = UI.SPACEDESTINATIONS.DWARFPLANETS.REDDWARF.DESCRIPTION;
			num = 64;
			text = "sun";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Aluminum,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.LiquidMethane,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Fossil,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			artifactDropRate = Db.Get().ArtifactDropRates.Amazing;
			this.RedDwarf = base.Add(new SpaceDestinationType(text4, parent, text3, text2, num, text, dictionary, null, artifactDropRate, 256000000, 255982000, 42, true));
			text = "GoldAsteroid";
			text2 = UI.SPACEDESTINATIONS.ASTEROIDS.GOLDASTEROID.NAME;
			text3 = UI.SPACEDESTINATIONS.ASTEROIDS.GOLDASTEROID.DESCRIPTION;
			num = 32;
			text4 = "new_02";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Gold,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Fullerene,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.FoolsGold,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			artifactDropRate = Db.Get().ArtifactDropRates.Bad;
			this.GoldAsteroid = base.Add(new SpaceDestinationType(text, parent, text2, text3, num, text4, dictionary, null, artifactDropRate, 128000000, 127988000, 90, true));
			text4 = "HeliumGiant";
			text3 = UI.SPACEDESTINATIONS.GIANTS.HYDROGENGIANT.NAME;
			text2 = UI.SPACEDESTINATIONS.GIANTS.HYDROGENGIANT.DESCRIPTION;
			num = 96;
			text = "new_11";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.LiquidHydrogen,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Water,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Niobium,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			artifactDropRate = Db.Get().ArtifactDropRates.Mediocre;
			this.HydrogenGiant = base.Add(new SpaceDestinationType(text4, parent, text3, text2, num, text, dictionary, null, artifactDropRate, 384000000, 383980000, 78, true));
			text = "OilyAsteriod";
			text2 = UI.SPACEDESTINATIONS.ASTEROIDS.OILYASTEROID.NAME;
			text3 = UI.SPACEDESTINATIONS.ASTEROIDS.OILYASTEROID.DESCRIPTION;
			num = 32;
			text4 = "new_09";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.SolidMethane,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.SolidCarbonDioxide,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.CrudeOil,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Petroleum,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			artifactDropRate = Db.Get().ArtifactDropRates.Mediocre;
			this.OilyAsteroid = base.Add(new SpaceDestinationType(text, parent, text2, text3, num, text4, dictionary, null, artifactDropRate, 128000000, 127988000, 12, true));
			text4 = "ShinyPlanet";
			text3 = UI.SPACEDESTINATIONS.PLANETS.SHINYPLANET.NAME;
			text2 = UI.SPACEDESTINATIONS.PLANETS.SHINYPLANET.DESCRIPTION;
			num = 96;
			text = "new_04";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Tungsten,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Wolframite,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			artifactDropRate = Db.Get().ArtifactDropRates.Good;
			this.ShinyPlanet = base.Add(new SpaceDestinationType(text4, parent, text3, text2, num, text, dictionary, null, artifactDropRate, 384000000, 383980000, 84, true));
			text = "ChlorinePlanet";
			text2 = UI.SPACEDESTINATIONS.PLANETS.CHLORINEPLANET.NAME;
			text3 = UI.SPACEDESTINATIONS.PLANETS.CHLORINEPLANET.DESCRIPTION;
			num = 96;
			text4 = "new_10";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.SolidChlorine,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.BleachStone,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			artifactDropRate = Db.Get().ArtifactDropRates.Bad;
			this.ChlorinePlanet = base.Add(new SpaceDestinationType(text, parent, text2, text3, num, text4, dictionary, null, artifactDropRate, 256000000, 255982000, 90, true));
			this.SaltDesertPlanet = base.Add(new SpaceDestinationType("SaltDesertPlanet", parent, UI.SPACEDESTINATIONS.PLANETS.SALTDESERTPLANET.NAME, UI.SPACEDESTINATIONS.PLANETS.SALTDESERTPLANET.DESCRIPTION, 96, "new_10", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.Salt,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.CrushedRock,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int> { { "Crab", 1 } }, Db.Get().ArtifactDropRates.Bad, 384000000, 383980000, 60, true));
			text4 = "Wormhole";
			text3 = UI.SPACEDESTINATIONS.WORMHOLE.NAME;
			text2 = UI.SPACEDESTINATIONS.WORMHOLE.DESCRIPTION;
			num = 96;
			text = "new_03";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax> { 
			{
				SimHashes.Vacuum,
				new MathUtil.MinMax(100f, 200f)
			} };
			artifactDropRate = Db.Get().ArtifactDropRates.Perfect;
			this.Wormhole = base.Add(new SpaceDestinationType(text4, parent, text3, text2, num, text, dictionary, null, artifactDropRate, 0, 0, 0, true));
			text = "Earth";
			text2 = UI.SPACEDESTINATIONS.PLANETS.SHATTEREDPLANET.NAME;
			text3 = UI.SPACEDESTINATIONS.PLANETS.SHATTEREDPLANET.DESCRIPTION;
			num = 96;
			text4 = "earth";
			dictionary = new Dictionary<SimHashes, MathUtil.MinMax>();
			artifactDropRate = Db.Get().ArtifactDropRates.None;
			this.Earth = base.Add(new SpaceDestinationType(text, parent, text2, text3, num, text4, dictionary, null, artifactDropRate, 0, 0, 0, false));
		}

		public SpaceDestinationType Satellite;

		public SpaceDestinationType MetallicAsteroid;

		public SpaceDestinationType RockyAsteroid;

		public SpaceDestinationType CarbonaceousAsteroid;

		public SpaceDestinationType IcyDwarf;

		public SpaceDestinationType OrganicDwarf;

		public SpaceDestinationType DustyMoon;

		public SpaceDestinationType TerraPlanet;

		public SpaceDestinationType VolcanoPlanet;

		public SpaceDestinationType GasGiant;

		public SpaceDestinationType IceGiant;

		public SpaceDestinationType Wormhole;

		public SpaceDestinationType SaltDwarf;

		public SpaceDestinationType RustPlanet;

		public SpaceDestinationType ForestPlanet;

		public SpaceDestinationType RedDwarf;

		public SpaceDestinationType GoldAsteroid;

		public SpaceDestinationType HydrogenGiant;

		public SpaceDestinationType OilyAsteroid;

		public SpaceDestinationType ShinyPlanet;

		public SpaceDestinationType ChlorinePlanet;

		public SpaceDestinationType SaltDesertPlanet;

		public SpaceDestinationType Earth;

		public static Dictionary<SimHashes, MathUtil.MinMax> extendedElementTable = new Dictionary<SimHashes, MathUtil.MinMax>
		{
			{
				SimHashes.Niobium,
				new MathUtil.MinMax(10f, 20f)
			},
			{
				SimHashes.Katairite,
				new MathUtil.MinMax(50f, 100f)
			},
			{
				SimHashes.Isoresin,
				new MathUtil.MinMax(30f, 60f)
			},
			{
				SimHashes.Fullerene,
				new MathUtil.MinMax(0.5f, 1f)
			}
		};
	}
}
