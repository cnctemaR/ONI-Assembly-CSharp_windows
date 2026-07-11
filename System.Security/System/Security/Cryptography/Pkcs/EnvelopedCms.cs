using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using Mono.Security;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class EnvelopedCms
	{
		public EnvelopedCms()
		{
			this._certs = new X509Certificate2Collection();
			this._recipients = new RecipientInfoCollection();
			this._uattribs = new CryptographicAttributeObjectCollection();
		}

		public EnvelopedCms(ContentInfo content)
			: this()
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			this._content = content;
		}

		public EnvelopedCms(ContentInfo contentInfo, AlgorithmIdentifier encryptionAlgorithm)
			: this(contentInfo)
		{
			if (encryptionAlgorithm == null)
			{
				throw new ArgumentNullException("encryptionAlgorithm");
			}
			this._identifier = encryptionAlgorithm;
		}

		public EnvelopedCms(SubjectIdentifierType recipientIdentifierType, ContentInfo contentInfo)
			: this(contentInfo)
		{
			this._idType = recipientIdentifierType;
			if (this._idType == SubjectIdentifierType.SubjectKeyIdentifier)
			{
				this._version = 2;
			}
		}

		public EnvelopedCms(SubjectIdentifierType recipientIdentifierType, ContentInfo contentInfo, AlgorithmIdentifier encryptionAlgorithm)
			: this(contentInfo, encryptionAlgorithm)
		{
			this._idType = recipientIdentifierType;
			if (this._idType == SubjectIdentifierType.SubjectKeyIdentifier)
			{
				this._version = 2;
			}
		}

		public X509Certificate2Collection Certificates
		{
			get
			{
				return this._certs;
			}
		}

		public AlgorithmIdentifier ContentEncryptionAlgorithm
		{
			get
			{
				if (this._identifier == null)
				{
					this._identifier = new AlgorithmIdentifier();
				}
				return this._identifier;
			}
		}

		public ContentInfo ContentInfo
		{
			get
			{
				if (this._content == null)
				{
					Oid oid = new Oid("1.2.840.113549.1.7.1");
					this._content = new ContentInfo(oid, new byte[0]);
				}
				return this._content;
			}
		}

		public RecipientInfoCollection RecipientInfos
		{
			get
			{
				return this._recipients;
			}
		}

		public CryptographicAttributeObjectCollection UnprotectedAttributes
		{
			get
			{
				return this._uattribs;
			}
		}

		public int Version
		{
			get
			{
				return this._version;
			}
		}

		private X509IssuerSerial GetIssuerSerial(string issuer, byte[] serial)
		{
			X509IssuerSerial x509IssuerSerial = default(X509IssuerSerial);
			x509IssuerSerial.IssuerName = issuer;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in serial)
			{
				stringBuilder.Append(b.ToString("X2"));
			}
			x509IssuerSerial.SerialNumber = stringBuilder.ToString();
			return x509IssuerSerial;
		}

		[MonoTODO]
		public void Decode(byte[] encodedMessage)
		{
			if (encodedMessage == null)
			{
				throw new ArgumentNullException("encodedMessage");
			}
			PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(encodedMessage);
			if (contentInfo.ContentType != "1.2.840.113549.1.7.3")
			{
				throw new Exception(string.Empty);
			}
			PKCS7.EnvelopedData envelopedData = new PKCS7.EnvelopedData(contentInfo.Content);
			Oid oid = new Oid(envelopedData.ContentInfo.ContentType);
			this._content = new ContentInfo(oid, new byte[0]);
			foreach (object obj in envelopedData.RecipientInfos)
			{
				PKCS7.RecipientInfo recipientInfo = (PKCS7.RecipientInfo)obj;
				Oid oid2 = new Oid(recipientInfo.Oid);
				AlgorithmIdentifier algorithmIdentifier = new AlgorithmIdentifier(oid2);
				SubjectIdentifier subjectIdentifier = null;
				if (recipientInfo.SubjectKeyIdentifier != null)
				{
					subjectIdentifier = new SubjectIdentifier(SubjectIdentifierType.SubjectKeyIdentifier, recipientInfo.SubjectKeyIdentifier);
				}
				else if (recipientInfo.Issuer != null && recipientInfo.Serial != null)
				{
					X509IssuerSerial issuerSerial = this.GetIssuerSerial(recipientInfo.Issuer, recipientInfo.Serial);
					subjectIdentifier = new SubjectIdentifier(SubjectIdentifierType.IssuerAndSerialNumber, issuerSerial);
				}
				KeyTransRecipientInfo keyTransRecipientInfo = new KeyTransRecipientInfo(recipientInfo.Key, algorithmIdentifier, subjectIdentifier, recipientInfo.Version);
				this._recipients.Add(keyTransRecipientInfo);
			}
			this._version = (int)envelopedData.Version;
		}

		[MonoTODO]
		public void Decrypt()
		{
			throw new InvalidOperationException("not encrypted");
		}

		[MonoTODO]
		public void Decrypt(RecipientInfo recipientInfo)
		{
			if (recipientInfo == null)
			{
				throw new ArgumentNullException("recipientInfo");
			}
			this.Decrypt();
		}

		[MonoTODO]
		public void Decrypt(RecipientInfo recipientInfo, X509Certificate2Collection extraStore)
		{
			if (recipientInfo == null)
			{
				throw new ArgumentNullException("recipientInfo");
			}
			if (extraStore == null)
			{
				throw new ArgumentNullException("extraStore");
			}
			this.Decrypt();
		}

		[MonoTODO]
		public void Decrypt(X509Certificate2Collection extraStore)
		{
			if (extraStore == null)
			{
				throw new ArgumentNullException("extraStore");
			}
			this.Decrypt();
		}

		[MonoTODO]
		public byte[] Encode()
		{
			throw new InvalidOperationException("not encrypted");
		}

		[MonoTODO]
		public void Encrypt()
		{
			if (this._content == null || this._content.Content == null || this._content.Content.Length == 0)
			{
				throw new CryptographicException("no content to encrypt");
			}
		}

		[MonoTODO]
		public void Encrypt(CmsRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			this.Encrypt();
		}

		[MonoTODO]
		public void Encrypt(CmsRecipientCollection recipients)
		{
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
		}

		private ContentInfo _content;

		private AlgorithmIdentifier _identifier;

		private X509Certificate2Collection _certs;

		private RecipientInfoCollection _recipients;

		private CryptographicAttributeObjectCollection _uattribs;

		private SubjectIdentifierType _idType;

		private int _version;
	}
}
