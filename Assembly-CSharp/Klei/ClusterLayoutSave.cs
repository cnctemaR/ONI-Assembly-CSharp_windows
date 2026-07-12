using System;
using System.Collections.Generic;

namespace Klei
{
	public class ClusterLayoutSave
	{
		public ClusterLayoutSave()
		{
			this.worlds = new List<ClusterLayoutSave.World>();
		}

		public string ID;

		public Vector2I version;

		public List<ClusterLayoutSave.World> worlds;

		public Vector2I size;

		public int currentWorldIdx;

		public int numRings;

		public Dictionary<ClusterLayoutSave.POIType, List<AxialI>> poiLocations = new Dictionary<ClusterLayoutSave.POIType, List<AxialI>>();

		public Dictionary<AxialI, string> poiPlacements = new Dictionary<AxialI, string>();

		public class World
		{
			public Data data = new Data();

			public string name = string.Empty;

			public bool isDiscovered;

			public List<string> traits = new List<string>();

			public List<string> storyTraits = new List<string>();

			public List<string> seasons = new List<string>();

			public List<string> generatedSubworlds = new List<string>();
		}

		public enum POIType
		{
			TemporalTear,
			ResearchDestination
		}
	}
}
