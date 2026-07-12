using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct EncryptedContentInfoAsn
	{
		[ObjectIdentifier]
		internal string ContentType;

		internal AlgorithmIdentifierAsn ContentEncryptionAlgorithm;

		[ExpectedTag(0)]
		[OctetString]
		[OptionalValue]
		internal ReadOnlyMemory<byte>? EncryptedContent;
	}
}
