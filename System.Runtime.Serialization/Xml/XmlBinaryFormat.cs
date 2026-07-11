using System;

namespace System.Xml
{
	internal class XmlBinaryFormat
	{
		public const byte EndElement = 1;

		public const byte Comment = 2;

		public const byte Array = 3;

		public const byte AttrString = 4;

		public const byte AttrStringPrefix = 5;

		public const byte AttrIndex = 6;

		public const byte AttrIndexPrefix = 7;

		public const byte DefaultNSString = 8;

		public const byte PrefixNSString = 9;

		public const byte DefaultNSIndex = 10;

		public const byte PrefixNSIndex = 11;

		public const byte PrefixNAttrIndexStart = 12;

		public const byte PrefixNAttrIndexEnd = 37;

		public const byte PrefixNAttrStringStart = 38;

		public const byte PrefixNAttrStringEnd = 63;

		public const byte ElemString = 64;

		public const byte ElemStringPrefix = 65;

		public const byte ElemIndex = 66;

		public const byte ElemIndexPrefix = 67;

		public const byte PrefixNElemIndexStart = 68;

		public const byte PrefixNElemIndexEnd = 93;

		public const byte PrefixNElemStringStart = 94;

		public const byte PrefixNElemStringEnd = 119;

		public const byte Zero = 128;

		public const byte One = 130;

		public const byte BoolFalse = 132;

		public const byte BoolTrue = 134;

		public const byte Int8 = 136;

		public const byte Int16 = 138;

		public const byte Int32 = 140;

		public const byte Int64 = 142;

		public const byte Single = 144;

		public const byte Double = 146;

		public const byte Decimal = 148;

		public const byte DateTime = 150;

		public const byte Chars8 = 152;

		public const byte Chars16 = 154;

		public const byte Chars32 = 156;

		public const byte Bytes8 = 158;

		public const byte Bytes16 = 160;

		public const byte Bytes32 = 162;

		public const byte EmptyText = 168;

		public const byte TextIndex = 170;

		public const byte UniqueId = 172;

		public const byte TimeSpan = 174;

		public const byte Guid = 176;

		public const byte UInt64 = 178;

		public const byte Bool = 180;

		public const byte Utf16_8 = 182;

		public const byte Utf16_16 = 184;

		public const byte Utf16_32 = 186;

		public const byte QNameIndex = 188;
	}
}
