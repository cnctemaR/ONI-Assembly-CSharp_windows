using System;

namespace Klei.AI.DiseaseGrowthRules
{
	public class CompositeGrowthRule
	{
		public string Name()
		{
			return this.name;
		}

		public void Overlay(GrowthRule rule)
		{
			float? num = rule.underPopulationDeathRate;
			if (num != null)
			{
				float? num2 = rule.underPopulationDeathRate;
				this.underPopulationDeathRate = num2.Value;
			}
			float? num3 = rule.populationHalfLife;
			if (num3 != null)
			{
				float? num4 = rule.populationHalfLife;
				this.populationHalfLife = num4.Value;
			}
			float? num5 = rule.overPopulationHalfLife;
			if (num5 != null)
			{
				float? num6 = rule.overPopulationHalfLife;
				this.overPopulationHalfLife = num6.Value;
			}
			float? num7 = rule.diffusionScale;
			if (num7 != null)
			{
				float? num8 = rule.diffusionScale;
				this.diffusionScale = num8.Value;
			}
			float? num9 = rule.minCountPerKG;
			if (num9 != null)
			{
				float? num10 = rule.minCountPerKG;
				this.minCountPerKG = num10.Value;
			}
			float? num11 = rule.maxCountPerKG;
			if (num11 != null)
			{
				float? num12 = rule.maxCountPerKG;
				this.maxCountPerKG = num12.Value;
			}
			int? num13 = rule.minDiffusionCount;
			if (num13 != null)
			{
				int? num14 = rule.minDiffusionCount;
				this.minDiffusionCount = num14.Value;
			}
			byte? b = rule.minDiffusionInfestationTickCount;
			if (b != null)
			{
				byte? b2 = rule.minDiffusionInfestationTickCount;
				this.minDiffusionInfestationTickCount = b2.Value;
			}
			this.name = rule.Name();
		}

		public float GetHalfLifeForCount(int count, float kg)
		{
			int num = (int)(this.minCountPerKG * kg);
			int num2 = (int)(this.maxCountPerKG * kg);
			float num3;
			if (count < num)
			{
				num3 = this.populationHalfLife;
			}
			else if (count < num2)
			{
				num3 = this.populationHalfLife;
			}
			else
			{
				num3 = this.overPopulationHalfLife;
			}
			return num3;
		}

		public string name;

		public float underPopulationDeathRate;

		public float populationHalfLife;

		public float overPopulationHalfLife;

		public float diffusionScale;

		public float minCountPerKG;

		public float maxCountPerKG;

		public int minDiffusionCount;

		public byte minDiffusionInfestationTickCount;
	}
}
