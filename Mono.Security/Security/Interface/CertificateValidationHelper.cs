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

		public static ICertificateValidator GetValidator(MonoTlsSettings settings)
		{
			return (ICertificateValidator)NoReflectionHelper.GetDefaultValidator(settings);
		}

		private const string SecurityLibrary = "/System/Library/Frameworks/Security.framework/Security";

		private static readonly bool noX509Chain;

		private static readonly bool supportsTrustAnchors;
	}
}
