using System;
using Klei.AI.DiseaseGrowthRules;

namespace Klei.AI
{
	public class RadiationPoisoning : Disease
	{
		public RadiationPoisoning(bool statsOnly)
			: base("RadiationSickness", 100, Disease.RangeInfo.Idempotent(), Disease.RangeInfo.Idempotent(), Disease.RangeInfo.Idempotent(), Disease.RangeInfo.Idempotent(), 0f, statsOnly)
		{
		}

		protected override void PopulateElemGrowthInfo()
		{
			base.InitializeElemGrowthArray(ref this.elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			base.AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = new float?(0f),
				minCountPerKG = new float?(0f),
				populationHalfLife = new float?(600f),
				maxCountPerKG = new float?(float.PositiveInfinity),
				overPopulationHalfLife = new float?(600f),
				minDiffusionCount = new int?(10000),
				diffusionScale = new float?(0f),
				minDiffusionInfestationTickCount = new byte?((byte)1)
			});
			base.InitializeElemExposureArray(ref this.elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
		}

		public const string ID = "RadiationSickness";
	}
}
