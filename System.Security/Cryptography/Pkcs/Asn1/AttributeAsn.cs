using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct AttributeAsn
	{
		public Oid AttrType;

		[AnyValue]
		public ReadOnlyMemory<byte> AttrValues;
	}
}
