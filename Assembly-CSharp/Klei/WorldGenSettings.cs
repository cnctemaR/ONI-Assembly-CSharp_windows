using System;
using System.Collections.Generic;
using Klei.Noise;
using UnityEngine;

namespace Klei
{
	public class WorldGenSettings
	{
		public TerrainBiomeSettings biomes { get; private set; }

		public LevelLayerSettings layers { get; private set; }

		public TerrainFeatureSettings features { get; private set; }

		public SubWorlds subworlds { get; private set; }

		public Rivers rivers { get; private set; }

		public RoomDescriptions rooms { get; private set; }

		public Temperatures temperatures { get; private set; }

		public NoiseTreeFiles noise { get; private set; }

		public DefaultSettings defaults { get; private set; }

		public MobSettings mobs { get; private set; }

		public FeatureSettings GetFeature(string name)
		{
			if (this.featuresettings.ContainsKey(name))
			{
				return this.featuresettings[name];
			}
			return null;
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

		public void Save(string path)
		{
			this.biomes.Save(path + WorldGenSettings.BIOME_FILE + ".yaml");
			this.layers.Save(path + WorldGenSettings.LAYERS_FILE + ".yaml");
			this.features.Save(path + WorldGenSettings.FEATURES_FILE + ".yaml");
			this.rivers.Save(path + WorldGenSettings.RIVERS_FILE + ".yaml");
			this.rooms.Save(path + WorldGenSettings.ROOMS_FILE + ".yaml");
			this.temperatures.Save(path + WorldGenSettings.TEMPERATURES_FILE + ".yaml");
			this.subworlds.Save(path + WorldGenSettings.SUBWORLDS_FILE + ".yaml");
			this.noise.Save(path + WorldGenSettings.NOISE_FILE + ".yaml");
			this.defaults.Save(path + WorldGenSettings.DEFAULTS_FILE + ".yaml");
			this.mobs.Save(path + WorldGenSettings.MOBS_FILE + ".yaml");
		}

		public static WorldGenSettings LoadFile(string path)
		{
			WorldGenSettings worldGenSettings = new WorldGenSettings();
			worldGenSettings.biomes = YamlIO<TerrainBiomeSettings>.LoadFile(path + WorldGenSettings.BIOME_FILE + ".yaml");
			if (worldGenSettings.biomes.LoadBiomeFiles.Count == 0)
			{
				Debug.LogError("WorldGen: No biome lookup table files will be loaded");
			}
			else
			{
				for (int i = 0; i < worldGenSettings.biomes.LoadBiomeFiles.Count; i++)
				{
					string text = worldGenSettings.biomes.LoadBiomeFiles[i];
					BiomeSettings biomeSettings = YamlIO<BiomeSettings>.LoadFile(path + "/biomeTables/" + text + ".yaml");
					if (biomeSettings != null)
					{
						foreach (KeyValuePair<string, Biome> keyValuePair in biomeSettings.TerrainBiomeLookupTable)
						{
							if (!worldGenSettings.biomes.TerrainBiomeLookupTable.ContainsKey(keyValuePair.Key))
							{
								worldGenSettings.biomes.TerrainBiomeLookupTable.Add(keyValuePair.Key, keyValuePair.Value);
							}
						}
					}
					else
					{
						Debug.LogWarning("WorldGen: Attempting to load biome: " + text + " failed");
					}
				}
			}
			foreach (KeyValuePair<string, Biome> keyValuePair2 in worldGenSettings.biomes.TerrainBiomeLookupTable)
			{
				keyValuePair2.Value.ConvertBandSizeToMaxSize();
			}
			worldGenSettings.layers = YamlIO<LevelLayerSettings>.LoadFile(path + WorldGenSettings.LAYERS_FILE + ".yaml");
			worldGenSettings.layers.LevelLayers.ConvertBandSizeToMaxSize();
			worldGenSettings.features = YamlIO<TerrainFeatureSettings>.LoadFile(path + WorldGenSettings.FEATURES_FILE + ".yaml");
			worldGenSettings.rivers = YamlIO<Rivers>.LoadFile(path + WorldGenSettings.RIVERS_FILE + ".yaml");
			worldGenSettings.rooms = YamlIO<RoomDescriptions>.LoadFile(path + WorldGenSettings.ROOMS_FILE + ".yaml");
			worldGenSettings.temperatures = YamlIO<Temperatures>.LoadFile(path + WorldGenSettings.TEMPERATURES_FILE + ".yaml");
			worldGenSettings.subworlds = YamlIO<SubWorlds>.LoadFile(path + WorldGenSettings.SUBWORLDS_FILE + ".yaml");
			worldGenSettings.defaults = YamlIO<DefaultSettings>.LoadFile(path + WorldGenSettings.DEFAULTS_FILE + ".yaml");
			worldGenSettings.mobs = YamlIO<MobSettings>.LoadFile(path + WorldGenSettings.MOBS_FILE + ".yaml");
			worldGenSettings.noise = YamlIO<NoiseTreeFiles>.LoadFile(path + WorldGenSettings.NOISE_FILE + ".yaml");
			worldGenSettings.noise.LoadAllTrees();
			worldGenSettings.featuresettings = new Dictionary<string, FeatureSettings>();
			if (worldGenSettings.features.LoadFeatureFiles.Count == 0)
			{
				Debug.LogError("WorldGen: No feature files will be loaded");
			}
			else
			{
				for (int j = 0; j < worldGenSettings.features.LoadFeatureFiles.Count; j++)
				{
					string text2 = worldGenSettings.features.LoadFeatureFiles[j];
					FeatureSettings featureSettings = YamlIO<FeatureSettings>.LoadFile(path + "features/" + text2 + ".yaml");
					if (featureSettings != null)
					{
						worldGenSettings.featuresettings.Add(text2, featureSettings);
					}
					else
					{
						Debug.LogWarning("WorldGen: Attempting to load feature: " + text2 + " failed");
					}
				}
			}
			return worldGenSettings;
		}

		private Dictionary<string, FeatureSettings> featuresettings;

		private static string BIOME_FILE = "biomelookup";

		private static string LAYERS_FILE = "layers";

		private static string FEATURES_FILE = "features";

		private static string RIVERS_FILE = "rivers";

		private static string ROOMS_FILE = "rooms";

		private static string TEMPERATURES_FILE = "temperatures";

		private static string SUBWORLDS_FILE = "subworlds";

		public static string NOISE_FILE = "noise";

		private static string DEFAULTS_FILE = "defaults";

		private static string MOBS_FILE = "mobs";
	}
}
