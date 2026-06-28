using System;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class PKCS1MaskGenerationMethod : MaskGenerationMethod
	{
		public PKCS1MaskGenerationMethod()
		{
			this.hashName = "SHA1";
		}

		public string HashName
		{
			get
			{
				return this.hashName;
			}
			set
			{
				this.hashName = ((value != null) ? value : "SHA1");
			}
		}

		public override byte[] GenerateMask(byte[] rgbSeed, int cbReturn)
		{
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create(this.hashName);
			return PKCS1.MGF1(hashAlgorithm, rgbSeed, cbReturn);
		}

		private string hashName;
	}
}
