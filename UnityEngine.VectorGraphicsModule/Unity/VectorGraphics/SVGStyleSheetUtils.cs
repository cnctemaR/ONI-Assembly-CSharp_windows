using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Unity.VectorGraphics
{
	internal static class SVGStyleSheetUtils
	{
		public static SVGStyleSheet Parse(string cssText)
		{
			SVGStyleSheet svgstyleSheet = new SVGStyleSheet();
			List<string> list = SVGStyleSheetUtils.Tokenize(cssText);
			SVGStyleSheet svgstyleSheet2 = new SVGStyleSheet();
			while (SVGStyleSheetUtils.ParseSelector(list, svgstyleSheet2))
			{
				List<string> list2 = new List<string>(svgstyleSheet.selectors);
				foreach (string text in svgstyleSheet2.selectors)
				{
					bool flag = list2.Contains(text);
					if (flag)
					{
						SVGStyleSheetUtils.CombineProperties(svgstyleSheet[text], svgstyleSheet2[text]);
					}
					else
					{
						svgstyleSheet[text] = svgstyleSheet2[text];
					}
				}
				svgstyleSheet2.Clear();
			}
			return svgstyleSheet;
		}

		public static SVGPropertySheet ParseInline(string cssText)
		{
			List<string> list = SVGStyleSheetUtils.Tokenize(cssText);
			SVGPropertySheet svgpropertySheet = new SVGPropertySheet();
			SVGStyleSheetUtils.ParseProperties(list, svgpropertySheet);
			return svgpropertySheet;
		}

		private static bool ParseSelector(List<string> tokens, SVGStyleSheet sheet)
		{
			bool flag = tokens.Count == 0;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				SVGStyleSheet svgstyleSheet = new SVGStyleSheet();
				bool flag3;
				do
				{
					string text = SVGStyleSheetUtils.PopToken(tokens);
					while (SVGStyleSheetUtils.PeekToken(tokens) != "" && SVGStyleSheetUtils.PeekToken(tokens) != "," && SVGStyleSheetUtils.PeekToken(tokens) != "{")
					{
						text = text + " " + SVGStyleSheetUtils.PopToken(tokens);
					}
					svgstyleSheet[text] = new SVGPropertySheet();
					while (SVGStyleSheetUtils.PeekToken(tokens) == ",")
					{
						SVGStyleSheetUtils.PopToken(tokens);
					}
					flag3 = SVGStyleSheetUtils.PeekToken(tokens) == "" || SVGStyleSheetUtils.PeekToken(tokens) == "{";
				}
				while (!flag3);
				string text2 = SVGStyleSheetUtils.PopToken(tokens);
				bool flag4 = text2 != "{";
				if (flag4)
				{
					Debug.LogError("Invalid CSS selector opening bracket: \"" + text2 + "\"");
					flag2 = false;
				}
				else
				{
					SVGPropertySheet svgpropertySheet = new SVGPropertySheet();
					SVGStyleSheetUtils.ParseProperties(tokens, svgpropertySheet);
					foreach (string text3 in svgstyleSheet.selectors)
					{
						sheet[text3] = SVGStyleSheetUtils.CopyProperties(svgpropertySheet);
					}
					text2 = SVGStyleSheetUtils.PopToken(tokens);
					bool flag5 = text2 != "}";
					if (flag5)
					{
						Debug.LogError("Invalid CSS selector closing bracket: \"" + text2 + "\"");
						flag2 = false;
					}
					else
					{
						flag2 = true;
					}
				}
			}
			return flag2;
		}

		private static void CombineProperties(SVGPropertySheet first, SVGPropertySheet second)
		{
			foreach (string text in second.Keys)
			{
				first[text] = second[text];
			}
		}

		private static SVGPropertySheet CopyProperties(SVGPropertySheet props)
		{
			SVGPropertySheet svgpropertySheet = new SVGPropertySheet();
			foreach (KeyValuePair<string, string> keyValuePair in props)
			{
				svgpropertySheet[keyValuePair.Key] = keyValuePair.Value;
			}
			return svgpropertySheet;
		}

		private static bool ParseProperties(List<string> tokens, SVGPropertySheet props)
		{
			string text;
			string text2;
			while (SVGStyleSheetUtils.ParseProperty(tokens, out text, out text2))
			{
				props[text] = text2;
				while (SVGStyleSheetUtils.PeekToken(tokens) == ";")
				{
					SVGStyleSheetUtils.PopToken(tokens);
				}
			}
			return true;
		}

		private static bool ParseProperty(List<string> tokens, out string name, out string value)
		{
			name = null;
			value = null;
			bool flag = SVGStyleSheetUtils.PeekToken(tokens) == "" || SVGStyleSheetUtils.PeekToken(tokens) == "}";
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				name = SVGStyleSheetUtils.PopToken(tokens);
				string text = SVGStyleSheetUtils.PopToken(tokens);
				bool flag3 = text != ":";
				if (flag3)
				{
					Debug.LogError("Invalid CSS property separator: \"" + text + "\"");
					flag2 = false;
				}
				else
				{
					value = "";
					while (SVGStyleSheetUtils.PeekToken(tokens) != "" && SVGStyleSheetUtils.PeekToken(tokens) != ";" && SVGStyleSheetUtils.PeekToken(tokens) != "}")
					{
						value = ((value == "") ? SVGStyleSheetUtils.PopToken(tokens) : (value + " " + SVGStyleSheetUtils.PopToken(tokens)));
						bool flag4 = SVGStyleSheetUtils.PeekToken(tokens) == "(";
						if (flag4)
						{
							value += SVGStyleSheetUtils.ParseParenValue(tokens);
						}
					}
					flag2 = true;
				}
			}
			return flag2;
		}

		private static string ParseParenValue(List<string> tokens)
		{
			string text = SVGStyleSheetUtils.PopToken(tokens);
			bool flag = text != "(";
			string text2;
			if (flag)
			{
				Debug.LogError("Invaid CSS value opening");
				text2 = "";
			}
			else
			{
				string text3 = text;
				while (SVGStyleSheetUtils.PeekToken(tokens) != "" && SVGStyleSheetUtils.PeekToken(tokens) != ")")
				{
					text3 += SVGStyleSheetUtils.PopToken(tokens);
				}
				bool flag2 = SVGStyleSheetUtils.PeekToken(tokens) != ")";
				if (flag2)
				{
					Debug.LogError("Invaid CSS value closing");
					text2 = "";
				}
				else
				{
					text3 += SVGStyleSheetUtils.PopToken(tokens);
					text2 = text3;
				}
			}
			return text2;
		}

		public static List<string> Tokenize(string cssText)
		{
			List<string> list = new List<string>();
			cssText = cssText.Replace(Environment.NewLine, "");
			cssText = Regex.Replace(cssText, "/\\*.*?\\*/", "");
			cssText = Regex.Replace(cssText, "<!--.*?-->", "");
			int num;
			for (int i = 0; i < cssText.Length; i = num)
			{
				while (i < cssText.Length && SVGStyleSheetUtils.IsWhitespace(cssText[i]))
				{
					i++;
				}
				num = i;
				while (num < cssText.Length && !SVGStyleSheetUtils.IsSeparator(cssText[num]))
				{
					num++;
				}
				bool flag = i == num;
				if (flag)
				{
					bool flag2 = i < cssText.Length;
					if (flag2)
					{
						list.Add(cssText[i].ToString());
					}
					num++;
				}
				else
				{
					list.Add(cssText.Substring(i, num - i));
				}
			}
			return list;
		}

		private static string PeekToken(List<string> tokens)
		{
			bool flag = tokens.Count == 0;
			string text;
			if (flag)
			{
				text = "";
			}
			else
			{
				text = tokens[0];
			}
			return text;
		}

		private static string PopToken(List<string> tokens)
		{
			bool flag = tokens.Count == 0;
			string text;
			if (flag)
			{
				text = "";
			}
			else
			{
				string text2 = tokens[0];
				tokens.RemoveAt(0);
				text = text2;
			}
			return text;
		}

		private static bool IsSeparator(char ch)
		{
			return SVGStyleSheetUtils.IsWhitespace(ch) || ch == ';' || ch == ':' || ch == '{' || ch == '}' || ch == '(' || ch == ')' || ch == ',';
		}

		private static bool IsWhitespace(char ch)
		{
			return ch == ' ' || ch == '\n' || ch == '\t';
		}
	}
}
