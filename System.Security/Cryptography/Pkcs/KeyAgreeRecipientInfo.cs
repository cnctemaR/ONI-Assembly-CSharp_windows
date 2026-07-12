using System;
using System.Threading;
using Internal.Cryptography;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class KeyAgreeRecipientInfo : RecipientInfo
	{
		internal KeyAgreeRecipientInfo(KeyAgreeRecipientInfoPal pal)
			: base(RecipientInfoType.KeyAgreement, pal)
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

		public SubjectIdentifierOrKey OriginatorIdentifierOrKey
		{
			get
			{
				SubjectIdentifierOrKey subjectIdentifierOrKey;
				if ((subjectIdentifierOrKey = this._lazyOriginatorIdentifierKey) == null)
				{
					subjectIdentifierOrKey = (this._lazyOriginatorIdentifierKey = this.Pal.OriginatorIdentifierOrKey);
				}
				return subjectIdentifierOrKey;
			}
		}

		public DateTime Date
		{
			get
			{
				if (this._lazyDate == null)
				{
					this._lazyDate = new DateTime?(this.Pal.Date);
					Interlocked.MemoryBarrier();
				}
				return this._lazyDate.Value;
			}
		}

		public CryptographicAttributeObject OtherKeyAttribute
		{
			get
			{
				CryptographicAttributeObject cryptographicAttributeObject;
				if ((cryptographicAttributeObject = this._lazyOtherKeyAttribute) == null)
				{
					cryptographicAttributeObject = (this._lazyOtherKeyAttribute = this.Pal.OtherKeyAttribute);
				}
				return cryptographicAttributeObject;
			}
		}

		private new KeyAgreeRecipientInfoPal Pal
		{
			get
			{
				return (KeyAgreeRecipientInfoPal)base.Pal;
			}
		}

		internal KeyAgreeRecipientInfo()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private volatile SubjectIdentifier _lazyRecipientIdentifier;

		private volatile AlgorithmIdentifier _lazyKeyEncryptionAlgorithm;

		private volatile byte[] _lazyEncryptedKey;

		private volatile SubjectIdentifierOrKey _lazyOriginatorIdentifierKey;

		private DateTime? _lazyDate;

		private volatile CryptographicAttributeObject _lazyOtherKeyAttribute;
	}
}
