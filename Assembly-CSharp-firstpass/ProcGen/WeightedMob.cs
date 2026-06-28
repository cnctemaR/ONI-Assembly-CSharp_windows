using System;

namespace ProcGen
{
	public class WeightedMob : IWeighted
	{
		public WeightedMob()
		{
		}

		public WeightedMob(string tag, float weight)
		{
			this.tag = tag;
			this.weight = weight;
		}

		public float weight { get; set; }

		public string tag { get; private set; }
	}
}
