using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class KeyTransRecipientInfoAsn
	{
		internal int Version;

		internal RecipientIdentifierAsn Rid;

		internal AlgorithmIdentifierAsn KeyEncryptionAlgorithm;

		[OctetString]
		internal ReadOnlyMemory<byte> EncryptedKey;
	}
}
