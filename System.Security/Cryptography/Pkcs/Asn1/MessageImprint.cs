using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct MessageImprint
	{
		internal AlgorithmIdentifierAsn HashAlgorithm;

		[OctetString]
		internal ReadOnlyMemory<byte> HashedMessage;
	}
}
