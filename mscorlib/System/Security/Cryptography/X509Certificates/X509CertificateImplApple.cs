using System;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Security.X509;
using XamMac.CoreFoundation;

namespace System.Security.Cryptography.X509Certificates
{
	internal class X509CertificateImplApple : X509CertificateImpl
	{
		public X509CertificateImplApple(IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
			{
				CFHelpers.CFRetain(handle);
			}
		}

		public override bool IsValid
		{
			get
			{
				return this.handle != IntPtr.Zero;
			}
		}

		public override IntPtr Handle
		{
			get
			{
				return this.handle;
			}
		}

		public override IntPtr GetNativeAppleCertificate()
		{
			base.ThrowIfContextInvalid();
			return this.handle;
		}

		public override X509CertificateImpl Clone()
		{
			base.ThrowIfContextInvalid();
			return new X509CertificateImplApple(this.handle, false);
		}

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecCertificateCopySubjectSummary(IntPtr cert);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecCertificateCopyData(IntPtr cert);

		public override byte[] GetRawCertData()
		{
			base.ThrowIfContextInvalid();
			IntPtr intPtr = X509CertificateImplApple.SecCertificateCopyData(this.handle);
			if (intPtr == IntPtr.Zero)
			{
				throw new ArgumentException("Not a valid certificate");
			}
			byte[] array;
			try
			{
				array = CFHelpers.FetchDataBuffer(intPtr);
			}
			finally
			{
				CFHelpers.CFRelease(intPtr);
			}
			return array;
		}

		public string GetSubjectSummary()
		{
			base.ThrowIfContextInvalid();
			IntPtr intPtr = X509CertificateImplApple.SecCertificateCopySubjectSummary(this.handle);
			string text = CFHelpers.FetchString(intPtr);
			CFHelpers.CFRelease(intPtr);
			return text;
		}

		protected override byte[] GetCertHash(bool lazy)
		{
			base.ThrowIfContextInvalid();
			return SHA1.Create().ComputeHash(this.GetRawCertData());
		}

		public override bool Equals(X509CertificateImpl other, out bool result)
		{
			X509CertificateImplApple x509CertificateImplApple = other as X509CertificateImplApple;
			if (x509CertificateImplApple != null && x509CertificateImplApple.handle == this.handle)
			{
				result = true;
				return true;
			}
			result = false;
			return false;
		}

		private void MustFallback()
		{
			base.ThrowIfContextInvalid();
			if (this.fallback != null)
			{
				return;
			}
			X509Certificate x509Certificate = new X509Certificate(this.GetRawCertData());
			this.fallback = new X509CertificateImplMono(x509Certificate);
		}

		public X509CertificateImpl FallbackImpl
		{
			get
			{
				this.MustFallback();
				return this.fallback;
			}
		}

		public override string GetSubjectName(bool legacyV1Mode)
		{
			return this.FallbackImpl.GetSubjectName(legacyV1Mode);
		}

		public override string GetIssuerName(bool legacyV1Mode)
		{
			return this.FallbackImpl.GetIssuerName(legacyV1Mode);
		}

		public override DateTime GetValidFrom()
		{
			return this.FallbackImpl.GetValidFrom();
		}

		public override DateTime GetValidUntil()
		{
			return this.FallbackImpl.GetValidUntil();
		}

		public override string GetKeyAlgorithm()
		{
			return this.FallbackImpl.GetKeyAlgorithm();
		}

		public override byte[] GetKeyAlgorithmParameters()
		{
			return this.FallbackImpl.GetKeyAlgorithmParameters();
		}

		public override byte[] GetPublicKey()
		{
			return this.FallbackImpl.GetPublicKey();
		}

		public override byte[] GetSerialNumber()
		{
			return this.FallbackImpl.GetSerialNumber();
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
			if (!full || this.fallback == null)
			{
				string subjectSummary = this.GetSubjectSummary();
				return string.Format("[X509Certificate: {0}]", subjectSummary);
			}
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
			if (this.handle != IntPtr.Zero)
			{
				CFHelpers.CFRelease(this.handle);
				this.handle = IntPtr.Zero;
			}
			if (this.fallback != null)
			{
				this.fallback.Dispose();
				this.fallback = null;
			}
		}

		private IntPtr handle;

		private X509CertificateImpl fallback;
	}
}
