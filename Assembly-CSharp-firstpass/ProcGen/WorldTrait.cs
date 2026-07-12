using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class WorldTrait
	{
		public string name { get; private set; }

		public string description { get; private set; }

		public string colorHex { get; private set; }

		public List<string> exclusiveWith { get; private set; }

		public MinMax startingBasePositionHorizontalMod { get; private set; }

		public MinMax startingBasePositionVerticalMod { get; private set; }

		public List<WeightedSubworldName> additionalSubworldFiles { get; private set; }

		public List<World.AllowedCellsFilter> additionalUnknownCellFilters { get; private set; }

		public Dictionary<string, int> globalFeatureTemplateMods { get; private set; }

		public Dictionary<string, int> globalFeatureMods { get; private set; }

		public List<WorldTrait.ElementBandModifier> elementBandModifiers { get; private set; }

		public WorldTrait()
		{
			this.additionalSubworldFiles = new List<WeightedSubworldName>();
			this.additionalUnknownCellFilters = new List<World.AllowedCellsFilter>();
			this.globalFeatureTemplateMods = new Dictionary<string, int>();
			this.globalFeatureMods = new Dictionary<string, int>();
			this.elementBandModifiers = new List<WorldTrait.ElementBandModifier>();
			this.exclusiveWith = new List<string>();
		}

		public string filePath;

		[Serializable]
		public class ElementBandModifier
		{
			public string element { get; private set; }

			public float massMultiplier { get; private set; }

			public float bandMultiplier { get; private set; }

			public ElementBandModifier()
			{
				this.massMultiplier = 1f;
				this.bandMultiplier = 1f;
			}
		}
	}
}
