using System;
using System.Linq;
using System.Text.RegularExpressions;
using FuzzySharp.PreProcess;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive.Generic;

namespace FuzzySharp.SimilarityRatio.Scorer.StrategySensitive
{
	public abstract class TokenDifferenceScorerBase : StrategySensitiveScorerBase<string>, IRatioScorer
	{
		public override int Score(string[] input1, string[] input2)
		{
			return this.Scorer(input1, input2);
		}

		public int Score(string input1, string input2)
		{
			string[] array = (from s in Regex.Split(input1, "\\s+")
				where s.Any<char>()
				orderby s
				select s).ToArray<string>();
			string[] array2 = (from s in Regex.Split(input2, "\\s+")
				where s.Any<char>()
				orderby s
				select s).ToArray<string>();
			return this.Score(array, array2);
		}

		public int Score(string input1, string input2, PreprocessMode preprocessMode)
		{
			Func<string, string> preprocessor = StringPreprocessorFactory.GetPreprocessor(preprocessMode);
			input1 = preprocessor(input1);
			input2 = preprocessor(input2);
			return this.Score(input1, input2);
		}
	}
}
