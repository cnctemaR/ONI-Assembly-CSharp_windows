using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct OtherName
	{
		internal string TypeId;

		[ExpectedTag(0, ExplicitTag = true)]
		[AnyValue]
		internal ReadOnlyMemory<byte> Value;
	}
}
