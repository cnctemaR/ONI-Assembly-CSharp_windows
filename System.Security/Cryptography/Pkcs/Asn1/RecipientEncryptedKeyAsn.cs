using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct RecipientEncryptedKeyAsn
	{
		internal KeyAgreeRecipientIdentifierAsn Rid;

		[OctetString]
		internal ReadOnlyMemory<byte> EncryptedKey;
	}
}
