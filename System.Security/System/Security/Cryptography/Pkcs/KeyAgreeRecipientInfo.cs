using System;

namespace System.Security.Cryptography.Pkcs
{
	[MonoTODO]
	public sealed class KeyAgreeRecipientInfo : RecipientInfo
	{
		internal KeyAgreeRecipientInfo()
			: base(RecipientInfoType.KeyAgreement)
		{
		}

		public DateTime Date
		{
			get
			{
				return DateTime.MinValue;
			}
		}

		public override byte[] EncryptedKey
		{
			get
			{
				return null;
			}
		}

		public override AlgorithmIdentifier KeyEncryptionAlgorithm
		{
			get
			{
				return null;
			}
		}

		public SubjectIdentifierOrKey OriginatorIdentifierOrKey
		{
			get
			{
				return null;
			}
		}

		public CryptographicAttributeObject OtherKeyAttribute
		{
			get
			{
				return null;
			}
		}

		public override SubjectIdentifier RecipientIdentifier
		{
			get
			{
				return null;
			}
		}

		public override int Version
		{
			get
			{
				return 0;
			}
		}
	}
}
