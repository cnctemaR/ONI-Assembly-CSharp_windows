using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace System.Xml
{
	internal class XmlBufferReader
	{
		public XmlBufferReader(XmlDictionaryReader reader)
		{
			this.reader = reader;
		}

		public XmlBufferReader(byte[] buffer)
		{
			this.reader = null;
			this.buffer = buffer;
		}

		public static XmlBufferReader Empty
		{
			get
			{
				return XmlBufferReader.empty;
			}
		}

		public byte[] Buffer
		{
			get
			{
				return this.buffer;
			}
		}

		public bool IsStreamed
		{
			get
			{
				return this.stream != null;
			}
		}

		public void SetBuffer(Stream stream, IXmlDictionary dictionary, XmlBinaryReaderSession session)
		{
			if (this.streamBuffer == null)
			{
				this.streamBuffer = new byte[128];
			}
			this.SetBuffer(stream, this.streamBuffer, 0, 0, dictionary, session);
			this.windowOffset = 0;
			this.windowOffsetMax = this.streamBuffer.Length;
		}

		public void SetBuffer(byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlBinaryReaderSession session)
		{
			this.SetBuffer(null, buffer, offset, count, dictionary, session);
		}

		private void SetBuffer(Stream stream, byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlBinaryReaderSession session)
		{
			this.stream = stream;
			this.buffer = buffer;
			this.offsetMin = offset;
			this.offset = offset;
			this.offsetMax = offset + count;
			this.dictionary = dictionary;
			this.session = session;
		}

		public void Close()
		{
			if (this.streamBuffer != null && this.streamBuffer.Length > 4096)
			{
				this.streamBuffer = null;
			}
			if (this.stream != null)
			{
				this.stream.Close();
				this.stream = null;
			}
			this.buffer = XmlBufferReader.emptyByteArray;
			this.offset = 0;
			this.offsetMax = 0;
			this.windowOffset = 0;
			this.windowOffsetMax = 0;
			this.dictionary = null;
			this.session = null;
		}

		public bool EndOfFile
		{
			get
			{
				return this.offset == this.offsetMax && !this.TryEnsureByte();
			}
		}

		public byte GetByte()
		{
			int num = this.offset;
			if (num < this.offsetMax)
			{
				return this.buffer[num];
			}
			return this.GetByteHard();
		}

		public void SkipByte()
		{
			this.Advance(1);
		}

		private byte GetByteHard()
		{
			this.EnsureByte();
			return this.buffer[this.offset];
		}

		public byte[] GetBuffer(int count, out int offset)
		{
			offset = this.offset;
			if (offset <= this.offsetMax - count)
			{
				return this.buffer;
			}
			return this.GetBufferHard(count, out offset);
		}

		public byte[] GetBuffer(int count, out int offset, out int offsetMax)
		{
			offset = this.offset;
			if (offset <= this.offsetMax - count)
			{
				offsetMax = this.offset + count;
			}
			else
			{
				this.TryEnsureBytes(Math.Min(count, this.windowOffsetMax - offset));
				offsetMax = this.offsetMax;
			}
			return this.buffer;
		}

		public byte[] GetBuffer(out int offset, out int offsetMax)
		{
			offset = this.offset;
			offsetMax = this.offsetMax;
			return this.buffer;
		}

		private byte[] GetBufferHard(int count, out int offset)
		{
			offset = this.offset;
			this.EnsureBytes(count);
			return this.buffer;
		}

		private void EnsureByte()
		{
			if (!this.TryEnsureByte())
			{
				XmlExceptionHelper.ThrowUnexpectedEndOfFile(this.reader);
			}
		}

		private bool TryEnsureByte()
		{
			if (this.stream == null)
			{
				return false;
			}
			if (this.offsetMax >= this.windowOffsetMax)
			{
				XmlExceptionHelper.ThrowMaxBytesPerReadExceeded(this.reader, this.windowOffsetMax - this.windowOffset);
			}
			if (this.offsetMax >= this.buffer.Length)
			{
				return this.TryEnsureBytes(1);
			}
			int num = this.stream.ReadByte();
			if (num == -1)
			{
				return false;
			}
			byte[] array = this.buffer;
			int num2 = this.offsetMax;
			this.offsetMax = num2 + 1;
			array[num2] = (byte)num;
			return true;
		}

		private void EnsureBytes(int count)
		{
			if (!this.TryEnsureBytes(count))
			{
				XmlExceptionHelper.ThrowUnexpectedEndOfFile(this.reader);
			}
		}

		private bool TryEnsureBytes(int count)
		{
			if (this.stream == null)
			{
				return false;
			}
			if (this.offset > 2147483647 - count)
			{
				XmlExceptionHelper.ThrowMaxBytesPerReadExceeded(this.reader, this.windowOffsetMax - this.windowOffset);
			}
			int num = this.offset + count;
			if (num < this.offsetMax)
			{
				return true;
			}
			if (num > this.windowOffsetMax)
			{
				XmlExceptionHelper.ThrowMaxBytesPerReadExceeded(this.reader, this.windowOffsetMax - this.windowOffset);
			}
			if (num > this.buffer.Length)
			{
				byte[] array = new byte[Math.Max(num, this.buffer.Length * 2)];
				global::System.Buffer.BlockCopy(this.buffer, 0, array, 0, this.offsetMax);
				this.buffer = array;
				this.streamBuffer = array;
			}
			int num2;
			for (int i = num - this.offsetMax; i > 0; i -= num2)
			{
				num2 = this.stream.Read(this.buffer, this.offsetMax, i);
				if (num2 == 0)
				{
					return false;
				}
				this.offsetMax += num2;
			}
			return true;
		}

		public void Advance(int count)
		{
			this.offset += count;
		}

		public void InsertBytes(byte[] buffer, int offset, int count)
		{
			if (this.offsetMax > buffer.Length - count)
			{
				byte[] array = new byte[this.offsetMax + count];
				global::System.Buffer.BlockCopy(this.buffer, 0, array, 0, this.offsetMax);
				this.buffer = array;
				this.streamBuffer = array;
			}
			global::System.Buffer.BlockCopy(this.buffer, this.offset, this.buffer, this.offset + count, this.offsetMax - this.offset);
			this.offsetMax += count;
			global::System.Buffer.BlockCopy(buffer, offset, this.buffer, this.offset, count);
		}

		public void SetWindow(int windowOffset, int windowLength)
		{
			if (windowOffset > 2147483647 - windowLength)
			{
				windowLength = int.MaxValue - windowOffset;
			}
			if (this.offset != windowOffset)
			{
				global::System.Buffer.BlockCopy(this.buffer, this.offset, this.buffer, windowOffset, this.offsetMax - this.offset);
				this.offsetMax = windowOffset + (this.offsetMax - this.offset);
				this.offset = windowOffset;
			}
			this.windowOffset = windowOffset;
			this.windowOffsetMax = Math.Max(windowOffset + windowLength, this.offsetMax);
		}

		public int Offset
		{
			get
			{
				return this.offset;
			}
			set
			{
				this.offset = value;
			}
		}

		public int ReadBytes(int count)
		{
			int num = this.offset;
			if (num > this.offsetMax - count)
			{
				this.EnsureBytes(count);
			}
			this.offset += count;
			return num;
		}

		public int ReadMultiByteUInt31()
		{
			int num = (int)this.GetByte();
			this.Advance(1);
			if ((num & 128) == 0)
			{
				return num;
			}
			num &= 127;
			int @byte = (int)this.GetByte();
			this.Advance(1);
			num |= (@byte & 127) << 7;
			if ((@byte & 128) == 0)
			{
				return num;
			}
			int byte2 = (int)this.GetByte();
			this.Advance(1);
			num |= (byte2 & 127) << 14;
			if ((byte2 & 128) == 0)
			{
				return num;
			}
			int byte3 = (int)this.GetByte();
			this.Advance(1);
			num |= (byte3 & 127) << 21;
			if ((byte3 & 128) == 0)
			{
				return num;
			}
			int byte4 = (int)this.GetByte();
			this.Advance(1);
			num |= byte4 << 28;
			if ((byte4 & 248) != 0)
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this.reader);
			}
			return num;
		}

		public int ReadUInt8()
		{
			int @byte = (int)this.GetByte();
			this.Advance(1);
			return @byte;
		}

		public int ReadInt8()
		{
			return (int)((sbyte)this.ReadUInt8());
		}

		public int ReadUInt16()
		{
			int num;
			byte[] array = this.GetBuffer(2, out num);
			int num2 = (int)array[num] + ((int)array[num + 1] << 8);
			this.Advance(2);
			return num2;
		}

		public int ReadInt16()
		{
			return (int)((short)this.ReadUInt16());
		}

		public int ReadInt32()
		{
			int num;
			byte[] array = this.GetBuffer(4, out num);
			byte b = array[num];
			byte b2 = array[num + 1];
			byte b3 = array[num + 2];
			int num2 = (int)array[num + 3];
			this.Advance(4);
			return (((num2 << 8) + (int)b3 << 8) + (int)b2 << 8) + (int)b;
		}

		public int ReadUInt31()
		{
			int num = this.ReadInt32();
			if (num < 0)
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this.reader);
			}
			return num;
		}

		public long ReadInt64()
		{
			long num = (long)((ulong)this.ReadInt32());
			return (long)(((ulong)this.ReadInt32() << 32) + (ulong)num);
		}

		[SecuritySafeCritical]
		public unsafe float ReadSingle()
		{
			int num;
			byte[] array = this.GetBuffer(4, out num);
			float num2;
			byte* ptr = (byte*)(&num2);
			*ptr = array[num];
			ptr[1] = array[num + 1];
			ptr[2] = array[num + 2];
			ptr[3] = array[num + 3];
			this.Advance(4);
			return num2;
		}

		[SecuritySafeCritical]
		public unsafe double ReadDouble()
		{
			int num;
			byte[] array = this.GetBuffer(8, out num);
			double num2;
			byte* ptr = (byte*)(&num2);
			*ptr = array[num];
			ptr[1] = array[num + 1];
			ptr[2] = array[num + 2];
			ptr[3] = array[num + 3];
			ptr[4] = array[num + 4];
			ptr[5] = array[num + 5];
			ptr[6] = array[num + 6];
			ptr[7] = array[num + 7];
			this.Advance(8);
			return num2;
		}

		[SecuritySafeCritical]
		public unsafe decimal ReadDecimal()
		{
			int num;
			byte[] array = this.GetBuffer(16, out num);
			byte b = array[num];
			byte b2 = array[num + 1];
			byte b3 = array[num + 2];
			int num2 = ((((int)array[num + 3] << 8) + (int)b3 << 8) + (int)b2 << 8) + (int)b;
			if ((num2 & 2130771967) == 0 && (num2 & 16711680) <= 1835008)
			{
				decimal num3;
				byte* ptr = (byte*)(&num3);
				for (int i = 0; i < 16; i++)
				{
					ptr[i] = array[num + i];
				}
				this.Advance(16);
				return num3;
			}
			XmlExceptionHelper.ThrowInvalidBinaryFormat(this.reader);
			return 0m;
		}

		public UniqueId ReadUniqueId()
		{
			int num;
			UniqueId uniqueId = new UniqueId(this.GetBuffer(16, out num), num);
			this.Advance(16);
			return uniqueId;
		}

		public DateTime ReadDateTime()
		{
			long num = 0L;
			DateTime dateTime;
			try
			{
				num = this.ReadInt64();
				dateTime = DateTime.FromBinary(num);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(num.ToString(CultureInfo.InvariantCulture), "DateTime", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(num.ToString(CultureInfo.InvariantCulture), "DateTime", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(num.ToString(CultureInfo.InvariantCulture), "DateTime", ex3));
			}
			return dateTime;
		}

		public TimeSpan ReadTimeSpan()
		{
			long num = 0L;
			TimeSpan timeSpan;
			try
			{
				num = this.ReadInt64();
				timeSpan = TimeSpan.FromTicks(num);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(num.ToString(CultureInfo.InvariantCulture), "TimeSpan", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(num.ToString(CultureInfo.InvariantCulture), "TimeSpan", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(num.ToString(CultureInfo.InvariantCulture), "TimeSpan", ex3));
			}
			return timeSpan;
		}

		public Guid ReadGuid()
		{
			int num;
			this.GetBuffer(16, out num);
			Guid guid = this.GetGuid(num);
			this.Advance(16);
			return guid;
		}

		public string ReadUTF8String(int length)
		{
			int num;
			this.GetBuffer(length, out num);
			char[] charBuffer = this.GetCharBuffer(length);
			int num2 = this.GetChars(num, length, charBuffer);
			string text = new string(charBuffer, 0, num2);
			this.Advance(length);
			return text;
		}

		[SecurityCritical]
		public unsafe void UnsafeReadArray(byte* dst, byte* dstMax)
		{
			this.UnsafeReadArray(dst, (int)((long)(dstMax - dst)));
		}

		[SecurityCritical]
		private unsafe void UnsafeReadArray(byte* dst, int length)
		{
			if (this.stream != null)
			{
				while (length >= 256)
				{
					byte[] array = this.GetBuffer(256, out this.offset);
					for (int i = 0; i < 256; i++)
					{
						*(dst++) = array[this.offset + i];
					}
					this.Advance(256);
					length -= 256;
				}
			}
			if (length > 0)
			{
				fixed (byte* ptr = &this.GetBuffer(length, out this.offset)[this.offset])
				{
					byte* ptr2 = ptr;
					byte* ptr3 = dst + length;
					while (dst < ptr3)
					{
						*dst = *ptr2;
						dst++;
						ptr2++;
					}
				}
				this.Advance(length);
			}
		}

		private char[] GetCharBuffer(int count)
		{
			if (count > 1024)
			{
				return new char[count];
			}
			if (this.chars == null || this.chars.Length < count)
			{
				this.chars = new char[count];
			}
			return this.chars;
		}

		private int GetChars(int offset, int length, char[] chars)
		{
			byte[] array = this.buffer;
			for (int i = 0; i < length; i++)
			{
				byte b = array[offset + i];
				if (b >= 128)
				{
					return i + XmlConverter.ToChars(array, offset + i, length - i, chars, i);
				}
				chars[i] = (char)b;
			}
			return length;
		}

		private int GetChars(int offset, int length, char[] chars, int charOffset)
		{
			byte[] array = this.buffer;
			for (int i = 0; i < length; i++)
			{
				byte b = array[offset + i];
				if (b >= 128)
				{
					return i + XmlConverter.ToChars(array, offset + i, length - i, chars, charOffset + i);
				}
				chars[charOffset + i] = (char)b;
			}
			return length;
		}

		public string GetString(int offset, int length)
		{
			char[] charBuffer = this.GetCharBuffer(length);
			int num = this.GetChars(offset, length, charBuffer);
			return new string(charBuffer, 0, num);
		}

		public string GetUnicodeString(int offset, int length)
		{
			return XmlConverter.ToStringUnicode(this.buffer, offset, length);
		}

		public string GetString(int offset, int length, XmlNameTable nameTable)
		{
			char[] charBuffer = this.GetCharBuffer(length);
			int num = this.GetChars(offset, length, charBuffer);
			return nameTable.Add(charBuffer, 0, num);
		}

		public int GetEscapedChars(int offset, int length, char[] chars)
		{
			byte[] array = this.buffer;
			int num = 0;
			int num2 = offset;
			int num3 = offset + length;
			for (;;)
			{
				if (offset >= num3 || !this.IsAttrChar((int)array[offset]))
				{
					num += this.GetChars(num2, offset - num2, chars, num);
					if (offset == num3)
					{
						break;
					}
					num2 = offset;
					if (array[offset] == 38)
					{
						while (offset < num3 && array[offset] != 59)
						{
							offset++;
						}
						offset++;
						int charEntity = this.GetCharEntity(num2, offset - num2);
						num2 = offset;
						if (charEntity > 65535)
						{
							SurrogateChar surrogateChar = new SurrogateChar(charEntity);
							chars[num++] = surrogateChar.HighChar;
							chars[num++] = surrogateChar.LowChar;
						}
						else
						{
							chars[num++] = (char)charEntity;
						}
					}
					else if (array[offset] == 10 || array[offset] == 9)
					{
						chars[num++] = ' ';
						offset++;
						num2 = offset;
					}
					else
					{
						chars[num++] = ' ';
						offset++;
						if (offset < num3 && array[offset] == 10)
						{
							offset++;
						}
						num2 = offset;
					}
				}
				else
				{
					offset++;
				}
			}
			return num;
		}

		private bool IsAttrChar(int ch)
		{
			return ch - 9 > 1 && ch != 13 && ch != 38;
		}

		public string GetEscapedString(int offset, int length)
		{
			char[] charBuffer = this.GetCharBuffer(length);
			int escapedChars = this.GetEscapedChars(offset, length, charBuffer);
			return new string(charBuffer, 0, escapedChars);
		}

		public string GetEscapedString(int offset, int length, XmlNameTable nameTable)
		{
			char[] charBuffer = this.GetCharBuffer(length);
			int escapedChars = this.GetEscapedChars(offset, length, charBuffer);
			return nameTable.Add(charBuffer, 0, escapedChars);
		}

		private int GetLessThanCharEntity(int offset, int length)
		{
			byte[] array = this.buffer;
			if (length != 4 || array[offset + 1] != 108 || array[offset + 2] != 116)
			{
				XmlExceptionHelper.ThrowInvalidCharRef(this.reader);
			}
			return 60;
		}

		private int GetGreaterThanCharEntity(int offset, int length)
		{
			byte[] array = this.buffer;
			if (length != 4 || array[offset + 1] != 103 || array[offset + 2] != 116)
			{
				XmlExceptionHelper.ThrowInvalidCharRef(this.reader);
			}
			return 62;
		}

		private int GetQuoteCharEntity(int offset, int length)
		{
			byte[] array = this.buffer;
			if (length != 6 || array[offset + 1] != 113 || array[offset + 2] != 117 || array[offset + 3] != 111 || array[offset + 4] != 116)
			{
				XmlExceptionHelper.ThrowInvalidCharRef(this.reader);
			}
			return 34;
		}

		private int GetAmpersandCharEntity(int offset, int length)
		{
			byte[] array = this.buffer;
			if (length != 5 || array[offset + 1] != 97 || array[offset + 2] != 109 || array[offset + 3] != 112)
			{
				XmlExceptionHelper.ThrowInvalidCharRef(this.reader);
			}
			return 38;
		}

		private int GetApostropheCharEntity(int offset, int length)
		{
			byte[] array = this.buffer;
			if (length != 6 || array[offset + 1] != 97 || array[offset + 2] != 112 || array[offset + 3] != 111 || array[offset + 4] != 115)
			{
				XmlExceptionHelper.ThrowInvalidCharRef(this.reader);
			}
			return 39;
		}

		private int GetDecimalCharEntity(int offset, int length)
		{
			byte[] array = this.buffer;
			int num = 0;
			for (int i = 2; i < length - 1; i++)
			{
				byte b = array[offset + i];
				if (b < 48 || b > 57)
				{
					XmlExceptionHelper.ThrowInvalidCharRef(this.reader);
				}
				num = num * 10 + (int)(b - 48);
				if (num > 1114111)
				{
					XmlExceptionHelper.ThrowInvalidCharRef(this.reader);
				}
			}
			return num;
		}

		private int GetHexCharEntity(int offset, int length)
		{
			byte[] array = this.buffer;
			int num = 0;
			for (int i = 3; i < length - 1; i++)
			{
				byte b = array[offset + i];
				int num2 = 0;
				if (b >= 48 && b <= 57)
				{
					num2 = (int)(b - 48);
				}
				else if (b >= 97 && b <= 102)
				{
					num2 = (int)(10 + (b - 97));
				}
				else if (b >= 65 && b <= 70)
				{
					num2 = (int)(10 + (b - 65));
				}
				else
				{
					XmlExceptionHelper.ThrowInvalidCharRef(this.reader);
				}
				num = num * 16 + num2;
				if (num > 1114111)
				{
					XmlExceptionHelper.ThrowInvalidCharRef(this.reader);
				}
			}
			return num;
		}

		public int GetCharEntity(int offset, int length)
		{
			if (length < 3)
			{
				XmlExceptionHelper.ThrowInvalidCharRef(this.reader);
			}
			byte[] array = this.buffer;
			byte b = array[offset + 1];
			if (b <= 97)
			{
				if (b != 35)
				{
					if (b == 97)
					{
						if (array[offset + 2] == 109)
						{
							return this.GetAmpersandCharEntity(offset, length);
						}
						return this.GetApostropheCharEntity(offset, length);
					}
				}
				else
				{
					if (array[offset + 2] == 120)
					{
						return this.GetHexCharEntity(offset, length);
					}
					return this.GetDecimalCharEntity(offset, length);
				}
			}
			else
			{
				if (b == 103)
				{
					return this.GetGreaterThanCharEntity(offset, length);
				}
				if (b == 108)
				{
					return this.GetLessThanCharEntity(offset, length);
				}
				if (b == 113)
				{
					return this.GetQuoteCharEntity(offset, length);
				}
			}
			XmlExceptionHelper.ThrowInvalidCharRef(this.reader);
			return 0;
		}

		public bool IsWhitespaceKey(int key)
		{
			string value = this.GetDictionaryString(key).Value;
			for (int i = 0; i < value.Length; i++)
			{
				if (!XmlConverter.IsWhitespace(value[i]))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsWhitespaceUTF8(int offset, int length)
		{
			byte[] array = this.buffer;
			for (int i = 0; i < length; i++)
			{
				if (!XmlConverter.IsWhitespace((char)array[offset + i]))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsWhitespaceUnicode(int offset, int length)
		{
			byte[] array = this.buffer;
			for (int i = 0; i < length; i += 2)
			{
				if (!XmlConverter.IsWhitespace((char)this.GetInt16(offset + i)))
				{
					return false;
				}
			}
			return true;
		}

		public bool Equals2(int key1, int key2, XmlBufferReader bufferReader2)
		{
			return key1 == key2 || this.GetDictionaryString(key1).Value == bufferReader2.GetDictionaryString(key2).Value;
		}

		public bool Equals2(int key1, XmlDictionaryString xmlString2)
		{
			if ((key1 & 1) == 0 && xmlString2.Dictionary == this.dictionary)
			{
				return xmlString2.Key == key1 >> 1;
			}
			return this.GetDictionaryString(key1).Value == xmlString2.Value;
		}

		public bool Equals2(int offset1, int length1, byte[] buffer2)
		{
			int num = buffer2.Length;
			if (length1 != num)
			{
				return false;
			}
			byte[] array = this.buffer;
			for (int i = 0; i < length1; i++)
			{
				if (array[offset1 + i] != buffer2[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool Equals2(int offset1, int length1, XmlBufferReader bufferReader2, int offset2, int length2)
		{
			if (length1 != length2)
			{
				return false;
			}
			byte[] array = this.buffer;
			byte[] array2 = bufferReader2.buffer;
			for (int i = 0; i < length1; i++)
			{
				if (array[offset1 + i] != array2[offset2 + i])
				{
					return false;
				}
			}
			return true;
		}

		public bool Equals2(int offset1, int length1, int offset2, int length2)
		{
			if (length1 != length2)
			{
				return false;
			}
			if (offset1 == offset2)
			{
				return true;
			}
			byte[] array = this.buffer;
			for (int i = 0; i < length1; i++)
			{
				if (array[offset1 + i] != array[offset2 + i])
				{
					return false;
				}
			}
			return true;
		}

		[SecuritySafeCritical]
		public unsafe bool Equals2(int offset1, int length1, string s2)
		{
			int length2 = s2.Length;
			if (length1 < length2 || length1 > length2 * 3)
			{
				return false;
			}
			byte[] array = this.buffer;
			if (length1 < 8)
			{
				int num = Math.Min(length1, length2);
				for (int i = 0; i < num; i++)
				{
					byte b = array[offset1 + i];
					if (b >= 128)
					{
						return XmlConverter.ToString(array, offset1, length1) == s2;
					}
					if (s2[i] != (char)b)
					{
						return false;
					}
				}
				return length1 == length2;
			}
			int num2 = Math.Min(length1, length2);
			fixed (byte* ptr = &array[offset1])
			{
				byte* ptr2 = ptr;
				byte* ptr3 = ptr2 + num2;
				fixed (string text = s2)
				{
					char* ptr4 = text;
					if (ptr4 != null)
					{
						ptr4 += RuntimeHelpers.OffsetToStringData / 2;
					}
					char* ptr5 = ptr4;
					int num3 = 0;
					while (ptr2 < ptr3 && *ptr2 < 128)
					{
						num3 = (int)(*ptr2 - (byte)(*ptr5));
						if (num3 != 0)
						{
							break;
						}
						ptr2++;
						ptr5++;
					}
					if (num3 != 0)
					{
						return false;
					}
					if (ptr2 == ptr3)
					{
						return length1 == length2;
					}
				}
			}
			return XmlConverter.ToString(array, offset1, length1) == s2;
		}

		public int Compare(int offset1, int length1, int offset2, int length2)
		{
			byte[] array = this.buffer;
			int num = Math.Min(length1, length2);
			for (int i = 0; i < num; i++)
			{
				int num2 = (int)(array[offset1 + i] - array[offset2 + i]);
				if (num2 != 0)
				{
					return num2;
				}
			}
			return length1 - length2;
		}

		public byte GetByte(int offset)
		{
			return this.buffer[offset];
		}

		public int GetInt8(int offset)
		{
			return (int)((sbyte)this.GetByte(offset));
		}

		public int GetInt16(int offset)
		{
			byte[] array = this.buffer;
			return (int)((short)((int)array[offset] + ((int)array[offset + 1] << 8)));
		}

		public int GetInt32(int offset)
		{
			byte[] array = this.buffer;
			byte b = array[offset];
			byte b2 = array[offset + 1];
			byte b3 = array[offset + 2];
			return ((((int)array[offset + 3] << 8) + (int)b3 << 8) + (int)b2 << 8) + (int)b;
		}

		public long GetInt64(int offset)
		{
			byte[] array = this.buffer;
			byte b = array[offset];
			byte b2 = array[offset + 1];
			byte b3 = array[offset + 2];
			long num = (long)((ulong)(((((int)array[offset + 3] << 8) + (int)b3 << 8) + (int)b2 << 8) + (int)b));
			b = array[offset + 4];
			b2 = array[offset + 5];
			b3 = array[offset + 6];
			return (long)(((ulong)(((((int)array[offset + 7] << 8) + (int)b3 << 8) + (int)b2 << 8) + (int)b) << 32) + (ulong)num);
		}

		public ulong GetUInt64(int offset)
		{
			return (ulong)this.GetInt64(offset);
		}

		[SecuritySafeCritical]
		public unsafe float GetSingle(int offset)
		{
			byte[] array = this.buffer;
			float num;
			byte* ptr = (byte*)(&num);
			*ptr = array[offset];
			ptr[1] = array[offset + 1];
			ptr[2] = array[offset + 2];
			ptr[3] = array[offset + 3];
			return num;
		}

		[SecuritySafeCritical]
		public unsafe double GetDouble(int offset)
		{
			byte[] array = this.buffer;
			double num;
			byte* ptr = (byte*)(&num);
			*ptr = array[offset];
			ptr[1] = array[offset + 1];
			ptr[2] = array[offset + 2];
			ptr[3] = array[offset + 3];
			ptr[4] = array[offset + 4];
			ptr[5] = array[offset + 5];
			ptr[6] = array[offset + 6];
			ptr[7] = array[offset + 7];
			return num;
		}

		[SecuritySafeCritical]
		public unsafe decimal GetDecimal(int offset)
		{
			byte[] array = this.buffer;
			byte b = array[offset];
			byte b2 = array[offset + 1];
			byte b3 = array[offset + 2];
			int num = ((((int)array[offset + 3] << 8) + (int)b3 << 8) + (int)b2 << 8) + (int)b;
			if ((num & 2130771967) == 0 && (num & 16711680) <= 1835008)
			{
				decimal num2;
				byte* ptr = (byte*)(&num2);
				for (int i = 0; i < 16; i++)
				{
					ptr[i] = array[offset + i];
				}
				return num2;
			}
			XmlExceptionHelper.ThrowInvalidBinaryFormat(this.reader);
			return 0m;
		}

		public UniqueId GetUniqueId(int offset)
		{
			return new UniqueId(this.buffer, offset);
		}

		public Guid GetGuid(int offset)
		{
			if (this.guid == null)
			{
				this.guid = new byte[16];
			}
			global::System.Buffer.BlockCopy(this.buffer, offset, this.guid, 0, this.guid.Length);
			return new Guid(this.guid);
		}

		public void GetBase64(int srcOffset, byte[] buffer, int dstOffset, int count)
		{
			global::System.Buffer.BlockCopy(this.buffer, srcOffset, buffer, dstOffset, count);
		}

		public XmlBinaryNodeType GetNodeType()
		{
			return (XmlBinaryNodeType)this.GetByte();
		}

		public void SkipNodeType()
		{
			this.SkipByte();
		}

		public object[] GetList(int offset, int count)
		{
			int num = this.Offset;
			this.Offset = offset;
			object[] array2;
			try
			{
				object[] array = new object[count];
				for (int i = 0; i < count; i++)
				{
					XmlBinaryNodeType nodeType = this.GetNodeType();
					this.SkipNodeType();
					this.ReadValue(nodeType, this.listValue);
					array[i] = this.listValue.ToObject();
				}
				array2 = array;
			}
			finally
			{
				this.Offset = num;
			}
			return array2;
		}

		public XmlDictionaryString GetDictionaryString(int key)
		{
			IXmlDictionary xmlDictionary;
			if ((key & 1) != 0)
			{
				xmlDictionary = this.session;
			}
			else
			{
				xmlDictionary = this.dictionary;
			}
			XmlDictionaryString xmlDictionaryString;
			if (!xmlDictionary.TryLookup(key >> 1, out xmlDictionaryString))
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this.reader);
			}
			return xmlDictionaryString;
		}

		public int ReadDictionaryKey()
		{
			int num = this.ReadMultiByteUInt31();
			if ((num & 1) != 0)
			{
				if (this.session == null)
				{
					XmlExceptionHelper.ThrowInvalidBinaryFormat(this.reader);
				}
				int num2 = num >> 1;
				XmlDictionaryString xmlDictionaryString;
				if (!this.session.TryLookup(num2, out xmlDictionaryString))
				{
					if (num2 < 0 || num2 > 536870911)
					{
						XmlExceptionHelper.ThrowXmlDictionaryStringIDOutOfRange(this.reader);
					}
					XmlExceptionHelper.ThrowXmlDictionaryStringIDUndefinedSession(this.reader, num2);
				}
			}
			else
			{
				if (this.dictionary == null)
				{
					XmlExceptionHelper.ThrowInvalidBinaryFormat(this.reader);
				}
				int num3 = num >> 1;
				XmlDictionaryString xmlDictionaryString2;
				if (!this.dictionary.TryLookup(num3, out xmlDictionaryString2))
				{
					if (num3 < 0 || num3 > 536870911)
					{
						XmlExceptionHelper.ThrowXmlDictionaryStringIDOutOfRange(this.reader);
					}
					XmlExceptionHelper.ThrowXmlDictionaryStringIDUndefinedStatic(this.reader, num3);
				}
			}
			return num;
		}

		public void ReadValue(XmlBinaryNodeType nodeType, ValueHandle value)
		{
			switch (nodeType)
			{
			case XmlBinaryNodeType.MinText:
				value.SetValue(ValueHandleType.Zero);
				return;
			case XmlBinaryNodeType.ZeroTextWithEndElement:
			case XmlBinaryNodeType.OneTextWithEndElement:
			case XmlBinaryNodeType.FalseTextWithEndElement:
			case XmlBinaryNodeType.TrueTextWithEndElement:
			case XmlBinaryNodeType.Int8TextWithEndElement:
			case XmlBinaryNodeType.Int16TextWithEndElement:
			case XmlBinaryNodeType.Int32TextWithEndElement:
			case XmlBinaryNodeType.Int64TextWithEndElement:
			case XmlBinaryNodeType.FloatTextWithEndElement:
			case XmlBinaryNodeType.DoubleTextWithEndElement:
			case XmlBinaryNodeType.DecimalTextWithEndElement:
			case XmlBinaryNodeType.DateTimeTextWithEndElement:
			case XmlBinaryNodeType.Chars8TextWithEndElement:
			case XmlBinaryNodeType.Chars16TextWithEndElement:
			case XmlBinaryNodeType.Chars32TextWithEndElement:
			case XmlBinaryNodeType.Bytes8TextWithEndElement:
			case XmlBinaryNodeType.Bytes16TextWithEndElement:
			case XmlBinaryNodeType.Bytes32TextWithEndElement:
				break;
			case XmlBinaryNodeType.OneText:
				value.SetValue(ValueHandleType.One);
				return;
			case XmlBinaryNodeType.FalseText:
				value.SetValue(ValueHandleType.False);
				return;
			case XmlBinaryNodeType.TrueText:
				value.SetValue(ValueHandleType.True);
				return;
			case XmlBinaryNodeType.Int8Text:
				this.ReadValue(value, ValueHandleType.Int8, 1);
				return;
			case XmlBinaryNodeType.Int16Text:
				this.ReadValue(value, ValueHandleType.Int16, 2);
				return;
			case XmlBinaryNodeType.Int32Text:
				this.ReadValue(value, ValueHandleType.Int32, 4);
				return;
			case XmlBinaryNodeType.Int64Text:
				this.ReadValue(value, ValueHandleType.Int64, 8);
				return;
			case XmlBinaryNodeType.FloatText:
				this.ReadValue(value, ValueHandleType.Single, 4);
				return;
			case XmlBinaryNodeType.DoubleText:
				this.ReadValue(value, ValueHandleType.Double, 8);
				return;
			case XmlBinaryNodeType.DecimalText:
				this.ReadValue(value, ValueHandleType.Decimal, 16);
				return;
			case XmlBinaryNodeType.DateTimeText:
				this.ReadValue(value, ValueHandleType.DateTime, 8);
				return;
			case XmlBinaryNodeType.Chars8Text:
				this.ReadValue(value, ValueHandleType.UTF8, this.ReadUInt8());
				return;
			case XmlBinaryNodeType.Chars16Text:
				this.ReadValue(value, ValueHandleType.UTF8, this.ReadUInt16());
				return;
			case XmlBinaryNodeType.Chars32Text:
				this.ReadValue(value, ValueHandleType.UTF8, this.ReadUInt31());
				return;
			case XmlBinaryNodeType.Bytes8Text:
				this.ReadValue(value, ValueHandleType.Base64, this.ReadUInt8());
				return;
			case XmlBinaryNodeType.Bytes16Text:
				this.ReadValue(value, ValueHandleType.Base64, this.ReadUInt16());
				return;
			case XmlBinaryNodeType.Bytes32Text:
				this.ReadValue(value, ValueHandleType.Base64, this.ReadUInt31());
				return;
			case XmlBinaryNodeType.StartListText:
				this.ReadList(value);
				return;
			default:
				switch (nodeType)
				{
				case XmlBinaryNodeType.EmptyText:
					value.SetValue(ValueHandleType.Empty);
					return;
				case XmlBinaryNodeType.DictionaryText:
					value.SetDictionaryValue(this.ReadDictionaryKey());
					return;
				case XmlBinaryNodeType.UniqueIdText:
					this.ReadValue(value, ValueHandleType.UniqueId, 16);
					return;
				case XmlBinaryNodeType.TimeSpanText:
					this.ReadValue(value, ValueHandleType.TimeSpan, 8);
					return;
				case XmlBinaryNodeType.GuidText:
					this.ReadValue(value, ValueHandleType.Guid, 16);
					return;
				case XmlBinaryNodeType.UInt64Text:
					this.ReadValue(value, ValueHandleType.UInt64, 8);
					return;
				case XmlBinaryNodeType.BoolText:
					value.SetValue((this.ReadUInt8() != 0) ? ValueHandleType.True : ValueHandleType.False);
					return;
				case XmlBinaryNodeType.UnicodeChars8Text:
					this.ReadUnicodeValue(value, this.ReadUInt8());
					return;
				case XmlBinaryNodeType.UnicodeChars16Text:
					this.ReadUnicodeValue(value, this.ReadUInt16());
					return;
				case XmlBinaryNodeType.UnicodeChars32Text:
					this.ReadUnicodeValue(value, this.ReadUInt31());
					return;
				case XmlBinaryNodeType.QNameDictionaryText:
					this.ReadQName(value);
					return;
				}
				break;
			}
			XmlExceptionHelper.ThrowInvalidBinaryFormat(this.reader);
		}

		private void ReadValue(ValueHandle value, ValueHandleType type, int length)
		{
			int num = this.ReadBytes(length);
			value.SetValue(type, num, length);
		}

		private void ReadUnicodeValue(ValueHandle value, int length)
		{
			if ((length & 1) != 0)
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this.reader);
			}
			this.ReadValue(value, ValueHandleType.Unicode, length);
		}

		private void ReadList(ValueHandle value)
		{
			if (this.listValue == null)
			{
				this.listValue = new ValueHandle(this);
			}
			int num = 0;
			int num2 = this.Offset;
			for (;;)
			{
				XmlBinaryNodeType nodeType = this.GetNodeType();
				this.SkipNodeType();
				if (nodeType == XmlBinaryNodeType.StartListText)
				{
					XmlExceptionHelper.ThrowInvalidBinaryFormat(this.reader);
				}
				if (nodeType == XmlBinaryNodeType.EndListText)
				{
					break;
				}
				this.ReadValue(nodeType, this.listValue);
				num++;
			}
			value.SetValue(ValueHandleType.List, num2, num);
		}

		public void ReadQName(ValueHandle value)
		{
			int num = this.ReadUInt8();
			if (num >= 26)
			{
				XmlExceptionHelper.ThrowInvalidBinaryFormat(this.reader);
			}
			int num2 = this.ReadDictionaryKey();
			value.SetQNameValue(num, num2);
		}

		public int[] GetRows()
		{
			if (this.buffer == null)
			{
				return new int[1];
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(this.offsetMin);
			for (int i = this.offsetMin; i < this.offsetMax; i++)
			{
				if (this.buffer[i] == 13 || this.buffer[i] == 10)
				{
					if (i + 1 < this.offsetMax && this.buffer[i + 1] == 10)
					{
						i++;
					}
					arrayList.Add(i + 1);
				}
			}
			return (int[])arrayList.ToArray(typeof(int));
		}

		private XmlDictionaryReader reader;

		private Stream stream;

		private byte[] streamBuffer;

		private byte[] buffer;

		private int offsetMin;

		private int offsetMax;

		private IXmlDictionary dictionary;

		private XmlBinaryReaderSession session;

		private byte[] guid;

		private int offset;

		private const int maxBytesPerChar = 3;

		private char[] chars;

		private int windowOffset;

		private int windowOffsetMax;

		private ValueHandle listValue;

		private static byte[] emptyByteArray = new byte[0];

		private static XmlBufferReader empty = new XmlBufferReader(XmlBufferReader.emptyByteArray);
	}
}
