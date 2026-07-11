using System;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class DESCryptoServiceProvider : DES
	{
		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new DESTransform(this, false, rgbKey, rgbIV);
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new DESTransform(this, true, rgbKey, rgbIV);
		}

		public override void GenerateIV()
		{
			this.IVValue = KeyBuilder.IV(DESTransform.BLOCK_BYTE_SIZE);
		}

		public override void GenerateKey()
		{
			this.KeyValue = DESTransform.GetStrongKey();
		}
	}
}
