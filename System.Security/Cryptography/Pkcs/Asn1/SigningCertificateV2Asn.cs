using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct SigningCertificateV2Asn
	{
		public EssCertIdV2[] Certs;

		[OptionalValue]
		public PolicyInformation[] Policies;
	}
}
