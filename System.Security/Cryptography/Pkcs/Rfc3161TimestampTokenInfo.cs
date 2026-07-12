using System;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Rfc3161TimestampTokenInfo
	{
		public long? AccuracyInMicroseconds
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public bool HasExtensions
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public Oid HashAlgorithmId
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public bool IsOrdering
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public Oid PolicyId
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public DateTimeOffset Timestamp
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public int Version
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public Rfc3161TimestampTokenInfo(Oid policyId, Oid hashAlgorithmId, ReadOnlyMemory<byte> messageHash, ReadOnlyMemory<byte> serialNumber, DateTimeOffset timestamp, long? accuracyInMicroseconds = null, bool isOrdering = false, ReadOnlyMemory<byte>? nonce = null, ReadOnlyMemory<byte>? timestampAuthorityName = null, X509ExtensionCollection extensions = null)
		{
			throw new PlatformNotSupportedException();
		}

		public byte[] Encode()
		{
			throw new PlatformNotSupportedException();
		}

		public X509ExtensionCollection GetExtensions()
		{
			throw new PlatformNotSupportedException();
		}

		public ReadOnlyMemory<byte> GetMessageHash()
		{
			throw new PlatformNotSupportedException();
		}

		public ReadOnlyMemory<byte>? GetNonce()
		{
			throw new PlatformNotSupportedException();
		}

		public ReadOnlyMemory<byte> GetSerialNumber()
		{
			throw new PlatformNotSupportedException();
		}

		public ReadOnlyMemory<byte>? GetTimestampAuthorityName()
		{
			throw new PlatformNotSupportedException();
		}

		public static bool TryDecode(ReadOnlyMemory<byte> encodedBytes, out Rfc3161TimestampTokenInfo timestampTokenInfo, out int bytesConsumed)
		{
			throw new PlatformNotSupportedException();
		}

		public bool TryEncode(Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
