using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[Choice]
	internal struct DirectoryString
	{
		[ExpectedTag(TagClass.Universal, 20)]
		internal ReadOnlyMemory<byte>? TeletexString;

		[PrintableString]
		internal string PrintableString;

		[ExpectedTag(TagClass.Universal, 28)]
		internal ReadOnlyMemory<byte>? UniversalString;

		[UTF8String]
		internal string Utf8String;

		[BMPString]
		internal string BMPString;
	}
}
