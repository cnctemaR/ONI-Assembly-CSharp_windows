using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class WorldMixingSettings
	{
		public string name { get; private set; }

		public string description { get; private set; }

		public string icon { get; private set; }

		public List<string> forbiddenClusterTags { get; private set; }

		public string world { get; private set; }

		public WorldMixingSettings()
		{
			this.forbiddenClusterTags = new List<string>();
		}

		public const string directory = "worldMixing";

		[NonSerialized]
		public bool isModded;
	}
}
