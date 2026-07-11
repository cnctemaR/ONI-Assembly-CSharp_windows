using System;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public enum ShutdownOption
	{
		SHUT_RD = 1,
		SHUT_WR,
		SHUT_RDWR
	}
}
