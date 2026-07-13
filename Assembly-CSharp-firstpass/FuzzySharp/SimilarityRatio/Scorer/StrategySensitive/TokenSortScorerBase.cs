using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FuzzySharp.SimilarityRatio.Scorer.StrategySensitive
{
	public abstract class TokenSortScorerBase : StrategySensitiveScorerBase
	{
		public override int Score(string input1, string input2)
		{
			string text = string.Join(" ", from s in Regex.Split(input1, "\\s+")
				where s.Any<char>()
				orderby s
				select s).Trim();
			string text2 = string.Join(" ", from s in Regex.Split(input2, "\\s+")
				where s.Any<char>()
				orderby s
				select s).Trim();
			return this.Scorer(text, text2);
		}
	}
}
