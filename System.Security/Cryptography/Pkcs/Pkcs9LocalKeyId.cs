using System;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs9LocalKeyId : Pkcs9AttributeObject
	{
		public ReadOnlyMemory<byte> KeyId
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public Pkcs9LocalKeyId()
		{
			throw new PlatformNotSupportedException();
		}

		public Pkcs9LocalKeyId(byte[] keyId)
		{
			throw new PlatformNotSupportedException();
		}

		public Pkcs9LocalKeyId(ReadOnlySpan<byte> keyId)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
