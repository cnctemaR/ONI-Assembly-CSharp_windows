using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct EncapsulatedContentInfoAsn
	{
		[ObjectIdentifier]
		public string ContentType;

		[AnyValue]
		[ExpectedTag(0, ExplicitTag = true)]
		[OptionalValue]
		public ReadOnlyMemory<byte>? Content;
	}
}
