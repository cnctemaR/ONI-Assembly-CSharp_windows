using System;
using System.Text;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	internal sealed class X509CertificateImplMono : X509CertificateImpl
	{
		public X509CertificateImplMono(X509Certificate x509)
		{
			this.x509 = x509;
		}

		public override bool IsValid
		{
			get
			{
				return this.x509 != null;
			}
		}

		public override IntPtr Handle
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		public override IntPtr GetNativeAppleCertificate()
		{
			return IntPtr.Zero;
		}

		public override X509CertificateImpl Clone()
		{
			base.ThrowIfContextInvalid();
			return new X509CertificateImplMono(this.x509);
		}

		public override string GetIssuerName(bool legacyV1Mode)
		{
			base.ThrowIfContextInvalid();
			if (legacyV1Mode)
			{
				return this.x509.IssuerName;
			}
			return X501.ToString(this.x509.GetIssuerName(), true, ", ", true);
		}

		public override string GetSubjectName(bool legacyV1Mode)
		{
			base.ThrowIfContextInvalid();
			if (legacyV1Mode)
			{
				return this.x509.SubjectName;
			}
			return X501.ToString(this.x509.GetSubjectName(), true, ", ", true);
		}

		public override byte[] GetRawCertData()
		{
			base.ThrowIfContextInvalid();
			return this.x509.RawData;
		}

		protected override byte[] GetCertHash(bool lazy)
		{
			base.ThrowIfContextInvalid();
			return SHA1.Create().ComputeHash(this.x509.RawData);
		}

		public override DateTime GetValidFrom()
		{
			base.ThrowIfContextInvalid();
			return this.x509.ValidFrom;
		}

		public override DateTime GetValidUntil()
		{
			base.ThrowIfContextInvalid();
			return this.x509.ValidUntil;
		}

		public override bool Equals(X509CertificateImpl other, out bool result)
		{
			result = false;
			return false;
		}

		public override string GetKeyAlgorithm()
		{
			base.ThrowIfContextInvalid();
			return this.x509.KeyAlgorithm;
		}

		public override byte[] GetKeyAlgorithmParameters()
		{
			base.ThrowIfContextInvalid();
			return this.x509.KeyAlgorithmParameters;
		}

		public override byte[] GetPublicKey()
		{
			base.ThrowIfContextInvalid();
			return this.x509.PublicKey;
		}

		public override byte[] GetSerialNumber()
		{
			base.ThrowIfContextInvalid();
			return this.x509.SerialNumber;
		}

		public override byte[] Export(X509ContentType contentType, byte[] password)
		{
			base.ThrowIfContextInvalid();
			switch (contentType)
			{
			case X509ContentType.Cert:
				return this.GetRawCertData();
			case X509ContentType.SerializedCert:
				throw new NotSupportedException();
			case X509ContentType.Pfx:
				throw new NotSupportedException();
			default:
				throw new CryptographicException(Locale.GetText("This certificate format '{0}' cannot be exported.", new object[] { contentType }));
			}
		}

		public override string ToString(bool full)
		{
			base.ThrowIfContextInvalid();
			string newLine = Environment.NewLine;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[Subject]{0}  {1}{0}{0}", newLine, this.GetSubjectName(false));
			stringBuilder.AppendFormat("[Issuer]{0}  {1}{0}{0}", newLine, this.GetIssuerName(false));
			stringBuilder.AppendFormat("[Not Before]{0}  {1}{0}{0}", newLine, this.GetValidFrom().ToLocalTime());
			stringBuilder.AppendFormat("[Not After]{0}  {1}{0}{0}", newLine, this.GetValidUntil().ToLocalTime());
			stringBuilder.AppendFormat("[Thumbprint]{0}  {1}{0}", newLine, X509Helper.ToHexString(base.GetCertHash()));
			stringBuilder.Append(newLine);
			return stringBuilder.ToString();
		}

		protected override void Dispose(bool disposing)
		{
			this.x509 = null;
		}

		private X509Certificate x509;
	}
}
