using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using ProcGen.Noise;
using UnityEngine;

namespace ProcGen
{
	public static class SettingsCache
	{
		public static LevelLayerSettings layers { get; private set; }

		public static TerrainFeatureSettings features { get; private set; }

		public static Rivers rivers { get; private set; }

		public static RoomDescriptions rooms { get; private set; }

		public static Temperatures temperatures { get; private set; }

		public static DefaultSettings defaults { get; set; }

		public static MobSettings mobs { get; private set; }

		public static string GetPath()
		{
			if (SettingsCache.path == null)
			{
				SettingsCache.path = FSUtil.Normalize(Path.Combine(Application.streamingAssetsPath, "worldgen/"));
			}
			return SettingsCache.path;
		}

		public static string GetDefaultBiome(string name)
		{
			if (SettingsCache.features.TerrainFeatures.ContainsKey(name))
			{
				return SettingsCache.features.TerrainFeatures[name].defaultBiome.type;
			}
			global::Debug.LogError("Couldn't get default biome [" + name + "]", null);
			return null;
		}

		public static FeatureSettings GetFeature(string name)
		{
			if (name == "features/Sedimentary/StartLocation")
			{
				int num = 0;
				num++;
			}
			if (!name.StartsWith("features/"))
			{
				return null;
			}
			if (SettingsCache.featuresettings.ContainsKey(name))
			{
				return SettingsCache.featuresettings[name];
			}
			throw new Exception("Couldnt get feature [" + name + "]");
		}

		public static string[] GetFeatureSettingsNames()
		{
			string[] array = new string[SettingsCache.featuresettings.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, FeatureSettings> keyValuePair in SettingsCache.featuresettings)
			{
				array[num++] = keyValuePair.Key;
			}
			return array;
		}

		private static bool GetPathAndName(string srcPath, string srcName, out string name)
		{
			if (File.Exists(srcPath + srcName + ".yaml"))
			{
				name = srcName;
				return true;
			}
			string[] array = srcName.Split(new char[] { '/' });
			name = array[0];
			for (int i = 1; i < array.Length - 1; i++)
			{
				name = name + "/" + array[i];
			}
			if (File.Exists(srcPath + name + ".yaml"))
			{
				return true;
			}
			name = srcName;
			return false;
		}

		private static void LoadBiome(string longName)
		{
			string empty = string.Empty;
			if (!SettingsCache.GetPathAndName(SettingsCache.GetPath(), longName, out empty))
			{
				return;
			}
			if (!SettingsCache.biomeSettingsCache.ContainsKey(empty))
			{
				BiomeSettings biomeSettings = YamlIO<BiomeSettings>.LoadFile(SettingsCache.GetPath() + empty + ".yaml", null);
				if (biomeSettings != null)
				{
					SettingsCache.biomeSettingsCache.Add(empty, biomeSettings);
					foreach (KeyValuePair<string, ElementBandConfiguration> keyValuePair in biomeSettings.TerrainBiomeLookupTable)
					{
						string text = empty + "/" + keyValuePair.Key;
						if (!SettingsCache.biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(text))
						{
							SettingsCache.biomes.BiomeBackgroundElementBandConfigurations.Add(text, keyValuePair.Value);
						}
					}
				}
				else
				{
					global::Debug.LogWarning("WorldGen: Attempting to load biome: " + empty + " failed", null);
				}
			}
		}

		private static string LoadFeature(string longName)
		{
			string empty = string.Empty;
			if (!SettingsCache.GetPathAndName(SettingsCache.GetPath(), longName, out empty))
			{
				global::Debug.LogWarning("LoadFeature GetPathAndName: Attempting to load feature: " + empty + " failed", null);
				return longName;
			}
			if (!SettingsCache.featuresettings.ContainsKey(empty))
			{
				FeatureSettings featureSettings = YamlIO<FeatureSettings>.LoadFile(SettingsCache.GetPath() + empty + ".yaml", null);
				if (featureSettings != null)
				{
					SettingsCache.featuresettings.Add(empty, featureSettings);
				}
				else
				{
					global::Debug.LogWarning("WorldGen: Attempting to load feature: " + empty + " failed", null);
				}
			}
			return empty;
		}

		public static void LoadZoneContents(IEnumerable<SubWorld> zones)
		{
			foreach (SubWorld subWorld in zones)
			{
				if (subWorld.centralFeature != null)
				{
					subWorld.centralFeature.type = SettingsCache.LoadFeature(subWorld.centralFeature.type);
				}
				foreach (WeightedBiome weightedBiome in subWorld.biomes)
				{
					SettingsCache.LoadBiome(weightedBiome.name);
				}
				foreach (Feature feature in subWorld.features)
				{
					feature.type = SettingsCache.LoadFeature(feature.type);
				}
			}
		}

		public static List<string> GetWorldNames()
		{
			return SettingsCache.worlds.GetNames();
		}

		public static Dictionary<string, Worlds.Data> GetAllWorldData()
		{
			return SettingsCache.worlds.worldCache;
		}

		public static void Save(string path)
		{
			SettingsCache.layers.Save(path + "layers.yaml", null);
			SettingsCache.features.Save(path + "features.yaml", null);
			SettingsCache.rivers.Save(path + "rivers.yaml", null);
			SettingsCache.rooms.Save(path + "rooms.yaml", null);
			SettingsCache.temperatures.Save(path + "temperatures.yaml", null);
			SettingsCache.defaults.Save(path + "defaults.yaml", null);
			SettingsCache.mobs.Save(path + "mobs.yaml", null);
		}

		public static void Clear()
		{
			SettingsCache.worlds.worldCache.Clear();
			SettingsCache.layers = null;
			SettingsCache.features = null;
			SettingsCache.biomes.BiomeBackgroundElementBandConfigurations.Clear();
			SettingsCache.biomeSettingsCache.Clear();
			SettingsCache.rivers = null;
			SettingsCache.rooms = null;
			SettingsCache.temperatures = null;
			SettingsCache.noise.tree_files.Clear();
			SettingsCache.defaults = null;
			SettingsCache.mobs = null;
			SettingsCache.featuresettings.Clear();
		}

		public static bool LoadFiles(IFileSystem filesystem)
		{
			if (SettingsCache.worlds.worldCache.Count > 0)
			{
				return false;
			}
			SettingsCache.worlds.LoadFiles(SettingsCache.GetPath(), filesystem);
			foreach (KeyValuePair<string, Worlds.Data> keyValuePair in SettingsCache.worlds.worldCache)
			{
				keyValuePair.Value.world.LoadZones(SettingsCache.noise, SettingsCache.GetPath());
				SettingsCache.LoadZoneContents(keyValuePair.Value.world.Zones.Values);
			}
			SettingsCache.layers = YamlIO<LevelLayerSettings>.LoadFile(SettingsCache.GetPath() + "layers.yaml", null);
			SettingsCache.layers.LevelLayers.ConvertBandSizeToMaxSize();
			SettingsCache.features = YamlIO<TerrainFeatureSettings>.LoadFile(SettingsCache.GetPath() + "features.yaml", null);
			foreach (KeyValuePair<string, TerrainFeature> keyValuePair2 in SettingsCache.features.TerrainFeatures)
			{
				keyValuePair2.Value.name = keyValuePair2.Key;
				if (keyValuePair2.Value.defaultBiome != null && keyValuePair2.Value.defaultBiome.type != null)
				{
					SettingsCache.LoadBiome(keyValuePair2.Value.defaultBiome.type);
				}
			}
			SettingsCache.rivers = YamlIO<Rivers>.LoadFile(SettingsCache.GetPath() + "rivers.yaml", null);
			SettingsCache.rooms = YamlIO<RoomDescriptions>.LoadFile(SettingsCache.path + "rooms.yaml", null);
			foreach (KeyValuePair<string, Room> keyValuePair3 in SettingsCache.rooms.rooms)
			{
				keyValuePair3.Value.name = keyValuePair3.Key;
			}
			SettingsCache.temperatures = YamlIO<Temperatures>.LoadFile(SettingsCache.GetPath() + "temperatures.yaml", null);
			SettingsCache.defaults = YamlIO<DefaultSettings>.LoadFile(SettingsCache.GetPath() + "defaults.yaml", null);
			SettingsCache.mobs = YamlIO<MobSettings>.LoadFile(SettingsCache.GetPath() + "mobs.yaml", null);
			foreach (KeyValuePair<string, Mob> keyValuePair4 in SettingsCache.mobs.MobLookupTable)
			{
				keyValuePair4.Value.name = keyValuePair4.Key;
			}
			foreach (KeyValuePair<string, ElementBandConfiguration> keyValuePair5 in SettingsCache.biomes.BiomeBackgroundElementBandConfigurations)
			{
				keyValuePair5.Value.ConvertBandSizeToMaxSize();
			}
			return true;
		}

		public static TerrainElementBandSettings biomes = new TerrainElementBandSettings();

		public static Worlds worlds = new Worlds();

		public static NoiseTreeFiles noise = new NoiseTreeFiles();

		private static Dictionary<string, FeatureSettings> featuresettings = new Dictionary<string, FeatureSettings>();

		private static string path = null;

		private static Dictionary<string, BiomeSettings> biomeSettingsCache = new Dictionary<string, BiomeSettings>();

		private const string LAYERS_FILE = "layers";

		private const string FEATURES_FILE = "features";

		private const string RIVERS_FILE = "rivers";

		private const string ROOMS_FILE = "rooms";

		private const string TEMPERATURES_FILE = "temperatures";

		private const string DEFAULTS_FILE = "defaults";

		private const string MOBS_FILE = "mobs";
	}
}
