using System;
using System.Collections.Generic;
using Klei;
using ProcGen.Noise;

namespace ProcGen
{
	public class World : YamlIO<World>
	{
		public World()
		{
			this.Zones = new Dictionary<string, SubWorld>();
			this.ZoneFiles = new List<WeightedName>();
			this.DefineTagSet = new Dictionary<string, List<string>>();
			this.UnknownCellsAllowedSubworlds = new List<World.AllowedCellsFilter>();
		}

		public string name { get; private set; }

		public string description { get; private set; }

		public bool show { get; private set; }

		public Vector2I worldsize { get; private set; }

		public DefaultSettings defaultsOverrides { get; private set; }

		public World.LayoutMethod layoutMethod { get; private set; }

		public List<WeightedName> ZoneFiles { get; private set; }

		public Dictionary<string, List<string>> DefineTagSet { get; private set; }

		public List<World.AllowedCellsFilter> UnknownCellsAllowedSubworlds { get; private set; }

		public SubWorld GetSubWorld(string name)
		{
			if (this.Zones.ContainsKey(name))
			{
				return this.Zones[name];
			}
			return null;
		}

		public void LoadZones(NoiseTreeFiles noise, string path)
		{
			foreach (WeightedName weightedName in this.ZoneFiles)
			{
				string text = WorldGenSettings.GetSimpleName(weightedName.name);
				if (weightedName.overrideName != null && weightedName.overrideName.Length > 0)
				{
					text = weightedName.overrideName;
				}
				if (!this.Zones.ContainsKey(text))
				{
					SubWorldFile subWorldFile = YamlIO<SubWorldFile>.LoadFile(path + weightedName.name + ".yaml", null);
					if (subWorldFile != null)
					{
						SubWorld zone = subWorldFile.zone;
						zone.name = text;
						zone.pdWeight = weightedName.weight;
						this.Zones[text] = zone;
						noise.LoadTree(zone.biomeNoise, path);
						noise.LoadTree(zone.densityNoise, path);
						noise.LoadTree(zone.overrideNoise, path);
					}
					else
					{
						Debug.LogWarning("WorldGen: Attempting to load zone: " + weightedName.name + " failed", null);
					}
				}
			}
		}

		public Dictionary<string, SubWorld> Zones;

		public enum LayoutMethod
		{
			Default,
			VoronoiTree = 0,
			PowerTree
		}

		public class AllowedCellsFilter
		{
			public AllowedCellsFilter()
			{
				this.temperatureRanges = new List<Temperature.Range>();
				this.zoneTypes = new List<SubWorld.ZoneType>();
				this.subworldNames = new List<string>();
			}

			public World.AllowedCellsFilter.TagCommand tagcommand { get; private set; }

			public string tagset { get; private set; }

			public int distance { get; private set; }

			public int maxDistance { get; private set; }

			public int distCmp { get; private set; }

			public World.AllowedCellsFilter.Command command { get; private set; }

			public List<Temperature.Range> temperatureRanges { get; private set; }

			public List<SubWorld.ZoneType> zoneTypes { get; private set; }

			public List<string> subworldNames { get; private set; }

			public enum TagCommand
			{
				Default,
				ContainsOne,
				ContainsAll,
				ContainsNone,
				DistanceFrom
			}

			public enum Command
			{
				Clear,
				Replace,
				UnionWith,
				IntersectWith,
				ExceptWith,
				SymmetricExceptWith
			}
		}
	}
}
