using System;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public enum FcntlCommand
	{
		F_DUPFD,
		F_GETFD,
		F_SETFD,
		F_GETFL,
		F_SETFL,
		F_GETLK = 12,
		F_SETLK,
		F_SETLKW,
		F_SETOWN = 8,
		F_GETOWN,
		F_SETSIG,
		F_GETSIG,
		F_SETLEASE = 1024,
		F_GETLEASE,
		F_NOTIFY
	}
}
