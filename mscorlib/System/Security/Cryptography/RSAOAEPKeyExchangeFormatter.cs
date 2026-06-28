using System;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class RSAOAEPKeyExchangeFormatter : AsymmetricKeyExchangeFormatter
	{
		public RSAOAEPKeyExchangeFormatter()
		{
			this.rsa = null;
		}

		public RSAOAEPKeyExchangeFormatter(AsymmetricAlgorithm key)
		{
			this.SetKey(key);
		}

		public byte[] Parameter
		{
			get
			{
				return this.param;
			}
			set
			{
				this.param = value;
			}
		}

		public override string Parameters
		{
			get
			{
				return null;
			}
		}

		public RandomNumberGenerator Rng
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

		public override byte[] CreateKeyExchange(byte[] rgbData)
		{
			if (this.random == null)
			{
				this.random = RandomNumberGenerator.Create();
			}
			if (this.rsa == null)
			{
				string text = Locale.GetText("No RSA key specified");
				throw new CryptographicUnexpectedOperationException(text);
			}
			SHA1 sha = SHA1.Create();
			return PKCS1.Encrypt_OAEP(this.rsa, sha, this.random, rgbData);
		}

		public override byte[] CreateKeyExchange(byte[] rgbData, Type symAlgType)
		{
			return this.CreateKeyExchange(rgbData);
		}

		public override void SetKey(AsymmetricAlgorithm key)
		{
			this.rsa = (RSA)key;
		}

		private RSA rsa;

		private RandomNumberGenerator random;

		private byte[] param;
	}
}
