using System;

namespace System.Security.Cryptography.X509Certificates
{
	public static class RSACertificateExtensions
	{
		public static RSA GetRSAPrivateKey(this X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			return certificate.PrivateKey as RSA;
		}

		public static RSA GetRSAPublicKey(this X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			return certificate.PublicKey.Key as RSA;
		}
	}
}
