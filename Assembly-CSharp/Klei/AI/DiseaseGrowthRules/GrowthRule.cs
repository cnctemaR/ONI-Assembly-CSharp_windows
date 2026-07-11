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
				Element element = elements[i];
				if (element.id != SimHashes.Vacuum && this.Test(element))
				{
					ElemGrowthInfo elemGrowthInfo = infoList[i];
					if (this.underPopulationDeathRate != null)
					{
						elemGrowthInfo.underPopulationDeathRate = this.underPopulationDeathRate.Value;
					}
					if (this.populationHalfLife != null)
					{
						elemGrowthInfo.populationHalfLife = this.populationHalfLife.Value;
					}
					if (this.overPopulationHalfLife != null)
					{
						elemGrowthInfo.overPopulationHalfLife = this.overPopulationHalfLife.Value;
					}
					if (this.diffusionScale != null)
					{
						elemGrowthInfo.diffusionScale = this.diffusionScale.Value;
					}
					if (this.minCountPerKG != null)
					{
						elemGrowthInfo.minCountPerKG = this.minCountPerKG.Value;
					}
					if (this.maxCountPerKG != null)
					{
						elemGrowthInfo.maxCountPerKG = this.maxCountPerKG.Value;
					}
					if (this.minDiffusionCount != null)
					{
						elemGrowthInfo.minDiffusionCount = this.minDiffusionCount.Value;
					}
					if (this.minDiffusionInfestationTickCount != null)
					{
						elemGrowthInfo.minDiffusionInfestationTickCount = this.minDiffusionInfestationTickCount.Value;
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
