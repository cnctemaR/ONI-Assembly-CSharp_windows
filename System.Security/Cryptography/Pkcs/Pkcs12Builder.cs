using System;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs12Builder
	{
		public bool IsSealed
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public void AddSafeContentsEncrypted(Pkcs12SafeContents safeContents, byte[] passwordBytes, PbeParameters pbeParameters)
		{
			throw new PlatformNotSupportedException();
		}

		public void AddSafeContentsEncrypted(Pkcs12SafeContents safeContents, ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
		{
			throw new PlatformNotSupportedException();
		}

		public void AddSafeContentsEncrypted(Pkcs12SafeContents safeContents, ReadOnlySpan<char> password, PbeParameters pbeParameters)
		{
			throw new PlatformNotSupportedException();
		}

		public void AddSafeContentsEncrypted(Pkcs12SafeContents safeContents, string password, PbeParameters pbeParameters)
		{
			throw new PlatformNotSupportedException();
		}

		public void AddSafeContentsUnencrypted(Pkcs12SafeContents safeContents)
		{
			throw new PlatformNotSupportedException();
		}

		public byte[] Encode()
		{
			throw new PlatformNotSupportedException();
		}

		public void SealWithMac(ReadOnlySpan<char> password, HashAlgorithmName hashAlgorithm, int iterationCount)
		{
			throw new PlatformNotSupportedException();
		}

		public void SealWithMac(string password, HashAlgorithmName hashAlgorithm, int iterationCount)
		{
			throw new PlatformNotSupportedException();
		}

		public void SealWithoutIntegrity()
		{
			throw new PlatformNotSupportedException();
		}

		public bool TryEncode(Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
