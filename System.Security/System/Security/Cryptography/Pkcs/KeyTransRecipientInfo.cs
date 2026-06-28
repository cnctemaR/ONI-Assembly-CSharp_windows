using System;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class KeyTransRecipientInfo : RecipientInfo
	{
		internal KeyTransRecipientInfo(byte[] encryptedKey, AlgorithmIdentifier keyEncryptionAlgorithm, SubjectIdentifier recipientIdentifier, int version)
			: base(RecipientInfoType.KeyTransport)
		{
			this._encryptedKey = encryptedKey;
			this._keyEncryptionAlgorithm = keyEncryptionAlgorithm;
			this._recipientIdentifier = recipientIdentifier;
			this._version = version;
		}

		public override byte[] EncryptedKey
		{
			get
			{
				return this._encryptedKey;
			}
		}

		public override AlgorithmIdentifier KeyEncryptionAlgorithm
		{
			get
			{
				return this._keyEncryptionAlgorithm;
			}
		}

		public override SubjectIdentifier RecipientIdentifier
		{
			get
			{
				return this._recipientIdentifier;
			}
		}

		public override int Version
		{
			get
			{
				return this._version;
			}
		}

		private byte[] _encryptedKey;

		private AlgorithmIdentifier _keyEncryptionAlgorithm;

		private SubjectIdentifier _recipientIdentifier;

		private int _version;
	}
}
