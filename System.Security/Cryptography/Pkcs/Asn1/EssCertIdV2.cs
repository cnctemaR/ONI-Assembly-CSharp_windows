using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class EssCertIdV2
	{
		[DefaultValue(new byte[]
		{
			48, 11, 6, 9, 96, 134, 72, 1, 101, 3,
			4, 2, 1
		})]
		public AlgorithmIdentifierAsn HashAlgorithm;

		[OctetString]
		public ReadOnlyMemory<byte> Hash;

		[OptionalValue]
		public CadesIssuerSerial? IssuerSerial;
	}
}
