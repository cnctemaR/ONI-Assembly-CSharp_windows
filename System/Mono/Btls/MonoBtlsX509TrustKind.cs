using System;

namespace Mono.Btls
{
	[Flags]
	internal enum MonoBtlsX509TrustKind
	{
		DEFAULT = 0,
		TRUST_CLIENT = 1,
		TRUST_SERVER = 2,
		TRUST_ALL = 4,
		REJECT_CLIENT = 32,
		REJECT_SERVER = 64,
		REJECT_ALL = 128
	}
}
