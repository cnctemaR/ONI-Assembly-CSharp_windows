using System;

namespace Klei.AI.DiseaseGrowthRules
{
	public class CompositeExposureRule
	{
		public string Name()
		{
			return this.name;
		}

		public void Overlay(ExposureRule rule)
		{
			if (rule.populationHalfLife != null)
			{
				this.populationHalfLife = rule.populationHalfLife.Value;
			}
			this.name = rule.Name();
		}

		public float GetHalfLifeForCount(int count)
		{
			return this.populationHalfLife;
		}

		public string name;

		public float populationHalfLife;
	}
}
