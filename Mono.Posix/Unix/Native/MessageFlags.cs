using System;

namespace Mono.Unix.Native
{
	[Flags]
	[Map]
	[CLSCompliant(false)]
	public enum MessageFlags
	{
		MSG_OOB = 1,
		MSG_PEEK = 2,
		MSG_DONTROUTE = 4,
		MSG_CTRUNC = 8,
		MSG_PROXY = 16,
		MSG_TRUNC = 32,
		MSG_DONTWAIT = 64,
		MSG_EOR = 128,
		MSG_WAITALL = 256,
		MSG_FIN = 512,
		MSG_SYN = 1024,
		MSG_CONFIRM = 2048,
		MSG_RST = 4096,
		MSG_ERRQUEUE = 8192,
		MSG_NOSIGNAL = 16384,
		MSG_MORE = 32768,
		MSG_WAITFORONE = 65536,
		MSG_FASTOPEN = 536870912,
		MSG_CMSG_CLOEXEC = 1073741824
	}
}
