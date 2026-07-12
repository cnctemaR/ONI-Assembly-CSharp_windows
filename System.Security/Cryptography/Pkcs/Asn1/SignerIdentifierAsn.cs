using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[Choice]
	internal struct SignerIdentifierAsn
	{
		public IssuerAndSerialNumberAsn? IssuerAndSerialNumber;

		[OctetString]
		[ExpectedTag(0)]
		public ReadOnlyMemory<byte>? SubjectKeyIdentifier;
	}
}
