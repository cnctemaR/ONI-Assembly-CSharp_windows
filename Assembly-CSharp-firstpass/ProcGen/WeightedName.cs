using System;

namespace ProcGen
{
	public class WeightedName : IWeighted
	{
		public WeightedName()
		{
			this.weight = 1f;
		}

		public WeightedName(string name, float weight)
		{
			this.name = name;
			this.weight = weight;
		}

		public string name { get; private set; }

		public string overrideName { get; private set; }

		public float weight { get; set; }
	}
}
