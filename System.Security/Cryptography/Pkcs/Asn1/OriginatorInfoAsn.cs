using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class OriginatorInfoAsn
	{
		[OptionalValue]
		[ExpectedTag(0)]
		[SetOf]
		public CertificateChoiceAsn[] CertificateSet;

		[OptionalValue]
		[ExpectedTag(1)]
		[AnyValue]
		public ReadOnlyMemory<byte>? RevocationInfoChoices;
	}
}
