using System;

namespace System.Security.Cryptography.Pkcs
{
	public abstract class Pkcs12SafeBag
	{
		public CryptographicAttributeObjectCollection Attributes
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public ReadOnlyMemory<byte> EncodedBagValue
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		protected Pkcs12SafeBag(string bagIdValue, ReadOnlyMemory<byte> encodedBagValue, bool skipCopy = false)
		{
			throw new PlatformNotSupportedException();
		}

		public byte[] Encode()
		{
			throw new PlatformNotSupportedException();
		}

		public Oid GetBagId()
		{
			throw new PlatformNotSupportedException();
		}

		public bool TryEncode(Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
