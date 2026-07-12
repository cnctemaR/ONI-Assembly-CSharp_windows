using System;
using Internal.Cryptography;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	public abstract class RecipientInfo
	{
		internal RecipientInfo(RecipientInfoType type, RecipientInfoPal pal)
		{
			this.Type = type;
			this.Pal = pal;
		}

		public RecipientInfoType Type { get; }

		public abstract int Version { get; }

		public abstract SubjectIdentifier RecipientIdentifier { get; }

		public abstract AlgorithmIdentifier KeyEncryptionAlgorithm { get; }

		public abstract byte[] EncryptedKey { get; }

		internal RecipientInfoPal Pal { get; }

		internal RecipientInfo()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
