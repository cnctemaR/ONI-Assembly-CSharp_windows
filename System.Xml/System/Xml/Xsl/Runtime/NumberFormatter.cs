using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal class NumberFormatter : NumberFormatterBase
	{
		public NumberFormatter(string formatString, int lang, string letterValue, string groupingSeparator, int groupingSize)
		{
			this.formatString = formatString;
			this.lang = lang;
			this.letterValue = letterValue;
			this.groupingSeparator = groupingSeparator;
			this.groupingSize = ((groupingSeparator.Length > 0) ? groupingSize : 0);
			if (formatString == "1" || formatString.Length == 0)
			{
				return;
			}
			this.tokens = new List<TokenInfo>();
			int num = 0;
			bool flag = CharUtil.IsAlphaNumeric(formatString[num]);
			if (flag)
			{
				this.tokens.Add(null);
			}
			for (int i = 0; i <= formatString.Length; i++)
			{
				if (i == formatString.Length || flag != CharUtil.IsAlphaNumeric(formatString[i]))
				{
					if (flag)
					{
						this.tokens.Add(TokenInfo.CreateFormat(formatString, num, i - num));
					}
					else
					{
						this.tokens.Add(TokenInfo.CreateSeparator(formatString, num, i - num));
					}
					num = i;
					flag = !flag;
				}
			}
		}

		public string FormatSequence(IList<XPathItem> val)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (val.Count == 1 && val[0].ValueType == typeof(double))
			{
				double valueAsDouble = val[0].ValueAsDouble;
				if (0.5 > valueAsDouble || valueAsDouble >= double.PositiveInfinity)
				{
					return XPathConvert.DoubleToString(valueAsDouble);
				}
			}
			if (this.tokens == null)
			{
				for (int i = 0; i < val.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append('.');
					}
					this.FormatItem(stringBuilder, val[i], '1', 1);
				}
			}
			else
			{
				int num = this.tokens.Count;
				TokenInfo tokenInfo = this.tokens[0];
				TokenInfo tokenInfo2;
				if (num % 2 == 0)
				{
					tokenInfo2 = null;
				}
				else
				{
					tokenInfo2 = this.tokens[--num];
				}
				TokenInfo tokenInfo3 = ((2 < num) ? this.tokens[num - 2] : NumberFormatter.DefaultSeparator);
				TokenInfo tokenInfo4 = ((0 < num) ? this.tokens[num - 1] : NumberFormatter.DefaultFormat);
				if (tokenInfo != null)
				{
					stringBuilder.Append(tokenInfo.formatString, tokenInfo.startIdx, tokenInfo.length);
				}
				int count = val.Count;
				for (int j = 0; j < count; j++)
				{
					int num2 = j * 2;
					bool flag = num2 < num;
					if (j > 0)
					{
						TokenInfo tokenInfo5 = (flag ? this.tokens[num2] : tokenInfo3);
						stringBuilder.Append(tokenInfo5.formatString, tokenInfo5.startIdx, tokenInfo5.length);
					}
					TokenInfo tokenInfo6 = (flag ? this.tokens[num2 + 1] : tokenInfo4);
					this.FormatItem(stringBuilder, val[j], tokenInfo6.startChar, tokenInfo6.length);
				}
				if (tokenInfo2 != null)
				{
					stringBuilder.Append(tokenInfo2.formatString, tokenInfo2.startIdx, tokenInfo2.length);
				}
			}
			return stringBuilder.ToString();
		}

		private void FormatItem(StringBuilder sb, XPathItem item, char startChar, int length)
		{
			double num;
			if (item.ValueType == typeof(int))
			{
				num = (double)item.ValueAsInt;
			}
			else
			{
				num = XsltFunctions.Round(item.ValueAsDouble);
			}
			char c = '0';
			if (startChar <= 'A')
			{
				if (startChar == '1')
				{
					goto IL_0087;
				}
				if (startChar != 'A')
				{
					goto IL_0082;
				}
			}
			else
			{
				if (startChar != 'I')
				{
					if (startChar == 'a')
					{
						goto IL_0052;
					}
					if (startChar != 'i')
					{
						goto IL_0082;
					}
				}
				if (num <= 32767.0)
				{
					NumberFormatterBase.ConvertToRoman(sb, num, startChar == 'I');
					return;
				}
				goto IL_0087;
			}
			IL_0052:
			if (num <= 2147483647.0)
			{
				NumberFormatterBase.ConvertToAlphabetic(sb, num, startChar, 26);
				return;
			}
			goto IL_0087;
			IL_0082:
			c = startChar - '\u0001';
			IL_0087:
			sb.Append(NumberFormatter.ConvertToDecimal(num, length, c, this.groupingSeparator, this.groupingSize));
		}

		private unsafe static string ConvertToDecimal(double val, int minLen, char zero, string groupSeparator, int groupSize)
		{
			string text = XPathConvert.DoubleToString(val);
			int num = (int)(zero - '0');
			int length = text.Length;
			int num2 = Math.Max(length, minLen);
			char* ptr;
			char c;
			checked
			{
				if (groupSize != 0)
				{
					num2 += (num2 - 1) / groupSize;
				}
				if (num2 == length && num == 0)
				{
					return text;
				}
				if (groupSize == 0 && num == 0)
				{
					return text.PadLeft(num2, zero);
				}
				ptr = stackalloc char[unchecked((UIntPtr)num2) * 2];
				c = ((groupSeparator.Length > 0) ? groupSeparator[0] : ' ');
			}
			fixed (string text2 = text)
			{
				char* ptr2 = text2;
				if (ptr2 != null)
				{
					ptr2 += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr3 = ptr2 + length - 1;
				char* ptr4 = ptr + num2 - 1;
				int num3 = groupSize;
				for (;;)
				{
					*(ptr4--) = ((ptr3 >= ptr2) ? ((char)((int)(*(ptr3--)) + num)) : zero);
					if (ptr4 < ptr)
					{
						break;
					}
					if (--num3 == 0)
					{
						*(ptr4--) = c;
						num3 = groupSize;
					}
				}
			}
			return new string(ptr, 0, num2);
		}

		private string formatString;

		private int lang;

		private string letterValue;

		private string groupingSeparator;

		private int groupingSize;

		private List<TokenInfo> tokens;

		public const char DefaultStartChar = '1';

		private static readonly TokenInfo DefaultFormat = TokenInfo.CreateFormat("0", 0, 1);

		private static readonly TokenInfo DefaultSeparator = TokenInfo.CreateSeparator(".", 0, 1);
	}
}
