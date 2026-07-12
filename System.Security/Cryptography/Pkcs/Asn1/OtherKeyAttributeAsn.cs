using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct OtherKeyAttributeAsn
	{
		[ObjectIdentifier]
		internal string KeyAttrId;

		[OptionalValue]
		[AnyValue]
		internal ReadOnlyMemory<byte>? KeyAttr;
	}
}
