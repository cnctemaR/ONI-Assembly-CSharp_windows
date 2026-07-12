using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[Choice]
	internal struct OriginatorIdentifierOrKeyAsn
	{
		internal IssuerAndSerialNumberAsn? IssuerAndSerialNumber;

		[OctetString]
		[ExpectedTag(0)]
		internal ReadOnlyMemory<byte>? SubjectKeyIdentifier;

		[ExpectedTag(1)]
		internal OriginatorPublicKeyAsn OriginatorKey;
	}
}
