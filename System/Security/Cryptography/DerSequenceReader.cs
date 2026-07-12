using System;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Threading;

namespace System.Security.Cryptography
{
	internal class DerSequenceReader
	{
		internal int ContentLength { get; private set; }

		private DerSequenceReader(bool startAtPayload, byte[] data, int offset, int length)
		{
			this._data = data;
			this._position = offset;
			this._end = offset + length;
			this.ContentLength = length;
		}

		internal DerSequenceReader(byte[] data)
			: this(data, 0, data.Length)
		{
		}

		internal DerSequenceReader(byte[] data, int offset, int length)
			: this(DerSequenceReader.DerTag.Sequence, data, offset, length)
		{
		}

		private DerSequenceReader(DerSequenceReader.DerTag tagToEat, byte[] data, int offset, int length)
		{
			if (offset < 0 || length < 2 || length > data.Length - offset)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			this._data = data;
			this._end = offset + length;
			this._position = offset;
			this.EatTag(tagToEat);
			int num = this.EatLength();
			this.ContentLength = num;
			this._end = this._position + num;
		}

		internal static DerSequenceReader CreateForPayload(byte[] payload)
		{
			return new DerSequenceReader(true, payload, 0, payload.Length);
		}

		internal bool HasData
		{
			get
			{
				return this._position < this._end;
			}
		}

		internal byte PeekTag()
		{
			if (!this.HasData)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			byte b = this._data[this._position];
			if ((b & 31) == 31)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			return b;
		}

		internal bool HasTag(DerSequenceReader.DerTag expectedTag)
		{
			return this.HasTag((byte)expectedTag);
		}

		internal bool HasTag(byte expectedTag)
		{
			return this.HasData && this._data[this._position] == expectedTag;
		}

		internal void SkipValue()
		{
			this.EatTag((DerSequenceReader.DerTag)this.PeekTag());
			int num = this.EatLength();
			this._position += num;
		}

		internal void ValidateAndSkipDerValue()
		{
			byte b = this.PeekTag();
			if ((b & 192) == 0)
			{
				if (b == 0 || b == 15)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				bool flag = false;
				int num = (int)(b & 31);
				if (num <= 11)
				{
					if (num != 8 && num != 11)
					{
						goto IL_004E;
					}
				}
				else if (num - 16 > 1 && num != 29)
				{
					goto IL_004E;
				}
				flag = true;
				IL_004E:
				bool flag2 = (b & 32) == 32;
				if (flag != flag2)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
			}
			this.EatTag((DerSequenceReader.DerTag)b);
			int num2 = this.EatLength();
			if (num2 > 0 && (b & 32) == 32)
			{
				DerSequenceReader derSequenceReader = new DerSequenceReader(true, this._data, this._position, this._end - this._position);
				while (derSequenceReader.HasData)
				{
					derSequenceReader.ValidateAndSkipDerValue();
				}
			}
			this._position += num2;
		}

		internal byte[] ReadNextEncodedValue()
		{
			this.PeekTag();
			int num2;
			int num = DerSequenceReader.ScanContentLength(this._data, this._position + 1, this._end, out num2);
			int num3 = 1 + num2 + num;
			byte[] array = new byte[num3];
			Buffer.BlockCopy(this._data, this._position, array, 0, num3);
			this._position += num3;
			return array;
		}

		internal bool ReadBoolean()
		{
			this.EatTag(DerSequenceReader.DerTag.Boolean);
			int num = this.EatLength();
			if (num != 1)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			bool flag = this._data[this._position] > 0;
			this._position += num;
			return flag;
		}

		internal int ReadInteger()
		{
			byte[] array = this.ReadIntegerBytes();
			Array.Reverse<byte>(array);
			return (int)new BigInteger(array);
		}

		internal byte[] ReadIntegerBytes()
		{
			this.EatTag(DerSequenceReader.DerTag.Integer);
			return this.ReadContentAsBytes();
		}

		internal byte[] ReadBitString()
		{
			this.EatTag(DerSequenceReader.DerTag.BitString);
			int num = this.EatLength();
			if (num < 1)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			if (this._data[this._position] > 7)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			num--;
			this._position++;
			byte[] array = new byte[num];
			Buffer.BlockCopy(this._data, this._position, array, 0, num);
			this._position += num;
			return array;
		}

		internal byte[] ReadOctetString()
		{
			this.EatTag(DerSequenceReader.DerTag.OctetString);
			return this.ReadContentAsBytes();
		}

		internal string ReadOidAsString()
		{
			this.EatTag(DerSequenceReader.DerTag.ObjectIdentifier);
			int num = this.EatLength();
			if (num < 1)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			StringBuilder stringBuilder = new StringBuilder(num * 4);
			byte b = this._data[this._position];
			byte b2 = b / 40;
			byte b3 = b % 40;
			stringBuilder.Append(b2);
			stringBuilder.Append('.');
			stringBuilder.Append(b3);
			bool flag = true;
			BigInteger bigInteger = new BigInteger(0);
			for (int i = 1; i < num; i++)
			{
				byte b4 = this._data[this._position + i];
				byte b5 = b4 & 127;
				if (flag)
				{
					stringBuilder.Append('.');
					flag = false;
				}
				bigInteger <<= 7;
				bigInteger += b5;
				if (b4 == b5)
				{
					stringBuilder.Append(bigInteger);
					bigInteger = 0;
					flag = true;
				}
			}
			this._position += num;
			return stringBuilder.ToString();
		}

		internal Oid ReadOid()
		{
			return new Oid(this.ReadOidAsString());
		}

		internal string ReadUtf8String()
		{
			this.EatTag(DerSequenceReader.DerTag.UTF8String);
			int num = this.EatLength();
			string @string = Encoding.UTF8.GetString(this._data, this._position, num);
			this._position += num;
			return DerSequenceReader.TrimTrailingNulls(@string);
		}

		private DerSequenceReader ReadCollectionWithTag(DerSequenceReader.DerTag expected)
		{
			DerSequenceReader.CheckTag(expected, this._data, this._position);
			int num2;
			int num = DerSequenceReader.ScanContentLength(this._data, this._position + 1, this._end, out num2);
			int num3 = 1 + num2 + num;
			DerSequenceReader derSequenceReader = new DerSequenceReader(expected, this._data, this._position, num3);
			this._position += num3;
			return derSequenceReader;
		}

		internal DerSequenceReader ReadSequence()
		{
			return this.ReadCollectionWithTag(DerSequenceReader.DerTag.Sequence);
		}

		internal DerSequenceReader ReadSet()
		{
			return this.ReadCollectionWithTag(DerSequenceReader.DerTag.Set);
		}

		internal string ReadPrintableString()
		{
			this.EatTag(DerSequenceReader.DerTag.PrintableString);
			int num = this.EatLength();
			string @string = Encoding.ASCII.GetString(this._data, this._position, num);
			this._position += num;
			return DerSequenceReader.TrimTrailingNulls(@string);
		}

		internal string ReadIA5String()
		{
			this.EatTag(DerSequenceReader.DerTag.IA5String);
			int num = this.EatLength();
			string @string = Encoding.ASCII.GetString(this._data, this._position, num);
			this._position += num;
			return DerSequenceReader.TrimTrailingNulls(@string);
		}

		internal string ReadT61String()
		{
			this.EatTag(DerSequenceReader.DerTag.T61String);
			int num = this.EatLength();
			Encoding encoding = LazyInitializer.EnsureInitialized<Encoding>(ref DerSequenceReader.s_utf8EncodingWithExceptionFallback, () => new UTF8Encoding(false, true));
			Encoding encoding2 = LazyInitializer.EnsureInitialized<Encoding>(ref DerSequenceReader.s_latin1Encoding, () => Encoding.GetEncoding("iso-8859-1"));
			string text;
			try
			{
				text = encoding.GetString(this._data, this._position, num);
			}
			catch (DecoderFallbackException)
			{
				text = encoding2.GetString(this._data, this._position, num);
			}
			this._position += num;
			return DerSequenceReader.TrimTrailingNulls(text);
		}

		internal DateTime ReadX509Date()
		{
			DerSequenceReader.DerTag derTag = (DerSequenceReader.DerTag)this.PeekTag();
			if (derTag == DerSequenceReader.DerTag.UTCTime)
			{
				return this.ReadUtcTime();
			}
			if (derTag != DerSequenceReader.DerTag.GeneralizedTime)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			return this.ReadGeneralizedTime();
		}

		internal DateTime ReadUtcTime()
		{
			return this.ReadTime(DerSequenceReader.DerTag.UTCTime, "yyMMddHHmmss'Z'");
		}

		internal DateTime ReadGeneralizedTime()
		{
			return this.ReadTime(DerSequenceReader.DerTag.GeneralizedTime, "yyyyMMddHHmmss'Z'");
		}

		internal string ReadBMPString()
		{
			this.EatTag(DerSequenceReader.DerTag.BMPString);
			int num = this.EatLength();
			string @string = Encoding.BigEndianUnicode.GetString(this._data, this._position, num);
			this._position += num;
			return DerSequenceReader.TrimTrailingNulls(@string);
		}

		private static string TrimTrailingNulls(string value)
		{
			if (value != null && value.Length > 0)
			{
				int num = value.Length;
				while (num > 0 && value[num - 1] == '\0')
				{
					num--;
				}
				if (num != value.Length)
				{
					return value.Substring(0, num);
				}
			}
			return value;
		}

		private DateTime ReadTime(DerSequenceReader.DerTag timeTag, string formatString)
		{
			this.EatTag(timeTag);
			int num = this.EatLength();
			string @string = Encoding.ASCII.GetString(this._data, this._position, num);
			this._position += num;
			DateTimeFormatInfo dateTimeFormatInfo = LazyInitializer.EnsureInitialized<DateTimeFormatInfo>(ref DerSequenceReader.s_validityDateTimeFormatInfo, delegate
			{
				DateTimeFormatInfo dateTimeFormatInfo2 = (DateTimeFormatInfo)CultureInfo.InvariantCulture.DateTimeFormat.Clone();
				dateTimeFormatInfo2.Calendar.TwoDigitYearMax = 2049;
				return dateTimeFormatInfo2;
			});
			DateTime dateTime;
			if (!DateTime.TryParseExact(@string, formatString, dateTimeFormatInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out dateTime))
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			return dateTime;
		}

		private byte[] ReadContentAsBytes()
		{
			int num = this.EatLength();
			byte[] array = new byte[num];
			Buffer.BlockCopy(this._data, this._position, array, 0, num);
			this._position += num;
			return array;
		}

		private void EatTag(DerSequenceReader.DerTag expected)
		{
			if (!this.HasData)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			DerSequenceReader.CheckTag(expected, this._data, this._position);
			this._position++;
		}

		private static void CheckTag(DerSequenceReader.DerTag expected, byte[] data, int position)
		{
			if (position >= data.Length)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			byte b = data[position];
			byte b2 = b & 31;
			if (b2 == 31)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			if ((b & 128) != 0)
			{
				return;
			}
			if ((byte)(expected & (DerSequenceReader.DerTag)31) != b2)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
		}

		private int EatLength()
		{
			int num2;
			int num = DerSequenceReader.ScanContentLength(this._data, this._position, this._end, out num2);
			this._position += num2;
			return num;
		}

		private static int ScanContentLength(byte[] data, int offset, int end, out int bytesConsumed)
		{
			if (offset >= end)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			byte b = data[offset];
			if (b < 128)
			{
				bytesConsumed = 1;
				if ((int)b > end - offset - bytesConsumed)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				return (int)b;
			}
			else
			{
				int num = (int)(b & 127);
				if (num > 4)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				bytesConsumed = 1 + num;
				if (bytesConsumed > end - offset)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				if (bytesConsumed == 1)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				int num2 = offset + bytesConsumed;
				int num3 = 0;
				for (int i = offset + 1; i < num2; i++)
				{
					num3 <<= 8;
					num3 |= (int)data[i];
				}
				if (num3 < 0)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				if (num3 > end - offset - bytesConsumed)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				return num3;
			}
		}

		internal const byte ContextSpecificTagFlag = 128;

		internal const byte ConstructedFlag = 32;

		internal const byte ContextSpecificConstructedTag0 = 160;

		internal const byte ContextSpecificConstructedTag1 = 161;

		internal const byte ContextSpecificConstructedTag2 = 162;

		internal const byte ContextSpecificConstructedTag3 = 163;

		internal const byte ConstructedSequence = 48;

		internal const byte TagClassMask = 192;

		internal const byte TagNumberMask = 31;

		internal static DateTimeFormatInfo s_validityDateTimeFormatInfo;

		private static Encoding s_utf8EncodingWithExceptionFallback;

		private static Encoding s_latin1Encoding;

		private readonly byte[] _data;

		private readonly int _end;

		private int _position;

		internal enum DerTag : byte
		{
			Boolean = 1,
			Integer,
			BitString,
			OctetString,
			Null,
			ObjectIdentifier,
			UTF8String = 12,
			Sequence = 16,
			Set,
			PrintableString = 19,
			T61String,
			IA5String = 22,
			UTCTime,
			GeneralizedTime,
			BMPString = 30
		}
	}
}
