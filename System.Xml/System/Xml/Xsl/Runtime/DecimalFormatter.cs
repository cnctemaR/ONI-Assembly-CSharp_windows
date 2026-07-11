using System;
using System.Globalization;
using System.Text;

namespace System.Xml.Xsl.Runtime
{
	internal class DecimalFormatter
	{
		public DecimalFormatter(string formatPicture, DecimalFormat decimalFormat)
		{
			if (formatPicture.Length == 0)
			{
				throw XsltException.Create("Format cannot be empty.", Array.Empty<string>());
			}
			this.zeroDigit = decimalFormat.zeroDigit;
			this.posFormatInfo = (NumberFormatInfo)decimalFormat.info.Clone();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			char c = this.posFormatInfo.NumberDecimalSeparator[0];
			char c2 = this.posFormatInfo.NumberGroupSeparator[0];
			char c3 = this.posFormatInfo.PercentSymbol[0];
			char c4 = this.posFormatInfo.PerMilleSymbol[0];
			int num = 0;
			int num2 = -1;
			int num3 = -1;
			int num4;
			for (int i = 0; i < formatPicture.Length; i++)
			{
				char c5 = formatPicture[i];
				if (c5 == decimalFormat.digit)
				{
					if (flag3 && flag)
					{
						throw XsltException.Create("Format '{0}' cannot have digit symbol after zero digit symbol before a decimal point.", new string[] { formatPicture });
					}
					num3 = stringBuilder.Length;
					flag6 = (flag4 = true);
					stringBuilder.Append('#');
				}
				else if (c5 == decimalFormat.zeroDigit)
				{
					if (flag4 && !flag)
					{
						throw XsltException.Create("Format '{0}' cannot have zero digit symbol after digit symbol after decimal point.", new string[] { formatPicture });
					}
					num3 = stringBuilder.Length;
					flag6 = (flag3 = true);
					stringBuilder.Append('0');
				}
				else if (c5 == decimalFormat.patternSeparator)
				{
					if (!flag6)
					{
						throw XsltException.Create("Format string should have at least one digit or zero digit.", Array.Empty<string>());
					}
					if (flag2)
					{
						throw XsltException.Create("Format '{0}' has two pattern separators.", new string[] { formatPicture });
					}
					flag2 = true;
					if (num2 < 0)
					{
						num2 = num3 + 1;
					}
					num4 = DecimalFormatter.RemoveTrailingComma(stringBuilder, num, num2);
					if (num4 > 9)
					{
						num4 = 0;
					}
					this.posFormatInfo.NumberGroupSizes = new int[] { num4 };
					if (!flag5)
					{
						this.posFormatInfo.NumberDecimalDigits = 0;
					}
					this.posFormat = stringBuilder.ToString();
					stringBuilder.Length = 0;
					num2 = -1;
					num3 = -1;
					num = 0;
					flag3 = (flag4 = (flag6 = false));
					flag5 = false;
					flag = true;
					this.negFormatInfo = (NumberFormatInfo)decimalFormat.info.Clone();
					this.negFormatInfo.NegativeSign = string.Empty;
				}
				else if (c5 == c)
				{
					if (flag5)
					{
						throw XsltException.Create("Format '{0}' cannot have two decimal separators.", new string[] { formatPicture });
					}
					num2 = stringBuilder.Length;
					flag5 = true;
					flag3 = (flag4 = (flag = false));
					stringBuilder.Append('.');
				}
				else if (c5 == c2)
				{
					num = stringBuilder.Length;
					num3 = num;
					stringBuilder.Append(',');
				}
				else if (c5 == c3)
				{
					stringBuilder.Append('%');
				}
				else if (c5 == c4)
				{
					stringBuilder.Append('‰');
				}
				else if (c5 == '\'')
				{
					int num5 = formatPicture.IndexOf('\'', i + 1);
					if (num5 < 0)
					{
						num5 = formatPicture.Length - 1;
					}
					stringBuilder.Append(formatPicture, i, num5 - i + 1);
					i = num5;
				}
				else
				{
					if ((('0' <= c5 && c5 <= '9') || c5 == '\a') && decimalFormat.zeroDigit != '0')
					{
						stringBuilder.Append('\a');
					}
					if ("0#.,%‰Ee\\'\";".IndexOf(c5) >= 0)
					{
						stringBuilder.Append('\\');
					}
					stringBuilder.Append(c5);
				}
			}
			if (!flag6)
			{
				throw XsltException.Create("Format string should have at least one digit or zero digit.", Array.Empty<string>());
			}
			NumberFormatInfo numberFormatInfo = (flag2 ? this.negFormatInfo : this.posFormatInfo);
			if (num2 < 0)
			{
				num2 = num3 + 1;
			}
			num4 = DecimalFormatter.RemoveTrailingComma(stringBuilder, num, num2);
			if (num4 > 9)
			{
				num4 = 0;
			}
			numberFormatInfo.NumberGroupSizes = new int[] { num4 };
			if (!flag5)
			{
				numberFormatInfo.NumberDecimalDigits = 0;
			}
			if (flag2)
			{
				this.negFormat = stringBuilder.ToString();
				return;
			}
			this.posFormat = stringBuilder.ToString();
		}

		private static int RemoveTrailingComma(StringBuilder builder, int commaIndex, int decimalIndex)
		{
			if (commaIndex > 0 && commaIndex == decimalIndex - 1)
			{
				builder.Remove(decimalIndex - 1, 1);
			}
			else if (decimalIndex > commaIndex)
			{
				return decimalIndex - commaIndex - 1;
			}
			return 0;
		}

		public string Format(double value)
		{
			NumberFormatInfo numberFormatInfo;
			string text;
			if (value < 0.0 && this.negFormatInfo != null)
			{
				numberFormatInfo = this.negFormatInfo;
				text = this.negFormat;
			}
			else
			{
				numberFormatInfo = this.posFormatInfo;
				text = this.posFormat;
			}
			string text2 = value.ToString(text, numberFormatInfo);
			if (this.zeroDigit != '0')
			{
				StringBuilder stringBuilder = new StringBuilder(text2.Length);
				int num = (int)(this.zeroDigit - '0');
				for (int i = 0; i < text2.Length; i++)
				{
					char c = text2[i];
					if (c - '0' <= '\t')
					{
						c += (char)num;
					}
					else if (c == '\a')
					{
						c = text2[++i];
					}
					stringBuilder.Append(c);
				}
				text2 = stringBuilder.ToString();
			}
			return text2;
		}

		public static string Format(double value, string formatPicture, DecimalFormat decimalFormat)
		{
			return new DecimalFormatter(formatPicture, decimalFormat).Format(value);
		}

		private NumberFormatInfo posFormatInfo;

		private NumberFormatInfo negFormatInfo;

		private string posFormat;

		private string negFormat;

		private char zeroDigit;

		private const string ClrSpecialChars = "0#.,%‰Ee\\'\";";

		private const char EscChar = '\a';
	}
}
