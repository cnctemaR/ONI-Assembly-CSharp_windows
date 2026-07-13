using System;
using FuzzySharp.PreProcess;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.Composite;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

namespace FuzzySharp
{
	public static class Fuzz
	{
		public static int Ratio(string input1, string input2)
		{
			return ScorerCache.Get<DefaultRatioScorer>().Score(input1, input2);
		}

		public static int Ratio(string input1, string input2, PreprocessMode preprocessMode)
		{
			return ScorerCache.Get<DefaultRatioScorer>().Score(input1, input2, preprocessMode);
		}

		public static int PartialRatio(string input1, string input2)
		{
			return ScorerCache.Get<PartialRatioScorer>().Score(input1, input2);
		}

		public static int PartialRatio(string input1, string input2, PreprocessMode preprocessMode)
		{
			return ScorerCache.Get<PartialRatioScorer>().Score(input1, input2, preprocessMode);
		}

		public static int TokenSortRatio(string input1, string input2)
		{
			return ScorerCache.Get<TokenSortScorer>().Score(input1, input2);
		}

		public static int TokenSortRatio(string input1, string input2, PreprocessMode preprocessMode)
		{
			return ScorerCache.Get<TokenSortScorer>().Score(input1, input2, preprocessMode);
		}

		public static int PartialTokenSortRatio(string input1, string input2)
		{
			return ScorerCache.Get<PartialTokenSortScorer>().Score(input1, input2);
		}

		public static int PartialTokenSortRatio(string input1, string input2, PreprocessMode preprocessMode)
		{
			return ScorerCache.Get<PartialTokenSortScorer>().Score(input1, input2, preprocessMode);
		}

		public static int TokenSetRatio(string input1, string input2)
		{
			return ScorerCache.Get<TokenSetScorer>().Score(input1, input2);
		}

		public static int TokenSetRatio(string input1, string input2, PreprocessMode preprocessMode)
		{
			return ScorerCache.Get<TokenSetScorer>().Score(input1, input2, preprocessMode);
		}

		public static int PartialTokenSetRatio(string input1, string input2)
		{
			return ScorerCache.Get<PartialTokenSetScorer>().Score(input1, input2);
		}

		public static int PartialTokenSetRatio(string input1, string input2, PreprocessMode preprocessMode)
		{
			return ScorerCache.Get<PartialTokenSetScorer>().Score(input1, input2, preprocessMode);
		}

		public static int TokenDifferenceRatio(string input1, string input2)
		{
			return ScorerCache.Get<TokenDifferenceScorer>().Score(input1, input2);
		}

		public static int TokenDifferenceRatio(string input1, string input2, PreprocessMode preprocessMode)
		{
			return ScorerCache.Get<TokenDifferenceScorer>().Score(input1, input2, preprocessMode);
		}

		public static int PartialTokenDifferenceRatio(string input1, string input2)
		{
			return ScorerCache.Get<PartialTokenDifferenceScorer>().Score(input1, input2);
		}

		public static int PartialTokenDifferenceRatio(string input1, string input2, PreprocessMode preprocessMode)
		{
			return ScorerCache.Get<PartialTokenDifferenceScorer>().Score(input1, input2, preprocessMode);
		}

		public static int TokenInitialismRatio(string input1, string input2)
		{
			return ScorerCache.Get<TokenInitialismScorer>().Score(input1, input2);
		}

		public static int TokenInitialismRatio(string input1, string input2, PreprocessMode preprocessMode)
		{
			return ScorerCache.Get<TokenInitialismScorer>().Score(input1, input2, preprocessMode);
		}

		public static int PartialTokenInitialismRatio(string input1, string input2)
		{
			return ScorerCache.Get<PartialTokenInitialismScorer>().Score(input1, input2);
		}

		public static int PartialTokenInitialismRatio(string input1, string input2, PreprocessMode preprocessMode)
		{
			return ScorerCache.Get<PartialTokenInitialismScorer>().Score(input1, input2);
		}

		public static int TokenAbbreviationRatio(string input1, string input2)
		{
			return ScorerCache.Get<TokenAbbreviationScorer>().Score(input1, input2);
		}

		public static int TokenAbbreviationRatio(string input1, string input2, PreprocessMode preprocessMode)
		{
			return ScorerCache.Get<TokenAbbreviationScorer>().Score(input1, input2, preprocessMode);
		}

		public static int PartialTokenAbbreviationRatio(string input1, string input2)
		{
			return ScorerCache.Get<PartialTokenAbbreviationScorer>().Score(input1, input2);
		}

		public static int PartialTokenAbbreviationRatio(string input1, string input2, PreprocessMode preprocessMode)
		{
			return ScorerCache.Get<PartialTokenAbbreviationScorer>().Score(input1, input2, preprocessMode);
		}

		public static int WeightedRatio(string input1, string input2)
		{
			return ScorerCache.Get<WeightedRatioScorer>().Score(input1, input2);
		}

		public static int WeightedRatio(string input1, string input2, PreprocessMode preprocessMode)
		{
			return ScorerCache.Get<WeightedRatioScorer>().Score(input1, input2, preprocessMode);
		}
	}
}
