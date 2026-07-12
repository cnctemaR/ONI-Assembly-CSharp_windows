using System;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Rfc3161TimestampRequest
	{
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

		public Oid RequestedPolicyId
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public bool RequestSignerCertificate
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

		internal Rfc3161TimestampRequest()
		{
			throw new PlatformNotSupportedException();
		}

		public static Rfc3161TimestampRequest CreateFromData(ReadOnlySpan<byte> data, HashAlgorithmName hashAlgorithm, Oid requestedPolicyId = null, ReadOnlyMemory<byte>? nonce = null, bool requestSignerCertificates = false, X509ExtensionCollection extensions = null)
		{
			throw new PlatformNotSupportedException();
		}

		public static Rfc3161TimestampRequest CreateFromHash(ReadOnlyMemory<byte> hash, HashAlgorithmName hashAlgorithm, Oid requestedPolicyId = null, ReadOnlyMemory<byte>? nonce = null, bool requestSignerCertificates = false, X509ExtensionCollection extensions = null)
		{
			throw new PlatformNotSupportedException();
		}

		public static Rfc3161TimestampRequest CreateFromHash(ReadOnlyMemory<byte> hash, Oid hashAlgorithmId, Oid requestedPolicyId = null, ReadOnlyMemory<byte>? nonce = null, bool requestSignerCertificates = false, X509ExtensionCollection extensions = null)
		{
			throw new PlatformNotSupportedException();
		}

		public static Rfc3161TimestampRequest CreateFromSignerInfo(SignerInfo signerInfo, HashAlgorithmName hashAlgorithm, Oid requestedPolicyId = null, ReadOnlyMemory<byte>? nonce = null, bool requestSignerCertificates = false, X509ExtensionCollection extensions = null)
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

		public Rfc3161TimestampToken ProcessResponse(ReadOnlyMemory<byte> responseBytes, out int bytesConsumed)
		{
			throw new PlatformNotSupportedException();
		}

		public static bool TryDecode(ReadOnlyMemory<byte> encodedBytes, out Rfc3161TimestampRequest request, out int bytesConsumed)
		{
			throw new PlatformNotSupportedException();
		}

		public bool TryEncode(Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
