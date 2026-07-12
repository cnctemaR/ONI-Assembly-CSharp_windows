using System;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates
{
	internal abstract class X509CertificateImpl : IDisposable
	{
		public abstract bool IsValid { get; }

		public abstract IntPtr Handle { get; }

		public abstract IntPtr GetNativeAppleCertificate();

		protected void ThrowIfContextInvalid()
		{
			if (!this.IsValid)
			{
				throw X509Helper.GetInvalidContextException();
			}
		}

		public abstract X509CertificateImpl Clone();

		public abstract string Issuer { get; }

		public abstract string Subject { get; }

		public abstract string LegacyIssuer { get; }

		public abstract string LegacySubject { get; }

		public abstract byte[] RawData { get; }

		public abstract DateTime NotAfter { get; }

		public abstract DateTime NotBefore { get; }

		public abstract byte[] Thumbprint { get; }

		public sealed override int GetHashCode()
		{
			if (!this.IsValid)
			{
				return 0;
			}
			byte[] thumbprint = this.Thumbprint;
			int num = 0;
			int num2 = 0;
			while (num2 < thumbprint.Length && num2 < 4)
			{
				num = (num << 8) | (int)thumbprint[num2];
				num2++;
			}
			return num;
		}

		public abstract bool Equals(X509CertificateImpl other, out bool result);

		public abstract string KeyAlgorithm { get; }

		public abstract byte[] KeyAlgorithmParameters { get; }

		public abstract byte[] PublicKeyValue { get; }

		public abstract byte[] SerialNumber { get; }

		public abstract bool HasPrivateKey { get; }

		public abstract RSA GetRSAPrivateKey();

		public abstract DSA GetDSAPrivateKey();

		public abstract byte[] Export(X509ContentType contentType, SafePasswordHandle password);

		public abstract X509CertificateImpl CopyWithPrivateKey(RSA privateKey);

		public abstract X509Certificate CreateCertificate();

		public sealed override bool Equals(object obj)
		{
			X509CertificateImpl x509CertificateImpl = obj as X509CertificateImpl;
			if (x509CertificateImpl == null)
			{
				return false;
			}
			if (!this.IsValid || !x509CertificateImpl.IsValid)
			{
				return false;
			}
			if (!this.Issuer.Equals(x509CertificateImpl.Issuer))
			{
				return false;
			}
			byte[] serialNumber = this.SerialNumber;
			byte[] serialNumber2 = x509CertificateImpl.SerialNumber;
			if (serialNumber.Length != serialNumber2.Length)
			{
				return false;
			}
			for (int i = 0; i < serialNumber.Length; i++)
			{
				if (serialNumber[i] != serialNumber2[i])
				{
					return false;
				}
			}
			return true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		~X509CertificateImpl()
		{
			this.Dispose(false);
		}
	}
}
