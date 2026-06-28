using System;
using System.Collections.Generic;

namespace Klei
{
	public class DefaultSettings : YamlIO<DefaultSettings>
	{
		public DefaultSettings()
		{
			this.baseData = new BaseLocation();
			this.data = new Dictionary<string, object>();
		}

		public BaseLocation baseData { get; private set; }

		public Dictionary<string, object> data { get; private set; }

		public List<string> defaultMoveTags { get; private set; }

		public List<string> overworldAddTags { get; private set; }

		public float GetFloat(string target)
		{
			object obj = WorldGen.Settings.defaults.data[target];
			if (obj.GetType() == typeof(float))
			{
				return (float)obj;
			}
			float num = float.Parse(obj as string);
			WorldGen.Settings.defaults.data[target] = num;
			return num;
		}

		public int GetInt(string target)
		{
			object obj = WorldGen.Settings.defaults.data[target];
			if (obj.GetType() == typeof(int))
			{
				return (int)obj;
			}
			int num = int.Parse(obj as string);
			WorldGen.Settings.defaults.data[target] = num;
			return num;
		}
	}
}
