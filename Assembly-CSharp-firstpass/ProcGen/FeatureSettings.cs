using System;
using System.Collections.Generic;
using KSerialization.Converters;

namespace ProcGen
{
	[Serializable]
	public class FeatureSettings
	{
		public FeatureSettings()
		{
			this.ElementChoiceGroups = new Dictionary<string, ElementChoiceGroup<WeightedSimHash>>();
			this.borders = new List<int>();
			this.tags = new List<string>();
			this.internalMobs = new List<MobReference>();
		}

		[StringEnumConverter]
		public Room.Shape shape { get; private set; }

		public List<int> borders { get; private set; }

		public MinMax blobSize { get; private set; }

		public string forceBiome { get; private set; }

		public List<string> biomeTags { get; private set; }

		public List<MobReference> internalMobs { get; private set; }

		public List<string> tags { get; private set; }

		public Dictionary<string, ElementChoiceGroup<WeightedSimHash>> ElementChoiceGroups { get; private set; }

		public bool HasGroup(string item)
		{
			return this.ElementChoiceGroups.ContainsKey(item);
		}

		public WeightedSimHash GetOneWeightedSimHash(string item, SeededRandom rnd)
		{
			if (this.ElementChoiceGroups.ContainsKey(item))
			{
				return WeightedRandom.Choose<WeightedSimHash>(this.ElementChoiceGroups[item].choices, rnd);
			}
			Debug.LogError("Couldnt get SimHash [" + item + "]");
			return null;
		}
	}
}
