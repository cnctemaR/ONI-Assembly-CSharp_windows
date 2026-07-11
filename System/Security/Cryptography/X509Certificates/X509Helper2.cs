using System;
using System.IO;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	internal static class X509Helper2
	{
		internal static long GetSubjectNameHash(global::System.Security.Cryptography.X509Certificates.X509Certificate certificate)
		{
			return X509Helper2.GetSubjectNameHash(certificate.Impl);
		}

		internal static long GetSubjectNameHash(X509CertificateImpl impl)
		{
			long subjectNameHash;
			using (global::System.Security.Cryptography.X509Certificates.X509Certificate nativeInstance = X509Helper2.GetNativeInstance(impl))
			{
				subjectNameHash = X509Helper2.GetSubjectNameHash(nativeInstance);
			}
			return subjectNameHash;
		}

		internal static void ExportAsPEM(global::System.Security.Cryptography.X509Certificates.X509Certificate certificate, Stream stream, bool includeHumanReadableForm)
		{
			X509Helper2.ExportAsPEM(certificate.Impl, stream, includeHumanReadableForm);
		}

		internal static void ExportAsPEM(X509CertificateImpl impl, Stream stream, bool includeHumanReadableForm)
		{
			using (global::System.Security.Cryptography.X509Certificates.X509Certificate nativeInstance = X509Helper2.GetNativeInstance(impl))
			{
				X509Helper2.ExportAsPEM(nativeInstance, stream, includeHumanReadableForm);
			}
		}

		internal static void Initialize()
		{
			X509Helper.InstallNativeHelper(new X509Helper2.MyNativeHelper());
		}

		internal static void ThrowIfContextInvalid(X509CertificateImpl impl)
		{
			X509Helper.ThrowIfContextInvalid(impl);
		}

		private static global::System.Security.Cryptography.X509Certificates.X509Certificate GetNativeInstance(X509CertificateImpl impl)
		{
			throw new PlatformNotSupportedException();
		}

		internal static X509Certificate2Impl Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags, bool disableProvider = false)
		{
			X509Certificate2ImplMono x509Certificate2ImplMono = new X509Certificate2ImplMono();
			x509Certificate2ImplMono.Import(rawData, password, keyStorageFlags);
			return x509Certificate2ImplMono;
		}

		internal static X509Certificate2Impl Import(global::System.Security.Cryptography.X509Certificates.X509Certificate cert, bool disableProvider = false)
		{
			X509Certificate2Impl x509Certificate2Impl = cert.Impl as X509Certificate2Impl;
			if (x509Certificate2Impl != null)
			{
				return (X509Certificate2Impl)x509Certificate2Impl.Clone();
			}
			return X509Helper2.Import(cert.GetRawCertData(), null, X509KeyStorageFlags.DefaultKeySet, false);
		}

		[MonoTODO("Investigate replacement; see comments in source.")]
		internal static Mono.Security.X509.X509Certificate GetMonoCertificate(X509Certificate2 certificate)
		{
			X509Certificate2Impl x509Certificate2Impl = certificate.Impl;
			if (x509Certificate2Impl == null)
			{
				x509Certificate2Impl = X509Helper2.Import(certificate, true);
			}
			X509Certificate2ImplMono x509Certificate2ImplMono = x509Certificate2Impl.FallbackImpl as X509Certificate2ImplMono;
			if (x509Certificate2ImplMono == null)
			{
				throw new NotSupportedException();
			}
			return x509Certificate2ImplMono.MonoCertificate;
		}

		internal static X509ChainImpl CreateChainImpl(bool useMachineContext)
		{
			return new X509ChainImplMono(useMachineContext);
		}

		public static bool IsValid(X509ChainImpl impl)
		{
			return impl != null && impl.IsValid;
		}

		internal static void ThrowIfContextInvalid(X509ChainImpl impl)
		{
			if (!X509Helper2.IsValid(impl))
			{
				throw X509Helper2.GetInvalidChainContextException();
			}
		}

		internal static Exception GetInvalidChainContextException()
		{
			return new CryptographicException(global::Locale.GetText("Chain instance is empty."));
		}

		private class MyNativeHelper : INativeCertificateHelper
		{
			public X509CertificateImpl Import(byte[] data, string password, X509KeyStorageFlags flags)
			{
				return X509Helper2.Import(data, password, flags, false);
			}

			public X509CertificateImpl Import(global::System.Security.Cryptography.X509Certificates.X509Certificate cert)
			{
				return X509Helper2.Import(cert, false);
			}
		}
	}
}
