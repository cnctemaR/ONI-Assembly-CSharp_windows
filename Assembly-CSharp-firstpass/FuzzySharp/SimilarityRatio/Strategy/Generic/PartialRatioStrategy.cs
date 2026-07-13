using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp.Edits;

namespace FuzzySharp.SimilarityRatio.Strategy.Generic
{
	internal class PartialRatioStrategy<T> where T : IEquatable<T>
	{
		public static int Calculate(T[] input1, T[] input2)
		{
			if (input1.Length == 0 || input2.Length == 0)
			{
				return 0;
			}
			T[] array;
			T[] array2;
			if (input1.Length < input2.Length)
			{
				array = input1;
				array2 = input2;
			}
			else
			{
				array = input2;
				array2 = input1;
			}
			MatchingBlock[] matchingBlocks = Levenshtein.GetMatchingBlocks<T>(array, array2);
			List<double> list = new List<double>();
			foreach (MatchingBlock matchingBlock in matchingBlocks)
			{
				int num = matchingBlock.DestPos - matchingBlock.SourcePos;
				int num2 = ((num > 0) ? num : 0);
				int num3 = num2 + array.Length;
				if (num3 > array2.Length)
				{
					num3 = array2.Length;
				}
				IEnumerable<T> enumerable = array2.Skip<T>(num2).Take<T>(num3 - num2);
				double ratio = Levenshtein.GetRatio<T>(array, enumerable);
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
