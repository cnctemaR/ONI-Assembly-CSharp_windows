using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct PolicyInformation
	{
		[ObjectIdentifier]
		public string PolicyIdentifier;

		[OptionalValue]
		public PolicyQualifierInfo[] PolicyQualifiers;
	}
}
