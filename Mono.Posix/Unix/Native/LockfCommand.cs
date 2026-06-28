using System;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public enum LockfCommand
	{
		F_ULOCK,
		F_LOCK,
		F_TLOCK,
		F_TEST
	}
}
