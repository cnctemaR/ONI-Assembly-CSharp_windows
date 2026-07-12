using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class OriginatorPublicKeyAsn
	{
		internal AlgorithmIdentifierAsn Algorithm;

		[BitString]
		internal ReadOnlyMemory<byte> PublicKey;
	}
}
