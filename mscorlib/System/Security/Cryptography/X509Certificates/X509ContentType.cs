using System;

namespace System.Security.Cryptography.X509Certificates
{
	public enum X509ContentType
	{
		Unknown,
		Cert,
		SerializedCert,
		Pfx,
		Pkcs12 = 3,
		SerializedStore,
		Pkcs7,
		Authenticode
	}
}
