using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct EnvelopedDataAsn
	{
		public int Version;

		[OptionalValue]
		[ExpectedTag(0)]
		public OriginatorInfoAsn OriginatorInfo;

		[SetOf]
		public RecipientInfoAsn[] RecipientInfos;

		public EncryptedContentInfoAsn EncryptedContentInfo;

		[OptionalValue]
		[ExpectedTag(1)]
		[SetOf]
		public AttributeAsn[] UnprotectedAttributes;
	}
}
