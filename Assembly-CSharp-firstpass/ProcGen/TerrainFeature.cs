using System;
using System.Collections.Generic;

namespace ProcGen
{
	public class TerrainFeature : SampleDescriber
	{
		public TerrainFeature()
		{
			this.tags = new List<string>();
			this.excludesTags = new List<string>();
			this.defaultBiome = new Feature();
		}

		public string type { get; private set; }

		public Feature defaultBiome { get; private set; }

		public List<string> tags { get; private set; }

		public List<string> excludesTags { get; private set; }
	}
}
