using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;
using Mono.Security.Authenticode;
using Mono.Security.Cryptography;

namespace Mono.Btls
{
	internal class X509CertificateImplBtls : X509Certificate2ImplUnix
	{
		internal X509CertificateImplBtls()
		{
		}

		internal X509CertificateImplBtls(MonoBtlsX509 x509)
		{
			this.x509 = x509.Copy();
		}

		private X509CertificateImplBtls(X509CertificateImplBtls other)
		{
			this.x509 = ((other.x509 != null) ? other.x509.Copy() : null);
			this.nativePrivateKey = ((other.nativePrivateKey != null) ? other.nativePrivateKey.Copy() : null);
			if (other.intermediateCerts != null)
			{
				this.intermediateCerts = other.intermediateCerts.Clone();
			}
		}

		internal X509CertificateImplBtls(byte[] data, MonoBtlsX509Format format)
		{
			this.x509 = MonoBtlsX509.LoadFromData(data, format);
		}

		internal X509CertificateImplBtls(byte[] data, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags)
		{
			if (password == null || password.IsInvalid)
			{
				try
				{
					this.Import(data);
					return;
				}
				catch (Exception ex)
				{
					try
					{
						this.ImportPkcs12(data, null);
					}
					catch
					{
						try
						{
							this.ImportAuthenticode(data);
						}
						catch
						{
							throw new CryptographicException(global::Locale.GetText("Unable to decode certificate."), ex);
						}
					}
					return;
				}
			}
			try
			{
				this.ImportPkcs12(data, password);
			}
			catch (Exception ex2)
			{
				try
				{
					this.Import(data);
				}
				catch
				{
					try
					{
						this.ImportAuthenticode(data);
					}
					catch
					{
						throw new CryptographicException(global::Locale.GetText("Unable to decode certificate."), ex2);
					}
				}
			}
		}

		public override bool IsValid
		{
			get
			{
				return this.x509 != null && this.x509.IsValid;
			}
		}

		public override IntPtr Handle
		{
			get
			{
				return this.x509.Handle.DangerousGetHandle();
			}
		}

		public override IntPtr GetNativeAppleCertificate()
		{
			return IntPtr.Zero;
		}

		internal MonoBtlsX509 X509
		{
			get
			{
				base.ThrowIfContextInvalid();
				return this.x509;
			}
		}

		internal MonoBtlsKey NativePrivateKey
		{
			get
			{
				base.ThrowIfContextInvalid();
				return this.nativePrivateKey;
			}
		}

		public override X509CertificateImpl Clone()
		{
			base.ThrowIfContextInvalid();
			return new X509CertificateImplBtls(this);
		}

		public override bool Equals(X509CertificateImpl other, out bool result)
		{
			X509CertificateImplBtls x509CertificateImplBtls = other as X509CertificateImplBtls;
			if (x509CertificateImplBtls == null)
			{
				result = false;
				return false;
			}
			result = MonoBtlsX509.Compare(this.X509, x509CertificateImplBtls.X509) == 0;
			return true;
		}

		protected override byte[] GetRawCertData()
		{
			base.ThrowIfContextInvalid();
			return this.X509.GetRawData(MonoBtlsX509Format.DER);
		}

		internal override X509CertificateImplCollection IntermediateCertificates
		{
			get
			{
				return this.intermediateCerts;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (this.x509 != null)
			{
				this.x509.Dispose();
				this.x509 = null;
			}
		}

		internal override X509Certificate2Impl FallbackImpl
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override bool HasPrivateKey
		{
			get
			{
				return this.nativePrivateKey != null;
			}
		}

		public override AsymmetricAlgorithm PrivateKey
		{
			get
			{
				if (this.nativePrivateKey == null)
				{
					return null;
				}
				return PKCS8.PrivateKeyInfo.DecodeRSA(this.nativePrivateKey.GetBytes(true));
			}
			set
			{
				if (this.nativePrivateKey != null)
				{
					this.nativePrivateKey.Dispose();
				}
				try
				{
					if (value != null)
					{
						this.nativePrivateKey = MonoBtlsKey.CreateFromRSAPrivateKey((RSA)value);
					}
				}
				catch
				{
					this.nativePrivateKey = null;
				}
			}
		}

		public override RSA GetRSAPrivateKey()
		{
			if (this.nativePrivateKey == null)
			{
				return null;
			}
			return PKCS8.PrivateKeyInfo.DecodeRSA(this.nativePrivateKey.GetBytes(true));
		}

		public override DSA GetDSAPrivateKey()
		{
			throw new PlatformNotSupportedException();
		}

		public override PublicKey PublicKey
		{
			get
			{
				base.ThrowIfContextInvalid();
				if (this.publicKey == null)
				{
					AsnEncodedData publicKeyAsn = this.X509.GetPublicKeyAsn1();
					AsnEncodedData publicKeyParameters = this.X509.GetPublicKeyParameters();
					this.publicKey = new PublicKey(publicKeyAsn.Oid, publicKeyParameters, publicKeyAsn);
				}
				return this.publicKey;
			}
		}

		private void Import(byte[] data)
		{
			if (data != null)
			{
				if (data.Length != 0 && data[0] != 48)
				{
					this.x509 = MonoBtlsX509.LoadFromData(data, MonoBtlsX509Format.PEM);
					return;
				}
				this.x509 = MonoBtlsX509.LoadFromData(data, MonoBtlsX509Format.DER);
			}
		}

		private void ImportPkcs12(byte[] data, SafePasswordHandle password)
		{
			using (MonoBtlsPkcs12 monoBtlsPkcs = new MonoBtlsPkcs12())
			{
				if (password == null || password.IsInvalid)
				{
					try
					{
						monoBtlsPkcs.Import(data, null);
						goto IL_0046;
					}
					catch
					{
						using (SafePasswordHandle safePasswordHandle = new SafePasswordHandle(string.Empty))
						{
							monoBtlsPkcs.Import(data, safePasswordHandle);
						}
						goto IL_0046;
					}
				}
				monoBtlsPkcs.Import(data, password);
				IL_0046:
				this.x509 = monoBtlsPkcs.GetCertificate(0);
				if (monoBtlsPkcs.HasPrivateKey)
				{
					this.nativePrivateKey = monoBtlsPkcs.GetPrivateKey();
				}
				if (monoBtlsPkcs.Count > 1)
				{
					this.intermediateCerts = new X509CertificateImplCollection();
					for (int i = 0; i < monoBtlsPkcs.Count; i++)
					{
						using (MonoBtlsX509 certificate = monoBtlsPkcs.GetCertificate(i))
						{
							if (MonoBtlsX509.Compare(certificate, this.x509) != 0)
							{
								X509CertificateImplBtls x509CertificateImplBtls = new X509CertificateImplBtls(certificate);
								this.intermediateCerts.Add(x509CertificateImplBtls, true);
							}
						}
					}
				}
			}
		}

		private void ImportAuthenticode(byte[] data)
		{
			if (data != null)
			{
				AuthenticodeDeformatter authenticodeDeformatter = new AuthenticodeDeformatter(data);
				this.Import(authenticodeDeformatter.SigningCertificate.RawData);
			}
		}

		public override bool Verify(X509Certificate2 thisCertificate)
		{
			bool flag;
			using (MonoBtlsX509Chain monoBtlsX509Chain = new MonoBtlsX509Chain())
			{
				monoBtlsX509Chain.AddCertificate(this.x509.Copy());
				if (this.intermediateCerts != null)
				{
					for (int i = 0; i < this.intermediateCerts.Count; i++)
					{
						X509CertificateImplBtls x509CertificateImplBtls = (X509CertificateImplBtls)this.intermediateCerts[i];
						monoBtlsX509Chain.AddCertificate(x509CertificateImplBtls.x509.Copy());
					}
				}
				flag = MonoBtlsProvider.ValidateCertificate(monoBtlsX509Chain, null);
			}
			return flag;
		}

		public override void Reset()
		{
			if (this.x509 != null)
			{
				this.x509.Dispose();
				this.x509 = null;
			}
			if (this.nativePrivateKey != null)
			{
				this.nativePrivateKey.Dispose();
				this.nativePrivateKey = null;
			}
			this.publicKey = null;
			this.intermediateCerts = null;
		}

		private MonoBtlsX509 x509;

		private MonoBtlsKey nativePrivateKey;

		private X509CertificateImplCollection intermediateCerts;

		private PublicKey publicKey;
	}
}
