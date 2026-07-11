using System;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public enum UnixSocketType
	{
		SOCK_STREAM = 1,
		SOCK_DGRAM,
		SOCK_RAW,
		SOCK_RDM,
		SOCK_SEQPACKET,
		SOCK_DCCP,
		SOCK_PACKET = 10
	}
}
