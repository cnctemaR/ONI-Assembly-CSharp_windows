using System;

namespace System.Security.Cryptography.X509Certificates
{
	internal interface INativeCertificateHelper
	{
		X509CertificateImpl Import(byte[] data, string password, X509KeyStorageFlags flags);

		X509CertificateImpl Import(X509Certificate cert);
	}
}
