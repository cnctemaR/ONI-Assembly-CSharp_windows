using System;
using System.Collections.Generic;

namespace ProcGen
{
	public class WorldGenSettings
	{
		public WorldGenSettings(string worldName, List<string> traits, bool assertMissingTraits)
		{
			if (!SettingsCache.worlds.HasWorld(worldName))
			{
				DebugUtil.LogWarningArgs(new object[] { string.Format("Failed to get worldGen data for {0}. Using {1} instead", worldName, "worlds/SandstoneDefault") });
				DebugUtil.Assert(SettingsCache.worlds.HasWorld("worlds/SandstoneDefault"));
				worldName = "worlds/SandstoneDefault";
			}
			World worldData = SettingsCache.worlds.GetWorldData(worldName);
			List<WorldTrait> list = new List<WorldTrait>();
			if (!worldData.disableWorldTraits && traits != null)
			{
				DebugUtil.LogArgs(new object[]
				{
					"Generating a world with the traits:",
					string.Join(", ", traits.ToArray())
				});
				foreach (string text in traits)
				{
					WorldTrait cachedTrait = SettingsCache.GetCachedTrait(text, assertMissingTraits);
					if (cachedTrait != null)
					{
						list.Add(cachedTrait);
					}
				}
			}
			else
			{
				Debug.Log("Generating a world without traits. Either this world has traits disabled or none were specified.");
			}
			this.mutatedWorldData = new MutatedWorldData(worldData, list);
			Debug.Log("Set world to [" + worldName + "] " + SettingsCache.GetPath());
		}

		public World world
		{
			get
			{
				return this.mutatedWorldData.world;
			}
		}

		public BaseLocation GetBaseLocation()
		{
			if (this.world != null && this.world.defaultsOverrides != null && this.world.defaultsOverrides.baseData != null)
			{
				DebugUtil.LogArgs(new object[] { string.Format("World '{0}' is overriding baseData", this.world.name) });
				return this.world.defaultsOverrides.baseData;
			}
			return SettingsCache.defaults.baseData;
		}

		public List<string> GetOverworldAddTags()
		{
			if (this.world != null && this.world.defaultsOverrides != null && this.world.defaultsOverrides.overworldAddTags != null)
			{
				DebugUtil.LogArgs(new object[] { string.Format("World '{0}' is overriding overworldAddTags", this.world.name) });
				return this.world.defaultsOverrides.overworldAddTags;
			}
			return SettingsCache.defaults.overworldAddTags;
		}

		public List<string> GetDefaultMoveTags()
		{
			if (this.world != null && this.world.defaultsOverrides != null && this.world.defaultsOverrides.defaultMoveTags != null)
			{
				DebugUtil.LogArgs(new object[] { string.Format("World '{0}' is overriding defaultMoveTags", this.world.name) });
				return this.world.defaultsOverrides.defaultMoveTags;
			}
			return SettingsCache.defaults.defaultMoveTags;
		}

		public string[] GetTraitIDs()
		{
			if (this.mutatedWorldData.traits != null && this.mutatedWorldData.traits.Count > 0)
			{
				string[] array = new string[this.mutatedWorldData.traits.Count];
				for (int i = 0; i < this.mutatedWorldData.traits.Count; i++)
				{
					array[i] = this.mutatedWorldData.traits[i].filePath;
				}
				return array;
			}
			return new string[0];
		}

		private bool GetSetting<T>(DefaultSettings set, string target, WorldGenSettings.ParserFn<T> parser, out T res)
		{
			if (set == null || set.data == null || !set.data.ContainsKey(target))
			{
				res = default(T);
				return false;
			}
			object obj = set.data[target];
			if (obj.GetType() == typeof(T))
			{
				res = (T)((object)obj);
				return true;
			}
			bool flag = parser(obj as string, out res);
			if (flag)
			{
				set.data[target] = res;
			}
			return flag;
		}

		private T GetSetting<T>(string target, WorldGenSettings.ParserFn<T> parser)
		{
			T t;
			if (this.world != null)
			{
				if (!this.GetSetting<T>(this.world.defaultsOverrides, target, parser, out t))
				{
					this.GetSetting<T>(SettingsCache.defaults, target, parser, out t);
				}
				else
				{
					DebugUtil.LogArgs(new object[] { string.Format("World '{0}' is overriding setting '{1}'", this.world.name, target) });
				}
			}
			else if (!this.GetSetting<T>(SettingsCache.defaults, target, parser, out t))
			{
				DebugUtil.LogWarningArgs(new object[] { string.Format("Couldn't find setting '{0}' in default settings!", target) });
			}
			return t;
		}

		public bool GetBoolSetting(string target)
		{
			return this.GetSetting<bool>(target, new WorldGenSettings.ParserFn<bool>(bool.TryParse));
		}

		private bool TryParseString(string input, out string res)
		{
			res = input;
			return true;
		}

		public string GetStringSetting(string target)
		{
			return this.GetSetting<string>(target, new WorldGenSettings.ParserFn<string>(this.TryParseString));
		}

		public float GetFloatSetting(string target)
		{
			return this.GetSetting<float>(target, new WorldGenSettings.ParserFn<float>(float.TryParse));
		}

		public int GetIntSetting(string target)
		{
			return this.GetSetting<int>(target, new WorldGenSettings.ParserFn<int>(int.TryParse));
		}

		public E GetEnumSetting<E>(string target) where E : struct
		{
			return this.GetSetting<E>(target, new WorldGenSettings.ParserFn<E>(WorldGenSettings.TryParseEnum<E>));
		}

		private static bool TryParseEnum<E>(string value, out E result) where E : struct
		{
			try
			{
				result = (E)((object)Enum.Parse(typeof(E), value));
				return true;
			}
			catch (Exception)
			{
				result = new E();
			}
			return false;
		}

		public bool HasFeature(string name)
		{
			return this.mutatedWorldData.features.ContainsKey(name);
		}

		public FeatureSettings GetFeature(string name)
		{
			if (this.mutatedWorldData.features.ContainsKey(name))
			{
				return this.mutatedWorldData.features[name];
			}
			throw new Exception("Couldnt get feature from active world data [" + name + "]");
		}

		public FeatureSettings TryGetFeature(string name)
		{
			FeatureSettings featureSettings;
			this.mutatedWorldData.features.TryGetValue(name, out featureSettings);
			return featureSettings;
		}

		public bool HasSubworld(string name)
		{
			return this.mutatedWorldData.subworlds.ContainsKey(name);
		}

		public SubWorld GetSubWorld(string name)
		{
			if (this.mutatedWorldData.subworlds.ContainsKey(name))
			{
				return this.mutatedWorldData.subworlds[name];
			}
			throw new Exception("Couldnt get subworld from active world data [" + name + "]");
		}

		public SubWorld TryGetSubWorld(string name)
		{
			SubWorld subWorld;
			this.mutatedWorldData.subworlds.TryGetValue(name, out subWorld);
			return subWorld;
		}

		public List<WeightedSubWorld> GetSubworldsForWorld(List<WeightedName> subworldList)
		{
			List<WeightedSubWorld> list = new List<WeightedSubWorld>();
			foreach (KeyValuePair<string, SubWorld> keyValuePair in this.mutatedWorldData.subworlds)
			{
				foreach (WeightedName weightedName in subworldList)
				{
					if (keyValuePair.Key == weightedName.name)
					{
						list.Add(new WeightedSubWorld(weightedName.weight, keyValuePair.Value));
					}
				}
			}
			return list;
		}

		public bool HasMob(string id)
		{
			return this.mutatedWorldData.mobs.HasMob(id);
		}

		public Mob GetMob(string id)
		{
			return this.mutatedWorldData.mobs.GetMob(id);
		}

		public ElementBandConfiguration GetElementBandForBiome(string name)
		{
			ElementBandConfiguration elementBandConfiguration;
			if (this.mutatedWorldData.biomes.BiomeBackgroundElementBandConfigurations.TryGetValue(name, out elementBandConfiguration))
			{
				return elementBandConfiguration;
			}
			return null;
		}

		private MutatedWorldData mutatedWorldData;

		public const string defaultWorldName = "worlds/SandstoneDefault";

		private delegate bool ParserFn<T>(string input, out T res);
	}
}
