using System;
using Unity;

namespace System.Security.Cryptography.X509Certificates
{
	public static class DSACertificateExtensions
	{
		[SecuritySafeCritical]
		public static DSA GetDSAPrivateKey(this X509Certificate2 certificate)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[SecuritySafeCritical]
		public static DSA GetDSAPublicKey(this X509Certificate2 certificate)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
