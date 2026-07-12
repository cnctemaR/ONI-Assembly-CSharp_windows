using System;
using System.IO;
using Mono.Btls;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	internal static class X509Helper2
	{
		[MonoTODO("Investigate replacement; see comments in source.")]
		internal static X509Certificate GetMonoCertificate(X509Certificate2 certificate)
		{
			X509Certificate2ImplMono x509Certificate2ImplMono = certificate.Impl as X509Certificate2ImplMono;
			if (x509Certificate2ImplMono != null)
			{
				return x509Certificate2ImplMono.MonoCertificate;
			}
			return new X509Certificate(certificate.RawData);
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

		[Obsolete("This is only used by Mono.Security's X509Store and will be replaced shortly.")]
		internal static long GetSubjectNameHash(X509Certificate certificate)
		{
			X509Helper.ThrowIfContextInvalid(certificate.Impl);
			long hash;
			using (MonoBtlsX509 nativeInstance = X509Helper2.GetNativeInstance(certificate.Impl))
			{
				using (MonoBtlsX509Name subjectName = nativeInstance.GetSubjectName())
				{
					hash = subjectName.GetHash();
				}
			}
			return hash;
		}

		[Obsolete("This is only used by Mono.Security's X509Store and will be replaced shortly.")]
		internal static void ExportAsPEM(X509Certificate certificate, Stream stream, bool includeHumanReadableForm)
		{
			X509Helper.ThrowIfContextInvalid(certificate.Impl);
			using (MonoBtlsX509 nativeInstance = X509Helper2.GetNativeInstance(certificate.Impl))
			{
				using (MonoBtlsBio monoBtlsBio = MonoBtlsBio.CreateMonoStream(stream))
				{
					nativeInstance.ExportAsPEM(monoBtlsBio, includeHumanReadableForm);
				}
			}
		}

		private static MonoBtlsX509 GetNativeInstance(X509CertificateImpl impl)
		{
			X509Helper.ThrowIfContextInvalid(impl);
			X509CertificateImplBtls x509CertificateImplBtls = impl as X509CertificateImplBtls;
			if (x509CertificateImplBtls != null)
			{
				return x509CertificateImplBtls.X509.Copy();
			}
			return MonoBtlsX509.LoadFromData(impl.RawData, MonoBtlsX509Format.DER);
		}
	}
}
