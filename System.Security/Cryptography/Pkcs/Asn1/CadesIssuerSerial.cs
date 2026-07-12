using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct CadesIssuerSerial
	{
		public GeneralName[] Issuer;

		[Integer]
		public ReadOnlyMemory<byte> SerialNumber;
	}
}
