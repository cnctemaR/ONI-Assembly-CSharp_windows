using System;

namespace FileHelpers.Detection
{
	internal static class QuoteHelper
	{
		public static int CountNumberOfDelimiters(string line, char delimiter, char quotedChar)
		{
			int num = 0;
			string text = line;
			while (!string.IsNullOrEmpty(text))
			{
				if (text.StartsWith(quotedChar.ToString()))
				{
					text = QuoteHelper.DiscardUntilQuotedChar(text, quotedChar);
				}
				else
				{
					int num2 = text.IndexOf(delimiter);
					if (num2 < 0)
					{
						return num;
					}
					num++;
					text = text.Substring(num2 + 1);
				}
			}
			return num;
		}

		private static string DiscardUntilQuotedChar(string line, char quoteChar)
		{
			if (line.StartsWith(quoteChar.ToString()))
			{
				line = line.Substring(1);
			}
			int num = line.IndexOf(quoteChar);
			if (num < 0)
			{
				return string.Empty;
			}
			return line.Substring(num + 1);
		}
	}
}
