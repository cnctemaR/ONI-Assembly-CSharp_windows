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
			this.Satellite = base.Add(new SpaceDestinationType(text, parent, text2, text3, num, text4, dictionary, null, artifactDropRate));
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
			}, new Dictionary<string, int> { { "HatchMetal", 3 } }, Db.Get().ArtifactDropRates.Mediocre));
			this.RockyAsteroid = base.Add(new SpaceDestinationType("RockyAsteroid", parent, UI.SPACEDESTINATIONS.ASTEROIDS.ROCKYASTEROID.NAME, UI.SPACEDESTINATIONS.ASTEROIDS.ROCKYASTEROID.DESCRIPTION, 32, "asteroid", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.IronOre,
					new MathUtil.MinMax(100f, 200f)
				},
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
			}, new Dictionary<string, int> { { "HatchHard", 3 } }, Db.Get().ArtifactDropRates.Good));
			text4 = "CarbonaceousAsteroid";
			text3 = UI.SPACEDESTINATIONS.ASTEROIDS.CARBONACEOUSASTEROID.NAME;
			text2 = UI.SPACEDESTINATIONS.ASTEROIDS.CARBONACEOUSASTEROID.DESCRIPTION;
			num = 32;
			text = "asteroid";
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
			this.CarbonaceousAsteroid = base.Add(new SpaceDestinationType(text4, parent, text3, text2, num, text, dictionary, null, artifactDropRate));
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
				},
				{
					SimHashes.SolidMethane,
					new MathUtil.MinMax(100f, 200f)
				}
			}, new Dictionary<string, int>
			{
				{ "ColdBreatherSeed", 3 },
				{ "ColdWheatSeed", 4 }
			}, Db.Get().ArtifactDropRates.Great));
			this.OrganicDwarf = base.Add(new SpaceDestinationType("OrganicDwarf", parent, UI.SPACEDESTINATIONS.DWARFPLANETS.ORGANICDWARF.NAME, UI.SPACEDESTINATIONS.DWARFPLANETS.ORGANICDWARF.DESCRIPTION, 64, "organicAsteroid", new Dictionary<SimHashes, MathUtil.MinMax>
			{
				{
					SimHashes.SlimeMold,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.DirtyWater,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Algae,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.CarbonDioxide,
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
			}, Db.Get().ArtifactDropRates.Great));
			text = "DustyMoon";
			text2 = UI.SPACEDESTINATIONS.DWARFPLANETS.DUSTYDWARF.NAME;
			text3 = UI.SPACEDESTINATIONS.DWARFPLANETS.DUSTYDWARF.DESCRIPTION;
			num = 64;
			text4 = "asteroid";
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
			this.DustyMoon = base.Add(new SpaceDestinationType(text, parent, text2, text3, num, text4, dictionary, null, artifactDropRate));
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
			}, Db.Get().ArtifactDropRates.Amazing));
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
					SimHashes.Obsidian,
					new MathUtil.MinMax(100f, 200f)
				},
				{
					SimHashes.Katairite,
					new MathUtil.MinMax(100f, 200f)
				}
			};
			artifactDropRate = Db.Get().ArtifactDropRates.Amazing;
			this.VolcanoPlanet = base.Add(new SpaceDestinationType(text4, parent, text3, text2, num, text, dictionary, null, artifactDropRate));
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
			this.GasGiant = base.Add(new SpaceDestinationType(text, parent, text2, text3, num, text4, dictionary, null, artifactDropRate));
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
			this.IceGiant = base.Add(new SpaceDestinationType(text4, parent, text3, text2, num, text, dictionary, null, artifactDropRate));
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
