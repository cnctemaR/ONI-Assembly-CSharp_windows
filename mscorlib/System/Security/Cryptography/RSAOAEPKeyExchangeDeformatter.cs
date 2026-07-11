using System;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class RSAOAEPKeyExchangeDeformatter : AsymmetricKeyExchangeDeformatter
	{
		public RSAOAEPKeyExchangeDeformatter()
		{
		}

		public RSAOAEPKeyExchangeDeformatter(AsymmetricAlgorithm key)
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

		public override byte[] DecryptKeyExchange(byte[] rgbData)
		{
			if (this.rsa == null)
			{
				string text = Locale.GetText("No RSA key specified");
				throw new CryptographicUnexpectedOperationException(text);
			}
			SHA1 sha = SHA1.Create();
			byte[] array = PKCS1.Decrypt_OAEP(this.rsa, sha, rgbData);
			if (array != null)
			{
				return array;
			}
			throw new CryptographicException(Locale.GetText("OAEP decoding error."));
		}

		public override void SetKey(AsymmetricAlgorithm key)
		{
			this.rsa = (RSA)key;
		}

		private RSA rsa;
	}
}
