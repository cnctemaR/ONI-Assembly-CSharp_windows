using System;

namespace Mono.AppleTls
{
	internal enum SslClientCertificateState
	{
		None,
		Requested,
		Sent,
		Rejected
	}
}
