using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class SubworldMixingSettings
	{
		public string name { get; private set; }

		public string description { get; private set; }

		public string icon { get; private set; }

		public List<string> forbiddenClusterTags { get; private set; }

		public WeightedSubworldName subworld { get; private set; }

		public List<string> mixingTags { get; private set; }

		public List<World.TemplateSpawnRules> additionalWorldTemplateRules { get; private set; }

		public SubworldMixingSettings()
		{
			this.mixingTags = new List<string>();
			this.additionalWorldTemplateRules = new List<World.TemplateSpawnRules>();
			this.forbiddenClusterTags = new List<string>();
		}

		public const string directory = "subworldMixing";

		[NonSerialized]
		public bool isModded;
	}
}
