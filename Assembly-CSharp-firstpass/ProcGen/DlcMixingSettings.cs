using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class DlcMixingSettings
	{
		public List<DlcMixingSettings.SpaceDestinationMix> spaceDesinations { get; private set; }

		public List<SpaceMapPOIPlacement> spacePois { get; private set; }

		public List<LoreCollectionOverride> globalLoreUnlocks { get; private set; }

		public DlcMixingSettings()
		{
			this.spaceDesinations = new List<DlcMixingSettings.SpaceDestinationMix>();
			this.spacePois = new List<SpaceMapPOIPlacement>();
			this.globalLoreUnlocks = new List<LoreCollectionOverride>();
		}

		public class SpaceDestinationMix
		{
			public int minTier { get; private set; }

			public int maxTier { get; private set; }

			public string type { get; private set; }

			public SpaceDestinationMix()
			{
				this.minTier = 0;
				this.maxTier = 99;
			}
		}
	}
}
