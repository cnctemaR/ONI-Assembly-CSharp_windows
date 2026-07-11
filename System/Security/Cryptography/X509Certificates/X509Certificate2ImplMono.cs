using System;
using System.Collections;
using System.IO;
using System.Text;
using Mono.Security;
using Mono.Security.Cryptography;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	internal class X509Certificate2ImplMono : X509Certificate2Impl
	{
		public override bool IsValid
		{
			get
			{
				return this._cert != null;
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

		private X509Certificate2ImplMono(Mono.Security.X509.X509Certificate cert)
		{
			this._cert = cert;
		}

		private X509Certificate2ImplMono(X509Certificate2ImplMono other)
		{
			this._cert = other._cert;
			if (other.intermediateCerts != null)
			{
				this.intermediateCerts = other.intermediateCerts.Clone();
			}
		}

		public override X509CertificateImpl Clone()
		{
			base.ThrowIfContextInvalid();
			return new X509Certificate2ImplMono(this);
		}

		public override string GetIssuerName(bool legacyV1Mode)
		{
			base.ThrowIfContextInvalid();
			if (legacyV1Mode)
			{
				return this._cert.IssuerName;
			}
			return Mono.Security.X509.X501.ToString(this._cert.GetIssuerName(), true, ", ", true);
		}

		public override string GetSubjectName(bool legacyV1Mode)
		{
			base.ThrowIfContextInvalid();
			if (legacyV1Mode)
			{
				return this._cert.SubjectName;
			}
			return Mono.Security.X509.X501.ToString(this._cert.GetSubjectName(), true, ", ", true);
		}

		public override byte[] GetRawCertData()
		{
			base.ThrowIfContextInvalid();
			return this._cert.RawData;
		}

		protected override byte[] GetCertHash(bool lazy)
		{
			base.ThrowIfContextInvalid();
			return SHA1.Create().ComputeHash(this._cert.RawData);
		}

		public override DateTime GetValidFrom()
		{
			base.ThrowIfContextInvalid();
			return this._cert.ValidFrom;
		}

		public override DateTime GetValidUntil()
		{
			base.ThrowIfContextInvalid();
			return this._cert.ValidUntil;
		}

		public override bool Equals(X509CertificateImpl other, out bool result)
		{
			result = false;
			return false;
		}

		public override string GetKeyAlgorithm()
		{
			base.ThrowIfContextInvalid();
			return this._cert.KeyAlgorithm;
		}

		public override byte[] GetKeyAlgorithmParameters()
		{
			base.ThrowIfContextInvalid();
			return this._cert.KeyAlgorithmParameters;
		}

		public override byte[] GetPublicKey()
		{
			base.ThrowIfContextInvalid();
			return this._cert.PublicKey;
		}

		public override byte[] GetSerialNumber()
		{
			base.ThrowIfContextInvalid();
			return this._cert.SerialNumber;
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
				throw new CryptographicException(global::Locale.GetText("This certificate format '{0}' cannot be exported.", new object[] { contentType }));
			}
		}

		public X509Certificate2ImplMono()
		{
			this._cert = null;
		}

		public override bool Archived
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2ImplMono.empty_error);
				}
				return this._archived;
			}
			set
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2ImplMono.empty_error);
				}
				this._archived = value;
			}
		}

		public override global::System.Security.Cryptography.X509Certificates.X509ExtensionCollection Extensions
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2ImplMono.empty_error);
				}
				if (this._extensions == null)
				{
					this._extensions = new global::System.Security.Cryptography.X509Certificates.X509ExtensionCollection(this._cert);
				}
				return this._extensions;
			}
		}

		public override bool HasPrivateKey
		{
			get
			{
				return this.PrivateKey != null;
			}
		}

		public override X500DistinguishedName IssuerName
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2ImplMono.empty_error);
				}
				if (this.issuer_name == null)
				{
					this.issuer_name = new X500DistinguishedName(this._cert.GetIssuerName().GetBytes());
				}
				return this.issuer_name;
			}
		}

		public override AsymmetricAlgorithm PrivateKey
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2ImplMono.empty_error);
				}
				try
				{
					if (this._cert.RSA != null)
					{
						RSACryptoServiceProvider rsacryptoServiceProvider = this._cert.RSA as RSACryptoServiceProvider;
						if (rsacryptoServiceProvider != null)
						{
							return rsacryptoServiceProvider.PublicOnly ? null : rsacryptoServiceProvider;
						}
						Mono.Security.Cryptography.RSAManaged rsamanaged = this._cert.RSA as Mono.Security.Cryptography.RSAManaged;
						if (rsamanaged != null)
						{
							return rsamanaged.PublicOnly ? null : rsamanaged;
						}
						this._cert.RSA.ExportParameters(true);
						return this._cert.RSA;
					}
					else if (this._cert.DSA != null)
					{
						DSACryptoServiceProvider dsacryptoServiceProvider = this._cert.DSA as DSACryptoServiceProvider;
						if (dsacryptoServiceProvider != null)
						{
							return dsacryptoServiceProvider.PublicOnly ? null : dsacryptoServiceProvider;
						}
						this._cert.DSA.ExportParameters(true);
						return this._cert.DSA;
					}
				}
				catch
				{
				}
				return null;
			}
			set
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2ImplMono.empty_error);
				}
				if (value == null)
				{
					this._cert.RSA = null;
					this._cert.DSA = null;
					return;
				}
				if (value is RSA)
				{
					this._cert.RSA = (RSA)value;
					return;
				}
				if (value is DSA)
				{
					this._cert.DSA = (DSA)value;
					return;
				}
				throw new NotSupportedException();
			}
		}

		public override PublicKey PublicKey
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2ImplMono.empty_error);
				}
				if (this._publicKey == null)
				{
					try
					{
						this._publicKey = new PublicKey(this._cert);
					}
					catch (Exception ex)
					{
						throw new CryptographicException(global::Locale.GetText("Unable to decode public key."), ex);
					}
				}
				return this._publicKey;
			}
		}

		public override Oid SignatureAlgorithm
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2ImplMono.empty_error);
				}
				if (this.signature_algorithm == null)
				{
					this.signature_algorithm = new Oid(this._cert.SignatureAlgorithm);
				}
				return this.signature_algorithm;
			}
		}

		public override X500DistinguishedName SubjectName
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2ImplMono.empty_error);
				}
				if (this.subject_name == null)
				{
					this.subject_name = new X500DistinguishedName(this._cert.GetSubjectName().GetBytes());
				}
				return this.subject_name;
			}
		}

		public override int Version
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2ImplMono.empty_error);
				}
				return this._cert.Version;
			}
		}

		[MonoTODO("always return String.Empty for UpnName, DnsFromAlternativeName and UrlName")]
		public override string GetNameInfo(X509NameType nameType, bool forIssuer)
		{
			switch (nameType)
			{
			case X509NameType.SimpleName:
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2ImplMono.empty_error);
				}
				Mono.Security.ASN1 asn = (forIssuer ? this._cert.GetIssuerName() : this._cert.GetSubjectName());
				Mono.Security.ASN1 asn2 = this.Find(X509Certificate2ImplMono.commonName, asn);
				if (asn2 != null)
				{
					return this.GetValueAsString(asn2);
				}
				if (asn.Count == 0)
				{
					return string.Empty;
				}
				Mono.Security.ASN1 asn3 = asn[asn.Count - 1];
				if (asn3.Count == 0)
				{
					return string.Empty;
				}
				return this.GetValueAsString(asn3[0]);
			}
			case X509NameType.EmailName:
			{
				Mono.Security.ASN1 asn4 = this.Find(X509Certificate2ImplMono.email, forIssuer ? this._cert.GetIssuerName() : this._cert.GetSubjectName());
				if (asn4 != null)
				{
					return this.GetValueAsString(asn4);
				}
				return string.Empty;
			}
			case X509NameType.UpnName:
				return string.Empty;
			case X509NameType.DnsName:
			{
				Mono.Security.ASN1 asn5 = this.Find(X509Certificate2ImplMono.commonName, forIssuer ? this._cert.GetIssuerName() : this._cert.GetSubjectName());
				if (asn5 != null)
				{
					return this.GetValueAsString(asn5);
				}
				return string.Empty;
			}
			case X509NameType.DnsFromAlternativeName:
				return string.Empty;
			case X509NameType.UrlName:
				return string.Empty;
			default:
				throw new ArgumentException("nameType");
			}
		}

		private Mono.Security.ASN1 Find(byte[] oid, Mono.Security.ASN1 dn)
		{
			if (dn.Count == 0)
			{
				return null;
			}
			for (int i = 0; i < dn.Count; i++)
			{
				Mono.Security.ASN1 asn = dn[i];
				for (int j = 0; j < asn.Count; j++)
				{
					Mono.Security.ASN1 asn2 = asn[j];
					if (asn2.Count == 2)
					{
						Mono.Security.ASN1 asn3 = asn2[0];
						if (asn3 != null && asn3.CompareValue(oid))
						{
							return asn2;
						}
					}
				}
			}
			return null;
		}

		private string GetValueAsString(Mono.Security.ASN1 pair)
		{
			if (pair.Count != 2)
			{
				return string.Empty;
			}
			Mono.Security.ASN1 asn = pair[1];
			if (asn.Value == null || asn.Length == 0)
			{
				return string.Empty;
			}
			if (asn.Tag == 30)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 1; i < asn.Value.Length; i += 2)
				{
					stringBuilder.Append((char)asn.Value[i]);
				}
				return stringBuilder.ToString();
			}
			return Encoding.UTF8.GetString(asn.Value);
		}

		private Mono.Security.X509.X509Certificate ImportPkcs12(byte[] rawData, string password)
		{
			Mono.Security.X509.PKCS12 pkcs = null;
			if (string.IsNullOrEmpty(password))
			{
				try
				{
					pkcs = new Mono.Security.X509.PKCS12(rawData, null);
					goto IL_002B;
				}
				catch
				{
					pkcs = new Mono.Security.X509.PKCS12(rawData, string.Empty);
					goto IL_002B;
				}
			}
			pkcs = new Mono.Security.X509.PKCS12(rawData, password);
			IL_002B:
			if (pkcs.Certificates.Count == 0)
			{
				return null;
			}
			if (pkcs.Keys.Count == 0)
			{
				return pkcs.Certificates[0];
			}
			Mono.Security.X509.X509Certificate x509Certificate = null;
			AsymmetricAlgorithm asymmetricAlgorithm = pkcs.Keys[0] as AsymmetricAlgorithm;
			string text = asymmetricAlgorithm.ToXmlString(false);
			foreach (Mono.Security.X509.X509Certificate x509Certificate2 in pkcs.Certificates)
			{
				if ((x509Certificate2.RSA != null && text == x509Certificate2.RSA.ToXmlString(false)) || (x509Certificate2.DSA != null && text == x509Certificate2.DSA.ToXmlString(false)))
				{
					x509Certificate = x509Certificate2;
					break;
				}
			}
			if (x509Certificate == null)
			{
				x509Certificate = pkcs.Certificates[0];
			}
			else
			{
				x509Certificate.RSA = asymmetricAlgorithm as RSA;
				x509Certificate.DSA = asymmetricAlgorithm as DSA;
			}
			if (pkcs.Certificates.Count > 1)
			{
				this.intermediateCerts = new X509CertificateImplCollection();
				foreach (Mono.Security.X509.X509Certificate x509Certificate3 in pkcs.Certificates)
				{
					if (x509Certificate3 != x509Certificate)
					{
						X509Certificate2ImplMono x509Certificate2ImplMono = new X509Certificate2ImplMono(x509Certificate3);
						this.intermediateCerts.Add(x509Certificate2ImplMono, true);
					}
				}
			}
			return x509Certificate;
		}

		[MonoTODO("missing KeyStorageFlags support")]
		public override void Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Reset();
			Mono.Security.X509.X509Certificate x509Certificate = null;
			if (password == null)
			{
				try
				{
					x509Certificate = new Mono.Security.X509.X509Certificate(rawData);
					goto IL_004A;
				}
				catch (Exception ex)
				{
					try
					{
						x509Certificate = this.ImportPkcs12(rawData, null);
					}
					catch
					{
						throw new CryptographicException(global::Locale.GetText("Unable to decode certificate."), ex);
					}
					goto IL_004A;
				}
			}
			try
			{
				x509Certificate = this.ImportPkcs12(rawData, password);
			}
			catch
			{
				x509Certificate = new Mono.Security.X509.X509Certificate(rawData);
			}
			IL_004A:
			this._cert = x509Certificate;
		}

		[MonoTODO("X509ContentType.SerializedCert is not supported")]
		public override byte[] Export(X509ContentType contentType, string password)
		{
			if (this._cert == null)
			{
				throw new CryptographicException(X509Certificate2ImplMono.empty_error);
			}
			switch (contentType)
			{
			case X509ContentType.Cert:
				return this._cert.RawData;
			case X509ContentType.SerializedCert:
				throw new NotSupportedException();
			case X509ContentType.Pfx:
				return this.ExportPkcs12(password);
			default:
				throw new CryptographicException(global::Locale.GetText("This certificate format '{0}' cannot be exported.", new object[] { contentType }));
			}
		}

		private byte[] ExportPkcs12(string password)
		{
			Mono.Security.X509.PKCS12 pkcs = new Mono.Security.X509.PKCS12();
			byte[] bytes;
			try
			{
				Hashtable hashtable = new Hashtable();
				ArrayList arrayList = new ArrayList();
				ArrayList arrayList2 = arrayList;
				byte[] array = new byte[4];
				array[0] = 1;
				arrayList2.Add(array);
				hashtable.Add("1.2.840.113549.1.9.21", arrayList);
				if (password != null)
				{
					pkcs.Password = password;
				}
				pkcs.AddCertificate(this._cert, hashtable);
				AsymmetricAlgorithm privateKey = this.PrivateKey;
				if (privateKey != null)
				{
					pkcs.AddPkcs8ShroudedKeyBag(privateKey, hashtable);
				}
				bytes = pkcs.GetBytes();
			}
			finally
			{
				pkcs.Password = null;
			}
			return bytes;
		}

		public override void Reset()
		{
			this._cert = null;
			this._archived = false;
			this._extensions = null;
			this._publicKey = null;
			this.issuer_name = null;
			this.subject_name = null;
			this.signature_algorithm = null;
			if (this.intermediateCerts != null)
			{
				this.intermediateCerts.Dispose();
				this.intermediateCerts = null;
			}
		}

		public override string ToString()
		{
			if (this._cert == null)
			{
				return "System.Security.Cryptography.X509Certificates.X509Certificate2";
			}
			return this.ToString(true);
		}

		public override string ToString(bool verbose)
		{
			if (this._cert == null)
			{
				return "System.Security.Cryptography.X509Certificates.X509Certificate2";
			}
			string newLine = Environment.NewLine;
			StringBuilder stringBuilder = new StringBuilder();
			if (!verbose)
			{
				stringBuilder.AppendFormat("[Subject]{0}  {1}{0}{0}", newLine, this.GetSubjectName(false));
				stringBuilder.AppendFormat("[Issuer]{0}  {1}{0}{0}", newLine, this.GetIssuerName(false));
				stringBuilder.AppendFormat("[Not Before]{0}  {1}{0}{0}", newLine, this.GetValidFrom().ToLocalTime());
				stringBuilder.AppendFormat("[Not After]{0}  {1}{0}{0}", newLine, this.GetValidUntil().ToLocalTime());
				stringBuilder.AppendFormat("[Thumbprint]{0}  {1}{0}", newLine, X509Helper.ToHexString(base.GetCertHash()));
				stringBuilder.Append(newLine);
				return stringBuilder.ToString();
			}
			stringBuilder.AppendFormat("[Version]{0}  V{1}{0}{0}", newLine, this.Version);
			stringBuilder.AppendFormat("[Subject]{0}  {1}{0}{0}", newLine, this.GetSubjectName(false));
			stringBuilder.AppendFormat("[Issuer]{0}  {1}{0}{0}", newLine, this.GetIssuerName(false));
			stringBuilder.AppendFormat("[Serial Number]{0}  {1}{0}{0}", newLine, this.GetSerialNumber());
			stringBuilder.AppendFormat("[Not Before]{0}  {1}{0}{0}", newLine, this.GetValidFrom().ToLocalTime());
			stringBuilder.AppendFormat("[Not After]{0}  {1}{0}{0}", newLine, this.GetValidUntil().ToLocalTime());
			stringBuilder.AppendFormat("[Thumbprint]{0}  {1}{0}", newLine, X509Helper.ToHexString(base.GetCertHash()));
			stringBuilder.AppendFormat("[Signature Algorithm]{0}  {1}({2}){0}{0}", newLine, this.SignatureAlgorithm.FriendlyName, this.SignatureAlgorithm.Value);
			AsymmetricAlgorithm key = this.PublicKey.Key;
			stringBuilder.AppendFormat("[Public Key]{0}  Algorithm: ", newLine);
			if (key is RSA)
			{
				stringBuilder.Append("RSA");
			}
			else if (key is DSA)
			{
				stringBuilder.Append("DSA");
			}
			else
			{
				stringBuilder.Append(key.ToString());
			}
			stringBuilder.AppendFormat("{0}  Length: {1}{0}  Key Blob: ", newLine, key.KeySize);
			X509Certificate2ImplMono.AppendBuffer(stringBuilder, this.PublicKey.EncodedKeyValue.RawData);
			stringBuilder.AppendFormat("{0}  Parameters: ", newLine);
			X509Certificate2ImplMono.AppendBuffer(stringBuilder, this.PublicKey.EncodedParameters.RawData);
			stringBuilder.Append(newLine);
			return stringBuilder.ToString();
		}

		private static void AppendBuffer(StringBuilder sb, byte[] buffer)
		{
			if (buffer == null)
			{
				return;
			}
			for (int i = 0; i < buffer.Length; i++)
			{
				sb.Append(buffer[i].ToString("x2"));
				if (i < buffer.Length - 1)
				{
					sb.Append(" ");
				}
			}
		}

		[MonoTODO("by default this depends on the incomplete X509Chain")]
		public override bool Verify(X509Certificate2 thisCertificate)
		{
			if (this._cert == null)
			{
				throw new CryptographicException(X509Certificate2ImplMono.empty_error);
			}
			return global::System.Security.Cryptography.X509Certificates.X509Chain.Create().Build(thisCertificate);
		}

		[MonoTODO("Detection limited to Cert, Pfx, Pkcs12, Pkcs7 and Unknown")]
		public static X509ContentType GetCertContentType(byte[] rawData)
		{
			if (rawData == null || rawData.Length == 0)
			{
				throw new ArgumentException("rawData");
			}
			X509ContentType x509ContentType = X509ContentType.Unknown;
			try
			{
				Mono.Security.ASN1 asn = new Mono.Security.ASN1(rawData);
				if (asn.Tag != 48)
				{
					throw new CryptographicException(global::Locale.GetText("Unable to decode certificate."));
				}
				if (asn.Count == 0)
				{
					return x509ContentType;
				}
				if (asn.Count == 3)
				{
					byte tag = asn[0].Tag;
					if (tag != 2)
					{
						if (tag == 48 && asn[1].Tag == 48 && asn[2].Tag == 3)
						{
							x509ContentType = X509ContentType.Cert;
						}
					}
					else if (asn[1].Tag == 48 && asn[2].Tag == 48)
					{
						x509ContentType = X509ContentType.Pfx;
					}
				}
				if (asn[0].Tag == 6 && asn[0].CompareValue(X509Certificate2ImplMono.signedData))
				{
					x509ContentType = X509ContentType.Pkcs7;
				}
			}
			catch (Exception ex)
			{
				throw new CryptographicException(global::Locale.GetText("Unable to decode certificate."), ex);
			}
			return x509ContentType;
		}

		[MonoTODO("Detection limited to Cert, Pfx, Pkcs12 and Unknown")]
		public static X509ContentType GetCertContentType(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			if (fileName.Length == 0)
			{
				throw new ArgumentException("fileName");
			}
			return X509Certificate2ImplMono.GetCertContentType(File.ReadAllBytes(fileName));
		}

		internal override X509CertificateImplCollection IntermediateCertificates
		{
			get
			{
				return this.intermediateCerts;
			}
		}

		internal Mono.Security.X509.X509Certificate MonoCertificate
		{
			get
			{
				return this._cert;
			}
		}

		internal override X509Certificate2Impl FallbackImpl
		{
			get
			{
				return this;
			}
		}

		private bool _archived;

		private global::System.Security.Cryptography.X509Certificates.X509ExtensionCollection _extensions;

		private PublicKey _publicKey;

		private X500DistinguishedName issuer_name;

		private X500DistinguishedName subject_name;

		private Oid signature_algorithm;

		private X509CertificateImplCollection intermediateCerts;

		private Mono.Security.X509.X509Certificate _cert;

		private static string empty_error = global::Locale.GetText("Certificate instance is empty.");

		private static byte[] commonName = new byte[] { 85, 4, 3 };

		private static byte[] email = new byte[] { 42, 134, 72, 134, 247, 13, 1, 9, 1 };

		private static byte[] signedData = new byte[] { 42, 134, 72, 134, 247, 13, 1, 7, 2 };
	}
}
