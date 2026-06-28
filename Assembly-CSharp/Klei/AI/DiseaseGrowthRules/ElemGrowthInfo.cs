using System;
using System.Collections.Generic;
using System.IO;

namespace Klei.AI.DiseaseGrowthRules
{
	public struct ElemGrowthInfo
	{
		public void Write(BinaryWriter writer)
		{
			writer.Write(this.underPopulationDeathRate);
			writer.Write(this.populationHalfLife);
			writer.Write(this.overPopulationHalfLife);
			writer.Write(this.diffusionScale);
			writer.Write(this.minCountPerKG);
			writer.Write(this.maxCountPerKG);
			writer.Write(this.minDiffusionCount);
			writer.Write(this.minDiffusionInfestationTickCount);
		}

		public static void SetBulk(ElemGrowthInfo[] info, Func<Element, bool> test, ElemGrowthInfo settings)
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

		public float CalculateDiseaseCountDelta(int disease_count, float kg, float dt)
		{
			float num = this.minCountPerKG * kg;
			float num2 = this.maxCountPerKG * kg;
			float num4;
			if (num <= (float)disease_count && (float)disease_count <= num2)
			{
				float num3 = Disease.HalfLifeToGrowthRate(this.populationHalfLife, dt);
				num4 = (num3 - 1f) * (float)disease_count;
			}
			else if ((float)disease_count < num)
			{
				num4 = -this.underPopulationDeathRate * dt;
			}
			else
			{
				float num5 = Disease.HalfLifeToGrowthRate(this.overPopulationHalfLife, dt);
				num4 = (num5 - 1f) * (float)disease_count;
			}
			return num4;
		}

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
