using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class DES : SymmetricAlgorithm
	{
		protected DES()
		{
			this.KeySizeValue = 64;
			this.BlockSizeValue = 64;
			this.FeedbackSizeValue = 8;
			this.LegalKeySizesValue = new KeySizes[1];
			this.LegalKeySizesValue[0] = new KeySizes(64, 64, 0);
			this.LegalBlockSizesValue = new KeySizes[1];
			this.LegalBlockSizesValue[0] = new KeySizes(64, 64, 0);
		}

		public new static DES Create()
		{
			return DES.Create("System.Security.Cryptography.DES");
		}

		public new static DES Create(string algName)
		{
			return (DES)CryptoConfig.CreateFromName(algName);
		}

		public static bool IsWeakKey(byte[] rgbKey)
		{
			if (rgbKey == null)
			{
				throw new CryptographicException(Locale.GetText("Null Key"));
			}
			if (rgbKey.Length != 8)
			{
				throw new CryptographicException(Locale.GetText("Wrong Key Length"));
			}
			for (int i = 0; i < rgbKey.Length; i++)
			{
				int num = (int)(rgbKey[i] | 17);
				if (num != 17 && num != 31 && num != 241 && num != 255)
				{
					return false;
				}
			}
			for (int j = 0; j < DES.weakKeys.Length >> 3; j++)
			{
				int k;
				for (k = 0; k < rgbKey.Length; k++)
				{
					if ((rgbKey[k] ^ DES.weakKeys[j, k]) > 1)
					{
						break;
					}
				}
				if (k == 8)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsSemiWeakKey(byte[] rgbKey)
		{
			if (rgbKey == null)
			{
				throw new CryptographicException(Locale.GetText("Null Key"));
			}
			if (rgbKey.Length != 8)
			{
				throw new CryptographicException(Locale.GetText("Wrong Key Length"));
			}
			for (int i = 0; i < rgbKey.Length; i++)
			{
				int num = (int)(rgbKey[i] | 17);
				if (num != 17 && num != 31 && num != 241 && num != 255)
				{
					return false;
				}
			}
			for (int j = 0; j < DES.semiWeakKeys.Length >> 3; j++)
			{
				int k;
				for (k = 0; k < rgbKey.Length; k++)
				{
					if ((rgbKey[k] ^ DES.semiWeakKeys[j, k]) > 1)
					{
						break;
					}
				}
				if (k == 8)
				{
					return true;
				}
			}
			return false;
		}

		public override byte[] Key
		{
			get
			{
				if (this.KeyValue == null)
				{
					this.GenerateKey();
				}
				return (byte[])this.KeyValue.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Key");
				}
				if (value.Length != 8)
				{
					throw new ArgumentException(Locale.GetText("Wrong Key Length"));
				}
				if (DES.IsWeakKey(value))
				{
					throw new CryptographicException(Locale.GetText("Weak Key"));
				}
				if (DES.IsSemiWeakKey(value))
				{
					throw new CryptographicException(Locale.GetText("Semi Weak Key"));
				}
				this.KeyValue = (byte[])value.Clone();
			}
		}

		private const int keySizeByte = 8;

		internal static readonly byte[,] weakKeys = new byte[,]
		{
			{ 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 31, 31, 31, 31, 15, 15, 15, 15 },
			{ 225, 225, 225, 225, 241, 241, 241, 241 },
			{ byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue }
		};

		internal static readonly byte[,] semiWeakKeys = new byte[,]
		{
			{ 0, 30, 0, 30, 0, 14, 0, 14 },
			{ 0, 224, 0, 224, 0, 240, 0, 240 },
			{ 0, 254, 0, 254, 0, 254, 0, 254 },
			{ 30, 0, 30, 0, 14, 0, 14, 0 },
			{ 30, 224, 30, 224, 14, 240, 14, 240 },
			{ 30, 254, 30, 254, 14, 254, 14, 254 },
			{ 224, 0, 224, 0, 240, 0, 240, 0 },
			{ 224, 30, 224, 30, 240, 14, 240, 14 },
			{ 224, 254, 224, 254, 240, 254, 240, 254 },
			{ 254, 0, 254, 0, 254, 0, 254, 0 },
			{ 254, 30, 254, 30, 254, 14, 254, 14 },
			{ 254, 224, 254, 224, 254, 240, 254, 240 }
		};
	}
}
