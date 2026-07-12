using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[Choice]
	internal struct SignedAttributesSet
	{
		[SetOf]
		[ExpectedTag(0)]
		public AttributeAsn[] SignedAttributes;
	}
}
