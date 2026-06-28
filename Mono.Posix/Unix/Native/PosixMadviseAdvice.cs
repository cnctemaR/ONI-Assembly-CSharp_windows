using System;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public enum PosixMadviseAdvice
	{
		POSIX_MADV_NORMAL,
		POSIX_MADV_RANDOM,
		POSIX_MADV_SEQUENTIAL,
		POSIX_MADV_WILLNEED,
		POSIX_MADV_DONTNEED
	}
}
