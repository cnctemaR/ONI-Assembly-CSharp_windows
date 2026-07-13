using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp.Edits;

namespace FuzzySharp.SimilarityRatio.Strategy
{
	internal class PartialRatioStrategy
	{
		public static int Calculate(string input1, string input2)
		{
			if (input1.Length == 0 || input2.Length == 0)
			{
				return 0;
			}
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
			MatchingBlock[] matchingBlocks = Levenshtein.GetMatchingBlocks(text, text2);
			List<double> list = new List<double>();
			foreach (MatchingBlock matchingBlock in matchingBlocks)
			{
				int num = matchingBlock.DestPos - matchingBlock.SourcePos;
				int num2 = ((num > 0) ? num : 0);
				int num3 = num2 + text.Length;
				if (num3 > text2.Length)
				{
					num3 = text2.Length;
				}
				string text3 = text2.Substring(num2, num3 - num2);
				double ratio = Levenshtein.GetRatio(text, text3);
				if (ratio > 0.995)
				{
					return 100;
				}
				list.Add(ratio);
			}
			return (int)Math.Round(100.0 * list.Max());
		}
	}
}
