using System;
using System.Collections.Generic;

namespace Klei
{
	public class SubWorlds : YamlIO<SubWorlds>
	{
		public SubWorlds()
		{
			this.zones = new Dictionary<string, SubWorld>();
		}

		public Dictionary<string, SubWorld> zones { get; private set; }

		public SubWorld GetSubWorld(string name)
		{
			if (this.zones.ContainsKey(name))
			{
				return this.zones[name];
			}
			return null;
		}
	}
}
