using System;
using System.Security.Cryptography.X509Certificates;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class EnvelopedCms
	{
		public EnvelopedCms()
			: this(new ContentInfo(Array.Empty<byte>()))
		{
		}

		public EnvelopedCms(ContentInfo contentInfo)
			: this(contentInfo, new AlgorithmIdentifier(Oid.FromOidValue("1.2.840.113549.3.7", OidGroup.EncryptionAlgorithm)))
		{
		}

		public EnvelopedCms(ContentInfo contentInfo, AlgorithmIdentifier encryptionAlgorithm)
		{
			if (contentInfo == null)
			{
				throw new ArgumentNullException("contentInfo");
			}
			if (encryptionAlgorithm == null)
			{
				throw new ArgumentNullException("encryptionAlgorithm");
			}
			this.Version = 0;
			this.ContentInfo = contentInfo;
			this.ContentEncryptionAlgorithm = encryptionAlgorithm;
			this.Certificates = new X509Certificate2Collection();
			this.UnprotectedAttributes = new CryptographicAttributeObjectCollection();
			this._decryptorPal = null;
			this._lastCall = EnvelopedCms.LastCall.Ctor;
		}

		public int Version { get; private set; }

		public ContentInfo ContentInfo { get; private set; }

		public AlgorithmIdentifier ContentEncryptionAlgorithm { get; private set; }

		public X509Certificate2Collection Certificates { get; private set; }

		public CryptographicAttributeObjectCollection UnprotectedAttributes { get; private set; }

		public RecipientInfoCollection RecipientInfos
		{
			get
			{
				switch (this._lastCall)
				{
				case EnvelopedCms.LastCall.Ctor:
					return new RecipientInfoCollection();
				case EnvelopedCms.LastCall.Encrypt:
					throw PkcsPal.Instance.CreateRecipientInfosAfterEncryptException();
				case EnvelopedCms.LastCall.Decode:
				case EnvelopedCms.LastCall.Decrypt:
					return this._decryptorPal.RecipientInfos;
				default:
					throw new InvalidOperationException();
				}
			}
		}

		public void Encrypt(CmsRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			this.Encrypt(new CmsRecipientCollection(recipient));
		}

		public void Encrypt(CmsRecipientCollection recipients)
		{
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			if (recipients.Count == 0)
			{
				throw new PlatformNotSupportedException("The recipients collection is empty. You must specify at least one recipient. This platform does not implement the certificate picker UI.");
			}
			if (this._decryptorPal != null)
			{
				this._decryptorPal.Dispose();
				this._decryptorPal = null;
			}
			this._encodedMessage = PkcsPal.Instance.Encrypt(recipients, this.ContentInfo, this.ContentEncryptionAlgorithm, this.Certificates, this.UnprotectedAttributes);
			this._lastCall = EnvelopedCms.LastCall.Encrypt;
		}

		public byte[] Encode()
		{
			if (this._encodedMessage == null)
			{
				throw new InvalidOperationException("The CMS message is not encrypted.");
			}
			return this._encodedMessage.CloneByteArray();
		}

		public void Decode(byte[] encodedMessage)
		{
			if (encodedMessage == null)
			{
				throw new ArgumentNullException("encodedMessage");
			}
			if (this._decryptorPal != null)
			{
				this._decryptorPal.Dispose();
				this._decryptorPal = null;
			}
			int num;
			ContentInfo contentInfo;
			AlgorithmIdentifier algorithmIdentifier;
			X509Certificate2Collection x509Certificate2Collection;
			CryptographicAttributeObjectCollection cryptographicAttributeObjectCollection;
			this._decryptorPal = PkcsPal.Instance.Decode(encodedMessage, out num, out contentInfo, out algorithmIdentifier, out x509Certificate2Collection, out cryptographicAttributeObjectCollection);
			this.Version = num;
			this.ContentInfo = contentInfo;
			this.ContentEncryptionAlgorithm = algorithmIdentifier;
			this.Certificates = x509Certificate2Collection;
			this.UnprotectedAttributes = cryptographicAttributeObjectCollection;
			this._encodedMessage = contentInfo.Content.CloneByteArray();
			this._lastCall = EnvelopedCms.LastCall.Decode;
		}

		public void Decrypt()
		{
			this.DecryptContent(this.RecipientInfos, null);
		}

		public void Decrypt(RecipientInfo recipientInfo)
		{
			if (recipientInfo == null)
			{
				throw new ArgumentNullException("recipientInfo");
			}
			this.DecryptContent(new RecipientInfoCollection(recipientInfo), null);
		}

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
			this.DecryptContent(new RecipientInfoCollection(recipientInfo), extraStore);
		}

		public void Decrypt(X509Certificate2Collection extraStore)
		{
			if (extraStore == null)
			{
				throw new ArgumentNullException("extraStore");
			}
			this.DecryptContent(this.RecipientInfos, extraStore);
		}

		private void DecryptContent(RecipientInfoCollection recipientInfos, X509Certificate2Collection extraStore)
		{
			switch (this._lastCall)
			{
			case EnvelopedCms.LastCall.Ctor:
				throw new InvalidOperationException("The CMS message is not encrypted.");
			case EnvelopedCms.LastCall.Encrypt:
				throw PkcsPal.Instance.CreateDecryptAfterEncryptException();
			case EnvelopedCms.LastCall.Decode:
			{
				extraStore = extraStore ?? new X509Certificate2Collection();
				X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
				PkcsPal.Instance.AddCertsFromStoreForDecryption(x509Certificate2Collection);
				x509Certificate2Collection.AddRange(extraStore);
				X509Certificate2Collection certificates = this.Certificates;
				ContentInfo contentInfo = null;
				Exception ex = PkcsPal.Instance.CreateRecipientsNotFoundException();
				foreach (RecipientInfo recipientInfo in recipientInfos)
				{
					X509Certificate2 x509Certificate = x509Certificate2Collection.TryFindMatchingCertificate(recipientInfo.RecipientIdentifier);
					if (x509Certificate == null)
					{
						ex = PkcsPal.Instance.CreateRecipientsNotFoundException();
					}
					else
					{
						contentInfo = this._decryptorPal.TryDecrypt(recipientInfo, x509Certificate, certificates, extraStore, out ex);
						if (ex == null)
						{
							break;
						}
					}
				}
				if (ex != null)
				{
					throw ex;
				}
				this.ContentInfo = contentInfo;
				this._encodedMessage = contentInfo.Content.CloneByteArray();
				this._lastCall = EnvelopedCms.LastCall.Decrypt;
				return;
			}
			case EnvelopedCms.LastCall.Decrypt:
				throw PkcsPal.Instance.CreateDecryptTwiceException();
			default:
				throw new InvalidOperationException();
			}
		}

		public EnvelopedCms(SubjectIdentifierType recipientIdentifierType, ContentInfo contentInfo)
			: this(contentInfo)
		{
			if (recipientIdentifierType == SubjectIdentifierType.SubjectKeyIdentifier)
			{
				this.Version = 2;
			}
		}

		public EnvelopedCms(SubjectIdentifierType recipientIdentifierType, ContentInfo contentInfo, AlgorithmIdentifier encryptionAlgorithm)
			: this(contentInfo, encryptionAlgorithm)
		{
			if (recipientIdentifierType == SubjectIdentifierType.SubjectKeyIdentifier)
			{
				this.Version = 2;
			}
		}

		public void Encrypt()
		{
			this.Encrypt(new CmsRecipientCollection());
		}

		private DecryptorPal _decryptorPal;

		private byte[] _encodedMessage;

		private EnvelopedCms.LastCall _lastCall;

		private enum LastCall
		{
			Ctor = 1,
			Encrypt,
			Decode,
			Decrypt
		}
	}
}
