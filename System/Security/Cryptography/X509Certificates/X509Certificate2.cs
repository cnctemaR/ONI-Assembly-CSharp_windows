using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Mono.Security;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	[Serializable]
	public class X509Certificate2 : global::System.Security.Cryptography.X509Certificates.X509Certificate
	{
		internal new X509Certificate2Impl Impl
		{
			get
			{
				X509Certificate2Impl x509Certificate2Impl = base.Impl as X509Certificate2Impl;
				X509Helper2.ThrowIfContextInvalid(x509Certificate2Impl);
				return x509Certificate2Impl;
			}
		}

		public X509Certificate2()
		{
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
			throw new NotImplementedException();
		}

		public X509Certificate2(global::System.Security.Cryptography.X509Certificates.X509Certificate certificate)
			: base(X509Helper2.Import(certificate, false))
		{
		}

		protected X509Certificate2(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		internal X509Certificate2(X509Certificate2Impl impl)
			: base(impl)
		{
		}

		public bool Archived
		{
			get
			{
				return this.Impl.Archived;
			}
			set
			{
				this.Impl.Archived = true;
			}
		}

		public global::System.Security.Cryptography.X509Certificates.X509ExtensionCollection Extensions
		{
			get
			{
				return this.Impl.Extensions;
			}
		}

		public string FriendlyName
		{
			get
			{
				base.ThrowIfContextInvalid();
				return this.friendlyName;
			}
			set
			{
				base.ThrowIfContextInvalid();
				this.friendlyName = value;
			}
		}

		public bool HasPrivateKey
		{
			get
			{
				return this.Impl.HasPrivateKey;
			}
		}

		public X500DistinguishedName IssuerName
		{
			get
			{
				return this.Impl.IssuerName;
			}
		}

		public DateTime NotAfter
		{
			get
			{
				return this.Impl.GetValidUntil().ToLocalTime();
			}
		}

		public DateTime NotBefore
		{
			get
			{
				return this.Impl.GetValidFrom().ToLocalTime();
			}
		}

		public AsymmetricAlgorithm PrivateKey
		{
			get
			{
				return this.Impl.PrivateKey;
			}
			set
			{
				this.Impl.PrivateKey = value;
			}
		}

		public PublicKey PublicKey
		{
			get
			{
				return this.Impl.PublicKey;
			}
		}

		public byte[] RawData
		{
			get
			{
				return this.GetRawCertData();
			}
		}

		public string SerialNumber
		{
			get
			{
				return this.GetSerialNumberString();
			}
		}

		public Oid SignatureAlgorithm
		{
			get
			{
				return this.Impl.SignatureAlgorithm;
			}
		}

		public X500DistinguishedName SubjectName
		{
			get
			{
				return this.Impl.SubjectName;
			}
		}

		public string Thumbprint
		{
			get
			{
				return this.GetCertHashString();
			}
		}

		public int Version
		{
			get
			{
				return this.Impl.Version;
			}
		}

		[MonoTODO("always return String.Empty for UpnName, DnsFromAlternativeName and UrlName")]
		public string GetNameInfo(X509NameType nameType, bool forIssuer)
		{
			return this.Impl.GetNameInfo(nameType, forIssuer);
		}

		public override void Import(byte[] rawData)
		{
			this.Import(rawData, null, X509KeyStorageFlags.DefaultKeySet);
		}

		[MonoTODO("missing KeyStorageFlags support")]
		public override void Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
		{
			X509Certificate2Impl x509Certificate2Impl = X509Helper2.Import(rawData, password, keyStorageFlags, false);
			base.ImportHandle(x509Certificate2Impl);
		}

		[MonoTODO("SecureString is incomplete")]
		public override void Import(byte[] rawData, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Import(rawData, null, keyStorageFlags);
		}

		public override void Import(string fileName)
		{
			byte[] array = File.ReadAllBytes(fileName);
			this.Import(array, null, X509KeyStorageFlags.DefaultKeySet);
		}

		[MonoTODO("missing KeyStorageFlags support")]
		public override void Import(string fileName, string password, X509KeyStorageFlags keyStorageFlags)
		{
			byte[] array = File.ReadAllBytes(fileName);
			this.Import(array, password, keyStorageFlags);
		}

		[MonoTODO("SecureString is incomplete")]
		public override void Import(string fileName, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			byte[] array = File.ReadAllBytes(fileName);
			this.Import(array, null, keyStorageFlags);
		}

		[MonoTODO("X509ContentType.SerializedCert is not supported")]
		public override byte[] Export(X509ContentType contentType, string password)
		{
			return this.Impl.Export(contentType, password);
		}

		public override void Reset()
		{
			this.friendlyName = string.Empty;
			base.Reset();
		}

		public override string ToString()
		{
			if (!base.IsValid)
			{
				return "System.Security.Cryptography.X509Certificates.X509Certificate2";
			}
			return base.ToString(true);
		}

		public override string ToString(bool verbose)
		{
			if (!base.IsValid)
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

		[MonoTODO("by default this depends on the incomplete X509Chain")]
		public bool Verify()
		{
			return this.Impl.Verify(this);
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
				if (asn[0].Tag == 6 && asn[0].CompareValue(X509Certificate2.signedData))
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
			return X509Certificate2.GetCertContentType(File.ReadAllBytes(fileName));
		}

		[MonoTODO("See comment in X509Helper2.GetMonoCertificate().")]
		internal Mono.Security.X509.X509Certificate MonoCertificate
		{
			get
			{
				return X509Helper2.GetMonoCertificate(this);
			}
		}

		private string friendlyName = string.Empty;

		private static byte[] signedData = new byte[] { 42, 134, 72, 134, 247, 13, 1, 7, 2 };
	}
}
