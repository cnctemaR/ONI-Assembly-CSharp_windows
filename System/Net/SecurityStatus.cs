using System;

namespace System.Net
{
	internal enum SecurityStatus
	{
		OK,
		ContinueNeeded = 590610,
		CompleteNeeded,
		CompAndContinue,
		ContextExpired = 590615,
		CredentialsNeeded = 590624,
		Renegotiate,
		OutOfMemory = -2146893056,
		InvalidHandle,
		Unsupported,
		TargetUnknown,
		InternalError,
		PackageNotFound,
		NotOwner,
		CannotInstall,
		InvalidToken,
		CannotPack,
		QopNotSupported,
		NoImpersonation,
		LogonDenied,
		UnknownCredentials,
		NoCredentials,
		MessageAltered,
		OutOfSequence,
		NoAuthenticatingAuthority,
		IncompleteMessage = -2146893032,
		IncompleteCredentials = -2146893024,
		BufferNotEnough,
		WrongPrincipal,
		TimeSkew = -2146893020,
		UntrustedRoot,
		IllegalMessage,
		CertUnknown,
		CertExpired,
		AlgorithmMismatch = -2146893007,
		SecurityQosFailed,
		SmartcardLogonRequired = -2146892994,
		UnsupportedPreauth = -2146892989,
		BadBinding = -2146892986
	}
}
