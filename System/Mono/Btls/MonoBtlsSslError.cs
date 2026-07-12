using System;

namespace Mono.Btls
{
	internal enum MonoBtlsSslError
	{
		None,
		Ssl,
		WantRead,
		WantWrite,
		WantX509Lookup,
		Syscall,
		ZeroReturn,
		WantConnect,
		WantAccept,
		WantChannelIdLookup,
		PendingSession = 11,
		PendingCertificate,
		WantPrivateKeyOperation
	}
}
