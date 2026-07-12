using System;

namespace System.Security.Cryptography.Asn1
{
	internal struct Asn1Tag : IEquatable<Asn1Tag>
	{
		public TagClass TagClass
		{
			get
			{
				return (TagClass)(this._controlFlags & 192);
			}
		}

		public bool IsConstructed
		{
			get
			{
				return (this._controlFlags & 32) > 0;
			}
		}

		public int TagValue
		{
			get
			{
				return this._tagValue;
			}
		}

		private Asn1Tag(byte controlFlags, int tagValue)
		{
			this._controlFlags = controlFlags & 224;
			this._tagValue = tagValue;
		}

		public Asn1Tag(UniversalTagNumber universalTagNumber, bool isConstructed = false)
		{
			this = new Asn1Tag(isConstructed ? 32 : 0, (int)universalTagNumber);
			if (universalTagNumber < UniversalTagNumber.EndOfContents || universalTagNumber > UniversalTagNumber.RelativeObjectIdentifierIRI || universalTagNumber == (UniversalTagNumber)15)
			{
				throw new ArgumentOutOfRangeException("universalTagNumber");
			}
		}

		public Asn1Tag(TagClass tagClass, int tagValue, bool isConstructed = false)
		{
			this = new Asn1Tag((byte)(tagClass | (isConstructed ? ((TagClass)32) : TagClass.Universal)), tagValue);
			if (tagClass < TagClass.Universal || tagClass > TagClass.Private)
			{
				throw new ArgumentOutOfRangeException("tagClass");
			}
			if (tagValue < 0)
			{
				throw new ArgumentOutOfRangeException("tagValue");
			}
		}

		public Asn1Tag AsConstructed()
		{
			return new Asn1Tag(this._controlFlags | 32, this._tagValue);
		}

		public Asn1Tag AsPrimitive()
		{
			return new Asn1Tag((byte)((int)this._controlFlags & -33), this._tagValue);
		}

		public unsafe static bool TryParse(ReadOnlySpan<byte> source, out Asn1Tag tag, out int bytesRead)
		{
			tag = default(Asn1Tag);
			bytesRead = 0;
			if (source.IsEmpty)
			{
				return false;
			}
			byte b = *source[bytesRead];
			bytesRead++;
			uint num = (uint)(b & 31);
			if (num == 31U)
			{
				num = 0U;
				while (source.Length > bytesRead)
				{
					byte b2 = *source[bytesRead];
					byte b3 = b2 & 127;
					bytesRead++;
					if (num >= 33554432U)
					{
						bytesRead = 0;
						return false;
					}
					num <<= 7;
					num |= (uint)b3;
					if (num == 0U)
					{
						bytesRead = 0;
						return false;
					}
					if ((b2 & 128) != 128)
					{
						if (num <= 30U)
						{
							bytesRead = 0;
							return false;
						}
						if (num > 2147483647U)
						{
							bytesRead = 0;
							return false;
						}
						goto IL_009B;
					}
				}
				bytesRead = 0;
				return false;
			}
			IL_009B:
			tag = new Asn1Tag(b, (int)num);
			return true;
		}

		public int CalculateEncodedSize()
		{
			if (this.TagValue < 31)
			{
				return 1;
			}
			if (this.TagValue <= 127)
			{
				return 2;
			}
			if (this.TagValue <= 16383)
			{
				return 3;
			}
			if (this.TagValue <= 2097151)
			{
				return 4;
			}
			if (this.TagValue <= 268435455)
			{
				return 5;
			}
			return 6;
		}

		public unsafe bool TryWrite(Span<byte> destination, out int bytesWritten)
		{
			int num = this.CalculateEncodedSize();
			if (destination.Length < num)
			{
				bytesWritten = 0;
				return false;
			}
			if (num == 1)
			{
				byte b = (byte)((int)this._controlFlags | this.TagValue);
				*destination[0] = b;
				bytesWritten = 1;
				return true;
			}
			byte b2 = this._controlFlags | 31;
			*destination[0] = b2;
			int i = this.TagValue;
			int num2 = num - 1;
			while (i > 0)
			{
				int num3 = i & 127;
				if (i != this.TagValue)
				{
					num3 |= 128;
				}
				*destination[num2] = (byte)num3;
				i >>= 7;
				num2--;
			}
			bytesWritten = num;
			return true;
		}

		public bool Equals(Asn1Tag other)
		{
			return this._controlFlags == other._controlFlags && this._tagValue == other._tagValue;
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Asn1Tag && this.Equals((Asn1Tag)obj);
		}

		public override int GetHashCode()
		{
			return ((int)this._controlFlags << 24) ^ this._tagValue;
		}

		public static bool operator ==(Asn1Tag left, Asn1Tag right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Asn1Tag left, Asn1Tag right)
		{
			return !left.Equals(right);
		}

		public override string ToString()
		{
			string text;
			if (this.TagClass == TagClass.Universal)
			{
				text = ((UniversalTagNumber)this.TagValue).ToString();
			}
			else
			{
				text = this.TagClass.ToString() + "-" + this.TagValue.ToString();
			}
			if (this.IsConstructed)
			{
				return "Constructed " + text;
			}
			return text;
		}

		private const byte ClassMask = 192;

		private const byte ConstructedMask = 32;

		private const byte ControlMask = 224;

		private const byte TagNumberMask = 31;

		internal static readonly Asn1Tag EndOfContents = new Asn1Tag(0, 0);

		internal static readonly Asn1Tag Boolean = new Asn1Tag(0, 1);

		internal static readonly Asn1Tag Integer = new Asn1Tag(0, 2);

		internal static readonly Asn1Tag PrimitiveBitString = new Asn1Tag(0, 3);

		internal static readonly Asn1Tag ConstructedBitString = new Asn1Tag(32, 3);

		internal static readonly Asn1Tag PrimitiveOctetString = new Asn1Tag(0, 4);

		internal static readonly Asn1Tag ConstructedOctetString = new Asn1Tag(32, 4);

		internal static readonly Asn1Tag Null = new Asn1Tag(0, 5);

		internal static readonly Asn1Tag ObjectIdentifier = new Asn1Tag(0, 6);

		internal static readonly Asn1Tag Enumerated = new Asn1Tag(0, 10);

		internal static readonly Asn1Tag Sequence = new Asn1Tag(32, 16);

		internal static readonly Asn1Tag SetOf = new Asn1Tag(32, 17);

		internal static readonly Asn1Tag UtcTime = new Asn1Tag(0, 23);

		internal static readonly Asn1Tag GeneralizedTime = new Asn1Tag(0, 24);

		private readonly byte _controlFlags;

		private readonly int _tagValue;
	}
}
