using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class WorldTrait
	{
		public WorldTrait()
		{
			this.additionalSubworldFiles = new List<WeightedName>();
			this.additionalUnknownCellFilters = new List<World.AllowedCellsFilter>();
			this.globalFeatureTemplateMods = new Dictionary<string, int>();
			this.globalFeatureMods = new Dictionary<string, int>();
			this.elementBandModifiers = new List<WorldTrait.ElementBandModifier>();
			this.exclusiveWith = new List<string>();
		}

		public string name { get; private set; }

		public string description { get; private set; }

		public string colorHex { get; private set; }

		public List<string> exclusiveWith { get; private set; }

		public MinMax startingBasePositionHorizontalMod { get; private set; }

		public MinMax startingBasePositionVerticalMod { get; private set; }

		public List<WeightedName> additionalSubworldFiles { get; private set; }

		public List<World.AllowedCellsFilter> additionalUnknownCellFilters { get; private set; }

		public Dictionary<string, int> globalFeatureTemplateMods { get; private set; }

		public Dictionary<string, int> globalFeatureMods { get; private set; }

		public List<WorldTrait.ElementBandModifier> elementBandModifiers { get; private set; }

		[Serializable]
		public class ElementBandModifier
		{
			public ElementBandModifier()
			{
				this.massMultiplier = 1f;
				this.bandMultiplier = 1f;
			}

			public string element { get; private set; }

			public float massMultiplier { get; private set; }

			public float bandMultiplier { get; private set; }
		}
	}
}
