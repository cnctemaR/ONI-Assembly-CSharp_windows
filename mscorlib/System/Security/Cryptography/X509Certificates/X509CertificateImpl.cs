using System;

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

		public abstract string GetIssuerName(bool legacyV1Mode);

		public abstract string GetSubjectName(bool legacyV1Mode);

		public abstract byte[] GetRawCertData();

		public abstract DateTime GetValidFrom();

		public abstract DateTime GetValidUntil();

		public byte[] GetCertHash()
		{
			this.ThrowIfContextInvalid();
			if (this.cachedCertificateHash == null)
			{
				this.cachedCertificateHash = this.GetCertHash(false);
			}
			return this.cachedCertificateHash;
		}

		protected abstract byte[] GetCertHash(bool lazy);

		public override int GetHashCode()
		{
			if (!this.IsValid)
			{
				return 0;
			}
			if (this.cachedCertificateHash == null)
			{
				this.cachedCertificateHash = this.GetCertHash(true);
			}
			if (this.cachedCertificateHash != null && this.cachedCertificateHash.Length >= 4)
			{
				return ((int)this.cachedCertificateHash[0] << 24) | ((int)this.cachedCertificateHash[1] << 16) | ((int)this.cachedCertificateHash[2] << 8) | (int)this.cachedCertificateHash[3];
			}
			return 0;
		}

		public abstract bool Equals(X509CertificateImpl other, out bool result);

		public abstract string GetKeyAlgorithm();

		public abstract byte[] GetKeyAlgorithmParameters();

		public abstract byte[] GetPublicKey();

		public abstract byte[] GetSerialNumber();

		public abstract byte[] Export(X509ContentType contentType, byte[] password);

		public abstract string ToString(bool full);

		public override bool Equals(object obj)
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
			bool flag;
			if (this.Equals(x509CertificateImpl, out flag))
			{
				return flag;
			}
			byte[] rawCertData = this.GetRawCertData();
			byte[] rawCertData2 = x509CertificateImpl.GetRawCertData();
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

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			this.cachedCertificateHash = null;
		}

		~X509CertificateImpl()
		{
			this.Dispose(false);
		}

		private byte[] cachedCertificateHash;
	}
}
