using System;

namespace Mono.Unix.Native
{
	[Map]
	[Flags]
	[CLSCompliant(false)]
	public enum UnixSocketFlags
	{
		SOCK_CLOEXEC = 524288,
		SOCK_NONBLOCK = 2048
	}
}
