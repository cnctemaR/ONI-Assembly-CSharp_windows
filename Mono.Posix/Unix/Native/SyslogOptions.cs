using System;

namespace Mono.Unix.Native
{
	[Flags]
	[Map]
	[CLSCompliant(false)]
	public enum SyslogOptions
	{
		LOG_PID = 1,
		LOG_CONS = 2,
		LOG_ODELAY = 4,
		LOG_NDELAY = 8,
		LOG_NOWAIT = 16,
		LOG_PERROR = 32
	}
}
