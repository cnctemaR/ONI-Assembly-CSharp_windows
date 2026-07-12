using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Security
{
	internal static class CertificateHelper
	{
		internal static X509Certificate2 GetEligibleClientCertificate(X509CertificateCollection candidateCerts)
		{
			if (candidateCerts.Count == 0)
			{
				return null;
			}
			X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
			x509Certificate2Collection.AddRange(candidateCerts);
			return CertificateHelper.GetEligibleClientCertificate(x509Certificate2Collection);
		}

		internal static X509Certificate2 GetEligibleClientCertificate(X509Certificate2Collection candidateCerts)
		{
			if (candidateCerts.Count == 0)
			{
				return null;
			}
			foreach (X509Certificate2 x509Certificate in candidateCerts)
			{
				if (x509Certificate.HasPrivateKey && CertificateHelper.IsValidClientCertificate(x509Certificate))
				{
					return x509Certificate;
				}
			}
			return null;
		}

		private static bool IsValidClientCertificate(X509Certificate2 cert)
		{
			foreach (X509Extension x509Extension in cert.Extensions)
			{
				X509EnhancedKeyUsageExtension x509EnhancedKeyUsageExtension = x509Extension as X509EnhancedKeyUsageExtension;
				if (x509EnhancedKeyUsageExtension != null && !CertificateHelper.IsValidForClientAuthenticationEKU(x509EnhancedKeyUsageExtension))
				{
					return false;
				}
				X509KeyUsageExtension x509KeyUsageExtension = x509Extension as X509KeyUsageExtension;
				if (x509KeyUsageExtension != null && !CertificateHelper.IsValidForDigitalSignatureUsage(x509KeyUsageExtension))
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsValidForClientAuthenticationEKU(X509EnhancedKeyUsageExtension eku)
		{
			OidEnumerator enumerator = eku.EnhancedKeyUsages.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value == "1.3.6.1.5.5.7.3.2")
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsValidForDigitalSignatureUsage(X509KeyUsageExtension ku)
		{
			return (ku.KeyUsages & X509KeyUsageFlags.DigitalSignature) == X509KeyUsageFlags.DigitalSignature;
		}

		internal static X509Certificate2 GetEligibleClientCertificate()
		{
			X509Certificate2Collection certificates;
			using (X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
			{
				x509Store.Open(OpenFlags.OpenExistingOnly);
				certificates = x509Store.Certificates;
			}
			return CertificateHelper.GetEligibleClientCertificate(certificates);
		}

		private const string ClientAuthenticationOID = "1.3.6.1.5.5.7.3.2";
	}
}
