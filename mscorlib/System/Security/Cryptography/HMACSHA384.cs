using System;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class HMACSHA384 : HMAC
	{
		public HMACSHA384()
			: this(KeyBuilder.Key(8))
		{
			this.ProduceLegacyHmacValues = HMACSHA384.legacy_mode;
		}

		public HMACSHA384(byte[] key)
		{
			this.ProduceLegacyHmacValues = HMACSHA384.legacy_mode;
			base.HashName = "SHA384";
			this.HashSizeValue = 384;
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
