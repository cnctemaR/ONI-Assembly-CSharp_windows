using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Newtonsoft.Json.Utilities
{
	internal static class JavaScriptUtils
	{
		static JavaScriptUtils()
		{
			IList<char> list = new List<char> { '\n', '\r', '\t', '\\', '\f', '\b' };
			for (int i = 0; i < 32; i++)
			{
				list.Add((char)i);
			}
			foreach (char c in list.Union<char>(new char[] { '\'' }))
			{
				JavaScriptUtils.SingleQuoteCharEscapeFlags[(int)c] = true;
			}
			foreach (char c2 in list.Union<char>(new char[] { '"' }))
			{
				JavaScriptUtils.DoubleQuoteCharEscapeFlags[(int)c2] = true;
			}
			foreach (char c3 in list.Union<char>(new char[] { '"', '\'', '<', '>', '&' }))
			{
				JavaScriptUtils.HtmlCharEscapeFlags[(int)c3] = true;
			}
		}

		public static bool[] GetCharEscapeFlags(StringEscapeHandling stringEscapeHandling, char quoteChar)
		{
			if (stringEscapeHandling == StringEscapeHandling.EscapeHtml)
			{
				return JavaScriptUtils.HtmlCharEscapeFlags;
			}
			if (quoteChar == '"')
			{
				return JavaScriptUtils.DoubleQuoteCharEscapeFlags;
			}
			return JavaScriptUtils.SingleQuoteCharEscapeFlags;
		}

		public static bool ShouldEscapeJavaScriptString(string s, bool[] charEscapeFlags)
		{
			if (s == null)
			{
				return false;
			}
			foreach (char c in s)
			{
				if ((int)c >= charEscapeFlags.Length || charEscapeFlags[(int)c])
				{
					return true;
				}
			}
			return false;
		}

		public static void WriteEscapedJavaScriptString(TextWriter writer, string s, char delimiter, bool appendDelimiters, bool[] charEscapeFlags, StringEscapeHandling stringEscapeHandling, ref char[] writeBuffer)
		{
			if (appendDelimiters)
			{
				writer.Write(delimiter);
			}
			if (s != null)
			{
				int num = 0;
				for (int i = 0; i < s.Length; i++)
				{
					char c = s[i];
					if ((int)c >= charEscapeFlags.Length || charEscapeFlags[(int)c])
					{
						char c2 = c;
						string text;
						if (c2 <= '\\')
						{
							switch (c2)
							{
							case '\b':
								text = "\\b";
								break;
							case '\t':
								text = "\\t";
								break;
							case '\n':
								text = "\\n";
								break;
							case '\v':
								goto IL_00D4;
							case '\f':
								text = "\\f";
								break;
							case '\r':
								text = "\\r";
								break;
							default:
								if (c2 != '\\')
								{
									goto IL_00D4;
								}
								text = "\\\\";
								break;
							}
						}
						else if (c2 != '\u0085')
						{
							switch (c2)
							{
							case '\u2028':
								text = "\\u2028";
								break;
							case '\u2029':
								text = "\\u2029";
								break;
							default:
								goto IL_00D4;
							}
						}
						else
						{
							text = "\\u0085";
						}
						IL_0125:
						if (text == null)
						{
							goto IL_01BC;
						}
						bool flag = string.Equals(text, "!");
						if (i > num)
						{
							int num2 = i - num + (flag ? 6 : 0);
							int num3 = (flag ? 6 : 0);
							if (writeBuffer == null || writeBuffer.Length < num2)
							{
								char[] array = new char[num2];
								if (flag)
								{
									Array.Copy(writeBuffer, array, 6);
								}
								writeBuffer = array;
							}
							s.CopyTo(num, writeBuffer, num3, num2 - num3);
							writer.Write(writeBuffer, num3, num2 - num3);
						}
						num = i + 1;
						if (!flag)
						{
							writer.Write(text);
							goto IL_01BC;
						}
						writer.Write(writeBuffer, 0, 6);
						goto IL_01BC;
						IL_00D4:
						if ((int)c >= charEscapeFlags.Length && stringEscapeHandling != StringEscapeHandling.EscapeNonAscii)
						{
							text = null;
							goto IL_0125;
						}
						if (c == '\'' && stringEscapeHandling != StringEscapeHandling.EscapeHtml)
						{
							text = "\\'";
							goto IL_0125;
						}
						if (c == '"' && stringEscapeHandling != StringEscapeHandling.EscapeHtml)
						{
							text = "\\\"";
							goto IL_0125;
						}
						if (writeBuffer == null)
						{
							writeBuffer = new char[6];
						}
						StringUtils.ToCharAsUnicode(c, writeBuffer);
						text = "!";
						goto IL_0125;
					}
					IL_01BC:;
				}
				if (num == 0)
				{
					writer.Write(s);
				}
				else
				{
					int num4 = s.Length - num;
					if (writeBuffer == null || writeBuffer.Length < num4)
					{
						writeBuffer = new char[num4];
					}
					s.CopyTo(num, writeBuffer, 0, num4);
					writer.Write(writeBuffer, 0, num4);
				}
			}
			if (appendDelimiters)
			{
				writer.Write(delimiter);
			}
		}

		public static string ToEscapedJavaScriptString(string value, char delimiter, bool appendDelimiters)
		{
			return JavaScriptUtils.ToEscapedJavaScriptString(value, delimiter, appendDelimiters, StringEscapeHandling.Default);
		}

		public static string ToEscapedJavaScriptString(string value, char delimiter, bool appendDelimiters, StringEscapeHandling stringEscapeHandling)
		{
			bool[] charEscapeFlags = JavaScriptUtils.GetCharEscapeFlags(stringEscapeHandling, delimiter);
			string text;
			using (StringWriter stringWriter = StringUtils.CreateStringWriter(StringUtils.GetLength(value) ?? 16))
			{
				char[] array = null;
				JavaScriptUtils.WriteEscapedJavaScriptString(stringWriter, value, delimiter, appendDelimiters, charEscapeFlags, stringEscapeHandling, ref array);
				text = stringWriter.ToString();
			}
			return text;
		}

		private const string EscapedUnicodeText = "!";

		internal static readonly bool[] SingleQuoteCharEscapeFlags = new bool[128];

		internal static readonly bool[] DoubleQuoteCharEscapeFlags = new bool[128];

		internal static readonly bool[] HtmlCharEscapeFlags = new bool[128];
	}
}
