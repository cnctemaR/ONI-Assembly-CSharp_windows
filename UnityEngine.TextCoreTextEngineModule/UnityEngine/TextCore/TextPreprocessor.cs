using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;

namespace UnityEngine.TextCore
{
	[global::System.Runtime.CompilerServices.Nullable(0)]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[global::System.Runtime.CompilerServices.NullableContext(1)]
	internal static class TextPreprocessor
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static void PreProcessString(ref string text, PreProcessFlags flags, [global::System.Runtime.CompilerServices.Nullable(2)] TextSettings textSettings)
		{
			bool flag = string.IsNullOrEmpty(text);
			if (!flag)
			{
				bool flag2 = ((textSettings != null) ? textSettings.defaultStyleSheet : null) != null && RichTextTagParser.ContainsStyleTags(text);
				if (flag2)
				{
					text = TextPreprocessor.ReplaceStyleTags(text, textSettings);
				}
				bool flag3 = flags == PreProcessFlags.None;
				if (!flag3)
				{
					bool flag4 = (flags & PreProcessFlags.CollapseWhiteSpaces) > PreProcessFlags.None;
					bool flag5 = (flags & PreProcessFlags.ParseEscapeSequences) > PreProcessFlags.None;
					PreProcessFlags preProcessFlags = PreProcessFlags.None;
					bool flag6 = text.IndexOfAny(new char[] { ' ', '\t', '\r', '\n', '\v' }) != -1;
					if (flag6)
					{
						preProcessFlags |= PreProcessFlags.CollapseWhiteSpaces;
					}
					bool flag7 = text.IndexOf('\\') != -1;
					if (flag7)
					{
						preProcessFlags |= PreProcessFlags.ParseEscapeSequences;
					}
					bool flag8 = (flags & preProcessFlags) == PreProcessFlags.None;
					if (!flag8)
					{
						StringBuilder stringBuilder = new StringBuilder(text.Length);
						int i = 0;
						bool flag9 = true;
						while (i < text.Length)
						{
							string text2 = "";
							char c = text[i];
							bool flag10 = flag5 && c == '\\' && i < text.Length - 1;
							if (flag10)
							{
								i++;
								char c2 = text[i];
								bool flag11 = true;
								char c3 = c2;
								char c4 = c3;
								if (c4 != 'U')
								{
									if (c4 != '\\')
									{
										switch (c4)
										{
										case 'n':
											text2 = "\n";
											goto IL_026A;
										case 'r':
											text2 = "\r";
											goto IL_026A;
										case 't':
											text2 = "\t";
											goto IL_026A;
										case 'u':
										{
											bool flag12 = i + 4 < text.Length;
											if (flag12)
											{
												string text3 = text.Substring(i + 1, 4);
												uint num;
												bool flag13 = uint.TryParse(text3, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num);
												if (flag13)
												{
													text2 = Convert.ToChar(num).ToString();
													i += 4;
												}
												else
												{
													flag11 = false;
												}
											}
											else
											{
												flag11 = false;
											}
											goto IL_026A;
										}
										case 'v':
											text2 = "\v";
											goto IL_026A;
										}
										flag11 = false;
									}
									else
									{
										text2 = "\\";
									}
								}
								else
								{
									bool flag14 = i + 8 < text.Length;
									if (flag14)
									{
										string text4 = text.Substring(i + 1, 8);
										uint num2;
										bool flag15 = uint.TryParse(text4, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num2);
										if (flag15)
										{
											text2 = char.ConvertFromUtf32((int)num2);
											i += 8;
										}
										else
										{
											flag11 = false;
										}
									}
									else
									{
										flag11 = false;
									}
								}
								IL_026A:
								bool flag16 = !flag11;
								if (flag16)
								{
									stringBuilder.Append('\\');
									text2 = c2.ToString();
								}
							}
							else
							{
								text2 = c.ToString();
							}
							bool flag17 = text2.Length == 1 && char.IsWhiteSpace(text2[0]);
							bool flag18 = flag4 && flag17;
							if (flag18)
							{
								bool flag19 = text2 == "\n";
								if (flag19)
								{
									bool flag20 = stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == ' ';
									if (flag20)
									{
										StringBuilder stringBuilder2 = stringBuilder;
										int length = stringBuilder2.Length;
										stringBuilder2.Length = length - 1;
									}
									stringBuilder.Append('\n');
									flag9 = true;
								}
								else
								{
									bool flag21 = !flag9;
									if (flag21)
									{
										stringBuilder.Append(' ');
										flag9 = true;
									}
								}
							}
							else
							{
								stringBuilder.Append(text2);
								flag9 = flag17;
							}
							i++;
						}
						bool flag22 = flag4 && stringBuilder.Length > 0;
						if (flag22)
						{
							int num3 = stringBuilder.Length - 1;
							while (num3 >= 0 && char.IsWhiteSpace(stringBuilder[num3]) && stringBuilder[num3] != '\n')
							{
								num3--;
							}
							stringBuilder.Length = num3 + 1;
						}
						text = stringBuilder.ToString();
					}
				}
			}
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		private unsafe static int GetStyleHashCode(ReadOnlySpan<char> text)
		{
			int num = 0;
			for (int i = 0; i < text.Length; i++)
			{
				num = ((num << 5) + num) ^ (int)TextUtilities.ToUpperFast((char)(*text[i]));
			}
			return num;
		}

		[return: global::System.Runtime.CompilerServices.Nullable(2)]
		private static TextStyle GetStyle(TextSettings textSettings, int hashCode)
		{
			TextStyleSheet defaultStyleSheet = textSettings.defaultStyleSheet;
			bool flag = defaultStyleSheet == null;
			TextStyle textStyle;
			if (flag)
			{
				textStyle = null;
			}
			else
			{
				textStyle = defaultStyleSheet.GetStyle(hashCode);
			}
			return textStyle;
		}

		internal static string ReplaceStyleTags(string text, TextSettings textSettings)
		{
			bool flag = string.IsNullOrEmpty(text);
			string text2;
			if (flag)
			{
				text2 = text;
			}
			else
			{
				ReadOnlySpan<char> readOnlySpan = text.AsSpan();
				ReadOnlySpan<char> readOnlySpan2 = "<style=\"".AsSpan();
				ReadOnlySpan<char> readOnlySpan3 = "</style>".AsSpan();
				StringBuilder stringBuilder = new StringBuilder(text.Length);
				List<TextStyle> list = new List<TextStyle>(4);
				int num = 0;
				ReadOnlySpan<char> readOnlySpan4;
				for (;;)
				{
					readOnlySpan4 = readOnlySpan.Slice(num);
					int num2 = readOnlySpan4.IndexOf('<');
					bool flag2 = num2 == -1;
					if (flag2)
					{
						break;
					}
					bool flag3 = num2 > 0;
					if (flag3)
					{
						stringBuilder.Append(readOnlySpan4.Slice(0, num2));
					}
					num += num2;
					readOnlySpan4 = readOnlySpan.Slice(num);
					bool flag4 = readOnlySpan4.StartsWith<char>(readOnlySpan2);
					if (flag4)
					{
						int length = readOnlySpan2.Length;
						int num3 = readOnlySpan4.Slice(length).IndexOf('"');
						bool flag5 = num3 != -1;
						if (flag5)
						{
							int styleHashCode = TextPreprocessor.GetStyleHashCode(readOnlySpan4.Slice(length, num3));
							TextStyle style = TextPreprocessor.GetStyle(textSettings, styleHashCode);
							bool flag6 = style != null;
							if (flag6)
							{
								int num4 = length + num3 + 1;
								int num5 = readOnlySpan4.Slice(num4).IndexOf('>');
								bool flag7 = num5 != -1;
								if (flag7)
								{
									stringBuilder.Append(style.styleOpeningDefinition);
									list.Add(style);
									num += num4 + num5 + 1;
									continue;
								}
							}
						}
					}
					else
					{
						bool flag8 = readOnlySpan4.StartsWith<char>(readOnlySpan3);
						if (flag8)
						{
							bool flag9 = list.Count > 0;
							if (flag9)
							{
								int num6 = list.Count - 1;
								TextStyle textStyle = list[num6];
								list.RemoveAt(num6);
								stringBuilder.Append(textStyle.styleClosingDefinition);
								num += readOnlySpan3.Length;
								continue;
							}
						}
					}
					stringBuilder.Append('<');
					num++;
				}
				stringBuilder.Append(readOnlySpan4);
				text2 = stringBuilder.ToString();
			}
			return text2;
		}

		private const char k_DoubleQuotes = '"';

		private const char k_GreaterThan = '>';

		private const char k_LessThan = '<';

		private const string k_StyleOpenTag = "<style=\"";

		private const string k_StyleCloseTag = "</style>";
	}
}
