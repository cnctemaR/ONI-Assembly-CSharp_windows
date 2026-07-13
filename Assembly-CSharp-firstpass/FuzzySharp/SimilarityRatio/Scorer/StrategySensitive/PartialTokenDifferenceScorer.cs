using System;
using FuzzySharp.SimilarityRatio.Strategy.Generic;

namespace FuzzySharp.SimilarityRatio.Scorer.StrategySensitive
{
	public class PartialTokenDifferenceScorer : TokenDifferenceScorerBase
	{
		protected override Func<string[], string[], int> Scorer
		{
			get
			{
				return new Func<string[], string[], int>(PartialRatioStrategy<string>.Calculate);
			}
		}
	}
}
