using System;
using System.Security.Permissions;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class AesCryptoServiceProvider : Aes
	{
		public AesCryptoServiceProvider()
		{
			this.FeedbackSizeValue = 8;
		}

		public override void GenerateIV()
		{
			this.IVValue = KeyBuilder.IV(this.BlockSizeValue >> 3);
		}

		public override void GenerateKey()
		{
			this.KeyValue = KeyBuilder.Key(this.KeySizeValue >> 3);
		}

		public override ICryptoTransform CreateDecryptor(byte[] key, byte[] iv)
		{
			if (this.Mode == CipherMode.CFB && this.FeedbackSize > 64)
			{
				throw new CryptographicException("CFB with Feedbaack > 64 bits");
			}
			return new AesTransform(this, false, key, iv);
		}

		public override ICryptoTransform CreateEncryptor(byte[] key, byte[] iv)
		{
			if (this.Mode == CipherMode.CFB && this.FeedbackSize > 64)
			{
				throw new CryptographicException("CFB with Feedbaack > 64 bits");
			}
			return new AesTransform(this, true, key, iv);
		}

		public override byte[] IV
		{
			get
			{
				return base.IV;
			}
			set
			{
				base.IV = value;
			}
		}

		public override byte[] Key
		{
			get
			{
				return base.Key;
			}
			set
			{
				base.Key = value;
			}
		}

		public override int KeySize
		{
			get
			{
				return base.KeySize;
			}
			set
			{
				base.KeySize = value;
			}
		}

		public override int FeedbackSize
		{
			get
			{
				return base.FeedbackSize;
			}
			set
			{
				base.FeedbackSize = value;
			}
		}

		public override CipherMode Mode
		{
			get
			{
				return base.Mode;
			}
			set
			{
				if (value == CipherMode.CTS)
				{
					throw new CryptographicException("CTS is not supported");
				}
				base.Mode = value;
			}
		}

		public override PaddingMode Padding
		{
			get
			{
				return base.Padding;
			}
			set
			{
				base.Padding = value;
			}
		}

		public override ICryptoTransform CreateDecryptor()
		{
			return this.CreateDecryptor(this.Key, this.IV);
		}

		public override ICryptoTransform CreateEncryptor()
		{
			return this.CreateEncryptor(this.Key, this.IV);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
