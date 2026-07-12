using System;

namespace ProcGen
{
	public class WeightedSubWorld : IWeighted
	{
		public WeightedSubWorld(float weight, SubWorld subWorld, float overridePower = -1f, int minCount = 0, int maxCount = 2147483647, int priority = 0)
		{
			this.weight = weight;
			this.subWorld = subWorld;
			this.overridePower = overridePower;
			this.minCount = minCount;
			this.maxCount = maxCount;
			this.priority = priority;
		}

		public SubWorld subWorld { get; set; }

		public float weight { get; set; }

		public float overridePower { get; set; }

		public int minCount { get; set; }

		public int maxCount { get; set; }

		public int priority { get; set; }

		public override int GetHashCode()
		{
			return this.subWorld.GetHashCode();
		}
	}
}
