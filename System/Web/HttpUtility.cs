using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Web.Util;

namespace System.Web
{
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public sealed class HttpUtility
	{
		public static void HtmlAttributeEncode(string s, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			HttpEncoder.Current.HtmlAttributeEncode(s, output);
		}

		public static string HtmlAttributeEncode(string s)
		{
			if (s == null)
			{
				return null;
			}
			string text;
			using (StringWriter stringWriter = new StringWriter())
			{
				HttpEncoder.Current.HtmlAttributeEncode(s, stringWriter);
				text = stringWriter.ToString();
			}
			return text;
		}

		public static string UrlDecode(string str)
		{
			return HttpUtility.UrlDecode(str, Encoding.UTF8);
		}

		private static char[] GetChars(MemoryStream b, Encoding e)
		{
			return e.GetChars(b.GetBuffer(), 0, (int)b.Length);
		}

		private static void WriteCharBytes(IList buf, char ch, Encoding e)
		{
			if (ch > 'ÿ')
			{
				foreach (byte b in e.GetBytes(new char[] { ch }))
				{
					buf.Add(b);
				}
				return;
			}
			buf.Add((byte)ch);
		}

		public static string UrlDecode(string str, Encoding e)
		{
			if (str == null)
			{
				return null;
			}
			if (str.IndexOf('%') == -1 && str.IndexOf('+') == -1)
			{
				return str;
			}
			if (e == null)
			{
				e = Encoding.UTF8;
			}
			long num = (long)str.Length;
			List<byte> list = new List<byte>();
			int num2 = 0;
			while ((long)num2 < num)
			{
				char c = str[num2];
				if (c == '%' && (long)(num2 + 2) < num && str[num2 + 1] != '%')
				{
					int num3;
					if (str[num2 + 1] == 'u' && (long)(num2 + 5) < num)
					{
						num3 = HttpUtility.GetChar(str, num2 + 2, 4);
						if (num3 != -1)
						{
							HttpUtility.WriteCharBytes(list, (char)num3, e);
							num2 += 5;
						}
						else
						{
							HttpUtility.WriteCharBytes(list, '%', e);
						}
					}
					else if ((num3 = HttpUtility.GetChar(str, num2 + 1, 2)) != -1)
					{
						HttpUtility.WriteCharBytes(list, (char)num3, e);
						num2 += 2;
					}
					else
					{
						HttpUtility.WriteCharBytes(list, '%', e);
					}
				}
				else if (c == '+')
				{
					HttpUtility.WriteCharBytes(list, ' ', e);
				}
				else
				{
					HttpUtility.WriteCharBytes(list, c, e);
				}
				num2++;
			}
			byte[] array = list.ToArray();
			return e.GetString(array);
		}

		public static string UrlDecode(byte[] bytes, Encoding e)
		{
			if (bytes == null)
			{
				return null;
			}
			return HttpUtility.UrlDecode(bytes, 0, bytes.Length, e);
		}

		private static int GetInt(byte b)
		{
			if (b >= 48 && b <= 57)
			{
				return (int)(b - 48);
			}
			if (b >= 97 && b <= 102)
			{
				return (int)(b - 97 + 10);
			}
			if (b >= 65 && b <= 70)
			{
				return (int)(b - 65 + 10);
			}
			return -1;
		}

		private static int GetChar(byte[] bytes, int offset, int length)
		{
			int num = 0;
			int num2 = length + offset;
			for (int i = offset; i < num2; i++)
			{
				int @int = HttpUtility.GetInt(bytes[i]);
				if (@int == -1)
				{
					return -1;
				}
				num = (num << 4) + @int;
			}
			return num;
		}

		private static int GetChar(string str, int offset, int length)
		{
			int num = 0;
			int num2 = length + offset;
			for (int i = offset; i < num2; i++)
			{
				char c = str[i];
				if (c > '\u007f')
				{
					return -1;
				}
				int @int = HttpUtility.GetInt((byte)c);
				if (@int == -1)
				{
					return -1;
				}
				num = (num << 4) + @int;
			}
			return num;
		}

		public static string UrlDecode(byte[] bytes, int offset, int count, Encoding e)
		{
			if (bytes == null)
			{
				return null;
			}
			if (count == 0)
			{
				return string.Empty;
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (offset < 0 || offset > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || offset + count > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			StringBuilder stringBuilder = new StringBuilder();
			MemoryStream memoryStream = new MemoryStream();
			int num = count + offset;
			int i = offset;
			while (i < num)
			{
				if (bytes[i] != 37 || i + 2 >= count || bytes[i + 1] == 37)
				{
					goto IL_00EE;
				}
				if (bytes[i + 1] == 117 && i + 5 < num)
				{
					if (memoryStream.Length > 0L)
					{
						stringBuilder.Append(HttpUtility.GetChars(memoryStream, e));
						memoryStream.SetLength(0L);
					}
					int num2 = HttpUtility.GetChar(bytes, i + 2, 4);
					if (num2 == -1)
					{
						goto IL_00EE;
					}
					stringBuilder.Append((char)num2);
					i += 5;
				}
				else
				{
					int num2;
					if ((num2 = HttpUtility.GetChar(bytes, i + 1, 2)) == -1)
					{
						goto IL_00EE;
					}
					memoryStream.WriteByte((byte)num2);
					i += 2;
				}
				IL_012C:
				i++;
				continue;
				IL_00EE:
				if (memoryStream.Length > 0L)
				{
					stringBuilder.Append(HttpUtility.GetChars(memoryStream, e));
					memoryStream.SetLength(0L);
				}
				if (bytes[i] == 43)
				{
					stringBuilder.Append(' ');
					goto IL_012C;
				}
				stringBuilder.Append((char)bytes[i]);
				goto IL_012C;
			}
			if (memoryStream.Length > 0L)
			{
				stringBuilder.Append(HttpUtility.GetChars(memoryStream, e));
			}
			return stringBuilder.ToString();
		}

		public static byte[] UrlDecodeToBytes(byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			return HttpUtility.UrlDecodeToBytes(bytes, 0, bytes.Length);
		}

		public static byte[] UrlDecodeToBytes(string str)
		{
			return HttpUtility.UrlDecodeToBytes(str, Encoding.UTF8);
		}

		public static byte[] UrlDecodeToBytes(string str, Encoding e)
		{
			if (str == null)
			{
				return null;
			}
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			return HttpUtility.UrlDecodeToBytes(e.GetBytes(str));
		}

		public static byte[] UrlDecodeToBytes(byte[] bytes, int offset, int count)
		{
			if (bytes == null)
			{
				return null;
			}
			if (count == 0)
			{
				return new byte[0];
			}
			int num = bytes.Length;
			if (offset < 0 || offset >= num)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || offset > num - count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			MemoryStream memoryStream = new MemoryStream();
			int num2 = offset + count;
			for (int i = offset; i < num2; i++)
			{
				char c = (char)bytes[i];
				if (c == '+')
				{
					c = ' ';
				}
				else if (c == '%' && i < num2 - 2)
				{
					int @char = HttpUtility.GetChar(bytes, i + 1, 2);
					if (@char != -1)
					{
						c = (char)@char;
						i += 2;
					}
				}
				memoryStream.WriteByte((byte)c);
			}
			return memoryStream.ToArray();
		}

		public static string UrlEncode(string str)
		{
			return HttpUtility.UrlEncode(str, Encoding.UTF8);
		}

		public static string UrlEncode(string str, Encoding e)
		{
			if (str == null)
			{
				return null;
			}
			if (str == string.Empty)
			{
				return string.Empty;
			}
			bool flag = false;
			int length = str.Length;
			for (int i = 0; i < length; i++)
			{
				char c = str[i];
				if ((c < '0' || (c < 'A' && c > '9') || (c > 'Z' && c < 'a') || c > 'z') && !HttpEncoder.NotEncoded(c))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return str;
			}
			byte[] array = new byte[e.GetMaxByteCount(str.Length)];
			int bytes = e.GetBytes(str, 0, str.Length, array, 0);
			return Encoding.ASCII.GetString(HttpUtility.UrlEncodeToBytes(array, 0, bytes));
		}

		public static string UrlEncode(byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			if (bytes.Length == 0)
			{
				return string.Empty;
			}
			return Encoding.ASCII.GetString(HttpUtility.UrlEncodeToBytes(bytes, 0, bytes.Length));
		}

		public static string UrlEncode(byte[] bytes, int offset, int count)
		{
			if (bytes == null)
			{
				return null;
			}
			if (bytes.Length == 0)
			{
				return string.Empty;
			}
			return Encoding.ASCII.GetString(HttpUtility.UrlEncodeToBytes(bytes, offset, count));
		}

		public static byte[] UrlEncodeToBytes(string str)
		{
			return HttpUtility.UrlEncodeToBytes(str, Encoding.UTF8);
		}

		public static byte[] UrlEncodeToBytes(string str, Encoding e)
		{
			if (str == null)
			{
				return null;
			}
			if (str.Length == 0)
			{
				return new byte[0];
			}
			byte[] bytes = e.GetBytes(str);
			return HttpUtility.UrlEncodeToBytes(bytes, 0, bytes.Length);
		}

		public static byte[] UrlEncodeToBytes(byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			if (bytes.Length == 0)
			{
				return new byte[0];
			}
			return HttpUtility.UrlEncodeToBytes(bytes, 0, bytes.Length);
		}

		public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
		{
			if (bytes == null)
			{
				return null;
			}
			return HttpEncoder.Current.UrlEncode(bytes, offset, count);
		}

		public static string UrlEncodeUnicode(string str)
		{
			if (str == null)
			{
				return null;
			}
			return Encoding.ASCII.GetString(HttpUtility.UrlEncodeUnicodeToBytes(str));
		}

		public static byte[] UrlEncodeUnicodeToBytes(string str)
		{
			if (str == null)
			{
				return null;
			}
			if (str.Length == 0)
			{
				return new byte[0];
			}
			MemoryStream memoryStream = new MemoryStream(str.Length);
			for (int i = 0; i < str.Length; i++)
			{
				HttpEncoder.UrlEncodeChar(str[i], memoryStream, true);
			}
			return memoryStream.ToArray();
		}

		public static string HtmlDecode(string s)
		{
			if (s == null)
			{
				return null;
			}
			string text;
			using (StringWriter stringWriter = new StringWriter())
			{
				HttpEncoder.Current.HtmlDecode(s, stringWriter);
				text = stringWriter.ToString();
			}
			return text;
		}

		public static void HtmlDecode(string s, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (!string.IsNullOrEmpty(s))
			{
				HttpEncoder.Current.HtmlDecode(s, output);
			}
		}

		public static string HtmlEncode(string s)
		{
			if (s == null)
			{
				return null;
			}
			string text;
			using (StringWriter stringWriter = new StringWriter())
			{
				HttpEncoder.Current.HtmlEncode(s, stringWriter);
				text = stringWriter.ToString();
			}
			return text;
		}

		public static void HtmlEncode(string s, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (!string.IsNullOrEmpty(s))
			{
				HttpEncoder.Current.HtmlEncode(s, output);
			}
		}

		public static string HtmlEncode(object value)
		{
			if (value == null)
			{
				return null;
			}
			return HttpUtility.HtmlEncode(value.ToString());
		}

		public static string JavaScriptStringEncode(string value)
		{
			return HttpUtility.JavaScriptStringEncode(value, false);
		}

		public static string JavaScriptStringEncode(string value, bool addDoubleQuotes)
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!addDoubleQuotes)
				{
					return string.Empty;
				}
				return "\"\"";
			}
			else
			{
				int length = value.Length;
				bool flag = false;
				for (int i = 0; i < length; i++)
				{
					char c = value[i];
					if ((c >= '\0' && c <= '\u001f') || c == '"' || c == '\'' || c == '<' || c == '>' || c == '\\')
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (addDoubleQuotes)
					{
						stringBuilder.Append('"');
					}
					for (int j = 0; j < length; j++)
					{
						char c = value[j];
						if ((c >= '\0' && c <= '\a') || (c == '\v' || (c >= '\u000e' && c <= '\u001f')) || c == '\'' || c == '<' || c == '>')
						{
							stringBuilder.AppendFormat("\\u{0:x4}", (int)c);
						}
						else
						{
							int num = (int)c;
							switch (num)
							{
							case 8:
								stringBuilder.Append("\\b");
								goto IL_0174;
							case 9:
								stringBuilder.Append("\\t");
								goto IL_0174;
							case 10:
								stringBuilder.Append("\\n");
								goto IL_0174;
							case 11:
								break;
							case 12:
								stringBuilder.Append("\\f");
								goto IL_0174;
							case 13:
								stringBuilder.Append("\\r");
								goto IL_0174;
							default:
								if (num == 34)
								{
									stringBuilder.Append("\\\"");
									goto IL_0174;
								}
								if (num == 92)
								{
									stringBuilder.Append("\\\\");
									goto IL_0174;
								}
								break;
							}
							stringBuilder.Append(c);
						}
						IL_0174:;
					}
					if (addDoubleQuotes)
					{
						stringBuilder.Append('"');
					}
					return stringBuilder.ToString();
				}
				if (!addDoubleQuotes)
				{
					return value;
				}
				return "\"" + value + "\"";
			}
		}

		public static string UrlPathEncode(string str)
		{
			return HttpEncoder.Current.UrlPathEncode(str);
		}

		public static NameValueCollection ParseQueryString(string query)
		{
			return HttpUtility.ParseQueryString(query, Encoding.UTF8);
		}

		public static NameValueCollection ParseQueryString(string query, Encoding encoding)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (query.Length == 0 || (query.Length == 1 && query[0] == '?'))
			{
				return new HttpUtility.HttpQSCollection();
			}
			if (query[0] == '?')
			{
				query = query.Substring(1);
			}
			NameValueCollection nameValueCollection = new HttpUtility.HttpQSCollection();
			HttpUtility.ParseQueryString(query, encoding, nameValueCollection);
			return nameValueCollection;
		}

		internal static void ParseQueryString(string query, Encoding encoding, NameValueCollection result)
		{
			if (query.Length == 0)
			{
				return;
			}
			string text = HttpUtility.HtmlDecode(query);
			int length = text.Length;
			int i = 0;
			bool flag = true;
			while (i <= length)
			{
				int num = -1;
				int num2 = -1;
				for (int j = i; j < length; j++)
				{
					if (num == -1 && text[j] == '=')
					{
						num = j + 1;
					}
					else if (text[j] == '&')
					{
						num2 = j;
						break;
					}
				}
				if (flag)
				{
					flag = false;
					if (text[i] == '?')
					{
						i++;
					}
				}
				string text2;
				if (num == -1)
				{
					text2 = null;
					num = i;
				}
				else
				{
					text2 = HttpUtility.UrlDecode(text.Substring(i, num - i - 1), encoding);
				}
				if (num2 < 0)
				{
					i = -1;
					num2 = text.Length;
				}
				else
				{
					i = num2 + 1;
				}
				string text3 = HttpUtility.UrlDecode(text.Substring(num, num2 - num), encoding);
				result.Add(text2, text3);
				if (i == -1)
				{
					break;
				}
			}
		}

		private sealed class HttpQSCollection : NameValueCollection
		{
			public override string ToString()
			{
				int count = this.Count;
				if (count == 0)
				{
					return "";
				}
				StringBuilder stringBuilder = new StringBuilder();
				string[] allKeys = this.AllKeys;
				for (int i = 0; i < count; i++)
				{
					stringBuilder.AppendFormat("{0}={1}&", allKeys[i], HttpUtility.UrlEncode(base[allKeys[i]]));
				}
				if (stringBuilder.Length > 0)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					int length = stringBuilder2.Length;
					stringBuilder2.Length = length - 1;
				}
				return stringBuilder.ToString();
			}
		}
	}
}
