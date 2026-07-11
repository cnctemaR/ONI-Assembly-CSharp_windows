using System;
using System.IO;
using Mono.Net.Security;

namespace Mono.Security.Interface
{
	public static class CertificateValidationHelper
	{
		static CertificateValidationHelper()
		{
			if (File.Exists("/System/Library/Frameworks/Security.framework/Security"))
			{
				CertificateValidationHelper.noX509Chain = true;
				CertificateValidationHelper.supportsTrustAnchors = true;
				return;
			}
			CertificateValidationHelper.noX509Chain = false;
			CertificateValidationHelper.supportsTrustAnchors = false;
		}

		public static bool SupportsX509Chain
		{
			get
			{
				return !CertificateValidationHelper.noX509Chain;
			}
		}

		public static bool SupportsTrustAnchors
		{
			get
			{
				return CertificateValidationHelper.supportsTrustAnchors;
			}
		}

		internal static ICertificateValidator2 GetInternalValidator(MonoTlsSettings settings, MonoTlsProvider provider)
		{
			return (ICertificateValidator2)NoReflectionHelper.GetInternalValidator(provider, settings);
		}

		[Obsolete("Use GetInternalValidator")]
		internal static ICertificateValidator2 GetDefaultValidator(MonoTlsSettings settings, MonoTlsProvider provider)
		{
			return CertificateValidationHelper.GetInternalValidator(settings, provider);
		}

		public static ICertificateValidator GetValidator(MonoTlsSettings settings)
		{
			return (ICertificateValidator)NoReflectionHelper.GetDefaultValidator(settings);
		}

		private const string SecurityLibrary = "/System/Library/Frameworks/Security.framework/Security";

		private static readonly bool noX509Chain;

		private static readonly bool supportsTrustAnchors;
	}
}
