using System;
using System.Runtime.Serialization;
using System.Text;

namespace System.Xml
{
	internal class ValueHandle
	{
		public ValueHandle(XmlBufferReader bufferReader)
		{
			this.bufferReader = bufferReader;
			this.type = ValueHandleType.Empty;
		}

		private static Base64Encoding Base64Encoding
		{
			get
			{
				if (ValueHandle.base64Encoding == null)
				{
					ValueHandle.base64Encoding = new Base64Encoding();
				}
				return ValueHandle.base64Encoding;
			}
		}

		public void SetConstantValue(ValueHandleConstStringType constStringType)
		{
			this.type = ValueHandleType.ConstString;
			this.offset = (int)constStringType;
		}

		public void SetValue(ValueHandleType type)
		{
			this.type = type;
		}

		public void SetDictionaryValue(int key)
		{
			this.SetValue(ValueHandleType.Dictionary, key, 0);
		}

		public void SetCharValue(int ch)
		{
			this.SetValue(ValueHandleType.Char, ch, 0);
		}

		public void SetQNameValue(int prefix, int key)
		{
			this.SetValue(ValueHandleType.QName, key, prefix);
		}

		public void SetValue(ValueHandleType type, int offset, int length)
		{
			this.type = type;
			this.offset = offset;
			this.length = length;
		}

		public bool IsWhitespace()
		{
			ValueHandleType valueHandleType = this.type;
			if (valueHandleType - ValueHandleType.True > 3)
			{
				switch (valueHandleType)
				{
				case ValueHandleType.UTF8:
					return this.bufferReader.IsWhitespaceUTF8(this.offset, this.length);
				case ValueHandleType.EscapedUTF8:
					return this.bufferReader.IsWhitespaceUTF8(this.offset, this.length);
				case ValueHandleType.Dictionary:
					return this.bufferReader.IsWhitespaceKey(this.offset);
				case ValueHandleType.Char:
				{
					int @char = this.GetChar();
					return @char <= 65535 && XmlConverter.IsWhitespace((char)@char);
				}
				case ValueHandleType.Unicode:
					return this.bufferReader.IsWhitespaceUnicode(this.offset, this.length);
				case ValueHandleType.ConstString:
					return ValueHandle.constStrings[this.offset].Length == 0;
				}
				return this.length == 0;
			}
			return false;
		}

		public Type ToType()
		{
			switch (this.type)
			{
			case ValueHandleType.Empty:
			case ValueHandleType.UTF8:
			case ValueHandleType.EscapedUTF8:
			case ValueHandleType.Dictionary:
			case ValueHandleType.Char:
			case ValueHandleType.Unicode:
			case ValueHandleType.QName:
			case ValueHandleType.ConstString:
				return typeof(string);
			case ValueHandleType.True:
			case ValueHandleType.False:
				return typeof(bool);
			case ValueHandleType.Zero:
			case ValueHandleType.One:
			case ValueHandleType.Int8:
			case ValueHandleType.Int16:
			case ValueHandleType.Int32:
				return typeof(int);
			case ValueHandleType.Int64:
				return typeof(long);
			case ValueHandleType.UInt64:
				return typeof(ulong);
			case ValueHandleType.Single:
				return typeof(float);
			case ValueHandleType.Double:
				return typeof(double);
			case ValueHandleType.Decimal:
				return typeof(decimal);
			case ValueHandleType.DateTime:
				return typeof(DateTime);
			case ValueHandleType.TimeSpan:
				return typeof(TimeSpan);
			case ValueHandleType.Guid:
				return typeof(Guid);
			case ValueHandleType.UniqueId:
				return typeof(UniqueId);
			case ValueHandleType.Base64:
				return typeof(byte[]);
			case ValueHandleType.List:
				return typeof(object[]);
			default:
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException());
			}
		}

		public bool ToBoolean()
		{
			ValueHandleType valueHandleType = this.type;
			if (valueHandleType == ValueHandleType.False)
			{
				return false;
			}
			if (valueHandleType == ValueHandleType.True)
			{
				return true;
			}
			if (valueHandleType == ValueHandleType.UTF8)
			{
				return XmlConverter.ToBoolean(this.bufferReader.Buffer, this.offset, this.length);
			}
			if (valueHandleType == ValueHandleType.Int8)
			{
				int @int = this.GetInt8();
				if (@int == 0)
				{
					return false;
				}
				if (@int == 1)
				{
					return true;
				}
			}
			return XmlConverter.ToBoolean(this.GetString());
		}

		public int ToInt()
		{
			ValueHandleType valueHandleType = this.type;
			if (valueHandleType == ValueHandleType.Zero)
			{
				return 0;
			}
			if (valueHandleType == ValueHandleType.One)
			{
				return 1;
			}
			if (valueHandleType == ValueHandleType.Int8)
			{
				return this.GetInt8();
			}
			if (valueHandleType == ValueHandleType.Int16)
			{
				return this.GetInt16();
			}
			if (valueHandleType == ValueHandleType.Int32)
			{
				return this.GetInt32();
			}
			if (valueHandleType == ValueHandleType.Int64)
			{
				long @int = this.GetInt64();
				if (@int >= -2147483648L && @int <= 2147483647L)
				{
					return (int)@int;
				}
			}
			if (valueHandleType == ValueHandleType.UInt64)
			{
				ulong @uint = this.GetUInt64();
				if (@uint <= 2147483647UL)
				{
					return (int)@uint;
				}
			}
			if (valueHandleType == ValueHandleType.UTF8)
			{
				return XmlConverter.ToInt32(this.bufferReader.Buffer, this.offset, this.length);
			}
			return XmlConverter.ToInt32(this.GetString());
		}

		public long ToLong()
		{
			ValueHandleType valueHandleType = this.type;
			if (valueHandleType == ValueHandleType.Zero)
			{
				return 0L;
			}
			if (valueHandleType == ValueHandleType.One)
			{
				return 1L;
			}
			if (valueHandleType == ValueHandleType.Int8)
			{
				return (long)this.GetInt8();
			}
			if (valueHandleType == ValueHandleType.Int16)
			{
				return (long)this.GetInt16();
			}
			if (valueHandleType == ValueHandleType.Int32)
			{
				return (long)this.GetInt32();
			}
			if (valueHandleType == ValueHandleType.Int64)
			{
				return this.GetInt64();
			}
			if (valueHandleType == ValueHandleType.UInt64)
			{
				ulong @uint = this.GetUInt64();
				if (@uint <= 9223372036854775807UL)
				{
					return (long)@uint;
				}
			}
			if (valueHandleType == ValueHandleType.UTF8)
			{
				return XmlConverter.ToInt64(this.bufferReader.Buffer, this.offset, this.length);
			}
			return XmlConverter.ToInt64(this.GetString());
		}

		public ulong ToULong()
		{
			ValueHandleType valueHandleType = this.type;
			if (valueHandleType == ValueHandleType.Zero)
			{
				return 0UL;
			}
			if (valueHandleType == ValueHandleType.One)
			{
				return 1UL;
			}
			if (valueHandleType >= ValueHandleType.Int8 && valueHandleType <= ValueHandleType.Int64)
			{
				long num = this.ToLong();
				if (num >= 0L)
				{
					return (ulong)num;
				}
			}
			if (valueHandleType == ValueHandleType.UInt64)
			{
				return this.GetUInt64();
			}
			if (valueHandleType == ValueHandleType.UTF8)
			{
				return XmlConverter.ToUInt64(this.bufferReader.Buffer, this.offset, this.length);
			}
			return XmlConverter.ToUInt64(this.GetString());
		}

		public float ToSingle()
		{
			ValueHandleType valueHandleType = this.type;
			if (valueHandleType == ValueHandleType.Single)
			{
				return this.GetSingle();
			}
			if (valueHandleType == ValueHandleType.Double)
			{
				double @double = this.GetDouble();
				if ((@double >= -3.4028234663852886E+38 && @double <= 3.4028234663852886E+38) || double.IsInfinity(@double) || double.IsNaN(@double))
				{
					return (float)@double;
				}
			}
			if (valueHandleType == ValueHandleType.Zero)
			{
				return 0f;
			}
			if (valueHandleType == ValueHandleType.One)
			{
				return 1f;
			}
			if (valueHandleType == ValueHandleType.Int8)
			{
				return (float)this.GetInt8();
			}
			if (valueHandleType == ValueHandleType.Int16)
			{
				return (float)this.GetInt16();
			}
			if (valueHandleType == ValueHandleType.UTF8)
			{
				return XmlConverter.ToSingle(this.bufferReader.Buffer, this.offset, this.length);
			}
			return XmlConverter.ToSingle(this.GetString());
		}

		public double ToDouble()
		{
			ValueHandleType valueHandleType = this.type;
			if (valueHandleType == ValueHandleType.Double)
			{
				return this.GetDouble();
			}
			if (valueHandleType == ValueHandleType.Single)
			{
				return (double)this.GetSingle();
			}
			if (valueHandleType == ValueHandleType.Zero)
			{
				return 0.0;
			}
			if (valueHandleType == ValueHandleType.One)
			{
				return 1.0;
			}
			if (valueHandleType == ValueHandleType.Int8)
			{
				return (double)this.GetInt8();
			}
			if (valueHandleType == ValueHandleType.Int16)
			{
				return (double)this.GetInt16();
			}
			if (valueHandleType == ValueHandleType.Int32)
			{
				return (double)this.GetInt32();
			}
			if (valueHandleType == ValueHandleType.UTF8)
			{
				return XmlConverter.ToDouble(this.bufferReader.Buffer, this.offset, this.length);
			}
			return XmlConverter.ToDouble(this.GetString());
		}

		public decimal ToDecimal()
		{
			ValueHandleType valueHandleType = this.type;
			if (valueHandleType == ValueHandleType.Decimal)
			{
				return this.GetDecimal();
			}
			if (valueHandleType == ValueHandleType.Zero)
			{
				return 0m;
			}
			if (valueHandleType == ValueHandleType.One)
			{
				return 1m;
			}
			if (valueHandleType >= ValueHandleType.Int8 && valueHandleType <= ValueHandleType.Int64)
			{
				return this.ToLong();
			}
			if (valueHandleType == ValueHandleType.UInt64)
			{
				return this.GetUInt64();
			}
			if (valueHandleType == ValueHandleType.UTF8)
			{
				return XmlConverter.ToDecimal(this.bufferReader.Buffer, this.offset, this.length);
			}
			return XmlConverter.ToDecimal(this.GetString());
		}

		public DateTime ToDateTime()
		{
			if (this.type == ValueHandleType.DateTime)
			{
				return XmlConverter.ToDateTime(this.GetInt64());
			}
			if (this.type == ValueHandleType.UTF8)
			{
				return XmlConverter.ToDateTime(this.bufferReader.Buffer, this.offset, this.length);
			}
			return XmlConverter.ToDateTime(this.GetString());
		}

		public UniqueId ToUniqueId()
		{
			if (this.type == ValueHandleType.UniqueId)
			{
				return this.GetUniqueId();
			}
			if (this.type == ValueHandleType.UTF8)
			{
				return XmlConverter.ToUniqueId(this.bufferReader.Buffer, this.offset, this.length);
			}
			return XmlConverter.ToUniqueId(this.GetString());
		}

		public TimeSpan ToTimeSpan()
		{
			if (this.type == ValueHandleType.TimeSpan)
			{
				return new TimeSpan(this.GetInt64());
			}
			if (this.type == ValueHandleType.UTF8)
			{
				return XmlConverter.ToTimeSpan(this.bufferReader.Buffer, this.offset, this.length);
			}
			return XmlConverter.ToTimeSpan(this.GetString());
		}

		public Guid ToGuid()
		{
			if (this.type == ValueHandleType.Guid)
			{
				return this.GetGuid();
			}
			if (this.type == ValueHandleType.UTF8)
			{
				return XmlConverter.ToGuid(this.bufferReader.Buffer, this.offset, this.length);
			}
			return XmlConverter.ToGuid(this.GetString());
		}

		public override string ToString()
		{
			return this.GetString();
		}

		public byte[] ToByteArray()
		{
			if (this.type == ValueHandleType.Base64)
			{
				byte[] array = new byte[this.length];
				this.GetBase64(array, 0, this.length);
				return array;
			}
			if (this.type == ValueHandleType.UTF8 && this.length % 4 == 0)
			{
				try
				{
					int num = this.length / 4 * 3;
					if (this.length > 0 && this.bufferReader.Buffer[this.offset + this.length - 1] == 61)
					{
						num--;
						if (this.bufferReader.Buffer[this.offset + this.length - 2] == 61)
						{
							num--;
						}
					}
					byte[] array2 = new byte[num];
					int bytes = ValueHandle.Base64Encoding.GetBytes(this.bufferReader.Buffer, this.offset, this.length, array2, 0);
					if (bytes != array2.Length)
					{
						byte[] array3 = new byte[bytes];
						Buffer.BlockCopy(array2, 0, array3, 0, bytes);
						array2 = array3;
					}
					return array2;
				}
				catch (FormatException)
				{
				}
			}
			byte[] bytes2;
			try
			{
				bytes2 = ValueHandle.Base64Encoding.GetBytes(XmlConverter.StripWhitespace(this.GetString()));
			}
			catch (FormatException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(ex.Message, ex.InnerException));
			}
			return bytes2;
		}

		public string GetString()
		{
			ValueHandleType valueHandleType = this.type;
			if (valueHandleType == ValueHandleType.UTF8)
			{
				return this.GetCharsText();
			}
			switch (valueHandleType)
			{
			case ValueHandleType.Empty:
				return string.Empty;
			case ValueHandleType.True:
				return "true";
			case ValueHandleType.False:
				return "false";
			case ValueHandleType.Zero:
				return "0";
			case ValueHandleType.One:
				return "1";
			case ValueHandleType.Int8:
			case ValueHandleType.Int16:
			case ValueHandleType.Int32:
				return XmlConverter.ToString(this.ToInt());
			case ValueHandleType.Int64:
				return XmlConverter.ToString(this.GetInt64());
			case ValueHandleType.UInt64:
				return XmlConverter.ToString(this.GetUInt64());
			case ValueHandleType.Single:
				return XmlConverter.ToString(this.GetSingle());
			case ValueHandleType.Double:
				return XmlConverter.ToString(this.GetDouble());
			case ValueHandleType.Decimal:
				return XmlConverter.ToString(this.GetDecimal());
			case ValueHandleType.DateTime:
				return XmlConverter.ToString(this.ToDateTime());
			case ValueHandleType.TimeSpan:
				return XmlConverter.ToString(this.ToTimeSpan());
			case ValueHandleType.Guid:
				return XmlConverter.ToString(this.ToGuid());
			case ValueHandleType.UniqueId:
				return XmlConverter.ToString(this.ToUniqueId());
			case ValueHandleType.UTF8:
				return this.GetCharsText();
			case ValueHandleType.EscapedUTF8:
				return this.GetEscapedCharsText();
			case ValueHandleType.Base64:
				return ValueHandle.Base64Encoding.GetString(this.ToByteArray());
			case ValueHandleType.Dictionary:
				return this.GetDictionaryString().Value;
			case ValueHandleType.List:
				return XmlConverter.ToString(this.ToList());
			case ValueHandleType.Char:
				return this.GetCharText();
			case ValueHandleType.Unicode:
				return this.GetUnicodeCharsText();
			case ValueHandleType.QName:
				return this.GetQNameDictionaryText();
			case ValueHandleType.ConstString:
				return ValueHandle.constStrings[this.offset];
			default:
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException());
			}
		}

		public bool Equals2(string str, bool checkLower)
		{
			if (this.type != ValueHandleType.UTF8)
			{
				return this.GetString() == str;
			}
			if (this.length != str.Length)
			{
				return false;
			}
			byte[] buffer = this.bufferReader.Buffer;
			for (int i = 0; i < this.length; i++)
			{
				byte b = buffer[i + this.offset];
				if ((char)b != str[i] && (!checkLower || char.ToLowerInvariant((char)b) != str[i]))
				{
					return false;
				}
			}
			return true;
		}

		public void Sign(XmlSigningNodeWriter writer)
		{
			switch (this.type)
			{
			case ValueHandleType.Empty:
				return;
			case ValueHandleType.Int8:
			case ValueHandleType.Int16:
			case ValueHandleType.Int32:
				writer.WriteInt32Text(this.ToInt());
				return;
			case ValueHandleType.Int64:
				writer.WriteInt64Text(this.GetInt64());
				return;
			case ValueHandleType.UInt64:
				writer.WriteUInt64Text(this.GetUInt64());
				return;
			case ValueHandleType.Single:
				writer.WriteFloatText(this.GetSingle());
				return;
			case ValueHandleType.Double:
				writer.WriteDoubleText(this.GetDouble());
				return;
			case ValueHandleType.Decimal:
				writer.WriteDecimalText(this.GetDecimal());
				return;
			case ValueHandleType.DateTime:
				writer.WriteDateTimeText(this.ToDateTime());
				return;
			case ValueHandleType.TimeSpan:
				writer.WriteTimeSpanText(this.ToTimeSpan());
				return;
			case ValueHandleType.Guid:
				writer.WriteGuidText(this.ToGuid());
				return;
			case ValueHandleType.UniqueId:
				writer.WriteUniqueIdText(this.ToUniqueId());
				return;
			case ValueHandleType.UTF8:
				writer.WriteEscapedText(this.bufferReader.Buffer, this.offset, this.length);
				return;
			case ValueHandleType.Base64:
				writer.WriteBase64Text(this.bufferReader.Buffer, 0, this.bufferReader.Buffer, this.offset, this.length);
				return;
			}
			writer.WriteEscapedText(this.GetString());
		}

		public object[] ToList()
		{
			return this.bufferReader.GetList(this.offset, this.length);
		}

		public object ToObject()
		{
			switch (this.type)
			{
			case ValueHandleType.Empty:
			case ValueHandleType.UTF8:
			case ValueHandleType.EscapedUTF8:
			case ValueHandleType.Dictionary:
			case ValueHandleType.Char:
			case ValueHandleType.Unicode:
			case ValueHandleType.ConstString:
				return this.ToString();
			case ValueHandleType.True:
			case ValueHandleType.False:
				return this.ToBoolean();
			case ValueHandleType.Zero:
			case ValueHandleType.One:
			case ValueHandleType.Int8:
			case ValueHandleType.Int16:
			case ValueHandleType.Int32:
				return this.ToInt();
			case ValueHandleType.Int64:
				return this.ToLong();
			case ValueHandleType.UInt64:
				return this.GetUInt64();
			case ValueHandleType.Single:
				return this.ToSingle();
			case ValueHandleType.Double:
				return this.ToDouble();
			case ValueHandleType.Decimal:
				return this.ToDecimal();
			case ValueHandleType.DateTime:
				return this.ToDateTime();
			case ValueHandleType.TimeSpan:
				return this.ToTimeSpan();
			case ValueHandleType.Guid:
				return this.ToGuid();
			case ValueHandleType.UniqueId:
				return this.ToUniqueId();
			case ValueHandleType.Base64:
				return this.ToByteArray();
			case ValueHandleType.List:
				return this.ToList();
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException());
		}

		public bool TryReadBase64(byte[] buffer, int offset, int count, out int actual)
		{
			if (this.type == ValueHandleType.Base64)
			{
				actual = Math.Min(this.length, count);
				this.GetBase64(buffer, offset, actual);
				this.offset += actual;
				this.length -= actual;
				return true;
			}
			if (this.type == ValueHandleType.UTF8 && count >= 3 && this.length % 4 == 0)
			{
				try
				{
					int num = Math.Min(count / 3 * 4, this.length);
					actual = ValueHandle.Base64Encoding.GetBytes(this.bufferReader.Buffer, this.offset, num, buffer, offset);
					this.offset += num;
					this.length -= num;
					return true;
				}
				catch (FormatException)
				{
				}
			}
			actual = 0;
			return false;
		}

		public bool TryReadChars(char[] chars, int offset, int count, out int actual)
		{
			if (this.type == ValueHandleType.Unicode)
			{
				return this.TryReadUnicodeChars(chars, offset, count, out actual);
			}
			if (this.type != ValueHandleType.UTF8)
			{
				actual = 0;
				return false;
			}
			int num = offset;
			int num2 = count;
			byte[] buffer = this.bufferReader.Buffer;
			int num3 = this.offset;
			int num4 = this.length;
			bool flag = false;
			for (;;)
			{
				if (num2 > 0 && num4 > 0)
				{
					byte b = buffer[num3];
					if (b < 128)
					{
						chars[num] = (char)b;
						num3++;
						num4--;
						num++;
						num2--;
						continue;
					}
				}
				if (num2 == 0 || num4 == 0 || flag)
				{
					break;
				}
				UTF8Encoding utf8Encoding = new UTF8Encoding(false, true);
				int num5;
				int num6;
				try
				{
					if (num2 >= utf8Encoding.GetMaxCharCount(num4) || num2 >= utf8Encoding.GetCharCount(buffer, num3, num4))
					{
						num5 = utf8Encoding.GetChars(buffer, num3, num4, chars, num);
						num6 = num4;
					}
					else
					{
						Decoder decoder = utf8Encoding.GetDecoder();
						num6 = Math.Min(num2, num4);
						num5 = decoder.GetChars(buffer, num3, num6, chars, num);
						while (num5 == 0)
						{
							if (num6 >= 3 && num2 < 2)
							{
								flag = true;
								break;
							}
							num5 = decoder.GetChars(buffer, num3 + num6, 1, chars, num);
							num6++;
						}
						num6 = utf8Encoding.GetByteCount(chars, num, num5);
					}
				}
				catch (FormatException ex)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateEncodingException(buffer, num3, num4, ex));
				}
				num3 += num6;
				num4 -= num6;
				num += num5;
				num2 -= num5;
			}
			this.offset = num3;
			this.length = num4;
			actual = count - num2;
			return true;
		}

		private bool TryReadUnicodeChars(char[] chars, int offset, int count, out int actual)
		{
			int num = Math.Min(count, this.length / 2);
			for (int i = 0; i < num; i++)
			{
				chars[offset + i] = (char)this.bufferReader.GetInt16(this.offset + i * 2);
			}
			this.offset += num * 2;
			this.length -= num * 2;
			actual = num;
			return true;
		}

		public bool TryGetDictionaryString(out XmlDictionaryString value)
		{
			if (this.type == ValueHandleType.Dictionary)
			{
				value = this.GetDictionaryString();
				return true;
			}
			value = null;
			return false;
		}

		public bool TryGetByteArrayLength(out int length)
		{
			if (this.type == ValueHandleType.Base64)
			{
				length = this.length;
				return true;
			}
			length = 0;
			return false;
		}

		private string GetCharsText()
		{
			if (this.length == 1 && this.bufferReader.GetByte(this.offset) == 49)
			{
				return "1";
			}
			return this.bufferReader.GetString(this.offset, this.length);
		}

		private string GetUnicodeCharsText()
		{
			return this.bufferReader.GetUnicodeString(this.offset, this.length);
		}

		private string GetEscapedCharsText()
		{
			return this.bufferReader.GetEscapedString(this.offset, this.length);
		}

		private string GetCharText()
		{
			int @char = this.GetChar();
			if (@char > 65535)
			{
				SurrogateChar surrogateChar = new SurrogateChar(@char);
				return new string(new char[] { surrogateChar.HighChar, surrogateChar.LowChar }, 0, 2);
			}
			return ((char)@char).ToString();
		}

		private int GetChar()
		{
			return this.offset;
		}

		private int GetInt8()
		{
			return this.bufferReader.GetInt8(this.offset);
		}

		private int GetInt16()
		{
			return this.bufferReader.GetInt16(this.offset);
		}

		private int GetInt32()
		{
			return this.bufferReader.GetInt32(this.offset);
		}

		private long GetInt64()
		{
			return this.bufferReader.GetInt64(this.offset);
		}

		private ulong GetUInt64()
		{
			return this.bufferReader.GetUInt64(this.offset);
		}

		private float GetSingle()
		{
			return this.bufferReader.GetSingle(this.offset);
		}

		private double GetDouble()
		{
			return this.bufferReader.GetDouble(this.offset);
		}

		private decimal GetDecimal()
		{
			return this.bufferReader.GetDecimal(this.offset);
		}

		private UniqueId GetUniqueId()
		{
			return this.bufferReader.GetUniqueId(this.offset);
		}

		private Guid GetGuid()
		{
			return this.bufferReader.GetGuid(this.offset);
		}

		private void GetBase64(byte[] buffer, int offset, int count)
		{
			this.bufferReader.GetBase64(this.offset, buffer, offset, count);
		}

		private XmlDictionaryString GetDictionaryString()
		{
			return this.bufferReader.GetDictionaryString(this.offset);
		}

		private string GetQNameDictionaryText()
		{
			return PrefixHandle.GetString(PrefixHandle.GetAlphaPrefix(this.length)) + ":" + this.bufferReader.GetDictionaryString(this.offset);
		}

		private XmlBufferReader bufferReader;

		private ValueHandleType type;

		private int offset;

		private int length;

		private static Base64Encoding base64Encoding;

		private static string[] constStrings = new string[] { "string", "number", "array", "object", "boolean", "null" };
	}
}
