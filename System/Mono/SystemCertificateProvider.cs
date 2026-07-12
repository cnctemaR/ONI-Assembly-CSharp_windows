using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using Mono.Btls;
using Mono.Net.Security;
using Mono.Security.Interface;

namespace Mono
{
	internal class SystemCertificateProvider : ISystemCertificateProvider
	{
		public MonoTlsProvider Provider
		{
			get
			{
				SystemCertificateProvider.EnsureInitialized();
				return SystemCertificateProvider.provider;
			}
		}

		private static X509PalImpl GetX509Pal()
		{
			MonoTlsProvider monoTlsProvider = SystemCertificateProvider.provider;
			Guid? guid = ((monoTlsProvider != null) ? new Guid?(monoTlsProvider.ID) : null);
			Guid btlsId = Mono.Net.Security.MonoTlsProviderFactory.BtlsId;
			if (guid != null && (guid == null || guid.GetValueOrDefault() == btlsId))
			{
				return new X509PalImplBtls(SystemCertificateProvider.provider);
			}
			return new X509PalImplMono();
		}

		private static void EnsureInitialized()
		{
			object obj = SystemCertificateProvider.syncRoot;
			lock (obj)
			{
				if (Interlocked.CompareExchange(ref SystemCertificateProvider.initialized, 1, 0) == 0)
				{
					SystemCertificateProvider.provider = Mono.Security.Interface.MonoTlsProviderFactory.GetProvider();
					SystemCertificateProvider.x509pal = SystemCertificateProvider.GetX509Pal();
				}
			}
		}

		public X509PalImpl X509Pal
		{
			get
			{
				SystemCertificateProvider.EnsureInitialized();
				return SystemCertificateProvider.x509pal;
			}
		}

		public X509CertificateImpl Import(byte[] data, CertificateImportFlags importFlags = CertificateImportFlags.None)
		{
			if (data == null || data.Length == 0)
			{
				return null;
			}
			if ((importFlags & CertificateImportFlags.DisableNativeBackend) == CertificateImportFlags.None)
			{
				X509CertificateImpl x509CertificateImpl = this.X509Pal.Import(data);
				if (x509CertificateImpl != null)
				{
					return x509CertificateImpl;
				}
			}
			if ((importFlags & CertificateImportFlags.DisableAutomaticFallback) != CertificateImportFlags.None)
			{
				return null;
			}
			return this.X509Pal.ImportFallback(data);
		}

		X509CertificateImpl ISystemCertificateProvider.Import(byte[] data, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags, CertificateImportFlags importFlags)
		{
			return this.Import(data, password, keyStorageFlags, importFlags);
		}

		public X509Certificate2Impl Import(byte[] data, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags, CertificateImportFlags importFlags = CertificateImportFlags.None)
		{
			if (data == null || data.Length == 0)
			{
				return null;
			}
			if ((importFlags & CertificateImportFlags.DisableNativeBackend) == CertificateImportFlags.None)
			{
				X509Certificate2Impl x509Certificate2Impl = this.X509Pal.Import(data, password, keyStorageFlags);
				if (x509Certificate2Impl != null)
				{
					return x509Certificate2Impl;
				}
			}
			if ((importFlags & CertificateImportFlags.DisableAutomaticFallback) != CertificateImportFlags.None)
			{
				return null;
			}
			return this.X509Pal.ImportFallback(data, password, keyStorageFlags);
		}

		X509CertificateImpl ISystemCertificateProvider.Import(X509Certificate cert, CertificateImportFlags importFlags)
		{
			return this.Import(cert, importFlags);
		}

		public X509Certificate2Impl Import(X509Certificate cert, CertificateImportFlags importFlags = CertificateImportFlags.None)
		{
			if (cert.Impl == null)
			{
				return null;
			}
			X509Certificate2Impl x509Certificate2Impl = cert.Impl as X509Certificate2Impl;
			if (x509Certificate2Impl != null)
			{
				return (X509Certificate2Impl)x509Certificate2Impl.Clone();
			}
			if ((importFlags & CertificateImportFlags.DisableNativeBackend) == CertificateImportFlags.None)
			{
				x509Certificate2Impl = this.X509Pal.Import(cert);
				if (x509Certificate2Impl != null)
				{
					return x509Certificate2Impl;
				}
			}
			if ((importFlags & CertificateImportFlags.DisableAutomaticFallback) != CertificateImportFlags.None)
			{
				return null;
			}
			return this.X509Pal.ImportFallback(cert.GetRawCertData());
		}

		private static MonoTlsProvider provider;

		private static int initialized;

		private static X509PalImpl x509pal;

		private static object syncRoot = new object();
	}
}
