using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[Choice]
	internal struct RecipientIdentifierAsn
	{
		internal IssuerAndSerialNumberAsn? IssuerAndSerialNumber;

		[ExpectedTag(0)]
		[OctetString]
		internal ReadOnlyMemory<byte>? SubjectKeyIdentifier;
	}
}
