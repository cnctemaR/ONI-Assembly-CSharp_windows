using System;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs12KeyBag : Pkcs12SafeBag
	{
		public ReadOnlyMemory<byte> Pkcs8PrivateKey
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public Pkcs12KeyBag(ReadOnlyMemory<byte> pkcs8PrivateKey, bool skipCopy = false)
			: base(null, default(ReadOnlyMemory<byte>), false)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
