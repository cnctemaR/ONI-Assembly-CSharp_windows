using System;

namespace Mono.Unix.Native
{
	[Map]
	internal enum SockaddrType
	{
		Invalid,
		SockaddrStorage,
		SockaddrUn,
		Sockaddr,
		SockaddrIn,
		SockaddrIn6,
		MustBeWrapped = 32768
	}
}
