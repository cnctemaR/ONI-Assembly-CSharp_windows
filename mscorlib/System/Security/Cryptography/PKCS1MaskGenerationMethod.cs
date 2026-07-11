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
			this.HashNameValue = "SHA1";
		}

		public string HashName
		{
			get
			{
				return this.HashNameValue;
			}
			set
			{
				this.HashNameValue = value;
				if (this.HashNameValue == null)
				{
					this.HashNameValue = "SHA1";
				}
			}
		}

		public override byte[] GenerateMask(byte[] rgbSeed, int cbReturn)
		{
			return PKCS1.MGF1(HashAlgorithm.Create(this.HashNameValue), rgbSeed, cbReturn);
		}

		private string HashNameValue;
	}
}
