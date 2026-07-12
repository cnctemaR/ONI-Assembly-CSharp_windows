using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct Rfc3161TimeStampReq
	{
		public int Version;

		public MessageImprint MessageImprint;

		[OptionalValue]
		public Oid ReqPolicy;

		[Integer]
		[OptionalValue]
		public ReadOnlyMemory<byte>? Nonce;

		[DefaultValue(new byte[] { 1, 1, 0 })]
		public bool CertReq;

		[ExpectedTag(0)]
		[OptionalValue]
		internal X509ExtensionAsn[] Extensions;
	}
}
