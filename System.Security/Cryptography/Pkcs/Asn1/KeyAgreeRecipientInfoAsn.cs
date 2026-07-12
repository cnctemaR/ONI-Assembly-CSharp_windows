using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class KeyAgreeRecipientInfoAsn
	{
		internal int Version;

		[ExpectedTag(0, ExplicitTag = true)]
		internal OriginatorIdentifierOrKeyAsn Originator;

		[ExpectedTag(1, ExplicitTag = true)]
		[OctetString]
		[OptionalValue]
		internal ReadOnlyMemory<byte>? Ukm;

		internal AlgorithmIdentifierAsn KeyEncryptionAlgorithm;

		internal RecipientEncryptedKeyAsn[] RecipientEncryptedKeys;
	}
}
