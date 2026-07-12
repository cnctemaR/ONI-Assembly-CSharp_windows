using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Cryptography.Asn1
{
	internal sealed class AsnWriter : IDisposable
	{
		public AsnEncodingRules RuleSet { get; }

		public AsnWriter(AsnEncodingRules ruleSet)
		{
			if (ruleSet != AsnEncodingRules.BER && ruleSet != AsnEncodingRules.CER && ruleSet != AsnEncodingRules.DER)
			{
				throw new ArgumentOutOfRangeException("ruleSet");
			}
			this.RuleSet = ruleSet;
		}

		public void Dispose()
		{
			this._nestingStack = null;
			if (this._buffer != null)
			{
				Array.Clear(this._buffer, 0, this._offset);
				ArrayPool<byte>.Shared.Return(this._buffer, false);
				this._buffer = null;
				this._offset = 0;
			}
		}

		private void EnsureWriteCapacity(int pendingCount)
		{
			if (pendingCount < 0)
			{
				throw new OverflowException();
			}
			if (this._buffer == null || this._buffer.Length - this._offset < pendingCount)
			{
				int num = checked(this._offset + pendingCount + 1023) / 1024;
				byte[] array = ArrayPool<byte>.Shared.Rent(1024 * num);
				if (this._buffer != null)
				{
					Buffer.BlockCopy(this._buffer, 0, array, 0, this._offset);
					Array.Clear(this._buffer, 0, this._offset);
					ArrayPool<byte>.Shared.Return(this._buffer, false);
				}
				this._buffer = array;
			}
		}

		private void WriteTag(Asn1Tag tag)
		{
			int num = tag.CalculateEncodedSize();
			this.EnsureWriteCapacity(num);
			int num2;
			if (!tag.TryWrite(this._buffer.AsSpan<byte>(this._offset, num), out num2) || num2 != num)
			{
				throw new CryptographicException();
			}
			this._offset += num;
		}

		private void WriteLength(int length)
		{
			if (length == -1)
			{
				this.EnsureWriteCapacity(1);
				this._buffer[this._offset] = 128;
				this._offset++;
				return;
			}
			if (length < 128)
			{
				this.EnsureWriteCapacity(1 + length);
				this._buffer[this._offset] = (byte)length;
				this._offset++;
				return;
			}
			int encodedLengthSubsequentByteCount = AsnWriter.GetEncodedLengthSubsequentByteCount(length);
			this.EnsureWriteCapacity(encodedLengthSubsequentByteCount + 1 + length);
			this._buffer[this._offset] = (byte)(128 | encodedLengthSubsequentByteCount);
			int num = this._offset + encodedLengthSubsequentByteCount;
			int num2 = length;
			do
			{
				this._buffer[num] = (byte)num2;
				num2 >>= 8;
				num--;
			}
			while (num2 > 0);
			this._offset += encodedLengthSubsequentByteCount + 1;
		}

		private static int GetEncodedLengthSubsequentByteCount(int length)
		{
			if (length <= 127)
			{
				return 0;
			}
			if (length <= 255)
			{
				return 1;
			}
			if (length <= 65535)
			{
				return 2;
			}
			if (length <= 16777215)
			{
				return 3;
			}
			return 4;
		}

		public void WriteEncodedValue(ReadOnlyMemory<byte> preEncodedValue)
		{
			AsnReader asnReader = new AsnReader(preEncodedValue, this.RuleSet);
			asnReader.GetEncodedValue();
			if (asnReader.HasData)
			{
				throw new ArgumentException("The input to WriteEncodedValue must represent a single encoded value with no trailing data.", "preEncodedValue");
			}
			this.EnsureWriteCapacity(preEncodedValue.Length);
			preEncodedValue.Span.CopyTo(this._buffer.AsSpan<byte>(this._offset));
			this._offset += preEncodedValue.Length;
		}

		private void WriteEndOfContents()
		{
			this.EnsureWriteCapacity(2);
			byte[] buffer = this._buffer;
			int num = this._offset;
			this._offset = num + 1;
			buffer[num] = 0;
			byte[] buffer2 = this._buffer;
			num = this._offset;
			this._offset = num + 1;
			buffer2[num] = 0;
		}

		public void WriteBoolean(bool value)
		{
			this.WriteBooleanCore(Asn1Tag.Boolean, value);
		}

		public void WriteBoolean(Asn1Tag tag, bool value)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.Boolean);
			this.WriteBooleanCore(tag.AsPrimitive(), value);
		}

		private void WriteBooleanCore(Asn1Tag tag, bool value)
		{
			this.WriteTag(tag);
			this.WriteLength(1);
			this._buffer[this._offset] = (value ? byte.MaxValue : 0);
			this._offset++;
		}

		public void WriteInteger(long value)
		{
			this.WriteIntegerCore(Asn1Tag.Integer, value);
		}

		public void WriteInteger(ulong value)
		{
			this.WriteNonNegativeIntegerCore(Asn1Tag.Integer, value);
		}

		public void WriteInteger(BigInteger value)
		{
			this.WriteIntegerCore(Asn1Tag.Integer, value);
		}

		public void WriteInteger(ReadOnlySpan<byte> value)
		{
			this.WriteIntegerCore(Asn1Tag.Integer, value);
		}

		public void WriteInteger(Asn1Tag tag, long value)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.Integer);
			this.WriteIntegerCore(tag.AsPrimitive(), value);
		}

		private void WriteIntegerCore(Asn1Tag tag, long value)
		{
			if (value >= 0L)
			{
				this.WriteNonNegativeIntegerCore(tag, (ulong)value);
				return;
			}
			int num;
			if (value >= -128L)
			{
				num = 1;
			}
			else if (value >= -32768L)
			{
				num = 2;
			}
			else if (value >= -8388608L)
			{
				num = 3;
			}
			else if (value >= -2147483648L)
			{
				num = 4;
			}
			else if (value >= -549755813888L)
			{
				num = 5;
			}
			else if (value >= -140737488355328L)
			{
				num = 6;
			}
			else if (value >= -36028797018963968L)
			{
				num = 7;
			}
			else
			{
				num = 8;
			}
			this.WriteTag(tag);
			this.WriteLength(num);
			long num2 = value;
			int num3 = this._offset + num - 1;
			do
			{
				this._buffer[num3] = (byte)num2;
				num2 >>= 8;
				num3--;
			}
			while (num3 >= this._offset);
			this._offset += num;
		}

		public void WriteInteger(Asn1Tag tag, ulong value)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.Integer);
			this.WriteNonNegativeIntegerCore(tag.AsPrimitive(), value);
		}

		private void WriteNonNegativeIntegerCore(Asn1Tag tag, ulong value)
		{
			int num;
			if (value < 128UL)
			{
				num = 1;
			}
			else if (value < 32768UL)
			{
				num = 2;
			}
			else if (value < 8388608UL)
			{
				num = 3;
			}
			else if (value < (ulong)(-2147483648))
			{
				num = 4;
			}
			else if (value < 549755813888UL)
			{
				num = 5;
			}
			else if (value < 140737488355328UL)
			{
				num = 6;
			}
			else if (value < 36028797018963968UL)
			{
				num = 7;
			}
			else if (value < 9223372036854775808UL)
			{
				num = 8;
			}
			else
			{
				num = 9;
			}
			this.WriteTag(tag);
			this.WriteLength(num);
			ulong num2 = value;
			int num3 = this._offset + num - 1;
			do
			{
				this._buffer[num3] = (byte)num2;
				num2 >>= 8;
				num3--;
			}
			while (num3 >= this._offset);
			this._offset += num;
		}

		public void WriteInteger(Asn1Tag tag, BigInteger value)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.Integer);
			this.WriteIntegerCore(tag.AsPrimitive(), value);
		}

		public void WriteInteger(Asn1Tag tag, ReadOnlySpan<byte> value)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.Integer);
			this.WriteIntegerCore(tag.AsPrimitive(), value);
		}

		private unsafe void WriteIntegerCore(Asn1Tag tag, ReadOnlySpan<byte> value)
		{
			if (value.IsEmpty)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			if (value.Length > 1)
			{
				ushort num = (ushort)(((int)(*value[0]) << 8) | (int)(*value[1])) & 65408;
				if (num == 0 || num == 65408)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
			}
			this.WriteTag(tag);
			this.WriteLength(value.Length);
			value.CopyTo(this._buffer.AsSpan<byte>(this._offset));
			this._offset += value.Length;
		}

		private void WriteIntegerCore(Asn1Tag tag, BigInteger value)
		{
			byte[] array = value.ToByteArray();
			Array.Reverse<byte>(array);
			this.WriteTag(tag);
			this.WriteLength(array.Length);
			Buffer.BlockCopy(array, 0, this._buffer, this._offset, array.Length);
			this._offset += array.Length;
		}

		public void WriteBitString(ReadOnlySpan<byte> bitString, int unusedBitCount = 0)
		{
			this.WriteBitStringCore(Asn1Tag.PrimitiveBitString, bitString, unusedBitCount);
		}

		public void WriteBitString(Asn1Tag tag, ReadOnlySpan<byte> bitString, int unusedBitCount = 0)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.BitString);
			this.WriteBitStringCore(tag, bitString, unusedBitCount);
		}

		private unsafe void WriteBitStringCore(Asn1Tag tag, ReadOnlySpan<byte> bitString, int unusedBitCount)
		{
			if (unusedBitCount < 0 || unusedBitCount > 7)
			{
				throw new ArgumentOutOfRangeException("unusedBitCount", unusedBitCount, "Unused bit count must be between 0 and 7, inclusive.");
			}
			if (bitString.Length == 0 && unusedBitCount != 0)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			int num = (1 << unusedBitCount) - 1;
			if ((((!bitString.IsEmpty && *bitString[bitString.Length - 1] != 0) ? 1 : 0) & num) != 0)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			if (this.RuleSet == AsnEncodingRules.CER && bitString.Length >= 1000)
			{
				this.WriteConstructedCerBitString(tag, bitString, unusedBitCount);
				return;
			}
			this.WriteTag(tag.AsPrimitive());
			this.WriteLength(bitString.Length + 1);
			this._buffer[this._offset] = (byte)unusedBitCount;
			this._offset++;
			bitString.CopyTo(this._buffer.AsSpan<byte>(this._offset));
			this._offset += bitString.Length;
		}

		private void WriteConstructedCerBitString(Asn1Tag tag, ReadOnlySpan<byte> payload, int unusedBitCount)
		{
			this.WriteTag(tag.AsConstructed());
			this.WriteLength(-1);
			int num2;
			int num = Math.DivRem(payload.Length, 999, out num2);
			int num3;
			if (num2 == 0)
			{
				num3 = 0;
			}
			else
			{
				num3 = 3 + num2 + AsnWriter.GetEncodedLengthSubsequentByteCount(num2);
			}
			int num4 = num * 1004 + num3 + 2;
			this.EnsureWriteCapacity(num4);
			byte[] buffer = this._buffer;
			int offset = this._offset;
			ReadOnlySpan<byte> readOnlySpan = payload;
			Asn1Tag primitiveBitString = Asn1Tag.PrimitiveBitString;
			Span<byte> span;
			while (readOnlySpan.Length > 999)
			{
				this.WriteTag(primitiveBitString);
				this.WriteLength(1000);
				this._buffer[this._offset] = 0;
				this._offset++;
				span = this._buffer.AsSpan<byte>(this._offset);
				readOnlySpan.Slice(0, 999).CopyTo(span);
				readOnlySpan = readOnlySpan.Slice(999);
				this._offset += 999;
			}
			this.WriteTag(primitiveBitString);
			this.WriteLength(readOnlySpan.Length + 1);
			this._buffer[this._offset] = (byte)unusedBitCount;
			this._offset++;
			span = this._buffer.AsSpan<byte>(this._offset);
			readOnlySpan.CopyTo(span);
			this._offset += readOnlySpan.Length;
			this.WriteEndOfContents();
		}

		public void WriteNamedBitList(object enumValue)
		{
			if (enumValue == null)
			{
				throw new ArgumentNullException("enumValue");
			}
			this.WriteNamedBitList(Asn1Tag.PrimitiveBitString, enumValue);
		}

		public void WriteNamedBitList<TEnum>(TEnum enumValue) where TEnum : struct
		{
			this.WriteNamedBitList<TEnum>(Asn1Tag.PrimitiveBitString, enumValue);
		}

		public void WriteNamedBitList(Asn1Tag tag, object enumValue)
		{
			if (enumValue == null)
			{
				throw new ArgumentNullException("enumValue");
			}
			this.WriteNamedBitList(tag, enumValue.GetType(), enumValue);
		}

		public void WriteNamedBitList<TEnum>(Asn1Tag tag, TEnum enumValue) where TEnum : struct
		{
			this.WriteNamedBitList(tag, typeof(TEnum), enumValue);
		}

		private void WriteNamedBitList(Asn1Tag tag, Type tEnum, object enumValue)
		{
			Type enumUnderlyingType = tEnum.GetEnumUnderlyingType();
			if (!tEnum.IsDefined(typeof(FlagsAttribute), false))
			{
				throw new ArgumentException("Named bit list operations require an enum with the [Flags] attribute.", "tEnum");
			}
			ulong num;
			if (enumUnderlyingType == typeof(ulong))
			{
				num = Convert.ToUInt64(enumValue);
			}
			else
			{
				num = (ulong)Convert.ToInt64(enumValue);
			}
			this.WriteNamedBitList(tag, num);
		}

		private unsafe void WriteNamedBitList(Asn1Tag tag, ulong integralValue)
		{
			Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)8], 8);
			span.Clear();
			int num = -1;
			int num2 = 0;
			while (integralValue != 0UL)
			{
				if ((integralValue & 1UL) != 0UL)
				{
					ref byte ptr = ref span[num2 / 8];
					ptr |= (byte)(128 >> num2 % 8);
					num = num2;
				}
				integralValue >>= 1;
				num2++;
			}
			if (num < 0)
			{
				this.WriteBitString(tag, ReadOnlySpan<byte>.Empty, 0);
				return;
			}
			int num3 = num / 8 + 1;
			int num4 = 7 - num % 8;
			this.WriteBitString(tag, span.Slice(0, num3), num4);
		}

		public void WriteOctetString(ReadOnlySpan<byte> octetString)
		{
			this.WriteOctetString(Asn1Tag.PrimitiveOctetString, octetString);
		}

		public void WriteOctetString(Asn1Tag tag, ReadOnlySpan<byte> octetString)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.OctetString);
			this.WriteOctetStringCore(tag, octetString);
		}

		private void WriteOctetStringCore(Asn1Tag tag, ReadOnlySpan<byte> octetString)
		{
			if (this.RuleSet == AsnEncodingRules.CER && octetString.Length > 1000)
			{
				this.WriteConstructedCerOctetString(tag, octetString);
				return;
			}
			this.WriteTag(tag.AsPrimitive());
			this.WriteLength(octetString.Length);
			octetString.CopyTo(this._buffer.AsSpan<byte>(this._offset));
			this._offset += octetString.Length;
		}

		private void WriteConstructedCerOctetString(Asn1Tag tag, ReadOnlySpan<byte> payload)
		{
			this.WriteTag(tag.AsConstructed());
			this.WriteLength(-1);
			int num2;
			int num = Math.DivRem(payload.Length, 1000, out num2);
			int num3;
			if (num2 == 0)
			{
				num3 = 0;
			}
			else
			{
				num3 = 2 + num2 + AsnWriter.GetEncodedLengthSubsequentByteCount(num2);
			}
			int num4 = num * 1004 + num3 + 2;
			this.EnsureWriteCapacity(num4);
			byte[] buffer = this._buffer;
			int offset = this._offset;
			ReadOnlySpan<byte> readOnlySpan = payload;
			Asn1Tag primitiveOctetString = Asn1Tag.PrimitiveOctetString;
			Span<byte> span;
			while (readOnlySpan.Length > 1000)
			{
				this.WriteTag(primitiveOctetString);
				this.WriteLength(1000);
				span = this._buffer.AsSpan<byte>(this._offset);
				readOnlySpan.Slice(0, 1000).CopyTo(span);
				this._offset += 1000;
				readOnlySpan = readOnlySpan.Slice(1000);
			}
			this.WriteTag(primitiveOctetString);
			this.WriteLength(readOnlySpan.Length);
			span = this._buffer.AsSpan<byte>(this._offset);
			readOnlySpan.CopyTo(span);
			this._offset += readOnlySpan.Length;
			this.WriteEndOfContents();
		}

		public void WriteNull()
		{
			this.WriteNullCore(Asn1Tag.Null);
		}

		public void WriteNull(Asn1Tag tag)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.Null);
			this.WriteNullCore(tag.AsPrimitive());
		}

		private void WriteNullCore(Asn1Tag tag)
		{
			this.WriteTag(tag);
			this.WriteLength(0);
		}

		public void WriteObjectIdentifier(Oid oid)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid");
			}
			this.WriteObjectIdentifier(oid.Value);
		}

		public void WriteObjectIdentifier(string oidValue)
		{
			if (oidValue == null)
			{
				throw new ArgumentNullException("oidValue");
			}
			this.WriteObjectIdentifier(oidValue.AsSpan());
		}

		public void WriteObjectIdentifier(ReadOnlySpan<char> oidValue)
		{
			this.WriteObjectIdentifierCore(Asn1Tag.ObjectIdentifier, oidValue);
		}

		public void WriteObjectIdentifier(Asn1Tag tag, Oid oid)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid");
			}
			this.WriteObjectIdentifier(tag, oid.Value);
		}

		public void WriteObjectIdentifier(Asn1Tag tag, string oidValue)
		{
			if (oidValue == null)
			{
				throw new ArgumentNullException("oidValue");
			}
			this.WriteObjectIdentifier(tag, oidValue.AsSpan());
		}

		public void WriteObjectIdentifier(Asn1Tag tag, ReadOnlySpan<char> oidValue)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.ObjectIdentifier);
			this.WriteObjectIdentifierCore(tag.AsPrimitive(), oidValue);
		}

		private unsafe void WriteObjectIdentifierCore(Asn1Tag tag, ReadOnlySpan<char> oidValue)
		{
			if (oidValue.Length < 3)
			{
				throw new CryptographicException("The OID value was invalid.");
			}
			if (*oidValue[1] != 46)
			{
				throw new CryptographicException("The OID value was invalid.");
			}
			byte[] array = ArrayPool<byte>.Shared.Rent(oidValue.Length / 2);
			int num = 0;
			try
			{
				int num2;
				switch (*oidValue[0])
				{
				case 48:
					num2 = 0;
					break;
				case 49:
					num2 = 1;
					break;
				case 50:
					num2 = 2;
					break;
				default:
					throw new CryptographicException("The OID value was invalid.");
				}
				ReadOnlySpan<char> readOnlySpan = oidValue.Slice(2);
				BigInteger bigInteger = AsnWriter.ParseSubIdentifier(ref readOnlySpan);
				bigInteger += 40 * num2;
				int num3 = AsnWriter.EncodeSubIdentifier(array.AsSpan<byte>(num), ref bigInteger);
				num += num3;
				while (!readOnlySpan.IsEmpty)
				{
					bigInteger = AsnWriter.ParseSubIdentifier(ref readOnlySpan);
					num3 = AsnWriter.EncodeSubIdentifier(array.AsSpan<byte>(num), ref bigInteger);
					num += num3;
				}
				this.WriteTag(tag);
				this.WriteLength(num);
				Buffer.BlockCopy(array, 0, this._buffer, this._offset, num);
				this._offset += num;
			}
			finally
			{
				Array.Clear(array, 0, num);
				ArrayPool<byte>.Shared.Return(array, false);
			}
		}

		private unsafe static BigInteger ParseSubIdentifier(ref ReadOnlySpan<char> oidValue)
		{
			int num = oidValue.IndexOf('.');
			if (num == -1)
			{
				num = oidValue.Length;
			}
			else if (num == oidValue.Length - 1)
			{
				throw new CryptographicException("The OID value was invalid.");
			}
			BigInteger bigInteger = BigInteger.Zero;
			for (int i = 0; i < num; i++)
			{
				if (i > 0 && bigInteger == 0L)
				{
					throw new CryptographicException("The OID value was invalid.");
				}
				bigInteger *= 10;
				bigInteger += AsnWriter.AtoI((char)(*oidValue[i]));
			}
			oidValue = oidValue.Slice(Math.Min(oidValue.Length, num + 1));
			return bigInteger;
		}

		private static int AtoI(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return (int)(c - '0');
			}
			throw new CryptographicException("The OID value was invalid.");
		}

		private unsafe static int EncodeSubIdentifier(Span<byte> dest, ref BigInteger subIdentifier)
		{
			if (subIdentifier.IsZero)
			{
				*dest[0] = 0;
				return 1;
			}
			BigInteger bigInteger = subIdentifier;
			int num = 0;
			do
			{
				byte b = (byte)(bigInteger & 127);
				if (subIdentifier != bigInteger)
				{
					b |= 128;
				}
				bigInteger >>= 7;
				*dest[num] = b;
				num++;
			}
			while (bigInteger != BigInteger.Zero);
			AsnWriter.Reverse(dest.Slice(0, num));
			return num;
		}

		public void WriteEnumeratedValue(object enumValue)
		{
			if (enumValue == null)
			{
				throw new ArgumentNullException("enumValue");
			}
			this.WriteEnumeratedValue(Asn1Tag.Enumerated, enumValue);
		}

		public void WriteEnumeratedValue<TEnum>(TEnum value) where TEnum : struct
		{
			this.WriteEnumeratedValue<TEnum>(Asn1Tag.Enumerated, value);
		}

		public void WriteEnumeratedValue(Asn1Tag tag, object enumValue)
		{
			if (enumValue == null)
			{
				throw new ArgumentNullException("enumValue");
			}
			this.WriteEnumeratedValue(tag.AsPrimitive(), enumValue.GetType(), enumValue);
		}

		public void WriteEnumeratedValue<TEnum>(Asn1Tag tag, TEnum value) where TEnum : struct
		{
			this.WriteEnumeratedValue(tag.AsPrimitive(), typeof(TEnum), value);
		}

		private void WriteEnumeratedValue(Asn1Tag tag, Type tEnum, object enumValue)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.Enumerated);
			Type enumUnderlyingType = tEnum.GetEnumUnderlyingType();
			if (tEnum.IsDefined(typeof(FlagsAttribute), false))
			{
				throw new ArgumentException("ASN.1 Enumerated values only apply to enum types without the [Flags] attribute.", "tEnum");
			}
			if (enumUnderlyingType == typeof(ulong))
			{
				ulong num = Convert.ToUInt64(enumValue);
				this.WriteNonNegativeIntegerCore(tag, num);
				return;
			}
			long num2 = Convert.ToInt64(enumValue);
			this.WriteIntegerCore(tag, num2);
		}

		public void PushSequence()
		{
			this.PushSequenceCore(Asn1Tag.Sequence);
		}

		public void PushSequence(Asn1Tag tag)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.Sequence);
			this.PushSequenceCore(tag.AsConstructed());
		}

		private void PushSequenceCore(Asn1Tag tag)
		{
			this.PushTag(tag.AsConstructed());
		}

		public void PopSequence()
		{
			this.PopSequence(Asn1Tag.Sequence);
		}

		public void PopSequence(Asn1Tag tag)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.Sequence);
			this.PopSequenceCore(tag.AsConstructed());
		}

		private void PopSequenceCore(Asn1Tag tag)
		{
			this.PopTag(tag, false);
		}

		public void PushSetOf()
		{
			this.PushSetOf(Asn1Tag.SetOf);
		}

		public void PushSetOf(Asn1Tag tag)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.Set);
			this.PushSetOfCore(tag.AsConstructed());
		}

		private void PushSetOfCore(Asn1Tag tag)
		{
			this.PushTag(tag);
		}

		public void PopSetOf()
		{
			this.PopSetOfCore(Asn1Tag.SetOf);
		}

		public void PopSetOf(Asn1Tag tag)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.Set);
			this.PopSetOfCore(tag.AsConstructed());
		}

		private void PopSetOfCore(Asn1Tag tag)
		{
			bool flag = this.RuleSet == AsnEncodingRules.CER || this.RuleSet == AsnEncodingRules.DER;
			this.PopTag(tag, flag);
		}

		public void WriteUtcTime(DateTimeOffset value)
		{
			this.WriteUtcTimeCore(Asn1Tag.UtcTime, value);
		}

		public void WriteUtcTime(Asn1Tag tag, DateTimeOffset value)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.UtcTime);
			this.WriteUtcTimeCore(tag.AsPrimitive(), value);
		}

		public void WriteUtcTime(DateTimeOffset value, int minLegalYear)
		{
			if (minLegalYear <= value.Year && value.Year < minLegalYear + 100)
			{
				this.WriteUtcTime(value);
				return;
			}
			throw new ArgumentOutOfRangeException("value");
		}

		private void WriteUtcTimeCore(Asn1Tag tag, DateTimeOffset value)
		{
			this.WriteTag(tag);
			this.WriteLength(13);
			DateTimeOffset dateTimeOffset = value.ToUniversalTime();
			int year = dateTimeOffset.Year;
			int month = dateTimeOffset.Month;
			int day = dateTimeOffset.Day;
			int hour = dateTimeOffset.Hour;
			int minute = dateTimeOffset.Minute;
			int second = dateTimeOffset.Second;
			Span<byte> span = this._buffer.AsSpan<byte>(this._offset);
			StandardFormat standardFormat = new StandardFormat('D', 2);
			int num;
			if (!Utf8Formatter.TryFormat(year % 100, span.Slice(0, 2), out num, standardFormat) || !Utf8Formatter.TryFormat(month, span.Slice(2, 2), out num, standardFormat) || !Utf8Formatter.TryFormat(day, span.Slice(4, 2), out num, standardFormat) || !Utf8Formatter.TryFormat(hour, span.Slice(6, 2), out num, standardFormat) || !Utf8Formatter.TryFormat(minute, span.Slice(8, 2), out num, standardFormat) || !Utf8Formatter.TryFormat(second, span.Slice(10, 2), out num, standardFormat))
			{
				throw new CryptographicException();
			}
			this._buffer[this._offset + 12] = 90;
			this._offset += 13;
		}

		public void WriteGeneralizedTime(DateTimeOffset value, bool omitFractionalSeconds = false)
		{
			this.WriteGeneralizedTimeCore(Asn1Tag.GeneralizedTime, value, omitFractionalSeconds);
		}

		public void WriteGeneralizedTime(Asn1Tag tag, DateTimeOffset value, bool omitFractionalSeconds = false)
		{
			AsnWriter.CheckUniversalTag(tag, UniversalTagNumber.GeneralizedTime);
			this.WriteGeneralizedTimeCore(tag.AsPrimitive(), value, omitFractionalSeconds);
		}

		private unsafe void WriteGeneralizedTimeCore(Asn1Tag tag, DateTimeOffset value, bool omitFractionalSeconds)
		{
			DateTimeOffset dateTimeOffset = value.ToUniversalTime();
			if (dateTimeOffset.Year > 9999)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			Span<byte> span = default(Span<byte>);
			if (!omitFractionalSeconds)
			{
				long num = dateTimeOffset.Ticks % 10000000L;
				if (num != 0L)
				{
					span = new Span<byte>(stackalloc byte[(UIntPtr)9], 9);
					int num2;
					if (!Utf8Formatter.TryFormat(num / 10000000m, span, out num2, new StandardFormat('G', 255)))
					{
						throw new CryptographicException();
					}
					span = span.Slice(1, num2 - 1);
				}
			}
			int num3 = 15 + span.Length;
			this.WriteTag(tag);
			this.WriteLength(num3);
			int year = dateTimeOffset.Year;
			int month = dateTimeOffset.Month;
			int day = dateTimeOffset.Day;
			int hour = dateTimeOffset.Hour;
			int minute = dateTimeOffset.Minute;
			int second = dateTimeOffset.Second;
			Span<byte> span2 = this._buffer.AsSpan<byte>(this._offset);
			StandardFormat standardFormat = new StandardFormat('D', 4);
			StandardFormat standardFormat2 = new StandardFormat('D', 2);
			int num4;
			if (!Utf8Formatter.TryFormat(year, span2.Slice(0, 4), out num4, standardFormat) || !Utf8Formatter.TryFormat(month, span2.Slice(4, 2), out num4, standardFormat2) || !Utf8Formatter.TryFormat(day, span2.Slice(6, 2), out num4, standardFormat2) || !Utf8Formatter.TryFormat(hour, span2.Slice(8, 2), out num4, standardFormat2) || !Utf8Formatter.TryFormat(minute, span2.Slice(10, 2), out num4, standardFormat2) || !Utf8Formatter.TryFormat(second, span2.Slice(12, 2), out num4, standardFormat2))
			{
				throw new CryptographicException();
			}
			this._offset += 14;
			span.CopyTo(span2.Slice(14));
			this._offset += span.Length;
			this._buffer[this._offset] = 90;
			this._offset++;
		}

		public bool TryEncode(Span<byte> dest, out int bytesWritten)
		{
			Stack<ValueTuple<Asn1Tag, int>> nestingStack = this._nestingStack;
			if (nestingStack != null && nestingStack.Count != 0)
			{
				throw new InvalidOperationException("Encode cannot be called while a Sequence or SetOf is still open.");
			}
			if (dest.Length < this._offset)
			{
				bytesWritten = 0;
				return false;
			}
			if (this._offset == 0)
			{
				bytesWritten = 0;
				return true;
			}
			bytesWritten = this._offset;
			this._buffer.AsSpan<byte>(0, this._offset).CopyTo(dest);
			return true;
		}

		public byte[] Encode()
		{
			Stack<ValueTuple<Asn1Tag, int>> nestingStack = this._nestingStack;
			if (nestingStack != null && nestingStack.Count != 0)
			{
				throw new InvalidOperationException("Encode cannot be called while a Sequence or SetOf is still open.");
			}
			if (this._offset == 0)
			{
				return Array.Empty<byte>();
			}
			return this._buffer.AsSpan<byte>(0, this._offset).ToArray();
		}

		public ReadOnlySpan<byte> EncodeAsSpan()
		{
			Stack<ValueTuple<Asn1Tag, int>> nestingStack = this._nestingStack;
			if (nestingStack != null && nestingStack.Count != 0)
			{
				throw new InvalidOperationException("Encode cannot be called while a Sequence or SetOf is still open.");
			}
			if (this._offset == 0)
			{
				return ReadOnlySpan<byte>.Empty;
			}
			return new ReadOnlySpan<byte>(this._buffer, 0, this._offset);
		}

		private void PushTag(Asn1Tag tag)
		{
			if (this._nestingStack == null)
			{
				this._nestingStack = new Stack<ValueTuple<Asn1Tag, int>>();
			}
			this.WriteTag(tag);
			this._nestingStack.Push(new ValueTuple<Asn1Tag, int>(tag, this._offset));
			this.WriteLength(-1);
		}

		private void PopTag(Asn1Tag tag, bool sortContents = false)
		{
			if (this._nestingStack == null || this._nestingStack.Count == 0)
			{
				throw new ArgumentException("Cannot pop the requested tag as it is not currently in progress.", "tag");
			}
			ValueTuple<Asn1Tag, int> valueTuple = this._nestingStack.Peek();
			Asn1Tag item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			if (item != tag)
			{
				throw new ArgumentException("Cannot pop the requested tag as it is not currently in progress.", "tag");
			}
			this._nestingStack.Pop();
			if (sortContents)
			{
				AsnWriter.SortContents(this._buffer, item2 + 1, this._offset);
			}
			if (this.RuleSet == AsnEncodingRules.CER)
			{
				this.WriteEndOfContents();
				return;
			}
			int num = this._offset - 1 - item2;
			int encodedLengthSubsequentByteCount = AsnWriter.GetEncodedLengthSubsequentByteCount(num);
			if (encodedLengthSubsequentByteCount == 0)
			{
				this._buffer[item2] = (byte)num;
				return;
			}
			this.EnsureWriteCapacity(encodedLengthSubsequentByteCount);
			int num2 = item2 + 1;
			Buffer.BlockCopy(this._buffer, num2, this._buffer, num2 + encodedLengthSubsequentByteCount, num);
			int offset = this._offset;
			this._offset = item2;
			this.WriteLength(num);
			this._offset = offset + encodedLengthSubsequentByteCount;
		}

		public void WriteCharacterString(UniversalTagNumber encodingType, string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			this.WriteCharacterString(encodingType, str.AsSpan());
		}

		public void WriteCharacterString(UniversalTagNumber encodingType, ReadOnlySpan<char> str)
		{
			Encoding encoding = AsnCharacterStringEncodings.GetEncoding(encodingType);
			this.WriteCharacterStringCore(new Asn1Tag(encodingType, false), encoding, str);
		}

		public void WriteCharacterString(Asn1Tag tag, UniversalTagNumber encodingType, string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			this.WriteCharacterString(tag, encodingType, str.AsSpan());
		}

		public void WriteCharacterString(Asn1Tag tag, UniversalTagNumber encodingType, ReadOnlySpan<char> str)
		{
			AsnWriter.CheckUniversalTag(tag, encodingType);
			Encoding encoding = AsnCharacterStringEncodings.GetEncoding(encodingType);
			this.WriteCharacterStringCore(tag, encoding, str);
		}

		private unsafe void WriteCharacterStringCore(Asn1Tag tag, Encoding encoding, ReadOnlySpan<char> str)
		{
			int num = -1;
			if (this.RuleSet == AsnEncodingRules.CER)
			{
				fixed (char* ptr = MemoryMarshal.GetReference<char>(str))
				{
					char* ptr2 = ptr;
					num = encoding.GetByteCount(ptr2, str.Length);
					if (num > 1000)
					{
						this.WriteConstructedCerCharacterString(tag, encoding, str, num);
						return;
					}
				}
			}
			fixed (char* ptr = MemoryMarshal.GetReference<char>(str))
			{
				char* ptr3 = ptr;
				if (num < 0)
				{
					num = encoding.GetByteCount(ptr3, str.Length);
				}
				this.WriteTag(tag.AsPrimitive());
				this.WriteLength(num);
				Span<byte> span = this._buffer.AsSpan<byte>(this._offset, num);
				fixed (byte* reference = MemoryMarshal.GetReference<byte>(span))
				{
					byte* ptr4 = reference;
					if (encoding.GetBytes(ptr3, str.Length, ptr4, span.Length) != num)
					{
						throw new InvalidOperationException();
					}
				}
				this._offset += num;
			}
		}

		private unsafe void WriteConstructedCerCharacterString(Asn1Tag tag, Encoding encoding, ReadOnlySpan<char> str, int size)
		{
			byte[] array;
			fixed (char* reference = MemoryMarshal.GetReference<char>(str))
			{
				char* ptr = reference;
				array = ArrayPool<byte>.Shared.Rent(size);
				byte[] array2;
				byte* ptr2;
				if ((array2 = array) == null || array2.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = &array2[0];
				}
				if (encoding.GetBytes(ptr, str.Length, ptr2, array.Length) != size)
				{
					throw new InvalidOperationException();
				}
				array2 = null;
			}
			this.WriteConstructedCerOctetString(tag, array.AsSpan<byte>(0, size));
			Array.Clear(array, 0, size);
			ArrayPool<byte>.Shared.Return(array, false);
		}

		private static void SortContents(byte[] buffer, int start, int end)
		{
			int num = end - start;
			if (num == 0)
			{
				return;
			}
			AsnReader asnReader = new AsnReader(new ReadOnlyMemory<byte>(buffer, start, num), AsnEncodingRules.BER);
			List<ValueTuple<int, int>> list = new List<ValueTuple<int, int>>();
			int num2 = start;
			while (asnReader.HasData)
			{
				ReadOnlyMemory<byte> encodedValue = asnReader.GetEncodedValue();
				list.Add(new ValueTuple<int, int>(num2, encodedValue.Length));
				num2 += encodedValue.Length;
			}
			AsnWriter.ArrayIndexSetOfValueComparer arrayIndexSetOfValueComparer = new AsnWriter.ArrayIndexSetOfValueComparer(buffer);
			list.Sort(arrayIndexSetOfValueComparer);
			byte[] array = ArrayPool<byte>.Shared.Rent(num);
			num2 = 0;
			foreach (ValueTuple<int, int> valueTuple in list)
			{
				int item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				Buffer.BlockCopy(buffer, item, array, num2, item2);
				num2 += item2;
			}
			Buffer.BlockCopy(array, 0, buffer, start, num);
			Array.Clear(array, 0, num);
			ArrayPool<byte>.Shared.Return(array, false);
		}

		internal unsafe static void Reverse(Span<byte> span)
		{
			int i = 0;
			int num = span.Length - 1;
			while (i < num)
			{
				byte b = *span[i];
				*span[i] = *span[num];
				*span[num] = b;
				i++;
				num--;
			}
		}

		private static void CheckUniversalTag(Asn1Tag tag, UniversalTagNumber universalTagNumber)
		{
			if (tag.TagClass == TagClass.Universal && tag.TagValue != (int)universalTagNumber)
			{
				throw new ArgumentException("Tags with TagClass Universal must have the appropriate TagValue value for the data type being read or written.", "tag");
			}
		}

		private byte[] _buffer;

		private int _offset;

		private Stack<ValueTuple<Asn1Tag, int>> _nestingStack;

		private class ArrayIndexSetOfValueComparer : IComparer<ValueTuple<int, int>>
		{
			public ArrayIndexSetOfValueComparer(byte[] data)
			{
				this._data = data;
			}

			public int Compare(ValueTuple<int, int> x, ValueTuple<int, int> y)
			{
				int item = x.Item1;
				int item2 = x.Item2;
				int item3 = y.Item1;
				int item4 = y.Item2;
				int num = SetOfValueComparer.Instance.Compare(new ReadOnlyMemory<byte>(this._data, item, item2), new ReadOnlyMemory<byte>(this._data, item3, item4));
				if (num == 0)
				{
					return item - item3;
				}
				return num;
			}

			private readonly byte[] _data;
		}
	}
}
