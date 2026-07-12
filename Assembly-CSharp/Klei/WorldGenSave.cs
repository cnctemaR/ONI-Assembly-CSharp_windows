using System;
using System.Collections.Generic;

namespace Klei
{
	public class WorldGenSave
	{
		public WorldGenSave()
		{
			this.data = new Data();
		}

		public Vector2I version;

		public Data data;

		public string worldID;

		public List<string> traitIDs;

		public List<string> storyTraitIDs;
	}
}
