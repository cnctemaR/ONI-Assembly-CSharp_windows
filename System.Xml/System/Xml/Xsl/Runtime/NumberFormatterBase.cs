using System;
using System.Text;

namespace System.Xml.Xsl.Runtime
{
	internal class NumberFormatterBase
	{
		public static void ConvertToAlphabetic(StringBuilder sb, double val, char firstChar, int totalChars)
		{
			char[] array = new char[7];
			int num = 7;
			int i;
			int num2;
			for (i = (int)val; i > totalChars; i = num2)
			{
				num2 = --i / totalChars;
				array[--num] = (char)((int)firstChar + (i - num2 * totalChars));
			}
			array[--num] = (char)((int)firstChar + (i - 1));
			sb.Append(array, num, 7 - num);
		}

		public static void ConvertToRoman(StringBuilder sb, double val, bool upperCase)
		{
			int i = (int)val;
			string text = (upperCase ? "IIVIXXLXCCDCM" : "iivixxlxccdcm");
			int num = NumberFormatterBase.RomanDigitValue.Length;
			while (num-- != 0)
			{
				while (i >= NumberFormatterBase.RomanDigitValue[num])
				{
					i -= NumberFormatterBase.RomanDigitValue[num];
					sb.Append(text, num, 1 + (num & 1));
				}
			}
		}

		protected const int MaxAlphabeticValue = 2147483647;

		private const int MaxAlphabeticLength = 7;

		protected const int MaxRomanValue = 32767;

		private const string RomanDigitsUC = "IIVIXXLXCCDCM";

		private const string RomanDigitsLC = "iivixxlxccdcm";

		private static readonly int[] RomanDigitValue = new int[]
		{
			1, 4, 5, 9, 10, 40, 50, 90, 100, 400,
			500, 900, 1000
		};

		private const string hiraganaAiueo = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをん";

		private const string hiraganaIroha = "いろはにほへとちりぬるをわかよたれそつねならむうゐのおくやまけふこえてあさきゆめみしゑひもせす";

		private const string katakanaAiueo = "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲン";

		private const string katakanaIroha = "イロハニホヘトチリヌルヲワカヨタレソツネナラムウヰノオクヤマケフコエテアサキユメミシヱヒモセスン";

		private const string katakanaAiueoHw = "ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜｦﾝ";

		private const string katakanaIrohaHw = "ｲﾛﾊﾆﾎﾍﾄﾁﾘﾇﾙｦﾜｶﾖﾀﾚｿﾂﾈﾅﾗﾑｳヰﾉｵｸﾔﾏｹﾌｺｴﾃｱｻｷﾕﾒﾐｼヱﾋﾓｾｽﾝ";

		private const string cjkIdeographic = "〇一二三四五六七八九";
	}
}
