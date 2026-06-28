using System;

namespace ArabicSupport
{
	public class ArabicFixer
	{
		public static string Fix(string str)
		{
			return ArabicFixer.Fix(str, false, true);
		}

		public static string Fix(string str, bool rtl)
		{
			string text;
			if (rtl)
			{
				text = ArabicFixer.Fix(str);
			}
			else
			{
				string[] array = str.Split(new char[] { ' ' });
				string text2 = "";
				string text3 = "";
				foreach (string text4 in array)
				{
					if (char.IsLower(text4.ToLower()[text4.Length / 2]))
					{
						text2 = text2 + ArabicFixer.Fix(text3) + text4 + " ";
						text3 = "";
					}
					else
					{
						text3 = text3 + text4 + " ";
					}
				}
				if (text3 != "")
				{
					text2 += ArabicFixer.Fix(text3);
				}
				text = text2;
			}
			return text;
		}

		public static string Fix(string str, bool showTashkeel, bool useHinduNumbers)
		{
			ArabicFixerTool.showTashkeel = showTashkeel;
			ArabicFixerTool.useHinduNumbers = useHinduNumbers;
			if (str.Contains("\n"))
			{
				str = str.Replace("\n", Environment.NewLine);
			}
			string text;
			if (str.Contains(Environment.NewLine))
			{
				string[] array = new string[] { Environment.NewLine };
				string[] array2 = str.Split(array, StringSplitOptions.None);
				if (array2.Length == 0)
				{
					text = ArabicFixerTool.FixLine(str);
				}
				else if (array2.Length == 1)
				{
					text = ArabicFixerTool.FixLine(str);
				}
				else
				{
					string text2 = ArabicFixerTool.FixLine(array2[0]);
					int i = 1;
					if (array2.Length > 1)
					{
						while (i < array2.Length)
						{
							text2 = text2 + Environment.NewLine + ArabicFixerTool.FixLine(array2[i]);
							i++;
						}
					}
					text = text2;
				}
			}
			else
			{
				text = ArabicFixerTool.FixLine(str);
			}
			return text;
		}
	}
}
