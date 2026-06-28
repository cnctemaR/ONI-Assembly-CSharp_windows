using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace System.Xml
{
	public class XmlConvert
	{
		static XmlConvert()
		{
			int num = XmlConvert.defaultDateTimeFormats.Length;
			XmlConvert.roundtripDateTimeFormats = new string[num];
			XmlConvert.localDateTimeFormats = new string[num];
			XmlConvert.utcDateTimeFormats = new string[num * 3];
			XmlConvert.unspecifiedDateTimeFormats = new string[num * 4];
			for (int i = 0; i < num; i++)
			{
				string text = XmlConvert.defaultDateTimeFormats[i];
				XmlConvert.localDateTimeFormats[i] = text + "zzz";
				XmlConvert.roundtripDateTimeFormats[i] = text + 'K';
				XmlConvert.utcDateTimeFormats[i * 3] = text;
				XmlConvert.utcDateTimeFormats[i * 3 + 1] = text + 'Z';
				XmlConvert.utcDateTimeFormats[i * 3 + 2] = text + "zzz";
				XmlConvert.unspecifiedDateTimeFormats[i * 4] = text;
				XmlConvert.unspecifiedDateTimeFormats[i * 4 + 1] = XmlConvert.localDateTimeFormats[i];
				XmlConvert.unspecifiedDateTimeFormats[i * 4 + 2] = XmlConvert.roundtripDateTimeFormats[i];
				XmlConvert.unspecifiedDateTimeFormats[i * 4 + 3] = XmlConvert.utcDateTimeFormats[i];
			}
		}

		private static string TryDecoding(string s)
		{
			if (s == null || s.Length < 6)
			{
				return s;
			}
			char c = char.MaxValue;
			try
			{
				c = (char)int.Parse(s.Substring(1, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			}
			catch
			{
				return s[0] + XmlConvert.DecodeName(s.Substring(1));
			}
			if (s.Length == 6)
			{
				return c.ToString();
			}
			return c + XmlConvert.DecodeName(s.Substring(6));
		}

		public static string DecodeName(string name)
		{
			if (name == null || name.Length == 0)
			{
				return name;
			}
			int num = name.IndexOf('_');
			if (num == -1 || num + 6 >= name.Length)
			{
				return name;
			}
			if ((name[num + 1] != 'X' && name[num + 1] != 'x') || name[num + 6] != '_')
			{
				return name[0] + XmlConvert.DecodeName(name.Substring(1));
			}
			return name.Substring(0, num) + XmlConvert.TryDecoding(name.Substring(num + 1));
		}

		public static string EncodeLocalName(string name)
		{
			if (name == null)
			{
				return name;
			}
			string text = XmlConvert.EncodeName(name);
			int num = text.IndexOf(':');
			if (num == -1)
			{
				return text;
			}
			return text.Replace(":", "_x003A_");
		}

		internal static bool IsInvalid(char c, bool firstOnlyLetter)
		{
			if (c == ':')
			{
				return false;
			}
			if (firstOnlyLetter)
			{
				return !XmlChar.IsFirstNameChar((int)c);
			}
			return !XmlChar.IsNameChar((int)c);
		}

		private static string EncodeName(string name, bool nmtoken)
		{
			if (name == null || name.Length == 0)
			{
				return name;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int length = name.Length;
			for (int i = 0; i < length; i++)
			{
				char c = name[i];
				if (XmlConvert.IsInvalid(c, i == 0 && !nmtoken))
				{
					stringBuilder.AppendFormat("_x{0:X4}_", (int)c);
				}
				else if (c == '_' && i + 6 < length && name[i + 1] == 'x' && name[i + 6] == '_')
				{
					stringBuilder.Append("_x005F_");
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public static string EncodeName(string name)
		{
			return XmlConvert.EncodeName(name, false);
		}

		public static string EncodeNmToken(string name)
		{
			if (name == string.Empty)
			{
				throw new XmlException("Invalid NmToken: ''");
			}
			return XmlConvert.EncodeName(name, true);
		}

		public static bool ToBoolean(string s)
		{
			s = s.Trim(XmlChar.WhitespaceChars);
			string text = s;
			switch (text)
			{
			case "1":
				return true;
			case "true":
				return true;
			case "0":
				return false;
			case "false":
				return false;
			}
			throw new FormatException(s + " is not a valid boolean value");
		}

		internal static string ToBinHexString(byte[] buffer)
		{
			StringWriter stringWriter = new StringWriter();
			XmlConvert.WriteBinHex(buffer, 0, buffer.Length, stringWriter);
			return stringWriter.ToString();
		}

		internal static void WriteBinHex(byte[] buffer, int index, int count, TextWriter w)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "index must be non negative integer.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", count, "count must be non negative integer.");
			}
			if (buffer.Length < index + count)
			{
				throw new ArgumentOutOfRangeException("index and count must be smaller than the length of the buffer.");
			}
			int num = index + count;
			for (int i = index; i < num; i++)
			{
				int num2 = (int)buffer[i];
				int num3 = num2 >> 4;
				int num4 = num2 & 15;
				if (num3 > 9)
				{
					w.Write((char)(num3 + 55));
				}
				else
				{
					w.Write((char)(num3 + 48));
				}
				if (num4 > 9)
				{
					w.Write((char)(num4 + 55));
				}
				else
				{
					w.Write((char)(num4 + 48));
				}
			}
		}

		public static byte ToByte(string s)
		{
			return byte.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
		}

		public static char ToChar(string s)
		{
			return char.Parse(s);
		}

		[Obsolete]
		public static DateTime ToDateTime(string s)
		{
			return XmlConvert.ToDateTime(s, XmlConvert.datetimeFormats);
		}

		public static DateTime ToDateTime(string value, XmlDateTimeSerializationMode mode)
		{
			switch (mode)
			{
			case XmlDateTimeSerializationMode.Local:
			{
				DateTime dateTime = XmlConvert.ToDateTime(value, XmlConvert.localDateTimeFormats);
				return (!(dateTime == DateTime.MinValue) && !(dateTime == DateTime.MaxValue)) ? dateTime.ToLocalTime() : dateTime;
			}
			case XmlDateTimeSerializationMode.Utc:
			{
				DateTime dateTime = XmlConvert.ToDateTime(value, XmlConvert.utcDateTimeFormats);
				return (!(dateTime == DateTime.MinValue) && !(dateTime == DateTime.MaxValue)) ? dateTime.ToUniversalTime() : dateTime;
			}
			case XmlDateTimeSerializationMode.Unspecified:
				return XmlConvert.ToDateTime(value, XmlConvert.unspecifiedDateTimeFormats);
			case XmlDateTimeSerializationMode.RoundtripKind:
				return XmlConvert.ToDateTime(value, XmlConvert.roundtripDateTimeFormats, XmlConvert._defaultStyle | DateTimeStyles.RoundtripKind);
			default:
				return XmlConvert.ToDateTime(value, XmlConvert.defaultDateTimeFormats);
			}
		}

		public static DateTime ToDateTime(string s, string format)
		{
			DateTimeStyles dateTimeStyles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite;
			return DateTime.ParseExact(s, format, DateTimeFormatInfo.InvariantInfo, dateTimeStyles);
		}

		public static DateTime ToDateTime(string s, string[] formats)
		{
			return XmlConvert.ToDateTime(s, formats, XmlConvert._defaultStyle);
		}

		private static DateTime ToDateTime(string s, string[] formats, DateTimeStyles style)
		{
			return DateTime.ParseExact(s, formats, DateTimeFormatInfo.InvariantInfo, style);
		}

		public static decimal ToDecimal(string s)
		{
			return decimal.Parse(s, CultureInfo.InvariantCulture);
		}

		public static double ToDouble(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException();
			}
			float num = XmlConvert.TryParseStringFloatConstants(s);
			if (num != 0f)
			{
				return (double)num;
			}
			return double.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowCurrencySymbol, CultureInfo.InvariantCulture);
		}

		private static float TryParseStringFloatConstants(string s)
		{
			int num = 0;
			while (num < s.Length && char.IsWhiteSpace(s[num]))
			{
				num++;
			}
			if (num == s.Length)
			{
				throw new FormatException();
			}
			int num2 = s.Length - 1;
			while (char.IsWhiteSpace(s[num2]))
			{
				num2--;
			}
			if (XmlConvert.TryParseStringConstant("NaN", s, num, num2))
			{
				return float.NaN;
			}
			if (XmlConvert.TryParseStringConstant("INF", s, num, num2))
			{
				return float.PositiveInfinity;
			}
			if (XmlConvert.TryParseStringConstant("-INF", s, num, num2))
			{
				return float.NegativeInfinity;
			}
			if (XmlConvert.TryParseStringConstant("Infinity", s, num, num2))
			{
				return float.PositiveInfinity;
			}
			if (XmlConvert.TryParseStringConstant("-Infinity", s, num, num2))
			{
				return float.NegativeInfinity;
			}
			return 0f;
		}

		private static bool TryParseStringConstant(string format, string s, int start, int end)
		{
			return end - start + 1 == format.Length && string.CompareOrdinal(format, 0, s, start, format.Length) == 0;
		}

		public static Guid ToGuid(string s)
		{
			Guid guid;
			try
			{
				guid = new Guid(s);
			}
			catch (FormatException ex)
			{
				throw new FormatException(string.Format("Invalid Guid input '{0}'", ex.InnerException));
			}
			return guid;
		}

		public static short ToInt16(string s)
		{
			return short.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
		}

		public static int ToInt32(string s)
		{
			return int.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
		}

		public static long ToInt64(string s)
		{
			return long.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(string s)
		{
			return sbyte.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
		}

		public static float ToSingle(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException();
			}
			float num = XmlConvert.TryParseStringFloatConstants(s);
			if (num != 0f)
			{
				return num;
			}
			return float.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowCurrencySymbol, CultureInfo.InvariantCulture);
		}

		public static string ToString(Guid value)
		{
			return value.ToString("D", CultureInfo.InvariantCulture);
		}

		public static string ToString(int value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		public static string ToString(short value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		public static string ToString(byte value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		public static string ToString(long value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		public static string ToString(char value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		public static string ToString(bool value)
		{
			if (value)
			{
				return "true";
			}
			return "false";
		}

		[CLSCompliant(false)]
		public static string ToString(sbyte value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		public static string ToString(decimal value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static string ToString(ulong value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		public static string ToString(TimeSpan value)
		{
			if (value == TimeSpan.Zero)
			{
				return "PT0S";
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (value.Ticks < 0L)
			{
				if (value == TimeSpan.MinValue)
				{
					return "-P10675199DT2H48M5.4775808S";
				}
				stringBuilder.Append('-');
				value = value.Negate();
			}
			stringBuilder.Append('P');
			if (value.Days > 0)
			{
				stringBuilder.Append(value.Days).Append('D');
			}
			long num = value.Ticks % 10000L;
			if (value.Days > 0 || value.Hours > 0 || value.Minutes > 0 || value.Seconds > 0 || value.Milliseconds > 0 || num > 0L)
			{
				stringBuilder.Append('T');
				if (value.Hours > 0)
				{
					stringBuilder.Append(value.Hours).Append('H');
				}
				if (value.Minutes > 0)
				{
					stringBuilder.Append(value.Minutes).Append('M');
				}
				if (value.Seconds > 0 || value.Milliseconds > 0 || num > 0L)
				{
					stringBuilder.Append(value.Seconds);
					bool flag = true;
					if (num > 0L)
					{
						stringBuilder.Append('.').AppendFormat("{0:0000000}", value.Ticks % 10000000L);
					}
					else if (value.Milliseconds > 0)
					{
						stringBuilder.Append('.').AppendFormat("{0:000}", value.Milliseconds);
					}
					else
					{
						flag = false;
					}
					if (flag)
					{
						while (stringBuilder[stringBuilder.Length - 1] == '0')
						{
							stringBuilder.Remove(stringBuilder.Length - 1, 1);
						}
					}
					stringBuilder.Append('S');
				}
			}
			return stringBuilder.ToString();
		}

		public static string ToString(double value)
		{
			if (double.IsNegativeInfinity(value))
			{
				return "-INF";
			}
			if (double.IsPositiveInfinity(value))
			{
				return "INF";
			}
			if (double.IsNaN(value))
			{
				return "NaN";
			}
			return value.ToString("R", CultureInfo.InvariantCulture);
		}

		public static string ToString(float value)
		{
			if (float.IsNegativeInfinity(value))
			{
				return "-INF";
			}
			if (float.IsPositiveInfinity(value))
			{
				return "INF";
			}
			if (float.IsNaN(value))
			{
				return "NaN";
			}
			return value.ToString("R", CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static string ToString(uint value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static string ToString(ushort value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		[Obsolete]
		public static string ToString(DateTime value)
		{
			return value.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture);
		}

		public static string ToString(DateTime value, XmlDateTimeSerializationMode mode)
		{
			switch (mode)
			{
			case XmlDateTimeSerializationMode.Local:
				return ((!(value == DateTime.MinValue)) ? ((!(value == DateTime.MaxValue)) ? value.ToLocalTime() : value) : DateTime.MinValue).ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz", CultureInfo.InvariantCulture);
			case XmlDateTimeSerializationMode.Utc:
				return ((!(value == DateTime.MinValue)) ? ((!(value == DateTime.MaxValue)) ? value.ToUniversalTime() : value) : DateTime.MinValue).ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFZ", CultureInfo.InvariantCulture);
			case XmlDateTimeSerializationMode.Unspecified:
				return value.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture);
			case XmlDateTimeSerializationMode.RoundtripKind:
				return value.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture);
			default:
				return value.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz", CultureInfo.InvariantCulture);
			}
		}

		public static string ToString(DateTime value, string format)
		{
			return value.ToString(format, CultureInfo.InvariantCulture);
		}

		public static TimeSpan ToTimeSpan(string s)
		{
			s = s.Trim(XmlChar.WhitespaceChars);
			if (s.Length == 0)
			{
				throw new FormatException("Invalid format string for duration schema datatype.");
			}
			int num = 0;
			if (s[0] == '-')
			{
				num = 1;
			}
			bool flag = num == 1;
			if (s[num] != 'P')
			{
				throw new FormatException("Invalid format string for duration schema datatype.");
			}
			num++;
			int num2 = 0;
			int num3 = 0;
			bool flag2 = false;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			long num7 = 0L;
			int i = 0;
			bool flag3 = false;
			int j = num;
			while (j < s.Length)
			{
				if (s[j] == 'T')
				{
					flag2 = true;
					num2 = 4;
					j++;
					num = j;
				}
				else
				{
					while (j < s.Length)
					{
						if (s[j] < '0' || '9' < s[j])
						{
							break;
						}
						j++;
					}
					if (num2 == 7)
					{
						i = j - num;
					}
					int num8 = int.Parse(s.Substring(num, j - num), CultureInfo.InvariantCulture);
					if (num2 == 7)
					{
						while (i > 7)
						{
							num8 /= 10;
							i--;
						}
						while (i < 7)
						{
							num8 *= 10;
							i++;
						}
					}
					char c = s[j];
					if (c != '.')
					{
						if (c != 'D')
						{
							if (c != 'H')
							{
								if (c != 'M')
								{
									if (c != 'S')
									{
										if (c != 'Y')
										{
											flag3 = true;
										}
										else
										{
											num3 += num8 * 365;
											if (num2 > 0)
											{
												flag3 = true;
											}
											else
											{
												num2 = 1;
											}
										}
									}
									else
									{
										if (num2 == 7)
										{
											num7 = (long)num8;
										}
										else
										{
											num6 = num8;
										}
										if (!flag2 || num2 > 7)
										{
											flag3 = true;
										}
										else
										{
											num2 = 8;
										}
									}
								}
								else if (num2 < 2)
								{
									num3 += 365 * (num8 / 12) + 30 * (num8 % 12);
									num2 = 2;
								}
								else if (flag2 && num2 < 6)
								{
									num5 = num8;
									num2 = 6;
								}
								else
								{
									flag3 = true;
								}
							}
							else
							{
								num4 = num8;
								if (!flag2 || num2 > 4)
								{
									flag3 = true;
								}
								else
								{
									num2 = 5;
								}
							}
						}
						else
						{
							num3 += num8;
							if (num2 > 2)
							{
								flag3 = true;
							}
							else
							{
								num2 = 3;
							}
						}
					}
					else
					{
						if (num2 > 7)
						{
							flag3 = true;
						}
						num6 = num8;
						num2 = 7;
					}
					if (flag3)
					{
						break;
					}
					j++;
					num = j;
				}
			}
			if (flag3)
			{
				throw new FormatException("Invalid format string for duration schema datatype.");
			}
			TimeSpan timeSpan = new TimeSpan(num3, num4, num5, num6);
			if (flag)
			{
				return TimeSpan.FromTicks(-(timeSpan.Ticks + num7));
			}
			return TimeSpan.FromTicks(timeSpan.Ticks + num7);
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(string s)
		{
			return ushort.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(string s)
		{
			return uint.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(string s)
		{
			return ulong.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
		}

		public static string VerifyName(string name)
		{
			if (name == null || name.Length == 0)
			{
				throw new ArgumentNullException("name");
			}
			if (!XmlChar.IsName(name))
			{
				throw new XmlException("'" + name + "' is not a valid XML Name");
			}
			return name;
		}

		public static string VerifyNCName(string ncname)
		{
			if (ncname == null || ncname.Length == 0)
			{
				throw new ArgumentNullException("ncname");
			}
			if (!XmlChar.IsNCName(ncname))
			{
				throw new XmlException("'" + ncname + "' is not a valid XML NCName");
			}
			return ncname;
		}

		public static string VerifyTOKEN(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				return name;
			}
			if (XmlChar.IsWhitespace((int)name[0]) || XmlChar.IsWhitespace((int)name[name.Length - 1]))
			{
				throw new XmlException("Whitespace characters (#xA, #xD, #x9, #x20) are not allowed as leading or trailing whitespaces of xs:token.");
			}
			for (int i = 0; i < name.Length; i++)
			{
				if (XmlChar.IsWhitespace((int)name[i]) && name[i] != ' ')
				{
					throw new XmlException("Either #xA, #xD or #x9 are not allowed inside xs:token.");
				}
			}
			return name;
		}

		public static string VerifyNMTOKEN(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (!XmlChar.IsNmToken(name))
			{
				throw new XmlException("'" + name + "' is not a valid XML NMTOKEN");
			}
			return name;
		}

		internal static byte[] FromBinHexString(string s)
		{
			char[] array = s.ToCharArray();
			byte[] array2 = new byte[array.Length / 2 + array.Length % 2];
			XmlConvert.FromBinHexString(array, 0, array.Length, array2);
			return array2;
		}

		internal static int FromBinHexString(char[] chars, int offset, int charLength, byte[] buffer)
		{
			int num = offset;
			for (int i = 0; i < charLength - 1; i += 2)
			{
				buffer[num] = ((chars[i] <= '9') ? ((byte)(chars[i] - '0')) : ((byte)(chars[i] - 'A' + '\n')));
				int num2 = num;
				buffer[num2] = (byte)(buffer[num2] << 4);
				int num3 = num;
				buffer[num3] += ((chars[i + 1] <= '9') ? ((byte)(chars[i + 1] - '0')) : ((byte)(chars[i + 1] - 'A' + '\n')));
				num++;
			}
			if (charLength % 2 != 0)
			{
				buffer[num++] = (byte)(((chars[charLength - 1] <= '9') ? ((byte)(chars[charLength - 1] - '0')) : ((byte)(chars[charLength - 1] - 'A' + '\n'))) << 4);
			}
			return num - offset;
		}

		public static DateTimeOffset ToDateTimeOffset(string s)
		{
			return XmlConvert.ToDateTimeOffset(s, XmlConvert.datetimeFormats);
		}

		public static DateTimeOffset ToDateTimeOffset(string s, string format)
		{
			return DateTimeOffset.ParseExact(s, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
		}

		public static DateTimeOffset ToDateTimeOffset(string s, string[] formats)
		{
			DateTimeStyles dateTimeStyles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AssumeUniversal;
			return DateTimeOffset.ParseExact(s, formats, CultureInfo.InvariantCulture, dateTimeStyles);
		}

		public static string ToString(DateTimeOffset value)
		{
			return XmlConvert.ToString(value, "yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz");
		}

		public static string ToString(DateTimeOffset value, string format)
		{
			return value.ToString(format, CultureInfo.InvariantCulture);
		}

		internal static Uri ToUri(string s)
		{
			return new Uri(s, UriKind.RelativeOrAbsolute);
		}

		private const string encodedColon = "_x003A_";

		private const NumberStyles floatStyle = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowCurrencySymbol;

		private const NumberStyles integerStyle = NumberStyles.Integer;

		private static readonly string[] datetimeFormats = new string[]
		{
			"yyyy-MM-ddTHH:mm:sszzz", "yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-ddTHH:mm:ss.FFFFFFFZ", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss.FFFFFFF", "HH:mm:ss", "HH:mm:ss.FFFFFFF", "HH:mm:sszzz", "HH:mm:ss.FFFFFFFzzz",
			"HH:mm:ssZ", "HH:mm:ss.FFFFFFFZ", "yyyy-MM-dd", "yyyy-MM-ddzzz", "yyyy-MM-ddZ", "yyyy-MM", "yyyy-MMzzz", "yyyy-MMZ", "yyyy", "yyyyzzz",
			"yyyyZ", "--MM-dd", "--MM-ddzzz", "--MM-ddZ", "---dd", "---ddzzz", "---ddZ"
		};

		private static readonly string[] defaultDateTimeFormats = new string[] { "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss.FFFFFFF", "yyyy-MM-dd", "HH:mm:ss", "yyyy-MM", "yyyy", "--MM-dd", "---dd" };

		private static readonly string[] roundtripDateTimeFormats;

		private static readonly string[] localDateTimeFormats;

		private static readonly string[] utcDateTimeFormats;

		private static readonly string[] unspecifiedDateTimeFormats;

		private static DateTimeStyles _defaultStyle = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite;
	}
}
