using System;
using System.Collections.Generic;

namespace Klei.AI.DiseaseGrowthRules
{
	public class ExposureRule
	{
		public void Apply(ElemExposureInfo[] infoList)
		{
			List<Element> elements = ElementLoader.elements;
			for (int i = 0; i < elements.Count; i++)
			{
				if (this.Test(elements[i]))
				{
					ElemExposureInfo elemExposureInfo = infoList[i];
					float? num = this.populationHalfLife;
					if (num != null)
					{
						float? num2 = this.populationHalfLife;
						elemExposureInfo.populationHalfLife = num2.Value;
					}
					infoList[i] = elemExposureInfo;
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

		public float? populationHalfLife;
	}
}
