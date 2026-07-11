using System;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class RSAPKCS1KeyExchangeDeformatter : AsymmetricKeyExchangeDeformatter
	{
		public RSAPKCS1KeyExchangeDeformatter()
		{
		}

		public RSAPKCS1KeyExchangeDeformatter(AsymmetricAlgorithm key)
		{
			this.SetKey(key);
		}

		public override string Parameters
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public RandomNumberGenerator RNG
		{
			get
			{
				return this.random;
			}
			set
			{
				this.random = value;
			}
		}

		public override byte[] DecryptKeyExchange(byte[] rgbIn)
		{
			if (this.rsa == null)
			{
				throw new CryptographicUnexpectedOperationException(Locale.GetText("No key pair available."));
			}
			byte[] array = PKCS1.Decrypt_v15(this.rsa, rgbIn);
			if (array != null)
			{
				return array;
			}
			throw new CryptographicException(Locale.GetText("PKCS1 decoding error."));
		}

		public override void SetKey(AsymmetricAlgorithm key)
		{
			this.rsa = (RSA)key;
		}

		private RSA rsa;

		private RandomNumberGenerator random;
	}
}
