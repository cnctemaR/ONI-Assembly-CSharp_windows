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
			float? num = rule.populationHalfLife;
			if (num != null)
			{
				float? num2 = rule.populationHalfLife;
				this.populationHalfLife = num2.Value;
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
