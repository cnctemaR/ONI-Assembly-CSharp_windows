using System;
using System.IO;
using System.Text;
using Mono.Security;
using Mono.Security.Cryptography;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	public class X509Certificate2 : X509Certificate
	{
		public X509Certificate2()
		{
			this._cert = null;
		}

		public X509Certificate2(byte[] rawData)
		{
			this.Import(rawData, null, X509KeyStorageFlags.DefaultKeySet);
		}

		public X509Certificate2(byte[] rawData, string password)
		{
			this.Import(rawData, password, X509KeyStorageFlags.DefaultKeySet);
		}

		public X509Certificate2(byte[] rawData, SecureString password)
		{
			this.Import(rawData, password, X509KeyStorageFlags.DefaultKeySet);
		}

		public X509Certificate2(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Import(rawData, password, keyStorageFlags);
		}

		public X509Certificate2(byte[] rawData, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Import(rawData, password, keyStorageFlags);
		}

		public X509Certificate2(string fileName)
		{
			this.Import(fileName, string.Empty, X509KeyStorageFlags.DefaultKeySet);
		}

		public X509Certificate2(string fileName, string password)
		{
			this.Import(fileName, password, X509KeyStorageFlags.DefaultKeySet);
		}

		public X509Certificate2(string fileName, SecureString password)
		{
			this.Import(fileName, password, X509KeyStorageFlags.DefaultKeySet);
		}

		public X509Certificate2(string fileName, string password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Import(fileName, password, keyStorageFlags);
		}

		public X509Certificate2(string fileName, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Import(fileName, password, keyStorageFlags);
		}

		public X509Certificate2(IntPtr handle)
			: base(handle)
		{
			this._cert = new X509Certificate(base.GetRawCertData());
		}

		public X509Certificate2(X509Certificate certificate)
			: base(certificate)
		{
			this._cert = new X509Certificate(base.GetRawCertData());
		}

		public bool Archived
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				return this._archived;
			}
			set
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				this._archived = value;
			}
		}

		public X509ExtensionCollection Extensions
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				if (this._extensions == null)
				{
					this._extensions = new X509ExtensionCollection(this._cert);
				}
				return this._extensions;
			}
		}

		public string FriendlyName
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				return this._name;
			}
			set
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				this._name = value;
			}
		}

		public bool HasPrivateKey
		{
			get
			{
				return this.PrivateKey != null;
			}
		}

		public X500DistinguishedName IssuerName
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				if (this.issuer_name == null)
				{
					this.issuer_name = new X500DistinguishedName(this._cert.GetIssuerName().GetBytes());
				}
				return this.issuer_name;
			}
		}

		public DateTime NotAfter
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				return this._cert.ValidUntil.ToLocalTime();
			}
		}

		public DateTime NotBefore
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				return this._cert.ValidFrom.ToLocalTime();
			}
		}

		public AsymmetricAlgorithm PrivateKey
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				try
				{
					if (this._cert.RSA != null)
					{
						RSACryptoServiceProvider rsacryptoServiceProvider = this._cert.RSA as RSACryptoServiceProvider;
						if (rsacryptoServiceProvider != null)
						{
							return (!rsacryptoServiceProvider.PublicOnly) ? rsacryptoServiceProvider : null;
						}
						RSAManaged rsamanaged = this._cert.RSA as RSAManaged;
						if (rsamanaged != null)
						{
							return (!rsamanaged.PublicOnly) ? rsamanaged : null;
						}
						this._cert.RSA.ExportParameters(true);
						return this._cert.RSA;
					}
					else if (this._cert.DSA != null)
					{
						DSACryptoServiceProvider dsacryptoServiceProvider = this._cert.DSA as DSACryptoServiceProvider;
						if (dsacryptoServiceProvider != null)
						{
							return (!dsacryptoServiceProvider.PublicOnly) ? dsacryptoServiceProvider : null;
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
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				if (value == null)
				{
					this._cert.RSA = null;
					this._cert.DSA = null;
				}
				else if (value is RSA)
				{
					this._cert.RSA = (RSA)value;
				}
				else
				{
					if (!(value is DSA))
					{
						throw new NotSupportedException();
					}
					this._cert.DSA = (DSA)value;
				}
			}
		}

		public PublicKey PublicKey
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				if (this._publicKey == null)
				{
					try
					{
						this._publicKey = new PublicKey(this._cert);
					}
					catch (Exception ex)
					{
						string text = global::Locale.GetText("Unable to decode public key.");
						throw new CryptographicException(text, ex);
					}
				}
				return this._publicKey;
			}
		}

		public byte[] RawData
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				return base.GetRawCertData();
			}
		}

		public string SerialNumber
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				if (this._serial == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					byte[] serialNumber = this._cert.SerialNumber;
					for (int i = serialNumber.Length - 1; i >= 0; i--)
					{
						stringBuilder.Append(serialNumber[i].ToString("X2"));
					}
					this._serial = stringBuilder.ToString();
				}
				return this._serial;
			}
		}

		public Oid SignatureAlgorithm
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				if (this.signature_algorithm == null)
				{
					this.signature_algorithm = new Oid(this._cert.SignatureAlgorithm);
				}
				return this.signature_algorithm;
			}
		}

		public X500DistinguishedName SubjectName
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				if (this.subject_name == null)
				{
					this.subject_name = new X500DistinguishedName(this._cert.GetSubjectName().GetBytes());
				}
				return this.subject_name;
			}
		}

		public string Thumbprint
		{
			get
			{
				return base.GetCertHashString();
			}
		}

		public int Version
		{
			get
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				return this._cert.Version;
			}
		}

		[global::System.MonoTODO("always return String.Empty for UpnName, DnsFromAlternativeName and UrlName")]
		public string GetNameInfo(X509NameType nameType, bool forIssuer)
		{
			switch (nameType)
			{
			case X509NameType.SimpleName:
			{
				if (this._cert == null)
				{
					throw new CryptographicException(X509Certificate2.empty_error);
				}
				ASN1 asn = ((!forIssuer) ? this._cert.GetSubjectName() : this._cert.GetIssuerName());
				ASN1 asn2 = this.Find(X509Certificate2.commonName, asn);
				if (asn2 != null)
				{
					return this.GetValueAsString(asn2);
				}
				if (asn.Count == 0)
				{
					return string.Empty;
				}
				ASN1 asn3 = asn[asn.Count - 1];
				if (asn3.Count == 0)
				{
					return string.Empty;
				}
				return this.GetValueAsString(asn3[0]);
			}
			case X509NameType.EmailName:
			{
				ASN1 asn4 = this.Find(X509Certificate2.email, (!forIssuer) ? this._cert.GetSubjectName() : this._cert.GetIssuerName());
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
				ASN1 asn5 = this.Find(X509Certificate2.commonName, (!forIssuer) ? this._cert.GetSubjectName() : this._cert.GetIssuerName());
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

		private ASN1 Find(byte[] oid, ASN1 dn)
		{
			if (dn.Count == 0)
			{
				return null;
			}
			for (int i = 0; i < dn.Count; i++)
			{
				ASN1 asn = dn[i];
				for (int j = 0; j < asn.Count; j++)
				{
					ASN1 asn2 = asn[j];
					if (asn2.Count == 2)
					{
						ASN1 asn3 = asn2[0];
						if (asn3 != null)
						{
							if (asn3.CompareValue(oid))
							{
								return asn2;
							}
						}
					}
				}
			}
			return null;
		}

		private string GetValueAsString(ASN1 pair)
		{
			if (pair.Count != 2)
			{
				return string.Empty;
			}
			ASN1 asn = pair[1];
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

		private void ImportPkcs12(byte[] rawData, string password)
		{
			PKCS12 pkcs = ((password != null) ? new PKCS12(rawData, password) : new PKCS12(rawData));
			if (pkcs.Certificates.Count > 0)
			{
				this._cert = pkcs.Certificates[0];
			}
			else
			{
				this._cert = null;
			}
			if (pkcs.Keys.Count > 0)
			{
				this._cert.RSA = pkcs.Keys[0] as RSA;
				this._cert.DSA = pkcs.Keys[0] as DSA;
			}
		}

		public override void Import(byte[] rawData)
		{
			this.Import(rawData, null, X509KeyStorageFlags.DefaultKeySet);
		}

		[global::System.MonoTODO("missing KeyStorageFlags support")]
		public override void Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
		{
			base.Import(rawData, password, keyStorageFlags);
			if (password == null)
			{
				try
				{
					this._cert = new X509Certificate(rawData);
				}
				catch (Exception ex)
				{
					try
					{
						this.ImportPkcs12(rawData, null);
					}
					catch
					{
						string text = global::Locale.GetText("Unable to decode certificate.");
						throw new CryptographicException(text, ex);
					}
				}
			}
			else
			{
				try
				{
					this.ImportPkcs12(rawData, password);
				}
				catch
				{
					this._cert = new X509Certificate(rawData);
				}
			}
		}

		[global::System.MonoTODO("SecureString is incomplete")]
		public override void Import(byte[] rawData, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Import(rawData, null, keyStorageFlags);
		}

		public override void Import(string fileName)
		{
			byte[] array = X509Certificate2.Load(fileName);
			this.Import(array, null, X509KeyStorageFlags.DefaultKeySet);
		}

		[global::System.MonoTODO("missing KeyStorageFlags support")]
		public override void Import(string fileName, string password, X509KeyStorageFlags keyStorageFlags)
		{
			byte[] array = X509Certificate2.Load(fileName);
			this.Import(array, password, keyStorageFlags);
		}

		[global::System.MonoTODO("SecureString is incomplete")]
		public override void Import(string fileName, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			byte[] array = X509Certificate2.Load(fileName);
			this.Import(array, null, keyStorageFlags);
		}

		private static byte[] Load(string fileName)
		{
			byte[] array = null;
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				fileStream.Close();
			}
			return array;
		}

		public override void Reset()
		{
			this._cert = null;
			this._archived = false;
			this._extensions = null;
			this._name = string.Empty;
			this._serial = null;
			this._publicKey = null;
			this.issuer_name = null;
			this.subject_name = null;
			this.signature_algorithm = null;
			base.Reset();
		}

		public override string ToString()
		{
			if (this._cert == null)
			{
				return "System.Security.Cryptography.X509Certificates.X509Certificate2";
			}
			return base.ToString(true);
		}

		public override string ToString(bool verbose)
		{
			if (this._cert == null)
			{
				return "System.Security.Cryptography.X509Certificates.X509Certificate2";
			}
			if (!verbose)
			{
				return base.ToString(true);
			}
			string newLine = Environment.NewLine;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[Version]{0}  V{1}{0}{0}", newLine, this.Version);
			stringBuilder.AppendFormat("[Subject]{0}  {1}{0}{0}", newLine, base.Subject);
			stringBuilder.AppendFormat("[Issuer]{0}  {1}{0}{0}", newLine, base.Issuer);
			stringBuilder.AppendFormat("[Serial Number]{0}  {1}{0}{0}", newLine, this.SerialNumber);
			stringBuilder.AppendFormat("[Not Before]{0}  {1}{0}{0}", newLine, this.NotBefore);
			stringBuilder.AppendFormat("[Not After]{0}  {1}{0}{0}", newLine, this.NotAfter);
			stringBuilder.AppendFormat("[Thumbprint]{0}  {1}{0}{0}", newLine, this.Thumbprint);
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
			X509Certificate2.AppendBuffer(stringBuilder, this.PublicKey.EncodedKeyValue.RawData);
			stringBuilder.AppendFormat("{0}  Parameters: ", newLine);
			X509Certificate2.AppendBuffer(stringBuilder, this.PublicKey.EncodedParameters.RawData);
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

		[global::System.MonoTODO("by default this depends on the incomplete X509Chain")]
		public bool Verify()
		{
			if (this._cert == null)
			{
				throw new CryptographicException(X509Certificate2.empty_error);
			}
			X509Chain x509Chain = (X509Chain)CryptoConfig.CreateFromName("X509Chain");
			return x509Chain.Build(this);
		}

		[global::System.MonoTODO("Detection limited to Cert, Pfx, Pkcs12, Pkcs7 and Unknown")]
		public static X509ContentType GetCertContentType(byte[] rawData)
		{
			if (rawData == null || rawData.Length == 0)
			{
				throw new ArgumentException("rawData");
			}
			X509ContentType x509ContentType = X509ContentType.Unknown;
			try
			{
				ASN1 asn = new ASN1(rawData);
				if (asn.Tag != 48)
				{
					string text = global::Locale.GetText("Unable to decode certificate.");
					throw new CryptographicException(text);
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
						if (tag == 48)
						{
							if (asn[1].Tag == 48 && asn[2].Tag == 3)
							{
								x509ContentType = X509ContentType.Cert;
							}
						}
					}
					else if (asn[1].Tag == 48 && asn[2].Tag == 48)
					{
						x509ContentType = X509ContentType.Pfx;
					}
				}
				if (asn[0].Tag == 6 && asn[0].CompareValue(X509Certificate2.signedData))
				{
					x509ContentType = X509ContentType.Pkcs7;
				}
			}
			catch (Exception ex)
			{
				string text2 = global::Locale.GetText("Unable to decode certificate.");
				throw new CryptographicException(text2, ex);
			}
			return x509ContentType;
		}

		[global::System.MonoTODO("Detection limited to Cert, Pfx, Pkcs12 and Unknown")]
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
			byte[] array = X509Certificate2.Load(fileName);
			return X509Certificate2.GetCertContentType(array);
		}

		internal X509Certificate MonoCertificate
		{
			get
			{
				return this._cert;
			}
		}

		private bool _archived;

		private X509ExtensionCollection _extensions;

		private string _name = string.Empty;

		private string _serial;

		private PublicKey _publicKey;

		private X500DistinguishedName issuer_name;

		private X500DistinguishedName subject_name;

		private Oid signature_algorithm;

		private X509Certificate _cert;

		private static string empty_error = global::Locale.GetText("Certificate instance is empty.");

		private static byte[] commonName = new byte[] { 85, 4, 3 };

		private static byte[] email = new byte[] { 42, 134, 72, 134, 247, 13, 1, 9, 1 };

		private static byte[] signedData = new byte[] { 42, 134, 72, 134, 247, 13, 1, 7, 2 };
	}
}
