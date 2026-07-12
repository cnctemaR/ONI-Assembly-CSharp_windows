using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct IssuerAndSerialNumberAsn
	{
		[AnyValue]
		public ReadOnlyMemory<byte> Issuer;

		[Integer]
		public ReadOnlyMemory<byte> SerialNumber;
	}
}
