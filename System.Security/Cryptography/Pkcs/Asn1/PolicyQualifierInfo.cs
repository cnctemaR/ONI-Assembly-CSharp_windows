using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct PolicyQualifierInfo
	{
		[ObjectIdentifier]
		public string PolicyQualifierId;

		[AnyValue]
		public ReadOnlyMemory<byte> Qualifier;
	}
}
