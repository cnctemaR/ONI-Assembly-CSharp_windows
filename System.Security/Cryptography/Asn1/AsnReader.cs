using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Cryptography.Asn1
{
	internal class AsnReader
	{
		public bool HasData
		{
			get
			{
				return !this._data.IsEmpty;
			}
		}

		public AsnReader(ReadOnlyMemory<byte> data, AsnEncodingRules ruleSet)
		{
			AsnReader.CheckEncodingRules(ruleSet);
			this._data = data;
			this._ruleSet = ruleSet;
		}

		public void ThrowIfNotEmpty()
		{
			if (this.HasData)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
		}

		public static bool TryPeekTag(ReadOnlySpan<byte> source, out Asn1Tag tag, out int bytesRead)
		{
			return Asn1Tag.TryParse(source, out tag, out bytesRead);
		}

		public Asn1Tag PeekTag()
		{
			Asn1Tag asn1Tag;
			int num;
			if (AsnReader.TryPeekTag(this._data.Span, out asn1Tag, out num))
			{
				return asn1Tag;
			}
			throw new CryptographicException("ASN1 corrupted data.");
		}

		private unsafe static bool TryReadLength(ReadOnlySpan<byte> source, AsnEncodingRules ruleSet, out int? length, out int bytesRead)
		{
			length = null;
			bytesRead = 0;
			AsnReader.CheckEncodingRules(ruleSet);
			if (source.IsEmpty)
			{
				return false;
			}
			byte b = *source[bytesRead];
			bytesRead++;
			if (b == 128)
			{
				if (ruleSet == AsnEncodingRules.DER)
				{
					bytesRead = 0;
					return false;
				}
				return true;
			}
			else
			{
				if (b < 128)
				{
					length = new int?((int)b);
					return true;
				}
				if (b == 255)
				{
					bytesRead = 0;
					return false;
				}
				byte b2 = (byte)((int)b & -129);
				if ((int)(b2 + 1) > source.Length)
				{
					bytesRead = 0;
					return false;
				}
				bool flag = ruleSet == AsnEncodingRules.DER || ruleSet == AsnEncodingRules.CER;
				if (flag && b2 > 4)
				{
					bytesRead = 0;
					return false;
				}
				uint num = 0U;
				for (int i = 0; i < (int)b2; i++)
				{
					byte b3 = *source[bytesRead];
					bytesRead++;
					if (num == 0U)
					{
						if (flag && b3 == 0)
						{
							bytesRead = 0;
							return false;
						}
						if (!flag && b3 != 0 && (int)b2 - i > 4)
						{
							bytesRead = 0;
							return false;
						}
					}
					num <<= 8;
					num |= (uint)b3;
				}
				if (num > 2147483647U)
				{
					bytesRead = 0;
					return false;
				}
				if (flag && num < 128U)
				{
					bytesRead = 0;
					return false;
				}
				length = new int?((int)num);
				return true;
			}
		}

		internal Asn1Tag ReadTagAndLength(out int? contentsLength, out int bytesRead)
		{
			Asn1Tag asn1Tag;
			int num;
			int? num2;
			int num3;
			if (AsnReader.TryPeekTag(this._data.Span, out asn1Tag, out num) && AsnReader.TryReadLength(this._data.Slice(num).Span, this._ruleSet, out num2, out num3))
			{
				int num4 = num + num3;
				if (asn1Tag.IsConstructed)
				{
					if (this._ruleSet == AsnEncodingRules.CER && num2 != null)
					{
						throw new CryptographicException("ASN1 corrupted data.");
					}
				}
				else if (num2 == null)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				bytesRead = num4;
				contentsLength = num2;
				return asn1Tag;
			}
			throw new CryptographicException("ASN1 corrupted data.");
		}

		private static void ValidateEndOfContents(Asn1Tag tag, int? length, int headerLength)
		{
			if (!tag.IsConstructed)
			{
				int? num = length;
				int num2 = 0;
				if (((num.GetValueOrDefault() == num2) & (num != null)) && headerLength == 2)
				{
					return;
				}
			}
			throw new CryptographicException("ASN1 corrupted data.");
		}

		private int SeekEndOfContents(ReadOnlyMemory<byte> source)
		{
			int num = 0;
			AsnReader asnReader = new AsnReader(source, this._ruleSet);
			int num2 = 1;
			while (asnReader.HasData)
			{
				int? num3;
				int num4;
				Asn1Tag asn1Tag = asnReader.ReadTagAndLength(out num3, out num4);
				if (asn1Tag == Asn1Tag.EndOfContents)
				{
					AsnReader.ValidateEndOfContents(asn1Tag, num3, num4);
					num2--;
					if (num2 == 0)
					{
						return num;
					}
				}
				if (num3 == null)
				{
					num2++;
					asnReader._data = asnReader._data.Slice(num4);
					num += num4;
				}
				else
				{
					ReadOnlyMemory<byte> readOnlyMemory = AsnReader.Slice(asnReader._data, 0, new int?(num4 + num3.Value));
					asnReader._data = asnReader._data.Slice(readOnlyMemory.Length);
					num += readOnlyMemory.Length;
				}
			}
			throw new CryptographicException("ASN1 corrupted data.");
		}

		public ReadOnlyMemory<byte> PeekEncodedValue()
		{
			int? num;
			int num2;
			this.ReadTagAndLength(out num, out num2);
			if (num == null)
			{
				int num3 = this.SeekEndOfContents(this._data.Slice(num2));
				return AsnReader.Slice(this._data, 0, new int?(num2 + num3 + 2));
			}
			return AsnReader.Slice(this._data, 0, new int?(num2 + num.Value));
		}

		public ReadOnlyMemory<byte> PeekContentBytes()
		{
			int? num;
			int num2;
			this.ReadTagAndLength(out num, out num2);
			if (num == null)
			{
				return AsnReader.Slice(this._data, num2, new int?(this.SeekEndOfContents(this._data.Slice(num2))));
			}
			return AsnReader.Slice(this._data, num2, new int?(num.Value));
		}

		public ReadOnlyMemory<byte> GetEncodedValue()
		{
			ReadOnlyMemory<byte> readOnlyMemory = this.PeekEncodedValue();
			this._data = this._data.Slice(readOnlyMemory.Length);
			return readOnlyMemory;
		}

		private unsafe static bool ReadBooleanValue(ReadOnlySpan<byte> source, AsnEncodingRules ruleSet)
		{
			if (source.Length != 1)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			byte b = *source[0];
			if (b == 0)
			{
				return false;
			}
			if (b != 255 && (ruleSet == AsnEncodingRules.DER || ruleSet == AsnEncodingRules.CER))
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			return true;
		}

		public bool ReadBoolean()
		{
			return this.ReadBoolean(Asn1Tag.Boolean);
		}

		public bool ReadBoolean(Asn1Tag expectedTag)
		{
			int? num;
			int num2;
			Asn1Tag asn1Tag = this.ReadTagAndLength(out num, out num2);
			AsnReader.CheckExpectedTag(asn1Tag, expectedTag, UniversalTagNumber.Boolean);
			if (asn1Tag.IsConstructed)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			bool flag = AsnReader.ReadBooleanValue(AsnReader.Slice(this._data, num2, new int?(num.Value)).Span, this._ruleSet);
			this._data = this._data.Slice(num2 + num.Value);
			return flag;
		}

		private unsafe ReadOnlyMemory<byte> GetIntegerContents(Asn1Tag expectedTag, UniversalTagNumber tagNumber, out int headerLength)
		{
			int? num;
			Asn1Tag asn1Tag = this.ReadTagAndLength(out num, out headerLength);
			AsnReader.CheckExpectedTag(asn1Tag, expectedTag, tagNumber);
			if (!asn1Tag.IsConstructed)
			{
				int? num2 = num;
				int num3 = 1;
				if (!((num2.GetValueOrDefault() < num3) & (num2 != null)))
				{
					ReadOnlyMemory<byte> readOnlyMemory = AsnReader.Slice(this._data, headerLength, new int?(num.Value));
					ReadOnlySpan<byte> span = readOnlyMemory.Span;
					if (readOnlyMemory.Length > 1)
					{
						ushort num4 = (ushort)(((int)(*span[0]) << 8) | (int)(*span[1])) & 65408;
						if (num4 == 0 || num4 == 65408)
						{
							throw new CryptographicException("ASN1 corrupted data.");
						}
					}
					return readOnlyMemory;
				}
			}
			throw new CryptographicException("ASN1 corrupted data.");
		}

		public ReadOnlyMemory<byte> GetIntegerBytes()
		{
			return this.GetIntegerBytes(Asn1Tag.Integer);
		}

		public ReadOnlyMemory<byte> GetIntegerBytes(Asn1Tag expectedTag)
		{
			int num;
			ReadOnlyMemory<byte> integerContents = this.GetIntegerContents(expectedTag, UniversalTagNumber.Integer, out num);
			this._data = this._data.Slice(num + integerContents.Length);
			return integerContents;
		}

		public BigInteger GetInteger()
		{
			return this.GetInteger(Asn1Tag.Integer);
		}

		public unsafe BigInteger GetInteger(Asn1Tag expectedTag)
		{
			int num;
			ReadOnlyMemory<byte> integerContents = this.GetIntegerContents(expectedTag, UniversalTagNumber.Integer, out num);
			byte[] array = ArrayPool<byte>.Shared.Rent(integerContents.Length);
			BigInteger bigInteger;
			try
			{
				byte b = (((*integerContents.Span[0] & 128) == 0) ? 0 : byte.MaxValue);
				new Span<byte>(array, integerContents.Length, array.Length - integerContents.Length).Fill(b);
				integerContents.CopyTo(array);
				AsnWriter.Reverse(new Span<byte>(array, 0, integerContents.Length));
				bigInteger = new BigInteger(array);
			}
			finally
			{
				Array.Clear(array, 0, array.Length);
				ArrayPool<byte>.Shared.Return(array, false);
			}
			this._data = this._data.Slice(num + integerContents.Length);
			return bigInteger;
		}

		private unsafe bool TryReadSignedInteger(int sizeLimit, Asn1Tag expectedTag, UniversalTagNumber tagNumber, out long value)
		{
			int num;
			ReadOnlyMemory<byte> integerContents = this.GetIntegerContents(expectedTag, tagNumber, out num);
			if (integerContents.Length > sizeLimit)
			{
				value = 0L;
				return false;
			}
			ReadOnlySpan<byte> span = integerContents.Span;
			long num2 = (((*span[0] & 128) > 0) ? (-1L) : 0L);
			for (int i = 0; i < integerContents.Length; i++)
			{
				num2 <<= 8;
				num2 |= (long)((ulong)(*span[i]));
			}
			this._data = this._data.Slice(num + integerContents.Length);
			value = num2;
			return true;
		}

		private unsafe bool TryReadUnsignedInteger(int sizeLimit, Asn1Tag expectedTag, UniversalTagNumber tagNumber, out ulong value)
		{
			int num;
			ReadOnlyMemory<byte> integerContents = this.GetIntegerContents(expectedTag, tagNumber, out num);
			ReadOnlySpan<byte> readOnlySpan = integerContents.Span;
			int length = integerContents.Length;
			if ((*readOnlySpan[0] & 128) > 0)
			{
				value = 0UL;
				return false;
			}
			if (readOnlySpan.Length > 1 && *readOnlySpan[0] == 0)
			{
				readOnlySpan = readOnlySpan.Slice(1);
			}
			if (readOnlySpan.Length > sizeLimit)
			{
				value = 0UL;
				return false;
			}
			ulong num2 = 0UL;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				num2 <<= 8;
				num2 |= (ulong)(*readOnlySpan[i]);
			}
			this._data = this._data.Slice(num + length);
			value = num2;
			return true;
		}

		public bool TryReadInt32(out int value)
		{
			return this.TryReadInt32(Asn1Tag.Integer, out value);
		}

		public bool TryReadInt32(Asn1Tag expectedTag, out int value)
		{
			long num;
			if (this.TryReadSignedInteger(4, expectedTag, UniversalTagNumber.Integer, out num))
			{
				value = (int)num;
				return true;
			}
			value = 0;
			return false;
		}

		public bool TryReadUInt32(out uint value)
		{
			return this.TryReadUInt32(Asn1Tag.Integer, out value);
		}

		public bool TryReadUInt32(Asn1Tag expectedTag, out uint value)
		{
			ulong num;
			if (this.TryReadUnsignedInteger(4, expectedTag, UniversalTagNumber.Integer, out num))
			{
				value = (uint)num;
				return true;
			}
			value = 0U;
			return false;
		}

		public bool TryReadInt64(out long value)
		{
			return this.TryReadInt64(Asn1Tag.Integer, out value);
		}

		public bool TryReadInt64(Asn1Tag expectedTag, out long value)
		{
			return this.TryReadSignedInteger(8, expectedTag, UniversalTagNumber.Integer, out value);
		}

		public bool TryReadUInt64(out ulong value)
		{
			return this.TryReadUInt64(Asn1Tag.Integer, out value);
		}

		public bool TryReadUInt64(Asn1Tag expectedTag, out ulong value)
		{
			return this.TryReadUnsignedInteger(8, expectedTag, UniversalTagNumber.Integer, out value);
		}

		public bool TryReadInt16(out short value)
		{
			return this.TryReadInt16(Asn1Tag.Integer, out value);
		}

		public bool TryReadInt16(Asn1Tag expectedTag, out short value)
		{
			long num;
			if (this.TryReadSignedInteger(2, expectedTag, UniversalTagNumber.Integer, out num))
			{
				value = (short)num;
				return true;
			}
			value = 0;
			return false;
		}

		public bool TryReadUInt16(out ushort value)
		{
			return this.TryReadUInt16(Asn1Tag.Integer, out value);
		}

		public bool TryReadUInt16(Asn1Tag expectedTag, out ushort value)
		{
			ulong num;
			if (this.TryReadUnsignedInteger(2, expectedTag, UniversalTagNumber.Integer, out num))
			{
				value = (ushort)num;
				return true;
			}
			value = 0;
			return false;
		}

		public bool TryReadInt8(out sbyte value)
		{
			return this.TryReadInt8(Asn1Tag.Integer, out value);
		}

		public bool TryReadInt8(Asn1Tag expectedTag, out sbyte value)
		{
			long num;
			if (this.TryReadSignedInteger(1, expectedTag, UniversalTagNumber.Integer, out num))
			{
				value = (sbyte)num;
				return true;
			}
			value = 0;
			return false;
		}

		public bool TryReadUInt8(out byte value)
		{
			return this.TryReadUInt8(Asn1Tag.Integer, out value);
		}

		public bool TryReadUInt8(Asn1Tag expectedTag, out byte value)
		{
			ulong num;
			if (this.TryReadUnsignedInteger(1, expectedTag, UniversalTagNumber.Integer, out num))
			{
				value = (byte)num;
				return true;
			}
			value = 0;
			return false;
		}

		private unsafe void ParsePrimitiveBitStringContents(ReadOnlyMemory<byte> source, out int unusedBitCount, out ReadOnlyMemory<byte> value, out byte normalizedLastByte)
		{
			if (this._ruleSet == AsnEncodingRules.CER && source.Length > 1000)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			if (source.Length == 0)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			ReadOnlySpan<byte> span = source.Span;
			unusedBitCount = (int)(*span[0]);
			if (unusedBitCount > 7)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			if (source.Length == 1)
			{
				if (unusedBitCount > 0)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				value = ReadOnlyMemory<byte>.Empty;
				normalizedLastByte = 0;
				return;
			}
			else
			{
				int num = -1 << unusedBitCount;
				byte b = *span[span.Length - 1];
				byte b2 = (byte)((int)b & num);
				if (b2 != b && (this._ruleSet == AsnEncodingRules.DER || this._ruleSet == AsnEncodingRules.CER))
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				normalizedLastByte = b2;
				value = source.Slice(1);
				return;
			}
		}

		private unsafe static void CopyBitStringValue(ReadOnlyMemory<byte> value, byte normalizedLastByte, Span<byte> destination)
		{
			if (value.Length == 0)
			{
				return;
			}
			value.Span.CopyTo(destination);
			*destination[value.Length - 1] = normalizedLastByte;
		}

		private int CountConstructedBitString(ReadOnlyMemory<byte> source, bool isIndefinite)
		{
			Span<byte> empty = Span<byte>.Empty;
			int num;
			int num2;
			return this.ProcessConstructedBitString(source, empty, null, isIndefinite, out num, out num2);
		}

		private void CopyConstructedBitString(ReadOnlyMemory<byte> source, Span<byte> destination, bool isIndefinite, out int unusedBitCount, out int bytesRead, out int bytesWritten)
		{
			bytesWritten = this.ProcessConstructedBitString(source, destination, delegate(ReadOnlyMemory<byte> value, byte lastByte, Span<byte> dest)
			{
				AsnReader.CopyBitStringValue(value, lastByte, dest);
			}, isIndefinite, out unusedBitCount, out bytesRead);
		}

		private int ProcessConstructedBitString(ReadOnlyMemory<byte> source, Span<byte> destination, AsnReader.BitStringCopyAction copyAction, bool isIndefinite, out int lastUnusedBitCount, out int bytesRead)
		{
			lastUnusedBitCount = 0;
			bytesRead = 0;
			int num = 1000;
			AsnReader asnReader = new AsnReader(source, this._ruleSet);
			Stack<ValueTuple<AsnReader, bool, int>> stack = null;
			int num2 = 0;
			Asn1Tag asn1Tag = Asn1Tag.ConstructedBitString;
			Span<byte> span = destination;
			for (;;)
			{
				if (asnReader.HasData)
				{
					int? num3;
					int num4;
					asn1Tag = asnReader.ReadTagAndLength(out num3, out num4);
					if (asn1Tag == Asn1Tag.PrimitiveBitString)
					{
						if (lastUnusedBitCount != 0)
						{
							break;
						}
						if (this._ruleSet == AsnEncodingRules.CER && num != 1000)
						{
							goto Block_4;
						}
						ReadOnlyMemory<byte> readOnlyMemory = AsnReader.Slice(asnReader._data, num4, new int?(num3.Value));
						ReadOnlyMemory<byte> readOnlyMemory2;
						byte b;
						this.ParsePrimitiveBitStringContents(readOnlyMemory, out lastUnusedBitCount, out readOnlyMemory2, out b);
						int num5 = num4 + readOnlyMemory.Length;
						asnReader._data = asnReader._data.Slice(num5);
						bytesRead += num5;
						num2 += readOnlyMemory2.Length;
						num = readOnlyMemory.Length;
						if (copyAction != null)
						{
							copyAction(readOnlyMemory2, b, span);
							span = span.Slice(readOnlyMemory2.Length);
							continue;
						}
						continue;
					}
					else if (asn1Tag == Asn1Tag.EndOfContents && isIndefinite)
					{
						AsnReader.ValidateEndOfContents(asn1Tag, num3, num4);
						bytesRead += num4;
						if (stack != null && stack.Count > 0)
						{
							ValueTuple<AsnReader, bool, int> valueTuple = stack.Pop();
							AsnReader item = valueTuple.Item1;
							bool item2 = valueTuple.Item2;
							int item3 = valueTuple.Item3;
							item._data = item._data.Slice(bytesRead);
							bytesRead += item3;
							isIndefinite = item2;
							asnReader = item;
							continue;
						}
					}
					else
					{
						if (!(asn1Tag == Asn1Tag.ConstructedBitString))
						{
							goto IL_01E7;
						}
						if (this._ruleSet == AsnEncodingRules.CER)
						{
							goto Block_10;
						}
						if (stack == null)
						{
							stack = new Stack<ValueTuple<AsnReader, bool, int>>();
						}
						stack.Push(new ValueTuple<AsnReader, bool, int>(asnReader, isIndefinite, bytesRead));
						asnReader = new AsnReader(AsnReader.Slice(asnReader._data, num4, num3), this._ruleSet);
						bytesRead = num4;
						isIndefinite = num3 == null;
						continue;
					}
				}
				if (isIndefinite && asn1Tag != Asn1Tag.EndOfContents)
				{
					goto Block_13;
				}
				if (stack != null && stack.Count > 0)
				{
					ValueTuple<AsnReader, bool, int> valueTuple2 = stack.Pop();
					AsnReader item4 = valueTuple2.Item1;
					bool item5 = valueTuple2.Item2;
					int item6 = valueTuple2.Item3;
					asnReader = item4;
					asnReader._data = asnReader._data.Slice(bytesRead);
					isIndefinite = item5;
					bytesRead += item6;
				}
				else
				{
					asnReader = null;
				}
				if (asnReader == null)
				{
					return num2;
				}
			}
			throw new CryptographicException("ASN1 corrupted data.");
			Block_4:
			throw new CryptographicException("ASN1 corrupted data.");
			Block_10:
			throw new CryptographicException("ASN1 corrupted data.");
			IL_01E7:
			throw new CryptographicException("ASN1 corrupted data.");
			Block_13:
			throw new CryptographicException("ASN1 corrupted data.");
		}

		private bool TryCopyConstructedBitStringValue(ReadOnlyMemory<byte> source, Span<byte> dest, bool isIndefinite, out int unusedBitCount, out int bytesRead, out int bytesWritten)
		{
			int num = this.CountConstructedBitString(source, isIndefinite);
			if (this._ruleSet == AsnEncodingRules.CER && num < 1000)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			if (dest.Length < num)
			{
				unusedBitCount = 0;
				bytesRead = 0;
				bytesWritten = 0;
				return false;
			}
			this.CopyConstructedBitString(source, dest, isIndefinite, out unusedBitCount, out bytesRead, out bytesWritten);
			return true;
		}

		private bool TryGetPrimitiveBitStringValue(Asn1Tag expectedTag, out Asn1Tag actualTag, out int? contentsLength, out int headerLength, out int unusedBitCount, out ReadOnlyMemory<byte> value, out byte normalizedLastByte)
		{
			actualTag = this.ReadTagAndLength(out contentsLength, out headerLength);
			AsnReader.CheckExpectedTag(actualTag, expectedTag, UniversalTagNumber.BitString);
			if (!actualTag.IsConstructed)
			{
				ReadOnlyMemory<byte> readOnlyMemory = AsnReader.Slice(this._data, headerLength, new int?(contentsLength.Value));
				this.ParsePrimitiveBitStringContents(readOnlyMemory, out unusedBitCount, out value, out normalizedLastByte);
				return true;
			}
			if (this._ruleSet == AsnEncodingRules.DER)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			unusedBitCount = 0;
			value = default(ReadOnlyMemory<byte>);
			normalizedLastByte = 0;
			return false;
		}

		public bool TryGetPrimitiveBitStringValue(out int unusedBitCount, out ReadOnlyMemory<byte> contents)
		{
			return this.TryGetPrimitiveBitStringValue(Asn1Tag.PrimitiveBitString, out unusedBitCount, out contents);
		}

		public unsafe bool TryGetPrimitiveBitStringValue(Asn1Tag expectedTag, out int unusedBitCount, out ReadOnlyMemory<byte> value)
		{
			Asn1Tag asn1Tag;
			int? num;
			int num2;
			byte b;
			bool flag = this.TryGetPrimitiveBitStringValue(expectedTag, out asn1Tag, out num, out num2, out unusedBitCount, out value, out b);
			if (flag)
			{
				if (value.Length != 0 && b != *value.Span[value.Length - 1])
				{
					unusedBitCount = 0;
					value = default(ReadOnlyMemory<byte>);
					return false;
				}
				this._data = this._data.Slice(num2 + value.Length + 1);
			}
			return flag;
		}

		public bool TryCopyBitStringBytes(Span<byte> destination, out int unusedBitCount, out int bytesWritten)
		{
			return this.TryCopyBitStringBytes(Asn1Tag.PrimitiveBitString, destination, out unusedBitCount, out bytesWritten);
		}

		public bool TryCopyBitStringBytes(Asn1Tag expectedTag, Span<byte> destination, out int unusedBitCount, out int bytesWritten)
		{
			Asn1Tag asn1Tag;
			int? num;
			int num2;
			ReadOnlyMemory<byte> readOnlyMemory;
			byte b;
			if (!this.TryGetPrimitiveBitStringValue(expectedTag, out asn1Tag, out num, out num2, out unusedBitCount, out readOnlyMemory, out b))
			{
				int num3;
				bool flag = this.TryCopyConstructedBitStringValue(AsnReader.Slice(this._data, num2, num), destination, num == null, out unusedBitCount, out num3, out bytesWritten);
				if (flag)
				{
					this._data = this._data.Slice(num2 + num3);
				}
				return flag;
			}
			if (readOnlyMemory.Length > destination.Length)
			{
				bytesWritten = 0;
				unusedBitCount = 0;
				return false;
			}
			AsnReader.CopyBitStringValue(readOnlyMemory, b, destination);
			bytesWritten = readOnlyMemory.Length;
			this._data = this._data.Slice(num2 + readOnlyMemory.Length + 1);
			return true;
		}

		public TFlagsEnum GetNamedBitListValue<TFlagsEnum>() where TFlagsEnum : struct
		{
			return this.GetNamedBitListValue<TFlagsEnum>(Asn1Tag.PrimitiveBitString);
		}

		public TFlagsEnum GetNamedBitListValue<TFlagsEnum>(Asn1Tag expectedTag) where TFlagsEnum : struct
		{
			Type typeFromHandle = typeof(TFlagsEnum);
			return (TFlagsEnum)((object)Enum.ToObject(typeFromHandle, this.GetNamedBitListValue(expectedTag, typeFromHandle)));
		}

		public Enum GetNamedBitListValue(Type tFlagsEnum)
		{
			return this.GetNamedBitListValue(Asn1Tag.PrimitiveBitString, tFlagsEnum);
		}

		public unsafe Enum GetNamedBitListValue(Asn1Tag expectedTag, Type tFlagsEnum)
		{
			Type enumUnderlyingType = tFlagsEnum.GetEnumUnderlyingType();
			if (!tFlagsEnum.IsDefined(typeof(FlagsAttribute), false))
			{
				throw new ArgumentException("Named bit list operations require an enum with the [Flags] attribute.", "tFlagsEnum");
			}
			int num = Marshal.SizeOf(enumUnderlyingType);
			Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)num], num);
			ReadOnlyMemory<byte> data = this._data;
			Enum @enum;
			try
			{
				int num2;
				int num3;
				if (!this.TryCopyBitStringBytes(expectedTag, span, out num2, out num3))
				{
					throw new CryptographicException(SR.Format("The encoded named bit list value is larger than the value size of the '{0}' enum.", tFlagsEnum.Name));
				}
				if (num3 == 0)
				{
					@enum = (Enum)Enum.ToObject(tFlagsEnum, 0);
				}
				else
				{
					ReadOnlySpan<byte> readOnlySpan = span.Slice(0, num3);
					if (this._ruleSet == AsnEncodingRules.DER || this._ruleSet == AsnEncodingRules.CER)
					{
						bool flag = *readOnlySpan[num3 - 1] != 0;
						byte b = (byte)(1 << num2);
						if (((flag ? 1 : 0) & b) == 0)
						{
							throw new CryptographicException("ASN1 corrupted data.");
						}
					}
					@enum = (Enum)Enum.ToObject(tFlagsEnum, AsnReader.InterpretNamedBitListReversed(readOnlySpan));
				}
			}
			catch
			{
				this._data = data;
				throw;
			}
			return @enum;
		}

		private unsafe static long InterpretNamedBitListReversed(ReadOnlySpan<byte> valueSpan)
		{
			long num = 0L;
			long num2 = 1L;
			for (int i = 0; i < valueSpan.Length; i++)
			{
				byte b = *valueSpan[i];
				for (int j = 7; j >= 0; j--)
				{
					int num3 = 1 << j;
					if (((int)b & num3) != 0)
					{
						num |= num2;
					}
					num2 <<= 1;
				}
			}
			return num;
		}

		public ReadOnlyMemory<byte> GetEnumeratedBytes()
		{
			return this.GetEnumeratedBytes(Asn1Tag.Enumerated);
		}

		public ReadOnlyMemory<byte> GetEnumeratedBytes(Asn1Tag expectedTag)
		{
			int num;
			ReadOnlyMemory<byte> integerContents = this.GetIntegerContents(expectedTag, UniversalTagNumber.Enumerated, out num);
			this._data = this._data.Slice(num + integerContents.Length);
			return integerContents;
		}

		public TEnum GetEnumeratedValue<TEnum>() where TEnum : struct
		{
			Type typeFromHandle = typeof(TEnum);
			return (TEnum)((object)Enum.ToObject(typeFromHandle, this.GetEnumeratedValue(typeFromHandle)));
		}

		public TEnum GetEnumeratedValue<TEnum>(Asn1Tag expectedTag) where TEnum : struct
		{
			Type typeFromHandle = typeof(TEnum);
			return (TEnum)((object)Enum.ToObject(typeFromHandle, this.GetEnumeratedValue(expectedTag, typeFromHandle)));
		}

		public Enum GetEnumeratedValue(Type tEnum)
		{
			return this.GetEnumeratedValue(Asn1Tag.Enumerated, tEnum);
		}

		public Enum GetEnumeratedValue(Asn1Tag expectedTag, Type tEnum)
		{
			Type enumUnderlyingType = tEnum.GetEnumUnderlyingType();
			if (tEnum.IsDefined(typeof(FlagsAttribute), false))
			{
				throw new ArgumentException("ASN.1 Enumerated values only apply to enum types without the [Flags] attribute.", "tEnum");
			}
			int num = Marshal.SizeOf(enumUnderlyingType);
			if (enumUnderlyingType == typeof(int) || enumUnderlyingType == typeof(long) || enumUnderlyingType == typeof(short) || enumUnderlyingType == typeof(sbyte))
			{
				long num2;
				if (!this.TryReadSignedInteger(num, expectedTag, UniversalTagNumber.Enumerated, out num2))
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				return (Enum)Enum.ToObject(tEnum, num2);
			}
			else
			{
				if (!(enumUnderlyingType == typeof(uint)) && !(enumUnderlyingType == typeof(ulong)) && !(enumUnderlyingType == typeof(ushort)) && !(enumUnderlyingType == typeof(byte)))
				{
					throw new CryptographicException();
				}
				ulong num3;
				if (!this.TryReadUnsignedInteger(num, expectedTag, UniversalTagNumber.Enumerated, out num3))
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				return (Enum)Enum.ToObject(tEnum, num3);
			}
		}

		private bool TryGetPrimitiveOctetStringBytes(Asn1Tag expectedTag, out Asn1Tag actualTag, out int? contentLength, out int headerLength, out ReadOnlyMemory<byte> contents, UniversalTagNumber universalTagNumber = UniversalTagNumber.OctetString)
		{
			actualTag = this.ReadTagAndLength(out contentLength, out headerLength);
			AsnReader.CheckExpectedTag(actualTag, expectedTag, universalTagNumber);
			if (actualTag.IsConstructed)
			{
				if (this._ruleSet == AsnEncodingRules.DER)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				contents = default(ReadOnlyMemory<byte>);
				return false;
			}
			else
			{
				ReadOnlyMemory<byte> readOnlyMemory = AsnReader.Slice(this._data, headerLength, new int?(contentLength.Value));
				if (this._ruleSet == AsnEncodingRules.CER && readOnlyMemory.Length > 1000)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				contents = readOnlyMemory;
				return true;
			}
		}

		private bool TryGetPrimitiveOctetStringBytes(Asn1Tag expectedTag, UniversalTagNumber universalTagNumber, out ReadOnlyMemory<byte> contents)
		{
			Asn1Tag asn1Tag;
			int? num;
			int num2;
			if (this.TryGetPrimitiveOctetStringBytes(expectedTag, out asn1Tag, out num, out num2, out contents, universalTagNumber))
			{
				this._data = this._data.Slice(num2 + contents.Length);
				return true;
			}
			return false;
		}

		public bool TryGetPrimitiveOctetStringBytes(out ReadOnlyMemory<byte> contents)
		{
			return this.TryGetPrimitiveOctetStringBytes(Asn1Tag.PrimitiveOctetString, out contents);
		}

		public bool TryGetPrimitiveOctetStringBytes(Asn1Tag expectedTag, out ReadOnlyMemory<byte> contents)
		{
			return this.TryGetPrimitiveOctetStringBytes(expectedTag, UniversalTagNumber.OctetString, out contents);
		}

		private int CountConstructedOctetString(ReadOnlyMemory<byte> source, bool isIndefinite)
		{
			int num2;
			int num = this.CopyConstructedOctetString(source, Span<byte>.Empty, false, isIndefinite, out num2);
			if (this._ruleSet == AsnEncodingRules.CER && num <= 1000)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			return num;
		}

		private void CopyConstructedOctetString(ReadOnlyMemory<byte> source, Span<byte> destination, bool isIndefinite, out int bytesRead, out int bytesWritten)
		{
			bytesWritten = this.CopyConstructedOctetString(source, destination, true, isIndefinite, out bytesRead);
		}

		private int CopyConstructedOctetString(ReadOnlyMemory<byte> source, Span<byte> destination, bool write, bool isIndefinite, out int bytesRead)
		{
			bytesRead = 0;
			int num = 1000;
			AsnReader asnReader = new AsnReader(source, this._ruleSet);
			Stack<ValueTuple<AsnReader, bool, int>> stack = null;
			int num2 = 0;
			Asn1Tag asn1Tag = Asn1Tag.ConstructedBitString;
			Span<byte> span = destination;
			for (;;)
			{
				if (asnReader.HasData)
				{
					int? num3;
					int num4;
					asn1Tag = asnReader.ReadTagAndLength(out num3, out num4);
					if (asn1Tag == Asn1Tag.PrimitiveOctetString)
					{
						if (this._ruleSet == AsnEncodingRules.CER && num != 1000)
						{
							break;
						}
						ReadOnlyMemory<byte> readOnlyMemory = AsnReader.Slice(asnReader._data, num4, new int?(num3.Value));
						int num5 = num4 + readOnlyMemory.Length;
						asnReader._data = asnReader._data.Slice(num5);
						bytesRead += num5;
						num2 += readOnlyMemory.Length;
						num = readOnlyMemory.Length;
						if (this._ruleSet == AsnEncodingRules.CER && num > 1000)
						{
							goto Block_5;
						}
						if (write)
						{
							readOnlyMemory.Span.CopyTo(span);
							span = span.Slice(readOnlyMemory.Length);
							continue;
						}
						continue;
					}
					else if (asn1Tag == Asn1Tag.EndOfContents && isIndefinite)
					{
						AsnReader.ValidateEndOfContents(asn1Tag, num3, num4);
						bytesRead += num4;
						if (stack != null && stack.Count > 0)
						{
							ValueTuple<AsnReader, bool, int> valueTuple = stack.Pop();
							AsnReader item = valueTuple.Item1;
							bool item2 = valueTuple.Item2;
							int item3 = valueTuple.Item3;
							item._data = item._data.Slice(bytesRead);
							bytesRead += item3;
							isIndefinite = item2;
							asnReader = item;
							continue;
						}
					}
					else
					{
						if (!(asn1Tag == Asn1Tag.ConstructedOctetString))
						{
							goto IL_01E7;
						}
						if (this._ruleSet == AsnEncodingRules.CER)
						{
							goto Block_11;
						}
						if (stack == null)
						{
							stack = new Stack<ValueTuple<AsnReader, bool, int>>();
						}
						stack.Push(new ValueTuple<AsnReader, bool, int>(asnReader, isIndefinite, bytesRead));
						asnReader = new AsnReader(AsnReader.Slice(asnReader._data, num4, num3), this._ruleSet);
						bytesRead = num4;
						isIndefinite = num3 == null;
						continue;
					}
				}
				if (isIndefinite && asn1Tag != Asn1Tag.EndOfContents)
				{
					goto Block_14;
				}
				if (stack != null && stack.Count > 0)
				{
					ValueTuple<AsnReader, bool, int> valueTuple2 = stack.Pop();
					AsnReader item4 = valueTuple2.Item1;
					bool item5 = valueTuple2.Item2;
					int item6 = valueTuple2.Item3;
					asnReader = item4;
					asnReader._data = asnReader._data.Slice(bytesRead);
					isIndefinite = item5;
					bytesRead += item6;
				}
				else
				{
					asnReader = null;
				}
				if (asnReader == null)
				{
					return num2;
				}
			}
			throw new CryptographicException("ASN1 corrupted data.");
			Block_5:
			throw new CryptographicException("ASN1 corrupted data.");
			Block_11:
			throw new CryptographicException("ASN1 corrupted data.");
			IL_01E7:
			throw new CryptographicException("ASN1 corrupted data.");
			Block_14:
			throw new CryptographicException("ASN1 corrupted data.");
		}

		private bool TryCopyConstructedOctetStringContents(ReadOnlyMemory<byte> source, Span<byte> dest, bool isIndefinite, out int bytesRead, out int bytesWritten)
		{
			bytesRead = 0;
			int num = this.CountConstructedOctetString(source, isIndefinite);
			if (dest.Length < num)
			{
				bytesWritten = 0;
				return false;
			}
			this.CopyConstructedOctetString(source, dest, isIndefinite, out bytesRead, out bytesWritten);
			return true;
		}

		public bool TryCopyOctetStringBytes(Span<byte> destination, out int bytesWritten)
		{
			return this.TryCopyOctetStringBytes(Asn1Tag.PrimitiveOctetString, destination, out bytesWritten);
		}

		public bool TryCopyOctetStringBytes(Asn1Tag expectedTag, Span<byte> destination, out int bytesWritten)
		{
			Asn1Tag asn1Tag;
			int? num;
			int num2;
			ReadOnlyMemory<byte> readOnlyMemory;
			if (!this.TryGetPrimitiveOctetStringBytes(expectedTag, out asn1Tag, out num, out num2, out readOnlyMemory, UniversalTagNumber.OctetString))
			{
				int num3;
				bool flag = this.TryCopyConstructedOctetStringContents(AsnReader.Slice(this._data, num2, num), destination, num == null, out num3, out bytesWritten);
				if (flag)
				{
					this._data = this._data.Slice(num2 + num3);
				}
				return flag;
			}
			if (readOnlyMemory.Length > destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			readOnlyMemory.Span.CopyTo(destination);
			bytesWritten = readOnlyMemory.Length;
			this._data = this._data.Slice(num2 + readOnlyMemory.Length);
			return true;
		}

		public void ReadNull()
		{
			this.ReadNull(Asn1Tag.Null);
		}

		public void ReadNull(Asn1Tag expectedTag)
		{
			int? num;
			int num2;
			Asn1Tag asn1Tag = this.ReadTagAndLength(out num, out num2);
			AsnReader.CheckExpectedTag(asn1Tag, expectedTag, UniversalTagNumber.Null);
			if (!asn1Tag.IsConstructed)
			{
				int? num3 = num;
				int num4 = 0;
				if ((num3.GetValueOrDefault() == num4) & (num3 != null))
				{
					this._data = this._data.Slice(num2);
					return;
				}
			}
			throw new CryptographicException("ASN1 corrupted data.");
		}

		private unsafe static void ReadSubIdentifier(ReadOnlySpan<byte> source, out int bytesRead, out long? smallValue, out BigInteger? largeValue)
		{
			if (*source[0] == 128)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			int num = -1;
			int i;
			for (i = 0; i < source.Length; i++)
			{
				if ((*source[i] & 128) == 0)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			bytesRead = num + 1;
			long num2 = 0L;
			if (bytesRead <= 9)
			{
				for (i = 0; i < bytesRead; i++)
				{
					byte b = *source[i];
					num2 <<= 7;
					num2 |= (long)((ulong)(b & 127));
				}
				largeValue = null;
				smallValue = new long?(num2);
				return;
			}
			int num3 = (bytesRead / 8 + 1) * 7;
			byte[] array = ArrayPool<byte>.Shared.Rent(num3);
			Array.Clear(array, 0, array.Length);
			Span<byte> span = array;
			Span<byte> span2 = new Span<byte>(stackalloc byte[(UIntPtr)8], 8);
			int j = bytesRead;
			i = bytesRead - 8;
			while (j > 0)
			{
				byte b2 = *source[i];
				num2 <<= 7;
				num2 |= (long)((ulong)(b2 & 127));
				i++;
				if (i >= j)
				{
					BinaryPrimitives.WriteInt64LittleEndian(span2, num2);
					span2.Slice(0, 7).CopyTo(span);
					span = span.Slice(7);
					num2 = 0L;
					j -= 8;
					i = Math.Max(0, j - 8);
				}
			}
			int num4 = array.Length - span.Length;
			largeValue = new BigInteger?(new BigInteger(array));
			smallValue = null;
			Array.Clear(array, 0, num4);
			ArrayPool<byte>.Shared.Return(array, false);
		}

		private string ReadObjectIdentifierAsString(Asn1Tag expectedTag, out int totalBytesRead)
		{
			int? num;
			int num2;
			Asn1Tag asn1Tag = this.ReadTagAndLength(out num, out num2);
			AsnReader.CheckExpectedTag(asn1Tag, expectedTag, UniversalTagNumber.ObjectIdentifier);
			if (!asn1Tag.IsConstructed)
			{
				int? num3 = num;
				int num4 = 1;
				if (!((num3.GetValueOrDefault() < num4) & (num3 != null)))
				{
					ReadOnlySpan<byte> readOnlySpan = AsnReader.Slice(this._data, num2, new int?(num.Value)).Span;
					StringBuilder stringBuilder = new StringBuilder((int)((byte)readOnlySpan.Length * 4));
					int num5;
					long? num6;
					BigInteger? bigInteger;
					AsnReader.ReadSubIdentifier(readOnlySpan, out num5, out num6, out bigInteger);
					if (num6 != null)
					{
						long num7 = num6.Value;
						byte b;
						if (num7 < 40L)
						{
							b = 0;
						}
						else if (num7 < 80L)
						{
							b = 1;
							num7 -= 40L;
						}
						else
						{
							b = 2;
							num7 -= 80L;
						}
						stringBuilder.Append(b);
						stringBuilder.Append('.');
						stringBuilder.Append(num7);
					}
					else
					{
						BigInteger bigInteger2 = bigInteger.Value;
						byte b = 2;
						bigInteger2 -= 80;
						stringBuilder.Append(b);
						stringBuilder.Append('.');
						stringBuilder.Append(bigInteger2.ToString());
					}
					readOnlySpan = readOnlySpan.Slice(num5);
					while (!readOnlySpan.IsEmpty)
					{
						AsnReader.ReadSubIdentifier(readOnlySpan, out num5, out num6, out bigInteger);
						stringBuilder.Append('.');
						if (num6 != null)
						{
							stringBuilder.Append(num6.Value);
						}
						else
						{
							stringBuilder.Append(bigInteger.Value.ToString());
						}
						readOnlySpan = readOnlySpan.Slice(num5);
					}
					totalBytesRead = num2 + num.Value;
					return stringBuilder.ToString();
				}
			}
			throw new CryptographicException("ASN1 corrupted data.");
		}

		public string ReadObjectIdentifierAsString()
		{
			return this.ReadObjectIdentifierAsString(Asn1Tag.ObjectIdentifier);
		}

		public string ReadObjectIdentifierAsString(Asn1Tag expectedTag)
		{
			int num;
			string text = this.ReadObjectIdentifierAsString(expectedTag, out num);
			this._data = this._data.Slice(num);
			return text;
		}

		public Oid ReadObjectIdentifier(bool skipFriendlyName = false)
		{
			return this.ReadObjectIdentifier(Asn1Tag.ObjectIdentifier, skipFriendlyName);
		}

		public Oid ReadObjectIdentifier(Asn1Tag expectedTag, bool skipFriendlyName = false)
		{
			int num;
			string text = this.ReadObjectIdentifierAsString(expectedTag, out num);
			Oid oid = (skipFriendlyName ? new Oid(text, text) : new Oid(text));
			this._data = this._data.Slice(num);
			return oid;
		}

		private bool TryCopyCharacterStringBytes(Asn1Tag expectedTag, UniversalTagNumber universalTagNumber, Span<byte> destination, out int bytesRead, out int bytesWritten)
		{
			Asn1Tag asn1Tag;
			int? num;
			int num2;
			ReadOnlyMemory<byte> readOnlyMemory;
			if (this.TryGetPrimitiveOctetStringBytes(expectedTag, out asn1Tag, out num, out num2, out readOnlyMemory, universalTagNumber))
			{
				bytesWritten = readOnlyMemory.Length;
				if (destination.Length < bytesWritten)
				{
					bytesWritten = 0;
					bytesRead = 0;
					return false;
				}
				readOnlyMemory.Span.CopyTo(destination);
				bytesRead = num2 + bytesWritten;
				return true;
			}
			else
			{
				int num3;
				bool flag = this.TryCopyConstructedOctetStringContents(AsnReader.Slice(this._data, num2, num), destination, num == null, out num3, out bytesWritten);
				if (flag)
				{
					bytesRead = num2 + num3;
					return flag;
				}
				bytesRead = 0;
				return flag;
			}
		}

		private unsafe static bool TryCopyCharacterString(ReadOnlySpan<byte> source, Span<char> destination, Encoding encoding, out int charsWritten)
		{
			if (source.Length == 0)
			{
				charsWritten = 0;
				return true;
			}
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(source))
			{
				byte* ptr = reference;
				fixed (char* reference2 = MemoryMarshal.GetReference<char>(destination))
				{
					char* ptr2 = reference2;
					try
					{
						if (encoding.GetCharCount(ptr, source.Length) > destination.Length)
						{
							charsWritten = 0;
							return false;
						}
						charsWritten = encoding.GetChars(ptr, source.Length, ptr2, destination.Length);
					}
					catch (DecoderFallbackException ex)
					{
						throw new CryptographicException("ASN1 corrupted data.", ex);
					}
					return true;
				}
			}
		}

		private unsafe string GetCharacterString(Asn1Tag expectedTag, UniversalTagNumber universalTagNumber, Encoding encoding)
		{
			byte[] array = null;
			int num;
			ReadOnlySpan<byte> octetStringContents = this.GetOctetStringContents(expectedTag, universalTagNumber, out num, ref array, default(Span<byte>));
			string text2;
			try
			{
				string text;
				if (octetStringContents.Length == 0)
				{
					text = string.Empty;
				}
				else
				{
					try
					{
						fixed (byte* ptr = MemoryMarshal.GetReference<byte>(octetStringContents))
						{
							byte* ptr2 = ptr;
							try
							{
								text = encoding.GetString(ptr2, octetStringContents.Length);
							}
							catch (DecoderFallbackException ex)
							{
								throw new CryptographicException("ASN1 corrupted data.", ex);
							}
						}
					}
					finally
					{
						byte* ptr = null;
					}
				}
				this._data = this._data.Slice(num);
				text2 = text;
			}
			finally
			{
				if (array != null)
				{
					Array.Clear(array, 0, octetStringContents.Length);
					ArrayPool<byte>.Shared.Return(array, false);
				}
			}
			return text2;
		}

		private bool TryCopyCharacterString(Asn1Tag expectedTag, UniversalTagNumber universalTagNumber, Encoding encoding, Span<char> destination, out int charsWritten)
		{
			byte[] array = null;
			int num;
			ReadOnlySpan<byte> octetStringContents = this.GetOctetStringContents(expectedTag, universalTagNumber, out num, ref array, default(Span<byte>));
			bool flag2;
			try
			{
				bool flag = AsnReader.TryCopyCharacterString(octetStringContents, destination, encoding, out charsWritten);
				if (flag)
				{
					this._data = this._data.Slice(num);
				}
				flag2 = flag;
			}
			finally
			{
				if (array != null)
				{
					Array.Clear(array, 0, octetStringContents.Length);
					ArrayPool<byte>.Shared.Return(array, false);
				}
			}
			return flag2;
		}

		public bool TryGetPrimitiveCharacterStringBytes(UniversalTagNumber encodingType, out ReadOnlyMemory<byte> contents)
		{
			return this.TryGetPrimitiveCharacterStringBytes(new Asn1Tag(encodingType, false), encodingType, out contents);
		}

		public bool TryGetPrimitiveCharacterStringBytes(Asn1Tag expectedTag, UniversalTagNumber encodingType, out ReadOnlyMemory<byte> contents)
		{
			AsnReader.CheckCharacterStringEncodingType(encodingType);
			return this.TryGetPrimitiveOctetStringBytes(expectedTag, encodingType, out contents);
		}

		public bool TryCopyCharacterStringBytes(UniversalTagNumber encodingType, Span<byte> destination, out int bytesWritten)
		{
			return this.TryCopyCharacterStringBytes(new Asn1Tag(encodingType, false), encodingType, destination, out bytesWritten);
		}

		public bool TryCopyCharacterStringBytes(Asn1Tag expectedTag, UniversalTagNumber encodingType, Span<byte> destination, out int bytesWritten)
		{
			AsnReader.CheckCharacterStringEncodingType(encodingType);
			int num;
			bool flag = this.TryCopyCharacterStringBytes(expectedTag, encodingType, destination, out num, out bytesWritten);
			if (flag)
			{
				this._data = this._data.Slice(num);
			}
			return flag;
		}

		public bool TryCopyCharacterString(UniversalTagNumber encodingType, Span<char> destination, out int charsWritten)
		{
			return this.TryCopyCharacterString(new Asn1Tag(encodingType, false), encodingType, destination, out charsWritten);
		}

		public bool TryCopyCharacterString(Asn1Tag expectedTag, UniversalTagNumber encodingType, Span<char> destination, out int charsWritten)
		{
			Encoding encoding = AsnCharacterStringEncodings.GetEncoding(encodingType);
			return this.TryCopyCharacterString(expectedTag, encodingType, encoding, destination, out charsWritten);
		}

		public string GetCharacterString(UniversalTagNumber encodingType)
		{
			return this.GetCharacterString(new Asn1Tag(encodingType, false), encodingType);
		}

		public string GetCharacterString(Asn1Tag expectedTag, UniversalTagNumber encodingType)
		{
			Encoding encoding = AsnCharacterStringEncodings.GetEncoding(encodingType);
			return this.GetCharacterString(expectedTag, encodingType, encoding);
		}

		public AsnReader ReadSequence()
		{
			return this.ReadSequence(Asn1Tag.Sequence);
		}

		public AsnReader ReadSequence(Asn1Tag expectedTag)
		{
			int? num;
			int num2;
			Asn1Tag asn1Tag = this.ReadTagAndLength(out num, out num2);
			AsnReader.CheckExpectedTag(asn1Tag, expectedTag, UniversalTagNumber.Sequence);
			if (!asn1Tag.IsConstructed)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			int num3 = 0;
			if (num == null)
			{
				num = new int?(this.SeekEndOfContents(this._data.Slice(num2)));
				num3 = 2;
			}
			ReadOnlyMemory<byte> readOnlyMemory = AsnReader.Slice(this._data, num2, new int?(num.Value));
			this._data = this._data.Slice(num2 + readOnlyMemory.Length + num3);
			return new AsnReader(readOnlyMemory, this._ruleSet);
		}

		public AsnReader ReadSetOf(bool skipSortOrderValidation = false)
		{
			return this.ReadSetOf(Asn1Tag.SetOf, skipSortOrderValidation);
		}

		public AsnReader ReadSetOf(Asn1Tag expectedTag, bool skipSortOrderValidation = false)
		{
			int? num;
			int num2;
			Asn1Tag asn1Tag = this.ReadTagAndLength(out num, out num2);
			AsnReader.CheckExpectedTag(asn1Tag, expectedTag, UniversalTagNumber.Set);
			if (!asn1Tag.IsConstructed)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			int num3 = 0;
			if (num == null)
			{
				num = new int?(this.SeekEndOfContents(this._data.Slice(num2)));
				num3 = 2;
			}
			ReadOnlyMemory<byte> readOnlyMemory = AsnReader.Slice(this._data, num2, new int?(num.Value));
			if (!skipSortOrderValidation && (this._ruleSet == AsnEncodingRules.DER || this._ruleSet == AsnEncodingRules.CER))
			{
				AsnReader asnReader = new AsnReader(readOnlyMemory, this._ruleSet);
				ReadOnlyMemory<byte> readOnlyMemory2 = ReadOnlyMemory<byte>.Empty;
				SetOfValueComparer instance = SetOfValueComparer.Instance;
				while (asnReader.HasData)
				{
					ReadOnlyMemory<byte> readOnlyMemory3 = readOnlyMemory2;
					readOnlyMemory2 = asnReader.GetEncodedValue();
					if (instance.Compare(readOnlyMemory2, readOnlyMemory3) < 0)
					{
						throw new CryptographicException("ASN1 corrupted data.");
					}
				}
			}
			this._data = this._data.Slice(num2 + readOnlyMemory.Length + num3);
			return new AsnReader(readOnlyMemory, this._ruleSet);
		}

		private static int ParseNonNegativeIntAndSlice(ref ReadOnlySpan<byte> data, int bytesToRead)
		{
			int num = AsnReader.ParseNonNegativeInt(AsnReader.Slice(data, 0, bytesToRead));
			data = data.Slice(bytesToRead);
			return num;
		}

		private static int ParseNonNegativeInt(ReadOnlySpan<byte> data)
		{
			uint num;
			int num2;
			if (Utf8Parser.TryParse(data, out num, out num2, '\0') && num <= 2147483647U && num2 == data.Length)
			{
				return (int)num;
			}
			throw new CryptographicException("ASN1 corrupted data.");
		}

		private unsafe DateTimeOffset ParseUtcTime(ReadOnlySpan<byte> contentOctets, int twoDigitYearMax)
		{
			if ((this._ruleSet == AsnEncodingRules.DER || this._ruleSet == AsnEncodingRules.CER) && contentOctets.Length != 13)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			if (contentOctets.Length < 11 || contentOctets.Length > 17 || (contentOctets.Length & 1) != 1)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			ReadOnlySpan<byte> readOnlySpan = contentOctets;
			int num = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2);
			int num2 = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2);
			int num3 = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2);
			int num4 = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2);
			int num5 = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2);
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			bool flag = false;
			if (contentOctets.Length == 17 || contentOctets.Length == 13)
			{
				num6 = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2);
			}
			if (contentOctets.Length == 11 || contentOctets.Length == 13)
			{
				if (*readOnlySpan[0] != 90)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
			}
			else
			{
				if (*readOnlySpan[0] == 45)
				{
					flag = true;
				}
				else if (*readOnlySpan[0] != 43)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				readOnlySpan = readOnlySpan.Slice(1);
				num7 = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2);
				num8 = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2);
			}
			if (num8 > 59)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			TimeSpan timeSpan = new TimeSpan(num7, num8, 0);
			if (flag)
			{
				timeSpan = -timeSpan;
			}
			int num9 = twoDigitYearMax / 100;
			if (num > twoDigitYearMax % 100)
			{
				num9--;
			}
			int num10 = num9 * 100 + num;
			DateTimeOffset dateTimeOffset;
			try
			{
				dateTimeOffset = new DateTimeOffset(num10, num2, num3, num4, num5, num6, timeSpan);
			}
			catch (Exception ex)
			{
				throw new CryptographicException("ASN1 corrupted data.", ex);
			}
			return dateTimeOffset;
		}

		public DateTimeOffset GetUtcTime(int twoDigitYearMax = 2049)
		{
			return this.GetUtcTime(Asn1Tag.UtcTime, twoDigitYearMax);
		}

		public unsafe DateTimeOffset GetUtcTime(Asn1Tag expectedTag, int twoDigitYearMax = 2049)
		{
			byte[] array = null;
			Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)17], 17);
			int num;
			ReadOnlySpan<byte> octetStringContents = this.GetOctetStringContents(expectedTag, UniversalTagNumber.UtcTime, out num, ref array, span);
			DateTimeOffset dateTimeOffset = this.ParseUtcTime(octetStringContents, twoDigitYearMax);
			if (array != null)
			{
				Array.Clear(array, 0, octetStringContents.Length);
				ArrayPool<byte>.Shared.Return(array, false);
			}
			this._data = this._data.Slice(num);
			return dateTimeOffset;
		}

		private static byte? ParseGeneralizedTime_GetNextState(byte octet)
		{
			if (octet == 90 || octet == 45 || octet == 43)
			{
				return new byte?(2);
			}
			if (octet == 46 || octet == 44)
			{
				return new byte?(1);
			}
			return null;
		}

		private unsafe static DateTimeOffset ParseGeneralizedTime(AsnEncodingRules ruleSet, ReadOnlySpan<byte> contentOctets, bool disallowFractions)
		{
			bool flag = ruleSet == AsnEncodingRules.DER || ruleSet == AsnEncodingRules.CER;
			if (flag && contentOctets.Length < 15)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			if (contentOctets.Length < 10)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			ReadOnlySpan<byte> readOnlySpan = contentOctets;
			int num = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 4);
			int num2 = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2);
			int num3 = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2);
			int num4 = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2);
			int? num5 = null;
			int? num6 = null;
			ulong num7 = 0UL;
			ulong num8 = 1UL;
			byte b = byte.MaxValue;
			TimeSpan? timeSpan = null;
			bool flag2 = false;
			byte b2 = 0;
			while (b2 == 0 && readOnlySpan.Length != 0)
			{
				byte? b3 = AsnReader.ParseGeneralizedTime_GetNextState(*readOnlySpan[0]);
				if (b3 == null)
				{
					if (num5 == null)
					{
						num5 = new int?(AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2));
					}
					else
					{
						if (num6 != null)
						{
							throw new CryptographicException("ASN1 corrupted data.");
						}
						num6 = new int?(AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2));
					}
				}
				else
				{
					b2 = b3.Value;
				}
			}
			if (b2 == 1)
			{
				if (disallowFractions)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				byte b4 = *readOnlySpan[0];
				if (b4 != 46)
				{
					if (b4 != 44)
					{
						throw new CryptographicException();
					}
					if (flag)
					{
						throw new CryptographicException("ASN1 corrupted data.");
					}
				}
				readOnlySpan = readOnlySpan.Slice(1);
				if (readOnlySpan.IsEmpty)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				int num9;
				if (!Utf8Parser.TryParse(AsnReader.SliceAtMost(readOnlySpan, 12), out num7, out num9, '\0') || num9 == 0)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				b = (byte)(num7 % 10UL);
				for (int i = 0; i < num9; i++)
				{
					num8 *= 10UL;
				}
				readOnlySpan = readOnlySpan.Slice(num9);
				uint num10;
				while (Utf8Parser.TryParse(AsnReader.SliceAtMost(readOnlySpan, 9), out num10, out num9, '\0'))
				{
					readOnlySpan = readOnlySpan.Slice(num9);
					b = (byte)(num10 % 10U);
				}
				if (readOnlySpan.Length != 0)
				{
					byte? b5 = AsnReader.ParseGeneralizedTime_GetNextState(*readOnlySpan[0]);
					if (b5 == null)
					{
						throw new CryptographicException("ASN1 corrupted data.");
					}
					b2 = b5.Value;
				}
			}
			if (b2 == 2)
			{
				byte b6 = *readOnlySpan[0];
				readOnlySpan = readOnlySpan.Slice(1);
				if (b6 == 90)
				{
					timeSpan = new TimeSpan?(TimeSpan.Zero);
					flag2 = true;
				}
				else
				{
					bool flag3;
					if (b6 == 43)
					{
						flag3 = false;
					}
					else
					{
						if (b6 != 45)
						{
							throw new CryptographicException("ASN1 corrupted data.");
						}
						flag3 = true;
					}
					if (readOnlySpan.IsEmpty)
					{
						throw new CryptographicException("ASN1 corrupted data.");
					}
					int num11 = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2);
					int num12 = 0;
					if (readOnlySpan.Length != 0)
					{
						num12 = AsnReader.ParseNonNegativeIntAndSlice(ref readOnlySpan, 2);
					}
					if (num12 > 59)
					{
						throw new CryptographicException("ASN1 corrupted data.");
					}
					TimeSpan timeSpan2 = new TimeSpan(num11, num12, 0);
					if (flag3)
					{
						timeSpan2 = -timeSpan2;
					}
					timeSpan = new TimeSpan?(timeSpan2);
				}
			}
			if (!readOnlySpan.IsEmpty)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			if (flag)
			{
				if (!flag2 || num6 == null)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				if (b == 0)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
			}
			double num13 = num7 / num8;
			TimeSpan zero = TimeSpan.Zero;
			if (num5 == null)
			{
				num5 = new int?(0);
				num6 = new int?(0);
				if (num7 != 0UL)
				{
					zero = new TimeSpan((long)(num13 * 36000000000.0));
				}
			}
			else if (num6 == null)
			{
				num6 = new int?(0);
				if (num7 != 0UL)
				{
					zero = new TimeSpan((long)(num13 * 600000000.0));
				}
			}
			else if (num7 != 0UL)
			{
				zero = new TimeSpan((long)(num13 * 10000000.0));
			}
			DateTimeOffset dateTimeOffset2;
			try
			{
				DateTimeOffset dateTimeOffset;
				if (timeSpan == null)
				{
					dateTimeOffset = new DateTimeOffset(new DateTime(num, num2, num3, num4, num5.Value, num6.Value));
				}
				else
				{
					dateTimeOffset = new DateTimeOffset(num, num2, num3, num4, num5.Value, num6.Value, timeSpan.Value);
				}
				dateTimeOffset += zero;
				dateTimeOffset2 = dateTimeOffset;
			}
			catch (Exception ex)
			{
				throw new CryptographicException("ASN1 corrupted data.", ex);
			}
			return dateTimeOffset2;
		}

		public DateTimeOffset GetGeneralizedTime(bool disallowFractions = false)
		{
			return this.GetGeneralizedTime(Asn1Tag.GeneralizedTime, disallowFractions);
		}

		public DateTimeOffset GetGeneralizedTime(Asn1Tag expectedTag, bool disallowFractions = false)
		{
			byte[] array = null;
			int num;
			ReadOnlySpan<byte> octetStringContents = this.GetOctetStringContents(expectedTag, UniversalTagNumber.GeneralizedTime, out num, ref array, default(Span<byte>));
			DateTimeOffset dateTimeOffset = AsnReader.ParseGeneralizedTime(this._ruleSet, octetStringContents, disallowFractions);
			if (array != null)
			{
				Array.Clear(array, 0, octetStringContents.Length);
				ArrayPool<byte>.Shared.Return(array, false);
			}
			this._data = this._data.Slice(num);
			return dateTimeOffset;
		}

		private ReadOnlySpan<byte> GetOctetStringContents(Asn1Tag expectedTag, UniversalTagNumber universalTagNumber, out int bytesRead, ref byte[] rented, Span<byte> tmpSpace = default(Span<byte>))
		{
			Asn1Tag asn1Tag;
			int? num;
			int num2;
			ReadOnlyMemory<byte> readOnlyMemory;
			if (this.TryGetPrimitiveOctetStringBytes(expectedTag, out asn1Tag, out num, out num2, out readOnlyMemory, universalTagNumber))
			{
				bytesRead = num2 + readOnlyMemory.Length;
				return readOnlyMemory.Span;
			}
			ReadOnlyMemory<byte> readOnlyMemory2 = AsnReader.Slice(this._data, num2, num);
			bool flag = num == null;
			int num3 = this.CountConstructedOctetString(readOnlyMemory2, flag);
			if (tmpSpace.Length < num3)
			{
				rented = ArrayPool<byte>.Shared.Rent(num3);
				tmpSpace = rented;
			}
			int num4;
			int num5;
			this.CopyConstructedOctetString(readOnlyMemory2, tmpSpace, flag, out num4, out num5);
			bytesRead = num2 + num4;
			return tmpSpace.Slice(0, num5);
		}

		private static ReadOnlySpan<byte> SliceAtMost(ReadOnlySpan<byte> source, int longestPermitted)
		{
			int num = Math.Min(longestPermitted, source.Length);
			return source.Slice(0, num);
		}

		private static ReadOnlySpan<byte> Slice(ReadOnlySpan<byte> source, int offset, int length)
		{
			if (length < 0 || source.Length - offset < length)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			return source.Slice(offset, length);
		}

		private static ReadOnlyMemory<byte> Slice(ReadOnlyMemory<byte> source, int offset, int? length)
		{
			if (length == null)
			{
				return source.Slice(offset);
			}
			int value = length.Value;
			if (value < 0 || source.Length - offset < value)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			return source.Slice(offset, value);
		}

		private static void CheckEncodingRules(AsnEncodingRules ruleSet)
		{
			if (ruleSet != AsnEncodingRules.BER && ruleSet != AsnEncodingRules.CER && ruleSet != AsnEncodingRules.DER)
			{
				throw new ArgumentOutOfRangeException("ruleSet");
			}
		}

		private static void CheckExpectedTag(Asn1Tag tag, Asn1Tag expectedTag, UniversalTagNumber tagNumber)
		{
			if (expectedTag.TagClass == TagClass.Universal && expectedTag.TagValue != (int)tagNumber)
			{
				throw new ArgumentException("Tags with TagClass Universal must have the appropriate TagValue value for the data type being read or written.", "expectedTag");
			}
			if (expectedTag.TagClass != tag.TagClass || expectedTag.TagValue != tag.TagValue)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
		}

		private static void CheckCharacterStringEncodingType(UniversalTagNumber encodingType)
		{
			switch (encodingType)
			{
			case UniversalTagNumber.UTF8String:
			case UniversalTagNumber.NumericString:
			case UniversalTagNumber.PrintableString:
			case UniversalTagNumber.TeletexString:
			case UniversalTagNumber.VideotexString:
			case UniversalTagNumber.IA5String:
			case UniversalTagNumber.GraphicString:
			case UniversalTagNumber.VisibleString:
			case UniversalTagNumber.GeneralString:
			case UniversalTagNumber.UniversalString:
			case UniversalTagNumber.BMPString:
				return;
			}
			throw new ArgumentOutOfRangeException("encodingType");
		}

		internal const int MaxCERSegmentSize = 1000;

		private const int EndOfContentsEncodedLength = 2;

		private ReadOnlyMemory<byte> _data;

		private readonly AsnEncodingRules _ruleSet;

		private const byte HmsState = 0;

		private const byte FracState = 1;

		private const byte SuffixState = 2;

		private delegate void BitStringCopyAction(ReadOnlyMemory<byte> value, byte normalizedLastByte, Span<byte> destination);
	}
}
