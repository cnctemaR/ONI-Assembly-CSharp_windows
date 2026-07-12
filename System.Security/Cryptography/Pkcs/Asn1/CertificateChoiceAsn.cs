using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[Choice]
	internal struct CertificateChoiceAsn
	{
		[ExpectedTag(TagClass.Universal, 16)]
		[AnyValue]
		public ReadOnlyMemory<byte>? Certificate;
	}
}
