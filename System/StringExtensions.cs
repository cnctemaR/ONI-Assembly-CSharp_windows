using System;

namespace System
{
	internal static class StringExtensions
	{
		internal static string SubstringTrim(this string value, int startIndex)
		{
			return value.SubstringTrim(startIndex, value.Length - startIndex);
		}

		internal static string SubstringTrim(this string value, int startIndex, int length)
		{
			if (length == 0)
			{
				return string.Empty;
			}
			int num = startIndex + length - 1;
			while (startIndex <= num)
			{
				if (!char.IsWhiteSpace(value[startIndex]))
				{
					break;
				}
				startIndex++;
			}
			while (num >= startIndex && char.IsWhiteSpace(value[num]))
			{
				num--;
			}
			int num2 = num - startIndex + 1;
			if (num2 == 0)
			{
				return string.Empty;
			}
			if (num2 != value.Length)
			{
				return value.Substring(startIndex, num2);
			}
			return value;
		}
	}
}
