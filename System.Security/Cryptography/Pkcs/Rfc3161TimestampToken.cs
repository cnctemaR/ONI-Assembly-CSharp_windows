using System;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Rfc3161TimestampToken
	{
		public Rfc3161TimestampTokenInfo TokenInfo
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		internal Rfc3161TimestampToken()
		{
			throw new PlatformNotSupportedException();
		}

		public SignedCms AsSignedCms()
		{
			throw new PlatformNotSupportedException();
		}

		public static bool TryDecode(ReadOnlyMemory<byte> encodedBytes, out Rfc3161TimestampToken token, out int bytesConsumed)
		{
			throw new PlatformNotSupportedException();
		}

		public bool VerifySignatureForData(ReadOnlySpan<byte> data, out X509Certificate2 signerCertificate, X509Certificate2Collection extraCandidates = null)
		{
			throw new PlatformNotSupportedException();
		}

		public bool VerifySignatureForHash(ReadOnlySpan<byte> hash, HashAlgorithmName hashAlgorithm, out X509Certificate2 signerCertificate, X509Certificate2Collection extraCandidates = null)
		{
			throw new PlatformNotSupportedException();
		}

		public bool VerifySignatureForHash(ReadOnlySpan<byte> hash, Oid hashAlgorithmId, out X509Certificate2 signerCertificate, X509Certificate2Collection extraCandidates = null)
		{
			throw new PlatformNotSupportedException();
		}

		public bool VerifySignatureForSignerInfo(SignerInfo signerInfo, out X509Certificate2 signerCertificate, X509Certificate2Collection extraCandidates = null)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
