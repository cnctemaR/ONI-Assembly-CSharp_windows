using System;
using System.Collections.Generic;

namespace ProcGen
{
	public class WorldGenSettings
	{
		public WorldGenSettings(string worldName = "worlds/Default")
		{
			if (!SettingsCache.worlds.HasWorld(worldName))
			{
				DebugUtil.LogWarningArgs(new object[] { string.Format("Failed to get worldGen data for {0}. Using {1} instead", worldName, "worlds/Default") });
				DebugUtil.Assert(SettingsCache.worlds.HasWorld("worlds/Default"));
				worldName = "worlds/Default";
			}
			this.world = SettingsCache.worlds.GetWorldData(worldName).world;
			Debug.Log("Set world to [" + worldName + "] " + SettingsCache.GetPath());
		}

		public World world { get; private set; }

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

		public static string GetSimpleName(string longName)
		{
			string[] array = longName.Split(new char[] { '/' });
			return array[array.Length - 1];
		}

		public const string defaultWorldName = "worlds/Default";

		private delegate bool ParserFn<T>(string input, out T res);
	}
}
