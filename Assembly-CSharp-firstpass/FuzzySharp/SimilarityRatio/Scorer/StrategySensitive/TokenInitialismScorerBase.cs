using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FuzzySharp.SimilarityRatio.Scorer.StrategySensitive
{
	public abstract class TokenInitialismScorerBase : StrategySensitiveScorerBase
	{
		public override int Score(string input1, string input2)
		{
			string text;
			string text2;
			if (input1.Length < input2.Length)
			{
				text = input1;
				text2 = input2;
			}
			else
			{
				text = input2;
				text2 = input1;
			}
			if ((double)text2.Length / (double)text.Length < 3.0)
			{
				return 0;
			}
			IEnumerable<char> enumerable = from s in Regex.Split(text2, "\\s+")
				where s.Any<char>()
				select s[0];
			return this.Scorer(string.Join<char>("", enumerable), text);
		}
	}
}
