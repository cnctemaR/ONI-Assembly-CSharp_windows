using System;

namespace Mono.Unix.Native
{
	[Flags]
	[Map]
	public enum EpollFlags
	{
		EPOLL_CLOEXEC = 2000000,
		EPOLL_NONBLOCK = 4000
	}
}
