using System;
using System.Collections.Generic;
using Klei;

namespace ProcGen
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
