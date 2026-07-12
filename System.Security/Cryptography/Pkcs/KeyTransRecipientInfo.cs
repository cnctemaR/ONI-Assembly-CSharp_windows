using System;
using Internal.Cryptography;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class KeyTransRecipientInfo : RecipientInfo
	{
		internal KeyTransRecipientInfo(KeyTransRecipientInfoPal pal)
			: base(RecipientInfoType.KeyTransport, pal)
		{
		}

		public override int Version
		{
			get
			{
				return this.Pal.Version;
			}
		}

		public override SubjectIdentifier RecipientIdentifier
		{
			get
			{
				SubjectIdentifier subjectIdentifier;
				if ((subjectIdentifier = this._lazyRecipientIdentifier) == null)
				{
					subjectIdentifier = (this._lazyRecipientIdentifier = this.Pal.RecipientIdentifier);
				}
				return subjectIdentifier;
			}
		}

		public override AlgorithmIdentifier KeyEncryptionAlgorithm
		{
			get
			{
				AlgorithmIdentifier algorithmIdentifier;
				if ((algorithmIdentifier = this._lazyKeyEncryptionAlgorithm) == null)
				{
					algorithmIdentifier = (this._lazyKeyEncryptionAlgorithm = this.Pal.KeyEncryptionAlgorithm);
				}
				return algorithmIdentifier;
			}
		}

		public override byte[] EncryptedKey
		{
			get
			{
				byte[] array;
				if ((array = this._lazyEncryptedKey) == null)
				{
					array = (this._lazyEncryptedKey = this.Pal.EncryptedKey);
				}
				return array;
			}
		}

		private new KeyTransRecipientInfoPal Pal
		{
			get
			{
				return (KeyTransRecipientInfoPal)base.Pal;
			}
		}

		internal KeyTransRecipientInfo()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private volatile SubjectIdentifier _lazyRecipientIdentifier;

		private volatile AlgorithmIdentifier _lazyKeyEncryptionAlgorithm;

		private volatile byte[] _lazyEncryptedKey;
	}
}
