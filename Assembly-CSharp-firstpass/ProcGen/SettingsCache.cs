using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

		public static string GetAbsoluteContentPath(string dlcId, string optionalSubpath = "")
		{
			string text;
			if (!SettingsCache.s_cachedPaths.TryGetValue(dlcId, out text))
			{
				if (dlcId == "")
				{
					text = FileSystem.Normalize(Path.Combine(new string[] { Application.streamingAssetsPath }));
				}
				else
				{
					string contentDirectoryName = DlcManager.GetContentDirectoryName(dlcId);
					text = FileSystem.Normalize(Path.Combine(Application.streamingAssetsPath, "dlc", contentDirectoryName));
				}
				SettingsCache.s_cachedPaths[dlcId] = text;
			}
			return FileSystem.Normalize(Path.Combine(text, optionalSubpath));
		}

		public static string RewriteWorldgenPath(string scopePath)
		{
			string text;
			string text2;
			SettingsCache.GetDlcIdAndPath(scopePath, out text, out text2);
			return SettingsCache.GetAbsoluteContentPath(text, "worldgen/" + text2);
		}

		public static string RewriteWorldgenPathYaml(string scopePath)
		{
			return SettingsCache.RewriteWorldgenPath(scopePath) + ".yaml";
		}

		public static string GetScope(string dlcId)
		{
			if (dlcId == "")
			{
				return "";
			}
			return DlcManager.GetContentDirectoryName(dlcId) + "::";
		}

		public static void GetDlcIdAndPath(string scopePath, out string dlcId, out string path)
		{
			string[] array = scopePath.Split(SettingsCache.s_sourceDelimiter, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 1 && scopePath.EndsWith("::"))
			{
				dlcId = DlcManager.GetDlcIdFromContentDirectory(array[0]);
				path = "";
				return;
			}
			if (array.Length > 1)
			{
				dlcId = DlcManager.GetDlcIdFromContentDirectory(array[0]);
				path = array[1];
				return;
			}
			dlcId = "";
			path = scopePath;
		}

		public static string GuessScopedPath(string path)
		{
			foreach (string text in DlcManager.RELEASE_ORDER)
			{
				if (DlcManager.IsContentActive(text))
				{
					string absoluteContentPath = SettingsCache.GetAbsoluteContentPath(text, "worldgen/");
					if (path.StartsWith(absoluteContentPath))
					{
						return SettingsCache.GetScope(text) + path.Substring(absoluteContentPath.Length);
					}
				}
			}
			return null;
		}

		public static void CloneInToNewWorld(MutatedWorldData worldData)
		{
			worldData.subworlds = SerializingCloner.Copy<Dictionary<string, SubWorld>>(SettingsCache.subworlds);
			worldData.features = SerializingCloner.Copy<Dictionary<string, FeatureSettings>>(SettingsCache.featureSettings);
			worldData.biomes = SerializingCloner.Copy<TerrainElementBandSettings>(SettingsCache.biomes);
			worldData.mobs = SerializingCloner.Copy<MobSettings>(SettingsCache.mobs);
		}

		public static List<string> GetCachedFeatureNames()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, FeatureSettings> keyValuePair in SettingsCache.featureSettings)
			{
				list.Add(keyValuePair.Key);
			}
			return list;
		}

		public static FeatureSettings GetCachedFeature(string name)
		{
			if (SettingsCache.featureSettings.ContainsKey(name))
			{
				return SettingsCache.featureSettings[name];
			}
			throw new Exception("Couldnt get feature from cache [" + name + "]");
		}

		public static List<string> GetCachedWorldTraitNames()
		{
			return new List<string>(SettingsCache.worldTraits.Keys);
		}

		public static List<WorldTrait> GetCachedWorldTraits()
		{
			return new List<WorldTrait>(SettingsCache.worldTraits.Values);
		}

		public static WorldTrait GetCachedWorldTrait(string name, bool assertMissingTrait)
		{
			if (SettingsCache.worldTraits.ContainsKey(name))
			{
				return SettingsCache.worldTraits[name];
			}
			if (assertMissingTrait)
			{
				throw new Exception("Couldn't get trait [" + name + "]");
			}
			global::Debug.LogWarning("Couldn't get trait [" + name + "]");
			return null;
		}

		public static List<string> GetCachedStoryTraitNames()
		{
			return new List<string>(SettingsCache.storyTraits.Keys);
		}

		public static List<WorldTrait> GetCachedStoryTraits()
		{
			return new List<WorldTrait>(SettingsCache.storyTraits.Values);
		}

		public static Dictionary<string, WorldTrait> GetCachedStoryTraitsDictionary()
		{
			return SettingsCache.storyTraits;
		}

		public static WorldTrait GetCachedStoryTrait(string name, bool assertMissingTrait)
		{
			if (SettingsCache.storyTraits.ContainsKey(name))
			{
				return SettingsCache.storyTraits[name];
			}
			if (assertMissingTrait)
			{
				throw new Exception("Couldn't get story trait [" + name + "]");
			}
			global::Debug.LogWarning("Couldn't get story trait [" + name + "]");
			return null;
		}

		public static SubWorld GetCachedSubWorld(string name)
		{
			if (SettingsCache.subworlds.ContainsKey(name))
			{
				return SettingsCache.subworlds[name];
			}
			throw new Exception("Couldnt get subworld [" + name + "]");
		}

		private static void SplitNameFromPath(string scopePath, out string path, out string name)
		{
			int num = scopePath.LastIndexOf('/');
			name = scopePath.Substring(num + 1);
			path = scopePath.Substring(0, num);
		}

		private static bool LoadBiome(string longName, List<YamlIO.Error> errors)
		{
			string text;
			string text2;
			SettingsCache.SplitNameFromPath(longName, out text, out text2);
			if (SettingsCache.biomeSettingsCache.ContainsKey(text))
			{
				return true;
			}
			string text3 = SettingsCache.RewriteWorldgenPathYaml(text);
			BiomeSettings biomeSettings = SettingsCache.MergeLoad<BiomeSettings>(null, text3, errors);
			if (biomeSettings == null)
			{
				global::Debug.LogWarning("WorldGen: Attempting to load biome: " + text2 + " failed");
				return false;
			}
			global::Debug.Assert(biomeSettings.TerrainBiomeLookupTable.Count > 0, "Worldgen: TerrainBiomeLookupTable is empty: " + longName);
			SettingsCache.biomeSettingsCache.Add(text, biomeSettings);
			foreach (KeyValuePair<string, ElementBandConfiguration> keyValuePair in biomeSettings.TerrainBiomeLookupTable)
			{
				string text4 = text + "/" + keyValuePair.Key;
				if (!SettingsCache.biomes.BiomeBackgroundElementBandConfigurations.ContainsKey(text4))
				{
					SettingsCache.biomes.BiomeBackgroundElementBandConfigurations.Add(text4, keyValuePair.Value);
				}
			}
			return true;
		}

		private static string LoadFeature(string longName, List<YamlIO.Error> errors)
		{
			if (SettingsCache.featureSettings.ContainsKey(longName))
			{
				return longName;
			}
			FeatureSettings featureSettings = YamlIO.LoadFile<FeatureSettings>(SettingsCache.RewriteWorldgenPathYaml(longName), null, null);
			if (featureSettings != null)
			{
				SettingsCache.featureSettings.Add(longName, featureSettings);
				if (featureSettings.forceBiome != null)
				{
					DebugUtil.Assert(SettingsCache.LoadBiome(featureSettings.forceBiome, errors), longName, "(feature) referenced a missing biome named", featureSettings.forceBiome);
				}
			}
			else
			{
				global::Debug.LogWarning("WorldGen: Attempting to load feature: " + longName + " failed");
			}
			return longName;
		}

		public static void LoadFeatures(Dictionary<string, int> features, List<YamlIO.Error> errors)
		{
			foreach (KeyValuePair<string, int> keyValuePair in features)
			{
				SettingsCache.LoadFeature(keyValuePair.Key, errors);
			}
		}

		public static void LoadSubworlds(List<WeightedSubworldName> subworlds, string prefix, List<YamlIO.Error> errors)
		{
			foreach (WeightedSubworldName weightedSubworldName in subworlds)
			{
				SubWorld subWorld = null;
				string text = weightedSubworldName.name;
				if (weightedSubworldName.overrideName != null && weightedSubworldName.overrideName.Length > 0)
				{
					text = weightedSubworldName.overrideName;
				}
				SubWorld subWorld2 = YamlIO.LoadFile<SubWorld>(SettingsCache.RewriteWorldgenPathYaml(text), null, null);
				if (subWorld2 != null)
				{
					subWorld = subWorld2;
					subWorld.name = text;
					subWorld.EnforceTemplateSpawnRuleSelfConsistency();
					SettingsCache.subworlds[text] = subWorld;
					SettingsCache.noise.LoadTree(subWorld.biomeNoise);
					SettingsCache.noise.LoadTree(subWorld.densityNoise);
					SettingsCache.noise.LoadTree(subWorld.overrideNoise);
				}
				else
				{
					global::Debug.LogWarning("WorldGen: Attempting to load subworld: " + text + " failed");
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

		public static void LoadWorldTraits(string path, string prefix, List<YamlIO.Error> errors)
		{
			List<FileHandle> list = new List<FileHandle>();
			FileSystem.GetFiles(FileSystem.Normalize(Path.Combine(path, "traits")), "*.yaml", list);
			list.Sort((FileHandle s1, FileHandle s2) => string.Compare(s1.full_path, s2.full_path, StringComparison.OrdinalIgnoreCase));
			foreach (FileHandle fileHandle in list)
			{
				SettingsCache.LoadTrait(fileHandle, path, prefix, SettingsCache.worldTraits, errors);
			}
		}

		public static void LoadStoryTraits(string path, string prefix, List<YamlIO.Error> errors)
		{
			List<FileHandle> list = new List<FileHandle>();
			FileSystem.GetFiles(FileSystem.Normalize(Path.Combine(path, "storytraits")), "*.yaml", list);
			list.Sort((FileHandle s1, FileHandle s2) => string.Compare(s1.full_path, s2.full_path, StringComparison.OrdinalIgnoreCase));
			foreach (FileHandle fileHandle in list)
			{
				SettingsCache.LoadTrait(fileHandle, path, prefix, SettingsCache.storyTraits, errors);
			}
		}

		public static void LoadTrait(FileHandle file, string path, string prefix, Dictionary<string, WorldTrait> traitsDict, List<YamlIO.Error> errors)
		{
			WorldTrait worldTrait = YamlIO.LoadFile<WorldTrait>(file, delegate(YamlIO.Error error, bool force_log_as_warning)
			{
				errors.Add(error);
			}, null);
			if (worldTrait.forbiddenDLCIds != null)
			{
				using (List<string>.Enumerator enumerator = worldTrait.forbiddenDLCIds.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (DlcManager.IsContentActive(enumerator.Current))
						{
							return;
						}
					}
				}
			}
			int num = SettingsCache.FirstUncommonCharacter(path, file.full_path);
			string text = ((num > -1) ? file.full_path.Substring(num) : file.full_path);
			text = Path.Combine(Path.GetDirectoryName(text), Path.GetFileNameWithoutExtension(text));
			text = text.Replace('\\', '/');
			text = prefix + text;
			worldTrait.filePath = text;
			DebugUtil.DevAssert(!traitsDict.ContainsKey(text), "Overwriting trait " + text + " already exists", null);
			traitsDict[text] = worldTrait;
		}

		public static List<string> GetWorldNames()
		{
			return SettingsCache.worlds.GetNames();
		}

		public static List<string> GetClusterNames()
		{
			return SettingsCache.clusterLayouts.GetNames();
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
			SettingsCache.featureSettings.Clear();
			SettingsCache.worldTraits.Clear();
			SettingsCache.storyTraits.Clear();
			SettingsCache.subworlds.Clear();
			SettingsCache.clusterLayouts.clusterCache.Clear();
			DebugUtil.LogArgs(new object[] { "World Settings cleared!" });
		}

		private static T MergeLoad<T>(T existing, string filename, List<YamlIO.Error> errors) where T : class, IMerge<T>, new()
		{
			ListPool<FileHandle, WorldGenSettings>.PooledList pooledList = ListPool<FileHandle, WorldGenSettings>.Allocate();
			FileSystem.GetFiles(filename, pooledList);
			if (pooledList.Count == 0)
			{
				pooledList.Recycle();
				if (existing != null)
				{
					return existing;
				}
				throw new Exception(string.Format("File not found in any file system: {0}", filename));
			}
			else
			{
				pooledList.Reverse();
				ListPool<T, WorldGenSettings>.PooledList pooledList2 = ListPool<T, WorldGenSettings>.Allocate();
				pooledList2.Add(new T());
				YamlIO.ErrorHandler <>9__0;
				foreach (FileHandle fileHandle in pooledList)
				{
					YamlIO.ErrorHandler errorHandler;
					if ((errorHandler = <>9__0) == null)
					{
						errorHandler = (<>9__0 = delegate(YamlIO.Error error, bool force_log_as_warning)
						{
							errors.Add(error);
						});
					}
					T t = YamlIO.LoadFile<T>(fileHandle, errorHandler, null);
					if (t != null)
					{
						pooledList2.Add(t);
					}
				}
				pooledList.Recycle();
				T t2 = pooledList2[0];
				for (int num = 1; num != pooledList2.Count; num++)
				{
					t2.Merge(pooledList2[num]);
				}
				pooledList2.Recycle();
				if (existing != null)
				{
					return existing.Merge(t2);
				}
				return t2;
			}
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
			SettingsCache.defaults = YamlIO.LoadFile<DefaultSettings>(SettingsCache.GetAbsoluteContentPath("", "worldgen/") + "defaults.yaml", null, null);
			foreach (string text in DlcManager.RELEASE_ORDER)
			{
				if (DlcManager.IsContentActive(text))
				{
					SettingsCache.LoadFiles(SettingsCache.GetAbsoluteContentPath(text, "worldgen/"), SettingsCache.GetScope(text), errors);
				}
			}
			SettingsCache.worlds.Validate();
			DebugUtil.LogArgs(new object[] { "World settings reload complete!" });
			return true;
		}

		private static bool LoadFiles(string worldgenFolderPath, string addPrefix, List<YamlIO.Error> errors)
		{
			SettingsCache.clusterLayouts.LoadFiles(worldgenFolderPath, addPrefix, errors);
			HashSet<string> hashSet = new HashSet<string>(from worldPlacment in SettingsCache.clusterLayouts.clusterCache.Values.SelectMany<ClusterLayout, WorldPlacement>((ClusterLayout clusterLayout) => clusterLayout.worldPlacements)
				select worldPlacment.world);
			SettingsCache.worlds.LoadReferencedWorlds(worldgenFolderPath, addPrefix, hashSet, errors);
			SettingsCache.LoadWorldTraits(worldgenFolderPath, addPrefix, errors);
			SettingsCache.LoadStoryTraits(worldgenFolderPath, addPrefix, errors);
			foreach (KeyValuePair<string, World> keyValuePair in SettingsCache.worlds.worldCache)
			{
				SettingsCache.LoadFeatures(keyValuePair.Value.globalFeatures, errors);
				SettingsCache.LoadSubworlds(keyValuePair.Value.subworldFiles, addPrefix, errors);
			}
			foreach (KeyValuePair<string, WorldTrait> keyValuePair2 in SettingsCache.worldTraits)
			{
				SettingsCache.LoadFeatures(keyValuePair2.Value.globalFeatureMods, errors);
				SettingsCache.LoadSubworlds(keyValuePair2.Value.additionalSubworldFiles, addPrefix, errors);
			}
			foreach (KeyValuePair<string, WorldTrait> keyValuePair3 in SettingsCache.storyTraits)
			{
				SettingsCache.LoadFeatures(keyValuePair3.Value.globalFeatureMods, errors);
				SettingsCache.LoadSubworlds(keyValuePair3.Value.additionalSubworldFiles, addPrefix, errors);
			}
			SettingsCache.layers = SettingsCache.MergeLoad<LevelLayerSettings>(SettingsCache.layers, worldgenFolderPath + "layers.yaml", errors);
			SettingsCache.layers.LevelLayers.ConvertBandSizeToMaxSize();
			SettingsCache.rivers = SettingsCache.MergeLoad<ComposableDictionary<string, River>>(SettingsCache.rivers, worldgenFolderPath + "rivers.yaml", errors);
			SettingsCache.rooms = SettingsCache.MergeLoad<ComposableDictionary<string, Room>>(SettingsCache.rooms, worldgenFolderPath + "rooms.yaml", errors);
			foreach (KeyValuePair<string, Room> keyValuePair4 in SettingsCache.rooms)
			{
				keyValuePair4.Value.name = keyValuePair4.Key;
			}
			SettingsCache.temperatures = SettingsCache.MergeLoad<ComposableDictionary<Temperature.Range, Temperature>>(SettingsCache.temperatures, worldgenFolderPath + "temperatures.yaml", errors);
			SettingsCache.borders = SettingsCache.MergeLoad<ComposableDictionary<string, List<WeightedSimHash>>>(SettingsCache.borders, worldgenFolderPath + "borders.yaml", errors);
			SettingsCache.mobs = SettingsCache.MergeLoad<MobSettings>(SettingsCache.mobs, worldgenFolderPath + "mobs.yaml", errors);
			foreach (KeyValuePair<string, Mob> keyValuePair5 in SettingsCache.mobs.MobLookupTable)
			{
				keyValuePair5.Value.name = keyValuePair5.Key;
			}
			return true;
		}

		public static List<string> GetRandomTraits(int seed, World world)
		{
			if (world.disableWorldTraits || world.worldTraitRules == null || seed == 0)
			{
				return new List<string>();
			}
			KRandom krandom = new KRandom(seed);
			List<WorldTrait> list = new List<WorldTrait>(SettingsCache.worldTraits.Values);
			List<WorldTrait> list2 = new List<WorldTrait>();
			TagSet tagSet = new TagSet();
			using (List<World.TraitRule>.Enumerator enumerator = world.worldTraitRules.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					World.TraitRule rule = enumerator.Current;
					if (rule.specificTraits != null)
					{
						foreach (string text in rule.specificTraits)
						{
							WorldTrait worldTrait;
							if (SettingsCache.worldTraits.TryGetValue(text, out worldTrait))
							{
								list2.Add(SettingsCache.worldTraits[text]);
							}
							else
							{
								DebugUtil.DevLogError("World traits " + text + " doesn't exist, skipping.");
							}
						}
					}
					List<WorldTrait> list3 = new List<WorldTrait>(list);
					TagSet requiredTags = ((rule.requiredTags != null) ? new TagSet(rule.requiredTags) : null);
					TagSet forbiddenTags = ((rule.forbiddenTags != null) ? new TagSet(rule.forbiddenTags) : null);
					list3.RemoveAll((WorldTrait trait) => (requiredTags != null && !trait.traitTagsSet.ContainsAll(requiredTags)) || (forbiddenTags != null && trait.traitTagsSet.ContainsOne(forbiddenTags)) || (rule.forbiddenTraits != null && rule.forbiddenTraits.Contains(trait.filePath)) || !trait.IsValid(world, true));
					int num = krandom.Next(rule.min, Mathf.Max(rule.min, rule.max + 1));
					int count = list2.Count;
					while (list2.Count < count + num && list3.Count > 0)
					{
						int num2 = krandom.Next(list3.Count);
						WorldTrait worldTrait2 = list3[num2];
						bool flag = false;
						using (List<string>.Enumerator enumerator2 = worldTrait2.exclusiveWith.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								string exclusiveId = enumerator2.Current;
								if (list2.Find((WorldTrait t) => t.filePath == exclusiveId) != null)
								{
									flag = true;
									break;
								}
							}
						}
						foreach (string text2 in worldTrait2.exclusiveWithTags)
						{
							if (tagSet.Contains(text2))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							list2.Add(worldTrait2);
							list.Remove(worldTrait2);
							foreach (string text3 in worldTrait2.exclusiveWithTags)
							{
								tagSet.Add(text3);
							}
						}
						list3.RemoveAt(num2);
					}
					if (list2.Count != count + num)
					{
						global::Debug.LogWarning(string.Format("TraitRule on {0} tried to generate {1} but only generated {2}", world.name, num, list2.Count - count));
					}
				}
			}
			List<string> list4 = new List<string>();
			foreach (WorldTrait worldTrait3 in list2)
			{
				list4.Add(worldTrait3.filePath);
			}
			return list4;
		}

		public static ClusterLayouts clusterLayouts = new ClusterLayouts();

		public static Worlds worlds = new Worlds();

		public static Dictionary<string, SubWorld> subworlds = new Dictionary<string, SubWorld>();

		public static TerrainElementBandSettings biomes = new TerrainElementBandSettings();

		public static NoiseTreeFiles noise = new NoiseTreeFiles();

		private static Dictionary<string, FeatureSettings> featureSettings = new Dictionary<string, FeatureSettings>();

		private static Dictionary<string, WorldTrait> worldTraits = new Dictionary<string, WorldTrait>();

		private static Dictionary<string, WorldTrait> storyTraits = new Dictionary<string, WorldTrait>();

		private static Dictionary<string, BiomeSettings> biomeSettingsCache = new Dictionary<string, BiomeSettings>();

		private static string[] s_sourceDelimiter = new string[] { "::" };

		private static Dictionary<string, string> s_cachedPaths = new Dictionary<string, string>();

		private const string LAYERS_FILE = "layers";

		private const string RIVERS_FILE = "rivers";

		private const string ROOMS_FILE = "rooms";

		private const string TEMPERATURES_FILE = "temperatures";

		private const string BORDERS_FILE = "borders";

		private const string DEFAULTS_FILE = "defaults";

		private const string MOBS_FILE = "mobs";

		private const string WORLD_TRAITS_PATH = "traits";

		private const string STORY_TRAITS_PATH = "storytraits";
	}
}
