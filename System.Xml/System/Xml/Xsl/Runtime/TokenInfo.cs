using System;
using System.Diagnostics;

namespace System.Xml.Xsl.Runtime
{
	internal class TokenInfo
	{
		private TokenInfo()
		{
		}

		[Conditional("DEBUG")]
		public void AssertSeparator(bool isSeparator)
		{
		}

		public static TokenInfo CreateSeparator(string formatString, int startIdx, int tokLen)
		{
			return new TokenInfo
			{
				startIdx = startIdx,
				formatString = formatString,
				length = tokLen
			};
		}

		public static TokenInfo CreateFormat(string formatString, int startIdx, int tokLen)
		{
			TokenInfo tokenInfo = new TokenInfo();
			tokenInfo.formatString = null;
			tokenInfo.length = 1;
			bool flag = false;
			char c = formatString[startIdx];
			if (c <= 'A')
			{
				if (c == '1' || c == 'A')
				{
					goto IL_0089;
				}
			}
			else if (c == 'I' || c == 'a' || c == 'i')
			{
				goto IL_0089;
			}
			if (!CharUtil.IsDecimalDigitOne(c))
			{
				if (CharUtil.IsDecimalDigitOne(c + '\u0001'))
				{
					int num = startIdx;
					do
					{
						tokenInfo.length++;
					}
					while (--tokLen > 0 && c == formatString[++num]);
					if (formatString[num] == (c += '\u0001'))
					{
						goto IL_0089;
					}
				}
				flag = true;
			}
			IL_0089:
			if (tokLen != 1)
			{
				flag = true;
			}
			if (flag)
			{
				tokenInfo.startChar = '1';
				tokenInfo.length = 1;
			}
			else
			{
				tokenInfo.startChar = c;
			}
			return tokenInfo;
		}

		public char startChar;

		public int startIdx;

		public string formatString;

		public int length;
	}
}
