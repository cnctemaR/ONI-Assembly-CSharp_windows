using System;

namespace ProcGen
{
	[Serializable]
	public class WeightedSubworldName : IWeighted
	{
		public WeightedSubworldName()
		{
			this.weight = 1f;
			this.maxCount = int.MaxValue;
		}

		public WeightedSubworldName(string name, float weight)
		{
			this.name = name;
			this.weight = weight;
		}

		public string name { get; private set; }

		public string overrideName { get; private set; }

		public float overridePower { get; private set; }

		public float weight { get; set; }

		public int minCount { get; set; }

		public int maxCount { get; set; }

		public int priority { get; set; }
	}
}
