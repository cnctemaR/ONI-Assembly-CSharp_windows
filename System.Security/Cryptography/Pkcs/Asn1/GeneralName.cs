using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[Choice]
	internal struct GeneralName
	{
		[ExpectedTag(0, ExplicitTag = true)]
		internal OtherName? OtherName;

		[IA5String]
		[ExpectedTag(1, ExplicitTag = true)]
		internal string Rfc822Name;

		[IA5String]
		[ExpectedTag(2, ExplicitTag = true)]
		internal string DnsName;

		[AnyValue]
		[ExpectedTag(3, ExplicitTag = true)]
		internal ReadOnlyMemory<byte>? X400Address;

		[AnyValue]
		[ExpectedTag(4, ExplicitTag = true)]
		internal ReadOnlyMemory<byte>? DirectoryName;

		[ExpectedTag(5, ExplicitTag = true)]
		internal EdiPartyName? EdiPartyName;

		[IA5String]
		[ExpectedTag(6, ExplicitTag = true)]
		internal string Uri;

		[OctetString]
		[ExpectedTag(7, ExplicitTag = true)]
		internal ReadOnlyMemory<byte>? IPAddress;

		[ExpectedTag(8, ExplicitTag = true)]
		[ObjectIdentifier]
		internal string RegisteredId;
	}
}
