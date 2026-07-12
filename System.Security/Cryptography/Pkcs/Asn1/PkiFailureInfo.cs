using System;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[Flags]
	internal enum PkiFailureInfo
	{
		None = 0,
		BadAlg = 1,
		BadMessageCheck = 2,
		BadRequest = 4,
		BadTime = 8,
		BadCertId = 16,
		BadDataFormat = 32,
		WrongAuthority = 64,
		IncorrectData = 128,
		MissingTimeStamp = 256,
		BadPop = 512,
		CertRevoked = 1024,
		CertConfirmed = 2048,
		WrongIntegrity = 4096,
		BadRecipientNonce = 8192,
		TimeNotAvailable = 16384,
		UnacceptedPolicy = 32768,
		UnacceptedExtension = 65536,
		AddInfoNotAvailable = 131072,
		BadSenderNonce = 262144,
		BadCertTemplate = 524288,
		SignerNotTrusted = 1048576,
		TransactionIdInUse = 2097152,
		UnsupportedVersion = 4194304,
		NotAuthorized = 8388608,
		SystemUnavail = 16777216,
		SystemFailure = 33554432,
		DuplicateCertReq = 67108864
	}
}
