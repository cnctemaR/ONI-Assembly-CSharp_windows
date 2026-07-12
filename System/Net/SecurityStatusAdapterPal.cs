using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Net
{
	internal static class SecurityStatusAdapterPal
	{
		internal static SecurityStatusPal GetSecurityStatusPalFromNativeInt(int win32SecurityStatus)
		{
			return SecurityStatusAdapterPal.GetSecurityStatusPalFromInterop((global::Interop.SECURITY_STATUS)win32SecurityStatus, false);
		}

		internal static SecurityStatusPal GetSecurityStatusPalFromInterop(global::Interop.SECURITY_STATUS win32SecurityStatus, bool attachException = false)
		{
			SecurityStatusPalErrorCode securityStatusPalErrorCode;
			if (!SecurityStatusAdapterPal.s_statusDictionary.TryGetForward(win32SecurityStatus, out securityStatusPalErrorCode))
			{
				throw new InternalException();
			}
			if (attachException)
			{
				return new SecurityStatusPal(securityStatusPalErrorCode, new Win32Exception((int)win32SecurityStatus));
			}
			return new SecurityStatusPal(securityStatusPalErrorCode, null);
		}

		internal static global::Interop.SECURITY_STATUS GetInteropFromSecurityStatusPal(SecurityStatusPal status)
		{
			global::Interop.SECURITY_STATUS security_STATUS;
			if (!SecurityStatusAdapterPal.s_statusDictionary.TryGetBackward(status.ErrorCode, out security_STATUS))
			{
				throw new InternalException();
			}
			return security_STATUS;
		}

		private const int StatusDictionarySize = 41;

		private static readonly BidirectionalDictionary<global::Interop.SECURITY_STATUS, SecurityStatusPalErrorCode> s_statusDictionary = new BidirectionalDictionary<global::Interop.SECURITY_STATUS, SecurityStatusPalErrorCode>(41)
		{
			{
				global::Interop.SECURITY_STATUS.AlgorithmMismatch,
				SecurityStatusPalErrorCode.AlgorithmMismatch
			},
			{
				global::Interop.SECURITY_STATUS.ApplicationProtocolMismatch,
				SecurityStatusPalErrorCode.ApplicationProtocolMismatch
			},
			{
				global::Interop.SECURITY_STATUS.BadBinding,
				SecurityStatusPalErrorCode.BadBinding
			},
			{
				global::Interop.SECURITY_STATUS.BufferNotEnough,
				SecurityStatusPalErrorCode.BufferNotEnough
			},
			{
				global::Interop.SECURITY_STATUS.CannotInstall,
				SecurityStatusPalErrorCode.CannotInstall
			},
			{
				global::Interop.SECURITY_STATUS.CannotPack,
				SecurityStatusPalErrorCode.CannotPack
			},
			{
				global::Interop.SECURITY_STATUS.CertExpired,
				SecurityStatusPalErrorCode.CertExpired
			},
			{
				global::Interop.SECURITY_STATUS.CertUnknown,
				SecurityStatusPalErrorCode.CertUnknown
			},
			{
				global::Interop.SECURITY_STATUS.CompAndContinue,
				SecurityStatusPalErrorCode.CompAndContinue
			},
			{
				global::Interop.SECURITY_STATUS.CompleteNeeded,
				SecurityStatusPalErrorCode.CompleteNeeded
			},
			{
				global::Interop.SECURITY_STATUS.ContextExpired,
				SecurityStatusPalErrorCode.ContextExpired
			},
			{
				global::Interop.SECURITY_STATUS.ContinueNeeded,
				SecurityStatusPalErrorCode.ContinueNeeded
			},
			{
				global::Interop.SECURITY_STATUS.CredentialsNeeded,
				SecurityStatusPalErrorCode.CredentialsNeeded
			},
			{
				global::Interop.SECURITY_STATUS.DowngradeDetected,
				SecurityStatusPalErrorCode.DowngradeDetected
			},
			{
				global::Interop.SECURITY_STATUS.IllegalMessage,
				SecurityStatusPalErrorCode.IllegalMessage
			},
			{
				global::Interop.SECURITY_STATUS.IncompleteCredentials,
				SecurityStatusPalErrorCode.IncompleteCredentials
			},
			{
				global::Interop.SECURITY_STATUS.IncompleteMessage,
				SecurityStatusPalErrorCode.IncompleteMessage
			},
			{
				global::Interop.SECURITY_STATUS.InternalError,
				SecurityStatusPalErrorCode.InternalError
			},
			{
				global::Interop.SECURITY_STATUS.InvalidHandle,
				SecurityStatusPalErrorCode.InvalidHandle
			},
			{
				global::Interop.SECURITY_STATUS.InvalidToken,
				SecurityStatusPalErrorCode.InvalidToken
			},
			{
				global::Interop.SECURITY_STATUS.LogonDenied,
				SecurityStatusPalErrorCode.LogonDenied
			},
			{
				global::Interop.SECURITY_STATUS.MessageAltered,
				SecurityStatusPalErrorCode.MessageAltered
			},
			{
				global::Interop.SECURITY_STATUS.NoAuthenticatingAuthority,
				SecurityStatusPalErrorCode.NoAuthenticatingAuthority
			},
			{
				global::Interop.SECURITY_STATUS.NoImpersonation,
				SecurityStatusPalErrorCode.NoImpersonation
			},
			{
				global::Interop.SECURITY_STATUS.NoCredentials,
				SecurityStatusPalErrorCode.NoCredentials
			},
			{
				global::Interop.SECURITY_STATUS.NotOwner,
				SecurityStatusPalErrorCode.NotOwner
			},
			{
				global::Interop.SECURITY_STATUS.OK,
				SecurityStatusPalErrorCode.OK
			},
			{
				global::Interop.SECURITY_STATUS.OutOfMemory,
				SecurityStatusPalErrorCode.OutOfMemory
			},
			{
				global::Interop.SECURITY_STATUS.OutOfSequence,
				SecurityStatusPalErrorCode.OutOfSequence
			},
			{
				global::Interop.SECURITY_STATUS.PackageNotFound,
				SecurityStatusPalErrorCode.PackageNotFound
			},
			{
				global::Interop.SECURITY_STATUS.QopNotSupported,
				SecurityStatusPalErrorCode.QopNotSupported
			},
			{
				global::Interop.SECURITY_STATUS.Renegotiate,
				SecurityStatusPalErrorCode.Renegotiate
			},
			{
				global::Interop.SECURITY_STATUS.SecurityQosFailed,
				SecurityStatusPalErrorCode.SecurityQosFailed
			},
			{
				global::Interop.SECURITY_STATUS.SmartcardLogonRequired,
				SecurityStatusPalErrorCode.SmartcardLogonRequired
			},
			{
				global::Interop.SECURITY_STATUS.TargetUnknown,
				SecurityStatusPalErrorCode.TargetUnknown
			},
			{
				global::Interop.SECURITY_STATUS.TimeSkew,
				SecurityStatusPalErrorCode.TimeSkew
			},
			{
				global::Interop.SECURITY_STATUS.UnknownCredentials,
				SecurityStatusPalErrorCode.UnknownCredentials
			},
			{
				global::Interop.SECURITY_STATUS.UnsupportedPreauth,
				SecurityStatusPalErrorCode.UnsupportedPreauth
			},
			{
				global::Interop.SECURITY_STATUS.Unsupported,
				SecurityStatusPalErrorCode.Unsupported
			},
			{
				global::Interop.SECURITY_STATUS.UntrustedRoot,
				SecurityStatusPalErrorCode.UntrustedRoot
			},
			{
				global::Interop.SECURITY_STATUS.WrongPrincipal,
				SecurityStatusPalErrorCode.WrongPrincipal
			}
		};
	}
}
