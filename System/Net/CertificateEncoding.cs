using System;

namespace System.Net
{
	internal enum CertificateEncoding
	{
		Zero,
		X509AsnEncoding,
		X509NdrEncoding,
		Pkcs7AsnEncoding = 65536,
		Pkcs7NdrEncoding = 131072,
		AnyAsnEncoding = 65537
	}
}
