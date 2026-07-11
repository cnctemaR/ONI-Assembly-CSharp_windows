using System;
using System.Collections.Generic;

namespace Database
{
	public class ArtifactDropRate : Resource
	{
		public void AddItem(ArtifactTier tier, float weight)
		{
			this.rates.Add(new Tuple<ArtifactTier, float>(tier, weight));
			this.totalWeight += weight;
		}

		public float GetTierWeight(ArtifactTier tier)
		{
			float num = 0f;
			foreach (Tuple<ArtifactTier, float> tuple in this.rates)
			{
				if (tuple.first == tier)
				{
					num = tuple.second;
				}
			}
			return num;
		}

		public List<Tuple<ArtifactTier, float>> rates = new List<Tuple<ArtifactTier, float>>();

		public float totalWeight;
	}
}
