using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;
using Mono.Security.Interface;

namespace Mono.Btls
{
	internal class X509PalImplBtls : X509PalImpl
	{
		public X509PalImplBtls(MonoTlsProvider provider)
		{
			this.Provider = (MonoBtlsProvider)provider;
		}

		private MonoBtlsProvider Provider { get; }

		public override X509CertificateImpl Import(byte[] data)
		{
			return this.Provider.GetNativeCertificate(data, null, X509KeyStorageFlags.DefaultKeySet);
		}

		public override X509Certificate2Impl Import(byte[] data, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags)
		{
			return this.Provider.GetNativeCertificate(data, password, keyStorageFlags);
		}

		public override X509Certificate2Impl Import(X509Certificate cert)
		{
			return this.Provider.GetNativeCertificate(cert);
		}
	}
}
