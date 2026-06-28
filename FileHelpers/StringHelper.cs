using System;
using System.Globalization;
using System.Text;

namespace FileHelpers
{
	internal static class StringHelper
	{
		internal static ExtractedInfo ExtractQuotedString(LineInfo line, char quoteChar, bool allowMultiline)
		{
			if (line.IsEOL())
			{
				throw new BadUsageException("An empty String found. This can not be parsed like a QuotedString try to use SafeExtractQuotedString");
			}
			if (line.mLineStr[line.mCurrentPos] != quoteChar)
			{
				throw new BadUsageException("The source string does not begin with the quote char: " + quoteChar);
			}
			StringBuilder stringBuilder = new StringBuilder(32);
			bool flag = false;
			int i = line.mCurrentPos + 1;
			while (line.mLineStr != null)
			{
				while (i < line.mLineStr.Length)
				{
					if (line.mLineStr[i] == quoteChar)
					{
						if (flag)
						{
							stringBuilder.Append(quoteChar);
							flag = false;
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						if (flag)
						{
							line.mCurrentPos = i;
							return new ExtractedInfo(stringBuilder.ToString());
						}
						stringBuilder.Append(line.mLineStr[i]);
					}
					i++;
				}
				if (flag)
				{
					line.mCurrentPos = i;
					return new ExtractedInfo(stringBuilder.ToString());
				}
				if (!allowMultiline)
				{
					throw new BadUsageException("The current field has an unclosed quoted string. Complete line: " + stringBuilder.ToString());
				}
				line.ReadNextLine();
				stringBuilder.Append(StringHelper.NewLine);
				i = 0;
			}
			throw new BadUsageException("The current field has an unclosed quoted string. Complete Filed String: " + stringBuilder.ToString());
		}

		internal static void CreateQuotedString(StringBuilder sb, string source, char quoteChar)
		{
			if (source == null)
			{
				source = string.Empty;
			}
			string text = quoteChar.ToString();
			string text2 = source.Replace(text, text + text);
			sb.Append(quoteChar);
			sb.Append(text2);
			sb.Append(quoteChar);
		}

		internal static string RemoveBlanks(string source)
		{
			int num = 0;
			while (num < source.Length && char.IsWhiteSpace(source[num]))
			{
				num++;
			}
			if (num >= source.Length)
			{
				return string.Empty;
			}
			if (source[num] != '+' && source[num] != '-')
			{
				return source;
			}
			num++;
			if (!char.IsWhiteSpace(source[num]))
			{
				return source;
			}
			StringBuilder stringBuilder = new StringBuilder(source[num - 1].ToString(), source.Length - num);
			num++;
			while (num < source.Length && char.IsWhiteSpace(source[num]))
			{
				num++;
			}
			if (num < source.Length)
			{
				stringBuilder.Append(source.Substring(num));
			}
			return stringBuilder.ToString();
		}

		internal static CompareInfo CreateComparer()
		{
			if (StringHelper.mCulture == null)
			{
				StringHelper.mCulture = CultureInfo.InvariantCulture;
			}
			return StringHelper.mCulture.CompareInfo;
		}

		internal static string ReplaceRecursive(string original, string oldValue, string newValue)
		{
			string text = original.Replace(oldValue, newValue);
			int num = 0;
			string text2;
			do
			{
				num++;
				text2 = text;
				text = text2.Replace(oldValue, newValue);
			}
			while (text2 != text || num > 1000);
			return text;
		}

		internal static string ToValidIdentifier(string original)
		{
			if (original.Length == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(original.Length + 1);
			if (!char.IsLetter(original[0]) && original[0] != '_')
			{
				stringBuilder.Append('_');
			}
			foreach (char c in original)
			{
				if (char.IsLetterOrDigit(c) || c == '_')
				{
					stringBuilder.Append(c);
				}
				else
				{
					stringBuilder.Append('_');
				}
			}
			string text = StringHelper.ReplaceRecursive(stringBuilder.ToString(), "__", "_").Trim(new char[] { '_' });
			if (text.Length == 0)
			{
				return "_";
			}
			if (char.IsDigit(text[0]))
			{
				text = "_" + text;
			}
			return text;
		}

		public static string ReplaceIgnoringCase(string original, string oldValue, string newValue)
		{
			return StringHelper.Replace(original, oldValue, newValue, StringComparison.OrdinalIgnoreCase);
		}

		public static string Replace(string original, string oldValue, string newValue, StringComparison comparisionType)
		{
			string text = original;
			if (!string.IsNullOrEmpty(oldValue))
			{
				int num = -1;
				int num2 = 0;
				StringBuilder stringBuilder = new StringBuilder(original.Length);
				while ((num = original.IndexOf(oldValue, num + 1, comparisionType)) >= 0)
				{
					stringBuilder.Append(original, num2, num - num2);
					stringBuilder.Append(newValue);
					num2 = num + oldValue.Length;
				}
				stringBuilder.Append(original, num2, original.Length - num2);
				text = stringBuilder.ToString();
			}
			return text;
		}

		public static bool IsNullOrWhiteSpace(string value)
		{
			if (value == null)
			{
				return true;
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (!char.IsWhiteSpace(value[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool StartsWithIgnoringWhiteSpaces(string source, string value, StringComparison comparisonType)
		{
			if (value == null)
			{
				return false;
			}
			int num = 0;
			int num2 = source.Length;
			while (num < num2 && char.IsWhiteSpace(source[num]))
			{
				num++;
			}
			num2 -= num;
			if (num2 < value.Length)
			{
				return false;
			}
			num2 = value.Length;
			return source.IndexOf(value, num, num2, comparisonType) == num;
		}

		internal static readonly string NewLine = Environment.NewLine;

		private static CultureInfo mCulture;
	}
}
