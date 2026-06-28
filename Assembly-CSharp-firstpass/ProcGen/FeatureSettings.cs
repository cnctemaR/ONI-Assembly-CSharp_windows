using System;
using System.Collections.Generic;
using Klei;
using KSerialization.Converters;

namespace ProcGen
{
	public class FeatureSettings : YamlIO<FeatureSettings>
	{
		public FeatureSettings()
		{
			this.ElementChoiceGroups = new Dictionary<string, ElementChoiceGroup<WeightedSimHash>>();
			this.borders = new List<int>();
			this.excludeTags = new List<string>();
			this.internalMobs = new List<MobReference>();
		}

		[StringEnumConverter]
		public Room.Shape shape { get; private set; }

		public List<int> borders { get; private set; }

		public MinMax blobSize { get; private set; }

		public List<MobReference> internalMobs { get; private set; }

		public List<string> excludeTags { get; private set; }

		public Dictionary<string, ElementChoiceGroup<WeightedSimHash>> ElementChoiceGroups { get; private set; }

		public bool HasGroup(string item)
		{
			return this.ElementChoiceGroups.ContainsKey(item);
		}

		public WeightedSimHash GetOneWeightedSimHash(string item, SeededRandom rnd)
		{
			WeightedSimHash weightedSimHash;
			if (this.ElementChoiceGroups.ContainsKey(item))
			{
				weightedSimHash = WeightedRandom.Choose<WeightedSimHash>(this.ElementChoiceGroups[item].choices, rnd);
			}
			else
			{
				Debug.LogError("Couldnt get SimHash [" + item + "]", null);
				weightedSimHash = null;
			}
			return weightedSimHash;
		}
	}
}
