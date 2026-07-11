using System;

namespace System.Security.Cryptography
{
	public abstract class Aes : SymmetricAlgorithm
	{
		protected Aes()
		{
			this.KeySizeValue = 256;
			this.BlockSizeValue = 128;
			this.FeedbackSizeValue = 128;
			this.LegalKeySizesValue = new KeySizes[1];
			this.LegalKeySizesValue[0] = new KeySizes(128, 256, 64);
			this.LegalBlockSizesValue = new KeySizes[1];
			this.LegalBlockSizesValue[0] = new KeySizes(128, 128, 0);
		}

		public new static Aes Create()
		{
			return Aes.Create("System.Security.Cryptography.AesManaged, System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
		}

		public new static Aes Create(string algName)
		{
			return (Aes)CryptoConfig.CreateFromName(algName);
		}
	}
}
