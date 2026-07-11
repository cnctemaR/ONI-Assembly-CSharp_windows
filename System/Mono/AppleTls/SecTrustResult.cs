using System;

namespace Mono.AppleTls
{
	internal enum SecTrustResult
	{
		Invalid,
		Proceed,
		Confirm,
		Deny,
		Unspecified,
		RecoverableTrustFailure,
		FatalTrustFailure,
		ResultOtherError
	}
}
