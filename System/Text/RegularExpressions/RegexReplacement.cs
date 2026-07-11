using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Text.RegularExpressions
{
	internal sealed class RegexReplacement
	{
		internal RegexReplacement(string rep, RegexNode concat, Hashtable _caps)
		{
			this._rep = rep;
			if (concat.Type() != 25)
			{
				throw new ArgumentException(global::SR.GetString("Replacement pattern error."));
			}
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list = new List<string>();
			List<int> list2 = new List<int>();
			int i = 0;
			while (i < concat.ChildCount())
			{
				RegexNode regexNode = concat.Child(i);
				switch (regexNode.Type())
				{
				case 9:
					stringBuilder.Append(regexNode._ch);
					break;
				case 10:
				case 11:
					goto IL_00E9;
				case 12:
					stringBuilder.Append(regexNode._str);
					break;
				case 13:
				{
					if (stringBuilder.Length > 0)
					{
						list2.Add(list.Count);
						list.Add(stringBuilder.ToString());
						stringBuilder.Length = 0;
					}
					int num = regexNode._m;
					if (_caps != null && num >= 0)
					{
						num = (int)_caps[num];
					}
					list2.Add(-5 - num);
					break;
				}
				default:
					goto IL_00E9;
				}
				i++;
				continue;
				IL_00E9:
				throw new ArgumentException(global::SR.GetString("Replacement pattern error."));
			}
			if (stringBuilder.Length > 0)
			{
				list2.Add(list.Count);
				list.Add(stringBuilder.ToString());
			}
			this._strings = list;
			this._rules = list2;
		}

		private void ReplacementImpl(StringBuilder sb, Match match)
		{
			for (int i = 0; i < this._rules.Count; i++)
			{
				int num = this._rules[i];
				if (num >= 0)
				{
					sb.Append(this._strings[num]);
				}
				else if (num < -4)
				{
					sb.Append(match.GroupToStringImpl(-5 - num));
				}
				else
				{
					switch (-5 - num)
					{
					case -4:
						sb.Append(match.GetOriginalString());
						break;
					case -3:
						sb.Append(match.LastGroupToStringImpl());
						break;
					case -2:
						sb.Append(match.GetRightSubstring());
						break;
					case -1:
						sb.Append(match.GetLeftSubstring());
						break;
					}
				}
			}
		}

		private void ReplacementImplRTL(List<string> al, Match match)
		{
			for (int i = this._rules.Count - 1; i >= 0; i--)
			{
				int num = this._rules[i];
				if (num >= 0)
				{
					al.Add(this._strings[num]);
				}
				else if (num < -4)
				{
					al.Add(match.GroupToStringImpl(-5 - num));
				}
				else
				{
					switch (-5 - num)
					{
					case -4:
						al.Add(match.GetOriginalString());
						break;
					case -3:
						al.Add(match.LastGroupToStringImpl());
						break;
					case -2:
						al.Add(match.GetRightSubstring());
						break;
					case -1:
						al.Add(match.GetLeftSubstring());
						break;
					}
				}
			}
		}

		internal string Pattern
		{
			get
			{
				return this._rep;
			}
		}

		internal string Replacement(Match match)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.ReplacementImpl(stringBuilder, match);
			return stringBuilder.ToString();
		}

		internal string Replace(Regex regex, string input, int count, int startat)
		{
			if (count < -1)
			{
				throw new ArgumentOutOfRangeException("count", global::SR.GetString("Count cannot be less than -1."));
			}
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat", global::SR.GetString("Start index cannot be less than 0 or greater than input length."));
			}
			if (count == 0)
			{
				return input;
			}
			Match match = regex.Match(input, startat);
			if (!match.Success)
			{
				return input;
			}
			StringBuilder stringBuilder;
			if (!regex.RightToLeft)
			{
				stringBuilder = new StringBuilder();
				int num = 0;
				do
				{
					if (match.Index != num)
					{
						stringBuilder.Append(input, num, match.Index - num);
					}
					num = match.Index + match.Length;
					this.ReplacementImpl(stringBuilder, match);
					if (--count == 0)
					{
						break;
					}
					match = match.NextMatch();
				}
				while (match.Success);
				if (num < input.Length)
				{
					stringBuilder.Append(input, num, input.Length - num);
				}
			}
			else
			{
				List<string> list = new List<string>();
				int num2 = input.Length;
				do
				{
					if (match.Index + match.Length != num2)
					{
						list.Add(input.Substring(match.Index + match.Length, num2 - match.Index - match.Length));
					}
					num2 = match.Index;
					this.ReplacementImplRTL(list, match);
					if (--count == 0)
					{
						break;
					}
					match = match.NextMatch();
				}
				while (match.Success);
				stringBuilder = new StringBuilder();
				if (num2 > 0)
				{
					stringBuilder.Append(input, 0, num2);
				}
				for (int i = list.Count - 1; i >= 0; i--)
				{
					stringBuilder.Append(list[i]);
				}
			}
			return stringBuilder.ToString();
		}

		internal static string Replace(MatchEvaluator evaluator, Regex regex, string input, int count, int startat)
		{
			if (evaluator == null)
			{
				throw new ArgumentNullException("evaluator");
			}
			if (count < -1)
			{
				throw new ArgumentOutOfRangeException("count", global::SR.GetString("Count cannot be less than -1."));
			}
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat", global::SR.GetString("Start index cannot be less than 0 or greater than input length."));
			}
			if (count == 0)
			{
				return input;
			}
			Match match = regex.Match(input, startat);
			if (!match.Success)
			{
				return input;
			}
			StringBuilder stringBuilder;
			if (!regex.RightToLeft)
			{
				stringBuilder = new StringBuilder();
				int num = 0;
				do
				{
					if (match.Index != num)
					{
						stringBuilder.Append(input, num, match.Index - num);
					}
					num = match.Index + match.Length;
					stringBuilder.Append(evaluator(match));
					if (--count == 0)
					{
						break;
					}
					match = match.NextMatch();
				}
				while (match.Success);
				if (num < input.Length)
				{
					stringBuilder.Append(input, num, input.Length - num);
				}
			}
			else
			{
				List<string> list = new List<string>();
				int num2 = input.Length;
				do
				{
					if (match.Index + match.Length != num2)
					{
						list.Add(input.Substring(match.Index + match.Length, num2 - match.Index - match.Length));
					}
					num2 = match.Index;
					list.Add(evaluator(match));
					if (--count == 0)
					{
						break;
					}
					match = match.NextMatch();
				}
				while (match.Success);
				stringBuilder = new StringBuilder();
				if (num2 > 0)
				{
					stringBuilder.Append(input, 0, num2);
				}
				for (int i = list.Count - 1; i >= 0; i--)
				{
					stringBuilder.Append(list[i]);
				}
			}
			return stringBuilder.ToString();
		}

		internal static string[] Split(Regex regex, string input, int count, int startat)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", global::SR.GetString("Count cannot be less than -1."));
			}
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat", global::SR.GetString("Start index cannot be less than 0 or greater than input length."));
			}
			if (count == 1)
			{
				return new string[] { input };
			}
			count--;
			Match match = regex.Match(input, startat);
			if (!match.Success)
			{
				return new string[] { input };
			}
			List<string> list = new List<string>();
			if (!regex.RightToLeft)
			{
				int num = 0;
				do
				{
					list.Add(input.Substring(num, match.Index - num));
					num = match.Index + match.Length;
					for (int i = 1; i < match.Groups.Count; i++)
					{
						if (match.IsMatched(i))
						{
							list.Add(match.Groups[i].ToString());
						}
					}
					if (--count == 0)
					{
						break;
					}
					match = match.NextMatch();
				}
				while (match.Success);
				list.Add(input.Substring(num, input.Length - num));
			}
			else
			{
				int num2 = input.Length;
				do
				{
					list.Add(input.Substring(match.Index + match.Length, num2 - match.Index - match.Length));
					num2 = match.Index;
					for (int j = 1; j < match.Groups.Count; j++)
					{
						if (match.IsMatched(j))
						{
							list.Add(match.Groups[j].ToString());
						}
					}
					if (--count == 0)
					{
						break;
					}
					match = match.NextMatch();
				}
				while (match.Success);
				list.Add(input.Substring(0, num2));
				list.Reverse(0, list.Count);
			}
			return list.ToArray();
		}

		internal string _rep;

		internal List<string> _strings;

		internal List<int> _rules;

		internal const int Specials = 4;

		internal const int LeftPortion = -1;

		internal const int RightPortion = -2;

		internal const int LastGroup = -3;

		internal const int WholeString = -4;
	}
}
