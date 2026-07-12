using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class EssCertId
	{
		[OctetString]
		public ReadOnlyMemory<byte> Hash;

		[OptionalValue]
		public CadesIssuerSerial? IssuerSerial;
	}
}
