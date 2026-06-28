using System;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class HMACSHA256 : HMAC
	{
		public HMACSHA256()
			: this(KeyBuilder.Key(8))
		{
		}

		public HMACSHA256(byte[] key)
		{
			base.HashName = "SHA256";
			this.HashSizeValue = 256;
			this.Key = key;
		}
	}
}
