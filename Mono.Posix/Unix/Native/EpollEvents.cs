using System;

namespace Mono.Unix.Native
{
	[Flags]
	[Map]
	[CLSCompliant(false)]
	public enum EpollEvents : uint
	{
		EPOLLIN = 1U,
		EPOLLPRI = 2U,
		EPOLLOUT = 4U,
		EPOLLRDNORM = 64U,
		EPOLLRDBAND = 128U,
		EPOLLWRNORM = 256U,
		EPOLLWRBAND = 512U,
		EPOLLMSG = 1024U,
		EPOLLERR = 8U,
		EPOLLHUP = 16U,
		EPOLLRDHUP = 8192U,
		EPOLLONESHOT = 1073741824U,
		EPOLLET = 2147483648U
	}
}
