using System;
using System.Globalization;

namespace System.Xml.Xsl.Runtime
{
	internal static class CharUtil
	{
		public static bool IsAlphaNumeric(char ch)
		{
			int unicodeCategory = (int)char.GetUnicodeCategory(ch);
			return unicodeCategory <= 4 || (unicodeCategory <= 10 && unicodeCategory >= 8);
		}

		public static bool IsDecimalDigitOne(char ch)
		{
			return char.GetUnicodeCategory(ch -= '\u0001') == UnicodeCategory.DecimalDigitNumber && char.GetNumericValue(ch) == 0.0;
		}
	}
}
