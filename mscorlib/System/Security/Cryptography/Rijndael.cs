using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class Rijndael : SymmetricAlgorithm
	{
		protected Rijndael()
		{
			this.KeySizeValue = 256;
			this.BlockSizeValue = 128;
			this.FeedbackSizeValue = 128;
			this.LegalKeySizesValue = new KeySizes[1];
			this.LegalKeySizesValue[0] = new KeySizes(128, 256, 64);
			this.LegalBlockSizesValue = new KeySizes[1];
			this.LegalBlockSizesValue[0] = new KeySizes(128, 256, 64);
		}

		public new static Rijndael Create()
		{
			return Rijndael.Create("System.Security.Cryptography.Rijndael");
		}

		public new static Rijndael Create(string algName)
		{
			return (Rijndael)CryptoConfig.CreateFromName(algName);
		}
	}
}
