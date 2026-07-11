using System;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Interface;

namespace Mono.AppleTls
{
	internal static class AppleCertificateHelper
	{
		public static SecIdentity GetIdentity(X509Certificate certificate)
		{
			X509Certificate2 x509Certificate = certificate as X509Certificate2;
			if (x509Certificate != null)
			{
				return SecImportExport.ItemImport(x509Certificate);
			}
			return null;
		}

		public static SecIdentity GetIdentity(X509Certificate certificate, out SecCertificate[] intermediateCerts)
		{
			SecIdentity identity = AppleCertificateHelper.GetIdentity(certificate);
			X509Certificate2Impl x509Certificate2Impl = certificate.Impl as X509Certificate2Impl;
			if (x509Certificate2Impl == null || x509Certificate2Impl.IntermediateCertificates == null)
			{
				intermediateCerts = new SecCertificate[0];
				return identity;
			}
			SecIdentity secIdentity;
			try
			{
				intermediateCerts = new SecCertificate[x509Certificate2Impl.IntermediateCertificates.Count];
				for (int i = 0; i < intermediateCerts.Length; i++)
				{
					intermediateCerts[i] = new SecCertificate(x509Certificate2Impl.IntermediateCertificates[i]);
				}
				secIdentity = identity;
			}
			catch
			{
				identity.Dispose();
				throw;
			}
			return secIdentity;
		}

		public static bool InvokeSystemCertificateValidator(ICertificateValidator2 validator, string targetHost, bool serverMode, X509CertificateCollection certificates, ref MonoSslPolicyErrors errors, ref int status11)
		{
			if (certificates == null)
			{
				errors |= MonoSslPolicyErrors.RemoteCertificateNotAvailable;
				return false;
			}
			if (!string.IsNullOrEmpty(targetHost))
			{
				int num = targetHost.IndexOf(':');
				if (num > 0)
				{
					targetHost = targetHost.Substring(0, num);
				}
			}
			bool flag;
			using (SecPolicy secPolicy = SecPolicy.CreateSslPolicy(!serverMode, targetHost))
			{
				using (SecTrust secTrust = new SecTrust(certificates, secPolicy))
				{
					if (validator.Settings.TrustAnchors != null)
					{
						SecStatusCode secStatusCode = secTrust.SetAnchorCertificates(validator.Settings.TrustAnchors);
						if (secStatusCode != SecStatusCode.Success)
						{
							throw new InvalidOperationException(secStatusCode.ToString());
						}
						secTrust.SetAnchorCertificatesOnly(false);
					}
					if (validator.Settings.CertificateValidationTime != null)
					{
						SecStatusCode secStatusCode2 = secTrust.SetVerifyDate(validator.Settings.CertificateValidationTime.Value);
						if (secStatusCode2 != SecStatusCode.Success)
						{
							throw new InvalidOperationException(secStatusCode2.ToString());
						}
					}
					SecTrustResult secTrustResult = secTrust.Evaluate();
					if (secTrustResult == SecTrustResult.Unspecified || secTrustResult == SecTrustResult.Proceed)
					{
						flag = true;
					}
					else
					{
						errors |= MonoSslPolicyErrors.RemoteCertificateChainErrors;
						flag = false;
					}
				}
			}
			return flag;
		}
	}
}
