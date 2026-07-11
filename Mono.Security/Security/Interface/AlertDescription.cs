using System;

namespace Mono.Security.Interface
{
	public enum AlertDescription : byte
	{
		CloseNotify,
		UnexpectedMessage = 10,
		BadRecordMAC = 20,
		DecryptionFailed_RESERVED,
		RecordOverflow,
		DecompressionFailure = 30,
		HandshakeFailure = 40,
		NoCertificate_RESERVED,
		BadCertificate,
		UnsupportedCertificate,
		CertificateRevoked,
		CertificateExpired,
		CertificateUnknown,
		IlegalParameter,
		UnknownCA,
		AccessDenied,
		DecodeError,
		DecryptError,
		ExportRestriction = 60,
		ProtocolVersion = 70,
		InsuficientSecurity,
		InternalError = 80,
		UserCancelled = 90,
		NoRenegotiation = 100,
		UnsupportedExtension = 110
	}
}
