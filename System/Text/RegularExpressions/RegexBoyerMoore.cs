using System;
using System.Globalization;

namespace System.Text.RegularExpressions
{
	internal sealed class RegexBoyerMoore
	{
		public RegexBoyerMoore(string pattern, bool caseInsensitive, bool rightToLeft, CultureInfo culture)
		{
			if (caseInsensitive)
			{
				StringBuilder stringBuilder = StringBuilderCache.Acquire(pattern.Length);
				for (int i = 0; i < pattern.Length; i++)
				{
					stringBuilder.Append(culture.TextInfo.ToLower(pattern[i]));
				}
				pattern = StringBuilderCache.GetStringAndRelease(stringBuilder);
			}
			this.Pattern = pattern;
			this.RightToLeft = rightToLeft;
			this.CaseInsensitive = caseInsensitive;
			this._culture = culture;
			int num;
			int num2;
			int num3;
			if (!rightToLeft)
			{
				num = -1;
				num2 = pattern.Length - 1;
				num3 = 1;
			}
			else
			{
				num = pattern.Length;
				num2 = 0;
				num3 = -1;
			}
			this.Positive = new int[pattern.Length];
			int num4 = num2;
			char c = pattern[num4];
			this.Positive[num4] = num3;
			num4 -= num3;
			while (num4 != num)
			{
				if (pattern[num4] != c)
				{
					num4 -= num3;
				}
				else
				{
					int num5 = num2;
					int num6 = num4;
					while (num6 != num && pattern[num5] == pattern[num6])
					{
						num6 -= num3;
						num5 -= num3;
					}
					if (this.Positive[num5] == 0)
					{
						this.Positive[num5] = num5 - num6;
					}
					num4 -= num3;
				}
			}
			for (int num5 = num2 - num3; num5 != num; num5 -= num3)
			{
				if (this.Positive[num5] == 0)
				{
					this.Positive[num5] = num3;
				}
			}
			this.NegativeASCII = new int[128];
			for (int j = 0; j < 128; j++)
			{
				this.NegativeASCII[j] = num2 - num;
			}
			this.LowASCII = 127;
			this.HighASCII = 0;
			for (num4 = num2; num4 != num; num4 -= num3)
			{
				c = pattern[num4];
				if (c < '\u0080')
				{
					if (this.LowASCII > (int)c)
					{
						this.LowASCII = (int)c;
					}
					if (this.HighASCII < (int)c)
					{
						this.HighASCII = (int)c;
					}
					if (this.NegativeASCII[(int)c] == num2 - num)
					{
						this.NegativeASCII[(int)c] = num2 - num4;
					}
				}
				else
				{
					int num7 = (int)(c >> 8);
					int num8 = (int)(c & 'ÿ');
					if (this.NegativeUnicode == null)
					{
						this.NegativeUnicode = new int[256][];
					}
					if (this.NegativeUnicode[num7] == null)
					{
						int[] array = new int[256];
						for (int k = 0; k < array.Length; k++)
						{
							array[k] = num2 - num;
						}
						if (num7 == 0)
						{
							Array.Copy(this.NegativeASCII, 0, array, 0, 128);
							this.NegativeASCII = array;
						}
						this.NegativeUnicode[num7] = array;
					}
					if (this.NegativeUnicode[num7][num8] == num2 - num)
					{
						this.NegativeUnicode[num7][num8] = num2 - num4;
					}
				}
			}
		}

		private bool MatchPattern(string text, int index)
		{
			if (!this.CaseInsensitive)
			{
				return string.CompareOrdinal(this.Pattern, 0, text, index, this.Pattern.Length) == 0;
			}
			if (text.Length - index < this.Pattern.Length)
			{
				return false;
			}
			TextInfo textInfo = this._culture.TextInfo;
			for (int i = 0; i < this.Pattern.Length; i++)
			{
				if (textInfo.ToLower(text[index + i]) != this.Pattern[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool IsMatch(string text, int index, int beglimit, int endlimit)
		{
			if (!this.RightToLeft)
			{
				return index >= beglimit && endlimit - index >= this.Pattern.Length && this.MatchPattern(text, index);
			}
			return index <= endlimit && index - beglimit >= this.Pattern.Length && this.MatchPattern(text, index - this.Pattern.Length);
		}

		public int Scan(string text, int index, int beglimit, int endlimit)
		{
			int num;
			int num2;
			int num3;
			int num4;
			int num5;
			if (!this.RightToLeft)
			{
				num = this.Pattern.Length;
				num2 = this.Pattern.Length - 1;
				num3 = 0;
				num4 = index + num - 1;
				num5 = 1;
			}
			else
			{
				num = -this.Pattern.Length;
				num2 = 0;
				num3 = -num - 1;
				num4 = index + num;
				num5 = -1;
			}
			char c = this.Pattern[num2];
			IL_0058:
			while (num4 < endlimit && num4 >= beglimit)
			{
				char c2 = text[num4];
				if (this.CaseInsensitive)
				{
					c2 = this._culture.TextInfo.ToLower(c2);
				}
				if (c2 != c)
				{
					int num6;
					int[] array;
					if (c2 < '\u0080')
					{
						num6 = this.NegativeASCII[(int)c2];
					}
					else if (this.NegativeUnicode != null && (array = this.NegativeUnicode[(int)(c2 >> 8)]) != null)
					{
						num6 = array[(int)(c2 & 'ÿ')];
					}
					else
					{
						num6 = num;
					}
					num4 += num6;
				}
				else
				{
					int num7 = num4;
					int num8 = num2;
					while (num8 != num3)
					{
						num8 -= num5;
						num7 -= num5;
						c2 = text[num7];
						if (this.CaseInsensitive)
						{
							c2 = this._culture.TextInfo.ToLower(c2);
						}
						if (c2 != this.Pattern[num8])
						{
							int num6 = this.Positive[num8];
							if ((c2 & 'ﾀ') == '\0')
							{
								num7 = num8 - num2 + this.NegativeASCII[(int)c2];
							}
							else
							{
								int[] array;
								if (this.NegativeUnicode == null || (array = this.NegativeUnicode[(int)(c2 >> 8)]) == null)
								{
									num4 += num6;
									goto IL_0058;
								}
								num7 = num8 - num2 + array[(int)(c2 & 'ÿ')];
							}
							if (this.RightToLeft ? (num7 < num6) : (num7 > num6))
							{
								num6 = num7;
							}
							num4 += num6;
							goto IL_0058;
						}
					}
					if (!this.RightToLeft)
					{
						return num7;
					}
					return num7 + 1;
				}
			}
			return -1;
		}

		public readonly int[] Positive;

		public readonly int[] NegativeASCII;

		public readonly int[][] NegativeUnicode;

		public readonly string Pattern;

		public readonly int LowASCII;

		public readonly int HighASCII;

		public readonly bool RightToLeft;

		public readonly bool CaseInsensitive;

		private readonly CultureInfo _culture;
	}
}
