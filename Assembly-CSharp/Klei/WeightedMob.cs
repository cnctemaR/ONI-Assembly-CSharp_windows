using System;
using Generated;

namespace Klei
{
	public class WeightedMob : global::Generated.Util.IWeighted
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
