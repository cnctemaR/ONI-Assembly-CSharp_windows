using System;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs12SecretBag : Pkcs12SafeBag
	{
		public ReadOnlyMemory<byte> SecretValue
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		internal Pkcs12SecretBag()
			: base(null, default(ReadOnlyMemory<byte>), false)
		{
			throw new PlatformNotSupportedException();
		}

		public Oid GetSecretType()
		{
			throw new PlatformNotSupportedException();
		}
	}
}
