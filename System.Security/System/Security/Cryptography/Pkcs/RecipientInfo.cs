using System;

namespace System.Security.Cryptography.Pkcs
{
	public abstract class RecipientInfo
	{
		internal RecipientInfo(RecipientInfoType recipInfoType)
		{
			this._type = recipInfoType;
		}

		public abstract byte[] EncryptedKey { get; }

		public abstract AlgorithmIdentifier KeyEncryptionAlgorithm { get; }

		public abstract SubjectIdentifier RecipientIdentifier { get; }

		public RecipientInfoType Type
		{
			get
			{
				return this._type;
			}
		}

		public abstract int Version { get; }

		private RecipientInfoType _type;
	}
}
