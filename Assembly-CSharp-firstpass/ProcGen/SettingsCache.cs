using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using ObjectCloner;
using ProcGen.Noise;
using UnityEngine;

namespace ProcGen
{
	public static class SettingsCache
	{
		public static LevelLayerSettings layers { get; private set; }

		public static ComposableDictionary<string, River> rivers { get; private set; }

		public static ComposableDictionary<string, Room> rooms { get; private set; }

		public static ComposableDictionary<Temperature.Range, Temperature> temperatures { get; private set; }

		public static ComposableDictionary<string, List<WeightedSimHash>> borders { get; private set; }

		public static DefaultSettings defaults { get; set; }

		public static MobSettings mobs { get; private set; }

		public static string GetPath()
		{
			if (SettingsCache.path == null)
			{
				SettingsCache.path = FileSystem.Normalize(Path.Combine(Application.streamingAssetsPath, "worldgen/"));
			}
			return SettingsCache.path;
		}

		public static void CloneInToNewWorld(MutatedWorldData worldData)
		{
			worldData.subworlds = SerializingCloner.Copy<Dictionary<string, SubWorld>>(SettingsCache.subworlds);
			worldData.features = SerializingCloner.Copy<Dictionary<string, FeatureSettings>>(SettingsCache.featuresettings);
			worldData.biomes = SerializingCloner.Copy<TerrainElementBandSettings>(SettingsCache.biomes);
			worldData.mobs = SerializingCloner.Copy<MobSettings>(SettingsCache.mobs);
		}

		public static List<string> GetCachedFeatureNames()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, FeatureSettings> keyValuePair in SettingsCache.featuresettings)
			{
				list.Add(keyValuePair.Key);
			}
			return list;
		}

		public static FeatureSettings GetCachedFeature(string name)
		{
			if (SettingsCache.featuresettings.ContainsKey(name))
			{
				return SettingsCache.featuresettings[name];
			}
			throw new Exception("Couldnt get feature from cache [" + name + "]");
		}

		public static List<string> GetCachedTraitNames()
		{
			return new List<string>(SettingsCache.traits.Keys);
		}

		public static WorldTrait GetCachedTrait(string name)
		{
			if (SettingsCache.traits.ContainsKey(name))
			{
				return SettingsCache.traits[name];
			}
			throw new Exception("Couldnt get trait [" + name + "]");
		}

		public static SubWorld GetCachedSubWorld(string name)
		{
			if (SettingsCache.subworlds.ContainsKey(name))
			{
				return SettingsCache.subworlds[name];
			}
			throw new Exception("Couldnt get subworld [" + name + "]");
		}

		private static bool GetPathAndName(string srcPath, string srcName, out string name)
		{
			if (FileSystem.FileExists(srcPath + srcName + ".yaml"))
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
			if (FileSystem.FileExists(srcPath + name + ".yaml"))
			{
				return true;
			}
			name = srcName;
			return false;
		}

		private static void LoadBiome(string longName, List<YamlIO.Error> errors)
		{
			string empty = string.Empty;
			if (!SettingsCache.GetPathAndName(SettingsCache.GetPath(), longName, out empty))
			{
				return;
			}
			if (SettingsCache.biomeSettingsCache.ContainsKey(empty))
			{
				return;
			}
			BiomeSettings biomeSettings = SettingsCache.MergeLoad<BiomeSettings>(SettingsCache.GetPath() + empty + ".yaml", errors);
			if (biomeSettings == null)
			{
				global::Debug.LogWarning("WorldGen: Attempting to load biome: " + empty + " failed");
				return;
			}
			global::Debug.Assert(biomeSettings.TerrainBiomeLookupTable.Count > 0, longName);
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

		private static string LoadFeature(string longName, List<YamlIO.Error> errors)
		{
			string empty = string.Empty;
			if (!SettingsCache.GetPathAndName(SettingsCache.GetPath(), longName, out empty))
			{
				global::Debug.LogWarning("LoadFeature GetPathAndName: Attempting to load feature: " + empty + " failed");
				return longName;
			}
			if (!SettingsCache.featuresettings.ContainsKey(empty))
			{
				FeatureSettings featureSettings = YamlIO.LoadFile<FeatureSettings>(SettingsCache.GetPath() + empty + ".yaml", null, null);
				if (featureSettings != null)
				{
					SettingsCache.featuresettings.Add(empty, featureSettings);
					if (featureSettings.forceBiome != null)
					{
						SettingsCache.LoadBiome(featureSettings.forceBiome, errors);
						DebugUtil.Assert(SettingsCache.biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(featureSettings.forceBiome), longName, "(feature) referenced a missing biome named", featureSettings.forceBiome);
					}
				}
				else
				{
					global::Debug.LogWarning("WorldGen: Attempting to load feature: " + empty + " failed");
				}
			}
			return empty;
		}

		public static void LoadFeatures(Dictionary<string, int> features, List<YamlIO.Error> errors)
		{
			foreach (KeyValuePair<string, int> keyValuePair in features)
			{
				SettingsCache.LoadFeature(keyValuePair.Key, errors);
			}
		}

		public static void LoadSubworlds(List<WeightedName> subworlds, List<YamlIO.Error> errors)
		{
			foreach (WeightedName weightedName in subworlds)
			{
				SubWorld subWorld = null;
				string text = weightedName.name;
				if (weightedName.overrideName != null && weightedName.overrideName.Length > 0)
				{
					text = weightedName.overrideName;
				}
				if (!SettingsCache.subworlds.ContainsKey(text))
				{
					SubWorld subWorld2 = YamlIO.LoadFile<SubWorld>(SettingsCache.path + weightedName.name + ".yaml", null, null);
					if (subWorld2 != null)
					{
						subWorld = subWorld2;
						subWorld.name = text;
						SettingsCache.subworlds[text] = subWorld;
						SettingsCache.noise.LoadTree(subWorld.biomeNoise, SettingsCache.path);
						SettingsCache.noise.LoadTree(subWorld.densityNoise, SettingsCache.path);
						SettingsCache.noise.LoadTree(subWorld.overrideNoise, SettingsCache.path);
					}
					else
					{
						global::Debug.LogWarning("WorldGen: Attempting to load subworld: " + weightedName.name + " failed");
					}
					if (subWorld.centralFeature != null)
					{
						subWorld.centralFeature.type = SettingsCache.LoadFeature(subWorld.centralFeature.type, errors);
					}
					foreach (WeightedBiome weightedBiome in subWorld.biomes)
					{
						SettingsCache.LoadBiome(weightedBiome.name, errors);
						DebugUtil.Assert(SettingsCache.biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(weightedBiome.name), subWorld.name, "(subworld) referenced a missing biome named", weightedBiome.name);
					}
					DebugUtil.Assert(subWorld.features != null, "Features list for subworld", subWorld.name, "was null! Either remove it from the .yaml or set it to the empty list []");
					foreach (Feature feature in subWorld.features)
					{
						feature.type = SettingsCache.LoadFeature(feature.type, errors);
					}
				}
			}
		}

		public static List<string> GetWorldNames()
		{
			return SettingsCache.worlds.GetNames();
		}

		public static void Save(string path)
		{
			YamlIO.Save<LevelLayerSettings>(SettingsCache.layers, path + "layers.yaml", null);
			YamlIO.Save<ComposableDictionary<string, River>>(SettingsCache.rivers, path + "rivers.yaml", null);
			YamlIO.Save<ComposableDictionary<string, Room>>(SettingsCache.rooms, path + "rooms.yaml", null);
			YamlIO.Save<ComposableDictionary<Temperature.Range, Temperature>>(SettingsCache.temperatures, path + "temperatures.yaml", null);
			YamlIO.Save<ComposableDictionary<string, List<WeightedSimHash>>>(SettingsCache.borders, path + "borders.yaml", null);
			YamlIO.Save<DefaultSettings>(SettingsCache.defaults, path + "defaults.yaml", null);
			YamlIO.Save<MobSettings>(SettingsCache.mobs, path + "mobs.yaml", null);
		}

		public static void Clear()
		{
			SettingsCache.worlds.worldCache.Clear();
			SettingsCache.layers = null;
			SettingsCache.biomes.BiomeBackgroundElementBandConfigurations.Clear();
			SettingsCache.biomeSettingsCache.Clear();
			SettingsCache.rivers = null;
			SettingsCache.rooms = null;
			SettingsCache.temperatures = null;
			SettingsCache.borders = null;
			SettingsCache.noise.Clear();
			SettingsCache.defaults = null;
			SettingsCache.mobs = null;
			SettingsCache.featuresettings.Clear();
			SettingsCache.traits.Clear();
			SettingsCache.subworlds.Clear();
			DebugUtil.LogArgs(new object[] { "World Settings cleared!" });
		}

		private static T MergeLoad<T>(string filename, List<YamlIO.Error> errors) where T : class, IMerge<T>, new()
		{
			ListPool<FileHandle, WorldGenSettings>.PooledList pooledList = ListPool<FileHandle, WorldGenSettings>.Allocate();
			FileSystem.GetFiles(filename, pooledList);
			if (pooledList.Count == 0)
			{
				pooledList.Recycle();
				throw new Exception(string.Format("File not found in any file system: {0}", filename));
			}
			pooledList.Reverse();
			ListPool<T, WorldGenSettings>.PooledList pooledList2 = ListPool<T, WorldGenSettings>.Allocate();
			pooledList2.Add(new T());
			using (List<FileHandle>.Enumerator enumerator = pooledList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FileHandle file = enumerator.Current;
					T t = YamlIO.Parse<T>(FileSystem.ConvertToText(file.source.ReadBytes(file.full_path)), file.full_path, delegate(YamlIO.Error error, bool force_log_as_warning)
					{
						error.file = file;
						errors.Add(error);
					}, null);
					if (t != null)
					{
						pooledList2.Add(t);
					}
				}
			}
			pooledList.Recycle();
			T t2 = pooledList2[0];
			for (int num = 1; num != pooledList2.Count; num++)
			{
				t2.Merge(pooledList2[num]);
			}
			pooledList2.Recycle();
			return t2;
		}

		private static int FirstUncommonCharacter(string a, string b)
		{
			int num = Mathf.Min(a.Length, b.Length);
			int num2 = -1;
			while (++num2 < num)
			{
				if (a[num2] != b[num2])
				{
					return num2;
				}
			}
			return num2;
		}

		public static bool LoadFiles(List<YamlIO.Error> errors)
		{
			if (SettingsCache.worlds.worldCache.Count > 0)
			{
				return false;
			}
			SettingsCache.worlds.LoadFiles(SettingsCache.GetPath(), errors);
			List<FileHandle> list = new List<FileHandle>();
			FileSystem.GetFiles(FileSystem.Normalize(Path.Combine(SettingsCache.path, "traits")), "*.yaml", list);
			using (List<FileHandle>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FileHandle trait_file = enumerator.Current;
					WorldTrait worldTrait = YamlIO.LoadFile<WorldTrait>(trait_file.full_path, delegate(YamlIO.Error error, bool force_log_as_warning)
					{
						error.file = trait_file;
						errors.Add(error);
					}, null);
					int num = SettingsCache.FirstUncommonCharacter(SettingsCache.path, trait_file.full_path);
					string text = ((num <= -1) ? trait_file.full_path : trait_file.full_path.Substring(num));
					text = Path.Combine(Path.GetDirectoryName(text), Path.GetFileNameWithoutExtension(text));
					if (worldTrait == null)
					{
						DebugUtil.LogWarningArgs(new object[] { "Failed to load trait: ", text });
					}
					else
					{
						SettingsCache.traits[text] = worldTrait;
						worldTrait.filePath = text;
					}
				}
			}
			foreach (KeyValuePair<string, World> keyValuePair in SettingsCache.worlds.worldCache)
			{
				SettingsCache.LoadFeatures(keyValuePair.Value.globalFeatures, errors);
				SettingsCache.LoadSubworlds(keyValuePair.Value.subworldFiles, errors);
			}
			foreach (KeyValuePair<string, WorldTrait> keyValuePair2 in SettingsCache.traits)
			{
				SettingsCache.LoadFeatures(keyValuePair2.Value.globalFeatureMods, errors);
				SettingsCache.LoadSubworlds(keyValuePair2.Value.additionalSubworldFiles, errors);
			}
			SettingsCache.layers = SettingsCache.MergeLoad<LevelLayerSettings>(SettingsCache.GetPath() + "layers.yaml", errors);
			SettingsCache.layers.LevelLayers.ConvertBandSizeToMaxSize();
			SettingsCache.rivers = SettingsCache.MergeLoad<ComposableDictionary<string, River>>(SettingsCache.GetPath() + "rivers.yaml", errors);
			SettingsCache.rooms = SettingsCache.MergeLoad<ComposableDictionary<string, Room>>(SettingsCache.path + "rooms.yaml", errors);
			foreach (KeyValuePair<string, Room> keyValuePair3 in SettingsCache.rooms)
			{
				keyValuePair3.Value.name = keyValuePair3.Key;
			}
			SettingsCache.temperatures = SettingsCache.MergeLoad<ComposableDictionary<Temperature.Range, Temperature>>(SettingsCache.GetPath() + "temperatures.yaml", errors);
			SettingsCache.borders = SettingsCache.MergeLoad<ComposableDictionary<string, List<WeightedSimHash>>>(SettingsCache.GetPath() + "borders.yaml", errors);
			SettingsCache.defaults = YamlIO.LoadFile<DefaultSettings>(SettingsCache.GetPath() + "defaults.yaml", null, null);
			SettingsCache.mobs = SettingsCache.MergeLoad<MobSettings>(SettingsCache.GetPath() + "mobs.yaml", errors);
			foreach (KeyValuePair<string, Mob> keyValuePair4 in SettingsCache.mobs.MobLookupTable)
			{
				keyValuePair4.Value.name = keyValuePair4.Key;
			}
			DebugUtil.LogArgs(new object[] { "World settings reload complete!" });
			return true;
		}

		public static List<string> GetRandomTraits(int seed)
		{
			global::System.Random random = new global::System.Random(seed);
			int num = random.Next(2, 5);
			List<string> list = new List<string>(SettingsCache.traits.Keys);
			list.Sort();
			List<string> list2 = new List<string>();
			while (list2.Count < num && list.Count > 0)
			{
				int num2 = random.Next(list.Count);
				string text = list[num2];
				bool flag = false;
				foreach (string text2 in SettingsCache.GetCachedTrait(text).exclusiveWith)
				{
					if (list2.Contains(text2))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list2.Add(text);
				}
				list.RemoveAt(num2);
			}
			return list2;
		}

		public static TerrainElementBandSettings biomes = new TerrainElementBandSettings();

		public static Worlds worlds = new Worlds();

		public static NoiseTreeFiles noise = new NoiseTreeFiles();

		private static Dictionary<string, FeatureSettings> featuresettings = new Dictionary<string, FeatureSettings>();

		private static Dictionary<string, WorldTrait> traits = new Dictionary<string, WorldTrait>();

		public static Dictionary<string, SubWorld> subworlds = new Dictionary<string, SubWorld>();

		private static string path = null;

		private static Dictionary<string, BiomeSettings> biomeSettingsCache = new Dictionary<string, BiomeSettings>();

		private const string LAYERS_FILE = "layers";

		private const string RIVERS_FILE = "rivers";

		private const string ROOMS_FILE = "rooms";

		private const string TEMPERATURES_FILE = "temperatures";

		private const string BORDERS_FILE = "borders";

		private const string DEFAULTS_FILE = "defaults";

		private const string MOBS_FILE = "mobs";

		private const string TRAITS_PATH = "traits";
	}
}
