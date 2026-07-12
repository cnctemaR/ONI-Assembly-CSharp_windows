using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct SignedDataAsn
	{
		public int Version;

		[SetOf]
		public AlgorithmIdentifierAsn[] DigestAlgorithms;

		public EncapsulatedContentInfoAsn EncapContentInfo;

		[ExpectedTag(0)]
		[SetOf]
		[OptionalValue]
		public CertificateChoiceAsn[] CertificateSet;

		[AnyValue]
		[ExpectedTag(1)]
		[OptionalValue]
		public ReadOnlyMemory<byte>? RevocationInfoChoices;

		[SetOf]
		public SignerInfoAsn[] SignerInfos;
	}
}
