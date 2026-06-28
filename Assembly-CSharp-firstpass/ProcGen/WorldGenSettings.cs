using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using ProcGen.Noise;

namespace ProcGen
{
	public class WorldGenSettings
	{
		public WorldGenSettings()
		{
			this.noise = new NoiseTreeFiles();
			this.worlds = new Worlds();
			this.biomes = new TerrainElementBandSettings();
		}

		public TerrainElementBandSettings biomes { get; private set; }

		public LevelLayerSettings layers { get; private set; }

		public TerrainFeatureSettings features { get; private set; }

		public Worlds worlds { get; private set; }

		public Rivers rivers { get; private set; }

		public RoomDescriptions rooms { get; private set; }

		public Temperatures temperatures { get; private set; }

		public NoiseTreeFiles noise { get; private set; }

		public DefaultSettings defaults { get; private set; }

		public MobSettings mobs { get; private set; }

		public string GetDefaultBiome(string name)
		{
			if (this.features.TerrainFeatures.ContainsKey(name))
			{
				return this.features.TerrainFeatures[name].defaultBiome.type;
			}
			Debug.LogError("Couldnt get default biome [" + name + "]", null);
			return null;
		}

		public FeatureSettings GetFeature(string name)
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
			if (this.featuresettings.ContainsKey(name))
			{
				return this.featuresettings[name];
			}
			throw new Exception("Couldnt get feature [" + name + "]");
		}

		public string[] GetFeatureSettingsNames()
		{
			string[] array = new string[this.featuresettings.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, FeatureSettings> keyValuePair in this.featuresettings)
			{
				array[num++] = keyValuePair.Key;
			}
			return array;
		}

		public float GetDefaultFloat(string target)
		{
			object obj = this.defaults.data[target];
			if (obj.GetType() == typeof(float))
			{
				return (float)obj;
			}
			float num = float.Parse(obj as string);
			this.defaults.data[target] = num;
			return num;
		}

		public int GetDefaultInt(string target)
		{
			object obj = this.defaults.data[target];
			if (obj.GetType() == typeof(int))
			{
				return (int)obj;
			}
			int num = int.Parse(obj as string);
			this.defaults.data[target] = num;
			return num;
		}

		public List<SubWorld> GetSubWorldList()
		{
			return new List<SubWorld>(this.world.Zones.Values);
		}

		public Dictionary<string, SubWorld> GetSubWorlds()
		{
			return this.world.Zones;
		}

		public SubWorld GetSubWorld(string name)
		{
			return this.world.GetSubWorld(name);
		}

		private bool GetPathAndName(string srcPath, string srcName, out string name)
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

		private void LoadBiome(string longName)
		{
			string empty = string.Empty;
			if (!this.GetPathAndName(this.base_path, longName, out empty))
			{
				return;
			}
			if (!WorldGenSettings.biomeSettingsCache.ContainsKey(empty))
			{
				BiomeSettings biomeSettings = YamlIO<BiomeSettings>.LoadFile(this.base_path + empty + ".yaml");
				if (biomeSettings != null)
				{
					WorldGenSettings.biomeSettingsCache.Add(empty, biomeSettings);
					foreach (KeyValuePair<string, ElementBandConfiguration> keyValuePair in biomeSettings.TerrainBiomeLookupTable)
					{
						string text = empty + "/" + keyValuePair.Key;
						if (!this.biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(text))
						{
							this.biomes.BiomeBackgroundElementBandConfigurations.Add(text, keyValuePair.Value);
						}
					}
				}
				else
				{
					Debug.LogWarning("WorldGen: Attempting to load biome: " + empty + " failed", null);
				}
			}
		}

		public static string GetSimpleName(string longName)
		{
			string[] array = longName.Split(new char[] { '/' });
			return array[array.Length - 1];
		}

		private string LoadFeature(string longName)
		{
			string empty = string.Empty;
			if (!this.GetPathAndName(this.base_path, longName, out empty))
			{
				Debug.LogWarning("LoadFeature GetPathAndName: Attempting to load feature: " + empty + " failed", null);
				return longName;
			}
			if (!this.featuresettings.ContainsKey(empty))
			{
				FeatureSettings featureSettings = YamlIO<FeatureSettings>.LoadFile(this.base_path + empty + ".yaml");
				if (featureSettings != null)
				{
					this.featuresettings.Add(empty, featureSettings);
				}
				else
				{
					Debug.LogWarning("WorldGen: Attempting to load feature: " + empty + " failed", null);
				}
			}
			return empty;
		}

		public void SetDefaultWorld(string path)
		{
			this.SetWorld("worlds/Default", path);
		}

		public void SetWorld(string name, string path)
		{
			this.world = this.worlds.GetWorld(name);
			if (this.world != null)
			{
				Debug.Log("Set world to [" + name + "] " + path, null);
				WorldGenSettings.biomeSettingsCache.Clear();
				this.base_path = path;
				this.world.LoadZones(this.noise, path);
				foreach (KeyValuePair<string, SubWorld> keyValuePair in this.world.Zones)
				{
					if (keyValuePair.Value.centralFeature != null)
					{
						keyValuePair.Value.centralFeature.type = this.LoadFeature(keyValuePair.Value.centralFeature.type);
					}
					foreach (WeightedBiome weightedBiome in keyValuePair.Value.biomes)
					{
						this.LoadBiome(weightedBiome.name);
					}
					foreach (Feature feature in keyValuePair.Value.features)
					{
						feature.type = this.LoadFeature(feature.type);
					}
				}
				foreach (KeyValuePair<string, TerrainFeature> keyValuePair2 in this.features.TerrainFeatures)
				{
					if (keyValuePair2.Value.defaultBiome != null && keyValuePair2.Value.defaultBiome.type != null)
					{
						this.LoadBiome(keyValuePair2.Value.defaultBiome.type);
					}
				}
				foreach (KeyValuePair<string, ElementBandConfiguration> keyValuePair3 in this.biomes.BiomeBackgroundElementBandConfigurations)
				{
					keyValuePair3.Value.ConvertBandSizeToMaxSize();
				}
			}
		}

		public List<string> GetWorldNames()
		{
			return this.worlds.GetNames();
		}

		public Dictionary<string, World> GetWorlds()
		{
			return this.worlds.worldCache;
		}

		public World GetWorld()
		{
			return this.world;
		}

		public void Save(string path)
		{
			this.layers.Save(path + WorldGenSettings.LAYERS_FILE + ".yaml");
			this.features.Save(path + WorldGenSettings.FEATURES_FILE + ".yaml");
			this.rivers.Save(path + WorldGenSettings.RIVERS_FILE + ".yaml");
			this.rooms.Save(path + WorldGenSettings.ROOMS_FILE + ".yaml");
			this.temperatures.Save(path + WorldGenSettings.TEMPERATURES_FILE + ".yaml");
			this.defaults.Save(path + WorldGenSettings.DEFAULTS_FILE + ".yaml");
			this.mobs.Save(path + WorldGenSettings.MOBS_FILE + ".yaml");
		}

		public static WorldGenSettings LoadFile(string path)
		{
			WorldGenSettings worldGenSettings = new WorldGenSettings();
			worldGenSettings.worlds.LoadFiles(path);
			worldGenSettings.layers = YamlIO<LevelLayerSettings>.LoadFile(path + WorldGenSettings.LAYERS_FILE + ".yaml");
			worldGenSettings.layers.LevelLayers.ConvertBandSizeToMaxSize();
			worldGenSettings.features = YamlIO<TerrainFeatureSettings>.LoadFile(path + WorldGenSettings.FEATURES_FILE + ".yaml");
			foreach (KeyValuePair<string, TerrainFeature> keyValuePair in worldGenSettings.features.TerrainFeatures)
			{
				keyValuePair.Value.name = keyValuePair.Key;
			}
			worldGenSettings.rivers = YamlIO<Rivers>.LoadFile(path + WorldGenSettings.RIVERS_FILE + ".yaml");
			worldGenSettings.rooms = YamlIO<RoomDescriptions>.LoadFile(path + WorldGenSettings.ROOMS_FILE + ".yaml");
			foreach (KeyValuePair<string, Room> keyValuePair2 in worldGenSettings.rooms.rooms)
			{
				keyValuePair2.Value.name = keyValuePair2.Key;
			}
			worldGenSettings.temperatures = YamlIO<Temperatures>.LoadFile(path + WorldGenSettings.TEMPERATURES_FILE + ".yaml");
			worldGenSettings.defaults = YamlIO<DefaultSettings>.LoadFile(path + WorldGenSettings.DEFAULTS_FILE + ".yaml");
			worldGenSettings.mobs = YamlIO<MobSettings>.LoadFile(path + WorldGenSettings.MOBS_FILE + ".yaml");
			foreach (KeyValuePair<string, Mob> keyValuePair3 in worldGenSettings.mobs.MobLookupTable)
			{
				keyValuePair3.Value.name = keyValuePair3.Key;
			}
			return worldGenSettings;
		}

		private World world;

		private Dictionary<string, FeatureSettings> featuresettings = new Dictionary<string, FeatureSettings>();

		private static Dictionary<string, BiomeSettings> biomeSettingsCache = new Dictionary<string, BiomeSettings>();

		private string base_path;

		private static string LAYERS_FILE = "layers";

		private static string FEATURES_FILE = "features";

		private static string RIVERS_FILE = "rivers";

		private static string ROOMS_FILE = "rooms";

		private static string TEMPERATURES_FILE = "temperatures";

		private static string DEFAULTS_FILE = "defaults";

		private static string MOBS_FILE = "mobs";
	}
}
