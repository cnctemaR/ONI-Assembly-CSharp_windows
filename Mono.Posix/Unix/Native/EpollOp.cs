using System;

namespace Mono.Unix.Native
{
	public enum EpollOp
	{
		EPOLL_CTL_ADD = 1,
		EPOLL_CTL_DEL,
		EPOLL_CTL_MOD
	}
}
