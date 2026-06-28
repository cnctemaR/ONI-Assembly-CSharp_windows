using System;
using System.Collections.Generic;
using Generated;
using KSerialization.Converters;

namespace Klei
{
	public class FeatureSettings : YamlIO<FeatureSettings>
	{
		public FeatureSettings()
		{
			this.ElementChoiceGroups = new Dictionary<string, ElementChoiceGroup>();
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

		public Dictionary<string, ElementChoiceGroup> ElementChoiceGroups { get; private set; }

		public bool HasGroup(string item)
		{
			return this.ElementChoiceGroups.ContainsKey(item);
		}

		public WeightedSimHash GetOneWeightedSimHash(string item)
		{
			if (this.ElementChoiceGroups.ContainsKey(item))
			{
				return global::Generated.Util.WeightedRandom.Choose<WeightedSimHash>(this.ElementChoiceGroups[item].choices);
			}
			Debug.LogError("Couldnt get SimHash [" + item + "]", null);
			return null;
		}
	}
}
