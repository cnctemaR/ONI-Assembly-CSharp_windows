using System;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs8PrivateKeyInfo
	{
		public Oid AlgorithmId
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public ReadOnlyMemory<byte>? AlgorithmParameters
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public CryptographicAttributeObjectCollection Attributes
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public ReadOnlyMemory<byte> PrivateKeyBytes
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public Pkcs8PrivateKeyInfo(Oid algorithmId, ReadOnlyMemory<byte>? algorithmParameters, ReadOnlyMemory<byte> privateKey, bool skipCopies = false)
		{
			throw new PlatformNotSupportedException();
		}

		public static Pkcs8PrivateKeyInfo Create(AsymmetricAlgorithm privateKey)
		{
			throw new PlatformNotSupportedException();
		}

		public static Pkcs8PrivateKeyInfo Decode(ReadOnlyMemory<byte> source, out int bytesRead, bool skipCopy = false)
		{
			throw new PlatformNotSupportedException();
		}

		public static Pkcs8PrivateKeyInfo DecryptAndDecode(ReadOnlySpan<byte> passwordBytes, ReadOnlyMemory<byte> source, out int bytesRead)
		{
			throw new PlatformNotSupportedException();
		}

		public static Pkcs8PrivateKeyInfo DecryptAndDecode(ReadOnlySpan<char> password, ReadOnlyMemory<byte> source, out int bytesRead)
		{
			throw new PlatformNotSupportedException();
		}

		public byte[] Encode()
		{
			throw new PlatformNotSupportedException();
		}

		public byte[] Encrypt(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
		{
			throw new PlatformNotSupportedException();
		}

		public byte[] Encrypt(ReadOnlySpan<char> password, PbeParameters pbeParameters)
		{
			throw new PlatformNotSupportedException();
		}

		public bool TryEncode(Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}

		public bool TryEncrypt(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters, Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}

		public bool TryEncrypt(ReadOnlySpan<char> password, PbeParameters pbeParameters, Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
