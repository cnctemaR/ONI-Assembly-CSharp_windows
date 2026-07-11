using System;
using System.Collections.Generic;
using System.IO;

namespace Klei.AI.DiseaseGrowthRules
{
	public struct ElemExposureInfo
	{
		public void Write(BinaryWriter writer)
		{
			writer.Write(this.populationHalfLife);
		}

		public static void SetBulk(ElemExposureInfo[] info, Func<Element, bool> test, ElemExposureInfo settings)
		{
			List<Element> elements = ElementLoader.elements;
			for (int i = 0; i < elements.Count; i++)
			{
				if (test(elements[i]))
				{
					info[i] = settings;
				}
			}
		}

		public float CalculateExposureDiseaseCountDelta(int disease_count, float dt)
		{
			return (Disease.HalfLifeToGrowthRate(this.populationHalfLife, dt) - 1f) * (float)disease_count;
		}

		public float populationHalfLife;
	}
}
