using System;
using System.Collections.Generic;

namespace Klei.AI.DiseaseGrowthRules
{
	public class GrowthRule
	{
		public void Apply(ElemGrowthInfo[] infoList)
		{
			List<Element> elements = ElementLoader.elements;
			for (int i = 0; i < elements.Count; i++)
			{
				if (this.Test(elements[i]))
				{
					ElemGrowthInfo elemGrowthInfo = infoList[i];
					float? num = this.underPopulationDeathRate;
					if (num != null)
					{
						float? num2 = this.underPopulationDeathRate;
						elemGrowthInfo.underPopulationDeathRate = num2.Value;
					}
					float? num3 = this.populationHalfLife;
					if (num3 != null)
					{
						float? num4 = this.populationHalfLife;
						elemGrowthInfo.populationHalfLife = num4.Value;
					}
					float? num5 = this.overPopulationHalfLife;
					if (num5 != null)
					{
						float? num6 = this.overPopulationHalfLife;
						elemGrowthInfo.overPopulationHalfLife = num6.Value;
					}
					float? num7 = this.diffusionScale;
					if (num7 != null)
					{
						float? num8 = this.diffusionScale;
						elemGrowthInfo.diffusionScale = num8.Value;
					}
					float? num9 = this.minCountPerKG;
					if (num9 != null)
					{
						float? num10 = this.minCountPerKG;
						elemGrowthInfo.minCountPerKG = num10.Value;
					}
					float? num11 = this.maxCountPerKG;
					if (num11 != null)
					{
						float? num12 = this.maxCountPerKG;
						elemGrowthInfo.maxCountPerKG = num12.Value;
					}
					int? num13 = this.minDiffusionCount;
					if (num13 != null)
					{
						int? num14 = this.minDiffusionCount;
						elemGrowthInfo.minDiffusionCount = num14.Value;
					}
					byte? b = this.minDiffusionInfestationTickCount;
					if (b != null)
					{
						byte? b2 = this.minDiffusionInfestationTickCount;
						elemGrowthInfo.minDiffusionInfestationTickCount = b2.Value;
					}
					infoList[i] = elemGrowthInfo;
				}
			}
		}

		public virtual bool Test(Element e)
		{
			return true;
		}

		public virtual string Name()
		{
			return null;
		}

		public float? underPopulationDeathRate;

		public float? populationHalfLife;

		public float? overPopulationHalfLife;

		public float? diffusionScale;

		public float? minCountPerKG;

		public float? maxCountPerKG;

		public int? minDiffusionCount;

		public byte? minDiffusionInfestationTickCount;
	}
}
