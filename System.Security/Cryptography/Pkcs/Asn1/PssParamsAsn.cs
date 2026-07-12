using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct PssParamsAsn
	{
		[ExpectedTag(0, ExplicitTag = true)]
		[DefaultValue(new byte[]
		{
			160, 9, 48, 7, 6, 5, 43, 14, 3, 2,
			26
		})]
		public AlgorithmIdentifierAsn HashAlgorithm;

		[ExpectedTag(1, ExplicitTag = true)]
		[DefaultValue(new byte[]
		{
			161, 22, 48, 20, 6, 9, 42, 134, 72, 134,
			247, 13, 1, 1, 8, 48, 9, 6, 5, 43,
			14, 3, 2, 26
		})]
		public AlgorithmIdentifierAsn MaskGenAlgorithm;

		[ExpectedTag(2, ExplicitTag = true)]
		[DefaultValue(new byte[] { 162, 3, 2, 1, 20 })]
		public int SaltLength;

		[ExpectedTag(3, ExplicitTag = true)]
		[DefaultValue(new byte[] { 163, 3, 2, 1, 1 })]
		public int TrailerField;
	}
}
