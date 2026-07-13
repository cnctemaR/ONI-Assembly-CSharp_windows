using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FuzzySharp.Utils;

namespace FuzzySharp.SimilarityRatio.Scorer.StrategySensitive
{
	public abstract class TokenAbbreviationScorerBase : StrategySensitiveScorerBase
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
			if ((double)text2.Length / (double)text.Length < 1.5)
			{
				return 0;
			}
			string[] array = (from Match m in Regex.Matches(text2, "[a-zA-Z]+")
				select m.Value).ToArray<string>();
			string[] array2 = (from Match m in Regex.Matches(text, "[a-zA-Z]+")
				select m.Value).ToArray<string>();
			if (array2.Length > 4)
			{
				return 0;
			}
			string[] array3;
			string[] array4;
			if (array.Length > array2.Length)
			{
				array3 = array;
				array4 = array2;
			}
			else
			{
				array3 = array2;
				array4 = array;
			}
			List<List<string>> list = array3.PermutationsOfSize<string>(array4.Length);
			List<int> list2 = new List<int>();
			foreach (List<string> list3 in list)
			{
				double num = 0.0;
				for (int i = 0; i < array4.Length; i++)
				{
					string text3 = list3[i];
					string text4 = array4[i];
					if (this.StringContainsInOrder(text3, text4))
					{
						int num2 = this.Scorer(text3, text4);
						num += (double)num2;
					}
				}
				list2.Add((int)(num / (double)array4.Length));
			}
			if (list2.Count != 0)
			{
				return list2.Max();
			}
			return 0;
		}

		private bool StringContainsInOrder(string s1, string s2)
		{
			if (s1.Length < s2.Length)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < s1.Length; i++)
			{
				if (s2[num] == s1[i])
				{
					num++;
				}
				if (num == s2.Length)
				{
					return true;
				}
				if (i + s2.Length - num == s1.Length)
				{
					return false;
				}
			}
			return false;
		}
	}
}
