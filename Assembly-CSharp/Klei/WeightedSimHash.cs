using System;
using System.Collections.Generic;
using Generated;

namespace Klei
{
	public class WeightedSimHash : global::Generated.Util.IWeighted
	{
		public WeightedSimHash()
		{
			this.overrides = new List<SampleDescriber.Override>();
		}

		public WeightedSimHash(SimHashes elementHash, float weight)
		{
			this.element = elementHash;
			this.weight = weight;
			this.overrides = new List<SampleDescriber.Override>();
		}

		public SimHashes element { get; private set; }

		public float weight { get; set; }

		public List<SampleDescriber.Override> overrides { get; private set; }
	}
}
