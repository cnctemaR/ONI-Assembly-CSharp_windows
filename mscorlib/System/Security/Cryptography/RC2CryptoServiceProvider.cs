using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class RC2CryptoServiceProvider : RC2
	{
		[SecuritySafeCritical]
		public RC2CryptoServiceProvider()
		{
			if (CryptoConfig.AllowOnlyFipsAlgorithms)
			{
				throw new InvalidOperationException(Environment.GetResourceString("This implementation is not part of the Windows Platform FIPS validated cryptographic algorithms."));
			}
			if (!Utils.HasAlgorithm(26114, 0))
			{
				throw new CryptographicException(Environment.GetResourceString("Cryptographic service provider (CSP) could not be found for this algorithm."));
			}
			this.LegalKeySizesValue = RC2CryptoServiceProvider.s_legalKeySizes;
			this.FeedbackSizeValue = 8;
		}

		public override int EffectiveKeySize
		{
			get
			{
				return this.KeySizeValue;
			}
			set
			{
				if (value != this.KeySizeValue)
				{
					throw new CryptographicUnexpectedOperationException(Environment.GetResourceString("EffectiveKeySize must be the same as KeySize in this implementation."));
				}
			}
		}

		[ComVisible(false)]
		public bool UseSalt
		{
			get
			{
				return this.m_use40bitSalt;
			}
			set
			{
				this.m_use40bitSalt = value;
			}
		}

		[SecuritySafeCritical]
		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			if (this.m_use40bitSalt)
			{
				throw new NotImplementedException("UseSalt=true is not implemented on Mono yet");
			}
			return new RC2Transform(this, true, rgbKey, rgbIV);
		}

		[SecuritySafeCritical]
		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			if (this.m_use40bitSalt)
			{
				throw new NotImplementedException("UseSalt=true is not implemented on Mono yet");
			}
			return new RC2Transform(this, false, rgbKey, rgbIV);
		}

		public override void GenerateKey()
		{
			this.KeyValue = new byte[this.KeySizeValue / 8];
			Utils.StaticRandomNumberGenerator.GetBytes(this.KeyValue);
		}

		public override void GenerateIV()
		{
			this.IVValue = new byte[8];
			Utils.StaticRandomNumberGenerator.GetBytes(this.IVValue);
		}

		private bool m_use40bitSalt;

		private static KeySizes[] s_legalKeySizes = new KeySizes[]
		{
			new KeySizes(40, 128, 8)
		};
	}
}
