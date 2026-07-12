using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace System.Xml
{
	internal static class XmlConverter
	{
		public static Base64Encoding Base64Encoding
		{
			get
			{
				if (XmlConverter.base64Encoding == null)
				{
					XmlConverter.base64Encoding = new Base64Encoding();
				}
				return XmlConverter.base64Encoding;
			}
		}

		private static UTF8Encoding UTF8Encoding
		{
			get
			{
				if (XmlConverter.utf8Encoding == null)
				{
					XmlConverter.utf8Encoding = new UTF8Encoding(false, true);
				}
				return XmlConverter.utf8Encoding;
			}
		}

		private static UnicodeEncoding UnicodeEncoding
		{
			get
			{
				if (XmlConverter.unicodeEncoding == null)
				{
					XmlConverter.unicodeEncoding = new UnicodeEncoding(false, false, true);
				}
				return XmlConverter.unicodeEncoding;
			}
		}

		public static bool ToBoolean(string value)
		{
			bool flag;
			try
			{
				flag = XmlConvert.ToBoolean(value);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Boolean", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Boolean", ex2));
			}
			return flag;
		}

		public static bool ToBoolean(byte[] buffer, int offset, int count)
		{
			if (count == 1)
			{
				byte b = buffer[offset];
				if (b == 49)
				{
					return true;
				}
				if (b == 48)
				{
					return false;
				}
			}
			return XmlConverter.ToBoolean(XmlConverter.ToString(buffer, offset, count));
		}

		public static int ToInt32(string value)
		{
			int num;
			try
			{
				num = XmlConvert.ToInt32(value);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Int32", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Int32", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Int32", ex3));
			}
			return num;
		}

		public static int ToInt32(byte[] buffer, int offset, int count)
		{
			int num;
			if (XmlConverter.TryParseInt32(buffer, offset, count, out num))
			{
				return num;
			}
			return XmlConverter.ToInt32(XmlConverter.ToString(buffer, offset, count));
		}

		public static long ToInt64(string value)
		{
			long num;
			try
			{
				num = XmlConvert.ToInt64(value);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Int64", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Int64", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Int64", ex3));
			}
			return num;
		}

		public static long ToInt64(byte[] buffer, int offset, int count)
		{
			long num;
			if (XmlConverter.TryParseInt64(buffer, offset, count, out num))
			{
				return num;
			}
			return XmlConverter.ToInt64(XmlConverter.ToString(buffer, offset, count));
		}

		public static float ToSingle(string value)
		{
			float num;
			try
			{
				num = XmlConvert.ToSingle(value);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "float", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "float", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "float", ex3));
			}
			return num;
		}

		public static float ToSingle(byte[] buffer, int offset, int count)
		{
			float num;
			if (XmlConverter.TryParseSingle(buffer, offset, count, out num))
			{
				return num;
			}
			return XmlConverter.ToSingle(XmlConverter.ToString(buffer, offset, count));
		}

		public static double ToDouble(string value)
		{
			double num;
			try
			{
				num = XmlConvert.ToDouble(value);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "double", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "double", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "double", ex3));
			}
			return num;
		}

		public static double ToDouble(byte[] buffer, int offset, int count)
		{
			double num;
			if (XmlConverter.TryParseDouble(buffer, offset, count, out num))
			{
				return num;
			}
			return XmlConverter.ToDouble(XmlConverter.ToString(buffer, offset, count));
		}

		public static decimal ToDecimal(string value)
		{
			decimal num;
			try
			{
				num = XmlConvert.ToDecimal(value);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "decimal", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "decimal", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "decimal", ex3));
			}
			return num;
		}

		public static decimal ToDecimal(byte[] buffer, int offset, int count)
		{
			return XmlConverter.ToDecimal(XmlConverter.ToString(buffer, offset, count));
		}

		public static DateTime ToDateTime(long value)
		{
			DateTime dateTime;
			try
			{
				dateTime = DateTime.FromBinary(value);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(XmlConverter.ToString(value), "DateTime", ex));
			}
			return dateTime;
		}

		public static DateTime ToDateTime(string value)
		{
			DateTime dateTime;
			try
			{
				dateTime = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "DateTime", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "DateTime", ex2));
			}
			return dateTime;
		}

		public static DateTime ToDateTime(byte[] buffer, int offset, int count)
		{
			DateTime dateTime;
			if (XmlConverter.TryParseDateTime(buffer, offset, count, out dateTime))
			{
				return dateTime;
			}
			return XmlConverter.ToDateTime(XmlConverter.ToString(buffer, offset, count));
		}

		public static UniqueId ToUniqueId(string value)
		{
			UniqueId uniqueId;
			try
			{
				uniqueId = new UniqueId(XmlConverter.Trim(value));
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "UniqueId", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "UniqueId", ex2));
			}
			return uniqueId;
		}

		public static UniqueId ToUniqueId(byte[] buffer, int offset, int count)
		{
			return XmlConverter.ToUniqueId(XmlConverter.ToString(buffer, offset, count));
		}

		public static TimeSpan ToTimeSpan(string value)
		{
			TimeSpan timeSpan;
			try
			{
				timeSpan = XmlConvert.ToTimeSpan(value);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "TimeSpan", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "TimeSpan", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "TimeSpan", ex3));
			}
			return timeSpan;
		}

		public static TimeSpan ToTimeSpan(byte[] buffer, int offset, int count)
		{
			return XmlConverter.ToTimeSpan(XmlConverter.ToString(buffer, offset, count));
		}

		public static Guid ToGuid(string value)
		{
			Guid guid;
			try
			{
				guid = Guid.Parse(XmlConverter.Trim(value));
			}
			catch (FormatException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Guid", ex));
			}
			catch (ArgumentException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Guid", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Guid", ex3));
			}
			return guid;
		}

		public static Guid ToGuid(byte[] buffer, int offset, int count)
		{
			return XmlConverter.ToGuid(XmlConverter.ToString(buffer, offset, count));
		}

		public static ulong ToUInt64(string value)
		{
			ulong num;
			try
			{
				num = ulong.Parse(value, NumberFormatInfo.InvariantInfo);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "UInt64", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "UInt64", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "UInt64", ex3));
			}
			return num;
		}

		public static ulong ToUInt64(byte[] buffer, int offset, int count)
		{
			return XmlConverter.ToUInt64(XmlConverter.ToString(buffer, offset, count));
		}

		public static string ToString(byte[] buffer, int offset, int count)
		{
			string @string;
			try
			{
				@string = XmlConverter.UTF8Encoding.GetString(buffer, offset, count);
			}
			catch (DecoderFallbackException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateEncodingException(buffer, offset, count, ex));
			}
			return @string;
		}

		public static string ToStringUnicode(byte[] buffer, int offset, int count)
		{
			string @string;
			try
			{
				@string = XmlConverter.UnicodeEncoding.GetString(buffer, offset, count);
			}
			catch (DecoderFallbackException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateEncodingException(buffer, offset, count, ex));
			}
			return @string;
		}

		public static byte[] ToBytes(string value)
		{
			byte[] bytes;
			try
			{
				bytes = XmlConverter.UTF8Encoding.GetBytes(value);
			}
			catch (DecoderFallbackException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateEncodingException(value, ex));
			}
			return bytes;
		}

		public static int ToChars(byte[] buffer, int offset, int count, char[] chars, int charOffset)
		{
			int chars2;
			try
			{
				chars2 = XmlConverter.UTF8Encoding.GetChars(buffer, offset, count, chars, charOffset);
			}
			catch (DecoderFallbackException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateEncodingException(buffer, offset, count, ex));
			}
			return chars2;
		}

		public static string ToString(bool value)
		{
			if (!value)
			{
				return "false";
			}
			return "true";
		}

		public static string ToString(int value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(long value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(float value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(double value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(decimal value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(TimeSpan value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(UniqueId value)
		{
			return value.ToString();
		}

		public static string ToString(Guid value)
		{
			return value.ToString();
		}

		public static string ToString(ulong value)
		{
			return value.ToString(NumberFormatInfo.InvariantInfo);
		}

		public static string ToString(DateTime value)
		{
			byte[] array = new byte[64];
			int num = XmlConverter.ToChars(value, array, 0);
			return XmlConverter.ToString(array, 0, num);
		}

		private static string ToString(object value)
		{
			if (value is int)
			{
				return XmlConverter.ToString((int)value);
			}
			if (value is long)
			{
				return XmlConverter.ToString((long)value);
			}
			if (value is float)
			{
				return XmlConverter.ToString((float)value);
			}
			if (value is double)
			{
				return XmlConverter.ToString((double)value);
			}
			if (value is decimal)
			{
				return XmlConverter.ToString((decimal)value);
			}
			if (value is TimeSpan)
			{
				return XmlConverter.ToString((TimeSpan)value);
			}
			if (value is UniqueId)
			{
				return XmlConverter.ToString((UniqueId)value);
			}
			if (value is Guid)
			{
				return XmlConverter.ToString((Guid)value);
			}
			if (value is ulong)
			{
				return XmlConverter.ToString((ulong)value);
			}
			if (value is DateTime)
			{
				return XmlConverter.ToString((DateTime)value);
			}
			if (value is bool)
			{
				return XmlConverter.ToString((bool)value);
			}
			return value.ToString();
		}

		public static string ToString(object[] objects)
		{
			if (objects.Length == 0)
			{
				return string.Empty;
			}
			string text = XmlConverter.ToString(objects[0]);
			if (objects.Length > 1)
			{
				StringBuilder stringBuilder = new StringBuilder(text);
				for (int i = 1; i < objects.Length; i++)
				{
					stringBuilder.Append(' ');
					stringBuilder.Append(XmlConverter.ToString(objects[i]));
				}
				text = stringBuilder.ToString();
			}
			return text;
		}

		public static void ToQualifiedName(string qname, out string prefix, out string localName)
		{
			int num = qname.IndexOf(':');
			if (num < 0)
			{
				prefix = string.Empty;
				localName = XmlConverter.Trim(qname);
				return;
			}
			if (num == qname.Length - 1)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Expected XML qualified name. Found '{0}'.", new object[] { qname })));
			}
			prefix = XmlConverter.Trim(qname.Substring(0, num));
			localName = XmlConverter.Trim(qname.Substring(num + 1));
		}

		private static bool TryParseInt32(byte[] chars, int offset, int count, out int result)
		{
			result = 0;
			if (count == 0)
			{
				return false;
			}
			int num = 0;
			int num2 = offset + count;
			if (chars[offset] == 45)
			{
				if (count == 1)
				{
					return false;
				}
				for (int i = offset + 1; i < num2; i++)
				{
					int num3 = (int)(chars[i] - 48);
					if (num3 > 9)
					{
						return false;
					}
					if (num < -214748364)
					{
						return false;
					}
					num *= 10;
					if (num < -2147483648 + num3)
					{
						return false;
					}
					num -= num3;
				}
			}
			else
			{
				for (int j = offset; j < num2; j++)
				{
					int num4 = (int)(chars[j] - 48);
					if (num4 > 9)
					{
						return false;
					}
					if (num > 214748364)
					{
						return false;
					}
					num *= 10;
					if (num > 2147483647 - num4)
					{
						return false;
					}
					num += num4;
				}
			}
			result = num;
			return true;
		}

		private static bool TryParseInt64(byte[] chars, int offset, int count, out long result)
		{
			result = 0L;
			if (count >= 11)
			{
				long num = 0L;
				int num2 = offset + count;
				if (chars[offset] == 45)
				{
					if (count == 1)
					{
						return false;
					}
					for (int i = offset + 1; i < num2; i++)
					{
						int num3 = (int)(chars[i] - 48);
						if (num3 > 9)
						{
							return false;
						}
						if (num < -922337203685477580L)
						{
							return false;
						}
						num *= 10L;
						if (num < -9223372036854775808L + (long)num3)
						{
							return false;
						}
						num -= (long)num3;
					}
				}
				else
				{
					for (int j = offset; j < num2; j++)
					{
						int num4 = (int)(chars[j] - 48);
						if (num4 > 9)
						{
							return false;
						}
						if (num > 922337203685477580L)
						{
							return false;
						}
						num *= 10L;
						if (num > 9223372036854775807L - (long)num4)
						{
							return false;
						}
						num += (long)num4;
					}
				}
				result = num;
				return true;
			}
			int num5;
			if (!XmlConverter.TryParseInt32(chars, offset, count, out num5))
			{
				return false;
			}
			result = (long)num5;
			return true;
		}

		private static bool TryParseSingle(byte[] chars, int offset, int count, out float result)
		{
			result = 0f;
			int num = offset + count;
			bool flag = false;
			if (offset < num && chars[offset] == 45)
			{
				flag = true;
				offset++;
				count--;
			}
			if (count < 1 || count > 10)
			{
				return false;
			}
			int num2 = 0;
			while (offset < num)
			{
				int num3 = (int)(chars[offset] - 48);
				if (num3 == -2)
				{
					offset++;
					int num4 = 1;
					while (offset < num)
					{
						num3 = (int)(chars[offset] - 48);
						if (num3 >= 10)
						{
							return false;
						}
						num4 *= 10;
						num2 = num2 * 10 + num3;
						offset++;
					}
					if (count > 8)
					{
						result = (float)((double)num2 / (double)num4);
					}
					else
					{
						result = (float)num2 / (float)num4;
					}
					if (flag)
					{
						result = -result;
					}
					return true;
				}
				if (num3 >= 10)
				{
					return false;
				}
				num2 = num2 * 10 + num3;
				offset++;
			}
			if (count == 10)
			{
				return false;
			}
			if (flag)
			{
				result = (float)(-(float)num2);
			}
			else
			{
				result = (float)num2;
			}
			return true;
		}

		private static bool TryParseDouble(byte[] chars, int offset, int count, out double result)
		{
			result = 0.0;
			int num = offset + count;
			bool flag = false;
			if (offset < num && chars[offset] == 45)
			{
				flag = true;
				offset++;
				count--;
			}
			if (count < 1 || count > 10)
			{
				return false;
			}
			int num2 = 0;
			while (offset < num)
			{
				int num3 = (int)(chars[offset] - 48);
				if (num3 == -2)
				{
					offset++;
					int num4 = 1;
					while (offset < num)
					{
						num3 = (int)(chars[offset] - 48);
						if (num3 >= 10)
						{
							return false;
						}
						num4 *= 10;
						num2 = num2 * 10 + num3;
						offset++;
					}
					if (flag)
					{
						result = -(double)num2 / (double)num4;
					}
					else
					{
						result = (double)num2 / (double)num4;
					}
					return true;
				}
				if (num3 >= 10)
				{
					return false;
				}
				num2 = num2 * 10 + num3;
				offset++;
			}
			if (count == 10)
			{
				return false;
			}
			if (flag)
			{
				result = (double)(-(double)num2);
			}
			else
			{
				result = (double)num2;
			}
			return true;
		}

		private static int ToInt32D2(byte[] chars, int offset)
		{
			byte b = chars[offset] - 48;
			byte b2 = chars[offset + 1] - 48;
			if (b > 9 || b2 > 9)
			{
				return -1;
			}
			return (int)(10 * b + b2);
		}

		private static int ToInt32D4(byte[] chars, int offset, int count)
		{
			return XmlConverter.ToInt32D7(chars, offset, count);
		}

		private static int ToInt32D7(byte[] chars, int offset, int count)
		{
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				byte b = chars[offset + i] - 48;
				if (b > 9)
				{
					return -1;
				}
				num = num * 10 + (int)b;
			}
			return num;
		}

		private static bool TryParseDateTime(byte[] chars, int offset, int count, out DateTime result)
		{
			int num = offset + count;
			result = DateTime.MaxValue;
			if (count < 19)
			{
				return false;
			}
			if (chars[offset + 4] != 45 || chars[offset + 7] != 45 || chars[offset + 10] != 84 || chars[offset + 13] != 58 || chars[offset + 16] != 58)
			{
				return false;
			}
			int num2 = XmlConverter.ToInt32D4(chars, offset, 4);
			int num3 = XmlConverter.ToInt32D2(chars, offset + 5);
			int num4 = XmlConverter.ToInt32D2(chars, offset + 8);
			int num5 = XmlConverter.ToInt32D2(chars, offset + 11);
			int num6 = XmlConverter.ToInt32D2(chars, offset + 14);
			int num7 = XmlConverter.ToInt32D2(chars, offset + 17);
			if ((num2 | num3 | num4 | num5 | num6 | num7) < 0)
			{
				return false;
			}
			DateTimeKind dateTimeKind = DateTimeKind.Unspecified;
			offset += 19;
			int num8 = 0;
			if (offset < num && chars[offset] == 46)
			{
				offset++;
				int num9 = offset;
				while (offset < num)
				{
					byte b = chars[offset];
					if (b < 48 || b > 57)
					{
						break;
					}
					offset++;
				}
				int num10 = offset - num9;
				if (num10 < 1 || num10 > 7)
				{
					return false;
				}
				num8 = XmlConverter.ToInt32D7(chars, num9, num10);
				if (num8 < 0)
				{
					return false;
				}
				for (int i = num10; i < 7; i++)
				{
					num8 *= 10;
				}
			}
			bool flag = false;
			int num11 = 0;
			int num12 = 0;
			if (offset < num)
			{
				byte b2 = chars[offset];
				if (b2 == 90)
				{
					offset++;
					dateTimeKind = DateTimeKind.Utc;
				}
				else if (b2 == 43 || b2 == 45)
				{
					offset++;
					if (offset + 5 > num || chars[offset + 2] != 58)
					{
						return false;
					}
					dateTimeKind = DateTimeKind.Utc;
					flag = true;
					num11 = XmlConverter.ToInt32D2(chars, offset);
					num12 = XmlConverter.ToInt32D2(chars, offset + 3);
					if ((num11 | num12) < 0)
					{
						return false;
					}
					if (b2 == 43)
					{
						num11 = -num11;
						num12 = -num12;
					}
					offset += 5;
				}
			}
			if (offset < num)
			{
				return false;
			}
			DateTime dateTime;
			try
			{
				dateTime = new DateTime(num2, num3, num4, num5, num6, num7, dateTimeKind);
			}
			catch (ArgumentException)
			{
				return false;
			}
			if (num8 > 0)
			{
				dateTime = dateTime.AddTicks((long)num8);
			}
			if (flag)
			{
				try
				{
					TimeSpan timeSpan = new TimeSpan(num11, num12, 0);
					if ((num11 >= 0 && dateTime < DateTime.MaxValue - timeSpan) || (num11 < 0 && dateTime > DateTime.MinValue - timeSpan))
					{
						dateTime = dateTime.Add(timeSpan).ToLocalTime();
					}
					else
					{
						dateTime = dateTime.ToLocalTime().Add(timeSpan);
					}
				}
				catch (ArgumentOutOfRangeException)
				{
					return false;
				}
			}
			result = dateTime;
			return true;
		}

		public static int ToChars(bool value, byte[] buffer, int offset)
		{
			if (value)
			{
				buffer[offset] = 116;
				buffer[offset + 1] = 114;
				buffer[offset + 2] = 117;
				buffer[offset + 3] = 101;
				return 4;
			}
			buffer[offset] = 102;
			buffer[offset + 1] = 97;
			buffer[offset + 2] = 108;
			buffer[offset + 3] = 115;
			buffer[offset + 4] = 101;
			return 5;
		}

		public static int ToCharsR(int value, byte[] chars, int offset)
		{
			int num = 0;
			if (value >= 0)
			{
				while (value >= 10)
				{
					int num2 = value / 10;
					num++;
					chars[--offset] = (byte)(48 + (value - num2 * 10));
					value = num2;
				}
				chars[--offset] = (byte)(48 + value);
				num++;
			}
			else
			{
				while (value <= -10)
				{
					int num3 = value / 10;
					num++;
					chars[--offset] = (byte)(48 - (value - num3 * 10));
					value = num3;
				}
				chars[--offset] = (byte)(48 - value);
				chars[--offset] = 45;
				num += 2;
			}
			return num;
		}

		public static int ToChars(int value, byte[] chars, int offset)
		{
			int num = XmlConverter.ToCharsR(value, chars, offset + 16);
			Buffer.BlockCopy(chars, offset + 16 - num, chars, offset, num);
			return num;
		}

		public static int ToCharsR(long value, byte[] chars, int offset)
		{
			int num = 0;
			if (value >= 0L)
			{
				while (value > 2147483647L)
				{
					long num2 = value / 10L;
					num++;
					chars[--offset] = (byte)(48 + (int)(value - num2 * 10L));
					value = num2;
				}
			}
			else
			{
				while (value < -2147483648L)
				{
					long num3 = value / 10L;
					num++;
					chars[--offset] = (byte)(48 - (int)(value - num3 * 10L));
					value = num3;
				}
			}
			return num + XmlConverter.ToCharsR((int)value, chars, offset);
		}

		public static int ToChars(long value, byte[] chars, int offset)
		{
			int num = XmlConverter.ToCharsR(value, chars, offset + 32);
			Buffer.BlockCopy(chars, offset + 32 - num, chars, offset, num);
			return num;
		}

		[SecuritySafeCritical]
		private unsafe static bool IsNegativeZero(float value)
		{
			float num = -0f;
			return *(int*)(&value) == *(int*)(&num);
		}

		[SecuritySafeCritical]
		private unsafe static bool IsNegativeZero(double value)
		{
			double num = -0.0;
			return *(long*)(&value) == *(long*)(&num);
		}

		private static int ToInfinity(bool isNegative, byte[] buffer, int offset)
		{
			if (isNegative)
			{
				buffer[offset] = 45;
				buffer[offset + 1] = 73;
				buffer[offset + 2] = 78;
				buffer[offset + 3] = 70;
				return 4;
			}
			buffer[offset] = 73;
			buffer[offset + 1] = 78;
			buffer[offset + 2] = 70;
			return 3;
		}

		private static int ToZero(bool isNegative, byte[] buffer, int offset)
		{
			if (isNegative)
			{
				buffer[offset] = 45;
				buffer[offset + 1] = 48;
				return 2;
			}
			buffer[offset] = 48;
			return 1;
		}

		public static int ToChars(double value, byte[] buffer, int offset)
		{
			if (double.IsInfinity(value))
			{
				return XmlConverter.ToInfinity(double.IsNegativeInfinity(value), buffer, offset);
			}
			if (value == 0.0)
			{
				return XmlConverter.ToZero(XmlConverter.IsNegativeZero(value), buffer, offset);
			}
			return XmlConverter.ToAsciiChars(value.ToString("R", NumberFormatInfo.InvariantInfo), buffer, offset);
		}

		public static int ToChars(float value, byte[] buffer, int offset)
		{
			if (float.IsInfinity(value))
			{
				return XmlConverter.ToInfinity(float.IsNegativeInfinity(value), buffer, offset);
			}
			if ((double)value == 0.0)
			{
				return XmlConverter.ToZero(XmlConverter.IsNegativeZero(value), buffer, offset);
			}
			return XmlConverter.ToAsciiChars(value.ToString("R", NumberFormatInfo.InvariantInfo), buffer, offset);
		}

		public static int ToChars(decimal value, byte[] buffer, int offset)
		{
			return XmlConverter.ToAsciiChars(value.ToString(null, NumberFormatInfo.InvariantInfo), buffer, offset);
		}

		public static int ToChars(ulong value, byte[] buffer, int offset)
		{
			return XmlConverter.ToAsciiChars(value.ToString(null, NumberFormatInfo.InvariantInfo), buffer, offset);
		}

		private static int ToAsciiChars(string s, byte[] buffer, int offset)
		{
			for (int i = 0; i < s.Length; i++)
			{
				buffer[offset++] = (byte)s[i];
			}
			return s.Length;
		}

		private static int ToCharsD2(int value, byte[] chars, int offset)
		{
			if (value < 10)
			{
				chars[offset] = 48;
				chars[offset + 1] = (byte)(48 + value);
			}
			else
			{
				int num = value / 10;
				chars[offset] = (byte)(48 + num);
				chars[offset + 1] = (byte)(48 + value - num * 10);
			}
			return 2;
		}

		private static int ToCharsD4(int value, byte[] chars, int offset)
		{
			XmlConverter.ToCharsD2(value / 100, chars, offset);
			XmlConverter.ToCharsD2(value % 100, chars, offset + 2);
			return 4;
		}

		private static int ToCharsD7(int value, byte[] chars, int offset)
		{
			int num = 7 - XmlConverter.ToCharsR(value, chars, offset + 7);
			for (int i = 0; i < num; i++)
			{
				chars[offset + i] = 48;
			}
			int num2 = 7;
			while (num2 > 0 && chars[offset + num2 - 1] == 48)
			{
				num2--;
			}
			return num2;
		}

		public static int ToChars(DateTime value, byte[] chars, int offset)
		{
			int num = offset;
			offset += XmlConverter.ToCharsD4(value.Year, chars, offset);
			chars[offset++] = 45;
			offset += XmlConverter.ToCharsD2(value.Month, chars, offset);
			chars[offset++] = 45;
			offset += XmlConverter.ToCharsD2(value.Day, chars, offset);
			chars[offset++] = 84;
			offset += XmlConverter.ToCharsD2(value.Hour, chars, offset);
			chars[offset++] = 58;
			offset += XmlConverter.ToCharsD2(value.Minute, chars, offset);
			chars[offset++] = 58;
			offset += XmlConverter.ToCharsD2(value.Second, chars, offset);
			int num2 = (int)(value.Ticks % 10000000L);
			if (num2 != 0)
			{
				chars[offset++] = 46;
				offset += XmlConverter.ToCharsD7(num2, chars, offset);
			}
			switch (value.Kind)
			{
			case DateTimeKind.Unspecified:
				break;
			case DateTimeKind.Utc:
				chars[offset++] = 90;
				break;
			case DateTimeKind.Local:
			{
				TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(value);
				if (utcOffset.Ticks < 0L)
				{
					chars[offset++] = 45;
				}
				else
				{
					chars[offset++] = 43;
				}
				offset += XmlConverter.ToCharsD2(Math.Abs(utcOffset.Hours), chars, offset);
				chars[offset++] = 58;
				offset += XmlConverter.ToCharsD2(Math.Abs(utcOffset.Minutes), chars, offset);
				break;
			}
			default:
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException());
			}
			return offset - num;
		}

		public static bool IsWhitespace(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (!XmlConverter.IsWhitespace(s[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsWhitespace(char ch)
		{
			return ch <= ' ' && (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n');
		}

		public static string StripWhitespace(string s)
		{
			int num = s.Length;
			for (int i = 0; i < s.Length; i++)
			{
				if (XmlConverter.IsWhitespace(s[i]))
				{
					num--;
				}
			}
			if (num == s.Length)
			{
				return s;
			}
			char[] array = new char[num];
			num = 0;
			foreach (char c in s)
			{
				if (!XmlConverter.IsWhitespace(c))
				{
					array[num++] = c;
				}
			}
			return new string(array);
		}

		private static string Trim(string s)
		{
			int num = 0;
			while (num < s.Length && XmlConverter.IsWhitespace(s[num]))
			{
				num++;
			}
			int num2 = s.Length;
			while (num2 > 0 && XmlConverter.IsWhitespace(s[num2 - 1]))
			{
				num2--;
			}
			if (num == 0 && num2 == s.Length)
			{
				return s;
			}
			if (num2 == 0)
			{
				return string.Empty;
			}
			return s.Substring(num, num2 - num);
		}

		public const int MaxDateTimeChars = 64;

		public const int MaxInt32Chars = 16;

		public const int MaxInt64Chars = 32;

		public const int MaxBoolChars = 5;

		public const int MaxFloatChars = 16;

		public const int MaxDoubleChars = 32;

		public const int MaxDecimalChars = 40;

		public const int MaxUInt64Chars = 32;

		public const int MaxPrimitiveChars = 64;

		private static char[] whiteSpaceChars = new char[] { ' ', '\t', '\n', '\r' };

		private static UTF8Encoding utf8Encoding;

		private static UnicodeEncoding unicodeEncoding;

		private static Base64Encoding base64Encoding;
	}
}
