using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Mono.Security.X509;
using XamMac.CoreFoundation;

namespace System.Security.Cryptography.X509Certificates
{
	internal static class X509Helper
	{
		public static X509CertificateImpl InitFromHandleApple(IntPtr handle)
		{
			return new X509CertificateImplApple(handle, false);
		}

		private static X509CertificateImpl ImportApple(byte[] rawData)
		{
			IntPtr intPtr = CFHelpers.CreateCertificateFromData(rawData);
			if (intPtr != IntPtr.Zero)
			{
				return new X509CertificateImplApple(intPtr, true);
			}
			X509Certificate x509Certificate;
			try
			{
				x509Certificate = new X509Certificate(rawData);
			}
			catch (Exception ex)
			{
				try
				{
					x509Certificate = X509Helper.ImportPkcs12(rawData, null);
				}
				catch
				{
					throw new CryptographicException(Locale.GetText("Unable to decode certificate."), ex);
				}
			}
			return new X509CertificateImplMono(x509Certificate);
		}

		internal static void InstallNativeHelper(INativeCertificateHelper helper)
		{
			if (X509Helper.nativeHelper == null)
			{
				Interlocked.CompareExchange<INativeCertificateHelper>(ref X509Helper.nativeHelper, helper, null);
			}
		}

		private static bool ShouldUseAppleTls
		{
			get
			{
				if (!Environment.IsMacOS)
				{
					return false;
				}
				string environmentVariable = Environment.GetEnvironmentVariable("MONO_TLS_PROVIDER");
				return string.IsNullOrEmpty(environmentVariable) || environmentVariable == "default" || environmentVariable == "apple";
			}
		}

		public static X509CertificateImpl InitFromHandle(IntPtr handle)
		{
			if (X509Helper.ShouldUseAppleTls)
			{
				return X509Helper.InitFromHandleApple(handle);
			}
			return X509Helper.InitFromHandleCore(handle);
		}

		private static X509CertificateImpl Import(byte[] rawData)
		{
			if (X509Helper.ShouldUseAppleTls)
			{
				return X509Helper.ImportApple(rawData);
			}
			return X509Helper.ImportCore(rawData);
		}

		[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
		public static X509CertificateImpl InitFromHandleCore(IntPtr handle)
		{
			X509Helper.CertificateContext certificateContext = (X509Helper.CertificateContext)Marshal.PtrToStructure(handle, typeof(X509Helper.CertificateContext));
			byte[] array = new byte[certificateContext.cbCertEncoded];
			Marshal.Copy(certificateContext.pbCertEncoded, array, 0, (int)certificateContext.cbCertEncoded);
			return new X509CertificateImplMono(new X509Certificate(array));
		}

		public static X509CertificateImpl InitFromCertificate(X509Certificate cert)
		{
			if (X509Helper.nativeHelper != null)
			{
				return X509Helper.nativeHelper.Import(cert);
			}
			return X509Helper.InitFromCertificate(cert.Impl);
		}

		public static X509CertificateImpl InitFromCertificate(X509CertificateImpl impl)
		{
			X509Helper.ThrowIfContextInvalid(impl);
			X509CertificateImpl x509CertificateImpl = impl.Clone();
			if (x509CertificateImpl != null)
			{
				return x509CertificateImpl;
			}
			byte[] rawCertData = impl.GetRawCertData();
			if (rawCertData == null)
			{
				return null;
			}
			return new X509CertificateImplMono(new X509Certificate(rawCertData));
		}

		public static bool IsValid(X509CertificateImpl impl)
		{
			return impl != null && impl.IsValid;
		}

		internal static void ThrowIfContextInvalid(X509CertificateImpl impl)
		{
			if (!X509Helper.IsValid(impl))
			{
				throw X509Helper.GetInvalidContextException();
			}
		}

		internal static Exception GetInvalidContextException()
		{
			return new CryptographicException(Locale.GetText("Certificate instance is empty."));
		}

		internal static X509Certificate ImportPkcs12(byte[] rawData, string password)
		{
			PKCS12 pkcs = ((password == null) ? new PKCS12(rawData) : new PKCS12(rawData, password));
			if (pkcs.Certificates.Count == 0)
			{
				return null;
			}
			if (pkcs.Keys.Count == 0)
			{
				return pkcs.Certificates[0];
			}
			string text = (pkcs.Keys[0] as AsymmetricAlgorithm).ToXmlString(false);
			foreach (X509Certificate x509Certificate in pkcs.Certificates)
			{
				if (x509Certificate.RSA != null && text == x509Certificate.RSA.ToXmlString(false))
				{
					return x509Certificate;
				}
				if (x509Certificate.DSA != null && text == x509Certificate.DSA.ToXmlString(false))
				{
					return x509Certificate;
				}
			}
			return pkcs.Certificates[0];
		}

		private static byte[] PEM(string type, byte[] data)
		{
			string @string = Encoding.ASCII.GetString(data);
			string text = string.Format("-----BEGIN {0}-----", type);
			string text2 = string.Format("-----END {0}-----", type);
			int num = @string.IndexOf(text) + text.Length;
			int num2 = @string.IndexOf(text2, num);
			return Convert.FromBase64String(@string.Substring(num, num2 - num));
		}

		private static byte[] ConvertData(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				return data;
			}
			if (data[0] != 48)
			{
				try
				{
					return X509Helper.PEM("CERTIFICATE", data);
				}
				catch
				{
				}
				return data;
			}
			return data;
		}

		private static X509CertificateImpl ImportCore(byte[] rawData)
		{
			X509Certificate x509Certificate;
			try
			{
				x509Certificate = new X509Certificate(rawData);
			}
			catch (Exception ex)
			{
				try
				{
					x509Certificate = X509Helper.ImportPkcs12(rawData, null);
				}
				catch
				{
					throw new CryptographicException(Locale.GetText("Unable to decode certificate."), ex);
				}
			}
			return new X509CertificateImplMono(x509Certificate);
		}

		public static X509CertificateImpl Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
		{
			if (password == null)
			{
				rawData = X509Helper.ConvertData(rawData);
				return X509Helper.Import(rawData);
			}
			X509Certificate x509Certificate;
			try
			{
				x509Certificate = X509Helper.ImportPkcs12(rawData, password);
			}
			catch
			{
				x509Certificate = new X509Certificate(rawData);
			}
			return new X509CertificateImplMono(x509Certificate);
		}

		public static byte[] Export(X509CertificateImpl impl, X509ContentType contentType, byte[] password)
		{
			X509Helper.ThrowIfContextInvalid(impl);
			return impl.Export(contentType, password);
		}

		public static bool Equals(X509CertificateImpl first, X509CertificateImpl second)
		{
			if (!X509Helper.IsValid(first) || !X509Helper.IsValid(second))
			{
				return false;
			}
			bool flag;
			if (first.Equals(second, out flag))
			{
				return flag;
			}
			byte[] rawCertData = first.GetRawCertData();
			byte[] rawCertData2 = second.GetRawCertData();
			if (rawCertData == null)
			{
				return rawCertData2 == null;
			}
			if (rawCertData2 == null)
			{
				return false;
			}
			if (rawCertData.Length != rawCertData2.Length)
			{
				return false;
			}
			for (int i = 0; i < rawCertData.Length; i++)
			{
				if (rawCertData[i] != rawCertData2[i])
				{
					return false;
				}
			}
			return true;
		}

		public static string ToHexString(byte[] data)
		{
			if (data != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < data.Length; i++)
				{
					stringBuilder.Append(data[i].ToString("X2"));
				}
				return stringBuilder.ToString();
			}
			return null;
		}

		private static INativeCertificateHelper nativeHelper;

		internal struct CertificateContext
		{
			public uint dwCertEncodingType;

			public IntPtr pbCertEncoded;

			public uint cbCertEncoded;

			public IntPtr pCertInfo;

			public IntPtr hCertStore;
		}
	}
}
