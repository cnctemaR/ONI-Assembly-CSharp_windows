using System;
using System.IO;
using System.Text;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class JsonEncodingStreamWrapper : Stream
	{
		public JsonEncodingStreamWrapper(Stream stream, Encoding encoding, bool isReader)
		{
			this.isReading = isReader;
			if (isReader)
			{
				this.InitForReading(stream, encoding);
				return;
			}
			if (encoding == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encoding");
			}
			this.InitForWriting(stream, encoding);
		}

		public override bool CanRead
		{
			get
			{
				return this.isReading && this.stream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return this.stream.CanTimeout;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return !this.isReading && this.stream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return this.stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
			}
			set
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return this.stream.ReadTimeout;
			}
			set
			{
				this.stream.ReadTimeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return this.stream.WriteTimeout;
			}
			set
			{
				this.stream.WriteTimeout = value;
			}
		}

		public static ArraySegment<byte> ProcessBuffer(byte[] buffer, int offset, int count, Encoding encoding)
		{
			ArraySegment<byte> arraySegment;
			try
			{
				JsonEncodingStreamWrapper.SupportedEncoding supportedEncoding = JsonEncodingStreamWrapper.GetSupportedEncoding(encoding);
				JsonEncodingStreamWrapper.SupportedEncoding supportedEncoding2;
				if (count < 2)
				{
					supportedEncoding2 = JsonEncodingStreamWrapper.SupportedEncoding.UTF8;
				}
				else
				{
					supportedEncoding2 = JsonEncodingStreamWrapper.ReadEncoding(buffer[offset], buffer[offset + 1]);
				}
				if (supportedEncoding != JsonEncodingStreamWrapper.SupportedEncoding.None && supportedEncoding != supportedEncoding2)
				{
					JsonEncodingStreamWrapper.ThrowExpectedEncodingMismatch(supportedEncoding, supportedEncoding2);
				}
				if (supportedEncoding2 == JsonEncodingStreamWrapper.SupportedEncoding.UTF8)
				{
					arraySegment = new ArraySegment<byte>(buffer, offset, count);
				}
				else
				{
					arraySegment = new ArraySegment<byte>(JsonEncodingStreamWrapper.ValidatingUTF8.GetBytes(JsonEncodingStreamWrapper.GetEncoding(supportedEncoding2).GetChars(buffer, offset, count)));
				}
			}
			catch (DecoderFallbackException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid bytes in JSON."), ex));
			}
			return arraySegment;
		}

		public override void Close()
		{
			this.Flush();
			base.Close();
			this.stream.Close();
		}

		public override void Flush()
		{
			this.stream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num2;
			try
			{
				if (this.byteCount == 0)
				{
					if (this.encodingCode == JsonEncodingStreamWrapper.SupportedEncoding.UTF8)
					{
						return this.stream.Read(buffer, offset, count);
					}
					this.byteOffset = 0;
					this.byteCount = this.stream.Read(this.bytes, this.byteCount, (this.chars.Length - 1) * 2);
					if (this.byteCount == 0)
					{
						return 0;
					}
					this.CleanupCharBreak();
					int num = this.encoding.GetChars(this.bytes, 0, this.byteCount, this.chars, 0);
					this.byteCount = Encoding.UTF8.GetBytes(this.chars, 0, num, this.bytes, 0);
				}
				if (this.byteCount < count)
				{
					count = this.byteCount;
				}
				Buffer.BlockCopy(this.bytes, this.byteOffset, buffer, offset, count);
				this.byteOffset += count;
				this.byteCount -= count;
				num2 = count;
			}
			catch (DecoderFallbackException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid bytes in JSON."), ex));
			}
			return num2;
		}

		public override int ReadByte()
		{
			if (this.byteCount == 0 && this.encodingCode == JsonEncodingStreamWrapper.SupportedEncoding.UTF8)
			{
				return this.stream.ReadByte();
			}
			if (this.Read(this.byteBuffer, 0, 1) == 0)
			{
				return -1;
			}
			return (int)this.byteBuffer[0];
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
		}

		public override void SetLength(long value)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.encodingCode == JsonEncodingStreamWrapper.SupportedEncoding.UTF8)
			{
				this.stream.Write(buffer, offset, count);
				return;
			}
			while (count > 0)
			{
				int num = ((this.chars.Length < count) ? this.chars.Length : count);
				int num2 = this.dec.GetChars(buffer, offset, num, this.chars, 0, false);
				this.byteCount = this.enc.GetBytes(this.chars, 0, num2, this.bytes, 0, false);
				this.stream.Write(this.bytes, 0, this.byteCount);
				offset += num;
				count -= num;
			}
		}

		public override void WriteByte(byte b)
		{
			if (this.encodingCode == JsonEncodingStreamWrapper.SupportedEncoding.UTF8)
			{
				this.stream.WriteByte(b);
				return;
			}
			this.byteBuffer[0] = b;
			this.Write(this.byteBuffer, 0, 1);
		}

		private static Encoding GetEncoding(JsonEncodingStreamWrapper.SupportedEncoding e)
		{
			switch (e)
			{
			case JsonEncodingStreamWrapper.SupportedEncoding.UTF8:
				return JsonEncodingStreamWrapper.ValidatingUTF8;
			case JsonEncodingStreamWrapper.SupportedEncoding.UTF16LE:
				return JsonEncodingStreamWrapper.ValidatingUTF16;
			case JsonEncodingStreamWrapper.SupportedEncoding.UTF16BE:
				return JsonEncodingStreamWrapper.ValidatingBEUTF16;
			default:
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("JSON Encoding is not supported.")));
			}
		}

		private static string GetEncodingName(JsonEncodingStreamWrapper.SupportedEncoding enc)
		{
			switch (enc)
			{
			case JsonEncodingStreamWrapper.SupportedEncoding.UTF8:
				return "utf-8";
			case JsonEncodingStreamWrapper.SupportedEncoding.UTF16LE:
				return "utf-16LE";
			case JsonEncodingStreamWrapper.SupportedEncoding.UTF16BE:
				return "utf-16BE";
			default:
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("JSON Encoding is not supported.")));
			}
		}

		private static JsonEncodingStreamWrapper.SupportedEncoding GetSupportedEncoding(Encoding encoding)
		{
			if (encoding == null)
			{
				return JsonEncodingStreamWrapper.SupportedEncoding.None;
			}
			if (encoding.WebName == JsonEncodingStreamWrapper.ValidatingUTF8.WebName)
			{
				return JsonEncodingStreamWrapper.SupportedEncoding.UTF8;
			}
			if (encoding.WebName == JsonEncodingStreamWrapper.ValidatingUTF16.WebName)
			{
				return JsonEncodingStreamWrapper.SupportedEncoding.UTF16LE;
			}
			if (encoding.WebName == JsonEncodingStreamWrapper.ValidatingBEUTF16.WebName)
			{
				return JsonEncodingStreamWrapper.SupportedEncoding.UTF16BE;
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("JSON Encoding is not supported.")));
		}

		private static JsonEncodingStreamWrapper.SupportedEncoding ReadEncoding(byte b1, byte b2)
		{
			if (b1 == 0 && b2 != 0)
			{
				return JsonEncodingStreamWrapper.SupportedEncoding.UTF16BE;
			}
			if (b1 != 0 && b2 == 0)
			{
				return JsonEncodingStreamWrapper.SupportedEncoding.UTF16LE;
			}
			if (b1 == 0 && b2 == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid bytes in JSON.")));
			}
			return JsonEncodingStreamWrapper.SupportedEncoding.UTF8;
		}

		private static void ThrowExpectedEncodingMismatch(JsonEncodingStreamWrapper.SupportedEncoding expEnc, JsonEncodingStreamWrapper.SupportedEncoding actualEnc)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Expected encoding '{0}', got '{1}' instead.", new object[]
			{
				JsonEncodingStreamWrapper.GetEncodingName(expEnc),
				JsonEncodingStreamWrapper.GetEncodingName(actualEnc)
			})));
		}

		private void CleanupCharBreak()
		{
			int num = this.byteOffset + this.byteCount;
			if (this.byteCount % 2 != 0)
			{
				int num2 = this.stream.ReadByte();
				if (num2 < 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Unexpected end of file in JSON.")));
				}
				this.bytes[num++] = (byte)num2;
				this.byteCount++;
			}
			int num3;
			if (this.encodingCode == JsonEncodingStreamWrapper.SupportedEncoding.UTF16LE)
			{
				num3 = (int)this.bytes[num - 2] + ((int)this.bytes[num - 1] << 8);
			}
			else
			{
				num3 = (int)this.bytes[num - 1] + ((int)this.bytes[num - 2] << 8);
			}
			if ((num3 & 56320) != 56320 && num3 >= 55296 && num3 <= 56319)
			{
				int num4 = this.stream.ReadByte();
				int num5 = this.stream.ReadByte();
				if (num5 < 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Unexpected end of file in JSON.")));
				}
				this.bytes[num++] = (byte)num4;
				this.bytes[num++] = (byte)num5;
				this.byteCount += 2;
			}
		}

		private void EnsureBuffers()
		{
			this.EnsureByteBuffer();
			if (this.chars == null)
			{
				this.chars = new char[128];
			}
		}

		private void EnsureByteBuffer()
		{
			if (this.bytes != null)
			{
				return;
			}
			this.bytes = new byte[512];
			this.byteOffset = 0;
			this.byteCount = 0;
		}

		private void FillBuffer(int count)
		{
			int num;
			for (count -= this.byteCount; count > 0; count -= num)
			{
				num = this.stream.Read(this.bytes, this.byteOffset + this.byteCount, count);
				if (num == 0)
				{
					break;
				}
				this.byteCount += num;
			}
		}

		private void InitForReading(Stream inputStream, Encoding expectedEncoding)
		{
			try
			{
				this.stream = new BufferedStream(inputStream);
				JsonEncodingStreamWrapper.SupportedEncoding supportedEncoding = JsonEncodingStreamWrapper.GetSupportedEncoding(expectedEncoding);
				JsonEncodingStreamWrapper.SupportedEncoding supportedEncoding2 = this.ReadEncoding();
				if (supportedEncoding != JsonEncodingStreamWrapper.SupportedEncoding.None && supportedEncoding != supportedEncoding2)
				{
					JsonEncodingStreamWrapper.ThrowExpectedEncodingMismatch(supportedEncoding, supportedEncoding2);
				}
				if (supportedEncoding2 != JsonEncodingStreamWrapper.SupportedEncoding.UTF8)
				{
					this.EnsureBuffers();
					this.FillBuffer(254);
					this.encodingCode = supportedEncoding2;
					this.encoding = JsonEncodingStreamWrapper.GetEncoding(supportedEncoding2);
					this.CleanupCharBreak();
					int num = this.encoding.GetChars(this.bytes, this.byteOffset, this.byteCount, this.chars, 0);
					this.byteOffset = 0;
					this.byteCount = JsonEncodingStreamWrapper.ValidatingUTF8.GetBytes(this.chars, 0, num, this.bytes, 0);
				}
			}
			catch (DecoderFallbackException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid bytes in JSON."), ex));
			}
		}

		private void InitForWriting(Stream outputStream, Encoding writeEncoding)
		{
			this.encoding = writeEncoding;
			this.stream = new BufferedStream(outputStream);
			this.encodingCode = JsonEncodingStreamWrapper.GetSupportedEncoding(writeEncoding);
			if (this.encodingCode != JsonEncodingStreamWrapper.SupportedEncoding.UTF8)
			{
				this.EnsureBuffers();
				this.dec = JsonEncodingStreamWrapper.ValidatingUTF8.GetDecoder();
				this.enc = this.encoding.GetEncoder();
			}
		}

		private JsonEncodingStreamWrapper.SupportedEncoding ReadEncoding()
		{
			int num = this.stream.ReadByte();
			int num2 = this.stream.ReadByte();
			this.EnsureByteBuffer();
			JsonEncodingStreamWrapper.SupportedEncoding supportedEncoding;
			if (num == -1)
			{
				supportedEncoding = JsonEncodingStreamWrapper.SupportedEncoding.UTF8;
				this.byteCount = 0;
			}
			else if (num2 == -1)
			{
				supportedEncoding = JsonEncodingStreamWrapper.SupportedEncoding.UTF8;
				this.bytes[0] = (byte)num;
				this.byteCount = 1;
			}
			else
			{
				supportedEncoding = JsonEncodingStreamWrapper.ReadEncoding((byte)num, (byte)num2);
				this.bytes[0] = (byte)num;
				this.bytes[1] = (byte)num2;
				this.byteCount = 2;
			}
			return supportedEncoding;
		}

		private static readonly UnicodeEncoding SafeBEUTF16 = new UnicodeEncoding(true, false, false);

		private static readonly UnicodeEncoding SafeUTF16 = new UnicodeEncoding(false, false, false);

		private static readonly UTF8Encoding SafeUTF8 = new UTF8Encoding(false, false);

		private static readonly UnicodeEncoding ValidatingBEUTF16 = new UnicodeEncoding(true, false, true);

		private static readonly UnicodeEncoding ValidatingUTF16 = new UnicodeEncoding(false, false, true);

		private static readonly UTF8Encoding ValidatingUTF8 = new UTF8Encoding(false, true);

		private const int BufferLength = 128;

		private byte[] byteBuffer = new byte[1];

		private int byteCount;

		private int byteOffset;

		private byte[] bytes;

		private char[] chars;

		private Decoder dec;

		private Encoder enc;

		private Encoding encoding;

		private JsonEncodingStreamWrapper.SupportedEncoding encodingCode;

		private bool isReading;

		private Stream stream;

		private enum SupportedEncoding
		{
			UTF8,
			UTF16LE,
			UTF16BE,
			None
		}
	}
}
