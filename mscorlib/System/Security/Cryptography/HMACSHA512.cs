using System;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class HMACSHA512 : HMAC
	{
		public HMACSHA512()
			: this(KeyBuilder.Key(8))
		{
			this.ProduceLegacyHmacValues = HMACSHA512.legacy_mode;
		}

		public HMACSHA512(byte[] key)
		{
			this.ProduceLegacyHmacValues = HMACSHA512.legacy_mode;
			base.HashName = "SHA512";
			this.HashSizeValue = 512;
			this.Key = key;
		}

		public bool ProduceLegacyHmacValues
		{
			get
			{
				return this.legacy;
			}
			set
			{
				this.legacy = value;
				base.BlockSizeValue = ((!this.legacy) ? 128 : 64);
			}
		}

		private static bool legacy_mode = Environment.GetEnvironmentVariable("legacyHMACMode") == "1";

		private bool legacy;
	}
}
