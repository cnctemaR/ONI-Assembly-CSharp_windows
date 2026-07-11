using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class SymmetricAlgorithm : IDisposable
	{
		protected SymmetricAlgorithm()
		{
			this.ModeValue = CipherMode.CBC;
			this.PaddingValue = PaddingMode.PKCS7;
			this.m_disposed = false;
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SymmetricAlgorithm()
		{
			this.Dispose(false);
		}

		public void Clear()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.m_disposed)
			{
				if (this.KeyValue != null)
				{
					Array.Clear(this.KeyValue, 0, this.KeyValue.Length);
					this.KeyValue = null;
				}
				if (disposing)
				{
				}
				this.m_disposed = true;
			}
		}

		public virtual int BlockSize
		{
			get
			{
				return this.BlockSizeValue;
			}
			set
			{
				if (!KeySizes.IsLegalKeySize(this.LegalBlockSizesValue, value))
				{
					throw new CryptographicException(Locale.GetText("block size not supported by algorithm"));
				}
				if (this.BlockSizeValue != value)
				{
					this.BlockSizeValue = value;
					this.IVValue = null;
				}
			}
		}

		public virtual int FeedbackSize
		{
			get
			{
				return this.FeedbackSizeValue;
			}
			set
			{
				if (value <= 0 || value > this.BlockSizeValue)
				{
					throw new CryptographicException(Locale.GetText("feedback size larger than block size"));
				}
				this.FeedbackSizeValue = value;
			}
		}

		public virtual byte[] IV
		{
			get
			{
				if (this.IVValue == null)
				{
					this.GenerateIV();
				}
				return (byte[])this.IVValue.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("IV");
				}
				if (value.Length << 3 != this.BlockSizeValue)
				{
					throw new CryptographicException(Locale.GetText("IV length is different than block size"));
				}
				this.IVValue = (byte[])value.Clone();
			}
		}

		public virtual byte[] Key
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
				int num = value.Length << 3;
				if (!KeySizes.IsLegalKeySize(this.LegalKeySizesValue, num))
				{
					throw new CryptographicException(Locale.GetText("Key size not supported by algorithm"));
				}
				this.KeySizeValue = num;
				this.KeyValue = (byte[])value.Clone();
			}
		}

		public virtual int KeySize
		{
			get
			{
				return this.KeySizeValue;
			}
			set
			{
				if (!KeySizes.IsLegalKeySize(this.LegalKeySizesValue, value))
				{
					throw new CryptographicException(Locale.GetText("Key size not supported by algorithm"));
				}
				this.KeySizeValue = value;
				this.KeyValue = null;
			}
		}

		public virtual KeySizes[] LegalBlockSizes
		{
			get
			{
				return this.LegalBlockSizesValue;
			}
		}

		public virtual KeySizes[] LegalKeySizes
		{
			get
			{
				return this.LegalKeySizesValue;
			}
		}

		public virtual CipherMode Mode
		{
			get
			{
				return this.ModeValue;
			}
			set
			{
				if (!Enum.IsDefined(this.ModeValue.GetType(), value))
				{
					throw new CryptographicException(Locale.GetText("Cipher mode not available"));
				}
				this.ModeValue = value;
			}
		}

		public virtual PaddingMode Padding
		{
			get
			{
				return this.PaddingValue;
			}
			set
			{
				if (!Enum.IsDefined(this.PaddingValue.GetType(), value))
				{
					throw new CryptographicException(Locale.GetText("Padding mode not available"));
				}
				this.PaddingValue = value;
			}
		}

		public virtual ICryptoTransform CreateDecryptor()
		{
			return this.CreateDecryptor(this.Key, this.IV);
		}

		public abstract ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV);

		public virtual ICryptoTransform CreateEncryptor()
		{
			return this.CreateEncryptor(this.Key, this.IV);
		}

		public abstract ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV);

		public abstract void GenerateIV();

		public abstract void GenerateKey();

		public bool ValidKeySize(int bitLength)
		{
			return KeySizes.IsLegalKeySize(this.LegalKeySizesValue, bitLength);
		}

		public static SymmetricAlgorithm Create()
		{
			return SymmetricAlgorithm.Create("System.Security.Cryptography.SymmetricAlgorithm");
		}

		public static SymmetricAlgorithm Create(string algName)
		{
			return (SymmetricAlgorithm)CryptoConfig.CreateFromName(algName);
		}

		protected int BlockSizeValue;

		protected byte[] IVValue;

		protected int KeySizeValue;

		protected byte[] KeyValue;

		protected KeySizes[] LegalBlockSizesValue;

		protected KeySizes[] LegalKeySizesValue;

		protected int FeedbackSizeValue;

		protected CipherMode ModeValue;

		protected PaddingMode PaddingValue;

		private bool m_disposed;
	}
}
