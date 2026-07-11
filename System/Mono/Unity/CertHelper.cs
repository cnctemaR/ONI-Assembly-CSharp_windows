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
			UnityTls.NativeInterface.unitytls_x509list_append_der(nativeCertificateChain, ptr, rawCertData.Length, errorState);
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

		public unsafe static X509CertificateCollection NativeChainToManagedCollection(UnityTls.unitytls_x509list_ref nativeCertificateChain, UnityTls.unitytls_errorstate* errorState)
		{
			X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();
			UnityTls.unitytls_x509_ref unitytls_x509_ref = UnityTls.NativeInterface.unitytls_x509list_get_x509(nativeCertificateChain, 0, errorState);
			int num = 0;
			while (unitytls_x509_ref.handle != UnityTls.NativeInterface.UNITYTLS_INVALID_HANDLE)
			{
				size_t size_t = UnityTls.NativeInterface.unitytls_x509_export_der(unitytls_x509_ref, null, 0, errorState);
				byte[] array = new byte[size_t];
				byte[] array2;
				byte* ptr;
				if ((array2 = array) == null || array2.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array2[0];
				}
				UnityTls.NativeInterface.unitytls_x509_export_der(unitytls_x509_ref, ptr, size_t, errorState);
				array2 = null;
				x509CertificateCollection.Add(new X509Certificate(array));
				unitytls_x509_ref = UnityTls.NativeInterface.unitytls_x509list_get_x509(nativeCertificateChain, num, errorState);
				num++;
			}
			return x509CertificateCollection;
		}
	}
}
