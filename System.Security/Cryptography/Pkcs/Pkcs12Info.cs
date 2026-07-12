using System;
using System.Collections.ObjectModel;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs12Info
	{
		public ReadOnlyCollection<Pkcs12SafeContents> AuthenticatedSafe
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public Pkcs12IntegrityMode IntegrityMode
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		internal Pkcs12Info()
		{
			throw new PlatformNotSupportedException();
		}

		public static Pkcs12Info Decode(ReadOnlyMemory<byte> encodedBytes, out int bytesConsumed, bool skipCopy = false)
		{
			throw new PlatformNotSupportedException();
		}

		public bool VerifyMac(ReadOnlySpan<char> password)
		{
			throw new PlatformNotSupportedException();
		}

		public bool VerifyMac(string password)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
