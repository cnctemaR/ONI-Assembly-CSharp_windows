using System;
using System.Collections.Generic;

namespace Klei
{
	public class WorldGenSave
	{
		public WorldGenSave()
		{
			this.data = new Data();
			this.stats = new Dictionary<string, object>();
		}

		public Vector2I version;

		public Dictionary<string, object> stats;

		public Data data;

		public string worldID;

		public List<string> traitIDs;

		public List<string> storyTraitIDs;
	}
}
