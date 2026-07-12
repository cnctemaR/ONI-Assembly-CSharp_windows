using System;
using System.Security.Cryptography.X509Certificates;

namespace Mono.Unity
{
	internal static class CertHelper
	{
		public unsafe static void AddCertificatesToNativeChain(UnityTls.unitytls_x509list* nativeCertificateChain, X509CertificateCollection certificates, UnityTls.unitytls_errorstate* errorState)
		{
			foreach (X509Certificate x509Certificate in certificates)
			{
				CertHelper.AddCertificateToNativeChain(nativeCertificateChain, x509Certificate, errorState);
			}
		}

		public unsafe static void AddCertificateToNativeChain(UnityTls.unitytls_x509list* nativeCertificateChain, X509Certificate certificate, UnityTls.unitytls_errorstate* errorState)
		{
			byte[] rawCertData = certificate.GetRawCertData();
			byte[] array;
			byte* ptr;
			if ((array = rawCertData) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			UnityTls.NativeInterface.unitytls_x509list_append_der(nativeCertificateChain, ptr, (IntPtr)rawCertData.Length, errorState);
			array = null;
			X509Certificate2Impl x509Certificate2Impl = certificate.Impl as X509Certificate2Impl;
			if (x509Certificate2Impl != null)
			{
				X509CertificateImplCollection intermediateCertificates = x509Certificate2Impl.IntermediateCertificates;
				if (intermediateCertificates != null && intermediateCertificates.Count > 0)
				{
					for (int i = 0; i < intermediateCertificates.Count; i++)
					{
						CertHelper.AddCertificateToNativeChain(nativeCertificateChain, new X509Certificate(intermediateCertificates[i]), errorState);
					}
				}
			}
		}
	}
}
